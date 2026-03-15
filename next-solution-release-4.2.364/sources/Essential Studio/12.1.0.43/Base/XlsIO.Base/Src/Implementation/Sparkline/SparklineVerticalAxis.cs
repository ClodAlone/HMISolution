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

namespace Syncfusion.XlsIO.Implementation
{
  public class SparklineVerticalAxis : ISparklineVerticalAxis
  {
    #region Fields

    private double m_customValue;
    private SparklineVerticalAxisOptions m_verticalAxisOptions = SparklineVerticalAxisOptions.Automatic;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the custom Value for the Vertical axis custom option.
    /// </summary>
    /// <value>The custom.</value>
    /// <exception cref=" NotSupportedException">If the VaerticalAxisOptions is not equal to Custom</exception>
    public double CustomValue
    {
      get
      {
        return m_customValue;
      }
      set
      {
        if( VerticalAxisOptions != SparklineVerticalAxisOptions.Custom )
          throw new NotSupportedException( "It is not supported for this  Vertical Axis type" );

        m_customValue = value;
      }
    }

    /// <summary>
    /// Gets or sets the vertical axis options.
    /// </summary>
    /// <value>The vertical axis options.</value>
    public SparklineVerticalAxisOptions VerticalAxisOptions
    {
      get
      {
        return m_verticalAxisOptions;
      }
      set
      {
        m_verticalAxisOptions = value;
      }
    }

    #endregion
  }
}
