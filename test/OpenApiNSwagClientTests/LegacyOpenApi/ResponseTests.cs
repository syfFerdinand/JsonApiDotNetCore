using System.Globalization;
using System.Net;
using FluentAssertions;
using JsonApiDotNetCore.OpenApi.Client.NSwag;
using OpenApiNSwagClientTests.LegacyOpenApi.GeneratedCode;
using Xunit;

namespace OpenApiNSwagClientTests.LegacyOpenApi;

public sealed class ResponseTests
{
    private const string HostPrefix = "http://localhost/api/";

    [Fact]
    public async Task Getting_resource_collection_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string flightDestination = "Amsterdam";
        const string flightServiceOnBoard = "Movies";
        const string flightDepartsAt = "2014-11-25T00:00:00";
        const string documentMetaValue = "1";
        const string flightMetaValue = "https://api.jsonapi.net/docs/#get-flights";
        const string purserMetaValue = "https://api.jsonapi.net/docs/#get-flight-purser";
        const string cabinCrewMembersMetaValue = "https://api.jsonapi.net/docs/#get-flight-cabin-crew-members";
        const string passengersMetaValue = "https://api.jsonapi.net/docs/#get-flight-passengers";
        const string topLevelLink = $"{HostPrefix}flights";
        const string flightResourceLink = $"{topLevelLink}/{flightId}";

