using HotJoes.Application.Vendor;

namespace HotJoes.Application.Vendor.Tests;

public sealed class BusinessAddressSnapshotTranslatorTests
{
    [Fact]
    public void Translate_WithAllSourceFields_MapsEveryFieldPositionally()
    {
        var snapshot = BusinessAddressSnapshotTranslator.Translate(
            sourceLine1: "Source Line 1 Recipient",
            sourceLine2: "Source Line 2 Building",
            sourceLine3: "Source Line 3 Street",
            sourceLine4: "Source Line 4 Locality",
            postTown: "GREENWICH",
            postcode: "SE10 8QY",
            county: "Greater London");

        Assert.Equal("Source Line 1 Recipient", snapshot.RecipientOrOrganisationName);
        Assert.Equal("Source Line 2 Building", snapshot.AddressLine1);
        Assert.Equal("Source Line 3 Street", snapshot.AddressLine2);
        Assert.Equal("Source Line 4 Locality", snapshot.AddressLine3);
        Assert.Equal("GREENWICH", snapshot.PostTown);
        Assert.Equal("SE10 8QY", snapshot.Postcode);
        Assert.Equal("Greater London", snapshot.County);
    }

    [Fact]
    public void Translate_WithAbsentOptionalSourceFields_PreservesAbsenceWithoutShifting()
    {
        var snapshot = BusinessAddressSnapshotTranslator.Translate(
            sourceLine1: null,
            sourceLine2: "Source Line 2 Building",
            sourceLine3: null,
            sourceLine4: "Source Line 4 Locality",
            postTown: "GREENWICH",
            postcode: "SE10 8QY",
            county: null);

        Assert.Null(snapshot.RecipientOrOrganisationName);
        Assert.Equal("Source Line 2 Building", snapshot.AddressLine1);
        Assert.Null(snapshot.AddressLine2);
        Assert.Equal("Source Line 4 Locality", snapshot.AddressLine3);
        Assert.Equal("GREENWICH", snapshot.PostTown);
        Assert.Equal("SE10 8QY", snapshot.Postcode);
        Assert.Null(snapshot.County);
    }
}
