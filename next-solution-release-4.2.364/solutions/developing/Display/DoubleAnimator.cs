using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_UWP
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Display
{
    internal class DoubleAnimator : PropertyAnimator
    {
        public override Timeline GetTimeline()
        {
            DoubleAnimation animation = new DoubleAnimation();
            if (base.From != null)
            {
                animation.From = new double?(Convert.ToDouble(base.From));
            }
            if (base.To != null)
            {
                animation.To = new double?(Convert.ToDouble(base.To));
            }
            if (base.By != null)
            {
                animation.By = new double?(Convert.ToDouble(base.By));
            }
            return animation;
        }
    }
}
