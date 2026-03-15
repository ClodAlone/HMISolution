// <copyright file="HostAdornerVS2005.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;
using System.Windows.Media;
using System;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the UI Element Adorner
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class UIElementAdorner : TemplatedAdornerBase
    {
        ///// <summary>
        ///// Specify the content presenter.
        ///// </summary>
        //// private ContentPresenter m_contentPresenter;

        /// <summary>
        /// Specify the UI element.
        /// </summary>
        private List<UIElement> m_uiElements = new List<UIElement>();

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="UIElementAdorner"/> class.
        /// </summary>
        static UIElementAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplatedAdornerInternalControl), new FrameworkPropertyMetadata(typeof(UIElementAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElementAdorner"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="panel">The panel.</param>
        public UIElementAdorner(UIElement element, Panel panel)
            : base(element)
        {
            SkinStorage.SetVisualStyle(this, SkinStorage.GetVisualStyle(element));
            //SkinStorage.SetVisualStylesList(this, SkinStorage.GetVisualStylesList(element));
            InnerControl.Loaded += new RoutedEventHandler(OnInnerControlLoaded);
            InternalPanel = panel;
        }

        /// <summary>
        /// Called when [inner control loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnInnerControlLoaded(object sender, RoutedEventArgs e)
        {
            ContentPresenter presenter = InnerControl.GetTemplateChildInternal("PART_ContentPresenter") as ContentPresenter;
            presenter.Content = InternalPanel;

            foreach (UIElement item in m_uiElements)
            {
                InternalPanel.Children.Add(item);
            }

            m_uiElements.Clear();
        }
        #endregion

        /// <summary>
        /// Gets or sets the internal panel.
        /// </summary>
        /// <value>The internal panel.</value>
        internal Panel InternalPanel
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AddElement(UIElement element)
        {
            InnerControl.ApplyTemplate();
            m_uiElements.Add(element);
        }

        /// <summary>
        /// Removes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void RemoveElement(UIElement element)
        {
            InternalPanel.Children.Remove(element);
        }
    }

    /// <summary>
    /// Represents HostAdornerVS2005 class.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HostAdornerVS2005 : TemplatedAdornerBase
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="HostAdornerVS2005"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public HostAdornerVS2005(UIElement element)
            : base(element)
        {
            SkinStorage.SetVisualStyle(this, SkinStorage.GetVisualStyle(element));
            //SkinStorage.SetVisualStylesList(this, SkinStorage.GetVisualStylesList(element));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Side of the <see cref="HostAdornerVS2005"/>.
        /// </summary>
        /// <value>The dock side.</value>
        public DockSide Side
        {
            get
            {
                return (DockSide)GetValue(SideProperty);
            }

            set
            {
                SetValue(SideProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }

            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        /// <value>The border thickness.</value>
        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }

            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab area border thickness.
        /// </summary>
        /// <value>The tab area border thickness.</value>
        public Thickness TabAreaBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabAreaBorderThicknessProperty);
            }

            set
            {
                SetValue(TabAreaBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab margin.
        /// </summary>
        /// <value>The tab margin.</value>
        public Thickness TabAreaTopMargin
        {
            get
            {
                return (Thickness)GetValue(TabAreaTopMarginProperty);
            }

            set
            {
                SetValue(TabAreaTopMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the tab area.
        /// </summary>
        /// <value>The width of the tab area.</value>
        public double TabAreaWidth
        {
            get
            {
                return (double)GetValue(TabAreaWidthProperty);
            }

            set
            {
                SetValue(TabAreaWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets TabStripPlacement of the <see cref="HostAdornerVS2005"/>.
        /// </summary>
        /// <value>The tab strip placement.</value>
        public Dock TabStripPlacement
        {
            get
            {
                return (Dock)GetValue(TabStripPlacementProperty);
            }

            set
            {
                SetValue(TabStripPlacementProperty, value);
            }
        }
        #endregion

        #region DependencyProperties

        /// <summary>
        /// Identifies HostAdornerVS2005.TabAreaTopMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty TabAreaTopMarginProperty =
            DependencyProperty.Register("TabAreaTopMargin", typeof(Thickness), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(new Thickness(7, 0, 0, 0)));

        /// <summary>
        /// Identifies HostAdornerVS2005.TabAreaWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty TabAreaWidthProperty =
            DependencyProperty.Register("TabAreaWidth", typeof(double), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(44d));

        /// <summary>
        /// Identifies HostAdornerVS2005.Side dependency property.
        /// </summary>
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(DockSide), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(DockSide.None));

        /// <summary>
        /// Identifies HostAdornerVS2005.Background dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(Convert.ToByte("7F", 16), Convert.ToByte("5F", 16), Convert.ToByte("AF", 16), Convert.ToByte("FF", 16)))));

        /// <summary>
        /// Identifies HostAdornerVS2005.BorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(Convert.ToByte("7F", 16), Convert.ToByte("5F", 16), Convert.ToByte("AF", 16), Convert.ToByte("FF", 16)))));

        /// <summary>
        /// Identifies HostAdornerVS2005.BorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies HostAdornerVS2005.TabAreaBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty TabAreaBorderThicknessProperty =
            DependencyProperty.Register("TabAreaBorderThickness", typeof(Thickness), typeof(HostAdornerVS2005), new FrameworkPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies HostAdornerVS2005.TabStripPlacement dependency property.
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(HostAdornerVS2005));
        #endregion
    }
}
