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
using Syncfusion.XlsIO.Interfaces;

#if WINRT
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif WINRT
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif


namespace Syncfusion.XlsIO.Implementation
{
  class ColorConditionValueWrapper :
    ConditionValueWrapper,
    IColorConditionValue
  {
    #region IColorConditionValue Members
    /// <summary>
    /// The color assigned to the threshold of a color scale conditional format.
    /// </summary>
    public Color FormatColorRGB
    {
      get
      {
        return Wrapped.FormatColorRGB;
      }
      set
      {
        BeginUpdate();
        Wrapped.FormatColorRGB = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns one of the constants of the XlConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined. Read-only.
    /// </summary>
    public ConditionValueType Type
    {
        get
        {
            return Wrapped.Type;
        }
        set
        {
            BeginUpdate();
            Wrapped.Type = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Returns or sets the shortest bar or longest bar threshold value for a data
    /// bar conditional format.
    /// </summary>
    public string Value
    {
        get
        {
            return Wrapped.Value;
        }
        set
        {
            BeginUpdate();
            Wrapped.Value = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Returns or sets one of the constants of the ConditionalFormatOperator enumeration, 
    /// which specifes if the threshold is "greater than" or "greater than or equal to" the threshold value.
    /// </summary>
    public ConditionalFormatOperator Operator
    {
        get
        {
            return Wrapped.Operator;
        }
        set
        {
            BeginUpdate();
            Wrapped.Operator = value;
            EndUpdate();
        }
    }

    #endregion

    #region Properties
    /// <summary>
    /// Returns wrapped object.
    /// </summary>
    new private ColorConditionValue Wrapped
    {
      get
      {
        return base.Wrapped as ColorConditionValue;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the wrapped.
    /// </summary>
    /// <param name="value">Object to wrap.</param>
    /// <param name="parent">Parent object.</param>
    public ColorConditionValueWrapper( IConditionValue value, IOptimizedUpdate parent )
      : base( value, parent )
    {
    }
    #endregion
  }
}
