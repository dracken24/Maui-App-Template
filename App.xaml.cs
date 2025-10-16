namespace MauiTemplate
{
	public partial class App : Application
	{
		public App()
		{
			try
			{
				InitializeComponent();
				System.Diagnostics.Debug.WriteLine("App initialisé avec succès");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Erreur dans App: {ex.Message}");
				throw;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			System.Diagnostics.Debug.WriteLine("App démarré");
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			try
			{
				var window = new Window(new AppShell());
				
				// Définir la taille par défaut pour Windows
			#if WINDOWS
				window.Width = 1200;
				window.Height = 800;
				window.MinimumWidth = 800;
				window.MinimumHeight = 600;
			#endif
				
				System.Diagnostics.Debug.WriteLine("Window créée avec succès");
				return window;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Erreur lors de la création de la Window: {ex.Message}");
				throw;
			}
		}
	}
}