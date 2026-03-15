////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynamicSettingsEditor.xaml.cs
//
// summary:	Implements the dynamic settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

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

namespace GESRTP2.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;
        /// <summary>   true to visible once. </summary>
        bool bVisibleOnce;
        /// <summary>   The idl. </summary>
        IDataLayer idl;
        /// <summary>   The ufw. </summary>
        UnitOfWork ufw;
        /// <summary>   The configuration. </summary>
        GESRTP2DriverSettings configuration;
        /// <summary>   this tag. </summary>
        IDynamicSettingsEditing thisTag;
        /// <summary>   this tag settings. </summary>
        GESRTP2DynTagSettings thisTagSettings;
        /// <summary>   Default constructor. </summary>
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"AreaType", new SettingsControls.AreaType()},
            {"StartAddress", new SettingsControls.StartAddress() },
            {"StringLength", new SettingsControls.StringLength() },
            {"SymbolicAddress", new SettingsControls.SymbolicAddress() }
        };

        ComboBox cmbStation = null;
        ComboBox cmbLinkType = null;
        ComboBox cmbAreaType = null;
        TextBlock TxBStringLength = null;
        TextBox TxStringLength = null;
        CheckBox chkSwapBytes;
        CheckBox chkSwapWords;

        DependencyPropertyDescriptor descriptor = null;

        UserControl rowStartAddress;
        UserControl rowSymbolicAddress;
        UserControl rowElementNr;
        UserControl rowSwapBytes;
        UserControl rowSwapWords;
        UserControl rowStringLength;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new GESRTP2DynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                }

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
                // Area Type
                if (DynamicSettingsDict.TryGetValue("AreaType", out row))
                {
                    cmbAreaType = (ComboBox)row.FindName("CmbAreaType");
                    if (cmbAreaType != null)
                        cmbAreaType.ItemsSource = Enum.GetValues(typeof(AreaTypes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);

                    descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                    descriptor.AddValueChanged(cmbAreaType, Base_CmbAreaTypeChanged);
                    Base_CmbAreaTypeChanged();
                }

                //Start Address
                if (DynamicSettingsDict.TryGetValue("StartAddress", out rowStartAddress))
                {
                    rowStartAddress.DataContext = DataContext;
                    MainStack.Children.Add(rowStartAddress);
                }

                //Symbolic Address
                if (DynamicSettingsDict.TryGetValue("SymbolicAddress", out rowSymbolicAddress))
                {
                    rowSymbolicAddress.DataContext = DataContext;
                    MainStack.Children.Add(rowSymbolicAddress);
                }

                //String Length
                if (DynamicSettingsDict.TryGetValue("StringLength", out rowStringLength))
                {
                    TxBStringLength = (TextBlock)rowStringLength.FindName("TxBStringLength");
                    TxStringLength = (TextBox)rowStringLength.FindName("TxStringLength");
                    rowStringLength.DataContext = DataContext;
                    MainStack.Children.Add(rowStringLength);
                }
                //Element Number*
                if (baseSettingsCtrls.TryGetValue("ElementNumber", out rowElementNr))
                {
                    rowElementNr.DataContext = DataContext;
                    MainStack.Children.Add(rowElementNr);
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
                    cmbLinkType = (ComboBox)row.FindName("CmbLinkType");
                    MainStack.Children.Add(row);
                }
                //Station*
                if (baseSettingsCtrls.TryGetValue("Station", out row))
                {
                    row.DataContext = DataContext;
                    cmbStation = (ComboBox)row.FindName("CmbStation");
                    MainStack.Children.Add(row);
                }
                //Swap Bytes*
                if (baseSettingsCtrls.TryGetValue("SwapBytes", out rowSwapBytes))
                {
                    chkSwapBytes = (CheckBox)rowSwapBytes.FindName("CheckSwapBytes");
                    rowSwapBytes.DataContext = DataContext;
                    MainStack.Children.Add(rowSwapBytes);
                }
                //Swap Words*
                if (baseSettingsCtrls.TryGetValue("SwapWords", out rowSwapWords))
                {
                    chkSwapWords = (CheckBox)rowSwapWords.FindName("CheckSwapWords");
                    rowSwapWords.DataContext = DataContext;
                    MainStack.Children.Add(rowSwapWords);
                }
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
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
                    configuration = (from tag in new XPQuery<GESRTP2DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new GESRTP2DriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    //insert Station in ComboBox baseDyn.CmbStation
                    cmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    cmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, GESRTP2StationSettings>();
                    foreach (GESRTP2StationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(cmbStation, Base_CmbStationChanged);
                Base_CmbStationChanged();

                if (cmbLinkType != null)
                    cmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    if (TxBStringLength != null)
                        TxBStringLength.Visibility = Visibility.Collapsed;
                    if (TxStringLength != null)
                        TxStringLength.Visibility = Visibility.Collapsed;
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
                    var s = DataContext as GESRTP2DynTagSettings;
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
                        
            if ((AreaTypes)cb.SelectedValue == AreaTypes.Symbolic) 
            {
                rowStartAddress.Visibility = Visibility.Collapsed;
                rowSymbolicAddress.Visibility = Visibility.Visible;
                rowElementNr.Visibility = Visibility.Collapsed;
                rowSwapBytes.Visibility = Visibility.Collapsed;
                rowSwapWords.Visibility = Visibility.Collapsed;
                rowStringLength.Visibility = Visibility.Collapsed;
                chkSwapBytes.IsChecked = false;
                chkSwapWords.IsChecked = false;
            } 
            else
            {
                rowStartAddress.Visibility = Visibility.Visible;
                rowSymbolicAddress.Visibility = Visibility.Collapsed;
                rowElementNr.Visibility = Visibility.Visible;
                rowSwapBytes.Visibility = Visibility.Visible;
                rowSwapWords.Visibility = Visibility.Visible;
                rowStringLength.Visibility = Visibility.Visible;
            }
        }

        private void Base_CmbStationChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;

            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedValue == null)
                return;

            AreaTypes[] areas = (AreaTypes[])Enum.GetValues(typeof(AreaTypes));
            if (thisTagSettings.stationSettingList.ContainsKey(cb.SelectedValue.ToString()))
            {
                if (!GESRTP2Protocol.PlcSupportSymbolic(thisTagSettings.stationSettingList[cb.SelectedValue.ToString()].PlcType))
                    areas = areas.Where(a => (AreaTypes)a != AreaTypes.Symbolic).ToArray();
            }
            
            cmbAreaType.ItemsSource = areas;            
        }

        /// <summary>   The connection. </summary>
        private string _Connection;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the connection. </summary>
        ///
        /// <value> The connection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }

        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            DynamicSettingsDict.Clear();

            if (baseSettingsCtrls != null)
                baseSettingsCtrls.Clear();

            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
