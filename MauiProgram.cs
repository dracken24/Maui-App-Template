using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MauiTemplate.Data;
using MauiTemplate.Services;
using MauiTemplate.Repositories;

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

            // Enregistrement des repositories (Scoped pour correspondre au DbContext)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRendezVousRepository, RendezVousRepository>();

            // Enregistrement des services
            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

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
                using var scope = app.Services.CreateScope();
                var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                databaseInitializer.InitializeAsync().GetAwaiter().GetResult();
                System.Diagnostics.Debug.WriteLine("Base de données initialisée avec succès");
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
