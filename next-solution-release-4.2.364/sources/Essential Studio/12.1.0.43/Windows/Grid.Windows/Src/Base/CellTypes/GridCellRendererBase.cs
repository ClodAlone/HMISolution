//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellRendererBase.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This is a base class for the renderer part of a cell type.
    /// </summary>
    /// <remarks>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridCellModelBase"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// You typically access cell renderers through the <see cref="GridControlBase.CellRenderers"/>
    /// property of the <see cref="GridControlBase"/> class.<para/>
    /// </remarks>
    public class GridCellRendererBase
        : NonFinalizeDisposable,
        IQueryFocusInside, IGridDropDownCell, IPopupParent, IDisposable
    {
        // Fields
        GridControlBase grid;
        Control control = null;
        internal GridCellModelBase cellModel = null;
        internal int currentRowIndex = GridConstants.Undefined;         //// Row for current cell
        internal int currentColIndex = GridConstants.Undefined;         //// Column for current cell
        private bool initalizeCalled = false;
        private bool inInitialize = false;

        GridRangeInfo rangeCanceled = GridRangeInfo.Empty;
        GridCellHitTestInfo ht = new GridCellHitTestInfo();
        GridCellHitTestInfo savedHt = new GridCellHitTestInfo();
        ////bool initializedButtons = false;
        bool hasFocusControl = false;
        bool supportsEditing = true;
        bool supportsFocusControl = false;
        GridCellContextValue layout = new GridCellContextValue(null);

        string controlText = string.Empty;
        bool hasControlText = false;
        object controlValue = null;
        bool hasControlValue = false;
        internal bool ignoreWmChar = false;

        Point scrolledInfo;
        ////bool wantsDoubleClick = true;
        ArrayList buttons = null;

        /// <overload>
        /// Releases the all resources used by the component.
        /// </overload>
        /// <summary>
        /// Releases the all resources used by the component.
        /// </summary>
        public new void Dispose()
        {
            this.inDispose = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
            this.inDispose = false;
            this.isDisposed = true;
        }

        bool inDispose = false;
        bool isDisposed = false;

        /// <summary>
        /// Gets a value indicating whether object is executing <see cref="Dispose()"/> method call. Returns true if object is executing <see cref="Dispose()"/> method call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposing
        {
            get
            {
                return this.inDispose;
            }
        }

        /// <summary>
        /// Gets a value indicating whether object has been disposed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal IGridDropDownCellImp dropDownImp = null;

        /// <summary>
        /// Gets or sets a helper object for drop-down cell functionality.
        /// </summary>
        public IGridDropDownCellImp DropDownImp
        {
            get
            {
                return this.dropDownImp;
            }

            set
            {
                this.dropDownImp = value;
            }
        }

        /// <summary>
        /// Lets you customize and redirect mouse wheel behavior to a cell renderer, e.g. if you have
        /// a drop-down displayed. Returns true if parent grid should not be scrolled; False if parent grid should scroll.
        /// </summary>
        /// <param name="e">The mouse event arguments.</param>
        /// <returns>returns False.</returns>
        public virtual bool ProcessMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Returns state information that lets you restore the current editing state (e.g. caret position or other
        /// cell-type specific information) at a later point with <see cref="SetEditState"/>.
        /// </summary>
        /// <returns>A cell-type specific object with editing state information.</returns>
        /// <remarks>
        /// You need to override this method in a derived cell-type class. Otherwise calling this method
        /// will have no effect.
        /// </remarks>
        public virtual object GetEditState()
        {
            return null;
        }

        /// <summary>
        /// Restores previously retrieved editing state information from a <see cref="GetEditState"/> call.
        /// </summary>
        /// <param name="state">The cell-type specific object with editing state information.</param>
        /// <remarks>
        /// You need to override this method in a derived cell-type class. Otherwise calling this method
        /// will have no effect.
        /// </remarks>
        public virtual void SetEditState(object state)
        {
        }

        /// <summary>
        /// Initializes a new GridCellRendererBase object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridCellRendererBase(GridControlBase grid, GridCellModelBase cellModel)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(grid, cellModel);
            }
#else
               ;
#endif

            this.grid = grid;
            this.cellModel = cellModel;
            grid.ViewLayout.LayoutChanged += new EventHandler(this.GridViewLayoutChanged);
            this.WireModel(cellModel);
        }

        /// <override/>
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return GetType().Name; //// + " (" + this.cellModel.DisplayName + ")";
        }

        /// <summary>
        /// Gets or sets a reference to the GridCellModelBase that this cell renderer
        /// is associated with.
        /// </summary>
        public GridCellModelBase Model
        {
            get
            {
                return this.cellModel;
            }

            set
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                    ;
#endif

                if (this.cellModel != value)
                {
                    if (this.cellModel != null)
                    {
                        this.UnwireModel(cellModel);
                    }

                    this.cellModel = value;
                    if (this.cellModel != null)
                    {
                        this.WireModel(cellModel);
                    }

                    this.OnModelChanged(EventArgs.Empty);
                }
            }
        }

        internal bool RaiseDeleting()
        {
            return this.OnDeleting();
        }

        /// <summary>
        /// Occurs before the text box contents are deleted when user presses <Delete/> key in cell.
        /// </summary>
        /// <returns>True if cell contents can be cleared.</returns>
        protected virtual bool OnDeleting()
        {
            return true;
        }

        internal void IntUnwireModel(GridCellModelBase cellModel)
        {
            this.UnwireModel(cellModel);
        }

        /// <summary>
        /// Override this method if you are subscribing to events from a cell model. It is called before the reference to the cell model is reset or when dispose has been called.
        /// </summary>
        /// <param name="cellModel">The GridCellModelBase.</param>
        protected virtual void UnwireModel(GridCellModelBase cellModel)
        {
        }

        /// <summary>
        /// Override this method if you are subscribing to events from a cell model. It is called after the reference to the cell model has been initialized.
        /// </summary>
        /// <param name="cellModel">The GridCellModelBase.</param>
        protected virtual void WireModel(GridCellModelBase cellModel)
        {
        }

        /// <summary>
        /// Override this method if you need to know when reference to cell model is changed. This method is called after the reference to cell model has been changed.
        /// </summary>
        /// <param name="e">Always EventArgs.Empty.</param>
        protected virtual void OnModelChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Gets a reference to GridCurrentCell implementation
        /// of the GridControlBase this cell renderer is associated with.
        /// </summary>
        public GridCurrentCell CurrentCell
        {
            get
            {
                return this.Grid.CurrentCell;
            }
        }

        /// <summary>
        /// Unwires any events subscribed from GridControlBase and releases cell buttons.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="T:System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Model != null)
                {
                    this.UnwireModel(Model);
                }

                if (this.dropDownImp != null)
                {
                    this.dropDownImp.Dispose();
                }

                if (this.control != null)
                {
                    this.UnwireControl();
                }

                if (this.buttons != null)
                {
                    foreach (GridCellButton button in this.buttons)
                    {
                        button.Clicked -= new GridCellEventHandler(this.ButtonClicked);
                        button.Dispose();
                    }

                    this.buttons.Clear();
                    this.buttons = null;
                }

                this.grid.ViewLayout.LayoutChanged -= new EventHandler(GridViewLayoutChanged);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Draw inverted rectangle at the given rectangle.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The Rectangle Bounds.</param>
        /// <param name="r">Rectangle to invert.</param>
        void InvertRect(Graphics g, Rectangle bounds, Rectangle r)
        {
            if (r.IntersectsWith(bounds))
            {
                this.Grid.InvertRect(g, Rectangle.Intersect(bounds, r));
            }
        }

        /// <summary>
        /// Called from GridCurrentCell.Deactivate after GridCurrentCell.Deactivating event
        /// and before the current cell is deactivated.
        /// </summary>
        /// <returns>True if renderer can be deactivated; False if deactivation should be aborted.</returns>
        protected virtual bool OnDeactivating()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            return true;
        }

        internal bool RaiseDeactivating()
        {
            return this.OnDeactivating();
        }

        /// <summary>
        /// Called from GridCurrentCell.Deactivated after the current cell is deactivated
        /// and before the GridCurrentCell.Deactivated event.
        /// </summary>
        protected virtual void OnDeactived(int rowIndex, int colIndex)
        {
            this.CurrentCell.notifyChangingCalled = false;
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
               ;
#endif
        }

        internal void RaiseDeactived(int rowIndex, int colIndex)
        {
            this.OnDeactived(rowIndex, colIndex);
            this.Hide();
            this.hasControlText = false;
            this.hasControlValue = false;
            this.currentStyle = null;
        }

        internal bool inBeginEdit = false;

        /// <summary>
        /// Gets a value indicating whether <see cref="OnBeginEdit"/> was called.
        /// </summary>
        public bool InBeginEdit
        {
            get
            {
                return this.inBeginEdit;
            }
        }

        /// <summary>
        /// Called from GridCurrentCell.BeginEdit. Checks if cell renderer support
        /// in place editing. If in-place editing is supported, <see cref="SetHasFocusControl"/>
        /// is called which triggers <see cref="OnHasFocusControlChanged"/> and initiates repainting
        /// the cell. In your overriden <see cref="OnDraw"/> method, you should then check <see cref="ShouldDrawFocused"/>.
        /// </summary>
        protected virtual void OnBeginEdit()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (SupportsFocusControl
                && this.CurrentCell.FocusRendererOnBeginEdit
                && (this.Grid.HasControlFocus || CurrentCell.IsInActivate))
            {
                this.SetHasFocusControl(true);
            }

            this.inBeginEdit = true;
            this.Model.SetActiveText(RowIndex, ColIndex, ControlText);
            this.inBeginEdit = false;
        }

        internal void RaiseBeginEdit()
        {
            this.OnBeginEdit();
        }

        /// <summary>
        /// Called from GridCurrentCell.EndEdit. If render had focus, <see cref="SetHasFocusControl"/>
        /// is called which triggers <see cref="OnHasFocusControlChanged"/> and initiates repainting
        /// the cell. In your overriden <see cref="OnDraw"/> method, you should then check <see cref="ShouldDrawFocused"/>
        /// which will return False.
        /// </summary>
        protected virtual void OnEndEdit()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (this.HasFocusControl)
            {
                this.SetHasFocusControl(false);
            }

            this.inBeginEdit = true;
            this.Model.ResetActiveText(RowIndex, ColIndex);
            this.inBeginEdit = false;
            if (!rangeCanceled.IsEmpty)
            {
                this.Grid.InvalidateRange(rangeCanceled);
            }
        }

        internal void RaiseEndEdit()
        {
            this.OnEndEdit();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell renderer supports in-place editing with focus
        /// </summary>
        public bool SupportsFocusControl
        {
            get
            {
                return this.supportsFocusControl;
            }

            set
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                    ;
#endif

                this.supportsFocusControl = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell renderer supports being switched into edit mode. Default is true, only for GridStaticCellRenderer
        /// and GridHeaderCellRenderer it is false.
        /// </summary>
        public bool SupportsEditing
        {
            get
            {
                return this.supportsEditing;
            }

            set
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                    ;
#endif

                this.supportsEditing = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cell renderer has the focus (is in-place edit mode).
        /// </summary>
        public bool HasFocusControl
        {
            get
            {
                return !CurrentCell.StaticDrawing && hasFocusControl && supportsFocusControl;
            }
        }

        /// <summary>
        /// Determines if this control contains focus. Override this method if you
        /// want to show drop-down windows and indicate the control has not lost focus when
        /// the drop-down is shown.
        /// </summary>
        /// <returns>True if the control or any child control has focus; False otherwise.</returns>
        public virtual bool QueryFocusInside()
        {
            if (this.dropDownImp != null)
            {
                return this.dropDownImp.QueryFocusInside();
            }

            return false;
        }

        /// <summary>
        /// If the control supports in-place editing, set internal member and then fire
        /// OnHasFocusControlChanged.
        /// </summary>
        /// <param name="value">The boolean value</param>
        internal void SetHasFocusControl(bool value)
        {
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(value);
            }
#else
               ;
#endif

            value &= this.SupportsFocusControl;
            if (value != this.hasFocusControl)
            {
                ////SSTraceUtil.TraceCurrentMethodInfo(value);
                ////                if (!value)
                ////                {
                ////                    lastModified = CurrentCell.IsModified;
                ////                    Hide();
                ////                }
                ////                else
                ////                    CurrentCell.IsModified |= lastModified;
                this.hasFocusControl = value;
                this.currentColIndex = this.CurrentCell.ColIndex;
                this.currentRowIndex = this.CurrentCell.RowIndex;
                this.OnHasFocusControlChanged();
                bool scrolled = false;
                if (this.hasFocusControl)
                {
                    if (!Grid.ScrollGrid.m_bInDoScroll && !Grid.IsMousePressed)
                    {
                        ////scrolled = CurrentCell.ScrollInView();
                    }

                    Control c = this.Control;
                    if (c != null && !c.Visible)
                    {
                        ////grid.SuspendLayout();
                        c.Location = new Point(-1000, -1000);
                        ////c.Visible = true;
                        ////grid.ResumeLayout(false);
                    }
                }
                else
                {
                    this.Hide();
                }

                if (!scrolled)
                {
                    this.CurrentCell.Invalidate();
                }
            }
        }

        ////bool lastModified = false;

        /// <summary>
        /// Override this method if your cell renderer supports in-place editing and you want
        /// to do any custom initialization at this point before cell gets redrawn.
        /// </summary>
        /// <remarks>
        /// The default behavior of this virtual method is to force a redraw of the current cell.
        /// A cell can then initialize an edit control and set focus in the OnDraw method call.
        /// </remarks>
        protected virtual void OnHasFocusControlChanged()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.HasFocusControl);
            }
#else
               ;
#endif

            if (this.dropDownImp != null)
            {
                this.dropDownImp.OnHasFocusControlChanged();
            }
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>True if changes were saved successfully; False if no changes were saved.</returns>
        protected virtual bool OnSaveChanges()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ControlText);
            }
#else
               ;
#endif

            if (this.CurrentCell.IsModified)
            {
                // Save Control Value directly - do not call base (which ends up calling style.FormattedText = this.TextBoxText;)
                GridStyleInfo style = this.Grid.Model[this.RowIndex, this.ColIndex];
                if (!this.HasControlValue && this.HasControlText)
                {
                    string text = this.ControlText;
                    if (this.Grid.DisableFormattedTextInEditMode)
                    {
                        double result;
                        if (!string.IsNullOrEmpty(style.Format) && double.TryParse(text, out result))
                        {
                            text = string.Format("{0:" + style.Format + "}", result);
                        }
                    }
                    if (style.Format != null && text != null && text.EndsWith("%"))
                        text = this.lastValidateString;
                    style.ApplyFormattedText(text);
                }
                else
                {
                    style.CellValue = this.ControlValue;
                }

                return true;
            }

            return false;
        }

        internal bool RaiseSaveChanges()
        {
            return this.OnSaveChanges();
        }

        /// <summary>
        /// This is called from GridCurrentCell.RejectChanges. Any drop-downs have been closed at this time.
        /// </summary>
        protected virtual void OnRejectChanges()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ControlText);
            }
