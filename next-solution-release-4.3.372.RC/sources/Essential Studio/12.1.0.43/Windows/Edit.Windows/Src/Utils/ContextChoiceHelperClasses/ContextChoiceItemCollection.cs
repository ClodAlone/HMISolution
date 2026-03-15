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
using System.Drawing;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Dialogs;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Collection of context choice items.
	/// </summary>
	public class ContextChoiceItemCollection
		: CollectionBase
	{
		#region Class Private Members
		/// <summary>
		/// Hashtable with items. Key - ID, Value - ContextChoiceItem.
		/// </summary>
		private Hashtable m_hashItems = new Hashtable();
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets item by ID.
		/// </summary>
		public IContextChoiceItem this[ int id ]
		{
			get
			{
				if( !m_hashItems.Contains( id ) )
					return null;

				return ( IContextChoiceItem )m_hashItems[ id ];
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Removes item from collection.
		/// </summary>
		/// <param name="item">Item to remove.</param>
		public void Remove( IContextChoiceItem item )
		{
			if( null == item )
				throw new ArgumentNullException( "item" );

			IContextChoiceItem itemToRemove = this[ item.ID ];
			List.Remove( itemToRemove );
		}
		/// <summary>
		/// Adds item to collection.
		/// </summary>
		/// <param name="item">Item to be added.</param>
		private void Add( ContextChoiceItem item )
		{
			if( null == item )
				throw new ArgumentNullException( "item" );

			if( m_hashItems.Contains( item.ID ) )
				throw new ArgumentException( "The specified item is already added to the collection.", "item" );

			List.Add( item );
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text )
		{
			return this.Add( text, string.Empty, null );
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <param name="tooltip">Tooltip of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text, string tooltip )
		{
			return this.Add( text, tooltip, null );
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <param name="image">Image of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text, INamedImage image )
		{
			return this.Add( text, string.Empty, image );
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <param name="tooltip">Tooltip of the item.</param>
		/// <param name="image">Image of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text, string tooltip, INamedImage image )
		{
			ContextChoiceItem item = new ContextChoiceItem( text, tooltip, image );

			this.Add( item );

			return item;
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <param name="tooltip">Tooltip of the item.</param>
		/// <param name="colorText">Color of the item's text.</param>
		/// <param name="image">Image of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text, string tooltip, Color colorText, INamedImage image )
		{
			ContextChoiceItem item = ( ContextChoiceItem )this.Add( text, tooltip, image );
			item.ForeColor = colorText;

			return item;
		}
		/// <summary>
		/// Creates new context choice item and adds it to collection.
		/// </summary>
		/// <param name="text">Text of the item.</param>
		/// <param name="tooltip">Tooltip of the item.</param>
		/// <param name="colorText">Color of the item's text.</param>
		/// <param name="colorBackground">Color of the item's background.</param>
		/// <param name="image">Image of the item.</param>
		/// <returns>Newly created context choice item.</returns>
		public IContextChoiceItem Add( string text, string tooltip, Color colorText, Color colorBackground, INamedImage image )
		{
			ContextChoiceItem item = ( ContextChoiceItem )this.Add( text, tooltip, colorText, image );
			item.BackColor = colorBackground;

			return item;
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of the collection.
		/// </summary>
		public ContextChoiceItemCollection()
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Clears items hash.
		/// </summary>
		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			m_hashItems.Clear();
			RaiseChangedEvent();
		}
		/// <summary>
		/// Adds item to the hash.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsertComplete( int index, object value )
		{
			base.OnInsertComplete( index, value );

			IContextChoiceItem item = ( IContextChoiceItem )value;
			m_hashItems[ item.ID ] = item;
			RaiseChangedEvent();
		}
		/// <summary>
		/// Removes item from the hash.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		protected override void OnRemoveComplete( int index, object value )
		{
			base.OnRemoveComplete( index, value );
			IContextChoiceItem item = ( IContextChoiceItem )value;
			m_hashItems.Remove( item.ID );
			RaiseChangedEvent();
		}
		/// <summary>
		/// Removes old item from the hash and adds new item to the hash.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSetComplete( int index, object oldValue, object newValue )
		{
			base.OnSetComplete( index, oldValue, newValue );

			IContextChoiceItem item = ( IContextChoiceItem )oldValue;
			m_hashItems.Remove( item.ID );

			item = ( IContextChoiceItem )newValue;
			m_hashItems[ item.ID ] = item;
			RaiseChangedEvent();
		}
		#endregion

		#region Events
		/// <summary>
		/// Raised when list of items was changed.
		/// </summary>
		public event EventHandler CollectionChanged;
		#endregion

		#region Private Methods
		/// <summary>
		/// Raises CollectionChanged event.
		/// </summary>
		private void RaiseChangedEvent()
		{
			if( CollectionChanged != null )
			{
				CollectionChanged( this, EventArgs.Empty );
			}
		}
		#endregion
	}
}
