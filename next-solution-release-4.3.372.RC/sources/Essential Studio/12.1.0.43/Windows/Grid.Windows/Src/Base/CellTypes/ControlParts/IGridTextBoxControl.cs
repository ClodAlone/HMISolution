//-------------------------------------------------------------------------------------------------
// <copyright file="IGridTextBoxControl.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides interface for <see cref="GridTextBoxControl"/> and <see cref="GridOriginalTextBoxControl"/> for
    /// the text box that is shown in a <see cref="GridTextBoxCellRenderer"/> when the
    /// user starts editing the cell.
    /// </summary>
    public interface IGridTextBoxControl
    {
        /// <summary>
        /// Suspends raising events.
        /// </summary>
        void SuspendEvents();

        /// <summary>
        /// Resumes raising events.
        /// </summary>
        void ResumeEvents();

        /// <summary>
        /// Gets a value indicating whether raising events is temporarily disabled.
        /// </summary>
        bool IsSuspendEvents { get; }

        /// <summary>
        /// Suspends raising modified events until EndInit is called and set <see cref="Initializing"/> property to
        /// True. You should check <see cref="Initializing"/> in your cell renderer's implementation to see
        /// if changes in the text box are done because of initialization or user interaction.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Resume raising modified events and resets the <see cref="Initializing"/> property.
        /// </summary>
        void EndInit();

        /// <summary>
        /// Gets a value indicating whether <see cref="BeginInit"/> was called.
        /// </summary>
        bool Initializing { get; }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        GridCellRendererBase ParentCell { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the cell was floated over another cell after the
        /// user inserted text. Will be reset when the cell is redrawn.
        /// </summary>
        bool FloatDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cell supports floating over another cell.
        /// </summary>
        bool Floatable { get; set; }

        /// <summary>
        /// Gets the default text box margins.
        /// </summary>
        GridMargins TextBoxMargins { get; }
    }
}
