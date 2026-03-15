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
    /// Class for Time Picker IOS7 Properties builder
    /// </summary>
    public class MobileTimePickerIOS7PropertiesBuilder
    {
        #region Fields

        private MobileTimePickerProperties MTimePickerModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTimePickerModel">The m time picker model.</param>
        public MobileTimePickerIOS7PropertiesBuilder(MobileTimePickerProperties mTimePickerModel)
        {
            this.MTimePickerModel = mTimePickerModel;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileTimePickerIOS7PropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MTimePickerModel.IOS7.RenderDefault = renderDefault;
            return this;
        }



        #endregion

    }

    /// <summary>
    /// Class for Time Picker Windows Properties builder
    /// </summary>
    public class MobileTimePickerWindowsPropertiesBuilder
    {
        #region Fields

        private MobileTimePickerProperties MTimePickerModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerWindowsPropertiesBuilder" /> class.
        /// </summary>
        /// <param name="mTimePickerModel">The m time picker model.</param>
        public MobileTimePickerWindowsPropertiesBuilder(MobileTimePickerProperties mTimePickerModel)
        {
            this.MTimePickerModel = mTimePickerModel;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileTimePickerWindowsPropertiesBuilder RenderDefault(bool render)
        {
            this.MTimePickerModel.Windows.RenderDefault = render;
            return this;
        }
        #endregion
    }

}
