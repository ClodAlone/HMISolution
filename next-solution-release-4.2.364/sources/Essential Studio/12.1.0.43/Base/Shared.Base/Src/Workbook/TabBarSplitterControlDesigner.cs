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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///    <para>
	///       Provides design-time functionality for the <see cref="TabBarSplitterControl"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para><see cref="TabBarSplitterControlDesigner"/> provides a way to add and remove
	///    tabs at design-time, as well as tab hit testing logic at design-time.</para>
	/// </remarks>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabBarSplitterControlDesigner : ParentControlDesigner
	{
		// Fields
		private static string[] nonBrowse = new string[] { "Click" };

		private bool disableDrawGrid = false;
		private int persistedActivePage = 0;
		private bool m_bPropChanged = false;
		private DesignerVerb removeVerb = null;
		private bool m_bTabComponentSelected = false;
		private DesignerVerbCollection verbs = null;

		// Constructors
		/// <summary>
		/// Initializes a new TabBarSplitterControlDesigner.
		/// </summary>
		public TabBarSplitterControlDesigner()  
		{ 
		}
 
		// Methods

		/// <summary>
		///    <para>
		///       Indicates whether the
		///       specified point was within the bounds of the component.</para>
		/// </summary>
		protected override bool GetHitTest(Point point)  
		{ 
			return m_bTabComponentSelected;
		}
      
		//[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]

		/// <override/>
		protected override void WndProc(ref Message m)  
		{ 
			if (m.Msg == NativeMethods.WM_NCHITTEST)
			{
				base.WndProc(ref m);

				if ((int) m.Result == -1)
					m.Result = (IntPtr) 1;
			}
			else
				base.WndProc(ref m);
		} 

		/// <summary>
		///     Given a component, this retrieves the tab page that it is parented to or
		///     NULL if it is not parented to any tab page.
		/// </summary>
		/// <param name='comp'>
		///     The component to check.
		/// </param>
		/// <returns>
		///     A TabPage that the component is parented to or NULL if
		///     no such page exists.  This will return the component if it
		///     is an instance of TabPage.
		/// </returns>
		internal static TabBarPage GetTabBarPageOfComponent( object comp )
		{
			TabBarPage tabBarPage = null;
			Control c = comp as Control;

			while( c != null )
			{
				tabBarPage = c as TabBarPage;

				if( null != tabBarPage )
				{
					break;
				}

				c = c.Parent;
			}

			return tabBarPage;
		}

		private void CheckVerbStatus(object sender, ComponentChangedEventArgs e)  
		{ 
			TabBarSplitterControl tc = Component as TabBarSplitterControl;
			if (removeVerb != null && tc != null)
			{
				removeVerb.Enabled = tc.Bar.TabCount > 0;
			}
		} 


		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
				if (ss != null) 
				{
					ss.SelectionChanged -= new EventHandler(OnSelectionChanged);
				}

				IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				if (cs != null)
				{
					cs.ComponentChanged -= new ComponentChangedEventHandler(CheckVerbStatus);
				}
				
				TabBarSplitterControl tc = this.Component as TabBarSplitterControl;
				if (tc != null)
				{
					tc.ActivePageChanged -= new ControlEventHandler(OnTabActivePageChanged);
					tc.Bar.RelativeWidthChanged -= new EventHandler(OnTabRelativeWidthChanged);
					tc.Click -= new EventHandler(OnControlClick);
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		///    <para>Initializes the designer using the specified component.</para>
		/// </summary>
		/// <param name="component">The component to associate this designer with. This must always be an instance of the control.</param>
		/// <seealso cref="System.ComponentModel.Design.IDesigner"/>
		public override void Initialize(IComponent component)  
		{ 
			TabBarSplitterControl tc = component as TabBarSplitterControl;
			if (tc == null)
			{
				throw new Exception("Error initializing TabBarSplitterControlDesigner. Assembly is not correctly referenced.");
			}
             
			base.Initialize(component);
                
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) 
			{
				ss.SelectionChanged += new EventHandler(OnSelectionChanged);
			}

			IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (cs != null)
			{
				cs.ComponentChanged += new ComponentChangedEventHandler(CheckVerbStatus);
			}
			
			if (tc != null)
			{
				tc.ActivePageChanged += new ControlEventHandler(OnTabActivePageChanged);
				tc.Bar.RelativeWidthChanged += new EventHandler(OnTabRelativeWidthChanged);
				tc.Click -= new EventHandler(OnControlClick);
			}

			Control.PerformLayout();
		} 

		void OnControlClick(object sender, EventArgs e)
		{
			ISelectionService ss = (ISelectionService) this.GetService(typeof(ISelectionService));
			if (ss != null)
				ss.SetSelectedComponents(new object[] { sender } );
		}

		/// <summary>
		///      Called in response to a verb to add a tab. This adds a new
		///      tab with a default name.
		/// </summary>
		private void OnAdd(object sender, EventArgs eevent)  
		{ 
			TabBarSplitterControl tc = Component as TabBarSplitterControl;
			Control tcontrol = (Control) Control;
			MemberDescriptor md = TypeDescriptor.GetProperties(tc)["Controls"];
			IDesignerHost dh = GetService(typeof(IDesignerHost)) as IDesignerHost;
			DesignerTransaction dt;
			if (dh != null) 
			{
				dt = dh.CreateTransaction("Add tab to " + Component.Site.Name);
				try
				{
					try
					{
						RaiseComponentChanging(md);
					}
					catch (CheckoutException ce)
					{
						if (ce == CheckoutException.Canceled)
							return;

						throw;
					}  
					TabBarPage page = (TabBarPage) dh.CreateComponent(typeof(TabBarPage));
					String s = null;
					PropertyDescriptor pd = TypeDescriptor.GetProperties(page)["Name"];
					if (pd != null)
					{
						if (pd.PropertyType == typeof(String))
						{
							s = pd.GetValue(page) as String;
							if (s != null)
							{
								page.Text = s;
								page.Text = s;
							}
							
							tcontrol.Controls.Add(page);
							RaiseComponentChanged(md, null, null);
							tc.Bar.SelectedItem = page;
						}
					}
				} 
				finally
				{
					dt.Commit();
				} 
			}
		} 
		
		/// <summary>
		///      This is called in response to a verb to remove a tab. It removes
		///      the current tab.
		/// </summary>
		private void OnRemove(object sender, EventArgs eevent)  
		{ 
			TabBarSplitterControl tc = Component as TabBarSplitterControl;
			if (tc != null)
			{
				int tabCount = tc.Bar.TabCount;
				if (tabCount == 0)
					return;

				MemberDescriptor md = TypeDescriptor.GetProperties(tc)["Controls"];
				TabBarPage page = tc.ActivePage;

				if (page != null)
				{
					IDesignerHost dh = GetService(typeof(IDesignerHost)) as IDesignerHost;
					DesignerTransaction dt;
					if (dh != null) 
					{
						dt = dh.CreateTransaction(String.Concat(new String[] 
							{ "Remove", page.Site.Name, "from", Component.Site.Name }));
						try
						{
							try
							{
								RaiseComponentChanging(md);
							}
							catch (CheckoutException ce)
							{
								if (ce == CheckoutException.Canceled)
									return;

								throw;
							}  

							dh.DestroyComponent(page);
							RaiseComponentChanged(md, null, null);
						}
						finally
						{
							dt.Commit();
						} 
					}
				}
			}
		}

		/// <summary>
		///      Called when the current selection changes. Here we check to
		///      see if the newly selected component is one of our tabs. If it
		///      is, we make sure that the tab is the currently visible tab.
		/// </summary>
		private void OnSelectionChanged( object sender, EventArgs e )
		{
			m_bTabComponentSelected = false;

			ISelectionService ss = (ISelectionService)this.GetService( typeof( ISelectionService ) );

			if( ss != null )
			{
				ICollection collection = ss.GetSelectedComponents();
				TabBarSplitterControl tabbar = Component as TabBarSplitterControl;

				foreach( object obj in collection )
				{
					if( obj is Control && obj != tabbar )
					{
						TabBarPage page = GetTabBarPageOfComponent( obj );

						if( null != page && page.Parent == tabbar && page.IsActiveControl )
						{
							tabbar.Bar.SelectedItem = page;
							break;
						}
					}
					else
					{
						m_bTabComponentSelected = true;
					}
				}

				m_bPropChanged = false;
			}
		}
           
		/// <summary>
		///      Called when the tab property changes. We just set a flag here to
		///      indicate to the later index changed event that the change was the
		///      result of a property change, so there is no need to update the
		///      selection.
		/// </summary>
		private void OnTabRelativeWidthChanged(object sender, EventArgs e)  
		{
			m_bPropChanged = true;

			MemberDescriptor md = TypeDescriptor.GetProperties(Component)["RelativeWidth"];
			RaiseComponentChanged(md, null, null);
		} 

		/// <summary>
		/// Called when the selected tab changes. This accesses the design
		/// time selection service to surface the new tab as the current
		/// selection.
		/// </summary>
		private void OnTabActivePageChanged(object sender, ControlEventArgs e)
		{
			if( m_bPropChanged )
			{
				m_bPropChanged = false;
				return;
			}

			int old = this.PersistedActivePageIndex;
			TabBarSplitterControl tc = Component as TabBarSplitterControl;

			this.PersistedActivePageIndex = tc.ActivePageIndex;

			MemberDescriptor md = TypeDescriptor.GetProperties( Component )["ActivePageIndex"];
			RaiseComponentChanged( md, old, PersistedActivePageIndex );

			ISelectionService ss = (ISelectionService)GetService( typeof( ISelectionService ) );

			if( ss != null && tc.ActivePage != null )
			{
				ss.SetSelectedComponents( new object[] { tc.ActivePage } );
			}
		} 


		// Properties

		/// <summary>
		/// Gets / sets the persisted active page index.
		/// </summary>
		public int PersistedActivePageIndex
		{
			get 
			{
				return persistedActivePage;
			}
			set
			{
				persistedActivePage = value;
			}
		}
		/// <summary>
		///    <para>Indicates whether to draw a grid for the control.</para>
		/// </summary>
		/// <value>
		/// <para><see langword='True'/> if a grid should be drawn; 
		/// <see langword='False'/> otherwise.</para>
		/// </value>
		protected override bool DrawGrid 
		{ 
			get 
			{ 
				if (disableDrawGrid)
					return false;
				
				return base.DrawGrid;
			} 
		}

		/// <summary>
		///    <para>
		///       Gets / sets the design-time verbs supported by the component associated with the designer.</para>
		/// </summary>
		/// <value>
		/// <para>An array of <see cref="System.ComponentModel.Design.DesignerVerb"/> objects.</para>
		/// </value>
		/// <remarks>
		///    <para>The verbs returned by this method are typically displayed in a right-click
		///       menu by the design-time environment. The return value may be NULL if the
		///       component has no design-time verbs. When a user selects one of the verbs, the
		///       performVerb() method is invoked with the the corresponding DesignerVerb object.
		///       NOTE: A design-time environment will typically provide a "Properties..." entry
		///       on a component's right-click menu. The getVerbs() method should therefore not
		///       include such an entry in the returned list of verbs.</para>
		/// </remarks>
		public override DesignerVerbCollection Verbs 
		{
			get 
			{ 
				if (verbs == null) 
				{
					removeVerb = new DesignerVerb(SR.GetString("TabBarSplitterControlRemove"), new EventHandler(OnRemove));
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb(SR.GetString("TabBarSplitterControlAdd"), new EventHandler(OnAdd)));
					verbs.Add(removeVerb);
				}

				removeVerb.Enabled = Control.Controls.Count > 0;					

				return verbs;
			}
		}	
	}
}
