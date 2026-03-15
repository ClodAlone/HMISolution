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
    [DataContract(Name = "WidthAnimation")]
    public class WidthAnimation : AnimationManager
    {
#region Properties

        double width = 100;
        [DataMember]
        public double Width
        {
            get { return width; }
            set
            {
                if (value == width)
                    return;
                width = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Width");
#endif
                Reexecute();
#endif
            }
        }

        bool bAdaptPosition = true;
        [DataMember]
        public bool AdaptPosition
        {
            get { return bAdaptPosition; }
            set
            {
                if (value == bAdaptPosition)
                    return;
                bAdaptPosition = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("AdaptPosition");
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
            if (propertyName == "AnimationBehavior")
            {
                if (AnimationBehavior == AnimationBehavior.Trigger)
                    return Properties.Resources.InvalidAnimationBehaviour;
            }
            
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
                if (propertyName == "Width" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }
                
                return base[propertyName];
            }
        }
        public override void Demo()
        {
            Control.WidthAnimation(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null, AdaptPosition);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control == null)
                return;
            Control.WidthAnimation(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null, AdaptPosition);
        }

        public override void Stop()
        {
            if (Control != null)
                Control.WidthAnimation(0, -1, Repeatable, Autoreverse, AnimationEquation, null, AdaptPosition);
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.WidthName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return Width;
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
