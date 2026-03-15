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

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Collection of <see cref="AutoCompleteDataColumnInfo"/>objects. Used in the
	/// <see cref="AutoComplete.Columns"/> property of the <see cref="AutoComplete"/>
	/// control.
	/// </summary>
	/// <remarks>The AutoCompleteDataColumnInfoCollection is a set of objects 
	/// each of which hold information required to create a column in a 
	/// <see cref="System.Windows.Forms.ListView"/>.</remarks>

	public class AutoCompleteDataColumnInfoCollection : CollectionBase
	{
		#region Class members
		/// <summary>
		/// The owner of the collection.
		/// </summary>
		[ DocumentationExclude() ]
		protected AutoComplete m_owner;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets / sets the Indexer property for the AutoCompleteDataColumnInfoCollection class.
		/// </summary>
		/// <remarks>Returns the <see cref="AutoCompleteDataColumnInfo"/> object based on the index
		/// in the collection.</remarks>
		public AutoCompleteDataColumnInfo this[ int index ]
		{
			get
			{
				return this.List[ index ] as AutoCompleteDataColumnInfo;
			}
			set
			{
				// BUG: we replace old code "Insert( index, value );" to default
				// behavior of the collection.
				this.List[ index ] = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates an instance of the AutoCompleteDataColumnInfoCollection class.
		/// </summary>
		/// <param name="autoComplete">The <see cref="AutoComplete"/> control that
		/// contains this collection.</param>
		public AutoCompleteDataColumnInfoCollection( AutoComplete autoComplete )
		{
			m_owner = autoComplete;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Inserts the <see cref="AutoCompleteDataColumnInfo"/> into the collection 
		/// at the specfied index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item is to be inserted.</param>
		/// <param name="item">The <see cref="AutoCompleteDataColumnInfo"/> to be inserted.</param>
		public void Insert( int index, AutoCompleteDataColumnInfo item )
		{
			this.List.Insert( index, item );
		}

		/// <summary>
		/// Adds one object to the collection.
		/// </summary>
		/// <param name="column">The <see cref="AutoCompleteDataColumnInfo"/> object to be added.</param>
		/// <returns>The count of the list items</returns>
		public int Add( AutoCompleteDataColumnInfo column )
		{
			if( null == column )
			{
				throw new ArgumentNullException( "column" );
			}

			column.columnsCollection = this;
			return this.List.Add( column );
		}

		/// <summary>
		/// Removes <see cref="AutoCompleteDataColumnInfo"/> objects from the
		/// collection. 
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo object to remove.</param>
		public void Remove( AutoCompleteDataColumnInfo column )
		{
			if( this.List.Contains( column ) )
			{
				this.List.Remove( column );

				if( m_owner != null && m_owner.Container != null )
				{
					m_owner.Container.Remove( column );
				}

				column.columnsCollection = null;
			}
		}

		/// <summary>
		/// Returns the index of the column that is used for matching.
		/// </summary>
		/// <returns>Index of the matching column.</returns>
		/// <remarks>
		/// Iterates through the collection and returns the index of the 
		/// element that has the <see cref="AutoCompleteDataColumnInfo.MatchingColumn"/>
		/// to be true.
		/// </remarks>
		public int GetMatchingColumnIndex()
		{
			int matchColumnIndex = 0;

			for( int i = 0, len = this.Count ; i < len ; i++ )
			{
				if( this[ i ].MatchingColumn == true )
				{
					matchColumnIndex = i;
					break;
				}
			}

			return matchColumnIndex;
		}

		/// <summary>
		/// Indicates whether the collection contains a specific AutoCompleteDataColumnInfo entry.
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo to locate in the access control list.</param>
		/// <returns>True if the AutoCompleteDataColumnInfo entry is found in the collection; false otherwise.</returns>
		public bool Contains( AutoCompleteDataColumnInfo column )
		{
			return this.List.Contains( column );
		}

		/// <summary>
		/// Copies all the elements of the current one-dimensional Array to the specified one-dimensional Array 
		/// starting at the specified destination Array index.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="index">Starting index from which to start copying.</param>
		public void CopyTo( AutoCompleteDataColumnInfo[ ] array, int index )
		{
			this.List.CopyTo( array, index );
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Overrides OnInsertComplete.
		/// </summary>
		[ DocumentationExclude() ]
		protected override void OnInsertComplete( int index, object value )
		{
			base.OnInsertComplete( index, value );

			AutoCompleteDataColumnInfo item = value as AutoCompleteDataColumnInfo;

			if( m_owner != null && m_owner.Container != null && item != null )
			{
				m_owner.Container.Add( item );
			}
		}

		/// <summary></summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		[ DocumentationExclude() ]
		protected override void OnSetComplete( int index, object oldValue, object newValue )
		{
			base.OnSetComplete( index, oldValue, newValue );

			AutoCompleteDataColumnInfo oldItem = oldValue as AutoCompleteDataColumnInfo;
			AutoCompleteDataColumnInfo newItem = newValue as AutoCompleteDataColumnInfo;

			if( m_owner != null && m_owner.Container != null )
			{
				if( oldItem != null )
				{
					m_owner.Container.Remove( oldItem );
				}

				if( newItem != null )
				{
					m_owner.Container.Add( newItem );
				}
			}
		}

		protected override void OnRemoveComplete( int index, object value )
		{
			base.OnRemoveComplete( index, value );

			AutoCompleteDataColumnInfo item = value as AutoCompleteDataColumnInfo;

			if( m_owner != null && m_owner.Container != null && item != null )
			{
				m_owner.Container.Remove( item );
			}
		}
		#endregion
	}
}