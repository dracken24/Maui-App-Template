using MauiTemplate.Models;
using MauiTemplate.Services;
using MauiTemplate.Repositories;

namespace MauiTemplate.Pages;

/// <summary>
/// Page permettant de créer un nouveau rendez-vous.
/// Respecte le principe SRP (Single Responsibility Principle) : responsabilité unique de création de rendez-vous.
/// Utilise l'injection de dépendances (DIP) via les interfaces IAuthService et IRendezVousRepository.
/// </summary>
public partial class CreateRendezVousPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly IRendezVousRepository _rendezVousRepository;
    
    /// <summary>
    /// Variable statique pour recevoir la date sélectionnée depuis le calendrier.
    /// Permet de passer la date sans utiliser de query string (qui ne fonctionne pas avec les routes relatives dans MAUI Shell).
    /// </summary>
    public static DateTime? SelectedDate { get; set; }
    
    /// <summary>
    /// Variables pour stocker les dates sélectionnées dans les DatePicker.
    /// Nécessaire car sur Android, la propriété DatePicker.Date peut ne pas être mise à jour correctement.
    /// On utilise donc l'événement DateSelected pour capturer la date sélectionnée.
    /// </summary>
    private DateTime _dateDebutSelectionnee = DateTime.Today;
    private DateTime _dateFinSelectionnee = DateTime.Today;

    /// <summary>
    /// Constructeur avec injection de dépendances (Dependency Inversion Principle).
    /// </summary>
    public CreateRendezVousPage(IAuthService authService, IRendezVousRepository rendezVousRepository)
    {
        InitializeComponent();
        _authService = authService;
        _rendezVousRepository = rendezVousRepository;
        
        InitializeDefaultValues();
        SubscribeToDatePickerEvents();
    }
    
    /// <summary>
    /// Initialise les valeurs par défaut des contrôles de formulaire.
    /// </summary>
    private void InitializeDefaultValues()
    {
        DateDebutPicker.Date = DateTime.Today;
        DateFinPicker.Date = DateTime.Today;
        _dateDebutSelectionnee = DateTime.Today;
        _dateFinSelectionnee = DateTime.Today;
        HeureDebutPicker.Time = new TimeSpan(9, 0, 0);
        HeureFinPicker.Time = new TimeSpan(10, 0, 0);
        StatutPicker.SelectedIndex = 0; // "Confirmé"
    }
    
    /// <summary>
    /// S'abonne aux événements DateSelected des DatePicker.
    /// Sur Android, DatePicker.Date peut ne pas être mis à jour, donc on utilise DateSelected pour capturer la date.
    /// </summary>
    private void SubscribeToDatePickerEvents()
    {
        DateDebutPicker.DateSelected += (s, e) => 
        {
            _dateDebutSelectionnee = e.NewDate;
        };
        
        DateFinPicker.DateSelected += (s, e) => 
        {
            _dateFinSelectionnee = e.NewDate;
        };
    }
    
    /// <summary>
    /// Appelé lorsque la page apparaît à l'écran.
    /// Récupère la date sélectionnée depuis le calendrier si elle a été passée.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (SelectedDate.HasValue)
        {
            DateTime dateParam = SelectedDate.Value;
            
            // Mettre à jour les DatePicker avec la date reçue depuis le calendrier
            DateDebutPicker.Date = dateParam;
            DateFinPicker.Date = dateParam;
            _dateDebutSelectionnee = dateParam;
            _dateFinSelectionnee = dateParam;
            
            // Réinitialiser la variable statique après utilisation
            SelectedDate = null;
        }
    }

    /// <summary>
    /// Gère le clic sur le bouton "Créer".
    /// Valide les données, crée le rendez-vous et le sauvegarde dans la base de données.
    /// </summary>
    private async void OnCreerClicked(object sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        try
        {
            DateTime dateDebut = BuildDateTime(_dateDebutSelectionnee, HeureDebutPicker.Time);
            DateTime dateFin = BuildDateTime(_dateFinSelectionnee, HeureFinPicker.Time);

            if (!await ValidateDates(dateDebut, dateFin))
            {
                return;
            }

            if (_authService.CurrentUser == null)
            {
                await DisplayAlert("Erreur", "Vous devez être connecté pour créer un rendez-vous", "OK");
                return;
            }

            RendezVous rendezVous = CreateRendezVous(dateDebut, dateFin);
            await _rendezVousRepository.CreateAsync(rendezVous);
            
            await DisplayAlert("Succès", "Rendez-vous créé avec succès !", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la création : {ex.Message}", "OK");
        }
    }
    
    /// <summary>
    /// Valide le formulaire avant la soumission.
    /// </summary>
    /// <returns>True si le formulaire est valide, false sinon.</returns>
    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(TitreEntry.Text))
        {
            DisplayAlert("Erreur", "Le titre est obligatoire", "OK");
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Construit une DateTime complète à partir d'une date et d'une heure.
    /// </summary>
    private DateTime BuildDateTime(DateTime date, TimeSpan time)
    {
        return date.Date.Add(time);
    }
    
    /// <summary>
    /// Valide que la date de fin est après la date de début.
    /// </summary>
    private async Task<bool> ValidateDates(DateTime dateDebut, DateTime dateFin)
    {
        if (dateFin <= dateDebut)
        {
            await DisplayAlert("Erreur", "La date de fin doit être après la date de début", "OK");
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Crée un objet RendezVous à partir des données du formulaire.
    /// </summary>
    private RendezVous CreateRendezVous(DateTime dateDebut, DateTime dateFin)
    {
        return new RendezVous
        {
            Titre = TitreEntry.Text.Trim(),
            Description = DescriptionEntry.Text?.Trim(),
            DateDebut = dateDebut,
            DateFin = dateFin,
            Lieu = LieuEntry.Text?.Trim(),
            Client = ClientEntry.Text?.Trim(),
            Statut = StatutPicker.SelectedItem?.ToString() ?? "Confirmé",
            UserId = _authService.CurrentUser!.Id
        };
    }

    /// <summary>
    /// Gère le clic sur le bouton "Annuler".
    /// Retourne à la page précédente sans sauvegarder.
    /// </summary>
    private async void OnAnnulerClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
