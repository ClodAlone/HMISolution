#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents ribon's separator item.
	/// </summary>
    /// 
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
     Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
  Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2003;component/Ribbon.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.VS2010;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
 Type = typeof(RibbonSeparator), XamlResource = "/Syncfusion.Theming.Transparent;component/Ribbon.xaml")]

    public class RibbonSeparator : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonSeparator"/> class.
		/// </summary>
		public RibbonSeparator()
		{
			this.DefaultStyleKey = typeof(RibbonSeparator);
		}

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RibbonSeparator), new PropertyMetadata(Orientation.Horizontal));

        private static void OnOrientationChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RibbonSeparator _separator = sender as RibbonSeparator;
            if ((Orientation)e.NewValue == Orientation.Horizontal)
            {
                _separator._horizontal.Visibility = Visibility.Visible;
                _separator._vertical.Visibility = Visibility.Collapsed;
            }
            else
            {
                _separator._horizontal.Visibility = Visibility.Collapsed;
                _separator._vertical.Visibility = Visibility.Visible;
            }
        }

        private FrameworkElement _horizontal;
        private FrameworkElement _vertical;

	    /// <summary>
	    /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
	    /// </summary>
	    public override void OnApplyTemplate()
        {
            _horizontal = GetTemplateChild("PART_Horizontal") as FrameworkElement;
            _vertical = GetTemplateChild("PART_Vertical") as FrameworkElement;
            if (_horizontal != null && _vertical != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    _horizontal.Visibility = Visibility.Visible;
                    _vertical.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _horizontal.Visibility = Visibility.Collapsed;
                    _vertical.Visibility = Visibility.Visible;
                }
            }
            base.OnApplyTemplate();
        }
	}
}
