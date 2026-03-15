////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynamicSettingsEditor.xaml.cs
//
// summary:	Implements the dynamic settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace BACnet.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        BACnetDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        BACnetDynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"ObjectName", new SettingsControls.ObjectName()},
            {"BACnetObjectType", new SettingsControls.BACnetObjectType() },
            {"InstanceNumber", new SettingsControls.InstanceNumber() },
            {"PropertyIdentifier", new SettingsControls.PropertyIdentifier() },
            {"EnableCOV", new SettingsControls.EnableCOV() },
            {"DataSize", new SettingsControls.DataSize() },
            {"ArrayIndex", new SettingsControls.ArrayIndex() },
            {"PriorityLevel", new SettingsControls.PriorityLevel() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        
        ComboBox CmbObjectType = null;
        
        ComboBox CmbPropertyIdentifier = null;
        
        CheckBox CkCOVEnable = null;
        TextBlock TxBCOVEnable = null;

        ComboBox CmbPriorityLevel = null;
        TextBlock TxBPriorityLevel = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

            //executed at loaded
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
                    thisTagSettings = new BACnetDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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

                if (DynamicSettingsDict.TryGetValue("ObjectName", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (DynamicSettingsDict.TryGetValue("BACnetObjectType", out row))
                {
                    CmbObjectType = (ComboBox)row.FindName("CmbObjectType");
                    if (CmbObjectType != null)
                    {
                        CmbObjectType.ItemsSource = new List<BACnetEnums.ObjectTypes>()
                        {
                            BACnetEnums.ObjectTypes.ACCUMULATOR,
                            BACnetEnums.ObjectTypes.ANALOG_INPUT,
                            BACnetEnums.ObjectTypes.ANALOG_OUTPUT,
                            BACnetEnums.ObjectTypes.ANALOG_VALUE,
                            BACnetEnums.ObjectTypes.BINARY_INPUT,
                            BACnetEnums.ObjectTypes.BINARY_OUTPUT,
                            BACnetEnums.ObjectTypes.BINARY_VALUE,
                            BACnetEnums.ObjectTypes.CALENDAR,
                            BACnetEnums.ObjectTypes.DEVICE,
                            BACnetEnums.ObjectTypes.MULTI_STATE_INPUT,
                            BACnetEnums.ObjectTypes.MULTI_STATE_OUTPUT,
                            BACnetEnums.ObjectTypes.MULTI_STATE_VALUE,
                            BACnetEnums.ObjectTypes.SCHEDULE,
                            BACnetEnums.ObjectTypes.LIFE_SAFETY_ZONE,
                            BACnetEnums.ObjectTypes.LIFE_SAFETY_POINT,
                            BACnetEnums.ObjectTypes.PULSE_CONVERTER,
                        };
                    }
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (DynamicSettingsDict.TryGetValue("InstanceNumber", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (DynamicSettingsDict.TryGetValue("PropertyIdentifier", out row))
                {
                    CmbPropertyIdentifier = (ComboBox)row.FindName("CmbPropertyIdentifier");
                    if (CmbPropertyIdentifier != null)
                    {
                        CmbPropertyIdentifier.ItemsSource = Enum.GetValues(typeof(PriorityLevels));
                    }
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (DynamicSettingsDict.TryGetValue("EnableCOV", out row))
                {
                    CkCOVEnable = (CheckBox)row.FindName("CkCOVEnable");
                    TxBCOVEnable = (TextBlock)row.FindName("TxBCOVEnable");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (thisTagSettings.VarType == UFUAModel.DataType.String)
                {
                    if (DynamicSettingsDict.TryGetValue("DataSize", out row))
                    {
                        row.DataContext = DataContext;
                        MainStack.Children.Add(row);
                    }
                }
                else
                {
                    // string size
                    thisTagSettings.DataSize = 0;
                }

                if (DynamicSettingsDict.TryGetValue("ArrayIndex", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                if (DynamicSettingsDict.TryGetValue("PriorityLevel", out row))
                {
                    CmbPriorityLevel = (ComboBox)row.FindName("CmbPriorityLevel");
                    if (CmbPriorityLevel != null)
                    {
                        CmbPriorityLevel.ItemsSource = Enum.GetValues(typeof(PriorityLevels));
                    }
                    TxBPriorityLevel = (TextBlock)row.FindName("TxBPriorityLevel");
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
                //Swap Bytes*
                if (baseSettingsCtrls.TryGetValue("SwapBytes", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Swap Words*
                if (baseSettingsCtrls.TryGetValue("SwapWords", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //Output at Startup*
                if (baseSettingsCtrls.TryGetValue("OutputStartup", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                
                //Element Number*
                if (baseSettingsCtrls.TryGetValue("ElementNumber", out row))
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
                    configuration = (from tag in new XPQuery<BACnetDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new BACnetDriverSettings(ufw);
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

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbObjectType, CmbObjectType_TextChanged);
                               
                CmbObjectType_TextChanged();

                DependencyPropertyDescriptor descriptor1 =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor1.AddValueChanged(CmbPropertyIdentifier, CmbPropertyIdentifier_TextChanged);
                CmbPropertyIdentifier_TextChanged();
            };

            IsVisibleChanged += (o, e) =>
            {
                bVisibleOnce |= (bool)e.NewValue;
            };

            Unloaded += (o, e) =>
            {
                if (!BACnetEnums.isCovSupported(CmbObjectType.Text, CmbPropertyIdentifier.Text))
                    CkCOVEnable.IsChecked = false;
                if (bLoaded && bVisibleOnce)
                {
                    bLoaded = false;
                    var s = DataContext as BACnetDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbObjectType_TextChanged(object sender, EventArgs e)
        {
            CmbObjectType_TextChanged();
        }

        private void CmbObjectType_TextChanged()
        {
            if (CmbObjectType.Text == String.Empty)
                return;
            CmbPropertyIdentifier.ItemsSource = BACnetEnums.ObjectPropertyDictionaryAll[CmbObjectType.Text];

            // when new object type was selected, last related property identifier could not be valid. If so, select the 1st element of list
            if (CmbPropertyIdentifier.SelectedValue == null)
            {
                if (CmbPropertyIdentifier.Items.Count > 0)
                    CmbPropertyIdentifier.SelectedIndex = 0;
            }
            CmbPropertyIdentifier_TextChanged();
        }

        private void CmbPropertyIdentifier_TextChanged(object sender, EventArgs e)
        {
            CmbPropertyIdentifier_TextChanged();
        }

        private void CmbPropertyIdentifier_TextChanged()
        {
            if (BACnetEnums.isCovSupported(CmbObjectType.Text, CmbPropertyIdentifier.Text))
            {
                CkCOVEnable.Visibility = System.Windows.Visibility.Visible;
                TxBCOVEnable.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CkCOVEnable.Visibility = System.Windows.Visibility.Collapsed;
                TxBCOVEnable.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (BACnetEnums.isPrioritySupported(CmbObjectType.Text, CmbPropertyIdentifier.Text))
            {
                TxBPriorityLevel.Visibility = System.Windows.Visibility.Visible;
                CmbPriorityLevel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TxBPriorityLevel.Visibility = System.Windows.Visibility.Collapsed;
                CmbPriorityLevel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>   The connection. </summary>
        private string _Connection;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the connection. </summary>
        ///
        /// <value> The connection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }

        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