#else
               ;
#endif

            this.ResetControlText();
            this.ResetControlValue();
            this.inBeginEdit = true;
            this.Model.ResetActiveText(RowIndex, ColIndex);
            this.inBeginEdit = false;
        }

        internal void RaiseRejectChanges()
        {
            this.OnRejectChanges();
        }

        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        /// <returns>True if editing the cell is allowed; false otherwise.</returns>
        /// <remarks>
        /// The grid will switch into editing mode when the user presses a key while the cell
        /// is not in editing mode or when you call <see cref="GridCurrentCell.BeginEdit()"/>.
        /// You can cancel the operation by overriding this method and returning false.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events you receive when the current cell is moved.
        /// </remarks>
        protected virtual bool OnStartEditing()
        {
            return true;
        }

        internal bool RaiseStartEditing()
        {
            return this.OnStartEditing();
        }

        /// <summary>
        /// Occurs when the grid completes editing mode for the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> objects <see cref="GridCurrentCell.EndEdit"/>
        /// or <see cref="GridCurrentCell.CancelEdit"/> method is called. The event occurs after <see cref="GridControlBase.CurrentCellRejectedChanges"/>
        /// or <see cref="GridControlBase.CurrentCellAcceptedChanges"/> is raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        protected virtual void OnEditingComplete()
        {
        }

        internal void RaiseEditingComplete()
        {
            this.OnEditingComplete();
        }

        /// <summary>
        /// This method is called from GridCurrentCell.Validate after GridCurrentCell.Validating event has been
        /// fired. The default version checks if the active text fits any criteria as specified
        /// in the style object: It can be parsed into a cell value and meets GridCellValidateValueInfo criteria.
        /// </summary>
        /// <returns>True if the modified text is valid; False otherwise.</returns>
        /// <remarks>
        /// If you throw an exception in your override, the exception message will be stored in the
        /// GridCurrentCell.ErrorMessage string and if specified, a message box will be shown.<para/>
        /// OnValidate will call ValidateString, which you can also override.
        /// </remarks>
        protected virtual bool OnValidate()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            bool bValid = true;

            string sText = String.Empty;

            if (this.CurrentCell.IsEditing && this.CurrentCell.IsModified)
            {
                sText = this.ControlText;
                if (this.CurrentCell.Renderer != null && this.CurrentCell.Renderer is GridDropDownMonthCalendarCellRenderer && this.ControlValue != null)
                    sText = this.ControlValue.ToString();

                using (GridStyleInfo style = this.Grid.GetViewStyleInfo(currentRowIndex, currentColIndex, false))
                {
                    //// GetViewStyleInfo returns a style that can be modifies without saving changes back (this allows
                    //// a user to call ApplyText and check if an exception is thrown).
                    GridCellValidateValueInfo vna = style.ReadOnlyValidateValue;

                    if (vna != null)
                    {
                        double val = float.NaN;
                        double min = vna.Minimum;
                        double max = vna.Maximum;
                        bool isNumber = false;
                        if (sText.Length > 0)
                        {
                            if (!isNumber && style.Format!= null && (style.Format.ToLower().StartsWith("p") || style.Format.EndsWith("%")))
                            {
                                string s = sText;
                                if (s.Contains("%"))
                                {
                                    s = s.TrimEnd('%');
                                    isNumber = Double.TryParse(s, NumberStyles.Any, style.GetCulture(true).NumberFormat, out val);
                                    val *= 0.01;
                                }
                                else
                                    Double.TryParse(s, NumberStyles.Any, style.GetCulture(true).NumberFormat, out val);
                                sText = val.ToString();
                            }
                            isNumber = Double.TryParse(sText, NumberStyles.Any, style.GetCulture(true).NumberFormat, out val);
                        }

                        if (min.Equals(GridCellValidateValueInfo.Default.Minimum))
                            min *= 10.0;
                        if (max.Equals(GridCellValidateValueInfo.Default.Maximum))
                            max *= 10.0;

                        if (vna.NumberRequired && !isNumber)
                        {
                            this.CurrentCell.ErrorMessage = sText + " is not a valid number.";
                            return false;
                        }

                        if (!GridUtil.IsNotValidNumber(min) && !GridUtil.IsNotValidNumber(val) && val < min)
                        {
                            bValid = false;
                        }

                        if (bValid && !GridUtil.IsNotValidNumber(max) && !GridUtil.IsNotValidNumber(val) && val > max)
                        {
                            bValid = false;
                        }

                        if (!bValid)
                        {
                            this.CurrentCell.ErrorMessage = vna.ErrorMessage;
                            return false;
                        }
                        if (this.CurrentCell.IsError)
                        {
                            return false;
                        }

                    }

                    ExceptionManager.SuspendCatchExceptions();
                    try
                    {
                        //// GetViewStyleInfo returned a style that can be modifies without saving changes back (this allows
                        //// a user to call ApplyFormattedText and check if a exception is thrown).
                        style.ApplyFormattedText(sText);
                        ExceptionManager.ResumeCatchExceptions();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.ResumeCatchExceptions();
                        TraceUtil.TraceExceptionCatched(ex);
                        Type type = style.CellValueType;
                        string msg = string.Empty;
                        if (type != null)
                        {
                            if (!this.CurrentCell.IsError)
                            {
                                if (this.CurrentCell.ValidationErrorText == string.Empty)
                                    msg = String.Format("{0} is not a valid value for {1}.", sText, type.Name);
                                else
                                    msg = String.Format("{0}", this.CurrentCell.ValidationErrorText);
                            }
                            else
                            {
                                msg = this.CurrentCell.ErrorMessage;
                            }
                        }

                        this.CurrentCell.Exception = ex;

                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            if (type != null)
                            {
                                throw new FormatException(msg, ex);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        if (msg != string.Empty)
                        {
                            this.CurrentCell.ErrorMessage = msg;
                        }

                        return false;
                    }

                    if (!ValidateString(sText))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal bool RaiseValidate()
        {
            return this.OnValidate();
        }

        /// <summary>
        /// This method is called from GridCurrentCell.Validate after the current cell contents were
        /// successfully validated and before GridCurrentCell.Validated event is.
        /// fired.
        /// </summary>
        protected virtual void OnValidated()
        {
        }

        internal void RaiseValidated()
        {
            this.OnValidated();
        }

        /// <summary>
        /// This is called from GridCurrentCell.Activate after the activating event has been raised
        /// and allows interception of cell activation.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True is cell can be activated; False otherwise.</returns>
        /// <remarks>
        /// When the current cell is moved from one position to another, Essential Grid will
        /// first deactivate the current cell and afterwards activate the current cell at the
        /// new position. At the time activate is called, the old current cell is deactivated
        /// and Essential Grid has no active current cell.</remarks>
        protected virtual bool OnActivating(int rowIndex, int colIndex)
        {
            ////lastModified = false;
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            return true;
        }

        internal bool RaiseActivating(int rowIndex, int colIndex)
        {
            return this.OnActivating(rowIndex, colIndex);
        }

        /// <summary>
        /// This is called after the activating event and notifies that at this time
        /// the current cell has now become the active current cell.
        /// </summary>
        protected virtual void OnActivated()
        {
            ////lastModified = false;
            this.CurrentCell.notifyChangingCalled = false;
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
        }

        internal void RaiseActivated()
        {
            this.OnActivated();
        }

        /// <summary>
        /// Gets a value indicating whether BeginEdit has been called.
        /// </summary>
        protected bool IsEditing
        {
            get
            {
                return this.CurrentCell.IsEditing;
            }
        }

        /// <summary>
        /// Determines if the cell at the specified row and column should be drawn in "editing" mode.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if the cell should be drawn in "editing" mode; False otherwise.</returns>
        public bool ShouldDrawEditing(int rowIndex, int colIndex)
        {
            return this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && this.CurrentCell.IsEditing
                && !Grid.IsPrinting()
                && !CurrentCell.InternalHide;
        }

        /// <summary>
        /// Determines if the cell at the specified row and column should be drawn "focused".
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if the cell should be drawn "focused".</returns>
        public bool ShouldDrawFocused(int rowIndex, int colIndex)
        {
            return this.ShouldDrawEditing(rowIndex, colIndex) && this.HasFocusControl;
        }

        /// <summary>
        /// Raises the GridCurrentCell.CellChanging event indicating the contents of the
        /// current cell are about to be changed by the user.
        /// </summary>
        /// <returns>True if cell contents can be modified; False if cell contents should not be changed.</returns>
        /// <remarks>Call this method from your derived renderer if you support modifying the cell
        /// contents the first time before the user edits the cell.
        /// </remarks>
        protected virtual bool NotifyCurrentCellChanging()
        {
            if (!Grid.Model.IgnoreReadOnly && CurrentStyle.ReadOnly)
            {
                return false;
            }

            //// QA issue 23 fix
            if (this.CurrentCell.IsModified || this.CurrentCell.notifyChangingCalled)
            {
                return true;
            }
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.CurrentCell.IsChanging = true;
            bool success = this.CurrentCell.NotifyChanging();
            this.CurrentCell.IsChanging = false;
            return success;
        }

        internal bool RaiseNotifyCurrentCellChanging()
        {
            return this.NotifyCurrentCellChanging();
        }

        /// <summary>
        /// Raises the GridCurrentCell.CellChanged event indicating the contents of the
        /// current cell have been changed (e.g. in response to a TextBox.Changed event).
        /// </summary>
        /// <remarks>If you have implemented a custom cell type you should call this method
        /// from your derived renderer if you support modifying the cell
        /// contents.</remarks>
        protected virtual void NotifyCurrentCellChanged()
        {
            this.Grid.RaiseNotifyCurrentCellChanged();
        }

        internal void RaiseNotifyCurrentCellChanged()
        {
            this.NotifyCurrentCellChanged();
        }

        /// <summary>
        /// Raises the GridCurrentCell.ControlDoubleClick event indicating the cell has
        /// in-place editing mode and the user double-clicked inside the control
        /// associated with the current cell.
        /// </summary>
        /// <param name="control">A reference to the control associated with the current cell.</param>
        /// <remarks>
        /// GridCurrentCell.ControlDoubleClick lets you detect a double click inside
        /// a cell for any CurrentCellActivateBehavior. If for example the focus is set
        /// to the renderers control after the first click, the grid will listen for a
        /// MouseDown on the newly focused control and raise this event on a second click.
        /// </remarks>
        protected void NotifyCurrentCellControlDoubleClick(Control control)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.OnControlDoubleClick(control);
        }

        /// <summary>
        /// Handles the DoubleClick event of the embedded control and raises the
        /// <see cref="GridControlBase.CurrentCellControlDoubleClick"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ControlDoubleClick(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.OnControlDoubleClick(this.Control);
        }

        /// <summary>
        /// Occurs when the current cell
        /// has in-place editing mode and the user double-clicked inside the control
        /// associated with the current cell.
        /// </summary>
        /// <remarks>
        /// GridCurrentCell.ControlDoubleClick lets you detect a double click inside
        /// a cell for any CurrentCellActivateBehavior. If for example the focus is set
        /// to the renderers control after the first click, the grid will listen for a
        /// MouseDown on the newly focused control and raise this event on a second click.
        /// <para/>
        /// Raising this event is only optional for the cell renderer that manages the active cell.<para/>
        /// <para/>
        /// A text box will usually send this event when the associated <see cref="TextBox"/> control has received
        /// the focus after the cell was switched into edit mode and the user double-clicked. Other cell renderers
        /// may or may not send this event.
        /// </remarks>
        protected virtual void OnControlDoubleClick(Control control)
        {
            this.CurrentCell.NotifyControlDoubleClick(control);
        }

        /// <summary>
        /// Raises the GridCurrentCell.ControlGotFocus event indicating the cell has
        /// switched to in-place editing and the control associated with the current cell
        /// has received the focus.
        /// </summary>
        /// <param name="control">A reference to the control associated with the current cell.</param>
        /// <remarks>No derived renderer currently calls this method.</remarks>
        protected void NotifyCurrentCellControlGotFocus(Control control)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.CurrentCell.NotifyControlGotFocus(control);
        }

        /// <summary>
        /// Raises the GridCurrentCell.ControlLostFocus event indicating the cell has
        /// switched to in-place editing and the control associated with the current cell
        /// has lost the focus.
        /// </summary>
        /// <param name="control">Control associated with the current cell.</param>
        /// <remarks>No derived renderer currently calls this method.</remarks>
        protected void NotifyCurrentCellControlLostFocus(Control control)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.CurrentCell.NotifyControlLostFocus(control);
        }

        /// <summary>
        /// Gets a value indicating whether the current cell is in a dropped-down state.
        /// </summary>
        protected virtual bool IsDroppedDown
        {
            get
            {
                return this.dropDownImp != null && this.dropDownImp.IsDroppedDown;
            }
        }

        internal bool GetDroppedDown()
        {
            return this.IsDroppedDown;
        }

        /// <summary>
        /// This is called from GridCurrentCell.ShowDropDown after BeginEdit has been called.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be made
        /// visible at this time.
        /// </remarks>
        protected virtual void OnShowDropDown()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.OnShowDropDown();
        }

        internal void RaiseShowDropDown()
        {
            this.OnShowDropDown();
        }

        /// <summary>
        /// This is called from GridCurrentCell.CloseDropDown.
        /// </summary>
        /// <remarks>
        /// If your renderer supports dropped-down state, the drop-down window should be made
        /// closed at this time.
        /// </remarks>
        protected virtual void OnCloseDropDown(PopupCloseType reason)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.OnCloseDropDown(reason);
        }

        internal void RaiseCloseDropDown(PopupCloseType reason)
        {
            this.OnCloseDropDown(reason);
        }

        /// <summary>
        /// Allows custom formatting of a cell by changing its style object.
        /// </summary>
        /// <param name="e">Event data</param>
        /// <remarks>
        /// <see cref="OnPrepareViewStyleInfo"/> is called from <see cref="GridControlBase.PrepareViewStyleInfo"/>
        /// in order to allow custom formatting of
        /// a cell by changing its style object.
        /// <para/>
        /// Set the cancel property true if you want to avoid
        /// the associated cell renderers object <see cref="GridCellRendererBase.OnPrepareViewStyleInfo"/>
        /// method to be called.<para/>
        /// Changes made to the style object will not be saved in the grid nor cached. This event
        /// is called every time a portion of the grid is repainted and the specified cell belongs
        /// to the invalidated region of the window that needs to be redrawn.<para/>
        /// Changes to the style object done at this time will also not be reflected when accessing
        /// cells though the models indexer. See <see cref="GridModel.QueryCellInfo"/>.<para/>
        /// <note type="note">Do not change base style or cell type at this time.</note>
        /// </remarks>
        /// <seealso cref="GridPrepareViewStyleInfoEventHandler"/>
        /// <seealso cref="GridControlBase.GetViewStyleInfo(int,int)"/>
        public virtual void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
        }

        /// <summary>
        /// Highlights the current cell by inverting the cells border or drawing a thick border.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="r">Specifies the cell rectangle.</param>
        /// <remarks>
        /// Override this method if you don't want the default highlighting
        /// of your cell.
        /// </remarks>
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(r);
            }
