//-------------------------------------------------------------------------------------------------
// <copyright file="GridOriginalTextBoxControl.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the text box that is shown in a <see cref="GridTextBoxCellRenderer"/> when the
    /// user starts editing the cell.
    /// </summary>
    [ToolboxItem(false)]
#if RICHTEXT
    public class GridTextBoxControl
        : RichTextBox, IGridTextBoxControl, ISupportsPopupControlContainer
#else
    public class GridOriginalTextBoxControl
        : TextBox, IGridTextBoxControl, ISupportsPopupControlContainer
#endif
    {
        // Fields
#if RICHTEXT
        private Size textSize = new Size();
#endif
        private bool floatDone = false;
        private GridTextBoxCellRenderer parent;
        internal bool inInit;
        internal readonly GridMargins textBoxMargins = new GridMargins(2, 0, 1, 0);
        IGridDropDownContainer dropDownContainer = null;

        GridMargins IGridTextBoxControl.TextBoxMargins
        {
            get
            {
                return this.textBoxMargins;
            }
        }

#if RICHTEXT
        string oldText = "";

        /// <override/>
        public override string Text
        {
            get
            {
                if (inImeComposition)
                    return oldText;
                oldText = base.Text;
                //    Trace.WriteLine(oldText);
                return oldText;
            }
            set
            {
                oldText = value;
                //    Trace.WriteLine(oldText);
                base.Text = value;
            }
        }

        /// <override/>
        public override string SelectedText
        {
            get
            {
                if (inImeComposition)
                    return oldText;
                return base.SelectedText;
            }
            set
            {
                oldText = value;
                base.SelectedText = value;
            }
        }

        /// <override/>
        protected override void Select(bool directed, bool forward)
        {
            if (inImeComposition)
                return;

            base.Select (directed, forward);
        }

#endif
        // Methods

        /// <summary>
        /// Initializes a new <see cref="GridTextBoxControl"/> and attaches it to a <see cref="GridTextBoxCellRenderer"/>
        /// </summary>
        /// <param name="parent">The <see cref="GridTextBoxCellRenderer"/> that the cell belongs to.</param>
#if RICHTEXT
        public GridTextBoxControl(GridTextBoxCellRenderer parent)
#else

        public GridOriginalTextBoxControl(GridTextBoxCellRenderer parent)
#endif
        {
            this.parent = parent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CausesValidation = false;
            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.StandardClick|ControlStyles.StandardDoubleClick, true);
        }

        private const int SES_ALLOWBEEPS = 256; // 0x0100
        private const int SES_BEEPONMAXTEXT = 2; // 0x0002

        /// <override/>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.ExStyle &= ~(SES_ALLOWBEEPS|SES_BEEPONMAXTEXT);
                return cp;
            }
        }

        /// <override/>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            // The implementation of the RichEdit control in the FCL is flawed. The FCL RichEdit gives out a COM interface
            // to the rich edit DLL via a windows message for drag-drop & clipboard support; unfortunately, this interface is not
            // released properly. This, in turn, causes the RichEdit and any associated controls to leak.
#if RICHTEXT
            System.Reflection.FieldInfo callbackField = typeof(RichTextBox).GetField("oleCallback", System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            if(callbackField != null)
            {
                object callback = callbackField.GetValue(this);
                if(callback != null)
                {
                    IntPtr p = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(callback);
                    while(System.Runtime.InteropServices.Marshal.Release(p) > 0)
                        ;
                }
            }
#endif
            base.OnHandleDestroyed(e);
        }

        int suspendEvents = 0;

        /// <summary>
        /// Suspends raising events.
        /// </summary>
        public void SuspendEvents()
        {
            this.suspendEvents++;
        }

        /// <summary>
        /// Resumes raising events.
        /// </summary>
        public void ResumeEvents()
        {
            if (this.suspendEvents > 0)
            {
                this.suspendEvents--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the raising events is temporarily disabled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSuspendEvents
        {
            get
            {
                return this.suspendEvents > 0;
            }
        }

#if RICHTEXT
        bool raiseTextChangeWhenImeEnd = false;
#endif
        /// <override/>
        protected override void OnTextChanged(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(Text, IsSuspendEvents);
#if RICHTEXT
            if (this.inImeComposition)
            {
                raiseTextChangeWhenImeEnd = true;
                return;
            }
#endif
            if (this.suspendEvents <= 0)
            {
                base.OnTextChanged(e);
            }
        }

        /// <override/>
        protected override void OnGotFocus(EventArgs e)
        {
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN", Text, Parent);
            }
#else
            ;
#endif

            if (ContextMenu == null)
            {
                if (Parent.ContextMenu != null)
                {
                    ContextMenu = Parent.ContextMenu;
                }
                else
                {
                    ContextMenu = new ContextMenu();
                }
            }

            base.OnGotFocus(e);
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END", Text, Parent);
            }
#else
            ;
#endif
        }

        /// <override/>
        protected override void OnLostFocus(EventArgs e)
        {
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN", Text, Parent);
            }