        const string responseBody = $$"""
            {
              "meta": {
                "total-resources": "{{documentMetaValue}}"
              },
              "links": {
                "self": "{{topLevelLink}}",
                "first": "{{topLevelLink}}",
                "last": "{{topLevelLink}}"
              },
              "data": [
                {
                  "type": "flights",
                  "id": "{{flightId}}",
                  "attributes": {
                    "final-destination": "{{flightDestination}}",
                    "stop-over-destination": null,
                    "operated-by": "DeltaAirLines",
                    "departs-at": "{{flightDepartsAt}}",
                    "arrives-at": null,
                    "services-on-board": [
                      "{{flightServiceOnBoard}}",
                      "",
                      null
                    ]
                  },
                  "relationships": {
                    "purser": {
                      "links": {
                        "self": "{{flightResourceLink}}/relationships/purser",
                        "related": "{{flightResourceLink}}/purser"
                      },
                      "meta": {
                         "docs": "{{purserMetaValue}}"
                      }
                    },
                    "cabin-crew-members": {
                      "links": {
                        "self": "{{flightResourceLink}}/relationships/cabin-crew-members",
                        "related": "{{flightResourceLink}}/cabin-crew-members"
                      },
                      "meta": {
                         "docs": "{{cabinCrewMembersMetaValue}}"
                      }
                    },
                    "passengers": {
                      "links": {
                        "self": "{{flightResourceLink}}/relationships/passengers",
                        "related": "{{flightResourceLink}}/passengers"
                      },
                      "meta": {
                         "docs": "{{passengersMetaValue}}"
                      }
                    }
                  },
                  "links": {
                    "self": "{{flightResourceLink}}"
                  },
                  "meta": {
                    "docs": "{{flightMetaValue}}"
                  }
                }
              ]
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        FlightCollectionResponseDocument response = await apiClient.GetFlightCollectionAsync(null, null);

        // Assert
        response.Jsonapi.Should().BeNull();
        response.Meta.Should().HaveCount(1);
        response.Meta["total-resources"].Should().Be(documentMetaValue);
        response.Links.Self.Should().Be(topLevelLink);
        response.Links.First.Should().Be(topLevelLink);
        response.Links.Last.Should().Be(topLevelLink);
        response.Data.Should().HaveCount(1);

        DataInFlightResponse flight = response.Data.First();
        flight.Id.Should().Be(flightId);
        flight.Links.Self.Should().Be(flightResourceLink);
        flight.Meta.Should().HaveCount(1);
        flight.Meta["docs"].Should().Be(flightMetaValue);

        flight.Attributes.FinalDestination.Should().Be(flightDestination);
        flight.Attributes.StopOverDestination.Should().BeNull();
        flight.Attributes.ServicesOnBoard.Should().HaveCount(3);
        flight.Attributes.ServicesOnBoard.ElementAt(0).Should().Be(flightServiceOnBoard);
        flight.Attributes.ServicesOnBoard.ElementAt(1).Should().Be(string.Empty);
        flight.Attributes.ServicesOnBoard.ElementAt(2).Should().BeNull();
        flight.Attributes.OperatedBy.Should().Be(Airline.DeltaAirLines);
        flight.Attributes.DepartsAt.Should().Be(DateTimeOffset.Parse(flightDepartsAt, new CultureInfo("en-GB")));
        flight.Attributes.ArrivesAt.Should().BeNull();

        flight.Relationships.Purser.Data.Should().BeNull();
        flight.Relationships.Purser.Links.Self.Should().Be($"{flightResourceLink}/relationships/purser");
        flight.Relationships.Purser.Links.Related.Should().Be($"{flightResourceLink}/purser");
        flight.Relationships.Purser.Meta.Should().HaveCount(1);
        flight.Relationships.Purser.Meta["docs"].Should().Be(purserMetaValue);

        flight.Relationships.CabinCrewMembers.Data.Should().BeNull();
        flight.Relationships.CabinCrewMembers.Links.Self.Should().Be($"{flightResourceLink}/relationships/cabin-crew-members");
        flight.Relationships.CabinCrewMembers.Links.Related.Should().Be($"{flightResourceLink}/cabin-crew-members");
        flight.Relationships.CabinCrewMembers.Meta.Should().HaveCount(1);
        flight.Relationships.CabinCrewMembers.Meta["docs"].Should().Be(cabinCrewMembersMetaValue);

        flight.Relationships.Passengers.Data.Should().BeNull();
        flight.Relationships.Passengers.Links.Self.Should().Be($"{flightResourceLink}/relationships/passengers");
        flight.Relationships.Passengers.Links.Related.Should().Be($"{flightResourceLink}/passengers");
        flight.Relationships.Passengers.Meta.Should().HaveCount(1);
        flight.Relationships.Passengers.Meta["docs"].Should().Be(passengersMetaValue);
    }

    [Fact]
    public async Task Getting_resource_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string departsAtInZuluTime = "2021-06-08T12:53:30.554Z";
        const string flightDestination = "Amsterdam";
        const string arrivesAtWithUtcOffset = "2019-02-20T11:56:33.0721266+01:00";
        const string flightServiceOnBoard = "Movies";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}&fields[flights]=departs-at,arrives-at"
              },
              "data": {
                  "type": "flights",
                  "id": "{{flightId}}",
                  "attributes": {
                    "departs-at": "{{departsAtInZuluTime}}",
                    "arrives-at": "{{arrivesAtWithUtcOffset}}",
                    "final-destination": "{{flightDestination}}",
                    "services-on-board": ["{{flightServiceOnBoard}}"]
                  },
                  "links": {
                    "self": "{{HostPrefix}}flights/{{flightId}}"
                  }
                }
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        PrimaryFlightResponseDocument response = await apiClient.GetFlightAsync(flightId, null, null);

        // Assert
        response.Jsonapi.Should().BeNull();
        response.Meta.Should().BeNull();
        response.Data.Meta.Should().BeNull();
        response.Data.Relationships.Should().BeNull();
        response.Data.Attributes.DepartsAt.Should().Be(DateTimeOffset.Parse(departsAtInZuluTime));
        response.Data.Attributes.ArrivesAt.Should().Be(DateTimeOffset.Parse(arrivesAtWithUtcOffset));
        response.Data.Attributes.ServicesOnBoard.Should().Contain(flightServiceOnBoard);
        response.Data.Attributes.FinalDestination.Should().Be(flightDestination);
        response.Data.Attributes.StopOverDestination.Should().BeNull();
        response.Data.Attributes.OperatedBy.Should().Be(default);
    }

    [Fact]
    public async Task Getting_unknown_resource_translates_error_response()
    {
        // Arrange
        const string flightId = "ZvuH1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "http://localhost/api/flights/ZvuH1",
                "describedby": "/swagger/v1/swagger.json"
              },
              "errors": [
                {
                  "id": "f1a520ac-02a0-466b-94ea-86cbaa86f02f",
                  "status": "404",
                  "title": "The requested resource does not exist.",
                  "detail": "Resource of type 'flights' with ID '{{flightId}}' does not exist."
                }
              ]
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NotFound, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        Func<Task> action = async () => await apiClient.GetFlightAsync(flightId, null, null);

        // Assert
        ApiException<ErrorResponseDocument> exception = (await action.Should().ThrowExactlyAsync<ApiException<ErrorResponseDocument>>()).Which;
        exception.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        exception.Result.Links.Should().NotBeNull();
        exception.Result.Links.Self.Should().Be("http://localhost/api/flights/ZvuH1");
        exception.Result.Links.Describedby.Should().Be("/swagger/v1/swagger.json");
        exception.Result.Errors.Should().HaveCount(1);

        ErrorObject error = exception.Result.Errors.ElementAt(0);
        error.Id.Should().Be("f1a520ac-02a0-466b-94ea-86cbaa86f02f");
        error.Status.Should().Be("404");
        error.Title.Should().Be("The requested resource does not exist.");
        error.Detail.Should().Be($"Resource of type 'flights' with ID '{flightId}' does not exist.");
        error.Source.Should().BeNull();
    }

    [Fact]
    public async Task Posting_resource_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string flightAttendantId = "bBJHu";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}&fields[flights]&include=purser,cabin-crew-members,passengers"
              },
              "data": {
                  "type": "flights",
                  "id": "{{flightId}}",
                  "relationships": {
                    "purser": {
                      "links": {
                        "self": "{{HostPrefix}}flights/{{flightId}}/relationships/purser",
                        "related": "{{HostPrefix}}flights/{{flightId}}/purser"
                      },
                      "data": {
                          "type": "flight-attendants",
                          "id": "{{flightAttendantId}}"
                        }
                    },
                    "cabin-crew-members": {
                      "links": {
                        "self": "{{HostPrefix}}flights/{{flightId}}/relationships/cabin-crew-members",
                        "related": "{{HostPrefix}}flights/{{flightId}}/cabin-crew-members"
                      },
                      "data": [
                        {
                          "type": "flight-attendants",
                          "id": "{{flightAttendantId}}"
                        }
                      ],
                    },
                    "passengers": {
                      "links": {
                        "self": "{{HostPrefix}}flights/{{flightId}}/relationships/passengers",
                        "related": "{{HostPrefix}}flights/{{flightId}}/passengers"
                      },
                      "data": [ ]
                    }
                  },
                  "links": {
                    "self": "{{HostPrefix}}flights/{{flightId}}&fields[flights]&include=purser,cabin-crew-members,passengers"
                  }
                }
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.Created, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new CreateFlightRequestDocument
        {
            Data = new DataInCreateFlightRequest
            {
                Relationships = new RelationshipsInCreateFlightRequest
                {
                    Purser = new ToOneFlightAttendantInRequest
                    {
                        Data = new FlightAttendantIdentifierInRequest
                        {
                            Id = flightAttendantId
                        }
                    }
                }
            }
        };

        // Act
        PrimaryFlightResponseDocument response = await apiClient.PostFlightAsync(null, requestBody);

        // Assert
        response.Data.Attributes.Should().BeNull();
        response.Data.Relationships.Purser.Data.Should().NotBeNull();
        response.Data.Relationships.Purser.Data.Id.Should().Be(flightAttendantId);
        response.Data.Relationships.CabinCrewMembers.Data.Should().HaveCount(1);
        response.Data.Relationships.CabinCrewMembers.Data.First().Id.Should().Be(flightAttendantId);
        response.Data.Relationships.CabinCrewMembers.Data.First().Type.Should().Be(FlightAttendantResourceType.FlightAttendants);
        response.Data.Relationships.Passengers.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Patching_resource_with_side_effects_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}&fields[flights]"
              },
              "data": {
                  "type": "flights",
                  "id": "{{flightId}}",
                  "links": {
                    "self": "{{HostPrefix}}flights/{{flightId}}&fields[flights]&include=purser,cabin-crew-members,passengers"
                  }
                }
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new UpdateFlightRequestDocument
        {
            Data = new DataInUpdateFlightRequest
            {
                Id = flightId
            }
        };

        // Act
        PrimaryFlightResponseDocument response = await apiClient.PatchFlightAsync(flightId, null, requestBody);

        // Assert
        response.Data.Attributes.Should().BeNull();
        response.Data.Relationships.Should().BeNull();
    }

    [Fact]
    public async Task Patching_resource_without_side_effects_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        PrimaryFlightResponseDocument? response = await ApiResponse.TranslateAsync(async () => await apiClient.PatchFlightAsync(flightId, null,
            new UpdateFlightRequestDocument
            {
                Data = new DataInUpdateFlightRequest
                {
                    Id = flightId
                }
            }));

        // Assert
        response.Should().BeNull();
    }

    [Fact]
    public async Task Deleting_resource_produces_empty_response()
    {
        // Arrange
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        Func<Task> action = async () => await apiClient.DeleteFlightAsync("ZvuH1");

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Getting_secondary_resource_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string purserId = "bBJHu";
        const string emailAddress = "email@example.com";
        const string age = "20";
        const string profileImageUrl = "www.image.com";
        const string distanceTraveledInKilometer = "5000";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/purser",
                "first": "{{HostPrefix}}flights/{{flightId}}/purser",
                "last": "{{HostPrefix}}flights/{{flightId}}/purser"
              },
              "data": {
                "type": "flight-attendants",
                "id": "{{purserId}}",
                "attributes": {
                  "email-address": "{{emailAddress}}",
                  "age": "{{age}}",
                  "profile-image-url": "{{profileImageUrl}}",
                  "distance-traveled-in-kilometers": "{{distanceTraveledInKilometer}}",
                },
                "relationships": {
                  "scheduled-for-flights": {
                    "links": {
                      "self": "{{HostPrefix}}flight-attendants/{{purserId}}/relationships/scheduled-for-flights",
                      "related": "{{HostPrefix}}flight-attendants/{{purserId}}/scheduled-for-flights"
                    }
                  },
                  "purser-on-flights": {
                    "links": {
                      "self": "{{HostPrefix}}flight-attendants/{{purserId}}/relationships/purser-on-flights",
                      "related": "{{HostPrefix}}flight-attendants/{{purserId}}/purser-on-flights"
                    }
                  },
                },
                "links": {
                  "self": "{{HostPrefix}}flight-attendants/{{purserId}}",
                }
              }
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        SecondaryFlightAttendantResponseDocument response = await apiClient.GetFlightPurserAsync(flightId, null, null);

        // Assert
        response.Data.Should().NotBeNull();
        response.Data.Id.Should().Be(purserId);
        response.Data.Attributes.EmailAddress.Should().Be(emailAddress);
        response.Data.Attributes.Age.Should().Be(int.Parse(age));
        response.Data.Attributes.ProfileImageUrl.Should().Be(profileImageUrl);
        response.Data.Attributes.DistanceTraveledInKilometers.Should().Be(int.Parse(distanceTraveledInKilometer));
    }

    [Fact]
    public async Task Getting_nullable_secondary_resource_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/backup-purser",
                "first": "{{HostPrefix}}flights/{{flightId}}/backup-purser",
                "last": "{{HostPrefix}}flights/{{flightId}}/backup-purser"
              },
              "data": null
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        NullableSecondaryFlightAttendantResponseDocument response = await apiClient.GetFlightBackupPurserAsync(flightId, null, null);

        // Assert
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task Getting_secondary_resources_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/cabin-crew-members",
                "first": "{{HostPrefix}}flights/{{flightId}}/cabin-crew-members"
              },
              "data": [ ]
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        FlightAttendantCollectionResponseDocument response = await apiClient.GetFlightCabinCrewMembersAsync(flightId, null, null);

        // Assert
        response.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Getting_nullable_ToOne_relationship_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/relationships/backup-purser",
                "related": "{{HostPrefix}}flights/{{flightId}}/relationships/backup-purser"
              },
              "data": null
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        NullableFlightAttendantIdentifierResponseDocument response = await apiClient.GetFlightBackupPurserRelationshipAsync(flightId, null, null);

        // Assert
        response.Data.Should().BeNull();
    }

    [Fact]
    public async Task Getting_ToOne_relationship_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string purserId = "bBJHu";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/relationships/purser",
                "related": "{{HostPrefix}}flights/{{flightId}}/relationships/purser"
              },
              "data": {
                "type": "flight-attendants",
                "id": "{{purserId}}"
              }
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        FlightAttendantIdentifierResponseDocument response = await apiClient.GetFlightPurserRelationshipAsync(flightId, null, null);

        // Assert
        response.Data.Should().NotBeNull();
        response.Data.Id.Should().Be(purserId);
        response.Data.Type.Should().Be(FlightAttendantResourceType.FlightAttendants);
    }

    [Fact]
    public async Task Patching_ToOne_relationship_translates_response()
    {
        // Arrange
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new ToOneFlightAttendantInRequest
        {
            Data = new FlightAttendantIdentifierInRequest
            {
                Id = "Adk2a"
            }
        };

        // Act
        await apiClient.PatchFlightPurserRelationshipAsync("ZvuH1", requestBody);
    }

    [Fact]
    public async Task Getting_ToMany_relationship_translates_response()
    {
        // Arrange
        const string flightId = "ZvuH1";
        const string flightAttendantId1 = "bBJHu";
        const string flightAttendantId2 = "ZvuHNInmX1";

        const string responseBody = $$"""
            {
              "links": {
                "self": "{{HostPrefix}}flights/{{flightId}}/relationships/cabin-crew-members",
                "related": "{{HostPrefix}}flights/{{flightId}}/relationships/cabin-crew-members",
                "first": "{{HostPrefix}}flights/{{flightId}}/relationships/cabin-crew-members"
              },
              "data": [{
                "type": "flight-attendants",
                "id": "{{flightAttendantId1}}"
              },
              {
                "type": "flight-attendants",
                "id": "{{flightAttendantId2}}"
              }]
            }
            """;

        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.OK, responseBody);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        // Act
        FlightAttendantIdentifierCollectionResponseDocument response = await apiClient.GetFlightCabinCrewMembersRelationshipAsync(flightId, null, null);

        // Assert
        response.Data.Should().HaveCount(2);
        response.Data.First().Id.Should().Be(flightAttendantId1);
        response.Data.First().Type.Should().Be(FlightAttendantResourceType.FlightAttendants);
        response.Data.Last().Id.Should().Be(flightAttendantId2);
        response.Data.Last().Type.Should().Be(FlightAttendantResourceType.FlightAttendants);
    }

    [Fact]
    public async Task Posting_ToMany_relationship_produces_empty_response()
    {
        // Arrange
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new ToManyFlightAttendantInRequest
        {
            Data =
            [
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Adk2a"
                },
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Un37k"
                }
            ]
        };

        // Act
        Func<Task> action = async () => await apiClient.PostFlightCabinCrewMembersRelationshipAsync("ZvuH1", requestBody);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Patching_ToMany_relationship_produces_empty_response()
    {
        // Arrange
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new ToManyFlightAttendantInRequest
        {
            Data =
            [
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Adk2a"
                },
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Un37k"
                }
            ]
        };

        // Act
        Func<Task> action = async () => await apiClient.PatchFlightCabinCrewMembersRelationshipAsync("ZvuH1", requestBody);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Deleting_ToMany_relationship_produces_empty_response()
    {
        // Arrange
        using var wrapper = FakeHttpClientWrapper.Create(HttpStatusCode.NoContent, null);
        var apiClient = new LegacyClient(wrapper.HttpClient);

        var requestBody = new ToManyFlightAttendantInRequest
        {
            Data =
            [
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Adk2a"
                },
                new FlightAttendantIdentifierInRequest
                {
                    Id = "Un37k"
                }
            ]
        };

        // Act
        Func<Task> action = async () => await apiClient.DeleteFlightCabinCrewMembersRelationshipAsync("ZvuH1", requestBody);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
