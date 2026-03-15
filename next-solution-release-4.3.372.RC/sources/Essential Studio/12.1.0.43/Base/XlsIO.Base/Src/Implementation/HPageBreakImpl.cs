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
using THPageBreak = Syncfusion.XlsIO.Parser.Biff_Records.HorizontalPageBreaksRecord.THPageBreak;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class contains information about Horizontal Page Break.
  /// </summary>
  public class HPageBreakImpl
    : CommonObject
    , IHPageBreak
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    ///
    /// </summary>
    public void Delete()
    {
      throw new NotImplementedException();
    }
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Page break extent.
    /// </summary>
    private ExcelPageBreakExtent m_extent = ExcelPageBreakExtent.PageBreakPartial;
    /// <summary>
    /// Type of the page break.
    /// </summary>
    private ExcelPageBreak m_type = ExcelPageBreak.PageBreakManual;
    /// <summary>
    /// Record with horizontal page break.
    /// </summary>
    private THPageBreak m_HPageBreak = null;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    #endregion

    #region IHPageBreak properties
    /// <summary>
    /// Read-only. Returns page break extent.
    /// </summary>
    public ExcelPageBreakExtent Extent
    {
      get
      {
        // TODO: Add HPageBreakImpl.Extent getter implementation.
        return m_extent;
      }
    }

    /// <summary>
    /// Location of the page break.
    /// </summary>
    public IRange Location
    {
      get
      {
        return m_sheet.Range[ m_HPageBreak.Row + 1, m_HPageBreak.StartColumn + 1
          , m_HPageBreak.Row + 1, m_HPageBreak.EndColumn + 1 ];
      }
      set
      {
        m_HPageBreak.StartColumn = ( ushort )( value.Column - 1 );
        m_HPageBreak.EndColumn = ( ushort )( value.LastColumn - 1 );
        m_HPageBreak.Row = ( ushort )( value.Row - 1 );
      }
    }

    /// <summary>
    /// Type of the page break.
    /// </summary>
    public ExcelPageBreak Type
    {
      get
      {
        // TODO: Add HPageBreakImpl.Type getter implementation.
        return m_type;
      }
      set
      {
        // TODO: Add HPageBreakImpl.Type setter implementation.
        m_type = value;
      }
    }

    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates page break and sets its Application and Parent 
    /// properties to the specified value.
    /// </summary>
    /// <param name="application">Application object for the new page break.</param>
    /// <param name="parent">Parent object for the new page break.</param>
    private HPageBreakImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Recovers page break from the stream.Since there is no corresponding Biff Record,
    /// this constructor should not be used.
    /// </summary>
    /// <param name="application">Application object for the new page break.</param>
    /// <param name="parent">Parent object for the new page break.</param>
    /// <param name="reader"></param>
    private HPageBreakImpl( IApplication application, object parent, BiffReader reader )
      : this( application, parent )
    {
    }
    /// <summary>
    /// Constructs page break using application, parent object, and THPageBreak object.
    /// Always creates manual page break.
    /// </summary>
    /// <param name="application">Application object for the page break.</param>
    /// <param name="parent">Parent object for the page break.</param>
    /// <param name="pagebreak">THPageBreak for create instance.</param>
    [ CLSCompliant( false ) ]
    public HPageBreakImpl( IApplication application, object parent, THPageBreak pagebreak )
      : this( application, parent )
    {
      m_HPageBreak = pagebreak;
      m_type = ExcelPageBreak.PageBreakManual;
    }
    /// <summary>
    /// Constructs page break
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent">Application object for the page break.</param>
    /// <param name="location">Page break location.</param>
    public HPageBreakImpl( IApplication application, object parent, IRange location )
      : this( application, parent )
    {
      m_HPageBreak = new THPageBreak();
      m_HPageBreak.Row = ( ushort )( location.Row - 1 );
      m_HPageBreak.StartColumn = ( ushort )( location.Column - 1 );
      m_HPageBreak.EndColumn = ( ushort )( location.LastColumn - 1 );
    }
  #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    [ CLSCompliant( false ) ]
    public THPageBreak HPageBreak
    {
      get
      {
        if( m_HPageBreak == null )
          throw new ArgumentNullException( "HPageBreak" );

        //m_HPageBreak.StartColumn = ( ushort ) ( m_sheet.PrintArea.Column - 1 );
        //m_HPageBreak.EndColumn = ( ushort ) ( m_sheet.PrintArea.LastColumn - 1 );

        return m_HPageBreak;
      }
      set
      {
        m_HPageBreak = value;
      }
    }
    /// <summary>
    /// Returns zero-based row index of the break location. Read-only.
    /// </summary>
    public int Row
    {
        get
        {
            return m_HPageBreak.Row;
        }
        internal set
        {
            m_HPageBreak.Row = (ushort)(value);
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
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
    public HPageBreakImpl Clone( object parent )
    {
      HPageBreakImpl result = ( HPageBreakImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();

      m_HPageBreak = ( THPageBreak )m_HPageBreak.Clone();

      return result;
    }
    #endregion
  }
}
