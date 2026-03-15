using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace ROCDriver.UI
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
        ROCDriverDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        ROCDriverDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"PointType", new SettingsControls.PointType()},
            {"LogicalNumber", new SettingsControls.LogicalNumber()},
            {"Parameter", new SettingsControls.Parameter()},
            {"DataType", new SettingsControls.DataType()},
            {"StringLength", new SettingsControls.StringLength()}
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        TextBlock TxStringLength = null;
        TextBox TbStringLength = null;
        ComboBox CmbDataType = null;
        TextBlock TElemNumber = null;
        TextBox edtElementNumber = null;

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
                    thisTagSettings = new ROCDriverDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                // Point Type
                if (DynamicSettingsDict.TryGetValue("PointType", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Logical Number
                if (DynamicSettingsDict.TryGetValue("LogicalNumber", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // Parameter
                if (DynamicSettingsDict.TryGetValue("Parameter", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                // DataType
                if (DynamicSettingsDict.TryGetValue("DataType", out row))
                {
                    CmbDataType = (ComboBox)row.FindName("CmbDataType");
                    if (CmbDataType != null)
                        CmbDataType.ItemsSource = Enum.GetValues(typeof(DataTypes));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //String Length
                if (DynamicSettingsDict.TryGetValue("StringLength", out row))
                {
                    TxStringLength = (TextBlock)row.FindName("TxStringLength");
                    TbStringLength = (TextBox)row.FindName("TbStringLength");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Element Number*
                if (baseSettingsCtrls.TryGetValue("ElementNumber", out row))
                {
                    TElemNumber = (TextBlock)row.FindName("TElemNumber");
                    edtElementNumber = (TextBox)row.FindName("edtElementNumber");
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
                    configuration = (from tag in new XPQuery<ROCDriverDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new ROCDriverDriverSettings(ufw);
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

                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    if (TxStringLength != null)
                        TxStringLength.Visibility = Visibility.Collapsed;
                    if (TbStringLength != null)
                        TbStringLength.Visibility = Visibility.Collapsed;
                }

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbDataType, CmbDataType_TextChanged);
                CmbDataType_TextChanged();

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
                    var s = DataContext as ROCDriverDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbDataType_TextChanged(object sender, EventArgs e)
        {
            CmbDataType_TextChanged();
        }

        private void CmbDataType_TextChanged()
        {
            if (CmbDataType == null)
                return;
            if((thisTagSettings.VarType == UFUAModel.DataType.String) &&
                ((DataTypes)CmbDataType.SelectedIndex == DataTypes.AC))
            {
                if (TxStringLength != null)
                    TxStringLength.Visibility = Visibility.Visible;
                if (TbStringLength != null)
                    TbStringLength.Visibility = Visibility.Visible;
            }
            else
            {
                if (TxStringLength != null)
                    TxStringLength.Visibility = Visibility.Collapsed;
                if (TbStringLength != null)
                    TbStringLength.Visibility = Visibility.Collapsed;

                if ((thisTagSettings.VarType == UFUAModel.DataType.Boolean) &&
                    ((DataTypes)CmbDataType.SelectedIndex == DataTypes.BIN))
                {
                    if (TElemNumber != null)
                        TElemNumber.Visibility = Visibility.Visible;
                    if (edtElementNumber != null)
                        edtElementNumber.Visibility = Visibility.Visible;
                }
                else
                {
                    if (TElemNumber != null)
                        TElemNumber.Visibility = Visibility.Collapsed;
                    if (edtElementNumber != null)
                        edtElementNumber.Visibility = Visibility.Collapsed;
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
    }
}
