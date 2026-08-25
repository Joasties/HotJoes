using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public sealed class NewVendorRegistrationProcessor : INewVendorRegistrationProcessor
{
    private readonly VendorRegisteredIntegrationEventMapper _eventMapper;
    private readonly INewVendorRegistrationCommitter _committer;
    private readonly IRegistrationIdentifierGenerator _identifierGenerator;
    private readonly TimeProvider _timeProvider;

    public NewVendorRegistrationProcessor(
        VendorRegisteredIntegrationEventMapper eventMapper,
        INewVendorRegistrationCommitter committer,
        IRegistrationIdentifierGenerator identifierGenerator,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(eventMapper);
        ArgumentNullException.ThrowIfNull(committer);
        ArgumentNullException.ThrowIfNull(identifierGenerator);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _eventMapper = eventMapper;
        _committer = committer;
        _identifierGenerator = identifierGenerator;
        _timeProvider = timeProvider;
    }

    public async Task<RegisterVendorResult> ProcessAsync(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues,
        VendorRegistrationIdentity identity,
        RegistrationSemanticFingerprint fingerprint,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(addressValues);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(fingerprint);

        DateTimeOffset registeredAt = _timeProvider.GetUtcNow();
        VendorRegistrationInformation information = CreateRegistrationInformation(
            command,
            addressValues);
        HotJoes.Domain.Vendor.Vendor vendor =
            HotJoes.Domain.Vendor.Vendor.Register(
                _identifierGenerator.CreateVendorId(),
                information,
                CreateWebsite(command.Website),
                command.BusinessDescription,
                registeredAt);
        VendorRegistered completedFact = vendor.DomainEvents
            .OfType<VendorRegistered>()
            .Single();
        RegisterVendorResult.Success originalResult =
            CreateSuccessfulResult(vendor);
        VendorRegisteredIntegrationEvent integrationEvent = _eventMapper.Map(
            completedFact,
            vendor,
            _identifierGenerator.CreateEventId(),
            registeredAt);
        var commit = new NewVendorRegistrationCommit(
            vendor,
            identity,
            fingerprint,
            originalResult,
            integrationEvent);

        try
        {
            await _committer.CommitAsync(commit, cancellationToken);
        }
        catch (ConcurrentVendorRegistrationException)
        {
            throw;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return RegisterVendorResult.PersistenceOrAtomicRecordingFailed();
        }

        return originalResult;
    }

    private static VendorRegistrationInformation CreateRegistrationInformation(
        RegisterVendorCommand command,
        AddressAuthoritativeValues addressValues)
    {
        return new VendorRegistrationInformation(
            command.LegalOperatorType,
            new VendorName(command.LegalOperatorName),
            new VendorName(command.TradingName),
            CreateCompanyRegistrationNumber(command.CompanyRegistrationNumber),
            new PrimaryContact(
                command.ContactName,
                new EmailAddress(command.ContactEmail),
                new TelephoneNumber(command.ContactTelephone)),
            addressValues.CanonicalAddressId,
            addressValues.BusinessAddressSnapshot,
            addressValues.FoodRegistrationAuthority,
            addressValues.PrimaryTradingAuthority,
            new TradingCharacteristics(
                command.TradingLocation,
                new OpeningHours(
                    command.OpeningHoursStartTime,
                    command.OpeningHoursEndTime),
                command.ServiceIncludesHotFood,
                command.AlcoholService));
    }

    private static CompanyRegistrationNumber? CreateCompanyRegistrationNumber(
        string? value)
    {
        return value is null ? null : new CompanyRegistrationNumber(value);
    }

    private static Uri? CreateWebsite(string? value)
    {
        return value is null ? null : new Uri(value, UriKind.Absolute);
    }

    private static RegisterVendorResult.Success CreateSuccessfulResult(
        HotJoes.Domain.Vendor.Vendor vendor)
    {
        return (RegisterVendorResult.Success)RegisterVendorResult.Succeeded(vendor.Id);
    }
}
