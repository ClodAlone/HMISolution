using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using Utilities.Animations;
using System.Windows;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Shapes;
#endif
#endif
using System.Runtime.Serialization;
using System.ComponentModel;

namespace AnimationManager
{
    [DataContract(Name = "MoveYAnimation")]
    public class MoveYAnimation : AnimationManager
    {
#region Properties

        double y = 100;
        [DataMember]
        public double Y
        {
            get { return y; }
            set
            {
                if (value == y)
                    return;
                y = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Y");
#endif
                Reexecute();
#endif
            }
        }

        #endregion

        #region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        protected override String PerformValidation(String propertyName)
        {
            return base.PerformValidation(propertyName);
        }
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationEquation")
                {
                    return false;
                }
                if (propertyName == "Y" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }
                
                return base[propertyName];
            }
        }

        private bool animationTimeZero;
        private double startingPoint;

        public override void Demo()
        {
            animationTimeZero = AnimationTime == 0 ? true : false;
            startingPoint = Canvas.GetTop(Control as FrameworkElement);
            Control.YAnimation(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control == null)
                return;
            Control.YAnimation(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null);
        }

        public override void Stop()
        {
            if (Control != null)
            {
                Control.YAnimation(0, -1, Repeatable, Autoreverse, AnimationEquation, null);

                if (animationTimeZero && base.bDemoMode)
                    Canvas.SetTop(Control, startingPoint);
            }
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.MoveYName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return Y;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }

#if !NET_STANDARD
        public override bool NotSupportedControl(UIElement element)
        {
            var propertyPoints = element.GetType().GetProperty("Points");
            if (propertyPoints != null || element is Line || element is Path)
                return true;
            return base.NotSupportedControl(element);
        }
#endif
#endif

#endregion
    }
}
