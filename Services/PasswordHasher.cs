using System.Security.Cryptography;
using System.Text;

namespace MauiTemplate.Services;

/// <summary>
/// Service de hachage de mots de passe
/// Respecte le principe de responsabilité unique (SRP)
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Le mot de passe ne peut pas être vide.", nameof(password));

        using var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        string hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}

