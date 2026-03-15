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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

using Microsoft.Vsa;

using Syncfusion.Scripting;
#endregion

namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// This control display property tree for browsable object
  /// and show properties by selected branch of tree
  /// </summary>
  [ ToolboxItem( false )
  , DesignTimeVisible( false ) ]
  public class ScriptObjectBrowser : Control
  {
    /// <summary>
    /// Minimal height for hiding box.
    /// </summary>
    private const int DEF_HEIGHT_MIN = 10;
    /// <summary>
    ///
    /// </summary>
    private const string DEF_NONE = ".... nothing ....";

    /// <summary>
    /// The object which will be browsed in TreeView
    /// </summary>
    private ScriptObject selectedObject = null;
  
    /// <summary>
    /// Language for create drag-and-drop string.
    /// </summary>
    private ScriptLanguages m_language = ScriptLanguages.CSharp;

    /// <summary>
    /// The x coordinate of mouse cursor
    /// </summary>
    private int m_dragDropX;

    /// <summary>
    /// The y coordinate of mouse cursor
    /// </summary>
    private int m_dragDropY;

    /// <summary>
    /// Indicated that drag-and-drop process began.
    /// </summary>
    private bool m_isDragDropStart = false;

    /// <summary>
    /// Indicated that drag-and-drop process finished.
    /// </summary>
    private bool m_isDragDropEnd = false;

    private TreeNode m_selectedTreeNode = null;
    private MouseButtons m_lastMouseButton = MouseButtons.None;
    private IVsaSite m_site;

    /// <summary>
    /// Object used for browse in TreeView.
    /// Get or set.
    /// </summary>
    public ScriptObject SelectedObject
    {
      get
      {
        return this.selectedObject;
      }

      set
      {
        if( value != selectedObject )
        {
          this.selectedObject = value;
          this.RefreshTreeView();
        }
      }
    }

    public TreeView BrowserTreeView
    {
      get
      {
        return this.treeMain;
      }
    }   

    /// <summary>
    /// Sets or gets script
    /// </summary>
    public ScriptLanguages ScriptLanguage
    {
      get { return m_language; }
      set { m_language = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ Browsable( false ) ]
    public IVsaSite ScriptSite
    {
      get
      {
        return m_site;
      }
      set
      {
        if( value != m_site )
        {
          m_site = value;
        }
      }
    }

    /// <summary>
    /// Send when user is double click by tree node.
    /// </summary>
    public event NodeDoubleClickEventHandler NodeDoubleClick;

    private TreeView treeMain;
    private ImageList imageList;
    private ContextMenu contextMenu;
    private MenuItem menuItem1;
    private IContainer components;

    public ScriptObjectBrowser()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      base.Dispose( disposing );

      if( disposing )
      {
        if( components != null )
        {
          components.Dispose();
        }
      }
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new Container();
      ResourceManager resources = new ResourceManager( typeof( ScriptObjectBrowser ) );
      this.treeMain = new TreeView();
      this.imageList = new ImageList( this.components );
      this.contextMenu = new ContextMenu();
      this.menuItem1 = new MenuItem();
      this.SuspendLayout();
      // 
      // treeMain
      // 
      this.treeMain.Dock = DockStyle.Fill;
      this.treeMain.HideSelection = false;
      this.treeMain.ImageList = this.imageList;
      this.treeMain.Name = "treeMain";
      this.treeMain.Size = new Size( 264, 146 );
      this.treeMain.TabIndex = 0;
      this.treeMain.MouseDown += new MouseEventHandler( this.treeMain_MouseDown );
      this.treeMain.Click += new EventHandler( this.treeMain_Click );
      this.treeMain.MouseUp += new MouseEventHandler( this.treeMain_MouseUp );
      this.treeMain.DoubleClick += new EventHandler( this.treeMain_DoubleClick );
      this.treeMain.BeforeExpand += new TreeViewCancelEventHandler( this.treeMain_BeforeExpand );
      this.treeMain.MouseMove += new MouseEventHandler( this.treeMain_MouseMove );
      // 
      // imageList
      // 
      this.imageList.ColorDepth = ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new Size( 16, 16 );
      this.imageList.ImageStream = ( ( ImageListStreamer )( resources.GetObject( "imageList.ImageStream" ) ) );
      this.imageList.TransparentColor = Color.Transparent;
      // 
      // contextMenu
      // 
      this.contextMenu.MenuItems.AddRange( new MenuItem[]
        {
          this.menuItem1
        } );
      // 
      // menuItem1
      // 
      this.menuItem1.Index = 0;
      this.menuItem1.Text = "Create EventHandler";
      this.menuItem1.Click += new EventHandler( this.menuItem1_Click );
      // 
      // ScriptObjectBrowser
      // 
      this.Controls.AddRange( new Control[] {this.treeMain} );
      this.Name = "ScriptObjectBrowser";
      this.Size = new Size( 264, 336 );
      this.ResumeLayout( false );

    }    

    /// <summary>
    ///	Create Tree node with Tag information
    /// </summary>
    /// <param name="label">Label of TreeNode</param>
    /// <param name="nodeType">Type of node</param>
    /// <param name="obj">value of item</param>
    /// <param name="name">Text used for Drag&Drop names creation</param>
    /// <returns></returns>
    private TreeNode CreateTreeNode( string label, TreeNodeType nodeType, object obj, string name )
    {
      TreeNode node = new TreeNode( label );
      node.SelectedImageIndex = node.ImageIndex = ( int )nodeType;

      switch( nodeType )
      {
        case TreeNodeType.Object:
        case TreeNodeType.Property:
        case TreeNodeType.Item:
          node.Tag = new TagContainer( obj, name, nodeType );
          break;

        case TreeNodeType.Event:
        case TreeNodeType.EventHandler:
          node.Tag = new TagEventContainer( obj as EventInfo, name, nodeType );
          break;
      }

      return node;
    }       

    private void RefreshTreeView()
    {
      treeMain.BeginUpdate();
      treeMain.Nodes.Clear();

      if(this.selectedObject != null)
      {
        TreeNode rootnode = this.CreateTreeNode( this.selectedObject.Name,
                                                 TreeNodeType.Object, this.selectedObject,
                                                 this.selectedObject.Name );

        this.treeMain.Nodes.Add( rootnode );
        this.BuildTreeNodeItem( rootnode, this.selectedObject );
      }

      treeMain.EndUpdate();
    }

    private string GetItemString( string name )
    {
      string res = string.Empty;
      switch( m_language )
      {
        case ScriptLanguages.VisualBasic:
          res = "(\"" + name + "\")";
          break;

        default:
          res = "[\"" + name + "\"]";
          break;
      }

      return res;
    }

    /// <summary>
    /// Gets string with "property path" for specified node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private string DragNodeToString( TreeNode node )
    {
      if( node == null )
      {
        throw new ArgumentNullException( "node" );
      }

      string res = "";
      TagContainer currNodeTag = node.Tag as TagContainer;
      TreeNode parentNode = node.Parent;

      if( currNodeTag != null )
      {
        switch( currNodeTag.NodeType )
        {
          case TreeNodeType.Object:
            res = currNodeTag.Name;
            break;

          case TreeNodeType.Property:
          case TreeNodeType.Event:
            res = "." + currNodeTag.Name;
            break;

          case TreeNodeType.Item:
            res = GetItemString( currNodeTag.Name );
            break;
        }

        if( parentNode != null )
        {
          res = DragNodeToString(parentNode) + res;
        }
      }

      return res;
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnNodeDoubleClick()
    {
      if( NodeDoubleClick == null || m_selectedTreeNode == null )
      {
        return;
      }

      TagContainer tag = m_selectedTreeNode.Tag as TagContainer;

      if( tag != null && ( tag.NodeType == TreeNodeType.Event ||
        tag.NodeType == TreeNodeType.EventHandler ) )
      {
        NodeDoubleClickEventArgs args = new NodeDoubleClickEventArgs( m_selectedTreeNode.Text, tag );
        NodeDoubleClick( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_Click( object sender, EventArgs e )
    {
      if( m_selectedTreeNode != null && m_lastMouseButton == MouseButtons.Right )
      {
        TagContainer tCont = m_selectedTreeNode.Tag as TagContainer;

        if( tCont != null && tCont.NodeType == TreeNodeType.Event && tCont.Property != null )
        {
          contextMenu.Show( treeMain, new Point( m_dragDropX, m_dragDropY ) );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_DoubleClick( object sender, EventArgs e )
    {
      OnNodeDoubleClick();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void menuItem1_Click( object sender, EventArgs e )
    {
      OnNodeDoubleClick();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_BeforeExpand( object sender, TreeViewCancelEventArgs e )
    {
      if( e.Node != null )
      {
        if( e.Node.Nodes.Count > 1 || e.Node.Nodes.Count == 0 )
        {
          return;
        }

        if( e.Node.Nodes.Count == 1 && e.Node.Nodes[ 0 ].Text == DEF_NONE )
        {
          e.Node.Nodes.Clear();

          TagContainer tag = e.Node.Tag as TagContainer;

          if( tag != null )
          {
            object value = tag.Property;
            BuildTreeNodeItem( e.Node, value );
          }
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <param name="value"></param>
    protected void BuildTreeNodeItem( TreeNode node, object value )
    {
      Type type = null;
      object resolved = null;

      if(value is ScriptObject)
      {
        type = AssemblyTypeMap.GetType( ( value as ScriptObject ).TypeName );
        string name = ( value as ScriptObject ).Name;
        resolved = this.ScriptSite.GetEventSourceInstance( name, name );
      }
      else
      {
        type = value.GetType();
        resolved = value;
      }

      /// extract properties and events
      PropertyInfo[] properties = type.GetProperties();
      EventInfo[] events = type.GetEvents();

      BuildItemsCollection( node, ( resolved as ICollection ) );
      BuildPropertiesList( node, properties, resolved );
      BuildEventsList( node, events );
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <param name="collection"></param>
    protected void BuildItemsCollection( TreeNode node, ICollection collection )
    {
      if( node == null )
      {
        throw new ArgumentNullException( "node" );
      }

      if( collection == null )
      {
        return;
      }
      if( collection.Count == 0 )
      {
        return;
      }

      if( IsBrowsableCollection( collection.GetType() ) )
      {
        foreach( object item in collection )
        {
          ScriptBrowsableAttribute[] attr;

          if( IsScriptBrowsable( item.GetType(), out attr ) )
          {
            // extract item name
            PropertyInfo property = attr[ 0 ].GetItemNameProperty();

            // something wrong with declarations
            if( property == null )
            {
              throw new ApplicationException( "Not correctly set ScriptBrowsable attribute values.\n" +
                "ScriptObjects browser can not extract item name. Please check ScriptBrowsable attribute of:\n" +
                "\"" + item.GetType().Name + "\" class." );
            }

            object name = property.GetValue( item, null );
            string lblName = ( name != null ) ? name.ToString() : "null";
            string nodeLabel = string.Format( "{0} = \"{1}\"", property.Name, lblName );

            TreeNode nNode = CreateTreeNode( nodeLabel, TreeNodeType.Item, item, lblName );

            node.Nodes.Add( nNode );
            nNode.Nodes.Add( DEF_NONE );
          }
        }
      }

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <param name="properties"></param>
    protected void BuildPropertiesList( TreeNode node, PropertyInfo[] properties, object value )
    {
      if( node == null )
      {
        throw new ArgumentNullException( "node" );
      }

      if( properties == null )
      {
        throw new ArgumentNullException( "properties" );
      }

      if( properties.Length == 0 )
      {
        return;
      }

      for( int i = 0, len = properties.Length ; i < len ; i++ )
      {
        ScriptBrowsableAttribute[] attr;

        if( IsScriptBrowsable( properties[ i ], out attr ) )
        {
          PropertyInfo property = properties[ i ];
          object item = property.GetValue( value, null );

          TreeNode nNode = CreateTreeNode( property.Name, TreeNodeType.Property, item, property.Name );
          node.Nodes.Add( nNode );
          nNode.Nodes.Add( DEF_NONE );
        }
      }

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <param name="events"></param>
    protected void BuildEventsList( TreeNode node, EventInfo[] events )
    {
      if( node == null )
      {
        throw new ArgumentNullException( "node" );
      }

      if( events == null )
      {
        throw new ArgumentNullException( "events" );
      }

      if( events.Length == 0 )
      {
        return;
      }

      for( int i = 0, len = events.Length ; i < len ; i++ )
      {
        EventInfo eventInfo = events[ i ];
        TreeNode nNode = CreateTreeNode( eventInfo.Name, TreeNodeType.Event, eventInfo, eventInfo.Name );
        ( nNode.Tag as TagEventContainer ).ClassName = node.Text;
        node.Nodes.Add( nNode );
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected bool IsScriptBrowsable( MemberInfo type )
    {
      if( type == null )
      {
        return false;
      }

      ScriptBrowsableAttribute[] attr = ( ScriptBrowsableAttribute[] )
        type.GetCustomAttributes( typeof( ScriptBrowsableAttribute ), true );

      return ( attr != null && attr.Length > 0 );
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attr"></param>
    /// <returns></returns>
    protected bool IsScriptBrowsable( MemberInfo type, out ScriptBrowsableAttribute[] attr )
    {
      attr = null;

      if( type == null )
      {
        return false;
      }

      attr = ( ScriptBrowsableAttribute[] )
        type.GetCustomAttributes( typeof( ScriptBrowsableAttribute ), true );

      return ( attr != null && attr.Length > 0 );
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected bool IsBrowsableCollection( MemberInfo type )
    {
      if( type == null )
      {
        return false;
      }

      ScriptBrowsableAttribute[] attr = ( ScriptBrowsableAttribute[] )
        type.GetCustomAttributes( typeof( ScriptBrowsableAttribute ), true );

      return ( attr != null && attr.Length > 0 &&
        attr[ 0 ].PropertyType == PropertyType.Collection );
    }

    /// <summary>
    /// Called when mouse button will be pressed. Mark a start of drag-and-drop process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_MouseDown( object sender, MouseEventArgs e )
    {
      m_dragDropX = e.X;
      m_dragDropY = e.Y;
      m_isDragDropStart = true;
      m_isDragDropEnd = false;
      m_selectedTreeNode = null;
      m_lastMouseButton = MouseButtons;
      m_selectedTreeNode = treeMain.GetNodeAt( e.X, e.Y );
    }

    /// <summary>
    /// Called when mouse button will be released. Break the drag-and-drop process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_MouseUp( object sender, MouseEventArgs e )
    {
      m_isDragDropStart = false;
      m_isDragDropEnd = false;
    }

    /// <summary>
    /// Called when mouse will be move. Run drag-and-drop process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void treeMain_MouseMove( object sender, MouseEventArgs e )
    {
      if( e.Button != MouseButtons.None && m_isDragDropStart && !m_isDragDropEnd )
      {
        TreeNode node = treeMain.GetNodeAt( m_dragDropX, m_dragDropY );

        if( node != null )
        {
          string dragData = DragNodeToString( node );
          treeMain.DoDragDrop( dragData, DragDropEffects.Copy );
        }

        m_isDragDropEnd = true;

      }
    }
  }

}