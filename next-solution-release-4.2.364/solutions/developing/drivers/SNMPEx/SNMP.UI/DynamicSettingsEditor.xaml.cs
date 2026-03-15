using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBaseEx.Enumerators;
using SNMP;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace SNMP.UI
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
        SNMPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        SNMPDynTagSettings thisTagSettings;

        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"DataType", new SettingsControls.SnmpDataType() },
            {"DataSize", new SettingsControls.SnmpDataSize() },
            {"OID", new SettingsControls.SnmpOID() },
            {"Community", new SettingsControls.SnmpCommunity() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbSNMPDataType = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                baseSettingsCtrls = new DriverCodeBaseEx.UI.BaseSettings().GetBaseDynamicSettings(); ;

                //baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.edtElementNumberErr.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new SNMPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }

                //if (!thisTag.IsMethod)
                //{
                //    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    thisTagSettings.MethodID = -1;
                //}

                DataContext = null;
                DataContext = thisTagSettings;
                UserControl row;

                //DataType
                if (DynamicSettingsDict.TryGetValue("DataType", out row))
                {
                    var cfc = (ComboBox)row.FindName("CmbSNMPDataType");
                    if (cfc != null)
                        cfc.ItemsSource = Enum.GetValues(typeof(SNMPDATATYPE));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //DataSize
                if (DynamicSettingsDict.TryGetValue("DataSize", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //OID
                if (DynamicSettingsDict.TryGetValue("OID", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Community
                if (DynamicSettingsDict.TryGetValue("Community", out row))
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
                //if (DynamicSettingsDict.TryGetValue("SwapDWord", out row))
                //{
                //    row.DataContext = DataContext;
                //    MainStack.Children.Add(row);
                //}
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Element Number*
                //if (baseSettingsCtrls.TryGetValue("ElementNumber", out row))
                //{
                //    row.DataContext = DataContext;
                //    MainStack.Children.Add(row);
                //}
                //Conditional Variable*
                if (baseSettingsCtrls.TryGetValue("JobConditionalVariable", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }


                //baseDyn.DataContext = DataContext;
                //if (LoadbaseDyn)
                //    MainStack.Children.Insert(0,baseDyn);

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

                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<SNMPDriverSettings>(ufw).AsParallel() select tag).Single();

                    
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new SNMPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //Dictionary<int, string> n = new Dictionary<int, string>();
                //n.Add((int)LinkType.Input, DriverCodeBase.Properties.Resources.LinkTypeInput);
                //n.Add((int)LinkType.InputOutput, DriverCodeBase.Properties.Resources.LinkTypeIO);
                //n.Add((int)LinkType.ExceptionOutput, DriverCodeBase.Properties.Resources.LinkTypeExcOut);
                //n.Add((int)LinkType.UnconditionalOutput, DriverCodeBase.Properties.Resources.LinkTypeUncOut);
                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                //CmbSNMPDataType.ItemsSource = Enum.GetValues(typeof(SNMPDATATYPE));

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
                    var s = DataContext as SNMPDynTagSettings;
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
