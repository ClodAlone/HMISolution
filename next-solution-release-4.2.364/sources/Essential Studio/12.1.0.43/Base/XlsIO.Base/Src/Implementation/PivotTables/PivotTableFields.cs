#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  public class PivotTableFields :
    CollectionBaseEx<PivotFieldImpl>,
    IPivotFields
  {
    #region Members
    private PivotTableImpl m_table;
    #endregion

    #region Properties
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="index">Item index to return.</param>
    /// <returns>Single entry from the collection.</returns>
    IPivotField IPivotFields.this[ int index ]
    {
      get
      {
        if( index < 0 || index >= Count )
          throw new ArgumentOutOfRangeException( "index" );

        return InnerList[ index ] as IPivotField;
      }
    }
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single entry from the collection.</returns>
    public IPivotField this[ string name ]
    {
      get
      {
        IPivotField result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IPivotField currentField = this[ i ];

          if( currentField.Name == name )
          {
            result = currentField;
            break;
          }
        }

        return result;
      }
    }
    #endregion

    #region Methods
    public PivotTableFields( IApplication application, object parent )
      : base( application, parent )
    {
      m_table = FindParent( typeof( PivotTableImpl ) ) as PivotTableImpl;

      if( m_table == null )
        throw new ArgumentException( "parent" );
    }
    /// <summary>
    /// Initializes new instance of the pivot fields collection.
    /// </summary>
    /// <param name="table">Parent table.</param>
    public PivotTableFields( PivotTableImpl table )
      : this( table.Application, table )
    {
      PivotCacheImpl cache = table.Cache;
      PivotCacheFieldsCollection fields = cache.CacheFields;

      for( int i = 0, len = fields.Count; i < len; i++ )
      {
        //PivotFieldImpl field = new PivotFieldImpl( fields[ i ] );
        Add( fields[ i ], table.Workbook );
      }
    }
    /// <summary>
    /// Adds new field based on the cached field.
    /// </summary>
    /// <param name="cacheField">Base cache field.</param>
    /// <param name="book">Parent workbook.</param>
    private void Add( PivotCacheFieldImpl cacheField, WorkbookImpl book )
    {
      PivotFieldImpl field = new PivotFieldImpl( cacheField, m_table );
      base.Add( field );
    }
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <param name="parent">New parent for the created object.</param>
    /// <returns></returns>
    public override object Clone( object parent )
    {
      PivotTableImpl table = parent as PivotTableImpl;

      if( table == null )
        throw new ArgumentException( "parent" );

      PivotTableFields result = new PivotTableFields( table );
      return base.Clone( parent );
    }
      
    #endregion

  }
}
