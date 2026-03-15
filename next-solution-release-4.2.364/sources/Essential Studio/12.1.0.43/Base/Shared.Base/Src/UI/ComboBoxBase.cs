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
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// An advanced combo box control.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This advanced combo box control provides a lot of improvements over the standard <see cref="System.Windows.Forms.ComboBox"/>
	/// control. The improvements are as follows:
	/// <list type="bullet">
	/// <item>
	/// <term>Plug in any <see cref="System.Windows.Forms.ListControl"/> derived class as the list for the list portion of the combo box
	/// using the <see cref="ListControl"/> property.</term>
	/// </item>
	/// <item>
	/// <term>Provides a <see cref="FlatStyle"/> mode for regular, flat or themed drawing.</term>
	/// </item>
	/// <item>
	/// <term>Written using native .NET controls, this control lets you customize everything in the combo box from the textbox to the drop-down window.</term>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// This control requires the plugged in <b>ListControl</b> to implement the <b>Items</b> property returning
	/// a valid <see cref="System.Collections.IList"/> instance. The <b>ListControl</b> can
	/// optionally implement an <b>IndexFromPoint</b> method that will take a single argument of type <see cref="System.Drawing.Point"/> and
	/// a <b>TopIndex</b> property of type int. The semantics of these above properties and methods are similar to
	/// that of a <see cref="ListBox"/>'s implementation.</para>
	/// <para>
	/// Implementing <b>IndexFromPoint</b> and <b>TopIndex</b> will enable QuickSelection capability for the combo box, wherein
	/// the user can click on the drop-down button and start selecting items in the list, all this
	/// without releasing the mouse.
	/// </para>
	/// </remarks>
	[Designer( typeof( ComboBaseDesigner ), typeof( System.ComponentModel.Design.IDesigner ) ),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.ComboBoxBase.bmp"),
	Description("Represents advanced ComboBox control with plugged in ListControl.")
	]
	public class ComboBoxBase : ComboDropDown
	{
		#region FIELDS
		
		
		private object cachedSelectedValue = null;
		private int indexOnListMouseDown = -1;
		private bool updateSelectionOnCommit = true;
        private bool preventLostFocus = false;
		
		//private bool themesEnabled = false;
		
		/// <summary>
		/// Indicating whether the control uses AutoComplete.
		/// </summary>
		private bool m_bAutoComplete = true;
		
		#endregion FIELDS

		#region INIT
		/// <summary>
		/// Creates a new instance of the ComboBoxBase class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializing this ComboBoxBase also requires you to set the
		/// <see cref="ListControl"/> property of this combo box.
		/// </para>
		/// </remarks>
		public ComboBoxBase():
			base()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ComboBoxBase));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
		}

		private void Init()
		{
			if( this.PopupContainer.PopupHost == null )
			{
				this.PopupContainer.PopupHost = new ComboBoxBasePopUpHost();
			}
		}
        
		protected override void OnEndInit()
		{
			if(this.DropDownStyle == ComboBoxStyle.DropDownList)
			{
				// Make sure that an item is selected in the list,
				// as the base class will call UpdateText if invalid text.
				IList list = this.GetListControlList();
				if(list != null && list.Count > 0
					&& this.ListControl.SelectedIndex == -1)
					this.ListControl.SelectedIndex = 0;
			}
			base.OnEndInit();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			Init();
		}

		#endregion INIT 

		#region PROPERTIES
		/// <summary>
		/// Gets or sets the <see cref="System.Windows.Forms.ListControl"/> that will be used in the drop-down portion.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.ListControl"/> derived instance.</value>
		/// <remarks>
		/// <para>
		/// Take a look at the <see cref="ComboBoxBase"/> class description for more information
		/// on requirements and usage of this list control.
		/// </para>
		/// </remarks>
		[
			DefaultValue(null),
			Category("Behavior"),
		Description("The ListControl that will be used in the drop-down portion.")
		]
		public virtual ListControl ListControl
		{
			get
			{
				return base.PopupControl as ListControl;
			}
			set
			{
				base.PopupControl = value;
			}
		}
        
        /// <summary>
        /// This will be used in the drop-down portion.
        /// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Control PopupControl
		{
			get{return base.PopupControl;}
			set{base.PopupControl = value;}
		}

		/// <summary>
		/// Indicates whether the selection in the list control should be updated
		/// with the new text entered by the user when the control loses focus.
		/// </summary>
		/// <remarks>This property is typically used only when the list mode is set to editable (<b>DropDown</b>).</remarks>
		[Description("Updates list selection with new text on Leave."),
		Category("Behavior"),
		DefaultValue(true)]
		public virtual bool UpdateSelectionBeforeValidate
		{
			get{return this.updateSelectionOnCommit;}
			set
			{
				this.updateSelectionOnCommit = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the control uses AutoComplete.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "Indicating whether the control uses AutoComplete." ),
		DefaultValue( true )
		]
		public bool AutoComplete 
		{
			get
			{
				return m_bAutoComplete;
			}
			set
			{
				if( value != m_bAutoComplete )
				{
					m_bAutoComplete = value;
					OnAutoCompleteChanged();
				}
			}
		}
		#endregion PROPERTIES

		#region EVENTS
		/// <summary>
		/// Occurs when the user clicks in the list box in the drop-down to let you
		/// cancel the subsequent drop-down close.
		/// </summary>
        [Description("Occurs when the user clicks in the list box in the drop-down to let you cancel the subsequent drop-down close.")
		]
		public event MouseClickCancelEventHandler DropDownCloseOnClick;

		/// <summary>
		/// Raises the DropDownCloseOnClick event.
		/// </summary>
		/// <param name="args">A <see cref="MouseClickCancelEventArgs"/> that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnDropDownCloseOnClick method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnDropDownCloseOnClick 
		/// in a derived class, be sure to call the base class's 
		/// OnDropDownCloseOnClick method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnDropDownCloseOnClick(MouseClickCancelEventArgs args)
		{
			if(this.DropDownCloseOnClick != null)
				this.DropDownCloseOnClick(this, args);
		}

		/// <summary>
		/// Occurs when the selected Index of list is about to be changed.
		/// Can be canceled to avoid selection of the specific item.
		/// </summary>
        [Description("Occurs when the selected Index of list is about to be changed.")
		]
		public event SelectedIndexChangingHandler SelectedIndexChanging;

		/// <summary>
		/// Risen by <see cref="OnAutoCompleteChanged"/> method.
		/// </summary>
		[Description( "Risen by OnAutoCompleteChanged method." )]
		public event EventHandler AutoCompleteChanged;

		/// <summary>
		/// Fires <see cref="SelectedIndexChanging"/> event.
		/// </summary>
		protected virtual void OnSelectedIndexChanging( SelectedIndexChangingArgs e )
		{
            if(this.DropDownStyle == ComboBoxStyle.DropDownList && this.DroppedDown)
            {
                preventLostFocus = true;
            }
			if( null != this.SelectedIndexChanging )
				this.SelectedIndexChanging( this, e );
            preventLostFocus = false;
		}
		#endregion EVENTS
	
		#region TEXTBOX
        /// <summary>
        /// Returns the text representation of the specified item in PopUpControl.
        /// </summary>
        /// <param name="index">The index.</param>
        
		protected override string GetPopupControlItemText(int index)
		{
			return this.ListControl.GetItemText(this.GetListControlList()[index]);
		}

		protected virtual ListControl GetPopupControlListControl()
		{
			return this.ListControl;
		}

        protected override PopupControlContainer CreatePopupContainer()
        {
            return new ComboBoxPopupContainer();
        }

		/// <summary>
		/// Indicate whether ListControl contains item with such index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>True - if there is such item, otherwise - false.</returns>
		protected override bool IsValidIndex(int index)
		{
			if (index < 0 || index > this.GetListControlList().Count - 1)
			{
				return false;
			}
			else
				return true;
		}
		
		/// <summary>
		/// Called to find a matching item in the attached <see cref="ListControl"/> given 
		/// the prefix of the item.
		/// </summary>
		/// <param name="prefix">The prefix string.</param>
		/// <param name="select">Indicates whether to change the selection in the list when a match occurs.</param>
		/// <param name="start">The index within each list item-string, where the search begins.</param>
		/// <param name="ignoreCase">Indicates whether to ignore case.</param>
		/// <returns>The index of the matching item; -1 otherwise.</returns>
		protected override int FindItem(string prefix, bool select, int start, bool ignoreCase)
		{
			if (ignoreCase)
				prefix = prefix.ToUpper();

			IList list = this.GetListControlList();
			if(list == null)
				return -1;

			int count = list.Count;
			for (int i = 0; i < count; i++) 
			{
				int index = (i+start+1)%count;
				string itemText = this.ListControl.GetItemText(list[index]);
				if (itemText.Length >= prefix.Length) 
				{
					if( prefix != String.Empty )
						itemText = itemText.Substring(0, prefix.Length);
					
					if (ignoreCase)
						itemText = itemText.ToUpper();

					if (prefix == itemText) 
					{
						if (select && index != this.ListControl.SelectedIndex)
							this.ListControl.SelectedIndex = index;
						return index;
					}
                    else if (prefix == String.Empty && ReadOnly)
                    {
                        return index;
                    }
				}
			}

			if (select && this.ListControl.SelectedIndex >= 0)
				this.ListControl.SelectedIndex = -1;
            
			return -1;	
		}

		/// <summary>
		/// Returns the <see cref="System.Collections.IList"/> interface representing the item's
		/// collection of the attached <see cref="ListControl"/>.
		/// </summary>
		/// <returns>An <b>IList</b> interface.</returns>
		/// <remarks>
		/// The base class version looks for the "Items" property in the attached <b>ListControl</b>.
		/// </remarks>
		protected virtual IList GetListControlList()
		{
			IList list = null;
			if(this.ListControl != null)
			{
				Type listType = this.ListControl.GetType();
				
				PropertyInfo pInfo = null;
				PropertyInfo[] pInfos = listType.GetProperties();
				foreach(PropertyInfo prop in pInfos)
				{
					if(prop.Name == "Items")
					{
						pInfo = prop;
						break;
					}
				}

				if(pInfo != null)
				{
					list = pInfo.GetValue(this.ListControl, new object[]{}) as IList;
				}
			}
			return list;
		}

		/// <summary>
		/// Moves the current selection in the attached <see cref="ListControl"/>.
		/// </summary>
		/// <param name="up">Indicates whether to move up.</param>
		protected override void MoveSelelctionInPopupControl(bool up)
		{
			base.MoveSelelctionInPopupControl( up );
			if(this.ListControl == null)
				return;

			string beforeSelectionChange = this.Text;
			int selectedIndex = this.ListControl.SelectedIndex;
			if( up )
			{
                if( selectedIndex < 0 )
                {
                    selectedIndex = 0;
                }
                else
                {
                    selectedIndex--;
                }
            }
			else
			{
                selectedIndex++;
            }

			IList list = this.GetListControlList();
			if(selectedIndex > -1 && list != null && selectedIndex < list.Count)
			{
				SelectedIndexChangingArgs sica =
                    new SelectedIndexChangingArgs(this.ListControl.SelectedIndex, selectedIndex);

				this.OnSelectedIndexChanging(sica);

				if( !sica.Cancel )
				{
					this.ignorePopupValueChange = true;

					this.ListControl.SelectedIndex = selectedIndex;
					this.ignoreNextPopupControlMouseMove = true;

					this.ignorePopupValueChange = false;

					this.OnSelectionChangedByKey();

					if(beforeSelectionChange == this.Text && !this.PopupContainer.IsShowing())
					{
						// Item with same Text detected, so event wouldn't have fired.
						this.OnSelectionChangeCommitted(EventArgs.Empty);
					}
				}
			}
			else
				beforeSelectionChange = String.Empty;
		}

       
		protected virtual void OnSelectionChangedByKey()
		{
			// Fire an event only if not drop-down.
			this.UpdateText(!this.PopupContainer.IsShowing());
		}
        /// <summary>
        /// Raises the Validating event.
        /// </summary>
       
		protected override void OnValidating(CancelEventArgs e)
		{
			if(this.UpdateSelectionBeforeValidate)
				this.OnUpdateSelectionBeforeValidate();

			base.OnValidating(e);
		}

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_KILLFOCUS && preventLostFocus)
                return;
            base.WndProc(ref m);
        }

        /// <summary>
        /// Called to update selection before validate.
        /// </summary>
         /// <remarks>
        /// This is useful when you enable auto completion
        /// in the text area and force the list box's SelectedValue to be updated to the
        /// latest text value in this control's Validated event.
        /// </remarks>

		protected virtual void OnUpdateSelectionBeforeValidate()
		{
            if(this.PopupControl != null && this.PopupControl.Visible)
			    this.UpdatePopupControl();
		}

		protected override void PerformTextAutoComplete(KeyPressEventArgs e)
		{
			if( this.AutoComplete )
			{
				base.PerformTextAutoComplete( e );
			}
		}

		private void RaiseAutoCompleteChanged()
		{
			if( this.AutoCompleteChanged != null )
			{
				this.AutoCompleteChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnAutoCompleteChanged()
		{
			RaiseAutoCompleteChanged();
		}
		#endregion TEXTBOX

		#region DROPDOWN

      [Syncfusion.Documentation.DocumentationExclude()]
		protected override bool HasListInterface()
		{
			return true;
		}
        /// <summary>
        /// Called before the popup gets dropped down.
        /// </summary>
		protected override void OnBeforePopup()
		{
			if(this.ListControl != null)
			{
				this.SetPopupText(this.Text);
				// The trouble with the ListBox type controls is that 2 different items 
				// could have the same text value! So caching the selected value to make
				// sure whether or not the selected value has changed.
				this.cachedSelectedValue = this.ListControl.SelectedValue;
			}
			base.OnBeforePopup();
		}
       
        /// <summary>
        /// Sets the popup text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <remarks>Compares the new PopUpText and resets if a match is not found. </remarks>
		protected override void SetPopupText(string value)
		{
			base.SetPopupText(value);
			if(this.GetPopupText().CompareTo(value) != 0)
			{
				// If the new text didn't take, then there was no match, so reset the index.
				this.ListControl.SelectedIndex = -1;
			}
		}


        [Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnNoChangeDetectedOnPopupClosed()
		{
			// Still haven't fired the event, so make sure if the two items had the same text value.
			if(this.ListControl != null)
			{
				if(this.cachedSelectedValue != this.ListControl.SelectedValue)
					this.OnSelectionChangeCommitted(EventArgs.Empty);
			}
			base.OnNoChangeDetectedOnPopupClosed();
		}
		#endregion DROPDOWN

		#region MOUSE_MESSAGES

        /// <summary>
        /// Handles the PopupCloseType of PopUpContainer based on SelectedIndex.
        /// </summary>
		protected override void OnMouseUpOnQuickSelect()
		{
			if(this.ListControl.SelectedIndex != -1)
				this.PopupContainer.HidePopup(PopupCloseType.Done);
			else
				this.PopupContainer.HidePopup(PopupCloseType.Canceled);

			base.OnMouseUpOnQuickSelect();
		}

		
		#endregion MOUSE_MESSAGES

		#region LISTCONTROL
		/// <summary>
		/// Indicates whether the attached ListControl has an "IndexFromPoint"
		/// method.
		/// </summary>
		/// <returns>True if such a method is available; False otherwise.</returns>
		/// <remarks>
		/// This control relies on the attached <b>ListControl</b> providing this method
		/// to support quick-selection mode. This is the mode where the user can click on the
		/// drop-down arrow and start selecting in the drop-down list even before mouse up.
		/// </remarks>
		protected virtual bool IsIndexFromPointAvaillable()
		{
			if(this.ListControl == null)
				return false;

			Type listType = this.ListControl.GetType();
			MethodInfo mInfo = listType.GetMethod("IndexFromPoint", new Type[]{typeof(Point)});

			// Move selection using the IndexFromPoint method.
			if(mInfo != null)
				return true;
			else
				return false;
		}
		/// <summary>
		/// Returns the index of the item at the specified point.
		/// </summary>
		/// <param name="pointScreen">A <see cref="System.Drawing.Point"/> in screen coordinates.</param>
		/// <returns>The zero based index; -1 if the point is not over an item.</returns>
		protected virtual int GetIndexFromPoint(Point pointScreen)
		{
			if(this.ListControl == null)
				return -1;

			Point pointClient = this.ListControl.PointToClient(pointScreen);
			if(!this.ListControl.ClientRectangle.Contains(pointClient))
				return -1;

			Type listType = this.ListControl.GetType();
			MethodInfo mInfo = listType.GetMethod("IndexFromPoint", new Type[]{typeof(Point)});

			// Move selection using the IndexFromPoint method.
			if(mInfo != null)
			{
				return (int)mInfo.Invoke(this.ListControl, new object[]{pointClient});
			}
			else
				return -1;
		}
		/// <summary>
		/// Called when a new <see cref="System.Windows.Forms.ListControl"/> gets attached
		/// to this ComboBoxBase using the <see cref="ListControl"/> property.
		/// </summary>
		/// <remarks>
		/// When you override this method, make sure to call the base class for proper initialization.
		/// </remarks>
		protected override void AttachPopupControl()
		{
			base.AttachPopupControl();

			if(!this.DesignMode)
			{
				this.ListControl.SelectedValueChanged += new EventHandler(this.List_SelectedValueChanged);
				this.ListControl.MouseUp += new MouseEventHandler(this.List_MouseUp);
				this.ListControl.MouseMove += new MouseEventHandler(this.List_MouseMove);
				this.ListControl.MouseDown += new MouseEventHandler(this.List_MouseDown);
				this.ListControl.Click += new EventHandler(this.ListControl_Click);
				this.ListControl.GotFocus += new EventHandler(this.ListControl_GotFocus);
			}
		}

		
		/// <summary>
		/// Called when an attached <see cref="System.Windows.Forms.ListControl"/> is being
		/// detached from this combo.
		/// </summary>
		/// <param name="disposing">Indicates whether this method is called from <b>Dispose</b>.</param>
		protected override void DetachPopupControl(bool disposing)
		{
			if(!this.DesignMode && this.ListControl != null)
			{
				this.ListControl.SelectedValueChanged -= new EventHandler(this.List_SelectedValueChanged);
				this.ListControl.MouseUp -= new MouseEventHandler(this.List_MouseUp);
				this.ListControl.MouseMove -= new MouseEventHandler(this.List_MouseMove);
				this.ListControl.MouseDown -= new MouseEventHandler(this.List_MouseDown);
				this.ListControl.Click -= new EventHandler(this.ListControl_Click);
				this.ListControl.GotFocus -= new EventHandler(this.ListControl_GotFocus);
			}
			base.DetachPopupControl(disposing);
		}
		/// <summary>
		/// Called when the user clicks on the associated list box.
		/// </summary>
		/// <param name="e">The MouseEventArgs from the list box's MouseUp event.</param>
		protected virtual void OnListMouseUp(MouseEventArgs e)
		{
			if(this.PopupContainer.IsShowing())
			{
				Point ptScreen = this.ListControl.PointToScreen(new Point(e.X, e.Y));
				MouseClickCancelEventArgs args = new MouseClickCancelEventArgs(ptScreen, false);
				this.OnDropDownCloseOnClick(args);
				if(!args.Cancel)
				{
					SelectedIndexChangingArgs sica =
						new SelectedIndexChangingArgs( m_lastFoundIndex, this.ListControl.SelectedIndex );

					this.OnSelectedIndexChanging(sica);

					if( !sica.Cancel )
					{
						m_lastFoundIndex = this.ListControl.SelectedIndex;
						// If no selection (like when clicked on the header in a grid) simply cancel popup.
						if( this.ListControl.SelectedIndex != -1 )
							this.PopupContainer.HidePopup( PopupCloseType.Done );
						else
							this.PopupContainer.HidePopup( PopupCloseType.Canceled );
					}
				}
			}

			if(this.DropDownStyle == ComboBoxStyle.Simple
				&& this.IsIndexFromPointAvaillable())
			{
				this.ignorePopupValueChange = false;
				// Get the selected index using the IndexFromPoint method.
				Point ptScreen = this.ListControl.PointToScreen(new Point(e.X, e.Y));
				int index = this.GetIndexFromPoint(ptScreen);
				// When index == -1, we don't make any change to the current selection.
				if(index != -1
					&& index != this.indexOnListMouseDown)
				{
					if(index == -1)
						this.UpdatePopupControl();
					else
					{
						SelectedIndexChangingArgs sica =
							new SelectedIndexChangingArgs( index, this.ListControl.SelectedIndex );

						this.OnSelectedIndexChanging(sica);

						if( !sica.Cancel )
						{
							this.OnListSelectedValueChangedWhileNotShowingPopup(index);

							if(this.TextBox.Visible)
								this.TextBox.SelectAll();
						}
					}
				}
			}
		}
		private void List_MouseUp(object sender, MouseEventArgs e)
		{
			this.OnListMouseUp(e);
		}
		
		private void ListControl_GotFocus(object sender, EventArgs e)
		{
			if(this.DropDownStyle == ComboBoxStyle.Simple)
			{
				if(this.TextBox.Visible)
				{
					this.TextBox.Focus();
					this.TextBox.SelectAll();
				}
				else
					this.Focus();
			}
		}
		private void ListControl_Click(object sender, EventArgs e)
		{
			if(this.DropDownStyle == ComboBoxStyle.Simple)
			{
				this.OnClick(e);
			}
		}
		private void List_MouseDown(object sender, MouseEventArgs e)
		{
			if(this.DropDownStyle == ComboBoxStyle.Simple
				&& this.IsIndexFromPointAvaillable())
			{
				// Update the selected index, since we make the list box unselectable (by handling its GotFocus and transferring focus to the text box).

				// Get the selected index using the IndexFromPoint method.
				Point ptScreen = this.ListControl.PointToScreen(new Point(e.X, e.Y));
				int index = this.GetIndexFromPoint(ptScreen);
				if(index != -1)
				{
					this.ignorePopupValueChange = true;
					this.indexOnListMouseDown = this.ListControl.SelectedIndex;
					this.ListControl.SelectedIndex = index;
				}
			}
		}
		private void List_MouseMove(object sender, MouseEventArgs e)
		{
			if(!this.ignoreNextPopupControlMouseMove)
				this.ProcessListMouseMove(sender, e);
			else
				this.ignoreNextPopupControlMouseMove = false;
		}

        /// <summary>
        /// Processes the mouse move over the PopupControl.
        /// </summary>
        
		protected override void ProcessPopupControlMouseMove(object sender, MouseEventArgs e)
		{
			this.ProcessListMouseMove(sender, e);
		}

		/// <summary>
		/// Processes mouse move over the list.
		/// </summary>
		/// <param name="sender">The control source of the MouseMove event.</param>
		/// <param name="e">The event args of the MouseMove event.</param>
		/// <remarks>
		/// This method moves the selection of the attached <see cref="ListControl"/> in some
		/// cases when the mouse moves over it and also to change the Top Index of the list control appropriately
		/// when QuickSelection is on.
		/// </remarks>
		protected virtual void ProcessListMouseMove(object sender, MouseEventArgs e)
		{
			IList listControlList = this.GetListControlList();

			if(!this.ContainsFocus || this.DropDownStyle == ComboBoxStyle.Simple
				|| listControlList == null)
				return;

			// Get the client co-ords in terms of the list.
			Control source = sender as Control;
			Point mouseMoveScreen = new Point(e.X, e.Y);
			mouseMoveScreen = source.PointToScreen(mouseMoveScreen);
			Point mouseMoveClient = this.ListControl.PointToClient(mouseMoveScreen);

			if(this.ListControl.ClientRectangle.Contains(mouseMoveClient)
				&& Control.MouseButtons == MouseButtons.Left)
				this.QuickSelectOn = true;

			// Move selection using the IndexFromPoint method.
			if(this.IsIndexFromPointAvaillable())
			{
				int index = this.GetIndexFromPoint(mouseMoveScreen);
				if( this.AllowQuickSelection && (this.QuickSelectOn || index > -1) && index < listControlList.Count)
				{
					this.ListControl.SelectedIndex = index;
					if(index == -1 && this.ListControl.SelectedIndex != -1)
						index++;
				}
			}
			// Update the TopIndex.
			if(this.AllowQuickSelection && this.QuickSelectOn && !this.ListControl.ClientRectangle.Contains(mouseMoveClient))
			{
				bool moveUp = this.ListControl.ClientRectangle.Bottom < mouseMoveClient.Y;
				bool moveDown = this.ListControl.ClientRectangle.Top > mouseMoveClient.Y;

				Type listType = this.ListControl.GetType();

				if(moveUp || moveDown)
				{
					// Check if the list has a TopIndex property
					PropertyInfo pInfo = listType.GetProperty("TopIndex", typeof(int));
					if(pInfo != null)
					{
						int index = (int)pInfo.GetValue(this.ListControl, new object[]{});
						if(moveUp)
							index++;
						else if(moveDown)
							index--;
						if(index > -1 || index >= listControlList.Count)
							pInfo.SetValue(this.ListControl, index, new object[]{});
					}
				}
			}
		}

		private void List_SelectedValueChanged(object sender, EventArgs e)
		{
			// If in Simple mode do not process this if mouse down on List.
			if(this.DropDownStyle == ComboBoxStyle.Simple 
				&& Control.MouseButtons == MouseButtons.Left
				&& this.ContainsFocus
				&& this.IsIndexFromPointAvaillable())
				this.ignorePopupValueChange = true;

			if(this.disposing)
				return;

			
			if( this.PopupContainer != null )
			{
				if(!this.ignorePopupValueChange && (this.DropDownStyle == ComboBoxStyle.Simple
					|| !this.PopupContainer.IsShowing()))
				{
					this.UpdateText(false);
					//this.OnListSelectedValueChangedWhileNotShowingPopup();
				}
			}
		}

		[Documentation.DocumentationExclude(), Browsable(false)]
		protected virtual void OnListSelectedValueChangedWhileNotShowingPopup(int newIndex)
		{
			this.UpdateText(true);
		}

		/// <summary>
		/// Indicates whether the supplied text is a valid entry in the attached <see cref="ListControl"/>.
		/// </summary>
		/// <param name="text">The text to validate.</param>
		/// <returns>True if valid; False otherwise.</returns>
		protected override bool IsTextValid(string text)
		{
			int match = -1;

			IList list = this.GetListControlList();
		
			if(list != null)
			{
				for(int i = 0; i < list.Count; i++)
				{
					object item = list[i];
					if(this.ListControl.GetItemText(item) == text)
					{
						match = i;
						break;
					}
				}
			}
			
			return match != -1;
		}

		#endregion

		#region MISC

		/// <summary>
		/// Correct popup control location.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		protected virtual Point CorrectPopupLocation( Point location )
		{
			return location;
		}

		#endregion

		#region OVERRIDES

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.PopupContainer.PopupHost != null )
				{
					this.PopupContainer.PopupHost.Dispose();
					this.PopupContainer.PopupHost = null;
				}
			}

			base.Dispose( disposing );
		}

		#endregion

		internal class ComboBoxBasePopUpHost:
			PopupHost
		{
			protected override Point GetAdjustedLocation( Point loc )
			{
				Point ptBaseReturn =  base.GetAdjustedLocation(loc);
				PopupControlContainer container = this.PopupControlContainer as PopupControlContainer;

				if( null != container )
				{
					Control parent = container.ParentControl;

					if( null != parent && null != parent.Parent )
					{
						Rectangle bounds = parent.Bounds;
						Point ptLocalion = parent.Parent.PointToScreen(bounds.Location);

						if( (ptLocalion.Y + bounds.Height) > ptBaseReturn.Y )
						{
							ptBaseReturn.Y = ptLocalion.Y - this.Height;
						}

						ComboBoxBase comboBox = parent as ComboBoxBase;
						
						if( comboBox != null )
						{
							ptBaseReturn = comboBox.CorrectPopupLocation( ptBaseReturn );
						}
					}
				}

				return ptBaseReturn;
			}

			protected override void AttachPopup()
			{
				base.AttachPopup();

				if( this.PopupControl != null )
				{
					this.PopupControl.SizeChanged += new EventHandler( PopupControlSizeChanged );
				}
			}
            protected override void DetachPopup()
            {
                base.DetachPopup();

                if( this.PopupControl != null )
                {
                    this.PopupControl.SizeChanged -= new EventHandler(PopupControlSizeChanged);
                }
            }
			private void PopupControlSizeChanged( object sender, EventArgs e )
			{
				if( this.PopupControl != null )
				{
					ComputeMySize();
				}
			}

			protected override void Dispose( bool disposing )
			{
				if( disposing && this.PopupControl != null )
				{
					this.PopupControl.SizeChanged -= new EventHandler( PopupControlSizeChanged );
				}

				base.Dispose( disposing );
			}
		}

        internal class ComboBoxPopupContainer :
            PopupControlContainer
        {
            public override bool IsRelatedControl( Control control, bool askPopupParent )
		    {
			    if( !this.IsShowing() )
				{
                    return false;
                }

			    if( control == this || this.Contains( control ) || control == this.Parent || 
                    ( control is VScrollBarCustomDraw && this.Bounds.Contains( control.Bounds ) ) )
				{
                    return true;
                }
			    else if( askPopupParent )
			    {
				    if( this.PopupParent != null )
					{
                        return this.PopupParent.IsRelatedControl( control, askPopupParent );
                    }
				    else if( control != null && this.ParentControl != null &&
					         ( control == this.ParentControl || this.ParentControl.Contains( control ) ) )
					{
                        return true;
                    }
			    }

			    return false;
		    }
        }
	}

	/// <summary>
	/// Handles a cancelable mouse-click event.
	/// </summary>
	public delegate void MouseClickCancelEventHandler(object sender, MouseClickCancelEventArgs args);

	/// <summary>
	/// Provides data for a cancelable mouse click event.
	/// </summary>
	public class MouseClickCancelEventArgs : CancelEventArgs
	{
		Point ptScreen;
		/// <summary>
		/// Creates a new instance of the MouseClickCancelEventArgs.
		/// </summary>
		/// <param name="ptScreen">The point in screen coordinates where the click occurred.</param>
		/// <param name="cancel">True to cancel the event; False otherwise.</param>
		public MouseClickCancelEventArgs(Point ptScreen, bool cancel):base(cancel)
		{
			this.ptScreen = ptScreen;
		}
		/// <summary>
		/// Returns the point in screen coordinates where the click occurred.
		/// </summary>
		public Point MouseClickPoint
		{
			get{return ptScreen;}
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public class SelectedIndexChangingArgs: CancelEventArgs
	{
		#region Construction
			public SelectedIndexChangingArgs( int nPrevIndex, int nNewIndex, bool bCancel ):
				base(bCancel)
			{
				m_nPrevIndex = nPrevIndex;
				m_nNewIndex = nNewIndex;
			}

			public SelectedIndexChangingArgs( int nPrevIndex, int nNewIndex ):
				this( nPrevIndex, nNewIndex, false )
			{
			}
		#endregion Construction
		#region Properties
			public int PrevIndex
		{
			get
			{
				return m_nPrevIndex;
			}
			set
			{
				m_nPrevIndex = value;
			}
		}
			public int NewIndex
		{
			get
			{
				return m_nNewIndex;
			}
			set
			{
				m_nNewIndex = value;
			}
		}
		#endregion Properties
		#region Members
			private int m_nPrevIndex = -1;
			private int m_nNewIndex = -1;
		#endregion Members
	}
    /// <summary>
    /// ComboDropDown Designer
    /// </summary>
    public class ComboBaseDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ComboBaseDesigner()
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
                    this.actionLists.Add(new ComboBoxBaseActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
	
	public delegate void SelectedIndexChangingHandler( object sender, SelectedIndexChangingArgs e );

	/// <summary>
	/// The ComboBoxExt type will soon be replaced with the ComboBoxBase for consistency in 
	/// control naming in our library. 
	/// Please replace all occurrences of ComboBoxExt with ComboBoxBase in your application.
	/// </summary>
	[Obsolete("The ComboBoxExt type will soon be replaced with the ComboBoxBase for consistency in naming in our library. Please replace all occurrences of ComboBoxExt with ComboBoxBase in your app."),
	ToolboxItem(false)]
	public class ComboBoxExt : ComboBoxBase
	{
		public ComboBoxExt():base(){}
	}
}
