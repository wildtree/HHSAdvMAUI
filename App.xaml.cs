
namespace HHSAdvMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.SizeChanged += (s, e) =>
            {
                Preferences.Set("Width", (int)window.Width);
                Preferences.Set("Height", (int)window.Height);
            };

            window.Width = Preferences.Get("Width", 800);
            window.Height = Preferences.Get("Height", 600);

            return window;
        }
    }
}
