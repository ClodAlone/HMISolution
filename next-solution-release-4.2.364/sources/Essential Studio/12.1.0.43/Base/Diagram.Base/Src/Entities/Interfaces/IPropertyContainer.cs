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
	/// Template interface type that contains Container Name property and GetPropertyContainerByName method.
	/// </summary>
	public interface IPropertyContainer
	{
		/// <summary>
		/// Gets the name of the property container by.
		/// </summary>
		/// <param name="strPropertyName">Name of the property.</param>
		/// <returns></returns>
		object GetPropertyContainerByName( string strPropertyName );
		/// <summary>
		/// Gets the full name of the container.
		/// </summary>
		/// <value>The full name of the container.</value>
		string FullContainerName{ get; }
	}
}
