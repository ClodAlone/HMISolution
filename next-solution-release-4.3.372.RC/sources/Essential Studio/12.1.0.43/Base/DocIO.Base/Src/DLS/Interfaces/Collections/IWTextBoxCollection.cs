#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWTextBox"/>.
    /// </summary>
    public interface IWTextBoxCollection
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWTextBox"/> at the specified index.
        /// </summary>
        /// <value></value>
        IWTextBox this[int index]
        {
            get;
        }
        /// <summary>
        /// Adds a textbox to the collection.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <returns></returns>
        int Add(IWTextBox textBox);
    }
}
