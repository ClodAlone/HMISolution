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
  /// <summary>
  /// Represents condition value for colorset condition.
  /// </summary>
  public class ColorConditionValue : 
    ConditionValue,
    IColorConditionValue
  {
    #region Members
    /// <summary>
    /// Color value.
    /// </summary>
    private Color m_color;
    #endregion

    #region Properties
    /// <summary>
    /// The color assigned to the threshold of a color scale conditional format.
    /// </summary>
    public Color FormatColorRGB
    {
      get
      {
        return m_color;
      }
      set
      {
        m_color = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="type">Type of the condition.</param>
    /// <param name="value">Value of the object.</param>
    /// <param name="color">Format color.</param>
    public ColorConditionValue( ConditionValueType type, string value, Color color )
      : base( type, value )
    {
      m_color = color;
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    internal ColorConditionValue()
    {
    }
    #endregion
  }
}
