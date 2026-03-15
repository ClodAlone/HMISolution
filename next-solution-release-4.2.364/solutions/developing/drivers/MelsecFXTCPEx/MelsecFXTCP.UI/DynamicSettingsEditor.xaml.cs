using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MelsecFXTCP;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Utilities.WPF;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;

namespace MelsecFXTCP.UI
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
        MelsecFXTCPDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        MelsecFXTCPDynTagSettings thisTagSettings;

        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"Address", new SettingsControls.Address() }
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
                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new MelsecFXTCPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                    {
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    }
                    thisTagSettings.VarType = (UFUAModel.DataType)thisTag.DataType;//(DataContext as UFUAModel.UFUATag).DataType;
                    // FOGBUGZ 9836
                    thisTagSettings.ArrayDimension = thisTag.ArrayDimension;
                    LoadbaseDyn = true;
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

                //Address
                if (DynamicSettingsDict.TryGetValue("Address", out row))
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
                if (DynamicSettingsDict.TryGetValue("SwapDWord", out row))
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

                //if (LoadbaseDyn)
                //    MainStack.Children.Insert(0,baseDyn);

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
                    configuration = (from tag in new XPQuery<MelsecFXTCPDriverSettings>(ufw).AsParallel() select tag).Single();
                    
                }
                catch (InvalidOperationException ex)
                {
                    configuration = new MelsecFXTCPDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }

                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                //baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

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
                    var s = DataContext as MelsecFXTCPDynTagSettings;
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
    }
}
