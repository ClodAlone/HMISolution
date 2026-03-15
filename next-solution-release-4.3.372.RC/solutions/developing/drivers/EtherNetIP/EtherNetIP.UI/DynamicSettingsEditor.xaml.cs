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
using EtherNetIP;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;
using System.ComponentModel;

namespace EtherNetIP.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded = false;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        EtherNetIPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        EtherNetIPDynTagSettings thisTagSettings;
        bool alreadyLoaded = false;

        DependencyPropertyDescriptor descriptor = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                var baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();

                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new EtherNetIPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
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

                descriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(baseDyn.CmbStation, Base_StationChanged);
                Base_StationChanged();


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
                    //driver specific class
                    configuration = (from tag in new XPQuery<EtherNetIPDriverSettings>(ufw).AsParallel() select tag).Single();

                    

                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new EtherNetIPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, EtherNetIPStationSettings>();
                    foreach (EtherNetIPStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }

                //Dictionary<int, string> n = new Dictionary<int, string>();
                //n.Add((int)LinkType.Input, DriverCodeBase.Properties.Resources.LinkTypeInput);
                //n.Add((int)LinkType.InputOutput, DriverCodeBase.Properties.Resources.LinkTypeIO);
                //n.Add((int)LinkType.ExceptionOutput, DriverCodeBase.Properties.Resources.LinkTypeExcOut);
                //n.Add((int)LinkType.UnconditionalOutput, DriverCodeBase.Properties.Resources.LinkTypeUncOut);
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                CmbAddressType.ItemsSource = Enum.GetValues(typeof(AddressTypes));
                CmbTagFormat.ItemsSource = Enum.GetValues(typeof(TagFormats));

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
                    var s = DataContext as EtherNetIPDynTagSettings;
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

        private void Base_StationChanged(object sender = null, EventArgs e = null)
        {
            if (sender == null)
                return;

            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedValue == null)
                return;


            string station = cb.SelectedValue.ToString();

            if (!thisTagSettings.stationSettingList.ContainsKey(station))
                return;

            EtherNetIPStationSettings st = thisTagSettings.stationSettingList[station];

            if ((st.PlcType != PlcTypes.ControlLogix_CompactLogix) &&
                (st.PlcType != PlcTypes.Micro800_series))
            {                
                CmbAddressType.SelectedIndex = (int)AddressTypes.DataFile;
                CmbAddressType.IsEnabled = false;
            }
            else
            {
                CmbAddressType.IsEnabled = true;
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
