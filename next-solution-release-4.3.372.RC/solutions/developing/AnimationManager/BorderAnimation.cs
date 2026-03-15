using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
#endif
using System.ComponentModel;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;

namespace AnimationManager
{
    [DataContract(Name = "BorderAnimation")]
#if !NET_STANDARD
    [KnownType(typeof(DoubleCollection))]
    [KnownType(typeof(PenLineCap))]
#endif    
    public class BorderAnimation : AnimationManager
    {
        public BorderAnimation()
        {
            targetOffset = 2000;
            AnimationTime = 60000;
            Repeatable = true;
            Autoreverse = false;
#if !NET_STANDARD
            AnimationEquation = null;
#endif
            AnimationBehavior = AnimationBehavior.Trigger;
        }

#region Members
#if !NET_STANDARD
        double oldstrokeThickness;
        DoubleCollection oldstrokeDashArray;
        double oldstrokeDashOffset;
        PenLineCap oldstrokeDashCap;
        bool bSaved;
#endif
#endregion

#region Properties

        [DataMember]
        double targetOffset = 20;
        public double TargetOffset
        {
            get { return targetOffset; }
            set
            {
                if (value == targetOffset)
                    return;
                targetOffset = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("TargetOffset");
#endif
                Reexecute();
#endif
            }
        }

        [DataMember]
        double strokeThickness = 20;
        public double StrokeThickness
        {
            get { return strokeThickness; }
            set
            {
                if (value == strokeThickness)
                    return;
                strokeThickness = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("StrokeThickness");
#endif
                Reexecute();
#endif
            }
        }

#if !NET_STANDARD
        [DataMember]
        DoubleCollection strokeDashArray = new DoubleCollection() { 4, 4 };
        public DoubleCollection StrokeDashArray
        {
            get { return strokeDashArray; }
            set
            {
                if (value == strokeDashArray)
                    return;
                strokeDashArray = value;
#if !WINDOWS_UWP
                OnPropertyChanged("StrokeDashArray");
#endif
                Reexecute();
            }
        }
#endif

        [DataMember]
        double strokeDashOffset = 20;
        public double StrokeDashOffset
        {
            get { return strokeDashOffset; }
            set
            {
                if (value == strokeDashOffset)
                    return;
                strokeDashOffset = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("StrokeDashOffset");
#endif
                Reexecute();
#endif
            }
        }

#if !NET_STANDARD
        [DataMember]
        PenLineCap strokeDashCap = PenLineCap.Square;
        public PenLineCap StrokeDashCap
        {
            get { return strokeDashCap; }
            set
            {
                if (value == strokeDashCap)
                    return;
                strokeDashCap = value;
#if !WINDOWS_UWP
                OnPropertyChanged("StrokeDashCap");
#endif
                Reexecute();
            }
        }
#endif

#endregion

#region Methods
#if !NET_STANDARD
        void SaveControlValues()
        {
            var shape = Control as Shape;
            if (shape == null || bSaved)
                return;
            bSaved = true;
            oldstrokeThickness = shape.StrokeThickness;
            oldstrokeDashArray = shape.StrokeDashArray;
            oldstrokeDashOffset = shape.StrokeDashOffset;
            oldstrokeDashCap = shape.StrokeDashCap;
        }

        void RestoreControlValues()
        {
            var shape = Control as Shape;
            if (shape == null || !bSaved)
                return;
            StopAnimate();

            shape.StrokeThickness = oldstrokeThickness;
            shape.StrokeDashArray = oldstrokeDashArray;
            shape.StrokeDashOffset = oldstrokeDashOffset;
            shape.StrokeDashCap = oldstrokeDashCap;
        }

        void StartAnimate()
        {
            var shape = Control as Shape;
            if (shape == null)
                return;

            shape.StrokeThickness = StrokeThickness;
            shape.StrokeDashArray = StrokeDashArray;
            shape.StrokeDashOffset = StrokeDashOffset;
            shape.StrokeDashCap = StrokeDashCap;
            shape.StrokeDashOffsetAnimation(TargetOffset, AnimationTime, Repeatable, Autoreverse, AnimationEquation);
        }

        void StopAnimate()
        {
            var shape = Control as Shape;
            if (shape == null)
                return;
            shape.StrokeDashOffsetAnimation(0, -1, false, false, null);
        }
#endif
#endregion

#region Overrides
#if !NET_STANDARD
        public override Type[] ExpectingControl()
        {
            return new Type[] { typeof(Shape) };
        }

        protected override void Reexecute()
        {
            if (!bExecuted)
                return;

            StopAnimate();
            RestoreControlValues();

            SaveControlValues();
            Execute();
        }

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            SaveControlValues();
        }

#if !WINDOWS_UWP
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationEquation" || propertyName == "TagMinValue" ||
                    propertyName == "TagMaxValue" || propertyName == "AnimationBehavior")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            SaveControlValues();
            StartAnimate();
            base.Demo();
        }
#endif
        public override void Execute()
        {
            StartAnimate();
        }

        public override void Stop()
        {
            StopAnimate();
            RestoreControlValues();
            if (bDemoMode)
                bSaved = false;
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.BorderName;
            }
        }

        public override double CommonTarget
        {
            get
            {
                return 1;
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
