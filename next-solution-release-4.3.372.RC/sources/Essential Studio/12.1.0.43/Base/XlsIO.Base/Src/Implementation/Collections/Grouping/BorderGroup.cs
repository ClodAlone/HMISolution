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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for BorderGroup.
	/// </summary>
	public class BorderGroup
    : CommonObject
    , IBorder
	{
    #region Class members
    /// <summary>
    /// Border index.
    /// </summary>
    private ExcelBordersIndex m_index;
    /// <summary>
    /// Parent style group.
    /// </summary>
    private BordersGroup m_bordersGroup;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="index">Border index.</param>
    public BorderGroup( IApplication application, object parent, ExcelBordersIndex index )
      : base( application, parent )
    {
      m_index = index;
      FindParents();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_bordersGroup = FindParent( typeof( BordersGroup ) ) as BordersGroup;

      if( m_bordersGroup == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent borders group." );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the group. Read-only.
    /// </summary>
    public IBorder this[ int index ]
    {
      get
      {
        return m_bordersGroup[ index ][ m_index ];
      }
    }
    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_bordersGroup.GroupCount;
      }
    }
    #endregion

    #region IBorder Members
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write Long.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 )
          return ExcelKnownColors.None;

        ExcelKnownColors result = this[ 0 ].Color;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Color )
            return ExcelKnownColors.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].Color = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write Long.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Returns color of the border.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ColorExtension.Empty;

        Color result = this[ 0 ].ColorRGB;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].ColorRGB )
            return ColorExtension.Empty;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].ColorRGB = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the line style for the border. Read/write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle LineStyle
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return ExcelLineStyle.None;

        ExcelLineStyle result = this[ 0 ].LineStyle;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].LineStyle )
            return ExcelLineStyle.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].LineStyle = value;
        }
      }
    }

    /// <summary>
    /// This property is used only by Diagonal borders. For any other border
    /// index property will have no influence.
    /// </summary>
    public bool ShowDiagonalLine
    {
      get
      {
        int iCount = Count;

        if( iCount == 0 ) return false;

        bool result = this[ 0 ].ShowDiagonalLine;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].ShowDiagonalLine )
            return false;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          this[ i ].ShowDiagonalLine = value;
        }
      }
    }

    #endregion
  }
}
