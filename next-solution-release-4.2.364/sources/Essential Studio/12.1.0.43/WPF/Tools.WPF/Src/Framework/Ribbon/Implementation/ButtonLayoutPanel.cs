// <copyright file="ButtonLayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is responsible for layout of RibbonButtons.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ButtonLayoutPanel : Panel
    {
        #region Fields
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Array of separators to display between buttons.
        /// </summary>
        private List<Rectangle> m_separatorArray = new List<Rectangle>();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Brush for separator between buttons.
        /// </summary>
        //SU I78477
        //private static Brush separatorBrush;
        //EU I78477
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ButtonLayoutPanel"/> class.
        /// </summary>
        static ButtonLayoutPanel()
        {
            //// separatorBrush = new SolidColorBrush(Color.FromRgb(180, 204, 232));                         
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="SeparatorBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SeparatorBrushChanged;

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="SeparatorBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(ButtonLayoutPanel), new FrameworkPropertyMetadata(Brushes.Blue, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSeparatorBrushChanged)));

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the separator brush.
        /// </summary>
        /// <value>The separator brush.</value>
        public Brush SeparatorBrush
        {
            get
            {
                return (Brush)GetValue(SeparatorBrushProperty);
            }

            set
            {
                SetValue(SeparatorBrushProperty, value);
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Positions child elements and determines a size for a panel.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            int count = VisualChildrenCount;
            Rect finalRect = new Rect(arrangeSize);
            double width = 0;
            int i = 0;
            while (i < count)
            {
                UIElement element = GetVisualChild(i) as UIElement;
                if (element != null)
                {
                    finalRect.X += width;
                    width = element.DesiredSize.Width;
                    finalRect.Width = width;
                    finalRect.Height = Math.Max(arrangeSize.Height, element.DesiredSize.Height);
                    element.Arrange(finalRect);
                }

                i++;
            }

            return arrangeSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for a panel.
        /// </summary>
        /// <param name="constraint">The available size that this
        /// element can give to the child.
        /// Infinity can be specified as a
        /// value to indicate that the element
        /// will size to whatever content is
        /// available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            InitializeSeparators();

            double width;
            int count = VisualChildrenCount;
            Size size = new Size();
            Size availableSize = constraint;
            availableSize.Width = double.PositiveInfinity;
            width = constraint.Width;
            int i = 0;
            while (i < count)
            {
                UIElement element = GetVisualChild(i) as UIElement;
                if (element != null)
                {
                    double height;
                    element.Measure(availableSize);
                    Size desiredSize = element.DesiredSize;

                    size.Width += desiredSize.Width;
                    size.Height = Math.Max(size.Height, desiredSize.Height);
                    height = desiredSize.Width;
                }

                i++;
            }

            return size;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes separators array.
        /// </summary>
        private void InitializeSeparators()
        {
            int childCount = InternalChildren.Count;
            if (childCount > 1)
            {
                int sepCount = childCount - 1;
                while (m_separatorArray.Count < sepCount)
                {
                    Rectangle rect = new Rectangle();
                    rect.Stroke = SeparatorBrush;
                    rect.Width = 1;
                    rect.Height = 25;
                    rect.Margin = new Thickness(0, -3, 0, -3);
                    AddVisualChild(rect);
                    m_separatorArray.Add(rect);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets visual child by index.
        /// </summary>
        /// <param name="index">The index of the visual object in the
        /// VisualCollection.</param>
        /// <returns>
        /// The child in the VisualCollection at the specified index
        /// value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            int indexBase = (int)Math.Truncate(index / 2d);
            int iSplitter = indexBase;
            bool bSplitter = indexBase * 2 != index;

            if (!bSplitter)
            {
                try
                {
                   return base.GetVisualChild(indexBase);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return m_separatorArray[iSplitter];
            }
        }

        /// <summary>
        /// Called when [separator brush changed].
        /// </summary>
        /// <param name="d">The d dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSeparatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonLayoutPanel instance = (ButtonLayoutPanel)d;
            instance.OnSeparatorBrushChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SeparatorBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSeparatorBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (Rectangle rect in m_separatorArray)
            {
                rect.Stroke = SeparatorBrush;
            }

            if (SeparatorBrushChanged != null)
            {
                SeparatorBrushChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets count of visual children.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = base.VisualChildrenCount;
                return (count > 1) ? ((count * 2) - 1) : count;
            }
        }

        #endregion
    }
}
