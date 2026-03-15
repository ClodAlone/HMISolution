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
using UFShortcutEditor.Controls;
using UFShortcutSettings.ShortcutModel;
using Utilities.WPF;
using Utilities;
namespace UFShortcutEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ShortcutKeyPropertyEditor.xaml
    /// </summary>
    public partial class ShortcutKeyPropertyEditor : UserControl
    {
        public ShortcutKeyPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var typeKey = new TypeKey();


            SimpleWindow hw = new SimpleWindow(typeKey) { Owner = this.FindParent<Window>() };
            var ret = (hw.ShowDialog() == true);
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (ret)
                {
                    button.Tag = typeKey.KeyName;
                    textBoxDynamic.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }

            });

            hw.Close();
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = String.Empty;
            textBoxDynamic.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
