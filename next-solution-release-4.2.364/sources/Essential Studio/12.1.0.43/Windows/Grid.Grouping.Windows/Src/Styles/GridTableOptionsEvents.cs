//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableOptionsEvents.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    //// eva GridTableOptionsChanged Syncfusion GridTableOptionsStyleInfoIdentity identity GridTableOptionsStyleInfo groupOptions StyleInfoProperty sip

    ////public delegate void GridTableOptionsChangedEventHandler(object sender, GridTableOptionsChangedEventArgs e);

    /// <summary>
    /// Provides event arguments for the <see cref="IGridGroupOptionsSource.RaiseGroupOptionsChanged"/>
    /// and <see cref="IGridGroupOptionsSource.RaiseGroupOptionsChanging"/> methods
    /// of the <see cref="IGridGroupOptionsSource"/> interface.
    /// </summary>
    public sealed class GridTableOptionsChangedEventArgs : SyncfusionEventArgs
    {
        GridTableOptionsStyleInfoIdentity identity;
        GridTableOptionsStyleInfo groupOptions;
        StyleInfoProperty sip;

        /// <summary>
        /// Initializes the object with GridTableOptionsStyleInfoIdentity, GridTableOptionsStyleInfo, and the StyleInfoProperty that were modified.
        /// </summary>
        /// <param name="identity">The identity object.</param>
        /// <param name="groupOptions">The style object.</param>
        /// <param name="sip">Specifies the property that is changed.</param>
        public GridTableOptionsChangedEventArgs(GridTableOptionsStyleInfoIdentity identity, GridTableOptionsStyleInfo groupOptions, StyleInfoProperty sip)
        {
            this.identity = identity;
            this.groupOptions = groupOptions;
            this.sip = sip;
        }

        /// <summary>
        /// The identity object.
        /// </summary>
        [TraceProperty(true)]
        public GridTableOptionsStyleInfoIdentity Identity
        {
            get
            {
                return identity;
            }
        }

        /// <summary>
        /// The style object.
        /// </summary>
        [TraceProperty(true)]
        public GridTableOptionsStyleInfo GroupOptions
        {
            get
            {
                return groupOptions;
            }
        }

        /// <summary>
        /// Specifies the property that is changed.
        /// </summary>
        [TraceProperty(true)]
        public StyleInfoProperty Sip
        {
            get
            {
                return sip;
            }
        }
    }
}