#else
               ;
#endif

            ////            if (!Grid.Model.Options.ExcelLikeCurrentCell && Grid.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SetCurrent)
            ////                return;
            Rectangle bounds = this.GetCellBoundsCore(this.currentRowIndex, currentColIndex);
            bounds.Intersect(Rectangle.Ceiling(g.ClipBounds));

            if (this.Grid.Model.Options.ExcelLikeCurrentCell)
            {
                r = this.grid.RangeInfoToRectangle(GridRangeInfo.Cell(currentRowIndex, currentColIndex), GridRangeOptions.MergeCoveredCells | GridRangeOptions.CalculateNonClientArea);
                this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Top + 2));
                this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left, r.Top + 2, r.Left + 2, r.Bottom));
                this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Right - 3, r.Top + 2, r.Right, r.Bottom - 3));
                this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left + 2, r.Bottom - 3, r.Right, r.Bottom));
                return;
            }

            GridShowCurrentCellBorder showBorder = this.Grid.Model.Options.ShowCurrentCellBorderBehavior;

            if (showBorder == GridShowCurrentCellBorder.HideAlways)
            {
                return;
            }

            bool gridFocused = this.Grid.HasControlFocus && !Grid.CurrentCell.StaticDrawing;
            bool drawGreyed = !(gridFocused || showBorder == GridShowCurrentCellBorder.AlwaysVisible);

            GridDrawCurrentCellBorderEventArgs e = new GridDrawCurrentCellBorderEventArgs(
                this.currentRowIndex,
                this.currentColIndex,
                g,
                r,
                gridFocused,
                StyleInfo,
                showBorder);

            // Give programmer chance to do own drawing (should set e.Cancel = true then).
            this.grid.RaiseDrawCurrentCellBorder(e);

            if (!e.Cancel)
            {
                if (!drawGreyed)
                {
                    drawGreyed = false;
                    GridProperties pProp = this.Grid.Model.Properties;

                    int nThick = 1;

                    //// left border
                    this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left, r.Top, r.Left + nThick, r.Bottom));
                    //// right border
                    this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Right - nThick, r.Top, r.Right, r.Bottom));
                    //// top border
                    this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left + nThick, r.Top, r.Right - nThick, r.Top + nThick));
                    //// bottom border
                    this.InvertRect(g, bounds, Rectangle.FromLTRB(r.Left + nThick, r.Bottom - nThick, r.Right - nThick, r.Bottom));
                }
                else
                {
                    //// left border
                    GridBorderPaint.DrawRectangle(g, new GridBorder(GridBorderStyle.Dashed, Color.Gray, GridBorderWeight.Thin), r, this.StyleInfo.Interior.BackColor, GridBorderSide.All);
                }
            }
        }

        internal void RaiseOutlineCurrentCell(Graphics g, Rectangle r)
        {
            this.OnOutlineCurrentCell(g, r);
        }

        /// <summary>
        /// Indicates that cell window coordinates in the grid view have changed. Cached coordinates need
        /// to be recalculated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event args.</param>
        private void GridViewLayoutChanged(object sender, EventArgs e)
        {
            this.ResetCellLayout();

            this.OnCellLayoutChanged();
        }

        /// <summary>
        /// Called when the ViewLayout of the parent grid is changed, for example if grid is scrolled or row height is changed.
        /// </summary>
        protected virtual void OnCellLayoutChanged()
        {
        }

        /// <summary>
        /// Called when the parent grid of this cell renderer gets the focus.
        /// </summary>
        /// <param name="e">The EventArgs.Empty</param>
        protected virtual void OnGridGotFocus(EventArgs e)
        {
        }

        internal void RaiseGridGotFocus(EventArgs e)
        {
            this.OnGridGotFocus(e);
        }

        /// <summary>
        /// Reset window coordinates (cell bounds) of the cell.
        /// </summary>
        public void ResetCellLayout()
        {
            this.layout = new GridCellContextValue(null);
        }

        /// <overload>
        /// Returns a rectangle with cell bounds for the specified cell. For covered cell,
        /// the total of all covered cells will be returned.
        /// </overload>
        /// <summary>
        /// Returns a rectangle with cell bounds for the specified cell. For covered cell,
        /// the total of all covered cells will be returned.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the bounds.</returns>
        public Rectangle GetCellBoundsCore(int rowIndex, int colIndex)
        {
            return this.GetCellBoundsCore(rowIndex, colIndex, false);
        }

        /// <summary>
        /// Returns a rectangle with cell bounds for the specified cell. For covered cell,
        /// the total of all covered cells will be returned.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="includeFloated">Specifies if the range covered by a floating cell should be returned</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> with the bounds.</returns>
        public Rectangle GetCellBoundsCore(int rowIndex, int colIndex, bool includeFloated)
        {
            GridRangeOptions ro = GridRangeOptions.MergeCoveredCells | GridRangeOptions.MergeMergedCells | GridRangeOptions.CalculateNonClientArea;
            if (includeFloated)
            {
                ro |= GridRangeOptions.MergeFloatedCells;
            }

            return this.Grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), ro);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="nonClientIfVisible">if set to <c>true</c> [non client if visible].</param>
        /// <returns>returns Rectangle</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Rectangle GetCellBoundsCoreInt(int rowIndex, int colIndex, bool nonClientIfVisible)
        {
            if (rowIndex <= this.Grid.ViewLayout.LastVisibleRow && colIndex <= this.Grid.ViewLayout.LastVisibleCol)
            {
                GridRangeInfo rgCell = this.Grid.ViewLayout.CombineSpannedRanges(GridRangeInfo.Cell(rowIndex, colIndex));
                if (this.Grid.ViewLayout.IsRangeVisible(rgCell))
                {
                    return this.grid.RangeInfoToRectangle(rgCell, nonClientIfVisible ? GridRangeOptions.CalculateNonClientArea : GridRangeOptions.None);
                }
            }

            return Rectangle.Empty;
        }

        /// <summary>
        /// Returns the <see cref="GridCellLayout"/> for the specified cell using cell state information as specified in the style object
        /// and caches the layout information.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="GridCellLayout"/> with layout information.</returns>
        public GridCellLayout GetCellLayout(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridCellLayout cellLayout = this.layout.GetValue(rowIndex, colIndex) as GridCellLayout;
            if (cellLayout == null)
            {
                Rectangle cellRectangle = this.GetCellBoundsCore(rowIndex, colIndex);
                cellLayout = this.PerformLayout(rowIndex, colIndex, style, cellRectangle);
                this.layout.SetValue(rowIndex, colIndex, cellLayout);
            }

            return cellLayout;
        }

        /// <summary>
        /// Get the cell client rectangle taking floated cells into consideration.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="floatedCells">Specifies if the range covered by a floating cell should be returned.</param>
        /// <returns>The <see cref="System.Drawing.Rectangle"/> for the client area.</returns>
        public Rectangle GetCellClientRectangle(int rowIndex, int colIndex, GridStyleInfo style, bool floatedCells)
        {
            if (!floatedCells)
            {
                return this.GetCellLayout(rowIndex, colIndex, style).ClientRectangle;
            }

            //// Get the client bounds taking floated cells into consideration.
            Rectangle cellRectangle = this.GetCellBoundsCore(rowIndex, colIndex, true);
            GridCellLayout cellLayout = this.PerformLayout(rowIndex, colIndex, style, cellRectangle);
            return cellLayout.ClientRectangle;
        }

        /// <overload>
        /// Calculates the CellLayout for a specified cell.
        /// </overload>
        /// <summary>
        /// Calculates the CellLayout for the specified cell using cell state information as specified in the style object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="GridCellLayout"/> with layout information.</returns>
        public GridCellLayout PerformLayout(int rowIndex, int colIndex)
        {
            Rectangle cellRectangle = this.GetCellBoundsCore(rowIndex, colIndex);
            return this.PerformLayout(rowIndex, colIndex, Grid.Model[rowIndex, colIndex], cellRectangle);
        }

        /// <summary>
        /// Calculates the CellLayout for the specified cell using cell state information as specified in the style object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The <see cref="GridCellLayout"/> with layout information.</returns>
        public GridCellLayout PerformLayout(int rowIndex, int colIndex, GridStyleInfo style)
        {
            Rectangle cellRectangle = this.GetCellBoundsCore(rowIndex, colIndex);
            return this.PerformLayout(rowIndex, colIndex, style, cellRectangle);
        }

        /// <summary>
        /// Calculates the CellLayout for the specified cell using cell state information as specified in the style object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="cellRectangle">The Rectangle.</param>
        /// <returns>The <see cref="GridCellLayout"/> with layout information.</returns>
        public GridCellLayout PerformLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle cellRectangle)
        {
            GridCellLayout layout = new GridCellLayout();
            layout.CellRectangle = cellRectangle;
            GridMargins margins = this.Grid.Model.StyleInfoBordersToMargins(style);
            if (this.Grid.IsRightToLeft())
            {
                margins = margins.SwapRightToLeft();
            }

            layout.InnerRectangle = GridMargins.RemoveMargins(cellRectangle, margins);

            int count = this.Buttons.Count;
            Rectangle[] buttonsBounds = new Rectangle[count];
            layout.ClientRectangle = this.OnLayout(rowIndex, colIndex, style, layout.InnerRectangle, buttonsBounds);
            for (int n = 0; n < count; n++)
            {
                this.GetButton(n).Bounds = buttonsBounds[n];
            }

            layout.Buttons = buttonsBounds;
            layout.TextRectangle = this.RemoveMargins(layout.ClientRectangle, style);
            return layout;
        }

        /// <summary>
        /// Removes TextMargins from the specified client bounds.
        /// </summary>
        /// <param name="clientRectangle">The Rectangle</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>A <see cref="System.Drawing.Rectangle"/> with the bounds.</returns>
        protected virtual Rectangle RemoveMargins(Rectangle clientRectangle, GridStyleInfo style)
        {
            GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
            if (this.Grid.IsRightToLeft())
            {
                margins = margins.SwapRightToLeft();
            }

            return GridMargins.RemoveMargins(clientRectangle, margins);
        }

        /// <summary>
        /// This method is called from PerformLayout to calculate the client rectangle given
        /// the inner rectangle of a cell and any boundaries of cell buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="innerBounds">The <see cref="System.Drawing.Rectangle"/> with the inner bounds of a cell.</param>
        /// <param name="buttonsBounds">An array of <see cref="System.Drawing.Rectangle"/> with bounds for each cell button element.</param>
        /// <returns>A <see cref="System.Drawing.Rectangle"/> with the bounds.</returns>
        protected virtual Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle clientRectangle = innerBounds;
            int count = this.Buttons.Count;
            if (!Grid.IsPrinting() && count > 0 && this.OnQueryShowButtons(rowIndex, colIndex, style))
            {
                innerBounds.Inflate(-1, -1);
                int nButtonBarWidth = this.Model.ButtonBarSize.Width;
                int width = nButtonBarWidth / count;
                clientRectangle.Width -= nButtonBarWidth;

                int height = Math.Min(this.Model.ButtonBarSize.Height, innerBounds.Height);
                if (height == 0)
                {
                    height = innerBounds.Height;
                }

                GridTextAlign textAlign = style.TextAlign;
                GridVerticalAlignment verticalAlign = style.VerticalAlignment;

                int yOffset = innerBounds.Top;
                if (verticalAlign == GridVerticalAlignment.Middle)
                {
                    yOffset += (innerBounds.Height - height) / 2;
                }
                else if (verticalAlign == GridVerticalAlignment.Bottom)
                {
                    yOffset += innerBounds.Height - height;
                }

                int xOffset = innerBounds.Left;
                if ((style.TextAlign == GridTextAlign.Right) != this.Grid.IsRightToLeft())
                {
                    clientRectangle.Offset(nButtonBarWidth, 0);
                }
                else
                {
                    xOffset += innerBounds.Width - nButtonBarWidth;
                }

                for (int n = 0; n < count; n++)
                {
                    Rectangle bounds = new Rectangle(xOffset, yOffset, width, height);
                    buttonsBounds[n] = bounds;
                    xOffset += width;
                }
            }

            return clientRectangle;
        }

        bool shouldDrawBorders = true;

        /// <summary>
        /// Draw the contents of the specified cell including cell background and optionally also the borders. Call
        /// this method if you want to draw a stand-alone cell and you want to ensure that also the background
        /// of the cell and also the borders are painted.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="cellRectangle">Specifies the cell rectangle.</param>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style of the cell.</param>
        /// <param name="includeBorders">Specifies whether borders should also be drawn or excluded.</param>
        /// <remarks>
        /// The method calls DrawBackground to draw
        /// the background as specified in the style object.<para/>
        /// If includeBorders is false the area covered by borders is excluded when the rectangle
        /// is passed to the DrawBackground method.
        /// </remarks>
        public void DrawSingleCell(Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style, bool includeBorders)
        {
            bool b = this.grid.m_bForceDrawBackground;
            this.grid.m_bForceDrawBackground = true;
            this.shouldDrawBorders = includeBorders;
            this.Draw(g, cellRectangle, rowIndex, colIndex, style);
            this.shouldDrawBorders = true;
            this.grid.m_bForceDrawBackground = b;

            if (includeBorders)
            {
                Rectangle r = cellRectangle;
                if (!style.Borders.Top.IsEmpty)
                {
                    GridBorderPaint.DrawRectangle(g, style.Borders.Top, r, style.BackColor, GridBorderSide.Top, this.Grid.PrintingMode);
                }

                if (!style.Borders.Left.IsEmpty)
                {
                    GridBorderPaint.DrawRectangle(g, style.Borders.Left, r, style.BackColor, GridBorderSide.Left, this.Grid.PrintingMode);
                }

                if (style.Borders.Bottom.Style == GridBorderStyle.Standard)
                {
                    GridBorder border = this.grid.Model.GetGridLineBorder();
                    GridBorderPaint.DrawRectangle(g, border, r, style.BackColor, GridBorderSide.Bottom, this.Grid.PrintingMode);
                }
                else if (!style.Borders.Bottom.IsEmpty)
                {
                    GridBorderPaint.DrawRectangle(g, style.Borders.Bottom, r, style.BackColor, GridBorderSide.Bottom, this.Grid.PrintingMode);
                }

                if (style.Borders.Right.Style == GridBorderStyle.Standard)
                {
                    GridBorder border = this.grid.Model.GetGridLineBorder();
                    GridBorderPaint.DrawRectangle(g, border, r, style.BackColor, GridBorderSide.Right, this.Grid.PrintingMode);
                }
                else if (!style.Borders.Right.IsEmpty)
                {
                    GridBorderPaint.DrawRectangle(g, style.Borders.Right, r, style.BackColor, GridBorderSide.Right, this.Grid.PrintingMode);
                }
            }
        }

        /// <summary>
        /// Draw the contents of the specified cell.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="cellRectangle">Specifies the cell rectangle.</param>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style of the cell.</param>
        /// <remarks>
        /// When drawing cells, it is necessary to determine if the
        /// specified cell (with rowIndex and colIndex) is the current cell and if
        /// it is active.<para/>
        /// If your control is associated with a control and supports in-place editing
        /// and the specified cell is the current cell, you should position the control. Take a look at the GridTextBoxCellRenderer
        /// implementation of this method for an example.<para/>
        /// Otherwise, you can simply draw the cell. <para/>
        /// Please note that you should first call DrawBackground to draw
        /// the background and borders as specified in the style object.<para/>
        /// The base-class version of this method first calls PerformLayout and then the
        /// cell renderers OnDraw method. After the inner cell has been drawn, it will draw
        /// each cell button for the cell.
        /// <note type="note">Overriding this method is normally not necessary. In most cases, it is
        /// sufficient to override the OnDraw method.</note>
        /// </remarks>
        public virtual void Draw(Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridCellLayout layout = this.PerformLayout(rowIndex, colIndex, style, cellRectangle);

            Rectangle backgroundRectangle = cellRectangle;
            if (!this.shouldDrawBorders)
            {
                backgroundRectangle = layout.InnerRectangle;
            }

            this.DrawBackground(g, backgroundRectangle, style, style.BackgroundImage != null && !Grid.m_bDrawBannerCell);
            
            bool IsEditWithFormat = this.ShouldDrawEditing(rowIndex, colIndex) & !string.IsNullOrEmpty(style.Format);
            double result;
            if (this.Grid.DisableFormattedTextInEditMode && IsEditWithFormat && style.CellValue != null 
                && (Double.TryParse(style.CellValue.ToString(), out result)))
            {
                this.ControlText = this.ControlValue.ToString();
                Grid.CurrentCell.IsModified = false;
            }

            this.OnDraw(g, layout.ClientRectangle, rowIndex, colIndex, style);

            bool showButton = this.OnQueryShowButtons(rowIndex, colIndex, style);

            if (showButton)
            {
                for (int i = 0; i < this.Buttons.Count; i++)
                {
                    GridCellButton button;
                    if ((button = this.GetButton(i)) != null)
                    {
                        if (!layout.Buttons[i].IsEmpty)
                        {
                            button.Bounds = layout.Buttons[i];
                            if (cellRectangle.Contains(button.Bounds))
                            {
                                this.OnDrawCellButton(
                                    button,
                                    g,
                                    rowIndex,
                                    colIndex,
                                    Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && !Grid.IsPrinting() && this.Grid.HasControlFocus,
                                    style);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the cell button element at the specified row and column index.
        /// </summary>
        /// <param name="button">The <see cref="GridCellButton"/> to be drawn.</param>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bActive">True if this is the active current cell; False otherwise.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        protected virtual void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            GridDrawCellButtonEventArgs e = new GridDrawCellButtonEventArgs(button, g, rowIndex, colIndex, bActive, style);
            this.grid.RaiseDrawCellButton(e);
            if (!e.Cancel)
            {
                button.Draw(g, rowIndex, colIndex, this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && !Grid.IsPrinting() && this.Grid.HasControlFocus, style);
            }
        }

        /// <summary>
        /// Draws a button using <see cref="ControlPaint.DrawButton(System.Drawing.Graphics,System.Drawing.Rectangle,System.Windows.Forms.ButtonState)"/>; if XP Themes
        /// are enabled, button will be drawn themed.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to have buttons with a customized color, e.g. a push button
        /// drawn with the back color as specified with the interior property.
        /// </remarks>
        /// <param name="button">The <see cref="GridCellButton"/> to be drawn.</param>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="buttonState">A <see cref="ButtonState"/> that specifies the current state.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <example>
        /// This example draws a custom button element background.
        /// <code lang="C#">
        ///         public static void Draw3dFrame(Graphics g, int x0, int y0, int x1, int y1, int w, Color rgbTopLeft, Color rgbBottomRight)
        ///         {
        ///             Rectangle rc;
        /// <para/>
        ///             for (int i = 0; i != w; i++)
        ///             {
        ///                 // Top
        ///                 Brush brTL = new SolidBrush(rgbTopLeft);
        ///                 rc = Rectangle.FromLTRB(x0, y0, x1, y0+1);
        ///                 g.FillRectangle(brTL, rc);
        /// <para/>
        ///                 // Left
        ///                 rc = Rectangle.FromLTRB(x0, y0, x0+1, y1);
        ///                 g.FillRectangle(brTL, rc);
        ///                 brTL.Dispose();
        /// <para/>
        ///                 Brush brBR = new SolidBrush(rgbBottomRight);
        /// <para/>
        ///                 // Bottom
        ///                 rc = Rectangle.FromLTRB(x0, y1, x1+1, y1+1);
        ///                 g.FillRectangle(brBR, rc);
        /// <para/>
        ///                 // Right
        ///                 rc = Rectangle.FromLTRB(x1, y0, x1+1, y1);
        ///                 g.FillRectangle(brBR, rc);
        ///                 brBR.Dispose();
        /// <para/>
        ///                 if (i != w-1)
        ///                 {
        ///                     x0++;
        ///                     y0++;
        ///                     x1--;
        ///                     y1--;
        ///                 }
        ///             }
        ///         }
        ///         protected override void OnDrawCellButtonBackground(GridCellButton button, Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        ///         {
        ///             bool drawPressed = (buttonState &amp; ButtonState.Pushed) != 0;
        ///             Color hilight = SystemColors.ControlLightLight;
        ///             Color shadow = SystemColors.ControlDarkDark;
        ///             if (!drawPressed)
        ///             {
        ///                 GridPaint.Draw3dFrame(g, rect.Left, rect.Top, rect.Right-1, rect.Bottom-1, 1,
        ///                     hilight, shadow);
        ///             }
        ///             else
        ///             {
        ///                 Brush br = new SolidBrush(shadow);
        ///                 g.FillRectangle(br, Rectangle.FromLTRB(rect.Left, rect.Bottom-1, rect.Right-1, rect.Bottom));
        ///                 g.FillRectangle(br, Rectangle.FromLTRB(rect.Right-1, rect.Top, rect.Right, rect.Bottom));
        ///                 br.Dispose();
        ///             }
        ///         }
        /// </code>
        /// <code lang="VB">
        /// Public Shared Sub Draw3dFrame(g As Graphics, x0 As Integer, y0 As Integer, x1 As Integer, y1 As Integer, w As Integer, rgbTopLeft As Color, rgbBottomRight As Color)
        ///     Dim rc As Rectangle
        /// <para/>
        ///     Dim i As Integer
        /// <para/>
        ///     While i &lt;&gt; w
        ///         ' Top
        ///         Dim brTL = New SolidBrush(rgbTopLeft)
        ///         rc = Rectangle.FromLTRB(x0, y0, x1, y0 + 1)
        ///         g.FillRectangle(brTL, rc)
        /// <para/>
        ///         ' Left
        ///         rc = Rectangle.FromLTRB(x0, y0, x0 + 1, y1)
        ///         g.FillRectangle(brTL, rc)
        ///         brTL.Dispose()
        /// <para/>
        ///         Dim brBR = New SolidBrush(rgbBottomRight)
        /// <para/>
        ///         ' Bottom
        ///         rc = Rectangle.FromLTRB(x0, y1, x1 + 1, y1 + 1)
        ///         g.FillRectangle(brBR, rc)
        /// <para/>
        ///         ' Right
        ///         rc = Rectangle.FromLTRB(x1, y0, x1 + 1, y1)
        ///         g.FillRectangle(brBR, rc)
        ///         brBR.Dispose()
        /// <para/>
        ///         If i &lt;&gt; w - 1 Then
        ///             x0 += 1
        ///             y0 += 1
        ///             x1 -= 1
        ///             y1 -= 1
        ///         End If
        ///         i += 1
        ///     End While
        /// End Sub 'Draw3dFrame
        /// <para/>
        /// Protected Overrides Sub OnDrawCellButtonBackground(button As GridCellButton, g As Graphics, rect As Rectangle, buttonState As ButtonState, style As GridStyleInfo)
        ///     Dim drawPressed As Boolean = (buttonState And ButtonState.Pushed) &lt;&gt; 0
        ///     Dim hilight As Color = SystemColors.ControlLightLight
        ///     Dim shadow As Color = SystemColors.ControlDarkDark
        ///     If Not drawPressed Then
        ///         GridPaint.Draw3dFrame(g, rect.Left, rect.Top, rect.Right - 1, rect.Bottom - 1, 1, hilight, shadow)
        ///     Else
        ///         Dim br = New SolidBrush(shadow)
        ///         g.FillRectangle(br, Rectangle.FromLTRB(rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom))
        ///         g.FillRectangle(br, Rectangle.FromLTRB(rect.Right - 1, rect.Top, rect.Right, rect.Bottom))
        ///         br.Dispose()
        ///     End If
        /// End Sub 'OnDrawCellButtonBackground
        /// </code>
        /// </example>
        protected virtual void OnDrawCellButtonBackground(GridCellButton button, Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {
            GridDrawCellButtonBackgroundEventArgs e = new GridDrawCellButtonBackgroundEventArgs(button, g, rect, buttonState, style);
            this.grid.RaiseDrawCellButtonBackground(e);
            if (!e.Cancel)
            {
                button.DrawButton(g, rect, buttonState, style);
            }
        }

        internal void RaiseDrawCellButtonBackground(GridCellButton button, Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {
            this.OnDrawCellButtonBackground(button, g, rect, buttonState, style);
        }

        /// <summary>
        /// Determines whether the cell buttons shall be drawn for the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>returns boolean value to indicate the cell buttons shall be drawn for the specific row and column index.</returns>
        protected virtual bool OnQueryShowButtons(int rowIndex, int colIndex, GridStyleInfo style)
        {
            return
                !Grid.IsPrinting()
                && (style.ShowButtons == GridShowButtons.Show
                || (style.ShowButtons == GridShowButtons.ShowCurrentRow && this.Grid.IsShowCurrentRow(rowIndex))
                || (style.ShowButtons == GridShowButtons.ShowCurrentCell && this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                || (style.ShowButtons == GridShowButtons.ShowCurrentCellEditing && this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && this.Grid.CurrentCell.IsEditing));
        }

        /// <summary>
        /// This method is called from the cell renderer's draw method to draw the contents
        /// of the client bounds for the cell, e.g. the text for a static cell.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="clientRectangle">Specifies the client rectangle. It is the cell rectangle without buttons and borders.</param>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        /// <remarks>
        /// When drawing cells, it is necessary to determine if the
        /// specified cell (with rowIndex and colIndex) is the current cell and if
        /// it is active.<para/>
        /// If your control is associated with a control and supports in-place editing
        /// and the specified cell is the current cell, you should position the control. Take a look at the GridTextBoxCellRenderer
        /// implementation of this method for an example.<para/>
        /// Otherwise, you can simply draw the cell. <para/>
        /// </remarks>
        protected virtual void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
        }

        private ArrayList Buttons
        {
            get
            {
                if (this.buttons == null)
                {
                    this.buttons = new ArrayList();
                }

                return this.buttons;
            }
        }

        /// <summary>
        /// Adds a <see cref="GridCellButton"/> to the cell renderers list of cell buttons.
        /// </summary>
        /// <param name="button">The <see cref="GridCellButton"/> to add.</param>
        protected void AddButton(GridCellButton button)
        {
            if (this.buttons == null)
            {
                this.buttons = new ArrayList();
            }

            this.buttons.Add(button);
            button.Clicked += new GridCellEventHandler(this.ButtonClicked);
        }

        internal void IntAddButton(GridCellButton button)
        {
            this.AddButton(button);
        }

        /// <summary>
        /// Removes a GridCellButton from the cell renderer's list of cell buttons.
        /// </summary>
        /// <param name="button">The <see cref="GridCellButton"/> to remove.</param>
        protected void RemoveButton(GridCellButton button)
        {
            if (this.buttons == null)
            {
                return;
            }

            if (this.buttons.Contains(button))
            {
                this.buttons.Remove(button);
            }

            button.Clicked -= new GridCellEventHandler(this.ButtonClicked);
        }

        internal void IntRemoveButton(GridCellButton button)
        {
            this.RemoveButton(button);
        }

        /// <summary>
        /// Returns a reference to the the GridCellButton at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="GridCellButton"/>.</param>
        /// <returns>The <see cref="GridCellButton"/> at the index.</returns>
        protected GridCellButton GetButton(int index)
        {
            if (this.buttons == null || index >= this.buttons.Count)
            {
                return null;
            }

            return (GridCellButton)this.buttons[index];
        }

        /// <summary>
        /// Checks if the background for a cell needs drawing and optionally calls <see cref="OnDrawCellBackground"/>
        /// and also draws the frame for the cell as specified in the style object.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The Rectangle</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="fillBackground">True if background needs to be redrawn.</param>
        /// <remarks>
        /// The method will call <see cref="OnDrawCellBackground"/> for covered cells and bannered cells. For regular
        /// cells, you can force <see cref="OnDrawCellBackground"/> to be called by assigning BrushInfo.Empty
        /// to the <see cref="GridStyleInfo.Interior"/> property of a <see cref="GridStyleInfo"/>. You can
        /// do this in <see cref="OnPrepareViewStyleInfo"/>, for example.
        /// <para/>
        /// Otherwise <see cref="OnDrawCellBackground"/> will not be called
        /// for regular cells because of internal drawing optimizations within the grid. By default
        /// the grid optimizes drawing such that the background of neighboring cells with the same
        /// color is drawn in one operation.
        /// <para/>
        /// After the background of a cell has been drawn (either by a call to <see cref="OnDrawCellBackground"/>
        /// or because of earlier drawing inside the grid) the cells frame is drawn as specified
        /// with <see cref="GridStyleInfo.CellAppearance"/> by calling <see cref="DrawCellAppearance"/>.
        /// </remarks>
        protected virtual void DrawBackground(Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground /*=false*/)
        {
            if (rect.Height == 0 || rect.Width == 0)
            {
                return;
            }

            bool bColor = !(this.Grid.PrintingMode && this.Grid.Model.Properties.BlackWhite);
            GridRangeInfo range = GridRangeInfo.Cell(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex);
            GridDrawCellBackgroundEventArgs e = new GridDrawCellBackgroundEventArgs(g, range, false, bColor, style, rect, rect, false);

            //// Check if background needs to be drawn.
            //// this.Grid.m_bDrawCoveredCell - this flag is set in OnDrawClientRowCol when a covered cell is drawn
            if (fillBackground || (!Grid.Model.Options.TransparentBackground
                && (this.Grid.m_bDrawCoveredCell
                || this.Grid.m_bForceDrawBackground)))
            {
                this.grid.RaiseDrawCellBackground(e);
                if (!e.Cancel)
                {
                    this.OnDrawCellBackground(e);
                }
            }

            e.Cancel = false;
            this.grid.RaiseDrawCellFrameAppearance(e);
            if (!e.Cancel)
            {
                this.DrawCellAppearance(g, rect, style);
            }
        }

        /// <summary>
        /// Called to draw / erase the background for a cell.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        /// <remarks>
        /// This method is called for covered cells and bannered cells. For regular
        /// cells, you can force <see cref="OnDrawCellBackground"/> to be called by assigning BrushInfo.Empty
        /// to the <see cref="GridStyleInfo.Interior"/> property of a <see cref="GridStyleInfo"/>. You can
        /// do this for example in <see cref="OnPrepareViewStyleInfo"/>.
        /// <para/>
        /// Otherwise <see cref="OnDrawCellBackground"/> will not be called
        /// for regular cells because of internal drawing optimizations within the grid. By default
        /// the grid optimizes drawing such that the background of neighboring cells with the same
        /// color is drawn in one operation.
        /// <para/>
        /// The method will also not be called if you handle the <see cref="GridControlBase.DrawCellBackground"/>
        /// event of <see cref="GridControlBase"/> and set e.Cancel = true.
        /// <para/>
        /// The default version of this method fills the entire background as specified
        /// with <see cref="GridStyleInfo.Interior"/> and also draws the image specified
        /// with <see cref="GridStyleInfo.BackgroundImage"/>.
        /// </remarks>
        protected virtual void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            bool bColor = e.IsColored;
            Graphics g = e.Graphics;
            Rectangle rect = e.TargetBounds;
            GridStyleInfo style = e.Style;

            //// No background needed (when printing).
            if (bColor)
            {
                BrushPaint.FillRectangle(g, rect, this.grid.GetInterior(style.Interior));
            }
            else
            {
                BrushPaint.FillRectangle(g, rect, style.Interior.MakeBlackAndWhite());
            }

            Image image = style.BackgroundImage;

            if (image != null)
            {
                if (style.BackgroundImageMode == GridBackgroundImageMode.TileImage)
                {
                    using (TextureBrush textureBrush = new TextureBrush(image, WrapMode.Tile))
                    {   
                        if (rect.Location != Point.Empty)
                        {
                            Matrix brushTransform = textureBrush.Transform;
                            brushTransform.Translate((float)rect.X, (float)rect.Y);
                            textureBrush.Transform = brushTransform;
                        }
                        g.FillRectangle(textureBrush, rect);
                    }
                }
                else
                {
                    PictureBoxSizeMode sizeMode = GridUtil.ConvertToPictureBoxSizeMode(style.BackgroundImageMode);
                    Rectangle imageRect = GridUtil.GetImageRectangle(image, rect.Size, sizeMode);
                    imageRect.Offset(rect.Location);
                    if (!e.IsClipped && (imageRect.Width > rect.Width || imageRect.Height > rect.Height))
                    {
                        Region clip = g.Clip;
                        g.IntersectClip(rect);
                        g.DrawImage(image, imageRect);
                        g.Clip = clip;
                    }
                    else
                    {
                        g.DrawImage(image, imageRect);
                    }
                }
            }
        }

        internal void RaiseDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            this.OnDrawCellBackground(e);
        }

        /// <summary>
        /// Draws the frame for the cells appearance (sunken or raised).
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <remarks>
        /// If you want to draw custom borders around a cell, be sure to reserve space
        /// for the extra area occupied by your borders. See <see cref="GridStyleInfo.BorderMargins"/>.
        /// </remarks>
        protected virtual void DrawCellAppearance(Graphics g, Rectangle rect, GridStyleInfo style)
        {
            bool bColor = !(this.Grid.PrintingMode && this.Grid.Model.Properties.BlackWhite);

            if (bColor)
            {
                Rectangle r = this.Model.SubtractBorders(rect, style, false);

                GridCellAppearance frame = style.CellAppearance;

                Color rgb3dDkShadow = SystemColors.ControlDarkDark;
                Color rgb3dHilight = SystemColors.ControlLightLight;

                switch (frame)
                {
                    case GridCellAppearance.Raised:
                        GridPaint.Draw3dFrame(g, r.Left - 1, r.Top - 1, r.Right, r.Bottom, 1, rgb3dHilight, rgb3dDkShadow);
                        break;

                    case GridCellAppearance.Sunken:
                        GridPaint.Draw3dFrame(g, r.Left - 1, r.Top - 1, r.Right, r.Bottom, 1, rgb3dDkShadow, rgb3dHilight);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if this is a header cell that should indicate current cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>True if header shall be outlined; False otherwise.</returns>
        protected virtual bool GetMarkHeaderState(int rowIndex, int colIndex, GridStyleInfo style)
        {
            return style.BackColor == SystemColors.Highlight ||
            (this.Grid.OutlineCurrentCellHeaderManager != null ?
                 this.Grid.OutlineCurrentCellHeaderManager.GetMarkHeaderState(this, rowIndex, colIndex, style) : false);
        }

        /// <summary>
        /// Gets a reference to the associated control. If no control is
        /// associated with the cell type, NULL will be returned.
        /// </summary>
        /// <returns>Reference to the associated control. Can be NULL.</returns>
        public Control Control
        {
            get
            {
                return this.control;
            }
        }

        /// <summary>
        /// Sets a reference to the associated control.
        /// </summary>
        /// <param name="control">The <see cref="Control"/> to be associated with.</param>
        protected internal void SetControl(Control control)
        {
            if (this.control != control)
            {
                if (this.control != null)
                {
                    this.UnwireControl();
                }

                this.control = control;
                if (this.control != null)
                {
                    this.WireControl();
                }
            }
        }

        void WireControl()
        {
            this.control.LostFocus += new EventHandler(this.ControlLostFocus);
            this.control.GotFocus += new EventHandler(this.ControlGotFocus);
            this.control.MouseDown += new MouseEventHandler(this.ControlMouseDown);
            ////this.control.DoubleClick += new EventHandler(ControlDoubleClick);
        }

        void UnwireControl()
        {
            this.control.LostFocus -= new EventHandler(this.ControlLostFocus);
            this.control.GotFocus -= new EventHandler(this.ControlGotFocus);
            this.control.MouseDown -= new MouseEventHandler(this.ControlMouseDown);
            ////this.control.DoubleClick -= new EventHandler(ControlDoubleClick);
        }

        /// <summary>
        /// Handles the LostFocus event of the embedded control and raises the
        /// <see cref="GridControlBase.CurrentCellControlLostFocus"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ControlLostFocus(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.NotifyCurrentCellControlLostFocus(this.Control);
        }

        /// <summary>
        /// Handles the GotFocus event of the embedded control and raises the
        /// <see cref="GridControlBase.CurrentCellControlGotFocus"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ControlGotFocus(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.NotifyCurrentCellControlGotFocus(this.Control);
        }

        bool inSetControlText = false;

        /// <summary>
        /// Gets a value indicating whether ControlText setter was called. True when ControlText setter was called.
        /// </summary>
        public bool InSetControlText
        {
            get
            {
                return this.inSetControlText;
            }
        }

        /// <summary>
        /// Gets or sets the active text that is displayed for the current cell, e.g. TextBox.Text.
        /// </summary>
        public virtual string ControlText
        {
            get
            {
                if (!this.HasControlText && this.HasControlValue)
                {
                    try
                    {
                        GridStyleInfo style = this.StyleInfo;
                        object value = this.ControlValue;
                        style.CellValue = value;
                        this.controlText = style.GetFormattedText(value, GridCellBaseTextInfo.CurrentText);
                        this.hasControlText = true;
                    }
                    finally
                    {
                    }
                }

                return this.controlText;
            }

            set
            {
                if (this.InSetControlText)
                {
                    return;
                }
#if DEBUG

                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else

                    ;
#endif

                this.inSetControlText = true;
                try
                {
                    this.onSetControlTextFailed = false;

                    if (!this.InSetControlValue && !InSetSetControlValue)
                    {
                        this.ResetControlValue();
                    }

                    bool _hasControlText = this.hasControlText;
                    string _controlText = this.controlText;
                    this.hasControlText = true;
                    this.controlText = value;
                    this.OnSetControlText(controlText);
                    if (this.onSetControlTextFailed)
                    {
                        this.hasControlText = _hasControlText;
                        this.controlText = _controlText;
                    }

                    if (this.CurrentCell.IsModified)
                    {
                        this.Model.SetActiveText(currentRowIndex, currentColIndex, value);
                    }
                }
                finally
                {
                    this.inSetControlText = false;
                }
            }
        }

        internal bool onSetControlTextFailed = false;

        /// <summary> Gets or sets a value indicating whether last OnSetControl TextFailed. For internal use.</summary>
        /// <exclude/>
        /// <remarks>
        /// GridComboBoxCellRenderer.OnSetControlText sets this LastOnSetControlTextFailed = true
        /// if (TextBoxText != text) after calling base.OnSetControlText(text); // Sets TextBoxText.
        /// <para/>
        /// In that case ControlText will revert back any changes.
        /// <para/>
        /// This fixes issue with cancelling Changing event in comboboxes.
        /// </remarks>
        [Browsable(false)]
        public bool LastOnSetControlTextFailed
        {
            get
            {
                return this.onSetControlTextFailed;
            }

            set
            {
                this.onSetControlTextFailed = value;
            }
        }

        /// <summary>
        /// Called right after ControlText was set.
        /// </summary>
        /// <param name="text">The ControlText</param>
        protected virtual void OnSetControlText(string text)
        {
        }

        /// <summary>
        /// Gets a value indicating whether ControlText for the current cell has been set.
        /// </summary>
        public bool HasControlText
        {
            get
            {
                return this.hasControlText;
            }
        }

        /// <summary>
        /// Resets the ControlText to its original state.
        /// </summary>
        public void ResetControlText()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.hasControlText = false;
            this.controlText = string.Empty;
        }

        bool inSetControlValue = false;

        /// <summary>
        /// Gets a value indicating whether the ControlValue setter was called. True if ControlValue setter was called.
        /// </summary>
        public bool InSetControlValue
        {
            get
            {
                return this.inSetControlValue;
            }
        }

        /// <summary>
        /// Gets or sets the cell value for the current cell.
        /// </summary>
        public virtual object ControlValue
        {
            get
            {
                if (!this.HasControlValue && this.HasControlText)
                {
                    try
                    {
                        GridStyleInfo style = this.StyleInfo;
                        this.Model.ApplyFormattedText(style, ControlText, GridCellBaseTextInfo.TextBox);
                        this.controlValue = style.CellValue;
                        this.hasControlValue = true;
                    }
                    catch
                    {
                        this.controlValue = null;
                        this.hasControlValue = true;
                    }
                    finally
                    {
                    }
                }

                return this.controlValue;
            }

            set
            {
                this.inSetControlValue = true;
                try
                {
                    this.SetControlValue(value, true);
                }
                finally
                {
                    this.inSetControlValue = false;
                }
            }
        }

        /// <summary>
        /// Forces the <see cref="ControlValue"/> to reflect current control contents. Call this method
        /// if you want to know the value that will be saved in <see cref="GridStyleInfo.CellValue"/>
        /// if user would accept current cells contents.
        /// </summary>
        /// <remarks>
        /// This method requires that <see cref="ControlText"/> contains most recent value. Override this
        /// method and derived cell renderers if you need to get the text from the embedded control. <para/>
        /// TextBoxCellRenderer does for example get the value from <see cref="GridTextBoxCellRenderer.TextBoxText"/> instead.
        /// </remarks>
        public virtual void UpdateControlValue()
        {
            try
            {
                GridStyleInfo style = this.StyleInfo;
                style.Text = this.ControlText;
                this.SetControlValue(style.CellValue, false);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        bool inSetSetControlValue = false;

        /// <summary>
        /// Gets a value indicating whether <see cref="SetControlValue"/> method was called. True when <see cref="SetControlValue"/> method was called.
        /// </summary>
        public bool InSetSetControlValue
        {
            get
            {
                return this.inSetSetControlValue;
            }
        }

        /// <summary>
        /// Sets the cell value for the current cell and optionally initializes the ControlText based on the value.
        /// </summary>
        /// <param name="value">The cell value.</param>
        /// <param name="initControlText">True if InitializeControlText should be called.</param>
        protected void SetControlValue(object value, bool initControlText)
        {
            this.inSetSetControlValue = true;
            try
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                    ;
#endif

                if (this.currentStyle.CellValueType == null || value == null || value is DBNull)
                {
                }
                else if (value.GetType() != this.currentStyle.CellValueType)
                {
                    if (!string.IsNullOrEmpty(this.currentStyle.Format))
                    {
                        value = GridCellValueConvert.ChangeType(value, this.currentStyle.CellValueType, this.currentStyle.GetCulture(true), this.currentStyle.Format, true);
                    }
                    else if (this.Model.GetTypeConverter(this.currentStyle) != null && this.Model.GetTypeConverter(this.currentStyle).GetType().BaseType.Name.Equals("EnumConverter") )
                    {
                        TypeConverter converter = this.Model.GetTypeConverter(this.currentStyle);
                        if (converter.CanConvertFrom(typeof(string)))
                            value = converter.ConvertFrom(value);
                    }
                    else
                    {
                        value = GridCellValueConvert.ChangeType(value, this.currentStyle.CellValueType, this.currentStyle.GetCulture(true), true);
                    }

                    if (value is DBNull)
                    {
                        this.hasControlValue = false;
                        this.controlValue = null;
                        if (initControlText && !this.InSetControlText)
                        {
                            this.InitializeControlText(value);
                        }

                        return;
                    }
                }

                this.hasControlValue = true;
                this.controlValue = value;
                this.currentStyle.CellValue = value;
                if (initControlText && !this.InSetControlText)
                {
                    this.InitializeControlText(value);
                }
            }
            finally
            {
                this.inSetSetControlValue = false;
            }
        }

        /// <summary>
        /// Initializes <see cref="ControlText"/> based on the specified cell value.
        /// </summary>
        /// <param name="controlValue">
        /// The cell value. See the <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridStyleInfo"/> class.
        /// </param>
        /// <remarks>
        /// The default behavior of this method is to call <see cref="GridStyleInfo.GetFormattedText(object)"/>
        /// of the <see cref="StyleInfo"/> object and then raise a <see cref="GridControlBase.CurrentCellInitializeControlText"/>
        /// event.<para/>
        /// The user can catch the event and change the <see cref="GridCurrentCellInitializeControlTextEventArgs.ControlText"/>
        /// property of the <see cref="GridCurrentCellInitializeControlTextEventArgs"/> object.
        /// </remarks>
        protected virtual void InitializeControlText(object controlValue)
        {
            string text = this.StyleInfo.GetFormattedText(controlValue, GridCellBaseTextInfo.CurrentText);
            GridCurrentCellInitializeControlTextEventArgs e = new GridCurrentCellInitializeControlTextEventArgs(this.RowIndex, this.ColIndex, this.StyleInfo, controlValue, text);
            if (this.Grid.RaiseCurrentCellInitializeControlText(e))
            {
                this.ControlText = e.ControlText;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cell value for the current cell has been changed.
        /// </summary>
        public bool HasControlValue
        {
            get
            {
                return this.hasControlValue;
            }
        }

        /// <summary>
        /// Resets the cell value of the current cell to its original state.
        /// </summary>
        public void ResetControlValue()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.hasControlValue = false;
            this.controlValue = null;
        }

        /// <summary>
        /// Returns <see cref="ControlText"/>.
        /// </summary>
        /// <returns>Display text.</returns>
        public virtual string GetDisplayText()
        {
            return this.ControlText;
        }

        /// <summary>
        /// Returns <see cref="ControlValue"/>.
        /// </summary>
        /// <returns>Cell value.</returns>
        public virtual object GetCellValue()
        {
            return this.ControlValue;
        }

        /// <summary>
        /// Gets a value indicating whether is in initialize method. True while in initialize method.
        /// </summary>
        protected bool InInitialize
        {
            get
            {
                return this.inInitialize;
            }
        }

        WeakReference _currentStyle = new WeakReference(null);

        GridStyleInfo currentStyle
        {
            get
            {
                GridStyleInfo style = this._currentStyle.Target as GridStyleInfo;
                if (style == null)
                {
                    style = this.StyleInfo;
                    this._currentStyle = new WeakReference(style);
                }

                return style;
            }

            set
            {
                this._currentStyle = new WeakReference(value);
            }
        }

        /// <summary>
        /// Gets the current view style for the current cell.
        /// </summary>
        public GridStyleInfo CurrentStyle
        {
            get
            {
                return this.currentStyle;
            }
        }

        /// <summary>
        /// Initializes the intrinsic state of the control for the current cell.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <remarks>
        /// Initializes the intrinsic state (e.g., row, column, style) of
        /// the control for the current cell. This method is called when
        /// the current cell has moved. The operations that follow all
        /// depend on the intrinsic state of the control.<para/>
        /// Initialize calculates the cell layout (inner, client rectangle bounds) and then calls OnInitialize.
        /// </remarks>
        public void Initialize(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
               ;
#endif

            this.lastValidateTime = int.MinValue;
            this.lastValidateString = null;

            //// called from OnDrawItem, GridCurrentCell.Initialize
            //// GridCurrentCell.Initialize called from MoveTo if same cell as before, MoveTo after Activate,
            this.currentRowIndex = rowIndex;
            this.currentColIndex = colIndex;
            this.inInitialize = true;
            this.initalizeCalled = true;
            this.PerformLayout(rowIndex, colIndex);
            this.hasControlText = false;
            this.hasControlValue = false;
            this.currentStyle = StyleInfo;
            this.Model.activeTextValue.ResetValue();
            this.OnInitialize(rowIndex, colIndex);
            if (this.dropDownImp != null)
            {
                this.dropDownImp.OnInitialize(rowIndex, colIndex);
            }

            this.inInitialize = false;
            ////               lastModified = false;
        }

        /// <summary>
        /// This method gets called from the cell renderer's initialize method. Override this method if you need to any initialization
        /// for the current cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        protected virtual void OnInitialize(int rowIndex, int colIndex)
        {
            if (!this.HasControlText && !this.HasControlValue)
            {
                GridStyleInfo style = this.Grid.Model[rowIndex, colIndex];
                this.ControlValue = style.CellValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this cell type serves as current cell or if
        /// the current cell is at the specified row and column id.
        /// </summary>
        /// <returns>True if control is used as current cell; False if control is not current cell.
        /// </returns>
        public bool Initalized
        {
            get
            {
                return this.initalizeCalled;
            }
        }

        /// <summary>
        /// Determines whether this cell type serves as current cell or if
        /// the current cell is at the specified row and column id.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if control is used as current cell; False if control is not current cell.
        /// </returns>
        public bool InitalizedAt(int rowIndex, int colIndex)
        {
            return this.initalizeCalled && currentRowIndex == rowIndex && currentColIndex == colIndex;
        }

        /// <summary>
        /// If there is a control associated with the cell type (for in-place editing), override this
        /// method to hide the control.
        /// </summary>
        public virtual void Hide()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            Control c = this.Control;
            if (c != null)
            {
                Rectangle r = c.Bounds;
                if (r.IntersectsWith(this.grid.GridBounds))
                {
                    ////                         if (!c.ContainsFocus)
                    ////                              c.Visible = false;
                    ////                         else
                    {
                        this.grid.SuspendLayout();
                        c.Location = new Point(10000, 10000);
                        this.grid.ResumeLayout(false);
                    }
                    ////Grid.Invalidate(r);
                }
            }
        }

        /// <summary>
        /// Determines if there is a control associated with the cell type (for in-place editing) and
        /// if it is visible.
        /// </summary>
        /// <returns>True if it is visible; False otherwise.</returns>
        public virtual bool IsControlVisible()
        {
            Control c = this.Control;
            if (c != null)
            {
                Rectangle r = c.Bounds;
                return r.IntersectsWith(this.grid.GridBounds);
            }

            return false;
        }

        byte charsToIgnore = 0;

        /// <summary>
        /// Calls protected virtual <see cref="ProcessKeyEventArgs"/> method.
        /// </summary>
        /// <param name="m">The <see cref="Message"/> with data of the keyboard event.</param>
        /// <returns>True if key was handled; False otherwise.</returns>
        public bool RaiseProcessKeyEventArgs(ref Message m)
        {
            return this.ProcessKeyEventArgs(ref m);
        }

        /// <summary>
        /// This is called from GridControlBase.ProcessKeyEventArgs and allows your customized cell renderer
        /// to process keyboard events before the GridControlBase gets the actual KeyDown / KeyUp event.
        /// </summary>
        /// <param name="m">The <see cref="Message"/> with data of the keyboard event.</param>
        /// <returns>True if key was handled; False otherwise.</returns>
        protected virtual bool ProcessKeyEventArgs(ref Message m)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(m.ToString());
            }
#else
               ;
#endif

            KeyEventArgs keyEventArgs0 = null;
            KeyPressEventArgs keyPressEventArgs1 = null;

            if (m.Msg == 0x102/*WM_CHAR*/ || m.Msg == 0x106/*WM_SYSCHAR*/)
            {
                if (this.charsToIgnore > (byte)0)
                {
                    this.charsToIgnore = (byte)(this.charsToIgnore - 1);
                    return false;
                }

                keyPressEventArgs1 = new KeyPressEventArgs((char)m.WParam);
                if (this.CurrentCell.IsEditing && !Grid.IsVisibleCell(this.RowIndex, this.ColIndex))
                {
                    this.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }

                this.Grid.OnCurrentCellKeyPress(keyPressEventArgs1);
                if (!keyPressEventArgs1.Handled)
                {
                    this.OnKeyPress(keyPressEventArgs1);
                }
            }
            else
            {
                if (m.Msg == 0x286/*WM_IME_CHAR*/)
                {
                    this.charsToIgnore = (byte)(this.charsToIgnore + ((byte)(3 - Marshal.SystemDefaultCharSize)));
                    if (Marshal.SystemDefaultCharSize == 1)
                    {
                        byte[] bs4 = new byte[]
                              {
                                   ((byte)((int) m.WParam >> 8)),
                                   ((byte)(int) m.WParam)
                              };
                        string s = Encoding.Default.GetString(bs4);
                        keyPressEventArgs1 = new KeyPressEventArgs(s[0]);
                    }
                    else
                    {
                        keyPressEventArgs1 = new KeyPressEventArgs((char)m.WParam);
                    }

                    this.Grid.OnCurrentCellKeyPress(keyPressEventArgs1);
                    if (!keyPressEventArgs1.Handled)
                    {
                        this.OnKeyPress(keyPressEventArgs1);
                    }
                }
                else
                {
                    keyEventArgs0 = new KeyEventArgs((Keys)(int)m.WParam | Control.ModifierKeys);
                    if (m.Msg == 0x100/*WM_KEYFIRST*/ || m.Msg == 0x104/*WM_SYSKEYDOWN*/)
                    {
                        this.Grid.OnCurrentCellKeyDown(keyEventArgs0);
                        if (!keyEventArgs0.Handled)
                        {
                            this.OnKeyDown(keyEventArgs0);
                        }
                    }
                    else
                    {
                        this.Grid.OnCurrentCellKeyUp(keyEventArgs0);
                        if (!keyEventArgs0.Handled)
                        {
                            this.OnKeyUp(keyEventArgs0);
                        }
                    }
                }
            }

            if (keyPressEventArgs1 != null)
            {
                return keyPressEventArgs1.Handled;
            }

            return keyEventArgs0.Handled;
        }

        /// <summary>
        /// Calls OnKeyDown.
        /// </summary>
        /// <param name="e">The KeyEventArgs</param>
        internal void RaiseKeyDown(KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        /// <summary>
        /// Calls OnKeyPress.
        /// </summary>
        /// <param name="e">The KeyPressEventArg</param>
        internal void RaiseKeyPress(KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        /// <summary>
        /// User pressed key down (similar to Control.OnKeyDown).
        /// </summary>
        /// <param name="e">The KeyEventArgs</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#else
               ;
#endif

            if (this.dropDownImp != null)
            {
                this.dropDownImp.OnKeyDown(e);
            }
        }

        /// <summary>
        /// User released key (similar to Control.OnKeyUp).
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> with data of the keyboard event.</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#else
               ;
#endif
        }

        /// <summary>
        /// User pressed key (similar to Control.OnKeyPress).
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> with data of the keyboard event.</param>
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyChar);
            }
#else
               ;
#endif
        }

        // Copy() performs the following job:
        // a) Copy selected text as formatted in the cell.
        // b) Only if cell is inactive, copy the GridStyleInfo info for the cell.

        /// <summary>
        /// Called when user initiates a clipboard copy and the grid has a current cell but no range is selected.
        /// </summary>
        /// <returns>True if successful; False if failed.</returns>
        /// <remarks>
        /// Copy() performs the following tasks:<para/>
        /// a) Copy selected text as formatted in the cell.<para/>
        /// b) Only if cell is inactive copy the GridStyleInfo info for the cell.<para/>
        /// </remarks>
        public virtual bool Copy()
        {
            return false;
        }

        /// <summary>
        /// Called when user initiates a clipboard paste and the grid has a current cell but no range is selected.
        /// </summary>
        /// <returns>True if successful; False if failed.</returns>
        public virtual bool Paste()
        {
            return false;
        }

        /// <summary>
        /// Called when user initiates a clipboard cut and the grid has a current cell but no range is selected.
        /// </summary>
        /// <returns>True if successful; False if failed.</returns>
        public virtual bool Cut()
        {
            return false;
        }

        /// <summary>
        /// Determines whether current cell can be copied to clipboard.
        /// </summary>
        /// <returns>True if copy is supported; otherwise False.</returns>
        public virtual bool CanCopy()
        {
            return false;
        }

        /// <summary>
        /// Determines whether current cell can be cut to clipboard.
        /// </summary>
        /// <returns>True if cut is supported; otherwise False.</returns>
        public virtual bool CanCut()
        {
            return false;
        }

        /// <summary>
        /// Determines whether current cell can be pasted from clipboard.
        /// </summary>
        /// <returns>True if paste is supported; otherwise False.</returns>
        public virtual bool CanPaste()
        {
            return false;
        }

        /// <summary>
        /// Searches the specified text and optionally moves the current cell to the cell if the
        /// text was found.
        /// </summary>
        /// <param name="find">The text to be searched.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">Search criteria.</param>
        /// <param name="bSetCell">True if current cell should be moved after text was found.</param>
        /// <returns>True if text was found; False otherwise.</returns>
        public virtual bool FindText(string find, int rowIndex, int colIndex, GridFindTextOptions options, bool bSetCell)
        {
            bool wholeCell = (options & GridFindTextOptions.MatchWholeCell) != 0;
            bool matchCase = (options & GridFindTextOptions.MatchCase) != 0;

            GridStyleInfo style = this.Grid.Model[rowIndex, colIndex];
            string text = style.FormattedText;
            CultureInfo culture = style.GetCulture(true);

            bool found = false;
            if (wholeCell)
            {
                found = String.Compare(find, text, !matchCase, culture) == 0;
            }
            else
            {
                if (!matchCase)
                {
                    find = find.ToLower(culture);
                    text = text.ToLower(culture);
                }

                found = text.IndexOf(find.ToString()) != -1;
            }

            if (found && bSetCell)
            {
                this.CurrentCell.MoveTo(rowIndex, colIndex);
            }

            return found;
        }

        ////          public virtual bool FindText(object find, int rowIndex, int colIndex, bool bCurrentPos, bool bSetCell)
        ////          {
        ////               //// Skip current cell.
        ////               if (!bCurrentPos && CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
        ////                    return false;
        ////
        ////               string text = Grid.Model[rowIndex, colIndex].FormattedText;
        ////
        ////               if (text.IndexOf(find.ToString()) != -1)
        ////               {
        ////                    if (bSetCell)
        ////                    {
        ////                         CurrentCell.MoveTo(rowIndex, colIndex);
        ////                    }
        ////                    return true;
        ////               }
        ////
        ////               return false;
        ////          }

        /// <summary>
        /// Searches the specified text and optionally moves the current cell to the cell if the
        /// text was found.
        /// </summary>
        /// <param name="find">The text to be searched.</param>
        /// <param name="replace">The replace text.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="options">Search criteria.</param>
        /// <param name="bSetCell">True if current cell should be moved after text was found.</param>
        /// <returns>True if text was found; False otherwise.</returns>
        public virtual bool ReplaceText(string find, string replace, int rowIndex, int colIndex, GridFindTextOptions options, bool bSetCell)
        {
            bool wholeCell = (options & GridFindTextOptions.MatchWholeCell) != 0;
            bool matchCase = (options & GridFindTextOptions.MatchCase) != 0;

            GridStyleInfo style = this.Grid.Model[rowIndex, colIndex];
            string text = style.FormattedText;
            CultureInfo culture = style.GetCulture(true);

            string text2 = text;
            if (!matchCase)
            {
                find = find.ToLower(culture);
                text2 = text.ToLower(culture);
            }

            bool found = false;
            if (wholeCell)
            {
                found = find.Equals(text2);
                if (found)
                {
                    if (bSetCell)
                    {
                        this.CurrentCell.MoveTo(rowIndex, colIndex);
                        if (!this.CurrentStyle.ReadOnly)
                        {
                            this.CurrentCell.BeginEdit();
                            this.CurrentCell.Renderer.ControlText = replace;
                        }
                    }
                    else
                    {
                        if (!this.Grid.Model[rowIndex, colIndex].ReadOnly)
                        {
                            this.Grid.Model[rowIndex, colIndex].FormattedText = replace;
                        }
                    }
                }
            }
            else
            {
                int index = text2.IndexOf(find.ToString());
                if (index != -1)
                {
                    found = true;
                    StringBuilder sb = new StringBuilder();
                    if (index > 0)
                    {
                        sb.Append(text.Substring(0, index));
                    }

                    sb.Append(replace);
                    index += find.Length;
                    int index2 = -1;
                    do
                    {
                        text2 = text.Substring(index);
                        if (!matchCase)
                        {
                            text2 = text2.ToLower(culture);
                        }

                        index2 = text2.IndexOf(find.ToString());
                        if (index2 != -1)
                        {
                            if (index2 > 0)
                            {
                                sb.Append(text.Substring(index, index2));
                            }

                            sb.Append(replace);
                            index += index2 + find.Length;
                        }
                    }
                    while (index2 != -1);
                    if (index < text.Length)
                    {
                        sb.Append(text.Substring(index));
                    }

                    if (bSetCell)
                    {
                        this.CurrentCell.MoveTo(rowIndex, colIndex);
                        if (!this.CurrentStyle.ReadOnly)
                        {
                            this.CurrentCell.BeginEdit();
                            this.CurrentCell.Renderer.ControlText = sb.ToString();
                        }
                    }
                    else
                    {
                        if (!this.Grid.Model[rowIndex, colIndex].ReadOnly)
                        {
                            this.Grid.Model[rowIndex, colIndex].FormattedText = sb.ToString();
                        }
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Replaces the selected text in the current cell.
        /// </summary>
        /// <param name="replacement">The string to replace the current selected text.</param>
        public virtual void ReplaceSel(string replacement)
        {
            if (this.Initalized && Grid.CurrentCell.HasCurrentCellAt(currentRowIndex, currentColIndex))
            {
                this.Grid.Model[currentRowIndex, currentColIndex].ApplyFormattedText(replacement, GridCellBaseTextInfo.ReplaceSelection);
            }
        }

        /// <summary>
        /// Return the selected text in the current cell.
        /// </summary>
        /// <param name="strResult">The resulting string with the selected text</param>
        /// <returns>True if successful; False if failed.</returns>
        public virtual bool GetSelectedText(out string strResult)
        {
            if (this.Initalized)
            {
                strResult = this.ControlText;
                return true;
            }

            strResult = string.Empty;
            return false;
        }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        public GridControlBase Grid
        {
            get
            {
                return this.grid;
            }
        }

        /// <summary>
        /// Called from OnVScroll, OnHScroll before grid is scrolled.
        /// </summary>
        /// <param name="pMsg">The Message</param>
        /// <remarks>
        /// Will deactivate the current cell when the user scrolls to the right and the current cell
        /// is a floating or covered cell which spans the freeze columns / row line.<para/>
        /// Also, any dropped-down windows will be closed.
        /// </remarks>
        public virtual void OnNotifyMsg(ref Message pMsg)
        {
        }

        /// <summary>
        /// Returns whether the current cell is Read-only.
        /// </summary>
        /// <returns>True if Read-only; False otherwise.</returns>
        public virtual bool IsReadOnly()
        {
            return !Grid.Model.IgnoreReadOnly
                 && (this.Grid.Model.IsReadOnly || this.Grid.Model[currentRowIndex, currentColIndex].ReadOnly);
        }

        /// <summary>
        /// Gets a reference to the style object associated with the current cell.
        /// </summary>
        public GridStyleInfo StyleInfo
        {
            get
            {
                return this.Grid.GetViewStyleInfo(currentRowIndex, currentColIndex);
            }
        }

        /// <summary>
        /// Called from OnValidate and checks whether the specified text is valid.
        /// </summary>
        /// <param name="text">Text to be validated.</param>
        /// <returns>True if text if valid; False otherwise.</returns>
        /// <remarks>
        /// This also works for limiting the keyboard input, e.g. only digits.
        /// Called after the user pressed a key and before it is accepted.
        /// </remarks>
        /// <example>
        /// Don't allow "-" to be typed.
        /// <code lang="C#">
        ///           public override bool ValidateString(string text)
        ///           {
        ///                TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, text);
        /// <para/>
        ///                if (text.IndexOf("-") != -1)
        ///                     return false;
        ///                return true;
        ///           }
        /// </code>
        /// </example>
        public virtual bool ValidateString(string text)
        {
            // Prevent multiple equal ValidateString events being raised.
            if (this.lastValidateString != null && text == this.lastValidateString && Environment.TickCount - lastValidateTime < 100)
            {
                return this.lastValidateResult;
            }
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(text);
            }
#else
               ;
#endif
            GridCurrentCellValidateStringEventArgs e = new GridCurrentCellValidateStringEventArgs(text);
            this.Grid.OnCurrentCellValidateString(e);

            this.lastValidateString = text;
            this.lastValidateResult = !e.Cancel;
            this.lastValidateTime = Environment.TickCount;

            return this.lastValidateResult;
        }

        //// Attributes:
        bool _forceRefreshOnActivateCell;
        string lastValidateString = null;
        int lastValidateTime = int.MinValue;
        bool lastValidateResult = false;

        /// <summary>Gets or sets a value indicating whether your control needs to be refreshed when it
        /// becomes the current cell in the grid.</summary>
        /// <remarks>
        /// If you set this
        /// attribute to be true, some grid internal optimizations with
        /// outlining the current cell are turned off. These optimizations
        /// assume that the style of a cell does not change when it has
        /// become the current cell.
        /// If your control's appearance depends on whether it is a
        /// current cell or not, you should set this attribute True (
        /// default is False).
        /// For example, if you have a password control which should be
        /// drawn with plain text only when it is the current cell, you
        /// may set the ForceRefreshOnActivateCell attribute of the
        /// specific control to be true.
        /// </remarks>
        public bool ForceRefreshOnActivateCell
        {
            get
            {
                return this._forceRefreshOnActivateCell;
            }

            set
            {
                this._forceRefreshOnActivateCell = value;
            }
        }

        /// <summary>
        /// Gets or sets the row id of the current cell.
        /// </summary>
        /// <remarks>
        /// This value is only valid if GridControlBase::Init has been called
        /// and an intrinsic state is initialized; if the control is
        /// currently not used as current cell, the value is
        /// undetermined.
        /// </remarks>
        public int RowIndex
        {
            get
            {
                return this.currentRowIndex;
            }

            set
            {
                this.currentRowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the column id of the current cell.
        /// </summary>
        /// <remarks>
        /// This value is only valid if GridControlBase::Init has been called
        /// and intrinsic state is initialized; if the control is
        /// currently not used as current cell, the value is
        /// undetermined.
        /// </remarks>
        public int ColIndex
        {
            get
            {
                return this.currentColIndex;
            }

            set
            {
                this.currentColIndex = value;
            }
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController. Checks if the mouse is over a cell button element
        /// and if the cell is clickable.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.</returns>
        /// <remarks>Override OnHitTest in your derived cell renderer if you want to catch mouse events.
        /// </remarks>
        public int RaiseHitTest(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs, IMouseController controller)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, controller);
            }
#else
               ;
#endif

            int r = this.HitTestCellButton(rowIndex, colIndex, mouseEventArgs, controller);

            GridCellHitTestEventArgs cea = new GridCellHitTestEventArgs(rowIndex, colIndex, mouseEventArgs, controller, this.ht.CellButtonElement, r);

            this.grid.RaiseCellHitTest(cea);
            if (cea.Cancel)
            {
                return cea.Result;
            }

            r = cea.Result;

            if (r != GridHitTestContext.None)
            {
                if (r != GridHitTestContext.CellButtonElement)
                {
                    this.ht.CellButtonElement = null;
                    this.ht.CellButtonIndex = -1;
                }

                return r;
            }

            return this.OnHitTest(rowIndex, colIndex, mouseEventArgs, controller);
        }

        int HitTestCellButton(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs, IMouseController controller)
        {
            this.ht.CellButtonElement = null;
            this.ht.CellButtonIndex = -1;

            if (this.Grid.Model[rowIndex, colIndex].Clickable)
            {
                GridCellLayout cellLayout = this.PerformLayout(rowIndex, colIndex);
                for (int i = 0; i < this.Buttons.Count; i++)
                {
                    GridCellButton button = this.GetButton(i);
                    int result = button.HitTest(rowIndex, colIndex, mouseEventArgs, controller);
                    if (result != 0)
                    {
                        this.ht.Point = new Point(mouseEventArgs.X, mouseEventArgs.Y);
                        this.ht.RowIndex = rowIndex;
                        this.ht.ColIndex = colIndex;
                        this.ht.CellButtonElement = button;
                        this.ht.CellRenderer = this;
                        this.ht.CellButtonIndex = i;
                        this.ht.CellButtonBounds = button.Bounds;
                        return GridHitTestContext.CellButtonElement;
                    }
                }
            }

            return GridHitTestContext.None;
        }

        /// <summary>
        /// This method is called to determine whether the cell renderer wants to receive mouse events
        /// for the give cell at the given coordinates.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.</returns>
        /// <remarks>Override this method and return GridHitTestContext.Cell (or any other non-zero value)
        /// to force mouse messages (OnMouseDown, OnMouseHover, OnMouseUp) being called for the cell renderer.</remarks>
        protected virtual int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            return GridHitTestContext.None;
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController. Returns the cursor to be displayed at the given row or column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>returns Cursor</returns>
        public Cursor RaiseGetCursor(int rowIndex, int colIndex)
        {
            // SD8340 - Commented out the below code as the default cursor is returned always from GridCellButton and there is no option available to user to set cursor explicitly to the cell button. Doint this, will allow the button to have cursor set to that cell.

            //if (this.ht.CellButtonElement != null)
            //{
            //    return this.ht.CellButtonElement.GetCursor(this.ht);
            //}

            GridCellCursorEventArgs cea = new GridCellCursorEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, null);
            this.grid.RaiseCellCursor(cea);

            if (cea.Cancel)
            {
                return cea.Cursor;
            }

            return this.OnGetCursor(rowIndex, colIndex);
        }

        /// <summary>
        /// Override this method if you want to change the cursor for this cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="Cursor"/> to be displayed.</returns>
        protected virtual Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            return Application.UseWaitCursor ? Cursors.WaitCursor : this.Grid.Cursor != null ? this.Grid.Cursor : Cursors.Default;
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when HitTest has indicated it wants
        /// to receive mouse events and the user has moved the mouse into the cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <remarks>See also MouseHoverLeave.</remarks>
        public void RaiseMouseHoverEnter(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
               ;
#endif

            this.ht.CellButtonElement = null;
            this.ht.CellButtonIndex = -1;
            this.savedHt.CellButtonElement = null;
            this.savedHt.CellButtonIndex = -1;

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, null);
            this.grid.RaiseCellMouseHoverEnter(cea);

            if (cea.Cancel)
            {
                return;
            }

            this.OnMouseHoverEnter(rowIndex, colIndex);
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user has moved
        /// the mouse into the cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <remarks>See also OnMouseHoverLeave.</remarks>
        protected virtual void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellMouseHover"/> event and initiates a call to the virtual <see cref="OnMouseHoverEnter"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        public void RaiseMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, this.savedHt.CellButtonElement, this.savedHt.CellButtonIndex);
            }
#else
               ;
#endif

            this.HitTestCellButton(rowIndex, colIndex, e, null);

            if (this.savedHt.RowIndex != ht.RowIndex
                 || this.savedHt.ColIndex != ht.ColIndex
                 || this.savedHt.CellButtonIndex != ht.CellButtonIndex)
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e.Button, e.X, e.Y);
                }
#else
                    ;
#endif

                if (this.savedHt.CellButtonElement != null)
                {
                    this.savedHt.CellButtonElement.MouseHoverLeave(e, this.savedHt);
                }

                if (this.ht.CellButtonElement != null)
                {
                    this.ht.CellButtonElement.MouseHoverEnter(this.ht);
                }
            }

            this.savedHt = (GridCellHitTestInfo)ht.Clone();

            if (this.ht.CellButtonElement != null)
            {
                this.ht.CellButtonElement.MouseHover(e, this.ht);
                return;
            }

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, e);

            this.grid.RaiseCellMouseHover(cea);
            if (cea.Cancel)
            {
                return;
            }

            this.OnMouseHover(rowIndex, colIndex, e);
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user is moving
        /// the mouse over the cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        /// <remarks>OnMouseHover will only be called after OnMouseHoveEnter. When the user
        /// moves the mouse out of the MouseHoverLeave is called.</remarks>
        protected virtual void OnMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellMouseHoverLeave"/> event and initiates a call to the virtual <see cref="OnMouseHoverLeave"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        public void RaiseMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
               ;
