// <copyright file="ChartAdornmentsPresenter.cs" company="Syncfusion">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents ChartAdornmentsPresenter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAdornmentsPresenter : FrameworkElement, IDisposable
    {
        #region InternalTypes
        /// <summary>
        /// Represents ChartAdornmentContainer.
        /// </summary>
        public class ChartAdornmentContainer : FrameworkElement, IDisposable
        {
            #region Internal Types
            
            #endregion

            #region Members
            /// <summary>
            /// Initializes m_ardorment.
            /// </summary>
            private ChartAdornment m_ardorment = null;

            /// <summary>
            /// Initializes m_symbolOffset.
            /// </summary>
            private Vector m_symbolOffset = new Vector();

            /// <summary>
            /// Initializes m_adornmentSize.
            /// </summary>
            internal Size m_adornmentSize = new Size();

            /// <summary>
            /// Initializes m_labelPresenter.
            /// </summary>
            private ContentPresenter m_labelPresenter = new ContentPresenter();

            /// <summary>
            /// Initializes m_symbolPresenter.
            /// </summary>
            private ContentPresenter m_symbolPresenter = new ContentPresenter();

            /// <summary>
            /// Initializes m_connectorPresenter.
            /// </summary>
            private ContentPresenter m_connectorPresenter = new ContentPresenter();

            /// <summary>
            /// Initializes m_predefinedSymbol.
            /// </summary>
            private Control m_predefinedSymbol = new Control();

            /// <summary>
            /// Initializes screen Height. 
            /// </summary>
            private double screenHeight = 0.0;

            /// <summary>
            /// Initializes screen Width.
            /// </summary>            
            private double screenWidth = 0.0;
            #endregion

            #region Dependency properties

            /// <summary>
            /// Using a DependencyProperty as the backing store for LabelHorizontalAlignment.  This enables animation, styling, binding, etc...
            /// </summary>
            public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
              DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentContainer), new UIPropertyMetadata(HorizontalAlignment.Center));

            /// <summary>
            /// Using a DependencyProperty as the backing store for LabelVerticalAlignment.  This enables animation, styling, binding, etc...
            /// </summary>
            public static readonly DependencyProperty LabelVerticalAlignmentProperty =
              DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentContainer), new UIPropertyMetadata(VerticalAlignment.Center));
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the connector shift.
            /// </summary>
            /// <value>The connector shift.</value>
            internal Double ConnectorShift
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the connector presenter.
            /// </summary>
            /// <value>The connector presenter.</value>
            public ContentPresenter ConnectorPresenter
            {
                get
                {
                    return this.m_connectorPresenter;
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
                    return 3;
                }
            }

            /// <summary>
            /// Gets the adornment.
            /// </summary>
            /// <value>The adornment.</value>
            public ChartAdornment Adornment
            {
                get
                {
                    return this.m_ardorment;
                }
            }

            /// <summary>
            /// Gets the symbol offset.
            /// </summary>
            /// <value>The symbol offset.</value>
            public Vector SymbolOffset
            {
                get { return this.m_symbolOffset; }
            }

            /// <summary>
            /// Gets or sets the label vertical alignment.
            /// </summary>
            /// <value>The label vertical alignment.</value>
            public VerticalAlignment LabelVerticalAlignment
            {
                get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
                set { SetValue(LabelVerticalAlignmentProperty, value); }
            }

            /// <summary>
            /// Gets or sets the label horizontal alignment.
            /// </summary>
            /// <value>The label horizontal alignment.</value>
            public HorizontalAlignment LabelHorizontalAlignment
            {
                get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
                set { SetValue(LabelHorizontalAlignmentProperty, value); }
            }

            /// <summary>
            /// Gets the label presenter.
            /// </summary>
            /// <value>The label presenter.</value>
            public ContentPresenter LabelPresenter
            {
                get
                {
                    return this.m_labelPresenter;
                }
            }

            /// <summary>
            /// Gets the label presenter.
            /// </summary>
            /// <value>The label presenter.</value>
            public ContentPresenter SymbolPresenter
            {
                get
                {
                    return this.m_symbolPresenter;
                }
            }

            /// <summary>
            /// Gets the predefined symbol.
            /// </summary>
            /// <value>The predefined symbol.</value>
            public Control PredefinedSymbol
            {
                get
                {
                    return this.m_predefinedSymbol;
                }
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartAdornmentContainer"/> class.
            /// </summary>
            /// <param name="ardorment">The ardorment.</param>
            /// <param name="RD"></param>
            public ChartAdornmentContainer(ChartAdornment ardorment, ResourceDictionary RD)
            {
                this.m_ardorment = ardorment;
                this.m_ardorment.Series.AdornmentsInfo.PropertyChanged += this.OnAdornmentsInfoChanged;

                this.AddVisualChild(m_labelPresenter);
                this.AddVisualChild(m_symbolPresenter);
                this.AddVisualChild(m_connectorPresenter);
                this.AddVisualChild(m_predefinedSymbol);

                this.LabelVerticalAlignment = ardorment.Series.AdornmentsInfo.VerticalAlignment;
                this.LabelHorizontalAlignment = ardorment.Series.AdornmentsInfo.HorizontalAlignment;
                this.SetSymbol(ardorment.Series.AdornmentsInfo.Symbol.ToString(), RD);
                baseRD = RD;
                if (ardorment.Series.AdornmentsInfo.Symbol.ToString() != "Custom")
                this.SetContentBinding(ardorment);
            }

            /// <summary>
            /// Sets the content binding.
            /// </summary>
            /// <param name="ardorment">The ardorment.</param>
            private void SetContentBinding(ChartAdornment ardorment)
            {
                bool isAccumulatedChart = (ardorment.Series != null && ardorment.Series.Area != null) ? ChartArea.CheckCompatibility(ardorment.Series.ChartType) : false;
                Binding bindingProvider = new Binding();

                bindingProvider = new Binding("SegmentLabelFontFamily");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.FontFamilyProperty, bindingProvider);

                bindingProvider = new Binding("SegmentLabelFontSize");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.FontSizeProperty, bindingProvider);

                bindingProvider = new Binding("AdornmentForeground");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.ForegroundProperty, bindingProvider);

                bindingProvider = new Binding("AdornmentFontStretch");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.FontStretchProperty, bindingProvider);

                bindingProvider = new Binding("AdornmentMargin");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.MarginProperty, bindingProvider);

                bindingProvider = new Binding("AdornmentWrapping");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.TextWrappingProperty, bindingProvider);

                bindingProvider = new Binding("SegmentLabelFontWeight");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(TextBlock.FontWeightProperty, bindingProvider);

                bindingProvider = new Binding("LabelTemplate");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_labelPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, bindingProvider);

                bindingProvider = new Binding("SymbolTemplate");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                if (this.m_ardorment.Series.AdornmentsInfo.Symbol == Symbol.Custom)
                {
                    this.m_symbolPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, bindingProvider);
                }

                bindingProvider = new Binding("ConnectorTemplate");
                bindingProvider.Source = ardorment.Series.AdornmentsInfo;
                this.m_connectorPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, bindingProvider);

                MultiBinding labelContentBinding = new MultiBinding();
                labelContentBinding.Converter = new AdornmentContentConverter();

                Binding contentPathBinding = new Binding(this.m_ardorment.Series.AdornmentsInfo.LabelContentPath);
                contentPathBinding.Mode = BindingMode.OneTime;
                contentPathBinding.Source = this.m_ardorment;

                Binding contentBinding = new Binding("SegmentLabelContent");
                contentBinding.Source = this.m_ardorment.Series.AdornmentsInfo;

                Binding adornmentBinding = new Binding();
                adornmentBinding.Source = this.m_ardorment;

                Binding labelFormatBinding = new Binding("SegmentLabelFormat");
                labelFormatBinding.Source = this.m_ardorment.Series.AdornmentsInfo;

                Binding labelDateTimeFormatBinding = new Binding("SegmentLabelDataTimeFormat");
                labelDateTimeFormatBinding.Source = this.m_ardorment.Series.AdornmentsInfo;

                labelContentBinding.Bindings.Add(contentPathBinding);
                labelContentBinding.Bindings.Add(contentBinding);
                labelContentBinding.Bindings.Add(adornmentBinding);
                labelContentBinding.Bindings.Add(labelFormatBinding);
                labelContentBinding.Bindings.Add(labelDateTimeFormatBinding);

                this.m_labelPresenter.SetBinding(ContentPresenter.ContentProperty, labelContentBinding);

                Binding bindingheight = new Binding("SymbolHeight");
                bindingheight.Source = Adornment.Series.AdornmentsInfo;
                if (!isAccumulatedChart)
                    this.m_predefinedSymbol.SetBinding(Control.HeightProperty, bindingheight);
                else
                    this.m_predefinedSymbol.Height = (double)ChartAdornmentInfo.SymbolHeightProperty.DefaultMetadata.DefaultValue;

                Binding bindingwidth = new Binding("SymbolWidth");
                bindingwidth.Source = Adornment.Series.AdornmentsInfo;
                if (!isAccumulatedChart)
                    this.m_predefinedSymbol.SetBinding(Control.WidthProperty, bindingwidth);
                else
                    this.m_predefinedSymbol.Width = (double)ChartAdornmentInfo.SymbolWidthProperty.DefaultMetadata.DefaultValue;

                Binding symbolInteriorBinding = new Binding("SymbolInterior");
                symbolInteriorBinding.Source = Adornment.Series.AdornmentsInfo;
                if (Adornment.Series.AdornmentsInfo.SymbolInterior == null)
                {
                    symbolInteriorBinding.Path = new PropertyPath("Interior");
                    symbolInteriorBinding.Source = this.Adornment.Series;
                }
                
                Binding symbolStrokeBinding = new Binding("SymbolStroke");
                symbolStrokeBinding.Source = Adornment.Series.AdornmentsInfo;

                if (Adornment.Series.AdornmentsInfo.SymbolStroke == null)
                {
                    symbolStrokeBinding.Path = new PropertyPath("Stroke");
                    symbolStrokeBinding.Source = this.Adornment.Series;
                }
                Binding symbolStrokeThicknessBinding = new Binding("SymbolStrokeThickness");
                symbolStrokeThicknessBinding.Source = Adornment.Series.AdornmentsInfo;
                if (double.IsNaN(Adornment.Series.AdornmentsInfo.SymbolStrokeThickness))
                {
                    symbolStrokeThicknessBinding.Path = new PropertyPath("StrokeThickness");
                    symbolStrokeThicknessBinding.Source = this.Adornment.Series;
                }
               

                this.m_predefinedSymbol.SetBinding(Control.BackgroundProperty, symbolInteriorBinding);
                this.m_predefinedSymbol.SetBinding(Control.BorderBrushProperty, symbolStrokeBinding);
                this.m_predefinedSymbol.SetBinding(Control.BorderThicknessProperty, symbolStrokeThicknessBinding);
             }

            #endregion

            #region Implementation
            /// <summary>
            /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
            /// </summary>
            /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
            /// <returns>The actual size used.</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                Rect arrangeRect = new Rect(this.DesiredSize);
                arrangePieRect = new Rect(this.DesiredSize);
                switch (this.LabelHorizontalAlignment)
                {
                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Center:
                        {
                            this.m_labelPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                            this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                            this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Center;
                        }

                        break;

                    case HorizontalAlignment.Left:
                        {
                            this.m_labelPresenter.HorizontalAlignment = HorizontalAlignment.Left;
                            this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Right;
                            this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Right;
                        }

                        break;

                    case HorizontalAlignment.Right:
                        {
                            this.m_labelPresenter.HorizontalAlignment = HorizontalAlignment.Right;
                            this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Left;
                            this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Left;
                        }

                        break;
                }

                switch (this.LabelVerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        {
                            this.m_labelPresenter.VerticalAlignment = VerticalAlignment.Bottom;
                            this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Top;
                            this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Top;
                        }

                        break;
                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Center:
                        {
                            this.m_labelPresenter.VerticalAlignment = VerticalAlignment.Center;
                            this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Center;
                            this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Center;
                        }

                        break;
                    case VerticalAlignment.Top:
                        {
                            this.m_labelPresenter.VerticalAlignment = VerticalAlignment.Top;
                            this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Bottom;
                            this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Bottom;
                        }

                        break;
                }

                if (Adornment.Series.AdornmentsInfo.IsSegmentAlignment)
                {
                    switch (Adornment.Series.Type)
                    {
                        case ChartTypes.Column:
                        case ChartTypes.StackingColumn:
                        case ChartTypes.StackingColumn100:
                        case ChartTypes.Area:      
                        case ChartTypes.Candle:                       
                        case ChartTypes.RangeColumn:
                            #region HorizontalAlignment
                            if (Adornment.Series.Type != ChartTypes.Area)
                            {
                                DoubleRange range = Adornment.Series.Area.GetSideBySideInfo(Adornment.Series);
                                double x1 = Adornment.DataPoint.X + range.Start;
                                double x2 = Adornment.DataPoint.X + range.End;
                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x1);
                                double trpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x2);
                                double leftvalue = 0d;
                                labelwidth = m_labelPresenter.DesiredSize.Width != 0 ? m_labelPresenter.DesiredSize.Width : labelwidth;

                                if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                {
                                    if (!Adornment.Series.AdornmentsInfo.SegmentIsOut)
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) + labelwidth;
                                    }
                                    else
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) - labelwidth;
                                    }
                                    switch (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment)
                                    {
                                        case System.Windows.HorizontalAlignment.Left:
                                            arrangePieRect.X = arrangePieRect.X + leftvalue;
                                            break;
                                        case System.Windows.HorizontalAlignment.Right:
                                            arrangePieRect.X = arrangePieRect.X - leftvalue;
                                            break;
                                    }
                                    m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.SegmentIsOut)
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) + labelwidth;
                                    }
                                    else
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) - labelwidth;
                                    }
                                    switch (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment)
                                    {
                                        case System.Windows.HorizontalAlignment.Left:
                                            arrangePieRect.X = arrangePieRect.X - leftvalue;
                                            break;
                                        case System.Windows.HorizontalAlignment.Right:
                                            arrangePieRect.X = arrangePieRect.X + leftvalue;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment)
                                {
                                    case System.Windows.HorizontalAlignment.Left:
                                        {
                                            if (Adornment.Index != 0)
                                            {
                                                double x1 = Adornment.Series.Data[Adornment.Index - 1].X;
                                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x1);
                                                double trpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, Adornment.DataPoint.X);
                                                double leftvalue = 0d;
                                                labelwidth = m_labelPresenter.DesiredSize.Width != 0 ? m_labelPresenter.DesiredSize.Width : labelwidth;
                                                if (Adornment.Series.AdornmentsInfo.SegmentIsOut)
                                                {
                                                    leftvalue = (trpoint - blpoint) / 2 + labelwidth;
                                                }
                                                else
                                                {
                                                    leftvalue = (trpoint - blpoint) / 2 - labelwidth;
                                                }
                                                arrangePieRect.X = arrangePieRect.X - leftvalue;
                                            }
                                        }
                                        break;
                                    case System.Windows.HorizontalAlignment.Right:
                                        {
                                            if (Adornment.Index < Adornment.Series.Data.Count - 1)
                                            {
                                                double x1 = Adornment.Series.Data[Adornment.Index + 1].X;
                                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, Adornment.DataPoint.X);
                                                double trpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x1);
                                                double leftvalue = 0d;
                                                labelwidth = m_labelPresenter.DesiredSize.Width != 0 ? m_labelPresenter.DesiredSize.Width : labelwidth;
                                                if (Adornment.Series.AdornmentsInfo.SegmentIsOut)
                                                {
                                                    leftvalue = (trpoint - blpoint) / 2 + labelwidth;
                                                }
                                                else
                                                {
                                                    leftvalue = (trpoint - blpoint) / 2 - labelwidth;
                                                }
                                                arrangePieRect.X = arrangePieRect.X + leftvalue;
                                            }
                                        }
                                        break;
                                }
                            }
                            #endregion
                            #region VerticalAllignment
                            switch (Adornment.Series.AdornmentsInfo.SegmentVerticalAlignment)
                            {
                                case System.Windows.VerticalAlignment.Center:
                                    {
                                        if (Adornment.Series.Type != ChartTypes.Candle && Adornment.Series.Type != ChartTypes.Gantt && Adornment.Series.Type != ChartTypes.RangeColumn)
                                        {
                                            if (Adornment.Series.Type != ChartTypes.StackingColumn100)
                                            {
                                                bool? forPositive = null;
                                                if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                                {
                                                    forPositive = Adornment.DataPoint.Y >= 0;
                                                }
                                                double y2 = Adornment.Series.Area.GetStackInfo(Adornment.Series, Adornment.DataPoint.X, forPositive);
                                                double y1 = Adornment.DataPoint.Y + y2;

                                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                                double leftvalue = 0d;
                                                labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                                leftvalue = (blpoint) - labelheight;
                                                if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                                {
                                                    arrangePieRect.Y = arrangePieRect.Y - (leftvalue / 2);
                                                    m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                                }
                                                else
                                                {
                                                    arrangePieRect.Y = arrangePieRect.Y + (leftvalue / 2);
                                                }
                                            }
                                            else if (Adornment.Series.Type == ChartTypes.StackingColumn100)
                                            {
                                                double leftvalue = 0d;
                                                labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;
                                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, 0);
                                                leftvalue = (blpoint) - labelheight;
                                                arrangePieRect.Y = arrangePieRect.Y + leftvalue / 2;
                                            }
                                        }
                                        else
                                        {

                                            bool? forPositive = null;
                                            if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                            {
                                                forPositive = Adornment.DataPoint.Y >= 0;
                                            }
                                            double y2 = Adornment.DataPoint.Values[0];
                                            double y1 = Adornment.DataPoint.Values[1];

                                            double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                            double leftvalue = 0d;
                                            labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                            leftvalue = (blpoint) - labelheight;
                                            arrangePieRect.Y = arrangePieRect.Y - (leftvalue / 2);
                                            if (Adornment.Series.Type == ChartTypes.Gantt)
                                            {
                                                arrangePieRect.Y = arrangePieRect.Y + (leftvalue);
                                                m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                            }

                                        }
                                    }
                                    break;
                                case System.Windows.VerticalAlignment.Bottom:
                                    {
                                        if (Adornment.Series.Type != ChartTypes.Candle && Adornment.Series.Type != ChartTypes.RangeColumn && Adornment.Series.Type != ChartTypes.Gantt && Adornment.Series.Type != ChartTypes.StackingColumn100)
                                        {
                                            bool? forPositive = null;
                                            if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                            {
                                                forPositive = Adornment.DataPoint.Y >= 0;
                                            }
                                            double y2 = Adornment.Series.Area.GetStackInfo(Adornment.Series, Adornment.DataPoint.X, forPositive);
                                            double y1 = Adornment.DataPoint.Y + y2;

                                            double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                            double leftvalue = 0d;
                                            labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                            leftvalue = (blpoint) - labelheight;
                                            if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                            {
                                                arrangePieRect.Y = arrangePieRect.Y - leftvalue - (2 * labelwidth);
                                                m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                            }
                                            else
                                            {
                                                arrangePieRect.Y = arrangePieRect.Y + leftvalue;
                                            }
                                        }
                                        else if (Adornment.Series.Type == ChartTypes.StackingColumn100)
                                        {
                                            double leftvalue = 0d;
                                            labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;
                                            double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, (Adornment.Series.YAxis.m_visibleRange.Start + Adornment.Series.YAxis.m_visibleInterval));
                                            leftvalue = (blpoint) - labelheight;
                                            arrangePieRect.Y = leftvalue;
                                        }
                                    }
                                    break;
                                case System.Windows.VerticalAlignment.Top:
                                    if (Adornment.Series.Type == ChartTypes.Candle || Adornment.Series.Type == ChartTypes.RangeColumn || Adornment.Series.Type == ChartTypes.Gantt)
                                    {
                                        bool? forPositive = null;
                                        if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                        {
                                            forPositive = Adornment.DataPoint.Y >= 0;
                                        }
                                        double y2 = Adornment.DataPoint.Values[0];
                                        double y1 = Adornment.DataPoint.Values[1];

                                        double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                        double leftvalue = 0d;
                                        labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                        leftvalue = (blpoint) - labelheight;

                                        if (Adornment.Series.Type == ChartTypes.Gantt)
                                        {
                                            arrangePieRect.Y = arrangePieRect.Y + leftvalue;
                                            m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                        }
                                        else
                                            arrangePieRect.Y = arrangePieRect.Y - leftvalue;
                                    }
                                    break;
                            }
                            #endregion
                            break;
                        case ChartTypes.Bar:
                        case ChartTypes.Gantt:
                        case ChartTypes.StackingBar:
                            #region HorizontalAllignment
                            {
                                DoubleRange range = Adornment.Series.Area.GetSideBySideInfo(Adornment.Series);
                                double x1 = Adornment.DataPoint.X + range.Start;
                                double x2 = Adornment.DataPoint.X + range.End;
                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x1);
                                double trpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.XAxis, x2);
                                double leftvalue = 0d;
                                labelwidth = m_labelPresenter.DesiredSize.Width != 0 ? m_labelPresenter.DesiredSize.Width : labelwidth;

                                if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                {
                                    if (!Adornment.Series.AdornmentsInfo.SegmentIsOut)
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) + labelwidth;
                                    }
                                    else
                                    {
                                        leftvalue = ((trpoint - blpoint) / 2) - labelwidth;
                                    }
                                    switch (Adornment.Series.AdornmentsInfo.SegmentVerticalAlignment)
                                    {
                                        case VerticalAlignment.Top:
                                            arrangePieRect.X = arrangePieRect.X + leftvalue;
                                            break;
                                        case VerticalAlignment.Bottom:
                                            arrangePieRect.X = arrangePieRect.X - leftvalue;
                                            break;
                                    }
                                    m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                    
                                }                               
                            }
                            #endregion
                            #region VerticalAllignment
                            switch (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment)
                            {
                                case System.Windows.HorizontalAlignment.Center:
                                    {
                                        if (Adornment.Series.Type != ChartTypes.Gantt)
                                        {
                                            if (Adornment.Series.Type != ChartTypes.StackingColumn100)
                                            {
                                                bool? forPositive = null;
                                                if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                                {
                                                    forPositive = Adornment.DataPoint.Y >= 0;
                                                }
                                                double y2 = Adornment.Series.Area.GetStackInfo(Adornment.Series, Adornment.DataPoint.X, forPositive);
                                                double y1 = Adornment.DataPoint.Y + y2;

                                                double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                                double leftvalue = 0d;
                                                labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                                leftvalue = (blpoint) - labelheight;
                                                if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                                {
                                                    arrangePieRect.Y = arrangePieRect.Y - (leftvalue / 2);
                                                    m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                                    m_symbolPresenter.RenderTransform = new RotateTransform(-90);
                                                }
                                            }
                                        }
                                        else
                                        {

                                            bool? forPositive = null;
                                            if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                            {
                                                forPositive = Adornment.DataPoint.Y >= 0;
                                            }
                                            double y2 = Adornment.DataPoint.Values[0];
                                            double y1 = Adornment.DataPoint.Values[1];

                                            double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                            double leftvalue = 0d;
                                            labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                            leftvalue = (blpoint) - labelheight;                                            
                                            if (Adornment.Series.Type == ChartTypes.Gantt)
                                            {
                                                arrangePieRect.Y = arrangePieRect.Y + (leftvalue);
                                                m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                                m_symbolPresenter.RenderTransform = new RotateTransform(-90);
                                            }

                                        }
                                    }
                                    break;
                                case System.Windows.HorizontalAlignment.Left:
                                    {
                                        if (Adornment.Series.Type != ChartTypes.Candle && Adornment.Series.Type != ChartTypes.RangeColumn && Adornment.Series.Type != ChartTypes.Gantt && Adornment.Series.Type != ChartTypes.StackingColumn100)
                                        {
                                            bool? forPositive = null;
                                            if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                            {
                                                forPositive = Adornment.DataPoint.Y >= 0;
                                            }
                                            double y2 = Adornment.Series.Area.GetStackInfo(Adornment.Series, Adornment.DataPoint.X, forPositive);
                                            double y1 = Adornment.DataPoint.Y + y2;

                                            double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                            double leftvalue = 0d;
                                            labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                            leftvalue = (blpoint) - labelheight;
                                            if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100)
                                            {
                                                arrangePieRect.Y = arrangePieRect.Y - leftvalue - (2 * labelwidth);
                                                m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                                m_symbolPresenter.RenderTransform = new RotateTransform(-90);
                                            }
                                        }
                                    }
                                    break;
                                case System.Windows.HorizontalAlignment.Right:
                                    if (Adornment.Series.Type == ChartTypes.Gantt)
                                    {
                                        bool? forPositive = null;
                                        if (ChartStackingColumnType.GetRequiresNegativeSeriesStack(Adornment.Series.Area))
                                        {
                                            forPositive = Adornment.DataPoint.Y >= 0;
                                        }
                                        double y2 = Adornment.DataPoint.Values[0];
                                        double y1 = Adornment.DataPoint.Values[1];

                                        double blpoint = Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y2) - Adornment.Series.Area.ValueToPoint(Adornment.Series.YAxis, y1);
                                        double leftvalue = 0d;
                                        labelheight = m_labelPresenter.DesiredSize.Height != 0 ? m_labelPresenter.DesiredSize.Height : labelheight;

                                        leftvalue = (blpoint) - labelheight;

                                        if (Adornment.Series.Type == ChartTypes.Gantt)
                                        {
                                            arrangePieRect.Y = arrangePieRect.Y + leftvalue;
                                            m_labelPresenter.RenderTransform = new RotateTransform(-90);
                                            m_symbolPresenter.RenderTransform = new RotateTransform(-90);
                                        }
                                    }
                                    break;
                            }
                            #endregion                                               
                            m_predefinedSymbol.RenderTransform = new RotateTransform(-90);
                            m_symbolPresenter.RenderTransform = new RotateTransform(-90);
                            break;
                    }
                }                
                if (!Adornment.Series.AdornmentsInfo.IsSegmentAlignment)
                {
                    arrangePieRect.X = arrangePieRect.X + Adornment.Series.AdornmentsInfo.OffsetX;
                    arrangePieRect.Y = arrangePieRect.Y + Adornment.Series.AdornmentsInfo.OffsetY;
                }
                
                if ((this.Adornment is ChartPieAdornment) && this.Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    this.ArrangePieAdornment();
                }
                else if ((this.Adornment is ChartAccumulationAdornment) && this.Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    this.ArrangePyramidAdornment();
                }
                else
                {
                    this.m_labelPresenter.Arrange(arrangePieRect);
                    this.m_symbolPresenter.Arrange(arrangeRect);
                    this.m_predefinedSymbol.Arrange(arrangeRect);
                }

                return this.DesiredSize;
            }            
            /// <summary>
            /// Arranges the pie adornment.
            /// </summary>
            private void ArrangePieAdornment()
            {
                ChartPieAdornment adornment = this.Adornment as ChartPieAdornment;

                Size cblSz = this.m_connectorPresenter.DesiredSize;
                Rect adrRect = new Rect();
                Rect connectorRect = new Rect();

                double offsetX = cblSz.Width;
                double offsetY = cblSz.Height;
                double explodedRadius = ChartPieType.GetExplodeRadius(adornment.Series);
                double explodeIndex = ChartPieType.GetExplodedIndex(adornment.Series);
                bool explodeAll = ChartPieType.GetExplodedAll(adornment.Series);

                // required to adjust the connector.
                int connectorOffset = 5;

                // required to adjust the adrRect.
                double adrRectOffset = 0.5;

                switch (adornment.ConnectorAlignment)
                {
                    case ConnectorAlignment.Left:
                        {
                            adrRect.X = 0;
                            connectorRect.X = m_adornmentSize.Width;
                            if (adornment.LabelAngle > Math.PI)
                            {
                                adrRect.Y = 0;
                                connectorRect.Y = m_adornmentSize.Height / 2;

                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    if (explodedRadius > 2)
                                    {
                                        connectorRect.X = m_adornmentSize.Width - explodedRadius - connectorOffset;
                                    }
                                    else
                                    {
                                        connectorRect.X = m_adornmentSize.Width - explodedRadius;
                                    }
                                }
                            }
                            else
                            {
                                adrRect.Y = Math.Max(offsetY, m_adornmentSize.Height / 2) - (m_adornmentSize.Height / 2);
                                connectorRect.Y = (m_adornmentSize.Height / 2) - offsetY;
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    if (explodedRadius >= 2)
                                    {
                                        connectorRect.X = m_adornmentSize.Width - explodedRadius;
                                        connectorRect.Y = (m_adornmentSize.Height / 2) - offsetY + explodedRadius;
                                    }
                                    else
                                    {
                                        connectorRect.X = m_adornmentSize.Width + explodedRadius;
                                        connectorRect.Y = (m_adornmentSize.Height / 2) - offsetY - explodedRadius;
                                    }
                                }
                            }

                            adrRect.Size = m_adornmentSize;
                            connectorRect.Size = cblSz;
                            if (ConnectorShift <= connectorRect.Width)
                            {
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    connectorRect.Width -= ConnectorShift - explodedRadius;
                                    if (adornment.LabelAngle < 3 || adornment.LabelAngle > 3.4)
                                    {
                                        adrRect.Y -= explodedRadius;
                                        adrRect.Height += explodedRadius + adrRectOffset;
                                    }
                                }
                                else
                                {
                                    connectorRect.Width -= ConnectorShift;
                                }
                            }
                            else
                            {
                                connectorRect.Width = 0;
                            }
                        }

                        break;

                    case ConnectorAlignment.Top:
                        {
                            adrRect.Y = 0;
                            connectorRect.Y = m_adornmentSize.Height - connectorOffset;
                            if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                            {
                                connectorRect.Y = m_adornmentSize.Height - explodedRadius - connectorOffset;
                            }

                            if (adornment.LabelAngle < 1.5 * Math.PI)
                            {
                                adrRect.X = Math.Max(offsetX, m_adornmentSize.Width / 2) - (m_adornmentSize.Width / 2);
                                connectorRect.X = Math.Max(offsetX, m_adornmentSize.Width / 2) - offsetX - adrRectOffset;
                            }
                            else
                            {
                                adrRect.X = 0;
                                connectorRect.X = m_adornmentSize.Width / 2;
                            }

                            adrRect.Size = m_adornmentSize;
                            connectorRect.Size = cblSz;
                            if (this.ConnectorShift <= connectorRect.Height)
                            {
                                connectorRect.Height -= ConnectorShift;
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    if (explodedRadius != 0.0)
                                    {
                                        if ((connectorRect.Width - (explodedRadius / 2) - .2) > 0)
                                        {
                                            connectorRect.Width -= (explodedRadius / 2) - .2;
                                        }
                                        else
                                        {
                                            connectorRect.Width = 0;
                                        }

                                        adrRect.Y -= (explodedRadius / 2) - adrRectOffset;
                                        adrRect.X += explodedRadius - connectorOffset;
                                        adrRect.Height += explodedRadius;
                                    }
                                }
                            }
                            else
                            {
                                connectorRect.Height = 0;
                            }
                        }

                        break;
                    case ConnectorAlignment.Right:
                        {
                            adrRect.X = offsetX;
                            connectorRect.X = 0;
                            if (adornment.LabelAngle > 0.25 * Math.PI)
                            {
                                adrRect.Y = 0;
                                connectorRect.Y = m_adornmentSize.Height / 2;
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    if (explodedRadius > 4)
                                    {
                                        connectorRect.Y = (m_adornmentSize.Height / 2) - (explodedRadius / 2);
                                    }
                                }
                            }
                            else
                            {
                                adrRect.Y = Math.Max(offsetY, m_adornmentSize.Height / 2) - (m_adornmentSize.Height / 2);
                                connectorRect.Y = Math.Max(offsetY, m_adornmentSize.Height / 2) - offsetY;
                                if (connectorRect.Height - ConnectorShift > 0)
                                {
                                    connectorRect.Height -= ConnectorShift;
                                }
                                else
                                {
                                    connectorRect.Height = 0;
                                }

                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    connectorRect.Y = Math.Max(offsetY, m_adornmentSize.Height / 2) - offsetY - explodedRadius - connectorOffset;
                                }
                            }

                            adrRect.Size = m_adornmentSize;
                            connectorRect.Size = cblSz;
                            if (ConnectorShift <= connectorRect.Width)
                            {
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    if (explodedRadius != 0.0)
                                    {
                                        adrRect.Y -= explodedRadius + adrRectOffset;
                                        connectorRect.X += ConnectorShift + explodedRadius + adrRectOffset;
                                    }
                                    else
                                    {
                                        connectorRect.X += ConnectorShift;
                                    }
                                }
                                else
                                {
                                    connectorRect.X += ConnectorShift;
                                }

                                connectorRect.Width -= ConnectorShift;
                            }
                            else
                            {
                                connectorRect.Width = 0;
                            }
                        }

                        break;
                    case ConnectorAlignment.Bottom:
                        {
                            adrRect.Y = offsetY;
                            connectorRect.Y = 0;
                            if (adornment.LabelAngle < 0.5 * Math.PI)
                            {
                                adrRect.X = Math.Max(offsetX, m_adornmentSize.Width / 2) - (m_adornmentSize.Width / 2);
                                connectorRect.X = Math.Max(offsetX, m_adornmentSize.Width / 2) - offsetX;
                            }
                            else
                            {
                                adrRect.X = 0;
                                connectorRect.X = m_adornmentSize.Width / 2;
                            }

                            adrRect.Size = m_adornmentSize;
                            connectorRect.Size = cblSz;
                            if (ConnectorShift <= connectorRect.Height)
                            {
                                connectorRect.Y += ConnectorShift;
                                connectorRect.Height -= ConnectorShift;
                                if (explodeAll == true || Convert.ToInt32(explodeIndex) == adornment.Index)
                                {
                                    connectorRect.Y += ConnectorShift + explodedRadius;
                                    adrRect.Y = offsetY + explodedRadius;
                                }
                            }
                            else
                            {
                                connectorRect.Height = 0;
                            }
                        }

                        break;

                    default:
                        adrRect = new Rect(DesiredSize);
                        connectorRect = new Rect(DesiredSize);
                        break;
                }

                m_labelPresenter.Arrange(adrRect);
                m_symbolPresenter.Arrange(adrRect);
                m_predefinedSymbol.Arrange(adrRect);
                m_connectorPresenter.Arrange(connectorRect);
            }

            /// <summary>
            /// Arranges the pyramid adornment.
            /// </summary>
            private void ArrangePyramidAdornment()
            {
                ChartPieAdornment adornment = this.Adornment as ChartPieAdornment;

                Size cblSz = m_connectorPresenter.DesiredSize;
                Rect adrRect = new Rect();
                Rect connectorRect = new Rect();
                int explodeIndex = ChartPyramidType.GetExplodedIndex(Adornment.Series);
                bool isExploded = explodeIndex == Adornment.Index;
                double explodedCenterOffset = 3;
                ScaleTransform connectorScale = null;
                ////Retrieving connector's scale transform object.
                if (this.m_connectorPresenter.RenderTransform is ScaleTransform)
                {
                    connectorScale = m_connectorPresenter.RenderTransform as ScaleTransform;
                }
                else
                {
                    connectorScale = new ScaleTransform();
                    m_connectorPresenter.RenderTransform = connectorScale;
                }
                ////Calculating scale factor in order to shrink the connector.              
                double scalefactor = 1 - (ConnectorShift / m_connectorPresenter.DesiredSize.Width);

                ////Setting scaling factor.
                connectorScale.ScaleX = scalefactor;
                if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Right)
                {
                    if ((Adornment.Series.Type == ChartTypes.Pyramid) && (scalefactor < 0))
                    {
                        scalefactor = 0;
                    }
                    else if ((Adornment.Series.Type == ChartTypes.Funnel) && (scalefactor < 0.5))
                    {
                        scalefactor = 0;
                    }

                    connectorScale.ScaleX = scalefactor;

                    if (m_connectorPresenter.RenderTransformOrigin.X != 1)
                    {
                        m_connectorPresenter.RenderTransformOrigin = new Point(1, 0);
                    }

                    adrRect.Size = m_adornmentSize;
                    connectorRect.Size = cblSz;
                    adrRect.X = cblSz.Width;

                    if (m_adornmentSize.Height > cblSz.Height)
                    {
                        connectorRect.Y = (m_adornmentSize.Height / 2) - (cblSz.Height / 2);
                    }
                    else
                    {
                        adrRect.Y = (cblSz.Height / 2) - (m_adornmentSize.Height / 2);
                    }

                    if (Adornment.Series.Type == ChartTypes.Funnel)
                    {
                        if (Adornment.Index == 0)
                        {
                            switch (Adornment.Series.AdornmentsInfo.SegmentVerticalAlignment)
                            {
                                case VerticalAlignment.Bottom:
                                    if (screenWidth > 724)
                                    {
                                        m_connectorPresenter.RenderTransformOrigin = new Point(1, 1);
                                        connectorScale.ScaleX = 0.4;
                                    }
                                    else
                                    {
                                        connectorScale.ScaleX = 0.4;
                                        connectorRect.X += 4;
                                    }

                                    break;
                                case VerticalAlignment.Top:
                                    if (screenWidth < 430)
                                    {
                                        connectorScale.ScaleX = 0.7;
                                    }

                                    break;
                                default:
                                    if (screenWidth <= 470)
                                    {
                                        connectorScale.ScaleX = 0.4;
                                    }
                                    else if ((screenWidth > 470) && (screenWidth < 595))
                                    {
                                        connectorScale.ScaleX = 0.6;
                                    }
                                    else if (screenWidth < 694)
                                    {
                                        connectorScale.ScaleX = 0.8;
                                    }

                                    break;
                            }
                        }
                    }

                    if (isExploded)
                    {
                        // For Pyramid Type and Funnel type having index greater than 0
                        if (scalefactor > 0.5)
                        {
                            if (Adornment.Index == Adornment.Series.Segments.Count - 1)
                            {
                                connectorScale.ScaleX = 0;
                            }
                            else
                            {
                                connectorRect.X += (m_adornmentSize.Width / 2) - explodedCenterOffset;
                                adrRect.X += (m_adornmentSize.Width / 2) - explodedCenterOffset;
                            }
                        }
                        else
                        {
                            scalefactor = 0;
                            connectorScale.ScaleX = scalefactor;
                        }
                    }
                }
                else if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Left)
                {
                    adrRect.Size = m_adornmentSize;
                    connectorRect.Size = cblSz;
                    connectorRect.X = m_adornmentSize.Width;

                    if (m_adornmentSize.Height > cblSz.Height)
                    {
                        connectorRect.Y = (m_adornmentSize.Height / 2) - (cblSz.Height / 2);
                        if (Adornment.Index == Adornment.Series.Segments.Count - 1)
                        {
                            if ((Adornment.Series.AdornmentsInfo.SegmentVerticalAlignment == VerticalAlignment.Bottom) && (Adornment.Series.Type == ChartTypes.Pyramid))
                            {
                                if (scalefactor < 0)
                                {
                                    scalefactor = 0.0;
                                }

                                connectorScale.ScaleX = scalefactor;
                            }
                        }

                        if ((Adornment.Series.Type == ChartTypes.Pyramid) && (Adornment.Index == Adornment.Series.Segments.Count - 1) && (Adornment.Series.AdornmentsInfo.SegmentLabelContent == LabelContent.YofTot))
                        {
                            if (screenWidth < 770)
                            {
                                scalefactor = 0.0;
                                connectorScale.ScaleX = scalefactor;
                            }
                        }
                    }
                    else
                    {
                        adrRect.Y = (cblSz.Height / 2) - (m_adornmentSize.Height / 2);
                    }

                    if (Adornment.Series.Type == ChartTypes.Funnel)
                    {
                        if (Adornment.Index == 0)
                        {
                            switch (Adornment.Series.AdornmentsInfo.SegmentVerticalAlignment)
                            {
                                case VerticalAlignment.Bottom:
                                    if (screenWidth > 585)
                                    {
                                        m_connectorPresenter.RenderTransformOrigin = new Point(0, 0);
                                        connectorScale.ScaleX = 0.4;
                                    }
                                    else
                                    {
                                        m_connectorPresenter.RenderTransformOrigin = new Point(0, 0);
                                        connectorScale.ScaleX = 0.4;
                                        connectorRect.X -= 5;
                                    }

                                    break;
                                case VerticalAlignment.Top:
                                    if (screenWidth < 430)
                                    {
                                        connectorScale.ScaleX = 0.7;
                                    }

                                    break;
                                default:
                                    if (screenWidth <= 470)
                                    {
                                        connectorScale.ScaleX = 0.4;
                                    }
                                    else if ((screenWidth > 470) && (screenWidth < 595))
                                    {
                                        connectorScale.ScaleX = 0.6;
                                    }
                                    else if (screenWidth < 694)
                                    {
                                        connectorScale.ScaleX = 0.8;
                                    }

                                    break;
                            }
                        }

                        if ((Adornment.Index == Adornment.Series.Segments.Count - 1) && (Adornment.Series.AdornmentsInfo.SegmentLabelContent == LabelContent.YofTot))
                        {
                            if (screenWidth < 770)
                            {
                                scalefactor = 0.0;
                                connectorScale.ScaleX = scalefactor;
                            }
                        }
                    }
                }

                m_labelPresenter.Arrange(adrRect);
                m_symbolPresenter.Arrange(adrRect);
                m_predefinedSymbol.Arrange(adrRect);
                m_connectorPresenter.Arrange(connectorRect);
            }
            private double labelwidth = 0d;
            private double labelheight = 0d;
            internal Rect arrangePieRect = new Rect();
            internal double customOffsetX = 0d, customOffsetY = 0d;

            /// <summary>
            /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
            /// </summary>
            /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
            protected override void OnRender(DrawingContext drawingContext)
            {
                if (Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    if ((Adornment.Series.AdornmentsInfo.OffsetX != 0 || Adornment.Series.AdornmentsInfo.OffsetY != 0) && Adornment.Series.AdornmentsInfo.SegmentShowLine && !Adornment.Series.AdornmentsInfo.IsSegmentAlignment && (Adornment.Series.Type != ChartTypes.Pie || Adornment.Series.Type != ChartTypes.Doughnut || Adornment.Series.Type != ChartTypes.Funnel || Adornment.Series.Type != ChartTypes.Pyramid))
                    {
                        drawingContext.DrawLine(new Pen(Brushes.Black, 0.75), new Point(m_symbolOffset.X, m_symbolOffset.Y), new Point((arrangePieRect.X), arrangePieRect.Y));
                    }
                }

                if (Adornment.Series.ShowSmartLabels)
                {
                    if (customOffsetX != 0 || customOffsetY != 0)
                    {
                        if (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.Gantt)
                        {
                            drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(this.m_adornmentSize.Width, this.m_adornmentSize.Height/2), new Point(this.customOffsetX, this.customOffsetY));
                        }
                        else
                        {
                            drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(this.m_adornmentSize.Width / 2, this.m_adornmentSize.Height), new Point(this.customOffsetX, this.customOffsetY));
                        }
                    }
                }
                base.OnRender(drawingContext);
            }
            /// <summary>
            /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
            /// </summary>
            /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
            /// <returns>
            /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
            /// </returns>
            protected override Size MeasureOverride(Size availableSize)
            {
                if (Adornment.Series.AdornmentsInfo.IsLabelRotate)
                {
                    m_labelPresenter.LayoutTransform = new RotateTransform(Adornment.Series.AdornmentsInfo.SegmentLabelRotation);
                    m_symbolPresenter.LayoutTransform = new RotateTransform(Adornment.Series.AdornmentsInfo.SegmentLabelRotation);
                }               
                screenHeight = availableSize.Height;
                screenWidth = availableSize.Width;
                m_labelPresenter.Measure(availableSize);
                m_symbolPresenter.Measure(availableSize);
                m_predefinedSymbol.Measure(availableSize);

                Size resultSize = new Size();
                Size lblSz = m_labelPresenter.DesiredSize;
                Size sblSz;
                if (Adornment.Series.AdornmentsInfo.Symbol == Symbol.Custom)
                {
                    sblSz = m_symbolPresenter.DesiredSize;
                }
                else
                {
                    sblSz = m_predefinedSymbol.DesiredSize;
                }
                bool isAccumulatedChart = (this.Adornment.Series != null && this.Adornment.Series.Area != null) ? ChartArea.CheckCompatibility(this.Adornment.Series.ChartType) : false;
                switch (LabelHorizontalAlignment)
                {
                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Center:
                        {
                            resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                            m_symbolOffset.X = 0.5 * resultSize.Width;
                        }

                        break;

                    case HorizontalAlignment.Left:
                        if (!isAccumulatedChart)
                        {
                            resultSize.Width = lblSz.Width + sblSz.Width;
                            m_symbolOffset.X = 0.5 * sblSz.Width + lblSz.Width;
                        }
                        else
                        {
                            resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                            m_symbolOffset.X = 0.5 * resultSize.Width;
                        }
                        break;

                    case HorizontalAlignment.Right:
                        if (!isAccumulatedChart)
                        {
                            resultSize.Width = lblSz.Width + sblSz.Width;
                            m_symbolOffset.X = 0.5 * sblSz.Width;
                        }
                        else
                        {
                            resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                            m_symbolOffset.X = 0.5 * resultSize.Width;
                        }
                        break;
                }

                switch (LabelVerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        if (!isAccumulatedChart)
                        {
                            resultSize.Height = lblSz.Height + sblSz.Height;
                            m_symbolOffset.Y = 0.5 * sblSz.Height;
                        }
                        else
                        {
                            resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                            m_symbolOffset.Y = 0.5 * resultSize.Height;
                        }
                        break;
                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Center:
                        {
                            resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                            m_symbolOffset.Y = 0.5 * resultSize.Height;
                        }

                        break;
                    case VerticalAlignment.Top:
                        if (!isAccumulatedChart)
                        {
                            resultSize.Height = lblSz.Height + sblSz.Height;
                            m_symbolOffset.Y = 0.5 * sblSz.Height + lblSz.Height;
                        }
                        else
                        {
                            resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                            m_symbolOffset.Y = 0.5 * resultSize.Height;
                        }
                        break;
                }




                if (Adornment.Series.Type == ChartTypes.Pie && !Adornment.Series.AdornmentsInfo.SegmentIsOut && Adornment.Series.AdornmentsInfo.IsLabelRotate && !Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    double totalValue = 0d;
                    double ActualTotalValue = 0d;
                    for (int i = 0; i < Adornment.Series.Data.Count; i++)
                    {
                        totalValue = (totalValue + Adornment.Series.Data[i].Y);
                        if (i <= Adornment.Index)
                        {
                            ActualTotalValue = ActualTotalValue + Adornment.Series.Data[i].Y;
                        }
                    }

                    double index = (Adornment.Index) == 0 ? 0 : Adornment.Series.Data[Adornment.Index - 1].Y;
                    double startAngle = (ActualTotalValue * 360) / totalValue;
                    double endAngle = ((ActualTotalValue - Adornment.DataPoint.Y) * 360) / totalValue;
                    double angle = (endAngle + startAngle) / 2;
                    bool isrotate = false;
                    if (angle > 170 && angle < 210)
                    {
                        angle = angle + 180;
                        isrotate = true;
                    }
                    TransformGroup group = new TransformGroup();
                    group.Children.Add(new RotateTransform(angle));
                    if (isrotate == false)
                        group.Children.Add(new TranslateTransform(resultSize.Width / 2, 0));
                    else
                        group.Children.Add(new TranslateTransform(-resultSize.Width / 2, 0));

                    m_labelPresenter.RenderTransform = group;
                }
                if (Adornment.Series.Type == ChartTypes.Pie && !Adornment.Series.AdornmentsInfo.SegmentIsOut && Adornment.Series.AdornmentsInfo.IsLabelRotate && !Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    double totalValue = 0d;
                    double ActualTotalValue = 0d;
                    for (int i = 0; i < Adornment.Series.Data.Count; i++)
                    {
                        totalValue = (totalValue + Adornment.Series.Data[i].Y);
                        if (i <= Adornment.Index)
                        {
                            ActualTotalValue = ActualTotalValue + Adornment.Series.Data[i].Y;
                        }
                    }

                    double index = (Adornment.Index) == 0 ? 0 : Adornment.Series.Data[Adornment.Index - 1].Y;
                    double startAngle = (ActualTotalValue * 360) / totalValue;
                    double endAngle = ((ActualTotalValue - Adornment.DataPoint.Y) * 360) / totalValue;
                    double angle = (endAngle + startAngle) / 2;
                    bool isrotate = false;
                    if (angle > 170 && angle < 210)
                    {
                      //  angle = angle + 180;
                        //isrotate = true;
                    }
                    TransformGroup group = new TransformGroup();
                    group.Children.Add(new RotateTransform(angle));
                    if (isrotate == false)
                        group.Children.Add(new TranslateTransform(resultSize.Width / 2, 0));
                    else
                        group.Children.Add(new TranslateTransform(-resultSize.Width / 2, 0));

                    m_labelPresenter.RenderTransform = group;
                }
                m_adornmentSize = resultSize;

                TransformGroup transformgroup = new TransformGroup();

                #region SymmetricLabelling for Adornments
                if (Adornment.Series.AdornmentsInfo.m_requiresSymmetricLabelling)
                {
                    if (Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom && Adornment.m_point.Y < 0 &&
                        (Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100))
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                    }
                    else if ((Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom && Adornment.m_point.Y < 0 &&
                             (Adornment.Series.Type == ChartTypes.Column || Adornment.Series.Type == ChartTypes.StackingColumn || Adornment.Series.Type == ChartTypes.StackingColumn100 || Adornment.Series.Type == ChartTypes.FastStackingColumn)) ||
                             (Adornment.m_point.Y < 0 && (Adornment.Series.Type == ChartTypes.Area || Adornment.Series.Type == ChartTypes.SplineArea || Adornment.Series.Type == ChartTypes.StackingArea || Adornment.Series.Type == ChartTypes.StackingArea100 || 
                              Adornment.Series.Type == ChartTypes.StepArea || Adornment.Series.Type == ChartTypes.StackingLine || Adornment.Series.Type == ChartTypes.StackingLine100 || Adornment.Series.Type == ChartTypes.StackingSpline ||
                              Adornment.Series.Type == ChartTypes.StackingSpline100 || Adornment.Series.Type == ChartTypes.StackingSplineArea || Adornment.Series.Type == ChartTypes.StackingSplineArea100)))
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                    }
                }
                if ((Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.Tornado) && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    double highVal = (Adornment.DataPoint.Values[0] > Adornment.DataPoint.Values[1]) ? Adornment.DataPoint.Values[0] : Adornment.DataPoint.Values[1];
                    if (Adornment.m_point.Y == highVal)
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                    }
                }
                if ((Adornment.Series.Type == ChartTypes.HiLoOpenClose || Adornment.Series.Type == ChartTypes.FastHiLoOpenClose) && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    double lowVal = (Adornment.DataPoint.Values[0] < Adornment.DataPoint.Values[1]) ? Adornment.DataPoint.Values[0] : Adornment.DataPoint.Values[1];
                    double closeVal = (Adornment.DataPoint.Values[2] < Adornment.DataPoint.Values[3]) ? Adornment.DataPoint.Values[2] : Adornment.DataPoint.Values[3];
                    if (Adornment.m_point.Y == closeVal || Adornment.m_point.Y == lowVal)
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                    }
                }
                if ((Adornment.Series.Type == ChartTypes.RangeColumn || Adornment.Series.Type == ChartTypes.RangeArea || Adornment.Series.Type == ChartTypes.HiLo) &&
                     Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    double lowVal = (Adornment.DataPoint.Values[0] < Adornment.DataPoint.Values[1]) ? Adornment.DataPoint.Values[0] : Adornment.DataPoint.Values[1];

                    if (Adornment.m_point.Y == lowVal)
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                    }
                }
                if (Adornment.Series.Type == ChartTypes.HiLoArea && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    List<ChartIndexedDataPoint> points = (Adornment.Series.ChartType as ChartHiLoAreaType).segmentSummaryPoints;
                    double lowVal = (points[Adornment.Index].DataPoint.Values[0] < points[Adornment.Index].DataPoint.Values[1]) ? points[Adornment.Index].DataPoint.Values[0] : points[Adornment.Index].DataPoint.Values[1];

                    if (Adornment.m_point.Y == lowVal)
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                    }
                }
                if ((Adornment.Series.Type == ChartTypes.Candle || Adornment.Series.Type == ChartTypes.HiLoOpenClose || Adornment.Series.Type == ChartTypes.FastHiLoOpenClose) &&
                     Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    double lowVal = (Adornment.DataPoint.Values[0] < Adornment.DataPoint.Values[1]) ? Adornment.DataPoint.Values[0] : Adornment.DataPoint.Values[1];
                    double closeVal = (Adornment.DataPoint.Values[2] < Adornment.DataPoint.Values[3]) ? Adornment.DataPoint.Values[2] : Adornment.DataPoint.Values[3];

                    if (Adornment.m_point.Y == lowVal || Adornment.m_point.Y == closeVal)
                    {
                        if (!Adornment.Series.IsRotated)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, m_adornmentSize.Height));
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, -m_adornmentSize.Height));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(m_adornmentSize.Width, 0));
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-m_adornmentSize.Width, 0));
                        }
                    }
                }

                #endregion

                double reqHeight = (-m_adornmentSize.Height);
                double reqWidth = (m_adornmentSize.Width);

                #region Primary Axis Start & End Adornment Label position check
                if (this.Adornment.m_point.X == this.Adornment.Series.XAxis.VisibleRange.Start)
                {
                    if (Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100 || Adornment.Series.Type == ChartTypes.Tornado || Adornment.Series.Type == ChartTypes.RotatedSpline)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                    }
                    else if (Adornment.Series.Type != ChartTypes.Pie && Adornment.Series.Type != ChartTypes.Doughnut && Adornment.Series.Type != ChartTypes.Funnel &&
                             Adornment.Series.Type != ChartTypes.Pyramid && Adornment.Series.Type != ChartTypes.Polar && Adornment.Series.Type != ChartTypes.Radar)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                }
                else if (this.Adornment.m_point.X == this.Adornment.Series.XAxis.VisibleRange.End)
                {
                    if (Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar || Adornment.Series.Type == ChartTypes.StackingBar100 || Adornment.Series.Type == ChartTypes.Tornado || Adornment.Series.Type == ChartTypes.RotatedSpline)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                    }
                    else if (Adornment.Series.Type != ChartTypes.Pie && Adornment.Series.Type != ChartTypes.Doughnut && Adornment.Series.Type != ChartTypes.Funnel &&
                             Adornment.Series.Type != ChartTypes.Pyramid && Adornment.Series.Type != ChartTypes.Polar && Adornment.Series.Type != ChartTypes.Radar)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.XAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                }
                
                #endregion

                #region Secondary Axis Start & End Adornment Label position check
                if (Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (Adornment.Series.Type == ChartTypes.Column || Adornment.Series.Type == ChartTypes.Bar))//(this.Adornment.DataPoint).X == this.Adornment.Series.XAxis.VisibleRange.Start)
                {
                    double originy = Adornment.Series.Area.SecondaryAxis.Origin;
                    double originx = Adornment.Series.Area.PrimaryAxis.Origin;

                    if (Adornment.Series.Type == ChartTypes.Column && originx == this.Adornment.Series.YAxis.VisibleRange.Start)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                        }
                    }
                    else if (Adornment.Series.Type == ChartTypes.Bar && originx == this.Adornment.Series.YAxis.VisibleRange.Start)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                        }
                    }
                }

                #region 100% Stacked Charts
                if (Adornment.Series.Type == ChartTypes.StackingColumn100)
                {
                    bool showValueAsProbability = ChartStackingColumn100Type.GetShowValueAsProbability(Adornment.Series.Area);
                    double start = showValueAsProbability ? this.Adornment.m_point.Y : Math.Round(this.Adornment.m_point.Y);
                    double end = showValueAsProbability ? this.Adornment.m_point.Y : Math.Round(this.Adornment.m_point.Y);

                    if (start == this.Adornment.Series.YAxis.VisibleRange.Start)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y > 0) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y < 0))
                                transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                        }
                        else
                        {
                            if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y > 0) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y < 0))
                                transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                        }
                    }
                    else if (end == this.Adornment.Series.YAxis.VisibleRange.End)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                            else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                        }
                    }
                }
                if (Adornment.Series.Type == ChartTypes.StackingBar100)
                {
                    bool showValueAsProbability = ChartStackingBar100Type.GetShowValueAsProbability(Adornment.Series.Area);
                    double start = showValueAsProbability ? this.Adornment.m_point.Y : Math.Round(this.Adornment.m_point.Y);
                    double end = showValueAsProbability ? this.Adornment.m_point.Y : Math.Round(this.Adornment.m_point.Y);

                    if (start == this.Adornment.Series.YAxis.VisibleRange.Start)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                        }
                        else
                        {
                            if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                        }
                    }
                    else if (end == this.Adornment.Series.YAxis.VisibleRange.End)
                    {
                        if (!this.Adornment.Series.YAxis.IsInversed)
                        {
                            if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.m_point.Y > 0) ||
                                (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.m_point.Y < 0))
                                transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                        }
                        else
                        {
                            if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.m_point.Y > 0) ||
                                (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.m_point.Y < 0))
                                transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                            else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                        }
                    }
                } 
                #endregion

                #region Other Charts
                if (this.Adornment.m_point.Y == this.Adornment.Series.YAxis.VisibleRange.Start)
                {
                    if (Adornment.Series.Type != ChartTypes.Pie && Adornment.Series.Type != ChartTypes.Bar && Adornment.Series.Type != ChartTypes.Doughnut && Adornment.Series.Type != ChartTypes.Funnel &&
                        Adornment.Series.Type != ChartTypes.Pyramid && Adornment.Series.Type != ChartTypes.Polar && Adornment.Series.Type != ChartTypes.Radar && Adornment.Series.Type != ChartTypes.RotatedSpline &&
                        Adornment.Series.Type != ChartTypes.StackingBar && Adornment.Series.Type != ChartTypes.StackingBar100 && Adornment.Series.Type != ChartTypes.StackingColumn100 &&
                        Adornment.Series.Type != ChartTypes.Gantt && Adornment.Series.Type != ChartTypes.Tornado)
                    {
                        if (((Adornment.Series.Type == ChartTypes.Column || Adornment.Series.Type == ChartTypes.FastStackingColumn || Adornment.Series.Type == ChartTypes.StackingColumn) && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom) ||
                             (Adornment.Series.Type == ChartTypes.Area || Adornment.Series.Type == ChartTypes.SplineArea || Adornment.Series.Type == ChartTypes.StackingArea || Adornment.Series.Type==ChartTypes.StackingArea100 || Adornment.Series.Type == ChartTypes.StepArea ||
                             Adornment.Series.Type == ChartTypes.StackingLine || Adornment.Series.Type == ChartTypes.StackingLine100 || Adornment.Series.Type == ChartTypes.StackingSpline || Adornment.Series.Type == ChartTypes.StackingSpline100 || Adornment.Series.Type == ChartTypes.StackingSplineArea || Adornment.Series.Type == ChartTypes.StackingSplineArea100))
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y > 0) ||
                                        (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y < 0))
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y < 0) ||
                                        (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y > 0))
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                        else if (Adornment.Series.Type == ChartTypes.RangeColumn || Adornment.Series.Type == ChartTypes.RangeArea || Adornment.Series.Type == ChartTypes.Candle ||
                                 Adornment.Series.Type == ChartTypes.HiLo || Adornment.Series.Type == ChartTypes.HiLoArea || Adornment.Series.Type == ChartTypes.HiLoOpenClose)
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom) ||
                                        (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom))
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom) ||
                                        (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom))
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom) ||
                                        (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom) ||
                                        (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                        else if (Adornment.Series.Type != ChartTypes.RangeColumn && Adornment.Series.Type != ChartTypes.Column)
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                    }
                    if ((Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar) && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y > 0) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y < 0))
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.m_point.Y < 0) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.m_point.Y > 0))
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                    else if (Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.Tornado || Adornment.Series.Type == ChartTypes.RotatedSpline ||
                            (Adornment.Series.Type == ChartTypes.StackingBar && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom))
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                }
                else if (this.Adornment.m_point.Y == this.Adornment.Series.YAxis.VisibleRange.End)
                {
                    if (Adornment.Series.Type != ChartTypes.Pie && Adornment.Series.Type != ChartTypes.Bar && Adornment.Series.Type != ChartTypes.Doughnut && Adornment.Series.Type != ChartTypes.Funnel &&
                        Adornment.Series.Type != ChartTypes.Pyramid && Adornment.Series.Type != ChartTypes.Polar && Adornment.Series.Type != ChartTypes.Radar && Adornment.Series.Type != ChartTypes.RotatedSpline &&
                        Adornment.Series.Type != ChartTypes.StackingBar && Adornment.Series.Type != ChartTypes.StackingBar100 && Adornment.Series.Type != ChartTypes.StackingColumn100 &&
                        Adornment.Series.Type != ChartTypes.Gantt && Adornment.Series.Type != ChartTypes.Tornado)
                    {
                        if (((Adornment.Series.Type == ChartTypes.Column || Adornment.Series.Type == ChartTypes.FastStackingColumn || Adornment.Series.Type == ChartTypes.StackingColumn || Adornment.Series.Type == ChartTypes.FastStackingColumn) && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom) ||
                             (Adornment.Series.Type == ChartTypes.Area || Adornment.Series.Type == ChartTypes.SplineArea || Adornment.Series.Type == ChartTypes.StackingArea || Adornment.Series.Type == ChartTypes.StackingArea100 || Adornment.Series.Type == ChartTypes.StepArea ||
                             Adornment.Series.Type == ChartTypes.StackingLine || Adornment.Series.Type == ChartTypes.StackingLine100 || Adornment.Series.Type == ChartTypes.StackingSpline || Adornment.Series.Type == ChartTypes.StackingSpline100 || Adornment.Series.Type == ChartTypes.StackingSplineArea || Adornment.Series.Type == ChartTypes.StackingSplineArea100))
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                        else if (Adornment.Series.Type == ChartTypes.RangeColumn || Adornment.Series.Type == ChartTypes.RangeArea || Adornment.Series.Type == ChartTypes.Candle ||
                                 Adornment.Series.Type == ChartTypes.HiLo || Adornment.Series.Type == ChartTypes.HiLoArea || Adornment.Series.Type == ChartTypes.HiLoOpenClose)
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                        else if (Adornment.Series.Type != ChartTypes.RangeColumn && Adornment.Series.Type != ChartTypes.Column)
                        {
                            if (!this.Adornment.Series.IsRotated)
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                        transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                        transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                    else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                                }
                            }
                            else
                            {
                                if (!this.Adornment.Series.YAxis.IsInversed)
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                        transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                                }
                                else
                                {
                                    if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                        transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                    else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                        transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                                }
                            }
                        }
                    }
                    if ((Adornment.Series.Type == ChartTypes.Bar || Adornment.Series.Type == ChartTypes.StackingBar) && Adornment.Series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.Bottom)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.m_point.Y > 0) ||
                                        (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.m_point.Y < 0))
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.m_point.Y > 0) ||
                                        (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.m_point.Y < 0))
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                    else if (Adornment.Series.Type == ChartTypes.Gantt || Adornment.Series.Type == ChartTypes.Tornado)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top) ||
                                    (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if ((Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top) ||
                                        (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if ((Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top) ||
                                    (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top && Adornment.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom))
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                    else if (Adornment.Series.Type == ChartTypes.RotatedSpline)
                    {
                        if (!this.Adornment.Series.IsRotated)
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Right)
                                    transformgroup.Children.Add(new TranslateTransform(-reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? -reqWidth / 2 : -(sblSz.Width + lblSz.Width) / 2), 0));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Left)
                                    transformgroup.Children.Add(new TranslateTransform(reqWidth, 0));
                                else if (Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Center || Adornment.Series.AdornmentsInfo.HorizontalAlignment == HorizontalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform((sblSz.Width == 0 ? reqWidth / 2 : (sblSz.Width + lblSz.Width) / 2), 0));
                            }
                        }
                        else
                        {
                            if (!this.Adornment.Series.YAxis.IsInversed)
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Top)
                                    transformgroup.Children.Add(new TranslateTransform(0, -reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? -reqHeight / 2 : (sblSz.Height + lblSz.Height) / 2)));
                            }
                            else
                            {
                                if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                                    transformgroup.Children.Add(new TranslateTransform(0, reqHeight));
                                else if (Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center || Adornment.Series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Stretch)
                                    transformgroup.Children.Add(new TranslateTransform(0, (sblSz.Height == 0 ? reqHeight / 2 : -(sblSz.Height + lblSz.Height) / 2)));
                            }
                        }
                    }
                } 
                #endregion
                #endregion

                m_labelPresenter.RenderTransform = transformgroup;

                if (Adornment is ChartPieAdornment && Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    resultSize = MeasurePieAdornment(availableSize, resultSize);
                }
                else if (Adornment is ChartAccumulationAdornment && Adornment.Series.AdornmentsInfo.SegmentShowLine)
                {
                    resultSize = MeasurePyramidAdornment(availableSize, resultSize);
                }

                return resultSize;
            }

            /// <summary>
            /// Measures the pie adornment.
            /// </summary>
            /// <param name="availableSize">Size of the available.</param>
            /// <param name="resultSize">Size of the result.</param>
            /// <returns>Returns the size</returns>
            private Size MeasurePieAdornment(Size availableSize, Size resultSize)
            {
                m_connectorPresenter.LayoutTransform = new RotateTransform(ChartMath.ToDegree * (Adornment as ChartPieAdornment).LabelAngle);
                m_connectorPresenter.Measure(availableSize);

                ChartPieAdornment adornment = Adornment as ChartPieAdornment;

                Size cblSz = m_connectorPresenter.DesiredSize;

                double xOffset = cblSz.Width;
                double yOffset = cblSz.Height;

                m_symbolOffset = new Vector();

                switch (adornment.ConnectorAlignment)
                {
                    case ConnectorAlignment.Left:
                        {
                            resultSize.Width = resultSize.Width + xOffset;
                            resultSize.Height = resultSize.Height / 2 + Math.Max(yOffset, resultSize.Height / 2);
                            m_symbolOffset.X = resultSize.Width;

                            if (adornment.LabelAngle > Math.PI)
                            {
                                m_symbolOffset.Y = m_adornmentSize.Height / 2 + yOffset;
                            }
                            else
                            {
                                m_symbolOffset.Y = m_adornmentSize.Height / 2 - yOffset;
                            }
                        }

                        break;

                    case ConnectorAlignment.Top:
                        {
                            resultSize.Width = resultSize.Width / 2 + Math.Max(xOffset, resultSize.Width / 2);
                            resultSize.Height = resultSize.Height + yOffset;
                            m_symbolOffset.Y = resultSize.Height;

                            if (adornment.LabelAngle > 1.5 * Math.PI)
                            {
                                m_symbolOffset.X = m_adornmentSize.Width / 2 + xOffset;
                            }
                            else
                            {
                                m_symbolOffset.X = m_adornmentSize.Width / 2 - xOffset;
                            }
                        }

                        break;
                    case ConnectorAlignment.Right:
                        {
                            resultSize.Width = resultSize.Width + xOffset;
                            resultSize.Height = resultSize.Height / 2 + Math.Max(yOffset, resultSize.Height / 2);
                            m_symbolOffset.X = 0;

                            if (adornment.LabelAngle < 0.25 * Math.PI)
                            {
                                m_symbolOffset.Y = m_adornmentSize.Height / 2 - yOffset;
                            }
                            else
                            {
                                m_symbolOffset.Y = m_adornmentSize.Height / 2 + yOffset;
                            }
                        }

                        break;
                    case ConnectorAlignment.Bottom:
                        {
                            resultSize.Width = resultSize.Width / 2 + Math.Max(xOffset, resultSize.Width / 2);
                            resultSize.Height = resultSize.Height + yOffset;

                            if (adornment.LabelAngle > 0.5 * Math.PI)
                            {
                                m_symbolOffset.X = m_adornmentSize.Width / 2 + xOffset;
                            }
                            else
                            {
                                m_symbolOffset.X = m_adornmentSize.Width / 2 - xOffset;
                            }
                        }

                        break;
                }


                return resultSize;
            }

            /// <summary>
            /// Measures the pyramid adornment.
            /// </summary>
            /// <param name="availableSize">Size of the available.</param>
            /// <param name="resultSize">Size of the result.</param>
            /// <returns>Returns the size</returns>
            private Size MeasurePyramidAdornment(Size availableSize, Size resultSize)
            {
                m_symbolOffset = new Vector();

                if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Right)
                {
                    m_connectorPresenter.LayoutTransform = new RotateTransform(0);
                    m_connectorPresenter.Measure(availableSize);
                }
                else if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Left)
                {
                    m_connectorPresenter.LayoutTransform = new RotateTransform(180);
                    m_connectorPresenter.Measure(availableSize);
                }

                Size cblSz = m_connectorPresenter.DesiredSize;

                if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Right)
                {
                    m_symbolOffset.X = 0;
                }
                else if (Adornment.Series.AdornmentsInfo.SegmentHorizontalAlignment == HorizontalAlignment.Left)
                {
                    m_symbolOffset.X = resultSize.Width + cblSz.Width;
                }

                resultSize.Width = resultSize.Width + cblSz.Width;
                resultSize.Height = Math.Max(cblSz.Height, resultSize.Height);

                m_symbolOffset.Y = resultSize.Height / 2;

                return resultSize;
            }

            ResourceDictionary baseRD = null;

            /// <summary>
            /// Sets the symbol.
            /// </summary>
            /// <param name="symbol">The symbol.</param>
            /// <param name="baseRD"></param>
            private void SetSymbol(string symbol, ResourceDictionary baseRD)
            {
                if (symbol == "Custom")
                {
                    SetContentBinding(m_ardorment);
                    this.m_predefinedSymbol.Template = null;
                    Adornment.Series.AdornmentSymbolTemplate = null;
                }
                else
                {
                    this.m_symbolPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
                    //if (baseRD == null)
                    //{
                      //  baseRD = new SharedResourceDictionary()
                      //  {
                      //      Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Base.xaml", UriKind.RelativeOrAbsolute)
                      //  };
                    //}
                    switch (symbol)
                    {
                        case "Cross":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Cross"] as ControlTemplate;
                            break;
                        case "Diamond":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Diamond"] as ControlTemplate;
                            break;
                        case "Hexagon":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Hexagon"] as ControlTemplate;
                            break;
                        case "HorizontalLine":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_HorizoneLine"] as ControlTemplate;
                            break;
                        case "Image":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Ellipse"] as ControlTemplate;
                            break;
                        case "InvertedTriangle":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_InvertedTriangle"] as ControlTemplate;
                            break;
                        case "Pentagon":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Pentagon"] as ControlTemplate;
                            break;
                        case "Plus":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Plus"] as ControlTemplate;
                            break;
                        case "Square":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Square"] as ControlTemplate;
                            break;
                        case "Triangle":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_Triangle"] as ControlTemplate;
                            break;
                        case "VerticalLine":
                            this.m_predefinedSymbol.Template = baseRD["Symbol_VerticalLine"] as ControlTemplate;
                            break;
                        case "Ellipse":
                            this.m_predefinedSymbol.Template = baseRD["Circle"] as ControlTemplate;
                            break;
                    }

                    Adornment.Series.AdornmentSymbolTemplate = this.m_predefinedSymbol.Template;
                    Adornment.Series.AdornmentSymbolInterior = this.Adornment.Series.AdornmentsInfo.SymbolInterior == null ? this.Adornment.Series.Interior : this.Adornment.Series.AdornmentsInfo.SymbolInterior;
                    Adornment.Series.AdornmentSymbolStroke = this.Adornment.Series.AdornmentsInfo.SymbolStroke == null ? this.Adornment.Series.Stroke : this.Adornment.Series.AdornmentsInfo.SymbolStroke;
                    Adornment.Series.AdornmentSymbolStrokeThickness = double.IsNaN(Adornment.Series.AdornmentsInfo.SymbolStrokeThickness) ? this.Adornment.Series.StrokeThickness : this.Adornment.Series.AdornmentsInfo.SymbolStrokeThickness;

                     }
            }

            /// <summary>
            /// Called when adornments info property changed.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
            private void OnAdornmentsInfoChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                //  this.m_ardorment.Series.AdornmentsInfo.PropertyChanged -= this.OnAdornmentsInfoChanged;
                if (this.m_ardorment != null && this.m_ardorment.Series != null)
                {
                    if (e.Property == ChartAdornmentInfo.LabelContentPathProperty)
                    {
                        SetContentBinding(m_ardorment);
                    }

                    if (e.Property == ChartAdornmentInfo.SymbolProperty || e.Property == ChartAdornmentInfo.SymbolInteriorProperty || e.Property == ChartAdornmentInfo.SymbolStrokeProperty || e.Property == ChartAdornmentInfo.SymbolStrokeThicknessProperty)
                    {
                        SetSymbol(Adornment.Series.AdornmentsInfo.Symbol.ToString(),baseRD);
                    }
                    if (e.Property == ChartAdornmentInfo.ConnectorTemplateProperty)
                    {
                        //if (m_connectorPresenter != null && m_connectorPresenter.ContentTemplate == null)
                        //this.Adornment.Series.RaiseAppearanceChanged(this.Adornment.Series, EventArgs.Empty);
                    }
                    if (e.Property == ChartAdornmentInfo.SegmentShowLineProperty)
                    {
                        this.InvalidateVisual();
                    }
                }
            }

            /// <summary>
            /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from a collection of child elements.
            /// </summary>
            /// <param name="index">The zero-based index of the requested child element in the collection.</param>
            /// <returns>
            /// The requested child element. This should not return null; if the provided index is out of range, an exception is raised.
            /// </returns>
            protected override Visual GetVisualChild(int index)
            {
                UIElement element;

                switch (index)
                {
                    case 0:
                        {
                            if (Adornment.Series.AdornmentsInfo != null)
                            {
                                if (Adornment.Series.AdornmentsInfo.Symbol == Symbol.Custom)
                                {
                                    element = m_symbolPresenter;
                                }

                                else
                                {
                                    element = m_predefinedSymbol;
                                }
                            }
                            else
                                element = m_predefinedSymbol;
                        }

                        break;

                    case 1:
                        element = m_labelPresenter;
                        break;

                    case 2:
                        element = m_connectorPresenter;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("index", index, "Index can be 0 or 1.");

                }

                return element;
            }
            #endregion

            #region IDisposable Members
            internal void Clear()
            {
                 if(this.m_ardorment.Series.AdornmentsInfo!=null)
                this.m_ardorment.Series.AdornmentsInfo.PropertyChanged -= this.OnAdornmentsInfoChanged;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if(this.m_ardorment.Series.AdornmentsInfo!=null)
                this.m_ardorment.Series.AdornmentsInfo.PropertyChanged -= this.OnAdornmentsInfoChanged;
                this.Resources = null;
                this.m_ardorment.Dispose();
                this.m_ardorment = null;
                this.m_connectorPresenter = null;
                this.m_labelPresenter = null;
                this.m_predefinedSymbol = null;

                this.m_symbolPresenter = null;
            }

            #endregion
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_elements
        /// </summary>
        public UIElementCollection m_elements;
        ResourceDictionary baseRD = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the VisibleSeries dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleSeriesProperty =
            DependencyProperty.Register("VisibleSeries", typeof(VisibleSeriesCollection), typeof(ChartAdornmentsPresenter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnVisibleSeriesPropertyChanged)));

        private PropertyChangedCallback pccaxes = new PropertyChangedCallback(OnAxesPropertyChanged);
        /// <summary>
        /// Identifies the Axes dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
            DependencyProperty.Register("Axes", typeof(ChartAxesCollection), typeof(ChartAdornmentsPresenter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxesPropertyChanged)));

        /// <summary>
        /// Identifies the Area dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(ChartArea), typeof(ChartAdornmentsPresenter), new UIPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the VisibleSeries. This is a dependency property.
        /// </summary>
        /// <value>The VisibleSeries.</value>
        public VisibleSeriesCollection VisibleSeries
        {
            get { return (VisibleSeriesCollection)GetValue(VisibleSeriesProperty); }
            set { SetValue(VisibleSeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Axes. This is a dependency property.
        /// </summary>
        /// <value>The Axes value.</value>
        public ChartAxesCollection Axes
        {
            get { return (ChartAxesCollection)GetValue(AxesProperty); }
            set { SetValue(AxesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Area. This is a dependency property.
        /// </summary>
        /// <value>The Area value.</value>
        public ChartArea Area
        {
            get { return (ChartArea)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return m_elements!=null?m_elements.Count:0;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornmentsPresenter"/> class.
        /// </summary>
        public ChartAdornmentsPresenter()
        {
            m_elements = new UIElementCollection(this, this);
            baseRD = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Wpf;component/ChartSeries/AdornmentSymbolTemplates.xaml", UriKind.RelativeOrAbsolute)
            };
        }
        #endregion

        #region Implementation
        private ChartAxesCollection axes;
        #region Static Methods
        /// <summary>
        /// Called when axes property changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ChartAdornmentsPresenter adornmentsPresenter = d as ChartAdornmentsPresenter;
                adornmentsPresenter.axes = e.NewValue as ChartAxesCollection;
                adornmentsPresenter.axes.CollectionChanged += new NotifyCollectionChangedEventHandler(adornmentsPresenter.OnAxesCollectionChanged);
                foreach (ChartAxis axis in adornmentsPresenter.axes)
                {
                    if (axis != null)
                        axis.Changed += new EventHandler(adornmentsPresenter.OnAxisChanged);
                }
            }
        }
        private VisibleSeriesCollection oldSeriesCollection;
        private VisibleSeriesCollection newSeriesCollection;
        /// <summary>
        /// Called when visible series property was changed.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibleSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAdornmentsPresenter adornmentsPresenter = d as ChartAdornmentsPresenter;
            adornmentsPresenter.oldSeriesCollection = e.OldValue as VisibleSeriesCollection;
            adornmentsPresenter.newSeriesCollection = e.NewValue as VisibleSeriesCollection;

            if (adornmentsPresenter.oldSeriesCollection != null)
            {
                adornmentsPresenter.oldSeriesCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(adornmentsPresenter.OnVisibleSeriesCollectionChanged);
                foreach (ChartSeries series in adornmentsPresenter.oldSeriesCollection)
                {
                    series.Adornments.CollectionChanged -= new NotifyCollectionChangedEventHandler(adornmentsPresenter.OnAdornmentsCollectionChanged);
                }

                adornmentsPresenter.ResetAdornments();
            }

            if (adornmentsPresenter.newSeriesCollection != null)
            {
                adornmentsPresenter.newSeriesCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(adornmentsPresenter.OnVisibleSeriesCollectionChanged);
                foreach (ChartSeries series in adornmentsPresenter.newSeriesCollection)
                {
                    if (series.Adornments != null)
                    {
                        series.Adornments.CollectionChanged += new NotifyCollectionChangedEventHandler(adornmentsPresenter.OnAdornmentsCollectionChanged);
                    }
                }

                adornmentsPresenter.ResetAdornments();
            }
        }
        #endregion

        #region Instance methods
        /// <summary>
        /// Called when [axes collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ChartAxis axis in e.OldItems)
                {
                    if (axis != null)
                    {
                        axis.Changed -= new EventHandler(OnAxisChanged);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (ChartAxis axis in e.NewItems)
                {
                    if (axis != null)
                    {
                        axis.Changed += new EventHandler(OnAxisChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Called when axis range was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisChanged(object sender, EventArgs e)
        {
            InvalidateArrange();
        }

        /// <summary>
        /// Called when visible series collection was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnVisibleSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                m_elements.Clear();
            }
            else
            {
                ResetAdornments();
                if (e.OldItems != null)
                {
                    foreach (object o in e.OldItems)
                    {
                        if ((o as ChartSeries).Adornments != null)
                        {
                            (o as ChartSeries).Adornments.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnAdornmentsCollectionChanged);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (object o in e.NewItems)
                    {
                        if ((o as ChartSeries).Adornments != null)
                        {
                            (o as ChartSeries).Adornments.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAdornmentsCollectionChanged);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when adornments collection was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnAdornmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ////Looking for series that cleared adornments.
                ChartSeries targetSeries = null;
                foreach (ChartSeries series in VisibleSeries)
                {
                    ////Looking for sender's parent.
                    if (series.Adornments == sender)
                    {
                        targetSeries = series;
                        break;
                    }
                }
                ////Making sure we've got reference to targer series.
                if (targetSeries != null)
                {
                    ////Removing elements that still present in wrapped items collection.
                    for (int i = 0; i < m_elements.Count; i++)
                    {
                        ////Verifying element's owner.
                        if ((m_elements[i] as ChartAdornmentContainer).Adornment.Series == targetSeries)
                        {
                            (m_elements[i] as ChartAdornmentContainer).Clear();
                            m_elements.RemoveAt(i--);
                        }
                    }
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (object o in e.OldItems)
                    {
                        this.RemoveAdornment(o as ChartAdornment);
                    }
                }

                if (e.NewItems != null)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.AddAdornment(e.NewItems[i] as ChartAdornment);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the adornments.
        /// </summary>
        private void ResetAdornments()
        {
            if(m_elements!=null)
            m_elements.Clear();
            if (VisibleSeries != null && VisibleSeries.Count > 0)
            {
                foreach (ChartSeries series in VisibleSeries)
                {
                    if (series.Adornments != null)
                    {
                        foreach (ChartAdornment adornment in series.Adornments)
                        {
                            if ((this.Area.SecondaryAxis.VisibleRange.Inside(adornment.DataPoint.Y) && this.Area.PrimaryAxis.VisibleRange.Inside(adornment.DataPoint.X))||Area.PrimaryAxis.ValueType==ChartValueType.Logarithmic||Area.SecondaryAxis.ValueType==ChartValueType.Logarithmic || (series.Type == ChartTypes.StackingLine100) || (series.Type == ChartTypes.StackingSpline100) || series.Type == ChartTypes.StackingArea100 || series.Type == ChartTypes.StackingSplineArea100 || (series.Type == ChartTypes.StackingColumn100) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut || series.Type == ChartTypes.Funnel || series.Type == ChartTypes.Pyramid))
                            {
                                m_elements.Add(new ChartAdornmentContainer(adornment,baseRD));
                                
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the adornment to UI children collection.
        /// </summary>   
        /// <param name="adornment">The adornment.</param>
        private void AddAdornment(ChartAdornment adornment)
        {
            //if ((this.Area.SecondaryAxis.VisibleRange.Inside(adornment.DataPoint.Y) && this.Area.PrimaryAxis.VisibleRange.Inside(adornment.DataPoint.X)) || adornment.Series.Type == ChartTypes.Pie || adornment.Series.Type == ChartTypes.Doughnut || adornment.Series.Type == ChartTypes.Funnel || adornment.Series.Type == ChartTypes.Pyramid )
            {
                ChartAdornmentContainer adornmentContatiner = new ChartAdornmentContainer(adornment,baseRD);
                ////m_elements.Insert(index, adornmentContatiner);
                m_elements.Add(adornmentContatiner);
            }
        }

        /// <summary>
        /// Removes the adornment from UI children collecion.
        /// </summary>
        /// <param name="adornment">The adornment.</param>
        private void RemoveAdornment(ChartAdornment adornment)
        {
            for (int i = 0; i < m_elements.Count; i++)
            {
                if ((m_elements[i] as ChartAdornmentContainer).Adornment == adornment)
                {
                    m_elements.RemoveAt(i);
                    break;
                }
            }
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
            return m_elements[index];
        }
        private double[] lastSeriesY = new double[0];
        private double[] lastSeriesX = new double[0];
        private ObservableCollection<ChartAdornmentContainer> m_IntersectAdornments = new ObservableCollection<ChartAdornmentContainer>();
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IChartTransformer transformer = null;
            int index = 2;
            Rect prevRect = Rect.Empty;
            ObservableCollection<Rect> adornRect = new ObservableCollection<Rect>();
            ObservableCollection<Rect> adornRect1 = new ObservableCollection<Rect>();
            
            foreach (UIElement element1 in m_elements)
            {
                ChartAdornmentContainer adornment = (ChartAdornmentContainer)element1;
                
                //Adornment Rect calculate for Stacking Series
                if (adornment.Adornment.Series.ShowSmartLabels && (adornment.Adornment.Series.Type == ChartTypes.StackingBar
                    || adornment.Adornment.Series.Type == ChartTypes.StackingColumn || adornment.Adornment.Series.Type == ChartTypes.StackingLine
                    || adornment.Adornment.Series.Type == ChartTypes.StackingSpline || adornment.Adornment.Series.Type == ChartTypes.StackingSplineArea
                    || adornment.Adornment.Series.Type == ChartTypes.StackingArea
                     || adornment.Adornment.Series.Type == ChartTypes.StackingBar100
                    || adornment.Adornment.Series.Type == ChartTypes.StackingColumn100 || adornment.Adornment.Series.Type == ChartTypes.StackingLine100 || adornment.Adornment.Series.Type == ChartTypes.StackingSpline100
                    || adornment.Adornment.Series.Type == ChartTypes.StackingSplineArea100 || adornment.Adornment.Series.Type == ChartTypes.StackingArea100))
                {
                    transformer = CreateTransformer(new Rect(finalSize), adornment.Adornment.Series);
                    adornment.Adornment.Update(transformer);
                    Rect rect = new Rect(adornment.DesiredSize);
                    rect.X = adornment.Adornment.X - adornment.SymbolOffset.X;
                    double offset = adornment.SymbolOffset.Y;
                    rect.Y = adornment.Adornment.Y - offset;
                    adornRect1.Add(rect);
                }
                if (adornment.Adornment.Series.ShowSmartLabels)
                {
                    Rect rect = new Rect(adornment.DesiredSize);
                    rect.X = adornment.Adornment.Series.Area.ValueToPoint(adornment.Adornment.Series,adornment.Adornment.Series.XAxis, adornment.Adornment.DataPoint.X);
                    rect.Y = adornment.Adornment.Series.Area.ValueToPoint(adornment.Adornment.Series,adornment.Adornment.Series.YAxis, adornment.Adornment.DataPoint.Y);
                    adornRect.Add(rect);
                }
            }
            foreach (UIElement element in m_elements)
            {
                ChartAdornmentContainer adornmentPresenter = (ChartAdornmentContainer)element;
                Rect adornmentRect = new Rect(adornmentPresenter.DesiredSize);
                transformer = CreateTransformer(new Rect(finalSize), adornmentPresenter.Adornment.Series);
                adornmentPresenter.Adornment.Update(transformer);

                adornmentRect.X = adornmentPresenter.Adornment.X - adornmentPresenter.SymbolOffset.X;
                double offset = adornmentPresenter.SymbolOffset.Y;
                ChartAdornmentInfo adornmentsInfo = adornmentPresenter.Adornment.Series.AdornmentsInfo;
                adornmentRect.Y = adornmentPresenter.Adornment.Y - offset;
                if (lastSeriesY.Length == 0)
                {
                    lastSeriesY = new double[m_elements.Count];
                }

                #region Top
                if (adornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                {
                    //This condition is included to avoid the cropping of adornment label when using stakingcolumn100
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Column || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.FastStackingColumn)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.Y = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).Y) - offset;
                        else
                            adornmentRect.X = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).X) - adornmentPresenter.SymbolOffset.X;
                    }
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar100)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                        {
                            adornmentRect.X = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).X) - adornmentPresenter.SymbolOffset.X;
                            adornmentRect.X = Math.Round(adornmentRect.X) <= 0 ? adornmentRect.X + adornmentPresenter.m_adornmentSize.Width : adornmentRect.X;
                        }
                        else
                            adornmentRect.Y = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).Y) - offset;
                    }
                    if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.Gantt || adornmentPresenter.Adornment.Series.Type == ChartTypes.Tornado))
                    {
                        double x = adornmentPresenter.Adornment.DataPoint.X;
                        double y = adornmentPresenter.Adornment.m_point.Y;
                        adornmentRect.X = transformer.TransformToVisible(x, y).X - adornmentPresenter.SymbolOffset.X;
                    }
                    if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.RangeColumn))
                    {
                        double x = adornmentPresenter.Adornment.DataPoint.X;
                        double y = adornmentPresenter.Adornment.m_point.Y;
                        adornmentRect.Y = transformer.TransformToVisible(x, y).Y - adornmentPresenter.SymbolOffset.Y;
                    }
                } 
                #endregion

                #region TopAndBottom
                if (adornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                {
                    double originy = adornmentPresenter.Adornment.Series.Area.SecondaryAxis.Origin;
                    double originx = adornmentPresenter.Adornment.Series.Area.PrimaryAxis.Origin;
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Column || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.FastStackingColumn)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.Y = ((transformer.TransformToVisible(originx, adornmentPresenter.Adornment.m_point.Y).Y - offset));
                        else
                            adornmentRect.X = ((transformer.TransformToVisible(originx, adornmentPresenter.Adornment.m_point.Y).X - adornmentPresenter.SymbolOffset.X));

                    }
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Bar)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.X = ((transformer.TransformToVisible(originx, adornmentPresenter.Adornment.m_point.Y).X - adornmentPresenter.SymbolOffset.X));
                        else
                            adornmentRect.Y = ((transformer.TransformToVisible(originx, adornmentPresenter.Adornment.m_point.Y).Y - offset));
                    }
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar100)
                    {
                        DoubleRange range = adornmentPresenter.Adornment.YDataMeasure;
                        double midPoint = range.Median;
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.X = (transformer.TransformToVisible(originx, midPoint).X) - adornmentPresenter.SymbolOffset.X;
                        else
                            adornmentRect.Y = (transformer.TransformToVisible(originx, midPoint).Y) - offset;
                    }
                    if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.Gantt || adornmentPresenter.Adornment.Series.Type == ChartTypes.Tornado))
                    {
                        double x = adornmentPresenter.Adornment.m_point.X;
                        double y = adornmentPresenter.Adornment.m_point.Y;
                        adornmentRect.X = transformer.TransformToVisible(x, y).X - adornmentPresenter.SymbolOffset.X;
                    }
                    if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.RangeColumn))
                    {
                        double x = adornmentPresenter.Adornment.m_point.X;
                        double y = adornmentPresenter.Adornment.m_point.Y;
                        adornmentRect.Y = transformer.TransformToVisible(x, y).Y - offset;
                    }
                } 
                #endregion

                #region Bottom
                if (adornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                {
                    //This property is to find the origin of the axis based on the origin the adornment position is calculated
                    double originy = adornmentPresenter.Adornment.Series.Area.SecondaryAxis.Origin;
                    double originx = adornmentPresenter.Adornment.Series.Area.PrimaryAxis.Origin;
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Column)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.Y = (transformer.TransformToVisible(0, originx).Y - offset);
                        else
                            adornmentRect.X = transformer.TransformToVisible(0, originx).X - adornmentPresenter.SymbolOffset.X;
                        //adornmentRect.Y = ((adornmentPresenter.Adornment.Series.Area.ValueToPoint(adornmentPresenter.Adornment.Series.YAxis, (adornmentPresenter.Adornment.DataPoint.Y + 20) / 2)) - offset);
                    }
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Bar)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.X = transformer.TransformToVisible(0, originx).X - adornmentPresenter.SymbolOffset.X;
                        else
                            adornmentRect.Y = (transformer.TransformToVisible(0, originx).Y - offset);
                    }

                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.FastStackingColumn)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.Y = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).Y) - offset;
                        else
                            adornmentRect.X = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).X) - adornmentPresenter.SymbolOffset.X;
                    }
                    if (adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar100)
                    {
                        if (!adornmentPresenter.Adornment.Series.IsRotated)
                            adornmentRect.X = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).X) - adornmentPresenter.SymbolOffset.X;
                        else
                            adornmentRect.Y = (transformer.TransformToVisible(0, adornmentPresenter.Adornment.m_point.Y).Y) - offset;
                    }
                    if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.RangeColumn))
                    {
                        double x = adornmentPresenter.Adornment.DataPoint.X;
                        double y = adornmentPresenter.Adornment.m_point.Y;
                        adornmentRect.Y = transformer.TransformToVisible(x, y).Y - adornmentPresenter.SymbolOffset.Y;
                    }
                } 
                #endregion

                if (adornmentPresenter.Adornment.Series.Type == ChartTypes.Pie || adornmentPresenter.Adornment.Series.Type == ChartTypes.Doughnut || adornmentPresenter.Adornment.Series.Type == ChartTypes.Funnel || adornmentPresenter.Adornment.Series.Type == ChartTypes.Pyramid)
                {
                    adornmentPresenter.ConnectorShift = 0;
                    Rect bounds = new Rect(new Point(0, 0), finalSize);
                    double top = bounds.Top - adornmentRect.Top;
                    double bottom = bounds.Bottom - adornmentRect.Bottom;
                    double right = bounds.Right - adornmentRect.Right;
                    double left = bounds.Left - adornmentRect.Left;

                    if (top > 0)
                    {
                        adornmentRect.Offset(0, top);
                        if ((adornmentPresenter.Adornment is ChartPieAdornment) && adornmentPresenter.Adornment.Series.AdornmentsInfo.SegmentShowLine)
                        {
                            adornmentPresenter.ConnectorShift = top;
                        }
                    }

                    if (bottom < 0)
                    {
                        adornmentRect.Offset(0, bottom);
                        if ((adornmentPresenter.Adornment is ChartPieAdornment) && adornmentPresenter.Adornment.Series.AdornmentsInfo.SegmentShowLine)
                        {
                            adornmentPresenter.ConnectorShift = -bottom;
                        }
                    }

                    if (left > 0)
                    {
                        adornmentRect.Offset(left, 0);
                        adornmentPresenter.ConnectorShift = left;
                    }

                    if (right < 0)
                    {
                        adornmentRect.Offset(right, 0);
                        adornmentPresenter.ConnectorShift = -right;
                    }
                }

                #region Pie Adornment Radial Alignment
                if ((adornmentPresenter.Adornment.Series.Type == ChartTypes.Pie || adornmentPresenter.Adornment.Series.Type == ChartTypes.Doughnut) && ChartPieAdornment.GetAdornmentSegmentsMode(adornmentPresenter.Adornment.Series) == AdornmentSegmentModes.Radial)
                {
                    double totalValue = 0d;
                    double ActualTotalValue = 0d;
                    for (int i = 0; i < adornmentPresenter.Adornment.Series.Data.Count; i++)
                    {
                        totalValue += Math.Abs(adornmentPresenter.Adornment.Series.Data[i].Y);
                        if (i <= adornmentPresenter.Adornment.Index)
                        {
                            ActualTotalValue = ActualTotalValue + Math.Abs(adornmentPresenter.Adornment.Series.Data[i].Y);
                        }
                    }

                    double startAngle = ((ActualTotalValue - adornmentPresenter.Adornment.DataPoint.Y) * 360) / totalValue;
                    double endAngle = (ActualTotalValue * 360) / totalValue;

                    double angle = startAngle + ((endAngle - startAngle) / 2);
                    adornmentPresenter.RenderTransform = new RotateTransform() { CenterX = adornmentPresenter.DesiredSize.Width / 2, CenterY = adornmentPresenter.DesiredSize.Height / 2 };

                    ChartPieAdornment pieAdornment = adornmentPresenter.Adornment as ChartPieAdornment;
                    double labelAngle = pieAdornment.LabelAngle;
                    
                    if (angle > 90 && angle <= 270)
                    {
                        adornmentPresenter.RenderTransform = new RotateTransform { Angle = ChartMath.ToDegree * labelAngle - 180 };
                    }
                    else
                    {
                        adornmentPresenter.RenderTransform = new RotateTransform { Angle = ChartMath.ToDegree * labelAngle };
                    }

                }
                #endregion

                if (adornmentPresenter.Adornment.Series.AdornmentsInfo.IsSegmentAlignment && (adornmentPresenter.Adornment.Series.Type == ChartTypes.Bar || adornmentPresenter.Adornment.Series.Type == ChartTypes.Gantt || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar100))
                {
                    adornmentPresenter.RenderTransform = new RotateTransform(90);
                }
                //Support the Stacking Chart with AdornmentIntersectAction 
                #region Stacking AdornmentIntersectAction
                if (adornmentPresenter.Adornment.Series.ShowSmartLabels && (adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar 
                    || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingLine 
                    || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingSpline || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingSplineArea
                    || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingArea
                     || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingBar100
                    || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingColumn100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingLine100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingSpline100
                    || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingSplineArea100 || adornmentPresenter.Adornment.Series.Type == ChartTypes.StackingArea100))
                {
                    bool isSet = false;
                    Rect presentrect = new Rect(adornmentPresenter.DesiredSize);
                    presentrect.X = adornmentPresenter.Adornment.X - adornmentPresenter.SymbolOffset.X;
                    presentrect.Y = adornmentPresenter.Adornment.Y - offset;
                    adornmentPresenter.customOffsetX = 0d;
                    adornmentPresenter.customOffsetY = 0d;
                    for (int i = 0; i < adornRect1.Count; i++)
                    {
                        if (isSet)
                        {
                            break;
                        }
                        if (i == m_elements.IndexOf(adornmentPresenter) && !isSet)
                        {
                            index = 2;
                            break;
                        }
                        else if (i != m_elements.IndexOf(adornmentPresenter))
                        {
                            if (presentrect.IntersectsWith(adornRect1[i]))
                            {
                                switch (adornmentPresenter.Adornment.Series.AdornmentIntersectAction)
                                {
                                    case AdornmentIntersectActions.Hide:
                                        adornmentPresenter.Visibility = System.Windows.Visibility.Hidden;
                                        if (!this.m_IntersectAdornments.Contains(adornmentPresenter))
                                        {
                                            this.m_IntersectAdornments.Add(adornmentPresenter);
                                        }
                                        break;
                                    case AdornmentIntersectActions.AdjustAroundPoints:
                                        adornmentPresenter.Visibility = System.Windows.Visibility.Visible;
                                        if (!this.m_IntersectAdornments.Contains(adornmentPresenter))
                                        {
                                            this.m_IntersectAdornments.Add(adornmentPresenter);
                                        }
                                        if (adornmentPresenter.Adornment.Series.YAxis.IsInversed)
                                        {
                                            if (adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar && adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar100 )
                                            {
                                                adornmentRect.Y = adornmentRect.Y + (index * adornmentPresenter.m_adornmentSize.Height);
                                                adornmentPresenter.customOffsetY = -((index * adornmentPresenter.m_adornmentSize.Height) - (adornmentPresenter.m_adornmentSize.Height / 2));
                                            }
                                            else
                                            {
                                                adornmentRect.X = adornmentRect.X - (index * adornmentPresenter.m_adornmentSize.Height);
                                                if (Math.Abs(prevRect.X - adornmentRect.X) <= adornmentPresenter.m_adornmentSize.Height)
                                                    adornmentPresenter.customOffsetX = 0d;
                                                else
                                                    adornmentPresenter.customOffsetX = ((index * adornmentPresenter.m_adornmentSize.Height) + (adornmentPresenter.m_adornmentSize.Height / 2));
                                            }
                                        }
                                        else
                                        {
                                            if (adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar && adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar100)
                                            {
                                                adornmentRect.Y = adornmentRect.Y - (index * adornmentPresenter.m_adornmentSize.Height);
                                                adornmentPresenter.customOffsetY = (index * adornmentPresenter.m_adornmentSize.Height) + (adornmentPresenter.m_adornmentSize.Height / 2);
                                            }
                                            else
                                            {
                                                adornmentRect.X = adornmentRect.X + (index * adornmentPresenter.m_adornmentSize.Height);
                                                if (Math.Abs(prevRect.X - adornmentRect.X) <= adornmentPresenter.m_adornmentSize.Height)
                                                    adornmentPresenter.customOffsetX = 0d;
                                                else
                                                    adornmentPresenter.customOffsetX = -((index * adornmentPresenter.m_adornmentSize.Height) - (adornmentPresenter.m_adornmentSize.Height / 2));
                                            }
                                        }
                                        if (adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar && adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar100)
                                        {
                                            adornmentPresenter.customOffsetX = adornmentPresenter.m_adornmentSize.Width / 2;
                                        }
                                        else
                                        {
                                            adornmentPresenter.customOffsetY = adornmentPresenter.m_adornmentSize.Width / 2;
                                        }

                                        if (adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar && adornmentPresenter.Adornment.Series.Type != ChartTypes.StackingBar100)
                                        {
                                            adornRect1[m_elements.IndexOf(adornmentPresenter)] = new Rect(new Point(adornRect1[m_elements.IndexOf(adornmentPresenter)].X, adornRect1[m_elements.IndexOf(adornmentPresenter)].Y - (index * adornmentPresenter.m_adornmentSize.Height)), adornRect1[m_elements.IndexOf(adornmentPresenter)].Size);
                                        }
                                        else
                                            adornRect1[m_elements.IndexOf(adornmentPresenter)] = new Rect(new Point(adornRect1[m_elements.IndexOf(adornmentPresenter)].X, adornRect1[m_elements.IndexOf(adornmentPresenter)].Y + (index * adornmentPresenter.m_adornmentSize.Height)), adornRect1[m_elements.IndexOf(adornmentPresenter)].Size);
                                        prevRect = adornmentRect;
                                        index = index + 1;
                                        isSet = true;
                                        break;
                                }
}
                            else
                            {
                                if (!presentrect.IntersectsWith(adornRect[i]) && this.m_IntersectAdornments.Contains(adornmentPresenter) && adornmentPresenter.Adornment.Series.AdornmentIntersectAction == AdornmentIntersectActions.Hide)
                                {
                                    adornmentPresenter.Visibility = Visibility.Visible;
                                    this.m_IntersectAdornments.Remove(adornmentPresenter);
                                }
                            }
                        }
                    }
                    prevRect = adornmentRect;
                    adornmentPresenter.InvalidateVisual();
                }
                #endregion

                if (adornmentPresenter.Adornment.Series.ShowSmartLabels && (adornmentPresenter.Adornment.Series.Type == ChartTypes.Area ||adornmentPresenter.Adornment.Series.Type == ChartTypes.Column|| adornmentPresenter.Adornment.Series.Type == ChartTypes.Spline||
                    adornmentPresenter.Adornment.Series.Type == ChartTypes.SplineArea || adornmentPresenter.Adornment.Series.Type == ChartTypes.Scatter || adornmentPresenter.Adornment.Series.Type == ChartTypes.Line || adornmentPresenter.Adornment.Series.Type == ChartTypes.Bubble || adornmentPresenter.Adornment.Series.Type == ChartTypes.Bar || adornmentPresenter.Adornment.Series.Type == ChartTypes.Gantt))
                {
                    bool isSet = false;
                    Rect presentrect = new Rect(adornmentPresenter.DesiredSize);
                    presentrect.X = adornmentPresenter.Adornment.Series.Area.ValueToPoint(adornmentPresenter.Adornment.Series,adornmentPresenter.Adornment.Series.XAxis, adornmentPresenter.Adornment.DataPoint.X);
                    presentrect.Y = adornmentPresenter.Adornment.Series.Area.ValueToPoint(adornmentPresenter.Adornment.Series,adornmentPresenter.Adornment.Series.YAxis, adornmentPresenter.Adornment.DataPoint.Y);
                    adornmentPresenter.customOffsetX = 0d;
                    adornmentPresenter.customOffsetY = 0d;                                       
                    for(int i=0; i<adornRect.Count; i++)
                    {
                        if (isSet)
                        {
                            break;
                        }
                        if (i == m_elements.IndexOf(adornmentPresenter) && !isSet)
                        {
                            index = 2;
                            break;
                        }
                        else if (i != m_elements.IndexOf(adornmentPresenter))
                        {
                            if (presentrect.IntersectsWith(adornRect[i]))
                            {
                                switch (adornmentPresenter.Adornment.Series.AdornmentIntersectAction)
                                {
                                    case AdornmentIntersectActions.Hide:
                                        adornmentPresenter.Visibility = System.Windows.Visibility.Hidden;
                                        if (!this.m_IntersectAdornments.Contains(adornmentPresenter))
                                        {
                                            this.m_IntersectAdornments.Add(adornmentPresenter);
                                        }
                                        break;
                                    case AdornmentIntersectActions.AdjustAroundPoints:
                                        adornmentPresenter.Visibility = System.Windows.Visibility.Visible;
                                        if (!this.m_IntersectAdornments.Contains(adornmentPresenter))
                                        {
                                            this.m_IntersectAdornments.Add(adornmentPresenter);
                                        }
                                        if (adornmentPresenter.Adornment.Series.YAxis.IsInversed)
                                        {
                                            if (adornmentPresenter.Adornment.Series.Type != ChartTypes.Bar && adornmentPresenter.Adornment.Series.Type != ChartTypes.Gantt)
                                            {
                                                adornmentRect.Y = adornmentRect.Y + (index * adornmentPresenter.m_adornmentSize.Height);
                                                adornmentPresenter.customOffsetY = -((index * adornmentPresenter.m_adornmentSize.Height) - (adornmentPresenter.m_adornmentSize.Height / 2));
                                            }
                                            else
                                            {
                                                adornmentRect.X = adornmentRect.X - (index * adornmentPresenter.m_adornmentSize.Height);
                                                if (Math.Abs(prevRect.X - adornmentRect.X) <= adornmentPresenter.m_adornmentSize.Height)
                                                    adornmentPresenter.customOffsetX = 0d;
                                                else
                                                    adornmentPresenter.customOffsetX = ((index * adornmentPresenter.m_adornmentSize.Height) + (adornmentPresenter.m_adornmentSize.Height / 2));
                                            }
                                        }
                                        else
                                        {
                                            if (adornmentPresenter.Adornment.Series.Type != ChartTypes.Bar && adornmentPresenter.Adornment.Series.Type != ChartTypes.Gantt)
                                            {
                                                adornmentRect.Y = adornmentRect.Y - (index * adornmentPresenter.m_adornmentSize.Height);
                                                adornmentPresenter.customOffsetY = (index * adornmentPresenter.m_adornmentSize.Height) + (adornmentPresenter.m_adornmentSize.Height / 2);
                                            }
                                            else
                                            {
                                                adornmentRect.X = adornmentRect.X + (index * adornmentPresenter.m_adornmentSize.Height);
                                                if (Math.Abs(prevRect.X - adornmentRect.X) <= adornmentPresenter.m_adornmentSize.Height)
                                                    adornmentPresenter.customOffsetX = 0d;
                                                else
                                                    adornmentPresenter.customOffsetX = -((index * adornmentPresenter.m_adornmentSize.Height) - (adornmentPresenter.m_adornmentSize.Height/2));
                                            }
                                        }
                                        if (adornmentPresenter.Adornment.Series.Type != ChartTypes.Bar && adornmentPresenter.Adornment.Series.Type != ChartTypes.Gantt)
                                        {
                                            adornmentPresenter.customOffsetX = adornmentPresenter.m_adornmentSize.Width / 2;
                                        }
                                        else
                                        {
                                            adornmentPresenter.customOffsetY = adornmentPresenter.m_adornmentSize.Width / 2;
                                        }

                                        if (adornmentPresenter.Adornment.Series.Type != ChartTypes.Bar && adornmentPresenter.Adornment.Series.Type != ChartTypes.Gantt)
                                        {
                                            adornRect[m_elements.IndexOf(adornmentPresenter)] = new Rect(new Point(adornRect[m_elements.IndexOf(adornmentPresenter)].X, adornRect[m_elements.IndexOf(adornmentPresenter)].Y - (index * adornmentPresenter.m_adornmentSize.Height)), adornRect[m_elements.IndexOf(adornmentPresenter)].Size);
                                        }
                                        else
                                            adornRect[m_elements.IndexOf(adornmentPresenter)] = new Rect(new Point(adornRect[m_elements.IndexOf(adornmentPresenter)].X, adornRect[m_elements.IndexOf(adornmentPresenter)].Y + (index * adornmentPresenter.m_adornmentSize.Height)), adornRect[m_elements.IndexOf(adornmentPresenter)].Size);
                                        prevRect = adornmentRect;
                                        index = index + 1;
                                        isSet = true;
                                        break;
                                }
                            }
                            else
                            {
                                if (!presentrect.IntersectsWith(adornRect[i]) && this.m_IntersectAdornments.Contains(adornmentPresenter) && adornmentPresenter.Adornment.Series.AdornmentIntersectAction == AdornmentIntersectActions.Hide)
                                {
                                    adornmentPresenter.Visibility = Visibility.Visible;
                                    this.m_IntersectAdornments.Remove(adornmentPresenter);
                                }
                            }
                            
                        }
                    }
                    prevRect = adornmentRect;
                    adornmentPresenter.InvalidateVisual();
                }
                adornmentPresenter.Arrange(adornmentRect);
            }

            List<int> animationenabledSeriesIndex = (from ser in Area.Series where ser.EnableAnimation == true && ser.AdornmentsInfo != null && ser.AdornmentsInfo.Visible == true select Area.Series.IndexOf(ser)).ToList<int>();
            if (animationenabledSeriesIndex.Count > 0)
            {
                foreach (int i in animationenabledSeriesIndex)
                {
                    if (Area.Series[i].Animation != null && Area.Series[i].Animation.AdornmentStoryboard.Children.Count == 0)
                    {
                        Area.Series[i].AdornmentPresenter = (from ele in m_elements.OfType<UIElement>() where ele is ChartAdornmentContainer && ((ChartAdornmentContainer)ele).Adornment.Series.Equals(Area.Series[i]) == true select ele).ToList<UIElement>();
                        Area.Series[i].Animation.AnimateAdornment(Area.Series[i].AdornmentPresenter);
                    }
                }
            }

            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in m_elements)
            {
                element.Measure(new Size(Double.PositiveInfinity,Double.PositiveInfinity));
            }

            return new Size();
        }

        /// <summary>
        /// Creates the transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="series">The series.</param>
        /// <returns>The Transformer</returns>
        private IChartTransformer CreateTransformer(Rect viewport, ChartSeries series)
        {
            if (Area != null && VisibleSeries.Count > 0)
            {
                return ChartTransform.CreateTransformer(this.Area.AreaType, viewport, series);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.Area != null && this.Area.Series != null)
            {
                foreach (ChartSeries item in this.Area.Series)
                {
                    DisposeSeries(item);
                }
                this.Area.Series.Clear();
            }
            if (this.oldSeriesCollection != null)
            {
                this.oldSeriesCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnVisibleSeriesCollectionChanged);
            }
            if (this.newSeriesCollection != null)
            {
                this.newSeriesCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnVisibleSeriesCollectionChanged);
            }
            if (this.axes != null)
            {
                this.axes.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnAxesCollectionChanged);
                foreach (ChartAxis axis in this.axes)
                {
                    if (axis != null)
                    {
                        axis.Changed -= new EventHandler(this.OnAxisChanged);                        
                    }
                }
                this.axes.Clear();
                this.axes = null;
            }
            if (this.Axes != null)
            this.Axes.Clear();
            this.Axes= null;
            if (m_elements != null)
            {
                for (int i = 0; i < m_elements.Count; i++)
                {
                    ChartAdornmentContainer cont = m_elements[i] as ChartAdornmentContainer;
                    cont.Adornment.Dispose();
                }
                m_elements.Clear();

                m_elements = null;
            }
            if (baseRD != null)
            {
                this.baseRD.MergedDictionaries.Clear();
                this.baseRD.Clear();
                this.baseRD = null;
            }
        }

        private void DisposeSeries(ChartSeries series)
        {
            if (series == null)
                return;
            foreach (ChartSegment item in series.Segments)
            {
                item.seriesCorrespondingPoints = null;
            }

            series.Data = null;
            series.DataSource = null;
            if (series.Adornments != null)
            {
                series.Adornments.Clear();
            }

            if (series.Segments != null)
            {
                series.Segments.Clear();
                //series.Segments = null;
            }
        }
        #endregion
    }
}
