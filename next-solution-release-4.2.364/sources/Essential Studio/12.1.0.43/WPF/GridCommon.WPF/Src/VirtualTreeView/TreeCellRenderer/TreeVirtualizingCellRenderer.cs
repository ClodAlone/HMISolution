#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;


namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// TreeVirtualizingCellRenderer is an abstract base class for cell renderers
    /// that need live UIElement visuals displayed in a cell. You can derive from
    /// this class and provide the type of the UIElement you want to show inside cells
    /// as type paramater. The class provides strong typed virtual methods for 
    /// initializing content of the cell and arranging the cell visuals. See 
    /// <see cref="TreeVirtualizingCellRendererBase{T}"/> for more details.
    /// <para/>
    /// The idea behind this class is to provide a place where we can 
    /// add general code that should be shared for all cell renderers in the tree derived
    /// from TreeVirtualizingCellRendererBase. While this class does at
    /// the moment not add meaningfull functionality to TreeVirtualizingCellRendererBase
    /// we created this extra layer of inheritance to make it easy to share 
    /// code for the TreeVirtualizingCellRendererBase base class between grid,
    /// tree and common assemblies and keep tree/grid control specific code
    /// out of the base class. It is currently not possible with C# to the base class as 
    /// template type parameter. This is the reason for this copy/paste approach for the 
    /// codebase for the base class of this class.
    /// </summary>
    /// <typeparam name="T">The type of the UIElement that should be placed inside cells</typeparam>
    public abstract class TreeVirtualizingCellRenderer<T> : TreeVirtualizingCellRendererBase<T>
        where T : FrameworkElement, new()   // FrameworkElement required for Unloaded event.
    {
        /// <summary>
        /// Wire events from uiElement
        /// </summary>
        /// <param name="uiElement"></param>
        protected override void OnWireUIElement(T uiElement)
        {
            // In grid I am wiring extra events in this corresponding class
            // that are used by all renderers derived from this class.

            //uiElement.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(uiElement_MouseDown);
            base.OnWireUIElement(uiElement);
        }

        /// <summary>
        /// Unwire previously wired events from uiElement.
        /// </summary>
        /// <param name="uiElement"></param>
        protected override void OnUnwireUIElement(T uiElement)
        {
            //uiElement.PreviewMouseDown -= new System.Windows.Input.MouseButtonEventHandler(uiElement_MouseDown);
            base.OnUnwireUIElement(uiElement);
        }

        //void uiElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //}
    }

}