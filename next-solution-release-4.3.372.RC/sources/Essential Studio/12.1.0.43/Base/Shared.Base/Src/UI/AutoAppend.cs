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
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Win32;
using Syncfusion.Runtime.Serialization;
using System.ComponentModel;

using Microsoft.Win32;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the information required by the <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> class to enable auto appending
	/// in a control.
	/// </summary>
	/// <remarks>
	/// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> for a usage example.
	/// </remarks>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/>
	public struct AutoAppendInfo
	{
		private bool autoAppend;
		private string categoryName;
		private IList items;
		private int maxItems;

		/// <summary>
		/// Indicates whether to turn on AutoAppend.
		/// </summary>
		/// <value>True to turn on auto appending; False to turn off.</value>
		/// <remarks>
		/// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> for a usage example.
		/// </remarks>
		public bool AutoAppend
		{
			get{
				return this.autoAppend;
			}
			set{this.autoAppend = value;}
		}
		/// <summary>
		/// Gets / sets the Category Name to which the contents of the list belong to.
		/// </summary>
		/// <value>The category name.</value>
 		/// <remarks>
		/// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> for a usage example.
		/// </remarks>
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set{this.categoryName = value;}
		}
		/// <summary>
		/// Gets / sets the IList into which new entries will be appended.
		/// </summary>
		/// <value>A reference to an IList instance.</value>
 		/// <remarks>
		/// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> for a usage example.
		/// </remarks>
		public IList Items
		{
			get
			{
				return this.items;
			}
			set{this.items = value;}
		}
		/// <summary>
		/// Gets / sets the desired maximum number of items in the list.
		/// </summary>
		/// <value>The maximum count.</value>
		/// <remarks>If the count exceeds the maximum count, then AutoAppend will keep
		/// discarding the older entries.
		/// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.AutoAppend"/> for a usage example.
		/// </remarks>
		public int MaxItems
		{
			get
			{
				return this.maxItems;
			}
			set{this.maxItems = value;}
		}

		/// <summary>
		/// Creates a new instance of this class and initializes it with these values.
		/// </summary>
		/// <param name="autoAppend">Indicates whether to turn on AutoAppend; False if not.</param>
		/// <param name="categoryName">The Category to which the contents in this control belong to.</param>
		/// <param name="items">The reference to an IList which will get the new items entered by the user.</param>
		/// <param name="maxItems">The maximum number of items in the list.</param>
		public AutoAppendInfo(bool autoAppend, string categoryName, IList items, int maxItems)
		{
			this.autoAppend = autoAppend;
			this.categoryName = categoryName;
			this.items = items;
			this.maxItems = maxItems;
		}
	}

	/// <summary>
	/// The AutoAppend class provides choice-list auto-append capabilities for editable combo boxes, etc.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Often editable combo boxes in your applications can benefit from reusing 
	/// previously user supplied entries in their choice-list, as in IE's address bar combo box and
	/// in the "Find" combo box in VS.NET.
	/// </para>
	/// <para>
	/// The AutoAppend class will easily enable this functionality in an associated combo box. 
	/// In fact, this class is more abstract in that
	/// it can take any control and an IList reference and enable "AutoAppend" logic on
	/// them as described below.
	/// </para>
	/// <para>
	/// Associate your control with this class through the <see cref="AutoAppendInfo"/> structure.
	/// The <see cref="AutoAppendInfo"/> structure will let you associate a control with an IList 
	/// reference and a Category string to an AutoAppend instance.
	/// </para>
 	/// <para>Once you set up this association, the AutoAppend instance listens to the Validated event 
 	/// thrown by the control
	/// and inserts the new entry on top of the supplied IList or if 
	/// already in the list, moves the entry to top of the list. When the control gets disposed, 
	/// it persists these entries in the registry in a key based on the corresponding Category string.
	/// The next time the control is created and associated, the AutoAppend instance will pick up 
	/// the persisted information corresponding to the Category and apply that information
	/// to the IList instance.
	/// </para>
	/// <para>
	/// Persistance is done in the registry under the HKLU key making the entries specific to the
	/// user and global to the machine (usable across applications). Due to the previous behavior,
	/// the very first instantiation of the above control will set the control's text and the IList
	/// to be empty.
	/// </para> 
	/// <para><bold>Note: </bold>Also make sure your control's Dispose() gets called before the ApplicationExit event
	/// thrown by the Application object. Otherwise, the control's state will not be persisted. This
	/// would normally be the case, unless the control gets "orphaned" (detached from the parent control) and the
	/// Dispose method never gets called until it is garbage collected.
	/// </para>
	/// <para>You can make any number of associations with one AutoAppend instance.</para>
	/// </remarks>
	/// <example>
	/// This is how you would associate a combo box with an instance of the AutoAppend class:
	/// <code lang="C#">
	/// this.autoAppend = new AutoAppend();
	/// this.autoAppend.SetAutoAppend(this.comboBox1, new AutoAppendInfo(true, "HttpAddress", this.comboBox1.Items, 30));
	/// // To disassociate call this:
	/// this.autoAppend.SetAutoAppend(this.comboBox1, new AutoAppendInfo(false, String.Empty, null, 30));
	/// </code>
	/// <code lang="VB">
	/// Me.autoAppend = New AutoAppend()
	/// Me.autoAppend.SetAutoAppend(Me.comboBox1, New AutoAppendInfo(True, "HttpAddress", Me.comboBox1.Items, 30))
	/// ' To disassociate call this:
	/// Me.autoAppend.SetAutoAppend(Me.comboBox1, New AutoAppendInfo(False, [String].Empty, Nothing, 30))
	/// </code>
	/// </example>
	public class AutoAppend : IDisposable
	{
		private Hashtable controls = new Hashtable();
		internal bool disposed = false;

		static AutoAppend()
		{
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo("Syncfusion.Shared.Base", typeof(AutoAppend).Assembly);
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Shared.Base", typeof(AutoAppend).Assembly);
			// To support backward compatibility.
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Shared.Base", typeof(AutoAppend).FullName, typeof(AutoAppend).Assembly);
#endif
		}

		/// <summary>
		/// Creates a new instance of the AutoAppend class.
		/// </summary>
		public AutoAppend()
		{
		}

		/// <summary>
		/// This member overrides Object.Finalize.
		/// </summary>
		~AutoAppend()
		{
			try 
			{
				this.Dispose(false);
			}
			catch{}
		}

		/// <summary>
		/// Releases all setup associations.
		/// </summary>
		/// <remarks>
		/// This will also persist the exisiting information before releasing the associations.
		/// </remarks>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Override this to release the unmanaged resources used by the control and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(this.disposed)
				return;

			this.disposed = true;

			// Store address combo box values in registry.
			Control[] controlArray = new Control[this.controls.Keys.Count];
			this.controls.Keys.CopyTo(controlArray, 0);
			foreach(Control control in controlArray)
				this.SetAutoAppend(control, new AutoAppendInfo(false, String.Empty, null, 30));

			this.controls.Clear();
			this.controls = null;
		}

		/// <summary>
		/// Returns the AutoAppendInfo associated with a control.
		/// </summary>
		/// <param name="control">The control whose AutoAppend info is required.</param>
		/// <returns>The corresponding AutoAppendInfo value. If this control is not associated
		/// yet, then an AutoAppendInfo with its AutoAppend value set to False will be returned.</returns>
		public virtual AutoAppendInfo GetAutoAppend(Control control)
		{
			if(this.controls[control] != null)
				return (AutoAppendInfo)this.controls[control];

			else return new AutoAppendInfo(false, String.Empty, null, 30);
		}

		/// <summary>
		/// Associates a control with this instance by providing its AutoAppendInfo.
		/// </summary>
		/// <param name="control">The control in which to AutoAppend.</param>
		/// <param name="autoAppendInfo">The AutoAppendInfo.</param>
		/// <remarks>
		/// <para>Calling this will enable <see cref="AutoAppend"/> behavior in the control. Take a look at the AutoAppend 
		/// class reference for detailed information on this behavior.</para>
		/// To disassociate a control from the AutoAppend instance, call this method with the
		/// AutoAppendInfo's AutoAppend value set to False.
		/// </remarks>
		public virtual void SetAutoAppend(Control control, AutoAppendInfo autoAppendInfo)
		{
			if(!autoAppendInfo.AutoAppend)
			{
				if(this.controls[control] != null)
					this.PersistInStorage((AutoAppendInfo)this.controls[control]);
				this.controls.Remove(control);
				control.Validated -= new EventHandler(this.ControlValidated);
				control.Disposed -= new EventHandler(this.ControlDisposed);
			}
			else
			{
				this.controls[control] = autoAppendInfo;
				control.Validated += new EventHandler(this.ControlValidated);
				control.Disposed += new EventHandler(this.ControlDisposed);
				this.InitFromStorage(control);
			}
		}
		private void ControlDisposed(object sender, EventArgs e)
		{
			if(this.disposed)
			{
				((Control)sender).Disposed -= new EventHandler(this.ControlValidated);
				return;
			}
			this.SetAutoAppend(sender as Control, new AutoAppendInfo(false, String.Empty, null, 10));			
		}

		private void ControlValidated(object sender, EventArgs e)
		{
			if(this.disposed)
			{
				((Control)sender).Validated -= new EventHandler(this.ControlValidated);
				return;
			}
			
			Control control = sender as Control;
			if(this.controls[control] == null)
				return;

			AutoAppendInfo autoAppendInfo = (AutoAppendInfo)this.controls[control];
			string newText = control.Text;

			if(newText == String.Empty)
				return;
			
			int selIndex = autoAppendInfo.Items.IndexOf(newText);
			if(selIndex == -1)
			{
				//if(this.autoAppendNewItems)
				{
					// New value entered in edit box.
					// Insert it into the items list.
					autoAppendInfo.Items.Insert(0, newText);
					control.Text = newText;
				}
			}
			else if(selIndex != 0)
			{
				// Make the selected index topmost in the list.
				MoveItemToTop(autoAppendInfo.Items, selIndex);
				control.Text = newText;
			}			
		}
		private void PersistInStorage(AutoAppendInfo autoAppendInfo)
		{
			if(autoAppendInfo.CategoryName != String.Empty)
			{
				ArrayList values = new ArrayList();
				int i = 0;
				foreach(string value in autoAppendInfo.Items)
				{
					i++;
					values.Add(value);
					if(i > autoAppendInfo.MaxItems)
						break;
				}

				RegistryKey regKey = Registry.CurrentUser;
				regKey = regKey.CreateSubKey("Software\\Syncfusion\\AutoAppend");
				AppStateSerializer serializer = new AppStateSerializer(SerializeMode.WindowsRegistry, regKey);
				serializer.SerializeObject(autoAppendInfo.CategoryName, values);
				serializer.PersistNow();
			}
		}
		private void InitFromStorage(Control control)
		{
			AutoAppendInfo autoAppendInfo = (AutoAppendInfo)this.controls[control];
			if(autoAppendInfo.CategoryName == String.Empty)
				return;

			// Will clear exisiting items, if any.
			// Initialize the combo box with more items in the HandleCreated event handler.
			autoAppendInfo.Items.Clear();

			ArrayList values;
			object valuesObject;
			RegistryKey regKey = Registry.CurrentUser;
			regKey = regKey.CreateSubKey("Software\\Syncfusion\\AutoAppend");
			AppStateSerializer serializer = new AppStateSerializer(SerializeMode.WindowsRegistry, regKey);
			valuesObject = serializer.DeserializeObject(autoAppendInfo.CategoryName);

			values = valuesObject as ArrayList;
			if(values != null)
			{
				foreach(string item in values)
					autoAppendInfo.Items.Add(item);
			}

			if(autoAppendInfo.Items.Count > 0 && autoAppendInfo.Items.IndexOf(control.Text) == -1)
				control.Text = (string)autoAppendInfo.Items[0];
		}

		private void MoveItemToTop(IList items, int index)
		{
			if(index < items.Count)
			{
				object selectedItem = items[index];
				items.RemoveAt(index);
				items.Insert(0, selectedItem);
			}
		}

		/// <summary>
		/// Forces an entry into the control's AutoAppend list.
		/// </summary>
		/// <param name="control">The control whose AutoAppend list is to be updated.</param>
		/// <param name="item">The value that is to be appended to the list.</param>
		/// <remarks>
		/// The AutoAppend instance automatically inserts entries into the list when the user
		/// enters a new value and leaves the control (when Validated will be fired).
		/// However, you might want new entries to be added when, for example, the user hits
		/// the Enter key. You do so by calling this method with the new value which will
		/// insert the new value into the list.
		/// </remarks>
		public void InsertOrMoveToTop(Control control, string item)
		{
			if(item == String.Empty)
				return;

			AutoAppendInfo autoAppendInfo = (AutoAppendInfo)this.controls[control];
			
			// Parse through the list to see if it matches any of the current items.
			int index = autoAppendInfo.Items.IndexOf(item);
			if(index != -1)
				MoveItemToTop(autoAppendInfo.Items, index);
			else
				autoAppendInfo.Items.Insert(0, item);

			//this.SelectedIndex = 0;
		}
	}
}
