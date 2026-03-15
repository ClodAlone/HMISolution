#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileListboxIOS7Properties
    {
        #region Fields
        private bool inline = false;
        #endregion

        #region IOS7Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileListboxIOS7Properties"/> is inline.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inline; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool Inline { get { return inline; } set { inline = value; } }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxIOS7Properties"/> class.
        /// </summary>
        public MobileListboxIOS7Properties() { }
        #endregion
    }
    public class MobileListboxWindowsProperties : WindowsBase
    {
        #region Fields
        private bool preventSkewing = false;
        #endregion

        #region WindowsProperties

        /// <summary>
        /// Gets or sets a value indicating whether [prevent skewing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent skewing]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool PreventSkewing { get { return preventSkewing; } set { preventSkewing = value; } }

        #endregion

        #region Contructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxWindowsProperties"/> class.
        /// </summary>
        public MobileListboxWindowsProperties() { }
        #endregion
    }
}
