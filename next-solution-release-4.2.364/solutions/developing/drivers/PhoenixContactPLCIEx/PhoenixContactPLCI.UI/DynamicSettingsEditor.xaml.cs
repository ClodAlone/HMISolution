using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using System.Collections.Generic;

namespace PhoenixContactPLCI.UI
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
        PhoenixContactPLCIDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        PhoenixContactPLCIDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"AddressCnt", new SettingsControls.AddressCnt()},
            {"DataFormatCnt", new SettingsControls.DataFormatCnt()},
            {"StringLengthCnt", new SettingsControls.StringLengthCnt()}
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        TextBlock LbLStringLength = null;
        TextBox TxTStringLength = null;
        ComboBox CmbDataFormat = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                bLoaded = true;
                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new PhoenixContactPLCIDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                //Address
                if (DynamicSettingsDict.TryGetValue("AddressCnt", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //DataFormat
                if (DynamicSettingsDict.TryGetValue("DataFormatCnt", out row))
                {
                    CmbDataFormat = (ComboBox)row.FindName("CmbDataFormat");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //StringLengthCnt
                if (DynamicSettingsDict.TryGetValue("StringLengthCnt", out row))
                {
                    LbLStringLength = (TextBlock)row.FindName("LbLStringLength");
                    TxTStringLength = (TextBox)row.FindName("TxTStringLength");
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
                    configuration = (from tag in new XPQuery<PhoenixContactPLCIDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new PhoenixContactPLCIDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                if (CmbLinkType != null)
                    CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (CmbDataFormat != null)
                {
                    DependencyPropertyDescriptor descriptor =
                        DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                        descriptor.AddValueChanged(CmbDataFormat, CmbDataFormat_SelectedValueChanged);

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

                    if (thisTagSettings.DataFormat == PhoenixContactPLCIProtocol.VarType.UNKNOWN  || !Enum.IsDefined(typeof(PhoenixContactPLCIProtocol.VarType), thisTagSettings.DataFormat))
                        thisTagSettings.DataFormat = PhoenixContactPLCIProtocol.VarType.BOOL;
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
                    var s = DataContext as PhoenixContactPLCIDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbDataFormat_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((PhoenixContactPLCIProtocol.VarType)CmbDataFormat.SelectedValue) == PhoenixContactPLCIProtocol.VarType.STRING)            
            {
                LbLStringLength.Visibility = System.Windows.Visibility.Visible;
                TxTStringLength.Visibility = System.Windows.Visibility.Visible;
            }
            else
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
