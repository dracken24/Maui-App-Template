using MauiTemplate.Data;
using MauiTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiTemplate.Repositories;

/// <summary>
/// Repository pour la gestion des rendez-vous
/// Respecte le principe de responsabilité unique (SRP)
/// </summary>
public class RendezVousRepository : IRendezVousRepository
{
    private readonly ApplicationDbContext _context;

    public RendezVousRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RendezVous?> GetByIdAsync(int id)
    {
        return await _context.RendezVous.FindAsync(id);
    }

    public async Task<List<RendezVous>> GetByUserIdAsync(int userId)
    {
        return await _context.RendezVous
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.DateDebut)
            .ToListAsync();
    }

    public async Task<List<RendezVous>> GetByUserIdAndDateAsync(int userId, DateTime date)
    {
        var dateStart = date.Date;
        var dateEnd = dateStart.AddDays(1).AddTicks(-1);
        
        return await _context.RendezVous
            .Where(r => r.UserId == userId && 
                       r.DateDebut >= dateStart && 
                       r.DateDebut < dateEnd)
            .OrderBy(r => r.DateDebut)
            .ToListAsync();
    }

    public async Task<List<RendezVous>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.RendezVous
            .Where(r => r.UserId == userId && 
                       r.DateDebut >= startDate && 
                       r.DateDebut <= endDate)
            .OrderBy(r => r.DateDebut)
            .ToListAsync();
    }

    public async Task<RendezVous> CreateAsync(RendezVous rendezVous)
    {
        _context.RendezVous.Add(rendezVous);
        await _context.SaveChangesAsync();
        return rendezVous;
    }

    public async Task UpdateAsync(RendezVous rendezVous)
    {
        _context.RendezVous.Update(rendezVous);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int rendezVousId)
    {
        var rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
        if (rendezVous != null)
        {
            _context.RendezVous.Remove(rendezVous);
            await _context.SaveChangesAsync();
        }
    }
}

