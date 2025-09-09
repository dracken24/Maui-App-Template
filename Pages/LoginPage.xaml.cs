using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Text = "Connexion...";

        try
        {
            var success = await _authService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
            
            if (success)
            {
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await DisplayAlert("Erreur", "Nom d'utilisateur ou mot de passe incorrect", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Se connecter";
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("register");
    }
}
