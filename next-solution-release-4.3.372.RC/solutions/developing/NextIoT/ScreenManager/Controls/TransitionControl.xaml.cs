using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ScreenManager.Controls
{
    public sealed partial class TransitionControl : UserControl
    {
        public TransitionControl()
        {
            this.InitializeComponent();
        }

        public async void SetContent(UserControl control)
        {
            await StartDissolvingAsync();
            SplitContent.Content = control;
        }

        public UserControl GetContent()
        {
            return SplitContent.Content as UserControl;
        }

        RenderTargetBitmap rtb = new RenderTargetBitmap();
        async Task StartDissolvingAsync()
        {
            try
            {
                await rtb.RenderAsync(SplitContent);
                DissolveImage.Source = rtb;

                DissolveImage.Visibility = Visibility.Visible;
                DissolveImage.Opacity = 1.0;
                AnimateDouble(DissolveImage, "Opacity", 0.0, 200, () =>
                {
                    DissolveImage.Visibility = Visibility.Collapsed;
                });

                SplitContentTransform.Y = 25;
                AnimateDouble(SplitContentTransform, "Y", 0, 150);
            }
            catch
            {
                // Ignore error
                DissolveImage.Visibility = Visibility.Collapsed;
                SplitContentTransform.Y = 25;
                AnimateDouble(SplitContentTransform, "Y", 0, 150);
            }
        }

        public static void AnimateDouble(DependencyObject target, string path, double to, double duration, Action onCompleted = null)
        {
            var animation = new DoubleAnimation
            {
                EnableDependentAnimation = true,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(duration))
            };
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, path);

            var sb = new Storyboard();
            sb.Children.Add(animation);

            if (onCompleted != null)
            {
                sb.Completed += (s, e) =>
                {
                    onCompleted();
                };
            }

            sb.Begin();
        }

    }
}
