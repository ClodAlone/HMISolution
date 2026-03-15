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
using System.Collections;

namespace Syncfusion.Windows.Tools.Controls 
{

    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
  Type = typeof(Ribbon), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2003;component/Ribbon.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.VS2010;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
  Type = typeof(RibbonStatusBar), XamlResource = "/Syncfusion.Theming.Transparent;component/Ribbon.xaml")]
    public class RibbonStatusBar : ItemsControl
	{
		#region Constructor

		/// <summary>
		/// Initialize a new instance of <see cref="RibbonStatusBar"/>
		/// </summary>
		public RibbonStatusBar()
		{
			this.DefaultStyleKey = typeof(RibbonStatusBar);
		}

        /// <summary>
        /// Initializes the <see cref="RibbonStatusBar"/> class.
        /// </summary>
        static RibbonStatusBar()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
		#endregion

		#region Properties

		#region StatusItemsPane

		/// <summary>
		/// Gets or sets status pane items
		/// </summary>
		public ItemsControl StatusItemsPane
		{
			get { return (ItemsControl)GetValue(StatusItemsPaneProperty); }
			set { SetValue(StatusItemsPaneProperty, value); }
		}

		/// <summary>
        /// The identifier of the <see cref="StatusItemsPane"/> property
		/// </summary>
		public static readonly DependencyProperty StatusItemsPaneProperty = DependencyProperty.Register(
			"StatusItemsPane", 
			typeof(ItemsControl), 
			typeof(RibbonStatusBar), 
			new PropertyMetadata(new PropertyChangedCallback(StatusItemsPaneChangedCallback)));

        /// <summary>
        /// Statuses the items pane changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void StatusItemsPaneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonStatusBar)d).OnStatusItemsPaneChanged(e.OldValue as ItemsControl, e.NewValue as ItemsControl);
		}

		#endregion

		#region ControlItemsPane

		/// <summary>
		/// Gets or sets controls pane items
		/// </summary>
		public ItemsControl ControlItemsPane
		{
			get { return (ItemsControl)GetValue(ControlItemsProperty); }
			set { SetValue(ControlItemsProperty, value); }
		}

		/// <summary>
        /// The identifier of the <see cref="ControlItemsPane"/> property
		/// </summary>
		public static readonly DependencyProperty ControlItemsProperty = DependencyProperty.Register(
			"ControlItemsPane", 
			typeof(ItemsControl), 
			typeof(RibbonStatusBar), 
			new PropertyMetadata(new PropertyChangedCallback(ControlItemsPaneChangedCallback)));

        /// <summary>
        /// Controls the items pane changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ControlItemsPaneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonStatusBar)d).OnControlItemsPaneChanged(e.OldValue as ItemsControl, e.NewValue as ItemsControl);
		}

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}
		#endregion

		#region Implementation

        /// <summary>
        /// Called when [status items pane changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
		private void OnStatusItemsPaneChanged(ItemsControl oldValue, ItemsControl newValue)
		{
			
		}

        /// <summary>
        /// Called when [control items pane changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
		private void OnControlItemsPaneChanged(ItemsControl oldValue, ItemsControl newValue)
		{
			
		}

		#endregion
	}
}
