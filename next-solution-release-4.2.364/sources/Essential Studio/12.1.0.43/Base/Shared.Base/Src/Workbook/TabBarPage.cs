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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Diagnostics;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///     TabPage implements a single page of a tab bar control. It is essentially
	///     a panel that can host other child controls. The TabBarSplitterControl will
	///     display the text property of this control as a label in the associated tab.
	/// </summary>
	[
	DefaultProperty("Text"),
	ToolboxItem(false),
	DefaultEvent("Click"),
    Designer(typeof(Syncfusion.Windows.Forms.TabBarPageDesigner), 
        typeof(System.ComponentModel.Design.IDesigner))
    ]
    public class TabBarPage: ContainerControl,
        IDynamicSplitterFrame,
        IInternalSplitterParent,
		IThemedControl,
		IContainerControl
    {
		// Event:
		/// <summary>
		/// Occurs when the <see cref="ToolTipText"/> property has changed.
		/// </summary>
		public event EventHandler ToolTipTextChanged;

		/// <summary>
		/// Occurs when the <see cref="SplitBars"/> property has changed.
		/// </summary>
		public event EventHandler SplitBarsChanged;

		/// <summary>
		/// Occurs when the vertical splitter position has changed.
		/// </summary>
		public event EventHandler VSplitPosChanged;

		/// <summary>
		/// Occurs when the horizontal splitter position has changed.
		/// </summary>
		public event EventHandler HSplitPosChanged;

		/// <summary>
		/// Occurs when the splitter layout has changed.
		/// </summary>
		public event EventHandler SplitterLayoutChanged;


        // Fields
        private string toolTipText = "";
        private bool tabEnabled = true;
		internal SplitterInfo splitterInfo;
		private Icon icon;
		
		/// <summary>
		/// Back color for TabBarPage Tab.
		/// </summary>
		private Color m_tabBackColor = SystemColors.Control;
		
        private ISplitterPaneFactory splitterPaneFactory = new SplitterPaneFactory();

        internal InternalSplitter horizontalSplitterBar = null;
        internal InternalSplitter verticalSplitterBar = null;
		internal bool flatLook = true;

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (splitterInfo != null)
					splitterInfo.Dispose();
			}
			base.Dispose (disposing);
		}


		/// <summary>
		/// Returns the parent splitter frame.
		/// </summary>
		IDynamicSplitterFrame SplitterParent 
		{
			get
			{
				return Parent as IDynamicSplitterFrame;
			}
		}

		/// <summary>
		/// Returns the number of visible row panes.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
        public int RowCount
        {
            get
            {
                return (VSplitPos != 100) ? 2: 1;
            }
        }

		/// <summary>
		/// Returns the number of visible column panes.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
		public int ColumnCount
        {
            get
            {
                return (HSplitPos != 100) ? 2: 1;
            }
        }

		/// <summary>
		/// Indicates whether we can split the rows at the given y coordinate.
		/// </summary>
		/// <param name="cy">The vertical position in percentages of the splitter control's height.</param>
		/// <returns>True if rows were split successfully; False if they were already split or the operation aborted.</returns>
		public bool SplitRow(int cy) 
        {
            if (cy >= Height || cy <= 0)
                return false;

            SplitterLayout._vSplitPos = cy*100/(Height-14);
            UpdateSplitter();
            return true;
        }

		/// <summary>
		/// Indicates whether we can split the columns horizontally at the specified x coordinate.
		/// </summary>
		/// <param name="cx">The horizontal position in percentages of the splitter control's width.</param>
		/// <returns>True if columns were split successfully; False if they were already split or the operation aborted.</returns>
		public bool SplitColumn(int cx) 
        {
            if (cx >= Width || cx < 0)
                return false;

            SplitterLayout._hSplitPos = cx*100/(Width-14);
			UpdateSplitter();
			return true;
        }

		/// <summary>
		/// Occurs after the control to be displayed in a new pane has been created. Use this
		/// event to implement additional initialization for the new control. 
		/// </summary>
		/// <remarks>
		/// PaneCreated is an ideal hook to add handler for events in the new control.
		/// </remarks>
		public event SplitterPaneEventHandler PaneCreated;

		/// <summary>
		/// Raises the <see cref="PaneCreated"/> event.
		/// </summary>
		/// <param name="e">A <see cref="SplitterPaneEventArgs" /> that contains the event data.</param>
		protected virtual void OnPaneCreated(SplitterPaneEventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, e);
#else
			;
#endif

			if (PaneCreated != null)
				PaneCreated(this, e);
		}

		/// <summary>
		/// Occurs after a row or column is hidden and before the control that is displayed in the pane 
		/// is disposed. Use this event to implement additional clean up for the control before <see cref="DisposePane"/>
		/// is called. 
		/// </summary>
		/// <remarks>
		/// PaneClosing is an ideal hook to unwire event handlers from the control.
		/// </remarks>
		public event SplitterPaneEventHandler PaneClosing;

		/// <summary>
		/// Raises the <see cref="PaneClosing"/> event.
		/// </summary>
		/// <param name="e">A <see cref="SplitterPaneEventArgs" /> that contains the event data.</param>
		protected virtual void OnPaneClosing(SplitterPaneEventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, e);
