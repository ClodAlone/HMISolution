using System;
using System.Windows;
using System.Windows.Controls;
using UFShortcutEditor.ComponentService;
using Utilities.WPF;
using UFShortcutSettings.ShortcutModel;
using OPCUAViewModel;

namespace UFShortcutEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewKeyCommandDefinition.xaml
    /// </summary>
    public partial class NewKeyCommandDefinition : UserControl
    {
        ShortcutEditorManagerComponent EditorComponent;
        public NewKeyCommandDefinition(ShortcutEditorManagerComponent editorComponent, UFShortcutSettings.Documents.UFShortcutDocument doc)
        {
            EditorComponent = editorComponent;
            InitializeComponent();

            Loaded += (o, e) =>
            {
                UFKeyCommandEntity a = DataContext as UFKeyCommandEntity;
                if (a != null)
                {
                    OPCUAEntityReferenceModel EnableTag = new OPCUAEntityReferenceModel() { Value = a.EnableTag };

                    textEditEnable.DataContext = EnableTag;

                    EnableTag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            a.EnableTag = EnableTag.Value;
                        }
                    };
                }

                if (CommandCtrl.Content == null)
                {
                    var commandor = EditorComponent.CommandExplorer.control;
                    EditorComponent.CommandExplorer.SetSync(commandor, true);
                    commandor.ClearValue(FrameworkElement.WidthProperty);
                    commandor.ClearValue(FrameworkElement.HeightProperty);

                    commandor.DataContext = DataContext;
                    CommandCtrl.Content = commandor;
                }
                else
                    (CommandCtrl.Content as FrameworkElement).DataContext = DataContext;
            };
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var typeKey = new TypeKey();


            SimpleWindow hw = new SimpleWindow(typeKey)
            {
                Title = Properties.Resources.TypeAnyKeyDialogTitle,
                Owner = this.FindParent<Window>()
            };
            if (hw.ShowDialog() == true)
            {
                var key = DataContext as UFKeyCommandEntity;
                key.ShortcutKey = typeKey.KeyName;
            }
            hw.Close();


            //var Dialog = new GeneralDialogContent(typeKey)
            //{
            //    Owner = this.FindParent<Window>()
            //};
            //if (Dialog.ShowDialog() == true)
            //{
            //    var key = DataContext as UFKeyCommandEntity;
            //    key.ShortcutKey = typeKey.KeyName;
            //}
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            var key = DataContext as UFKeyCommandEntity;
            if(key != null)
                key.ShortcutKey = String.Empty;
        }
    }
}
