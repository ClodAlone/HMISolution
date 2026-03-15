#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// The cell render style information for a cell inside the <see cref="VirtualTreeView"/>.
    /// TreeRenderStyleInfo is created at runtime before cells are arranged in the 
    /// tree control and you can change its settings with the <see cref="Syncfusion.Windows.Controls.VirtualTreeView.VirtualTreeView.PrepareRenderCell"/>
    /// event of the tree control. Changes made to this style's properties will only be made
    /// for rendering the style and will not be commited back to the tree node.<para/>
    /// TreeRenderStyleInfo is bound to the <see cref="VirtualTreeView"/> and provides
    /// a public property to the get access to it. The base class <see cref="TreeStyleInfo"/>
    /// provides properties to access also the TreeNode, TreeColumn, TreeLevel and 
    /// TreeModel the style is bound to.<para/>
    /// When making changes to the style that should be written back to the data store
    /// you should make the changes in the <see cref="TreeRenderStyleInfo.ModelStyle"/>.
    /// TreeRenderStyleInfo.ModelStyle will fire Changed notifications that a TreeNode
    /// can then react to (or ignore if no mechanism for saving changes back has 
    /// been implemented.)
    /// </summary>
    public class TreeRenderStyleInfo : TreeStyleInfo
    {
        VirtualTreeView treeControl;
        TreeStyleInfo modelStyle;
        
        #region Ctor
        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="TreeStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="treeControl">The tree control.</param>
        /// <param name="modelStyle">The model style.</param>
        public TreeRenderStyleInfo(VirtualTreeView treeControl, TreeStyleInfo modelStyle)
            : base(new TreeStyleInfoIdentity((TreeStyleInfoIdentity) modelStyle.Identity), (TreeStyleInfoStore)modelStyle.Store.Clone())
        {
            this.CacheValues = true;
            this.modelStyle = modelStyle;
            this.treeControl = treeControl;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the tree control.
        /// </summary>
        /// <value>The tree control.</value>
        public VirtualTreeView VirtualTreeView
        {
            get { return treeControl; }
        }

        /// <summary>
        /// Gets the underlying model style. Use it to apply changes to style 
        /// that should be written back to the data store (.e.g. in grid textbox: modelstyle.cellvalue = ...)
        /// TreeRenderStyleInfo.ModelStyle will fire Changed notifications that a TreeNode
        /// can then react to (or ignore if no mechanism for saving changes back has 
        /// been implemented.)
        /// </summary>
        public TreeStyleInfo ModelStyle
        {
            get { return modelStyle; }
        }

        #endregion
    }

}
