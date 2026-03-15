#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class SfGridSplitter : Control
    {
        /// <summary>
        /// Exposes test hooks to unit tests with internal access.
        /// </summary>
        private InternalTestHook _testHook;

        /// <summary>
        /// Gets a test hook for unit tests with internal access.
        /// </summary>
        internal InternalTestHook TestHook
        {
            get
            {
                if (_testHook == null)
                {
                    _testHook = new InternalTestHook(this);
                }
                return _testHook;
            }
        }

        /// <summary>
        /// Expose test hooks for internal and private members of the
        /// SfGridSplitter.
        /// </summary>
        internal class InternalTestHook
        {
            /// <summary>
            /// Reference to the outer 'parent' SfGridSplitter.
            /// </summary>
            private SfGridSplitter _gridSplitter;

            /// <summary>
            /// Initializes a new instance of the InternalTestHook class.
            /// </summary>
            /// <param name="gridSplitter">The grid splitter to hook.</param>
            internal InternalTestHook(SfGridSplitter gridSplitter)
            {
                _gridSplitter = gridSplitter;
            }

            /// <summary>
            /// Gets the SfGridSplitter's GridResizeDirection.
            /// </summary>
            internal GridResizeDirection GridResizeDirection
            {
                get { return _gridSplitter._currentGridResizeDirection; }
            }

            /// <summary>
            /// Gets the SfGridSplitter's PreviewLayer.
            /// </summary>
            internal Canvas PreviewLayer
            {
                get { return _gridSplitter._previewLayer; }
            }

            /// <summary>
            /// Gets the SfGridSplitter's ResizeData.
            /// </summary>
            internal ResizeData ResizeData
            {
                get { return _gridSplitter.ResizeDataInternal; }
            }

            /// <summary>
            /// Simulate the DragValidator's DragCompleted event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
            {
                _gridSplitter.DragValidator_DragCompletedEvent(sender, e);
            }

            /// <summary>
            /// Simulate the DragValidator's DragDelta event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
            {
                _gridSplitter.DragValidator_DragDeltaEvent(sender, e);
            }

            /// <summary>
            /// Simulate the DragValidator's DragStarted event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
            {
                _gridSplitter.DragValidator_DragStartedEvent(sender, e);
            }

            /// <summary>
            /// Simulate using the keyboard to move the splitter.
            /// </summary>
            /// <param name="horizontalChange">Horizontal change.</param>
            /// <param name="verticalChange">Vertical change.</param>
            /// <returns>
            /// A value indicating whether the splitter was moved.
            /// </returns>
            internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
            {
                return _gridSplitter.KeyboardMoveSplitter(horizontalChange, verticalChange);
            }
        }
    }
}