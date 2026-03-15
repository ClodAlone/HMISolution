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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Threading;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	#region ICustomItem
	/// <summary>
	/// 
	/// </summary>
	public interface ICustomItem
	{
		ToolStrip Owner { get; }
		ToolStripItemPlacement Placement { get; }
	}
	#endregion

	#region ToolStripItemsToolTipService
	class ToolStripItemsToolTipService : Hashtable, IToolTipService
	{
		#region *** ToolStripData
		class ToolStripData : ArrayList, INativeMessageFilter, IDisposable
		{
			#region MSG
			[StructLayout(LayoutKind.Sequential)]
			struct MSG
			{
				public IntPtr hwnd;
				public int message;
				public int wParam;
				public int lParam;
				public int time;
				public POINT pt;
			}
			#endregion

			#region Constructors
			public ToolStripData(ToolStrip tsMain, ToolStripItemsToolTipService owner)
			{
				m_owner = owner;

				m_mhMain = new NativeMessageHandler();
				m_mhOverflow = new NativeMessageHandler();

				if (tsMain != null)
				{
					m_tsMain = tsMain;
					m_tsMain.HandleCreated += new EventHandler(OnMainHandleCreated);
					m_tsMain.HandleDestroyed += new EventHandler(OnMainHandleDestroyed);
					m_tsMain.LayoutCompleted += new EventHandler(OnMainLayoutCompleted);

					m_tsOverflow = tsMain.OverflowButton.DropDown;
					m_tsOverflow.HandleCreated += new EventHandler(OnOverflowHandleCreated);
					m_tsOverflow.LayoutCompleted += new EventHandler(OnOverflowLayoutCompleted);

					if (m_tsMain.IsHandleCreated)
					{
						OnMainHandleCreated(m_tsMain, EventArgs.Empty);
					}
					if (m_tsOverflow.IsHandleCreated)
					{
						OnOverflowHandleCreated(m_tsOverflow, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region INativeMessageFilter implementation
			bool INativeMessageFilter.ProcessMessage(ref Message m)
			{
				bool bResult = false;

				switch ((Msg)m.Msg)
				{
					case Msg.WM_LBUTTONDOWN:
					case Msg.WM_MOUSEMOVE:
					case Msg.WM_LBUTTONUP:
					case Msg.WM_RBUTTONDOWN:
					case Msg.WM_MBUTTONDOWN:
					case Msg.WM_RBUTTONUP:
					case Msg.WM_MBUTTONUP:
                        if (!(m_tsMain.TopLevelControl is Form) || (m_tsMain.TopLevelControl is Form && m_tsMain.TopLevelControl.ContainsFocus))
                        {
                            bResult = m_owner.OnMouseMessage(ref m);
                        }
						break;
					case Msg.WM_NOTIFY:
						bResult = m_owner.OnNotify(ref m, this);
						break;
				}

				return bResult;
			}
			#endregion

			#region IDisposable implementation
			public virtual void Dispose()
			{
				Clear();

				if (m_mhMain != null)
				{
					m_mhMain.MessageFilter = null;
					m_mhMain = null;
				}
				if (m_mhOverflow != null)
				{
					m_mhOverflow.MessageFilter = null;
					m_mhOverflow = null;
				}
				if (m_tsMain != null)
				{
					m_tsMain.HandleCreated -= new EventHandler(OnMainHandleCreated);
					m_tsMain.HandleDestroyed -= new EventHandler(OnMainHandleDestroyed);
					m_tsMain.LayoutCompleted -= new EventHandler(OnMainLayoutCompleted);
					m_tsMain = null;
				}
				if (m_tsOverflow != null)
				{
					m_tsOverflow.HandleCreated -= new EventHandler(OnOverflowHandleCreated);
					m_tsOverflow.LayoutCompleted -= new EventHandler(OnOverflowLayoutCompleted);
					m_tsOverflow = null;
				}
			}
			#endregion

			#region Overrides
			
			public override int Add(object obj)
			{
				int index = base.Add(obj);

				OnItemAdded(index, obj as ToolStripItem);

				return index;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			public override void RemoveAt(int index)
			{
				if (index >= 0 && index < this.Count)
				{
					OnItemRemoving(index, this[index] as ToolStripItem);

					base.RemoveAt(index);

					OnItemRemoved(index);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public override void Clear()
			{
				for (int i = 0, count = this.Count; i < count; i++)
				{
					OnItemRemoving(i, this[i] as ToolStripItem);
				}
				base.Clear();
			}
			#endregion

			#region Properties
			public override object this[int index]
			{
				get
				{
					if (index >= 0 && index < this.Count)
					{
						return base[index];
					}
					return null;
				}
				set
				{
					base[index] = value;
				}
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="id"></param>
			/// <param name="item"></param>
			void OnItemAdded(int id, ToolStripItem item)
			{
				if (item != null)
				{
					item.OwnerChanged += new EventHandler(OnItemOwnerChanged);

					if (GetPlacement(item)!= ToolStripItemPlacement.None)
					{
						AddTool(id, item);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="id"></param>
			/// <param name="item"></param>
			void OnItemRemoving(int id, ToolStripItem item)
			{
				if (item != null)
				{
					item.OwnerChanged -= new EventHandler(OnItemOwnerChanged);

					if (GetPlacement(item) != ToolStripItemPlacement.None)
					{
						DeleteTool(id);
					}
				}
			}
			
			void OnItemRemoved(int id)
			{
				TOOLINFO ti = new TOOLINFO();
				IntPtr lpszText = Marshal.StringToHGlobalAuto("tooltip");

				for (int i = id, count = this.Count; i < count; i++)
				{
					if (GetToolInfo(i + 1, ref ti))
					{
						m_owner.SendMessage(TTM.TTM_DELTOOLW, 0, ref ti);

						ti.lpszText = lpszText;
						ti.uId = new IntPtr(i);

						m_owner.SendMessage(TTM.TTM_ADDTOOLW, 0, ref ti);
					}
				}

				Marshal.FreeHGlobal(lpszText);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="id"></param>
			/// <param name="item"></param>
			void AddTool(int id, ToolStripItem item)
			{
				if (m_tsMain!=null && m_tsMain.IsHandleCreated)
				{
					TOOLINFO ti = new TOOLINFO();
					ti.cbSize = (uint)Marshal.SizeOf(ti);
					ti.uFlags = ADDFLAGS;
					ti.lpszText = Marshal.StringToHGlobalAuto("tooltip");
					ti.hinst = IntPtr.Zero;
					ti.lParam = IntPtr.Zero;

					ti.hwnd = item.IsOnOverflow ? m_tsOverflow.Handle : m_tsMain.Handle;
					ti.uId = new IntPtr(id);
					ti.rect = (RECT)item.Bounds;

					m_owner.SendMessage(TTM.TTM_ADDTOOLW, 0, ref ti);

					Marshal.FreeHGlobal(ti.lpszText);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="id"></param>
			void DeleteTool(int id)
			{
				TOOLINFO ti = new TOOLINFO();
				if (GetToolInfo(id, ref ti))
				{
					m_owner.SendMessage(TTM.TTM_DELTOOLW, 0, ref ti);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="id"></param>
			/// <param name="ti"></param>
			/// <returns></returns>
			bool GetToolInfo(int id, ref TOOLINFO ti)
			{
				bool bResult = false;

				if (m_tsMain!=null && m_tsMain.IsHandleCreated)
				{
					ti.cbSize = (uint)Marshal.SizeOf(ti);
					ti.hwnd = m_tsMain.Handle;
					ti.uId = new IntPtr(id);

					bResult = m_owner.SendMessage(TTM.TTM_GETTOOLINFOW, 0, ref ti) != 0;
					if (!bResult && m_tsMain.CanOverflow)
					{
						if (m_tsOverflow.IsHandleCreated)
						{
							ti.hwnd = m_tsOverflow.Handle;
							bResult = m_owner.SendMessage(TTM.TTM_GETTOOLINFOW, 0, ref ti) != 0;
						}
					}
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			void UpdateMainTools()
			{
				IntPtr lpszText = Marshal.StringToHGlobalAuto("tooltip");
				TOOLINFO ti = new TOOLINFO();

				for (int i = 0, count = this.Count; i < count; i++)
				{
					ToolStripItem item = this[i] as ToolStripItem;
					if (item != null)
					{
						bool bToolExists = GetToolInfo(i, ref ti);

						if (GetPlacement(item) == ToolStripItemPlacement.Main)
						{
							ti.lpszText = lpszText;
							ti.rect = (RECT)item.Bounds;

							if (bToolExists && ti.hwnd == m_tsMain.Handle)
							{
								m_owner.SendMessage(TTM.TTM_NEWTOOLRECTW, 0, ref ti);
							}
							else
							{
								ti.cbSize = (uint)Marshal.SizeOf(ti);
								ti.uFlags = ADDFLAGS;
								ti.hwnd = m_tsMain.Handle;
								ti.uId = new IntPtr(i);
								ti.lParam = IntPtr.Zero;

								m_owner.SendMessage(TTM.TTM_ADDTOOLW, 0, ref ti);
							}
						}
						else
						{
							if (bToolExists && ti.hwnd == m_tsMain.Handle)
							{
								m_owner.SendMessage(TTM.TTM_DELTOOLW, 0, ref ti);
							}
						}
					}
				}

				Marshal.FreeHGlobal(lpszText);
			}
			/// <summary>
			/// 
			/// </summary>
			void UpdateOverflowTools()
			{
				TOOLINFO ti = new TOOLINFO();
				IntPtr lpszText = Marshal.StringToHGlobalAuto("tooltip");

				for (int i = 0, count = this.Count; i < count; i++)
				{
					ToolStripItem item = this[i] as ToolStripItem;
					if (item != null)
					{
						bool bToolExists = GetToolInfo(i, ref ti);

						if (GetPlacement(item) == ToolStripItemPlacement.Overflow)
						{
							ti.lpszText = lpszText;
							ti.rect = (RECT)item.Bounds;

							if (bToolExists && ti.hwnd == m_tsOverflow.Handle)
							{
								m_owner.SendMessage(TTM.TTM_SETTOOLINFOW, 0, ref ti);
							}
							else
							{
								ti.cbSize = (uint)Marshal.SizeOf(ti);
								ti.uFlags = ADDFLAGS;
								ti.hwnd = m_tsOverflow.Handle;
								ti.uId = new IntPtr(i);
								ti.lParam = IntPtr.Zero;

								m_owner.SendMessage(TTM.TTM_ADDTOOLW, 0, ref ti);
							}
						}
						else
						{
							if (bToolExists && ti.hwnd == m_tsOverflow.Handle)
							{
								m_owner.SendMessage(TTM.TTM_DELTOOLW, 0, ref ti);
							}
						}
					}
				}
				Marshal.FreeHGlobal(lpszText);
			}
			#endregion

			#region Event handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnMainHandleCreated(object sender, EventArgs e)
			{
				Control control = sender as Control;
				if (control != null && m_mhMain != null)
				{
					UpdateMainTools();

					m_mhMain.MessageFilter = this;
					m_mhMain.Assign(control.Handle);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnMainHandleDestroyed(object sender, EventArgs e)
			{
				for (int i = 0, count = this.Count; i < count; i++)
				{
					ToolStripItem item = this[i] as ToolStripItem;
					if ( GetPlacement(item) != ToolStripItemPlacement.None)
					{
						DeleteTool(i);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnOverflowHandleCreated(object sender, EventArgs e)
			{
				Control control = sender as Control;
				if (control != null && m_mhOverflow != null)
				{
					UpdateOverflowTools();

					m_mhOverflow.MessageFilter = this;
					m_mhOverflow.Assign(control.Handle);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnMainLayoutCompleted(object sender, EventArgs e)
			{
				if (m_tsMain.IsHandleCreated)
				{
					UpdateMainTools();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnOverflowLayoutCompleted(object sender, EventArgs e)
			{
				if (m_tsOverflow.IsHandleCreated)
				{
					UpdateOverflowTools();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnItemOwnerChanged(object sender, EventArgs e)
			{
				this.Remove(sender);
				m_owner.AddComponent(sender);
			}
			#endregion

			#region Fields
			ToolStripItemsToolTipService m_owner = null;

			ToolStrip m_tsMain = null;
			ToolStrip m_tsOverflow = null;

			NativeMessageHandler m_mhMain = null;
			NativeMessageHandler m_mhOverflow = null;

			static uint ADDFLAGS = 0;
			#endregion
		}
		#endregion

		#region Constructors
		public ToolStripItemsToolTipService(ToolTipControl tooltipControl)
		{
			m_tooltip = tooltipControl;
			m_unownedItems = new ToolStripData(null, this);
		}
		#endregion

		#region IToolTipService implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		public void AddComponent(object component)
		{
			ToolStripItem item = component as ToolStripItem;
			if (item != null)
			{
				ToolStripData tsData = GetData( GetOwner(item) );
				if (tsData != null)
				{
					tsData.Add(component);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		public void RemoveComponent(object component)
		{
			ToolStripItem item = component as ToolStripItem;
			if (item != null)
			{
				ToolStrip ts = GetOwner(item);
				if (ts != null)
				{
					ToolStripData tsData = this[ts] as ToolStripData;
					if (tsData != null)
					{
						tsData.Remove(component);

						if (tsData.Count == 0)
						{
							tsData.Dispose();
							this.Remove(ts);
						}
					}
				}
				else m_unownedItems.Remove(item);
			}
		}

		public event PopupHandler Popup;
		public event EventHandler Pop;
		#endregion

		#region IDisposable implementation
		public void Dispose()
		{
			foreach (object obj in this.Values)
			{
				IDisposable iDisposable = obj as IDisposable;
				if (iDisposable != null)
				{
					iDisposable.Dispose();
				}
			}
            if( m_unownedItems != null )
            {
                m_unownedItems.Clear();
                m_unownedItems = null;
            }
            
            m_tooltip = null;

			Clear();
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="wparam"></param>
		/// <param name="ti"></param>
		/// <returns></returns>
		int SendMessage(TTM msg, int wparam, ref TOOLINFO ti)
		{
			if (!m_tooltip.IsDisposed)
			{
				return WindowsAPI.SendMessage(m_tooltip.Handle, (int)msg, wparam, ref ti);
			}
			return 0;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="wparam"></param>
		/// <param name="lparam"></param>
		/// <returns></returns>
		IntPtr SendMessage(int msg, int wparam, IntPtr lparam)
		{
			if (!m_tooltip.IsDisposed)
			{
				return WindowsAPI.SendMessage(m_tooltip.Handle, msg, wparam, lparam);
			}
			return IntPtr.Zero;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		bool OnMouseMessage(ref Message m)
		{
			MSG msg = new MSG();
			msg.hwnd = m.HWnd;
			msg.message = m.Msg;
			msg.wParam = m.WParam;
			msg.lParam = m.LParam;

			WindowsAPI.SendMessage(m_tooltip.Handle, (int)TTM.TTM_RELAYEVENT, 0, ref msg);

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool OnNotify(ref Message m, ToolStripData tsData)
		{
			bool bResult = false;

			NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
			switch (hdr.code)
			{
				case (int)TTN.TTN_SHOW:
					if (m_tooltip.IsHandleCreated && m_tooltip.Handle == hdr.hwndFrom)
					{
						if (Popup != null)
						{
							Popup(tsData[(int)hdr.idFrom] as Component, ref m);
							bResult = true;
						}
					}
					break;
				case (int)TTN.TTN_POP:
					if (m_tooltip.IsHandleCreated && m_tooltip.Handle == hdr.hwndFrom)
					{
						if (Pop != null)
						{
							Pop(null, EventArgs.Empty);
							bResult = true;
						}
					}
					break;
			}
			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		ToolStripData GetData(ToolStrip ts)
		{
			if (ts != null)
			{
				ToolStripData tsData = this[ts] as ToolStripData;
				if (tsData == null)
				{
					tsData = new ToolStripData(ts, this);
					Add(ts, tsData);
				}
				return tsData;
			}
			return m_unownedItems;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static ToolStrip GetOwner(ToolStripItem item)
		{
			if (item != null)
			{
				if (item is ICustomItem)
				{
					return ((ICustomItem)item).Owner;
				}

				ToolStrip tsOwner = item.Owner;

				if (tsOwner is MenuDropDown.IPanel)
				{
					return ((MenuDropDown.IPanel)tsOwner).Owner;
				}
				
				return tsOwner;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static ToolStripItemPlacement GetPlacement(ToolStripItem item)
		{
			if (item != null)
			{
				ICustomItem custom = item as ICustomItem;
				if (custom != null)
				{
					return custom.Placement;
				}

				MenuDropDown.IPanel panel = item.Owner as MenuDropDown.IPanel;
				if (panel != null)
				{
					return panel.GetItemPlacement(item);
				}

				return  item.Placement;
			}
			return ToolStripItemPlacement.None;
		}
		#endregion

		#region Fields
		ToolStripData m_unownedItems;
		ToolTipControl m_tooltip;
		#endregion
	}
	#endregion
}

#endif