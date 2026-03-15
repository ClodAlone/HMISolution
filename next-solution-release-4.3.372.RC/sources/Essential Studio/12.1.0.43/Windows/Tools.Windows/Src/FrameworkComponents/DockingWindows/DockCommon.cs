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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the type of docking.
	/// </summary>
	/// <remarks>
	/// The DockingStyle enumeration is used by the <see cref="DockingManager"/> to convey
	/// and to get information about a dock operation. The DockingStyle value is always
	/// expressed relative to the dock target. For example, when a control is being
	/// docked to the main form and DockingStyle.Left is used, this implies that the
	/// control will be docked to the left border of the form and have a width equal to the specified
	/// width and a height equal to the form's client rectangle height.
	/// </remarks>
	public enum DockingStyle
	{
		/// <summary>
		/// The control is docked to the left edge of the parent control.
		/// </summary>
		Left = 0,
		/// <summary>
		/// The control is docked to the right edge of the parent control.
		/// </summary>
		Right,
		/// <summary>
		/// The control is docked to the top edge of the parent control.
		/// </summary>
		Top,
		/// <summary>
		/// The control is docked to the bottom edge of the parent control.
		/// </summary>
		Bottom,
		/// <summary>
		/// The control is docked as a tabbed window along with the dock target. This style
		/// is not applicable when the dock target is the host form.
		/// </summary>
		Tabbed,
		/// <summary>
		/// For internal use.
		/// </summary>
		Fill,
		/// <summary>
		/// No dock border.
		/// </summary>
		None
	}

	/// <summary>
	/// Specifies transit operations.
	/// </summary>
	/// <remarks>
	/// This enum is used by ApplyDockInfo method.
	/// </remarks> 
	internal enum TransitType
	{
		FloatToDock = 1,
		DockToFloat = 2,
		FloatToFloat =3
	}

    [Serializable]
	internal struct RelationNamePair
	{
		public string Name;
		public float Relation;
		
		public RelationNamePair( string name, float relation )
		{
			Name = name;
			Relation = relation;
		}
	}

	/// <summary>
	/// Specifies where user can dock in some control using drag providers.
	/// </summary>
	/// <remarks>
	/// This enum is used by DockAbility and OuterDockAbility extended properties in DockingManager.
	/// </remarks>
	[Flags]
	public enum DockAbility
	{
		/// <summary>
		/// No docking aviable in current control.
		/// </summary>
		None = 0x0000,
		/// <summary>
		/// The user can dock to the left side of control.
		/// </summary>
		Left = 0x0001,
		/// <summary>
		/// The user can dock to top of control.
		/// </summary>
		Top = 0x0002,
		/// <summary>
		/// The user can dock to the right side of control.
		/// </summary>
		Right = 0x0004,
		/// <summary>
		/// The user can dock to bottom of control.
		/// </summary>
		Bottom = 0x0008,
		/// <summary>
		/// The user can dock another control in tab group.
		/// </summary>
		Tabbed = 0x0010,
		/// <summary>
		/// Left, Right and Tabbed docking enabled.
		/// </summary>
		Horizontal = 0x0015,
		/// <summary>
		/// Top, Bottom and Tabbed docking enabled.
		/// </summary>
		Vertical = 0x001A,
		/// <summary>
		/// All kinds of docking enabled.
		/// </summary>
		All = 0x001F
	}
	/// <summary>
	/// Defines control states according to its size.
	/// </summary>
	public enum ControlSizeStates
	{ 
		/// <summary>
		/// Control is maximized.
		/// </summary>
		Maximized,
		/// <summary>
		/// Control is maximizing.
		/// </summary>
		Maximize,
		/// <summary>
		/// Control is restoring from minimized/maximized states.
		/// </summary>
		Restore,
		/// <summary>
		/// Control is minimizing.
		/// </summary>
		Minimize
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum MouseAction
	{
		LBtnDown = 1,
		LBtnUp = 2,
		RBtnDown,
		RBtnUp,
		LBtnDblClk,
		MouseMove,
		MouseLeave
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public enum DockPreference
	{
		All = 0,
		Horizontal = 1,
		Vertical,
		Tabbed,
		None
	}


	// The DockInfo class represents the state of a dockhost at any given instant. DockHosts' have a past, current and
	// new DockInfo values.
 	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class DockInfo : ISerializable
	{
		[NonSerialized]
		public DockControllerBase dController;
		public DockingStyle dStyle;
		public int nPriority;
		public int nDockIndex;
		public DockPreference DP;
		public Rectangle rcDockArea;
		public Rectangle rcControlArea;

		private string m_controllerName;

		public string ControlleName
		{
			get
			{
				return m_controllerName;
			}
			set
			{
				if( m_controllerName != value )
				{
					m_controllerName = value;
				}
			}
		}

		public static DockInfo NullInfo
		{
			get { return new DockInfo(null, DockingStyle.Fill, 0, 0, DockPreference.All, Rectangle.Empty); }
		}

		private DockInfo()
		{
		}

		public DockInfo(DockControllerBase dc, DockingStyle ds, int priority, int index, DockPreference dp, Rectangle rc)
		{
			this.dController = dc;
			this.dStyle = ds;
			this.nPriority = priority;
			this.nDockIndex = index;
			this.DP = dp;
			this.rcDockArea = rc;
		}

		public DockInfo(DockInfo di)
		{
			this.dController = di.dController;
			this.dStyle = di.dStyle;
			this.nPriority = di.nPriority;
			this.nDockIndex = di.nDockIndex;
			this.DP = di.DP;
			this.rcDockArea = di.rcDockArea;
		}

		// Private constructor called during the deserialization process
		private DockInfo(SerializationInfo info, StreamingContext context)
		{
			this.dStyle = (DockingStyle)info.GetValue("DockStyle", typeof(DockingStyle));
			this.nPriority = info.GetInt32("Priority");
			this.nDockIndex = info.GetInt32("DockIndex");
			this.DP = (DockPreference)info.GetValue("DockPreference", typeof(DockPreference));
			this.rcDockArea = (Rectangle)info.GetValue("DockArea", typeof(Rectangle));
			Hashtable hash = new Hashtable();
			foreach( SerializationEntry infoData in info )
			{
				hash[infoData.Name] = null;
			}
			if( hash.Contains("Controller") )
				this.m_controllerName = info.GetString("Controller");
		}

		// ISerializable implementation
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if( this.dController is DockHostController )
				info.AddValue("Controller", ( dController as DockHostController ).UniqueName);
			else
				info.AddValue("Controller", String.Empty);

			info.AddValue("DockStyle", this.dStyle, typeof(DockingStyle));
			info.AddValue("Priority", this.nPriority);
			info.AddValue("DockIndex", this.nDockIndex);
			info.AddValue("DockPreference", this.DP, typeof(DockPreference));
			info.AddValue("DockArea", this.rcDockArea, typeof(Rectangle));
		}

		// Used for depersistence only after a version change of the Syncfusion Tools library.
		internal static DockInfo CloneByReflection(Object objthis)
		{
			DockInfo dinfo = new DockInfo();

			FieldInfo[] fields = objthis.GetType().GetFields();
			foreach(FieldInfo finfo in fields)
			{
				switch(finfo.Name)
				{
					case "dStyle":
						dinfo.dStyle = (DockingStyle)finfo.GetValue(objthis);
						break;
					case "nPriority":
						dinfo.nPriority = (int)finfo.GetValue(objthis);
						break;
					case "nDockIndex":
						dinfo.nDockIndex = (int)finfo.GetValue(objthis);
						break;
					case "DP":
						dinfo.DP = (DockPreference)finfo.GetValue(objthis);
						break;
					case "rcDockArea":
						dinfo.rcDockArea = (Rectangle)finfo.GetValue(objthis);
						break;
				}
			}

			return dinfo;
		}
	}



	/// Custom Event argument class used for controller change notifcations. Parent Controller's, while being undocked,
	/// use this event argument to notify all child subscriber controllers of the new dock parent controller information.
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ControllerChangedEventArgs : EventArgs
	{
		protected DockInfo dockInfoNew = null;
		protected DCRelationship dcRS = null;

		public ControllerChangedEventArgs(DockInfo di, DCRelationship dcr)
		{
			this.dockInfoNew =di;
			this.dcRS = dcr;
		}

		public DockInfo NewDockInfo
		{
			get { return this.dockInfoNew; }
		}

		public DCRelationship DCRelation
		{
			get { return this.dcRS; }
		}
	}

	// Delegate used for ControllerChangedEvent notifications
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void ControllerChangedEH(Object obj, ControllerChangedEventArgs arg);


	// Class used for laying down the relationship that exists between controllers.
	// DockHostControllers maintains two stacks, a floating and a docking, of DCRelationship objects.
	// SizingControllers have just a reference to an instance of the current relationship
	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class DCRelationship
	{
		public int nRelation;
		public bool bChild;
		public DockPreference DP;
		public int nIndex;

		private DCRelationship()
		{
		}

		public DCRelationship(int relation, bool child, DockPreference priority, int index)
		{
			this.nRelation = relation;
			this.bChild = child;
			this.DP = priority;
			this.nIndex = index;
		}

		// Used for depersistence only after a version change of the Syncfusion Tools library.
		internal static DCRelationship CloneByReflection(Object objthis)
		{
			DCRelationship dcr = new DCRelationship();

			FieldInfo[] fields = objthis.GetType().GetFields();
			foreach(FieldInfo finfo in fields)
			{
				switch(finfo.Name)
				{
					case "nRelation":
						dcr.nRelation = (int)finfo.GetValue(objthis);
						break;
					case "bChild":
						dcr.bChild = (bool)finfo.GetValue(objthis);
						break;
					case "DP":
						dcr.DP = (DockPreference)finfo.GetValue(objthis);
						break;
					case "nIndex":
						dcr.nIndex = (int)finfo.GetValue(objthis);
						break;
				}
			}

			return dcr;
		}
	}

	internal enum Direction
	{
		/// <summary>
		/// Specifies no particular direction.
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Specifies horizontal direction.
		/// </summary>
		Horizontal = 0x01,
		/// <summary>
		/// Specifies vertical direction.
		/// </summary>
		Vertical = 0x10,
		/// <summary>
		/// Specifies horizontal and vertical directions at a time.
		/// </summary>
		Both = 0x11
	}

	public enum Minimization
	{ 
		/// <summary>
		/// Controller is horizontally minimized.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Controller is vertically minimized.
		/// </summary>
		Vertical,
		/// <summary>
		/// Controller is not minimized.
		/// </summary>
		None
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class ControllerDCRPair
	{
		public DockControllerBase Controller = null;
		public DCRelationship DCR = null;
		public ControllerDCRPair(DockControllerBase dc, DCRelationship dcr)
		{
			this.Controller = dc;
			this.DCR = dcr;
		}
	}

	/// <summary>
	/// DockingManagerException raised when user tries to perform
	/// restricted operation in DockingManager.
	/// </summary>
	[Serializable]
	public class DockingManagerException
	  : ApplicationException
	{
		#region Class constants
		/// <summary>
		/// Default message to show when exception fired.
		/// </summary>
		private const string c_message = @"This operation is invalid in current context.";
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Constructor.
		/// </summary>
		public DockingManagerException()
			: this(c_message)
		{
		}
		/// <summary>
		///constructor.
		/// </summary>
		/// <param name="innerExc">inner exeption.</param>
		public DockingManagerException( Exception innerExc )
			: this(c_message, innerExc)
		{
		}
		/// <summary>
		///constructor.
		/// </summary>
		/// <param name="message">message to show.</param>
		public DockingManagerException( string message )
			: base(message)
		{
		}
		/// <summary>
		///constructor.
		/// </summary>
		/// <param name="message">message to show.</param>
		/// <param name="innerExc">inner exception.</param>
		public DockingManagerException( string message, Exception innerExc )
			: base(message, innerExc)
		{
		}
		/// <summary>
		///constructor.
		/// </summary>
		/// <param name="info">info.</param>
		/// <param name="context">context.</param>
		public DockingManagerException( SerializationInfo info, StreamingContext context )
			: base(info, context)
		{
		}
		#endregion
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class IEnumWrapper : IEnumerator
	{
		private enum Position
		{
			Begin = 0,
			Current,
			End
		}

		private Object eObject = null;
		private Position pos;

		public IEnumWrapper(Object obj)
		{
			this.eObject = obj;
			this.pos = Position.Begin;
		}

		// IEnumerator implementation
		public object Current
		{
			get
			{
				if(this.pos == Position.Current)
					  return this.eObject;
				else
					throw(new InvalidOperationException());
			}
		}

		public bool MoveNext()
		{
			if( (this.eObject != null) && (++this.pos != Position.End) )
				return true;
			else
				return false;
	    }

		public void Reset()
		{
			this.pos = Position.Begin;
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public abstract class DockControllerBase : IDisposable
	{
        protected DockingManager m_PreviousDockingMgr = null;
		protected DockingManager dockingMgr;
		protected DockControllerBase dcParent = null;
		protected internal DockInfo dockInfoCurrent = DockInfo.NullInfo;
		protected int nDockBoundary = 15;
		protected bool bDeleting = false;
		protected DockControllerBase m_childWrapper = null;
		protected Minimization m_bMinimized = Minimization.None;
		protected bool m_bMaximized = false;
		private Direction m_mustResize = Direction.Both;

		protected DockInfo dockInfoTransient = DockInfo.NullInfo;		// Temporary DockInfo cache

		// Controller Changed Event - fired whenever a target controller is changed
		public event ControllerChangedEH ControllerChanged;

		protected Size minSize = new Size(0,0);
		protected bool m_bAllowFloating = true;

		internal virtual bool AllowFloating
		{
			get
			{
				return m_bAllowFloating;
			}
			set
			{
				if( m_bAllowFloating != value )
				{
					m_bAllowFloating = value;
				}
			}
		}

        internal DockingManager PreviousDockingMgr
        {
            get
            {
                return this.m_PreviousDockingMgr;
            }
            set
            {
                if (this.m_PreviousDockingMgr != value)
                    this.m_PreviousDockingMgr = value;
            }
        }

		public DockingManager DockingManager
		{
			get { return this.dockingMgr; }
			set	{ this.dockingMgr = value; }
		}

		internal virtual Minimization Minimized
		{
			get { return m_bMinimized; }
			set { m_bMinimized = value; }
		}

		internal virtual bool Maximized
		{
			get { return m_bMaximized; }
			set { m_bMaximized = value; }
		}

		public abstract Control HostControl
		{
			get;
		}

		public virtual bool Deleting
		{
			get
			{
				return bDeleting;
			}
			set
			{
				if( bDeleting != value )
				{
					bDeleting = value;
				}
			}
		}

		internal virtual DockControllerBase ChildWrapper
		{
			set
			{
				if( m_childWrapper != value )
				{
					m_childWrapper = value;
				}
			}
		}

		internal virtual ArrayList SiblingDCR
		{
			get { return null; }
		}

		public virtual DockControllerBase ParentController
		{
			set { this.dcParent = value; }
			get { return this.dcParent; }
		}

		public virtual DockControllerBase ToplevelController
		{
			get
			{
				return (this.ParentController == null) ? this : this.ParentController.ToplevelController;
			}
		}

		public virtual void Refresh()
		{
			if( this.ParentController != null && !this.dockingMgr.ForbidWrapperLogic )
			{
				DockControllerBase parent = this.ParentController;

				if( !parent.ChildControllers.Contains(this) )
					return;
			
				if( this.ChildCount == 0 )
				{
					parent.RemoveChild(this);
					parent.Refresh();
				}
				else if( this.ChildCount == 1 
					&& !(this.ToplevelController is SizingController)
					&& !(this.ParentController is MainFormController))
				{
					DockControllerBase child = this.ChildControllers[0] as DockControllerBase;
					this.RemoveChild(child);
					parent.ReplaceChild(this, child);

					if( m_childWrapper != null )
						m_childWrapper.ParentController = parent;

					this.ParentController = null;
					parent.Refresh();
				}
			}
		}

		protected internal virtual DockControllerBase QueryController( string uniqueName )
		{
			DockControllerBase baseController = null;

			foreach( DockControllerBase dcb in this.ChildControllers )
			{
				if( dcb != null )
					baseController = dcb.QueryController( uniqueName );

				if( baseController != null )
					break;
			}

			return baseController;
		}

		internal virtual void RemoveDockController()
		{
			if( this.HostControl != null )
				this.ToplevelController.HostControl.Controls.Remove(this.HostControl);
			if( this.ChildControllers != null )
			{
				ArrayList childCtrls = new ArrayList(this.ChildControllers);
				foreach( DockControllerBase child in childCtrls )
				{
					child.RemoveDockController();
				}
			}

			if( this.ParentController != null )
				this.ParentController.RemoveChild(this);
		}

		internal virtual Direction MustResize
		{
			get
			{
				return m_mustResize;
			}
			set
			{
				if( m_mustResize != value )
				{
					m_mustResize = value;
				}
			}
		}

		public abstract Rectangle LayoutRect
		{
			get;
			set;
		}

		public virtual DockInfo DICurrent
		{
			get { return this.dockInfoCurrent; }
			set { this.dockInfoCurrent = value; }
		}

		public DockInfo DITransient
		{
			get { return this.dockInfoTransient; }
			set { this.dockInfoTransient = value; }
		}

		public int DockBoundary
		{
			get { return this.nDockBoundary; }
			set { this.nDockBoundary = value; }
		}

		// Returns true if the controller is hosted within a floating frame. Else false
		public abstract bool Floating
		{
			get;
			set;
		}

		public virtual bool MainFormController
		{
			get
			{
				return ((this.Floating == false)&&(this.ParentController == null)) ? true : false;
			}
		}

		public virtual int ChildCount
		{
			get { return -1; }
		}

		public virtual int ChildHostCount
		{
			get { return -1; }
		}

		public virtual IEnumerator DCR
		{
			get { return null; }
		}

		public virtual DCRelationship DCRCurrent
		{
			get { return null; }
			set { ; }
		}

		public virtual IEnumerator ChildEnumerator
		{
			get { return null; }
		}

		public virtual ArrayList ChildControllers
		{
			get { return null; }
		}

		public virtual IEnumerator ChildHostEnumerator
		{
			get { return null; }
		}

		public virtual Size MinimumSize
		{
			get { return this.minSize; }
			set
			{
				if(this.minSize != value)
					this.minSize = value;
			}
		}

		protected DockControllerBase(DockingManager mgr)
		{
			this.dockingMgr = mgr;
		}

		~DockControllerBase()
		{
			this.Dispose(false);
		}

		public virtual void AddToDCR(DCRelationship dcr)
		{
			// No imp
		}

		public virtual void RemoveFromDCR( DCRelationship dcr )
		{
			// No imp
		}

		public virtual void InsertIntoDCR(ArrayList al, int nindex, DCRelationship dcr)
		{
			// No imp
		}

		public virtual void UpdateDCRIndex(DCRelationship dcrs)
		{
			// No imp
		}

		public virtual bool IsTargetController(Point ptscreen)
		{
			if( (this.HostControl.Visible == true) && (this.HostControl.RectangleToScreen(this.HostControl.ClientRectangle).Contains(ptscreen) == true) )
				return true;
			return false;
		}

		public virtual bool IsTargetController(Control ctrl)
		{
			return this.HostControl.Equals(ctrl) ? true : false;
		}

		public virtual void AddChild(DockControllerBase dc, DockingStyle db)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public virtual void InsertChild(DockControllerBase dc, int index, DockingStyle db)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public virtual void RemoveChild(DockControllerBase dc)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public virtual void ReplaceChild(DockControllerBase dccurrent, DockControllerBase dcnew)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public virtual DockControllerBase GetChildAt(int index)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
			return null;
		}

		public virtual int GetChildHostIndex(DockControllerBase child)
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
			return -1;
		}

		public virtual void AdjustLayout()
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public abstract void GetDockInfo(Control ctrl, Point pt, DockInfo di);

		public virtual bool QueryDropProceedWithDock(Control ctrldrop, DockingStyle style)
		{
			Debug.Assert(false, "Invalid Call.\n");
			return false;
		}

		public virtual void CloseController()
		{
			// No imp
			Debug.Assert(false, "Invalid Call.\n");
		}

		public void FireControllerChanged(DockInfo newdi, DCRelationship dcr)
		{
			OnControllerChanged(new ControllerChangedEventArgs(newdi, dcr));
		}

		public virtual void InvokeDocking(DockControllerBase dc)
		{
			Debug.Assert(false, "Invalid Dock Controller.\n");
		}

		public virtual void InvokePrevDockFloatTransition( bool showFloating )
		{
			Debug.Assert(false, "Invalid Dock Controller.\n");
		}

		public virtual DockControllerBase RedockController(DockInfo di, bool bforcenew)
		{
			// Default - no imp
			Debug.Assert(false, "Invalid Call.\n");
			return null;
		}

		public virtual bool AttemptDCRDocking(DockControllerBase dc, IEnumerator iedcr)
		{
			return false;
		}

		public virtual void InvokeDCRDocking(DockControllerBase dc, DCRelationship dcr)
		{
			Debug.Assert(false, "Invalid Dock Controller.\n");
		}

		public virtual bool QueryRelationship(DCRelationship dcr)
		{
			// Default - no imp
			return false;
		}

		protected ControllerDCRPair IterChildControllers(DockControllerBase dc, DCRelationship dcr)
		{
			bool brelexists = dc.QueryRelationship(dcr);
			if(brelexists == true)
				return new ControllerDCRPair(dc, dcr);
			else if(dc.ChildCount <= 0)
				return null;

			for(int i = 0; i < dc.ChildCount; i++)
			{
				ControllerDCRPair cdcrp = IterChildControllers(dc.GetChildAt(i), dcr);
				if(cdcrp != null)
					return cdcrp;
			}
			return null;
		}

		protected void ComputeLRTBBorders(Rectangle rcmain, int nboundary, ref Rectangle[] rc)
		{
			if(rc == null)
				rc = new Rectangle[4];

			rc[0] = new Rectangle(rcmain.Left, rcmain.Top, nboundary, rcmain.Height);	// Left Hit Border
			rc[1] = new Rectangle(rcmain.Left, rcmain.Top, rcmain.Width, nboundary);		// Top Hit Border
			rc[2] = new Rectangle(rcmain.Right-nboundary, rcmain.Top, nboundary, rcmain.Height);	// Right Hit Border
			rc[3] = new Rectangle(rcmain.Left, rcmain.Bottom-nboundary, rcmain.Width, nboundary);	// Bottom Hit Border
		}

		protected virtual void OnControllerChanged(ControllerChangedEventArgs args)
		{
			if(ControllerChanged != null)
			{
				ControllerChanged(this, args);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool bdispose) {}
		
		internal virtual void AddWrapper(ControllerWrapper cw)
		{
		}
		
		internal virtual void StoreControllers(ArrayList controllers)
		{
		}

		internal virtual void ApplyWrapper(ControllerWrapper cw)
		{
		}

		internal virtual bool IsEqual(ControllerWrapper cw)
		{
			return false;
		}

		internal virtual void ResizeControllers(ControllerWrapper cw)
		{
		}

		public virtual void ApplyDockInfo()
		{
		}

		public virtual void MoveController()
		{
		}

		internal virtual void DockAsMDIChild( DockInfo dockInfo )
		{
		}		

		internal abstract bool IsFloatOnly();

		public abstract void DockAsMDIChild();

		public abstract void UpdateControl();
	}


	// A DockStateControllerBase type can be dragged around as well as serve as drop targets. This controller provides
	// feedback and also allows itself to be dropped on other controllers.
	[Syncfusion.Documentation.DocumentationExclude()]
	public abstract class DockStateControllerBase : DockControllerBase
	{
		private int autoHideIndex = -1;
        private int m_previousAutoHideIndex = -1;
		// DockStateControllerBase adds the dockInfoPrevious and dockInfoNew states in addition to dockInfoCurrent
		protected DockInfo dockInfoNew = DockInfo.NullInfo;
		protected DockInfo dockInfoPrevious = DockInfo.NullInfo;

		private DockingStyle m_dockEdge = DockingStyle.Fill;
		protected FloatingForm m_form = null;
		protected DockStateControllerWrapper m_prevWrapper = null;
		protected FloatingForm m_sharedForm = null;
		protected internal DockStateControllerWrapper m_tempWrapper = null;
		protected Size m_prevFloatSize = Size.Empty;
		protected Size m_prevDockSize = Size.Empty;
		protected Point m_prevFloatLocation = Point.Empty;
		protected DockingStyle m_prevAHStyle = DockingStyle.None;
		protected bool bFreezeResize = false;
		protected ArrayList m_storedDockSizes = null;
		protected ArrayList m_storedFloatSizes = null;
		protected DockStateControllerWrapper m_releasedWrapper = null;

		internal Point PreviousFloatLocation
		{
			get
			{
				return m_prevFloatLocation;
			}
			set
			{ 
				if( m_prevFloatLocation != value )
				{
					m_prevFloatLocation = value;
				}
			}
		}

		internal Size PreviousFloatSize
		{
			get
			{
				return m_prevFloatSize;
			}
			set 
			{
				if( m_prevFloatSize != value )
				{
					m_prevFloatSize = value;
				}
			}
		}

		internal Size PreviousDockSize
		{
			get
			{
				return m_prevDockSize;
			}
			set 
			{
				if( m_prevDockSize != value )
				{
					m_prevDockSize = value;
				}
			}
		}

		internal DockStateControllerWrapper PrevWrapper
		{
			get
			{
				return m_prevWrapper;
			}
			set
			{
				if( m_prevWrapper != value )
				{
					m_prevWrapper = value;
				}
			}
		}

		internal DockingStyle PrevAutohideStyle
		{
			get
			{
				return m_prevAHStyle;
			}
			set
			{
				if( m_prevAHStyle != value )
				{
					m_prevAHStyle = value;
				}
			}
		}

		internal virtual FloatingForm InternalForm
		{
			get
			{
				return m_form;
			}
			set
			{
				if( m_form != value )
				{
					m_form = value;
				}
			}
		}

		internal virtual DockStateControllerWrapper TempWrapper
		{
			get
			{
				return m_tempWrapper;
			}
			set
			{
				if( m_tempWrapper != value )
				{
					m_tempWrapper = value;
				}
			}
		}

		internal virtual FloatingForm SharedForm
		{
			get
			{
				return m_sharedForm;
			}
			set
			{
				m_sharedForm = value;
			}
		}
		
		private DockStateControllerWrapper m_dockWrap = null;
		private bool m_bForceSetWrapper = false;

		internal bool ForceSetWrapper
		{
			get
			{
				return m_bForceSetWrapper;
			}
			set
			{
				if( m_bForceSetWrapper != value )
				{
					m_bForceSetWrapper = value;
				}
			}
		}

		internal virtual bool FreezeResize
		{
			get
			{
				return bFreezeResize && !this.DockingManager.ForbidFreeze;
			}
			set
			{
				if( bFreezeResize != value )
				{
					bFreezeResize = value;

					if( value )
						MustResize = Direction.None;
					else
					{
						MustResize = Direction.Both;

						if( this.ParentController != null )
							foreach( DockControllerBase dcb in this.ParentController.ChildControllers )
								dcb.DITransient.rcDockArea = Rectangle.Empty;
					}
				}
			}
		}


		internal virtual DockStateControllerWrapper InternalDockWrapper
		{
			get
			{				
				return m_dockWrap;
			}
			set
			{				
				if( m_dockWrap != value )
				{
					if( m_dockWrap != null )
					{
						if( m_dockWrap.DockRelationControllers.Count != 0 )
						{
							if( value != null )
								m_dockWrap.DockRelationControllers.Remove( this );
							else
								m_releasedWrapper = m_dockWrap;
						}

						DockControllerBase parent = m_dockWrap.ParentController;
						if( parent != null )
						{
							if( m_dockWrap.DockRelationControllers.Count == 0 )
								parent.RemoveChild(m_dockWrap);
							if( value != null )
							{
								//Set wrapper to all related controllers
								if( m_dockWrap.Relations.Count != 0
									&& value.ParentController == m_dockWrap.ParentController
									&& !ForceSetWrapper )
								{
									ArrayList relatedControllers = new ArrayList( m_dockWrap.DockRelationControllers );
									value.Relations = m_dockWrap.Relations;
									value.DockRelationControllers = relatedControllers;

									foreach( DockStateControllerBase relatedController
										in relatedControllers )
									{
										relatedController.InternalDockWrapper = null;
										relatedController.InternalDockWrapper = value;
									}
								}
							}
							// Clear Sizing Controller if needed
							if( parent is SizingController )
								parent.Refresh();
						}
					}

					if( m_releasedWrapper != null && m_releasedWrapper != m_dockWrap
						&& m_releasedWrapper.DockRelationControllers.Count != 0 )
					{
						if( m_releasedWrapper != value )
							m_releasedWrapper.DockRelationControllers.Remove( this );

						m_releasedWrapper = null;
					}

					this.PrevAutohideStyle = DockingStyle.None;
					m_dockWrap = value;
				}
				m_prevWrapper = value;
			}
		}

		private DockStateControllerWrapper m_floatWrap = null;

		protected internal virtual void PerformSizeCorrection( SizingController sc )
		{
			if( sc != null )
				PerformSizeCorrection( sc, sc.DockingOrder );
		}

		protected internal virtual void PerformSizeCorrection( SizingController sc, DockPreference preference )
		{
			if( sc != null )
			{
				if( preference == DockPreference.Horizontal )
				{
					int height = sc.GetDockControllers().Count == 0
						? this.LayoutRect.Height : sc.LayoutRect.Height;

					sc.LayoutRect = new Rectangle(sc.LayoutRect.Left, sc.LayoutRect.Top,
						sc.LayoutRect.Width + this.LayoutRect.Width, height);
				}
				else
				{
					int width = sc.GetDockControllers().Count == 0
						? this.LayoutRect.Width : sc.LayoutRect.Width;

					sc.LayoutRect = new Rectangle(sc.LayoutRect.Left, sc.LayoutRect.Top,
						width, sc.LayoutRect.Height + this.LayoutRect.Height);
				}

				sc.DITransient.rcDockArea = sc.LayoutRect;
				SizingController parentSc = sc.ParentController as SizingController;

				if( this.dockingMgr.DockToFill )
				{
					if( parentSc != null && parentSc.ChildCount == 1
						&& parentSc.ParentController is MainFormController )
						return;
				}

				if( parentSc != null
					&& (sc.DockingOrder == parentSc.DockingOrder || parentSc.GetDockControllers().Count == 1 ))
				{
					SizingController parentSizing = sc.ParentController as SizingController;
					PerformSizeCorrection(parentSizing);
				}
			}
		}

		protected bool CanMaximize
		{
			get
			{
				SizingController sc = null;

				if( this.ParentController is DockTabController )
					sc = this.ParentController.ParentController as SizingController;
				else
					sc = this.ParentController as SizingController;

				SizingController topSizing = this.GetTopSizingController(sc);
				bool maxVis = ( sc != null && topSizing.GetDockControllers().Count > 1 ) && !this.FreezeResize;
				DockTabController tabParent = this.ParentController as DockTabController;

				if( tabParent != null && tabParent.FreezeResize )
					maxVis = false;

				return maxVis;
			}
		}

		internal virtual DockStateControllerWrapper InternalFloatWrapper
		{
			get
			{				
				return m_floatWrap;
			}
			set
			{
				if( m_floatWrap != value )
				{
					if( m_floatWrap != null)
					{
						if( value != null && m_floatWrap.DockRelationControllers.Count != 0
							&& m_floatWrap.ParentController != null
							&& m_floatWrap.ParentController != value.ParentController )
							m_floatWrap.DockRelationControllers.Remove(this);

						DockControllerBase parent = m_floatWrap.ParentController;
						if( parent != null )
						{
							if( m_floatWrap.DockRelationControllers.Count == 0 )
								parent.RemoveChild(m_floatWrap);
							DockControllerBase topLevel = parent.ToplevelController;

							if( value != null )
							{	
								//Set wrapper to all related controllers
								if( m_floatWrap.Relations.Count != 0 &&
									value.ParentController == m_floatWrap.ParentController )
								{
									ArrayList relatedControllers = new ArrayList( m_floatWrap.DockRelationControllers );
									value.Relations = m_floatWrap.Relations;
									value.DockRelationControllers = relatedControllers;

									foreach( DockStateControllerBase relatedController
										in relatedControllers )
									{
										relatedController.InternalFloatWrapper = null;
										relatedController.InternalFloatWrapper = value;
									}
								}
							}

							if( parent is SizingController )
								parent.Refresh();

							if( !(topLevel is SizingController) )
								topLevel.AdjustLayout();
						}
					}

					m_floatWrap = value;
				}
				m_prevWrapper = value;
			}
		}
		
		public DockingStyle DockEdge
		{
			get { return m_dockEdge; }
			set { m_dockEdge = value; }
		}

		public bool bInAutoHide = false;
		public bool bAutoHideSizing = false;

		public int AutoHideIndex
		{
			get { return autoHideIndex; }
			set { autoHideIndex = value; }
		}

        public int PreviousAutoHideIndex
        {
            get 
            { 
                return this.m_previousAutoHideIndex; 
            }
            set
            {
                if (this.m_previousAutoHideIndex != value)
                    this.m_previousAutoHideIndex = value;
            }
        }
		
		public virtual DockInfo DINew
		{
			get { return this.dockInfoNew; }
			set { this.dockInfoNew = value; }
		}

		public virtual DockInfo DIPrevious
		{
			get { return this.dockInfoPrevious; }
			set { this.dockInfoPrevious = value; }
		}

		public virtual bool AutoHideMode
		{
			get { return this.bInAutoHide; }
			set { this.bInAutoHide = value; }
		}

		protected DockStateControllerBase(DockingManager mgr) : base(mgr)
		{
			m_storedDockSizes = new ArrayList();
			m_storedFloatSizes = new ArrayList();
		}

		internal ArrayList StoredDockSizes
		{
			get
			{
				return m_storedDockSizes;
			}
			set
			{
				if( m_storedDockSizes != value )
				{
					m_storedDockSizes = value;
				}
			}
		}

		internal ArrayList StoredFloatSizes
		{
			get
			{
				return m_storedFloatSizes;
			}
			set
			{
				if( m_storedFloatSizes != value )
				{
					m_storedFloatSizes = value;
				}
			}
		}

		public abstract void EnterAutoHideMode();
		public abstract void ExitAutoHideMode(bool bcloseonexit);

		// Controller changed eventhandler for Dock <-> AHMode transitions.
		public virtual void TransientControllerChanged(Object sender, ControllerChangedEventArgs e)
		{
			if( sender.Equals(this.dockInfoTransient.dController) == false )
			{
				Debug.Assert(false, "Error: Event subscriptions out of sync.\n");
				return;
			}

			// Unsubscribe from the previous controller and subscribe to this new controller's event
			if(this.dockInfoTransient.dController != null)
				this.dockInfoTransient.dController.ControllerChanged -= new ControllerChangedEH(this.TransientControllerChanged);
			// Update the dockinfo within the DIPrevious info cache. This new controller will be used while exiting AutoHide mode.
			DockInfo diupdate = e.NewDockInfo;
			if(diupdate.dController != null)
			{
				if( DockingManager.DockToFill )
				{
					bool related = false;
					IEnumerator relEnum = this.DCR;
					relEnum.Reset();
					while( relEnum.MoveNext() )
					{
						related = e.NewDockInfo.dController.QueryRelationship(relEnum.Current as DCRelationship);
						if( related )
							break;
					}
					if( !related )
					{						
						int ncode = this.GetHashCode();
						DCRelationship relation = new DCRelationship(ncode, false, e.NewDockInfo.DP, e.NewDockInfo.nPriority);
						this.AddToDCR(relation);
						e.NewDockInfo.dController.AddToDCR(relation);
					}
				}
				// Retain the dockindex, and update the remaining attributes
				this.dockInfoTransient = new DockInfo(diupdate.dController, diupdate.dStyle, diupdate.nPriority, this.dockInfoTransient.nDockIndex,
					diupdate.DP, diupdate.rcDockArea);
				diupdate.dController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
			}
			else
			{
				// diupdate.dController is null, when a floating controller is closed. ie., no parentcontroller is
				// available higher up in the hierarchy. The dockinfo contains the frame float rect.
				this.dockInfoTransient.dController = null;
				if((diupdate.rcDockArea.Width<=0 || diupdate.rcDockArea.Height<=0) == false)
					this.dockInfoTransient.rcDockArea = diupdate.rcDockArea;
			}
		}

		internal virtual void RestoreController()
		{
			RestoreControllerInternal();

			m_storedSizes.Clear();
			this.Maximized = false;
			this.ToplevelController.AdjustLayout();
		}

		protected void RestoreControllerInternal()
		{			
			DockControllerBase baseCtrl = null;

			if( this.ParentController is DockTabController )
				baseCtrl = this.ParentController;
			else
				baseCtrl = this;

			DockControllerBase sibling = null;
			Size size = Size.Empty;
			Size parentSize = Size.Empty;

			for( int i = 0; i < m_storedSizes.Count - 1; i++ )
			{
				sibling = null;
				int index = -1;
				size = ( Size )m_storedSizes[i];
				parentSize = ( Size )m_storedSizes[i + 1];

				if( baseCtrl.ParentController != null )
				{
					index = baseCtrl.ParentController.GetChildHostIndex(baseCtrl);
					foreach (DockControllerBase ctrl in baseCtrl.ParentController.ChildControllers)
						ctrl.Minimized = Minimization.None;
				}

				baseCtrl.LayoutRect = new Rectangle(baseCtrl.LayoutRect.Location , size);
				baseCtrl.DITransient.rcDockArea = baseCtrl.LayoutRect;

				if( baseCtrl.ParentController != null && baseCtrl.ParentController.ChildCount == 3 )
				{
					if( index == 0 )
						sibling = baseCtrl.ParentController.GetChildAt(2);
					else
						sibling = baseCtrl.ParentController.GetChildAt(0);
				}
				int splitterWidth = this.dockingMgr.SplitterWidth;

				if( m_siblingMinimization == Minimization.Vertical )
					size.Height = parentSize.Height - size.Height - splitterWidth;
				else if( m_siblingMinimization == Minimization.Horizontal )
					size.Width = parentSize.Width - size.Width - splitterWidth;

				if( sibling != null )
				{ 
					sibling.LayoutRect = new Rectangle(sibling.LayoutRect.Location , size);
					sibling.DITransient.rcDockArea = sibling.LayoutRect;
				}

				if( baseCtrl.ParentController != null )
					baseCtrl = baseCtrl.ParentController as SizingController;

				if( baseCtrl.ParentController == null )
					break;
			}            
		}

		protected ArrayList m_storedSizes = new ArrayList();

		internal ArrayList StoredSizes
		{
			get
			{
				return m_storedSizes;
			}
			set
			{
				if( m_storedSizes != value )
				{
					m_storedSizes = value;
				}
			}
		}

		internal virtual void MaximizeController()
		{
			SizingController parent = this.ParentController as SizingController;

			if( parent != null )
			{
				m_storedSizes.Clear();
				SizingController topSizing = GetTopSizingController(parent);

				if( topSizing.DockingOrder == DockPreference.Vertical )
					m_siblingMinimization = Minimization.Vertical;
				else if( topSizing.DockingOrder == DockPreference.Horizontal )
					m_siblingMinimization = Minimization.Horizontal;

				ArrayList controllers = topSizing.GetDockControllers();

				foreach( DockStateControllerBase ctrl in controllers )
					if( ctrl.Maximized )
						ctrl.RestoreController();

				topSizing.Minimized = Minimization.None;
				this.Maximized = true;
				Rectangle rect = this.LayoutRect;

				m_storedSizes.Add(this.LayoutRect.Size);
				Size newSize = new Size(this.LayoutRect.Width, this.LayoutRect.Height);

				if( m_siblingMinimization == Minimization.Vertical )
					newSize.Height = topSizing.LayoutRect.Height;
				else if( m_siblingMinimization == Minimization.Horizontal )
					newSize.Width = topSizing.LayoutRect.Width;

				this.LayoutRect = new Rectangle(this.LayoutRect.Location
					, newSize);

				MaximizeControllerInternal(this.LayoutRect.Size);

				int height = 0;
				foreach( DockControllerBase sibling in this.ParentController.ChildControllers )
				{
					if( sibling != this )
					{
						height += topSizing.DockingOrder == DockPreference.Vertical
							? sibling.LayoutRect.Height : sibling.LayoutRect.Width;
					}
				}

				if( m_siblingMinimization == Minimization.Vertical )
					rect.Height = this.ParentController.LayoutRect.Height - height;
				else if( m_siblingMinimization == Minimization.Horizontal )
					rect.Width = this.ParentController.LayoutRect.Width - height;

				this.DITransient.rcDockArea = rect;
				topSizing.AdjustLayout();

				this.DITransient.rcDockArea = this.LayoutRect;
			}
		}

		internal void ExitMaxMinState()
		{
			DockStateControllerBase baseCtrl = this;

			if( baseCtrl.ParentController is DockTabController )
				baseCtrl = baseCtrl.ParentController as DockStateControllerBase;

			if( this.Maximized )
			{
				SizingController sc = GetTopSizingController(baseCtrl.ParentController as SizingController);
				baseCtrl.Maximized = false;

				if( sc != null )
				{
					sc.Minimized = Minimization.None;
					sc.AdjustLayout();
				}
			}
			else
				baseCtrl.Minimized = Minimization.None;
		}

		protected Minimization m_siblingMinimization = Minimization.None;

		protected virtual void MaximizeControllerInternal( Size size )
		{
			SizingController parent = this.ParentController as SizingController;
			DockControllerBase prev = this;

			while( parent != null )
			{
				parent.PriorityController = prev;
				m_storedSizes.Add(parent.LayoutRect.Size);
				Size newSize = new Size(parent.LayoutRect.Width, parent.LayoutRect.Height);

				if( m_siblingMinimization == Minimization.Vertical )
					newSize.Height = size.Height;
				else if( m_siblingMinimization == Minimization.Horizontal )
					newSize.Width = size.Width;

				parent.LayoutRect = new Rectangle(parent.LayoutRect.Location
					, newSize);
				parent.DITransient.rcDockArea = parent.LayoutRect;
				parent.PriorityController = null;

				foreach( DockControllerBase child in parent.ChildControllers )
				{
					if( child != prev )
					{
						bool differSiblingDirection = false;

						if( child.ParentController == this.ParentController )
						{
							if( ( parent.DockingOrder == DockPreference.Vertical
								&& m_siblingMinimization == Minimization.Horizontal )
								|| ( parent.DockingOrder == DockPreference.Horizontal
								&& m_siblingMinimization == Minimization.Vertical ) )
								differSiblingDirection = true;
						}

						if( !differSiblingDirection )
							child.Minimized = m_siblingMinimization;
					}
				}

				prev = parent;
				parent = parent.ParentController as SizingController;

				if( parent != null
					&& ((parent.DockingOrder != (prev as SizingController).DockingOrder)
					&& (prev as SizingController).GetDockControllers().Count > 1))
					parent = null;
			}
		}

		protected virtual SizingController GetTopSizingController( SizingController sc )
		{
			if( sc == null )
				return null;

			SizingController topLevel = null;
			SizingController parentSizing = sc.ParentController as SizingController;

			if( parentSizing != null
				&& (parentSizing.DockingOrder == sc.DockingOrder || sc.GetDockControllers().Count == 1 ))
				topLevel = GetTopSizingController(parentSizing);
			else
				topLevel = sc;

			return topLevel;
		}

		internal override bool Maximized
		{
			get
			{
				return base.Maximized;
			}
			set
			{
				base.Maximized = value;
			}
		}

		internal override Minimization Minimized
		{
			get
			{
				return m_bMinimized;
			}
			set
			{
				if( m_bMinimized != value )
				{
					if( value != Minimization.None )
					{
						int captionHeight;
						int borderWidth = 3;

						if( DockingManager.VisualStyle == VisualStyle.Default
							|| DockingManager.VisualStyle == VisualStyle.VS2005 )
						{
							captionHeight = SystemInformation.ToolWindowCaptionHeight;
							captionHeight += borderWidth;
						}
						else
							captionHeight = DockingManager.Renderer.CaptionWidth;

						Size minimized = new Size(this.LayoutRect.Width, this.LayoutRect.Height );

						if( value == Minimization.Vertical )
							minimized.Height = captionHeight;
						else
							minimized.Width = captionHeight;

						this.LayoutRect = new Rectangle(this.LayoutRect.Location, minimized);
						this.DITransient.rcDockArea = this.LayoutRect;
					}

					m_bMinimized = value;
				}
			}
		}

		internal override void ApplyWrapper(ControllerWrapper cw)
		{
		}

		internal abstract void SetAutohiddenControlSize( Size size );
	}


	// Enum of permissible drag axis
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum DragAxis
	{
		None = 0,
		X = 1,
		Y,
		XY
	}


	/// All components that expect to be dragged will need to implement the IDraggable interface. This interface
	/// is used by the drag provider to provide drag services to the component.
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDraggable
	{
		DockControllerBase InternalController
		{
			get;
		}

		DockInfo DragDockInfo
		{
			get;
			set;
		}

		Rectangle DragRectangle
		{
			get;
			set;
		}

		bool IsSuitableDockTarget(DockControllerBase dc);
		bool InitiateDrag(MouseAction action, Point pt);
		DragAxis AllowedDragAxis(ref Point ptdrag, Point ptdelta);
		bool DrawHollow();
		void AbortDrag();
		bool QueryDragProceedWithDock();
	}


	/// The DockingManager designer uses this interface for communicating mouse messages
	/// to the docked/floating controls.
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDockingManagerDesignerMouseHook
	{
		void HandleMouseDown(MouseButtons button, Point ptscreen);
		void HandleMouseMove(MouseButtons button, Point ptscreen);
		void HandleMouseUp(MouseButtons button, Point ptscreen);
		void HandleDoubleClick(Point ptscreen);
		void HandleMouseLeave();
		void InitiateFloatingResize(Point ptscreen, int nchittest);
		bool GetDesignMode();
	}


	/// ITabFeedback is implemented by controls that allow tabbed docking/undocking. The FBProvider implementation
	/// uses the ITabFeedback methods to interact with controls, primarily dockhosts, during a tabbed docking
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ITabFeedback
	{
		bool ProvideTabFeedback(IDraggable idg, MouseAction action);
	}

	internal class RectangleCollection : CollectionBase
	{
		public Rectangle this[ int index ]  
		{
			get  
			{
				return( (Rectangle) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( Rectangle value )  
		{
			return( List.Add( value ) );
		}
	}

	internal interface IResizable
	{
		Size CalculateSize( Size parentSize, Size newParentSize );
		bool IsVerticallyResizable();
		bool IsHorizontallyResizable();
	}
	
	// This class is used to calculate controllers size after resizing 
	// or inserting / deleting ect. 
	internal class ControllerSizeCalculator
	{
		private static void RemoveUnresizableArea(DockControllerBase controller,
			ref Size newParentSize)
		{
			SizingController parentController = controller.ParentController as SizingController;
			Size unresizableArea = Size.Empty;
			for(int i = 0; i < parentController.ChildCount; i++ )
			{
				DockControllerBase controllerBase = parentController.GetChildAt(i);
				IResizable resizable = (IResizable) controllerBase;
				if( !resizable.IsHorizontallyResizable() )				
					unresizableArea.Width += controllerBase.LayoutRect.Size.Width;
				if( !resizable.IsVerticallyResizable() )
					unresizableArea.Height += controllerBase.LayoutRect.Size.Height;
				
			}
			
			unresizableArea.Width = parentController.DICurrent.DP == 
				DockPreference.Horizontal ? unresizableArea.Width : 0;
			unresizableArea.Height = parentController.DICurrent.DP == 
				DockPreference.Vertical ? unresizableArea.Height : 0;
			
			newParentSize.Width -= unresizableArea.Width;
			newParentSize.Height -= unresizableArea.Height;
		}
		
		public static int SplitterWidth = 0;

		public static Size CalculateSize(DockControllerBase controller,
			Size parentSize, Size newParentSize)
        {

			SizingController parentController = controller.ParentController as SizingController;
			Size calculatedSize = Size.Empty;
			if( parentController != null )
			{
			Rectangle layoutRect = controller.LayoutRect;
			Rectangle transientRect = controller.DITransient.rcDockArea;
			DockPreference preference = parentController.DICurrent.DP;
			RemoveUnresizableArea(controller, ref newParentSize);

			DockControllerBase priority = parentController.PriorityController;
			bool suitable = (priority != null && priority != controller);

			if( controller.Minimized == Minimization.Vertical && suitable )
				return new Size(
				( int )(parentSize.Width - (priority.LayoutRect.Size).Width - SplitterWidth) ,
				( int )controller.LayoutRect.Height);
			else if( controller.Minimized == Minimization.Vertical && priority == controller )
				return new Size(
				( int )( ( float ) newParentSize.Width / parentSize.Width * transientRect.Width + 0.5) ,
				( int )controller.LayoutRect.Height);
			else if( controller.Minimized == Minimization.Horizontal )
				return new Size(
				( int )controller.LayoutRect.Width ,
				( int )( ( float ) newParentSize.Height / parentSize.Height * transientRect.Height + 0.5));
			else if( controller.Minimized == Minimization.Horizontal && priority == controller )
				return new Size(
				( int )( ( float )newParentSize.Width / parentSize.Width * transientRect.Width + 0.5) ,
				( int )controller.LayoutRect.Height);
            if (newParentSize.Width == 0)
                newParentSize.Width = 5;
			SizingController sc = controller.ParentController as SizingController;

			if( sc != null && sc.GetDockControllers().Count == 1 
				&& ( sc.Floating || sc.DockingManager.DockToFill ) )
			{
				calculatedSize = sc.LayoutRect.Size;
			}
			else
			{
				if( parentController == null || parentController.PriorityController == null )
				{
					calculatedSize = new Size(
						( int )( ( float )newParentSize.Width / parentSize.Width * transientRect.Width + 0.5 ),
						( int )( ( float )newParentSize.Height / parentSize.Height * transientRect.Height + 0.5 ));
				}
				else
				{
					int nWidth = newParentSize.Width;
					int nHeight = newParentSize.Height;
					Size prioritySize = parentController.PriorityController.LayoutRect.Size;

					if( preference == DockPreference.Horizontal )
					{
						if( parentController.PriorityController == controller )
						{
							nWidth = layoutRect.Width;
						}
						else
						{
                            int parentWidth = 0;
                            if (controller.DockingManager.m_bLoadingDockState)
                                parentWidth = nWidth - prioritySize.Width;
                            else
                                parentWidth = parentSize.Width - prioritySize.Width - SplitterWidth;

							nWidth = ( int )( ( ( float )parentWidth ) * layoutRect.Width / parentSize.Width + 0.5 );
						}
					}
					else
					{
						if( parentController.PriorityController == controller )
						{
							nHeight = layoutRect.Height;
						}
						else
						{
                            int parentHeight= 0;
                            if (controller.DockingManager.m_bLoadingDockState)
                                parentHeight = nHeight - prioritySize.Height;
                            else
                                parentHeight = parentSize.Height - prioritySize.Height - SplitterWidth;
                            
							nHeight = ( int )( ( ( float )parentHeight ) * layoutRect.Height / parentSize.Height + 0.5 );
						}
					}
					calculatedSize = new Size(nWidth, nHeight);
				}
			}
		}
		
			IResizable resizable = controller as IResizable;

			if( resizable != null )
			{
				if( !resizable.IsHorizontallyResizable() )
					calculatedSize.Width = controller.LayoutRect.Width;
				if( !resizable.IsVerticallyResizable() )
					calculatedSize.Height = controller.LayoutRect.Height;
			}

			return calculatedSize;
		}
	}
}

