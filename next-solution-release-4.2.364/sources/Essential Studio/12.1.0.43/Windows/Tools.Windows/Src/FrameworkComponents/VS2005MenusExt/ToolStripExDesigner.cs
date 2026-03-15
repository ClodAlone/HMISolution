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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region Delegates
	delegate Adorner GetAdorner();
	delegate Graphics GetGraphics();
	delegate Control GetControl();
	#endregion

	#region DesignerUtils
	class DesignerUtils
	{
		static DesignerUtils()
		{
			m_tDesignerToolStripControlHost = Type.GetType( "System.Windows.Forms.Design.DesignerToolStripControlHost, System.Design" );
			m_tToolStripItemDesigner = Type.GetType( "System.Windows.Forms.Design.ToolStripItemDesigner, System.Design" );
			m_tToolStripItemBehavior = Type.GetType( "System.Windows.Forms.Design.ToolStripItemBehavior, System.Design" );
			m_tToolStripItemGlyph = Type.GetType( "System.Windows.Forms.Design.ToolStripItemGlyph, System.Design" );

			if( m_tToolStripItemDesigner != null )
			{
				m_fiBodyGlyph = m_tToolStripItemDesigner.GetField( "bodyGlyph", BindingFlags.Instance | BindingFlags.NonPublic );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static BehaviorService GetBehaviorService( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				return item.Site.GetService( typeof( BehaviorService ) ) as BehaviorService;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ISelectionService GetSelectionService( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				return item.Site.GetService( typeof( ISelectionService ) ) as ISelectionService;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static DesignerActionService GetDesignerActionService( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				return item.Site.GetService( typeof( DesignerActionService ) ) as DesignerActionService;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IMenuCommandService GetMenuCommandService( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				return item.Site.GetService( typeof( IMenuCommandService ) ) as IMenuCommandService;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IComponentChangeService GetComponentChangeService( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				return item.Site.GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IDesigner GetDesigner( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				IDesignerHost host = item.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					return host.GetDesigner( item );
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IRootDesigner GetRootDesigner( IComponent item )
		{
			if( item != null && item.Site != null )
			{
				IDesignerHost host = item.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					return host.GetDesigner( host.RootComponent ) as IRootDesigner;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Control GetRootControl( IComponent item )
		{
			IRootDesigner rootDesigner = DesignerUtils.GetRootDesigner( item );
			if( rootDesigner != null )
			{
				return rootDesigner.Component as Control;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dropDown"></param>
		public static void UpdateDropDownParent( ToolStripDropDown dropDown )
		{
			Control root = DesignerUtils.GetRootControl( dropDown );
			if( root != null )
			{
				if( dropDown.TopLevel )
				{
					dropDown.TopLevel = false;
					dropDown.Parent = root.Parent;
				}
			}
		}

		public static void GetAdjustedBounds( ToolStripItem item, ref Rectangle rc )
		{
			if( !( item is ToolStripControlHost ) || !item.IsOnDropDown )
			{
				if( ( item is ToolStripMenuItem ) && item.IsOnDropDown )
				{
					rc.Inflate( -3, -2 );
					rc.Width++;
				}
				else if( ( item is ToolStripControlHost ) && !item.IsOnDropDown )
				{
					rc.Inflate( 0, -2 );
				}
				else if( ( item is ToolStripMenuItem ) && !item.IsOnDropDown )
				{
					rc.Inflate( -3, -3 );
				}
				else
				{
					rc.Inflate( -1, -1 );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ComponentGlyph GetToolStripItemGlyph( ToolStripItem item )
		{
			ComponentGlyph glyph = null;

			if( item != null && item.Site != null && m_fiBodyGlyph != null )
			{
				IDesignerHost host = item.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					if( !m_tDesignerToolStripControlHost.IsAssignableFrom( item.GetType() ) )
					{
						IDesigner designer = host.GetDesigner( item );
						if( designer != null )
						{
							Behavior behavior = Activator.CreateInstance( m_tToolStripItemBehavior, new object[] { } ) as Behavior;
							object[] args = new object[] { item, designer, Rectangle.Empty, behavior };

							ComponentGlyph baseGlyph = Activator.CreateInstance( m_tToolStripItemGlyph, args ) as ComponentGlyph;
							if( baseGlyph != null )
							{
								glyph = new ToolStripExItemGlyph( baseGlyph );
								m_fiBodyGlyph.SetValue( designer, glyph );
							}
						}
					}
				}
			}
			return glyph;
		}

		public static bool IsDropDownItem( ToolStripDropDown dropDown, ToolStripItem item )
		{
			if( item != null )
			{
				ToolStrip ts = GetParentToolStrip( item );
				while( ts != null )
				{
					if( ts != dropDown )
					{
						if( ts is ToolStripDropDown )
						{
							ToolStripItem ownerItem = ( (ToolStripDropDown)ts ).OwnerItem;
							if( ownerItem != null )
							{
								ts = GetParentToolStrip( ownerItem );
							}
							else ts = null;
						}
						else ts = ts.Parent as ToolStrip;
					}
					else return true;
				}
			}
			return false;
		}

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private static ToolStrip GetParentToolStrip( ToolStripItem item )
		{
			ToolStrip ts = null;

			if( item != null )
			{
				ts = item.GetCurrentParent();
				if( ts == null )
				{
					ts = item.Owner;
				}
			}

			return ts;
		}
		#endregion

		#region Fields
		static Type m_tDesignerToolStripControlHost;
		static Type m_tToolStripItemDesigner;
		static Type m_tToolStripItemBehavior;
		static Type m_tToolStripItemGlyph;
		static FieldInfo m_fiBodyGlyph;
		#endregion
	}
	#endregion

	#region ToolStripDesignerProxy
	class ToolStripDesignerProxy
	{
		#region Constructors
		static ToolStripDesignerProxy()
		{
			m_baseType = Type.GetType( "System.Windows.Forms.Design.ToolStripDesigner, System.Design" );
			if( m_baseType != null )
			{
				m_fiDragItem = m_baseType.GetField( "dragItem", BindingFlags.Static | BindingFlags.NonPublic );
			}
		}
		#endregion

		#region Properties
		public static ToolStripItem DragItem
		{
			get
			{
				if( m_fiDragItem!=null )
				{
					return m_fiDragItem.GetValue( null ) as ToolStripItem;
				}
				return null;
			}
			set
			{
				if( m_fiDragItem!=null )
				{
					m_fiDragItem.SetValue( null, value );
				}
			}
		}
		#endregion

		#region Fields
		static Type m_baseType = null;
		static FieldInfo m_fiDragItem = null;
		#endregion
	}
	#endregion

	#region ToolStripExActionList
	/// <summary>
	/// 
	/// </summary>
	class ToolStripExActionList: DesignerActionList
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		public ToolStripExActionList( IComponent component )
			: base( component )
		{
			m_toolStrip = component as ToolStripEx;
			m_daUIService = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override DesignerActionItemCollection GetSortedActionItems()
		{
			DesignerActionItemCollection result = new DesignerActionItemCollection();

			if( m_toolStrip != null )
			{
				InheritanceAttribute inheritance = (InheritanceAttribute)TypeDescriptor.GetAttributes( m_toolStrip )[typeof( InheritanceAttribute )];
				if( ( inheritance != null ) && ( inheritance.InheritanceLevel == InheritanceLevel.NotInherited ) )
				{
					result.Add( new DesignerActionPropertyItem( "Office12Mode", "Office 12 Mode", "Appearance", "Forces usage of Office12 style renderer" ) );
				}
			}

			return result;
		}

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public bool Office12Mode
		{
			get
			{
				return m_toolStrip != null && m_toolStrip.Office12Mode;
			}
			set
			{
				if( m_toolStrip != null && m_toolStrip.Office12Mode != value )
				{
					PropertyDescriptor pdesc = TypeDescriptor.GetProperties( m_toolStrip )["Office12Mode"];
					if( pdesc != null )
					{
						pdesc.SetValue( m_toolStrip, value );

						if( this.DesignerActionUIService != null )
						{
							this.DesignerActionUIService.Refresh( m_toolStrip );
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private DesignerActionUIService DesignerActionUIService
		{
			get
			{
				if( m_daUIService == null )
				{
					m_daUIService = m_toolStrip.Site.GetService( typeof( DesignerActionUIService ) ) as DesignerActionUIService;
				}
				return m_daUIService;
			}
		}
		#endregion

		private DesignerActionUIService m_daUIService;
		private ToolStripEx m_toolStrip;
	}
	#endregion

	#region ToolStripExItemBehavior
	class ToolStripExItemBehavior: Behavior
	{
		#region Constructors
		static ToolStripExItemBehavior()
		{
			m_tToolStripItemDataObject = Type.GetType( "System.Windows.Forms.Design.ToolStripItemDataObject, System.Design" );
		}
		#endregion

		#region Overrides

		#region Mouse suppport
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public override bool OnMouseEnter( Glyph g )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;

			if( glyph != null )
			{
				glyph.UpdateBorder( true );
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public override bool OnMouseLeave( Glyph g )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				glyph.UpdateBorder( false );
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="button"></param>
		/// <param name="mouseLoc"></param>
		/// <returns></returns>
		public override bool OnMouseMove( Glyph g, MouseButtons button, Point mouseLoc )
		{
			bool bResult = false;

			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				glyph.UpdateBorder( true );

				if( IsSelected( glyph.Item ) )
				{
					ComponentGlyph baseGlyph = glyph.BaseGlyph; ;
					if( baseGlyph != null && baseGlyph.Behavior != null )
					{
						bResult = baseGlyph.Behavior.OnMouseMove( baseGlyph, button, mouseLoc );
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="button"></param>
		/// <param name="mouseLoc"></param>
		/// <returns></returns>
		public override bool OnMouseDown( Glyph g, MouseButtons button, Point mouseLoc )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				ComponentGlyph baseGlyph = glyph.BaseGlyph;
				if( baseGlyph != null && baseGlyph.Behavior != null )
				{
					baseGlyph.Behavior.OnMouseDown( baseGlyph, button, mouseLoc );
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="button"></param>
		/// <returns></returns>
		public override bool OnMouseUp( Glyph g, MouseButtons button )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				ComponentGlyph baseGlyph = glyph.BaseGlyph;
				if( baseGlyph != null && baseGlyph.Behavior != null )
				{
					baseGlyph.Behavior.OnMouseUp( baseGlyph, button );
				}
			}
			return false;
		}
		#endregion

		#region Drag&Drop suppport
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="e"></param>
		public override void OnDragEnter( Glyph g, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			if( m_tToolStripItemDataObject != null && m_tToolStripItemDataObject.IsInstanceOfType( e.Data ) )
			{
				ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
				if( glyph != null && glyph.Item != null )
				{
					ToolStripItem item = glyph.RelatedComponent as ToolStripItem;
					if( item != null )
					{
						ToolStripDesignerProxy.DragItem = item;

						glyph.UpdateBorder( true );
						e.Effect = ( Control.ModifierKeys == Keys.Control ) ? DragDropEffects.Copy : DragDropEffects.Move;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="e"></param>
		public override void OnDragLeave( Glyph g, EventArgs e )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				glyph.UpdateBorder( false );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="e"></param>
		public override void OnDragOver( Glyph g, DragEventArgs e )
		{
			e.Effect = DragDropEffects.None;

			if( m_tToolStripItemDataObject != null && m_tToolStripItemDataObject.IsInstanceOfType( e.Data ) )
			{
				ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
				if( glyph != null )
				{
					glyph.UpdateBorder( true );
					e.Effect = ( Control.ModifierKeys == Keys.Control ) ? DragDropEffects.Copy : DragDropEffects.Move;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="e"></param>
		public override void OnDragDrop( Glyph g, DragEventArgs e )
		{
			ToolStripExItemGlyph glyph = g as ToolStripExItemGlyph;
			if( glyph != null )
			{
				ComponentGlyph baseGlyph = glyph.BaseGlyph; ;
				if( baseGlyph != null && baseGlyph.Behavior != null )
				{
					baseGlyph.Behavior.OnDragDrop( baseGlyph, e );
				}
			}
		}
		#endregion

		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool IsSelected( ToolStripItem item )
		{
			ISelectionService selSvc = DesignerUtils.GetSelectionService( item );
			return selSvc != null && selSvc.GetComponentSelected( item );
		}
		#endregion

		#region Fields
		static Type m_tToolStripItemDataObject = null;
		#endregion

	}
	#endregion

	#region ToolStripExItemGlyph
	class ToolStripExItemGlyph: ControlBodyGlyph
	{
		#region Constructors

		public ToolStripExItemGlyph( ComponentGlyph baseGlyph ) :
			this( baseGlyph, new ToolStripExItemBehavior() )
		{
		}

		protected ToolStripExItemGlyph( ComponentGlyph baseGlyph, Behavior behavior ) :
			base( Rectangle.Empty, Cursors.Default, baseGlyph.RelatedComponent, behavior )
		{
			m_behaviorSvc = DesignerUtils.GetBehaviorService( baseGlyph.RelatedComponent );
			m_selectionSvc = DesignerUtils.GetSelectionService( baseGlyph.RelatedComponent );

			m_baseGlyph = baseGlyph;
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bHighlight"></param>
		internal void UpdateBorder( bool bHighlight )
		{
			if( !this.Selected )
			{
				if( bHighlight )
				{
					PaintHighlightBorder();
				}
				else
				{
					InvalidateBorder();
				}
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnInsert()
		{
			if( ( m_bSelected = this.Selected ) )
			{
				InvalidateBorder();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRemove()
		{
			if( m_bSelected && !this.Selected )
			{
				InvalidateBorder();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pe"></param>
		public override void Paint( PaintEventArgs pe )
		{
			Rectangle rcPaint = GetPaintingBounds();
			if( !rcPaint.IsEmpty && rcPaint.IntersectsWith( pe.ClipRectangle ) )
			{
				if( this.Item is ToolStripComboBox && this.Item.IsOnDropDown )
				{
					this.Item.Invalidate();
				}
				if( this.Selected )
				{
					pe.Graphics.DrawRectangle( SystemPens.ControlText, rcPaint );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override Cursor GetHitTest( Point p )
		{
			if( this.Item != null && this.Item.Visible )
			{
				if( this.Bounds.Contains( p ) )
				{
					return Cursors.Default;
				}
			}
			return null;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected Rectangle GetPaintingBounds()
		{
			Rectangle result = this.Bounds;

			if( !result.IsEmpty )
			{
				DesignerUtils.GetAdjustedBounds( this.Item, ref result );
				result.Inflate( 1, 1 );
				result.Width--;
				result.Height--;
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void InvalidateBorder()
		{
			if( m_behaviorSvc != null )
			{
				Rectangle rc = GetPaintingBounds();

				if( !rc.IsEmpty )
				{
					rc.Width += 1;
					rc.Height += 1;
					using( Region region = new Region( rc ) )
					{
						rc.Inflate( -1, -1 );
						region.Exclude( rc );
						m_behaviorSvc.Invalidate( region );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void PaintHighlightBorder()
		{
			if( m_behaviorSvc != null )
			{
				Rectangle rc = GetPaintingBounds();
				if( !rc.IsEmpty )
				{
					Graphics g = m_behaviorSvc.AdornerWindowGraphics;
					if( g != null )
					{
						using( Pen pen = new Pen( new SolidBrush( Color.Black ) ) )
						{
							pen.DashStyle = DashStyle.Dot;
							g.DrawRectangle( pen, rc );
						}
						g.Dispose();
					}
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public ComponentGlyph BaseGlyph
		{
			get
			{
				return m_baseGlyph;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public ToolStripItem Item
		{
			get { return this.RelatedComponent as ToolStripItem; }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Rectangle Bounds
		{
			get
			{
				Rectangle rcBounds = Rectangle.Empty;
				if( m_behaviorSvc != null )
				{
					ToolStripItem item = this.Item;
					if( item != null && item.Visible )
					{
						ToolStrip parent = item.GetCurrentParent();
						if( parent != null && parent.IsHandleCreated )
						{
							rcBounds = item.Bounds;
							rcBounds.Location = m_behaviorSvc.MapAdornerWindowPoint( parent.Handle, rcBounds.Location );
						}
					}
				}
				return rcBounds;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool Selected
		{
			get
			{
				return m_selectionSvc != null && m_selectionSvc.GetComponentSelected( this.RelatedComponent );
			}
		}
		#endregion

		#region Fields
		protected bool m_bSelected = false;

		protected ComponentGlyph m_baseGlyph = null;
		protected BehaviorService m_behaviorSvc = null;
		protected ISelectionService m_selectionSvc = null;
		#endregion
	}
	#endregion

	#region ToolStripExGlyphCollection
	public class ToolStripExGlyphCollection: GlyphCollection, IList
	{
		#region Constructors/Destructors
		/// <summary>
		/// 
		/// </summary>
		static ToolStripExGlyphCollection()
		{
			m_tMiniLockedBorderGlyph = Type.GetType( "System.Windows.Forms.Design.Behavior.MiniLockedBorderGlyph, System.Design" );
			m_tToolStripItemGlyph = Type.GetType( "System.Windows.Forms.Design.ToolStripItemGlyph, System.Design" );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		protected override void OnClear()
		{
			for( int i = 0, count = InnerList.Count; i < count; i++ )
			{
				ToolStripExItemGlyph glyph = InnerList[i] as ToolStripExItemGlyph;
				if( glyph != null )
				{
					glyph.OnRemove();
				}
			}
			base.OnClear();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete( int index, object value )
		{
			if( value is ToolStripExItemGlyph )
			{
				( value as ToolStripExItemGlyph ).OnInsert();
			}
			base.OnInsertComplete( index, value );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemoveComplete( int index, object value )
		{
			if( value is ToolStripExItemGlyph )
			{
				( value as ToolStripExItemGlyph ).OnRemove();
			}
			base.OnRemoveComplete( index, value );
		}
		#endregion

		#region IList implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.Add( object value )
		{
			int rc = -1;

			object obj = GetValidObject( value );
			if( obj!=null )
			{
				rc = this.InnerList.Add( obj );
				OnInsertComplete( rc, obj );
			}
			return rc;
		}
		/// <summary>
		/// Inserts an item to the System.Collections.IList at the specified index
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The System.Object to insert into the System.Collections.IList</param>
		void IList.Insert( int index, object value )
		{
			if( index >= 0 && index <=this.InnerList.Count )
			{
				object obj = GetValidObject( value );

				if( obj != null )
				{
					this.InnerList.Insert( index, obj );
					OnInsertComplete( index, obj );
				}
			}
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the System.Collections.IList
		/// </summary>
		/// <param name="value">The System.Object to remove from the System.Collections.IList</param>
		void IList.Remove( object value )
		{
			OnValidate( value );

			int itemToRemove = GetIndexOf( value );

			if( itemToRemove >= 0 )
			{
				object obj = this.InnerList[itemToRemove];

				this.InnerList.RemoveAt( itemToRemove );
				OnRemoveComplete( itemToRemove, obj );
			}
		}
		/// <summary>
		/// Determines the index of a specific item in the System.Collections.IList
		/// </summary>
		/// <param name="value">The System.Object to locate in the System.Collections.IList</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		int IList.IndexOf( object value )
		{
			return GetIndexOf( value );
		}
		/// <summary>
		/// Determines whether the System.Collections.IList contains a specific value
		/// </summary>
		/// <param name="value">The System.Object to locate in the System.Collections.IList</param>
		/// <returns>true if the System.Object is found in the System.Collections.IList; otherwise, false</returns>
		bool IList.Contains( object value )
		{
			for( int i = 0,count=InnerList.Count; i < count; i++ )
			{
				object obj = InnerList[i];

				if( obj == value || GetBaseGlyph( obj ) == value )
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		object GetValidObject( object value )
		{
			if( !( value is ToolStripExItemGlyph ) )
			{
				if( IsPanelItemGlyph( value as ComponentGlyph ) )
				{
					return new ToolStripPanelItemGlyph( value as ComponentGlyph, this );
				}
				if( m_tToolStripItemGlyph != null && m_tToolStripItemGlyph.IsInstanceOfType( value ) )
				{
					return new ToolStripExItemGlyph( value as ComponentGlyph );
				}
				if( m_tMiniLockedBorderGlyph != null && m_tMiniLockedBorderGlyph.IsInstanceOfType( value ) )
				{
					return null;
				}
			}
			return value;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		ComponentGlyph GetBaseGlyph( object value )
		{
			ToolStripExItemGlyph glyph = value as ToolStripExItemGlyph;
			return glyph != null ? glyph.BaseGlyph : null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int GetIndexOf( object value )
		{
			int rc = this.InnerList.IndexOf( value );

			if( rc < 0 && m_tToolStripItemGlyph.IsInstanceOfType( value ) )
			{
				for( int i = 0, count = this.InnerList.Count; i < count; i++ )
				{
					if( GetBaseGlyph( this.InnerList[i] ) == value )
					{
						rc = i;
						break;
					}
				}
			}
			return rc;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IsPanelItemGlyph( object value )
		{
			bool bResult = false;

			if( value is ComponentGlyph )
			{
				ToolStripControlHost item = ( (ComponentGlyph)value ).RelatedComponent as ToolStripControlHost;
				if( item != null )
				{
					bResult = item.Control is ToolStrip;
				}
			}
			return bResult;
		}
		#endregion

		#region Fields
		static Type m_tMiniLockedBorderGlyph = null;
		static Type m_tToolStripItemGlyph = null;
		#endregion
	}
	#endregion

	#region ToolStripExDesigner
	public class ToolStripExDesigner: IDesigner
	{
		#region Constructors
		static ToolStripExDesigner()
		{
			m_tToolStripDesigner = Type.GetType( "System.Windows.Forms.Design.ToolStripDesigner, System.Design" );
		}
		#endregion

		#region IDesigner implementation
		/// <summary>
		/// 
		/// </summary>
		IComponent IDesigner.Component
		{
			get { return m_component; }
		}
		/// <summary>
		/// 
		/// </summary>
		DesignerVerbCollection IDesigner.Verbs
		{
			get { return null; }
		}
		/// <summary>
		/// 
		/// </summary>
		void IDesigner.DoDefaultAction()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		void IDesigner.Initialize( IComponent component )
		{
			m_component = component;

			UpdateActionService();
			ToolStripPanelItemDesigner.UpdateSerializationService( component );

			if( component != null )
			{
				ToolStripExService tsSvc = ToolStripExService.Get( component.Site );
				if( tsSvc != null && tsSvc.Designers != null )
				{
                    if(m_tToolStripDesigner == null)
                        m_tToolStripDesigner = Type.GetType("System.Windows.Forms.Design.ToolStripDesigner, System.Design",true);
                    
					IDesigner designer = Activator.CreateInstance( m_tToolStripDesigner ) as IDesigner;
					if( designer != null )
					{
						tsSvc.Designers[component] = designer;
						designer.Initialize( component );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void IDisposable.Dispose()
		{
			m_component = null;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		void UpdateActionService()
		{
			DesignerActionService svc = DesignerUtils.GetDesignerActionService( m_component );
			if( null != svc )
			{
				svc.Add( m_component, new ToolStripExActionList( m_component ) );
			}
		}
		#endregion

		#region Fields
		protected IComponent m_component = null;

		static Type m_tToolStripDesigner = null;
		#endregion
	}
	#endregion
}

#endif