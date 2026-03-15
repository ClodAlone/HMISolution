// -----------------------------------------------------------------------
// <copyright file="GameLogic.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Status returned after an attempt to place a symbol on the game board.
    /// </summary>
    public enum PlacingStatus
    {
        /// <summary>
        /// Placement did not succeed because square was already occupied by a different symbol.
        /// </summary>
        SquareOccupied,

        /// <summary>
        /// Placement did not succeed because it was attempted during the turn of a different player.
        /// </summary>
        OutOfTurn,

        /// <summary>
        /// Placement was valid. Board was updated as appropriate.
        /// </summary>
        Valid,

        /// <summary>
        /// Placement was valid and resulted in a draw.
        /// </summary>
        Draw,

        /// <summary>
        /// Placement was valid and resulted in a victory.
        /// </summary>
        Victory,

        /// <summary>
        /// Placement was ignored because game already has ended.
        /// </summary>
        GameEnded
    }

    /// <summary>
    /// Tic-Tac-Toe game logic.
    /// </summary>
    public class GameLogic
    {
        /// <summary>
        /// Collection sorted in the order in which players are expected to move, in alternating turns.
        /// </summary>
        private static readonly ReadOnlyCollection<PlayerSymbol> SymbolOrderField = new ReadOnlyCollection<PlayerSymbol>(new List<PlayerSymbol> { PlayerSymbol.XSymbol, PlayerSymbol.OSymbol });
        
        /// <summary>
        /// Distinct directions in which a straight line of the same length as the board can result in a victory.
        /// </summary>
        private static readonly int[][] VictoryDirections = new[]
        {
            new[] { 1, 0 },
            new[] { 0, 1 },
            new[] { 1, 1 },
            new[] { 1, -1 }
        };

        /// <summary>
        /// Board managed by this GameLogic object.
        /// </summary>
        private readonly Board board = new Board();

        /// <summary>
        /// Symbol's whose turn it currently is.
        /// </summary>
        private PlayerSymbol currentSymbol;

        /// <summary>
        /// true if game has ended already, false otherwise.
        /// </summary>
        private bool hasEnded;

        /// <summary>
        ///  Collection sorted in the order in which players are expected to move, in alternating turns.
        /// </summary>
        public static ReadOnlyCollection<PlayerSymbol> SymbolOrder
        {
            get
            {
                return SymbolOrderField;
            }
        }

        /// <summary>
        /// Symbol's whose turn it currently is.
        /// </summary>
        public PlayerSymbol CurrentSymbol
        {
            get
            {
                return this.currentSymbol;
            }
        }

        /// <summary>
        /// Clears previous game state and begins a new Tic-Tac-Toe game.
        /// </summary>
        /// <returns>
        /// Tic tac toa board holding game state.
        /// </returns>
        public Board StartGame()
        {
            this.board.Clear();
            this.board.PopulateDefaultIds();

            this.currentSymbol = SymbolOrder[0];
            this.hasEnded = false;

            return this.board;
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
        public BoardSquare FindSquare(string id)
        {
            return board.Find(id);
        }

        /// <summary>
        /// Attempt to place specified symbol in specified board square.
        /// </summary>
        /// <param name="symbol">
        /// Symbol to be placed.
        /// </param>
        /// <param name="square">
        /// Square where placement will be attempted.
        /// </param>
        /// <returns>
        /// PlacementStatus indicating whether the move attempted was valid or invalid.
        /// </returns>
        public PlacingStatus PlaceSymbol(PlayerSymbol symbol, BoardSquare square)
        {
            if (null == square)
            {
                throw new ArgumentException(
                    Properties.Resources.InvalidPlacingSquare, "square");
            }

            // If game has already ended, let player know
            if (this.hasEnded)
            {
                return PlacingStatus.GameEnded;
            }

            // If square is already occupied, let player know
            if (PlayerSymbol.None != square.Symbol)
            {
                return PlacingStatus.SquareOccupied;
            }
            
            // If it's currently not the turn of the player that spoke, let them know
            if (currentSymbol != symbol)
            {
                return PlacingStatus.OutOfTurn;
            }
            
            // Move was valid, so place symbol in square
            square.Symbol = symbol;

            // Check if player has won
            IEnumerable<BoardSquare> victorySquares;
            if (IsPlayerVictory(symbol, square, out victorySquares))
            {
                foreach (BoardSquare victorySquare in victorySquares)
                {
                    victorySquare.IsHighlighted = true;
                }

                this.hasEnded = true;
                return PlacingStatus.Victory;
            }

            if (this.board.IsFull())
            {
                this.hasEnded = true;
                return PlacingStatus.Draw;
            }

            // If player did not win, it's the other player's turn
            this.currentSymbol = this.currentSymbol.Opponent();

            return PlacingStatus.Valid;
        }

        /// <summary>
        /// Determine if the placement of the specified symbol on the specified square resulted in a victory.
        /// </summary>
        /// <param name="symbol">
        /// Symbol that was just recently placed.
        /// </param>
        /// <param name="square">
        /// Square where symbol was placed
        /// </param>
        /// <param name="victorySquares">
        /// In case of victory, this out parameter will hold the set of squares that participated in this victory.
        /// </param>
        /// <returns>
        /// true if placement was determined to result in victory. false otherwise.
        /// </returns>
        private bool IsPlayerVictory(PlayerSymbol symbol, BoardSquare square, out IEnumerable<BoardSquare> victorySquares)
        {
            victorySquares = null;

            if (symbol != square.Symbol)
            {
                return false;
            }

            var victorySet = new HashSet<BoardSquare>();

            foreach (var direction in VictoryDirections)
            {
                int numSquaresMatching = 0;

                // Find the edge of the board, in current direction from square
                int beginRow = square.Row;
                int beginColumn = square.Column;
                while ((beginRow - direction[0] >= 0) && (beginColumn - direction[1] >= 0) &&
                    (beginRow - direction[0] < Board.Size) && (beginColumn - direction[1] < Board.Size))
                {
                    beginRow -= direction[0];
                    beginColumn -= direction[1];
                }

                // go to the other edge of the board, looking for matching squares
                for (int row = beginRow, column = beginColumn;
                    (row >= 0) && (column >= 0) && (row < Board.Size) && (column < Board.Size);
                    row += direction[0], column += direction[1])
                {
                    if (symbol == board.GetAt(row, column).Symbol)
                    {
                        ++numSquaresMatching;
                    }
                }

                // A full board length of matching squares is required for victory
                if (numSquaresMatching < Board.Size)
                {
                    continue;
                }

                // Add the squares as victory squares, but keep iterating because there might be victory squares in other directions
                for (int row = beginRow, column = beginColumn;
                    (row >= 0) && (column >= 0) && (row < Board.Size) && (column < Board.Size);
                    row += direction[0], column += direction[1])
                {
                    victorySet.Add(board.GetAt(row, column));
                }
            }

            // If victory set is non-empty, then there was victory in at least one direction
            if (0 == victorySet.Count)
            {
                return false;
            }

            victorySquares = victorySet;
            return true;
        }
    }
}
