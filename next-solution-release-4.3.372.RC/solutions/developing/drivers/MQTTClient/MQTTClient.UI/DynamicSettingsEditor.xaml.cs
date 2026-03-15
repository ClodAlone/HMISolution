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
using System.ComponentModel;
using MQTTClient;
using DriverCodeBase;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using Utilities.WPF;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace MQTTClient.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        private class TimeFormat
        {
            public JSonTimestampFormats Code { get; set; }
            public string Description { get; set; }

            public TimeFormat(JSonTimestampFormats code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        MQTTClientDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        MQTTClientDynTagSettings thisTagSettings;
        DriverCodeBase.UI.Controls.BaseDynamicSettings baseDyn;

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
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new MQTTClientDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                    configuration = (from tag in new XPQuery<MQTTClientDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new MQTTClientDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, MQTTClientStationSettings>();
                    foreach (MQTTClientStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                //swap checkboxes have been hidden, because the driver handles strings.
                baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;

                baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                CmbQOS.ItemsSource = Enum.GetValues(typeof(QualityOfServiceLevels));

                // Fill the combo of the time formats for the timestamp of the JSon message
                List<TimeFormat> timeFormatList = new List<TimeFormat>();
                timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_ISO, "ISO: YYYY-MM-DDTHH:MM:SS.mmm"));
                timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_en_US, "MM-DD-YYYYTHH:MM:SS.mmm"));
                timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_it_IT, "DD-MM-YYYYTHH:MM:SS.mmm"));
                timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_EPOCH, "EPOCH (UNIX)"));
                CmbJsonTimestampFormat.ItemsSource = timeFormatList;

                DependencyPropertyDescriptor descriptor =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(baseDyn.CmbStation, CmbStation_TextChanged);
                CmbStation_TextChanged();

                DependencyPropertyDescriptor descriptor2 =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor2.AddValueChanged(CmbJsonTimestampFormat, CmbJsonTimestampFormat_TextChanged);
                CmbJsonTimestampFormat_TextChanged();

                DependencyPropertyDescriptor descriptor3 =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor3.AddValueChanged(baseDyn.CmbLinkType, CmbLinkType_TextChanged);
                CmbLinkType_TextChanged();

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
                    var s = DataContext as MQTTClientDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            LinkType jobType = LinkType.InputOutput;
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                if (cb.SelectedItem != null)
                {
                    jobType = (LinkType)cb.SelectedItem;
                }
            }
            if(jobType != LinkType.ExceptionOutput)
            {
                txtHysteresis.Visibility = Visibility.Collapsed;
                boxHysteresis.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(thisTagSettings.IsObjectType || (thisTagSettings.VarType == UFUAModel.DataType.Boolean) || (thisTagSettings.VarType == UFUAModel.DataType.String) || (thisTagSettings.ArrayDimension > 0))
                {
                    txtHysteresis.Visibility = Visibility.Collapsed;
                    boxHysteresis.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtHysteresis.Visibility = Visibility.Visible;
                    boxHysteresis.Visibility = Visibility.Visible;
                }
            }
        }

        private void CmbJsonTimestampFormat_TextChanged(object sender = null, EventArgs e = null)
        {
            CmbJsonTimestampFormat_TextChanged();
        }

        private void CmbJsonTimestampFormat_TextChanged()
        {
            if(CmbJsonTimestampFormat.Visibility == Visibility.Collapsed)
            {
                return;
            }
            if (CmbJsonTimestampFormat.SelectedItem == null)
            {
                return;
            }

            TimeFormat timeFormat = (TimeFormat)CmbJsonTimestampFormat.SelectedItem;
            JSonTimestampFormats timestampFormat = timeFormat.Code;

            if (timestampFormat == JSonTimestampFormats.tf_EPOCH)
            {
                textBlockUseLocalTime.Visibility = Visibility.Collapsed;
                checkUseLocalTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlockUseLocalTime.Visibility = Visibility.Visible;
                checkUseLocalTime.Visibility = Visibility.Visible;
            }
        }


        private void CmbStation_TextChanged(object sender = null, EventArgs e = null)
        {
            MQTTClientMessageFormats stationMessageFormat = MQTTClientMessageFormats.XML;
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                string stationName = String.Empty;
                if (cb.SelectedItem != null)
                {
                    MQTTClientStationSettings stationSettings = (MQTTClientStationSettings)cb.SelectedItem;
                    stationMessageFormat = stationSettings.MQTTClientMessageFormat;
                }
            }

            if (stationMessageFormat == MQTTClientMessageFormats.JSON)
            {
                if (baseDyn != null)
                {
                    baseDyn.TXBStateCommandVariable.Visibility = Visibility.Visible;
                    baseDyn.DKPStateCommandVariable.Visibility = Visibility.Visible;
                    baseDyn.CmbLinkTypeText.Visibility = Visibility.Visible;
                    baseDyn.CmbLinkType.Visibility = Visibility.Visible;
                    baseDyn.OutputAtStartupText.Visibility = Visibility.Visible;
                    baseDyn.OutputAtStartup.Visibility = Visibility.Visible;
                }

                txtJsonMessageFormat.Visibility = Visibility.Visible;
                boxJsonMessageFormat.Visibility = Visibility.Visible;
                txtJsonMessageTimestampField.Visibility = Visibility.Visible;
                boxJsonMessageTimestampField.Visibility = Visibility.Visible;
                TxtJsonTimestampFormat.Visibility = Visibility.Visible;
                CmbJsonTimestampFormat.Visibility = Visibility.Visible;
                textBlockUseLocalTime.Visibility = Visibility.Visible;
                checkUseLocalTime.Visibility = Visibility.Visible;
                CmbJsonTimestampFormat_TextChanged();
                //if (!thisTagSettings.IsObjectType)
                //{
                //    txtJsonMessageFormat.Visibility = Visibility.Visible;
                //    boxJsonMessageFormat.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    txtJsonMessageFormat.Visibility = Visibility.Collapsed;
                //    boxJsonMessageFormat.Visibility = Visibility.Collapsed;
                //}
            }
            else
            {
                if (baseDyn != null)
                {
                    baseDyn.TXBStateCommandVariable.Visibility = Visibility.Visible;
                    baseDyn.DKPStateCommandVariable.Visibility = Visibility.Visible;
                    baseDyn.CmbLinkTypeText.Visibility = Visibility.Visible;
                    baseDyn.CmbLinkType.Visibility = Visibility.Visible;
                    baseDyn.OutputAtStartupText.Visibility = Visibility.Visible;
                    baseDyn.OutputAtStartup.Visibility = Visibility.Visible;
                }

                txtJsonMessageFormat.Visibility = Visibility.Collapsed;
                boxJsonMessageFormat.Visibility = Visibility.Collapsed;
                txtJsonMessageTimestampField.Visibility = Visibility.Collapsed;
                boxJsonMessageTimestampField.Visibility = Visibility.Collapsed;
                TxtJsonTimestampFormat.Visibility = Visibility.Collapsed;
                CmbJsonTimestampFormat.Visibility = Visibility.Collapsed;
                textBlockUseLocalTime.Visibility = Visibility.Collapsed;
                checkUseLocalTime.Visibility = Visibility.Collapsed;
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
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
