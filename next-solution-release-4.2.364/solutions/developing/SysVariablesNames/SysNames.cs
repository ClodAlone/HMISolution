using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysVariables
{
    public static class SysNames
    {
        static readonly public String dataSynkName = "SystemVariables";

        #region Sys Variables
        static readonly public String Blink200ms = "Blink200ms";
        static readonly public String Blink500ms = "Blink500ms";
        static readonly public String Blink1s = "Blink1s";
        static readonly public String Blink2500ms = "Blink2500ms";
        static readonly public String Blink5s = "Blink5s";
        static readonly public String Blink10s = "Blink10s";
        static readonly public String Blink30s = "Blink30s";
        static readonly public String CurrentTime = "CurrentTime";
        static readonly public String CurrentDate = "CurrentDate";
        static readonly public String CurrentLongDate = "CurrentLongDate";
        static readonly public String CurrentUser = "CurrentUser";
        static readonly public String CurrentRole = "CurrentRole";
        static readonly public String CurrentAccessLevel = "CurrentAccessLevel";
        static readonly public String CurrentAccessMask = "CurrentAccessMask";
        static readonly public String ActiveScreen = "ActiveScreen";
        static readonly public String LicenseSerialNumber = "LicenseSerialNumber";
        static readonly public String MouseMove = "MouseMove";

        static readonly public String PI = "PI";
        static readonly public String GPI = "GPI";
        static readonly public String NLP = "NLP";
        static readonly public String NP = "NP";

        static readonly public String Sine = "Sine";
        static readonly public String Square = "Square";
        static readonly public String Triangle = "Triangle";
        static readonly public String Sawtooth = "Sawtooth";
        static readonly public String Pulse = "Pulse";
        static readonly public String WhiteNoise = "WhiteNoise";
        static readonly public String GaussNoise = "GaussNoise";
        static readonly public String DigitalNoise = "DigitalNoise";

        static readonly public String LastHostNameUsed = "LastHostNameUsed";

        static readonly public String CurrentCulture = "CurrentCulture";
        static readonly public String CurrentConverter = "CurrentConverter";

        static readonly public String ServerConnectionError = "ServerConnectionError";
        static readonly public String TotalConnectedClientTags = "TotalConnectedClientTags";
        static readonly public String PercentageOfConnectedStartupTags = "PercentageOfConnectedStartupTags";

        static readonly public String NumActiveWebClientUsers = "NumActiveWebClientUsers";

        static readonly public String AlarmSoundActiveOnClient = "AlarmSoundActiveOnClient";

        #endregion
    }
}
