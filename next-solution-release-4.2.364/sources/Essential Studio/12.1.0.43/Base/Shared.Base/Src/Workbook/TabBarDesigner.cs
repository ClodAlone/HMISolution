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

using Syncfusion.Diagnostics; 
namespace Syncfusion.Windows.Forms
{

	/// <summary>
	///    <para>
	///       Provides design-time functionality for the <see cref="TabBar"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para><see cref="Syncfusion.Windows.Forms.TabBarDesigner"/> provides a way to add and remove
	///    tabs at design-time, as well as tab hit testing logic at design-time.</para>
	/// </remarks>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TabBarDesigner: ControlDesigner
	{
		private int persistedSelectedIndex = 0;
		private bool propChanged = false;
		private bool tabComponentSelected = false;

		/// <override/>
		protected override bool GetHitTest(Point point)  
		{ 
			return tabComponentSelected;
		}
      
		/// <override/>
		protected override void WndProc(ref Message m)  
		{ 
			if (m.Msg == 0x84/*WM_NCHITTEST*/)
			{
				base.WndProc(ref m);

				if ((int) m.Result == -1)
					m.Result = (IntPtr) 1;
			}
			else
				base.WndProc(ref m);
		} 

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ISelectionService ss = (ISelectionService)this.GetService(typeof(ISelectionService));
				if (ss != null) 
				{
					ss.SelectionChanged -= new EventHandler(OnSelectionChanged);
				}
			}
			base.Dispose(disposing);
		}

		/// <override/>
		public override void Initialize(IComponent component)  
		{ 
			//TraceStack.TraceMethodInfo();
			TabBar tabBar = component as TabBar;
			if (tabBar == null)
			{
				throw new Exception("Error initializing TabBarDesigner. Assembly is not correctly referenced.\r\n"
					+ "Use \"/r\" option when starting devenv or \"-i\" for WinDes.");
			}
             
			base.Initialize(component);
                
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) 
				ss.SelectionChanged += new EventHandler(OnSelectionChanged);

			if (tabBar != null)
			{
				tabBar.SelectedIndexChanged += new SelectedIndexEventHandler(OnTabSelectedIndexChanged);
				tabBar.RelativeWidthChanged += new EventHandler(OnTabRelativeWidthChanged);
			}

			Control.PerformLayout();
		} 

		/// <summary>
		///      Called when the current selection changes. Here we check to
		///      see if the newly selected component is one of our tabs. If it
		///      is, we make sure that the tab is the currently visible tab.
		/// </summary>
		private void OnSelectionChanged(object sender, EventArgs e)  
		{ 
			//TraceStack.TraceMethodInfo();
			TabBar tabbar = Component as TabBar;
			ICollection collection;
			ISelectionService ss = (ISelectionService) this.GetService(typeof(ISelectionService));
			tabComponentSelected = false;

			if (ss != null)
			{
				collection = ss.GetSelectedComponents();

				foreach (object obj in collection)
				{
					// MessageBox.Show("In OnSelectionChanged: " + objects[n].ToString());
					if (obj is Control && obj != tabbar)
					{
						continue;
					}
					else
						tabComponentSelected = true;
				}
				ss.SetSelectedComponents(collection);

				propChanged = false;
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
			//TraceStack.TraceMethodInfo();
			propChanged = true;

			MemberDescriptor md = TypeDescriptor.GetProperties(Component)["RelativeWidth"];
			RaiseComponentChanged(md, null, null);
		} 

		/// <summary>
		///      Called when the selected tab changes. This accesses the design
		///      time selection service to surface the new tab as the current
		///      selection.
		/// </summary>
		private void OnTabSelectedIndexChanged(object sender, SelectedIndexEventArgs e)  
		{ 
			//TraceStack.TraceMethodInfo();
			if (propChanged)
			{
				propChanged = false;
				return;
			}

			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) 
			{
				ss.SetSelectedComponents(new object[] { Component });
			}

			int old = this.PersistedSelectedIndex;
			TabBar tabBar = Component as TabBar;
			this.PersistedSelectedIndex = tabBar.SelectedIndex;

			MemberDescriptor md = TypeDescriptor.GetProperties(Component)["SelectedIndex"];
			RaiseComponentChanged(md, old, PersistedSelectedIndex);
		} 


		// Properties

		/// <summary>
		/// <para>Accessor method for the <see cref="TabBar.SelectedIndex"/> property on
		/// <see cref="TabBar"/>.</para>
		/// </summary>
		/// <value>
		///    <para>The selected index.</para>
		/// </value>
		/// <remarks>
		///    <para>This property is shadowed at design-time.</para>
		/// </remarks>
		public int PersistedSelectedIndex 
		{
			get 
			{
				return persistedSelectedIndex;
			}
			set
			{
				persistedSelectedIndex = value;
			}
		}
	}
}
