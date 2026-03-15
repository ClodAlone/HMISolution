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
using System.Runtime.Serialization;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for DatePicker IOS7 Properties
    /// </summary>
    public class MobileDatePickerIOS7Properties
    {
        #region Fields

        private bool renderDefault = false;
        private DateType dateType = DateType.Date;

        #endregion

        #region IOS7Properties

        /// <summary>
        /// Gets or sets the type of the date.
        /// </summary>
        /// <value>
        /// The type of the date.
        /// </value>
        [JsonProperty("dateType")]
        [DefaultValue(DateType.Date)]
        public DateType DateType { get { return dateType; } set { dateType = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [render default].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render default]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("renderDefault")]
        [DefaultValue(false)]
        public bool RenderDefault { get { return renderDefault; } set { renderDefault = value; } }

        #endregion
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerIOS7Properties"/> class.
        /// </summary>
        public MobileDatePickerIOS7Properties() { }

        #endregion
    }


    /// <summary>
    /// Class for DatePicker Windows Properties
    /// </summary>
    public class MobileDatePickerWindowsProperties : WindowsBase
    {
        #region Fields
      
        #endregion

        #region WindowsProperties
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerWindowsProperties"/> class.
        /// </summary>
        public MobileDatePickerWindowsProperties() { }

        #endregion
    }

}
