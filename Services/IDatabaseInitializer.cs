namespace MauiTemplate.Services;

/// <summary>
/// Interface pour l'initialisation de la base de données
/// </summary>
public interface IDatabaseInitializer
{
    Task InitializeAsync();
}

