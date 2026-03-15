using System;
using System.Runtime.CompilerServices;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Display
{

    internal abstract class Animator : DependencyObject
    {
        protected Animator()
        {
        }

        public static void Animate(Animator animator, TimeSpan duration)
        {
            Storyboard storyboard = animator.GetStoryboard(duration.TotalMilliseconds);
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        public abstract Storyboard GetStoryboard(double allowedTimeMs);

        public bool AutoReverse { get; set; }

        public virtual double DesiredDurationMs { get; set; }

        public DependencyObject TargetObject { get; set; }
    }
}
