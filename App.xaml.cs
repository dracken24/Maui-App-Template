using MauiTemplate.Services;

namespace MauiTemplate
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

			protected override void OnStart()
			{
				base.OnStart();
			}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			var window = new Window(new AppShell());
			
			// Définir la taille par défaut pour Windows
	#if WINDOWS
			window.Width = 1200;
			window.Height = 800;
			window.MinimumWidth = 800;
			window.MinimumHeight = 600;
	#endif
			
			return window;
		}
	}
}