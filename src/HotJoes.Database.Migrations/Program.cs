using HotJoes.Database.Migrations;

const string vendorConnectionStringKey =
    "ConnectionStrings__VendorDatabase";
const string complianceConnectionStringKey =
    "ConnectionStrings__ComplianceDatabase";

string? vendorConnectionString = Environment.GetEnvironmentVariable(
    vendorConnectionStringKey);
string? complianceConnectionString = Environment.GetEnvironmentVariable(
    complianceConnectionStringKey);

if (string.IsNullOrWhiteSpace(vendorConnectionString) ||
    string.IsNullOrWhiteSpace(complianceConnectionString))
{
    Console.Error.WriteLine(
        "Required database migration configuration is missing.");
    return 2;
}

try
{
    var operation = new DatabaseMigrationOperation(
        vendorConnectionString,
        complianceConnectionString);
    await operation.ExecuteAsync();
    return 0;
}
catch (Exception)
{
    Console.Error.WriteLine("Database migration operation failed.");
    return 1;
}
