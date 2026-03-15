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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Design
{
  /// <summary></summary>
  public class MultiColumnTreeViewDesigner : ControlDesigner
  {
    #region Class members
    private ISelectionService selectionService = null;

    private bool m_bSkipSelectionChanging = false;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public MultiColumnTreeView.MultiColumnTreeView Instance
    {
      get
      {
        return this.Control as MultiColumnTreeView.MultiColumnTreeView;
      }
    }
    /// <summary></summary>
    public override DesignerVerbCollection Verbs
    {
      get
      {
        DesignerVerbCollection verbs = new DesignerVerbCollection();

        verbs.Add( new DesignerVerb( "Nodes &Editor...", new EventHandler( tree_NodesEditor ) ) );
        verbs.Add( new DesignerVerb( "Columns Editor...", new EventHandler( tree_ColumnsEditor ) ) );
        verbs.Add( new DesignerVerb( "Styles Editor...", new EventHandler( tree_StylesEditor ) ) );
        verbs.Add( new DesignerVerb( "Add Node", new EventHandler( tree_AddNode ) ) );
        verbs.Add( new DesignerVerb( "Add Column", new EventHandler( tree_AddColumn ) ) );

        return verbs;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary></summary>
    public MultiColumnTreeViewDesigner() :
      base()
    {
    }

    public override void Initialize( IComponent component )
    {
      base.Initialize( component );
      selectionService = ( ISelectionService )this.GetService( typeof( ISelectionService ) );
      selectionService.SelectionChanging += new EventHandler( selectionService_SelectionChanging );
    }
    #endregion

    #region Class event handlers
    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tree_ColumnsEditor( object sender, EventArgs e )
    {
      if( this.Instance != null )
      {
        IServiceProvider provider = this.Instance.Site;

        using( MultiColumnTreeView.ColumnsEditorForm form =
          new MultiColumnTreeView.ColumnsEditorForm( this.Instance.Columns, provider ) )
        {
          if( provider != null )
          {
            IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )this.GetService( 
              typeof( IWindowsFormsEditorService ) );

            if( edSvc != null )
            {
              edSvc.ShowDialog( form );
            }
            else
            {
              form.ShowDialog();
            }
          }
          else
          {
            form.ShowDialog();
          }
        }
      }
    }

    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tree_NodesEditor( object sender, EventArgs e )
    {
      if( this.Instance != null )
      {
        IServiceProvider provider = this.Instance.Site;

        using( MultiColumnTreeView.TreeViewAdvEditorForm form =
          new MultiColumnTreeView.TreeViewAdvEditorForm( this.Instance, provider ) )
        {
          if( provider != null )
          {
            IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )this.GetService( 
              typeof( IWindowsFormsEditorService ) );

            if( edSvc != null )
            {
              edSvc.ShowDialog( form );
            }
            else
            {
              form.ShowDialog();
            }
          }
          else
          {
            form.ShowDialog();
          }
        }
      }
    }

    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tree_AddNode( object sender, EventArgs e )
    {
      MultiColumnTreeView.MultiColumnTreeView tree = this.Instance;

      if( tree != null )
      {
        MultiColumnTreeView.TreeNodeAdv node =
          new MultiColumnTreeView.TreeNodeAdv( "Node" + tree.NodeCount.ToString() );

        if( tree.SelectedNode != null && tree.SelectedNode.Parent != null )
        {
          tree.SelectedNode.Parent.Nodes.Add( node );
        }
        else
        {
          tree.Nodes.Add( node );
        }

        tree.NodeCount++;
        tree.Invalidate();
      }
    }

    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tree_AddColumn( object sender, EventArgs e )
    {
      MultiColumnTreeView.MultiColumnTreeView tree = this.Instance;

      if( tree != null )
      {
        MultiColumnTreeView.TreeColumnAdv column =
          new MultiColumnTreeView.TreeColumnAdv( "Column" + tree.Columns.Count.ToString() );
        tree.Columns.Add( column );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tree_StylesEditor( object sender, EventArgs e )
    {
      if( this.Instance != null )
      {
        IServiceProvider provider = this.Instance.Site;

        using( MultiColumnTreeView.TreeViewAdvBaseStylesEditorForm form =
          new MultiColumnTreeView.TreeViewAdvBaseStylesEditorForm( this.Instance ) )
        {
          if( provider != null )
          {
            IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )this.GetService( 
              typeof( IWindowsFormsEditorService ) );

            if( edSvc != null )
            {
              edSvc.ShowDialog( form );
            }
            else
            {
              form.ShowDialog();
            }
          }
          else
          {
            form.ShowDialog();
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void selectionService_SelectionChanging( object sender, EventArgs e )
    {
      if( m_bSkipSelectionChanging )
      {
        m_bSkipSelectionChanging = false;
        return;
      }

      foreach( Component component in selectionService.GetSelectedComponents() )
      {
        Control control = component as Control;
        if( control != null && this.Control != null )
        {
          if( this.Control.Controls.Contains( control ) )
          {
            m_bSkipSelectionChanging = true;

            ArrayList al = new ArrayList();
            al.Add( this.Control );
            selectionService.SetSelectedComponents( al );

            break;
          }
        }
      }
    }
    #endregion

#if ! ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

    private DesignerActionListCollection actionLists;

    public override DesignerActionListCollection ActionLists
    {
      get
      {
        if( null == actionLists )
        {
          actionLists = new DesignerActionListCollection();
          actionLists.Add(
						new MultiColumnTreeView.MultiColumnTreeViewActionList( this.Component ) );
        }
        return actionLists;
      }
    }

#endif
  }
}