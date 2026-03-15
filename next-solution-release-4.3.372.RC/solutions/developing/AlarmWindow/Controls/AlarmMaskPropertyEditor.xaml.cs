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
using Utilities.WPF;
using Utilities;

namespace AlarmWindow.Controls
{
    /// <summary>
    /// Alarm Mask Model Class
    /// </summary>
    public class AlarmMaskModel
    {
        public int? Value { get; set; }
    }

    /// <summary>
    /// Interaction logic for AlarmMaskPropertyEditor.xaml
    /// User Control for Alarm Mask Filter interface in property toolbar
    /// </summary>
    public partial class AlarmMaskPropertyEditor : UserControl
    {
        public AlarmMaskPropertyEditor()
        {
            InitializeComponent();
        }

        private void OpenAlarmMaskDialogButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int? value = 0;
            if (button.Tag is int)
            {
                try
                {
                    value = (int)button.Tag;
                }
                catch (Exception ex)
                {

                }
            }

            if (value == -1)
            {
                value = 0;
            }

            var mdl = new AlarmMaskModel() { Value = value };
            var editor = new AlarmMaskEditor() { DataContext = mdl };
            var ColorDialog = new GeneralDialogContent(editor)
            {
                Owner = button.FindParent<Window>(),
                Title = Properties.Resources.AlarmMaskEditor
            };
            if (ColorDialog.ShowDialog() != true)
            {
                return;
            }
            if (mdl.Value.HasValue)
                button.Tag = mdl.Value;
            else
                button.Tag = -1;
        }
    }
}
