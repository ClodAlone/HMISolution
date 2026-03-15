//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellObjectFactory.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridCellModelFactory creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
    /// </summary>
    public class GridCellModelFactory : GridBaseCellModelFactory
    {
        /// <summary>
        /// Initializes a <see cref="GridCellModelFactory"/>.
        /// </summary>
        public GridCellModelFactory()
        {
        }

        /// <summary>
        /// Creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="cellTypeName">A cell type name that identifies the cell model to be instantiated.</param> 
        /// <param name="pGrid">The <see cref="GridModel"/> the new cell model object should be associated with.</param>
        /// <returns>A cell model.</returns>
        public override GridCellModelBase CreateCellModel(string cellTypeName, GridModel pGrid)
        {
            switch (cellTypeName)
            {
                case "Header":
                    return new GridHeaderCellModel(pGrid);

                case "Static":
                    return new GridStaticCellModel(pGrid);

                case "TextBox":
                    return new GridTextBoxCellModel(pGrid);

                case "Image":
                    return new GridImageCellModel(pGrid);

                case "CheckBox":
                    return new GridCheckBoxCellModel(pGrid);

                case "PushButton":
                    return new GridPushButtonCellModel(pGrid);

                case "NumericUpDown":
                    return new GridNumericUpDownCellModel(pGrid);

                case "DropDownGrid":
                    return new GridDropDownGridCellModel(pGrid);

                case "GridListControl":
                    return new GridDropDownGridListControlCellModel(pGrid);

                case "ComboBox":
                    if (pGrid.EnableGridListControlInComboBox && !pGrid.EnableLegacyStyle)
                    {
                        return new GridDropDownGridListControlCellModel(pGrid);
                    }
                    else
                    {
                        return new GridComboBoxCellModel(pGrid);
                    }

                case "ColorEdit":
                    return new GridDropDownColorUICellModel(pGrid);

                case "MonthCalendar":
                    return new GridDropDownMonthCalendarCellModel(pGrid);

                case "FormulaCell":
                    return new GridFormulaCellModel(pGrid);

                case "Currency":
                    return new GridCurrencyTextBoxCellModel(pGrid);

                case "MaskEdit":
                    return new GridMaskEditCellModel(pGrid);

                case "RichText":
                    return new GridRichTextBoxCellModel(pGrid);

                case "Control":
                    return new GridGenericControlCellModel(pGrid);

                case "OriginalTextBox":
                    return new GridOriginalTextBoxCellModel(pGrid);

                case "ProgressBar":
                    return new GridProgressBarCellModel(pGrid);

                case "RadioButton":
                    return new GridRadioButtonCellModel(pGrid);

                case "StandardValuesCell":
                    return new GridDropDownStandardValuesCellModel(pGrid);

                case "UITypeEditorCell":
                    return new GridUITypeEditorCellModel(pGrid);

                case "PropertyGridCell":
                    return new GridPropertyGridCellModel(pGrid);

                default:
                    return new GridStaticCellModel(pGrid);
            }
        }
    }
}
