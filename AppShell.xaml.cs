namespace MauiTemplate
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();
                
                // Enregistrer les routes
                Routing.RegisterRoute("login", typeof(Pages.LoginPage));
                Routing.RegisterRoute("register", typeof(Pages.RegisterPage));
                Routing.RegisterRoute("main", typeof(Pages.AccueilPage));
                Routing.RegisterRoute("accueil", typeof(Pages.AccueilPage));
                Routing.RegisterRoute("calendrier", typeof(Pages.CalendrierPage));
                Routing.RegisterRoute("fonctionnalites", typeof(Pages.FonctionnalitesPage));
                Routing.RegisterRoute("parametres", typeof(Pages.ParametresPage));
                Routing.RegisterRoute("apropos", typeof(Pages.AProposPage));
                Routing.RegisterRoute("createRendezVous", typeof(Pages.CreateRendezVousPage));
                
                System.Diagnostics.Debug.WriteLine("AppShell initialisé avec succès");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans AppShell: {ex.Message}");
                throw;
            }
        }
    }
}
