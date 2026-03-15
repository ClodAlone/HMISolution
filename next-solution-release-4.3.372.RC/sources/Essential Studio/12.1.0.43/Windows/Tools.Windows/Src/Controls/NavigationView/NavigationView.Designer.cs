#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Navigation View
    /// </summary>
    public partial class NavigationView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._textBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
			this._popupMenu = new Syncfusion.Windows.Forms.Tools.NavigationView.PopupMenu( this.components );
			this._parentBarItem = new Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem();
			this._autoComplete = new Syncfusion.Windows.Forms.Tools.AutoComplete( this.components );
			this._popupContainer = new Syncfusion.Windows.Forms.PopupControlContainer();
			this._recentList = new System.Windows.Forms.ListView();
			this._columnHeader = new System.Windows.Forms.ColumnHeader();
			this._scrollersFrame = new Syncfusion.Windows.Forms.ScrollersFrame( this.components );
			this.autoCompleteDataColumnInfo1 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo( "Column0", 100, true );
			this.autoCompleteDataColumnInfo2 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo( "Column1", 100, true );
			( (System.ComponentModel.ISupportInitialize)( this._autoComplete ) ).BeginInit();
			this.SuspendLayout();
			// 
			// _textBox
			// 
			this._textBox.AcceptsReturn = true;
			this._textBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._autoComplete.SetAutoComplete( this._textBox, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoSuggest );
			this._textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBox.Location = new System.Drawing.Point( 0, 0 );
			this._textBox.Name = "_textBox";
			this._textBox.OverflowIndicatorToolTipText = null;
			this._textBox.ShortcutsEnabled = false;
			this._textBox.Size = new System.Drawing.Size( 100, 13 );
			this._textBox.TabIndex = 0;
			this._textBox.Visible = false;
			this._textBox.WordWrap = false;
			this._textBox.KeyDown += new System.Windows.Forms.KeyEventHandler( this.OnTextBoxKeyDown );
			this._textBox.Leave += new System.EventHandler( this.OnTextBoxLostFocus );
			// 
			// _popupMenu
			// 
			this._popupMenu.ParentBarItem = this._parentBarItem;
			this._popupMenu.Collapse += new System.EventHandler( this.OnPopupMenuClosed );
			// 
			// _autoComplete
			// 
			this._autoComplete.AccessibleDescription = null;
			this._autoComplete.AccessibleName = null;
			this._autoComplete.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this._autoComplete.AdjustHeightToItemCount = true;
			this._autoComplete.AutoAddItem = false;
			this._autoComplete.AutoSerialize = false;
			this._autoComplete.BorderType = Syncfusion.Windows.Forms.Tools.AutoCompleteBorderTypes.Fixed;
            this._autoComplete.CategoryName = string.Empty;
			this._autoComplete.Columns.Add( this.autoCompleteDataColumnInfo1 );
			this._autoComplete.Columns.Add( this.autoCompleteDataColumnInfo2 );
			this._autoComplete.PreferredHeight = 200;
			this._autoComplete.SelectedIndex = -1;
            this._autoComplete.SelectedValue = string.Empty;
			this._autoComplete.ShowCloseButton = true;
			this._autoComplete.ShowGripper = true;
			this._autoComplete.DropDownDisplayed += new System.EventHandler( this.OnAutoCompleteDropDownDisplayed );
			this._autoComplete.DropDownClosed += new Syncfusion.Windows.Forms.PopupClosedEventHandler( this.OnAutoCompleteDropDownClosed );
			this._autoComplete.AutoCompleteItemSelected += new Syncfusion.Windows.Forms.Tools.AutoCompleteItemEventHandler( this.OnAutoCompleteItemSelected );
			// 
			// _popupContainer
			// 
			this._popupContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._popupContainer.Location = new System.Drawing.Point( 0, 0 );
			this._popupContainer.Name = "_popupContainer";
			this._popupContainer.Size = new System.Drawing.Size( 200, 100 );
			this._popupContainer.TabIndex = 0;
			this._popupContainer.Visible = false;
			// 
			// _recentList
			// 
			this._recentList.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this._recentList.AutoArrange = false;
			this._recentList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._recentList.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this._columnHeader} );
			this._recentList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._recentList.FullRowSelect = true;
			this._recentList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this._recentList.HideSelection = false;
			this._recentList.HoverSelection = true;
			this._recentList.Location = new System.Drawing.Point( 0, 0 );
			this._recentList.Name = "_recentList";
			this._recentList.ShowGroups = false;
			this._recentList.Size = new System.Drawing.Size( 121, 97 );
			this._recentList.TabIndex = 0;
			this._recentList.UseCompatibleStateImageBehavior = false;
			this._recentList.View = System.Windows.Forms.View.Details;
			this._recentList.ItemActivate += new System.EventHandler( this.OnRecentItemClicked );
			// 
			// _scrollersFrame
			// 
			this._scrollersFrame.AttachedTo = this._recentList;
			this._scrollersFrame.SizeGripperVisibility = Syncfusion.Windows.Forms.SizeGripperVisibility.Hidden;
			this._scrollersFrame.VisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Office2007;
			// 
			// autoCompleteDataColumnInfo1
			// 
			this.autoCompleteDataColumnInfo1.ColumnHeaderText = "Column0";
			this.autoCompleteDataColumnInfo1.ImageColumn = false;
			this.autoCompleteDataColumnInfo1.MatchingColumn = false;
			this.autoCompleteDataColumnInfo1.Visible = true;
			// 
			// autoCompleteDataColumnInfo2
			// 
			this.autoCompleteDataColumnInfo2.ColumnHeaderText = string.Empty;
			this.autoCompleteDataColumnInfo2.ImageColumn = true;
			this.autoCompleteDataColumnInfo2.MatchingColumn = false;
			this.autoCompleteDataColumnInfo2.Visible = true;
			( (System.ComponentModel.ISupportInitialize)( this._autoComplete ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

        private PopupMenu _popupMenu;
        private ParentBarItem _parentBarItem;
        private TextBoxExt _textBox;
        private AutoComplete _autoComplete;
        private PopupControlContainer _popupContainer;
        private ListView _recentList;
        private ColumnHeader _columnHeader;
        private ScrollersFrame _scrollersFrame;
        private AutoCompleteDataColumnInfo autoCompleteDataColumnInfo1;
        private AutoCompleteDataColumnInfo autoCompleteDataColumnInfo2;
    }
}
