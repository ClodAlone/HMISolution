#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents a selectable item contained in a <see cref="RibbonComboBox"/> control.
	/// </summary>
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
	[TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
	[TemplateVisualState(GroupName = "SelectionStates", Name = "SelectedUnfocused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
	public class RibbonComboBoxItem : ComboBoxItem, IRibbonControl
	{
		#region Fields

		private RibbonComboBox owner;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonComboBoxItem"/> class.
		/// </summary>
		public RibbonComboBoxItem()
		{
			this.DefaultStyleKey = typeof(RibbonComboBoxItem);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Builds the visual tree for the <see cref="T:System.Windows.Controls.ListBoxItem"/> control when a new template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		/// <summary>
		/// Provides handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			if (!e.Handled)
			{
				RibbonComboBox rcb = this.Owner;

				e.Handled = true;

				if (rcb != null)
				{
					rcb.CloseDropDownWithTransition();
				}
			}
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
		internal RibbonComboBox Owner
		{
			get { return this.owner; }
			set { this.owner = value; }
		}

		#endregion
	}
}
