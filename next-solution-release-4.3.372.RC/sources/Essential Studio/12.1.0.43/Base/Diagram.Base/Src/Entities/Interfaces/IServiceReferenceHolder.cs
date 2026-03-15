#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Used to update all internal references from given provider.
	/// </summary>
	public interface IServiceReferenceHolder
	{
		/// <summary>
		/// Updates the service references from service provider.
		/// </summary>
		/// <param name="provider">The service provider.</param>
		void UpdateServiceReferences( IServiceReferenceProvider provider );
	}
}
