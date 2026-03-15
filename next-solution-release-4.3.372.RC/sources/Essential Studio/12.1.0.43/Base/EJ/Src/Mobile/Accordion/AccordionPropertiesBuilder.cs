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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileAccordionPropertiesBuilder
    {
        #region Fields
        /// <summary>
        /// The m accordion
        /// </summary>
        private MobileAccordion mAccordion;
        /// <summary>
        /// Gets or sets the items collection.
        /// </summary>
        /// <value>
        /// The items collection.
        /// </value>
        internal List<MobileAccordionBaseItem> ItemsCollection { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mAccordion">The m accordion.</param>
        public MobileAccordionPropertiesBuilder(MobileAccordion mAccordion)
        {
            this.mAccordion = new MobileAccordion(mAccordion.ID, mAccordion.MobileAccordionModel);

        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mAccordion.MobileAccordionModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Theme(Theme theme)
        {
            mAccordion.MobileAccordionModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Collapsibles the specified collapsible.
        /// </summary>
        /// <param name="collapsible">if set to <c>true</c> [collapsible].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Collapsible(bool collapsible)
        {
            mAccordion.MobileAccordionModel.Collapsible = collapsible;
            return this;
        }

        /// <summary>
        /// Selecteds the index of the item.
        /// </summary>
        /// <param name="selectedItemIndex">Index of the selected item.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder SelectedItemIndex(int[] selectedItemIndex) 
        {
            mAccordion.MobileAccordionModel.SelectedItemIndex = selectedItemIndex;
            return this;
        }

        /// <summary>
        /// Heights the style.
        /// </summary>
        /// <param name="heightStyle">The height style.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder HeightStyle(HeightStyle heightStyle)
        {
            mAccordion.MobileAccordionModel.HeightStyle = heightStyle;
            return this;
        }

        /// <summary>
        /// Shows the header icon.
        /// </summary>
        /// <param name="showHeaderIcon">if set to <c>true</c> [show header icon].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder ShowHeaderIcon(bool showHeaderIcon)
        {
            mAccordion.MobileAccordionModel.ShowHeaderIcon = showHeaderIcon;
            return this;
        }

        /// <summary>
        /// Caches the specified cache.
        /// </summary>
        /// <param name="cache">if set to <c>true</c> [cache].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Cache(bool cache)
        {
            mAccordion.MobileAccordionModel.Cache = cache;
            return this;
        }

        /// <summary>
        /// Multiples the open.
        /// </summary>
        /// <param name="multipleOpen">if set to <c>true</c> [multiple open].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder MultipleOpen(bool multipleOpen)
        {
            mAccordion.MobileAccordionModel.MultipleOpen = multipleOpen;
            return this;
        }

        /// <summary>
        /// Persists the specified persist.
        /// </summary>
        /// <param name="persist">if set to <c>true</c> [persist].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Persist(bool persist)
        {
            mAccordion.MobileAccordionModel.Persist = persist;
            return this;
        }

        /// <summary>
        /// Enableds the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Enabled(bool enabled)
        {
            mAccordion.MobileAccordionModel.Enabled = enabled;
            return this;
        }

        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder CssClass(string cssClass)
        {
            mAccordion.MobileAccordionModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// Itemses the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Items(Action<MobileAccordionBaseItemAdder> item)
        {
            this.ItemsCollection = new List<MobileAccordionBaseItem>();
            MobileAccordionBaseItemAdder mAccAdder = new MobileAccordionBaseItemAdder(mAccordion.MobileAccordionModel.Items);
            item.Invoke(mAccAdder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder ClientSideEvents(Action<MobileAccordionClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileAccordionClientSideEventsBuilder(this.mAccordion.MobileAccordionModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileAccordionPropertiesBuilder Windows(Action<MobileAccordionWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileAccordionWindowsPropertiesBuilder(this.mAccordion.MobileAccordionModel);
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
            return new HtmlString(mAccordion.Render().ToString());
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
