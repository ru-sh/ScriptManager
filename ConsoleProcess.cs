using System.Diagnostics;
using System.IO;

namespace ScriptCommander
{
    public class ConsoleProcess
    {
        public Process Process { get; private set; }
        private readonly ProcessStartInfo _startInfo;

        public StreamWriter Input { get; set; }

        public ConsoleProcess(ProcessStartInfo startInfo)
        {
            _startInfo = startInfo;

            _startInfo.RedirectStandardInput = true;
            _startInfo.RedirectStandardOutput = true;
            _startInfo.RedirectStandardError = true;
            _startInfo.CreateNoWindow = false;
            _startInfo.UseShellExecute = false;

            Process = new Process
            {
                StartInfo = _startInfo,
                EnableRaisingEvents = true,
            };

            Process.ErrorDataReceived += ErrorDataReceived;
            Process.OutputDataReceived += OutputDataReceived;
        }

        public void Start()
        {
            Trace.Write(_startInfo.FileName);
            Trace.Write(" ");
            Trace.WriteLine(_startInfo.Arguments);

            Process.Start();

            Input = Process.StandardInput;
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Trace.WriteLine(e.Data);
            }
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.TraceError(e.Data);
        }
    }
}
