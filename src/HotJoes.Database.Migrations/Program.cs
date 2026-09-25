using HotJoes.Database.Migrations;

const string vendorConnectionStringKey =
    "ConnectionStrings__VendorDatabase";
const string complianceConnectionStringKey =
    "ConnectionStrings__ComplianceDatabase";
const string communityConnectionStringKey =
    "ConnectionStrings__CommunityDatabase";

string? vendorConnectionString = Environment.GetEnvironmentVariable(
    vendorConnectionStringKey);
string? complianceConnectionString = Environment.GetEnvironmentVariable(
    complianceConnectionStringKey);
string? communityConnectionString = Environment.GetEnvironmentVariable(
    communityConnectionStringKey);

if (string.IsNullOrWhiteSpace(vendorConnectionString) ||
    string.IsNullOrWhiteSpace(complianceConnectionString) ||
    string.IsNullOrWhiteSpace(communityConnectionString))
{
    Console.Error.WriteLine(
        "Required database migration configuration is missing.");
    return 2;
}

try
{
    var operation = new DatabaseMigrationOperation(
        vendorConnectionString,
        complianceConnectionString,
        communityConnectionString);
    await operation.ExecuteAsync();
    return 0;
}
catch (Exception)
{
    Console.Error.WriteLine("Database migration operation failed.");
    return 1;
}
