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
using System.Windows.Media.Animation;

namespace ScreenManager.SpecialObjects
{
    /// <summary>
    /// Interaction logic for AnimatedProgressBar.xaml
    /// </summary>
    public partial class AnimatedProgressBar : ProgressBar
    {
        public AnimatedProgressBar()
        {
            InitializeComponent();
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(AnimatedProgressBar), new PropertyMetadata(0d, ValueChanged));

        public static void ValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            // Get out of a static and into the instance ASAP.
            AnimatedProgressBar control = (AnimatedProgressBar)sender;
            control.ValueChanged();
        }

        private void ValueChanged()
        {
            DoubleAnimation animation = new DoubleAnimation(); 
            animation.To = this.Value;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.EasingFunction = new SineEase();
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ProgressBar.ValueProperty));
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }
    }
}
