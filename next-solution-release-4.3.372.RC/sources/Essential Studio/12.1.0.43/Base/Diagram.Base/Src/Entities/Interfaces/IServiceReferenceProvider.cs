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

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Used for get references from provider.
	/// </summary>
	public interface IServiceReferenceProvider
	{
		/// <summary>
		/// Get the service reference from provider.
		/// </summary>
        /// <param name="typeHandle">The type.</param>
		/// <returns></returns>
        object ProvideServiceReference( RuntimeTypeHandle typeHandle );
	}
}
