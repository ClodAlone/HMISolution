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

using System;
using System.Drawing;

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// 
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public interface IXDLSAttributeWriter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteValue( string name, float value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteValue( string name, int value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteValue( string name, string value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteValue( string name, Enum value );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void WriteValue( string name, bool value );
    /// <summary>
    /// Writes color as string to XML.
    /// </summary>
    /// <param name="name">Name of attribute.</param>
    /// <param name="value">Color structure.</param>
    void WriteValue( string name, Color value );
    /// <summary>
    /// Writes DateTime as string to XML.
    /// </summary>
    /// <param name="name">Name of attribute.</param>
    /// <param name="value">Color structure.</param>
    void WriteValue( string name, DateTime value );
  }

  /// <summary>
  /// 
  /// </summary>
  public interface IXDLSAttributeReader
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool HasAttribute( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string ReadString( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int ReadInt( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    short ReadShort( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    float ReadFloat( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool ReadBoolean( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    byte ReadByte( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    Enum ReadEnum( string name, Type enumType );
    /// <summary>
    /// Reads color from XML.
    /// </summary>
    /// <param name="name">Name of attribute.</param>
    /// <returns></returns>
    Color ReadColor( string name );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    DateTime ReadDateTime( string s );
  }
}