using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ScriptCommander.Annotations;

namespace ScriptCommander
{
    public class TextTraceListener : TraceListener, INotifyPropertyChanged
    {
        private readonly Subject<string> _textOut = new Subject<string>();
        public IObservable<string> TextOut { get { return _textOut; } }

        private string _trace = string.Empty;
        public string Trace
        {
            get { return _trace; }
            private set
            {
                if (value == _trace) return;
                _trace = value;
                OnPropertyChanged();
            }
        }

        public override void Write(string message)
        {
            Trace += message;
            Task.Factory.StartNew(() => _textOut.OnNext(message));
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
