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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	#region Office12ToolStripRenderer
	/// <summary>
	/// 
	/// </summary>
	public partial class Office12ToolStripRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
	{
		#region *** CToolStripWindow
		/// <summary>
		/// 
		/// </summary>
		internal class CToolStripWindow : NativeWindowBase
		{
			#region Constants
			const RedrawWindowFlags rdfCaption = RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_ALLCHILDREN;
			#endregion

			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="toolStrip"></param>
			public CToolStripWindow(ToolStrip toolStrip)
				: base()
			{
				m_toolStrip = toolStrip;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="bDisposing"></param>
			protected override void Dispose(bool bDisposing)
			{
				if (bDisposing)
				{
					m_toolStrip = null;
				}
				base.Dispose(bDisposing);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			public override bool ProcessMessage(ref Message m)
			{
				bool bProcessed = false;

				switch (m.Msg)
				{
					case (int)Msg.WM_NCCALCSIZE:
						bProcessed = OnNcCalcSize(ref m);
						break;
					case (int)Msg.WM_NCPAINT:
						bProcessed = OnNcPaint(ref m);
						break;
					case (int)Msg.WM_MOUSEMOVE:
					case (int)Msg.WM_NCMOUSEMOVE:
					case (int)UtilMsg.WM_HOVERITEM:
						bProcessed = OnMouseMove(ref m);
						break;
					case (int)Msg.WM_MOUSELEAVE:
					case (int)UtilMsg.WM_LEAVEITEM:
						bProcessed = OnMouseLeave(ref m);
						break;
					case (int)Msg.WM_NCHITTEST:
						bProcessed = OnNcHitTest(ref m);
						break;
				}

				return bProcessed || base.ProcessMessage(ref m);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			public override void PostProcessMessage(ref Message m)
			{
				switch (m.Msg)
				{
					case (int)Msg.WM_SIZE:
						OnSize(ref m);
						break;
				}
				base.PostProcessMessage(ref m);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected virtual bool OnNcCalcSize(ref Message m)
			{
				bool bResult = false;

				if (m_toolStrip != null)
				{
                    ERENDERTYPE renderType = ERENDERTYPE.Normal;
                    RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

					if (m_toolStrip.Renderer is Office12ToolStripRenderer)
					{
                        renderType = (m_toolStrip.Renderer as Office12ToolStripRenderer).RenderType;
                        ToolStripRendererUtils.ApplyBorders(m_toolStrip, ref rc);
                    }
                    else if (m_toolStrip is IToolStripExSupport2)
                    {
                        if ((m_toolStrip as IToolStripExSupport2).BorderStyle != ToolStripBorderStyle.None)
                            rc.right -= 3;
                    }

                    ToolStripRendererUtils.ApplyCaption(m_toolStrip, ref rc, renderType);

                    Marshal.StructureToPtr(rc, m.LParam, false);
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected virtual bool OnNcPaint(ref Message m)
			{
				bool bResult = false;

				if (m_toolStrip != null)
				{
                    if (m_toolStrip.Renderer is Office12ToolStripRenderer)
                    {
                        (m_toolStrip.Renderer as Office12ToolStripRenderer).DrawBorders(m_toolStrip);
                        (m_toolStrip.Renderer as Office12ToolStripRenderer).DrawCaption(m_toolStrip);
                    }
                    else if (m_toolStrip.Renderer is Office2010ToolStripRenderer)
                    {
                        (m_toolStrip.Renderer as Office2010ToolStripRenderer).DrawBorders(m_toolStrip);
                        (m_toolStrip.Renderer as Office2010ToolStripRenderer).DrawCaption(m_toolStrip);
                    }
                    else if (m_toolStrip.Renderer is Office2013ToolStripRenderer)
                    {
                        (m_toolStrip.Renderer as Office2013ToolStripRenderer).DrawBorders(m_toolStrip);
                        (m_toolStrip.Renderer as Office2013ToolStripRenderer).DrawCaption(m_toolStrip);
                    }
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected virtual bool OnMouseMove(ref Message m)
			{
				if (!m_bSelected && !this.DesignMode)
				{
					m_bSelected = true;
					WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, rdfCaption);
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected virtual bool OnMouseLeave(ref Message m)
			{
				if (m_bSelected && !this.DesignMode)
				{
					Win32API.POINT pt = new Win32API.POINT();
					Win32API.WindowsAPI.GetCursorPos(ref pt);

					Rectangle rc = m_toolStrip.RectangleToScreen(m_toolStrip.ClientRectangle);

					if (!rc.Contains(pt.x, pt.y))
					{
						m_bSelected = false;
						WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, rdfCaption);
					}
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			/// <returns></returns>
			protected virtual bool OnNcHitTest(ref Message m)
			{
				bool bResult = false;
				if (this.HasCaption)
				{
					Office12ToolStripRenderer renderer = m_toolStrip.Renderer as Office12ToolStripRenderer;
					ERENDERTYPE renderType = ERENDERTYPE.Normal;

					if (renderer != null)
						renderType = renderer.RenderType;

					int xPos = WindowsAPI.GET_X_LPARAM((int)m.LParam);
					int yPos = WindowsAPI.GET_Y_LPARAM((int)m.LParam);

					Rectangle rc = ToolStripRendererUtils.GetCaptionBounds(m_toolStrip, renderType);

					if (rc.Contains(xPos, yPos))
					{
						m.Result = new IntPtr((int)HitTest.HTCLIENT);
						bResult = true;
					}
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected virtual void OnSize(ref Message m)
			{
				m_toolStrip.Region = ToolStripRendererUtils.GetRegion(m_toolStrip);
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public bool Selected
			{
				get
				{
					return m_bSelected;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private bool HasCaption
			{
				get
				{
					bool bResult = false;

					if (m_toolStrip != null)
					{
						Office12ToolStripRenderer renderer = m_toolStrip.Renderer as Office12ToolStripRenderer;
						ERENDERTYPE renderType = ERENDERTYPE.Normal;

						if (renderer != null)
							renderType = renderer.RenderType;

						bResult = ToolStripRendererUtils.HasCaption(m_toolStrip, renderType);
					}

					return bResult;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private bool DesignMode
			{
				get
				{
					if (m_toolStrip != null && m_toolStrip.Site != null)
					{
						return m_toolStrip.Site.DesignMode;
					}
					return false;
				}
			}
			#endregion

			#region Fields
			ToolStrip m_toolStrip;
			private bool m_bSelected = false;
			#endregion
		}
		#endregion
	}
	#endregion
    #region MetroToolStripRenderer
    /// <summary>
    /// 
    /// </summary>
    public partial class MetroToolStripRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
    {
        #region *** CToolStripWindow
        /// <summary>
        /// 
        /// </summary>
        internal class CToolStripWindow : NativeWindowBase
        {
            #region Constants
            const RedrawWindowFlags rdfCaption = RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_ALLCHILDREN;
            #endregion

            #region Constructors
            /// <summary>
            /// 
            /// </summary>
            /// <param name="toolStrip"></param>
            public CToolStripWindow(ToolStrip toolStrip)
                : base()
            {
                m_toolStrip = toolStrip;
            }
            #endregion

            #region Overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bDisposing"></param>
            protected override void Dispose(bool bDisposing)
            {
                if (bDisposing)
                {
                    m_toolStrip = null;
                }
                base.Dispose(bDisposing);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            public override bool ProcessMessage(ref Message m)
            {
                bool bProcessed = false;

                switch (m.Msg)
                {
                    case (int)Msg.WM_NCCALCSIZE:
                        bProcessed = OnNcCalcSize(ref m);
                        break;
                    case (int)Msg.WM_NCPAINT:
                        bProcessed = OnNcPaint(ref m);
                        break;
                    case (int)Msg.WM_MOUSEMOVE:
                    case (int)Msg.WM_NCMOUSEMOVE:
                    case (int)UtilMsg.WM_HOVERITEM:
                        bProcessed = OnMouseMove(ref m);
                        break;
                    case (int)Msg.WM_MOUSELEAVE:
                    case (int)UtilMsg.WM_LEAVEITEM:
                        bProcessed = OnMouseLeave(ref m);
                        break;
                    case (int)Msg.WM_NCHITTEST:
                        bProcessed = OnNcHitTest(ref m);
                        break;
                }

                return bProcessed || base.ProcessMessage(ref m);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            public override void PostProcessMessage(ref Message m)
            {
                switch (m.Msg)
                {
                    case (int)Msg.WM_SIZE:
                        OnSize(ref m);
                        break;
                }
                base.PostProcessMessage(ref m);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            protected virtual bool OnNcCalcSize(ref Message m)
            {
                bool bResult = false;

                if (m_toolStrip != null)
                {
                    ERENDERTYPE renderType = ERENDERTYPE.Normal;
                    RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

                    if (m_toolStrip.Renderer is MetroToolStripRenderer)
                    {
                        renderType = (m_toolStrip.Renderer as MetroToolStripRenderer).RenderType;
                        ToolStripRendererUtils.ApplyBorders(m_toolStrip, ref rc);
                    }
                    else if (m_toolStrip is IToolStripExSupport2)
                    {
                        if ((m_toolStrip as IToolStripExSupport2).BorderStyle != ToolStripBorderStyle.None)
                            rc.right -= 3;
                    }

                    ToolStripRendererUtils.ApplyCaption(m_toolStrip, ref rc, renderType);

                    Marshal.StructureToPtr(rc, m.LParam, false);
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            protected virtual bool OnNcPaint(ref Message m)
            {
                bool bResult = false;

                if (m_toolStrip != null)
                {
                    if (m_toolStrip.Renderer is MetroToolStripRenderer)
                    {
                        (m_toolStrip.Renderer as MetroToolStripRenderer).DrawBorders(m_toolStrip);
                        (m_toolStrip.Renderer as MetroToolStripRenderer).DrawCaption(m_toolStrip);
                    }
                    else if (m_toolStrip.Renderer is MetroToolStripRenderer)
                    {
                        (m_toolStrip.Renderer as MetroToolStripRenderer).DrawBorders(m_toolStrip);
                        (m_toolStrip.Renderer as MetroToolStripRenderer).DrawCaption(m_toolStrip);
                    }
                }

                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            protected virtual bool OnMouseMove(ref Message m)
            {
                if (!m_bSelected && !this.DesignMode)
                {
                    m_bSelected = true;
                    WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, rdfCaption);
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            protected virtual bool OnMouseLeave(ref Message m)
            {
                if (m_bSelected && !this.DesignMode)
                {
                    Win32API.POINT pt = new Win32API.POINT();
                    Win32API.WindowsAPI.GetCursorPos(ref pt);

                    Rectangle rc = m_toolStrip.RectangleToScreen(m_toolStrip.ClientRectangle);

                    if (!rc.Contains(pt.x, pt.y))
                    {
                        m_bSelected = false;
                        WindowsAPI.RedrawWindow(m.HWnd, IntPtr.Zero, IntPtr.Zero, rdfCaption);
                    }
                }
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            protected virtual bool OnNcHitTest(ref Message m)
            {
                bool bResult = false;
                if (this.HasCaption)
                {
                    MetroToolStripRenderer renderer = m_toolStrip.Renderer as MetroToolStripRenderer;
                    ERENDERTYPE renderType = ERENDERTYPE.Normal;

                    if (renderer != null)
                        renderType = renderer.RenderType;

                    int xPos = WindowsAPI.GET_X_LPARAM((int)m.LParam);
                    int yPos = WindowsAPI.GET_Y_LPARAM((int)m.LParam);

                    Rectangle rc = ToolStripRendererUtils.GetCaptionBounds(m_toolStrip, renderType);

                    if (rc.Contains(xPos, yPos))
                    {
                        m.Result = new IntPtr((int)HitTest.HTCLIENT);
                        bResult = true;
                    }
                }
                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            protected virtual void OnSize(ref Message m)
            {
                m_toolStrip.Region = ToolStripRendererUtils.GetRegion(m_toolStrip);
            }
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            public bool Selected
            {
                get
                {
                    return m_bSelected;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private bool HasCaption
            {
                get
                {
                    bool bResult = false;

                    if (m_toolStrip != null)
                    {
                        MetroToolStripRenderer renderer = m_toolStrip.Renderer as MetroToolStripRenderer;
                        ERENDERTYPE renderType = ERENDERTYPE.Normal;

                        if (renderer != null)
                            renderType = renderer.RenderType;

                        bResult = ToolStripRendererUtils.HasCaption(m_toolStrip, renderType);
                    }

                    return bResult;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private bool DesignMode
            {
                get
                {
                    if (m_toolStrip != null && m_toolStrip.Site != null)
                    {
                        return m_toolStrip.Site.DesignMode;
                    }
                    return false;
                }
            }
            #endregion

            #region Fields
            ToolStrip m_toolStrip;
            private bool m_bSelected = false;
            #endregion
        }
        #endregion
    }
    #endregion
}
#endif