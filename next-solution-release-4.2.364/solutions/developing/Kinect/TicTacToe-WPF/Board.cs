// -----------------------------------------------------------------------
// <copyright file="Board.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a Tic Tac Toe board.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Number of squares in each side of TicTacToe board.
        /// </summary>
        private const int BoardSize = 3;

        /// <summary>
        /// Board squares, stored in a row major array.
        /// </summary>
        private readonly BoardSquare[][] boardSquares;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class.
        /// </summary>
        public Board()
        {
            boardSquares = new BoardSquare[BoardSize][];
            for (int row = 0; row < BoardSize; ++row)
            {
                boardSquares[row] = new BoardSquare[BoardSize];

                for (int column = 0; column < BoardSize; ++column)
                {
                    boardSquares[row][column] = new BoardSquare(row, column);
                }
            }
        }

        /// <summary>
        /// Number of squares in each side of TicTacToe board.
        /// </summary>
        public static int Size
        {
            get
            {
                return BoardSize;
            }
        }

        /// <summary>
        /// Get the square at specified row and column of board.
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
        public BoardSquare GetAt(int row, int column)
        {
            if ((row < 0) || (row >= BoardSize))
            {
                throw new ArgumentException(Properties.Resources.InvalidBoardIndex, "row");
            }

            if ((column < 0) || (column >= BoardSize))
            {
                throw new ArgumentException(Properties.Resources.InvalidBoardIndex, "column");
            }

            return this.boardSquares[row][column];
        }

        /// <summary>
        /// Restore all board squares to their initial state.
        /// </summary>
        public void Clear()
        {
            for (int row = 0; row < BoardSize; ++row)
            {
                for (int column = 0; column < BoardSize; ++column)
                {
                    this.boardSquares[row][column].Clear();
                }
            }
        }

        /// <summary>
        /// Determines if all board squares are occupied.
        /// </summary>
        /// <returns>
        /// true if all board squares are occupied, false if any is empty.
        /// </returns>
        public bool IsFull()
        {
            for (int row = 0; row < BoardSize; ++row)
            {
                for (int column = 0; column < BoardSize; ++column)
                {
                    if (PlayerSymbol.None == this.boardSquares[row][column].Symbol)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Assign default ids to board squares. Ids correspond to sequential numbers,
        /// starting from square at {0, 0} and ending with square at {Size-1, Size-1}.
        /// </summary>
        public void PopulateDefaultIds()
        {
            for (int row = 0; row < BoardSize; ++row)
            {
                for (int column = 0; column < BoardSize; ++column)
                {
                    int squareId = (BoardSize * row) + (column + 1);
                    boardSquares[row][column].Id = squareId.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Find board square with specified ID.
        /// </summary>
        /// <param name="id">
        /// ID corresponding to square to be found.
        /// </param>
        /// <returns>
        /// Square that has specified Id, or null if no square was found with that ID.
        /// </returns>
        public BoardSquare Find(string id)
        {
            if (null == id)
            {
                return null;
            }

            BoardSquare match = null;

            foreach (var row in boardSquares)
            {
                foreach (var square in row)
                {
                    if (id.Equals(square.Id))
                    {
                        match = square;
                        break;
                    }
                }
            }

            return match;
        }
    }
}
