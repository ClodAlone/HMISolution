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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Draws a themed check button.
	/// </summary>
	public class ThemedCheckButton : ThemedButtonBase
	{
		ThemedControlDrawing tcd;

       	/// <summary>
		/// Indicates the checked state of the button.
		/// </summary>
		public bool Checked
		{
			get{return this.CheckState == CheckState.Checked;}
			set
			{
				CheckState = (value?CheckState.Checked:CheckState.Unchecked);
			}
		}
		/// <summary>
		/// MetroColor
		/// </summary>
        private Color metroColor = Color.FromArgb(22,165,220);
		/// <summary>
		/// Gets or setsthe metrocolor.
		/// </summary>
        public Color MetroColor
        {
            get { return metroColor; }
            set
            {
                metroColor = value;
            }
        }
        private bool m_bSelected = false;
        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        /// <summary>
		/// Disposes all resources.
		/// </summary>
		/// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(tcd!=null)
				{
					tcd.Dispose();
					tcd = null;
				}
			}
		}

		/// <summary>
		/// Initializes a new object.
		/// </summary>
		public ThemedCheckButton() 
		{
			this.SetStyle(ControlStyles.Selectable,false);
			if(XPThemes.IsThemedOS && XPThemes.IsAppThemed && XPThemes.IsThemeActive)
			{
				tcd = new Syncfusion.Windows.Forms.ThemedControlDrawing(Syncfusion.Windows.Forms.ThemedControls.BUTTON);
			}
		}

		/// <override/>
		protected override void DrawThemedControl(Graphics g,ButtonState buttonState,CheckState checkState)
		{
			int partNr = 1;
			switch(checkState)
			{
				case CheckState.Unchecked: partNr = 1;break;
				case CheckState.Checked: partNr = 5;break;
				case CheckState.Indeterminate: partNr = 9; break;
			}
			if(mouseOver && buttonState!=ButtonState.Pushed)
			{
				partNr +=1;
			}
			else
			{
				switch(buttonState)
				{
					case ButtonState.Normal: partNr+=0;break;
					case ButtonState.Flat: partNr +=1;break;
					case ButtonState.Pushed:partNr +=2;break;
					case ButtonState.Inactive: partNr+=3;break;
				}
			}
			tcd.DrawThemeBackground(g,3,partNr,ClientRectangle);

		}

		/// <override/>
		protected override void DrawNotThemedControl(Graphics g,ButtonState buttonState,CheckState checkState)
		{	
			ButtonState bstate = buttonState;
            
            switch (checkState)
            {
                case CheckState.Checked:
                    {
                        bstate = ButtonState.Checked;
                        if (buttonState == ButtonState.Inactive)
                        {
                            bstate = ButtonState.All;
                        }
                        break;
                    }
                case CheckState.Indeterminate: bstate = ButtonState.All; break;
            }
            
            ControlPaint.DrawCheckBox(g, ClientRectangle, bstate);
		}

        protected override void DrawStyledControl(Graphics g, ButtonState buttonState, CheckState checkState)
        {
            Rectangle innerRect = this.ClientRectangle;
            if (this.Style == VisualStyle.Office2007)
            {
                g.DrawRectangle(this.GetOffice2007BorderPen(buttonState), 0, 0, this.Width - 1, this.Height - 1);

                Color penColor;
                Color color;

                if (buttonState == ButtonState.Pushed)
                {
                    penColor = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectBorderPushedColor;
                    color = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectFillPushedColor;
                }
                else if (this.mouseOver || this.Parent.Focused || this.Selected || checkState == CheckState.Indeterminate)
                {
                    penColor = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectBorderSelectedColor;
                    color = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectFillSelectedColor;
                }
                else //buttonState == ButtonState.Normal
                {
                    penColor = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                    color = this.office2007ColorTable.DataTimePickerCheckBoxInnerRectFillNormalColor;
                }

                //Rectangle innerRect = this.ClientRectangle;

                Pen pen = new Pen(new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal));
                LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

                innerRect.Inflate(-2, -2);
                innerRect.Width--;
                innerRect.Height--;

                g.DrawRectangle(pen, innerRect);
                pen.Dispose();
                innerRect.Inflate(-1, -1);
                innerRect.Width++;
                innerRect.Height++;

                g.FillRectangle(brush, innerRect);

                if (checkState != CheckState.Unchecked)
                {
                    Pen checkPen;

                    if (checkState == CheckState.Checked)
                    {
                        if (this.mouseOver || this.Parent.Focused || this.Selected)
                            checkPen = new Pen(this.office2007ColorTable.DataTimePickerCheckBoxSelectedColor, 2);
                        else
                            checkPen = new Pen(this.office2007ColorTable.DataTimePickerCheckBoxNormalColor, 2);
                    }
                    else
                        checkPen = new Pen(this.office2007ColorTable.DataTimePickerCheckBoxBorderNormalColor, 2);

                    Point[] points = new Point[] {
                                                    new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
                                                    new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
                                                    new Point( innerRect.Right - 1, innerRect.Top -1 )
                                                    };

                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLines(checkPen, points);
                    g.SmoothingMode = prevMode;
                    checkPen.Dispose();
                }
            }
            else if (this.Style == VisualStyle.Office2010)
            {
                g.DrawRectangle(this.GetOffice2010BorderPen(buttonState), 0, 0, this.Width - 1, this.Height - 1);

                Color penColor;
                Color color;

                if (buttonState == ButtonState.Pushed)
                {
                    penColor = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectBorderPushedColor;
                    color = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectFillPushedColor;
                }
                else if (this.mouseOver || this.Parent.Focused || this.Selected || checkState == CheckState.Indeterminate)
                {
                    penColor = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectBorderSelectedColor;
                    color = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectFillSelectedColor;
                }
                else //buttonState == ButtonState.Normal
                {
                    penColor = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectBorderNormalColor;
                    color = this.office2010ColorTable.DataTimePickerCheckBoxInnerRectFillNormalColor;
                }

                //Rectangle innerRect = this.ClientRectangle;

                Pen pen = new Pen(new LinearGradientBrush(innerRect, penColor, Color.White, LinearGradientMode.ForwardDiagonal));
                LinearGradientBrush brush = new LinearGradientBrush(innerRect, color, Color.White, LinearGradientMode.ForwardDiagonal);

                innerRect.Inflate(-2, -2);
                innerRect.Width--;
                innerRect.Height--;

                g.DrawRectangle(pen, innerRect);
                pen.Dispose();
                innerRect.Inflate(-1, -1);
                innerRect.Width++;
                innerRect.Height++;

                g.FillRectangle(brush, innerRect);

                if (checkState != CheckState.Unchecked)
                {
                    Pen checkPen;

                    if (checkState == CheckState.Checked)
                    {
                        if (this.mouseOver || this.Parent.Focused || this.Selected)
                            checkPen = new Pen(this.office2010ColorTable.DataTimePickerCheckBoxSelectedColor, 2);
                        else
                            checkPen = new Pen(this.office2010ColorTable.DataTimePickerCheckBoxNormalColor, 2);
                    }
                    else
                        checkPen = new Pen(this.office2010ColorTable.DataTimePickerCheckBoxBorderNormalColor, 2);

                    Point[] points = new Point[] {
                                                    new Point( innerRect.X, innerRect.Y + innerRect.Height / 2 ),
                                                    new Point( innerRect.X + innerRect.Width / 2 - 1, innerRect.Bottom - 2 ),
                                                    new Point( innerRect.Right - 1, innerRect.Top -1 )
                                                    };

                    SmoothingMode prevMode = g.SmoothingMode;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLines(checkPen, points);
                    g.SmoothingMode = prevMode;
                    checkPen.Dispose();
                }
            }
            else if (this.Style == VisualStyle.Metro)
            {
                using (Pen pen = new Pen(MetroColor))
                {
                    g.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
                if (this.CheckState == CheckState.Checked)
                {
                    Point[] points = new Point[] {
                                                   new Point(innerRect.X + 3, innerRect.Y + 6),
                                                   new Point(innerRect.X + 5, innerRect.Y + 9), 
                                                   new Point(innerRect.X + 9, innerRect.Y + 2) 
                                                    };
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen penBlack = new Pen(MetroColor, 2))
                    {
                        g.DrawLines(penBlack, points);
                    }
                }
            }
            else
                this.DrawNotThemedControl(g, buttonState, checkState);
        }

        private Pen GetOffice2007BorderPen(ButtonState buttonState)
        {
            Pen pen;

            if (buttonState == ButtonState.Pushed)
                pen = new Pen(this.office2007ColorTable.DataTimePickerCheckBoxBorderPushedColor);
            else if (this.mouseOver || this.Parent.Focused || this.Selected)
                pen = new Pen(this.office2007ColorTable.DataTimePickerBorderColor);
            else //buttonState == ButtonState.Normal
                pen = new Pen(this.office2007ColorTable.DataTimePickerCheckBoxBorderNormalColor);

            return pen;
        }

        private Pen GetOffice2010BorderPen(ButtonState buttonState)
        {
            Pen pen;

            if (buttonState == ButtonState.Pushed)
                pen = new Pen(this.office2010ColorTable.DataTimePickerCheckBoxBorderPushedColor);
            else if (this.mouseOver || this.Parent.Focused || this.Selected)
                pen = new Pen(this.office2010ColorTable.DataTimePickerBorderColor);
            else //buttonState == ButtonState.Normal
                pen = new Pen(this.office2010ColorTable.DataTimePickerCheckBoxBorderNormalColor);

            return pen;
        }
	}
}
