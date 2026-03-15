using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace EtherNetIP.UI
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
        EtherNetIPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        EtherNetIPDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"AddressType", new SettingsControls.AddressType()},
            {"TagFormat", new SettingsControls.TagFormat() },
            {"ABAddres", new SettingsControls.ABAddress() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbAddressType = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbTagFormat = null;
        //bool alreadyLoaded = false;

        DependencyPropertyDescriptor descriptor = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

                //bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new EtherNetIPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    //LoadbaseDyn = true;
                }

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
                //AddressType
                if (DynamicSettingsDict.TryGetValue("AddressType", out row))
                {
                    CmbAddressType = (ComboBox)row.FindName("CmbAddressType");
                    if (CmbAddressType != null)
                        CmbAddressType.ItemsSource = Enum.GetValues(typeof(AddressTypes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //TagFormat
                if (DynamicSettingsDict.TryGetValue("TagFormat", out row))
                {
                    CmbTagFormat = (ComboBox)row.FindName("CmbTagFormat");
                    if (CmbTagFormat != null)
                        CmbTagFormat.ItemsSource = Enum.GetValues(typeof(TagFormats));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //ABAddres
                if (DynamicSettingsDict.TryGetValue("ABAddres", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
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
                if (baseSettingsCtrls.TryGetValue("SwapBytes", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Swap Words*
                if (baseSettingsCtrls.TryGetValue("SwapWords", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Element Number*
                if (baseSettingsCtrls.TryGetValue("ElementNumber", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
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
                    configuration = (from tag in new XPQuery<EtherNetIPDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new EtherNetIPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, EtherNetIPStationSettings>();
                    foreach (EtherNetIPStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }


                descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbStation, Base_StationChanged);
                Base_StationChanged();

                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //CmbAddressType.ItemsSource = Enum.GetValues(typeof(AddressTypes));
                CmbTagFormat.ItemsSource = Enum.GetValues(typeof(TagFormats));

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
                    var s = DataContext as EtherNetIPDynTagSettings;
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

        private void Base_StationChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;

            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedValue == null)
                return;


            string station = cb.SelectedValue.ToString();

            if (!thisTagSettings.stationSettingList.ContainsKey(station))
                return;

            EtherNetIPStationSettings st = thisTagSettings.stationSettingList[station];

            if ((st.PlcType != PlcTypes.ControlLogix_CompactLogix) &&
                (st.PlcType != PlcTypes.Micro800_series))
            {                
                CmbAddressType.SelectedIndex = (int)AddressTypes.DataFile;
                CmbAddressType.IsEnabled = false;
            }
            else if (st.PlcType == PlcTypes.Micro800_series)
            {
                CmbAddressType.SelectedIndex = (int)AddressTypes.TagName;
                CmbAddressType.IsEnabled = false;
            }
            else
            {
                CmbAddressType.IsEnabled = true;
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
