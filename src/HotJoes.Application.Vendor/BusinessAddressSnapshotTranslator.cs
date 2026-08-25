using HotJoes.Domain.Vendor;

namespace HotJoes.Application.Vendor;

public static class BusinessAddressSnapshotTranslator
{
    public static BusinessAddressSnapshot Translate(
        string? sourceLine1,
        string sourceLine2,
        string? sourceLine3,
        string? sourceLine4,
        string postTown,
        string postcode,
        string? county)
    {
        return new BusinessAddressSnapshot(
            addressLine1: sourceLine2,
            addressLine2: sourceLine3,
            addressLine3: sourceLine4,
            postTown: postTown,
            postcode: postcode,
            county: county,
            recipientOrOrganisationName: sourceLine1);
    }
}
