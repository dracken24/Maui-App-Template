using MauiTemplate.Models;
using MauiTemplate.Services;
using MauiTemplate.Repositories;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;

namespace MauiTemplate.Pages;

/// <summary>
/// Page affichant un calendrier mensuel avec les rendez-vous.
/// Respecte le principe SRP (Single Responsibility Principle) : responsabilité unique d'affichage du calendrier.
/// Utilise l'injection de dépendances (DIP) via les interfaces IAuthService et IRendezVousRepository.
/// </summary>
public partial class CalendrierPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly IRendezVousRepository _rendezVousRepository;
    private DateTime _currentDate = DateTime.Today;
    private DateTime _selectedDate = DateTime.Today;
    private ObservableCollection<RendezVous> _rendezVousDuJour = new();

    public ObservableCollection<RendezVous> RendezVousDuJour => _rendezVousDuJour;

    /// <summary>
    /// Constructeur avec injection de dépendances (Dependency Inversion Principle).
    /// </summary>
    public CalendrierPage(IAuthService authService, IRendezVousRepository rendezVousRepository)
    {
        InitializeComponent();
        _authService = authService;
        _rendezVousRepository = rendezVousRepository;
        BindingContext = this;
    }

    /// <summary>
    /// Appelé lorsque la page apparaît à l'écran.
    /// Initialise le calendrier et charge les rendez-vous du jour.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            RendezVousCollectionView.ItemsSource = _rendezVousDuJour;
            BuildCalendar();
            await UpdateCalendarDisplay();
            await LoadRendezVousForDate(_currentDate);
        }
        catch (Exception)
        {
            // Log silencieux en cas d'erreur (pourrait être amélioré avec un service de logging)
        }
    }

    /// <summary>
    /// Construit la grille du calendrier avec 42 cellules (6 semaines x 7 jours).
    /// Chaque cellule est un Grid contenant un Label pour afficher le numéro du jour.
    /// </summary>
    private void BuildCalendar()
    {
        CalendarGrid.Clear();
        _dayLabels.Clear();
        _dayGrids.Clear();
        
        // Créer 42 cellules pour afficher un mois complet (6 semaines)
        for (int i = 0; i < 42; i++)
        {
            var grid = CreateCalendarCell(i);
            CalendarGrid.Add(grid, i % 7, i / 7);
        }
    }
    
    /// <summary>
    /// Crée une cellule du calendrier (Grid + Label) avec un gestionnaire de tap.
    /// </summary>
    private Grid CreateCalendarCell(int index)
    {
        var grid = new Grid
        {
            Margin = new Thickness(1),
            BackgroundColor = Colors.White
        };
        
        var label = new Label
        {
            Text = "",
            FontSize = 12,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Transparent
        };
        
        grid.Add(label);
        
        // Ajouter un gestionnaire de tap pour sélectionner une date
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => 
        {
            if (s is Grid g)
                DayButton_Click(g, e);
        };
        grid.GestureRecognizers.Add(tapGesture);
        
        grid.StyleId = ""; // Sera rempli avec la date lors de la mise à jour
        
        // Stocker les références pour accès rapide
        _dayLabels[index] = label;
        _dayGrids[index] = grid;
        
        return grid;
    }
    
    // Dictionnaires pour stocker les éléments
    private readonly Dictionary<int, Label> _dayLabels = new();
    private readonly Dictionary<int, Grid> _dayGrids = new();

    /// <summary>
    /// Met à jour l'affichage du calendrier (titre du mois et cellules).
    /// </summary>
    private async Task UpdateCalendarDisplay()
    {
        var culture = new System.Globalization.CultureInfo("fr-FR");
        CurrentMonthLabel.Text = _currentDate.ToString("MMMM yyyy", culture);
        await UpdateCalendarDays();
    }

    /// <summary>
    /// Met à jour les cellules du calendrier avec les dates et les rendez-vous.
    /// Applique les couleurs selon les priorités : date sélectionnée > aujourd'hui > dates avec rendez-vous.
    /// </summary>
    private async Task UpdateCalendarDays()
    {
        try
        {
            DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            DateTime lastDayOfPreviousMonth = firstDayOfMonth.AddDays(-1);
            int daysInPreviousMonth = DateTime.DaysInMonth(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month);

            HashSet<DateTime> datesWithRendezVous = await GetDatesWithRendezVous();
            
            // Toutes les modifications UI doivent être faites sur le thread principal
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                for (int i = 0; i < 42; i++)
                {
                    if (!_dayGrids.ContainsKey(i) || !_dayLabels.ContainsKey(i))
                    {
                        continue;
                    }
                        
                    Grid dayGrid = _dayGrids[i];
                    Label dayLabel = _dayLabels[i];
                    DateTime buttonDate = CalculateDateForCell(i, firstDayOfWeek, daysInMonth, daysInPreviousMonth, lastDayOfPreviousMonth);
                    
                    UpdateCalendarCell(dayGrid, dayLabel, buttonDate, datesWithRendezVous);
                }
            });
        }
        catch (Exception)
        {
            // Log silencieux en cas d'erreur (pourrait être amélioré avec un service de logging)
        }
    }
    
    /// <summary>
    /// Calcule la date correspondant à une cellule du calendrier.
    /// </summary>
    private DateTime CalculateDateForCell(int index, int firstDayOfWeek, int daysInMonth, int daysInPreviousMonth, DateTime lastDayOfPreviousMonth)
    {
        if (index < firstDayOfWeek)
        {
            // Jours du mois précédent
            int day = daysInPreviousMonth - firstDayOfWeek + index + 1;
            return new DateTime(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month, day);
        }
        else if (index >= firstDayOfWeek && index < firstDayOfWeek + daysInMonth)
        {
            // Jours du mois actuel
            int day = index - firstDayOfWeek + 1;
            return new DateTime(_currentDate.Year, _currentDate.Month, day);
        }
        else
        {
            // Jours du mois suivant
            int day = index - firstDayOfWeek - daysInMonth + 1;
            return new DateTime(_currentDate.AddMonths(1).Year, _currentDate.AddMonths(1).Month, day);
        }
    }
    
    /// <summary>
    /// Met à jour l'affichage d'une cellule du calendrier (texte, couleurs).
    /// </summary>
    private void UpdateCalendarCell(Grid dayGrid, Label dayLabel, DateTime buttonDate, HashSet<DateTime> datesWithRendezVous)
    {
        dayGrid.StyleId = buttonDate.ToString("yyyy-MM-dd");
        
        // Jours du mois précédent ou suivant : affichage en gris
        if (buttonDate.Month != _currentDate.Month)
        {
            dayLabel.Text = buttonDate.Day.ToString();
            dayLabel.TextColor = Colors.LightGray;
            dayGrid.BackgroundColor = Colors.Transparent;
            return;
        }
        
        // Jours du mois actuel
        dayLabel.Text = buttonDate.Day.ToString();
        DateTime normalizedDate = buttonDate.Date;
        bool hasRendezVous = datesWithRendezVous.Contains(normalizedDate);
        
        // Déterminer les couleurs selon les priorités
        var (backgroundColor, textColor) = GetCellColors(buttonDate, hasRendezVous);
        
        dayGrid.BackgroundColor = backgroundColor;
        dayLabel.TextColor = textColor;
        dayGrid.InvalidateMeasure();
    }
    
    /// <summary>
    /// Détermine les couleurs d'une cellule selon les priorités :
    /// 1. Date sélectionnée (bleu clair)
    /// 2. Aujourd'hui avec rendez-vous (vert) ou sans rendez-vous (jaune)
    /// 3. Date avec rendez-vous (orange)
    /// 4. Date normale (blanc)
    /// </summary>
    private (Color backgroundColor, Color textColor) GetCellColors(DateTime buttonDate, bool hasRendezVous)
    {
        if (buttonDate.Date == _selectedDate.Date)
        {
            // Date sélectionnée - priorité la plus haute
            return (Color.FromArgb("#ADD8E6"), Colors.Black);
        }
        else if (buttonDate.Date == DateTime.Today.Date)
        {
            // Date d'aujourd'hui
            if (hasRendezVous)
            {
                return (Color.FromArgb("#4CAF50"), Colors.White); // Vert vif
            }
            else
            {
                return (Color.FromArgb("#FFEB3B"), Colors.Black); // Jaune vif
            }
        }
        else if (hasRendezVous)
        {
            // Date avec rendez-vous
            return (Color.FromArgb("#FF6B35"), Colors.White); // Orange vif
        }
        else
        {
            // Date normale
            return (Colors.White, Colors.Black);
        }
    }

    /// <summary>
    /// Récupère toutes les dates du mois actuel qui ont des rendez-vous.
    /// </summary>
    /// <returns>HashSet contenant les dates avec rendez-vous pour le mois affiché.</returns>
    private async Task<HashSet<DateTime>> GetDatesWithRendezVous()
    {
        if (_authService.CurrentUser == null)
        {
            return new HashSet<DateTime>();
        }

        try
        {
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            
            var allRendezVous = await _rendezVousRepository.GetByUserIdAsync(_authService.CurrentUser.Id);
            
            return allRendezVous
                .Where(r => 
                {
                    var dateDebut = r.DateDebut.Date;
                    return dateDebut >= firstDayOfMonth.Date && dateDebut <= lastDayOfMonth.Date;
                })
                .Select(r => r.DateDebut.Date)
                .Distinct()
                .ToHashSet();
        }
        catch (Exception)
        {
            return new HashSet<DateTime>();
        }
    }

    /// <summary>
    /// Gère le clic sur une cellule du calendrier.
    /// Sélectionne la date et charge les rendez-vous correspondants.
    /// </summary>
    private async void DayButton_Click(object sender, EventArgs e)
    {
        if (sender is Grid grid && !string.IsNullOrEmpty(grid.StyleId))
        {
            if (DateTime.TryParse(grid.StyleId, out DateTime date))
            {
                _selectedDate = date;
                await UpdateCalendarDays();
                await LoadRendezVousForDate(date);
            }
        }
    }

    /// <summary>
    /// Charge les rendez-vous pour une date spécifique.
    /// Met à jour la CollectionView et les statistiques.
    /// </summary>
    private async Task LoadRendezVousForDate(DateTime date)
    {
        if (_authService.CurrentUser == null)
        {
            return;
        }

        try
        {
            var rendezVous = await _rendezVousRepository.GetByUserIdAndDateAsync(_authService.CurrentUser.Id, date);
            
            _rendezVousDuJour.Clear();
            foreach (RendezVous rdv in rendezVous)
            {
                _rendezVousDuJour.Add(rdv);
            }

            // Forcer le rafraîchissement de la CollectionView sur le thread principal
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NoRendezVousLabel.IsVisible = rendezVous.Count == 0;
                RendezVousCollectionView.IsVisible = rendezVous.Count > 0;
                
                // Forcer le rafraîchissement en réinitialisant l'ItemsSource
                RendezVousCollectionView.ItemsSource = null;
                RendezVousCollectionView.ItemsSource = _rendezVousDuJour;
            });

            UpdateStats(rendezVous.Count);
        }
        catch (Exception)
        {
            // Log silencieux en cas d'erreur (pourrait être amélioré avec un service de logging)
        }
    }

    /// <summary>
    /// Met à jour le label des statistiques avec le nombre de rendez-vous.
    /// </summary>
    private void UpdateStats(int count)
    {
        if (count == 0)
        {
            StatsLabel.Text = "Aucun rendez-vous aujourd'hui";
        }
        else if (count == 1)
        {
            StatsLabel.Text = "1 rendez-vous aujourd'hui";
        }
        else
        {
            StatsLabel.Text = $"{count} rendez-vous aujourd'hui";
        }
    }


    /// <summary>
    /// Gère le clic sur le bouton "Mois précédent".
    /// Change le mois affiché et réinitialise la sélection si nécessaire.
    /// </summary>
    private async void OnPrevMonthClicked(object sender, EventArgs e)
    {
        _currentDate = _currentDate.AddMonths(-1);
        
        // Réinitialiser la date sélectionnée au premier jour du nouveau mois si elle n'est plus dans le mois
        if (_selectedDate.Month != _currentDate.Month || _selectedDate.Year != _currentDate.Year)
        {
            _selectedDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        }
        
        await UpdateCalendarDisplay();
        await LoadRendezVousForDate(_selectedDate);
    }

    /// <summary>
    /// Gère le clic sur le bouton "Mois suivant".
    /// Change le mois affiché et réinitialise la sélection si nécessaire.
    /// </summary>
    private async void OnNextMonthClicked(object sender, EventArgs e)
    {
        _currentDate = _currentDate.AddMonths(1);
        
        // Réinitialiser la date sélectionnée au premier jour du nouveau mois si elle n'est plus dans le mois
        if (_selectedDate.Month != _currentDate.Month || _selectedDate.Year != _currentDate.Year)
        {
            _selectedDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        }
        
        await UpdateCalendarDisplay();
        await LoadRendezVousForDate(_selectedDate);
    }

    /// <summary>
    /// Gère le clic sur le bouton "Ajouter un rendez-vous".
    /// Passe la date sélectionnée à la page de création et navigue vers celle-ci.
    /// </summary>
    private async void OnAddRendezVousClicked(object sender, EventArgs e)
    {
        CreateRendezVousPage.SelectedDate = _selectedDate;
        await Shell.Current.GoToAsync("createRendezVous");
    }
}

