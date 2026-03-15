// -----------------------------------------------------------------------
// <copyright file="PlayerSymbol.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.TicTacToe
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents a symbol corresponding to a Tic-Tac-Toe player. Can be X, O or no occupant.
    /// </summary>
    public enum PlayerSymbol
    {
        /// <summary>
        /// Corresponds to an 'X' placed on Tic-Tac-Toe board.
        /// </summary>
        XSymbol,

        /// <summary>
        /// Corresponds to an 'O' placed on Tic-Tac-Toe board.
        /// </summary>
        OSymbol,

        /// <summary>
        /// Represents the absence of a symbol.
        /// </summary>
        None
    }

    /// <summary>
    /// Extensions to PlayerSymbol enumeration, used to keep functionality together.
    /// </summary>
    public static class PlayerSymbolExtensions
    {
        /// <summary>
        /// Recommended percentage of rendered symbol dimensions that are used as X line thickness.
        /// </summary>
        private const double RecommendedXThickness = 0.2;

        /// <summary>
        ///  Recommended percentage of rendered symbol dimensions that are used as O line thickness.
        /// </summary>
        private const double RecommendedOThickness = 0.15;

        /// <summary>
        /// Returns the opponent corresponding to the specified symbol.
        /// </summary>
        /// <param name="symbol">
        /// Symbol of interest.
        /// </param>
        /// <returns>
        /// The opponent for specified symbol.
        /// </returns>
        public static PlayerSymbol Opponent(this PlayerSymbol symbol)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    return PlayerSymbol.OSymbol;

                case PlayerSymbol.OSymbol:
                    return PlayerSymbol.XSymbol;
            }

            return PlayerSymbol.None;
        }

        /// <summary>
        /// The recommended relative rendered thickness for specified symbol.
        /// </summary>
        /// <param name="symbol">
        /// Symbol of interest.
        /// </param>
        /// <returns>
        /// A number in [0.0, 1.0] interval, where 0.0 corresponds to no thickness
        /// and 1.0 corresponds to a thickness equal to the rendered size of symbol.
        /// </returns>
        public static double GetRecommendedThickness(this PlayerSymbol symbol)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    return RecommendedXThickness;

                case PlayerSymbol.OSymbol:
                    return RecommendedOThickness;
            }

            return 0.0;
        }

        /// <summary>
        /// Get symbol-appropriate display text similar to "Player X, say a number".
        /// </summary>
        /// <param name="symbol">
        /// Symbol of interest.
        /// </param>
        /// <returns>
        /// A display-ready string corresponding to the specified symbol. May be null.
        /// </returns>
        public static string GetSayNumberText(this PlayerSymbol symbol)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    return Properties.Resources.XSayNumber;

                case PlayerSymbol.OSymbol:
                    return Properties.Resources.OSayNumber;
            }

            return null;
        }

        /// <summary>
        /// Get symbol-appropriate display text similar to "It's "X's" turn".
        /// </summary>
        /// <param name="symbol">
        /// Symbol of interest.
        /// </param>
        /// <returns>
        /// A display-ready string corresponding to the specified symbol. May be null.
        /// </returns>
        public static string GetTurnWarningText(this PlayerSymbol symbol)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    return Properties.Resources.XTurn;

                case PlayerSymbol.OSymbol:
                    return Properties.Resources.OTurn;
            }

            return null;
        }

        /// <summary>
        /// Get symbol-appropriate display text similar to "X Won!".
        /// </summary>
        /// <param name="symbol">
        /// Symbol of interest.
        /// </param>
        /// <returns>
        /// A display-ready string corresponding to the specified symbol. May be null.
        /// </returns>
        public static string GetWinAnnouncementText(this PlayerSymbol symbol)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    return Properties.Resources.XWon;

                case PlayerSymbol.OSymbol:
                    return Properties.Resources.OWon;
            }

            return null;
        }

        /// <summary>
        /// Draw specified symbol in specified DrawingContext.
        /// </summary>
        /// <param name="symbol">
        /// Symbol to draw.
        /// </param>
        /// <param name="pen">
        /// Pen used to draw symbol.
        /// </param>
        /// <param name="rect">
        /// Bounding rectangle where symbol will be drawn.
        /// </param>
        /// <param name="dc">
        /// DrawingContext used to draw symbol.
        /// </param>
        public static void Draw(this PlayerSymbol symbol, Pen pen, Rect rect, DrawingContext dc)
        {
            switch (symbol)
            {
                case PlayerSymbol.XSymbol:
                    DrawX(pen, rect, dc);
                    break;

                case PlayerSymbol.OSymbol:
                    DrawO(pen, rect, dc);
                    break;
            }
        }

        /// <summary>
        /// Draw X symbol in specified DrawingContext.
        /// </summary>
        /// <param name="pen">
        /// Pen used to draw symbol.
        /// </param>
        /// <param name="rect">
        /// Bounding rectangle where X will be drawn.
        /// </param>
        /// <param name="dc">
        /// DrawingContext used to draw X.
        /// </param>
        private static void DrawX(Pen pen, Rect rect, DrawingContext dc)
        {
            dc.DrawLine(pen, rect.TopLeft, rect.BottomRight);
            dc.DrawLine(pen, rect.TopRight, rect.BottomLeft);
        }

        /// <summary>
        /// Draw O symbol in specified DrawingContext.
        /// </summary>
        /// <param name="pen">
        /// Pen used to draw O.
        /// </param>
        /// <param name="rect">
        /// Bounding rectangle where O will be drawn.
        /// </param>
        /// <param name="dc">
        /// DrawingContext used to draw O.
        /// </param>
        private static void DrawO(Pen pen, Rect rect, DrawingContext dc)
        {
            double radiusX = rect.Width / 2;
            double radiusY = rect.Height / 2;
            dc.DrawEllipse(null, pen, new Point(rect.Left + radiusX, rect.Top + radiusY), radiusX, radiusY);
        }
    }
}