#else
			;
#endif

			if (PaneClosing != null)
				PaneClosing(this, e);
		}

		/// <summary>
		/// Closes the specified pane.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public virtual void DisposePane(int row, int column)
		{
			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				parent.DisposePane(row, column);
		}

		/// <summary>
		/// Closes the splitter panes at the specified row.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		public void DeleteRow(int row)
        {
			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				parent.DeleteRow(row);
        }

		/// <summary>
		/// Closes the splitter panes at the specified column.
		/// </summary>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public void DeleteColumn(int column)
        {
			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				parent.DeleteColumn(column);
        }

		/// <summary>
		/// Indicates whether the scrollbar belongs to the active pane.
		/// </summary>
		/// <param name="control">The control associated with the scrollbar.</param>
		/// <param name="sbType">Specifies the vertical or horizontal scrollbar.</param>
		/// <returns>True if active; False otherwise.</returns>
		/// <remarks>
		/// 
		/// </remarks>
		/// <example>
		/// ScrollControl checks IsActive to find out if it is target of a HScroll event
		/// <code lang="C#">
		/// 		protected virtual void OnHScroll(object sender, ScrollEventArgs se)
		/// 		{
		/// 			try
		/// 			{
		/// 				IScrollBarFrame sbf = GetScrollBarFrameOfComponent(this);
		/// 				if (sbf != null &amp;&amp; !sbf.IsActive(this, ScrollBars.Horizontal))
		/// 					return;
		/// 
		///					...
		/// </code>
		/// </example>
		public bool IsActive(Control control, ScrollBars sbType)
		{
			int row, column;
			if (this.FindPane(control, out row, out column))
			{
				if (sbType == ScrollBars.Vertical) 
					return column == this.splitterInfo.activeColumn;
				else
					return row == this.splitterInfo.activeRow;
			}
			return false;
		}

		/// <summary>
		/// Returns the splitter pane at the specified row and column. If there is no pane found at the
		/// specified row and column a pane will be created on demand with a call to <see cref="SplitterControl.OnCreateNewControl"/>.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <returns>The control at the pane.</returns>
		public Control GetPane(int row, int column)
        {
			if (Controls.Count == 0)
				return null;

			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				return parent.GetPane(row, column);
			return null;
        }

		/// <summary>
		/// Returns the splitter pane at the specified row and column. If there is no pane found at the
		/// specified row and column a null reference will be returned.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		/// <returns>The control at the pane.</returns>
		public virtual Control GetPaneInternal(int row, int column)
		{
			if (Controls.Count == 0)
				return null;

			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				return parent.GetPaneInternal(row, column);
			return null;
		}

		/// <summary>
		/// Returns the row and column index for a child pane.
		/// </summary>
		/// <param name="control">The control to search for.</param>
		/// <param name="row">A placeholder where the row is returned.</param>
		/// <param name="column">A placeholder where the column is returned.</param>
		/// <returns>True if the control is a pane; False if the control was not a child pane.</returns>
		public bool FindPane(Control control, out int row, out int column)
        {
			for (row = 0; row <= 1; row++)
                for (column = 0; column <= 1; column++)
                    if (splitterInfo.splitterPanes[row, column] == control)
                        return true;

            row = -1;
            column = -1;
            return false;
        }
        

		/// <summary>
		/// Gets / sets the active pane in the splitter control.
		/// </summary>
		[
		Browsable(false), 
		Description(@"The currently active control."), 
		Category(@"Behavior"), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Control ActivePane 
		{ 
			set
			{
				int row, column;
				if (FindPane(value, out row, out column))
				{
					SetActivePane(row, column);
				}
			} 
			get
			{
				return GetPane(splitterInfo.activeRow, splitterInfo.activeColumn);
			} 
		}

		/// <summary>
		/// Sets the active pane in the splitter control specified by the row and column indices.
		/// </summary>
		/// <param name="row">The zero-based index for the splitter pane row.</param>
		/// <param name="column">The zero-based index for the splitter pane column.</param>
		public void SetActivePane(int row, int column)
        {
			if (Controls.Count == 0)
				return;

			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				parent.SetActivePane(row, column);
        }            
        
		/// <summary>
		/// Indicates whether there is a next or previous pane that can be activated. 
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		/// <returns>True if activating next or previous pane is good; False if already at last or first pane.</returns>
		public bool CanActivateNext(bool prev)
        {
            if (!prev)
                return splitterInfo.activeRow < RowCount-1 || splitterInfo.activeRow < ColumnCount-1;
            else
                return splitterInfo.activeRow > 0 || splitterInfo.activeRow > 0;
        }

		/// <summary>
		/// Activates the next or previous pane. 
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		public void ActivateNext(bool prev)
        {
			if (Controls.Count == 0)
				return;

			SplitterControl parent = Parent as SplitterControl;
			if (parent != null)
				parent.ActivateNext(prev);
        }

        
		/// <summary>
		/// Occurs when the user drags the splitterbar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="x">The current horizontal position in pixels.</param>
		/// <param name="y">The current vertical position in pixels.</param>
		public virtual void OnMoveSplitter(object sender, int x, int y)
		{
		}

		/// <summary>
		/// Occurs after the user has moved the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		public virtual void OnMovedSplitter(object sender) 
		{
		}

		/// <summary>
		/// Repaints the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		public virtual void InvalidateSplitter(object sender)
        {
            if (sender == horizontalSplitterBar)
            {
                Invalidate(horizontalSplitterBar.Bounds);
            }
            else if (sender == verticalSplitterBar)
            {
                Invalidate(verticalSplitterBar.Bounds);
            }
			Invalidate();
        }

		Cursor IInternalSplitterParent.OverrideCursor 
		{	
			get
			{
                IInternalSplitterParent ip = Parent as IInternalSplitterParent;
                if (ip != null)
                    return ip.OverrideCursor;
                return null;
            }
			set
			{
                IInternalSplitterParent ip = Parent as IInternalSplitterParent;
                if (ip != null)
                    ip.OverrideCursor = value;
			}
        }

        // Cursor

        /// <summary>
        ///     Handles the WM_SETCURSOR message.
        /// </summary>
        /// <internalonly/>
		private void WmSetCursor(ref Message m) 
		{

			// Accessing through the Handle property has side effects that break this
			// logic. You must use InternalHandle.
			//
			if (m.WParam == Handle && ((int)m.LParam & 0x0000FFFF) == NativeMethods.HTCLIENT) 
			{
                OnSetCursor(ref m);
			}
			else 
			{
				DefWndProc(ref m);
			}

		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSetCursor(ref Message m) 
		{
            IInternalSplitterParent ip = Parent as IInternalSplitterParent;
            if (ip != null && ip.OverrideCursor != null)
				Cursor.Current = ip.OverrideCursor;
			else
                Cursor.Current = Cursor;
		}

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message msg) 
		{
			if (DesignMode)
				base.WndProc(ref msg);
			else switch (msg.Msg) 
			{
				case NativeMethods.WM_SETCURSOR:
					WmSetCursor(ref msg);
					break;
				default:
					base.WndProc(ref msg);
					break;
			}
		}

        public override Rectangle DisplayRectangle
        {
            get
            {
                if (this.Parent!=null && (this.Parent as SplitterControl).ShowVerticalScrollBar)
                   return new Rectangle(0, 0, this.Width - SystemInformation.VerticalScrollBarWidth, this.Height);
               else
                   return base.DisplayRectangle;
            }
        }

		/// <override/>
		protected override void OnPaint(PaintEventArgs pe)
		{
			lock(this)
			{
				if( this.SplitBars != DynamicSplitBars.None )
				{
                    Rectangle hBounds = Rectangle.Empty;
                    Rectangle vBounds = Rectangle.Empty;

                    if( this.Parent is TabBarSplitterControl && ( this.Parent as TabBarSplitterControl ).Style == TabBarSplitterStyle.Office2007 )
                    {
                        TabBarSplitterControl control = this.Parent as TabBarSplitterControl;

                        using( Brush br = new SolidBrush( control.Office2007ColorTable.TabBarSplitterBackColor ) )
                        {
                            pe.Graphics.FillRectangle( br, this.Bounds );
                        }

                        if( horizontalSplitterBar != null && !horizontalSplitterBar.Bounds.IsEmpty )
                        {
                            horizontalSplitterBar.Style = control.Style;
                            horizontalSplitterBar.Office2007ColorScheme = control.Office2007ColorScheme;
                            horizontalSplitterBar.Paint( pe.Graphics, flatLook );
                            hBounds = horizontalSplitterBar.Bounds;
                        }

                        if( verticalSplitterBar != null && !verticalSplitterBar.Bounds.IsEmpty )
                        {
                            verticalSplitterBar.Style = control.Style;
                            verticalSplitterBar.Office2007ColorScheme = control.Office2007ColorScheme;
                            verticalSplitterBar.Paint( pe.Graphics, flatLook );
                            vBounds = verticalSplitterBar.Bounds;

                            if( vBounds.IntersectsWith( hBounds ) )
                            {
                                Rectangle intersect = Rectangle.Intersect( hBounds, vBounds );
                                intersect.X++;                                
                                intersect.Width -= 3;
                                Color startColor = control.Office2007ColorTable.TabBarSplitterTabStartColor;
                                Color endColor = control.Office2007ColorTable.TabBarSplitterTabEndColor;
                                using( LinearGradientBrush br = new LinearGradientBrush( Rectangle.Inflate( intersect, 0, 1 ), startColor, endColor, LinearGradientMode.Vertical ) )
                                {
                                    pe.Graphics.FillRectangle( br, intersect );
                                }
                            }
                        }
                    }
                    else
                    {
                        if( horizontalSplitterBar != null && !horizontalSplitterBar.Bounds.IsEmpty )
                        {
                            horizontalSplitterBar.Style = TabBarSplitterStyle.Default;
                            horizontalSplitterBar.Paint( pe.Graphics, flatLook );
                            hBounds = horizontalSplitterBar.Bounds;
                        }
                        
                        if (verticalSplitterBar != null && !verticalSplitterBar.Bounds.IsEmpty)
                        {
                            verticalSplitterBar.Style = TabBarSplitterStyle.Default;
                            verticalSplitterBar.Paint(pe.Graphics, flatLook);
                            vBounds = verticalSplitterBar.Bounds;

                            if (vBounds.IntersectsWith(hBounds))
                            {
                                Rectangle intersect = Rectangle.Intersect(hBounds, vBounds);
                                intersect.Width -= 3;
                                intersect.X++;
                                pe.Graphics.FillRectangle(SystemBrushes.Control, intersect);
                            }
                        }
                    }

                    pe.Graphics.ExcludeClip( hBounds );
                    pe.Graphics.ExcludeClip( vBounds );
				}

#if DEBUG
				if (Switches.TabBarSplitterControlEvents.TraceVerbose)
				    TraceUtil.TraceCurrentMethodInfo(pe.ClipRectangle);
#else
				;
#endif

				base.OnPaint(pe);
			}
        }

        internal Rectangle ReverseRectangleRTL( Rectangle r )
        {
            if( this.RightToLeft == RightToLeft.Yes )
            {
                return new Rectangle( this.ClientRectangle.Width - r.Right, r.Top, r.Width, r.Height );
            }
            else
            {
                return r;
            }
        }

		/// <override/>
		protected override void OnLayout(LayoutEventArgs levent)  
		{
			if (Size.IsEmpty)
				return;

			foreach (Control c in Controls)
			{
				IScrollBarWrapperContainer sc = c as IScrollBarWrapperContainer;
				if (sc != null)
					sc.UpdateScrollBars();
			}
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(levent.AffectedProperty, levent.AffectedControl);
#else
			;
#endif

			base.OnLayout(levent);
        }

		/// <overload>
		///     Initializes a new TabBarPage.
		/// </overload>
		/// <summary>
	    ///     Constructs a TabBarPage with text for the tab.
	    /// </summary>
	    /// <param name='text'>
	    ///     The text for this tab.
	    /// </param>
        public TabBarPage(string text)  
        {
			splitterInfo = new SplitterInfo(this);
            Text = text;
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|WhidbeyCompatibleControlStyles.DoubleBuffer, true);
		}

		/// <summary>
		///     Initializes a new TabBarPage.
		/// </summary>
		public TabBarPage()  
			: this(null)
        {
        }

        // Methods

		/// <override/>
		public override string ToString()  
        {
            return String.Concat("TabBarPage: {", Text, "}");
        }

        /// <summary>
        ///     Given a component, this retrieves the tab page that it is parented to or
        /// NULL if it is not parented to any tab page.
        /// </summary>
        /// <param name='comp'>
        ///     The component to check.
        /// </param>
        /// <returns>
        ///     A TabBarPage that the component is parented to or NULL if
        ///     no such page exists. This will return the component if it
        ///     is an instance of TabBarPage.
        /// </returns>
        public static TabBarPage GetTabBarPageOfComponent(object comp)  
        {
 			Control c = comp as Control;
			while (c != null)
			{
				if (c is TabBarPage)
					return (TabBarPage) c;

				c = c.Parent;
			}

			return null;
        }

		/// <override/>
		protected override Control.ControlCollection CreateControlsInstance()  
        {
            return new TabBarPage.TabBarPageControlCollection(this);
        }

        internal void UpdateParent()  
        {
            Control control = Parent;
			if (control is TabBarSplitterControl) 
			{
				TabBarSplitterControl tc = (TabBarSplitterControl) control;
				int index = tc.TabBarPages.IndexOf(this);
				if (index != -1)
					tc.TabBarPages[index] = this;
			}
		}

        // Properties

		/// <override/>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AnchorStyles Anchor
        {
            get 
            {
                return base.Anchor;
            }
            set 
            {
                base.Anchor = value;
            }
        }

		/// <override/>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DockStyle Dock
        {
            get 
            {
                return base.Dock;
            }
            set 
            {
                base.Dock = value;
            }
        }

		/// <summary>
		/// Enables / disables this page in a <see cref="TabBarSplitterControl"/>.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Enabled
        {
            get 
            {
                return base.Enabled;
            }
            set 
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or set the value indicating whether the TabBarPage is enabled or not
        /// </summary>
        [Description("Specifies whether the TabBarPage is enabled or not."), Category("Appearance"), DefaultValueAttribute(true),
        Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), RefreshProperties(RefreshProperties.All)]

        public bool TabEnabled
        {
            get
            {
                return tabEnabled;
            }
            set
            {
                if (tabEnabled != value)
                {
                    tabEnabled = value;
                    UpdateParent();
                }

            }
        }


		/// <override/>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex
        {
			get 
            {
                return base.TabIndex;
            }
            set 
            {
                base.TabIndex = value;
            }
        }

		/// <override/>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop
        {
            get 
            {
                return base.TabStop;
            }
            set 
            {
                base.TabStop = value;
            }
        }

		/// <override/>
		[Browsable(true)]
		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
		
		/// <override/>
		protected override void OnTextChanged(EventArgs e)
        {
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, Text);
#else
			;
