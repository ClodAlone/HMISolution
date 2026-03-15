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
    /// <summary>
    /// Represents a defined name for a range of cells. Names can be
    /// either built-in names such as Database, Print_Area, and
    /// Auto_Open or custom names.
    /// </summary>
    public interface IMetaProperty
    {
        string Value { get; set; }
        string Name { get; }
    }
}
