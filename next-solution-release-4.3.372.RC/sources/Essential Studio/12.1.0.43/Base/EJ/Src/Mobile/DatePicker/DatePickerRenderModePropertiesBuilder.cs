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
    /// Class for DatePicker IOS7 Properties Builder
    /// </summary>
    public class MobileDatePickerIOS7PropertiesBuilder
    {
        #region Fields

        /// <summary>
        /// Gets or sets the m date picker model.
        /// </summary>
        /// <value>
        /// The m date picker model.
        /// </value>
        private MobileDatePickerProperties MDatePickerModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mDatePickerModel">The m date picker model.</param>
        public MobileDatePickerIOS7PropertiesBuilder(MobileDatePickerProperties mDatePickerModel)
        {
            this.MDatePickerModel = mDatePickerModel;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Dates the type.
        /// </summary>
        /// <param name="dateType">Type of the date.</param>
        /// <returns></returns>
        public MobileDatePickerIOS7PropertiesBuilder DateType(DateType dateType)
        {
            this.MDatePickerModel.IOS7.DateType = dateType;
            return this;
        }

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileDatePickerIOS7PropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MDatePickerModel.IOS7.RenderDefault = renderDefault;
            return this;
        }
      

        
        #endregion

    }

    /// <summary>
    /// Class for DatePicker Windows Properties Builder
    /// </summary>
    public class MobileDatePickerWindowsPropertiesBuilder 
    {
        #region Fields
        
                private MobileDatePickerProperties MDatePickerModel { get; set; }

        #endregion

        #region Constructor

                /// <summary>
                /// Initializes a new instance of the <see cref="MobileDatePickerWindowsPropertiesBuilder"/> class.
                /// </summary>
                /// <param name="mDatePickerModel">The m date picker model.</param>
        public MobileDatePickerWindowsPropertiesBuilder(MobileDatePickerProperties mDatePickerModel)
        {
            this.MDatePickerModel = mDatePickerModel;
        }
     
        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileDatePickerWindowsPropertiesBuilder RenderDefault(bool render)
        {
            this.MDatePickerModel.Windows.RenderDefault = render;
            return this;
        }
        #endregion
    }

}