#endif

            if (this.savedHt.CellButtonElement != null)
            {
                this.savedHt.CellButtonElement.Bounds = this.savedHt.CellButtonBounds;
                this.savedHt.CellButtonElement.MouseHoverLeave(e, this.savedHt);
            }

            this.ht.CellButtonElement = null;
            this.savedHt.CellButtonElement = null;

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, null);

            this.grid.RaiseCellMouseHoverLeave(cea);

            if (cea.Cancel)
            {
                return;
            }

            this.OnMouseHoverLeave(rowIndex, colIndex, e);
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user is moving
        /// the mouse out of the cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        /// <remarks>Once OnMouseHoveEnter has been called you are guaranteed to receive a OnMouseHoverLeave
        /// call either if the user moves the mouse of the cell boundaries or presses a button or if the
        /// mouse operation is canceled.</remarks>
        protected virtual void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellMouseDown"/> event and initiates a call to the virtual <see cref="OnMouseDown"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        public void RaiseMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, new Point(e.X, e.Y));
            }
#else
               ;
#endif

            if (this.ht.CellButtonElement != null)
            {
                this.ht.CellButtonElement.MouseDown(e, this.ht);
            }
            else
            {
                MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, e.X + scrolledInfo.X, e.Y + scrolledInfo.Y, e.Delta);
                GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, me);
                this.grid.RaiseCellMouseDown(cea);
                if (cea.Cancel)
                {
                    return;
                }

                this.OnMouseDown(rowIndex, colIndex, e);
            }
        }

        int mouseDownTick = 0;
        Point mouseDownPoint = Point.Empty;

        /// <summary>
        /// Gets or sets the cached Environment.TickCount of the last MouseDown event.
        /// </summary>
        protected int MouseDownTick
        {
            get
            {
                return this.mouseDownTick;
            }

            set
            {
                this.mouseDownTick = value;
            }
        }

        /// <summary>
        /// Gets or sets the cached mouse position of the last MouseDown event.
        /// </summary>
        protected Point MouseDownPoint
        {
            get
            {
                return this.mouseDownPoint;
            }

            set
            {
                this.mouseDownPoint = value;
            }
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user has pressed
        /// a mouse button.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        protected virtual void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            this.grid.CurrentCell.MoveTo(rowIndex, colIndex);

            if (e.Clicks == 2)
            {
                this.RaiseDoubleClick(rowIndex, colIndex, e);
            }
        }

        ////          void ControlDoubleClick(object sender, EventArgs e)
        ////          {
        ////               this.NotifyCurrentCellControlDoubleClick(this.Control);
        ////          }
        ////

        /// <summary>
        /// Handles the Control.MouseDown event of the attached cell control.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The MouseEventArgs with mouse position relative to top-left corner of cell.</param>
        protected void ControlMouseDown(object sender, MouseEventArgs e)
        {
            if (!this.HasFocusControl)
            {
                return;
            }
#if DEBUG

            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.X, e.Y, e.Clicks);
            }
