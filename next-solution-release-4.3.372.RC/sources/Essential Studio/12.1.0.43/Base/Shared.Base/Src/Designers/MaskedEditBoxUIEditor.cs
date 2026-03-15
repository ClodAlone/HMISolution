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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// Summary description for MaskedEditCultureEditor.
	/// </summary>
	public class MaskedEditCultureEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		private ListBox lb;
		private MaskedEditBox maskedEditBox;
		private IServiceProvider serviceProvider;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{

			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{

				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				serviceProvider = provider;

				if (edSvc != null) 
				{
					
					lb = new ListBox();
					lb.BorderStyle = BorderStyle.None;
					lb.SelectedIndexChanged+=new EventHandler(ValueChanged);

					maskedEditBox = (MaskedEditBox)context.Instance;
					
					lb.Items.Add("(Default)");
					lb.Items.Add("(UICulture)");
					lb.Items.Add("(InstalledUICulture)");
					foreach(CultureInfo c in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
					{
						lb.Items.Add(c.Name + "  " + c.EnglishName);
					}

					if(maskedEditBox.SpecialCultureValue == SpecialCultureValues.CurrentCulture)
					{
						lb.SetSelected(0, true);
					}
					if(maskedEditBox.SpecialCultureValue == SpecialCultureValues.UICulture)
					{
						lb.SetSelected(1, true);
					}
					if(maskedEditBox.SpecialCultureValue == SpecialCultureValues.InstalledCulture)
					{
						lb.SetSelected(2, true);
					}

					edSvc.DropDownControl(lb);
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		private void ValueChanged(object sender, EventArgs e) 
		{
			if(serviceProvider!= null)
			{
				IComponentChangeService changeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			
				if(changeService != null)
				{
					changeService.OnComponentChanging(maskedEditBox, null);
					changeService.OnComponentChanged(maskedEditBox, null, null, null);
				}
			}
			if (edSvc != null) 
			{
				edSvc.CloseDropDown();
				ListBox lb = (ListBox)sender;
				string selected = lb.GetItemText(lb.SelectedItem);
			
				if(selected.Equals("(Default)"))
				{
					maskedEditBox.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
					maskedEditBox.Culture = new CultureInfo(CultureInfo.CurrentCulture.LCID, maskedEditBox.UseUserOverride);
				}
				else if(selected.Equals("(UICulture)"))
				{
					maskedEditBox.SpecialCultureValue = SpecialCultureValues.UICulture;
					maskedEditBox.Culture = new CultureInfo(CultureInfo.CurrentUICulture.LCID, maskedEditBox.UseUserOverride);
				}
				else if(selected.Equals("(InstalledUICulture)"))
				{
					maskedEditBox.SpecialCultureValue = SpecialCultureValues.InstalledCulture;
					maskedEditBox.Culture = new CultureInfo(CultureInfo.InstalledUICulture.LCID, maskedEditBox.UseUserOverride);
				}
				else 
				{
					int spacepos = selected.IndexOf(' ');
					string cultureString = selected.Substring(0, spacepos);
					maskedEditBox.SpecialCultureValue = SpecialCultureValues.None;
					maskedEditBox.Culture = new CultureInfo(cultureString,maskedEditBox.UseUserOverride);
				}
			}
		}
	}

	/// <summary>
	/// TypeConverter for MaskedEditDataGroupInfo.
	/// </summary>
	public class MaskedEditDataGroupInfoConverter : TypeConverter
	{
		/// <summary>
		/// Indicates whether this converter can convert an object to
		/// the given destination type using the context.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext object that provides a format context. </param>
		/// <param name="destinationType">A <see cref="Type"/> object that represents the type to which you want to convert. </param>
		/// <returns>True if conversion is possible; false otherwise.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// This member overrides <see cref="TypeConverter.ConvertTo(ITypeDescriptorContext, CultureInfo, object, Type) "/>.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed. </param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns>Converted object.</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) 
			{
				MaskedEditDataGroupInfo info = value as MaskedEditDataGroupInfo;

				return new InstanceDescriptor(typeof(MaskedEditDataGroupInfo).GetConstructor
					(new Type[3] {typeof(string),typeof(int),typeof(MaskGroupAlignment)}), 
					new object[3] {info.DataGroupName, info.DataGroupSize,info.DataGroupAlignment});
			}
			return base.ConvertTo(context, culture, value, destinationType);

		}
	}
}