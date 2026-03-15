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

namespace ScreenManager.Adorners
{
    /// <summary>
    /// Interaction logic for EditingAdornerControl.xaml
    /// </summary>
    public partial class ManipulationAdornerControl : UserControl
    {
        public ManipulationAdornerControl(UIElement e)
        {
            InitializeComponent();
            AdornedControl = e;
        }

        public static readonly DependencyProperty AdornedControlProperty =
            DependencyProperty.Register("AdornedControl", typeof(UIElement), typeof(ManipulationAdornerControl), new UIPropertyMetadata(null));
        public UIElement AdornedControl
        {
            get { return (UIElement)GetValue(AdornedControlProperty); }
            set { SetValue(AdornedControlProperty, value); }
        }
    }
}