#endif

			base.OnTextChanged(e);
			UpdateParent();
        }

        /// <summary>
        ///     Gets / sets the ToolTip text for the tab that will appear when the mouse hovers
        ///     over the tab and the TabBarSplitterControl's showToolTips property is True.
        /// </summary>
        [
            DefaultValueAttribute(""),
        ]
        public string ToolTipText
        {
            get 
            {
                return toolTipText;
            }
            set 
            {
                if (value == null || value == toolTipText)
                    return ;
		   
                toolTipText = value;
				OnToolTipTextChanged(EventArgs.Empty);
            }
        }

		/// <summary>
		/// Raises the <see cref="ToolTipTextChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnToolTipTextChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.ToolTipText);
#else
			;
#endif

			if (ToolTipTextChanged != null)
				ToolTipTextChanged(this, e);

			UpdateParent();
		}

        private bool ignoreVisibilityChange = false;
        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
        private bool visible = true;
        /// <summary>
        /// Gets or sets the visibility of the control.
        /// </summary>
		public new bool Visible
        {
            get 
            {
                return this.visible;
            }
            set
            {
                this.visible = value;

                if (!value && !ignoreVisibilityChange)
                {
                    TabBarSplitterControl owner = this.Parent as TabBarSplitterControl;
                    if (owner != null)
                        owner.HidePage(this);
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
		internal bool Active
        {
            get 
            {
                return base.Visible;
            }
            set 
            {
                ignoreVisibilityChange = true;
                base.Visible = value;
                ignoreVisibilityChange = false;
            }
        }

		DynamicSplitBars IDynamicSplitterFrame.SplitBars
		{
			get 
			{
				return SplitBars;
			}
		}

		int IDynamicSplitterFrame.RowCount 
		{ 
			get
			{
				return RowCount; 
			}
		}

		int IDynamicSplitterFrame.ColumnCount 
		{ 
			get
			{
				return ColumnCount; 
			}
		}

		Control IDynamicSplitterFrame.ActivePane 
		{ 
			get 
			{ 
				return ActivePane; 
			} 
			set 
			{ 
				ActivePane = value; 
			} 
		}

		/// <summary>
		/// Gets / sets a value indicating what split behavior is supported. Rows, columns or both.
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint)]
		public DynamicSplitBars SplitBars
        {
            get
            {
                return splitterInfo.splitBars;
            }
            set
            {
				if (SplitBars != value)
				{
					splitterInfo.splitBars = value;
					UpdateSplitter();
					OnSplitBarsChanged(EventArgs.Empty);
				}
            }
        }

		/// <summary>
		/// Raises the <see cref="SplitBarsChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnSplitBarsChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.SplitBars);
#else
			;
#endif

			try
			{
				if (SplitBarsChanged != null)
					SplitBarsChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}

		/// <summary>
		/// Gets / sets the horizontal splitter position in percentages of the splitter control's width.
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(100)]
		public int HSplitPos
        {
            get
            {
                if ((SplitBars & DynamicSplitBars.SplitColumns) == 0)
                    return 100;
                
                SplitterLayout splitterLayout = SplitterLayout;
                int hSplitPos = splitterLayout._hSplitPos;
                if (hSplitPos < 5 || hSplitPos > 98)
                    hSplitPos = 100;

                return hSplitPos;
            }
            set
            {
                if (value < 5 || value > 98)
                    value = 100;

				if (value != HSplitPos)
				{
					SplitterLayout splitterLayout = SplitterLayout;
					splitterLayout._hSplitPos = value;
					UpdateSplitter();
					OnHSplitPosChanged(EventArgs.Empty);
				}
            }
        }

		/// <summary>
		/// Raises the <see cref="HSplitPosChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnHSplitPosChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.HSplitPos);
