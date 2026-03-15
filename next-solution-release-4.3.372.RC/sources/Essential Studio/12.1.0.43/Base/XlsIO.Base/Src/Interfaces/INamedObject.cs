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
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// Implement objects that contains Name.
  /// </summary>
  public interface INamedObject
  {
    #region Interface Properties
    /// <summary>
    /// Name of the object.
    /// </summary>
    string Name { get; }
    #endregion
  }
  /// <summary>
  /// INamedObject interface declaration - object with name,
  /// index, and ability to serialize.
  /// </summary>
  [ CLSCompliant( false ) ]
  public interface ISerializableNamedObject : INamedObject
  {
    #region Interface Properties
    /// <summary>
    /// Name of the object.
    /// </summary>
    new string Name { get; set; }
    /// <summary>
    /// Index of the object in the collection.
    /// </summary>
    int RealIndex { get; set; }
    #endregion

    #region Interface Methods
    /// <summary>
    /// Serializes this object into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive records of the object.
    /// </param>
    void Serialize( OffsetArrayList records );
    #endregion

    #region Interface events
    /// <summary>
    /// 
    /// </summary>
    event ValueChangedEventHandler NameChanged;
    #endregion
  }
}
