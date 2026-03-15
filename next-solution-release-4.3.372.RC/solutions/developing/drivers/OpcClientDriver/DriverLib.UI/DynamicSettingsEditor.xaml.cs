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
using OpcClientDriver;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using OPCUAViewModel;
using UFInterfaces.Editors;
using System.Windows.Threading;
using Utilities;

namespace OpcClientDriver.UI
{
    /// <summary>
    /// Interaction logic for DynamicSettingsEditor.xaml
    /// </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        bool alreadyLoaded = false;
        bool bVisibleOnce;
        IDataLayer idl = null;
        UnitOfWork ufw = null;
        OpcClientDriverDriverSettings configuration = null;
        IDynamicSettingsEditing thisTag = null;
        OpcClientDriverDynTagSettings thisTagSettings = null;
        OPCUAEntityReference item;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;
                var baseDyn = new DriverCodeBase.UI.Controls.BaseDynamicSettings();
                bool LoadbaseDyn = false;
                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new OpcClientDriverDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    LoadbaseDyn = true;
                }

                if (!thisTag.IsMethod)
                {
                    thisTagSettings.MethodID = -1;
                }

                DataContext = null;
                DataContext = thisTagSettings;

                baseDyn.DataContext = DataContext;
                baseDyn.CmbLinkType.Visibility = Visibility.Collapsed;
                baseDyn.CmbLinkTypeErr.Visibility = Visibility.Collapsed;
                baseDyn.CmbLinkTypeText.Visibility = Visibility.Collapsed;

                baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;

                baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;

                baseDyn.OutputAtStartup.Visibility = Visibility.Collapsed;
                baseDyn.OutputAtStartupText.Visibility = Visibility.Collapsed;

                baseDyn.TMethod.Visibility = Visibility.Collapsed;
                baseDyn.CmbMethod.Visibility = Visibility.Collapsed;
                baseDyn.CmbMethodErr.Visibility = Visibility.Collapsed;

                baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;
                baseDyn.edtElementNumberErr.Visibility = Visibility.Collapsed;

                baseDyn.DKPStateCommandVariable.Visibility = Visibility.Collapsed;
                baseDyn.TXBStateCommandVariable.Visibility = Visibility.Collapsed;
                if (LoadbaseDyn)
                    MainStack.Children.Add(baseDyn);

                if (idl == null)
                {
                    Assembly a = Assembly.GetAssembly(this.GetType());
                    string DriverName = a.GetName().Name;
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    Assembly a = Assembly.GetAssembly(this.GetType());
                    string DriverName = a.GetName().Name;
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBase.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);

                

                try
                {
                    configuration = (from tag in new XPQuery<OpcClientDriverDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new OpcClientDriverDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

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
                if (alreadyLoaded && bVisibleOnce)
                {
                    alreadyLoaded = false;
                    var s = DataContext as OpcClientDriverDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void ResetItem_Click(object sender, RoutedEventArgs e)
        {
            var baseDyn = DataContext as OpcClientDriverDynTagSettings;
            baseDyn.ItemName = null;
            baseDyn.AppName = null;
            baseDyn.RelativePath = null;
            baseDyn.EndpointUrl = null;
            baseDyn.ResolvedNodeId = null;
            DataContext = null;
            DataContext = baseDyn;
        }

        private void btnItem_Click(object sender, RoutedEventArgs e)
        {
            if (item == null)
                item = new OPCUAEntityReference(null);
            
            if (item.Edit(sync:true, noDataSinks: true, noLocalServer: true) == true)
            {
                var a = DataContext as OpcClientDriverDynTagSettings;
                if (a != null)
                {
                    a.ItemName = item.HumanReadable;
                    //a.HostName = item.HostName;
                    a.AppName = item.AppName;
                    a.RelativePath = item.RelativePath;
                    a.EndpointUrl = item.EndpointUrl;
                    a.ResolvedNodeId = item.ResolvedNodeId;
                    //a.StartingAddress = item.StartingAddress;

                    //a.TypeDefinitionName = item.TypeDefinitionName;
                    //a.ResolvedNodeId = item.ResolvedNodeId;
                    //a.ResolvedStartingNodeId = item.ResolvedStartingNodeId;
                    //a.TypeDefinitionNodeId = item.TypeDefinitionNodeId;
                  
                    
                    DataContext = null;
                    DataContext = a;
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
