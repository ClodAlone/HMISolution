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
    /// Class for TimePicker Property Builder
    /// </summary>
    public class MobileTimePickerPropertiesBuilder
    {
        #region Fields
        private TimePicker mTimePicker;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTimePicker">The m time picker.</param>
        public MobileTimePickerPropertiesBuilder(TimePicker mTimePicker)
        {
            this.mTimePicker = new TimePicker(mTimePicker.ID, mTimePicker.MTimePickerModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mTimePicker.MTimePickerModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder Theme(Theme theme)
        {
            mTimePicker.MTimePickerModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Times the format.
        /// </summary>
        /// <param name="timeFormat">The time format.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder TimeFormat(string timeFormat)
        {
            mTimePicker.MTimePickerModel.TimeFormat = timeFormat;
            return this;
        }


        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder CssClass(string cssClass)
        {
            mTimePicker.MTimePickerModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// Hours the mode.
        /// </summary>
        /// <param name="hourMode">The hour mode.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder HourMode(HourMode hourMode)
        {
            mTimePicker.MTimePickerModel.HourMode = hourMode;
            return this;
        }


        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder Enabled(bool enabled)
        {
            mTimePicker.MTimePickerModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Defaults the time.
        /// </summary>
        /// <param name="defaultTime">The default time.</param>
        /// <returns></returns>
         MobileTimePickerPropertiesBuilder DefaultTime(string defaultTime)
        {
            mTimePicker.MTimePickerModel.DefaultTime = defaultTime;
            return this;
        }

         /// <summary>
         /// Displays the default time.
         /// </summary>
         /// <param name="displayDefaultTime">if set to <c>true</c> [display default time].</param>
         /// <returns></returns>
        public MobileTimePickerPropertiesBuilder DisplayDefaultTime(bool displayDefaultTime)
        {
            mTimePicker.MTimePickerModel.DisplayDefaultTime = displayDefaultTime;
            return this;
        }


        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder IOS7(Action<MobileTimePickerIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileTimePickerIOS7PropertiesBuilder(this.mTimePicker.MTimePickerModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder Windows(Action<MobileTimePickerWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileTimePickerWindowsPropertiesBuilder(this.mTimePicker.MTimePickerModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileTimePickerPropertiesBuilder ClientSideEvents(Action<MobileTimePickerClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileTimePickerClientSideEventsBuilder(this.mTimePicker.MTimePickerModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        #endregion

        #region Render
        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mTimePicker.Render().ToString());
        }
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
