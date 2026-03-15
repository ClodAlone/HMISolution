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

namespace MSZUtil
{

    enum LicenseType : int
    {
        Editor, Unlimited, R100, R200, R500, R1K,
        R2K, R5K, R10K, R30K, R60K, R100K
        
    }
    

    public partial class Form1 : Form
    {
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

        private int _disabled = 0;
        private int _small = 99;
        private int _huge = 9999999;

        private string _boptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("QEocl/uPVXyKWY5o1Sgl8PTpDq0dHzXUpoFRkQ7H81M88o8PunSoR35hgHPitYZgak4SQ1cqnor7v5QG/26+MiMjKwse7pbLSf0UuHq7NvU="); } }
        //SVR,RT,DEV,SMD,CMD,DLR,RCP,VB,SCD,NTW,GEO,G3D,REP,DIS,STA,OUAS,WDEP
        private string _ioptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("bnK8MMxXeiUYq62hFZlhcynX2bn2qjjorkrSq0x2oMpE+o58a4VpRKIPdwD0CWEi"); } }
        //WCL5,WCL,STG,CTG,DRV,CHLD,SCR,ALR
        private string _ivaloptions { get { return WPFUtilities.CryptString.CryptString.DecryptString("V2O4XPALQeadRtZW3aPxApGZMuVQ8sBnaOzZmBZ30eHB3nuVpesGrRW4aHONPUtZ8CtSiawk/LrhcUcUYhQesw=="); } }
        //99,99,9999999,9999999,99,9999999,9999999,9999999

        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();        
        
        
        private static string xCode = string.Empty;
        private const int REGISTER_MAX_BYTECOUNT = 188;
        private const int REGISTER_MAX_COUNT = 47;

        private uint mMovicon = 0;

        private static string FilePath
        {
            get
            {
                return String.Format("{0}\\{1}",
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                          WPFUtilities.CryptString.CryptString.DecryptString("qeDRnBYSvVODSl4TSUUVjZi5mXWW1erIHSd4xxNxLTQ="));
            }
        }

        private string InputFile { get; set; }

        public Form1()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();
            foreach (string s in args)
            {
                InputFile = s;
            }

            var _i = _ioptions.Split(',').ToList();
            var _ival = _ivaloptions.Split(',').ToList();
            
            foreach (string e in _i)
            {
              mapItems.Add(e,_ival[_i.IndexOf(e)]);
            }

            //Redundancy.Enabled = false;

            NumVarServer.Minimum = uint.MinValue;
            NumVarClient.Minimum = uint.MinValue;
            NumDrivers.Minimum = uint.MinValue;
            NumChilds.Minimum = uint.MinValue;
            NumWebClientsHTML5.Minimum = uint.MinValue;
            NumWebClients.Minimum = uint.MinValue;
            NumAlarms.Minimum = uint.MinValue;
            NumScreens.Minimum = uint.MinValue;
            NumVarServer.Minimum = uint.MinValue;
            NumVarClient.Minimum = uint.MinValue;
            numSerial.Minimum = uint.MinValue;

            NumVarServer.Maximum = uint.MaxValue;
            NumVarClient.Maximum = uint.MaxValue;
            NumDrivers.Maximum = uint.MaxValue;
            NumChilds.Maximum = uint.MaxValue;
            NumWebClientsHTML5.Maximum = uint.MaxValue;
            NumWebClients.Maximum = uint.MaxValue;
            NumAlarms.Maximum = uint.MaxValue;
            NumScreens.Maximum = uint.MaxValue;
            NumVarServer.Maximum = uint.MaxValue;
            NumVarClient.Maximum = uint.MaxValue;
            numSerial.Maximum = uint.MaxValue;

            if (!string.IsNullOrEmpty(InputFile) && File.Exists(InputFile))
                try
                {
                    ReadFile(InputFile);
                }
                catch (Exception)
                {
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string chiaro = PrevMarkUp;
            string chiaro1 = DateMarkUp;
            string chiaro2 = ActivationMarkUp;
            string chiaro3 = ExpiringDelta;
            string chiaro4 = _boptions;
            string chiaro5 = _ioptions;
            string chiaro6 = _ivaloptions;

            ConvertXML(textBox1.Text);   
        }
        
        private string GenerateLicenceXML()
        {
            System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();

            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="), "");
            xDoc.AppendChild(xRoot);

            //SiteCode
            try
            {
                XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", PrevMarkUp));
                xElemL0.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(textBox4.Text);
                xRoot.AppendChild(xElemL0);
            }
            catch (Exception e)
            {
                MessageBox.Show("SiteCode non valido!", Properties.Resources.ErrorCaption);
                return null;
            }
            //ExpiringDate
            XmlElement xElemL1 = xDoc.CreateElement(string.Format("{0}", DateMarkUp));
            //DateTime aDay = DateTime.UtcNow;
            //TimeSpan aDelta = new System.TimeSpan(int.Parse(ExpiringDelta), 0, 0, 0);
            //DateTime aDate = aDay.Add(aDelta);
            DateTime aDay = dateTimePicker1.Value;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);

            if (Unlimited.Checked)
            {
                xElemL1.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(NoDate);
            }
            else
            {
                xElemL1.InnerText = aDate.ToString();
            }
            
            xRoot.AppendChild(xElemL1);
            //ActivationDate
            XmlElement xElemL10 = xDoc.CreateElement(string.Format("{0}", ActivationMarkUp));
            xRoot.AppendChild(xElemL10);

            //boolean options
            if (Developer.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _developer));
                xRoot.AppendChild(xElemL10);
            }
            if (Runtime.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _runtime));
                xRoot.AppendChild(xElemL10);
            }
            if (Server.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _server));
                xRoot.AppendChild(xElemL10);
            }
            if (Dataloggers.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _datalogger));
                xRoot.AppendChild(xElemL10);
            }
            if (Client.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _client));
                xRoot.AppendChild(xElemL10);
            }
            if (Recipes.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _recipe));
                xRoot.AppendChild(xElemL10);
            }
            if (VBNET.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _vbnet));
                xRoot.AppendChild(xElemL10);
            }
            if (Scheduler.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _scheduler));
                xRoot.AppendChild(xElemL10);
            }
            if (WebDeploy.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _deploy));
                xRoot.AppendChild(xElemL10);
            }
            if (DataAnalysis.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _report));
                xRoot.AppendChild(xElemL10);
            }
            if (OPCUAServer.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _opcuaserver));
                xRoot.AppendChild(xElemL10);
            }
            if (ThreeD.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _3D));
                xRoot.AppendChild(xElemL10);
            }
            if (AlarmStatistics.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarmstat));
                xRoot.AppendChild(xElemL10);
            }
            if (Geolocalization.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _geolocal));
                xRoot.AppendChild(xElemL10);
            }
            if (Dispatcher.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _dispatcher));
                xRoot.AppendChild(xElemL10);
            }
            if (Networking.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _networking));
                xRoot.AppendChild(xElemL10);
            }
            if (Redundancy.Checked)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _redundancy));
                xRoot.AppendChild(xElemL10);
            }
            //numeric options
            if (NumVarServer.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _servertag)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumVarServer.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumVarClient.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _clienttag)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumVarClient.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumDrivers.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _drivers)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumDrivers.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumChilds.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _childs)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumChilds.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumWebClientsHTML5.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient5)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumWebClientsHTML5.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumWebClients.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumWebClients.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumAlarms.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarms)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumAlarms.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (NumScreens.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _screens)); // int
                xElemL10.InnerText = Convert.ToUInt32(NumScreens.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }

            if (numSerial.Value > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _serial)); // int
                xElemL10.InnerText = Convert.ToUInt32(numSerial.Value).ToString();
                xRoot.AppendChild(xElemL10);
            }

            xCode = string.Format("{0}", xDoc.InnerXml);
            return xCode;
        }
        private void ConvertXML(string p)
        {
            System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();

            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="), "");
            xDoc.AppendChild(xRoot);


            XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", PrevMarkUp));
            //xElemL0.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(GetPrev());
            xElemL0.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(textBox4.Text);
            xRoot.AppendChild(xElemL0);

            XmlElement xElemL1 = xDoc.CreateElement(string.Format("{0}", DateMarkUp));

            //DateTime aDay = DateTime.UtcNow;
            //TimeSpan aDelta = new System.TimeSpan(int.Parse(ExpiringDelta), 0, 0, 0);
            //DateTime aDate = aDay.Add(aDelta);
            DateTime aDay = dateTimePicker1.Value;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);

            //xElemL1.InnerText = aDate.ToString(); 
            if (Unlimited.Checked)
            {
                xElemL1.InnerText = WPFUtilities.CryptString.CryptString.DecryptString(NoDate);
            }
            else
            {
                xElemL1.InnerText = aDate.ToString();
            }
            //xElemL1.InnerText = dateTimePicker1.Value.ToString(); 
            xRoot.AppendChild(xElemL1);

            XmlElement xElemL10 = xDoc.CreateElement(string.Format("{0}", ActivationMarkUp));
            xRoot.AppendChild(xElemL10);

            foreach (string e in _boptions.Split(',').ToList())
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", e)); // bool
                xRoot.AppendChild(xElemL10);
            }
            foreach (string key in mapItems.Keys.ToList())
            {

                xElemL10 = xDoc.CreateElement(string.Format("{0}",key)); // int
                xElemL10.InnerText = mapItems[key];
                xRoot.AppendChild(xElemL10);
            }


            if (!textBox1.Text.Equals(string.Empty))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(string.Format("<LM>{0}</LM>", textBox1.Text));

                foreach (XmlElement e in xmlDoc)
                {
                    foreach (XmlElement ei in e)
                    {
                        XmlElement xElemL2 = xDoc.CreateElement(string.Format("{0}", ei.Name));
                        xElemL2.InnerText = ei.InnerText;
                        xRoot.AppendChild(xElemL2);
                    }
                }
            }


            //var module = WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ==");
            xCode = string.Format("{0}", xDoc.InnerXml);
            textBox2.Text = WPFUtilities.CryptString.CryptString.EncryptString(xCode);
        }
        private string GetPrev()
        {
            var Prev = MSZUtils.GetPrevious();
            return Prev;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConvertXML(textBox1.Text);   
            File.WriteAllText(FilePath, textBox2.Text);
        }

        private static void Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                    & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {

                    // Create the compressed file.
                    using (FileStream outFile =
                                File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress =
                            new GZipStream(outFile,
                            CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);

                            //Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                            //    fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                        }
                    }
                }
            }
        }
        private static void Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length -
                        fi.Extension.Length);

                //Create the decompressed file.
                using (FileStream outFile = File.Create(origName))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        // Copy the decompression stream 
                        // into the output file.
                        Decompress.CopyTo(outFile);

                        //Console.WriteLine("Decompressed: {0}", fi.Name);

                    }
                }
            }
        }
        private static byte[] Compress(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Write(data, 0, data.Length);
            ds.Flush();
            ds.Close();
            return ms.ToArray();
        }
        private static byte[] Decompress(byte[] data)
        {
            const int BUFFER_SIZE = 256;
            byte[] tempArray = new byte[BUFFER_SIZE];
            List<byte[]> tempList = new List<byte[]>();
            int count = 0, length = 0;
 
            MemoryStream ms = new MemoryStream(data);
            DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
 
            while ((count = ds.Read(tempArray, 0, BUFFER_SIZE)) > 0)
            {
                if (count == BUFFER_SIZE)
                {
                    tempList.Add(tempArray);
                    tempArray = new byte[BUFFER_SIZE];
                }
                else
                {
                    byte[] temp = new byte[count];
                    Array.Copy(tempArray, 0, temp, 0, count);
                    tempList.Add(temp);
                }
                length += count;
            }
 
            byte[] retVal = new byte[length];
 
            count = 0;
            foreach (byte[] temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }
 
            return retVal;
        }

        private static uint BAToUInt32(byte[] bytes, int index)
        {
            return BitConverter.ToUInt32(bytes, index);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConvertXML(textBox1.Text);

            //File.WriteAllText(FilePath, textBox2.Text);
            //File.WriteAllText(string.Format("{0}.zop", FilePath), xCode);

            //FileInfo fi = new FileInfo(string.Format("{0}.zop", FilePath));
            //Compress(fi);

            //byte[] _data = File.ReadAllBytes(string.Format("{0}.zop.gz", FilePath));

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            //byte[] _indata = encoding.GetBytes(xCode);

            byte[] _indata = encoding.GetBytes(textBox2.Text);
            byte[] _data = Compress(_indata);

            if (_data.Count() > REGISTER_MAX_BYTECOUNT)
            {
                Console.WriteLine("WriteData Error: key too much long!!");
                textBox3.Text = "WriteData Error: key too much long!!";
                return;
            }

            byte[] _dataPair = new byte[REGISTER_MAX_BYTECOUNT];

            int i = 0;
            for (i = 0; i < (_data.Count()); i++)
            {
                _dataPair[i] = _data[i];
            }

            uint[] Data = new uint[REGISTER_MAX_COUNT];

            //File.Delete(string.Format("{0}.zop", FilePath));
            //File.Delete(string.Format("{0}.zop.gz", FilePath));

            for (i = 0; i < REGISTER_MAX_COUNT; i++)
            {
                Data[i] = BAToUInt32(_dataPair, i * 4);
            }

            MSZDelRead.WriteData(Data);
            textBox3.Text = "WriteData terminated!!";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            uint[] Data = new uint[REGISTER_MAX_COUNT];
            byte[] _dataPair = new byte[REGISTER_MAX_BYTECOUNT];
            bool bRead = false; 

            Data = MSZDelRead.ReadData();
            if (Data == null)
                return;
            int i = 0;
            foreach (uint bp in Data)
            {
                byte[] _bp = BitConverter.GetBytes(bp);

                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(_bp);

                _dataPair[i*4] = _bp[0];
                _dataPair[i*4 + 1] = _bp[1];
                _dataPair[i*4 + 2] = _bp[2];
                _dataPair[i*4 + 3] = _bp[3];
                i++;
            }

            for (i = 0; i < REGISTER_MAX_COUNT; i++)
            {
                bRead = (Data[i] != 0);
                if (bRead) break;
            }

            if (bRead)
            {
                //File.WriteAllBytes(string.Format("{0}.zop.gz", FilePath), _dataPair);
                //FileInfo fi = new FileInfo(string.Format("{0}.zop.gz", FilePath));
                //Decompress(fi);
                //File.Delete(string.Format("{0}.zop.gz", FilePath));
                //textBox3.Text = WPFUtilities.CryptString.CryptString.EncryptString(File.ReadAllText(string.Format("{0}.zop", FilePath)));
                //File.Delete(string.Format("{0}.zop", FilePath));

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                textBox3.Text = encoding.GetString(Decompress(_dataPair));
            }
            else
            {
                textBox3.Text = "The Key is empty!!";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            uint[] Data = new uint[REGISTER_MAX_COUNT];
            MSZDelRead.WriteData(Data);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            //Editor
            PresetOptions(LicenseType.Editor);
        }
        private void button7_Click(object sender, System.EventArgs e)
        {
            //Pico
            PresetOptions(LicenseType.R100);
        }

        private void button8_Click(object sender, System.EventArgs e)
        {
            //Micro
            PresetOptions(LicenseType.R200);
        }

        private void button9_Click(object sender, System.EventArgs e)
        {
            //Entry
            PresetOptions(LicenseType.R500);
        }

        private void button10_Click(object sender, System.EventArgs e)
        {
            //Small
            PresetOptions(LicenseType.R1K);
        }

        private void button11_Click(object sender, System.EventArgs e)
        {
            //Medium
            PresetOptions(LicenseType.R2K);
        }

        private void button12_Click(object sender, System.EventArgs e)
        {
            //Pro
            PresetOptions(LicenseType.R5K);
        }

        private void button13_Click(object sender, System.EventArgs e)
        {
            //Corporate
            PresetOptions(LicenseType.R10K);
        }

        private void button14_Click(object sender, System.EventArgs e)
        {
            //Enterprise
            PresetOptions(LicenseType.R30K);
        }

        private void button15_Click(object sender, System.EventArgs e)
        {
            //Unlimited
            PresetOptions(LicenseType.R60K);
        }
        private void button21_Click(object sender, EventArgs e)
        {
            PresetOptions(LicenseType.R100K);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            PresetOptions(LicenseType.Unlimited);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Developer.Checked = false;
            Runtime.Checked = false;
            Server.Checked = false;
            Dataloggers.Checked = false;
            Client.Checked = false;
            Recipes.Checked = false;
            VBNET.Checked = false;
            Scheduler.Checked = false;
            WebDeploy.Checked = false;
            DataAnalysis.Checked = false;
            OPCUAServer.Checked = false;
            ThreeD.Checked = false;
            AlarmStatistics.Checked = false;
            Geolocalization.Checked = false;
            Dispatcher.Checked = false;
            Networking.Checked = false;
            Redundancy.Checked = false;
            NumVarServer.Value = _disabled;
            NumVarClient.Value = _disabled;
            NumDrivers.Value = _disabled;
            NumChilds.Value = _disabled;
            NumWebClientsHTML5.Value = _disabled;
            NumWebClients.Value = _disabled;
            NumAlarms.Value = _disabled;
            NumScreens.Value = _disabled;
            rClient.Checked = false;
            rServer.Checked = false;
            rClientServer.Checked = false;
        }
        private void PresetOptions(LicenseType licenseType)
        {
            if (licenseType == LicenseType.Editor)
            {
                Developer.Checked = true;
                Runtime.Checked = false;
                Server.Checked = true;
                Dataloggers.Checked = true;
                Client.Checked = true;
                Recipes.Checked = true;
                VBNET.Checked = true;
                Scheduler.Checked = true;
                WebDeploy.Checked = true;
                DataAnalysis.Checked = true;
                OPCUAServer.Checked = true;
                ThreeD.Checked = true;
                AlarmStatistics.Checked = true;
                Geolocalization.Checked = true;
                Dispatcher.Checked = true;
                Networking.Checked = true;
                //Redundancy.Checked = true;
                NumVarServer.Value = _huge;
                NumVarClient.Value = _huge;
                NumDrivers.Value = _small;
                NumChilds.Value = _huge;
                NumWebClientsHTML5.Value = _small;
                NumWebClients.Value = _small;
                NumAlarms.Value = _huge;
                NumScreens.Value = _huge;
                return;
            }
            switch (licenseType)
            { 
                case LicenseType.R100:
                    NumVarServer.Value = 100;
                    NumVarClient.Value = 100;
                    break;
                case LicenseType.R200:
                    NumVarServer.Value = 200;
                    NumVarClient.Value = 200;
                    break;
                case LicenseType.R500:
                    NumVarServer.Value = 500;
                    NumVarClient.Value = 500;
                    break;
                case LicenseType.R1K:
                    NumVarServer.Value = 1000;
                    NumVarClient.Value = 1000;
                    break;
                case LicenseType.R2K:
                    NumVarServer.Value = 2000;
                    NumVarClient.Value = 2000;
                    break;
                case LicenseType.R5K:
                    NumVarServer.Value = 5000;
                    NumVarClient.Value = 5000;
                    break;
                case LicenseType.R10K:
                    NumVarServer.Value = 10000;
                    NumVarClient.Value = 10000;
                    break;
                case LicenseType.R30K:
                    NumVarServer.Value = 30000;
                    NumVarClient.Value = 30000;
                    break;
                case LicenseType.R60K:
                    NumVarServer.Value = 60000;
                    NumVarClient.Value = 60000;
                    break;
                case LicenseType.R100K:
                    NumVarServer.Value = 100000;
                    NumVarClient.Value = 100000;
                    break;
                case LicenseType.Unlimited:
                    NumVarServer.Value = _huge;
                    NumVarClient.Value = _huge;
                    break;
            }
            Developer.Checked = false;
            Runtime.Checked = true;

            //Dataloggers.Checked = true;
            Recipes.Checked = true;
            VBNET.Checked = true;
            //Scheduler.Checked = true;
            Networking.Checked = true;
           // Geolocalization.Checked = true;
            NumAlarms.Value = _huge;
            NumScreens.Value = _huge;
            NumChilds.Value = _small;
            Server.Checked = (rServer.Checked || rClientServer.Checked);
            Client.Checked = (rClient.Checked || rClientServer.Checked);

            WebDeploy.Checked = false;
            DataAnalysis.Checked = false;
            OPCUAServer.Checked = false;
            //ThreeD.Checked = false;
            //AlarmStatistics.Checked = false;
            //Dispatcher.Checked = true;
            //NumDrivers.Value = 1;
            NumWebClientsHTML5.Value = 0;
            NumWebClients.Value = 0;
            NumDrivers.Value = Server.Checked ? 1 : 0;
        }

        private void rServer_Clicked(object sender, System.EventArgs e)
        {
            Server.Checked = true;
            NumDrivers.Value = 1;
            Client.Checked = false;
            NumAlarms.Value = _huge;
            NumScreens.Value = _huge;
        }
        private void rClient_Clicked(object sender, EventArgs e)
        {
            Server.Checked = false;
            NumDrivers.Value = 0;
            Client.Checked = true;
            NumAlarms.Value = _huge;
            NumScreens.Value = _huge;
        }

        private void rClientServer_Cliked(object sender, EventArgs e)
        {
            Server.Checked = true;
            NumDrivers.Value = 1;
            Client.Checked = true;
            NumAlarms.Value = 1024;
            NumScreens.Value = 64;
        }

        private void button16_Click(object sender, System.EventArgs e)
        {
            sglockCheck.Visible = false;

            if (numSerial.Value == 0)
            {
                MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                return;
            }
            if (textBox4.Text.Length == 0)
            {
                MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                return;
            }

            string pt = FilePath;

            StringBuilder param = new StringBuilder();
            //ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            DateTime aDay = dateTimePicker1.Value;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);
            param.Append(string.Format("ED={0};", aDate.ToString()));
            //UN Unlimited -> 0,1
            param.Append(string.Format("UN={0};", Unlimited.Checked ? 1 : 0));
            //DV Developer -> 0,1
            param.Append(string.Format("DV={0};", Developer.Checked ? 1 : 0));
            //RT Runtime -> 0,1
            param.Append(string.Format("RT={0};", Runtime.Checked ? 1 : 0));
            //SR Server -> 0,1
            param.Append(string.Format("SR={0};", Server.Checked ? 1 : 0));
            //DL Dataloggers -> 0,1
            param.Append(string.Format("DL={0};", Dataloggers.Checked ? 1 : 0));
            //CL Client -> 0,1
            param.Append(string.Format("CL={0};", Client.Checked ? 1 : 0));
            //RC Recipes -> 0,1
            param.Append(string.Format("RC={0};", Recipes.Checked ? 1 : 0));
            //VB VBNET -> 0,1
            param.Append(string.Format("VB={0};", VBNET.Checked ? 1 : 0));
            //SC Scheduler -> 0,1
            param.Append(string.Format("SC={0};", Scheduler.Checked ? 1 : 0));
            //WD WebDeploy -> 0,1
            param.Append(string.Format("WD={0};", WebDeploy.Checked ? 1 : 0));
            //DA DataAnalysis -> 0,1
            param.Append(string.Format("DA={0};", DataAnalysis.Checked ? 1 : 0));
            //OS OPCUAServer -> 0,1
            param.Append(string.Format("OS={0};", OPCUAServer.Checked ? 1 : 0));
            //TD ThreeD -> 0,1
            param.Append(string.Format("TD={0};", ThreeD.Checked ? 1 : 0));
            //AS AlarmStatistics -> 0,1
            param.Append(string.Format("AS={0};", AlarmStatistics.Checked ? 1 : 0));
            //GL Geolocalization -> 0,1
            param.Append(string.Format("GL={0};", Geolocalization.Checked ? 1 : 0));
            //DR Dispatcher -> 0,1
            param.Append(string.Format("DR={0};", Dispatcher.Checked ? 1 : 0));
            //NW Networking -> 0,1
            param.Append(string.Format("NW={0};", Networking.Checked ? 1 : 0));
            //RED Redundancy -> 0,1
            param.Append(string.Format("RD={0};", Redundancy.Checked ? 1 : 0));
            //{numeric}
            //NVS NumVarServer -> UINT32
            param.Append(string.Format("NVS={0};", NumVarServer.Value));
            //NVC NumVarClient -> UINT32
            param.Append(string.Format("NVC={0};", NumVarClient.Value));
            //NDR NumDrivers -> UINT32
            param.Append(string.Format("NDR={0};", NumDrivers.Value));
            //NCH NumChilds -> UINT32
            param.Append(string.Format("NCH={0};", NumChilds.Value));
            //NW5 NumWebClientsHTML5 -> UINT32
            param.Append(string.Format("NW5={0};", NumWebClientsHTML5.Value));
            //NWC NumWebClients -> UINT32
            param.Append(string.Format("NWC={0};", NumWebClients.Value));
            //NAL NumAlarms -> UINT32
            param.Append(string.Format("NAL={0};", NumAlarms.Value));
            //NSC NumScreens -> UINT32
            param.Append(string.Format("NSC={0};", NumScreens.Value));
            //NSR numSerial -> UINT32
            param.Append(string.Format("NSR={0}", numSerial.Value));

            var pippo = new Craddle();
            string lic  = pippo.Generate(
                (uint)(rMoviconBA.Checked ?
                    MSZ.MSZDelRead.ApplicationType.apMoviconBA :
                        (rMovTrace.Checked ? MSZ.MSZDelRead.ApplicationType.apMovTrace : MSZ.MSZDelRead.ApplicationType.apMovicon)),

                            textBox4.Text,
                                param.ToString());

            //string xml = GenerateLicenceXML();
            //if (xml == null)
            //    return;
            //string lic = WPFUtilities.CryptString.CryptString.EncryptString(xml);
            byte[] data = Encoding.ASCII.GetBytes(lic);
            
            File.WriteAllText(pt, lic);
            Process.Start("explorer.exe", string.Format("/select,{0}", pt));
        }
        private bool checkAppType()
        {
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
        private void button17_Click(object sender, System.EventArgs e)
        {
            if (!checkAppType())
            {
                if (MessageBox.Show(Properties.Resources.ApplicationError, Properties.Resources.ApplicationTitle, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            rMovicon.Checked = true;
            //unlimited value is mandatory for SGLock 
            Unlimited.Checked = true;
            dateTimePicker1.Enabled = false;

            uint serial = (uint)numSerial.Value;
            MSZDelRead.WriteMovSerialNumber(serial);

            uint[] Data = new uint[REGISTER_MAX_COUNT];

            textBox4.Text = string.Empty;

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
            //}
            //else 
            //{
            //    btemp[0] = Convert.ToByte(dateTimePicker1.Value.Day);
            //    btemp[1] = Convert.ToByte(dateTimePicker1.Value.Month);
            //    btemp[2] = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
            //    //btemp[3] = Convert.ToByte(dateTimePicker1.Value.Hour);
            //    //btemp[4] = Convert.ToByte(dateTimePicker1.Value.Minute);
            //    //btemp[5] = Convert.ToByte(dateTimePicker1.Value.Second);
            //    btemp[3] = Convert.ToByte(23);
            //    btemp[4] = Convert.ToByte(59);
            //    btemp[5] = Convert.ToByte(59);
            //}


            Data[4] = BitConverter.ToUInt32(btemp, 0);
            Data[5] = BitConverter.ToUInt32(btemp, 4);
            //ActivationDate 6 bytes
            btemp[0] = 0;// Convert.ToByte(dateTimePicker1.Value.Day);
            btemp[1] = 0;//Convert.ToByte(dateTimePicker1.Value.Month);
            btemp[2] = 0;//Convert.ToByte(dateTimePicker1.Value.Year);
            btemp[3] = 0;//Convert.ToByte(dateTimePicker1.Value.Hour);
            btemp[4] = 0;//Convert.ToByte(dateTimePicker1.Value.Minute);
            btemp[5] = 0;//Convert.ToByte(dateTimePicker1.Value.Second);
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
            if (Developer.Checked)
                btemp[0] |= 1;
            if (Runtime.Checked)
                btemp[0] |= 2;
            if (Server.Checked)
                btemp[0] |= 4;
            if (Dataloggers.Checked)
                btemp[0] |= 8;
            if (Client.Checked)
                btemp[0] |= 16;
            if (Recipes.Checked)
                btemp[0] |= 32;
            if (VBNET.Checked)
                btemp[0] |= 64;
            if (Scheduler.Checked)
                btemp[0] |= 128;
            if (WebDeploy.Checked)
                btemp[1] |= 1;
            if (DataAnalysis.Checked)
                btemp[1] |= 2;
            if (OPCUAServer.Checked)
                btemp[1] |= 4;
            if (ThreeD.Checked)
                btemp[1] |= 8;
            if (AlarmStatistics.Checked)
                btemp[1] |= 16;
            if (Geolocalization.Checked)
                btemp[1] |= 32;
            if (Dispatcher.Checked)
                btemp[1] |= 64;
            if (Networking.Checked)
                btemp[1] |= 128;
            if (Redundancy.Checked)
                btemp[2] |= 1;
            Data[++idx] = BitConverter.ToUInt32(btemp, 0);
            Data[++idx] = BitConverter.ToUInt32(btemp, 4);
            //int options 64 bytes
            if (NumVarServer.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumVarServer.Value);
            if (NumVarClient.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumVarClient.Value);
            if (NumDrivers.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumDrivers.Value);
            if (NumChilds.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumChilds.Value);
            if (NumWebClientsHTML5.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumWebClientsHTML5.Value);
            if (NumWebClients.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumWebClients.Value);
            if (NumAlarms.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumAlarms.Value);
            if (NumScreens.Value >= 0)
                Data[++idx] = Convert.ToUInt32(NumScreens.Value);
            
            uint aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovicon;
            if (rMovTrace.Checked)
                aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovTrace;
            else if (rMoviconBA.Checked)
                aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMoviconBA;

            try
            {
                MSZDelRead.WriteData(Data, aptype);
                sglockCheck.Visible = false;
            }
            catch
            {
                sglockCheck.Visible = true;
            }

        }

        private void ReadFile(string fileName)
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
                    numSerial.Value = Convert.ToInt32((string)modules[_serial]);
                //SiteCode
                if (modules.Keys.Contains(PrevMarkUp))
                {
                    textBox4.Text = WPFUtilities.CryptString.CryptString.EncryptString((string)modules[PrevMarkUp]);
                }
                //ExpiringDate
                if (modules.Keys.Contains(DateMarkUp))
                {
                    if (((string)modules[DateMarkUp]).Equals(WPFUtilities.CryptString.CryptString.DecryptString(NoDate)))
                    {
                        dateTimePicker1.Enabled = false;
                        Unlimited.Checked = true;
                    }
                    else
                    {
                        dateTimePicker1.Enabled = true;
                        Unlimited.Checked = false;
                        dateTimePicker1.Value = DateTime.Parse((string)modules[DateMarkUp]);
                    }
                }

                Developer.Checked = modules.Keys.Contains(_developer);
                Runtime.Checked = modules.Keys.Contains(_runtime);
                Server.Checked = modules.Keys.Contains(_server);
                Dataloggers.Checked = modules.Keys.Contains(_datalogger);
                Client.Checked = modules.Keys.Contains(_client);
                Recipes.Checked = modules.Keys.Contains(_recipe);
                VBNET.Checked = modules.Keys.Contains(_vbnet);
                Scheduler.Checked = modules.Keys.Contains(_scheduler);
                WebDeploy.Checked = modules.Keys.Contains(_deploy);
                DataAnalysis.Checked = modules.Keys.Contains(_report);
                OPCUAServer.Checked = modules.Keys.Contains(_opcuaserver);
                ThreeD.Checked = modules.Keys.Contains(_3D);
                AlarmStatistics.Checked = modules.Keys.Contains(_alarmstat);
                Geolocalization.Checked = modules.Keys.Contains(_geolocal);
                Dispatcher.Checked = modules.Keys.Contains(_dispatcher);
                Networking.Checked = modules.Keys.Contains(_networking);
                Redundancy.Checked = modules.Keys.Contains(_redundancy);
                if (modules.Keys.Contains(_servertag))
                    NumVarServer.Value = Convert.ToInt32((string)modules[_servertag]);
                if (modules.Keys.Contains(_clienttag))
                    NumVarClient.Value = Convert.ToInt32((string)modules[_clienttag]);
                if (modules.Keys.Contains(_drivers))
                    NumDrivers.Value = Convert.ToInt32((string)modules[_drivers]);
                if (modules.Keys.Contains(_childs))
                    NumChilds.Value = Convert.ToInt32((string)modules[_childs]);
                if (modules.Keys.Contains(_webclient5))
                    NumWebClientsHTML5.Value = Convert.ToInt32((string)modules[_webclient5]);
                if (modules.Keys.Contains(_alarms))
                    NumAlarms.Value = Convert.ToInt32((string)modules[_alarms]);
                if (modules.Keys.Contains(_screens))
                    NumScreens.Value = Convert.ToInt32((string)modules[_screens]);
                if (modules.Keys.Contains(_webclient))
                    NumWebClients.Value = Convert.ToInt32((string)modules[_webclient]);
            }

        }
        private void button18_Click(object sender, System.EventArgs e)
        {
            sglockCheck.Visible = false;

            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReadFile(op.FileName);
            }
        }

        private void button19_Click(object sender, System.EventArgs e)
        {
            //read serial number
            uint serial = MSZDelRead.ReadMovSerialNumber();
            numSerial.Value = serial;

            uint[] Data = new uint[REGISTER_MAX_COUNT];
            
            Data = MSZDelRead.ReadData();
            if (Data == null)
                return;

            //Application type
            switch ((MSZ.MSZDelRead.ApplicationType)MSZDelRead.ProductId)
            {
                case MSZ.MSZDelRead.ApplicationType.apMovicon:
                    rMovicon.Checked = true;
                    break;
                case MSZ.MSZDelRead.ApplicationType.apMovTrace:
                    rMovTrace.Checked = true;
                    break;
                case MSZ.MSZDelRead.ApplicationType.apMoviconBA:
                    rMoviconBA.Checked = true;
                    break;
            }

            //sitecode
            byte[] btemp = new byte[16];
            int k = -1;
            for (int i = 0; i < 4; i++)
            {
                btemp[++k] = (byte)Data[i];
                btemp[++k] = (byte)(Data[i] >> 8);
                btemp[++k] = (byte)(Data[i] >> 16);
                btemp[++k] = (byte)(Data[i] >> 24);
            }
            
            string sc = System.Text.Encoding.ASCII.GetString(btemp);
            var _pgrtemp = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));

            if (_pgrtemp.Equals(pgrCode))
                sglockCheck.Visible = false;
            else
            {
                sglockCheck.Visible = true;
                return;
            }

            //textBox4.Text = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));
            textBox4.Text = string.Empty;

            //ExpiringDate
            k = -1;
            btemp[++k] = (byte)Data[4];
            btemp[++k] = (byte)(Data[4] >> 8);
            btemp[++k] = (byte)(Data[4] >> 16);
            btemp[++k] = (byte)(Data[4] >> 24);
            btemp[++k] = (byte)Data[5];
            btemp[++k] = (byte)(Data[5] >> 8);
            btemp[++k] = (byte)(Data[5] >> 16);
            btemp[++k] = (byte)(Data[5] >> 24);
            try
            {
                if (btemp[2] == 0 &&
                   btemp[1] == 0 &&
                   btemp[0] == 0 &&
                   btemp[3] == 0 &&
                   btemp[4] == 0 &&
                   btemp[5] == 0)
                {
                    Unlimited.Checked = true;
                    dateTimePicker1.Enabled = false;
                }
                else
                {
                    Unlimited.Checked = false;
                    dateTimePicker1.Enabled = true;
                    dateTimePicker1.Value = new DateTime(btemp[2] + 2000, btemp[1], btemp[0], btemp[3], btemp[4], btemp[5]);
                }
            }
            catch
            {
            }
            //Activation date

            //bool option
            k = -1;
            btemp[++k] = (byte)Data[8];
            btemp[++k] = (byte)(Data[8] >> 8);
            btemp[++k] = (byte)(Data[8] >> 16);
            btemp[++k] = (byte)(Data[8] >> 24);
            btemp[++k] = (byte)Data[9];
            btemp[++k] = (byte)(Data[9] >> 8);
            btemp[++k] = (byte)(Data[9] >> 16);
            btemp[++k] = (byte)(Data[9] >> 24);

            Developer.Checked = ((btemp[0] & 1) > 0);
            Runtime.Checked = ((btemp[0] & 2) > 0);
            Server.Checked = ((btemp[0] & 4) > 0);
            Dataloggers.Checked = ((btemp[0] & 8) > 0);
            Client.Checked = ((btemp[0] & 16) > 0);
            Recipes.Checked = ((btemp[0] & 32) > 0);
            VBNET.Checked = ((btemp[0] & 64) > 0);
            Scheduler.Checked = ((btemp[0] & 128) > 0);
            WebDeploy.Checked = ((btemp[1] & 1) > 0);
            DataAnalysis.Checked = ((btemp[1] & 2) > 0);
            OPCUAServer.Checked = ((btemp[1] & 4) > 0);
            ThreeD.Checked = ((btemp[1] & 8) > 0);
            AlarmStatistics.Checked = ((btemp[1] & 16) > 0);
            Geolocalization.Checked = ((btemp[1] & 32) > 0);
            Dispatcher.Checked = ((btemp[1] & 64) > 0);
            Networking.Checked = ((btemp[1] & 128) > 0);
            Redundancy.Checked = ((btemp[2] & 1) > 0);


            //int options
            NumVarServer.Value = Data[10];
            NumVarClient.Value = Data[11];
            NumDrivers.Value = Data[12];
            NumChilds.Value = Data[13];
            NumWebClientsHTML5.Value = Data[14];
            NumWebClients.Value = Data[15];
            NumAlarms.Value = Data[16];
            NumScreens.Value = Data[17];
        }

        private void button20_Click(object sender, System.EventArgs e)
        {
            sglockCheck.Visible = false;
            rMovicon.Checked = true;
            //unlimited value is mandatory for SGLock 
            Unlimited.Checked = true;
            dateTimePicker1.Enabled = false;

            uint serial = (uint)numSerial.Value;
            string pt = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), string.Format("{0}.key", serial));
            string lic = WPFUtilities.CryptString.CryptString.EncryptString(GenerateLicenceXML());
            byte[] data = Encoding.ASCII.GetBytes(lic);
            File.WriteAllText(pt, lic);
            Process.Start("explorer.exe", string.Format("/select,{0}", pt));
        }

        private void rMovicon_CheckedChanged(object sender, System.EventArgs e)
        {
            mMovicon = (uint)MSZ.MSZDelRead.ApplicationType.apMovicon;
        }

        private void rMovTrace_CheckedChanged(object sender, System.EventArgs e)
        {
            mMovicon = (uint)MSZ.MSZDelRead.ApplicationType.apMovTrace;
        }

        private void rMoviconBA_CheckedChanged(object sender, System.EventArgs e)
        {
            mMovicon = (uint)MSZ.MSZDelRead.ApplicationType.apMoviconBA;
        }

        private void Unlimited_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !(sender as CheckBox).Checked;
        }

        private void btnCraddle_Click(object sender, EventArgs e)
        {
            StringBuilder param = new StringBuilder();
            //ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            DateTime aDay = dateTimePicker1.Value;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);
            param.Append(string.Format("ED={0};",aDate.ToString()));
            //UN Unlimited -> 0,1
            param.Append(string.Format("UN={0};", Unlimited.Checked ? 1 : 0));
            //DV Developer -> 0,1
            param.Append(string.Format("DV={0};", Developer.Checked ? 1 : 0));
            //RT Runtime -> 0,1
            param.Append(string.Format("RT={0};", Runtime.Checked ? 1 : 0));
            //SR Server -> 0,1
            param.Append(string.Format("SR={0};", Server.Checked ? 1 : 0));
            //DL Dataloggers -> 0,1
            param.Append(string.Format("DL={0};", Dataloggers.Checked ? 1 : 0));
            //CL Client -> 0,1
            param.Append(string.Format("CL={0};", Client.Checked ? 1 : 0));
            //RC Recipes -> 0,1
            param.Append(string.Format("RC={0};", Recipes.Checked ? 1 : 0));
            //VB VBNET -> 0,1
            param.Append(string.Format("VB={0};", VBNET.Checked ? 1 : 0));
            //SC Scheduler -> 0,1
            param.Append(string.Format("SC={0};", Scheduler.Checked ? 1 : 0));
            //WD WebDeploy -> 0,1
            param.Append(string.Format("WD={0};", WebDeploy.Checked ? 1 : 0));
            //DA DataAnalysis -> 0,1
            param.Append(string.Format("DA={0};", DataAnalysis.Checked ? 1 : 0));
            //OS OPCUAServer -> 0,1
            param.Append(string.Format("OS={0};", OPCUAServer.Checked ? 1 : 0));
            //TD ThreeD -> 0,1
            param.Append(string.Format("TD={0};", ThreeD.Checked ? 1 : 0));
            //AS AlarmStatistics -> 0,1
            param.Append(string.Format("AS={0};", AlarmStatistics.Checked ? 1 : 0));
            //GL Geolocalization -> 0,1
            param.Append(string.Format("GL={0};", Geolocalization.Checked ? 1 : 0));
            //DR Dispatcher -> 0,1
            param.Append(string.Format("DR={0};", Dispatcher.Checked ? 1 : 0));
            //NW Networking -> 0,1
            param.Append(string.Format("NW={0};", Networking.Checked ? 1 : 0));
            //RD Redundancy -> 0,1
            param.Append(string.Format("RD={0};", Redundancy.Checked ? 1 : 0));
            //{numeric}
            //NVS NumVarServer -> UINT32
            param.Append(string.Format("NVS={0};", NumVarServer.Value));
            //NVC NumVarClient -> UINT32
            param.Append(string.Format("NVC={0};", NumVarClient.Value));
            //NDR NumDrivers -> UINT32
            param.Append(string.Format("NDR={0};", NumDrivers.Value));
            //NCH NumChilds -> UINT32
            param.Append(string.Format("NCH={0};", NumChilds.Value));
            //NW5 NumWebClientsHTML5 -> UINT32
            param.Append(string.Format("NW5={0};", NumWebClientsHTML5.Value));
            //NWC NumWebClients -> UINT32
            param.Append(string.Format("NWC={0};", NumWebClients.Value));
            //NAL NumAlarms -> UINT32
            param.Append(string.Format("NAL={0};", NumAlarms.Value));
            //NSC NumScreens -> UINT32
            param.Append(string.Format("NSC={0};", NumScreens.Value));
            //NSR numSerial -> UINT32
            param.Append(string.Format("NSR={0}", numSerial.Value));

            var pippo = new Craddle();
            var lic = pippo.Generate(
                (uint)(rMoviconBA.Checked ? 
                    MSZ.MSZDelRead.ApplicationType.apMoviconBA : 
                        (rMovTrace.Checked ? MSZ.MSZDelRead.ApplicationType.apMovTrace : MSZ.MSZDelRead.ApplicationType.apMovicon)), 

                            textBox4.Text, 
                                param.ToString());

            var xml = GenerateLicenceXML();

            if (xml == null)
                return;
            string classic = WPFUtilities.CryptString.CryptString.EncryptString(xml);

            if (lic.Equals(classic))
                MessageBox.Show("It works!!");
            else
                MessageBox.Show("Check Failed!!");
        }



    }
}
