#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if !WinRT

namespace Syncfusion.Windows.Controls.Grid
#else

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// GridCellModelFactory creates <see cref="GridCellModelBase"/> objects to be used in a <see cref="GridModel"/>.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        /// <param name="isDefault"></param>
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

                case "DataBoundTemplate":
                    return new GridCellDataTemplateCellModel();

                case "ImageCell":
                    return new GridCellImageCellModel();
                case "CheckBox":
                    return new GridCellCheckBoxCellModel();

                //case "DataTemplate":
                //    return new GridCellDataTemplateModel();

                //case "Button":
                //    return new GridCellButtonModel();

                case "FormulaCell":
                    return new GridCellFormulaModel(pGrid);
#if !WinRT

                case "MaskEdit":
                    return new GridCellMaskEditCellModel();

                case "PercentEdit":
                    return new GridCellPercentEditCellModel();

                case "DoubleEdit":
                    return new GridCellDoubleEditCellModel();

                case "IntegerEdit":
                    return new GridCellIntegerEditCellModel();

                case "CurrencyEdit":
                    return new GridCellCurrencyEditCellModel();

                case "RichText":
                    return new GridCellRichTextBoxModel();
                
                case "DateTimeEdit":
                    return new GridCellDateTimeEditCellModel();

                case "UpDownEdit":
                    return new GridCellUpDownEditCellModel();

                case "TimeSpanEdit":
                    return new GridCellTimeSpanEditCellModel();
#endif
                case "ComboBox":
                    return new GridCellComboBoxCellModel();
                case "Hyperlink":
                    return new GridCellHyperlinkModel();
#if !WinRT
                //case "DropDownList":
                //    return new GridCellGridListControlDropDownCellModel();                

                //case "ImageContent":
                //    return new GridCellModel<GridCellImageContentRenderer>();
#endif
                default:
                    return new GridCellTextBlockModel();
            }
        }

    }
}
