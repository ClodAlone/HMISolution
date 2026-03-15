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
using DocumentManager.ComponentService;
using UriResolver.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using ScreenManager.ComponentService;
using UFUAEditor.ComponentService;
using UFProjectManager.ComponentService;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpf.NavBar;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using Utilities.WPF;

namespace NewDriverWizard.ComponentService
{
    public class NewDriverWizardComponent : ComponentBase<IUFNewDriverWizard>, IUFNewDriverWizard
    {

        public static ProjectWizardUI wizard;

        public static UFInterfaces.Editors.DriverXmlInfo xmlDriver = null;
        public static String driver = null;
        public static String currentConn = null;
        public static object genSett;
        public static String channelName;
        public static String StationName;
        public static NewDriverWizardComponent wizardPluginComponent { get; protected set; }

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

        static void CleanSavedInfo()
        {
            xmlDriver = null;
            driver = null;
            currentConn = null;
            genSett = null;
            channelName = null;
            StationName = null;
        }

        internal void UpdateHelpLink(FrameworkElement frameworkElement, string helpLink)
        {
            GeneralDialog generalDialogContent = frameworkElement?.FindParent<GeneralDialog>();
            if (generalDialogContent != null)
                generalDialogContent.HelpLink = helpLink;
        }

        internal string GetHelpLink(FrameworkElement frameworkElement)
        {
            GeneralDialog generalDialogContent = frameworkElement?.FindParent<GeneralDialog>();
            if (generalDialogContent != null)
                return generalDialogContent.HelpLink;
            else
                return null;
        }

        public object ConfigureNewDriver(IDocument doc, object projectview, String conn)
        {
            CleanSavedInfo();
            LoadSettings();

            ProjectView = (ProjectViewModel)projectview;
            currentConn = conn;

            object retValue = null;

            wizard = new ProjectWizardUI();

            if (wizard == null) return null;

            if (FontStyleHelper.hasCustomFontSize)
                wizard.FontSize = FontStyleHelper.customFontSize;
            if (FontStyleHelper.hasCustomFontFamily)
                wizard.FontFamily = FontStyleHelper.customFontFamily;

            wizard.DataContext = doc;// NewProject;
            GeneralDialogContent wnd = new GeneralDialogContent(wizard,
                WPFUtilities.Properties.Settings.Default.DialogFontSize,
                WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                GeneralDialogButtons.None, new Dictionary<GeneralDialogButtons, String>(), bmaximizeContent: true)
            {
                Owner = Application.Current.MainWindow,
                DialogKeepContent = true,
                HelpLink = "NewDrivertWizard"
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
                                if (item.Content is NewDriverWizard.Step2 &&
                                    (item.Content as NewDriverWizard.Step2).stepContent.Children.Count > 0)
                                {
                                    var genSettControl =
                                    ((item.Content as NewDriverWizard.Step2).stepContent.Children[0] as UserControl/*DriverCodeBase.Controls.BaseGeneralSettings*/);
                                    if (genSettControl != null)
                                    {
                                        string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                                        var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(
                                            String.Format("{0}\\Drivers\\{1}", rootPath,
                                            NewDriverWizardComponent.driver));

                                        NewDriverWizardComponent.driver = string.Empty;
                                        ICommunicationDriverWpfEditing driverWpfEditing = null;
                                        try
                                        {
                                            var types = Assembly.LoadFile(uidll).GetTypes();
                                            var list = (from t in types/*.AsParallel()*/
                                                        where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                                        select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                                            driverWpfEditing = list[0];

                                            if (driverWpfEditing == null)
                                                return false;
                                            driverWpfEditing.SaveSettings(genSettControl);

                                            retValue = xmlDriver;
                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            //if (Document.EditorManagerComponent.UIInterface != null)
                                            //    Document.EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.CommDriverNotFound, uidll));
                                            retValue = null;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        if (retValue == null)
                            SaveSettings();
                    }
                }
            }
            finally
            {
                wizard.Dispose();
            }

            return retValue;
        }
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
