#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Gantt.Grid
{
    /// <summary>
    /// Implements the model part of a Add New Column cell.
    /// </summary>
    public class AddNewColumnCellModel : GridCellModel<AddNewColumnCellRenderer>
    {
    }

    /// <summary>
    ///  Renders a ComboBox control inside a grid cell.
    /// </summary>
    public class AddNewColumnCellRenderer:GridComboBoxCellRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewColumnCellRenderer"/> class.
        /// </summary>
        public AddNewColumnCellRenderer()
        {
            this.SupportsRenderOptimization = false;
            this.AllowRecycle = false;      
        }

        /// <summary>
        /// Gets or sets the gantt grid.
        /// </summary>
        /// <value>
        /// The gantt grid.
        /// </value>
        GanttGrid ganttGrid { get; set; }

        public override void OnInitializeContent(ComboBox comboBox, GridRenderStyleInfo style)
        {
            // To Get the Gantt grid to perform some action on it based on the combo box selection change
            // event.
            if (this.GridControl != null)
            {
                // Getting the Gantt grid
                var grid = this.GridControl.FindParentElementOfType<GridTreeControl>();
                if (grid != null)
                {
                    this.ganttGrid = grid as GanttGrid;
                }
            }

            // Getting the resource dictionary and Setting the Style of the ComboBox Drop down Renderer.
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("/Syncfusion.Gantt.Silverlight;component/GanttGrid/Themes/GanttGrid.xaml", UriKind.RelativeOrAbsolute);

            // Applying the custom style to the combo box
            Style cmboboxStyle = new Style(typeof(ComboBox));
            cmboboxStyle = rd["ComboBoxStyle1"] as Style;
            comboBox.Style = cmboboxStyle;

            base.OnInitializeContent(comboBox, style);
        }
       
        /// <summary>
        /// Called when [wire UI element].
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        protected override void OnWireUIElement(ComboBox comboBox)
        {      
            // Hooking Required events
            comboBox.DropDownOpened += new EventHandler(comboBox_DropDownChanged);
            comboBox.DropDownClosed += new EventHandler(comboBox_DropDownChanged);
            comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);        
        }

        /// <summary>
        /// In this event we have Unload the CurrentCell UIElement Because if Dynamically set itemsource means Dropdown list doesnt load so that we have Unload and reload the UIElement.
        /// </summary>
        /// <param name="comboBox"></param>
        protected override void OnUnwireUIElement(ComboBox comboBox)
        {
            // unHooks the Hooked events.
            comboBox.SelectionChanged -= new SelectionChangedEventHandler(comboBox_SelectionChanged);
            comboBox.DropDownOpened -= new EventHandler(comboBox_DropDownChanged);
            comboBox.DropDownClosed -= new EventHandler(comboBox_DropDownChanged);
        }
     
        /// <summary>
        /// Handles the DropDownChanged[Opended/closed states] event of the comboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void comboBox_DropDownChanged(object sender, EventArgs e)
        {
            ComboBox cmbo = sender as ComboBox;

            if (cmbo == null || this.ganttGrid == null)
                return;
            
            // Invalidates the Addnew Column cell.
            cmbo.Foreground = this.GetForegroundColor(this.ganttGrid, cmbo.IsDropDownOpen);
            if (!cmbo.IsDropDownOpen)
            {
                // To change the foreground color of the combo box
                this.ganttGrid.InternalGrid.InvalidateCell(GridRangeInfo.Row(0));
                this.ganttGrid.InternalGrid.InvalidateVisual(true);  
            }
        }

        /// <summary>
        /// Gets the color of the foreground.
        /// </summary>
        /// <param name="ganttGrid">The gantt grid.</param>
        /// <param name="isOpened">if set to <c>true</c> [is opened].</param>
        /// <returns></returns>
        private Brush GetForegroundColor(GanttGrid ganttGrid,bool isOpened)
        {
            // default Brush
            Brush foregroundBrush=new SolidColorBrush(Colors.Black);
            if (ganttGrid == null || ganttGrid.ParentControl==null)
                return foregroundBrush;

            // Brushes for Drop down opened state
            if (!isOpened)
            {
                if (ganttGrid.ParentControl.VisualStyle == VisualStyle.Blend || ganttGrid.ParentControl.VisualStyle == VisualStyle.Metro || ganttGrid.ParentControl.VisualStyle==VisualStyle.VS2010)
                    foregroundBrush = new SolidColorBrush(Colors.White);
                else
                    foregroundBrush = this.ganttGrid.ParentControl.GanttChart.ResourceNameForeground;
            }

            // Brushes for Drop Down closed state.
            else
            {
                if (ganttGrid.ParentControl.VisualStyle == VisualStyle.Blend)
                    return foregroundBrush;
                else
                    foregroundBrush = this.ganttGrid.ParentControl.GanttChart.ResourceNameForeground;
            }
            return foregroundBrush;
        }
       
        /// <summary>
        /// Handles the SelectionChanged event of the comboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (ganttGrid == null)
                return;

            // To add a new column to the grid based on current selected item
            ganttGrid.AddNewColumn(args.AddedItems[0].ToString());
            // Refreshing the grid to resize the columns based on newly added column
            ganttGrid.RefreshGrid();
        }
    }
}
