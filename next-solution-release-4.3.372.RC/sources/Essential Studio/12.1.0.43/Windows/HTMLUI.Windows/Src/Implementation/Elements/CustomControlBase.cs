#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Implements base functionality of custom controls wrapper classes.
    /// </summary>
    public class CustomControlBase : ControlBase
    {
        #region Class members
        /// <summary>
        /// Indicates whether the location was set at first.
        /// </summary>
        private bool m_bLocationSet;
        #endregion

        #region Class properties

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the CustomControlBase class from being created
        /// </summary>
        private CustomControlBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomControlBase class
        /// </summary>
        /// <param name="parent">Parent custom tag element.</param>
        /// <param name="customControl">Custom control instance.</param>
        public CustomControlBase(IHTMLElement parent, Control customControl)
            : base(parent, customControl)
        {
            InitializeControl();
        }

        /// <summary>
        /// Finalizes an instance of the CustomControlBase class
        /// </summary>
        ~CustomControlBase()
        {
            ////DisposeControl();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Creates an instance of the control and configures it.
        /// </summary>
        public override void InitializeControl()
        {
            if (this.ParentElement != null)
            {
                this.ParentElement.Document.UserControls.Add(this);
                this.ParentElement.Type = ElementType.BlockSimple;
                this.ParentElement.FocusDefault = false;
                AttachEvents();

                SetTabOrder();
                OnInitEndCallback();
            }
        }

        /// <summary>
        /// Overridden. Sets the location of the custom controls.
        /// </summary>
        public override void SetLocation()
        {
            if (!m_bLocationSet && !this.ParentElement.Control.InDesignMode)
            {
                Point location = this.ParentElement.Location;

                if (this.ParentElement != null)
                {
                    location = this.ParentElement.ReduceLocation(location);
                    location = this.Parent.Control.VirtualToClient(location);
                }

                this.CustomControl.Location = location;
            }
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raised when the location of the parent element has been calculated.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Parent_LocationCalculated(object sender, EventArgs e)
        {
            if (this.ParentElement != null && this.ParentElement.Blocks.Count > 0)
            {
                Block first = this.ParentElement.MainBlock;

                if (first != null && !this.ParentElement.Document.IsPrinting)
                {
                    first.LocationChanged -= new EventHandler(Block_LocationChanged);
                    first.LocationChanged += new EventHandler(Block_LocationChanged);

                    //// set size of the control.
                    Rectangle controlRect = this.ParentElement.ReduceBounds(first.Rectangle);
                    controlRect.Location = this.ParentElement.Control.VirtualToClient(controlRect.Location);

                    //// this.CustomControl.Bounds = controlRect;
                    this.CustomControl.Size = controlRect.Size;
                }
            }
        }

        /// <summary>
        /// Raised when location of parent block changes.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Block_LocationChanged(object sender, EventArgs e)
        {
            Block block = sender as Block;

            if (block != null && this.ParentElement != null &&
              !this.ParentElement.Document.IsPrinting)
            {
                Point location = new Point(block.X, block.Y);
                location = this.ParentElement.ReduceLocation(location);
                location = this.ParentElement.Control.VirtualToClient(location);
                this.CustomControl.Location = location;
                m_bLocationSet = true;
            }
        }

        /// <summary>
        /// Raised after parent element size is calculated.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Parent_SizeCalculated(object sender, EventArgs e)
        {
            bool bSizeSet = this.ParentElement.IsStyleWidth || this.ParentElement.IsStyleHeight;
            bool bWidthNumberSet = this.ParentElement.IsStyleWidth && this.ParentElement.GetWidthType() == SizeTypeEx.Number;

            bool bHeightNumberSet = this.ParentElement.IsStyleHeight && this.ParentElement.GetHeightType() == SizeTypeEx.Number;

            if (this.ParentElement != null && bWidthNumberSet)
            {
                //// Width of element is defined by numbers.
                this.CustomControl.Width = this.ParentElement.Width;
            }
            else if (this.ParentElement != null && !bWidthNumberSet)
            {
                //// Width of element is set by percents or didn't set.
                int controlWidth = this.ParentElement.ExpandWidth(this.DefaultSize.Width);
                this.ParentElement.Width = controlWidth;
            }            
            if (this.ParentElement != null && bHeightNumberSet)
            {
                //// Height of element is defined by numbers.
                this.CustomControl.Height = this.ParentElement.Height;
            }          
            else if (this.ParentElement != null && !bHeightNumberSet)
            {
                //// Height of element is set by percents or didn't set.
                int controlHeight = this.ParentElement.ExpandHeight(this.DefaultSize.Height);
                this.ParentElement.Height = controlHeight;
            }

            if (this.ParentElement != null && !bSizeSet)
            {
                // Size of element is not set. We set its size abowe and must change 
                // type of element.
                this.ParentElement.Type = ElementType.BlockFixedSize;
            }
        }

        /// <summary>
        /// Raised after parent element has got focus.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Parent_GotFocus(object sender, EventArgs e)
        {
            if (!this.QuiteMode)
            {
                this.ParentElement.QuietMode = true;
                this.CustomControl.Focus();
                this.ParentElement.QuietMode = false;
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Attaches event handlers to the parent element.
        /// </summary>
        public override void AttachEvents()
        {
            base.AttachEvents();

            if (this.ParentElement != null)
            {
                this.ParentElement.LocationCalculated += new EventHandler(Parent_LocationCalculated);
                this.ParentElement.SizeCalculated += new EventHandler(Parent_SizeCalculated);
                this.ParentElement.GotFocus += new EventHandler(Parent_GotFocus);
            }
        }

        /// <summary>
        /// Detaches all events.
        /// </summary>
        public override void DetachEvents()
        {
            base.DetachEvents();

            if (this.ParentElement != null && !this.ParentElement.IsDisposed)
            {
                this.ParentElement.LocationCalculated -= new EventHandler(Parent_LocationCalculated);
                this.ParentElement.SizeCalculated -= new EventHandler(Parent_SizeCalculated);
                this.ParentElement.GotFocus -= new EventHandler(Parent_GotFocus);
            }
        }
        #endregion
    }
}
