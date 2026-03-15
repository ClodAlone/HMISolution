using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using System.Windows.Controls;
#endif
using System.ComponentModel;

namespace AnimationManager
{
#if !SILVERLIGHT
    [DataContract(Name = "AngleRotationY3DAnimation")]
    public class AngleRotationY3DAnimation : AnimationManager
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
#if !SILVERLIGHT
                OnPropertyChanged("TargetAngle");
#endif
                Reexecute();
#endif
            }
        }

        private double centerX = 0.5;
        [DataMember]
        public double CenterX
        {
            get { return centerX; }
            set
            {
                if (centerX == value)
                    return;

                centerX = value;
#if !NET_STANDARD
#if !SILVERLIGHT
                OnPropertyChanged("CenterX");
#endif
                Reexecute();
#endif
            }
        }

        private double centerY = 0.5;
        [DataMember]
        public double CenterY
        {
            get { return centerY; }
            set
            {
                if (centerY == value)
                    return;

                centerY = value;
#if !NET_STANDARD
#if !SILVERLIGHT
                OnPropertyChanged("CenterY");
#endif
                Reexecute();
#endif
            }
        }

        private double centerZ = 0.5;
        [DataMember]
        public double CenterZ
        {
            get { return centerZ; }
            set
            {
                if (centerZ == value)
                    return;

                centerZ = value;
#if !NET_STANDARD
#if !SILVERLIGHT
                OnPropertyChanged("CenterZ");
#endif
                Reexecute();
#endif
            }
        }
        #endregion

        #region Overrides
#if !NET_STANDARD
#if !SILVERLIGHT
        public override void Demo()
        {
            Control3D.RotateY3D(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
            base.Demo();
        }
#endif

        public override void Execute()
        {
            if (Control3D == null)
                return;
            Control3D.RotateY3D(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
        }

        public override void Stop()
        {
            Control3D.RotateY3D(0, -1, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
            base.Stop();
        }
#endif
#if !SILVERLIGHT
        public override String Name
        {
            get
            {
                return Properties.Resources.AngleRotationY3DName;
            }
        }
#endif
#if !WINDOWS_PHONE
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return TargetAngle;
            }
        }

#if !WINDOWS_PHONE
        [Browsable(false)]
#endif
        public override bool Is3D
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_PHONE
        [Browsable(false)]
#endif
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }

#endregion
    }
#endif
    }
