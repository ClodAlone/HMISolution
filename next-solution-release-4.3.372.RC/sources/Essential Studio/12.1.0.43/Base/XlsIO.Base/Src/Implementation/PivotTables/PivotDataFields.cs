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

using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  /// <summary>
  /// Represents pivot table data field collection.
  /// </summary>
  public class PivotDataFields :
    CollectionBaseEx<PivotDataField>,
    IPivotDataFields
  {
    #region Properties
    /// <summary>
    /// Gets single entry from the collection. Read-only.
    /// </summary>
    /// <param name="index">Item's index to get from the collection.</param>
    /// <returns>Single entry from the collection.</returns>
    IPivotDataField IPivotDataFields.this[ int index ]
    {
      get
      {
        return ( PivotDataField )List[ index ];
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public PivotDataFields( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Adds new data field to the collection.
    /// </summary>
    /// <param name="field">Parent field to use.</param>
    /// <param name="name">Name for the new data field.</param>
    /// <param name="subtotal">Subtotal function for the new data field.</param>
    /// <returns>Newly added data field.</returns>
    public IPivotDataField Add( IPivotField field, string name, PivotSubtotalTypes subtotal )
    {
      PivotDataField dataField = new PivotDataField( name, subtotal, field as PivotFieldImpl );
      base.Add( dataField );

      return dataField;
    }
    #endregion
  }
}
