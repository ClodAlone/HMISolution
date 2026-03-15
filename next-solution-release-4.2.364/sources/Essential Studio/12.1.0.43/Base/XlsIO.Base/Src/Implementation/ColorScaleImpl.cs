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

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation
{
  class ColorScaleImpl : IColorScale
  {
    #region Constants
    /// <summary>
    /// Default color sequence for two color gradient.
    /// </summary>
    private static readonly Color[] DefaultColors2 = new Color[]
    {
      Color.FromArgb( 0xFF, 0xFF, 0x71, 0x28 ),
      Color.FromArgb( 0xFF, 0xFF, 0xEF, 0x9C ),
    };
    /// <summary>
    /// Default color sequence for three color gradient.
    /// </summary>
    private static readonly Color[] DefaultColors3 = new Color[]
    {
      Color.FromArgb( 0xFF, 0xF8, 0x69, 0x6B ),
      Color.FromArgb( 0xFF, 0xFF, 0xEB, 0x84 ),
      Color.FromArgb( 0xFF, 0x63, 0xBE, 0x7B ),
    };
    #endregion

    #region Members
    /// <summary>
    /// A collection of individual IColorConditionValue objects.
    /// </summary>
    private IList<IColorConditionValue> m_arrCriteria = new List<IColorConditionValue>( 3 );
    #endregion

    #region IColorScale Members
    /// <summary>
    /// Returns a collection of individual IColorConditionValue objects.
    /// The IColorConditionValue object specifies the type, value, and the color
    /// of threshold criteria used in the color scale conditional format. Read-only.
    /// </summary>
    public IList<IColorConditionValue> Criteria
    {
      get
      {
        return m_arrCriteria;
      }
    }
    /// <summary>
    /// Sets number of IColorConditionValue objects in the collection. Supported values are 2 and 3.
    /// </summary>
    /// <param name="count">Number of conditions.</param>
    public void SetConditionCount( int count )
    {
      if( count < 2 || count > 3 )
        throw new ArgumentOutOfRangeException( "count", "Only 2 or 3 can be used as color count." );

      UpdateCount( count );
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the color scale object.
    /// </summary>
    public ColorScaleImpl()
    {
      const int DefaultCount = 2;
      SetConditionCount( DefaultCount );
    }
    /// <summary>
    /// Updates count of object in the collection.
    /// </summary>
    /// <param name="count">Desired number of objects.</param>
    private void UpdateCount( int count )
    {
      m_arrCriteria.Clear();

      Color[] arrColors = ( count == 2 ) ? DefaultColors2 : DefaultColors3;

      int iColorIndex = 0;
      m_arrCriteria.Add( new ColorConditionValue( ConditionValueType.LowestValue, "0", arrColors[ iColorIndex++ ] ) );

      if( count == 3 )
        m_arrCriteria.Add( new ColorConditionValue( ConditionValueType.Percentile, "50", arrColors[ iColorIndex++ ] ) );

      m_arrCriteria.Add( new ColorConditionValue( ConditionValueType.HighestValue, "0", arrColors[ iColorIndex++ ] ) );
    }
    #endregion
  }
}
