// -----------------------------------------------------------------------
// <copyright file="BoardSquare.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System.Windows;

    /// <summary>
    /// Represents a square in a Tic Tac Toe board.
    /// </summary>
    public class BoardSquare : Freezable
    {
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                "BoardSquareSymbol",
                typeof(PlayerSymbol),
                typeof(BoardSquare),
                new PropertyMetadata(PlayerSymbol.None, null));

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register(
                "BoardSquareIsHighlighted",
                typeof(bool),
                typeof(BoardSquare),
                new PropertyMetadata(false, null));

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(
                "BoardSquareId",
                typeof(string),
                typeof(BoardSquare),
                new PropertyMetadata(null, null));

        /// <summary>
        /// Square's row within overall board.
        /// </summary>
        private readonly int row;

        /// <summary>
        /// Square's column within overall board.
        /// </summary>
        private readonly int column;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardSquare"/> class.
        /// </summary>
        /// <param name="row">
        /// Board row associated with this square.
        /// </param>
        /// <param name="column">
        /// Board column associated with this square.
        /// </param>
        public BoardSquare(int row, int column)
        {
            Clear();

            this.row = row;
            this.column = column;
        }

        /// <summary>
        /// PlayerSymbol associated with square. Player.None means that no symbol is
        /// curerntly associated with square.
        /// </summary>
        public PlayerSymbol Symbol
        {
            get { return (PlayerSymbol)this.GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// true if square should be highlighted, false otherwise.
        /// </summary>
        public bool IsHighlighted
        {
            get { return (bool)this.GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        /// <summary>
        /// Application-specified unique ID associated with square.
        /// </summary>
        public string Id
        {
            get { return (string)this.GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// Square's row within overall board.
        /// </summary>
        public int Row
        {
            get
            {
                return this.row;
            }
        }

        /// <summary>
        /// Square's column within overall board.
        /// </summary>
        public int Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// Resets square properties to their initial state.
        /// </summary>
        public void Clear()
        {
            Symbol = PlayerSymbol.None;
            IsHighlighted = false;
            Id = null;
        }

        #region Freezable

        /// <summary>
        /// Creates a new instance of the Freezable derived class. 
        /// </summary>
        /// <returns>
        /// A new instance of BoardSquare class.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BoardSquare(this.row, this.column);
        }

        /// <summary>
        /// Called as part of Freezable.Freeze() to determine whether this class can be frozen.
        /// We can freeze, but only if the KinectSensor is null.
        /// </summary>
        /// <returns>
        /// True if a Freeze is legal or has occurred, false otherwise.
        /// </returns>
        protected override bool FreezeCore(bool isChecking)
        {
            return false;
        }
        #endregion
    }
}
