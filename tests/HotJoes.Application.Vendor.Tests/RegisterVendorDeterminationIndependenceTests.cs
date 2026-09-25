using System.Reflection;
using HotJoes.Application.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorDeterminationIndependenceTests
{
    [Fact]
    public void VR_DETERMINATION_023_RegisterVendorCommand_ContainsNoDeterminationState()
    {
        string[] prohibitedFragments =
        [
            "ComplianceDetermination",
            "DeterminationFingerprint",
            "DeterminationItem",
            "RequiredLicenceType",
            "RuleSetVersion"
        ];

        PropertyInfo[] properties = typeof(RegisterVendorCommand)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        Assert.DoesNotContain(properties, property =>
            prohibitedFragments.Any(fragment =>
                property.Name.Contains(
                    fragment,
                    StringComparison.OrdinalIgnoreCase) ||
                property.PropertyType.Name.Contains(
                    fragment,
                    StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void VR_DETERMINATION_023_RegisterVendorService_HasNoComplianceDeterminationDependency()
    {
        ConstructorInfo constructor = Assert.Single(
            typeof(RegisterVendorService).GetConstructors());

        Assert.DoesNotContain(
            constructor.GetParameters(),
            parameter =>
                parameter.ParameterType ==
                    typeof(IComplianceDeterminationPort) ||
                parameter.ParameterType.Name.Contains(
                    "ComplianceDetermination",
                    StringComparison.OrdinalIgnoreCase));
    }
}
