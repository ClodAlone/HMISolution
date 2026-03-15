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
using System.Data;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The AutoCompleteInfo class provides an encapsulation for
	/// the data to be serialized for persisting the current state
	/// of the <see cref="AutoComplete"/> class.
	/// </summary>
	/// <remarks>The AutoCompleteInfo class provides a wrapper around the
	/// DataTable used by the <see cref="AutoComplete"/> class to hold the
	/// history items when in <see cref="AutoComplete.AutoSerialize"/> mode.
	/// </remarks>
	[ Serializable ]
	public class AutoCompleteInfo
	{
		#region Class members
		/// <summary>
		/// The internal sorted list object used to save the
		/// history items.
		/// </summary>
		private DataTable completeListValue;
		#endregion

		#region Class properties
		/// <summary>
		/// Returns the complete list of items in the <see cref="AutoComplete"/> 
		/// object.
		/// </summary>
		/// <remarks>This value needs to be set to the DataTable being used
		/// by the <see cref="AutoComplete"/> class that this <see cref="AutoCompleteInfo"/>
		/// object is providing services for.</remarks>
		public DataTable CompleteList
		{
			get
			{
				return completeListValue;
			}

			set
			{
				completeListValue = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates an object of type AutoCompleteInfo and initializes
		/// the member objects.
		/// </summary>
		/// <remarks>The DataTable that is used by the AutoCompleteInfo
		/// class to hold information about the history items of the 
		/// AutoComplete class is initialized with a new <see cref="System.Data.DataTable"/>.
		/// The <see cref="CompleteList"/> property needs to be set with
		/// the appropriate DataTable.</remarks>
		public AutoCompleteInfo()
		{
			completeListValue = new DataTable();
		}

		#endregion
	}
}