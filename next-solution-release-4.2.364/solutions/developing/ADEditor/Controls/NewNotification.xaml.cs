using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ADEditor.Document;
using Utilities;
using Utilities.WPF;
using Ookii.Dialogs.Wpf;
using Opc.Ua;
using ADEditor.PropertyDataTemplate;
using UFInterfaces;
using DevExpress.Xpf.Grid;
using System.Text;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewNotification.xaml
    /// </summary>
    public partial class NewNotification : UserControl
    {

        #region Declarations
        ADEditorDocument Document;
        List<pluginDesc> plugList = new List<pluginDesc>();
        bool loading = true;
        
        #endregion

        public NewNotification(ADEditorDocument doc)
        {
            loading = true;
            InitializeComponent();

            Document = doc;
            comboNotificationType.ItemsSource = Enum.GetValues(typeof(ADModel.NotificationTypes));
            comboAlarmType.ItemsSource = Enum.GetValues(typeof(UFUAModel.AlarmType));
            comboConditionType.ItemsSource = Enum.GetValues(typeof(UFUAModel.ConditionType));
            comboDeviationType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DeviationType));
            textEditTimeUnit.Mask =
                textEditDelayTimeOn.Mask =
                textEditDelayTimeOff.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);

            int[] pri = {0,1,2,3,4,5,6,7,8,9};
            comboPriority.ItemsSource = pri;
            Loaded += (o, e) =>
            {
                ADModel.ADNotification a = DataContext as ADModel.ADNotification;
                if (a != null)
                {
                    if (a.NotificationType == ADModel.NotificationTypes.Server)
                    {
                        textBoxEditItem.setNotification(a);
                        textBoxEditItem.Visibility = Visibility.Visible;
                        textEditItem.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        textEditItem.setNotification(a);
                        textEditItem.Visibility = Visibility.Visible;
                        textBoxEditItem.Visibility = Visibility.Collapsed;
                    }
                        
                    ItemReferenceModel NotificationItem = new ItemReferenceModel() { Value = a.NotificationItem };
                    textEditItem.DataContext = NotificationItem;

                    NotificationItem.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (NotificationItem.Value != null)
                                a.NotificationItem = NotificationItem.Value;
                            else
                                a.NotificationItem = null;
                        }
                        
                    };
                    
                }
            };
        }

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                if (workspace == null && Document != null)
                    workspace = workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

                return workspace;
            }
        }

        private void btnAddPlugin_Click(object sender, RoutedEventArgs e)
        {
            AvailablePlugins selectplugin = new AvailablePlugins(Document.GetGeneralSettings());

            GeneralDialogContent addPluginDialog = new GeneralDialogContent(selectplugin)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "AvailablePlugins"
            };
            if (addPluginDialog.ShowDialog() == true)
            {
                var plugindesc = selectplugin.GetSelectedPluginInfo();
                var adn = DataContext as ADModel.ADNotification;
                if (adn != null && plugindesc != null)
                {
                    adn.PluginID = plugindesc.NodeId;
                    adn.PluginName = plugindesc.Name;
                }
                
                e.Handled = true;
            }
        }

        private void btnRecipient_Click(object sender, RoutedEventArgs e)
        {
            var listroles = Document.GetRoles();

            if (listroles == null || listroles.Count == 0)
            {
                MessageBox.Show(Properties.Resources.NoUsers);
                return;
            }

            
            var seluser = new SelectUser() { DataContext = listroles};

            seluser.treeListControl.SelectionMode = MultiSelectMode.MultipleRow;
            GeneralDialogContent Dialog = new GeneralDialogContent(seluser)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "SelectUser"
            };
            if (Dialog.ShowDialog() == true)
            {
                ADModel.ADNotification a = DataContext as ADModel.ADNotification;
                if (a != null)
                {
                    a.Recipient = string.Empty;
                    a.RecipientID = string.Empty;
                    a.MultiRecipientID = string.Empty;
                }
                StringBuilder str = new StringBuilder();
                StringBuilder strRec = new StringBuilder();
                foreach (TreeListNode tvm in seluser.treeListControl.GetSelectedNodes())
                {
                    if (tvm != null && a != null)
                    {
                        UFUserModel.UFUser us = null;
                        UFUserModel.UFRole el = tvm.Tag as UFUserModel.UFRole;
                        if (el == null)
                            us = tvm.Tag as UFUserModel.UFUser;

                        if (el != null || us != null)
                        {
                            //                           ADModel.ADNotification a = DataContext as ADModel.ADNotification;
                            //                           if (a != null)
                            //                           {
                            //                               a.RecipientID = null;
                            //                                a.Recipient = string.Empty;
                            str.Append("g=");
                            str.Append((el != null ? el.NodeId : us.NodeId).ToString());
                            str.Append(ADModel.ADNotification.delimiter);
                            strRec.Append(el != null ? el.Name : us.Name);
                            strRec.Append(ADModel.ADNotification.delimiter);
                            //                           }
                        }
                    }
                }
                a.MultiRecipientID = str.ToString();
                a.Recipient = strRec.ToString();
            }
        }

        private void comboAlarmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlarmTypeVisibility();
        }

        void AlarmTypeVisibility()
        {
            var tag = DataContext as ADModel.ADNotification;
            if (tag.NotificationType == ADModel.NotificationTypes.Server)
                return;
            if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveLevel || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveLevel)
            {
                textDeviationType.Visibility = Visibility.Collapsed;
                comboDeviationType.Visibility = Visibility.Collapsed;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textBlockSetPointValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLimit.Visibility = Visibility.Collapsed;
                textBlockHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighHighLimit.Visibility = Visibility.Collapsed;

                textBlockLowLowLevel.Visibility = Visibility.Visible;
                textBlockLowLevel.Visibility = Visibility.Visible;
                textBlockHighLevel.Visibility = Visibility.Visible;
                textBlockHighHighLevel.Visibility = Visibility.Visible;

                checkLowLowLimit.Visibility = Visibility.Visible;
                checkLowLimit.Visibility = Visibility.Visible;
                checkHighLimit.Visibility = Visibility.Visible;
                checkHighHighLimit.Visibility = Visibility.Visible;

                textLowLowLimit.Visibility = Visibility.Visible;
                textLowLimit.Visibility = Visibility.Visible;
                textHighLimit.Visibility = Visibility.Visible;
                textHighHighLimit.Visibility = Visibility.Visible;

                txtBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;
                txtBlockDelayTimeOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;

            }
            else if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveDeviation || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveDeviation)
            {
                textDeviationType.Visibility = Visibility.Visible;
                comboDeviationType.Visibility = Visibility.Visible;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textBlockSetPointValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Visible;
                textBlockLowLimit.Visibility = Visibility.Visible;
                textBlockHighLimit.Visibility = Visibility.Visible;
                textBlockHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                checkLowLowLimit.Visibility = Visibility.Visible;
                checkLowLimit.Visibility = Visibility.Visible;
                checkHighLimit.Visibility = Visibility.Visible;
                checkHighHighLimit.Visibility = Visibility.Visible;

                textLowLowLimit.Visibility = Visibility.Visible;
                textLowLimit.Visibility = Visibility.Visible;
                textHighLimit.Visibility = Visibility.Visible;
                textHighHighLimit.Visibility = Visibility.Visible;

                txtBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;
                txtBlockDelayTimeOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;

                CheckCurrencySymbol();
            }
            else if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange)
            {
                textDeviationType.Visibility = Visibility.Visible;
                comboDeviationType.Visibility = Visibility.Visible;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;
                textBlockSetPointValue.Visibility = Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Visible;
                textBlockLowLimit.Visibility = Visibility.Visible;
                textBlockHighLimit.Visibility = Visibility.Visible;
                textBlockHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                checkLowLowLimit.Visibility = Visibility.Visible;
                checkLowLimit.Visibility = Visibility.Visible;
                checkHighLimit.Visibility = Visibility.Visible;
                checkHighHighLimit.Visibility = Visibility.Visible;

                textLowLowLimit.Visibility = Visibility.Visible;
                textLowLimit.Visibility = Visibility.Visible;
                textHighLimit.Visibility = Visibility.Visible;
                textHighHighLimit.Visibility = Visibility.Visible;

                txtBlockTimeUnit.Visibility = Visibility.Visible;
                textEditTimeUnit.Visibility = Visibility.Visible;
                txtBlockDelayTimeOn.Visibility = Visibility.Collapsed;
                textEditDelayTimeOn.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOff.Visibility = Visibility.Collapsed;
                textEditDelayTimeOff.Visibility = Visibility.Collapsed;

                CheckCurrencySymbol();
            }
            else if (tag.AlarmType == UFUAModel.AlarmType.TripAlarm)
            {
                textDeviationType.Visibility = Visibility.Collapsed;
                comboDeviationType.Visibility = Visibility.Collapsed;

                textConditionType.Visibility = Visibility.Visible;
                comboConditionType.Visibility = Visibility.Visible;
                textBlockActivationValue.Visibility = Visibility.Visible;
                textActivationValue.Visibility = Visibility.Visible;
                textBlockSetPointValue.Visibility = Visibility.Collapsed;

                checkLowLowLimit.Visibility = Visibility.Collapsed;
                checkLowLimit.Visibility = Visibility.Collapsed;
                checkHighLimit.Visibility = Visibility.Collapsed;
                checkHighHighLimit.Visibility = Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLimit.Visibility = Visibility.Collapsed;
                textBlockHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighHighLimit.Visibility = Visibility.Collapsed;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                textLowLowLimit.Visibility = Visibility.Collapsed;
                textLowLimit.Visibility = Visibility.Collapsed;
                textHighLimit.Visibility = Visibility.Collapsed;
                textHighHighLimit.Visibility = Visibility.Collapsed;

                txtBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;
                txtBlockDelayTimeOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;
            }

            textEditDelayTimeOn.InvalidateProperty(WPFUtilities.Controls.TimeSpanEdit.TimeSpanProperty);
            textEditDelayTimeOff.InvalidateProperty(WPFUtilities.Controls.TimeSpanEdit.TimeSpanProperty);
        }

        private void comboDeviationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCurrencySymbol();
        }

        void CheckCurrencySymbol()
        {
            var tag = DataContext as ADModel.ADNotification;

            if (tag.AlarmType != UFUAModel.AlarmType.ExclusiveDeviation &&
                tag.AlarmType != UFUAModel.AlarmType.NonExclusiveDeviation &&
                tag.AlarmType != UFUAModel.AlarmType.ExclusiveRateOfChange &&
                tag.AlarmType != UFUAModel.AlarmType.NonExclusiveRateOfChange)
                return;

            bool percent = tag.DeviationType == UFUAModel.DeviationType.PercentOfEURange ||
                            tag.DeviationType == UFUAModel.DeviationType.PercentOfRange ||
                            tag.DeviationType == UFUAModel.DeviationType.PercentOfValue;

            textHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textHighHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textLowPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textLowLowPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            textHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textHighHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textLowLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textLowLowLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void btnAddAttach_Click(object sender, RoutedEventArgs e)
        {
            ADModel.ADNotification a = DataContext as ADModel.ADNotification;
            if (a != null)
            {
                string filter = string.Empty;
                var fileType = new VistaOpenFileDialog();

                if (fileType.ShowDialog() == true)
                {
                    String fileName = fileType.FileName;
                    if (a.Attachments != null && a.Attachments.Length > 0)
                    {
                        a.Attachments += string.Format(";{0}", fileName);
                    }
                    else
                        a.Attachments = fileName;
                }
            }
        }

        private void btnRecipient_Clear(object sender, RoutedEventArgs e)
        {
            ADModel.ADNotification a = DataContext as ADModel.ADNotification;
            if (a != null)
            {
                a.Recipient = String.Empty;
                a.RecipientID = String.Empty;
            }
        }

        void ClearItem()
        {
            ADModel.ADNotification a = DataContext as ADModel.ADNotification;
            if (a != null)
            {
                //a.AlarmName = String.Empty;
                //a.NotificationItem = String.Empty;
                try
                {
                    ItemReferenceModel NotificationItem = new ItemReferenceModel() { Value = String.Empty };
                    textEditItem.DataContext = NotificationItem;
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnAddPlugin_Clear(object sender, RoutedEventArgs e)
        {
            var adn = DataContext as ADModel.ADNotification;
            if (adn != null)
            {
                adn.PluginID = NodeId.Null;
                adn.PluginName = string.Empty;
            }
        }

        private void btnAddAttach_Clear(object sender, RoutedEventArgs e)
        {
            ADModel.ADNotification a = DataContext as ADModel.ADNotification;
            if (a != null)
                a.Attachments = string.Empty;
        }
        bool NotificationTypeVisibility()
        {
            bool ret = false;
            var tag = DataContext as ADModel.ADNotification;
            if(tag.NotificationType == ADModel.NotificationTypes.Local)
            {
                ret = true;
                txtAlarmType.Visibility = Visibility.Visible;
                comboAlarmType.Visibility = Visibility.Visible;
                textConditionType.Visibility = Visibility.Visible;
                comboConditionType.Visibility = Visibility.Visible;
                textBlockActivationValue.Visibility = Visibility.Visible;
                textBlockSetPointValue.Visibility = Visibility.Visible;
                textActivationValue.Visibility = Visibility.Visible;
                textDeviationType.Visibility = Visibility.Visible;
                comboDeviationType.Visibility = Visibility.Visible;
                textBlockHighHighLimit.Visibility = Visibility.Visible;
                textBlockHighHighLevel.Visibility = Visibility.Visible;
                gridHighHighLimit.Visibility = Visibility.Visible;
                textBlockHighLimit.Visibility = Visibility.Visible;
                textBlockHighLevel.Visibility = Visibility.Visible;
                gridHighLimit.Visibility = Visibility.Visible;
                textBlockLowLimit.Visibility = Visibility.Visible;
                textBlockLowLevel.Visibility = Visibility.Visible;
                gridLowLimit.Visibility = Visibility.Visible;
                textBlockLowLowLimit.Visibility = Visibility.Visible;
                textBlockLowLowLevel.Visibility = Visibility.Visible;
                gridLowLowLimit.Visibility = Visibility.Visible;
                textBlockSeverity.Visibility = Visibility.Visible;
                textSeverity.Visibility = Visibility.Visible;
                txtBlockTimeUnit.Visibility = Visibility.Visible;
                textEditTimeUnit.Visibility = Visibility.Visible;
                txtBlockDelayTimeOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;
                txtBlockDelayTimeOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;
                textEditAlarmName.Visibility = Visibility.Collapsed;
                txtAlarmName.Visibility = Visibility.Collapsed;
                textBlockEnableON.Visibility = Visibility.Visible;
                checkEnableON.Visibility = Visibility.Visible;
                textBlockEnableOFF.Visibility = Visibility.Visible;
                checkEnableOFF.Visibility = Visibility.Visible;
                textBlockEnableACK.Visibility = Visibility.Collapsed;
                checkEnableACK.Visibility = Visibility.Collapsed;
                textBlockEnableCONFIRMED.Visibility = Visibility.Collapsed;
                checkEnableCONFIRMED.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtAlarmType.Visibility = Visibility.Collapsed;
                comboAlarmType.Visibility = Visibility.Collapsed;
                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textBlockSetPointValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;
                textDeviationType.Visibility = Visibility.Collapsed;
                comboDeviationType.Visibility = Visibility.Collapsed;
                textBlockHighHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;
                gridHighHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                gridHighLimit.Visibility = Visibility.Collapsed;
                textBlockLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                gridLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                gridLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockSeverity.Visibility = Visibility.Collapsed;
                textSeverity.Visibility = Visibility.Collapsed;
                txtBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOn.Visibility = Visibility.Collapsed;
                textEditDelayTimeOn.Visibility = Visibility.Collapsed;
                txtBlockDelayTimeOff.Visibility = Visibility.Collapsed;
                textEditDelayTimeOff.Visibility = Visibility.Collapsed;
                textEditAlarmName.Visibility = Visibility.Visible;
                txtAlarmName.Visibility = Visibility.Visible;
                textBlockEnableON.Visibility = Visibility.Visible;
                checkEnableON.Visibility = Visibility.Visible;
                textBlockEnableOFF.Visibility = Visibility.Visible;
                checkEnableOFF.Visibility = Visibility.Visible;
                textBlockEnableACK.Visibility = Visibility.Visible;
                checkEnableACK.Visibility = Visibility.Visible;
                textBlockEnableCONFIRMED.Visibility = Visibility.Visible;
                checkEnableCONFIRMED.Visibility = Visibility.Visible;
            }
            return ret;
        }
        private void comboNotificationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(NotificationTypeVisibility())
                AlarmTypeVisibility();

            ADModel.ADNotification a = DataContext as ADModel.ADNotification;
            if (e.AddedItems.Count > 0)
            {
                var selected = (ADModel.NotificationTypes)e.AddedItems[0];
                if (selected == ADModel.NotificationTypes.Local)
                {
                    textEditItem.setNotification(a);
                    textEditItem.Visibility = Visibility.Visible;
                    textBoxEditItem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textBoxEditItem.setNotification(a);
                    textEditItem.Visibility = Visibility.Collapsed;
                    textBoxEditItem.Visibility = Visibility.Visible;
                }
            }
            

            if (!loading)
                ClearItem();
            else
                loading = false;
                
        }
    }
}
