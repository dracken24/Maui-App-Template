using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class AccueilPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;

    public AccueilPage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();
		
        _authService = authService;
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserInfo();
        await LoadStatistics();
    }

    private Task LoadUserInfo()
    {
        if (_authService.CurrentUser != null)
        {
            Models.User? user = _authService.CurrentUser;
            string displayName = !string.IsNullOrEmpty(user.Prenom) && !string.IsNullOrEmpty(user.Nom) 
                ? $"{user.Prenom} {user.Nom}" 
                : user.Username;
            
            WelcomeLabel.Text = $"Bonjour {displayName} !";
        }
        return Task.CompletedTask;
    }

    private async Task LoadStatistics()
    {
        if (_authService.CurrentUser != null)
        {
            try
            {
                List<Models.RendezVous> allRendezVous = await _databaseService.GetRendezVousByUserIdAsync(_authService.CurrentUser.Id);
                List<Models.RendezVous> todayRendezVous = await _databaseService.GetRendezVousByDateAsync(_authService.CurrentUser.Id, DateTime.Today);

                TotalRendezVousLabel.Text = allRendezVous.Count.ToString();
                RendezVousAujourdhuiLabel.Text = todayRendezVous.Count.ToString();
            }
            catch (Exception)
            {
                // En cas d'erreur, afficher 0
                TotalRendezVousLabel.Text = "0";
                RendezVousAujourdhuiLabel.Text = "0";
            }
        }
    }

    // private async void OnCalendrierClicked(object sender, EventArgs e)
    // {
    //     await Shell.Current.GoToAsync("calendrier");
    // }

    private async void OnFonctionnalitesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("fonctionnalites");
    }

    // private async void OnNouveauRendezVousClicked(object sender, EventArgs e)
    // {
    //     await Shell.Current.GoToAsync("calendrier");
    // }

    private async void OnParametresClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("parametres");
    }
}
