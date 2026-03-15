using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using IEC61850.UI.SettingsControls;
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
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>()
        {
            {"LogicalDeviceName", new SettingsControls.LogicalDeviceName()},
            {"LogicalNodeName", new SettingsControls.LogicalNodeName()},
            {"FunctionalConstraint", new SettingsControls.FunctionalConstraint()},
            {"DataItemIdentifier", new SettingsControls.DataItemIdentifier()},
            {"MMSDataType", new SettingsControls.MMSDataType()},
            {"DataMaximumLength", new SettingsControls.DataMaximumLength()},
            {"RetryOutputInCaseOfError", new SettingsControls.RetryOutputInCaseOfError()},
            {"ReportType", new SettingsControls.ReportType()},            
            {"ReportLogicalDeviceName", new SettingsControls.ReportLogicalDeviceName()},
            {"ReportLogicalNodeName", new SettingsControls.ReportLogicalNodeName()},
            {"ReportName", new SettingsControls.ReportName()},
            {"InitializeData", new SettingsControls.InitializeData()},
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbReportType = null;
        ComboBox CmbFunctionalConstraint = null;
        ComboBox CmbMMSDataType = null;
        TextBlock TxtDataMaximumLength = null;
        TextBox tbDataMaximumLength = null;
        TextBlock TxtReportLogicalDeviceName = null;
        TextBox tbReportLogicalDeviceName = null;
        TextBlock TxtReportLogicalNodeName = null;
        TextBox tbReportLogicalNodeName = null;
        TextBlock TxtReportName = null;
        TextBox tbReportName = null;
        DependencyPropertyDescriptor descriptor = null;

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

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
                //LogicalDeviceName
                if (DynamicSettingsDict.TryGetValue("LogicalDeviceName", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //LogicalNodeName
                if (DynamicSettingsDict.TryGetValue("LogicalNodeName", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //FunctionalConstraint
                if (DynamicSettingsDict.TryGetValue("FunctionalConstraint", out row))
                {
                    CmbFunctionalConstraint = (ComboBox)row.FindName("CmbFunctionalConstraint");
                    if (CmbFunctionalConstraint != null)
                        CmbFunctionalConstraint.ItemsSource = Enum.GetValues(typeof(FunctionalConstraints));

                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //DataItemIdentifier
                if (DynamicSettingsDict.TryGetValue("DataItemIdentifier", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //MMSDataType
                if (DynamicSettingsDict.TryGetValue("MMSDataType", out row))
                {
                    CmbMMSDataType = (ComboBox)row.FindName("CmbMMSDataType");
                    if (CmbMMSDataType != null)
                        CmbMMSDataType.ItemsSource = Enum.GetValues(typeof(MMSDataTypes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //DataMaximumLength
                if (DynamicSettingsDict.TryGetValue("DataMaximumLength", out row))
                {
                    tbDataMaximumLength = (TextBox)row.FindName("tbDataMaximumLength");
                    TxtDataMaximumLength = (TextBlock)row.FindName("TxtDataMaximumLength");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //RetryOutputInCaseOfError
                if (DynamicSettingsDict.TryGetValue("RetryOutputInCaseOfError", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //ReportType
                if (DynamicSettingsDict.TryGetValue("ReportType", out row))
                {
                    CmbReportType = (ComboBox)row.FindName("CmbReportType");
                    if (CmbReportType != null)
                        CmbReportType.ItemsSource = Enum.GetValues(typeof(ReportTypes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //ReportLogicalDeviceName
                if (DynamicSettingsDict.TryGetValue("ReportLogicalDeviceName", out row))
                {
                    TxtReportLogicalDeviceName = (TextBlock)row.FindName("TxtReportLogicalDeviceName");
                    tbReportLogicalDeviceName = (TextBox)row.FindName("tbReportLogicalDeviceName");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //ReportLogicalNodeName
                if (DynamicSettingsDict.TryGetValue("ReportLogicalNodeName", out row))
                {
                    TxtReportLogicalNodeName = (TextBlock)row.FindName("TxtReportLogicalNodeName");
                    tbReportLogicalNodeName = (TextBox)row.FindName("tbReportLogicalNodeName");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //ReportName
                if (DynamicSettingsDict.TryGetValue("ReportName", out row))
                {
                    TxtReportName = (TextBlock)row.FindName("TxtReportName");
                    tbReportName = (TextBox)row.FindName("tbReportName");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //InitializeData
                if (DynamicSettingsDict.TryGetValue("InitializeData", out row))
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
                    configuration = (from tag in new XPQuery<IEC61850DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new IEC61850DriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                if (CmbReportType != null)
                {
                    descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                    descriptor.AddValueChanged(CmbReportType, CmbReportType_SelectionChanged);
                }

                if (CmbMMSDataType != null)
                {
                    if (descriptor  == null)
                        descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                    descriptor.AddValueChanged(CmbMMSDataType, CmbMMSDataType_SelectionChanged);
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
                    var s = DataContext as IEC61850DynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                    descriptor.RemoveValueChanged(CmbReportType, CmbReportType_SelectionChanged);
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

        private void CmbMMSDataType_SelectionChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;

            if (CmbMMSDataType.SelectedValue == null)
                return;
            
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

        private void CmbReportType_SelectionChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;
                        
            if (CmbReportType.SelectedValue == null)
                return;

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
