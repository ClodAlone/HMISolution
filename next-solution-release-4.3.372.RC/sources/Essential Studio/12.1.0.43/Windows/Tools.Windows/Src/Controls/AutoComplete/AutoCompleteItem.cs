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

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	public class AutoCompleteItem
	{
		#region Class members
		/// <summary>
		/// The list of subitems.
		/// </summary>
		private object[ ] m_itemarray;
		/// <summary></summary>
		private int m_matchColumnIndex;
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

		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="array"/>
		/// <param name="matchColumnIndex"/>
		public AutoCompleteItem( object[ ] array, int matchColumnIndex )
		{
			m_itemarray = array;
			m_matchColumnIndex = matchColumnIndex;
		}
		#endregion

		#region Class overrides
		/// <summary></summary>
		/// <returns></returns>
		public override string ToString()
		{
			string baseString = base.ToString();
			int count = m_itemarray.Length;
			if( count > 0 )
			{
				for( int i = 0 ; i < count ; i++ )
				{
					baseString += " " + m_itemarray[ i ].ToString();
				}
			}
			return baseString;
		}
		#endregion
	}
}