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
    /// class for Slider IOS7 Specific Properties
    /// </summary>
    public class MobileSliderIOS7Properties
    {
        #region Fields
        /// <summary>
        /// The thumb style
        /// </summary>
        private ThumbStyle thumbStyle = ThumbStyle.Normal;
        #endregion

        #region IOS7Properties

        /// <summary>
        /// Gets or sets the thumb style.
        /// </summary>
        /// <value>
        /// The thumb style.
        /// </value>
        [DefaultValue(ThumbStyle.Normal)]
        public ThumbStyle ThumbStyle { get { return thumbStyle; } set { thumbStyle = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderIOS7Properties"/> class.
        /// </summary>
        public MobileSliderIOS7Properties() { }
        #endregion
    }

    /// <summary>
    /// class for Slider Windows Specific Properties
    /// </summary>
    public class MobileSliderWindowsProperties : WindowsBase
    {
        #region Fields
        #endregion

        #region WindowsProperties

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderWindowsProperties"/> class.
        /// </summary>
        public MobileSliderWindowsProperties() { }
        #endregion
    }

}
