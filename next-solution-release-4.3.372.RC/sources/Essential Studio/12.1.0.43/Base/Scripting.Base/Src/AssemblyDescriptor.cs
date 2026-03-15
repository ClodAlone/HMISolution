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
using System.Reflection;
using System.Runtime.Serialization;

namespace Syncfusion.Scripting
{
  /// <summary>
  /// Summary description for AssemblyDescriptor.
  /// </summary>
  [ Serializable ]
  public class AssemblyDescriptor : ISerializable
  {
    protected String strDisplayName = String.Empty;
    protected String strFullName = String.Empty;
    protected String strLocation = String.Empty;

    public String DisplayName
    {
      get
      {
        return this.strDisplayName;
      }
    }

    public String FullName
    {
      get
      {
        return this.strFullName;
      }
    }

    public String Location
    {
      get
      {
        return this.strLocation;
      }
    }

    protected AssemblyDescriptor()
    {
    }

    public AssemblyDescriptor( String displayname, String fullname, String location )
    {
      if( location != String.Empty )
      {
        this.InitializeDescriptorFromLocation( location );
      }
      else
      {
        this.strDisplayName = displayname;
        this.strFullName = fullname;
      }
    }

    public AssemblyDescriptor( String location )
    {
      this.InitializeDescriptorFromLocation( location );
    }

    public AssemblyDescriptor( Assembly srcassembly )
    {
      if( srcassembly != null )
      {
        this.strDisplayName = srcassembly.GetName().Name;
        this.strFullName = srcassembly.FullName;
        if( srcassembly.GlobalAssemblyCache == true )
        {
          this.strLocation = srcassembly.Location.Substring( srcassembly.Location.LastIndexOf( '\\' ) + 1 );
        }
        else
        {
          this.strLocation = srcassembly.Location;
        }
      }
    }

    protected void InitializeDescriptorFromLocation( String location )
    {
      try
      {
        Assembly loadedassembly = Assembly.LoadFrom( location );
        if( loadedassembly != null )
        {
          this.strDisplayName = loadedassembly.GetName().Name;
          this.strFullName = loadedassembly.FullName;
          this.strLocation = location;
        }
      }
      catch( Exception e )
      {
        Debug.Assert( false, e.Message );
      }
    }

    protected AssemblyDescriptor( SerializationInfo info, StreamingContext context )
    {
      this.strDisplayName = info.GetString( "DisplayName" );
      this.strFullName = info.GetString( "FullName" );
      this.strLocation = info.GetString( "Location" );
    }

    void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue( "DisplayName", this.strDisplayName );
      info.AddValue( "FullName", this.strFullName );
      info.AddValue( "Location", this.strLocation );
    }

    public override string ToString()
    {
      return this.strDisplayName;
    }
  }

  /// <summary>
  /// Type-safe collection for AssemblyDescriptor objects.
  /// </summary>
  [ Serializable() ]
  public class AssemblyDescriptorCollection : CollectionBase
  {
    public int Add( AssemblyDescriptor descriptor )
    {
      return this.List.Add( descriptor );
    }

    public AssemblyDescriptor this[ int index ]
    {
      get
      {
        return ( AssemblyDescriptor )this.List[ index ];
      }
      set
      {
        this.List[ index ] = value;
      }
    }

    public void Remove( AssemblyDescriptor descriptor )
    {
      this.List.Remove( descriptor );
    }

    public bool Contains( AssemblyDescriptor descriptor )
    {
      return this.List.Contains( descriptor );
    }
  }
}