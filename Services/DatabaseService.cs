using MauiTemplate.Data;
using MauiTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiTemplate.Services
{
    public class DatabaseService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        // Méthodes pour les utilisateurs
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            Models.User? user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Méthodes pour les rendez-vous
        public async Task<List<RendezVous>> GetRendezVousByUserIdAsync(int userId)
        {
            return await _context.RendezVous
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.DateDebut)
                .ToListAsync();
        }

        public async Task<List<RendezVous>> GetRendezVousByDateAsync(int userId, DateTime date)
        {
            // Normaliser la date (enlever l'heure)
            var dateStart = date.Date;
            var dateEnd = dateStart.AddDays(1).AddTicks(-1);
            
            return await _context.RendezVous
                .Where(r => r.UserId == userId && 
                            r.DateDebut >= dateStart && 
                            r.DateDebut < dateEnd)
                .OrderBy(r => r.DateDebut)
                .ToListAsync();
        }

        public async Task<RendezVous> CreateRendezVousAsync(RendezVous rendezVous)
        {
            _context.RendezVous.Add(rendezVous);
            await _context.SaveChangesAsync();
            return rendezVous;
        }

        public async Task UpdateRendezVousAsync(RendezVous rendezVous)
        {
            _context.RendezVous.Update(rendezVous);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRendezVousAsync(int rendezVousId)
        {
            Models.RendezVous? rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
            if (rendezVous != null)
            {
                _context.RendezVous.Remove(rendezVous);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.RendezVous)
                .ToListAsync();
        }
    }
}
