using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace MelsecQEth.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        MelsecQEthDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        MelsecQEthDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"AddressType", new SettingsControls.AddressType()},
            {"Address", new SettingsControls.Address() },
            {"CpuTarget", new SettingsControls.CpuTarget() },
            {"StringLength", new SettingsControls.StringLength() },
            {"UnicodeString", new SettingsControls.UnicodeString() }
        };

        ComboBox CmbAddressType = null;
        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbCpuTarget = null;
        CheckBox ChkSwapBytes = null;
        CheckBox ChkSwapWords = null;        

        DependencyPropertyDescriptor descriptor = null;

        UserControl rowAddressType;
        UserControl rowAddress;
        UserControl rowElementNr;
        UserControl rowSwapBytes;
        UserControl rowSwapWords;        
        UserControl rowStringLength;
        UserControl rowUnicodeString;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                baseSettingsCtrls = new DriverCodeBaseEx.UI.BaseSettings().GetBaseDynamicSettings();
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new MelsecQEthDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                    {
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    }
                    thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;//(DataContext as UFUAModel.UFUATag).DataType;
                    // FOGBUGZ 9836
                    thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                }
                
                DataContext = null;
                DataContext = thisTagSettings;
                UserControl row;
                //AddressType
                if (DynamicSettingsDict.TryGetValue("AddressType", out rowAddressType))
                {
                    CmbAddressType = (ComboBox)rowAddressType.FindName("CmbAddressType");
                    if (CmbAddressType != null)
                        CmbAddressType.ItemsSource = Enum.GetValues(typeof(MelsecQEthProtocol.AddressTypes));
                    rowAddressType.DataContext = DataContext;
                    MainStack.Children.Add(rowAddressType);

                    descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                    descriptor.AddValueChanged(CmbAddressType, Base_CmbAreaTypeChanged);
                    Base_CmbAreaTypeChanged();
                }
                //Address
                if (DynamicSettingsDict.TryGetValue("Address", out rowAddress))
                {
                    rowAddress.DataContext = DataContext;
                    MainStack.Children.Add(rowAddress);
                }
                //CpuTarget
                if (DynamicSettingsDict.TryGetValue("CpuTarget", out row))
                {
                    row.DataContext = DataContext;
                    CmbCpuTarget = (ComboBox)row.FindName("CmbCpuTarget");
                    if (CmbCpuTarget != null)
                    {
                        CmbCpuTarget.ItemsSource = Enum.GetValues(typeof(CpuTargets));
                    }
                    MainStack.Children.Add(row);
                }
                //StringLength
                if (DynamicSettingsDict.TryGetValue("StringLength", out rowStringLength))
                {
                    rowStringLength.DataContext = DataContext;
                    MainStack.Children.Add(rowStringLength);
                }
                //UnicodeString
                if (DynamicSettingsDict.TryGetValue("UnicodeString", out rowUnicodeString))
                {
                    rowUnicodeString.DataContext = DataContext;
                    MainStack.Children.Add(rowUnicodeString);
                }
                //Method*
                if (baseSettingsCtrls.TryGetValue("Method", out row))
                {
                    var cm = (ComboBox)row.FindName("CmbMethod");
                    if (cm != null)
                        cm.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                    if (!thisTag.IsMethod)
                    {
                        row.Visibility = System.Windows.Visibility.Collapsed;
                        thisTagSettings.MethodID = -1;
                    }
                }
                //Link Type*
                if (baseSettingsCtrls.TryGetValue("LinkType", out row))
                {
                    row.DataContext = DataContext;
                    CmbLinkType = (ComboBox)row.FindName("CmbLinkType");
                    if (CmbLinkType != null)
                    {
                        CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                    }
                    MainStack.Children.Add(row);
                }
                //Station*
                if (baseSettingsCtrls.TryGetValue("Station", out row))
                {
                    row.DataContext = DataContext;
                    CmbStation = (ComboBox)row.FindName("CmbStation");
                    MainStack.Children.Add(row);
                }
                //Swap Bytes*
                if (baseSettingsCtrls.TryGetValue("SwapBytes", out rowSwapBytes))
                {
                    ChkSwapBytes = (CheckBox)rowSwapBytes.FindName("CheckSwapBytes");
                    rowSwapBytes.DataContext = DataContext;
                    MainStack.Children.Add(rowSwapBytes);
                }
                //Swap Words*
                if (baseSettingsCtrls.TryGetValue("SwapWords", out rowSwapWords))
                {
                    ChkSwapWords = (CheckBox)rowSwapWords.FindName("CheckSwapWords");
                    rowSwapWords.DataContext = DataContext;
                    MainStack.Children.Add(rowSwapWords);
                }
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Element Number*
                if (baseSettingsCtrls.TryGetValue("ElementNumber", out rowElementNr))
                {
                    rowElementNr.DataContext = DataContext;
                    MainStack.Children.Add(rowElementNr);
                }
                //Conditional Variable*
                if (baseSettingsCtrls.TryGetValue("JobConditionalVariable", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<MelsecQEthDriverSettings>(ufw).AsParallel() select tag).Single();
                    
                }
                catch (InvalidOperationException ex)
                {
                    configuration = new MelsecQEthDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, MelsecQEthStationSettings>();
                    foreach (MelsecQEthStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    if ((rowStringLength != null))
                    {
                        rowStringLength.Visibility = Visibility.Collapsed;
                        rowUnicodeString.Visibility = Visibility.Collapsed;
                    }
                }

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, MelsecQEthStationSettings>();
                    foreach (MelsecQEthStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                if (thisTag.IsMethod && !thisTagSettings.IsMethodSupported)
                {
                    MainStack.Children.Clear();
                    MainStack.HorizontalAlignment = HorizontalAlignment.Center;
                    MainStack.VerticalAlignment = VerticalAlignment.Center;

                    var NoMethod = new DriverCodeBaseEx.UI.Controls.BaseDynamicSettingsNoMethod();
                    NoMethod.DataContext = DataContext;
                    MainStack.Children.Add(NoMethod);
                }
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
                    var s = DataContext as MelsecQEthDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void Base_CmbAreaTypeChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;

            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedValue == null)
                return;

            if ((MelsecQEthProtocol.AddressTypes)cb.SelectedValue == MelsecQEthProtocol.AddressTypes.Label)
            {                
                rowElementNr.Visibility = Visibility.Collapsed;
                rowSwapBytes.Visibility = Visibility.Collapsed;
                rowSwapWords.Visibility = Visibility.Collapsed;
                ChkSwapBytes.IsChecked = false;
                ChkSwapWords.IsChecked = false;
            }
            else
            {
                rowElementNr.Visibility = Visibility.Visible;
                rowSwapBytes.Visibility = Visibility.Visible;
                rowSwapWords.Visibility = Visibility.Visible;                
            }
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
