#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using Syncfusion.CompoundFile.XlsIO;

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Summary description for ICustomDocumentProperties.
	/// </summary>
	public interface ICustomDocumentProperties
	{
    #region Interface properties
    /// <summary>
    /// Returns single entry from the collection. Creates new entry
    /// if property with specified name is not found. Read-only.
    /// </summary>
    IDocumentProperty this[ string strName ] { get; }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    IDocumentProperty this[ int iIndex ] { get; }
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Count { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Removes specified object from the collection.
    /// </summary>
    /// <param name="strName">Property name.</param>
    void Remove( string strName );
    /// <summary>
    /// Checks whether collection contains property with specified name.
    /// </summary>
    /// <param name="strName">Name to check.</param>
    /// <returns>True if property is contained by collection; false otherwise.</returns>
    bool Contains( string strName );
    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    void Clear();
    #endregion
	}
}
