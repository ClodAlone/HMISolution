#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;

    /// <summary>
    /// Specifies the custom cell types
    /// </summary>
    public enum CustomCellTypes
    {
        /// <summary>
        /// Represent ButtonEdit cell type
        /// </summary>
        ButtonEdit,

        /// <summary>
        /// Represent Calendar cell type
        /// </summary>
        Calendar,

        /// <summary>
        /// Represent DateTimePicker cell type
        /// </summary>
        DateTimePicker,

        /// <summary>
        /// Represent Calculate TextBox cell type
        /// </summary>
        CalculatorTextBox,

        /// <summary>
        /// Represent LinkLabel cell type
        /// </summary>
        LinkLabelCell,

        /// <summary>
        /// Represent Grid cell type
        /// </summary>
        GridinCell,

        /// <summary>
        /// Represent PictureBox cell type
        /// </summary>
        PictureBox,

        /// <summary>
        /// Represent NumericUpDown cell type
        /// </summary>
        FNumericUpDown,

        /// <summary>
        /// Represent XHTML cell type
        /// </summary>
        XhtmlCell,

        /// <summary>
        /// Represent IntegerTextBox cell type
        /// </summary>
        IntegerTextBox,

        /// <summary>
        /// Represent PerCentTextBox cell type
        /// </summary>
        PercentTextBox,

        /// <summary>
        /// Represent DoubleTextBox cell type
        /// </summary>        
        DoubleTextBox,

        /// <summary>
        /// Represent OLE Container cell type
        /// </summary>        
        OleContainerCell
    }

    /// <summary>
    /// RegisterCellModel will include the cell model of some custom cell types to the Grid
    /// </summary>
    public class RegisterCellModel
    {
        private static GridModel _Grid;

        /// <summary>
        /// Intializes a new <see cref="RegisterCellModel"/>
        /// </summary>
        public RegisterCellModel()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the GridModel.
        /// </summary>
        public static GridModel Grid
        {
            get
            {
                return _Grid;
            }

            set
            {
                _Grid = value;
            }
        }

        /// <summary>
        /// GridCellType is used to register some custom cell type to GridControlwhich are in the CustomCellTypes enum.
        /// </summary>
        /// <param name="Grid">Grid control on which the cell type has to be registered</param>
        /// <param name="res">The name of the cell type</param>
        public static void GridCellType(GridControl Grid, CustomCellTypes res)
        {
            RegisterCellModel.Grid = Grid.Model;
            ModelType(res);
        }

        /// <summary>
        /// GridCellType is used to register some custom cell type to GridDataBoundGrid control which are in the CustomCellTypes enum.
        /// </summary>
        /// <param name="Grid">GridDataBoundGrid on which the cell type has to be registered</param>
        /// <param name="res">The name of the cell type</param>
        public static void GridDataBoundCellType(GridDataBoundGrid Grid, CustomCellTypes res)
        {
            RegisterCellModel.Grid = Grid.Model;
            ModelType(res);
        }
        // public static void GridCellType(GridControl Grid, GridControl EmbedGrid, String res)
        // {
        //    DropDownGridCellModel model = new DropDownGridCellModel(Grid.Model);
        //    model.EmbeddedGrid = EmbedGrid;
        //    Grid.CellModels.Add(res.ToString(), model);
        // }

        /// <summary>
        /// GridGroupingCellType is used to reegister some custom cell type to GridGroupingControl which are in the CustomCellTypes enum.
        /// </summary>
        /// <param name="Grid">GridGroupingcontrol on which the cell type has to be registered</param>
        /// <param name="res">The name of the cell type</param>
        public static void GridGroupingCellType(GridGroupingControl Grid, CustomCellTypes res)
        {
            RegisterCellModel.Grid = Grid.TableModel;
            ModelType(res);
        }

        private static void ModelType(CustomCellTypes res)
        {
            switch (res)
            {
                case CustomCellTypes.ButtonEdit:
                    Grid.CellModels.Add(res.ToString(), new ButtonEditCellModel(Grid.Model));
                    break;
                case CustomCellTypes.CalculatorTextBox:
                    Grid.CellModels.Add(res.ToString(), new DropDownCalculatorTextBoxCellModel(Grid.Model));
                    break;
                case CustomCellTypes.Calendar:
                    Grid.CellModels.Add(res.ToString(), new CalendarCellModel(Grid.Model));
                    break;
                case CustomCellTypes.DateTimePicker:
                    Grid.CellModels.Add(res.ToString(), new DateTimeCellModel(Grid.Model));
                    break;
                case CustomCellTypes.FNumericUpDown:
                    Grid.CellModels.Add(res.ToString(), new FNumericUpDownCellModel(Grid.Model));
                    break;
                case CustomCellTypes.GridinCell:
                    Grid.CellModels.Add(res.ToString(), new GridInCellModel(Grid.Model));
                    break;
                case CustomCellTypes.LinkLabelCell:
                    Grid.CellModels.Add(res.ToString(), new LinkLabelCellModel(Grid.Model));
                    break;
                case CustomCellTypes.PictureBox:
                    Grid.CellModels.Add(res.ToString(), new PictureBoxCellModel(Grid.Model));
                    break;
                case CustomCellTypes.XhtmlCell:
                    Grid.CellModels.Add(res.ToString(), new XhtmlCellModel(Grid.Model));
                    break;
                case CustomCellTypes.IntegerTextBox:
                    Grid.CellModels.Add(res.ToString(), new IntegerTextBoxCellModel(Grid.Model));
                    break;
                case CustomCellTypes.PercentTextBox:
                    Grid.CellModels.Add(res.ToString(), new PercentTextBoxCellModel(Grid.Model));
                    break;
                case CustomCellTypes.DoubleTextBox:
                    Grid.CellModels.Add(res.ToString(), new DoubleTextBoxCellModel(Grid.Model));
                    break;
                case CustomCellTypes.OleContainerCell:
                    Grid.CellModels.Add(res.ToString(), new OleContainerCellModel(Grid.Model));
                    break;
            }
        }
    }
}
