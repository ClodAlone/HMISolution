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
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Encapsulates the name and type of an object in a script.
  /// </summary>
  [ Serializable ]
  public class ScriptObject : ISerializable
  {
    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ScriptObject()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="src"></param>
    public ScriptObject( ScriptObject src )
    {
      this.name = src.Name;
      this.typeName = src.TypeName;
    }

    /// <summary>
    /// Construct a script object given a name and type.
    /// </summary>
    /// <param name="name">Name of object</param>
    /// <param name="typeName">Type of object as a string</param>
    public ScriptObject( string name, string typeName )
    {
      this.Name = name;
      this.TypeName = typeName;
    }

    /// <summary>
    /// Serialization constructor for script objects.
    /// </summary>
    /// <param name="info">Serialization state information</param>
    /// <param name="context">Streaming context information</param>
    protected ScriptObject( SerializationInfo info, StreamingContext context )
    {
      this.name = info.GetString( "name" );
      this.typeName = info.GetString( "typeName" );
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Name of object.
    /// </summary>
    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    /// <summary>
    /// Type of object as a string.
    /// </summary>
    public string TypeName
    {
      get
      {
        return this.typeName;
      }
      set
      {
        this.typeName = value;
      }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.Name + " : " + this.TypeName;
    }
    #endregion

    #region ISerializable Interface
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue( "name", this.name );
      info.AddValue( "typeName", this.typeName );
    }
    #endregion

    #region Fields
    private string name = "";
    private string typeName = "";
    #endregion
  }

  /// <summary>
  /// Type-safe collection for ScriptObject objects.
  /// </summary>
  [ Serializable() ]
  public class ScriptObjectCollection : CollectionBase
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptObj"></param>
    /// <returns></returns>
    public int Add( ScriptObject scriptObj )
    {
      return this.List.Add( scriptObj );
    }

    /// <summary>
    /// 
    /// </summary>
    public ScriptObject this[ int index ]
    {
      get
      {
        return ( ScriptObject )this.List[ index ];
      }
      set
      {
        this.List[ index ] = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptObj"></param>
    public void Remove( ScriptObject scriptObj )
    {
      this.List.Remove( scriptObj );
    }

    public bool Contains( ScriptObject scriptObj )
    {
      return this.List.Contains( scriptObj );
    }
  }
}