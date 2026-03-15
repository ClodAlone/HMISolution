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
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Summary description for ShapeStyle.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class ShapeStyle : Style
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected ShapeFormat m_shapeFormat;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets shape format
    /// </summary>
    public ShapeFormat ShapeFormat
    {
      get
      {
        return m_shapeFormat;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    public ShapeStyle( IDocument doc )
      : base( doc )
    {
      m_shapeFormat = DocumentEx.CreateShapeFormatImpl();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <param name="doc"></param>
    internal ShapeStyle( IShapeStyle style, IDocument doc )
      : base( style, doc )
    {
      m_shapeFormat.ImportContainer( style.ShapeFormat );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Cloned itself
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public override IStyle Clone( IDocument document )
    {
      throw new NotImplementedException();
      //return new ShapeStyle( this, document );
    }
    #endregion

    #region Class XDLSSerializable overrides
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( "shape-format", ShapeFormat );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );
      writer.WriteValue( XDLSConstants.TypeTag, ( Enum )StyleType.ShapeStyle );
    }

    #endregion
  }
}