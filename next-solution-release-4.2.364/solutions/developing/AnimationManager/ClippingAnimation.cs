using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media;
#endif
using System.ComponentModel;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum ClippingType
    {
        BottomUp,
        UpBottom,
        LeftRight,
        RightLeft,
        CenterWidth,
        CenterHeight,
        CenterCircular
    }

    [DataContract(Name = "ClippingAnimation")]
    public class ClippingAnimation : AnimationManager
    {
#if !NET_STANDARD
#if !WINDOWS_UWP
        Geometry oldClipping;
#else
        RectangleGeometry oldClipping;
#endif
#endif
        // Storyboard.TargetProperty="(UIElement.Clip).(EllipseGeometry.RadiusX)"
        // EllipseGeometry eg;
        // RectangleGeometry rg;
        // LineGeometry lg;
        // GeometryGroup gg;
        // Path pg;
#region Properties

        private ClippingType type = ClippingType.BottomUp;
        [DataMember]
        public ClippingType Type
        {
            get { return type; }
            set
            {
                if (type == value)
                    return;

                type = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Type");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#region Methods
#if !NET_STANDARD
        void ApplyClipping()
        {
            var fe = Control as FrameworkElement;
            if (fe == null)
                return;

#if !WINDOWS_UWP
            Geometry clip = null;
#else
            RectangleGeometry clip = null;
#endif

            var height = fe.ActualHeight;
            var width = fe.ActualWidth;
            switch(Type)
            {
                case ClippingType.CenterCircular:
#if !WINDOWS_UWP
                    clip = new EllipseGeometry()
                    {
                        RadiusX = dTargetValue,
                        RadiusY = dTargetValue,
                        Center = new Point(width /2, height / 2)
                    };
                    break;
#endif
                case ClippingType.BottomUp:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, Math.Max(height - dTargetValue, 0), width, height)
                    };
                    break;
                case ClippingType.UpBottom:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, 0, width, dTargetValue)
                    };
                    break;
                case ClippingType.LeftRight:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, 0, dTargetValue, height)
                    };
                    break;
                case ClippingType.RightLeft:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(Math.Max(width - dTargetValue, 0), 0, width, height)
                    };
                    break;
                case ClippingType.CenterHeight:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(0, Math.Max((height / 2) - (dTargetValue / 2), 0), width, dTargetValue)
                    };
                    break;
                case ClippingType.CenterWidth:
                    clip = new RectangleGeometry()
                    {
                        Rect = new Rect(Math.Max((width / 2) - (dTargetValue / 2), 0), 0, dTargetValue, height)
                    };
                    break;
            };
                // dTargetValue;
            Control.Clip = clip;
        }
#endif
#endregion

#region Overrides
#if !NET_STANDARD
        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            if (Control != null)
                oldClipping = Control.Clip;
        }

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
                if (propertyName == "AnimationEquation" ||
                    propertyName == "Repeatable" || propertyName == "Autoreverse")
                {
                    return false;
                }
                if (propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }
                return base[propertyName];
            }
        }
        public override void Demo()
        {
            ApplyClipping();
            base.Demo();
        }
#endif

        public override void Execute()
        {
            if (Control == null)
                return;
            ApplyClipping();
        }

        public override void Stop()
        {
            if (Control != null)
                Control.Clip = oldClipping;
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.ClippingName;
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
#if !NET_STANDARD
                var fe = Control as FrameworkElement;
                if (fe == null)
                    return base.CommonTarget;

                var height = fe.ActualHeight;
                var width = fe.ActualWidth;
                switch (Type)
                {
                    case ClippingType.CenterCircular:
                        return Math.Max(width, height);
                    case ClippingType.BottomUp:
                        return height;
                    case ClippingType.UpBottom:
                        return height;
                    case ClippingType.LeftRight:
                        return width;
                    case ClippingType.RightLeft:
                        return width;
                    case ClippingType.CenterHeight:
                        return height;
                    case ClippingType.CenterWidth:
                        return width;
                };
#endif
                return base.CommonTarget;
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
    }
}
