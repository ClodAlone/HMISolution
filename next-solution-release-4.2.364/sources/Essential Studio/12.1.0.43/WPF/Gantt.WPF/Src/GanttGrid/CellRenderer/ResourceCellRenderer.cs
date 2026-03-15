#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Gantt.Grid
{
    /// <summary>
    /// Implements the model part of a Resource cell.
    /// </summary>
    public class ResourceCellModel : GridCellModel<ResourceCellRenderer>
    {
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>
        /// True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.
        /// </returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            // This is to avoid exception on value converter,
            // It will try to conver the text to the collection type that is used in underlying source
            return false;
        }
    }

    /// <summary>
    /// Renders a TextBox control inside a grid cell.
    /// </summary>
    public class ResourceCellRenderer : GridCellTextBoxRenderer
    {
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rca">The rca.</param>
        /// <param name="style">The style.</param>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            // Sets the Cell as Readonly
            style.ReadOnly = true;
            base.OnRender(dc, rca, style);           
        }
       
        /// <summary>
        /// Initialize the Cell conent.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            // This is to block editing of this text box.
            textBox.IsReadOnly = true;
            base.OnInitializeContent(textBox, style);
        }

        /// <summary>
        /// Gets the control text core.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns></returns>
        protected override string GetControlTextCore(GridRenderStyleInfo style, object cellValue)
        {
            return this.GetResources(style);
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        string GetResources(GridRenderStyleInfo style)
        {
            string predecessors = string.Empty;

            if (!(style.CellValue is IEnumerable<Resource>))
                return string.Empty;

            foreach (Resource res in (style.CellValue as IEnumerable<Resource>))
            {
                predecessors += predecessors.Length > 0 ? ", " + res.Name : res.Name;
            }

            return predecessors;
        }
    }
}
