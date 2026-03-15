using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Controls.Primitives;
using Utilities;
using Utilities.WPF;
using System.Windows.Media;
using PropertyControl.ComponentService;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for Effects.xaml
    /// </summary>
    public partial class EffectSettings : UserControl, IDisposable
    {
        readonly IPropertyControl Property;
        readonly UserControl controlBlur;
        readonly UserControl controlShadow;

        public EffectSettings(IPropertyControl property)
        {
            InitializeComponent();

            Property = property;
            controlBlur = property.controlNoSelection;
            controlBlur.ClearValue(FrameworkElement.WidthProperty);
            controlBlur.ClearValue(FrameworkElement.HeightProperty);
            propertyGridBlur.Content = controlBlur;

            controlShadow = property.controlNoSelection;
            controlShadow.ClearValue(FrameworkElement.WidthProperty);
            controlShadow.ClearValue(FrameworkElement.HeightProperty);
            propertyGridShadow.Content = controlShadow;

            DataContextChanged += (o, e) =>
                {
                    btnShadow.IsChecked = GetEffect<DropShadowEffect>(Property, controlShadow);
                    btnBlur.IsChecked = GetEffect<BlurEffect>(Property, controlBlur);
                };
        }

        private void Shadow_Click(object sender, RoutedEventArgs e)
        {
            bool? bSet = btnShadow.IsChecked;
            SetEffect<DropShadowEffect>(bSet != null && bSet == true, Property, controlShadow);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            bool? bSet = btnBlur.IsChecked;
            SetEffect<BlurEffect>(bSet != null && bSet == true, Property, controlBlur);
        }

        private bool GetEffect<T>(IPropertyControl property, UserControl control) where T : Effect
        {
            bool bRet = false;
            //property.SetControlSelection(control, null);
            UIElement TargetElement = DataContext as UIElement;
            control.Visibility = Visibility.Collapsed;
            if (TargetElement != null)
            {
                if (TargetElement.Effect is T)
                {
                    bRet = true;
                    property.SetControlSelection(control, TargetElement.Effect);
                    control.Visibility = Visibility.Visible;
                }
            }

            return bRet;
        }

        private void SetEffect<T>(bool bSet, IPropertyControl property, UserControl control) where T : Effect, new()
        {
            UIElement TargetElement = DataContext as UIElement;
            if (TargetElement != null)
            {
                if (!bSet)
                {
                    if (TargetElement.Effect is T)
                        TargetElement.Effect = null;
                    //property.SetControlSelection(control, null);
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {
                    T t = new T();
                    TargetElement.Effect = t;
                    property.SetControlSelection(control, t);
                    control.Visibility = Visibility.Visible;
                }
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            UIElement TargetElement = DataContext as UIElement;
            if (TargetElement != null)
            {
                // TargetElement.BitmapEffect = null;
                TargetElement.Effect = null;
            }
        }

        #region IDisposable
        public void Dispose()
        {
            if (propertyGridBlur.Content is IDisposable)
                (propertyGridBlur.Content as IDisposable).Dispose();

            if (propertyGridShadow.Content is IDisposable)
                (propertyGridShadow.Content as IDisposable).Dispose();
        }
        #endregion
    }
}
