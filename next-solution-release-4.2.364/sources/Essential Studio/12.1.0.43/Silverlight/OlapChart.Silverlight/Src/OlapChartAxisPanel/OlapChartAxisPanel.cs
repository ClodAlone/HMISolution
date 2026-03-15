#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Chart;
using System.Windows.Controls.Primitives;
using System;
using Syncfusion.OlapSilverlight.Engine;
using System.Diagnostics;

namespace Syncfusion.Silverlight.Chart.Olap
{
    public class OlapChartAxisPanel : ChartAxisElementPanel
    {
        #region Members
        
        private OlapArea _olapArea = null;
        private OlapLabelPanel _olapLablePanel = null;
        private Popup headerPopup = null;

        #endregion

        #region Measure and Arrange

        protected override Size ArrangeOverride(Size finalSize)
        {
            Thickness axisThickness = this.ParentArea.AxesThickness;

            foreach (UIElement uielement in this.Children)
            {
                if (uielement is OlapLabelPanel)
                {
                    uielement.Arrange(new Rect(0, 0, uielement.DesiredSize.Width, uielement.DesiredSize.Height));
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (headerPopup == null)
            {
                headerPopup = OlapArea.resource["HeaderToolTipPopup"] as Popup;
            }

            headerPopup.IsOpen = false;

            if (this.ParentArea.ChartControl.ChartType != Syncfusion.Windows.Chart.ChartTypes.Pie ||
                this.ParentArea.ChartControl.ChartType != Syncfusion.Windows.Chart.ChartTypes.Funnel ||
                this.ParentArea.ChartControl.ChartType != ChartTypes.Pyramid ||
                this.ParentArea.ChartControl.ChartType != ChartTypes.Polar ||
                this.ParentArea.ChartControl.ChartType != ChartTypes.Radar)
            {
                //// Check if OlapLabelPanel already exist
                var isPanelExist = false;
                _olapArea.UpdatePanel = true;
                      if ((GetOlapLabelPanelOrientation(this._olapArea.ChartControl.ChartType) == Orientation.Horizontal && !this._olapArea.isNonPanelChartType) || (this._olapArea.CurrentEngine!=null && !this._olapArea.CurrentEngine.IsOLAP))
                      {                          
                          //// Check if OlapLabelPanel already exist
                         isPanelExist = !this._olapArea.UpdatePanel;
                          this._olapArea.UpdatePanel = false;
                         this._olapArea.isNonPanelChartType = false;                    
                      }
                      else
                      {
                          isPanelExist = false;

                          foreach (var child in this.Children)
                          {
                              if (child is OlapLabelPanel)
                              {
                                  isPanelExist = true;
                              }
                          }
                      }
               


                //// if not available then add as a children
                if (!isPanelExist)
                {
                    this._olapLablePanel = new OlapLabelPanel();
                    this._olapLablePanel.MouseMove += new System.Windows.Input.MouseEventHandler(_olapLablePanel_MouseMove);
                    this._olapLablePanel.MouseLeave += new System.Windows.Input.MouseEventHandler(_olapLablePanel_MouseLeave);

                    if (this._olapArea != null)
                    {
                        if (this._olapArea.ChartControl != null)
                        {
                            this._olapLablePanel.Orientation = GetOlapLabelPanelOrientation(this._olapArea.ChartControl.ChartType);
                            this._olapLablePanel.LabelRotationAngle = this.ParentArea.ChartControl.PrimaryAxisLabelRoatationAngle;
                            this._olapLablePanel.PrimaryAxisLabelDateTimeFormat = this._olapArea.PrimaryAxis.LabelDateTimeFormat;
                            //// Updating chart axis panel properties
                            this.SetBinding(GridLineBrushProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("GridLineStroke") });
                            this.SetBinding(LabelBackgroundProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("LabelBackground") });
                            this.SetBinding(LabelForegroundProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("LabelForeground") });
                            this.SetBinding(LabelFontSizeProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("LabelFontSize") });
                            this.SetBinding(LabelFontStyleProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("FontStyle") });
                            this.SetBinding(ExpanderStyleProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("ExpanderStyle") });

                            //// Updating the base chart's axis line stroke properties
                            this._olapArea.PrimaryAxis.SetBinding(ChartAxis.LineStrokeProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("GridLineStroke") });
                            this._olapArea.SecondaryAxis.SetBinding(ChartAxis.LineStrokeProperty, new Binding() { Source = this._olapArea.SecondaryAxis, Path = new PropertyPath("GridLineStroke") });