#else
            ;
#endif

            base.OnLostFocus(e);
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END", Text, Parent);
            }
#else
            ;
#endif
        }

        /// <override/>
        protected override void OnEnter(EventArgs e)
        {
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN", Text, Parent);
            }
#else
            ;
#endif

            base.OnEnter(e);
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END", Text, Parent);
            }
#else
            ;
#endif
        }

        /// <override/>
        protected override void OnLeave(EventArgs e)
        {
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("BEGIN", Text, Parent);
            }
#else
            ;
#endif

            base.OnLeave(e);
#if DEBUG
            if (Switches.GridFocus.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("END", Text, Parent);
            }
#else
            ;
#endif
        }
#if RICHTEXT
        bool inImeComposition = false;

        /// <summary>
        /// Returns True after WM_IME_STARTCOMPOSITION was sent and False once WM_IME_ENDCOMPOSITION was handled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InImeComposition
        {
            get
            {
                return inImeComposition ;
            }
        }

#endif

#if RICHTEXT
        bool firstTimeIME_ENDCOMPOSITION = true;
#endif

        /// <override/>
        /// <summary>
        /// Preprocesses the keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <param name="msg">Message that has to be dispatched.</param>
        /// <returns>True if the messages can be preprocessed.</returns>
        public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlPreProcessMessage(ref msg))
            {
                return true;
            }

            return base.PreProcessMessage(ref msg);
        }

        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        protected override void WndProc(ref Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlWndProc(ref msg))
            {
                return;
            }

            ////if (msg.Msg >= NativeMethods.WM_KEYFIRST && msg.Msg <= NativeMethods.WM_KEYLAST)
            //    TraceUtil.TraceCurrentMethodInfo(this.ParentCell.CurrentCell.ToString(), msg.ToString());
            try
            {
                switch (msg.Msg)
                {
                    case NativeMethods.WM_PASTE:
                        if (!ignoreWmPaste)
                        {
                            this.ParentCell.Grid.Model.CutPaste.Paste();
                        }

                        return;
#if RICHTEXT
                    case NativeMethods.WM_IME_COMPOSITION:
                        // See also comments in FloatDone property.
                        break;
                    case NativeMethods.WM_IME_ENDCOMPOSITION:
                        inImeComposition = false;

                        // firstTimeIME_ENDCOMPOSITION: Workaround for qa issue 14576 - The very first time you enter
                        // a1 into Chinese (Simplified) IME (MS PingYng 98) it did not get saved
                        // when the user leaves cell.
                        //if (firstTimeIME_ENDCOMPOSITION)
                        //{
                        //    if (!this.ParentCell.CurrentCell.IsModified)
                        //    {
                        //        if (this.ParentCell.RaiseNotifyCurrentCellChanging())
                        //        {
                        //            this.ParentCell.CurrentCell.IsModified = true;
                        //            this.ParentCell.RaiseNotifyCurrentCellChanged();
                        //        }
                        //    }
                        //    firstTimeIME_ENDCOMPOSITION = false;
                        //}
                        // raiseTextChangeWhenImeEnd Workaround for qa issue 1388, incident 23111. This also works
                        // for issue 14576 mentioned before. Therefore I commented out above workaround.
                        if (/*firstTimeIME_ENDCOMPOSITION ||*/ raiseTextChangeWhenImeEnd)
                        {
                            BeginInvoke(new EventHandler(delayTextChange), new object[] { this, null });
                            raiseTextChangeWhenImeEnd = false;
                            firstTimeIME_ENDCOMPOSITION = false;
                        }
                        break;
                    case NativeMethods.WM_IME_STARTCOMPOSITION:
                        inImeComposition = true;
                        break;
#endif

                    case NativeMethods.WM_KEYDOWN:
                        this.ParentCell.ignoreWmChar = false;
                        break;
                    case NativeMethods.WM_KEYUP:
                        {
                            Keys keyCode = (Keys)((int)msg.WParam) & Keys.KeyCode;

                            if ((int)keyCode == 0x0012 /*Alt Key*/)
                            {
                                this.ParentCell.ignoreWmChar = false;
                            }
                            break;
                        }
                    case NativeMethods.WM_CHAR:
                        if (!ParentCell.HasFocusControl || this.ParentCell.ignoreWmChar)
                        {
                            return;
                        }

                        break;

                    case 0x128:
                        msg.Result = IntPtr.Zero;
                        return;

                    case 0x129:
                        // Avoid Control.ProcessUICues being called from Control.PreProcessMessage
                        ////#define UISF_HIDEFOCUS                  0x1
                        ////#define UISF_HIDEACCEL                  0x2
                        msg.Result = (IntPtr) 2;
                        return;
                }
                
                if (msg.Msg == NativeMethods.WM_KEYDOWN)
                {                    
                    Keys keyCode = (Keys)((int)msg.WParam) & Keys.KeyCode;
                    if (this.parent != null&&keyCode==Keys.ControlKey)
                    {
                        this.parent.Grid.EnableForwardCurrentCellControlKeyMessages(false);
                        this.parent.Grid.ProcessMessage(ref msg);
                        this.parent.Grid.EnableForwardCurrentCellControlKeyMessages(true);
                    }
                }

                base.WndProc(ref msg);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                if (this.parent != null)
                {
                    this.parent.Grid.NotifyCancelMode();
                }
            }
        }

