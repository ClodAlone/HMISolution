#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the panel which contains all the ChartAdornment elements.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of adornment labels, marker symbols and connector lines to connect the labels.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartAdornmentContainer : Panel
    {
        #region fields

        private Point m_symbolOffset = new Point();

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
        private SymbolControl m_predefinedSymbol = new SymbolControl();

        private ChartAdornment adornment;

        internal ChartAdornment Adornment
        {
            get
            {
                return adornment;
            }
            set
            {
                if (value != adornment)
                {
                    adornment = value;
                    UpdateContainers();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the label vertical alignment.
        /// </summary>
        /// <value>The label vertical alignment.</value>
        [ClassReference(IsReviewed = false)]
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelVerticalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
          DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(ChartAdornmentContainer), new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the label horizontal alignment.
        /// </summary>
        /// <value>The label horizontal alignment.</value>
        [ClassReference(IsReviewed = false)]
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelHorizontalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
          DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartAdornmentContainer), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or Sets the chart symbol
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSymbol Symbol
        {
            get { return (ChartSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(ChartSymbol), typeof(ChartAdornmentContainer), new PropertyMetadata(ChartSymbol.Custom, new PropertyChangedCallback(OnAdornmentsInfoChanged)));

        /// <summary>
        /// Gets the symbol offset.
        /// </summary>
        /// <value>The symbol offset.</value>
        [ClassReference(IsReviewed = false)]
        public Point SymbolOffset
        {
            get { return this.m_symbolOffset; }
        }

        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAdornmentContainer).SetSymbol((d as ChartAdornmentContainer).Symbol.ToString());
        }

        #endregion

        #region ctor
        /// <summary>
        /// Constructor
        /// </summary>
        public ChartAdornmentContainer()
        {

        }

        /// <summary>
        /// Called when instance created for ChartAdornmentContainer
        /// </summary>
        /// <param name="adornment"></param>
        public ChartAdornmentContainer(ChartAdornment adornment)
        {
            Adornment = adornment;
        }

        #endregion

        #region methods

        internal void UpdateContainers()
        {
            if (this.Adornment != null)
            {
                var adornmentInfo = Adornment.Series.adornmentInfo;



                if (adornmentInfo.SymbolTemplate != null
                    && !this.Children.Contains(m_symbolPresenter))
                    this.Children.Add(m_symbolPresenter);

                if (!this.Children.Contains(m_predefinedSymbol))
                    this.Children.Add(m_predefinedSymbol);

                this.LabelVerticalAlignment = adornmentInfo.VerticalAlignment;
                this.LabelHorizontalAlignment = adornmentInfo.HorizontalAlignment;

                this.Symbol = adornmentInfo.Symbol;

                this.SetSymbol(this.Symbol.ToString());
                this.SetContentBinding(Adornment);
            }
        }

        private void SetContentBinding(ChartAdornment adornment)
        {
            ChartAdornmentInfoBase adornmentInfo = adornment.Series.adornmentInfo;

            if (adornmentInfo.ShowMarker)
            {
                this.SetBinding(ChartAdornmentContainer.SymbolProperty
                         , CreateAdormentBinding("Symbol", adornmentInfo));

                this.m_predefinedSymbol.SetBinding(Control.HeightProperty
                    , CreateAdormentBinding("SymbolHeight", adornmentInfo));

                this.m_predefinedSymbol.SetBinding(Control.WidthProperty
                    , CreateAdormentBinding("SymbolWidth", adornmentInfo));

                this.m_predefinedSymbol.SetBinding(Control.BackgroundProperty
                   , CreateAdormentBinding("SymbolInterior", adornmentInfo));

                this.m_predefinedSymbol.SetBinding(Control.BorderBrushProperty
                   , CreateAdormentBinding("SymbolStroke", adornmentInfo));

                if (adornmentInfo.SymbolTemplate != null)
                    this.m_symbolPresenter.SetBinding(ContentPresenter.ContentTemplateProperty
                         , CreateAdormentBinding("SymbolTemplate", adornmentInfo));

                this.m_symbolPresenter.SetBinding(ContentPresenter.ContentProperty
                    , CreateAdormentBinding("", adornmentInfo));
            }

        }

        private Binding CreateAdormentBinding(string path, object source)
        {
            Binding bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(path);
            bindingProvider.Source = source;
            bindingProvider.Mode = BindingMode.OneWay;
            return bindingProvider;
        }

        private void SetSymbol(string symbol)
        {
            if (symbol != "Custom")
            {
                m_predefinedSymbol.DataContext = this;
                this.m_predefinedSymbol.Template = ChartDictionaries.GenericSymbolDictionary[symbol] as ControlTemplate;
            }
            else
            {
                this.m_symbolPresenter.Content = null;
                this.m_symbolPresenter.ContentTemplate = null;
            }
        }
        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect arrangeRect = new Rect(new Point(), this.DesiredSize);
            Rect arrangePieRect = new Rect(new Point(), this.DesiredSize);
            switch (this.LabelHorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Center:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                        this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Center;
                    }

                    break;

                case HorizontalAlignment.Left:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Right;
                        this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Right;
                    }

                    break;

                case HorizontalAlignment.Right:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Left;
                        this.m_predefinedSymbol.HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    break;
            }

            switch (this.LabelVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Top;
                        this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Top;
                    }

                    break;
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Center:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Center;
                        this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Center;
                    }

                    break;
                case VerticalAlignment.Top:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Bottom;
                        this.m_predefinedSymbol.VerticalAlignment = VerticalAlignment.Bottom;
                    }

                    break;
            }

            this.m_symbolPresenter.Arrange(arrangeRect);
            this.m_predefinedSymbol.Arrange(arrangeRect);

            return this.DesiredSize;
        }
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Adornment == null)
                return ChartLayoutUtils.CheckSize(availableSize);
            m_predefinedSymbol.Measure(availableSize);
            m_symbolPresenter.Measure(availableSize);

            Size resultSize = new Size();
            Size lblSz = Size.Empty;
            Size sblSz;
            if (Adornment.Series.adornmentInfo.Symbol == ChartSymbol.Custom)
            {
                sblSz = m_symbolPresenter.DesiredSize;
            }
            else
            {
                sblSz = m_predefinedSymbol.DesiredSize;
            }

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
                    //if (!isAccumulatedChart)
                    //{
                    //    resultSize.Width = lblSz.Width + sblSz.Width;
                    //    m_symbolOffset.X = 0.5 * sblSz.Width + lblSz.Width;
                    //}
                    //else
                    //{
                    resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                    m_symbolOffset.X = 0.5 * resultSize.Width;
                    //}
                    break;

                case HorizontalAlignment.Right:
                    //if (!isAccumulatedChart)
                    //{
                    //    resultSize.Width = lblSz.Width + sblSz.Width;
                    //    m_symbolOffset.X = 0.5 * sblSz.Width;
                    //}
                    //else
                    //{
                    resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                    m_symbolOffset.X = 0.5 * resultSize.Width;
                    //}
                    break;
            }

            switch (LabelVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    //if (!isAccumulatedChart)
                    //{
                    //    resultSize.Height = lblSz.Height + sblSz.Height;
                    //    m_symbolOffset.Y = 0.5 * sblSz.Height;
                    //}
                    //else
                    //{
                    resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                    m_symbolOffset.Y = 0.5 * resultSize.Height;
                    //}
                    break;
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Center:
                    {
                        resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                        m_symbolOffset.Y = 0.5 * resultSize.Height;
                    }

                    break;
                case VerticalAlignment.Top:
                    //if (!isAccumulatedChart)
                    //{
                    //    resultSize.Height = lblSz.Height + sblSz.Height;
                    //    m_symbolOffset.Y = 0.5 * sblSz.Height + lblSz.Height;
                    //}
                    //else
                    //{
                    resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                    m_symbolOffset.Y = 0.5 * resultSize.Height;
                    //}
                    break;
            }

            return resultSize;
        }

        #endregion
    }

    /// <summary>
    /// A control that represents symbol in chart adornments
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SymbolControl : Control
    {
        /// <summary>
        /// Gets or Sets the stroke
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolStroke.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(SymbolControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


    }


}
