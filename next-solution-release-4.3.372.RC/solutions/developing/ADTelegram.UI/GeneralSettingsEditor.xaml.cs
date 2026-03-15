using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using System.Reflection;
using DevExpress.Xpo.DB;
using ADTelegram;
using System.ComponentModel;


namespace ADTelegram.UI
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

                
                DataContext = null;
                DataContext = configuration;

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