#if RICHTEXT
        void delayTextChange(object sender, EventArgs e)
        {
            //TraceUtil.TraceCurrentMethodInfo(oldText);
            OnTextChanged(EventArgs.Empty);
        }
#endif
#if !RICHTEXT && !SyncfusionFramework2_0
        /// <summary>
        ///   <para>Gets the line number from the specified character
        /// position within the text of the <see cref="TextBox" />
        /// control.</para>
        /// </summary>
        /// <param name="index">The character index position to search.</param>
        /// <returns>
        ///   <para> The zero-based line number where the character index is located in.</para>
        /// </returns>
        public int GetLineFromCharIndex(int index)
        {
            int line = (int)(NativeMethods.SendMessage(Handle, NativeMethods.EM_LINEFROMCHAR, index, 0));
            return line;
        } // end of method GetLineFromCharIndex
#endif

        /// <override/>
        /// <returns> A value indicating whether the key data can be allowed </returns>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Tab:
                    return this.ParentCell.Grid.WantTabKey;
                case Keys.Escape:
                    return this.ParentCell.Grid.WantEscapeKey;
                case Keys.Enter:
                    return this.ParentCell.Grid.WantEnterKey;
                case Keys.V:
                    this.ignoreWmPaste = true;
                    ////TraceUtil.TraceCurrentMethodInfo(keyData.ToString());
                    return false;
            }

            return base.IsInputKey(keyData);
        }

        bool ignoreWmPaste = false;

        /// <override/>
        /// <returns> returns the message of the processed key </returns>
        protected override bool ProcessKeyMessage(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_KEYUP)
            {
                this.ignoreWmPaste = false;
            }
#if DEBUG

            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(m.ToString());
            }
#else

            ;
#endif
#if RICHTEXT
            if (this.inImeComposition)
            {
                // When IME window is active do not let grid handle keystrokes. Instead let
                // the IME window handle them. Fixes issue issue 1388 (incident 23111) and
                // probably also related to incident 14576
                return false;
            }
