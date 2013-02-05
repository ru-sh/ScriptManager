using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ScriptCommander.Annotations;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for ScriptViewer.xaml
    /// </summary>
    public partial class ScriptViewer
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ISubject<string> _standartOutput = new Subject<string>();
        private readonly ISubject<string> _errorOutput = new Subject<string>();

        public IObservable<string> StandartOutput { get { return _standartOutput; } }
        public IObservable<string> ErrorOutput { get { return _errorOutput; } }

        public static readonly DependencyProperty ScriptPathProperty =
            DependencyProperty.Register("ScriptPath", typeof(string), typeof(ScriptViewer),
                                        new PropertyMetadata(default(string)));

        private ConsoleProcess _console;

        public ScriptViewer()
        {
            InitializeComponent();

        }

        public string ScriptPath
        {
            get { return (string)GetValue(ScriptPathProperty); }
            set
            {
                SetValue(ScriptPathProperty, value);
                LoadScript(value);
            }
        }

        private void LoadScript(string scriptPath)
        {
            var lines = File.ReadLines(scriptPath);
            UiScriptList.ItemsSource = lines;
            UiScriptList.SelectedIndex = 0;

            if (_console != null)
            {
                _console.Close();
            }

            var procStartInfo = new ProcessStartInfo("cmd");
            var app = (App)Application.Current;
            procStartInfo.WorkingDirectory = app.AppSettings.AdbDirectory;
            _console = new ConsoleProcess(procStartInfo);
            _console.Start();

            _console.StandartOutput
                .Buffer(TimeSpan.FromSeconds(1))
                .Select(x => new string(x.ToArray()))
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(_standartOutput);

            _console.ErrorOutput
                .Buffer(TimeSpan.FromSeconds(1))
                .Select(x => new string(x.ToArray()))
                .Where(x => !string.IsNullOrEmpty(x))
                .Subscribe(_errorOutput);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var cmd = UiTxtRun.Text;

            ExecuteCommand(cmd);

            if (UiScriptList.SelectedIndex < UiScriptList.Items.Count - 1)
            {
                UiScriptList.SelectedIndex++;
                var nextCmd = (string)UiScriptList.SelectedItem;
                if (nextCmd.StartsWith("#"))
                {
                    //# wait 1s
                    //# wait "ok"
                }
            }
        }

        private void ExecuteCommand(string cmd)
        {
            try
            {
                _console.Input.WriteLine(cmd);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UiTxtRun_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonBase_OnClick(sender, null);
            }
        }

    }
}
