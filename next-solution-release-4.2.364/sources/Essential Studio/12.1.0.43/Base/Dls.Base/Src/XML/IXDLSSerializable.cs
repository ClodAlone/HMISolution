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
#endregion

namespace Syncfusion.DLS.XML
{
  /// <summary>
  /// Represents required functionality for serialization by XDLSReader/Writer.
  /// <remarks>Used for objects/items implementation</remarks>
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public interface IXDLSSerializable
  {
    /// <summary>
    /// Object can writes by this method own "value" properties
    /// </summary>
    /// <param name="writer"></param>
    void WriteXmlAttributes( IXDLSAttributeWriter writer );
    /// <summary>
    /// Object can writes by this method own complex/binary data.
    /// </summary>
    /// <param name="writer"></param>
    void WriteXmlContent( IXDLSContentWriter writer );
    /// <summary>
    /// Object can reads by this method own "value" properties
    /// </summary>
    /// <param name="reader"></param>
    void ReadXmlAttributes( IXDLSAttributeReader reader );
    /// <summary>
    /// Object can reads by this method own complex/binary data.
    /// </summary>
    /// <param name="reader"></param>
    bool ReadXmlContent( IXDLSContentReader reader );
    /// <summary>
    /// Gets special holder with child objects.
    /// </summary>
    XDLSHolder XDLSHolder { get; }
    /// <summary>
    /// Object can use this method for restore references.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void RestoreReference( string name, int value );
  }

  /// <summary>
  /// Represents required functionality for serialization by XDLSReader/Writer.
  /// <remarks>Used for collections/lists implementation</remarks>
  /// </summary>
  public interface IXDLSSerializableCollection : ICollectionBase
  {
    /// <summary>
    /// Collection must creates and adds new empty item.
    /// </summary>
    /// <returns></returns>
    IXDLSSerializable AddNewItem( IXDLSContentReader reader );
    /// <summary>
    /// 
    /// </summary>
    string TagItemName { get; }
  }

  /// <summary>
  /// 
  /// </summary>
  public interface IXDLSFactory
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    IXDLSSerializable Create( IXDLSContentReader reader );
  }
}