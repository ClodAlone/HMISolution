using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace ModbusTCP.UI
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
        ModbusTCPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        ModbusTCPDynTagSettings thisTagSettings;
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
                    thisTagSettings = new ModbusTCPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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

                try
                {
                    configuration = (from tag in new XPQuery<ModbusTCPDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new ModbusTCPDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                baseDyn.CmbStation.SelectionChanged += CmbStation_SelectionChanged;

                CmbFunctionCode.ItemsSource = Enum.GetValues(typeof(FunctionCodes));

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                if (thisTagSettings.VarType != UFUAModel.DataType.String)
                {
                    TxBStringLength.Visibility = Visibility.Collapsed;
                    TxStringLength.Visibility = Visibility.Collapsed;
                }

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(baseDyn.CmbLinkType, CmbLinkType_TextChanged);
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
                    var s = DataContext as ModbusTCPDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nAddressType = (int)AddressTypes.ZeroBased;
            if (e.AddedItems.Count == 0 && !(e.AddedItems[0] is DriverCodeBase.StationSettings))
                return;
            string sn = (e.AddedItems[0] as DriverCodeBase.StationSettings).Name;
            if (configuration != null && configuration.StationSettings.Count > 0)
            {
                var at = (from s in configuration.StationSettings.AsParallel()
                 where s.Name == sn
                 orderby s.Name
                 select s).ToList();
                if(at.Count > 0 && at[0] is ModbusTCPStationSettings)
                {
                    nAddressType = (at[0] as ModbusTCPStationSettings).AddressType;
                }
            }
            var ds = DataContext as ModbusTCPDynTagSettings;
            if (ds != null)
                ds.AddressType = nAddressType;
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                if ((LinkType)cb.SelectedIndex == LinkType.Input)
                {
                    TxBroadcast.Visibility = Visibility.Collapsed;
                    CheckBroadcast.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TxBroadcast.Visibility = Visibility.Visible;
                    CheckBroadcast.Visibility = Visibility.Visible;
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
            baseDyn.CmbStation.SelectionChanged -= CmbStation_SelectionChanged;
            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
