using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MauiTemplate.Data;
using MauiTemplate.Services;

namespace MauiTemplate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configuration de la base de données
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "ApplicationBureau.db")}"));

            // Enregistrement des services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<AuthService>();

            // Enregistrement des pages
            builder.Services.AddTransient<Pages.LoginPage>();
            builder.Services.AddTransient<Pages.RegisterPage>();
            builder.Services.AddTransient<Pages.AccueilPage>();
            builder.Services.AddTransient<Pages.CalendrierPage>();
            builder.Services.AddTransient<Pages.FonctionnalitesPage>();
            builder.Services.AddTransient<Pages.AProposPage>();
            builder.Services.AddTransient<Pages.ParametresPage>();
            builder.Services.AddTransient<Pages.CreateRendezVousPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            
            // Initialiser la base de données de manière synchrone au démarrage
            try
            {
                var databaseService = app.Services.GetService<DatabaseService>();
                if (databaseService != null)
                {
                    // Initialisation synchrone pour éviter les problèmes de timing
                    databaseService.InitializeAsync().GetAwaiter().GetResult();
                    System.Diagnostics.Debug.WriteLine("Base de données initialisée avec succès");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur d'initialisation de la base de données: {ex.Message}");
                // Ne pas faire échouer l'application si la DB ne peut pas être initialisée
            }

            return app;
        }
    }
}
