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
    /// Class for DatePicker Property Builder
    /// </summary>
    public class MobileDatePickerPropertiesBuilder
    {
        #region Fields
        private DatePicker mDatePicker;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mDatePicker">The m date picker.</param>
        public MobileDatePickerPropertiesBuilder(DatePicker mDatePicker)
        {
            this.mDatePicker = new DatePicker(mDatePicker.ID, mDatePicker.MDatePickerModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mDatePicker.MDatePickerModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder Theme(Theme theme)
        {
            mDatePicker.MDatePickerModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder CssClass(string cssClass)
        {
            mDatePicker.MDatePickerModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// Dates the format.
        /// </summary>
        /// <param name="dateFormat">The date format.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder DateFormat(string dateFormat)
        {
            mDatePicker.MDatePickerModel.DateFormat = dateFormat;
            return this;
        }

        /// <summary>
        /// Minimums the date.
        /// </summary>
        /// <param name="minDate">The minimum date.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder MinDate(string minDate)
        {
            mDatePicker.MDatePickerModel.MinDate = minDate;
            return this;
        }

        /// <summary>
        /// Maximums the date.
        /// </summary>
        /// <param name="maxDate">The maximum date.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder MaxDate(string maxDate)
        {
            mDatePicker.MDatePickerModel.MaxDate = maxDate;
            return this;
        }

        /// <summary>
        /// Localizes the specified localize.
        /// </summary>
        /// <param name="localize">The localize.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder Localize(string localize)
        {
            mDatePicker.MDatePickerModel.Localize = localize;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder Enabled(bool enabled)
        {
            mDatePicker.MDatePickerModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Defaults the date.
        /// </summary>
        /// <param name="defaultDate">The default date.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder DefaultDate(string defaultDate)
        {
            mDatePicker.MDatePickerModel.DefaultDate = defaultDate;
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder IOS7(Action<MobileDatePickerIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileDatePickerIOS7PropertiesBuilder(this.mDatePicker.MDatePickerModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder Windows(Action<MobileDatePickerWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileDatePickerWindowsPropertiesBuilder(this.mDatePicker.MDatePickerModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }


        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileDatePickerPropertiesBuilder ClientSideEvents(Action<MobileDatePickerClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileDatePickerClientSideEventsBuilder(this.mDatePicker.MDatePickerModel);
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
            return new HtmlString(mDatePicker.Render().ToString());
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
