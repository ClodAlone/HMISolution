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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Provides data for the <see cref="EditableList.ItemChanging"/> event.
	/// </summary>
	public class ListBoxTextChangingEventArgs:
		CancelEventArgs
	{
		private string m_sNewText;
		/// <summary>
		/// The last selected index for the edited row in the list.
		/// </summary>
		private int m_iLastSelectedIndex;

		/// <summary>
		/// Creates a new instance of the ListBoxTextChangingEventArgs class.
		/// </summary>
		/// <param name="pNewText">The new text after the change.</param>
		/// <param name="pSelectedIndex">Last selected index.</param>
		public ListBoxTextChangingEventArgs( string pNewText, int pSelectedIndex )
		{
			m_sNewText = pNewText;
			m_iLastSelectedIndex = pSelectedIndex;
		}

		/// <summary>
		/// Creates a new instance of the ListBoxTextChangingEventArgs class.
		/// </summary>
		/// <param name="pNewText">The new text after the change.</param>
		public ListBoxTextChangingEventArgs( string pNewText ):
			this( pNewText, -1 )
		{
		}

		/// <summary>
		/// Returns the new text for the edited row in the list.
		/// </summary>
		public string NewText
		{
			get
			{
				return m_sNewText;
			}
		}
		/// <summary>
		/// Gets the last selected index for the edited row in the list.
		/// </summary>
		public int LastSelectedIndex
		{
			get
			{
				return m_iLastSelectedIndex;
			}
		}
	}

	/// <summary>
	/// Handles the <see cref="EditableList.ItemChanging"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="ListBoxTextChangingEventArgs"/> that contains the event data.</param>
	public delegate void ListBoxTextChangingEventHandler( object sender, ListBoxTextChangingEventArgs e );

	/// <summary>
	/// A <see cref="System.Windows.Forms.UserControl"/> that provides you an editable <see cref="System.Windows.Forms.ListBox"/>
	/// with a <see cref="System.Windows.Forms.TextBox"/> and <see cref="System.Windows.Forms.Button"/>
	/// on the current row.
	/// </summary>
	/// <remarks>
	/// <para>
	/// During design-time (and in code) you can access the list box, text box and button components of this
	/// control using the <see cref="System.Windows.Forms.ListBox"/>, <see cref="System.Windows.Forms.TextBox"/> and <see cref="Button"/>
	/// properties. You can add certain items to the list and set some properties on the textbox and button.
	/// </para>
	/// <para>
	/// The <see cref="ButtonClick"/> event is thrown when the user clicks the button
	/// and <see cref="ItemChanging"/> event is thrown when the user completes editing an item.
	/// </para>
	/// </remarks>
	[Designer(typeof(EditableListDesigner), typeof(System.ComponentModel.Design.IDesigner)),
    System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.EditableList.bmp")]
	[Description("Represents a ListBox control with item editing functionality.")]
	public class EditableList : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// The <see cref="System.Windows.Forms.ListBox"/> used by the control.
		/// </summary>
		private CustomListBox listBox;
		/// <summary>
		/// The <see cref="System.Windows.Forms.TextBox"/> used by the control.
		/// </summary>
		private System.Windows.Forms.TextBox textBox;
		/// <summary>
		/// The <see cref="System.Windows.Forms.Button"/> used by the control.
		/// </summary>
		private System.Windows.Forms.Button button;
		// Use selIndexChanged and lastKnownSelIndex to figure out if editing should be
		// started on mouse up. The complexity arises due to the fact that in ListBox_MouseDown
		// the SelectedIndex has already changed, but the SelectedIndexChanged event not yet fired.
		private bool selIndexChanged = false;
		private int lastKnownSelIndex = -1;
		private bool editing = false;
		private bool wantButton = true;
		private static int EditorStripHeight = 20;
		/// <summary>
		/// Index of the item, that is being edited.
		/// </summary>
		private int m_nEditedItem = -1;
		/// <summary> 
		/// Indicates whether <see "EditableList.ListBox_SelectedIndexChanged"/> method is ignoring changes of list box selected index.
		/// </summary>
		private bool bIgnoreIndexChanging = false;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);
        /// <summary>
        /// Default height of the ListBoxItem
        /// </summary>
        private int LISTITEMHEIGHT = default(int);
        /// <summary>
        /// Default height of the ListBoxItem
        /// </summary>
        private int LISTBUTTONHEIGHT = default(int);


		/// <summary>
		/// Fired when the user clicks on the button.
		/// </summary>
        [Description("Fired when the user clicks on the button")]
		public event EventHandler ButtonClick;
		/// <summary>
		/// Fired when the user completes editing a row.
		/// </summary>
        [Description("Fired when the user completes editing a row")]
		public event ListBoxTextChangingEventHandler ItemChanging;

		/// <summary>
		/// Raised before a list item is edited.
		/// </summary>
        [Description("Raised before a list item is edited.")]

		public event CancelEventHandler BeforeListItemEdit;

		/// <summary>
		/// Raised when a list item is to be drawn.
		/// </summary>
        [Description("Raised when a list item is to be drawn.")]
		public event ListItemDrawEventHandler ListItemDraw;

		/// <summary>
		/// Creates a new instance of the <see cref="EditableList"/> control.
		/// </summary>
		public EditableList()
		{
			// This call is required by the Windows.Forms Form designer.
			InitializeComponent();
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(EditableList));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            CTRLSIZE = this.Size;
            LISTITEMHEIGHT = this.ListBox.ItemHeight;
            LISTBUTTONHEIGHT = this.Button.Height;
		}

        #region For Touch

        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
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
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            this.ListBox.ItemHeight = (int)(LISTITEMHEIGHT * scaleFactor);
            this.Button.Height = this.TextBox.Height;
             isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }

        #endregion


		/// <summary>
		/// Returns the <see cref="System.Windows.Forms.ListBox"/> associated with this control.
		/// </summary>
		/// <remarks>
		/// You should access the list box to add items to it.
		/// </remarks>
		[Category("Child Components"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBox ListBox
		{
			get{return this.listBox;}
		}
		/// <summary>
		/// Returns the <see cref="System.Windows.Forms.TextBox"/> associated with this control.
		/// </summary>
		/// <remarks>
		/// This textbox represents the textbox used in the editable current row.
		/// </remarks>
		[Category("Child Components"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TextBox TextBox
		{
			get{return this.textBox;}
		}
		/// <summary>
		/// Returns the <see cref="System.Windows.Forms.Button"/> associated with this control.
		/// </summary>
		/// <remarks>
		/// This button represents the button drawn at the right of the current row.
		/// </remarks>
		[Category("Child Components"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Button Button
		{
			get{return this.button;}
		}
		/// <summary>
		/// Indicates whether you want the button to be shown to the right while editing.
		/// </summary>
		/// <value>True if button should be shown; False otherwise. Default is True.</value>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether you want the button to be shown to the right while editing.")]
		public bool WantButton
		{
			get{return this.wantButton;}
			set
			{
				if(this.wantButton != value)
				{
					this.wantButton = value;
				}
			}
		}
		/// <summary>
		/// Indicates whether the current row is being edited.
		/// </summary>
		/// <value>
		/// True to indicate its being edited; False otherwise.
		/// </value>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
		public bool Editing
		{
			get{return this.editing;}
			set
			{
				if(this.editing != value)
				{
					if(!value)
						this.EndEditing(value);
					else
						this.StartEditing();
				}
			}
		}

		/// <summary>
		/// Gets or sets the list box text alignment.
		/// </summary>
		[DefaultValue( TextAlignment.Left )]
		[Description( "Gets or sets the list box text alignment." )]
		public TextAlignment ListBoxTextAlignment
		{
			get
			{
				return listBox.TextAlignment;
			}
			set
			{
				if( listBox.TextAlignment != value )
				{
					listBox.TextAlignment = value;

					listBox.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates, whether <see cref="ListBoxTextAlignment"/> property value should be serialized.
		/// </summary>
		[Description( "Indicates, whether ListBoxTextAlignment property value should be serialized." )]
		public bool ShouldSerializeListBoxTextAlignment()
		{
			return (this.ListBoxTextAlignment != TextAlignment.Left);
		}

		/// <summary>
		/// Resets the <see cref="TextAlignment"/> property to the default value.
		/// </summary>
		[Description( "Resets the TextAlignment property to the default value." )]
		public void ResetTextAlignment()
		{
			this.ListBoxTextAlignment = TextAlignment.Left;
		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.listBox.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseUp);
				this.listBox.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
				this.listBox.SelectedIndexChanged -= new System.EventHandler(this.ListBox_SelectedIndexChanged);
				this.listBox.ItemDraw -= new ListItemDrawEventHandler(this.HandleListItemDraw);
				
				this.button.Click -= new System.EventHandler(this.Button_Click);
				this.button.LostFocus -= new System.EventHandler(this.Button_LostFocus);

				this.textBox.LostFocus -= new System.EventHandler(this.TextBox_LostFocus);

				if(components != null)
				{
					components.Dispose();

				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox = new CustomListBox();
			this.textBox = new System.Windows.Forms.TextBox();
			this.button = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox
			// 
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(150, 147);
			this.listBox.TabIndex = 0;
			this.listBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseUp);
			this.listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
			this.listBox.ItemDraw += new ListItemDrawEventHandler(this.HandleListItemDraw);

			// 
			// textBox
			// 
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox.Location = new System.Drawing.Point(8, 120);
			this.textBox.Name = "textBox";
			this.textBox.TabIndex = 2;
			this.textBox.Text = "";
			this.textBox.Visible = false;
			this.textBox.Size = new Size(100, EditorStripHeight);
			this.textBox.LostFocus += new System.EventHandler(this.TextBox_LostFocus);
			// 
			// button
			// 
			this.button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button.Location = new System.Drawing.Point(112, 120);
			this.button.Name = "button";
			this.button.Size = new System.Drawing.Size(30, EditorStripHeight);
			this.button.TabIndex = 2;
			this.button.Text = "...";
			this.button.Visible = false;
			this.button.Click += new System.EventHandler(this.Button_Click);
			this.button.LostFocus += new System.EventHandler(this.Button_LostFocus);
			// 
			// EditableList
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button,
																		  this.textBox,
																		  this.listBox});
			this.Name = "EditableList";
			this.ResumeLayout(false);

		}
		#endregion
		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(Control.FromHandle(msg.HWnd) == this.textBox)
			{
				if(keyData == Keys.Enter)
				{
					this.EndEditing(true);
					return true;
				}
				else if(keyData == Keys.Escape)
				{
					this.EndEditing(false);
					return true;
				}
			}
			if (keyData == Keys.F2)
				this.StartEditing();

			return base.ProcessCmdKey(ref msg, keyData);
		}
		private void ListBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.selIndexChanged = false;
		}
		private void ListBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(!this.selIndexChanged
				&& this.listBox.IndexFromPoint(e.X, e.Y) == this.listBox.SelectedIndex)
			{
				this.StartEditing();
			}
		}

		/// <summary>
		/// Called just before a row goes into editing mode.
		/// </summary>
		protected virtual void StartEditing()
		{
			if(this.Editing)
				return;

			if(this.listBox.SelectedIndex != -1)
			{
				bool cancel = this.RaiseBeforeListItemEdit();
				
				if(!cancel)
				{
					m_nEditedItem = this.listBox.SelectedIndex;

					bool bIsMirrored = GetIsMirrored();

					Rectangle rect = this.listBox.GetItemRectangle( m_nEditedItem );
					rect = this.listBox.RectangleToScreen(rect);
					rect = this.listBox.Parent.RectangleToClient(rect);

					if(rect.Height < 20)
					{
						rect.Y -= (20 - rect.Height)/2;
						if(rect.Y < 0)
							rect.Y = this.ClientRectangle.Top;
					}

					if(this.WantButton)
					{
						int nBtnWidth = this.button.Width;
						rect.Width -= nBtnWidth;
						if (bIsMirrored)
						{
							rect.X += nBtnWidth;
						}
					}

					this.textBox.Modified = false;
					this.textBox.Bounds = rect;
					this.textBox.Visible = true;
					//This returns the actual value of the selected item and not "System.Data.DataRowView".
					this.textBox.Text = this.listBox.GetItemText(this.listBox.SelectedItem);
					this.textBox.Focus();
					this.textBox.SelectionStart = 0;
					this.textBox.SelectionLength = 0;

					if(this.WantButton)
					{
						this.button.Visible = true;
						
						int nBtnRight = bIsMirrored ? this.textBox.Left - this.button.Width : this.textBox.Right;
						this.button.Location = new Point( nBtnRight, this.textBox.Top );
					}
					this.editing = true;
				}
			}
		}

		/// <summary>
		/// Called just after a row comes out of editing mode.
		/// </summary>
		protected virtual void EndEditing(bool save)
		{
			if( !this.Editing )
				return;

			if( save && (this.textBox.Modified || this.TextBox.Text != this.listBox.Items[m_nEditedItem].ToString()) )
			{
				ListBoxTextChangingEventArgs e = new ListBoxTextChangingEventArgs( this.textBox.Text, m_nEditedItem );
				this.OnItemChanging( e );
				if( !e.Cancel )
				{
					bIgnoreIndexChanging = true;
					this.listBox.Items[m_nEditedItem] = this.textBox.Text;
					bIgnoreIndexChanging = false;
				}
			}

			m_nEditedItem = -1;
			this.editing = false;			
			this.textBox.Visible = false;
			this.button.Visible = false;
		}

		private void TextBox_LostFocus(object sender, System.EventArgs e)
		{
			PopupControlContainer popupContainer = PopupManager.ActivePopupClient as PopupControlContainer;

			if (!this.button.Focused && (popupContainer == null || !popupContainer.ContainsFocus))			
				this.EndEditing(true);
		}

		private void Button_LostFocus(object sender, EventArgs e)
		{
			if(!this.textBox.Focused)
				this.EndEditing(true);
		}

		private void ListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( !bIgnoreIndexChanging )
			{
				this.selIndexChanged = this.lastKnownSelIndex != this.ListBox.SelectedIndex;
				this.lastKnownSelIndex = this.ListBox.SelectedIndex;

				EndEditing( true );
			}
		}

		private void Button_Click(object sender, System.EventArgs e)
		{
			this.OnButtonClick(EventArgs.Empty);						
		}

		private void HandleListItemDraw(object sender, ListItemDrawEventArgs e)
		{
			this.OnListItemDraw(e);
		}

		protected virtual void OnListItemDraw(ListItemDrawEventArgs e)
		{
			if(this.ListItemDraw != null)
				this.ListItemDraw(this,e);
		}

		/// <summary>
		/// Raises the ItemChanging event.
		/// </summary>
		/// <param name="e">A <see cref="ListBoxTextChangingEventArgs"/> that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The OnItemChanging method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnItemChanging 
		/// in a derived class, be sure to call the base class's 
		/// OnItemChanging method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnItemChanging(ListBoxTextChangingEventArgs e)
		{
			if(this.ItemChanging != null)
				this.ItemChanging(this, e);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool RaiseBeforeListItemEdit()
		{
			return this.OnBeforeListItemEdit(new CancelEventArgs());
		}

		/// <summary>
		/// Raises the BeforeListItemEdit event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The OnBeforeListItemEdit method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnBeforeListItemEdit 
		/// in a derived class, be sure to call the base class's 
		/// OnBeforeListItemEdit method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual bool OnBeforeListItemEdit(CancelEventArgs e)
		{
			if(this.BeforeListItemEdit != null)
				this.BeforeListItemEdit(this,e);

			return e.Cancel;
		}


		/// <summary>
		/// Raises the ButtonClick event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks><para>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event.</para> <para>The OnButtonClick method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnButtonClick 
		/// in a derived class, be sure to call the base class's 
		/// OnButtonClick method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnButtonClick(EventArgs e)
		{
			this.EndEditing(true);

			if(this.ButtonClick != null)
			{
				this.ButtonClick(this, e);
			}
		}
	
		private bool GetIsMirrored()
		{
			return (RightToLeft.Yes == base.RightToLeft);
		}
	}
    /// <summary>
    /// FolderBrowser Designer
    /// </summary>
    public class EditableListDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public EditableListDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new EditableListActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
	#region CUSTOMLISTBOX
	[ToolboxItem(false)]
	internal class CustomListBox : ListBox
	{
		public ListItemDrawEventHandler ItemDraw;

		//Constructor
		public CustomListBox() : base()
		{	
			base.DrawMode = DrawMode.OwnerDrawFixed;
		}
		
		private TextAlignment m_textAlignment = TextAlignment.Left;

		[DefaultValue(TextAlignment.Left)]
		internal TextAlignment TextAlignment
		{
			get 
			{
				return m_textAlignment;
			}
			set
			{
				if( m_textAlignment != value ) 
				{
					m_textAlignment = value;
				}
			}
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem( e );

			if( !this.DesignMode )
			{
				Rectangle currentBounds = e.Bounds;
				e.DrawBackground();
				if( (e.State & DrawItemState.Selected) == DrawItemState.Selected )
					e.DrawFocusRectangle();
				bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

				ListItemDrawEventArgs args = new ListItemDrawEventArgs( selected, e.Index );
				if( this.OnItemDraw( args ) == true )
				{
					using( Brush brush = new SolidBrush( args.BackColor ) )
					{
						e.Graphics.FillRectangle( brush, e.Bounds );
					}
				}
				if( e.Index >= 0 && e.Index < this.Items.Count )
				{
					StringFormat sf = new StringFormat();
					if( GetIsMirrored() )
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}
					//This returns the actual values and not "System.Data.DataRowView".
					string sItemText = this.GetItemText( this.Items[e.Index] );

					SizeF strSize = e.Graphics.MeasureString( sItemText, e.Font, this.Width, sf );

					Graphics g = e.Graphics;
					Point location = Point.Empty;

					if( !GetIsMirrored() )
					{
						if( this.TextAlignment == TextAlignment.Left )
						{
							location = e.Bounds.Location;
						}
						else if( this.TextAlignment == TextAlignment.Right )
						{
							location = new Point( e.Bounds.Right - Size.Ceiling( strSize ).Width, e.Bounds.Y );
						}
						else
						{
							location = new Point( e.Bounds.Width / 2 - Size.Ceiling( strSize ).Width / 2, e.Bounds.Y );
						}
					}
					else
					{
						if( this.TextAlignment == TextAlignment.Left )
						{
							location = new Point( e.Bounds.Right, e.Bounds.Y );
						}
						else if( this.TextAlignment == TextAlignment.Right )
						{
							location = new Point( e.Bounds.Left + Size.Ceiling( strSize ).Width, e.Bounds.Y );
						}
						else
						{
							location = new Point( e.Bounds.Width / 2 + Size.Ceiling( strSize ).Width / 2, e.Bounds.Y );
						}
					}

					using( Brush brush = new SolidBrush( e.ForeColor ) )
					{
						g.DrawString( sItemText, e.Font, brush, location, sf );	
					}
                    sf.Dispose();
				}
			}
		}
        /// <summary>
        ///
        /// </summary>
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
             base.OnMeasureItem(e);
        }

		public virtual bool OnItemDraw(ListItemDrawEventArgs e)
		{
			if(ItemDraw != null)
				this.ItemDraw(this,e);

			return e.Handled;
		}
	
		protected bool GetIsMirrored()
		{
			return (RightToLeft.Yes == base.RightToLeft);
		}	
	}

	public delegate void ListItemDrawEventHandler(object sender, ListItemDrawEventArgs args);

	public class ListItemDrawEventArgs : EventArgs
	{
		private bool selected = false;
		private Color backColor = Control.DefaultBackColor;
		private bool handled = false;
		private int index;

		public ListItemDrawEventArgs(bool selected, int index)
		{
			this.selected = selected;
			this.index = index;
			this.handled = false;

		}

		public bool Selected
		{
			get
			{
				return this.selected;
			}
		}

		public Color BackColor
		{
			get
			{
				return this.backColor;
			}

			set
			{
				this.backColor = value;
			}

		}

		public int Index
		{
			get
			{
				return this.index;
			}

			set
			{
				this.index = value;
			}

		}

		public bool Handled
		{
			get
			{
				return this.handled;
			}

			set
			{
				this.handled = value;
			}
		}


	}
	#endregion
}
