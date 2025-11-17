using MauiTemplate.Models;
using MauiTemplate.Repositories;

namespace MauiTemplate.Services;

/// <summary>
/// Service d'authentification
/// Respecte le principe de responsabilité unique (SRP) et d'inversion de dépendances (DIP)
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private User? _currentUser;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !_passwordHasher.VerifyPassword(password, user.Password))
            return false;

        if (!user.EstActif)
            return false;

        _currentUser = user;
        user.DerniereConnexion = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> RegisterAsync(string username, string email, string password, string? nom = null, string? prenom = null, string? telephone = null)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        // Vérifier si l'utilisateur existe déjà
        if (await _userRepository.ExistsByUsernameAsync(username))
            return false;

        if (await _userRepository.ExistsByEmailAsync(email))
            return false;

        // Créer le nouvel utilisateur
        var user = new User
        {
            Username = username,
            Email = email,
            Password = _passwordHasher.HashPassword(password),
            Nom = nom,
            Prenom = prenom,
            Telephone = telephone,
            DateCreation = DateTime.Now,
            EstActif = true
        };

        try
        {
            await _userRepository.CreateAsync(user);
            _currentUser = user;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Logout()
    {
        _currentUser = null;
    }
}
