using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ScriptCommander
{
    public class ConsoleProcess
    {
        private Process Process { get; set; }
        private readonly ProcessStartInfo _startInfo;

        public StreamWriter Input { get; set; }

        private readonly ISubject<char> _standartOutput = new Subject<char>();
        private readonly ISubject<char> _errorOutput = new Subject<char>();

        public IObservable<char> StandartOutput { get { return _standartOutput; } }
        public IObservable<char> ErrorOutput { get { return _errorOutput; } }

        public ConsoleProcess(ProcessStartInfo startInfo)
        {
            _startInfo = startInfo;

            _startInfo.RedirectStandardInput = true;
            _startInfo.RedirectStandardOutput = true;
            _startInfo.RedirectStandardError = true;
            _startInfo.CreateNoWindow = false;
            _startInfo.UseShellExecute = false;
            _startInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
        }

        public void Start()
        {
            Trace.Write(_startInfo.FileName);
            Trace.Write(" ");
            Trace.WriteLine(_startInfo.Arguments);

            Process = new Process
            {
                StartInfo = _startInfo,
                EnableRaisingEvents = true,
            };

            Process.Start();
            Input = Process.StandardInput;

            Action<StreamReader, ISubject<char>> readToSubject = (reader, subject) =>
                {
                    char s;
                    while (Process != null && !Process.HasExited && (s = (char)reader.Read()) != '\0')
                    {
                        subject.OnNext(s);
                    }

                };

            Task.Factory.StartNew(() => readToSubject(Process.StandardOutput, _standartOutput));
            Task.Factory.StartNew(() => readToSubject(Process.StandardError, _errorOutput));
        }

        public void Close()
        {
            if (!Process.HasExited)
            {
                Process.CloseMainWindow();
                Process.Close();
            }

            Process = null;
        }
    }
}
