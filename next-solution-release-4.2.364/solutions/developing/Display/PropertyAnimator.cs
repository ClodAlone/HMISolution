using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
#endif

namespace Display
{
    internal abstract class PropertyAnimator : Animator
    {
        private PropertyInfo property;

        public PropertyAnimator()
        {
            this.DesiredDurationMs = 1000.0;
            this.TargetIsSet = false;
        }

        public override Storyboard GetStoryboard(double allowedTimeMs)
        {
            if ((base.TargetObject == null) || string.IsNullOrEmpty(this.TargetProperty))
            {
                return null;
            }
            Timeline element = this.GetTimeline();
            Storyboard storyboard = new Storyboard();
            element.Duration = new Duration(TimeSpan.FromMilliseconds(allowedTimeMs));
            element.AutoReverse = base.AutoReverse;
            if (!this.TargetIsSet)
            {
#if !WINDOWS_UWP
                Storyboard.SetTarget(element, base.TargetObject);
                Storyboard.SetTargetProperty(element, new PropertyPath(this.TargetProperty, new object[0]));
#else
                Storyboard.SetTarget(storyboard, base.TargetObject);
                Storyboard.SetTargetProperty(storyboard, TargetProperty);
#endif
            }
            storyboard.Children.Add(element);
            return storyboard;
        }

        public abstract Timeline GetTimeline();

        public object By { get; set; }

        public object From { get; set; }

        protected PropertyInfo Property
        {
            get
            {
                if (this.property == null)
                {
                    this.property = base.TargetObject.GetType().GetProperty(this.TargetProperty);
                    if (this.property == null)
                    {
                        throw new Exception("Type '" + base.TargetObject.GetType().Name + "' doesn't have property '" + this.TargetProperty + "'.");
                    }
                }
                return this.property;
            }
        }

        protected bool TargetIsSet { get; set; }

        public string TargetProperty { get; set; }

        public object To { get; set; }
    }
}
