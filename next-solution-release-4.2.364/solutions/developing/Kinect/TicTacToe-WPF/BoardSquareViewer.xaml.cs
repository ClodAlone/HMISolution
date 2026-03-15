//------------------------------------------------------------------------------
// <copyright file="BoardSquareViewer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for BoardSquareViewer.xaml
    /// </summary>
    public partial class BoardSquareViewer : UserControl
    {
        public static readonly DependencyProperty SquareProperty =
            DependencyProperty.Register(
                "Square",
                typeof(BoardSquare),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty XBrushProperty =
            DependencyProperty.Register(
                "SquareXBrush",
                typeof(Brush),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OBrushProperty =
            DependencyProperty.Register(
                "SquareOBrush",
                typeof(Brush),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdBrushProperty =
            DependencyProperty.Register(
                "SquareIdBrush",
                typeof(Brush),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightGray), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdTypefaceProperty =
            DependencyProperty.Register(
                "SquareIdTypefaceBrush",
                typeof(Typeface),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(new Typeface(new FontFamily("SegoeUI"), FontStyles.Normal, FontWeights.Light, FontStretches.Normal), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdFontSizeProperty =
            DependencyProperty.Register(
                "SquareIdFontSize",
                typeof(double),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(36.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(
                "SquareHighlightBrush",
                typeof(Brush),
                typeof(BoardSquareViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Percentage of board square dimensions that are used as margin.
        /// </summary>
        private const double SquareMargin = 0.2;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSquareViewer"/> class.
        /// </summary>
        public BoardSquareViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets and sets the BuardSquare associated with this BoardSquareViewer.
        /// </summary>
        public BoardSquare Square
        {
            get { return (BoardSquare)GetValue(SquareProperty); }
            set { SetValue(SquareProperty, value); }
        }

        /// <summary>
        /// Gets and sets brush used to draw X symbol.
        /// </summary>
        public Brush XBrush
        {
            get { return (Brush)GetValue(XBrushProperty); }
            set { SetValue(XBrushProperty, value); }
        }

        /// <summary>
        /// Gets and sets brush used to draw O symbol.
        /// </summary>
        public Brush OBrush
        {
            get { return (Brush)GetValue(OBrushProperty); }
            set { SetValue(OBrushProperty, value); }
        }

        /// <summary>
        /// Gets and sets brush used to draw square ID.
        /// </summary>
        public Brush IdBrush
        {
            get { return (Brush)GetValue(IdBrushProperty); }
            set { SetValue(IdBrushProperty, value); }
        }

        /// <summary>
        /// Gets and sets typeface used to render square ID text.
        /// </summary>
        public Typeface IdTypeface
        {
            get { return (Typeface)GetValue(IdTypefaceProperty); }
            set { SetValue(IdTypefaceProperty, value); }
        }

        /// <summary>
        /// Gets and sets font size used to render square ID text.
        /// </summary>
        public double IdFontSize
        {
            get { return (double)GetValue(IdFontSizeProperty); }
            set { SetValue(IdFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets and sets brush used to draw square highlight foreground.
        /// </summary>
        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        /// Perform a blink animation on the square ID.
        /// </summary>
        public void BlinkId()
        {
            // Relative font size at peak, compared to the current font size.
            const double PeakRelativeFontSize = 2.0;

            var animation = new DoubleAnimationUsingKeyFrames { Duration = TimeSpan.FromSeconds(0.7) };
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(IdFontSize * PeakRelativeFontSize, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(IdFontSize, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.7))));

            this.BeginAnimation(IdFontSizeProperty, animation);
        }

        /// <summary>
        /// Renders contents of associated board square.
        /// </summary>
        /// <param name="drawingContext">
        /// The DrawingContext used to draw board square contents.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Don't render if we don't have a square
            if (null == Square)
            {
                return;
            }

            double horizontalMargin = SquareMargin * this.ActualWidth;
            double verticalMargin = SquareMargin * this.ActualHeight;

            var boundsRect = new Rect(
                    horizontalMargin,
                    verticalMargin,
                    this.ActualWidth - (2 * horizontalMargin),
                    this.ActualHeight - (2 * verticalMargin));

            switch (Square.Symbol)
            {
                case PlayerSymbol.XSymbol:
                case PlayerSymbol.OSymbol:
                    if (!Square.IsHighlighted)
                    {
                        Square.Symbol.Draw(new Pen(Square.Symbol == PlayerSymbol.XSymbol ? XBrush : OBrush, this.ActualWidth * Square.Symbol.GetRecommendedThickness()), boundsRect, drawingContext);
                    }
                    else if (null != this.HighlightBrush)
                    {
                        drawingContext.DrawRectangle(
                            Square.Symbol == PlayerSymbol.XSymbol ? XBrush : OBrush,
                            null,
                            new Rect(0, 0, this.ActualWidth, this.ActualHeight));
                        Square.Symbol.Draw(
                            new Pen(this.HighlightBrush, this.ActualWidth * Square.Symbol.GetRecommendedThickness()),
                            boundsRect,
                            drawingContext);
                    }

                    break;

                case PlayerSymbol.None:
                    if (Square.Id != null)
                    {
                        this.DrawId(Square.Id, boundsRect, drawingContext);
                    }

                    break;
            }
        }

        /// <summary>
        /// Renders the specified ID text.
        /// </summary>
        /// <param name="text">
        /// Text to be rendered.
        /// </param>
        /// <param name="rect">
        /// Drawing bounds for text.
        /// </param>
        /// <param name="dc">
        /// DeviceContext used to draw text.
        /// </param>
        private void DrawId(string text, Rect rect, DrawingContext dc)
        {
            var formattedText = new FormattedText(
                text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, IdTypeface, IdFontSize, IdBrush);
            double textWidth = formattedText.WidthIncludingTrailingWhitespace;
            double textHeight = formattedText.Height;
            double left = rect.Left + ((rect.Width - textWidth) / 2);
            double top = rect.Top + ((rect.Height - textHeight) / 2);
            dc.DrawText(formattedText, new Point(left, top));
        }
    }
}
