using System.Collections.Generic;

namespace ScriptCommander.Core
{
    public class Api
    {
        public Console Console { get; private set; }
        public Script Script { get; private set; }
        public Trace Trace { get; private set; }

        public Api(ConsoleProcess consoleProcess, IEnumerable<string> lines)
        {
            Console = new Console(consoleProcess);
            Script = new Script(lines);
            Trace = new Trace();
        }

        public void ExecuteNextCommand()
        {
            if (Script.CurrentIndex == Script.Lines.Length - 1)
            {
                return;
            }

            Script.CurrentIndex++;
            Console.Write(Script.CurrentLine);
        }
    }
}