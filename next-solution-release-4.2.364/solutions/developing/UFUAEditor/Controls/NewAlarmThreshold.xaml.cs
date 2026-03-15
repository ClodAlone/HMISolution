using System;
using System.Collections.Generic;
using System.Globalization;
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
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using UFUAEditor.ComponentService;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using UFUAEditor.Alarms;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewAlarmDefinition.xaml
    /// </summary>
    public partial class NewAlarmThreshold : UserControl
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        #endregion

        #region Constructors
        public NewAlarmThreshold(UFUAServerDocument doc)
        {
            InitializeComponent();
            Document = doc;

            comboBeep.ItemsSource = Enum.GetValues(typeof(UFUAModel.ThreeStateType));
            comboEnableQualityGood.ItemsSource = Enum.GetValues(typeof(UFUAModel.ThreeStateType));

            Loaded += (s, e) =>
            {
                var desc = (DataContext as UFUAModel.UFUAAlarmThreshold);
                editAlarmPrototype.IsEnabled = desc.UFUAAlarmDefinitionRef != null;

                templateCommandsOn.DataContext = new AlarmCommandsViewModel(desc, AlarmCommandsEventType.CommandsOn);
                templateCommandsOff.DataContext = new AlarmCommandsViewModel(desc, AlarmCommandsEventType.CommandsOff);
                templateCommandsAck.DataContext = new AlarmCommandsViewModel(desc, AlarmCommandsEventType.CommandsAck);
                templateCommandsReset.DataContext = new AlarmCommandsViewModel(desc, AlarmCommandsEventType.CommandsReset);
                templateCommandsDbClick.DataContext = new AlarmCommandsViewModel(desc, AlarmCommandsEventType.CommandsDbClick);
                if(desc != null)
                {
                    TagEntityReferenceModel ActivationLowTag = new TagEntityReferenceModel() { Value = desc.ActivationLowValueTag };
                    textActivationLowValueThresholdTag.DataContext = ActivationLowTag;
                    ActivationLowTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (ActivationLowTag.Value != null)
                                desc.ActivationLowValueTag = ActivationLowTag.Value;
                            else
                                desc.ActivationLowValueTag = null;
                        }
                    };

                    TagEntityReferenceModel ActivationTag = new TagEntityReferenceModel() { Value = desc.ActivationValueTag };
                    textActivationValueThresholdTag.DataContext = ActivationTag;
                    ActivationTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (ActivationTag.Value != null)
                                desc.ActivationValueTag = ActivationTag.Value;
                            else
                                desc.ActivationValueTag = null;
                        }
                    };

                    TagEntityReferenceModel EnableTag = new TagEntityReferenceModel() { Value = desc.EnableTag };
                    textEnabledThresholdTag.DataContext = EnableTag;
                    EnableTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (EnableTag.Value != null)
                                desc.EnableTag = EnableTag.Value;
                            else
                                desc.EnableTag = null;
                        }
                    };

                    TagEntityReferenceModel HighHighLimitTag = new TagEntityReferenceModel() { Value = desc.HighHighLimitTag };
                    textHighHighLimitThresholdTag.DataContext = HighHighLimitTag;
                    HighHighLimitTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (HighHighLimitTag.Value != null)
                                desc.HighHighLimitTag = HighHighLimitTag.Value;
                            else
                                desc.HighHighLimitTag = null;
                        }
                    };

                    TagEntityReferenceModel HighLimitTag = new TagEntityReferenceModel() { Value = desc.HighLimitTag };
                    textHighLimitThresholdTag.DataContext = HighLimitTag;
                    HighLimitTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (HighLimitTag.Value != null)
                                desc.HighLimitTag = HighLimitTag.Value;
                            else
                                desc.HighLimitTag = null;
                        }
                    };

                    TagEntityReferenceModel LowLimitTag = new TagEntityReferenceModel() { Value = desc.LowLimitTag };
                    textLowLimitThresholdTag.DataContext = LowLimitTag;
                    LowLimitTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (LowLimitTag.Value != null)
                                desc.LowLimitTag = LowLimitTag.Value;
                            else
                                desc.LowLimitTag = null;
                        }
                    };

                    TagEntityReferenceModel LowLowLimitTag = new TagEntityReferenceModel() { Value = desc.LowLowLimitTag };
                    textLowLowLimitThresholdTag.DataContext = LowLowLimitTag;
                    LowLowLimitTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (LowLowLimitTag.Value != null)
                                desc.LowLowLimitTag = LowLowLimitTag.Value;
                            else
                                desc.LowLowLimitTag = null;
                        }
                    };
                }
            };
        }
        #endregion

        #region Properties
        public bool IsCommandsSupported
        {
            get
            {
                return UFUAEditorManagerComponent.ufuaEditorManagerComponent.CommandExplorer != null;

            }
        }
        #endregion

        #region Commands
        private void EditAlarmPrototype_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var desc = (DataContext as UFUAModel.UFUAAlarmThreshold);
            if (desc.UFUAAlarmDefinitionRef != null)
            {
                var alr = Document.FindAlarmDefinitionByNodeId(desc.UFUAAlarmDefinitionRef.NodeId);
                if (alr != null)
                {
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var newhsControl = new NewAlarmDefinition(Document)
                        {
                            DataContext = uow.GetNestedObject(alr)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(newhsControl)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditAlarmDefinition"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            uow.CommitChanges();
                        }
                    }
                }
            }
        }

        private void textEditName_Click(object sender, RoutedEventArgs e)
        {
            var stringEditor = Document.EditorManagerComponent.StringEditor.GetStringEditor(Document.Parent);
            stringEditor.DataContext = textEditName.Text;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "SelectStringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            textEditName.Text = stringEditor.DataContext as String;
            textEditName.Focus();
            textEditName.SelectAll();
        }

        private void textEditName_Clear(object sender, RoutedEventArgs e)
        {
            textEditName.Text = String.Empty;
        }

        private void AliasTags_Click_Edit(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            var xpTags = button.Tag as DevExpress.Xpo.XPCollection<UFUAModel.XPTagEntityReference>;
            if (xpTags != null)
            {
                var tags = (from c in xpTags orderby c.Oid ascending select c.TagEntity).ToArray();
                var control = new ListTagEntityReferenceEditor(tags);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.ListTagEntityReferenceTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AliasTagsEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.Workspace.ContextDocument as UFUAServerDocument;
                    if (doc != null)
                    {
                        var previousLenght = tags == null ? 0 : tags.Length;
                        bool updateNumItems = (previousLenght != control.CurrentTags.Length);

                        var session = xpTags.Session;
                        var alarm = (from tag in new DevExpress.Xpo.XPQuery<UFUAModel.UFUAAlarmThreshold>(session, true).AsParallel()
                                     where tag.AliasTags == xpTags
                                     select tag).SingleOrDefault();

                        while (alarm.AliasTags.Count > 0)
                            alarm.AliasTags[0].Delete();

                        foreach (var tag in control.CurrentTags)
                            alarm.AliasTags.Add(new UFUAModel.XPTagEntityReference(session) { TagEntity = tag });

                        if (updateNumItems)
                            uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                    }
                }
            }
        }

        private void AliasTags_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            var tags = button.Tag as DevExpress.Xpo.XPCollection<UFUAModel.XPTagEntityReference>;

            var alarmThresholds = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUAAlarmThreshold>();
            foreach (var alarm in alarmThresholds)
            {
                while (alarm.AliasTags.Count > 0)
                    alarm.AliasTags[0].Delete();
            }
            uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }

        private void textEditExpression_Clear(object sender, RoutedEventArgs e)
        {
            textEditExpression.Text = string.Empty;
        }

        private void textEditExpression_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            string expression = button.Tag as string;

            // var variables = (from c in UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetFlatListTags(Document).AsParallel() orderby c select c);
            var varEditor = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetRuntimeAddressSpaceControl(Document);
            if (varEditor == null)
                return;

            WPFUtilities.PropertyDataTemplate.ExpressonEditor editor = new WPFUtilities.PropertyDataTemplate.ExpressonEditor(varEditor, expression);
            var Dialog = new GeneralDialogContent(editor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.ExpressionEditor,
                HelpLink = "ExpressionEditor"
            };

            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                textEditExpression.Text = editor.model.Expression;
            } 
        }
        #endregion
    }
}
