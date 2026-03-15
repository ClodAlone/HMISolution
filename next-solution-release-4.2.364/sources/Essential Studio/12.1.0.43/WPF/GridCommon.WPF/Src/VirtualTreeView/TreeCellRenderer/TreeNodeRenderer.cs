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
    /// A renderer for the collapsible tree node defined by the <see cref="VirtualTreeView.NodeTemplate"/>
    /// DataTemplate specified in the <see cref="VirtualTreeView"/>.
    /// </summary>
    /// <exclude/>
    public class TreeCellNodeRenderer : TreeVirtualizingCellRenderer<ContentControl>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeCellNodeRenderer"/> class.
        /// </summary>
        public TreeCellNodeRenderer()
        {
            AllowRecycle = false;
        }

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
            VirtualTreeView tc = style.VirtualTreeView;
            TreeNode node = style.TreeNode;
            DataTemplate dt;
            dt = tc.NodeTemplate;
            if (dt == null)
                dt = (DataTemplate)tc.TryFindResource("TreeNodeTemplate");

            OnUnwireUIElement(uiElement);

            uiElement.BeginInit();

            uiElement.ContentTemplate = dt;
            uiElement.Content = node;

            uiElement.EndInit();

            OnWireUIElement(uiElement);

            // do not call base - it overrides Content and ContentTemplate properties ...
            // base.InitializeContent(aca, rowIndex, columnIndex, uiElement, cellInfo);
        }
    }
}