#else
			;
#endif

			try
			{
				if (HSplitPosChanged != null)
					HSplitPosChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}
		
		/// <summary>
		/// Gets / sets the vertical splitter position in percentages of the splitter control's height.
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(100)]
		public int VSplitPos
        {
            get
            {
                if ((SplitBars & DynamicSplitBars.SplitRows) == 0)
                    return 100;

                SplitterLayout splitterLayout = SplitterLayout;
                int vSplitPos = splitterLayout._vSplitPos;
                if (vSplitPos < 5 || vSplitPos > 98)
                    vSplitPos = 100;

                return vSplitPos;
            }
            set
            {
                if (value < 5 || value > 98)
                    value = 100;

				if (value != VSplitPos)
				{
					SplitterLayout splitterLayout = SplitterLayout;
					splitterLayout._vSplitPos = value;
					UpdateSplitter();
					OnVSplitPosChanged(EventArgs.Empty);
				}
            }
        }

		/// <summary>
		/// Raises the <see cref="VSplitPosChanged"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnVSplitPosChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.VSplitPos);
#else
			;
#endif

			try
			{
				if (VSplitPosChanged != null)
					VSplitPosChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}

		void UpdateSplitter()
		{
			SuspendLayout();
			SplitterLayout splitterLayout = SplitterLayout;
			if (splitterLayout._hSplitPos == 100 && ColumnCount > 1)
				DeleteColumn(1);
			if (splitterLayout._vSplitPos == 100 && RowCount > 1)
				DeleteRow(1);
			ResumeLayout();
			if (Parent != null)
				Parent.PerformLayout();
			Refresh();
		}

		/// <summary>
		/// Gets / sets the <see cref="SplitterLayout"/> that holds information about current vertical and horizontal split positions.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
        public virtual SplitterLayout SplitterLayout
        {
            get
            {
                if (splitterInfo.splitterLayout == null)
                {
                    splitterInfo.splitterLayout = new SplitterLayout();
                }
                return splitterInfo.splitterLayout;
            }
            set
            {
				if (splitterInfo.splitterLayout != value)
				{
					splitterInfo.splitterLayout = value;
					UpdateSplitter();
					OnSplitterLayoutChanged(EventArgs.Empty);
				}
            }
		}

		/// <summary>
		/// Raises the <see cref="SplitterLayoutChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnSplitterLayoutChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.SplitterLayout);
