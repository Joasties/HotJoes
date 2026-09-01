using Azure.Core;
using Azure.Identity;

namespace HotJoes.Api.Vendor.Configuration;

public sealed class VendorApiAzureCredentialFactory
{
    public TokenCredential CreateProduction(
        string? userAssignedClientId = null)
    {
        if (userAssignedClientId is not null &&
            string.IsNullOrWhiteSpace(userAssignedClientId))
        {
            throw new ArgumentException(
                "User-assigned managed-identity client ID must not be blank.",
                nameof(userAssignedClientId));
        }

        ManagedIdentityId identity = userAssignedClientId is null
            ? ManagedIdentityId.SystemAssigned
            : ManagedIdentityId.FromUserAssignedClientId(
                userAssignedClientId);

        return new ManagedIdentityCredential(identity);
    }

    public TokenCredential CreateDevelopment()
    {
        return new DefaultAzureCredential();
    }
}
