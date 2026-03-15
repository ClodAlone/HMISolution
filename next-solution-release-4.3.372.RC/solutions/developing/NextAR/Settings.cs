using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextAR
{
    static public class Settings
    {
        // Fields...
        static private string _WebUrl;

        static public string WebUrl
        {
            get { return _WebUrl; }
            set
            {
                _WebUrl = value;
            }
        }
        
        static public void LoadSettigs()
        {
            WebUrl = Storage.LoadSettings(WebUrlSettings);
        }

        static public void SaveSettigs()
        {
            Storage.SaveSettings(WebUrlSettings, WebUrl);
        }

        const String WebUrlSettings = "WebUrl";
        static public bool IsSettingsAvailable()
        {
            return Storage.IsSettingSet(WebUrlSettings);
        }
    }
}
