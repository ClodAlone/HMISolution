#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// An interface for hosting <see cref="InternalArrowButton"/> objects and
	/// receiving clicks from these buttons.
	/// </summary>
	interface IInternalArrowButtonParent
	{
		/// <summary>
		/// Gets / sets the color of arrows in enabled buttons.
		/// </summary>
		Color EnabledColor { get; set; }

		/// <summary>
		/// Gets / sets the color of arrows in disabled buttons.
		/// </summary>
		Color DisabledColor { get; set; }
	}

    /// <summary>
    /// This is a specialized version of the <see cref="InternalButton"/> that draws an arrow. Used by <see cref="ArrowButtonBar"/> in
    /// <see cref="TabBarSplitterControl"/> and <see cref="RecordNavigationControl"/>.
    /// </summary>
	[
	ToolboxItem(false),
	]
	public class InternalArrowButton: InternalButton
    {
		/// <overload>
		/// Initializes a new <see cref="InternalArrowButton"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="InternalArrowButton"/> and specifies the <see cref="ArrowType"/> for the button.
		/// </summary>
		/// <param name="type">Specifies the arrow to be drawn in the button.</param>
        public InternalArrowButton(ArrowType type)
			: this(null, type)
        {
        }

		/// <summary>
		/// Initializes a new <see cref="InternalArrowButton"/> and specifies the <see cref="ArrowType"/> for the button and owner.
		/// </summary>
		/// <param name="owner">The owner of this button.</param>
		/// <param name="type">Specifies the arrow to be drawn in the button.</param>
		public InternalArrowButton(object owner, ArrowType type)
			: base(owner, type)
        {
        }
        
		/// <summary>
		/// Initializes a new <see cref="InternalArrowButton"/> and specifies the <see cref="ArrowType"/> for the button, owner, and ToolTip.
		/// </summary>
		/// <param name="owner">The owner of this button.</param>
		/// <param name="type">Specifies the arrow to be drawn in the button.</param>
		/// <param name="tooltip">The ToolTip for this button.</param>
		public InternalArrowButton(object owner, ArrowType type, string tooltip)
			: base(owner, type, tooltip)
        {
        }
        
		/// <summary>
		/// Returns the arrow to draw.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrowType Type
		{
			get { return (ArrowType) this.Cookie; }
		}
		
		/// <override/>
		public override string ToString()
        {
			return Enum.GetName(typeof(ArrowType), this.Cookie);
        }

		/// <override/>
		public override Size GetPreferredSize(Size maxSize)
        {
            return new Size(Math.Min(maxSize.Width, 19), maxSize.Height);
        }
        
		/// <override/>
		public override void Paint( Graphics g, Rectangle bounds, bool flatLook, Rectangle barArea )
		{
			try
			{
                if ((this.Owner is TabBar))
                
                {
                    if (((this.owner as TabBar).Parent as TabBarSplitterControl).Style == TabBarSplitterStyle.Metro)
                    {
                        if (bounds.Height >= 10)
                        {
                            Color arrowColor;
                            IInternalArrowButtonParent parent = this.Owner as IInternalArrowButtonParent;
                            if (parent != null)
                                arrowColor = Enabled ? parent.EnabledColor : parent.DisabledColor;
                            else
                                arrowColor = Enabled ? SystemColors.WindowText : SystemColors.GrayText;

                            if (this.Style == TabBarSplitterStyle.Office2007)
                            {
                                Color disabledColor = Color.FromArgb(141, 141, 141);
                                arrowColor = Enabled ? this.Office2007ColorTable.TabBarSplitterTextColor : disabledColor;
                            }

                            Point offset = new Point(0, 0);
                            if (Hovered && this.Style != TabBarSplitterStyle.Office2007)
                            {
                                SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
                                g.FillRectangle(brush, bounds);
                                brush.Dispose();
                                arrowColor = Color.White;
                            }
                            if (Pushed && this.Style != TabBarSplitterStyle.Office2007)
                            {
                                //   offset = new Point(1, 1);
                                SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
                                g.FillRectangle(brush, bounds);
                                brush.Dispose();
                                arrowColor = Color.Black;
                            }

                            ArrowType arrowType = (ArrowType)this.Cookie;

                            if (parent is Control && ((Control)parent).RightToLeft == RightToLeft.Yes)
                            {
                                switch (arrowType)
                                {
                                    case ArrowType.First:
                                        arrowType = ArrowType.Last; break;
                                    case ArrowType.Next:
                                        arrowType = ArrowType.Previous; break;
                                    case ArrowType.Previous:
                                        arrowType = ArrowType.Next; break;
                                    case ArrowType.Last:
                                        arrowType = ArrowType.First; break;
                                }

                            }

                            ArrowPaint.DrawArrow(g, bounds, arrowType, offset, arrowColor);

                        }


                    }
                    else
                    {
                        InitToolTip(Rectangle.Intersect(bounds, barArea));

                        if (g.ClipBounds.IntersectsWith(bounds))
                        {
                            ButtonState buttonState = GetWinFormButtonState(flatLook);

                            IInternalButtonParent btnParent = this.Owner as IInternalButtonParent;
                            if (btnParent != null && XPThemes.IsThemedOS && XPThemes.IsThemeActive && btnParent.ThemesEnabled)
                            {
                                if (this.Owner is RecordNavigationScrollBar)
                                {
                                    RecordNavigationScrollBar ScrollBar = (RecordNavigationScrollBar)this.Owner;
                                    RecordNavigationControl control = (RecordNavigationControl)ScrollBar.Parent;
                                    if (control.Office2007ScrollBars || control.GridOfficeScrollBars == OfficeScrollBars.Office2007)
                                    {
                                        switch (control.Office2007ScrollBarsColorScheme)
                                        {
                                            case Syncfusion.Windows.Forms.Office2007ColorScheme.Black:
                                                GridVisualStylesOffice2007Black innerButtonBlack = new GridVisualStylesOffice2007Black(GridVisualStyles.Office2007Black);
                                                innerButtonBlack.DrawPushButtonStyle(g, bounds, buttonState);
                                                break;
                                            case Syncfusion.Windows.Forms.Office2007ColorScheme.Silver:
                                                GridVisualStylesOffice2007Silver innerButtonSilver = new GridVisualStylesOffice2007Silver(GridVisualStyles.Office2007Silver);
                                                innerButtonSilver.DrawPushButtonStyle(g, bounds, buttonState);
                                                break;
                                            case Syncfusion.Windows.Forms.Office2007ColorScheme.Blue:
                                            default:
                                                GridVisualStylesOffice2007Blue innerButtonBlue = new GridVisualStylesOffice2007Blue(GridVisualStyles.Office2007Blue);
                                                innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                                                break;
                                        }
                                    }
                                    else if (control.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                                    {
                                        GridVisualStyles VisualStyle;
                                        switch (control.Office2010ScrollBarsColorScheme)
                                        {
                                            case Syncfusion.Windows.Forms.Office2010ColorScheme.Black:
                                                VisualStyle = GridVisualStyles.Office2010Black;
                                                break;
                                            case Syncfusion.Windows.Forms.Office2010ColorScheme.Silver:
                                                VisualStyle = GridVisualStyles.Office2010Silver;
                                                break;
                                            case Syncfusion.Windows.Forms.Office2010ColorScheme.Blue:
                                            default:
                                                VisualStyle = GridVisualStyles.Office2010Blue;
                                                break;
                                        }
                                        GridVisualStylesOffice2010 innerButtonBlue = new GridVisualStylesOffice2010(VisualStyle);
                                        innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                                    }
                                    else if (control.GridOfficeScrollBars == OfficeScrollBars.Metro)
                                    {
                                        GridMetroStyle innerButtonBlue = new GridMetroStyle(GridVisualStyles.Metro);
                                        innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                                    }
                                    else
                                    {
                                        this.ThemedDrawing.DrawPushButton(g, bounds, buttonState);
                                    }
                                }
                                else
                                {
                                    this.ThemedDrawing.DrawPushButton(g, bounds, buttonState);
                                }
                            }
                            else if (this.Style == TabBarSplitterStyle.Office2007)
                            {
                                Rectangle rect = bounds;

                                using (Brush br = new SolidBrush(Color.White))
                                {
                                    g.FillRectangle(br, rect);
                                }

                                using (Pen pen = new Pen(this.Office2007ColorTable.TabBarSplitterBorderColor))
                                {
                                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                                }

                                rect.Inflate(0, -2);
                                rect.Height += 1;

                                Color lightColor = Color.Empty;
                                Color darkColor = Color.Empty;

                                if (this.Pushed)
                                {
                                    lightColor = this.Office2007ColorTable.TabBarSplitterButtonPushedStartColor;
                                    darkColor = this.Office2007ColorTable.TabBarSplitterButtonPushedEndColor;
                                }
                                else if (this.Hovered)
                                {
                                    lightColor = this.Office2007ColorTable.TabBarSplitterButtonHoveredStartColor;
                                    darkColor = this.Office2007ColorTable.TabBarSplitterButtonHoveredEndColor;
                                }
                                else
                                {
                                    lightColor = this.Office2007ColorTable.TabBarSplitterTabStartColor;
                                    darkColor = this.Office2007ColorTable.TabBarSplitterTabEndColor;
                                }

                                Rectangle rectBrush = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height / 2);
                                using (Brush br = new SolidBrush(lightColor))
                                {
                                    g.FillRectangle(br, rectBrush);
                                }

                                rect.Y -= 1;
                                rectBrush = new Rectangle(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
                                using (LinearGradientBrush linearBr = new LinearGradientBrush(rectBrush, darkColor, lightColor, LinearGradientMode.Vertical))
                                {
                                    g.FillRectangle(linearBr, rectBrush);
                                }

                                rect.Width -= 1;
                                if (this.Pushed || this.Hovered)
                                {
                                    using (Pen pen = new Pen(this.Office2007ColorTable.TabBarSplitterBorderColor))
                                    {
                                        g.DrawRectangle(pen, rect);
                                    }
                                }
                            }
                            else
                            {
                                // Draw a blank button.
                                ControlPaint.DrawButton(g, bounds, buttonState);
                            }

                            // < 10 is too small to draw anything inside.
                            if (bounds.Height >= 10)
                            {
                                Color arrowColor;
                                IInternalArrowButtonParent parent = this.Owner as IInternalArrowButtonParent;
                                if (parent != null)
                                    arrowColor = Enabled ? parent.EnabledColor : parent.DisabledColor;
                                else
                                    arrowColor = Enabled ? SystemColors.WindowText : SystemColors.GrayText;

                                if (this.Style == TabBarSplitterStyle.Office2007)
                                {
                                    Color disabledColor = Color.FromArgb(141, 141, 141);
                                    arrowColor = Enabled ? this.Office2007ColorTable.TabBarSplitterTextColor : disabledColor;
                                }

                                Point offset = new Point(0, 0);
                                if (Pushed && this.Style != TabBarSplitterStyle.Office2007)
                                {
                                    offset = new Point(1, 1);
                                }

                                ArrowType arrowType = (ArrowType)this.Cookie;

                                if (parent is Control && ((Control)parent).RightToLeft == RightToLeft.Yes)
                                {
                                    switch (arrowType)
                                    {
                                        case ArrowType.First:
                                            arrowType = ArrowType.Last; break;
                                        case ArrowType.Next:
                                            arrowType = ArrowType.Previous; break;
                                        case ArrowType.Previous:
                                            arrowType = ArrowType.Next; break;
                                        case ArrowType.Last:
                                            arrowType = ArrowType.First; break;
                                    }

                                }
                                ArrowPaint.DrawArrow(g, bounds, arrowType, offset, arrowColor);
                            }
                        }
                    }
            }
                    else
                    {
                InitToolTip( Rectangle.Intersect( bounds, barArea ) );

                if( g.ClipBounds.IntersectsWith( bounds ) )
				{
					ButtonState buttonState = GetWinFormButtonState( flatLook );
				
					IInternalButtonParent btnParent = this.Owner as IInternalButtonParent;
					if( btnParent != null && XPThemes.IsThemedOS && XPThemes.IsThemeActive && btnParent.ThemesEnabled )
					{
                        if (this.Owner is RecordNavigationScrollBar)
                        {
                            RecordNavigationScrollBar ScrollBar = (RecordNavigationScrollBar)this.Owner;
                            RecordNavigationControl control = (RecordNavigationControl)ScrollBar.Parent;
                            if (control.Office2007ScrollBars || control.GridOfficeScrollBars == OfficeScrollBars.Office2007)
                            {
                                switch (control.Office2007ScrollBarsColorScheme)
                                {
                                    case Syncfusion.Windows.Forms.Office2007ColorScheme.Black:
                                        GridVisualStylesOffice2007Black innerButtonBlack = new GridVisualStylesOffice2007Black(GridVisualStyles.Office2007Black);
                                        innerButtonBlack.DrawPushButtonStyle(g, bounds, buttonState);
                                        break;
                                    case Syncfusion.Windows.Forms.Office2007ColorScheme.Silver:
                                        GridVisualStylesOffice2007Silver innerButtonSilver = new GridVisualStylesOffice2007Silver(GridVisualStyles.Office2007Silver);
                                        innerButtonSilver.DrawPushButtonStyle(g, bounds, buttonState);
                                        break;
                                    case Syncfusion.Windows.Forms.Office2007ColorScheme.Blue:
                                    default:
                                        GridVisualStylesOffice2007Blue innerButtonBlue = new GridVisualStylesOffice2007Blue(GridVisualStyles.Office2007Blue);
                                        innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                                        break;
                                }
                            }
                            else if (control.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                            {
                                GridVisualStyles VisualStyle;
                                switch (control.Office2010ScrollBarsColorScheme)
                                {
                                    case Syncfusion.Windows.Forms.Office2010ColorScheme.Black:
                                        VisualStyle = GridVisualStyles.Office2010Black;
                                        break;
                                    case Syncfusion.Windows.Forms.Office2010ColorScheme.Silver:
                                        VisualStyle = GridVisualStyles.Office2010Silver;
                                        break;
                                    case Syncfusion.Windows.Forms.Office2010ColorScheme.Blue:
                                    default:
                                        VisualStyle = GridVisualStyles.Office2010Blue;
                                        break;
                                }
                                GridVisualStylesOffice2010 innerButtonBlue = new GridVisualStylesOffice2010(VisualStyle);
                                innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                            }
                            else if (control.GridOfficeScrollBars == OfficeScrollBars.Metro)
                            {
                                GridMetroStyle innerButtonBlue = new GridMetroStyle(GridVisualStyles.Metro);
                                innerButtonBlue.DrawPushButtonStyle(g, bounds, buttonState);
                            }
                            else
                            {
                                this.ThemedDrawing.DrawPushButton(g, bounds, buttonState);
                            }
                        }
                        else
                        {
                            this.ThemedDrawing.DrawPushButton(g, bounds, buttonState);
                        }
                    }
                    else if( this.Style == TabBarSplitterStyle.Office2007 )
                    {
                        Rectangle rect = bounds;

                        using( Brush br = new SolidBrush( Color.White ) )
                        {
                            g.FillRectangle( br, rect );
                        }

                        using( Pen pen = new Pen( this.Office2007ColorTable.TabBarSplitterBorderColor ) )
                        {
                            g.DrawLine( pen, rect.Left, rect.Top, rect.Right, rect.Top );
                            g.DrawLine( pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1 );
                        }

                        rect.Inflate( 0, -2 );
                        rect.Height += 1;

                        Color lightColor = Color.Empty;
                        Color darkColor = Color.Empty;

                        if( this.Pushed )
                        {
                            lightColor = this.Office2007ColorTable.TabBarSplitterButtonPushedStartColor;
                            darkColor = this.Office2007ColorTable.TabBarSplitterButtonPushedEndColor;
                        }
                        else if( this.Hovered )
                        {
                            lightColor = this.Office2007ColorTable.TabBarSplitterButtonHoveredStartColor;
                            darkColor = this.Office2007ColorTable.TabBarSplitterButtonHoveredEndColor;
                        }
                        else
                        {
                            lightColor = this.Office2007ColorTable.TabBarSplitterTabStartColor;
                            darkColor = this.Office2007ColorTable.TabBarSplitterTabEndColor;
                        }

                        Rectangle rectBrush = new Rectangle( rect.Left, rect.Top, rect.Width, rect.Height / 2 );
                        using( Brush br = new SolidBrush( lightColor ) )
                        {
                            g.FillRectangle( br, rectBrush );
                        }

                        rect.Y -= 1;
                        rectBrush = new Rectangle( rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2 );
                        using( LinearGradientBrush linearBr = new LinearGradientBrush( rectBrush, darkColor, lightColor, LinearGradientMode.Vertical ) )
                        {
                            g.FillRectangle( linearBr, rectBrush );
                        }
                        
                        rect.Width -= 1;
                        if( this.Pushed || this.Hovered )
                        {
                            using( Pen pen = new Pen( this.Office2007ColorTable.TabBarSplitterBorderColor ) )
                            {
                                g.DrawRectangle( pen, rect );
                            }
                        }
                    }
                    else
					{
						// Draw a blank button.
						ControlPaint.DrawButton( g, bounds, buttonState );
					}
				
					// < 10 is too small to draw anything inside.
					if( bounds.Height >= 10 )
					{
						Color arrowColor;
						IInternalArrowButtonParent parent = this.Owner as IInternalArrowButtonParent;
						if( parent != null )
							arrowColor = Enabled ? parent.EnabledColor : parent.DisabledColor;
						else
							arrowColor = Enabled ? SystemColors.WindowText : SystemColors.GrayText;

                        if( this.Style == TabBarSplitterStyle.Office2007 )
                        {
                            Color disabledColor = Color.FromArgb( 141, 141, 141 );
                            arrowColor = Enabled ? this.Office2007ColorTable.TabBarSplitterTextColor : disabledColor;
                        }

                        Point offset = new Point( 0, 0 );
                        if( Pushed && this.Style != TabBarSplitterStyle.Office2007 ) 
                        {
                            offset = new Point( 1, 1 );
                        }

						ArrowType arrowType = ( ArrowType ) this.Cookie;

						if( parent is Control && ( ( Control ) parent ).RightToLeft == RightToLeft.Yes )
						{
							switch( arrowType )
							{
								case ArrowType.First:
									arrowType = ArrowType.Last; break;
								case ArrowType.Next:
									arrowType = ArrowType.Previous; break;
								case ArrowType.Previous:
									arrowType = ArrowType.Next; break;
								case ArrowType.Last:
									arrowType = ArrowType.First; break;
							}

						}
						ArrowPaint.DrawArrow( g, bounds, arrowType, offset, arrowColor );	
					}
				}
            }
        
			}
			finally
			{
				this.Dirty = false;
			}
		}
    }
}
