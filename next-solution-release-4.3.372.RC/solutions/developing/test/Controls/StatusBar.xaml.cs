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

namespace test
{
    /// <summary>
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl
    {
        private KeyManager _keyManager = null;
        private TimeManager _timeManager = null;

        /// <summary>
        /// Initializes a new instance of <see cref="StatusBar"/>.
        /// </summary>
        public StatusBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called once the control has loaded. We need to hook into this event rather than
        /// the constructor to set up the key manager.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this.Parent;

            while (parent != null && !(parent is Window))
                parent = LogicalTreeHelper.GetParent(parent);

            if (parent != null)
            {
                _keyManager = new KeyManager(parent as Window);
                statusBar.DataContext = _keyManager;
                _timeManager = new TimeManager(parent as Window);
                TimePortion.DataContext = _timeManager;
                _timeManager.Start();
            }

        }

        /// <summary>
        /// When the control is unloading, we want to remove the time manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timeManager != null)
            {
                _timeManager.Dispose();
            }
        }
    }
}
