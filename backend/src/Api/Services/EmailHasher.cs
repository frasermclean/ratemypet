using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace RateMyPet.Api.Services;

public class EmailHasher
{
    private readonly ConcurrentDictionary<string, string> dictionary = new();

    public string GetSha256Hash(string? emailAddress)
    {
        if (emailAddress is null)
        {
            return string.Empty;
        }

        return dictionary.GetOrAdd(emailAddress, value =>
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        });
    }
}
