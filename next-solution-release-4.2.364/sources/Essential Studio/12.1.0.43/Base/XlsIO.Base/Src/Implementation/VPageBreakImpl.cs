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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

using TVPageBreak = Syncfusion.XlsIO.Parser.Biff_Records.VerticalPageBreaksRecord.TVPageBreak;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class contains information about Vertical Page Break.
  /// </summary>
  public class VPageBreakImpl : CommonObject, IVPageBreak
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    ///
    /// </summary>
    public ExcelPageBreakExtent Extent
    {
      get
      {
        // TODO: Add VPageBreakImpl.Extent getter implementation.
        throw new NotImplementedException();
//        return ExcelPageBreakExtent.xlPageBreakPartial;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public void Delete()
    {
      // TODO: Add VPageBreakImpl.Delete implementation.
      throw new NotImplementedException();
    }
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Record with vertical page break.
    /// </summary>
    private TVPageBreak m_vPageBreak;
    /// <summary>
    /// Type of the page break.
    /// </summary>
    private ExcelPageBreak m_type = ExcelPageBreak.PageBreakManual;
    /// <summary>
    /// Represents worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    #endregion

    #region IVPageBreak Properties
    /// <summary>
    /// Location of the page break.
    /// </summary>
    public IRange Location
    {
      get
      {
        return m_sheet.Range[ ( int )m_vPageBreak.StartRow + 1, m_vPageBreak.Column + 1
          , ( int )m_vPageBreak.EndRow + 1, m_vPageBreak.Column + 1 ];
      }
      set
      {
        m_vPageBreak.Column = ( ushort )( value.Column - 1 );
        m_vPageBreak.StartRow = ( ushort )( value.Row - 1 );
        m_vPageBreak.EndRow = ( ushort )( value.LastRow - 1 );
      }
    }

    /// <summary>
    /// Type of the page break.
    /// </summary>
    public ExcelPageBreak Type
    {
      get
      {
        return m_type;
      }
      set
      {
        m_type = value;
      }
    }

    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates page break by application and parent objects.
    /// </summary>
    /// <param name="application">Application object for the page break.</param>
    /// <param name="parent">Parent object for the page break.</param>
    public VPageBreakImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Creates page break by application, parent objects and reader.
    /// </summary>
    /// <param name="application">Application object for the page break.</param>
    /// <param name="parent">Parent object for the page break.</param>
    /// <param name="reader">Reader that contains page break record.</param>
    private VPageBreakImpl( IApplication application, object parent, BiffReader reader )
      : this( application, parent )
    {
    }

    /// <summary>
    /// Constructs page break by application, parent object, and TVPageBreak object.
    /// Always creates manual page break.
    /// </summary>
    /// <param name="application">Application object for the page break.</param>
    /// <param name="parent">Parent object for the page break.</param>
    /// <param name="pagebreak">Vertical page break.</param>
    [ CLSCompliant( false ) ]
    public VPageBreakImpl( IApplication application, object parent,
      VerticalPageBreaksRecord.TVPageBreak pagebreak )
      : this( application, parent )
    {
      m_vPageBreak = pagebreak;
      m_type = ExcelPageBreak.PageBreakManual;
    }
    /// <summary>
    /// Initializes new instance of the vertical page break.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="location">Page break location.</param>
    public VPageBreakImpl( IApplication application, object parent,
      IRange location )
      : this( application, parent )
    {
      m_vPageBreak = new TVPageBreak();
      m_vPageBreak.Column = ( ushort )( location.Column - 1 );
      m_vPageBreak.StartRow = ( ushort )( location.Row - 1 );
      m_vPageBreak.EndRow = ( ushort )( location.LastRow - 1);
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / Sets vertical page break record.
    /// </summary>
    [ CLSCompliant( false ) ]
    public TVPageBreak VPageBreak
    {
      get
      {
        if( m_vPageBreak == null )
          throw new ArgumentNullException( "VPageBreak" );

        //m_vPageBreak.StartRow = ( ushort ) ( m_sheet.PrintArea.Row - 1 );
        //m_vPageBreak.EndRow = ( ushort ) ( m_sheet.PrintArea.LastRow - 1 );

        return m_vPageBreak;
      }
      set
      {
        m_vPageBreak = value;
      }
    }
    /// <summary>
    /// Returns zero-based column index of the page break. Read-only.
    /// </summary>
    public int Column
    {
        get
        {
            return m_vPageBreak.Column;
        }
        internal set
        {
            m_vPageBreak.Column = (ushort)(value);
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Finds parent object.
    /// </summary>
    private void FindParents()
    {
      object l_parent = FindParent( typeof( WorksheetImpl ) );

      if( l_parent == null )
        throw new ArgumentNullException( "Can't find parent worksheet" );

      m_sheet = ( WorksheetImpl ) l_parent;
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent for new instance.</param>
    /// <returns>A clone of the current instance.</returns>
    public VPageBreakImpl Clone( object parent )
    {
      VPageBreakImpl result = ( VPageBreakImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();

      m_vPageBreak = ( TVPageBreak )m_vPageBreak.Clone();

      return result;
    }
    #endregion
  }
}
