using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFUAEditor.ComponentService;
using UFUAEditor.Controls;
using Utilities;
using Utilities.WPF;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListTagEntityReferencePropertyEditor.xaml
    /// </summary>
    public partial class ListTagEntityReferencePropertyEditor : UserControl
    {
        public ListTagEntityReferencePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            var xpTags = button.Tag as XPCollection<UFUAModel.XPTagEntityReference>;
            if (xpTags != null)
            {
                var tags = (from c in xpTags orderby c.Oid ascending select c.TagEntity).ToArray();
                var control = new ListTagEntityReferenceEditor(tags);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.ListTagEntityReferenceTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "TagListEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var previousLenght = tags == null ? 0 : tags.Length;
                    bool updateNumItems = (previousLenght != control.CurrentTags.Length);

                    var alarmThresholds = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUAAlarmThreshold>();
                    foreach (var alarm in alarmThresholds)
                    {
                        while (alarm.AliasTags.Count > 0)
                            alarm.AliasTags[0].Delete();

                        foreach (var tag in control.CurrentTags)
                            alarm.AliasTags.Add(new UFUAModel.XPTagEntityReference(xpTags.Session) { TagEntity = tag });
                    }
                    if (updateNumItems)
                        uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            var tags = button.Tag as XPCollection<UFUAModel.XPTagEntityReference>;

            var alarmThresholds = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUAAlarmThreshold>();
            foreach (var alarm in alarmThresholds)
            {
                while (alarm.AliasTags.Count > 0)
                    alarm.AliasTags[0].Delete();
            }
            uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }
    }
}
