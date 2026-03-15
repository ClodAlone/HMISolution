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
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Custom Tab Control Class.
    /// </summary>
    public class CustomTabControl :TabControl
    {
        /// <summary>
        /// Represents the Custom Tab Panel.
        /// </summary>
        protected internal CustomTabPanel primitiveTabPanel = null;

        /// <summary>
        /// Gets or sets the related window.
        /// </summary>
        /// <value>The related window.</value>
        protected internal Window RelatedWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the Tab Panel Border.
        /// </summary>
        protected internal Border TabPanelBorder = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTabControl"/> class.
        /// </summary>
        public CustomTabControl()
        {
            this.DefaultStyleKey = typeof(CustomTabControl);
        }

        /// <summary>
        /// Represents the Outer Border.
        /// </summary>
        protected internal Border outerBorder = null;

        /// <summary>
        /// Represents the Content.
        /// </summary>
        protected internal ContentPresenter content = null;

        /// <summary>
        /// It is applying the Own style of the Baseclass TabContol.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            primitiveTabPanel = GetTemplateChild("TabPanelBottom") as CustomTabPanel;
            TabPanelBorder = GetTemplateChild("TabPanelBorder") as Border;
            content = GetTemplateChild("ContentBottom") as ContentPresenter;
            
            if (content != null)
            {
                content.MouseLeftButtonDown += new MouseButtonEventHandler(content_MouseLeftButtonDown);
            }
            if (primitiveTabPanel != null && TabPanelBorder != null)
            {
                if (this.Items.Count <= 1)
                {
                    primitiveTabPanel.Visibility = Visibility.Collapsed;
                    TabPanelBorder.Visibility = Visibility.Collapsed;
                }
                if (RelatedWindow != null)
                {
                    if (WindowContentBorderBrush != RelatedWindow.DockingManager.WindowContentBorderBrush)
                    {
                        WindowContentBorderBrush = RelatedWindow.WindowBorderBrush;
                    }
                    if (TabPanelBackground != RelatedWindow.DockingManager.TabPanelBackground)
                    {
                        TabPanelBackground = RelatedWindow.DockingManager.TabPanelBackground;
                    }
                }
                 
                TabPanelBorder.BorderBrush = WindowContentBorderBrush;
                primitiveTabPanel.Background = TabPanelBackground; 
                if (this.RelatedWindow != null)
                {
                    if (this.RelatedWindow.DockState == DockState.AutoHidden)
                    {
                        primitiveTabPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            this.SelectionChanged += new SelectionChangedEventHandler(CustomTabControl_SelectionChanged);            
        }

        /// <summary>
        /// Handles the SelectionChanged event of the CustomTabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void CustomTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem != null && e.RemovedItems != null && e.RemovedItems.Count > 0 && (e.RemovedItems[0] as CustomTabItem).OwnWindow.DockingManager._tabisDragging == false)
            {
                (this.SelectedItem as CustomTabItem).OwnWindow.Caption = (this.SelectedItem as CustomTabItem).Header.ToString();
                (this.SelectedItem as CustomTabItem).OwnWindow.DockingManager.ActiveWindow = (this.SelectedItem as CustomTabItem).OwnWindow;
            }
            else if (this.SelectedItem != null && e.RemovedItems == null)
            {
                (this.SelectedItem as CustomTabItem).OwnWindow.Caption = (this.SelectedItem as CustomTabItem).Header.ToString();
                (this.SelectedItem as CustomTabItem).OwnWindow.DockingManager.ActiveWindow = (this.SelectedItem as CustomTabItem).OwnWindow;
            }
            else if (this.SelectedItem != null && e.AddedItems != null && e.AddedItems.Count > 0 && (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager._tabisDragging == true)
            {
                CustomTabControl cstab = null;
                Window oldWindow = null;
                if ((e.AddedItems[0] as CustomTabItem).OwnWindow != null && (FrameworkElement)((e.AddedItems[0] as CustomTabItem).OwnWindow).WindowChildElement != null && ((FrameworkElement)((e.AddedItems[0] as CustomTabItem).OwnWindow).WindowChildElement).Parent != null && ((FrameworkElement)((e.AddedItems[0] as CustomTabItem).OwnWindow).WindowChildElement).Parent.GetType() == typeof(CustomTabItem))
                {
                    cstab = ((CustomTabItem)((FrameworkElement)((e.AddedItems[0] as CustomTabItem).OwnWindow).WindowChildElement).Parent).Parent as CustomTabControl;
                    if (cstab != null && cstab.Items.Count>1)
                    {
                        oldWindow = (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.GetWindow(cstab); ;
                    }
                    else
                    {
                        oldWindow = (e.AddedItems[0] as CustomTabItem).OwnWindow;
                    }
                }
                if (oldWindow != null && cstab != null)
                {
                    if (oldWindow.DockState == DockState.AutoHidden)
                    {
                        oldWindow.AutoHide = true;
                        (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.timer.Start();
                    }
                    if (oldWindow.DockState == DockState.Float || oldWindow.DockState == DockState.Hidden)
                    {
                        oldWindow.HeaderBackgroud = (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.FloatWindowHeaderBackground;
                        oldWindow.CaptionForeGround = (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.CaptionForeGround;
                    }
                    else
                    {
                        oldWindow.HeaderBackgroud = (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.HeaderBackground;
                        oldWindow.CaptionForeGround = (e.AddedItems[0] as CustomTabItem).OwnWindow.DockingManager.CaptionForeGround;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the content control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void content_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        /// <summary>
        /// Des the attach event.
        /// </summary>
        protected internal void DeAttachEvent()
        {
            if (content != null)
            {
                content.MouseLeftButtonDown -= new MouseButtonEventHandler(content_MouseLeftButtonDown);
                this.SelectionChanged -= new SelectionChangedEventHandler(CustomTabControl_SelectionChanged);
                TabPanelBackground = null;
                WindowBackground = null;
                WindowContentBorderBrush = null;
            }
        }

        //public new event EventHandler MouseLeftButtonDown;

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    if (MouseLeftButtonDown != null)
        //    {
        //        MouseLeftButtonDown(this, e);
        //    }
        //}

        /// <summary>
        /// Gets or sets the Background of the TabPanel.
        /// </summary>
        /// <value>
        /// Default value is Brushes.White.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush TabPanelBackground
        {
            get
            {
                return (Brush)GetValue(TabPanelBackgroundProperty);
            }

            set
            {
                SetValue(TabPanelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabPanelBackground"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty TabPanelBackgroundProperty =
      DependencyProperty.Register("TabPanelBackground", typeof(Brush), typeof(CustomTabControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the Background of the window.
        /// </summary>
        /// <value>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush WindowBackground
        {
            get
            {
                return (Brush)GetValue(WindowBackgroundProperty);
            }

            set
            {
                SetValue(WindowBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowBackground"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowBackgroundProperty =
      DependencyProperty.Register("WindowBackground", typeof(Brush), typeof(CustomTabControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the BorderBrush of the window content.
        /// </summary>
        /// <value>
        /// Default value is Brushes.Black.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush WindowContentBorderBrush
        {
            get
            {
                return (Brush)GetValue(WindowContentBorderBrushProperty);
            }

            set
            {
                SetValue(WindowContentBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBorderBrush"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentBorderBrushProperty =
      DependencyProperty.Register("WindowContentBorderBrush", typeof(Brush), typeof(CustomTabControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));




        /// <summary>
        /// Gets or sets the window content conrner radius.
        /// </summary>
        /// <value>The window content conrner radius.</value>
        public CornerRadius WindowContentConrnerRadius
        {
            get
            {
                return (CornerRadius)GetValue(WindowContentConrnerRadiusProperty);
            }

            set
            {
                SetValue(WindowContentConrnerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBorderBrush"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentConrnerRadiusProperty =
      DependencyProperty.Register("WindowContentConrnerRadius", typeof(CornerRadius), typeof(CustomTabControl), new PropertyMetadata(new CornerRadius(0)));


        
        /// <summary>
        /// Gets or sets the Thickness of the window Content Border.
        /// </summary>
        /// <value>
        /// Default Content Border Thickness is 1.
        /// </value>
        /// <seealso cref="Brush"/>
        public Thickness WindowContentBorderThickness
        {
            get
            {
                return (Thickness)GetValue(WindowContentBorderThicknessProperty);
            }

            set
            {
                SetValue(WindowContentBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBorderThickness"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentBorderThicknessProperty =
      DependencyProperty.Register("WindowContentBorderThickness", typeof(Thickness), typeof(CustomTabControl), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the BackGround of the window Content.
        /// </summary>
        /// <value>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush WindowContentBackground
        {
            get
            {
                return (Brush)GetValue(WindowContentBackgroundProperty);
            }

            set
            {
                SetValue(WindowContentBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBackground"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentBackgroundProperty =
      DependencyProperty.Register("WindowContentBackground", typeof(Brush), typeof(CustomTabControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the Margin of the window Content.
        /// </summary>
        /// <value>
        /// Default Thickness is 0.
        /// </value>
        /// <seealso cref="Brush"/>
        public Thickness WindowContentMargin
        {
            get
            {
                return (Thickness)GetValue(WindowContentMarginProperty);
            }

            set
            {
                SetValue(WindowContentMarginProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentMargin"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentMarginProperty =
      DependencyProperty.Register("WindowContentMargin", typeof(Thickness), typeof(CustomTabControl), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Gets or sets the value of the TabPanelBorderBrush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabPanelBorderBrush value for the <see cref="DockingManager"/>. The default value of the TabPanelBorderBrush property is Transparent.
        /// </value>
        /// <remarks>
        /// TabPanelBorderBrush dependency property defines border brush of the tab panel. 
        /// </remarks>
        /// <example>
        /// To set TabPanelBorderBrush property please see <see cref="TabPanelBorderBrush"/> property example.
        /// </example>
        public Brush TabPanelBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabPanelBorderBrushProperty);
            }

            set
            {
                SetValue(TabPanelBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabPanelBackground"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty TabPanelBorderBrushProperty =
     DependencyProperty.Register("TabPanelBackground", typeof(Brush), typeof(CustomTabControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        
        /// <summary>
        /// Gets or sets the value of the TabPanelBorderThickness dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Provides TabPanelBorderThickness value for the <see cref="DockingManager"/>. The default value of the TabPanelBorderThickness property is 0.
        /// </value>
        /// <remarks>
        /// TabPanelBorderThickness dependency property defines border thickness of the tab panel. 
        /// </remarks>
        /// <example>
        /// To set TabPanelBorderThickness property please see <see cref="TabPanelBorderThickness"/> property example.
        /// </example>
        public Thickness TabPanelBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabPanelBorderThicknessProperty);
            }

            set
            {
                SetValue(TabPanelBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabPanelBorderThickness"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty TabPanelBorderThicknessProperty =
     DependencyProperty.Register("TabPanelBorderThickness", typeof(Thickness), typeof(CustomTabControl), new PropertyMetadata(new Thickness(0)));
       
    }
}
