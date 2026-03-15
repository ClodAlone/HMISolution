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
using System.Collections;
#endregion

namespace Syncfusion.XlsIO
{
    public interface IMetaProperties:IEnumerable
    {
        #region Interface properties
        IMetaProperty this[int iIndex] { get; }
        /// <summary>
        /// Returns number of elements in the collection. Read-only.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Returns Xml Schema
        /// </summary>
        string SchemaXml { get; }
        /// <summary>
        /// Returns Xml Schema
        /// </summary>
        IApplication Application { get; }
        /// /// <summary>
        /// Returns Xml Schema
        /// </summary>
        object Parent { get; }
        /// <summary>
        /// Checks whether the Name object is present in the collection or not
        /// </summary>
        /// <param name="name">Name object to check whether it is present or not.</param>
        /// <returns></returns>
        IMetaProperty GetItemByInternalName(string InternalName);

        #endregion

    }
}
