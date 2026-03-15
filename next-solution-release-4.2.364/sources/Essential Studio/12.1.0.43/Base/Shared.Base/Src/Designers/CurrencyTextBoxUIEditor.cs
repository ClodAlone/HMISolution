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


using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// Summary description for CurrencyCultureEditor.
	/// </summary>
	public class CurrencyCultureEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		private ListBox lb;
		private NumberTextBoxBase currencyTextBox;
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

					currencyTextBox = (NumberTextBoxBase)context.Instance;
					
					lb.Items.Add("(Default)");
					lb.Items.Add("(UICulture)");
					lb.Items.Add("(InstalledUICulture)");
					foreach(CultureInfo c in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
					{
						lb.Items.Add(c.Name + "  " + c.EnglishName);
					}

					if(currencyTextBox.SpecialCultureValue == SpecialCultureValues.CurrentCulture)
					{
						lb.SetSelected(0, true);
					}
					if(currencyTextBox.SpecialCultureValue == SpecialCultureValues.UICulture)
					{
						lb.SetSelected(1, true);
					}
					if(currencyTextBox.SpecialCultureValue == SpecialCultureValues.InstalledCulture)
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
					changeService.OnComponentChanging(currencyTextBox, null);
					changeService.OnComponentChanged(currencyTextBox, null, null, null);
				}
			}
			if (edSvc != null) 
			{
				edSvc.CloseDropDown();
				ListBox lb = (ListBox)sender;
				string selected = lb.GetItemText(lb.SelectedItem);
			
				if(selected.Equals("(Default)"))
				{
					currencyTextBox.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
					currencyTextBox.Culture = new CultureInfo(CultureInfo.CurrentCulture.LCID, currencyTextBox.UseUserOverride);
				}
				else if(selected.Equals("(UICulture)"))
				{
					currencyTextBox.SpecialCultureValue = SpecialCultureValues.UICulture;
					currencyTextBox.Culture = new CultureInfo(CultureInfo.CurrentUICulture.LCID, currencyTextBox.UseUserOverride);
				}
				else if(selected.Equals("(InstalledUICulture)"))
				{
					currencyTextBox.SpecialCultureValue = SpecialCultureValues.InstalledCulture;
					currencyTextBox.Culture = new CultureInfo(CultureInfo.InstalledUICulture.LCID, currencyTextBox.UseUserOverride);
				}
				else 
				{
					int spacePos = selected.IndexOf(" ");
					string cultureString = selected.Substring(0, spacePos);
					currencyTextBox.SpecialCultureValue = SpecialCultureValues.None;
					currencyTextBox.Culture = new CultureInfo(cultureString, currencyTextBox.UseUserOverride);
				}
			}
		}
	}
}