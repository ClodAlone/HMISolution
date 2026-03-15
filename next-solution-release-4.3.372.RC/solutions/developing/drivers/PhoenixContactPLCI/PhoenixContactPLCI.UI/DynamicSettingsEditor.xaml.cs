using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace PhoenixContactPLCI.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded = false;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        PhoenixContactPLCIDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        PhoenixContactPLCIDynTagSettings thisTagSettings;
        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                var baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new PhoenixContactPLCIDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }

                if (!thisTag.IsMethod)
                {
                    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                    thisTagSettings.MethodID = -1;
                }

                DataContext = null;
                DataContext = thisTagSettings;
                baseDyn.DataContext = DataContext;
                if (LoadbaseDyn)
                    MainStack.Children.Add(baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    return;
                }

                ufw = new UnitOfWork(idl);

                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<PhoenixContactPLCIDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new PhoenixContactPLCIDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, PhoenixContactPLCIStationSettings>();
                    foreach (PhoenixContactPLCIStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                // disable default driver's unsupported feature
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
                
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                CmbDataFormat.ItemsSource = new List<PhoenixContactPLCIProtocol.VarType>()
                {
                    PhoenixContactPLCIProtocol.VarType.BOOL,
                    PhoenixContactPLCIProtocol.VarType.SINT,
                    PhoenixContactPLCIProtocol.VarType.INT,
                    PhoenixContactPLCIProtocol.VarType.DINT,
                    PhoenixContactPLCIProtocol.VarType.USINT,
                    PhoenixContactPLCIProtocol.VarType.UINT,
                    PhoenixContactPLCIProtocol.VarType.UDINT,
                    PhoenixContactPLCIProtocol.VarType.REAL,
                    PhoenixContactPLCIProtocol.VarType.LREAL,
                    //PhoenixContactPLCIProtocol.VarType.TIME,
                    PhoenixContactPLCIProtocol.VarType.BYTE,
                    PhoenixContactPLCIProtocol.VarType.WORD,
                    PhoenixContactPLCIProtocol.VarType.DWORD,
                    PhoenixContactPLCIProtocol.VarType.STRING,
                    PhoenixContactPLCIProtocol.VarType.STRUCT
                };

                if (((PhoenixContactPLCIProtocol.VarType)CmbDataFormat.SelectedValue) == PhoenixContactPLCIProtocol.VarType.UNKNOWN)
                    CmbDataFormat.SelectedIndex = 0;

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
                    var s = DataContext as PhoenixContactPLCIDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbDataFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((PhoenixContactPLCIProtocol.VarType)CmbDataFormat.SelectedValue) == PhoenixContactPLCIProtocol.VarType.STRING)
            {
                LbLStringLength.Visibility = System.Windows.Visibility.Visible;
                TxTStringLength.Visibility = System.Windows.Visibility.Visible;
            } else
            {
                LbLStringLength.Visibility = System.Windows.Visibility.Collapsed;
                TxTStringLength.Visibility = System.Windows.Visibility.Collapsed;
                TxTStringLength.Text = "0";
            }
        }

        #region Property
        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }
        #endregion


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
