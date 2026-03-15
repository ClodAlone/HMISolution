#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Custom Tab Item.
    /// </summary>
    public class CustomTabItem : TabItem
    {
        private bool isMouseMoving = false;

        private bool isMouseDown = false;

        internal bool CanDragWindow = false;

        internal bool isMouseLeave = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTabItem"/> class.
        /// </summary>
        public CustomTabItem()
        {
            this.DefaultStyleKey = typeof(CustomTabItem);
        }

        /// <summary>
        /// Occurs when [mouse left button down].
        /// </summary>
        public new event EventHandler MouseLeftButtonDown;

        /// <summary>
        /// Occurs when [mouse left button up].
        /// </summary>
        public new event EventHandler MouseLeftButtonUp;

        /// <summary>
        /// Represents the Unselected Bottom Template.
        /// </summary>
        protected internal Grid templateBottomUnselected = null;

        /// <summary>
        /// Gets or sets the own window.
        /// </summary>
        /// <value>The own window.</value>
        protected internal Window OwnWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsSelected)
                CanDragWindow = true;

            isMouseDown = true;
            if (MouseLeftButtonDown != null)
            {
                MouseLeftButtonDown(this, e);
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            isMouseDown = false;
            if (MouseLeftButtonUp != null)
            {
                MouseLeftButtonUp(this, e);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            isMouseLeave = true;
            if (isMouseMoving && isMouseDown)
            {
                CanDragWindow = true;
                isMouseDown = false;
                isMouseMoving = false;
            }
            else if (!CanDragWindow && isMouseDown)
            {
                this.CaptureMouse();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            /*** Microsoft TabControl is having an issue. When we click the tab item for the first time, mouse leave event will be
             * fired for tab item. So in order to avoid it, below coding helps ****/

            isMouseMoving = false;
            if (isMouseDown)
            {
                isMouseMoving = true;
                if (isMouseDown && isMouseLeave && !CanDragWindow)
                {
                    CanDragWindow = true;
                    this.ReleaseMouseCapture();
                    if(OwnWindow != null && OwnWindow.DockingManager != null)
                        this.OwnWindow.DockingManager.TabItemMouseLeave(this, e);                 
                }
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// The Border b.
        /// </summary>
        private Border b = null;

        /// <summary>
        /// The TextBlock dc.
        /// </summary>
        internal TextBlock dc = null;

        /// <summary>
        /// Represents the Selected Dot Text.
        /// </summary>
        protected internal TextBlock txtDotSelected = null;

        /// <summary>
        /// Represented the UnSelected Dot text.
        /// </summary>
        protected internal TextBlock txtDotUnselected = null;

        /// <summary>
        /// Representsm the Visibility Dot Selected text.
        /// </summary>
        protected internal bool visibilitytxtDotSelected = false;

        /// <summary>
        /// Represents the Unselected Visibility Dot Text.
        /// </summary>
        protected internal bool visibilitytxtDotUnselected = false;

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.TabItem"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            b = GetTemplateChild("TempBorder") as Border;
            dc= GetTemplateChild("HeaderBottomUnselected") as TextBlock;
            txtDotSelected = GetTemplateChild("txtDotSelected") as TextBlock;
            txtDotUnselected = GetTemplateChild("txtDotUnselected") as TextBlock;
            templateBottomUnselected = GetTemplateChild("TemplateBottomUnselected") as Grid;            
            if (dc != null)
            {
                dc.Text = this.Header.ToString();
            }
            ////dc.Foreground = new SolidColorBrush(Colors.Red);            
            ////b.Background = new SolidColorBrush(Colors.Red);
            base.OnApplyTemplate();
            if (txtDotSelected != null)
            {
                if (visibilitytxtDotSelected)
                {
                    txtDotSelected.Visibility = Visibility.Visible;
                }
                else
                {
                    txtDotSelected.Visibility = Visibility.Collapsed;
                }
            }
            if (txtDotUnselected != null)
            {
                if (visibilitytxtDotUnselected)
                {
                    txtDotUnselected.Visibility = Visibility.Visible;
                }
                else
                {
                    txtDotUnselected.Visibility = Visibility.Collapsed;
                }
            }            
        }


        /// <summary>
        /// Gets or sets the Image  value.
        /// </summary>
        /// <value>The icon Brush.</value>
        public Brush Icon
        {
            get
            {
                return (Brush)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Identifies Icon dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register("Icon", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value of the TabItemBackgroundSelected dependency property.
        /// TabItemBackgroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemBackgroundSelected value for the <see cref="CustomTabItem"/>. The default value of the TabItemBackgroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemBackgroundSelected dependency property defines background of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBackgroundSelected property please see <see cref="TabItemBackgroundSelected"/> property example.
        /// </example>
        public Brush TabItemBackgroundSelected
        {
            get
            {
                return (Brush)GetValue(TabItemBackgroundSelectedProperty);
            }

            set
            {
                SetValue(TabItemBackgroundSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemForegroundSelected dependency property.
        /// TabItemForegroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemForegroundSelected value for the <see cref="CustomTabItem"/>. The default value of the TabItemForegroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemForegroundSelected dependency property defines foreground of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemForegroundSelected property please see <see cref="TabItemForegroundSelected"/> property example.
        /// </example>
        public Brush TabItemForegroundSelected
        {
            get
            {
                return (Brush)GetValue(TabItemForegroundSelectedProperty);
            }

            set
            {
                SetValue(TabItemForegroundSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemBackgroundSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemBackgroundSelectedProperty =
        DependencyProperty.Register("TabItemBackgroundSelected", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the tab item font size selected.
        /// </summary>
        /// <value>The tab item font size selected.</value>
        public double TabItemFontSizeSelected
        {
            get
            {
                return (double)GetValue(TabItemFontSizeSelectedProperty);
            }

            set
            {
                SetValue(TabItemFontSizeSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemFontSizeSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemFontSizeSelectedProperty =
       DependencyProperty.Register("TabItemFontSizeSelected", typeof(double), typeof(CustomTabItem), new PropertyMetadata(12d));

        /// <summary>
        /// Identifies TabItemForegroundSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemForegroundSelectedProperty =
       DependencyProperty.Register("TabItemForegroundSelected", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value of the TabItemBackgroundSelected dependency property.
        /// TabItemBackgroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemBackgroundSelected value for the <see cref="CustomTabItem"/>. The default value of the TabItemBackgroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemBackgroundSelected dependency property defines background of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBackgroundSelected property please see <see cref="TabItemBackgroundUnSelected"/> property example.
        /// </example>
        public Brush TabItemBackgroundUnSelected
        {
            get
            {
                return (Brush)GetValue(TabItemBackgroundUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemBackgroundUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemForegroundUnSelected dependency property.
        /// TabItemForegroundUnSelected property is used to store background value for
        /// selected tab control item of the dock window.
        /// </summary>
        /// <remarks>
        /// TabItemForegroundUnSelected dependency property defines foreground of the
        /// selected tab item.
        /// </remarks>
        /// <value>
        /// Type: <see cref="Brush">Brush</see> Provides TabItemForegroundUnSelected value
        /// for the <see cref="DockingManager">DockingManager</see>. The default value of
        /// the TabItemForegroundUnSelected property depends on selected theme or skin.
        /// </value>
        /// <example>
        /// To set TabItemForegroundUnSelected property please see <see
        /// cref="TabItemForegroundUnSelected">SplitterBackground</see> property example.
        /// </example>
        public Brush TabItemForegroundUnSelected
        {
            get
            {
                return (Brush)GetValue(TabItemForegroundUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemForegroundUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemForegroundSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemBackgroundUnSelectedProperty =
        DependencyProperty.Register("TabItemBackgroundUnSelected", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        /// <summary>
        /// Gets or sets the value of the TabItemFontSizeUnSelected dependency property.
        /// TabItemFontSizeUnSelected property is used to store Fontsize value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>The tab item font size un selected.</value>
        public double TabItemFontSizeUnSelected
        {
            get
            {
                return (double)GetValue(TabItemFontSizeUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemFontSizeUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemFontSizeUnSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemFontSizeUnSelectedProperty =
       DependencyProperty.Register("TabItemFontSizeUnSelected", typeof(double), typeof(CustomTabItem), new PropertyMetadata(12d));

        /// <summary>
        /// Identifies TabItemForegroundUnSelected dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemForegroundUnSelectedProperty =
       DependencyProperty.Register("TabItemForegroundUnSelected", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the value of the TabItemInnerBorderThickness dependency property.
        /// TabItemInnerBorderThickness property is used to store Thickness value for
        /// all tab control item of the dock window.
        /// </summary>
        /// <value>The tab item inner border thickness.</value>
        public Thickness TabItemInnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabItemInnerBorderThicknessProperty);
            }

            set
            {
                SetValue(TabItemInnerBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemOuterBorderThickness dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemInnerBorderThicknessProperty =
      DependencyProperty.Register("TabItemInnerBorderThickness", typeof(Thickness), typeof(CustomTabItem), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the value of the TabItemOuterBorderThickness dependency property
        /// TabItemOuterBorderThickness property is used to store Thickness value for all
        /// tab control item of the dock window.
        /// </summary>
        /// <value>The tab item outer border thickness.</value>
        public Thickness TabItemOuterBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabItemOuterBorderThicknessProperty);
            }

            set
            {
                SetValue(TabItemOuterBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemOuterBorderThickness dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemOuterBorderThicknessProperty =
      DependencyProperty.Register("TabItemOuterBorderThickness", typeof(Thickness), typeof(CustomTabItem), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the value of the TabItemInnerBorderBrush dependency property
        /// TabItemInnerBorderBrush property is used to store Color value for all tab
        /// control item of the dock window.
        /// </summary>
        /// <value>The tab item inner border brush.</value>
        public Brush TabItemInnerBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemInnerBorderBrushProperty);
            }

            set
            {
                SetValue(TabItemInnerBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemInnerBorderBrush dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemInnerBorderBrushProperty =
        DependencyProperty.Register("TabItemInnerBorderBrush", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF))));

        /// <summary>
        /// Gets or sets the value of the TabItemOuterBorderBrush dependency property
        /// TabItemOuterBorderBrush property is used to store Color value for all tab
        /// control item of the dock window.
        /// </summary>
        /// <value>The tab item outer border brush.</value>
        public Brush TabItemOuterBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemOuterBorderBrushProperty);
            }

            set
            {
                SetValue(TabItemOuterBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies TabItemOuterBorderBrush dependency property of the <see cref="CustomTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemOuterBorderBrushProperty =
        DependencyProperty.Register("TabItemOuterBorderBrush", typeof(Brush), typeof(CustomTabItem), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9))));        
    }
}
