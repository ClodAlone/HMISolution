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
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBaseEx.Enumerators;
using OmronEthernetIP;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using DocumentManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace OmronEthernetIP.UI
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
        OmronEthernetIPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        OmronEthernetIPDynTagSettings thisTagSettings;

        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"StructureStringLengths", new SettingsControls.StructureStringLengths()},
            {"TagFormat", new SettingsControls.TagFormat() },
            {"ABAddres", new SettingsControls.ABAddress() }
        };

        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;
        ComboBox CmbTagFormat = null;
        DockPanel DKPStructStringLength = null;
        TextBlock TxStructStringLength = null;
        TextBox TbStructStringLength = null;
        Button btnSet = null;
        Button btnReset = null;
        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (bLoaded)
                    return;

                bLoaded = true;

                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new OmronEthernetIPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
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
                //TagFormat
                if (DynamicSettingsDict.TryGetValue("TagFormat", out row))
                {
                    CmbTagFormat = (ComboBox)row.FindName("CmbTagFormat");
                    if (CmbTagFormat != null)
                        CmbTagFormat.ItemsSource = Enum.GetValues(typeof(TagFormats));
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //ABAddres
                if (DynamicSettingsDict.TryGetValue("ABAddres", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //StructureStringLengths
                if (DynamicSettingsDict.TryGetValue("StructureStringLengths", out row))
                {
                    DKPStructStringLength = ((DockPanel)row.FindName("DKPStructStringLength"));
                    TxStructStringLength = ((TextBlock)row.FindName("TxStructStringLength"));
                    TbStructStringLength = ((TextBox)row.FindName("TbStructStringLength"));

                    btnSet = ((Button)row.FindName("btnSet"));
                    if (btnSet != null)
                        btnSet.Click += SetStructStringLength_Click;
                    btnReset = ((Button)row.FindName("btnReset"));
                    if (btnReset != null)
                        btnReset.Click += ResetStructStringLength_Click;

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
                    if(CmbLinkType != null)
                        CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
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
                    //driver specific class
                    configuration = (from tag in new XPQuery<OmronEthernetIPDriverSettings>(ufw).AsParallel() select tag).Single();                
                }
                catch (InvalidOperationException ex)
                {
                    //driver specific class
                    configuration = new OmronEthernetIPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                    thisTagSettings.stationSettingList = new Dictionary<string, OmronEthernetIPStationSettings>();
                    foreach (OmronEthernetIPStationSettings settings in configuration.StationSettings)
                    {
                        if (!thisTagSettings.stationSettingList.ContainsKey(settings.Name))
                            thisTagSettings.stationSettingList.Add(settings.Name, settings);
                    }
                }


                // If the variable is a structure, shows the controls for setting the lengths of the elements of type string
                if(thisTag.IsObjectType)
                {
                    if(DKPStructStringLength != null)
                    DKPStructStringLength.Visibility = Visibility.Visible;
                    if(TxStructStringLength != null)
                    TxStructStringLength.Visibility = Visibility.Visible;
                    if(TbStructStringLength != null)
                    TbStructStringLength.Visibility = Visibility.Visible;
                }
                else
                {
                    if (DKPStructStringLength != null)
                        DKPStructStringLength.Visibility = Visibility.Collapsed;
                    if (TxStructStringLength != null)
                        TxStructStringLength.Visibility = Visibility.Collapsed;
                    if (TbStructStringLength != null)
                        TbStructStringLength.Visibility = Visibility.Collapsed;
                }

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
                    var s = DataContext as OmronEthernetIPDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
                if (btnSet != null)
                    btnSet.Click -= SetStructStringLength_Click;
                if (btnReset != null)
                    btnReset.Click -= ResetStructStringLength_Click;
            };
        }

        private void ResetStructStringLength_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as OmronEthernetIPDynTagSettings;
            nDC.StructStringFieldLengths = String.Empty;
        }

        private void SetStructStringLength_Click(object sender, RoutedEventArgs e)
        {
            if (thisTag.Members == null || thisTag.Members.Count == 0)
            {
                MessageBox.Show(Properties.Resources.ErrorPrototypeNotExist);
                TbStructStringLength.Text = string.Empty;
                return;
            }

            OmronEthernetIPStructStringLength SSL = new OmronEthernetIPStructStringLength();

            SSL.Parse(thisTag, TbStructStringLength.Text);
            // check if prototype contain at least one string's member
            if (!SSL.HasMembers())
            {
                MessageBox.Show(Properties.Resources.ErrorNoOnePrototypeMembersIsString);
                TbStructStringLength.Text = string.Empty;
                return;
            }

            PrototypeMemberStringLength d = new PrototypeMemberStringLength();
            d.DataContext = SSL.MemberView;

            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d, GeneralDialogButtons.OkCancelButtons, false)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.PrototypeEditor
            };

            if (newChDetDialog.ShowDialog() == true)
            {
                OmronEthernetIPStructStringLength.ProMemberView v = d.DataContext as OmronEthernetIPStructStringLength.ProMemberView;
                if (v != null)
                    TbStructStringLength.Text = SSL.UnSplitToStructString(v);
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
