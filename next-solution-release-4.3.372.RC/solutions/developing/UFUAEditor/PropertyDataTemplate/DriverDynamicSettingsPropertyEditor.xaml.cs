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
using PropertyControl.ComponentService;
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;

namespace UFUAEditor.PropertyDataTemplate
{

    /// <summary>
    /// Interaction logic for DriverDynamicSettingsPropertyEditor.xaml
    /// </summary>
    public partial class DriverDynamicSettingsPropertyEditor : UserControl
    {
        public DriverDynamicSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = btnClear.Tag as UFUAModel.DriverDynamicSettings;
            if (settings == null)
                return;

            var doc = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetContextServerDocument();
            if (doc != null)
            {
                var oldDynamic = settings.DynamicSettingsForEditing;
                var dynamicSettings = new Controls.DynamicSettings(doc, settings, allowInstalldDriver: false, settings.DriverName);
                GeneralDialogContent Dialog = new GeneralDialogContent(dynamicSettings)
                {
                    Title = Properties.Resources.DynamicSettingsTitle,
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true
                };

                var ret = (Dialog.ShowDialog() == true);
                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    if (ret)
                        settings.ApplyChanges();
                    else
                        settings.DischargeChanges();
                    PropagateChanges(settings);
                });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            var settings = btnClear.Tag as UFUAModel.DriverDynamicSettings;
            if (settings != null)
            {
                settings.DynamicSettingsForEditing = String.Empty;
                settings.ApplyChanges();
                PropagateChanges(settings);
            }
        }

        private void textBoxDynamic_LostFocus(object sender, RoutedEventArgs e)
        {
            var settings = btnClear.Tag as UFUAModel.DriverDynamicSettings;
            if (settings != null)
            {
                settings.DynamicSettingsForEditing = textBoxDynamic.Text;
                settings.ApplyChanges();
                PropagateChanges(settings);
            }
        }

        void PropagateChanges(UFUAModel.DriverDynamicSettings settings)
        {
            var selectedTags = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUATag>();
            if (selectedTags.Count > 1)
            {
                foreach (var tag in selectedTags)
                {
                    if (settings.Tag == tag)
                        continue;

                    tag.DynamicSettings = settings.Tag.DynamicSettings;
                }
            }
        }
    }
}
