#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Theming;
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
     Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
 Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2003;component/Ribbon.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.VS2010;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(ScreenTip), XamlResource = "/Syncfusion.Theming.Transparent;component/Ribbon.xaml")]
    public class ScreenTip : ToolTip
    {
        /// <summary>
        /// 
        /// </summary>
        public ScreenTip()
        {
            this.DefaultStyleKey = typeof(ScreenTip);
        }

        static ScreenTip()
        {
 
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ScreenTip), new PropertyMetadata(string.Empty));
        
        /// <summary>
        /// 
        /// </summary>
        public ImageSource HelpImage
        {
            get { return (ImageSource)GetValue(HelpImageProperty); }
            set { SetValue(HelpImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HelpImage.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HelpImageProperty =
            DependencyProperty.Register("HelpImage", typeof(ImageSource), typeof(ScreenTip), new PropertyMetadata(null));
        
        /// <summary>
        /// 
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ScreenTip), new PropertyMetadata(OnImageSourceChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderobj = sender as ScreenTip;
            if (senderobj.IconSize.Width == 0 && senderobj.IconSize.Height == 0)
            {
                senderobj.IconSize = new Size(32, 32);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HelpText
        {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HelpText.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HelpTextProperty =
            DependencyProperty.Register("HelpText", typeof(string), typeof(ScreenTip), new PropertyMetadata(String.Empty));
        
        /// <summary>
        /// 
        /// </summary>
        public Size IconSize
        {
            get { return (Size)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(Size), typeof(ScreenTip), new PropertyMetadata(new Size(0, 0)));

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ToolTip"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.VerticalOffset = 5;
            this.Opened += new RoutedEventHandler(ScreenTip_Opened);
        }

        internal static double CurrentHorizontalOffset = 0.00;
        internal FrameworkElement parentUIElement = null;

        private void ScreenTip_Opened(object sender, RoutedEventArgs e)
        {
            //this.HorizontalOffset = ((parentUIElement != null) ? parentUIElement.DesiredSize.idth : 0) - CurrentHorizontalOffset;
            //this.HorizontalOffset = CurrentHorizontalOffset;

            if (parentUIElement != null)
            {
                RibbonBar ribbonbar = VisualUtils.FindAncestor(parentUIElement, typeof(RibbonBar)) as RibbonBar;

                if (ribbonbar != null)
                {
                    GeneralTransform elementTrans = parentUIElement.TransformToVisual(ribbonbar);
                    Point elementPoint = elementTrans.Transform(new Point(0, 0));

                    VerticalOffset = 3+ribbonbar.RenderSize.Height - (elementPoint.Y + parentUIElement.RenderSize.Height);
                }
            }
        }
    }
}