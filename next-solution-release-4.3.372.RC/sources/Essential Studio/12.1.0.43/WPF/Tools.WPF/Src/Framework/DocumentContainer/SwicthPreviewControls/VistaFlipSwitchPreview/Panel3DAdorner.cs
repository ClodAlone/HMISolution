// <copyright file="Panel3DAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents adorner for Vista Flip.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Panel3DAdorner : Adorner
    {
        #region Private members
        /// <summary>
        /// Presents ViewportHost
        /// </summary>
        private readonly DockPanel m_ViewportHost = new DockPanel();
        
        /// <summary>
        /// Original point. 
        /// </summary>
        private static readonly Point ORIGIN_POINT = new Point(0, 0);

        /// <summary>
        /// Presents logical children.
        /// </summary>
        private ArrayList m_logicalChildren = null;
        
        /// <summary>
        /// Presents parent size.
        /// </summary>
        private Size m_parentSize = Size.Empty;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Panel3DAdorner"/> class.
        /// </summary>
        /// <param name="adornedPanel3D">The adorned panel3 D.</param>
        /// <param name="viewport">The viewport.</param>
        public Panel3DAdorner(UIElement adornedPanel3D, UIElement viewport)
            : base(adornedPanel3D)
        {
            m_ViewportHost.Children.Add(viewport);
            m_ViewportHost.Background = Brushes.Transparent;
            AddLogicalChild(m_ViewportHost);
            AddVisualChild(m_ViewportHost);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Sets the size of the parent.
        /// </summary>
        /// <value>The size of the parent.</value>
        public Size PerentSize
        {
            set
            {
                m_parentSize = value;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        
        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>An enumerator for logical child elements of this element.</returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (null == m_logicalChildren)
                {
                    m_logicalChildren = new ArrayList { m_ViewportHost };
                }

                return m_logicalChildren.GetEnumerator();
            }
        }
        #endregion

        #region Implemnetation
        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_ViewportHost.Measure(m_parentSize);
            return m_parentSize;
        }
        
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rect = new Rect(ORIGIN_POINT, m_parentSize);
            m_ViewportHost.Arrange(rect);
            return m_parentSize;
        }
        
        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (0 == index)
            {
                return m_ViewportHost;
            }

            throw new ArgumentOutOfRangeException(string.Format("Incorrect index {0}", index));
        }
        #endregion
    }
}