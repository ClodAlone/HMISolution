#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents ribbon's radio-button control.
	/// </summary>
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
	[TemplateVisualState(GroupName = "CheckStates", Name = "Checked")]
	[TemplateVisualState(GroupName = "CheckStates", Name = "Unchecked")]
	public class RibbonRadioButton :
        RadioButton, IRibbonControl
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonRadioButton"/> class.
		/// </summary>
		public RibbonRadioButton()
		{
			this.DefaultStyleKey = typeof(RibbonRadioButton);
			this.IsTabStop = false;
		}

        /// <summary>
        /// Initializes the <see cref="RibbonRadioButton"/> class.
        /// </summary>
        static RibbonRadioButton()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
		#endregion
	}
}