#else

               ;
#endif

            int ticks = Environment.TickCount;
            int clicks = e.Clicks;
            if (clicks < 2 && clicks > 0)
            {
                if (this.mouseDownPoint.IsEmpty || ticks - this.mouseDownTick > SystemInformation.DoubleClickTime)
                {
                    this.mouseDownTick = Environment.TickCount;
                    this.mouseDownPoint = new Point(e.X, e.Y);
                    if (this.isGridClick)
                    {
                        //// Just in case the second mouse click goes to the parent grid ...
                        this.Grid.MouseDown += new MouseEventHandler(ControlMouseDown);
                        Timer t = new Timer();
                        t.Interval = SystemInformation.DoubleClickTime + 1;
                        t.Tick += new EventHandler(this.RemoveMouseDownTick);
                        t.Start();
                        this.isGridClick = false;
                    }
                }
                else
                {
                    if (Math.Abs(this.mouseDownPoint.X - e.X) < SystemInformation.DoubleClickSize.Width
                         && Math.Abs(this.mouseDownPoint.Y - e.Y) < SystemInformation.DoubleClickSize.Height)
                    {
                        clicks = 2;
                    }

                    this.mouseDownPoint = Point.Empty;
                    this.mouseDownTick = 0;
                }
            }

            if (clicks == 2)
            {
                this.NotifyCurrentCellControlDoubleClick(this.Control);
            }
        }

        void RemoveMouseDownTick(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            Timer t = sender as Timer;
            if (t != null)
            {
                t.Dispose();
            }

            this.Grid.MouseDown -= new MouseEventHandler(ControlMouseDown);
            this.isGridClick = false;
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellMouseMove"/> event and initiates a call to the virtual <see cref="OnMouseMove"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        public void RaiseMouseMove(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.ht.CellButtonElement != null)
            {
                this.ht.CellButtonElement.MouseMove(e, this.ht);
                return;
            }

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, e);
            this.grid.RaiseCellMouseMove(cea);
            if (cea.Cancel)
            {
                return;
            }

            this.OnMouseMove(rowIndex, colIndex, e);
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user has pressed
        /// a mouse button and is moving the mouse pointer.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        protected virtual void OnMouseMove(int rowIndex, int colIndex, MouseEventArgs e)
        {
        }

        /// <summary>
        /// RaiseMouseUp wil save the RaiseHitTest return value here. OnMouseUp will check this and
        /// only raise CellClick when no cell button was clicked.
        /// </summary>
        int mouseUpCellButtonHitTest = 0;

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellMouseUp"/> event and initiates a call to the virtual <see cref="OnMouseUp"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        public void RaiseMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            this.mouseUpCellButtonHitTest = 0;
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, new Point(e.X, e.Y));
            }
