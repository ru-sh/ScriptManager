﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ScriptCommander.Core;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        readonly App _app;
        private ConsoleProcess _console;

        public Main()
        {
            InitializeComponent();
            this.Loaded += Main_Loaded;

            _app = (App)Application.Current;
        }

        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            var traceListener = new TextTraceListener();
            System.Diagnostics.Trace.Listeners.Add(traceListener);

            traceListener.TextOut.Subscribe(s => Dispatcher.Invoke(() =>
                {
                    UiTraceOut.Text += s;
                    ScrollParent(UiTraceOut);
                }));

            Browser.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == "SelectedScriptPath")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var path = Browser.SelectedScriptPath;
                            Title = path;

                            if (_console != null)
                            {
                                _console.Close();
                            }

                            var procStartInfo = new ProcessStartInfo("cmd");
                            var app = (App)Application.Current;
                            procStartInfo.WorkingDirectory = app.AppSettings.AdbDirectory;
                            _console = new ConsoleProcess(procStartInfo);
                            _console.Start();

                            var script = File.ReadAllLines(path);
                            var api = new Api(_console, script);
                            var output = new Subject<string>();
                            var errors = new Subject<string>();

                            api.Console.OnOutputReceived(output.OnNext);
                            api.Console.OnErrorReceived(errors.OnNext);

                            output.Merge(errors).Subscribe(str => System.Diagnostics.Trace.Write(str));

                            UiScriptCommander.Api = api;
                        });
                    }
                };

            var adbDirectory = _app.AppSettings.AdbDirectory;
            if (string.IsNullOrWhiteSpace(adbDirectory))
            {
                SetAdbPath();
            }
        }

        private void SetAdbPath()
        {
            const string messageBoxText = "Need to set adb path!";
            const string caption = "Adb not found";
            const MessageBoxButton button = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;
            var boxResult = MessageBox.Show(messageBoxText, caption, button, icon);
            if (boxResult == MessageBoxResult.OK)
            {
                var dlg = new Microsoft.Win32.OpenFileDialog
                    {
                        FileName = "adb",
                        DefaultExt = ".exe",
                        Filter = "Executive Files(*.exe;*.com)|*.exe;*.com|All files (*.*)|*.*"
                    };

                var result = dlg.ShowDialog();

                if (result == true)
                {
                    var filename = dlg.FileName;
                    var dir = Path.GetDirectoryName(filename);
                    _app.AppSettings.AdbDirectory = dir;
                }
                else
                {
                    SetAdbPath();
                }
            }
            else
            {
                this.Close();
            }
        }

        private static void ScrollParent(FrameworkElement element)
        {
            if (element == null) return;

            var textBoxBase = element.Parent as TextBoxBase;
            if (textBoxBase != null)
            {
                textBoxBase.ScrollToEnd();
            }
            else
            {
                var scrollViewer = element.Parent as ScrollViewer;
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToEnd();
                }
                else
                {
                    ScrollParent(element.Parent as FrameworkElement);
                }
            }
        }

    }


}
