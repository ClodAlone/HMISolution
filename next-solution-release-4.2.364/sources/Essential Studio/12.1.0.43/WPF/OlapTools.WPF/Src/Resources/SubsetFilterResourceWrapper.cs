#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Olap.Resources
{
	internal sealed class SubsetFilterResourceWrapper
	{
		#region Constants
		
		const string OLAPTOOLS_SUBSETFILTER_DROPDOWN_TEXT = "OlapTools_SubsetFilter_DropDown_Text";
		
		#endregion
		
		#region Members
		
		private string olapToolsSubsetFilterDropDownText;
		
		#endregion
	
		#region Constructor

        public SubsetFilterResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapToolsSubsetFilterDropDownText = SR.GetString(ci, OLAPTOOLS_SUBSETFILTER_DROPDOWN_TEXT);
		} 
		
		#endregion
		
		#region Properties
		
		public string OlapToolsSubsetFilterDropDownText
        {
            get { return olapToolsSubsetFilterDropDownText; }
            set { olapToolsSubsetFilterDropDownText = value; }
        }
		
		#endregion
	}
}
