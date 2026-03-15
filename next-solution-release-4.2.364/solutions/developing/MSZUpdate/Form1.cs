using System.IO.Compression;
using System.Windows.Forms;
using System.Xml;
using MSZ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using MSZFactory;

namespace MSZUpdate
{
    public partial class Form1 : Form
    {
        #region declarations
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
        private string _linux { get { return WPFUtilities.CryptString.CryptString.DecryptString("qm0Yrk8EOrSn8UWbFEq19Q=="); } }//("LNX")
        private string _arealty { get { return WPFUtilities.CryptString.CryptString.DecryptString("QIySLm54AfStlYVTfb9iWg=="); } }//("ART")

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

        private int _disabled = 0;
        private int _small = 99;
        private int _huge = 9999999;

        private string _boptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("QEocl/uPVXyKWY5o1Sgl8PTpDq0dHzXUpoFRkQ7H81M88o8PunSoR35hgHPitYZgak4SQ1cqnor7v5QG/26+MiMjKwse7pbLSf0UuHq7NvU="); } }
        //SVR,RT,DEV,SMD,CMD,DLR,RCP,VB,SCD,NTW,GEO,G3D,REP,DIS,STA,OUAS,WDEP
        private string _ioptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("bnK8MMxXeiUYq62hFZlhcynX2bn2qjjorkrSq0x2oMpE+o58a4VpRKIPdwD0CWEi"); } }
        //WCL5,WCL,STG,CTG,DRV,CHLD,SCR,ALR
        private string _ivaloptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("V2O4XPALQeadRtZW3aPxApGZMuVQ8sBnaOzZmBZ30eHB3nuVpesGrRW4aHONPUtZ8CtSiawk/LrhcUcUYhQesw=="); } }
        //99,99,9999999,9999999,99,9999999,9999999,9999999

        private int numSerial = 0;

        private string sitecode = string.Empty;

        private bool Unlimited;

        private DateTime dateTimePicker1;

        private bool Developer = false;

        private bool Runtime = false;

        private bool Server = false;

        private bool Dataloggers = false;

        private bool Client = false;

        private bool Recipes = false;

        private bool VBNET = false;

        private bool Scheduler = false;

        private bool WebDeploy = false;

        private bool DataAnalysis = false;

        private bool OPCUAServer = false;

        private bool ThreeD = false;

        private bool AlarmStatistics = false;

        private bool Geolocalization = false;

        private bool Dispatcher = false;

        private bool Networking = false;

        private bool Redundancy = false;

        private bool Debugging = false;

        private bool Modbus = false;
        private bool Automation = false;
        private bool Telemetry = false;
        private bool Facilities = false;
        private bool IOT = false;
        private bool Linux = false;
        private bool AReality = false;

        private int NumVarServer = 0;

        private int NumVarClient = 0;

        private int NumDrivers = 0;

        private int NumChilds = 0;

        private int NumWebClientsHTML5 = 0;

        private int NumAlarms = 0;

        private int NumScreens = 0;

        private int NumNet = 0;

        private int NumMeas = 0;

        private int NumLeanMeas = 0;

        private int SInstance = 0;
        private int NumWebClients = 0;
        private static string xCode = string.Empty;
        private const int REGISTER_MAX_BYTECOUNT = 188;
        private const int REGISTER_MAX_COUNT = 47;

        private uint mMovicon = 0;

        uint serial;

        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateKey();
        }

        private void UpdateKey()
        {
            try
            {
                MSZDelRead.ProductId = uint.MaxValue;

                serial = MSZDelRead.ReadMovSerialNumber();
                string filename = string.Format("{0}.key", serial);
                if (serial == 0)
                    MessageBox.Show(Properties.Resources.HWKeyError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
                else
                {
                    if (File.Exists(filename))
                    {
                        if(ReadFile(filename))
                            UpdateOp();
                    }
                    else
                        MessageBox.Show(Properties.Resources.FileError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.GenericError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
            }
            this.Close();
        }
        private bool checkAppType()
        {
            MSZDelRead.ProductId = uint.MaxValue;

            //read serial number
            uint serial = MSZDelRead.ReadMovSerialNumber();
            uint[] Data = new uint[REGISTER_MAX_COUNT];

            Data = MSZDelRead.ReadData();
            if (Data == null)
            {
                return true;
            }

            //Application type
            switch ((MSZ.MSZDelRead.ApplicationType)MSZDelRead.ProductId)
            {
                case MSZ.MSZDelRead.ApplicationType.apMovicon:
                    return true;
                    break;
                case MSZ.MSZDelRead.ApplicationType.apMovTrace:
                case MSZ.MSZDelRead.ApplicationType.apMoviconBA:
                    return false;
                    break;
            }

            return true;
        }
        private void UpdateOp()
        {
            if (!checkAppType())
            {
                if (MessageBox.Show(Properties.Resources.ApplicationError, Properties.Resources.ApplicationTitle, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            //unlimited value is mandatory for SGLock 

            uint serial = (uint)numSerial;
            MSZDelRead.WriteMovSerialNumber(serial);

            uint[] Data = new uint[REGISTER_MAX_COUNT];

            //Sitecode 12-16 bytes
            //string sc = WPFUtilities.CryptString.CryptString.DecryptString(textBox4.Text);
            //byte[] btemp = System.Text.Encoding.ASCII.GetBytes(sc); //new byte[16];
            byte[] btemp = System.Text.Encoding.ASCII.GetBytes(WPFUtilities.CryptString.CryptString.DecryptString(pgrCode)); //new byte[16];
            for (int i = 0; i < btemp.Length / 4; i++)
            {
                Data[i] = BitConverter.ToUInt32(btemp, i * 4);
            }
            ////ExpiringDate 6 bytes
            //btemp = new byte[8];

            btemp = new byte[8];

            //if (Unlimited.Checked)
            //{

            //MovNext programming Code
            btemp[0] = Convert.ToByte(0);
            btemp[1] = Convert.ToByte(0);
            btemp[2] = Convert.ToByte(0);
            btemp[3] = Convert.ToByte(0);
            btemp[4] = Convert.ToByte(0);
            btemp[5] = Convert.ToByte(0);


            Data[4] = BitConverter.ToUInt32(btemp, 0);
            Data[5] = BitConverter.ToUInt32(btemp, 4);
            //ActivationDate 6 bytes
            btemp[0] = 0;
            btemp[1] = 0;
            btemp[2] = 0;
            btemp[3] = 0;
            btemp[4] = 0;
            btemp[5] = 0;
            int idx = 5;
            Data[++idx] = BitConverter.ToUInt32(btemp, 0);
            Data[++idx] = BitConverter.ToUInt32(btemp, 4);
            //bool options 8 bytes
            btemp[0] = 0;
            btemp[1] = 0;
            btemp[2] = 0;
            btemp[3] = 0;
            btemp[4] = 0;
            btemp[5] = 0;
            btemp[6] = 0;
            btemp[7] = 0;
            if (Developer)
                btemp[0] |= 1;
            if (Runtime)
                btemp[0] |= 2;
            if (Server)
                btemp[0] |= 4;
            if (Dataloggers)
                btemp[0] |= 8;
            if (Client)
                btemp[0] |= 16;
            if (Recipes)
                btemp[0] |= 32;
            if (VBNET)
                btemp[0] |= 64;
            if (Scheduler)
                btemp[0] |= 128;
            if (WebDeploy)
                btemp[1] |= 1;
            if (DataAnalysis)
                btemp[1] |= 2;
            if (OPCUAServer)
                btemp[1] |= 4;
            if (ThreeD)
                btemp[1] |= 8;
            if (AlarmStatistics)
                btemp[1] |= 16;
            if (Geolocalization)
                btemp[1] |= 32;
            if (Dispatcher)
                btemp[1] |= 64;
            if (Networking)
                btemp[1] |= 128;
            if (Redundancy)
                btemp[2] |= 1;
            if (Debugging)
                btemp[2] |= 2;
            if (Modbus)
                btemp[2] |= 4;
            if (Automation)
                btemp[2] |= 8;
            if (Telemetry)
                btemp[2] |= 16;
            if (Facilities)
                btemp[2] |= 32;
            if (IOT)
                btemp[2] |= 64;
            if (Linux)
                btemp[2] |= 128;
            if (AReality)
                btemp[3] |= 1;
            Data[++idx] = BitConverter.ToUInt32(btemp, 0);
            Data[++idx] = BitConverter.ToUInt32(btemp, 4);
            //int options 64 bytes
            if (NumVarServer >= 0)
                Data[++idx] = Convert.ToUInt32(NumVarServer);
            if (NumVarClient >= 0)
                Data[++idx] = Convert.ToUInt32(NumVarClient);
            if (NumDrivers >= 0)
                Data[++idx] = Convert.ToUInt32(NumDrivers);
            if (NumChilds >= 0)
                Data[++idx] = Convert.ToUInt32(NumChilds);
            if (NumWebClientsHTML5 >= 0)
                Data[++idx] = Convert.ToUInt32(NumWebClientsHTML5);
            if (NumWebClients >= 0)
                Data[++idx] = Convert.ToUInt32(NumWebClients);
            if (NumAlarms >= 0)
                Data[++idx] = Convert.ToUInt32(NumAlarms);
            if (NumScreens >= 0)
                Data[++idx] = Convert.ToUInt32(NumScreens);
            if (NumNet >= 0)
                Data[++idx] = Convert.ToUInt32(NumNet);
            if (NumMeas >= 0)
                Data[++idx] = Convert.ToUInt32(NumMeas);
            if (NumLeanMeas >= 0)
                Data[++idx] = Convert.ToUInt32(NumLeanMeas);
            if (SInstance >= 0)
                Data[++idx] = Convert.ToUInt32(SInstance);

            uint aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovicon;

            try
            {
                MSZDelRead.WriteData(Data, aptype);
                MessageBox.Show(Properties.Resources.SuccesfullyUpdated, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
            }
            catch
            {
                MessageBox.Show(Properties.Resources.GenericError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
            }
        }
        private bool ReadFile(string fileName)
        {
            System.IO.StreamReader sr = new
                System.IO.StreamReader(fileName);
            var value = string.Empty;
            string ss = WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ==");
            string xKey = WPFUtilities.CryptString.CryptString.DecryptString(sr.ReadToEnd());
            sr.Close();
            var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xKey, ss, true, false);
            if (keyexpandolist.Count() != 0)
            {
                //var pippo = keyexpandolist.ToList();
                var modules = keyexpandolist.ToList()[0] as IDictionary<string, object>;
                //Serial
                if (modules.Keys.Contains(_serial))
                    numSerial = Convert.ToInt32((string)modules[_serial]);

                if (numSerial == 0)
                {
                    MessageBox.Show(Properties.Resources.FileKeyError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
                    return false;
                }
                if (serial != numSerial)
                {
                    MessageBox.Show(Properties.Resources.KeyError, Properties.Resources.ApplicationTitle, MessageBoxButtons.OK);
                    return false;
                }

                //SiteCode
                if (modules.Keys.Contains(PrevMarkUp))
                {
                    sitecode = WPFUtilities.CryptString.CryptString.EncryptString((string)modules[PrevMarkUp]);
                }
                //ExpiringDate
                if (modules.Keys.Contains(DateMarkUp))
                {
                    if (((string)modules[DateMarkUp]).Equals(WPFUtilities.CryptString.CryptString.DecryptString(NoDate)))
                    {
                        Unlimited = true;
                    }
                    else
                    {
                        Unlimited = false;
                        dateTimePicker1 = DateTime.Parse((string)modules[DateMarkUp]);
                    }
                }

                Developer = modules.Keys.Contains(_developer);
                Runtime = modules.Keys.Contains(_runtime);
                Server = modules.Keys.Contains(_server);
                Dataloggers = modules.Keys.Contains(_datalogger);
                Client = modules.Keys.Contains(_client);
                Recipes = modules.Keys.Contains(_recipe);
                VBNET = modules.Keys.Contains(_vbnet);
                Scheduler = modules.Keys.Contains(_scheduler);
                WebDeploy = modules.Keys.Contains(_deploy);
                DataAnalysis = modules.Keys.Contains(_report);
                OPCUAServer = modules.Keys.Contains(_opcuaserver);
                ThreeD = modules.Keys.Contains(_3D);
                AlarmStatistics = modules.Keys.Contains(_alarmstat);
                Geolocalization = modules.Keys.Contains(_geolocal);
                Dispatcher = modules.Keys.Contains(_dispatcher);
                Networking = modules.Keys.Contains(_networking);
                Redundancy = modules.Keys.Contains(_redundancy);
                Debugging = modules.Keys.Contains(_debug);

                Modbus = modules.Keys.Contains(_modbus);
                Automation = modules.Keys.Contains(_automation);
                Telemetry = modules.Keys.Contains(_telemetry);
                Facilities = modules.Keys.Contains(_facilities);
                IOT = modules.Keys.Contains(_iot);
                Linux = modules.Keys.Contains(_linux);
                AReality = modules.Keys.Contains(_arealty);


                if (modules.Keys.Contains(_servertag))
                    NumVarServer = Convert.ToInt32((string)modules[_servertag]);
                if (modules.Keys.Contains(_clienttag))
                    NumVarClient = Convert.ToInt32((string)modules[_clienttag]);
                if (modules.Keys.Contains(_drivers))
                    NumDrivers = Convert.ToInt32((string)modules[_drivers]);
                if (modules.Keys.Contains(_childs))
                    NumChilds = Convert.ToInt32((string)modules[_childs]);
                if (modules.Keys.Contains(_webclient5))
                    NumWebClientsHTML5 = Convert.ToInt32((string)modules[_webclient5]);
                if (modules.Keys.Contains(_alarms))
                    NumAlarms = Convert.ToInt32((string)modules[_alarms]);
                if (modules.Keys.Contains(_screens))
                    NumScreens = Convert.ToInt32((string)modules[_screens]);
                if (modules.Keys.Contains(_net))
                    NumNet = Convert.ToInt32((string)modules[_net]);
                if (modules.Keys.Contains(_proenergy))
                    NumMeas = Convert.ToInt32((string)modules[_proenergy]);
                if (modules.Keys.Contains(_prolean))
                    NumLeanMeas = Convert.ToInt32((string)modules[_prolean]);
                if (modules.Keys.Contains(_webclient))
                    NumWebClients = Convert.ToInt32((string)modules[_webclient]);
                if (modules.Keys.Contains(_sinstance))
                    SInstance = Convert.ToInt32((string)modules[_sinstance]);
                return true;
            }

            return false;
        }

    }
}
