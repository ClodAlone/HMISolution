#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// WizardPageSelect EventArgs.
    /// </summary>
    public class WizardPageSelectEventArgs : System.EventArgs
    {
        private WizardPage page;

        /// <summary>
        /// Gets or sets the Wizard Page
        /// </summary>
        public WizardPage Page
        {
            get { return page; }
            set { page = value; }
        }
        public WizardPageSelectEventArgs(WizardPage page)
        {
            this.page = page;
        }
    }
}
