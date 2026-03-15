//-------------------------------------------------------------------------------------------------
// <copyright file="GridDesignerMain.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Resources;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <summary>
    /// Static entry point for design editor
    /// </summary>
    [Serializable]
    public class GridDesignerMain
    {
        internal static string manifestNamespace = AssemblyInfo.RootNamespace + ".Design.Resources.";
        internal static string menuNamespace = AssemblyInfo.RootNamespace + ".Design.Actions.";
        internal static GridSyncProperties syncProps;

        internal static ResourceManager IconResources;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridDesignerMain()
            : base()
        {
        }

        /// <summary>
        ///     Initializes the design editor based on the supplied GridControl
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public static void Design(GridControl grid)
        {
            if (IconResources == null)
            {
                IconResources = new ResourceManager(manifestNamespace + "MainMenuItemResources", typeof(GridFrame).Module.Assembly, null);
            }

            GridFrame frame = new GridFrame(grid);
            frame.ShowDialog();
            frame.Dispose();
        }
    }
}
