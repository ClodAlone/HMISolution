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
    public class MobileFooterPropertiesBuilder
    {
        #region Fields
        private Footer mFooter;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MFooterPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mFooter">The m Footer.</param>
        public MobileFooterPropertiesBuilder(Footer mFooter)
        {
            this.mFooter = new Footer(mFooter.ID, mFooter.MobileFooterModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mFooter.MobileFooterModel.RenderMode = renderMode;
            return this;
        }
        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder Theme(Theme theme)
        {
            mFooter.MobileFooterModel.Theme = theme;
            return this;
        }
        /// <summary>
        /// Hides for un supported device.
        /// </summary>
        /// <param name="check">if set to <c>true</c> [check].</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder HideForUnSupportedDevice(bool check)
        {
            mFooter.MobileFooterModel.HideForUnSupportedDevice = check;
            return this;
        }

        /// <summary>
        /// Left button navigation URL.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder LeftButtonNavigationUrl(string enabled)
        {
            mFooter.MobileFooterModel.LeftButtonNavigationUrl = enabled;
            return this;
        }

        /// <summary>
        /// Right button navigation URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder RightButtonNavigationUrl(string url)
        {
            mFooter.MobileFooterModel.RightButtonNavigationUrl = url;
            return this;
        }

        /// <summary>
        /// Set the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder Title(string title)
        {
            mFooter.MobileFooterModel.Title = title;
            return this;
        }

        /// <summary>
        /// Shows the title.
        /// </summary>
        /// <param name="showtitle">if set to <c>true</c> [showtitle].</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder ShowTitle(bool showtitle)
        {
            mFooter.MobileFooterModel.ShowTitle = showtitle;
            return this;
        }

        /// <summary>
        /// Left button caption.
        /// </summary>
        /// <param name="leftbuttoncaption">The leftbuttoncaption.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder LeftButtonCaption(string leftbuttoncaption)
        {
            mFooter.MobileFooterModel.LeftButtonCaption = leftbuttoncaption;
            return this;
        }

        /// <summary>
        /// Right button caption.
        /// </summary>
        /// <param name="rightbuttoncaption">The rightbuttoncaption.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder RightButtonCaption(string rightbuttoncaption)
        {
            mFooter.MobileFooterModel.RightButtonCaption = rightbuttoncaption;
            return this;
        }

        /// <summary>
        /// Shows the left button.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder ShowLeftButton(bool value)
        {
            mFooter.MobileFooterModel.ShowLeftButton = value;
            return this;
        }

        /// <summary>
        /// Shows the right button.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder ShowRightButton(bool value)
        {
            mFooter.MobileFooterModel.ShowRightButton = value;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileFooterPropertiesBuilder TemplateId(string id)
        {
            mFooter.MobileFooterModel.TemplateID = id;
            return this;
        }
        public MobileFooterPropertiesBuilder ClientSideEvents(Action<MobileFooterClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileFooterClientSideEventsBuilder(this.mFooter.MobileFooterModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        public MobileFooterPropertiesBuilder Windows(Action<MobileFooterWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileFooterWindowsPropertiesBuilder(this.mFooter.MobileFooterModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
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
            return new HtmlString(mFooter.Render().ToString());
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
