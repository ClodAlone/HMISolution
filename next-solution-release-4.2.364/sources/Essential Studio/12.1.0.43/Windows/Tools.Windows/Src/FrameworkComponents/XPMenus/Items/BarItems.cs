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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;

using Syncfusion.Runtime.Serialization;
using System.Runtime.InteropServices.ComTypes;
using Syncfusion.Windows.Forms.Tools.Design;
using EnvDTE;
using System.Collections.Generic;


namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IParentBarItem
	{
		ParentBarItemStyle ParentStyle { get;set;}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class SubMenuPaintEventArgs
	{
		private PaintEventArgs args;
		private Rectangle bgBounds;
		private Rectangle itemBounds;

		public SubMenuPaintEventArgs(PaintEventArgs args, Rectangle bgBounds, Rectangle itemBounds)
		{
			this.args = args;
			this.bgBounds = bgBounds;
			this.itemBounds = itemBounds;
		}

		public Rectangle Bounds
		{
			get { return this.bgBounds; }
		}
		public Rectangle ItemBounds
		{
			get { return this.itemBounds; }
		}
		public PaintEventArgs PaintEventArgs
		{
			get { return this.args; }
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void SubMenuPaintEventHandler(object sender, SubMenuPaintEventArgs args);

	/// <summary>
	/// A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> derived class that will drop down a <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> when the user
	/// clicks on it.
	/// </summary>
	/// <remarks>
	/// This <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> derived class allows you to show custom drop-downs from
	/// submenus, context menus and tool bars. Use the <see cref="PopupControlContainer"/> property to
	/// associate this item with a <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>.
	/// <para>You can use this to display for example a Color picker drop-down (by placing a <see cref="Syncfusion.Windows.Forms.ColorUIControl"/>
	/// in a derived <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>.</para>
	/// </remarks>
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	Serializable()
	]
	public class DropDownBarItem :
		BarItemEx,
		ICloneable,
		IParentBarItem,
		ISerializable,
		IIgnoreWorkingArea
	{
		private PopupControlContainer popupContainer;
		private ParentBarItemStyle parentStyle = ParentBarItemStyle.DropDown;
		#region CONSTRUCTORS
		/// <summary>
		///	Overloaded. Initializes a new instance of the DropDownBarItem class with default settings.
		/// </summary>
		public DropDownBarItem()
			: this(MenuMerge.Add, 0, Shortcut.None, "", null, null, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the DropDownBarItem class and sets its
		/// caption.
		/// </summary>
		/// <param name="text">The DropDownBarItem's caption.</param>
		public DropDownBarItem(string text)
			: this(MenuMerge.Add, 0, Shortcut.None, text, null, null, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the DropDownBarItem class, sets its caption
		/// and event handler for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event of the DropDownBarItem.
		/// </summary>
		/// <param name="text">The DropDownBarItem's caption.</param>
		/// <param name="onClick">The event handler for the ItemClick event.</param>
		public DropDownBarItem(string text, EventHandler onClick)
			: this(MenuMerge.Add, 0, Shortcut.None, text, onClick, null, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the DropDownBarItem class, sets its caption,
		/// event handler for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event and
		/// the Shortcut for this item.
		/// </summary>
		/// <param name="text">The DropDownBarItem's caption.</param>
		/// <param name="onClick">The event handler for the ItemClick event.</param>
		/// <param name="shortcut">The Shortcut for this item.</param>
		public DropDownBarItem(string text, EventHandler onClick, Shortcut shortcut)
			: this(MenuMerge.Add, 0, shortcut, text, onClick, null, null)
		{
		}
		/// <summary>
		/// Initializes a new instance of the DropDownBarItem class, sets its merge type,
		/// merge order, shortcut, caption, event handler for ItemClick, event handler
		/// for Popup and the event handler for Selected.
		/// </summary>
		/// <param name="mergeType">The item's <see cref="MenuMerge"/></param>
		/// <param name="mergeOrder">The item's merge order.</param>
		/// <param name="shortcut">The item's Shortcut.</param>
		/// <param name="text">The item's caption.</param>
		/// <param name="onClick">The handler for the ItemClick event.</param>
		/// <param name="onPopup">The handler for the Popup event.</param>
		/// <param name="onSelect">The handler for the Select event.</param>
		public DropDownBarItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect)
			: base(mergeType, mergeOrder, shortcut, text, onClick, onPopup, onSelect)
		{
			this.Init();
		}
		private void Init() { }
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.popupContainer != null)
				{
					this.popupContainer.Dispose();
					this.popupContainer = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion CONSTRUCTORS

		#region PROPERTIES
		/// <summary>
		/// Gets or sets the PopupControlContainer that will be dropped down from this item when placed in a menu or toolbar.
		/// </summary>
		/// <remarks>
		/// A PopupControlContainer that represents the popup control associated
		/// with this Item.
		/// <para>
		/// This item will popup the specified PopupControlContainer when the user clicks
		/// on the drop-down portion of the bar item.
		/// </para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// <para>The set PopupControlContainer will be disposed when this DropDownBarItem is disposed.</para>
		/// </remarks>
		[
		DefaultValue(null),
		Category("Behavior"),
		Description("PopupControlContainer that will be dropped down from this item when placed in a menu or toolbar.")
		]
		public PopupControlContainer PopupControlContainer
		{
			get
			{
				return this.popupContainer;
			}
			set
			{
				if (this.popupContainer != value)
				{
					this.popupContainer = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "PopupControlContainer", null, null));
				}
			}
		}
		/// <summary>
		/// Gets or sets the ParentStyle in which the menu will be drawn.
		/// </summary>
		/// <remarks>
		/// A ParentBarItemStyle value indicating the ParentStyle in which the menu
		/// will be drawn. The default value is ParentBarItemStyle.DropDown.
		/// <para>Take a look at the documentation for the ParentBarItemStyle enumeration
		/// for more information on the interpretation of each ParentBarItemStyle value.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(ParentBarItemStyle.DropDown),
		Category("Behavior"),
		Localizable(true),
		Description("Indicates the ParentBarItemStyle in which the menu will be drawn.")
		]
		public virtual ParentBarItemStyle ParentStyle
		{
			get
			{
				return this.parentStyle;
			}
			set
			{
				if (this.parentStyle != value)
				{
					this.parentStyle = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "ParentStyle", null, null));
				}
			}
		}
		#endregion PROPERTIES
		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this DropDownBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this DropDownBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of DropDownBarItem and calls the <see cref="BarItem.CopyTo"/> method to copy over properties.
		/// It does not copy over the <see cref="PopupControlContainer"/> property value.
		/// </remarks>
		public override object Clone()
		{
			DropDownBarItem newItem = new DropDownBarItem();
			base.CopyTo(newItem);
			return newItem;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
		protected DropDownBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
		}
		#endregion

		#region IIgnoreWorkingArea Members

		private bool m_bIgnoreWorkingArea = false;
		[DefaultValue(false)]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				m_bIgnoreWorkingArea = value;
			}
		}

		#endregion
	}
	/// <summary>
	/// Represents a bar item that provides the combo box functionality in the XP Menus framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This item provides you the flat look combo-boxes in your menus and toolbars.
	/// The item can operate in either editable or listbox mode (controlled by the Editable property).
	/// The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.TextBoxValue"/> represents the current Text in the text box selected by the user.
	/// </para>
	/// <para>
	/// When in list box mode, you can fill the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.ChoiceList"/> with the choices you want to provide the user.</para>
	/// <para>When in editable mode, you can turn on <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.AutoAppend"/> which will make the framework track and persist
	/// the entries made by the user and automatically append them to the choice list
	/// in the drop-down. Turning on AutoAppend will ignore the entries you made in the ChoiceList.
	/// The entries made by the user will be stored in the registry for reuse across applications. This entry will
	/// be associated with a custom AutoAppend category ID which will be synthesized based on the ComboBoxBarItem's text and CategoryID property.
	/// </para>
	/// <para>
	/// When in editable mode, a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event will be thrown when the user presses the Return key
	/// when in the TextBox or when the user selects an item from the drop-down.
	/// When in list box mode, an <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event will be thrown when the user selects an
	/// item from the drop-down list box.
	/// </para>
	/// <para>
	/// Use the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.MinWidth"/> property to provide a minimum width the corresponding bar item should be drawn with.
	/// The actual width might be bigger than the provided MinWidth in some cases.
	/// </para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package\ folder
	/// for usage example.
	/// </example>
	[Serializable()]
	public class ComboBoxBarItem :
		BarItemEx,
		IRequiresControl,
		ICloneable,
		IIgnoreWorkingArea,
		ISupportInitialize
	{
		private bool m_bCloseOnClick = true;

		private bool editable = true;
		private bool autoAppend = false;
		private StringCollection list;
		private string curText = String.Empty;
		private bool persistTextBoxValue = false;
		private int minwidth = 50;
		private ListBox listBox = null;
		private int maxDropDownItems = 8;
		private int dropDownWidth = -1;
        private bool persistAutoAppendList = true;
		private bool deserializingValue = false;
		static bool s_isDevEnv = (Application.ExecutablePath.ToLower().IndexOf("devenv.exe") >= 0);
		/// <summary>
		///Lets handle the textBox inside the ComboBox.
		/// </summary>
		[Description("Lets handle the textBox inside the ComboBox.")]
		public event TextBoxBoundEventHandler TextBoxBound;
		/// <summary>
		/// Raises the TextBoxBound event.
		/// </summary>
		public virtual void OnTextBoxBound(TextBoxBoundEventArgs e)
		{
			if (this.TextBoxBound != null)
				this.TextBoxBound(this, e);
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		public void BeginInit()
		{
			OnBeginInit();
		}

		/// <summary>
		/// Begins the initialization.
		/// </summary>
		protected virtual void OnBeginInit()
		{
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		public void EndInit()
		{
			OnEndInit();
		}

		/// <summary>
		/// Indicates  that the initialization is complete.
		/// </summary>
		protected virtual void OnEndInit()
		{
			if (!DesignMode && ListBox != null)
			{
				ListBox.Visible = false;
			}
		}

		/// <summary>
		/// Creates a new instance of the ComboBoxBarItem class.
		/// </summary>
		public ComboBoxBarItem()
		{
			this.Init();
		}
		private void Init()
		{
			list = new StringCollection();
		}

		private void SaveTextBoxValue()
		{
			if (!this.PersistTextBoxValue)
				return;

			if (this.DesignMode || s_isDevEnv)
				return;

			BarItemID barItemID = new BarItemID(this.ID,
				this.Manager != null ?
				BarManager.GetFormTypeName(this.Manager) : String.Empty);

			AppStateSerializer.GetSingleton().SerializeObject(barItemID.ToString(), this.TextBoxValue);
		}

		private void GetTextBoxValue()
		{
			if (!this.PersistTextBoxValue)
				return;

			if (this.DesignMode || s_isDevEnv)
				return;

			BarItemID barItemID = new BarItemID(this.ID,
				this.Manager != null ?
				BarManager.GetFormTypeName(this.Manager) : String.Empty);

			this.deserializingValue = true;
			try
			{
				object o = AppStateSerializer.GetSingleton().DeserializeObject(barItemID.ToString());
				if (o != null && o is string)
				{
					this.TextBoxValue = (string)o;
				}
			}
			finally
			{
				this.deserializingValue = false;
			}
		}

		private object _value;
		object IRequiresControl.Value
		{
			get
			{
				return (_value == null) ? this.TextBoxValue : _value;
			}
			set
			{
				_value = value;
				this.TextBoxValue = (value != null) ? value.ToString() : string.Empty;
			}
		}

		public override string ID
		{
			get { return base.ID; }
			set
			{
				if (base.ID != value)
				{
					base.ID = value;
					this.GetTextBoxValue();
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum width of this item when placed in a menu or toolbar.
		/// </summary>
		/// <value>
		/// The minimum width with which this item should be drawn.
		/// </value>
		/// <remarks><para>Changing this property will fire the PropertyChanged event.</para></remarks>
		[DefaultValue(50),
		Localizable(true),
		Category("Layout"),
		Description("Minimum width of this item when placed in a menu or toolbar.")]
		public virtual int MinWidth
		{
			get
			{
				return this.minwidth;
			}
			set
			{
				if (this.minwidth != value)
				{
					this.minwidth = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "MinWidth", null, null));
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of items to be shown in the drop-down portion of the ComboBoxBarItem.
		/// </summary>
		/// <value>The maximum number of items in the drop-down portion. The minimum for this property is 1 and the maximum is 100.</value>
		/// <exception cref="ArgumentException">The maximum number is set less than one or greater than 100.</exception>
		[DefaultValue(8), Category("Appearance"),
			Description("Gets or sets the maximum number of items to be shown in the drop-down portion of the ComboBoxBarItem.")]
		public virtual int MaxDropDownItems
		{
			get
			{
				return this.maxDropDownItems;
			}
			set
			{
				if (value < 1 || value > 100)
					throw new ArgumentException("MaxDropDownItems should be within the range 1 to 100.");
				this.maxDropDownItems = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the dropdown.
		/// </summary>
		/// <value>-1 indicates the dropdown width will be as wide as the combo.
		/// Default is -1. The width could be bigger if the combo's width is bigger.</value>
		[DefaultValue(-1), Category("Layout"),
			Description("Specifies the minimum width of the dropdown.")]
		public virtual int MinDropDownWidth
		{
			get
			{
				return this.dropDownWidth;
			}
			set
			{
				this.dropDownWidth = value;
			}
		}


		/// <summary>
		/// Indicates whether the DropDown list should close when an item is selected.
		/// </summary>
		[DefaultValue(true),
		Category("Appearance"),
		Description("Indicates whether the DropDown list should close when an item is selected.")]
		public virtual bool CloseOnClick
		{
			get
			{
				return m_bCloseOnClick;
			}
			set
			{
				if (value != m_bCloseOnClick)
				{
					m_bCloseOnClick = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the user can edit the value in this item with a TextBox.
		/// </summary>
		/// <value>True to let the user edit the value through a TextBox. False to draw this in list box mode.
		/// <para>Changing this property will fire the PropertyChanged event.</para></value>
		[DefaultValue(true),
		Category("Behavior"),
		Description("Indicates whether the user can edit the value in this item with a TextBox.")]
		public bool Editable
		{
			get { return this.editable; }
			set
			{
				if (this.editable != value)
				{
					this.editable = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Editable", null, null));
				}
			}
		}

		/// <summary>
		/// Indicates whether to automatically append items
		/// entered by the user in the TextBox into the drop-down list.
		/// </summary>
		/// <value>True to turn on auto append; flase otherwise.</value>
		/// <remarks>
		/// This property will be used only when in editable mode.
		/// <para>Changing this property will fire the PropertyChanged event.</para>
		/// </remarks>
		[DefaultValue(false),
		Category("Behavior"),
		Description("Indicates whether or not to automatically append items	entered by the user in the TextBox into the drop-down list.")]
		public bool AutoAppend
		{
			get { return this.autoAppend; }
			set
			{
				if (this.autoAppend != value)
				{
					this.autoAppend = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "AutoAppend", null, null));
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether to persist autoappend list when autoappend is true.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [persist auto append list]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true),
        Category("Behavior"),
        Description("Indicates whether or not to persist autoappend list when autoappend is true")]
        public bool PersistAutoAppendList
        {
            get { return this.persistAutoAppendList; }
            set
            {
                this.persistAutoAppendList = value;
            }
        }

		/// <summary>
		/// Returns the choice list for the user.
		/// </summary>
		/// <value>A StringCollection representing the choices available to the user.</value>
		/// <remarks>This collection will be reset when you turn-on AutoAppend and when in editable mode.
		/// <para>Changing this property will fire the PropertyChanged event.</para></remarks>
		[
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			Category("Data"),
			Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
				"System.Drawing.Design.UITypeEditor, System.Drawing"),
			Description("Returns the choice list for the user.")
		]
		public StringCollection ChoiceList
		{
			get { return this.list; }
		}

		/// <summary>
		/// Gets or sets the value in the TextBox.
		/// </summary>
		/// <value>The string representing the value in the TextBox.</value>
		/// <remarks><para>Changing this property will fire the PropertyChanged event.</para></remarks>
		[DefaultValue(""),
		Category("Data"),
		Description("Specifies the value in the TextBox."),
		Bindable(BindableSupport.Yes)]
		public string TextBoxValue
		{
			get
			{
				if (this.curText == null)
					this.curText = String.Empty;
				return this.curText;
			}
			set
			{
				if (this.curText != value)
				{
					TextBoxValueChangeEventArgs e = new TextBoxValueChangeEventArgs(value);
					this.OnTextBoxValueChange(e);
					if (!e.Cancel)
					{
						value = e.NewValue;
						string sOldValue = this.curText;
						this.curText = value;

						_value = value;

						this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(
							PropertyChangeEffect.NeedRepaint, "TextBoxValue", sOldValue, value));

						if (!this.deserializingValue)
							this.SaveTextBoxValue();
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the TextBoxValue should be persisted when the application is shutdown.
		/// </summary>
		/// <value>True to persist; false otherwise. Default is false.</value>
		/// <remarks>This property need not be set when <see cref="AutoAppend"/> is true.
		/// When AutoAppend is turned on, the latest value will be persisted automatically.</remarks>
		[DefaultValue(false),
			Category("Behavior"),
			Description("Specifies whether or not the TextBoxValue should be persisted when the app. is shutsdown.")]
		public bool PersistTextBoxValue
		{
			get
			{
				return this.persistTextBoxValue;
			}
			set
			{
				if (this.persistTextBoxValue != value)
				{
					this.persistTextBoxValue = value;
					if (value)
					{
						this.GetTextBoxValue();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a custom ListBox in the combo's dropdown.
		/// </summary>
		/// <value>A ListBox instance. Default is null.</value>
		/// <remarks>
		/// <para>The ListBox specified using this property will be used in the dropdown region
		/// of the combo. When no value is specified a default ListBox will be used.</para>
		/// <para>Changing this property will fire the PropertyChanged event.</para>
		/// </remarks>
		[DefaultValue(null), Category("Data"),
			Description("Lets you specify a custom ListBox in the combo's dropdown.")]
		public ListBox ListBox
		{
			get { return this.listBox; }
			set
			{
				if (this.listBox != value)
				{
					ListBox old = this.listBox;
					this.listBox = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.None, "ListBox", old, this.listBox));
				}
			}
		}

		/// <summary>
		/// This event optionally lets you customize the listbox before it's drop down.
		/// </summary>
		/// <remarks>
		/// <para>This event will be fired after the user clicks on the dropdown button and
		/// before the listbox is shown.</para>
		/// <para>Also if this combo is editable, then this event will also be fired when
		/// the user uses the keyboard to browse through the different items.</para>
		/// </remarks>
		[Description("This event optionally lets you customize the listbox before it's drop down."),
		Category("Data")]
		public event ComboBoxBarItemInitListBoxEventHandler InitListBox;

		/// <summary>
		/// Raises the InitListBox event.
		/// </summary>
		/// <param name="e">An <see cref="ComboBoxBarItemInitListBoxEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnInitListBox method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnInitListBox in a derived
		/// class, be sure to call the base class's OnInitListBox method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		public virtual void OnInitListBox(ComboBoxBarItemInitListBoxEventArgs e)
		{
			if (this.InitListBox != null)
				this.InitListBox(this, e);
		}

		/// <summary>
		/// This event optionally lets TextBoxValue change.
		/// </summary>
		[Description("This event optionally lets TextBoxValue change.")]
		public event TextBoxValueChangeEventHandler TextBoxValueChange;

		/// <summary>
		/// Raises the TextBoxValueChange event.
		/// </summary>
		/// <param name="e"></param>
		public virtual void OnTextBoxValueChange(TextBoxValueChangeEventArgs e)
		{
			if (this.TextBoxValueChange != null)
			{
				BarManager mgr = this.Manager;

				if (null != mgr)
				{
					MessageFilterEntryHelper.Suspend(mgr.Form);
				}

				this.TextBoxValueChange(this, e);

				if (null != mgr)
				{
					MessageFilterEntryHelper.Resume(mgr.Form);
				}
			}
		}

		protected ComboBoxBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "MinWidth":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetInt32("MinWidth");
							this.MinWidth = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.MinWidth = (int)entry.Value;
						break;
					case "Editable":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetBool("Editable");
							this.Editable = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.Editable = (bool)entry.Value;
						break;
					case "AutoAppend":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetBool("AutoAppend");
							this.AutoAppend = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.AutoAppend = (bool)entry.Value;
						break;
					case "ChoiceList":
						StringCollection sc = (StringCollection)entry.Value;
						foreach (String s in sc)
							this.ChoiceList.Add(s);
						break;
					case "TextBoxValue":
						this.TextBoxValue = (string)entry.Value;
						break;
				}
			}
			//			this.MinWidth = info.GetInt32("MinWidth");
			//			this.Editable = info.GetBoolean("Editable");
			//			this.AutoAppend = info.GetBoolean("AutoAppend");
			//
			//			StringCollection sc = (StringCollection)info.GetValue("ChoiceList", typeof(StringCollection));
			//			foreach(String s in sc)
			//				this.ChoiceList.Add(s);
			//			this.TextBoxValue = info.GetString("TextBoxValue");
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("MinWidth", this.MinWidth);
			info.AddValue("Editable", this.Editable);
			info.AddValue("AutoAppend", this.AutoAppend);

			info.AddValue("ChoiceList", this.ChoiceList);

			info.AddValue("TextBoxValue", this.TextBoxValue);
		}

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this ComboBoxBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this ComboBoxBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of ComboBoxBarItem and calls the <see cref="CopyTo"/> method to copy over properties.
		/// </remarks>
		public override object Clone()
		{
			ComboBoxBarItem newItem = new ComboBoxBarItem();
			this.CopyTo(newItem);
			return newItem;
		}
		/// <summary>
		/// Copies the properties of this ComboBoxBarItem into the specified BarItem.
		/// </summary>
		/// <param name="barItem">The ComboBoxBarItem where the values should be copied to.</param>
		/// <remarks>
		/// This method will also call <see cref="BarItem.CopyTo"/> to copy over base class properties.
		/// </remarks>
		public override void CopyTo(BarItem barItem)
		{
			if (!(barItem is ComboBoxBarItem))
				throw new ArgumentException("barItem should be of type ComboBoxBarItem.", "barItem");
			base.CopyTo(barItem);

			ComboBoxBarItem cbitem = barItem as ComboBoxBarItem;

			cbitem.MinWidth = this.MinWidth;
			cbitem.Editable = this.Editable;
			cbitem.AutoAppend = this.AutoAppend;
			foreach (string choice in this.ChoiceList)
				cbitem.ChoiceList.Add(choice);
			cbitem.TextBoxValue = this.TextBoxValue;
		}
		#endregion

		#region IIgnoreWorkingArea Members

		private bool m_bIgnoreWorkingArea = false;

		[DefaultValue(false)]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				m_bIgnoreWorkingArea = value;
			}
		}

		#endregion

		#region Internal methods
		/// <summary>
		/// 
		/// </summary>
		internal void OnDropDownOpened()
		{
			if (this.DownOpened != null)
			{
				this.DownOpened(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void OnDropDownClosed()
		{
			if (this.DownClosed != null)
			{
				this.DownClosed(this, EventArgs.Empty);
			}
		}
		#endregion

		#region Events

        /// <summary>
        /// Occurs when combo dropdown is opened.
        /// </summary>
        [Description("Occurs when combo dropdown is opened.")]
		public event EventHandler DownOpened;

        /// <summary>
        /// Occurs when combo dropdown is closed.
        /// </summary>
        [Description("Occurs when combo dropdown is closed.")]
		public event EventHandler DownClosed;

		#endregion
	}

	/// <summary>
	/// Represents a bar item that could be used as a label for an adjacent bar item.
	/// </summary>
	/// <remarks>
	/// The StaticBarItem does not respond to user mouse click and move.
	/// </remarks>
	[Serializable()]
	public class StaticBarItem :
		BarItemEx,
		ICloneable,
		ISerializable
	{
		private Color flatBorderColor = Color.Empty;

		/// <summary>
		/// Overloaded. Creates a new instance of the StaticBarItem class with default settings.
		/// </summary>
		public StaticBarItem()
		{
			this.Init();
		}
		/// <summary>
		/// Creates a new insatnce of the StaticBarItem class and initializes its Text property.
		/// </summary>
		public StaticBarItem(string text)
			: base(text)
		{
			this.Init();
		}

		private void Init() { }

		[Browsable(false)]
		public override bool Checked
		{
			get { return base.Checked; }
			set { base.Checked = value; }
		}
		[Browsable(false)]
		public override bool Enabled
		{
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[Browsable(false)]
		public override bool IsRecentlyUsedItem
		{
			get { return base.IsRecentlyUsedItem; }
			set { base.IsRecentlyUsedItem = value; }
		}
		[Browsable(false)]
		public override Shortcut Shortcut
		{
			get { return base.Shortcut; }
			set { base.Shortcut = value; }
		}
		/// <summary>
		/// Gets or sets the color of the flat border around this item when parented by a Bar (toolbar).
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value. Default is Color.Empty.</value>
		[Category("Appearance")]
		public virtual Color FlatBorderColor
		{
			get { return this.flatBorderColor; }
			set
			{
				if (this.flatBorderColor != value)
				{
					Color oldStyle = this.flatBorderColor;
					this.flatBorderColor = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "FlatBorderColor", oldStyle, this.flatBorderColor));
				}
			}
		}
		[Browsable(false)]
		public void ResetFlatBorderColor()
		{
			this.flatBorderColor = Color.Empty;
		}
		[Browsable(false)]
		public bool ShouldSerializeFlatBorderColor()
		{
			if (this.flatBorderColor == Color.Empty)
				return false;
			else
				return true;
		}

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this StaticBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this StaticBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of StaticBarItem and calls the <see cref="StaticBarItem.CopyTo"/> method to copy over properties.
		/// </remarks>
		public override object Clone()
		{
			StaticBarItem newItem = new StaticBarItem();
			this.CopyTo(newItem);
			return newItem;
		}

		/// <summary>
		/// Copies the properties of this StaticBarItem into the specified StaticBarItem.
		/// </summary>
		/// <param name="barItem">The StaticBarItem where the values should be copied to.</param>
		/// <remarks>
		/// The Tags will be copied over only if the actual objects are cloneable (implements IClonable).
		/// </remarks>
		public override void CopyTo(BarItem barItem)
		{
			if (!(barItem is StaticBarItem))
				throw new ArgumentException("barItem should be of type StaticBarItem.", "barItem");

			base.CopyTo(barItem);

			StaticBarItem staticItem = barItem as StaticBarItem;

			staticItem.FlatBorderColor = this.FlatBorderColor;
		}


		#endregion

		protected StaticBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "FlatBorderColor":
						this.FlatBorderColor = (Color)entry.Value;
						break;
				}
			}
			//			this.FlatBorderColor = (Color)info.GetValue("FlatBorderColor", typeof(Color));
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("FlatBorderColor", this.FlatBorderColor);
		}
	}
	/// <summary>
	/// A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> derived class that expands into a list when shown.
	/// </summary>
	/// <remarks>
	/// <para>Use this class when you have to represent a dynamic list of <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/>s.
	/// When shown this bar item will be replaced by a numbered list of
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/>s based on the supplied <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItem.ChildCaptions"/> list and when one of the items gets
	/// clicked by the user a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event is thrown by this instance with the
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItemClickedEventArgs"/> args.</para>
	/// <para>This also means you should not use a ListBarItem in a tool bar. If you do so, you
	/// might see unpredictable behavior. The user will automatically be prevented from dropping a ListBarItem into a tool bar during customization.</para>
	/// <para>Note that the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event handler will be called with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItemClickedEventArgs"/>
	/// argument containing data pertaining to the child item that was clicked. Which means you
	/// should cast the regular EventArgs argument into this type in your handler.</para>
	/// <para>A typical example is the Most Recently Used Files list. The framework also
	/// internally uses this to represent the MDI Child list(<see cref="MdiListBarItem"/>).</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[Serializable()]
	public class ListBarItem :
		BarItemEx,
		ICloneable
	{
		private StringCollection childCaptions;
		private ArrayList tags;
		private IntList checkedIndices = null;
		private bool useNumbers = true;
		private ParentBarItem currentParent = null;
		private int beginningChildItemIndex = -1;
		private int lastChildItemIndex = -1;

		/// <summary>
		/// Creates an instance of the ListBarItem class.
		/// </summary>
		public ListBarItem()
		{
			this.Init();
		}

		private void Init()
		{
			this.childCaptions = new StringCollection();
			this.tags = new ArrayList();
			this.checkedIndices = new IntList();
		}
		/// <summary>
		/// Called before the item gets expanded by its parent.
		/// </summary>
		/// <remarks>
		/// This is a good place to fill the ChildCaptions list with appropriate values.
		/// </remarks>
		protected internal virtual void OnBeforeExpand()
		{
			if (this.BeforeExpand != null)
				this.BeforeExpand(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs before the expansion of the Captions into BarItems. This is a
		/// good place to delay-insert Captions into the ChildCaptions list.
		/// </summary>
		[Description("Called before the item gets expanded by its parent."),
		Category("Action")]
		public event EventHandler BeforeExpand;

		/// <summary>
		/// Raises the <see cref="AfterExpand"/> event.
		/// </summary>
		/// <value>The event args for the event.</value>
		protected internal virtual void OnAfterExpand(EventArgs e)
		{
			if (this.AfterExpand != null)
				this.AfterExpand(this, e);
		}

		/// <summary>
		/// Occurs after the expansion of the Captions into BarItems.
		/// </summary>
		/// <remarks>
		/// The handler for this event is a good place to access the expanded BarItems and set some properties on it.
		/// Note that these expanded items will be disposed when the parent menu is hidden.
		/// </remarks>
		[Description("Occurs after the expansion of the Captions into BarItems."),
		Category("Action")]
		public event EventHandler AfterExpand;

		/// <summary>
		/// Called after this item's parent expands this item.
		/// </summary>
		/// <param name="parent">The parent where this item is hosted.</param>
		/// <param name="firstChildIndex">The index into the parent representing the BarItem corresponding to the first item in the ChildCaptions list.</param>
		/// <param name="lastChildIndex">The index into the parent representing the BarItem corresponding to the last item in the ChildCaptions list.</param>
		/// <remarks>
		/// With the reference to the parent, you can insert items if necessary before the parent gets shown.
		/// </remarks>
		protected internal virtual void PostExpand(ParentBarItem parent, int firstChildIndex, int lastChildIndex)
		{
			this.currentParent = parent;
			this.beginningChildItemIndex = firstChildIndex;
			this.lastChildItemIndex = lastChildIndex;
			this.OnAfterExpand(EventArgs.Empty);
		}

		protected internal virtual void PopupClosed()
		{
			this.beginningChildItemIndex = -1;
			this.lastChildItemIndex = -1;
			this.currentParent = null;
		}
        public void AddCheckIndices(int str)
        {
            this.CheckedIndices.Add(str);
            string str1 = this.ChildCaptions[str];
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).AddCheck(str,str1);

        }
        public void AddChildCaptions(string str)
        {
            this.ChildCaptions.Add(str);
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).ExpandListBarItems(str);
        }
        public void InsertChildCaptions(int index,string str)
        {
            this.ChildCaptions.Insert(index, str);
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).InsertListBarItems(index,str);
        }
        internal void RemoveChildCaptions(int index,string caption)
        {          
            this.childCaptions.RemoveAt(index);
            
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).RemoveListBarItems(index, caption);
        }
        public void RemoveChildCaptions(int str)
        {
            string str1 = this.ChildCaptions[str];
            this.childCaptions.RemoveAt(str);
            
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).RemoveExpandListBarItems("&" + (str + 1) + " " + str1); ;
        }

        public void RemoveChildCaptions(string str)
        {
            int s = this.childCaptions.IndexOf(str);
            this.ChildCaptions.Remove(str);
            if (this.CurrentParent is ParentBarItem)
                (this.CurrentParent as ParentBarItem).RemoveExpandListBarItems("&" + (s + 1) + " " + str); ;
        }
		/// <summary>
		/// Returns the items in the ChildCaptions list that should be marked as checked.
		/// </summary>
		/// <value>A list of indices into the ChildCaptions list.</value>
		/// <remarks>
		/// This list will be used to determine the checked state of the expanded
		/// BarItems.
		/// </remarks>
		[
		Description("The items in the ChildCaptions list that should be marked as checked."),
		Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public virtual IntList CheckedIndices
		{
			get { return this.checkedIndices; }
		}

		/// <summary>
		/// Indicates whether to use numbers in the expanded list.
		/// </summary>
		/// <value>True to include numbers; false otherwise.</value>
		[
		Description("Specifies whether or not to use a numbers in the expanded list."),
		Category("Behavior"),
		DefaultValue(true)
		]
		public bool UseNumberedList
		{
			get { return this.useNumbers; }
			set { this.useNumbers = value; }
		}

		internal ParentBarItem CurrentParent
		{
			get { return this.currentParent; }
		}

		/// <summary>
		/// Returns a list of expanded BarItems.
		/// </summary>
		/// <value>An ArrayList if the expanded BarItems are currently shown. Null (or Nothing) otherwise.</value>
		/// <remarks>This property will return a non-null value only when queried from
		/// the <see cref="AfterExpand"/> event handler. It's recommended that you do not
		/// hold any references to the BarItems returned by this property.</remarks>
		public ArrayList ExpandedBarItems
		{
			get
			{
				if (this.currentParent == null)
					return null;

				ArrayList al = new ArrayList();

				// Add the expanded items items to the list.
                if (this.beginningChildItemIndex != -1)
                {
                    for (int i = this.beginningChildItemIndex; i <= this.lastChildItemIndex; i++)
                    {
                        al.Add(this.currentParent.Items[i]);
                    }
                }
				return al;
			}
		}
		/// <summary>
		/// Updates the checked state of the expanded list BarItems. This method
		/// is usable only when the ListBarItem is being shown in a submenu.
		/// </summary>
		public virtual void UpdateCheckedStates()
		{
			if (this.currentParent == null)
				return;

            ParentBarItem parentbaritem = this.CurrentParent;
            foreach (BarItem baritem in parentbaritem.Items)
            {
                if (baritem is ExpandedListBarItem)
                {
                    this.beginningChildItemIndex = parentbaritem.Items.IndexOf(baritem);
                    break;
                }
            }
            
			// Parse through the corresponding BarItems and updated their checked state.            
			for (int i = this.beginningChildItemIndex; i <= this.lastChildItemIndex; i++)
			{
				int localIndex = i - beginningChildItemIndex;
				if (this.CheckedIndices.Contains(localIndex))
                    this.currentParent.Items[i].Checked = true;
                else
                   if (i < this.currentParent.Items.Count)
                       this.currentParent.Items[i].Checked = false;
			}
		}

		/// <summary>
		/// Returns the captions for the expanded bar items.
		/// </summary>
		/// <value>Represents the StringCollection that will hold the list of captions.</value>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Data"),
		Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
			"System.Drawing.Design.UITypeEditor, System.Drawing"),
		Description("The captions for the expanded bar items.")
		]
		public virtual StringCollection ChildCaptions
		{
			get { return this.childCaptions; }
		}

		/// <summary>
		/// Returns the list of application specific values corresponding to the ChildCaptions entries.
		/// </summary>
		/// <value>The list that contains the application specific values.</value>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Data"),
		Description("List of Application specific values corresponding to the ChildCaptions entries.")
		]
		public virtual ArrayList Tags
		{
			get { return this.tags; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual internal void ItemClicked(int index, BarItem item)
		{
			this.OnItemClicked(new ListBarItemClickedEventArgs(index));
		}

		protected ListBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "CheckedIndices":
						ArrayList il = (ArrayList)entry.Value;
						foreach (int i in il)
							this.CheckedIndices.Add(i);
						break;
					case "UseNumberedList":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetBool("UseNumberedList");
							this.UseNumberedList = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.UseNumberedList = (bool)entry.Value;
						break;
					case "ChildCaptions":
						ArrayList sc = (ArrayList)entry.Value;
						foreach (string s in sc)
							this.ChildCaptions.Add(s);
						break;
				}
			}
			//			ArrayList il = (ArrayList)info.GetValue("CheckedIndices", typeof(ArrayList));
			//			foreach(int i in il)
			//				this.CheckedIndices.Add(i);
			//
			//			this.UseNumberedList = info.GetBoolean("UseNumberedList");
			//
			//			StringCollection sc = (StringCollection)info.GetValue("ChildCaptions", typeof(StringCollection));
			//			foreach(string s in sc)
			//				this.ChildCaptions.Add(s);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			ArrayList temp = new ArrayList(this.CheckedIndices);
			info.AddValue("CheckedIndices", temp);

			info.AddValue("UseNumberedList", UseNumberedList);

			temp = new ArrayList(this.ChildCaptions);
			info.AddValue("ChildCaptions", temp);
		}
		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this ListBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this ListBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of ListBarItem and calls the <see cref="ListBarItem.CopyTo"/> method to copy over properties.
		/// </remarks>
		public override object Clone()
		{
			ListBarItem newItem = new ListBarItem();
			this.CopyTo(newItem);
			return newItem;
		}

		/// <summary>
		/// Copies the properties of this ListBarItem into the specified ListBarItem.
		/// </summary>
		/// <param name="barItem">The ListBarItem where the values should be copied to.</param>
		/// <remarks>
		/// The tags will be copied over only if the actual objects are cloneable (implements IClonable).
		/// </remarks>
		public override void CopyTo(BarItem barItem)
		{
			if (!(barItem is ListBarItem))
				throw new ArgumentException("barItem should be of type ListBarItem.", "barItem");

			base.CopyTo(barItem);

			ListBarItem listitem = barItem as ListBarItem;

			listitem.CheckedIndices.Clear();
			foreach (int index in this.CheckedIndices)
				listitem.CheckedIndices.Add(index);

			listitem.UseNumberedList = this.UseNumberedList;

			listitem.ChildCaptions.Clear();
			foreach (string caption in this.ChildCaptions)
				listitem.ChildCaptions.Add(caption);

			listitem.Tags.Clear();
			foreach (object tag in this.Tags)
			{
				if (tag is ICloneable)
					listitem.Tags.Add(((ICloneable)tag).Clone());
				else
					listitem.Tags.Add(null);
			}
		}


		#endregion
	}
	internal class ExpandedListBarItem : StandAloneBarItem
	{
		private ListBarItem parent;
		private int myIndex;
		public ExpandedListBarItem(ListBarItem parent, int myIndex)
		{
			this.parent = parent;
			this.myIndex = myIndex;
		}

		public override BarManager Manager
		{
			get { return this.parent.Manager; }
			set { throw new NotSupportedException("Cannot set BarManager on this ExpandedListBarItem."); }
		}

		protected override void OnItemClicked(EventArgs e)
		{
			this.parent.ItemClicked(this.myIndex, this);
		}

		public override string ID
		{
			get { return BarManager.SyncfusionTransientItemID; }
			set { }
		}
	}

	/// <summary>
	/// The class that represents the event arguments provided by the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.CanDragDrop"/>
	/// event.
	/// </summary>
	public class CanDragDropEventArgs : CancelEventArgs
	{
		private object destinationParent;
		/// <summary>
		/// Creates a new instance of the CanDragDropEventArgs class.
		/// </summary>
		/// <param name="destinationParent">The destination ParentBarItem or Bar.</param>
		/// <param name="cancel">The initial cancel state.</param>
		public CanDragDropEventArgs(object destinationParent, bool cancel)
			: base(cancel)
		{
			this.destinationParent = destinationParent;
		}

		/// <summary>
		/// Returns the parent into which a drag-drop is attempted.
		/// </summary>
		/// <value>This object could be a ParentBarItem or a Bar.</value>
		public object DestinationParent
		{
			get { return this.destinationParent; }
		}
	}

	/// <summary>
	/// The class that represents the event arguments
	/// for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.ContainmentChanged"/>
	/// event.
	/// </summary>
	public class ContainmentChangedEventArgs : EventArgs
	{
		private object parent;
		private bool inserted;
		/// <summary>
		/// Creates a new instance of ContainmentChangedEventArgs.
		/// </summary>
		/// <param name="parent">The destination ParentBarItem or Bar.</param>
		/// <param name="inserted">Indicates whether the BarItem was inserted or removed.</param>
		public ContainmentChangedEventArgs(object parent, bool inserted)
		{
			this.parent = parent;
			this.inserted = inserted;
		}

		/// <summary>
		/// Returns the new logical parent from which the BarItem was removed or into which the
		/// BarItem was inserted.
		/// </summary>
		/// <value>This object could be a ParentBarItem or a Bar.</value>
		public object Parent
		{
			get { return this.parent; }
		}

		/// <summary>
		///  Indicates whether the BarItem has been inserted or removed.
		/// </summary>
		public bool Inserted
		{
			get { return this.inserted; }
		}
	}

	/// <summary>
	/// The class that represents the event arguments in the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItem"/>'s
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event.
	/// event.
	/// </summary>
	/// <remarks>
	/// <para>Note that the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event handler will be called with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItemClickedEventArgs"/>
	/// argument containing data pertaining to the child item that was clicked. Which means you
	/// should cast the regular EventArgs argument into a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ListBarItemClickedEventArgs"/> in your handler.</para>
	/// </remarks>
	public class ListBarItemClickedEventArgs : EventArgs
	{
		private int indexClicked;
		/// <summary>
		/// Creates a new instance of the ListBarItemClickedEventArgs class.
		/// </summary>
		/// <param name="indexClicked">An index into the corresponding ListBarItem's ChildCaptions list.</param>
		public ListBarItemClickedEventArgs(int indexClicked)
		{
			this.indexClicked = indexClicked;
		}

		/// <summary>
		/// Returns an index into the ChildCaptions list, identifying the item that was clicked.
		/// </summary>
		public int IndexClicked
		{
			get { return this.indexClicked; }
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IRequiresControl
	{
		object Value { get; set;}
		int MinWidth { get;}
	}

	/// <summary>
	/// Represents the item that will expand to show the current list of toolbars and their visible states.
	/// </summary>
	/// <remarks>
	/// <para>You would typically use this item as a child in the "View" submenu in the main menu bar
	/// of a <see cref="MainFrameBarManager"/>.</para>
	/// <para>Users can show/hide toolbars through this submenu and also invoke the customization dialog.</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	ToolboxItem(false),
	DesignTimeVisible(false)
	]
	public class ToolbarListBarItem : ParentBarItem
	{
		public ToolbarListBarItem()
			: base()
		{
		}

		public override void OnBeforePopup(CancelEventArgs args)
		{
			MainFrameBarManager mainManager = this.barManager as MainFrameBarManager;
			if (mainManager == null)
			{
				Trace.Assert(this.barManager is MainFrameBarManager, "A ToolbarListBarItem can and should be part of a MainFrameBarManager.");

				args.Cancel = true;
			}

			base.OnBeforePopup(args);
		}

		public override void OnPopup(EventArgs args)
		{
			if (this.barManager != null)
				this.barManager.commandBarManager.PrepareToolbarListItem(this);

			if (this.Items != null)
			{
				foreach (BarItem bitem in this.Items)
					bitem.CustomTextFont = this.CustomTextFont;
			}

			base.OnPopup(args);
		}

		public override void OnPopupClosed(EventArgs args)
		{
			// This is necessary since we cannot keep items that are not part of the
			// corresponding BarManager in this popup.
			this.Items.Clear();
			base.OnPopupClosed(args);
		}
	}

	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	Serializable()
	]
	public class TextBoxBarItem :
		BarItemEx,
		IRequiresControl,
		ICloneable
	{
		private string textBoxValue;
		private int minwidth = 40;

		#region CONSTRUCTORS
		public TextBoxBarItem()
			: this(MenuMerge.Add, 0, Shortcut.None, "", null, null, null)
		{
		}
		public TextBoxBarItem(string text)
			: this(MenuMerge.Add, 0, Shortcut.None, text, null, null, null)
		{
		}
		public TextBoxBarItem(string text, EventHandler onClick)
			: this(MenuMerge.Add, 0, Shortcut.None, text, onClick, null, null)
		{
		}
		public TextBoxBarItem(string text, EventHandler onClick, Shortcut shortcut)
			: this(MenuMerge.Add, 0, shortcut, text, onClick, null, null)
		{
		}
		public TextBoxBarItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect)
			: base(mergeType, mergeOrder, shortcut, text, onClick, onPopup, onSelect)
		{
			this.Init();
		}
		private void Init()
		{
			//			this.barItemStyle = BarItemStyle.Default;
		}
		#endregion CONSTRUCTORS

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),]
		public string ControlType
		{
			get { return "TextBox"; }
		}
		[Browsable(false)]
		public object Value
		{
			get { return this.TextBoxValue; }
			set
			{
				if (value != null)
				{
					this.TextBoxValue = value.ToString();
				}
				else
				{
					this.TextBoxValue = String.Empty;
				}
			}
		}

		/// <summary>
		///Lets handle the textBox inside the textBoxItem.
		/// </summary>
		[Description("Lets handle the textBox inside the textBoxItem.")]
		public event TextBoxItemBoundEventHandler TextBoxItemBound;
		/// <summary>
		/// Raises the TextBoxItemBound event.
		/// </summary>
		public virtual void OnTextBoxItemBound(TextBoxItemBoundEventArgs e)
		{
			if (this.TextBoxItemBound != null)
				this.TextBoxItemBound(this, e);
		}

		/// <summary>
		/// Gets or sets the value in the text box.
		/// </summary>
		[DefaultValue(""),
		Localizable(true),]
		public virtual string TextBoxValue
		{
			get { return this.textBoxValue; }
			set
			{
				if (this.textBoxValue != value)
				{
					this.textBoxValue = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "TextBoxValue", null, null));
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the item.
		/// </summary>
		[DefaultValue(40),
		Localizable(true),]
		public virtual int MinWidth
		{
			get
			{
				return this.minwidth;
			}
			set
			{
				if (this.minwidth != value)
				{
					this.minwidth = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Width", null, null));
				}
			}
		}

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this TextBoxBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this TextBoxBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of TextBoxBarItem and calls the <see cref="CopyTo"/> method to copy over properties.
		/// </remarks>
		public override object Clone()
		{
			TextBoxBarItem newItem = new TextBoxBarItem();
			this.CopyTo(newItem);
			return newItem;
		}

		/// <summary>
		/// Copies the properties of this TextBoxBarItem into the specified TextBoxBarItem.
		/// </summary>
		/// <param name="barItem">The TextBoxBarItem where the values should be copied to.</param>
		/// <remarks>
		/// The Items list will be shallow copied over.
		/// </remarks>
		public override void CopyTo(BarItem barItem)
		{
			if (!(barItem is TextBoxBarItem))
				throw new ArgumentException("barItem should be of type TextBoxBarItem.", "barItem");

			base.CopyTo(barItem);

			TextBoxBarItem tbitem = barItem as TextBoxBarItem;

			tbitem.TextBoxValue = this.TextBoxValue;
			tbitem.MinWidth = this.MinWidth;
		}
		#endregion
		protected TextBoxBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "TextBoxValue":
						this.TextBoxValue = (string)entry.Value;
						break;
					case "MinWidth":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetInt32("MinWidth");
							this.MinWidth = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.MinWidth = (int)entry.Value;
						break;
				}
			}
			//			this.TextBoxValue = info.GetString("TextBoxValue");
			//			this.MinWidth = info.GetInt32("MinWidth");
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("TextBoxValue", this.TextBoxValue);
			info.AddValue("MinWidth", this.MinWidth);
		}
	}

	/// <summary>
	/// Represents an individual item that can be displayed in a menu structure, a tool bar
	/// or a popup menu in the XP Menus framework.
	/// </summary>
	/// <remarks>
	/// <para>In order for the BarItem to be displayed, you must add it to the Items property of
	/// a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> (to appear in menus or context menus) or a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> (to appear in the
	/// tool bar). This is normally achieved using simple drag-and-drop during
	/// design-time. The BarItem will of course be rendered differently based on whether it
	/// is part of a menu structure or the tool bar.</para>
	/// <para>The BarItem class provides properties that enable you to configure the
	/// appearance and functionality of a bar item. To display a check mark next
	/// to this bar item (when in a menu) or to give it a special checked highlight
	/// (when in a tool bar), use the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Checked"/> property. You can use this feature to identify
	/// a bar item that is selected in a list of mutually exclusive bar items. For
	/// example, if you have a set of bar items for setting the color of text in a
	/// TextBox control, you can use the Checked property to identify which color is
	/// currently selected. </para><para>The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Shortcut"/> property can be used to define a keyboard
	/// combination that can be pressed to select the bar item.</para>
	/// <para>You can enable partial menus behavior in bar items when they are contained within a
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> by setting the BarItem's <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.IsRecentlyUsedItem"/> property.</para>
	/// <para>In an MDI scenario, bar items added to an MDI parent's MainMenu(BarStyle.IsMainMenu enabled in the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar.BarStyle"/> property) and the MDI child's
	/// MainMenu will automatically be merged to create a consolidated menu structure.</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	Designer(
		typeof(Syncfusion.Windows.Forms.Tools.Design.BarItemDesigner),
		typeof(System.ComponentModel.Design.IDesigner)),
	Serializable()
	]
	public class BarItem
		: Component
		, IChangeNotifyingItem
		, ICustomTypeDescriptor
		, ICloneable
		, IDataBindingSupport
		, ISerializable
	{
		#region PRIVATE_MEMBERS
		/// <summary>
		/// Indicates whether to show underlines with mnemonics always.
		/// </summary>
		private bool m_bShowUnderlinesAlways = false;

		private PaintStyle paintStyle;
		private string text = String.Empty;
		private string id = String.Empty;
		private int categoryIndex = -1;
		internal BarManager barManager;
		private int state;
		private Shortcut shortcut;
		private string shortcutText = null;
		private MenuMerge mergeType;
		private int mergeOrder;
		private object tag;

		private ImageList imageList = null;
		private ImageListAdv imageListAdv = null;

		private ImageList largeImageList = null;
		private ImageListAdv largeImageListAdv = null;

		private ImageList m_disabledImgList = null;
		private ImageListAdv m_disabledImgListAdv = null;

		private ImageList m_disabledLargeImgList = null;
		private ImageListAdv m_disabledLargeImgListAdv = null;

		private ImageList m_highlightImgList = null;
		private ImageListAdv m_highlightImgListAdv = null;

		private ImageList m_highlightLargeImgList = null;
		private ImageListAdv m_highlightLargeImgListAdv = null;

		private ImageList m_pressedImageList = null;
		private ImageListAdv m_pressedImageListAdv = null;

		private ImageList m_pressedLargeImageList = null;
		private ImageListAdv m_pressedLargeImageListAdv = null;

		private int m_imageIndex = -1;
		private int m_disabledImgIndex = -1;
		private int m_highlightedImageIndex = -1;
		private int m_pressedImageIndex = -1;

		private string tooltip = String.Empty;
		private bool customizable = true;
		private bool _uiUpdateOnAppIdle = false;
		private bool m_bDrawImageMirrored = false;
		private Form topLevelForm = null;
		private bool m_shortCutProcessing = false;
		/// <summary>
		/// Indicates whether doubleClick event is triggered on demand.
		/// </summary>
		private bool m_handleDoubleClick = false;

		/// <summary>
		/// Indicates whether the bar item text shows hot key prefix when rendered.
		/// </summary>
		private bool m_bHintViaHotKeyPrefix = false;

		/// <summary>
		/// Indicates whether the bar item is drawn in large icons mode. Valid only for XPToolBar related items.
		/// </summary>
		private bool m_bLargeIcons = false;

		/// <summary>
		/// Text alignment. Used only in popup menu.
		/// </summary>
		private TextAlignment m_TextAlignment = TextAlignment.Near;
		/// <summary>
		/// Text color in normal mode.
		/// </summary>
		private Color m_cCustomNormalTextColor = Color.Empty;
		/// <summary>
		/// Text color in Disabled mode.
		/// </summary>
		private Color m_cCustomDisabledTextColor = Color.Empty;
		/// <summary>
		/// Text color in active mode.
		/// </summary>
		private Color m_cCustomActiveTextColor = Color.Empty;

		/// <summary>
		/// Gets or Sets, the transparency color for the image.
		/// </summary>
		private Color m_imageTransparentColor = Color.Empty;
		/// <summary>
		/// Text font.
		/// </summary>
		private Font m_fCustomTextFont = null;

		/// <summary>
		/// Indicates whether the tooltip is shown.
		/// </summary>
		private bool m_bShowTooltip = true;

		/// <summary>
		/// Displayed icon or bitmap.
		/// </summary>
		private ImageExt m_image = null;

		/// <summary>
		/// Disabled image.
		/// </summary>
		private ImageExt m_disabledImage = null;

		/// <summary>
		/// Highlight image.
		/// </summary>
		private ImageExt m_highlightImage = null;

		/// <summary>
		/// Default image size.
		/// </summary>
		internal static readonly Size DEF_IMAGE_SIZE = new Size(16, 16);

		/// <summary>
		/// Size for <see cref="Image"/>.
		/// </summary>
		private Size m_imageSize = DEF_IMAGE_SIZE;

		internal const int STATE_MDILIST = 8 /*0x0008*/;
		internal const int STATE_CHECKED = 4 /*0x0004*/;
		internal const int STATE_DISABLED = 16 /*0x0010*/;
		internal const int STATE_VISIBLE = 32 /*0x0020*/;
		internal const int STATE_ISNOT_RECENTLYUSEDITEM = 128 /*0x0080*/;
		internal const int STATE_CLONE_MASK = 188/*Sum of the above*/;

		#region CONSTRUCTORS

		/// <summary>
		/// Overloaded. Creates a new instance of the BarItem class.
		/// </summary>
		public BarItem()
			: this(MenuMerge.Add, 0, Shortcut.None, "", null, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarItem class and initializes its Text property.
		/// </summary>
		public BarItem(string text)
			: this(MenuMerge.Add, 0, Shortcut.None, text, null, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarItem class and initializes its Text property and Click event.
		/// </summary>
		public BarItem(string text, EventHandler onClick)
			: this(MenuMerge.Add, 0, Shortcut.None, text, onClick, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarItem class and initializes its Text property, Click event and Shortcut.
		/// </summary>
		public BarItem(string text, EventHandler onClick, Shortcut shortcut)
			: this(MenuMerge.Add, 0, shortcut, text, onClick, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarItem class and initializes its
		/// merge type, merge order, shortcut, Text property, Click, Popup and Select event.
		/// </summary>
		public BarItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect)
		{
			this.Init(mergeType, shortcut, text, onClick, onSelect);
		}
		private void Init(MenuMerge mergeType, Shortcut shortcut, string text, EventHandler onClick, EventHandler onSelect)
		{
			this.state = 0;
			this.m_disabledImgIndex = -1;
			this.m_imageIndex = -1;
			this.mergeType = mergeType;
			this.mergeOrder = 0;
			this.shortcut = shortcut;
			this.text = text;
			this.Click += onClick;
			this.Selected += onSelect;
			this.Visible = true;
			this.paintStyle = PaintStyle.Default;
		}

		private bool m_bIsDisposing = false;

		protected internal bool IsDisposing
		{
			get
			{
				return m_bIsDisposing;
			}
		}

		private bool m_bIsDisposed = false;

		/// <summary>
		/// Indicates whether BarItem is disposed.
		/// </summary>
		[Description("Indicates whether BarItem is disposed.")]
		public bool IsDisposed
		{
			get
			{
				return m_bIsDisposed;
			}
		}

		/// </override>
		protected override void Dispose(bool disposing)
		{
			m_bIsDisposing = true;

			if (disposing)
			{
				if (this.Manager != null && this.Manager.SelectedItem == this)
					this.Manager.SelectedItem = null;

				if (this._uiUpdateOnAppIdle)
				{
					Application.Idle -= new EventHandler(this.OnIdle);
				}
				this.barManager = null;

                this.PropertyChanged = null;

				m_bIsDisposed = true;
			}
			base.Dispose(disposing);

			m_bIsDisposing = false;
		}
		#endregion CONSTRUCTORS

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ComponentDesigner Designer
		{
			get { return (this.GetService(typeof(IDesignerHost)) as IDesignerHost).GetDesigner(this) as ComponentDesigner; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GetState(int flag)
		{
			return (this.state & flag) > 0;
		}
		// returns true if value changed; or else false.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool SetState(int flag, bool value)
		{
			if (this.GetState(flag) != value)
			{
				if (value)
					this.state |= flag;
				else
					this.state &= ~flag;

				return true;
			}
			return false;
		}

		/// <summary>
		/// Holds cursor position while last mouse down event.
		/// </summary>
		private Point m_lastClickLocation = Point.Empty;

		/// <summary>
		/// Holds time when last mouse down event took place.
		/// </summary>
		private long m_tiksLastClick = 0;

		/// <summary>
		/// Indicates whether BarItem was double clicked.
		/// </summary>
		private bool m_bDoubleClick = false;

		/// <summary>
		/// Factor for double click time.
		/// </summary>
		private const int DEF_DELAY_FACTOR = 10000;

		#endregion PRIVATE_MEMBERS

		#region EVENTS
		/// <summary>
		/// Occurs when the menu item is clicked by the user.
		/// </summary>
		[Description("Occurs when the menu item is clicked by the user."),
		Category("Action")]
		public event EventHandler Click;
		/// <summary>
		/// Occurs when the user selects a BarItem during menu navigation using mouse or keyboard.
		/// </summary>
		[Description("Occurs when the user selects a BarItem during menu navigation using mouse or keyboard."),
		Category("Action")]
		public event EventHandler Selected;
		/// <summary>
		/// Occurs when the item has be unselected during user navigation using mouse or keyboard.
		/// </summary>
		[Description("Occurs when the item has be unselected during user navigation via mouse or keyboard."),
		Category("Action")]
		public event EventHandler Unselected;

        [Description("Occurs when the mousedown on item."),
        Category("Action")]
        public event MouseEventHandler MouseDown;


        [Description("Occurs when the mouseup on item."),
        Category("Action")]
        public event MouseEventHandler MouseUp;
		/// <summary>
		/// Occurs when a property's value changes in this object.
		/// </summary>
		/// <remarks>
		/// This event may not be thrown for some of the properties
		/// in BarItem. Take a look at the property's documentation
		/// to confirm whether this event will be thrown for a property.
		/// </remarks>
		[Description("Occurs when a Property's value changes in this object."),
		Category("Property Changed")]
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;

		// Not supported in this version.
		[Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false)]
		public event DrawToolbarItemEventHandler DrawToolbarItem;
		/// <summary>
		/// Occurs when a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> gets added to or removed from a
		/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/>
		/// or <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/>.
		/// </summary>
		[Description("Occurs when a BarItem gets added to or removed from a ParentBarItem or Bar."),
		Category("Layout")]
		public event ContainmentChangedEventHandler ContainmentChanged;

		/// <summary>
		/// Occurs when the BarItem gets dragged over a ParentBarItem(submenu) or a Bar(Tool Bar)
		/// during user-customization.
		/// </summary>
		[Description("Occurs when a BarItem gets dragged over a ParentBarItem or Bar during user-customization."),
		Category("Drag Drop")]
		public event CanDropEventHandler CanDragDrop;

		/// <summary>
		/// Occurs if either the <see cref="UpdateUIOnAppIdle"/> or <see cref="BarManager.UpdateUIMFCStyle"/>
		/// is on.
		/// </summary>
		/// <remarks>
		/// <para>
		/// You can update the state of the BarItems in the following ways:
		/// </para>
		/// <list type="number">
		/// <item><description><b>Neurotic Approach:</b> In this approach you change the
		/// state of the BarItem as and when the corresponding application state changes.
		/// This is what the XPMenus framework expects you to do by default and so
		/// it will not fire the UpdateUI event under any circumstances.</description></item>
		/// <item><description><b>Relaxed Approach:</b> The above neurotic approach is sometimes
		/// cumbersome as it's difficult to keep track of state changes in application and
		/// updating the UI state appropriately. So, the framework provides another alternative
		/// where you can update the BarItem states in a relaxed manner. There are 2 ways
		/// of updating the BarItems in the relaxed approach:
		/// <para>
		/// <b>1. Fast Updates:</b> If updating the BarItem states is a trivial operation
		/// then use this approach, which is also how MFC does it. In this approach
		/// the UpdateUI event will be called when the <see cref="ParentBarItem"/> hosting
		/// this BarItem is dropped down, or when the BarItem is hosted in a toolbar and when
		/// the application goes into an Idle state, or when a shortcut corresponding to this
		/// item is about to be processed. You can turn on this behavior through out the
		/// menu structure by setting the <see cref="BarManager.UpdateUIMFCStyle"/> to true.
		/// For <see cref="XPToolBar"/>s and <see cref="ParentBarItem"/>s that are outside
		/// the scope of a BarManager set the <b>UpdateUIMFCStyle</b> property in those instances
		/// explicitly.
		/// </para>
		/// <para>
		/// <b>2. Slow Updates:</b> If updating the BarItem states is not a trivial operation
		/// then use this approach. In this approach you simply turn on the <see cref="UpdateUIOnAppIdle"/>
		/// property of the BarItem whose state has changed one or more times and the framework
		/// will then fire it's <see cref="UpdateUI"/> event the next time the application goes
		/// into an idle state.
		/// </para>
		/// </description></item>
		/// </list>
		/// </remarks>
		[Description("Occurs when the BarItem.UpdateUIOnAppIdle or BarManager.UpdateUIMFCStyle flag is set to true."),
		Category("Appearance")]
		public event EventHandler UpdateUI;

		/// <summary>
		/// Lets you provide a custom font for this bar item.
		/// </summary>
		[Category("Appearance"),
		Description("Lets you provide a custom font for this bar item.")
		]
		public event ProvideFontInfoEventHandler ProvideFontInfo;
		/// <summary>
		/// Occurs after the PopupItem is painted.
		/// </summary>
		[Description("Occurs after the PopupItem is painted.")]
		public event PopupItemPaintEventHandler AfterPopupItemPaint;
		/// <summary>
		/// Occurs before the PopupItem is painted.
		/// </summary>
		[Description("Occurs before the PopupItem is painted.")]
		public event PopupItemPaintEventHandler BeforePopupItemPaint;

		/// <summary>
		/// Occurs when <see cref="ShowTooltip"/> is changed.
		/// </summary>
		[
		Description("Occurs when ShowTooltip is changes."),
		Category("Property Changed")
		]
		public event EventHandler ShowTooltipChanged;

		/// <summary>
		/// Occurs when the menu item is double clicked by the user.
		/// </summary>
		[
		Description("Occurs when the menu item is double clicked by the user."),
		Category("Action")
		]
		public event EventHandler DoubleClick;

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Padding for themes control X.
		/// </summary>
		private int m_iPaddingForThemesX = 4;
		/// <summary>
		/// Padding for themes control Y.
		/// </summary>
		private int m_iPaddingForThemesY = 6;
		/// <summary>
		/// Enable or Disable the tooltip of the BarItem when it is in popup menu.
		/// </summary>
		private bool showToolTipInPopUp = false;

		/// <summary>
		/// Gets or sets padding for themes control X.
		/// </summary>
		[DefaultValue(4)]
		[Description("Gets or sets padding for themes control X.")]
		[Category("Appearance")]
		public int PaddingForThemesX
		{
			get
			{
				return m_iPaddingForThemesX;
			}
			set
			{
				if (m_iPaddingForThemesX != value)
				{
					int oldValue = m_iPaddingForThemesX;
					m_iPaddingForThemesX = value;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout,
						"PaddingForThemesX", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Enable or disable the tooltip of the BarItem in popup
		/// </summary>
		[DefaultValue(4)]
		[Description("Gets or sets the value to enable the tooltip of the BarItem in popup.")]
		[Category("Appearance")]
		public bool ShowToolTipInPopUp
		{
			get
			{
				return showToolTipInPopUp;
			}
			set
			{
				showToolTipInPopUp = value;
			}
		}
		/// <summary>
		/// Gets or sets padding for themes control Y.
		/// </summary>
		[DefaultValue(6)]
		[Description("Gets or sets padding for themes control Y.")]
		[Category("Appearance")]
		public int PaddingForThemesY
		{
			get
			{
				return m_iPaddingForThemesY;
			}
			set
			{
				if (m_iPaddingForThemesY != value)
				{
					int oldValue = m_iPaddingForThemesY;
					m_iPaddingForThemesY = value;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout,
						"PaddingForThemesY", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Padding for control with disabled themes.
		/// </summary>
		private Point m_padding = Point.Empty;

		/// <summary>
		/// Gets or sets padding for control with disabled themes.
		/// </summary>
		[Description("Gets or sets padding for items with disabled themes")]
		[Category("Appearance")]
		public Point Padding
		{
			get
			{
				return m_padding;
			}
			set
			{
				if (m_padding != value)
				{
					if (value.X < 0 || value.Y < 0)
						throw new ArgumentException("Value cannot be negative.");

					Point oldValue = m_padding;
					m_padding = value;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout,
						"Padding", oldValue, value));
				}
			}
		}

		public bool ShouldSerializePadding()
		{
			if (m_padding.Equals(Point.Empty))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Gets or sets the text alignment. Used only in popup menu.
		/// </summary>
		[
		Description("Gets or sets text alignment. Use only in popup menu."),
		Category("Appearance"),
		DefaultValue(TextAlignment.Near)
		]
		public virtual TextAlignment TextAlignment
		{
			get
			{
				return m_TextAlignment;
			}
			set
			{
				if (m_TextAlignment != value)
				{
					TextAlignment oldValue = m_TextAlignment;
					m_TextAlignment = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "TextAlignment", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the text color in normal mode.
		/// If value is empty default color is used.
		/// </summary>
		[
		Description("Gets or sets text color in normal mode."),
		Category("Appearance")
		]
		public virtual Color CustomNormalTextColor
		{
			get
			{
				return m_cCustomNormalTextColor;
			}
			set
			{
				if (m_cCustomNormalTextColor != value)
				{
					Color oldValue = m_cCustomNormalTextColor;
					m_cCustomNormalTextColor = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "CustomNormalTextColor", oldValue, value));
				}
			}
		}

		protected bool ShouldSerializeCustomNormalTextColor()
		{
			return (CustomNormalTextColor != Color.Empty);
		}

		protected void ResetCustomNormalTextColor()
		{
			this.CustomNormalTextColor = Color.Empty;
		}

		/// <summary>
		/// Gets or sets the text color in disabled mode.
		/// If value is empty use default color.
		/// </summary>
		[
		Description("Gets or sets text color in disabled mode. If value is empty use default color."),
		Category("Appearance")
		]
		public virtual Color CustomDisabledTextColor
		{
			get
			{
				return m_cCustomDisabledTextColor;
			}
			set
			{
				if (m_cCustomDisabledTextColor != value)
				{
					Color oldValue = m_cCustomDisabledTextColor;
					m_cCustomDisabledTextColor = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "CustomDisabledTextColor", oldValue, value));
				}
			}
		}

		protected bool ShouldSerializeCustomDisabledTextColor()
		{
			return (CustomDisabledTextColor != Color.Empty);
		}

		protected void ResetCustomDisabledTextColor()
		{
			this.CustomDisabledTextColor = Color.Empty;
		}

		/// <summary>
		/// Gets or sets the text color in active mode.
		/// If value is empty default color is used.
		/// </summary>
		[
		Description("Gets or sets text color in active mode."),
		Category("Appearance")
		]
		public virtual Color CustomActiveTextColor
		{
			get
			{
				return m_cCustomActiveTextColor;
			}
			set
			{
				if (m_cCustomActiveTextColor != value)
				{
					Color oldValue = m_cCustomActiveTextColor;
					m_cCustomActiveTextColor = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "CustomActiveTextColor", oldValue, value));
				}
			}
		}

		protected bool ShouldSerializeCustomActiveTextColor()
		{
			return (this.CustomActiveTextColor != Color.Empty);
		}

		protected void ResetCustomActiveTextColor()
		{
			this.CustomActiveTextColor = Color.Empty;
		}

		#region CustomTextFont
		/// <summary>
		/// Gets or sets text font.
		/// If value is null default font is used.
		/// </summary>
		[ Category("Appearance"), Description("Gets or sets text font.") ]
		public virtual Font CustomTextFont
		{
			get
			{
				if (m_fCustomTextFont == null)
				{
					BarManager manager = this.Manager;
					
					if (manager != null)
					{
						if (manager is MainFrameBarManager)
						{
							return ((MainFrameBarManager)manager).Font;
						}
						
						if (manager.Form != null)
						{
							return manager.Form.Font;
						}
					}
				}
				return m_fCustomTextFont;
			}
			set
			{
				if (m_fCustomTextFont != value)
				{
					SyncfusionPropertyChangedEventArgs ea = new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "CustomTextFont", m_fCustomTextFont, value);
					
					m_fCustomTextFont = value;

					this.OnPropertyChanged(ea);
				}
			}
		}
		private bool ShouldSerializeCustomTextFont()
		{
			return m_fCustomTextFont != null;
		}
		private void ResetCustomTextFont()
		{
			m_fCustomTextFont = null;
		}

		/// <summary>
		/// Indicates whether to Reset menu Arrow size always.
		/// </summary>
		private bool resizeGlyphToFit = false;
		[
		DefaultValue(false),
		Description("Indicates whether to resize drop-down arrow to fit the containing cell.")
		]
		public bool ResizeGlyphToFit
		{
			get 
			{
				return resizeGlyphToFit; 
			}
			set 
			{
				if (resizeGlyphToFit != value)
				{
					bool oldValue = resizeGlyphToFit;
					resizeGlyphToFit = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "ResizeGlyphToFit", oldValue, resizeGlyphToFit));
				}
			}
		}
		#endregion

		/// <summary>
		/// Indicates whether to show underlines with mnemonics always.
		/// </summary>
		[DefaultValue(false),
	   Description("Indicates whether to show underlines with mnemonics always.")]
		public virtual bool ShowMnemonicUnderlinesAlways
		{
			get
			{
				return m_bShowUnderlinesAlways;
			}
			set
			{
				if (value != m_bShowUnderlinesAlways)
				{
					m_bShowUnderlinesAlways = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating the behavior of this bar item when its
		/// bar is merged with another.
		/// </summary>
		/// <remarks>
		/// A MenuMerge value that represents the bar item's merge type.
		/// <para>The merge type of a bar item indicates how the bar item behaves
		/// when it has the same merge order as another bar item being merged.
		/// You can use merged menus/bars to create a consolidated menus/bars based on two or
		/// more existing menus/bars.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example> The following example creates a BarItem with the MergeType and
		/// MergeOrder specified so that the bar item is added to the merged menu/bar at
		/// first position.
		/// <code lang="C#">
		/// public void InitMyFileMenu()
		/// {
		///		// Set the MergeType to Add so that the bar item is added to the
		///		// merged menu/bar.
		///		barItem1.MergeType = MenuMerge.Add;
		///		// Set the MergeOrder to 1 so that this bar item is placed lower
		///		// in the merged menu/bar order.
		///		barItem1.MergeOrder = 1;
		///	}
		/// </code>
		/// </example>
		[DefaultValue(MenuMerge.Add),
		Category("Appearance"),
		Description("Indicates the behavior of this bar item when its bar is merged with another.")
		]
		public virtual MenuMerge MergeType
		{
			get { return this.mergeType; }
			set
			{
				if (this.mergeType != value)
				{
					this.mergeType = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "MergeType", null, null));
				}
			}
		}
		/// <summary>
		/// Indicates whether this item will participate in user customization.
		/// </summary>
		/// <value>True to make it customizable; false otherwise. Default is true.</value>
		/// <remarks>
		/// When turned off, the item will still be visible and active in the
		/// menus/toolbars for normal click, mouse move actions. When the user opens
		/// the Customize dialog or when the user presses the Alt+Click on an item, all
		/// these items will become invisible in the menus/toolbars and also in
		/// the Customize dialog. When you presses the Alt+Click on an item whose customizable
		/// property is set to false, no customization will start.
		/// </remarks>
		[DefaultValue(true),
		Description("Specifies whether or not this item will participate in user customization."),
		Category("Behavior")]
		public virtual bool Customizable
		{
			get { return this.customizable; }
			set
			{
				if (this.customizable != value)
				{
					this.customizable = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "Customizable", null, null));
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating the relative position of the bar item
		/// when it is merged with another.
		/// </summary>
		/// <remarks>
		/// <para>A zero-based index representing the merge order position for this bar item.
		/// The default is zero.</para>
		/// <para>The merge order of a bar item specifies the relative position that
		/// this bar item will assume if the parent item that the BarItem is
		/// contained in is merged with another.</para>
		/// <para>Changing this property's value will throw the
		/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.PropertyChanged"/> event.</para>
		/// <para>The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/>s in a main-menu bar (the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> with the BarStyle.IsMainMenu flag set)
		/// will appear in an order based on their MergeOrder value during runtime, irrespective of
		/// their order during design-time. This is because the main-menu is created by merging the
		/// main-menus of the mdi container form and the mdi children, if any.</para>
		/// </remarks>
		[DefaultValue(0),
		Category("Appearance"),
		Description("Gets or sets a value indicating the relative position of the menu item when it is merged with another.")
		]
		public virtual int MergeOrder
		{
			get { return this.mergeOrder; }
			set
			{
				if (this.mergeOrder != value)
				{
					this.mergeOrder = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "MergeOrder", null, null));
				}
			}
		}
		/// <summary>
		/// Gets or sets the painting style in which this bar item
		/// will be drawn when placed in a Menu or Bar.
		/// </summary>
		/// <remarks>
		/// A PaintStyle value that represents the bar item's paint style.
		/// <para>Take a look at the documentation for the PaintStyle enumeration
		/// for more information on the interpretation of each PaintStyle value.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(PaintStyle.Default),
		Category("Appearance"),
		Localizable(true),
		Description("Indicates the painting style when placed in a menu or toolbar.")
		]
		public virtual PaintStyle PaintStyle
		{
			get { return this.paintStyle; }
			set
			{
				if (this.paintStyle != value)
				{
					this.paintStyle = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "PaintStyle", null, null));
				}
			}
		}

		/// <summary>
		/// Indicates whether the menu item is enabled.
		/// </summary>
		/// <remarks>
		/// True if the bar item is enabled; false otherwise. The default is true.
		/// A BarItem that is disabled is displayed in gray color to indicate its
		/// state. When a parent bar item is disabled, all child bar items are not
		/// displayed.
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example>
		/// The following example creates an event handler for the Popup event
		/// for three bar items that handle cut, copy and delete operations
		/// in an application. The event handler code enables or disables the
		/// bar items based on whether a specific TextBox control in the application,
		/// named textBox1, has text selected within it. This example assumes that
		/// three BarItem objects are created named menuItemCut,
		/// menuItemCopy and menuItemDelete have been created.
		/// <code lang="C#">
		/// public void Popup(Object sender, EventArgs e)
		/// {
		///     // Determine if there is text selected in textBox1.
		///     if(textBox1.SelectedText == "")
		///     {
		///         // Disable the menus since no text is selected in textBox1.
		///         menuItemCut.Enabled = false;
		///         menuItemCopy.Enabled = false;
		///         menuItemDelete.Enabled = false;
		///     }
		///     else
		///     {
		///         // Text is selected in textBox1, so enable menu items.
		///         menuItemCut.Enabled = true;
		///         menuItemCopy.Enabled = true;
		///         menuItemDelete.Enabled = true;
		///     }
		/// }
		/// </code>
		/// </example>
		[DefaultValue(true),
		Category("Appearance"),
		Description("Gets or sets a value indicating whether the menu item is enabled."),
		Localizable(true)
		]
		public virtual bool Enabled
		{
			get { return !(this.GetState(STATE_DISABLED)); }
			set
			{
				if (this.SetState(STATE_DISABLED, !value))
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "Enabled", null, null));
			}
		}
		/// <summary>
		/// Indicates whether the BarItem should be drawn with a checked appearance.
		/// </summary>
		/// <remarks>
		/// When in a menu, a check mark will be placed to the left of the item.
		/// When in a Command Bar this will be drawn with a selected background.
		/// <para>
		/// You can use the Checked property in combination with other bar items
		/// in a menu or tool bar to provide state for an application. For example, you can
		/// place a check mark on a bar item in a group of items to identify the
		/// size of the font to be displayed for the text in an application. You
		/// can also use the Checked property to identify the selected bar item
		/// in a group of mutually exclusive bar items.
		/// </para>
		/// <bold>   Note   </bold>This property will be ignored for parent bar items
		/// (ParentBarItem and DropDownBarItem).
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example>
		/// The following example uses the Checked property to provide the state
		/// in an application. In this example, a group of bar items are used to
		/// specify the color for the text in an TextBox control. The event handler
		/// provided is used by the Click event of the three bar items. Each bar
		/// item specifies a text color, menuItemRed (Red), menuItemGreen (Green),
		/// or menuItemBlue (Blue). The event handler determines which bar item
		/// was clicked, places a checkmark to the selected bar item and changes
		/// the text color of the form's TextBox control. The example assumes
		/// that the System.Drawing namespace has been added to the form that
		/// this code is placed in. The example also assumes that a TextBox has
		/// been added to the form that this example code is located in that is
		/// named textBox1.
		/// <code lang="C#">
		/// // The following event handler would be connected to three menu items.
		/// private void MyMenuClick(Object sender, EventArgs e)
		/// {
		///		// Determine if clicked menu item is the Blue bar item.
		///		if(sender == menuItemBlue)
		///		{
		///			// Set the checkmark for the menuItemBlue bar item.
		///			menuItemBlue.Checked = true;
		///			// Uncheck the menuItemRed and menuItemGreen bar items.
		///			menuItemRed.Checked = false;
		///			menuItemGreen.Checked = false;
		///			// Set the color of the text in the TextBox control to Blue.
		///			textBox1.ForeColor = Color.Blue;
		///		}
		///		else if(sender == menuItemRed)
		///		{
		///			// Set the checkmark for the menuItemRed bar item.
		///			menuItemRed.Checked = true;
		///			// Uncheck the menuItemBlue and menuItemGreen bar items.
		///			menuItemBlue.Checked = false;
		///			menuItemGreen.Checked = false;
		///			// Set the color of the text in the TextBox control to Red.
		///			textBox1.ForeColor = Color.Red;
		///		}
		///		else
		///		{
		///			// Set the checkmark for the menuItemGreen.
		///			menuItemGreen.Checked = true;
		///			// Uncheck the menuItemRed and menuItemBlue bar items.
		///			menuItemBlue.Checked = false;
		///			menuItemRed.Checked = false;
		///			// Set the color of the text in the TextBox control to Blue.
		///			textBox1.ForeColor = Color.Green;
		///		}
		///	}
		/// </code>
		/// </example>
		[
		DefaultValue(false),
		Category("Appearance"),
		Description("Determines whether the BarItem should be drawn with a Checked appearance.")
		]
		public virtual bool Checked
		{
			get { return (this.GetState(STATE_CHECKED)); }
			set
			{
				if (this.SetState(STATE_CHECKED, value))
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Checked", null, null));
			}
		}

		#region ImageIndex
		/// <summary>
		/// Gets or sets the index value of the image displayed in the BarItem.
		/// </summary>
		/// <remarks>
		/// A zero-based index that represents the position in the ImageList
		/// control (assigned to the ImageList property) where the image is located.
		/// The default is -1.
		/// The ImageList (or the LargeImageList) and the ImageIndex property
		/// together will be used to determine the image that will be drawn
		/// in the BarItem.
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[Category("Appearance"), Description("Gets or sets the index value of the image displayed in the BarItem.")]
		[TypeConverter(typeof(BarItemImageIndexConverter)), Editor(typeof(BarItemImageIndexEditor), typeof(UITypeEditor))]
		[Localizable(true)]
		[DefaultValue(-1)]
		public virtual int ImageIndex
		{
			get
			{
				return this.m_imageIndex;
			}
			set
			{
				if (this.m_imageIndex != value)
				{
					this.m_imageIndex = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "ImageIndex", null, null));
				}
			}
		}
		#endregion

		#region ImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display in the BarItem.
		/// </summary>
		/// <remarks>
		/// An ImageList is that which stores the collection of image objects.
		/// The default value is a null reference (Nothing in Visual Basic) or the
		/// BarManager's ImageList to which this BarItem is a part of.
		/// <para>
		/// The ImageList and the ImageIndex property together will be used to
		/// determine the image that will be drawn in the BarItem. However, if
		/// the LargeIcons property of the BarManager this BarItem is part of is
		/// set to TRUE, the LargeImageList ImageList will be used rather than this
		/// ImageList property.
		/// </para>
		///
		/// </remarks>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display in the Label control.")]
		public virtual ImageList ImageList
		{
			get
			{
				if (this.imageList != null)
				{
					return this.imageList;
				}
				if (this.imageListAdv == null && this.barManager != null)
				{
					return this.barManager.ImageList;
				}
				return null;
			}
			set
			{
				if (this.imageList != value)
				{
					this.imageList = value;

					if (this.imageList != null)
					{
						this.imageListAdv = null;
					}
                    this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "ImageList", null, null));
		
				}

			}
		}
		protected bool ShouldSerializeImageList()
		{
			return this.imageList != null;
		}
		protected void ResetImageList()
		{
			this.imageList = null;
		}
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display in the BarItem.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display in the Label control.")]
		public virtual ImageListAdv ImageListAdv
		{
			get
			{
				if (this.imageListAdv != null)
				{
					return this.imageListAdv;
				}
				if (this.imageList == null && this.barManager != null)
				{
					return this.barManager.ImageListAdv;
				}
				return null;
			}
			set
			{
				if (this.imageListAdv != value)
				{
					this.imageListAdv = value;

					if (this.imageListAdv != null)
					{
						this.imageList = null;
					}
				}
			}
		}
		protected bool ShouldSerializeImageListAdv()
		{
			return this.imageListAdv != null;
		}
		protected void ResetImageListAdv()
		{
			this.imageListAdv = null;
		}
		#endregion

		#region LargeImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display in the
		/// BarItem when in LargeIcons mode.
		/// </summary>
		/// <remarks>
		/// An ImageList is that which stores the collection of image objects.
		/// The default value is a null reference (Nothing in Visual Basic) or the
		/// BarManager's ImageList to which this BarItem is a part of.
		/// <para>Make sure to set the ImageSize of this property's ImageList to a
		/// larger size than the ImageList associated with the ImageList property.
		/// The recommended size for this LargeImageList is 32X32.</para>
		/// <para>
		/// The LargeImageList and the ImageIndex property together will be used to
		/// determine the image that will be drawn in the BarItem if and only if this BarItem
		/// is parented to a BarManager and the LargeIcons property of the BarManager is
		/// set to TRUE.
		/// </para>
		/// </remarks>
		[Category("Appearance"), Description("ImageList to use when the BarManager is in LargeIcons mode.")]
		public virtual ImageList LargeImageList
		{
			get
			{
				if (this.largeImageList != null)
				{
					return this.largeImageList;
				}
				if (this.largeImageListAdv == null && this.barManager != null)
				{
					return this.barManager.LargeImageList;
				}
				return null;
			}
			set
			{
				this.largeImageList = value;

				if (this.largeImageList != null)
				{
					this.largeImageListAdv = null;
				}
			}
		}
		protected bool ShouldSerializeLargeImageList()
		{
			return this.largeImageList != null;
		}
		protected void ResetLargeImageList()
		{
			this.largeImageList = null;
		}
		/// <summary>
		/// ImageListAdv to use when the BarManager is in LargeIcons mode.
		/// </summary>
		[Category("Appearance"), Description("ImageListAdv to use when the BarManager is in LargeIcons mode.")]
		public virtual ImageListAdv LargeImageListAdv
		{
			get
			{
				if (this.largeImageListAdv != null)
				{
					return this.largeImageListAdv;
				}
				if (this.largeImageList == null && this.barManager != null)
				{
					return this.barManager.LargeImageListAdv;
				}
				return null;
			}
			set
			{
				this.largeImageListAdv = value;

				if (this.largeImageListAdv != null)
				{
					this.largeImageList = null;
				}
			}
		}
		protected bool ShouldSerializeLargeImageListAdv()
		{
			return this.largeImageListAdv != null;
		}
		protected void ResetLargeImageListAdv()
		{
			this.largeImageListAdv = null;
		}
		#endregion

		#region DisabledImageList

		/// <summary>
		/// Gets or sets the ImageList that contains the images to display in the disabled Label control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display in the disabled Label control.")]
		public virtual ImageList DisabledImageList
		{
			get
			{
				if (m_disabledImgList != null)
				{
					return m_disabledImgList;
				}
				if (m_disabledImgListAdv == null && this.barManager != null)
				{
					return this.barManager.DisabledImageList;
				}
				return null;
			}
			set
			{
				if (value != m_disabledImgList)
				{
					m_disabledImgList = value;

					if (this.m_disabledImgList != null)
					{
						this.m_disabledImgListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializeDisabledImageList()
		{
			return m_disabledImgList != null;
		}
		protected void ResetDisabledImageList()
		{
			this.m_disabledImgList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv that contains the images to display in the disabled Label control.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageListAdv that contains the images to display in the disabled Label control.")]
		public virtual ImageListAdv DisabledImageListAdv
		{
			get
			{
				if (m_disabledImgListAdv != null)
				{
					return m_disabledImgListAdv;
				}

				if (m_disabledImgList == null && this.barManager != null)
				{
					return this.barManager.DisabledImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_disabledImgListAdv)
				{
					m_disabledImgListAdv = value;

					if (this.m_disabledImgListAdv != null)
					{
						this.m_disabledImgList = null;
					}
				}
			}
		}
		private bool ShouldSerializeDisabledImageListAdv()
		{
			return m_disabledImgListAdv != null;
		}
		private void ResetDisabledImageListAdv()
		{
			this.m_disabledImgListAdv = null;
		}

		#endregion

		#region DisabledLargeImageList
		/// <summary>
		/// Gets or sets the ImageList with disabled images to be used when the BarManager is in LargeIcons mode.
		/// </summary>
		[Category("Appearance"), Description("ImageList with disabled images to use when the BarManager is in LargeIcons mode.")]
		public virtual ImageList DisabledLargeImageList
		{
			get
			{
				if (m_disabledLargeImgList != null)
				{
					return this.m_disabledLargeImgList;
				}
				if (m_disabledLargeImgListAdv == null && this.barManager != null)
				{
					return this.barManager.DisabledLargeImageList;
				}
				return null;
			}
			set
			{
				if (value != m_disabledLargeImgList)
				{
					m_disabledLargeImgList = value;

					if (m_disabledLargeImgList != null)
					{
						m_disabledLargeImgListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializeDisabledLargeImageList()
		{
			return this.m_disabledLargeImgList != null;
		}
		protected void ResetDisabledLargeImageList()
		{
			this.m_disabledLargeImgList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv with disabled images to be used when the BarManager is in LargeIcons mode.
		/// </summary>
		[Category("Appearance"), Description("ImageListAdv with disabled images to use when the BarManager is in LargeIcons mode.")]
		public virtual ImageListAdv DisabledLargeImageListAdv
		{
			get
			{
				if (m_disabledLargeImgListAdv != null)
				{
					return this.m_disabledLargeImgListAdv;
				}
				if (m_disabledLargeImgList == null && this.barManager != null)
				{
					return this.barManager.DisabledLargeImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_disabledLargeImgListAdv)
				{
					m_disabledLargeImgListAdv = value;

					if (m_disabledLargeImgListAdv != null)
					{
						m_disabledLargeImgList = null;
					}
				}
			}
		}
		private bool ShouldSerializeDisabledLargeImageListAdv()
		{
			return this.m_disabledLargeImgListAdv != null;
		}
		private void ResetDisabledLargeImageListAdv()
		{
			this.m_disabledLargeImgListAdv = null;
		}
		#endregion

		#region HighlightImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display during item's highlighted state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display for item's highlighted state.")]
		public virtual ImageList HighlightImageList
		{
			get
			{
				if (m_highlightImgList != null)
				{
					return m_highlightImgList;
				}
				if (m_highlightImgListAdv == null && this.barManager != null)
				{
					return this.barManager.HighlightImageList;
				}
				return null;
			}
			set
			{
				if (value != m_highlightImgList)
				{
					m_highlightImgList = value;

					if (m_highlightImgList != null)
					{
						m_highlightImgListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializeHighlightImageList()
		{
			return m_highlightImgList != null;
		}
		protected void ResetHighlightImageList()
		{
			this.m_highlightImgList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv that contains the images to display during item's highlighted state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageListAdv that contains the images to display for item's highlighted state.")]
		public virtual ImageListAdv HighlightImageListAdv
		{
			get
			{
				if (m_highlightImgListAdv != null)
				{
					return m_highlightImgListAdv;
				}
				if (m_highlightImgList == null && this.barManager != null)
				{
					return this.barManager.HighlightImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_highlightImgListAdv)
				{
					m_highlightImgListAdv = value;

					if (m_highlightImgListAdv != null)
					{
						m_highlightImgList = null;
					}
				}
			}
		}
		protected bool ShouldSerializeHighlightImageListAdv()
		{
			return m_highlightImgListAdv != null;
		}
		protected void ResetHighlightImageListAdv()
		{
			m_highlightImgListAdv = null;
		}
		#endregion

		#region HighlightLargeImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display during item's highlighted state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display during item's highlighted state.")]
		public virtual ImageList HighlightLargeImageList
		{
			get
			{
				if (m_highlightLargeImgList != null)
				{
					return m_highlightLargeImgList;
				}
				if (m_highlightLargeImgListAdv == null && this.barManager != null)
				{
					return this.barManager.HighlightLargeImageList;
				}
				return null;
			}
			set
			{
				if (value != m_highlightLargeImgList)
				{
					m_highlightLargeImgList = value;

					if (m_highlightLargeImgList != null)
					{
						m_highlightLargeImgListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializeHighlightLargeImageList()
		{
			return m_highlightLargeImgList != null;
		}
		protected void ResetHighlightLargeImageList()
		{
			this.m_highlightLargeImgList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv that contains the images to display during item's highlighted state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageListAdv that contains the images to display during item's highlighted state.")]
		public virtual ImageListAdv HighlightLargeImageListAdv
		{
			get
			{
				if (m_highlightLargeImgListAdv != null)
				{
					return m_highlightLargeImgListAdv;
				}
				if (m_highlightLargeImgList == null && this.barManager != null)
				{
					return this.barManager.HighlightLargeImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_highlightLargeImgListAdv)
				{
					m_highlightLargeImgListAdv = value;

					if (m_highlightLargeImgListAdv != null)
					{
						m_highlightLargeImgList = null;
					}
				}
			}
		}
		protected bool ShouldSerializeHighlightLargeImageListAdv()
		{
			return m_highlightLargeImgListAdv != null;
		}
		protected void ResetHighlightLargeImageListAdv()
		{
			m_highlightLargeImgListAdv = null;
		}
		#endregion

		#region PressedImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display during item's pressed state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display during item's pressed state.")]
		public virtual ImageList PressedImageList
		{
			get
			{
				if (m_pressedImageList != null)
				{
					return m_pressedImageList;
				}
				if (m_pressedImageListAdv == null && this.barManager != null)
				{
					return this.barManager.PressedImageList;
				}
				return null;
			}
			set
			{
				if (value != m_pressedImageList)
				{
					m_pressedImageList = value;

					if (m_pressedImageList != null)
					{
						m_pressedImageListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializePressedImageList()
		{
			return m_pressedImageList != null;
		}
		protected void ResetPressedImageList()
		{
			this.m_pressedImageList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv that contains the images to display during item's pressed state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageListAdv that contains the images to display during item's pressed state.")]
		public virtual ImageListAdv PressedImageListAdv
		{
			get
			{
				if (m_pressedImageListAdv != null)
				{
					return m_pressedImageListAdv;
				}
				if (m_pressedImageList == null && this.barManager != null)
				{
					return this.barManager.PressedImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_pressedImageListAdv)
				{
					m_pressedImageListAdv = value;

					if (m_pressedImageListAdv != null)
					{
						m_pressedImageList = null;
					}
				}
			}
		}
		protected bool ShouldSerializePressedImageListAdv()
		{
			return m_pressedImageListAdv != null;
		}
		protected void ResetPressedImageListAdv()
		{
			m_pressedImageListAdv = null;
		}
		#endregion

		#region PressedLargeImageList
		/// <summary>
		/// Gets or sets the ImageList that contains the images to display during item's pressed state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageList that contains the images to display during item's pressed state.")]
		public virtual ImageList PressedLargeImageList
		{
			get
			{
				if (m_pressedLargeImageList != null)
				{
					return m_pressedLargeImageList;
				}
				if (m_pressedLargeImageListAdv == null && this.barManager != null)
				{
					return this.barManager.PressedLargeImageList;
				}
				return null;
			}
			set
			{
				if (value != m_pressedLargeImageList)
				{
					m_pressedLargeImageList = value;

					if (m_pressedLargeImageList != null)
					{
						m_pressedLargeImageListAdv = null;
					}
				}
			}
		}
		protected bool ShouldSerializePressedLargeImageList()
		{
			return m_pressedLargeImageList != null;
		}
		protected void ResetPressedLargeImageList()
		{
			this.m_pressedLargeImageList = null;
		}

		/// <summary>
		/// Gets or sets the ImageListAdv that contains the images to display during item's pressed state.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets the ImageListAdv that contains the images to display during item's pressed state.")]
		public virtual ImageListAdv PressedLargeImageListAdv
		{
			get
			{
				if (m_pressedLargeImageListAdv != null)
				{
					return m_pressedLargeImageListAdv;
				}
				if (m_pressedLargeImageList == null && this.barManager != null)
				{
					return this.barManager.PressedLargeImageListAdv;
				}
				return null;
			}
			set
			{
				if (value != m_pressedLargeImageListAdv)
				{
					m_pressedLargeImageListAdv = value;

					if (m_pressedLargeImageListAdv != null)
					{
						m_pressedLargeImageList = null;
					}
				}
			}
		}
		protected bool ShouldSerializePressedLargeImageListAdv()
		{
			return m_pressedLargeImageListAdv != null;
		}
		protected void ResetPressedLargeImageListAdv()
		{
			m_pressedLargeImageListAdv = null;
		}
		#endregion

		/// <summary>
		/// Gets a boolean value indicating whether the DisabledImageIndex is valid. 
		/// </summary>
		protected internal bool IsValidDisabledImageIndex
		{
			get
			{
				IList images = GetDisabledImageListInternal(false);
				return (images != null && DisabledImageIndex >= 0 && DisabledImageIndex < images.Count);
			}
		}

		/// <summary>
		/// Determines if the HighlightedImageIndex is valid.
		/// </summary>
		/// <param name="bLargeIcons"></param>
		/// <returns></returns>
		protected internal bool IsValidHighlightedImageIndex(bool bLargeIcons)
		{
			IList images = GetHighlightImageListInternal(bLargeIcons);

			return images != null && this.HighlightedImageIndex >= 0 && this.HighlightedImageIndex < images.Count;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bLargeIcons"></param>
		/// <returns></returns>
		protected internal bool IsValidPressedImageIndex(bool bLargeIcons)
		{
			IList images = GetPressedImageListInternal(bLargeIcons);

			return images != null && this.PressedImageIndex >= 0 && this.PressedImageIndex < images.Count;
		}

		/// <summary>
		/// Gets or sets the index value of the image displayed in the highlighted BarItem.
		/// </summary>
		[
		DefaultValue(-1),
		Category("Appearance"),
		TypeConverter(typeof(HighlightedImageIndexConverter)),
		Editor(typeof(HighlightedImageIndexEditor), typeof(UITypeEditor)),
		Description("Gets or sets the index value of the image displayed in the highlighted BarItem."),
		Localizable(true),
		]
		public virtual int HighlightedImageIndex
		{
			get
			{
				return m_highlightedImageIndex;
			}
			set
			{
				if (value != m_highlightedImageIndex)
				{
					m_highlightedImageIndex = value;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[
		DefaultValue(-1),
		Category("Appearance"),
		TypeConverter(typeof(PressedImageIndexConverter)),
		Editor(typeof(PressedImageIndexEditor), typeof(UITypeEditor)),
		Description("Gets or sets the index value of the image displayed in the pressed BarItem."),
		Localizable(true),
		]
		public virtual int PressedImageIndex
		{
			get
			{
				return m_pressedImageIndex;
			}
			set
			{
				if (value != m_pressedImageIndex)
				{
					m_pressedImageIndex = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the index value of the image displayed in the disabled BarItem.
		/// </summary>
		[
		DefaultValue(-1),
		Category("Appearance"),
		TypeConverter(typeof(DisabledImageIndexConverter)),
		Editor(typeof(DisabledImageIndexEditor), typeof(UITypeEditor)),
		Description("Gets or sets the index value of the image displayed in the disabled BarItem."),
		Localizable(true),
		]
		public virtual int DisabledImageIndex
		{
			get
			{
				return m_disabledImgIndex;
			}
			set
			{
				if (value != m_disabledImgIndex)
				{
					m_disabledImgIndex = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the bar item is visible.
		/// </summary>
		/// <remarks>
		/// True if the bar item will be made visible on the parent menu/bar; false otherwise.
		/// The default is true.
		/// <para>You can use this property to modify a menu structure without having to
		/// merge menus or disable menus. For example, if you want to hide a complete
		/// section of functionality from the menus for your application, you can hide
		/// them from the user by setting this property to false.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example>
		/// The following example creates a bar item, sets the caption, assigns a
		/// shortcut key, makes the bar item visible and shows the shortcut key display
		/// for the bar item. The example assumes a BarItem object has been created
		/// that is named barItem1.
		/// <code lang="C#">
		/// public void SetupMyMenuItem()
		/// {
		/// 	// Set the caption for the bar item.
		/// 	barItem1.Text = "New";
		/// 	// Assign a shortcut key.
		/// 	barItem1.Shortcut = Shortcut.CtrlN;
		/// 	// Make the bar item visible.
		/// 	barItem1.Visible = true;
		/// }
		/// </code>
		/// </example>
		[DefaultValue(true),
		Category("Appearance"),
		Localizable(true),
		Description("Changes the BarItems visibility when placed in a menu or toolbar.")]
		public virtual bool Visible
		{
			get { return (this.GetState(STATE_VISIBLE)); }
			set
			{
				if (this.SetState(STATE_VISIBLE, value))
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Visible", null, null));
			}
		}
		/// <summary>
		/// Gets or sets the object that is associated with this BarItem.
		/// </summary>
		/// <remarks>
		/// An object that is associated with this BarItem. The default
		/// value is a null reference (Nothing in Visual Basic).
		/// <para>Any type derived from the Object class can be assigned to this property.
		/// If the Tag property is set through the Windows Forms designer, only text may
		/// be assigned.</para>
		/// A common use for the Tag property is to store data that is closely associated with this item.
		/// </remarks>
		[
		DefaultValue(null),
		Category("Misc"),
		TypeConverter(typeof(System.ComponentModel.StringConverter)),
		Description("An object that contains data about this BarItem.")
		]
		public virtual object Tag
		{
			get { return this.tag; }
			set { this.tag = value; }
		}
		/// <summary>
		/// Gets or sets the caption of the bar item.
		/// </summary>
		/// <remarks>
		/// The text caption of the menu item.
		/// <para>When you specify a caption for your menu item with the text parameter,
		/// you can also specify an access key by placing an '&amp;' before the character
		/// to be used as the access key. For example, to specify the "F" in "File" as
		/// an access key, you would specify the caption for the menu item as "&amp;File".
		/// You can use this feature to provide keyboard navigation for your menus.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example>
		/// The following example creates a bar item, sets the caption, assigns a
		/// shortcut key, makes the item visible, and shows the shortcut key display
		/// for the item. The example assumes a BarItem object has been created
		/// that is named barItem1.
		/// <code lang="C#">
		/// public void SetupMyMenuItem()
		/// {
		/// 	// Set the caption for the bar item.
		/// 	barItem1.Text = "New";
		/// 	// Assign a shortcut key.
		/// 	barItem1.Shortcut = Shortcut.CtrlN;
		/// 	// Make the bar item visible.
		/// 	barItem1.Visible = true;
		/// }
		/// </code>
		/// <code lang="VB">
		/// Public Sub SetupMyMenuItem()
		///		' Set the caption for the bar item.
		///		barItem1.Text = "New"
		///		' Assign a shortcut key.
		///		barItem1.Shortcut = Shortcut.CtrlN
		/// 	' Make the bar item visible.
		///		barItem1.Visible = True
		/// End Sub 'SetupMyMenuItem
		/// </code>
		/// </example>
		[
		DefaultValue(""),
		Localizable(true),
		Category("Appearance"),
		Description("The BarItem's caption."),
		MergableProperty(false),
		Bindable(BindableSupport.Yes),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))
        ]
		public virtual string Text
		{
			get { return this.text; }
			set
			{
				if (this.text != value)
				{
					string oldValue = text;

					this.text = value;

					// Validate ID Uniqueness
					if (this.barManager != null)
					{
						// Will automatically update ID only if it is Empty.
						if (this.ID == String.Empty || IDGenerator.IsAutoGeneratedID(this.ID))
						{
							string newID = this.text;
							while (!this.barManager.Items.IsValidItemID(this, newID))
								newID = IDGenerator.GetNextID(newID);
							this.ID = newID;
						}
					}
					// Give ID a reasonable value, for now. Will be validated when added to a BarManager.
					else if (this.ID == String.Empty && value != String.Empty)
						this.ID = this.text;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Text", oldValue, value));
				}
			}
		}

        String barName = "BarItem";
        public virtual string BarName
        {
            get
            {
                return this.barName;
            }
            set
            {
                this.barName = value;
            }
        }

		/// <summary>
		/// Gets or sets the BarItem's ID. Should be unique among all the BarItems in a <see cref="BarManager"/>, if added to a BarManager.
		/// </summary>
		/// <value>A string representing the ID.</value>
		/// <remarks>
		/// A default value will be generated for this property based on the Text value.
		/// </remarks>
		[
		Category("ID"),
		Description("The BarItem's ID."),
		MergableProperty(false),
		DefaultValue("")]
		public virtual string ID
		{
			get
			{
				return this.id;
			}
			set
			{
                string bname = TypeDescriptor.GetComponentName(this, true);
                if (bname != null)
                {
                    BarName = bname;
                }
				if (this.id != value)
				{
					// Validate ID Uniqueness
					if (this.barManager != null)
					{
						if (!this.barManager.Items.IsValidItemID(this, value))
						{
							if (this.barManager.DesignMode)
								MessageBox.Show("ID Update Failed. A BarItem with the same ID value already exists in the BarManager. Set the ID value of this BarItem to be unique.");
							else
								MessageBox.Show(SR.GetString(SR.DuplicateNameWarning, value));
							return;
						}
					}

					string sOldID = this.id;
					this.id = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.None, "ID", sOldID, value));
				}
			}
		}

		//		public void ResetID()
		//		{
		//			// Reset the ID only if the Text itself will be unique.
		//			if(this.ShouldSerializeID())
		//			{
		//				if(this.barManager != null)
		//				{
		//					if(this.barManager.Items.IsValidItemID(this, this.Text))
		//						this.id = String.Empty;
		//					else if(this.DesignMode)
		//						MessageBox.Show("Cannot reset ID to default as the Text property is not unique");
		//				}
		//				else
		//					this.id = String.Empty;
		//			}
		//		}
		//		public bool ShouldSerializeID()
		//		{
		//			return true;
		//		}

		/// <summary>
		/// Gets or sets the tooltip for the item.
		/// </summary>
		/// <value>A string representing the tooltip.</value>
		/// <remarks>
		/// <para>
		/// Tooltips will be shown only when the item is in a toolbar (not in a dropdown submenu).
		/// When this tooltip text is empty, a tooltip will be synthesized based on the
		/// Text property and the Shortcut property.
		/// </para>
		/// </remarks>
		[
		Category("Appearance"),
		DefaultValue(""),
		Localizable(true),
		Description("The BarItem's tooltip."),
		MergableProperty(false),
		Bindable(BindableSupport.Yes),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))
		]
		public virtual string Tooltip
		{
			get { return this.tooltip; }
			set { this.tooltip = value; }
		}

		/// <summary>
		/// Gets or sets the category under which this BarItem will be listed in the Customization Dialog.
		/// </summary>
		/// <remarks>
		/// <para>The CategoryIndex is used to categorize the BarItem in the Customization
		/// Dialog.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
		Category("ID"),
		Description("The Category under which this BarItem will be listed in the Customization Dialog."),
		DefaultValue(-1)
		]
		public virtual int CategoryIndex
		{
			get { return this.categoryIndex; }
			set
			{
				if (this.categoryIndex != value)
				{
					if (this.barManager != null)
					{
						if (this.barManager.Categories.Count <= value)
						{
							if (this.barManager.DesignMode)
								MessageBox.Show("Category Update Failed. The new value exceeded the available Categories count. Please specify a value less than " + this.barManager.Categories.Count.ToString());
							return;
						}
					}
					int oldValue = this.categoryIndex;
					this.categoryIndex = value;
					// Validate ID Uniqueness (Need to do this now because validation was always successful with cat was -1.
					if (this.categoryIndex != -1 && oldValue == -1 &&
						this.barManager != null)
					{
						string adjustedID = this.ID;
						while (!this.barManager.Items.IsValidItemID(this, adjustedID))
							adjustedID = IDGenerator.GetNextID(adjustedID);
						this.ID = adjustedID;
					}
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "CategoryIndex", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the shortcut key associated with the menu item.
		/// </summary>
		/// <remarks>
		/// <para>One of the Shortcut values. The default is Shortcut.None.</para>
		/// <para>Shortcut keys provide a method for users to activate frequently
		/// used menu items in your menu system and to provide keyboard access to
		/// your application for those users who do not have access to a mouse or
		/// other pointer device.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		/// <example>
		///  The following example creates a menu item, sets the caption, assigns
		///  a shortcut key, makes the menu item visible and shows the shortcut
		///  key display for the menu item. The example assumes a BarItem
		///  object has been created that is named menuItem1.
		///  <para>
		///  <code lang = "C#">
		///  public void SetupMyMenuItem()
		///  {
		///         // Set the caption for the menu item.
		///         menuItem1.Text = "New";
		///         // Assign a shortcut key.
		///         menuItem1.Shortcut = Shortcut.CtrlN;
		///         // Make the menu item visible.
		///         menuItem1.Visible = true;
		/// }
		/// </code>
		/// <code lang="VB">
		/// Public Sub SetupMyMenuItem()
		///		' Set the caption for the menu item.
		///		menuItem1.Text = "New"
		///		' Assign a shortcut key.
		///		menuItem1.Shortcut = Shortcut.CtrlN
		///		' Make the menu item visible.
		///		menuItem1.Visible = True
		///	End Sub 'SetupMyMenuItem
		///  </code>
		///  </para>
		/// </example>
		[DefaultValue(Shortcut.None),
		Category("Behavior"),
		Localizable(true),
		Description("Indicates the shortcut key associated with the menu item.")
		]
		public virtual Shortcut Shortcut
		{
			get { return this.shortcut; }
			set
			{
				if (this.shortcut != value)
				{
					Shortcut oldValue = this.shortcut;
					this.shortcut = value;
					if (this.shortcut == Shortcut.None)
						this.shortcutText = null;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "Shortcut", oldValue, this.shortcut));
				}
			}
		}

		/// <summary>
		/// Gets or sets the custom shortcut text that is to be used in displaying the menu item.
		/// </summary>
		/// <value>By default, this property will return a string based on the <see cref="Shortcut"/>
		/// property value. But if you specify a non-null value (including String.Empty) to this property that value
		/// will be returned.</value>
		/// <remarks>
		/// <p>To reset this property programmatically, specify null. In design time select "Reset"
		/// from the context menu of the property grid entry of this property.</p>
		/// <p>This property is useful when you want to replace the default
		/// shortcut text (for example, "Ctrl+D0", displayed when Shortcut.Ctrl0 is the shortcut),
		/// with something else (say "Ctrl0", in the above example).
		/// </p>
		/// </remarks>
		[
		Category("Behavior"),
		Localizable(true),
		Description("Specifies the custom shortcut text to be used in the display.")
		]
		public virtual string ShortcutText
		{
			get
			{
				if (this.shortcutText != null)
					return this.shortcutText;
				else if (this.Shortcut == Shortcut.None)
					return String.Empty;
				else
					return TypeDescriptor.GetConverter(typeof(System.Windows.Forms.Keys)).ConvertToString((int)this.Shortcut);

			}
			set
			{
				if (this.shortcutText != value)
				{
					this.shortcutText = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "ShortcutText", null, null));
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public void ResetShortcutText()
		{
			this.ShortcutText = null;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ShouldSerializeShortcutText()
		{
			return this.shortcutText != null;
		}
		/// <summary>
		/// The BarManager to which this BarItem will be parented to.
		/// </summary>
		/// <remarks>
		/// <para>Including this BarItem in a BarManager will allow it to be usable in a
		/// Form's Menus and tool bars. And also participate in the Customization feature
		/// enabled by the BarManager. Make sure that this BarItem has a unique text and category index before you add it to a manager.</para>
		/// <para>The BarItem can also be created and used outside the context of
		/// BarManagers when used in a stand-alone PopupMenu. Take a look at the PopupMenu
		/// class reference for an example of how to create and use a stand-alone BarItem in a PopupMenu.</para>
		/// </remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Category("ID"),
		RefreshProperties(RefreshProperties.All),
		Description("The BarManager to which this BarItem will be parented to.")
		]
		public virtual BarManager Manager
		{
			get { return this.barManager; }
			set
			{
				if (this.barManager != value)
				{
					if (this.barManager != null
						&& this.barManager.Items.IndexOf(this) != -1)
					{
						if (this.barManager.SelectedItem == this)
							this.barManager.SelectedItem = null;

						this.barManager.Items.Remove(this);
					}

					this.barManager = value;

					if (null != this.barManager)
					{
						MainFrameBarManager mainManger = this.barManager.MainFrameBarManager;
						if (mainManger != null && !mainManger.DesignMode &&
							!mainManger.Initializing && mainManger.NeedSaveCustomData)
						{
							m_bDesignTimeCreated = false;
						}

                        bool requiredAdding = true;

                        foreach (BarItem barItm in barManager.Items)
                        {
                            if (barItm.Text == this.Text)
                            {
                                requiredAdding = false;
                                break;
                            }
                        }
						// To avoid recursion
						if (requiredAdding && this.barManager.Items.IndexOf(this) == -1)
						{
							this.barManager.Items.Add(this);
						}
					}
				}
			}
		}

		private bool m_bDesignTimeCreated = true;
		internal bool DesignTimeCreated
		{
			get
			{
				return m_bDesignTimeCreated;
			}
			set
			{
				if (m_bDesignTimeCreated != value)
				{
					m_bDesignTimeCreated = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether this bar item will appear in it's
		/// parent's partial menus list.
		/// </summary>
		/// <remarks>
		/// True to make this appear in partial menus; False otherwise.
		/// Default value is true.
		/// <para>When this value is true, the item will always be visible when it's parent menu is
		/// shown. </para>
		/// <para>If false, the item will appear only when the user expands the hidden portion of the
		/// partial parent menu (through the arrows at the bottom). Note that this assumes that the parent menu has
		/// its UsePartialMenus property set to true.
		/// Note that if the user selects this item then this value will be set to true
		/// for the period of RecentlyUsedItemResetDelay. Also, this property is meaningful
		/// only when this item is a child of another menu(ParentBarItem).</para>
		/// <para>If this is a <see cref="ParentBarItem"/> then this value
		/// can be set to false only if all the children also have their
		/// corresponding property set to false.</para>
		/// <para>Changing this property's value will fire the <see cref="BarItem.PropertyChanged"/> event.</para>
		/// </remarks>
		[DefaultValue(true),
		Category("Appearance"),
		Localizable(true),
		Description("Indicate whether this bar item will appear in it's	parent's partial menus list.")
		]
		public virtual bool IsRecentlyUsedItem
		{
			get { return !this.GetState(STATE_ISNOT_RECENTLYUSEDITEM); }
			set
			{
				if (this.SetState(STATE_ISNOT_RECENTLYUSEDITEM, !value))
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "IsRecentlyUsedItem", null, null));
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="UpdateUI"/> event should be fired in the next application
		/// idle event.
		/// </summary>
		/// <value>True to fire the UpdateUI event; false otherwise. Default is false.</value>
		/// <remarks>
		/// If this property is set to true, then this BarItem will listen to the
		/// <see cref="System.Windows.Forms.Application.Idle"/> event and then fire the <see cref="UpdateUI"/>
		/// event. It will continue doing this in the Application.Idle event handler until you turn
		/// off this property. Take a look at the <see cref="UpdateUI"/> event for more information
		/// on it and when you should use this pattern for your BarItem UI update.
		/// </remarks>
		[DefaultValue(false),
		Description("Specifies whether the UpdateUI event should be fired in the next Application Idle event."),
		Category("Behavior")
		]
		public bool UpdateUIOnAppIdle
		{
			get { return this._uiUpdateOnAppIdle; }
			set
			{
				if (this._uiUpdateOnAppIdle != value)
				{
					this._uiUpdateOnAppIdle = value;
					if (this._uiUpdateOnAppIdle)
						Application.Idle += new EventHandler(this.OnIdle);
					else
						Application.Idle -= new EventHandler(this.OnIdle);
				}
			}
		}

		/// <summary>
		/// This property is obsolete, please use the UpdateUIOnAppIdle instead.
		/// </summary>
		[Obsolete("This property is obsolete, please use the UpdateUIOnAppIdle instead."),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UpdateUIRequired
		{
			get { return this.UpdateUIOnAppIdle; }
			set { this.UpdateUIOnAppIdle = value; }

		}

		/// <summary>
		/// Indicates whether the bar item image should be drawn mirrored.
		/// </summary>
		[
			Category("Appearance"),
			Browsable(false),
			DefaultValue(false)
		]
		public bool DrawImageMirrored
		{
			get
			{
				return m_bDrawImageMirrored;
			}
			set
			{
				if (value != m_bDrawImageMirrored)
				{
					m_bDrawImageMirrored = value;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(
						PropertyChangeEffect.NeedRepaint, "DrawImageMirrored", !value, value));
				}
			}
		}
        private bool sizetofit = true;
        public bool SizeToFit
        {
            get
            {
                return sizetofit;
            }
            set
            {
                sizetofit = value;
            }
        }

		private void OnIdle(object sender, EventArgs e)
		{
			if (this.UpdateUIOnAppIdle)
				this.OnUpdateUI(EventArgs.Empty);
		}

		[Documentation.DocumentationExclude]
		internal virtual bool HintViaHotKeyPrefix
		{
			get
			{
				return m_bHintViaHotKeyPrefix;
			}
			set
			{
				m_bHintViaHotKeyPrefix = value;
			}
		}

		// Valid only for XPToolBar related items.
		[Documentation.DocumentationExclude]
		internal bool LargeIcons
		{
			get
			{
				return m_bLargeIcons;
			}
			set
			{
				m_bLargeIcons = value;
			}
		}

		/// <summary>
		/// This property can be optionally set to be the top level form that hosts this BarItem in the 
		/// case that the BarItem is also hosted by another form embedded inside a UserControl. Setting this 
		/// property to be the top level parent form will make tooltips work for this BarItem.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("This property can be optionally set to be the top level form that hosts this BarItem in the case that the BarItem is also hosted by another form embedded inside a UserControl.")]
		public Form TopLevelForm
		{
			get
			{
				return this.topLevelForm;
			}

			set
			{
				this.topLevelForm = value;
			}
		}

		/// <summary>
		/// Indicates whether the tooltip is shown.
		/// </summary>
		[
		DefaultValue(true),
		Description("Indicates whether the tooltip is shown."),
		Category("Appearance")
		]
		public bool ShowTooltip
		{
			get
			{
				return m_bShowTooltip;
			}
			set
			{
				if (value != m_bShowTooltip)
				{
					m_bShowTooltip = value;
					OnShowTooltipChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets displayed icon or bitmap.
		/// </summary>
		[
		DefaultValue(null),
		Description("Gets or sets the image displayed in the barItem."),
		Category("Appearance")
		]
		public virtual ImageExt Image
		{
			get
			{
				return m_image;
			}
			set
			{
				if (value != m_image)
				{
					m_image = value;

					if (m_image != null)
					{
						m_image.ImageTransparentColor = m_imageTransparentColor;
					}

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "Image", null, null));
				}
			}
		}

		/// <summary>
		/// Gets or Sets, the transparency color for the image.
		/// </summary>
		[Category("Appearance"), Description("Indicates the transparency color for the image.")]
		public Color ImageTransparentColor
		{
			get
			{
				return m_imageTransparentColor;
			}
			set
			{
				if (m_imageTransparentColor != value)
				{
					Color oldValue = m_imageTransparentColor;
					m_imageTransparentColor = value;

					if (value != Color.Empty && m_image != null)
					{
						m_image.ImageTransparentColor = value;

						this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "ImageTransparentColor", oldValue, m_imageTransparentColor));
					}
				}
			}
		}

		protected bool ShouldSerializeImageTransparentColor()
		{
			return m_imageTransparentColor != Color.Empty;
		}

		protected void ResetImageTransparentColor()
		{
			m_imageTransparentColor = Color.Empty;
		}

		/// <summary>
		/// Gets or sets the image painting while BarItem is disabled.
		/// </summary>
		[
		DefaultValue(null),
		Description("Disabled image."),
		Category("Appearance")
		]
		public ImageExt DisabledImage
		{
			get
			{
				return m_disabledImage;
			}
			set
			{
				if (value != m_disabledImage)
				{
					m_disabledImage = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "DisabledImage", null, null));
				}
			}
		}

		/// <summary>
		/// Gets or sets the image painting while BarItem is highlighted.
		/// </summary>
		[
		DefaultValue(null),
		Description("Highlighted image."),
		Category("Appearance")
		]
		public ImageExt HighlightedImage
		{
			get
			{
				return m_highlightImage;
			}
			set
			{
				if (value != m_highlightImage)
				{
					m_highlightImage = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "HighlightImage", null, null));
				}
			}
		}

		#region ImageSize
		/// <summary>
		/// Gets or set size for <see cref="Image"/>.
		/// </summary>
		[Description("Size for Image."), Category("Appearance")]
		public Size ImageSize
		{
			get
			{
				return m_imageSize;
			}
			set
			{
				if (value != m_imageSize)
				{
					m_imageSize = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedRepaint, "ImageSize", null, null));
				}
			}
		}
		protected void ResetImageSize()
		{
			ImageSize = DEF_IMAGE_SIZE;
		}
		protected bool ShouldSerializeImageSize()
		{
			return (m_imageSize != DEF_IMAGE_SIZE);
		}
		#endregion

		/// <summary>
		/// Gets or sets value whether doubleClick event is triggered on demand.
		/// </summary>
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Gets or sets value whether doubleClick event is triggered on demand.")
		]
		public bool HandleDoubleClick
		{
			get
			{
				return m_handleDoubleClick;
			}
			set
			{
				m_handleDoubleClick = value;
			}
		}
		#endregion PROPERTIES

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this BarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this BarItem.</returns>
		/// <remarks>
		/// Creates a new instance of BarItem and calls the <see cref="CopyTo"/> method to copy over properties.
		/// </remarks>
		public virtual object Clone()
		{
			BarItem newItem = new BarItem();
			this.CopyTo(newItem);
			return newItem;
		}

		/// <summary>
		/// Copies the properties of this BarItem into the specified BarItem.
		/// </summary>
		/// <param name="barItem">The BarItem where the values should be copied to.</param>
		/// <remarks>
		/// The Manager property will not be copied over.
		/// </remarks>
		public virtual void CopyTo(BarItem barItem)
		{
			barItem.CategoryIndex = this.CategoryIndex;
			barItem.Checked = this.Checked;
			barItem.Enabled = this.Enabled;
			barItem.ImageIndex = this.ImageIndex;
			barItem.MergeOrder = this.MergeOrder;
			barItem.MergeType = this.MergeType;
			barItem.PaintStyle = this.PaintStyle;
			barItem.Shortcut = this.Shortcut;
			barItem.Text = this.Text;
            barItem.Image = this.Image;
			barItem.Tooltip = this.Tooltip;
			barItem.ID = this.ID;
			barItem.ImageList = this.ImageList;
			barItem.ImageListAdv = this.ImageListAdv;
			barItem.LargeImageList = this.LargeImageList;
			barItem.LargeImageListAdv = this.LargeImageListAdv;
			barItem.DisabledImageIndex = this.DisabledImageIndex;
			barItem.DisabledImageList = this.DisabledImageList;
			barItem.DisabledImageListAdv = this.DisabledImageListAdv;
			barItem.DisabledLargeImageList = this.DisabledLargeImageList;
			barItem.DisabledLargeImageListAdv = this.DisabledLargeImageListAdv;
			barItem.HighlightedImageIndex = this.HighlightedImageIndex;
			barItem.HighlightImageList = this.HighlightImageList;
			barItem.HighlightImageListAdv = this.HighlightImageListAdv;
			barItem.HighlightLargeImageList = this.HighlightLargeImageList;
			barItem.HighlightLargeImageListAdv = this.HighlightLargeImageListAdv;

			barItem.PressedImageList = this.PressedImageList;
			barItem.PressedImageListAdv = this.PressedImageListAdv;
			barItem.PressedLargeImageList = this.PressedLargeImageList;
			barItem.PressedLargeImageListAdv = this.PressedLargeImageListAdv;

			barItem.DesignTimeCreated = this.DesignTimeCreated;

			// Event handlers:
			barItem.Click = this.Click;
		}
		#endregion CLONING

		#region SERIALIZATION
		protected BarItem(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "CategoryIndex":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetInt32("CategoryIndex");
							this.CategoryIndex = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.CategoryIndex = (int)entry.Value;
						break;
					case "Checked":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("Checked");
							this.Checked = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.Checked = (bool)entry.Value;
						break;
					case "Enabled":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("Enabled");
							this.Enabled = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.Enabled = (bool)entry.Value;
						break;
					case "ImageIndex":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("ImageIndex");
							this.ImageIndex = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.ImageIndex = (int)entry.Value;
						break;
					case "DisabledImageIndex":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("ImageIndex");
							this.DisabledImageIndex = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.DisabledImageIndex = (int)entry.Value;
						break;
					case "HighlightedImageIndex":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("ImageIndex");
							this.HighlightedImageIndex = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.HighlightedImageIndex = (int)entry.Value;
						break;
					case "MergeOrder":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("MergeOrder");
							this.MergeOrder = (int)Convert.ChangeType(entry.Value, typeof(int));
						else
							this.MergeOrder = (int)entry.Value;
						break;
					case "MergeType":
						this.MergeType = (MenuMerge)entry.Value;
						break;
					case "PaintStyle":
						this.PaintStyle = (PaintStyle)entry.Value;
						break;
					case "Shortcut":
						this.Shortcut = (Shortcut)entry.Value;
						break;
					case "Text":
						this.Text = (string)entry.Value;
						break;
					case "Tooltip":
						this.Tooltip = (string)entry.Value;
						break;
					case "ID":
						this.ID = (string)entry.Value;
						break;
					case "Manager":
						this.Manager = (BarManager)entry.Value;
						break;
				}
			}
			//			this.CategoryIndex = info.GetInt32("CategoryIndex");
			//			this.Checked = info.GetBoolean("Checked");
			//			this.Enabled = info.GetBoolean("Enabled");
			//			this.ImageIndex = info.GetInt32("ImageIndex");
			//			this.MergeOrder = info.GetInt32("MergeOrder");
			//			this.MergeType = (MenuMerge)info.GetValue("MergeType", typeof(MenuMerge));
			//			this.PaintStyle = (PaintStyle)info.GetValue("PaintStyle", typeof(PaintStyle));
			//			this.Shortcut = (Shortcut)info.GetValue("Shortcut", typeof(Shortcut));
			//			this.Text = info.GetString("Text");
			//			this.Tooltip = info.GetString("Tooltip");
			//			this.ID = info.GetString("ID");
			//			// Specify typeof(object), instead of typeof(BarManager). Otherwise, the framework
			//			// has trouble converting this to a BarManager.
			//			this.Manager = (BarManager)info.GetValue("Manager", typeof(object));
		}
		/// <summary>
		/// Adds the BarItem properties to the SerializationInfo object for Serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("CategoryIndex", this.CategoryIndex);
			info.AddValue("Checked", this.Checked);
			info.AddValue("Enabled", this.Enabled);
			info.AddValue("ImageIndex", this.ImageIndex);
			info.AddValue("MergeOrder", this.MergeOrder);
			info.AddValue("MergeType", this.MergeType);
			info.AddValue("PaintStyle", this.PaintStyle);
			info.AddValue("Shortcut", this.Shortcut);
			info.AddValue("Text", this.Text);
			info.AddValue("Tooltip", this.Tooltip);
			info.AddValue("ID", this.ID);
			info.AddValue("Manager", this.Manager);
			info.AddValue("DisabledImageIndex", this.DisabledImageIndex);
			info.AddValue("HighlightedImageIndex", this.HighlightedImageIndex);
		}
		#endregion SERIALIZATION

		#region PUBLIC_MEMBERS
		/// <summary>
		/// Forces the BarItem to fire an UpdateUI event.
		/// </summary>
		public void PerformUpdateUI()
		{
			this.OnUpdateUI(EventArgs.Empty);
		}

        /// <summary>
        /// Forces the BarItem to fire an ItemDoubleClicked event.
        /// </summary>
        public void PerformDoubleClick()
        {
            if (this.Enabled)
            {
                if (this.Manager != null && !this.Manager.Customizing
                    && this.Manager.MainFrameBarManager != null)
                {
                    this.Manager.MainFrameBarManager.RecordRecentlyUsedItemClicked(this);
                }
                this.OnItemDoubleClicked(EventArgs.Empty);
                m_bDoubleClick = true;
                m_lastClickLocation = Cursor.Position;
            }
        }
        
		/// <summary>
		/// Forces the BarItem to fire an ItemClicked event.
		/// </summary>
		public void PerformClick()
		{
			if (this.Enabled)
			{
				if (this.Manager != null && !this.Manager.Customizing
					&& this.Manager.MainFrameBarManager != null)
				{
					this.Manager.MainFrameBarManager.RecordRecentlyUsedItemClicked(this);
				}
			
				this.OnItemClicked(EventArgs.Empty);
				m_bDoubleClick = false;

				m_tiksLastClick = DateTime.Now.Ticks;
				m_lastClickLocation = Cursor.Position;
			}
		}
		/// <summary>
		/// Forces the BarItem to fire a Selected event.
		/// </summary>
		public void PerformSelected()
		{
			// Do nothing when Customizing
			if ((this.Manager != null && this.Manager.Customizing))
				return;

			this.OnSelected(EventArgs.Empty);
		}
		/// <summary>
		/// Forces the BarItem to fire an Unselected event.
		/// </summary>
		public void PerformUnselected()
		{
			// Do nothing when Customizing
			if ((this.Manager != null && this.Manager.Customizing))
				return;

			this.OnUnselected(EventArgs.Empty);
		}
		/// <summary>
		/// Indicates whether the component is currently in design mode.
		/// </summary>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new bool DesignMode
		{
			get { return base.DesignMode; }
		}

		/// <summary>
        /// Lets you data-bind certain properties of the Form containing the MainFrameBarManager of this BarItem.
		/// </summary>
		[
		Category(@"Data"),
		ParenthesizePropertyName(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(
			typeof(Syncfusion.Windows.Forms.Design.ControlBindingsConverter)),
        Description("Lets you data-bind certain properties of the Form containing the MainFrameBarManager of this BarItem.")
		]
		public ControlBindingsCollection DataBindings
		{
			get
			{
				if (this.Manager != null && this.Manager.Form != null)
					return this.Manager.Form.DataBindings;
				else
					return null;
			}
		}
		string IDataBindingSupport.PropertyNamePrefix
		{
			get
			{
				return "MenuItem:" + this.ID;
			}
		}
		#endregion PUBLIC_MEMBERS

		#region Internal methods
		/// <summary>
		/// 
		/// </summary>
		internal IList GetImageListInternal(bool bLargeIcons)
		{
			if (bLargeIcons)
			{
				ImageListAdv imageListAdv = this.LargeImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}

				ImageList imageList = this.LargeImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			else
			{
				ImageListAdv imageListAdv = this.ImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}

				ImageList imageList = this.ImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bLargeIcons"></param>
		/// <returns></returns>
		internal IList GetDisabledImageListInternal(bool bLargeIcons)
		{
			if (bLargeIcons)
			{
				ImageListAdv imageListAdv = this.DisabledLargeImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.DisabledLargeImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			else
			{
				ImageListAdv imageListAdv = this.DisabledImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.DisabledImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bLargeIcons"></param>
		/// <returns></returns>
		internal IList GetHighlightImageListInternal(bool bLargeIcons)
		{
			if (bLargeIcons)
			{
				ImageListAdv imageListAdv = this.HighlightLargeImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.HighlightLargeImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			else
			{
				ImageListAdv imageListAdv = this.HighlightImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.HighlightImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			return null;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="bLargeIcons"></param>
		/// <returns></returns>
		internal IList GetPressedImageListInternal(bool bLargeIcons)
		{
			if (bLargeIcons)
			{
				ImageListAdv imageListAdv = this.PressedLargeImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.PressedLargeImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			else
			{
				ImageListAdv imageListAdv = this.PressedImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.Images;
				}
				ImageList imageList = this.PressedImageList;
				if (imageList != null)
				{
					return imageList.Images;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		internal Size GetImageSizeInternal(bool bLargeIcons)
		{
			if (this.Image != null)
			{
				return this.ImageSize;
			}
			if (bLargeIcons)
			{
				ImageListAdv imageListAdv = this.LargeImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.ImageSize;
				}

				ImageList imageList = this.LargeImageList;
				if (imageList != null)
				{
					return imageList.ImageSize;
				}
			}
			else
			{
				ImageListAdv imageListAdv = this.ImageListAdv;
				if (imageListAdv != null)
				{
					return imageListAdv.ImageSize;
				}

				ImageList imageList = this.ImageList;
				if (imageList != null)
				{
					return imageList.ImageSize;
				}
			}
			return Size.Empty;
		}
		#endregion

		#region PROTECTED_MEMBERS
		/// <summary>
		/// Raises the AfterPopupItemPaint event.
		/// </summary>
		/// <param name="eventArgs"></param>

		protected internal virtual void OnAfterPopupItemPaint(PopupItemPaintEventArgs eventArgs)
		{
			if (this.AfterPopupItemPaint != null)
			{
				this.AfterPopupItemPaint(this, eventArgs);
			}
		}
		/// <summary>
		/// Raises the BeforePopupItemPaint event.
		/// </summary>
		/// <param name="eventArgs"></param>

		protected internal virtual void OnBeforePopupItemPaint(PopupItemPaintEventArgs eventArgs)
		{
			if (this.BeforePopupItemPaint != null)
			{
				this.BeforePopupItemPaint(this, eventArgs);
			}
		}

		/// <summary>
		/// Raises the DrawToolbarItem event.
		/// </summary>
		/// <param name="eventArgs">A <see cref="DrawToolbarItemEventArgs"/> that contains the event data.</param>
		/// <returns>True if there were listeners; false otherwise.</returns>
		/// <remarks>Raising an event invokes the event handler
		/// through a delegate. For more information, see Raising
		/// an Event. <para>The OnDrawToolbarItem method also
		/// allows derived classes to handle the event without
		/// attaching a delegate. This is the preferred technique
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnDrawToolbarItem
		/// in a derived class, be sure to call the base class's
		/// OnDrawToolbarItem method so that registered
		/// delegates receive the event.</para>
		/// </remarks>
		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
			Browsable(false)]
		protected internal virtual bool OnDrawToolbarItem(DrawToolbarItemEventArgs eventArgs)
		{
			if (this.DrawToolbarItem != null)
			{
				this.DrawToolbarItem(this, eventArgs);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="args">An SyncfusionPropertyChangedEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnPropertyChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Notes to Inheritors:  When overriding OnPropertyChanged in a derived
		/// class, be sure to call the base class's OnPropertyChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnPropertyChanged(SyncfusionPropertyChangedEventArgs args)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, args);
			}
		}

		/// <summary>
		/// Raises the CanDragDrop event.
		/// </summary>
		/// <param name="args">An CanDragDropEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnCanDragDrop method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Notes to Inheritors:  When overriding OnCanDragDrop in a derived
		/// class, be sure to call the base class's OnCanDragDrop method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnCanDragDrop(CanDragDropEventArgs args)
		{
			if (this.CanDragDrop != null)
			{
				this.CanDragDrop(this, args);
			}
		}

		/// <summary>
		/// Raises the UpdateUI event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnUpdateUI method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Notes to Inheritors:  When overriding OnUpdateUI in a derived
		/// class, be sure to call the base class's OnUpdateUI method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnUpdateUI(EventArgs args)
		{
			if (this.UpdateUI != null)
			{
				this.UpdateUI(this, args);
			}
		}

		/// <summary>
		/// Raises the ProvideFontInfo event.
		/// </summary>
		/// <param name="args">A ProvideFontInfoEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnProvideFontInfo method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Notes to Inheritors:  When overriding OnProvideFontInfo in a derived
		/// class, be sure to call the base class's OnProvideFontInfo method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnProvideFontInfo(ProvideFontInfoEventArgs args)
		{
			if (this.CustomTextFont != null && this.CustomTextFont != args.Font)
			{
				args.Font = this.CustomTextFont;
			}

			if (this.ProvideFontInfo != null)
			{
				this.ProvideFontInfo(this, args);
			}
		}

		/// <summary>
		/// Raises the ContainmentChanged event.
		/// </summary>
		/// <param name="args">A ContainmentChangedEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnContainmentChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnContainmentChanged in a derived
		/// class, be sure to call the base class's OnContainmentChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnContainmentChanged(ContainmentChangedEventArgs args)
		{
			if (this.ContainmentChanged != null)
			{
				this.ContainmentChanged(this, args);
			}
		}
		/// <summary>
		/// Raises the ItemClicked event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnItemClicked method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnItemClicked in a derived
		/// class, be sure to call the base class's OnItemClicked method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnItemClicked(EventArgs args)
		{
			bool needResume = false;
			Form managerForm = null;

			if (this.Manager != null)
			{
				managerForm = this.Manager.Form;
				MessageFilterEntryHelper.Suspend(this.Manager.Form);
				needResume = true;
			}

			if (this.Manager != null)
			{
				this.Manager.OnItemClicked(new BarItemClickedEventArgs(this));
			}
			if (this.Click != null)
			{
				this.Click(this, args);
			}

			if (needResume)
			{
				MessageFilterEntryHelper.Resume(managerForm);
			}
		}
        public void OnItemMouseDown(MouseEventArgs mea)
        {
             if(this.MouseDown!=null)
                this.MouseDown(this, mea);
           
           
        }

        public void OnItemMouseUp(MouseEventArgs mea)
        {
            if (this.MouseUp != null)
                this.MouseUp(this, mea);

        }

		/// <summary>
		/// Raises the Selected event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnSelected method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnSelected in a derived
		/// class, be sure to call the base class's OnSelected method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnSelected(EventArgs args)
		{
			if (this.Enabled && this.Selected != null)
			{
				if (this.Manager != null)
					this.Manager.SelectedItem = this;
				this.Selected(this, args);
			}
		}
		/// <summary>
		/// Raises the Unselected event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnUnselected method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnUnselected in a derived
		/// class, be sure to call the base class's OnUnselected method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnUnselected(EventArgs args)
		{
			if (this.Enabled && this.Unselected != null)
			{
				if (this.Manager != null)
					this.Manager.SelectedItem = null;
				this.Unselected(this, args);
			}
		}

		protected virtual void OnShowTooltipChanged()
		{
			RaiseShowTooltipChanged();
		}

		internal void SetShortCutProcessing(bool bProcessing)
		{
			m_shortCutProcessing = bProcessing;
		}

		/// <summary>
		/// Checks whether time interval between two last mouse click is less 
		/// than SystemInformation.DoubleClickTime.
		/// </summary>
		private bool IsDoubleClickInterval()
		{
			bool result = (DateTime.Now.Ticks - m_tiksLastClick) <= SystemInformation.DoubleClickTime * DEF_DELAY_FACTOR;
			return result;
		}

		/// <summary>
		/// Check whether position of cursor satisfy double click condition.
		/// </summary>
		private bool IsDoubleClickPosition()
		{
			return m_lastClickLocation.Equals(Cursor.Position);
		}

		/// <summary>
		/// Checks whether satisfy double click condition.
		/// </summary>
		private bool IsDoubleClick()
		{
			return (!m_bDoubleClick && IsDoubleClickInterval() && IsDoubleClickPosition()) == true;
		}

		protected virtual void OnItemDoubleClicked(EventArgs args)
		{
            bool needResume = false;
            Form managerForm = null;

            if (this.Manager != null)
            {
                managerForm = this.Manager.Form;
                MessageFilterEntryHelper.Suspend(this.Manager.Form);
                needResume = true;
            }

			RaiseDoubleClick(args);

            if (needResume)
            {
                MessageFilterEntryHelper.Resume(managerForm);
            }
		}

		private void RaiseDoubleClick(EventArgs args)
		{
			if (this.DoubleClick != null)
			{
				this.DoubleClick(this, EventArgs.Empty);
			}
		}

		#endregion PROTECTED_MEMBERS

		#region CUSTOM_PROPERTIES
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
            this.barName = TypeDescriptor.GetComponentName(this, true);
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}


		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual StringCollection GetPropertiesToDisableInDesignTime()
		{
			StringCollection coll = new StringCollection();
			coll.Add("CategoryIndex");
			return coll;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);

			if (this.Manager == null && this.DesignMode)
			{
				StringCollection ignoreList = this.GetPropertiesToDisableInDesignTime();
				// Lose certain properties if in designer and BarManager not set.
				ArrayList newpds = new ArrayList();
				foreach (PropertyDescriptor pd in pdc)
				{
					if (ignoreList.Contains(pd.Name))
					{
						Attribute[] attrs = new Attribute[pd.Attributes.Count];
						int i = 0;
						foreach (Attribute a in pd.Attributes)
							attrs[i++] = a;
						newpds.Add(new MyReadOnlyPropertyDescriptor(pd, attrs));
					}
					else
						newpds.Add(pd);
				}

				PropertyDescriptor[] pds = new PropertyDescriptor[newpds.Count];
				int n = 0;
				foreach (PropertyDescriptor pd in newpds)
					pds[n++] = pd as PropertyDescriptor;

				pdc = new PropertyDescriptorCollection(pds);
			}

			return pdc;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		#endregion CUSTOM_PROPERTIES

		/// <summary>
		/// Raises the BeforePopupItemPaint event.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="Bounds"></param>
		/// <param name="Selected"></param>
		/// <param name="Element"></param>
		/// <param name="Style"></param>
		/// <returns></returns>
		protected internal bool ReiseBeforePopupItemPaint(Graphics g, Rectangle Bounds,
			ref bool Selected, DrawElement Element, Syncfusion.Windows.Forms.Grid.GridStyleInfo Style)
		{
			PopupItemPaintEventArgs e = new PopupItemPaintEventArgs(g, Bounds, Selected, Element, Style);
			this.OnBeforePopupItemPaint(e);
			Selected = e.Selected;
			return e.Handled;
		}
		/// <summary>
		/// Raises the AfterPopupItemPaint event.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="Bounds"></param>
		/// <param name="Selected"></param>
		/// <param name="Element"></param>
		/// <param name="Style"></param>
		/// <returns></returns>

		protected internal bool ReiseAfterPopupItemPaint(Graphics g, Rectangle Bounds,
			bool Selected, DrawElement Element, Syncfusion.Windows.Forms.Grid.GridStyleInfo Style)
		{
			PopupItemPaintEventArgs e = new PopupItemPaintEventArgs(g, Bounds, Selected, Element, Style);
			this.OnAfterPopupItemPaint(e);
			return e.Handled;
		}

		internal bool ReiseAfterPopupItemPaint(Graphics g, Rectangle Bounds,
			bool Selected, DrawElement Element, Syncfusion.Windows.Forms.Grid.GridStyleInfo Style, MenuComboBoxCellRenderer menuRenderer)
		{
			PopupItemPaintEventArgs2 e = new PopupItemPaintEventArgs2(g, Bounds, Selected, Element, Style, menuRenderer);
			this.OnAfterPopupItemPaint(e);
			return e.Handled;
		}

		private void RaiseShowTooltipChanged()
		{
			if (ShowTooltipChanged != null)
			{
				ShowTooltipChanged(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>
	/// Helper class for hiding implementation in derived classes.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarItemEx :
		BarItem
	{
		#region Construction

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx()
			: base()
		{
		}

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect)
			: base(mergeType, mergeOrder, shortcut, text, onClick, onPopup, onSelect)
		{
		}

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx(string text)
			: base(text)
		{
		}

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx(string text, EventHandler onClick)
			: base(text, onClick)
		{
		}

		/// <summary>
		/// See appropriate <see cref="BarItem"/> constructor.
		/// </summary>
		protected BarItemEx(string text, EventHandler onClick, Shortcut shortcut)
			: base(text, onClick, shortcut)
		{
		}

		#endregion Construction

		#region Properties

		/// <summary>
		/// Hides HandleDoubleClick property from Intellisense and property grid for derived classes.
		/// </summary>
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false)
		]
		new public bool HandleDoubleClick
		{
			get
			{
				return base.HandleDoubleClick;
			}
			set
			{
				base.HandleDoubleClick = value;
			}
		}

		#endregion Properties
	}

	// Delegate to base class for everything except IsReadOnly
	internal class MyReadOnlyPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor baseProp;
		internal MyReadOnlyPropertyDescriptor(PropertyDescriptor baseProp, Attribute[] attrArray)
			: base(baseProp.Name, attrArray)
		{
			this.baseProp = baseProp;
		}
		public override bool CanResetValue(object comp)
		{
			return baseProp.CanResetValue(comp);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return baseProp.ShouldSerializeValue(component);
		}

		public override object GetValue(object comp)
		{
			return baseProp.GetValue(comp);
		}

		public override void ResetValue(object comp)
		{
			baseProp.ResetValue(comp);
		}

		public override void SetValue(object comp, object value)
		{
			baseProp.SetValue(comp, value);
		}

		public override Type ComponentType
		{
			get
			{
				return baseProp.ComponentType;
			}
		}
		public override string DisplayName
		{
			get
			{
				return baseProp.DisplayName;
			}
		}
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}
		public override Type PropertyType
		{
			get
			{
				return baseProp.PropertyType;
			}
		}
	}

	[
	TypeConverter(
		typeof(Syncfusion.Windows.Forms.Tools.Design.BarItemsConverter)
	),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class BarItemsDesignTime : BarItems
	{
		public BarItemsDesignTime() { }
		public BarItemsDesignTime(BarItem[] barItems)
			: base(barItems)
		{
		}
	}

	/// <summary>
	/// Represents a collection of <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> objects.
	/// </summary>
	/// <remarks>
	/// This class represents a collection of <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> objects stored in a BarManager,
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> or <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPToolBar"/>.
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	Editor(
		typeof(Syncfusion.Windows.Forms.Tools.Design.BarItemsCollectionEditor),
		typeof(UITypeEditor))]
	public class BarItems :
		VisuallyInheritableList,
		IDisposable
	{
		private BarManager barManager;
		// For performance reasons, having a reverse hash.

		private Hashtable itemsByHotKey = new Hashtable();
		private Hashtable hotKeyByItems = new Hashtable();

		private Hashtable itemsByID = new Hashtable();
		private Hashtable idByItems = new Hashtable();

		private Hashtable itemsByCatAndName = new Hashtable();
		private Hashtable catAndNameByItems = new Hashtable();

        private Hashtable itemList = new Hashtable(); 

		/// <summary>
		/// Used to avoid infinite recursion on backward merges update of items collection (from MergedBar to underlying Bar).
		/// </summary>
		private int m_nMergeRecursionSpinLock = 0;

		/// <summary>
		/// Overloaded. The constructor that instantiates a BarItems class.
		/// </summary>
		public BarItems()
		{
		}
		/// <summary>
		/// The constructor that instantiates a BarItems class and intializes it with
		/// the supplied array of BarItems.
		/// </summary>
		/// <param name="barItems">An array of BarItem(s).</param>
		public BarItems(BarItem[] barItems)
			: base(barItems)
		{
		}
		/// <summary>
		/// The constructor that instantiates a BarItems class and sets its BarManager to
		/// the specified BarManager.
		/// </summary>
		/// <param name="manager">A BarManager instance.</param>
		public BarItems(BarManager manager)
			: base(manager as IDesignable)
		{
			this.barManager = manager;
		}
		internal BarItems(IDesignable parent)
			: base(parent)
		{

		}
		/// <summary>
		/// Gets or sets a reference to the BarItem at the specified index location in the
		/// BarItems object.
		/// In C#, this property is the indexer for the BarItems class.
		/// </summary>
		/// <param name="index">The location of the BarItem in the BarItems collection.</param>
		/// <value>The reference to the BarItem.</value>
		public new BarItem this[int index]
		{
			get
			{
				return (BarItem)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Returns a <see cref="BarItem"/> based on it's BarItemId.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Always),
			Documentation.DocumentationExclude(), Browsable(false)]
		public BarItem this[BarItemID barItemId]
		{
			get
			{
				return this.FindItem(barItemId.ToString());
			}
		}

        /// <summary>
        /// Returns a <see cref="BarItem"/> based on it's BarItemId string.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always),
            Documentation.DocumentationExclude(), Browsable(false)]
        public BarItem this[string barItemId]
        {
            get
            {
                return this.FindItem(barItemId);
            }
        }

		/// <summary>
		/// Adds an array of BarItem objects to the collection.
		/// </summary>
		/// <param name="items">An array of <see cref="BarItem"/> objects to add to the collection.</param>
		public virtual void AddRange(BarItem[] items)
		{
			base.AddRange(items);
		}

		/// <summary>
		/// Override. See <see cref="Syncfusion.Collections.ArrayListExt.Insert"/>.
		/// </summary>
		public override void Insert(int index, object value)
		{
			BarItem bi = value as BarItem;

			base.Insert(index, bi);

			IDesignable parent = this.Parent;

			MergeBarItem(index, bi, parent);
		}

		/// <summary>
		/// Override. See <see cref="Syncfusion.Collections.ArrayListExt.InsertRange"/>.
		/// </summary>
		public override void InsertRange(int index, ICollection c)
		{
			base.InsertRange(index, c);

			IDesignable parent = this.Parent;

			foreach (BarItem bi in c)
			{
				MergeBarItem(index, bi, parent);
			}
		}

		/// <summary>
		/// Override. See <see cref="Syncfusion.Collections.ArrayListExt.Add"/>.
		/// </summary>
		public override int Add(object value)
		{
			int nAddedAt = base.Add(value);
			BarItem barItem = value as BarItem;

			MergeBarItem(-1, barItem, this.Parent);

			return nAddedAt;
		}

		/// <summary>
		/// Merges <see cref="BarItem"/> into appropriate <see cref="MergedBar"/> and <see cref="MergedParentBarItem"/>
		/// if necessary.
		/// </summary>
		/// <param name="barItem">BarItem to merge.</param>
		private void MergeBarItem(int index, BarItem barItem, IDesignable parent)
		{
			BarManager barMan = null;

			if (null != barItem && null != parent && null != (barMan = barItem.Manager) && !barMan.Initializing)
			{
				Bar parentBar = parent as Bar;

				if (null != parentBar)
				{
					if (parentBar is MergedBar)
					{
						MergedBar mergedBar = (MergedBar)parentBar;

						if (mergedBar.CanPropagateChanges)
						{
							mergedBar.PropagateAdd(barItem);
						}
					}
					else if (!this.IsMergeRecurstionSuspended)
					{
						Bar mergedBar = barMan.GetMergedEquivalent(parentBar, null);

						if (mergedBar is MergedBar && mergedBar.Manager != parentBar.Manager)
						{
							ParentBarItem parentBarItem = barItem as ParentBarItem;

							if (null != parentBarItem)
							{
								// NOTE: if we does not create MergedBar then we catch very special issue
								// which does not required Merge logic at all. Can be reproduced in case of
								// dynamically created toolbars and MainMenu in MDI application.
								barItem = new MergedParentBarItem(
									new ParentBarItem[] { parentBarItem, new ParentBarItem() }, parentBar.Manager);

								barItem.Visible = parentBarItem.Visible;
							}

							InsertMergedBarItem(index, barItem, mergedBar.Items);
						}
					}
				}
				else if (!this.IsMergeRecurstionSuspended)
				{
					ParentBarItem parentBarItem = parent as ParentBarItem;

					if (!(parentBarItem is MergedParentBarItem))
					{
						MainFrameBarManager mainBarMan = barMan.MainFrameBarManager;

						if (null != parentBarItem && null != mainBarMan)
						{
							MergedParentBarItem[] mergedBarItems = mainBarMan.GetMergedEquivalents(parentBarItem);

							if (mergedBarItems != null && mergedBarItems.Length > 0)
							{
								foreach (MergedParentBarItem mergedBarItem in mergedBarItems)
								{
                                    if (!mergedBarItem.Items.Contains(barItem))
                                    {
                                        mergedBarItem.Items.SuspendMergeRecurstion();
                                        InsertMergedBarItem(index, barItem, mergedBarItem.Items);
                                        mergedBarItem.Items.ResumeMergeRecurstion();
                                    }
								}
							}
						}
					}
				}
			}
		}

		private static void InsertMergedBarItem(int index, BarItem barItem, BarItems mergedBarItems)
		{
			if (index < 0)
			{
				mergedBarItems.Add(barItem);
			}
			else
			{
				mergedBarItems.Insert(index, barItem);
			}
		}

		/// <summary>
		/// Tests whether a BarItem will be unique when added to this list with the
		/// specified text and categoryID. A reference to the BarItem itself is passed
		/// so that the BarItem if already in the list will be ignored in the test for uniqueness.
		/// </summary>
		/// <param name="itemToValidate">The BarItem to validate for uniqueness.</param>
		/// <param name="newID">The text value of the BarItem when it will get added to the list.</param>
		/// <returns>True if the BarItem will make a unique entry in the list; false if not.</returns>
		public bool IsValidItemID(BarItem itemToValidate, string newID)
		{
			if (newID == BarManager.SyncfusionTransientItemID)
				return true;

			if (newID == String.Empty || newID == null)
				return false;

			//			BarItemID itemToValidateID = BarItemID.Empty;
			//
			//			if(itemToValidate != null)
			//			{
			//				itemToValidateID = new BarItemID(itemToValidate.ID,
			//					itemToValidate.Manager != null ? itemToValidate.Manager.Form.GetType().FullName
			//					: this.barManager.Form.GetType().FullName);
			//			}

			foreach (BarItem item in this)
			{
				if (itemToValidate != item &&
					(itemToValidate == null || itemToValidate.Manager == item.Manager))
				{
					// Also make sure that they are not the same bar items (using their baritem ids).
					//					BarItemID itemID = new BarItemID(item.ID,
					//						item.Manager != null ? item.Manager.Form.GetType().FullName
					//						: this.barManager.Form.GetType().FullName);
					//
					//					if(itemID != itemToValidateID)
					{
						if (newID == item.ID)
							return false;
					}
				}
			}

			if (this.barManager != null)
			{
				MainFrameBarManager mainManger = this.barManager.MainFrameBarManager;
				if (mainManger != null && !mainManger.bUpdateUserChangeIn)
				{
					BarItemID id = new BarItemID(newID, BarManager.GetFormTypeName(mainManger));

					if (mainManger.customAddedItemsVsBarItemID != null &&
						mainManger.customAddedItemsVsBarItemID.Contains(id))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets or sets the BarManager to which this collection will be associated with.
		/// </summary>
		/// <remarks>
		/// The BarManager to which this BarItems collection will be associated with.
		/// <para>
		/// When this collection is associated with a BarManager, all the existing and any
		/// newly added BarItem child's Manager property will be reset to this Manager property.
		/// </para>
		/// </remarks>
		public BarManager Manager
		{
			get { return this.barManager; }
			set
			{
				if (this.barManager != value)
				{
					this.barManager = value;
					if (this.barManager != null)
					{
						foreach (BarItem item in this)
							item.Manager = this.barManager;
					}
				}
			}
		}
		// Vaidate the Text of the item to be unique within the specified category
		// Set the Manager to be the Manager of the List (if not null)
		protected override void AddHandlers(object item)
		{
			if (!(item is BarItem))
				throw new ArrayTypeMismatchException();

			BarItem barItem = (BarItem)item;

			if (this.barManager != null)
			{
				// To avoid recursion
				if (barItem.Manager != this.barManager)
					barItem.Manager = this.barManager;

				// Validate the ID of the BarItem.
				string adjustedID = barItem.ID;
				while (!this.IsValidItemID(barItem, adjustedID))
					adjustedID = IDGenerator.GetNextID(adjustedID);
				barItem.ID = adjustedID;

				this.StoreHashInfoForItem(barItem);
			}
			else
				this.StoreHashInfoForItem(barItem);

			base.AddHandlers(item);
		}
		protected override void ReleaseHandler(object item)
		{
			base.ReleaseHandler(item);
			if (item is BarItem && this.barManager != null
				&& !this.barManager.DesignMode)
			{
				BarItem barItem = (BarItem)item;
				barItem.Manager = null;
			}
			if (item is BarItem)
				this.RemoveHashInfoForItem((BarItem)item);
		}
		internal Dictionary<string, BarItem> sameCharHotKey = new Dictionary<string, BarItem>();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void StoreHashInfoForItem(BarItem item)
		{
			this.RemoveHashInfoForItem(item);

			// Update itemsByHotKey hash
			// Honor this item only if it's part of a active BarManager.
			if (item.Manager == null ||
				item.Manager.Form == null ||
				item.Manager.Form.Text != "Dummy Form")
			{
				string text = item.Text;
				int hotkeyindex = text.IndexOf('&');
				if (hotkeyindex != -1 && hotkeyindex + 1 < text.Length)
				{
					char hotKey = text[hotkeyindex + 1];
					hotKey = Char.ToLower(hotKey);
					string temp = hotKey.ToString();
					if (!(item is ExpandedListBarItem))
					{
						if (this.itemsByHotKey.ContainsKey(hotKey) || sameCharHotKey.ContainsKey(temp))
						{
							temp = temp + sameCharHotKey.Count;
						}
						if(!sameCharHotKey.ContainsValue(item))
							sameCharHotKey.Add(temp, item);
					}
					this.itemsByHotKey[hotKey] = item;
					this.hotKeyByItems[item] = hotKey;
				}
			}
			// Update itemsByID hash
			this.itemsByID[item.ID] = item;
			this.idByItems[item] = item.ID;

			// Update itemsByCatAndName hash
			string catAndName = item.CategoryIndex.ToString() + item.Text;
			this.itemsByCatAndName[catAndName] = item;
			this.catAndNameByItems[item] = catAndName;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RemoveHashInfoForItem(BarItem barItem)
		{
			// Update itemsByHotKey hash
			if (this.hotKeyByItems[barItem] != null)
			{
				char key = (char)this.hotKeyByItems[barItem];
				this.hotKeyByItems.Remove(barItem);
				// In a merged ParentBarItem scenario, this is necessary
				if (this.itemsByHotKey[key] == barItem)
					this.itemsByHotKey.Remove(key);
			}

			// Update itemsByID hash
			if (this.idByItems[barItem] != null)
			{
				string id = (string)this.idByItems[barItem];
				this.idByItems.Remove(barItem);
				this.itemsByID.Remove(id);
			}

			// Update itemsByCatAndName hash
			if (this.catAndNameByItems[barItem] != null)
			{
				string catAndName = (string)this.catAndNameByItems[barItem];
				this.catAndNameByItems.Remove(barItem);
				this.itemsByCatAndName.Remove(catAndName);
			}
		}
		/// <summary>
		/// Overloaded. Finds a <see cref="BarItem"/> in the list given it's ID.
		/// </summary>
		/// <param name="id">The BarItem's ID.</param>
		/// <returns>A <see cref="BarItem"/> with the specified ID. Null, if not found.</returns>
		public BarItem FindItem(string id)
		{
            if(id.StartsWith("\\"))
                id = id.Remove(0, 1);
			if (this.itemsByID[id] != null)
				return this.itemsByID[id] as BarItem;

			return null;
		}

		/// <summary>
		/// Finds a <see cref="BarItem"/> in the list given it's text and categoryIndex value.
		/// </summary>
		/// <param name="text">The BarItem's text value.</param>
		/// <param name="categoryIndex">The BarItem's category Index value.</param>
		/// <returns>A <see cref="BarItem"/> with the specified text and categoryIndex. Null, if not found.</returns>
		/// <remarks>
		/// If there are multiple bar items with the same text and categoryindex value, then the BarItem returned will
		/// be one of those in random.
		/// </remarks>
		public BarItem FindItem(string text, int categoryIndex)
		{
			string catAndName = categoryIndex + text;
			if (this.itemsByCatAndName[catAndName] != null)
				return this.itemsByCatAndName[catAndName] as BarItem;

			return null;
		}

        /// <summary>
        /// Finds the nested BarItem in XPToolbar.
        /// </summary>
        /// <param name="id">The baritem ID.</param>
        /// <returns>The baritem containing the id</returns>
        /// <remarks>
        /// The method can be used to iterate through all items in XPToolBar to find the item
        /// containing the mentioned ID.
        /// </remarks>
        public BarItem FindNestedItem(string id)
        {
            GetNestedItems();

            if (itemList[id] != null)
                return itemList[id] as BarItem;
          
            return null;
        }

        private void GetNestedItems()
        {
            if (this.Parent is XPToolBar)
            {
                XPToolBar toolBar = (XPToolBar)this.Parent;
                foreach (BarItem t in toolBar.Items)
                {
                    RecursiveNestedItems(t, ref itemList);
                }
            }
        }

        private void RecursiveNestedItems(BarItem current, ref Hashtable itemList)
        {
            itemList[current.ID] = current;
            if (current is ParentBarItem)
            {
                ParentBarItem pItem = (ParentBarItem)current;
                foreach (BarItem item in pItem.Items)
                {
                    RecursiveNestedItems(item, ref itemList);
                }
            }            
        }   

		/// <summary>
		/// Returns the BarItem that has this hotkey in the list, if any.
		/// </summary>
		/// <param name="hotkey">The hotkey for which to retrieve the BarItem.</param>
		/// <returns>The BarItem if found; null otherwise.</returns>
		public BarItem GetItemFromHotKey(char hotkey)
		{
			return this.itemsByHotKey[hotkey] as BarItem;
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Collections.ArrayListExt.OnItemPropertyChanged"/>.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event data.</param>
		protected override void OnItemPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			BarItem barItem = sender as BarItem;

			//			if(this.barManager != null && !this.IsValidItemID(barItem, barItem.ID))
			//				throw new Exception("Mulitple BarItem entries of the same ID " + barItem.ID + " found " + ".");

			if (e.PropertyName == "Text" || e.PropertyName == "CategoryIndex"
				|| e.PropertyName == "ID")
				this.StoreHashInfoForItem(barItem);

			base.OnItemPropertyChanged(sender, e);
		}
		// Update the item's position after its mergeOrder has changed
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateItemPositions(BarItem updatedItem)
		{
			if (updatedItem != null)
			{
				int itemIndex = this.IndexOf(updatedItem);
				if (itemIndex != -1)
				{
					int mergePos = this.FindMergePosition(updatedItem.MergeOrder);
					if (mergePos == itemIndex + 1)
						return;
					else
						this.Move(itemIndex, mergePos, 1);
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual int FindMergePosition(int mergeOrder)
		{
			// Perform binary search to find out the merge position to insert at.
			int top = 0, bottom = this.Count - 1;

			while (top < bottom)
			{
				int testIndex = (top + bottom) / 2;
				if (this[testIndex].MergeOrder > mergeOrder)
					bottom = testIndex;
				else
					top = testIndex + 1;
			}

			if (top < this.Count && this[top].MergeOrder <= mergeOrder)
				top++;

			return top;
		}

		internal void SuspendMergeRecurstion()
		{
			++m_nMergeRecursionSpinLock;
		}

		internal void ResumeMergeRecurstion()
		{
			Debug.Assert(m_nMergeRecursionSpinLock > 0, "Unpaired ResumeMergeRecurstion() is called!");

			--m_nMergeRecursionSpinLock;
		}

		internal bool IsMergeRecurstionSuspended
		{
			get
			{
				return m_nMergeRecursionSpinLock > 0;
			}
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.SuspendEvents();

			if (this.Count > 0)
			{
				BarItem item = null;
				for (int i = 0; i < this.Count; i++)
				{
					item = this[i];
					item.Dispose();
				}
			}

			this.Clear();
			this.itemsByHotKey.Clear();
			this.hotKeyByItems.Clear();
			this.itemsByID.Clear();
			this.idByItems.Clear();
			this.itemsByCatAndName.Clear();
			this.catAndNameByItems.Clear();
            this.itemList.Clear();

			this.Parent = null;
			this.barManager = null;
		}
	}
	/// <summary>
	/// Specifies the style in which the bar items will be painted.
	/// </summary>
	public enum PaintStyle
	{
		/// <summary>
		/// By default the BarItem will be painted based on what they are parented by. In a
		/// main menu, only the text of the items will be drawn; in other tool bars only
		/// the image will be drawn; in a drop-down menu, both the image and text will be drawn.
		/// </summary>
		Default,
		/// <summary>
		/// Image will be ignored in all the above cases.
		/// </summary>
		TextOnly,
		/// <summary>
		/// Image will be ignored only when the BarItem is in a drop-down menu.
		/// </summary>
		TextOnlyInMenus,
		/// <summary>
		/// Both image and text will be drawn in all the above cases.
		/// </summary>
		ImageAndText
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarItemContainer
	{
		void BeginGroupAt(BarItem barItem);
		void RemoveGroupAt(BarItem barItem);
		bool IsGroupBeginning(BarItem barItem);
		//		int SeparatorCount {get;}
		void RemoveItem(BarItem item);
		BarItems Items { get;}
		BarManager Manager { get;}
	}

	/// <summary>
	/// Specifies the appearance and behavior of a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/>.
	/// </summary>
	/// <remarks>
	/// The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> uses this enum to determine whether to act like a drop-down parent
	/// or a submenu parent.
	/// </remarks>
	public enum ParentBarItemStyle
	{
		/// <summary>
		/// The parent item will be drawn like a regular submenu item.
		/// </summary>
		Default,
		/// <summary>
		/// The parent item will be drawn like a drop-down parent. The parent item itself
		/// will be clickable and the drop-down can be dropped down by an arrow to the right.
		/// </summary>
		DropDown
	}

    /// <summary>
    /// Specifies the Text Orientation of ParentBarItem
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Specifies whether ParentBarItem text gets rendered in Vertical Orientation
        /// </summary>
        Vertical,
        /// <summary>
        /// Specifies whether ParentBarItem text gets rendered in Horizontal Orientation
        /// </summary>
        Horizontal
    }

	/// <summary>
	/// Represents the submenu that can be dropped down when part of a tool bar or another submenu in the XP Menus framework or when
	/// associated with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/>.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="Items"/> property of this class lets you add <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/>s to this ParentBarItem.</para>
	/// <para>The <see cref="MergeItems"/> function allows you to merge two ParentBarItems together.</para>
	/// <para>You can turn on partial menus behavior by setting the <see cref="UsePartialMenus"/> property to true.</para>
	/// <para>You can also make the ParentBarItem act like a Checked-ListBox by setting the
	/// <see cref="CloseOnClick"/> property to false.</para>
	/// <para>Note that when you call Dispose on the ParentBarItem object, it will not automatically dispose
	/// the BarItems in its Items collection. You will have to manually call Dispose on the children yourself, if necessary.</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[Serializable()]
	public class ParentBarItem :
		BarItemEx,
		IBarItemContainer,
		IDesignable,
		ICloneable,
		IParentBarItem,
		IDeserializationCallback,
		IIgnoreWorkingArea,
        IVisualStyle 
	{
		#region PRIVATE_MEMBERS
		internal Hashtable separators; // List of BarSeparator(s)
		private VisuallyInheritableIntList separatorList;
		protected BarItems barItems;
		private bool partialMenus = true;
		private bool popupOn = false;
		private bool closeOnClick = true;
		private ParentBarItemStyle parentStyle = ParentBarItemStyle.Default;
		private ArrayList deserializedItems;
		private VisualStyle _style = VisualStyle.OfficeXP;
		private bool _styleSet = false;
		private bool m_bSetOffice2007Theme = false;
        private bool m_bSetOffice2010Theme = false;
        /// <summary>
        /// Set BackColor of ParentBarItem
        /// </summary>
        internal Color BarItemBackColor = Color.White;
		// -1 if default, 0 for explicit false and 1 for explicit true.
		private int _uiUpdateMFCStyle = -1;
		/// <summary>
		/// Colorschemes for Office2007 visual style.
		/// </summary>
		private Office2007Theme m_enOffice2007Theme = Office2007Theme.Blue;
        /// <summary>
        /// Colorschemes for Office2010 visual style.
        /// </summary>
        private Office2010Theme m_enOffice2010Theme = Office2010Theme.Blue;
		#endregion PRIVATE_MEMBERS

		/// <summary>
		/// Overloaded. Creates a new instance of the ParentBarItem class and sets it default values.
		/// </summary>
		public ParentBarItem()
			: this(MenuMerge.Add, 0, Shortcut.None, "", null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the ParentBarItem class and sets it caption.
		/// </summary>
		/// <param name="text">The caption for this item.</param>
		public ParentBarItem(string text)
			: this(MenuMerge.Add, 0, Shortcut.None, text, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the ParentBarItem class, sets its caption and its shortcut.
		/// </summary>
		/// <param name="text">The caption for this item.</param>
		/// <param name="shortcut">The shortcut for this item.</param>
		public ParentBarItem(string text, Shortcut shortcut)
			: this(MenuMerge.Add, 0, shortcut, text, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the ParentBarItem class,
		/// sets its merge type, merge order, Shortcut, caption, Popup event handler
		/// and the Selected event handler.
		/// </summary>
		/// <param name="mergeType">The item's <see cref="MenuMerge"/>.</param>
		/// <param name="mergeOrder">The item's merge order.</param>
		/// <param name="shortcut">The item's shortcut.</param>
		/// <param name="text">The item's caption.</param>
		/// <param name="onPopup">The handler for the Popup event.</param>
		/// <param name="onSelect">The handler for the Selected event.</param>
		public ParentBarItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onPopup, EventHandler onSelect)
			: base(mergeType, mergeOrder, shortcut, text, null, onPopup, onSelect)
		{
			this.Init();
		}

		private void Init()
		{
			this.barItems = new BarItems(this);
			this.barItems.CollectionChanged += new CollectionChangeEventHandler(this.OnItemsCollectionChanged);
			this.separators = new Hashtable();
			this.separatorList = this.CreateSeparatorList();
			this.InitSeparatorList();
		}
		private VisuallyInheritableIntList CreateSeparatorList()
		{
			return new VisuallyInheritableIntList(this);
		}
		private void InitSeparatorList()
		{
			this.separatorList.CollectionChanged += new CollectionChangeEventHandler(this.SeparatorList_Changed);
		}
		private void ReleaseSeparatorList()
		{
			this.separatorList.CollectionChanged -= new CollectionChangeEventHandler(this.SeparatorList_Changed);
			this.separatorList.Parent = null;
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			ReleaseSeparatorList();

			separators.Clear();

			if (null != deserializedItems)
			{
				deserializedItems.Clear();
			}

			if (this.barItems != null)
			{
				// Do not call Dispose on the children since they could be used elsewhere.
				this.barItems.CollectionChanged -= new CollectionChangeEventHandler(this.OnItemsCollectionChanged);
				this.barItems = null;
			}

			base.Dispose(disposing);
		}

		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false),
		]
		public event SubMenuPaintEventHandler DrawBackground;

		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false)]
		protected internal virtual bool OnDrawBackground(SubMenuPaintEventArgs args)
		{
			if (this.DrawBackground != null)
			{
				this.DrawBackground(this, args);

				return true;
			}
			return false;
		}

		bool IDesignable.DesignMode
		{
			get { return this.DesignMode; }
		}

		private void MakeDirty()
		{
			if (this.Designer != null)
			{
				IAllowMakeDirty myDesigner = this.Designer as IAllowMakeDirty;
				if (myDesigner != null)
					myDesigner.SetDirty();
			}
		}

		private bool m_bScrollOnMouseMove = true;

		/// <summary>
		/// Default scrolling speed for displayed child items ( in milliseconds ).
		/// </summary>
		protected internal const int DEF_SCROLL_SPEED = 100;
        private bool m_OverlapCheckBoxImageBounds = true;
        /// <summary>
        /// Gets/Sets whether check box overlaps baritem image bounds or not
        /// </summary>
        [DefaultValue( true )]
        [Description("Gets/Sets whether check box overlaps baritem image bounds or not")]
        public bool OverlapCheckBoxImageBounds
        {
            get { return m_OverlapCheckBoxImageBounds; }
            set 
            { 
                if(m_OverlapCheckBoxImageBounds != value)
                    m_OverlapCheckBoxImageBounds = value; 
            }
        }
		private int m_scrollingSpeed = DEF_SCROLL_SPEED;

		/// <summary>
		/// Gets or Sets scrolling speed for displayed child menu items.
		/// </summary>
		[DefaultValue(100)]
		[Description("Indicates scrolling speed for displayed child menu items.")]
		public int ScrollingSpeed
		{
			get
			{
				return m_scrollingSpeed;
			}
			set
			{
				if (value != m_scrollingSpeed)
				{
					m_scrollingSpeed = value;
				}
			}
		}

		/// <summary>
		/// Gets or Sets, scroll items in menu, when mouse moves over scroll buttons.
		/// </summary>
		[DefaultValue(true)]
		[Description("Indicates, scroll child items, when mouse moves over scroll buttons or not.")]
		public bool ScrollOnMouseMove
		{
			get
			{
				return m_bScrollOnMouseMove;
			}
			set
			{
				if (value != m_bScrollOnMouseMove)
				{
					m_bScrollOnMouseMove = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether to add item to parentBarItem.
		/// </summary>
		/// <param name="value">Item to add to parentBarItem.</param>
		/// <returns>True if item can be added, otherwise false.</returns>
		internal bool ShouldAddItem(object value)
		{
			bool retVal = true;

			ParentBarItem item = value as ParentBarItem;
			if (item != null && item.Items != null)
			{
				if (item.Items.Contains(this))
				{
					retVal = false;
				}
				else
				{
					foreach (BarItem barItem in item.Items)
					{
						if (barItem is ParentBarItem)
						{
							ParentBarItem parentBarItem = barItem as ParentBarItem;
							if (parentBarItem.Items != null && parentBarItem.Items.Contains(this))
							{
								retVal = false;
								break;
							}
							else
							{
								retVal = ShouldAddItem(parentBarItem);
							}
						}
					}
				}
			}

			return retVal;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnItemsCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (this.DesignMode)
			{
				this.MakeDirty();
			}

			if (e.Action == CollectionChangeAction.Add && !ShouldAddItem(e.Element))
			{
				this.Items.Remove(e.Element);
			}

			// When there is more than 1 item then there is no need for our StandAloneBarItem
			// (the place holder inserted when there are no items)
			if (this.popupOn && this.Items.Count > 1)
			{
				foreach (BarItem item in this.Items)
				{
					if (item is StandAloneBarItem && item.Text == String.Empty)
					{
						this.Items.Remove(item);
						break;
					}
				}
			}

			// Notify this change
			BarItem barItem = e.Element as BarItem;
			if (barItem != null)
			{
				if (e.Action == CollectionChangeAction.Add)
				{
					if (barItem.Manager == null && this.Manager != null && !this.Manager.Items.Contains(barItem))
					{
						if (!this.Manager.Initializing)
						{
							this.Manager.CustomizationHelper.ParentItem = this;
						}

						barItem.Manager = this.Manager;

						if (barItem.CategoryIndex == -1)
							barItem.CategoryIndex = 0;

						if (!this.Manager.Initializing)
						{
							this.Manager.CustomizationHelper.ParentItem = null;
						}
					}

					barItem.OnContainmentChanged(new ContainmentChangedEventArgs(this, true));
				}
				else
				{
                    //if (e.Action == CollectionChangeAction.Remove)
                    //{
                    //    bool skip = false;

                    //    if (barItem == null || barItem.Manager == null || barItem.Manager.MainFrameBarManager == null
                    //        || barItem.Manager.MainFrameBarManager.Items == null || barItem.Manager.MainFrameBarManager.helper == null)
                    //    {
                    //        skip = true;
                    //    }
                        
                    //    if (!skip && !this.updatingBarItemPositions)
                    //    {
                    //        if (barItem.Manager.MainFrameBarManager.helper.ParentItem == null)
                    //            barItem.Manager.MainFrameBarManager.helper.ParentItem = this;
                    //        barItem.Manager.MainFrameBarManager.Items.Remove(barItem);
                    //    }
                    //}
					barItem.OnContainmentChanged(new ContainmentChangedEventArgs(this, false));
				}
			}

			this.UpdateSeparatorIndices();
		}

		private void SeparatorList_Changed(object sender, CollectionChangeEventArgs e)
		{
			this.ApplySeparatorList();
		}
		private void ApplySeparatorList()
		{
			// Update my separators hashtable here.
			this.separators.Clear();
			foreach (int index in this.separatorList)
			{
				if (index < this.barItems.Count)
					this.separators[this.barItems[index]] = 1;
			}
		}
		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		protected virtual void UpdateSeparatorIndices()
		{
			if (this.Items == null || this.Items.Count == 0) return;

			// We shouldn't have to check this, but seems like we are holding on
			// to a disposed ParentBarItem some where (10343) which later causes a crash.
			// So, check this for now, till we figure out the bug.
			if (this.separatorList != null)
			{
				this.separatorList.SuspendEvents();
				this.separatorList.Clear();
				int i = -1;
				foreach (BarItem item in this.Items)
				{
					if (item is SeparatorItem)
						continue;
					i++;
					if (this.IsGroupBeginning(item))
						this.separatorList.Add(i);
				}
				this.separatorList.ResumeEvents(false);
			}
		}

		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(
			typeof(Syncfusion.Windows.Forms.Tools.Design.BarManagerCollectionEditor),
			typeof(UITypeEditor)),
		Localizable(true),
		Description("Specifies the list of separator positions.")
		]
		public VisuallyInheritableIntList SeparatorIndices
		{
			// Code relies on SeparatorIndices getting called after Items in "InitializeComponent"
			// due to alphabetical order
			get { return this.separatorList; }
		}
		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		Browsable(false),
		DefaultValue(null),
		]
		public IntListDesignTime UpdatedSeparatorPositions
		{
			get { return this.separatorList.DesignTimeChanges; }
			set
			{
				this.separatorList.DesignTimeChanges = value;
			}
		}


		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
			Browsable(false),
			DefaultValue(null),
		]
		public IntListDesignTime UpdatedBarItemPositions
		{
			get
			{
				if (this.barItems != null)
					return this.barItems.DesignTimeChanges;
				else
					return null;
			}
			set
			{
                try
                {
                    // Cache the separator list
                    ArrayList cached = new ArrayList(this.separatorList);

                    this.Items.DesignTimeChanges = value;

                    // And reset the separator list:
                    this.ReleaseSeparatorList();

                    this.separatorList = this.CreateSeparatorList();
                    this.InitSeparatorList();
                    this.separatorList.AddRange(cached);
                    this.separatorList.ReinitBaseClassCopy();
                }
                finally
                {
                }
			}
		}

		/// <summary>
		/// Returns the collection of BarItem objects associated
		/// with this ParentBarItem.
		/// </summary>
		/// <remarks>
		/// A BarItems collection that represents the list of BarItem objects
		/// stored in the menu.
		/// <para>You can use this property to obtain a reference to the list of bar items
		/// that are currently stored in the ParentBarItem. With the reference to the
		/// collection of bar items for the ParentBarItem (provided by this property),
		/// you can add and remove bar items, determine the total number of bar items
		/// and clear the list of bar items from the collection.</para>
		/// <para>A ParentBarItem can be placed within a tool bar or within another
		/// ParentBarItem. It can also be associated with a PopupMenu to create context
		/// menus. All the above can be accomplished during design-time with simple drag
		/// and drop in the presence of a BarManager.</para>
		/// </remarks>
		/// <example>
		/// The following example code creates a context menu with three items and shows it.
		/// <code lang="C#">
		/// private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		/// {
		///     if(e.Button != MouseButtons.Right)
		///             return;
		///
		///     PopupMenu popup = new PopupMenu();
		///     ParentBarItem parentItem = new ParentBarItem();
		///
		///     barItem1.Checked = true;
		///     barItem1.Click += new EventHandler(ItemClicked);
		///     barItem2.Click += new EventHandler(ItemClicked);
		///
		///     barItem3.Click += new EventHandler(ItemClicked);
		///
		///     parentItem.Items.Add(barItem1);
		///     parentItem.Items.Add(barItem2);
		///     parentItem.Items.Add(barItem3);
		///
		///     popup.ParentBarItem = parentItem;
		///     popup.Show(this, new Point(e.X, e.Y));
		/// }
		/// </code>
		/// </example>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false),
		Description("The collection of child BarItems.")
		]
		public BarItems Items
		{
			get
			{
				return this.barItems;
			}
		}

		private int m_maxItemsToDisplay = int.MaxValue;

		/// <summary>
		/// Gets or Sets maximum count of child items to be displayed at one time.
		/// </summary>
		[DefaultValue(int.MaxValue)]
		[Description("Indicates, how much child items should be displayed for this ParentBarItem.")]
		public int MaximumItemsToDisplay
		{
			get
			{
				return m_maxItemsToDisplay;
			}
			set
			{
				if (value != m_maxItemsToDisplay)
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException("value");
					}

					m_maxItemsToDisplay = value;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/>.
		/// </summary>
		public override BarManager Manager
		{
			get { return base.Manager; }
			set
			{
				base.Manager = value;
				// Its very critical that we do not do this during runtime,
				// since the architecture allows the parent and its children to belong to
				// different managers and uses this functionality in mdi menu merging.
				if (this.DesignMode)
					this.barItems.Manager = base.Manager;
			}
		}
		/// <summary>
		/// Gets or sets the ParentStyle on which the menu will be drawn.
		/// </summary>
		/// <remarks>
		/// A ParentBarItemStyle value indicating the ParentStyle in which the menu
		/// will be drawn. The default value is ParentBarItemStyle.Default.
		/// <para>Take a look at the documentation for the ParentBarItemStyle enumeration
		/// for more information on the interpretation of each ParentBarItemStyle value.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(ParentBarItemStyle.Default),
		Category("Behavior"),
		Localizable(true),
		Description("Indicates the ParentBarItemStyle in which the menu will be drawn.")
		]
		public virtual ParentBarItemStyle ParentStyle
		{
			get
			{
				return this.parentStyle;
			}
			set
			{
				if (this.parentStyle != value)
				{
					this.parentStyle = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "ParentStyle", null, null));
				}
			}
		}

		/// <summary>
		/// Gets or sets the visual style of the dropdown.
		/// </summary>
		/// <value>A <see cref="VisualStyle"/> value. Default is <b>VisualStyle.OfficeXP</b>.</value>
		/// <remarks>Note that by default this value will be inherited from the <see cref="BarManager"/>
		/// that contains this ParentBarItem. If this ParentBarItem is not parented by any BarManager, then
		/// you might have to set this style manually.
		/// </remarks>
		[
		Category("Appearance"),
		Description("Specifies the Metro color for the PopupMenu.")
		]
        private Color metroColor = Color.LightSkyBlue;
        public Color MetroColor
        {
            get
            {
                 if (this.barManager != null)
					return this.barManager.MetroColor;
                return metroColor;
            }
            set
            {
                metroColor = value;
                MetroMenuPainter.GetMetroColor(metroColor);
                if (this.Style == VisualStyle.Metro)
                {
                    MetroMenuPainter.GetBarItemBackColor(this.BarItemBackColor);
                }
            }
        }

        private bool m_MultiLine = false;
        private bool m_oldMultiLine = false;
        /// <summary>
        /// Gets/Sets whether multiline function can be used.
        /// </summary>
        [
        DefaultValue(false),
		Category("Appearance"),
        Description("Specifies whether multiline function can be used in ParentBarItems")
		]
        public bool MultiLine
        {
            get
            {
                return m_MultiLine;
            }
            set
            {
                if (value != m_MultiLine)
                {
                    m_oldMultiLine = m_MultiLine;
                    m_MultiLine = value;
                    this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "MultiLine", m_oldMultiLine, m_MultiLine));
                }
            }
        }
        private bool useGDIDrawing = true;
        /// <summary>
        /// Gets/Sets whether UseGDIDrawing can be used in ParentBarItems.
        /// </summary>
        [
        DefaultValue(true),
        Category("Appearance"),
        Description("Specifies whether UseGDIDrawing can be used in ParentBarItems")
        ]
        public bool UseGDIDrawing
        {
            get
            {
                return useGDIDrawing;
            }
            set
            {
                if (value != useGDIDrawing)
                {
                    useGDIDrawing = value;
                    this.PerformUpdateUI();
                }
            }
        }
        private int wrapLength = 20;
        /// <summary>
        /// Gets/Sets the Length for text wrapping.
        /// </summary>
        [
        DefaultValue(false),
        Category("Appearance"),
        Description("Specifies the Length for text wrapping")
        ]
        public int WrapLength
        {
            get
            {
                return wrapLength;
            }
            set
            {
                if (value != wrapLength)
                {
                    wrapLength = value;
                    this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "MultiLine", m_oldMultiLine, MultiLine));
                }
            }
        }
        internal Orientation m_ParentBarItemOrientation = Orientation.Horizontal;
        internal Orientation m_OldParentBarItemOrientation = Orientation.Horizontal;
        /// <summary>
        /// Gets/Sets whether Vertical/Horizontal ParentBarItem function can be used in ParentBarItem.
        /// </summary>
        [
        DefaultValue(Orientation.Horizontal),
		Category("Appearance"),
        Description("Gets/Sets whether Vertical/Horizontal ParentBarItem function can be used in ParentBarItem")
		]
        public Orientation Orientation
        {
            get
            {
                return m_ParentBarItemOrientation;
            }
            set
            {
                if (value != m_ParentBarItemOrientation)
                {
                    m_OldParentBarItemOrientation = this.m_ParentBarItemOrientation;
                    m_ParentBarItemOrientation = value;
                    this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "ParentBarItemAlignment", m_OldParentBarItemOrientation, m_ParentBarItemOrientation));
                }
            }
        }
        

        [
		Category("Appearance"),
		Description("Specifies the visual style of the dropdown.")
		]
		public virtual VisualStyle Style
		{
			get
			{
				if (this._styleSet)
					return this._style;
				else if (this.barManager != null)
					return this.barManager.Style;
				else
					return VisualStyle.OfficeXP;
			}
			set
			{
				this._style = value;
				this._styleSet = true;
			}
		}
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                    Office2007Theme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007Theme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007Theme = Office2007Theme.Black;
                if (value == "Office2010Blue")
                    Office2010Theme = Office2010Theme.Blue;
                else if (value == "Office2010Silver")
                    Office2010Theme = Office2010Theme.Silver;
                else if (value == "Office2010Black")
                    Office2010Theme = Office2010Theme.Black;
                else if (value == "Managed")
                {
                    if(this.Style==VisualStyle.Office2010)
                        Office2010Theme = Office2010Theme.Managed;
                    else
                        Office2007Theme = Office2007Theme.Managed;
                }
            }
        }
		/// <summary>
		/// Gets or sets colorschemes for Office2007 visual style.
		/// </summary>
		[
		Description("Colorschemes for Office2007 visual style."),
		Category("Appearance"),
		]
		public Office2007Theme Office2007Theme
		{
			get
			{
				Office2007Theme theme = m_enOffice2007Theme;

				if (barManager != null)
				{
					theme = barManager.Office2007Theme;
				}

				return theme;
			}
			set
			{
				if (value != m_enOffice2007Theme)
				{
					m_enOffice2007Theme = value;

					if (barManager != null)
					{
						barManager.Office2007Theme = value;
					}

					m_bSetOffice2007Theme = true;
				}
			}
		}


		protected bool ShouldSerializeOffice2007Theme()
		{
			return m_bSetOffice2007Theme;
		}

		/// <summary>
		/// Resets the <see cref="Office2007Theme"/> property.
		/// </summary>
		public void ResetOffice2007Theme()
		{
			m_bSetOffice2007Theme = false;
			Office2007Theme = Office2007Theme.Blue;
		}

        /// <summary>
        /// Gets or sets colorschemes for Office2010 visual style.
        /// </summary>
        [
        Description("Colorschemes for Office2010 visual style."),
        Category("Appearance"),
        ]
        public Office2010Theme Office2010Theme
        {
            get
            {
                Office2010Theme theme = m_enOffice2010Theme;

                if (barManager != null)
                {
                    theme = barManager.Office2010Theme;
                }

                return theme;
            }
            set
            {
                if (value != m_enOffice2010Theme)
                {
                    m_enOffice2010Theme = value;

                    if (barManager != null)
                    {
                        barManager.Office2010Theme = value;
                    }

                    m_bSetOffice2010Theme = true;
                }
            }
        }

        protected bool ShouldSerializeParentBarItemAlignment()
        {
            return this.Orientation != XPMenus.Orientation.Horizontal;
        }

        /// <summary>
        /// Resets the <see cref="Orientation"/> property.
        /// </summary>
        public void ResetParentBarItemAlignment()
        {
            this.Orientation = XPMenus.Orientation.Vertical;
        }

        protected bool ShouldSerializeMultiLine()
        {
            return this.MultiLine != false;
        }
        protected bool ShouldSerializeUseGDIDrawing()
        {
            return UseGDIDrawing != true;
        }
        protected void ResetUseGDIDrawing()
        {
            UseGDIDrawing = true;
        }
        /// <summary>
        /// Serialize the <see cref="WrapLength"/> property.
        /// </summary>
        protected bool ShouldSerializeWrapLength()
        {
            return this.WrapLength != 20;
        }
        /// <summary>
        /// Reset the <see cref="WrapLength"/> property.
        /// </summary>
        protected void ResetWrapLength()
        {
             this.WrapLength = 20;
        }
        /// <summary>
        /// Serialize the <see cref="CheckBoxImageBounds"/> property.
        /// </summary>
        bool ShouldSerializeOverlapCheckBoxImageBounds()
        {
            return OverlapCheckBoxImageBounds != true;
        }
        /// <summary>
        /// Resets the <see cref="CheckBoxImageBounds"/> property.
        /// </summary>
        void ResetOverlapCheckBoxImageBounds()
        {
            OverlapCheckBoxImageBounds = true;
        }
        /// <summary>
        /// Resets the <see cref="MultiLine"/> property.
        /// </summary>
        public void ResetMultiLine()
        {
            this.MultiLine = false;
        }

        protected bool ShouldSerializeOffice2010Theme()
        {
            return m_bSetOffice2010Theme;
        }

        /// <summary>
        /// Resets the <see cref="Office2010Theme"/> property.
        /// </summary>
        public void ResetOffice2010Theme()
        {
            m_bSetOffice2010Theme = false;
            Office2010Theme = Office2010Theme.Blue;
        }
		protected bool ShouldSerializeStyle()
		{
			return this._styleSet;
		}
		/// <summary>
		/// Resets the <see cref="Style"/> property.
		/// </summary>
		/// <remarks>Once you reset, the <see cref="Style"/> property will then
		/// return a value based on the parent <see cref="BarManager"/>, if any.</remarks>
		public void ResetStyle()
		{
			this._styleSet = false;
			this._style = VisualStyle.OfficeXP;
		}

		/// <summary>
		/// Indicates whether the menu should close when an item is selected.
		/// </summary>
		/// <remarks>
		/// <para>True if the menu should be closed when an item is selected; false otherwise.
		/// Default value is true.</para>
		/// <para>If set to true, the menu can only be closed by clicking elsewhere or
		/// by pressing the Esc key. This setting is useful to implement, for example,
		/// a Checked-List like behavior for the drop-down menus.</para>
		/// </remarks>
		/// <example>
		/// <para>The following example code creates a Checked-List like menu with three items.
		/// The item's Checked value will be toggled as the user selects it.</para>
		/// <code lang="C#">
		/// private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		/// {
		///     PopupMenu popup = new PopupMenu();
		///     ParentBarItem parentItem = new ParentBarItem();
		///     parentItem.CloseOnClick = false;
		///
		///     barItem1.Checked = true;
		///     barItem1.Click += new EventHandler(ItemClicked);
		///     barItem2.Checked = true;
		///     barItem2.Click += new EventHandler(ItemClicked);
		///
		///     barItem3.Checked = true;
		///     barItem3.Click += new EventHandler(ItemClicked);
		///
		///     parentItem.Items.Add(barItem1);
		///     parentItem.Items.Add(barItem2);
		///     parentItem.Items.Add(barItem3);
		///
		///     popup.ParentBarItem = parentItem;
		///     popup.Show(this, new Point(e.X, e.Y));
		/// }
		/// private void ItemClicked(object sender, EventArgs e)
		/// {
		///     BarItem itemClicked = sender as BarItem;
		///     itemClicked.Checked = !itemClicked.Checked;
		/// }
		/// </code>
		/// </example>
		[DefaultValue(true),
		Category("Appearance"),
		Localizable(true),
		Description("Indicates whether the menu should close when an item is selected.")]
		public virtual bool CloseOnClick
		{
			get { return this.closeOnClick; }
			set { this.closeOnClick = value; }
		}
		/// <summary>
		/// Indicates whether the ParentBarItem will first
		/// show a list of Recently Used Items and an Expand button when dropped down.
		/// </summary>
		/// <remarks>
		/// True to turn on partial menus; false otherwise. Default is false.
		/// <para>
		/// When in partial menus mode and when this menu is dropped down, it will hide the child BarItems
		/// that has the IsRecentlyUsedItem property set to false. When the user presses the Expand button
		/// then all the child bar items will be made visible. </para>
		/// <para>
		/// In addition if this ParentBarItem is parented to a BarManager, which in turn is associated with a Form,
		/// then the user's historical menu usage pattern will be reflected in the partial menus behavior.
		/// Which means any item that the user had selected in the past 90 days (or the value set
		/// in the BarManager's RecentlyUsedItemResetDelay property) will be marked as a recently used item.</para>
		/// <para>Also, if parented to a BarManager, the BarManager's UsePartialMenus property should be true
		/// in addition to this UsePartialMenus property for partial menus mode to be turned on in this submenu.</para>
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Localizable(true),
		Description("Indicates whether the ParentBarItem will first show a list of Recently Used Items and an Expand button when dropped down.")
		]
		public virtual bool UsePartialMenus
		{
			get { return this.partialMenus; }
			set { this.partialMenus = value; }
		}

		/// <override/>
		public override bool IsRecentlyUsedItem
		{
			get
			{
				if (base.IsRecentlyUsedItem)
					return true;
				else
				{
					// If false, then override this value if any one of the children's value is set to true.
					foreach (BarItem item in this.barItems)
					{
						if (item.IsRecentlyUsedItem)
							return true;
					}
					return false;
				}
			}
			set
			{
				base.IsRecentlyUsedItem = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool ShouldDrawVisible(BarItem item)
		{
			// Do not show this if the item is a merge of items from child managers
			// and none of those child managers are active currently.
			if (item is MergedParentBarItem)
			{
				MergedParentBarItem mergedParent = item as MergedParentBarItem;
				if (!mergedParent.IsAnyChildItemManagerActive())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Indicates whether the specified key is a shortcut in any of the contained child <see cref="BarItem"/> and
		/// if so fires it's Click event.
		/// </summary>
		/// <param name="key">The shortcut key.</param>
		/// <returns>True if shortcut processed; false otherwise.</returns>
		public bool ProcessShortcut(Keys key)
		{
			BarItem item = this.FindProcessableItemWithShortcut(key);
			if (item != null && item.Visible == true && item.Enabled == true)
			{
				item.PerformClick();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Finds a visible and enabled item whose <see cref="key"/> is set to the specified key.
		/// </summary>
		/// <param name="key">The shortcut key to search.</param>
		/// <returns>A <see cref="BarItem"/> with the specified shortcut.</returns>
		/// <remarks>
		/// This method does a recursive search on all the child and <see cref="ParentBarItem"/>s as well.
		/// </remarks>
		public BarItem FindProcessableItemWithShortcut(Keys key)
		{
			BarItem foundItem = null;
			foreach (BarItem item in this.Items)
			{
				if (item is ParentBarItem)
				{
					foundItem = ((ParentBarItem)item).FindProcessableItemWithShortcut(key);
				}
				else
				{
					if (item.Visible && ((Keys)item.Shortcut) == key)
					{
						if (this.UpdateUIMFCStyle)
							item.PerformUpdateUI();

						if (item.Enabled)
							foundItem = item;
					}
				}
				if (foundItem != null)
					break;
			}
			return foundItem;
		}

		/// <summary>
		/// Indicates whether the <see cref="BarItem.UpdateUI"/> event should be fired MFC style for
		/// the bar items in this parent menu before the dropdown.
		/// </summary>
		/// <value>True to fire the UpdateUI event; false otherwise. Default is false.</value>
		/// <remarks>
		/// Take a look at the <see cref="BarItem.UpdateUI"/> event for more information
		/// on if and when you should use this pattern for your BarItem UI update.
		/// </remarks>
		[Description("Specifies whether the UpdateUI event should be fired in a MFC style fashion."),
		Category("Behavior")
		]
		public virtual bool UpdateUIMFCStyle
		{
			get
			{
				if (this._uiUpdateMFCStyle == -1)
				{
					if (this.barManager != null)
						return this.barManager.UpdateUIMFCStyle;

					return false;
				}
				else if (this._uiUpdateMFCStyle == 0)
					return false;
				else
					return true;
			}
			set
			{
				if (this.UpdateUIMFCStyle != value)
				{
					if (value == true)
						this._uiUpdateMFCStyle = 1;
					else
						this._uiUpdateMFCStyle = 0;
				}
			}
		}
		protected void ResetUpdateUIMFCStyle()
		{
			this._uiUpdateMFCStyle = -1;
		}
		protected bool ShouldSerializeUpdateUIMFCStyle()
		{
			return this._uiUpdateMFCStyle != -1;
		}

		[Documentation.DocumentationExclude]
		internal override bool HintViaHotKeyPrefix
		{
			set
			{
				if (value != base.HintViaHotKeyPrefix)
				{
					base.HintViaHotKeyPrefix = value;

					foreach (BarItem bi in this.Items)
					{
						bi.HintViaHotKeyPrefix = value;
					}
				}
			}
		}

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if (value != this.Visible)
				{
					base.Visible = value;

					if (this.barManager != null)
					{
						MergedParentBarItem merged = barManager.GetMergedEquivalent(this, null) as MergedParentBarItem;

						if (merged != null)
						{
							merged.Visible = value;
						}
					}
				}
			}
		}

		/// <summary>
		/// Shows popup for this item.
		/// </summary>
		/// <remarks>This method will not work for the items contained in XPToolBar component.
		/// Use <see cref="XPToolBar.ShowPopup"/> method instead.</remarks>
		public void ShowPopup()
		{
			if (this.Manager != null)
			{
				this.Manager.ShowPopup(this);
			}
		}

		/// <summary>
		/// Hides currently open popup.
		/// </summary>
		/// <remarks>This method will not work for the items contained in XPToolBar component.
		/// Use <see cref="XPToolBar.HidePopup"/> method instead.</remarks>
		public void HidePopup()
		{
			if (this.Manager != null)
			{
				this.Manager.HidePopup();
			}
		}

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this ParentBarItem instance.
		/// </summary>
		/// <returns>An object that has similar properties to this ParentBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of ParentBarItem and calls the <see cref="ParentBarItem.CopyTo"/> method to copy over properties.
		/// </remarks>
		public override object Clone()
		{
			ParentBarItem newItem = new ParentBarItem();
			this.CopyTo(newItem);
			return newItem;
		}

		/// <summary>
		/// Creates a clone of this ParentBarItem instance.
		/// </summary>
		/// <param name="copyClones">Indicates whether to copy clones of baritems, which are containing in <see cref="ParentBarItem"/>'s collection of <see cref="BarItem"/>s.</param>
		/// <returns>An object that has similar properties to this ParentBarItem.</returns>
		/// <remarks>
		/// Creates a new instance of ParentBarItem and calls the <see cref="ParentBarItem.CopyTo"/> method to copy over properties.
		/// </remarks>
		public object Clone(bool copyClones)
		{
			ParentBarItem newItem = new ParentBarItem();
			this.CopyTo(newItem, copyClones);
			return newItem;
		}

		/// <summary>
		/// Copies the properties of this ParentBarItem into the specified ParentBarItem.
		/// </summary>
		/// <param name="barItem">The ParentBarItem where the values should be copied to.</param>
		/// <remarks>
		/// The items list will be shallow copied over.
		/// </remarks>
		public override void CopyTo(BarItem barItem)
		{
			if (!(barItem is ParentBarItem))
				throw new ArgumentException("barItem should be of type ParentBarItem.", "barItem");

			base.CopyTo(barItem);

			ParentBarItem pbitem = barItem as ParentBarItem;

			if (!BarManager.preventCopyingItemsInContainers)
			{
				foreach (BarItem item in this.Items)
					pbitem.Items.Add(item);

				foreach (int index in this.SeparatorIndices)
					pbitem.SeparatorIndices.Add(index);
			}

			pbitem.ParentStyle = this.ParentStyle;

			pbitem.CloseOnClick = this.CloseOnClick;

			pbitem.UsePartialMenus = this.UsePartialMenus;

			if (this._styleSet)
				pbitem.Style = this.Style;
		}

		/// <summary>
		/// Copies the properties of this ParentBarItem into the specified ParentBarItem.
		/// </summary>
		/// <param name="barItem">The ParentBarItem where the values should be copied to.</param>
		/// <param name="copyClones">Indicates whether to copy clones of baritems, which are containing in <see cref="ParentBarItem"/>'s collection of <see cref="BarItem"/>s.</param>
		public void CopyTo(BarItem barItem, bool copyClones)
		{
			if (!(barItem is ParentBarItem))
				throw new ArgumentException("barItem should be of type ParentBarItem.", "barItem");

			base.CopyTo(barItem);

			ParentBarItem pbitem = barItem as ParentBarItem;

			if (!BarManager.preventCopyingItemsInContainers)
			{
				foreach (BarItem item in this.Items)
				{
					if (copyClones)
						pbitem.Items.Add(item.Clone() as BarItem);
					else
						pbitem.Items.Add(item);
				}

				foreach (int index in this.SeparatorIndices)
					pbitem.SeparatorIndices.Add(index);
			}

			pbitem.ParentStyle = this.ParentStyle;

			pbitem.CloseOnClick = this.CloseOnClick;

			pbitem.UsePartialMenus = this.UsePartialMenus;

			if (this._styleSet)
				pbitem.Style = this.Style;
		}

		#endregion

		#region SERIALIZATION
		protected ParentBarItem(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Init();
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "SeparatorIndices":
						ArrayList viil = (ArrayList)entry.Value;
						foreach (int i in viil)
							this.SeparatorIndices.Add(i);
						break;
					case "ParentStyle":
						this.ParentStyle = (ParentBarItemStyle)entry.Value;
						break;
					case "CloseOnClick":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetBool("CloseOnClick");
							this.CloseOnClick = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.CloseOnClick = (bool)entry.Value;
						break;
					case "UsePartialMenus":
						// When using SoapFormatter, the primitive types will be stored as strings
						if (entry.Value is string)
							// This is faster than calling info.GetByte("UsePartialMenus");
							this.UsePartialMenus = (bool)Convert.ChangeType(entry.Value, typeof(bool));
						else
							this.UsePartialMenus = (bool)entry.Value;
						break;
					case "Items":
						this.deserializedItems = (ArrayList)entry.Value;
						break;
					case "Style":
						this.Style = (VisualStyle)entry.Value;
						break;
				}
			}
			//			ArrayList viil = (ArrayList)info.GetValue("SeparatorIndices", typeof(ArrayList));
			//			foreach(int i in viil)
			//				this.SeparatorIndices.Add(i);
			//
			//			this.ParentStyle = (ParentBarItemStyle)info.GetValue("ParentStyle", typeof(ParentBarItemStyle));
			//			this.CloseOnClick = info.GetBoolean("CloseOnClick");
			//			this.UsePartialMenus = info.GetBoolean("UsePartialMenus");
			//
			//			ArrayList items = (ArrayList)info.GetValue("Items", typeof(ArrayList));
			//			this.deserializedItems = items;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			ArrayList temp = new ArrayList(this.SeparatorIndices);
			info.AddValue("SeparatorIndices", temp);

			info.AddValue("ParentStyle", ParentStyle);

			info.AddValue("CloseOnClick", CloseOnClick);

			info.AddValue("UsePartialMenus", this.UsePartialMenus);

			if (this._styleSet)
				info.AddValue("Style", this.Style);

			if (this.Items != null)
				temp = new ArrayList(this.Items);
			else
				temp = new ArrayList();

			info.AddValue("Items", temp);
		}
		void IDeserializationCallback.OnDeserialization(object sender)
		{
			foreach (BarItem item in this.deserializedItems)
				this.Items.Add(item);
		}
		#endregion SERIALIZATION

		#region CONTAINER_INTERFACE
		/// <summary>
		/// Lets you specify a separator in the items list. The separator will be
		/// just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		public void BeginGroupAt(BarItem barItem)
		{
			if (this.separators[barItem] == null)
			{
				this.separators[barItem] = 1;
				if (this.popupOn)
				{
					// Remove separator item from my Items collection
					int index = this.Items.IndexOf(barItem);
					if (index >= 0)
					{
						BarItem separator = new SeparatorItem();
						separator.Text = "-";
						this.Items.Insert(index, separator);
					}
				}
				this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "SeparatorIndices", null, null));
			}
			this.UpdateSeparatorIndices();
		}
		/// <summary>
		/// Removes the separator just before this BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		public void RemoveGroupAt(BarItem barItem)
		{
			if (this.separators[barItem] != null)
			{
				this.separators.Remove(barItem);
				if (this.popupOn)
				{
					// Remove separator item from my Items collection
					int index = this.Items.IndexOf(barItem);
					index--;
					if (index >= 0)
					{
						BarItem separator = this.Items[index] as BarItem;
						if (separator.Text == "-")
							this.Items.RemoveAt(index);
					}
				}
				this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "SeparatorIndices", null, null));
			}
			this.UpdateSeparatorIndices();
		}
		/// <summary>
		/// Indicates whether a separator is drawn just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		/// <returns>True if there is a separator; false if not.</returns>
		public bool IsGroupBeginning(BarItem barItem)
		{
			if (this.separators[barItem] != null)
				return true;
			else
				return false;
		}
		/// <summary>
		/// Removes an item from the Items list.
		/// </summary>
		/// <param name="item">The BarItem to remove.</param>
		public virtual void RemoveItem(BarItem item)
		{
			this.RemoveGroupAt(item);

			if (this.barItems != null)
			{
				this.barItems.Remove(item);
			}
		}

		/// <summary>
		/// Occurs just before the dropdown gets closed.
		/// </summary>
		[Description("Occurs just before the dropdown gets closed.."),
		Category("Action")]
		public event CancelEventHandler PopupClosing;

		/// <summary>
		/// Raises the PopupClosing event.
		/// </summary>
		/// <param name="e">A CancelEventArgs that contains the event data.</param>
		public virtual void OnPopupClosing(CancelEventArgs e)
		{
			if (PopupClosing != null)
			{
				PopupClosing(this, e);
			}
		}

		/// <summary>
		/// Raises the PopupClosed event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnPopupClosed method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnPopupClosed in a derived
		/// class, be sure to call the base class's OnPopupClosed method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		public virtual void OnPopupClosed(EventArgs args)
		{
			this.popupOn = false;

			if (this.Items != null)
			{
				this.Items.SuspendEvents();
				// Remove the inserted items from the list
				for (int i = this.Items.Count - 1; i >= 0; i--)
				{
					BarItem item = this.Items[i];
					if (item is StandAloneBarItem)
						this.Items.RemoveAt(i);
					if (item is ListBarItem)
					{
						ListBarItem listItem = item as ListBarItem;
						// If the list item was expanded.
						if (listItem.CurrentParent != null)
						{
							item.Visible = true;
							((ListBarItem)item).PopupClosed();
						}
					}
				}
				this.Items.ResumeEvents(!this.DesignMode);
			}
			//Throw event
			if (this.PopupClosed != null)
			{
				this.PopupClosed(this, args);
			}
		}
		/// <summary>
		/// Merges this <see cref="ParentBarItem"/> with another ParentBarItem.
		/// </summary>
		/// <param name="parentItemSrc">A <see cref="ParentBarItem"/> that is to be merged with this one.</param>
		/// <remarks>
		/// <para>Bar items with the same text value are merged according to their MergeType
		/// and MergeOrder properties.  </para>
		/// <para>Menu merging of MDI parent and MDI children is handled automatically when the
		/// toolbars and the menu items follow the merge rules. </para> <para>You can use this method to merge two
		/// <see cref="ParentBarItem"/> objects (and their submenu items) into a single ParentBarItem.  Note that the
		/// menu items in the source ParentBarItems should already be added to the BarManager that contains the destination ParentBarItem.
		/// For example, you can call this method to merge
		/// the menu items of a File and Edit ParentBarItems into a single ParentBarItem
		/// that can then be associated with and displayed by a <para>PopupMenu</para>.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">The parentItemSrc cannot be the same as this object.</exception>
		public virtual void MergeItems(ParentBarItem parentItemSrc)
		{
			if (parentItemSrc == this)
				throw new ArgumentException("Trying to Merge with Self", "parentItemSrc");
			MergeHelper.MergeItems(this, parentItemSrc);
		}
		#endregion CONTAINER_INTERFACE

		#region EVENTS
		/// <summary>
		/// Occurs before the submenu item's list of menu items is displayed.
		/// </summary>
		/// <remarks>
		/// This event only occurs when a menu item has submenu items to display.
		/// You can use this event handler to add, remove, enable, disable, check
		/// or uncheck menu items based on the state of your application before
		/// they are displayed.
		/// </remarks>
		[Description("Occurs before the submenu item's list of menu items is displayed."),
		Category("Action")]
		public event EventHandler Popup;
		/// <summary>
		/// Occurs just after the menu item has closed.
		/// </summary>
		[Description("Occurs just after the menu item has closed."),
		Category("Action")]
		public event EventHandler PopupClosed;
		/// <summary>
		/// Occurs before the submenu gets shown allowing you to cancel it.
		/// </summary>
		[Description("Occurs before the submenu gets shown allowing you to cancel it."),
		Category("Action")]
		public event CancelEventHandler BeforePopup;
		#endregion EVENTS

		#region PROTECTED_METHODS
		private void ExpandListBarItems()
		{
			if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
				return;
			int groupCount = 0;
			// Do this only when not customizing or design mode.
			for (int i = 0; i < this.Items.Count; i++)
			{
				BarItem item = this.Items[i];
				// If ListBarItem, expand
            if (this.IsGroupBeginning(item))
                groupCount++;
				if (item is ListBarItem && item.Visible)
				{
					// Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
					// and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
					//item.Visible = false;
					ListBarItem listItem = item as ListBarItem;
					listItem.OnBeforeExpand();
					int childIndex = 0;
					BarItem firstChild = null;
					BarItem lastChild = null;
					foreach (string caption in listItem.ChildCaptions)
					{
						childIndex++;
						// Insert a temp BarItem for each caption
						ExpandedListBarItem expandedItem =
							new ExpandedListBarItem(listItem, childIndex - 1);
						if (listItem.UseNumberedList)
						{
							if (childIndex < 10)
								expandedItem.Text = "&" + childIndex.ToString();
							else if (childIndex == 10)
								expandedItem.Text = "1&0";
							else
								expandedItem.Text = childIndex.ToString();

							expandedItem.Text += " " + caption;
						}
						else
							expandedItem.Text = caption;

						expandedItem.CustomTextFont = item.CustomTextFont;
						expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;
						if (listItem.CheckedIndices.Contains(childIndex - 1))
							expandedItem.Checked = true;

						if (childIndex == 1)
							firstChild = expandedItem;
						lastChild = expandedItem;
						this.Items.SuspendMergeRecurstion();
						this.Items.Insert(i + childIndex, expandedItem);
						this.Items.ResumeMergeRecurstion();

						// Add a separator to the top of this list, if the parent has a separator.
						if (childIndex == 1 && this.IsGroupBeginning(listItem))
							this.BeginGroupAt(expandedItem);
					}

					listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount , this.Items.IndexOf(lastChild) + groupCount );
				}
			}
		}
        internal void InsertListBarItems(int index,string caption)
        {
            if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
                return;
            int groupCount = 0;
            // Do this only when not customizing or design mode.
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                // If ListBarItem, expand
                if (this.IsGroupBeginning(item))
                    groupCount++;
                if (item is ListBarItem && item.Visible)
                {
                    // Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
                    // and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
                    //item.Visible = false;
                    ListBarItem listItem = item as ListBarItem;
                    listItem.OnBeforeExpand();
                    int childIndex = listItem.ChildCaptions.Count;
                    BarItem firstChild = null;
                    BarItem lastChild = null;
                    int count1 = this.Items.Count;
                    // Insert a temp BarItem for each caption
                    ExpandedListBarItem expandedItem =
                        new ExpandedListBarItem(listItem, index);

                    if (listItem.UseNumberedList)
                    {
                        if (childIndex < 10)
                            expandedItem.Text = "&" + index.ToString();
                        else if (childIndex == 10)
                            expandedItem.Text = "1&0";
                        else
                            expandedItem.Text = index.ToString();
                        expandedItem.Text += " " + caption;
                        Console.WriteLine(expandedItem.Text);
                    }
                    else
                        expandedItem.Text = caption;
                    expandedItem.CustomTextFont = item.CustomTextFont;
                    expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;

                    if (childIndex == 1)
                        firstChild = expandedItem;
                    lastChild = expandedItem;
                    this.Items.SuspendMergeRecurstion();
                    this.Items.Insert(i + index+1, expandedItem);

                    // Add a separator to the top of this list, if the parent has a separator.
                    if (childIndex == 1 && this.IsGroupBeginning(listItem))
                        this.BeginGroupAt(expandedItem);
                    listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount, this.Items.IndexOf(lastChild) + groupCount);

                    if (listItem.CheckedIndices.Contains(index))
                        expandedItem.Checked = true;
                }
            }
        }
        internal void ExpandListBarItems(string caption)
        {
            if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
                return;
            int groupCount = 0;
            // Do this only when not customizing or design mode.
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                // If ListBarItem, expand
                if (this.IsGroupBeginning(item))
                    groupCount++;
                if (item is ListBarItem && item.Visible)
                {
                    // Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
                    // and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
                    //item.Visible = false;
                    ListBarItem listItem = item as ListBarItem;
                    listItem.OnBeforeExpand();
                    int childIndex = listItem.ChildCaptions.Count ;
                    BarItem firstChild = null;
                    BarItem lastChild = null;
                    int count1 = this.Items.Count;
                        // Insert a temp BarItem for each caption
                        ExpandedListBarItem expandedItem =
                            new ExpandedListBarItem(listItem, childIndex - 1);
                        
                            if (listItem.UseNumberedList)
                            {
                                if (childIndex < 10)
                                    expandedItem.Text = "&" + childIndex.ToString();
                                else if (childIndex == 10)
                                    expandedItem.Text = "1&0";
                                else
                                    expandedItem.Text = childIndex.ToString();
                                expandedItem.Text += " " + caption;
                                Console.WriteLine(expandedItem.Text);
                            }
                            else
                                expandedItem.Text = caption;
                            expandedItem.CustomTextFont = item.CustomTextFont;
                            expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;                           
                           
                            if (childIndex == 1)
                                firstChild = expandedItem;
                            lastChild = expandedItem;
                            this.Items.SuspendMergeRecurstion();
                            this.Items.Insert(i + childIndex, expandedItem);
                    
                            // Add a separator to the top of this list, if the parent has a separator.
                            if (childIndex == 1 && this.IsGroupBeginning(listItem))
                                this.BeginGroupAt(expandedItem);
                    listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount, this.Items.IndexOf(lastChild) + groupCount);
                    if (listItem.CheckedIndices.Contains(childIndex - 1))
                        expandedItem.Checked = true;
                }
            }
        }
       
           //   internal void removeExpandListBarItems(string caption)
        internal void AddCheck(int index,string caption1)
        {
            if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
                return;
            int groupCount = 0;
            // Do this only when not customizing or design mode.
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                // If ListBarItem, expand
                if (this.IsGroupBeginning(item))
                    groupCount++;
                if (item is ListBarItem && item.Visible)
                {
                    // Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
                    // and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
                    //item.Visible = false;
                    ListBarItem listItem = item as ListBarItem;
                    listItem.OnBeforeExpand();
                    int childIndex = 0;
                    BarItem firstChild = null;
                    BarItem lastChild = null;
                    listItem.RemoveChildCaptions( index, caption1);
                    listItem.InsertChildCaptions(index, caption1);
                    foreach (string caption in listItem.ChildCaptions)
                    {
                        childIndex++;
                        // Insert a temp BarItem for each caption
                        ExpandedListBarItem expandedItem =
                            new ExpandedListBarItem(listItem, childIndex - 1);
                        if (listItem.UseNumberedList)
                        {
                            if (childIndex < 10)
                                expandedItem.Text = "&" + childIndex.ToString();
                            else if (childIndex == 10)
                                expandedItem.Text = "1&0";
                            else
                                expandedItem.Text = childIndex.ToString();

                            expandedItem.Text += " " + caption;
                        }
                        else
                            expandedItem.Text = caption;

                        expandedItem.CustomTextFont = item.CustomTextFont;
                        expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;
                        if (listItem.CheckedIndices.Contains(childIndex - 1))
                            expandedItem.Checked = true;

                        if (childIndex == 1)
                            firstChild = expandedItem;
                        lastChild = expandedItem;
                        this.Items.SuspendMergeRecurstion();
                        this.Items.ResumeMergeRecurstion();

                        // Add a separator to the top of this list, if the parent has a separator.
                        if (childIndex == 1 && this.IsGroupBeginning(listItem))
                            this.BeginGroupAt(expandedItem);
                    }

                    listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount, this.Items.IndexOf(lastChild) + groupCount);
                }
            }
        }
        internal void RemoveExpandListBarItems(string caption)
        {
            if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
                return;
            int groupCount = 0;
            // Do this only when not customizing or design mode.
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                if (item.Text == caption)
                    this.Items.RemoveAt(i);
            }
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                // If ListBarItem, expand
                if (this.IsGroupBeginning(item))
                    groupCount++;
                if (item is ListBarItem && item.Visible)
                {
                    // Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
                    // and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
                    //item.Visible = false;
                    ListBarItem listItem = item as ListBarItem;
                    listItem.OnBeforeExpand();
                    int childIndex = listItem.ChildCaptions.Count;
                    BarItem firstChild = null;
                    BarItem lastChild = null;
                    int count1 = this.Items.Count;
                    // Insert a temp BarItem for each caption
                    ExpandedListBarItem expandedItem =
                        new ExpandedListBarItem(listItem, childIndex - 1);

                    if (listItem.UseNumberedList)
                    {
                        if (childIndex < 10)
                            expandedItem.Text = "&" + childIndex.ToString();
                        else if (childIndex == 10)
                            expandedItem.Text = "1&0";
                        else
                            expandedItem.Text = childIndex.ToString();
                        expandedItem.Text += " " + caption;
                        Console.WriteLine(expandedItem.Text);
                    }
                    else
                        expandedItem.Text = caption;
                    expandedItem.CustomTextFont = item.CustomTextFont;
                    expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;
                    if (listItem.CheckedIndices.Contains(childIndex - 1))
                        expandedItem.Checked = true;
                    if (childIndex == 1)
                        firstChild = expandedItem;
                    lastChild = expandedItem;
                    this.Items.SuspendMergeRecurstion();
                    // Add a separator to the top of this list, if the parent has a separator.
                    if (childIndex == 1 && this.IsGroupBeginning(listItem))
                        this.BeginGroupAt(expandedItem);
                    listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount, this.Items.IndexOf(lastChild) + groupCount);
                }
            }
        }
        internal void RemoveListBarItems(int index,string caption)
        {
            if (this.DesignMode || (this.Manager != null && this.Manager.Customizing))
                return;
            int groupCount = 0;
            int group = 0;
           
            for (int i = 0; i < this.Items.Count; i++)
            {
                BarItem item = this.Items[i];
                if(item is ExpandedListBarItem )
                {
                    
                    if (group == index)
                        this.Items.RemoveAt(i);
                    group++;
                }
                // If ListBarItem, expand
                if (this.IsGroupBeginning(item))
                    groupCount++;
                if (item is ListBarItem && item.Visible)
                {                 

                    // Do not set it to false; because this happens twice in a merged scenario. Once for the merged parent
                    // and then for the source parent as well. Instead mark it as hidden in ShouldDrawVisible
                    //item.Visible = false;
                    ListBarItem listItem = item as ListBarItem;
                    listItem.OnBeforeExpand();
                    int childIndex = listItem.ChildCaptions.Count;
                    BarItem firstChild = null;
                    BarItem lastChild = null;
                    int count1 = this.Items.Count;
                    // Insert a temp BarItem for each caption
                    ExpandedListBarItem expandedItem =
                        new ExpandedListBarItem(listItem, childIndex - 1);

                    if (listItem.UseNumberedList)
                    {
                        if (childIndex < 10)
                            expandedItem.Text = "&" + childIndex.ToString();
                        else if (childIndex == 10)
                            expandedItem.Text = "1&0";
                        else
                            expandedItem.Text = childIndex.ToString();
                        expandedItem.Text += " " + caption;
                        Console.WriteLine(expandedItem.Text);
                    }
                    else
                        expandedItem.Text = caption;
                    expandedItem.CustomTextFont = item.CustomTextFont;
                    expandedItem.IsRecentlyUsedItem = listItem.IsRecentlyUsedItem;
                    if (listItem.CheckedIndices.Contains(childIndex - 1))
                        expandedItem.Checked = true;
                    if (childIndex == 1)
                        firstChild = expandedItem;
                    lastChild = expandedItem;
                    this.Items.SuspendMergeRecurstion();
                    // Add a separator to the top of this list, if the parent has a separator.
                    if (childIndex == 1 && this.IsGroupBeginning(listItem))
                        this.BeginGroupAt(expandedItem);
                    listItem.PostExpand(this, this.Items.IndexOf(firstChild) + groupCount, this.Items.IndexOf(lastChild) + groupCount);
                }
            }
        }
	private bool ShouldInsertDummyItem()
		{
			bool bShouldInsert = (this.Items.Count == 0);

			if (!bShouldInsert && this.Manager != null && this.Manager.Customizing)
			{
				bShouldInsert = true;
				BarItem item = null;
				for (int i = 0, len = this.Items.Count; i < len; i++)
				{
					item = this.Items[i];
					if (item != null && item.Customizable)
					{
						bShouldInsert = false;
						break;
					}
				}
			}

			return bShouldInsert;
		}
		private void InsertItemsOnPopup()
		{
			if (this.ShouldInsertDummyItem())
			{
				// A dummy item to let insert new items.
				this.barItems.Add(new StandAloneBarItem());
			}
			else
			{
				this.Items.SuspendEvents();
				this.ExpandListBarItems();
				// Inserting separator items.
				BarItem latestVisibleItem = null;
				for (int i = this.Items.Count - 1; i >= 0; i--)
				{
					BarItem barItem = this.Items[i];

					if (this.UpdateUIMFCStyle)
						barItem.PerformUpdateUI();

					bool barItemVisible = false;
					if (MenuGridControlBase.ShouldDrawVisible(this, barItem,
						this.Manager != null ? this.Manager.Customizing : false))
					{
						latestVisibleItem = barItem;
						barItemVisible = true;
					}
					if (this.separators[barItem] != null
						&& latestVisibleItem != null
						&& !(latestVisibleItem is SeparatorItem))
					{
						//if item is invisble, add separator only if this item is from the Main-manager
						if (barItemVisible
							|| barItem.Manager == null || barItem.Manager is MainFrameBarManager)
						{
							SeparatorItem separatorItem = new SeparatorItem();
							separatorItem.Text = "-";
							separatorItem.IsRecentlyUsedItem = latestVisibleItem.IsRecentlyUsedItem;
							this.Items.Insert(i, separatorItem);
							latestVisibleItem = separatorItem;
						}
					}
				}
				// If the first one is a separator, then don't show it.
				if (latestVisibleItem is SeparatorItem)
					this.Items.Remove(latestVisibleItem);
				this.Items.ResumeEvents(!this.DesignMode);
			}
		}

		/// <summary>
		/// Raises the Popup event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnPopup method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnPopup in a derived
		/// class, be sure to call the base class's OnPopup method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		public virtual void OnPopup(EventArgs args)
		{
			if (this.Popup != null)
			{
				this.Popup(this, args);
			}
			this.InsertItemsOnPopup();
			this.popupOn = true;
		}
		/// <summary>
		/// Raises the BeforePopup event.
		/// </summary>
		/// <param name="args">A CancelEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnBeforePopup method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnBeforePopup in a derived
		/// class, be sure to call the base class's OnBeforePopup method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		public virtual void OnBeforePopup(CancelEventArgs args)
		{
			if (this.BeforePopup != null)
			{
				this.BeforePopup(this, args);
			}

            // Ensures at least one item Visisble on the drop down.
            if (!(this is ToolbarListBarItem) && !this.DesignMode && !args.Cancel)
            {
                // DropDown must be allowed for customization
                if (this.Manager != null && this.Manager.Customizing)
                    return;

                args.Cancel = true;
                foreach (BarItem item in this.Items)
                {
                    if (item.Visible)
                    {
                        args.Cancel = false;
                        break;
                    }
                }
            }
		}
		#endregion PROTECTED_METHODS

		#region IIgnoreWorkingArea Members

		private bool m_bIgnoreWorkingArea = false;

		[DefaultValue(false)]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				m_bIgnoreWorkingArea = value;
			}
		}

		#endregion

		#region Implementation

		internal bool HasChild(BarItem barItem)
		{
			bool bIsChild = (this.Items.IndexOf(barItem) >= 0);

			if (!bIsChild)
			{
				foreach (BarItem bi in this.Items)
				{
					ParentBarItem pbi = bi as ParentBarItem;

					if (pbi != null && pbi.HasChild(barItem))
					{
						bIsChild = true;
						break;
					}
				}
			}

			return bIsChild;
		}

		#endregion
	}

	// This parent will be used when the parent is used just as a place holder without its own set of properties
	[Syncfusion.Documentation.DocumentationExclude()]
	public class StandAloneParentBarItem : ParentBarItem
	{
		public override BarManager Manager
		{
			get { return this.barManager; }
			set
			{
				// Unlike the base ParentBarItem this doesn't insert itself into the Manager.
				this.barManager = value;
			}
		}
		public override string ID
		{
			get { return BarManager.SyncfusionTransientItemID; }
			set { }
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class StandAloneBarItem :
		BarItemEx
	{
		public override BarManager Manager
		{
			get { return this.barManager; }
			set
			{
				// Unlike the base BarItem this doesn't insert itself into the Manager's list.
				this.barManager = value;
			}
		}
		public override string ID
		{
			get { return BarManager.SyncfusionTransientItemID; }
			set { }
		}
	}

	internal class SeparatorItem : StandAloneBarItem
	{
		public override string Text
		{
			get { return "-"; }
			set { }
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public class NewMenuItem : ParentBarItem
	{
		public override string ID
		{
			get { return BarManager.SyncfusionTransientItemID; }
			set { }
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class CustomParentMenuItem : ParentBarItem
	{
		int referenceCount = 0;
		public int RefrenceCount
		{
			get { return this.referenceCount; }
			set
			{
				this.referenceCount = value;
			}
		}
	}

	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public struct BarItemID
	{
		public string barItemID;
		public string formTypeName;
		internal BarItemID(string barItemID, string formTypeName)
		{
			this.barItemID = barItemID;
			this.formTypeName = formTypeName;
		}
		private static BarItemID emptyID;
		static BarItemID()
		{
			emptyID = new BarItemID("", "");
		}
		// Expects the item to be parented to a BarManager (otherwise throws an exception).
		public static BarItemID FromBarItem(BarItem item)
		{
			if (item.Manager == null)
				throw new ApplicationException("A BarItem not part of a BarManager's Items list is encountered.");
			return new BarItemID(item.ID, BarManager.GetFormTypeName(item.Manager));
		}

		public static BarItemID Empty
		{
			get { return emptyID; }
		}
		public static bool operator ==(BarItemID lhs, BarItemID rhs)
		{
			if ((object)lhs == null || (object)rhs == null)
				return false;

			return lhs.Equals(rhs);
		}
		public static bool operator !=(BarItemID lhs, BarItemID rhs)
		{
			if ((object)lhs == null || (object)rhs == null)
				return false;
			return !lhs.Equals(rhs);
		}
		// Required to override GetHashCode when overriding the above operators.
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public new string ToString()
		{
			return this.formTypeName + "\\" + this.barItemID;
		}
		public static bool IsItemContainedInManager(BarItemID itemId, string formTypeName)
		{
			return IsItemContainedInManager(itemId.ToString(), formTypeName);
		}
		public static bool IsItemContainedInManager(string itemIdString, string formTypeName)
		{
			if (itemIdString.IndexOf(formTypeName + "\\") == 0)
				return true;
			else
				return false;
		}

		public override bool Equals(object o)
		{
			bool eq = false;

			if ((object)this == o)
				return true;

			if (o is BarItemID)
			{
				BarItemID barItemID = (BarItemID)o;

				eq = (barItemID.barItemID == this.barItemID &&
					barItemID.formTypeName == this.formTypeName);
			}

			return eq;
		}
	}
	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public struct BarID
	{
		public string containerName;
		public string formTypeName;
		internal BarID(string barName, string formTypeName)
		{
			this.containerName = barName;
			this.formTypeName = formTypeName;
		}
		public new string ToString()
		{
			return this.formTypeName + containerName;
		}

		public static BarID FromBar(Bar bar)
		{
			if (bar.Manager == null)
				throw new ApplicationException("A Bar not part of a BarManager's Bars list is encountered.");
			return new BarID(bar.BarName, BarManager.GetFormTypeName(bar.Manager));
		}
		public static bool operator ==(BarID lhs, BarID rhs)
		{
			if ((object)lhs == null || (object)rhs == null)

				return false;

			return lhs.Equals(rhs);
		}
		public static bool operator !=(BarID lhs, BarID rhs)
		{
			if ((object)lhs == null || (object)rhs == null)
				return false;
			return !lhs.Equals(rhs);
		}
		public override bool Equals(object o)
		{
			bool eq = false;

			if ((object)this == o)
				return true;

			if (o is BarID)
			{
				BarID barID = (BarID)o;

				eq = (barID.containerName == this.containerName &&
					barID.formTypeName == this.formTypeName);
			}

			return eq;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
	/// <summary>
	/// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.Click"/> event.
	/// </summary>
	public class BarItemClickedEventArgs : EventArgs
	{
		BarItem itemClicked;
		/// <summary>
		/// Creates an instance of the BarItemClickedEventArgs class.
		/// </summary>
		/// <param name="itemClicked">The BarItem that was clicked.</param>
		public BarItemClickedEventArgs(BarItem itemClicked)
		{
			this.itemClicked = itemClicked;
		}
		/// <summary>
		/// Returns the BarItem that was just clicked.
		/// </summary>
		public BarItem ClickedBarItem
		{
			get { return this.itemClicked; }
		}
	}
	/// <summary>
	/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.ItemClicked"/> event of the BarManager component in XP Menus framework.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItemClickedEventArgs"/> that contains the event data.</param>
	public delegate void BarItemClickedEventHandler(object sender, BarItemClickedEventArgs args);

	/// <summary>
	/// The event that will be thrown when a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> gets added or removed to a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> or <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ContainmentChangedEventArgs"/> that contains the event data.</param>
	public delegate void ContainmentChangedEventHandler(object sender, ContainmentChangedEventArgs args);

	/// <summary>
	/// The event that will be thrown when a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> gets dragged over a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/>(submenu) or a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/>(Tool Bar)
	/// during user-customization in the XP Menus framework.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.CanDragDropEventArgs"/> that contains the event data.</param>
	public delegate void CanDropEventHandler(object sender, CanDragDropEventArgs args);

	/// <summary>
	/// Specifies the style in which a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/>(tool bar) will be drawn in the XP Menus framework.
	/// </summary>
	/// <remarks>
	/// <para>This enumeration has a FlagsAttribute attribute that allows a bitwise
	/// combination of its member values.</para>
	/// <para>The tool bars use this enum to specify their drawing style.</para>
	/// </remarks>
	[
	Serializable,
	Flags
	]
	public enum BarStyle
	{
		/// <summary>
		/// No Style.
		/// </summary>
		None = 0 /*0x0000*/,
		/// <summary>
		/// This will draw an arrow button to the right of the tool bar that will allow
		/// the user to drop-down a menu from which they can select hidden items, add or remove
		/// buttons and invoke the Customization dialog.
		/// </summary>
		AllowQuickCustomizing = 1 /*0x0001*/,
		/// <summary>
		/// Marks this bar as the MainMenu. Setting this flag will force the bar to occupy the
		/// whole row irrespective of the MultiLine setting and enables MainMenu like keyboard
		/// and mouse based navigation. It also shows the Minimize/Maximize/Close buttons to the right
		/// and the System Menu to the left when an MDI child window is maximized.
		/// When there are more than one bar marked as IsMainMenu, the first setting is
		/// honored and the rest are ignored.
		/// </summary>
		IsMainMenu = 2 /*0x0002*/,
		/// <summary>
		/// Wraps the bar into multiple rows when there isn't enough space in a row
		/// while docked or when the user resizes the floating bars.
		/// </summary>
		MultiLine = 4 /*0x0004*/,
		/// <summary>
		/// When this flag is set, the text in the bars will always be drawn horizontal
		/// irrespective of which border the bar is docked to. If not set, the text will be
		/// drawn vertical when the bars are docked to the left or right border.
		/// </summary>
		RotateWhenVertical = 8 /*0x0008*/,
		/// <summary>
		/// Lets you show or hide a bar.
		/// </summary>
		Visible = 16 /*0x0010*/,
		/// <summary>
		/// Forces the bar to take the whole row when docked to a form's border.
		/// </summary>
		UseWholeRow = 32 /*0x0020*/,
		/// <summary>
		/// Allows the user to drag the bar around, allowing him to move it within the
		/// dock border, dock to a different border or float it. If not set, the bar cannot be
		/// moved.
		/// </summary>
		DrawDragBorder = 64 /*0x0040*/,
		/// <summary>
		/// Marks this Bar as the status bar. This will take the Bar to take the whole row
		/// and dock to the bottom of the application. When there are more than one bar marked as IsStatusBar,
		/// the first setting is honored and the rest are ignored. You can also not specify
		/// IsMainMenu and IsStatusBar for the same Bar. The IsMainMenu setting will take precedence.
		/// </summary>
		IsStatusBar = 128 /*0x0080*/,
		/// <summary>
		/// Specifies that the text should be drawn below the image in this toolbar.
		/// </summary>
		TextBelowImage = 256 /*0x0100*/
	}
	// TODO: This not necessary.
	internal class ParentBarItemDesigner : ComponentDesigner
	{
		private DesignerVerbCollection verbs;
		static string VerbCaption = "Fallback to Inherited State";
		public ParentBarItemDesigner()
		{
		}
		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb(VerbCaption, new EventHandler(this.OnFallback)));
				}

				return this.verbs;
			}
		}
		private void OnFallback(object sender, EventArgs eevent)
		{
			ParentBarItem parentItem = this.Component as ParentBarItem;
			if (parentItem != null)
			{
				parentItem.UpdatedBarItemPositions = null;
				parentItem.UpdatedSeparatorPositions = null;
			}
		}
	}

	public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

	public class TextChangedEventArgs : EventArgs
	{
		#region Class constants
		private static readonly TextChangedEventArgs _empty = new TextChangedEventArgs();
		#endregion

		#region Static Properties
		public new static TextChangedEventArgs Empty
		{
			[DebuggerStepThrough]
			get
			{
				return _empty;
			}
		}
		#endregion

		#region Class members
		private string m_strText = null;
		#endregion

		#region Class Properties
		public string Text
		{
			get
			{
				return m_strText;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		private TextChangedEventArgs()
		{
		}

		[DebuggerStepThrough]
		public TextChangedEventArgs(string strText)
		{
			m_strText = strText;
		}
		#endregion
	}

	/// <summary>
	/// Represents and encapsulates the data required for a tool bar in the XP Menus framework.
	/// </summary>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPToolBar"/>
	/// <remarks>
	/// This class represents a tool bar's data structure.
	/// This has to be associated with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/> instance
	/// to be displayed in the associated form's command bar. This class is also used by the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPToolBar"/> class
	/// to represent a tool bar.
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	TypeConverter(
		typeof(Syncfusion.Windows.Forms.Tools.Design.BarTypeConverter)
		),
	ToolboxItem(false),
	DesignTimeVisible(false),
	Serializable()
	]
	public class Bar : Component, IChangeNotifyingItem, IBarItemContainer, IDesignable, ICloneable, ISerializable, IDeserializationCallback
	{
		#region PRIVATE_MEMBERS
		private VisuallyInheritableIntList separatorList;
		private bool suspendSeparatorListUpdate = false;
		private BarItems barItems;
		private BarStyle barStyle;
		private string barName;
		private Hashtable separators; // List of BarSeparator(s)
		internal BarManager manager;
		private bool allowCustomizing = true;
		private bool allowItemsReorderOnShrunk = true;
		private bool allowHiding = true;
		private ArrayList deserializedItems;
		private int menuItemMergeOrder = 0;

		/// <summary>
		/// Allow resize bar in floating mode.
		/// </summary>
		private bool m_bAllowResizing = true;
		#endregion PRIVATE_MEMBERS
		/// <summary>
		/// Gets or sets the BarManager this is associated with, if any.
		/// </summary>
		/// <value>An instance of the BarManager object. Can be null.</value>
		[
		Browsable(false)
		]
		public virtual BarManager Manager
		{
			get { return this.manager; }
			set
			{
				if (this.manager != value)
				{
					this.manager = value;
				}
			}
		}

		bool IDesignable.DesignMode
		{
			get { return this.DesignMode; }
		}

		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false)]
		public event PaintEventHandler DrawBackground;

		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false)]
		protected internal virtual bool OnDrawBackground(PaintEventArgs args)
		{
			if (this.DrawBackground != null)
			{
				this.DrawBackground(this, args);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Occurs when a property's value changes in this object.
		/// </summary>
		/// <remarks>
		/// This event may not be thrown for some of the properties
		/// in BarItem. Take a look at the property's documentation
		/// to confirm whether this event will be thrown for a property.
		/// </remarks>
		[Description("Occurs when a Property's value changes in this object."),
		Category("Property Changed")]
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="args">A SyncfusionPropertyChangedEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnPropertyChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Notes to Inheritors:  When overriding OnPropertyChanged in a derived
		/// class, be sure to call the base class's OnPropertyChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnPropertyChanged(SyncfusionPropertyChangedEventArgs args)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, args);
			}
		}

		/// <summary>
		/// Overloaded. Creates a new instance of the Bar.
		/// </summary>
		public Bar()
			: this(null)
		{
		}
		/// <summary>
		/// Creates a new instance of the Bar class and
		/// associates it with a BarManager.
		/// </summary>
		/// <param name="manager">A BarManager instance.</param>
		public Bar(BarManager manager)
			: this(manager, "", BarStyle.AllowQuickCustomizing | BarStyle.Visible | BarStyle.DrawDragBorder, null, null)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarManager class and
		/// associates it with a BarManager.
		/// </summary>
		/// <param name="manager">A BarManager instance.</param>
		/// <param name="separatorIndices">An integer array of indices representing the positions where separators should be introduced.</param>
		public Bar(BarManager manager, int[] separatorIndices)
			: this(manager, "", BarStyle.AllowQuickCustomizing | BarStyle.Visible | BarStyle.DrawDragBorder, null, separatorIndices)
		{
		}
		/// <summary>
		/// Creates a new instance of the BarManager class and
		/// associates it with a BarManager.
		/// </summary>
		/// <param name="manager">A BarManager instance.</param>
		/// <param name="barName">The bar's name.</param>
		public Bar(BarManager manager, string barName)
			: this(manager, barName, BarStyle.AllowQuickCustomizing | BarStyle.Visible | BarStyle.DrawDragBorder, null, null)
		{
		}

		/// <summary>
		/// Creates a new instance of the BarManager class and
		/// associates it with a BarManager.
		/// </summary>
		/// <param name="manager">A BarManager instance.</param>
		/// <param name="barName">The bar's name.</param>
		/// <param name="barStyle">The bar's style.</param>
		/// <param name="barItemsDesignTime">A list of <see cref="BarItem"/>s.</param>
		/// <param name="separatorIndices">A list of indices representing the separator positions.</param>
		public Bar(BarManager manager, string barName, BarStyle barStyle, BarItemsDesignTime barItemsDesignTime,
			int[] separatorIndices)
		{
			this.separatorList = this.CreateSeparatorList();
			this.manager = manager;
			this.barName = barName;
			this.barStyle = barStyle;
			this.barItems = new BarItems(this);

			if (barItemsDesignTime != null)
			{
				foreach (BarItem item in barItemsDesignTime)
					this.barItems.Add(item);
			}

			this.InitSeparatorList();
			this.barItems.CollectionChanged += new CollectionChangeEventHandler(this.OnItemsCollectionChanged);

			this.separators = new Hashtable();
			this.suspendSeparatorListUpdate = true;
			if (separatorIndices != null && barItems != null)
			{
				foreach (int index in separatorIndices)
				{
					if (index < barItems.Count)
						this.BeginGroupAt(barItems[index]);
				}
			}
			this.suspendSeparatorListUpdate = false;
			this.UpdateSeparatorIndices();
		}

		private VisuallyInheritableIntList CreateSeparatorList()
		{
			return new VisuallyInheritableIntList(this);
		}

		private void InitSeparatorList()
		{
			this.separatorList.CollectionChanged += new CollectionChangeEventHandler(this.SeparatorList_Changed);
		}
		private void ReleaseSeparatorList()
		{
			this.separatorList.CollectionChanged -= new CollectionChangeEventHandler(this.SeparatorList_Changed);
			this.separatorList.Parent = null;
		}
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ReleaseSeparatorList();

				if (this.barItems != null)
				{
					// Do not call Dispose on the children since they could be used elsewhere.
					this.barItems.CollectionChanged -= new CollectionChangeEventHandler(this.OnItemsCollectionChanged);
					this.barItems.Clear();
					this.barItems = null;
				}

				this.manager = null;
			}

			base.Dispose(disposing);
		}

		[Syncfusion.Documentation.DocumentationExclude(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool DesignMode
		{
			get
			{
				if (base.DesignMode)
					return base.DesignMode;
				// THis is necessary since, the Bar could sometimes be created outside
				// the scope of the designer (XPToolBar), but still this needs to treated
				// as design-time for VisuallyInheritableList to work in VI mode.
				else if (this.Items != null && this.Items.Parent != null
					&& this.Items.Parent != this)
					return this.Items.Parent.DesignMode;
				else
					return false;
			}
		}

		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		Browsable(false),
		DefaultValue(null),
		]
		public IntListDesignTime UpdatedBarItemPositions
		{
			get
			{
				IntListDesignTime positions = null;

				if (null != this.barItems)
				{
					positions = this.barItems.DesignTimeChanges;
				}

				return positions;
			}
			set
			{
				// Cache the separator list
				ArrayList cached = new ArrayList(this.separatorList);

				this.Items.DesignTimeChanges = value;

				// And reset the separator list:
				this.ReleaseSeparatorList();

				this.separatorList = this.CreateSeparatorList();
				this.InitSeparatorList();
				this.separatorList.AddRange(cached);
				this.separatorList.ReinitBaseClassCopy();
			}
		}
		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		Browsable(false),
		DefaultValue(null),
		]
		public IntListDesignTime UpdatedSeparatorPositions
		{
			get { return this.separatorList.DesignTimeChanges; }
			set
			{
				this.separatorList.DesignTimeChanges = value;
			}
		}

		private void SeparatorList_Changed(object sender, CollectionChangeEventArgs e)
		{
			this.ApplySeparatorList();
		}
		private void ApplySeparatorList()
		{
			// Update my separators hashtable here.
			this.separators.Clear();
			foreach (int index in this.separatorList)
			{
				if (index < this.barItems.Count)
					this.separators[this.barItems[index]] = 1;
			}
		}
		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		protected virtual void UpdateSeparatorIndices()
		{
			if (this.suspendSeparatorListUpdate)
				return;

			this.separatorList.SuspendEvents();
			this.separatorList.Clear();
			int i = -1;
			if (this.Items != null)
			{
				foreach (BarItem item in this.Items)
				{
					i++;
					if (this.IsGroupBeginning(item))
						this.separatorList.Add(i);
				}
			}
			this.separatorList.ResumeEvents(false);
		}
		protected virtual void OnItemsCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			this.SetDirtyOnDesigner();

			// Notify this change
			BarItem item = args.Element as BarItem;

			if (item != null)
			{
				if (args.Action == CollectionChangeAction.Add)
				{
					if (this.manager != null && !this.manager.DesignMode)
					{
						if (!this.Manager.Initializing)
						{
							this.manager.CustomizationHelper.ParentItem = this;
						}

						if (item.Manager == null && !this.manager.Items.Contains(item))
						{
							if (item.CategoryIndex == -1)
							{
								item.CategoryIndex = 0;
							}

							item.Manager = this.manager;
						}
						else
						{
							MainFrameBarManager main = item.Manager.MainFrameBarManager;

							if (main != null)
							{
								main.RecordInsertEx(item, true);
							}
						}

						if (!this.Manager.Initializing)
						{
							this.manager.CustomizationHelper.ParentItem = null;
						}

						item.OnContainmentChanged(new ContainmentChangedEventArgs(this, true));
					}
				}
				else
				{
					item.OnContainmentChanged(new ContainmentChangedEventArgs(this, false));
				}
			}
		}

		private void SetDirtyOnDesigner()
		{
			if (this.manager != null && this.manager.DesignMode)
			{
				IBarManagerDesigner designer = (this.manager.GetService(typeof(IDesignerHost)) as IDesignerHost).GetDesigner(this.manager) as IBarManagerDesigner;
				if (designer != null)
					designer.SetDirty();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool ShouldDrawVisible(BarItem item)
		{
			if (this.Manager != null)
			{
				MainFrameBarManager mainManager = this.Manager.MainFrameBarManager;
				if (mainManager != null)
				{
					bool visibility = item.Visible | mainManager.Customizing;
					if (mainManager.Customizing && !mainManager.IsItemCustomizable(item))
						visibility = false;
					else
					{
						if (!this.ShouldDrawInactiveItems())
							visibility = mainManager.ShouldDrawVisible(item);

						// Check if the user customized this visibility setting.
						if (mainManager.IsBarItemVisibilityPrefAvailable
							(item, this))
							visibility = mainManager.ShouldDrawVisibleInBar
								(item, this);
						// Do not show this if the item is a merge of items from child managers
						// and none of those child managers are active currently.
						if (visibility == true && item is MergedParentBarItem)
						{
							MergedParentBarItem mergedParent = item as MergedParentBarItem;
							if (!mergedParent.IsAnyChildItemManagerActive())
							{
								visibility = false;
							}
						}
					}
					return visibility;
				}
			}
			return item.Visible;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool ShouldDrawInactiveItems()
		{
			return true;
		}
		#region CONTAINER_INTERFACE
		/// <summary>
		/// Removes all separators between bar items in the tool bar.
		/// </summary>
		public void ClearSeparators()
		{
			this.separators.Clear();
		}

		/// <summary>
		/// Lets you specify a separator in the Items list. The separator will be
		/// just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the items list.</param>
		public void BeginGroupAt(BarItem barItem)
		{
			if (this.separators[barItem] == null)
			{
				this.separators[barItem] = 1;
				this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "SeparatorIndices", null, null));
			}
			this.UpdateSeparatorIndices();
		}

		/// <summary>
		/// Removes the separator just before this BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the items list.</param>
		public void RemoveGroupAt(BarItem barItem)
		{
			if (this.separators[barItem] != null)
			{
				this.separators.Remove(barItem);
				this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "SeparatorIndices", null, null));
			}
			this.UpdateSeparatorIndices();
		}

		/// <summary>
		/// Indicates whether a separator is drawn just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		/// <returns>True if there is a separator; false if not.</returns>
		public bool IsGroupBeginning(BarItem barItem)
		{
			if (this.separators[barItem] != null)
				return true;
			else
				return false;
		}
		[
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false),
			// Need this to prevent this property from being set (buggy behavior in VS.Net)
			// in "visually inherited" scenario.
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int SeparatorCount
		{
			get
			{
				return this.separators.Keys.Count;
			}
		}

		/// <summary>
		/// Advanced property, meant for use by the design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false),
		Localizable(true),
		]
		public VisuallyInheritableIntList SeparatorIndices
		{
			// Code relies on SeparatorIndices getting called after Items in "InitializeComponent"
			// due to alphabetical order
			get { return this.separatorList; }
		}

		/// <summary>
		/// Removes a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> from the BarItems list.
		/// </summary>
		/// <param name="item">The BarItem to remove.</param>
		public virtual void RemoveItem(BarItem item)
		{
			this.RemoveGroupAt(item);

			if (barItems != null)
			{
				this.barItems.Remove(item);
			}
		}

		/// <summary>
		/// Merges this <see cref="Bar"/> with another Bar.
		/// </summary>
		/// <param name="barSrc">A <see cref="Bar"/> that specifies
		/// the Bar to merge with this one.</param>
		/// <remarks>
		/// <para>Bar items with the same text value are merged according to their MergeType
		/// and MergeOrder properties.  </para>
		/// <para>Menu merging of MDI parent and MDI children is handled automatically when the
		/// toolbars and the menu items follow the merge rules.</para> <para>You can use this method to manually merge two
		/// <see cref="Bar"/> objects (and their submenu items) into a single Bar. Note that the
		/// menu items in the source bar should already be added to the BarManager that contains the destination bar.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">The barSrc cannot be the same as this object.</exception>
		public virtual void MergeItems(Bar barSrc)
		{
			if (barSrc == this)
				throw new ArgumentException("Trying to Merge with Self", "barSrc");
			MergeHelper.MergeItems(this, barSrc);
		}
		#endregion CONTAINER_INTERFACE

		private string m_strCaption = null;
		/// <summary>
		/// Occurs when the Caption value changes in this object.
		/// </summary>
		[Description("Occurs when the Caption value changes in this object.")]
		public event TextChangedEventHandler CaptionChanged;
		/// <summary>
		///	Raises the CaptionChanged event
		/// </summary>
		protected void RaiseCaptionChanged()
		{
			if (CaptionChanged == null) return;

			CaptionChanged(this, new TextChangedEventArgs(m_strCaption));
		}
		/// <summary>
		///  Raises the CaptionChanged event.
		/// </summary>
		protected virtual void OnCaptionChanged()
		{
			MainFrameBarManager barMan = this.manager as MainFrameBarManager;
			if (barMan != null)
			{
				barMan.RecordBarCaption(this, m_strCaption);
			}

			RaiseCaptionChanged();
		}
		/// <summary>
		/// Gets or sets user friendly bar's caption string, which appears when the tool bar floats and in the customization dialog.
		/// </summary>
		/// <remarks>
		/// This will appear as the caption when the tool bar floats and in the customization dialog as the bar's identity.
		/// </remarks>
		[
			DefaultValue(null),
			Description("Caption which appears when the tool bar floats and in the customization dialog."),
			Localizable(true)
		]
		public virtual string Caption
		{
			get
			{
				return m_strCaption;
			}
			set
			{
				if (value != m_strCaption)
				{
					m_strCaption = value;

					OnCaptionChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets the bar's name.
		/// </summary>
		/// <value>The bar's name.</value>
		/// <remarks>
		/// <para>This will appear as the caption when <see cref="Bar.Caption"/> is empty.</para>
		/// <para>The names of these different bars within the manager should be unique.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
			Category("Data"),
			Description("A unique name for the toolbar."),
			Localizable(true)
		]
		public virtual string BarName
		{
			get { return this.barName; }
			set
			{
				if (this.barName != value)
				{
					// Validate BarName uniqueness
					// If empty value then will not particiapate in Customization
					if (this.manager != null && value != "")
					{
						foreach (Bar bar in this.manager.Bars)
						{
							if (bar != this && bar.BarName == value)
							{
								MessageBox.Show(SR.GetString(SR.DuplicateNameWarning, value));
								return;
							}
						}
					}
					string oldName = this.barName;
					this.barName = value;

					if (m_strCaption == null)
						m_strCaption = barName;

					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "BarName", oldName, this.barName));
				}
			}
		}

		internal void LoadBarInfo(BarUpdateInfo updateInfo)
		{
			switch (updateInfo.updateType)
			{
				case UpdateType.ModifiedText:
					Caption = updateInfo.updateData as string;
					break;
			}
		}
		/// <summary>
		/// Gets or sets the merge order for the corresponding menu item in the toolbar list popup menu.
		/// </summary>
		/// <remarks>
		/// The items in the context menu that show up with the list of toolbars when right-clicked in the
		/// menu area will be ordered based on this setting.
		/// </remarks>
		[DefaultValue(0)]
		[Category("Behavior")]
		[Description("Specifies the merge order for the corresponding menu item in the toolbar list popup menu.")]
		public virtual int MenuItemMergeOrder
		{
			get { return this.menuItemMergeOrder; }
			set { this.menuItemMergeOrder = value; }
		}
		/// <summary>
		/// Gets or sets the bar's style.
		/// </summary>
		/// <value>One of the BarStyle values.</value>
		/// <remarks>
		/// <para>The BarStyle will be used only when this bar is associated with a BarManager. </para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(BarStyle.AllowQuickCustomizing | BarStyle.Visible | BarStyle.DrawDragBorder),
		Editor(typeof(EnumFlagsEditor), typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.Repaint),
		Browsable(true),
		Category("Behavior"),
		Description("Specifies the Bar's Style."),
		]
		public virtual BarStyle BarStyle
		{
			get { return this.barStyle; }
			set
			{
				// Force RotateWhenVertical when themes are on.
				if (this.Manager != null && this.Manager is MainFrameBarManager
					&& ((MainFrameBarManager)this.Manager).ThemesEnabled)
				{
					if ((value & BarStyle.RotateWhenVertical) <= 0)
						value |= BarStyle.RotateWhenVertical;
				}
				if (this.barStyle != value)
				{
					BarStyle oldStyle = value;
					this.barStyle = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "BarStyle", oldStyle, this.barStyle));
				}
			}
		}
		/// <summary>
		/// Indicates whether the toolbar items can be dragged out and into this bar by the user.
		/// </summary>
		/// <value>True to allow user customizing; false otherwise. Default is true.</value>
		/// <remarks>
		/// <para>Changing this property's value will fire the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Specifies whether or not toolbar items can be dragged out and into this Bar during runtime."),
		]
		public virtual bool AllowCustomizing
		{
			get { return this.allowCustomizing; }
			set
			{
				if (this.allowCustomizing != value)
				{
					bool oldAC = this.allowCustomizing;
					this.allowCustomizing = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.None, "AllowCustomizing", oldAC, this.allowCustomizing));
				}
			}
		}
		/// <summary>
		/// Indicates whether the bar items in this toolbar will be reordered
		/// when the toolbar is shrunk to show more bar items.
		/// </summary>
		/// <value>True to allow reordering; false otherwise. Default is true.</value>
		/// <remarks>
		/// <para>Changing this property's value will fire the PropertyChanged event.</para>
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Specifies if the bar items will be reordered when the toolbar is shrunk to show more bar items."),
		]
		public virtual bool AllowItemsReorderOnShrunk
		{
			get { return this.allowItemsReorderOnShrunk; }
			set
			{
				if (this.allowItemsReorderOnShrunk != value)
				{
					bool oldRO = this.allowItemsReorderOnShrunk;
					this.allowItemsReorderOnShrunk = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.None, "AllowItemsReorderOnShrunk", oldRO, this.allowItemsReorderOnShrunk));
				}
			}
		}
		/// <summary>
		/// Indicates whether this toolbar can be hidden by the user during runtime.
		/// </summary>
		/// <value>True to allow user hiding; false otherwise. Default is true.</value>
		/// <remarks>
		/// <para>Changing this property's value will fire the PropertyChanged event.</para>
		/// </remarks>
		[
		Category("Behavior"),
		Description("Specifies whether or not this toolbar can be hidden by the user during runtime."),
		]
		public virtual bool AllowHiding
		{
			get { return this.allowHiding && (this.BarStyle & BarStyle.IsMainMenu) <= 0; }
			set
			{
				if (this.allowHiding != value)
				{
					bool oldAH = this.allowHiding;
					this.allowHiding = value;
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.None, "AllowHiding", oldAH, this.allowHiding));
				}
			}
		}
		protected void ResetAllowHiding()
		{
			this.allowHiding = true;
		}
		protected bool ShouldSerializeAllowHiding()
		{
			// If main-menu and allowHiding is not modified
			if ((this.BarStyle & BarStyle.IsMainMenu) > 0 && this.allowHiding)
				return false;

			if (this.allowHiding)
				return false;

			return true;
		}
		/// <summary>
		/// Returns the BarItems list.
		/// </summary>
		/// <value>The BarItems collection.</value>
		/// <remarks>
		/// <para>You can add, remove and insert BarItems into this
		/// collection.</para>
		/// <para>Changing this property's value will throw the PropertyChanged event.</para>
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CustDlgEditor), typeof(System.Drawing.Design.UITypeEditor)),
		Category("Data"),
		Description("Collection of BarItems, representing the menu items in the toolbar."),
		]
		public virtual BarItems Items
		{
			get { return this.barItems; }
		}

		/// <summary>
		/// Gets or sets allow resize bar in floating mode.
		/// </summary>
		[
		Category("Behavior"),
		Description("Gets or sets allow resize bar in floating mode."),
		DefaultValue(true)
		]
		public bool AllowResizing
		{
			get
			{
				return m_bAllowResizing;
			}
			set
			{
				if (value != m_bAllowResizing)
				{
					m_bAllowResizing = value;
				}
			}
		}

		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this bar instance.
		/// </summary>
		/// <returns>An object that has similar properties to this bar.</returns>
		/// <remarks>
		/// Creates a new instance of bar and calls the <see cref="Bar.CopyTo"/> method to copy over properties.
		/// </remarks>
		public object Clone()
		{
            BarManager.DesignerBarClone = true;
			Bar newBar = new Bar();
			this.CopyTo(newBar);
			return newBar;
		}

		/// <summary>
		/// Copies the properties of this bar into the specified bar.
		/// </summary>
		/// <param name="bar">The bar where the values should be copied to.</param>
		/// <remarks>
		/// The items list will be shallow copied.
		/// </remarks>
		public void CopyTo(Bar bar)
		{
			if (!BarManager.preventCopyingItemsInContainers)
			{
				foreach (BarItem item in this.Items)
					bar.Items.Add(item);

				foreach (int index in this.SeparatorIndices)
					bar.SeparatorIndices.Add(index);
			}

			bar.BarName = this.BarName;
			bar.BarStyle = this.BarStyle;
			bar.MenuItemMergeOrder = this.MenuItemMergeOrder;
			bar.Caption = this.Caption;
		}

		/// <summary>
		/// Creates a clone of this bar instance.
		/// </summary>
		/// <param name="copyClones">Indicates whether to copy clones of baritems, which are containing in <see cref="Bar"/>'s collection of <see cref="BarItem"/>s.</param>
		/// <returns>An object that has similar properties to this bar.</returns>
		/// <remarks>
		/// Creates a new instance of bar and calls the <see cref="Bar.CopyTo"/> method to copy over properties.
		/// </remarks>
		public object Clone(bool copyClones)
		{
			Bar newBar = new Bar();
			this.CopyTo(newBar, copyClones);
			return newBar;
		}

		/// <summary>
		/// Copies the properties of this bar into the specified bar.
		/// </summary>
		/// <param name="bar">The bar where the values should be copied to.</param>
		/// <param name="copyClones">Indicates whether to copy clones of baritems, which are containing in <see cref="Bar"/>'s collection of <see cref="BarItem"/>s.</param>
		/// <remarks>
		/// The items list will be shallow copied.
		/// </remarks>
		public void CopyTo(Bar bar, bool copyClones)
		{
			if (!BarManager.preventCopyingItemsInContainers)
			{
				foreach (BarItem item in this.Items)
				{
					if (copyClones)
						bar.Items.Add(item.Clone() as BarItem);
					else
						bar.Items.Add(item);
				}

				foreach (int index in this.SeparatorIndices)
					bar.SeparatorIndices.Add(index);
			}

			bar.BarName = this.BarName;
			bar.BarStyle = this.BarStyle;
			bar.MenuItemMergeOrder = this.MenuItemMergeOrder;
		}
		#endregion
		protected Bar(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach (SerializationEntry entry in info)
			{
				switch (entry.Name)
				{
					case "SeparatorIndices":
						ArrayList viil = (ArrayList)entry.Value;
						foreach (int i in viil)
							this.SeparatorIndices.Add(i);
						break;

					case "BarName":
						this.BarName = (string)entry.Value;
						break;
					case "BarStyle":
						this.BarStyle = (BarStyle)entry.Value;
						break;
					case "Items":
						this.deserializedItems = (ArrayList)info.GetValue("Items", typeof(ArrayList));
						break;
				}
			}
			//			ArrayList viil = (ArrayList)info.GetValue("SeparatorIndices", typeof(ArrayList));
			//			foreach(int i in viil)
			//				this.SeparatorIndices.Add(i);
			//			this.BarName = info.GetString("BarName");
			//			this.BarStyle = (BarStyle)info.GetValue("BarStyle", typeof(BarStyle));
			//
			//			ArrayList items = (ArrayList)info.GetValue("Items", typeof(ArrayList));
			//			this.deserializedItems = items;
		}
		void IDeserializationCallback.OnDeserialization(object sender)
		{
			foreach (BarItem item in this.deserializedItems)
				this.Items.Add(item);
		}
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ArrayList temp = new ArrayList(this.SeparatorIndices);
			info.AddValue("SeparatorIndices", temp);
			info.AddValue("BarName", this.BarName);
			info.AddValue("BarStyle", this.BarStyle);
			temp = new ArrayList(this.Items);
			info.AddValue("Items", temp);
		}
	}
	internal class IDGenerator
	{
		/// <summary>
		/// Generates the new ID based on the provided ID.
		/// </summary>
		/// <param name="currentID">The ID based on which to generate the new ID.</param>
		/// <returns>The new ID.</returns>
		/// <remarks>
		/// If "File" is passed the new ID will be "File_1". If "File_1" is passed, the new ID will be "File_2", etc.
		/// </remarks>
		public static string GetNextID(string currentID)
		{
			string[] substrings = currentID.Split('_');

			if (substrings.Length > 1)
			{
				string idcount = substrings[substrings.Length - 1];
				try
				{
					int iCount = Int32.Parse(idcount);
					iCount++;
					int lastIndex = currentID.LastIndexOf('_');
					string baseString = currentID.Substring(0, lastIndex);
					return baseString + "_" + iCount.ToString();
				}
				catch { }
			}

			return currentID + "_1";
		}

		/// <summary>
		/// Indicates whether the specified ID could have been generated automatically.
		/// </summary>
		/// <param name="ID">The ID to analyze.</param>
		/// <returns>True if possibly auto generated; false otherwise.</returns>
		public static bool IsAutoGeneratedID(string ID)
		{
			if (ID.Length < 2)
				return false;

			if (ID[0] == '_')
			{
				string intPortion = ID.Substring(1);
				int iVal = -1;
				try
				{
					iVal = Int32.Parse(intPortion);
				}
				catch { }
				if (iVal != -1)
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.InitListBox"/> event.
	/// </summary>
	public delegate void ComboBoxBarItemInitListBoxEventHandler(object sender, ComboBoxBarItemInitListBoxEventArgs args);

	/// <summary>
	/// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.InitListBox"/> event.
	/// </summary>
	public class ComboBoxBarItemInitListBoxEventArgs : EventArgs
	{
		ListBox listBox;
		public ComboBoxBarItemInitListBoxEventArgs(ListBox listBox)
		{
			this.listBox = listBox;
		}
		public ListBox ListBox
		{
			get { return this.listBox; }
		}
	}

	/// <summary>
	/// Delegate for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.TextBoxValueChange"/> event.
	/// </summary>
	/// <remarks>
	/// See the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ComboBoxBarItem.TextBoxValueChange"/> event for more information.
	/// </remarks>
	public delegate void TextBoxValueChangeEventHandler(object sender, TextBoxValueChangeEventArgs args);

	/// <summary>
	/// Provides data for the TextBoxValueChange event.
	/// </summary>
	public class TextBoxValueChangeEventArgs : CancelEventArgs
	{
		private string m_newValue;

		public TextBoxValueChangeEventArgs(string newValue)
		{
			this.m_newValue = newValue;
		}

		public string NewValue
		{
			get
			{
				return this.m_newValue;
			}
			set
			{
				m_newValue = value;
			}
		}
	}

	/// <summary>
	/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.ProvideFontInfo"/> event.
	/// </summary>
	public delegate void ProvideFontInfoEventHandler(object sender, ProvideFontInfoEventArgs args);

	/// <summary>
	/// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem.ProvideFontInfo"/> event.
	/// </summary>
	public class ProvideFontInfoEventArgs : EventArgs
	{
		private Font font;
		public ProvideFontInfoEventArgs(Font font)
		{
			this.font = font;
		}

		/// <summary>
		/// Gets or sets the corresponding Font object.
		/// </summary>
		/// <remarks>The Font object returned will be font that will be used by default. </remarks>
		public Font Font
		{
			get { return this.font; }
			set
			{
				if (value == null)
					throw new ArgumentException("Font property cannot be set to null in the ProvideFontInfoEventArgs.");
				this.font = value;
			}
		}
	}

	public delegate void PopupItemPaintEventHandler(object sender, PopupItemPaintEventArgs drawItemInfo);

	public class PopupItemPaintEventArgs : EventArgs
	{
		private Graphics m_graphics;
		private Rectangle m_bounds;
		private bool m_selected;
		private DrawElement m_element;
		private bool m_handled = false;
		private Syncfusion.Windows.Forms.Grid.GridStyleInfo m_style;

		public PopupItemPaintEventArgs(Graphics g, Rectangle bounds, bool selected, DrawElement element, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
			: base()
		{
			this.m_graphics = g;
			this.m_bounds = bounds;
			this.m_selected = selected;
			this.m_element = element;
			this.m_style = style;
		}

		public Graphics Graphics
		{
			get
			{
				return m_graphics;
			}
		}

		virtual public Rectangle Bounds
		{
			get
			{
				return m_bounds;
			}
		}

		public bool Selected
		{
			get
			{
				return m_selected;
			}
			set
			{
				m_selected = value;
			}
		}

		public DrawElement Element
		{
			get
			{
				return m_element;
			}
		}

		public bool Handled
		{
			get
			{
				return m_handled;
			}
			set
			{
				m_handled = value;
			}
		}

		public Syncfusion.Windows.Forms.Grid.GridStyleInfo Style
		{
			get
			{
				return m_style;
			}
		}
	}

	internal class PopupItemPaintEventArgs2 :
		PopupItemPaintEventArgs
	{
		MenuComboBoxCellRenderer m_menuRenderer;

		public PopupItemPaintEventArgs2(Graphics g, Rectangle bounds, bool selected, DrawElement element, Syncfusion.Windows.Forms.Grid.GridStyleInfo style, MenuComboBoxCellRenderer menuRenderer)
			: base(g, bounds, selected, element, style)
		{
			m_menuRenderer = menuRenderer;
		}

		public override Rectangle Bounds
		{
			get
			{
				Rectangle bounds = base.Bounds;

				bounds.Width -= m_menuRenderer.DropDownButton.Bounds.Width + this.Style.TextMargins.Right;

				return bounds;
			}
		}
	}

	public enum DrawElement
	{
		CheckMark,
		Icon,
		ComboBox,
		EditableCombo,
		Glyph,
		Text,
		Separator,
		Shortcut,
		TextBox
	}

	/// <summary>
	/// Specifies the text alignment of the bar.
	/// </summary>
	public enum TextAlignment
	{
		/// <summary>
		/// Specifies that the contents of a text are aligned with the near.
		/// </summary>
		Near = 0,
		/// <summary>
		/// Specifies that the contents of a text are aligned with the far.
		/// </summary>
		Far = 1,
		/// <summary>
		/// Specifies that the contents of a text are aligned with the center.
		/// </summary>
		Center = 2
	}

	#region BarItemImageIndexEditor
	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarItemImageIndexEditor : UITypeEditor
	{
		#region Constructor
		static BarItemImageIndexEditor()
		{
			m_imageEditor = TypeDescriptor.GetEditor(typeof(System.Drawing.Image), typeof(UITypeEditor)) as UITypeEditor;
		}
		#endregion

		#region Properties
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			if (m_imageEditor != null)
			{
				return m_imageEditor.GetPaintValueSupported(context);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public override void PaintValue(PaintValueEventArgs e)
		{
			if (m_imageEditor != null)
			{
				ITypeDescriptorContext context = e.Context;
				if (context != null)
				{
					BarItem item = context.Instance as BarItem;
					if (item != null)
					{
						Image image = null;

						int index = (int)e.Value;
						if(index>=0)
						{
							IList images = GetImages(item);
							
							if (images != null && index < images.Count)
							{
								image = images[index] as Image;
							}
						}
						if (image != null)
						{
							m_imageEditor.PaintValue(new PaintValueEventArgs(context, image, e.Graphics, e.Bounds));
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion

		#region Fields
		static UITypeEditor m_imageEditor;
		#endregion
	}
	#endregion

	#region HighlightedImageIndexEditor
	[Syncfusion.Documentation.DocumentationExclude()]
	public class HighlightedImageIndexEditor : BarItemImageIndexEditor
	{
		#region Overrides
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetHighlightImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region PressedImageIndexEditor
	[Syncfusion.Documentation.DocumentationExclude()]
	public class PressedImageIndexEditor : BarItemImageIndexEditor
	{
		#region Overrides
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetPressedImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region DisabledImageIndexEditor
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DisabledImageIndexEditor : BarItemImageIndexEditor
	{
		#region Overrides
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetDisabledImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region BarItemImageIndexConverter
	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarItemImageIndexConverter : ImageIndexConverter
	{
		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			IList images = GetImages(context);
			
			ArrayList stdValues = new ArrayList();

			if (images != null)
			{
				int nValuesCount = images.Count;
				for (int i = 0; i < nValuesCount; i++)
				{
					stdValues.Add(i);
				}

				if (this.IncludeNoneAsStandardValue)
				{
					stdValues.Add(-1);
				}

			}
			else
			{
				stdValues.Add(this.IncludeNoneAsStandardValue ? -1 : 0);
			}

			return new TypeConverter.StandardValuesCollection(stdValues);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IList GetImages(ITypeDescriptorContext context)
		{
			IList images = null;

			if (context != null)
			{
				BarItem item = context.Instance as BarItem;
				
				if (item != null)
				{
					images = GetImages(item);
				}
			}

			return images;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region HighlightedImageIndexConverter
	[Syncfusion.Documentation.DocumentationExclude()]
	public class HighlightedImageIndexConverter : BarItemImageIndexConverter
	{
		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetHighlightImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region PressedImageIndexConverter
	[Syncfusion.Documentation.DocumentationExclude()]
	public class PressedImageIndexConverter : BarItemImageIndexConverter
	{
		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetPressedImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion

	#region DisabledImageIndexConverter
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DisabledImageIndexConverter : BarItemImageIndexConverter
	{
		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override IList GetImages(BarItem item)
		{
			BarManager manager = item.Manager;

			return item.GetDisabledImageListInternal(manager != null && manager.LargeIcons);
		}
		#endregion
	}
	#endregion
}
