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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Event data for pre matching modifications.
	/// </summary>
	public class AutoCompletePreMatchItemEventArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// The item to be matched.
		/// </summary>
		private string m_currentTextValue;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets / sets the currently selected item.
		/// The first item in the array is the first column of the matching item and
		/// so on for all the sub items.
		/// </summary>
		public string CurrentText
		{
			get
			{
				return m_currentTextValue;
			}

			set
			{
				m_currentTextValue = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates an object of type AutoCompleteSelectedEventArgs and sets the internal
		/// object array with the information about the currently selected item.
		/// </summary>
		/// <param name="currentText">The array that holds the information about the currently
		/// selected item.</param>
		/// <remarks>The array value set through this constructor can be accessed
		/// through the <see cref="CurrentText"/> property.</remarks>
		public AutoCompletePreMatchItemEventArgs( string currentText )
		{
			m_currentTextValue = currentText;
		}
		#endregion
	}

	/// <summary>
	/// Event data for the <see cref="AutoComplete.MatchItem"/> event.
	/// </summary>
	/// <remarks><see cref="AutoCompleteMatchItemEventHandler"/> for the delegate
	/// that uses this event handler.
	/// This event handler is used to package the information that is sent
	/// by the <see cref="AutoComplete"/> class when in <see cref="AutoCompleteMatchModes.Manual"/><para>This is set to false when the event is raised. The event handler
	/// can set this value to true if it wants the <see cref="AutoComplete"/> class to
	/// accept the current <see cref="PossibleMatch"/> item as a possible match
	/// for the <see cref="CurrentText"/>. Setting this value to false with tell the
	/// <see cref="AutoComplete"/> class to ignore this entry.
	/// </para></remarks>
	public class AutoCompleteMatchItemEventArgs : CancelEventArgs
	{
		#region Class members
		/// <summary>
		/// The current text value to be matched.
		/// </summary>
		private string m_currentTextValue;
		/// <summary>
		/// The possible match value.
		/// </summary>
		private string m_possibleMatchValue;
		#endregion

		#region Class properties
		/// <summary>
		/// Returns the current text value to be matched.
		/// </summary>
		/// <remarks>The current text that is to be compared against for possible
		/// matches. This is done one at a time using this event argument. The possible match
		/// for this is accessed through the <see cref="PossibleMatch"/> property.
		/// You can set the CurrentText and <see cref="PossibleMatch"/> properties using
		/// the appropriate constructor.</remarks>
		public string CurrentText
		{
			get
			{
				return m_currentTextValue;
			}
		}

		/// <summary>
		/// Returns the possible match value that needs to be compared against the <see cref="CurrentText"/>
		/// property by the event handler.
		/// </summary>
		/// <remarks>The current text that this possible match will be checked against is 
		/// accessed through the <see cref="CurrentText"/> property.
		/// You can set the <see cref="CurrentText"/> and PossibleMatch properties using
		/// the appropriate constructor.
		/// </remarks>
		public string PossibleMatch
		{
			get
			{
				return m_possibleMatchValue;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates a new object of type AutoCompleteMatchItemEventArgs.
		/// </summary>
		/// <param name="currentText">The current text to be matched.</param>
		/// <param name="possibleMatch">The possible match string.</param>
		/// <remarks>This constructor for the <see cref="AutoCompleteMatchItemEventArgs"/>
		/// class takes the current text of the target control that is to be matched and
		/// the possible match string that it is to be compared against as arguments.
		/// The values set through this constructor can overriden through the
		/// <see cref="CurrentText"/> and <see cref="PossibleMatch"/> properties.</remarks>
		public AutoCompleteMatchItemEventArgs( string currentText, string possibleMatch )
		{
			m_currentTextValue = currentText;
			m_possibleMatchValue = possibleMatch;
		}
		#endregion
	}

	/// <summary>
	/// The event argument for the <see cref="AutoCompleteAddItemCancelEventHandler"/> delegate. 
	/// </summary>
	/// <remarks>
	/// This event handler is used
	/// as the event data for the <see cref="AutoComplete.BeforeAddItem"/> event raised by the
	/// <see cref="AutoComplete"/> control.
	/// </remarks>
	public class AutoCompleteAddItemCancelEventArgs : CancelEventArgs
	{
		#region Class members
		/// <summary>
		/// The image column index.
		/// </summary>
		private int m_imageColumnIndex;
		/// <summary>
		/// The DataRow that has the values for all the columns.
		/// </summary>
		private DataRow m_rowItem;
		#endregion

		#region Class properties
		/// <summary>
		/// The <see cref="DataRow"/> object that contains the value that is to be added to the
		/// history list.
		/// </summary>
		/// <remarks>The <see cref="AutoComplete"/> class uses a <see cref="DataTable"/>
		/// and this value is one of the rows for the internal history list of the
		/// <see cref="AutoComplete"/> class.
		/// You can also set the values of sub items or change their values when
		/// it is handled in the <see cref="AutoComplete.BeforeAddItem"/> event raised by
		/// the <see cref="AutoComplete"/> control.
		/// </remarks>
		public DataRow RowItem
		{
			get
			{
				return m_rowItem;
			}

			set
			{
				m_rowItem = value;
			}
		}

		/// <summary>
		/// Gets / sets the column index into the <see cref="AutoComplete.ImageList"/> property.
		/// </summary>
		/// <remarks>
		/// The <see cref="AutoComplete.BeforeAddItem"/> event uses the
		/// <see cref="AutoCompleteAddItemCancelEventArgs"/> type for the event data. 
		/// This value can be set in the event hander to be a different value. 
		/// Changing the index will change the image that will be displayed.</remarks>
		public int ImageColumnIndex
		{
			get
			{
				return m_imageColumnIndex;
			}

			set
			{
				m_imageColumnIndex = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates and intializes the AutoCompleteAddItemCancelEventArgs object.
		/// </summary>
		/// <remarks>
		/// The <see cref="ImageColumnIndex"/> property is initialized to 
		/// -1 and the <see cref="RowItem"/>  property is set to null.
		/// </remarks>
		public AutoCompleteAddItemCancelEventArgs()
		{
			m_imageColumnIndex = -1;
			m_rowItem = null;
		}
		#endregion
	}

	/// <summary>
	/// The event data for AutoCompleteSelected event. This event is raised by
	/// the <see cref="AutoComplete"/> class when the user selects an item in the
	/// list off possible matches for the current text being displayed in the
	/// target edit control.
	/// </summary>
	/// <remarks><see cref="AutoCompleteItemEventHandler"/> is the event handler delegate that
	/// uses this class as the event data.
	/// </remarks>
	public class AutoCompleteItemEventArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// The list of subitems.
		/// </summary>
		private object[ ] m_itemarray;
		/// <summary></summary>
		private int m_matchColumnIndex;
		/// <summary>
		/// Specifies if the selected value is to be set to the Target control.
		/// </summary>
		private string m_selectedValue = string.Empty;
		/// <summary></summary>
		private bool m_bHandled = false;
		#endregion

		#region Class properties
		/// <summary>
		/// Returns the Auto Complete item as an object array.
		/// </summary>
		/// <remarks>
		/// This property holds the information about the currently selected item.
		/// The first item in the array is the first column of the matching item and
		/// so on for all the sub items.
		/// </remarks>
		public object[ ] ItemArray
		{
			get
			{
				return m_itemarray;
			}
		}

		/// <summary>
		/// Returns the index of the item that was used for the matching.
		/// </summary>
		/// <remarks>
		/// This index could be different from the matching index of the
		/// <see cref="AutoComplete"/> control. The <see cref="ItemArray"/>
		/// returned will only have items/columns that are displayed in the
		/// drop down list of the <see cref="AutoComplete"/> control.
		/// </remarks>
		public int MatchColumnIndex
		{
			get
			{
				return m_matchColumnIndex;
			}
		}

		/// <summary>
		/// Indicates whether the SelectedValue should be applied to the target control. This only applies
		/// when used in the AutoCompleteItemSelected event.
		/// </summary>
		public bool Handled
		{
			get
			{
				return m_bHandled;
			}

			set
			{
				m_bHandled = value;
			}
		}
		/// <summary>
		/// Gets/Sets the value selected.
		/// </summary>
		public string SelectedValue
		{
			get
			{
				return m_selectedValue;
			}

			set
			{
				m_selectedValue = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates an object of type AutoCompleteSelectedEventArgs and sets the internal
		/// object array with the information about the currently selected item.
		/// </summary>
		/// <param name="array">The array that holds the information about the currently
		/// selected item.</param>
		/// <param name="matchColumnIndex">The column index to be used for matching.</param>
		/// <remarks>The array value set through this constructor can be accessed
		/// through the <see cref="ItemArray"/> property.</remarks>
		public AutoCompleteItemEventArgs( object[ ] array, int matchColumnIndex )
		{
			m_itemarray = array;
			m_matchColumnIndex = matchColumnIndex;
		}
		#endregion
	}

	/// <summary>
	/// This class wraps an exception that is thrown by the system as event
	/// data.
	/// </summary>
	/// <remarks>
	/// The event data can be used to track the problem with the exception
	/// that was raised by the system during the auto completion of the
	/// <see cref="AutoComplete"/> control.
	/// </remarks>
	public class AutoCompleteErrorArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// The exception thrown by the system.
		/// </summary>
		private Exception m_error;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		///	Initializes a new instance of the class with an exception.
		/// </summary>
		/// <param name="error">The exception that resulted in this event data 
		/// being created.</param>
		/// <remarks>The exception that is set through the constructor can be 
		/// accessed through the <see cref="GetException"/> method.</remarks>
		public AutoCompleteErrorArgs( Exception error )
		{
			m_error = error;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		///	Returns the original exception that was thrown by the system.
		/// </summary>
		/// <remarks>
		/// The <see cref="Exception"/> that is returned can be used to 
		/// track down any issues that occur in the auto completion process
		/// of the <see cref="AutoComplete"/> control.
		/// </remarks>
		/// <returns>The exception that was thrown by the system.</returns>
		public Exception GetException()
		{
			return m_error;
		}
		#endregion
	}

	/// <summary>
	/// The event data for <see cref="AutoComplete.TargetChanging"/> event.
	/// </summary>
	/// <remarks>
	/// The event data provides information that can be used to tailor the
	/// behavior of the <see cref="AutoComplete"/> control for different
	/// edit controls.
	/// <para>
	/// <see cref="AutoCompleteTargetChangingEventHandler"/> is the event handler
	/// delegate that uses this class as the event data.
	/// </para>
	/// </remarks>
	public class AutoCompleteTargetChangingEventArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// The editControl that is getting the focus or losing 
		/// the focus.
		/// </summary>
		private Control m_editControl;
		/// <summary>
		/// Indicates whether the control is losing the focus.
		/// </summary>
		private bool m_isLeaving;
		#endregion

		#region Class properties
		/// <summary>
		///	Returns the edit control that is gaining or losing focus.
		/// </summary>
		/// <remarks>
		/// The <see cref="AutoComplete"/> control's behavior can be tailored
		/// to be different for each control that it is providing auto completion
		/// for. When handling the <see cref="AutoComplete.TargetChanging"/> event
		/// this property provides information about the currently active edit
		/// control.
		/// </remarks>
		public Control EditControl
		{
			get
			{
				return m_editControl;
			}
		}

		/// <summary>
		/// Indicates whether the event has been raised in response to a 
		/// control gaining focus or losing focus.
		/// </summary>
		/// <returns></returns>
		public bool IsLeaving
		{
			get
			{
				return m_isLeaving;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoCompleteTargetChangingEventArgs"/>
		/// class using a <see cref="Control"/> and a boolean.
		/// </summary>
		/// <param name="editControl">The edit control that is losing the focus or gaining focus.</param>
		/// <param name="isLeaving">Indicates whether the edit control is gaining focus or losing it.</param>
		/// <remarks>
		/// The <paramref name="editControl"></paramref> parameter is the edit control that is gaining
		/// focus or losing it. The <see cref="AutoComplete"/> control's behavior can be changed
		/// based on which edit control it is providing auto completion for.
		/// <para>
		/// The <paramref name="isLeaving"></paramref> parameter indicates whether the edit
		/// control is losing or gaining focus. The value will be false if the edit
		/// control is gaining focus.
		/// </para>
		/// </remarks>
		public AutoCompleteTargetChangingEventArgs( Control editControl, bool isLeaving )
		{
			m_editControl = editControl;
			m_isLeaving = isLeaving;
		}
		#endregion
	}

	/// <summary>
	/// This class wraps an exception that is thrown by the system as event
	/// data.
	/// </summary>
	/// <remarks>
	/// The event data can be used to track the problem with the exception
	/// that was raised by the system during the auto completion of the
	/// <see cref="AutoComplete"/> control.
	/// </remarks>
	public class AutoCompleteCustomizeEventArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// The location of the top left corner of the AutoSuggest window.
		/// </summary>
		private Point m_autoSuggestLocation;

		private string m_textForAutoCompletion;

		private bool m_bHandled;

		private bool m_bCancel;

		private Control m_activeFocusControl;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="AutoCompleteCustomizeEventArgs"/> is handled.
		/// </summary>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled
		{
			get
			{
				return m_bHandled;
			}
			set
			{
				m_bHandled = value;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the AutoCompleteCustomize event should be canceled
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_bCancel;
			}
			set
			{
				m_bCancel = value;
			}
		}

		/// <summary>
		///  Gets /sets  values that determine the position of AutoCompletion.
		/// </summary>
		public Point AutoSuggestLocation
		{
			get
			{
				return m_autoSuggestLocation;
			}

			set
			{
				m_autoSuggestLocation = value;
			}
		}
		/// <summary>
		/// Gets/sets the text for AutoCompletion
		/// </summary>
		public string TextForAutoCompletion
		{
			get
			{
				return m_textForAutoCompletion;
			}

			set
			{
				m_textForAutoCompletion = value;
			}
		}
		/// <summary>
		/// Gets or sets the active control on the AutoComplete control.
		/// </summary>
		public Control ActiveFocusControl
		{
			get
			{
				return m_activeFocusControl;
			}

			set
			{
				m_activeFocusControl = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		public AutoCompleteCustomizeEventArgs( string textForAutoCompletion, Point autoSuggestLocation, Control activeFocusControl )
		{
			m_textForAutoCompletion = textForAutoCompletion;
			m_autoSuggestLocation = autoSuggestLocation;
			m_activeFocusControl = activeFocusControl;
		}
		#endregion
	}

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.AutoCompleteCustomize"/> event.
	/// </summary>
	/// <remarks>The AutoCompleteCustomizeEventHandler uses the <see cref="AutoCompleteCustomizeEventArgs"/>
	/// class as the event data.</remarks>
	public delegate void AutoCompleteCustomizeEventHandler( object sender, AutoCompleteCustomizeEventArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.TargetChanging"/> event.
	/// </summary>
	/// <remarks>The AutoCompleteTargetChangingEventHandler uses the <see cref="AutoCompleteTargetChangingEventArgs"/>
	/// class as the event data.</remarks>
	public delegate void AutoCompleteTargetChangingEventHandler( object sender, AutoCompleteTargetChangingEventArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.AutoCompleteItemSelected"/> event.
	/// </summary>
	/// <remarks>The AutoCompleteSelectedEventHandler uses the <see cref="AutoCompleteItemEventArgs"/>
	/// class as the event data.</remarks>
	public delegate void AutoCompleteErrorEventHandler( object sender, AutoCompleteErrorArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.AutoCompleteItemSelected"/> event.
	/// </summary>
	/// <remarks>The AutoCompleteSelectedEventHandler uses the <see cref="AutoCompleteItemEventArgs"/>
	/// class as the event data.</remarks>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void AutoCompleteItemEventHandler( object sender, AutoCompleteItemEventArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.MatchItem"/> event.
	/// </summary>
	/// <remarks>The AutoCompleteMatchItemEventHandler takes a <see cref="AutoCompleteMatchItemEventArgs"/>
	/// object as the event data argument.</remarks>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void AutoCompleteMatchItemEventHandler( object sender, AutoCompleteMatchItemEventArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.MatchItem"/> event.
	/// </summary>
	/// <remarks>The AutoCompletePreMatchItemEventHandler takes a <see cref="AutoCompletePreMatchItemEventArgs"/>
	/// object as the event data argument.</remarks>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void AutoCompletePreMatchItemEventHandler( object sender, AutoCompletePreMatchItemEventArgs args );

	/// <summary>
	/// Delegate for the <see cref="AutoComplete.BeforeAddItem"/> event
	/// of the <see cref="AutoComplete"/> control.
	/// </summary>
	/// <remarks>The AddItemCancelEventHandler uses the <see cref="AutoCompleteAddItemCancelEventArgs"/>
	/// as the event data.</remarks>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void AutoCompleteAddItemCancelEventHandler( object sender, AutoCompleteAddItemCancelEventArgs args );
}