using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ScriptCommander.Annotations;

namespace ScriptCommander.Core
{
    public class Script : INotifyPropertyChanged
    {
        private int _currentIndex;
        public string[] Lines { get; private set; }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (_currentIndex < 0 || _currentIndex >= Lines.Length)
                {
                    throw new IndexOutOfRangeException("CurrentIndex");
                }

                _currentIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("CurrentLine");
            }
        }

        public string CurrentLine { get { return Lines[CurrentIndex]; } }

        public Script(IEnumerable<string> lines)
        {
            Lines = lines.Concat(new[] { "" }).ToArray();
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