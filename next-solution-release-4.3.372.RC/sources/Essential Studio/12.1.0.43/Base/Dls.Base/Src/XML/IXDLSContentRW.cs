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
using System.Drawing.Imaging;
using System.Xml;
#endregion

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// 
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public interface IXDLSContentWriter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteChildBinaryElement( string name, byte[] value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteChildStringElement( string name, string value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteChildElement( string name, object value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="refToElement"></param>
    void WriteChildRefElement( string name, int refToElement );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    void WriteImage( Image image );
    /// <summary>
    /// 
    /// </summary>
    XmlWriter InnerWriter { get; }
  }

  /// <summary>
  /// 
  /// </summary>
  public interface IXDLSContentReader
  {
    /// <summary>
    /// 
    /// </summary>
    string TagName { get; }
    /// <summary>
    /// 
    /// </summary>
    XmlNodeType NodeType { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetAttributeValue( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool ReadChildElement( object value );
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    object ReadChildElement( Type type );
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    string ReadChildStringContent();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    byte[] ReadChildBinaryElement();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Image ReadImage();
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Image ReadImage( bool isMetafile );
    /// <summary>
    /// 
    /// </summary>
    XmlReader InnerReader { get; }
  }
}