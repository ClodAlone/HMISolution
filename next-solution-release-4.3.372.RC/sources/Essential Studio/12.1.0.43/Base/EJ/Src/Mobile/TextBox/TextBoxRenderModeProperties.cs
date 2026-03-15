#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for MobileTextBoxWindowsProperties
    /// </summary>
    public class MobileTextBoxWindowsProperties : WindowsBase
    {
        #region Fields
        private bool showReset = true;
        #endregion

        #region WindowsProperties

        /// <summary>
        /// Gets or sets a value indicating whether [show reset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show reset]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showReset")]
        [DefaultValue(true)]
        public bool ShowReset { get { return showReset; } set { showReset = value; } }
        #endregion
    }
}
