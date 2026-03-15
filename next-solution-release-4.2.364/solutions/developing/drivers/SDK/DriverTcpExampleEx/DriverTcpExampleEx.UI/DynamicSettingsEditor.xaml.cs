using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using System.Windows.Markup;
using System.Collections.Generic;

namespace DriverTcpExample.UI
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
        DriverTcpExampleDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        DriverTcpExampleDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"FunctionCode", new SettingsControls.FunctionCode()},
            {"StartAddress", new SettingsControls.StartAddress() },
            {"FileNum", new SettingsControls.FileNum() },
            {"SwapDWord", new SettingsControls.SwapDWord() },
            {"StringLength", new SettingsControls.StringLength() },
            {"Broadcast", new SettingsControls.Broadcast() }
        };

        TextBlock TxBroadcast = null;
        CheckBox CheckBroadcast = null;
        TextBlock TxBStringLength = null;
        TextBox TxStringLength = null;
        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

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
                    thisTagSettings = new DriverTcpExampleDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                //Function Code
                if(DynamicSettingsDict.TryGetValue("FunctionCode", out row))
                {
                    var cfc = (ComboBox)row.FindName("CmbFunctionCode");
                    if (cfc != null)
                        cfc.ItemsSource = Enum.GetValues(typeof(FunctionCodes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Start Address
                if (DynamicSettingsDict.TryGetValue("StartAddress", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //FileNum
                if (DynamicSettingsDict.TryGetValue("FileNum", out row))
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
                //Swap DWord
                if (DynamicSettingsDict.TryGetValue("SwapDWord", out row))
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
                //Broadcast
                if (DynamicSettingsDict.TryGetValue("Broadcast", out row))
                {
                    TxBroadcast = (TextBlock)row.FindName("TxBroadcast");
                    CheckBroadcast = (CheckBox)row.FindName("CheckBroadcast");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //String Length
                if (DynamicSettingsDict.TryGetValue("StringLength", out row))
                {
                    TxBStringLength = (TextBlock)row.FindName("TxBStringLength");
                    TxStringLength = (TextBox)row.FindName("TxStringLength");
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
                    configuration = (from tag in new XPQuery<DriverTcpExampleDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new DriverTcpExampleDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }
                if(CmbStation != null)
                    CmbStation.SelectionChanged += CmbStation_SelectionChanged;

                if(CmbLinkType != null)
                    CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    if(TxBStringLength != null)
                        TxBStringLength.Visibility = Visibility.Collapsed;
                    if(TxStringLength != null)
                        TxStringLength.Visibility = Visibility.Collapsed;
                }

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                if(CmbLinkType != null)
                    descriptor.AddValueChanged(CmbLinkType, CmbLinkType_TextChanged);
                CmbLinkType_TextChanged();

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
                    var s = DataContext as DriverTcpExampleDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nAddressType = (int)AddressTypes.ZeroBased;
            if (e.AddedItems.Count == 0 && !(e.AddedItems[0] is DriverCodeBaseEx.StationSettings))
                return;
            string sn = (e.AddedItems[0] as DriverCodeBaseEx.StationSettings).Name;
            if (configuration != null && configuration.StationSettings.Count > 0)
            {
                var at = (from s in configuration.StationSettings.AsParallel()
                 where s.Name == sn 
                 orderby s.Name
                 select s).ToList();
                if(at.Count > 0 && at[0] is DriverTcpExampleStationSettings)
                {
                    nAddressType = (at[0] as DriverTcpExampleStationSettings).AddressType;
                }
            }
            var ds = DataContext as DriverTcpExampleDynTagSettings;
            if (ds != null)
                ds.AddressType = nAddressType;
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                if ((LinkType)cb.SelectedIndex == LinkType.Input)
                {
                    if(TxBroadcast != null)
                        TxBroadcast.Visibility = Visibility.Collapsed;
                    if(CheckBroadcast != null)
                        CheckBroadcast.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (TxBroadcast != null)
                        TxBroadcast.Visibility = Visibility.Visible;
                    if (CheckBroadcast != null)
                        CheckBroadcast.Visibility = Visibility.Visible;
                }
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
            DynamicSettingsDict.Clear();

            if (CmbStation != null)
                CmbStation.SelectionChanged -= CmbStation_SelectionChanged;

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
