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

namespace AnimationManager
{
#if !SILVERLIGHT
    [DataContract(Name = "MoveZ3DAnimation")]
    public class MoveZ3DAnimation : AnimationManager
    {
#region Properties

        double z = 100;
        [DataMember]
        public double Z
        {
            get { return z; }
            set
            {
                if (value == z)
                    return;
                z = value;
#if !NET_STANDARD
#if !SILVERLIGHT
                OnPropertyChanged("Z");
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
            Control3D.TranslateZ3D(CommonTarget, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control3D == null)
                return;
            Control3D.TranslateZ3D(dTargetValue, AnimationTime, Repeatable, Autoreverse, AnimationEquation, null);
        }

        public override void Stop()
        {
            Control3D.TranslateZ3D(0, -1, Repeatable, Autoreverse, AnimationEquation, null);
            base.Stop();
        }
#endif

#if !SILVERLIGHT
        public override String Name
        {
            get
            {
                return Properties.Resources.MoveZ3DName;
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
                return Z;
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
