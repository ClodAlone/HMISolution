#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Grid.GridCellRenderer;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// GridCellModelFactory creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
    /// </summary>
    public class GridBaseCellModelFactory : IGridCellModelFactory
    {
        Dictionary<string, Type> cellTypes = new Dictionary<string,Type>();

        private Dictionary<string, Type> CustomCellTypes
        {
            get { return cellTypes; }
        }

        /// <summary>
        /// Initializes a <see cref="GridBaseCellModelFactory"/>
        /// </summary>
        public GridBaseCellModelFactory()
        {
            isDefault = false;
        }

        /// <summary>
        /// Initializes a <see cref="GridBaseCellModelFactory"/> and optionally marks it as "Default",
        /// allowing the grid to replace it with a derived factory at any time.
        /// </summary>
        /// <param name="isDefault">True if this cell model factory is default.</param>
        public GridBaseCellModelFactory(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        bool isDefault;

        /// <summary>
        /// Returns true when the grid is allowed to replace this factory with a derived factory at any time.
        /// </summary>
        public virtual bool IsDefault
        {
            get
            {
                return isDefault;
            }
        }


        /// <summary>
        /// Creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="cellTypeName">A cell type name that identifies the cell model to be instantiated.</param> 
        /// <param name="pGrid">The <see cref="GridModel"/> the new cell model object should be associated with.</param>
        /// <returns>Appropriate cell model.</returns>
        public virtual GridCellModelBase CreateCellModel(string cellTypeName, GridModel pGrid)
        {
            if (cellTypes.ContainsKey(cellTypeName))
                return (GridCellModelBase) Activator.CreateInstance(cellTypes[cellTypeName]);

            switch (cellTypeName)
            {
                case "Header":
                    return new GridStaticCellModel();

                case "Static":
                    return new GridStaticCellModel();

                case "TextBox":
                    return new GridCellTextBoxModel();

                case "TextBlock":
                    return new GridCellTextBlockModel();

                case "CheckBox":
                    return new GridCellCheckboxModel();

                case "DataTemplate":
                    return new GridCellDataTemplateModel();

                case "DataBoundTemplate":
                    return new GridCellDataBoundTemplateModel();

                case "Button":
                    return new GridCellButtonModel();

                case "FormulaCell":
                    return new GridCellFormulaModel(pGrid);

                case "MaskEdit":
                    return new GridCellMaskEditCellModel();

                case "PercentEdit":
                    return new GridCellPercentEditCellModel();

                case "DoubleEdit":
                    return new GridCellDoubleEditCellModel();

                case "RichText":
                    return new GridCellRichTextBoxCellModel();

                case "IntegerEdit":
                    return new GridCellIntegerEditCellModel();

                case "CurrencyEdit":
                    return new GridCellCurrencyEditCellModel();
                
                case "DateTimeEdit":
                    return new GridCellDateTimeEditCellModel();

                case "TimeSpanEdit":
                    return new GridCellTimeSpanEditCellModel();

                case "UpDownEdit":
                    return new GridCellUpDownCellModel();

                case "ComboBox":
                    return new GridCellComboBoxCellModel();

                case "DropDownList":
                    return new GridCellGridListControlDropDownCellModel();

                case "ImageCell":
                    return new GridCellImageCellModel();

                case "ImageContent":
                    return new GridCellModel<GridCellImageContentRenderer>();

                case "Hyperlink":
                    return new GridCellHyperlinkModel();

                default:
                    return new GridStaticCellModel();
            }
        }

    }
}
