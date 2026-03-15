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
    ///  Class for Time Picker IOS7 Properties
    /// </summary>
    public class MobileTimePickerIOS7Properties
    {
        #region Fields

        private bool renderDefault = false;

        #endregion

        #region IOS7Properties

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
        /// Initializes a new instance of the <see cref="MobileTimePickerIOS7Properties"/> class.
        /// </summary>
        public MobileTimePickerIOS7Properties() { }

        #endregion
    }

  
    /// <summary>
    /// Class for Time Picker Windows Properties
    /// </summary>
    public class MobileTimePickerWindowsProperties : WindowsBase
    {
        #region Fields
      
        #endregion

        #region WindowsProperties
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerWindowsProperties"/> class.
        /// </summary>
        public MobileTimePickerWindowsProperties() { }

        #endregion
    }

}
