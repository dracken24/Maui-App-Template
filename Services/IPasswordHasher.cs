namespace MauiTemplate.Services;

/// <summary>
/// Interface pour le service de hachage de mots de passe
/// Respecte le principe de responsabilité unique (SRP) et d'inversion de dépendances (DIP)
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

