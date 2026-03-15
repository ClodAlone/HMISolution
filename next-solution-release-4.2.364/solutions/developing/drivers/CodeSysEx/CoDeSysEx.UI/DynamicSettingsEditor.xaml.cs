using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace CoDeSys.UI
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
        CoDeSysDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        CoDeSysDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"VariableName", new SettingsControls.VariableName()},            
        };
        
        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;

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

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new CoDeSysDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
                
                //VariableName
                if (DynamicSettingsDict.TryGetValue("VariableName", out row))
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

                //baseDyn.DataContext = DataContext;
                //if (LoadbaseDyn)
                //    MainStack.Children.Add(baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<CoDeSysDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new CoDeSysDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, CoDeSysStationSettings>();
                    foreach (CoDeSysStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                if (CmbLinkType != null)
                    CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
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
                    var s = DataContext as CoDeSysDynTagSettings;
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
