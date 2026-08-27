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
    public void RequestValidationFailed_WithMixedErrors_RetainsOneImmutableFailure()
    {
        RegistrationValidationError[] errors =
        {
            new(
                nameof(RegisterVendorCommand.TradingName),
                RegistrationValidationErrorCode.Required,
                "Trading Name is required."),
            new(
                nameof(RegisterVendorCommand.AuthorisedToRegisterBusiness),
                RegistrationValidationErrorCode.InvalidValue,
                "Authorisation must be accepted."),
            new(
                nameof(RegisterVendorCommand.CompanyRegistrationNumber),
                RegistrationValidationErrorCode.ConditionallyRequired,
                "Company Registration Number is required.")
        };

        RegisterVendorResult result =
            RegisterVendorResult.RequestValidationFailed(errors);

        var failure = Assert.IsType<RegisterVendorResult.RequestValidationFailure>(
            result);
        Assert.Equal(errors, failure.Errors);
        Assert.NotSame(errors, failure.Errors);
        Assert.Throws<NotSupportedException>(
            () => ((IList<RegistrationValidationError>)failure.Errors).Add(errors[0]));
    }

    [Fact]
    public void ExpectedFailures_AreDistinctClosedApplicationOutcomeKinds()
    {
        var expectedFailures = new (RegisterVendorResult Result, Type Type)[]
        {
            (
                RegisterVendorResult.RequestValidationFailed(
                    new[]
                    {
                        new RegistrationValidationError(
                            nameof(RegisterVendorCommand.TradingName),
                            RegistrationValidationErrorCode.Required,
                            "Trading Name is required.")
                    }),
                typeof(RegisterVendorResult.RequestValidationFailure)),
            (RegisterVendorResult.ReferenceIsInvalid(), typeof(RegisterVendorResult.InvalidReference)),
            (RegisterVendorResult.AddressResultIsInvalid(), typeof(RegisterVendorResult.InvalidAddressResult)),
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
    public void PublicOutcomeSet_ContainsOnlySuccessAndApprovedControlledFailures()
    {
        string[] expectedOutcomeTypeNames =
        {
            nameof(RegisterVendorResult.AddressServiceTemporarilyUnavailable),
            nameof(RegisterVendorResult.AggregateInvariantFailure),
            nameof(RegisterVendorResult.IdempotencyConflict),
            nameof(RegisterVendorResult.InvalidAddressResult),
            nameof(RegisterVendorResult.InvalidReference),
            nameof(RegisterVendorResult.PersistenceOrAtomicRecordingFailure),
            nameof(RegisterVendorResult.RequestValidationFailure),
            nameof(RegisterVendorResult.Success)
        };

        Type[] actualOutcomeTypes = typeof(RegisterVendorResult)
            .GetNestedTypes(BindingFlags.Public)
            .OrderBy(type => type.Name)
            .ToArray();

        Assert.Equal(
            expectedOutcomeTypeNames.Order(),
            actualOutcomeTypes.Select(type => type.Name));
        Assert.DoesNotContain(
            actualOutcomeTypes,
            type => type.Name is "RegistrationDeclarationFailure" or
                "ConditionalRuleFailure");
        Assert.True(typeof(RegisterVendorResult).IsAbstract);
        Assert.All(actualOutcomeTypes, type => Assert.True(type.IsSealed));
    }

    [Fact]
    public void PublicFactorySet_ContainsNoSeparateDeclarationOrConditionalFailure()
    {
        string[] factoryNames = typeof(RegisterVendorResult)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(method => method.Name)
            .ToArray();

        Assert.DoesNotContain("RegistrationDeclarationFailed", factoryNames);
        Assert.DoesNotContain("ConditionalRuleFailed", factoryNames);
    }

    [Fact]
    public void FailureOutcomes_ExposeNoSuccessPayload()
    {
        Type[] failureTypes = typeof(RegisterVendorResult)
            .GetNestedTypes(BindingFlags.Public)
            .Where(type => type != typeof(RegisterVendorResult.Success))
            .ToArray();

        Assert.NotEmpty(failureTypes);
        Assert.All(
            failureTypes,
            type =>
            {
                PropertyInfo[] properties = type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public);
                Assert.DoesNotContain(
                    properties,
                    property => property.PropertyType == typeof(VendorId));
                Assert.DoesNotContain(
                    properties,
                    property => property.PropertyType == typeof(VendorState));
            });
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
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
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
            typeof(IEnumerable<RegistrationValidationError>),
            typeof(IReadOnlyList<RegistrationValidationError>),
            typeof(RegisterVendorResult),
            typeof(VendorId),
            typeof(VendorState)
        };

        Assert.All(exposedTypes, type => Assert.Contains(type, permittedTypes));
    }
}
