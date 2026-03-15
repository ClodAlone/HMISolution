#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

#region File using derectives

using System.Drawing;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// This will render the horizontal ruler for the diagram control.
    /// </summary>
	public class HorizontalRuler
		: Ruler
	{
		#region Class initialize/finalize methods
		/// <summary>
		/// 
		/// </summary>
		public HorizontalRuler()
			: base()
		{}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		public HorizontalRuler( HorizontalRuler src )
			: base( src )
		{}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public HorizontalRuler( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{}
		#endregion
		
		#region Class overrides
        /// <summary>
        /// Renders the ruler
        /// </summary>
        /// <param name="gfx">Graphics element gfx</param>
        /// <param name="view">View class view</param>
		public override void Draw( Graphics gfx, View view )
		{
			RectangleF rectModel = MeasureUnitsConverter.ToPixels( view.Model.Bounds, view.Model.MeasurementUnits );
			RectangleF bounds =
				new RectangleF( rectModel.X + this.Location.X - ( view.Origin.X * view.Magnification / 100f ), Location.Y,
				                rectModel.Width * view.Magnification / 100, Size.Height );

			// fill background
			using( Brush brBack = new SolidBrush( this.BackgroundColor ) )
			{
				gfx.FillRectangle( brBack, Location.X, Location.Y, Size.Width, Size.Height );
			}

			m_markupRenderer.RenderMarkup(gfx, bounds, view);
			
			// Draw current tool work area
			if( this.HighlightAreaWidth > 0 )
			{
				using( Brush brushHighlight = new SolidBrush( this.HighlightColor ) )
				{
					System.Drawing.Rectangle rectHighlight = System.Drawing.Rectangle.Empty;
					rectHighlight.Location = new Point( this.HighlightAreaStart, this.Location.Y );
					rectHighlight.Size = new Size( this.HighlightAreaWidth, this.Size.Height );
        					
					gfx.FillRectangle( brushHighlight, rectHighlight );
				}
			}
			
			//draw markup
			using( Pen pen = new Pen( this.MarkerColor, 0f ) )
			{
				gfx.DrawLine( pen, this.MarkerPosition, 0, this.MarkerPosition, this.Size.Height );
			}
			// outline border
			using( Pen pen = new Pen( Color.Black, 0f ) )
			{
				gfx.DrawRectangle( pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1 );
			}
		}
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public override object Clone()
		{
			return new HorizontalRuler( this );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return DPN.HorizontalRuler;
		}
		#endregion
	}
}