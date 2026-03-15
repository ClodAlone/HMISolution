using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBaseEx.Enumerators;
using EIB;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace EIB.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded = false;
        bool bVisibleOnce = false;
        UnitOfWork ufw = null;
        IDataLayer idl = null;
        EIBDriverSettings configuration = null;
        IDynamicSettingsEditing thisTag = null;
        EIBDynTagSettings thisTagSettings = null;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"OutputOnlyOnRequestCnt", new SettingsControls.OutputOnlyOnRequestCnt()},
            {"PollingOnlyOnRequestCnt", new SettingsControls.PollingOnlyOnRequestCnt()},
            {"EnablePollingCnt", new SettingsControls.EnablePollingCnt()},
            {"PollingTimeCnt", new SettingsControls.PollingTimeCnt()},
            {"RetryInitialPollingCnt", new SettingsControls.RetryInitialPollingCnt()},
            {"AutoResetNewDataTimeCnt", new SettingsControls.AutoResetNewDataTimeCnt()},
            {"RetryOutputCnt", new SettingsControls.RetryOutputCnt()},
            {"InputGroupsCnt", new SettingsControls.InputGroupsCnt()},
            {"PollingGroupCnt", new SettingsControls.PollingGroupCnt()},
            {"OutputGroupCnt", new SettingsControls.OutputGroupCnt()},
            {"DataFormatCnt", new SettingsControls.DataFormatCnt()}
        };

        CheckBox OutputOnlyOnRequest = null;
        TextBlock OutputOnlyOnRequestText = null;
        CheckBox PollingOnlyOnRequest = null;
        TextBlock PollingOnlyOnRequestText = null;
        CheckBox EnablePolling = null;
        TextBlock EneblePollingText = null;        
        TextBox PollingTime = null;
        TextBlock PollingTimeText = null;
        CheckBox RetryInitialPolling = null;
        TextBlock RetryInitialPollingText = null;
        TextBlock AutoResetNewDataTimeText = null;
        TextBox AutoResetNewDataTime = null;
        CheckBox RetryOutput = null;
        TextBlock RetryOutputText = null;
        TextBox InputGroups = null;
        TextBlock InputGroupsText = null;
        TextBox PollingGroup = null;
        TextBlock PollingGroupText = null;
        TextBox OutputGroup = null;
        TextBlock OutputGroupText = null;
        ComboBox CmbEisDataFormat= null;
        TextBlock CmbEisDataFormatText = null;

        CheckBox CheckSwapBytes = null;
        //CheckBox CheckSwapBytesErr = null;
        TextBlock CheckSwapBytesText = null;

        CheckBox CheckSwapWords = null;
        //CheckBox CheckSwapWordsErr = null;
        TextBlock CheckSwapWordsText = null;
        

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;


        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                {
                    return;
                }
                bLoaded = true;

                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new EIBDynTagSettings();
                    if (thisTag.DynamicSettingsForEditing != null)
                    {
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    }
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                }
                                
                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;

                //OutputOnlyOnRequest
                if (DynamicSettingsDict.TryGetValue("OutputOnlyOnRequestCnt", out row))
                {
                    OutputOnlyOnRequest = (CheckBox)row.FindName("OutputOnlyOnRequest");
                    OutputOnlyOnRequestText = (TextBlock)row.FindName("OutputOnlyOnRequestText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //PollingOnlyOnRequestCnt
                if (DynamicSettingsDict.TryGetValue("PollingOnlyOnRequestCnt", out row))
                {                    
                    PollingOnlyOnRequest = (CheckBox)row.FindName("PollingOnlyOnRequest");
                    PollingOnlyOnRequestText = (TextBlock)row.FindName("PollingOnlyOnRequestText");

                    PollingOnlyOnRequest.Checked += PollingOnlyOnRequest_Checked;
                    PollingOnlyOnRequest.Unchecked += PollingOnlyOnRequest_Checked;
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //EnablePollingCnt
                if (DynamicSettingsDict.TryGetValue("EnablePollingCnt", out row))
                {
                    EnablePolling = (CheckBox)row.FindName("EnablePolling");
                    EneblePollingText = (TextBlock)row.FindName("EnablePollingText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //PollingTimeCnt
                if (DynamicSettingsDict.TryGetValue("PollingTimeCnt", out row))
                {
                    PollingTime = (TextBox)row.FindName("PollingTime");
                    PollingTimeText = (TextBlock)row.FindName("PollingTimeText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //RetryInitialPollingCnt
                if (DynamicSettingsDict.TryGetValue("RetryInitialPollingCnt", out row))
                {
                    RetryInitialPolling = (CheckBox)row.FindName("RetryInitialPolling");
                    RetryInitialPollingText = (TextBlock)row.FindName("RetryInitialPollingText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //AutoResetNewDataTimeCnt
                if (DynamicSettingsDict.TryGetValue("AutoResetNewDataTimeCnt", out row))
                {
                    AutoResetNewDataTime = (TextBox)row.FindName("AutoResetNewDataTime");
                    AutoResetNewDataTimeText = (TextBlock)row.FindName("AutoResetNewDataTimeText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //RetryOutputCnt
                if (DynamicSettingsDict.TryGetValue("RetryOutputCnt", out row))
                {
                    RetryOutput = (CheckBox)row.FindName("RetryOutput");
                    RetryOutputText = (TextBlock)row.FindName("RetryOutputText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //InputGroupsCnt
                if (DynamicSettingsDict.TryGetValue("InputGroupsCnt", out row))
                {
                    InputGroups = (TextBox)row.FindName("InputGroups");
                    InputGroupsText = (TextBlock)row.FindName("InputGroupsText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //OutputGroupCnt
                if (DynamicSettingsDict.TryGetValue("OutputGroupCnt", out row))
                {
                    OutputGroup = (TextBox)row.FindName("OutputGroup");
                    OutputGroupText = (TextBlock)row.FindName("OutputGroupText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                
                //PollingGroupCnt
                if (DynamicSettingsDict.TryGetValue("PollingGroupCnt", out row))
                {
                    PollingGroup = (TextBox)row.FindName("PollingGroup");
                    PollingGroupText = (TextBlock)row.FindName("PollingGroupText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //DataFormatCnt
                if (DynamicSettingsDict.TryGetValue("DataFormatCnt", out row))
                {
                    CmbEisDataFormat = (ComboBox)row.FindName("CmbEisDataFormat");
                    CmbEisDataFormatText = (TextBlock)row.FindName("CmbEisDataFormatText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // Standard properties                
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
                    CheckSwapBytes = (CheckBox)row.FindName("CheckSwapBytes");
                    CheckSwapBytesText = (TextBlock)row.FindName("CheckSwapBytesText");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Swap Words*
                if (baseSettingsCtrls.TryGetValue("SwapWords", out row))
                {
                    CheckSwapWords = (CheckBox)row.FindName("CheckSwapWords");
                    CheckSwapWordsText = (TextBlock)row.FindName("CheckSwapWordsText");
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
                    configuration = (from tag in new XPQuery<EIBDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new EIBDriverSettings(ufw);
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

                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFBit,  Tag = EIBProtocol.EISDATAFORMAT.EISDFBit });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFByte, Tag = EIBProtocol.EISDATAFORMAT.EISDFByte });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFWord, Tag = EIBProtocol.EISDATAFORMAT.EISDFWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFDWord, Tag = EIBProtocol.EISDATAFORMAT.EISDFDWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFFloat, Tag = EIBProtocol.EISDATAFORMAT.EISDFFloat });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS3, Tag = EIBProtocol.EISDATAFORMAT.EISDFEIS3 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS4, Tag = EIBProtocol.EISDATAFORMAT.EISDFEIS4 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS5, Tag = EIBProtocol.EISDATAFORMAT.EISDFEIS5 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS6, Tag = EIBProtocol.EISDATAFORMAT.EISDFEIS6 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD6Bytes, Tag = EIBProtocol.EISDATAFORMAT.EISDFAccessPWD6Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD10Bytes, Tag = EIBProtocol.EISDATAFORMAT.EISDFAccessPWD10Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFInt64, Tag = EIBProtocol.EISDATAFORMAT.EISDFInt64 });

                
                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));

                descriptor.AddValueChanged(CmbLinkType, CmbLinkType_TextChanged);
                CmbLinkType_TextChanged();
                descriptor.AddValueChanged(CmbEisDataFormat, CmbEisDataFormat_TextChanged);
                CmbEisDataFormat_TextChanged();

                //DependencyPropertyDescriptor descriptorCheck =
                //   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                //descriptor.AddValueChanged(PollingOnlyOnRequest, PollingOnlyOnRequest_Checked);
                //PollingOnlyOnRequest_Checked(PollingOnlyOnRequest, null);

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
                    var s = DataContext as EIBDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        //private void PollingOnlyOnRequest_Checked(object sender, EventArgs e)
        //{
        //    CheckBox checkBox = (CheckBox)sender;

        //    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility =
        //    PollingTimeError.Visibility = PollingTime.Visibility = PollingTimeText.Visibility =
        //    EnablePolling.Visibility = EneblePollingText.Visibility = ((bool)checkBox.IsChecked ? Visibility.Collapsed : Visibility.Visible);
        //}

        private void CmbEisDataFormat_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                switch ((EIBProtocol.EISDATAFORMAT)cb.SelectedIndex)
                {
                    case EIBProtocol.EISDATAFORMAT.EISDFBit:
                    case EIBProtocol.EISDATAFORMAT.EISDFByte:
                        CheckSwapBytes.Visibility = Visibility.Collapsed;
                        //CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                        CheckSwapBytesText.Visibility = Visibility.Collapsed;
                        CheckSwapWords.Visibility = Visibility.Collapsed;
                        //CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        break;
                    /*case EISDATAFORMAT.EISDFFloat:
                        break;
                    case EISDATAFORMAT.EISDFDWord:
                        break;*/
                    case EIBProtocol.EISDATAFORMAT.EISDFWord:
                        CheckSwapWords.Visibility = Visibility.Collapsed;
                        //CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        break;
                    /*case EISDATAFORMAT.EISDFEIS3:
                        break;
                    case EISDATAFORMAT.EISDFEIS4:
                        break;
                    case EISDATAFORMAT.EISDFEIS5:
                        break;
                    case EISDATAFORMAT.EISDFEIS6:
                        break;
                    case EISDATAFORMAT.EISDFAccessPWD6Bytes:
                        break;
                    case EISDATAFORMAT.EISDFAccessPWD10Bytes:
                        break;*/
                    default:
                        CheckSwapBytes.Visibility = Visibility.Visible;
                        //CheckSwapBytesErr.Visibility = Visibility.Visible;
                        CheckSwapBytesText.Visibility = Visibility.Visible;
                        CheckSwapWords.Visibility = Visibility.Visible;
                        //CheckSwapWordsErr.Visibility = Visibility.Visible;
                        CheckSwapWordsText.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if(cb != null)
            {
                if((LinkType)cb.SelectedIndex == LinkType.Input)
                {
                    CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Collapsed;
                    PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Visible;
                    InputGroups.Visibility = InputGroupsText.Visibility = Visibility.Visible;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Collapsed;
                    AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Visible;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Collapsed;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Visible;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Visible;
                    PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Visible;
                    EnablePolling.Visibility = EneblePollingText.Visibility = Visibility.Visible;
                }
                else if ((LinkType)cb.SelectedIndex == LinkType.ExceptionOutput || (LinkType)cb.SelectedIndex == LinkType.UnconditionalOutput)
                {
                    CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Visible;
                    PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Collapsed;
                    InputGroups.Visibility = InputGroupsText.Visibility = Visibility.Collapsed;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Visible;
                    AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Collapsed;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Visible;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Collapsed;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Collapsed;
                    PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Collapsed;
                    EnablePolling.Visibility = EneblePollingText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Visible;
                    PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Visible;
                    InputGroups.Visibility = InputGroupsText.Visibility = Visibility.Visible;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Visible;
                    AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Visible;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Visible;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Visible;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Visible;
                    PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Visible;
                    EnablePolling.Visibility = EneblePollingText.Visibility = Visibility.Visible;
                }
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

        private void PollingOnlyOnRequest_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility =
            PollingTime.Visibility = PollingTimeText.Visibility =
            EnablePolling.Visibility = EneblePollingText.Visibility = ((bool)checkBox.IsChecked ? Visibility.Collapsed : Visibility.Visible);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (PollingOnlyOnRequest != null) {
                PollingOnlyOnRequest.Unchecked += PollingOnlyOnRequest_Checked;
                PollingOnlyOnRequest.Checked += PollingOnlyOnRequest_Checked;
            }
        }
    }
}
