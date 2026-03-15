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

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Summary description for ContextPromptBoldItemCollection.
	/// </summary>
	public class ContextPromptBoldItemCollection
		: CollectionBase
	{
		#region Class Private Members
		/// <summary>
		/// Parent ContextPromptItem.
		/// </summary>
		private ContextPromptItem m_parent;
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when boled item index has changed.
		/// </summary>
		public event EventHandler SelectionChanged;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets parent item.
		/// </summary>
		public ContextPromptItem Parent
		{
			get
			{
				return m_parent;
			}
		}
		/// <summary>
		/// Gets or sets item by index.
		/// </summary>
		public ContextPromptBoldTextItem this[ int index ]
		{
			get
			{
				return List[ index ] as ContextPromptBoldTextItem;
			}
			set
			{
				List[ index ] = value;
			}
		}
		/// <summary>
		/// Gets or Sets currently selected item.
		/// </summary>
		public ContextPromptBoldTextItem SelectedItem
		{
			get
			{
				foreach( ContextPromptBoldTextItem item in List )
				{
					if( item.Selected )
						return item;
				}

				return null;
			}
			set
			{
				ContextPromptBoldTextItem itemSelected = SelectedItem;

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

		#region Class Public Methods
		/// <summary>
		/// Removes item from collection.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		public void Remove( ContextPromptBoldTextItem item )
		{
			List.Remove( item );
		}
		/// <summary>
		/// Creates new ContextPromptBoldTextItem and adds it to the list.
		/// </summary>
		/// <param name="start">Start index of the bolded part in subject.</param>
		/// <param name="length">Length of the bolded text.</param>
		/// <param name="description">Description of the bolded text.</param>
		/// <returns>Newly created ContextPromptBoldTextItem.</returns>
		public ContextPromptBoldTextItem Add( int start, int length, string description )
		{
			ContextPromptBoldTextItem item = new ContextPromptBoldTextItem( this, start, length, description );
			item.SelectionChanged += new EventHandler( ItemSelectionChanged );

			List.Add( item );
			return item;
		}
		#endregion

		#region Class Initialization/Finalization
		/// <summary>
		/// Creates new instance of ContextPromptBoldItemCollection.
		/// </summary>
		/// <param name="parent">Parent item.</param>
		public ContextPromptBoldItemCollection( ContextPromptItem parent )
			: base()
		{
			if( parent == null )
				throw new ArgumentNullException( "parent" );

			m_parent = parent;
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
			ContextPromptBoldTextItem selectedItem = sender as ContextPromptBoldTextItem;

			if( selectedItem.Selected )
			{
				foreach( ContextPromptBoldTextItem item in List )
				{
					if( item != selectedItem && item.Selected )
						item.Selected = false;
				}

			}

			if( SelectionChanged != null )
			{
				SelectionChanged( sender, EventArgs.Empty );
			}
		}
		#endregion
	}
}
