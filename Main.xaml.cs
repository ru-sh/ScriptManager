using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            this.Loaded += Main_Loaded;
        }

        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            var traceListener = new TextTraceListener();
            Trace.Listeners.Add(traceListener);

            traceListener.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == "Trace")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UiTraceOut.Text = traceListener.Trace;
                            ScrollParent(UiTraceOut);
                        });
                    }
                };

            Browser.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == "SelectedScriptPath")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var path = Browser.SelectedScriptPath;
                            Title = path;
                            UiScriptCommander.ScriptPath = path;
                        });
                    }
                };
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
