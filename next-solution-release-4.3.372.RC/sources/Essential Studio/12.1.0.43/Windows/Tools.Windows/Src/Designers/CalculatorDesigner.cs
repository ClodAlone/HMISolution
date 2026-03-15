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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
    public class CalculatorDesigner :ControlDesigner
    {
        /// <summary>
        /// Fields
        /// </summary>
		protected DesignerVerb dvWindowsStandard = null;
		protected DesignerVerb dvFinancial = null;
		protected DesignerVerbCollection dvcVerbs = null;

		/// <summary>
		/// 
		/// </summary>
		public override DesignerVerbCollection Verbs 
		{
			get
			{
				if(((CalculatorControl)this.Control).LayoutType == CalculatorLayoutTypes.Financial)
				{
					this.dvWindowsStandard.Enabled = true;
					this.dvFinancial.Enabled = false;
				}
				else
				{
					this.dvFinancial.Enabled = true;
					this.dvWindowsStandard.Enabled = false;
				}
				return this.dvcVerbs;	
			}
		}
        
        /// <summary>
        /// 
        /// </summary>
        public CalculatorDesigner()
        {
			this.dvWindowsStandard = new DesignerVerb("Windows Standard Layout", new EventHandler(this.OnWindowsStandardLayout));
			this.dvFinancial = new DesignerVerb("Financial Layout", new EventHandler(this.OnFinancialLayout));
			this.dvWindowsStandard.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { this.dvWindowsStandard, this.dvFinancial}; 
			this.dvcVerbs = new DesignerVerbCollection(dvarray);

        }

//SmartTags added for .NET Framework 2.0        
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        DesignerActionListCollection actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(
                        new CalculatorControlActionList(this.Component));
                }
                return actionLists;
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			CalculatorControl calculatorControl = component as CalculatorControl;
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			iccs.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void OnComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if(this.dvcVerbs != null && this.dvcVerbs.Count > 0)
			{
				if(this.Control != null)
				{
					if(((CalculatorControl)this.Control).LayoutType == CalculatorLayoutTypes.Financial)
					{
						this.dvFinancial.Enabled = false;
						this.dvWindowsStandard.Enabled = true;
					}
					else
					{
						this.dvFinancial.Enabled = true;
						this.dvWindowsStandard.Enabled = false;
					}
				}
				else
				{
					this.dvFinancial.Enabled = false;
					this.dvWindowsStandard.Enabled = false;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnWindowsStandardLayout(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CalculatorControl calculatorControl = this.Control as CalculatorControl;
			CalculatorLayoutTypes old = calculatorControl.LayoutType;
			calculatorControl.LayoutType = CalculatorLayoutTypes.WindowsStandard;
						
			// Raise the component changed event, so that the bar collection can be repersisted.
			this.RaiseComponentChanged(TypeDescriptor.GetProperties((CalculatorControl)this.Control)["LayoutType"], old, ((CalculatorControl)this.Control).LayoutType);			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnFinancialLayout(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CalculatorLayoutTypes old = ((CalculatorControl)this.Control).LayoutType;
			((CalculatorControl)this.Control).LayoutType = CalculatorLayoutTypes.Financial;
						
			// Raise the component changed event, so that the bar collection can be repersisted.
			this.RaiseComponentChanged(TypeDescriptor.GetProperties((CalculatorControl)this.Control)["LayoutType"], CalculatorLayoutTypes.WindowsStandard , ((CalculatorControl)this.Control).LayoutType);			
		}
    }


	/// <summary>
	/// Summary description for CurrencyCultureEditor.
	/// </summary>
	public class CalculatorCultureEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		private ListBox lb;
		private CalculatorControl calculatorControl;
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

					calculatorControl = (CalculatorControl)context.Instance;
					
					lb.Items.Add("(Default)");
					lb.Items.Add("(UICulture)");
					lb.Items.Add("(InstalledUICulture)");
					foreach(CultureInfo c in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
					{
						lb.Items.Add(c.Name + "  " + c.EnglishName);
					}

					if(calculatorControl.SpecialCultureValue == SpecialCultureValues.CurrentCulture)
					{
						lb.SetSelected(0, true);
					}
					if(calculatorControl.SpecialCultureValue == SpecialCultureValues.UICulture)
					{
						lb.SetSelected(1, true);
					}
					if(calculatorControl.SpecialCultureValue == SpecialCultureValues.InstalledCulture)
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
					changeService.OnComponentChanging(calculatorControl, null);
					changeService.OnComponentChanged(calculatorControl, null, null, null);
				}
			}
			if (edSvc != null) 
			{
				edSvc.CloseDropDown();
				ListBox lb = (ListBox)sender;
				string selected = lb.GetItemText(lb.SelectedItem);
			
				if(selected.Equals("(Default)"))
				{
					calculatorControl.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
					calculatorControl.Culture = new CultureInfo(CultureInfo.CurrentCulture.LCID, calculatorControl.UseUserOverride);
				}
				else if(selected.Equals("(UICulture)"))
				{
					calculatorControl.SpecialCultureValue = SpecialCultureValues.UICulture;
					calculatorControl.Culture = new CultureInfo(CultureInfo.CurrentUICulture.LCID, calculatorControl.UseUserOverride);
				}
				else if(selected.Equals("(InstalledUICulture)"))
				{
					calculatorControl.SpecialCultureValue = SpecialCultureValues.InstalledCulture;
					calculatorControl.Culture = new CultureInfo(CultureInfo.InstalledUICulture.LCID, calculatorControl.UseUserOverride);
				}
				else 
				{
					int spacePos = selected.IndexOf(" ");
					string cultureString = selected.Substring(0, spacePos);
					calculatorControl.SpecialCultureValue = SpecialCultureValues.None;
					calculatorControl.Culture = new CultureInfo(cultureString, calculatorControl.UseUserOverride);
				}
			}
		}
	}
}

