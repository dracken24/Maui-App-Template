using MauiTemplate.Models;
using MauiTemplate.Services;
using System.Collections.ObjectModel;

namespace MauiTemplate.Pages;

public partial class CalendrierPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;
    private DateTime _currentDate = DateTime.Today;
    private DateTime _selectedDate = DateTime.Today;
    private ObservableCollection<RendezVous> _rendezVousDuJour = new();

    public ObservableCollection<RendezVous> RendezVousDuJour => _rendezVousDuJour;

    public CalendrierPage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();
        _authService = authService;
        _databaseService = databaseService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            BuildCalendar();
            UpdateCalendarDisplay();
            await LoadRendezVousForDate(_currentDate);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur dans OnAppearing: {ex.Message}");
        }
    }

    private void BuildCalendar()
    {
        CalendarGrid.Clear();
        
        // Créer 42 boutons (6 semaines x 7 jours)
        for (int i = 0; i < 42; i++)
        {
            var dayButton = new Button
            {
                Margin = new Thickness(1),
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.LightGray,
                BorderWidth = 1,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            
            dayButton.Clicked += DayButton_Click;
            CalendarGrid.Add(dayButton, i % 7, i / 7);
        }
    }

    private void UpdateCalendarDisplay()
    {
        var culture = new System.Globalization.CultureInfo("fr-FR");
        CurrentMonthLabel.Text = _currentDate.ToString("MMMM yyyy", culture);
        UpdateCalendarDays();
    }

    private async void UpdateCalendarDays()
    {
        try
        {
            // Obtenir le premier jour du mois
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            
            // Obtenir le jour de la semaine du premier jour (0 = Dimanche, 1 = Lundi, etc.)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            
            // Obtenir le nombre de jours dans le mois
            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            
            // Obtenir le dernier jour du mois précédent
            var lastDayOfPreviousMonth = firstDayOfMonth.AddDays(-1);
            int daysInPreviousMonth = DateTime.DaysInMonth(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month);

            // Obtenir les dates avec des rendez-vous pour le mois actuel
            var datesWithRendezVous = await GetDatesWithRendezVous();

        // Mettre à jour chaque bouton
        for (int i = 0; i < 42; i++)
        {
            var dayButton = (Button)CalendarGrid.Children[i];
            
            // Calculer la date correspondante
            DateTime buttonDate;
            if (i < firstDayOfWeek)
            {
                // Jours du mois précédent
                int day = daysInPreviousMonth - firstDayOfWeek + i + 1;
                buttonDate = new DateTime(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month, day);
                dayButton.Text = day.ToString();
                dayButton.TextColor = Colors.LightGray;
                dayButton.BackgroundColor = Colors.Transparent;
            }
            else if (i >= firstDayOfWeek && i < firstDayOfWeek + daysInMonth)
            {
                // Jours du mois actuel
                int day = i - firstDayOfWeek + 1;
                buttonDate = new DateTime(_currentDate.Year, _currentDate.Month, day);
                dayButton.Text = day.ToString();
                
                // Vérifier si la date a des rendez-vous
                bool hasRendezVous = datesWithRendezVous.Contains(buttonDate.Date);
                
                // Mettre en surbrillance selon les priorités
                if (buttonDate.Date == _selectedDate.Date)
                {
                    // Date sélectionnée - priorité la plus haute
                    dayButton.BackgroundColor = Color.FromArgb("#ADD8E6"); // LightBlue
                    dayButton.BorderColor = Colors.Blue;
                    dayButton.TextColor = Colors.Black;
                }
                else if (buttonDate.Date == DateTime.Today.Date)
                {
                    // Date d'aujourd'hui
                    if (hasRendezVous)
                    {
                        dayButton.BackgroundColor = Color.FromArgb("#4CAF50"); // Vert vif
                        dayButton.BorderColor = Color.FromArgb("#2E7D32"); // Vert foncé
                        dayButton.TextColor = Colors.White;
                    }
                    else
                    {
                        dayButton.BackgroundColor = Color.FromArgb("#FFEB3B"); // Jaune vif
                        dayButton.BorderColor = Color.FromArgb("#F57F17"); // Jaune foncé
                        dayButton.TextColor = Colors.Black;
                    }
                }
                else if (hasRendezVous)
                {
                    // Date avec rendez-vous
                    dayButton.BackgroundColor = Color.FromArgb("#FF6B35"); // Orange vif
                    dayButton.BorderColor = Color.FromArgb("#E55A2B"); // Orange foncé
                    dayButton.TextColor = Colors.White;
                }
                else
                {
                    // Date normale
                    dayButton.BackgroundColor = Colors.White;
                    dayButton.BorderColor = Colors.LightGray;
                    dayButton.TextColor = Colors.Black;
                }
            }
            else
            {
                // Jours du mois suivant
                int day = i - firstDayOfWeek - daysInMonth + 1;
                buttonDate = new DateTime(_currentDate.AddMonths(1).Year, _currentDate.AddMonths(1).Month, day);
                dayButton.Text = day.ToString();
                dayButton.TextColor = Colors.LightGray;
                dayButton.BackgroundColor = Colors.Transparent;
            }
            
            dayButton.ClassId = buttonDate.ToString("yyyy-MM-dd"); // Stocker la date dans ClassId
        }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur dans UpdateCalendarDays: {ex.Message}");
        }
    }

    private async Task<HashSet<DateTime>> GetDatesWithRendezVous()
    {
        if (_authService.CurrentUser == null) return new HashSet<DateTime>();

        // Obtenir le premier et dernier jour du mois affiché
        var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        // Récupérer toutes les dates avec des rendez-vous pour ce mois
        var allRendezVous = await _databaseService.GetRendezVousByUserIdAsync(_authService.CurrentUser.Id);
        var datesWithRendezVous = allRendezVous
            .Where(r => r.DateDebut.Date >= firstDayOfMonth.Date && 
						r.DateDebut.Date <= lastDayOfMonth.Date)
            .Select(r => r.DateDebut.Date)
            .Distinct()
            .ToHashSet();
            
        return datesWithRendezVous;
    }

    private async void DayButton_Click(object sender, EventArgs e)
    {
        if (sender is Button button && !string.IsNullOrEmpty(button.ClassId))
        {
            if (DateTime.TryParse(button.ClassId, out DateTime date))
            {
                _selectedDate = date;
                UpdateCalendarDays();
                await LoadRendezVousForDate(date);
            }
        }
    }

    private async Task LoadRendezVousDuJour()
    {
        await LoadRendezVousForDate(DateTime.Today);
    }

    private async Task LoadRendezVousForDate(DateTime date)
    {
        if (_authService.CurrentUser == null) return;

        try
        {
            var rendezVous = await _databaseService.GetRendezVousByDateAsync(_authService.CurrentUser.Id, date);
            
            _rendezVousDuJour.Clear();
            foreach (var rdv in rendezVous)
            {
                _rendezVousDuJour.Add(rdv);
            }

            // Afficher/masquer le message "Aucun rendez-vous"
            NoRendezVousLabel.IsVisible = rendezVous.Count == 0;
            RendezVousCollectionView.IsVisible = rendezVous.Count > 0;

            // Mettre à jour les statistiques
            UpdateStats(rendezVous.Count);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des rendez-vous : {ex.Message}");
        }
    }

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


    private async void OnPrevMonthClicked(object sender, EventArgs e)
    {
        _currentDate = _currentDate.AddMonths(-1);
        UpdateCalendarDisplay();
    }

    private async void OnNextMonthClicked(object sender, EventArgs e)
    {
        _currentDate = _currentDate.AddMonths(1);
        UpdateCalendarDisplay();
    }

    private async void OnAddRendezVousClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("createRendezVous");
    }
}
