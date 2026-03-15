using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using Utilities.Animations;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#endif
using System.Runtime.Serialization;
using System.ComponentModel;
using UFInterfaces;
using DocumentManager.ComponentService;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum ScaleType
    {
        Both,
        X,
        Y
    }

    [DataContract(Name = "ScaleAnimation")]
    public class ScaleAnimation : AnimationManager
    {
#region Properties

        double scale = 0.5;
        [DataMember]
        public double Scale
        {
            get { return scale; }
            set
            {
                if (value == scale)
                    return;
                scale = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Scale");
#endif
                Reexecute();
#endif
            }
        }

        ScaleType scaleType = ScaleType.Both;
        [DataMember]
        public ScaleType ScaleType
        {
            get { return scaleType; }
            set
            {
                if (value == scaleType)
                    return;
                scaleType = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("ScaleType");
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
                if (propertyName == "Scale" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            switch(ScaleType)
            {
                case ScaleType.Both:
                    Control.Scale(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
                case ScaleType.X:
                    Control.ScaleX(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
                case ScaleType.Y:
                    Control.ScaleY(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
            }
            base.Demo();
        }
#endif
        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            bRangeLimit = true;
        }

        public override void Execute()
        {
            if (Control == null)
                return;
            switch (ScaleType)
            {
                case ScaleType.Both:
                    Control.Scale(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
                case ScaleType.X:
                    Control.ScaleX(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
                case ScaleType.Y:
                    Control.ScaleY(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, false, null);
                    break;
            }
        }

        public override void Stop()
        {
            if (Control != null)
                Control.Scale(0, -1, Repeatable, Autoreverse, AnimationEquation, false, null);
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.ScaleName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return Scale;
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
