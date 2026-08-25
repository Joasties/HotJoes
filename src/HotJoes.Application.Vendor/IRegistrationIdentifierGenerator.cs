using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public interface IRegistrationIdentifierGenerator
{
    VendorId CreateVendorId();

    Guid CreateEventId();
}
