#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Represents collection of all custom properties in the worksheet.
  /// </summary>
  public class WorksheetCustomProperties
    : TypedSortedListEx<string, ICustomProperty>
    , IWorksheetCustomProperties
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public WorksheetCustomProperties()
    {
    }
    /// <summary>
    /// Extracts custom properties from the array of Biff records.
    /// </summary>
    /// <param name="m_arrRecords">Array to parse.</param>
    /// <param name="iCustomPropertyPos">Position of the first custom property.</param>
    public WorksheetCustomProperties( IList m_arrRecords, int iCustomPropertyPos )
    {
      if( m_arrRecords == null )
        throw new ArgumentNullException( "m_arrRecords" );

      int iCount = m_arrRecords.Count;

      if( iCustomPropertyPos < 0 || iCustomPropertyPos >= iCount )
        throw new ArgumentOutOfRangeException( "iCustomPropertyPos" );

      for( ; iCustomPropertyPos < iCount; iCustomPropertyPos++ )
      {
        CustomPropertyRecord property = m_arrRecords[ iCustomPropertyPos ] as CustomPropertyRecord;

        if( property == null ) break;

        Add( property );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection by index. Read-only.
    /// </summary>
    public ICustomProperty this[ int index ]
    {
      get
      {
        return GetByIndex( index ) as ICustomProperty;
      }
    }
    /// <summary>
    /// Returns single entry from the collection by index. Read-only.
    /// </summary>
    public ICustomProperty this[ string strName ]
    {
      get
      {
        return GetByName(strName) as ICustomProperty;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Serializes collection into as set of Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        WorksheetCustomProperty property = GetByIndex( i ) as WorksheetCustomProperty;
        property.Serialize( records );
      }
    }
    /// <summary>
    /// Adds new property to the collection.
    /// </summary>
    /// <param name="strName">Name of the new property.</param>
    /// <returns>Newly created property.</returns>
    public ICustomProperty Add( string strName )
    {
      WorksheetCustomProperty property = new WorksheetCustomProperty( strName );
      return Add( property );
    }
    /// <summary>
    /// Adds new property to the collection.
    /// </summary>
    /// <param name="property">Property to add.</param>
    /// <returns>Added property.</returns>
    public ICustomProperty Add( ICustomProperty property )
    {
      Add( property.Name, property );
      return property;
    }
    /// <summary>
    /// Adds new property to the collection.
    /// </summary>
    /// <param name="property">Property to add.</param>
    /// <returns>Added property.</returns>
    [ CLSCompliant( false ) ]
    public void Add( CustomPropertyRecord property )
    {
      if( property == null )
        throw new ArgumentNullException( "property" );

      WorksheetCustomProperty customProperty = new WorksheetCustomProperty( property );
      Add( customProperty );
    }
    #endregion
  }
}
