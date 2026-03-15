using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Utilities;
using System.Runtime.Serialization;
using System.IO;
using MSZ;
using UFUAServiceLibrary;
using DevExpress.Xpf.Core;
using WPFUtilities;

namespace UFUAInstallDongleService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {

        #region Settings
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings", typeof(MSZ.ServiceSettings), typeof(MainWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSettingsChanged), new CoerceValueCallback(OnCoerceSettings)));

        private static object OnCoerceSettings(DependencyObject o, object value)
        {
            MainWindow control = o as MainWindow;
            if (control != null)
                return control.OnCoerceSettings((MSZ.ServiceSettings)value);
            else
                return value;
        }

        private static void OnSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow control = o as MainWindow;
            if (control != null)
                control.OnSettingsChanged((MSZ.ServiceSettings)e.OldValue, (MSZ.ServiceSettings)e.NewValue);
        }

        protected virtual MSZ.ServiceSettings OnCoerceSettings(MSZ.ServiceSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSettingsChanged(MSZ.ServiceSettings oldValue, MSZ.ServiceSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public MSZ.ServiceSettings Settings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (MSZ.ServiceSettings)GetValue(SettingsProperty);
            }
            set
            {
                SetValue(SettingsProperty, value);
            }
        }

        #endregion
        
        #region Declarations
        internal CommadLineOptions cmdOptions;
        bool bLoaded;
        #endregion
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            ShowIcon = false;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
#if DEBUG
                    if (!System.Diagnostics.Debugger.IsAttached &&
                        Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                            "DebugMe - InstallDongleService", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        System.Diagnostics.Debugger.Launch();
#endif
                    ApplicationPropertiesHelper.SetProperty("CurrentSkin", cmdOptions.CurrentSkin);
                    ThemeHelper.SetTheme(this);

                    if (!string.IsNullOrEmpty(cmdOptions.Title))
                        Title = cmdOptions.Title;

                    serviceControl.cmdOptions = cmdOptions;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() => ExecuteReloadSettings());
                }
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand SaveSettings = new RoutedCommand();
        public static readonly RoutedCommand ReloadSettings = new RoutedCommand();
        #endregion

        #region Command Handlers
        private void OnSaveSettings(object sender, ExecutedRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings["ServerName"]))
            {
                serverName.Focus();
                serverName.SelectAll();
            }
            else if (!string.IsNullOrEmpty(Settings["PortNumber"]))
            {
                portNumber.Focus();
                portNumber.SelectAll();
            }
            else
                WriteSettings();
        }

        private void WriteSettings(bool onInitFile = false)
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                                     Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                     Properties.Settings.Default.CompanyName,
                                     Properties.Settings.Default.CommonApplicationFolder);
            var settingsfile = System.IO.Path.Combine(commonFolder, "msz_netcontrols.config");

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(settingsfile)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(settingsfile));

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            using (XmlWriter writer = XmlWriter.Create(settingsfile, settings))
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(ServiceSettings));
                    serializer.WriteObject(writer, Settings.ToXml());
                }
                catch (Exception ex)
                {
                    writer.Close();
                }
            }
            
            if(!onInitFile)
                MessageBox.Show(this, Properties.Resources.RestartService, this.Title);
        }
        private void CanSaveSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnReloadSettings(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteReloadSettings();
        }

        void ExecuteReloadSettings()
        {
            Settings = ReadSettings();

            serverName.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            portNumber.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        public ServiceSettings ReadSettings()
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                                     Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                     Properties.Settings.Default.CompanyName,
                                     Properties.Settings.Default.CommonApplicationFolder);
            var settingsfile = System.IO.Path.Combine(commonFolder, "msz_netcontrols.config");

            ServiceSettings ss = new ServiceSettings();
            if (!File.Exists(settingsfile))
            {
                ss.PortNumber = Properties.Settings.Default.ServerPort;
                ss.ServerName = Properties.Settings.Default.HostName;
                Settings = ss;
                WriteSettings(true);
            }

            using (XmlReader reader = XmlReader.Create(settingsfile))
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(ServiceSettings));
                    ss = (serializer.ReadObject(reader) as string).FromXml<ServiceSettings>();
                }
                catch (Exception ex)
                {
                    reader.Close();
                }
            }

            return ss;
        }

        private void CanReloadSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
    }
}
