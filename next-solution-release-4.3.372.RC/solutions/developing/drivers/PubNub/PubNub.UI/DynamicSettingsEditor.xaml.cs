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
using PubNub;
using DriverCodeBase;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using Utilities.WPF;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace PubNub.UI
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
        PubNubDriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        PubNubDynTagSettings thisTagSettings;
        public DynamicSettingsEditor()
        {
            InitializeComponent();

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
                    thisTagSettings = new PubNubDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                DriverCodeBase.CommunicationDriver.UpdateDriverSchema(ufw);
                try
                {
                    configuration = (from tag in new XPQuery<PubNubDriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new PubNubDriverSettings(ufw) ;
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //swap checkboxes have been hidden, because the driver handles strings.
                baseDyn.CheckSwapBytes.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapBytesText.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWords.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsErr.Visibility = Visibility.Collapsed;
                baseDyn.CheckSwapWordsText.Visibility = Visibility.Collapsed;

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

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
                    var s = DataContext as PubNubDynTagSettings;
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
