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
    /// Menu Properties Builder
    /// </summary>
    public class MobileMenuPropertiesBuilder
    {
        #region Fields
        private Menu mMenu;
        internal List<MobileMenuBaseItem> ItemsCollection { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenu">The m menu.</param>
        public MobileMenuPropertiesBuilder(Menu mMenu)
        {
            this.mMenu = new Menu(mMenu.ID, mMenu.MobileMenuModel);

        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mMenu.MobileMenuModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Theme(Theme theme)
        {
            mMenu.MobileMenuModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Heights the specified height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Height(double height)
        {
            mMenu.MobileMenuModel.Height = height;
            return this;
        }

        /// <summary>
        /// Widthes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Width(double width)
        {
            mMenu.MobileMenuModel.Width = width;
            return this;
        }

        /// <summary>
        /// Allows the scrolling.
        /// </summary>
        /// <param name="allowScrolling">if set to <c>true</c> [allow scrolling].</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder AllowScrolling(bool allowScrolling)
        {
            mMenu.MobileMenuModel.AllowScrolling = allowScrolling;
            return this;
        }

        /// <summary>
        /// Targets the identifier.
        /// </summary>
        /// <param name="targetId">The target identifier.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder TargetId(string targetId)
        {
            mMenu.MobileMenuModel.TargetId = targetId;
            return this;
        }

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <param name="openMenu">The open menu.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder OpenMenu(string openMenu)
        {
            mMenu.MobileMenuModel.OpenMenu = openMenu;
            return this;
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="renderTemplate">if set to <c>true</c> [render template].</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder RenderTemplate(bool renderTemplate)
        {
            mMenu.MobileMenuModel.RenderTemplate = renderTemplate;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder TemplateId(string templateId)
        {
            mMenu.MobileMenuModel.TemplateId = templateId;
            return this;
        }

        /// <summary>
        /// Scrollbarses the specified scroll bar.
        /// </summary>
        /// <param name="scrollBar">if set to <c>true</c> [scroll bar].</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Scrollbars(bool scrollBar)
        {
            mMenu.MobileMenuModel.Scrollbars = scrollBar;
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder ClientSideEvents(Action<MobileMenuClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileMenuClientSideEventsBuilder(this.mMenu.MobileMenuModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder IOS7(Action<MobileMenuIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileMenuIOS7PropertiesBuilder(this.mMenu.MobileMenuModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified android model.
        /// </summary>
        /// <param name="androidModel">The android model.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Android(Action<MobileMenuAndroidPropertiesBuilder> androidModel)
        {
            var builder = new MobileMenuAndroidPropertiesBuilder(this.mMenu.MobileMenuModel);
            if (androidModel != null)
                androidModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Windows(Action<MobileMenuWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileMenuWindowsPropertiesBuilder(this.mMenu.MobileMenuModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Itemses the specified menu item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns></returns>
        public MobileMenuPropertiesBuilder Items(Action<MobileMenuBaseItemAdder> menuItem)
        {
            this.ItemsCollection = new List<MobileMenuBaseItem>();
            MobileMenuBaseItemAdder mMenuAdder = new MobileMenuBaseItemAdder(mMenu.MobileMenuModel.Items);
            menuItem.Invoke(mMenuAdder);
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
            return new HtmlString(mMenu.Render().ToString());
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
