#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Implements the data/model part of a card caption cell.
    /// </summary>
    public class GridCardCaptionCellModel : GridPushButtonCellModel
    {
        private GridCardView card = null;
        /// <summary>
        /// Initializes a new GridCardCaptionCellModel object and stores a reference to the 
        /// Syncfusion.Windows.Forms.Grid.GridModel this cell belongs to.
        /// </summary>
        /// <param name="grid">The Syncfusion.Windows.Forms.Grid.GridModel for this cell model.</param>
        /// <param name="card">The Syncfusion.Windows.Forms.GridCardView for this cell model.</param>
        public GridCardCaptionCellModel(GridModel grid, GridCardView card)
            :base(grid)
        {
            this.card = card;
        }
        
        /// <summary>
        ///  Initializes a new GridCardCaptionCellModel from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info.</param>
        protected GridCardCaptionCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        /// <summary>
        /// Creates a renderer for this cell model.
        /// </summary>
        /// <param name="control"> The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridCardCaptionCellRenderer(control, this, this.card);
        }

    }

    /// <summary>
    /// Implements the render part of a card caption cell.
    /// </summary>
    public class GridCardCaptionCellRenderer : GridPushButtonCellRenderer
    {
        private GridCardView card = null;

        /// <summary>
        /// Initializes a new GridCardCaptionCellRenderer object for the given GridControlBase and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        /// <param name="card">GridCardView</param> 
        public GridCardCaptionCellRenderer(GridControlBase grid, GridCellModelBase cellModel, GridCardView card)
            : base(grid, cellModel)
        {
            this.card = card;
        }

        /// <summary>
        /// Is triggered when the background for cell is drawn
        /// </summary>
        /// <param name="button">GridCellButton</param>
        /// <param name="g">Graphics</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="buttonState">ButtonState</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDrawCellButtonBackground(GridCellButton button, Graphics g, Rectangle rect, System.Windows.Forms.ButtonState buttonState, GridStyleInfo style)
        {
            base.OnDrawCellButtonBackground(button, g, rect, buttonState, style);
            if (buttonState == System.Windows.Forms.ButtonState.Flat
                && !(card.VisualStyle == CardVisualStyles.None || card.VisualStyle == CardVisualStyles.System || card.VisualStyle == CardVisualStyles.Metro))
            {
                FillButton(card.VisualStyle, g, rect);
            }
            if (card.IsActiveCard(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex) && this.card.HighlightActiveCard)
            {
                button.SetPushed(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex, rect, true);
                button.DrawButton(g, rect, System.Windows.Forms.ButtonState.Pushed, style);
            }
        }

        /// <summary>
        /// Fills the caption cell with theme colors.
        /// </summary>
        /// <param name="theme">visual theme.</param>
        /// <param name="g">Graphics</param>
        /// <param name="rect">The cell bound.</param>
        private void FillButton(CardVisualStyles theme, Graphics g, Rectangle rect)
        {
            Color topColor = SystemColors.Control;
            Color bottomColor = SystemColors.Control;
            switch (theme)
            {
                case CardVisualStyles.Office2007Black:
                    topColor = Off2007Colors.TopFirstBlack;
                    bottomColor = Off2007Colors.BottomLastBlack;
                    break;
                case CardVisualStyles.Office2007Blue:
                    topColor = Off2007Colors.TopFirstBlue;
                    bottomColor = Off2007Colors.BottomLastBlue;
                    break;
                case CardVisualStyles.Office2007Silver:
                    topColor = Off2007Colors.TopFirstSilver;
                    bottomColor = Off2007Colors.BottomLastSilver;
                    break;
                case CardVisualStyles.Office2010Black:
                    topColor = Off2010Colors.TopFirstBlack;
                    bottomColor = Color.DimGray;
                    break;
                case CardVisualStyles.Office2010Blue:
                    topColor = Off2010Colors.TopFirstBlue;
                    bottomColor = Off2010Colors.BottomLastBlue;
                    break;
                case CardVisualStyles.Office2010Silver:
                    topColor = Off2010Colors.TopFirstSilver;
                    bottomColor = Off2010Colors.BottomLastSilver;
                    break;
            }
            g.FillRectangle(new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical), rect);
        }
    }
}
