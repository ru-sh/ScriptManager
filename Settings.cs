using System.Configuration;

namespace ScriptCommander
{
    public class Settings : ApplicationSettingsBase
    {
//        [ApplicationScopedSetting]
        public string ScriptsExtensions
        {
            get { return ConfigurationManager.AppSettings["ScriptsExtensions"]; }
            set { ConfigurationManager.AppSettings["ScriptsExtensions"] = value; }
        }

        [UserScopedSetting]
        public string ScriptsDir
        {
            get { return (string)(this["ScriptsDir"]); }
            set { this["ScriptsDir"] = value; }
        }
        public string AdbDirectory
        {
            get { return ConfigurationManager.AppSettings["AdbDirectory"]; }
            set { ConfigurationManager.AppSettings["AdbDirectory"] = value; }
        }

    }
}
