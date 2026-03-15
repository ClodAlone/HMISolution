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

    public class MobileHeaderAndroidProperties
    {
        #region Fields
        private string androidBackImageCss = null;
        #endregion

        #region AndroidProperties
        /// <summary>
        /// Gets or sets the android back image CSS.
        /// </summary>
        /// <value>
        /// The android back image CSS.
        /// </value>
        [JsonProperty("androidBackImageCss")]
        [DefaultValue(null)]
        public string AndroidBackImageCss { get { return androidBackImageCss; } set { androidBackImageCss = value; } }
        #endregion
    }
    public class MobileHeaderWindowsProperties : WindowsBase
    {
        #region Fields
        private string customText = "";
        #endregion

        #region WindowsProperties
        /// <summary>
        /// Gets or sets the custom text.
        /// </summary>
        /// <value>
        /// The custom text.
        /// </value>
        [JsonProperty("customText")]
        [DefaultValue("")]
        public string CustomText { get { return customText; } set { customText = value; } }
        #endregion
    }
}
