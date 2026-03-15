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
	/// Summary description for BordersGroup.
	/// </summary>
	public class BordersGroup
    : CollectionBaseEx<object>
    , IBorders
	{
    #region Class members
    /// <summary>
    /// Parent style group.
    /// </summary>
    private StyleGroup m_style;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public BordersGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();

      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.DiagonalDown ) );
      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.DiagonalUp ) );
      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.EdgeBottom ) );
      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.EdgeLeft ) );
      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.EdgeRight ) );
      InnerList.Add( new BorderGroup( application, this, ExcelBordersIndex.EdgeTop ) );
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_style = FindParent( typeof( StyleGroup ) ) as StyleGroup;

      if( m_style == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent style group." );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the group. Read-only.
    /// </summary>
    public IBorders this[ int index ]
    {
      get
      {
        return m_style[ index ].Borders;
      }
    }
    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int GroupCount
    {
      get
      {
        return m_style.Count;
      }
    }
    #endregion

    #region IBorders Members
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        int iCount = GroupCount;

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
        for( int i = 0, iCount = GroupCount; i < iCount; i++ )
        {
          this[ i ].Color = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read / write Color.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        int iCount = GroupCount;

        if( iCount == 0 )
          return ColorExtension.Empty;

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
        for( int i = 0, iCount = GroupCount; i < iCount; i++ )
        {
          this[ i ].ColorRGB = value;
        }
      }
    }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only, Long.
    /// </summary>
    int IBorders.Count
    {
      get
      {
        int iCount = GroupCount;

        if( iCount == 0 ) return int.MinValue;

        int result = this[ 0 ].Count;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Count )
            return int.MinValue;
        }

        return result;
      }
    }

    /// <summary>
    /// Returns a Border object that represents one of the borders of either a
    /// range of cells or a style.
    /// </summary>
    public IBorder this[ ExcelBordersIndex Index ]
    {
      get
      {
        // TODO:  Add BordersGroup.this getter implementation
        return null;
      }
    }

    /// <summary>
    /// Returns or sets the line style for the border. Read / write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle LineStyle
    {
      get
      {
        int iCount = GroupCount;

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
        for( int i = 0, iCount = GroupCount; i < iCount; i++ )
        {
          this[ i ].LineStyle = value;
        }
      }
    }

    /// <summary>
    /// Synonym for Borders.LineStyle. Read / write.
    /// </summary>
    public ExcelLineStyle Value
    {
      get
      {
        int iCount = GroupCount;

        if( iCount == 0 ) return ExcelLineStyle.None;

        ExcelLineStyle result = this[ 0 ].Value;

        for( int i = 1; i < iCount; i++ )
        {
          if( result != this[ i ].Value )
            return ExcelLineStyle.None;
        }

        return result;
      }
      set
      {
        for( int i = 0, iCount = GroupCount; i < iCount; i++ )
        {
          this[ i ].Value = value;
        }
      }
    }

    #endregion
  }
}