#else
               ;
#endif

            if (this.ht.CellButtonElement != null)
            {
                GridCellButton bt = this.ht.CellButtonElement;
                this.mouseUpCellButtonHitTest = RaiseHitTest(rowIndex, colIndex, e, null); // OnMouseUp will check this.mouseUpCellButtonHitTest later.
                bt.MouseUp(e, this.ht);
            }

            if (this.grid == null || this.grid.IsDisposed)
            {
                return;
            }

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, e);
            this.grid.RaiseCellMouseUp(cea);
            if (cea.Cancel)
            {
                return;
            }

            this.OnMouseUp(rowIndex, colIndex, e);
        }

        /// <summary>
        /// Checks if the specified point is over an image (see <see cref="GridStyleInfo.ImageIndex"/>) .
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="pt">The point to be tested.</param>
        /// <returns>True if inside image; False otherwise.</returns>
        protected virtual bool IsPointOverImage(int rowIndex, int colIndex, Point pt)
        {
            return false;
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the user has pressed
        /// a mouse button and is releasing the button.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data.</param>
        protected virtual void OnMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            this.clickOverImage = this.IsPointOverImage(rowIndex, colIndex, new Point(e.X, e.Y));
            if (e.Clicks < 2 && this.mouseUpCellButtonHitTest == 0)
            {
                this.RaiseClick(rowIndex, colIndex, e);
            }
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellCancelMode"/> event and initiates a call to the virtual <see cref="OnCancelMode"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public void RaiseCancelMode(int rowIndex, int colIndex)
        {
            if (this.ht.CellButtonElement != null)
            {
                this.ht.CellButtonElement.CancelMode(this.ht);
                return;
            }

            GridCellMouseEventArgs cea = new GridCellMouseEventArgs(rowIndex, colIndex, this.ht.CellButtonElement, null);
            this.grid.RaiseCellCancelMode(cea);

            this.OnCancelMode(rowIndex, colIndex);
        }

        /// <summary>
        /// This is called from GridClickCellsMouseController or GridSelectCellsMouseController when your cell renderer has indicated
        /// in its OnHitTest override that it wants to receive mouse events and the mouse operation
        /// is canceled.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        protected virtual void OnCancelMode(int rowIndex, int colIndex)
        {
        }

        void GridWindowScrolled(object sender, ScrollWindowEventArgs e)
        {
            this.scrolledInfo.X = e.XAmount;
            this.scrolledInfo.Y = e.YAmount;
        }

        internal bool clickOverImage = false;

        /// <summary>
        /// Gets or sets a value indicating whether the mouse was over an image (see <see cref="GridStyleInfo.ImageIndex"/>)
        /// in static cell when the mouse was released.
        /// </summary>
        protected bool ClickOverImage
        {
            get
            {
                return this.clickOverImage;
            }

            set
            {
                this.clickOverImage = value;
            }
        }

        ////          bool isFakeMouseDown = false;
        ////
        ////          ///// <summary>
        ////          ///// Indicates if the next MouseDown is fake.
        ////          ///// </summary>
        ////          public bool IsFakeMouseDown
        ////          {
        ////               get
        ////               {
        ////                    return isFakeMouseDown;
        ////               }
        ////               set
        ////               {
        ////                    isFakeMouseDown = value;
        ////               }
        ////          }

        bool isGridClick = false;
        bool alwaysRaiseCellClick = true;

        /// <summary>
        /// Gets or sets a value indicating whether a CellClick event should always be raised. If
        /// this property is false, it depends on <see cref="GridStyleInfo.Clickable"/>
        /// if <see cref="GridControlBase.CellClick"/> is raised.
        /// </summary>
        public bool AlwaysRaiseCellClick
        {
            get
            {
                return this.alwaysRaiseCellClick;
            }

            set
            {
                this.alwaysRaiseCellClick = value;
            }
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellClick"/> event and initiates a call to the virtual <see cref="OnClick"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        public void RaiseClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (!alwaysRaiseCellClick && !grid.Model[rowIndex, colIndex].Clickable)
            {
                return;
            }

            //// The mouse click coordinates are based on the client area of the form.
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, new Point(e.X, e.Y));
            }
