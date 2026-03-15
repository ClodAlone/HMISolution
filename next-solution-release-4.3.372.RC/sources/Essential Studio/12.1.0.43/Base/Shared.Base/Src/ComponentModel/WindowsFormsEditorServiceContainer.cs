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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// Provides support for the <see cref="GetItemProperties"/> method that returns a <see cref="PropertyDescriptorCollection"/>.
	/// </summary>
	public interface IItemPropertiesSource
	{
		/// <summary>
		/// Returns a collection of property descriptors.
		/// </summary>
		/// <returns></returns>
		PropertyDescriptorCollection GetItemProperties();
	}


	/// <summary>
	/// Provides a basic implementation for the IWindowsFormsEditorService and IServiceProvider interfaces and can be used
	/// together with TypeDescriptorContext to launch an Editor directly outside a property grid.
	/// </summary>
	/// <example>
	/// The grid uses this class to display a collection editor.
	/// <code lang="C#">
	/// public static DialogResult ShowGridBaseStylesMapDialog(object instance, string propertyName)
	/// {
	///     GridBaseStyleCollectionEditor ce = new GridBaseStyleCollectionEditor(typeof(ArrayList));
	/// WindowsFormsEditorServiceContainer esc = new WindowsFormsEditorServiceContainer(null);
	///     PropertyDescriptor pd = TypeDescriptor.GetProperties(instance)[propertyName];
	///     TypeDescriptorContext tdc = new TypeDescriptorContext(instance, pd);
	///     tdc.ServiceProvider = esc;
	///     object v = ce.EditValue(tdc, esc, ((ICloneable) pd.GetValue(instance)).Clone());
	///     if (esc.DialogResult == DialogResult.OK)
	///     {
	///         pd.SetValue(instance, v);
	///     }
	///     return esc.DialogResult;
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="TypeDescriptorContext"/>
	public class WindowsFormsEditorServiceContainer : IWindowsFormsEditorService, IServiceProvider
	{
		DialogResult result = DialogResult.Cancel;
		private IServiceProvider serviceProvider;
		//private IHelpService helpService;

		/// <summary>
		/// Initializes a WindowsFormsEditorServiceContainer with the given IServiceProvider.
		/// </summary>
		/// <param name="serviceProvider">An IServiceProvider. Can be NULL.</param>
		public WindowsFormsEditorServiceContainer(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		void IWindowsFormsEditorService.CloseDropDown()
		{
		}

		/// <summary>
		/// Occurs immediately before the Dialog is displayed. The ControlEventArgs.Control 
		/// the form.
		/// </summary>
		public event ControlEventHandler ShowingDialog;

		DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
		{
			if(ShowingDialog != null)
			{
				ControlEventArgs e = new ControlEventArgs(dialog);
				ShowingDialog(this, e);
			}

			// Weird, but if I don't explicitly initialize the cursor I get
			// a totally strange cursor for the dialog.
			dialog.Cursor = Cursors.Default;
			 foreach (Control c in dialog.Controls)
			{
				c.Cursor = Cursors.Default;
			}

            Button acceptButton = ((Button) dialog.AcceptButton);
            acceptButton.Click += new EventHandler(WindowsFormsEditorServiceContainer_Click);
			dialog.ShowDialog();
            acceptButton.Click -= new EventHandler(WindowsFormsEditorServiceContainer_Click);
            dialog.DialogResult = result;
            return result;
		}

        void WindowsFormsEditorServiceContainer_Click(object sender, EventArgs e)
        {
             result = DialogResult.OK;
        }

		/// <summary>
		/// Returns the dialog result of the edit operation.
		/// </summary>
		public DialogResult DialogResult
		{
			get { return result; }
		}

		void IWindowsFormsEditorService.DropDownControl(Control ctl)
		{
		}

		object IServiceProvider.GetService(Type classService)
		{
			if ((classService == typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))) 
				return this;

			if (this.ServiceProvider != null) 
				return this.serviceProvider.GetService(classService);

			return null;
		} 

		/// <summary>
		/// Gets / sets the associated IServiceProvider.
		/// </summary>
		public IServiceProvider ServiceProvider 
		{ 
			get
			{
				return this.serviceProvider;
			} 
			set
			{
				if (value != this.serviceProvider)
				{
					this.serviceProvider = value;
					//this.helpService = null;
				}
			} 
		}
	}
	
}
