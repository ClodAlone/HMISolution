//-------------------------------------------------------------------------------------------------
// <copyright file="GridStyleInfoSubObjectIdentity.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides style identity information for nested objects of the <see cref="GridStyleInfo"/> class.
    /// </summary>
    public class GridStyleInfoSubObjectIdentity : CachedStyleInfoSubObjectIdentity
    {
        /// <summary>
        /// Creates a new <see cref="GridStyleInfoSubObjectIdentity"/> object and associates it with a <see cref="GridStyleInfo"/>.
        /// </summary>
        /// <param name="owner">The <see cref="StyleInfoBase"/> that owns this subobject.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> descriptor for this expandable subobject.</param>
        public GridStyleInfoSubObjectIdentity(StyleInfoBase owner, StyleInfoProperty sip)
            : base(owner, sip)
        {
        }
    }
}
