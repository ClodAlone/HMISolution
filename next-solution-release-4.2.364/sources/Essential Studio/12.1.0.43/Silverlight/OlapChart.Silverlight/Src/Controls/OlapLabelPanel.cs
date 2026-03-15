#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.Windows.Chart;
// using Syncfusion.Silverlight.Chart.Olap.Controls;
using System.Windows.Data;

namespace Syncfusion.Silverlight.Chart.Olap
{
    public class OlapLabelPanel : Grid
    {
        #region Members

        private object olapArea = null;

        #endregion
        
        #region Constructor

        public OlapLabelPanel()
        {
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty LabelSourceProperty = 
            DependencyProperty.Register("LabelSource", typeof(PivotEngine), typeof(OlapLabelPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelsSourceChanged)));

        public static DependencyProperty LabelTemplateProperty = 
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(OlapLabelPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(OlapLabelPanel), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty LabelGridBorderProperty =
            DependencyProperty.Register("LabelGridBorder", typeof(Brush), typeof(OlapLabelPanel), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(OlapLabelPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(OlapLabelPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty ChartVisualStyleProperty =
            DependencyProperty.Register("ChartVisualStyle", typeof(OlapChartVisualStyle), typeof(OlapLabelPanel), new PropertyMetadata(OlapChartVisualStyle.Default));

        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(OlapLabelPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(OlapLabelPanel), new PropertyMetadata(13d));

        public static readonly DependencyProperty LabelFontStyleProperty =
           DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(OlapLabelPanel), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty ToolTipVisibilityProperty =
            DependencyProperty.Register("ToolTipVisibility", typeof(Visibility), typeof(OlapLabelPanel), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets value for LabelSource of LabelPresenter(PrimaryAxis)
        /// </summary>
        public PivotEngine LabelSource
        {
            get
            {
                return (PivotEngine)GetValue(LabelSourceProperty);
            }
            set
            {
                SetValue(LabelSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets DataTemplate for LabelPresenter(PrimaryAxis)
        /// </summary>
        public DataTemplate LableTemplate
        {
            get
            {
                return (DataTemplate)GetValue(LabelTemplateProperty);
            }
            set
            {
                SetValue(LabelTemplateProperty, value);
            }
        }

        internal OlapArea ParentArea
        {
            get
            {
                if (this.olapArea == null)
                {
                    return this.GetParentArea();
                }

                return this.olapArea as OlapArea;
            }
        }

        internal Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        internal double LabelRotationAngle { get; set; }

        internal Brush LabelGridBorder
        {
            get
            {
                return (Brush)GetValue(LabelGridBorderProperty);
            }
            set
            {
                SetValue(LabelGridBorderProperty, value);
            }
        }

        public Brush LabelForeground
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

        internal OlapChartVisualStyle ChartVisualStyle
        {
            get { return (OlapChartVisualStyle)GetValue(ChartVisualStyleProperty); }
            set { SetValue(ChartVisualStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets value for LabelBackground of LabelPresenter(PrimaryAxis)
        /// </summary>
        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }


        /// <summary>
        /// Gets or sets value for LabelFontSize of LabelPresenter(PrimaryAxis)
        /// </summary>
        public double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        
        
        /// <summary>
        /// Gets or sets value for LabelFontStyle of LabelPresenter(PrimaryAxis)
        /// </summary>
        public FontStyle LabelFontStyle
        {
            get { return (FontStyle)GetValue(LabelFontStyleProperty); }
            set { SetValue(LabelFontStyleProperty, value); }
        }
         
        /// <summary>
        /// Gets or sets value for visibility of LabelPresenter(PrimaryAxis) ToolTip
        /// </summary>
        public Visibility ToolTipVisibility
        {
            get { return (System.Windows.Visibility)GetValue(ToolTipVisibilityProperty); }
            set { SetValue(ToolTipVisibilityProperty, value); }
        }

        internal string PrimaryAxisLabelDateTimeFormat { get; set; }

        #endregion

        #region Events and CallBacks
        
        private void labelPresenter_LabelClick(object sender, OlapLabelClickEvenArgs e)
        {
            if (this.ParentArea != null)
            {
                this.ParentArea.OnLabelClick(e);
            }
        }

        private void labelPresenter_LabelCellClick(object sender, OlapLabelClickEvenArgs e)
        {
            if (this.ParentArea != null)
            {
                this.ParentArea.OnLabelCellClick(e);
            }
        }

        public static void OnLabelsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapLabelPanel labelPanel = d as OlapLabelPanel;

            if (labelPanel != null && e.NewValue != e.OldValue && e.NewValue != null)
            {
                labelPanel.RebuildOlapLabels();
            }
        }

        #endregion
        
        #region Helper Methods

        private Thickness OlapLabelBorder(Border olapLabelPresenter, OlapLabelPanel olapLabelPanel, double thickness, int rowCount)
        {
            int columnCount = olapLabelPanel.LabelSource.RowsCount - olapLabelPanel.LabelSource.HeaderSection.Height;
            double verticalSpanThickness = olapLabelPanel.Orientation == System.Windows.Controls.Orientation.Vertical ? 0d : 1d;
            double horizontalSpanThickness = olapLabelPanel.Orientation == System.Windows.Controls.Orientation.Horizontal ? 0d : 1d;

            Thickness retValue;
            if (olapLabelPanel.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                if ((olapLabelPresenter.Child as OlapLabelPresenter).CellDescriptor == null)
                {
                    //// This is a span cell for Bar Types charts
                    retValue = new Thickness(verticalSpanThickness, 0, 0, thickness);

                    if (olapLabelPresenter != null && olapLabelPanel != null)
                    {
                        if (Grid.GetRow(olapLabelPresenter) == 0)
                            retValue = new Thickness(verticalSpanThickness, thickness, thickness, thickness);
                    }
                }
                else
                {
                    //// This is normal header cell for Bar Type charts
                    retValue = new Thickness(thickness, 0, 0, thickness);

                    if (olapLabelPresenter != null && olapLabelPanel != null)
                    {
                        if (Grid.GetRow(olapLabelPresenter) == 0)
                            retValue = new Thickness(thickness, thickness, 0, thickness);
                    }
                }
            }
            else
            {
                if ((olapLabelPresenter.Child as OlapLabelPresenter).CellDescriptor == null)
                {
                    //// This is a span cell for Column Type charts
                    retValue = new Thickness(thickness, 0, 0, horizontalSpanThickness);

                    if (columnCount == rowCount)
                        retValue = new Thickness(thickness, 0, thickness, horizontalSpanThickness);

                    if (olapLabelPresenter != null && olapLabelPanel != null)
                    {
                        if (Grid.GetRow(olapLabelPresenter) == rowCount - 1)
                        {
                            retValue = new Thickness(thickness, 0, thickness, horizontalSpanThickness);
                        }

                        return retValue;
                    }
                }
                else
                {
                    //// This is normal header cell for Column type charts.
                    retValue = new Thickness(thickness, 0, 0, thickness);

                    if (columnCount == rowCount)
                        retValue = new Thickness(thickness, 0, thickness, thickness);

                    if (olapLabelPresenter != null && olapLabelPanel != null)
                    {
                        if (Grid.GetRow(olapLabelPresenter) == rowCount - 1)
                        {
                            retValue = new Thickness(thickness, 0, thickness, thickness);
                        }

                        return retValue;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Gets the row count (No. of labels going to be present).
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <returns>Row count as an integer</returns>
        internal int GetRowCount(PivotEngine engine)
        {
            return engine.RowHeaderSection.Bottom - engine.RowHeaderSection.Top;
        }

        /// <summary>
        /// Gets the parent area.
        /// </summary>
        /// <returns>Parent area of OlapLablePanel</returns>
        internal OlapArea GetParentArea()
        {
            DependencyObject element = this as DependencyObject;

            while (!(element is ChartArea) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                OlapArea area = element as OlapArea;
                return element as OlapArea;
            }
            return null;
        }

        private void RebuildOlapLabels()
        {
            this.Children.Clear();
            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();

            if (this.LabelSource != null)
            {
                //// Finding the row expander cell contents range.
                int left = LabelSource.RowHeaderSection.Left,
                    right = LabelSource.RowHeaderSection.Right,
                    top = LabelSource.HeaderSection.Height,
                    bottom = LabelSource.RowsCount;

                for (int i = left; i <= right; i++)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    else
                    {
                        ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    }

                    PivotColumnDescriptor columnDescriptor = this.LabelSource.TableColumns[i];

                    for (int j = top; j < bottom; j++)
                    {

                        if (i == left)
                        {
                            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                ColumnDefinitions.Add(new ColumnDefinition());
                            }
                            else
                            {
                                RowDefinitions.Add(new RowDefinition());
                            }
                        }

                        PivotCellDescriptor cellDescriptor = columnDescriptor.Cells[j];
                        Border border = new Border();
                        //// Binding the gridlines color to the label panel border color.
                        // border.BorderBrush = this.LabelGridBorder;
                        border.SetBinding(Border.BorderBrushProperty, new Binding() { Source = this, Path = new PropertyPath("LabelGridBorder") });
                        OlapLabelPresenter labelPresenter = new OlapLabelPresenter() { PrimaryAxisLabelDateTimeFormat = this.PrimaryAxisLabelDateTimeFormat };
                        labelPresenter.ToolTipVisibility = this.ToolTipVisibility;
                        labelPresenter.LabelClick += new OlapLabelClick(labelPresenter_LabelClick);
                        labelPresenter.LabelCellClick += new OlapLabelClick(labelPresenter_LabelCellClick);
                        labelPresenter.Height = 20;

                        if (cellDescriptor.SpanCell == null)
                        {
                            labelPresenter.VerticalAlignment = VerticalAlignment.Center;
                            labelPresenter.CellDescriptor = cellDescriptor;
                            labelPresenter.LabelRotationAngle = this.LabelRotationAngle;
                            labelPresenter.SetBinding(OlapLabelPresenter.LabelForegroundProperty, new Binding() { Source = this, Path = new PropertyPath("LabelForeground") });
                            labelPresenter.SetBinding(OlapLabelPresenter.LabelBackgroundProperty, new Binding() { Source = this, Path = new PropertyPath("LabelBackground") });
                            labelPresenter.SetBinding(OlapLabelPresenter.LabelFontStyleProperty, new Binding() { Source = this, Path = new PropertyPath("LabelFontStyle") });
                            labelPresenter.SetBinding(OlapLabelPresenter.LabelFontSizeProperty, new Binding() { Source = this, Path = new PropertyPath("LabelFontSize") });
                            labelPresenter.SetBinding(OlapLabelPresenter.ExpanderStyleProperty, new Binding() { Source = this, Path = new PropertyPath("ExpanderStyle") });
                            labelPresenter.SetBinding(OlapLabelPresenter.ChartVisualStyleProperty, new Binding() { Source = this, Path = new PropertyPath("ChartVisualStyle") });
                            border.Child = labelPresenter;                            

                            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                labelPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                                Grid.SetRow(border, right - i);
                                Grid.SetColumn(border, j - top);
                                Grid.SetColumnSpan(border, cellDescriptor.Range.Height);                                
                                border.Margin = new Thickness(0, -5, 0, 5);
                            }
                            else
                            {
                                Grid.SetColumn(border, i - left);
                                Grid.SetRow(border, bottom - j - cellDescriptor.Range.Height);
                                Grid.SetRowSpan(border, cellDescriptor.Range.Height);
                                border.Margin = new Thickness(5, 0, -5, 0);
                            }

                            border.BorderThickness = OlapLabelBorder(border, this, 0.25, j-1);
                            // border.BorderBrush = new SolidColorBrush(Color.FromArgb(96, 01, 65, 0));

                            this.Children.Add(border);
                        }
                        else if (cellDescriptor.CellValue == string.Empty)
                        {
                            border.Child = labelPresenter;

                            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                Grid.SetRow(border, right - i);
                                Grid.SetColumn(border, j - top);
                                border.Margin = new Thickness(0, -5, 0, 5);
                            }
                            else
                            {
                                Grid.SetColumn(border, i - left);
                                Grid.SetRow(border, bottom - j - cellDescriptor.Range.Height);
                                border.Margin = new Thickness(5, 0, -5, 0);
                            }

                            border.BorderThickness = OlapLabelBorder(border, this, 0.25, j-1);
                            // border.BorderBrush = new SolidColorBrush(Color.FromArgb(96, 01, 65, 0));

                            this.Children.Add(border);
                        }
                    }
                }
            }            
        }
        #endregion
    }
}
