// <copyright file="ContextAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used for reflecting of ContextTabGroup. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ContextAdorner : TemplatedAdornerBase
    {
        #region	Private	members
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Cached value of the TestProperty property. 
        /// </summary>
        private ContextTabGroup m_contextTabGroup = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Time when the last click on ContextTab was performed.
        /// </summary>
        private DateTime m_lastContextTabClick;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Point where the last click on ContextTab bar was performed.
        /// </summary>
        private Point m_lastContextTabPoint;
        #endregion

        #region Initialization
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextAdorner"/> class..
        /// </summary>
        /// <param name="adornedTab">Tab which will be adorned.</param>
        public ContextAdorner(RibbonTab adornedTab)
            : base(adornedTab)
        {
            CoerceValue(ContextTabGroupProperty);
            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            //IsHitTestVisible = false;           
        }
        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Computes the desired size of the UniformGrid by measuring all
        /// of the child elements.
        /// </summary>
        /// <param name="constraint">The Size of the available area for
        /// the grid.</param>
        /// <returns>
        /// The desired Size based on the child content of the grid and
        /// the constraint parameter.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            InnerControl.Measure(constraint);
            if ((SkinStorage.GetVisualStyle(this) == "Office2010Blue") || (SkinStorage.GetVisualStyle(this) == "Office2010Black") || (SkinStorage.GetVisualStyle(this) == "Office2010Silver"))
                this.SizeChanged += new SizeChangedEventHandler(ContextAdorner_SizeChanged);
            return InnerControl.DesiredSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContextAdorner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double temp = 0;
            foreach (RibbonTab ribbonTab in m_contextTabGroup.ContextAdorner.ContextTabGroup.RibbonTabs)
            {
                temp += ribbonTab.ActualWidth;
            }
            if (m_contextTabGroup.ContextAdorner.ActualWidth < temp)
            {
                Border bd = VisualUtils.FindDescendant(m_contextTabGroup.ContextAdorner, typeof(Border)) as Border;
                Border bd1 = null;
                if (bd != null)
                    bd1 = VisualUtils.FindDescendant(bd, typeof(Border)) as Border;
                if (bd1 != null)
                    bd1.BorderThickness = new Thickness(1, 0, 0, 0);
            }
            else
            {
                Border bd = VisualUtils.FindDescendant(m_contextTabGroup.ContextAdorner, typeof(Border)) as Border;
                Border bd1 = null;
                if (bd != null)
                    bd1 = VisualUtils.FindDescendant(bd, typeof(Border)) as Border;
                if (bd1 != null)
                    bd1.BorderThickness = new Thickness(1, 0, 1, 0);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnContextTabGroupChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnContextTabGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextAdorner instance = (ContextAdorner)d;
            instance.OnContextTabGroupChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises ContextTabGroupChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnContextTabGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            m_contextTabGroup = (ContextTabGroup)e.NewValue;
            m_contextTabGroup.ContextAdorner = this;
            if (ContextTabGroupChanged != null)
            {
                ContextTabGroupChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Coerces ContextTabGroup property.
        /// </summary>
        /// <param name="d">ContextAdorner instance.</param>
        /// <param name="baseValue">Base value.</param>
        /// <returns>
        /// ContextTabGroup of adorned tab.
        /// </returns>
        private static object CoerceContextTabGroupProperty(DependencyObject d, object baseValue)
        {
            ContextAdorner ca = (ContextAdorner)d;

            return (ca.InnerControl.AdornedElement as RibbonTab).ContextTabGroup;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            RibbonWindow ribbonWindow = (RibbonWindow)VisualUtils.FindAncestor(this, typeof(RibbonWindow));

            Point position = e.GetPosition(this);
            if (ribbonWindow != null)
            {
                if (((DateTime.Now.Subtract(m_lastContextTabClick).TotalMilliseconds < System.Windows.Forms.SystemInformation.DoubleClickTime) && (Math.Abs((double)(m_lastContextTabPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastContextTabPoint.Y - position.Y)) <= 2))
                {
                    if (ribbonWindow.WindowState != WindowState.Maximized)
                    {
                        ribbonWindow.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        ribbonWindow.WindowState = WindowState.Normal;                   
                    }
                }
                else
                {
                    m_lastContextTabPoint = e.GetPosition(this);
                    ribbonWindow.DragMove();
                }

                m_lastContextTabClick = DateTime.Now;
            }
            else
            {
                Debug.WriteLine("Main Window can not be found");
            }
        }
        #endregion

        #region Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the ContextTabGroup dependency
        /// property. 
        /// </summary>
        public ContextTabGroup ContextTabGroup
        {
            get
            {
                return m_contextTabGroup;
            }

            protected set
            {
                SetValue(ContextTabGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets the template inner control.
        /// </summary>
        /// <value>The template inner control.</value>
        protected internal Control TemplatedInnerControl
        {
            get
            {
                return base.InnerControl;
            }
        }
        #endregion

        #region Events
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when ContextTabGroup property is
        /// changed. 
        /// </summary>
        public event PropertyChangedCallback ContextTabGroupChanged;
        #endregion

        #region Dependency properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Identifies ContextAdorner. ContextTabGroup dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty ContextTabGroupProperty =
            DependencyProperty.Register("ContextTabGroup", typeof(ContextTabGroup), typeof(ContextAdorner), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContextTabGroupChanged), CoerceContextTabGroupProperty));
        #endregion
    }
}