#else
               ;
#endif

            MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, e.X + scrolledInfo.X, e.Y + scrolledInfo.Y, e.Delta);
            this.clickOverImage = this.IsPointOverImage(rowIndex, colIndex, new Point(e.X, e.Y));
            if (this.Grid.RaiseCellClick(rowIndex, colIndex, me, clickOverImage))
            {
                this.OnClick(rowIndex, colIndex, me);
            }

            this.isGridClick = true;
        }

        /// <summary>
        /// This is called from GridSelectCellsMouseController when the user clicked inside a cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        protected virtual void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            this.scrolledInfo = Point.Empty;
            this.Grid.WindowScrolled += new ScrollWindowEventHandler(GridWindowScrolled);
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                this.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.Click);
            }
            else
            {
                this.Grid.CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.ScrollInView);
            }

            this.Grid.WindowScrolled -= new ScrollWindowEventHandler(GridWindowScrolled);
        }

        /// <summary>
        /// Raises a <see cref="GridControlBase.CellDoubleClick"/> event and initiates a call to the virtual <see cref="OnDoubleClick"/> method.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        public void RaiseDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, new Point(e.X, e.Y));
            }
#else
               ;
#endif

            if (this.Grid.RaiseCellDoubleClick(rowIndex, colIndex, e, clickOverImage))
            {
                this.OnDoubleClick(rowIndex, colIndex, e);
            }
        }

        /// <summary>
        /// This is called from GridSelectCellsMouseController when the user double-clicks inside a cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        protected virtual void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
        }

        void ButtonClicked(object sender, GridCellEventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
               ;
#endif

            if (this.grid.RaiseCellButtonClicked(e.RowIndex, e.ColIndex, this.Buttons.IndexOf(sender), (GridCellButton)sender))
            {
                this.OnButtonClicked(e.RowIndex, e.ColIndex, this.Buttons.IndexOf(sender));
            }
        }

        /// <summary>
        /// This method is called when the user clicks a cell button inside cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="button">The button</param>
        /// <remarks>In your overriden version
        /// of this method, you can activate the current cell for the given row and column index and then
        /// drop-down a list.</remarks>
        protected virtual void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            if (this.dropDownImp != null)
            {
                this.dropDownImp.OnButtonClicked(rowIndex, colIndex, button);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IGridDropDownCellImp"/> that provides drop-down logic for this renderer.
        /// </summary>
        public IGridDropDownCellImp DropDownPart
        {
            get
            {
                return this.dropDownImp;
            }

            set
            {
                this.dropDownImp = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop-down button.
        /// </summary>
        public GridCellButton DropDownButton
        {
            get
            {
                if (this.dropDownImp == null)
                {
                    throw new InvalidOperationException("dropDownImp is null");
                }

                return this.dropDownImp.DropDownButton;
            }

            set
            {
                if (this.dropDownImp == null)
                {
                    throw new InvalidOperationException("dropDownImp is null");
                }

                this.dropDownImp.DropDownButton = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="GridDropDownContainer"/> and associates it with
        /// the cell's parent grid.
        /// </summary>
        /// <returns>The container where you can insert child controls to be displayed as drop-down part for your cell.</returns>
        protected virtual IGridDropDownContainer CreateDropDownContainer()
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return this.dropDownImp.CreateDropDownContainer();
        }

        internal IGridDropDownContainer IntCreateDropDownContainer()
        {
            return this.CreateDropDownContainer();
        }

        /// <summary>
        /// Ensures the container is valid and initialized.
        /// </summary>
        public void EnsureDropDownContainer()
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.EnsureDropDownContainer();
        }

        /// <summary>
        /// Called to initialize contents of the drop-down container for the first time.
        /// </summary>
        protected virtual void InitializeDropDownContainer()
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.InitializeDropDownContainer();
        }

        internal void IntInitializeDropDownContainer()
        {
            this.InitializeDropDownContainer();
        }

        /// <summary>
        /// Will be called to indicate that the popup child was closed in the specified mode.
        /// </summary>
        /// <param name="childUI">The child that was closed.</param>
        /// <param name="popupCloseType">A <see cref="PopupCloseType"/> value.</param>
        public virtual void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.ChildClosing(childUI, popupCloseType);
        }

        /// <summary>
        /// Will be called to indicate that the popup child was closed.
        /// </summary>
        /// <param name="sender">The child that was closed.</param>
        /// <param name="e">The event data with a <see cref="PopupClosedEventArgs.PopupCloseType"/> value.</param>
        public virtual void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.DropDownContainerCloseDropDown(sender, e);
        }

        Point IPopupParent.GetLocationForPopupAlignment(PopupRelativeAlignment prevAlign, out PopupRelativeAlignment newAlign)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return ((IPopupParent)this.dropDownImp).GetLocationForPopupAlignment(prevAlign, out newAlign);
        }

        Point[] IPopupParent.GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return ((IPopupParent)this.dropDownImp).GetBorderOverlapCue(relativeAlignment);
        }

        bool IPopupParent.IsRightToLeft
        {
            get
            {
                return this.grid.IsRightToLeft();
            }
        }

        /// <summary>
        /// Occurs after the popup has been dropped-down and made visible.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.DropDownContainerShowedDropDown(sender, e);
        }

        /// <summary>
        /// Occurs when the drop-down container is about to be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.DropDownContainerShowingDropDown(sender, e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowingDropDown"/> for the parent grid.
        /// </summary>
        /// <param name="size">The suggested size of the drop-down.</param>
        /// <returns>True if drop-down should be shown; False if operation should be canceled.</returns>
        protected bool NotifyShowingDropDown(ref Size size)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return this.dropDownImp.NotifyShowingDropDown(ref size);
        }

        /// <summary>
        /// Gets the container where you can insert child controls to be displayed as drop-down part for your cell.
        /// </summary>
        public IGridDropDownContainer DropDownContainer
        {
            get
            {
                if (this.dropDownImp != null)
                {
                    return this.dropDownImp.PopupControlContainer;
                }

                return null;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.OnCurrentCellShowedDropDown"/> for the parent grid.
        /// </summary>
        protected void NotifyShowedDropDown()
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            this.dropDownImp.NotifyShowedDropDown();
        }

        Control IPopupItem.GetPopupParentControl()
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return ((IPopupItem)this.dropDownImp).GetPopupParentControl();
        }

        /// <summary>
        /// Called to find out whether a specified control is part of the popup hierarchy.
        /// </summary>
        /// <param name="control">A control instance. </param>
        /// <param name="askPopupParent">True indicates this query should be passed to the IPopupParent, if any; False indicates you should not query the popup parent.</param>
        /// <returns>True if the control is part of the popup hierarchy; False if not.</returns>
        /// <seealso cref="IPopupItem"/>
        public virtual bool IsRelatedControl(Control control, bool askPopupParent)
        {
            if (this.dropDownImp == null)
            {
                throw new InvalidOperationException("dropDownImp is null");
            }

            return this.dropDownImp.IsRelatedControl(control, askPopupParent);
        }

        /// <summary>
        /// Called from GridCurrentCell.ScrollInView to determine if cell should be scrolled into
        /// view when GridCurrentCell.ScrollInView is called.
        /// </summary>
        /// <param name="reason">The reason for scrolling current cell into view.</param>
        /// <returns>True if cell supports scrolling into view.</returns>
        public virtual bool OnScrollInView(GridScrollCurrentCellReason reason)
        {
            return true;
        }

        /// <summary>
        /// Returns a nested current cell if this cell type hosts a GridControl by itself (for
        /// example GridNestedTableControlCellRenderer with a GridGroupingControl overrides
        /// this method).
        /// </summary>
        /// <returns>A nested current cell object.</returns>
        public virtual GridCurrentCell GetNestedCurrentCell()
        {
            return this.CurrentCell;
        }

        /// <summary>
        /// Called to determine whether the cell needs to be repainted when it becomes
        /// the active current cell.
        /// </summary>
        /// <returns>True if cell needs to be repainted; False otherwise.</returns>
        public virtual bool ShouldRefreshCurrentCell()
        {
            return true;
        }
    }
}
