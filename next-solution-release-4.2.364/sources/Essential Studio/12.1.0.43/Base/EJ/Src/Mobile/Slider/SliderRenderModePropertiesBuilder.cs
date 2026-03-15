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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// class for Slider IOS7 Properties Builder
    /// </summary>
    public class MobileSliderIOS7PropertiesBuilder
    {
        #region Fields
        /// <summary>
        /// Gets or sets the m slider model.
        /// </summary>
        /// <value>
        /// The m slider model.
        /// </value>
        private MobileSliderProperties MSliderModel { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSliderModel">The m slider model.</param>
        public MobileSliderIOS7PropertiesBuilder(MobileSliderProperties mSliderModel)
        {
            this.MSliderModel = mSliderModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Thumbs the style.
        /// </summary>
        /// <param name="thumbStyle">The thumb style.</param>
        /// <returns></returns>
        public MobileSliderIOS7PropertiesBuilder ThumbStyle(ThumbStyle thumbStyle)
        {
            this.MSliderModel.IOS7.ThumbStyle = thumbStyle;
            return this;
        }
        #endregion

    }


    /// <summary>
    /// class for Slider Windows Properties Builder
    /// </summary>
    public class MobileSliderWindowsPropertiesBuilder
    {
        #region Fields
        /// <summary>
        /// Gets or sets the m slider model.
        /// </summary>
        /// <value>
        /// The m slider model.
        /// </value>
        private MobileSliderProperties MSliderModel { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSliderModel">The m slider model.</param>
        public MobileSliderWindowsPropertiesBuilder(MobileSliderProperties mSliderModel)
        {
            this.MSliderModel = mSliderModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileSliderWindowsPropertiesBuilder RenderDefault(bool render)
        {
            this.MSliderModel.Windows.RenderDefault = render;
            return this;
        }
        #endregion
    }

}
