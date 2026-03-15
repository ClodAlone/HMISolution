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
using System.Text;
using System.Security;
using System.Security.Permissions;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{


    /// <summary>
    /// Implements a list box that can be displayed in a drop-down window for 
    /// a combo box or dropdown menus. It lets you select items and scroll the
    /// listbox without setting the focus to the listbox.
    /// </summary>
    [ToolboxItem(false)]
    public class FocuslessListBox : ListBox
    {
        // Fields
        internal Timer timer = new Timer();
        internal int direction = 0;
        internal Point oldPoint = Point.Empty;

        // Constructors

        /// <summary>
        /// Initializes a new <see cref="FocuslessListBox"/> control.
        /// </summary>
        public FocuslessListBox()
        {
            this.Size = new Size(2, 2);
            this.Location = new Point(0, 0);
            this.DrawMode = DrawMode.Normal;
            this.IntegralHeight = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.UserMouse, true);
            this.CausesValidation = false;
        }

        // Methods

        /// <summary>
        /// Handles the <see cref="Timer.Tick"/> event of an internal
        /// timer that is started when the user has pressed the mouse down.
        /// </summary>
        /// <param name="sender">The source of the event.></param>
        /// <param name="e">The <see cref="EventArgs"/> with event data.</param>
        protected virtual void OnTimerEvent(object sender, EventArgs e)
        {
            switch (this.direction)
            {
                case -1:
                    if (this.TopIndex > 0)
                    {
                        this.TopIndex = (this.TopIndex - 1);
                        this.SelectedIndex = (this.SelectedIndex - 1);
                    }
                    break;
                case 1:
                    int linesVisible = this.Height / this.ItemHeight;
                    if (this.TopIndex < ((this.Items.Count - linesVisible)))
                    {
                        this.TopIndex = (this.TopIndex + 1);
                        this.SelectedIndex = (this.SelectedIndex + 1);
                    }
                    break;
            }
        }

        /// <override/>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Tick -= new EventHandler(this.OnTimerEvent);
            }
            if (this.IsHandleCreated)
                this.Capture = false;

            base.OnMouseUp(e);
        }

        /// <override/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnClick(null);
            base.OnMouseDown(e);

            this.Capture = true;
            this.oldPoint = Point.Empty;
            this.timer.Tick += new EventHandler(this.OnTimerEvent);
            this.timer.Stop();

            this.OnMouseMove(e);
        }

        /// <override/>
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == 0x21/*WM_MOUSEACTIVATE*/)
            {
                msg.Result = (IntPtr)3; //MA_NOACTIVATE
                return;
            }

            switch (msg.Msg)
            {
                case 277: // WM_VSCROLL:
                    this.WmVScroll(ref msg);
                    break;

                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        private void WmVScroll(ref Message m)
        {
            OnClick(null);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Sends WM_SETREDRAW message to the window.
        /// </summary>
        /// <param name="value"></param>
        public void SetRedraw(bool value)
        {
            NativeMethods.SendMessage(Handle, 11/*0xb WM_SETREDRAW*/, value ? 1 : 0, 0);
        }

        // Note: GridComboBoxCell subscribes to the following events to handle focus
        //        this.listBoxPart.MouseUp += new MouseEventHandler(this.ListBoxMouseUp);
        //        this.listBoxPart.Click += new EventHandler(ListBoxClick);

    }
}


namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the list box that can be displayed in a drop-down window for 
    /// a combo box. Handles autoscrolling and resize to fit contents.
    /// </summary>
	[ToolboxItem(false)]
	public class GridComboBoxListBoxPart : ListBox
    {
        // Fields
        internal Timer timer = new Timer();
        internal int direction = 0;
        internal Point oldPoint = Point.Empty;
        private int dropDownRows = 6;

        // Constructors

		/// <summary>
		/// Initializes a new <see cref="GridComboBoxListBoxPart"/> control.
		/// </summary>
        public GridComboBoxListBoxPart()  
        {
            this.Size = new Size(2,2);
            this.Location = new Point(0,0);
            this.DrawMode = DrawMode.Normal;
            this.IntegralHeight = true;
            this.BorderStyle = BorderStyle.FixedSingle;
			this.SetStyle(ControlStyles.Selectable, false);
			this.SetStyle(ControlStyles.UserMouse, true);
			this.CausesValidation = false;
        }

        // Methods

		/// <summary>
		/// Handles the <see cref="Timer.Tick"/> event of an internal
		/// timer that is started when the user has pressed the mouse down.
		/// </summary>
		/// <param name="sender">The source of the event.></param>
		/// <param name="e">The <see cref="EventArgs"/> with event data.</param>
        protected virtual void OnTimerEvent(object sender, EventArgs e)  
        {
            switch(this.direction) 
            {
            case -1:
                if (this.TopIndex > 0)
                {
                    this.TopIndex = (this.TopIndex - 1);
                    this.SelectedIndex = (this.SelectedIndex - 1);
                }
                break;
            case 1:
                int linesVisible = this.Height / this.ItemHeight;
                if (this.TopIndex < ((this.Items.Count - linesVisible) )) 
                {
                    this.TopIndex = (this.TopIndex + 1);
                    this.SelectedIndex = (this.SelectedIndex + 1);
                }
                break;
            }
        }

		/// <override/>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)  
        {
            // Compute the size we want to be, DropDownHolder will use this size.
			if ((specified & (BoundsSpecified.Height|BoundsSpecified.Width)) != 0)
			{
				//Graphics g = this.CreateGraphics();

				if ((specified & BoundsSpecified.Height) != 0)
				{
					int linesVisible = Math.Min(this.dropDownRows, this.Items.Count);
//				    SizeF sz = g.MeasureString("Wg", this.Font);
					float lineHeight = ItemHeight;
					height = (int) Math.Ceiling(linesVisible*lineHeight+2);
				}

				if ((specified & BoundsSpecified.Width) != 0)
				{
					width = Math.Max(this.GetOptimalWidth(), width );
				}
				//g.Dispose();
			}

            base.SetBoundsCore(x, y, width, height, specified);
        }

		/// <summary>
		/// Overloaded. Calculates optimal width for this list box based on current items.
		/// </summary>
		/// <returns>The width in pixels large enough so that no item text needs to be clipped.</returns>
		protected int GetOptimalWidth()  
		{
			Graphics g = this.CreateGraphics();
			int width = GetOptimalWidth(g);
			g.Dispose();
			
			width = Math.Min( width, SystemInformation.WorkingArea.Width );
			return width;
		}

		/// <summary>
		/// Calculates optimal width for this list box based on current items using a
		/// provided <see cref="Graphics"/> object.
		/// </summary>
		/// <returns>The width in pixels large enough so that no item text needs to be clipped.</returns>
		protected virtual int GetOptimalWidth(Graphics g)  
        {
            int minWidth = 0;
            for (int n = 0; n < this.Items.Count; n++)
            {
                Size size = g.MeasureString(GetItemText(this.Items[n]), this.Font).ToSize();
                minWidth = Math.Max(size.Width, minWidth);
            }
            if (this.Items.Count > this.dropDownRows) 
                minWidth = (minWidth + SystemInformation.VerticalScrollBarWidth);
            return minWidth;
        }

		/// <override/>
		protected override void OnMouseUp(MouseEventArgs e)  
        {
			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer.Tick -= new EventHandler(this.OnTimerEvent);
			}
			if(this.IsHandleCreated)
				this.Capture = false;
         
            base.OnMouseUp(e);
        }

		/// <override/>
		protected override void OnMouseMove(MouseEventArgs e)  
        {
            if (e == null || e.X == this.oldPoint.X && e.Y == this.oldPoint.Y) 
            {
                base.OnMouseMove(e);
                return;
            }

            this.oldPoint.X = e.X;
            this.oldPoint.Y = e.Y;
		   
            Rectangle bounds = this.Bounds;
            if ((bounds.Contains(e.X,e.Y)))
            {
                this.timer.Stop();
                int index = this.IndexFromPoint(e.X,e.Y);
                if (index != this.SelectedIndex && index != -1) 
                    this.SetSelected(index, true);
                this.direction = 0;
            }
            else
            {
                int delta = 0;
                if (e.Y > this.Height) 
                {
                    delta = (e.Y - this.Height);
                    this.direction = 1;
                }
                else
                {
                    delta = (-e.Y);
                    this.direction = -1;
                }

                this.timer.Interval = Math.Max(5, 100-delta*2);
                if (!this.timer.Enabled) 
                    this.timer.Start();

                this.OnTimerEvent(null, null);
            }
        }

		/// <override/>
		protected override void OnMouseDown(MouseEventArgs e)  
        {
            OnClick(null);
            base.OnMouseDown(e);

            this.Capture = true;
            this.oldPoint = Point.Empty;
            this.timer.Tick += new EventHandler(this.OnTimerEvent);
            this.timer.Stop();

            this.OnMouseMove(e);
        }

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message msg) 
		{
			if (msg.Msg == 0x21/*WM_MOUSEACTIVATE*/) 
			{
				msg.Result = (IntPtr)3; //MA_NOACTIVATE
				return;
			}

            switch (msg.Msg) 
            {
            case NativeMethods.WM_VSCROLL:
                this.WmVScroll(ref msg);
                break;

            default:
                base.WndProc(ref msg);
                break;
            }
        }

        private void WmVScroll(ref Message m)  
        {
            OnClick(null);
            base.WndProc(ref m);
        }

        // Note: GridComboBoxCell subscribes to the following events to handle focus
        //        this.listBoxPart.MouseUp += new MouseEventHandler(this.ListBoxMouseUp);
        //        this.listBoxPart.Click += new EventHandler(ListBoxClick);

        // Properties

		/// <summary>
		/// Gets / sets the preferred number of visible rows.
		/// </summary>
        public int DropDownRows
        {
            get
            {
                return this.dropDownRows;
            }
            set
            {
                this.dropDownRows = value;
            }
        }


		BindingContext bindingContext;
	
		/// <override/>
		public override BindingContext BindingContext
		{
			get
			{
				if (bindingContext == null)
					bindingContext = new BindingContext();
				return bindingContext;
			}
			set
			{
			}
		}

		public override int SelectedIndex
		{
			get
			{
				return base.SelectedIndex;
			}
			set
			{
				if (value != base.SelectedIndex && value < this.Items.Count)
					base.SelectedIndex = value;
			}
		}


    }

}

