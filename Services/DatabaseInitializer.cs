using MauiTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace MauiTemplate.Services;

/// <summary>
/// Service d'initialisation de la base de données
/// Respecte le principe de responsabilité unique (SRP)
/// </summary>
public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ApplicationDbContext _context;

    public DatabaseInitializer(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }
}

