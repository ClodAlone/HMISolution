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
using OmronEthernetIP;
using DriverCodeBase.Helpers;
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
                    thisTagSettings = new OmronEthernetIPDynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };//driver specific class
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
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                CmbTagFormat.ItemsSource = Enum.GetValues(typeof(TagFormats));

                // If the variable is a structure, shows the controls for setting the lengths of the elements of type string
                if(thisTag.IsObjectType)
                {
                    DKPStructStringLength.Visibility = Visibility.Visible;
                    TxStructStringLength.Visibility = Visibility.Visible;
                    TbStructStringLength.Visibility = Visibility.Visible;
                }
                else
                {
                    DKPStructStringLength.Visibility = Visibility.Collapsed;
                    TxStructStringLength.Visibility = Visibility.Collapsed;
                    TbStructStringLength.Visibility = Visibility.Collapsed;
                }

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
                    var s = DataContext as OmronEthernetIPDynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
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
