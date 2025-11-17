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
            Models.User? user = await _databaseService.GetUserByUsernameAsync(username);
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
            Models.User? existingUser = await _databaseService.GetUserByUsernameAsync(username);
            if (existingUser != null)
                return false;

            Models.User? existingEmail = await _databaseService.GetUserByEmailAsync(email);
            if (existingEmail != null)
                return false;

            // Créer le nouvel utilisateur
            Models.User user = new Models.User
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
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
