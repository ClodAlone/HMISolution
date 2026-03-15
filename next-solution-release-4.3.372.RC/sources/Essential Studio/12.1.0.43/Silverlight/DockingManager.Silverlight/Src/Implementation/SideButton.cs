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
    /// Generate the Tabitem in the Side Panel.
    /// </summary>
    public class SideButton:Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SideButton"/> class.
        /// </summary>
        public SideButton()
        {
            this.DefaultStyleKey = typeof(SideButton);
            initial = 0;
        }

        /// <summary>
        /// Stores the ParentGrid of the button.
        /// </summary>
        protected internal Grid layOutRoot = null;

        /// <summary>
        /// Occurs when MouseLeftButtonUp.
        /// </summary>
        public new event EventHandler MouseLeftButtonDown;

        /// <summary>
        /// Occurs when MouseLeftButtonDown.
        /// </summary>
        public new event EventHandler MouseLeftButtonUp;

        /// <summary>
        /// Invoked When MouseLeftButtonUp.
        /// </summary>
        /// <param name="e">MouseLeft Button Event arg.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
            if (MouseLeftButtonDown != null)
            {
                MouseLeftButtonDown(this, e);
            }
        }

        /// <summary>
        /// Invoked when MouseLefButtonDown.
        /// </summary>
        /// <param name="e">Event Argument of MouseLeftButton down.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (MouseLeftButtonUp != null)
            {
                MouseLeftButtonUp(this, e);
            }
        }

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
        /// Gets or sets the ImageBrush for TabIcon .
        /// </summary>
        /// <value>The icon.</value>
        public ImageBrush Icon
        {
            get
            {
                return (ImageBrush)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pinned window.
        /// </summary>
        /// <value>The pinned window.</value>
        protected internal Window PinnedWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial.
        /// </summary>
        /// <value>The initial.</value>
        protected internal int initial
        {
            get;
            set;
        }

       /// <summary>
       /// 
       /// </summary>
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register("Icon", typeof(ImageBrush), typeof(SideButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Brush for SideTabPanel Background .
        /// </summary>
        /// <value>The side items background.</value>
        public Brush SideItemsBackground
        {
            get
            {
                return (Brush)GetValue(SideItemsBackgroundProperty);
            }

            set
            {
                SetValue(SideItemsBackgroundProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SideItemsBackgroundProperty =
          DependencyProperty.Register("SideItemsBackground", typeof(Brush), typeof(SideButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF,0xC1,0xD8,0xF6))));

        /// <summary>
        /// Gets or sets SideTabItems BorderBrush .
        /// </summary>
        /// <value>The side items border brush.</value>
        public Brush SideItemsBorderBrush
        {
            get
            {
                return (Brush)GetValue(SideItemsBorderBrushProperty);
            }

            set
            {
                SetValue(SideItemsBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SideItemsBorderBrushProperty =
          DependencyProperty.Register("SideItemsBorderBrush", typeof(Brush), typeof(SideButton), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets the Thickness of the SideItemBorder Thickness .
        /// </summary>
        /// <value>The side items border thickness.</value>
        public Thickness SideItemsBorderThickness
        {
            get
            {
                return (Thickness)GetValue(SideItemsBorderThicknessProperty);
            }

            set
            {
                SetValue(SideItemsBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SideItemsBorderThicknessProperty =
          DependencyProperty.Register("SideItemsBorderThickness", typeof(Thickness), typeof(SideButton), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the ForGround For SideTabItem .
        /// </summary>
        /// <value>The side items foreground.</value>
        public Brush SideItemsForeground
        {
            get
            {
                return (Brush)GetValue(SideItemsForegroundProperty);
            }

            set
            {
                SetValue(SideItemsForegroundProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SideItemsForegroundProperty =
          DependencyProperty.Register("SideItemsForeground", typeof(Brush), typeof(SideButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.Button"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            layOutRoot = GetTemplateChild("PART_LayOut") as Grid;
            base.OnApplyTemplate();
            this.LayoutUpdated += new EventHandler(SideButton_LayoutUpdated);             
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the SideButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void SideButton_LayoutUpdated(object sender, EventArgs e)
        {
            if (PinnedWindow != null)
            {
                if (PinnedWindow.DockingManager != null)
                {
                    if (PinnedWindow.DockingManager.SideButtonTemplate != null && this.Style == null)
                    {
                        this.Style = PinnedWindow.DockingManager.SideButtonTemplate;
                        this.LayoutUpdated -= new EventHandler(SideButton_LayoutUpdated);
                    }
                }
            } 
        }
    }
}
