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
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using EIB;
using DriverCodeBase.Helpers;
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
        DriverCodeBase.UI.Controls.BaseDynamicSettings baseDyn = null;
        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                {
                    return;
                }
                bLoaded = true;

                baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
                bool LoadbaseDyn = false;

                baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumberErr.Visibility = Visibility.Collapsed;
                
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
                {
                    MainStack.Children.Insert(0,baseDyn);
                }

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

                try
                {
                    configuration = (from tag in new XPQuery<EIBDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (InvalidOperationException ex)
                {
                    configuration = new EIBDriverSettings(ufw) { AggregationLimit = 0, AggregationThreshold = 5 };
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFBit,  Tag = EISDATAFORMAT.EISDFBit });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFByte, Tag = EISDATAFORMAT.EISDFByte });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFWord, Tag = EISDATAFORMAT.EISDFWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFDWord, Tag = EISDATAFORMAT.EISDFDWord });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFFloat, Tag = EISDATAFORMAT.EISDFFloat });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS3, Tag = EISDATAFORMAT.EISDFEIS3 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS4, Tag = EISDATAFORMAT.EISDFEIS4 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS5, Tag = EISDATAFORMAT.EISDFEIS5 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFEIS6, Tag = EISDATAFORMAT.EISDFEIS6 });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD6Bytes, Tag = EISDATAFORMAT.EISDFAccessPWD6Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFAccessPWD10Bytes, Tag = EISDATAFORMAT.EISDFAccessPWD10Bytes });
                CmbEisDataFormat.Items.Add(new ComboBoxItem() { Content = Properties.Resources.EISDFInt64, Tag = EISDATAFORMAT.EISDFInt64 });

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));

                descriptor.AddValueChanged(baseDyn.CmbLinkType, CmbLinkType_TextChanged);
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
                switch ((EISDATAFORMAT)cb.SelectedIndex)
                {
                    case EISDATAFORMAT.EISDFBit:
                    case EISDATAFORMAT.EISDFByte:
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                        break;
                    /*case EISDATAFORMAT.EISDFFloat:
                        break;
                    case EISDATAFORMAT.EISDFDWord:
                        break;*/
                    case EISDATAFORMAT.EISDFWord:
                        baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
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
                        baseDyn.CheckSwapBytes.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapBytesText.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWords.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsErr.Visibility = Visibility.Visible;
                        baseDyn.CheckSwapWordsText.Visibility = Visibility.Visible;
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
                    CmbEisDataFormatError.Visibility = CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroupError.Visibility = OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Collapsed;
                    PollingGroupError.Visibility = PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Visible;
                    InputGroupError.Visibility = InputGroups.Visibility = InputGroupText.Visibility = Visibility.Visible;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Collapsed;
                    AutoResetNewDataTimeError.Visibility = AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Visible;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Collapsed;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Visible;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Visible;
                    PollingTimeError.Visibility = PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Visible;
                    EnablePolling.Visibility = EneblePollingText.Visibility = Visibility.Visible;
                }
                else if ((LinkType)cb.SelectedIndex == LinkType.ExceptionOutput || (LinkType)cb.SelectedIndex == LinkType.UnconditionalOutput)
                {
                    CmbEisDataFormatError.Visibility = CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroupError.Visibility = OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Visible;
                    PollingGroupError.Visibility = PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Collapsed;
                    InputGroupError.Visibility = InputGroups.Visibility = InputGroupText.Visibility = Visibility.Collapsed;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Visible;
                    AutoResetNewDataTimeError.Visibility = AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Collapsed;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Visible;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Collapsed;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Collapsed;
                    PollingTimeError.Visibility = PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Collapsed;
                    EnablePolling.Visibility = EneblePollingText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CmbEisDataFormatError.Visibility = CmbEisDataFormat.Visibility = CmbEisDataFormatText.Visibility = Visibility.Visible;
                    OutputGroupError.Visibility = OutputGroup.Visibility = OutputGroupText.Visibility = Visibility.Visible;
                    PollingGroupError.Visibility = PollingGroup.Visibility = PollingGroupText.Visibility = Visibility.Visible;
                    InputGroupError.Visibility = InputGroups.Visibility = InputGroupText.Visibility = Visibility.Visible;
                    RetryOutput.Visibility = RetryOutputText.Visibility = Visibility.Visible;
                    AutoResetNewDataTimeError.Visibility = AutoResetNewDataTime.Visibility = AutoResetNewDataTimeText.Visibility = Visibility.Visible;
                    OutputOnlyOnRequest.Visibility = OutputOnlyOnRequestText.Visibility = Visibility.Visible;
                    PollingOnlyOnRequest.Visibility = PollingOnlyOnRequestText.Visibility = Visibility.Visible;
                    RetryInitialPolling.Visibility = RetryInitialPollingText.Visibility = Visibility.Visible;
                    PollingTimeError.Visibility = PollingTime.Visibility = PollingTimeText.Visibility = Visibility.Visible;
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
            PollingTimeError.Visibility = PollingTime.Visibility = PollingTimeText.Visibility =
            EnablePolling.Visibility = EneblePollingText.Visibility = ((bool)checkBox.IsChecked ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
