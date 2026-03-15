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
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class MessageFilterEntryHelper : IMessageFilter, IMouseHookHLProcClient,
		IKeyboardProcHookClient
	{
		// This needs to be ThreadStatic because pre-filter notifications happen on 
		// a thread-basis.
		[ThreadStatic()]
		static MessageFilterEntryHelper singleton = null;

		ArrayList filters = null;
		Hashtable m_suspend = null;
		Hashtable m_htFilters = null;

		private MouseProcHookerUtil mouseProcHooker = null;
		private KeyboardProcHooker keyboardProcHooker = null;
		private bool parsingFilters = false;
		private ArrayList filtersToRemoveAfterParse = null;
		private bool bFlag = true;
        static bool s_isDevEnv = SystemInformationExt.IsDotNetApp && !SystemInformationExt.IsDevStudio; 

		internal MessageFilterEntryHelperWeakContainer messageFilterEntryHelperWeakContainer = null;

		#region INIT
		private MessageFilterEntryHelper()
		{
			this.filters = new ArrayList();
			this.filtersToRemoveAfterParse = new ArrayList();
			this.m_htFilters = new Hashtable();
			this.m_suspend = new Hashtable();
		}
		static MessageFilterEntryHelper Singleton
		{
			get
			{
				// Create a new instance for this thread.
				if(MessageFilterEntryHelper.singleton == null)
					MessageFilterEntryHelper.singleton = new MessageFilterEntryHelper();

				return singleton;
			}
		}
		#endregion INIT

		#region FILTER_LIST_MANAGEMENT

		/// <summary>
		/// Suspends applying filters by tag.
		/// </summary>
		/// <param name="tag"></param>
		public static void Suspend( object tag )
		{
			Singleton.SuspendInternal( tag );
		}

		internal void SuspendInternal( object tag )
		{
			if( tag != null && !m_suspend.ContainsKey( tag ) )
			{
				m_suspend.Add( tag, null );
			}
		}

		/// <summary>
		/// Resumes applying filters by tag.
		/// </summary>
		/// <param name="tag"></param>
		public static void Resume( object tag )
		{
			Singleton.ResumeInternal( tag );
		}

		internal void ResumeInternal( object tag )
		{
			if( tag != null && m_suspend.ContainsKey( tag ) )
			{
				m_suspend.Remove( tag );
			}
		}

		public static void AddMessageFilter(IMessageFilter filter, bool ontop, object tag )
		{
			lock( typeof( MessageFilterEntryHelper ) )
			{
				Singleton.AddMessageFilterInternal( filter, ontop, tag );
			}
		}

		internal void AddMessageFilterInternal(IMessageFilter filter, bool ontop, object tag )
		{
			if( filter != null && !m_htFilters.ContainsKey( filter ) )
			{
				m_htFilters.Add( filter, tag );
			}

			AddMessageFilterInternal( filter, ontop );
		}

		// Sometimes filters need to be inserted on top, for highest priority.
		// This of course, will work only with filters installed using this utility class.
		// The BarManager and MainBarManagers use this utility class.
		// With menus, this logic is good enough since assumption is made that once a popup requiring
		// filtering is shown, no other popup from a different manager will be shown before
		// the current popup gets hidden.
		/// <summary>
		/// Utility to help you filter messages in both a .NET and Native app.
		/// </summary>
		/// <param name="filter">The interface that will receive messages for filtering in a .NET app.</param>
		/// <param name="ontop">Specifies whether or not this filter should be inserted on top (with highest priority).</param>
		public static void AddMessageFilter(IMessageFilter filter, bool ontop)
		{
			lock( typeof( MessageFilterEntryHelper ) )
			{
				Singleton.AddMessageFilterInternal(filter, ontop);
			}
		}
		internal void AddMessageFilterInternal(IMessageFilter filter, bool ontop)
		{
			if( filter == null && !m_htFilters.ContainsKey( filter ) )
			{
				m_htFilters.Add( filter, null );
			}

			// It's already there...
			if(filters.Contains(filter))
			{
				if( filtersToRemoveAfterParse.Contains( filter ) )
				{
					filtersToRemoveAfterParse.Remove( filter );
				}
				// Just make sure it's on top.
				if(ontop)
				{
					if(filters[0] != filter)
					{
						filters.Remove(filter);
						filters.Insert(0, filter);
					}
				}
			}
				// A new entry...
			else
			{
				if(ontop)
					filters.Insert(0, filter);
				else
					filters.Add(filter);

				this.filtersToRemoveAfterParse.Remove(filter);
			}

			// Start filtering...

			if( s_isDevEnv )
			{
				// Subscribe myself when the first filter gets added.
				if(filters.Count == 1)
				{
					// Cancel listenting to the idle message for unsubscribing.
					//Application.Idle -= new EventHandler(this.OnAppIdle);
					if(this.messageFilterEntryHelperWeakContainer != null)
						Application.Idle -= new EventHandler(this.messageFilterEntryHelperWeakContainer.AppIdleWeakEventHandler);

					Application.AddMessageFilter(this);
				}
			}
			else
			{
				// This is the first entry, so start hooking.
				if(filters.Count == 1)
				{
					this.InitHooks();
					this.mouseProcHooker.HookMessages = true;
					this.keyboardProcHooker.HookMessages = true;
                    this.ReleaseHooks();
				}
			}
		}

		public static void RemoveMessageFilter(IMessageFilter filter)
		{
			lock( typeof( MessageFilterEntryHelper ) )
			{
				Singleton.RemoveMessageFilterInternal(filter);
			}
		}
		
		internal void RemoveMessageFilterInternal(IMessageFilter filter)
		{
			if( filter != null && this.m_htFilters.ContainsKey( filter ) )
			{
				this.m_htFilters.Remove( filter );
			}

			if(this.parsingFilters)
				this.filtersToRemoveAfterParse.Add(filter);
			else
			{
				this.filters.Remove(filter);
				this.VerifyHooksRequirement();
			}
		}
		internal void OnAppIdle(object sender, EventArgs a)
		{
			Application.RemoveMessageFilter(this);
			//Application.Idle -= new EventHandler(this.OnAppIdle);
			Application.Idle -= new EventHandler(this.messageFilterEntryHelperWeakContainer.AppIdleWeakEventHandler);
		}
		private void VerifyHooksRequirement()
		{
            if (s_isDevEnv)
            {
                if (this.filters.Count == 0)
                {
                    if (bFlag)
                    {

                        if (messageFilterEntryHelperWeakContainer == null)
                            messageFilterEntryHelperWeakContainer = new MessageFilterEntryHelperWeakContainer(this);
                        // Delay until AppIdle handler to unsubscribe.
                        Application.Idle += new EventHandler(this.messageFilterEntryHelperWeakContainer.AppIdleWeakEventHandler);
                        bFlag = false;
                    }
                }
            }
            else
            {
                if (this.filters.Count == 0)
                    this.ReleaseHooks();
            }
		}
		private void OnBeforeFiltersEnumeration()
		{
			this.parsingFilters = true;
		}
		private void OnAfterFiltersEnumeration()
		{
			this.parsingFilters = false;
			if(this.filtersToRemoveAfterParse.Count > 0)
			{
				foreach(IMessageFilter filter in this.filtersToRemoveAfterParse)
					this.filters.Remove(filter);

				this.VerifyHooksRequirement();
			}
		}
		#endregion FILTER_LIST_MANAGEMENT

		#region NATIVE_APP_FILTERS
		void InitHooks()
		{
			if(this.mouseProcHooker == null)
			{
				// I will use this in case filtering is requested in the context of a Native app.
				this.mouseProcHooker = new MouseProcHookerUtil(IntPtr.Zero, (IMouseHookHLProcClient)this);
				this.keyboardProcHooker = new KeyboardProcHooker(IntPtr.Zero, this);


				// No hooking for now.
				this.mouseProcHooker.HookMessages = false;
				this.keyboardProcHooker.HookMessages = false;
			}
		}
		void ReleaseHooks()
		{
			if(this.mouseProcHooker != null)
			{
				this.mouseProcHooker.HookMessages = false;
				this.mouseProcHooker.Dispose();
				this.mouseProcHooker = null;
			}
			if(this.keyboardProcHooker != null)
			{
				this.keyboardProcHooker.HookMessages = false;
				this.keyboardProcHooker.Dispose();
				this.keyboardProcHooker = null;
			}
		}
		bool IMouseHookHLProcClient.MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
		{
			this.OnBeforeFiltersEnumeration();
			try
			{
				for( int i = 0; i < this.filters.Count; ++i )
				{
					IMouseHookHLProcClient filter = this.filters[i] as IMouseHookHLProcClient;
					
					if( null != filter 
						&& ( this.m_htFilters[filter] == null || !m_suspend.ContainsKey( this.m_htFilters[filter] ) )
						&& ((IMouseHookHLProcClient)filter).MouseHookProc(msg, point, hwnd, wHitTestCode, dwExtraInfo))
						return true;
				}
			}
			finally{this.OnAfterFiltersEnumeration();}

			return false;
		}
		bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
		{
			this.OnBeforeFiltersEnumeration();
			try
			{
				for( int i = 0; i < this.filters.Count; ++i )
				{
					IKeyboardProcHookClient filter = this.filters[i] as IKeyboardProcHookClient;

					if( null != filter
						&& ( this.m_htFilters[filter] == null || !m_suspend.ContainsKey( this.m_htFilters[filter] ) )
						&& ((IKeyboardProcHookClient)filter).KeyboardHookProc(wParam, lParam))
						return true;
				}
			}
			finally{this.OnAfterFiltersEnumeration();}
			return false;
		}
		#endregion NATIVE_APP_FILTERS

		#region DOTNETAPP_FILTERS
		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			this.OnBeforeFiltersEnumeration();
			try
			{
					IMessageFilter filter = null;
					for( int i = 0; i < filters.Count; ++i )
					{
						filter = (IMessageFilter)filters[i];
						if( this.m_htFilters[filter] == null || !m_suspend.ContainsKey( this.m_htFilters[filter] ) )
						{
							// If a filter processed it, return, else continue.
							if(filter.PreFilterMessage(ref m))
								return true;
						}
					}
			}
			finally{this.OnAfterFiltersEnumeration();}

			return false;
		}
		#endregion DOTNETAPP_FILTERS
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class PopupManager : IMessageFilter, IMouseHookHLProcClient,
		IKeyboardProcHookClient
	{
		IPopupChild curPopup = null;
		bool latestSetFocusSetting = false;
		private System.Windows.Forms.Timer setFocusTimer = null;
		private Control latestFocusControl = null;
		[ThreadStatic()]
		static PopupManager singleton = null;
        private IPopupChild previousPopUp = null;

		// Can be used as a static singleton or can also be instantiated.
		static PopupManager()
		{
		}

		static PopupManager Singleton
		{
			get
			{
				// Create a new instance for this thread.
				if(PopupManager.singleton == null)
					PopupManager.singleton = new PopupManager();

				return singleton;
			}
		}

		public PopupManager()
		{
			this.setFocusTimer = new System.Windows.Forms.Timer();
			this.setFocusTimer.Interval = 100;
			this.setFocusTimer.Tick += new EventHandler(this.SetFocusTimer_Event);
		}

		public static IPopupChild ActivePopupClient
		{
			get
			{
				return Singleton.ActivePopup;
			}
		}

		public IPopupChild ActivePopup
		{
			get
			{
				return this.curPopup;
			}
		}

		public static void SetCurrentPopupClient(IPopupChild client, bool active, bool callKillFocusOnFocused)
		{
			Singleton.SetCurrentPopup(client, active, callKillFocusOnFocused);
		}
		protected virtual void CallKillFocusOnFocused()
		{
			this.setFocusTimer.Stop();
			IntPtr focused = NativeMethods.GetFocus();
			// Ignore if the focused control is somewhere in the customization dlg.
			Control focusControl = Control.FromHandle(focused);
			if(focusControl == null)
				focusControl = PopupUtils.GetADotNetParentControl(focused);
			if(focusControl != null
				&& !(focusControl is IDontCallKillFocus)
				&& !(FindFormHelper.FindForm(focusControl) is IDontCallKillFocus))
			{
				this.latestFocusControl = focusControl;
				NativeMethods.SendMessage(focused, NativeMethods.WM_KILLFOCUS, 0, 0);
			}
		}

		protected virtual void CallSetFocusOnFocused()
		{
			this.setFocusTimer.Start();
		}

		// Delayed set focus.
		private void SetFocusTimer_Event(object sender, EventArgs e)
		{
			this.setFocusTimer.Stop();
			IntPtr focused = NativeMethods.GetFocus();
			// Ignore if the focused control is somewhere in the customization dlg.
			Control focusControl = Control.FromHandle(focused);
			if(focusControl == null)
				focusControl = PopupUtils.GetADotNetParentControl(focused);
			if(focusControl == null && this.latestFocusControl != null && this.latestFocusControl.IsHandleCreated && this.latestFocusControl.Visible)
				focusControl = this.latestFocusControl;
			if(focusControl != null && !(FindFormHelper.FindForm(focusControl) is IDontCallSetFocus))
			{
				while(focusControl != null && focusControl is IDelegateFocusToPrevWindow
					&& ((IDelegateFocusToPrevWindow)focusControl).PreviousWindow != null)
					focusControl = ((IDelegateFocusToPrevWindow)focusControl).PreviousWindow;

				if(!focusControl.Focused)
					focusControl.Focus();
				else
				{
					if(focusControl != null)
						focused = focusControl.Handle;

					NativeMethods.SendMessage(focused, NativeMethods.WM_SETFOCUS, 0, 0);
				}
			}
		}

		private bool IsAncestor(IPopupChild popup, IPopupParent parent)
		{
			if(parent == null)
				return false;
			while(popup.PopupParent != null)
			{
				if(parent == popup.PopupParent)
					return true;
				else if(popup.PopupParent is IPopupChild)
					popup = popup.PopupParent as IPopupChild;
				else
					break;
			}
			return false;
		}

		public void SetCurrentPopup(IPopupChild popup, bool active, bool callKillFocusOnFocused)
		{
            bool hideCurrentPopup = true;

            if ((popup is ToolTipAdv || popup is ToolTip) && !(this.curPopup is ToolTipAdv || this.curPopup is ToolTip))
            {
                hideCurrentPopup = false; //Do not hide the active pop up if the new pop up is a Tooltip
                if(this.previousPopUp != this.curPopup)
                    this.previousPopUp = this.curPopup;
            }

			// Hide any Active Popup, if not the current popup's parent.
			if(active && this.curPopup != null && !this.IsAncestor(popup, this.curPopup as IPopupParent)
				&& this.curPopup != popup && this.curPopup.IsShowing() && hideCurrentPopup)
				this.curPopup.HidePopup(PopupCloseType.Deactivated);

			// Set the new active popup.
			if(active)
			{
				MessageFilterEntryHelper.AddMessageFilter(this, true);
				this.curPopup = popup;
				if(callKillFocusOnFocused)
					this.CallKillFocusOnFocused();
				this.latestSetFocusSetting |= callKillFocusOnFocused;
			}
			else if(this.curPopup == popup)
			{
                if ((popup is ToolTipAdv || popup is ToolTip) && this.previousPopUp != null)
                {
                    this.SetCurrentPopup(this.previousPopUp, true, false);
                    this.previousPopUp = null;
                }
                else
                {
                    this.curPopup = null;
                    if (this.latestSetFocusSetting == true)
                    {
                        this.latestSetFocusSetting = false;
                        this.CallSetFocusOnFocused();
                    }
                    MessageFilterEntryHelper.RemoveMessageFilter(this);
                }
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		public virtual bool PreFilterMessage(ref Message m) 
		{
			if(this.curPopup == null)
				return false;
			
			int msg = m.Msg;

			// Send Keyboard and Mouse messages to active menus and bars.
			bool processed = false;
			
			// Keeping the try / catch within the if blocks to reduce performance hit.
			if((msg >= 0x200 && msg <= 0x020D || msg == NativeMethods.WM_MOUSEWHEEL )	//WM_MOUSEFIRST to WM_MOUSELAST?
				//|| m.Msg == 0x2A1 || m.Msg == 0x2A3
				|| (msg >= 0x00A0 && msg <= 0x00AD))//WM_NCMOUSEMOVE to WM_NCMBUTTONDBLCLK?
			{
				try
				{
					processed = this.curPopup.MouseMessage(ref m);
				}
				catch(Exception ex)
				{
					Application.OnThreadException(ex);
				}
			}
			else if(msg >= 0x100 && msg <= 0x0108 )//WM_KEYFIRST to WM_KEYLAST
			{
				try
				{
					processed = this.curPopup.KeyboardMessage(ref m);
				}
				catch(Exception ex)
				{
					Application.OnThreadException(ex);
				}
			}

			return processed;
		}
		bool IMouseHookHLProcClient.MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
		{
			if(this.curPopup == null)
				return false;
			
			IMouseHookHLProcClient mhpc = this.curPopup as IMouseHookHLProcClient;

			if( null == mhpc )
				return false;

			return mhpc.MouseHookProc(msg, point, hwnd, wHitTestCode, dwExtraInfo);
		}
		bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
		{
			if(this.curPopup == null)
				return false;
			
			IKeyboardProcHookClient kbpc = this.curPopup as IKeyboardProcHookClient;
			return kbpc.KeyboardHookProc(wParam, lParam);
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class PopupUtils
	{
		public static Control GetADotNetParentControl(IntPtr handle)
		{
			Control parent = null;

			while(handle != IntPtr.Zero)
			{
				handle = NativeMethods.GetParent(handle);
				if(handle != IntPtr.Zero)
				{
					parent = Control.FromHandle(handle);
					if(parent != null)
						break;
				}
			}

			return parent;
		}

		public static Point ComputeDefaultTopBottomAlignment
			(PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment,
			Rectangle parentBounds, bool isRTL)
		{
			Point location = Point.Empty;
			if(!isRTL)
			{
				switch(prevAlignment)
				{
					case PopupRelativeAlignment.Default:
						// 1st Preference
						newAlignment = PopupRelativeAlignment.BottomLeft;
						location = new Point(parentBounds.Left, parentBounds.Bottom);
						break;
					case PopupRelativeAlignment.BottomLeft:
						// 2nd Preference
						newAlignment = PopupRelativeAlignment.BottomRight;
						location = new Point(parentBounds.Right - 1, parentBounds.Bottom);
						break;
					case PopupRelativeAlignment.BottomRight:
						// 3rd Preference
						newAlignment = PopupRelativeAlignment.TopLeft;
						location = new Point(parentBounds.Left, parentBounds.Top - 1);
						break;
					case PopupRelativeAlignment.TopLeft:
						// 4th Preference
						newAlignment = PopupRelativeAlignment.TopRight;
						location = new Point(parentBounds.Right - 1, parentBounds.Top - 1);
						break;
					case PopupRelativeAlignment.TopRight:
					default:
						// Default preference
						newAlignment = PopupRelativeAlignment.Default;
						location = new Point(parentBounds.Left, parentBounds.Bottom);
						break;
				}
			}
			else
			{
				switch(prevAlignment)
				{
					case PopupRelativeAlignment.Default:
						// 1st Preference
						newAlignment = PopupRelativeAlignment.BottomRight;
						location = new Point(parentBounds.Right - 1, parentBounds.Bottom);
						break;

					case PopupRelativeAlignment.BottomRight:
						// 2nd Preference
						newAlignment = PopupRelativeAlignment.BottomLeft;
						location = new Point(parentBounds.Left, parentBounds.Bottom);
						break;
					
					case PopupRelativeAlignment.BottomLeft:
						// 3rd Preference
						newAlignment = PopupRelativeAlignment.TopRight;
						location = new Point(parentBounds.Right - 1, parentBounds.Top - 1);
						break;

					case PopupRelativeAlignment.TopRight:
						// 4th Preference
						newAlignment = PopupRelativeAlignment.TopLeft;
						location = new Point(parentBounds.Left, parentBounds.Top - 1);
						break;

					case PopupRelativeAlignment.TopLeft:
					default:
						// Default preference
						newAlignment = PopupRelativeAlignment.Default;
						location = new Point(parentBounds.Left, parentBounds.Bottom);
						break;
				}
			}
			return location;
		}

        public static Point ComputeDefaultTopBottomAlignmentExt
            (PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment,
            Control parent, Rectangle parentBounds, bool isRTL)
        {
            bool RTLLayout = false;
            Point location = Point.Empty;

            if(parent.Parent != null)
            {
                Type parentType = parent.Parent.GetType();
                PropertyInfo pInfo = parentType.GetProperty("RightToLeftLayout", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                if (pInfo != null)
                {
                    RTLLayout = (bool)pInfo.GetValue(parent.Parent, null);
                }
            }

            if (!RTLLayout || !isRTL)
            {
                return ComputeDefaultTopBottomAlignment(prevAlignment, out newAlignment, parentBounds, isRTL);
            }
            else
            {
                switch (prevAlignment)
                {
                    case PopupRelativeAlignment.Default:
                        // 1st Preference
                        newAlignment = PopupRelativeAlignment.BottomRight;
                        location = new Point(parentBounds.Left + 1, parentBounds.Bottom);
                        break;

                    case PopupRelativeAlignment.BottomRight:
                        // 2nd Preference
                        newAlignment = PopupRelativeAlignment.BottomLeft;
                        location = new Point(parentBounds.Right, parentBounds.Bottom);
                        break;

                    case PopupRelativeAlignment.BottomLeft:
                        // 3rd Preference
                        newAlignment = PopupRelativeAlignment.TopRight;
                        location = new Point(parentBounds.Left + 1, parentBounds.Top - 1);
                        break;

                    case PopupRelativeAlignment.TopRight:
                        // 4th Preference
                        newAlignment = PopupRelativeAlignment.TopLeft;
                        location = new Point(parentBounds.Right, parentBounds.Top - 1);
                        break;

                    case PopupRelativeAlignment.TopLeft:
                    default:
                        // Default preference
                        newAlignment = PopupRelativeAlignment.Default;
                        location = new Point(parentBounds.Right, parentBounds.Bottom);
                        break;
                }
            }
            
            return location;
        }

        public static Point ComputeDefaultLeftRightAlignment
			(PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment,
			Rectangle parentBounds, bool isRTL)
		{
			Point location = Point.Empty;
			if(!isRTL)
			{
				switch(prevAlignment)
				{
					case PopupRelativeAlignment.Default:
						// 1st Preference
						newAlignment = PopupRelativeAlignment.RightTop;
						location = new Point(parentBounds.Right, parentBounds.Top);
						break;
					case PopupRelativeAlignment.RightTop:
						// 2nd Preference
						newAlignment = PopupRelativeAlignment.RightBottom;
						location = new Point(parentBounds.Right, parentBounds.Bottom - 1);
						break;
					case PopupRelativeAlignment.RightBottom:
						// 3rd Preference
						newAlignment = PopupRelativeAlignment.LeftTop;
						location = new Point(parentBounds.Left - 1, parentBounds.Top);
						break;
					case PopupRelativeAlignment.LeftTop:
						// 4th Preference
						newAlignment = PopupRelativeAlignment.LeftBottom;
						location = new Point(parentBounds.Left - 1, parentBounds.Bottom - 1);
						break;
					case PopupRelativeAlignment.LeftBottom:
					default:
						// Default preference
						newAlignment = PopupRelativeAlignment.Default;
						location = new Point(parentBounds.Right, parentBounds.Top);
						break;
				}
			}
			else
			{
				switch(prevAlignment)
				{
					case PopupRelativeAlignment.Default:
						// 1st Preference
						newAlignment = PopupRelativeAlignment.LeftTop;
						location = new Point(parentBounds.Left - 1, parentBounds.Top);
						break;
					case PopupRelativeAlignment.LeftTop:
						// 2nd Preference
						newAlignment = PopupRelativeAlignment.LeftBottom;
						location = new Point(parentBounds.Left - 1, parentBounds.Bottom - 1);
						break;
					case PopupRelativeAlignment.LeftBottom:
						// 3rd Preference
						newAlignment = PopupRelativeAlignment.RightTop;
						location = new Point(parentBounds.Right, parentBounds.Top);
						break;
					case PopupRelativeAlignment.RightTop:
						// 4th Preference
						newAlignment = PopupRelativeAlignment.RightBottom;
						location = new Point(parentBounds.Right, parentBounds.Bottom - 1);
						break;					
					case PopupRelativeAlignment.RightBottom:
					default:
						// Default preference
						newAlignment = PopupRelativeAlignment.Default;
						location = new Point(parentBounds.Left - 1, parentBounds.Top);
						break;
				}
			}
			return location;
		}

        public static Point ComputeDefaultLeftRightAlignmentExt
            (PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment,
            Control parent, Rectangle parentBounds, bool isRTL)
        {
            bool RTLLayout = false;
            Point location = Point.Empty;

            if (parent.Parent != null)
            {
                Type parentType = parent.Parent.GetType();
                PropertyInfo pInfo = parentType.GetProperty("RightToLeftLayout", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                if (pInfo != null)
                {
                    RTLLayout = (bool)pInfo.GetValue(parent.Parent, null);
                }
            }

            if (!RTLLayout || !isRTL)
            {
                return ComputeDefaultLeftRightAlignment(prevAlignment, out newAlignment, parentBounds, isRTL);
            }
            else
            {
                switch (prevAlignment)
                {
                    case PopupRelativeAlignment.Default:
                        // 1st Preference
                        newAlignment = PopupRelativeAlignment.LeftTop;
                        location = new Point(parentBounds.Right, parentBounds.Top);
                        break;
                    case PopupRelativeAlignment.LeftTop:
                        // 2nd Preference
                        newAlignment = PopupRelativeAlignment.LeftBottom;
                        location = new Point(parentBounds.Right, parentBounds.Bottom - 1);
                        break;
                    case PopupRelativeAlignment.LeftBottom:
                        // 3rd Preference
                        newAlignment = PopupRelativeAlignment.RightTop;
                        location = new Point(parentBounds.Left + 1, parentBounds.Top);
                        break;
                    case PopupRelativeAlignment.RightTop:
                        // 4th Preference
                        newAlignment = PopupRelativeAlignment.RightBottom;
                        location = new Point(parentBounds.Left + 1, parentBounds.Bottom - 1);
                        break;
                    case PopupRelativeAlignment.RightBottom:
                    default:
                        // Default preference
                        newAlignment = PopupRelativeAlignment.Default;
                        location = new Point(parentBounds.Right, parentBounds.Top);
                        break;
                }
            }
            return location;
        }

		public static Point ComputeLocationFromAlignment(PopupRelativeAlignment align, Rectangle dropDownBounds)
		{
			Point loc = Point.Empty;
			switch(align)
			{
				case PopupRelativeAlignment.BottomLeft:
				case PopupRelativeAlignment.BottomRight:
				case PopupRelativeAlignment.LeftBottom:
				case PopupRelativeAlignment.RightBottom:
				case PopupRelativeAlignment.Default:
					loc.Y = (int)dropDownBounds.Bottom - 1;
					break;
				default:
					loc.Y = (int)dropDownBounds.Top;
					break;
			};
			switch(align)
			{
				case PopupRelativeAlignment.BottomLeft:
				case PopupRelativeAlignment.Default:
				case PopupRelativeAlignment.LeftBottom:
				case PopupRelativeAlignment.LeftTop:
				case PopupRelativeAlignment.TopLeft:
					loc.X = (int)dropDownBounds.Left;
					break;
				default:
					loc.X = (int)dropDownBounds.Right - 1;
					break;
			};
			return loc;
		}
		public static Point ComputeDefaultPopupAlignment
			(PopupRelativeAlignment prevAlign, out PopupRelativeAlignment newAlign,
			PopupRelativeAlignment firstPrefAlignment, PopupRelativeAlignment lastPrefAlignment,
			Rectangle parentBounds)
		{
			Point pos = Point.Empty;

			switch(prevAlign)
			{
				case PopupRelativeAlignment.Default:
					// 1st preference
					newAlign = firstPrefAlignment;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.BottomLeft:
					newAlign = PopupRelativeAlignment.BottomRight;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.BottomRight:
					newAlign = PopupRelativeAlignment.RightBottom;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.RightBottom:
					newAlign = PopupRelativeAlignment.RightTop;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.RightTop:
					newAlign = PopupRelativeAlignment.TopRight;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.TopRight:
					newAlign = PopupRelativeAlignment.TopLeft;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.TopLeft:
					newAlign = PopupRelativeAlignment.LeftTop;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.LeftTop:
					newAlign = PopupRelativeAlignment.LeftBottom;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				case PopupRelativeAlignment.LeftBottom:
					newAlign = PopupRelativeAlignment.BottomLeft;
					if(newAlign == firstPrefAlignment)goto default;
					pos = ComputeLocationFromAlignment(newAlign, parentBounds);
					break;
				default:
					newAlign = PopupRelativeAlignment.Default;
					pos = ComputeLocationFromAlignment(lastPrefAlignment, parentBounds);
					break;
			}
			return pos;
		}
		public static Point[] ComputeDefaultBorderOverlapCue(PopupRelativeAlignment rAlign,
			Rectangle dropDownBounds)
		{
			if(rAlign == PopupRelativeAlignment.Default)
				return null;

			Point[] overlapLines = new Point[2];
			Point posStart = Point.Empty, posEnd = Point.Empty;

			// Set the starting pos.
			switch(rAlign)
			{
				case PopupRelativeAlignment.BottomLeft:
				case PopupRelativeAlignment.BottomRight:
					posStart = new Point((int)dropDownBounds.Left + 1, (int)dropDownBounds.Bottom - 1);
					break;
				case PopupRelativeAlignment.TopLeft:
				case PopupRelativeAlignment.TopRight:
					posStart = new Point((int)dropDownBounds.Left + 1, (int)dropDownBounds.Top);
					break;
				case PopupRelativeAlignment.LeftBottom:
				case PopupRelativeAlignment.LeftTop:
					posStart = new Point((int)dropDownBounds.Left, (int)dropDownBounds.Top + 1);
					break;
				case PopupRelativeAlignment.RightBottom:
				case PopupRelativeAlignment.RightTop:
					posStart = new Point((int)dropDownBounds.Right - 1, (int)dropDownBounds.Top + 1);
					break;
			}

			overlapLines[0] = posStart;

			// Set the ending pos.
			switch(rAlign)
			{
				case PopupRelativeAlignment.BottomLeft:
				case PopupRelativeAlignment.BottomRight:
				case PopupRelativeAlignment.TopLeft:
				case PopupRelativeAlignment.TopRight:
					overlapLines[1] = 
						new Point(overlapLines[0].X + (int)dropDownBounds.Width - 3, overlapLines[0].Y);
					break;
				default:
					overlapLines[1] = 
						new Point(overlapLines[0].X , overlapLines[0].Y + (int)dropDownBounds.Height - 3);
					break;
			}

			return overlapLines;
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDontCallKillFocus
	{
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDontCallSetFocus
	{
	}
}
