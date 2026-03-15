// <copyright file="ChartGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

   
    /// <summary>
    /// Represents ChartGrid class.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartGrid"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartGrid : UniformGrid
    {
        #region DependencyProperties
        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        /// <seealso cref="Chart"/>
        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartGrid), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnAutoCalcChanged)));

        /// <summary>
        /// Identifies the CalcType dependency property.
        /// </summary>
        public static readonly DependencyProperty CalcTypeProperty =
          DependencyProperty.Register("CalcType", typeof(ChartGridCalcCellType), typeof(ChartGrid), new PropertyMetadata(ChartGridCalcCellType.Auto, new PropertyChangedCallback(OnAutoCalcChanged)));

        /// <summary>
        /// Identifies the AutoRowsCount dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoRowsCountProperty =
          DependencyProperty.Register("AutoRowsCount", typeof(int), typeof(ChartGrid), new PropertyMetadata(1, new PropertyChangedCallback(OnAutoCalcChanged), new CoerceValueCallback(CoerceAutoRowsCount)));

        /// <summary>
        /// Identifies the AutoColumnsCount dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoColumnsCountProperty =
          DependencyProperty.Register("AutoColumnsCount", typeof(int), typeof(ChartGrid), new PropertyMetadata(1, new PropertyChangedCallback(OnAutoCalcChanged), new CoerceValueCallback(CoerceAutoColumnsCount)));

        /// <summary>
        /// Identifies the CalcCoefficient dependency property.
        /// </summary>
        public static readonly DependencyProperty CalcCoefficientProperty =
          DependencyProperty.Register("Calccoefficient", typeof(double), typeof(ChartGrid), new PropertyMetadata(1d, new PropertyChangedCallback(OnAutoCalcChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Orientation
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(ChartGrid.OrientationProperty);
            }

            set
            {
                SetValue(ChartGrid.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ChartGrid CalcCellType
        /// </summary>
        public ChartGridCalcCellType CalcType
        {
            get
            {
                return (ChartGridCalcCellType)GetValue(CalcTypeProperty);
            }

            set
            {
                SetValue(CalcTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the AutoRowsCount
        /// </summary>
        public int AutoRowsCount
        {
            get
            {
                return (int)GetValue(ChartGrid.AutoRowsCountProperty);
            }

            set
            {
                SetValue(ChartGrid.AutoRowsCountProperty, value);
            }
        }

        private static object CoerceAutoRowsCount(DependencyObject d, object value)
        {
            var chartGrid = d as ChartGrid;
            if (chartGrid != null && (int)value < (int)AutoRowsCountProperty.DefaultMetadata.DefaultValue)
            {
                return AutoRowsCountProperty.DefaultMetadata.DefaultValue;
            }
            return value;
        }

        /// <summary>
        /// Gets or sets the AutoColumnsCount
        /// </summary>
        public int AutoColumnsCount
        {
            get
            {
                return (int)GetValue(ChartGrid.AutoColumnsCountProperty);
            }

            set
            {
                SetValue(ChartGrid.AutoColumnsCountProperty, value);
            }
        }

        private static object CoerceAutoColumnsCount(DependencyObject d, object value)
        {
            var chartGrid = d as ChartGrid;
            if (chartGrid != null && (int)value < (int)AutoColumnsCountProperty.DefaultMetadata.DefaultValue)
            {
                return AutoColumnsCountProperty.DefaultMetadata.DefaultValue;
            }
            return value;
        }

        /// <summary>
        /// Gets or sets the CalcCoefficient
        /// </summary>
        public double CalcCoefficient
        {
            get
            {
                return (double)GetValue(ChartGrid.CalcCoefficientProperty);
            }

            set
            {
                SetValue(ChartGrid.CalcCoefficientProperty, value);
            }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_isAutoSetInvalidate
        /// </summary>
        private bool m_isAutoSetInvalidate = true;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartGrid">ChartGrid</see> class. 
        /// </summary>
        public ChartGrid()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Computes the desired size of the <see cref="T:System.Windows.Controls.Primitives.UniformGrid"></see> by measuring all of the child elements.
        /// </summary>
        /// <param name="constraint">The <see cref="T:System.Windows.Size"></see> of the available area for the grid.</param>
        /// <returns>
        /// The desired <see cref="T:System.Windows.Size"></see> based on the child content of the grid and the constraint parameter.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (m_isAutoSetInvalidate && (CalcType == ChartGridCalcCellType.Auto || CalcType == ChartGridCalcCellType.Children))
            {
                Orientation orientation = Orientation;
                double itemsCount = Children.Count;
                foreach (UIElement child in Children)
                {
                    if (child.Visibility == Visibility.Collapsed)
                        itemsCount--;
                }
                if (itemsCount != 0)
                {
                    if (CalcType == ChartGridCalcCellType.Auto)
                    {
                        this.Columns = orientation == Orientation.Horizontal ? (int)Math.Ceiling(itemsCount / AutoRowsCount) : AutoColumnsCount;
                        this.Rows = orientation == Orientation.Vertical ? (int)Math.Ceiling(itemsCount / AutoColumnsCount) : AutoRowsCount;
                    }
                    else
                    {
                        double coef = CalcCoefficient;
                        double a = Math.Ceiling(Math.Sqrt(itemsCount / coef));
                        double b = Math.Ceiling(itemsCount / a);

                        this.Columns = (int)(orientation == Orientation.Horizontal ? a : b);
                        this.Rows = (int)(orientation == Orientation.Vertical ? a : b);
                    }
                }

                m_isAutoSetInvalidate = false;
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"></see> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"></see> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"></see> that was removed from the collection.</param>
        /// <seealso cref="ChartGrid"/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            m_isAutoSetInvalidate = true;

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// Raises on AutoCalcChanged
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">DependencyPropertyChangedEvent arguments e.</param>
        private static void OnAutoCalcChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartGrid cg = d as ChartGrid;

            if (cg != null)
            {
                cg.m_isAutoSetInvalidate = true;
                cg.InvalidateMeasure();
            }
        }
        #endregion
    }


    /// <summary>
    /// Custom panel implementation for UniformWrapPanel
    /// </summary>
    public class UniformWrapPanel : WrapPanel
    {
        /// <summary>
        /// Empty constructor for UniformWrapPanel
        /// </summary>
        public UniformWrapPanel()
        {
        }     
         
        private double m_ItemWidth;

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.WrapPanel"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.WrapPanel.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the desired size of the element.
        /// </returns>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        protected override Size MeasureOverride(Size constraint)
        {
            m_ItemWidth = 0;  
            UIElementCollection internalChildren = base.InternalChildren;           
            foreach (UIElement element in internalChildren)
            {
              if (element != null)
              {
                  element.Measure(constraint);
                  if (m_ItemWidth < element.DesiredSize.Width)
                  m_ItemWidth = element.DesiredSize.Width;
              }
            } 
                      
              return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.WrapPanel"/> element.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.WrapPanel"/> element and its children.
        /// </returns>
        /// <param name="finalSize">The <see cref="T:System.Windows.Size"/> that this element should use to arrange its child elements.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {            
            this.ItemWidth = m_ItemWidth;
            return base.ArrangeOverride(finalSize);
        }  
    }
}
