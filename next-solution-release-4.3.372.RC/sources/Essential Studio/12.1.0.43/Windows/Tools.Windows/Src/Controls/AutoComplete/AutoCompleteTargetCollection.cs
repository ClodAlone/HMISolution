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
	[
	Serializable,
	DocumentationExclude()
	]
	public class AutoCompleteTargetCollection : CollectionBase
	{
		#region Class properties
		/// <summary>
		/// Gets / sets the Indexer property for the AutoCompleteDataColumnInfoCollection class.
		/// </summary>
		/// <remarks>Returns the <see cref="AutoCompleteDataColumnInfo"/> object based 
		/// on the index in the collection.</remarks>
		[ DocumentationExclude() ]
		public AutoCompleteTarget this[ int index ]
		{
			get
			{
				return this.List[ index ] as AutoCompleteTarget;
			}

			set
			{
				this.List[ index ] = value;
			}
		}

		#endregion

		#region Class Public Methods
		/// <summary>
		/// Adds one person to the collection.
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo object to be added.</param>
		/// <returns>returns the count of the list items</returns>
		[ DocumentationExclude() ]
		public int Add( AutoCompleteTarget column )
		{
			return this.List.Add( column );
		}
		/// <summary>
		/// Add array of targets into collection.
		/// </summary>
		/// <param name="array">array of columns.</param>
		[ DocumentationExclude() ]
		public void AddRange( AutoCompleteTarget[] array )
		{
			foreach( AutoCompleteTarget column in array )
			{
				this.Add( column );
			}
		}

		/// <summary>
		/// Removes DataColomnInfo objrects from the
		/// collectoin. 
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo object to remove.</param>
		[ DocumentationExclude() ]
		public void Remove( AutoCompleteTarget column )
		{
			if( this.List.Contains( column ) == true )
			{
				this.List.Remove( column );
			}
		}

		/// <summary>
		/// Indicates whether the collection contains a specific 
		/// AutoCompleteDataColumnInfo entry.
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo to locate 
		/// in the access control list.</param>
		/// <returns>True if the AutoCompleteDataColumnInfo entry is 
		/// found in the collection; false otherwise.</returns>
		[ DocumentationExclude() ]
		public bool Contains( AutoCompleteTarget column )
		{
			return this.List.Contains( column );
		}

		/// <summary>
		/// Return order index of the column in collection, otherwise -1.
		/// </summary>
		/// <param name="column">The AutoCompleteDataColumnInfo to locate 
		/// in the access control list.</param>
		/// <returns>Order index in collection, otherwise -1.</returns>
		[ DocumentationExclude() ]
		public int IndexOf( AutoCompleteTarget column )
		{
			return this.List.IndexOf( column );
		}
		/// <summary>Insert spcified column into collection.</summary>
		/// <param name="index">position where to insert column.</param>
		/// <param name="column">The AutoCompleteDataColumnInfo to locate 
		/// in the access control list.</param>
		[ DocumentationExclude() ]
		public void Insert( int index, AutoCompleteTarget column )
		{
			this.List.Insert( index, column );
		}
		/// <summary>
		/// Copies all the elements of the current one-dimensional 
		/// Array to the specified one-dimensional Array 
		/// starting at the specified destination Array index.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="index">Starting index from which to start the copying.</param>
		[ DocumentationExclude() ]
		public void CopyTo( AutoCompleteTarget[ ] array, int index )
		{
			this.List.CopyTo( array, index );
		}
		#endregion
	}
}