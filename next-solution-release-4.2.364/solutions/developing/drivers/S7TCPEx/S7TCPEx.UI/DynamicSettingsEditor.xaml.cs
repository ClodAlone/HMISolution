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

namespace S7TCP.UI
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
        S7TCPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        S7TCPDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"StartAddress", new SettingsControls.StartAddress()},
            {"LenStringEnable", new SettingsControls.LenStringEnable() },
            {"SwapDWords", new SettingsControls.SwapDWords() },
            {"StructStringLengths", new SettingsControls.StructStringLengths() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        TextBlock TxBLenStringEnable = null;
        CheckBox CheckLenStringEnable = null;
        DockPanel DKPStructStringLength = null;
        TextBlock TxStructStringLength = null;
        TextBox TbStructStringLength = null;

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
                    thisTagSettings = new S7TCPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                // StartAddress
                if (DynamicSettingsDict.TryGetValue("StartAddress", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // LenStringEnable
                if (DynamicSettingsDict.TryGetValue("LenStringEnable", out row))
                {
                    TxBLenStringEnable = (TextBlock)row.FindName("TxBLenStringEnable");
                    CheckLenStringEnable = (CheckBox)row.FindName("CheckLenStringEnable");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // StructStringLengths
                if (DynamicSettingsDict.TryGetValue("StructStringLengths", out row))
                {
                    DKPStructStringLength = (DockPanel)row.FindName("DKPStructStringLength");
                    TxStructStringLength = (TextBlock)row.FindName("TxStructStringLength");
                    TbStructStringLength = (TextBox)row.FindName("TbStructStringLength");
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
                //Swap DWords
                if (DynamicSettingsDict.TryGetValue("SwapDWords", out row))
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
                //Conditional Variable*
                if (baseSettingsCtrls.TryGetValue("JobConditionalVariable", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Offset Variable*
                if (baseSettingsCtrls.TryGetValue("JobOffsetVariable", out row))
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

                if(idl == null)
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
                    configuration = (from tag in new XPQuery<S7TCPDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new S7TCPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                if (CmbLinkType != null)
                    CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    if((TxBLenStringEnable != null) && (CheckLenStringEnable != null))
                    {
                        TxBLenStringEnable.Visibility = Visibility.Collapsed;
                        CheckLenStringEnable.Visibility = Visibility.Collapsed;
                    }
                }

                // If the variable is a structure, shows the controls for setting the lengths of the elements of type string
                // and for managing the structure in an atomic way.
                if (thisTag.IsObjectType)
                {
                    if((DKPStructStringLength != null) && (TxStructStringLength != null) && (TbStructStringLength != null))
                    {
                        DKPStructStringLength.Visibility = Visibility.Visible;
                        TxStructStringLength.Visibility = Visibility.Visible;
                        TbStructStringLength.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if ((DKPStructStringLength != null) && (TxStructStringLength != null) && (TbStructStringLength != null))
                    {
                        DKPStructStringLength.Visibility = Visibility.Collapsed;
                        TxStructStringLength.Visibility = Visibility.Collapsed;
                        TbStructStringLength.Visibility = Visibility.Collapsed;
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
                    var s = DataContext as S7TCPDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToStringSA();
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
