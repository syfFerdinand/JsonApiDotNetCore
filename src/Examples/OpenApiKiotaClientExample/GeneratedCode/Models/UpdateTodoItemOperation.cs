// <auto-generated/>
#nullable enable
#pragma warning disable CS8625
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace OpenApiKiotaClientExample.GeneratedCode.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class UpdateTodoItemOperation : global::OpenApiKiotaClientExample.GeneratedCode.Models.AtomicOperation, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The data property</summary>
        public global::OpenApiKiotaClientExample.GeneratedCode.Models.DataInUpdateTodoItemRequest? Data
        {
            get { return BackingStore?.Get<global::OpenApiKiotaClientExample.GeneratedCode.Models.DataInUpdateTodoItemRequest?>("data"); }
            set { BackingStore?.Set("data", value); }
        }

        /// <summary>The op property</summary>
        public global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateOperationCode? Op
        {
            get { return BackingStore?.Get<global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateOperationCode?>("op"); }
            set { BackingStore?.Set("op", value); }
        }

        /// <summary>The ref property</summary>
        public global::OpenApiKiotaClientExample.GeneratedCode.Models.TodoItemIdentifierInRequest? Ref
        {
            get { return BackingStore?.Get<global::OpenApiKiotaClientExample.GeneratedCode.Models.TodoItemIdentifierInRequest?>("ref"); }
            set { BackingStore?.Set("ref", value); }
        }

        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateTodoItemOperation"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static new global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateTodoItemOperation CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateTodoItemOperation();
        }

        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public override IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>(base.GetFieldDeserializers())
            {
                { "data", n => { Data = n.GetObjectValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.DataInUpdateTodoItemRequest>(global::OpenApiKiotaClientExample.GeneratedCode.Models.DataInUpdateTodoItemRequest.CreateFromDiscriminatorValue); } },
                { "op", n => { Op = n.GetEnumValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateOperationCode>(); } },
                { "ref", n => { Ref = n.GetObjectValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.TodoItemIdentifierInRequest>(global::OpenApiKiotaClientExample.GeneratedCode.Models.TodoItemIdentifierInRequest.CreateFromDiscriminatorValue); } },
            };
        }

        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public override void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            base.Serialize(writer);
            writer.WriteObjectValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.DataInUpdateTodoItemRequest>("data", Data);
            writer.WriteEnumValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.UpdateOperationCode>("op", Op);
            writer.WriteObjectValue<global::OpenApiKiotaClientExample.GeneratedCode.Models.TodoItemIdentifierInRequest>("ref", Ref);
        }
    }
}
#pragma warning restore CS0618
