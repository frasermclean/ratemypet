using Azure.Core;
using Azure.Identity;

namespace RateMyPet.Api.Startup;

public static class TokenCredentialFactory
{
    private static readonly TokenCredential[] Sources =
    [
        new AzureCliCredential(),
        new ManagedIdentityCredential()
    ];

    public static TokenCredential Create() => new ChainedTokenCredential(Sources);
}
