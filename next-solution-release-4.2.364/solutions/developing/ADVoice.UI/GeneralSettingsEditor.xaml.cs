using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Ozeki.Media;

namespace ADVoice.UI
{
    /// <summary>
    /// Interaction logic for GeneralSettingsEditor.xaml
    /// </summary>
    public partial class GeneralSettingsEditor : UserControl, IDisposable
    {

        string conn;
        IDataLayer idl = null;
        UnitOfWork ufw = null;
        ADVoicePluginSettings configuration = null;
        string fileBase;
        InMemoryDataStore InMemory = null;
        bool protect;
        static TextToSpeech _textToSpeech;

        bool alreadyLoaded = false;

        public GeneralSettingsEditor()
        {
            InitializeComponent();

            if (_textToSpeech == null)
                _textToSpeech = new TextToSpeech();

            var listvoices = _textToSpeech.GetAvailableVoices();
            //error, no voices!!!
            if (listvoices.Count > 0)
            {
                var list = (from v in listvoices select v.Name).ToList();
                SelectedVoice.ItemsSource = list;
            }

            DataContextChanged += (o, e) =>
            {
                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                conn = lista[0];
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
                    var conf = (from tag in new XPQuery<ADVoicePluginSettings>(ufw).AsParallel() select tag).ToList();
                    if (conf.Count == 0)
                    {
                        configuration = new ADVoicePluginSettings(ufw);
                        configuration.DefaultSettings();
                    }
                    if (conf.Count > 0)
                        configuration = conf[0];
                    while (conf.Count > 1)
                    {
                        conf[1].Delete();
                        conf.Remove(conf[1]);
                    }


                }
                catch (Exception ex)
                {
                    configuration = new ADVoicePluginSettings(ufw);
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

                //conn = lista[0];
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
                //    var conf = (from tag in new XPQuery<ADSmtpPluginSettings>(ufw).AsParallel() select tag).ToList();
                //    if (conf.Count == 0)
                //    {
                //        configuration = new ADSmtpPluginSettings(ufw);
                //        configuration.DefaultSettings();
                //    }
                //    if(conf.Count > 0)
                //        configuration = conf[0];
                //    while(conf.Count > 1)
                //    {
                //        conf[1].Delete();
                //        conf.Remove(conf[1]);
                //    }


                //}
                //catch (Exception ex)
                //{
                //    configuration = new ADSmtpPluginSettings(ufw);
                //    configuration.DefaultSettings();
                //}

                DataContext = null;
                DataContext = configuration;
            };
        }

        private void SelectedVoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
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
