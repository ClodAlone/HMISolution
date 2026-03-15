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
using System.Linq;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ZoomingToolKit
    /// </summary>
    public class ZoomingToolKit : Control, IDisposable
    {
        #region dependency properties
        /// <summary>
        /// Identifies the ZoomingToolkitVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomingToolkitVisibilityProperty =
                DependencyProperty.RegisterAttached("ZoomingToolkitVisibility", typeof(Visibility), typeof(ZoomingToolKit), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the ZoomInButtonVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomInButtonVisibilityProperty =
                DependencyProperty.RegisterAttached("ZoomInButtonVisibility", typeof(Visibility), typeof(ZoomingToolKit), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the ZoomOutButtonVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomOutButtonVisibilityProperty =
                DependencyProperty.RegisterAttached("ZoomOutButtonVisibility", typeof(Visibility), typeof(ZoomingToolKit), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the ZoomCloseButtonVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomCloseButtonVisibilityProperty =
                DependencyProperty.RegisterAttached("ZoomCloseButtonVisibility", typeof(Visibility), typeof(ZoomingToolKit), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the ZoomResetButtonVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomResetButtonVisibilityProperty =
                DependencyProperty.RegisterAttached("ZoomResetButtonVisibility", typeof(Visibility), typeof(ZoomingToolKit), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies Area Dependency Property
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(ChartArea), typeof(ZoomingToolKit), new PropertyMetadata(null));

        #endregion

        #region Implementation
        /// <summary>
        /// Gets the value of the ZoomResetButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <returns>Returns the Visibility value</returns>
        public static Visibility GetZoomResetButtonVisibility(UIElement obj)
        {
            return (Visibility)obj.GetValue(ZoomResetButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the ZoomResetButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <param name="value">The Visibility value</param>
        public static void SetZoomResetButtonVisibility(UIElement obj, Visibility value)
        {
            obj.SetValue(ZoomResetButtonVisibilityProperty, value);
            ChartArea area = obj as ChartArea;
            if (area.ZoomTool != null)
            {
                area.ZoomTool.SetValue(ZoomResetButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the ZoomCloseButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <returns>Returns the Visibility value</returns>
        public static Visibility GetZoomCloseButtonVisibility(UIElement obj)
        {
            return (Visibility)obj.GetValue(ZoomCloseButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the ZoomCloseButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <param name="value">The Visibility value</param>
        public static void SetZoomCloseButtonVisibility(UIElement obj, Visibility value)
        {
            obj.SetValue(ZoomCloseButtonVisibilityProperty, value);
            ChartArea area = obj as ChartArea;
            if (area.ZoomTool != null)
            {
                area.ZoomTool.SetValue(ZoomCloseButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the ZoomOutButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <returns>Returns the Visibility value</returns>
        public static Visibility GetZoomOutButtonVisibility(UIElement obj)
        {
            return (Visibility)obj.GetValue(ZoomOutButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the ZoomOutButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <param name="value">The Visibility value</param>
        public static void SetZoomOutButtonVisibility(UIElement obj, Visibility value)
        {
            obj.SetValue(ZoomOutButtonVisibilityProperty, value);
            ChartArea area = obj as ChartArea;
            if (area.ZoomTool != null)
            {
                area.ZoomTool.SetValue(ZoomOutButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the ZoomingToolkitVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <returns>Returns the Visibility value</returns>
        public static Visibility GetZoomingToolkitVisibility(UIElement obj)
        {
            return (Visibility)obj.GetValue(ZoomingToolkitVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the ZoomingToolkitVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <param name="value">The Visibility value</param>
        public static void SetZoomingToolkitVisibility(UIElement obj, Visibility value)
        {
            ChartArea area = obj as ChartArea;
            if (area != null && area.toolkitvisible != value && !area.isinternalinitialize)
            {
                area.toolkitvisible = value;
            }

            if (area != null && area.isSwitchzoom)
            {
                obj.SetValue(ZoomingToolkitVisibilityProperty, value);
                if (area.ZoomTool != null)
                {
                    area.ZoomTool.SetValue(ZoomingToolkitVisibilityProperty, value);
                    area.IsZoomAllAxes = (Visibility)value == Visibility.Visible ? true : false;
                }
            }
        }

        /// <summary>
        /// Gets the value of the ZoomInButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <returns>Returns the Visibility value</returns>
        public static Visibility GetZoomInButtonVisibility(UIElement obj)
        {
            return (Visibility)obj.GetValue(ZoomInButtonVisibilityProperty);
        }

        /// <summary>
        /// Sets the value of the ZoomInButtonVisibility attached dependency property.
        /// </summary>
        /// <param name="obj">The UIElement obj</param>
        /// <param name="value">The Visibility value</param>
        public static void SetZoomInButtonVisibility(UIElement obj, Visibility value)
        {
            obj.SetValue(ZoomInButtonVisibilityProperty, value);
            ChartArea area = obj as ChartArea;
            if (area.ZoomTool != null)
            {
                area.ZoomTool.SetValue(ZoomInButtonVisibilityProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Get or Set Area property
        /// </summary>
        public ChartArea Area
        {
            get { return (ChartArea)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }

        private ChartArea m_area = null;
        internal ChartArea ParentArea
        {
            get
            {
                if (m_area == null)
                {
                    m_area = this.GetParentArea();
                }

                return m_area;
            }
        }

        internal ChartArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is ChartArea))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as ChartArea;
            }

            return null;
        }

        /// <summary>
        /// Called when instance created for ZoomingToolKit
        /// </summary>
        public ZoomingToolKit()
        {
            DefaultStyleKey = typeof(ZoomingToolKit);
        }

        Button zoomin, zoomout, zoomclose, zoomreset, zoompan;

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            zoomout = GetTemplateChild("ZoomOut") as Button;
            zoomin = GetTemplateChild("ZoomIn") as Button;
            zoomclose = GetTemplateChild("ZoomClose") as Button;
            zoomreset = GetTemplateChild("ZoomReset") as Button;
            zoompan = GetTemplateChild("ZoomPan") as Button;
            if (zoomin != null)
            {
                zoomin.MouseEnter += new MouseEventHandler(zoomin_MouseEnter);
                zoomin.MouseLeave += new MouseEventHandler(zoomin_MouseLeave);
                zoomin.Click += new RoutedEventHandler(zoomin_Click);
            }

            if (zoomout != null)
            {
                zoomout.MouseEnter += new MouseEventHandler(zoomout_MouseEnter);
                zoomout.MouseLeave += new MouseEventHandler(zoomout_MouseLeave);
                zoomout.Click += new RoutedEventHandler(zoomout_Click);
            }

            if (zoomclose != null)
            {
                zoomclose.MouseEnter += new MouseEventHandler(zoomclose_MouseEnter);
                zoomclose.MouseLeave += new MouseEventHandler(zoomclose_MouseLeave);
                zoomclose.Click += new RoutedEventHandler(zoomclose_Click);
            }

            if (zoomreset != null)
            {
                zoomreset.MouseEnter += new MouseEventHandler(zoomreset_MouseEnter);
                zoomreset.MouseLeave += new MouseEventHandler(zoomreset_MouseLeave);
                zoomreset.Click += new RoutedEventHandler(zoomreset_Click);
            }

            if (zoompan != null)
            {
                zoompan.MouseEnter += new MouseEventHandler(zoompan_MouseEnter);
                zoompan.MouseLeave += new MouseEventHandler(zoompan_MouseLeave);
                zoompan.Click += new RoutedEventHandler(zoompan_Click);
            }

            if (ParentArea != null)
            {
                Area = ParentArea;
                this.SetValue(ZoomCloseButtonVisibilityProperty, (Visibility)Area.GetValue(ZoomCloseButtonVisibilityProperty));
                this.SetValue(ZoomInButtonVisibilityProperty, (Visibility)Area.GetValue(ZoomInButtonVisibilityProperty));
                this.SetValue(ZoomOutButtonVisibilityProperty, (Visibility)Area.GetValue(ZoomOutButtonVisibilityProperty));
                this.SetValue(ZoomResetButtonVisibilityProperty, (Visibility)Area.GetValue(ZoomResetButtonVisibilityProperty));
                this.SetValue(ZoomingToolkitVisibilityProperty, (Visibility)Area.GetValue(ZoomingToolkitVisibilityProperty));
            }
        }

        void zoompan_Click(object sender, RoutedEventArgs e)
        {
            ChartArea area = this.ParentArea;
            if (area != null)
            {
                if (area.IsPanning == false)
                {
                    area.Cursor = Cursors.Hand;
                    area.IsPanning = true;
                }
                else
                {
                    area.IsPanning = false;
                    area.Cursor = Cursors.Arrow;
                }
            }
        }

        void zoompan_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomPanMouseLeave", true);   
        }

        void zoompan_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomPanMouseEnter", true);
        }

        void zoomreset_Click(object sender, RoutedEventArgs e)
        {
            //List<int> axiszoomfactorindexes = new List<int>();
            //int axisindex = 0;
            ChartArea area = this.ParentArea;
            if (area != null && area.isSwitchzoom == true)
            {
                area.IsPanning = false;
                area.Cursor = Cursors.Arrow;
                area.ZoomResetCommand();
                //foreach (ChartAxis axis in area.Axes)
                //{
                //    if (double.IsNaN(axis.ZoomRange.Start) == false)
                //    {
                //        axiszoomfactorindexes.Add(axisindex);
                //    }

                //    axisindex++;
                //}

                //foreach (int index in axiszoomfactorindexes)
                //{
                //    area.Axes[index].ZoomFactor = 1;
                //    area.Axes[index].IsAutoSetRange = area.Axes[index].zoomisautosetrange;
                //    area.Axes[index].Range = area.Axes[index].ZoomRange;
                //    area.Axes[index].m_visibleInterval = area.Axes[index].ZoomInterval;
                //    area.Axes[index].ActualInterval = area.Axes[index].ZoomInterval;
                //    if (area.HorizontalBar != null)
                //    {
                //        area.HorizontalBar.ViewportSize = double.MaxValue;
                //    }

                //    if (area.VerticalBar != null)
                //    {
                //        area.VerticalBar.ViewportSize = double.MaxValue;
                //    }
                //}

                //area.LoadArea();
            }
        }

        void zoomclose_Click(object sender, RoutedEventArgs e)
        {
            if (ParentArea != null)
            {
                ParentArea.IsPanning = false;
                ParentArea.Cursor = Cursors.Arrow;
                ZoomingToolKit.SetZoomingToolkitVisibility(ParentArea, Visibility.Collapsed);
                ParentArea.isSwitchzoom = false;
            }
        }

        void zoomout_Click(object sender, RoutedEventArgs e)
        {
            //List<int> axiszoomfactorindexes = new List<int>();
            //int axisindex = 0;
            ChartArea area = this.ParentArea;
            if (area != null && area.isSwitchzoom == true)
            {
                area.IsPanning = false;
                area.Cursor = Cursors.Arrow;
                area.ZoomOutCommand();
                //foreach (ChartAxis axis in area.Axes)
                //{
                //    if (axis.EnableZooming == true)
                //    {
                //        axiszoomfactorindexes.Add(axisindex);
                //    }

                //    axisindex++;
                //}

                //foreach (int index in axiszoomfactorindexes)
                //{
                //    if (area.Axes[index].ZoomFactor * 2 <= 1)
                //    {
                //        area.Axes[index].ZoomFactor *= 2;
                //    }
                //    else
                //    {
                //        area.Axes[index].ZoomFactor = 1;
                //    }
                //}
            }
        }

        void zoomin_Click(object sender, RoutedEventArgs e)
        {
            //List<int> axiszoomfactorindexes = new List<int>();
            //int axisindex = 0;
            ChartArea area = this.ParentArea;
            if (area != null && area.isSwitchzoom == true)
            {
                area.IsPanning = false;
                area.Cursor = Cursors.Arrow;
                area.ZoomInCommand();
                //foreach (ChartAxis axis in area.Axes)
                //{
                //    if (axis.EnableZooming == true)
                //    {
                //        axiszoomfactorindexes.Add(axisindex);
                //    }

                //    axisindex++;
                //}

                //foreach (int index in axiszoomfactorindexes)
                //{
                //    if (area.Axes[index].ZoomFactor / 2 >= 0.001)
                //    {
                //        area.Axes[index].ZoomFactor /= 2;
                //    }
                //    else
                //    {
                //        area.Axes[index].ZoomFactor = 0.001;
                //    }
                //}
            }
        }

        void zoomreset_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomResetMouseLeave", true);
        }

        void zoomreset_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomResetMouseEnter", true);
        }

        void zoomclose_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomCloseMouseLeave", true);
        }

        void zoomclose_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomCloseMouseEnter", true);
        }

        void zoomout_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomOutMouseLeave", true);
        }

        void zoomout_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomOutMouseEnter", true);
        }

        void zoomin_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomInMouseLeave", true);
        }

        void zoomin_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ZoomInMouseEnter", true);
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Area != null)
                this.Area = null;
            if (this.m_area != null)
                this.m_area = null;
            if (this.Area != null)
                this.Area = null;
            if (this.Template != null)
                this.Template = null;
        }

        #endregion
    }
}
