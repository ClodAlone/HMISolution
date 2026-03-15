using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MSZFactory
{
    public class Craddle
    {
        /* -------------- Parameter format --------------
            {parameters}
            SiteCode -> string
            AppType -> apMovicon=0, apMovTrace, apMoviconBA
            
            "ID1=value1;ID2=value2;....."
            ID Description
            ------------------------------------------------------
            ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            AD ActivationDate -> dd/mm/yyyy
            {boolean}
            UN Unlimited -> 0,1
            DV Developer -> 0,1
            RT Runtime -> 0,1
            SR Server -> 0,1
            DL Dataloggers -> 0,1
            CL Client -> 0,1
            RC Recipes -> 0,1
            VB VBNET -> 0,1
            SC Scheduler -> 0,1
            WD WebDeploy -> 0,1
            DA DataAnalysis -> 0,1
            OS OPCUAServer -> 0,1
            TD ThreeD -> 0,1
            AS AlarmStatistics -> 0,1
            GL Geolocalization -> 0,1
            DR Dispatcher -> 0,1
            NW Networking -> 0,1
            RD Redundancy -> 0,1
            DB Debugging -> 0,1
            MDO Modbus -> 0,1
            AUO Automation -> 0,1
            TLO telemetry -> 0,1
            FCO Facilities -> 0,1
            IOO IOT -> 0,1
            AART Augmented reality -> 0,1
            LLNX Linux -> 0,1
            {numeric}
            NVS NumVarServer -> UINT32
            NVC NumVarClient -> UINT32
            NDR NumDrivers -> UINT32
            NCH NumChilds -> UINT32
            NW5 NumWebClientsHTML5 -> UINT32
            NWC NumWebClients -> UINT32
            NAL NumAlarms -> UINT32
            NSC NumScreens -> UINT32
            NSR numSerial -> UINT32
            NTL numNetworkClient -> UINT32
            NPE numProEnergyMeasures -> UINT32
            NPL numProLeanMeasures -> UINT32
            SIN numServerInstance -> UINT32
         */
        #region private declarations
        private static string NoDate = "rg51dnJNzvrnhxjPoi4zmw==";
        private static string pgrCode = "aqyFuVmb6LP+jg3oTfy4Dhz2ys8ZxGGjtEDqhdp/mf8=";
        private string PrevMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("BTEKkPCMPZn/v6d2OM7ekQ=="); } }//SiteCode
        private string DateMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("HOa44ZEOiZwUy5LheAVQUw=="); } }//ExpiringDate
        private string ActivationMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("t+V7RhAwpnVWTiyYcYIy+w=="); } }//ActivationDate
        private string ExpiringDelta { get { return WPFUtilities.CryptString.CryptString.DecryptString("prv/e1GpOfRUtjX+HrMM2A=="); } }//30
        private string AppMarkup { get { return WPFUtilities.CryptString.CryptString.DecryptString(""); } }//AppType

        private string _server { get { return WPFUtilities.CryptString.CryptString.DecryptString("aWWmz7U/Nodh7mSA9L8oAQ=="); } }//("SVR");
        private string _runtime { get { return WPFUtilities.CryptString.CryptString.DecryptString("ABmVqlvmqLNT3RbPaCt1Pg=="); } }//("RT");
        private string _developer { get { return WPFUtilities.CryptString.CryptString.DecryptString("2/gZ3e0cSqiFpdGw57W2ZA=="); } }//("DEV");
        private string _client { get { return WPFUtilities.CryptString.CryptString.DecryptString("ZNsToNGejkH59OOzi7bWvQ=="); } }//("CMD");
        private string _datalogger { get { return WPFUtilities.CryptString.CryptString.DecryptString("QfHPNIlk1a6nPnEyRK9clw=="); } }//("DLR");
        private string _recipe { get { return WPFUtilities.CryptString.CryptString.DecryptString("nCEgMHYEH5RLOaN03LCbxw=="); } }//("RCP");
        private string _vbnet { get { return WPFUtilities.CryptString.CryptString.DecryptString("/e40Vrioka3qo/RSOgI2xw=="); } }//("VB");
        private string _scheduler { get { return WPFUtilities.CryptString.CryptString.DecryptString("TfIcH8BA94C2MbB6w+u4Mw=="); } }//("SCD");
        private string _networking { get { return WPFUtilities.CryptString.CryptString.DecryptString("qr7ETdFumhQnxIJbEbhorQ=="); } }//("NTW");
        private string _redundancy { get { return WPFUtilities.CryptString.CryptString.DecryptString("+TUE/Sg1qoePVZu7g+dR+w=="); } }//("RED");
        private string _debug { get { return WPFUtilities.CryptString.CryptString.DecryptString("fug/a/XB+OiTnd/2ZdhCXA=="); } }//("DBG")

        private string _modbus { get { return WPFUtilities.CryptString.CryptString.DecryptString("8td5jGWD4tzp6zylCEPuww=="); } }//("MDB")
        private string _automation { get { return WPFUtilities.CryptString.CryptString.DecryptString("vSwyN4V9dnvKnDFD3OYujA=="); } }//("AUT")
        private string _telemetry { get { return WPFUtilities.CryptString.CryptString.DecryptString("0PQBKWp51zv/Ir+CTa8jTg=="); } }//("TLM")
        private string _facilities { get { return WPFUtilities.CryptString.CryptString.DecryptString("Yp4rzMtuSxNanPXI6JTivA=="); } }//("FCS")
        private string _iot { get { return WPFUtilities.CryptString.CryptString.DecryptString("SSaQsA8U3VuElRmBmDg8GQ=="); } }//("IOT")
        private string _art { get { return WPFUtilities.CryptString.CryptString.DecryptString("QIySLm54AfStlYVTfb9iWg=="); } }//("ART")
        private string _lnx { get { return WPFUtilities.CryptString.CryptString.DecryptString("qm0Yrk8EOrSn8UWbFEq19Q=="); } }//("LNX")

        private string _geolocal { get { return WPFUtilities.CryptString.CryptString.DecryptString("xh0e5EYFBkZtn0unXtNJfA=="); } }//("GEO");
        private string _3D { get { return WPFUtilities.CryptString.CryptString.DecryptString("NEcoOsNxAg4tjat/8T5zoQ=="); } }//("G3D");
        private string _report { get { return WPFUtilities.CryptString.CryptString.DecryptString("lohgq2F1Shi/lvnweCixaA=="); } }//("REP");
        private string _dispatcher { get { return WPFUtilities.CryptString.CryptString.DecryptString("iFUE/lJmjCniaH2/8irz5w=="); } }//("DIS");
        private string _alarmstat { get { return WPFUtilities.CryptString.CryptString.DecryptString("eDJgTyZ8URP38n06+AK8ug=="); } }//("STA");
        private string _opcuaserver { get { return WPFUtilities.CryptString.CryptString.DecryptString("2LEJER2ToEwTJrOoBYYDLg=="); } }//("OUAS");
        private string _deploy { get { return WPFUtilities.CryptString.CryptString.DecryptString("F8EDBcvXLdyv/OpSrhoqcg=="); } }//("WDEP");
        private string _webclient5 { get { return WPFUtilities.CryptString.CryptString.DecryptString("p6OOEyokimxEBwJapBYnaQ=="); } }//("WCL5");
        private string _webclient { get { return WPFUtilities.CryptString.CryptString.DecryptString("GWRgSK00mEYcSGJIfVzStQ=="); } }//("WCL");
        private string _servertag { get { return WPFUtilities.CryptString.CryptString.DecryptString("w89JD4DHxzgkFpU10NWGLA=="); } }//("STG");
        private string _clienttag { get { return WPFUtilities.CryptString.CryptString.DecryptString("1uZeuk41g5tEN8PkGiCwLw=="); } }//("CTG");
        private string _drivers { get { return WPFUtilities.CryptString.CryptString.DecryptString("PuXfSP24fhC4JL6M7LAQzA=="); } }//("DRV");
        private string _childs { get { return WPFUtilities.CryptString.CryptString.DecryptString("FmzaxK+3RVr/LUPMmVHeCQ=="); } }//("CHLD");
        private string _screens { get { return WPFUtilities.CryptString.CryptString.DecryptString("/H16Mdag/oIEZx3eDz8BEg=="); } }//("SCR");
        private string _alarms { get { return WPFUtilities.CryptString.CryptString.DecryptString("I+TwLvoQ7TUbaQ+YyVetAw=="); } }//("ALR");
        private string _little { get { return WPFUtilities.CryptString.CryptString.DecryptString("wstSI/P41LJeGb6158MEww=="); } }//("99");
        private string _big { get { return WPFUtilities.CryptString.CryptString.DecryptString("mxPFclWvubfF/qDzqyi2zg=="); } }//("9999999");
        private string _serial { get { return WPFUtilities.CryptString.CryptString.DecryptString("/2WysmBwr00nKx3ylLf4Pg=="); } }//("SN");
        private string _net { get { return WPFUtilities.CryptString.CryptString.DecryptString("zxVCYC4E/39MftYBrBiV3Q=="); } }//("NET");
        private string _proenergy { get { return WPFUtilities.CryptString.CryptString.DecryptString("gg9YQvhvx70kafcB0yaFmA=="); } }//("PEN")
        private string _prolean { get { return WPFUtilities.CryptString.CryptString.DecryptString("b2eHWGOY6pOyUuDOqvHGtw=="); } }//("PLN")
        private string _sinstance { get { return WPFUtilities.CryptString.CryptString.DecryptString("ojGsNaTsIoN+EtaYLi40Jw=="); } }//("SIN")
        #endregion

        #region CTors
        #endregion

        #region Public methods
        public string Generate(uint type, string code, string par)
        {
            Dictionary<string, string> KeyValues = new Dictionary<string, string>();
            string [] parameters = par.Split(';');
            if(parameters.Length == 0)
                return string.Empty;
            foreach (var p in parameters)
            {
                string[] wrk = p.Split('=');
                if (wrk.Length != 2)
                    continue;
                KeyValues.Add(wrk[0].Trim(), wrk[1].Trim());
            }

            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="), "");
            xDoc.AppendChild(xRoot);

            //SiteCode
            try
            {
                XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", PrevMarkUp));
                if (!code.Equals(WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==")))
                    xElemL0.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(code);
                xRoot.AppendChild(xElemL0);
            }
            catch (Exception e)
            {
                //MessageBox.Show("SiteCode non valido!", Properties.Resources.ErrorCaption);
                return "THISISANERROR - code invalid.";
            }
            //ExpiringDate
            if (KeyValues.ContainsKey("ED"))
            {
                XmlElement xElemL1 = xDoc.CreateElement(string.Format("{0}", DateMarkUp));
                if(KeyValues.ContainsKey("UN") && Convert.ToUInt32(KeyValues["UN"])==1)
                    xElemL1.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(NoDate);
                else
                    xElemL1.InnerText = KeyValues["ED"];
                xRoot.AppendChild(xElemL1);
            }
            //ActivationDate
            XmlElement xElemL10 = xDoc.CreateElement(string.Format("{0}", ActivationMarkUp));
            xRoot.AppendChild(xElemL10);
            //Developer
            if (KeyValues.ContainsKey("DV") && Convert.ToUInt32(KeyValues["DV"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _developer));
                xRoot.AppendChild(xElemL10);
            }
            //Runtime
            if (KeyValues.ContainsKey("RT") && Convert.ToUInt32(KeyValues["RT"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _runtime));
                xRoot.AppendChild(xElemL10);
            }
            //Server
            if (KeyValues.ContainsKey("SR") && Convert.ToUInt32(KeyValues["SR"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _server));
                xRoot.AppendChild(xElemL10);
            }
            //Dataloggers
            if (KeyValues.ContainsKey("DL") && Convert.ToUInt32(KeyValues["DL"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _datalogger));
                xRoot.AppendChild(xElemL10);
            }
            //Client
            if (KeyValues.ContainsKey("CL") && Convert.ToUInt32(KeyValues["CL"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _client));
                xRoot.AppendChild(xElemL10);
            }
            //Recipes
            if (KeyValues.ContainsKey("RC") && Convert.ToUInt32(KeyValues["RC"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _recipe));
                xRoot.AppendChild(xElemL10);
            }
            //VBNET
            if (KeyValues.ContainsKey("VB") && Convert.ToUInt32(KeyValues["VB"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _vbnet));
                xRoot.AppendChild(xElemL10);
            }
            //Scheduler
            if (KeyValues.ContainsKey("SC") && Convert.ToUInt32(KeyValues["SC"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _scheduler));
                xRoot.AppendChild(xElemL10);
            }
            //WebDeploy
            if (KeyValues.ContainsKey("WD") && Convert.ToUInt32(KeyValues["WD"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _deploy));
                xRoot.AppendChild(xElemL10);
            }
            //DataAnalysis
            if (KeyValues.ContainsKey("DA") && Convert.ToUInt32(KeyValues["DA"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _report));
                xRoot.AppendChild(xElemL10);
            }
            //OPCUAServer
            if (KeyValues.ContainsKey("OS") && Convert.ToUInt32(KeyValues["OS"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _opcuaserver));
                xRoot.AppendChild(xElemL10);
            }
            //ThreeD
            if (KeyValues.ContainsKey("TD") && Convert.ToUInt32(KeyValues["TD"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _3D));
                xRoot.AppendChild(xElemL10);
            }
            //AlarmStatistics
            if (KeyValues.ContainsKey("AS") && Convert.ToUInt32(KeyValues["AS"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarmstat));
                xRoot.AppendChild(xElemL10);
            }
            //Geolocalization
            if (KeyValues.ContainsKey("GL") && Convert.ToUInt32(KeyValues["GL"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _geolocal));
                xRoot.AppendChild(xElemL10);
            }
            //Dispatcher
            if (KeyValues.ContainsKey("DR") && Convert.ToUInt32(KeyValues["DR"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _dispatcher));
                xRoot.AppendChild(xElemL10);
            }
            //Networking
            if (KeyValues.ContainsKey("NW") && Convert.ToUInt32(KeyValues["NW"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _networking));
                xRoot.AppendChild(xElemL10);
            }
            //Redundancy
            if (KeyValues.ContainsKey("RD") && Convert.ToUInt32(KeyValues["RD"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _redundancy));
                xRoot.AppendChild(xElemL10);
            }
            //Debugging
            if (KeyValues.ContainsKey("DB") && Convert.ToUInt32(KeyValues["DB"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _debug));
                xRoot.AppendChild(xElemL10);
            }
            //Modbus  
            if (KeyValues.ContainsKey("MDO") && Convert.ToUInt32(KeyValues["MDO"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _modbus));
                xRoot.AppendChild(xElemL10);
            }
            //Automation
            if (KeyValues.ContainsKey("AUO") && Convert.ToUInt32(KeyValues["AUO"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _automation));
                xRoot.AppendChild(xElemL10);
            }
            //Telemetry
            if (KeyValues.ContainsKey("TLO") && Convert.ToUInt32(KeyValues["TLO"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _telemetry));
                xRoot.AppendChild(xElemL10);
            }
            //Facilities
            if (KeyValues.ContainsKey("FCO") && Convert.ToUInt32(KeyValues["FCO"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _facilities));
                xRoot.AppendChild(xElemL10);
            }
            //IOT
            if (KeyValues.ContainsKey("IOO") && Convert.ToUInt32(KeyValues["IOO"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _iot));
                xRoot.AppendChild(xElemL10);
            }
            //ART
            if (KeyValues.ContainsKey("AART") && Convert.ToUInt32(KeyValues["AART"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _art));
                xRoot.AppendChild(xElemL10);
            }
            //LNX
            if (KeyValues.ContainsKey("LLNX") && Convert.ToUInt32(KeyValues["LLNX"]) == 1)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _lnx));
                xRoot.AppendChild(xElemL10);
            }
            //
            //NumVarServer
            if (KeyValues.ContainsKey("NVS") && Convert.ToUInt32(KeyValues["NVS"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _servertag)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NVS"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumVarClient
            if (KeyValues.ContainsKey("NVC") && Convert.ToUInt32(KeyValues["NVC"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _clienttag)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NVC"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumDrivers
            if (KeyValues.ContainsKey("NDR") && Convert.ToUInt32(KeyValues["NDR"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _drivers)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NDR"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumChilds
            if (KeyValues.ContainsKey("NCH") && Convert.ToUInt32(KeyValues["NCH"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _childs)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NCH"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumWebClientsHTML5
            if (KeyValues.ContainsKey("NW5") && Convert.ToUInt32(KeyValues["NW5"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient5)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NW5"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumWebClients
            if (KeyValues.ContainsKey("NWC") && Convert.ToUInt32(KeyValues["NWC"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NWC"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumAlarms
            if (KeyValues.ContainsKey("NAL") && Convert.ToUInt32(KeyValues["NAL"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarms)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NAL"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumScreens
            if (KeyValues.ContainsKey("NSC") && Convert.ToUInt32(KeyValues["NSC"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _screens)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NSC"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //NumNetworkLicenceClient
            if (KeyValues.ContainsKey("NTL") && Convert.ToUInt32(KeyValues["NTL"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _net)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NTL"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //ProEnergy
            if (KeyValues.ContainsKey("NPE") && Convert.ToUInt32(KeyValues["NPE"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _proenergy)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NPE"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //ProLean
            if (KeyValues.ContainsKey("NPL") && Convert.ToUInt32(KeyValues["NPL"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _prolean)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NPL"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //ServerInstanceNumber
            if (KeyValues.ContainsKey("SI") && Convert.ToUInt32(KeyValues["SI"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _sinstance)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["SI"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            //numSerial
            if (KeyValues.ContainsKey("NSR") && Convert.ToUInt32(KeyValues["NSR"]) > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _serial)); // int
                xElemL10.InnerText = Convert.ToUInt32(KeyValues["NSR"]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            return WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}", xDoc.InnerXml));
            
        }
        #endregion
    }
}
