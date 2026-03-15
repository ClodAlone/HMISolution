#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	[Serializable]
	internal class DockRelation
		: ISerializable
	{
		protected ArrayList m_relatedControllers;
		protected DockInfo m_relation = null;

		public ArrayList RelatedControllers
		{
			get
			{
				return m_relatedControllers;
			}
			set
			{
				if( m_relatedControllers != value )
				{
					m_relatedControllers = value;
				}
			}
		}

		public DockInfo Relation
		{
			get
			{
				return m_relation;
			}
			set
			{
				if( m_relation != value )
				{
					m_relation = value;
				}
			}
		}

		public DockRelation()
		{
			m_relatedControllers = new ArrayList();
		}

		public DockRelation( ArrayList controllers, DockInfo relation )
		{
			RelatedControllers = controllers;
			Relation = relation;
		}

		#region ISerializable Members

		protected DockRelation( SerializationInfo info, StreamingContext context )			
		{
			m_relation = ( DockInfo )info.GetValue("DockRelation", typeof(DockInfo));
		}

		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue("DockRelation", m_relation);
		}

		#endregion
	}

	public class DockStateControllerWrapper
		: DockStateControllerBase
		, IResizable
	{
		private DockStateControllerBase m_ctrlRef = null;
		private Rectangle m_layoutRect = Rectangle.Empty;
		protected DockInfo m_dockRelation = null;
		protected ArrayList m_dockRelControllers = new ArrayList();
		protected Hashtable m_relations = new Hashtable();
		protected Minimization m_minimized = Minimization.None;
		protected Size m_controlSize = Size.Empty;

		internal Size ControlSize
		{
			get
			{
				return m_controlSize;
			}
			set 
			{
				if( m_controlSize != value )
				{
					m_controlSize = value;
				}
			}
		}

		public DockStateControllerBase InternalControl
		{
			get
			{
				return m_ctrlRef;
			}
			set
			{
				if( m_ctrlRef != value)
				{
					m_ctrlRef = value;
					m_minimized = m_ctrlRef.Minimized;
				}
			}
		}


		public Hashtable Relations
		{
			get
			{
				return m_relations;
			}
			set
			{
				if( m_relations != value )
				{
					m_relations = value;
				}
			}
		}

		public override DockControllerBase RedockController( DockInfo di, bool bforcenew )
		{
			return null;
		}

		internal override Minimization Minimized
		{
			get
			{
				return m_minimized;
			}
			set
			{
				m_minimized = value;
			}
		}

		internal bool Valid
		{
			get
			{
				bool valid = false;

				if( this.ParentController != null 
					&& this.ParentController.ChildControllers.Contains(this) )
					valid = true;

				return valid;
			}
		}

		internal virtual ArrayList DockRelationControllers
		{
			get
			{
				return m_dockRelControllers;
			}
			set
			{
				if( m_dockRelControllers != value )
				{
					m_dockRelControllers = value;
				}
			}
		}

		public override bool Floating
		{
			get
			{
				bool floating = false;
				if( this.dcParent != null )
					floating = this.dcParent.Floating;

				return floating;
			}
			set
			{}
		}

		public override Rectangle LayoutRect
		{
			get
			{
				return this.m_layoutRect;
			}
			set
			{}
		}

		public override Control HostControl
		{
			get 
			{
				return null;
			}
		}

		public override bool AutoHideMode
		{
			get
			{
				return false;
			}
			set
			{}
		}

		public override void CloseController()
		{}

		public DockStateControllerWrapper( DockingManager manager, DockStateControllerBase intControl )
			: base( manager )
		{			
			this.InternalControl = intControl;

			if( intControl != null )
			{
				m_controlSize = intControl.LayoutRect.Size;
				m_minimized = intControl.Minimized;

				if( intControl.DICurrent.DP == DockPreference.Vertical )
					this.m_layoutRect.Width = intControl.DICurrent.rcDockArea.Width;
				else
					this.m_layoutRect.Height = intControl.DICurrent.rcDockArea.Height;
			}
		}

		protected internal override DockControllerBase QueryController( string uniqueName )
		{
			return null;
		}

		internal void RefreshSize()
		{
			SizingController sc = this.ParentController as SizingController;
			if( sc != null )
			{
				if( sc.DockingOrder == DockPreference.Vertical )
					this.m_layoutRect.Size = new Size( sc.LayoutRect.Width, 0 );
				else
					this.m_layoutRect.Size = new Size( 0, sc.LayoutRect.Height );
			}
		}

		public override void Refresh()
		{}

		internal override void AddWrapper( ControllerWrapper cw )
		{
			DockStateWrapper dsw = new DockStateWrapper();
			Hashtable tempRelations = new Hashtable();
			ArrayList intControllers = new ArrayList();

			if( this.InternalControl is DockHostController 
				&& m_dockRelControllers.Count < 2 )
				intControllers.Add(( this.InternalControl as DockHostController ).UniqueName);
			else
			{
				foreach( DockHostController dhc in this.m_dockRelControllers )
				{
					if( dhc != null )
					{
						string key = dhc.UniqueName;
						intControllers.Add(key);

						if( m_relations.ContainsKey(dhc.UniqueName) )
							tempRelations.Add( dhc.UniqueName, m_relations[dhc.UniqueName] );
					}
				}
			}

			dsw.InternalController = intControllers;
			dsw.Relations = tempRelations;
			dsw.StoredLayoutSize = m_controlSize;

			cw.Children.Add(dsw);
		}

		internal InternalDockStateWrapper GetDockStateWrapper()
		{
			InternalDockStateWrapper dsw = new InternalDockStateWrapper();
	
			ArrayList intControllers = new ArrayList();
	
			if( this.InternalControl is DockHostController )
				intControllers.Add(( this.InternalControl as DockHostController ).UniqueName);
			else
			{
				foreach( string key in m_relations.Keys )
				{
					intControllers.Add(key);
				}
			}
	
			dsw.InternalController = intControllers;
			dsw.Relations = m_relations;
			return dsw;
		}

		internal override void StoreControllers( ArrayList controllers )
		{
			base.StoreControllers(controllers);

			this.ParentController.RemoveChild(this);		
		}

		public override bool IsTargetController( Point ptscreen )
		{
			return false;
		}
		
		public override void UpdateControl()
		{}

		public override void DockAsMDIChild()
		{}

		internal override bool IsFloatOnly()
		{
			return false;
		}

		public override void GetDockInfo( Control ctrl, Point pt, DockInfo di )
		{}

		public override void EnterAutoHideMode()
		{}

		public override void ExitAutoHideMode( bool bcloseonexit )
		{}

		internal override void SetAutohiddenControlSize( Size size )
		{}

		#region IResizable Members

		public Size CalculateSize( Size parentSize, Size newParentSize )
		{
			return Size.Empty;
		}

		public bool IsVerticallyResizable()
		{
			return false;
		}

		public bool IsHorizontallyResizable()
		{
			return false;
		}

		internal override bool FreezeResize
		{
			get
			{
				return false;
			}
			set
			{}
		}

		internal override Direction MustResize
		{
			get{ return Direction.Both; }
		}
		#endregion
	}
}
