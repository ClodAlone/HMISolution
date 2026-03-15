using System;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace IEC61850.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        IEC61850DriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        IEC61850DynTagSettings thisTagSettings;
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
                    thisTagSettings = new IEC61850DynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                    configuration = (from tag in new XPQuery<IEC61850DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new IEC61850DriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                CmbFunctionalConstraint.ItemsSource = Enum.GetValues(typeof(FunctionalConstraints));
                CmbMMSDataType.ItemsSource = Enum.GetValues(typeof(MMSDataTypes));
                CmbReportType.ItemsSource = Enum.GetValues(typeof(ReportTypes));
                CmbReportType_SelectionChanged(null, null);

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                // hide unsupported standard property
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapBytes.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = System.Windows.Visibility.Collapsed;
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
                    var s = DataContext as IEC61850DynTagSettings;
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

        private void CmbMMSDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IEC61850DynTagSettings.IsMMSDataStringType((MMSDataTypes)CmbMMSDataType.SelectedItem) || ((MMSDataTypes)CmbMMSDataType.SelectedItem) == MMSDataTypes.UTCTime || ((MMSDataTypes)CmbMMSDataType.SelectedItem) == MMSDataTypes.BinaryTime)
            {
                tbDataMaximumLength.Visibility = System.Windows.Visibility.Visible;
                TxtDataMaximumLength.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TxtDataMaximumLength.Visibility = System.Windows.Visibility.Collapsed;
                tbDataMaximumLength.Visibility = System.Windows.Visibility.Collapsed;
                tbDataMaximumLength.Text = "0";
            }
        }

        //private void CmbFunctionalConstraint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //if (((FunctionalConstraints)CmbFunctionalConstraint.SelectedItem) == FunctionalConstraints.BR || ((FunctionalConstraints)CmbFunctionalConstraint.SelectedItem) == FunctionalConstraints.RP)
            //{                
            //    TxtReportType.Visibility = System.Windows.Visibility.Visible;
            //    CmbReportType.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    TxtReportType.Visibility = System.Windows.Visibility.Collapsed;
            //    CmbReportType.Visibility = System.Windows.Visibility.Collapsed;
            //    CmbReportType.SelectedValue = ReportTypes.None;                
            //}
        //}

        private void CmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((CmbReportType.SelectedItem != null) && ((ReportTypes)CmbReportType.SelectedItem) == ReportTypes.None)
            {
                TxtReportLogicalDeviceName.Visibility = System.Windows.Visibility.Collapsed;
                tbReportLogicalDeviceName.Visibility = System.Windows.Visibility.Collapsed;
                TxtReportLogicalNodeName.Visibility = System.Windows.Visibility.Collapsed;
                tbReportLogicalNodeName.Visibility = System.Windows.Visibility.Collapsed;
                TxtReportName.Visibility = System.Windows.Visibility.Collapsed;
                tbReportName.Visibility = System.Windows.Visibility.Collapsed;
                tbReportLogicalDeviceName.Text = string.Empty;
                tbReportLogicalNodeName.Text = string.Empty;
                tbReportName.Text = string.Empty;
            }
            else
            {
                TxtReportLogicalDeviceName.Visibility = System.Windows.Visibility.Visible;
                tbReportLogicalDeviceName.Visibility = System.Windows.Visibility.Visible;
                TxtReportLogicalNodeName.Visibility = System.Windows.Visibility.Visible;
                tbReportLogicalNodeName.Visibility = System.Windows.Visibility.Visible;
                TxtReportName.Visibility = System.Windows.Visibility.Visible;
                tbReportName.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
