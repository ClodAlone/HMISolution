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
using System.Collections;
using System.Diagnostics;
using System.Reflection;
#endregion

namespace Syncfusion.Scripting.Design
{
  public class NodeDoubleClickEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// The node name.
    /// </summary>
    private string m_name;

    /// <summary>
    /// The tag container which attached to node.
    /// </summary>
    private TagContainer m_tag;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets node name.
    /// </summary>
    public string NodeName
    {
      get
      {
        return m_name;
      }
    }

    /// <summary>
    /// Gets node tag container.
    /// </summary>
    public TagContainer TagContainer
    {
      get
      {
        return m_tag;
      }
    }
    #endregion

    #region Class Initalize/finalize methods
    /// <summary>
    /// Disabled default constructor
    /// </summary>
    private NodeDoubleClickEventArgs()
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    public NodeDoubleClickEventArgs( string name, TagContainer tag )
    {
      m_name = name;
      m_tag = tag;
    }
    #endregion
  }

  /// <summary>
  ///
  /// </summary>
  public delegate void NodeDoubleClickEventHandler( object sender, NodeDoubleClickEventArgs e );
  

  /// <summary>
  /// Indicated a node type which must be created.
  /// </summary>
  public enum TreeNodeType
  {
    /// <summary>
    /// For creating "object" type of tree node.
    /// </summary>
    Object,
    /// <summary>
    /// For creating "property" type of tree node.
    /// </summary>
    Property,
    /// <summary>
    /// For creating "item" type of tree node.
    /// </summary>
    Item,
    /// <summary>
    /// For creating "event" type of tree node.
    /// </summary>
    Event,
    /// <summary>
    /// Already created event handler.
    /// </summary>
    EventHandler
  }

  public class TagContainer
  {
    private object m_property;
    private EventInfo m_evInfo;
    private string m_name;
    private TreeNodeType m_nodeType;

    public object Property
    {
      get
      {
        return m_property;
      }
    }

    public EventInfo EventInfo
    {
      get
      {
        return m_evInfo;
      }
    }

    public string Name
    {
      get
      {
        return m_name;
      }
    }

    public TreeNodeType NodeType
    {
      get
      {
        return m_nodeType;
      }
    }

    private TagContainer()
    {
    }

    public TagContainer( object prop, string name, TreeNodeType nodeType )
    {
      if( nodeType == TreeNodeType.Event )
      {
        throw new ArgumentException( "nodeType can not be TreeNodeType.Event for this constructor" );
      }

      if( nodeType == TreeNodeType.EventHandler )
      {
        throw new ArgumentException( "nodeType can not be TreeNodeType.EventHandler for this constructor" );
      }

      this.m_property = prop;
      this.m_name = name;
      this.m_nodeType = nodeType;
    }

    public TagContainer( EventInfo evInfo, string name, TreeNodeType nodeType )
    {
      if( nodeType != TreeNodeType.Event && nodeType != TreeNodeType.EventHandler )
      {
        throw new ArgumentException( "nodeType must be equal TreeNodeType.Event or TreeNodeType.EventHabndler for this constructor" );
      }

      this.m_evInfo = evInfo;
      this.m_name = name;
      this.m_nodeType = nodeType;
    }
  }

  public class TagEventContainer : TagContainer
  {
    private string m_className;

    public string ClassName
    {
      get
      {
        return this.m_className;
      }
      set
      {
        if( this.m_className != value )
        {
          this.m_className = value;
        }
      }
    }

    public TagEventContainer( EventInfo evInfo, string name, TreeNodeType nodeType )
      : base( evInfo, name, nodeType )
    {
    }
  }

  public class AssemblyTypeMap
  {
    private static Hashtable htTypeMap = new Hashtable();

    public static Type GetType( string typename )
    {
      // If the ScriptObject typemap does not contain the type for this object, then obtain the 
      // type and add it to the hashtable.
      if( AssemblyTypeMap.htTypeMap.Contains( typename ) == false )
      {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach( Assembly assmbly in assemblies )
        {
          Type objtype = assmbly.GetType( typename, false );
          if( objtype != null )
          {
            AssemblyTypeMap.htTypeMap.Add( typename, objtype );
            break;
          }
        }
      }
      return AssemblyTypeMap.htTypeMap[ typename ] as Type;
    }
  }

}