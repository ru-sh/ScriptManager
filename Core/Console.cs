using System;
using System.Linq;
using System.Reactive.Linq;

namespace ScriptCommander.Core
{
    public class Console
    {
        private readonly ConsoleProcess _console;
        private readonly IObservable<string> _onOutput;
        private readonly IObservable<string> _onError;

        public Console(ConsoleProcess process)
        {
            _console = process;

            _onOutput = _console.StandartOutput
                                .Buffer(TimeSpan.FromSeconds(1)) // TODO 1sec or on new line
                                .Select(x => new string(x.ToArray()))
                                .Where(x => !string.IsNullOrEmpty(x));

            _onError = _console.ErrorOutput
                               .Buffer(TimeSpan.FromSeconds(1))
                               .Select(x => new string(x.ToArray()))
                               .Where(x => !string.IsNullOrEmpty(x));
        }

        public void OnOutputReceived(Action<string> onNext)
        {
            _onOutput.Subscribe(onNext);
        }

        public void OnErrorReceived(Action<string> onNext)
        {
            _onError.Subscribe(onNext);
        }

        public void Write(string str)
        {
            _console.Input.Write(str);
        }
    }
}