using DocumentManager.ComponentService;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AlarmWindow.Controls
{
    /// <summary>
    /// Interaction logic for AlarmMaskEditor.xaml
    /// Dialog window for Alarm State (On, On Ack, Off, Off Ack) filter selection 
    /// </summary>
    public partial class AlarmMaskEditor : UserControl
    {
        public IDocument Document;

        public AlarmMaskEditor(bool useRuntimeSettings = false)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (useRuntimeSettings)
                {
                    FontSize = WPFUtilities.Properties.Settings.Default.DialogControlsFontSize;
                    allBtn.Width = noneBtn.Width = WPFUtilities.Properties.Settings.Default.ButtonsWidth;
                    allBtn.Height = noneBtn.Height = WPFUtilities.Properties.Settings.Default.ButtonsHeight;
                }
            };
        }

        private void Click_All(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(true);
        }

        private void Click_None(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(false);
        }

        private void SetAllCheckBoxes(bool? bSet)
        {
            (from c in stackPanel.Children.OfType<CheckBox>() select c).ToList().ForEach(check =>
            {
                if (bSet.HasValue && !bSet.Value)
                {
                    if (!check.IsChecked.HasValue || check.IsChecked == true)
                        check.IsChecked = false;
                }
                else
                    check.IsChecked = bSet;
            });
        }
    }
}
