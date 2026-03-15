using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Input;
using UFProjectWizard.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Utilities;
using WizardSettings;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Threading;
using DevExpress.Xpf.NavBar;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using Utilities.WPF;

namespace StandardWizard.ComponentService
{
    public class StandardWizardComponent : ComponentBase<IUFProjectWizardPlugin>, IUFProjectWizardPlugin
    {

        public static ProjectWizardUI wizard;
        public static StandardWizardComponent wizardPluginComponent { get; protected set; }

        #region IUFInterfaceBase Members
        void IUFInterfaceBase.Initialize()
        {
            if (wizardPluginComponent == null)
                wizardPluginComponent = this;
        }
        #endregion

        #region IProjectWizard Members

        internal static BitmapImage GetControlImage(string image)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("StartupWelcome", image);
            return bm;
        }

        public Uri CreateNewProject(Uri current, Window parent, object projectview)
        {
            LoadSettings();

            ProjectView = (ProjectViewModel)projectview; 

            wizard = new ProjectWizardUI();

            if (wizard == null) return null;

            wizard.DataContext = NewProject;
            GeneralDialogContent wnd = new GeneralDialogContent(wizard,
                WPFUtilities.Properties.Settings.Default.DialogFontSize,
                WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                GeneralDialogButtons.None, new Dictionary<GeneralDialogButtons, String>(), bmaximizeContent: true)
            {
                Owner = parent,
                DialogKeepContent = true
            };

            try
            {
                if (wnd.ShowDialog() == true)
                {
                    bool result = false;
                    using (new WaitCursor())
                    {
                        foreach (NavBarGroup item in wizard.wizardControl.Groups)
                        {
                            try
                            {
                                if (item.Content is IWizardElement)
                                    result = (item.Content as IWizardElement).Execute();
                            }
                            catch
                            {
                            }
                        }

                        SaveSettings();

                        return ProjectUri;
                    }
                }
            }
            finally
            {
                wizard.Dispose();
            }

            return null;
        }

        public bool IsStartable { get { return true; } }
        #endregion
        #region Isolated Storage
        readonly String StoreLayoutFileName = String.Format("{0}Layout.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        public void SaveSettings()
        {
            try
            {
                var name = Assembly.GetEntryAssembly().FullName;
                using (var Mutex = new Mutex(true, name))
                {
                    var isoStorage = GetStorage();
                    if (null == isoStorage)
                        return;

                    using (var stream = new IsolatedStorageFileStream(StoreLayoutFileName, FileMode.Create, isoStorage))
                    {

                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(String));
                                serializer.WriteObject(writer, StartingFolder);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadSettings()
        {
            try
            {
                var name = Assembly.GetEntryAssembly().FullName;
                using (var Mutex = new Mutex(true, name))
                {
                    var isoStorage = GetStorage();
                    if (null == isoStorage)
                        return;

                    using (Stream stream = new IsolatedStorageFileStream(StoreLayoutFileName, FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(String));
                                StartingFolder = serializer.ReadObject(reader) as String;
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Properties
        public static Uri ProjectUri { get; set; }
        public static ProjectWizardModel NewProject = new ProjectWizardModel();
        public static ProjectViewModel ProjectView;
        public static String StartingFolder = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
        #endregion

    }
}
