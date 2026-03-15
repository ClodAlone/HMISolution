#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents the RibbonItemsGroup Class.
	/// </summary>
	public class ButtonPanel : RibbonItemsControl, IRibbonControl
	{
		#region Constructor

		/// <summary>
		/// Initialize a new instance of <see cref="ButtonPanel"/>
		/// </summary>
		public ButtonPanel()
		{
			this.DefaultStyleKey = typeof(ButtonPanel);
			this.IsTabStop = false;
		}

        Border border;

	    /// <summary>
	    /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
	    /// </summary>
	    public override void OnApplyTemplate()
        {
            border = (Border)GetTemplateChild("PART_Separator");
            base.OnApplyTemplate();
        }

		#endregion

		#region Overrides

	    /// <summary>
	    /// Called before the <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> event occurs.
	    /// </summary>
	    /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
	    protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			FrameworkElement realSource = RibbonContextMenu.GetRealSource(this, e);
			if (realSource == null) return;
			var parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon));
			var parentRibbonBar = VisualUtils.FindAncestor(this, typeof(RibbonBar));
			if (parentRibbonBar != null)
			{
				((RibbonBar)parentRibbonBar).IsRightClickNeeded = true;
			}
			if (parentRibbon != null && parentRibbon is Ribbon && realSource != null)
			{
				((Ribbon)parentRibbon).AddQATinContextMenu(realSource);
			}
			base.OnMouseRightButtonDown(e);
		}

		/// <summary>
		/// Called when [item clicked].
		/// </summary>
		/// <param name="item">The item.</param>
		internal override void OnItemClicked(UIElement item)
		{
			base.OnItemClicked(item);

			if (this.Selector != null)
			{
				this.Selector.OnItemClicked(item);
			}
		}

		/// <summary>
		/// Called when [item selected].
		/// </summary>
		/// <param name="item">The item.</param>
		internal override void OnItemSelected(UIElement item)
		{
			base.OnItemSelected(item);

			if (this.Selector != null)
			{
				this.Selector.OnItemSelected(item);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the selector.
		/// </summary>
		/// <value>The selector.</value>
		internal IRibbonSelector Selector
		{
			get { return this.selector; }
			set { this.selector = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Visibility  SeparatorVisibility
		{
			get { return (Visibility )GetValue(SeparatorVisibilityProperty); }
			set { SetValue(SeparatorVisibilityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SeparatorVisibility.  This enables animation, styling, binding, etc...
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty SeparatorVisibilityProperty =
			DependencyProperty.Register("SeparatorVisibility", typeof(Visibility ), typeof(ButtonPanel), new PropertyMetadata(Visibility.Visible,new PropertyChangedCallback(OnSepratorVisibilityChanged)));

		
        internal static void OnSepratorVisibilityChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            ButtonPanel buttonpanel=obj as ButtonPanel;
            if(buttonpanel.border !=null)
            {
                buttonpanel.border.Visibility=(Visibility)e.NewValue;
            }

        }

		#endregion

		#region Fields

		private IRibbonSelector selector = null;

		#endregion
	}
}
