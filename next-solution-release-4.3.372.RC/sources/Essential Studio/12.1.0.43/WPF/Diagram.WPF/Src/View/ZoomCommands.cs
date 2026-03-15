// <copyright file="ZoomCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents zoom commands.
    /// </summary>
    public static class ZoomCommands
    {
        #region Class Field

        /// <summary>
        /// Zoom in command
        /// </summary>
        private static RoutedUICommand zincommand = new RoutedUICommand("ZoomIn", "ZoomIn", typeof(ZoomCommands));
      
        /// <summary>
        /// Zoom out command
        /// </summary>
        private static RoutedUICommand zoutcommand = new RoutedUICommand("ZoomOut", "ZoomOut", typeof(ZoomCommands));

        /// <summary>
        /// Reset command
        /// </summary>
        private static RoutedUICommand resetcommand = new RoutedUICommand("Reset", "Reset", typeof(ZoomCommands));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ZoomCommands"/> class.
        /// </summary>
        static ZoomCommands()
        {
            CommandBinding zoominbinding = new CommandBinding(ZoomIn, OnZoomInCommand, CanExecuteZoomingCommands);
            CommandBinding zoomOutBinding = new CommandBinding(ZoomOut, new ExecutedRoutedEventHandler(OnZoomOutCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandBinding resetBinding = new CommandBinding(Reset, new ExecutedRoutedEventHandler(OnResetCommand), new CanExecuteRoutedEventHandler(CanExecuteZoomingCommands));
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), zoominbinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), zoomOutBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DiagramView), resetBinding);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ZoomIn RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        ///  ZoomCommands.ZoomIn.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand ZoomIn
        {
            get
            {
                return zincommand;
            }
        }

        /// <summary>
        /// Gets the ZoomOut RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        ///  ZoomCommands.ZoomOut.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand ZoomOut
        {
            get
            {
                return zoutcommand;
            }
        }

        /// <summary>
        /// Gets the Reset RoutedUICommand.
        /// </summary>
        /// Type:<see cref="RoutedUICommand"/>
        /// <example>
        /// C#:
        /// <code language="C#">
        ///  ZoomCommands.Reset.Execute(diagramView.Page, diagramView);
        /// </code>
        /// </example>
        public static RoutedUICommand Reset
        {
            get
            {
                return resetcommand;
            }
        }

        #endregion

        #region Command Handlers
        
        /// <summary>
        /// Specifies when the ZoomCommands are to be executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void CanExecuteZoomingCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Invoked when the ZoomIn Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnZoomInCommand(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as DiagramView).ZoomIn(sender as DiagramView,new Point(0,0));
        }
       
        /// <summary>
        /// Invoked when the ZoomIn Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnZoomOutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as DiagramView).ZoomOut(sender as DiagramView);
        }
       
        /// <summary>
        /// Invoked when the Reset Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnResetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as DiagramView).Reset(sender as DiagramView);
        }

        #endregion
    }
}
