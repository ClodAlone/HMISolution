#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// For internal use.
    /// </summary>
    [TemplatePart(Name = GridCellsControl.TemplateGrid, Type = typeof(GridControl))]
    public class GridCellsControl : ContentControl
    {
        public const string TemplateGrid = "PART_GridControl";

        static GridCellsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCellsControl), new FrameworkPropertyMetadata(typeof(GridCellsControl)));
            /*CommandManager.RegisterClassCommandBinding(typeof(GridCellsControl), new CommandBinding(EndEditCommand, new ExecutedRoutedEventHandler(OnExecutedEndEdit), new CanExecuteRoutedEventHandler(OnCanExecuteEndEdit)));*/
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public GridCellsControl()
        {
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public GridControl InternalGrid
        {
            get;
            set;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InternalGrid = this.GetTemplateChild(GridCellsControl.TemplateGrid) as GridControl;
        }

    }
}