using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using Utilities.Animations;
using System.Windows;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using System.Runtime.Serialization;
using System.ComponentModel;

namespace AnimationManager
{
    [DataContract(Name = "VisualStateAnimation")]
    public class VisualStateAnimation : AnimationManager
    {
#region Properties

#endregion

#region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        public override void Demo()
        {
            VisualStateManager.GoToState(Control as FrameworkElement, "Demo", true);
            base.Demo();
        }
#endif
        IList<VisualStateGroup> listVisualGroups;
        public override void Execute()
        {
            if (Control == null)
                return;
#if !WINDOWS_UWP
            var control = Control as FrameworkElement;
#else
            var control = Control as Control;
#endif
            if (Control is ContentControl && (Control as ContentControl).Content is UIElement && !(Control is UserControl))
            {
                var content = Control as ContentControl;
#if !WINDOWS_UWP
                control = content.Content as FrameworkElement;
#else
                control = content.Content as Control;
#endif
                if (control == null)
                    return;
            }

            var child = System.Windows.Media.VisualTreeHelper.GetChild(control, 0) as FrameworkElement;
            if (child != null)
                listVisualGroups = VisualStateManager.GetVisualStateGroups(child) as IList<VisualStateGroup>;
            if (listVisualGroups == null)
                return;

            int index = 0;
            int toReach = (int)(CommonTarget);
            String toActivate = String.Empty;
            String toReachName = String.Empty;
            if (LastData != null && LastData.Value != null)
            {
                int num;
                bool isNumeric = int.TryParse(LastData.Value.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out num);

                if (isNumeric)
                {
                    toReach = num;
                }
                else
                {
                    toReachName = LastData.Value.ToString();
                }
            }

            foreach (var group in listVisualGroups)
            {
                foreach (VisualState state in group.States)
                {
                    if (!string.IsNullOrEmpty(toReachName))
                    {
                        if(toReachName == state.Name)
                        {
                            toActivate = state.Name;
                            break;
                        }
                    }
                    else if (index == toReach)
                    {
                        toActivate = state.Name;
                        break;
                    }
                    ++index;
                }
                if (!String.IsNullOrEmpty(toActivate))
                    break;
            }

            if (!String.IsNullOrEmpty(toActivate))
            {
#if !WINDOWS_UWP
                VisualStateManager.GoToState(control, toActivate, true);
#else
                VisualStateManager.GoToState(control, toActivate, true);
#endif
            }
        }

        public override void Stop()
        {
#if !WINDOWS_UWP
            VisualStateManager.GoToState(Control as FrameworkElement, "0", true);
#else
            VisualStateManager.GoToState(Control as Control, "0", true);
#endif
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.VisualStateName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return 0;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return false;
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
