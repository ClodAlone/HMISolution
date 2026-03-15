using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using Utilities.Animations;
using System.Windows.Controls;
#endif
using System.Runtime.Serialization;
using System.ComponentModel;
using UFInterfaces;
using DocumentManager.ComponentService;

namespace AnimationManager
{
#if !SILVERLIGHT
    [DataContract(Name = "Scale3DAnimation")]
    public class Scale3DAnimation : AnimationManager
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
#if !SILVERLIGHT
                OnPropertyChanged("Scale");
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
            Control3D.Scale3D(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
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
            if (Control3D == null)
                return;
            Control3D.Scale3D(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
        }

        public override void Stop()
        {
            Control3D.Scale3D(0, -1, Repeatable, Autoreverse, AnimationEquation, CenterX, CenterY, CenterZ);
            base.Stop();
        }
#endif

#if !SILVERLIGHT
        public override String Name
        {
            get
            {
                return Properties.Resources.Scale3DName;
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
                return Scale;
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
