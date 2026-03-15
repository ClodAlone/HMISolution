//-------------------------------------------------------------------------------------------------
// <copyright file="GridFindReplaceDialog.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A find and replace dialog that can be used to search cells within a grid. The dialog
    /// communicates with the grid through the <see cref="IGridFindReplaceDialogSink"/> interface.
    /// </summary>
    /// <remarks>
    /// See the  <see cref="GridFindReplaceDialogSink"/> for an example of how to enable find and
    /// replace support for a grid.
    /// <para/>
    /// If you want to find and replace in the grid without displaying this dialog,
    /// you should instantiate a <see cref="GridFindReplaceDialogSink"/> object and call its
    /// <see cref="GridFindReplaceDialogSink.Find"/> or <see cref="GridFindReplaceDialogSink.Replace"/> \
    /// method. <para/>
    /// </remarks>
    public class GridFindReplaceDialog
        : Form
    {
        private IGridFindReplaceDialogSink editView;
        private object lcrFoundOld;
        private int editOriginalCloseTop;
        private int offsetY;
        ////        private bool bFound = false;
        private bool inFind = false;
        private System.Windows.Forms.Label labelFindWhat;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.CheckBox checkBoxMatchWholeCell;
        private System.Windows.Forms.CheckBox checkBoxSearchUp;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.Button buttonReplaceAll;
        private System.Windows.Forms.Label labelReplaceWith;
        private System.Windows.Forms.ComboBox comboBoxFindWhat;
        private System.Windows.Forms.ComboBox comboBoxReplaceWith;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ComboBox comboBoxSelection;
        private System.ComponentModel.IContainer components;

        const int indexWholeTable = 0;
        const int indexColumnOnly = 1;
        const int indexSelectionOnly = 2;

        // TODO: Add support for localization
        string strNotFound = "The specified text was not found.";
        string strCategory = "Search Results";
        string strReachedStart = "Find reached the starting point of the search.";

        [ThreadStaticAttribute]
        static GridFindReplaceDialog dialog = null;

        /// <summary>
        /// Gets an instance of the find replace dialog. Creates a single
        /// shared instance if necessary.
        /// </summary>
        public static GridFindReplaceDialog Instance
        {
            get
            {
                if (dialog == null)
                {
                    dialog = new GridFindReplaceDialog();
                }

                return dialog;
            }
        }

        /// <summary>
        /// Disallow explicit construction of this dialog. Use <see cref="Instance"/>
        /// instead.
        /// </summary>
        GridFindReplaceDialog()
        {
            InitializeComponent();

            editOriginalCloseTop = buttonClose.Top;
            offsetY = editOriginalCloseTop - buttonReplaceAll.Top;
            this.comboBoxSelection.SelectedIndex = indexColumnOnly;
            ApplyOptions();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the text to be searched.
        /// </summary>
        public string TextSearched
        {
            get
            {
                return comboBoxFindWhat.Text;
            }

            set
            {
                comboBoxFindWhat.Text = value;
            }
        }

        GridFindReplaceDialogOptions options = GridFindReplaceDialogOptions.All;

        /// <summary>
        /// Gets or sets show or hide various dialog elements.
        /// </summary>
        [DefaultValue(GridFindReplaceDialogOptions.All)]
        public GridFindReplaceDialogOptions Options
        {
            get
            {
                return this.options;
            }

            set
            {
                if (this.options != value)
                {
                    this.options = value;
                    OnOptionsChanged(EventArgs.Empty);
                    ApplyOptions();
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="Options"/> property has changed.
        /// </summary>
        public event EventHandler OptionsChanged;

        /// <summary>
        /// Raises the <see cref="OptionsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnOptionsChanged(EventArgs e)
        {
            if (OptionsChanged != null)
            {
                OptionsChanged(this, e);
            }
        }

        /// <summary>
        /// Register the current target of find and replace operations. Call this
        /// from your controls <see cref="Control.Enter"/> event handler.
        /// </summary>
        /// <param name="sink">The new target for find and replace operations.</param>
        /// <remarks>
        /// The method checks if there is an active dialog displayed. Otherwise,
        /// the method will have no effect.
        /// </remarks>
        public static void SetActiveSinkIfVisible(IGridFindReplaceDialogSink sink)
        {
            if (dialog != null)
            {
                dialog.ActiveSink = sink;
            }
        }

        /// <summary>
        /// Gets or sets current selection in your current find and replace target control. For a 
        /// grid, this should be a GridRangeInfo.
        /// </summary>
        public static void ResetFindLocation()
        {
            if (dialog != null && !dialog.inFind)
            {
                dialog.lcrFoundOld = null;
            }
        }

        /// <summary>
        /// Gets or sets the current target of find and replace operations.
        /// </summary>
        public IGridFindReplaceDialogSink ActiveSink
        {
            get
            {
                return editView;
            }

            set
            {
                editView = value;
            }
        }

        /// <summary>
        /// Adds the string to the search list of the find what combo box.
        /// </summary>
        /// <param name="str">The search string to be added.</param>
        public void AddToSearchedList(string str)
        {
            if (!comboBoxFindWhat.Items.Contains(comboBoxFindWhat.Text))
            {
                comboBoxFindWhat.Items.Insert(0, comboBoxFindWhat.Text);
            }
        }

        /// <override/>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            dialog = null;
            base.OnHandleDestroyed(e);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GridFindReplaceDialog));
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.labelReplaceWith = new System.Windows.Forms.Label();
            this.checkBoxSearchUp = new System.Windows.Forms.CheckBox();
            this.comboBoxReplaceWith = new System.Windows.Forms.ComboBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.labelFindWhat = new System.Windows.Forms.Label();
            this.comboBoxFindWhat = new System.Windows.Forms.ComboBox();
            this.checkBoxMatchWholeCell = new System.Windows.Forms.CheckBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.comboBoxSelection = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReplaceAll.Location = new System.Drawing.Point(352, 64);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.TabIndex = 16;
            this.buttonReplaceAll.Text = "Replace &All";
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // labelReplaceWith
            // 
            this.labelReplaceWith.Location = new System.Drawing.Point(8, 36);
            this.labelReplaceWith.Name = "labelReplaceWith";
            this.labelReplaceWith.Size = new System.Drawing.Size(84, 14);
            this.labelReplaceWith.TabIndex = 3;
            this.labelReplaceWith.Text = "Re&place with:";
            // 
            // checkBoxSearchUp
            // 
            this.checkBoxSearchUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxSearchUp.Location = new System.Drawing.Point(208, 64);
            this.checkBoxSearchUp.Name = "checkBoxSearchUp";
            this.checkBoxSearchUp.Size = new System.Drawing.Size(124, 16);
            this.checkBoxSearchUp.TabIndex = 10;
            this.checkBoxSearchUp.Text = "Search &up";
            // 
            // comboBoxReplaceWith
            // 
            this.comboBoxReplaceWith.DropDownWidth = 280;
            this.comboBoxReplaceWith.Location = new System.Drawing.Point(96, 32);
            this.comboBoxReplaceWith.Name = "comboBoxReplaceWith";
            this.comboBoxReplaceWith.Size = new System.Drawing.Size(240, 21);
            this.comboBoxReplaceWith.TabIndex = 4;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(8, 64);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(124, 16);
            this.checkBoxMatchCase.TabIndex = 6;
            this.checkBoxMatchCase.Text = "Match &case";
            // 
            // labelFindWhat
            // 
            this.labelFindWhat.Location = new System.Drawing.Point(8, 8);
            this.labelFindWhat.Name = "labelFindWhat";
            this.labelFindWhat.Size = new System.Drawing.Size(84, 14);
            this.labelFindWhat.TabIndex = 0;
            this.labelFindWhat.Text = "Fi&nd what:";
            // 
            // comboBoxFindWhat
            // 
            this.comboBoxFindWhat.DropDownWidth = 256;
            this.comboBoxFindWhat.Location = new System.Drawing.Point(96, 4);
            this.comboBoxFindWhat.Name = "comboBoxFindWhat";
            this.comboBoxFindWhat.Size = new System.Drawing.Size(240, 21);
            this.comboBoxFindWhat.TabIndex = 1;
            this.comboBoxFindWhat.TextChanged += new System.EventHandler(this.comboBoxFindWhat_TextChanged);
            // 
            // checkBoxMatchWholeCell
            // 
            this.checkBoxMatchWholeCell.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxMatchWholeCell.Location = new System.Drawing.Point(8, 94);
            this.checkBoxMatchWholeCell.Name = "checkBoxMatchWholeCell";
            this.checkBoxMatchWholeCell.Size = new System.Drawing.Size(124, 16);
            this.checkBoxMatchWholeCell.TabIndex = 7;
            this.checkBoxMatchWholeCell.Text = "Match &whole cell";
            // 
            // buttonClose
            // 
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(352, 92);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 18;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReplace.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonReplace.Image")));
            this.buttonReplace.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonReplace.ImageIndex = 1;
            this.buttonReplace.ImageList = this.imageList1;
            this.buttonReplace.Location = new System.Drawing.Point(352, 36);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.TabIndex = 15;
            this.buttonReplace.Text = "&Replace";
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonFindNext.Location = new System.Drawing.Point(352, 8);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.TabIndex = 14;
            this.buttonFindNext.Text = "&Find Next";
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // comboBoxSelection
            // 
            this.comboBoxSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelection.DropDownWidth = 128;
            this.comboBoxSelection.Enabled = false;
            this.comboBoxSelection.Items.AddRange(new object[] {
                                                                   "Whole table",
                                                                   "Column only",
                                                                   "Selection only"});
            this.comboBoxSelection.Location = new System.Drawing.Point(208, 92);
            this.comboBoxSelection.Name = "comboBoxSelection";
            this.comboBoxSelection.Size = new System.Drawing.Size(128, 21);
            this.comboBoxSelection.TabIndex = 13;
            // 
            // GridFindReplaceDialog
            // 
            this.AcceptButton = this.buttonFindNext;
#if !SyncfusionFramework2_0
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
#endif
            this.ClientSize = new System.Drawing.Size(434, 124);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.comboBoxReplaceWith,
                                                                          this.labelReplaceWith,
                                                                          this.buttonReplaceAll,
                                                                          this.buttonClose,
                                                                          this.buttonReplace,
                                                                          this.buttonFindNext,
                                                                          this.comboBoxFindWhat,
                                                                          this.checkBoxSearchUp,
                                                                          this.checkBoxMatchWholeCell,
                                                                          this.checkBoxMatchCase,
                                                                          this.labelFindWhat,
                                                                          this.comboBoxSelection});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridFindReplaceDialog";
            this.Text = "GridFindReplaceDialog";
            this.TopMost = true;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GridFindReplaceDialog_Closing);
            this.VisibleChanged += new System.EventHandler(this.This_VisibleChanged);
            this.ResumeLayout(false);

        }
        #endregion

        private void This_VisibleChanged(object sender, System.EventArgs e)
        {
            if (this.Visible)
            {
                // Make "Find What" the focused control.
                comboBoxFindWhat.Select();
            }
            else
            {
                if (this.editView is Control)
                {
                    ((Control)this.editView).Focus();
                }
            }
        }

        private void buttonClose_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Occurs when the find text was not found.
        /// </summary>
        /// <remarks>
        /// Set e.Cancel = True if you do not want to display the
        /// default "not found" message box or if you want to display
        /// a customized dialog.
        /// </remarks>
        public event CancelEventHandler TextNotFound;

        /// <summary>
        /// Raises the <see cref="TextNotFound"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTextNotFound(CancelEventArgs e)
        {
            if (TextNotFound != null)
            {
                TextNotFound(this, e);
            }

            if (!e.Cancel)
            {
                this.buttonClose_Click(this, e);
                editView.ShowInfoMessage(strNotFound, strCategory);
            }
        }

        /// <summary>
        /// Occurs when the find operation reaches the starting point again.
        /// </summary>
        /// <remarks>
        /// Set e.Cancel = True if you do not want to display the
        /// default "reached starting point" message box or if you want to display
        /// a customized dialog.
        /// </remarks>
        public event CancelEventHandler ReachedStartingPoint;

        /// <summary>
        /// Raises the <see cref="ReachedStartingPoint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnReachedStartingPoint(CancelEventArgs e)
        {
            if (ReachedStartingPoint != null)
            {
                ReachedStartingPoint(this, e);
            }

            if (!e.Cancel)
            {
                this.buttonClose_Click(this, e);
                editView.ShowInfoMessage(strReachedStart, strCategory);
            }
        }
        
        /// <summary>
        /// Find Next occurrence of string.
        /// </summary>
        public void FindNext()
        {
            buttonFindNext_Click(this, System.EventArgs.Empty);
        }

        private void buttonFindNext_Click(object sender, System.EventArgs e)
        {
            object foundOld = this.lcrFoundOld;
            inFind = true;

            try
            {
                if (editView != null)
                {
                    AddToSearchedList(comboBoxFindWhat.Text);
                    GridFindReplaceEventArgs fea = CreateFindReplaceEventArgs();
                    object lcrFound = editView.Find(fea);
                    if (lcrFound == null)
                    {
                        OnTextNotFound(new CancelEventArgs(false));
                    }
                    else
                    {
                        Rectangle bounds = editView.GetScreenRect(lcrFound);
                        Point ptTemp = new Point(bounds.Right, bounds.Bottom);
                        if (this.Bounds.Contains(ptTemp))
                        {
                            if (Screen.PrimaryScreen.WorkingArea.Contains(new
                                Rectangle(ptTemp.X + 8, this.Top, this.Width, this.Height)))
                            {
                                this.Left = ptTemp.X + 8;
                            }
                            else if (Screen.PrimaryScreen.WorkingArea.Contains(new Rectangle(this.Left, ptTemp.Y - 8 - this.Height, this.Width, this.Height)))
                            {
                                this.Top = ptTemp.Y - 8 - this.Height;
                            }
                            else
                            {
                                ptTemp = bounds.Location;
                                if (Screen.PrimaryScreen.WorkingArea.Contains(new Rectangle(ptTemp.X - 8 - this.Width, this.Top, this.Width, this.Height)))
                                {
                                    this.Left = ptTemp.X - 8 - this.Width;
                                }
                                else
                                {
                                    this.Left = 1;
                                    this.Top = 1;
                                }
                            }
                        }

                        if (lcrFound.Equals(foundOld))
                        {
                            OnReachedStartingPoint(new CancelEventArgs(false));
                        }
                        else
                        {
                            if (lcrFoundOld == null)
                            {
                                lcrFoundOld = lcrFound;
                            }
                        }
                        ////                        bFound = true;
                    }
                }
            }
            finally
            {
                inFind = false;
            }
        }
        
         ////        private void comboBoxFindWhat_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        ////        {
        ////            if (e.KeyChar == '\r')
        ////            {
        ////                if (editView != null)
        ////                {
        ////                    GridFindReplaceEventArgs fea = CreateFindReplaceEventArgs();
        ////                    if (!editView.FindNext(fea))
        ////                    {
        ////                        editView.ShowInfoMessage("The specified text was not found.", 
        ////                            "Search Results");
        ////                    }
        ////                }
        ////            }
        ////        }

        private void buttonReplace_Click(object sender, System.EventArgs e)
        {
            if (buttonClose.Top != editOriginalCloseTop)
            {
                SetToReplaceState();
                this.comboBoxReplaceWith.Focus();
            }
            else
            {
                object foundOld = this.lcrFoundOld;
                inFind = true;

                try
                {
                    if (editView != null)
                    {
                        AddToSearchedList(comboBoxFindWhat.Text);
                        GridFindReplaceEventArgs fea = CreateFindReplaceEventArgs();
                        object lcrFound = editView.Replace(fea);
                        if (lcrFound == null)
                        {
                            OnTextNotFound(new CancelEventArgs(false));
                        }
                        else
                        {
                            Rectangle bounds = editView.GetScreenRect(lcrFound);
                            Point ptTemp = new Point(bounds.Right, bounds.Bottom);
                            if (this.Bounds.Contains(ptTemp))
                            {
                                if (Screen.PrimaryScreen.WorkingArea.Contains(new
                                    Rectangle(ptTemp.X + 8, this.Top, this.Width, this.Height)))
                                {
                                    this.Left = ptTemp.X + 8;
                                }
                                else if (Screen.PrimaryScreen.WorkingArea.Contains(new Rectangle(this.Left, ptTemp.Y - 8 - this.Height, this.Width, this.Height)))
                                {
                                    this.Top = ptTemp.Y - 8 - this.Height;
                                }
                                else
                                {
                                    ptTemp = bounds.Location;
                                    if (Screen.PrimaryScreen.WorkingArea.Contains(new Rectangle(ptTemp.X - 8 - this.Width, this.Top, this.Width, this.Height)))
                                    {
                                        this.Left = ptTemp.X - 8 - this.Width;
                                    }
                                    else
                                    {
                                        this.Left = 1;
                                        this.Top = 1;
                                    }
                                }
                            }

                            if (lcrFound.Equals(foundOld))
                            {
                                OnReachedStartingPoint(new CancelEventArgs(false));
                            }
                            else
                            {
                                if (lcrFoundOld == null)
                                {
                                    lcrFoundOld = lcrFound;
                                }
                            }
                            ////                            bFound = true;
                        }
                    }
                }
                finally
                {
                    inFind = false;
                }
            }
        }

        private GridFindReplaceEventArgs CreateFindReplaceEventArgs()
        {
            bool wholeTable = this.comboBoxSelection.SelectedIndex == indexWholeTable;
            bool columnOnly = this.comboBoxSelection.SelectedIndex == indexColumnOnly;
            bool selectionOnly = this.comboBoxSelection.SelectedIndex == indexSelectionOnly;

            GridFindReplaceEventArgs fea = new GridFindReplaceEventArgs(
                comboBoxFindWhat.Text, 
                comboBoxReplaceWith.Text,
                GridFindReplaceEventArgs.MakeFindOptions(checkBoxMatchCase.Checked, checkBoxMatchWholeCell.Checked, checkBoxSearchUp.Checked, wholeTable, columnOnly, selectionOnly),
                lcrFoundOld);

            return fea;
        }

        private void comboBoxFindWhat_TextChanged(object sender, System.EventArgs e)
        {
            CheckButtonEnabledState();
            lcrFoundOld = null;
        }

        internal void SetFindText(string str)
        {
            comboBoxFindWhat.Text = str;
        }

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        /// <param name="view">The target for find / replace operations.</param>
        /// <param name="str">The text to be shown in find text box.</param>
        /// <param name="bReplace">Specifies if dialog should be shown in find-only or replace mode.</param>
        public void SetState(IGridFindReplaceDialogSink view, string str, bool bReplace)
        {
            this.editView = view;
            comboBoxFindWhat.Text = str;
            ////            bFound = false;
            if (bReplace)
            {
                SetToReplaceState();
            }
            else
            {
                SetToFindState();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the "Replace" button should be visible. Set this false
        /// if you called <see cref="SetState"/> to show dialog in find-only mode and do not
        /// want users to give a choice to switch to replace mode.
        /// </summary>
        public bool ShowReplaceButton
        {
            get
            {
                // Old coding.
                // return buttonReplaceAll.Visible;
                return buttonReplace.Visible;
            }

            set
            {
                //// Old coding.
                ////buttonReplaceAll.Visible = false;
                buttonReplace.Visible = false;
            }
        }

        private void SetToFindState()
        {
            if (buttonClose.Top == editOriginalCloseTop)
            {
                this.Height -= offsetY;
                labelReplaceWith.Visible = false;
                comboBoxReplaceWith.Visible = false;
                buttonReplaceAll.Visible = false;
                buttonReplace.ImageIndex = 1;
                buttonClose.Top -= offsetY;
                checkBoxMatchCase.Top -= offsetY;
                checkBoxMatchWholeCell.Top -= offsetY;
                checkBoxSearchUp.Top -= offsetY;
                comboBoxSelection.Top -= offsetY;
                CheckButtonEnabledState();
            }
        }

        private void SetToReplaceState()
        {
            if (buttonClose.Top != editOriginalCloseTop)
            {
                this.Height += offsetY;
                labelReplaceWith.Visible = true;
                comboBoxReplaceWith.Visible = true;
                buttonReplaceAll.Visible = true;
                buttonReplace.ImageIndex = -1;
                buttonClose.Top += offsetY;
                checkBoxMatchCase.Top += offsetY;
                checkBoxMatchWholeCell.Top += offsetY;
                checkBoxSearchUp.Top += offsetY;
                comboBoxSelection.Top += offsetY;
                if (GridUtil.IsEmpty(comboBoxFindWhat.Text))
                {
                    buttonReplace.Enabled = false;
                }

                comboBoxFindWhat.Focus();
            }
        }

        private void CheckButtonEnabledState()
        {
            if (GridUtil.IsEmpty(comboBoxFindWhat.Text))
            {
                buttonFindNext.Enabled = false;
                if (buttonClose.Top == editOriginalCloseTop)
                {
                    buttonReplace.Enabled = false;
                }
                else
                {
                    buttonReplace.Enabled = true;
                }

                buttonReplaceAll.Enabled = false;
            }
            else
            {
                buttonFindNext.Enabled = true;
                buttonReplace.Enabled = true;
                buttonReplaceAll.Enabled = true;
            }
        }

        private void buttonReplaceAll_Click(object sender, System.EventArgs e)
        {
            GridFindReplaceEventArgs fea = CreateFindReplaceEventArgs();
            editView.ReplaceAll(fea);
        }

        private string GetReplacedString(string strOld)
        {
            return comboBoxReplaceWith.Text;
        }

        private void GridFindReplaceDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        void ApplyOptions()
        {
            if ((options & GridFindReplaceDialogOptions.ShowMatchCase) != GridFindReplaceDialogOptions.None)
            {
                this.checkBoxMatchCase.Visible = true;
            }
            else
            {
                this.checkBoxMatchCase.Visible = false;
            }

            if ((options & GridFindReplaceDialogOptions.ShowMatchWholeCell) != GridFindReplaceDialogOptions.None)
            {
                this.checkBoxMatchWholeCell.Visible = true;
            }
            else
            {
                this.checkBoxMatchWholeCell.Visible = false;
            }

            if ((options & GridFindReplaceDialogOptions.ShowSearchSelection) != GridFindReplaceDialogOptions.None)
            {
                this.comboBoxSelection.Visible = true;
                this.comboBoxSelection.Enabled = true;
            }
            else
            {
                this.comboBoxSelection.Visible = false;
            }

            if ((options & GridFindReplaceDialogOptions.ShowSearchUp) != GridFindReplaceDialogOptions.None)
            {
                this.checkBoxSearchUp.Visible = true;
            }
            else
            {
                this.checkBoxSearchUp.Visible = false;
            }
        }
    }
    
    /// <summary>
    /// Provides information about the find and replace dialog.
    /// </summary>
    public sealed class GridFindReplaceEventArgs : SyncfusionEventArgs
    {
        string _findString;
        string _replaceString;
        GridFindTextOptions _options;
        object _locationInfo;

        /// <summary>
        /// Initializes a <see cref="GridFindReplaceEventArgs"/> with options for a find or replace operation.
        /// </summary>
        /// <param name="findString">The text to be searched.</param>
        /// <param name="replaceString">The replacement text.</param>
        /// <param name="options">Search criteria.</param>
        /// <param name="locationInfo">Information about the current selection. If target is a grid control,
        /// this should be of type <see cref="GridRangeInfo"/>.</param>
        public GridFindReplaceEventArgs(string findString, string replaceString, GridFindTextOptions options, object locationInfo)
        {
            this._findString = findString;
            this._replaceString = replaceString;
            this._options = options;
            this._locationInfo = locationInfo;
        }

        /// <summary>
        /// Gets or sets the text to be searched.
        /// </summary>
        [TraceProperty(true)]
        public string FindString
        {
            get
            {
                return _findString;
            }

            set
            {
                _findString = value;
            }
        }

        /// <summary>
        /// Gets or sets the replacement text.
        /// </summary>
        [TraceProperty(true)]
        public string ReplaceString
        {
            get
            {
                return _replaceString;
            }

            set
            {
                _replaceString = value;
            }
        }

        /// <summary>
        /// Gets or sets Options. Search criteria.
        /// </summary>
        [TraceProperty(true)]
        public GridFindTextOptions Options
        {
            get
            {
                return _options;
            }

            set
            {
                _options = value;
            }
        }

        /// <summary>
        /// Gets or sets information about the current selection. If target is a grid control,
        /// this is of type <see cref="GridRangeInfo"/>.
        /// </summary>
        [TraceProperty(true)]
        public object LocationInfo
        {
            get
            {
                return _locationInfo;
            }

            set
            {
                _locationInfo = value;
            }
        }

        internal static GridFindTextOptions MakeFindOptions(bool bMatchCase, bool bMatchWholeCell, bool bSearchUp, bool bWholeTable, bool bColumnOnly, bool bSelectionOnly)
        {
            GridFindTextOptions opt = GridFindTextOptions.None;
            if (bMatchCase)
            {
                opt |= GridFindTextOptions.MatchCase;
            }

            if (bMatchWholeCell)
            {
                opt |= GridFindTextOptions.MatchWholeCell;
            }

            if (bSearchUp)
            {
                opt |= GridFindTextOptions.SearchUp;
            }

            if (bSelectionOnly)
            {
                opt |= GridFindTextOptions.SelectionOnly;
            }

            if (bColumnOnly)
            {
                opt |= GridFindTextOptions.ColumnOnly;
            }

            if (bWholeTable)
            {
                opt |= GridFindTextOptions.WholeTable;
            }

            return opt;
        }
    }

    /// <summary>
    /// Options used for customizing <see cref="GridFindReplaceDialog"/>:
    /// </summary>
    [Flags]
    public enum GridFindReplaceDialogOptions
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Show Match Case check box.
        /// </summary>
        ShowMatchCase = 1,

        /// <summary>
        /// Show Match Case check box.
        /// </summary>
        ShowMatchWholeCell = 2,

        /// <summary>
        /// Show Search Up check box.
        /// </summary>
        ShowSearchUp = 4,

        /// <summary>
        /// Show Selection combo box.
        /// </summary>
        ShowSearchSelection = 8,

        /// <summary>
        /// Default setting.
        /// </summary>
        [Browsable(false)]
        All = 15
    }

    // Should be used as input for Find, Replace etc.

    // GridFindReplaceDialogSink vs DataBoundGridFindReplaceDialogSink

    /// <summary>
    /// Provides a default implementation of a <see cref="IGridFindReplaceDialogSink"/>
    /// for a grid control.
    /// </summary>
    /// <example>
    /// The following example demonstrates how to enable a grid control for find and replace support:
    /// <code lang="C#">
    /// GridFindReplaceDialogSink findReplaceDialogSink;
    /// <para/>
    /// public GridFindReplaceDialogSink GridFindReplaceDialogSink
    /// {
    ///     get
    ///     {
    ///         if (findReplaceDialogSink == null)
    ///         {
    ///             findReplaceDialogSink = new GridFindReplaceDialogSink(this);
    ///         }
    ///         return findReplaceDialogSink;
    ///     }
    /// }
    /// <para/>
    /// protected override void OnCurrentCellActivated(EventArgs e)
    /// {
    ///     GridFindReplaceDialog.ResetFindLocation();
    ///     base.OnCurrentCellActivated(e);
    /// }
    /// <para/>
    /// protected override void OnControlGotFocus()
    /// {
    ///     GridFindReplaceDialog.SetActiveSinkIfVisible(GridFindReplaceDialogSink);
    ///     base.OnControlGotFocus();
    /// }
    /// <para/>
    /// // to show the dialog:
    /// GridFindReplaceDialog frDialog = GridFindReplaceDialog.Instance;
    /// frDialog.SetState(grid.GridFindReplaceDialogSink, "", false);
    /// frDialog.Show();
    /// </code>
    /// <code lang="VB">
    /// Dim findReplaceDialogSink As GridFindReplaceDialogSink
    /// <para/>
    /// <para/>
    /// Public ReadOnly Property GridFindReplaceDialogSink() As GridFindReplaceDialogSink
    ///     Get
    ///         If findReplaceDialogSink Is Nothing Then
    ///             findReplaceDialogSink = New GridFindReplaceDialogSink(Me)
    ///         End If
    ///         Return findReplaceDialogSink
    ///     End Get
    /// End Property
    /// <para/>
    /// <para/>
    /// Protected Overrides Sub OnCurrentCellActivated(e As EventArgs)
    ///     GridFindReplaceDialog.ResetFindLocation()
    ///     MyBase.OnCurrentCellActivated(e)
    /// End Sub 'OnCurrentCellActivated
    /// <para/>
    /// <para/>
    /// Protected Overrides Sub OnControlGotFocus()
    ///     GridFindReplaceDialog.SetActiveSinkIfVisible(GridFindReplaceDialogSink)
    ///     MyBase.OnControlGotFocus()
    /// End Sub 'OnEnter
    /// <para/>
    /// ' to show the dialog:
    /// Dim frDialog As GridFindReplaceDialog = GridFindReplaceDialog.Instance
    /// frDialog.SetState(grid.GridFindReplaceDialogSink, "", False)
    /// frDialog.Show()
    /// </code>
    /// </example>
    public class GridFindReplaceDialogSink : GridSubComponent, IGridFindReplaceDialogSink
    {
        enum Operation
        {
            /// <summary>
            /// Represents None
            /// </summary>
            None,

            /// <summary>
            /// Represents Find
            /// </summary>
            Find,

            /// <summary>
            /// Represents Replace
            /// </summary>
            Replace,

            /// <summary>
            /// Represents ReplaceAll
            /// </summary>
            ReplaceAll
        }

        Operation op = Operation.None;
        bool replaceAllFound = false;

        /// <summary>
        /// Constructs a GridFindReplaceDialogSink and associates it with a grid control.
        /// </summary>
        /// <param name="grid">The grid control this object operates on.</param>
        public GridFindReplaceDialogSink(GridControlBase grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Finds the text with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        /// <returns>The location info (typically a <see cref="GridRangeInfo"/>) or NULL if not found.</returns>
        public virtual object Find(GridFindReplaceEventArgs e)
        {
            op = Operation.Find;

            return InternalFind(e);
        }

        /// <summary>
        /// Internals the find.
        /// </summary>
        /// <param name="find">The <see cref="Syncfusion.Windows.Forms.Grid.GridFindReplaceEventArgs"/> instance containing the event data.</param>
        /// <returns>returns object</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual object InternalFind(GridFindReplaceEventArgs find)
        {
            bool searchUp = (find.Options & GridFindTextOptions.SearchUp) != 0;

            int rowIndex, colIndex;
            if (!Grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
            {
                return null;
            }

            Grid.CurrentCell.ConfirmChanges();

            try
            {
                GridRangeInfo rgCell = GridRangeInfo.Cell(rowIndex, colIndex);
                GridRangeInfo found = null;
                GridRangeInfo selRange = GridRangeInfo.Empty;
                GridRangeInfo wholeRange = Grid.GridCellsRange;

                if ((find.Options & GridFindTextOptions.SelectionOnly) != GridFindTextOptions.None)
                {
                    selRange = Grid.Selections.Ranges.GetRangesContaining(rgCell).ActiveRange;
                }
                else if ((find.Options & GridFindTextOptions.ColumnOnly) != GridFindTextOptions.None)
                {
                    selRange = GridRangeInfo.Col(colIndex);
                }
                else if ((find.Options & GridFindTextOptions.WholeTable) != GridFindTextOptions.None)
                {
                    selRange = GridRangeInfo.Table();
                }

                if (!selRange.IsEmpty)
                {
                    selRange = selRange.ExpandRange(wholeRange.Top, wholeRange.Left, wholeRange.Bottom, wholeRange.Right);

                    GridRangeInfo rgStart = GridRangeInfo.Cell(selRange.Top, selRange.Left);
                    GridRangeInfo rgEnd = GridRangeInfo.Cell(selRange.Bottom, selRange.Right);

                    useFirstRowCol = false;
                    if (!searchUp)
                    {
                        found = FindInRange(find, selRange, rgCell, rgEnd);
                        if (found == null)
                        {
                            useFirstRowCol = true;
                            found = FindInRange(find, selRange, rgStart, rgCell);
                        }
                    }
                    else
                    {
                        found = FindInRange(find, selRange, rgCell, rgStart);
                        if (found == null)
                        {
                            useFirstRowCol = true;
                            found = FindInRange(find, selRange, rgEnd, rgCell);
                        }
                    }

                    useFirstRowCol = false;
                }

                return found;
            }
            finally
            {
                Grid.CurrentCell.Refresh();
            }
        }

        bool useFirstRowCol = false;

        /// <summary>
        /// Finds the in range.
        /// </summary>
        /// <param name="find">The <see cref="Syncfusion.Windows.Forms.Grid.GridFindReplaceEventArgs"/> instance containing the event data.</param>
        /// <param name="selRange">The sel grid range.</param>
        /// <param name="start">The start grid range.</param>
        /// <param name="end">The end grid range.</param>
        /// <returns>returns GridRangeInfo</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual GridRangeInfo FindInRange(GridFindReplaceEventArgs find, GridRangeInfo selRange, GridRangeInfo start, GridRangeInfo end)
        {
            bool searchUp = (find.Options & GridFindTextOptions.SearchUp) != 0;

            using (OperationFeedback feedback = new OperationFeedback(Grid.Model))
            {
                // Return GridRangeInfo if found, otherwise NULL.
                int rowIndex = start.Top;
                int colIndex = start.Left;
                useFirstRowCol |= op != Operation.Find;
                while (useFirstRowCol || GetNextCell(selRange, ref rowIndex, ref colIndex, true, searchUp))
                {
                    useFirstRowCol = false;
                    GridStyleInfo style = Grid.Model[rowIndex, colIndex];
                    GridCellRendererBase renderer = Grid.CellRenderers[style.CellType];
                    // TODO: Match case, Match whole cell // set curpos etc necessary?
                    bool found = false;
                    switch (op)
                    {
                        case Operation.Find:
                            found = renderer.FindText(find.FindString, rowIndex, colIndex, find.Options, true);
                            break;
                        case Operation.Replace:
                            found = renderer.ReplaceText(find.FindString, find.ReplaceString, rowIndex, colIndex, find.Options, true);
                            break;
                        case Operation.ReplaceAll:
                            replaceAllFound |= renderer.ReplaceText(find.FindString, find.ReplaceString, rowIndex, colIndex, find.Options, false);
                            break;
                    }

                    if (found)
                    {
                        Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.FindText);
                        return GridRangeInfo.Cell(rowIndex, colIndex);
                    }

                    if (rowIndex == end.Top && colIndex == end.Left)
                    {
                        return null;
                    }

                    if (feedback.ShouldCancel)
                    {
                        return null;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the next cell.
        /// </summary>
        /// <param name="rg">The grid range info.</param>
        /// <param name="nRow">The row index</param>
        /// <param name="nCol">The col index.</param>
        /// <param name="bSortByRow">if set to <c>true</c> [b sort by row].</param>
        /// <param name="searchUp">if set to <c>true</c> [search up].</param>
        /// <returns>returns the boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        static public bool GetNextCell(GridRangeInfo rg, ref int nRow, ref int nCol, bool bSortByRow, bool searchUp)
        {
            if (!searchUp)
            {
                if (bSortByRow && ++nCol > rg.Right)
                {
                    if (++nRow > rg.Bottom)
                    {
                        nRow = nCol = 0;
                    }
                    else
                    {
                        nCol = rg.Left;
                    }
                }
                else if (!bSortByRow && ++nRow > rg.Bottom)
                {
                    if (++nCol > rg.Right)
                    {
                        nRow = nCol = 0;
                    }
                    else
                    {
                        nRow = rg.Top;
                    }
                }

                return nRow > 0 || nCol > 0;
            }
            else
            {
                if (bSortByRow && --nCol < rg.Left)
                {
                    if (--nRow < rg.Top)
                    {
                        nRow = rg.Bottom; 
                        nCol = rg.Right;
                    }
                    else
                    {
                        nCol = rg.Right;
                    }
                }
                else if (!bSortByRow && --nRow < rg.Top)
                {
                    if (--nCol < rg.Left)
                    {
                        nRow = rg.Bottom; 
                        nCol = rg.Right;
                    }
                    else
                    {
                        nRow = rg.Bottom;
                    }
                }

                return nRow < rg.Bottom || nCol < rg.Right;
            }
        }

        /// <summary>
        /// Returns the screen bounds for the selection object. The find and replace dialog
        /// calls this method to ensure that it does not hide the current selected text after
        /// a successful find operation.
        /// </summary>
        /// <param name="lc">The location object, typically a <see cref="GridRangeInfo"/>.</param>
        /// <returns>The bounds in screen coordinates.</returns>
        public virtual Rectangle GetScreenRect(object lc)
        {
            GridRangeInfo rg = lc as GridRangeInfo;
            if (lc != null)
            {
                Rectangle r = Grid.RangeInfoToRectangle(rg);
                return Grid.RectangleToScreen(r);
            }

            return Rectangle.Empty;
        }

        ////        public bool HasSelection 
        ////        { 
        ////            get
        ////            {
        ////                return Grid.CurrentCell.HasCurrentCell;
        ////            }
        ////        }
        ////        public string SelectedText 
        ////        { 
        ////            get
        ////            {
        ////                if (!Grid.CurrentCell.HasCurrentCell)
        ////                    return "";
        ////
        ////                GridCellRendererBase renderer = Grid.CurrentCell.Renderer;
        ////                string text;
        ////                if (renderer.GetSelectedText(out text))
        ////                    return text;
        ////                return ""; //// selected text in current cell
        ////            }
        ////        }

        /// <summary>
        /// Replaces the text with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        /// <returns>The location info (typically a <see cref="GridRangeInfo"/>) or NULL if not found.</returns>
        public virtual object Replace(GridFindReplaceEventArgs e)
        {
            op = Operation.Replace;
            return InternalFind(e);
        }

        /// <summary>
        /// Replace the text in all cells with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        public virtual void ReplaceAll(GridFindReplaceEventArgs e)
        {
            replaceAllFound = false;
            op = Operation.ReplaceAll;
            InternalFind(e);
        }

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="msg">The message text.</param>
        /// <param name="category">The dialog title.</param>
        public virtual void ShowInfoMessage(string msg, string category)
        {
            MessageBox.Show(Grid, msg, category);
        }
    }

    // implementation of IGridFindReplaceDialogSink 
    // should have knowlege about the parent grid (Current Cell position,
    // selected text etc.)

    /// <summary>
    /// Provides an interface that the <see cref="GridFindReplaceDialog"/> uses
    /// to communicate with its target control for find and replace operations. See <see cref="GridFindReplaceDialogSink"/>
    /// for an implementation for a regular grid control.
    /// </summary>
    public interface IGridFindReplaceDialogSink
    {
        /// <summary>
        /// Finds the text with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        /// <returns>The location info (typically a <see cref="GridRangeInfo"/>) or NULL if not found.</returns>
        object Find(GridFindReplaceEventArgs e);

        /// <summary>
        /// Replaces the text with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        /// <returns>The location info (typically a <see cref="GridRangeInfo"/>) or NULL if not found.</returns>
        object Replace(GridFindReplaceEventArgs e);

        /// <summary>
        /// Replace the text in all cells with the specified criteria.
        /// </summary>
        /// <param name="e">Text and search criteria.</param>
        void ReplaceAll(GridFindReplaceEventArgs e);

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="msg">The message text.</param>
        /// <param name="category">The dialog title.</param>
        void ShowInfoMessage(string msg, string category);

        /// <summary>
        /// Returns the screen bounds for the selection object. The find and replace dialog
        /// calls this method to ensure that it does not hide the current selected text after
        /// a successful find operation.
        /// </summary>
        /// <param name="lc">The location object, typically a <see cref="GridRangeInfo"/>.</param>
        /// <returns>The bounds in screen coordinates.</returns>
        Rectangle GetScreenRect(object lc);
    }
}
