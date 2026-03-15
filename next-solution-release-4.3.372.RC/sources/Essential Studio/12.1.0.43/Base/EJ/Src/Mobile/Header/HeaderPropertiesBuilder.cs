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
    public class MobileHeaderPropertiesBuilder
    {
        #region Fields
        private Header mHeader;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MHeaderPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mHeader">The m Header.</param>
        public MobileHeaderPropertiesBuilder(Header mHeader)
        {
            this.mHeader = new Header(mHeader.ID, mHeader.MobileHeaderModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mHeader.MobileHeaderModel.RenderMode = renderMode;
            return this;
        }
        /// <summary>
        /// Specifies the position.
        /// </summary>
        /// <param name="renderMode">Position</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder Position(MobileHeaderPosition position)
        {
            mHeader.MobileHeaderModel.Position = position;
            return this;
        }
        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder Theme(Theme theme)
        {
            mHeader.MobileHeaderModel.Theme = theme;
            return this;
        }
        /// <summary>
        /// Hides for un supported device.
        /// </summary>
        /// <param name="check">if set to <c>true</c> [check].</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder HideForUnSupportedDevice(bool check)
        {
            mHeader.MobileHeaderModel.HideForUnSupportedDevice = check;
            return this;
        }

        /// <summary>
        /// Left button navigation URL.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder LeftButtonNavigationUrl(string enabled)
        {
            mHeader.MobileHeaderModel.LeftButtonNavigationUrl = enabled;
            return this;
        }

        /// <summary>
        /// Right button navigation URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder RightButtonNavigationUrl(string url)
        {
            mHeader.MobileHeaderModel.RightButtonNavigationUrl = url;
            return this;
        }

        /// <summary>
        /// Set the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder Title(string title)
        {
            mHeader.MobileHeaderModel.Title = title;
            return this;
        }

        /// <summary>
        /// Shows the title.
        /// </summary>
        /// <param name="showtitle">if set to <c>true</c> [showtitle].</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder ShowTitle(bool showtitle)
        {
            mHeader.MobileHeaderModel.ShowTitle = showtitle;
            return this;
        }

        /// <summary>
        /// Left button caption.
        /// </summary>
        /// <param name="leftbuttoncaption">The leftbuttoncaption.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder LeftButtonCaption(string leftbuttoncaption)
        {
            mHeader.MobileHeaderModel.LeftButtonCaption = leftbuttoncaption;
            return this;
        }

        /// <summary>
        /// Right button caption.
        /// </summary>
        /// <param name="rightbuttoncaption">The rightbuttoncaption.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder RightButtonCaption(string rightbuttoncaption)
        {
            mHeader.MobileHeaderModel.RightButtonCaption = rightbuttoncaption;
            return this;
        }

        /// <summary>
        /// Left button style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder LeftButtonStyle(LeftButtonStyle style)
        {
            mHeader.MobileHeaderModel.LeftButtonStyle = style;
            return this;
        }

        /// <summary>
        /// Shows the left button.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder ShowLeftButton(bool value)
        {
            mHeader.MobileHeaderModel.ShowLeftButton = value;
            return this;
        }

        /// <summary>
        /// Shows the right button.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder ShowRightButton(bool value)
        {
            mHeader.MobileHeaderModel.ShowRightButton = value;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileHeaderPropertiesBuilder TemplateId(string id)
        {
            mHeader.MobileHeaderModel.TemplateID = id;
            return this;
        }
        public MobileHeaderPropertiesBuilder ClientSideEvents(Action<MobileHeaderClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileHeaderClientSideEventsBuilder(this.mHeader.MobileHeaderModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        public MobileHeaderPropertiesBuilder Windows(Action<MobileHeaderWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileHeaderWindowsPropertiesBuilder(this.mHeader.MobileHeaderModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        public MobileHeaderPropertiesBuilder Android(Action<MobileHeaderAndroidPropertiesBuilder> androidModel)
        {
            var builder = new MobileHeaderAndroidPropertiesBuilder(this.mHeader.MobileHeaderModel);
            if (androidModel != null)
                androidModel.Invoke(builder);
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
            return new HtmlString(mHeader.Render().ToString());
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
