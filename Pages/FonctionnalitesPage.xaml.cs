using MauiTemplate.Services;

namespace MauiTemplate.Pages;

public partial class FonctionnalitesPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly DatabaseService _databaseService;

    public FonctionnalitesPage(AuthService authService, DatabaseService databaseService)
    {
        InitializeComponent();
        _authService = authService;
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatistics();
    }

    private async Task LoadStatistics()
    {
        try
        {
            List<Models.User> users = await _databaseService.GetAllUsersAsync();
            int totalEvents = users.Sum(u => u.RendezVous.Count);

            TotalUsersLabel.Text = users.Count.ToString();
            TotalEventsLabel.Text = totalEvents.ToString();
        }
        catch (Exception)
        {
            TotalUsersLabel.Text = "0";
            TotalEventsLabel.Text = "0";
        }
    }

    private async void OnCreateUserClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de création d'utilisateur à implémenter", "OK");
    }

    private async void OnViewUsersClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de visualisation des utilisateurs à implémenter", "OK");
    }

    private async void OnCreateEventClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de création d'événement à implémenter", "OK");
    }

    private async void OnViewEventsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité de visualisation des événements à implémenter", "OK");
    }

    private async void OnCleanDatabaseClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Confirmation", 
            "Êtes-vous sûr de vouloir nettoyer la base de données ? Cette action est irréversible.", 
            "Oui", "Non");

        if (result)
        {
            await DisplayAlert("Information", "Fonctionnalité de nettoyage de base de données à implémenter", "OK");
        }
    }

    private async void OnExportDataClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Information", "Fonctionnalité d'export des données à implémenter", "OK");
    }
}
