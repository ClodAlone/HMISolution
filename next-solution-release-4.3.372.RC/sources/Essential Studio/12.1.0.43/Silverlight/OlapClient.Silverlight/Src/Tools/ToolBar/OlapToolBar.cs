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
using System.ComponentModel;
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.Silverlight.Tools.Olap
{
    /// <summary>
    /// ToolBar
    /// </summary>
    [DesignTimeVisible(false)]
    public class OlapToolBar : HeaderedItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapToolBar"/> class.
        /// </summary>
        public OlapToolBar()
        {
            this.DefaultStyleKey = typeof(OlapToolBar);
            this.BorderThickness = new Thickness(0.5);
            this.BorderBrush = (Brush)new SolidColorBrush(Colors.Gray);
        }

        #endregion

        #region Overrided Properties

        /// <summary>
        /// Gets or sets the item that labels the control.
        /// </summary>
        /// <value></value>
        /// <returns>The item that labels the control. The default value is null. </returns>
        public new Object Header { get; private set; }

        /// <summary>
        /// Gets or sets a data template that is used to display the contents of the control's <see cref="P:System.Windows.Controls.HeaderedItemsControl.Header"/>.
        /// </summary>
        /// <value></value>
        /// <returns>Gets or sets a data template that is used to display the contents of the control's header. The default is null.</returns>
        public new DataTemplate HeaderTemplate { get; private set; }

        /// <summary>
        /// Gets or sets a brush that describes the border background of a control.
        /// </summary>
        /// <value></value>
        /// <returns>The brush that is used to fill the control's border; the default is null.</returns>
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(OlapToolBar), new PropertyMetadata((Brush)new SolidColorBrush(Colors.LightGray)));

        #endregion

        #region OverridedMethods

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The container element used to display the specified item.</param>
        /// <param name="item">The content to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="toolTip">The tool tip.</param>
        public void AddItem(Object item, string toolTip)
        {
            this.Items.Add(item);
            if(!string.IsNullOrEmpty(toolTip))
            {
                ToolTipService.SetToolTip(item as DependencyObject, toolTip);
            }
        }

        /// <summary>
        /// Removes the item at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveItemAt(int index)
        {
            if (index > 0 && this.Items.Count > 0 && index < this.Items.Count)
            {
                this.Items.RemoveAt(index);
            }
        }
        #endregion
    }

    /// <summary>
    /// ToolBar Button
    /// </summary>
    [DesignTimeVisible(false)]
    public class OlapToolBarButton:Button
    {
        #region Constructor

        Border hover;

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapToolBarButton"/> class.
        /// </summary>
        public OlapToolBarButton()
        {
            this.DefaultStyleKey = typeof(OlapToolBarButton);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OlapToolBarButton_IsEnabledChanged);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the IsEnabledChanged event of the OlapToolBarButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void OlapToolBarButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                //this.Background = (Brush)new SolidColorBrush(SystemColors.InactiveCaptionColor);
                this.Opacity = 0.4;
            }
            else
            {
                //this.Background = null;
                this.Opacity = 1;
            }
        }

        #endregion

        #region DependencyProperty

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public Object ToolTip
        {
            get { return (Object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(Object), typeof(OlapToolBarButton), new PropertyMetadata(null, (sender, e) =>
                {
                    string s = (string)e.NewValue;
                    if (!string.IsNullOrEmpty(s))
                    {
                        ToolTipService.SetToolTip(sender, s);
                    }
                }));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(OlapToolBarButton), new PropertyMetadata(false,
                (sen, eve) =>
                {
                    OlapToolBarButton toolBarBtn = sen as OlapToolBarButton;
                    if (toolBarBtn != null && toolBarBtn.hover != null)
                    {
                        if (Boolean.Parse(eve.NewValue.ToString())) 
                            toolBarBtn.SetPressBackground();
                        else
                            toolBarBtn.SetLeaveBackground();
                    }
                }
                ));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is toggle.
        /// </summary>
        /// <value><c>true</c> if this instance is toggle; otherwise, <c>false</c>.</value>
        public bool IsToggle
        {
            get { return (bool)GetValue(IsToggleProperty); }
            set { SetValue(IsToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsToggleProperty =
            DependencyProperty.Register("IsToggle", typeof(bool), typeof(OlapToolBarButton), new PropertyMetadata(false));

        #endregion

        #region Overrided methods

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.Button"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.hover = this.GetTemplateChild("hoverBorder") as Border;

            if (this.IsChecked)
            {
                if (this.hover != null)
                {
                    this.hover.BorderBrush = this.GetPressBorder();
                    this.hover.Background = this.GetPressBackground();
                    //this.hover.Background.Opacity = 0.5;
                }
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> event that occurs when the mouse pointer moves while over this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.hover != null && !IsChecked)
            {
                this.hover.BorderBrush = (Brush)new SolidColorBrush(GetColorFromHexaDecimal("#F4D129"));
                this.hover.Background = this.GetHoverBackground(); 
                //this.hover.Background.Opacity = 0.3;
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> routed event that occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeave"/> event.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.hover != null && !IsChecked)
            {
                SetLeaveBackground();
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event that occurs when the left mouse button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (IsToggle)
            {
                IsChecked = !IsChecked;
            }
        }

        /// <summary>
        /// Sets the pressed status background.
        /// </summary>
        private void SetPressBackground()
        {
            this.hover.BorderBrush = this.GetPressBorder();
            this.hover.Background = this.GetPressBackground();
            //this.hover.Background.Opacity = 0.5;
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event that occurs when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.hover != null && !IsChecked)
            {
                SetLeaveBackground();
            }
        }

        /// <summary>
        /// Sets the leave status background.
        /// </summary>
        private void SetLeaveBackground()
        {
            this.hover.BorderBrush = (Brush)new SolidColorBrush(Colors.Transparent);
            this.hover.Background = (Brush)new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Gets the hover background.
        /// </summary>
        /// <returns></returns>
        private Brush GetHoverBackground()
        {
            LinearGradientBrush hoverBrush = new LinearGradientBrush();
            hoverBrush.StartPoint = new Point(0.5, 0);
            hoverBrush.EndPoint = new Point(0.5, 1);
            hoverBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFFBDA"), Offset = 0.022 });
            hoverBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFE8A8"), Offset = 0.499 });
            hoverBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFD767"), Offset = 0.507 });
            hoverBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FCD463"), Offset = 1 });

            return hoverBrush;
        }

        /// <summary>
        /// Gets the pressed status background.
        /// </summary>
        /// <returns></returns>
        private Brush GetPressBackground()
        {
            LinearGradientBrush pressBrush = new LinearGradientBrush();
            pressBrush.StartPoint = new Point(0.5, 0);
            pressBrush.EndPoint = new Point(0.5, 1);
            pressBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#E8A567"), Offset = 0.006 });
            pressBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFA83D"), Offset = 0.298 });
            pressBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FF8D00"), Offset = 0.304 });
            pressBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFC551"), Offset = 0.963 });
            pressBrush.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#FFBC35"), Offset = 1 });

            return pressBrush;
        }

        /// <summary>
        /// Gets the pressed status border.
        /// </summary>
        /// <returns></returns>
        private Brush GetPressBorder()
        {
            LinearGradientBrush pressBorder = new LinearGradientBrush();
            pressBorder.StartPoint = new Point(0.5, 0);
            pressBorder.EndPoint = new Point(0.5, 1);
            pressBorder.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#7B6541"), Offset = 0 });
            pressBorder.GradientStops.Add(new GradientStop { Color = GetColorFromHexaDecimal("#A48D62"), Offset = 0.996 });

            return pressBorder;
        }

        /// <summary>
        /// Gets the color from hexa decimal.
        /// </summary>
        /// <param name="hexaColor">Color of the hexa.</param>
        /// <returns></returns>
        private Color GetColorFromHexaDecimal(string hexaColor)
        {
            return
                Color.FromArgb(
                    Convert.ToByte("ff", 16),
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16));
        }
        #endregion
    }
}
