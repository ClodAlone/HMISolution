using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using System.Windows.Media.Effects;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities.Animations;
#endif
using System.ComponentModel;
#endif
using Utilities;
using UFInterfaces;
using DocumentManager.ComponentService;
using WPFUtilities;

namespace AnimationManager
{
    [DataContract(Name = "BlurAnimation")]
    public class BlurAnimation : AnimationManager
    {
#region Members
#endregion

#region Methods
#if !NET_STANDARD
        void SetBlur(int radius)
        {
            if (Control == null)
                return;
            if (Control.Effect is BlurEffect)
            {
                if (radius <= 0)
                    Control.Effect = null;
                else
                {
                    var effect = Control.Effect as BlurEffect;
                    effect.Radius = radius;
                    effect.KernelType = KernelType.Gaussian;
                }
            }
            else if (radius > 0)
            {
                var effect = new BlurEffect();
                effect.Radius = radius;
                effect.KernelType = KernelType.Gaussian;
                Control.Effect = effect;
                Control.ClipToBounds = false;
            }
        }
#endif
#endregion

                #region Properties

                int blurRadius = 5;
        [DataMember]
        public int BlurRadius
        {
            get { return blurRadius; }
            set
            {
                if (value == blurRadius)
                    return;
                blurRadius = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("BlurRadius");
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
                if (propertyName == "AnimationTime" || propertyName == "Repeatable" || 
                    propertyName == "Autoreverse" || propertyName == "AnimationEquation")
                {
                    return false;
                }
                if (propertyName == "BlurRadius" ||
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }

                return base[propertyName];
            }
        }

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            if (oldEffect != null)
                oldEffect = Control.Effect;

            base.Init(entity, parent, sessionname);
        }

        Effect oldEffect;
        public override void Demo()
        {
            if (Control == null)
                return;
            if (oldEffect == null)
                oldEffect = Control.Effect;

            SetBlur(BlurRadius);

            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control == null)
                return;
            try
            {
                SetBlur(Convert.ToInt32(dTargetValue));
            }
            catch { }
        }

        public override void Stop()
        {
            Control.Effect = oldEffect;
            if (bDemoMode)
                oldEffect = null;
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.BlurName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return BlurRadius;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is3D
        {
            get
            {
                return false;
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
