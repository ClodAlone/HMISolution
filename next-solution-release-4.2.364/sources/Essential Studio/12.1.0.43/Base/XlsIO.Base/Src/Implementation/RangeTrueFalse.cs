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
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class contains stores true/false/undefined values for ranges.
  /// </summary>
  class RangeTrueFalse
  {
    #region Members
    /// <summary>
    /// Object that contains ranges with value equal to true.
    /// </summary>
    private RangesOperations m_trueValues = new RangesOperations();
    /// <summary>
    /// Object that contains ranges with value equal to false.
    /// </summary>
    private RangesOperations m_falseValues = new RangesOperations();
    #endregion

    #region Methods
    /// <summary>
    /// Returns value for the range object.
    /// </summary>
    /// <param name="range">Range to get information about.</param>
    /// <returns>Value for the range.</returns>
    public bool? GetRangeValue( ICombinedRange range )
    {
      Rectangle[] arrRectangles = range.GetRectangles();
      bool? result = null;

      if( m_trueValues.Contains( arrRectangles ) )
      {
        result = true;
      }
      else if( m_falseValues.Contains( arrRectangles ) )
      {
        result = false;
      }

      return result;
    }
    /// <summary>
    /// Sets property value for the range.
    /// </summary>
    /// <param name="range">Range to set value for.</param>
    /// <param name="value">Value to set.</param>
    public void SetRange( ICombinedRange range, bool? value )
    {
      Rectangle[] arrRectangles = range.GetRectangles();

      if( value == null )
      {
        m_trueValues.Remove( arrRectangles );
        m_falseValues.Remove( arrRectangles );
      }
      else if( value == true )
      {
        m_trueValues.AddRectangles( arrRectangles );
        m_falseValues.Remove( arrRectangles );
      }
      else
      {
        m_trueValues.Remove( arrRectangles );
        m_falseValues.AddRectangles( arrRectangles );
      }
    }
    /// <summary>
    /// Clears all ranges.
    /// </summary>
    public void Clear()
    {
      m_trueValues.Clear();
      m_falseValues.Clear();
    }
    #endregion
  }
}
