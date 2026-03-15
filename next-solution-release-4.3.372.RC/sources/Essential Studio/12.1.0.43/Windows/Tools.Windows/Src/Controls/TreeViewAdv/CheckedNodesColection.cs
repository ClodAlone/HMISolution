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
using System.Diagnostics;

using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// This collection contains checked nodes in treeView. Use Add/Remove methods to add/remove nodes.
	/// These methods will check/uncheck proceed nodes automatically.
	/// Use Clear method to delete and uncheck all nodes from collection.
	/// </summary>
	public class CheckedNodesColection:
		ArrayListExt
	{
		private bool m_bIsCleaning = false;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		public CheckedNodesColection()
		{
			this.ForceReadOnly = true;
		}

		public new TreeNodeAdv this[ int index ]
		{
			get
			{
				return ( TreeNodeAdv )base[ index ];
			}
		}

		/// <summary>
		/// Adds the specified node to the collection and checks it.
		/// </summary>
		public override int Add( object value )
		{
			TreeNodeAdv node = value as TreeNodeAdv;
			Debug.Assert( null != node );

			if( null == node || node.Checked ) return -1;

			node.Checked = true;

			return base.Add( node );
		}

		/// <summary>
		/// Removes specified node from collection and unchecks it.
		/// </summary>
		/// <param name="obj"></param>
		public override void Remove( object obj )
		{
			TreeNodeAdv node = obj as TreeNodeAdv;
			Debug.Assert( null != node );
			if( node == null )
				throw new ArgumentNullException( "node" );
      
			node.Checked = false;

			// remove node from list
			base.Remove( node );
		}
		
		/// <summary>
		/// Clears collection.
		/// </summary>
		public override void Clear()
		{
			m_bIsCleaning = true;
			for( int i = 0; i < Count; i++ )
			{
				this[ i ].Checked = false;
				i--;
			}
			m_bIsCleaning = false;
		}

		/// <summary>
		/// Add node to or remove from checked nodes collection and process the same way all node's subtree.
		/// </summary>
		public void ResolveNode( TreeNodeAdv node )
		{
			if( node == null )
				throw new ArgumentNullException( "node" );

            if (node.Checked)
			{
                if( !this.Contains( node ) )
                {
                    this.Add( node );
                }
			}
			else
			{
                this.Remove( node );
            }
		}

		/// <summary>
		/// Removes checked nodes in the specified collection from the list of checked nodes.
		/// </summary>
		public void RemoveNodes( TreeNodeAdvCollection nodes )
		{
			if( nodes == null )
				throw new ArgumentNullException( "nodes" );
      
			if( nodes.Count == 0 ) return;

			for( int i = 0, len = nodes.Count ; i < len; i++ )
			{
				Remove( nodes[ i ] );
			}
		}

		/// <summary>
		/// Adds checked nodes from the collection to the list of checked nodes.
		/// </summary>
		public void AddNodes( TreeNodeAdvCollection nodes )
		{
			if( nodes == null )
				throw new ArgumentNullException( "nodes" );

			if( nodes.Count == 0 ) return;

			for( int i = 0, len = nodes.Count ; i < len; i++ )
			{
				Add( nodes[ i ] );
			}
		}

		/// <summary>
		/// Adds the specified node and all it's checked subnodes to the collection.
		/// </summary>
		/// <param name="node"></param>
		internal int Add( TreeNodeAdv node )
		{
			if( node == null )
				throw new ArgumentNullException( "node" );
      
			if( node.Checked )
			{
				return base.Add( node );
			}

			return  -1;
		}

		/// <summary>
		/// Removes specified node and all it's subnodes from collection.
		/// </summary>
		/// <param name="obj"></param>
		internal void Remove( TreeNodeAdv node )
		{
            if (node != null)
            {
                if (m_bIsCleaning) node.Checked = false;

                // remove node from list
                int idx = base.IndexOf(node);
                
                if (idx >= 0)
                {
                    base.RemoveAt(idx);
                }
            }
            else throw new ArgumentNullException("node");
        }
	}
}
