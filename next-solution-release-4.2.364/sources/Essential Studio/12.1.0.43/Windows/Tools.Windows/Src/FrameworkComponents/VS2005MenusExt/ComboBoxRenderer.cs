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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region Office12ToolStripRenderer
	public partial class Office12ToolStripRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
	{
		#region ComboBoxRenderer
		class ComboBoxRenderer : INativeMessageFilter
        {
            #region ComboBoxRenderer Constants
            const int STATE_SYSTEM_INVISIBLE = 0x00008000;
			const int STATE_SYSTEM_SELECTED = 0x00000002;
			const int STATE_SYSTEM_PRESSED = 0x00000008;

			const int ARROW_WIDTH = 5;
			const int ARROW_HEIGHT = 3;
			const int ARROW_PADDING = 4;
			const int BUTTON_WIDTH = ARROW_WIDTH + 2 * ARROW_PADDING;
			#endregion

			#region Constructor/Destructor
			public ComboBoxRenderer(Office12ToolStripRenderer parentRenderer)
			{
				m_parentRenderer = new WeakReference(parentRenderer);
			}
			#endregion

			#region INativeMessageFilter implementation
			bool INativeMessageFilter.ProcessMessage(ref Message m)
			{
				switch ((Msg)m.Msg)
				{
					case Msg.WM_CTLCOLOREDIT:
					case Msg.WM_CTLCOLORSTATIC:
						return OnCtlColorEdit(ref m);
					case Msg.WM_PAINT:
						return OnPaint(ref m);
					case Msg.WM_MOUSEMOVE:
						return OnMouseMove(ref m);
					case Msg.WM_MOUSELEAVE:
						return OnMouseLeave(ref m);
				}
				return false;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			private bool OnMouseMove(ref Message m)
			{
				if(m_hHoverWindow!=m.HWnd)
				{
					WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
					m_hHoverWindow = m.HWnd;
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			private bool OnMouseLeave(ref Message m)
			{
				WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
				m_hHoverWindow = IntPtr.Zero;
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			bool OnCtlColorEdit(ref Message m)
			{
				ComboBox cb = Control.FromHandle(m.HWnd) as ComboBox;
                if (!GetIsHighlighted(cb) && !SystemInformation.HighContrast)
                {
                    Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
                    if (parentRenderer != null)
                    {
                        Office12ColorTable colorTable = parentRenderer.ColorTable as Office12ColorTable;
                        if (colorTable != null)
                        {
                            WindowsAPI.SetBkMode(m.WParam, BackgroundMode.TRANSPARENT);
                            m.Result = colorTable.ComboBoxBackgroundBrush;
                            return true;
                        }
                    }
                }
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			bool OnPaint(ref Message m)
			{
				bool bResult = false;

				PAINTINFO pi = new PAINTINFO();

				pi.cb = Control.FromHandle(m.HWnd) as ComboBox;
				if (pi.cb != null)
				{
					pi.cbi.cbSize = (uint)Marshal.SizeOf(pi.cbi);

					if (WindowsAPI.GetComboBoxInfo(m.HWnd, ref pi.cbi))
					{
						PAINTSTRUCT stPaint = new PAINTSTRUCT();
						IntPtr hdc = WindowsAPI.BeginPaint(m.HWnd, ref stPaint);

						if (hdc != IntPtr.Zero)
						{
							Rectangle rcClient = pi.cb.ClientRectangle;

							if (rcClient.Width > 0 && rcClient.Height > 0)
							{
								using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcClient))
								{
									pi.graphics = bg.Graphics;

									DrawBackground(ref pi, ref rcClient);

									if (pi.cb.DropDownStyle==ComboBoxStyle.DropDownList)
									{
										DrawItem(ref pi);
									}

									if (pi.cbi.stateButton != STATE_SYSTEM_INVISIBLE)
									{
										DrawButton(ref pi);
									}

									bg.Render();
								}
							}
						}

						WindowsAPI.EndPaint(m.HWnd, ref stPaint);
						bResult = true;
					}
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			/// <param name="rcClient"></param>
			void DrawBackground(ref PAINTINFO pi, ref Rectangle rcClient)
			{
				Color bkColorc = GetIsHighlighted(pi.cb) ? SystemColors.Window : this.BackgroundColor;

				using (Brush brush = new SolidBrush(bkColorc))
				{
					pi.graphics.FillRectangle(brush, rcClient);
				}
                if (SystemInformation.HighContrast)
                {
                    using (Pen pen = new Pen(SystemColors.MenuHighlight))
                    {
                        pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                    }
                }
                else
                {
                    using (Pen pen = new Pen(this.BorderColor))
                    {
                        pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			private void DrawItem(ref PAINTINFO pi)
			{
				ComboBox cb = pi.cb;

				object oSelected = cb.SelectedItem;
				if (oSelected != null)
				{
					Graphics g = pi.graphics;
                    string sText = cb.Text;
					
					Rectangle rc = (Rectangle)pi.cbi.rcItem;
					Rectangle rcText = Rectangle.Inflate(rc, -1, -1);
					TextFormatFlags tf = cb.RightToLeft == RightToLeft.Yes ? TextFormatFlags.NoPadding | TextFormatFlags.Right : TextFormatFlags.NoPadding;

					if (cb.Focused && !cb.DroppedDown)
					{
						g.FillRectangle(SystemBrushes.Highlight, rcText);
						TextRenderer.DrawText(g, sText, cb.Font, rcText, SystemColors.HighlightText, tf);
						
						ControlPaint.DrawFocusRectangle(g, rc);
					}
					else
					{
						TextRenderer.DrawText(g, sText, cb.Font, rcText, cb.ForeColor, tf);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			void DrawButton(ref PAINTINFO pi)
			{
				Rectangle rcButton = (Rectangle)pi.cbi.rcButton;

				if (rcButton.Width > BUTTON_WIDTH)
				{
					if (pi.cb.RightToLeft != RightToLeft.Yes)
					{
						rcButton.X += rcButton.Width - BUTTON_WIDTH;
					}
					rcButton.Width = BUTTON_WIDTH;
				}

				if (!DrawButtonPressed(ref pi, ref rcButton))
				{
					if (!DrawButtonSelected(ref pi, ref rcButton))
					{
						DrawButtonNormal(ref pi, ref rcButton);
					}
				}
				DrawArrow(ref pi, ref rcButton);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			/// <param name="rcButton"></param>
			/// <returns></returns>
			bool DrawButtonPressed(ref PAINTINFO pi, ref Rectangle rcButton)
			{
				bool bResult = false;

				if ( GetIsPressed(pi.cb) )
				{
					if (rcButton.Width > 0 && rcButton.Height > 0)
					{
						Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
						if (parentRenderer != null)
						{
							Graphics g = pi.graphics;

							Color cl1 = parentRenderer.ColorTable.ButtonPressedGradientBegin;
							Color cl2 = parentRenderer.ColorTable.ButtonPressedGradientEnd;

							parentRenderer.PaintGradientSelected(g, rcButton, cl1, cl2);

							g.DrawRectangle(parentRenderer.ButtonPressedBorder, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
							g.DrawRectangle(parentRenderer.ButtonHighlightBorder, rcButton.X + 1, rcButton.Y + 1, rcButton.Width - 3, rcButton.Height - 3);
						}
					}
					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			/// <param name="rcButton"></param>
			/// <returns></returns>
			private bool DrawButtonSelected(ref PAINTINFO pi, ref Rectangle rcButton)
			{
				bool bResult = false;

				if (GetIsSelected(pi.cb))
				{
                    if (!(pi.cb.TopLevelControl is Form) || rcButton.Width > 0 && rcButton.Height > 0 && pi.cb.TopLevelControl.ContainsFocus)
					{
						Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
						if (parentRenderer != null)
						{
							Graphics g = pi.graphics;

							Color cl1 = parentRenderer.ColorTable.ButtonSelectedGradientBegin;
							Color cl2 = parentRenderer.ColorTable.ButtonSelectedGradientEnd;

							parentRenderer.PaintGradientSelected(g, rcButton, cl1, cl2);

							g.DrawRectangle(parentRenderer.ButtonSelectedBorder, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
							g.DrawRectangle(parentRenderer.ButtonHighlightBorder, rcButton.X + 1, rcButton.Y + 1, rcButton.Width - 3, rcButton.Height - 3);
						}
					}
					
					bResult = true;
				}
				
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			/// <param name="rcButton"></param>
			void DrawButtonNormal(ref PAINTINFO pi, ref Rectangle rcButton)
			{
                if (!SystemInformation.HighContrast)
                {
                    if (rcButton.Width > 0 && rcButton.Height > 0)
                    {
                        Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
                        if (parentRenderer != null)
                        {
                            Graphics g = pi.graphics;
                            parentRenderer.PaintGradientGrouped(g, rcButton);
                            using (Pen pen = new Pen(this.BorderColor))
                            {
                                g.DrawRectangle(pen, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
                            }
                        }
                    }
                }
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="pi"></param>
			/// <param name="rcButton"></param>
			void DrawArrow(ref PAINTINFO pi, ref Rectangle rcButton)
			{
				int x = rcButton.X + rcButton.Width / 2;
				int y = rcButton.Y + rcButton.Height / 2;

				Point[] points = new Point[]
				{
					new Point(x - 2, y - 1), 
					new Point(x + 3, y - 1),
					new Point(x, y + 2)
				};

				Brush brush = pi.cb.Enabled ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
                if (SystemInformation.HighContrast)
                    brush = SystemBrushes.Highlight;
				pi.graphics.FillPolygon(brush, points);
			}

			/// <summary>
			/// 
			/// </summary>
			bool GetIsHighlighted(ComboBox cb)
			{
				if (cb != null && cb.Enabled)
				{
					return GetIsSelected(cb) || GetIsPressed(cb);
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="cb"></param>
			/// <returns></returns>
			private bool GetIsSelected(ComboBox cb)
			{
				bool bResult = false;
				
				if(cb!=null)
				{
					bResult = cb.Focused;
					if(!bResult)
					{
						Rectangle rc = cb.RectangleToScreen(cb.ClientRectangle);
						bResult = rc.Contains(Cursor.Position);
					}
				}
				
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="cb"></param>
			/// <returns></returns>
			private bool GetIsPressed(ComboBox cb)
			{
				if (cb != null)
				{
					return cb.DroppedDown;
				}
				return false;
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			Color BorderColor
			{
				get
				{
					Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
					if (parentRenderer != null)
					{
						Office12ColorTable colorTable = parentRenderer.ColorTable as Office12ColorTable;

						if (colorTable != null)
						{
							return colorTable.GroupBorder;
						}
					}
					return SystemColors.WindowFrame;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			Color BackgroundColor
			{
				get
				{
					Office12ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office12ToolStripRenderer;
					if (parentRenderer != null)
					{
						Office12ColorTable colorTable = parentRenderer.ColorTable as Office12ColorTable;

						if (colorTable != null)
						{
                            Color color = colorTable.ComboBoxBackgroundColor;
                            if(SystemInformation.HighContrast)
                                color = Color.Black;
                            return color;
						}
					}
					return SystemColors.Window;
				}
			}
			#endregion

			#region Fields
			WeakReference m_parentRenderer;
			IntPtr m_hHoverWindow;
			#endregion
		}
		#endregion
	}
	#endregion

    #region Office2010ToolStripRenderer
    public partial class Office2010ToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region ComboBoxRenderer
        class ComboBoxRenderer : INativeMessageFilter
        {
            #region ComboBoxRenderer Constants
            const int STATE_SYSTEM_INVISIBLE = 0x00008000;
            const int STATE_SYSTEM_SELECTED = 0x00000002;
            const int STATE_SYSTEM_PRESSED = 0x00000008;

            const int ARROW_WIDTH = 5;
            const int ARROW_HEIGHT = 3;
            const int ARROW_PADDING = 4;
            const int BUTTON_WIDTH = ARROW_WIDTH + 2 * ARROW_PADDING;
            #endregion

            #region Constructor/Destructor
            public ComboBoxRenderer(Office2010ToolStripRenderer parentRenderer)
            {
                m_parentRenderer = new WeakReference(parentRenderer);
            }
            #endregion

            #region INativeMessageFilter implementation
            bool INativeMessageFilter.ProcessMessage(ref Message m)
            {
                switch ((Msg)m.Msg)
                {
                    case Msg.WM_CTLCOLOREDIT:
                    case Msg.WM_CTLCOLORSTATIC:
                        return OnCtlColorEdit(ref m);
                    case Msg.WM_PAINT:
                        return OnPaint(ref m);
                    case Msg.WM_MOUSEMOVE:
                        return OnMouseMove(ref m);
                    case Msg.WM_MOUSELEAVE:
                        return OnMouseLeave(ref m);
                }
                return false;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            private bool OnMouseMove(ref Message m)
            {
                if (m_hHoverWindow != m.HWnd)
                {
                    WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
                    m_hHoverWindow = m.HWnd;
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            private bool OnMouseLeave(ref Message m)
            {
                WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
                m_hHoverWindow = IntPtr.Zero;
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            bool OnCtlColorEdit(ref Message m)
            {
                ComboBox cb = Control.FromHandle(m.HWnd) as ComboBox;

                if (!GetIsHighlighted(cb) && !SystemInformation.HighContrast)
                {
                    Office2010ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2010ToolStripRenderer;
                    if (parentRenderer != null)
                    {
                        Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                        if (colorTable != null)
                        {
                            WindowsAPI.SetBkMode(m.WParam, BackgroundMode.TRANSPARENT);

                            m.Result = colorTable.ComboBoxBackgroundBrush;
                            return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            bool OnPaint(ref Message m)
            {
                bool bResult = false;

                PAINTINFO pi = new PAINTINFO();

                pi.cb = Control.FromHandle(m.HWnd) as ComboBox;
                if (pi.cb != null)
                {
                    pi.cbi.cbSize = (uint)Marshal.SizeOf(pi.cbi);

                    if (WindowsAPI.GetComboBoxInfo(m.HWnd, ref pi.cbi))
                    {
                        PAINTSTRUCT stPaint = new PAINTSTRUCT();
                        IntPtr hdc = WindowsAPI.BeginPaint(m.HWnd, ref stPaint);

                        if (hdc != IntPtr.Zero)
                        {
                            Rectangle rcClient = pi.cb.ClientRectangle;

                            if (rcClient.Width > 0 && rcClient.Height > 0)
                            {
                                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcClient))
                                {
                                    pi.graphics = bg.Graphics;

                                    DrawBackground(ref pi, ref rcClient);

                                    if (pi.cb.DropDownStyle == ComboBoxStyle.DropDownList)
                                    {
                                        DrawItem(ref pi);
                                    }

                                    if (pi.cbi.stateButton != STATE_SYSTEM_INVISIBLE)
                                    {
                                        DrawButton(ref pi);
                                    }

                                    bg.Render();
                                }
                            }
                        }

                        WindowsAPI.EndPaint(m.HWnd, ref stPaint);
                        bResult = true;
                    }
                }
                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcClient"></param>
            void DrawBackground(ref PAINTINFO pi, ref Rectangle rcClient)
            {
                Color bkColorc = GetIsHighlighted(pi.cb) ? SystemColors.Window : this.BackgroundColor;

                using (Brush brush = new SolidBrush(bkColorc))
                {
                    pi.graphics.FillRectangle(brush, rcClient);
                }

                Office2010ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2010ToolStripRenderer;
                
                if (parentRenderer != null)
                {
                    Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

 					if (SystemInformation.HighContrast)
                    {
                        using (Pen pen = new Pen(SystemColors.MenuHighlight))
                        {
                            pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                        {
                            pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                        }
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            private void DrawItem(ref PAINTINFO pi)
            {
                ComboBox cb = pi.cb;

                object oSelected = cb.SelectedItem;
                if (oSelected != null)
                {
                    Graphics g = pi.graphics;
                    string sText = cb.Text;

                    Rectangle rc = (Rectangle)pi.cbi.rcItem;
                    Rectangle rcText = Rectangle.Inflate(rc, -1, -1);
                    TextFormatFlags tf = cb.RightToLeft == RightToLeft.Yes ? TextFormatFlags.NoPadding | TextFormatFlags.Right : TextFormatFlags.NoPadding;

                    if (cb.Focused && !cb.DroppedDown)
                    {
                        g.FillRectangle(SystemBrushes.Highlight, rcText);
                        TextRenderer.DrawText(g, sText, cb.Font, rcText, SystemColors.HighlightText, tf);

                        ControlPaint.DrawFocusRectangle(g, rc);
                    }
                    else
                    {
                        TextRenderer.DrawText(g, sText, cb.Font, rcText, cb.ForeColor, tf);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            void DrawButton(ref PAINTINFO pi)
            {
                Rectangle rcButton = (Rectangle)pi.cbi.rcButton;

                if (rcButton.Width > BUTTON_WIDTH)
                {
                    if (pi.cb.RightToLeft != RightToLeft.Yes)
                    {
                        rcButton.X += rcButton.Width - BUTTON_WIDTH;
                    }
                    rcButton.Width = BUTTON_WIDTH;
                }

                if (!DrawButtonPressed(ref pi, ref rcButton))
                {
                    if (!DrawButtonSelected(ref pi, ref rcButton))
                    {
                        DrawButtonNormal(ref pi, ref rcButton);
                    }
                }
                DrawArrow(ref pi, ref rcButton);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            /// <returns></returns>
            bool DrawButtonPressed(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                bool bResult = false;

                if (GetIsPressed(pi.cb))
                {
                    if (rcButton.Width > 0 && rcButton.Height > 0)
                    {
                        Office2010ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2010ToolStripRenderer;
                        if (parentRenderer != null)
                        {
                            Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                            Graphics g = pi.graphics;

                            using (Brush brush = new LinearGradientBrush(rcButton, colorTable.ToolstripButtonPressedBackground, colorTable.ToolstripButtonSelectedBottomCenterColor, LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(brush, rcButton);
                            }

                            using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                            {
                                g.DrawRectangle(pen, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
                            }
                        }
                    }
                    bResult = true;
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            /// <returns></returns>
            private bool DrawButtonSelected(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                bool bResult = false;

                if (GetIsSelected(pi.cb))
                {
                    if (!(pi.cb.TopLevelControl is Form) || rcButton.Width > 0 && rcButton.Height > 0 && pi.cb.TopLevelControl.ContainsFocus)
                    {
                        Office2010ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2010ToolStripRenderer;
                        if (parentRenderer != null)
                        {
                            Graphics g = pi.graphics;

                            Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                            using (Brush brush = new LinearGradientBrush(rcButton, colorTable.ComboDropDownSelectedGradientBegin, colorTable.ComboDropDownSelectedGradientEnd, LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(brush, Rectangle.Inflate(rcButton, -1, -1));
                            }

                            using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                            {
                                g.DrawRectangle(pen, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
                            }
                        }
                    }

                    bResult = true;
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            void DrawButtonNormal(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                return;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            void DrawArrow(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                int x = rcButton.X + rcButton.Width / 2;
                int y = rcButton.Y + rcButton.Height / 2;

                Point[] points = new Point[]
				{
					new Point(x - 2, y - 1), 
					new Point(x + 3, y - 1),
					new Point(x, y + 2)
				};

                Brush brush = pi.cb.Enabled ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
                if (SystemInformation.HighContrast)
                    brush = SystemBrushes.Highlight;
                pi.graphics.FillPolygon(brush, points);
            }

            /// <summary>
            /// 
            /// </summary>
            bool GetIsHighlighted(ComboBox cb)
            {
                if (cb != null && cb.Enabled)
                {
                    return GetIsSelected(cb) || GetIsPressed(cb);
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private bool GetIsSelected(ComboBox cb)
            {
                bool bResult = false;

                if (cb != null)
                {
                    bResult = cb.Focused;
                    if (!bResult)
                    {
                        Rectangle rc = cb.RectangleToScreen(cb.ClientRectangle);
                        bResult = rc.Contains(Cursor.Position);
                    }
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private bool GetIsPressed(ComboBox cb)
            {
                if (cb != null)
                {
                    return cb.DroppedDown;
                }
                return false;
            }
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            Color BackgroundColor
            {
                get
                {
                    Office2010ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2010ToolStripRenderer;
                    if (parentRenderer != null)
                    {
                        Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                        if (colorTable != null)
                        {
                            Color color = colorTable.ComboBoxBackgroundColor;
                            if (SystemInformation.HighContrast)
                                color = Color.Black;
                            return color;
                        }
                    }
                    return SystemColors.Window;
                }
            }
            #endregion

            #region Fields
            WeakReference m_parentRenderer;
            IntPtr m_hHoverWindow;
            #endregion
        }
        #endregion
    }
    #endregion
    #region Office2013ToolStripRenderer
    public partial class Office2013ToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region ComboBoxRenderer
        class ComboBoxRenderer : INativeMessageFilter
        {
            #region ComboBoxRenderer Constants
            const int STATE_SYSTEM_INVISIBLE = 0x00008000;
            const int STATE_SYSTEM_SELECTED = 0x00000002;
            const int STATE_SYSTEM_PRESSED = 0x00000008;

            const int ARROW_WIDTH = 5;
            const int ARROW_HEIGHT = 3;
            const int ARROW_PADDING = 4;
            const int BUTTON_WIDTH = ARROW_WIDTH + 2 * ARROW_PADDING;
            #endregion

            #region Constructor/Destructor
            public ComboBoxRenderer(Office2013ToolStripRenderer parentRenderer)
            {
                m_parentRenderer = new WeakReference(parentRenderer);
            }
            #endregion

            #region INativeMessageFilter implementation
            bool INativeMessageFilter.ProcessMessage(ref Message m)
            {
                switch ((Msg)m.Msg)
                {
                    case Msg.WM_CTLCOLOREDIT:
                    case Msg.WM_CTLCOLORSTATIC:
                        return OnCtlColorEdit(ref m);
                    case Msg.WM_PAINT:
                        return OnPaint(ref m);
                    case Msg.WM_MOUSEMOVE:
                        return OnMouseMove(ref m);
                    case Msg.WM_MOUSELEAVE:
                        return OnMouseLeave(ref m);
                }
                return false;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            private bool OnMouseMove(ref Message m)
            {
                if (m_hHoverWindow != m.HWnd)
                {
                    WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
                    m_hHoverWindow = m.HWnd;
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            private bool OnMouseLeave(ref Message m)
            {
                WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_ALLCHILDREN | RedrawWindowFlags.RDW_INVALIDATE);
                m_hHoverWindow = IntPtr.Zero;
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            bool OnCtlColorEdit(ref Message m)
            {
                ComboBox cb = Control.FromHandle(m.HWnd) as ComboBox;

                if (!GetIsHighlighted(cb) && !SystemInformation.HighContrast)
                {
                    Office2013ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2013ToolStripRenderer;
                    if (parentRenderer != null)
                    {
                        Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                        if (colorTable != null)
                        {
                            WindowsAPI.SetBkMode(m.WParam, BackgroundMode.TRANSPARENT);
                            Color c = Color.White;
                            m.Result =  WindowsAPI.CreateSolidBrush((uint)(c.R | (c.G << 8) | (c.B << 16)));
                            
                            return true;
                        }
                    }
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            bool OnPaint(ref Message m)
            {
                bool bResult = false;

                PAINTINFO pi = new PAINTINFO();

                pi.cb = Control.FromHandle(m.HWnd) as ComboBox;
                if (pi.cb != null)
                {
                    pi.cbi.cbSize = (uint)Marshal.SizeOf(pi.cbi);

                    if (WindowsAPI.GetComboBoxInfo(m.HWnd, ref pi.cbi))
                    {
                        PAINTSTRUCT stPaint = new PAINTSTRUCT();
                        IntPtr hdc = WindowsAPI.BeginPaint(m.HWnd, ref stPaint);

                        if (hdc != IntPtr.Zero)
                        {
                            Rectangle rcClient = pi.cb.ClientRectangle;

                            if (rcClient.Width > 0 && rcClient.Height > 0)
                            {
                                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcClient))
                                {
                                    pi.graphics = bg.Graphics;

                                    DrawBackground(ref pi, ref rcClient);

                                    if (pi.cb.DropDownStyle == ComboBoxStyle.DropDownList)
                                    {
                                        DrawItem(ref pi);
                                    }

                                    if (pi.cbi.stateButton != STATE_SYSTEM_INVISIBLE)
                                    {
                                        DrawButton(ref pi);
                                    }

                                    bg.Render();
                                }
                            }
                        }

                        WindowsAPI.EndPaint(m.HWnd, ref stPaint);
                        bResult = true;
                    }
                }
                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcClient"></param>
            void DrawBackground(ref PAINTINFO pi, ref Rectangle rcClient)
            {
                Color bkColorc = GetIsHighlighted(pi.cb) ? SystemColors.Window : this.BackgroundColor;

                using (Brush brush = new SolidBrush(bkColorc))
                {
                    pi.graphics.FillRectangle(brush, rcClient);
                }

                Office2013ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2013ToolStripRenderer;
                
                if (parentRenderer != null)
                {
                    Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;
                    if (SystemInformation.HighContrast)
                    {
                        using (Pen pen = new Pen(SystemColors.MenuHighlight))
                        {                         
                            pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                        {
                            pi.graphics.DrawRectangle(pen, rcClient.X, rcClient.Y, rcClient.Width - 1, rcClient.Height - 1);
                        }
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            private void DrawItem(ref PAINTINFO pi)
            {
                ComboBox cb = pi.cb;

                object oSelected = cb.SelectedItem;
                if (oSelected != null)
                {
                    Graphics g = pi.graphics;
                    string sText = cb.Text;

                    Rectangle rc = (Rectangle)pi.cbi.rcItem;
                    Rectangle rcText = Rectangle.Inflate(rc, -1, -1);
                    TextFormatFlags tf = cb.RightToLeft == RightToLeft.Yes ? TextFormatFlags.NoPadding | TextFormatFlags.Right : TextFormatFlags.NoPadding;

                    if (cb.Focused && !cb.DroppedDown)
                    {
                        g.FillRectangle(SystemBrushes.Highlight, rcText);
                        TextRenderer.DrawText(g, sText, cb.Font, rcText, SystemColors.HighlightText, tf);

                        ControlPaint.DrawFocusRectangle(g, rc);
                    }
                    else
                    {
                        TextRenderer.DrawText(g, sText, cb.Font, rcText, cb.ForeColor, tf);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            void DrawButton(ref PAINTINFO pi)
            {
                Rectangle rcButton = (Rectangle)pi.cbi.rcButton;

                if (rcButton.Width > BUTTON_WIDTH)
                {
                    if (pi.cb.RightToLeft != RightToLeft.Yes)
                    {
                        rcButton.X += rcButton.Width - BUTTON_WIDTH;
                    }
                    rcButton.Width = BUTTON_WIDTH;
                }

                if (!DrawButtonPressed(ref pi, ref rcButton))
                {
                    if (!DrawButtonSelected(ref pi, ref rcButton))
                    {
                        DrawButtonNormal(ref pi, ref rcButton);
                    }
                }
                DrawArrow(ref pi, ref rcButton);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            /// <returns></returns>
            bool DrawButtonPressed(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                bool bResult = false;

                if (GetIsPressed(pi.cb))
                {
                    if (rcButton.Width > 0 && rcButton.Height > 0)
                    {
                        Office2013ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2013ToolStripRenderer;
                        if (parentRenderer != null)
                        {
                            Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                            Graphics g = pi.graphics;

                            using (Brush brush = new SolidBrush(ControlPaint.Light(MenuColor)))
                            {
                                g.FillRectangle(brush, rcButton);
                            }

                            using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                            {
                                g.DrawRectangle(pen, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
                            }
                        }
                    }
                    bResult = true;
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            /// <returns></returns>
            private bool DrawButtonSelected(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                bool bResult = false;

                if (GetIsSelected(pi.cb))
                {
                    if (!(pi.cb.TopLevelControl is Form) || rcButton.Width > 0 && rcButton.Height > 0 && pi.cb.TopLevelControl.ContainsFocus)
                    {
                        Office2013ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2013ToolStripRenderer;
                        if (parentRenderer != null)
                        {
                            Graphics g = pi.graphics;

                            Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                            using (Brush brush = new SolidBrush (ControlPaint.LightLight(MenuColor)))
                            {
                                g.FillRectangle(brush, Rectangle.Inflate(rcButton, -1, -1));
                            }

                            using (Pen pen = new Pen(colorTable.ComboBoxBorderColor))
                            {
                                g.DrawRectangle(pen, rcButton.X, rcButton.Y, rcButton.Width - 1, rcButton.Height - 1);
                            }
                        }
                    }

                    bResult = true;
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            void DrawButtonNormal(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                return;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pi"></param>
            /// <param name="rcButton"></param>
            void DrawArrow(ref PAINTINFO pi, ref Rectangle rcButton)
            {
                int x = rcButton.X + rcButton.Width / 2;
                int y = rcButton.Y + rcButton.Height / 2;

                Point[] points = new Point[]
				{
					new Point(x - 2, y - 1), 
					new Point(x + 3, y - 1),
					new Point(x, y + 2)
				};

                Brush brush =pi.cb.Enabled ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
                if (SystemInformation.HighContrast)
                    brush = SystemBrushes.Highlight;
                pi.graphics.FillPolygon(brush, points);
            }

            /// <summary>
            /// 
            /// </summary>
            bool GetIsHighlighted(ComboBox cb)
            {
                if (cb != null && cb.Enabled)
                {
                    return GetIsSelected(cb) || GetIsPressed(cb);
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private bool GetIsSelected(ComboBox cb)
            {
                bool bResult = false;

                if (cb != null)
                {
                    bResult = cb.Focused;
                    if (!bResult)
                    {
                        Rectangle rc = cb.RectangleToScreen(cb.ClientRectangle);
                        bResult = rc.Contains(Cursor.Position);
                    }
                }

                return bResult;
            }
            private Color menuColor = Color.Blue;
            internal Color MenuColor
            {
                get
                {
                    return menuColor;
                }
                set
                {
                    if (menuColor != value)
                    {
                        menuColor = value;
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="cb"></param>
            /// <returns></returns>
            private bool GetIsPressed(ComboBox cb)
            {
                if (cb != null)
                {
                    return cb.DroppedDown;
                }
                return false;
            }
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            Color BackgroundColor
            {
                get
                {
                    Office2013ToolStripRenderer parentRenderer = m_parentRenderer.Target as Office2013ToolStripRenderer;
                    if (parentRenderer != null)
                    {
                        Office2010ColorTable colorTable = parentRenderer.ColorTable as Office2010ColorTable;

                        if (colorTable != null)
                        {
                            Color color = Color.White;
                            if (SystemInformation.HighContrast)
                                color = Color.Black;
                            return color;
                        }
                    }
                    return SystemColors.Window;
                }
            }
            #endregion

            #region Fields
            WeakReference m_parentRenderer;
            IntPtr m_hHoverWindow;
            #endregion
        }
        #endregion
    }
    #endregion
}
#endif
