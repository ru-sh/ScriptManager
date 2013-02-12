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
using ScriptCommander.Core;

namespace ScriptCommander
{
    /// <summary>
    /// Interaction logic for ScriptViewer.xaml
    /// </summary>
    public partial class ScriptViewer
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty ApiProperty =
            DependencyProperty.Register("Api", typeof (Api), typeof (ScriptViewer), new PropertyMetadata(default(Api)));

        public Api Api
        {
            get { return (Api) GetValue(ApiProperty); }
            set { SetValue(ApiProperty, value); }
        }

        public ScriptViewer()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var cmd = UiTxtRun.Text;

            Api.Console.Write(cmd + Environment.NewLine);

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
