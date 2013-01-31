using System.Diagnostics;

namespace ScriptCommander
{
    public class ConsoleProcess
    {
        public Process Process { get; private set; }
        private readonly ProcessStartInfo _startInfo;

        public ConsoleProcess(ProcessStartInfo startInfo)
        {
            _startInfo = startInfo;
        }

        public void Start()
        {
            Trace.Write(_startInfo.FileName);
            Trace.Write(" ");
            Trace.WriteLine(_startInfo.Arguments);

            _startInfo.RedirectStandardOutput = true;
            _startInfo.RedirectStandardError = true;
            _startInfo.CreateNoWindow = false;
            _startInfo.UseShellExecute = false;

            Process = new Process
                {
                    StartInfo = _startInfo, 
                    EnableRaisingEvents = true
                };

            Process.ErrorDataReceived += ErrorDataReceived;
            Process.OutputDataReceived += OutputDataReceived;

            Process.Start();

            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.Write(e.Data);
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.TraceError(e.Data);
        }
    }
}
