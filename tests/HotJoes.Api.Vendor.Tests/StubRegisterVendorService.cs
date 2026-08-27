using HotJoes.Application.Vendor;

namespace HotJoes.Api.Vendor.Tests;

public sealed class StubRegisterVendorService : IRegisterVendorService
{
    public RegisterVendorResult NextResult { get; set; } =
        RegisterVendorResult.ReferenceIsInvalid();

    public Exception? NextException { get; set; }

    public int InvocationCount { get; private set; }

    public RegisterVendorCommand? LastCommand { get; private set; }

    public bool LastCancellationTokenCanBeCanceled { get; private set; }

    public Task<RegisterVendorResult> RegisterAsync(
        RegisterVendorCommand command,
        CancellationToken cancellationToken = default)
    {
        InvocationCount++;
        LastCommand = command;
        LastCancellationTokenCanBeCanceled = cancellationToken.CanBeCanceled;

        if (NextException is not null)
        {
            return Task.FromException<RegisterVendorResult>(NextException);
        }

        return Task.FromResult(NextResult);
    }
}
