using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Animations;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows;

namespace AnimationManager
{
    [DataContract(Name = "VisualStateAnimation")]
    class VisualStateAnimation : AnimationManager
    {
        #region Properties

        #endregion

        #region Overrides

#if !SILVERLIGHT
        public override void Demo()
        {
            VisualStateManager.GoToState(Control as FrameworkElement, "Demo", true);
        }
#endif
        public override void Execute()
        {
            VisualStateManager.GoToState(Control as Control, ((int)(CommonTarget)).ToString(), true);
        }

        public override void Stop()
        {
            VisualStateManager.GoToState(Control as Control, "0", true);
        }

#if !SILVERLIGHT
        [Browsable(false)]
        public override String Name
        {
            get
            {
                return Properties.Resources.VisualStateName;
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
                return 0;
            }
        }

        #endregion
    }
}
