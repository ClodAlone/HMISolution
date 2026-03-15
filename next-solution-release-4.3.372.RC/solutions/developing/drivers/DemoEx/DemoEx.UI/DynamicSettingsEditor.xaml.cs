using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Demo.UI.SettingsControls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace Demo.UI
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
        DemoDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        DemoDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>()
        {
            {"DemoType", new SettingsControls.DemoType()},
            {"MinValue", new SettingsControls.MinValue()},
            {"MaxValue", new SettingsControls.MaxValue()},
            {"SimulationInterval", new SettingsControls.SimulationInterval()},
            {"DeltaValue", new SettingsControls.DeltaValue()},
            {"NrCycles", new SettingsControls.NrCycles()},
            {"FactorSinCos", new SettingsControls.FactorSinCos()}
        };

        ComboBox cmbStation = null;
        ComboBox cmbLinkType = null;
        ComboBox cmbDemoType = null;
        UserControl rowFactorSinCos = null;
        TextBlock deltaValueTB = null;
        UserControl rowMinValue = null;
        UserControl rowMaxValue = null;
        UserControl rowDeltaValue = null;
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

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new DemoDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
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
                if (thisTag.IsMethod)
                {                   
                    //Method*
                    if (baseSettingsCtrls.TryGetValue("Method", out row))
                    {
                        var cm = (ComboBox)row.FindName("CmbMethod");
                        if (cm != null)
                            cm.ItemsSource = Enum.GetValues(typeof(DemoDriver.DemoMethods));
                        row.DataContext = DataContext;
                        MainStack.Children.Add(row);                        
                    }                    
                }
                else
                {                                     
                    //DemoType
                    if (DynamicSettingsDict.TryGetValue("DemoType", out row))
                    {
                        cmbDemoType = (ComboBox)row.FindName("CmbDemoType");
                        if (cmbDemoType != null)
                        {
                            cmbDemoType.ItemsSource = new Dictionary<int, string>()
                            {
                                { (int)(DemoProtocol.DemoTypes.Sin), Properties.Resources.DemoTypeSin },
                                { (int)(DemoProtocol.DemoTypes.Cos), Properties.Resources.DemoTypeCos },
                                { (int)(DemoProtocol.DemoTypes.Ramp), Properties.Resources.DemoTypeRamp },
                                { (int)(DemoProtocol.DemoTypes.Random), Properties.Resources.DemoTypeRandom  },
                                { (int)(DemoProtocol.DemoTypes.SquareWave), Properties.Resources.DemoTypeSquareWave },
                                { (int)(DemoProtocol.DemoTypes.UpDownCounter), Properties.Resources.DemoTypeUpDownCounter }
                            };
                        }
                        row.DataContext = DataContext;
                        MainStack.Children.Add(row);
                    }
                    //FactorSinCos*
                    if (DynamicSettingsDict.TryGetValue("FactorSinCos", out rowFactorSinCos))
                    {
                        rowFactorSinCos.DataContext = DataContext;
                        MainStack.Children.Add(rowFactorSinCos);
                    }
                    //MinValue*
                    if (DynamicSettingsDict.TryGetValue("MinValue", out rowMinValue))
                    {
                        rowMinValue.DataContext = DataContext;
                        MainStack.Children.Add(rowMinValue);
                    }
                    //MaxValue*
                    if (DynamicSettingsDict.TryGetValue("MaxValue", out rowMaxValue))
                    {
                        rowMaxValue.DataContext = DataContext;
                        MainStack.Children.Add(rowMaxValue);
                    }
                    //SimulationInterval*
                    if (DynamicSettingsDict.TryGetValue("SimulationInterval", out row))
                    {
                        row.DataContext = DataContext;
                        MainStack.Children.Add(row);
                    }
                    //DeltaValue*
                    if (DynamicSettingsDict.TryGetValue("DeltaValue", out rowDeltaValue))
                    {
                        deltaValueTB = (TextBlock)rowDeltaValue.FindName("DeltaValueTB");
                        rowDeltaValue.DataContext = DataContext;
                        MainStack.Children.Add(rowDeltaValue);
                    }
                    if (DynamicSettingsDict.TryGetValue("NrCycles", out row))
                    {
                        row.DataContext = DataContext;
                        MainStack.Children.Add(row);
                    }                    
                    //Link Type*
                    if (baseSettingsCtrls.TryGetValue("LinkType", out row))
                    {
                        row.DataContext = DataContext;
                        cmbLinkType = (ComboBox)row.FindName("CmbLinkType");
                        MainStack.Children.Add(row);
                    }                   
                    if (cmbLinkType != null)
                    {
                        cmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                    }

                    if (cmbDemoType != null)
                    {
                        descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                        descriptor.AddValueChanged(cmbDemoType, CmbDemoType_SelectionChanged);
                        CmbDemoType_SelectionChanged();
                    }                
                }

                //Station*
                if (baseSettingsCtrls.TryGetValue("Station", out row))
                {
                    row.DataContext = DataContext;
                    cmbStation = (ComboBox)row.FindName("CmbStation");
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
                    return;
                }

                ufw = new UnitOfWork(idl);
                try
                {
                    //driver specific class
                    configuration = (from tag in new XPQuery<DemoDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new DemoDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    cmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
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
                    var s = DataContext as DemoDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbDemoType_SelectionChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                switch ((DemoProtocol.DemoTypes)cb.SelectedValue)
                {
                    case DemoProtocol.DemoTypes.Sin:
                        rowFactorSinCos.Visibility = Visibility.Visible;
                        rowMinValue.Visibility = Visibility.Collapsed;
                        deltaValueTB.Text = Properties.Resources.CaptionNrStepsForCycle;
                        rowMaxValue.Visibility = Visibility.Collapsed;
                        rowDeltaValue.Visibility = Visibility.Visible;
                        break;
                    case DemoProtocol.DemoTypes.Cos:
                        rowFactorSinCos.Visibility = Visibility.Visible;
                        rowMinValue.Visibility = Visibility.Collapsed;
                        deltaValueTB.Text = Properties.Resources.CaptionNrStepsForCycle;
                        rowMaxValue.Visibility = Visibility.Collapsed;
                        rowDeltaValue.Visibility = Visibility.Visible;
                        break;
                    case DemoProtocol.DemoTypes.Ramp:
                        rowFactorSinCos.Visibility = Visibility.Collapsed;
                        rowMinValue.Visibility = Visibility.Visible;
                        deltaValueTB.Text = Properties.Resources.CaptionDeltaValue;
                        rowMaxValue.Visibility = Visibility.Visible;
                        rowDeltaValue.Visibility = Visibility.Visible;
                        break;
                    case DemoProtocol.DemoTypes.Random:
                        rowFactorSinCos.Visibility = Visibility.Collapsed;
                        rowMinValue.Visibility = Visibility.Visible;
                        deltaValueTB.Text = Properties.Resources.CaptionDeltaValue;
                        rowMaxValue.Visibility = Visibility.Visible;
                        rowDeltaValue.Visibility = Visibility.Collapsed;
                        break;
                    case DemoProtocol.DemoTypes.SquareWave:
                        rowFactorSinCos.Visibility = Visibility.Collapsed;
                        rowMinValue.Visibility = Visibility.Visible;
                        deltaValueTB.Text = Properties.Resources.CaptionDeltaValue;
                        rowMaxValue.Visibility = Visibility.Visible;
                        rowDeltaValue.Visibility = Visibility.Collapsed;
                        break;
                    case DemoProtocol.DemoTypes.UpDownCounter:
                        rowFactorSinCos.Visibility = Visibility.Collapsed;
                        rowMinValue.Visibility = Visibility.Visible;
                        deltaValueTB.Text = Properties.Resources.CaptionDeltaValue;
                        rowMaxValue.Visibility = Visibility.Visible;                        
                        rowDeltaValue.Visibility = Visibility.Visible;
                        break;
                }

                if (thisTagSettings.VarType == UFUAModel.DataType.Boolean)
                {
                    rowMinValue.Visibility = Visibility.Collapsed;
                    rowMaxValue.Visibility = Visibility.Collapsed;
                    rowDeltaValue.Visibility = Visibility.Collapsed;
                }
            }
        }

        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
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