#endif

            ////TraceUtil.TraceCurrentMethodInfo(m.ToString());
            GridTextBoxCellRenderer tbr = this.ParentCell as GridTextBoxCellRenderer;
            if (tbr == null)
            {
                return base.ProcessKeyMessage(ref m);
            }

            if (tbr.DisableTextBox)
            {
                return false;
            }

            Rectangle intersect = Rectangle.Intersect(this.Bounds, this.parent.Grid.GridBounds);

            Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;
            if (m.Msg == 0x102/*WM_CHAR*/ || (tbr.Grid.HandleWMSYSCHAR && m.Msg == 0x104 /*WM_SYSCHAR*/))
            {
                if ((this.parent.Grid.WantEnterKey && keyCode == Keys.Enter)
                    || (this.parent.Grid.WantEscapeKey && keyCode == Keys.Escape))
                {
                    return true;
                }

                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }

                bool b = this.ParentCell.RaiseProcessKeyEventArgs(ref m)
                    || this.ProcessKeyEventArgs(ref m);
                ////return this.ProcessKeyEventArgs(ref m);
                return b;
            }

            bool bCtl = (Control.ModifierKeys & Keys.Control) != Keys.None;
            bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;
            bool bShift = (Control.ModifierKeys & Keys.Shift) != Keys.None;

            bool forwardParent = false;
            bool scrollInView = false;
            bool callBase = true;

            Keys kc = keyCode;
            if (RightToLeft == RightToLeft.Yes)
            {
                switch (kc)
                {
                    case Keys.Left:
                        kc = Keys.Right;
                        break;
                    case Keys.Right:
                        kc = Keys.Left;
                        break;
                }
            }

            switch (kc)
            {
                case Keys.Home:
                case Keys.End:
                    if (bCtl)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;

                case Keys.Left:
                    if (bCtl || (this.SelectionStart <= 0 && (this.SelectionLength == 0 || !bShift)))
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Right:
                    if (bCtl || this.SelectionStart + this.SelectionLength >= this.Text.Length)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Up:
                    if (bCtl || this.GetLineFromCharIndex(SelectionStart) <= 0)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Down:
                    if (bCtl || this.GetLineFromCharIndex(this.SelectionStart + this.SelectionLength) >= this.GetLineFromCharIndex(this.Text.Length))
                    {
                        callBase = !bAlt;
                        forwardParent = !bAlt;
                    }
                    else
                    {
                        scrollInView = true;
                        callBase = false;
                    }

                    break;
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Tab:
                case Keys.F2:
                    forwardParent = true;
                    callBase = false;
                    break;

                case Keys.Escape:
                    if (this.parent.Grid.WantEscapeKey)
                    {
                        forwardParent = true;
                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.Enter:
                    // FYI - In the following line, we define the behavior when the
                    // user presses <CTRL>-<Enter>. Right now, the Enter will insert a newline
                    // if <CTRL> is pressed.
                    if (this.parent.Grid.WantEnterKey)
                    {
                        if (!this.parent.StyleInfo.AllowEnter && !bCtl)
                        {
                            forwardParent = true;
                        }

                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.V:
                    scrollInView = bCtl;
                    forwardParent = bCtl || bAlt;
                    callBase = false;
                    break;

                case Keys.X:
                case Keys.C:
                    scrollInView = bCtl;
                    forwardParent = false;
                    callBase = true;
                    break;

                case Keys.Insert:
                    scrollInView = true;
                    forwardParent = ((bCtl || bShift) && this.SelectionLength == 0) || bAlt; // || bShift;
                    callBase = false;
                    break;

#if !RICHTEXT
                case Keys.Back:
                    if (this.ParentCell.Grid.IsWindowless)
                    {
                        return true;
                    }

                    scrollInView = true;
                    forwardParent = (bCtl || bAlt || bShift) && this.SelectionLength == 0;
                    callBase = false;
                    break;
#endif

                case Keys.Delete:

                    scrollInView = true;
                    forwardParent = (bCtl || bAlt || bShift) && this.SelectionLength == 0;
                    callBase = false;
                    break;

                case Keys.F4:
                    if (bCtl || bAlt)
                    {
                        return base.ProcessKeyMessage(ref m);
                    }

                    callBase = false;
                    break;

                case Keys.ShiftKey:
                case Keys.ControlKey:
                    forwardParent = true;
                    callBase = false;
                    break;

                default:
                    forwardParent = bCtl || bAlt;
                    break;
            }

            // Give programmers a chance to modify the default behavior of ProcessKeyMessage
            // without subclassing this control.
            GridCurrentCellControlKeyMessageEventArgs e = new GridCurrentCellControlKeyMessageEventArgs(this, m, scrollInView, forwardParent, callBase, true);
            this.parent.Grid.RaiseCurrentCellControlKeyMessage(e);
            if (e.Handled)
            {
                return e.Result;
            }

            scrollInView = e.ScrollInView;
            forwardParent = e.CallProcessKeyPreview;
            callBase = e.CallBaseProcessKeyMessage;

            if (scrollInView)
            {
                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }
            }

            if (forwardParent)
            {
                // This will trigger Grid.OnKeyDown and Grid.OnKeyUp events. CurrentCellKeyDown and CurrentCellKeyPress events might also
                // be triggered from Grids ProcessKeyPreview method.
                this.ProcessKeyPreview(ref m);
                return true;
            }
            else
            {
                // Call to ParentCell.ProcessKeyEventArgs will trigger CurrentCellKeyDown and CurrentCellKeyPress events but no
                // Grid.OnKeyDown and Grid.OnKeyUp events.
                if (callBase)
                {
                    return base.ProcessKeyMessage(ref m);
                }
                else
                {
                    // Call to ParentCell.ProcessKeyEventArgs will trigger CurrentCellKeyDown and CurrentCellKeyPress events but no
                    // Grid.OnKeyDown and Grid.OnKeyUp events.
                    bool b = this.ParentCell.RaiseProcessKeyEventArgs(ref m)
                        || this.ProcessKeyEventArgs(ref m);

                    return b;
                }
            }
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, e.Modifiers);
            }
