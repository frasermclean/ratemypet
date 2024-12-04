using System.Security.Cryptography;
using System.Text;

namespace RateMyPet.Api.Extensions;

public static class StringExtensions
{
    public static string ToSha256Hash(this string? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        var stringBuilder = new StringBuilder();
        foreach (var b in bytes)
        {
            stringBuilder.Append(b.ToString("x2"));
        }

        return stringBuilder.ToString();
    }
}
