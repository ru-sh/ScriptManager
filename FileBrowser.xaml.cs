using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScriptCommander.Annotations;
using Trace = ScriptCommander.Core.Trace;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for FileBrowser.xaml
    /// </summary>
    public partial class FileBrowser : INotifyPropertyChanged
    {
        private string _selectedScriptPath;
        public event PropertyChangedEventHandler PropertyChanged;

        readonly App _app = (App)Application.Current;

        public string SelectedScriptPath
        {
            get { return _selectedScriptPath; }
            set
            {
                if (value == _selectedScriptPath) return;
                _selectedScriptPath = value;
                this.OnPropertyChanged();
            }
        }

        public FileBrowser()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var scriptsDir = _app.AppSettings.ScriptsDir;
            UpdatePath(scriptsDir);
        }

        private void UiListFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ListBoxItem)sender;
            var str = (string)item.DataContext;

            var currentPath = (string)UiComboDrives.SelectedValue;

            currentPath += str;
            currentPath = Path.GetFullPath(currentPath);

            if (str.EndsWith("\\"))
            {
                UpdatePath(currentPath);
            }
            else
            {
                SelectedScriptPath = currentPath;
            }
        }

        void UpdatePath(string scriptsDir)
        {
            _app.AppSettings.ScriptsDir = scriptsDir;

            IEnumerable<string> drives = Directory.GetLogicalDrives();
            if (string.IsNullOrWhiteSpace(scriptsDir))
            {
                scriptsDir = drives.First();
            }
            try
            {

                drives = drives.Select(d => scriptsDir.StartsWith(d, StringComparison.InvariantCultureIgnoreCase) ? scriptsDir : d);
                UiComboDrives.ItemsSource = drives;
                UiComboDrives.SelectedValue = scriptsDir;

                var dirs = Directory.GetDirectories(scriptsDir).Select(d => Path.GetFileName(d) + "\\");
                var scriptsExtensions = _app.AppSettings.ScriptsExtensions.Split(',').Select(s => s.Trim());
                var files = Directory.EnumerateFiles(scriptsDir)
                    .Select(Path.GetFileName)
                    .Select(f => Tuple.Create(f, Path.GetExtension(f)))
                    .Where(f => scriptsExtensions.Contains(f.Item2 ?? string.Empty,
                                                           StringComparer.InvariantCultureIgnoreCase))
                    .Select(t => t.Item1);

                var list = new[] { "..\\" }.Concat(dirs.Concat(files));

                UiListFiles.ItemsSource = list;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString(), "Error");
            }

            this.UpdateLayout();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UiComboDrives_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = (ComboBox)sender;
            if (c.SelectedValue != null) UpdatePath((string)c.SelectedValue);
        }
    }
}