#else
            ;
#endif

            ////            this.ParentCell.Grid.OnCurrentCellKeyDown(e);
            base.OnKeyDown(e);
        }

        /// <override/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, e.Modifiers);
            }
#else
            ;
#endif

            ////            this.ParentCell.Grid.OnCurrentCellKeyUp(e);
            base.OnKeyUp(e);
        }

        /// <override/>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (this.ParentCell != null)
            {
                string s = Text;
#if DEBUG
                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyChar);
                }
#else
                ;
#endif

                ////            this.ParentCell.Grid.OnCurrentCellKeyPress(e);//            if (!e.Handled)
                ////                this.ParentCell.RaiseKeyPress(e);
                if (!e.Handled)
                {
                    string t = string.Empty;
                    if (this.SelectionLength == -1)
                    {
                        t = e.KeyChar.ToString();
                    }
                    else
                    {
                        if (this.SelectionStart > 0)
                        {
                            t = s.Substring(0, this.SelectionStart);
                        }

                        t = t + e.KeyChar.ToString();
                        if (this.SelectionStart + this.SelectionLength < this.Text.Length)
                        {
                            t = t + s.Substring(this.SelectionStart + this.SelectionLength);
                        }
                    }

                    if (!ParentCell.ValidateString(t))
                    {
                        e.Handled = true;
                    }
                }
            }

            base.OnKeyPress(e);
        }

        /// <override/>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
#if DEBUG
            if (Switches.TextBoxCellEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(pevent.ClipRectangle);
            }
#else
            ;
#endif

            base.OnPaintBackground(pevent);
        }

        /// <summary>
        /// Scroll the specified number of lines.
        /// </summary>
        /// <param name="count">The number of lines to scroll.</param>
        /// <remarks>
        /// Sends an EM_LINESCROLL message to the control.
        /// </remarks>
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
        public void LineScroll(int count)
        {
            NativeMethods.SendMessage(Handle, 182/*EM_LINESCROLL*/, 0, count);
        }

#if RICHTEXT
        /// <override/>
        protected override void OnContentsResized(ContentsResizedEventArgs e)
        {
            base.OnContentsResized(e);
            if (parent != null)
            {
                textSize.Height = e.NewRectangle.Height;//this.GetLineCount();//
                textSize.Width = e.NewRectangle.Width;
#if DEBUG

                if (Switches.TextBoxCellEvents.TraceVerbose)

                    TraceUtil.TraceCurrentMethodInfo(e.NewRectangle, textSize);
#else

                ;
#endif

                base.OnContentsResized(e);

                if (!(Visible && Focused && Floatable))
                    return;

                Size size = this.parent.Grid.Model.AddBorders(textSize, this.parent.StyleInfo);

                GridRangeInfo rangeCell = GridRangeInfo.Cell(this.parent.RowIndex, this.parent.ColIndex);

                this.parent.Grid.Model.FloatingCells.DelayFloatCells(rangeCell);
                if (this.parent.Grid.Model.FloatingCells.EvaluateFloatingCells(rangeCell))
                {
                    this.floatDone = true;
                }
                else if (this.parent.StyleInfo.AutoSize && parent.Grid.AllowTextBoxAutoSize)
                {
                    this.parent.Grid.Model.GetSpannedRangeInfo(this.parent.RowIndex, this.parent.ColIndex, out rangeCell);
                    bool bChanged = false;
                    if (textSize.Height > ClientRectangle.Height)
                    {
                        int nOldHeight = this.parent.Grid.GetRowHeight(rangeCell.Bottom);
                        this.parent.Grid.Model.RowHeights.ResizeToFit(rangeCell, GridResizeToFitOptions.NoShrinkSize|GridResizeToFitOptions.ResizeCoveredCells);
                        bChanged = this.parent.Grid.GetRowHeight(rangeCell.Bottom) != nOldHeight;
                        Update();
                        LineScroll(-1);
                        LineScroll(-1);
                    }

                    if (textSize.Width > ClientRectangle.Width)
                    {
                        int nOldWidth = this.parent.Grid.GetColWidth(rangeCell.Right);
                        this.parent.Grid.Model.ColWidths.ResizeToFit(rangeCell, GridResizeToFitOptions.NoShrinkSize|GridResizeToFitOptions.ResizeCoveredCells);
                        bChanged = this.parent.Grid.GetColWidth(rangeCell.Right) != nOldWidth;
                        //        .SetColWidth(this.parent.currentColIndex, this.parent.currentColIndex, size.Width, null);
                    }

                }
            }
        }
