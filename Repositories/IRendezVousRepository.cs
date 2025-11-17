using MauiTemplate.Models;

namespace MauiTemplate.Repositories;

/// <summary>
/// Interface pour le repository des rendez-vous
/// Respecte le principe d'inversion de dépendances (DIP)
/// </summary>
public interface IRendezVousRepository
{
    Task<RendezVous?> GetByIdAsync(int id);
    Task<List<RendezVous>> GetByUserIdAsync(int userId);
    Task<List<RendezVous>> GetByUserIdAndDateAsync(int userId, DateTime date);
    Task<List<RendezVous>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<RendezVous> CreateAsync(RendezVous rendezVous);
    Task UpdateAsync(RendezVous rendezVous);
    Task DeleteAsync(int rendezVousId);
}

