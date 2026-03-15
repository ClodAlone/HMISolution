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
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBaseEx.Enumerators;
using OPCUAViewModel;
using UFInterfaces.Editors;
using System.Windows.Threading;
using Utilities;
using DriverCodeBaseEx.Helpers;
using System.ComponentModel;
using Opc.Ua;

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
        OpcClientDriverDynTagSettings previousTagSettings = null;
        OPCUAEntityReference item;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"ItemName", new SettingsControls.ItemName()},
            {"AppName", new SettingsControls.AppName() },
            {"EndpointUrl", new SettingsControls.EndpointUrl() },
            {"RelativePath", new SettingsControls.RelativePath() },
            {"IECStruct", new SettingsControls.IECStruct() }
        };

        ComboBox CmbStation = null;
        Button bntResetItem = null;
        Button bntEditItem = null;
        TextBox tbRelativePath = null;

        public DynamicSettingsEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                System.Diagnostics.Trace.TraceInformation(string.Format("DynamicSettingsEditor Loaded"));
                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;
                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();
                bool LoadbaseDyn = false;

                if (thisTag == null)
                {
                    thisTag = DataContext as IDynamicSettingsEditing;
                    thisTagSettings = new OpcClientDriverDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    previousTagSettings = new OpcClientDriverDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
                    if (thisTag.DynamicSettingsForEditing != null)
                    {
                        thisTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                        previousTagSettings.TryParse(thisTag.DynamicSettingsForEditing);
                    }

                    LoadbaseDyn = true;
                }

                if (!thisTag.IsMethod)
                {
                    thisTagSettings.MethodID = -1;
                }

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
 
                // Item Name
                if (DynamicSettingsDict.TryGetValue("ItemName", out row))
                {
                    bntResetItem = (Button)row.FindName("bntResetItem");
                    bntEditItem = (Button)row.FindName("bntEditItem");

                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(Button.IsPressedProperty, typeof(Button));
                    descriptor.AddValueChanged(bntResetItem, ResetItem_Click);                    
                    descriptor.AddValueChanged(bntEditItem, btnItem_Click);

                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // App Name
                if (DynamicSettingsDict.TryGetValue("AppName", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // Endpoint URL
                if (DynamicSettingsDict.TryGetValue("EndpointUrl", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // Relative Path
                if (DynamicSettingsDict.TryGetValue("RelativePath", out row))
                {
                    tbRelativePath = (TextBox)row.FindName("edtRelativePath");
                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
                    descriptor.AddValueChanged(tbRelativePath, RelativePathChanged);
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                // IECStruct
                if(DynamicSettingsDict.TryGetValue("IECStruct", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //Station*
                if (baseSettingsCtrls.TryGetValue("Station", out row))
                {
                    row.DataContext = DataContext;
                    CmbStation = (ComboBox)row.FindName("CmbStation");
                    MainStack.Children.Add(row);
                }

                if (idl == null)
                {
                    Assembly a = Assembly.GetAssembly(this.GetType());
                    string DriverName = a.GetName().Name;
                    DriverName = DriverName.Replace(".UI", "");
                    idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(Connection, DriverName);
                }

                if (idl == null)
                {
                    Assembly a = Assembly.GetAssembly(this.GetType());
                    string DriverName = a.GetName().Name;
                    DriverName = DriverName.Replace(".UI", "");
                    MessageBox.Show(string.Format(DriverCodeBaseEx.Properties.Resources.ErrorOpeningDocument, Connection, DriverName));
                    return;
                }

                ufw = new UnitOfWork(idl);
                DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);


                try
                {
                    configuration = (from tag in new XPQuery<OpcClientDriverDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new OpcClientDriverDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
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

        private void RelativePathChanged(object sender, EventArgs e)
        {
            var baseDyn = DataContext as OpcClientDriverDynTagSettings;
            if(previousTagSettings != null)
            {
                if(previousTagSettings.ResolvedNodeId != null)
                {
                    if(previousTagSettings.ResolvedNodeId == baseDyn.ResolvedNodeId)
                    {
                        baseDyn.ResolvedNodeId = null;
                        previousTagSettings.ResolvedNodeId = null;
                    }
                    else if (baseDyn.ResolvedNodeId != null)
                    {
                        previousTagSettings.ResolvedNodeId = null;
                        previousTagSettings.ResolvedNodeId = baseDyn.ResolvedNodeId;
                    }
                }
                else if (baseDyn.ResolvedNodeId != null)
                {
                    previousTagSettings.ResolvedNodeId = baseDyn.ResolvedNodeId;
                }
            }
        }

        private void ResetItem_Click(object sender, EventArgs e)
        {
            var baseDyn = DataContext as OpcClientDriverDynTagSettings;
            baseDyn.ItemName = null;
            baseDyn.AppName = null;
            baseDyn.RelativePath = null;
            baseDyn.EndpointUrl = null;
            baseDyn.ResolvedNodeId = null;
            DataContext = null;
            DataContext = baseDyn;

            foreach (var child in MainStack.Children)
            {
                if (child as UserControl != null)
                {
                    ((UserControl)child).DataContext = null;
                    ((UserControl)child).DataContext = baseDyn;
                }
            }
        }

        private void btnItem_Click(object sender, EventArgs e)
        {
            if (item == null)
                item = new OPCUAEntityReference(null);
            
            if (item.Edit(sync:true, noDataSinks: true, noLocalServer: true) == true)
            {
                var a = DataContext as OpcClientDriverDynTagSettings;
                if (a != null)
                {
                    a.ItemName = item.HumanReadable;
                    a.AppName = item.AppName;
                    a.RelativePath = item.RelativePath;
                    a.EndpointUrl = item.EndpointUrl;
                    a.ResolvedNodeId = item.ResolvedNodeId;
                    DataContext = null;
                    DataContext = a;

                    foreach (var child in MainStack.Children)
                    {
                        if (child as UserControl != null)
                        {
                            ((UserControl)child).DataContext = null;
                            ((UserControl)child).DataContext = a;
                        }
                    }
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
            DynamicSettingsDict.Clear();

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
