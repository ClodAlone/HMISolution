using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCUAViewModel;
using ScreenSettings.Entities;
using ScreenManager.Adorners.Converters;
using Utilities;

namespace ScreenManager.Adorners
{
    /// <summary>
    /// Interaction logic for EditingAdornerControl.xaml
    /// </summary>
    public partial class EditingAdornerControl : UserControl
    {
        UIElement element;
        public EditingAdornerControl(UIElement e)
        {
            InitializeComponent();

            var converter = FindResource("InverseZoomConverter") as InverseZoomConverter;
            converter.Control = this;

            panelButtons.Background = ApplicationPropertiesHelper.GetProperty("CurrentSkinBackColor") as Brush;
            menuLabel1.Foreground = menuLabel2.Foreground = menuLabel3.Foreground = menuLabel4.Foreground = menuLabel5.Foreground = menuLabel6.Foreground = menuLabel10.Foreground = menuLabel11.Foreground = menuLabel12.Foreground = menuLabel13.Foreground = menuLabel14.Foreground = menuLabel15.Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;

            // WPFUtilities.ThemeHelper.SetTheme(this, DocumentManager.ComponentService.ThemeType.Blend.ToString());
            element = e;
            if (element is ContentControl && (element as ContentControl).Content is UIElement)
                txtType.Text = ((element as ContentControl).Content as UIElement).GetType().Name;
            else
                txtType.Text = element.GetType().Name;
            Manipulation.IsChecked = element.IsManipulationEnabled;
            btnBitmapCache.IsChecked = element.CacheMode != null;

            Type t = element.GetType();
            var cust = t.GetProperty("SmartControl");
            if (cust != null)
            {
                var smartControl = cust.GetValue(element) as UserControl;
                if (smartControl != null)
                {
                    btnShowSmart.Visibility = System.Windows.Visibility.Visible;
                    popupSmart.Opened += (ob, ev) =>
                        {
                            //popupSmart.Child = smartControl;
                            //panelSmart.Children.Clear();
                            //panelSmart.Children.Add(smartControl);
                            smartControl.ClearValue(FrameworkElement.WidthProperty);
                            smartControl.ClearValue(FrameworkElement.HeightProperty);
                            contentSmart.Content = smartControl;
                        };
                }
            }
        }

        private void Manipulation_Click(object sender, RoutedEventArgs e)
        {
            element.IsManipulationEnabled = !element.IsManipulationEnabled;
            Manipulation.IsChecked = element.IsManipulationEnabled;
        }

        private void btnBitmapCache_Click(object sender, RoutedEventArgs e)
        {
            if (element.CacheMode == null)
                element.CacheMode = new BitmapCache() { EnableClearType = true };
            else
                element.CacheMode = null;
            btnBitmapCache.IsChecked = element.CacheMode != null;
        }

        private void popup_Opened(object sender, EventArgs e)
        {
            btnBitmapCache.IsChecked = element.CacheMode != null;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    bool bIsChecked = btnShow.IsChecked != null && btnShow.IsChecked.Value == true;
        //    panelButtons.IsHitTestVisible = bIsChecked;
        //    btnShow.Content = bIsChecked ? "<" : ">";
        //    var storyboard = TryFindResource(bIsChecked ? "MouseOverOpacity" : "MouseLeaveOpacity") as Storyboard;
        //    storyboard.Begin();
        //}
    }
}
