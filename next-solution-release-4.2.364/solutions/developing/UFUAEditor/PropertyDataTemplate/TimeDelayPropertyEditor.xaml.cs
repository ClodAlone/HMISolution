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
using Mindscape.WpfElements;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TimeDelayPropertyEditor.xaml
    /// </summary>
    public partial class TimeDelayPropertyEditor : UserControl
    {
        #region Declarations

        readonly static TimeSpan[] timeSpanSource = new TimeSpan[] 
        { 
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(30),
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(6),
            TimeSpan.FromHours(12),
            TimeSpan.FromDays(1)
        };

        #endregion

        #region Constructors

        public TimeDelayPropertyEditor()
        {
            InitializeComponent();
            timeSpanControl.ItemsSource = timeSpanSource;
            SetChangeValue(timeSpanControl);
        }

        #endregion

        #region Methods

        void SetChangeValue(TimeSpanPicker picker)
        {
            if (picker.SelectedTimeSpan == TimeSpan.Zero)
                picker.Change = TimeSpan.FromSeconds(1);
            else if (picker.SelectedTimeSpan.Days > 0)
                picker.Change = TimeSpan.FromDays(1);
            else if (picker.SelectedTimeSpan.Hours > 0)
                picker.Change = TimeSpan.FromHours(1);
            else if (picker.SelectedTimeSpan.Minutes > 0)
                picker.Change = TimeSpan.FromMinutes(1);
            else if (picker.SelectedTimeSpan.Seconds > 0)
                picker.Change = TimeSpan.FromSeconds(1);
            else if (picker.SelectedTimeSpan.Milliseconds > 0)
                picker.Change = TimeSpan.FromMilliseconds(50);
        }

        #endregion

        #region Events Handler

        private void timeSpanControl_SelectedTimeSpanChanged(object sender, RoutedEventArgs e)
        {
            TimeSpanPicker picker = (TimeSpanPicker)sender;
            if (picker.SelectedTimeSpan < TimeSpan.Zero)
                picker.SelectedTimeSpan = TimeSpan.Zero;
            SetChangeValue(picker);
        }

        #endregion
    }
}
