#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

#if !DOCIO && (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#endif
#if DOCIO 
#if (SILVERLIGHT) && !(WINRT ) 
using Syncfusion.DocIO.Implementation.Silverlight;
#elif WP
using Syncfusion.DocIO.Implementation.WP;
#endif
#elif !DOCIO && WP
using Syncfusion.XlsIO.Implementation.WP;
#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  public interface IPropertyData
  {
    /// <summary>
    /// Gets property value.
    /// </summary>
    object Value { get; }
    /// <summary>
    /// Sets type of the variant. Write-only.
    /// </summary>
    VarEnum Type { get; }
    /// <summary>
    /// Name of the property.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets property id.
    /// </summary>
    int Id { get; set; }
    /// <summary>
    /// Sets property value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="type">Type of the property to set.</param>
    bool SetValue( object value, PropertyType type );
  }
}
