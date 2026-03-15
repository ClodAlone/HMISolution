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
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Diagnostics;
using Syncfusion.Win32;
using Microsoft.Win32;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.ComponentModel;
using Syncfusion.Collections;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// 
	/// </summary>
	[Description( "Represents the class that manages the tabbed grouped mdi." )]
	[ToolboxBitmap( typeof( TabbedGroupedMDIManager ), "ToolboxIcons.TabbedGroup.bmp" )]
	public class TabbedGroupedMDIManager : TabbedMDIManager
	{
		#region Constants
		/// <summary>
		/// Default group name.
		/// </summary>
		private const string DEF_GROUP_NAME = "TabbedGroup";
		/// <summary>
		/// Default name format.
		/// </summary>
		private const string DEF_UNIQUE_NAME_FORMAT = "{0}{1}";
		#endregion

		#region Fields
		/// <summary>
		/// Collection of Tabbed groups.
		/// </summary>
		private TabbedGroupsCollection m_collTabbedGroups = null;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIsManualAction = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bIgnoreValidation = false;
		#endregion

		#region Static fields
		/// <summary>
		/// 
		/// </summary>
		private static int m_iUniqueID = 0;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the Tabbed groups collection.
		/// </summary>
        [Description("Gets the Tabbed groups collection.")]
		public TabbedGroupsCollection TabbedGroups
		{
			get
			{
				return m_collTabbedGroups;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
        /// Initialize new instance of TabbedGroupedMDIManager.
		/// </summary>
		public TabbedGroupedMDIManager()
		{
			m_collTabbedGroups = CreateGroupsCollection();
			InitTabbedGroupsCollection( m_collTabbedGroups );
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void TabbedGroups_CollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			if( args.Action == CollectionChangeAction.Add )
			{
				TabbedGroup newGroup = args.Element as TabbedGroup;

				Form[] mdiChildren = newGroup.MdiChildren;
				foreach( Form form in mdiChildren )
				{
					ActualizeFormAddition( form, newGroup );
				}
			}
			else if( args.Action == CollectionChangeAction.Remove )
			{
				TabbedGroup removedGroup = args.Element as TabbedGroup;

				Form[] mdiChildren = removedGroup.MdiChildren;
				foreach( Form form in mdiChildren )
				{
					ActualizeFormRemoval( form, removedGroup );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void TabbedGroups_ItemPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs args )
		{
			TabbedGroup group = sender as TabbedGroup;

			if( args.PropertyName == TabbedGroup.FormAddedPropertyName )
			{
				Form newChildForm = args.NewValue as Form;

				ActualizeFormAddition( newChildForm, group );
			}
			else if( args.PropertyName == TabbedGroup.FormRemovedPropertyName )
			{
				Form removedChildForm = args.NewValue as Form;

				ActualizeFormRemoval( removedChildForm, group );
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				base.Dispose( disposing );

				if( m_collTabbedGroups != null )
				{
					m_collTabbedGroups.CollectionChanged -= new CollectionChangeEventHandler( TabbedGroups_CollectionChanged );
					m_collTabbedGroups.ItemPropertyChanged -= new SyncfusionPropertyChangedEventHandler( TabbedGroups_ItemPropertyChanged );

					if( m_collTabbedGroups.Count > 0 )
					{
						foreach( TabbedGroup group in m_collTabbedGroups )
						{
							group.Dispose();
						}

						m_collTabbedGroups.Clear();
						m_collTabbedGroups = null;
					}
				}

				GC.SuppressFinalize( this );
			}
		}
		/// <summary>
		/// Called when tab is moved from one group to another group.
		/// </summary>
		protected override void OnActiveTabToNewGroupMoved( Form mdiChild, TabHost prevHost, TabHost newHost )
		{
			if( mdiChild == null )
				throw new ArgumentNullException( "mdiChild" );

			if( newHost == null )
				throw new ArgumentNullException( "newHost" );

			if( !m_bIsManualAction )
			{
				m_bIgnoreValidation = true;

				if( prevHost != null )
				{
					TabbedGroup group = GetGroupOfHost( prevHost );

					if( group != null )
					{
						group.RemoveForm( mdiChild );
						if( group.MdiChildren.Length == 0 )
						{
							m_collTabbedGroups.Remove( group );
						}
					}
				}

				TabbedGroup groupMovedTo = GetGroupOfHost( newHost );

				if( groupMovedTo == null )
				{
					string groupName = m_strNewGroupName == null ? GenerateUniqueGroupName() : m_strNewGroupName;
					groupMovedTo = new TabbedGroup( groupName );
					groupMovedTo.tabHost = newHost;
					m_collTabbedGroups.Add( groupMovedTo );
				}

				groupMovedTo.AddForm( mdiChild );

				m_bIgnoreValidation = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="tabHost"></param>
		internal override void SetTabHostForGroup( string groupName, TabHost tabHost )
		{
			TabbedGroup group = m_collTabbedGroups[ groupName ] as TabbedGroup;

			if( group == null )
			{
				group = new TabbedGroup( groupName );
				m_collTabbedGroups.Add( group );
			}

			group.tabHost = tabHost;
		}
		/// <summary>
		/// Called when an mdi child form gets removed.
		/// </summary>
		/// <param name="form">The Form that gets removed.</param>
		protected override void OnMdiChildRemoved( Form form )
		{
			TabbedGroup tabbedGroup = GetGroupOfForm( form );

			if( tabbedGroup != null )
			{
				tabbedGroup.RemoveForm( form, false );
			}

			base.OnMdiChildRemoved( form );
		}
		#endregion

		#region Static implementation
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static string GenerateUniqueGroupName()
		{
			return string.Format( DEF_UNIQUE_NAME_FORMAT, DEF_GROUP_NAME, ( ++m_iUniqueID ) );
		}
		#endregion

		#region Implementation
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual TabbedGroupsCollection CreateGroupsCollection()
		{
			return new TabbedGroupsCollection( this );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitTabbedGroupsCollection( TabbedGroupsCollection coll )
		{
			m_collTabbedGroups.CollectionChanged += new CollectionChangeEventHandler( TabbedGroups_CollectionChanged );
			m_collTabbedGroups.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler( TabbedGroups_ItemPropertyChanged );
		}
		/// <summary>
		/// Returns the TabbedGroup in the TabHost.
		/// </summary>
		/// <param name="host"> The group of TabHost to be found. </param>
		public TabbedGroup GetGroupOfHost( TabHost host )
		{
			if( host == null )
			{
				throw new ArgumentNullException( "host" );
			}

			TabbedGroup foundGroup = null;

			foreach( TabbedGroup group in m_collTabbedGroups )
			{
				if( group.tabHost == host || group.tabHost == null )
				{
					foundGroup = group;
					break;
				}
			}

			return foundGroup;
		}
		/// <summary>
		/// Returns the group that contains the form.
		/// </summary>
		/// <param name="form"> The form in the group. </param>
		public TabbedGroup GetGroupOfForm( Form form )
		{
			if( form == null )
			{
				throw new ArgumentNullException( "form" );
			}

			TabHost host = GetTabHostFromForm( form );

			return ( host == null ) ? null : GetGroupOfHost( host );
		}
		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		/// <returns></returns>
		public string GetUniqueGroupName()
		{
			return String.Empty;
		}
        /// <summary>
        /// Gets the unique name of the group.
        /// </summary>
        /// <returns></returns>
        public string GetUniqueGroupName(TabbedGroup group)
        {
            if (m_collTabbedGroups.Contains(group))
                return m_collTabbedGroups[group.GroupName].UniqueName;
            else
                return String.Empty;
             
        }
		/// <summary>
		/// Gets the group name of the tab host.
		/// </summary>
		/// <param name="tabHost">The tab host.</param>
		public string GetGroupNameOfHost( TabHost tabHost )
		{
			if( tabHost == null )
				throw new ArgumentNullException( "tabHost" );

			string groupName = null;

			if( m_collTabbedGroups != null && m_collTabbedGroups.Count > 0 )
			{
				foreach( TabbedGroup group in m_collTabbedGroups )
				{
					if( group.tabHost == tabHost )
					{
						groupName = group.GroupName;
						break;
					}
				}
			}

			return groupName;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ActualizeFormAddition( Form pFrmNewChild, TabbedGroup pAddedGroup )
		{
			if( m_bIgnoreValidation )
			{
				return;
			}

			m_bIsManualAction = true;

			TabbedGroup tgCurrent = GetGroupOfForm( pFrmNewChild );
			if( tgCurrent != null )
			{
				// The child is already in a group, so first remove it.
				tgCurrent.RemoveForm( pFrmNewChild ,false);
			}

			// Proceed to add the Form.
			pFrmNewChild.MdiParent = MdiParent;

			if( !IsValidTabHost( pAddedGroup.tabHost ) )
			{
				pAddedGroup.tabHost = null;
			}

			if( pAddedGroup.tabHost == null )
			{
				this.UpdateActiveTabHost( pFrmNewChild );

				// If this is the first form, then...
				EnsureUniqueGroup( pFrmNewChild );
				// Cache this new group.
				pAddedGroup.tabHost = GetTabHostFromForm( pFrmNewChild );
				// Validate the order of the new tab host.
				ValidateGroupOrder();
			}
			else
			{
				// Move the form to this group.
				pFrmNewChild.Activate();
				MoveActiveDocTo( pAddedGroup.tabHost, pFrmNewChild );				
			}

			pAddedGroup.AddForm( pFrmNewChild, false );
			pFrmNewChild.Show();

			m_bIsManualAction = false;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ActualizeFormRemoval( Form removedChildForm, TabbedGroup group )
		{
			if( m_bIgnoreValidation )
			{
				return;
			}

			removedChildForm.MdiParent = null;
			removedChildForm.Visible = false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		private void EnsureUniqueGroup( Form form )
		{
			TabHost tabHost = GetTabHostFromForm( form );
			// Check if this form is the only form in its group
			if( tabHost != null && tabHost.MDITabPanel.TabCount != 1 )
			{
				// Move it to a new group.
				form.Activate();

				// Create vertical or horizontal group here.
				if( this.Horizontal )
				{
					CreateNewHorizontalGroup();
				}
				else
				{
					CreateNewVerticalGroup();
				}
			}
		}
		/// <summary>
		/// Validates the position of TabbedGroups.
		/// </summary>
		public void ValidateGroupOrder()
		{
			ArrayListExt tabHosts = TabGroupHostsInternal;
			ArrayListExt splitterHosts = SplitterHostsInternal;

			if( tabHosts.Count == 0 )
			{
				return;
			}

			bool bOrdering = true;
			bool bMoved = false;

			// Comparing the position of the TabbedGroup in it's collection with that of the
			// position of the corresponding TabHost in it's collection. If not in the same order
			// then moving the TabHost in it's collection.
			while( bOrdering )
			{
				int i = -1;
				bool bRepeat = false;

				foreach( TabbedGroup tabbedGroup in m_collTabbedGroups )
				{
					// Validating the TabHost in the TabbedGroup before comparing it.
					if( !IsValidTabHost( tabbedGroup.tabHost ) )
					{
						tabbedGroup.tabHost = null;
					}

					// Null means there are no Forms in that group.
					if( tabbedGroup.tabHost != null )
					{
						i++;
						if( tabHosts[ i ] != tabbedGroup.tabHost )
						{
							// Order mis-match; Move tabHost;
							bMoved = true;
							bRepeat = true;

							int iCurrentIndex = tabHosts.IndexOf( tabbedGroup.tabHost );
							tabHosts.Move( iCurrentIndex, i, 1 );
							splitterHosts.Move( iCurrentIndex, i, 1 );

							break;
						}
					}
				}

				if( !bRepeat )
				{
					break;
				}
			}
			if( bMoved )
			{
				MdiClient.PerformLayout();
			}
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class TabbedGroupsCollection : ArrayListExt
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private TabbedGroupedMDIManager manager;
		/// <summary>
		/// 
		/// </summary>
		private Hashtable htGroupsByName = new Hashtable();
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public TabbedGroup this[ string groupName ]
		{
			get
			{
				if( this.htGroupsByName.Contains( groupName ) )
					return ( TabbedGroup )this.htGroupsByName[ groupName ];
				else
					return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public new TabbedGroup this[ int index ]
		{
			get
			{
				return ( TabbedGroup )base[ index ];
			}
			set
			{
				base[ index ] = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		internal TabbedGroupsCollection( TabbedGroupedMDIManager manager )
		{
			this.manager = manager;
		}
		#endregion

		#region Overrides
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void AddHandlers( object item )
		{
			if( !( item is TabbedGroup ) )
				throw new ArrayTypeMismatchException( "An object of type TabbedGroup is expected. Instead an object of type " + item.GetType().ToString() + "  is being added to the TabbedGroupsCollection." );

			TabbedGroup tg = item as TabbedGroup;

			if( this.htGroupsByName.Contains( tg.GroupName ) )
				throw new Exception( "A group with the above name is already found. Please specify a unique name." );

			this.htGroupsByName[ tg.GroupName ] = tg;
			base.AddHandlers( item );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void ReleaseHandler( object item )
		{
			TabbedGroup tabbedGroup = item as TabbedGroup;
            foreach (Form m_from in tabbedGroup.MdiChildren)
            {
                m_from.Close();
            }
			this.htGroupsByName.Remove( tabbedGroup.GroupName );
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Indicates whether the specified group name is unique.
		/// </summary>
		public bool IsUniqueName( string groupName )
		{
			return !htGroupsByName.Contains( groupName );
		}
		#endregion
	}

	/// <summary></summary>
	public class TabbedGroup :
		DisposableWithDisposedProp,
		IChangeNotifyingItem
	{
		#region Class constants
		/// <summary></summary>
		internal const string FormAddedPropertyName = "FormAdded";
		/// <summary></summary>
		internal const string FormRemovedPropertyName = "FormRemoved";
		#endregion

		#region Class members
		/// <summary></summary>
		private string groupName = String.Empty;
        /// <summary>
        /// Default group name.
        /// </summary>
        private const string DEF_GROUP_NAME = "TabbedGroup";
        /// <summary>
        /// Default name format.
        /// </summary>
        private const string DEF_UNIQUE_NAME_FORMAT = "{0}{1}";
        /// <summary>
        /// Default group unique ID.
        /// </summary>
        private static int m_iUniqueID = 0;
        /// <summary>
        /// Default group Unique name.
        /// </summary>
         private string m_UniqueName = String.Empty;
		/// <summary></summary>
		internal TabHost tabHost = null;
		/// <summary></summary>
		private Hashtable htForms = new Hashtable();
		#endregion

		#region Class properties
		/// <summary>
		/// Returns the tabGroup Name
		/// </summary>
		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}
        /// <summary>
        /// Returns the tabGroup unique Name
        /// </summary>
        public String UniqueName
        {
            get
            {
                return m_UniqueName;
            }
            
        }
		/// <summary>
		/// Returns the MDI children collection
		/// </summary>
		public Form[] MdiChildren
		{
			get
			{
				if( this.htForms.Count == 0 )
				{
					return new Form[] { };
				}

				Form[] children = new Form[ this.htForms.Count ];

				int i = -1;
				foreach( Form form in this.htForms.Keys )
				{
					i++;
					children[ i ] = form;
				}
				return children;
			}
		}
		#endregion

		#region Class events
		/// <summary>
		/// Occurs when a property gets changed
		/// </summary>
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes a new instance of the TabbedGroup class.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		public TabbedGroup( string groupName )
		{
            m_UniqueName = string.Format(DEF_UNIQUE_NAME_FORMAT, DEF_GROUP_NAME, (++m_iUniqueID));
			this.groupName = groupName;
		}

		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.htForms != null )
				{
					this.htForms.Clear();
					this.htForms = null;
				}
			}

			base.Dispose( disposing );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Adds a form to the corresponding tabgroup
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="fireEvent">if set to <c>true</c> property changed event is fired.</param>
		internal void AddForm( Form form, bool fireEvent )
		{
			if( !this.htForms.Contains( form ) )
			{
				this.htForms[ form ] = 1;

				if( fireEvent )
				{
					this.OnPropertyChanged(
						new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.None, FormAddedPropertyName, null, form ) );
				}
			}
		}

		/// <summary>
		/// Adds a form to the corresponding tabgroup
		/// </summary>
		/// <param name="form">The form.</param>
		public void AddForm( Form form )
		{
			AddForm( form, true );
		}

		/// <summary>
		/// Removes a form from the corresponding tabgroup
		/// </summary>
		/// <param name="form"/>
		public void RemoveForm( Form form )
		{
			RemoveForm( form, true );
		}
		#endregion

		#region Class utility methods
		/// <summary></summary>
		/// <param name="form"/>
		/// <param name="fireEvent"/>
		internal void RemoveForm( Form form, bool fireEvent )
		{
			if( htForms.Contains( form ) )
			{
				htForms.Remove( form );

				if( fireEvent )
				{
					OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.None, FormRemovedPropertyName, null, form ) );
				}
			}
		}

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="args"/>
		protected virtual void OnPropertyChanged( SyncfusionPropertyChangedEventArgs args )
		{
			if( this.PropertyChanged != null )
			{
				this.PropertyChanged( this, args );
			}
		}
		#endregion
	}
}