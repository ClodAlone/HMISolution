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
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents Shape formatting.
  /// </summary>
  public class ShapeFormat : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int LineKey = 1;
    /// <summary>
    /// 
    /// </summary>
    private const int FillKey = 2;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets pen object for the shape if needed.
    /// </summary>
    public LineData Line
    {
      get
      {
        return this[ LineKey ] as LineData;
      }
    }
    /// <summary>
    /// Gets / sets brush object for the shape if needed.
    /// </summary>
    public FillData Fill
    {
      get
      {
        return this[ FillKey ] as FillData;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public ShapeFormat()
      : base()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal override void EnsureComposites()
    {
      EnsureComposites( FillKey, LineKey );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override FormatBase GetDefComposite( int key )
    {
      switch( key )
      {
        case FillKey:
          return GetDefComposite( FillKey, new FillData( this, FillKey, m_keysOffset ) );
        case LineKey:
          return GetDefComposite( LineKey, new LineData( this, LineKey, m_keysOffset ) );
      }
      
      return null;
    }
    /// <summary>
    /// Inits composite data for XML serialization.
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      if( PropertiesHash.Count == 0 )
      {
        XDLSHolder.SkipMe = true;
      }

      XDLSHolder.AddElement( PropertyNames.Line, this.Line );
      XDLSHolder.AddElement( PropertyNames.Fill, this.Fill );
    }
    #endregion
  }
}