using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RMS621;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace RMS621.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl
    {
        private class ProcessValue
        {
            public string Description { get; set; }
            public RMS621Protocol.Process Value { get; set; }

            public ProcessValue(string description, RMS621Protocol.Process value)
            {
                Description = description;
                Value = value;
            }
        }

        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        RMS621DriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        RMS621DynTagSettings thisTagSettings;
        public DynamicSettingsEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                var baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
                bool LoadbaseDyn = false;

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    //thisTagSettings = new MelsecFXDynTagSettings() { IsMethod = thisTag.IsMethod };
                    thisTagSettings = new RMS621DynTagSettings();
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                    LoadbaseDyn = true;
                }
                if (!thisTag.IsMethod)
                {
                    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                    thisTagSettings.MethodID = -1;
                }

                DataContext = null;
                DataContext = thisTagSettings;
                baseDyn.DataContext = DataContext;
                if (LoadbaseDyn)
                    MainStack.Children.Add(baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<RMS621DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new RMS621DriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                CmbCommand.ItemsSource = new List<RMS621Protocol.Command>()
                {
                    RMS621Protocol.Command.Read,
                    RMS621Protocol.Command.ResetAllCounters,
                    RMS621Protocol.Command.ResetEventList
                };

                List<ProcessValue> ListProcess = new List<ProcessValue>() {
                    new ProcessValue("Heat Flow",RMS621Protocol.Process.HeatFlow),
                    new ProcessValue("Heat Sum", RMS621Protocol.Process.HeatSum),
                    new ProcessValue("Mass Flow", RMS621Protocol.Process.MassFlow),
                    new ProcessValue("Mass Sum", RMS621Protocol.Process.MassSum),
                    new ProcessValue("Flow Rate", RMS621Protocol.Process.FlowRate),
                    new ProcessValue("Pressure", RMS621Protocol.Process.Pressure),
                    new ProcessValue("Temperature", RMS621Protocol.Process.Temperature),
                    new ProcessValue("Temperature Difference", RMS621Protocol.Process.TemperatureDifference),
                    new ProcessValue("Flow Sum", RMS621Protocol.Process.FlowSum),
                    new ProcessValue("Density", RMS621Protocol.Process.Density),
                    new ProcessValue("Specific Enthalpy", RMS621Protocol.Process.SpecificEnthalpy),
                    new ProcessValue("Tot Heat Sum", RMS621Protocol.Process.TotHeatSum),
                    new ProcessValue("Tot Mass Sum", RMS621Protocol.Process.TotMassSum),
                    new ProcessValue("Tot Flow Rate", RMS621Protocol.Process.TotFlowRate),
                    new ProcessValue("Negative Heat Sum", RMS621Protocol.Process.NegativeHeatSum),
                    new ProcessValue("Negative Mass Sum", RMS621Protocol.Process.NegativeMassSum),
                    new ProcessValue("Tot Negative Heat Sum", RMS621Protocol.Process.TotNegativeHeatSum),
                    new ProcessValue("Tot Negative Mass Sum", RMS621Protocol.Process.TotNegativeMassSum)
                };
                CmbProcess.ItemsSource = ListProcess;
                CmbProcess.DisplayMemberPath = "Description";
                CmbProcess.SelectedValuePath = "Value";

                // hide unsupported standard property
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.OutputAtStartupText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.OutputAtStartup.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
                CmbCommand_SelectionChanged(null,null);
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
                    var s = DataContext as RMS621DynTagSettings;
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

        private void CmbCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((RMS621Protocol.Command)(CmbCommand.SelectedValue))
            {
                case RMS621Protocol.Command.Read:
                    lblProcess.Visibility = Visibility.Visible;
                    CmbProcess.Visibility = Visibility.Visible;
                    lblProcessNumber.Visibility = Visibility.Visible;
                    txtProcessNumber.Visibility = Visibility.Visible;
                    break;
                case RMS621Protocol.Command.ResetAllCounters:
                case RMS621Protocol.Command.ResetEventList:
                    lblProcess.Visibility = Visibility.Collapsed;
                    CmbProcess.Visibility = Visibility.Collapsed;
                    lblProcessNumber.Visibility = Visibility.Collapsed;
                    txtProcessNumber.Visibility = Visibility.Collapsed;
                    CmbProcess.SelectedIndex = 0;
                    txtProcessNumber.Text = "1";
                    break;
            }
        }
    }
}
