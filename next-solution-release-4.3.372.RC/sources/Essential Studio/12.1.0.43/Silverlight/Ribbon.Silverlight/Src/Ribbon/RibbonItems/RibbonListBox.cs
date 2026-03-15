#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents ribbon's list-box control.
	/// </summary>
    public class RibbonListBox : ListBox, IRibbonControl
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonListBox"/> class.
		/// </summary>
		public RibbonListBox()
		{
			this.DefaultStyleKey = typeof(RibbonListBox);
			this.IsTabStop = false;
		}

		#endregion
	}
}
