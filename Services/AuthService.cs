using MauiTemplate.Models;
using MauiTemplate.Services;
using System.Security.Cryptography;
using System.Text;

namespace MauiTemplate.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private User? _currentUser;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public User? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _databaseService.GetUserByUsernameAsync(username);
            if (user != null && VerifyPassword(password, user.Password))
            {
                _currentUser = user;
                user.DerniereConnexion = DateTime.Now;
                await _databaseService.UpdateUserAsync(user);
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string? nom = null, string? prenom = null, string? telephone = null)
        {
            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _databaseService.GetUserByUsernameAsync(username);
            if (existingUser != null)
                return false;

            var existingEmail = await _databaseService.GetUserByEmailAsync(email);
            if (existingEmail != null)
                return false;

            // Créer le nouvel utilisateur
            var user = new User
            {
                Username = username,
                Email = email,
                Password = HashPassword(password),
                Nom = nom,
                Prenom = prenom,
                Telephone = telephone,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            try
            {
                await _databaseService.CreateUserAsync(user);
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

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
