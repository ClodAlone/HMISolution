using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace S7TIASymbolic.UI
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
        S7TIADriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        S7TIADynTagSettings thisTagSettings;
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
                    thisTagSettings = new S7TIADynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                    configuration = (from tag in new XPQuery<S7TIADriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new S7TIADriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                                      orderby s.Name
                                                      select s).ToList();
                }

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));
                CmbS7DataFormat.ItemsSource = Enum.GetValues(typeof(S7DataFormats));

                if (thisTag.IsMethod && !thisTagSettings.IsMethodSupported)
                {
                    MainStack.Children.Clear();
                    MainStack.HorizontalAlignment = HorizontalAlignment.Center;
                    MainStack.VerticalAlignment = VerticalAlignment.Center;

                    var NoMethod = new DriverCodeBase.UI.Controls.BaseDynamicSettingsNoMethod();
                    NoMethod.DataContext = DataContext;
                    MainStack.Children.Add(NoMethod);
                }
                else if ((UFUAModel.DataType)thisTag.DataType == UFUAModel.DataType.String)
                {
                    TxBStringLength.Visibility = Visibility.Visible;
                    TxStringLength.Visibility = Visibility.Visible;
                }
                else
                {
                    TxBStringLength.Visibility = Visibility.Collapsed;
                    TxStringLength.Visibility = Visibility.Collapsed;
                    TxStringLength.Text = "0";
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
                    var s = DataContext as S7TIADynTagSettings;
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
