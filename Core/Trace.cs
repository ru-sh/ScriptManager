namespace ScriptCommander.Core
{
    public class Trace
    {
        public void Write(string str)
        {
            System.Diagnostics.Trace.Write(str);
        }

        public void WriteLine(string str)
        {
            System.Diagnostics.Trace.WriteLine(str);
        }
    }
}