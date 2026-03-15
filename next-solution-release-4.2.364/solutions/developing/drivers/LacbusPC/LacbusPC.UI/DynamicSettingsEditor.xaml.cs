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
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using LacbusPC;
using Utilities.WPF;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace LacbusPC.UI
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
        IDynamicSettingsEditing thisTag;
        LacbusPCDynTagSettings thisTagSettings;
        LacbusPCDriverSettings configuration;
        DriverCodeBase.UI.Controls.BaseDynamicSettings baseDyn = null;
        Dictionary<string, byte> underlyingProtocolDictionary;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
                bool LoadbaseDyn = false;

                baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumberErr.Visibility = Visibility.Collapsed;

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new LacbusPCDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                    MainStack.Children.Insert(0,baseDyn);

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    string DriverName = DriverInfo.GetDriverName(this);
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBase.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<LacbusPCDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new LacbusPCDriverSettings(ufw);
                    configuration.DefaultSettings();
                }

                underlyingProtocolDictionary = new Dictionary<string, byte>();

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    var stationssettings = (from s in configuration.StationSettings.AsParallel()
                                            orderby s.Name
                                            select s).ToList();
                    baseDyn.CmbStation.ItemsSource = stationssettings;
                    foreach (var settings in stationssettings)
                    {
                        LacbusPCStationSettings lacbuspcSettings = (LacbusPCStationSettings)settings;
                        underlyingProtocolDictionary.Add(settings.Name, (byte)lacbuspcSettings.LacbusPCProtocolType);
                    }
                }

                CmbDatumType.ItemsSource = Enum.GetValues(typeof(DatumTypes));
                CmbDatumCategory.ItemsSource = Enum.GetValues(typeof(DatumCategories));

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbDatumType, CmbDatumType_TextChanged);
                CmbDatumType_TextChanged(CmbDatumType);
                descriptor.AddValueChanged(baseDyn.CmbLinkType, CmbLinkType_TextChanged);
                CmbLinkType_TextChanged(baseDyn.CmbLinkType);
                descriptor.AddValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
                CmbStation_TextChanged(baseDyn.CmbStation);
                descriptor.AddValueChanged(CmbDatumCategory, CmbDatumCategory_TextChanged);
                CmbDatumCategory_TextChanged(CmbDatumCategory);

                if (thisTag.IsMethod && !thisTagSettings.IsMethodSupported)
                {
                    MainStack.Children.Clear();
                    MainStack.HorizontalAlignment = HorizontalAlignment.Center;
                    MainStack.VerticalAlignment = VerticalAlignment.Center;

                    var NoMethod = new DriverCodeBase.UI.Controls.BaseDynamicSettingsNoMethod();
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
                    var s = DataContext as LacbusPCDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private byte GetUnderlyingProtocols()
        {
            byte underlyingProtocol = (byte)LacbusPcUnderlyingProtocols.LacbusPC;
            string stationName = baseDyn.CmbStation.Text.ToString();
            if (!string.IsNullOrWhiteSpace(stationName))
            {
                underlyingProtocol = underlyingProtocolDictionary[stationName];
            }
            return (underlyingProtocol);
        }

        private void CmbDatumCategory_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                byte underlyingProtocol = GetUnderlyingProtocols();
                Visibility varVisibility = Visibility.Collapsed;
                if (underlyingProtocol == (byte)LacbusPcUnderlyingProtocols.SofbusPL)
                {
                    if (((CmbDatumType.SelectedIndex == (int)DatumTypes.AnalogInput) ||
                        (CmbDatumType.SelectedIndex == (int)DatumTypes.AnalogOutput) ||
                        (CmbDatumType.SelectedIndex == (int)DatumTypes.CountInput)) ||
                        ((CmbDatumType.SelectedIndex == (int)DatumTypes.DigitalInput)&&
                        ((DatumCategories)cb.SelectedIndex == DatumCategories.ReportDIEventCount)||
                        ((DatumCategories)cb.SelectedIndex == DatumCategories.ReportDIActiveStateTimeCount)))
                    {
                        varVisibility = Visibility.Visible;
                    }
                }

                ConvMinRawValueText.Visibility = varVisibility;
                ConvMinRawValue.Visibility = varVisibility;
                ConvMaxRawValueText.Visibility = varVisibility;
                ConvMaxRawValue.Visibility = varVisibility;
                ConvMinValueText.Visibility = varVisibility;
                ConvMinValue.Visibility = varVisibility;
                ConvMaxValueText.Visibility = varVisibility;
                ConvMaxValue.Visibility = varVisibility;
            }
        }

        private void CmbDatumType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                byte underlyingProtocol = GetUnderlyingProtocols();

                switch ((DatumTypes)cb.SelectedIndex)
                {
                    case DatumTypes.SetDateTime:
                    case DatumTypes.ShutdownFR1000FrontEnd:
                        DatumNumber.Visibility = Visibility.Collapsed;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Collapsed;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Collapsed;
                        DatumCategoryText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    case DatumTypes.Alarm:
                        DatumNumber.Visibility = Visibility.Collapsed;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Collapsed;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    case DatumTypes.RTUPollRequest:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Visible;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Visible;
                        CmbDatumCategory.Visibility = Visibility.Collapsed;
                        DatumCategoryText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    case DatumTypes.ModbusCoilSetpoints:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Collapsed;
                        DatumCategoryText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    case DatumTypes.ModbusRegisterSetpoints:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Collapsed;
                        DatumCategoryText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    case DatumTypes.DigitalInput:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Visible;
                        DatumCategoryText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
                        if((CmbDatumCategory.SelectedIndex == (int)DatumCategories.ReportDIActiveStateTimeCount) ||
                           (CmbDatumCategory.SelectedIndex == (int)DatumCategories.ReportDIEventCount) &&
                           (underlyingProtocol == (byte)LacbusPcUnderlyingProtocols.SofbusPL))
                        {
                            ConvMinRawValueText.Visibility = Visibility.Visible;
                            ConvMinRawValue.Visibility = Visibility.Visible;
                            ConvMaxRawValueText.Visibility = Visibility.Visible;
                            ConvMaxRawValue.Visibility = Visibility.Visible;
                            ConvMinValueText.Visibility = Visibility.Visible;
                            ConvMinValue.Visibility = Visibility.Visible;
                            ConvMaxValueText.Visibility = Visibility.Visible;
                            ConvMaxValue.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ConvMinRawValueText.Visibility = Visibility.Collapsed;
                            ConvMinRawValue.Visibility = Visibility.Collapsed;
                            ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                            ConvMaxRawValue.Visibility = Visibility.Collapsed;
                            ConvMinValueText.Visibility = Visibility.Collapsed;
                            ConvMinValue.Visibility = Visibility.Collapsed;
                            ConvMaxValueText.Visibility = Visibility.Collapsed;
                            ConvMaxValue.Visibility = Visibility.Collapsed;
                        }
                        break;
                    case DatumTypes.DigitalOutput:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Visible;
                        DatumCategoryText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
                        ConvMinRawValueText.Visibility = Visibility.Collapsed;
                        ConvMinRawValue.Visibility = Visibility.Collapsed;
                        ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                        ConvMaxRawValue.Visibility = Visibility.Collapsed;
                        ConvMinValueText.Visibility = Visibility.Collapsed;
                        ConvMinValue.Visibility = Visibility.Collapsed;
                        ConvMaxValueText.Visibility = Visibility.Collapsed;
                        ConvMaxValue.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        DatumNumber.Visibility = Visibility.Visible;
                        CommunicationDuration.Visibility = Visibility.Collapsed;
                        DatumNumberText.Visibility = Visibility.Visible;
                        CommunicationDurationText.Visibility = Visibility.Collapsed;
                        CmbDatumCategory.Visibility = Visibility.Visible;
                        DatumCategoryText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
                        if (underlyingProtocol == (byte)LacbusPcUnderlyingProtocols.SofbusPL)
                        {
                            ConvMinRawValueText.Visibility = Visibility.Visible;
                            ConvMinRawValue.Visibility = Visibility.Visible;
                            ConvMaxRawValueText.Visibility = Visibility.Visible;
                            ConvMaxRawValue.Visibility = Visibility.Visible;
                            ConvMinValueText.Visibility = Visibility.Visible;
                            ConvMinValue.Visibility = Visibility.Visible;
                            ConvMaxValueText.Visibility = Visibility.Visible;
                            ConvMaxValue.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ConvMinRawValueText.Visibility = Visibility.Collapsed;
                            ConvMinRawValue.Visibility = Visibility.Collapsed;
                            ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                            ConvMaxRawValue.Visibility = Visibility.Collapsed;
                            ConvMinValueText.Visibility = Visibility.Collapsed;
                            ConvMinValue.Visibility = Visibility.Collapsed;
                            ConvMaxValueText.Visibility = Visibility.Collapsed;
                            ConvMaxValue.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
            }
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                if ((LinkType)cb.SelectedIndex == LinkType.Input)
                {
                    baseDyn.OutputAtStartup.Visibility = Visibility.Collapsed;
                    baseDyn.OutputAtStartupText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    baseDyn.OutputAtStartup.Visibility = Visibility.Visible;
                    baseDyn.OutputAtStartupText.Visibility = Visibility.Visible;
                }
            }
        }

        private void CmbStation_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                string stationName = cb.Text;
                if(!String.IsNullOrWhiteSpace(stationName))
                {
                    if (underlyingProtocolDictionary.Keys.Contains(stationName))
                    {
                        byte underlyingProtocol = underlyingProtocolDictionary[stationName];
                        if(underlyingProtocol == (byte)LacbusPcUnderlyingProtocols.SofbusPL)
                        {
                            if((CmbDatumType.SelectedIndex == (int)DatumTypes.AnalogInput) ||
                               (CmbDatumType.SelectedIndex == (int)DatumTypes.AnalogOutput) ||
                               (CmbDatumType.SelectedIndex == (int)DatumTypes.CountInput) ||
                               ((CmbDatumType.SelectedIndex == (int)DatumTypes.DigitalInput) &&
                                ((CmbDatumCategory.SelectedIndex == (int)DatumCategories.ReportDIEventCount) ||
                                 (CmbDatumCategory.SelectedIndex == (int)DatumCategories.ReportDIActiveStateTimeCount))))
                            {
                                ConvMinRawValueText.Visibility = Visibility.Visible;
                                ConvMinRawValue.Visibility = Visibility.Visible;
                                ConvMaxRawValueText.Visibility = Visibility.Visible;
                                ConvMaxRawValue.Visibility = Visibility.Visible;
                                ConvMinValueText.Visibility = Visibility.Visible;
                                ConvMinValue.Visibility = Visibility.Visible;
                                ConvMaxValueText.Visibility = Visibility.Visible;
                                ConvMaxValue.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ConvMinRawValueText.Visibility = Visibility.Collapsed;
                                ConvMinRawValue.Visibility = Visibility.Collapsed;
                                ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                                ConvMaxRawValue.Visibility = Visibility.Collapsed;
                                ConvMinValueText.Visibility = Visibility.Collapsed;
                                ConvMinValue.Visibility = Visibility.Collapsed;
                                ConvMaxValueText.Visibility = Visibility.Collapsed;
                                ConvMaxValue.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            ConvMinRawValueText.Visibility = Visibility.Collapsed;
                            ConvMinRawValue.Visibility = Visibility.Collapsed;
                            ConvMaxRawValueText.Visibility = Visibility.Collapsed;
                            ConvMaxRawValue.Visibility = Visibility.Collapsed;
                            ConvMinValueText.Visibility = Visibility.Collapsed;
                            ConvMinValue.Visibility = Visibility.Collapsed;
                            ConvMaxValueText.Visibility = Visibility.Collapsed;
                            ConvMaxValue.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        #region Properties

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
