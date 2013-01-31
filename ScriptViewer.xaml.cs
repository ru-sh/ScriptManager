using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ScriptCommander.Annotations;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for ScriptViewer.xaml
    /// </summary>
    public partial class ScriptViewer
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty ScriptPathProperty =
            DependencyProperty.Register("ScriptPath", typeof(string), typeof(ScriptViewer),
                                        new PropertyMetadata(default(string)));

        private ConsoleProcess _process = null;

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
        }

        public ScriptViewer()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var cmd = UiTxtRun.Text;

            ExecuteCommand(cmd);

            if (UiScriptList.SelectedIndex < UiScriptList.Items.Count - 1)
            {
                UiScriptList.SelectedIndex++;
            }
            else
            {
            }
        }

        private void ExecuteCommand(string cmd)
        {
            try
            {

                if (cmd.StartsWith(">") || cmd.StartsWith("#"))
                {
                }
                else
                {
                    if (_process != null && _process.Process != null)
                    {
                        _process.Process.WaitForExit();
                    }

                    var strings = cmd.Split(' ');
                    var fileName = strings.First();
                    var arguments = strings.Skip(1).Aggregate((ac, s) => ac + ' ' + s);

                    var procStartInfo = new ProcessStartInfo(fileName, arguments);
                    var app = (App)Application.Current;
                    procStartInfo.WorkingDirectory = app.AppSettings.AdbDirectory;
                    _process = new ConsoleProcess(procStartInfo);
                    _process.Start();
                }
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
    }
}
