#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
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

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Represents error indicator.
	/// </summary>
	public class ErrorIndicatorImpl
    : RangesOperations
    , IErrorIndicator
  {
    #region Constants
    /// <summary>
    /// Maximum number of error indicators in the single Excel 2003 record.
    /// </summary>
    public const int MaximumIndicatorsInRecord = 1024;
    #endregion

    #region Class members
    /// <summary>
    /// Represents hide options.
    /// </summary>
    private ExcelIgnoreError m_options;
    #endregion

    #region Class properties
    /// <summary>
    /// Represents error indicator hide options.
    /// </summary>
    public ExcelIgnoreError IgnoreOptions
    {
      get
      {
        return m_options;
      }
      set
      {
        m_options = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of current class.
    /// </summary>
    /// <param name="rect">Cell range of this error indicator.</param>
    /// <param name="options">Hide option.</param>
    public ErrorIndicatorImpl( Rectangle rect, ExcelIgnoreError options )
      : this( options )
    {
      if( rect.Top < 0 || rect.Left < 0 )
        throw new ArgumentException( "Incorrect range" );

      AddRange( rect );
    }
    /// <summary>
    /// Creates new instance of current class.
    /// </summary>
    /// <param name="option">Hide option.</param>
    public ErrorIndicatorImpl( ExcelIgnoreError option )
      : base( new List<Rectangle>() )
    {
      m_options = option;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <returns>Returns cloned object.</returns>
    new public ErrorIndicatorImpl Clone()
    {
      return ( ErrorIndicatorImpl )base.Clone();
    }
    ///// <summary>
    ///// Adds new range for this error indicator.
    ///// </summary>
    ///// <param name="range"></param>
    //public void Add( Rectangle range )
    //{
    //  m_arrCells.Add( range );
    //}
    /// <summary>
    /// Adds cells range list from another error indicator.
    /// </summary>
    /// <param name="errorIndicator">Error indicator to get cells from.</param>
    public void AddCells( ErrorIndicatorImpl errorIndicator )
    {
      if( errorIndicator == null )
        return;

      AddCells( errorIndicator.CellList );
    }
    #endregion
	}
}
