using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpo;
using ADGsmSMS;
using System.Reflection;
using DevExpress.Xpo.DB;
using System.IO;
using System.Xml;
using System.IO.Ports;

namespace ADGsmSMS.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {
        string conn;
        IDataLayer idl = null;
        UnitOfWork ufw = null;
        PluginSettings configuration = null;
        string fileBase;
        InMemoryDataStore InMemory = null;
        bool alreadyLoaded = false;
        bool protect;

        public GeneralSettingsEditor()
        {
            InitializeComponent();

            DataContextChanged += (o, e) =>
                {
                    //if (alreadyLoaded)
                    //    return;

                    //alreadyLoaded = true;

                    List<string> lista = DataContext as List<string>;
                    if (lista == null || lista.Count < 2)
                        return;

                    conn = lista[0];//DataContext as string;
                    protect = (lista[1].ToLower().IndexOf("true") != -1);

                    Assembly a = Assembly.GetAssembly(this.GetType());
                    string DriverName = a.GetName().Name;
                    DriverName = DriverName.Replace(".UI", "");
                    idl = ADPluginBase.PluginBase.GetPluginDataLayer(conn, DriverName, false);
                    fileBase = ADPluginBase.PluginBase.GetFileBase(conn, DriverName);

                    if (fileBase.Length > 0)
                    {
                        InMemory = ADPluginBase.PluginBase.GetDataStore(fileBase);
                        if (idl != null)
                            idl.Dispose();
                        idl = new SimpleDataLayer(InMemory);
                    }
                    if (ufw != null)
                        ufw.Dispose();
                    ufw = new UnitOfWork(idl);

                    try
                    {
                        var conf = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).ToList();
                        if (conf.Count == 0)
                        {
                            configuration = new PluginSettings(ufw);
                            configuration.DefaultSettings();
                        }
                        if (conf.Count > 0)
                            configuration = conf[0];
                        while (conf.Count > 1)
                        {
                            conf[1].Delete();
                            conf.Remove(conf[1]);
                        }
                        //configuration = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).Single();
                    }
                    catch (InvalidOperationException ex)
                    {
                        configuration = new PluginSettings(ufw);
                        configuration.DefaultSettings();
                    }

                };

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;

                //List<string> lista = DataContext as List<string>;
                //if (lista == null || lista.Count < 2)
                //    return;

                //conn = lista[0];//DataContext as string;
                //protect = (lista[1].ToLower().IndexOf("true") != -1);

                //Assembly a = Assembly.GetAssembly(this.GetType());
                //string DriverName = a.GetName().Name;
                //DriverName = DriverName.Replace(".UI", "");
                //idl = ADPluginBase.PluginBase.GetPluginDataLayer(conn, DriverName, false);
                //fileBase = ADPluginBase.PluginBase.GetFileBase(conn, DriverName);

                //if (fileBase.Length > 0)
                //{
                //    InMemory = ADPluginBase.PluginBase.GetDataStore(fileBase);
                //    idl = new SimpleDataLayer(InMemory);
                //}

                //ufw = new UnitOfWork(idl);

                //try
                //{
                //    var conf = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).ToList();
                //    if (conf.Count == 0)
                //    {
                //        configuration = new PluginSettings(ufw);
                //        configuration.DefaultSettings();
                //    }
                //    if (conf.Count > 0)
                //        configuration = conf[0];
                //    while (conf.Count > 1)
                //    {
                //        conf[1].Delete();
                //        conf.Remove(conf[1]);
                //    }
                //    //configuration = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).Single();
                //}
                //catch (InvalidOperationException ex)
                //{
                //    configuration = new PluginSettings(ufw);
                //    configuration.DefaultSettings();
                //}

                DataContext = null;
                DataContext = configuration;

                Dictionary<int, string> l = new Dictionary<int, string>();
                l.Add(1, "Com1");
                l.Add(2, "Com2");
                l.Add(3, "Com3");
                l.Add(4, "Com4");
                l.Add(5, "Com5");
                l.Add(6, "Com6");
                l.Add(7, "Com7");
                l.Add(8, "Com8");
                l.Add(9, "Com9");
                l.Add(10, "Com10");
                CmbPort.ItemsSource = l;

                Dictionary<int, string> m = new Dictionary<int, string>();
                m.Add(1, "110");
                m.Add(2, "300");
                m.Add(3, "600");
                m.Add(4, "1200");
                m.Add(5, "2400");
                m.Add(6, "4800");
                m.Add(7, "9600");
                m.Add(8, "14400");
                m.Add(9, "19200");
                m.Add(10, "38400");
                m.Add(11, "56000");
                m.Add(12, "57600");
                m.Add(13, "115200");
                m.Add(14, "128000");
                m.Add(15, "256000");
                CmbBaud.ItemsSource = m;

                Dictionary<int, string> n = new Dictionary<int, string>();
                n.Add(0, "None");
                n.Add(1, "Odd");
                n.Add(2, "Even");
                n.Add(3, "Mark");
                n.Add(4, "Space");
                CmbParity.ItemsSource = n;


                Dictionary<int, string> q = new Dictionary<int, string>();
                //q.Add(0, "None");
                q.Add(1, "One");
                q.Add(2, "Two");
                q.Add(3, "One and a half");
                CmbStop.ItemsSource = q;

                Dictionary<int, string> p = new Dictionary<int, string>();
                p.Add(0, "None");
                p.Add(1, "XOnXOff");
                p.Add(2, "RequestToSend");
                p.Add(3, "RequestToSendXOnXOff");
                CmbHandShake.ItemsSource = p;

                /*Entry.IsEnabled = configuration.RasEnable;
                PhoneNumber.IsEnabled = configuration.RasEnable;
                RASUser.IsEnabled = configuration.RasEnable;
                RASPassword.IsEnabled = configuration.RasEnable;
                RetryTime.IsEnabled = configuration.RasEnable;
                DisconnectAfter.IsEnabled = configuration.RasEnable;
                Retries.IsEnabled = configuration.RasEnable;
                PhonebookPath.IsEnabled = configuration.RasEnable;*/
            };
        }

        

        public void SavePluginSettings()
        {
            ufw.CommitChanges();
            ADPluginBase.PluginBase.SavePluginSettings(InMemory, fileBase, protect);
        }
        #region IDisposable Members
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }


        #endregion

    }
}
