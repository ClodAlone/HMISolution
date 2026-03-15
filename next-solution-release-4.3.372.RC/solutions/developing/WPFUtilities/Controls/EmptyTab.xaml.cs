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

namespace WPFUtilities.Controls
{
    /// <summary>
    /// Interaction logic for EmptyTab.xaml
    /// </summary>
    public partial class EmptyTab : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty InfoMessageProperty = DependencyProperty.Register("InfoMessage", typeof(string), typeof(EmptyTab), new UIPropertyMetadata(null));

        public string InfoMessage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(InfoMessageProperty);
            }
            set
            {
                SetValue(InfoMessageProperty, value);
            }
        }
        #endregion

        #region Constructors
        public EmptyTab()
        {
            InitializeComponent();
        }
        #endregion
    }
}
