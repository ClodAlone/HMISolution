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
using Transitionals.Controls;
using Transitionals;
using Transitionals.Transitions;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for TransitionControl.xaml
    /// </summary>
    public partial class TransitionControl : UserControl
    {
        public TransitionControl()
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

        public Transition transition
        {
            get
            {
                return transitionBox.Transition;
            }
            set
            {
                transitionBox.Transition = value;
            }
        }

        public void SetTransition3D(double angle = 90)
        {
            transitionBox.Transition = new RotateTransition() { Angle = angle };
        }

        public void SetTransitionTranslate()
        {
            transitionBox.Transition = new TranslateTransition();
        }

        public void SetTransitionFadeAndGrow()
        {
            transitionBox.Transition = new FadeAndGrowTransition();
        }
    }
}
