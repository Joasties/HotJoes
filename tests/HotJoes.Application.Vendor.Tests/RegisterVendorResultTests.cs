using System.Reflection;
using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class RegisterVendorResultTests
{
    [Fact]
    public void Succeeded_WithCommittedVendorIdentityAndState_RetainsSuccessData()
    {
        var vendorId = new VendorId(
            Guid.Parse("7b247759-bc21-431f-82f5-38b4339a6075"));

        RegisterVendorResult result = RegisterVendorResult.Succeeded(vendorId);

        var success = Assert.IsType<RegisterVendorResult.Success>(result);

        Assert.Equal(vendorId, success.VendorId);
        Assert.Equal(VendorState.PendingActivation, success.VendorState);
    }

    [Fact]
    public void ExpectedFailures_AreDistinctClosedApplicationOutcomeKinds()
    {
        var expectedFailures = new (RegisterVendorResult Result, Type Type)[]
        {
            (
                RegisterVendorResult.RequestValidationFailed(),
                typeof(RegisterVendorResult.RequestValidationFailure)),
            (
                RegisterVendorResult.RegistrationDeclarationFailed(),
                typeof(RegisterVendorResult.RegistrationDeclarationFailure)),
            (
                RegisterVendorResult.ConditionalRuleFailed(),
                typeof(RegisterVendorResult.ConditionalRuleFailure)),
            (
                RegisterVendorResult.ReferenceIsInvalid(),
                typeof(RegisterVendorResult.InvalidReference)),
            (
                RegisterVendorResult.AddressResultIsInvalid(),
                typeof(RegisterVendorResult.InvalidAddressResult)),
            (
                RegisterVendorResult.AddressServiceIsTemporarilyUnavailable(),
                typeof(RegisterVendorResult.AddressServiceTemporarilyUnavailable)),
            (
                RegisterVendorResult.AggregateInvariantFailed(),
                typeof(RegisterVendorResult.AggregateInvariantFailure)),
            (
                RegisterVendorResult.PersistenceOrAtomicRecordingFailed(),
                typeof(RegisterVendorResult.PersistenceOrAtomicRecordingFailure)),
            (
                RegisterVendorResult.IdempotencyConflictDetected(),
                typeof(RegisterVendorResult.IdempotencyConflict))
        };

        Assert.All(
            expectedFailures,
            expected => Assert.Equal(expected.Type, expected.Result.GetType()));

        Assert.Equal(
            expectedFailures.Length,
            expectedFailures.Select(expected => expected.Type).Distinct().Count());
    }

    [Fact]
    public void PublicOutcomeSet_ContainsOnlySuccessAndExpectedControlledFailures()
    {
        Type[] expectedOutcomeTypes =
        {
            typeof(RegisterVendorResult.AddressServiceTemporarilyUnavailable),
            typeof(RegisterVendorResult.AggregateInvariantFailure),
            typeof(RegisterVendorResult.ConditionalRuleFailure),
            typeof(RegisterVendorResult.IdempotencyConflict),
            typeof(RegisterVendorResult.InvalidAddressResult),
            typeof(RegisterVendorResult.InvalidReference),
            typeof(RegisterVendorResult.PersistenceOrAtomicRecordingFailure),
            typeof(RegisterVendorResult.RegistrationDeclarationFailure),
            typeof(RegisterVendorResult.RequestValidationFailure),
            typeof(RegisterVendorResult.Success)
        };

        Type[] actualOutcomeTypes = typeof(RegisterVendorResult)
            .GetNestedTypes(BindingFlags.Public)
            .OrderBy(type => type.Name)
            .ToArray();

        Assert.Equal(
            expectedOutcomeTypes.OrderBy(type => type.Name),
            actualOutcomeTypes);
        Assert.True(typeof(RegisterVendorResult).IsAbstract);
        Assert.All(actualOutcomeTypes, type => Assert.True(type.IsSealed));
    }

    [Fact]
    public void FailureOutcomes_ExposeNoSuccessPayloadOrFailureDetail()
    {
        Type[] failureTypes = typeof(RegisterVendorResult)
            .GetNestedTypes(BindingFlags.Public)
            .Where(type => type != typeof(RegisterVendorResult.Success))
            .ToArray();

        Assert.NotEmpty(failureTypes);
        Assert.All(
            failureTypes,
            type => Assert.Empty(
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public)));
    }

    [Fact]
    public void PublicSurface_IsImmutableAndExposesOnlyApplicationOutcomeTypes()
    {
        Type resultType = typeof(RegisterVendorResult);
        Type[] outcomeTypes = resultType.GetNestedTypes(BindingFlags.Public);

        Assert.All(
            outcomeTypes.SelectMany(
                type => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)),
            property => Assert.Null(property.SetMethod));

        Type[] exposedTypes = resultType
            .GetMethods(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .SelectMany(method => method
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .Append(method.ReturnType))
            .Concat(outcomeTypes.SelectMany(type => type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property => property.PropertyType)))
            .Distinct()
            .ToArray();

        Type[] permittedTypes =
        {
            typeof(RegisterVendorResult),
            typeof(VendorId),
            typeof(VendorState)
        };

        Assert.All(exposedTypes, type => Assert.Contains(type, permittedTypes));
    }
}
