using MauiTemplate.Models;

namespace MauiTemplate.Services;

/// <summary>
/// Interface pour le service d'authentification
/// Respecte le principe d'inversion de dépendances (DIP)
/// </summary>
public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string email, string password, string? nom = null, string? prenom = null, string? telephone = null);
    void Logout();
}

