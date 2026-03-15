using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace Fatek.UI
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
        FatekDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        FatekDynTagSettings thisTagSettings;

        //DriverCodeBaseEx.UI.Controls.BaseDynamicSettingsNoMethod baseDyn;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"StartAddress", new SettingsControls.StartAddress() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;
                baseSettingsCtrls = new DriverCodeBaseEx.UI.BaseSettings().GetBaseDynamicSettings();
                
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new FatekDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    if (!thisTag.IsObjectType)
                    {
                        thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;
                        thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    }
                }

                //if (!thisTag.IsMethod)
                //{
                //    baseDyn.CmbMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    baseDyn.TMethod.Visibility = System.Windows.Visibility.Collapsed;
                //    thisTagSettings.MethodID = -1;
                //}

                DataContext = null;
                DataContext = thisTagSettings;
                UserControl row;
                //baseDyn.DataContext = DataContext;
                //if (LoadbaseDyn)
                //    MainStack.Children.Insert(0,baseDyn);

                //Address
                if (DynamicSettingsDict.TryGetValue("StartAddress", out row))
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
                    if (CmbLinkType != null)
                    {
                        CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                    }
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
                //Swap DWord
                //if (DynamicSettingsDict.TryGetValue("SwapDWord", out row))
                //{
                //    row.DataContext = DataContext;
                //    MainStack.Children.Add(row);
                //}
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
                    configuration = (from tag in new XPQuery<FatekDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new FatekDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                CmbStation.SelectionChanged += CmbStation_SelectionChanged;

                //CmbFunctionCode.ItemsSource = Enum.GetValues(typeof(FatekProtocol.FunctionCodes));

                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                //if (thisTagSettings.VarType != UFUAModel.DataType.String)
                //{
                //    TxBStringLength.Visibility = Visibility.Collapsed;
                //    TxStringLength.Visibility = Visibility.Collapsed;
                //}

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbLinkType, CmbLinkType_TextChanged);
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

                //baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                //baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                //baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                //baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;

                //baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                //baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;
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
                    var s = DataContext as FatekDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int nAddressType = (int)AddressTypes.ZeroBased;
            if (e.AddedItems.Count == 0 && !(e.AddedItems[0] is DriverCodeBaseEx.StationSettings))
                return;
            string sn = (e.AddedItems[0] as DriverCodeBaseEx.StationSettings).Name;
            if (configuration != null && configuration.StationSettings.Count > 0)
            {
                var at = (from s in configuration.StationSettings.AsParallel()
                 where s.Name == sn
                 orderby s.Name
                 select s).ToList();
                //if(at.Count > 0 && at[0] is FatekStationSettings)
                //{
                //    nAddressType = (at[0] as FatekStationSettings).AddressType;
                //}
            }
            //var ds = DataContext as FatekDynTagSettings;
            //if (ds != null)
            //    ds.AddressType = nAddressType;
        }

        private void CmbLinkType_TextChanged(object sender = null, EventArgs e = null)
        {
            var cb = (ComboBox)sender;
            if (cb != null)
            {
                //if ((LinkType)cb.SelectedIndex == LinkType.Input)
                //{
                //    TxBroadcast.Visibility = Visibility.Collapsed;
                //    CheckBroadcast.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    TxBroadcast.Visibility = Visibility.Visible;
                //    CheckBroadcast.Visibility = Visibility.Visible;
                //}
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
            DynamicSettingsDict.Clear();

            if (CmbStation != null)
                CmbStation.SelectionChanged -= CmbStation_SelectionChanged;

            if (baseSettingsCtrls != null)
                baseSettingsCtrls.Clear();

            if (ufw != null)
                ufw.Dispose();
            if (idl != null)
                idl.Dispose();
        }
        #endregion
    }
}
