#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
	/// <summary>
	/// Presents panel that replaces in ItemsPanelTemplate of Command Menu. 
	/// </summary>
	public class CustomWrapPanel : WrapPanel
	{
		/// <summary>
		/// Gets the number of child elements for the control.
		/// </summary>
		/// <returns>An Int32 value that represents the number of child elements.</returns>
		protected override int VisualChildrenCount
		{
			get
			{
				int count = base.VisualChildrenCount;
				return ( count >= 1 ) ? ( count + 1 ) : count;
			}
		}
		private FrameworkElement m_Button = null;
		private FrameworkElement MButton
		{
			get
			{
				if( null == m_Button )
				{
					StackPanel sp = new StackPanel
					{
						Orientation = Orientation.Horizontal
					};

					Button b0 = new Button
					{
						Content = "_"
					};
					sp.Children.Add( b0 );

					Button b1 = new Button
					{
						Content = "!!"
					};
					sp.Children.Add( b1 );

					Button b2 = new Button
					{
						Content = "X"
					};
					sp.Children.Add( b2 );

					m_Button = sp;
					AddLogicalChild( m_Button );
					AddVisualChild( m_Button );
				}

				return m_Button;
			}
		}

		protected override Visual GetVisualChild( int index )
		{
			if( VisualChildrenCount - 1 == index )
			{
				return MButton;
			}
			else
				return base.GetVisualChild( index );
		}
		protected override Size ArrangeOverride( Size finalSize )
		{
			int start = 0;
			double y = 0.0;
			Size longSize = new Size( 0, 0 );
			bool isWidthNaN = !double.IsNaN( ItemWidth );
			bool isHeightNaN = !double.IsNaN( ItemHeight );

			int end = 0;
			int count = InternalChildren.Count;
			int lineCount = GetLineCount( isWidthNaN, finalSize.Width );
			int line = 1;

			Size bSize = MButton.DesiredSize;
			Rect rect = new Rect( new Point( finalSize.Width - bSize.Width, finalSize.Height - bSize.Height ), bSize );
			MButton.Arrange( rect );
			Size aviableSize = finalSize;

			if( 1 == lineCount )
			{
				aviableSize.Width -= bSize.Width;
			}

			while( end < count )
			{
				UIElement element = InternalChildren[ end ];

				if( element != null )
				{
					Size elementSize = new Size( isWidthNaN ? ItemWidth : element.DesiredSize.Width, isHeightNaN ? ItemHeight : element.DesiredSize.Height );

					if( longSize.Width + elementSize.Width > aviableSize.Width )
					{
						ArrangeLine( y, longSize.Height, start, end, isWidthNaN );
						y += longSize.Height;
						longSize = elementSize;
						start = end;

						if( ++line == lineCount )
						{
							aviableSize.Width -= bSize.Width;
						}
					}
					else
					{
						longSize.Width += elementSize.Width;
						longSize.Height = Math.Max( elementSize.Height, longSize.Height );
					}
				}

				++end;
			}

			if( start < count )
			{
				ArrangeLine( y, longSize.Height, start, InternalChildren.Count, isWidthNaN );
			}

			return finalSize;
		}
		protected override Size MeasureOverride( Size constraint )
		{
			MButton.Measure( constraint );
			return base.MeasureOverride( constraint );
		}
		private void ArrangeLine( double y, double height, int start, int end, bool isWidthNaN )
		{
			double x = 0.0;

			for( int i = start; i < end; i++ )
			{
				UIElement element = InternalChildren[ i ];

				if( element != null )
				{
					Size size = new Size( element.DesiredSize.Width, element.DesiredSize.Height );
					double width = isWidthNaN ? ItemWidth : size.Width;
					element.Arrange( new Rect( x, y, width, height ) );
					x += width;
				}
			}
		}
		private int GetLineCount( bool isWidthNaN, double finalSizeWidth )
		{
			int line = 1;
			double longSizeWidth = 0;

			foreach( UIElement element in InternalChildren )
			{
				double width = isWidthNaN ? ItemWidth : element.DesiredSize.Width;

				if( longSizeWidth + width > finalSizeWidth )
				{
					longSizeWidth = width;
					++line;
				}
				else
					longSizeWidth += width;
			}

			return line;
		}
	}
}