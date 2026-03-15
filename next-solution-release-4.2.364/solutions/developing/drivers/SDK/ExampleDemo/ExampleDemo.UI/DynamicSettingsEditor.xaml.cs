using System;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace ExampleDemo.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {


        internal class DIn
        {
            public int id { get; set; }
            public String FriendlyName { get; set; }
        }

        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        ExampleDemoDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        ExampleDemoDynTagSettings thisTagSettings;
        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new ExampleDemoDynTagSettings() { IsMethod = thisTag.IsMethod };
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                }

                

                if (!thisTag.IsMethod)
                {
                    CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                    TMethod.Visibility = System.Windows.Visibility.Collapsed;
                    thisTagSettings.MethodID = -1;
                }
                else
                {
                    TDemoType.Visibility = System.Windows.Visibility.Collapsed;
                    CmbDemoType.Visibility = System.Windows.Visibility.Collapsed;
                    
                }
                
                DataContext = thisTagSettings;

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                ufw = new UnitOfWork(idl);

                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<ExampleDemoDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch
                {
                    configuration = new ExampleDemoDriverSettings(ufw);//driver specific class
                    configuration.DefaultSettings();
                }

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                CmbDemoType.ItemsSource = Enum.GetValues(typeof(ExampleDemoTypes));
                CmbMethod.ItemsSource = Enum.GetValues(typeof(ExampleDemoMethods));
            };

            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded && bVisibleOnce)
                {
                    bLoaded = false;
                    var s = DataContext as ExampleDemoDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
