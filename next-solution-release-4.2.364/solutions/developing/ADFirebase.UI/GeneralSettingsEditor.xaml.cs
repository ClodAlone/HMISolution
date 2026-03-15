using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using System.Reflection;
using System.IO;
using DevExpress.Xpo.DB;
using Microsoft.Win32;
using System.Windows;

namespace ADFirebase.UI
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
        bool isDbProject;

        public GeneralSettingsEditor()
        {
            InitializeComponent();
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

                isDbProject = XpoHelpers.XpoHelper.IsSQlDataProvider(conn);

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

        private void btnBrowseServiceAccountKeyFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            var dialog = new OpenFileDialog()
            {
                Filter = String.Format("{0}|*.json",Properties.Resources.Filter),
                Title = Properties.Resources.FileDialogTitle
            };

            if (dialog.ShowDialog() == true)
            {
                if(!isDbProject)
                {
                    string name = System.IO.Path.GetFileName(dialog.FileName);
                    if (MessageBox.Show(CommonControls.Properties.Resources.AskCopyFile, name, MessageBoxButton.YesNo, MessageBoxImage.Question)
                        == MessageBoxResult.Yes)
                    {
                        if (!isDbProject)
                        {
                            var dest = Path.GetDirectoryName(fileBase);
                            string destfilename = System.IO.Path.Combine(dest, name);
                            System.IO.File.Copy(dialog.FileName, destfilename, true);
                            txtServiceAccountKeyFile.Text = name;
                        }
                        else
                        {
                            using (var idl = GetServiceAccountFile())
                            {
                                using (UnitOfWork uow = new UnitOfWork(idl))
                                {
                                    List<JSONFile> Files = (from S in new XPQuery<JSONFile>(ufw).AsParallel() select S).ToList();
                                    if (Files.Count > 0)
                                    {
                                        // before to import delete previous data
                                        ufw.Delete(Files);
                                        ufw.CommitChanges();
                                    }
                                    byte[] fileBody;
                                    fileBody = File.ReadAllBytes(dialog.FileName);
                                    ADFirebase.JSONFile jSON = new ADFirebase.JSONFile(ufw);

                                    jSON.FileBody = fileBody;
                                    ufw.CommitChanges();
                                    txtServiceAccountKeyFile.Text = Properties.Resources.FileOnDB;
                                }
                            }
                        }
                    }
                    else
                        txtServiceAccountKeyFile.Text = dialog.FileName;
                }
            }

            IDataLayer GetServiceAccountFile()
            {
                IDataLayer idl;

                idl = XpoDefault.GetDataLayer(conn, AutoCreateOption.DatabaseAndSchema);

                return idl;
            }

        }
    }
}
