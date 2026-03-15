using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for OPCUAEntityReferenceListPropertyEditor.xaml
    /// </summary>
    public partial class OPCUAEntityReferenceListPropertyEditor : UserControl
    {
        public OPCUAEntityReferenceListPropertyEditor()
        {
            InitializeComponent();
        }
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var listUri = button.Tag as OPCUAEntityReferenceList;
            if (listUri != null)
            {
                var control = new ListOPCUAEntityReferenceEditor(listUri);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resource.TagList,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "TagList"
                };
                if (Dialog.ShowDialog() == true)
                {
                    bool update = (listUri.Count != control.CurrentUri.Count);

                    if (update)
                        uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                    button.Tag = new OPCUAEntityReferenceList(control.CurrentUri);
                }
            }
        }
    }
}
