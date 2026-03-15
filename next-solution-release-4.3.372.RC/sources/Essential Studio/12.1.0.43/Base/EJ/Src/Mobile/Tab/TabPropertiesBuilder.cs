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
    /// Tab Properties Builder
    /// </summary>
    public class MobileTabPropertiesBuilder
    {
        #region Fields
        private Tab mTab;
        internal List<MobileTabBaseItem> ItemsCollection { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTab">The m tab.</param>
        public MobileTabPropertiesBuilder(Tab mTab)
        {
            this.mTab = new Tab(mTab.ID, mTab.MobileTabModel);

        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mTab.MobileTabModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Theme(Theme theme)
        {
            mTab.MobileTabModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Allows the scrolling.
        /// </summary>
        /// <param name="allowScrolling">if set to <c>true</c> [allow scrolling].</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder AllowScrolling(bool allowScrolling)
        {
            mTab.MobileTabModel.AllowScrolling = allowScrolling;
            return this;
        }

        /// <summary>
        /// Loads the content of the ajax.
        /// </summary>
        /// <param name="loadAjaxContent">if set to <c>true</c> [load ajax content].</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder LoadAjaxContent(bool loadAjaxContent)
        {
            mTab.MobileTabModel.LoadAjaxContent = loadAjaxContent;
            return this;
        }

        /// <summary>
        /// Allows the cahce.
        /// </summary>
        /// <param name="allowCache">if set to <c>true</c> [allow cache].</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder AllowCahce(bool allowCache)
        {
            mTab.MobileTabModel.AllowCache = allowCache;
            return this;
        }

        /// <summary>
        /// Selecteds the index of the item.
        /// </summary>
        /// <param name="selectedItemIndex">Index of the selected item.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder SelectedItemIndex(int selectedItemIndex)
        {
            mTab.MobileTabModel.SelectedItemIndex = selectedItemIndex;
            return this;
        }

        /// <summary>
        /// Shows the badge.
        /// </summary>
        /// <param name="showBadge">if set to <c>true</c> [show badge].</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder ShowBadge(bool showBadge)
        {
            mTab.MobileTabModel.ShowBadge = showBadge;
            return this;
        }

        /// <summary>
        /// Badges the value.
        /// </summary>
        /// <param name="badgeValue">The badge value.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder BadgeValue(double badgeValue)
        {
            mTab.MobileTabModel.BadgeValue = badgeValue;
            return this;
        }

        /// <summary>
        /// Maximums the badge value.
        /// </summary>
        /// <param name="maxBadgeValue">The maximum badge value.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder MaxBadgeValue(double maxBadgeValue)
        {
            mTab.MobileTabModel.MaxBadgeValue = maxBadgeValue;
            return this;
        }

        /// <summary>
        /// Itemses the specified tab item.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Items(Action<MobileTabBaseItemAdder> tabItem)
        {
            this.ItemsCollection = new List<MobileTabBaseItem>();
            MobileTabBaseItemAdder mTabAdder = new MobileTabBaseItemAdder(mTab.MobileTabModel.Items);
            tabItem.Invoke(mTabAdder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder ClientSideEvents(Action<MobileTabClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileTabClientSideEventsBuilder(this.mTab.MobileTabModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder IOS7(Action<MobileTabIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileTabIOS7PropertiesBuilder(this.mTab.MobileTabModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified android model.
        /// </summary>
        /// <param name="androidModel">The android model.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Android(Action<MobileTabAndroidPropertiesBuilder> androidModel)
        {
            var builder = new MobileTabAndroidPropertiesBuilder(this.mTab.MobileTabModel);
            if (androidModel != null)
                androidModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Windowses the specified windows model.
        /// </summary>
        /// <param name="windowsModel">The windows model.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Windows(Action<MobileTabWindowsPropertiesBuilder> windowsModel)
        {
            var builder = new MobileTabWindowsPropertiesBuilder(this.mTab.MobileTabModel);
            if (windowsModel != null)
                windowsModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Flats the specified flat model.
        /// </summary>
        /// <param name="flatModel">The flat model.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder Flat(Action<MobileTabFlatPropertiesBuilder> flatModel)
        {
            var builder = new MobileTabFlatPropertiesBuilder(this.mTab.MobileTabModel);
            if (flatModel != null)
                flatModel.Invoke(builder);
            return this;
        }
        /// <summary>
        /// Ajaxes the options.
        /// </summary>
        /// <param name="ajaxOptions">The ajax options.</param>
        /// <returns></returns>
        public MobileTabPropertiesBuilder AjaxOptions(Action<jQueryAjaxOptionsBuilder> ajaxOptions)
        {
            var ajaxOpt = new jQueryAjaxOptions();
            mTab.MobileTabModel.AjaxOptions = ajaxOpt;
            var builder = new jQueryAjaxOptionsBuilder(ajaxOpt);
            if (ajaxOptions != null)
                ajaxOptions.Invoke(builder);
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
            return new HtmlString(mTab.Render().ToString());
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
