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
    /// Class for MobileButtonIOS7PropertiesBuilder
    /// </summary>
    public class MobileButtonIOS7PropertiesBuilder
    {
        #region Fields
        private MobileButtonProperties MobileButtonModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileButtonModel">The mobile button model.</param>
        public MobileButtonIOS7PropertiesBuilder(MobileButtonProperties MobileButtonModel)
        {
            this.MobileButtonModel = MobileButtonModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Buttons the style.
        /// </summary>
        /// <param name="buttonStyle">The button style.</param>
        /// <returns></returns>
        public MobileButtonIOS7PropertiesBuilder ButtonStyle(IOS7ButtonStyle buttonStyle)
        {
            this.MobileButtonModel.IOS7.ButtonStyle = buttonStyle;
            return this;
        }

        /// <summary>
        /// Buttons the color.
        /// </summary>
        /// <param name="buttonColor">Color of the button.</param>
        /// <returns></returns>
        public MobileButtonIOS7PropertiesBuilder ButtonColor(IOS7ButtonColor buttonColor)
        {
            this.MobileButtonModel.IOS7.ButtonColor = buttonColor;
            return this;
        }
        #endregion

    }

    /// <summary>
    /// Class for MobileButtonAndroidPropertiesBuilder
    /// </summary>
    public class MobileButtonAndroidPropertiesBuilder
    {
        #region Fields
        private MobileButtonProperties MobileButtonModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileButtonModel">The mobile button model.</param>
        public MobileButtonAndroidPropertiesBuilder(MobileButtonProperties MobileButtonModel)
        {
            this.MobileButtonModel = MobileButtonModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Buttons the style.
        /// </summary>
        /// <param name="buttonStyle">The button style.</param>
        /// <returns></returns>
        public MobileButtonAndroidPropertiesBuilder ButtonStyle(AndroidButtonStyle buttonStyle)
        {
            this.MobileButtonModel.Android.ButtonStyle = buttonStyle;
            return this;
        }        
        #endregion

    }
    /// <summary>
    /// Class for MobileButtonWindowsPropertiesBuilder
    /// </summary>
    public class MobileButtonWindowsPropertiesBuilder
    {
        #region Fields
        private MobileButtonProperties MobileButtonModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileButtonModel">The mobile button model.</param>
        public MobileButtonWindowsPropertiesBuilder(MobileButtonProperties MobileButtonModel)
        {
            this.MobileButtonModel = MobileButtonModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Buttons the style.
        /// </summary>
        /// <param name="buttonStyle">The button style.</param>
        /// <returns></returns>
        public MobileButtonWindowsPropertiesBuilder ButtonStyle(WindowsButtonStyle buttonStyle)
        {
            this.MobileButtonModel.Windows.ButtonStyle = buttonStyle;
            return this;
        }

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileButtonWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileButtonModel.Windows.RenderDefault = renderDefault;
            return this;
        }

        #endregion

    }

    /// <summary>
    /// Class for MobileButtonFlatPropertiesBuilder
    /// </summary>
    public class MobileButtonFlatPropertiesBuilder
    {
        #region Fields
        private MobileButtonProperties MobileButtonModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonFlatPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileButtonModel">The mobile button model.</param>
        public MobileButtonFlatPropertiesBuilder(MobileButtonProperties MobileButtonModel)
        {
            this.MobileButtonModel = MobileButtonModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Buttons the style.
        /// </summary>
        /// <param name="buttonStyle">The button style.</param>
        /// <returns></returns>
        public MobileButtonFlatPropertiesBuilder ButtonStyle(FlatButtonStyle buttonStyle)
        {
            this.MobileButtonModel.Flat.ButtonStyle = buttonStyle;
            return this;
        }
        #endregion

    }

   
}
