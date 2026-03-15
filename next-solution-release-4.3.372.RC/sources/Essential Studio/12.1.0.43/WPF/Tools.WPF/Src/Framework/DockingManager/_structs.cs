// <copyright file="_structs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Identifies the place, an element can be docked to.
    /// </summary>
    /// <exclude/>
    public struct DockPreviewRecord
    {
        /// <summary>
        /// Element for docking.
        /// </summary>
        public FrameworkElement Element;

        /// <summary>
        /// Side, the element is to be docked to.
        /// </summary>
        public DockSide Side;

        /// <summary>
        /// Target, the docking will be done relative to.
        /// </summary>
        public FrameworkElement TargetElement;

        /// <summary>
        /// State, used to proceed with docking.
        /// </summary>
        public DockState State;

        /// <summary>
        /// Preview size.
        /// </summary>
        public double PreviewSize;

        /// <summary>
        /// Drag Provider Action
        /// </summary>
        public DragProviderAction Action;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Overloaded Used for comparison of 2 objects of DockPreviewRecord struct.
        /// </summary>
        /// <param name="first">First DockPreviewRecord struct.</param>
        /// <param name="second">Second DockPreviewRecord struct.</param>
        /// <returns>true if structs are equal, false otherwise.</returns>
        public static bool operator ==(DockPreviewRecord first, DockPreviewRecord second)
        {
            return first.TargetElement == second.TargetElement
                    && first.State == second.State
                    && first.Side == second.Side;
        }

        /// <summary>
        /// Overloaded Used for comparison of 2 objects of DockPreviewRecord struct.
        /// </summary>
        /// <param name="first">First DockPreviewRecord struct.</param>
        /// <param name="second">Second DockPreviewRecord struct.</param>
        /// <returns>true if structs are different, false otherwise.</returns>
        public static bool operator !=(DockPreviewRecord first, DockPreviewRecord second)
        {
            return first.TargetElement != second.TargetElement
                    || first.State != second.State
                    || first.Side != second.Side;
        }

        /// <summary>
        /// Returns the string that represents the DockPreviewRecord object. 
        /// </summary>
        /// <returns>A System.String that represents this instance of DockPreviewRecord.</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", TargetElement, Side, State);
        }
    }
}
