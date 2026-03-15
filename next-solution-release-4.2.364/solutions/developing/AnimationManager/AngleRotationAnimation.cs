using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
#if !NET_STANDARD
using Utilities.Animations;
using System.Windows.Shapes;

#if !WINDOWS_UWP
using System.Windows.Controls;
#endif
#endif
using System.ComponentModel;

namespace AnimationManager
{
    [DataContract(Name = "AngleRotationAnimation")]
    public class AngleRotationAnimation : AnimationManager
    {
#region Properties

        private double targetAngle = 360;
        [DataMember]
        public double TargetAngle
        {
            get { return targetAngle; }
            set
            {
                if (targetAngle == value)
                    return;

                targetAngle = value;
#if !NET_STANDARD

#if !WINDOWS_UWP
                OnPropertyChanged("TargetAngle");
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
                if(propertyName == "TargetAngle" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }
                
                return base[propertyName];
            }
        }
        public override void Demo()
        {
            Control.Rotate(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
            base.Demo();
        }
#endif

        public override void Execute()
        {
            if (Control == null)
                return;
            Control.Rotate(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
        }

        public override void Stop()
        {
            if (Control != null)
                Control.Rotate(0, -1, Repeatable, Autoreverse, AnimationEquation, false, null);
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.AngleRotationName;
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
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return TargetAngle;
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
#endregion
#if !NET_STANDARD
        public override bool NotSupportedControl(UIElement element)
        {
            var propertyPoints = element.GetType().GetProperty("Points");
            if (propertyPoints != null || element is Line || element is Path)
                return true;
            return base.NotSupportedControl(element);
        }
#endif
    }
}
