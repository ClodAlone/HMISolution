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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Design
{
  /// <summary></summary>
	public class TileLayoutDesigner : ControlDesigner
	{
		#region Class members
		private ISelectionService selectionService = null;

		private bool m_bSkipSelectionChanging = false;
		#endregion

		#region Class properties
		/// <summary>
		/// 
		/// </summary>
        public TileLayout Instance
		{
			get
			{
                return this.Control as TileLayout;
			}
		}
		/// <summary></summary>
		public override DesignerVerbCollection Verbs
		{
			get
			{
				DesignerVerbCollection verbs = base.Verbs;

				if( verbs == null )
				{
					verbs = new DesignerVerbCollection();
				}

				verbs.Add( new DesignerVerb( "Tile &Editor", new EventHandler( tile_NodesEditor ) ) );
				verbs.Add( new DesignerVerb( "&Add Node", new EventHandler( tree_AddNode ) ) );
				verbs.Add( new DesignerVerb( "Add &Child Node", new EventHandler( tree_AddChildNode ) ) );

				return verbs;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
        public TileLayoutDesigner() :
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
		private void tile_NodesEditor( object sender, EventArgs e )
		{
			if( this.Instance != null )
			{
				IServiceProvider provider = this.Instance.Site;

                using (LayoutGroupItems form = new LayoutGroupItems())
				{
					if( provider != null )
					{
						IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )this.GetService( typeof( IWindowsFormsEditorService ) );

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
		private void tree_AddChildNode( object sender, EventArgs e )
		{
            //TreeViewAdv tree = this.Instance;

            //if( tree != null )
            //{
            //    TreeNodeAdv node = new TreeNodeAdv( "Node" + tree.NodeCount.ToString() );

            //    if( tree.SelectedNode != null )
            //    {
            //        tree.SelectedNode.Nodes.Add( node );
            //        tree.NodeCount++;
            //        tree.SelectedNode.Expand();
            //    }

            //    tree.Invalidate();
            //}
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tree_AddNode( object sender, EventArgs e )
		{
            //TreeViewAdv tree = this.Instance;

            //if( tree != null )
            //{
            //    TreeNodeAdv node = new TreeNodeAdv( "Node" + tree.NodeCount.ToString() );

            //    if( tree.SelectedNode != null && tree.SelectedNode.Parent != null )
            //    {
            //        tree.SelectedNode.Parent.Nodes.Add( node );
            //    }
            //    else
            //    {
            //        tree.Nodes.Add( node );
            //    }

            //    tree.NodeCount++;
            //    tree.Invalidate();
            //}
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

			foreach( object component in selectionService.GetSelectedComponents() )
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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		private DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if( null == actionLists )
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new TileLayoutActionList( this.Component ) );
				}
				return actionLists;
			}
		}

#endif
	}
}