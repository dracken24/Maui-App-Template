using MauiTemplate.Models;

namespace MauiTemplate.Repositories;

/// <summary>
/// Interface pour le repository des utilisateurs
/// Respecte le principe d'inversion de dépendances (DIP)
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int userId);
    Task<List<User>> GetAllAsync();
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}

