using MauiTemplate.Models;
using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class CreateRendezVousPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;

    public CreateRendezVousPage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();
        _authService = authService;
        _databaseService = databaseService;
        
        // Initialiser les dates avec des valeurs valides
        DateDebutPicker.Date = DateTime.Today;
        DateFinPicker.Date = DateTime.Today;
        HeureDebutPicker.Time = new TimeSpan(9, 0, 0); // 09:00
        HeureFinPicker.Time = new TimeSpan(10, 0, 0);  // 10:00
        
        // Initialiser le statut par défaut
        StatutPicker.SelectedIndex = 0; // "Confirmé"
    }

    private async void OnCreerClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(TitreEntry.Text))
        {
            await DisplayAlert("Erreur", "Le titre est obligatoire", "OK");
            return;
        }

        try
        {
            // Créer les dates avec validation
            DateTime dateDebut = DateDebutPicker.Date.Date.Add(HeureDebutPicker.Time);
            DateTime dateFin = DateFinPicker.Date.Date.Add(HeureFinPicker.Time);

            // Vérifier que la date de fin est après la date de début
            if (dateFin <= dateDebut)
            {
                await DisplayAlert("Erreur", "La date de fin doit être après la date de début", "OK");
                return;
            }

            // Vérifier que l'utilisateur est connecté
            if (_authService.CurrentUser == null)
            {
                await DisplayAlert("Erreur", "Vous devez être connecté pour créer un rendez-vous", "OK");
                return;
            }

            // Créer le rendez-vous
            RendezVous rendezVous = new RendezVous
            {
                Titre = TitreEntry.Text.Trim(),
                Description = DescriptionEntry.Text?.Trim(),
                DateDebut = dateDebut,
                DateFin = dateFin,
                Lieu = LieuEntry.Text?.Trim(),
                Client = ClientEntry.Text?.Trim(),
                Statut = StatutPicker.SelectedItem?.ToString() ?? "Confirmé",
                UserId = _authService.CurrentUser.Id
            };

            // Sauvegarder
            await _databaseService.CreateRendezVousAsync(rendezVous);
            
            await DisplayAlert("Succès", "Rendez-vous créé avec succès !", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la création : {ex.Message}", "OK");
        }
    }

    private async void OnAnnulerClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
