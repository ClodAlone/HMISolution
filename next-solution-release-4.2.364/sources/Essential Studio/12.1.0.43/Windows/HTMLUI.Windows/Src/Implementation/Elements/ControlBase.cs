#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Base class for supporting custom controls.
    /// </summary>
    public abstract class ControlBase : ICustomControlBase
    {
        #region Class constants
        /// <summary>
        /// Resolution of the control image during printing.
        /// </summary>
        private const int DEF_PRINT_DPI = 100;
        #endregion

        #region Class members
        /// <summary>
        /// Custom control object.
        /// </summary>
        private Control m_control;

        /// <summary>
        /// Parent custom tag.
        /// </summary>
        private BaseElement m_parent;

        /// <summary>
        /// Indicates whether events were attached.
        /// </summary>
        private bool m_bEventsAttached;

        /// <summary>
        /// Indicates whether TabIndex can be assigned to custom control.
        /// </summary>
        private bool m_bAssignTabIndex = true;

        /// <summary>
        /// Indicates quite mode state.
        /// </summary>
        private bool m_bQuiteMode;

        /// <summary>
        /// Default size of the control.
        /// </summary>
        private Size m_defaultSize;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the custom control object.
        /// </summary>
        public Control CustomControl
        {
            get
            {
                return m_control;
            }
        }

        /// <summary>
        /// Gets the parent element container for the custom control.
        /// </summary>
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets the BaseElement from parent tag element.
        /// </summary>
        protected internal BaseElement ParentElement
        {
            get
            {
                return (BaseElement)this.Parent;
             }
        }

        /// <summary>
        /// Gets or sets a value indicating whether events were attached to user control.
        /// </summary>
        protected internal bool EventsAttached
        {
            get
            {
                return m_bEventsAttached;
            }
            set
            {
                if (m_bEventsAttached != value)
                {
                    m_bEventsAttached = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Quite mode state.
        /// </summary>
        protected bool QuiteMode
        {
            get
            {
                return m_bQuiteMode;
            }
            set
            {
                if (m_bQuiteMode != value)
                {
                    m_bQuiteMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default size of the control.
        /// </summary>
        public Size DefaultSize
        {
            get
            {
                return m_defaultSize;
            }
            set
            {
                if (m_defaultSize != value)
                {
                    m_defaultSize = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the ControlBase class
        /// </summary>
        protected ControlBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ControlBase class
        /// </summary>
        /// <param name="parent">Parent custom tag element.</param>
        /// <param name="customControl">Custom control instance.</param>
        protected ControlBase(IHTMLElement parent, Control customControl)
            : this()
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (customControl == null)
                throw new ArgumentNullException("customControl");

            m_parent = parent as BaseElement;
            SetControl(customControl);
        }

        /// <summary>
        /// Initializes a new instance of the ControlBase class
        /// </summary>
        /// <param name="parent">Parent element for this object.</param>
        protected ControlBase(IHTMLElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            m_parent = parent as BaseElement;
        }

        /// <summary>
        /// Disposes of control and releases all of its resources.
        /// </summary>
        public virtual void DisposeControl()
        {
            if (m_control != null && !m_control.IsDisposed)
            {
                DetachEvents();

                m_control.Dispose();
            }

            m_control = null;
        }
        #endregion

        #region Class abstract methods
        /// <summary>
        /// Sets the location of the custom controls.
        /// </summary>
        public abstract void SetLocation();

        /// <summary>
        /// Creates an instance of the control and configures it.
        /// </summary>
        public abstract void InitializeControl();
        #endregion

        #region Class overrides
        /// <summary>
        /// Attaches events to the control.
        /// </summary>
        public virtual void AttachEvents()
        {
            if (this.CustomControl != null && !this.EventsAttached)
            {
                this.CustomControl.Click += new EventHandler(CustomControl_Click);
                this.CustomControl.DoubleClick += new EventHandler(CustomControl_DoubleClick);
                this.CustomControl.MouseEnter += new EventHandler(CustomControl_MouseEnter);
                this.CustomControl.MouseMove += new MouseEventHandler(CustomControl_MouseMove);
                this.CustomControl.MouseLeave += new EventHandler(CustomControl_MouseLeave);
                this.CustomControl.MouseDown += new MouseEventHandler(CustomControl_MouseDown);
                this.CustomControl.KeyDown += new KeyEventHandler(CustomControl_KeyDown);
                this.CustomControl.KeyUp += new KeyEventHandler(CustomControl_KeyUp);
                this.CustomControl.KeyPress += new KeyPressEventHandler(CustomControl_KeyPress);
                this.CustomControl.GotFocus += new EventHandler(CustomControl_GotFocus);
                this.CustomControl.Leave += new EventHandler(CustomControl_Leave);
                this.CustomControl.TabIndexChanged += new EventHandler(CustomControl_TabIndexChanged);
                this.CustomControl.VisibleChanged += new EventHandler(CustomControl_VisibleChanged);

                if (this.ParentElement != null)
                {
                    this.ParentElement.TabIndexChanged += new EventHandler(ParentElement_TabIndexChanged);
                    this.ParentElement.Paint += new ElementPaintEventHandler(ParentElement_Paint);
                    this.ParentElement.BeforeDisposing += new EventHandler(Parent_BeforeDisposing);
                }

                this.EventsAttached = true;
            }
        }

        /// <summary>
        /// Detaches events from the element.
        /// </summary>
        public virtual void DetachEvents()
        {
            if (this.CustomControl != null && this.EventsAttached)
            {
                this.CustomControl.Click -= new EventHandler(CustomControl_Click);
                this.CustomControl.DoubleClick -= new EventHandler(CustomControl_DoubleClick);
                this.CustomControl.MouseEnter -= new EventHandler(CustomControl_MouseEnter);
                this.CustomControl.MouseMove -= new MouseEventHandler(CustomControl_MouseMove);
                this.CustomControl.MouseLeave -= new EventHandler(CustomControl_MouseLeave);
                this.CustomControl.MouseDown -= new MouseEventHandler(CustomControl_MouseDown);
                this.CustomControl.KeyDown -= new KeyEventHandler(CustomControl_KeyDown);
                this.CustomControl.KeyUp -= new KeyEventHandler(CustomControl_KeyUp);
                this.CustomControl.KeyPress -= new KeyPressEventHandler(CustomControl_KeyPress);
                this.CustomControl.GotFocus -= new EventHandler(CustomControl_GotFocus);
                this.CustomControl.Leave -= new EventHandler(CustomControl_Leave);
                this.CustomControl.TabIndexChanged -= new EventHandler(CustomControl_TabIndexChanged);
                this.CustomControl.VisibleChanged -= new EventHandler(CustomControl_VisibleChanged);

                if (this.ParentElement != null && !this.ParentElement.IsDisposed)
                {
                    this.ParentElement.TabIndexChanged -= new EventHandler(ParentElement_TabIndexChanged);
                    this.ParentElement.Paint -= new ElementPaintEventHandler(ParentElement_Paint);
                    this.ParentElement.BeforeDisposing -= new EventHandler(Parent_BeforeDisposing);
                }

                this.EventsAttached = false;
            }
        }

        /// <summary>
        /// Raised when initialization has been finished.
        /// </summary>
        public virtual void OnInitEndCallback()
        {
            if (this.CustomControl != null)
            {
                this.DefaultSize = this.CustomControl.Size;
            }
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises corresponding event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        internal void CustomControl_Click(object sender, EventArgs e)
        {
            this.ParentElement.RaiseClickEvent(e);
        }

        /// <summary>
        /// Raises double click event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_DoubleClick(object sender, EventArgs e)
        {
            this.ParentElement.RaiseDoubleClickEvent(e);
        }

        /// <summary>
        /// Raises mouse enter event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_MouseEnter(object sender, EventArgs e)
        {
            this.ParentElement.RaiseMouseEnterEvent(e);
        }

        /// <summary>
        /// Raises mouse move event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_MouseMove(object sender, MouseEventArgs e)
        {
            this.ParentElement.RaiseMouseMoveEvent(e);
        }

        /// <summary>
        /// Raises mouse leave event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_MouseLeave(object sender, EventArgs e)
        {
            this.ParentElement.RaiseMouseLeaveEvent(e);
        }

        /// <summary>
        /// Raises mouse down event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.ParentElement.RaiseMouseDownEvent(e);
        }

        /// <summary>
        /// Raises key down event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_KeyDown(object sender, KeyEventArgs e)
        {
            this.ParentElement.RaiseKeyDownEvent(e);
        }

        /// <summary>
        /// Raises key up event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_KeyUp(object sender, KeyEventArgs e)
        {
            this.ParentElement.RaiseKeyUpEvent(e);
        }

        /// <summary>
        /// Raises key press event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.ParentElement.RaiseKeyPressEvent(e);
        }

        /// <summary>
        /// Raises got focus event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_GotFocus(object sender, EventArgs e)
        {
            this.QuiteMode = true;
            this.ParentElement.Focus();
            this.QuiteMode = false;
        }

        /// <summary>
        /// Raises leave event on parent tag element.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CustomControl_Leave(object sender, EventArgs e)
        {
            this.ParentElement.LeaveFocus();
        }

        /// <summary>
        /// Raised when tab index of the control has been changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void CustomControl_TabIndexChanged(object sender, EventArgs e)
        {
            m_bAssignTabIndex = false;

            this.ParentElement.TabIndex = this.CustomControl.TabIndex;
            this.ParentElement.RaiseTabIndexChangedEvent(e);

            m_bAssignTabIndex = true;
        }

        /// <summary>
        /// Raised when visibility of the control has been changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void CustomControl_VisibleChanged(object sender, EventArgs e)
        {
            SetTabOrder();
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Raised when TabIndex property of the parent element has been changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void ParentElement_TabIndexChanged(object sender, EventArgs e)
        {
            if (m_bAssignTabIndex && this.CustomControl != null && this.ParentElement != null)
            {
                this.ParentElement.QuietMode = true;
                int elmTabIndex = this.ParentElement.TabIndex;
                this.CustomControl.TabIndex = (elmTabIndex >= 0) ? elmTabIndex : 0;
                this.ParentElement.QuietMode = false;
            }
        }

        /// <summary>
        /// Raised when parent element is painting.
        /// This method is used for custom controls printing.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event data.</param>
        private void ParentElement_Paint(object sender, ElementPaintEventArgs e)
        {
            e.DrawDefault();

            // If printing - print control.
            if (e.PaintingBlock == this.ParentElement.MainBlock &&
              this.ParentElement.Document.IsPrinting)
            {
                //// NOTE: when we print control redrawing of HTMLUIControl control may be disabled.
                //// So we need to enable redrawing for proper printing this control.
                NativeMethods.SendMessage(Parent.Control.Handle, NativeMethods.WM_SETREDRAW, 1, 0);

                // Print control.
                using (Bitmap img = WinUtilities.PrintControl(this.CustomControl))
                {
                    ////img.MakeTransparent( this.CustomControl.BackColor );
                    img.SetResolution(DEF_PRINT_DPI, DEF_PRINT_DPI);
                    Rectangle bound = this.ParentElement.ReduceBounds(e.PaintingBlock.Rectangle);
                    bound = this.ParentElement.GlobalToClient(bound);
                    e.Graphics.DrawImageUnscaled(img, bound);
                }

                //// NOTE: We need to disable redrawing again.
                NativeMethods.SendMessage(Parent.Control.Handle, NativeMethods.WM_SETREDRAW, 0, 0);

                //// Add image to the regions during printing.
                if (this.ParentElement.Document.TextRegionManager != null)
                {
                    Rectangle boundRect = e.PaintingBlock.Rectangle;
                    TextRegion region = new TextRegion(boundRect.Y, boundRect.Height);
                    this.ParentElement.Document.TextRegionManager.Add(region);
                }
            }
        }

        /// <summary>
        /// Raised before the parent element is disposed.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Parent_BeforeDisposing(object sender, EventArgs e)
        {
            if (this.ParentElement != null && this.ParentElement.Document != null)
            {
                this.ParentElement.Document.UserControls.Remove(this);
                DisposeControl();
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets the tab order in the control.
        /// </summary>
        protected virtual void SetTabOrder()
        {
            // enable tab stop for control element by default.
            /*!this.ParentElement.Attributes.Contains( AttributeName.TabIndex )*/
            if (this.ParentElement.TabIndex == -1 && this.CustomControl.Visible)
            {
                this.ParentElement.TabIndex = 0;
            }
            else if (!this.CustomControl.Visible)
            {
                this.ParentElement.TabIndex = -1;
            }

            if (this.CustomControl.Visible)
            {
                int elmTabIndex = this.ParentElement.TabIndex;
                this.CustomControl.TabIndex = (elmTabIndex >= 0) ? elmTabIndex : 0;
            }

            this.CustomControl.TabStop = false;
        }

        /// <summary>
        /// Sets the control instance.
        /// </summary>
        /// <param name="control">Control object.</param>
        protected void SetControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;

            this.DefaultSize = m_control.Size;
        }
        #endregion
    }
}
