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

    /// <summary>
    /// Tests if the string is a valid email address.
    /// </summary>
    /// <param name="value">The string to test</param>
    /// <returns>True on valid email address, false if not</returns>
    public static bool IsEmailAddress(this string value)
    {
        if (value.AsSpan().ContainsAny('\r', '\n'))
        {
            return false;
        }

        // return true if there is only 1 '@' character, and it is neither the first nor the last character
        var index = value.IndexOf('@');

        return index > 0 && index != value.Length - 1 && index == value.LastIndexOf('@');
    }
}
