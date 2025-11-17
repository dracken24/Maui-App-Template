using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class ParametresPage : ContentPage
{
    private readonly AuthService _authService;

    public ParametresPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();
        LoadSystemInfo();
    }

    private void LoadUserInfo()
    {
        if (_authService.CurrentUser != null)
        {
            Models.User user = _authService.CurrentUser;
            UsernameLabel.Text = user.Username;
            EmailLabel.Text = user.Email;
            NomLabel.Text = user.Nom ?? "Non renseigné";
            PrenomLabel.Text = user.Prenom ?? "Non renseigné";
            TelephoneLabel.Text = user.Telephone ?? "Non renseigné";
            LastLoginLabel.Text = user.DerniereConnexion?.ToString("dd/MM/yyyy HH:mm") ?? "Jamais";
        }
    }

    private void LoadSystemInfo()
    {
        PlatformLabel.Text = DeviceInfo.Platform.ToString();
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de modification de profil à implémenter", "OK");
    }

    private void OnNotificationsToggled(object sender, ToggledEventArgs e)
    {
        // Implémenter la logique de notifications
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        // Implémenter le mode sombre
    }

    private void OnAutoSyncToggled(object sender, ToggledEventArgs e)
    {
        // Implémenter la synchronisation automatique
    }

    private async void OnRefreshDataClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Données actualisées", "OK");
    }

    private async void OnBackupDataClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de sauvegarde à implémenter", "OK");
    }

    private async void OnClearCacheClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Confirmation", 
            "Êtes-vous sûr de vouloir effacer le cache ?", 
            "Oui", "Non");

        if (result)
        {
            await DisplayAlert("Information", "Cache effacé", "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Confirmation", 
            "Êtes-vous sûr de vouloir vous déconnecter ?", 
            "Oui", "Non");

        if (result)
        {
            _authService.Logout();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
