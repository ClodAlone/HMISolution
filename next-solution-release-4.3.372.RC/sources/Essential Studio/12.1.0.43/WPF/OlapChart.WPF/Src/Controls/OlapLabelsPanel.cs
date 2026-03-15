#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Syncfusion.Olap.Engine;
    using System.Linq;
    using System.Diagnostics;


#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapLabelsPanel : Grid
    {
        #region DependencyProperties
        ///<summary>
        /// Identifies the LabelsSource dependency property.
        ///</summary>
        public static readonly DependencyProperty LabelsSourceProperty =
            DependencyProperty.Register("LabelsSource", typeof(PivotEngine), typeof(OlapLabelsPanel), new UIPropertyMetadata(null, new PropertyChangedCallback(OnLabelsSourceChanged)));

        ///<summary>
        /// Identifies the LabelTemplate dependency property.
        ///</summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(OlapLabelsPanel), new UIPropertyMetadata(null));
        private const double m_labelsTolarance = 10;

        ///<summary>
        /// Identifies the Orientation dependency property.
        ///</summary>
        public static readonly DependencyProperty OrientationProperty = OlapScrollingPanel.OrientationProperty.AddOwner(typeof(OlapLabelsPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnOrientationChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the labels source.
        /// </summary>
        /// <value>The labels source.</value>
        public PivotEngine LabelsSource
        {
            get { return (PivotEngine)GetValue(LabelsSourceProperty); }
            set { SetValue(LabelsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LabelTemplate. This is a dependency property.
        /// </summary>
        /// <value>The LabelTemplate.</value>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Orientation. This is a dependency property.
        /// </summary>
        /// <value>The Orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        #endregion

        #region Implementation
#if DEBUG
        Stopwatch swatch = new Stopwatch();
#endif
        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.Grid"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.Grid.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">Indicates an upper limit size that should not be exceeded.</param>
        /// <returns>
        /// 	<see cref="T:System.Windows.Size"/> that represents the required size to arrange child content.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if DEBUG
#if SyncfusionFramework3_5
            swatch.Reset();
            swatch.Start();
#else
            swatch.Restart();
#endif
#endif
            OlapScrollingPanel scrollingPanel = (VisualTreeHelper.GetParent(this) as OlapScrollingPanel);
            double requestedScale = 1;
            //Determining minimum allowed olap cell size with respect to axis orientation.
            double parentAllowedCellSize;

            if (Orientation == Orientation.Horizontal)
            {
                parentAllowedCellSize = (scrollingPanel.Parent as ChartCartesianAxisPanel).ActualWidth / ColumnDefinitions.Count;
            }
            else
            {
                parentAllowedCellSize = (scrollingPanel.Parent as ChartCartesianAxisPanel).ActualHeight / RowDefinitions.Count;
            }

            //Verifying size for each label.
            if (InternalChildren.Count > 0)
            {
                var m_elementCollection = InternalChildren.Cast<OlapLabelPresenter>().Where(i => (i.Content as PivotCellDescriptor).CellCaption != null).OrderByDescending(i => (i.Content as PivotCellDescriptor).CellCaption.Length).Take(10);
                if (m_elementCollection != null)
                {
                    foreach (FrameworkElement element in m_elementCollection)
                    {
                        //Measuring label to determine it's desired size.
                        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        //Assuming all columns have the same size. Calculating scale required to fit the label.
                        double scale = (Orientation == Orientation.Horizontal) ?
                          parentAllowedCellSize / (element.DesiredSize.Width + m_labelsTolarance) :
                          parentAllowedCellSize / (element.DesiredSize.Height + m_labelsTolarance);
                        //Determining minimum scale.
                        if (scale <= 1)
                        {
                            requestedScale = Math.Min(requestedScale, scale);
                        }
                    }
                }
            }
            //Requesting parent scrolling panel to get zoomed by scale determined.
            scrollingPanel.RequestZoomFactor(requestedScale);
#if DEBUG
            swatch.Stop();
            Debug.WriteLine("Label panel updated in: " + swatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds.");
#endif
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called when labels source changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapLabelsPanel panel = (d as OlapLabelsPanel);
            if (panel != null)
            {
                panel.RebuildLabels();
            }
        }

        /// <summary>
        /// Called when orientation changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapLabelsPanel panel = (d as OlapLabelsPanel);

            if (panel != null)
            {
                panel.RebuildLabels();
            }
        }

        /// <summary>
        /// Rebuilds the labels.
        /// </summary>
        private void RebuildLabels()
        {
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            Children.Clear();
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
            if (LabelsSource != null)
            {
                bool isHorizontal = Orientation == Orientation.Horizontal ? true : false;
                int left = LabelsSource.RowHeaderSection.Left,
                  right = LabelsSource.RowHeaderSection.Right,
                  top = LabelsSource.HeaderSection.Height,
                  bottom = LabelsSource.RowsCount;
                for (int i = left; i <= right; i++)
                {
                    if (isHorizontal)
                    {
                        RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    }
                    else
                    {
                        ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    }

                    PivotColumnDescriptor columnDescriptor = LabelsSource.TableColumns[i];
                    for (int j = top; j < bottom; j++)
                    {
                        if (i == left)
                        {
                            if (isHorizontal)
                            {
                                ColumnDefinitions.Add(new ColumnDefinition());
                            }
                            else
                            {
                                RowDefinitions.Add(new RowDefinition());
                            }
                        }

                        PivotCellDescriptor cellDescriptior = columnDescriptor.Cells[j];
                        OlapLabelPresenter label = new OlapLabelPresenter(cellDescriptior);
                        if(cellDescriptior.Tag == null || cellDescriptior.CellValue == string.Empty || cellDescriptior.SpanCell == null)
                        {
                            Binding binding = new Binding("LabelTemplate");
                            binding.Source = this;
                            BindingOperations.SetBinding(label, ContentControl.ContentTemplateProperty, binding);

                            if (isHorizontal)
                            {
                                if (cellDescriptior.SpanCell == null)
                                {
                                    if (this.LabelsSource.ItemSource == null)
                                    {
                                        Grid.SetColumnSpan(label, cellDescriptior.Range.Height);
                                    }
                                    else
                                    {
                                        // Grid.SetColumnSpan(label, 1);
                                        Grid.SetColumnSpan(label, cellDescriptior.Range.Height);
                                    }
                                }
                                Grid.SetRow(label, right - i);
                                Grid.SetColumn(label, j - top);
                            }
                            else
                            {
                                if (cellDescriptior.SpanCell == null)
                                {
                                    if (this.LabelsSource.ItemSource == null)
                                    {
                                        Grid.SetRowSpan(label, cellDescriptior.Range.Height);
                                    }
                                    else
                                    {
                                        // Grid.SetRowSpan(label, 1);
                                        Grid.SetRowSpan(label, cellDescriptior.Range.Height);
                                    }
                                }
                                Grid.SetColumn(label, i - left);
                                Grid.SetRow(label, bottom - j - cellDescriptior.Range.Height);
                            }
                            Children.Add(label);
                        }
                    }
                }
            }
            st.Stop();
            System.Diagnostics.Debug.WriteLine(st.ElapsedMilliseconds);
        }

        #endregion
    }

    /// <summary>
    /// Represent OlapLabelBorderConverter
    /// </summary>
    public class OlapLabelBorderConverter : IMultiValueConverter
    {
        #region Constructor
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// If the method returns null, the valid null value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Checking whether all bindings have reported correct value.
            foreach (object o in values)
            {
                if (o == DependencyProperty.UnsetValue)
                {
                    // At least 1 binding reported incorrect value.
                    return DependencyProperty.UnsetValue;
                }
            }
            ContentControl olapLabelPresenter = values[0] as ContentControl;
            OlapLabelsPanel olapPanel = values[1] as OlapLabelsPanel;
            OlapChart olapChart = values[2] as OlapChart;

            //if (olapChart.ShowPrimaryAxisLabelBorder == true)
            {
                if (olapChart.ChartType == ChartTypes.Pie)
                {
                    return new Thickness(0.5);
                }

                int columnSpan = Grid.GetColumnSpan(olapLabelPresenter);
                int columnPos = Grid.GetColumn(olapLabelPresenter);

                double thickness = (double)values[3];

                Thickness retValue = new Thickness(thickness, 0, 0, thickness);
                if (columnPos + columnSpan == olapPanel.ColumnDefinitions.Count)
                    retValue = new Thickness(thickness, 0, 1, thickness);
                if (olapLabelPresenter != null && olapPanel != null && (bool)values[4])
                {
                    switch (olapPanel.Orientation)
                    {
                        case Orientation.Horizontal:
                            {
                                if (Grid.GetColumn(olapLabelPresenter) == olapPanel.ColumnDefinitions.Count - 1)
                                {
                                    retValue = new Thickness(thickness, 0, thickness, thickness);
                                }
                                break;
                            }
                        case Orientation.Vertical:
                            {
                                if (Grid.GetRow(olapLabelPresenter) == 0)
                                {
                                    retValue = new Thickness(thickness, thickness, 0, thickness);
                                }
                                break;
                            }
                        default:
                            {
                                throw new NotSupportedException(olapPanel.Orientation.ToString() + " orientation is not supported by OlapPanel");
                            }
                    }
                    return retValue;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Back conversion is not supported by Olap panel");
        }
        #endregion

    }
}
