using HotJoes.Application.Vendor;
using HotJoes.Domain.Vendor;

namespace HotJoes.Api.Vendor;

internal sealed class SystemRegistrationIdentifierGenerator
    : IRegistrationIdentifierGenerator
{
    public VendorId CreateVendorId()
    {
        return new VendorId(Guid.NewGuid());
    }

    public Guid CreateEventId()
    {
        return Guid.NewGuid();
    }
}
