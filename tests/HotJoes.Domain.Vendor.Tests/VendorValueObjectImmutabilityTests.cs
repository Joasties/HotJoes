using System.Reflection;
using System.Runtime.CompilerServices;
using HotJoes.Domain.Vendor;

namespace HotJoes.Domain.Vendor.Tests;

public sealed class VendorValueObjectImmutabilityTests
{
    [Theory]
    [InlineData("VendorId")]
    [InlineData("TradingCharacteristics")]
    [InlineData("OpeningHours")]
    [InlineData("VendorName")]
    [InlineData("CompanyRegistrationNumber")]
    [InlineData("PrimaryContact")]
    [InlineData("EmailAddress")]
    [InlineData("TelephoneNumber")]
    [InlineData("CanonicalAddressId")]
    [InlineData("BusinessAddressSnapshot")]
    public void PublicSurface_ForApplicableValueObject_ExposesNoPublicMutation(
        string valueObjectType)
    {
        var type = ResolveType(valueObjectType);
        var publicInstanceProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var publicInstanceFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

        Assert.All(
            publicInstanceProperties,
            property => Assert.False(HasPublicMutationSetter(property)));
        Assert.All(publicInstanceFields, field => Assert.True(field.IsInitOnly));
    }

    [Theory]
    [InlineData("TradingCharacteristics")]
    [InlineData("VendorName")]
    [InlineData("CompanyRegistrationNumber")]
    [InlineData("PrimaryContact")]
    [InlineData("EmailAddress")]
    [InlineData("TelephoneNumber")]
    [InlineData("CanonicalAddressId")]
    [InlineData("BusinessAddressSnapshot")]
    public void Construction_ForReferenceValueObject_RequiresDefiningValues(
        string valueObjectType)
    {
        var type = ResolveType(valueObjectType);
        var publicConstructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

        Assert.NotEmpty(publicConstructors);
        Assert.DoesNotContain(
            publicConstructors,
            constructor => constructor.GetParameters().Length == 0);
    }

    private static Type ResolveType(string valueObjectType)
    {
        return valueObjectType switch
        {
            "VendorId" => typeof(VendorId),
            "TradingCharacteristics" => typeof(TradingCharacteristics),
            "OpeningHours" => typeof(OpeningHours),
            "VendorName" => typeof(VendorName),
            "CompanyRegistrationNumber" => typeof(CompanyRegistrationNumber),
            "PrimaryContact" => typeof(PrimaryContact),
            "EmailAddress" => typeof(EmailAddress),
            "TelephoneNumber" => typeof(TelephoneNumber),
            "CanonicalAddressId" => typeof(CanonicalAddressId),
            "BusinessAddressSnapshot" => typeof(BusinessAddressSnapshot),
            _ => throw new ArgumentOutOfRangeException(nameof(valueObjectType))
        };
    }

    private static bool HasPublicMutationSetter(PropertyInfo property)
    {
        var setMethod = property.SetMethod;

        if (setMethod is null || !setMethod.IsPublic)
        {
            return false;
        }

        var isInitOnly = setMethod.ReturnParameter
            .GetRequiredCustomModifiers()
            .Contains(typeof(IsExternalInit));

        return !isInitOnly;
    }

}
