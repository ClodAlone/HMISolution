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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Base class of custom tags containing custom controls.
    /// </summary>
    public abstract class UserControlHolderBase
      : BaseElement, IUserControlHolder
    {
        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the UserControlHolderBase class
        /// </summary>
        /// <param name="parent">Parent element for this custom tag.</param>
        /// <param name="name">Name of the custom tag.</param>
        protected UserControlHolderBase(IHTMLElement parent, string name)
            : base(parent, name)
        {
        }
        #endregion

        #region IUserControlHolder Members

        /// <summary>
        /// Gets an instance of the custom control wrapper.
        /// </summary>
        public abstract IControlImpl UserControl { get; }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Type = ElementType.BlockSimple;
            this.Size = DefaultCalculateSizeInternal();

            SetSize();

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element.
        /// </summary>
        /// <param name="curPosition">Current position.</param>
        /// <param name="bounds">Bounds for the element.</param>
        /// <returns>Array of the blocks.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            // NOTE: Skip measuring of any internal content of the control.
            return new BlocksCollection();
        }

        /// <summary>
        /// Overridden. Indicates whether its control is active.
        /// </summary>
        /// <returns>True if the input focus request was successful; false otherwise.</returns>
        protected override bool FocusElementInternal()
        {
            bool result;

            if (this.UserControl != null && this.UserControl.CustomControl.CanFocus &&
              this.UserControl.CustomControl != null && this.TabIndex >= 0)
            {
                RaiseGotFocusEvent(EventArgs.Empty);

                result = this.UserControl.CustomControl.Focus();
                this.UserControl.CustomControl.Refresh();
            }
            else
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region Class helper methods

        /// <summary>
        /// Detaches events and clears blocks.
        /// </summary>
        protected void ClearBlocks()
        {
            if (m_blocks != null && m_blocks.Count > 0)
            {
                Block oldBlock = m_blocks[0] as Block;

                if (oldBlock != null)
                {
                    oldBlock.LocationChanged -= new EventHandler(Block_LocationChanged);
                }

                m_blocks.Clear();
            }

            m_curBlock = null;
        }

        /// <summary>
        /// Changes location of the control.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        protected void Block_LocationChanged(object sender, EventArgs e)
        {
            Block block = sender as Block;

            if (block != null && !this.Document.IsPrinting)
            {
                Point location = new Point(block.X, block.Y);
                location = this.ReduceLocation(location);
                location = this.Control.VirtualToClient(location);
                this.UserControl.CustomControl.Location = location;

                (this.UserControl as UserControlImpl).LocationSet = true;
            }
        }

        /// <summary>
        /// Overridden. Raises the LocationCalculated Event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected override void OnLocationCalculated(EventArgs args)
        {
            base.OnLocationCalculated(args);

            Block first = this.MainBlock;

            if (first != null)
            {
                Rectangle controlRect = this.ReduceBounds(first.Rectangle);
                controlRect.Location = this.Control.VirtualToClient(controlRect.Location);

                this.UserControl.CustomControl.Width = controlRect.Width;
                this.UserControl.CustomControl.Height = controlRect.Height;
            }
        }

        /// <summary>
        /// Calculates the size depending on settings.
        /// </summary>
        private void SetSize()
        {
            bool bSizeSet = this.IsStyleWidth || this.IsStyleHeight;
            bool bWidthNumberSet = this.IsStyleWidth && this.GetWidthType() == SizeTypeEx.Number;

            bool bHeightNumberSet = this.IsStyleHeight && this.GetHeightType() == SizeTypeEx.Number;

            if (bWidthNumberSet)
            {
                //// Width of element is defined by numbers.
                this.UserControl.CustomControl.Width = this.Width;
            }            
            else if (!bWidthNumberSet)
            {
                //// Width of element is set by percents or didn't set.
                int controlWidth = ExpandWidth(this.UserControl.DefaultSize.Width);
                this.Width = controlWidth;
            }
                       
            if (bHeightNumberSet)
            {
                //// Height of element is defined by numbers.
                this.UserControl.CustomControl.Height = this.Height;
            }           
            else if (!bHeightNumberSet)
            {
                //// Height of element is set by percents or didn't set.
                int controlHeight = ExpandHeight(this.UserControl.DefaultSize.Height);
                this.Height = controlHeight;
            }

            if (!bSizeSet)
            {
                // Size of element is not set. We set its size abowe and must change 
                // type of element.
                this.Type = ElementType.BlockFixedSize;
            }

           if ((this.Type & ElementType.BlockFixed) > 0)
            {
                //// If element is fixed - set min width and height for the element.
                this.MinWidth = Math.Max(this.Width, this.MinWidth);
                this.MinHeight = Math.Max(this.Height, this.MinHeight);
            }
        }

        /// <summary>
        /// Overridden. Draws an element.
        /// </summary>
        /// <param name="block">Block to be painted.</param>
        /// <param name="e">Paint arguments.</param>
        protected internal override void ProcessDrawBlock(Block block, PaintEventArgs e)
        {
            if (block.IsMain)
            {
                Point location = new Point(block.X, block.Y);
                location = this.ReduceLocation(location);
                location = this.Control.VirtualToClient(location);
                this.UserControl.CustomControl.Location = location;

                (this.UserControl as UserControlImpl).LocationSet = true;

                DrawBorders(block, e.Graphics);
            }
        }
        #endregion
    }
}
