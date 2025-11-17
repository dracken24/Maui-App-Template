using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;

    public RegisterPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Validation des champs obligatoires
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || 
            string.IsNullOrWhiteSpace(EmailEntry.Text) || 
            string.IsNullOrWhiteSpace(PasswordEntry.Text) || 
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs obligatoires", "OK");
            return;
        }

        // Validation de l'email
        if (!IsValidEmail(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer une adresse email valide", "OK");
            return;
        }

        // Validation de la confirmation du mot de passe
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }

        // Validation de la longueur du mot de passe
        if (PasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères", "OK");
            return;
        }

        RegisterButton.IsEnabled = false;
        RegisterButton.Text = "Création...";

        try
        {
            bool success = await _authService.RegisterAsync(
                UsernameEntry.Text,
                EmailEntry.Text,
                PasswordEntry.Text,
                string.IsNullOrWhiteSpace(NomEntry.Text) ? null : NomEntry.Text,
                string.IsNullOrWhiteSpace(PrenomEntry.Text) ? null : PrenomEntry.Text,
                string.IsNullOrWhiteSpace(TelephoneEntry.Text) ? null : TelephoneEntry.Text
            );

            if (success)
            {
                await DisplayAlert("Succès", "Compte créé avec succès !", "OK");
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await DisplayAlert("Erreur", "Ce nom d'utilisateur ou cette adresse email est déjà utilisé", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
        finally
        {
            RegisterButton.IsEnabled = true;
            RegisterButton.Text = "Créer le compte";
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
