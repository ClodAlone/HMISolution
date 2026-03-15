using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using DevExpress.Mvvm.Native;

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
            {"OID", new SettingsControls.SnmpOID() },
            {"Community", new SettingsControls.SnmpCommunity() },
            {"TrapOnly", new SettingsControls.SnmpTrapOnly() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbSNMPDataType = null;
        UserControl rowTrapOny;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                SNMPVERSION snmpVersion = SNMPVERSION.SNMPv1;

                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                baseSettingsCtrls = new DriverCodeBaseEx.UI.BaseSettings().GetBaseDynamicSettings(); ;

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
                }

                DataContext = null;
                DataContext = thisTagSettings;
                UserControl row;

                //OID
                if (DynamicSettingsDict.TryGetValue("OID", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //DataType
                if (DynamicSettingsDict.TryGetValue("DataType", out row))
                {
                    CmbSNMPDataType = (ComboBox)row.FindName("CmbSNMPDataType");
                    if (CmbSNMPDataType != null)
                    {
                        //CmbSNMPDataType.ItemsSource = Enum.GetValues(typeof(SNMPDATATYPE));

                        CmbSNMPDataType.ItemsSource = new List<SNMPDATATYPE>() { SNMPDATATYPE.Integer, SNMPDATATYPE.Integer32, SNMPDATATYPE.OctetString, SNMPDATATYPE.Counter32, SNMPDATATYPE.Unsigned32, SNMPDATATYPE.Gauge32, SNMPDATATYPE.TimeTicks, SNMPDATATYPE.IpAddress };
                    }
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Community
                if (DynamicSettingsDict.TryGetValue("Community", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Trap only
                if (DynamicSettingsDict.TryGetValue("TrapOnly", out rowTrapOny))
                {
                    rowTrapOny.DataContext = DataContext;
                    MainStack.Children.Add(rowTrapOny);
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

                    var thisS = (from s in configuration.StationSettings.AsParallel()
                                             where s.Name == thisTagSettings.StationName
                                             select s).ToList();

                    if (thisS != null && thisS.Count == 1)
                    {
                        var thisC = (from s in configuration.ChannelSettings.AsParallel()
                                     where s.Name == thisS[0].Channel
                                     select s).ToList();

                        if (thisC != null && thisC.Count == 1)
                            snmpVersion = ((SNMPChannelSettings)thisC[0]).snmpVersion;
                    }
                }

                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (snmpVersion == SNMPVERSION.SNMPv1)
                    rowTrapOny.Visibility = Visibility.Collapsed;

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
