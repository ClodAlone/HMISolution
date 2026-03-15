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

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using Syncfusion.Core.Licensing;
using Syncfusion.Diagnostics;
using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Tools.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	#region AutoCompleteStyle

	public enum AutoCompleteStyle
	{
		Default,
		Metro
	}

	#endregion

	/// <summary>
	/// The AutoComplete class provides auto completion capabilities for
	/// edit controls (<see cref="System.Windows.Forms.TextBox"/>
	/// ,<see cref="System.Windows.Forms.ComboBox"/>
	///  and <see cref="Syncfusion.Windows.Forms.IEditControlsEmbed"/> based controls).
	/// </summary>
	/// <remarks><para>
	/// AutoCompletion is the process in which a program prompts the user with
	/// helpful completion suggestions when the user inputs some text through
	/// a edit control (TextBox or ComboBox controls). The address bar in Microsoft
	/// Internet Explorer provides this feature when users type in part of a
	/// web address.
	/// </para><para>
	/// The AutoComplete control is implemented as an extender control that provides an
	/// "AutoComplete" extender property to edit control type controls.
	/// </para><para>
	/// The AutoComplete control can operate in <see cref="AutoCompleteModes.AutoSuggest"/>
	/// or <see cref="AutoCompleteModes.AutoAppend"/> Mode.
	/// In AutoSuggest mode a drop down list of possible matches for the current
	/// text will be displayed and the user can select the appropriate entry they
	/// want. In AutoAppend mode the edit control with the focus will be filled
	/// with the possible match and the user can continue typing to change the
	/// current suggestion or accept the text as it is.
	/// </para><para>
	/// The AutoComplete control can use a internal list of 'history
	/// items' to provide the auto completion to its target controls or it can take
	/// an external data source as the source for the auto completion items.
	/// </para><para>
	/// The <seealso cref="Columns"/> property can be used to customize the appearance of the drop down list of items.
	/// </para></remarks>
	/// <example><coderef file="tools\samples\editors package\autocompleteDemo\cs\MainForm.cs" name="AutoComplete Initialization" lang="C#"><code lang="C#">
	///             // This code shows how the AutoComplete control
	///             // is initialized and used
	///             this.autoComplete1 = new AutoComplete();
	///             this.textBox1 = new TextBox();
	///             // Set buttonEdit1 properties
	///             this.autoComplete1.AdjustHeightToItemCount = true;
	///             // The AutoComplete control will add contents of the target control to
	///             // its history list automatically (when the enter key is hit)
	///             this.autoComplete1.AutoAddItem = true;
	///             // The AutoComplete control will maintain its history items
	///             this.autoComplete1.AutoSerialize = true;
	///             // The category under which the history list will be persisted
	///             this.autoComplete1.CategoryName = "websites";
	///             // Specifies if any of the columns in the data source are to be treated as
	///             // image index
	///             this.autoComplete1.ImageColumnIndex = -1;
	///             // The image list to be used as the image source
	///             this.autoComplete1.ImageList = null;
	///             // The preferred height for the drop down window
	///             this.autoComplete1.PreferredHeight = 200;
	///             // Add a event to customize data added to history
	///             this.autoComplete1.BeforeAddItem += new AutoCompleteAddItemCancelEventHandler(this.autoComplete1_BeforeAddItem);
	///             // Set Auto Complete
	///             // properties for target TextBox
	///             this.autoComplete1.SetAutoComplete(this.textBox1, AutoCompleteMode.Both);
	///             // Added by designer - Add the TextBox to the Form
	///             this.Controls.Add(textBox1);
	/// 	</code></coderef><coderef file="tools\samples\editors package\autocompleteDemo\vb\MainForm.vb" name="AutoComplete Initialization" lang="VB"><code lang="VB">
	///            ' This code shows how the AutoComplete control
	///            ' is initialized and used
	///            Me.AutoComplete1 = New AutoComplete()
	///            Me.textBox1 = New TextBox()
	///            ' Set AutoComplete1 properties
	///            ' Set the ParentForm property of the AutoComplete control
	///            Me.AutoComplete1.ParentForm = Me
	///            Me.AutoComplete1.AdjustHeightToItemCount = True
	///            ' The AutoComplete control will add contents of the target control to
	///            ' its history list automatically (when the enter key is hit)
	///            Me.AutoComplete1.AutoAddItem = True
	///            ' The AutoComplete control will maintain its history items
	///            Me.AutoComplete1.AutoSerialize = True
	///            ' The category under which the history list will be persisted
	///            Me.autoComplete1.CategoryName = "websites"
	///            ' Specifies if any of the columns in the data source are to be treated as
	///            ' image index
	///            Me.AutoComplete1.ImageColumnIndex = -(1)
	///            ' The image list to be used as the image source
	///            Me.AutoComplete1.ImageList = Nothing
	///            ' The preferred height for the drop down window
	///            Me.AutoComplete1.PreferredHeight = 200
	///            ' Add a event to customize data added to history
	///            AddHandler Me.autoComplete1.BeforeAddItem, New AutoCompleteAddItemCancelEventHandler(AddressOf AutoComplete1_BeforeAddItem)
	///            ' Set Auto Complete
	///            ' properties for target TextBox
	///            Me.AutoComplete1.SetAutoComplete(Me.textBox1, AutoCompleteMode.Both)
	///            ' Added by designer - Add the TextBox to the Form
	///            Me.Controls.Add(textBox1)
	/// 	</code></coderef></example>
	[
	ProvideProperty( "AutoComplete", typeof( Control ) ),
	Designer( typeof( AutoCompleteDesigner ), typeof( IDesigner ) ),
	ToolboxBitmap( typeof( AutoComplete ), "ToolboxIcons.AutoComplete.bmp" ),
	ToolboxItemFilter( "System.Windows.Forms" ),
	Description("Provides auto completion capabilities for edit controls")
	]
	public class AutoComplete :
		Component,
		IExtenderProvider,
		IEditControlsEmbedListener,
		IDataListViewOwner,
        IMessageFilter,
		ISupportInitialize
	{
		#region Class constants
		/// <summary></summary>
		private const int DEF_MAX_KEY_COUNT = 32;
		/// <summary></summary>
		private const string c_strAUTO_COMPLETE = "AutoComplete";
		#endregion

		#region Class members
		/// <summary></summary>
		private AutoCompleteItem selectedItem;
		/// <summary></summary>
		private int selectedIndex = -1;
		/// <summary></summary>
        private bool isEnterHit = false;
        /// <summary></summary>
		private string selectedValue = String.Empty;
		/// <summary></summary>
		private static int nCounter = 0;
		/// <summary>
		/// The event args for the AutoCompleteAddItemCancelEventHandler. This event handler is used
		/// as the event data for the <see cref="AutoComplete.BeforeAddItem"/> event raised by the
		/// <see cref="AutoComplete"/> control.
		/// </summary>
		private Container components = null;
		/// <summary>
		/// Holds information about what the AutoComplete extender
		/// property is set on the target controls. The control
		/// is the key and the AutoComplete is the value.
		/// </summary>
		private Hashtable targetEnabledMap;
		/// <summary>
		/// Holds information for each control if ComboUpDown behavior is
		/// to be shown.
		/// </summary>
		private Hashtable targetComboUpDownMap;
		/// <summary>
		/// The control that has the current focus.
		/// </summary>
		private Control activeControlObject;
		/// <summary>
		/// The text of the target control before the change
		/// suggested by the AutoComplete control.
		/// </summary>
		private string preChangeText;
		/// <summary>
		/// Specifies whether the TextChanged event should be ignored.
		/// </summary>
		private int ignoreChangeMessage;
		/// <summary>
		/// Indicates whether case sensitivity for matching should be used.
		/// </summary>
		private bool ignoreCase;
		/// <summary>
		/// The PopupControlContainer.
		/// </summary>
		private SizablePopupControlContainer dropDownContainer;
		/// <summary>
		/// Indicates whether the content has changed.
		/// </summary>
		private bool contentChanged;
		/// <summary>
		/// The list used for displaying the matches.
		/// </summary>
		private VirtualListView dropDownList;
		/// <summary>
		/// Indicates whether the internal data will be persisted
		/// by the control itself. 
		/// </summary>
		private bool autoSerializeValue;
		/// <summary>
		/// Overrides the IgnoreCompletion check when set. This
		/// is needed when the ProcessAutoComplete method is
		/// called when the Text property of the target control is empty.
		/// </summary>
		private bool internalOverrideIgnoreCompletion;
		/// <summary>
		/// The collection of Columns specifying the attributes of the columns of
		/// the List displayed with the matches.
		/// </summary>
		private AutoCompleteDataColumnInfoCollection columnInfoList;
		/// <summary>
		/// The image list that will be used by the AutoComplete object.
		/// </summary>
		private ImageList imageListValue;
		/// <summary>
		/// Indicates whether the drop down list will have a column header.
		/// </summary>
		private bool showColumnHeaderValue;
		/// <summary>
		/// Specifies the kind of border for the popup control.
		/// </summary>
		private AutoCompleteBorderTypes borderType;
		/// <summary>
		/// Specifies the user defined category under which the
		/// data held by this AutoComplete needs to persist historical
		/// data.
		/// </summary>
		private string categoryName = String.Empty;
		/// <summary>
		/// The minimum column width when a new column is added.
		/// </summary>
		private static int minColumnWidth;
		/// <summary>
		/// Indicates whether the current content should be added to the history
		/// during validation (when the target control loses focus).
		/// </summary>
		private bool autoAddItemOnValidate;
		/// <summary>
		/// The internal DataTale that will hold the history items.
		/// </summary>
		private DataTable tableData;
		/// <summary>
		/// The external data source.
		/// </summary>
		private object dataSource;
		/// <summary>
		/// InvalidDataType is raised when the specified  data type is not valid.
		/// </summary>
        [Description("InvalidDataType is raised when the specified  data type is not valid.")]
		public event AutoCompleteErrorEventHandler InvalidDataType;
		/// <summary>
		/// Indicates whether the controls have been inistialized for the drop down.
		/// </summary>
		private bool controlsInitialized = false;
		/// <summary>
		/// The preferred height.
		/// </summary>
		private int preferredHeight;
		/// <summary>
		/// The preferred width.
		/// </summary>
		private int preferredWidth = -1;
		/// <summary>
		/// Indicates whether the height of the drop down is to be adjusted based on the number
		/// of items.
		/// </summary>
		private bool adjustHeightToItemCount;
		/// <summary>
		/// Indicates whether a SelesctionChangeCommitted event has been raised by a target
		/// combobox. This is tracked so that the following TextChanged event will be 
		/// ignored by the AutoCompletion logic.
		/// </summary>
		private bool selectionChangeCommitted;
		/// <summary>
		/// The mode for the matching routine.
		/// </summary>
		private AutoCompleteMatchModes matchMode;
		/// <summary>
		/// The internal field that indicates whether the AutoComplete control 
		/// should suppress or override the ComboBox control's own dropdown.
		/// </summary>
		protected bool overrideComboValue;
		/// <summary></summary>
		private bool inInit = false;
		/// <summary></summary>
		private DataView listDataView = null;
		/// <summary></summary>
		private string accessibleName;
		/// <summary></summary>
		private string accessibleDescription;
		/// <summary></summary>
		private AccessibleRole accessibleRole;
		/// <summary>for the data binding support</summary>
		private Control parentForm;
		/// <summary></summary>
		private int comboUpDownState = 0;
		/// <summary></summary>
		private string lastMatchColumnName = String.Empty;
		/// <summary></summary>
		private bool changeDataManagerPosition = false;
		/// <summary></summary>
		private int indexToEnsureVisible = -1;
		/// <summary>
		/// Indicates whether the dropdown  for an AutoComplete enabled control appears in a single click.
		/// </summary>
		private bool singleClick = false;
		/// <summary></summary>
		private string currentTextBeingMatched = String.Empty;
		/// <summary></summary>
		private bool caseSensitiveValue = false;
		/// <summary>
		/// Allows user to delete entries from list.
		/// </summary>
		private bool allowListDelete = false;
        /// <summary>
        /// Allows user to set the maximum no of list to be displayed. By default the value is -1 .
        /// </summary>
        private int maxNumberofSuggestion = -1;
		/// <summary>
		/// Indicates whether the list is to be sorted automatically.
		/// </summary>
		private bool autoSortList = true;
		/// <summary>
		/// The RightToLeft state of the target control.
		/// </summary>
		private RightToLeft targetRightToLeft = RightToLeft.No;
		/// <summary>
		/// Indicates whether DropDown size is persistent.
		/// </summary>
		private bool m_bPersistentDropDownSize = false;

        private bool bProcess = true;

		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the value of the selected item in the AutoComplete control.
		/// </summary>
		[Description( "Gets or sets the value of the selected item in the AutoComplete control." ), DefaultValue("")]
		public object SelectedValue
		{
			get
			{
				return selectedValue;
			}

			set
			{
				selectedValue = ( string )value;
			}
		}

		/// <summary>
		/// Sets or retrieves the index of the selected option.
		/// </summary>
		[Description( "Sets or retrieves the index of the selected option." ), DefaultValue( - 1 )]
		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}

			set
			{
                if( this.selectedIndex != value )
                {
                    this.selectedIndex = value;
                    this.dropDownList.SetSelectedIndex( this.selectedIndex );
                }
				
			}
		}

		/// <summary>
		/// Returns the selected item. This is a changing property and valid only at runtime.
		/// The last browsed item in an AutoComplete control drop down will be set as the
		/// selected item.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public AutoCompleteItem SelectedItem
		{
			get
			{
				return this.selectedItem;
			}
		}
		/// <summary>
		/// Specifies whether the auto completion should be performed for this text change.
		/// </summary>
		/// <remarks>Use internally as a counter to calculate if the current
		/// change in the target control's text requires auto completion or is to be 
		/// ignored.</remarks>
		private int IgnoreChangeMessage
		{
			get
			{
				return this.ignoreChangeMessage;
			}

			set
			{
				this.ignoreChangeMessage = value;
			}
		}
		/// <summary>
		/// Indicates whether the dropdown for an AutoComplete enabled control appears in a single click.
		/// </summary>
		/// <remarks >
		/// The SingleClick property controls how many times the user has to click on an AutoComplete enabled Control for the dropdown to appear.  By default, it requires a double-click.  But if SingleClick is set to true, the first click will make the dropdown appear. 
		/// </remarks>
		/// <example>
		/// Example shows the SingleClick property activated when AutoComplete enabled edit control gets focus.
		/// <code lang="C#">
		/// private void textBox1_GotFocus(object sender, EventArgs e)
		/// {
		///   this.autoComplete1.SingleClick = true;
		/// }  
		/// </code>
		/// <code lang="VB">
		/// Private Sub textBox1_GotFocus(ByVal sender As Object, ByVal e As EventArgs) 
		/// Me.autoComplete1.SingleClick = True 
		/// </code>
		/// </example> 
		[
		DefaultValue( false ),
		Category( "Behavior" ),
		Description( "Indicates whether the dropdown for an AutoComplete enabled control appears in a single click." )
		]
		public bool SingleClick
		{
			get
			{
				return this.singleClick;
			}

			set
			{
				this.singleClick = value;
			}
		}

		/// <summary>
		/// Indicates whether to allow deletion of items in the list when user pressed Delete Key.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Indicates whether to allow deletion of items in the list when user pressed Delete Key." )
		]
		public bool AllowListDelete
		{
			get
			{
				return this.allowListDelete;
			}

			set
			{
				this.allowListDelete = value;
			}
		}

        /// <summary>
        /// Indicates How many number of items to be displayed in the AutoComplete.
        /// </summary>

        [
        DefaultValue(-1),
        Description("Indicates maximum number of items to be displayed in AutoComplete.")
        ]

        public int MaxNumberofSuggestion
        {
            get
            {
                return this.maxNumberofSuggestion;
            }
            set
            {
                this.maxNumberofSuggestion = value;
            }
        }

		/// <summary>
		/// Indicates whether default sorting is to be performed.
		/// </summary>
		[
		DefaultValue( true ),
		Description( "Indicates whether default sorting is to be performed." )
		]
		public bool AutoSortList
		{
			get
			{
				return this.autoSortList;
			}
			set
			{
				this.autoSortList = value;
			}
		}

		/// <summary>
		/// Indicates whether the current item in the target control is to be automatically
        /// added to the history list during validation and when the Enter key is pressed.
		/// </summary>
		/// <remarks>The AutoComplete control has two distinct functions. One is to 
		/// provide AutoCompletion to partial strings typed into edit controls that are 
		/// targeted by the AutoComplete control. The second function is to maintain the
		/// history or item list against which the partial string will be compared for
		/// possible matches.
		/// The AutoAddItem property specifies automatic addition to the history list or 
		/// item list that the AutoComplete control is referring to.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Behavior" ),
        DefaultValue( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "Set to true if you want items to be added to the history automatically when the control's validation event is raised." )
		]
		public bool AutoAddItem
		{
			get
			{
				return this.autoAddItemOnValidate;
			}

			set
			{
				this.autoAddItemOnValidate = value;
			}
		}

		/// <summary>
		/// Gets or sets the user defined category name under which the internal data is serialized.
		/// </summary>
		/// <remarks>The CategoryName is an important property that is useful
		/// in properly using the AutoComplete control. You can specify a 
		/// common CategoryName for AutoComplete controls in use in various
		/// forms. For example, if you have a form based application that 
		/// collects the First Name of users in more than one
		/// form, you can set the CategoryName to be "FirstName" and set the
		/// edit controls used for collecting the First Name under the 
		/// operation of the AutoComplete controls. This CategoryName will help share
		/// the First Names across forms.</remarks>
		[
		Browsable( true ),
		Category( "Data" ),
        DefaultValue( "" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "The category name under which history items will be persisted. This is also used when to build the history items this control will use." )
		]
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}

			set
			{
				this.categoryName = value;
                this.DeSerializeList();
			}
		}

		/// <summary>
		/// Specifies the kind of border the <see cref="AutoComplete"/> control should use for 
		/// the popup control displayed when in <see cref="AutoCompleteModes.AutoSuggest"/> Mode.
		/// </summary>
		/// <seealso cref="AutoCompleteBorderTypes"/>
		/// <seealso cref="AutoCompleteModes"/>
		/// <remarks>
		/// The drop down window's appearance can be controlled by setting this property.
		/// The default value is <see cref="AutoCompleteBorderTypes.Sizable"/>.
		/// </remarks>
		[
		Browsable( true ),
		DefaultValue( AutoCompleteBorderTypes.Sizable ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( @"The border type for the popup list displayed when in AutoSuggest Mode" )
		]
		public AutoCompleteBorderTypes BorderType
		{
			get
			{
				return this.borderType;
			}

			set
			{
				this.borderType = value;
			}
		}

		/// <summary>
		/// Gets or sets the matching mode for the matching routine.
		/// </summary>
		/// <seealso cref="AutoCompleteMatchModes"/>
		/// <remarks>
		/// The matching mode that is specified will have a marked impact
		/// on the nature and speed of the auto completion process. The Matching mode can
		/// be set to <see cref="AutoCompleteMatchModes.Automatic"/> or 
		/// <see cref="AutoCompleteMatchModes.Manual"/>.
		/// <para>
		/// The default mode is <see cref="AutoCompleteMatchModes.Automatic"/>
		/// In the automatic mode, the current text of the target edit control is matched
		/// with the internal history list or external data source using a <see cref="DataTable"/>
		/// filter and the results are displayed in the drop down list when in 
		/// AutoSuggest mode and used to fill the edit control when in AutoAppend mode.
		/// </para>
		/// <para>
		/// Setting the MatchMode to Manual brings in a lot of change in the functionality
		/// of the AutoComplete control. Instead of getting the list of items matching the
		/// current text, the AutoComplete control will loop through all of the items in the
		/// history list and raise a <see cref="AutoComplete.MatchItem"/> event for each of 
		/// these entries.
		/// </para>
		/// <para>
		/// The user can handle the event and do their own comparison and let the AutoComplete
		/// control to include an item in the matching list.
		/// However, the Manual setting will tend to slow down the process and the desired
		/// method for providing your own custom matching would be to override the 
        /// <see cref="AutoComplete.PopulateListWithMatches(string, ref System.Data.DataView, bool, ref System.Data.DataRow)"/> method.
		/// </para>
		/// </remarks>
		[
		Browsable( true ),
		DefaultValue( AutoCompleteMatchModes.Automatic ),
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( @"The border type for the popup list displayed when in AutoSuggest Mode" )
		]
		public AutoCompleteMatchModes MatchMode
		{
			get
			{
				return this.matchMode;
			}

			set
			{
				this.matchMode = value;
			}
		}

		/// <summary>
		/// Returns the columns that will be displayed in the popup control when the AutoCompleteMode is
		/// set to AutoSuggest. The Columns property is a collection of <see cref="AutoCompleteDataColumnInfo"/> objects
		/// that specifies the attributes of a column.
		/// </summary>
		/// <seealso cref="AutoCompleteDataColumnInfoCollection"/>
		/// <seealso cref="AutoCompleteDataColumnInfo"/>
		/// <remarks>You can specify a list of columns and their width and column heading
		/// for each column in the drop down list of the matching items. In the event that an
		/// external data source is specified through the <see cref="AutoComplete.DataSource"/> property
		/// the columns in the data source will automatically be represented in the
		/// Columns collection (except for the columns set to be invisible).
		/// <para>
		/// At design time there will be a Refresh Columns verb available that can be
		/// invoked to synchronize the <see cref="DataSource"/> with the <see cref="Columns"/> collection.
		/// Any changes made to the <see cref="Columns"/> collection after invoking this
		/// verb will be retained. Do not invoke the Refresh Columns verb after making
		/// changes to the <see cref="Columns"/> collection as the changes made will be lost and the
		/// column data will be refreshed from the data source.
		/// </para>
		/// <para> There is no constraint to the number of columns that will be displayed.
		///  But, the first 32 columns are considered to be "Keys" and the matching column has to be within the first 32 columns.</para>
		/// </remarks>
		/// <example>
		///<code lang="C#">
		///this.autoComplete1.DataSource =this.dataView1 ; 
		///this.autoCompleteDataColumnInfo1 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("EmployeeID", 100, true); 
		///this.autoCompleteDataColumnInfo2 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("LastName", 100, true); 
		///this.autoCompleteDataColumnInfo3 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("BirthDate", 100, true); 
		///this.autoCompleteDataColumnInfo4 = new Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("City", 100, true);  
		///</code>
		///<code lang="VB">
		///Me.autoComplete1.DataSource =Me.dataView1 
		///Me.autoCompleteDataColumnInfo1 = New Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("EmployeeID", 100, True) 
		///Me.autoCompleteDataColumnInfo2 = New Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("LastName", 100, True) 
		///Me.autoCompleteDataColumnInfo3 = New Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("BirthDate", 100, True) 
		///Me.autoCompleteDataColumnInfo4 = New Syncfusion.Windows.Forms.Tools.AutoCompleteDataColumnInfo("City", 100, True) 
		///</code>
		/// </example>
		[
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		MergableProperty( false ),
		Localizable( true ),
		Description( "The collection of Columns for the drop down list when in AutoSuggest Mode." )
		]
		public AutoCompleteDataColumnInfoCollection Columns
		{
			get
			{
				return this.columnInfoList;
			}
		}

		/// <summary>
		/// Indicates whether the popup control displayed during AutoSuggest
		/// mode has headers. This property will apply to all the columns.
		/// </summary>
		/// <remarks>You can specify headings for the columns that are displayed
		/// through the <see cref="AutoComplete.Columns"/> collection through the property
		/// grid.</remarks>
		[
		Browsable( true ),
		DefaultValue( false ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( @"Specifies if the drop down list of possible matches will show their headers." )
		]
		public bool ShowColumnHeader
		{
			get
			{
				return this.showColumnHeaderValue;
			}

			set
			{
				this.showColumnHeaderValue = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the close button at the 
		/// bottom right of the DropDownContainer.
		/// </summary>
		[
        Description( "Gets or sets a value indicating whether to show the close button at the bottom right of the DropDownContainer." ),
        DefaultValue( true )
        ]
		public bool ShowCloseButton
		{
			get
			{
				return dropDownContainer.ShowCloseButton;
			}
			set
			{
				if( value != dropDownContainer.ShowCloseButton )
				{
					dropDownContainer.ShowCloseButton = value;
				}
			}
		}

		/// <summary>
		/// The size gripper at the bottom right of the DropDownContainer will be shown 
		/// as long as the ShowGripper property is set to true.
		/// </summary>
        
		[
        Description( "Indicates whether sizing gripper is shown at the bottom right of the DropDownContainer"),
        DefaultValue(true)
        ]
		public bool ShowGripper
		{
			get
			{
				return dropDownContainer.ShowGripper;
			}
			set
			{
				if( value != dropDownContainer.ShowGripper )
				{
					dropDownContainer.ShowGripper = value;
				}
			}
		}

		/// <summary>
		/// Returns the PopUp of AutoComplete
		/// </summary>
		/// <example>
		/// This example shows how the AutoCompletePopup property exposes the internal SizablePopupControlContainer through ShowPopup method. 
		///<code lang="C#">
		///private void ComboBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) 
		///{ 
		///if (CheckBox1.Checked) { 
		///this.AutoComplete1.AutoCompletePopup.ShowPopup(this.PointToScreen(new Point(ComboBox1.Location.X, ComboBox1.Location.Y + ComboBox1.Height))); 
		///} 
		///}
		///</code>
		///<code lang="VB">
		///Private Sub ComboBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ComboBox1.KeyDown
		///If CheckBox1.Checked Then
		///Me.AutoComplete1.AutoCompletePopup.ShowPopup(Me.PointToScreen(New Point(ComboBox1.Location.X, ComboBox1.Location.Y + ComboBox1.Height)))
		///End If///End Sub
		///</code>
		/// </example>
		public PopupControlContainer AutoCompletePopup
		{
			get
			{
				return dropDownContainer;
			}
		}
		/// <summary>
		/// Gets or sets the ImageList that will specify the images to be used
		/// by the popup control when in AutoSuggest Mode.
		/// </summary>
		/// <remarks>You can specify an ImageList object as the source of 
		/// images or icons for the matching items. 
		/// First drag and drop (or create through code) an <see cref="ImageList"/> object.
		/// Then add the icons you want to use to the ImageList.
		/// Set the AutoComplete.ImageList property to point to the ImageList you created.
		/// You also need to set the <see cref="AutoComplete.ImageColumnIndex"/> property to indicate
		/// in which column of the data the index (of the image in the ImageList) is
		/// specified.
		/// </remarks>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( "The imagelist to be used by the control to display icons for the items in the history list" )
		]
		public ImageList ImageList
		{
			get
			{
				return this.imageListValue;
			}

			set
			{
				this.imageListValue = value;
			}
		}

		/// <summary>
		/// Indicates whether the AutoComplete control persists its data.
		/// </summary>
		/// <remarks>The AutoComplete control can maintain its own internal
		/// history data and persist it to IsolatedStorage and read it back.
		/// The default value for this property is true.
		/// If you leave the default values as is for the AutoComplete control
		/// and set the <see cref="AutoComplete.CategoryName"/> property, the new entries
		/// that are added will also be persisted and read back.</remarks>
		/// <example>
		///If AutoSerialize is true, history items are saved to / loaded  from registry 
		///automatically.This example uses AutoAddItem and CategoryName properties along with AutoSerialize set 
		///to false inorder to make the control save list items to an external datasource like XML.
		///<code lang="C#">
		///this.autoComplete.AutoAddItem = true;
		///this.autoComplete.AutoSerialize = false;
		///this.autoComplete.CategoryName = "SomeCategory";
		///</code>
		///<code lang="VB">
		///Me.autoComplete.AutoAddItem = True 
		///Me.autoComplete.AutoSerialize = False 
		///Me.autoComplete.CategoryName = "SomeCategory"
		///</code>
		/// </example>
		[ Browsable( true ),
		Category( "Behavior" ),
        DefaultValue( true ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( @"Specifies whether the AutoComplete control persists its data." ) ]
		public bool AutoSerialize
		{
			get
			{
				return this.autoSerializeValue;
			}

			set
			{
				this.autoSerializeValue = value;
			}
		}

		/// <summary>
		/// Indicates whether the case sensitivity should be used for string comparison.
		/// </summary>
		/// <remarks>This setting specifies if the default matching routine is case
		/// sensitive.</remarks>
		[ Browsable( true ),
		DefaultValue( true ),
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Description( @"Specifies the case sensitivity for string comparison.." ) ]
		public bool IgnoreCase
		{
			get
			{
				return this.ignoreCase;
			}

			set
			{
				this.ignoreCase = value;
			}
		}
        private AutoCompleteStyle style = AutoCompleteStyle.Default;

        /// <summary>
        /// Indicates the visual style.
        /// </summary>
        /// <value>Default value is true.</value>
        [Description("Indicates the visual style.")]
        [Category("Appearance")]
        public AutoCompleteStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                if (style == AutoCompleteStyle.Metro)
                {
                    scroll.VisualStyle = ScrollBarCustomDrawStyles.Metro;
                    scroll.MetroColorScheme = MetroColorScheme.Blue;
                    scroll.AttachedTo = this.dropDownList;
                }
                else
                {
                    scroll.DetachFrame();
                }
            }
        }
        private Color metroColor = ColorTranslator.FromHtml("#119EDA");

        /// <summary>
        /// Indicates the metro color.
        /// </summary>
        /// <value>Default value is true.</value>
        [Description("Indicates the metro color.")]
        [Category("Appearance")]
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                metroColor = value;
            }
        }

        private Color foreColor = Color.Black;

        /// <summary>
        /// Indicates the Text ForeColor.
        /// </summary>
        /// <value>Default value is Black.</value>
        [Description("Indicates the Text color.")]
        [Category("Appearance")]
        public Color TextColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                if(value != foreColor)
                    foreColor = value;
            }
        }

		/// <summary>
		/// Gets or sets the target control with the current focus.
		/// </summary>
		/// <remarks>This property is used internally to get to the 
		/// active edit control that is being provided auto completion by
		/// this AutoComplete control.</remarks>
		/// <example>
		///This example overrides the Form's ProcessCmdKey method and close the drop down if the ActiveControl of the 
		///form is the same as the ActiveFocusControl of the AutoComplete control.
		///<code lang="C#">
		///public class Form1 : System.Windows.Forms.Form
		///{
		///   protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		///   {
		///        if (keyData == Keys.Escape)
		///        {
		///           if (this.ActiveControl == this.autoComplete1.ActiveFocusControl)
		///            {
		///                this.autoComplete1.CloseDropDown();
		///                return true;
		///            }
		///        }
		///        return base.ProcessCmdKey(ref msg, keyData);
		///    }
		///}
		///</code>
		///  <code lang="VB">
		///Public Class Form1 Inherits System.Windows.Forms.Form 
		///Protected Overloads Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean 
		///If keyData = Keys.Escape Then 
		///If Me.ActiveControl = Me.autoComplete1.ActiveFocusControl Then 
		///Me.autoComplete1.CloseDropDown 
		/// Return True 
		///End If 
		///End If
		///Return MyBase.ProcessCmdKey(msg, keyData) 
		///End Function 
		///End Class
		///</code>
		/// </example>
		[ Browsable( false ),
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Description( @"The target control with the current focus" ) ]
		public Control ActiveFocusControl
		{
			get
			{
				return this.activeControlObject;
			}

			set
			{
                if (this.activeControlObject != value)
                {
                    this.activeControlObject = value;
                }
			}
		}

		/// <summary>
		/// Gets or Sets whether DropDown size is automatically persistent.
		/// </summary>
		[
		Browsable( true ),
		DefaultValue( false ),
		Category( "Behavior" ),
		Description( "Indicates whether DropDown size is persistent." )
		]
		public bool AutoPersistentDropDownSize
		{
			get
			{
				return m_bPersistentDropDownSize;
			}
			set
			{
				m_bPersistentDropDownSize = value;
			}
		}

		/// <summary>
		/// Returns the minimum column width. The AutoComplete class supports multiple columns in the drop down
		/// window of the possible matches. This static member is set to the minimum
		/// width of such columns when displayed in the drop down.
		/// </summary>
		public static int MinColumnWidth
		{
			get
			{
				return AutoComplete.minColumnWidth;
			}
			set
			{
				AutoComplete.minColumnWidth = value;
			}
		}

		/// <summary>
		/// Indicates whether the DataManager position is to be changed when entries
		/// in the ListControl are selected (when the DataSource property is set to a 
		/// data source).
		/// </summary>
		[
		Browsable( true ),
		DefaultValue( false ),
		Description( "Specifies if the DataManager position is to be changed when entries in the ListControl are selected." )
		]
		public bool ChangeDataManagerPosition
		{
			get
			{
				return this.changeDataManagerPosition;
			}

			set
			{
				this.changeDataManagerPosition = value;
			}
		}

		/// <summary>
		/// Indicates whether the replacement of the matching entry is to be case
		/// sensitive. If the user enters 'A' and there is a matching entry
		/// 'abcd', the AutoComplete will set the target edit control's Text
		/// to 'abcd' if this property is set to false and 'Abcd' if this
		/// property is set to true.
		/// </summary>
		[
		Browsable( true ),
		DefaultValue( false ),
		Description( "Specifies if the replacement of the matching entry is to be case sensitive." )
		]
		public bool CaseSensitive
		{
			get
			{
				return this.caseSensitiveValue;
			}

			set
			{
				this.caseSensitiveValue = value;
			}
		}

		/// <summary>
		/// Gets / sets the text which was before the next ProcessAutoComplete method is called.
		/// </summary>
		internal string PreChangeText
		{
			get
			{
				return preChangeText;
			}
			set
			{
				if( value != preChangeText )
				{
					preChangeText = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the description of the control used by accessibility client applications.
		/// </summary>
		[
		Category( "Accessibility" ),
		Description( "Gets or sets the description of the control used by accessibility client applications." ),
        DefaultValue(null)
		]
		public string AccessibleDescription
		{
			get
			{
				return this.accessibleDescription;
			}

			set
			{
				this.accessibleDescription = value;
			}
		}
		/// <summary>
		/// Gets or sets the name of the control used by accessibility client applications.
		/// </summary>
		[
		Category( "Accessibility" ),
		Description( "Gets or sets the name of the control used by accessibility client applications." ),
        DefaultValue(null)
		]
		public string AccessibleName
		{
			get
			{
				return this.accessibleName;
			}

			set
			{
				this.accessibleName = value;
			}

		}

		/// <summary>
		/// Gets or sets the accessible role of the control used by accessibility client applications.
		/// </summary>
		[
		Category( "Accessibility" ),
		Description( "Gets or sets the accessible role of the control." ),
        DefaultValue(typeof(AccessibleRole), "None")
		]
		public AccessibleRole AccessibleRole
		{
			get
			{
				return this.accessibleRole;
			}

			set
			{
				this.accessibleRole = value;
			}
		}

		/// <summary>
		/// Gets or sets the ParentForm for databinding purposes.
		/// </summary>
		/// <example>
		/// Example shows how the AutoComplete control can be used in a UserControl 
		/// In this case when the AutoComplete control is used in a UserControl, the 
		/// parent form of the UserControl has to be set to the ParentForm property 
		/// of the AutoComplete control.
		/// <code lang="C#">
		/// private void UserControl1_Load(object sender, System.EventArgs e) 
		/// {  
		/// this.autoComplete1.ParentForm = this.ParentForm;  
		/// this.autoComplete1.DataSource = this.items; 
		/// } 
		/// </code>
		/// <code lang="VB">
		/// Private Sub UserControl1_Load(ByVal sender As Object, ByVal e As System.EventArgs) 
		/// Me.autoComplete1.ParentForm = Me.ParentForm Me.autoComplete1.DataSource = Me.items 
		/// End Sub 
		/// </code>
		/// </example>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Browsable( false )
		]
		public Control ParentForm
		{
			get
			{
				if( this.parentForm == null )
				{
					if (this.DesignMode)
					{
						IDesignerHost idhost = this.Site.Container as IDesignerHost;
						Form mainfrm = idhost.RootComponent as Form;
						this.parentForm = mainfrm;
					}
				}
				return this.parentForm;
			}

			set
			{
				this.parentForm = value;
			}
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public bool Initializing
		{
			get
			{
				return this.inInit;
			}
		}
		/// <summary>
		/// Indicates whether the AutoComplete control should suppress or override the ComboBox 
		/// control's own dropdown.
		/// </summary>
		/// <remarks>
		/// Set this property to true if the target <see cref="ComboBox"/>'s drop down
		/// list is replaced by the <see cref="AutoComplete"/> control's drop down list.
		/// If the value is set to false the AutoComplete control's drop down will not be
		/// displayed when the <see cref="ComboBox"/>'s drop down list is visible.
		/// </remarks>
		[
		Browsable( true ),
		Description( "Setting this property to true will override the ComboBox (target) control's drop down." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool OverrideCombo
		{
			get
			{
				return this.overrideComboValue;
			}

			set
			{
				this.overrideComboValue = value;
			}
		}
		/// <summary>
		/// Gets or sets the column that is to be used for matching during AutoCompletion.
		/// </summary>
		[
		Browsable( true ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Category( "Behavior" ),
		Description( "Gets or sets the column that is to be used for matching during AutoCompletion." )
		]
		public string MatchColumnName
		{
			get
			{
				for( int i = 0 ; i < this.Columns.Count ; i++ )
				{
					if( this.Columns[ i ].MatchingColumn == true )
					{
						return this.Columns[ i ].ColumnName;
					}
				}

				if( this.Columns.Count > 0 )
				{
					return this.Columns[ 0 ].ColumnName;
				}
				else
				{
					return String.Empty;
				}
			}
		}
		/// <summary>
		/// Returns the column index that will serve as the image index.
		/// </summary>
		/// <remarks>
		/// The AutoComplete control supports displaying an icon in the 
		/// drop down list for each matching item. This icon is specified
		/// to the AutoComplete control by specifying the data for one of its
		/// <see cref="AutoComplete.Columns"/> as the image index into the <see cref="ImageList"/>
		/// provided in the <see cref="AutoComplete.ImageList"/> property.
		/// </remarks>
		[
		Description( "The column index that will serve as the image index." ),
		Category( "Appearance" ),
		Browsable( true )
		]
		public int ImageColumnIndex
		{
			get
			{
				//return this.dropDownList.ImageColumnIndex;
				int imageColumnIndex = -1;
				int columnIndex = 0;
				foreach( AutoCompleteDataColumnInfo info in this.Columns )
				{
					if( info.ImageColumn == true )
					{
						return columnIndex;
					}
					columnIndex++;
				}

				return imageColumnIndex;
			}
		}

		/// <summary>
		/// This is a read only property that provides the internal <see cref="System.Data.DataTable"/>
		/// object that is used to manipulate the list of history items.
		/// </summary>
		/// <remarks>
		/// The AutoComplete control uses a <see cref="System.Data.DataTable"/> object
		/// internally to manipluate the matching data that is to be used for its 
		/// matching operations. This property is populated from an internal history list that the
		/// AutoComplete class maintains or from the external data source that is
		/// specified through the <see cref="AutoComplete.DataSource"/> property.
		/// </remarks>
		/// <example>
		/// This example shows how the rows of internal DataTable can be accessed
		///<code lang="C#">
		/// foreach(DataRow dr in this.autoComplete1.TableData.Rows)
		///{
		/// Console.WriteLine ("Rows "+dr[0]);
		///}
		///</code>
		///<code lang="VB">
		///For Each dr As DataRow In Me.autoComplete1.TableData.Rows 
		///Console.WriteLine("Rows " + dr(0)) 
		///Next
		///</code>
		/// </example>
		[
		Browsable( false ),
		Description( "The internal DataTable object used by the AutoComplete control to manipulate the matching items history list." ),
		Category( "Data" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public DataTable TableData
		{
			get
			{
				return this.tableData;
			}
		}

		/// <summary>
		/// Indicates whether the height of the drop down should be adjusted based on the
		/// number of items.
		/// </summary>
		/// <remarks>The AutoComplete control displays a list of matching items when
		/// operating in <see cref="AutoCompleteModes.AutoSuggest"/> mode. This property indicates whether the 
		/// height of the drop down should be adjusted to make the maximum number
		/// of items visible.</remarks>
		[
		Category( "Appearance" ),
		Description( "Specifies if the height of the drop down should be adjusted based on the number of items." ),
		Browsable( true ),
        DefaultValue( true )
		]
		public bool AdjustHeightToItemCount
		{
			get
			{
				return this.adjustHeightToItemCount;
			}

			set
			{
				this.adjustHeightToItemCount = value;
			}
		}

		/// <summary>
		/// Gets or sets the preferred height for the drop down list displayed by the AutoComplete control.
		/// </summary>
		/// <remarks>The preferred height is used by the AutoComplete control to
		/// fix the initial height of the drop down window that is displayed
		/// when in <see cref="AutoCompleteModes.AutoSuggest"/> mode.</remarks>
		[
		Category( "Appearance" ),
		Description( "The preferred height for the drop down list displayed by the AutoComplete control." ),
		Browsable( true ),
        DefaultValue ( 200 )
		]
		public int PreferredHeight
		{
			get
			{
				return this.preferredHeight;
			}

			set
			{
				this.preferredHeight = value;
			}
		}

		/// <summary>
		/// Gets or sets the preferred width for the drop down list displayed by the AutoComplete control.
		/// </summary>
		/// <remarks>The preferred width is used by the AutoComplete control to
		/// fix the initial width of the drop down window that is displayed
		/// when in <see cref="AutoCompleteModes.AutoSuggest"/> mode.</remarks>
		[
		Category( "Appearance" ),
		Description( "The preferred width for the drop down list displayed by the AutoComplete control." ),
		Browsable( true ),
		DefaultValue( -1 )
		]
		public int PreferredWidth
		{
			get
			{
				return this.preferredWidth;
			}

			set
			{
				this.preferredWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the external data source that will be used as the history item list.
		/// </summary>
		/// <remarks>The AutoComplete control can take an external data source
		/// (any data source that implements IList or IListSource) for its history
		/// list. When this property is set, the AutoComplete control will
		/// initialize itself with the data source and use that as the basis for 
		/// its matching routines. </remarks>
		/// <example>
		///For example, if you have a DataSet with the
		/// list of names of the States in the US and specify that as the 
		/// DataSource, the AutoComplete control will display all matches
		/// from within these names when the user types in the target edit control.
		///<code lang="C#">
		///private void Form1_Load(object sender, System.EventArgs e)
		///{
		///DataTable dt;
		///dt = new DataTable("select");
		///dt.Columns.Add("Countries");
		///dt.Columns.Add("states");
		///dt.Rows.Add(new object[] { "India " });
		///dt.Rows.Add(new object[] { "New York " });
		///dt.Rows.Add(new object[] { "Washington " });
		///dt.Rows.Add(new object[] { "London" });
		///dt.Rows.Add(new object[] { "Canada" });
		///autoComplete1.DataSource = dt;
		///} 
		///</code>
		///<code lang="VB">
		///Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs)
		///Private dt As DataTable  
		///dt = New DataTable("select")  
		///dt.Columns.Add("Countries")  
		///dt.Columns.Add("states")  
		///dt.Rows.Add(New Object() {"India "})  
		///dt.Rows.Add(New Object() {"New York "})  
		///dt.Rows.Add(New Object() {"Washington "})  
		///dt.Rows.Add(New Object() {"London"})  
		///dt.Rows.Add(New Object() {"Canada"})  
		///autoComplete1.DataSource = dt  
		///End Sub  
		///</code>
		/// </example>
		[
		Description( @"Indicates the source of data for the AutoCompleteControl" ),
		DefaultValue( null ),
		Category( @"Data" ),
		RefreshProperties( RefreshProperties.Repaint ),
		TypeConverter(typeof(AutoComplete.DataSourceConverter)),
		Browsable( true )
		]
		public object DataSource
		{
			get
			{
				return this.dataSource;
			}

			set
			{
				if( value != null && ( value is IList || value is IListSource ) )
				{
					CurrencyManager currencyManager = this.GetDataManager(this.dataSource) as CurrencyManager;
					if (currencyManager != null)
						currencyManager.ListChanged -= new ListChangedEventHandler(OnBindingListChanged);
					this.dataSource = value;
					this.SetTableData();
					currencyManager = this.GetDataManager( this.dataSource) as CurrencyManager;
					if (currencyManager != null )
						currencyManager.ListChanged += new ListChangedEventHandler(OnBindingListChanged);
				}
				else if( value == null )
				{
					this.dataSource = value;
				}
			}
		}

		#endregion

		#region Class events
		/// <summary>
		/// The event that will be raised before the AutoComplete control
		/// performs a matching operation for the current text content of the
		/// active edit control.
		/// </summary>
		/// <remarks>
		/// This event can be handled to change the text that will be used
		/// for the auto completion. 
		/// </remarks>
		[Description( "This event is raised before the AutoComplete performs a matching operation for the current text of the active edit control." )]
		public event AutoCompletePreMatchItemEventHandler PreMatchItem;
		/// <summary>
		/// This event is raised when the target control of the AutoComplete
		/// control changes.
		/// </summary>
		/// <remarks>
		/// This event can be handled to change the constraints for displaying
		/// matching items differently for different controls.
		/// </remarks>
		[Description( "This event is raised when the target control of the AutoComplete control changes." )]
		public event AutoCompleteTargetChangingEventHandler TargetChanging;
		/// <summary>
		/// Handle this event to customize the AutoCompletion. The position of the
		/// drop down can be changed. The Text to be used for auto completion can also 
		/// be changed.
		/// </summary>
		[Description( "Handle this event to customize the AutoCompletion (position, text for auto completion etc)." )]
		public event AutoCompleteCustomizeEventHandler AutoCompleteCustomize;
		/// <summary>
		/// Occurs after the dropdown has been dropped down and made visible.
		/// </summary>
		/// <remarks>Custom processing can be done when the drop down is displayed.</remarks>
		[ Description( "Occurs after the dropdown has been dropped down and made visible." ) ]
		public event EventHandler DropDownDisplayed;
		/// <summary>
		/// Occurs when the AutoComplete dropdown is closed.
		/// </summary>
		/// <remarks>
		/// Handling this event will tell you whether the dropdown was
		/// closed or cancelled by the user.
		/// </remarks>
		[ Description( "Occurs when a popup is closed." ) ]
		public event PopupClosedEventHandler DropDownClosed;
		/// <summary>
		/// Raised when a new item is about to be added. New items can be 
		/// added explicitly by calling the <see cref="AddHistoryItem(string)"/> method.
		/// </summary>
		/// <remarks>
		/// You may choose to cancel adding this item in this handler. 
		/// </remarks>
		/// <example>This example shows how you can handle this event and 
		/// selectively choose to add an item to the history list.</example>
		[
		Description( "The BeforeAddItem event is raised when a new item is about to be added." ),
		Category( "Behavior" )
		]
		public event AutoCompleteAddItemCancelEventHandler BeforeAddItem;
		/// <summary>
		/// This event enables you to provide a custom matching routine for
		/// the current value in the edit control.
		/// You can consume this event and set the <see cref="CancelEventArgs.Cancel"/>
		/// property of the <see cref="AutoCompleteMatchItemEventArgs"/> argument.
		/// </summary>
		/// <remarks>
		/// Returning true will add the entry and false will ignore the entry.
		/// </remarks>
		[
		Category( "Behavior" ),
		Description( "This event enables you to provide a custom matching routine for the current value in the edit control." )
		]
		public event AutoCompleteMatchItemEventHandler MatchItem;
		/// <summary>
		/// Raised when a new item has been selected by the user
		/// when the AutoComplete drop down list is displayed when the AutoComplete
		/// mode is set to <see cref="AutoCompleteModes.AutoSuggest"/>.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "This event is raised when a new item has been selected by the user in the drop down list." )
		]
		public event AutoCompleteItemEventHandler AutoCompleteItemSelected;
		/// <summary>
		/// Occus when the user selects an item from the list of possible matches
		/// when the AutoCompletMode is set to AutoSuggest.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "This event is raised when a new item has been selected and the text of the edit control is changed." )
		]
		public event AutoCompleteItemEventHandler AutoCompleteItemBrowsed;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes the static fields.
		/// </summary>
		static AutoComplete()
		{
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Windows", typeof( AutoComplete ).Assembly );
#else
			AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Controls", typeof( AutoComplete ).Assembly );
			AppStateSerializer.SetTypeBindingInfo( "Syncfusion.Tools.Windows", typeof( AutoCompleteInfo ).FullName, typeof( AutoCompleteInfo ).Assembly );
			AppStateSerializer.SetTypeBindingInfo( "Syncfusion.Tools.Windows", typeof( AutoCompleteDataColumnInfo ).FullName, typeof( AutoCompleteDataColumnInfo ).Assembly );
#endif

			AutoComplete.minColumnWidth = 100;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoComplete"/> class and registers it in owner's container.
		/// </summary>
		/// <param name="container">Parent container.</param>
		public AutoComplete( IContainer container ):
			this()
		{
			container.Add( this );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoComplete"/> class.
		/// </summary>
		/// <remarks>The AutoComplete class can be created through the 
		/// designer and also programmatically through code. The AutoComplete
		/// class uses the <see cref="AutoComplete.CategoryName"/> property to provide 
		/// support for differentiating between auto complete histor lists. The CategoryName
		/// property is not set by default.</remarks>
		public AutoComplete()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new LicensedComponent( typeof( AutoComplete ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.targetEnabledMap = new Hashtable();
			this.targetComboUpDownMap = new Hashtable();
			this.ignoreCase = true;
			this.internalOverrideIgnoreCompletion = false;
			this.columnInfoList = new AutoCompleteDataColumnInfoCollection( this );
			this.imageListValue = null;
			this.selectionChangeCommitted = false;

			this.preferredHeight = 200;
			this.adjustHeightToItemCount = true;
			this.contentChanged = true;

			this.borderType = AutoCompleteBorderTypes.Sizable;

			this.matchMode = AutoCompleteMatchModes.Automatic;

			this.autoSerializeValue = true;
			this.autoAddItemOnValidate = false;

			this.tableData = new DataTable( "AutoComplete" );
			this.dropDownList = new VirtualListView();
            this.dropDownList.ForeColor = this.foreColor;
			this.dropDownContainer = new SizablePopupControlContainer( this.dropDownList );

            MessageFilterEntryHelper.AddMessageFilter(this, false);

			nCounter++;
		}

		/// <summary></summary>
		~AutoComplete()
		{
			Dispose( false );
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                MessageFilterEntryHelper.RemoveMessageFilter(this);

				if( components != null )
				{
					components.Dispose();
				}

				if( this.IsDropDownShowing() )
				{
					this.dropDownContainer.Hide();
				}

				if( this.controlsInitialized )
				{
					this.dropDownContainer.Popup -= new EventHandler( this.HandlePopupDisplayed );
					this.dropDownContainer.CloseUp -= new PopupClosedEventHandler( this.HandlePopupClosed );
					this.dropDownList.SelectedIndexChanged -= new EventHandler( this.HandleAutoListSelectionChanged );
					this.dropDownList.DoubleClick -= new EventHandler( this.HandleAutoListDoubleClick );
					this.dropDownList.Click -= new EventHandler( this.HandleAutoListClick );
					this.dropDownList.KeyDown -= new KeyEventHandler( HandleListControlKeyDown );

					this.dropDownContainer.ParentControl = null;
					this.dropDownContainer.Dispose();
                    this.dropDownContainer = null;
					this.dropDownList.Dispose();					
					this.dropDownList = null;
				}

				if( this.dropDownContainer != null )
				{
					this.dropDownContainer.Dispose();
					this.dropDownContainer = null;
				}

				this.tableData.Dispose();
				this.tableData = null;

				this.DropDownClosed -= new PopupClosedEventHandler( dropDown_Closed );

				IDictionaryEnumerator controlEnumerator = this.targetEnabledMap.GetEnumerator();
				while( controlEnumerator.MoveNext() )
				{
					if( ( AutoCompleteModes )controlEnumerator.Value != AutoCompleteModes.Disabled )
					{
						Control editControl = ( Control )controlEnumerator.Key;

						UnadwiseControlEvents( editControl );
					}
				}

				if( this.dropDownList != null )
				{
					this.dropDownList.Dispose();
					this.dropDownList = null;
				}

                if( this.targetEnabledMap != null )
                {
                    this.targetEnabledMap.Clear();
					this.targetEnabledMap = null;
                }
                if( this.targetComboUpDownMap != null )
                {
                    this.targetComboUpDownMap.Clear();
				this.targetComboUpDownMap = null;
                }
                if( this.columnInfoList != null )
                {
                    this.columnInfoList.Clear();
					this.columnInfoList = null;				
                }
				this.dataSource = null;
				this.parentForm = null;
				CurrencyManager currencyManager = this.GetDataManager(this.dataSource) as CurrencyManager;
				if (currencyManager != null)
					currencyManager.ListChanged -= new ListChangedEventHandler(OnBindingListChanged);
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		public void BeginInit()
		{
			if( inInit )
			{
				throw new Exception( "BeginInit called twice." );
			}

			inInit = true;
		}

		/// <summary>
		/// Implementation of the <see cref="ISupportInitialize"/> interface.
		/// </summary>
		public void EndInit()
		{
			inInit = false;
			SetTableData();
			this.DeSerializeList();

			// Subscribe for DropDownClosed event to save DropDown size
			this.DropDownClosed += new PopupClosedEventHandler( dropDown_Closed );
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		ScrollersFrame scroll = new ScrollersFrame();
		/// <summary>
		/// Initializes the drop down list control with the list of matches.
		/// </summary>
		/// <returns>True if the drop down was properly initialized; false otherwise.</returns>
		private bool InitializeDropDown()
		{
			try
			{
				if( this.controlsInitialized == false )
				{
					if( this.Columns.Count == 0 )
					{
						if( this.tableData.Columns.Count > 0 )
						{
							this.RefreshColumns();
						}
						else
						{
							this.Columns.Add( new AutoCompleteDataColumnInfo( "", 150, true ) );
						}
					}

					if( this.dropDownList == null )
					{
						this.dropDownList = new VirtualListView();
					}
					this.dropDownList.ListOwner = this;
					this.dropDownList.TabStop = false;
                    this.dropDownList.ForeColor = this.foreColor;
					//this.dropDownList.ColumnInfoList = this.Columns;
					this.dropDownList.BorderStyle = BorderStyle.None;
					this.dropDownList.Location = Point.Empty;
					this.dropDownList.FullRowSelect = true;
					this.dropDownList.TabIndex = 0;
					this.dropDownList.View = View.Details;
					this.dropDownList.MultiSelect = false;
					this.dropDownList.Scrollable = true;
					this.dropDownList.HideSelection = false;
					if( this.ImageList != null )
					{
						this.dropDownList.SmallImageList = this.ImageList;
					}
					if( this.showColumnHeaderValue == false )
					{
						this.dropDownList.HeaderStyle = ColumnHeaderStyle.None;
					}
					else
					{
						this.dropDownList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
					}

					for( int i = 0; i < this.Columns.Count; i++ )
					{
                        AutoCompleteDataColumnInfo info = this.Columns[i];
                        if (info != null && !info.Visible)
                            continue;
						ColumnHeader column = new ColumnHeader();
						column.Tag = i;

						this.dropDownList.Columns.Add( column );
					}

					UpdateDropDownListWidth();

					if( this.dropDownContainer == null )
					{
						this.dropDownContainer = new SizablePopupControlContainer( this.dropDownList );
					}

					this.dropDownContainer.SuspendLayout();

					this.dropDownContainer.BorderStyle = BorderStyle.None;
					this.dropDownContainer.Location = new Point( 8, 16 );
					this.dropDownContainer.Name = "dropDownContainer";
					this.dropDownContainer.TabIndex = 0;
					this.dropDownContainer.ResumeLayout( false );

					this.dropDownContainer.Controls.Add( this.dropDownList );

					// Restore persisted DropDownSize state
					if( this.AutoPersistentDropDownSize )
					{
						LoadDropDownSizeState();
					}

					this.dropDownContainer.Popup += new EventHandler( this.HandlePopupDisplayed );
					this.dropDownContainer.CloseUp += new PopupClosedEventHandler( this.HandlePopupClosed );
					this.dropDownList.SelectedIndexChanged += new EventHandler( this.HandleAutoListSelectionChanged );
					this.dropDownList.DoubleClick += new EventHandler( this.HandleAutoListDoubleClick );
					this.dropDownList.Click += new EventHandler( this.HandleAutoListClick );
					this.dropDownList.KeyDown += new KeyEventHandler( HandleListControlKeyDown );
					this.controlsInitialized = true;
				}

				this.dropDownList.RightToLeft = this.targetRightToLeft;
				this.dropDownList.RightToLeftLayout = ( this.targetRightToLeft == RightToLeft.Yes );

				if( this.targetRightToLeft == RightToLeft.Yes )
				{
					this.dropDownList.Mirrored = true;
				}
				else
				{
					this.dropDownList.Mirrored = false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void UpdateDropDownListWidth()
		{
			int totalWidth = 0;

			foreach( ColumnHeader column in this.dropDownList.Columns )
			{
				int iCol = (int)column.Tag;				

				if( iCol != this.ImageColumnIndex )
				{
					AutoCompleteDataColumnInfo info = (AutoCompleteDataColumnInfo)this.Columns[iCol];

					if( info != null )
					{
						column.Text = info.ColumnHeaderText;
						column.Width = info.MinColumnWidth;
					}
					else
					{
						column.Text = "";
						column.Width = AutoComplete.minColumnWidth;
					}

					totalWidth += column.Width;
				}
				else
				{
					if( this.ImageList != null )
					{
						column.Width = 20;
						totalWidth += 20;
					}
				}				
			}
			
			if( this.PreferredWidth != -1 )
			{
				this.dropDownList.Width = this.PreferredWidth;
			}
			else
			{
				int controlWidth = this.GetActiveEditControl( this.ActiveFocusControl ).Width;
				UpdateDropDownListWidth(Math.Max(totalWidth, controlWidth));
			}
		}

		/// <summary>
		/// Updates the width of the DropDownList.
		/// </summary>
		/// <param name="width">Width of the DropDownList</param>
		protected virtual void UpdateDropDownListWidth(int width)
		{
			this.dropDownList.Width = width;
		}

		#endregion

		#region Class codedom serialization
		/// <summary></summary>
		/// <returns></returns>
		private bool ShouldSerializeImageList()
		{
			return this.ImageList != null;
		}

		/// <summary></summary>
		private void ResetImageList()
		{
			this.ImageList = null;
		}

		/// <summary>
		/// Indicates whether the ParentForm property is to be serialized.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeParentForm()
		{
			if( this.parentForm == null )
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Indicates whether the control needs AutoCompletion at this time.
		/// </summary>
		/// <param name="currentText">The current text.</param>
		/// <returns>True if the current state of the control does not
		/// require completion.</returns>
		protected virtual bool IgnoreCompletion( string currentText )
		{
			if( this.preChangeText == null )
			{
				this.preChangeText = "";
			}

			if( this.internalOverrideIgnoreCompletion == true )
			{
				return false;
			}

			if( this.preChangeText.Length >= currentText.Length )
			{
				return currentText == this.preChangeText.Substring( 0, currentText.Length );
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Populates the list with matches.
		/// </summary>
		/// <param name="currentText">The current text.</param>
		/// <param name="listDataView">The DataView object that has the list of matches.</param>
		protected virtual int PopulateListWithMatches( string currentText, ref DataView listDataView )
		{
			this.dropDownList.Refresh();
			DataRow matchingRow = null;
			return PopulateListWithMatches( currentText, ref listDataView, false, ref matchingRow );
		}

		/// <summary>
		/// Raises the TargetChanging event.
		/// </summary>
		[ DocumentationExclude() ]
		protected void OnTargetChanging( AutoCompleteTargetChangingEventArgs args )
		{
			try
			{
				if( this.TargetChanging != null )
				{
					this.TargetChanging( this, args );
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Raises the AutoCompleteCustomize event.
		/// </summary>
		[ DocumentationExclude() ]
		protected void OnAutoCompleteCustomize( AutoCompleteCustomizeEventArgs args )
		{
			try
			{
				if( this.AutoCompleteCustomize != null )
				{
					this.AutoCompleteCustomize( this, args );
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Filters the complete list of items to be considered for matches to form a 
		/// set of probable matches.
		/// </summary>
		/// <param name="currentText">The text to the matched.</param>
		/// <param name="listDataView">The matching items will be added to this ListView.</param>
		/// <returns>The count of the matches.</returns>
		/// <remarks>
		/// Override this function if you want to use a different matching routine
		/// from the internal built in matching routine.
		/// </remarks>
		protected virtual int PopulateListWithMatches( string currentText, ref DataView listDataView, bool exactMatch, ref DataRow matchingRow )
		{
			string rowFilterText = String.Empty;
			string sortText = String.Empty;

			if( this.tableData.Columns.Count == 0 )
			{
				this.SetTableData();
			}

			// PreMatchItemEvent
			currentText = this.RaisePreMatchItemEvent( currentText );

			// Replace characters for SQL
			currentText = this.FormatStringForSQL( currentText );

			int matchColumnIndex = 0;
			string matchColumnName = this.MatchColumnName;

			for( int i = 0 ; i < this.tableData.Columns.Count ; i++ )
			{
				if( matchColumnName == this.tableData.Columns[ i ].ColumnName )
				{
					matchColumnIndex = i;
					sortText = matchColumnName;
					break;
				}
			}

			if( this.MatchMode == AutoCompleteMatchModes.Automatic )
			{
				if( currentText.Length > 0 )
				{
					switch (GetControlAutoMode())
					{
						case AutoCompleteModes.MultiSuggest:
							listDataView = GetDataView(currentText, exactMatch, false);
							break;
						case AutoCompleteModes.MultiSuggestExtended:
							listDataView = GetDataView(currentText, exactMatch, true);
							break;
						default:
							if (this.tableData.Columns.Count > matchColumnIndex)
							{
								// changed to currentText.ToString as it blows up if a non string item comes in 04/25/02 DJ
								if (this.tableData.Columns[matchColumnIndex].DataType == currentText.GetType())
								{
									rowFilterText = "[" + this.tableData.Columns[matchColumnIndex].ColumnName + "]" + " LIKE '" + currentText.ToString();
									rowFilterText += exactMatch ? "'" :"*'";

									listDataView = new DataView(this.tableData, rowFilterText, null, DataViewRowState.CurrentRows);
								}
								else
								{
									this.OnInvalidDataType(new AutoCompleteErrorArgs(new Exception("Invalid data type for the matching column. Please change to a String data type column. See the MatchColumnIndex property.")));
								}
							}
							break;
					}
				}
				else
				{
					if( this.tableData.Columns.Count > 0 && exactMatch == false )
					{
						listDataView = new DataView(this.tableData, rowFilterText, null, DataViewRowState.CurrentRows);
					}
				}
			}	
			else
			{
				DataTable matchTable = this.tableData;

				//Manual match mode
				if( currentText.Length > 0 )
				{
					matchTable = this.tableData.Clone();

					foreach( DataRow itemRow in this.tableData.Rows )
					{
						string possibleMatch = ( string )itemRow.ItemArray[ 0 ];

						if( !this.MatchString( currentText, possibleMatch ) )
						{
							matchTable.ImportRow( itemRow );

							if( this.GetControlAutoMode() == AutoCompleteModes.AutoAppend )
							{
								break;
							}
						}
					}
				}

				listDataView = new DataView(matchTable, rowFilterText, null, DataViewRowState.CurrentRows);
			}

			if( listDataView == null )
			{
				listDataView = new DataView(this.tableData, rowFilterText, null, DataViewRowState.CurrentRows);
			}

			if (this.AutoSortList)
			{
				listDataView.Sort = sortText;
			}

			if( listDataView.Count > 0 && exactMatch == true )
			{
				matchingRow = listDataView[ 0 ].Row;
			}

            listDataView.Table.CaseSensitive = this.CaseSensitive;

			return listDataView.Count;
		}

		/// <summary>
		/// Sets the internal table data based on the <see cref="AutoComplete.DataSource"/> property.
		/// </summary>
		/// <remarks>
		/// This method is invoked by the <see cref="AutoComplete.DataSource"/> property when
		/// an external data source is specified. Override this method if you want to provide
		/// your own implenmentation for custom data sources. The <see cref="AutoComplete.TableData"/>
		/// property needs to be set with the appropriate data in this method.
		/// </remarks>
		public virtual void SetTableData()
		{
			try
			{
				// Get the CurrencyManager or Property Manager
				BindingManagerBase currencyManager = this.GetDataManager( this.dataSource );
				if( currencyManager != null )
				{
					if( currencyManager is CurrencyManager )
					{
						IList list = ( ( CurrencyManager )currencyManager ).List;

						this.tableData = new DataTable();

						this.tableData.Columns.Clear();
						this.tableData.Rows.Clear();

						// Find out what type of object this list holds

						object firstItem = null;
						if( list.Count > 0 )
						{
							firstItem = list[ 0 ];
						}

						if( firstItem != null && firstItem is String )
						{
							this.tableData.Columns.Add( "Column0" );
						}
						else
						{
							foreach( PropertyDescriptor p in currencyManager.GetItemProperties() )
							{
								this.tableData.Columns.Add( p.Name );
							}
						}

						// Just to make sure we do not have an empty list
						if( this.tableData.Columns.Count == 0 )
						{
							String columnText = "Column0";
							this.tableData.Columns.Add( columnText );
						}

						// Loop through the list and add the items to the DataTable
						foreach( object o in list )
						{
							DataRow newItem = this.tableData.NewRow();

							if( o is String )
							{
								newItem[ "Column0" ] = o.ToString();
							}
							else
							{
								foreach( PropertyDescriptor p in currencyManager.GetItemProperties() )
								{
									object oValue = p.GetValue( o );
									string sValue = String.Empty;

									if( oValue != null )
									{
										sValue = oValue.ToString();
									}

									newItem[ p.Name ] = sValue;
								}
							}

							this.tableData.Rows.Add( newItem );
						}
					}
				}
				else
				{
					this.tableData = new DataTable();

					this.tableData.Columns.Clear();
					this.tableData.Rows.Clear();

					foreach( AutoCompleteDataColumnInfo ac in this.Columns )
					{
						this.tableData.Columns.Add( ac.ColumnName );
					}

					if( this.tableData.Columns.Count == 0 )
					{
						this.tableData.Columns.Add( "Column0" );
					}

				}

			}
			catch( Exception e )
			{
				Console.WriteLine( e.Message );
				Console.WriteLine( e.GetType() );
			}

			if( this.tableData.Columns.Count > 0 )
			{
                this.tableData.CaseSensitive = true;
    
                int count = Math.Min( this.tableData.Columns.Count, DEF_MAX_KEY_COUNT );
				DataColumn[ ] primary = new DataColumn[count];

				for( int i = 0 ; i < count ; i++ )
				{
					primary[ i ] = this.tableData.Columns[ i ];
				}

                this.tableData.CaseSensitive = this.CaseSensitive;

				this.tableData.PrimaryKey = primary;
			}
		}

		/// <summary>
		/// Raises the DropDownDisplayed event.
		/// </summary>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnDropDownDisplayed( EventArgs args )
		{
			if( this.DropDownDisplayed != null )
			{
				this.DropDownDisplayed( this, args );
			}
		}

		/// <summary>
		/// Invokes the DropDownClosed Event.
		/// </summary>
		protected virtual void OnDropDownClosed( PopupClosedEventArgs args )
		{
			if( this.DropDownClosed != null )
			{
				this.DropDownClosed( this, args );
			}
		}

		/// <summary>
		/// Invokes the AutoCompleteItemSelected Event.
		/// <param name="args">An AutoCompleteSelectedEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnAutoCompleteSelected method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// 
		/// <note type="note">Inheritors:  When overriding OnAutoCompleteItemSelected in a derived
		/// class, be sure to call the base class's OnAutoCompleteSelected method so that
		/// registered delegates receive the event.</note>
		/// 
		/// </remarks>
		/// </summary>		
		protected virtual void OnAutoCompleteItemSelected( AutoCompleteItemEventArgs args )
		{
			try
			{
				if( AutoCompleteItemSelected != null )
				{
					AutoCompleteItemSelected( this, args );
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Raises the <see cref="AutoComplete.BeforeAddItem"/> event.
		/// </summary>
		/// <param name="args">A CancelEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnBeforeAddItem method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>
		/// Notes to Inheritors:  When overriding OnBeforeAddItem in a derived
		/// class, be sure to call the base class's OnBeforeAddItem method so that
		/// registered delegates receive the event.
		/// </para>
		/// </remarks>
		/// <example>
		///		<coderef file="tools\samples\editors package\autocompletedemo\VB\MainForm.cs" name="AutoComplete BeforeAddItem event" lang="C#">
		///		<code lang="C#">
		///             // Add a event to customize data added to history
		///             this.autoComplete1.BeforeAddItem += new AutoCompleteAddItemCancelEventHandler(this.autoComplete1_BeforeAddItem);
		/// 
		///             // autoComplete1_BeforeAddItem
		///             int columnCount = args.RowItem.Table.Columns.Count;
		///             object [] itemarray = args.RowItem.ItemArray;
		/// 
		///             string itemText = (string)itemarray[0];// the url field
		///             string nameText = (string)itemarray[1];// The name field
		/// 
		///             if(itemText.Substring(0,4) == "http")
		///             {
		///                 if(nameText == null || nameText == String.Empty)
		///                     nameText = "Website";
		///             }
		///             else if(itemText.Substring(0,3) == "ftp")
		///             {
		///                 if(nameText == null || nameText == String.Empty)
		///                     nameText = "FTP site";
		///             }
		///             else
		///                 args.Cancel = true;
		/// 
		///             itemarray[0] = itemText;
		///             itemarray[1] = nameText;
		///             args.RowItem.ItemArray = itemarray;
		///		</code>
		///		</coderef>
		///		<coderef file="tools\samples\editors package\autocompletedemo\VB\MainForm.vb" name="AutoComplete BeforeAddItem event" lang="VB">
		///		<code lang="VB">
		///            ' Add a event to customize data added to history
		///            AddHandler Me.autoComplete1.BeforeAddItem, New AutoCompleteAddItemCancelEventHandler(AddressOf autoComplete1_BeforeAddItem)
		///            ' autoComplete1_BeforeAddItem
		///            Dim columnCount As Integer
		///            columnCount = args.RowItem.Table.Columns.Count
		///            Dim itemarray() As Object
		///            itemarray = args.RowItem.ItemArray
		///            Dim itemText As String
		///            itemText = CType(itemarray(0), String)
		///            ' the url field
		///            Dim nameText As String
		///            nameText = CType(itemarray(1), String)
		///            ' The name field
		///            If (itemText.Substring(0, 4) Is "http") Then
		///                If ((nameText Is Nothing) _
		///                            OrElse (nameText Is String.Empty)) Then
		///                    nameText = "Website"
		///                End If
		///            Else
		///                If (itemText.Substring(0, 3) Is "ftp") Then
		///                    If ((nameText Is Nothing) _
		///                                OrElse (nameText Is String.Empty)) Then
		///                        nameText = "FTP site"
		///                    End If
		///                Else
		///                    args.Cancel = True
		///                End If
		///            End If
		///            itemarray(0) = itemText
		///            itemarray(1) = nameText
		///            args.RowItem.ItemArray = itemarray
		///		</code>
		///		</coderef>
		/// </example>
		protected virtual void OnBeforeAddItem( AutoCompleteAddItemCancelEventArgs args )
		{
			if( BeforeAddItem != null )
			{
				try
				{
					BeforeAddItem( this, args );
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// The AutoComplete control has the ability to display an icon next
		/// to a possible match item when displaying the drop down list
		/// in <see cref="AutoCompleteModes.AutoSuggest"/> mode. This overloaded
		/// version of AddHistoryItem adds an item to history and also 
		/// sets the image index for that item.
		/// </summary>
		/// <param name="newItemText">The string to be added to the history list.</param>
		/// <param name="imageIndexValue">The image index of the icon to be set for this
		/// item in the <see cref="AutoComplete.ImageList"/> assigned to this control.</param>
		public virtual void AddHistoryItem( string newItemText, int imageIndexValue )
		{
			string insertItem = String.Empty;
			string formattedInsertItem = String.Empty;

			if( this.tableData != null && this.tableData.Columns.Count == 0 )
			{
				this.SetTableData();
			}
			try
			{
				if( newItemText != null && newItemText != String.Empty )
				{
					insertItem = newItemText.Trim();

					formattedInsertItem = FormatStringForSQL( insertItem );

					if( this.tableData != null )
					{
						// Check if the entry already exists in the data table
						String selectText = this.tableData.Columns[ 0 ].ColumnName + " = '" + formattedInsertItem + "'";
						DataRow[ ] matchingRows = this.tableData.Select( selectText );

						if( matchingRows.Length <= 0 )
						{
							DataRow newRow = this.tableData.NewRow();
							newRow[ 0 ] = insertItem;

							for( int i = 1 ; i < tableData.Columns.Count ; ++i )
							{
								newRow[ i ] = String.Empty;
							}

							if( this.ImageColumnIndex > 0 && this.ImageColumnIndex < this.tableData.Columns.Count )
							{
								newRow[ this.ImageColumnIndex ] = imageIndexValue;
							}

							if( this.RaiseBeforeAddItemEvent( newRow ) == true )
							{
								return;
							}

							this.tableData.Rows.Add( newRow );
						}
					}
				}
			}
			catch( Exception e )
			{
				Debug.WriteLine( "AddHistoryItem Failed " + e.Message );
			}
		}

		/// <summary>
		/// Overloaded. The AutoComplete control can display sub items with columns and also
		/// an icon for possible matches when in <see cref="AutoCompleteModes.AutoSuggest"/>
		/// mode.
		/// <para>
		/// This method sets the associated icon for any item already in 
		/// history. 
		/// </para>
		/// </summary>
		/// <param name="itemKey">The item key identifier string.</param>
		/// <param name="imageIndexValue">The Image Index.</param>
		/// <returns>True if the item is added to the histor list; false otherwise.</returns>
		public virtual bool SetHistoryItem( string itemKey, int imageIndexValue )
		{
			string setItem = itemKey.Trim();
			String selectText = this.tableData.Columns[ 0 ].ColumnName + " = " + setItem;
			DataRow[ ] matchingRows = this.tableData.Select( selectText );

			if( matchingRows.Length > 0 )
			{
				DataRow currentRow = matchingRows[ 0 ];
				if( this.ImageColumnIndex > 0 && this.ImageColumnIndex < this.tableData.Columns.Count )
				{
					currentRow[ this.ImageColumnIndex ] = imageIndexValue;
				}
			}

			return true;
		}

		/// <summary>
		/// The AutoComplete control can display sub items with columns and also
		/// an icon for possible matches when in <see cref="AutoCompleteModes.AutoSuggest"/>
		/// mode.
		/// <para>
		/// This method sets the associated icon for any item already in
		/// history. 
		/// </para>
		/// </summary>
		/// <param name="itemKey">The item key identifier string.</param>
		/// <param name="subindex">The sub item index - starts at zero.</param>
		/// <param name="subItemValue">The sub item value.</param>
		/// <returns>True if the sub item was set successfully; false otherwise.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The sub item index was out of range.</exception>
		public virtual void SetHistoryItem( string itemKey, int subindex, string subItemValue )
		{
			string setItem = itemKey.Trim();
			String selectText = this.tableData.Columns[ 0 ].ColumnName + " = " + setItem;
			DataRow[ ] matchingRows = this.tableData.Select( selectText );

			if( matchingRows.Length > 0 )
			{
				DataRow currentRow = matchingRows[ 0 ];
				if( subindex < this.tableData.Columns.Count )
				{
					currentRow[ subindex ] = subItemValue;
				}
			}
		}

		/// <summary>
		/// Indicates whether the current text is a substring of the possible match. 
		/// Does the match between the current text and a possible match.
		/// This can be overriden to provide an implementation that does
		/// something more specific. 
		/// </summary>
		/// <param name="currentText">The text to be completed.</param>
		/// <param name="possibleMatch">The possible match string.</param>
		/// <returns>True if the possible match string is an acceptable match; false otherwise.</returns>
		protected virtual bool MatchString( string currentText, string possibleMatch )
		{
			return RaiseMatchItemEvent( currentText, possibleMatch );
		}

		/// <summary>
		/// Invokes the OnMatchItem Event.
		/// <param name="arg">The AutoCompleteMatchItem event data.</param>
		/// <remarks>
		/// The OnMatchItem method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// 
		/// <note type="note">Inheritors:  When overriding OnMatchItem in a derived
		/// class, be sure to call the base class's OnMatchItem method so that
		/// registered delegates receive the event.</note>
		/// 
		/// </remarks>
		/// </summary>
		protected virtual void OnMatchItem( AutoCompleteMatchItemEventArgs arg )
		{
			try
			{
				if( this.MatchItem != null )
				{
					this.MatchItem( this, arg );
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Invokes the AutoCompleteItemBrowsed event.
		/// <param name="args">An AutoCompleteSelectedEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnAutoCompleteItemBrowsed method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// 
		/// <note type="note">Inheritors:  When overriding OnAutoCompleteItemBrowsed in a derived
		/// class, be sure to call the base class's OnAutoCompleteItemBrowsed method so that
		/// registered delegates receive the event.</note>
		/// </remarks>
		/// </summary>		
		protected virtual void OnAutoCompleteItemBrowsed( AutoCompleteItemEventArgs args )
		{
			try
			{
				if( AutoCompleteItemBrowsed != null )
				{
					AutoCompleteItemBrowsed( this, args );
				}
			}
			catch
			{
			}
		}

		[ DocumentationExclude() ]
		protected void OnPreMatchItemEvent( AutoCompletePreMatchItemEventArgs args )
		{
			try
			{
				if( this.PreMatchItem != null )
				{
					this.PreMatchItem( this, args );
				}
			}
			catch
			{
			}
		}

		[ DocumentationExclude() ]
		protected void OnInvalidDataType( AutoCompleteErrorArgs args )
		{
			try
			{
				if( this.InvalidDataType != null )
				{
					this.InvalidDataType( this, args );
				}
			}
			catch
			{
			}
		}
		#endregion

		#region Class event handlers
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dropDown_Closed( object sender, PopupClosedEventArgs e )
		{
			if( this.AutoPersistentDropDownSize )
			{
				SaveDropDownSizeState();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private void OnBindingListChanged(object sender, ListChangedEventArgs e)
		{
			this.SetTableData();
		}

		/// <summary>
		/// Handles the KeyDown event of the list view control.
		/// </summary>
		/// <param name="sender">The list control.</param>
		/// <param name="e">The event data.</param>
		private void HandleListControlKeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Up )
			{
				if( dropDownList.ItemsCount > 0 && dropDownList.IsItemSelected( 0 ) )
				{
					Control editControl = this.GetActiveEditControl( this.ActiveFocusControl );

					if( null != editControl && !this.AutoCompletePopup.IsShowing() )
					{
						editControl.Focus();
					}
				}
			}
			else if( e.KeyCode == Keys.Delete && this.AllowListDelete && this.IsDropDownShowing() )
			{
				try
				{
					this.SelectedIndex = this.dropDownList.GetSelectedIndex();
					int matchColumnIndex = 0, headerIndex = 0;

					string matchColumnName = this.GetMatchColumnHeaderText();
					foreach( AutoCompleteDataColumnInfo column in this.Columns )
					{
						if( column.ColumnName == matchColumnName )
						{
							matchColumnIndex = headerIndex;
							break;
						}
						headerIndex++;
					}

					string strExp = this.tableData.Columns[ matchColumnIndex ].ColumnName + " = '" +
						this.dropDownList.GetSelectedItemText() + "'";
					string strSort = this.tableData.Columns[ matchColumnIndex ].ColumnName;
					DataRow[ ] foundRows = this.tableData.Select( strExp, strSort, DataViewRowState.CurrentRows );

					this.tableData.Rows.Remove( foundRows[ 0 ] );
					this.ProcessAutoComplete( this.preChangeText );

                    //if (dropDownList.ItemsCount > 0)
                    //{
                    //    dropDownList.ItemsCount--;
                    //}

					if( this.dropDownList.ItemsCount > this.SelectedIndex - 1 )
					{
						this.dropDownList.SelectItem( Math.Max( 0, this.SelectedIndex - 1 ), true );
					}
				}
				catch( Exception ex )
				{
					Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
				}
			}
		}

		/// <summary>
		/// Handles the KeyDown event of the target control.
		/// </summary>
		/// <param name="sender">The control with the focus.</param>
		/// <param name="e">The key event data.</param>
		private void HandleControlKeyDown( object sender, KeyEventArgs e )
		{
			if( Keys.Alt != e.Modifiers )
			{
				HandleControlKeyDownNoModifiers( sender, e );
                HandleListControlKeyDown(sender, e);
			}
		}

		private void HandleControlKeyDownNoModifiers( object sender, KeyEventArgs e )
		{
            Control editControl = (Control)sender;

			if( e.KeyCode == Keys.Down )
			{
				if( this.IsDropDownShowing() )
				{
					if(dropDownList.ItemsCount > 0)
					{
                        if( this.dropDownList.SelectedIndices.Count > 0 )
                            this.dropDownList.SelectItem( this.dropDownList.GetSelectedIndex() + 1, true );
                        else
                            this.dropDownList.SelectItem( 0, true );

                        e.Handled = true;
					}
				}
				else
				{
					this.internalOverrideIgnoreCompletion = true;
					this.comboUpDownState = 1; //Down
					this.ProcessAutoComplete( editControl.Text );
					this.internalOverrideIgnoreCompletion = false;
				}
			}
			else if( e.KeyCode == Keys.Up )
			{
                if (this.IsDropDownShowing())
                {
                    if (this.dropDownList.GetSelectedIndex() > 0 && this.dropDownList.GetSelectedIndex() != 0)
                        this.dropDownList.SelectItem(this.dropDownList.GetSelectedIndex() - 1, true);

                    e.Handled = true;
                }
                else 
                {
                    this.internalOverrideIgnoreCompletion = true;
                    this.comboUpDownState = 2; //UP
                    this.ProcessAutoComplete(editControl.Text);
                    this.internalOverrideIgnoreCompletion = false;
                }
			}
			else if( e.KeyCode == Keys.Escape )
			{
				CloseDropDownCancel();
			}
			else if( e.KeyCode == Keys.Enter )
			{
                isEnterHit = true;
				AddItemOnValidate( editControl );
                isEnterHit = false;
				if( this.IsDropDownShowing() )
				{
					this.dropDownContainer.HidePopup( PopupCloseType.Done );
				}
			}
			else if( e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown )
			{
				if( this.IsDropDownShowing() )
				{
					NativeMethods.PostMessage( this.dropDownList.Handle, NativeMethods.WM_KEYDOWN,
						new IntPtr( (int)e.KeyCode ), new IntPtr( NativeMethods.MAKELPARAM( 1, 0 ) ) );
				}
			}
		}

		/// <summary>
		/// This is an event handler that responds to the ControlEnter
		/// event.  We attach this to each control we are providing AutoComplete
		/// text for.
		/// </summary>
		/// <param name="sender">The control with the current focus.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>This method is the handler for the Enter event of the target
		/// control. All controls that are provided with auto completion get their
		/// Enter event handled to provide a way for the AutoComplete control
		/// to hook into the other events for the edit control.</remarks>
		private void HandleControlEnter( object sender, EventArgs e )
		{
			if( this.ActiveFocusControl != ( Control )sender )
			{
				this.ActiveFocusControl = ( Control )sender;
				Control editControl = this.GetActiveEditControl( this.ActiveFocusControl );

				if( editControl != null )
				{
					this.RaiseTargetChangingEvent( editControl, false );

					editControl.TextChanged += new EventHandler( HandleControlTextChanged );
					editControl.KeyDown += new KeyEventHandler( HandleControlKeyDown );
					editControl.DoubleClick += new EventHandler( HandleControlDoubleClick );
					editControl.Click += new EventHandler( HandleControlClick );
					editControl.Validated += new EventHandler( HandleControlValidated );
					editControl.HandleDestroyed += new EventHandler( HandleControlDestroyed );

					targetRightToLeft = editControl.RightToLeft;

					if( editControl is ComboBox )
					{
						( ( ComboBox )editControl ).SelectionChangeCommitted += new EventHandler( HandleControlSelectionChangeCommitted );
						( ( ComboBox )editControl ).DropDown += new EventHandler( HandleControlDropDown );
					}
                    else
                    {
                        editControl.MouseWheel += new MouseEventHandler( HandleControlMouseWheel );
                    }
				}
			}

		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleControlMouseWheel( object sender, MouseEventArgs e )
        {
            if( e.Delta != 0 )
			{
				if( this.dropDownList != null && this.dropDownList.IsHandleCreated && this.dropDownList.Visible )
				{
                    int delta = e.Delta;
                    IntPtr wParam = ( IntPtr ) NativeMethods.MAKELONG( 0, delta );
				
                    NativeMethods.SendMessage( this.dropDownList.Handle, NativeMethods.WM_MOUSEWHEEL, wParam, IntPtr.Zero );
				}
			}
        }

		/// <summary>
        /// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleControlDestroyed( object sender, EventArgs e )
		{
			if( this.dropDownContainer != null )
			{
				this.dropDownContainer.ParentControl = null;
			}
		}

		/// <summary>
		/// This is an event handler that responds to the ControlLeave
		/// event of the current control.  
		/// </summary>
		/// <param name="sender">The control with the current focus</param>
		/// <param name="e">The event data.</param>
		/// <remarks>Handles the ControlLeave event and unwires the events 
		/// that we hooked into in the <see cref="HandleControlEnter"/> event.</remarks>
		private void HandleControlLeave( object sender, EventArgs e )
		{
			if( sender == this.ActiveFocusControl )
			{
				Control editControl = this.GetActiveEditControl( this.ActiveFocusControl );

				if( editControl != null )
				{
					this.RaiseTargetChangingEvent( editControl, true );

					editControl.TextChanged -= new EventHandler( HandleControlTextChanged );
					editControl.KeyDown -= new KeyEventHandler( HandleControlKeyDown );
					editControl.DoubleClick -= new EventHandler( HandleControlDoubleClick );
					editControl.Click -= new EventHandler( HandleControlClick );
					//editControl.Validated -=  new EventHandler(HandleControlValidated);// Prevents HandleControlValidated from being called
					editControl.HandleDestroyed -= new EventHandler( HandleControlDestroyed );

					if( editControl is ComboBox )
					{
						( ( ComboBox )editControl ).SelectionChangeCommitted -= new EventHandler( HandleControlSelectionChangeCommitted );
						( ( ComboBox )editControl ).DropDown -= new EventHandler( HandleControlDropDown );
					}
                    else
                    {
                        editControl.MouseWheel -= new MouseEventHandler( HandleControlMouseWheel );
                    }

                    if( this.dropDownContainer.IsShowing() && this.dropDownList.GetSelectedItemText() != "" )
                    {
                        editControl.Text = this.dropDownList.GetSelectedItemText();

                        int matchColumnIndex = 0;
                        string matchColumnName = this.MatchColumnName;

                        if( this.tableData != null && this.tableData.Columns != null )
			            {
                            for( int i = 0 ; i < this.tableData.Columns.Count ; i++ )
			                {
				                if( matchColumnName == this.tableData.Columns[ i ].ColumnName )
				                {
					                matchColumnIndex = i;
					                break;
				                }
			                }
                        }

                        this.PopulateListWithMatches( editControl.Text, ref listDataView );
                        DataRowView rowView = this.GetAutoAppendEntry( editControl.Text, listDataView, matchColumnIndex );

                        if( rowView != null && rowView.Row != null )
                        {
                            object[] itemArray = rowView.Row.ItemArray;
                            string newSelectedValue = String.Empty;
                            string matchText = ( string ) rowView [ matchColumnIndex ].ToString();

                            this.SetSelectedItem( new AutoCompleteItem( itemArray, matchColumnIndex ) );
                            this.RaiseAutoCompleteItemSelectedEvent( itemArray, matchColumnIndex, matchText, ref newSelectedValue );
                        }
                    }
				}
				this.ActiveFocusControl = null;
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the current control. The ProcessAutoComplete
		/// method is invoked if there is a change in the control's text content.
		/// </summary>
		/// <seealso cref="ProcessAutoComplete(string)"/>
		/// 
		/// <param name="sender">The control being provided with AutoComplete.</param>
		/// <param name="e">The Event Args.</param>
		/// <remarks>This is the core of the auto completion process where the TextChanged
		/// event from the edit control is handled and the auto complete tries to
		/// provide auto completion.</remarks>
		private void HandleControlTextChanged( object sender, EventArgs e )
		{
			Control editControl = ( Control )sender;
			string currentText = editControl.Text;
			this.dropDownContainer.ParentControl = editControl;

			if( editControl is TextBox )
			{
				if( ( ( TextBox )editControl ).Multiline == true )
				{
					//TODO
				}
			}

			if( this.selectionChangeCommitted == false )
			{
				if( IgnoreChangeMessage++ <= 0 )
				{
                    if (StartProcess(bProcess))
					    this.ProcessAutoComplete( currentText );
				}
				IgnoreChangeMessage--;

			}
			else
			{
				this.selectionChangeCommitted = false;
			}
		}

        /// <summary>
        /// Indicates whether autocomplete process should start.
        /// </summary>
        /// <param name="process">if set to <c>true</c> process autocomplete.</param>
        /// <returns></returns>
        /// <remarks>
        /// If AutoComplete should be processed after typing some text, it should be set 
        /// to false on load, and then set to true after typing the text.
        /// </remarks>
        public bool StartProcess(bool process)
        {
            bProcess = process;
            return bProcess;
        }
       
		/// <summary>
		/// Handles the SelectionChangeCommitted event of the current control.
		/// </summary>
		/// 
		/// <param name="sender">The control being provided with AutoComplete.</param>
		/// <param name="e">The Event Args.</param>
		/// <remarks>This event is handled to prevent a problem that appears when
		/// auto completion is applied to a ComboBox. When the user selects an
		/// item in the drop down list of the ComboBox, the TextChanged event
		/// is raised and the auto completion process starts in response to this.
		/// This should be avoided and the workaround is to handle the SelectionChanged
		/// event that's raised by the ComboBox when the selection changes and suppress
		/// the TextChanged event that is raised right after.</remarks>
		private void HandleControlSelectionChangeCommitted( object sender, EventArgs e )
		{
			this.selectionChangeCommitted = true;
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleControlDropDown( object sender, EventArgs e )
		{
			Control editControl = sender as Control;

			if( this.OverrideCombo == false && this.IsDropDownShowing() )
			{
				CloseDropDownCancel();
			}
			else if( this.OverrideCombo == true && this.IsDropDownShowing() == false )
			{
				this.ProcessAutoComplete( editControl.Text );
			}

		}

		/// <summary>
		/// Displays the popup control when the user DoubleClicks in the target
		/// control.
		/// </summary>
		/// <param name="sender">The control with the focus.</param>
		/// <param name="e">The event data.</param>
		private void HandleControlDoubleClick( object sender, EventArgs e )
		{
			Control editControl = ( Control )sender;

			if( this.IsDropDownShowing() )
			{
				CloseDropDownCancel();
			}
			else
			{
				this.ProcessAutoComplete( editControl.Text );
			}
		}

		/// <summary>
		/// Displays the popup control when the user Clicks on the target
		/// control.
		/// </summary>
		/// <param name="sender">The control with the focus.</param>
		/// <param name="e">The event data.</param>
		private void HandleControlClick( object sender, EventArgs e )
		{
			Control editControl = ( Control )sender;

			if( this.SingleClick )
			{
				if( this.IsDropDownShowing() )
				{
					CloseDropDownCancel();
				}
				else if( this.GetAutoComplete( editControl ) != AutoCompleteModes.AutoAppend )
				{
					this.ProcessAutoComplete( editControl.Text );
				}
			}
		}

		/// <summary>
		/// Handles the DoubleClick event of the drop down list.
		/// </summary>
		/// <param name="sender">The drop down list.</param>
		/// <param name="e">The EventArgs object with event data.</param>
		private void HandleAutoListDoubleClick( object sender, EventArgs e )
		{
			if( this.IsDropDownShowing() )
			{
				this.dropDownContainer.HidePopup( PopupCloseType.Done );
			}
		}

		/// <summary>
		/// Handles the Click event of the drop down list.
		/// </summary>
		/// <param name="sender">The drop down list.</param>
		/// <param name="e">The EventArgs object with event data.</param>
		private void HandleAutoListClick( object sender, EventArgs e )
		{
			if( this.IsDropDownShowing() )
			{
				this.dropDownContainer.HidePopup( PopupCloseType.Done );
			}
		}

		/// <summary>
		/// Handles the SelectionChanged event of the drop down list.
		/// </summary>
		/// <param name="sender">The drop down list.</param>
		/// <param name="e">The EventArgs object with event data.</param>
		private void HandleAutoListSelectionChanged( object sender, EventArgs e )
		{
			//string selectedValue = String.Empty;
			int matchColumnIndex = 0;
			int headerIndex = 0;

			string matchColumnName = this.GetMatchColumnHeaderText();

			foreach( AutoCompleteDataColumnInfo column in this.Columns )
			{
				if( column.ColumnName == matchColumnName )
				{
					matchColumnIndex = headerIndex;
					break;
				}
				headerIndex++;
			}

			if (this.dropDownList.SelectedIndices.Count > 0)
			{
				try
				{
					this.SelectedIndex = this.dropDownList.SelectedIndices[0];
					object[] itemArray = this.listDataView[this.SelectedIndex].Row.ItemArray;
					this.RaiseAutoCompleteItemBrowsedEvent(itemArray, matchColumnIndex);
				}
				catch
				{
					// if the item has been deleted by AllowListDelete
				}
			}
		}

		/// <summary>
		/// The handler for the Validated event of the edit control.
		/// The current text of the edit control will be added to the history
		/// list if the <see cref="AutoComplete.AutoAddItem"/> property is set.
		/// </summary>
		/// <param name="sender">The edit control with the focus.</param>
		/// <param name="e">The event data.</param>
		protected void HandleControlValidated( object sender, EventArgs e )
		{
			AddItemOnValidate( ( Control )sender );
		}

		/// <summary>
		/// Handles the PopupClosed event of the drop down control.
		/// </summary>
		/// <param name="sender">The drop down container.</param>
		/// <param name="e">The EventArgs object with event data.</param>
		private void HandlePopupClosed( object sender, PopupClosedEventArgs e )
		{
			this.dropDownContainer.ParentControl = null;

			try
			{
				if( e.PopupCloseType == PopupCloseType.Done )
				{
					string selectedValue = String.Empty;
					int matchColumnIndex = 0;
					int headerIndex = 0;

					string matchColumnName = this.GetMatchColumnHeaderText();

					foreach( AutoCompleteDataColumnInfo column in this.Columns )
					{
						if( column.ColumnName == matchColumnName )
						{
							matchColumnIndex = headerIndex;
							break;
						}
						headerIndex++;
					}

					// Added to accomodate for Both Mode in which case no entry
					// would be selected in the drop down 07/11/02 DJ
					if( this.dropDownList.SelectedItemsCount < 1 && this.dropDownList.ItemsCount > 0 )
					{
						this.dropDownList.SelectItem( 0, true );
					}

					this.SelectedIndex = this.dropDownList.GetSelectedIndex();

					if( this.dropDownList.SelectedIndices.Count > 0 )
					{
						object[ ] itemArray = this.listDataView[ this.SelectedIndex ].Row.ItemArray;

						if( matchColumnIndex == 0 )
						{
							selectedValue = dropDownList.GetSelectedItemText();
						}
						else
						{
							int matchIndexInDataTable = GetMatchColumnIndexInDataTable( this.MatchColumnName );
							selectedValue = ( string )itemArray[ matchIndexInDataTable ].ToString();
						}

						BindingManagerBase dataManager = this.GetDataManager( DataSource );

						int actualIndex = this.GetActualRowIndex( listDataView, this.SelectedIndex, matchColumnIndex );
                        if (dataManager != null && dataManager.Position != actualIndex && actualIndex != -1 && e.PopupCloseType != PopupCloseType.Done)
						{
							dataManager.Position = actualIndex;
						}

						string newSelectedValue = String.Empty;
						//Set the SelectedItem
						this.SetSelectedItem( new AutoCompleteItem( itemArray, matchColumnIndex ) );

						bool handled = this.RaiseAutoCompleteItemSelectedEvent( itemArray, matchColumnIndex, selectedValue, ref newSelectedValue );
						if( handled )
						{
							this.SetSelectedValue( newSelectedValue );
						}
						else
						{
							this.SetSelectedValue( selectedValue );
						}
					}

				}
			}
			catch( Exception exception )
			{
				Console.WriteLine( exception.Message );
				//				throw exception;
			}
			this.OnDropDownClosed( e );
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandlePopupDisplayed( object sender, EventArgs e )
		{
			this.OnDropDownDisplayed( e );
			if( this.indexToEnsureVisible != -1 && this.dropDownList.ItemsCount > this.indexToEnsureVisible )
			{
				NativeMethods.SendMessage( dropDownList.Handle, NativeMethods.LVM_ENSUREVISIBLE,
					indexToEnsureVisible, IntPtr.Zero );

				this.dropDownList.SelectItem( indexToEnsureVisible, true );
				this.indexToEnsureVisible = -1;
			}

            NativeMethods.SetWindowPos(this.dropDownContainer.PopupHost.Handle, new IntPtr(NativeMethods.HWND_TOPMOST), 0, 0, 0, 0, (int)(NativeMethods.SetWindowPosFlags.SWP_NOMOVE | NativeMethods.SetWindowPosFlags.SWP_NOSIZE | NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE));
		}

        private bool bProcessingAutoComplete = false;
        internal bool ProcessingAutoComplete
        {
            get { return bProcessingAutoComplete; }
        }
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Initiates the AutoComplete process. This is
		/// called automatically when the AutoCompleteMode is not set to
		/// Disabled.
		/// <para>This method invokes the <see cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/>
		/// method to get a list of matches for the current text. Override the
        /// <see cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/> method
		/// if you are providing your own history source.</para>
		/// </summary>
		/// <param name="currentText">The current text content of the edit control.</param>
        /// <seealso cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/>
		/// <param name="selectedEntry">Specifies the entry that should be shown as selected.</param>
		public void ProcessAutoComplete( string currentText, string selectedEntry, Point location )
		{
            bProcessingAutoComplete = true;

			DataView displayDataView = null;
			bool showDropDown = true;
			int matchColumnIndex = 0;
			AutoCompleteModes mode = this.GetControlAutoMode();
			bool comboUpDownMode = this.GetControlComboUpDownMode();
			DataRowView rowView = null;

			// Raise the AutoCompleteCustomize event
			AutoCompleteCustomizeEventArgs args = new AutoCompleteCustomizeEventArgs( currentText, location, this.ActiveFocusControl );
			this.OnAutoCompleteCustomize( args );
			if( args.Handled == true )
			{
				if( args.Cancel )
				{
					return;
				}

				currentText = args.TextForAutoCompletion;
				location = args.AutoSuggestLocation;
			}

			this.currentTextBeingMatched = currentText;
			string matchColumnName = this.MatchColumnName;

			for( int i = 0 ; i < this.tableData.Columns.Count ; i++ )
			{
				if( matchColumnName == this.tableData.Columns[ i ].ColumnName )
				{
					matchColumnIndex = i;
					break;
				}
			}

			Control editControl = this.GetActiveEditControl( this.ActiveFocusControl );
			if( editControl == null )
			{
				return;
			}

			ComboBox comboBox = null;
			TextBoxBase textBox = null;

			this.dropDownContainer.ParentControl = editControl;

			if( editControl is ComboBox )
			{
				if( ( ( ComboBox )editControl ).DroppedDown == true )
				{
					showDropDown = false;
				}

				comboBox = ( ComboBox )editControl;
			}
			else
			{
				textBox = ( TextBoxBase )editControl;
			}

			if( comboUpDownMode == true && ( mode == AutoCompleteModes.AutoAppend || mode == AutoCompleteModes.Both ) && this.comboUpDownState > 0 )
			{
				int itemsCount = this.PopulateListWithMatches( "", ref listDataView );

				if( itemsCount > 0 )
				{
					if( this.comboUpDownState == 1 )
					{
						rowView = GetAutoAppendEntryPrevious( currentText, listDataView, matchColumnIndex );
					}
					else if( this.comboUpDownState == 2 )
					{
						rowView = GetAutoAppendEntryNext( currentText, listDataView, matchColumnIndex );
					}

					if( this.comboUpDownState > 0 )
					{
						string matchText = String.Empty;

						if( editControl is ComboBox )
						{
							IgnoreChangeMessage++;
							matchText = ( string )rowView[ matchColumnIndex ].ToString();
							if( CaseSensitive )
							{
								if( currentText.Length > 0 && matchText.Length >= currentText.Length )
								{
									matchText = currentText + matchText.Substring( currentText.Length );
								}
							}

							comboBox.Text = matchText;
							IgnoreChangeMessage--;
							comboBox.SelectionStart = 0;
							comboBox.SelectionLength = comboBox.Text.Length;
						}
						else
						{
							IgnoreChangeMessage++;
							matchText = ( string )rowView[ matchColumnIndex ].ToString();
							if( CaseSensitive )
							{
								if( currentText.Length > 0 && matchText.Length >= currentText.Length )
								{
									matchText = currentText + matchText.Substring( currentText.Length );
								}
							}
							textBox.Text = matchText;
							IgnoreChangeMessage--;
							textBox.SelectionStart = 0;
							textBox.SelectionLength = textBox.TextLength;
						}

						object[ ] itemArray = rowView.Row.ItemArray;
						string newSelectedValue = String.Empty;

						//Set the SelectedItem
						this.SetSelectedItem( new AutoCompleteItem( itemArray, matchColumnIndex ) );
						bool handled = this.RaiseAutoCompleteItemSelectedEvent( itemArray, matchColumnIndex, matchText, ref newSelectedValue );
					}
				}
			}
			else
			{
				this.InitializeDropDown();
				if (showDropDown && this.PopulateListWithMatches(currentText, ref listDataView) > 0)
				{
					if( mode == AutoCompleteModes.AutoSuggest || mode >= AutoCompleteModes.Both )
					{
						int selectionStart = 0;
						int selectionLength = 0;

						if( editControl is ComboBox )
						{
							selectionStart = ( ( ComboBox )editControl ).SelectionStart;
							selectionLength = ( ( ComboBox )editControl ).SelectionLength;
						}
						else
						{
							selectionStart = ( ( TextBoxBase )editControl ).SelectionStart;
							selectionLength = ( ( TextBoxBase )editControl ).SelectionLength;
						}

						// The DataView filled by PopulateListWithMatches will not hide the
						// invisible columns - this call will drop the invisible columns and
						// create a new DataView
						displayDataView = this.CreateDisplayDataView( listDataView, this.Columns );
						this.FillDropDownList( displayDataView, selectedEntry, matchColumnIndex );

                        if( !this.dropDownContainer.IsShowing() )
                        {
                            this.UpdateDropDownListWidth();
                            this.dropDownContainer.ShowPopup(location);
                            editControl.Focus();
                        }
                        else
                        {
                            //for resizing the popup to its default size when it showing.
                            //this.dropDownContainer.AdjustPopupHostBounds();
                        }

						if( editControl is ComboBox )
						{
							( ( ComboBox )editControl ).SelectionStart = selectionStart;
							( ( ComboBox )editControl ).SelectionLength = selectionLength;
						}
						else
						{
							( ( TextBoxBase )editControl ).SelectionStart = selectionStart;
							( ( TextBoxBase )editControl ).SelectionLength = selectionLength;
						}
					}

					if( mode == AutoCompleteModes.AutoAppend || mode == AutoCompleteModes.Both )
					{
						if( !this.IgnoreCompletion( currentText ) )
						{
							string matchText = String.Empty;
							if( editControl is ComboBox )
							{
								rowView = GetAutoAppendEntry( currentText, listDataView, matchColumnIndex );
								IgnoreChangeMessage++;
								matchText = ( string )rowView[ matchColumnIndex ].ToString();
								if( CaseSensitive )
								{
									if( currentText.Length > 0 && matchText.Length >= currentText.Length )
									{
										matchText = currentText + matchText.Substring( currentText.Length );
									}
								}
								comboBox.Text = matchText;
								comboBox.SelectionStart = currentText.Length;
								IgnoreChangeMessage--;
								comboBox.SelectionLength = comboBox.Text.Length - comboBox.SelectionStart;
							}
							else
							{
								rowView = GetAutoAppendEntry( currentText, listDataView, matchColumnIndex );
								IgnoreChangeMessage++;
								matchText = ( string )rowView[ matchColumnIndex ].ToString();
								if( CaseSensitive )
								{
									if( currentText.Length > 0 && matchText.Length >= currentText.Length )
									{
										matchText = currentText + matchText.Substring( currentText.Length );
									}
								}
								textBox.Text = matchText;
								IgnoreChangeMessage--;
								textBox.SelectionStart = currentText.Length;
								textBox.SelectionLength = textBox.TextLength - currentText.Length;
							}

							object[ ] itemArray = rowView.Row.ItemArray;
							string newSelectedValue = String.Empty;
							//Set the SelectedItem
							this.SetSelectedItem( new AutoCompleteItem( itemArray, matchColumnIndex ) );
							this.RaiseAutoCompleteItemSelectedEvent( itemArray, matchColumnIndex, matchText, ref newSelectedValue );
						}
					}
				}
				else
				{
					CloseDropDownCancel();
				}
			}
			this.preChangeText = currentText;
			this.comboUpDownState = 0;

            bProcessingAutoComplete = false;
		}

		/// <summary>
		/// Indicates whether the popup control is currently being displayed.
		/// </summary>
		/// <returns>True if the drop down list is being displayed; false otherwise.</returns>
		public bool IsDropDownShowing()
		{
			if( this.dropDownContainer != null )
			{
				return this.dropDownContainer.IsShowing();
			}

			return false;
		}

		/// <summary>
		/// Returns the number of probable matches for a particular
		/// string in the AutoComplete control's current history list or data source.
		/// </summary>
		/// <param name="currentText">The text to use for the matching.</param>
		/// <returns>The count of items that match.</returns>
		public int GetMatchesCount( string currentText )
		{
			DataView dataView = null;
			return this.PopulateListWithMatches( currentText, ref dataView );
		}

		/// <summary>
		/// Initiates the AutoComplete process. This is
		/// called automatically when the AutoCompleteMode is not set to
		/// Disabled.
        /// <para>This method invokes the <see cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/>
		/// method to get a list of matches for the current text. Override the
        /// <see cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/> method
		/// if you are providing your own history source.</para>
		/// </summary>
		/// <param name="currentText">The current text content of the edit control.</param>
        /// <seealso cref="AutoComplete.PopulateListWithMatches(string, ref DataView)"/>
		public void ProcessAutoComplete( string currentText )
		{
			this.ProcessAutoComplete( currentText, String.Empty, Point.Empty );
 			if( this.dropDownContainer.IsShowing())
			this.dropDownContainer.AdjustPopupHostBounds();
		}

		/// <summary>
		/// Allows you to open the auto complete list in your code.
		/// </summary>
		/// <remarks>AutoComplete control requires that the control that needs to be provided with the AutoCompletion services have the focus first before the ProcessAutoComplete is called.</remarks>
		/// <param name="currentText">The current text content of the edit control.</param>
		public void ProcessAutoComplete( string currentText, string selectedEntry )
		{
			this.ProcessAutoComplete( currentText, selectedEntry, Point.Empty );
		}

		/// <summary>
		/// Returns the ItemArray for the matching entry.
		/// </summary>
		/// <param name="currentText">The current text content of the edit control.</param>
		/// <returns>An itemArray.</returns>
		public object[ ] GetItemArray( string currentText )
		{
			DataRowView rowView = null;
			bool matchFound = false;

			int matchColumnIndex = GetMatchColumnIndex();

			for( int i = 0 ; i < this.tableData.DefaultView.Count ; i++ )
			{
				rowView = this.tableData.DefaultView[ i ];
				if( rowView[ matchColumnIndex ].ToString() == currentText )
				{
					matchFound = true;
					break;
				}
			}

			object[ ] itemArray = null;
			if( rowView != null && matchFound )
			{
				itemArray = rowView.Row.ItemArray;
			}
			return itemArray;
		}

		/// <summary>
		/// Updates the selected item to represent the match.
		/// </summary>
		/// <param name="currentText"></param>
		public void UpdateSelectedItem( string currentText )
		{
			DataView dataView = null;
			DataRow matchingRow = null;
			AutoCompleteItem item = null;

			int matchingCount = this.PopulateListWithMatches( currentText, ref dataView, true, ref matchingRow );

			if( matchingCount > 0 && matchingRow != null )
			{
				int matchColumnIndex = this.GetMatchColumnIndex();
				item = new AutoCompleteItem( matchingRow.ItemArray, matchColumnIndex );
				this.SelectedIndex = FindIndex( matchingRow.Table, matchingRow );
				this.SelectedValue = matchingRow.ItemArray[ matchColumnIndex ].ToString();
			}

			this.SetSelectedItem( item );
		}

		/// <summary>
		/// Set the SelectedItem
		/// </summary>
		public virtual void SetSelectedItem( AutoCompleteItem item )
		{
			this.selectedItem = item;
			//todo - change the display to reflect this
		}

		/// <summary>
		/// Internal helper function to fill the drop down list with the
		/// possible matches.
		/// </summary>
		/// <param name="listDataView">The DataView object that has the list of matches.</param>
		/// <remarks>
		/// This function sets the ListView control with the DataView
		/// object that is to be used as its data source. The ListView control used here
		/// is a ListView derived class that can take a DataSource and populate
		/// its columns and rows.</remarks>
		public void FillDropDownList( DataView listDataView, string selectedEntry, int matchColumnIndex )
		{
			int dataRowViewIndex = 0;
			string compareString = String.Empty;

			this.dropDownList.ImageColumnIndex = this.ImageColumnIndex;
			if( this.dropDownList.DataSource.Equals( listDataView ) == false )
			{
				this.dropDownList.DataSource = listDataView;
                this.contentChanged = true;
                if (this.dropDownList.ItemsCount > this.maxNumberofSuggestion && this.maxNumberofSuggestion >= 0)
                {
                    this.dropDownList.ItemsCount = this.maxNumberofSuggestion;
                }

			}

			if( this.AdjustHeightToItemCount == true && this.contentChanged == true )
			{
				this.dropDownList.AdjustHeight( this.PreferredHeight );
			}
			this.contentChanged = false;
			this.dropDownList.ResizeListColumns( this.dropDownList.Width );

			if( selectedEntry != null && selectedEntry != String.Empty )
			{
				if( matchColumnIndex < listDataView.Table.Columns.Count )
				{
					for( int i = 0 ; i < listDataView.Count ; i++ )
					{
						DataRowView rowView = listDataView[ i ];
						compareString = rowView[ matchColumnIndex ].ToString().ToUpper();
						if( compareString.CompareTo( selectedEntry.ToUpper() ) == 0 )
						{
							dataRowViewIndex = i;
							this.indexToEnsureVisible = i;
							this.dropDownList.SelectItem( i, true );
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Resets the internal history list. Override this if you
		/// are providing your own history source in your derived
		/// class.
		/// </summary>
		public virtual void ResetHistory()
		{
			this.tableData.Clear();
		}

		/// <summary>
		/// This is an implementation for the IEditControlsEmbedListener.
		/// </summary>
		/// <param name="parentControl">The parent control that makes the call.</param>
		/// <param name="editControl">The edit control that has the focus.</param>
		/// <remarks>
		/// This is used when there is a composite control that has more than one edit control
		/// embedded and is being provided auto completion. The parent composite will inform
		/// the auto complete control through this method when there is a change in focus
		/// between its multiple edit controls.
		/// </remarks>
		public void SetActiveEditControl( IEditControlsEmbed parentControl, Control editControl )
		{
			if( parentControl == this.ActiveFocusControl )
			{
				if( editControl != null )
				{
					editControl.TextChanged += new EventHandler( HandleControlTextChanged );
					editControl.KeyDown += new KeyEventHandler( HandleControlKeyDown );
					editControl.DoubleClick += new EventHandler( HandleControlDoubleClick );
					editControl.Click += new EventHandler( HandleControlClick );
					editControl.Validated += new EventHandler( HandleControlValidated );

					if( editControl is ComboBox )
					{
						( ( ComboBox )editControl ).SelectionChangeCommitted += new EventHandler( HandleControlSelectionChangeCommitted );
					}
				}
			}
		}

		/// <summary>
		/// Raises the BeforeAddItem event, if this value is set to true
		/// </summary>
		/// <remarks>Calls the <see cref="AutoComplete.OnBeforeAddItem" />method to 
		/// raise the <see cref="AutoComplete.BeforeAddItem"/> event.</remarks>
		/// <returns>True if the item is not to be added to the history.</returns>
		public bool RaiseBeforeAddItemEvent( DataRow newRow )
		{
			AutoCompleteAddItemCancelEventArgs args = new AutoCompleteAddItemCancelEventArgs();
			args.ImageColumnIndex = this.ImageColumnIndex;
			args.RowItem = newRow;
			this.OnBeforeAddItem( args );
			return args.Cancel;
		}

		/// <summary>
		/// This method implements IDataViewListOwner.GetImageColumnIndex.
		/// </summary>
		/// <returns>The index of the column in the data source that provides the
		/// index of images in the assigned image list.</returns>
		public int GetImageColumnIndex()
		{
			return this.ImageColumnIndex;
		}

		/// <summary>
		/// This method implements IDataViewListOwner.GetColumnWidth.
		/// </summary>
		/// <param name="columnIndex">The index of the column for which to 
		/// return the width.
		/// </param>
		/// <returns>The width of the column.</returns>
		public int GetColumnWidth( int columnIndex )
		{
			int actualIndex = GetActualIndex( columnIndex );

			if( this.Columns.Count > actualIndex )
			{
				return this.Columns[ actualIndex ].MinColumnWidth;
			}
			else
			{
				return AutoComplete.MinColumnWidth;
			}
		}

		/// <summary>
		/// Returns the BindingManagerBase for the datasource.
		/// </summary>
		public BindingManagerBase GetDataManager( object dataSource )
		{
			BindingManagerBase dataManager = null;

			if( this.ParentForm != null && dataSource != null )
			{
				dataManager = this.ParentForm.BindingContext[ dataSource ];
			}
			return dataManager;
		}

		/// <summary>
		/// This should only be invoked when there is a DataSource.
		/// </summary>
		public void RefreshColumns()
		{
			try
			{
				// The Columns collection has to reflect the changes made to the
				// DataSource property
				SortedList matchedColumns = new SortedList();

				int dataColumnIndex = 0;

				foreach( DataColumn dataColumn in this.tableData.Columns )
				{
					int infoColumnIndex = 0;
					if( matchedColumns.ContainsValue( dataColumnIndex ) == false )
					{
						foreach( AutoCompleteDataColumnInfo columnInfo in this.Columns )
						{
							if( dataColumn.ColumnName == columnInfo.ColumnName &&
								matchedColumns.ContainsKey( infoColumnIndex ) == false )
							{
								matchedColumns.Add( infoColumnIndex, dataColumnIndex );
								break;
							}
							infoColumnIndex++;
						}
					}
					dataColumnIndex++;
				}

				AutoCompleteDataColumnInfoCollection newCollection = new AutoCompleteDataColumnInfoCollection( this );

				foreach( int i in matchedColumns.Keys )
				{
					newCollection.Add( new AutoCompleteDataColumnInfo( this.Columns[ i ].ColumnHeaderText, this.Columns[ i ].MinColumnWidth, this.Columns[ i ].Visible ) );
					newCollection[ newCollection.Count - 1 ].MatchingColumn = this.Columns[ i ].MatchingColumn;
					newCollection[ newCollection.Count - 1 ].ImageColumn = this.Columns[ i ].ImageColumn;
				}

				dataColumnIndex = 0;
				foreach( DataColumn dataColumn in this.tableData.Columns )
				{
					if( matchedColumns.ContainsValue( dataColumnIndex ) == false )
					{
						newCollection.Add( new AutoCompleteDataColumnInfo( dataColumn.ColumnName, AutoComplete.MinColumnWidth, true ) );
					}
					dataColumnIndex++;
				}

                columnInfoList.Clear();

				this.columnInfoList = newCollection;
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
				throw new Exception( "There was a exception in synchronizing the column display data with the auto complete data. Please check the inner exception for more details.", e );
			}
		}

		/// <summary>
		/// Indicates the Extender property target.
		/// </summary>
		/// <param name="target">The target control to be considered for extending.</param>
		/// <returns>True if the target control can be extended; false otherwise.</returns>
		/// <remarks>The AutoComplete control provides auto completion for edit
		/// contols (the TextBox and ComboBox controls in Windows Forms) and classes
		/// that implement the <see cref="Syncfusion.Windows.Forms.IEditControlsEmbed"/> interface. The method will
		/// return true if any of these cases are met.</remarks>
		bool IExtenderProvider.CanExtend( object target )
		{
			try
			{
				if( target is TextBox || target is ComboBox || target is IEditControlsEmbed )
				{
					return true;
				}
			}
			catch
			{
				return false;
			}

			return false;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            bool returnValue = false;
            
            if (m.Msg == NativeMethods.WM_KEYDOWN)
            {
                Control editControl = this.GetActiveEditControl(this.ActiveFocusControl);
                if (editControl != null)
                {
                    Keys keys = (Keys)m.WParam.ToInt32();
                    Form parentForm = editControl.FindForm();

                    if (parentForm != null && this.dropDownContainer != null && this.dropDownContainer.IsShowing())
                    {
                        if (parentForm.AcceptButton != null && (keys == Keys.Enter || keys == Keys.Return))
                        {
                            AddItemOnValidate(editControl);

                            if (this.IsDropDownShowing())
                            {
                                this.dropDownContainer.HidePopup(PopupCloseType.Done);
                            }

                            returnValue = true;
                        }
                        else if (parentForm.CancelButton != null && keys == Keys.Escape)
                        {
                            CloseDropDownCancel();

                            returnValue = true;
                        }
                    }
                }
            }

            return returnValue;
        }

		/// <summary>
		/// This is the extended property for the AutoComplete property. 
		/// This will be called by the framework when the AutoComplete property 
		/// is set on any control.
		/// </summary>
		/// <param name="control">The control that the property is applied to.</param>
		/// <param name="enableAutoComplete">Specifies whether the control is to be provided 
		/// auto completion. Auto complete will be provided if this parameter is true.</param>
		/// <remarks>When using the designer, the AutoComplete control adds an extender 
		/// property to the target controls. When using the AutoComplete control
		/// programmatically through the code, you need to use this method to add and remove
		/// auto completion for a target control.</remarks>
		/// <example>
		/// In this example,AutoComplete with AutoSuggest mode is enabled to a TextBox control.
		///<code lang="C#">
		/// this.autoComplete1.SetAutoComplete(this.textBox1, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoSuggest);
		///</code>
		///<code lang="VB">
		///Me.autoComplete1.SetAutoComplete(Me.textBox1, Syncfusion.Windows.Forms.Tools.AutoCompleteModes.AutoSuggest)
		///</code>
		/// </example>
		[
		DefaultValue( AutoCompleteModes.Disabled ),
		Category( "AutoComplete" )
		]
		public void SetAutoComplete( Control control, AutoCompleteModes enableAutoComplete )
		{
			this.targetEnabledMap[ control ] = ( int )enableAutoComplete;

			if( enableAutoComplete != AutoCompleteModes.Disabled )
			{
				AdwiseControlEvents( control );
			}
			else
			{
				UnadwiseControlEvents( control );
			}
		}

		private void UnadwiseControlEvents( Control control )
		{
			control.Enter -= new EventHandler( HandleControlEnter );
			control.GotFocus -= new EventHandler( HandleControlEnter );
			control.Leave -= new EventHandler( HandleControlLeave );
		}

		private void AdwiseControlEvents( Control control )
		{
			control.Enter += new EventHandler( HandleControlEnter );
			control.GotFocus += new EventHandler( HandleControlEnter );
			control.Leave += new EventHandler( HandleControlLeave );
		}

		/// <summary>
		/// Sets the auto complete combo up down.
		/// </summary>
		/// <param name="control">The control that the property is applied to..</param>
		/// <param name="enableComboUpDown">if set to <c>true</c> [enable combo up down].</param>
		[
		DefaultValue( false ),
		Category( "AutoComplete" )
		]
		public void SetAutoCompleteComboUpDown( Control control, bool enableComboUpDown )
		{
			this.targetComboUpDownMap[ control ] = enableComboUpDown;
		}

		/// <summary>
		/// Indicates whether a control has AutoComplete Enabled.
		/// </summary>
		/// <remarks>
		/// The AutoComplete control maintains a list of controls that it needs
		/// to provide auto completion for. This method looks up the AutoCompletion mode
		/// for the control being considered.
		/// </remarks>
		/// <example>
		///		<coderef file="tools\samples\editors package\autocompletedemo\VB\MainForm.cs" name="AutoComplete GetAutoComplete" lang="C#">
		///		<code lang="C#">
		///             AutoCompleteMode mode = this.autoComplete1.GetAutoComplete(this.comboBox1);
		///             if(mode == AutoCompleteMode.Disabled)
		///                 this.autoComplete1.SetAutoComplete(this.comboBox1, Syncfusion.Windows.Forms.Tools.AutoCompleteMode.AutoSuggest);
		///             else
		///                 this.autoComplete1.SetAutoComplete(this.comboBox1, AutoCompleteMode.Disabled);
		///		</code>
		///		</coderef>
		///     <coderef file="tools\samples\editors package\autocompletedemo\VB\MainForm.vb" name="AutoComplete GetAutoComplete" lang="VB">
		///     <code lang="VB">
		///            Dim mode As AutoCompleteMode
		///            mode = Me.autoComplete1.GetAutoComplete(Me.comboBox1)
		///            If (mode = AutoCompleteMode.Disabled) Then
		///                Me.autoComplete1.SetAutoComplete(Me.comboBox1, Syncfusion.Windows.Forms.Tools.AutoCompleteMode.AutoSuggest)
		///            Else
		///                Me.autoComplete1.SetAutoComplete(Me.comboBox1, AutoCompleteMode.Disabled)
		///            End If
		///		</code>
		///		</coderef>
		/// </example>
		[
		DefaultValue( AutoCompleteModes.Disabled ),
		Category( "AutoComplete" )
		]
		public AutoCompleteModes GetAutoComplete( Control control )
		{
			int autoMode = 0;

			if( this.targetEnabledMap[ control ] != null )
			{
				autoMode = ( int )this.targetEnabledMap[ control ];
			}

			return ( AutoCompleteModes )autoMode;
		}

		/// <summary>
		/// Returns information for each control if ComboUpDown behavior is  shown or not.
		/// </summary>
		[
		Category( "AutoComplete" )
		]
		public bool GetAutoCompleteComboUpDown( Control control )
		{
			bool autoMode = false;

			if( this.targetComboUpDownMap[ control ] != null && this.targetComboUpDownMap.Contains( control ) )
			{
				autoMode = ( bool )this.targetComboUpDownMap[ control ];
			}

			return autoMode;
		}

		/// <summary>
		/// Adds an item to the internal history of the AutoComplete control. 
		/// This method will create a new entry for this string in the internal list.
		/// <para>
		/// This call will be ignored if an item already exists with
		/// the same string value.
		/// </para>
		/// This method is overloaded.
		/// </summary>
		/// <param name="newItem">The string to be added to the history list.</param>
		public void AddHistoryItem( string newItem )
		{
			if( newItem != null && newItem != String.Empty )
			{
				this.AddHistoryItem( newItem, -1 );
			}
		}

		/// <summary>
		/// Overloaded. Saves the current state of the control to the windows registry.
		/// </summary>
		public virtual void SaveCurrentState()
		{
			//this.SaveCurrentState(SerializeMode.WindowsRegistry, null);
			RegistryKey regKey = Registry.CurrentUser;
			regKey = regKey.CreateSubKey( "Software\\Syncfusion\\AutoComplete" );
			AppStateSerializer serializer = new AppStateSerializer( SerializeMode.WindowsRegistry, regKey );
			this.SaveCurrentState( serializer );
		}

		/// <summary>
		/// Saves the current internal list information to the specified persistence medium.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath"> Specifies the name of an IsolatedStorage/INI/XML file or a registry key to
		/// which the persistence information will be written.</param>
		/// <remarks>
		/// <para>
		/// Writes the internal history information to the persistence medium specified by the
		/// <paramref name="mode"/> parameter and at the path specified by the <paramref name="persistpath"/> object.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="AutoComplete.SaveCurrentState()"/> and <see cref="AutoComplete.LoadCurrentState()"/>
		/// methods.
		/// </para>
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible LoadCurrentState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		[ Obsolete( "This method will be removed in a future version. Please use the more flexible SaveCurrentState(AppStateSerializer) variant, instead.", false ) ]
		public virtual void SaveCurrentState( SerializeMode mode, Object persistpath )
		{
			string specificKey = String.Empty;

			try
			{
				if( this.CategoryName == String.Empty )
				{
					return;
				}

				AutoCompleteInfo info = new AutoCompleteInfo();
				info.CompleteList = ( DataTable )this.tableData;

				if( ( mode == SerializeMode.WindowsRegistry ) && ( persistpath == null ) )
				{
					persistpath = ( RegistryKey )Registry.CurrentUser;
					persistpath = ( ( RegistryKey )persistpath ).CreateSubKey( "Software\\Syncfusion\\AutoComplete" );
					specificKey = this.CategoryName;
				}
				AppStateSerializer serializer = new AppStateSerializer( mode, persistpath );
				serializer.SerializeObject( specificKey, info );
				serializer.PersistNow();
			}
			catch( Exception e )
			{
				throw( e );
			}
		}

		/// <summary>
		/// Saves the current internal list information to the specified persistence medium.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// Writes the internal history information to the persistence medium specified by the
		/// <paramref name="mode"/> parameter and at the path specified by the <paramref name="persistpath"/> object.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="AutoComplete.SaveCurrentState()"/> and <see cref="AutoComplete.LoadCurrentState()"/>
		/// methods.
		/// </remarks>
		public virtual void SaveCurrentState( AppStateSerializer serializer )
		{
			string specificKey = String.Empty;

			try
			{
				if( this.CategoryName == String.Empty )
				{
					return;
				}
				else
				{
					specificKey = this.CategoryName;
				}

				AutoCompleteInfo info = new AutoCompleteInfo();
				info.CompleteList = ( DataTable )this.tableData;

				serializer.SerializeObject( specificKey, info );
				serializer.PersistNow();
			}
			catch( Exception e )
			{
				throw( e );
			}
		}

		/// <summary>
		/// Overloaded. Reads the persisted state from the windows registry.
		/// </summary>
		/// <returns>True if the read is successful; false otherwise.</returns>
		public bool LoadCurrentState()
		{
			RegistryKey regKey = Registry.CurrentUser;
			regKey = regKey.CreateSubKey( "Software\\Syncfusion\\AutoComplete" );
			AppStateSerializer serializer = new AppStateSerializer( SerializeMode.WindowsRegistry, regKey );
			return this.LoadCurrentState( serializer );
		}

		/// <summary>
		/// Reads a previously serialized internal history list.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <returns>True if the load is successful; false otherwise.</returns>
		/// <remarks>
		/// Reads the internal history information from the specified persistent store and applies the new state.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="AutoComplete.SaveCurrentState()"/> and <see cref="AutoComplete.LoadCurrentState()"/>
		/// methods.
		/// </remarks>
		public virtual bool LoadCurrentState( AppStateSerializer serializer )
		{
			try
			{
				string specificKey = this.CategoryName;
				AutoCompleteInfo info = new AutoCompleteInfo();

				info = serializer.DeserializeObject( specificKey ) as AutoCompleteInfo;

				if( info == null || info.CompleteList == null )
				{
					this.tableData = new DataTable();
					return false;
				}
				this.tableData = info.CompleteList;
				return true;
			}
			catch( Exception e )
			{
				throw( e );
			}
		}

		/// <summary>
		/// Reads a previously serialized internal history list.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistPath">The name of the IsolatedStorage/INI/XML file or the
		/// registry key containing the  persisted dockstate information.</param>
		/// <returns>True if the load is successful; false otherwise.</returns>
		/// <remarks>
		/// Reads the internal history information from the specified persistent store and applies the new state.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="AutoComplete.SaveCurrentState()"/> and <see cref="AutoComplete.LoadCurrentState()"/>
		/// methods.
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible LoadCurrentState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		[ Obsolete( "This method will be removed in a future version. Please use the more flexible LoadCurrentState(AppStateSerializer) variant, instead.", false ) ]
		public virtual bool LoadCurrentState( SerializeMode mode, Object persistPath )
		{
			try
			{
				if( this.CategoryName == null )
				{
					return false;
				}

				string specificKey = String.Empty;
				AutoCompleteInfo info = new AutoCompleteInfo();

				if( ( mode == SerializeMode.WindowsRegistry ) && ( persistPath == null ) )
				{
					persistPath = ( RegistryKey )Registry.CurrentUser;
					persistPath = ( ( RegistryKey )persistPath ).CreateSubKey( "Software\\Syncfusion\\AutoComplete" );
					specificKey = this.CategoryName;
				}

				AppStateSerializer serializer = new AppStateSerializer( mode, persistPath );
				info = serializer.DeserializeObject( specificKey ) as AutoCompleteInfo;

				if( info == null )
				{
					this.tableData = new DataTable();
					return false;
				}
				this.tableData = info.CompleteList;
			}
			catch( Exception e )
			{
				throw( e );
			}

			return true;
		}

		/// <summary>
		/// Closes the <see cref="AutoComplete"/> drop down window.
		/// </summary>
		/// <example>
		/// In this example,DropDown closes if character '@' is entered in AutoComplete enabled TextBox.
		///<code lang="C#">
		///private void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		///{
		///    if (e.KeyChar == '@')
		///   {
		///        this.autoComplete1.CloseDropDown();
		///    }
		///}
		///</code>
		///<code lang="VB">
		///Private Sub textBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) 
		///If e.KeyChar = "@"C Then 
		///Me.autoComplete1.CloseDropDown 
		///End If 
		///End Sub
		///</code>
		/// </example>
		public void CloseDropDown()
		{
			if( this.IsDropDownShowing() )
			{
				this.dropDownContainer.HidePopup( PopupCloseType.Canceled );
			}
		}
		#endregion

		#region Class event raisers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentText"></param>
		private string RaisePreMatchItemEvent( string currentText )
		{
			AutoCompletePreMatchItemEventArgs args = new AutoCompletePreMatchItemEventArgs( currentText );
			this.OnPreMatchItemEvent( args );
			return args.CurrentText;
		}

		/// <summary></summary>
		/// <param name="editControl"></param>
		/// <param name="isLeaving"></param>
		private void RaiseTargetChangingEvent( Control editControl, bool isLeaving )
		{
			AutoCompleteTargetChangingEventArgs args = new AutoCompleteTargetChangingEventArgs( editControl, isLeaving );
			this.OnTargetChanging( args );
		}

		/// <summary>
		/// Creates the AutoCompleteItemSelectedEventArgs and Invokes OnAutoCompleteItemSelected.
		/// </summary>
		/// <param name="itemArray">The itemArray object.</param>
		/// <param name="matchColumnIndex">The column index.</param>
		private bool RaiseAutoCompleteItemSelectedEvent( object[ ] itemArray, int matchColumnIndex, string selectedValue, ref string newSelectedValue )
		{
			AutoCompleteItemEventArgs args = new AutoCompleteItemEventArgs( itemArray, matchColumnIndex );
			args.SelectedValue = selectedValue;
			this.OnAutoCompleteItemSelected( args );
			try
			{
				if( args.Handled )
				{
					newSelectedValue = args.SelectedValue;
				}
			}

			catch( Exception e )
			{
				TraceUtil.TraceExceptionCatched( e );
				if( ExceptionManager.RaiseExceptionCatched( this, e ) )
				{
					throw e;
				}
			}
			return args.Handled;
		}

		/// <summary>
		/// Raises the MatchItem event <see cref="AutoComplete.MatchItem"/>.
		/// </summary>
		/// <param name="currentText">The current text of the edit control.</param>
		/// <param name="possibleMatch">The possible match to be displayed.</param>
		/// <returns>True if the possible match string is an acceptable match; false otherwise.</returns>
		protected bool RaiseMatchItemEvent( string currentText, string possibleMatch )
		{
			AutoCompleteMatchItemEventArgs args = new AutoCompleteMatchItemEventArgs( currentText, possibleMatch );
			this.OnMatchItem( args );
			return args.Cancel;
		}

		/// <summary>
		/// Creates the AutoCompleteSelectedEventArgs and invokes OnAutoCompleteItemBrowsed.
		/// </summary>
		/// <param name="itemArray">The item array that contains the item and the sub items 
		/// (columns) of the entry selected.</param>
		/// <param name="matchColumnIndex">The column index.</param>
		private void RaiseAutoCompleteItemBrowsedEvent( object[ ] itemArray, int matchColumnIndex )
		{
			AutoCompleteItemEventArgs args = new AutoCompleteItemEventArgs( itemArray, matchColumnIndex );
			this.OnAutoCompleteItemBrowsed( args );
		}
		#endregion

		#region Class Settings Serialization
		/// <summary>
		/// Serializes the internal list to the isolated storage. This method is only invoked when
		/// the <see cref="AutoSerialize"/> property is set to true.
		/// </summary>
		/// <remarks>
		/// This method invokes the <see cref="SaveCurrentState()"/> method to persist the 
		/// history items. Items are not persisted when an external data source is provided.
		/// </remarks>
		private void SerializeList()
		{
			try
			{
				if( this.autoSerializeValue == true && this.DesignMode == false && this.dataSource == null
					&& this.CategoryName != null && this.CategoryName != String.Empty )
				{
					this.SaveCurrentState();
				}
			}
			catch( Exception e )
			{
				throw( e );
			}
		}

		/// <summary>
		/// Deserializes the internal history list. Used by the default serialization.
		/// method.
		/// </summary>
		private void DeSerializeList()
		{
			try
			{
				if( this.autoSerializeValue == true && this.DesignMode == false && this.dataSource == null
					&& this.CategoryName != null && this.CategoryName != String.Empty )
				{
					this.LoadCurrentState();
				}

			}
			catch( Exception e )
			{
				throw( e );
			}
		}

		/// <summary>
		/// Restores DropDown state.
		/// </summary>
		private void LoadDropDownSizeState()
		{
			// Deserialize persisted value
			AppStateSerializer ass = AppStateSerializer.GetSingleton();
			AutoCompleteStateSerializer acStateSerializer = ass.DeserializeObject( c_strAUTO_COMPLETE + nCounter.ToString() ) as AutoCompleteStateSerializer;

			if( acStateSerializer != null )
			{
				// When DropDown is shown for the first time
				// we have to create PopupHost here to ensure right DropDown Size
				if( this.AutoCompletePopup.PopupHost == null )
				{
					SizablePopupHost sphDropDown = new SizablePopupHost( dropDownList );
					sphDropDown.ShowCloseButton = true;
					sphDropDown.ShowGripper = true;
					sphDropDown.LastSize = new Size( acStateSerializer.DropDownWidth, acStateSerializer.DropDownHeight );
					this.AutoCompletePopup.PopupHost = sphDropDown;
				}
				else // Just in case
				{
					PopupHost puDropDown = this.AutoCompletePopup.PopupHost as PopupHost;
					if( puDropDown != null )
					{
						( ( SizablePopupHost )puDropDown ).LastSize =
							new Size( acStateSerializer.DropDownWidth, acStateSerializer.DropDownHeight );
					}
				}
			}
		}

		/// <summary>
		/// Saves DropDown state.
		/// </summary>
		private void SaveDropDownSizeState()
		{
			AppStateSerializer ass = AppStateSerializer.GetSingleton();
			AutoCompleteStateSerializer acStateSerializer = new AutoCompleteStateSerializer( this );
			ass.SerializeObject( c_strAUTO_COMPLETE + nCounter.ToString(), acStateSerializer );
		}
		#endregion

		#region Class helper methods
		/// <summary>
		/// There will be a difference between the number of columns that
		/// is passed to the VirtualListView and the number of columns in the
		/// <see cref="Columns"/> property. This method gets the actual index
		/// of a column in the VirtualListView with the Columns property.
		/// </summary>
		/// <param name="visibleIndex">The index of the visible column.</param>
		/// <returns>The actual index of the visible column.</returns>
		private int GetActualIndex( int visibleIndex )
		{
			int visibleCount = 0;
			int columnIndex = 0;

			foreach( AutoCompleteDataColumnInfo info in this.Columns )
			{
				if( info.Visible == true || info.ImageColumn == true )
				{
					if( visibleCount == visibleIndex )
					{
						return columnIndex;
					}

					visibleCount++;
				}

				columnIndex++;
			}
			return visibleIndex;
		}

		/// <summary></summary>
		/// <param name="matchColumnName"></param>
		/// <returns></returns>
		private int GetMatchColumnIndexInDataTable( string matchColumnName )
		{
			int index = 0;
			foreach( DataColumn c in this.tableData.Columns )
			{
				if( c.ColumnName == matchColumnName )
				{
					break;
				}
				else
				{
					index++;
				}
			}

			return index;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="matchColumnName"></param>
		/// <param name="dataView"></param>
		/// <returns></returns>
		private int GetMatchColumnIndexInDataView( string matchColumnName, ref DataView dataView )
		{
			int index = 0;
			foreach( DataColumn c in dataView.Table.Columns )
			{
				if( c.ColumnName == matchColumnName )
				{
					break;
				}
				else
				{
					index++;
				}
			}

			return index;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string GetMatchColumnHeaderText()
		{
			for( int i = 0 ; i < this.Columns.Count ; i++ )
			{
				if( this.Columns[ i ].MatchingColumn == true )
				{
					return this.Columns[ i ].ColumnHeaderText;
				}
			}

			if( this.Columns.Count > 0 )
			{
				return this.Columns[ 0 ].ColumnHeaderText;
			}
			else
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="incomingText"></param>
		/// <returns></returns>
		private string FormatStringForSQL( string incomingText )
		{
			string outgoingText = String.Empty;

			try
			{
				// Unicode quotes check and replace
				if( incomingText.Length > 0 )
				{
					outgoingText = incomingText;
					outgoingText = outgoingText.Replace( "’", "'" );
					outgoingText = outgoingText.Replace( "‘", "'" );

					outgoingText = outgoingText.Replace( "'", "''" );
					outgoingText = outgoingText.Replace( "[", "[[]" );
					outgoingText = outgoingText.Replace( "%", "[%]" );
					outgoingText = outgoingText.Replace( "*", "[*]" );
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( e.Message );
			}

			return outgoingText;

		}

		/// <summary>
		/// 
		/// </summary>
		private void CloseDropDownCancel()
		{
			this.SetSelectedItem( null );
			this.CloseDropDown();
		}

		/// <summary>
		/// Returns the AutoCompleteMode for the current control.
		/// </summary>
		/// <returns>The <see cref="AutoCompleteModes"/> type value
		/// for the control with the focus.</returns>
		private AutoCompleteModes GetControlAutoMode()
		{
			AutoCompleteModes autoMode = AutoCompleteModes.Disabled;
			try
			{
				if( this.targetEnabledMap != null && this.ActiveFocusControl != null )
				{
					autoMode = (AutoCompleteModes)this.targetEnabledMap[this.ActiveFocusControl];
				}
			}
			catch( Exception e )
			{
				throw e;
			}

			return autoMode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool GetControlComboUpDownMode()
		{
			bool autoMode = false;
			try
			{
				if( this.targetComboUpDownMap != null && this.ActiveFocusControl != null && this.targetComboUpDownMap.Contains( this.ActiveFocusControl ) )
				{
					autoMode = ( bool )this.targetComboUpDownMap[ this.ActiveFocusControl ];
				}
			}
			catch( Exception e )
			{
				throw e;
			}

			return autoMode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private int GetMatchColumnIndex()
		{
			int matchColumnIndex = 0;
			string matchColumnName = this.MatchColumnName;

			for( int i = 0 ; i < this.tableData.Columns.Count ; i++ )
			{
				if( matchColumnName == this.tableData.Columns[ i ].ColumnName )
				{
					matchColumnIndex = i;
					break;
				}
			}

			return matchColumnIndex;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentText"></param>
		/// <param name="dataView"></param>
		/// <param name="matchColumnIndex"></param>
		/// <returns></returns>
		private DataRowView GetAutoAppendEntry( string currentText, DataView dataView, int matchColumnIndex )
		{
			int dataRowViewIndex = 0;
			BindingManagerBase dataManager = this.GetDataManager( DataSource );

            if (dataView.Count == 0)
                return null;

            if (!this.IsDropDownShowing())
            {
                for (int i = 0; i < dataView.Count; i++)
                {
                    DataRowView rowView = dataView[i];
                    if (rowView[matchColumnIndex].ToString() != currentText)
                    {
                        dataRowViewIndex = i;
                        if (this.ChangeDataManagerPosition == true)
                        {
                            int actualIndex = this.GetActualRowIndex(dataView, i, matchColumnIndex);
                            if (dataManager != null && dataManager.Position != actualIndex && actualIndex != -1)
                            {
                                dataManager.Position = actualIndex;
                            }
                        }
                        break;
                    }
                }
            }

            DataRowView returnRowView = dataView[ dataRowViewIndex ];
			return returnRowView;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataView"></param>
		/// <param name="index"></param>
		/// <param name="matchColumnIndex"></param>
		/// <returns></returns>
		private int GetActualRowIndex( DataView dataView, int index, int matchColumnIndex )
		{
			int actualIndex = 0;
			// This is a workaround for the limitation that the DataRowCollection can take only
			// 32 rows for matching. The side effect is that the matching is only effective for the
			// first 32 rows.
			object[ ] itemArray = dataView[ index ].Row.ItemArray;
			int columnCount = itemArray.Length;
			if( columnCount > DEF_MAX_KEY_COUNT )
			{
				object[ ] fullArray = dataView[ index ].Row.ItemArray;
				itemArray = new object[DEF_MAX_KEY_COUNT];
				for( int i = 0 ; i < DEF_MAX_KEY_COUNT ; i++ )
				{
					itemArray[ i ] = fullArray[ i ];
				}
			}
			DataRow actualRow = dataView.Table.Rows.Find( itemArray );
			string matchColumnName = String.Empty;
			DataView dvOriginal = dataView.Table.DefaultView;
			//dvOriginal = new DataView(dataView.Table , null,dataView.Table.Columns[matchColumnIndex].ColumnName  , DataViewRowState.OriginalRows) ;
			dvOriginal.Sort = dataView.Table.Columns[ matchColumnIndex ].ColumnName;
			actualIndex = dvOriginal.Find( actualRow.ItemArray[ matchColumnIndex ] );
			return actualIndex;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentText"></param>
		/// <param name="dataView"></param>
		/// <param name="matchColumnIndex"></param>
		/// <returns></returns>
		private DataRowView GetAutoAppendEntryPrevious( string currentText, DataView dataView, int matchColumnIndex )
		{
			int dataRowViewIndex = 0;
			BindingManagerBase dataManager = this.GetDataManager( DataSource );

			for( int i = 0 ; i < dataView.Count ; i++ )
			{
				DataRowView rowView = dataView[ i ];
				if( rowView[ matchColumnIndex ].ToString() == currentText )
				{
					dataRowViewIndex = Math.Min( dataView.Count - 1, i + 1 );
					if( this.ChangeDataManagerPosition == true )
					{
						if( dataManager != null && dataManager.Position != dataRowViewIndex )
						{
							dataManager.Position = dataRowViewIndex;
						}
					}

					break;
				}
			}

			DataRowView returnRowView = dataView[ dataRowViewIndex ];
			return returnRowView;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentText"></param>
		/// <param name="dataView"></param>
		/// <param name="matchColumnIndex"></param>
		/// <returns></returns>
		private DataRowView GetAutoAppendEntryNext( string currentText, DataView dataView, int matchColumnIndex )
		{
			int dataRowViewIndex = 0;
			BindingManagerBase dataManager = this.GetDataManager( DataSource );

			for( int i = 0 ; i < dataView.Count ; i++ )
			{
				DataRowView rowView = dataView[ i ];
				if( rowView[ matchColumnIndex ].ToString() == currentText )
				{
					dataRowViewIndex = Math.Max( 0, i - 1 );
					if( this.ChangeDataManagerPosition == true )
					{
						if( dataManager != null && dataManager.Position != dataRowViewIndex )
						{
							dataManager.Position = dataRowViewIndex;
						}
					}

					break;
				}
			}

			DataRowView returnRowView = dataView[ dataRowViewIndex ];
			return returnRowView;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="dr"></param>
		/// <returns></returns>
		private int FindIndex( DataTable dt, DataRow dr )
		{
			for( int i = 0 ; i < dt.Rows.Count ; i++ )
			{
				if( Object.ReferenceEquals( dt.Rows[ i ], dr ) )
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Returns the active edit control. The edit control is not the
		/// same as the active control. The control that we hold as the
		/// active control could embed an edit control and may not be 
		/// the actual edit control that needs to be provided with the
		/// AutoComplete services.
		/// </summary>
		/// <param name="activeControl">The control with the focus.</param>
		/// <returns></returns>
		private Control GetActiveEditControl( Control activeControl )
		{
			Control editControl = null;

			if( activeControl is IEditControlsEmbed )
			{
				editControl = ( ( IEditControlsEmbed )this.ActiveFocusControl ).GetActiveEditControl( this );
			}
			else if( activeControl is ComboBox || activeControl is TextBox )
			{
				editControl = this.ActiveFocusControl;
			}

			return editControl;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exactMatch"></param>
		/// <param name="bExtended"></param>
		/// <param name="sSortColumn"></param>
		/// <returns></returns>
		private DataView GetDataView(string sText, bool exactMatch, bool bExtended)
		{
			string sFilter = "";
			string sKey = " LIKE '";

			if (exactMatch)
			{
				sKey += sText;
			}
			else
			{
				if (bExtended)
				{
					sKey += "*";
				}
				sKey += sText + "*"; ;
			}

			sKey += "'";

			foreach (AutoCompleteDataColumnInfo ci in this.Columns)
			{
				if (!ci.ImageColumn)
				{
					DataColumn col = this.tableData.Columns[ci.ColumnName];

					if (sFilter.Length > 0)
					{
						sFilter += " OR ";
					}
					sFilter += "[" + col.ColumnName + "]" + sKey;
				}
			}
			
			return new DataView(this.tableData, sFilter, null, DataViewRowState.CurrentRows);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="listDataView"></param>
		/// <param name="columnsInfo"></param>
		/// <returns></returns>
		private DataView CreateDisplayDataView( DataView listDataView, AutoCompleteDataColumnInfoCollection columnsInfo )
		{
			DataView newView = new DataView();
			DataTable newTable = new DataTable();
			ArrayList arrangeList = new ArrayList();
			int columnIndex = 0;
			int dataColumnIndex = 0;

			foreach( AutoCompleteDataColumnInfo column in columnsInfo )
			{
				dataColumnIndex = 0;

				if( column.Visible == true || column.ImageColumn == true )
				{
					//find this column in the dataview
					foreach( DataColumn dataColumn in listDataView.Table.Columns )
					{
						if( dataColumn.ColumnName == column.ColumnName )
						{
							if( newTable.Columns.Contains( column.ColumnHeaderText ) == false )
							{
								arrangeList.Add( dataColumnIndex );
								newTable.Columns.Add( column.ColumnHeaderText );
								break;
							}
						}
						dataColumnIndex++;
					}
				}
			}

			// The table has been created with the right columns
			foreach( DataRowView row in listDataView )
			{
				DataRow newRow;
				newRow = newTable.NewRow();
				columnIndex = 0;
				foreach( int i in arrangeList )
				{
					newRow[ columnIndex ] = row[ i ];
					columnIndex++;
				}
				newTable.Rows.Add( newRow );
			}

			// Create the view
			newView = newTable.DefaultView;
			return newView;
		}


		/// <summary>
		/// Sets the text of the edit control target to the selected text.
		/// </summary>
		/// <param name="selectedValue">The selected text.</param>
		private void SetSelectedValue( string selectedValue )
		{
			Control editControl = this.GetActiveEditControl( this.ActiveFocusControl );

			if( editControl != null )
			{
				this.IgnoreChangeMessage++;
				string matchText = selectedValue;
				if( CaseSensitive )
				{
					if( this.currentTextBeingMatched.Length > 0 && matchText.Length >= this.currentTextBeingMatched.Length )
					{
						matchText = this.currentTextBeingMatched + matchText.Substring( this.currentTextBeingMatched.Length );
					}
				}
				editControl.Text = matchText;
				this.IgnoreChangeMessage--;

				if( editControl is TextBox )
				{
					( ( TextBox )editControl ).SelectionStart = selectedValue.Length;
					( ( TextBox )editControl ).SelectionLength = 0;
				}
				else if( editControl is ComboBox )
				{
					( ( ComboBox )editControl ).SelectionStart = selectedValue.Length;
					( ( ComboBox )editControl ).SelectionLength = 0;
				}
			}
		}

		/// <summary>
		/// Adds an item to the history list when invoked.
		/// </summary>
		/// <param name="editControl">The target edit control.</param>
		private void AddItemOnValidate( Control editControl )
		{
			string itemToInsert = String.Empty;
			bool insertItem = false;

			if( this.AutoAddItem == true )
			{
				if( editControl is ComboBox )
				{
					if( ( ( ComboBox )editControl ).SelectedIndex == -1 )
					{
						itemToInsert = editControl.Text;
						insertItem = true;

						ComboBoxAutoComplete cmbAutoComplete = editControl as ComboBoxAutoComplete;
						if( cmbAutoComplete != null )
						{
							insertItem = insertItem && cmbAutoComplete.AllowNewText;
						}
					}
				}
				else if( editControl is TextBox )
				{
					itemToInsert = editControl.Text;
					insertItem = true;
				}
			}

            if (!isEnterHit)
            {
                insertItem = false;
            }

			if( insertItem == true )
			{
				this.AddHistoryItem( itemToInsert );
				SerializeList();
                this.RefreshColumns();
			}
		}

		/// <summary>
		/// Returns AutoCompleteModes of the ActiveFocusControl
		/// </summary>
		/// <returns></returns>
		internal AutoCompleteModes GetAutoComplete()
		{
			return GetControlAutoMode();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal Control GetActiveEditControl()
		{
			return GetActiveEditControl(this.ActiveFocusControl);
		}
		#endregion

		#region Nested classes
		class DataSourceConverter : ReferenceConverter
		{
			public DataSourceConverter() : base(typeof(IListSource))
			{
			}
		}
		#endregion

	}
}