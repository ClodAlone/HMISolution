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
using System.Windows.Forms;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	[ DocumentationExclude() ]
	public class AutoCompleteTarget
	{
		#region Class members
		/// <summary></summary>
		private Control m_editControl;
		/// <summary></summary>
		private AutoCompleteModes m_autoCompleteMode;
		#endregion

		#region Class properties
		/// <summary></summary>
		[ DocumentationExclude() ]
		public Control EditControl
		{
			get
			{
				return m_editControl;
			}
		}

		/// <summary></summary>
		[ DocumentationExclude() ]
		public AutoCompleteModes AutoCompleteMode
		{
			get
			{
				return m_autoCompleteMode;
			}

			set
			{
				m_autoCompleteMode = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="editControl"></param>
		/// <param name="autoCompleteMode"></param>
		public AutoCompleteTarget( Control editControl, AutoCompleteModes autoCompleteMode )
		{
			m_editControl = editControl;
			m_autoCompleteMode = autoCompleteMode;
		}
		#endregion
	}
}