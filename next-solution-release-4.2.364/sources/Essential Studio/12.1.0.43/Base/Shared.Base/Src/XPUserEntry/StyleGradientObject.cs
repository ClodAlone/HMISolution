#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	[ Serializable
	, TypeConverter( typeof( StyleGradientObjectConvertor ) ) ]
	public class StyleGradientObject
	{
		#region Class constants
		#endregion

		#region Class members

		/// <summary>
		///	Start gradient color.
		/// </summary>
		private Color m_clrStart;

		/// <summary>
		///	End gradient color.
		/// </summary>
		private Color m_clrEnd;

		/// <summary>
		///	Gradient style.
		/// </summary>
		private LinearGradientMode m_gradient;

		#endregion

		#region Class properties

		/// <summary>
		///	Get or set start gradient color.
		/// </summary>
		[ Description( "Get or set start gradient color." ) ]
		public Color StartColor
		{
			get
			{
				return m_clrStart;
			}
			set
			{
				if( value != m_clrStart )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrStart, value );
					m_clrStart = value;
					OnStartColorChanged( args );
				}
			}
		}
		

		/// <summary>
		///	Get or set end gradient color.
		/// </summary>
		[ Description( "Get or set end gradient color." ) ]
		public Color EndColor
		{
			get
			{
				return m_clrEnd;
			}
			set
			{
				if( value != m_clrEnd )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrEnd, value );
					m_clrEnd = value;
					OnEndColorChanged( args );
				}
			}
		}


		/// <summary>
		///	Get or set gradient style.
		/// </summary>
		[ Description( "Get or set gradient style." ) ]
		public LinearGradientMode GradientMode
		{
			get
			{
				return m_gradient;
			}
			set
			{
				if( value != m_gradient )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_gradient, value );
					m_gradient = value;
					OnGradientModeChanged( args );
				}
			}
		}


		#endregion

		#region Class events

		/// <summary>
		/// Occurs when this class changed.
		/// </summary>
		public event EventHandler OnChanged;

		/// <summary>
		///	Occurs when	start color changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler StartColorChanged;

		/// <summary>
		///	Occurs when	end color changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler EndColorChanged;

		/// <summary>
		///	Occurs when	gradient mode changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler GradientModeChanged;
		#endregion

		#region Class initialize/finalize methods
		public StyleGradientObject()
		{
			// do nothing
		}

		public StyleGradientObject( Color start, Color end, LinearGradientMode mode )
			: this()
		{
			m_clrStart = start;
			m_clrEnd = end;
			m_gradient = mode;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Draw control background.
		/// </summary>
		/// <param name="g">Graphics for paint.</param>
		/// <param name="bounds">Rectangle it sketch.</param>
		public void FillRectangle( Graphics g, Rectangle bounds )
		{
			Brush brush = CreateBrush( bounds );

			g.FillRectangle( brush, bounds );

			brush.Dispose();
		}
		/// <summary>
		/// Draw control background.
		/// </summary>
		/// <param name="g">Graphics for paint.</param>
		/// <param name="bounds">Rectangle it sketch.</param>
		public void FillRectangle( Graphics g, GraphicsPath bounds )
		{
			Region rg = new Region( bounds );
			RectangleF rcInner = bounds.GetBounds();
			Region rgOld = g.Clip;
			g.Clip = rg;
			Brush brush = CreateBrush( rcInner );
			g.FillRectangle( brush, rcInner );
			brush.Dispose();

			g.Clip = rgOld;
			rg.Dispose();
		}

		
		/// <summary>
		/// Create gradient brush.
		/// </summary>
		/// <param name="bounds">Rectangle it sketch.</param>
		/// <returns>Gradient brush.</returns>
		private Brush CreateBrush( RectangleF bounds )
		{
			Brush brush = null;
	
			if( this.EndColor == this.StartColor )
			{
				brush = new SolidBrush( this.StartColor );
			}
			else
			{
				brush = new LinearGradientBrush( bounds, this.StartColor, this.EndColor, this.GradientMode );
			}

			return brush;
		}
		#endregion

		#region Class event raisers

		protected void RaiseOnChanged( ValueChangedEventArgs args )
		{
			if( OnChanged != null )
			{
				OnChanged( this, args );
			}
		}
		protected void RaiseStartColorChanged( ValueChangedEventArgs args )
		{
			if( StartColorChanged != null )
			{
				StartColorChanged( this, args );
			}

			RaiseOnChanged( args );
		}

		protected void RaiseEndColorChanged( ValueChangedEventArgs args )
		{
			if( EndColorChanged != null )
			{
				EndColorChanged( this, args );
			}

			RaiseOnChanged( args );
		}

		protected void RaiseGradientModeChanged( ValueChangedEventArgs args )
		{
			if( GradientModeChanged != null )
			{
				GradientModeChanged( this, args );
			}

			RaiseOnChanged( args );
		}

		#endregion
  
		#region Class overrides

		///	<override/>
		public override string ToString()
		{
			return string.Format( "Start: {0} End: {1} Mode: {2}", 
				this.StartColor, this.EndColor, this.GradientMode );
		}

		/// <summary>
		///	Occurs when	start color changed.
		/// </summary>
		protected virtual void OnStartColorChanged( ValueChangedEventArgs args )
		{
			RaiseStartColorChanged( args );
		}


		/// <summary>
		///	Occurs when	end color changed.
		/// </summary>
		protected virtual void OnEndColorChanged( ValueChangedEventArgs args )
		{
			RaiseEndColorChanged( args );
		}


		/// <summary>
		///	Occurs when	gradient mode changed.
		/// </summary>
		protected virtual void OnGradientModeChanged( ValueChangedEventArgs args )
		{
			RaiseGradientModeChanged( args );
		}
		#endregion

		#region Class utility methods
		#endregion
	}

	
	public class StyleGradientObjectConvertor : ExpandableObjectConverter
	{
		#region TypeConverter class overrides

		///	<override/>
		public override bool CanConvertTo( ITypeDescriptorContext context, Type destType )
		{
			if( destType == typeof( InstanceDescriptor ) )
			{
				return true;
			}

			return base.CanConvertTo( context, destType );
		}

		///	<override/>
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture,
			object value, Type destType )
		{
			if( destType == typeof( InstanceDescriptor ) )
			{
				Type[] types = new Type[]{ typeof( Color ),
																	 typeof( Color ),
																	 typeof( LinearGradientMode ) };

				ConstructorInfo ci = typeof( StyleGradientObject ).GetConstructor( types );

				StyleGradientObject gradient = (( StyleGradientObject )value);

				return new InstanceDescriptor( ci,
					new object[]{ gradient.StartColor, gradient.EndColor, gradient.GradientMode },
					true );
			}

			return base.ConvertTo( context, culture, value, destType );
		}
		#endregion
	}
}