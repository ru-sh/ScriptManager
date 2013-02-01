using System.Globalization;
using System.Threading;
using System.Windows;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Settings AppSettings { get; private set; }

        public App()
        {
            this.Startup += App_Startup;
            this.Exit += App_Exit;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            var s = new Settings();
            AppSettings = s;
            var c = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = c;
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            AppSettings.Save();
        }
    }
}