                            this._olapArea.PrimaryAxis.SetBinding(ChartAxis.SmallTicksStrokeProperty, new Binding() { Source = this._olapArea.PrimaryAxis, Path = new PropertyPath("GridLineStroke") });
                            this._olapArea.SecondaryAxis.SetBinding(ChartAxis.SmallTicksStrokeProperty, new Binding() { Source = this._olapArea.SecondaryAxis, Path = new PropertyPath("GridLineStroke") });
                            
                            //// Updating each label panel properties
                            this.SetBinding(ChartVisualStyleProperty, new Binding() { Source = this._olapArea.ChartControl, Path = new PropertyPath("ChartVisualStyle") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.LabelGridBorderProperty, new Binding() { Source = this._olapArea.SecondaryAxis, Path = new PropertyPath("GridLineStroke") });
                        
                            this._olapLablePanel.SetBinding(OlapLabelPanel.LabelForegroundProperty, new Binding() { Source = this, Path = new PropertyPath("LabelForeground") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.LabelBackgroundProperty, new Binding() { Source = this, Path = new PropertyPath("LabelBackground") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.LabelFontStyleProperty, new Binding() { Source = this, Path = new PropertyPath("LabelFontStyle") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.LabelFontSizeProperty, new Binding() { Source = this, Path = new PropertyPath("LabelFontSize") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.ExpanderStyleProperty, new Binding() { Source = this._olapArea.ChartControl, Path = new PropertyPath("ExpanderStyle") });
                            this._olapLablePanel.SetBinding(OlapLabelPanel.ChartVisualStyleProperty, new Binding() { Source = this, Path = new PropertyPath("ChartVisualStyle") });
                           
                        }
                    }

                    this._olapLablePanel.ToolTipVisibility = this.ParentArea.ChartControl.ShowHeaderToolTip ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    this._olapLablePanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    this._olapLablePanel.LabelSource = this.ParentArea.CurrentEngine;
                    this.Children.Clear();
                    this.Children.Add(_olapLablePanel);
                }

                //// Measure the size
                if (this._olapLablePanel != null)
                {
                    ChartAxis primaryAxis;

                    primaryAxis = this._olapLablePanel.Orientation == Orientation.Horizontal ? this.ParentArea.PrimaryAxis : this.ParentArea.SecondaryAxis;
                    
                    if (primaryAxis.ChartAxesProvider != null)
                    {
                        //// Getting the IEnumerable of chart point available int he primary axis
                        IEnumerable<ChartAxisPoints> chartPoints = primaryAxis.ChartAxesProvider.GetPoints(
                            primaryAxis.Range.Start,
                            primaryAxis.Range.End,
                            primaryAxis.VisibleInterval,
                            primaryAxis.Orientation,
                            primaryAxis.OpposedPosition,
                            availableSize,
                            new List<double>(),
                            false,
                            10,
                            double.NaN, false, primaryAxis);

                        /// Calculating the size of the Axis and assigning it to the size of the olap label panel.
                        double labelWidth = 10;
                        double labelHeight = 10;
                        int chartPointsCount = chartPoints.Count();
                        if (chartPointsCount > 1)
                        {
                            if (this._olapLablePanel.Orientation == Orientation.Horizontal)
                            {
                                //// Taking the last chart point's x2 position to find the length of the horizontal axis.
                                //// This is applicable for Column type charts.
                                //// This is optimal. Since, previously we were finding the distance between two points and applying it each time.
                                labelWidth = chartPoints.ElementAt(chartPointsCount - 1).X2;
                                this._olapLablePanel.Width = labelWidth;
                            }
                            else
                            {
                                //// Taking the y2 value of 0th element will result in the height of the vertical axis.
                                //// This is applicable for Bar type charts.
                                labelHeight = chartPoints.ElementAt(0).Y2;
                                this._olapLablePanel.Height = labelHeight;
                            }
                        }
                    }

                    foreach (UIElement ele in this.Children)
                    {
                        ele.Measure(availableSize);
                    }

                    return _olapLablePanel.DesiredSize;
                }

                // return availableSize;
                return new Size(0,0);
            }
            else
            {
                return new Size();
            }
        }

        void _olapLablePanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(null);
            headerPopup.DataContext = GetPopupDataContext(position, _olapLablePanel);

            if (headerPopup.DataContext != null)
            {
                OlapLabelPresenter lablePresenter = headerPopup.DataContext as OlapLabelPresenter;
                if (lablePresenter != null && lablePresenter.ToolTipVisibility == System.Windows.Visibility.Visible && lablePresenter.CellDescriptor != null && !string.IsNullOrEmpty(lablePresenter.CellDescriptor.CellValue))
                {
                    headerPopup.IsOpen = true;
                    headerPopup.VerticalOffset = position.Y + 10;
                    headerPopup.HorizontalOffset = position.X + 10;
                }
            }
        }

