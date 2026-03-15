//------------------------------------------------------------------------------
// <copyright file="BoardViewer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for BoardViewer.xaml
    /// </summary>
    public partial class BoardViewer : UserControl
    {
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                "Stretch",
                typeof(Stretch),
                typeof(BoardViewer),
                new UIPropertyMetadata(Stretch.Uniform));

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(
                "Board",
                typeof(Board),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty XBrushProperty =
            DependencyProperty.Register(
                "BoardXBrush",
                typeof(Brush),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OBrushProperty =
            DependencyProperty.Register(
                "BoardOBrush",
                typeof(Brush),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdBrushProperty =
            DependencyProperty.Register(
                "BoardIdBrush",
                typeof(Brush),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdTypefaceProperty =
            DependencyProperty.Register(
                "BoardIdTypeface",
                typeof(Typeface),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(new Typeface(new FontFamily("SegoeUI"), FontStyles.Normal, FontWeights.Light, FontStretches.Normal), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IdFontSizeProperty =
            DependencyProperty.Register(
                "BoardIdFontSize",
                typeof(double),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(36.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(
                "BoardHighlightBrush",
                typeof(Brush),
                typeof(BoardViewer),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));

        private const int DesignSize = 300;

        private readonly BoardSquareViewer[][] squareViewers;

        public BoardViewer()
        {
            InitializeComponent();

            squareViewers = new BoardSquareViewer[Board.Size][];
            for (int row = 0; row < Board.Size; ++row)
            {
                squareViewers[row] = new BoardSquareViewer[Board.Size];

                for (int column = 0; column < Board.Size; ++column)
                {
                    var viewer = new BoardSquareViewer();
                    Grid.SetRow(viewer, 2 * row);
                    Grid.SetColumn(viewer, 2 * column);
                    squareViewers[row][column] = viewer;
                }
            }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public Board Board
        {
            get
            {
                return (Board)GetValue(BoardProperty);
            }

            set
            {
                SetValue(BoardProperty, value);

                if (null != value)
                {
                    for (int row = 0; row < Board.Size; ++row)
                    {
                        for (int column = 0; column < Board.Size; ++column)
                        {
                            squareViewers[row][column].Square = value.GetAt(row, column);
                        }
                    }
                }
            }
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
        /// Gets and sets brush used to draw board square IDs.
        /// </summary>
        public Brush IdBrush
        {
            get { return (Brush)GetValue(IdBrushProperty); }
            set { SetValue(IdBrushProperty, value); }
        }

        /// <summary>
        /// Gets and sets typeface used to render text for board square IDs.
        /// </summary>
        public Typeface IdTypeface
        {
            get { return (Typeface)GetValue(IdTypefaceProperty); }
            set { SetValue(IdTypefaceProperty, value); }
        }

        /// <summary>
        /// Gets and sets font size used to render text for board square IDs.
        /// </summary>
        public double IdFontSize
        {
            get { return (double)GetValue(IdFontSizeProperty); }
            set { SetValue(IdFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets and sets brush used to draw board square highlight foreground.
        /// </summary>
        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        /// Get the square viewer at specified row and column of board.
        /// </summary>
        /// <param name="row">
        /// Row of square to be retrieved.
        /// </param>
        /// <param name="column">
        /// Column of square to be retrieved.
        /// </param>
        /// <returns>
        /// Square corresponding to specified row and column.
        /// </returns>
        public BoardSquareViewer GetAt(int row, int column)
        {
            if ((row < 0) || (row >= Board.Size) || (column < 0) || (column >= Board.Size))
            {
                throw new ArgumentException();
            }

            return squareViewers[row][column];
        }

        /// <summary>
        /// Determines actual desired size for BoardViewer, given specified stretch behavior.
        /// </summary>
        /// <param name="arrangeBounds">
        /// The area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size to be used by BoardViewer control.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var newSize = arrangeBounds; // accept specified bounds by default

            switch (Stretch)
            {
                case Stretch.None:
                    {
                        // If no stretch was specified, we use the design size no matter how much area is really available.
                        newSize = new Size(DesignSize, DesignSize);
                        break;
                    }

                case Stretch.Uniform:
                    {
                        // We use the smaller of with and height as the desired size for both.
                        double sideLength = Math.Min(arrangeBounds.Width, arrangeBounds.Height);
                        newSize = new Size(sideLength, sideLength);
                        break;
                    }

                case Stretch.UniformToFill:
                    {
                        // We use the bigger of with and height as the desired size for both.
                        double sideLength = Math.Max(arrangeBounds.Width, arrangeBounds.Height);
                        newSize = new Size(sideLength, sideLength);
                        break;
                    }
            }

            return base.ArrangeOverride(newSize);
        }

        /// <summary>
        /// Adds control children and binds board properties to each of their properties that shoul vary together.
        /// </summary>
        /// <param name="sender">
        /// Object sending the event.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < Board.Size; ++row)
            {
                for (int column = 0; column < Board.Size; ++column)
                {
                    BoardGrid.Children.Add(squareViewers[row][column]);

                    var symbolXBrushBinding = new Binding("BoardXBrush") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.XBrushProperty, symbolXBrushBinding);

                    var symbolOBrushBinding = new Binding("BoardOBrush") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.OBrushProperty, symbolOBrushBinding);

                    var idBrushBinding = new Binding("BoardIdBrush") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.IdBrushProperty, idBrushBinding);

                    var idTypefaceBinding = new Binding("BoardIdTypeface") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.IdTypefaceProperty, idTypefaceBinding);

                    var idFontSizeBinding = new Binding("BoardIdFontSize") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.IdFontSizeProperty, idFontSizeBinding);

                    var idHighlightBrush = new Binding("BoardHighlightBrush") { Source = this };
                    squareViewers[row][column].SetBinding(BoardSquareViewer.HighlightBrushProperty, idHighlightBrush);
                }
            }
        }
    }
}
