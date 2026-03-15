////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynamicSettingsEditor.xaml.cs
//
// summary:	Implements the dynamic settings editor.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Utilities.WPF;
using DriverCodeBaseEx.Helpers;
using UFInterfaces.Editors;
using OPCUAViewModel;
using DriverCodeBaseEx.Extensions;

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for DynamicSettingsEditor.xaml. </summary>
    public partial class DynamicSettingsEditor : UserControl, IDisposable
    {
        bool bLoaded;
        bool bVisibleOnce;
        IDataLayer idl;
        UnitOfWork ufw;
        IEC60870_5_104DriverSettings configuration;
        IDynamicSettingsEditing thisTag;
        IEC60870_5_104DynTagSettings thisTagSettings;
        Dictionary<string, UserControl> baseSettingsCtrls = null;

        Dictionary<string, UserControl> DynamicSettingsDict = new Dictionary<string, UserControl>() {
            {"ASDUTypeCnt", new SettingsControls.ASDUTypeCnt()},
            {"StartAddMonBoxCnt", new SettingsControls.StartAddMonBoxCnt() },
            {"StartAddCtrlBoxCnt", new SettingsControls.StartAddCtrlBoxCnt() },
            {"CmdQualifierCnt", new SettingsControls.CmdQualifierCnt() },
            {"ParamQualifierCnt", new SettingsControls.ParamQualifierCnt() },            
            {"CmdActionCnt", new SettingsControls.CmdActionCnt() },             
            {"CotVariableNameCnt", new SettingsControls.CotVariableNameCnt() },
            {"QualityVariableNameCnt", new SettingsControls.QualityVariableNameCnt() },                                    
            {"UpdateTimeStampCnt", new SettingsControls.UpdateTimeStampCnt() },
            {"WriteTimeStampCnt", new SettingsControls.WriteTimeStampCnt() },
            {"FileNameVariableNameCnt", new SettingsControls.FileNameVariableNameCnt() }
        };

        ComboBox CmbASDUType = null;
        ComboBox CmbCmdQualifier = null;
        TextBlock TxtCmdQualifier = null;
        ComboBox CmbParamQualifier = null;
        TextBlock TxtParamQualifier = null;
        TextBlock TxtStartAddMon = null;
        TextBox StartAddMonBox = null;
        TextBlock TxtStartAddCtrl = null;
        TextBox StartAddCtrlBox = null;
        TextBlock TxBWriteTimeStamp = null;
        CheckBox CkWriteTimeStamp = null;
        CheckBox CkUpdateTimeStamp = null;
        TextBlock TxBUpdateTimeStamp = null;
        ComboBox CmbCmdAction = null;
        TextBlock TxtCmdAction = null;
                
        UserControl rowFileNameVariable;
        
        ComboBox CmbStation = null;
        ComboBox CmbLinkType = null;

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
                var baseSettings = new DriverCodeBaseEx.UI.BaseSettings();
                baseSettingsCtrls = baseSettings.GetBaseDynamicSettings();

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
                }

                DataContext = null;
                DataContext = thisTagSettings;

                UserControl row;
                //ASDUType
                if (DynamicSettingsDict.TryGetValue("ASDUTypeCnt", out row))
                {
                    CmbASDUType = (ComboBox)row.FindName("CmbASDUType");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //StartAddMonBox
                if (DynamicSettingsDict.TryGetValue("StartAddMonBoxCnt", out row))
                {
                    TxtStartAddMon = (TextBlock)row.FindName("TxtStartAddMon");
                    StartAddMonBox = (TextBox)row.FindName("StartAddMonBox");

                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }

                //StartAddCtrlBox
                if (DynamicSettingsDict.TryGetValue("StartAddCtrlBoxCnt", out row))
                {
                    TxtStartAddCtrl = (TextBlock)row.FindName("TxtStartAddCtrl");
                    StartAddCtrlBox = (TextBox)row.FindName("StartAddCtrlBox");

                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //CmdQualifier
                if (DynamicSettingsDict.TryGetValue("CmdQualifierCnt", out row))
                {
                    CmbCmdQualifier = (ComboBox)row.FindName("CmbCmdQualifier");
                    TxtCmdQualifier = (TextBlock)row.FindName("TxtCmdQualifier");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //ParamQualifier
                if (DynamicSettingsDict.TryGetValue("ParamQualifierCnt", out row))
                {
                    CmbParamQualifier = (ComboBox)row.FindName("CmbParamQualifier");
                    TxtParamQualifier = (TextBlock)row.FindName("TxtParamQualifier");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //CmdAction
                if (DynamicSettingsDict.TryGetValue("CmdActionCnt", out row))
                {
                    CmbCmdAction = (ComboBox)row.FindName("CmbCmdAction");
                    TxtCmdAction = (TextBlock)row.FindName("TxtCmdAction");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //CotVariableName
                if (DynamicSettingsDict.TryGetValue("CotVariableNameCnt", out row))
                {                    
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //QualityVariableName
                if (DynamicSettingsDict.TryGetValue("QualityVariableNameCnt", out row))
                {
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //UpdateTimeStamp
                if (DynamicSettingsDict.TryGetValue("UpdateTimeStampCnt", out row))
                {
                    CkUpdateTimeStamp = (CheckBox)row.FindName("CkUpdateTimeStamp");
                    TxBUpdateTimeStamp = (TextBlock)row.FindName("TxBUpdateTimeStamp");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //WriteTimeStamp
                if (DynamicSettingsDict.TryGetValue("WriteTimeStampCnt", out row))
                {
                    TxBWriteTimeStamp = (TextBlock)row.FindName("TxBWriteTimeStamp");
                    CkWriteTimeStamp = (CheckBox)row.FindName("CkWriteTimeStamp");
                    row.DataContext = DataContext;
                    MainStack.Children.Add(row);
                }
                //FileNameVariableName
                if (DynamicSettingsDict.TryGetValue("FileNameVariableNameCnt", out row))
                {
                    rowFileNameVariable = row;
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
                    configuration = (from tag in new XPQuery<IEC60870_5_104DriverSettings>(ufw).AsParallel() select tag).Single();

                }
                catch (InvalidOperationException ex)
                {
                    configuration = new IEC60870_5_104DriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if (configuration != null && configuration.StationSettings.Count > 0 && CmbStation != null)
                {
                    CmbStation.ItemsSource = (from s in configuration.StationSettings.AsParallel()
                                              orderby s.Name
                                              select s).ToList();
                }

                //insert option in ComboBox
                CmbASDUType.ItemsSource = Enum.GetValues(typeof(ASDUSelectableTypes));
                CmbCmdQualifier.ItemsSource = Enum.GetValues(typeof(CommandQualifiers));
                CmbParamQualifier.ItemsSource = Enum.GetValues(typeof(ParamQualifiers));
                CmbCmdAction.ItemsSource = Enum.GetValues(typeof(CommandActions));
                CmbLinkType.ItemsSource = Enum.GetValues(typeof(LinkType));

                DependencyPropertyDescriptor descriptorCmbASDUType =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptorCmbASDUType.AddValueChanged(CmbASDUType, CmbASDUType_TextChanged);
                CmbASDUType_TextChanged();

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
                rowFileNameVariable.Visibility = Visibility.Visible;
            //    textEditFileNameVariable.Visibility = System.Windows.Visibility.Visible;
            //    TxBFileNameVariable.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                rowFileNameVariable.Visibility = Visibility.Collapsed;
                //    textEditFileNameVariable.Visibility = System.Windows.Visibility.Collapsed;
                //    TxBFileNameVariable.Visibility = System.Windows.Visibility.Collapsed;
            }
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
            set { _Connection = value; }
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
