using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
#if !NET_STANDARD
using Utilities.Animations;
#if !WINDOWS_UWP
using System.Windows.Controls;
#endif
#endif
using System.ComponentModel;

namespace AnimationManager
{
    [DataContract(Name = "BlinkAnimation")]
    public class BlinkAnimation : AnimationManager
    {
        public BlinkAnimation()
        {
            AnimationBehavior = AnimationBehavior.Trigger;
        }
#region Properties

        double opacityFrom = 0.0;
        [DataMember]
        public double OpacityFrom
        {
            get { return opacityFrom; }
            set
            {
                if (value == opacityFrom)
                    return;
                opacityFrom = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("OpacityFrom");
#endif
                Reexecute();
#endif
            }
        }

        double opacityTo = 1.0;
        [DataMember]
        public double OpacityTo
        {
            get { return opacityTo; }
            set
            {
                if (value == opacityTo)
                    return;
                opacityTo = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("OpacityTo");
#endif
                Reexecute();
#endif
            }
        }

        double scaleTo = 1.0;
        [DataMember]
        public double ScaleTo
        {
            get { return scaleTo; }
            set
            {
                if (value == scaleTo)
                    return;
                scaleTo = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("ScaleTo");
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
            if (propertyName == "OpacityTo" || propertyName == "OpacityFrom")
            {
                if (OpacityTo < 0 || OpacityTo > 1 || OpacityFrom < 0 || OpacityFrom > 1)
                    return Properties.Resources.OpacityOutOfRange;
            }

            return base.PerformValidation(propertyName);
        }
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "Repeatable" || propertyName == "Autoreverse" ||
                    propertyName == "AnimationEquation" || propertyName == "TagMinValue" || 
                    propertyName == "TagMaxValue" || propertyName == "AnimationBehavior")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            if (scaleTo != 1.0)
                Control.BlinkScale(CommonTarget, AnimationTime, AnimationEquation, false, null);
            if (OpacityFrom != OpacityTo)
                Control.Blink(AnimationTime, OpacityFrom, OpacityTo, AnimationEquation, null);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control == null)
                return;

            if (ScaleTo != 1.0)
            {
                var scaleValue = ScaleTo;
                double value1 = GetDataValue(lastdata);
                if (IsInvalidDouble(value1))
                    value1 = 0;
                switch (AnimationBehavior)
                {
                    case AnimationBehavior.Trigger:
                        {
                            if (value1 == 0)
                                scaleValue = 0;
                            break;
                        }
                    case AnimationBehavior.Absolute:
                        {
                            scaleValue = value1;
                            if (bRangeLimit)
                            {
                                scaleValue = Math.Max(scaleValue, Range.Low);
                                scaleValue = Math.Min(scaleValue, Range.High);
                            }
                            break;
                        }
                    case AnimationBehavior.Proportional:
                        {
                            scaleValue = (value1 - Range.Low) / (Range.High - Range.Low) * CommonTarget;
                            if (bRangeLimit)
                            {
                                scaleValue = Math.Max(scaleValue, Range.Low);
                                scaleValue = Math.Min(scaleValue, Range.High);
                            }
                            if (IsInvalidDouble(scaleValue))
                                scaleValue = 0;
                            break;
                        }
                }

                Control.BlinkScale(scaleValue, AnimationTime, AnimationEquation, false, null);
            }

            if (OpacityFrom != OpacityTo)
            {
                var opacityTo = OpacityTo;
                double value1 = GetDataValue(lastdata);
                if (IsInvalidDouble(value1))
                    value1 = 0;
                switch (AnimationBehavior)
                {
                    case AnimationBehavior.Trigger:
                        {
                            if (value1 == 0)
                                opacityTo = 0;
                            break;
                        }
                    case AnimationBehavior.Absolute:
                        {
                            opacityTo = value1;
                            if (bRangeLimit)
                            {
                                opacityTo = Math.Max(opacityTo, Range.Low);
                                opacityTo = Math.Min(opacityTo, Range.High);
                            }
                            break;
                        }
                    case AnimationBehavior.Proportional:
                        {
                            opacityTo = (value1 - Range.Low) / (Range.High - Range.Low) * CommonTarget;
                            if (bRangeLimit)
                            {
                                opacityTo = Math.Max(opacityTo, Range.Low);
                                opacityTo = Math.Min(opacityTo, Range.High);
                            }
                            if (IsInvalidDouble(opacityTo))
                                opacityTo = 0;
                            break;
                        }
                }

                Control.Blink(AnimationTime, OpacityFrom, opacityTo, AnimationEquation, null);
            }
        }

        public override void Stop()
        {
            if (Control != null)
            {
                Control.Blink(-1, OpacityFrom, OpacityTo, AnimationEquation, null);
                Control.BlinkScale(dTargetValue, -1, AnimationEquation, false, null);
            }
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.BlinkName;
            }
        }
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return ScaleTo;
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

        
#endif

        #endregion
    }
}
