////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynamicSettingsEditor.xaml.cs
//
// summary:	Implements the dynamic settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

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
using System.ComponentModel;
using IEC60870_5_104;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using Utilities.WPF;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;
using OPCUAViewModel;
using System.Windows.Threading;
using Utilities;
using DriverCodeBase.Extensions;

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {

        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;
        /// <summary>   true to visible once. </summary>
        bool bVisibleOnce;
        /// <summary>   The idl. </summary>
        IDataLayer idl;
        /// <summary>   The ufw. </summary>
        UnitOfWork ufw;
        /// <summary>   The configuration. </summary>
        IEC60870_5_104DriverSettings configuration;
        /// <summary>   this tag. </summary>
        IDynamicSettingsEditing thisTag;
        /// <summary>   this tag settings. </summary>
        IEC60870_5_104DynTagSettings thisTagSettings;
        OPCUAEntityReference CotVariable;
        OPCUAEntityReference QualityVariable;
        OPCUAEntityReference FileNameVariable;

        /// <summary>   Default constructor. </summary>
        public DynamicSettingsEditor()
        {
            InitializeComponent();

            //executed at loaded
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
                    thisTagSettings = new IEC60870_5_104DynTagSettings() { IsMethod = thisTag.IsMethod, IsObjectType = thisTag.IsObjectType };
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
                baseDyn.TElemNumber.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.edtElementNumber.Visibility = System.Windows.Visibility.Collapsed;
                DataContext = null;
                DataContext = thisTagSettings;
                baseDyn.DataContext = DataContext;

                //insert base DynamicSettings setup
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
                    configuration = (from tag in new XPQuery<IEC60870_5_104DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new IEC60870_5_104DriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0)
                {
                    //insert Station in ComboBox baseDyn.CmbStation
                    baseDyn.CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //insert option in ComboBox
                CmbASDUType.ItemsSource = Enum.GetValues(typeof(ASDUSelectableTypes));
                CmbCmdQualifier.ItemsSource = Enum.GetValues(typeof(CommandQualifiers));
                CmbParamQualifier.ItemsSource = Enum.GetValues(typeof(ParamQualifiers));
                CmbCmdAction.ItemsSource = Enum.GetValues(typeof(CommandActions));
                baseDyn.CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));
                baseDyn.CmbMethod.ItemsSource = Enum.GetValues(typeof(DriverMethods));

                DependencyPropertyDescriptor descriptorCmbASDUType =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptorCmbASDUType.AddValueChanged(CmbASDUType, CmbASDUType_TextChanged);
                CmbASDUType_TextChanged();

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
                if (!IEC60870_5_104Protocol.ASDUTypesRequiresCQ(thisTagSettings))
                    thisTagSettings.CmdQualifier = CommandQualifiers.NotUsedCQ;
                if (!IEC60870_5_104Protocol.ASDUTypesRequiresPQ(thisTagSettings))
                    thisTagSettings.ParamQualifier = ParamQualifiers.NotUsedPQ;
                if (!IEC60870_5_104Protocol.ASDUTypesRequiresCA(thisTagSettings))
                    thisTagSettings.CmdAction = CommandActions.NotUsedCA;
                if (bLoaded && bVisibleOnce)
                {
                    bLoaded = false;
                    var s = DataContext as IEC60870_5_104DynTagSettings;
                    if (s != null)
                    {
                        thisTag.DynamicSettingsForEditing = s.ToString();
                    }
                }
            };
        }

        private void CmbASDUType_TextChanged(object sender, EventArgs e)
        {
            CmbASDUType_TextChanged();
        }

        private void CmbASDUType_TextChanged()
        {
            if (IEC60870_5_104Protocol.ASDUTypesRequiresCQ(thisTagSettings))
            {
                CmbCmdQualifier.Visibility = System.Windows.Visibility.Visible;
                TxtCmdQualifier.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CmbCmdQualifier.Visibility = System.Windows.Visibility.Collapsed;
                TxtCmdQualifier.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (IEC60870_5_104Protocol.ASDUTypesRequiresPQ(thisTagSettings))
            {
                CmbParamQualifier.Visibility = System.Windows.Visibility.Visible;
                TxtParamQualifier.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CmbParamQualifier.Visibility = System.Windows.Visibility.Collapsed;
                TxtParamQualifier.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (IEC60870_5_104Protocol.ASDUTypesRequiresCA(thisTagSettings))
            {
                CmbCmdAction.Visibility = System.Windows.Visibility.Visible;
                TxtCmdAction.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                CmbCmdAction.Visibility = System.Windows.Visibility.Collapsed;
                TxtCmdAction.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (IEC60870_5_104Protocol.isASDUTypesWritable(thisTagSettings))
            {
                StartAddCtrlBox.Visibility = System.Windows.Visibility.Visible;
                TxtStartAddCtrl.Visibility = System.Windows.Visibility.Visible;
                CkWriteTimeStamp.Visibility = System.Windows.Visibility.Visible;
                TxBWriteTimeStamp.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StartAddCtrlBox.Visibility = System.Windows.Visibility.Collapsed;
                TxtStartAddCtrl.Visibility = System.Windows.Visibility.Collapsed;
                CkWriteTimeStamp.Visibility = System.Windows.Visibility.Collapsed;
                TxBWriteTimeStamp.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (IEC60870_5_104Protocol.isASDUTypesReadable(thisTagSettings))
            {
                StartAddMonBox.Visibility = System.Windows.Visibility.Visible;
                TxtStartAddMon.Visibility = System.Windows.Visibility.Visible;
                CkUpdateTimeStamp.Visibility = System.Windows.Visibility.Visible;
                TxBUpdateTimeStamp.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StartAddMonBox.Visibility = System.Windows.Visibility.Collapsed;
                TxtStartAddMon.Visibility = System.Windows.Visibility.Collapsed;
                CkUpdateTimeStamp.Visibility = System.Windows.Visibility.Collapsed;
                TxBUpdateTimeStamp.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (IEC60870_5_104Protocol.ASDUTypesRequiresFileName(thisTagSettings))
            {
                textEditFileNameVariable.Visibility = System.Windows.Visibility.Visible;
                TxBFileNameVariable.Visibility = System.Windows.Visibility.Visible;
                btnFileVariableNameSet.Visibility = System.Windows.Visibility.Visible;
                btnFileVariableNameReset.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                textEditFileNameVariable.Visibility = System.Windows.Visibility.Collapsed;
                TxBFileNameVariable.Visibility = System.Windows.Visibility.Collapsed;
                btnFileVariableNameSet.Visibility = System.Windows.Visibility.Collapsed;
                btnFileVariableNameReset.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SetCotVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            if (nDC != null)
            {
                var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.CotVariableName, nDC.CotVariableId);
                var tag = tagEntityReference.Edit(this.FindParent<Window>());
                if (tag != null)
                {
                    nDC.CotVariableName = tag.ToString();
                    nDC.CotVariableId = tag.NodeId.ToString();
                }
            }
        }

        private void ResetCotVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            nDC.CotVariableName = null;
            nDC.CotVariableId = null;
        }

        private void SetQualityVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            if (nDC != null)
            {
                var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.QualityVariableName, nDC.QualityVariableId);
                var tag = tagEntityReference.Edit(this.FindParent<Window>());
                if (tag != null)
                {
                    nDC.QualityVariableName = tag.ToString();
                    nDC.QualityVariableId = tag.NodeId.ToString();
                }
            }
        }

        private void ResetQualityVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            nDC.QualityVariableName = null;
            nDC.QualityVariableId = null;
        }

        private void SetFileNameVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            if (nDC != null)
            {
                var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.FileNameVariableName, nDC.FileNameVariableId);
                var tag = tagEntityReference.Edit(this.FindParent<Window>());
                if (tag != null)
                {
                    nDC.FileNameVariableName = tag.ToString();
                    nDC.FileNameVariableId = tag.NodeId.ToString();
                }
            }
        }

        private void ResetFileNameVariableTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as IEC60870_5_104DynTagSettings;
            nDC.FileNameVariableName = null;
            nDC.FileNameVariableId = null;
        }

        /// <summary>   The connection. </summary>
        private string _Connection;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the connection. </summary>
        ///
        /// <value> The connection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Connection
        {
            get { return _Connection; }
            set
            {
                _Connection = value;
            }
        }


        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
