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
	/// Represents a text box of ribbon control. 
	/// </summary>
	[TemplatePart(Name = "RootElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "ContentElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "FocusVisualElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "ReadOnlyVisualElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "DisabledVisualElement", Type = typeof(FrameworkElement))]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "ReadOnly")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
	public class RibbonTextBox :
		TextBox
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonTextBox"/> class.
		/// </summary>
		public RibbonTextBox()
		{
			this.DefaultStyleKey = typeof(RibbonTextBox);
		}

        /// <summary>
        /// Initializes the <see cref="RibbonTextBox"/> class.
        /// </summary>
        static RibbonTextBox()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

		#endregion

		#region Overrides

		/// <summary>
		/// Builds the visual tree for the <see cref="T:System.Windows.Controls.ComboBox"/> when a new template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		#endregion
	}
}