#else
			;
#endif

			try
			{
				if (SplitterLayoutChanged != null)
					SplitterLayoutChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}


		/// <override/>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
        public new Size Size
        {
            get 
            {
                if (DesignMode && Parent != null)
                {
                    Size size = Parent.Size;
        			int sbHeight = SystemInformation.HorizontalScrollBarHeight;
                    size.Height -= sbHeight;
                    return size;
                }
                return base.Size;
            }
            set 
            {
                base.Size = value;
            }
        }

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.ThemesEnabled);
#else
			;
#endif

			TabBarSplitterControl tabBarSplitter = this.SplitterParent as TabBarSplitterControl;

			if( tabBarSplitter != null )
			{
				tabBarSplitter.Bar.Invalidate();
			}


			if(this.ThemeChanged != null)
			{
				try
				{
					this.ThemeChanged(this, e);
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(this, ex))
						throw;
				}
			}
			foreach (Control control in Controls)
			{
				IThemedControl themedControl = control as IThemedControl;
				if (themedControl != null)
					themedControl.ThemesEnabled = this.ThemesEnabled;
			}
		}

		/// <override/>
		protected override void OnControlAdded(ControlEventArgs e)
		{
			IThemedControl themedControl = e.Control as IThemedControl;
			if (themedControl != null)
				themedControl.ThemesEnabled = this.ThemesEnabled;
#if DEBUG
			
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			
			    TraceUtil.TraceCurrentMethodInfo(Name, e.Control);
#else
			
			;
#endif

			_ControlAdded(e);
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		public virtual bool ThemesEnabled
		{
			get{return this.themesEnabled;}
			set
			{
				if(this.themesEnabled != value)
				{
					this.themesEnabled = value;
					this.OnThemeChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
		public event EventHandler ThemeChanged;

		private bool themesEnabled = false;



		internal bool validatingFailed = false;
		int inActivateControl = 0;

		/// <summary>
		/// Activates a specified control.
		/// </summary>
		/// <param name="c">The <see cref="Control"/> being activated.</param>
		/// <returns>True if the control is successfully activated; False otherwise.</returns>
		/// <remarks>
		/// The control must be a child of the container control.
		/// </remarks>
		public bool ActivateControl(Control c)
		{
#if DEBUG
			if (Switches.Workbook.TraceVerbose)
				TraceUtil.TraceCurrentMethodInfo(inActivateControl, "----BEGIN----", this, c);
#else
			;
#endif

			try
			{
				inActivateControl++;
				this.validatingFailed = false;
				Control active = ActiveControl;
				if (c != active) 
					base.ActiveControl = c;
				if (ActiveControl != c)
					this.validatingFailed = true;

				int row, column;
				if (FindPane(ActiveControl, out row, out column))
				{
					splitterInfo.activeRow = row;
					splitterInfo.activeColumn = column;
				}

				return !validatingFailed;
			}
			finally
			{
#if DEBUG
				if (Switches.Workbook.TraceVerbose)
				    TraceUtil.TraceCurrentMethodInfo(inActivateControl, this.validatingFailed, "----END----", this, ActiveControl);
#else
				;
#endif

				inActivateControl--;
			}
		}

		/// <override/>
		public new Control ActiveControl
		{
			get
			{
				return base.ActiveControl;
			}
			set
			{
				ActivateControl(value);
			}
		}


		// Focus
		bool isActiveControl = false;
		bool isValidating = false;
		bool isDeactivatedCalled = false;
		bool hasControlFocus = false;
		bool isValidated = false;


		/// <summary>
		/// Indicates whether the <see cref="OnValidating"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsValidating
		{
			get
			{
				return isValidating;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="OnValidated"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsValidated
		{
			get
			{
				return isValidated;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="OnEnter"/> has been called. <see cref="OnLeave"/> resets this flag.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsActiveControl
		{
			get
			{
				return isActiveControl;
			}
		}

		/// <summary>
		/// Indicates whether both <see cref="OnDeactivated"/> has been called. <see cref="OnEnter"/> resets this flag.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDeactivated
		{
			get
			{
				return isDeactivatedCalled;
			}
		}

		/// <summary>
		/// Indicates whether both <see cref="OnControlGotFocus"/> has been called. <see cref="OnControlLostFocus"/> resets this flag.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasControlFocus
		{
			get
			{
				return hasControlFocus;
			}
		}

		/// <override/>
		protected override void OnEnter(EventArgs e)
		{
			isActiveControl = true;
			isValidating = false;
			isValidated = false;
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			base.OnEnter(e);
		}

		/// <override/>
		protected override void OnLeave(EventArgs e)
		{
			isActiveControl = false;
			isValidating = false;
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			base.OnLeave(e);

			if (!this.CausesValidation && !hasControlFocus)
				OnDeactivated(e);
		}

		/// <override/>
		protected override void OnValidating(CancelEventArgs e)
		{
			isValidating = true;
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(e.Cancel, this);
#else
			;
#endif

			base.OnValidating(e);

			if (!e.Cancel)
				e.Cancel = !this.Validate();
		}

		/// <override/>
		protected override void OnValidated(EventArgs e)
		{
			isValidating = false;
			isValidated = true;
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			base.OnValidated(e);

			if (!isActiveControl && !hasControlFocus)
				OnDeactivated(e);
		}

		/// <override/>
		protected override void OnLostFocus(EventArgs e)
		{
			RaiseControlLostFocus();
		}

		/// <override/>
		protected override void OnGotFocus(EventArgs e)
		{
			RaiseControlGotFocus();
		}

		/// <summary>
		/// Occurs when both <see cref="OnControlLostFocus"/> and <see cref="OnLeave"/> occurs.
		/// </summary>
		public event EventHandler Deactivated;

		/// <summary>
		/// Raises the <see cref="Deactivated"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
		protected virtual void OnDeactivated(EventArgs e)
		{
			isDeactivatedCalled = true;
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			if (Deactivated != null)
				Deactivated(this, e);
		}

		void ChildGotFocus(object sender, EventArgs e)
		{
			RaiseControlGotFocus();
		}

		void ChildLostFocus(object sender, EventArgs e)
		{
			RaiseControlLostFocus();
		}

		/// <summary>
		/// Indicates whether this control contains focus. If <see cref="ActiveControl"/> 
		/// implements <see cref="IQueryFocusInside"/>, the <see cref="IQueryFocusInside.QueryFocusInside"/>
		/// method is called on the <see cref="ActiveControl"/>.
		/// </summary>
		/// <returns>True if the control or any child control has focus; False otherwise.</returns>
		public virtual bool QueryFocusInside()
		{ 
			if (ContainsFocus)
				return true;
			
			IQueryFocusInside qfi = this.ActiveControl as IQueryFocusInside;
			if (qfi != null)
				return qfi.QueryFocusInside();

			return false;
		}
		
		/// <summary>
		/// Raises the <see cref="Control.GotFocus"/> event. This method is called when the control
		/// or any child control gets focus and this control did not have focus before.
		/// </summary>
		/// <remarks>
		/// Inheriting classes should override this method instead of overriding <see cref="Control.OnGotFocus"/>
		/// because <see cref="OnControlGotFocus"/> is also called when child controls get focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlGotFocus()
		{
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			base.OnGotFocus(EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="Control.LostFocus"/> event. This method is called when the control
		/// or any child control loses focus and the newly focused control is not a child of this control. 
		/// </summary>
		/// <remarks>
		/// Inheriting classed should override this method instead of overriding <see cref="Control.OnLostFocus"/>
		/// because <see cref="OnControlLostFocus"/> is also called when child controls lose focus and it
		/// is not called when focus is moved within child controls of this control.
		/// </remarks>
		protected virtual void OnControlLostFocus()
		{
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this);
#else
			;
#endif

			base.OnLostFocus(EventArgs.Empty);
			if (!isActiveControl && !isValidating && !(CausesValidation && !this.isValidated))
				OnDeactivated(EventArgs.Empty);
			else if (isValidating)
				OnValidatingLostFocus();
		}

		/// <summary>
		/// This method is called if the control's <see cref="OnControlLostFocus"/> notification occurs
		/// while handling a <see cref="Control.Validating"/> event. This typically occurs if a
		/// message box is displayed from a <see cref="Control.Validating"/> event handler.
		/// </summary>
		protected virtual void OnValidatingLostFocus()
		{
		}

		void RaiseControlGotFocus()
		{
			if (!this.hasControlFocus)
			{
				hasControlFocus = true;
				OnControlGotFocus();
			}
		}

		void RaiseControlLostFocus()
		{
			if (!hasControlFocus)
			{
				TraceUtil.TraceCurrentMethodInfo("-Duplicate call-", isValidating, this); 
			}
			else if (!QueryFocusInside())
			{
				hasControlFocus = false;
				OnControlLostFocus();
			}
		}


		/// <override/>
		protected override void OnControlRemoved(System.Windows.Forms.ControlEventArgs e)
		{
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(e.Control, this);
#else
			;
#endif

			base.OnControlRemoved(e);
			
			UnwireChildControl(e.Control);
		}

		protected virtual void WireChildControl(Control c)
		{
			c.GotFocus += new EventHandler(ChildGotFocus);
			c.LostFocus += new EventHandler(ChildLostFocus);
			c.KeyDown += new KeyEventHandler(ChildKeyDown);
		}

		protected virtual void UnwireChildControl(Control c)
		{
			c.GotFocus -= new EventHandler(ChildGotFocus);
			c.LostFocus -= new EventHandler(ChildLostFocus);
			c.KeyDown -= new KeyEventHandler(ChildKeyDown);

			foreach (Control cc in c.Controls)
			{
				IScrollBarWrapperContainer sw = cc as IScrollBarWrapperContainer;
				if (sw != null)
				{
					if (sw.HScrollBar != null)
						sw.HScrollBar.InnerScrollBar = null;

					if (sw.VScrollBar != null)
						sw.VScrollBar.InnerScrollBar = null;
				}
			}
		}

		void _ControlAdded(System.Windows.Forms.ControlEventArgs e)
		{
			base.OnControlAdded(e);

			WireChildControl(e.Control);
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged (e);
		}



		internal void ChildKeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Handled)
			{
				TabBarSplitterControl tsc = Parent as TabBarSplitterControl;
				if (tsc != null)
				{
					Keys keyCode = e.KeyCode;
					bool controlKeyDown = (e.Modifiers & Keys.Control) != Keys.None;

					switch (keyCode)
					{
						case Keys.Next:
							if (controlKeyDown)
							{
								tsc.ActivateNextPage(false);
								e.Handled = true;
							}
							break;
						case Keys.Prior:
							if (controlKeyDown)
							{							
								tsc.ActivateNextPage(true);
								e.Handled = true;
							}
							break;
					}
				}
			}
		}

		

        class TabBarPageControlCollection : Control.ControlCollection 
        {
            /// <summary>
            ///      Creates a new TabBarPageControlCollection.
            /// </summary>
            /// <param name='owner'>
            ///      The owner of this collection. This is the control whose child
            ///      controls we are to represent.
            /// </param>
            public TabBarPageControlCollection(TabBarPage owner)  
                : base(owner)
            {
            }

            /// <summary>
            ///    <para>Adds a child control to this control. The control becomes the last control
            ///       in the child control list. If the control is already a child of another
            ///       control, it is first removed from that control. The tab page overrides
            ///       this method to ensure that child tab pages are not added to it, as these
            ///       are illegal.</para>
            /// </summary>
            /// <param name="value">The child control to be added.</param>
            /// <exception cref="System.SystemException">If the specified control is a toplevel control or if a circular control reference would result.</exception>
            public override void Add(Control value)  
            {
                if (value is TabBarPage) 
                    throw new SystemException(SR.GetString("TabBarPageOnTabBarPage"));
                    
                base.Add(value);
            }
        }


		/// <summary>
		/// Gets / sets the icon displayed on the tab. 
		/// </summary>
		/// <value>An Icon value.</value>
		[
		Description("The icon displayed on the tab."),
		Category("Appearance"),
		DefaultValue(null),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public Icon Icon
		{
			get { return icon; }
			set 
			{ 
				if(this.icon != value)
				{
					this.icon = value; 
					if (Active && !Bounds.IsEmpty && Parent != null)
					{
						Parent.PerformLayout();
						Parent.Refresh();
					}
				}
			}
		}	

		/// <summary>
		/// Gets or sets back color for TabBarPage Tab.
		/// </summary>
		[
		Description("Gets or sets back color for TabBarPage Tab."),
		Category("Appearance"),
		DefaultValue( typeof( SystemColors ), "Control" ),
		RefreshProperties(RefreshProperties.Repaint)
		]
		public Color TabBackColor
		{
			get
			{
				return m_tabBackColor;
			}
			set
			{
				if( value != m_tabBackColor )
				{
					m_tabBackColor = value;
				}
			}
		}

	}
}
