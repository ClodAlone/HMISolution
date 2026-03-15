#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileToolbarPropertiesBuilder
    {

        #region Fields

        private Toolbar mobileToolbar;
        internal List<MobileToolbarBaseItem> ItemsCollection { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mobileToolbar">The mobile toolbar.</param>
        public MobileToolbarPropertiesBuilder(Toolbar mobileToolbar)
        {
            this.mobileToolbar = new Toolbar(mobileToolbar.ID, mobileToolbar.MobileToolbarModel);
        }
        #endregion


        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mobileToolbar.MobileToolbarModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder TemplateId(string templateId)
        {
            mobileToolbar.MobileToolbarModel.TemplateId = templateId;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder Enabled(bool enabled)
        {
            mobileToolbar.MobileToolbarModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Hides the specified hide.
        /// </summary>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder Hide(bool hide)
        {
            mobileToolbar.MobileToolbarModel.Hide = hide;
            return this;
        }

        /// <summary>
        /// Toolbars the position.
        /// </summary>
        /// <param name="toolbarPosition">The toolbar position.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder ToolbarPosition(ToolbarPosition toolbarPosition)
        {
            mobileToolbar.MobileToolbarModel.ToolbarPosition = toolbarPosition;
            return this;
        }

        /// <summary>
        /// Itemses the specified toolbar item.
        /// </summary>
        /// <param name="toolbarItem">The toolbar item.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder Items(Action<MobileToolbarBaseItemAdder> toolbarItem)
        {
            this.ItemsCollection = new List<MobileToolbarBaseItem>();
            MobileToolbarBaseItemAdder mobileToolbarAdder = new MobileToolbarBaseItemAdder(mobileToolbar.MobileToolbarModel.Items);
            toolbarItem.Invoke(mobileToolbarAdder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileToolbarPropertiesBuilder ClientSideEvents(Action<MobileToolbarClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileToolbarClientSideEventsBuilder(this.mobileToolbar.MobileToolbarModel);
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
            return new HtmlString(mobileToolbar.Render().ToString());
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
