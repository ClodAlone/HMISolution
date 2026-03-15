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
using System.Diagnostics;
using System.Drawing;

using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Summary description for ContextPromptCollection.
	/// </summary>
	public class ContextPromptCollection
		: EventBaseCollection
	{
		#region Class Properties
		/// <summary>
		/// Gets or sets item in the list by index.
		/// </summary>
		public ContextPromptItem this[ int index ]
		{
			get
			{
				return List[ index ] as ContextPromptItem;
			}
			set
			{
				List[ index ] = value;
			}
		}
		/// <summary>
		/// Gets or Sets currently selected item.
		/// </summary>
		public ContextPromptItem SelectedItem
		{
			get
			{
				foreach( ContextPromptItem item in List )
				{
					if( item.Selected )
						return item;
				}

				return null;
			}
			set
			{
				ContextPromptItem itemSelected = SelectedItem;

				if( value != itemSelected )
				{
					if( value == null )
						itemSelected.Selected = false;
					else
						value.Selected = true;
				}
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when selection has changed.
		/// </summary>
		public event EventHandler SelectionChanged;
		/// <summary>
		/// Event that is raised when bolded item selection of the currently selected item has changed.
		/// </summary>
		public event EventHandler BoldedItemSelectionChanged;
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Creates new context prompt item and adds it to the list.
		/// </summary>
		/// <returns>Newly created ContextPromptItem object.</returns>
		public ContextPromptItem Add()
		{
			return Add( string.Empty, string.Empty );
		}
		/// <summary>
		/// Creates new context prompt item and adds it to the list.
		/// </summary>
		/// <param name="subject">Subject of the item.</param>
		/// <returns>Newly created ContextPromptItem object.</returns>
		public ContextPromptItem Add( string subject )
		{
			return Add( subject, string.Empty, null );
		}
		/// <summary>
		/// Creates new context prompt item and adds it to the list.
		/// </summary>
		/// <param name="subject">Subject of the item.</param>
		/// <param name="description">Default description of the item.</param>
		/// <returns>Newly created ContextPromptItem object.</returns>
		public ContextPromptItem Add( string subject, string description )
		{
			return Add( subject, description, null );
		}
		/// <summary>
		/// Creates new context prompt item and adds it to the list.
		/// </summary>
		/// <param name="subject">Subject of the item.</param>
		/// <param name="description">Default description of the item.</param>
		/// <param name="image">Image to associate with item.</param>
		/// <returns>Newly created ContextPromptItem object.</returns>
		public ContextPromptItem Add( string subject, string description, Image image )
		{
			ContextPromptItem item = new ContextPromptItem( this );
			item.Subject = subject;
			item.DefaultDescription = description;
			item.Image = image;
			item.SelectionChanged += new EventHandler( ItemSelectionChanged );
			item.BoldedItemSelectionChanged += new EventHandler( item_BoldedItemSelectionChanged );
			Add( item );
			return item;
		}
		/// <summary>
		/// Adds item to the list.
		/// </summary>
		/// <param name="item">ContextPromptItem to be added to the list.</param>
		public void Add( ContextPromptItem item )
		{
			List.Add( item );
		}
		/// <summary>
		/// Removes item from the list.
		/// </summary>
		/// <param name="item">Item to be removed from the list.</param>
		public void Remove( ContextPromptItem item )
		{
			List.Remove( item );
		}
		/// <summary>
		/// Gets index of the item.
		/// </summary>
		/// <param name="item">Item to find.</param>
		/// <returns>Index of the item.</returns>
		public int IndexOf( ContextPromptItem item )
		{
			return List.IndexOf( item );
		}
		#endregion

		#region Class Initialization/Finalization
		/// <summary>
		/// Creates and Initializes new instance of the class.
		/// </summary>
		public ContextPromptCollection()
			: base()
		{ }
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Notifies about change of data.
		/// </summary>
		protected void OnDataChanged()
		{
		}
		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Sets Selected properties of all items to false.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemSelectionChanged( object sender, EventArgs e )
		{
			ContextPromptItem selectedItem = sender as ContextPromptItem;

			if( selectedItem.Selected )
			{
				foreach( ContextPromptItem item in List )
				{
					if( item != selectedItem && item.Selected )
						item.Selected = false;
				}

				if( SelectionChanged != null )
				{
					SelectionChanged( sender, EventArgs.Empty );
				}
			}
		}
		/// <summary>
		/// Raises BoldedItemSelectionChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void item_BoldedItemSelectionChanged( object sender, EventArgs e )
		{
			ContextPromptBoldTextItem itemBolded = sender as ContextPromptBoldTextItem;
			ContextPromptItem item = itemBolded.ParentCollection.Parent;

			if( item.Selected && BoldedItemSelectionChanged != null )
			{
				BoldedItemSelectionChanged( sender, e );
			}
		}
		#endregion
	}
}
