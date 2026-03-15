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
#if !WinRT
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Styles
#else
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Styles
#endif
{
	/// <summary>
	/// Provides data for the <see cref="StyleInfoBase.Changed"/> event.
	/// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class StyleChangedEventArgs : SyncfusionEventArgs
	{
		StyleInfoProperty sip;

		/// <summary>
		/// Initializes the object with the property that has changed.
		/// </summary>
		/// <param name="sip">Identifies the property that has changed.</param>
		public StyleChangedEventArgs(StyleInfoProperty sip)
		{
			this.sip = sip;
		}

		/// <summary>
		/// Returns the property that has changed.
		/// </summary>
		
		public StyleInfoProperty Sip
		{
			get
			{
				return sip;
			}
		}
	}
}
