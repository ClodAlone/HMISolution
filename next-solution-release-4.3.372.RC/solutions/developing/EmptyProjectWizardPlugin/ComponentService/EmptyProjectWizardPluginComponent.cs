using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows;
using System.Linq;
using Utilities;
using System.Windows.Input;
using UFProjectWizard.ComponentService;
using WizardSettings;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using DevExpress.Xpf.NavBar;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using Utilities.WPF;

namespace EmptyProjectWizardPlugin.ComponentService
{
    public class EmptyProjectWizardPluginComponent : ComponentBase<IUFProjectWizardPlugin>, IUFProjectWizardPlugin
    {

        public static ProjectWizardUI wizard;
        public static EmptyProjectWizardPluginComponent wizardPluginComponent { get; protected set; }

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
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("WizardPlugins", image);
            return bm;
        }
        #endregion
        #region IProjectWizard Members

        public Uri CreateNewProject(Uri current, Window parent,object projectview)
        {
            LoadSettings();

            ProjectView = (ProjectViewModel)projectview;
#if CONNEXT
            NewProject.Architecture = ArchType.local;
#endif
            wizard = new ProjectWizardUI();

            if (wizard == null) return null;

            if (FontStyleHelper.hasCustomFontSize)
                wizard.FontSize = FontStyleHelper.customFontSize;
            if (FontStyleHelper.hasCustomFontFamily)
                wizard.FontFamily = FontStyleHelper.customFontFamily;

            wizard.DataContext = NewProject;
            GeneralDialogContent wnd = new GeneralDialogContent(wizard,
                WPFUtilities.Properties.Settings.Default.DialogFontSize,
                WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                GeneralDialogButtons.None, new Dictionary<GeneralDialogButtons, String>(), bmaximizeContent: true)
            {
                Owner = parent,
                DialogKeepContent = true,
                HelpLink = "EmptyProjectWizard"
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
                            if (item.Content is IWizardElement)
                                result = (item.Content as IWizardElement).Execute();
                            if (result)
                                break;
                        }

                        SaveSettings();

                        if (!result)
                            return ProjectUri;
                        else
                            return null;
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

        #region Properties
        public static Uri ProjectUri { get; set; }
        public static ProjectWizardModel NewProject = new ProjectWizardModel();
        public static ProjectViewModel ProjectView;
        public static String StartingFolder = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
        public static String StartingDBFolder = string.Empty;
        #endregion
        #region Isolated Storage
        readonly String StoreDBFileName = String.Format("{0}DBLayout.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        readonly String StoreFileName = String.Format("{0}Layout.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

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

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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

                    using (var stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
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
                    using (var stream = new IsolatedStorageFileStream(StoreDBFileName, FileMode.Create, isoStorage))
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
                                serializer.WriteObject(writer, StartingDBFolder);
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
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                try
                {
                    var name = Assembly.GetEntryAssembly().FullName;
                    using (var Mutex = new Mutex(true, name))
                    {
                        var isoStorage = GetStorage();
                        if (null == isoStorage)
                            return;

                        using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
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
                        using (Stream stream = new IsolatedStorageFileStream(StoreDBFileName, FileMode.OpenOrCreate, isoStorage))
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
                                    StartingDBFolder = serializer.ReadObject(reader) as String;
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
        }

        #endregion
    }
}
