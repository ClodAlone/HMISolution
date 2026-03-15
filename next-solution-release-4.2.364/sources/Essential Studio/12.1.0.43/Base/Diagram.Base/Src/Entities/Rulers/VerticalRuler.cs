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
    /// This will render the vertical ruler for the diagram control.
	/// </summary>
	public class VerticalRuler
		: Ruler
	{
		#region Class initialize/finalize methods
		/// <summary>
		/// Creates instance
		/// </summary>
		public VerticalRuler()
			: base()
		{}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		public VerticalRuler( VerticalRuler src )
			: base( src )
		{}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public VerticalRuler( SerializationInfo info, StreamingContext context )
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
				new RectangleF( Location.X, rectModel.Y + this.Location.Y - ( view.Origin.Y * view.Magnification / 100f ),
				                Size.Width, rectModel.Height * view.Magnification / 100 );

			using( Brush brBack = new SolidBrush( this.BackgroundColor ) )
			{
				gfx.FillRectangle( brBack, Location.X, Location.Y, Size.Width, Size.Height );
			}

			m_markupRenderer.RenderMarkup( gfx, bounds, view);

			// Draw current tool work area
			if( this.HighlightAreaWidth > 0 )
			{
				using( Brush brushHighlight = new SolidBrush( this.HighlightColor ) )
				{
					System.Drawing.Rectangle rectHighlight = System.Drawing.Rectangle.Empty;
					rectHighlight.Location = new Point( this.Location.X, this.HighlightAreaStart );
					rectHighlight.Size = new Size( this.Size.Width, this.HighlightAreaWidth );
					gfx.FillRectangle( brushHighlight, rectHighlight );
				}
			}
			
			// Draw cursor marker
			using( Pen pen = new Pen( this.MarkerColor, 1 ) )
			{
				gfx.DrawLine( pen, 0, this.MarkerPosition, this.Size.Width, this.MarkerPosition );
			}
			// outline border
			using( Pen pen = new Pen( Color.Black, 0f ) )
			{
				gfx.DrawRectangle( pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height -1 );
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
			return new VerticalRuler( this );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return DPN.VerticalRuler;
		}
		#endregion
	}
}