        private object GetPopupDataContext(Point position, OlapLabelPanel _olapLablePanel)
        {
            var hitTestResult = VisualTreeHelper.FindElementsInHostCoordinates(position, _olapLablePanel);

            Predicate<UIElement> perdicateElement = (u) =>
                {
                    var currentUIElement = u as Border;

                    if (currentUIElement != null)
                    {
                        if (currentUIElement.Child is OlapLabelPresenter)
                        {
                            return true;
                        }
                    }

                    return false;
                };

            var border = (Border)hitTestResult.Select(s => s).Where(p => perdicateElement(p)).FirstOrDefault();
            var olapLabelPresenter = border.Child;

            if (olapLabelPresenter != null)
            {
                //return (olapLabelPresenter as OlapLabelPresenter).CellDescriptor;
                return (olapLabelPresenter as OlapLabelPresenter);
            }

            return null;
        }

        void _olapLablePanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            headerPopup.IsOpen = false;
            headerPopup.DataContext = null;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the parent OlapArea
        /// </summary>
        internal OlapArea ParentArea
        {
            get { return _olapArea ?? (_olapArea = this.GetParentArea()); }
        }

        /// <summary>
        /// Gets or sets the chart visual style.
        /// </summary>
        /// <value>The chart visual style.</value>
        internal OlapChartVisualStyle ChartVisualStyle
        {
            get { return (OlapChartVisualStyle)GetValue(ChartVisualStyleProperty); }
            set { SetValue(ChartVisualStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid line brush.
        /// </summary>
        /// <value>The grid line brush.</value>
        internal Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>The label foreground.</value>
        internal Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the expander style.
        /// </summary>
        /// <value>The expander style.</value>
        internal Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        internal Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        internal double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        internal FontStyle LabelFontStyle
        {
            get { return (FontStyle)GetValue(LabelFontStyleProperty); }
            set { SetValue(LabelFontStyleProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ChartVisualStyleProperty =
            DependencyProperty.Register("ChartVisualStyle", typeof(OlapChartVisualStyle), typeof(OlapChartAxisPanel), new PropertyMetadata(OlapChartVisualStyle.Default, OnChartVisualStyleChanged));

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(OlapChartAxisPanel), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(OlapChartAxisPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        
        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(OlapChartAxisPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(OlapChartAxisPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(OlapChartAxisPanel), new PropertyMetadata(13d));

        public static readonly DependencyProperty LabelFontStyleProperty =
           DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(OlapChartAxisPanel), new PropertyMetadata(FontStyles.Normal));
       
        #endregion

        #region Callbacks

        private static void OnChartVisualStyleChanged(DependencyObject depenencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapChartAxisPanel olapChartAxisPanel = depenencyObject as OlapChartAxisPanel;

            if (olapChartAxisPanel != null)
            {
                if (olapChartAxisPanel._olapArea != null)
                {
                    olapChartAxisPanel.GridLineBrush = olapChartAxisPanel._olapArea.PrimaryAxis.GridLineStroke;
                    olapChartAxisPanel.LabelBackground = olapChartAxisPanel._olapArea.PrimaryAxis.LabelBackground;
                    olapChartAxisPanel.LabelFontSize = olapChartAxisPanel._olapArea.PrimaryAxis.LabelFontSize;
                    olapChartAxisPanel.LabelForeground = olapChartAxisPanel._olapArea.PrimaryAxis.LabelForeground;
                    olapChartAxisPanel.LabelFontStyle = olapChartAxisPanel._olapArea.PrimaryAxis.FontStyle;
                    olapChartAxisPanel.ExpanderStyle = olapChartAxisPanel._olapArea.ChartControl.ExpanderStyle;                    
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the parent OlapArea iterating the VisualTreeHelper
        /// </summary>
        /// <returns></returns>
        internal OlapArea GetParentArea()
        {
            DependencyObject element = this;

            while (!(element is ChartArea))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as OlapArea;
            }

            return null;
        } 

        /// <summary>
        /// Gets the olap label panel orientation.
        /// </summary>
        /// <param name="chartTypes">The chart types.</param>
        /// <returns>The Orientation of the OlapLabel</returns>
        internal static Orientation GetOlapLabelPanelOrientation(ChartTypes chartTypes)
        {
            if (chartTypes == ChartTypes.Bar ||
               chartTypes == ChartTypes.StackingBar ||
               chartTypes == ChartTypes.StackingBar100 ||
               chartTypes == ChartTypes.RotatedSpline)
            {
                return Orientation.Vertical;
            }

            return Orientation.Horizontal;
        }

        #endregion
    }
}
