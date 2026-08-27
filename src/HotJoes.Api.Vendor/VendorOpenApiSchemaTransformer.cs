using System.Reflection;
using System.Text.Json.Nodes;
using HotJoes.Application.Vendor;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HotJoes.Api.Vendor;

public sealed class VendorOpenApiSchemaTransformer
    : IOpenApiSchemaTransformer
{
    private const string TimePattern =
        "^([01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d$";

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        Type schemaType = context.JsonPropertyInfo?.PropertyType
            ?? context.JsonTypeInfo.Type;

        AddRequiredMembers(schema, schemaType);
        ApplyPropertyContract(schema, context.JsonPropertyInfo?.AttributeProvider);
        return Task.CompletedTask;
    }

    private static void AddRequiredMembers(OpenApiSchema schema, Type type)
    {
        string[] required = type == typeof(RegisterVendorRequest)
            ?
            [
                "tradingName",
                "legalOperatorName",
                "legalOperatorType",
                "tradingCharacteristics",
                "primaryContact",
                "addressResolutionReference",
                "registrationDeclarations"
            ]
            : type == typeof(RegisterVendorTradingCharacteristicsRequest)
                ?
                [
                    "tradingLocation",
                    "openingHours",
                    "serviceIncludesHotFood",
                    "alcoholService"
                ]
                : type == typeof(RegisterVendorOpeningHoursRequest)
                    ? ["startTime", "endTime"]
                    : type == typeof(RegisterVendorPrimaryContactRequest)
                        ? ["contactName", "contactEmail", "contactTelephone"]
                        : type == typeof(RegisterVendorRegistrationDeclarationsRequest)
                            ?
                            [
                                "authorisedToRegisterBusiness",
                                "informationAccurate",
                                "acceptHotJoesPlatformTerms"
                            ]
                            : ResponseRequiredMembers(type);

        if (required.Length == 0)
        {
            return;
        }

        schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
        foreach (string member in required)
        {
            schema.Required.Add(member);
        }
    }

    private static string[] ResponseRequiredMembers(Type type)
    {
        if (type == typeof(RegisterVendorResponse))
        {
            return ["vendorId", "vendorState"];
        }

        if (type == typeof(RegisteredVendorDetailsResponse))
        {
            return
            [
                "vendorId",
                "registeredAt",
                "vendorState",
                "tradingPreference",
                "tradingName",
                "legalOperatorType",
                "legalOperatorName",
                "companyRegistrationNumber",
                "tradingCharacteristics",
                "primaryContact",
                "canonicalAddressId",
                "businessAddressSnapshot",
                "foodRegistrationAuthority",
                "primaryTradingAuthority",
                "website",
                "businessDescription"
            ];
        }

        if (type == typeof(RegisteredVendorTradingCharacteristicsResponse))
        {
            return
            [
                "tradingLocation",
                "openingHours",
                "serviceIncludesHotFood",
                "alcoholService"
            ];
        }

        if (type == typeof(RegisteredVendorOpeningHoursResponse))
        {
            return ["startTime", "endTime"];
        }

        if (type == typeof(RegisteredVendorPrimaryContactResponse))
        {
            return ["contactName", "contactEmail", "contactTelephone"];
        }

        if (type == typeof(RegisteredVendorBusinessAddressResponse))
        {
            return
            [
                "addressLine1",
                "addressLine2",
                "addressLine3",
                "postTown",
                "postcode",
                "county",
                "recipientOrOrganisationName"
            ];
        }

        if (type == typeof(VendorApiErrorResponse))
        {
            return ["code", "message", "validationErrors"];
        }

        if (type == typeof(VendorApiValidationErrorResponse))
        {
            return ["field", "code", "message"];
        }

        return [];
    }

    private static void ApplyPropertyContract(
        OpenApiSchema schema,
        ICustomAttributeProvider? attributeProvider)
    {
        if (attributeProvider is not PropertyInfo property)
        {
            return;
        }

        Type? declaringType = property.DeclaringType;

        if (declaringType == typeof(RegisterVendorRequest)
            && property.Name == nameof(RegisterVendorRequest.LegalOperatorType))
        {
            SetEnum(
                schema,
                "soleTrader",
                "generalPartnership",
                "limitedCompany",
                "limitedLiabilityPartnership",
                "charitableCommunityGroup",
                "charitableIncorporatedOrganisation");
        }
        else if ((declaringType == typeof(RegisterVendorTradingCharacteristicsRequest)
                    || declaringType == typeof(RegisteredVendorTradingCharacteristicsResponse))
                 && property.Name is "TradingLocation")
        {
            SetEnum(schema, "restaurant", "stall", "kitchen");
        }
        else if ((declaringType == typeof(RegisterVendorOpeningHoursRequest)
                    || declaringType == typeof(RegisteredVendorOpeningHoursResponse))
                 && property.Name is "StartTime" or "EndTime")
        {
            schema.Pattern = TimePattern;
        }
        else if ((declaringType == typeof(RegisterVendorResponse)
                    || declaringType == typeof(RegisteredVendorDetailsResponse))
                 && property.Name == "VendorId")
        {
            schema.Format = "uuid";
        }
        else if (declaringType == typeof(RegisteredVendorDetailsResponse)
                 && property.Name == "RegisteredAt")
        {
            schema.Format = "date-time";
        }
        else if (declaringType == typeof(RegisterVendorResponse)
                 && property.Name == "VendorState")
        {
            SetEnum(schema, "pendingActivation");
        }
        else if (declaringType == typeof(RegisteredVendorDetailsResponse)
                 && property.Name == "VendorState")
        {
            SetEnum(
                schema,
                "pendingActivation",
                "activated",
                "suspended",
                "deactivated");
        }
        else if (declaringType == typeof(RegisteredVendorDetailsResponse)
                 && property.Name == "TradingPreference")
        {
            SetEnum(schema, "offline", "online");
        }
        else if (declaringType == typeof(RegisteredVendorDetailsResponse)
                 && property.Name == "LegalOperatorType")
        {
            SetEnum(
                schema,
                "soleTrader",
                "generalPartnership",
                "limitedCompany",
                "limitedLiabilityPartnership",
                "charitableCommunityGroup",
                "charitableIncorporatedOrganisation");
        }
    }

    private static void SetEnum(
        OpenApiSchema schema,
        params string[] values)
    {
        schema.Enum = values
            .Select(value => (JsonNode)JsonValue.Create(value)!)
            .ToList();
    }
}
