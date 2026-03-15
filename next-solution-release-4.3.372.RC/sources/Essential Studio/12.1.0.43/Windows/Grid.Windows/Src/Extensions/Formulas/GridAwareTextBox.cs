//-------------------------------------------------------------------------------------------------
// <copyright file="GridAwareTextBox.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A derived text box that dynamically displays and edits the contents of the 
    /// CurrentCell of a grid. 
    /// </summary>
    /// <remarks>
    /// You can drop this text box anywhere on a form, and use its WireGrid method to 
    /// bind it to the CurrentCell of a grid. So, any changes in the text box
    /// are reflected in the CurrentCell of the grid, and any changes to the CurrentCell
    /// are reflected in the text box.
    /// </remarks>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(GridAwareTextBox), "ToolboxIcons.GridAwareTextBox.bmp")]
    public class GridAwareTextBox : TextBox
    {
        private bool ignoreTextChanged = false;
        GridControlBase activeGrid = null;
        private Color disabledColor = SystemColors.Window;
        private Color enabledColor = SystemColors.Window;

        /// <summary>
        /// Initializes a new <see cref="GridAwareTextBox"/>.
        /// </summary>
        public GridAwareTextBox()
            : base()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridAwareTextBox));
                CTRLSIZE = this.Size;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            base.CausesValidation = false;
            base.Text = string.Empty;
        }
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);

        #region For Touch

        bool isScaling = false;
        bool _touchMode = false;

        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual bool EnableTouchMode
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
                    {
                        ApplyScaleToControl(1.5F);
                    }
                    else
                    {
                        ApplyScaleToControl(1);
                    }

                }
            }
        }
        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * sf), (int)(CTRLSIZE.Height * sf));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion
        /// <summary>
        /// Gets or sets color of the text box background when the grid cell is not editable for some reason.
        /// </summary>
        /// <remarks>
        /// This color is used for the text box backcolor when the current grid cell is not editable,
        /// or if there is no current grid cell. You can set it to SystemColors.Control to see a 
        /// a standard gray disabled background.
        /// </remarks>
        [Category("Appearance")]
        [Description("Specifies the back color of the control when it is disabled.")]
        public Color DisabledBackColor
        {
            get { return disabledColor; }
            set { disabledColor = value; }
        }

        /// <summary>
        /// Gets or sets color of the background of the text box when the current grid cell is editable.
        /// </summary>
        [Category("Appearance")]
        [Description("Specifies the back color of the control when it is enabled.")]
        public Color EnabledBackColor
        {
            get { return enabledColor; }
            set { enabledColor = value; }
        }

        /// <summary>
        /// Hides the base.BackColor. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// Use to bind this text box to the CurrentCell of a GridControlBase.
        /// </summary>
        /// <param name="grid">The GridControlBase whose CurrentCell is being bound.</param>
        public virtual void WireGrid(GridControlBase grid)
        {
            if (grid != null)
            {
                grid.CurrentCellActivated += new EventHandler(GridCurrentCellActivated);
                grid.CurrentCellChanged += new EventHandler(GridCurrentCellChanged);
                grid.CurrentCellRejectedChanges += new System.EventHandler(GridCurrentCellRejectedChanges);
                GridCurrentCellActivated(grid, EventArgs.Empty); ////make sure activeGrid is set
            }
        }

        /// <summary>
        /// Removes the binding between this text box and the CurrentCell of the grid.
        /// </summary>
        /// <param name="grid">The GridControlBase whose CurrentCell is being bound.</param>
        public virtual void UnwireGrid(GridControlBase grid)
        {
            if (grid != null)
            {
                grid.CurrentCellActivated -= new EventHandler(GridCurrentCellActivated);
                grid.CurrentCellChanged -= new EventHandler(GridCurrentCellChanged);
                grid.CurrentCellRejectedChanges -= new System.EventHandler(GridCurrentCellRejectedChanges);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to cause validation. Force a false setting.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool CausesValidation
        {
            get { return false; }
            set { base.CausesValidation = false; }
        }

        /// <summary>
        /// Force an empty initial string.
        /// </summary>
        [Browsable(false), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellActivated"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// The GridCurrentCellActivated is called when the user enters into the grid cell.
        /// The default behavior is to set the text in the GridAwareTextBox to the current text that is stored in the
        /// cell's GridStyleInfo object. Override this method to modify this behavior or to add additional actions when
        /// the user enters the grid cell.
        /// </remarks>
        protected virtual void GridCurrentCellActivated(object sender, EventArgs e)
        {
            GridControlBase grid = sender as GridControlBase;
            if (grid != null)
            {
                activeGrid = grid;
                GridCurrentCell cc = grid.CurrentCell;
                if (cc.Renderer != null)
                {
                    SetText(cc.Renderer.ControlText);
                }

                this.BackColor = GetReadOnlyBackColor();
            }
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellChanged"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// The GridCurrentCellChanged is called when the user modifies the text in the grid cell.
        /// The default behavior is to set the text in the GridAwareTextBox to the ControlText from the CurrentCell renderer.
        /// Override this method to modify this behavior or to add additional actions when
        /// the user types the text within the grid cell.
        /// </remarks>
        protected virtual void GridCurrentCellChanged(object sender, System.EventArgs e)
        {
            GridControlBase grid = sender as GridControlBase;
            SetText(grid.CurrentCell.Renderer.ControlText);
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellRejectedChanges"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// The GridCurrentCellRejectedChanges is called when the user presses the escape while editing a cell in the grid.
        /// The default behavior is to reset the text in the GridAwareTextBox to the current text that is stored in the
        /// cell's GridStyleInfo object. Override this method to modify this behavior or to add additional actions when
        /// the user presses escape while editing within the grid cell.
        /// </remarks>
        protected virtual void GridCurrentCellRejectedChanges(object sender, EventArgs e)
        {
            GridControlBase grid = sender as GridControlBase;
            GridCurrentCell cc = grid.CurrentCell;
            SetText(grid.Model[cc.RowIndex, cc.ColIndex].Text);
        }

        string cText;
        /// <summary>
        /// Sets the Text property of the GridAwareTextBox without raising a TextChanged event.
        /// </summary>
        /// <param name="text">The text to be set.</param>
        protected void SetText(string text)
        {
            this.ignoreTextChanged = true;
            this.Text = text;
            if (this.Multiline)
            {
                GridCurrentCell cc = GetCurrentCell();
                cText = cc.Renderer.ControlText;
                cText = cText.Replace("\n", Environment.NewLine);
                cText = cText.Replace("\r\r", "\r");
                this.Text = cText;
                if (!cc.IsInMoveTo && !cc.IsEditing)
                    this.SelectionStart = this.Text.Length;
                this.ScrollToCaret();
            }
            this.ignoreTextChanged = false;
        }
        /// <summary>
        /// Returns the CurrentCell of the wired grid.
        /// </summary>
        /// <returns>The CurrentCell.</returns>
        protected GridCurrentCell GetCurrentCell()
        {
            GridCurrentCell cc = null;
            if (activeGrid == null)
            {
                return cc;
            }
            if (activeGrid.Parent is SplitterControl)
            {
                Control c = ((SplitterControl)activeGrid.Parent).ActivePane;
                if (c is GridControlBase)
                {
                    cc = ((GridControlBase)c).CurrentCell;
                }
            }

            if (cc == null)
            {
                if (activeGrid.Model.ActiveGridView != null)
                {
                    cc = activeGrid.Model.ActiveGridView.CurrentCell;
                }

                if (cc == null)
                {
                    cc = activeGrid.CurrentCell;
                }
            }

            return cc;
        }

        /// <override/>
        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);

            if (!this.ignoreTextChanged && activeGrid != null)
            {
                ////bool focused = this.Focused;
                GridCurrentCell cc = GetCurrentCell();
                if (cc != null)
                {
                    if (!cc.IsEditing)
                    {
                        // We are setting the current cell here into editing mode. The current cell's text box
                        // should not receive focus. 
                        cc.BeginEdit(false);
                    }

                    GridCellRendererBase renderer = cc.Renderer;
                    if (renderer != null)
                    {
                        TextBoxBase tb = renderer.Control as TextBoxBase;
                        if (renderer.ValidateString(this.Text))
                        {
                            renderer.ControlText = this.Text;
                            cc.IsModified = true;
                            if (tb != null)
                            {
                                tb.Select(this.SelectionStart, this.SelectionLength);
                            }
                        }
                        else
                        {
                            ignoreTextChanged = true;
                            this.Text = cc.Renderer.ControlText;
                            this.Select(this.selStart, this.selLength);
                            ignoreTextChanged = false;
                        }
                    }
                }
                ////if (focused && !this.Focused)
                ////    this.Focus();
            }
        }

        int selStart = 0;
        int selLength = -1;

        /// <summary>
        /// Overridden to make the grid process the Enter key.
        /// </summary>
        /// <param name="e">The KeyEventArgs</param>
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            ////handle the Enter key...
            if (e.KeyCode == Keys.Enter && activeGrid != null)
            {
                // The grid OnControlGotFocus will move the focus to the current cell
                // without changing the current cell's "modified" state. Then the grid
                // will process the Enter key and save any changes.
                if (e.Alt == false && e.Modifiers == Keys.None)
                {
                    GridCurrentCell cc = GetCurrentCell();
                    if (cc.ConfirmChanges())
                    {
                        cc.Grid.Focus();
                        //                    FindFormHelper.FindForm(cc.Grid).ActiveControl = cc.Grid;
                        SendKeys.Send("{ENTER}");
                    }
                    else if (!GridUtil.IsEmpty(cc.ErrorMessage))
                    {
                        cc.DisplayWarningText(cc.ErrorMessage);
                    }

                    return;
                }
            }

            ////handle the Esc key
            if (e.KeyCode == Keys.Escape && activeGrid != null)
            {
                GridCurrentCell cc = GetCurrentCell();
                cc.RejectChanges();
                cc.Refresh();
                this.SelectionStart = this.Text.Length;
                this.SelectionLength = 0;
                return;
            }

            base.OnKeyDown(e);
        }

        /// <override/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (Focused)
            {
                GridCurrentCell cc = GetCurrentCell();
                if (cc == null)
                {
                    return;
                }
                GridCellRendererBase renderer = cc.Renderer;
                if (renderer != null)
                {
                    TextBoxBase tb = renderer.Control as TextBoxBase;
                    if (tb != null)
                    {
                        tb.Select(this.SelectionStart, this.SelectionLength);
                    }
                }
            }
        }

        /// <override/>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (Focused)
            {
                GridCurrentCell cc = GetCurrentCell();
                if (cc == null)
                {
                    return;
                }
                GridCellRendererBase renderer = cc.Renderer;
                if (renderer != null)
                {
                    TextBoxBase tb = renderer.Control as TextBoxBase;
                    if (tb != null)
                    {
                        tb.Select(this.SelectionStart, this.SelectionLength);
                    }
                }
            }
        }

        /// <summary>
        /// Overridden to unwire the grid.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only
        /// unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        ////Overriden to make sure proper backcolor is used on first display.

        /// <override/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnVisibleChanged(System.EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                this.BackColor = GetReadOnlyBackColor();
            }
        }

        ////Used to get backcolor depending upon Read-only status.
        private Color GetReadOnlyBackColor()
        {
            GridControlBase grid = activeGrid;
            if (grid != null)
            {
                GridCurrentCell cc = grid.CurrentCell;
                if (cc.Renderer != null)
                {
                    GridStyleInfo style = grid.Model[cc.RowIndex, cc.ColIndex];
                    if (((style.ReadOnly || grid.Model.ReadOnly) && !grid.Model.IgnoreReadOnly)
                        || !(cc.Renderer.Control is TextBoxBase))
                    {
                        this.ReadOnly = true;
                    }
                    else
                    {
                        this.ReadOnly = false;
                    }
                }
                else
                {
                    this.ReadOnly = true;
                }
            }

            return this.ReadOnly ? this.DisabledBackColor : this.EnabledBackColor;
        }
    }
}
