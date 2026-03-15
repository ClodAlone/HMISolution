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
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;
using DevExpress.XtraReports.Native;

namespace BACnet.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;
        /// <summary>   true to visible once. </summary>
        bool bVisibleOnce;
        /// <summary>   The idl. </summary>
        IDataLayer idl;
        /// <summary>   The ufw. </summary>
        UnitOfWork ufw;
        /// <summary>   The configuration. </summary>
        BACnetDriverSettings configuration;
        /// <summary>   this tag. </summary>
        IDynamicSettingsEditing thisTag;
        /// <summary>   this tag settings. </summary>
        BACnetDynTagSettings thisTagSettings;
        /// <summary>   Default constructor. </summary>
        public DynamicSettingsEditor()
        {
            InitializeComponent();

            //executed at loaded
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
                    thisTagSettings = new BACnetDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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

                //insert base DynamicSettings setup
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

                try
                {
                    configuration = (from tag in new XPQuery<BACnetDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new BACnetDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    //insert Station in ComboBox baseDyn.CmbStation
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }

                //insert option in ComboBox 
                CmbPriorityLevel.ItemsSource = Enum.GetValues(typeof(PriorityLevels));


                //insert option in baseDyn ComboBox
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbObjectType, CmbObjectType_TextChanged);

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
                };

                CmbObjectType_TextChanged();

                DependencyPropertyDescriptor descriptor1 =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor1.AddValueChanged(CmbPropertyIdentifier, CmbProperty_TextChanged);
                CmbProperty_TextChanged();

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
            CmbProperty_TextChanged();
        }

        private void CmbProperty_TextChanged(object sender, EventArgs e)
        {
            CmbProperty_TextChanged();
        }

        private void CmbProperty_TextChanged()
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
