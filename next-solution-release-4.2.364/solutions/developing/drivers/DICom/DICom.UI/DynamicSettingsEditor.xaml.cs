using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpo;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace DICom.UI
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
        DIComDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        DIComDynTagSettings thisTagSettings;
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
                    thisTagSettings = new DIComDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                {
                    //baseDyn.CmbLinkType.ClearValue()
                    baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                    baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                    baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;
                    baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                    baseDyn.OutputAtStartup.Visibility = Visibility.Collapsed;
                    baseDyn.OutputAtStartupText.Visibility = Visibility.Collapsed;
                    baseDyn.edtElementNumber.Visibility = Visibility.Collapsed;
                    baseDyn.TElemNumber.Visibility = Visibility.Collapsed;
                    baseDyn.TXBStateCommandVariable.Visibility = Visibility.Collapsed;
                    baseDyn.DKPStateCommandVariable.Visibility = Visibility.Collapsed;
                    baseDyn.CmbLinkType.Visibility = Visibility.Collapsed;
                    baseDyn.CmbLinkTypeText.Visibility = Visibility.Collapsed;
                    MainStack.Children.Add(baseDyn);
                }

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
                DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<DIComDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new DIComDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }

                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                //baseDyn.CmbLinkType.ItemsSource = Enum
                //                                .GetValues(typeof(LinkType))
                //                                .Cast<LinkType>()
                //                                .Where(item => item == LinkType.InputOutput);
                //baseDyn.CmbLinkType.SelectedIndex = 0;

                if (thisTag.IsMethod && !thisTagSettings.IsMethodSupported)
                {
                    MainStack.Children.Clear();
                    MainStack.HorizontalAlignment = HorizontalAlignment.Center;
                    MainStack.VerticalAlignment = VerticalAlignment.Center;

                    var NoMethod = new DriverCodeBase.UI.Controls.BaseDynamicSettingsNoMethod();
                    NoMethod.DataContext = DataContext;
                    MainStack.Children.Add(NoMethod);
                }

                if (thisTagSettings.IsObjectType)
                {
                    tbVarName.Visibility = Visibility.Collapsed;
                    txtVarName.Visibility = Visibility.Collapsed;
                    txtVarName.Text = string.Empty;
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
                    var s = DataContext as DIComDynTagSettings;
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
