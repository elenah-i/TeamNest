using System.Security.Cryptography;

namespace TeamNest.Helpers;

public static class InviteCodeGenerator
{
    // No 0/O or 1/I/L, so codes are easy to read aloud and type
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    /// <summary>Creates a random code like "K7MX-Q4TP".</summary>
    public static string Create()
    {
        var chars = new char[9];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = i == 4 ? '-' : Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }
        return new string(chars);
    }

    /// <summary>Cleans up what a user typed so "k7mx-q4tp " matches "K7MX-Q4TP".</summary>
    public static string Normalize(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();
}
