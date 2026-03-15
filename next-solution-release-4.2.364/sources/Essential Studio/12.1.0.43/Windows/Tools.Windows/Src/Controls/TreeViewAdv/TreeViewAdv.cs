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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel.Design;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
using System.Drawing.Design;

using Syncfusion.Styles;
using System.Diagnostics;
using System.Collections.Generic;
using Syncfusion.Windows.Forms.Tools.Controls;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
    #region TreeStyle

    public enum TreeStyle
    {
        Default,
        Metro,
        Office2007,
        Office2010
    }

    #endregion

    #region TreeViewSearchFunctions

    /// <summary>
    /// Enum for specifying the options to find and replace in TreeView.
    /// </summary>
    [Flags]
    public enum TreeViewSearchOption
    {
        MatchWholeText = 0,
        MatchCase = 1,
    }

    /// <summary>
    /// Enum for specifying the levels of range to find and replace in TreeView.
    /// </summary>
    public enum TreeViewSearchRange
    {
        TreeView,
        RootNode,
        ChildNode
    }

    /// <summary>
    /// Enum for specifying the navigation style to find and replace in TreeView.
    /// </summary>
    public enum TreeViewSearchNavigation
    {
        SearchUp,
        SearchDown,
        SearchAll
    }

    #endregion

    /// <summary>
	/// The TreeViewAdv control is an advanced tree control that surpasses the functionality and look of the standard TreeView control.
	/// </summary>
	[ToolboxItem(true),
	Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.TreeViewAdvDesigner)),
	ToolboxBitmap(typeof(TreeViewAdv), "ToolboxIcons.TreeViewAdv.bmp"),
	DefaultProperty("Nodes"),
    Description("Advanced tree control that surpasses the functionality and look of the standard TreeView control.")
	]
	public class TreeViewAdv : ScrollControl,
		ISupportInitialize,
		IThemedControl,
		INonClientPaintingSupport,
		IProvideCustomContextMenuPositionalInformation,
		IDragDispatcher,
        ISuppportHistory, IVisualStyle 
	{
		#region GradientPanel

		private Border3DSide borderSides = Border3DSide.All;
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		private BorderStyle borderStyle = BorderStyle.Fixed3D;
		private ButtonBorderStyle borderSingle = ButtonBorderStyle.Solid;
		private Color borderColor = Color.Black;
		private ControlDrawing themedDrawing;
		private ThemedControlDrawing themedEditDrawing;
		private bool themedBorder = true;
		private bool ignoreThemeBackground = false;
		private Syncfusion.Windows.Forms.ToolTipAdv helpText;
		private bool ignoreNextMouseMove = false;
        private Timer timer;
        private TreeNodeAdv  toolTipNode = null;
        internal TreeNodeAdv OptionedNode = null;
		internal ITreeNodeAdvPaintFilter paintFilter = null;
		/// <summary>
		/// Indicates whether mouse has left out of control.
		/// </summary>
		private bool m_bMouseLeaved = true;
		/// <summary>
		/// Nodes needed to be highlighted for selecting child.
		/// </summary>
		private Hashtable m_hashHighlightedNodes = new Hashtable();
		/// <summary>
		/// Node which must be highlighted like parent for dragging.
		/// </summary>
		private TreeNodeAdv m_highlightedNode;
		/// <summary>
		/// Stub variable which indicates whether artificial drag-and-drop works
		/// when AllowDrop is set to false.
		/// </summary>
		private bool		m_bAllowDropStubWorks;

		/// <summary>
		/// Indicates whether control must draw dotted rectangle around 
		/// selected node when it has no focus.
		/// </summary>
		private bool m_bKeepDottedSelection = true;
		/// <summary>
		/// Indicates whether cue image should be drawn 
		/// at a distance below the mouse cursor while dragging. 
		/// </summary>
		private bool m_bKeepDragCapturePoint = false;

		/// <summary>
		/// Default image index.
		/// </summary>
		internal const int DEF_DEFAULT_IMAGE_INDEX = -1;

		/// <summary>
		/// ImageList with images that are displayed 
		/// instead of expand/collapse button.
		/// </summary>
		private ImageList m_nodeStateImageList = null;

		/// <summary>
		/// Index of default image for collapse button.
		/// </summary>
		private int m_defaultCollapseImageIndex = DEF_DEFAULT_IMAGE_INDEX;

		/// <summary>
		/// Index of default image for expand button.
		/// </summary>
		private int m_defaultExpandImageIndex = DEF_DEFAULT_IMAGE_INDEX;

        /// <summary>
        /// Delays the tooltip
        /// </summary>
        private bool delayToolTip = false;

        /// <summary>
        /// Delays the tooltip by one second
        /// </summary>
        [Browsable(true)
       , Category("Behavior")
       , Description("Delays the tooltip by one second")]
        [DefaultValue(false)]
        public bool DelayToolTip
        {
            get
            {
                return delayToolTip;
            }
            set
            {
                this.delayToolTip = value;
            }
        }

		/// <summary>
		/// Indicates whether control must draw dotted rectangle around 
		/// selected node when it has no focus.
		/// </summary>
		[ Browsable( true )
		, Category( "Appearance" )
		, Description( "Gets or sets value which indicates if control must draw dotted rectangle around  selected node then it has no focus." ) ]
		[DefaultValue(true)]
		public bool KeepDottedSelection
		{
			get
			{
				return m_bKeepDottedSelection;
			}
			set
			{
				m_bKeepDottedSelection = value;
			}
		}
		/// <summary>
		/// Indicates whether cue image should be drawn 
		/// at a distance below the mouse cursor while dragging. 
		/// </summary>
		[ Browsable( true )
		, Category( "Appearance" )
		, Description( "Gets or sets value which indicates if cue image should be drawn at a distance below the mouse cursor while draging or not. " ) ]
		[DefaultValue(false)]
		public bool KeepDragCapturePoint
		{
			get
			{
				return m_bKeepDragCapturePoint;
			}
			set
			{
				m_bKeepDragCapturePoint = value;
				this.TreeNodeDragHelper.DragWindow.DragParent = ( value ) ? this : null;
			}
		}
        /// <summary>
        /// Gets or sets a value indicating whether sort treeview including all the child nodes.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if sort all with child nodes; otherwise, <c>false</c>.
        /// </value>
        /// 
		/// <example>This example describes how to sort all the nodes in the TreeViewAdv
        /// <code language="C#">
        /// If SortWithChildNodes property is set to true,the user can sort all the nodes including all the child nodes in the treeViewAdv.
        /// The SortOrder of the Root should be specified for the sorting all nodes.
        /// //Sorts only the root nodes.
        /// private void button1_Click(object sender, System.EventArgs e)
        /// {
        /// this.treeViewAdv1.Nodes.Sort();
        /// }
        /// //Sort all the root nodes and the child nodes in the TreeviewAdv
        /// private void button2_Click_1(object sender, System.EventArgs e)
        /// {
        /// this.treeViewAdv1.Root.SortOrder=SortOrder.Ascending;
        /// this.treeViewAdv1.SortWithChildNodes=true;
        /// this.treeViewAdv1.Root.Sort();
        /// }
        /// </code>
		/// <code language="VB">
		/// 'Sorts only the root nodes.
		/// Private Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
	    /// Me.treeViewAdv1.Nodes.Sort()
		/// End Sub
		/// 'Sort all the root nodes and the child nodes in the TreeviewAdv
		/// Private Sub button2_Click_1(ByVal sender As Object, ByVal e As System.EventArgs)
		/// Me.treeViewAdv1.Root.SortOrder=SortOrder.Ascending
		/// Me.treeViewAdv1.SortWithChildNodes=True
		/// Me.treeViewAdv1.Root.Sort()
		/// End Sub
		/// </code>
		/// </example>
       [Browsable(true),
        Description("This will sort all the nodes including child nodes."),
        Category("Behavior")]
        [DefaultValue(false)]
        public bool SortWithChildNodes
        {
            get
            {
                return sortWithChildNodes;
            }
            set
            {
                sortWithChildNodes = value;
            }
        }

        /// <summary>
        /// Occurs when the tree's BorderStyle is changed
        /// </summary>
		[Category("Property Changed")]
       [Description("Occurs when the tree's BorderStyle is changed.")]
		public event EventHandler BorderStyleChanged;
        /// <summary>
        /// Occurs when the tree's Border3DStyle is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's Border3DStyle is changed.")]
		public event EventHandler Border3DStyleChanged;
        /// <summary>
        /// Occurs when the tree's Border2DStyle is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's Border2DStyle is changed.")]
		public event EventHandler BorderSingleChanged;
        /// <summary>
        /// Occurs when the tree's BorderColor is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's BorderColor is changed.")]
		public event EventHandler BorderColorChanged;
        /// <summary>
        /// Occurs when the tree's GradientBackground is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's GradientBackground is changed.")]
		public event EventHandler GradientBackgroundChanged;
        /// <summary>
        /// Occurs when the tree's VerticalGradient is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's VerticalGradient is changed.")]
		public event EventHandler VerticalGradientChanged;
        /// <summary>
        /// Occurs when the tree's GradientColors is changed 
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's GradientColors is changed.")]
		public event EventHandler GradientColorsChanged;
        /// <summary>
        /// Occurs when the tree's BorderSides is changed
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the tree's BorderSides is changed.")]
		public event EventHandler BorderSidesChanged;
        /// <summary>
        /// Occurs when the ThemesEnabled property changes
        /// </summary>
		[Category("Property Changed")]
        [Description("Occurs when the ThemesEnabled property changes.")]
		public event EventHandler ThemeChanged;

		private IntPtr cachedRgn = IntPtr.Zero;

		IntPtr INonClientPaintingSupport.NonClientPaint(PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen)
		{
			Graphics g = e.Graphics;
			Rectangle bounds = displayRect;

			// This is not good for the following reasons:
			// 1) When dragging a hidden tree into the visible desktop range, clipping
			// is not proper and as a result the BG is drawn over the whole control and stays there!
			// 2) Even in other scenarios, we can see the BG color being drawn first
			// followed by the tree. Causing a flicker effect.
			//
			// So, instead fill the bg only in the border area.
			// 
			// Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
			// g.FillRectangle(new SolidBrush(this.BackColor),bounds);

			int w = this.borderStyle == BorderStyle.Fixed3D ? 2 : 1;
			if (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed && this.themedBorder)
				w = 1;
			if (borderStyle == BorderStyle.None)
				w = 0;

			// The borders as 4 rectangles
			Rectangle[] clipRects = new Rectangle[]
			            {
			                new Rectangle(bounds.Location,new Size(w,bounds.Height)),
			                new Rectangle(bounds.Location,new Size(bounds.Width,w)),
			                new Rectangle(bounds.Width-w,bounds.Y,w,bounds.Height),
			                new Rectangle(bounds.X,bounds.Height-w,bounds.Width,w)
			            };

			if (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed && this.themedBorder)
			{
				for (int i = 0; i < 4; i++)
				{
					ThemedEditDrawing.DrawThemeBackground(g, 1, 1, bounds, clipRects[i]);
				}
			}
			else
			{
				Brush backBrush = new SolidBrush(this.BackColor);
				// Fill the border-rectangles with the bg brush, since some of the 
				// 3d border types are only 1 pixel wide.
				for (int i = 0; i < 4; i++)
				{
					g.FillRectangle(backBrush, clipRects[i]);
				}
				backBrush.Dispose();

				if (this.BorderSides != Border3DSide.All)
				{
					if (this.BorderSides != Border3DSide.Middle)
						ThemedDrawing.DrawBorder(g, bounds, this.borderStyle, this.border3DStyle, this.borderSingle, this.borderColor, this.borderSides);
				}
				else
					ThemedDrawing.DrawBorder(g, bounds, this.borderStyle, this.border3DStyle, this.borderSingle, this.borderColor);
			}

			using (NCPaintEventArgs ncEventArgs = new NCPaintEventArgs(e.Graphics, e.ClipRectangle, displayRect, windowRectInScreen, IntPtr.Zero))
			{
				this.OnNCPaint(ncEventArgs);
			}

			// return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn(windowRectInScreen.Left + w, windowRectInScreen.Top + w, windowRectInScreen.Right - w, windowRectInScreen.Bottom - w);
		}

		/// </override>
		protected override void WndProc( ref Message m )
        {
			if( this.UpdatingStyles && m.Msg != NativeMethods.WM_NCPAINT )
			{
				base.WndProc( ref m );
				return;
			}

            Point p = this.PointToClient( new Point( MousePosition.X, MousePosition.Y ) );
            if( m.Msg == NativeMethods.WM_MOUSEACTIVATE && this.CanFocus && this.Visible )
            {
                if( !this.labelEditor.Bounds.Contains( p ) || !this.IsEditing )
                {
                    bool bAllowFocus = true;
                    
                    foreach( DictionaryEntry element in this.CustomControlCollection )
                    {
                        Control control = (Control)element.Key;
                        if( control.Visible && control.Bounds.Contains( p ) )
                        {
                            bAllowFocus = false;
                            break;
                        }
                    }

                    IContainerControl icc = this.GetContainerControl();
                    bAllowFocus = bAllowFocus && ( icc != null
						? ( icc is Form || icc.ActiveControl == null || icc.ActiveControl == this || this.Controls.Contains( icc.ActiveControl ) )
						: true );
                    
                    if( bAllowFocus )
                    {
                        this.Focus();
                        if (!this.Focused)
                            focusFailedOnValidation = true;
                    }
                }
            }

            if( this.cachedRgn != IntPtr.Zero )
            {
                NativeMethods.DeleteObject( this.cachedRgn );
                this.cachedRgn = IntPtr.Zero;
            }
            if( m.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                this.InvalidateWindow();
            }
            if( m.Msg == Syncfusion.Runtime.InteropServices.NativeMethods.WM_NCPAINT )
            {
                this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref m );
            }
            if( m.Msg == 0x007B /*WM_CONTEXTMENU*/)
            {
                // LParam == -1 means that this is due to a keyboard message
                if( (int)m.LParam == -1 )
                {
                    if( this.WmContextMenuByKey( ref m ) )
                        return;
                }
                else if( this.IsDraggedPastMouseDownPoint( this.PointToClient( Control.MousePosition ) ) )
                    return; // without calling the base class.

            }
            else if( m.Msg == NativeMethods.WM_NCLBUTTONDOWN && this.GetNodeAtPoint( p ) != null )
            {
                if( this.IsEditing )
                {
                    this.EndEdit( false );
                    // If the edit wasn't committed don't let the control process this message.
                    if( this.IsEditing )
                        return;
                }
            }
            else if( m.Msg == NativeMethods.WM_GETDLGCODE )
            {
                m.Result = new IntPtr( (int)( NativeMethods.DialogCodes.DLGC_WANTARROWS ) | m.Result.ToInt32() );
                return;
            }
            else if( m.Msg == NativeMethods.WM_LBUTTONDBLCLK && NativeMethods.MK_LBUTTON == (int)m.WParam )
            {
                Point pt = new Point( NativeMethods.LOWORD( m.LParam ), NativeMethods.HIWORD( m.LParam ) );
                bool bToolTipClicked = CheckToolTipClicked( pt );

                if( bToolTipClicked )
                {
                    m.Result = IntPtr.Zero;

                    this.OnDoubleClick( new EventArgs() );
                    //Fix for the defect MouseDoubleClick event does not trigger when we double click on ToolTip
#if SyncfusionFramework2_0
                    this.OnMouseDoubleClick( new MouseEventArgs( MouseButtons.Left, 2, pt.X, pt.Y, 0 ) );
#endif
                    return;
                }
            }
            else if (this.DesignMode && this.RightToLeft == RightToLeft.Yes
                && m.Msg == NativeMethods.WM_WIN_FORMS_MOUSE_ENTER)
            {
                return;
            }
            base.WndProc( ref m );
        }

		/// <summary>
		/// Implement this interface to support keyboard based (Shift+F10) context menu 
		/// invocation. The context menu will then appear near the selected node.
		/// </summary>
		/// <remarks>The PopupMenu in the XPMenus framework will then call this method to
		/// determine the location for popup.</remarks>
		Point IProvideCustomContextMenuPositionalInformation.GetMenuPositionForKeyboardInvoke()
		{
			Point pt = new Point(0, 0);

			if(this.SelectedNode != null)
			{
				Rectangle selBounds = this.SelectedNode.TextBounds;

				pt.X = Math.Max(selBounds.X, 0);
				pt.Y = Math.Max(selBounds.Y + selBounds.Height / 2, 0);
				
			}
			
			return pt;
		}

		private bool WmContextMenuByKey(ref Message m)
		{
			// (This workaround only helps .Net ContextMenus)

			// If showing context menu by keyboard then
			// override the default behavior (which is to show the menu in the middle of the control)
			// and show the context menu beside the selected node
			if(this.ContextMenu != null && this.SelectedNode != null)
			{
				Point pt = ((IProvideCustomContextMenuPositionalInformation)this).GetMenuPositionForKeyboardInvoke();
				this.RMouseDownNode = this.SelectedNode;
				this.ContextMenu.Show(this, pt);
				this.RMouseDownNode = null;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged(EventArgs e)
		{
			if(this.ThemeChanged != null)
			{
				this.ThemeChanged(this, e);
			}
		} 
		/// </override>
		//		protected override void OnFontChanged(EventArgs e)
		//		{
		//			base.OnFontChanged(e);
		//
		//			this.StandardStyle.Font = this.Font;
		//
		//			this.Root.RecalculateAllDimensions();
		//			this.Invalidate();
		//		}
		//		protected override void OnForeColorChanged(EventArgs e)
		//		{
		//			base.OnForeColorChanged(e);
		//			
		//			this.StandardStyle.TextColor = this.ForeColor;
		//
		//			this.Root.RecalculateAllDimensions();
		//			this.Invalidate();
		//		}
		/// <summary>
		/// Raises the BorderStyleChanged event.
		/// </summary>
		/// <remarks>
		/// <para>The OnBorderStyleChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorderStyleChanged in a derived
		/// class, be sure to call the base class's OnBorderStyleChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderStyleChanged()
		{
			if(BorderStyleChanged!=null){ BorderStyleChanged(this,EventArgs.Empty);}
		}

		/// <summary>
		/// Raises the Border3DStyleChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorder3DStyleChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorder3DStyleChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorder3DStyleChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorder3DStyleChanged()
		{
			if(Border3DStyleChanged!=null){ Border3DStyleChanged(this,EventArgs.Empty);}
		}
		/// <summary>
		/// Raises the BorderSingleChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderSingleChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorderSingleChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderSingleChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderSingleChanged()
		{
			if(BorderSingleChanged!=null){ BorderSingleChanged(this,EventArgs.Empty);}
		}
		/// <summary>
		/// Raises the BorderColorChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderColorChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorderColorChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderColorChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderColorChanged()
		{
			if(BorderColorChanged!=null){ BorderColorChanged(this,EventArgs.Empty);}
		}
		/// <summary>
		/// Raises the GradientBackgroundChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnGradientBackgroundChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnGradientBackgroundChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnGradientBackgroundChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnGradientBackgroundChanged()
		{
			if(GradientBackgroundChanged!=null){ GradientBackgroundChanged(this,EventArgs.Empty);}
		}
		/// <summary>
		/// Raises the VerticalGradientChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnVerticalGradientChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnVerticalGradientChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnVerticalGradientChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnVerticalGradientChanged()
		{
			if(VerticalGradientChanged!=null){ VerticalGradientChanged(this,EventArgs.Empty);}
		}
		
		/// <summary>
		/// Raises the GradientColorsChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnGradientColorsChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnGradientColorsChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnGradientColorsChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnGradientColorsChanged()
		{
			if(GradientColorsChanged!=null){ GradientColorsChanged(this,EventArgs.Empty);}
		}
		/// <summary>
		/// Raises the BorderSidesChanged event.
		/// </summary>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderSidesChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OmBorderSidesChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderSidesChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderSidesChanged()
		{
			if(BorderSidesChanged!=null){ BorderSidesChanged(this,EventArgs.Empty);}
		}

		/// <summary>
		/// Indicates whether the control will ignore the theme's background color and draw 
		/// the <see cref="BackgroundColor"/> instead when <see cref="ThemesEnabled"/> is true.
		/// </summary>
		/// <value>True to ignore theme background; false otherwise. Default is false.</value>
		[Description("This will ignore the theme's background color and draw the BackgroundColor when ThemesEnabled is true.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool IgnoreThemeBackground
		{
			get{return ignoreThemeBackground;}
			set
			{
				if(ignoreThemeBackground!=value)
				{
					ignoreThemeBackground = value;
					Invalidate();
				}
			}
		}

		internal bool ThemedBorder
		{
			get{return themedBorder;}
			set{this.themedBorder = value;}
		}

		/// </override>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cparams;
				BorderStyle border;

				cparams = base.CreateParams;
				cparams.ExStyle = cparams.ExStyle | 65536;
				cparams.ExStyle = cparams.ExStyle & -513;
				cparams.Style = cparams.Style & -8388609;
				border = this.borderStyle;
				if(border == BorderStyle.Fixed3D)
					if(ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed && this.themedBorder)
						border = BorderStyle.FixedSingle;
				switch (border) 
				{
					case BorderStyle.Fixed3D:
						cparams.ExStyle = cparams.ExStyle | 512;
						break;
					case BorderStyle.FixedSingle:
						cparams.Style = cparams.Style | 8388608;
						break;
				}

				cparams.Style = cparams.Style |(int) ControlStyles.AllPaintingInWmPaint | 	(int)ControlStyles.UserPaint| (int)ControlStyles.DoubleBuffer;
				return cparams;
			}

		}


		/// <summary>
		/// Indicates whether the control and it's parts should be drawn themed.
		/// </summary>
		/// <value>True to enable themes; false otherwise. Default is false.</value>
		[Description("Indicates if the control is drawn themed.")]
		[Category("Appearance")]
		public bool ThemesEnabled
		{
			get{return this.StandardStyle.ThemesEnabled;}
			set
			{
				this.StandardStyle.ThemesEnabled = value;
				this.RecreateHandle();
				this.Root.RecalculateAllDimensions();
				Invalidate();
                OnThemeChanged(EventArgs.Empty);
			}
		}

        internal ArrayList m_RemovedCustomControls = new ArrayList();
        internal Hashtable m_ControlBounds = new Hashtable();
        internal Hashtable m_ControlParent = new Hashtable();

		protected void ResetThemesEnabled()
		{
			this.StandardStyle.ResetThemesEnabled();
		}

		protected bool ShouldSerializeThemesEnabled()
		{
			return this.StandardStyle.ShouldSerializeThemesEnabled();
		}
		
		/// <summary>
		/// Gets or sets the border sides of the control that will be drawn.
		/// </summary>
		/// <value>One of the <see cref="System.Windows.Forms.Border3DSide"/> values. Default is Border3DSide.All.</value>
		[Description("Indicates the border sides of the control.")]
		[Category("Appearance")]
		[DefaultValue(Border3DSide.All)]
		public Border3DSide BorderSides
		{
			get{return borderSides;}
			set
			{
				if(borderSides!=value)
				{
					borderSides = value;
					this.OnBorderSidesChanged();
					InvalidateWindow();
				}
			}
		}

		/// <summary>
        /// Gets or sets the background color for the control. (overridden property)
		/// </summary>
		public override Color BackColor 
		{
			get{return base.BackColor;} 
			set{base.BackColor = value;}
		}
        private bool canSelectDisabledNode =false ;
        /// <summary>
        ///  Gets or sets to make disabled node selection
        /// </summary>
        public bool CanSelectDisabledNode
        {
            get { return canSelectDisabledNode; }
            set { canSelectDisabledNode = value ; }
        }


		private bool showFocusRect = true;
        /// <summary>
        ///  Gets or sets the FocusRect of the node
        /// </summary>
		public bool ShowFocusRect
		{
			get
			{
				return showFocusRect;
			}
			set
			{
				showFocusRect = value;
			}
		}
        /// <summary>
        /// Determines if the BackColor property was modified.
        /// </summary>
        /// <returns></returns>
		protected bool ShouldSerializeBackColor()
		{
			if(this.BackColor == SystemColors.Window)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Resets the BackColor property to its default value.
		/// </summary>
		public override void ResetBackColor()
		{
			this.BackColor = SystemColors.Window;
		}

        /// <summary>
        /// Resets the ToolTip property to its default value.
        /// </summary>
        public virtual void ResetToolTip()
        {
            this.toolTip = new Syncfusion.Windows.Forms.ToolTipAdv(this);
            this.toolTip.BackColor = System.Drawing.SystemColors.Info;
            this.toolTip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolTip.Name = "toolTip";
            this.toolTip.TabIndex = 1;
            this.toolTip.Text = "toolTip";
            this.toolTip.Visible = false;
        }

        /// <summary>
        /// Resets the HelpText property to its default value.
        /// </summary>
        public virtual void ResetHelpText()
        {
            this.helpText = new Syncfusion.Windows.Forms.ToolTipAdv(this);
            this.helpText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.helpText.Name = "helpText";
            this.helpText.TabIndex = 0;
            this.helpText.Text = "help text";
            this.helpText.Visible = false;
        }

		/// <summary>
		/// Gets or sets the background color, gradient and other styles. This will override the BackColor setting.
		/// </summary>
		/// <remarks>
		/// The <see cref="TreeViewAdv"/> provides this property to enable specialized
		/// custom gradient backgrounds.
		/// </remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Appearance"),
		Description("Lets you set the background color, gradient, etc. overriding the BackColor setting.")
		]
		public BrushInfo BackgroundColor
		{
			get
			{
				return this.bgBrush;
			}

			set
			{
				if(this.bgBrush != value)
				{
					this.bgBrush = value;
					if(this.IsVerticalGradient)
						this.DisableScrollWindow = true;
					else
						this.DisableScrollWindow = false;
					this.Invalidate();
				}
			}
		}

		protected void ResetBackgroundColor()
		{
			this.BackgroundColor = BrushInfo.Empty;
		}

		protected bool ShouldSerializeBackgroundColor()
		{
			if(this.bgBrush == BrushInfo.Empty)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Gets or sets the color of the 2D border.
		/// </summary>
		[Description("Indicates the color of the 2D border.")]
		[Category("Appearance")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				if( borderColor!=value )
				{
					borderColor = value;
					this.OnBorderColorChanged();

					UpdateNc();
				}
			}
		}

		protected void ResetBorderColor()
		{
			this.BorderColor = Color.Black;
		}

		protected bool ShouldSerializeBorderColor()
		{
			if(this.borderColor == Color.Black)
				return false;
			else
				return true;
		}


		/// <summary>
		/// Gets or sets the 2D border style.
		/// </summary>
		/// <value>One of the <see cref="ButtonBorderStyle"/> values. Default is ButtonBorderStyle.Solid.</value>
		[Description("Indicates the 2D border style.")]
		[Category("Appearance")]
		[DefaultValue(ButtonBorderStyle.Solid)]
		public ButtonBorderStyle BorderSingle
		{
			get{return borderSingle;}
			set
			{
				if(borderSingle!=value)
				{
					borderSingle = value;
					this.OnBorderSingleChanged();
					InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border style of the control.
		/// </summary>
		/// <value>One of the <see cref="BorderStyle"/> values. Default is BorderStyle.Fixed3D.</value>
		[DefaultValue(BorderStyle.Fixed3D)]
		[Description("Indicates the border style of the control.")]
		public new BorderStyle BorderStyle
		{
			get{return borderStyle;}
			set
			{
				base.BorderStyle = BorderStyle.None;
				if(borderStyle !=value)
				{
					BorderStyle baseValue = value;
					// The textbox doesn't give you a 1 pixel border for FixedSingle,
					// so specifying 3D border and then drawing a single border manually.
					if(value == BorderStyle.FixedSingle)
						baseValue = BorderStyle.Fixed3D;
					base.BorderStyle = baseValue;

					borderStyle = value;
					this.UpdateStyles();
					InvalidateWindow();
					this.OnBorderStyleChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets the style of the 3D border.
		/// </summary>
		/// <value>One of the <see cref="Border3DStyle"/> values. Default is Border3DStyle.Sunken.</value>
		[Description("Indicates the style of the 3D border.")]
		[Category("Appearance")]
		[DefaultValue(Border3DStyle.Sunken)]
		public Border3DStyle Border3DStyle
		{
			get{return border3DStyle;}
			set
			{
				if(border3DStyle !=value)
				{
					border3DStyle = value;
					this.OnBorder3DStyleChanged();
					InvalidateWindow();
				}
			}
		}

		//Point pt = Point.Empty;
		//		private void RefreshBrush()
		//		{
		//			if(Height<=0 || Width<=0) return;
		//			if(gradientColors.Count==0)
		//			{
		//				this.gradientBrush = new SolidBrush(BackColor);
		//				return;
		//			}
		//			if(verticalGradient)
		//			{
		//					
		//				bool v = this.VScrollBar.InnerScrollBar !=null && this.VScrollBar.InnerScrollBar.Visible;
		//				pt = new Point(0,Height);
		//				try
		//				{
		//					TreeNodeAdv fnode = this.RowIndexToNode(VScrollBar.Minimum);
		//					TreeNodeAdv lnode = this.RowIndexToNode(VScrollBar.Maximum);
		//					TreeNodeAdv mnode = this.PointToNode(new Point(3,3));
		//					//					pt = new Point(v?this.NodeToPoint(fnode).Y:0,v?this.NodeToPoint(lnode).Y + lnode.Height:Height);
		//					pt = new Point(v?-this.GetHeightOfRows(VScrollBar.Minimum,this.NodeToRowIndex(mnode)):0,
		//						v?this.GetHeightOfRows(VScrollBar.Minimum,VScrollBar.Maximum)+this.ItemHeight*2:Height);
		//				}
		//				catch
		//				{
		//					pt = new Point(0,Height);
		//				}
		//				if( (pt.X == pt.Y)&&(pt.Y == 0)) pt = new Point(0,Height);
		//				gradientBrush= new LinearGradientBrush(
		//					new Point(0, pt.X),
		//					new Point(0, pt.X+pt.Y),
		//					Color.Black,
		//					Color.Black);
		//			}
		//			else
		//			{
		//				Point pt = new Point(HorisontalScroll?-this.HScrollPos:0,HorisontalScroll?this.HScrollBar.Maximum:Width);
		//				gradientBrush= new LinearGradientBrush(
		//					new Point(pt.X/*0*/, 0),
		//					new Point(pt.Y +pt.X/*Width*/,0),
		//					Color.Black,
		//					Color.Black);
		//			}
		//			ColorBlend cb = new ColorBlend(gradientColors.Count);
		//			for(int i=0;i<gradientColors.Count;i++)
		//			{
		//				cb.Colors.SetValue(gradientColors[i],i);
		//				cb.Positions.SetValue((float)i/(gradientColors.Count-1),i);
		//			}
		//
		//			((LinearGradientBrush)gradientBrush).InterpolationColors = cb;
		//		}

		/// </override>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			bool bgPainted = false;
			if( this.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && !this.ignoreThemeBackground )
			{
				int ox = pevent.ClipRectangle.Left;
				int oy = pevent.ClipRectangle.Top;
				int dx = pevent.ClipRectangle.Width;
				int dy = pevent.ClipRectangle.Height;

				if( ( ox != 0 ) || ( oy != 0 ) || ( dx != this.ClientRectangle.Width ) || ( dy != this.ClientRectangle.Height ) )
				{
					bgPainted = this.PaintChildrenBackground( pevent.Graphics, this, pevent.ClipRectangle );
				}
				bgPainted = false;
			}

			if( !bgPainted )
				base.OnPaintBackground(pevent);
		}

		[Documentation.DocumentationExclude()]
		protected virtual void ThemedPaintBackground(System.Drawing.Graphics graphics, Rectangle rect, Rectangle clip)
		{
			ThemedEditDrawing.DrawThemeBackground(graphics,1,Enabled?1:4,rect,clip);
		}
		private bool PaintChildrenBackground(System.Drawing.Graphics graphics, System.Windows.Forms.Control control, Rectangle clipRect)
		{
			foreach (System.Windows.Forms.Control child in control.Controls)
			{
				System.Drawing.Rectangle childBounds = new System.Drawing.Rectangle (child.Location, child.Size);
				childBounds = child.Parent.RectangleToScreen(childBounds);
				childBounds = this.RectangleToClient(childBounds);
						
				if (childBounds.Contains (clipRect))
				{
					if (this.PaintChildrenBackground (graphics, child, clipRect))
					{
						return true;
					}
					
					Rectangle client = this.ClientRectangle;
					client = this.RectangleToScreen(client);
					client = child.RectangleToClient(client);

					clipRect = this.RectangleToScreen(clipRect);
					clipRect = child.RectangleToClient(clipRect);
					this.ThemedPaintBackground (graphics, client, clipRect);
					return true;
				}
			}
			
			return false;
		}
		private bool IsVerticalGradient
		{
			get
			{
				if(this.bgBrush != null
					&& this.bgBrush.Style == BrushStyle.Gradient
					&& this.bgBrush.GradientStyle == GradientStyle.Vertical)
					return true;
				else
					return false;
			}
		}
		private bool IsHorizontalGradient
		{
			get
			{
				if(this.bgBrush != null
					&& this.bgBrush.Style == BrushStyle.Gradient
					&& this.bgBrush.GradientStyle == GradientStyle.Horizontal)
					return true;
				else
					return false;
			}
		}

		[Documentation.DocumentationExclude()]
		protected virtual void DrawBackground(System.Windows.Forms.PaintEventArgs e)
		{
			if( this.ClientRectangle.Height <= 0 || this.ClientRectangle.Width <= 0 )
				return;

			Rectangle rc = this.ClientRectangle;
			
			if(!(ThemesEnabled && XPThemes.IsThemedOS &&  XPThemes.IsThemeActive &&XPThemes.IsAppThemed) || IgnoreThemeBackground)
			{
				if(this.BackgroundColor != BrushInfo.Empty)
				{
					if(this.IsHorizontalGradient)
					{
						int left = HorisontalScroll?-this.HScrollPos:0;
						int top = 0;
                        int width = 0;

                        //int width = HorisontalScroll?
                        //    // Need to add VerticallScroll width (even when VerticallScroll is not visible), 
                        //    // otherwise there is some empty space left out to the right when HorisontalScroll is on.
                        //    this.HScrollBar.Maximum + SystemInformation.VerticalScrollBarWidth
                        //    :this.ClientRectangle.Width;

                        //Fix for the defect #2532 - Increase the width amount while Horizontal Brushing.

                        width = this.HScrollBar.Maximum + this.Width;  

						int height = rc.Height;
						rc = new Rectangle(left, top, width, height);
					}
					BrushPaint.FillRectangle(e.Graphics, rc, this.BackgroundColor);
				}
			}
			else
			{
				ThemedEditDrawing.DrawThemeBackground(e.Graphics, 1, Enabled ? 1 : 4, new Rectangle(-2, -2, this.ClientRectangle.Width + 4, Height + 4));
			}
		}

		private void GradientPanel_BackColorChanged(object sender, System.EventArgs e)
		{
			InvalidateWindow();
		}
		private void InvalidateWindow()
		{
			int redrawFlags = NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW| NativeMethods.RDW_INVALIDATE;
			NativeMethodsHelper.RedrawWindow(this.Handle, redrawFlags);
		}

		#endregion

		#region Internal Properties

		[Syncfusion.Documentation.DocumentationExclude()]
		internal ControlDrawing ThemedDrawing
		{
			get
			{
				if (themedDrawing == null)
					themedDrawing = new ControlDrawing();

				return themedDrawing;
			}
			set { themedDrawing = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal ThemedControlDrawing ThemedEditDrawing
		{
			get
			{
				if (themedEditDrawing == null)
					themedEditDrawing = new ThemedControlDrawing("EDIT");

				return themedEditDrawing;
			}
			set { themedEditDrawing = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal ThemedControlDrawing ThemedButtonDrawing
		{
			get
			{
				if (themedButtonDrawing == null)
					themedButtonDrawing = new ThemedControlDrawing("BUTTON");

				return themedButtonDrawing;
			}
			set { themedButtonDrawing = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal DragHelper TreeNodeDragHelper
		{
			get
			{
				if (treeNodedragHelper == null)
					treeNodedragHelper = new DragHelper();

				return treeNodedragHelper;
			}
			set { treeNodedragHelper = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal ThemedControlDrawing ThemedTreeDrawing
		{
			get
			{
				if (themedButtonDrawing == null)
					themedTreeDrawing = new ThemedControlDrawing("TREEVIEW");

				return themedTreeDrawing;
			}
			set { themedTreeDrawing = value; }
		}

		#endregion

		#region Variables
		/// <summary>
		/// Point to remember last click-point.
		/// NOTE : It will be set at OnMouseDown and null
		///  ( set to Point.Empty ) at OnMouseUp.
		///  ( need to fix issue # 180 )
		/// </summary>
		private Point       m_lastButtonDownPoint = Point.Empty;
		private TreeNodeAdv m_lastButtonDownNode  = null;

		internal static string NodeLevelStyleBaseName = "NodeLevel";
		internal static string BaseStyleBaseName = "BaseStyle";
		private System.ComponentModel.IContainer components;
		private TreeNodeAdv root;
		private int itemHeight = -1;
		private int parentIndent = 19;
		internal static string DefaultBaseStyleName = "Standard";
		private TreeNodeAdv activeNode = null;
		private TreeNodeAdv selectionBaseNode = null;
		private bool mouseBasedSelectionOn = false;
		private ArrayList latestMouseBasedSelectionCollection = null;
		private bool keySearching = true;
		private System.Windows.Forms.Timer keyInputTimer;
		private TextBox labelEditor;
		private string keySearch = "";
        private bool searchFromNextSelectableNode = false;
		private DashStyle lineStyle = DashStyle.Dot;
		private Color lineColor = Color.Gray;
		private bool fullRowSelect = false;
		private bool hideSelection = true;
		private bool preparingDragCueBitmap = false;
		private bool dragging = false;
        internal Dictionary<Control, Point> CustomControlsImage;
		/// <summary>
		/// Collection contains checked nodes in tree
		/// </summary>
		private CheckedNodesColection checkedNodes;
		private SelectedNodesCollection selectedNodes;
		//private int startTick = 0;
		private bool clickedOnSelection = false;
		private Point mouseDownPoint = Point.Empty;
		private Timer labelEditStartTimer;
		private TreeNodeAdv activeNodeForLabelEdit = null;
		private bool canDrag = false;
		private bool hotTracking = false;
        internal bool DisableReplacing = false;
        internal bool DisableFinding = false;
		private bool allowMouseBasedSelection = false;
		private bool showLines = true;
		private bool showRootLines = true;
		private string pathSeparator = "\\";
		private bool printing = false;
		private Syncfusion.Windows.Forms.ToolTipAdv toolTip;
		private bool loadOnDemand = false;
		private bool ownerDrawNodes = false;
		private bool ownerDrawNodesBackground = false;
		private ThemedControlDrawing themedButtonDrawing;
		private ThemedControlDrawing themedTreeDrawing;
		private bool labelEdit = false;
		private bool addSeparatorAtEnd = false;
		private int gutterSpace = 3;
		private Graphics measuringGraphics = null;
		private int nodeCount = 0;
		//private bool multipleNodeSelection = false;
		private TreeSelectionMode selectionMode = TreeSelectionMode.Single;
		private bool ensureVisibleSelectedNode = true;
		private bool transparentControls = false;
		//private bool enableDragDrop = true;
		private ImageList leftImageList;
		private ImageList rightImageList;
		private ImageList stateImageList;
		private bool dragOnText = true;
		private BrushInfo selectedNodeBackground = new BrushInfo(SystemColors.Highlight);
		private BrushInfo inactiveSelectedNodeBackground = new BrushInfo(SystemColors.Control);
		private Color selectedNodeForeColor = SystemColors.HighlightText;
		private Color inactiveSelectedNodeForeColor = SystemColors.ControlText;
		private TreeNodeAdv rmousedownnode = null;
		private TreeNodeAdv lmousedownnode = null;
		internal int curSelectedNodeIndex = -1;
		private BrushInfo bgBrush;
		private DragHelper treeNodedragHelper = null;
		private bool showDragNodeCue = true;
		private Hashtable baseStyles = new Hashtable();
		private TreeNodeAdvStyleInfo boundStyle = null;
		//		private bool changeSelectionOnMouseDown = true;
		private TreeNodeAdv[] draggedTnas;
		private bool broughtIntoView = false;
        //indicates the direction of selection
        private bool selectUpwardDirection = false;	  
        
        //Indicates the currently selected keys.
        private Keys bKeypressed = Keys.None;
		//Indicates the current now which is selected by key pressing
        private TreeNodeAdv m_lastSelectedByKeyBoard = null;
        private bool sortWithChildNodes = false;
        private bool recalculateExpansion = true;
        private bool focusFailedOnValidation = false;
        internal bool inNodeRefresh = false;
        DateTime firstkeypress = new DateTime ();
        DateTime secondkeypress = new DateTime();
        TimeSpan timediff = new TimeSpan ();
        /// <summary> 
        /// Default size of the control 
        /// </summary> 
        private Size CTRLSIZE = default(Size);
        /// <summary>
        /// Default item height
        /// </summary>
        private static int ITMHEIGHT = default(int);
		#endregion

		#region Events

        /// <summary>
        /// This Event will be triggered when TreeNodeAdv match is found based on search string
        /// </summary>
        public event TreeViewOnAfterFindArgs OnNodeAfterFound;

        internal void RaiseNodeAfterFoundEvent(TreeNodeAdv node, string searchText)
        {
            if (this.OnNodeAfterFound != null)
            {
                OnNodeAfterFound(this, new TreeNodeAdvAfterFindArgs(node, searchText));
            }
        }

        /// <summary>
        /// This Event will be triggered once TreeNodeAdv match yet to be found based on search string
        /// </summary>
        public event TreeViewOnBeforeFindArgs OnNodeBeforeFind;

        internal void RaiseNodeBeforeFindEvent(TreeNodeAdv node, string searchText)
        {
            TreeNodeAdvBeforeFindArgs args = new TreeNodeAdvBeforeFindArgs(node, searchText);
            if (this.OnNodeBeforeFind != null)
            {
                OnNodeBeforeFind(this, args);
                DisableFinding = args.Cancel;
            }
        }

        /// <summary>
        /// This Event will be triggered on matched treenode text is being replaced based on search string
        /// </summary>
        public event TreeViewOnReplacingArgs OnNodeReplacing;

        internal void RaiseNodeReplacingEvent(TreeNodeAdv node, string searchText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            TreeNodeAdvOnReplacingArgs args = new TreeNodeAdvOnReplacingArgs(node, searchText, replaceText, searchOption, searchRange);
            if (this.OnNodeReplacing != null)
            {
                OnNodeReplacing(this, args);
                this.DisableReplacing = args.Cancel;
            }
        }

        /// <summary>
        /// This Event will be triggered on matched treenode text after gets replaced based on search string
        /// </summary>
        public event TreeViewOnReplacedArgs OnNodeReplaced;

        internal void RaiseNodeReplacedEvent(TreeNodeAdv node, string searchText, string replaceText)
        {
            if (this.OnNodeReplaced != null)
            {
                OnNodeReplaced(this, new TreeNodeAdvOnReplacedArgs(searchText, replaceText, node));
            }
        }

        /// <summary>
        /// This event will be triggered when mousehover occurs in tree nodes and it returns the particular node details which is currently being pointed from its arguement.
        /// </summary>
        public event TreeViewAdvNodeEventHandler NodeHotTrackChanged;

        protected void RaiseNodeHotTracked()
        {
            if( this.NodeHotTrackChanged != null )
                NodeHotTrackChanged(this, new TreeViewAdvNodeEventArgs(lastNodeOver));
        }

        /// <summary>
        /// This event will be triggered if double click occurs on TreeNodeAdv 
        /// </summary>
        public event TreeNodeAdvMouseClickArgs NodeMouseDoubleClick;
        protected void RaiseNodeDoubleClick(TreeNodeAdv hitnode, MouseButtons button, int clicks, int x, int y, int delta )
        {
            if (this.NodeMouseDoubleClick != null)
            {
                this.NodeMouseDoubleClick(this, new TreeViewAdvMouseClickEventArgs(hitnode, button, clicks, x, y, delta));
            }
        }

        /// <summary>
        /// This event will be triggered if single click occurs on TreeNodeAdv 
        /// </summary>
        public event TreeNodeAdvMouseClickArgs NodeMouseClick;
        protected void RaiseNodeSingleClick(TreeNodeAdv hitnode, MouseButtons button, int clicks, int x, int y, int delta)
        {
            if (this.NodeMouseClick != null)
            {
                this.NodeMouseClick(this, new TreeViewAdvMouseClickEventArgs(hitnode, button, clicks, x, y, delta));
            }
        }

        /// <summary>
		/// Occurs when the user begins a drag of one or more items in the tree view control.
		/// </summary>
		/// <remarks>
		/// <p>The <b>Item</b> property in the argument is an array of TreeViewAdv nodes that
		/// are currently selected.</p>
		/// <p>
		/// You can choose to initiate an ole drag-and-drop operation in this event handler.
		/// </p>
		/// </remarks>
		/// <example>
		/// To initiate an ole drag-drop in this event handler:
		/// <code lang = "C#">
		/// // TreeViewAdv.ItemDrag event listener
		/// private void treeViewAdv1_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		/// {
		/// 	// Begin a drag and drop operation of the selected nodes (or some other data).
		/// 	TreeNodeAdv[] nodes = e.Item as TreeNodeAdv[];
		/// 	DragDropEffects result = this.DoDragDrop(nodes, DragDropEffects.Copy | DragDropEffects.Move);
		/// 	// more app logic based on result...
		/// }
		/// </code>
		/// <code lang = "VB">
		/// ' TreeViewAdv.ItemDrag event listener
		/// Private Sub treeViewAdv1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles treeViewAdv2.ItemDrag
		///		' Begin a drag and drop operation of the selected nodes (or some other data).
		///		Dim nodes As TreeNodeAdv() = CType(e.Item, TreeNodeAdv())
		///		Dim result As DragDropEffects = Me.DoDragDrop(nodes, DragDropEffects.Copy Or DragDropEffects.Move)
		///		' more app logic based on result...
		///	End Sub 'treeViewAdv1_ItemDrag '
		/// </code>
		/// <para>Also take a look at our ..\Tools\Samples\Tree Package\TreeViewAdvDragDrop
		/// sample for more information on how to turn on drag-drop cues.</para>
		/// </example>
		[Description("Occurs when the user begins dragging an item."), Category("Drag Drop")]
		public event ItemDragEventHandler ItemDrag;

		/// <summary>
		/// Occurs when <see cref="NodeStateImageList"/> is changed.
		/// </summary>
		[
		Description( "Occurs when NodeStateImageList is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler NodeStateImageListChanged;

		/// <summary>
		/// Occurs when <see cref="DefaultCollapseImageIndex"/> is changed.
		/// </summary>
		[
		Description( "Occurs when DefaultCollapseImageIndex is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler DefaultCollapseImageIndexChanged;

		/// <summary>
		/// Occurs when <see cref="DefaultExpandImageIndex"/> is changed.
		/// </summary>
		[
		Description( "Occurs when DefaultExpandImageIndex is changed." ),
		Category( "Property Changed" )
		]
		public event EventHandler DefaultExpandImageIndexChanged;
         /// <summary>
         /// Raises the NodeStateImageListChanged event.
         /// </summary>
		protected virtual void OnNodeStateImageListChanged()
		{
			this.Invalidate();

			RaiseNodeStateImageList();
		}
       /// <summary>
       /// Raises the DefaultExpandImageIndexChanged event.
       /// </summary>
		protected virtual void OnDefaultExpandImageIndexChanged()
		{
			this.Invalidate();

			RaiseDefaultExpandImageIndexChanged();
		}
        /// <summary>
        /// Raises the DefaultCollapseImageIndexChanged event.
        /// </summary>
		protected virtual void OnDefaultCollapseImageIndexChanged()
		{
			this.Invalidate();

			RaiseDefaultCollapseImageIndexChanged();
		}

		private void RaiseNodeStateImageList()
		{
			if( NodeStateImageListChanged != null )
			{
				NodeStateImageListChanged( this, EventArgs.Empty );
			}
		}	

		private void RaiseDefaultExpandImageIndexChanged()
		{
			if( DefaultExpandImageIndexChanged != null )
			{
				DefaultExpandImageIndexChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseDefaultCollapseImageIndexChanged()
		{
			if( DefaultCollapseImageIndexChanged != null )
			{
				DefaultCollapseImageIndexChanged( this, EventArgs.Empty );
			}
		}


		#region EDITING
		/// <summary>
		/// Overloaded. Begins the editing of the specified node.
		/// </summary>
		/// <param name="node">The node to edit.</param>
		public void BeginEdit(TreeNodeAdv node)
		{
			if(node != null && node.TreeView == this)
			{
				//this.SelectedNode = node;
				if(this.SetSelectedNode(node, this.selectedNodes, TreeViewAdvAction.Unknown))
				{
					this.ActiveNode = node;
					this.selectionBaseNode = node;
				}
				if(this.ActiveNode == node)
					BeginEdit();
			}
		}
	
		private bool m_needUpdateEditTop = false;

		/// <summary>
		/// Begins the editing of the selected node.
		/// </summary>
		public virtual void BeginEdit()
		{
			if(this.ActiveNode == null)
				return;

			// In case the node was just added then a paint message will set it's bounds.
			this.Update();

			this.SetSelectedNode(this.ActiveNode, this.selectedNodes, TreeViewAdvAction.Unknown);

			if(this.SelectedNode != this.ActiveNode)
				return;

			 //Initiates the LabelEditor for every node's before edit event.
            			this.labelEditor.Text = String.Empty;
			TreeNodeAdvBeforeEditEventArgs args = new TreeNodeAdvBeforeEditEventArgs(activeNode,labelEditor);
			this.OnBeforeEdit(args);
			 //Checks whether the text box value has been changed or not. If it's changed, the changed values should be displayed in Label edit text box.
            if (args.TextBox.Text != String.Empty)
                this.labelEditor.Text = args.TextBox.Text;
            else
                this.labelEditor.Text = args.Node.Text;			

			if( args.Cancel )
				return;

			Rectangle editorBounds =  activeNode.TextBounds;
			if(editorBounds.Width < 50)
				editorBounds.Width = 50;

			this.labelEditor.Bounds = editorBounds;
           
            PrepareEditor();

			this.m_needUpdateEditTop = true;
			this.ShowEditor();
		}

		private void PrepareEditor()
		{
			//Set this before the BeforeEdit event.
			this.lastText = activeNode.Text;
			this.labelEditor.Visible = true;
            this.IsEditEnding = true;
            this.labelEditor.Focus();
            this.IsEditEnding = false;
			this.labelEditor.SelectAll();
		}

		private void ShowEditor()
		{
			this.labelEditor.Tag = activeNode.Text;
			this.labelEditor.Font = activeNode.Font;
			
			m_textSelectionLength = labelEditor.SelectionLength;
			m_selectionStart = labelEditor.SelectionStart;
		}
		/// <summary>
		/// Saves or Cancels the editing of the selected node.
		/// </summary>
		/// <param name="cancel">True to cancel editing; false to save changes.</param>
		public virtual void EndEdit(bool cancel)
		{
			if(!IsEditing || IsEditEnding)
				return;

			bool continueEditing = false;
            if (activeNode != null)
            {
                if (!cancel)
                {
                    TreeNodeAdvCancelableEditEventArgs args = new TreeNodeAdvCancelableEditEventArgs(activeNode, labelEditor.Text);
                    this.OnNodeEditorValidating(args);
                    cancel = args.Cancel;
                    if (cancel)
                        continueEditing = args.ContinueEditing;
                }
                if (cancel)
                {
                    activeNode.Text = (string)labelEditor.Tag;
                }
                else
                {
                    string newText = labelEditor.Text;
                    if (m_historyManager != null && m_bHistoryEnabled && newText != activeNode.Text)
                    {
                        TreeViewCommand cmd = new TreeViewCommand(activeNode, newText);
                        m_historyManager.Do(cmd);
                    }
                    activeNode.Text = newText;
                }
            }
			if(!continueEditing)
			{
				this.HideEditor();
				this.Focus();
			}
			else
			{
				this.PrepareEditor();
				this.ShowEditor();
			}
            if (activeNode != null)
            {
                if (!cancel)
                    this.OnNodeEditorValidated(new TreeNodeAdvEditEventArgs(activeNode, activeNode.Text));
                else if (cancelEdit)
                    this.OnEditCancelled(new TreeNodeAdvEditEventArgs(activeNode, this.labelEditor.Text));
            }
		}
		/// <summary>
		/// Forces the end of the editing of the selected node.
		/// </summary>
		public void EndEdit()
		{
			this.EndEdit(false);			
		}
		/// <summary>
		/// Forces the end of the editing of the selected node.
		/// </summary>
        internal void EndEdit(string text, bool cancel)
        {
            this.labelEditor.Visible = true;
            bool continueEditing = false;
            if (activeNode != null)
            {
                if (!cancel)
                {
                    labelEditor.Text = text;
                    TreeNodeAdvCancelableEditEventArgs args = new TreeNodeAdvCancelableEditEventArgs(activeNode, labelEditor.Text);
                    this.OnNodeEditorValidating(args);
                    cancel = args.Cancel;
                    if (cancel)
                        continueEditing = args.ContinueEditing;
                }
                if (cancel)
                {
                    activeNode.Text = (string)labelEditor.Tag;
                }
                else
                {
                    string newText = labelEditor.Text;
                    if (m_historyManager != null && m_bHistoryEnabled && newText != activeNode.Text)
                    {
                        TreeViewCommand cmd = new TreeViewCommand(activeNode, newText);
                        m_historyManager.Do(cmd);
                    }
                    activeNode.Text = newText;
                }
            }
            if (!continueEditing)
            {
                this.HideEditor();
                this.Focus();
            }
            else
            {
                this.PrepareEditor();
                this.ShowEditor();
            }
            if (activeNode != null)
            {
                if (!cancel)
                    this.OnNodeEditorValidated(new TreeNodeAdvEditEventArgs(activeNode, activeNode.Text));
                else if (cancelEdit)
                    this.OnEditCancelled(new TreeNodeAdvEditEventArgs(activeNode, this.labelEditor.Text));
            }
        }
		bool ignoreLeave = false;
		private void labelEditor_Leave(object sender, System.EventArgs e)
		{
			if(!ignoreLeave)
				EndEdit();
		}
		private void HideEditor()
		{
			this.ignoreLeave = true;
			this.labelEditor.Hide();
			this.ignoreLeave = false;
		}
		/// <summary>
		/// Occurs when the text entered by the user changes in the Node editor control.
		/// </summary>
		[Description("Occurs for each TextChanged event in the node editor control.")
		, Category("Editor Action")]
		public event TreeNodeAdvCancelableEditEventHandler NodeEditorValidateString;

		/// <summary>
		/// Occurs before the newly entered text in the Node editor gets stored.
		/// </summary>
		[Description("Lets you validate the new node label entered by the user.")
		, Category("Editor Action")]
		public event TreeNodeAdvCancelableEditEventHandler NodeEditorValidating;

		/// <summary>
		/// Occurs after the newly entered text in the Node editor gets stored.
		/// </summary>
		[Description("Notifies that a new label has been provided for a node by the user.")
		, Category("Editor Action")]
		public event TreeNodeAdvEditEventHandler NodeEditorValidated;

        /// <summary>
        /// Occurs after the Editing mode gets cancelled by Escape key.
        /// </summary>
        [Description("Occurs when the user cancels the editing mode.")
        , Category("Editor Action")]
        public event TreeNodeAdvEditEventHandler EditCancelled;

		/// <summary>
		/// Raises the NodeEditorValidateString event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnNodeEditorValidateString method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnNodeEditorValidateString in a derived
		/// class, be sure to call the base class's OnNodeEditorValidateString method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnNodeEditorValidateString(TreeNodeAdvCancelableEditEventArgs e)
		{
			if(NodeEditorValidateString !=null)
			{
				NodeEditorValidateString(this,e);
			}
			return;
		}

		/// <summary>
		/// Raises the NodeEditorValidating event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnNodeEditorValidating method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnNodeEditorValidating in a derived
		/// class, be sure to call the base class's OnNodeEditorValidating method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnNodeEditorValidating(TreeNodeAdvCancelableEditEventArgs e)
		{
			if(NodeEditorValidating !=null)
			{
                IsEditEnding = true;
				NodeEditorValidating(this,e);
                IsEditEnding = false;
			}
			return;
		}

        //To avoid nested calling of EndEdit method.
        private bool IsEditEnding = false;

		/// <summary>
		/// Raises the NodeEditorValidated event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnNodeEditorValidated method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnNodeEditorValidated in a derived
		/// class, be sure to call the base class's OnNodeEditorValidated method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnNodeEditorValidated(TreeNodeAdvEditEventArgs e)
		{
			if(NodeEditorValidated !=null)
			{
				NodeEditorValidated(this,e);
			}
		}

        /// <summary>
        /// Raises the edit cancel event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// <para>The OnEditCancelled method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnEditCancelled in a derived
        /// class, be sure to call the base class's OnEditCancelled method so that
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnEditCancelled(TreeNodeAdvEditEventArgs e)
        {
            if (EditCancelled != null)
            {
                EditCancelled(this, e);
            }
        }
		/// <summary>
		/// Occurs before a node gets into the edit mode.
		/// </summary>
		[Description("Occurs before a node gets into edit mode.")
		, Category("Editor Action")]
		public event TreeViewAdvBeforeEditEventHandler BeforeEdit;

		/// <summary>
		/// Raises the BeforeEdit event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeEdit method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeEdit in a derived
		/// class, be sure to call the base class's OnBeforeEdit method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforeEdit(TreeNodeAdvBeforeEditEventArgs e)
		{
			if(BeforeEdit !=null)
			{
				BeforeEdit(this,e);
			}
			return;
		}
		#endregion EDITING

		/// <summary>
		/// Calls the <see cref="OnItemDrag"/> to raise the <see cref="ItemDrag"/> event.
		/// </summary>
		/// <param name="e">An ItemDragEventArgs that contains the event data.</param>
		public void RaiseItemDrag(ItemDragEventArgs e)
		{
			if(! (e.Item is TreeNodeAdv[]))
				throw new Exception("The RaiseItemDrag method is called with invalid argument type.");

			this.draggedTnas = e.Item as TreeNodeAdv[];
			this.OnItemDrag(e);

			if( !m_bAllowDropStubWorks )
			{
				this.draggedTnas = null;
			}
		}
		/// <summary>
		/// Raises the ItemDrag event.
		/// </summary>
		/// <param name="e">An ItemDragEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnItemDrag method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnItemDrag in a derived
		/// class, be sure to call the base class's OnItemDrag method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnItemDrag(ItemDragEventArgs e)
		{
			if(ItemDrag!=null)
			{
				if(! (e.Item is TreeNodeAdv[]))
					throw new Exception("The OnItemDrag method is called with invalid argument type.");
				
				ItemDrag(this,e);
			}
		}

		/// <summary>
		/// Fired before a node is being painted when the <see cref="TreeViewAdv.OwnerDrawNodes"/> property is set to true.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to draw the node yourself. If you set the <see cref="TreeNodeAdvPaintEventArgs.Handled"/>
        /// property to true the TreeViewAdv assumes that you have drawn all the contents of the node and no additional drawing will be done by the TreeViewAdv.
		/// If you leave it to false the TreeViewAdv will automatically draw the usual contents of the node. Do not draw the background of the node here. 
        /// Otherwise it will draw over the vertical line. Use the NodeBackgroundPaint for painting the background.
		/// </remarks>
		[Category("Appearance")]
        [Description("Fired before a node is being painted when the OwnerDrawNodes property is set to true.")]
		public event TreeNodeAdvPaintEventHandler BeforeNodePaint;

		/// <summary>
		/// Fired after a node is being painted when the <see cref="TreeViewAdv.OwnerDrawNodes"/> property is set to true.
		/// </summary>
		/// <remarks>
		/// This event is ideal for custom drawing portions of the node in addition to the default drawing.
		/// The HandledXXX properties of the event args can be ignored for this event.
		/// </remarks>
		[Category("Appearance")]
        [Description("Fired after a node is being painted when the OwnerDrawNodes property is set to true.")]
		public event TreeNodeAdvPaintEventHandler AfterNodePaint;

		/// <summary>
		/// Fired to draw the background of a node if the <see cref="OwnerDrawNodesBackground"/> property is set.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to draw the background of the node yourself.
		/// </remarks>
		[Category("Appearance")]
        [Description("Fired to draw the background of a node if the OwnerDrawNodesBackground property is set.")]
		public event TreeNodeAdvPaintBackgroundEventHandler NodeBackgroundPaint;

		/// <summary>
		/// Raises the NodeBackgroundPaint event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnNodeBackgroundPaint method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnNodeBackgroundPaint in a derived
		/// class, be sure to call the base class's OnNodeBackgroundPaint method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnNodeBackgroundPaint(TreeNodeAdvPaintBackgroundEventArgs e)
		{
			if(this.NodeBackgroundPaint !=null)
			{ 
				this.NodeBackgroundPaint(this,e);
			}
		}
		/// <summary>
		/// Occurs before a node is selected.
		/// </summary>
		/// <remarks>
		/// The collection in the <see cref="TreeViewAdvSelectionEventArgs.SelectedNodes"/> property is 
		/// both read-only and fixed size.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs before a node is selected.")]
		public event TreeNodeAdvBeforeSelectEventHandler BeforeSelect;

		/// <summary>
		/// Occurs before a node's checkbox is checked.
		/// </summary>
		[Category("Action")]
        [Description("Occurs before a node's checkbox is checked.")]
		public event TreeViewAdvBeforeCheckEventHandler BeforeCheck;

		/// <summary>
		/// Occurs after a node is selected.
		/// </summary>
		/// <remarks>
		/// You can determine the selected node using the <see cref="TreeViewAdv.SelectedNode"/> property.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs after a node is selected.")]
		public event EventHandler AfterSelect;

		/// <summary>
		/// Occurs after a node is checked.
		/// </summary>
		/// <remarks>
		/// <para>This event will be fired when the node's <see cref="TreeNodeAdv.CheckState"/> property has changed or when a new 
		/// node has been <see cref="TreeNodeAdv.Optioned"/>.</para>
		/// <para>You could alternatively listen to the individual node's 
		/// <see cref="Syncfusion.Windows.Forms.Tools.TreeNodeAdv.CheckStateChanged"/> event.</para>
		/// </remarks>
		[Category("Action")]
        [Description("Occurs after a node is checked.")]
		public event TreeNodeAdvEventHandler AfterCheck;

		/// <summary>
		/// Occurs after one or more node's CheckState has changed due to <see cref="TreeNodeAdv.InteractiveCheckBox"/> setting.
		/// </summary>
		/// <remarks>
		/// <para>When <see cref="TreeNodeAdv.InteractiveCheckBox"/> is turned on in a parent node, changing the parent's <see cref="TreeNodeAdv.CheckState"/> or one
		/// of it's children's will cause the CheckStates of the parent and the child nodes to be updated appropriately.
		/// This event will be fired at the end of all these updates.</para>
		/// </remarks>
		[Category("Action")]
        [Description("Occurs after one or more node's CheckState has changed due to InteractiveCheckBox setting.")]
		public event TreeNodeAdvEventHandler AfterInteractiveChecks;

		/// <summary>
		/// Occurs before a node is expanded.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to do some processing of the specified node before it's expanded.
		/// Use this event when you set the <see cref="TreeViewAdv.LoadOnDemand"/> property to true to add child nodes to the specified node before it is expanded.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs before a node is expanded.")]
		public event TreeViewAdvCancelableNodeEventHandler BeforeExpand;

		/// <summary>
		/// Occurs before a node has collapsed.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to do some processing of the specified node before it's collapsed.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs before a node has collapsed.")]
		public event TreeViewAdvCancelableNodeEventHandler BeforeCollapse;

		/// <summary>
		/// Occurs after a node is expanded.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to do some processing of the specified node after it's expanded.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs after a node is expanded.")]
		public event TreeViewAdvNodeEventHandler AfterExpand;

		/// <summary>
		/// Occurs after a node has collapsed.
		/// </summary>
		/// <remarks>
		/// Handle this event when you want to do some processing of the specified node after it's collapsed.
		/// </remarks>
		[Category("Action")]
        [Description("Occurs after a node has collapsed.")]
		public event TreeViewAdvNodeEventHandler AfterCollapse;

		/// <summary>
		/// Raises the BeforeNodePaint event.
		/// </summary>
		/// <param name="e">An <see cref="TreeNodeAdvPaintEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeNodePaint method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeNodePaint in a derived
		/// class, be sure to call the base class's OnBeforeNodePaint method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforeNodePaint(TreeNodeAdvPaintEventArgs e)
		{
			if(BeforeNodePaint!=null)
			{
				BeforeNodePaint(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterNodePaint event.
		/// </summary>
		/// <param name="e">An <see cref="TreeNodeAdvPaintEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterNodePaint method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterNodePaint in a derived
		/// class, be sure to call the base class's OnAfterNodePaint method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnAfterNodePaint(TreeNodeAdvPaintEventArgs e)
		{
			if(AfterNodePaint!=null)
			{
				AfterNodePaint(this,e);
			}
		}

		/// <summary>
		/// Raises the BeforeExpand event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeExpand method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeExpand in a derived
		/// class, be sure to call the base class's OnBeforeExpand method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforeExpand(TreeViewAdvCancelableNodeEventArgs e)
		{
			if(BeforeExpand!=null)
			{
				BeforeExpand(this,e);
			}
		}

		/// <summary>
		/// Raises the BeforeCollapse event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeCollapse method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeCollapse in a derived
		/// class, be sure to call the base class's OnBeforeCollapse method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforeCollapse(TreeViewAdvCancelableNodeEventArgs e)
		{
			if(BeforeCollapse!=null)
			{
				BeforeCollapse(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterExpand event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterExpand method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterExpand in a derived
		/// class, be sure to call the base class's OnAfterExpand method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnAfterExpand(TreeViewAdvNodeEventArgs e)
		{
			this.ValidateScrollPosition();
			if(AfterExpand!=null)
			{
				AfterExpand(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterCollapse event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterCollapse method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterCollapse in a derived
		/// class, be sure to call the base class's OnAfterCollapse method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnAfterCollapse(TreeViewAdvNodeEventArgs e)
		{
			this.ValidateScrollPosition();
			if(AfterCollapse!=null)
			{
				AfterCollapse(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterSelect event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterSelect method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterSelect in a derived
		/// class, be sure to call the base class's OnAfterSelect method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnAfterSelect(EventArgs e)
		{
			if(AfterSelect!=null)
			{
				AfterSelect(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterCheck event.
		/// </summary>
		/// <param name="e">A <see cref="TreeNodeAdvEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterCheck method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterCheck in a derived
		/// class, be sure to call the base class's OnAfterCheck method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnAfterCheck(TreeNodeAdvEventArgs e)
		{
			if( m_bIsMouseDown )
			{
				e.Action = TreeViewAdvAction.ByMouse;
			}
			if( m_bIsKeyDown )
        		e.Action = TreeViewAdvAction.ByKeyboard;
			// add node to or remove from checked nodes collection
			if( e.Node != null )
			{
				checkedNodes.ResolveNode( e.Node );
			}

			if(AfterCheck!=null)
			{
				AfterCheck(this,e);
			}
		}

		/// <summary>
		/// Raises the AfterInteractiveChecks event.
		/// </summary>
		/// <param name="e">A <see cref="TreeNodeAdvEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnAfterInteractiveChecks method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnAfterInteractiveChecks in a derived
		/// class, be sure to call the base class's OnAfterInteractiveChecks method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnAfterInteractiveChecks(TreeNodeAdvEventArgs e)
		{
			if(AfterInteractiveChecks!=null)
			{
				AfterInteractiveChecks(this,e);
			}
		}

		/// <summary>
		/// Raises the BeforeSelect event.
		/// </summary>
		/// <param name="e">An <see cref="TreeViewAdvCancelableSelectionEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeSelect method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeSelect in a derived
		/// class, be sure to call the base class's OnBeforeSelect method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforeSelect(TreeViewAdvCancelableSelectionEventArgs e)
		{
			if(BeforeSelect!=null)
			{
				BeforeSelect(this,e);
			}
		}

		/// <summary>
		/// Raises the BeforeCheck event.
		/// </summary>
		/// <param name="e">An <see cref="TreeViewAdvBeforeCheckEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforeCheck method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBeforeCheck in a derived
		/// class, be sure to call the base class's OnBeforeCheck method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnBeforeCheck(TreeNodeAdvBeforeCheckEventArgs e)
		{
			if(BeforeCheck!=null)
			{
				BeforeCheck(this,e);
			}
		}

		#endregion

		#region Undo\Redo implementation
		private HistoryManager m_historyManager = null;
		private bool m_bHistoryEnabled = false;

		/// <summary>
		/// Gets or sets the HistoryManager to use.
		/// </summary>
		[ Category( "Appearance" ) ]
		[ Description( @"HistoryManager to use for Undo\Redo operations." ) ]
		[DefaultValue(null)]
		public HistoryManager HistoryManager
		{ 
			get
			{
				return m_historyManager;
			}
			set
			{
				if( value != m_historyManager )
				{
					m_historyManager = value;
				}
			}
		}
		
		/// <summary>
		/// Indicates whether register items in history list.
		/// </summary>
		[ DefaultValue( false ) ]
		[ Category( "Appearance" ) ]
		[ Description( @"Indicates, record actions for Undo\Redo using, or not." ) ]
		public bool HistoryEnabled
		{
			get
			{
                return m_bHistoryEnabled;
			}
			set
			{
				if( value != m_bHistoryEnabled )
				{
                    m_bHistoryEnabled = value;					
				}
			}
		}
		#endregion

		#region Properties
		internal bool IsBroughtIntoView
		{
			get
			{
				return broughtIntoView;
			}
			set
			{
				if( broughtIntoView != value )
				{
					broughtIntoView = value;
				}
			}
		}
		/// <summary>
		/// Indicates whether an alpha blended image of the selected nodes
		/// should be drawn beside the cursor during drag and drop.
		/// </summary>
		/// <value>
		/// <para>True to show an alpha blended image; false otherwise. Default is true.</para>
		/// <para>You could customize the style in which nodes are drawn in the above image by 
		/// adding a "DragNodeCueStyle" style to the <see cref="BaseStyles"/> collection.</para>
		/// </value>
		[DefaultValue(true),
		Description("Specifies whether a semi-transaprent image of the selected nodes should be drawn beside the cursor during drag and drop.")]
		[Category("Appearance")]
		public virtual bool ShowDragNodeCue
		{
			get{return this.showDragNodeCue;}
			set{this.showDragNodeCue = value;}
		}
        bool isScaling = false;
        float _scalefactor = 1f;
        /// <summary> 
        /// Gets or sets a value to scale the control based upon. 
        /// </summary> 
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Layout"), Description("Gets or sets a value to scale the control based upon."),]
        public float ScaleFactor
        {
            get
            {
                return _scalefactor;
            }
            set
            {
                _scalefactor = value;
            }
        }
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary> 
        /// Gets or sets value to enable or disable the Touchmode to the controls. 
        /// </summary> 
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks> 
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(2.0F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }
        /// <summary></summary> 
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary> 
        /// Scale the control based on the scale factor passed in the argument. 
        /// </summary> 
        /// <param name="scaleFactor">value to scale the factor based upon.</param> 
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.ItemHeight = (int)(ITMHEIGHT * scaleFactor);
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        } 
        /// <summary>
        /// Gets or sets the active tree node.
        /// </summary>
		[
		Browsable(false),
		Documentation.DocumentationExclude(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public TreeNodeAdv ActiveNode
		{
			get{return this.activeNode;}
			set
			{
				if(this.activeNode != value)
				{
					this.activeNode = value;
					this.Invalidate();
				}
			}
		}
		/// <summary>
		/// Returns the helptext control of the TreeViewAdv.
		/// </summary>
		/// <remarks>This is the control used to display the <see cref="TreeNodeAdv.HelpText"/> of the nodes.</remarks>
		[Description("The helptext control of the TreeViewAdv.")]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ToolTipAdv HelpTextControl
		{
			get
			{
				if (helpText == null)
				{
					CreateHelpText();
				}

				return helpText;
			}
		}

		/// <summary>
		/// Returns the tooltip control of the TreeViewAdv.
		/// </summary>
		/// <remarks>
		/// This is the control used to display the tooltip for the nodes when the text of the nodes are partially visible.
		/// </remarks>
		[Description("The tooltip control of the TreeViewAdv.")]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ToolTipAdv ToolTipControl
        {
			get
			{
				if (toolTip == null)
				{
					CreatetoolTip();
				}
				return toolTip;
			}
		}

		/// <summary>
		/// Gets or sets the text color of the selected node.
		/// </summary>
		/// <value>Default is a system color.</value>
		[Description("Indicates the  text color of the selected node.")]
		[Category("Appearance")]
		public virtual Color SelectedNodeForeColor
		{
			get{return selectedNodeForeColor;}
			set{selectedNodeForeColor = value; Invalidate();}
		}

		[Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedNodeForeColor()
		{
			return selectedNodeForeColor != SystemColors.HighlightText;
		}

		[Documentation.DocumentationExclude()]
		protected void ResetSelectedNodeForeColor()
		{
			SelectedNodeForeColor = SystemColors.HighlightText;
		}

        public void CreatetoolTip()
        {
            this.toolTip = new Syncfusion.Windows.Forms.ToolTipAdv(this);

            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.SystemColors.Info;
            //this.toolTip.Border3DStyle = System.Windows.Forms.Border3DStyle.Sunken;
            //this.toolTip.BorderSingle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.toolTip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.toolTip.Location = new System.Drawing.Point(642, 0);
            this.toolTip.Name = "toolTip";
            //this.toolTip.RestrictWidth = 0;
            //this.toolTip.Size = new System.Drawing.Size(41, 17);
            this.toolTip.TabIndex = 1;
            this.toolTip.Text = "toolTip";
            //this.toolTip.InheritHostCursor = true;
            this.toolTip.Visible = false;
        }
        public void CreateHelpText()
        {
            this.helpText = new Syncfusion.Windows.Forms.ToolTipAdv(this);
            // 
            // helpText
            // 
            //this.helpText.Border3DStyle = System.Windows.Forms.Border3DStyle.Sunken;
            //this.helpText.BorderSingle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.helpText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.helpText.Location = new System.Drawing.Point(732, 0);
            this.helpText.Name = "helpText";
            //this.helpText.RestrictWidth = 0;
            //this.helpText.Size = new System.Drawing.Size(50, 17);
            this.helpText.TabIndex = 0;
            this.helpText.Text = "help text";
            //this.helpText.InheritHostCursor = true;
            this.helpText.Visible = false;
        }
		/// <summary>
		/// Gets or sets the text color of the selected node when not focused.
		/// </summary>
		/// <value>Default is a system color.</value>
		[Description("Indicates the  text color of the selected node when not focused.")]
		[Category("Appearance")]
		public virtual Color InactiveSelectedNodeForeColor
		{
			get{return inactiveSelectedNodeForeColor;}
			set{inactiveSelectedNodeForeColor = value; Invalidate();}
		}

		[Documentation.DocumentationExclude()]
		protected bool ShouldSerializeInactiveSelectedNodeForeColor()
		{
			return inactiveSelectedNodeForeColor != SystemColors.ControlText;
		}

		[Documentation.DocumentationExclude()]
		protected void ResetInactiveSelectedNodeForeColor()
		{
			InactiveSelectedNodeForeColor = SystemColors.ControlText;
		}

		/// <summary>
		/// Gets or sets the background of the selected node.
		/// </summary>
		/// <value>Default is based on a system color.</value>
		[Description("Indicates the background of the selected node.")]
		[Category("Appearance")]
		public virtual BrushInfo SelectedNodeBackground
		{
			get{return this.selectedNodeBackground;}
			set{this.selectedNodeBackground = value; Invalidate();}
		}

		[Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedNodeBackground()
		{
			return !selectedNodeBackground.Equals(new BrushInfo(SystemColors.Highlight));
		}

		[Documentation.DocumentationExclude()]
		protected void ResetSelectedNodeBackground()
		{
			this.SelectedNodeBackground = new BrushInfo(SystemColors.Highlight);
		}

		/// <summary>
		/// Gets or sets the background of the selected node when the control is not focused.
		/// </summary>
		/// <value>Default is based on a system color.</value>
		[Description("Indicates the background of the selected node when the control is not focused.")]
		[Category("Appearance")]
		public virtual BrushInfo InactiveSelectedNodeBackground
		{
			get{return this.inactiveSelectedNodeBackground;}
			set{this.inactiveSelectedNodeBackground = value; Invalidate();}
		}

		[Documentation.DocumentationExclude()]
		protected bool ShouldSerializeInactiveSelectedNodeBackground()
		{
			return !inactiveSelectedNodeBackground.Equals(new BrushInfo(SystemColors.Control));
		}

		[Documentation.DocumentationExclude()]
		protected void ResetInactiveSelectedNodeBackground()
		{
			this.InactiveSelectedNodeBackground = new BrushInfo(SystemColors.Control);
		}

		/// <summary>
		/// Indicates whether the drag-drop operation will occur only if the node is dragged on the text area.
		/// </summary>
		/// <value>Default is true.</value>
		[Description("Indicates if the drag-drop operation will occur only if the node is dragged on the text area.")]
		[DefaultValue(true)]
		[Category("Behavior")]
		public virtual bool DragOnText
		{
			get{return dragOnText;}
			set{dragOnText = value;}
		}

		/// <summary>
		/// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node has no children.
		/// </summary>
		/// <value>An index into the <see cref="StateImageList"/>. Default is zero.</value>
		[Description("Specifies the StateImageList index value of the image that is displayed when a tree node has no children.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public virtual int NoChildrenImgIndex
		{
			get 
			{
				return this.StandardStyle.NoChildrenImgIndex;
			}
			set 
			{
				if(this.NoChildrenImgIndex != value)
				{
					this.StandardStyle.NoChildrenImgIndex = value;
					this.Root.RecalculateAllDimensions();
					this.Invalidate();
				}
			}
		}
		
		
		/// <summary>
		/// Resets the <see cref="NoChildrenImgIndex"/> property.
		/// </summary>
		public void ResetNoChildrenImgIndex()
		{
			this.StandardStyle.ResetNoChildrenImgIndex();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeNoChildrenImgIndex()
		{
			return this.StandardStyle.ShouldSerializeNoChildrenImgIndex();
		}

		/// <summary>
		/// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node is collapsed.
		/// </summary>
		/// <value>An index into the <see cref="StateImageList"/>. Default is 1.</value>
		[Description("Specifies the StateImageList index value of the image that is displayed when a tree node is collapsed.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public virtual int ClosedImgIndex
		{
			get 
			{
				return this.StandardStyle.ClosedImgIndex;
			}
			set 
			{
				if(this.ClosedImgIndex != value)
				{
					this.StandardStyle.ClosedImgIndex = value;
					this.Root.RecalculateAllDimensions();
					this.Invalidate();
				}
			}
		}
		
		/// <summary>
		/// Resets the <see cref="ClosedImgIndex"/> property.
		/// </summary>
		public void ResetClosedImgIndex()
		{
			this.StandardStyle.ResetClosedImgIndex();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeClosedImgIndex()
		{
			return this.StandardStyle.ShouldSerializeClosedImgIndex();
		}

		/// <summary>
		/// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node is expanded.
		/// </summary>
		/// <value>An index into the <see cref="StateImageList"/>. Default is 2.</value>
		[Description("Specifies the StateImageList index value of the image that is displayed when a tree node is expanded.")]
		[Category("Appearance - Images")]
		[Localizable(true)]
		public virtual int OpenImgIndex
		{
			get 
			{
				return this.StandardStyle.OpenImgIndex;
			}
			set 
			{
				if(this.OpenImgIndex != value)
				{
					this.StandardStyle.OpenImgIndex = value;
					this.Root.RecalculateAllDimensions();
					this.Invalidate();
				}
			}
		}
		
		/// <summary>
		/// Resets the <see cref="OpenImgIndex"/> property.
		/// </summary>
		public void ResetOpenImgIndex()
		{
			this.StandardStyle.ResetOpenImgIndex();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeOpenImgIndex()
		{
			return this.StandardStyle.ShouldSerializeOpenImgIndex();
		}

		/// <summary>
		/// Gets or sets the imagelist that holds images to be drawn based on the state of the node.
		/// </summary>
		/// <remarks>The <see cref="OpenImgIndex"/>, <see cref="ClosedImgIndex"/> and
		/// <see cref="NoChildrenImgIndex"/> properties refer to an image inside this list.</remarks>
		[Description("Indicates the imagelist that holds images to be drawn based the state of the node.")]
		[Category("Appearance - Images")]
		[DefaultValue(null)]
		public virtual ImageList StateImageList
		{
			get{return stateImageList;}
			set
			{
				UpdateImageList( ref stateImageList, value );
			}
		}
		private void ImageList_HandleRecreated(object sender, EventArgs e)
		{
			this.Root.RecalculateAllDimensions();
			Invalidate();
		}
		/// <summary>
		/// Gets or sets the imagelist that holds images to be drawn on the right of the node.
		/// </summary>
		/// <remarks>The <see cref="TreeNodeAdv.RightImageIndices"/> will then indicate
		/// which images are to be drawn in the node.</remarks>
		[Description("Indicates the imagelist that holds images to be drawn on the right of the node.")]
		[Category("Appearance - Images")]
		[DefaultValue(null)]
		public virtual ImageList RightImageList
		{
			get{return rightImageList;}
			set
			{
				UpdateImageList( ref rightImageList, value );
			}
		}

		/// <summary>
		/// Gets or sets the imagelist that holds images to be drawn on the left of the node.
		/// </summary>
		/// <remarks>The <see cref="TreeNodeAdv.LeftImageIndices"/> will then indicate
		/// which images are to be drawn in the node.</remarks>
		[Description("Indicates the imagelist that holds images to be drawn on the left of the node.")]
		[Category("Appearance - Images")]
		[DefaultValue(null)]
		public virtual ImageList LeftImageList
		{
			get{return leftImageList;}
			set
			{
				UpdateImageList( ref leftImageList, value );
			}
		}

		[Documentation.DocumentationExclude()]
		private void UpdateImageList( ref ImageList ilTarget, ImageList ilSource )
		{
			if (ilTarget != ilSource)
			{
				if (ilTarget != null)
				{
					ilTarget.RecreateHandle -= new EventHandler( ImageList_HandleRecreated );
				}
				
				ilTarget = ilSource;

				if (ilTarget != null)
				{
					ilTarget.RecreateHandle += new EventHandler( ImageList_HandleRecreated );
				}

				this.Root.RecalculateAllDimensions();
				Invalidate();
			}
		}       


		/// <summary>
		/// Indicates whether the controls (eg PlusMinus) will have a transparent background.
		/// Setting this property slows down drawing of the TreeViewAdv control.
		/// </summary>
		/// <value>Default is false.</value>
		[Description("Indicates if the controls (eg PlusMinus) will have a transparent background.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool TransparentControls
		{
			get{return transparentControls;}
			set
			{
				if(transparentControls!=value)
				{
					transparentControls = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Indicates whether the selected node will be brought to view by scrolling, if necessary.
		/// </summary>
		/// <value>Default is true.</value>
		[Description("Indicates if the selected node will be brought to view by scrolling, if necessary.")]
		[Category("Behavior")]
		[DefaultValue(true)]
		public virtual bool EnsureVisibleSelectedNode
		{
			get{return this.ensureVisibleSelectedNode;}
			set{this.ensureVisibleSelectedNode = value;}
		}
		internal int NodeCount
		{
			get{return nodeCount;}
			set{nodeCount = value;}
		}

		/// <summary>
		/// Gets or sets the selection mode for the tree.
		/// </summary>
		/// <value>Default is <b>TreeSelectionMode.Single</b>.</value>
		/// <remarks>
		/// Note that setting this property does not affect the current selection state.
		/// For example, if the current selection includes multiple nodes and this property gets set
		/// to <b>TreeSelectionMode.Single</b>, then the <see cref="SelectionNodes"/>
		/// will not be cleared to show a single selection.
		/// </remarks>
		[Description("Indicates the selection mode for the tree.")]
		[Category("Behavior")]
		[DefaultValue(TreeSelectionMode.Single)]
		public virtual TreeSelectionMode SelectionMode
		{
			get{return this.selectionMode;}
			set
			{
				if(this.selectionMode != value)
				{
					this.selectionMode = value;
					OnSelectionModeChanged();
				}
			}
		}
        [Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSelectionModeChanged()
		{
			// reset selected nodes to satisfy new SelectionMode
			// in any case
			if( selectedNodes.Count > 1 )
			{
				selectedNodes.RemoveRange( 1, selectedNodes.Count - 1 );
				ActiveNode = SelectedNode = selectedNodes[ 0 ];
			}
			
		}

		//		/// <summary>
		//		/// Specifies if node selection should change on mouse down.
		//		/// </summary>
		//		/// <value>Default is true.</value>
		//		/// <remarks>By default node selection will change on mouse up, turn on this
		//		/// property to make node selection change on mouse down.</remarks>
		//		[Description("Specifies if node selection should change on mouse down.")]
		//		[Category("Behavior")]
		//		[DefaultValue(true)]
		//		private bool ChangeSelectionOnMouseDown
		//		{
		//			get{return this.SelectionMode != TreeSelectionMode.Single;}
		//			set
		//			{
		//				this.changeSelectionOnMouseDown = value;
		//			}
		//		}
		private bool MultiSelect
		{
			get{return this.SelectionMode != TreeSelectionMode.Single;}
		}
		private bool SingleSelect
		{
			get{return !this.MultiSelect;}
		}


		internal TreeNodeAdvStyleInfo BoundStyle
		{
			get{return this.boundStyle;}
		}

		/// <summary>
		/// Returns a collection of base styles used in the tree.
		/// </summary>
		/// <value>A Hashtable of style names versus styles. The style names are of type string and
		/// the styles are of type <see cref="TreeNodeAdvStyleInfo"/>.</value>
		/// <remarks>
		/// This collection holds the standard style that specifies the global node settings
		/// for all the nodes (is named "Standard"), the node level styles for nodes at specific levels (should use the
		/// convention "NodeLevelX") and other custom base styles. Also when you specify a style named
		/// "DragNodeCueStyle" that style will be applied on the nodes before preparing the 
		/// drag-cue bitmap during drag-and-drop, a feature that can be turned on using the <see cref="ShowDragNodeCue"/> property.
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Editor(typeof(TreeNodeAdvBaseStylesEditor),typeof(UITypeEditor)), Category("Appearance")]
        [Description("Returns a collection of base styles used in the tree.")]
		public virtual Hashtable BaseStyles
		{
			get
			{
				return baseStyles;
			}
		}

		internal bool IsBaseStyleRemoveable(string styleName)
		{
			if(styleName == DefaultBaseStyleName)
				return false;
			else
				return true;
		}

		internal string GetHintTextForStyle(string styleName)
		{
			string hint = String.Empty;
			if(styleName == DefaultBaseStyleName)
				hint = "Standard Style applied for all nodes. Not deletable.";
			else
			{
				int nodeLevel = this.IsNodeLevelStyle(styleName);
				if(nodeLevel != -1)
				{
					hint = "Style will be used for nodes in Level " + nodeLevel.ToString();
				}
			}

			return hint;
		}

		private int IsNodeLevelStyle(string styleName)
		{
			string nodeLevelString = TreeViewAdv.NodeLevelStyleBaseName;
			if(styleName.IndexOf(nodeLevelString) == 0)
			{
				styleName = styleName.Substring(nodeLevelString.Length);
				int level = -1;
				try
				{
					level = Int32.Parse(styleName);
				}
				catch{}
				if(level < -1)
					level = -1;
				return level;
			}
			else 
				return -1;
		}

		private StyleNamePairsList stylePairs = null;

        /// <summary>
        /// Gets or sets the base style name pairs.
        /// </summary>
		[Documentation.DocumentationExclude()]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StyleNamePairsList BaseStylePairs
		{
            get
            {
                bool isDifferent = true;
                StyleNamePairsList list = new StyleNamePairsList(this);

                ArrayList keyList = new ArrayList();

                foreach (string key in this.baseStyles.Keys)
                    keyList.Add(key);

                foreach (string styleName in keyList)
                    list.Add(new StyleNamePair( styleName, this.baseStyles[styleName] as TreeNodeAdvStyleInfo ));

                keyList.Clear();
                keyList = null;

                if( this.stylePairs.Count == list.Count )
                {
                    isDifferent = false;

                    for( int i = 0; i < list.Count; i++ )
                    {
                        if( (stylePairs[i] as StyleNamePair).name != (list[i] as StyleNamePair).name )
                        {
                            isDifferent = true;
                            break;
                        }
                    }
                }

                if( isDifferent )
                {
                    this.stylePairs = list;
                }

                return stylePairs;
            }
		}

		/// <summary>
		/// Returns the standard style that all the nodes inherit from, by default.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Can also be edited via the BaseStyles property editor. It is the style that all the nodes inherit from, by default.")]
		public TreeNodeAdvStyleInfo StandardStyle
		{
			get
			{
				if(this.baseStyles.Contains(TreeViewAdv.DefaultBaseStyleName))
					return this.baseStyles[TreeViewAdv.DefaultBaseStyleName] as TreeNodeAdvStyleInfo;
				else
					return null;
			}
		}
		/// <summary>
		/// Indicates whether the <see cref="TreeNodeAdv.GetPath"/> method adds a separator at the end of the path string returned.
		/// </summary>
		/// <value>Default is false.</value>
		[Description("Indicates if the TreeNodeAdv.GetPath method adds a separator at the end of the path string returned.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool AddSeparatorAtEnd
		{
			get{return addSeparatorAtEnd;}
			set{addSeparatorAtEnd = value;}
		}
		/// <summary>
		/// Gets or sets the space left on the left side of the control.
		/// </summary>
		/// <value>Default is 3.</value>
		[Description("Indicates the space left on the left side of the control.")]
		[Category("Appearance")]
		[DefaultValue(3)]
		public virtual int GutterSpace
		{
			get{return this.gutterSpace;}
			set
			{
				if(this.gutterSpace!=value)
				{
					this.gutterSpace = value;
					NeedUpdateCustomControls = true;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Indicates whether the label text of the tree nodes can be edited.
		/// </summary>
		/// <value>True if the label text of the tree nodes can be edited; false otherwise. The default is false.</value>
		/// <remarks>
		/// The <see cref="BeginEdit"/> method will let you begin editing a node
		/// programmatically irrespective of this setting.
		/// </remarks>
		[Description("Gets or sets a value indicating whether the label text of the tree nodes can be edited.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool LabelEdit
		{
			get{return labelEdit;}
			set{labelEdit = value;}
		}

		/// <summary>
		/// Indicates whether the <see cref="BeforeNodePaint"/> and <see cref="AfterNodePaint"/> events will be fired before drawing a node.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if the BeforeNodePaint event will be fired before drawing a node.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool OwnerDrawNodes
		{
			get{return ownerDrawNodes;}
			set{ownerDrawNodes = value;}
		}

		/// <summary>
		/// Indicates whether the <see cref="NodeBackgroundPaint"/> event will be fired before drawing a node's background.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if the NodeBackgroundPaint event will be fired before drawing a node's background.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool OwnerDrawNodesBackground
		{
			get{return ownerDrawNodesBackground;}
			set{ownerDrawNodesBackground = value;}
		}
	
		/// <summary>
		/// Indicates whether the tree should follow the load-on-demand paradigm.
		/// </summary>
		/// <value>Default value is false.</value>
		/// <remarks>
		/// <para>When set to true, all the nodes will have the plus-minus set to visible to begin with. 
		/// You should then handle the <see cref="BeforeExpand"/> event of the nodes and add subnodes to the respective nodes.
		/// The tree will then keep or hide the plus-minus based on whether or not children were added.</para>
		/// <para>This provides you a way to delay loading nodes in trees until the user initiates a node expand.</para>
		/// </remarks>
		[Description("Specifies if the tree should follow the load-on-demand paradigm.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool LoadOnDemand
		{
			get{return loadOnDemand;}
			set{loadOnDemand = value;}
		}

		/// <summary>
		/// Indicates whether the TreeViewAdv is printing.
		/// </summary>
		[Description("Indicates if the TreeViewAdv is printing.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Printing
		{
			get{return printing;}
		}

        /// <summary>
        /// Obsolete method. Use PrintPreview function instead.
        /// </summary>
        [Obsolete("Use PrintPreview function instead.")]
        public PrintDocument PrintDocument
        {
            get { return null; }
        }

		/// <summary>
        /// Indicates if node dimension calculation should be done on load.
        /// </summary>
        [Description("Indicates if node dimension calculation should be done on load.")]
        [DefaultValue(true)]
        public bool RecalculateExpansion
        {
            get { return recalculateExpansion; }
            set
            {
                if (recalculateExpansion != value)
                    recalculateExpansion = value;
            }
        }

		/// <summary>
		/// Indicates whether the nodes will have an option button.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if the nodes will have an option button.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool ShowOptionButtons
		{
			get{return this.StandardStyle.ShowOptionButton;}
			set
			{
				this.StandardStyle.ShowOptionButton = value;
				this.Root.RecalculateAllDimensions();
				Invalidate();
			}
		}
		/// <summary>
		/// Resets the <see cref="ShowOptionButtons"/> property.
		/// </summary>
		public void ResetShowOptionButtons()
		{
			this.StandardStyle.ResetShowOptionButton();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeShowOptionButtons()
		{
			return this.StandardStyle.ShouldSerializeShowOptionButton();
		}

		/// <summary>
		/// Indicates whether the selected node is in editing mode.
		/// </summary>
		[Description("Indicates if the selected node is in editing mode.")]
		[Category("Appearance"),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public bool IsEditing
		{
			get{return this.labelEditor.Visible;}
		}
		/// <summary>
		/// Gets or sets the separator string that splits the path of a node. 
		/// Call <see cref="TreeNodeAdv.GetPath"/> to get the path of the specified node.
		/// </summary>
		/// <value>Default value is "\".</value>
		[Description("Indicates the separator string that splits the path of a node.")]
		[Category("Appearance")]
		[DefaultValue("\\")]
		[Localizable(true)]
		public virtual string PathSeparator
		{
			get{return pathSeparator;}
			set{pathSeparator = value;}
		}

        private TreeStyle style = TreeStyle.Default;

        /// <summary>
        /// Indicates the visual style.
        /// </summary>
        /// <value>Default value is true.</value>
        [Description("Indicates the visual style.")]
        [Category("Appearance")]
        public TreeStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                if (style == TreeStyle.Metro)
                {
                    scroll.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                    scroll.AttachedTo = this;
                    this.Root.ShowLine = false;
                    this.ShowFocusRect = false;
                    if(this.DesignMode)
                        this.BorderStyle = BorderStyle.FixedSingle;
                    this.Border3DStyle = Border3DStyle.Flat;
                    this.FullRowSelect = false;
                    this.BackColor = Color.White;
                    this.SelectedNodeBackground = new BrushInfo(this.MetroColor);
                }
                else if (style == TreeStyle.Office2007)
                {
                    scroll.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
                    scroll.AttachedTo = this;
                    this.Root.ShowLine = false;
                    this.ShowFocusRect = false;
                    this.Border3DStyle = Border3DStyle.Sunken;
                    this.FullRowSelect = false;
                    OnStyleChanged();
                }
                else if (style == TreeStyle.Office2010)
                {
                    scroll.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                    scroll.AttachedTo = this;
                    this.Root.ShowLine = false;
                    this.ShowFocusRect = false;
                    this.Border3DStyle = Border3DStyle.Sunken;
                    this.FullRowSelect = true;
                    OnStyleChanged();
                }
                else
                {
                    scroll.DetachFrame();
                    this.Root.ShowLine = true;
                    this.ShowFocusRect = true;
                    this.Border3DStyle = Border3DStyle.Sunken;
                }
                Invalidate();
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string skinstyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return skinstyle;
            }
            set
            {
                skinstyle = value;

                if (value == "Office2007Blue")
                {
                    Style = TreeStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = TreeStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = TreeStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Style = TreeStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Style = TreeStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Style = TreeStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    Style = TreeStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Metro")
                    Style = TreeStyle.Metro;
                else if (value == "Default")
                    Style = TreeStyle.Default;

            }
        }
        private void OnStyleChanged()
        {
            if (style == TreeStyle.Office2007)
            {
                this.SelectedNodeBackground = new BrushInfo(this.Office2007ColorTable.SelectedNodeBackground);
                this.BackColor = this.Office2007ColorTable.TreeviewBackColor;
                this.ForeColor = this.Office2007ColorTable.TreeViewFontColor;
                this.SelectedNodeForeColor = this.Office2007ColorTable.TreeViewFontColor;
            }
            else if (style == TreeStyle.Office2010)
            {
                this.SelectedNodeBackground = new BrushInfo(this.Office2010ColorTable.SelectedNodeBackground);
                this.BackColor = this.Office2010ColorTable.TreeviewBackColor;
                this.ForeColor = this.Office2010ColorTable.TreeViewFontColor;
                this.SelectedNodeForeColor = this.Office2010ColorTable.TreeViewFontColor;
            }
        }

        /// <summary>
        /// Specifies office 2007 color scheme.
        /// </summary>
        private Office2007Theme color2007Scheme = Office2007Theme.Blue;
        /// <summary>
        /// Gets or sets office 2007 color scheme.
        /// </summary>
        [Description("Gets or sets office 2007 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2007Theme.Blue)]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return this.color2007Scheme;
            }

            set
            {
                if (this.color2007Scheme != value)
                {
                    this.color2007Scheme = value;
                    this.OnStyleChanged();
                }
            }
        }

        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        public Office2007Colors Office2007ColorTable
        {
            get
            {
                Office2007Colors colorTable = Office2007Colors.GetColorTable(this.Office2007ColorScheme);

                return colorTable;
            }
        }
        private void Office2007ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            OnStyleChanged();
        }


        /// <summary>
        /// Specifies office 2010 color scheme.
        /// </summary>
        private Office2010Theme color2010Scheme = Office2010Theme.Blue;
        /// <summary>
        /// Gets or sets office 2010 color scheme.
        /// </summary>
        [Description("Gets or sets office 2010 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2010Theme.Blue)]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return this.color2010Scheme;
            }

            set
            {
                if (this.color2010Scheme != value)
                {
                    this.color2010Scheme = value;
                    this.OnStyleChanged();
                }
            }
        }

        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        public Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = Office2010Colors.GetColorTable(this.Office2010ColorScheme);

                return colorTable;
            }
        }
        private void Office2010ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
        {
            OnStyleChanged();
        }
        
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color  metroColor = ColorTranslator .FromHtml ("#16A5DC");

        /// <summary>
        /// Indicates the metro color.
        /// </summary>
        /// <value>Default value is true.</value>
        [Description("Indicates the metro color.")]
        [Category("Appearance")]
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                metroColor = value;
                Invalidate();
            }
        }


		/// <summary>
		/// Indicates whether the plus minus controls are visible.
		/// </summary>
		/// <value>Default value is true.</value>
		[Description("Indicates if the plus minus controls are visible.")]
		[Category("Appearance")]
		[DefaultValue(true)]
		public virtual bool ShowPlusMinus
		{
			get{return this.StandardStyle.ShowPlusMinus;}
			set
			{
				this.StandardStyle.ShowPlusMinus = value;
				this.Root.UpdateAllPlusMinusVisibility();
				this.Root.RecalculateAllDimensions();
				Invalidate();
			}
		}
		/// <summary>
		/// Resets the <see cref="ShowPlusMinus"/> property.
		/// </summary>
		public void ResetShowPlusMinus()
		{
			this.StandardStyle.ResetShowPlusMinus();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeShowPlusMinus()
		{
			return this.StandardStyle.ShouldSerializeShowPlusMinus();
		}

		/// <summary>
		/// Indicates whether the tree lines are visible.
		/// </summary>
		/// <value>Default value is true.</value>
		[Description("Indicates if the tree lines are visible.")]
		[Category("Appearance")]
		[DefaultValue(true)]
		public virtual bool ShowLines
		{
			get{return showLines;}
			set
			{
				if(showLines !=value)
				{
					showLines = value;
					this.Root.RecalculateAllDimensions();
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether lines are drawn between the tree nodes that are at the root of the tree view.
		/// </summary>
		/// <value>Default value is true.</value>
		[Description("Indicates whether lines are displayed between root nodes.")]
		[Category("Appearance")]
		[DefaultValue(true)]
		public virtual bool ShowRootLines
		{
			get{return showRootLines;}
			set
			{
				if(showRootLines !=value)
				{
					showRootLines = value;
					this.UpdateRootPlusMinusVisibility();
					this.Root.RecalculateAllDimensions();
					Invalidate();
				}
			}
		}
		private void UpdateRootPlusMinusVisibility()
		{
			// Pushing the visibility setting of the PM part in the node
			// to improve painting performance.
			foreach(TreeNodeAdv node in this.Root.Nodes)
				node.UpdatePlusMinusVisibility();
		}

		
		internal bool NeedRootLinesSpace
		{
			get
			{
				if(!this.ShowLines && !this.ShowPlusMinus)
					return false;
				else return this.ShowRootLines;
			}
		}

		/// <summary>
		/// Gets or sets the indent of the child nodes from the parent node.
		/// </summary>
		/// <value>Default value is 19.</value>
		[Description("Indicates the indent of the child nodes from the parent node.")]
		[Category("Appearance")]
		[DefaultValue(19)]
		public virtual int Indent
		{
			get{return parentIndent;}
			set
			{
				if(parentIndent!=value)
				{
					parentIndent = value;
					this.Root.RecalculateAllDimensions();
					Invalidate();
				}
			}

		}

		/// <summary>
		/// Indicates whether the nodes will have a hot tracked appearance when the mouse cursor is hovering over them.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if the nodes will have a hot tracked appearance when the mouse cursor is hovering over them.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool HotTracking
		{
			get{return hotTracking;}
			set{hotTracking = value;}
		}
		/// <summary>
		/// Indicates whether multiple nodes can be selected with mouse down and drag.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates multiple nodes can be selected with mouse down and drag.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool AllowMouseBasedSelection
		{
			get{return this.allowMouseBasedSelection;}
			set{this.allowMouseBasedSelection = value;}
		}
		private bool MouseBasedSelectionOn
		{
			get{return this.mouseBasedSelectionOn;}
			set
			{
				if(this.mouseBasedSelectionOn != value)
				{
					this.mouseBasedSelectionOn = value;
					this.latestMouseBasedSelectionCollection = null;
				}
			}
		}
        private TreeNodeAdvCollection tnaCollection = new TreeNodeAdvCollection();
        private void iterate(TreeNodeAdv tna, bool bExpanded)
        {
            //Checks whether the node contains child node or not. If yes, foreach statement will iterate through all sibiling nodes of this node.
            if (tna.HasChildren)
            {
                if (tna.Expanded == bExpanded)
                {
                    if (!tnaCollection.Contains(tna))
                        tnaCollection.Add(tna);
                }

                foreach (TreeNodeAdv tnac in tna.Nodes)
                    iterate(tnac, bExpanded);
            }
            if (tna.NextNode != null)
            {
                iterate(tna.NextNode, bExpanded);
            }
        }

        /// <summary>
        /// Returns the collection of Nodes which are in Expanded state
        /// </summary>        
        [
        Browsable(false)
        ]
        public TreeNodeAdvCollection ExpandedNodes
        {
            get
            {
                tnaCollection.Clear();
                iterate(Nodes[0], true);
                return tnaCollection;
            }
        }

        /// <summary>
        /// Returns the collection of Nodes which are in Collapsed state
        /// </summary>
        [
		Browsable(false)
        ]        
        public TreeNodeAdvCollection CollapsedNodes
        {
            get
            {
                tnaCollection.Clear();
                iterate(Nodes[0], false);
                return tnaCollection;
            }
        }

		/// <summary>
		/// Returns the selected nodes of the TreeViewAdv.
		/// </summary>
		/// <remarks>Use this property only when <see cref="SelectionMode"/> property
		/// lets you select multiple nodes. Otherwise, use <see cref="SelectedNode"/> to get the single selected node.</remarks>
		[Description("Indicates the selected nodes of the TreeViewAdv.")]
		[Category("Behavior")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public SelectedNodesCollection SelectedNodes
		{
			get{return selectedNodes;}
		}
		/// <summary>
		/// Returns the checked nodes of the TreeViewAdv.
		/// </summary>
		[Description("Indicates the checked nodes of the TreeViewAdv.")]
		[Category("Behavior")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public CheckedNodesColection CheckedNodes
		{
			get
			{
				return checkedNodes;
			}
		}
		internal Rectangle SelectedNodesBounds
		{
			get
			{
				Rectangle bounds = Rectangle.Empty;
				foreach(TreeNodeAdv node in this.selectedNodes)
				{
					Rectangle nodeBounds = node.Bounds;
					bounds = Rectangle.Union(bounds, nodeBounds);
				}
				return bounds;
			}
		}
		/// <summary>
		/// Indicates whether the TreeViewAdv will hide it's selected nodes when not focussed.
		/// </summary>
		/// <value>True to hide selection; false otherwise. Default value is true.</value>
		[Description("Indicates if the TreeViewAdv will hide it's selected nodes when not focused.")]
		[Category("Appearance")]
		[DefaultValue(true)]
		public virtual bool HideSelection
		{
			get{return hideSelection;}
			set
			{
				if(hideSelection!=value)
				{
					hideSelection = value;
					if(!this.Focused)
						Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the complete row will be highlighted when a node is selected.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if the complete row will be highlighted when a node is selected.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool FullRowSelect
		{
			get{return fullRowSelect;}
			set
			{
				if(fullRowSelect!=value)
				{
					fullRowSelect = value;
					if(this.selectedNodes.Count > 0) Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the tree lines.
		/// </summary>
		/// <value>Default is Color.Gray.</value>
		[Description("Indicates the color of the tree lines.")]
		[Category("Appearance")]
		public virtual Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				if(lineColor!=value)
				{
					lineColor = value;
					
					Invalidate();
				}
			}
		}

		protected bool ShouldSerializeLineColor()
		{
			return lineColor != Color.Gray;
		}

		protected void ResetLineColor()
		{
			LineColor = Color.Gray;
		}

		/// <summary>
		/// Gets or sets the line style of the tree lines.
		/// </summary>
		/// <value>Default value is DashStyle.Dot.</value>
		[Description("Indicates the line style of the tree lines.")]
		[Category("Appearance")]
		[DefaultValue(DashStyle.Dot)]
		public virtual DashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				if(value == DashStyle.Custom)
				{
					if(DesignMode) MessageBox.Show("You can't set the LineStyle to Custom");
					return;
				}
				if(lineStyle !=value)
				{
					lineStyle = value;
					
					Invalidate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Pen LinePen
		{
			get
			{
				Pen p = new Pen(this.LineColor);
				
				p.DashStyle = this.LineStyle;
				
				return p;
			}
		}

		/// <summary>
		/// Indicates whether keyboard based searching should be allowed.
		/// </summary>
		/// <value>Default value is true.</value>
		/// <remarks>
		/// When set to true, the users can key in char keys to browse to the next node that begins with
		/// that character(s). Multiple characters entered in succession will be assumed to be part of the
		/// same word, so search will be performed on that substring. Search will be restricted to 
		/// <see cref="TreeNodeAdv.IsVisible"/> and <see cref="TreeNodeAdv.Expanded"/> nodes.
		/// </remarks>
		[Description("Gets or sets a value to indicate if keyboard based searching should be allowed.")]
		[DefaultValue(true)]
		[Category("Behavior")]
		public virtual bool AllowKeyboardSearch
		{
			get{return keySearching;}
			set{keySearching = value;}
		}

		/// <summary>
		/// Indicates whether the state of the parent node's checkbox is based on the checkstate of it's child nodes' checkboxes.
		/// </summary>
		/// <value>Default value is false.</value>
		/// <remarks>
		/// If all child nodes are checked the parent node is also checked. The same with unchecked.
		/// If some child nodes are checked and some are unchecked then the parent node will have an indeterminate state.
		/// If the CheckState of the parent node is set by code or by clicking on it the state of all subnodes will be set to that state.
		/// </remarks>
		[Description("Indicates if the state of the a node's checkbox indicates the checkstate of the child nodes checkboxes.")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool InteractiveCheckBoxes
		{
			get{return this.StandardStyle.InteractiveCheckBox;}
			set
			{
				this.StandardStyle.InteractiveCheckBox = value;
			}
		}
		/// <summary>
		/// Resets the <see cref="InteractiveCheckBoxes"/> property.
		/// </summary>
		public void ResetInteractiveCheckBoxes()
		{
			this.StandardStyle.ResetInteractiveCheckBox();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeInteractiveCheckBoxes()
		{
			return this.StandardStyle.ShouldSerializeInteractiveCheckBox();
		}



        private bool selectOnCollapse = true;

        /// <summary>
        /// Gets or sets a value indicating whether the collapsed node should be selected if any of the child node is selected or not.
        /// </summary>
        /// <value><c>true</c> if the collapsed node should be selected if that node has a selected child node; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If this property is set to false, it won't trigger the <see cref="BeforeSelect"/> and <see cref="AfterSelect"/> event after collapsing the node. 
        /// </remarks>
        [Description("Indicates whether the node should be selected if it's child node is selected or not.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool SelectOnCollapse
        {
            get { return selectOnCollapse; }
            set { selectOnCollapse = value; }
        }


		/// <summary>
		/// Indicates whether checkboxes will be shown for the nodes.
		/// </summary>
		/// <value>Default value is false.</value>
		[Description("Indicates if check boxes will be shown for the nodes.")]
		[Category("Appearance")]
		public virtual bool ShowCheckBoxes
		{
			get{return this.StandardStyle.ShowCheckBox;}
			set
			{
				this.StandardStyle.ShowCheckBox = value;
				this.Root.RecalculateAllDimensions();
				Invalidate();
			}
		}

		/// <summary>
		/// Resets the <see cref="ShowCheckBoxes"/> property.
		/// </summary>
		public void ResetShowCheckBoxes()
		{
			this.StandardStyle.ResetShowCheckBox();
			this.Root.RecalculateAllDimensions();
			this.Invalidate();
		}
		
		protected bool ShouldSerializeShowCheckBoxes()
		{
			return this.StandardStyle.ShouldSerializeShowCheckBox();
		}

		internal bool SetSelectedNode(TreeNodeAdv nodeToAdd, ArrayList removedNodes, 
			TreeViewAdvAction action)
		{
			ArrayList addedNodes = new ArrayList();
			if( nodeToAdd != null )
				addedNodes.Add( nodeToAdd );

			return this.SetSelectedNode(addedNodes, removedNodes, action);
		}

		internal bool SetSelectedNode(TreeNodeAdv nodeToAdd, TreeNodeAdv nodeToRemove, 
			TreeViewAdvAction action)
		{
			ArrayList addedNodes = new ArrayList(), removedNodes = new ArrayList();

			if( nodeToAdd != null )
				addedNodes.Add( nodeToAdd );

			if( nodeToRemove != null )
				removedNodes.Add( nodeToRemove );

			return this.SetSelectedNode( addedNodes, removedNodes, action );
		}
		internal bool SetSelectedNode(ArrayList nodesToAdd, 
			ArrayList nodesToRemove, TreeViewAdvAction action)
		{
			return this.SetSelectedNode( nodesToAdd, nodesToRemove, action, true, false );
		}
		/// <summary>
		/// This method is used internally by the tree control to add and remove selected nodes. This
		/// method will fire the appropriate selection events to let the user cancel the selection, etc.
		/// </summary>
		/// <param name="nodesToAdd">The nodes to add.</param>
		/// <param name="nodesToRemove">The nodes to remove.</param>
		/// <param name="action">Specifies what kind of action triggered this call.</param>
		/// <param name="fireEvent">Indicates whether selection events should be fired before and after this selection change.</param>
		/// <param name="forceRemove">Indicates whether the specified nodes to be removed from selection will be removed even if the user
		/// cancelled the selection change the BeforeSelect event handler.</param>
		/// <returns>True if the selection changed; false otherwise.</returns>
		protected internal bool SetSelectedNode(ArrayList nodesToAdd, ArrayList nodesToRemove,
			TreeViewAdvAction action, bool fireEvent, bool forceRemove)
		{
            if (nodesToRemove == null && nodesToAdd == null)
                return true;

            if (nodesToRemove == null)
                nodesToRemove = new ArrayList();
            if (nodesToAdd == null)
            {
                // See if there is anything to remove, or else quit.
                bool removeable = false;
                foreach (TreeNodeAdv node in nodesToRemove)
                {
                    if (this.selectedNodes != null && this.selectedNodes.Contains(node))
                    {
                        removeable = true;
                        break;
                    }
                }
                if (!removeable)
                    return true;

                nodesToAdd = new ArrayList();
            }
            // Procede only if the nodes to add and remove are different.
            if (nodesToRemove.Count == nodesToAdd.Count)
            {
                bool different = false;
                for (int i = 0; i < nodesToRemove.Count; i++)
                {
                    if (nodesToRemove[i] != nodesToAdd[i])
                    {
                        different = true;
                        break;
                    }
                }
                if (!different)
                    return true;
            }

            // We will be operating on this collection, so cannot enumerate on it.
            if (nodesToRemove == this.selectedNodes)
            {
                nodesToRemove = this.selectedNodes.Clone() as ArrayList;
            }

            // NOthing to change, return.
            if (nodesToAdd.Count == 0 && nodesToRemove.Count == 0)
                return true;

            SelectedNodesCollection newNodes = this.SelectedNodes.Clone() as SelectedNodesCollection;

            foreach (TreeNodeAdv node in nodesToRemove)
                newNodes.Remove(node);

            bool bIsSingleValid = (newNodes.Count == 0 && SelectionMode == TreeSelectionMode.Single);

            foreach (TreeNodeAdv node in nodesToAdd)
            {
                bool bIsMultipleValid = ((newNodes.Count > 0 && newNodes[0].Parent == node.Parent &&
                    SelectionMode == TreeSelectionMode.MultiSelectSameLevel) ||
                    newNodes.Count == 0 || SelectionMode == TreeSelectionMode.MultiSelectAll);

		        if( ( !bIsSingleValid && !bIsMultipleValid ) || ( newNodes.Contains( node ) && SelectionMode == TreeSelectionMode.MultiSelectSameLevel ) )
          			continue;

                newNodes.Add(node);
            }

            this.OrderNodesByRowIndex(newNodes);

            TreeViewAdvCancelableSelectionEventArgs args = new TreeViewAdvCancelableSelectionEventArgs(newNodes, action, false);
            if (fireEvent)
            {
                args.SelectedNodes.SetFixedSize(true);
                if (newNodes.Count > 0&&!newNodes[0].Enabled && !CanSelectDisabledNode)
                    args.Cancel = true;
                this.OnBeforeSelect(args);
                args.SelectedNodes.SetFixedSize(false);
            }

            if (!args.Cancel)
            {
                this.selectedNodes.Clear();

                TreeNodeAdv lastSelected = null;

                //Select nodes in upward and downward direction according to the selection by up/down keys
                if (selectUpwardDirection)
                {
                    for (int i = newNodes.Count - 1; i >= 0; i--)
                    {
                        TreeNodeAdv node = newNodes[i];
                        lastSelected = node;
                        this.selectedNodes.Add(node);
                        if (this.ensureVisibleSelectedNode && this.ensureVisible)
                        {
                            if ((!m_bIsMouseUp) || (m_bIsMouseUp && node == m_lastSelectedNode))
                            {
                                this.EnsureVisible(node);
                            }

                        }
                    }
                }
                else
                {
                    foreach (TreeNodeAdv node in newNodes)
                    {
                        lastSelected = node;
                        this.selectedNodes.Add(node);
                        if (this.ensureVisibleSelectedNode && this.ensureVisible)
                        {
                            if ((!m_bIsMouseUp) || (m_bIsMouseUp && node == m_lastSelectedNode))
                            {
                                if (m_lastSelectedByKeyBoard != null)
                                {
                                    if (node == m_lastSelectedByKeyBoard)
                                        this.EnsureVisible(node);
                                    
                                }
                                else
                                    this.EnsureVisible(node);
                            }
                        }
                    }
                    m_lastSelectedByKeyBoard = null;
                }

                if (newNodes.Count > 0)
                {
                    this.RefreshHighlitedNodes();
                    this.Invalidate();
                }

                if (lastSelected != null && lastSelected != this.Root)
                    this.curSelectedNodeIndex = lastSelected.Parent.Nodes.IndexOf(lastSelected);
                else
                    this.curSelectedNodeIndex = -1;

                if (action == TreeViewAdvAction.ByMouse && this.HScrollBar.Value != 0)
                    this.HScrollBar.Value = 0;

                if (fireEvent)
                    this.OnAfterSelect(EventArgs.Empty);
            }
            else if (forceRemove)
            {
                foreach (TreeNodeAdv node in nodesToRemove)
                {
                    this.selectedNodes.Remove(node);
                }
                if (nodesToRemove.Count > 0)
                {
                    this.RefreshHighlitedNodes();
                    this.Invalidate();
                }
            }

            return !args.Cancel;
		}

		/// <summary>
		/// Highlighted all parent nodes.
		/// </summary>
		/// <param name="node"></param>
		private void AddHighlightParent(TreeNodeAdv node)
		{			
			if( node != null && node != this.Root && 
				!this.m_hashHighlightedNodes.ContainsKey( node ) )
			{
				this.m_hashHighlightedNodes.Add( node, node );
        		AddHighlightParent(node.Parent);
			}
		}

		/// <summary>
		/// Refresh Highlighting.
		/// </summary>
		private void RefreshHighlitedNodes()
		{
			if( null == this.m_hashHighlightedNodes )
			{
				this.m_hashHighlightedNodes = new Hashtable();
			}
			else
			{
				this.m_hashHighlightedNodes.Clear();
			}

			foreach(TreeNodeAdv node in this.SelectedNodes)
			{
				AddHighlightParent(node.Parent);
			}
		}

		private void OrderNodesByRowIndex(ArrayList nodes)
		{
			ArrayList nodesX = new ArrayList();

			foreach(TreeNodeAdv node in nodes)
			{
				if(nodesX.Count == 0)
				{
					// Insert on top
					nodesX.Add(node);
					continue;
				}

				int nodeRowIndex = this.NodeToRowIndex(node);
				int i = -1;
				bool inserted = false;
				foreach(TreeNodeAdv nodeX in nodesX)
				{
					i++;
					if(this.NodeToRowIndex(nodeX) > nodeRowIndex)
					{
						// Insert before the one that has a higher rowindex.
						nodesX.Insert(i, node);
						inserted = true;
						break;
					}
				}
				if(!inserted)
					// Insert at the bottom.
					nodesX.Add(node);
			}
			nodes.Clear();
			nodes.AddRange(nodesX);
		}

		/// <summary>
		/// Gets or sets the selected node of the TreeViewAdv.
		/// </summary>
		/// <remarks>
		/// <para>The tree fires the <see cref="BeforeSelect"/> event to let you cancel the change 
		/// and <see cref="AfterSelect"/> event to notify you of a new selected node.</para>
		/// <para>Use to <see cref="SelectedNodes"/> property when multi-node selection is turned on.</para>
		/// </remarks>
		[Description("Indicates the selected node of the TreeViewAdv.")]
		[Category("Behavior")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public virtual TreeNodeAdv SelectedNode
		{
			get
			{
				if(this.SelectedNodes != null && this.SelectedNodes.Count > 0)
					return this.SelectedNodes[0];
				else
					return null;
			}
			set
			{
				if(value == this.Root)
					value = null;
				if(this.selectedNodes.Count != 1
					|| this.selectedNodes[0] != value)
				{
					if(this.SetSelectedNode(value, this.selectedNodes, TreeViewAdvAction.Unknown))
					{
						this.ActiveNode = value;
						this.selectionBaseNode = value;
						if(this.ActiveNode != null)
							this.ActiveNode.BringIntoView();
					}
				}
				this.IsBroughtIntoView = false;
			}
		}
		/// <summary>
		/// The base node, based on which multiple selection will be performed.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> instance or null if there is no such node.</value>
		/// <remarks>
		/// This node will be consulted while extending the selection in a multi-select
		/// scenario using user interaction or when calling the <see cref="ExtendSelectionTo"/> method.
		/// </remarks>
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Always),
		Description("The base node, based on which multiple selection will be performed."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual TreeNodeAdv SelectionBaseNode
		{
			get{return this.selectionBaseNode;}
		}
		/// <summary>
		/// Gets or sets the default height of the nodes.
		/// </summary>
		/// <value>Default value is dependent on the control's font height.</value>
		[Description("Indicates the default height of the nodes.")]
		[Category("Appearance")]
		public virtual int ItemHeight
		{
			get
			{
				if(this.itemHeight != -1)
					return itemHeight;
				else
					return base.FontHeight + 3;
			}
			set
			{
				if(itemHeight!=value)
				{
					//root.SetHeightIfChanged(itemHeight,value);
					itemHeight = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Resets the <see cref="ItemHeight"/> property.
		/// </summary>
		public void ResetItemHeight()
		{
			this.itemHeight = -1;
			this.Root.RecalculateAllDimensions();
		}

		protected bool ShouldSerializeItemHeight()
		{
			return this.itemHeight == -1 ? false : true;
		}

		/// <summary>
		/// Gets or sets the root node of the TreeViewAdv.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual TreeNodeAdv Root
		{
			get{return root;}
			set
			{
				root = value;
				root.TreeView = this;
				
				RefreshCustomControlCollection();

				root.Visible = true;
				this.UpdateRootPlusMinusVisibility();
				root.RecalculateAllDimensions();
				root.Expand();
				this.Invalidate();
			}
		}

		/// <summary>
		/// Recreate CustomControlCollection.
		/// </summary>
		private void RefreshCustomControlCollection()
		{
			foreach( DictionaryEntry element in this.CustomControlCollection )
			{
				Control control = (Control) element.Key;
                TreeNodeAdv node = (TreeNodeAdv) element.Value;
				if( this.Controls.Contains ( control ) )
				{
					this.Controls.Remove( control );
                    node.UnSubscribeControlEvents( control );
				}
			}

			this.CustomControlCollection.Clear();

			if( this.Root != null )
			{
				this.Root.CustomControlCollectionChanging( this.Root, CollectionChangeAction.Add );
			}
		}

		/// <summary>
		/// Auto custom controls adding in TreeViewAdv.Controls
		/// false - only in designer editor.
		/// </summary>
		internal bool m_bAutoControlsAdding = true;

		/// <summary>
		/// Custom control collection.
		/// Key - custom control.
		/// Value - node.
		/// </summary>
		private Hashtable m_htCustomControlCollection = new Hashtable();

		/// <summary>
		/// Gets custom control collection.
		/// </summary>
		internal Hashtable CustomControlCollection
		{
			get
			{
				return m_htCustomControlCollection;
			}
		}

		/// <summary>
		/// Gets or sets the top-level nodes collection of the TreeViewAdv.
		/// </summary>
		[Description("Indicates the top-level nodes of the TreeViewAdv.")]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TreeNodeAdvCollection Nodes
		{
			get{return root.Nodes;}
			//set{root.Nodes = value;}
		}

		/// <summary>
		/// Gets or sets the node on which the user did a right-mouse down.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> instance.</value>
		/// <remarks>
		/// <para>This property will return a non-null value only when the user
		/// has his mouse down or when the context menu is being shown for the tree.</para>
		/// <para>Use this property in your context-menu's popup event to determine on which
		/// node the user had right-clicked. However, do not use this property in a context menu
		/// item's Click property as this would be set to null by then. If the user right-clicked in the empty region then
		/// this property will return null.</para>
		/// <para>
		/// When the user instead used the keyboard to invoke the context menu (via Shift+F10)
		/// then this property will return the currently selected node and the
		/// menu will also appear beside the selected node.
		/// </para>
		/// </remarks>
		[Description("Indicates the node on which the user did a right-mouse down.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual TreeNodeAdv RMouseDownNode
		{
			get{return this.rmousedownnode;}
			set
			{
				if(this.rmousedownnode != value)
				{
					this.rmousedownnode = value;
					this.Invalidate();
				}
			}
		}
		// TODO: Should we clear this on MouseLeave?
		private TreeNodeAdv LMouseDownNode
		{
			get{return this.lmousedownnode;}
			set
			{
				if(this.lmousedownnode != value)
				{
					this.lmousedownnode = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the control should scroll while the user is dragging a horizontal scrollbar thumb.
		/// </summary>
		[
		Browsable(true),
		Category("Scrolling"),
		Description("Specifies if the control should scroll while the user is dragging a horizontal scrollbar thumb."),
		DefaultValue(true)
		]
		public override bool HorizontalThumbTrack
		{
			get
			{
				return base.HorizontalThumbTrack;
			}
			set
			{
				base.HorizontalThumbTrack = value;
			}
		}
        private bool suspendExpandRecalculate = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Recalculation of the Nodes maximum hieght should be done while expanding or collapsing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if suspend recalculate the nodes hieght while expand/collapse; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property can be reduced the delay while expanding/ collapsing the large number of nodes, if we set it as true.
        /// </remarks>
        [
       Browsable(true),
       Category("Behaviour"),
       Description("Gets or sets a value indicating whether the Recalculation of the Nodes maximum hieght should be done while expanding or collapsing."),
       DefaultValue(false)
       ]
        public bool SuspendExpandRecalculate
        {
            get
            {
                return this.suspendExpandRecalculate;
            }
            set
            {
                this.suspendExpandRecalculate = value;
            }
        }
        

		/// <summary>
		/// Indicates whether the size box should be drawn when both scrollbars are visible
		/// and the control is not a docked window in an MDIChild window. Note: Another better solution is drawing the NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </summary>
		/// <remarks>
		/// Showing the size box works around a problem with .NET controls because by
		/// default the the area at the bottom right is not drawn and that can cause
		/// drawing glitches. Note: Another better solution is drawing NonClientArea
		/// ourselves. See SizeGripStyle which implements this newer solution.
		/// </remarks>
		[DefaultValue(false),
		Browsable(false),
		Category("Scrolling"),
		Description("Specifies if size box should be drawn when both scrollbars are visible and the control is not docked in an MDIChild window.")
		]
		public new bool SmartSizeBox
		{
			get
			{
				return base.SmartSizeBox;
			}
			set
			{
				base.SmartSizeBox = value;
			}
		}

		/// <summary>
		/// Indicates whether the control should scroll while the user is dragging a vertical scrollbar thumb.
		/// </summary>
		[
		Browsable(true),
		Category("Scrolling"),
		Description("Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb."),
		DefaultValue(true)
		]
		public override bool VerticalThumbTrack
		{
			get
			{
				return base.VerticalThumbTrack;
			}
			set
			{
				base.VerticalThumbTrack = value;
			}
		}

		/// <summary>
		/// Gets or sets ImageList with images that are displayed 
		/// instead of expand or collapse button.
		/// </summary>
		/// 
		///The below description helps the user to set Custom images for expand/collapse (+/-) signs in the TreeViewAdv 
		///The standard +/- signs for the expand/collapse buttons in the TreeViewAdv can be replaced with 
		///the custom images by setting ImageList to the newly added NodeStateImageList property of the TreeViewAdv.
		///Single click on the image expands or collapses the current node.
		///By setting some particular index of default image for expand/collapse button in the 
		///TreeviewAdv's DefaultCollapseImageIndex and DefaultExpandImageIndex property ,all the
		///ParentNode's can be displayed with default Images for expanding and collapsing actions.
		///Each Parent Node's +/- signs can be set with different images ,by setting the TreeNodeAdv's
		///CollpaseImageIndex and ExpandImageIndex.
        [
		DefaultValue( null ),
		Description( "ImageList with images that are displayed instead of expand or collapse button." ),
		Category( "Appearance - Images" )
		]
		public ImageList NodeStateImageList
		{
			get
			{
				return m_nodeStateImageList;
			}
			set
			{
				if( value != m_nodeStateImageList )
				{
					m_nodeStateImageList = value;
					OnNodeStateImageListChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets index of default image for collapse button.
		/// </summary>
		[
		DefaultValue( DEF_DEFAULT_IMAGE_INDEX ),
		Description( "Index of default image for collapse button." ),
		Category( "Appearance - Images" )
		]
		public int DefaultCollapseImageIndex
		{
			get
			{
				return m_defaultCollapseImageIndex;
			}
			set
			{
				if( value != m_defaultCollapseImageIndex )
				{
					m_defaultCollapseImageIndex = value;
					OnDefaultCollapseImageIndexChanged();
				}
			}
		}

		
		/// <summary>
		/// Gets or sets the index of default image for expand button.
		/// </summary>
		[
		DefaultValue( DEF_DEFAULT_IMAGE_INDEX ),
		Description( "Index of default image for expand button." ),
		Category( "Appearance - Images" )
		]
		public int DefaultExpandImageIndex
		{
			get
			{
				return m_defaultExpandImageIndex;
			}
			set
			{
				if( value != m_defaultExpandImageIndex )
				{
					m_defaultExpandImageIndex = value;
					OnDefaultExpandImageIndexChanged();
				}
			}
		}

		#endregion

		#region OVERRIDES
		/// <override/>
		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new TreeViewAdvAcessibleObject(this);
		}
		/// </override>
		protected override void OnEnter(EventArgs arg)
		{
            // Choose a default selected node if none is selected.
			if(this.SelectedNode == null && !this.IsBroughtIntoView)
			{
				if(ShouldSelectNodeOnEnter)
                    this.SetDefaultSelectionOnEnter();	
                else 
                    if(this.Focused)
                        this.SetDefaultSelectionOnEnter();	
			}
			base.OnEnter(arg);
		}

        //Implemented for FR:730.
        private bool shouldSelectNodeOnEnter = true;

        /// <summary>
        /// Gets or sets a value indicating whether the default node should be selected on the TreeViewAdv control gains focus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if node should be selected on TreeViewAdv gains focus; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether the default node should be selected on the TreeViewAdv control gains focus.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShouldSelectNodeOnEnter
        {
			get
			{
				return shouldSelectNodeOnEnter;
			}
			set
			{
				shouldSelectNodeOnEnter = value;
			}
        }

		/// <summary>
		/// Selects a default node (the first visible one) if the tree did not
		/// have anything focused.
		/// </summary>
		protected virtual void SetDefaultSelectionOnEnter()
		{
			if(this.Nodes.Count > 0)
			{
				// If the mouse is down on a node, then select that node instead.
				// This will be the case when the tree gets focus via mouse down the first time.
				if(Control.MouseButtons != MouseButtons.None )
				{
					TreeNodeAdv nodeBelowMouse = this.GetNodeAtPoint(this.PointToClient(Control.MousePosition));
					if( nodeBelowMouse != null && nodeBelowMouse.Enabled 
						&& this.SetSelectedNode(nodeBelowMouse, this.selectedNodes, TreeViewAdvAction.Unknown))
					{
						this.ActiveNode = nodeBelowMouse;
						this.selectionBaseNode = nodeBelowMouse;
					}
				}
				if (this.SelectedNode == null && ShouldSelectNodeOnEnter)
				{
					// This logic should change once Enabled property is supported.
					TreeNodeAdv newSelNode = this.Root.NextSelectableNode;
					if(this.SetSelectedNode(newSelNode, this.selectedNodes, TreeViewAdvAction.Unknown))
					{
						this.ActiveNode = newSelNode;
						this.selectionBaseNode = newSelNode;
					}
				}
			}
		}

		/// <summary>
		/// Invalidates nodes in collection.
		/// </summary>
		/// <param name="col">Collection to invalidate.</param>
		private void InvalidateNodeCollection( System.Collections.ICollection col )
		{
			foreach( TreeNodeAdv node in col )
			{
				if( node.Visible )
				{
					Rectangle invRect = new Rectangle( this.ClientRectangle.X,
						node.Bounds.Y, this.ClientRectangle.Width, node.Bounds.Height );
					this.Invalidate( invRect );
				}
			}
		}

		/// </override>
		protected override void OnGotFocus(EventArgs e)
		{
			this.InvalidateNodeCollection( this.SelectedNodes );
			this.InvalidateNodeCollection( this.m_hashHighlightedNodes.Values );
			base.OnGotFocus(e);
		}
		/// </override>
		protected override void OnLostFocus(EventArgs e)
		{
			this.UpdateTips(false);
          
            for (int i = 0; i < this.SelectedNodes.Count; i++)
            {
                TreeNodeAdv tn = this.SelectedNodes[i];
                if (tn.Visible)
                {
                    //Fix for defect #1694: TreeViewAdv control has incorrect painting/invalidation when the control loses focus.
                    if (this.FullRowSelect)
                    {
                        Rectangle rect = tn.Bounds;

                        rect.Inflate(tn.Bounds.X, 0);
                        rect.Width += this.gutterSpace - tn.Bounds.X;
                        this.Invalidate(rect);

                    }
                    else
                        this.Invalidate(tn.Bounds);
                }
            }

            using (Graphics g = this.CreateGraphics())
            {

			// This change made by lucas in order to keep dotted rectangle
			// around selected nodes when control has no focus.
			if( KeepDottedSelection )
			{
				for( int i = 0, len = SelectedNodes.Count; i < len; ++i )
				{					
					TreeNodeAdv node = SelectedNodes[ i ];
					if( node.Visible )
					{
						node.DrawFocusRect( g, this );
					}
				}
			}			
            }
			base.OnLostFocus(e);
		}

        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
		public override void Refresh()
		{
			this.Root.RecalculateAllDimensions();

			base.Refresh();
		}

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
        /// <summary>
        /// Gets or sets padding within the control.
        /// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding = value;
			}
		}

        /// <summary>
        /// Occurs when the control's padding changes.
        /// </summary>
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false)
		]
		public new event EventHandler PaddingChanged
		{
			add
			{
				base.PaddingChanged += value;
			}
			remove
			{
				base.PaddingChanged -= value;
			}
		}
#endif	  

		protected override void OnRightToLeftChanged( EventArgs e )
		{
			this.NeedUpdateCustomControls = true;
			base.OnRightToLeftChanged( e );

			if( this.HScrollBar.InnerScrollBar != null )
			{
				this.HScrollBar.InnerScrollBar.RightToLeft = this.RightToLeft;
			}

			if( this.VScrollBar.InnerScrollBar != null )
			{
				this.VScrollBar.InnerScrollBar.RightToLeft = this.RightToLeft;
			}

			this.VScrollBar.RightToLeft = this.RightToLeft;
			this.HScrollBar.RightToLeft = this.RightToLeft;

            if( this.Office2007ScrollBars && this.RightToLeft == RightToLeft.Yes )
            {
                this.HScrollBar.Value = this.ReflectPosition( this.HScrollPos );
            }
            else
            {
                this.HScrollBar.Value = this.HScrollPos;
            }
        }

        #endregion OVERRIDES

		/// <summary>
		/// Creates a new TreeViewAdv control.
		/// </summary>
		public TreeViewAdv()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(TreeViewAdv));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}

			this.bgBrush = BrushInfo.Empty;
			this.stylePairs = new StyleNamePairsList(this);
			//RefreshBrush();

			// Create the default base style:
			this.boundStyle = new TreeNodeAdvStyleInfo(new TreeBoundStyleInfoStore(this));
			TreeNodeAdvStyleInfo baseStyle = new TreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this));
			this.baseStyles[TreeViewAdv.DefaultBaseStyleName] = baseStyle;

			InitializeComponent();

			this.BackColor = System.Drawing.SystemColors.Window;
			this.BackColorChanged += new System.EventHandler(this.GradientPanel_BackColorChanged);

			this.SetStyle(ControlStyles.Selectable,true);
			this.SetStyle(ControlStyles.DoubleBuffer,true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor,true);

			//Initialize the Root node.
			Root = new TreeNodeAdv("root");
			root.Expand();
			
			this.checkedNodes = new CheckedNodesColection();
			this.selectedNodes = new SelectedNodesCollection();
			this.selectedNodes.CollectionChanged += new CollectionChangeEventHandler(this.selectedNodes_Changed);

			VScrollBar.SupportsThumbTrack = true;
			HScrollBar.SupportsThumbTrack = true;

			this.HScrollBar.SmallChange = 10;
			this.AllowIncreaseSmallChange = false;

			this.InsideScrollMargins = new Size(0, 10);
            this.timer = new Timer();
            CustomControlsImage = new Dictionary<Control, Point>();

            CTRLSIZE = this.Size;
            ITMHEIGHT = this.ItemHeight;
		}
		ScrollersFrame scroll = new ScrollersFrame();
		/// <summary>
		/// Begins the printing process of the TreeViewAdv.
		/// </summary>
		public void Print()
		{
			PrintHelper helper = new PrintHelper();

			helper.PrintTree( this, String.Empty );
		}
		/// <summary>
		/// Begins the printing process of the TreeViewAdv
		/// and shows TreeViewAdv before printing.
		/// </summary>
		public void PrintPreview()
		{
			PrintHelper helper = new PrintHelper();

			helper.PrintPreviewTree( this, String.Empty );
		}
        public Image ToImage()
        {
            PrintHelper helper = new PrintHelper();
            Image sample = helper.ToImage(this);
            return sample;
        }
		/// <summary>
		/// Returns a <see cref="System.Drawing.Bitmap"/> that contains the image of the dragged nodes
		/// with it's state image.
		/// </summary>
		/// <returns>A <see cref="System.Drawing.Bitmap"/> instance when there is atleast
		/// one selected node; Null otherwise.</returns>
		public Bitmap GetDraggedNodesBitmap()
		{
			if(this.draggedTnas == null || this.draggedTnas.Length == 0)
				return null;

			Rectangle clipRect = this.ClientRectangle;
			
			// If HorisontalScroll is on, adjust the X and width to reflect the whole tree rect.
			if(HScrollBar.Enabled)
			{
				clipRect.X = -this.HScrollBar.Value;
				clipRect.Width = this.ClientRectangle.Width+this.HScrollBar.Maximum;
			}

			Bitmap bmp = new Bitmap(clipRect.Width, clipRect.Height);
			
			Graphics g = Graphics.FromImage(bmp);

			if(clipRect.X < 0)
			{
				// Translate b'cos the bmp's coords start from 0 whereas
				// the clip-rect's X co-ord is negative b'cos of scrolling.
				g.TranslateTransform((float)-clipRect.X, 0, MatrixOrder.Append);
			}

			PaintEventArgs pea = new PaintEventArgs(g, clipRect);

			// Prevent drawing the selection rect while preparing the bitmap
			this.preparingDragCueBitmap = true;
			// Also apply the DragNodeCueStyle while preparing the bitmap.
			string oldBaseStyle = String.Empty;
			if(this.BaseStyles.Contains("DragNodeCueStyle"))
			{
				oldBaseStyle = this.StandardStyle.BaseStyle;
				this.StandardStyle.BaseStyle = "DragNodeCueStyle";
				
			}
			// Don't call this since OnPaintBackground includes logic for drawing child backgrounds.
			// this.InvokePaintBackground(this, pea);
			g.FillRectangle(SystemBrushes.Window, clipRect);

			this.OnPaint(pea);

			if(this.BaseStyles.Contains("DragNodeCueStyle"))
				this.StandardStyle.BaseStyle = oldBaseStyle;
			this.preparingDragCueBitmap = false;

			g.Dispose();

			Bitmap selBmp = this.ExtractSelectedBoundsFromBmp(bmp, this.draggedTnas);

			bmp.Dispose();

			return selBmp;
		}
		private Bitmap ExtractSelectedBoundsFromBmp(Bitmap bmp, TreeNodeAdv[] tnas)
		{
			Rectangle boundingRect = this.GetSelectedNodesRectangle( tnas );
            TreeNodeAdv parent = (tnas != null && tnas.Length > 0) ? (TreeNodeAdv)tnas[0].Parent : null;

			// Adjust the bounds to take scrolling into account
			if(this.HScrollBar.Enabled)
				boundingRect.X += this.HScrollBar.Value;

			Rectangle[] rectsDest = new Rectangle[tnas.Length];
			
			int minIndex = GetMinIndex( tnas );
			int maxIndex = GetMaxIndex( tnas );

            if (this.SelectedNodes != null)
            {
                foreach (TreeNodeAdv item in this.SelectedNodes)
                {
                    Point location = new Point(0, this.ItemHeight * this.SelectedNodes.IndexOf(item));
                    Size size = new Size(boundingRect.Width, this.ItemHeight);

                    // Adjust the bounds to take scrolling into account
                    if (this.HScrollBar.Enabled)
                        location.X += this.HScrollBar.Value;

                    rectsDest[this.SelectedNodes.IndexOf(item)] = new Rectangle(location, size);
                }
            }

			Bitmap selBmp = new Bitmap(boundingRect.Width, boundingRect.Height);

			Graphics g = Graphics.FromImage(selBmp);

			if(this.BackgroundColor != BrushInfo.Empty)
				BrushPaint.FillRectangle(g, new Rectangle(0, 0, selBmp.Width, selBmp.Height), this.BackgroundColor);
			else
				// To get the BackColor.
				//Don't call OnPaintBackground since it includes logic to draw child control's bg.
				//this.InvokePaintBackground(this, pea);
				g.FillRectangle(SystemBrushes.Window, 0, 0, boundingRect.Width, boundingRect.Height);

			for( int i = 0; i < this.SelectedNodes.Count; i++ )
			{
				TreeNodeAdv treeNode = this.SelectedNodes[i] as TreeNodeAdv;

				g.DrawString( treeNode.Text, treeNode.Font, Brushes.Black,  rectsDest[i]);
			}

			g.Dispose();
			
			return selBmp;
		}
		/// <summary>
		/// Returns the smallest rectangle enclosing the selected region of all the nodes specified.
		/// </summary>
		/// <param name="tnas">The nodes whose selected region is to be included in the resultant rect.</param>
		/// <returns>The bounding Rectangle.</returns>
		/// <remarks>The node's text and left images will be included in the rectangles.</remarks>
		protected Rectangle GetSelectedNodesRectangle(TreeNodeAdv[] tnas)
		{
			Rectangle boundingRect = Rectangle.Empty;
            TreeNodeAdv parent = tnas[0].Parent; 

			int minIndex = GetMinIndex( tnas );
			int maxIndex = GetMaxIndex( tnas );

			int maxWidth = tnas[ 0 ].Width;
			for( int i = 1; i < tnas.Length; i++)
				if( tnas[ i ].Width > maxWidth )
					maxWidth = tnas[ i ].Width;

            Point location = (parent != null) ? parent.Nodes[minIndex].Bounds.Location : Point.Empty;
			Size size = new Size( maxWidth, ( this.SelectedNodes.Count ) * this.ItemHeight );

			boundingRect = new Rectangle( location, size );

			return boundingRect;
		}

		private int GetMinIndex( TreeNodeAdv[] tnas )
		{
			int min = tnas[0].Index;
			for( int i = 1; i < tnas.Length; i++)
				if( tnas[ i ].Index < min )
					min = tnas[ i ].Index;
			
			return min;
		}

		private int GetMaxIndex( TreeNodeAdv[] tnas )
		{
			int max = tnas[0].Index;
			for( int i = 1; i < tnas.Length; i++)
				if( tnas[ i ].Index > max )
					max = tnas[ i ].Index;
			
			return max;
		}

        private void ResolveNodes( TreeNodeAdv node )
        {
            for( int i = 0; i < node.Nodes.Count; i++ ) 
            {
                ResolveNodes( node.Nodes[ i ] );
            }
            this.checkedNodes.ResolveNode( node );
        }

		void ISupportInitialize.BeginInit()
		{
			// Need this for initial vertical scroller setup
			this.RefreshVScrollbar( null, 0 );
		}
		void ISupportInitialize.EndInit()
		{
			root.RecalculateAllDimensions();
			root.Expand();

			VerticallScroll = true;
			HorisontalScroll = true;

			for( int i = 0; i < this.Nodes.Count; i++ )
			{
				this.ResolveNodes( this.Nodes[i] );
			}

			// Another option is to call BeginUpdate and EndUpdate in BeginInit and EndInit respectively.
			this.RefreshVScrollbar();

            this.rootMaxXChanged( root.MaxX );
		}
		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.LabelEditStartTimer = null;
                //For defect 2792:Memory leak due to ToolTips.
		        if( this.helpText != null )
        			this.helpText.Dispose();
        		if( this.toolTip != null )
          			this.toolTip.Dispose();
				if(this.selectedNodes != null)
				{
					this.selectedNodes.CollectionChanged -= new CollectionChangeEventHandler(this.selectedNodes_Changed);
					this.selectedNodes = null;
				}
				if( this.checkedNodes != null )
				{
					this.checkedNodes = null;
				}
				if(this.labelEditor != null)
				{
					this.labelEditor.TextChanged -= new System.EventHandler(this.labelEditor_TextChanged);
					this.labelEditor.MouseUp-= new MouseEventHandler( labelEditor_MouseUp );
					this.labelEditor.KeyDown -= new KeyEventHandler(this.labelEditor_KeyDown);
					this.labelEditor.Leave -= new System.EventHandler(this.labelEditor_Leave);
                    this.labelEditor.LostFocus -= new System.EventHandler(this.labelEditor_LostFocus);
				}
				// Make sure to relelase these event handlers
				if(this.stateImageList != null)
				{
					if(stateImageList != null)
						stateImageList.RecreateHandle -= new EventHandler(ImageList_HandleRecreated);
					
					stateImageList = null;
				}
				if(this.rightImageList != null)
				{
					if(rightImageList != null)
						rightImageList.RecreateHandle -= new EventHandler(ImageList_HandleRecreated);
					
					rightImageList = null;
				}
				if(this.leftImageList != null)
				{
					if(leftImageList != null)
						leftImageList.RecreateHandle -= new EventHandler(ImageList_HandleRecreated);
					
					leftImageList = null;
				}
				if (this.treeNodedragHelper != null)
                {
					this.treeNodedragHelper.DragWindow.Dispose();
                }
				
				if (themedEditDrawing != null)
				{
					themedEditDrawing.Dispose();
				}

				if (themedButtonDrawing != null)
				{
					themedButtonDrawing.Dispose();
				}
				
				if (themedTreeDrawing != null)
				{
					themedTreeDrawing.Dispose();
				}
                if (timer != null)
                    timer.Dispose();

				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.keyInputTimer = new System.Windows.Forms.Timer(this.components);
			this.labelEditor = new TextBox();
			this.SuspendLayout();
			// 
			// keyInputTimer
			// 
			this.keyInputTimer.Interval = 400;
			this.keyInputTimer.Tick += new System.EventHandler(this.keyInputTimer_Tick);
			// 
			// labelEditor
			// 
			this.labelEditor.AcceptsReturn = true;
			this.labelEditor.AcceptsTab = true;
			this.labelEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelEditor.Location = new System.Drawing.Point(530, 0);
			this.labelEditor.Name = "labelEditor";
			this.labelEditor.TabIndex = 0;
			this.labelEditor.Text = "";
			this.labelEditor.Visible = false;
			this.labelEditor.TextChanged += new System.EventHandler(this.labelEditor_TextChanged);
			this.labelEditor.KeyUp += new KeyEventHandler( this.labelEditor_KeyUp );
			this.labelEditor.KeyDown += new KeyEventHandler(this.labelEditor_KeyDown);
			this.labelEditor.MouseUp+=new MouseEventHandler(labelEditor_MouseUp);
			this.labelEditor.Leave += new System.EventHandler(this.labelEditor_Leave);
            this.labelEditor.LostFocus += new EventHandler(labelEditor_LostFocus);
			// 
			// TreeViewAdv
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.labelEditor});
			this.Size = new System.Drawing.Size(312, 368);
			this.SizeChanged += new System.EventHandler(this.TreeViewAdv_SizeChanged);
			this.ScrollbarsVisibleChanged += new System.EventHandler(this.TreeViewAdv_ScrollbarsVisibleChanged);
			this.Leave += new System.EventHandler(this.TreeViewAdv_Leave);
			this.ResumeLayout(false);

		}

		#endregion  

		internal void rootMaxXChanged(int maxX)
		{
			if( !HScroll )
				return;

			// Use Width instead of ClientSize.Width. Width will also include area occupied by scrollbars,
			// ClientSize does not.
			int width = this.Width;
			if( this.VerticallScroll )
			{
                width -= SystemInformation.VerticalScrollBarWidth;
                width -= 3; // for borders
            }

			if( width < 0 )
			{
				width = 0;
			}

			if( maxX > width && this.Nodes.Count > 0 )
			{
				if (!this.HorisontalScroll || !this.HScrollBar.Enabled)
				{
					this.HorisontalScroll = true;
					this.HScrollBar.Enabled = true;
				}
			}
			else
			{
				if (this.HorisontalScroll || this.HScrollBar.Enabled)
				{
					this.HScrollBar.Value = 0;
					this.HScrollBar.Enabled = false;
					this.HorisontalScroll = false;
				}
			}

			if( !inHScroll && maxX>width )
			{
				this.HScrollBar.Minimum = 0;
				this.HScrollBar.Maximum = maxX;//-Width;
				this.HScrollBar.LargeChange = width;//(int)(((float)maxX-Width) * ((float)Width/(float)maxX));  
                this.HScrollPos = 0;

				if( this.Office2007ScrollBars && this.RightToLeft == RightToLeft.Yes )
				{
					this.HScrollBar.Value = this.ReflectPosition( this.HScrollPos );
				}
				else
				{
					this.HScrollBar.Value = this.HScrollPos;
				}
			}
            else
            {
                if (maxX < width)
                    this.HScrollBar.LargeChange = this.Width;
            }

			if( HScrollBar.InnerScrollBar != null )
			{
				IScrollBar scrollBar = HScrollBar.InnerScrollBar as IScrollBar;
				if( scrollBar != null )
				{
					scrollBar.UpdateScrollInfo();
				}
			}
		}

        internal int ReflectPosition( int position )
        {
            if( this.HScrollBar != null )
            {
                return ( ( this.HScrollBar.Minimum + ( ( this.HScrollBar.Maximum - this.HScrollBar.LargeChange ) + 1 ) ) - position );
            }

            return -1;
        }
		
		internal virtual int DrawNode(Graphics g,Rectangle clip,TreeNodeAdv node,int y,int level, Point mousePos,bool mouseDown,bool background)
		{
			int spc = 3;
			int newY = y;

			int nNodeX = 0;
			int nNodeWidth = this.ClientRectangle.Width - this.gutterSpace + this.HScrollBar.Value;

			bool bIsMirrored = GetIsMirrored();
			if( bIsMirrored )
			{
				nNodeX = MirrorHorizontalPosition( this.gutterSpace, nNodeWidth );
			}
			else
			{
				nNodeX = this.gutterSpace - this.HScrollBar.Value;
			}

			node.iBounds = new Rectangle( nNodeX, y, nNodeWidth, node.Height );

			bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
			bool selected = false;
			bool active = false;
			bool hotTrack = false;

			if( !preparingDragCueBitmap )
			{
				selected = (selectedNodes.Contains( node ))&&(!hideSelection||Focused);
				active = node == this.activeNode;
				hotTrack = hotTracking && !m_bMouseLeaved && node.TextAndImageBounds.Contains( this.PointToClient( Control.MousePosition ) ) && !dragging;
			}

			int nNodeLeft = node.Bounds.X;
			int nNodeWidth1 = spc + node.Level * Indent + node.Width;
			if( bIsMirrored )
			{
				nNodeLeft = MirrorHorizontalPosition( nNodeLeft, nNodeWidth1 );
			}
			int nNodeRight = nNodeLeft + nNodeWidth1;

			// Highlighting node
			if( node.OptionedChild != null && !node.OptionedChild.IsVisible )
			{
				foreach( TreeNodeAdv highlightednode in this.m_hashHighlightedNodes.Values )
				{
					if( highlightednode == node )
					{
						selected = true;
						break;
					}
				}
			}

			if( !background )
			{
				if( active || this.ownerDrawNodes || 
					!((nNodeLeft > clip.Left && nNodeLeft > clip.Right) || (nNodeRight < clip.Left && nNodeRight < clip.Right)) )
				{
					#region /* comments */
					// This condition below was added by Lucas in order
					// to resolve problem # 191
					#endregion

					/**/
					//Call the BeforeNodePaint event
					bool bFlag = (node == m_highlightedNode) || selected;

					if( !selected )
					{
						if( m_highlightedNode != null )
						{
							if( m_highlightedNode.Nodes.Count == 0 )
							{
								bFlag = false;
							}
						}
						else
						{
							bFlag = false;
						}
					}
					/**/

					Point point = new Point( node.TextBounds.Left, node.TextBounds.Y );
					Color c = node.GetForeColor( selected, hotTrack );
					TreeNodeAdvPaintEventArgs e = new TreeNodeAdvPaintEventArgs( node,
						g, node.Bounds, point, level, parentIndent,
						bFlag, active, this.fullRowSelect, hotTrack, c );

					if( paintFilter == null ||
						!paintFilter.OnBeforeNodePaint( e ) )
					{
						if( this.ownerDrawNodes )
						{
							this.OnBeforeNodePaint( e );
						}
					}

					if (!e.Handled)
					{
						using (Pen p = this.LinePen)
						{
							node.Draw(ThemedTreeDrawing, ThemedButtonDrawing, p, mousePos, mouseDown,showFocusRect, e);
						}
					}
					if( this.ownerDrawNodes )
						this.OnAfterNodePaint( e );

					if( paintFilter != null )
						paintFilter.OnAfterNodePaint( e );
				}
			}
			else
			{
				// Also, assuming this is the case in ITreeNodeAdvPaintFilter.OnNodeBackgroundPaint
				BrushInfo bi = BrushInfo.Empty;
				if( selected )
				{
					if( this.Focused )
						bi = this.SelectedNodeBackground.Clone();
					else if( !this.HideSelection )
						bi = this.InactiveSelectedNodeBackground.Clone();
				}

				TreeNodeAdvPaintBackgroundEventArgs e = new TreeNodeAdvPaintBackgroundEventArgs( node, g, selected, active, fullRowSelect, hotTrack, bi );

				if( paintFilter == null ||
					!paintFilter.OnNodeBackgroundPaint( e ) )
				{
					if( this.ownerDrawNodesBackground )
					{
						this.OnNodeBackgroundPaint( e );
					}
				}
				Rectangle fullRow = node.Bounds;
				fullRow.Inflate( node.Bounds.X, 0 );
				fullRow.Width +=this.gutterSpace - node.Bounds.X;

				if( !e.Handled )
				{
					BrushPaint.FillRectangle( g, fullRow, node.Background );
				}
				if( !e.BrushInfo.IsEmpty && !e.Handled && !this.IsEditing )
				{
					Rectangle bounds = fullRowSelect?fullRow:node.TextBounds;
                    if (this.EnableTouchMode && (node.ShowOptionButton || node.ShowCheckBox))
                    {
                        bounds.X += 20;
                    }
					if( bounds.IntersectsWith( clip ) )
					{
                        BrushPaint.FillRectangle(g, fullRowSelect ? fullRow : bounds, e.BrushInfo);
					}
				}
			}

			newY += node.Height;

			return newY;
		}

		/// <summary>
		/// Draws the vertical lines of the tree.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="node">Node to draw the vertical lines to.</param>
		/// <param name="rowIndex">The RowIndex of the node.</param>
		/// <param name="lastNode">The Last Visible Node Row Index for comparing if in the node iteration nodes have passed it.</param>
		private void DrawVerticalLines(Graphics g, TreeNodeAdv topCRNode, TreeNodeAdv node,int rowIndex,int lastNode)
		{
			int visibleCount = node.VisibleNodeCount;

			// if  node and children are not visible upwards  
			// || node (and children) are not visible downwards
			// || has no children 
			// || not expanded
			if((VerticallScroll && rowIndex+visibleCount<=VScrollBar.Value) 
				|| (VerticallScroll && rowIndex>lastNode)
				|| !node.HasChildren 
				|| !node.Expanded)
				return;

			if(node.Nodes.Count > 0)
			{
				TreeNodeAdv lastCNode = node.Nodes[node.Nodes.Count - 1];
				// Lose the visible children of the last node
				visibleCount -= (lastCNode.VisibleNodeCount - (lastCNode.Visible ? 1 : 0));
			}

			TreeNodeAdv topVNode = topCRNode;

			if(topVNode.Level <= node.Level)
				// First child starts somewhere below the top visible node.
				topVNode = node.Nodes[0];
			else
			{
				// Find the immediate child (one of the parent).
				while(topVNode != null && topVNode.Parent != node)
					topVNode = topVNode.Parent;
				// The top node was not a child or grand-child
				if(topVNode == null)
					topVNode = node.Nodes[0];
			}

			// if !(node and immediate children are not visible - upwards)
			if(!(VerticallScroll && rowIndex+visibleCount<=VScrollBar.Value)) 
			{
				int level = node.Level;
				if(level != 0 || this.ShowRootLines)
				{
					bool bIsMirrored = GetIsMirrored();

					//the X of the line
					int lineX = level*Indent + node.PlusMinus.Width/2+this.gutterSpace;
					if (HScrollBar.Enabled && !bIsMirrored)
					{
						lineX -= HScrollBar.Value;
					}

					if(level != 0 && !this.NeedRootLinesSpace)
					{
						lineX -= this.Indent;
						lineX += node.PlusMinus.Width/2;
					}

					// Swap horizontally if mirrored
					if (bIsMirrored)
					{
						lineX = MirrorHorizontalPosition( lineX, 1 );
					}

					//The last child of the node. It will determine the endpoint of the vertical line.
					TreeNodeAdv lastChild = node.Nodes[node.Nodes.Count-1];

					// Instead of going through each child node, start from the top visible node
					// and look for a direct child of this node.
					int nodeCount = node.Nodes.Count;
					for(int i = node.Nodes.IndexOf(topVNode);i< nodeCount;i++)
					{
						TreeNodeAdv cNode = node.Nodes[i];
						int rowI = NodeToRowIndex(cNode);
						//if node not visible downwards break because the nodes after this won't be visible either
						if(VerticallScroll && rowI>lastNode)
						{
							lastChild = cNode;
							break;
						}
						//if node not visible upwards continue because the nodes after might be visible
						//if(VerticallScroll && rowI<VScrollPos) continue;
						//if the plusminus is visible (HasChildren && ShowPlusMinus)
						if(this.transparentControls && cNode.ShouldDrawPlusMinus() )
						{
							Point pt = NodeToPoint(cNode);
							//Set the clip to the rectangle of the plusminus.
							g.SetClip(new Rectangle(lineX-2,pt.Y + (cNode.Height-cNode.PlusMinus.Height)/2,4,cNode.PlusMinus.Height),CombineMode.Exclude);
						}
					}

					//The start Y of the line.
					int lineY1 = 0;
                    if ((level == 0 && VScrollPos == 1) ||
                        (this.NodeToRowIndex(node) >= VScrollPos))
                    {
                        lineY1 = (level == 0 && this.VScrollPos == 1) ?
                        NodeToPoint(node.FirstNode).Y + node.FirstNode.Height / 2 :
                        NodeToPoint(node).Y + node.Height;
                    }

					//The end Y of the line.
					int lineY2 = NodeToPoint(lastChild).Y + lastChild.Height/2;
				
					//Draw the line
					using(Pen p = this.LinePen)
					{
						 if (node.ShowLine )
						{
							g.DrawLine(p,new Point(lineX,lineY1),new Point(lineX,lineY2));
						}
					}
					if(transparentControls)
					{
						//Reset the clip of the  graphics.
						g.ResetClip();
					}
				}
			}			
			int count = node.Nodes.Count;	 
			for(int i=node.Nodes.IndexOf(topVNode);i<count;i++)
			{
				int rowI = NodeToRowIndex(node.Nodes[i]);
				//Testing if during the for statement nodes have passed visible range (bottom) so the method won't get called useless.
				if(VerticallScroll && rowI>lastNode)
				{
					return;
				}
				
				//Draw the lines for the child nodes.
				DrawVerticalLines(g, topCRNode, node.Nodes[i],rowI,lastNode);
			}
		}

        /// <summary>
        /// Updates Layout for CustomControl in all nodes.
        /// </summary>
        private void CustomControlsRefresh()
        {
            if (this.CustomControlCollection.Count > 0)
            {
                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    TreeNodeAdv node = this.Nodes[i];
                    if (node.CustomControl != null)
                    {
                        node.CustomControlCollectionChanging(node, CollectionChangeAction.Refresh);
                    }
                }
            }
        }
	
        /// <summary>
        /// Implements the draw method.
        /// </summary>
        /// <param name="e"></param>
		[Documentation.DocumentationExclude()]
		public void Draw(PaintEventArgs e)
        {
            if( Nodes.Count == 0 )
            {
                this.VerticallScroll = false;
                this.HorisontalScroll = false;
                return;
            }

            Point point = PointToClient( Control.MousePosition );
            bool mouseDown = ( Control.MouseButtons&MouseButtons.Left )==MouseButtons.Left;

            int y = VScrollBar.Value+2;

            int bottom = this.ClientRectangle.Bottom;
            TreeNodeAdv currentNode = PointToNode( this.ClientRectangle.Location );

			DrawSelectionRectangles( e.Graphics, e.ClipRectangle );

			if( this.showLines && currentNode != null )
			{
				TreeNodeAdv lastVisibleNode = this.LastVisibleNode;
				if( lastVisibleNode != null )
					DrawVerticalLines( e.Graphics, currentNode, Root, 0, NodeToRowIndex( lastVisibleNode ) );
			}

            int rowIndex = 0;
            if( currentNode != null )
            {
                y = NodeToPoint( currentNode ).Y; // better would be to have PointToNode return the top ... 

                // Draw each node.
                ArrayList drawedNodes = new ArrayList();

                rowIndex = currentNode.TreeRowIndex;
                while( y < bottom && currentNode != null )
                {
                    int level = currentNode.Level-1;

                    y = DrawNode( e.Graphics, e.ClipRectangle, currentNode, y, level, point, mouseDown, false );

                    if( NeedUpdateCustomControls )
                    {
                        drawedNodes.Add( currentNode );
                    }

                    rowIndex++;
                    //
                    currentNode = this.RowIndexToNode( rowIndex );
                    if( currentNode != null && currentNode.TreeRowIndex != rowIndex )
                        currentNode = this.RowIndexToNode( rowIndex );
                }

                if( this.NeedUpdateCustomControls )
                {
                    this.NeedUpdateCustomControls = false;

                    Hashtable controls = (Hashtable)this.CustomControlCollection.Clone();

                    foreach( DictionaryEntry htElement in controls )
                    {
                        Control control = (Control)htElement.Key;
                        TreeNodeAdv node = (TreeNodeAdv)htElement.Value;

                        if( drawedNodes.Contains( htElement.Value ) )
                        {
                            control.Visible = true;
                            node.UpdateCustomConrtol();
                        }
                        else if( node.Bounds.Y >= e.ClipRectangle.Y )
                        {
                            control.Visible = false;
                        }
                    }
                }
            }

            this.RefreshVScrollbar( currentNode, y );
        }

		[Documentation.DocumentationExclude()]
		internal bool GetIsMirrored()
		{
			return RightToLeft.Yes == RightToLeft;
		}

		[Documentation.DocumentationExclude()]
		protected int MirrorHorizontalPosition( int nX, int nWidth )
		{
			int nBordWidth = this.borderStyle == BorderStyle.Fixed3D ? 2 : 1;
			int nOffset1 = 2*nBordWidth + nWidth + nX;
			
			if (HorisontalScroll)
			{
				nOffset1 -= this.HScrollPos;
			}
			if( VerticallScroll && !this.Office2007ScrollBars )
			{
				nOffset1 += SystemInformation.VerticalScrollBarWidth;
			}

			int nRes = Width - nOffset1;
			
			return nRes;
		}

		/// <summary>
        /// Indicates whether the scroll control can increase the <see cref="ScrollBar.SmallChange"/>. (overridden property)
		/// </summary>
		[DefaultValue(false),
        Description("Specifies if the scroll control can increase the ScrollBar.SmallChange property when doing accelerated scrolling.")]
		public override bool AllowIncreaseSmallChange
		{
			get{return base.AllowIncreaseSmallChange;}
			set{base.AllowIncreaseSmallChange = value;}
		}

		private void UpdateVerticalScrollBar()
		{
			if (VScroll)
			{
				int vPosMin = 1;
				int vPosMax = Math.Max(vPosMin, Root.VisibleNodeCount);

				for (int y = this.ClientSize.Height; y > 0 && vPosMax > vPosMin; vPosMax--)
				{
					TreeNodeAdv node = RowIndexToNode(vPosMax - 1);
					if (node != null)
					{
						y -= node.Height;
						if (y <= 0)
						{
							break;
						}
					}
				}

				VScrollBar.Minimum = vPosMin;
				VScrollBar.Maximum = vPosMax;

				Invalidate();
			}
		}

		private void RefreshVScrollbar()
		{
			this.RefreshVScrollbar( PointToNode(new Point(0, 0)), 0);
		}

		private void RefreshVScrollbar(TreeNodeAdv currentNode, int y)
		{
			if( !VScroll || !this.IsHandleCreated )
				return;

			// init scrollbar values
			if( !inVScroll )
			{
				VScrollBar.Minimum = 1;
				VScrollBar.Maximum = Math.Max( VScrollBar.Minimum, Root.VisibleNodeCount - 1 );
				VScrollPos = VScrollPos;	// Hack for setting correct VScrollPos (including min/max)		
				VScrollBar.Value = VScrollPos;

				int visibleNodesCount = VScrollPos;

				TreeNodeAdv node = null;
				int iNodesHeight = 0;

				do
				{
					visibleNodesCount +=1;
					node = RowIndexToNode( visibleNodesCount );
					if( node != null )
                    {
						iNodesHeight += node.Height;
					}
				}
				while( node != null && iNodesHeight + node.Height < ClientHeight );
				visibleNodesCount -= VScrollPos;

				VScrollBar.LargeChange = visibleNodesCount;
                
				// should scrollbar be shown - check for last visible node
				int height = this.Height; // this.Height includes scrollbars.
				if( currentNode != null && VScrollPos == 1 )
				{
					int rowIndex = currentNode.TreeRowIndex;

					while( y <= height && currentNode != null )
					{
						y += currentNode.Height;
						rowIndex++;
						currentNode = this.RowIndexToNode( rowIndex );
					}
				}

				bool isVScrollBarShown = VerticallScroll;

				if( VScrollPos == 1 && y > height - SystemInformation.HorizontalScrollBarHeight && y <= height )
				{
					// check again if horizontal scroll bar is needed when VScrollbar would not be there.
					if( isVScrollBarShown )
					{
						this.rootMaxXChanged( root.MaxX );
						// If horinzontal scroll bar is not needed in the case when no vertical scrollbar is there
						// then we can also take away vertical scrollbar and heigth increases appropriately.
					}
					if( HorisontalScroll )
						height -= SystemInformation.HorizontalScrollBarHeight;
				}

				bool needsScrolling = y > height || currentNode != null || VScrollPos > 1;

				if( isVScrollBarShown != needsScrolling && !this.FillSplitterPane )
				{
					this.VerticallScroll = needsScrolling;
				}

				this.VScrollBar.Enabled = needsScrolling;

				if( VerticallScroll && !isVScrollBarShown && !isHScrollValueChanged)
					this.rootMaxXChanged( root.MaxX );
				isHScrollValueChanged = false ;
				if( VScrollBar.InnerScrollBar != null )
				{
					IScrollBar scrollBar = VScrollBar.InnerScrollBar as IScrollBar;
					if( scrollBar != null )
					{
						scrollBar.UpdateScrollInfo();
					}
				}
			}
		}
		private bool isHScrollValueChanged = false;
		private void MoveScrollBarOnClick(TreeNodeAdv currentNode)
		{
			if( currentNode == null )
				throw new ArgumentNullException( "currentNode" );

			int yOffset = 0;
			Point currentLocation = NodeToPoint( currentNode );
			TreeNodeAdv childNode;
			Point childLocation;

			// Count nuber of subnodes will be visible.
			for( int i = 0, len = currentNode.Nodes.Count; i < len; i++ )
			{
				// if clicked node is shifted to start - stop lifting it.
				if( currentLocation.Y < 0 ) break;

				childNode	  = currentNode.Nodes[ i ];
				childLocation = NodeToPoint( childNode );

				// If subnode doesn't fit to be shown in the client area
				// we must shift scroll by this node's height.
				if( ( childLocation.Y + childNode.Height ) > this.ClientRectangle.Height )
				{
					yOffset++;
					currentLocation.Y -= childNode.Height;
				}
				
				// if clicked node is shifted to start - stop lifting it.
				if( currentLocation.Y == 0 ) break;
			}

			// there are some subnodes expanded and are likely to be shown.
			if( yOffset > 0 )
			{
				VScrollPos += yOffset;
			}
		}
		private void DrawSelectionRectangles(Graphics g,Rectangle clipRectangle)
		{
			TreeNodeAdv currentNode = this.PointToNode( new Point( clipRectangle.X, clipRectangle.Y ) );
			if( currentNode == null || currentNode == root )
				return;

			int y = NodeToPoint( currentNode ).Y;
			int height = clipRectangle.Bottom;
			int rowIndex = currentNode.TreeRowIndex;

			while( y < height && currentNode != null )
			{
				int level = currentNode.Level-1;

				y = DrawNode( g, clipRectangle, currentNode, y, level, new Point( 0, 0 ), false, true );
				rowIndex++;
				currentNode = this.RowIndexToNode( rowIndex );
				if( currentNode != null && currentNode.TreeRowIndex != rowIndex )
					currentNode = this.RowIndexToNode( rowIndex );
			}

		}
		private Control m_focusSubscribed = null;

		protected override void OnParentChanged( EventArgs e )
		{
			if( m_focusSubscribed != null )
				m_focusSubscribed.GotFocus -= new EventHandler(Parent_GotFocus);

			if( this.Parent != null )
				this.Parent.GotFocus += new EventHandler(Parent_GotFocus);

			m_focusSubscribed = this.Parent;

			base.OnParentChanged(e);
		}

		private void Parent_GotFocus( object sender, EventArgs e )
		{
			if( this.Parent != null	&& this.Parent.Controls.Count == 1 )
				this.Focus();
		}
		/// </override>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint( e );
			this.DrawBackground( e );
			Draw( e );

			if( this.m_needUpdateEditTop )
			{
				this.m_needUpdateEditTop = false;

				if( labelEditor.Visible && this.activeNode != null
			        && labelEditor.Top != this.activeNode.Bounds.Top )
				{
					labelEditor.Top = this.activeNode.Bounds.Top;
				}
			}

		}

		private bool m_bHScroll = true;
		private bool m_bVScroll = true;

		/// <summary>
		/// Enables or disables horizontal scrollbar. 
		/// This property will be set/reset by the tree as and when required.
		/// </summary>	
		protected bool HorisontalScroll
		{
			get
			{
				return base.HScroll;
			}
			set
			{
                base.HScroll = value && !this.UseSharedScrollBars;
			}
		}

		/// <summary>
		/// Enables or disables vertical scrollbar. 
		/// This property will be set/reset by the tree as and when required.
		/// </summary>
		protected bool VerticallScroll
		{
			get
			{
				return base.VScroll;
			}
			set
			{
				base.VScroll = value && !this.UseSharedScrollBars;
			}
		}

		/// <summary>
		/// Overriden. Enables or disables vertical scrollbar.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool VScroll
		{
			get 
			{
				return m_bVScroll;
			}
			set 
			{
                m_bVScroll = value;
                VerticallScroll = m_bVScroll;
			}
		}
		
		/// <summary>
		/// Overriden. Enables or disables horizontal scrollbar.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool HScroll
		{
			get 
			{
				return m_bHScroll;
			}
			set 
			{
				m_bHScroll = value;
				HorisontalScroll = m_bHScroll;
			}
		}

		int _hScrollPos = 1;
		/// <summary>
		/// Gets or sets the position of the Horizontal scrollbar.
		/// </summary>
		[Description("The position of the Horizontal scrollbar")]
		[Category("Scrolling"), Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollPos
		{
			get
			{
				return this.HScrollBar.Enabled ? _hScrollPos : 0;
			}
			set
			{
				if( this._hScrollPos != value )
				{
					NeedUpdateCustomControls = true;
					int width = this._hScrollPos - value;
					_hScrollPos = value;
                	ScrollWindow( width, 0, this.ClientRectangle, this.ClientRectangle, true );
				}
			}
		}
		int _vScrollPos = 1;
		/// <summary>
		/// Gets or sets the position of the Vertical scrollbar.
		/// </summary>
		[Description("The position of the Vertical scrollbar.")]
		[Category("Scrolling"), Browsable(false)]
		[DefaultValue(1)]
		public int VScrollPos
		{
			get
			{
				return _vScrollPos;
			}
			set
			{
				int delta = 0;

				if (this._vScrollPos != value)
				{
					NeedUpdateCustomControls = true;

					if( this.IsEditing )
					{
						cancelEdit = true;
						this.EndEdit( false );
						cancelEdit = false;
					}

					int nValue = value;

					if( nValue < VScrollBar.Minimum )
					{
						nValue = VScrollBar.Minimum;
					}
					else if( nValue > VScrollBar.Maximum )
					{
						nValue = VScrollBar.Maximum;
					}

					delta = _vScrollPos > nValue ? 1 : -1;

					_vScrollPos = nValue;

                    if (VScrollBar.Value != value)
                        VScrollBar.Value = value;

					ScrollWindow(0, delta, this.ClientRectangle, this.ClientRectangle, true);
				}
			}
		}

		/// <summary>
		/// Returns the first fully-visible tree node in the tree view control.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> that represents the first fully-visible 
		/// tree node in the tree view control.</value>
		/// <remarks>
		/// Initially, the <b>TopVisibleNode</b> returns the first root tree node, which is 
		/// located at the top of the <see cref="TreeViewAdv"/>. However, if the user has scrolled 
		/// the contents, another tree node might be at the top.
		/// <seealso cref="TopVisibleNode"/>
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public TreeNodeAdv TopVisibleNode
		{
			get
			{
				return RowIndexToNode(VScrollPos);
			}
		}
		//
		//		public TreeNodeAdv GetNextVisibleNode(TreeNodeAdv node)
		//		{
		//			if (node == null)
		//				return null;
		//
		//			int rowIndex = node.TreeRowIndex;
		//			return Root.GetNodeAtAbsoluteRowIndex(rowIndex+1);
		//		}
		//
		//		public TreeNodeAdv GetNextVisibleNode(TreeNodeAdv node, ref int rowIndex)
		//		{
		//			if (node == null)
		//				return null;
		//
		//			rowIndex++;
		//			return Root.GetNodeAtAbsoluteRowIndex();
		//		}

		/// <summary>
		/// Returns the tree node at the specified point in client co-ordinates.
		/// </summary>
		/// <param name="pt">The point in client co-ordinates.</param>
		/// <returns>A <see cref="TreeNodeAdv"/>.</returns>
		public TreeNodeAdv PointToNode(Point pt)
		{
			int y = 0;
			int rowIndex = VScrollPos;

			TreeNodeAdv node = RowIndexToNode(rowIndex);
			while (node != null)
			{
				y += node.Height;
				if (y > pt.Y)
					return node;
				rowIndex++;
				node = RowIndexToNode(rowIndex);
			}
			return null;
		}

		/// <summary>
		/// Returns the location of the tree node in client co-ordinates.
		/// </summary>
		/// <param name="node">The <see cref="TreeNodeAdv"/> whose location you need.</param>
		/// <returns>A <see cref="System.Drawing.Point"/>.</returns>
		public Point NodeToPoint(TreeNodeAdv node)
		{
			int rowIndex = NodeToRowIndex(node);

			if(node == root)
			{
				if(!VerticallScroll || !root.HasChildren)
					return new Point(0,-root.Height/2);
				else
				{
					if(root.HasChildren)
						return new Point(0,NodeToPoint(RowIndexToNode(1)).Y - root.Height/2);
				}
			}
			if (rowIndex < VScrollPos)
			{
				int y = 0;
				for(int n = VScrollPos;n>rowIndex;n--)
				{
					node = RowIndexToNode(n);
					if( node != null )
					{
						y -= node.Height;
					}
				}
				return new Point(0,y);
				//				return Point.Empty;
			}
			else
			{
				int y = 0;
				for (int n = VScrollPos; n < rowIndex /*&& y < ClientRectangle.Height*/; n++)
				{
					node = RowIndexToNode(n);
					if(node!=null)
					{
						y += node.Height;
					}
				}
				/*				if (y > ClientRectangle.Height)
									return Point.Empty;
				*/
				return new Point(0, y);
			}
		}

		/// <summary>
		/// Returns the total height of the rows from the specified start to end.
		/// </summary>
		/// <param name="start">The top row.</param>
		/// <param name="end">The bottom row.</param>
		/// <returns>The total height.</returns>
		public int GetHeightOfRows(int start, int end)
		{
			int y = 0;
			for (int n = start; n <= end; n++)
			{
				TreeNodeAdv node = RowIndexToNode(n);
				if (node == null)
					break;
				y += node.Height;
			}
			return y;
		}

		/// <summary>
		/// Returns the rectangular area in which the tree node will be drawn.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/>.</param>
		/// <returns>A <see cref="System.Drawing.Rectangle"/>.</returns>
		public Rectangle NodeToRectangle(TreeNodeAdv node)
		{
			Point pt = NodeToPoint(node);
			return new Rectangle(pt, new Size(ClientRectangle.Width, node.Height));
		}

		/// <summary>
		/// Returns the tree node at the specified row index.
		/// </summary>
		/// <param name="rowIndex">The row index.</param>
		/// <returns>A <see cref="TreeNodeAdv"/>.</returns>
		public TreeNodeAdv RowIndexToNode(int rowIndex)
		{
			return Root.GetNodeAtAbsoluteRowIndex(rowIndex);
		}

		/// <summary>
		/// Returns the row index of a tree node.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/>.</param>
		/// <returns>The row index.</returns>
		public int NodeToRowIndex(TreeNodeAdv node)
		{
			return node.TreeRowIndex;
		}

		/// <summary>
		/// Cancels any current mouse based selection and edit mode.
		/// </summary>
		public new void CancelMode()
		{
			this.CancelMouseBasedSelection();
            cancelEdit = true;
			this.EndEdit(true);
            cancelEdit = false;
		}

		bool inVScroll = false;
		bool inHScroll = false;

		/// <summary>
		/// Need update custom controls visibilyti and bounds.
		/// </summary>
		private bool m_bNeedUpdateCustomControls = true;

		/// <summary>
		/// Gets or sets custom controls visibilyti and bounds update need.
		/// </summary>
		internal virtual bool NeedUpdateCustomControls
		{
			get
			{
				return m_bNeedUpdateCustomControls;
			}
			set
			{
				m_bNeedUpdateCustomControls = value;
			}
		}

		/// </override>
		protected override void OnVScrollBarValueChanged(object sender, EventArgs e)
		{
			if (this.VScrollPos != VScrollBar.Value)
			{
				inVScroll = true;
				this.VScrollPos = VScrollBar.Value;
				inVScroll = false;

			}
			base.OnVScrollBarValueChanged (sender, e);
		}
		/// </override>
		protected override void OnHScrollBarValueChanged(object sender,EventArgs e)
		{
			if(this.HScrollPos != HScrollBar.Value)
			{
				inHScroll = true;
				this.HScrollPos = HScrollBar.Value;
				inHScroll = false;
			}
			isHScrollValueChanged = true;
			base.OnHScrollBarValueChanged(sender,e);
		}

//		/// <override/>
//		protected override void OnWindowScrolled(ScrollWindowEventArgs e)
//		{
//			base.OnWindowScrolled(e);
//			int yAmount = Math.Sign(e.YAmount) == -1 ? -e.YAmount : e.YAmount;
//
//			if(yAmount < this.ClientHeight)
//			{
//				int top = 0, bottom = 0;
//				// Some portion of the tree was scrolled without redrawing, so let us update thier bounds
//				if(e.YAmount < 0)
//				{
//					top = 0;
//					bottom = this.ClientHeight - yAmount;
//				}
//				else
//				{
//					top = yAmount;
//					bottom = this.ClientHeight;
//				}
//				int yCur = top;
//				TreeNodeAdv node = PointToNode(new Point(0, yCur));
//				if(node == null)
//					return;

//				int rowIndex = NodeToRowIndex(node);
//				while(node != null && node != this.Root && yCur < bottom)
//				{
//					Point pt = NodeToPoint(node);
//					
//					int nInc = this.gutterSpace - this.HScrollBar.Value;
//					node.iBounds = new Rectangle(
//						nInc, pt.Y,
//						this.ClientRectangle.Width - nInc, node.Height);
//					yCur += node.Height;
//
//					rowIndex++;
//					node = RowIndexToNode(rowIndex);
//				}
//			}
//		}

		private void ValidateScrollPosition()
		{
			if (!ClientRectangle.IsEmpty)
			{
				if(this.Root != null && this.VScrollPos != 1)
				{
					// Determine if top row needs to be adjusted:
					TreeNodeAdv firstVisibleNode = this.TopVisibleNode;
				
					TreeNodeAdv lastVisibleNode = this.LastVisibleNode;

					// If atleast one node
					if(firstVisibleNode != null)
					{
						// Check if this is the last node in the tree:
						int belowLastNodeLocationY = lastVisibleNode.Bounds.Bottom;
						belowLastNodeLocationY += 3;
						TreeNodeAdv belowLastNode = this.PointToNode(new Point(1, belowLastNodeLocationY));
						if(belowLastNode == null)
						{
							// The lastVisibleNode is the last in the tree
							// Check if the empty space can be accomodated by some hidden tree nodes on top
							int htAvailable = this.ClientHeight - lastVisibleNode.Bounds.Bottom;
							int firstVisibleRowIndex = this.NodeToRowIndex(firstVisibleNode);
							int preferredFirstVisibleRowIndex = firstVisibleRowIndex;
							while(htAvailable > 0 && preferredFirstVisibleRowIndex > 1)
							{
								preferredFirstVisibleRowIndex--;
								TreeNodeAdv node = this.RowIndexToNode(preferredFirstVisibleRowIndex);
								htAvailable -= node.Bounds.Height;
							}
							if(htAvailable <= 0)
								preferredFirstVisibleRowIndex++;
							if(firstVisibleRowIndex > preferredFirstVisibleRowIndex)
							{
								// Adjust scroll bar to make hidden rows visible
								this.VScrollPos -= (firstVisibleRowIndex - preferredFirstVisibleRowIndex);
							}
						}
					}
				}
			}
		}

		/// <override/>
		protected override void OnSizeChanged(EventArgs e)
		{
			this.NeedUpdateCustomControls=true;
			this.ValidateScrollPosition();
			base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
            CustomControlsRefresh();
		}

		/// </override>
		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			//	System.Diagnostics.Trace.WriteLine("OnInvalidated" + e.InvalidRect);
			base.OnInvalidated (e);
		}

    /// <summary>
    /// Gets node by path
    /// </summary>
    /// <param name="node"> TreeNodeAdv object, root for search </param>
    /// <param name="path"> Node path. </param>
    /// <returns> TreeNodeAdv object by path. </returns>
		private TreeNodeAdv GetNode(TreeNodeAdv node,string path)
		{
			string name;
			int index = path.IndexOf(this.PathSeparator);
      		if( index == -1 )
        		name = path;
      		else
        		name = path.Substring( 0, index );
            
			for(int i=0;i<node.Nodes.Count;i++)
			{
				if(node.Nodes[i].Text == name)
				{
          if( (path == name) || ( AddSeparatorAtEnd && path == name + this.PathSeparator ) )
          {
            return node.Nodes[ i ];
          }
					
          string newPath = path.Substring(index+1,path.Length-index-1);
          TreeNodeAdv ret = GetNode( node.Nodes[ i ], newPath );

          if( ret != null )
          {
            return ret;
          }
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a node from the specified path. Make sure that the path does not end with a separator when calling this.
		/// </summary>
		/// <param name="path">The path of the node.</param>
		/// <returns>The node that has the specified path.</returns>
		public TreeNodeAdv GetNodeFromPath(string path)
		{
			return GetNode(root,path);
		}

		/// <summary>
		/// Returns the path of the specified node.
		/// </summary>
		/// <param name="node">Node whose path is to be returned.</param>
		/// <returns>The path of the node.</returns>
		public string GetPathFromNode(TreeNodeAdv node)
		{
			if(node == null) return "";
			return node.GetPath(pathSeparator);
		}

		private void TreeViewAdv_SizeChanged(object sender, System.EventArgs e)
		{
			this.rootMaxXChanged(root.MaxX);
			//RefreshBrush();
			Invalidate();
		}
		private void selectedNodes_Changed(object sender,CollectionChangeEventArgs e)
		{
			if(e.Action == CollectionChangeAction.Add
				&& e.Element != null)
			{
				TreeNodeAdv node = e.Element as TreeNodeAdv;
				if(node.TreeView != this)
					throw new ArgumentException("A TreeNodeAdv that was not a child of the TreeViewAdv was added to the TreeViewAdv's SelectedNodes collection. This is not allowed.");

				this.LMouseDownNode = this.RMouseDownNode = null;
			}
            else if (e.Action == CollectionChangeAction.Remove
                && e.Element != null)
            {
                this.clickedOnSelection = false;
            }

			Invalidate();
		}

		private bool CheckToolTipClicked( Point mousePos )
		{
			if( toolTip == null || !toolTip.IsShowing() ) return false;
			
			Point ptScreen = this.PointToScreen( mousePos );
			Rectangle rcWindow = this.toolTip.Parent.RectangleToScreen( this.toolTip.Bounds );

			return rcWindow.Contains(ptScreen);
		}
		
		private bool m_bIsKeyDown = false; 
		private bool m_bIsMouseDown = false;
        private bool IsLeftMouseDown = false;

		/// <summary>
		/// 
		/// </summary>
	    /// <param name="sender"></param>
	    /// <param name="e"></param>
        protected virtual void FocusInternal(object sender, EventArgs e)
        {
            this.Focus();
            if (!this.Focused)
            {
                focusFailedOnValidation = true;
            }
        }        

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1 && this.GetNodeAtPoint(e.X, e.Y) != null)
            {
                this.RaiseNodeSingleClick(this.GetNodeAtPoint(e.X, e.Y), e.Button, e.Clicks, e.X, e.Y, e.Delta);
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 2 && this.GetNodeAtPoint(e.X,e.Y)!= null)
            {
                this.RaiseNodeDoubleClick(this.GetNodeAtPoint(e.X, e.Y), e.Button, e.Clicks, e.X, e.Y, e.Delta);
            }
           
			if((e.Button & MouseButtons.Right) > 0)
				this.RMouseDownNode = this.PointToNode(new Point(e.X, e.Y));
			//			else if(e.Button != MouseButtons.None)
			//				this.RMouseDownNode = null;

            if (this.focusFailedOnValidation)
            {
                focusFailedOnValidation = false;
                return;
            }

			base.OnMouseDown(e);
            if (IsEditing)
                return;
			m_bIsMouseDown = true;

			dragging = false;
			clickedOnSelection = false;
			//Enabled dragging on mouse move.
			canDrag = false;                    
			Point pt = new Point(e.X,e.Y);
			this.mouseDownPoint = pt;
			m_lastButtonDownPoint = pt;
			m_lastButtonDownNode = GetNodeAtPoint( pt );
			bool newlyFocused = !this.ContainsFocus;

			bool leftButtonClick = e.Button==MouseButtons.Left;
			bool rightButtonClick = e.Button==MouseButtons.Right;
            if (leftButtonClick || rightButtonClick)
            {
                OnClick(e);
                OnMouseClick(e);
            }
            IsLeftMouseDown = leftButtonClick;

			//Tells the node to process the click.
            TreeNodeAdv nodeAtPt = GetNodeAtPoint(pt);
			if( nodeAtPt == null )
			{
				nodeAtPt = this.Root;
			}
			if(leftButtonClick && nodeAtPt.ProcessMouseDown(pt))
			{
				m_bIsMouseDown = false;
				return;				
			}
           
			bool prevFocus = this.Focused;

			// If some other control fails validating and takes away the focus, then just return
			if( prevFocus && !this.Focused )
			{				
				m_bIsMouseDown = false;
				return;
			}

           

       		if( !this.Focused && this.CanFocus)
      		{	
                if (this.Parent.Parent == null)
                    this.FocusInternal(this, EventArgs.Empty);
                else
                    this.BeginInvoke(new EventHandler(FocusInternal));
      		}
            if (this.focusFailedOnValidation)
            {
                focusFailedOnValidation = false;
                return;
            }
            
			bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
			bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			bool continueProcessing = true;
			if(leftButtonClick && activeNode != null)
			{
				bool bToolTipClicked = CheckToolTipClicked( pt );

				if(e.Clicks == 2)
				{
					continueProcessing = false;
				}
				else if ( e.Clicks == 1 )
				{
					if( bToolTipClicked )
					{
                        this.ActiveNode = GetNodeAtPoint(pt);
 						this.OnClick( e );
#if SyncfusionFramework2_0
                        this.OnMouseClick(e);
#endif
					}
				}
			}
			
			if( e.Clicks == 2 )
			{
				OnMouseUp( e );
				m_bIsMouseDown = false;
				return;
			}

			if(continueProcessing)
			{
				// Check if we should turn on mouse based selection:
				if(leftButtonClick && this.AllowMouseBasedSelection)
				{
					TreeNodeAdv mouseDownNode = this.GetNodeAtPoint(pt, false, !this.fullRowSelect);
					if( mouseDownNode == null )
					{
						mouseDownNode = this.Root;
					}
					if(mouseDownNode != activeNode && !this.SelectedNodes.Contains(mouseDownNode))
						this.MouseBasedSelectionOn = true;
				}

				//If NO Ctrl || Shift
				if(!bCtrl && !bShift)
				{
					if(leftButtonClick)
					{
						// Clicked on active node
						if(activeNode!=null && activeNode.Bounds.Contains(pt))
						{
							if(activeNode.TextBounds.Contains(pt)
								|| this.FullRowSelect)
							{
								if(!newlyFocused)
									// Can start the edit mode.
									this.clickedOnSelection = true;
								// Can start a drag
								canDrag = true;
							}
							else if(!dragOnText)
								canDrag = true;
						}
					}
					else if(rightButtonClick)
					{
						if(this.RMouseDownNode != null && this.RMouseDownNode.Bounds.Contains(pt))
						{
							if(!dragOnText
								|| RMouseDownNode.TextBounds.Contains(pt)
								|| this.FullRowSelect)
								canDrag = true;
						}
					}
				}
				if( !leftButtonClick )
				{
                    m_lastSelectedNode = GetNodeAtPoint(pt);
                    this.ensureVisible = false;

                    if (m_lastSelectedNode != null)
                    {
                        if (m_lastSelectedNode != this.ActiveNode && rightButtonClick && this.toolTip != null && this.toolTip.IsShowing())
                        {
                            this.toolTip.HidePopup(PopupCloseType.Deactivated);
                        }
                        
                        this.ApplyMouseBasedSelectionOn(m_lastSelectedNode);
                    }

                    this.ensureVisible = true;
                    this.RMouseDownNode = m_lastSelectedNode;
                    m_lastSelectedNode = null;
					m_bIsMouseDown = false;
					return;
				}

				if(continueProcessing)
				{
					// Clicked on selection nodes WITHOUT Ctrl || Shift
					TreeNodeAdv nodeAtPoint = this.GetNodeAtPoint(pt);
					if( nodeAtPoint == null )
					{
						nodeAtPoint = this.Root;
					}
					if( selectedNodes.Contains( nodeAtPoint ) && !bCtrl && !bShift )
					{
                        if (!newlyFocused)
                            //Need set to true to change selection, doesn't depend wheather clicking on text area or not.
                            this.clickedOnSelection = true;

						if(!dragOnText || (this.dragOnText && nodeAtPoint.TextBounds.Contains(pt))
							|| this.FullRowSelect)
						{
							//canDrag = this.enableDragDrop;
							canDrag = true;
						}
					}
					else
					{
						TreeNodeAdv retNode = this.GetNodeAtPoint(pt, false, !fullRowSelect);
						// Mouse down on a new node.
						if( retNode!=null && retNode.Enabled )
						{
							if(!dragOnText || (dragOnText && retNode.TextBounds.Contains(pt))
								|| this.FullRowSelect)
							{
								//canDrag = enableDragDrop;
								canDrag = true;
							}
								
							#region /* comments */
							// Condition below was added by Lucas in order to resolve problem # 196
							// ( the clicked node ( if ctrl prssed ) should 
							// become the new selected node, just like in the .NET TreeView )
							#endregion

							if( this.SingleSelect && bCtrl )
							{
								clickedOnSelection = true;
							}

							// If NO Ctrl & Shift, Add to selection only the clicked node
							if((!bCtrl && !bShift) || this.SingleSelect)
							{
								this.lmousedownnode = retNode;

								#region /* comments */
								// This snippet of code was commented by Lucas :
								// it calls invalidation inside property definition
								// and user can see "effect of non removed dotted rectangle"
								//LMouseDownNode = retNode;
								#endregion
							}
								// Control pressed or Shift and Control both pressed
							else if(!bShift ||(bCtrl && bShift))
							{
								// For all cases
								this.lmousedownnode = retNode;
							}
								// Shift pressed
							else
							{
								bool removeCurrentMultipleSelection = !bCtrl;
								//Scroll position should not move while selecting node by mouse
                                bKeypressed = Keys.None;
                                this.ensureVisible = false;
								this.ExtendSelectionTo(retNode, removeCurrentMultipleSelection);
                                this.ensureVisible = true;
							}
						}

						this.StartAutoScrollingInternal();
					}
                    if (leftButtonClick)
                    {
                        m_lastSelectedNode = GetNodeAtPoint(pt);
                        this.LMouseDownNode = m_lastSelectedNode;
                        this.ensureVisible = false;
                        if (m_lastSelectedNode != null /*&& !m_lastSelectedNode.IsSelected*/)
                        {

                            if (m_lastSelectedNode != this.ActiveNode && this.toolTip != null && this.toolTip.IsShowing())
                            {
                                this.toolTip.HidePopup(PopupCloseType.Deactivated);
                            }
                            this.ApplyMouseBasedSelectionOn(m_lastSelectedNode);
                        }
                        this.ensureVisible = true;
                        m_lastSelectedNode = null;
                        m_bIsMouseDown = false;
                    }
				}
			}
          
		}
		
		private bool m_bIsMouseUp = false;
		private TreeNodeAdv m_lastSelectedNode = null;
        private bool ensureVisible = true;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
            if (e.Button == MouseButtons.Right && activeNode != null)
            {
                bool bToolTipClicked = CheckToolTipClicked(new Point(e.X, e.Y));

                if (e.Clicks == 1)
                {
                    if (bToolTipClicked)
                    {
                        this.OnClick(e);
#if SyncfusionFramework2_0
                        this.OnMouseClick(e);
#endif
                    }
                }
                
            }

            m_bIsMouseUp = true;
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
			// This code snippet was added by Lucas in order to fix problem 169
			/**/
			if( m_bAllowDropStubWorks )
			{
				this.draggedTnas = null;
				m_bAllowDropStubWorks = false;
				TreeNodeDragHelper.EndDrag();
			}
			/**/

			m_highlightedNode    = null;
			m_lastButtonDownNode = null;

			this.MouseBasedSelectionOn = false;

			

			if(e.Button!=MouseButtons.Left)
			{
				dragging = false;
				Invalidate();
				this.mouseDownPoint = Point.Empty;
				// NOTE: fixed because right button doesn't handle.
				base.OnMouseUp(e);

				m_bIsMouseUp = false;
                if ((e.Button & MouseButtons.Right) > 0)
                    this.RMouseDownNode = null;
				return;
			}
           

			Point pt = new Point(e.X,e.Y);

			//If it was clicked on selection update selection on mouse up in case of dragging begin.
			if(clickedOnSelection && this.mouseDownPoint == pt)
			{
				TreeNodeAdv node = this.GetNodeAtPoint(pt);
				if(node != null)
				{
					ArrayList newNodes = new ArrayList(new TreeNodeAdv[]{node});
                    if (this.SetSelectedNode(newNodes, this.selectedNodes, TreeViewAdvAction.ByMouse, true, true) )
					{
						this.ActiveNode = node;
						this.selectionBaseNode = node;

                        if( !bCtrl && node.TextBounds.Contains( pt ) )
                        {
                            this.LabelEditStartTimer = new Timer();
                        }
					}
				}
			}
            
            if (!bShift || (bCtrl && bShift))
            {
                if (this.MultiSelect)
                {
                    TreeNodeAdv node = this.GetNodeAtPoint(pt);
                    //Add/Remove to the SelectedNodes.
                    if (selectedNodes.Contains(node) && IsMouseDownWithCtrl)
                    {
                        if (this.SetSelectedNode(null, node, TreeViewAdvAction.ByMouse))
                            this.ActiveNode = node;
                        IsMouseDownWithCtrl = false;
                    }
                }
            }
			this.LMouseDownNode = this.RMouseDownNode = null;

			this.mouseDownPoint = Point.Empty;

			this.StopAutoScrollingInternal();      

			base.OnMouseUp(e);
			m_bIsMouseUp = false;
		}

		private void ApplyMouseBasedSelectionOn(TreeNodeAdv node)
		{
			bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
			bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			if(!bCtrl && !bShift)
			{
                if (!(this.MultiSelect && selectedNodes.Contains(node)))
                {
                    // Remove every thing else.
                    if (this.SetSelectedNode(node, this.selectedNodes, TreeViewAdvAction.ByMouse))
                    {
                        this.ActiveNode = node;
                        this.selectionBaseNode = node;
                    }
                }
			}
				// Control pressed or Shift with Control key pressed
			else if(!bShift ||(bCtrl && bShift))
			{
				if(this.MultiSelect)
				{
					//Add/Remove to the SelectedNodes.
                    if (selectedNodes.Contains(node) && !bCtrl) 
					{
						if(this.SetSelectedNode(null, node, TreeViewAdvAction.ByMouse))
							this.ActiveNode = node;
					}
                    else /*if(!bCtrl)*/
					{
						bool allSelect = this.SelectionMode == TreeSelectionMode.MultiSelectAll;

						TreeNodeAdv firstSelNode = null;
						if(this.SelectedNodes.Count > 0)
							firstSelNode = this.SelectedNodes[0];
                        if (this.selectedNodes.Count > 0 && this.selectedNodes.Contains(node) && bCtrl)
                        {
                            IsMouseDownWithCtrl = true;
                            return;
                        }
                        else
                            IsMouseDownWithCtrl = false;
						if(firstSelNode == null
							|| allSelect
							|| firstSelNode.Parent == node.Parent
							)
						{
							if(this.SetSelectedNode(node, new ArrayList(), TreeViewAdvAction.ByMouse))
								this.ActiveNode = node;
						}
                        
					}
					if(this.ActiveNode == node)
						this.selectionBaseNode = node;
                   
				}
			}
		}

        private bool IsMouseDownWithCtrl = false;
		/// </override>
		protected override void OnDoubleClick( EventArgs e )
		{
			Point pt = PointToClient( Cursor.Position );
			TreeNodeAdv node = GetNodeAtPoint( pt );

			if( node != null )
			{
				CheckBoxPart checkBox = node.CheckBox as CheckBoxPart;

				if( node.PlusMinus.Visible
					&& !(null != checkBox && checkBox.Visible && node.EnabledButtons && checkBox.Bounds.Contains( pt )) )
				{
					Rectangle pmBounds = node.PlusMinus.Bounds;
					// Provide some leeway around the bounds.
					pmBounds.Inflate( 4, 3 );

					if( !pmBounds.Contains( pt ) )
					{
						// fix for 2409
						if( (this.IsLeftMouseDown && node.Bounds.Contains( pt )) && (node.HasChildren || this.LoadOnDemand) )
						{
							node.Expanded = !node.Expanded;
						}
					}
				}
			}
			ClearSelectionClickMonitors();
			base.OnDoubleClick( e );
		}

		private void ClearSelectionClickMonitors()
		{
			this.LabelEditStartTimer = null;
			this.clickedOnSelection = false;
			this.mouseDownPoint = Point.Empty;
		}
		
		/// </override>
		protected internal void SetSelectionBaseNode(TreeNodeAdv node)
		{
			this.selectionBaseNode = node;
		}

		/// <override/>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.Root.RecalculateAllDimensions();
		}
		private Timer LabelEditStartTimer
		{
			get
			{
				return this.labelEditStartTimer;
			}
			set
			{
				if(this.labelEditStartTimer != null)
				{
					this.labelEditStartTimer.Tick -= new EventHandler(this.LabelEditStartTimer_Tick);
					this.labelEditStartTimer.Enabled = false;
				}
				if(this.ActiveNode == null || !this.LabelEdit)
					value = null;
				this.labelEditStartTimer = value;
				if(this.labelEditStartTimer != null)
				{
					this.labelEditStartTimer.Tick += new EventHandler(this.LabelEditStartTimer_Tick);
					this.labelEditStartTimer.Interval = SystemInformation.DoubleClickTime;
					this.labelEditStartTimer.Enabled = true;
					this.activeNodeForLabelEdit = this.ActiveNode;
				}
			}
		}
		private void LabelEditStartTimer_Tick(object sender, EventArgs e)
		{
			if(this.ActiveNode == this.activeNodeForLabelEdit)
			{
				this.LabelEditStartTimer = null;
				this.BeginEdit(this.ActiveNode);
			}
		}
		/// <summary>
		/// Overloaded. Extends the selection to the specified node.
		/// </summary>
		/// <param name="selNode">A TreeNodeAdv.</param>
		/// <remarks>This method will not do anything if the <see cref="SelectionMode"/>
		/// property is set to <b>TreeSelectionMode.Single</b>.</remarks>
		public void ExtendSelectionTo(TreeNodeAdv selNode)
		{
			this.ExtendSelectionTo(selNode, true);
		}
		/// <summary>
		/// Extends the selection to the specified node.
		/// </summary>
		/// <param name="selNode">A TreeNodeAdv.</param>
		/// <param name="removeCurrentMultipleSelection">Indicates whether or not any current selection should be removed.</param>
		/// <remarks>This method will not do anything if the <see cref="SelectionMode"/>
		/// property is set to <b>TreeSelectionMode.Single</b>.</remarks>
		public void ExtendSelectionTo(TreeNodeAdv selNode, bool removeCurrentMultipleSelection)
		{
			if(this.SelectionMode == TreeSelectionMode.Single)
				return;

			if((this.selectionBaseNode != null) && selNode != null)
			{
				bool allSelect = this.SelectionMode == TreeSelectionMode.MultiSelectAll;

				if(allSelect || (selNode.Parent == this.selectionBaseNode.Parent))
				{
                    

					ArrayList newNodes = new ArrayList();
					TreeNodeAdv tna;

					if (this.Nodes.Contains(selectionBaseNode))
						tna = selectionBaseNode;
					else if (this.SelectedNodes.Count > 0)
					{
						selectionBaseNode = tna = this.SelectedNodes[0];
					}
					else
						selectionBaseNode = tna = selNode;

					//If nodes has been selected by mouse
                    if (bKeypressed == Keys.None)
                    {
                        selectUpwardDirection = selectionBaseNode.Bounds.Y >= selNode.Bounds.Y;
					while(tna!=null && tna!=selNode)
					{
						if(tna.Enabled && (allSelect || tna.Parent == selNode.Parent))
							newNodes.Add(tna);
                            tna = (selectUpwardDirection ? tna.PrevVisibleNode : tna.NextVisibleNode);
                        }
                    }
                    else
                    {
                        while (tna != null && tna != selNode)
                        {
                            if (tna.Enabled && (allSelect || tna.Parent == selNode.Parent))
                                newNodes.Add(tna);
                            tna = (selectUpwardDirection ? tna.PrevVisibleNode : tna.NextVisibleNode);
                        }
					}
					newNodes.Add(selNode);
					//If we deselect a node by Up/Down arrow key the previously selected node should be removed from selected nodes list.
                    if (this.selectedNodes.Contains(selNode) && (bKeypressed==Keys.Up||bKeypressed==Keys.Down))
                    {
                        if(selectUpwardDirection)
                            this.selectedNodes.Remove(selNode.NextVisibleNode);
                        else
                            this.selectedNodes.Remove(selNode.PrevVisibleNode);
                        EnsureVisible(selNode);
                        newNodes = this.selectedNodes;
                        bKeypressed = Keys.None;
                    }
					ArrayList nodesToRemove = null;
					if(removeCurrentMultipleSelection)
					{
						if(this.MouseBasedSelectionOn)
							nodesToRemove = this.latestMouseBasedSelectionCollection;
						else
							nodesToRemove = this.SelectedNodes;
					}
					// Cache the latest set of selected nodes if this is mouse-based selection.
					if(this.MouseBasedSelectionOn)
						this.latestMouseBasedSelectionCollection = new ArrayList(newNodes);

					if(this.SetSelectedNode(newNodes, nodesToRemove, TreeViewAdvAction.ByMouse))
						this.ActiveNode = selNode;
				}
			}
		}

		TreeNodeAdv lastNodeOver = null;
		private bool IsDraggedPastMouseDownPoint(Point pt)
		{
			if(this.mouseDownPoint == Point.Empty)
				return true;

			if(Math.Abs(this.mouseDownPoint.X - pt.X) > 2
				|| Math.Abs(this.mouseDownPoint.Y - pt.Y) > 2)
				return true;
			else
				return false;
		}

		/// <override/>
		protected override void OnMouseEnter(EventArgs e)
		{
            this.m_bMouseLeaved = false;
			base.OnMouseEnter (e);
		}

		/// <override/>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			// This condition was added by Lucas in order
			// to fix issue 169
			if( m_bAllowDropStubWorks )
			{
				GiveFeedbackEventArgs ea =
					new GiveFeedbackEventArgs( DragDropEffects.None, true );

				OnGiveFeedback( ea );

                //Fix for defect #1247: TreeViewAdv has apparently not notified that the "drag and drop" operation has ended.
                this.draggedTnas = null;
                m_bAllowDropStubWorks = false;
                TreeNodeDragHelper.EndDrag();

				return;
			}

            Point mousept = new Point(e.X, e.Y);
            TreeNodeAdv nodeAtPt = GetNodeAtPoint(mousept);
            if (nodeAtPt == null)
            {
                nodeAtPt = this.Root;
            }

			// Call this method to continue listening to MouseHover, otherwise
			// MouseHover doesn't occur until you move out of the bounds and come back in.
			this.ResetMouseEventArgs();

			if(e.Button == MouseButtons.None && this.ContextMenu != null)
			{
				// If the user showed a context menu on mouse_down then we don't get a mouseUp where we could
				// have turned this off.
				this.RMouseDownNode = null;
				this.LMouseDownNode = null;
			}

			if(this.ignoreNextMouseMove)
			{
				// When the help-text tooltip is shown, that immediately generates a MouseMove on the tree.
				this.ignoreNextMouseMove = false;
				return;
			}
			if(this.helpText!=null && this.helpText.IsShowing())
			{
				if( this.m_tnHelpTextNode != null )
				{
					if( !this.m_tnHelpTextNode.TextBounds.Contains( e.X, e.Y ) )
					{
						this.helpText.HidePopup();
						this.m_tnHelpTextNode = null;
					}
				}
				else
				{
					this.helpText.HidePopup();
				}
			}
			if(e.Button  == MouseButtons.Left)
			{
				TreeNodeAdv[] tnas = null;

				if(this.IsDraggedPastMouseDownPoint(new Point(e.X, e.Y)))
				{
					if( activeNode != root && !dragging && canDrag )
					{
						// Determine the data for ItemDrag:
						
						bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
						bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

						if(this.LMouseDownNode != null && ( this.SingleSelect) )
						{
							tnas = new TreeNodeAdv[]{this.LMouseDownNode};
						}
						else if(this.MultiSelect && this.selectedNodes.Count > 0)
						{
							ArrayList tnasArray = new ArrayList( this.selectedNodes.ToArray( typeof( TreeNodeAdv ) ) );

							if( this.LMouseDownNode != null )
							{
                               	bool needAdd = true;
								if( this.SelectionMode == TreeSelectionMode.MultiSelectSameLevel )
								{
									TreeNodeAdv treeNode = this.selectedNodes[0];
									needAdd = ( treeNode.parent == this.LMouseDownNode.parent );
								}
                                if (tnasArray.Contains(this.LMouseDownNode))
                                    needAdd = false;

								if( needAdd )
								{
                                    tnasArray.Add(this.LMouseDownNode);
								}
							}
							tnas = tnasArray.ToArray( typeof( TreeNodeAdv ) ) as TreeNodeAdv[];
							
						}
						else if(this.SingleSelect && this.SelectedNode != null)
						{
							tnas = new TreeNodeAdv[]{this.SelectedNode};
						}
					
						TreeNodeAdv prevLMouseDownNode = this.LMouseDownNode;
						this.LMouseDownNode = null;

						if(tnas != null)
						{
							// Let the user begin a drag and drop.
							this.RaiseItemDrag(new ItemDragEventArgs(e.Button,tnas));
							// This happens when sometimes OnQueryFeedback doesn't get called.
							if(this.DragCueOn)
							{
								this.DragCueOn = false;
								this.TreeNodeDragHelper.EndDrag();
							}

							// Otherwise the tree thinks that we are still dragging even after an Esc.
							canDrag = false;

							// If no dragging took place:
							if(this.MouseBasedSelectionOn)
								// ProcessMouseBasedSelection will reset this anyway,
								// but a paint occurs before the reset happens and the user
								// see a flicker. We do this to avoid that flicker.
								this.LMouseDownNode = prevLMouseDownNode;
						}
					}

					if(this.MouseBasedSelectionOn)
					{
						this.ProcessMouseBasedSelection(e);
					}
					else
						this.LMouseDownNode = null;
				}
			}
			else if(e.Button == MouseButtons.Right)
			{
				if(this.IsDraggedPastMouseDownPoint(new Point(e.X, e.Y)))
				{
					if(!dragging && canDrag
						&& this.RMouseDownNode != null
						)
					{
						TreeNodeAdv[] tnas = null;
						if(this.selectedNodes.Contains(this.RMouseDownNode))
							tnas = this.selectedNodes.ToArray(typeof(TreeNodeAdv)) as TreeNodeAdv[];
						else
							tnas = new TreeNodeAdv[]{this.RMouseDownNode};

						// Let the user begin a drag and drop.
						this.RaiseItemDrag(new ItemDragEventArgs(e.Button,tnas));
						// Otherwise the tree thinks that we are still dragging even after an Esc.
						canDrag = false;		
					}
					//this.RMouseDownNode = null;
				}
			}

			// Invalidate mouse over nodes if hottracking is enabled.
			if(this.HotTracking)
			{
				TreeNodeAdv lastVisibleNode = this.LastVisibleNode;

				int y = 0;
				
				if(lastVisibleNode != null)
					y = lastVisibleNode.Bounds.Bottom;

				Point pt = new Point(e.X, e.Y);

				TreeNodeAdv node = null;
				if(pt.Y <= y)
					// Call this sparingly. Calling this only if the mouse is above the last visible node.
					node = GetNodeAtPoint(pt);

				if(node != lastNodeOver)
				{
					if(lastNodeOver!=null)
					{
						// Clear previous hot-tracking node:
						Invalidate(lastNodeOver.Bounds);
					}
					lastNodeOver = node;
				}
				if(node!=null)
				{
					// Redraw current hot-tracking node.
					Invalidate(node.Bounds);
                    RaiseNodeHotTracked();
				}
			}

            if (this.Style == TreeStyle.Metro && this.ShowPlusMinus)
            {
                TreeNodeAdv lastVisibleNode = this.LastVisibleNode;
                int y = 0;

                if (lastVisibleNode != null)
                    y = lastVisibleNode.Bounds.Bottom;

                Point pt = new Point(e.X, e.Y);

                TreeNodeAdv node = null;
                if (pt.Y <= y)
                    node = GetNodeAtPoint(pt);

                if (node != lastNodeOver)
                {
                    if (lastNodeOver != null)
                    {
                        lastNodeOver.PlusMinusArrowColor = Color.Black;
                        Invalidate(lastNodeOver.Bounds);
                    }
                    lastNodeOver = node;
                }
                if (node != null)
                {
                    nodeAtPt.ProcessPlusMinusColor(nodeAtPt, mousept);
                    Invalidate(node.Bounds);
                }
            }
		}
		/// <override/>
		protected override void OnMouseLeave(EventArgs e)
		{
			m_bMouseLeaved = true;
			// Clear hot-tracking node, if any.			
			if(this.lastNodeOver != null)
			{
                this.lastNodeOver.PlusMinusArrowColor = Color.Black;
				this.Invalidate(this.lastNodeOver.Bounds);
				this.lastNodeOver = null;
			}
			base.OnMouseLeave(e);

			if (Control.MouseButtons == MouseButtons.None) // Make sure this is not called from treeViewAdv.DoDragDrop
				this.StopAutoScrollingInternal();
		}
		/// <summary>
		/// Begins a drag-and-drop operation.
		/// Added by lucas in order to resolve problem 169.
		/// </summary>
		/// <param name="data">The data to drag.</param>
		/// <param name="allowedEffects">One of the DragDropEffects values.</param>
		/// <returns>A value from the DragDropEffects enumeration that
		///  represents the final effect that was performed
		///  during the drag-and-drop operation.</returns>
		public new DragDropEffects DoDragDrop( object data,	DragDropEffects allowedEffects )
		{
			canDrag = false;
			DragDropEffects result = DragDropEffects.None;
			result = base.DoDragDrop( data, allowedEffects );

			if( !AllowDrop )
			{
				// This snippet of code was added by Lucas i order to fix
				// issue # 169 
				m_bAllowDropStubWorks = true;		
				result = DragDropEffects.None;
			}

			return result;
		}
        /// <summary>
        /// This method indicates that the multipleNodes can be selected with mouseDown and drag .
        /// </summary>
        /// <param name="e"></param>
		protected virtual void ProcessMouseBasedSelection(MouseEventArgs e)
		{
			TreeNodeAdv mouseOverNode = this.GetNodeAtPoint(new Point(e.X, e.Y), false);

			if(mouseOverNode == null || mouseOverNode == this.Root || mouseOverNode.Enabled == false)
				return;

			if(this.SingleSelect)
			{
				this.LMouseDownNode = mouseOverNode;
			}
			else
			{
				if(LMouseDownNode != null)
				{
					this.ApplyMouseBasedSelectionOn(this.LMouseDownNode);
					this.LMouseDownNode = null;
					// But continue selection:
					//this.MouseBasedSelectionOn = false;
				}
				else
				{
					this.ExtendSelectionTo(mouseOverNode, true);
				}
			}
		}

        private TreeNodeAdv m_tnHelpTextNode = null;
        [Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateTips(bool mouseHover)
		{
			if (this.helpText == null)
				CreateHelpText();
			if (this.toolTip == null)
				CreatetoolTip();
			TreeNodeAdv node = null;
			bool hidePopups = false;
			// Don't show tooltips if in edit mode.
			// hide tooltip when we have't focus and mouse is not hover.
			if( !( this.ContainsFocus || this.Focused || mouseHover ) || this.IsEditing )
			{
				hidePopups = true;
			}

			// If this is not called form mouse-hover and if the parent form
			// doesn't have focus:
			Form form = this.FindForm();
			// Form.ContainsFocus returns true even if a dialog is currently open!
			// So the tips stay open even if a MessageBox is shown.
			if(!mouseHover && form != null && !form.ContainsFocus)
				hidePopups = true;

			if(!hidePopups)
			{
				Point ptClient = PointToClient(Control.MousePosition);
				// Show tooltips only if the mouse hovers over text.
				node = GetNodeAtPoint(ptClient, true, false);
				if(node == null
					|| node == root) 
				{
					hidePopups = true;
				}
			}

			if(hidePopups)
			{
				this.helpText.HidePopup();
				this.toolTip.HidePopup();
                if (this.timer.Enabled)
                {
                    this.timer.Tick -= new EventHandler(timer_Tick);
                    this.timer.Stop();
                }
				return;
			}

			Rectangle rc = new Rectangle(node.TextBounds.Location,node.TextBounds.Size);
			rc.Intersect(ClientRectangle);

			if(node.TextBounds != rc)
			{
                if (this.toolTip.IsDisposed)
                {
                    ResetToolTip();
                }
                
				if(!this.toolTip.IsShowing())
				{
					this.toolTip.Font = node.Font;
					this.toolTip.Text = node.Text;
                    toolTipNode = node;
                    if (this.delayToolTip)
                    {
                        this.timer.Interval = 1000;
                        this.timer.Tick += new EventHandler(timer_Tick);
                        this.timer.Start();
                    }
                    else
                    {
                        this.toolTip.ShowPopup(PointToScreen(node.TextBounds.Location));
                    }
				}
				this.toolTip.CurrentPopupChild = this.helpText;
				this.helpText.PopupParent = this.toolTip;
			}
			if(node.HelpText != String.Empty)
			{
				// Reset the PopupParent relationship if the toolTip is not showing.
				if(!this.toolTip.IsDisposed && !this.toolTip.IsShowing())
					this.helpText.PopupParent = null;
                
                if (this.helpText.IsDisposed)
                {
                    ResetHelpText();
                }

				if( !this.helpText.IsShowing() )
				{
					this.helpText.Text = node.HelpText;
					Point pt = new Point(Control.MousePosition.X,Control.MousePosition.Y+30);
					this.ignoreNextMouseMove = true;

					m_tnHelpTextNode = node;

					this.helpText.ShowPopup(pt);
				}
			}
		}

        void timer_Tick(object sender, EventArgs e)
        {
            this.toolTip.ShowPopup(PointToScreen(toolTipNode.TextBounds.Location));
            this.timer.Tick -= new EventHandler(timer_Tick);
            this.timer.Stop();
        }

		/// <summary>
        /// Performs actions required on mousehover. (Overridden method)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseHover(System.EventArgs e)
		{
			base.OnMouseHover(e);
			try
			{
				this.UpdateTips(true);
			}
			finally
			{
				// Commented out on 7/26/04. Moved this to OnMouseMove, otherwise, MouseHover gets call multiple times for each hover.

				// Call this method to continue listening to MouseHover, otherwise
				// MouseHover occurs only once.
				//this.ResetMouseEventArgs();
			}
		}
		
        /// <summary>
        /// Performs actions required on setting up the cursor. (Overridden method)
        /// </summary>
        /// <param name="m"></param>
		protected override void OnSetCursor(ref Message m) 
		{
			Cursor.Current = this.Cursor;
		}

		#region DRAGDROP
		
        /// <summary>
        /// Performs actions required while drag enter. (Overridden method)
        /// </summary>
        /// <param name="drgevent"></param>
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (this.helpText == null)
				CreateHelpText();
			if (this.toolTip == null)
				CreatetoolTip();
			//this.dragOverNode = null;
			dragging = true;
			base.OnDragEnter(drgevent);

			this.helpText.HidePopup();
			this.toolTip.HidePopup();
            if (this.timer.Enabled)
            {
                this.timer.Tick -= new EventHandler(timer_Tick);
                this.timer.Stop();
            }

			Invalidate();

			this.StartAutoScrollingInternal();
		}

		private void StopAutoScrollingInternal()
		{
			this.AutoScrollBounds = Rectangle.Empty;

			#region /* comments */
			// Code snippet was commented by Lucas in order to fix issue # 146
			// ( part 2 - problem dealed with autoscrolling )
			//this.AutoScrolling = ScrollBars.None;
			#endregion

		}

		private void StartAutoScrollingInternal()
		{
			this.AccelerateScrolling = AccelerateScrollingBehavior.Immediate;

			#region /* comments */
			// Code snippet was commented by Lucas in order to fix issue # 146
			// ( part 2 - problem dealed with autoscrolling )
			//this.AutoScrolling = ScrollBars.None;
			#endregion
		}

		/// <summary>
        /// Performs actions required while drag leaves. (Overridden method)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragLeave(EventArgs e)
		{
			m_highlightedNode = null;
			base.OnDragLeave(e);
			dragging = false;
			Invalidate();
			this.StopAutoScrollingInternal();
		}

		/// <summary>
		/// Performs the actions required while drag over. (Overridden method)
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragOver( DragEventArgs e )
		{
			// Define on what node cursor is located.
			Point p = new Point( e.X, e.Y );
			p = PointToClient( p );
			m_highlightedNode = GetNodeAtPoint( p );

			if( m_highlightedNode == m_lastButtonDownNode )
			{
				m_highlightedNode = null;
			}
           	this.MouseBasedSelectionOn = false;
			base.OnDragOver( e );
		}

		/// <summary>
        /// Performs the actions required on dragdrop. (Overridden method)
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			dragging = false;
			Invalidate();

			// Let the client use his own semantics for insertion.
			base.OnDragDrop(drgevent);
		}

		/// <summary>
        /// Performs actions required while QueryContinueDrag. (Overridden method)
		/// </summary>
		/// <param name="args"></param>
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs args)
		{
			base.OnQueryContinueDrag(args);

			if(args.Action == DragAction.Drop)
			{
				m_highlightedNode = null;
				this.TreeNodeDragHelper.EndDrag();
				this.DragCueOn = false;
			}
			else if(args.Action == DragAction.Cancel)
			{
				this.TreeNodeDragHelper.CancelDrag();
				this.DragCueOn = false;
			}
		}
		
        /// <summary>
        /// Performs actions required OnGiveFeedback. (Overridden method)
        /// </summary>
        /// <param name="args"></param>
		protected override void OnGiveFeedback(GiveFeedbackEventArgs args)
		{
			base.OnGiveFeedback(args);

			if(this.ShowDragNodeCue)
			{
				this.DragCueOn = true;
				if (this.TreeNodeDragHelper.IsDragging 
					// This condition is added by Lucas in order
					// to resolve problem # 169.
					|| m_bAllowDropStubWorks )
				{
					Point pt = GetDragWindowLocation( true );
					this.TreeNodeDragHelper.DoDrag(pt, args.Effect);
				}
			}
		}

        /// <summary>
        /// Dispatches the dragover operation.
        /// </summary>
        /// <param name="drgevent"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchDragOver( DragEventArgs drgevent )
		{
			OnDragOver( drgevent );
		}

        /// <summary>
        /// Dispatches the drag drop operation
        /// </summary>
        /// <param name="drgevent"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchOnDragDrop( DragEventArgs drgevent )
		{
			OnDragDrop( drgevent );
		}
        /// <summary>
        /// Dispatches the drag enter operation.
        /// </summary>
        /// <param name="drgevent"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchOnDragEnter( DragEventArgs drgevent )
		{
			OnDragEnter( drgevent );
		}
        /// <summary>
        /// Dispatches the DragLeave operation.
        /// </summary>
        /// <param name="e"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchOnDragLeave( EventArgs e )
		{
			OnDragLeave( e );
		}

        /// <summary>
        /// Dispatches the QueryContinueDrag operation.
        /// </summary>
        /// <param name="args"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchOnQueryContinueDrag( QueryContinueDragEventArgs args )
		{
			OnQueryContinueDrag( args );
		}

        /// <summary>
        /// Dispatches the GiveFeedback operation.
        /// </summary>
        /// <param name="args"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void DispatchOnGiveFeedback( GiveFeedbackEventArgs args )
		{
			OnGiveFeedback( args );
		}
        /// <summary>
        /// Dispatches the Dragdrop operation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="allowedEffects"></param>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public DragDropEffects DispatchDoDragDrop( object data, DragDropEffects allowedEffects )
		{
			return DoDragDrop( data, allowedEffects );
		}

		bool dragCueOn = false;
		private bool DragCueOn
		{
			get
			{
				return this.dragCueOn;
			}
			set
			{
				if(this.dragCueOn != value)
				{
					this.dragCueOn = value;

					if(value)
					{
						Bitmap bmp = this.GetDraggedNodesBitmap();
						if(bmp != null)
						{
							Point pt = GetDragWindowLocation( false );
							pt.Y += bmp.Height / 2;
							//pt.Y += SystemInformation.CursorSize.Height;
							this.TreeNodeDragHelper.StartDrag(bmp, pt, DragDropEffects.All);
						}
					}
				}
			}
		}
		private Point GetDragWindowLocation( bool bFinal )
		{
			Point pt = Control.MousePosition;

			if( !KeepDragCapturePoint )
			{
				pt.Y += 20;

				if( bFinal )
				{
					pt.Y += this.TreeNodeDragHelper.DragWindow.DragBitmap.Height / 2;
				}
			}
			// make x coordinate for proper drag window location.
			else
			{
				Rectangle boundingRect = this.GetSelectedNodesRectangle( this.draggedTnas );

				// Adjust the bounds to take scrolling into account
				if(this.HScrollBar.Enabled)
					boundingRect.X += this.HScrollBar.Value;

				Point origin = Point.Empty;
				Point cltPoint = PointToClient( pt );
				origin.X = cltPoint.X - boundingRect.Left;
				origin.Y = cltPoint.Y - boundingRect.Top;

				this.TreeNodeDragHelper.DragWindow.SetOrigin(origin);
			}

			return pt;
		}
		#endregion DRAGDROP

		private int m_textSelectionLength = 0;
		private int m_selectionStart = 0;

		private void labelEditor_KeyUp( object sender, KeyEventArgs e )
		{
			m_textSelectionLength = labelEditor.SelectionLength;
			m_selectionStart = labelEditor.SelectionStart;
		}

		// This will be called in a native app. scenario (with COM interop)
		private void labelEditor_KeyDown(object sender, KeyEventArgs e)
		{
			m_textSelectionLength = labelEditor.SelectionLength;
			m_selectionStart = labelEditor.SelectionStart;
			Keys keyData = e.KeyData;

			if(keyData == Keys.Return)
			{
				if(this.IsEditing)
				{
					this.EndEdit(false);
					return;
				}
			}
			else if(keyData == Keys.Escape)
			{
				if(this.IsEditing)
				{
                    cancelEdit = true;
                    this.EndEdit(true);
                    cancelEdit = false;
					return;
				}
			}
		}


		private string lastText = "";
		private void labelEditor_TextChanged(object sender, System.EventArgs e)
		{
			if( labelEditor.Text != lastText)
			{
				TreeNodeAdvCancelableEditEventArgs args = new TreeNodeAdvCancelableEditEventArgs(activeNode,labelEditor.Text);
				this.OnNodeEditorValidateString(args);
				
				if( args.Cancel )
				{
					this.labelEditor.Text = lastText;

					labelEditor.SelectionStart = ( labelEditor.SelectionStart == -1 ) ?
						labelEditor.Text.Length : m_selectionStart;

					labelEditor.SelectionLength = m_textSelectionLength;
				}
				else
				{
                    lastText = this.labelEditor.Text;
				}

				//adding +15 to the measured width because the edit when typing did not display the first character.
				Graphics g = CreateGraphics();
				int editorWidth = Math.Min(
					g.MeasureString(labelEditor.Text,labelEditor.Font).ToSize().Width+15,
					this.ClientRectangle.Width-activeNode.TextBounds.X);
				g.Dispose();

				if(editorWidth < 50)
					editorWidth = 50;
				this.labelEditor.Width = editorWidth;
				lastText = labelEditor.Text;
			}
		}
        private bool cancelEdit = false;
		// Do this instead of listening to KeyDown in TextBox which prevents the gong sound when hitting esc. in text box.
		// In a native app. scenario this won't be hit, so we instead listen to the TextBox's KeyDown event and process the keys there.
		/// <override/>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(keyData == Keys.Return)
			{
				if(this.IsEditing)
				{
					this.EndEdit(false);
					return true;
				}
			}
			else if(keyData == Keys.Escape)
			{
				// This gets called only when the label editor has focus.

				if(this.IsEditing)
				{
                    cancelEdit = true;
					this.EndEdit(true);
                    cancelEdit = false;
					return true;
				}
			}
			
			return base.ProcessDialogKey(keyData);
		}

		/// <override/>
		protected override bool IsInputKey(Keys keyData)
		{
			if( (this.labelEdit && keyData == Keys.F2) || keyData == Keys.Return )
				return true;
			if(keyData == Keys.Escape)
				return true;
			
			bool bCtrl = (keyData & Keys.Control) == Keys.Control;

			if((keyData & Keys.KeyCode) == Keys.PageDown)
			{
				if(bCtrl)
					return true;
				else if(this.ActiveNode != null)
					return true;
			}
			else if((keyData & Keys.KeyCode) == Keys.PageUp)
			{
				if(bCtrl)
					return true;
				else if(this.ActiveNode != null)
					return true;
			}
			else if((keyData & Keys.KeyCode) == Keys.Home)
			{
				if(bCtrl)
					return true;
				else if(this.ActiveNode != null)
					return true;
			}
			else if((keyData & Keys.KeyCode) == Keys.End)
			{
				if(bCtrl)
					return true;
				else if(this.ActiveNode != null)
					return true;
			}
			if((keyData & Keys.KeyCode) == Keys.Up && this.ActiveNode != null) 
			{
				return true;
			}
			if((keyData & Keys.KeyCode) == Keys.Down && activeNode != null) 
			{
				return true;
			}
            if (keyData == Keys.Add || keyData == Keys.Subtract && activeNode != null)
            {
                return true;
            }
			if(keyData == (GetIsMirrored() ? Keys.Right : Keys.Left) && activeNode != null) 
			{
				return true;
			}
			if(keyData == (GetIsMirrored() ? Keys.Left : Keys.Right))
				return true;

			if((keyData & Keys.KeyCode) == Keys.Space)
				return true;
					
			if(this.keySearching && keyData == Keys.Back)
				return true;

			return base.IsInputKey(keyData);
		}
		private void CancelMouseBasedSelection()
		{
			this.MouseBasedSelectionOn = false;
			this.LMouseDownNode = null;
		}

		private void SetNextNodeSelected( TreeNodeAdv newNode )
		{
			if( newNode == null )
				throw new ArgumentNullException( "newNode" );

			if( !this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) ) return;

			this.ActiveNode = newNode;
			this.selectionBaseNode = newNode;
			//Redraw a tree line for currently selected node.
			this.RefreshVScrollbar( newNode, newNode.Bounds.Y );
		}

		private bool IsMultipleSelection( TreeNodeAdv node )
		{
			if( node == null )
				throw new ArgumentNullException( "node" );

			if( SelectionMode == TreeSelectionMode.Single ) return false;

			return true;
		}

        /// <summary>
        /// Selectess all nodes from current active node to first or last node.
        /// </summary>
        /// <param name="fromCurrentToFirst">If True - select all nodes from current active node to first. 
        /// Otherwise - select all nodes from current active node to last one.</param>
        private void SetNodesSelected( bool fromCurrentToFirst )
        {
            if( fromCurrentToFirst )
                this.VScrollBar.SendScrollMessage(ScrollEventType.First);
            else
                this.VScrollBar.SendScrollMessage(ScrollEventType.Last);

            if (this.ActiveNode != null)
            {
                this.BeginUpdate();

                if (fromCurrentToFirst)
                {
                    for (int i = this.NodeToRowIndex(this.ActiveNode); i >= this.VScrollBar.Minimum; i--)
                    {
                        this.PerformNodeSelection( i );
                    }
                }
                else
                {
                    for (int i = this.NodeToRowIndex(this.ActiveNode); i <= this.VScrollBar.Maximum; i++)
                    {
                        this.PerformNodeSelection( i );
                    }
                }

                this.EndUpdate(false);

                TreeNodeAdv resultNode;
                if (fromCurrentToFirst)
                {                    
                    resultNode = this.RowIndexToNode(this.VScrollBar.Minimum);
                }
                else
                {
                    resultNode = this.RowIndexToNode(this.VScrollBar.Maximum);
                }

                resultNode.BringIntoView();
            }
        }

        private void PerformNodeSelection(int i)
        {
            TreeNodeAdv newNode = this.RowIndexToNode( i );

            if (IsMultipleSelection(newNode))
            {
                this.ExtendSelectionTo(newNode);
            }
            else
            {
                SetNextNodeSelected(newNode);
            }
        }

		/// <override/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			firstkeypress = DateTime.Now;
			timediff = firstkeypress - secondkeypress; 
			int milliSeconds = (int)timediff.TotalMilliseconds;
			//It ensures the visibility of the node and moves the scroll bar position according to the selection.
			this.EnsureVisibleSelectedNode = true;
			Keys keyData = e.KeyData;

			bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);

			if( keyData == Keys.F2 && labelEdit )
			{
				bKeypressed = Keys.F2;
				/**/
				// This condition was added by Lucas in order to resolve inssue # 180
				bool bIsAnyMouseButtonDown = 
					(Control.MouseButtons == MouseButtons.Left || 
					Control.MouseButtons == MouseButtons.Right);

				if( bIsAnyMouseButtonDown && m_lastButtonDownPoint != Point.Empty )
				{
					if( GetNodeAtPointEx( m_lastButtonDownPoint ) != null )
					{
						return;
					}
				}
				/**/

				BeginEdit();
				e.Handled = true;
			}
			else if( keyData == Keys.Return )
			{
				if( this.activeNode != null && this.activeNode.HasChildren )
				{
					this.activeNode.Expanded = !this.activeNode.Expanded;

					e.Handled = true;
				}
			}

			if( !e.Handled && keyData == Keys.Escape )
			{
				bKeypressed = Keys.Escape;
				this.CancelMouseBasedSelection();
				e.Handled = true;

				//Fix for the issue with Focus , when escape key is pressed  in treeViewAdv displayed in a dialog
				if( this.Parent is Form && ((Form)this.Parent).CancelButton != null )
					((Form)this.Parent).CancelButton.PerformClick();
			}

			if( !e.Handled )
			{
				bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
				bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

				if( (keyData & Keys.KeyCode) == Keys.PageDown )
				{
					selectUpwardDirection = false;
					bKeypressed = Keys.PageDown;

					if( bCtrl )
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.LargeIncrement );

						TreeNodeAdv newNode = LastVisibleNode;
						// Move the active node.
						if( bIsValidSingleSelection || IsMultipleSelection( newNode ) )
						{
							this.ActiveNode = newNode;
							this.selectionBaseNode = newNode;
						}

						e.Handled = true;
					}
					else if( bShift && this.SelectionMode != TreeSelectionMode.Single )
					{
						this.SetNodesSelected( false );

						e.Handled = true;
					}
					else if( this.ActiveNode != null )
					{
						// If the node that is LargeChange distance away from the
						// current ActiveNode is selectable, do so.
						int curPos = this.NodeToRowIndex( this.ActiveNode );
						int newSelRow = curPos + this.VScrollBar.LargeChange - 1;
						if( newSelRow > this.VScrollBar.Maximum )
							newSelRow = this.VScrollBar.Maximum;

						TreeNodeAdv newNode = this.RowIndexToNode( newSelRow );

						if( newNode.Enabled && this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
						{
							this.ActiveNode = newNode;
							this.selectionBaseNode = newNode;
						}
						e.Handled = true;
					}
				}
				else if( (keyData & Keys.KeyCode) == Keys.PageUp )
				{
					selectUpwardDirection = true;
					bKeypressed = Keys.PageUp;

					if( bCtrl )
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.LargeDecrement );

						if( bIsValidSingleSelection )
						{
							// Move the active node.
							this.ActiveNode = this.TopVisibleNode;
							this.selectionBaseNode = this.TopVisibleNode;
						}

						e.Handled = true;
					}
					else if( bShift && this.SelectionMode != TreeSelectionMode.Single )
					{
						this.SetNodesSelected( true );

						e.Handled = true;
					}
					else if( this.ActiveNode != null )
					{
						// If the node that is LargeChange distance away from the
						// current ActiveNode is selectable, do so.
						int curPos = this.NodeToRowIndex( this.ActiveNode );
						int newSelRow = curPos - this.VScrollBar.LargeChange + 1;
						if( newSelRow < this.VScrollBar.Minimum )
							newSelRow = this.VScrollBar.Minimum;

						TreeNodeAdv newNode = this.RowIndexToNode( newSelRow );

						if( newNode.Enabled && this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
						{
							this.ActiveNode = newNode;
							this.selectionBaseNode = newNode;
						}
						e.Handled = true;
					}
				}
				else if( (keyData & Keys.KeyCode) == Keys.Home )
				{
					selectUpwardDirection = true;
					bKeypressed = Keys.Home;

					if( bCtrl && !bShift )
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.First );

						if( bIsValidSingleSelection )
						{
							this.ActiveNode = this.RowIndexToNode( this.VScrollBar.Minimum );
							this.selectionBaseNode = this.ActiveNode;
						}

						e.Handled = true;
					}
					else if( (bShift || bCtrl) && this.SelectionMode != TreeSelectionMode.Single )
					{
						this.SetNodesSelected( true );

						e.Handled = true;
					}
					else if( this.ActiveNode != null )
					{
						TreeNodeAdv newNode = this.RowIndexToNode( this.VScrollBar.Minimum );

						if( newNode.Enabled && this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
						{
							this.ActiveNode = newNode;
							this.selectionBaseNode = newNode;
							//Fix for defect #1698: Root lines do not get painted consistently in treeviewAdv using End key and Home key 
							this.RefreshVScrollbar( newNode, newNode.Bounds.Y );
						}
						e.Handled = true;
					}
				}
				else if( (keyData & Keys.KeyCode) == Keys.End )
				{
					selectUpwardDirection = false;
					bKeypressed = Keys.End;

					if( bCtrl && !bShift )
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.Last );

						if( bIsValidSingleSelection )
						{
							this.ActiveNode = this.RowIndexToNode( this.VScrollBar.Maximum );
							this.selectionBaseNode = this.ActiveNode;
						}

						e.Handled = true;
					}
					else if( (bShift || bCtrl) && this.SelectionMode != TreeSelectionMode.Single )
					{
						this.SetNodesSelected( false );

						e.Handled = true;
					}
					else if( this.ActiveNode != null )
					{
						TreeNodeAdv newNode = this.RowIndexToNode( this.VScrollBar.Maximum );

						if( newNode.Enabled && this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
						{
							this.ActiveNode = newNode;
							this.selectionBaseNode = newNode;
						}
						e.Handled = true;
					}
				}
				if( !e.Handled && (keyData & Keys.KeyCode) == Keys.Up && this.ActiveNode != null )
				{
					bKeypressed = Keys.Up;
					if( this.ActiveNode.PrevVisibleNode!=null && this.ActiveNode.PrevVisibleNode != root )
					{
						TreeNodeAdv newNode = this.ActiveNode.PrevVisibleNode;
						selectUpwardDirection = true;
						if( bShift )
						{
							//Fix for defect 1695:  ScrollBar flashes ,when multiselection of nodes are done by holding down the shift key.
							this.BeginUpdate();

							if( IsMultipleSelection( newNode ) )
							{
								m_lastSelectedByKeyBoard = newNode;
								this.ExtendSelectionTo( newNode );
							}
							else
							{
								SetNextNodeSelected( newNode );
							}

							this.EndUpdate( false );
						}
						else if( bCtrl )
						{
							if( bIsValidSingleSelection || IsMultipleSelection( newNode ) )
							{
								//Fix for defect 1700: Attempting to make a multi-selection of tree nodes using the keyboard is pretty broken.
								if( ActiveNode == TopVisibleNode )
								{
									VScrollBar.SendScrollMessage( ScrollEventType.SmallDecrement );
								}

								this.ActiveNode = newNode;
							}
							else
							{
								this.VScrollBar.SendScrollMessage( ScrollEventType.SmallDecrement );
							}
						}
						else
						{
							SetNextNodeSelected( newNode );
						}
					}
					else
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.SmallDecrement );
					}

					e.Handled = true;
				}
				if( !e.Handled && (keyData & Keys.KeyCode) == Keys.Down && activeNode != null )
				{
					bKeypressed = Keys.Down;
					selectUpwardDirection = false;
					TreeNodeAdv newNode = activeNode.NextVisibleNode;
					if( newNode != null )
					{
						if( bShift )
						{
							//Fix for defect 1695:  ScrollBar flashes ,when multiselection of nodes are done by holding down the shift key.
							this.BeginUpdate();
							if( IsMultipleSelection( newNode ) )
							{
								m_lastSelectedByKeyBoard = newNode;
								this.ExtendSelectionTo( newNode );
							}
							else
							{
								SetNextNodeSelected( newNode );
							}
							this.EndUpdate( false );
						}
						else if( bCtrl )
						{
							if( bIsValidSingleSelection || IsMultipleSelection( newNode ) )
							{
								//Fix for defect 1702: The treeViewAdv is not getting scrolled to keep the focus rectangle in sight .
								this.ActiveNode = newNode;

								if( ActiveNode == LastVisibleNode )
								{
									VScrollBar.SendScrollMessage( ScrollEventType.SmallIncrement );
								}
							}
							else
							{
								VScrollBar.SendScrollMessage( ScrollEventType.SmallIncrement );
							}
						}
						else
						{
							SetNextNodeSelected( newNode );
						}
						//					Invalidate();
					}
					else
					{
						this.VScrollBar.SendScrollMessage( ScrollEventType.SmallIncrement );
					}
					e.Handled = true;
				}

				if( !e.Handled && 
					(keyData == (GetIsMirrored() ? Keys.Right : Keys.Left) || keyData == Keys.Back) &&
					activeNode != null )
				{
					if( activeNode.Expanded && activeNode.HasChildren && keyData != Keys.Back )
						activeNode.Expanded = false;
					else
					{
						if( activeNode.Parent!=null && activeNode.Parent != root
							&& activeNode.Parent.Enabled )
						{
							TreeNodeAdv newNode = activeNode.Parent;
							if( this.SetSelectedNode( newNode, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
							{
								this.ActiveNode = newNode;
								this.selectionBaseNode = newNode;
							}
						}
					}

					e.Handled = true;
				}
                if (!e.Handled && ((keyData ==Keys.Add) || (keyData== Keys.Subtract)) &&
                    activeNode != null)
                {
                    if (activeNode.Expanded && activeNode.HasChildren && keyData == Keys.Subtract)
                        activeNode.Expanded = false;
                    if (!activeNode.Expanded && activeNode.HasChildren && keyData == Keys.Add)
                        activeNode.Expanded = true;
                    e.Handled = true;
                }
				if( !e.Handled && 
					keyData == (GetIsMirrored() ? Keys.Left : Keys.Right) &&
                    activeNode != null )
				{
					if( !activeNode.Expanded )
						activeNode.Expanded = true;
					else
					{
						TreeNodeAdv next = activeNode.NextSelectableNode;
						if( next != null && next.Parent == activeNode )
						{
							TreeNodeAdv newNode = next;
							if( this.SetSelectedNode( next, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
							{
								this.ActiveNode = newNode;
								this.selectionBaseNode = newNode;
							}
						}
					}

					e.Handled = true;
				}
				if( !e.Handled )
				{
					if( this.MultiSelect && (keyData & Keys.KeyCode) == Keys.Space && bCtrl )
					{
						if( activeNode != null )
						{
							if( activeNode.IsSelected )
								this.SetSelectedNode( null, activeNode, TreeViewAdvAction.ByKeyboard );
							else
								this.SetSelectedNode( activeNode, new ArrayList(), TreeViewAdvAction.ByKeyboard );

							this.selectionBaseNode = activeNode;
						}
					}
					// Space but not Ctrl
					else if( keyData == Keys.Space )
					{
						if( keySearch=="" || !keySearching )
						{
							CheckState newState = CheckState.Checked;
							bool newStateKnown = false;
							
							// Toggle the activeNode
							m_bIsKeyDown = true;

                            if (activeNode != null && activeNode.EnabledButtons)
							{
                                if (activeNode.Checked)
                                    newState = CheckState.Unchecked;
                                else
                                    newState = CheckState.Checked;
							
                                activeNode.Checked = !activeNode.Checked;
								newStateKnown = true;
							}

							// And also toggle the selected nodes, if any.
							foreach( TreeNodeAdv node in this.SelectedNodes )
							{
								// Set the state based on the current activeNode's state.
								if( node != activeNode && node.EnabledButtons )
								{
									if( newStateKnown )
									{
										node.CheckState = newState;
									}
									else
									{
										node.Checked = !node.Checked;
										newState = node.CheckState;
										newStateKnown = true;
									}
								}
							}

							m_bIsKeyDown = false;
						}
					}
				}

				if( !e.Handled &&
					keySearching && 
					(keyData.ToString().Length == 1 
					|| keyData == Keys.Space 
					|| keyData == Keys.Back 
                    || (bShift && (keyData & ~(Keys.ShiftKey | Keys.Shift)) > 0)
					|| (keyData.ToString().StartsWith( "D" ) 
					&& keyData.ToString().Length == 2)) )
				{
                    if (!this.keyInputTimer.Enabled)
                        searchFromNextSelectableNode = true;

					this.keyInputTimer.Enabled = false;
					this.keyInputTimer.Enabled = true;

					if( keyData == Keys.Space && keySearch!=null && keySearch.Length > 0 )
					{
						this.keySearch += " ";
					}

					if( keyData == Keys.Back )
					{
						if( keySearch.Length > 0 )
							this.keySearch = keySearch.Substring( 0, keySearch.Length - 1 );
					}

					if( keyData.ToString().StartsWith( "D" ) && keyData.ToString().Length == 2 )
					{
						this.keySearch += keyData.ToString().Substring( keyData.ToString().Length -  1, 1 );
					}
					else if( keyData != Keys.Space && keyData != Keys.Back )
					{
                        if (bShift)
                        {
                            if (keyData.ToString().Substring(0, 2).Contains(","))
                                this.keySearch += (keyData & ~e.Modifiers);
                        }
                        else
                            this.keySearch += keyData.ToString();
					}
					if (milliSeconds > 150 && milliSeconds < 300)
						searchFromNextSelectableNode = false;
					else
						searchFromNextSelectableNode = true;
					FindNode( keySearch, bShift );
					secondkeypress = DateTime.Now;

					if (this.keyInputTimer.Enabled && milliSeconds < 200)
						keySearch = null;
				}
			}
			base.OnKeyDown( e );
		}
		/// </override>
		protected override bool IsInputChar(char charCode)
		{
			return true;
		}

		private void FindNode( string s, bool bShiftPressed )
		{
            if (activeNode != null)
            {
				if (s!=null && (activeNode.Enabled || s != String.Empty ) )
				{
					int count = 0;
					TreeNodeAdv firstNode;
	
					if( bShiftPressed && Nodes.Count > 0 || null == activeNode )
					{
						firstNode = this.Nodes[0];
					}
					else
					{
						firstNode = activeNode;
					}
	
					if( firstNode == null )
					{
						return;
					}

                    if (this.ActiveNode == firstNode 
                        && searchFromNextSelectableNode)
                    {
                        firstNode = firstNode.NextSelectableNode;
                        searchFromNextSelectableNode = false;
                        if (firstNode == null)
                            firstNode = Nodes[0];
                    }
	
					while( !firstNode.Enabled )
					{
						firstNode = firstNode.NextSelectableNode;
					}
	
					TreeNodeAdv node;
	
					if( firstNode.Text.Length>=s.Length && s.ToUpper() == firstNode.Text.Substring( 0, s.Length ).ToUpper() )
					{
						node = firstNode;
					}
					else
					{
						node = firstNode.NextSelectableNode;
	
						if( node == null )
						{
							node = firstNode;
						}
					}
	
					while( !activeNode.Enabled )
					{
						activeNode = activeNode.PrevSelectableNode;
					}
	
					bool head = false;
	
					while( node != firstNode || !head )
					{
						int i = 0;
						for( ; i < s.Length && i < node.Text.Length; ++i )
						{
							if( s.ToUpper()[i] != node.Text.ToUpper()[i] )
							{
								break;
							}
						}
	
						if( i > count )
						{
							count = i;
	
							if( this.SetSelectedNode( node, this.selectedNodes, TreeViewAdvAction.ByKeyboard ) )
							{
								this.ActiveNode = node;
								this.selectionBaseNode = node;
							}
						}
	
						node = node.NextSelectableNode;
	
						if( node == null )
						{
							node = Nodes[0];
							firstNode = activeNode;
							head = true;
						}
					}
	
					Invalidate();
				}
            }
		}

		internal void MakeDirty()
		{
			IComponentChangeService changeService =this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			
			if(changeService != null)
			{
				changeService.OnComponentChanging(this, null);
				changeService.OnComponentChanged(this, null, null, null);
			}
			this.Invalidate();
		}

		private bool updating = false;

		/// </override>
		protected override void OnUpdatingChanged(EventArgs e)
		{
			this.updating = base.Updating;
			if(updating)
				this.measuringGraphics = this.CreateGraphics();
			else
			{
				//this.Root.RecalculateAllDimensions();
				//this.RefreshVScrollbar();
				UpdateVerticalScrollBar();
				if (this.measuringGraphics != null)
				{
					this.measuringGraphics.Dispose();
					this.measuringGraphics = null;
				}
			}
		}

		/// <summary>
		/// Returns the width required to draw the text specified using the font specified.
		/// </summary>
		/// <param name="graphics">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="text">The text that is to be drawn.</param>
		/// <param name="font">The <see cref="System.Drawing.Font"/> using which to draw.</param>
		/// <returns>Width required.</returns>
		public Size MeasureDisplayStringSize( Graphics graphics, string text, Font font )
		{
			if( string.Empty == text ) return Size.Empty;

			IntPtr hdc = graphics.GetHdc();
			IntPtr hFont = font.ToHfont();
            NativeMethods.RECT rect = new NativeMethods.RECT( 0, 0, 0, 0 );
            try
            {
                IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

                int nFlags = TreeNodeAdv.c_nDrawTextFlags | DrawTextFormats.DT_CALCRECT;

                if (GetIsMirrored())
                {
                    nFlags |= DrawTextFormats.DT_RTLREADING;
                }

                NativeMethods.DrawText(hdc, text, text.Length, ref rect, nFlags);

                prevFont = NativeMethods.SelectObject(hdc, prevFont);
            }
            finally
            {
                NativeMethods.DeleteObject(hFont);
                graphics.ReleaseHdc(hdc);
            }
			return new Size( rect.Width, rect.Height );
		}

		/// <summary>
		/// Collapses all the tree nodes.
		/// </summary>
		/// <remarks>
		/// <p>The CollapseAll method collapses all the <see cref="TreeNodeAdv"/> 
		/// objects, including all the child tree nodes, that are in the 
		/// <see cref="TreeViewAdv"/> control.</p>
		/// <p>The state of a <b>TreeNodeAdv</b> persists. For example, suppose that 
		/// a parent tree node is expanded. If the child tree nodes were not 
		/// previously collapsed, they will appear in their previously-expanded 
		/// state. Calling the <b>CollapseAll</b> method ensures that all the tree nodes 
		/// appear in the collapsed state.</p>
		/// </remarks>
		public void CollapseAll()
		{
			foreach(TreeNodeAdv topLevel in this.Root.Nodes)
				topLevel.CollapseAll();
		}

		/// <summary>
		/// Expands all the tree nodes.
		/// </summary>
		/// <remarks>
		/// <p>The <b>ExpandAll</b> method expands all the <see cref="TreeNodeAdv"/> 
		/// objects, including all the child tree nodes, that are in the <see cref="TreeViewAdv"/> 
		/// control.</p>
		/// </remarks>
		public void ExpandAll()
		{
			foreach(TreeNodeAdv topLevel in this.Root.Nodes)
				topLevel.ExpandAll();
		}

        /// <summary>
        /// Cancels the edit mode.
        /// </summary>
        /// <remarks>
        /// <p> The <b>CancelEditMode</b> method cancels the edit mode when the node is in the EditingMode.</p>
        /// </remarks>
        public void CancelEditMode()
        {
            cancelEdit = true;
            EndEdit(true);
            cancelEdit = false;
        }
		/// <summary>
		/// Returns the number of tree nodes that can be fully visible in the tree view 
		/// control.
		/// </summary>
		/// <value>
		/// The number of <see cref="TreeNodeAdv"/> items that can be fully visible in 
		/// the <see cref="TreeViewAdv"/> control.
		/// </value>
		/// <remarks>
		/// The <b>VisibleCount</b> value can be greater than the number of tree nodes 
		/// in the tree view. The value is calculated by dividing the height of the 
		/// client window by the height of a tree node item. The result is the total 
		/// number of <see cref="TreeNodeAdv"/> objects that the <see cref="TreeViewAdv"/> is 
		/// capable of displaying within its current dimensions.
		/// </remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
		Description("Gets the number of tree nodes that can be fully visible in the tree view  control.")]
		public int VisibleCount
		{
			get
			{
				return this.ClientRectangle.Height / this.ItemHeight;
			}
		}

		/// <summary>
		/// Retrieves the number of tree nodes, optionally including those in all 
		/// subtrees, assigned to the tree view control.
		/// </summary>
		/// <param name="includeSubTrees"><b>true</b> to count the <see cref="TreeNodeAdv"/> 
		/// items that the subtrees contain; false otherwise. </param>
		/// <returns>The number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.</returns>
		/// <remarks>
		/// If includeSubTrees is <b>true</b>, the result is the number of all the tree nodes in the entire tree structure.
		/// </remarks>
        /// <example>This example describes how to count all the nodes(including child nodes) of the treeViewAdv
        ///The user could get the total number of nodes by calling GetNodeCount method with the 
        ///bool argument which indicates whether count should include sub trees or not. If we
        ///pass it as true, it will count the nodes with the subtrees also.
        ///<code language="C#">
        ///private void button1_Click(object sender, System.EventArgs e) 
        ///{ 
        ///   //Call the tree control's "GetNodeCount" method with true to 
        ///   //get the total number of nodes in the tree 
        /// int TotalNodesInTree = this.treeViewAdv1.GetNodeCount( true ); 
        /// MessageBox.Show( "Total nodes in tree = " + TotalNodesInTree.ToString()); 
        ///} 
        /// //Add nodes  
        ///private void button2_Click(object sender, System.EventArgs e) 
        ///{ 
        ///this.treeViewAdv1.SelectedNode.Nodes.Add(new TreeNodeAdv()); 
        ///} 
        /// //Remove nodes 
        ///private void button3_Click(object sender, System.EventArgs e) 
        ///{ 
        ///this.treeViewAdv1.SelectedNode.Parent.Nodes.Remove(this.treeViewAdv1.SelectedNode); 
        ///} 
        ///</code>
        ///<code language="VB"> 
        ///Private Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        /// ' Call the tree control's  "GetNodeCount" method with true to 
        /// ' get the total number of nodes in the tree 
        /// Dim TotalNodesInTree As Integer = Me.treeViewAdv1.GetNodeCount(True) 
        /// MessageBox.Show("Total nodes in tree = " &amp; TotalNodesInTree.ToString()) 
        /// End Sub 
        /// 'Add nodes  
        ///Private Sub button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        ///Me.treeViewAdv1.SelectedNode.Nodes.Add(New TreeNodeAdv()) 
        ///End Sub 
        /// 'Remove nodes 
        ///Private Sub button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        ///Me.treeViewAdv1.SelectedNode.Parent.Nodes.Remove(Me.treeViewAdv1.SelectedNode) 
        ///End Sub 
        ///</code>
        ///</example>
        public int GetNodeCount(bool includeSubTrees)
		{
			return this.Root.GetNodeCount(includeSubTrees);
		}
		internal void ExpandedChanged(TreeNodeAdv node, bool expanded)
		{
			try
			{
				if(updating)return;
				if(this.IsVerticalGradient)
				{
					Invalidate();
					return;
				}
				Point pt = NodeToPoint(node);
				if(node.HasChildren)
				{
					if(pt.Y<Height)
						Invalidate(new Rectangle(pt,new Size(this.ClientRectangle.Width,Height-pt.Y)));
				}
				else
					Invalidate(new Rectangle(pt,new Size(this.ClientRectangle.Width,node.Height)));

				// Init the scrollbar before updating since the
				// DrawVerticalLines method is more efficient if the VScrollbar setting is known.
				this.RefreshVScrollbar();

				if(expanded)
				{
					MoveScrollBarOnClick( node );
				}

				Update();
			}
			finally
			{
				// Call the AfterXX events after updating, so that the node's Y will be up to date.
				// Node's Y will be used from within the ValidateScrollPosition
				TreeViewAdvNodeEventArgs e = new TreeViewAdvNodeEventArgs(node);
				if(expanded)
					OnAfterExpand(e);
				else
					OnAfterCollapse(e);
			}
		}
		internal void NodesChanging()
		{
			this.CancelMode();
		}
		internal void NodesChanged()
		{
			// If last node is null, then just reset the scroll to top.
			if(this.LastVisibleNode == null)
			{
				this.VScrollPos = 1;
				this.VScrollBar.Value = this.VScrollPos;
			}

			if( !updating )
			{
				Invalidate();
			}
			// Don't want to update immediately. Otherwise, multiple node additions is very slow.
			//Update();
		}



		private void keyInputTimer_Tick(object sender, System.EventArgs e)
		{
			this.keyInputTimer.Enabled = false;
			this.keySearch = "";
			Invalidate();
		}



		private int GetEVVScrollPos(TreeNodeAdv node)
		{
			int y0 = NodeToPoint(node).Y+node.Height;
			int y = y0;
			int rowIndex = NodeToRowIndex(node);
			int targetRowIndex = rowIndex;

			int height = ClientHeight;

			while (y0-y<height && node != null)
			{
				y-=node.Height;
				rowIndex--;
				TreeNodeAdv nNode = RowIndexToNode(rowIndex);
				if(y0 - y <height)
				{
					node = nNode;
				}
				else break;
			}
			if(node != null)
			{
				int topRowIndex = NodeToRowIndex(node);
				// If only partly-visible then scroll once more so that it's fully visible
				if(y0-y > height && topRowIndex != targetRowIndex)// as opposed to yo-y = height
				{
					topRowIndex++;
				}
				return topRowIndex;
			}
			else
				return 1;

		}

		/// <summary>
		/// Overloaded. Scrolls the control so that the specified node becomes visible.
		/// </summary>
		/// <param name="node">The node that requires visibility.</param>
		public void EnsureVisible( TreeNodeAdv node )
		{
			RefreshVScrollbar();
			this.EnsureVisibleV( node, false );
		}

		/// <summary>        
		/// Scrolls the control so that the specified node becomes visible and 
		/// optionally forces it to be the top-most visible node.
		/// </summary>
		/// <param name="node">The node that is to be scrolled</param>
		public void EnsureVisibleH( TreeNodeAdv node )
		{
			// if node is not full displayed than change scroll position
			if( null != root )
			{
                int nodeXRel = this.Indent * ( node.Level - 1 );
                int point = nodeXRel + node.Width - ClientRectangle.Width;

                if( point >= this.HScrollBar.Value )
			    {
                    this.HScrollPos = point;
			    }
			    else if( nodeXRel < this.HScrollBar.Value )
			    {
                    this.HScrollPos = nodeXRel;
			    }

                if( this.RightToLeft == RightToLeft.Yes && this.Office2007ScrollBars )
                {
                    this.HScrollBar.Value = this.ReflectPosition( this.HScrollPos ); 
                }
                else
                {
                    this.HScrollBar.Value = this.HScrollPos;
                }
			}
		}

		/// <summary>
		/// Scrolls the control so that the specified node becomes visible and 
		/// optionally forces it to be the top-most visible node.
		/// </summary>
		/// <param name="node">The node that is to be scrolled.</param>
		/// <param name="showOnTop">True to force it to be the the top-most visible node; false to just scroll it into view.</param>
		public void EnsureVisibleV(TreeNodeAdv node, bool showOnTop)
		{
    		if( !VerticallScroll || !node.Visible )
        		return;

			int rowIndex = NodeToRowIndex(node);

			TreeNodeAdv last = this.LastVisibleNode;
			int lastRowIndex;
			if(last!=null)
			{
				lastRowIndex = NodeToRowIndex(last);
			}
			else
			{
				lastRowIndex = this.VScrollBar.Maximum;
			}

			if(showOnTop)
			{
				VScrollPos = rowIndex;
			}
			else
			{
		        if( rowIndex >= VScrollPos && rowIndex < lastRowIndex )
        			return;

				if(rowIndex<this.VScrollPos)
				{
					VScrollPos = rowIndex;
				}
				if(rowIndex>=lastRowIndex)
				{
					VScrollPos = GetEVVScrollPos(node);
				}
			}
		}

		[Documentation.DocumentationExclude()]
		protected int ClientHeight
		{
			get
			{
				int height = ClientRectangle.Height;
				// The HorizontalScrollBarHeight should be excluded already when calling ClientRectangle.Height.
				//if(HorisontalScroll)
				//	height -= SystemInformation.HorizontalScrollBarHeight;
				return height;
			}
		}

		/// <summary>
		/// Returns the last visible node.
		/// </summary>
		/// <value>A <see cref="TreeNodeAdv"/> instance.</value>
		/// <remarks><seealso cref="TopVisibleNode"/></remarks>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public virtual TreeNodeAdv LastVisibleNode
		{
			get
			{
				TreeNodeAdv node = PointToNode(new Point(3,3));
				if(node == null)
					return null;
				int y = NodeToPoint(node).Y+node.Height;
				int rowIndex = NodeToRowIndex(node);
				int height = ClientHeight;
				while (y< height)
				{
					y+=node.Height;
					rowIndex++;
					TreeNodeAdv nNode = RowIndexToNode(rowIndex);
					if(nNode !=null)
					{
						node = nNode;
					}
					else
					{
						return node;
					}
				}
				return node;
			}
		}

		private void TreeViewAdv_Leave(object sender, System.EventArgs e)
		{
			if(hideSelection)
				Invalidate();
		}

		/// <summary>
		/// Overloaded. Returns the node at the specified location.
		/// </summary>
		/// <param name="x">The X co-ordinate.</param>
		/// <param name="y">The Y co-ordinate.</param>
		/// <returns>The node at the point.</returns>
		public TreeNodeAdv GetNodeAtPoint(int x,int y)
		{
			return GetNodeAtPoint(new Point(x,y));
		}

		/// <summary>
		/// Returns the node at the specified location.
		/// </summary>
		/// <param name="pt">The point.</param>
		/// <returns>The node at the point.</returns>
		public TreeNodeAdv GetNodeAtPoint(Point pt)
		{
			return GetNodeAtPoint(pt,false);
		}

		/// <summary>
		/// Returns the node at the specified location.
		/// </summary>
		/// <param name="pt">Location.</param>
		/// <param name="textBounds">Indicates whether the testing will be done using the bounds of the text, not the whole bounds of the node.</param>
		/// <returns>The node at the point.</returns>
		public TreeNodeAdv GetNodeAtPoint(Point pt, bool textBounds)
		{
			return this.GetNodeAtPoint(pt, textBounds, false);
		}

		/// <summary>
		/// Returns the node at the specified location.
		/// </summary>
		/// <param name="pt">Location.</param>
		/// <param name="textBounds">Indicates whether the testing will be done using the bounds of the text, not the whole bounds of the node.</param>
		/// <param name="textOrImageBounds">Indicates whether the testing will be done using the bounds of the images and text, 
		/// not the whole bounds of the node.</param>
		/// <returns>The node at the point.</returns>
		/// <remarks>If both the textBounds and textOrImageBounds params are false then the testing will be done on the 
		/// whole node.</remarks>
		public TreeNodeAdv GetNodeAtPoint(Point pt, bool textBounds, bool textOrImageBounds)
		{
			// Force any pending paint messages so that the bounds of nodes can be updated.
			//this.Update();

			TreeNodeAdv nd =  this.PointToNode(pt);
			if(nd != null )
			{
				int rowIndex = this.NodeToRowIndex(nd);
				if(rowIndex>= this.VScrollPos && rowIndex<= NodeToRowIndex(LastVisibleNode) && (textBounds || textOrImageBounds))
				{
					if(textBounds && nd.TextBounds.Contains(pt))
						return nd;
					if(textOrImageBounds && nd.TextAndImageBounds.Contains(pt))
						return nd;
				}
				else
				{
					return nd;
				}
			}

			return null;
		}
		/// <summary>
		/// Returns the node at the specified point.  
		/// </summary>
		/// <param name="pt">Specified point.</param>
		/// <returns>Node at specified point if exist; null otherwise.</returns>
		public TreeNodeAdv GetNodeAtPointEx( Point pt )
		{
			// Force any pending paint messages so that the bounds of nodes can be updated.
			this.Update();

			TreeNodeAdv nd =  this.PointToNode(pt);
			if(nd != null )
			{
				int rowIndex = this.NodeToRowIndex(nd);
				if(rowIndex>= this.VScrollPos && rowIndex<= NodeToRowIndex(LastVisibleNode) )
				{
					if( nd.TextBounds.Contains(pt))
						return nd;
					if( nd.TextAndImageBounds.Contains(pt))
						return nd;
				}
				else
				{
					return nd;
				}
			}

			return null;
		}
		internal bool ExpandedChanging(TreeNodeAdv node, bool expanding)
		{
			TreeViewAdvCancelableNodeEventArgs e = new TreeViewAdvCancelableNodeEventArgs(node, false);
			if(expanding)
			{
				this.VScrollBar.BeginUpdate();
				this.HScrollBar.BeginUpdate();
				OnBeforeExpand(e);
				this.HScrollBar.EndUpdate();
				this.VScrollBar.EndUpdate();
			}
			else
			{
                OnBeforeCollapse(e);

                if (!SelectOnCollapse)
                {
                    if (!e.Cancel &&IsCollapsedContainsSelectionNodes(e.Node))
                    {
                            TreeNodeAdvCollection NodesToBeRemoved = new TreeNodeAdvCollection();
                            bool shouldSelectCollapse = false;
                            bool shouldSetActive = false;
                            
                            if (IsCollapsedContainsActiveNode(e.Node))
                                shouldSetActive= true;
                            
                            if (IsCollapsedContainsSelectionNodes(e.Node))
                                shouldSelectCollapse = true;

                            foreach (TreeNodeAdv tna in SelectedNodes)
                                if (IsNodeUnderCollapsedNode(e.Node, tna))
                                    NodesToBeRemoved.Add(tna);
                            
                            foreach (TreeNodeAdv tna in NodesToBeRemoved)
                            {
                                SelectedNodes.Remove(tna);
                            }

                            NodesToBeRemoved.Clear();
                            foreach (TreeNodeAdv highlighted in this.m_hashHighlightedNodes.Values)
                                if (IsNodeUnderCollapsedNode(e.Node, highlighted))
                                    NodesToBeRemoved.Add(highlighted);

                            NodesToBeRemoved.Add(e.Node);

                            foreach (TreeNodeAdv tna in NodesToBeRemoved)
                            {
                                this.m_hashHighlightedNodes.Remove(tna);
                            }

                            if (!this.MultiSelect)
                                SelectedNode = null;

                            if (shouldSelectCollapse && this.selectedNodes.Contains(e.Node))
                                 this.SetSelectedNode(e.Node, new ArrayList(), TreeViewAdvAction.Collapse);
                             if (shouldSetActive && shouldSelectCollapse)
                                 this.activeNode = e.Node;
                    }
                }
                else
                {
                    TreeNodeAdv selectedNode = null;
                    //Added conditions for collapsed parent node is to be focused when its + icon is clicked.
                    if (!e.Cancel && this.MultiSelect)
                    {
                        if (IsCollapsedContainsSelectionNodes(e.Node) ||IsCollapsedContainsActiveNode(e.Node)|| IsNodeUnderCollapsedNode(e.Node, SelectedNode) || e.Node == SelectedNode)
                            this.RemoveNodeFromList(e.Node);

                    }
                    //if SelectionMode is Single and the collapsed node contains the selected node as child node, The collapsed node should change into SelectedNode .
                    else if (!e.Cancel && this.SingleSelect)
                        if (IsNodeUnderCollapsedNode(e.Node, SelectedNode))
                        {
                            selectedNode = SelectedNode;
                            SelectedNode = e.Node;
                        }



                    //Fix #1471: AfterSelect event does not fire if selection is changed due to collapse of node

                    if ((SelectedNode != null) && !e.Cancel && (!(SelectedNode == e.Node) && SelectedNode.Parent == e.Node) && selectedNode != SelectedNode)
                    {
                        SelectedNode = e.Node;
                    }
                }
            }


            return !e.Cancel;
        }

      
        /// <summary>
        /// Returns true/false if the specified node is under the collapsed node.
        /// </summary>
        /// <param name="collapsed">The collapsed.</param>
        /// <param name="activeNode1">The node.</param>
        /// <returns>
        /// 	<c>true</c> if specified node under collapsed node; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNodeUnderCollapsedNode(TreeNodeAdv collapsed, TreeNodeAdv node)
        {
            try
            {
                if (node==null||node.parent == null || node.parent == this.Root)
                    return false;
                else if (node.parent == collapsed)
                    return true;
                else
                    return IsNodeUnderCollapsedNode(collapsed, node.parent);
            }
            catch (NullReferenceException nre)
            {
                System.Diagnostics.Debug.WriteLine(nre.Message + Environment.NewLine + nre.StackTrace);
                return false;
            }

        }
        /// <summary>
        /// Returns true/false if the active node is under the collapsed node.
        /// </summary>
        /// <param name="collapsed">The collapsed.</param>
        /// <returns>
        /// 	<c>true</c> if active node under collapsed node; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCollapsedContainsActiveNode(TreeNodeAdv collapsed)
        {
            if (this.activeNode != null)
                if (IsNodeUnderCollapsedNode(collapsed, this.activeNode))
                    return true;
            return false;

        }
        /// <summary>
        /// Removes the nodes list from selected nodes list, if its under the collapsed node.
        /// </summary>
        /// <param name="nodeToBeAdded">Collapsed node.</param>
        private void RemoveNodeFromList(TreeNodeAdv collapsedNode)
        {
            TreeNodeAdvCollection NodesToBeRemoved = new TreeNodeAdvCollection();

            //If selected node is under collapsed node, it should be removed from selected list.
            foreach (TreeNodeAdv tna in SelectedNodes)
                if (IsNodeUnderCollapsedNode(collapsedNode, tna))
                    NodesToBeRemoved.Add(tna);
			this.activeNode = collapsedNode;
			if (!this.selectedNodes.Contains(collapsedNode))
				this.SetSelectedNode(collapsedNode, NodesToBeRemoved, TreeViewAdvAction.Collapse);
			else if (NodesToBeRemoved.Count > 0)
			{
				collapsedNode = null;
           		this.SetSelectedNode(collapsedNode, NodesToBeRemoved, TreeViewAdvAction.Collapse);
			}

        }

        /// <summary>
        /// Returns true if the node which is to be collapsed contains any selcted nodes.
        /// </summary>
        /// <param name="collapsednode">The Collapsed node.</param>
        /// <returns>
        /// 	<c>true</c> if collapsed node contains selection nodes; otherwise, it returns <c>false</c>.
        /// </returns>
        private bool IsCollapsedContainsSelectionNodes(TreeNodeAdv collapsednode)
        {
            foreach (TreeNodeAdv tna in SelectedNodes)
                if (IsNodeUnderCollapsedNode(collapsednode, tna))
                    return true;
            if (!SelectOnCollapse && IsNodeUnderCollapsedNode(collapsednode, this.activeNode) )
                return true;
            return false;
        }

        
		
		//bool firstMouseMove = true;

		private void TreeViewAdv_ScrollbarsVisibleChanged(object sender, System.EventArgs e)
		{
			if(!this.HorisontalScroll)
			{
				this.HScrollPos = 0;
			}
			//			if(!this.VerticallScroll)
			//			{
			//				this.VScrollPos = 0;
			//			}
		}

		internal void RemovedNode(TreeNodeAdv node)
		{
			ArrayList removedNodes = new ArrayList(new TreeNodeAdv[]{node});
			this.SetSelectedNode(null, removedNodes, TreeViewAdvAction.Unknown, false, true);
			if(this.ActiveNode == node)
				this.ActiveNode = null;
			// The removed node cannot be the selectionBaseNode.
			//if(this.selectionBaseNode == null)
			//	this.selectionBaseNode = node;
		}

		//		internal void Clearing(TreeNodeAdv node)
		//		{
		//			if(selectedNode ==null) return;
		//			if(node.HasNode(selectedNode)) this.SelectedNode = null;
		//		}

		private void labelEditor_MouseUp( object sender, MouseEventArgs e )
		{
			m_textSelectionLength = labelEditor.SelectionLength;
			m_selectionStart = labelEditor.SelectionStart;
		}
        private void labelEditor_LostFocus(object sender, EventArgs e)
        {
            if (!ignoreLeave && this.ClientHeight>0 && !this.IsEditEnding)
                EndEdit(false);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
            base.OnHandleDestroyed(e);
        }
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );
            Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
			if( this.DesignMode && this.RightToLeft == RightToLeft.Yes )
			{
				this.Width += 1; this.Width -= 1;
			}
		}
	}



    #region TreeViewAdv SearchFunctionality

    public class TreeViewAdvFindReplaceDialog
    {
        internal bool isMatchFound = false;
        private int NodePointIndex = -1;
        private string searchText = string.Empty;
        internal TreeNodeAdvCollection treeNodeAdvCollection = new TreeNodeAdvCollection();
        private TreeViewAdv treeView = null;
        private TreeViewSearchOption m_searchOptions = TreeViewSearchOption.MatchWholeText;
        private TreeViewSearchRange m_treeSearchRange = TreeViewSearchRange.TreeView;
        private TreeViewSearchNavigation m_nodeSearchType = TreeViewSearchNavigation.SearchAll;

        /// <summary>
        /// Initializes new instances of TreeViewAdvFindReplaceDialog class
        /// </summary>
        /// <param name="tree">TreeViewAdv instance</param>
        public TreeViewAdvFindReplaceDialog(TreeViewAdv tree)
        {
            treeView = tree;
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdvCollection that matches search string
        /// </summary>
        internal TreeNodeAdvCollection SearchTreeNodeAdvCollection
        {
            get
            {
                return treeNodeAdvCollection;
            }
            set
            {
                treeNodeAdvCollection = value;
            }
        }

        /// <summary>
        /// TreeViewAdv instance
        /// </summary>
        internal TreeViewAdv TreeView
        {
            get
            {
                return treeView;
            }
            set
            {
                if (value != treeView)
                    treeView = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeViewAdv Search Option
        /// </summary>
        public TreeViewSearchOption TreeViewSearchOption
        {
            get
            {
                return m_searchOptions;
            }
            set
            {
                if (m_searchOptions != value)
                    m_searchOptions = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeViewAdv search range 
        /// </summary>
        public TreeViewSearchRange TreeViewSearchRange
        {
            get
            {
                return m_treeSearchRange;
            }
            set
            {
                if (m_treeSearchRange != value)
                    m_treeSearchRange = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdv search navigation type
        /// </summary>
        public TreeViewSearchNavigation TreeViewSearchNavigation
        {
            get
            {
                return m_nodeSearchType;
            }
            set
            {
                if (m_nodeSearchType != value)
                    m_nodeSearchType = value;
            }
        }

        /// <summary>
        /// Highlights matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>true if match found</returns>
        public bool Find(string nodeText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeMatched = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            isNodeMatched = this.Find(nodeText);
            return isNodeMatched;
        }

        /// <summary>
        /// Highlights matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <returns>true if match found</returns>
        public bool Find(string nodeText)
        {
            this.SearchTreeNodeAdvCollection.Clear();
            if (this.Search(nodeText))
            {
                this.searchText = nodeText;
                if (this.TreeView != null && this.NodePointIndex == -1)
                {
                    NodePointIndex = 0;
                }
                else
                {
                    if (this.TreeViewSearchNavigation != TreeViewSearchNavigation.SearchUp)
                    {
                        NodePointIndex = this.SearchTreeNodeAdvCollection.IndexOf(this.TreeView.SelectedNode);
                        if (NodePointIndex == this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            if (this.TreeViewSearchNavigation == TreeViewSearchNavigation.SearchAll)
                                NodePointIndex = 0;
                        }
                        else if (NodePointIndex <= this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            NodePointIndex += 1;
                        }

                    }
                    else
                    {
                        NodePointIndex = this.SearchTreeNodeAdvCollection.IndexOf(this.TreeView.SelectedNode);
                        if (NodePointIndex > 0 && NodePointIndex <= this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            NodePointIndex -= 1;
                        }
                    }
                }
                if (this.NodePointIndex >= 0 && this.NodePointIndex < this.SearchTreeNodeAdvCollection.Count
                          && this.TreeView.SelectedNode != this.SearchTreeNodeAdvCollection[NodePointIndex])
                {
                    this.TreeView.RaiseNodeBeforeFindEvent(this.SearchTreeNodeAdvCollection[NodePointIndex], this.searchText);
                    if (!this.TreeView.DisableFinding)
                    {
                        this.TreeView.SelectedNode = this.SearchTreeNodeAdvCollection[NodePointIndex];
                        this.TreeView.RaiseNodeAfterFoundEvent(this.SearchTreeNodeAdvCollection[NodePointIndex], this.searchText);
                    }
                    return true;
                }
            }
            else
            {
                this.TreeView.SelectedNode = null;
                return false;
            }
            return false;
        }

        /// <summary>
        /// Highlights all matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if match found</returns>
        public bool FindAll(string nodeText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeMatched = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            isNodeMatched = this.FindAll(nodeText);
            return isNodeMatched;
        }

        /// <summary>
        /// Highlights all matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <returns>returns true if match found</returns>
        public bool FindAll(string nodeText)
        {
            this.SearchTreeNodeAdvCollection.Clear();
            this.TreeView.SelectedNode = null;
            if (this.Search(nodeText))
            {
                this.searchText = nodeText;
                foreach (TreeNodeAdv node in this.SearchTreeNodeAdvCollection)
                {
                    this.TreeView.RaiseNodeBeforeFindEvent(node, nodeText);
                }
                if (!this.TreeView.DisableFinding)
                {
                    this.TreeView.SelectedNodes.AddRange(this.SearchTreeNodeAdvCollection);
                    foreach (TreeNodeAdv nodes in this.TreeView.SelectedNodes)
                    {
                        this.TreeView.RaiseNodeAfterFoundEvent(nodes, nodeText);
                    }
                }
                return true;
            }
            else
            {
                this.TreeView.SelectedNode = null;
                return false;
            }
        }

        /// <summary>
        /// returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if match found</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplaced = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            if (this.Find(nodeText))
            {
                isNodeTextReplaced = Replace(replaceText);
                return isNodeTextReplaced;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchOption searchOption)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, searchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="ReplaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeReplaceText)
        {
            if (this.SearchTreeNodeAdvCollection != null && this.SearchTreeNodeAdvCollection.Count > 0 && this.TreeView.SelectedNode != null)
            {
                this.TreeView.RaiseNodeReplacingEvent(this.TreeView.SelectedNode, this.TreeView.SelectedNode.Text, nodeReplaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
                if (!this.TreeView.DisableReplacing)
                {
                    this.TreeView.SelectedNode.Text = nodeReplaceText;
                    this.TreeView.RaiseNodeReplacedEvent(this.TreeView.SelectedNode, this.TreeView.SelectedNode.Text, nodeReplaceText);
                }
                this.SearchTreeNodeAdvCollection.Remove(this.TreeView.SelectedNode);
                this.TreeView.SelectedNode = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchoption">TreeViewSearchOption</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchOption searchoption)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, searchoption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if all matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if all matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplaced = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            if (this.FindAll(nodeText))
            {
                isNodeTextReplaced = ReplaceAll(replaceText);
                return isNodeTextReplaced;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched all TreeNodeAdv text replaced
        /// </summary>
        /// <param name="ReplaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeReplaceText)
        {
            if (this.SearchTreeNodeAdvCollection != null && this.SearchTreeNodeAdvCollection.Count > 0 && this.TreeView.SelectedNode != null)
            {
                foreach (TreeNodeAdv node in SearchTreeNodeAdvCollection)
                {
                    this.TreeView.RaiseNodeReplacingEvent(node, node.Text, nodeReplaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
                    node.Text = nodeReplaceText;
                    this.TreeView.RaiseNodeReplacedEvent(node, node.Text, nodeReplaceText);
                }
                this.TreeView.SelectedNode = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Defines if Search Text matches any TreeNodeAdv text
        /// </summary>
        /// <param name="Text">Search String</param>
        /// <returns>returns true if match found</returns>
        internal bool Search(string Text)
        {
            this.FindNameInTreeView(this.TreeView, Text);
            if (this.SearchTreeNodeAdvCollection.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Find and return TreeNodeAdv based on TreeSearchOption
        /// </summary>
        /// <param name="node">TreeNodeAdv instances</param>
        /// <param name="name">Search Text</param>
        /// <returns>SearchTreeNodeAdvCollection</returns>
        internal TreeNodeAdv FindNameInTreeView(TreeNodeAdv node, String name)
        {
            TreeNodeAdv matchedNode = null;
            if (node == null)
                return null;

            switch (this.TreeViewSearchOption)
            {
                case TreeViewSearchOption.MatchCase:
                    if (node.Text == name)
                    {
                        isMatchFound = true;
                    }
                    break;
                case TreeViewSearchOption.MatchWholeText:
                    if (node.Text.ToLower() == (name.ToLower()) || name != string.Empty && node.Text.ToLower().StartsWith(name.ToLower()))
                    {
                        isMatchFound = true;
                    }
                    break;
            }

            if (node != null && isMatchFound)
            {
                if (this.TreeViewSearchRange == TreeViewSearchRange.ChildNode)
                {
                    if (node.Level > 1)
                        this.SearchTreeNodeAdvCollection.Add(node);
                }
                else
                {
                    this.SearchTreeNodeAdvCollection.Add(node);
                }
                isMatchFound = false;
            }

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                matchedNode = FindNameInTreeView(node.Nodes[i], name);
                if (matchedNode != null)
                    return matchedNode;
            }

            return null;
        }

        /// <summary>
        /// Find and return matched TreeNodeAdv Collection
        /// </summary>
        /// <param name="treeView">TreeViewAdv Instances</param>
        /// <param name="name">Search String</param>
        /// <returns>SearchTreeNodeAdvCollection</returns>
        internal TreeNodeAdv FindNameInTreeView(TreeViewAdv treeView, String name)
        {
            bool isNodePresent = false;
            if (treeView == null)
                return null;

            if (this.TreeViewSearchRange == TreeViewSearchRange.RootNode)
            {
                for (int i = 0; i < this.TreeView.Nodes.Count; i++)
                {
                    if (this.TreeViewSearchOption == TreeViewSearchOption.MatchCase)
                    {
                        if (this.TreeView.Nodes[i].Text == name)
                        {
                            isNodePresent = true;
                            this.SearchTreeNodeAdvCollection.Add(this.TreeView.Nodes[i]);
                        }
                    }
                    else if (this.TreeView.Nodes[i].Text.ToLower() == name.ToLower() || this.TreeView.Nodes[i].Text.ToLower().StartsWith(name))
                    {
                        isNodePresent = true;
                        this.SearchTreeNodeAdvCollection.Add(this.TreeView.Nodes[i]);
                    }
                }
            }

            if (this.TreeViewSearchRange == TreeViewSearchRange.TreeView
                || this.TreeViewSearchRange == TreeViewSearchRange.ChildNode)
            {
                for (int i = 0; i < treeView.Nodes.Count; i++)
                {
                    TreeNodeAdv matchedNode = FindNameInTreeView(treeView.Nodes[i], name);
                    if (matchedNode != null)
                    {
                        isNodePresent = true;
                        this.SearchTreeNodeAdvCollection.Add(matchedNode);
                    }
                }
            }

            if (isNodePresent)
            {
                isNodePresent = false;
                return this.SearchTreeNodeAdvCollection[0];
            }
            else
            {
                return null;
            }
        }
    }
    #endregion

    /// <summary>
	/// Handles the <see cref="TreeViewAdv.AfterCheck"/> event.
	/// </summary>
	public delegate void TreeNodeAdvEventHandler(object sender,TreeNodeAdvEventArgs e);

	/// <summary>
	/// Provides data for the TreeViewAdv selection events.
	/// </summary>
	public class TreeNodeAdvEventArgs: EventArgs
	{
		private TreeViewAdvAction action;
		private TreeNodeAdv node;

		/// <summary>
		/// Overloaded. Creates a new instance of this class.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
		public TreeNodeAdvEventArgs(TreeNodeAdv node)
		{
			this.node = node;
		}

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
		/// <param name="action">A <see cref="TreeViewAdvAction"/> type.</param>
		public TreeNodeAdvEventArgs(TreeNodeAdv node, TreeViewAdvAction action)
		{
			this.node = node;
			this.action = action;
		}

		/// <summary>
		/// Gets / sets the <see cref="TreeViewAdvAction"/> associated with the event.
		/// </summary>
		public TreeViewAdvAction Action
		{
			get
			{
				return this.action;
			}

			set
			{
				this.action = value;
			}
		}

		/// <summary>
		/// Gets / sets the <see cref="TreeNodeAdv"/> associated with the event.
		/// </summary>
		public TreeNodeAdv Node
		{
			get
			{
				return this.node;
			}

			set
			{
				this.node = value;
			}
		}
	}

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.NodeEditorValidated"/> event.
	/// </summary>
	public delegate void TreeNodeAdvEditEventHandler(object sender,TreeNodeAdvEditEventArgs e);
	/// <summary>
	/// <p>Handles the <see cref="TreeViewAdv.NodeEditorValidateString"/> 
	/// and <see cref="TreeViewAdv.NodeEditorValidating"/> event.</p>
	/// </summary>
	public delegate void TreeNodeAdvCancelableEditEventHandler(object sender,TreeNodeAdvCancelableEditEventArgs e);

	/// <summary>
	/// Provides data for the cancelable validation events in the TreeViewAdv.
	/// </summary>
	public class TreeNodeAdvCancelableEditEventArgs : TreeNodeAdvEditEventArgs
	{
		private bool cancel;
		private bool continueEditing;

		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
		/// <param name="label">The new text for the node.</param>
		public TreeNodeAdvCancelableEditEventArgs(TreeNodeAdv node,string label)
			: base(node,label)
		{
			this.cancel = false;
			this.continueEditing = true;
		}
		/// <summary>
		/// Indicates whether the event should be cancelled.
		/// </summary>
		public bool Cancel
		{
			get{return this.cancel;}
			set{this.cancel = value;}
		}
		/// <summary>
		/// Indicates whether editing should end now.
		/// </summary>
		/// <value>This property is consulted only when <see cref="Cancel"/> is set to true.
		/// If you Cancel the operation and if this property is set
		/// to false, editing mode will end; otherwise editing mode will be preserved.
		/// Default is true.</value>
		/// <remarks>
		/// <p>This property will be ignored by the 
		/// <see cref="TreeViewAdv.NodeEditorValidateString"/> event.</p>
		/// </remarks>
		public bool ContinueEditing
		{
			get{return this.continueEditing;}
			set{this.continueEditing = value;}
		}
	}
	/// <summary>
	/// Provides data for the editing events in the <see cref="TreeViewAdv"/>.
	/// </summary>
	public class TreeNodeAdvEditEventArgs : EventArgs
	{
		private string label;
		private TreeNodeAdv node;

		/// <summary>
		/// Returns the label for the node.
		/// </summary>
		public string Label
		{
			get{return label;}
		}
		/// <summary>
		/// Returns the <see cref="TreeNodeAdv"/> that is currently being edited.
		/// </summary>
		public TreeNodeAdv Node
		{
			get{return node;}
		}

		/// <summary>
		/// Creates a new instance of the TreeNodeAdvEditEventArgs.
		/// </summary>
		/// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
		/// <param name="label">The label for the node.</param>
		public TreeNodeAdvEditEventArgs(TreeNodeAdv node,string label)
		{
			this.node = node;
			this.label = label;
		}

	}

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.NodeDoubleClick"/> event.
    /// </summary>
    public delegate void TreeNodeAdvMouseClickArgs(object sender, TreeViewAdvMouseClickEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="TreeViewAdv.NodeDoubleClick"/> event.
    /// </summary>
    public class TreeViewAdvMouseClickEventArgs : EventArgs
    {
        private TreeNodeAdv node;
        private MouseButtons m_Mousebutton;
        private int m_Click;
        private int x;
        private int y;
        private int m_Delta;

        public TreeViewAdvMouseClickEventArgs(TreeNodeAdv treenode, MouseButtons button, int clicks, int X, int Y, int delta)
        {
            this.node = treenode;
            this.Mousebutton = button;
            this.m_Click = clicks;
            this.x = X;
            this.y = Y;
            this.m_Delta = delta;
        }

        /// <summary>
        /// Returns TreeNodeAdv instance
        /// </summary>

        public TreeNodeAdv Node
        {
            get { return node; }
        }
        
        /// <summary>
        /// Gets/Sets which mouse button was pressed
        /// </summary>
        public MouseButtons Mousebutton
        {
            get { return m_Mousebutton; }
            set { m_Mousebutton = value; }
        }

        /// <summary>
        /// Gets the number of times the mouse button was pressed and released
        /// </summary>
        public int Clicks
        {
            get { return m_Click; }
        }

        /// <summary>
        /// Gets the x-coordinate of the mouse during the generating mouse event.
        /// </summary>
        public int X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-coordinate of the mouse during the generating mouse event
        /// </summary>
        public int Y
        {
            get { return y; }
        }

        /// <summary>
        /// Gets a signed count of the number of detents the mouse wheel has rotated, multiplied by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
        /// </summary>
        public int Delta
        {
            get { return m_Delta; }
        }
	}

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.OnNodeAfterFound"/> event.
    /// </summary>
    public delegate void TreeViewOnAfterFindArgs(object sender, TreeNodeAdvAfterFindArgs e);
    public class TreeNodeAdvAfterFindArgs : EventArgs
    {
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvAfterFindArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv instance</param>
        /// <param name="n_searchText">TreeNodeAdv Text which needs to be searched</param>
        public TreeNodeAdvAfterFindArgs(TreeNodeAdv m_node, string n_searchText)
        {
            node = m_node;
            m_searchText = n_searchText;
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdv instance that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set 
            { 
                if (node != value && value != null) 
                { 
                    node = value;
                } 
            }
        }

        /// <summary>
        /// Gets/Sets value of search string
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.OnNodeBeforeFind"/> event.
    /// </summary>
    public delegate void TreeViewOnBeforeFindArgs(object sender, TreeNodeAdvBeforeFindArgs e);
    public class TreeNodeAdvBeforeFindArgs : SyncfusionCancelEventArgs
    {
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvBeforeFindArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv instance</param>
        /// <param name="n_searchText">TreeNodeAdv text which needs to be searched</param>
        public TreeNodeAdvBeforeFindArgs(TreeNodeAdv m_node, string n_searchText)
        {
            node = m_node;
            m_searchText = n_searchText;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.OnNodeReplacing"/> event.
    /// </summary>
    public delegate void TreeViewOnReplacingArgs(object sender, TreeNodeAdvOnReplacingArgs e);
    public class TreeNodeAdvOnReplacingArgs : SyncfusionCancelEventArgs
    {
        private string m_replaceText = string.Empty;
        private string m_searchText = string.Empty;
        private TreeViewSearchOption m_treesearchOption;
        private TreeViewSearchRange m_treesearchrange;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvOnReplacingArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv Instance</param>
        /// <param name="n_searchText">Search String</param>
        /// <param name="n_replace">Replace String</param>
        /// <param name="findOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        public TreeNodeAdvOnReplacingArgs(TreeNodeAdv m_node, string n_searchText, string n_replace, TreeViewSearchOption findOption, TreeViewSearchRange searchRange)
        {
            node = m_node;
            m_searchText = n_searchText;
            m_replaceText = n_replace;
            m_treesearchOption = findOption;
            m_treesearchrange = searchRange;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches search string
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                if (!base.Cancel)
                    return node;
                else
                    return null;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeViewAdv search option value
        /// </summary>
        public TreeViewSearchOption TreeViewSearchOption
        {
            get
            {
                if (!base.Cancel)
                    return m_treesearchOption;
                else
                    return TreeViewSearchOption.MatchCase;
            }
            set
            {
                if (m_treesearchOption != value)
                {
                    m_treesearchOption = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeViewAdv search range value
        /// </summary>
        public TreeViewSearchRange TreeViewSearchRange
        {
            get
            {
                if (!base.Cancel)
                    return m_treesearchrange;
                else
                    return TreeViewSearchRange.TreeView;
            }
            set
            {
                if (m_treesearchrange != value)
                {
                    m_treesearchrange = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                if (!base.Cancel)
                    return m_searchText;
                else
                    return string.Empty;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv replace text value
        /// </summary>
        public string ReplaceText
        {
            get
            {
                if (!base.Cancel)
                    return m_replaceText;
                else
                    return string.Empty;
            }
            set
            {
                if (m_replaceText != value)
                {
                    m_replaceText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.OnNodeReplaced"/> event.
    /// </summary>
    public delegate void TreeViewOnReplacedArgs(object sender, TreeNodeAdvOnReplacedArgs e);
    public class TreeNodeAdvOnReplacedArgs : SyncfusionEventArgs
    {
        private string m_replaceText = string.Empty;
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvOnReplacedArgs class
        /// </summary>
        /// <param name="n_searchText">Search String</param>
        /// <param name="n_replace">Replace String</param>
        /// <param name="m_node">TreeNodeAdv Instances</param>
        public TreeNodeAdvOnReplacedArgs(string n_searchText, string n_replace, TreeNodeAdv m_node)
        {
            m_searchText = n_searchText;
            m_replaceText = n_replace;
            node = m_node;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv replace text value
        /// </summary>
        public string ReplaceText
        {
            get
            {
                return m_replaceText;
            }
            set
            {
                if (m_replaceText != value)
                {
                    m_replaceText = value;
                }
            }
        }
    }

  

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeEdit"/> event.
	/// </summary>
	public delegate void TreeViewAdvBeforeEditEventHandler(object sender,TreeNodeAdvBeforeEditEventArgs e);
	
	/// <summary>
	/// Provides data for the <see cref="TreeNodeAdv.BeforeEdit"/> event.
	/// </summary>
	public class TreeNodeAdvBeforeEditEventArgs : CancelEventArgs
	{
		private TextBox textBox;
		private TreeNodeAdv node;

		/// <summary>
		/// Returns the <see cref="TreeNodeAdv"/>.
		/// </summary>
		public TreeNodeAdv Node
		{
			get{return node;}
		}

		/// <summary>
		/// Returns the <see cref="TextBox"/> that is used to edit the node.
		/// </summary>
		public TextBox TextBox
		{
			get{return textBox;}
		}
		/// <summary>
		/// Creates a new instance of the class.
		/// </summary>
		/// <param name="node">Specifies the <see cref="TreeNodeAdv"/>.</param>
		/// <param name="textBox">A <see cref="TextBox"/> instance.</param>
		public TreeNodeAdvBeforeEditEventArgs(TreeNodeAdv node,TextBox textBox)
		{
			this.node = node;
			this.textBox = textBox;
		}
	}
	/// <summary>
	/// Handles the <see cref="TreeViewAdv.BeforeNodePaint"/> event of the TreeViewAdv control.
	/// </summary>
	public delegate void TreeNodeAdvPaintEventHandler(object sender,TreeNodeAdvPaintEventArgs e);

	/// <summary>
	/// Handles the <see cref="TreeViewAdv.NodeBackgroundPaint"/> event of the TreeViewAdv control.
	/// </summary>
	public delegate void TreeNodeAdvPaintBackgroundEventHandler(object sender,TreeNodeAdvPaintBackgroundEventArgs e);

	[Documentation.DocumentationExclude()]
	public class StyleNamePairsList : ArrayList
	{
		TreeViewAdv tree;
		public StyleNamePairsList(TreeViewAdv tree)
		{
			this.tree = tree;
		}
		public void AddRange(StyleNamePair[] stylePairs)
		{
			foreach(StyleNamePair pair in stylePairs)
			{
				pair.style.Identity = new TreeViewAdvStyleInfoIdentity(tree);
				this.tree.BaseStyles[pair.name] = pair.style;
			}
		}
        public void Add(StyleNamePair stylePair)
        {
            stylePair.style.Identity = new TreeViewAdvStyleInfoIdentity(tree);
            this.tree.BaseStyles[stylePair.name] = stylePair.style;
        }
	}

	[Documentation.DocumentationExclude()]
	[TypeConverter(typeof(StyleNamePairConverter))]
	public class StyleNamePair
	{
		internal string name;
		internal TreeNodeAdvStyleInfo style;
		public StyleNamePair(string name, TreeNodeAdvStyleInfo style)
		{
			this.name = name;
			this.style = style;
		}
	}
	internal class TreeBoundStyleInfoStore : TreeNodeAdvStyleInfoStore
	{
		private TreeViewAdv tree;
		public TreeBoundStyleInfoStore(TreeViewAdv tree)
		{
			this.tree = tree;
		}
		public override bool HasValue(StyleInfoProperty sip)
		{
			if(sip == TreeNodeAdvStyleInfoStore.FontProperty)
				return true;
			else if(sip == TreeNodeAdvStyleInfoStore.TextColorProperty)
				return true;
			else if(sip == TreeNodeAdvStyleInfoStore.HeightProperty)
				return true;
			return base.HasValue(sip);
		}
		public override object GetValue(StyleInfoProperty sip)
		{
			if(sip == TreeNodeAdvStyleInfoStore.FontProperty)
				return tree.Font;
			else if(sip == TreeNodeAdvStyleInfoStore.TextColorProperty)
				return tree.ForeColor;
			else if(sip == TreeNodeAdvStyleInfoStore.HeightProperty)
				return tree.ItemHeight;

			return base.GetValue(sip);
		}
	}
	/// <summary>
	/// Specifies the selection mode for the tree.
	/// </summary>
	public enum TreeSelectionMode
	{
		/// <summary>
		/// Lets you select one node at a time.
		/// </summary>
		Single,
		/// <summary>
		/// Lets you select multiple nodes within the same level.
		/// </summary>
		MultiSelectSameLevel,
		/// <summary>
		/// Lets you select multiple nodes across all levels.
		/// </summary>
		MultiSelectAll
	}
}