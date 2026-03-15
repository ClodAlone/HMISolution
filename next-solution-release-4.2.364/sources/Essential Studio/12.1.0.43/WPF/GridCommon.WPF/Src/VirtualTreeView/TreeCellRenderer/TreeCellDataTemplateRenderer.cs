#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;


namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// A renderer that manages a DataTemplate specified with <see cref="TreeStyleInfo.CellTemplateKey"/>
    /// of the <see cref="TreeRenderStyleInfo"/> inside cells. The <see cref="ContentControl.Content"/>
    /// will be the  <see cref="TreeStyleInfo.CellValue"/> which binds the DataTemplate to the value
    /// of the style.
    /// </summary>
    /// <exclude/>
    public class TreeCellDataTemplateRenderer : TreeVirtualizingCellRenderer<ContentControl>
    {
        /// <summary>
        /// Called to initialize the content of the cell 
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(ContentControl uiElement, TreeRenderStyleInfo style)
        {
            bool found = false;

            if (style.CellTemplateKey != null)
            {
                DataTemplate dt = (DataTemplate) style.VirtualTreeView.TryFindResource(style.CellTemplateKey);
                found = dt != null;
                if (found)
                    uiElement.ContentTemplate = dt;
            }
            
            if (!found)
                uiElement.ContentTemplate = style.CellTemplate;
          
            uiElement.Content = style.CellValue;
        }
    }

}
