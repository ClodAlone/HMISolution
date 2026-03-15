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

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for TransitionControl3D.xaml
    /// </summary>
    public partial class TransitionControl3D : UserControl
    {
        public TransitionControl3D()
        {
            InitializeComponent();
        }

        public Object control
        {
            get 
            { 
                return transitionBox.Content; 
            }
            set
            {
                transitionBox.Content = value;
            }
        }

    }
}
