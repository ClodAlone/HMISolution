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

using System;
using System.Collections;
using System.Diagnostics;

using SFArrayListType = Syncfusion.XlsIO.Implementation.Collections.SFArrayList<object>;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Implements a two-dimensional table that holds an SFArrayList of rows. Each row
  /// is an SFArrayList of objects.
  /// </summary>
  /// <remarks>
  /// <p>This is a memory efficient way to represent a table where values can remain empty. Only rows
  /// that actually contain data will allocate an SFArrayList and the array only holds
  /// as many objects as the specific row contains columns.</p>
  /// <p>When you access data that are out of range, an empty () object will be returned.
  /// If you set data that are out of range, an exception will be thrown. If you set data for
  /// a row that is empty, the row will be allocated before the value is stored.</p>
  /// <p>SFTable provides methods that let you insert, remove or rearrange columns or m_arrRows
  /// in the table.</p>
  /// </remarks>
  public class SFTable : ICloneable
  {
    #region Class members
    /// <summary>
    /// Number of m_arrRows in the collection.
    /// </summary>
    private int m_iRowCount;
    /// <summary>
    /// Number of columns in the collection.
    /// </summary>
    private int m_iColumnCount;
    /// <summary>
    /// Collection of rows.
    /// </summary>
    private SFArrayListType m_arrRows;
    /// <summary>
    /// Number of cells in the collection.
    /// </summary>
    private int m_iCellCount = 0;
    #endregion

    #region Class Initialize/Finalize methods
    /// <overload>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class.
    /// </overload>
    /// <summary>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class that is empty.
    /// </summary>
    /// <param name="iRowCount">Number of rows in the collection.</param>
    /// <param name="iColumnCount">Number of columns in the collection.</param>
    public SFTable( int iRowCount, int iColumnCount )
    {
      m_iRowCount = iRowCount;
      m_iColumnCount = iColumnCount;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class and optional copies of data from an existing table.
    /// </summary>
    /// <param name="data">Table to copy.</param>
    /// <param name="clone">Indicates whether to copy data.</param>
    protected SFTable( SFTable data, bool clone )
    {
      m_iRowCount = data.m_iRowCount;
      m_iColumnCount = data.m_iColumnCount;

      if( data.m_arrRows != null && clone )
      {
        m_iCellCount = data.m_iCellCount;
        m_arrRows = ( SFArrayListType )data.m_arrRows.Clone();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class and optional copies of data from an existing table.
    /// </summary>
    /// <param name="data">Table to copy.</param>
    /// <param name="clone">Indicates whether to copy data.</param>
    /// <param name="parent">Parent object for the new items.</param>
    protected SFTable( SFTable data, bool clone, object parent )
    {
      m_iRowCount = data.m_iRowCount;
      m_iColumnCount = data.m_iColumnCount;

      if( data.m_arrRows != null && clone )
      {
        m_iCellCount = data.m_iCellCount;
        m_arrRows = ( SFArrayListType )data.m_arrRows.Clone( parent );
      }
    }
    #endregion

    #region ICloneable methods
    /// <summary>
    ///   <para>Creates a deep copy of the <see cref="SFTable" />.</para>
    /// </summary>
    /// <returns>
    ///   <para>A deep copy of the <see cref="SFTable" />.</para>
    /// </returns>
    public virtual object Clone()
    {
      return new SFTable( this, true );
    }
    /// <summary>
    ///   <para>Creates a deep copy of the <see cref="SFTable" />.</para>
    /// </summary>
    /// <param name="parent">Parent object for the items.</param>
    /// <returns>
    ///   <para>A deep copy of the <see cref="SFTable" />.</para>
    /// </returns>
    public virtual object Clone( object parent )
    {
      return new SFTable( this, true, parent );
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns the SFArrayList from all rows.
    /// </summary>
      public SFArrayListType Rows
    {
      get
      {
        if( m_arrRows == null )
            m_arrRows = new SFArrayListType();

        return m_arrRows;
      }
    }
    /// <summary>
    /// Gets the number of rows contained in the <see cref="SFTable" />. Read-only.
    /// </summary>
    public int RowCount
    {
      get
      {
        return m_iRowCount;
      }
    }

    /// <summary>
    /// Gets the number of columns contained in the <see cref="SFTable" />. Read-only.
    /// </summary>
    public int ColCount
    {
      get
      {
        return m_iColumnCount;
      }
    }

    /// <summary>
    /// Returns number of cells in the collection. Read-only.
    /// </summary>
    public int CellCount
    {
      get
      {
        return m_iCellCount;
      }
    }
    #endregion

    #region Class methods

    /// <summary>
    ///   <para>Removes all elements from the <see cref="SFTable" />.</para>
    /// </summary>
    public void Clear()
    {
      m_arrRows = null;
    }

    /// <summary>
    /// Creates a collection of cells for a row.
    /// </summary>
    /// <returns>An SFArrayList or derived object for the cell collection.</returns>
      public virtual SFArrayListType CreateCellCollection()
    {
        return new SFArrayListType();
    }

    /// <summary>
    ///   <para>Indicates whether an element is at the specified coordinates in the <see cref="SFTable" />.</para>
    /// </summary>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="colIndex">The zero-based column index.</param>
    /// <returns>
    ///   <para>
    ///     <see langword="true" /> if an element exists at the specified coordinates in the <see cref="SFTable" />;
    ///  <see langword="false" /> otherwise.</para>
    /// </returns>
    public bool Contains( int rowIndex, int colIndex )
    {
      if( rowIndex < 0 || rowIndex >= m_iRowCount 
        || colIndex < 0 || colIndex >= m_iColumnCount )
        return false;

      SFArrayListType arrRow = Rows[ rowIndex ] as SFArrayListType;

      return ( arrRow == null )
        ? false
        : arrRow[ colIndex ] != null;
    }

    /// <summary>
    ///   <para>Gets / sets an element at the specified coordinates in the <see cref="SFTable" />.</para>
    /// </summary>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="colIndex">The zero-based column index.</param>
    /// <remarks>
    /// If you query for an element and the coordinates are out of range, an empty (<see langword="null" />) object will be returned.<para/>
    /// If you set an element and the the coordinates are out of range, an exception is thrown.
    /// </remarks>
    public object this[ int rowIndex, int colIndex ]
    {
      get
      {
        if( rowIndex >= m_iRowCount || rowIndex < 0
          || colIndex >= m_iColumnCount || colIndex < 0 )
          return null;

        SFArrayListType arrRow = Rows[ rowIndex ] as SFArrayListType;

        return ( arrRow == null )
          ? null
          : arrRow[ colIndex ];
      }
      set
      {
        if( rowIndex >= m_iRowCount || rowIndex < 0 )
          throw new ArgumentOutOfRangeException( "rowIndex" );

        if( colIndex >= m_iColumnCount || colIndex < 0 )
          throw new ArgumentOutOfRangeException( "colIndex" );

        SFArrayListType arrRows = Rows;
        SFArrayListType arrRow = arrRows[ rowIndex ] as SFArrayListType;

        if( arrRow == null )
        {
          if( value == null ) return;

          arrRows[ rowIndex ] = arrRow = CreateCellCollection();
        }

        object savedValue = arrRow[ colIndex ];

        if( savedValue != null )
        {
          if( value == null ) m_iCellCount--;
        }
        else if( value != null )
        {
          m_iCellCount++;
        }

        arrRow[ colIndex ] = value;
      }
    }
    #endregion
  }
}
//#endif