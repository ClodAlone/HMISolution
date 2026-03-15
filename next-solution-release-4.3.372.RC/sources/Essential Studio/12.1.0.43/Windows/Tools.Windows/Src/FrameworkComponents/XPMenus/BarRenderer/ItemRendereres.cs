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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	#region ActivationNativeWindow

	public class ActivationNativeWindow: NativeWindow
	{
		#region Class members
		private Control m_control;
		#endregion

		#region Class events
		public event EventHandler LostFocus;
		public event EventHandler MouseActivated;
		#endregion

		#region Class Initialize/Finalize methods
		public ActivationNativeWindow( Control value )
		{
			if( value == null )
				throw new ArgumentNullException( "value" );

			m_control = value;
			AssignHandle( m_control.Handle );
		}
		#endregion

        public override void DestroyHandle()
        {
            base.DestroyHandle();
            m_control = null;
        }
		#region Class overrides
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_MOUSEACTIVATE || m.Msg == NativeMethods.WM_LBUTTONDOWN ||
				m.Msg == NativeMethods.WM_RBUTTONDOWN || m.Msg == NativeMethods.WM_MBUTTONDOWN ||
				m.Msg == NativeMethods.WM_LBUTTONDBLCLK || m.Msg == NativeMethods.WM_MBUTTONDBLCLK ||
				m.Msg == NativeMethods.WM_RBUTTONDBLCLK )
			{
				if( MouseActivated != null )
				{
					MouseActivated( this, EventArgs.Empty );
				}
			}
			else if( m.Msg == NativeMethods.WM_KILLFOCUS )
			{
				if( LostFocus != null )
				{
					LostFocus( this, EventArgs.Empty );
				}
			}
			base.WndProc( ref m );
		}
		#endregion
	}

	#endregion

	#region ListBoxContainer

	[ToolboxItem( false ), Syncfusion.Documentation.DocumentationExclude()]
	public class ListBoxContainer: PopupControlContainer
	{
		private ListBox listBox = null;
		private ListBox defaultListBox = null;
		public ListBoxContainer()
		{
			this.SetStyle( ControlStyles.Selectable, false );
			this.Size = new Size( 20, 20 );
			this.BorderStyle = BorderStyle.FixedSingle;

			this.ListBox = new GridComboBoxListBoxPart();
			this.defaultListBox = this.ListBox;
		}

		private bool m_bShouldHidePopup = true;

		private bool m_bCloseOnClick = true;

		protected internal bool CloseOnClick
		{
			get
			{
				return m_bCloseOnClick;
			}
			set
			{
				if( value != m_bCloseOnClick )
				{
					m_bCloseOnClick = value;
				}
			}
		}
        private bool bmouseup;
        internal bool Mouseup
        {
            get
            {
                return bmouseup;
            }
            set
            {
                if (value != bmouseup)
                {
                    bmouseup = value;
                }
            }
        }
		protected internal bool ShouldHidePopup
		{
			get
			{
				return m_bShouldHidePopup;
			}
			set
			{
				if( value != m_bShouldHidePopup )
				{
					m_bShouldHidePopup = value;
				}
			}
		}
		public override void ConfirmDeactivate()
		{
			if( m_bShouldHidePopup )
			{
				base.ConfirmDeactivate();
			}
		}
		protected internal virtual void AttachListBox()
		{
			if( listBox != null )
			{
				this.Controls.Add( listBox );

				listBox.BorderStyle = BorderStyle.None;
				listBox.Visible = true;

				listBox.Location = new Point( 0, 0 );
				if( listBox.Height < 10 || listBox.Items.Count == 0 )
				{
					listBox.Height = 10;
				}

				Height = listBox.Height + 1; // Need an additional pixel to show the whole listbox.
                listBox.MouseLeave += new EventHandler(listBox_MouseLeave);
				listBox.MouseUp += new MouseEventHandler( ListBoxMouseUp );
			}
		}
        void listBox_MouseLeave(object sender, EventArgs e)
        {
            Mouseup = false;
        }
		protected virtual void DetachListBox()
		{
			if( this.listBox != null )
			{
				this.listBox.MouseUp -= new MouseEventHandler( this.ListBoxMouseUp );
				this.listBox = null;
			}
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.defaultListBox != this.listBox )
				{
					// If this is not the defaultListBox then don't let it get disposed
					// as it could be reused in a different renderer.
					// It will get disposed in the client Form.
					this.Controls.Remove( this.listBox );
					this.defaultListBox = null;
				}
				this.DetachListBox();
			}
			base.Dispose( disposing );
		}
		protected override void WndProc( ref Message msg )
		{
			if( msg.Msg == 0x21/*WM_MOUSEACTIVATE*/)
			{
				msg.Result = (IntPtr)3; //MA_NOACTIVATE
				return;
			}
			base.WndProc( ref msg );
		}
		public ListBox ListBox
		{
			get { return this.listBox; }
			set
			{
				if( this.listBox != value )
				{
					this.DetachListBox();

					this.listBox = value;

					this.AttachListBox();
				}
			}
		}

		protected override void OnBeforePopup( CancelEventArgs args )
		{
			// Need this since the ListBox might get reused between renderers.
			this.AttachListBox();
			base.OnBeforePopup( args );
		}

		protected override bool ProcessMouseMessage( Control destination, int msg, IntPtr lParam, IntPtr wParam )
		{
			if( msg == NativeMethods.WM_MOUSEWHEEL )
			{
                int delta = 0;

                bool success = int.TryParse(wParam.ToString(), out delta);

                if (!success)
                    delta = 0;

                OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
			}

			if( !this.IgnoreMouseMessages )
			{
				if( !this.DesignMode
					&& ( msg == 0x200 /*WM_MOUSEMOVE*/|| msg == 0x2A3/*WM_MOUSELEAVE*/
					|| msg == 0x2A1/*WM_MOUSEHOVER*/) )
				{
					// If the destination is in the parent chain, let it pass through
					if( destination != null && this.IsRelatedControl( destination, true ) )
						return false;
					// else don't send it to the destination.
					else
						return true;
				}
				else
					this.VeryifyMouseBasedDeactivation( destination, msg );
			}
			return false;
		}
		private void ListBoxMouseUp( object sender, MouseEventArgs e )
		{
            Mouseup = true;
			if( m_bCloseOnClick )
			{
				this.HidePopup( PopupCloseType.Done );
			}
		}
	}

	#endregion

	#region EditableComboRenderer

	[Syncfusion.Documentation.DocumentationExclude()]
	public class EditableComboRenderer: ComboBoxItemRenderer
	{
		#region Data members

		private ComboTextBox m_textBox;
		private AutoAppend autoAppend;
		private bool forceHotTrack = false;
		private bool needLayout = true;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected internal bool HitTestInternal()
		{
			bool result = false;

			if( this.TextBox != null && this.TextBox.Parent != null )
			{
				Point mousePos = Control.MousePosition;
				result = this.TextBox.Bounds.Contains( this.TextBox.Parent.PointToClient( mousePos ) );
			}

			return result;

		}

		protected override void ListBox_LostFocus( object sender, EventArgs e )
		{
			int selIndex = listBoxContainer.ListBox.SelectedIndex;
			if( selIndex >= 0 && selIndex < listBoxContainer.ListBox.Items.Count )
			{
				this.TextBox.Text = GetItemText( listBoxContainer.ListBox.Items[selIndex] );
			}

			base.ListBox_LostFocus( sender, e );
		}

		protected override void OnEditPartKeyDown( object sender, KeyEventArgs e )
		{
			base.OnEditPartKeyDown( sender, e );

			string text = this.TextBox.Text;

			if( e.KeyCode == Keys.Enter )
			{
				if( this.listBoxContainer.ListBox.Items.IndexOf( text ) >= 0 )
				{
					this.listBoxContainer.ListBox.SelectedItem = this.TextBox.Text;
				}
				else
				{
					this.listBoxContainer.ListBox.SelectedIndex = -1;
				}
			}

			if( this.parent != null )
			{
				this.parent.SetHotTrack( this, true );
			}
		}

		#region INIT
		public EditableComboRenderer( IBarRenderer parent )
			: base( parent )
		{
			IgnoreExternalMouseMessages = false;
			this.CreateTextBox();
			this.listBoxContainer.ParentControl = this.TextBox;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				ComboBoxBarItem comboItem = this.BarItem as ComboBoxBarItem;
                if (comboItem.AutoAppend && !comboItem.PersistAutoAppendList)
                {
                    if (this.autoAppend != null)
                    {
                        this.autoAppend.SetAutoAppend(this.TextBox, new AutoAppendInfo(true, String.Empty, null, 30));
                    }
                }
                if (this.BarItem != null && this.BarItem.Manager != null
                        && this.BarItem.Manager.MainFrameBarManager != null)
                {
                    this.BarItem.Manager.MainFrameBarManager.CustomizationBegin -= new EventHandler(this.CustomizationStart);
                    this.BarItem.Manager.MainFrameBarManager.CustomizationDone -= new EventHandler(this.CustomizationDone);
                }
				if( comboItem != null )
				{
					comboItem.TextBoxBound -= new TextBoxBoundEventHandler( ComboBoxBarItemTextBoxBound );
				}

				if (m_textBox != null)
				{
					m_textBox.Dispose();
					m_textBox = null;
				}
				DisposeTextBox( this.TextBox );

				if( this.autoAppend != null )
				{
					this.autoAppend.Dispose();
					this.autoAppend = null;
				}
			}
			base.Dispose( disposing );
		}
		#endregion INIT

		#region CALLS_FROM_BARRENDERER
		public override BarItem BarItem
		{
			get
			{
				return base.BarItem;
			}
			set
			{
				if( base.BarItem != value )
				{
					if( this.BarItem != null && this.BarItem.Manager != null
						&& this.BarItem.Manager.MainFrameBarManager != null )
					{
						this.BarItem.Manager.MainFrameBarManager.CustomizationBegin -= new EventHandler( this.CustomizationStart );
						this.BarItem.Manager.MainFrameBarManager.CustomizationDone -= new EventHandler( this.CustomizationDone );
					}

					ComboBoxBarItem comboItem = this.BarItem as ComboBoxBarItem;

					if( comboItem != null )
					{
						comboItem.TextBoxBound -= new TextBoxBoundEventHandler( ComboBoxBarItemTextBoxBound );
					}

					base.BarItem = value;
					comboItem = value as ComboBoxBarItem;

					comboItem.TextBoxBound += new TextBoxBoundEventHandler( ComboBoxBarItemTextBoxBound );

					if( this.TextBox != null )
					{
						this.TextBox.Text = comboItem.TextBoxValue;
						if( this.BarItem != null )
						{
							this.TextBox.Visible = this.BarItem.Visible && this.BarItem.Enabled;

							comboItem.OnTextBoxBound( new TextBoxBoundEventArgs( this.TextBox ) );
						}
					}

					if( this.BarItem != null && this.BarItem.Manager != null
						&& this.BarItem.Manager.MainFrameBarManager != null )
					{
						this.BarItem.Manager.MainFrameBarManager.CustomizationBegin += new EventHandler( this.CustomizationStart );
						this.BarItem.Manager.MainFrameBarManager.CustomizationDone += new EventHandler( this.CustomizationDone );
					}
				}
			}
		}

		void ComboBoxBarItemTextBoxBound( object sender, TextBoxBoundEventArgs args )
		{
			this.ApplyAutoAppendSettings();
		}

		private void CustomizationStart( object sender, EventArgs e )
		{
			if( this.TextBox != null )
				this.TextBox.Visible = false;
		}
		private void CustomizationDone( object sender, EventArgs e )
		{
			if( this.TextBox != null )
				this.TextBox.Visible = ( this.BarItem == null || ( this.BarItem.Visible && this.BarItem.Enabled ) );
		}

		public override bool HotTrack
		{
			get { return base.HotTrack | this.forceHotTrack; }
			set { base.HotTrack = value; }
		}

		public override void BarItemPropertyChanged( Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "TextBoxValue" )
			{
                if (this.TextBox != null)
                {
                    this.TextBox.Text = this.ComboBoxBarItem.TextBoxValue;
                    if (this.ComboBoxBarItem.ChoiceList.Contains(this.TextBox.Text))
                    {
                        this.TextBox.SelectAll();
                    }
                }
			}
			else if( e.PropertyName == "AutoAppend" )
				this.ApplyAutoAppendSettings();
			else if( e.PropertyName == "Enabled" )
			{
				if( this.TextBox != null )
					this.TextBox.Visible = this.ComboBoxBarItem.Visible && this.ComboBoxBarItem.Enabled;
			}
			else if( e.PropertyName == "CustomTextFont" )
			{
				if( this.TextBox != null )
					this.TextBox.Font = this.BarItem.CustomTextFont;
			}

			base.BarItemPropertyChanged( e );
		}

		#region Overrides

		public override Control GetPopupParentControl()
		{
			return this.TextBox;
		}

		public override void AddTextPreferredSize( IGraphicsProvider gp, ref SizeF preferredSize )
		{
			base.AddTextPreferredSize( gp, ref preferredSize );

			if( this.TextBox != null && preferredSize.Height < this.TextBox.PreferredHeight )
			{
				preferredSize.Height = this.TextBox.PreferredHeight;
			}
		}

		public override bool IsRelatedControl( Control ctl )
		{
			bool bRelated = base.IsRelatedControl( ctl );

			if( !bRelated )
			{
				bRelated = ( ctl == this.listBoxContainer );// || (ctl == this.TextBox);
			}

			return bRelated;
		}

		internal override void OnActiveChanged()
		{
			if( base.Active )
			{
				this.Activated();
			}
			else
			{
				this.Deactivated();
			}
		}

		#endregion

		#endregion CALLS_FROM_BARRENDERER

		#region TEXTBOX

		// Passing the mouse-down to the renderer, doesn't seem to be good enough.
		public void MouseDownWithAlt( MouseEventArgs e )
		{
			Point pt = new Point( e.X, e.Y );
			pt = this.TextBox.PointToScreen( pt );
			pt = this.parent.GetBarControl().GetControl().PointToClient( pt );

			this.parent.OnMouseDown( new MouseEventArgs( e.Button, e.Clicks, pt.X, pt.Y, e.Delta ) );
		}
		public void HitReturn()
		{
			this.ComboBoxBarItem.TextBoxValue = this.TextBox.Text;
			if( this.ComboBoxBarItem.TextBoxValue != String.Empty )
			{
				if( this.autoAppend != null )
					this.autoAppend.InsertOrMoveToTop( this.TextBox, this.TextBox.Text );

				this.ComboBoxBarItem.PerformClick();

				if( this.listBoxContainer.IsShowing() )
				{
					if( this.listBoxContainer.ListBox.SelectedIndex != -1 )
					{
						this.TextBox.Text = this.listBoxContainer.ListBox.SelectedItem.ToString();
						this.TextBox.SelectAll();
					}

					this.listBoxContainer.HidePopup( PopupCloseType.Canceled );
				}
			}
			else
				this.HitEscape();
		}
		public void HitEscape()
		{
			if( this.listBoxContainer.IsShowing() )
				this.listBoxContainer.HidePopup( PopupCloseType.Canceled );

			this.RemoveFocus();
		}
		public virtual void OnDoubleClick()
		{

		}

		internal void UpdateTextBoxValue()
		{
			this.ComboBoxBarItem.TextBoxValue = this.TextBox.Text;
		}
		internal void TextBoxValueChanged()
		{
			if( this.listBoxContainer.IsShowing() )
			{
				if( this.listBoxContainer.ListBox.Items.IndexOf( this.TextBox.Text ) != -1 )
					this.listBoxContainer.ListBox.SelectedItem = this.TextBox.Text;
				else
					this.listBoxContainer.ListBox.SelectedIndex = -1;
			}
		}
		private void RemoveFocus()
		{
            // 1) When the user hits Escape or when this item gets programmatically deactivated,
			// the focus gets set to the previous contorl with focus.
			// 2) When the user selects a different comboboxbaritem, first this routine
			// will force the focus back to the previous control with focus and then
			// the new comboboxbaritem will take the focus back from the prev-control.
			// This way the prev control will never be a comboboxbaritem.
			if( this.TextBox != null )
			{
				Control prevControl = this.TextBox.prevControlWithFocus;

				if( ( prevControl != null && prevControl.IsHandleCreated ) )
				{
					Form activeForm = Form.ActiveForm;
					Control prevControlForm = prevControl.FindForm();
					if( activeForm == null || prevControlForm == activeForm
						// Mdi scenario
						|| activeForm.ActiveMdiChild == prevControlForm )
					{
						prevControl.Focus();
						// Clear this setting now because sometimes the above call will
						// set the focus back on this TextBox when there are no other controls
						// on the form and that will prevent the user from selecting other
						// comboboxbaritems in the toolbar.
						this.TextBox.prevControlWithFocus = null;
					}
				}
                else if (this.TextBox.prevControlWithFocus == null)
                {
                    prevControl = this.TextBox.FindForm();
                    if (prevControl != null)
                    {
                        prevControl.Focus();
                        this.TextBox.prevControlWithFocus = null;
                    }
                }
			}
		}
		protected override Rectangle TextAreaBounds
		{
			get
			{
				Rectangle rectText = base.TextAreaBounds;
				// If textbox is not null, use the textbox's preferred height.
				if( this.TextBox != null )
				{
					if( rectText.Width > 0 && rectText.Height > this.TextBox.PreferredHeight )
					{
						rectText.Y = rectText.Y + ( rectText.Height - this.TextBox.PreferredHeight ) / 2;
						rectText.Height = this.TextBox.PreferredHeight;
					}

					rectText.Width -= DEF_BORDER_OFFSET;
				}
				return rectText;
			}
		}

		protected virtual void CreateTextBox()
		{
			if( this.TextBox != null )
				return;

			if( !this.DesignMode )
			{
				ComboTextBox textBox = new ComboTextBox( this );
				Control parentCtl = this.parent.GetBarControl().GetControl();

				parentCtl.Controls.Add( textBox );

				textBox.Visible = false;
				textBox.KeyDown += new KeyEventHandler( this.OnEditPartKeyDown );

				m_textBox = textBox;
				m_textBox.Font = parentCtl.Font;

				if( parentCtl.IsHandleCreated )
				{
					DefferedTextBoxBinding( parentCtl );
				}

				parentCtl.HandleCreated += new EventHandler( textBox_HandleCreated );
			}
		}

		private void textBox_HandleCreated( object sender, EventArgs e )
		{
			DefferedTextBoxBinding( sender );
		}

		private void DefferedTextBoxBinding( object sender )
		{
			ComboTextBox textBox = m_textBox;

			if(textBox != null)
			textBox.Parent.HandleCreated -= new EventHandler( textBox_HandleCreated );
			m_textBox = null;

			DefferedTextBoxAssignmentDelegate deffer = new DefferedTextBoxAssignmentDelegate( DefferedTextBoxAssignment );
			Control ctl = sender as Control;

			ctl.BeginInvoke( deffer, new object[] { textBox } );
		}

		private delegate void DefferedTextBoxAssignmentDelegate( ComboTextBox textBox );

		void DefferedTextBoxAssignment( ComboTextBox textBox )
		{
            if (textBox != null)
            {
                if (this.IsDisposed)
                {
                    DisposeTextBox(textBox);
                }
                else
                {
                    IRequiresControl reqCtl = this.BarItem as IRequiresControl;

                    if (reqCtl != null)
                    {
                        textBox.Text = reqCtl.Value.ToString();
                    }

                    this.TextBox = textBox;

                    if (textBox != null)
                    {
                        this.TextBox.Bounds = this.TextAreaBounds;
                    }
                }
            }
		}

		private void DisposeTextBox( ComboTextBox textBox )
		{
			if( textBox != null )
			{
				textBox.Parent.HandleCreated -= new EventHandler( textBox_HandleCreated );
				textBox.KeyDown -= new KeyEventHandler( this.OnEditPartKeyDown );

				Control parent = textBox.Parent;

				if( null != parent && parent.IsHandleCreated )
				{
					parent.SuspendLayout();
					parent.Controls.Remove( textBox );
					parent.ResumeLayout( false );
				}

				textBox.Dispose();	// Dispose() automatically removes control from parent if necessary.
				textBox = null;
			}
		}

		protected virtual void ApplyAutoAppendSettings()
		{
			if( this.DesignMode )
				return;

			if( this.ComboBoxBarItem.AutoAppend)
			{
				if( this.autoAppend == null )
					this.autoAppend = new AutoAppend();
				BarItemID barItemID = new BarItemID( this.ComboBoxBarItem.ID,
				  this.ComboBoxBarItem.Manager != null ?
				  BarManager.GetFormTypeName( this.ComboBoxBarItem.Manager ) : String.Empty );
				IList appendList = this.IsCustomListBox ? (IList)this.ComboBoxBarItem.ListBox.Items : (IList)this.ComboBoxBarItem.ChoiceList;
				this.autoAppend.SetAutoAppend( this.TextBox, new AutoAppendInfo( true, barItemID.ToString(), appendList, 30 ) );
				this.ComboBoxBarItem.TextBoxValue = this.TextBox.Text;
			}
			else
			{
				if( this.autoAppend != null )
					this.autoAppend.SetAutoAppend( this.TextBox, new AutoAppendInfo( false, String.Empty, null, 30 ) );
			}
		}

		#region LAYOUT
		public override RectangleF Bounds
		{
			get { return base.Bounds; }
			set
			{
				if( this.Bounds != value )
				{
					base.Bounds = value;
					if( this.TextBox != null )
					{
						if( !this.TextBox.Visible )
							this.TextBox.Bounds = this.TextAreaBounds;
						else
							// Don't set this immediately if already visible. Wait till Paint is hit.
							this.needLayout = true;
					}
				}
			}
		}

		public override void OnPaint( Graphics g, Rectangle clippingRect )
		{
			if( this.needLayout )
			{
				if( this.TextBox != null )
					this.TextBox.Bounds = this.TextAreaBounds;
				this.needLayout = false;
			}

			base.OnPaint( g, clippingRect );
		}
		#endregion LAYOUT

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;
				if( this.TextBox != null )
					this.TextBox.Visible = this.Visible && !this.parent.Customizing
					  && ( this.BarItem == null || this.BarItem.Enabled );
			}
		}

		internal protected ComboTextBox TextBox
		{
			get
			{
				return m_textBox;
			}
			set
			{
				if( value != this.TextBox )
				{
					m_textBox = value;
					m_textBox.Font = this.parent.GetBarControl().GetControl().Font;

					if( null != m_textBox )
					{
						ComboBoxBarItem comboItem = this.BarItem as ComboBoxBarItem;

						if( null != comboItem )
						{
							comboItem.OnTextBoxBound( new TextBoxBoundEventArgs( this.TextBox ) );
						}
					}
				}
			}
		}

		protected internal void TextBoxGotFocus()
		{
			this.forceHotTrack = false;
            if (this.parent != null)
            {
                BarRenderer br1 = (this.parent as BarRenderer);
                if (br1 != null)
                {
                    foreach (BarItemRenderer br in br1.barItemRenderers)
                    {
                        if (br != null)
                            br.Active = false;
                    }
                }
            }
			this.Active = true;
            this.parent.SetHotTrack(this, this.Active);
        }

		protected internal void TextBoxLostFocus( string sText )
		{
            if(sText != string.Empty)
                this.ComboBoxBarItem.TextBoxValue = sText;
			this.forceHotTrack = false;

			if( this.listBoxContainer.TopLevelControl == null
				|| !this.listBoxContainer.TopLevelControl.ContainsFocus )
			{
				this.Active = false;
				this.parent.SetHotTrack( this, false );
			}

			this.Repaint();
		}
		protected internal void TextBoxMouseEnter()
		{
            if (this.Active)
                this.parent.SetHotTrack(this, true);
            else
            {
                this.HotTrack = true;
            }
		}
		protected internal void TextBoxMouseLeave()
		{
			if( !this.TextBox.Focused && !this.listBoxContainer.IsShowing() )
				this.parent.SetHotTrack( this, false );
		}
		#endregion TEXTBOX

		#region MOUSE_PROCESSING
		public override void OnMouseDown( Point pointMouseDown )
		{
			// Called because mouse was pressed within my bounds
			if( !this.BarItem.Enabled && !this.parent.Customizing )
				return;

			if( !this.ShowingDropDown )
			{
				this.Active = true;

				if( !this.parent.Customizing )
				{
					pointMouseDown = this.OffsetPointByDropDownArea( pointMouseDown );
					if( !this.Bounds.Contains( pointMouseDown ) )
					{
						this.ShowingDropDown = true;
					}
				}
			}
			else
				this.ShowingDropDown = false;
		}
		#endregion MOUSE_PROCESSING

		#region TEXTBOX_AND_DROPDOWN
		public override void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			forceHotTrack = true;
			this.popupChild = null;

			if( popupCloseType == PopupCloseType.Done )
			{
				// Praveen: Our MenuGridControlBase.ModelSaveCellInfo method similarly updates the combobaritem via the
				// IRequiresControl interface, shouldn't a similar change happen there?

				// set object to Value property correctly and update Text
				// Praveen: This will fire the PropertyChanged event indicating the TextBoxValue has changed.
				// FIX: alexk
				//(( IRequiresControl )this.ComboBoxBarItem).Value = this.listBoxContainer.ListBox.SelectedItem;
				// Praveen: This will again fire the PropertyChanged event. Firing the event twice doesn't seem right.
				this.ComboBoxBarItem.TextBoxValue = this.GetListBoxText();

				this.TextBox.SelectAll();
				if( this.autoAppend != null )
					this.autoAppend.InsertOrMoveToTop( this.TextBox, this.TextBox.Text );
				this.ComboBoxBarItem.PerformClick();
			}

			this.ShowingDropDown = false;
		}
		public override void AfterChildClosing()
		{
			if( !this.IsDisposed )
			{
				if( !this.parent.IsShowingDropdown() )
				{
					this.parent.StopKeyboardNavigation();

					// Set the focus back on the text box if necessary.
					Form activeForm = Form.ActiveForm;
					Control controlForm = this.TextBox.FindForm();
					if(this.Active &&( activeForm != null || controlForm == activeForm)
						// Mdi scenario
						|| ( activeForm != null && activeForm.ActiveMdiChild == controlForm ) )
					{
						this.TextBox.Focus();
					}
				}
			}
		}

		protected virtual void Activated()
		{
			if( this.TextBox != null && !this.parent.Customizing )
			{
				if( !this.TextBox.Focused )
				{
					this.TextBox.StorePrevFocus();

                    if( !( this.TextBox.FindForm() is CommandBarForm ) || !(this.TextBox.FindForm() is FloatingForm))
					{
						this.TextBox.Focus();
					}
				}

				this.TextBox.SelectAll();
			}
		}
		protected virtual void Deactivated()
		{
			if( !this.parent.Customizing && !this.IsShowing() )
				this.RemoveFocus();
		}
		#endregion TEXTBOX_AND_DROPDOWN
	}

	#endregion

	#region ComboBoxItemRenderer

	[Syncfusion.Documentation.DocumentationExclude()]
	public class ComboBoxItemRenderer: BarItemRenderer, IDropDownItem
	{
		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				RectangleF newValue = value;

				int y = (int)newValue.Top;
				newValue.Y = y;

				base.Bounds = newValue;

			}
		}

		protected IPopupChild popupChild;
		protected IPopupParent popupParent;

		internal static int ComboBoxDropDownWidth = 12;
		protected ListBoxContainer listBoxContainer = null;

		private SyncfusionPropertyChangedEventHandler m_PropertyChangedHandler = null;

		public ComboBoxItemRenderer( IBarRenderer parent )
			: base( parent )
		{
			m_bIgnoreExternalMouseMessages = true;
			this.popupParent = parent;
            IgnoreExternalMouseMessages = false;
			this.listBoxContainer = new ListBoxContainer();
			this.listBoxContainer.PopupParent = this.popupParent;
			this.listBoxContainer.MouseWheel += new MouseEventHandler( listBoxContainer_MouseWheel );

			m_PropertyChangedHandler = new SyncfusionPropertyChangedEventHandler( BarItem_PropertyChanged );
		}

		public ListBox ListBox
		{
			get { return this.listBoxContainer.ListBox; }
		}

		private bool m_bIgnoreExternalMouseMessages = false;

		protected internal bool IgnoreExternalMouseMessages
		{
			get
			{
				return m_bIgnoreExternalMouseMessages;
			}
			set
			{
				if( value != m_bIgnoreExternalMouseMessages )
				{
					m_bIgnoreExternalMouseMessages = value;
				}
			}
		}

		ActivationNativeWindow m_lstNativeWnd = null;

		#region Properties

		public override BarItem BarItem
		{
			get { return base.BarItem; }
			set
			{
				// This will get called only once during the lifetime of the renderer.
				if( base.BarItem != value )
				{
					if( null != base.BarItem )
					{
						base.BarItem.PropertyChanged -= m_PropertyChangedHandler;
					}

					base.BarItem = value;

					if( null != base.BarItem )
					{
						base.BarItem.PropertyChanged += m_PropertyChangedHandler;
						this.listBoxContainer.RightToLeft = GetIsMirrored() ? RightToLeft.Yes : RightToLeft.No;
					}

					if( this.ComboBoxBarItem.ListBox != null
						&& !this.ComboBoxBarItem.DesignMode )
					{
						this.listBoxContainer.ListBox = this.ComboBoxBarItem.ListBox;
						m_lstNativeWnd = new ActivationNativeWindow( this.listBoxContainer.ListBox );
						m_lstNativeWnd.MouseActivated += new EventHandler( m_lstNativeWnd_MouseActivated );
						m_lstNativeWnd.LostFocus += new EventHandler( ListBox_LostFocus );
					}

					this.listBoxContainer.ListBox.KeyDown += new KeyEventHandler( OnEditPartKeyDown );
				}
			}
		}

		#endregion

		public override void DrawSeparator( Graphics g )
		{
			IBarControl ibcBarCtl = this.parent.GetBarControl();
			Control barCtl = ibcBarCtl.GetControl();

			if( null != barCtl )
			{
				GraphicsProvider gp = new GraphicsProvider( barCtl );

				SizeF sizeBase = base.GetPreferredSize( gp );
				SizeF size = this.Bounds.Size;

				if( sizeBase.Height > size.Height )
				{
					size = sizeBase;
				}

				RectangleF parentBounds = this.parent.Bounds;
				PointF location = this.Bounds.Location;
				float parentHeight = parentBounds.Height;

				location.Y = parentBounds.Top + ( parentHeight - size.Height ) / 2.0f;

				RectangleF bounds = new RectangleF( location, size );

				DrawSeparator( g, bounds );
			}
			else
			{
				base.DrawSeparator( g, this.Bounds );
			}
		}

		protected bool IsCustomListBox
		{
			get { return this.ComboBoxBarItem != null && this.ComboBoxBarItem.ListBox != null; }
		}

		protected bool GetIsMirrored()
		{
			return this.IsRTL;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.listBoxContainer != null )
				{
					this.listBoxContainer.ListBox.KeyDown -= new KeyEventHandler( OnEditPartKeyDown );
                    this.listBoxContainer.MouseWheel -= new MouseEventHandler(listBoxContainer_MouseWheel);
					this.listBoxContainer.Dispose();
					this.listBoxContainer = null;
				}
				if(this.BarItem !=null )
					this.BarItem.PropertyChanged -= m_PropertyChangedHandler;
                this.popupParent = null;

				if( m_lstNativeWnd != null )
				{
                    m_lstNativeWnd.MouseActivated -= new EventHandler(m_lstNativeWnd_MouseActivated);
                    m_lstNativeWnd.LostFocus -= new EventHandler(ListBox_LostFocus);
					m_lstNativeWnd.ReleaseHandle();
				}
			}
			base.Dispose( disposing );
		}

		public override void BarItemPropertyChanged( Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e )
		{
			base.BarItemPropertyChanged( e );
			if( e.PropertyName == "Editable"
			  || e.PropertyName == "ListBox" )
			{
				IBarRenderer cachedParent = this.parent;
				this.parent.RemoveRenderer( this );
				cachedParent.UpdateRenderers();
			}
		}
		
		protected ComboBoxBarItem ComboBoxBarItem
		{
			get { return this.BarItem as ComboBoxBarItem; }
		}

		#region Overrides

		/// <summary>
		/// Process keyboard messages.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override bool ProcessKeyDown( Keys key )
		{
			OnEditPartKeyDown( this, new KeyEventArgs( key ) );

			return base.ProcessKeyDown( key );
		}

		public override bool ShouldDrawText()
		{
			if( ( this.BarItem != null &&
			  this.BarItem.PaintStyle != PaintStyle.Default &&
			  this.BarItem.PaintStyle != PaintStyle.TextOnlyInMenus ) )
				return true;
			else
				return false;
		}

		internal override void OnActiveChanged()
		{
			if( this.Active )
			{
				this.parent.GetBarControl().GetControl().Select();
			}
		}

		#endregion

		/// <summary>
		///	Used to correct customization selection rectangle for ComboDropDown items.
		/// </summary>
		protected internal const int DEF_SELECTION_OFFSET = 2;

		/// <summary>
		/// Used due to ComboDropDown item should be drawn with little left and right margins.
		/// </summary>
		protected internal const int DEF_BORDER_OFFSET = 3;

		private const int DEF_MIN_HEIGHT = 22;

		public override SizeF GetPreferredSize( IGraphicsProvider gp )
		{
			SizeF preferredSize = SizeF.Empty;
			this.AddTextPreferredSize( gp, ref preferredSize );

			int minTextHeight = DEF_MIN_HEIGHT - BarItemRenderer.PadY * 2;

			ProvideFontInfoEventArgs args = new ProvideFontInfoEventArgs(this.parent.GetBarControl().GetControl().Font);

			this.BarItem.OnProvideFontInfo(args);

			Size fontSz = MeasureText(gp.Graphics, WinFormsUtils.MeasureEmptyCellString, args.Font);

			if (minTextHeight < fontSz.Height)
			{
				minTextHeight = fontSz.Height;
			}

			if (preferredSize.Height < minTextHeight)
			{
				preferredSize.Height = minTextHeight;
			}

			preferredSize.Width += this.ComboBoxBarItem.MinWidth;
			preferredSize.Height += ( BarItemRenderer.PadY * 2 );

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
				&& this.Style != VisualStyle.Office2007
				&& this.Style != VisualStyle.Office2007Outlook
                && this.Style != VisualStyle.Office2010)
			{
				preferredSize.Width += this.PaddingForThemesX;
				preferredSize.Height += this.PaddingForThemesY;
			}

			preferredSize.Width += DEF_BORDER_OFFSET * 2;

			return preferredSize;
		}

		protected Point OffsetPointByDropDownArea( Point point )
		{
			// Adjust the hit point so that the Contains check below will be true only if hit
			// outside the drop-down area

			if( GetIsMirrored() )
			{
				point.X = (int)this.Bounds.Right - point.X - this.GetDropDownAreaWidth() - PadX;
			}
			else
			{
				point.X += this.GetDropDownAreaWidth() + PadX + DEF_BORDER_OFFSET * 2;
			}

			return point;
		}

		#region DROPDOWN
		public bool IsShowing()
		{
			if( this.popupChild != null
			  && this.popupChild.IsShowing() )
				return true;
			else
				return false;
		}
		protected override bool ShowDropDown( Queue pbiQueue )
		{
			if( this.IsShowing() )
				return true;

			if( !this.parent.Customizing )
			{
				ListBox listBox = this.listBoxContainer.ListBox;

				if( listBox != null )
				{
					this.popupChild = this.listBoxContainer;

					MainFrameBarManager barMan = this.GetMainManager();
					if( barMan != null )
					{
						barMan.ShouldHidePopup = true;
					}

					if( listBoxContainer.PopupHost == null )
					{
						this.listBoxContainer.EnsurePopupHost();
					}

					this.listBoxContainer.CloseOnClick = this.ComboBoxBarItem.CloseOnClick;
					this.listBoxContainer.PopupHost.IgnoreWorkingArea = this.ComboBoxBarItem.IgnoreWorkingArea;

					this.InitChoiceList();
					this.listBoxContainer.ShowPopup( Point.Empty );

					ComboBoxBarItem item = this.ComboBoxBarItem;
					if( item != null )
					{
						item.OnDropDownOpened();
					}

					return true;
				}
				else
				{
					return false;
				}
			}
			else
				return false;
		}

		protected virtual void InitChoiceList()
		{
			ListBox lb = this.listBoxContainer.ListBox;

			if( !this.IsCustomListBox )
			{
				lb.Items.Clear();
				foreach( string choice in this.ComboBoxBarItem.ChoiceList )
				{
					lb.Items.Add( choice );
				}
			}
			else
			{
				Object ds = lb.DataSource;
				if( ds != null )
				{
					BindingContext bc = lb.BindingContext;
					if( bc != null )
					{
						CurrencyManager cm = bc[ds] as CurrencyManager;
						if( cm != null )
						{
							cm.Refresh();
						}
					}
				}
			}

			// Provide the user a	chance to add/remove items into	the	listbox.
			this.ComboBoxBarItem.OnInitListBox( new ComboBoxBarItemInitListBoxEventArgs( lb ) );

			GridComboBoxListBoxPart dropDown = lb as GridComboBoxListBoxPart;
			if( dropDown != null )
			{
				dropDown.DropDownRows = this.ComboBoxBarItem.MaxDropDownItems;
			}

			// FIX: by Alexk
			//this.listBoxContainer.ListBox.SelectedItem = (( IRequiresControl )this.ComboBoxBarItem).Value;
			lb.SelectedItem = this.ComboBoxBarItem.TextBoxValue;

			if( this.ComboBoxBarItem.MinDropDownWidth < this.ComboBoxBarItem.MinWidth )
				lb.Width = this.ComboBoxBarItem.MinWidth;
			else
				lb.Width = this.ComboBoxBarItem.MinDropDownWidth;

			lb.Width -= DEF_BORDER_OFFSET;

			if( this.ComboBoxBarItem.MaxDropDownItems <= lb.Items.Count )
			{
				int listHeight = 0;
				if( lb.DrawMode != DrawMode.OwnerDrawVariable )
				{
					listHeight = this.ComboBoxBarItem.MaxDropDownItems * lb.ItemHeight;
				}
				else
				{
					for( int i = 0; i < this.ComboBoxBarItem.MaxDropDownItems; i++ )
					{
						listHeight += lb.GetItemHeight( i );
					}
				}
				lb.Height = listHeight;
			}
			else
			{
				if( lb.Items.Count > 0 )
				{
					lb.Height = lb.PreferredHeight;
				}
				else
				{
					// This is good when the listbox is databound but shown the first time (when ItemCount == 0)
					// (When databound the items will be initialized only when shown the first time)
					// But not good when there are really no items.
					int count = this.ComboBoxBarItem.MaxDropDownItems;
					IList dataSource = this.ListBox.DataSource as IList;
					if( dataSource != null )
					{
						count = dataSource.Count;
					}
					else
					{
						IListSource listSource = this.ListBox.DataSource as IListSource;
						if( listSource != null )
						{
							count = listSource.GetList().Count;
						}
					}

					this.ListBox.Height = Math.Min( count, this.ComboBoxBarItem.MaxDropDownItems ) * this.ListBox.ItemHeight;
				}
			}

			this.listBoxContainer.Size = new Size( lb.Width + 2, lb.Height + 1 );
		}

		protected override void HideDropDown()
		{
			if( this.popupChild != null )
				this.popupChild.HidePopup( PopupCloseType.Canceled );

			ComboBoxBarItem item = this.ComboBoxBarItem;
			if( item != null )
			{
				item.OnDropDownClosed();
			}
		}
		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			this.popupChild = null;
			this.ShowingDropDown = false;
			if( popupCloseType == PopupCloseType.Done )
			{
				this.ComboBoxBarItem.TextBoxValue = this.GetListBoxText();
				this.BarItem.PerformClick();
			}
		}
		#region DATABINDING_INLIST
		protected virtual string GetListBoxText()
		{
			object selItem = this.listBoxContainer.ListBox.SelectedItem;
			if( selItem != null )
				return this.GetItemText( selItem );
			else
				return String.Empty;
		}
		public string GetItemText( object item )
		{
			if( this.ListBox.DataSource != null &&
			  this.ListBox.DisplayMember != null )
			{
				item = this.FilterItemOnProperty( item, this.ListBox.DisplayMember );
				if( item == null )
					return "";
			}
			return Convert.ToString( item, CultureInfo.CurrentCulture );
		}
		protected object FilterItemOnProperty( object item, string field )
		{
			PropertyDescriptor pd;

			if( item != null && field.Length > 0 )
			{
				try
				{
					if( this.ListBox.DataSource != null )
					{
						CurrencyManager cm = (CurrencyManager)this.ListBox.BindingContext[this.ListBox.DataSource, new BindingMemberInfo( this.ListBox.DisplayMember ).BindingPath];
						pd = cm.GetItemProperties().Find( field, true );
					}
					else
						pd = TypeDescriptor.GetProperties( item ).Find( field, true );
					if( pd != null )
						item = pd.GetValue( item );
				}
				catch( Exception ) { }
			}
			return item;
		}
		#endregion DATABINDING_INLIST
		public virtual void AfterChildClosing()
		{
		}
		Point IDropDownItem.GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
		  out PopupRelativeAlignment newAlign )
		{
			Control parentControl = this.parent.GetBarControl().GetControl();
			Rectangle rectBounds = Rectangle.Ceiling( this.Bounds );
			// Lose the label's width
			if( rectBounds.Width > this.ComboBoxBarItem.MinWidth )
			{
				rectBounds.Width = this.ComboBoxBarItem.MinWidth;
				rectBounds.X += (int)this.Bounds.Width - rectBounds.Width;
			}
			rectBounds.Height += 1;

			Point pos = PopupUtils.ComputeDefaultPopupAlignment( prevAlign, out newAlign,
			  this.GetFirstAlignPreference(), PopupRelativeAlignment.RightTop, rectBounds );

			newAlign = PopupRelativeAlignment.Default;
			pos = parentControl.PointToScreen( pos );

			return pos;
		}
		public virtual Control GetPopupParentControl()
		{
			return null;
		}
		protected PopupRelativeAlignment GetFirstAlignPreference()
		{
			switch( this.parent.Alignment )
			{
				default:
				case CommandBarDockState.Top:
				case CommandBarDockState.Float:
				return PopupRelativeAlignment.BottomLeft;
				case CommandBarDockState.Left:
				return PopupRelativeAlignment.RightBottom;
				case CommandBarDockState.Bottom:
				return PopupRelativeAlignment.TopRight;
				case CommandBarDockState.Right:
				return PopupRelativeAlignment.LeftBottom;
			}
		}
		Point[] IDropDownItem.GetBorderOverlapCue( PopupRelativeAlignment rAlign )
		{
			Control parentControl = this.parent.GetBarControl().GetControl();

			Rectangle bounds = parentControl.RectangleToScreen( Rectangle.Ceiling( this.Bounds ) );

			return PopupUtils.ComputeDefaultBorderOverlapCue( rAlign, bounds );
		}

		public virtual bool IsRelatedControl( Control ctl )
		{
			return ( ctl == this.ListBox );
		}

		#endregion DROPDOWN

		#region MOUSE_AND_KEYS_HANDLING
		public override void OnMouseDown( Point pointMouseDown )
		{
			// Called because mouse was pressed within my bounds
			if( !this.BarItem.Enabled && !this.parent.Customizing )
				return;

			// If clicked in the label area, do nothing.
			//      if(this.Bounds.Contains(new Point(pointMouseDown.X + this.ComboBoxBarItem.MinWidth, pointMouseDown.Y)))
			//        return;

			if( !this.ShowingDropDown )
			{
				this.Active = true;
				this.ShowingDropDown = true;
				this.parent.GetBarControl().GetControl().Focus();
			}
			else
			{
				this.ShowingDropDown = false;
			}
		}
		public override void OnMouseUp( Point pointMouseUp )
		{
		}
		public override void OnMouseMove( Point pointMouseMove )
		{
		}
		public override void OnMouseWheel( bool isUp )
		{
            ComboBoxItemRenderer comboRenderer = this as ComboBoxItemRenderer;
            if (!comboRenderer.ShowingDropDown && comboRenderer.Active)
            {
                // Make sure to sync the item's choice list with the list box choice list.
                this.InitChoiceList();

			if( this.listBoxContainer.ListBox.Items.Count == 0 ) return;

			int curSel = this.listBoxContainer.ListBox.SelectedIndex;

			if( isUp ) curSel--; else curSel++;

			if( curSel != this.listBoxContainer.ListBox.SelectedIndex )
			{
				if( curSel > this.listBoxContainer.ListBox.Items.Count - 1 )
					curSel = this.listBoxContainer.ListBox.Items.Count - 1;
				if( curSel < 0 ) curSel = 0;

				//this.listBoxContainer.ListBox.SetSelected( curSel, true );

				// FIX: by alexk
				//(( IRequiresControl )this.ComboBoxBarItem).Value = this.listBoxContainer.ListBox.Items[ curSel ];

				this.ComboBoxBarItem.TextBoxValue = GetItemText( this.ListBox.Items[curSel] );
				this.ListBox.SelectedIndex = curSel;
                }
			}
		}

		protected virtual void OnEditPartKeyDown( object sender, KeyEventArgs e )
		{
			int curSel = -1;
			int linesVisible = 0;

			// FIX: by alexk - show/hide dropdown on Alt+ArrowDown; on Enter key 
			// perform Click event for ComboBoxBarItem
			if( e.Alt && e.KeyCode == Keys.Down )
			{
				this.ShowingDropDown = !this.ShowingDropDown;

				if( !this.ShowingDropDown )
				{
					this.Active = true;
				}
				return;
			}
			else if( e.KeyCode == Keys.Enter )
			{
				this.ComboBoxBarItem.TextBoxValue = (sender as TextBox).Text;
				this.ComboBoxBarItem.PerformClick();
			}
			else if( e.KeyCode == Keys.Down || e.KeyCode == Keys.Up
			  || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp )
			{
				// Make sure to sync the item's choice list with the list box choice list.
				if( !this.IsShowing() )
				{
					this.InitChoiceList();
				}
				if( this.listBoxContainer.ListBox.Items.Count == 0 )
					return;

				curSel = this.listBoxContainer.ListBox.SelectedIndex;
				linesVisible = this.listBoxContainer.ListBox.Height / this.listBoxContainer.ListBox.GetItemHeight( 0 ) - 1;
			}

			e.Handled = true;

			switch( e.KeyCode )
			{
				case Keys.Down:
				curSel++;
				break;
				case Keys.Up:
				curSel = curSel - 1;
				break;
				case Keys.PageDown:
				curSel += linesVisible;
				break;
				case Keys.PageUp:
				curSel = curSel - linesVisible;
				break;
				default:
				e.Handled = false;
				break;
			}

			if( e.Handled && curSel != this.listBoxContainer.ListBox.SelectedIndex )
			{
				if( curSel > this.listBoxContainer.ListBox.Items.Count - 1 )
					curSel = this.listBoxContainer.ListBox.Items.Count - 1;
				if( curSel < 0 )
					curSel = 0;
				this.listBoxContainer.ListBox.SetSelected( curSel, true );

				// FIX: by alexk
				//(( IRequiresControl )this.ComboBoxBarItem).Value = this.listBoxContainer.ListBox.Items[ curSel ];

				this.ComboBoxBarItem.TextBoxValue = GetItemText( this.ListBox.Items[curSel] );
				this.ListBox.SelectedIndex = curSel;
			}
		}
		#endregion MOUSE_AND_KEYS_HANDLING

		#region DRAWING
		public static float GetDropDownArrowWidth(bool resizeToFit, Rectangle rect)
		{
			float ddwidth = 5f;
			if (resizeToFit)
				ddwidth = ddwidth * rect.Width;
			else
				ddwidth = ddwidth * (SystemInformation.MenuCheckSize.Width);
			ddwidth = ddwidth / 13f;
			int iddwidth = (int)ddwidth;
			if ((iddwidth % 2) == 0)
				ddwidth++;
			return ddwidth;
		}			
		public static Point[] GetDropDownBorderBounds(bool resizeToFit,Rectangle btnBounds)
		{
			if (resizeToFit)
			{
				int arWidth = (int)ComboBoxItemRenderer.GetDropDownArrowWidth(resizeToFit, btnBounds);
				int arHeight = (int)ComboBoxItemRenderer.GetDropDownArrowHeight(resizeToFit, btnBounds);
				return ComboBoxItemRenderer.GetDropDownBorderBounds(btnBounds, arWidth, arHeight);
			}
			else
				return ComboBoxItemRenderer.GetDropDownBorderBounds(btnBounds);
		}
		public static float GetDropDownArrowHeight(bool resizeToFit, Rectangle rect)
		{
			return ((int)ComboBoxItemRenderer.GetDropDownArrowWidth(resizeToFit, rect)) / 2 + 1;
		}
		public static float GetDropDownArrowWidth()
		{
			float ddwidth = 5f *
			  ( SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the checkboxes.*/);
			// ddwidth cannot be an even no.
			int iddwidth = (int)ddwidth;
			if( ( iddwidth % 2 ) == 0 )
				ddwidth++;

			return ddwidth;
		}
		public static float GetDropDownArrowHeight()
		{
			return ( (int)ComboBoxItemRenderer.GetDropDownArrowWidth() ) / 2 + 1;
		}
		public static Point[] GetDropDownBorderBounds( Rectangle btnBounds )
		{
			int arWidth = (int)ComboBoxItemRenderer.GetDropDownArrowWidth();
			int arHeight = (int)ComboBoxItemRenderer.GetDropDownArrowHeight();

			return ComboBoxItemRenderer.GetDropDownBorderBounds( btnBounds, arWidth, arHeight );
		}
		public static Point[] GetDropDownBorderBounds( Rectangle btnBounds,
		  int arWidth, int arHeight )
		{
			int left = btnBounds.Left + ( btnBounds.Width - arWidth ) / 2 + 1;
			int top = btnBounds.Top + ( btnBounds.Height - arHeight ) / 2;

			Rectangle rcddbtn = new Rectangle( left, top, arWidth, arHeight );

			Point[] ptsscrll = new Point[] {
							 new Point(rcddbtn.Left, rcddbtn.Top),
							 new Point(rcddbtn.Right, rcddbtn.Top),
							 new Point(left + arWidth/2, rcddbtn.Bottom),
							 new Point(rcddbtn.Left, rcddbtn.Top) };

			return ptsscrll;
		}

		/// <summary>
		/// Gets color of the ComboBox button amenably with VisualStyle.
		/// </summary>
		private Brush GetButtonBrush( VisualStyle style, Rectangle bounds )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new SolidBrush( Office2003Colors.DockBarColorLight );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( bounds, Office2007OutlookColors.ComboButtonLightColor,
						Office2007OutlookColors.ComboButtonDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( bounds, VS2005Colors.MenuColumnStyleLightColor,
						VS2005Colors.MenuColumnStyleDarkColor, LinearGradientMode.Vertical );
					break;
				}
				default:
				{
					brush = new SolidBrush( this.parent.GetBarControl().GetControl().BackColor );
					break;
				}
			}

			return brush;
		}


		/// <summary>
		/// Gets color of the highlighted ComboBox button amenably with VisualStyle.
		/// </summary>
		private Brush GetHighlightButtonBrush( VisualStyle style, Rectangle bounds )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new SolidBrush( Office2003Colors.SelColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( bounds, Office2007OutlookColors.BarItemHighlightLightColor,
						Office2007OutlookColors.BarItemHighlightDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( bounds, VS2005Colors.BarItemHighlightLightColor,
						VS2005Colors.BarItemHighlightDarkColor, LinearGradientMode.Vertical );
					break;
				}
				default:
				{
					brush = new SolidBrush( MenuColors.SelColor );
					break;
				}
			}

			return brush;
		}


		/// <summary>
		/// Gets color of the pressed ComboBox button amenably with VisualStyle.
		/// </summary>
		private Brush GetPressedButtonBrush( VisualStyle style, Rectangle bounds )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new SolidBrush( Office2003Colors.PressedSelColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( bounds, Office2007OutlookColors.BarItemPressLightColor,
						Office2007OutlookColors.BarItemPressDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( bounds, VS2005Colors.BarItemPressLightColor,
						VS2005Colors.BarItemPressDarkColor, LinearGradientMode.Vertical );
					break;
				}
				default:
				{
					brush = new SolidBrush( MenuColors.PressedSelColor );
					break;
				}
			}

			return brush;
		}


		/// <summary>
		/// Gets color for arrow of the ComboBox button amenably with VisualStyle.
		/// </summary>
		protected Color GetArrowColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = SystemColors.ControlText;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = SystemColors.ControlText;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = SystemColors.ControlText;
					break;
				}
				default:
				{
					color = ( this.ShowingDropDown ) ? SystemColors.Window : SystemColors.ControlText;
					break;
				}
			}

			return color;
		}

		/// <summary>
		/// Draws ComboButton background.
		/// </summary>
		private void DrawComboButton( Graphics g, Rectangle rect )
		{
			rect.X += 1;
			rect.Width -= 1;

			Color buttonColor = this.parent.GetBarControl().GetControl().BackColor;
			Brush buttonBrush = GetButtonBrush( this.Style, rect );

			if( buttonColor == Color.Transparent ||
				( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled ) )
			{
				if( this.Style != VisualStyle.VS2005
					&& this.Style != VisualStyle.Office2007Outlook )
				{
					buttonBrush = new SolidBrush( SystemColors.Control );
				}
			}

			if( !this.parent.Customizing && this.ShowHighlightRectangle )
			{
				if( this.ShowingDropDown )
				{
					buttonBrush = GetPressedButtonBrush( this.Style, rect );
				}
				else if( this.HotTrack || this.Active )
				{
					buttonBrush = GetHighlightButtonBrush( this.Style, rect );
				}
			}

			using( buttonBrush )
			{
				g.FillRectangle( buttonBrush, rect );
			}
		}

		/// <summary>
		/// Draws arrow for ComboButton.
		/// </summary>
		private void DrawComboButtonArrow( Graphics g, Rectangle rect )
		{
			Point[] dropDownArrowBounds = ComboBoxItemRenderer.GetDropDownBorderBounds(this.BarItem.ResizeGlyphToFit, rect);

			GraphicsPath path = new GraphicsPath();
			path.AddLines( dropDownArrowBounds );
			Color arrowColor = GetArrowColor( this.Style );

			if( !this.BarItem.Enabled )
			{
				arrowColor = SystemColors.GrayText;
			}

			using( Brush brush = new SolidBrush( arrowColor ) )
			{
				g.FillRegion( brush, new Region( path ) );
			}
		}

		protected override void DrawTextAndImage( Graphics g, RectangleF rectTextAndImage, Font textFont, Color textColor, Color bgColor, DrawItemState state )
		{
			// Draw the label
			if( this.ShouldDrawText() && this.BarItem.Text != String.Empty )
			{
				TextFormatFlags formatFlags = TextFormatFlags.Left;
				RectangleF rect = GetTextPosition( g, this.BarItem.Text, textFont, rectTextAndImage, formatFlags );
				Rectangle rectText = Rectangle.Round( rect );
				int textOffsetY = ( (int)rectTextAndImage.Height - rectText.Height ) / 2;

				if( ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
					&& this.Style != VisualStyle.Office2007
                    && this.Style != VisualStyle.Office2010
					&& this.Style != VisualStyle.Office2007Outlook )
				{
					textOffsetY += DEF_TEXT_OFFSET_Y;
				}

				rectText.Offset( 0, textOffsetY );

				if( rectText.Width > 0 && rectText.Height > 0 )
				{
					DrawText( g, this.BarItem.Text, textFont, textColor, Rectangle.Round( rectText ), formatFlags );
				}
			}

			if( this.Style == VisualStyle.Office2007 )
			{
				// draw ComboButon with Office2007 visual style
				ItemState drawState = GetDrawState();
				Office2007BarItemPainter.DrawComboButton( g, this.DropDownButtonBounds, drawState, this.IsHorizontalAligned );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                // draw ComboButon with Office2007 visual style
                ItemState drawState = GetDrawState();
                Office2010BarItemPainter.DrawComboButton(g, this.DropDownButtonBounds, drawState, this.IsHorizontalAligned);
            }
			else
			{
				// Draw the button portion.
				DrawComboButton( g, this.DropDownButtonBounds );

				// Draw the button arrow
				DrawComboButtonArrow( g, this.DropDownButtonBounds );
			}
			
			// Draw the text
            DrawText(g, this.ComboBoxBarItem.TextBoxValue, textFont, textColor, this.TextAreaBounds,
				TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine );
		}

		protected override Color GetBGColor()
		{
			if( this.BarItem.Enabled )
				return SystemColors.Window;
			else
				return SystemColors.Control;
		}

		protected override Color GetForeColorDefault( DrawItemState state )
		{
			Color foreColor = Color.Empty;
			if( ( state & DrawItemState.Disabled ) == 0 )
			{
				if( this.Active || this.HotTrack )
				{
					if( this.BarItem.Enabled )
					{
						foreColor = MenuColors.SelTextColor;
						return foreColor;
					}
				}
			}

			return base.GetForeColorDefault( state );
		}

		protected int GetComboBoxDropDownWidth()
		{
			return (int)( 3 + ( ComboBoxDropDownWidth - 3 ) *
			  ( SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the checkboxes.*/)
			  );
		}
		internal Rectangle DropDownButtonBounds
		{
			get
			{
				const int nButtonOffsetX = 1;
				const int nButtonOffsetY = 1;
				const int nButtonWidthDiff = 2;

				Rectangle btnBounds = Rectangle.Round( this.Bounds );
				btnBounds.X = GetIsMirrored() ? btnBounds.Left :
							( btnBounds.Right - GetComboBoxDropDownWidth() - DEF_BORDER_OFFSET - nButtonOffsetX );

				if( GetIsMirrored() )
				{
					int nWidthDiff = btnBounds.Width - this.ComboBoxBarItem.MinWidth;

					if( nWidthDiff > 0 )
					{
						btnBounds.X += nWidthDiff;
					}
				}

				// offset due to bounds calculation inaccuracy
				bool isInt = BarRenderer.IsInteger( Bounds.Y );
				if( !isInt && BarItem != null && !BarItem.Enabled )
				{
					btnBounds.Offset( 0, nButtonOffsetY );
				}

				btnBounds.Offset( 0, nButtonOffsetY );
				btnBounds.Width = GetComboBoxDropDownWidth();
				btnBounds.Height -= nButtonWidthDiff;

				return btnBounds;
			}
		}

		public override bool NeedCenterVAlign
		{
			get { return true; }
		}

		private const int DEF_TEXT_OFFSET_Y = 1;

		protected virtual Rectangle TextAreaBounds
		{
			get
			{
				int nDDWidth = this.DropDownButtonBounds.Width;
				RectangleF rectText = this.Bounds;
				// Lose the label's width
				if( rectText.Width > this.ComboBoxBarItem.MinWidth )
				{
					rectText.Width = this.ComboBoxBarItem.MinWidth;
					rectText.X += this.Bounds.Width - rectText.Width;
				}
				rectText.X += 3;
				rectText.Width -= ( nDDWidth + 4 );

				if( GetIsMirrored() )
				{
					rectText.X += nDDWidth;
				}

				bool isInt = BarRenderer.IsInteger( Bounds.Y );
				int textOffsetY = 0;

				if( !( XPThemes.IsThemedOS && XPThemes.IsThemeActive &&
					this.ThemesEnabled ) )
				{
					textOffsetY = ( isInt ) ? DEF_TEXT_OFFSET_Y : DEF_TEXT_OFFSET_Y * 2;
				}
				else
				{
					textOffsetY = DEF_TEXT_OFFSET_Y;
				}

				rectText.Offset( 0, -textOffsetY );

				return Rectangle.Ceiling( rectText );
			}
		}

		protected override ItemState GetDrawState()
		{
			ItemState state = base.GetDrawState();

			if( !this.parent.Customizing && this.ShowHighlightRectangle )
			{
				if( this.ShowingDropDown )
				{
					state = ItemState.Pressed;
				}
				else if( this.HotTrack || this.Active )
				{
					state = ItemState.Selected;
				}
			}
			else
			{
				state = ItemState.Normal;
			}

			return state;
		}


		protected override void DrawBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			if( this.Style == VisualStyle.Office2007 )
			{
				Rectangle comboBoxBounds = drawItemInfo.Bounds;
				comboBoxBounds.Width = this.ComboBoxBarItem.MinWidth;
				comboBoxBounds.X += drawItemInfo.Bounds.Width - comboBoxBounds.Width;
				comboBoxBounds.Width -= DEF_BORDER_OFFSET + 1;
				comboBoxBounds.Height -= 1;

				ItemState state = GetDrawState();
				Office2007BarItemPainter.DrawComboBoxBarItem( drawItemInfo.Graphics, comboBoxBounds, state );
				return;
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                Rectangle comboBoxBounds = drawItemInfo.Bounds;
                comboBoxBounds.Width = this.ComboBoxBarItem.MinWidth;
                comboBoxBounds.X += drawItemInfo.Bounds.Width - comboBoxBounds.Width;
                comboBoxBounds.Width -= DEF_BORDER_OFFSET + 1;
                comboBoxBounds.Height -= 1;

                ItemState state = GetDrawState();
                Office2010BarItemPainter.DrawComboBoxBarItem(drawItemInfo.Graphics, comboBoxBounds, state);
                return;
            }

			if( drawItemInfo.Bounds.Width > this.ComboBoxBarItem.MinWidth )
			{
				// Remove the label area from the bounds
				Rectangle newBounds = drawItemInfo.Bounds;
				newBounds.Width = this.ComboBoxBarItem.MinWidth;
				newBounds.X += drawItemInfo.Bounds.Width - newBounds.Width;
				newBounds.Width -= DEF_BORDER_OFFSET;

				DrawToolbarItemEventArgs args = new DrawToolbarItemEventArgs( drawItemInfo.Graphics,
					drawItemInfo.MouseDown, drawItemInfo.DropDown, drawItemInfo.Font,
					newBounds, drawItemInfo.Index, drawItemInfo.State, drawItemInfo.ForeColor,
					drawItemInfo.BackColor, drawItemInfo.BackColor2, drawItemInfo.BoundsInterior, drawItemInfo.defaultDrawBackground,
					drawItemInfo.defaultDrawBorders, drawItemInfo.defaultDrawInterior );

				base.DrawBorders( args, false );
			}
			else
				base.DrawBorders( drawItemInfo, false );

			if( ( this.HotTrack || this.Active ) && this.ShowHighlightRectangle )
			{
				Rectangle btnBounds = this.DropDownButtonBounds;
				bool bPressed = this.HotTrack && this.Active;
				Pen borderPen = null;

				if( bPressed )
				{
					borderPen = GetBorderPressPen( this.Style );
				}
				else
				{
					borderPen = ( this.BarItem.Checked ) ? GetBorderCheckPen( this.Style ) :
						GetBorderPen( this.Style );
				}

				int nX = this.IsRTL ? btnBounds.Right : btnBounds.Left;
				drawItemInfo.Graphics.DrawLine( borderPen, nX, btnBounds.Top, nX, btnBounds.Bottom );
			}

			if( !this.BarItem.Enabled )
			{
				Rectangle rectBounds = Rectangle.Ceiling( this.Bounds );

				// Lose the label's width
				if( rectBounds.Width > this.ComboBoxBarItem.MinWidth )
				{
					rectBounds.Width = this.ComboBoxBarItem.MinWidth;
					rectBounds.X += (int)this.Bounds.Width - rectBounds.Width;
				}

				rectBounds.Width -= DEF_BORDER_OFFSET;
				rectBounds.Width -= 1;
				rectBounds.Height -= 1;
                using (Pen pen = new Pen(SystemColors.ControlDark))
                {
                    drawItemInfo.Graphics.DrawRectangle(pen, rectBounds);
                }
			}
		}
		protected override void DrawBackground( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			Graphics g = drawItemInfo.Graphics;
			Rectangle bgBounds = drawItemInfo.Bounds;
			Color bgColor = Color.Empty;

			// Lose the label's width
			if( bgBounds.Width > this.ComboBoxBarItem.MinWidth )
			{
				bgBounds.X += bgBounds.Width - this.ComboBoxBarItem.MinWidth;
				bgBounds.Width = this.ComboBoxBarItem.MinWidth;

				Rectangle labelBounds = drawItemInfo.Bounds;
				labelBounds.X += DEF_BORDER_OFFSET;
				labelBounds.Width = labelBounds.Width - bgBounds.Width - DEF_BORDER_OFFSET;

				// Draw the label
				// color for the label area
				bgColor = this.parent.GetBarControl().GetControl().BackColor;
				// Take a look at BarControlInternal constructor for notes on why we need this check.
				if( bgColor != this.parent.GetBarControl().GetControl().BackColor )
					g.FillRectangle( new SolidBrush( bgColor ), labelBounds );
			}

			// Draw the text area.
			bgColor = drawItemInfo.BackColor;

			bgBounds.Width -= DEF_BORDER_OFFSET;
			// Take a look at BarControlInternal constructor for notes on why we need this check.
			if( bgColor != this.parent.GetBarControl().GetControl().BackColor )
				g.FillRectangle( new SolidBrush( bgColor ), bgBounds );


			//if(!this.HotTrack && !this.Active)
			//{
			//drawItemInfo.Graphics.DrawLine(new Pen(this.parent.GetBarControl().GetControl().BackColor, 1),
			//  new PointF(this.Bounds.Left, this.Bounds.Top), new PointF(this.Bounds.Left, this.Bounds.Bottom));
			//drawItemInfo.Graphics.DrawLine(new Pen(this.parent.GetBarControl().GetControl().BackColor, 1),
			//  new PointF(this.Bounds.Left + 1, this.Bounds.Top), new PointF(this.Bounds.Left+1, this.Bounds.Bottom));
			//}
		}
		#endregion DRAWING

		#region Event handlers

		private void BarItem_PropertyChanged(object sender, Syncfusion.ComponentModel.SyncfusionPropertyChangedEventArgs e)
		{
			if( "RightToLeft" == e.PropertyName && null != this.listBoxContainer )
			{
				this.listBoxContainer.RightToLeft = (RightToLeft)e.NewValue;
			}
		}

		private void listBoxContainer_MouseWheel( object sender, MouseEventArgs e )
		{
			if( ComboBoxBarItem.Editable ) return;

			OnMouseWheel( e.Delta > 0 );
		}

		private void m_lstNativeWnd_MouseActivated( object sender, EventArgs e )
		{

			if( this.listBoxContainer != null )
			{
				this.listBoxContainer.ShouldHidePopup = false;
			}

			MainFrameBarManager mainManager = GetMainManager();
			if( mainManager != null )
			{
				mainManager.ShouldHidePopup = false;
			}
		}

		protected virtual void ListBox_LostFocus( object sender, EventArgs e )
		{
			if( this.listBoxContainer != null )
			{
				this.listBoxContainer.ShouldHidePopup = true;
			}

			BarRenderer renderer = parent as BarRenderer;
			if( renderer != null )
			{
				renderer.NeedDropDown = false;
			}

			int selIndex = listBoxContainer.ListBox.SelectedIndex;
			if( selIndex >= 0 && selIndex < listBoxContainer.ListBox.Items.Count )
			{
				if( ComboBoxBarItem != null )
				{
					ComboBoxBarItem.TextBoxValue =
						  GetItemText( listBoxContainer.ListBox.Items[selIndex] );
				}
			}

			MainFrameBarManager mainManager = GetMainManager();
			if( mainManager != null )
			{
				mainManager.ShouldHidePopup = true;
			}
		}

		#endregion

		internal bool DesignMode
		{
			get
			{
				return ( this.BarItem != null && this.BarItem.DesignMode ) || this.parent.GetBarControl().DesignMode
					|| ( this.parent.Bar.Manager != null && this.parent.Bar.Manager.DesignMode );
			}
		}
	}

	#endregion

	#region ComboTextBox

	[ToolboxItem( false ), Syncfusion.Documentation.DocumentationExclude()]
	public class ComboTextBox: TextBox, IToolbarControl, IDontCallKillFocus //, IDelegateFocusToPrevWindow
	{
		private EditableComboRenderer renderer;
		internal Control prevControlWithFocus;
		public ComboTextBox( EditableComboRenderer renderer )
		{
			this.renderer = renderer;
			this.BorderStyle = BorderStyle.None;
			this.TabStop = false;
			this.AcceptsReturn = true;
			if( renderer.BarItem != null )
			{
				this.Name = "comboTextBox" + renderer.BarItem.ID;
			}
		}

		protected override bool IsInputKey( Keys keyData )
		{
			if( keyData == Keys.Return )
				return true;

			return base.IsInputKey( keyData );
		}

		protected override bool ProcessDialogKey( Keys keyData )
		{
			if( keyData == Keys.Return )
			{
				this.renderer.HitReturn();
				return true;
			}
			else if( keyData == Keys.Escape )
			{
				this.renderer.HitEscape();
				return true;
			}
			else
				return base.ProcessDialogKey( keyData );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			// Doing this here rather than double-click to reduce flicker.

			base.OnMouseDown( e );
		}

		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );
			this.renderer.TextBoxValueChanged();
		}

		internal void StorePrevFocus()
		{
			IntPtr focused = Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus();
			Control prevFocus = Control.FromHandle( focused );
			if( prevFocus != this )
				this.prevControlWithFocus = prevFocus;
		}

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x21/*WM_MOUSEACTIVATE*/
			  || m.Msg == Syncfusion.Runtime.InteropServices.NativeMethods.WM_ACTIVATE )
			{
				this.StorePrevFocus();
				m.Result = (IntPtr)1; // This will force activation and prevent the WM_MOUSEACTIVATE call to the parent.
				return;
			}

			base.WndProc( ref m );
		}
		protected override void OnMouseEnter( EventArgs e )
		{
			base.OnMouseEnter( e );
			this.renderer.TextBoxMouseEnter();
		}
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );
			this.renderer.TextBoxMouseLeave();
		}
		protected override void OnGotFocus( EventArgs e )
		{
			base.OnGotFocus( e );
			this.renderer.TextBoxGotFocus();
		}
		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );
			this.renderer.TextBoxLostFocus( this.Text );
		}
		void IToolbarControl.OnMouseDownOutside()
		{
			this.renderer.UpdateTextBoxValue();
		}
	}

	#endregion

	/// <summary>
	/// This event is triggered when the internal TextBox gets added to the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem"/>.
	/// </summary>
	public delegate void TextBoxBoundEventHandler( object sender, TextBoxBoundEventArgs args );

	#region TextBoxBoundEventArgs

	/// <summary>
	/// Provides data for the TextBoxBoundEvent.
	/// </summary>
	public class TextBoxBoundEventArgs: EventArgs
	{
		private TextBoxBase textBoxBase = null;

		public TextBoxBoundEventArgs( TextBoxBase textBox )
		{
			this.textBoxBase = textBox;
		}

		public TextBoxBase TextBox
		{
			get
			{
				return this.textBoxBase;
			}
		}
	}

	#endregion

	/// <summary>
	/// This event is triggered when the internal TextBox gets added to the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.TextBoxBarItem"/>.
	/// </summary>
	public delegate void TextBoxItemBoundEventHandler( object sender, TextBoxItemBoundEventArgs args );

	#region TextBoxItemBoundEventArgs

	/// <summary>
	/// Provides data for the TextBoxItemBoundEvent.
	/// </summary>
	public class TextBoxItemBoundEventArgs: EventArgs
	{
		private TextBoxBase m_textBoxBase = null;

		public TextBoxItemBoundEventArgs( TextBoxBase textBoxBase )
		{
			m_textBoxBase = textBoxBase;
		}

		public TextBoxBase TextBox
		{
			get
			{
				return m_textBoxBase;
			}
		}
	}

	#endregion

	#region TextBoxBarItemRenderer

	public class TextBoxBarItemRenderer: BarItemRenderer
	{

		#region Constants
		private const int c_iMinHeigth = 20;
		private const int c_iIndentWidth = 1;
		private const int c_iBackTextBoxWidthDecrease = 2;
		private const int c_iShrinkTextBoxHorizontal = 3;
		private const int c_iShrinkTextBoxVertical = 2;
		#endregion

		#region Members

		/// <summary>
		/// Text Box.
		/// </summary>
		private TextBoxItem m_txtTextBox = null;
		/// <summary>
		/// True - need refresh text box layout.
		/// </summary>
		private bool m_bNeedLayout = true;
		/// <summary>
		/// True - this control need highlighted.
		/// </summary>
		private bool m_bHighlighted = false;

		#endregion

		#region Construction\disposing

		public TextBoxBarItemRenderer( IBarRenderer parent )
			: base( parent )
		{
			if( this.m_txtTextBox == null && !this.DesignMode() )
			{
				this.m_txtTextBox = new TextBoxItem( this );
				this.GetBarControls().Add( this.m_txtTextBox );

				this.m_txtTextBox.GotFocus += new EventHandler( m_txtTextBox_GotFocus );
				this.m_txtTextBox.LostFocus += new EventHandler( m_txtTextBox_LostFocus );
				this.m_txtTextBox.MouseEnter += new EventHandler( m_txtTextBox_MouseEnter );
				this.m_txtTextBox.MouseLeave += new EventHandler( m_txtTextBox_MouseLeave );
				this.m_txtTextBox.TextChanged += new EventHandler( m_txtTextBox_TextChanged );

				this.m_txtTextBox.Visible = true;
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.m_txtTextBox != null )
				{
					this.m_txtTextBox.GotFocus -= new EventHandler( m_txtTextBox_GotFocus );
					this.m_txtTextBox.LostFocus -= new EventHandler( m_txtTextBox_LostFocus );
					this.m_txtTextBox.MouseEnter -= new EventHandler( m_txtTextBox_MouseEnter );
					this.m_txtTextBox.MouseLeave -= new EventHandler( m_txtTextBox_MouseLeave );
					this.m_txtTextBox.TextChanged -= new EventHandler( m_txtTextBox_TextChanged );

					this.GetBarControls().Remove( this.m_txtTextBox );
					this.parent.GetBarControl().GetControl().ResumeLayout( false );

					this.m_txtTextBox.Dispose();
					this.m_txtTextBox = null;
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region TextBox Events

		private void m_txtTextBox_GotFocus( object sender, EventArgs e )
		{
			this.m_bHighlighted = true;
			this.Repaint();
		}

		private void m_txtTextBox_LostFocus( object sender, EventArgs e )
		{
			this.m_bHighlighted = false;
			this.Repaint();
		}

		private void m_txtTextBox_MouseEnter( object sender, EventArgs e )
		{
			this.m_bHighlighted = true;
		}

		private void m_txtTextBox_MouseLeave( object sender, EventArgs e )
		{
			this.m_bHighlighted = this.m_txtTextBox.Focused;
		}

		private void m_txtTextBox_TextChanged( object sender, EventArgs e )
		{
			this.GetTextBoxBarItem().TextBoxValue = this.m_txtTextBox.Text;
		}

		#endregion

		#region Overrides

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if( base.Visible != value )
				{
					if( this.m_txtTextBox != null )
					{
						this.m_txtTextBox.Visible = value;
					}
				}
				base.Visible = value;
			}
		}

		public override void BarItemPropertyChanged( SyncfusionPropertyChangedEventArgs e )
		{
			if( this.m_txtTextBox != null && this.GetTextBoxBarItem() != null )
			{
				if( e.PropertyName == "Enabled" )
				{
					this.m_txtTextBox.Enabled = this.BarItem.Enabled;
				}
				else if( e.PropertyName == "TextBoxValue" )
				{
					this.m_txtTextBox.Text = this.GetTextBoxBarItem().TextBoxValue;
				}
				else if( e.PropertyName == "Style" )
				{
					if( (VisualStyle)e.NewValue == VisualStyle.Office2007 )
					{
						this.TextBox.BackColor = Office2007Colors.Default.TextBarItemBackColor;
					}
                    else if ((VisualStyle)e.NewValue == VisualStyle.Office2010)
                    {
                        this.TextBox.BackColor = Office2010Colors.Default.TextBarItemBackColor;
                    }
					else
					{
						this.TextBox.BackColor = SystemColors.Window;
					}
				}
			}

			base.BarItemPropertyChanged( e );
		}

		private void CustomizationStart( object sender, EventArgs e )
		{
			if( this.m_txtTextBox != null )
				this.m_txtTextBox.Visible = false;
		}
		private void CustomizationDone( object sender, EventArgs e )
		{
			if( this.m_txtTextBox != null )
				this.m_txtTextBox.Visible = ( this.BarItem == null || ( this.BarItem.Visible && this.BarItem.Enabled ) );
		}

		public override BarItem BarItem
		{
			get
			{
				return base.BarItem;
			}
			set
			{
				if( base.BarItem != value )
				{
					if( this.BarItem != null && this.BarItem.Manager != null
						&& this.BarItem.Manager.MainFrameBarManager != null )
					{
						this.BarItem.Manager.MainFrameBarManager.CustomizationBegin -= new EventHandler( this.CustomizationStart );
						this.BarItem.Manager.MainFrameBarManager.CustomizationDone -= new EventHandler( this.CustomizationDone );
					}

					base.BarItem = value;
					TextBoxBarItem textBoxItem = value as TextBoxBarItem;

					if( this.m_txtTextBox != null && this.GetTextBoxBarItem() != null )
					{
						this.m_txtTextBox.Text = this.GetTextBoxBarItem().TextBoxValue;
						this.m_txtTextBox.Enabled = this.BarItem.Enabled;

						TextBoxBarItem textItem = value as TextBoxBarItem;

						m_txtTextBox.HandleCreated += new EventHandler( TextBoxHandleCreated );

						if( m_txtTextBox.IsHandleCreated )
						{
							DeferredTextBoxBinding();
						}

						this.m_bNeedLayout = true;
					}

					if( this.BarItem != null && this.BarItem.Manager != null
						&& this.BarItem.Manager.MainFrameBarManager != null )
					{
						this.BarItem.Manager.MainFrameBarManager.CustomizationBegin += new EventHandler( this.CustomizationStart );
						this.BarItem.Manager.MainFrameBarManager.CustomizationDone += new EventHandler( this.CustomizationDone );
					}
				}
			}
		}

		private void DeferredTextBoxBinding()
		{
			TextBoxBarItem tbItem = (TextBoxBarItem)this.BarItem;

			if( tbItem != null )
			{
				tbItem.OnTextBoxItemBound( new TextBoxItemBoundEventArgs( this.m_txtTextBox ) );
			}
		}

		void TextBoxHandleCreated( object sender, EventArgs e )
		{
			DeferredTextBoxBinding();
		}

		public override bool ShouldDrawText()
		{
			return ( this.BarItem != null &&
				this.BarItem.PaintStyle != PaintStyle.Default &&
				this.BarItem.PaintStyle != PaintStyle.TextOnlyInMenus );
		}

		public override void OnMouseDown( Point pointMouseDown )
		{
			if( this.m_txtTextBox != null && !this.m_txtTextBox.Focused )
			{
				this.m_txtTextBox.SelectAll();
				this.m_txtTextBox.Focus();
			}
		}

		protected override void DrawBackground( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			Color backColor = SystemColors.Window;

			if( this.m_txtTextBox != null )
			{
				backColor = this.m_txtTextBox.BackColor;
			}

			if( !this.BarItem.Enabled )
			{
				backColor = SystemColors.Control;
			}

			drawItemInfo.Graphics.FillRectangle( new SolidBrush( backColor ), this.GetBackTextBoxBounds( drawItemInfo.Bounds ) );

			if( this.DesignMode() && this.GetTextBoxBarItem().TextBoxValue != String.Empty )
			{
				DrawText( drawItemInfo.Graphics, this.GetTextBoxBarItem().TextBoxValue, Control.DefaultFont,
					( this.BarItem.Enabled ? SystemColors.WindowText : SystemColors.GrayText ), GetTextBoxBounds( drawItemInfo.Bounds ),
					TextFormatFlags.Default );
			}
		}

		protected override ItemState GetDrawState()
		{
			ItemState state = base.GetDrawState();

			if( this.m_bHighlighted || this.HotTrack )
			{
				state = ItemState.Selected;
			}

			return state;
		}

		protected override void DrawBorders( DrawToolbarItemEventArgs drawItemInfo, bool useThemes )
		{
			Rectangle rc = this.GetBackTextBoxBounds( drawItemInfo.Bounds );
			rc.Height -= 1;
			rc.Width -= 1;

			if( this.Style == VisualStyle.Office2007 )
			{
				ItemState state = GetDrawState();
				Office2007BarItemPainter.DrawTextBoxBarItem( drawItemInfo.Graphics, rc, state, this.IsHorizontalAligned );
				return;
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                ItemState state = GetDrawState();
                Office2010BarItemPainter.DrawTextBoxBarItem(drawItemInfo.Graphics, rc, state, this.IsHorizontalAligned);
                return;
            }

			if( this.BarItem.Enabled )
			{
				if( this.m_bHighlighted || this.HotTrack || this.GetTextBoxBarItem().Checked )
				{
					bool bPressed = this.HotTrack && this.Active;
					Pen borderPen = null;

					if( bPressed )
					{
						borderPen = GetBorderPressPen( this.Style );
					}
					else
					{
						borderPen = ( this.BarItem.Checked ) ? GetBorderCheckPen( this.Style ) :
							GetBorderPen( this.Style );
					}

					drawItemInfo.Graphics.DrawRectangle( borderPen, rc );
				}
			}
			else
			{
                using (Pen pen = new Pen(SystemColors.ControlDark))
                {
                    drawItemInfo.Graphics.DrawRectangle(pen, rc);
                }
			}
		}

		protected override void DrawTextAndImage( Graphics g, RectangleF rectTextAndImage, Font textFont, Color textColor, Color bgColor, DrawItemState state )
		{
			if( this.ShouldDrawText() )
			{
				rectTextAndImage.Width = MeasureText( g, this.BarItem.Text, textFont ).Width;

				TextBoxBarItem barItem = this.GetTextBoxBarItem();

				if( barItem.PaintStyle == PaintStyle.ImageAndText )
				{
					Size szImage = this.BarItem.GetImageSizeInternal( false );

					if( !szImage.IsEmpty )
					{
						rectTextAndImage.Width += szImage.Width + c_iShrinkTextBoxHorizontal;
					}
				}

				if( this.GetIsMirrored() )
				{
					rectTextAndImage.X += this.Bounds.Width - rectTextAndImage.Width - c_iIndentWidth;
				}

				base.DrawTextAndImage( g, rectTextAndImage, textFont, textColor, bgColor, state );
			}
		}

		public override SizeF GetPreferredSize( IGraphicsProvider gp )
		{
			SizeF preferredSize = SizeF.Empty;
			TextBoxBarItem barItem = this.GetTextBoxBarItem();

			if( barItem.PaintStyle == PaintStyle.TextOnly ||
				barItem.PaintStyle == PaintStyle.ImageAndText )
			{
				this.AddTextPreferredSize( gp, ref preferredSize );
			}

			if( barItem.PaintStyle == PaintStyle.ImageAndText )
			{
				Size szImage = this.BarItem.GetImageSizeInternal( false );

				if( !szImage.IsEmpty )
				{
					preferredSize.Width += szImage.Width + c_iShrinkTextBoxHorizontal;
				}
			}

			preferredSize.Width += barItem.MinWidth;

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive
				&& this.ThemesEnabled && !IsParentMainMenu
				&& this.Style != VisualStyle.Office2007
                && this.Style != VisualStyle.Office2010
				&& this.Style != VisualStyle.Office2007Outlook )
			{
				preferredSize.Width += this.PaddingForThemesX;
				preferredSize.Height += this.PaddingForThemesY;
			}

			if( preferredSize.Height < c_iMinHeigth )
			{
				preferredSize.Height = c_iMinHeigth;
			}

			preferredSize.Width += c_iIndentWidth * 2;

			return preferredSize;
		}

		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				if( base.Bounds != value )
				{
					this.m_bNeedLayout = true;
					base.Bounds = value;
				}
			}
		}

		public override void OnPaint( Graphics g, Rectangle clippingRect )
		{
			if( this.m_txtTextBox != null && this.m_bNeedLayout )
			{
				this.m_txtTextBox.Bounds = this.GetTextBoxBounds( Rectangle.Ceiling( this.GetDrawingBounds() ) );
				this.m_bNeedLayout = false;
			}
			base.OnPaint( g, clippingRect );
		}

		#endregion

		/// <summary>
		/// Gets bar controls.
		/// </summary>
		/// <returns></returns>
		private Control.ControlCollection GetBarControls()
		{
			IBarControl barControl = this.parent.GetBarControl();

			if( barControl == null )
			{
				throw new NullReferenceException( "BarControl is null." );
			}

			Control control = barControl.GetControl();

			if( control == null )
			{
				throw new NullReferenceException( "Control is null." );
			}

			Control.ControlCollection controlCollection = control.Controls;

			if( controlCollection == null )
			{
				throw new Exception( "�ontrolCollection is null." );
			}

			return controlCollection;
		}

		/// <summary>
		/// Gets text box control.
		/// </summary>
		public TextBox TextBox
		{
			get
			{
				return this.m_txtTextBox;
			}
		}

		internal bool DesignMode()
		{
			return ( this.BarItem != null && this.BarItem.DesignMode ) || this.parent.GetBarControl().DesignMode
				|| ( this.parent.Bar.Manager != null && this.parent.Bar.Manager.DesignMode );
		}

		/// <summary>
		/// Returns true, if control must be mirrored.
		/// </summary>
		protected bool GetIsMirrored()
		{
			return this.IsRTL;
		}

		/// <summary>
		/// Gets visible text box rectangle.
		/// </summary>
		protected virtual Rectangle GetBackTextBoxBounds( Rectangle bounds )
		{
			Rectangle rc = bounds;
			rc.Width = this.GetTextBoxBarItem().MinWidth;
			rc.Width -= c_iBackTextBoxWidthDecrease;

			if( ( this.BarItem.PaintStyle == PaintStyle.TextOnly ||
				this.BarItem.PaintStyle == PaintStyle.ImageAndText ) && !this.GetIsMirrored() )
			{
				rc.X += (int)this.GetDrawingBounds().Width - rc.Width - c_iIndentWidth;
			}
			else
			{
				rc.X += c_iIndentWidth;
			}
			return rc;
		}

		/// <summary>
		/// Gets text box rectangle.
		/// </summary>
		/// <returns></returns>
		protected virtual Rectangle GetTextBoxBounds( Rectangle bounds )
		{
			Rectangle rc = this.GetBackTextBoxBounds( bounds );

			rc.Offset( c_iShrinkTextBoxHorizontal, c_iShrinkTextBoxVertical );
			rc.Width -= c_iShrinkTextBoxHorizontal * 2;
			rc.Height -= c_iShrinkTextBoxVertical * 2;
			return rc;
		}

		/// <summary>
		/// Gets TextBoxBarItem.
		/// </summary>
		private TextBoxBarItem GetTextBoxBarItem()
		{
			TextBoxBarItem barItem = this.BarItem as TextBoxBarItem;

			return barItem;
		}

	}

	#endregion

	#region TextBoxItem

	[ToolboxItem( false )]
	public class TextBoxItem: TextBox, IToolbarControl, IDontCallKillFocus
	{
		private TextBoxBarItemRenderer renderer;
		internal Control prevControlWithFocus;

		public TextBoxItem( TextBoxBarItemRenderer renderer )
		{
			this.renderer = renderer;
			this.BorderStyle = BorderStyle.None;
			this.TabStop = false;
			this.AcceptsReturn = true;

			if( renderer.BarItem != null )
			{
				this.Name = "TextBox" + renderer.BarItem.ID;
			}
		}

		internal void StorePrevFocus()
		{
			IntPtr focused = Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus();
			Control prevFocus = Control.FromHandle( focused );
			if( prevFocus != this )
				this.prevControlWithFocus = prevFocus;
		}

		void IToolbarControl.OnMouseDownOutside()
		{
		}
	}

	#endregion
}