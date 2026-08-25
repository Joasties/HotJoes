using System.Reflection;
using HotJoes.Application.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegistrationOutcomeDeterminationContractTests
{
    [Fact]
    public void FirstProcessingRequired_ReturnsFirstProcessingDetermination()
    {
        RegistrationOutcomeDetermination determination =
            RegistrationOutcomeDetermination.FirstProcessingRequired();

        Assert.IsType<RegistrationOutcomeDetermination.FirstProcessing>(
            determination);
    }

    [Fact]
    public void EquivalentReplay_RetainsOriginalCommittedSuccessfulResult()
    {
        var originalResult = Assert.IsType<RegisterVendorResult.Success>(
            RegisterVendorResult.Succeeded(
                new(new Guid("4d25fd15-f6f2-48ea-ad94-d309317b96c9"))));

        var determination = Assert.IsType<
            RegistrationOutcomeDetermination.EquivalentReplay>(
                RegistrationOutcomeDetermination.Replay(originalResult));

        Assert.Same(originalResult, determination.OriginalResult);
    }

    [Fact]
    public void ConflictDetected_ReturnsIdempotencyConflictDetermination()
    {
        RegistrationOutcomeDetermination determination =
            RegistrationOutcomeDetermination.ConflictDetected();

        Assert.IsType<RegistrationOutcomeDetermination.Conflict>(determination);
    }

    [Fact]
    public void DeterminationSurface_IsClosedImmutableAndTransportIndependent()
    {
        var determinationType = typeof(RegistrationOutcomeDetermination);
        var expectedOutcomeTypeNames = new[]
        {
            "Conflict",
            "EquivalentReplay",
            "FirstProcessing"
        };
        var outcomeTypes = determinationType.GetNestedTypes(
            BindingFlags.Public);

        Assert.True(determinationType.IsAbstract);
        Assert.Equal(
            expectedOutcomeTypeNames,
            outcomeTypes.Select(type => type.Name).Order());
        Assert.All(outcomeTypes, type => Assert.True(type.IsSealed));
        Assert.All(
            outcomeTypes.SelectMany(type => type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public)),
            property => Assert.Null(property.SetMethod));
        Assert.DoesNotContain(
            outcomeTypes.SelectMany(type => type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public)),
            property => IsTransportOrInfrastructureType(property.PropertyType));
    }

    [Fact]
    public void DeterminerBoundary_AcceptsOnlyDerivedValuesAndCancellationToken()
    {
        var determinerType = typeof(IRegistrationOutcomeDeterminer);
        var publicMethods = determinerType.GetMethods();

        Assert.True(determinerType.IsInterface);
        var determineMethod = Assert.Single(publicMethods);
        Assert.Equal("DetermineAsync", determineMethod.Name);
        Assert.Equal(
            typeof(Task<RegistrationOutcomeDetermination>),
            determineMethod.ReturnType);
        Assert.Equal(
            new[]
            {
                typeof(VendorRegistrationIdentity),
                typeof(RegistrationSemanticFingerprint),
                typeof(CancellationToken)
            },
            determineMethod.GetParameters()
                .Select(parameter => parameter.ParameterType));
    }

    private static bool IsTransportOrInfrastructureType(Type type)
    {
        var namespaceName = type.Namespace ?? string.Empty;

        return namespaceName.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
            || namespaceName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
            || namespaceName.StartsWith("Npgsql", StringComparison.Ordinal);
    }
}
