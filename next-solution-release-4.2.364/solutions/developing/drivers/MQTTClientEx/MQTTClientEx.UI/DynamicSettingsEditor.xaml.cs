using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using MQTTClient;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Utilities.WPF;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using DevExpress.Xpf.Editors;

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
        Dictionary<string, UserControl> baseSettingsCtrls = null;
        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbJsonTimestampFormat = null;
        TextBlock txtHysteresis = null;
        SpinEdit boxHysteresis = null;
        TextBlock textBlockUseLocalTime = null;
        CheckBox checkUseLocalTime = null;
        TextBlock txtJsonMessageFormat = null;
        TextBox boxJsonMessageFormat = null;
        TextBlock txtJsonMessageTimestampField = null;
        TextBox boxJsonMessageTimestampField = null;
        TextBlock TxtJsonTimestampFormat = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"TopicName", new SettingsControls.TopicName()},
            {"Retained", new SettingsControls.Retained() },
            {"QualityOfService", new SettingsControls.QualityOfService() },
            {"Hysteresis", new SettingsControls.Hysteresis() },
            {"JsonMessageFormat", new SettingsControls.JsonMessageFormat() },
            {"JsonMessageTimestapField", new SettingsControls.JsonMessageTimestapField() },
            {"JsonTimestampFormat", new SettingsControls.JsonTimestampFormat() },
            {"UseLocalTime", new SettingsControls.UseLocalTime() }
        };

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
                    thisTagSettings = new MQTTClientDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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

                // Topic Name
                if (DynamicSettingsDict.TryGetValue("TopicName", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Retained
                if (DynamicSettingsDict.TryGetValue("Retained", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Quality Of Service
                if (DynamicSettingsDict.TryGetValue("QualityOfService", out row))
                {
                    var cQos = (ComboBox)row.FindName("CmbQOS");
                    if (cQos != null)
                        cQos.ItemsSource = Enum.GetValues(typeof(QualityOfServiceLevels));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Hysteresis
                if (DynamicSettingsDict.TryGetValue("Hysteresis", out row))
                {
                    txtHysteresis = (TextBlock)row.FindName("txtHysteresis");
                    boxHysteresis = (SpinEdit)row.FindName("boxHysteresis");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Json Message Format
                if (DynamicSettingsDict.TryGetValue("JsonMessageFormat", out row))
                {
                    txtJsonMessageFormat = (TextBlock)row.FindName("txtJsonMessageFormat");
                    boxJsonMessageFormat = (TextBox)row.FindName("boxJsonMessageFormat");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Json Message Timestap Field
                if (DynamicSettingsDict.TryGetValue("JsonMessageTimestapField", out row))
                {
                    txtJsonMessageTimestampField = (TextBlock)row.FindName("txtJsonMessageTimestampField");
                    boxJsonMessageTimestampField = (TextBox)row.FindName("boxJsonMessageTimestampField");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Json Timestamp Format
                if (DynamicSettingsDict.TryGetValue("JsonTimestampFormat", out row))
                {
                    // Fill the combo of the time formats for the timestamp of the JSon message
                    CmbJsonTimestampFormat = (ComboBox)row.FindName("CmbJsonTimestampFormat");
                    if (CmbJsonTimestampFormat != null)
                    {
                        List<TimeFormat> timeFormatList = new List<TimeFormat>();
                        timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_ISO, "ISO: YYYY-MM-DDTHH:MM:SS.mmm"));
                        timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_en_US, "MM-DD-YYYYTHH:MM:SS.mmm"));
                        timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_it_IT, "DD-MM-YYYYTHH:MM:SS.mmm"));
                        timeFormatList.Add(new TimeFormat(JSonTimestampFormats.tf_EPOCH, "EPOCH (UNIX)"));
                        CmbJsonTimestampFormat.ItemsSource = timeFormatList;
                    }
                    TxtJsonTimestampFormat = (TextBlock)row.FindName("TxtJsonTimestampFormat");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // UseLocalTime
                if (DynamicSettingsDict.TryGetValue("UseLocalTime", out row))
                {
                    textBlockUseLocalTime = (TextBlock)row.FindName("textBlockUseLocalTime");
                    checkUseLocalTime = (CheckBox)row.FindName("checkUseLocalTime");
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
                    configuration = (from tag in new XPQuery<MQTTClientDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new MQTTClientDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, MQTTClientStationSettings>();
                    foreach (MQTTClientStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }
                if (CmbStation != null)
                    CmbStation.SelectionChanged += CmbStation_TextChanged;

                if (CmbLinkType != null)
                    CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                DependencyPropertyDescriptor descriptor =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbStation, CmbStation_TextChanged);
                CmbStation_TextChanged();

                DependencyPropertyDescriptor descriptor2 =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor2.AddValueChanged(CmbJsonTimestampFormat, CmbJsonTimestampFormat_TextChanged);
                CmbJsonTimestampFormat_TextChanged();

                DependencyPropertyDescriptor descriptor3 =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor3.AddValueChanged(CmbLinkType, CmbLinkType_TextChanged);
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
                //if (baseDyn != null)
                //{
                //    baseDyn.TXBStateCommandVariable.Visibility = Visibility.Visible;
                //    baseDyn.DKPStateCommandVariable.Visibility = Visibility.Visible;
                //    baseDyn.CmbLinkTypeText.Visibility = Visibility.Visible;
                //    baseDyn.CmbLinkType.Visibility = Visibility.Visible;
                //    baseDyn.OutputAtStartupText.Visibility = Visibility.Visible;
                //    baseDyn.OutputAtStartup.Visibility = Visibility.Visible;
                //}

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
                //if (baseDyn != null)
                //{
                //    baseDyn.TXBStateCommandVariable.Visibility = Visibility.Visible;
                //    baseDyn.DKPStateCommandVariable.Visibility = Visibility.Visible;
                //    baseDyn.CmbLinkTypeText.Visibility = Visibility.Visible;
                //    baseDyn.CmbLinkType.Visibility = Visibility.Visible;
                //    baseDyn.OutputAtStartupText.Visibility = Visibility.Visible;
                //    baseDyn.OutputAtStartup.Visibility = Visibility.Visible;
                //}

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
