// <copyright file="ChartAreaCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents commands that can be invoked on <see cref="ChartArea"/>.
    /// </summary>
    /// <remarks>
    /// Commanding is an input mechanism in Windows Presentation Foundation 
    /// which provides input handling at a more semantic level than device input.
    /// </remarks>
    /// <seealso cref="ChartAreaCommands"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public static class ChartAreaCommands
    {
        #region Members
        /// <summary>
        /// Identifies ZoomIn routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_zoomIn = new RoutedUICommand("ZoomIn", "ZoomIn", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies ZoomOut routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_zoomOut = new RoutedUICommand("ZoomOut", "ZoomOut", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies ZoomReset routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_zoomReset = new RoutedUICommand("ZoomReset", "ZoomReset", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies SwitchZooming routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_switchZooming = new RoutedUICommand("SwitchZooming", "SwitchZooming", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies ChangePalette routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_changePalette = new RoutedUICommand("Palette", "Palette", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies ZoomSector routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_zoomSector = new RoutedUICommand("ZoomSector", "ZoomSector", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies CancelZooming routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_calcelZooming = new RoutedUICommand("CalcelZooming", "CalcelZooming", typeof(ChartAreaCommands));

        /// <summary>
        /// Identifies ZoomPanning routed UI command.
        /// </summary>
        private static readonly RoutedUICommand c_zoomPanning = new RoutedUICommand("ZoomPanning", "ZoomPanning", typeof(ChartAreaCommands));

        private static readonly RoutedUICommand c_changeStyle = new RoutedUICommand("ChangeStyle ", "ChangeStyle", typeof(ChartAreaCommands));
   
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the cancel zooming UI command.
        /// </summary>
        /// <remarks>
        /// When chart area is in zooming mode, this command can be used to switch off zooming mode for <see cref="ChartArea"/>.
        /// </remarks>
        /// <value>The cancel zooming <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand CancelZooming
        {
            get
            {
                return c_calcelZooming;
            }
        }

        /// <summary>
        /// Gets the zoom in routed UI command.
        /// </summary>
        /// <remarks>
        /// Command should be used to zoom in the chart area.
        /// </remarks>
        /// <value>The zoom in <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand ZoomIn
        {
            get
            {
                return c_zoomIn;
            }
        }

        /// <summary>
        /// Gets the zoom out routed UI command.
        /// </summary>
        /// <remarks>
        /// Command should be used to zoom out the chart area.
        /// </remarks>
        /// <value>The zoom out <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand ZoomOut
        {
            get
            {
                return c_zoomOut;
            }
        }

        /// <summary>
        /// Gets the zoom reset routed UI command.
        /// </summary>
        /// <remarks>
        /// Command should be used to reset zoom on the chart area.
        /// </remarks>
        /// <value>The zoom reset <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand ZoomReset
        {
            get
            {
                return c_zoomReset;
            }
        }

        /// <summary>
        /// Gets the enable zooming mode routed UI command.
        /// </summary>
        /// <remarks>
        /// Command should be used to enable zooming mode on <see cref="ChartArea"/>.
        /// </remarks>
        /// <value>The <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand SwitchZooming
        {
            get
            {
                return c_switchZooming;
            }
        }

        /// <summary>
        /// Gets the ZoomSector command. Use <see cref="ICommandSource.CommandParameter"/> for set the sector. 
        /// Type of parameter should be <see cref="Rect"/>.
        /// </summary>
        public static RoutedUICommand ZoomSector
        {
            get
            {
                return c_zoomSector;
            }
        }

        /// <summary>
        /// Gets the Change palette command. Use <see cref="ICommandSource.CommandParameter"/> for set the palette.
        /// Type of parameter should be <see cref="ChartColorPalette"/>.
        /// </summary>
        public static RoutedUICommand ChangePalette
        {
            get
            {
                return c_changePalette;
            }
        }


        /// <summary>
        /// ChangeStyle CLR property declaration 
        /// </summary>
        public static RoutedUICommand ChangeStyle
        {
            get
            {
                return c_changeStyle;
            }
        }

        /// <summary>
        /// Gets the Zoom Panning routed UI command.
        /// </summary>
        /// <remarks>
        /// Command should be used to Pan the Zoomed chart area.
        /// </remarks>
        /// <value>The Zoom Panning <see cref="RoutedUICommand"/>.</value>
        public static RoutedUICommand ZoomPanning
        {
            get
            {
                return c_zoomPanning;
            }
        }
        #endregion
    }
}
