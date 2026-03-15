#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// Class used for holding border settings.
  /// </summary>
  public class BorderSettingsHolder
    : IBorder
    , ICloneable
  {
    #region Members
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write ExcelKnownColors.
    /// </summary>
    private ColorObject m_color = new ColorObject( ExcelKnownColors.None );
    /// <summary>
    /// Represents border line style.
    /// </summary>
    private ExcelLineStyle m_lineStyle;
    /// <summary>
    /// This field is used only by Diagonal borders. For any other
    /// border index property will have no influence.
    /// </summary>
    private bool m_bShowDiagonalLine;
    /// <summary>
    /// Specifies a boolean value that indicate whether the border setting has a empty border
    /// </summary>
    private bool m_bIsEmptyBorder = true;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the BorderSettingsHolder class.
    /// </summary>
    public BorderSettingsHolder()
    {
    }
    #endregion

    #region IBorder Members
    /// <summary>
    /// Gets or sets the primary color of the object.
    /// Read/write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return ( m_color.ColorType == ColorType.Indexed ) ?
          ( ExcelKnownColors )m_color.Value :
          ExcelKnownColors.None;
      }
      set
      {
        m_color.SetIndexed( value );
      }
    }
    /// <summary>
    /// Gets color of the border.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Gets or sets color of the border.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        return ( m_color.ColorType == ColorType.RGB ) ?
          ColorExtension.FromArgb( m_color.Value ) :
          ColorExtension.Empty;
      }
      set
      {
        m_color.SetRGB( value );
      }
    }

    /// <summary>
    /// Gets or sets the line style for the border. Read/write ExcelLineStyle.
    /// </summary>
    public Syncfusion.XlsIO.ExcelLineStyle LineStyle
    {
      get
      {
        return m_lineStyle;
      }
      set
      {
        m_lineStyle = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show Diagonal lines. This property is used only by Diagonal borders. For any other border
    /// index property will have no influence.
    /// </summary>
    public bool ShowDiagonalLine
    {
      get
      {
        return m_bShowDiagonalLine;
      }
      set
      {
        m_bShowDiagonalLine = value;
      }
    }

    /// <summary>
    /// Specifies a boolean value that indicate whether the border setting has a empty border
    /// </summary>  
    internal bool IsEmptyBorder
    {
        get
        {
            return m_bIsEmptyBorder;
        }
        set
        {
            m_bIsEmptyBorder = value;
        }
    }
    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a shallow copy of the current System.Object.
    /// </summary>
    /// <returns>A shallow copy of the current System.Object.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }
    #endregion
  }
}