#endif

        /// <summary>
        /// Suspends raising Modified events until EndInit is called and set <see cref="Initializing"/> property to
        /// true. You should check <see cref="Initializing"/> your cell renderer's implementation to see
        /// if changes in the text box are done because of initialization or user interaction.
        /// </summary>
        public void BeginInit()
        {
            if (this.inInit)
            {
                throw new System.Exception("BeginInit called twice.");
            }

            this.inInit = true;
            this.SuspendEvents();
        }

        /// <summary>
        /// Resume raising modified events and resets the <see cref="Initializing"/> property.
        /// </summary>
        public void EndInit()
        {
            this.ResumeEvents();
            this.inInit = false;
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BeginInit"/> was called.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initializing
        {
            get
            {
                return this.inInit;
            }
        }

        /// <override/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (this.parent != null)
            {
                this.parent.Grid.GetGridWindow().ProcessMouseWheel(e);
            }
        }

        // Properties

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellRendererBase ParentCell
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the cell was floated over another cell after the
        /// user inserted text. Will be reset when the cell is redrawn.
        /// </summary>
        public bool FloatDone
        {
            get
            {
#if RICHTEXT
                // A bit here for supporting chinese and other languages. TextBoxCellRenderer.OnDraw
                // checks FloatDone property. If it returns true the text box will be left untouched. This
                // is important because if the TextBoxCellRenderer.OnDraw would set .Text or call .SelectAll
                // or other methods a WM_IME_ENDCOMPOSITION would be triggered and changes to the cell
                // will be lost.
                return floatDone || this.inImeComposition;
#else
                return this.floatDone;
#endif
            }

            set
            {
                this.floatDone = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the cell supports floating over another cell.
        /// </summary>
        public virtual bool Floatable
        {
            get
            {
                GridStaticCellModel model = this.ParentCell.Model as GridStaticCellModel;
                return model != null && model.AllowFloating;
            }

            set
            {
            }
        }

        // Properties

        /// <summary>
        /// Gets or sets a value indicating whether the dropped-down state or shows or hides the drop-down window.
        /// </summary>
        public bool DroppedDown
        {
            get
            {
                return this.PopupControlContainer != null && this.PopupControlContainer.IsShowing();
            }

            set
            {
                if (this.PopupControlContainer == null)
                {
                    return;
                }

                if (value)
                {
                    this.PopupControlContainer.ShowPopup(Point.Empty);
                }
                else
                {
                    this.PopupControlContainer.HidePopup(PopupCloseType.Canceled);
                }
            }
        }

        IGridDropDownContainer ISupportsPopupControlContainer.PopupControlContainer
        {
            get
            {
                return this.PopupControlContainer;
            }

            set
            {
                this.PopupControlContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the container this child control is associated with.
        /// </summary>
        protected virtual IGridDropDownContainer PopupControlContainer
        {
            get
            {
                return this.dropDownContainer;
            }

            set
            {
                this.dropDownContainer = value;
                if (this.dropDownContainer != null)
                {
                    this.dropDownContainer.ParentControl = this;
                }
            }
        }
    }
}
