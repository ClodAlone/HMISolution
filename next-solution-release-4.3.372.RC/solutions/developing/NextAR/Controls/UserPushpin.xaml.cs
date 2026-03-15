
#if WP8
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace NextAR.Controls
{
    public sealed partial class UserPushpin : UserControl
    {
        public UserPushpin()
        {
            this.InitializeComponent();
        }
    }
}
