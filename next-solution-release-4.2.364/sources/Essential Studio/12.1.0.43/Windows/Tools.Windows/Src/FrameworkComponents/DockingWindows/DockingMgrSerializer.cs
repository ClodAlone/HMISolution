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
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;

using Syncfusion.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	// New Version of docking manager serializer
	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockingMgrSerializationWrapperAdv
		:DockingMgrSerializationWrapper
	{
		private ArrayList ffControllerWrappers = new ArrayList();
		private ArrayList m_zOrderArray = null;
		private ArrayList m_autohideOnLoad = new ArrayList();

		internal ArrayList MdiZOrder
		{
			get
			{
				return m_zOrderArray;
			}
			set
			{
				if( m_zOrderArray != value )
				{
					m_zOrderArray = value;
				}
			}
		}

		internal ArrayList FloatingControllerWrappers
		{
			get
			{
				return ffControllerWrappers;
			}
			set
			{
				if( ffControllerWrappers != value )
				{
					ffControllerWrappers = value;
				}
			}
		}

		internal ArrayList AutohideOnLoad
		{
			get
			{
				return m_autohideOnLoad;
			}
			set
			{
				if( m_autohideOnLoad != value )
				{
					m_autohideOnLoad = value;
				}
			}
		}

		private DockingMgrSerializationWrapperAdv(SerializationInfo info, StreamingContext context)
		{
			// ALEXK: added code which prevent exceptions throwing - exceptions catching is
			// too long operation and it greatly reduce library startup speed
			Hashtable hash = new Hashtable();
			foreach( SerializationEntry infoData in info )
			{
				hash[ infoData.Name ] = null;
			}

			if( hash.Contains( "HTDockingMgrSerializer" ) )
				this.htDHCWrapper = info.GetValue( "HTDockingMgrSerializer", typeof( Hashtable ) ) as Hashtable;

			if( hash.Contains( SerializationConsts.WRAPPER ) )
				controllerWrapper = info.GetValue( SerializationConsts.WRAPPER, typeof( ControllerWrapper ) ) as ControllerWrapper;

			if( hash.Contains(SerializationConsts.FLOAT_WRAPPER) )
				ffControllerWrappers = info.GetValue(SerializationConsts.FLOAT_WRAPPER, typeof(ArrayList)) as ArrayList;

			if( hash.Contains(SerializationConsts.MDIZORDER) )
				m_zOrderArray = info.GetValue(SerializationConsts.MDIZORDER, typeof(ArrayList)) as ArrayList;

			if( hash.Contains( SerializationConsts.AHONLOAD ) )
				m_autohideOnLoad = info.GetValue(SerializationConsts.AHONLOAD, typeof( ArrayList)) as ArrayList;

			hash.Clear();
		}

		// ISerializable implementation
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("HTDockingMgrSerializer", this.htDHCWrapper);
			info.AddValue(SerializationConsts.WRAPPER, controllerWrapper, typeof(ControllerWrapper));
			info.AddValue(SerializationConsts.FLOAT_WRAPPER, ffControllerWrappers, typeof(ArrayList));
			info.AddValue(SerializationConsts.MDIZORDER, m_zOrderArray, typeof(ArrayList));
			info.AddValue(SerializationConsts.AHONLOAD, m_autohideOnLoad, typeof( ArrayList));
		}

		public DockingMgrSerializationWrapperAdv(DockingManager dmgr)
		{
			ArrayList alcontrollers = dmgr.DockAreaControllers;

			if( dmgr.MdiZOrder.Count == 0 )
				dmgr.SaveMdiZOrder();

			m_zOrderArray = new ArrayList(dmgr.MdiZOrder);
			dmgr.MdiZOrder.Clear();

			foreach(DockControllerBase dcbase in alcontrollers)
			{
				if(dcbase is DockHostController)
				{
					String strname = String.Empty;
					if(dmgr.DesignMode == true)
						strname= TypeDescriptor.GetComponentName(dcbase.HostControl.Controls[0]);
					else
					{
						if(dcbase.HostControl.Controls.Count > 0)
							strname = dcbase.HostControl.Controls[0].Name;
						else
						{
							// The DockHostController is in the MDI mode
							DockHostController dhc = dcbase as DockHostController;
							Debug.Assert((dhc.bInMDIMode == true) && (dhc.ctrlReference != null));
							strname = dhc.ctrlReference.Name;
						}
					}
					if(strname == String.Empty)
						throw( new ApplicationException("Dockable controls must have an unique name for proper persistence.") );
					if(this.htDHCWrapper.Contains(strname) == true)
						this.htDHCWrapper.Remove(strname);
					DHCSerializationWrapper dhcserializer = new DHCSerializationWrapper(dcbase as DockHostController);
					this.htDHCWrapper.Add(strname, dhcserializer);
				}
			}
			
			controllerWrapper = dmgr.GetWrapper();
			ffControllerWrappers = dmgr.GetFloatingWrapper();
			m_autohideOnLoad = dmgr.GetAhOnLoadList();
		}
	}
	// The DockingMgrSerializationWrapper and DHCSerializationWrapper are classes that aid in
	// the serialization of the dockingmanager and the associated dockhostcontrollers.
	// DockHostControllers cannot be directly deserialized as the dockhost needs to be in a certain state, depending upon the
	// deserialized attribute, before applying the persisted state to the current dockhostcontroller. Further, deserialzing a
	// dockhost is not classic deserialization, but merely an update of certain attributes and applying the relevant layout changes.
	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockingMgrSerializationWrapper : ISerializable
	{
		public Hashtable htDHCWrapper = new Hashtable();
		internal ControllerWrapper controllerWrapper = null;
		
		public DockingMgrSerializationWrapper()
		{
		}

		// Private constructor called during the deserialization process
		private DockingMgrSerializationWrapper(SerializationInfo info, StreamingContext context)
		{
			// ALEXK: added code which prevent exceptions throwing - exceptions catching is
			// too long operation and it greatly reduce library startup speed
			Hashtable hash = new Hashtable();
			foreach( SerializationEntry infoData in info )
			{
				hash[ infoData.Name ] = null;
			}

			if( hash.Contains( "HTDockingMgrSerializer" ) )
				this.htDHCWrapper = info.GetValue( "HTDockingMgrSerializer", typeof( Hashtable ) ) as Hashtable;

			if( hash.Contains( SerializationConsts.WRAPPER ) )
				controllerWrapper = info.GetValue( SerializationConsts.WRAPPER, typeof( ControllerWrapper ) ) as ControllerWrapper;

			hash.Clear();
      
			#region /* comments */
			/*
			this.htDHCWrapper = info.GetValue("HTDockingMgrSerializer", typeof(Hashtable)) as Hashtable;
			try
			{
			  controllerWrapper = (ControllerWrapper)
				info.GetValue(SerializationConsts.WRAPPER, typeof(ControllerWrapper));
			}
			catch( SerializationException )
			{
			  controllerWrapper = null;
			}
			*/
			#endregion
		}

		// ISerializable implementation
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("HTDockingMgrSerializer", this.htDHCWrapper);
			info.AddValue(SerializationConsts.WRAPPER, controllerWrapper, typeof(ControllerWrapper));
		}

		public DockingMgrSerializationWrapper(DockingManager dmgr)
		{
			ArrayList alcontrollers = dmgr.DockAreaControllers;
			foreach(DockControllerBase dcbase in alcontrollers)
			{
				if(dcbase is DockHostController)
				{
					String strname = String.Empty;
					if(dmgr.DesignMode == true)
						strname= TypeDescriptor.GetComponentName(dcbase.HostControl.Controls[0]);
					else
					{
						if(dcbase.HostControl.Controls.Count > 0)
							strname = dcbase.HostControl.Controls[0].Name;
						else
						{
							// The DockHostController is in the MDI mode
							DockHostController dhc = dcbase as DockHostController;
							Debug.Assert((dhc.bInMDIMode == true) && (dhc.ctrlReference != null));
							strname = dhc.ctrlReference.Name;
						}
					}
					if(strname == String.Empty)
						throw( new ApplicationException("Dockable controls must have an unique name for proper persistence.") );
					if(this.htDHCWrapper.Contains(strname) == true)
						this.htDHCWrapper.Remove(strname);
					DHCSerializationWrapper dhcserializer = new DHCSerializationWrapper(dcbase as DockHostController);
					this.htDHCWrapper.Add(strname, dhcserializer);
				}
			}
			
			controllerWrapper = dmgr.GetWrapper();
		}

		public DHCSerializationWrapper GetDHCSerializationWrapper(DockHostController dhc)
		{
			String strname = String.Empty;
			if(dhc.DockingManager.DesignMode == true)
				strname = TypeDescriptor.GetComponentName(dhc.HostControl.Controls[0]);
			else
			{
				if(dhc.HostControl.Controls.Count > 0)
					strname = dhc.HostControl.Controls[0].Name;
				else
				{
					// The DockHostController is in the MDI mode
					Debug.Assert((dhc.bInMDIMode == true) && (dhc.ctrlReference != null));
					strname = dhc.ctrlReference.Name;
				}
			}
			if(strname == String.Empty)
				throw( new ApplicationException("All dockable controls must have an unique name for proper persistence.") );
			if(this.htDHCWrapper.Contains(strname) == true)
				return ((this.htDHCWrapper[strname]) as DHCSerializationWrapper);
			return null;
		}

		public DHCSerializationWrapper GetDHCSerializationWrapper(String ctrlname)
		{
			if(this.htDHCWrapper.Contains(ctrlname) == true)
				return ((this.htDHCWrapper[ctrlname]) as DHCSerializationWrapper);
			return null;
		}

		// Used for depersistence only after a version change of the Syncfusion Tools library.
		internal static DockingMgrSerializationWrapper CloneByReflection(Object objthis)
		{
			DockingMgrSerializationWrapper dmgrserializer = new DockingMgrSerializationWrapper();

			FieldInfo finfo = objthis.GetType().GetField("htDHCWrapper");
			Hashtable ht = finfo.GetValue(objthis) as Hashtable;
			foreach(Object objdhcwrapper in ht.Values)
			{
				DHCSerializationWrapper dhcwrapper = DHCSerializationWrapper.CloneByReflection(objdhcwrapper);
				if(dmgrserializer.htDHCWrapper.Contains(dhcwrapper.strName) == true)
					dmgrserializer.htDHCWrapper.Remove(dhcwrapper.strName);
				dmgrserializer.htDHCWrapper.Add(dhcwrapper.strName,dhcwrapper);
			}

			return dmgrserializer;
		}
	}


	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class DHCSerializationWrapper : ISerializable
	{
		public String uniqueName;
		public String strName;
		public String strLabel = String.Empty;
		public DockInfo dockInfoPrevious = DockInfo.NullInfo;
		public DockInfo dockInfoCurrent = DockInfo.NullInfo;
		public ArrayList alDockDCR = new ArrayList();
		public ArrayList alFloatDCR = new ArrayList();
		public ArrayList tabSiblings = new ArrayList();
		public bool bDockVisibility = false;
		public bool bAutoHideMode = false;
		public bool bFloatOnly = false;
		public bool bInMDIMode = false;
		public bool bSelectedPage = false;
		public Size ctrlSize = Size.Empty;
		public Size layoutSize = Size.Empty;
		public int autoHideIndex = -1;
		public DockingStyle dockEdge = DockingStyle.Fill;
		public Point ctrlLocation = Point.Empty;
		public bool previousFloat = false;
		public FloatingForm sharedForm = null;
		public bool maximized = false;
		public int imageIndex = -1;
		public int dockAbility = -1;
		public int outerDockAbility = -1;
		public DockingStyle prevAHStyle = DockingStyle.None;
		public bool bFreezeResize = false;
		public bool bAllowFloating = true;
		public Rectangle transientRect = Rectangle.Empty;
        public FormWindowState windowState = FormWindowState.Normal;
        public ArrayList m_storedDockSizes = new ArrayList();
        public ArrayList m_storedFloatSizes = new ArrayList();

		private DHCSerializationWrapper()
		{
		}

		public DHCSerializationWrapper(DockHostController dhc)
		{
			Control dockhostclient;
			bool mdiChild = false;
			if(dhc.HostControl.Controls.Count > 0)
				dockhostclient = dhc.HostControl.Controls[0];
			else
			{
				// The DockHostController is in the MDI mode
				Debug.Assert((dhc.bInMDIMode == true) && (dhc.ctrlReference != null));
				dockhostclient = dhc.ctrlReference;
				mdiChild = true;
			}

			if(dhc.DockingManager.DesignMode == true)
				this.strName = TypeDescriptor.GetComponentName(dockhostclient);
			else
				this.strName = dockhostclient.Name;
			this.strLabel = dhc.HostControl.Text;			
			this.alDockDCR = dhc.DockDCRList;
			this.alFloatDCR = dhc.FloatDCRList;
			this.bDockVisibility = dhc.DockVisibility;
			this.bAutoHideMode = dhc.AutoHideMode;

			if( !bDockVisibility )
				this.bAutoHideMode = this.bAutoHideMode || dhc.AutoHiddedBeforeHide;

			this.bFloatOnly = dhc.FloatOnly;
			this.bInMDIMode = dhc.MdiChildState || dhc.bInMDIMode;
			if( dhc.PrevWrapper != null && (!dhc.DockVisibility || dhc.bInMDIMode))
				this.previousFloat = dhc.PrevWrapper.Floating;
			this.sharedForm = dhc.SharedForm;
			this.prevAHStyle = dhc.PrevAutohideStyle;
			this.transientRect = dhc.DITransient.rcDockArea;

			if( !dhc.DockVisibility || dhc.bInMDIMode )
			{
				if( dhc.PrevWrapper != null && dhc.PrevWrapper.DockRelationControllers.Count > 0 )
				{
					foreach( DockHostController sibling in dhc.PrevWrapper.DockRelationControllers )
						this.tabSiblings.Add( sibling.UniqueName );
				}
			}
			else if( dhc.AutoHideMode && !( dhc.ParentController is DockTabController ) )
			{
				if( dhc.InternalDockWrapper != null && dhc.InternalDockWrapper.DockRelationControllers.Count > 0 )
				{
					foreach( DockHostController sibling in dhc.InternalDockWrapper.DockRelationControllers )
						if( (!sibling.AutoHideMode && sibling.DockVisibility && !sibling.Floating)
							|| sibling == dhc )
							this.tabSiblings.Add( sibling.UniqueName );
				}
			}

			if( dhc.bInAutoHide )
				this.layoutSize = dhc.DINew.rcDockArea.Size;
			else
				this.layoutSize = dhc.LayoutRect.Size;

			if( mdiChild )
			{
				this.ctrlSize = dockhostclient.Parent.Size;
				this.ctrlLocation = dockhostclient.Parent.Location;
			}
			else
			{
				if( dhc.MdiChildState )
				{
					this.ctrlSize = dhc.MdiChildBounds.Size;
					this.ctrlLocation = dhc.MdiChildBounds.Location;
				}
				else
				{
					this.ctrlSize = dockhostclient.Size;
					this.ctrlLocation = Point.Empty;
				}
			}
			this.autoHideIndex = dhc.AutoHideIndex;
			this.bSelectedPage = dhc.IsSelectedTabPage;
			this.uniqueName = dhc.UniqueName;
			this.dockEdge = dhc.DockEdge;
			this.maximized = dhc.Maximized;
			this.imageIndex = dhc.ImageIndex;
            this.windowState = dhc.MdiWindowState;

			Control ctrl = dhc.ctrlReference;
			if( ctrl == null && dhc.HostControl.Controls.Count > 0 )
				ctrl = dhc.HostControl.Controls[0];

			this.dockAbility = (int)dhc.DockingManager.GetDockAbility( ctrl );
			this.outerDockAbility = (int)dhc.DockingManager.GetOuterDockAbility( ctrl );
			this.bFreezeResize = dhc.FreezeResize;
			this.bAllowFloating = dhc.AllowFloating;

            this.m_storedDockSizes = dhc.StoredDockSizes.Clone() as ArrayList;
            this.m_storedFloatSizes = dhc.StoredFloatSizes.Clone() as ArrayList;
		}

		private bool GetBoolean( Hashtable hashtable, string key )
		{
			object obj = hashtable[ key ];
			if( obj.GetType() == typeof(System.Boolean) )
				return (bool) obj;
			else
			{
				string str = (string) obj;
				return str == "true" ? true : false;
			}
		}

		private int GetInterger( Hashtable hashtable, string key )
		{
			object obj = hashtable[ key ];
			if( obj.GetType() == typeof(System.Int32) )
				return (int) obj;
			else
			{
				string str = (string) obj;
				return Int32.Parse( str );
			}
		}
		
		// Private constructor called during the deserialization process
		private DHCSerializationWrapper(SerializationInfo info, StreamingContext context)
		{
			this.strName = info.GetString("DHCName");
			try
			{
				Hashtable hashTable = new Hashtable();
				SerializationInfoEnumerator items = info.GetEnumerator();
				while( items.MoveNext() )
				{
					hashTable.Add( items.Name, items.Value);
				}

				strLabel = (string) hashTable["DHCLabel"];
				dockInfoPrevious = (DockInfo) hashTable["PreviousDI"];
				dockInfoCurrent = (DockInfo) hashTable["CurrentDI"];
				alDockDCR = (ArrayList) hashTable["DockDCRList"];
				alFloatDCR = (ArrayList) hashTable["FloatDCRList"];
				bDockVisibility = GetBoolean( hashTable, "DockVisibility" );
				bAutoHideMode = GetBoolean( hashTable, "AutoHideMode" );
				bFloatOnly = GetBoolean( hashTable, "FloatOnly" );
				bInMDIMode = GetBoolean( hashTable, "MDIMode" );
				ctrlSize = (Size) hashTable["ControlSize"];				
				layoutSize = (Size) hashTable["LayoutSize"];

				if( hashTable.Contains("PreviousFloat") )
					previousFloat = GetBoolean(hashTable, "PreviousFloat");

				if( hashTable.Contains("AutoHideIndex") )
					autoHideIndex = GetInterger( hashTable, "AutoHideIndex" );
				else
					autoHideIndex = -1;
				if( hashTable.Contains("IsSelectedPage") )
					bSelectedPage = GetBoolean( hashTable, "IsSelectedPage" );
				if( hashTable.Contains("UniqueName") )
					uniqueName = (string) hashTable["UniqueName"];
				else
					uniqueName = strName;

				if( hashTable.Contains("DockEdge") )
					dockEdge = (DockingStyle) hashTable["DockEdge"];
				if( hashTable.Contains("ControlLocation") )
					ctrlLocation = ( Point )hashTable["ControlLocation"];
				if( hashTable.Contains("Maximized") )
					maximized = GetBoolean(hashTable, "Maximized");
				if( hashTable.Contains("ImageIndex") )
					imageIndex = GetInterger(hashTable, "ImageIndex");
				else
					imageIndex = -2;
				if( hashTable.Contains("DockAbility") )
					dockAbility = Convert.ToInt32(hashTable["DockAbility"]);
				if (hashTable.Contains("OuterDockAbility"))
					outerDockAbility = Convert.ToInt32(hashTable["OuterDockAbility"]);
				if( hashTable.Contains("PrevAHStyle") )
					prevAHStyle = (DockingStyle)hashTable["PrevAHStyle"];
				if( hashTable.Contains("FreezeResize") )
					bFreezeResize = GetBoolean(hashTable, "FreezeResize");
				if( hashTable.Contains("TabSiblings") )
					tabSiblings = (ArrayList)hashTable["TabSiblings"];
				if( hashTable.Contains( "AllowFloating" ) )
					bAllowFloating = GetBoolean( hashTable, "AllowFloating" );
				if( hashTable.Contains( "TransientRectangle" ) )
					transientRect = (Rectangle)hashTable["TransientRectangle"];
                if (hashTable.Contains("FormWindowState"))
                    windowState = (FormWindowState)hashTable["FormWindowState"];
                if (hashTable.Contains("StoredDockSizes"))
                    m_storedDockSizes = (ArrayList)hashTable["StoredDockSizes"];
                if (hashTable.Contains("StoredFloatSizes"))
                    m_storedFloatSizes = (ArrayList)hashTable["StoredFloatSizes"];
			}
			catch(Exception)
			{
				// Use default values
			}
		}

		// ISerializable implementation
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("DHCName", this.strName);
			info.AddValue("DHCLabel", this.strLabel);
			info.AddValue("PreviousDI", this.dockInfoPrevious, typeof(DockInfo));
			info.AddValue("CurrentDI", this.dockInfoCurrent, typeof(DockInfo));
			info.AddValue("DockDCRList", this.alDockDCR, typeof(ArrayList));
			info.AddValue("FloatDCRList", this.alFloatDCR, typeof(ArrayList));
			info.AddValue("DockVisibility", this.bDockVisibility);
			info.AddValue("AutoHideMode", this.bAutoHideMode);
			info.AddValue("FloatOnly", this.bFloatOnly);
			info.AddValue("MDIMode", this.bInMDIMode);
			info.AddValue("ControlSize", this.ctrlSize, typeof(Size));
			info.AddValue("ControlLocation", this.ctrlLocation, typeof(Point));
			info.AddValue("LayoutSize", this.layoutSize, typeof(Size));
			info.AddValue("AutoHideIndex", this.autoHideIndex);
			info.AddValue("IsSelectedPage", this.bSelectedPage);
			info.AddValue("UniqueName", this.uniqueName);
			info.AddValue("DockEdge", this.dockEdge);
			info.AddValue("PreviousFloat", this.previousFloat);
			info.AddValue("Maximized", this.maximized);
			info.AddValue("ImageIndex", this.imageIndex);
			info.AddValue("DockAbility", this.dockAbility);
			info.AddValue("OuterDockAbility", this.outerDockAbility);
			info.AddValue("PrevAHStyle", this.prevAHStyle);
			info.AddValue("FreezeResize", this.bFreezeResize);
			info.AddValue("TabSiblings", this.tabSiblings);
			info.AddValue("AllowFloating", this.bAllowFloating);
			info.AddValue("TransientRectangle", this.transientRect);
            info.AddValue("FormWindowState", this.windowState);
            info.AddValue("StoredDockSizes", this.m_storedDockSizes, typeof(ArrayList));
            info.AddValue("StoredFloatSizes", this.m_storedFloatSizes, typeof(ArrayList));
		}

		// Used for depersistence only after a version change of the Syncfusion Tools library.
		internal static DHCSerializationWrapper CloneByReflection(Object objthis)
		{
			DHCSerializationWrapper dhcwrapper = new DHCSerializationWrapper();

			FieldInfo[] fields = objthis.GetType().GetFields();
			foreach(FieldInfo finfo in fields)
			{
				switch(finfo.Name)
				{
					case "strName":
						dhcwrapper.strName = finfo.GetValue(objthis) as String;
						break;
					case "strLabel":
						dhcwrapper.strLabel = finfo.GetValue(objthis) as String;
						break;
					case "dockInfoPrevious":
						dhcwrapper.dockInfoPrevious = DockInfo.CloneByReflection(finfo.GetValue(objthis));
						break;
					case "dockInfoCurrent":
						dhcwrapper.dockInfoCurrent = DockInfo.CloneByReflection(finfo.GetValue(objthis));
						break;
					case "alDockDCR":
						dhcwrapper.alDockDCR = new ArrayList();
						ArrayList aldockdcr = finfo.GetValue(objthis) as ArrayList;
						foreach(Object objdcrelation in aldockdcr)
						{
							DCRelationship dcr = DCRelationship.CloneByReflection(objdcrelation);
							dhcwrapper.alDockDCR.Add(dcr);
						}
						break;
					case "alFloatDCR":
						dhcwrapper.alFloatDCR = new ArrayList();
						ArrayList alfloatdcr = finfo.GetValue(objthis) as ArrayList;
						foreach(Object objdcrelation in alfloatdcr)
						{
							DCRelationship dcr = DCRelationship.CloneByReflection(objdcrelation);
							dhcwrapper.alFloatDCR.Add(dcr);
						}
						break;
					case "bDockVisibility":
						dhcwrapper.bDockVisibility = (bool)finfo.GetValue(objthis);
						break;
					case "bAutoHideMode":
						dhcwrapper.bAutoHideMode = (bool)finfo.GetValue(objthis);
						break;
					case "bFloatOnly":
						dhcwrapper.bFloatOnly = (bool)finfo.GetValue(objthis);
						break;
					case "bInMDIMode":
						dhcwrapper.bInMDIMode = (bool)finfo.GetValue(objthis);
						break;
					case "ctrlSize":
						dhcwrapper.ctrlSize = (Size)finfo.GetValue(objthis);
						break;
					case "layoutSize":
						dhcwrapper.layoutSize = (Size)finfo.GetValue(objthis);
						break;
					case "autoHideIndex":
						dhcwrapper.autoHideIndex = (int)finfo.GetValue(objthis);
						break;
					case "bSelectedPage":
						dhcwrapper.bSelectedPage = (bool)finfo.GetValue(objthis);
						break;
					case "uniqueName":
						dhcwrapper.uniqueName = (string)finfo.GetValue(objthis);
						break;
					case "DockEdge":
						dhcwrapper.dockEdge = (DockingStyle) finfo.GetValue(objthis);
						break;
					case "previousFloat":
						dhcwrapper.previousFloat = (bool)finfo.GetValue(objthis);
						break;
					case "maximized":
						dhcwrapper.maximized = (bool)finfo.GetValue(objthis);
						break;
					case "imageIndex":
						dhcwrapper.imageIndex = (int)finfo.GetValue(objthis);
						break;
					case "dockAbility":
						dhcwrapper.dockAbility = (int)finfo.GetValue(objthis);
						break;
					case "outerDockAbility":
						dhcwrapper.outerDockAbility = (int)finfo.GetValue(objthis);
						break;
					case "prevAHStyle":
						dhcwrapper.prevAHStyle = (DockingStyle)finfo.GetValue(objthis);
						break;
					case "FreezeResize":
						dhcwrapper.bFreezeResize = (bool)finfo.GetValue(objthis);
						break;
                    case "FormWindowState":
                        dhcwrapper.windowState = (FormWindowState)finfo.GetValue(objthis);
                        break;	
					case "TabSiblings":
						dhcwrapper.tabSiblings.Clear();
						ArrayList tabSibling = finfo.GetValue(objthis) as ArrayList;
						foreach( string name in tabSibling )
						{
							string unName = name.Clone() as string;
							dhcwrapper.tabSiblings.Add( unName );
						}
						break;
					case "AllowFloating":
						dhcwrapper.bAllowFloating = (bool)finfo.GetValue(objthis);
						break;
					case "TransientRectangle":
						dhcwrapper.transientRect = (Rectangle)finfo.GetValue(objthis);
						break;
                    case "StoredDockSizes":
                        dhcwrapper.m_storedDockSizes = (ArrayList)finfo.GetValue(objthis);
                        break;
                    case "StoredFloatSizes":
                        dhcwrapper.m_storedFloatSizes = (ArrayList)finfo.GetValue(objthis);
                        break;
				}
			}

			return dhcwrapper;
		}
	}

	[Serializable]
	internal class ControllerWrapper: ISerializable
	{
		public ArrayList Children = new ArrayList();
		
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)		
		{
			info.AddValue(SerializationConsts.CHILDREN, Children, typeof(ArrayList));
		}
		
		protected ControllerWrapper(SerializationInfo info, StreamingContext context)
		{
			Children = (ArrayList) info.GetValue(SerializationConsts.CHILDREN, typeof(ArrayList));
		}	
		
		public ControllerWrapper()
		{
		}	
	}
	
	[Serializable]
	internal class MainFormControllerWrapper: ControllerWrapper, ISerializable
	{
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected MainFormControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		
		}	
		
		public MainFormControllerWrapper(): base()
		{
		}	
	}

	[Serializable]
	internal class FloatingFormControllerWrapper:LayoutControllerWrapper, ISerializable
	{
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData(info, context);
		}

		protected FloatingFormControllerWrapper( SerializationInfo info, StreamingContext context )
			: base(info, context)
		{}

		public FloatingFormControllerWrapper()
			: base()
		{ }
	}
	
	[Serializable]
	internal class LayoutControllerWrapper: ControllerWrapper, ISerializable
	{
		private Rectangle layoutRect;
		public Rectangle LayoutRect
		{
			get { return layoutRect; }
			set { layoutRect = value; }
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SerializationConsts.LAYOUT, layoutRect, typeof(Rectangle));
		}
		
		protected LayoutControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			layoutRect = (Rectangle) info.GetValue(SerializationConsts.LAYOUT, typeof(Rectangle));
		}		
		
		public LayoutControllerWrapper(): base()
		{
		}
	}
	
	[Serializable]
	internal class SizingControllerWrapper: LayoutControllerWrapper, ISerializable
	{
		private DockPreference orientation;
		public DockPreference Orientation
		{
			get { return orientation; }
			set { orientation = value; }
		}	
		
		private DockingStyle style;
		public DockingStyle Style
		{
			get { return style; }
			set { style = value; }
		}

		private Int32 nPriority = -1;
		public Int32 Priority
		{
			get { return nPriority; }
			set { nPriority = value; }
		}

		private Rectangle m_transientRect = Rectangle.Empty;
		public Rectangle TransientRect
		{
			get { return m_transientRect; }
			set { m_transientRect = value; }
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SerializationConsts.ORIENTATION, orientation, typeof(DockPreference));
			info.AddValue(SerializationConsts.STYLE, style, typeof(DockingStyle));
			info.AddValue(SerializationConsts.PRIORITY, nPriority, typeof(Int32));
			info.AddValue(SerializationConsts.TRANSIENTRECT, m_transientRect, typeof(Rectangle));
		}

		protected SizingControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			orientation = (DockPreference)
				info.GetValue(SerializationConsts.ORIENTATION, typeof(DockPreference));
			style = (DockingStyle) info.GetValue(SerializationConsts.STYLE, typeof(DockingStyle));

			Hashtable hashTable = new Hashtable();
			SerializationInfoEnumerator items = info.GetEnumerator();
			while( items.MoveNext() )
			{
				hashTable.Add( items.Name, items.Value);
			}

			if( hashTable.Contains(SerializationConsts.PRIORITY) )
				nPriority = info.GetInt32(SerializationConsts.PRIORITY);
			if( hashTable.Contains( SerializationConsts.TRANSIENTRECT ) )
				m_transientRect = (Rectangle)info.GetValue( SerializationConsts.TRANSIENTRECT, typeof(Rectangle));
		}		
		
		public SizingControllerWrapper(): base()
		{
		}
	}
	
	[Serializable]
	internal class DockHostControllerWrapper: LayoutControllerWrapper, ISerializable
	{
		private string controlName;
		private string uniqueName;
        private DockingStyle m_DockEdge;

        public DockingStyle DockEdge
        {
            get { return this.m_DockEdge; }
            set
            {
                if (this.m_DockEdge != value)
                    this.m_DockEdge = value;
            }
        }
		public string ControlName
		{
			get { return controlName; }
			set { controlName = value; }
		}

		public string UniqueName
		{
			get { return uniqueName; }
			set { uniqueName = value; }
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SerializationConsts.CONTROL_NAME, controlName);
			info.AddValue(SerializationConsts.UNIQUE_NAME, uniqueName);
            info.AddValue(SerializationConsts.STYLE, m_DockEdge, typeof(DockingStyle));
		}

		protected DockHostControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			controlName = info.GetString(SerializationConsts.CONTROL_NAME);

			try
			{
				uniqueName = info.GetString(SerializationConsts.UNIQUE_NAME);
			}
			catch
			{
				uniqueName = controlName;
			}

            SerializationInfoEnumerator e = info.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Name == SerializationConsts.STYLE)
                {
                    m_DockEdge = (DockingStyle)info.GetValue(SerializationConsts.STYLE, typeof(DockingStyle));
                    break;
                }
            }
          
		}	
		
		public DockHostControllerWrapper(): base()
		{
		}	
	}

	[Serializable]
	public class InternalDockStateWrapper
		: ISerializable
	{
		private ArrayList m_internalController = null;
		private Hashtable m_relations = null;

		public ArrayList InternalController
		{
			get
			{
				return m_internalController;
			}
			set
			{
				if( m_internalController != value )
				{
					m_internalController = value;
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

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(SerializationConsts.INTERNAL_CONTROL, m_internalController);
			info.AddValue(SerializationConsts.RELATIONS, m_relations);
		}

		protected InternalDockStateWrapper(SerializationInfo info, StreamingContext context)
		{
			m_internalController = (ArrayList)info.GetValue(SerializationConsts.INTERNAL_CONTROL, typeof(ArrayList));
			m_relations = (Hashtable)info.GetValue(SerializationConsts.RELATIONS, typeof(Hashtable));
		}

		public InternalDockStateWrapper()
			: base()
		{
		}	
	}

	[Serializable]
	internal class DockStateWrapper
		: LayoutControllerWrapper, ISerializable
	{
		private ArrayList m_internalController = null;
		private Hashtable m_relations = null;
		private Size m_storedLayoutSize = Size.Empty;

		public Size StoredLayoutSize
		{
			get
			{
				return m_storedLayoutSize;
			}
			set
			{
				if( m_storedLayoutSize != value )
				{
					m_storedLayoutSize = value;
				}
			}
		}

		public ArrayList InternalController
		{
			get
			{
				return m_internalController;
			}
			set
			{
				if( m_internalController != value )
				{
					m_internalController = value;
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

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SerializationConsts.INTERNAL_CONTROL, m_internalController);
			info.AddValue(SerializationConsts.RELATIONS, m_relations);
			info.AddValue(SerializationConsts.STORED_LAYOUT_SIZE, m_storedLayoutSize);
		}

		protected DockStateWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Hashtable hash = new Hashtable();
			foreach( SerializationEntry infoData in info )
			{
				hash[ infoData.Name ] = null;
			}

			m_internalController = (ArrayList)info.GetValue(SerializationConsts.INTERNAL_CONTROL, typeof(ArrayList));
			m_relations = (Hashtable)info.GetValue(SerializationConsts.RELATIONS, typeof(Hashtable));

			if( hash.Contains(SerializationConsts.STORED_LAYOUT_SIZE) )
				m_storedLayoutSize = ( Size )info.GetValue(SerializationConsts.STORED_LAYOUT_SIZE, typeof(Size));
		}

		public DockStateWrapper()
			: base()
		{
		}	
	}
	
	[Serializable]
	internal class DragSplitterControllerWrapper: LayoutControllerWrapper, ISerializable
	{
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected DragSplitterControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}		
		
		public DragSplitterControllerWrapper(): base()
		{
		}
	}
	
	[Serializable]
	internal class DockTabControllerWrapper: LayoutControllerWrapper, ISerializable
	{
		private StringCollection controls = new StringCollection();
		public StringCollection Controls
		{
			get { return controls; }
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SerializationConsts.TAB_CONTROLS, controls, typeof(StringCollection));
		}
		
		protected DockTabControllerWrapper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{	
			controls = (StringCollection)
				info.GetValue(SerializationConsts.TAB_CONTROLS, typeof(StringCollection));
		}		
		
		public DockTabControllerWrapper(): base()
		{
		
		}
	}
	
	internal class SerializationConsts
	{
		public readonly static string WRAPPER = "Wrapper";
		public readonly static string FLOAT_WRAPPER = "FloatWrapper";
		public readonly static string CHILDREN = "Children";
		public readonly static string LAYOUT = "Layout";
		public readonly static string ORIENTATION = "Orientation";
		public readonly static string STYLE = "Style";
		public readonly static string CONTROL_NAME = "ControlName";
		public readonly static string TAB_CONTROLS = "TabControls";
		public readonly static string UNIQUE_NAME = "UniqueName";
		public readonly static string INTERNAL_CONTROL = "InternalControl";
		public readonly static string RELATIONS = "Relations";
		public readonly static string MDIZORDER = "MdiZOrder";
		public readonly static string STORED_LAYOUT_SIZE = "StoredLayoutSize";
		public readonly static string PRIORITY = "Priority";
		public readonly static string TRANSIENTRECT = "TransientRect";
		public readonly static string AHONLOAD = "AhOnLoad";
	}
}
