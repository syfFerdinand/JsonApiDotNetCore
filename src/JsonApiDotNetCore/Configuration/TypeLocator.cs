using System.Reflection;
using JsonApiDotNetCore.Resources;

namespace JsonApiDotNetCore.Configuration;

/// <summary>
/// Used to locate types and facilitate resource auto-discovery.
/// </summary>
internal sealed class TypeLocator
{
    // As a reminder, the following terminology is used for generic types:
    // non-generic          string
    // generic
    //     unbound          Dictionary<,>
    //     constructed
    //         open         Dictionary<TKey,TValue>
    //         closed       Dictionary<string,int>

    /// <summary>
    /// Attempts to lookup the ID type of the specified resource type. Returns <c>null</c> if it does not implement <see cref="IIdentifiable{TId}" />.
    /// </summary>
    public Type? LookupIdType(Type? resourceClrType)
    {
        Type? identifiableClosedInterface = resourceClrType?.GetInterfaces().FirstOrDefault(@interface =>
            @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IIdentifiable<>));

        return identifiableClosedInterface?.GetGenericArguments()[0];
    }

    /// <summary>
    /// Attempts to get a descriptor for the specified resource type.
    /// </summary>
    public ResourceDescriptor? ResolveResourceDescriptor(Type? type)
    {
        if (type != null && type.IsOrImplementsInterface<IIdentifiable>())
        {
            Type? idType = LookupIdType(type);

            if (idType != null)
            {
                return new ResourceDescriptor(type, idType);
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the implementation type with service interface (to be registered in the IoC container) for the specified unbound generic interface and its type
    /// arguments, by scanning for types in the specified assembly that match the signature.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to search for matching types.
    /// </param>
    /// <param name="unboundInterface">
    /// The unbound generic interface to match against.
    /// </param>
    /// <param name="interfaceTypeArguments">
    /// Generic type arguments to construct <paramref name="unboundInterface" />.
    /// </param>
    /// <example>
    /// <code><![CDATA[
    /// GetContainerRegistrationFromAssembly(assembly, typeof(IResourceService<,>), typeof(Article), typeof(Guid));
    /// ]]></code>
    /// </example>
    public (Type implementationType, Type serviceInterface)? GetContainerRegistrationFromAssembly(Assembly assembly, Type unboundInterface,
        params Type[] interfaceTypeArguments)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(unboundInterface);
        ArgumentNullException.ThrowIfNull(interfaceTypeArguments);

        if (!unboundInterface.IsInterface || !unboundInterface.IsGenericType || unboundInterface != unboundInterface.GetGenericTypeDefinition())
        {
            throw new ArgumentException($"Specified type '{unboundInterface.FullName}' is not an unbound generic interface.", nameof(unboundInterface));
        }

        if (interfaceTypeArguments.Length != unboundInterface.GetGenericArguments().Length)
        {
            throw new ArgumentException(
                $"Interface '{unboundInterface.FullName}' requires {unboundInterface.GetGenericArguments().Length} type arguments " +
                $"instead of {interfaceTypeArguments.Length}.", nameof(interfaceTypeArguments));
        }

        // @formatter:wrap_chained_method_calls chop_always
        // @formatter:wrap_before_first_method_call true

        return assembly
            .GetTypes()
            .Select(type => GetContainerRegistrationFromType(type, unboundInterface, interfaceTypeArguments))
            .FirstOrDefault(result => result != null);

        // @formatter:wrap_before_first_method_call restore
        // @formatter:wrap_chained_method_calls restore
    }

    private static (Type implementationType, Type serviceInterface)? GetContainerRegistrationFromType(Type nextType, Type unboundInterface,
        Type[] interfaceTypeArguments)
    {
        if (nextType is { IsNested: false, IsAbstract: false, IsInterface: false })
        {
            foreach (Type nextConstructedInterface in nextType.GetInterfaces().Where(type => type.IsGenericType))
            {
                Type nextUnboundInterface = nextConstructedInterface.GetGenericTypeDefinition();

                if (nextUnboundInterface == unboundInterface)
                {
                    Type[] nextTypeArguments = nextConstructedInterface.GetGenericArguments();

                    if (nextTypeArguments.Length == interfaceTypeArguments.Length && nextTypeArguments.SequenceEqual(interfaceTypeArguments))
                    {
                        return (nextType, nextUnboundInterface.MakeGenericType(interfaceTypeArguments));
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all derivatives of the specified type.
    /// </summary>
    /// <param name="assembly">
    /// The assembly to search.
    /// </param>
    /// <param name="baseType">
    /// The inherited type.
    /// </param>
    /// <example>
    /// <code><![CDATA[
    /// GetDerivedTypes(assembly, typeof(DbContext))
    /// ]]></code>
    /// </example>
    public IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type baseType)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(baseType);

        foreach (Type type in assembly.GetTypes())
        {
            if (baseType.IsAssignableFrom(type))
            {
                yield return type;
            }
        }
    }
}
