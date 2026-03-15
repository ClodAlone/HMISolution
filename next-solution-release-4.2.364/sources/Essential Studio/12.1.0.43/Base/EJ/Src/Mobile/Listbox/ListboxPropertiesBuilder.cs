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
    public class MobileListboxPropertiesBuilder
    {
        #region Fields
        private MobileListbox mListbox;
        internal List<MobileListboxItem> ItemsCollection { get; set; }
        internal List<MobileListboxGroupItem> GroupCollection { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="MobileListbox">The mobile listbox.</param>
        public MobileListboxPropertiesBuilder(MobileListbox MobileListbox)
        {
            this.mListbox = new MobileListbox(MobileListbox.ID, MobileListbox.MobileListboxModel);
            this.ItemsCollection = new List<MobileListboxItem>();
            this.GroupCollection = new List<MobileListboxGroupItem>();
        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mListbox.MobileListboxModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Theme(Theme theme)
        {
            mListbox.MobileListboxModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Automatics the height.
        /// </summary>
        /// <param name="autoHeight">if set to <c>true</c> [automatic height].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder AutoHeight(bool autoHeight)
        {
            mListbox.MobileListboxModel.AutoHeight = autoHeight;
            return this;
        }
        /// <summary>
        /// Heights the specified height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Height(int height)
        {
            mListbox.MobileListboxModel.Height = height;
            return this;
        }
        /// <summary>
        /// Widthes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Width(int width)
        {
            mListbox.MobileListboxModel.Width = width;
            return this;
        }

        /// <summary>
        /// Selecteds the index of the item.
        /// </summary>
        /// <param name="selectedItemIndex">Index of the selected item.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder SelectedItemIndex(int selectedItemIndex)
        {
            mListbox.MobileListboxModel.SelectedItemIndex = selectedItemIndex;
            return this;
        }
        /// <summary>
        /// Headers the title.
        /// </summary>
        /// <param name="headerTitle">The header title.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder HeaderTitle(string headerTitle)
        {
            mListbox.MobileListboxModel.HeaderTitle = headerTitle;
            return this;
        }
        /// <summary>
        /// Headers the back.
        /// </summary>
        /// <param name="headerBack">if set to <c>true</c> [header back].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder HeaderBack(bool headerBack)
        {
            mListbox.MobileListboxModel.HeaderBack = headerBack;
            return this;
        }
        /// <summary>
        /// Headers the back text.
        /// </summary>
        /// <param name="headerBackText">The header back text.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder HeaderBackText(string headerBackText)
        {
            mListbox.MobileListboxModel.HeaderBackText = headerBackText;
            return this;
        }

        /// <summary>
        /// Shows the header.
        /// </summary>
        /// <param name="showHeader">if set to <c>true</c> [show header].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder ShowHeader(bool showHeader)
        {
            mListbox.MobileListboxModel.ShowHeader = showHeader;
            return this;
        }
        /// <summary>
        /// Automatics the height of the scroll.
        /// </summary>
        /// <param name="autoScrollHeight">if set to <c>true</c> [automatic scroll height].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder AutoScrollHeight(bool autoScrollHeight)
        {
            mListbox.MobileListboxModel.AutoScrollHeight = autoScrollHeight;
            return this;
        }
        /// <summary>
        /// Adjusts the fixed position.
        /// </summary>
        /// <param name="adjustFixedPosition">if set to <c>true</c> [adjust fixed position].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder AdjustFixedPosition(bool adjustFixedPosition)
        {
            mListbox.MobileListboxModel.AdjustFixedPosition = adjustFixedPosition;
            return this;
        }
        /// <summary>
        /// Natives the scroll.
        /// </summary>
        /// <param name="nativeScroll">if set to <c>true</c> [native scroll].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder NativeScroll(bool nativeScroll)
        {
            mListbox.MobileListboxModel.NativeScroll = nativeScroll;
            return this;
        }
        /// <summary>
        /// Headers the hide for un supported device.
        /// </summary>
        /// <param name="headerHideForUnSupportedDevice">if set to <c>true</c> [header hide for un supported device].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder HeaderHideForUnSupportedDevice(bool headerHideForUnSupportedDevice)
        {
            mListbox.MobileListboxModel.HeaderHideForUnSupportedDevice = headerHideForUnSupportedDevice;
            return this;
        }
        /// <summary>
        /// Groups the list.
        /// </summary>
        /// <param name="groupList">if set to <c>true</c> [group list].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder GroupList(bool groupList)
        {
            mListbox.MobileListboxModel.GroupList = groupList;
            return this;
        }
        /// <summary>
        /// Allows the scrolling.
        /// </summary>
        /// <param name="allowScrolling">if set to <c>true</c> [allow scrolling].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder AllowScrolling(bool allowScrolling)
        {
            mListbox.MobileListboxModel.AllowScrolling = allowScrolling;
            return this;
        }
        /// <summary>
        /// Caches the specified cache.
        /// </summary>
        /// <param name="cache">if set to <c>true</c> [cache].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Cache(bool cache)
        {
            mListbox.MobileListboxModel.Cache = cache;
            return this;
        }
        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="renderTemplate">if set to <c>true</c> [render template].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder RenderTemplate(bool renderTemplate)
        {
            mListbox.MobileListboxModel.RenderTemplate = renderTemplate;
            return this;
        }
        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder TemplateId(string templateId)
        {
            mListbox.MobileListboxModel.TemplateId = templateId;
            return this;
        }
        /// <summary>
        /// Persists the selection.
        /// </summary>
        /// <param name="persistSelection">if set to <c>true</c> [persist selection].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder PersistSelection(bool persistSelection)
        {
            mListbox.MobileListboxModel.PersistSelection = persistSelection;
            return this;
        }
        /// <summary>
        /// Prevents the selection.
        /// </summary>
        /// <param name="preventSelection">if set to <c>true</c> [prevent selection].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder PreventSelection(bool preventSelection)
        {
            mListbox.MobileListboxModel.PreventSelection = preventSelection;
            return this;
        }
        /// <summary>
        /// Datas the binding.
        /// </summary>
        /// <param name="dataBinding">if set to <c>true</c> [data binding].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder DataBinding(bool dataBinding)
        {
            mListbox.MobileListboxModel.DataBinding = dataBinding;
            return this;
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Query(string query)
        {
            mListbox.MobileListboxModel.Query = query;
            return this;
        }
        /// <summary>
        /// Enables the filtering.
        /// </summary>
        /// <param name="enableFiltering">if set to <c>true</c> [enable filtering].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder EnableFiltering(bool enableFiltering)
        {
            mListbox.MobileListboxModel.EnableFiltering = enableFiltering;
            return this;
        }
        /// <summary>
        /// Enables the check mark.
        /// </summary>
        /// <param name="enableCheckMark">if set to <c>true</c> [enable check mark].</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder EnableCheckMark(bool enableCheckMark)
        {
            mListbox.MobileListboxModel.EnableCheckMark = enableCheckMark;
            return this;
        }
        /// <summary>
        /// Transitions the specified transition.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Transition(string transition)
        {
            mListbox.MobileListboxModel.Transition = transition;
            return this;
        }

        /// <summary>
        /// Itemses the specified list item.
        /// </summary>
        /// <param name="listItem">The list item.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Items(Action<MobileListboxItemAdder> listItem)
        {
            this.ItemsCollection = new List<MobileListboxItem>();
            MobileListboxItemAdder mListItemAdder = new MobileListboxItemAdder(mListbox.MobileListboxModel.Items);
            listItem.Invoke(mListItemAdder);
            return this;
        }

        /// <summary>
        /// Groupses the specified group list.
        /// </summary>
        /// <param name="groupList">The group list.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Groups(Action<MobileListboxGroupItemAdder> groupList)
        {
            this.GroupCollection = new List<MobileListboxGroupItem>();

            MobileListboxGroupItemAdder mListItemAdder = new MobileListboxGroupItemAdder(mListbox.MobileListboxModel.Groups);
            groupList.Invoke(mListItemAdder);
            return this;
        }

        /// <summary>
        /// Datas the source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder DataSource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            mListbox.MobileListboxModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        /// <summary>
        /// Datasources the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder DataSource(DataSource dataSource)
        {
            mListbox.MobileListboxModel.DataSource = dataSource;
            return this;
        }
        /// <summary>
        /// Datasources the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder DataSource(IEnumerable dataSource)
        {
            mListbox.MobileListboxModel.DataSource = dataSource;
            return this;
        }

        /// <summary>
        /// Fieldses the specified fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Fields(Action<MobileListboxFieldsBuilder> fields)
        {
            var field = new MobileListboxFields();
            mListbox.MobileListboxModel.Fields = field;
            var builder = new MobileListboxFieldsBuilder(field);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }

        public MobileListboxPropertiesBuilder ClientSideEvents(Action<MobileListboxClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileListboxClientSideEventsBuilder(this.mListbox.MobileListboxModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder IOS7(Action<MobileListboxIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileListboxIOS7PropertiesBuilder(this.mListbox.MobileListboxModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified windows model.
        /// </summary>
        /// <param name="androidModel">The windows model.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder Windows(Action<MobileListboxWindowsPropertiesBuilder> androidModel)
        {
            var builder = new MobileListboxWindowsPropertiesBuilder(this.mListbox.MobileListboxModel);
            if (androidModel != null)
                androidModel.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder ContentTemplate(Action<MobileListboxProperties> template)
        {
            mListbox.MobileListboxModel.ContentTemplate.WebFormDataTemplate = template;
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder ContentTemplate(Func<MobileListboxProperties, object> template)
        {
            mListbox.MobileListboxModel.ContentTemplate.RazorViewTemplate = template;
            return this;
        }

        /// <summary>
        /// Ajaxes the options.
        /// </summary>
        /// <param name="ajaxOptions">The ajax options.</param>
        /// <returns></returns>
        public MobileListboxPropertiesBuilder AjaxOptions(Action<jQueryAjaxOptionsBuilder> ajaxOptions)
        {
            var ajaxOpt = new jQueryAjaxOptions();
            mListbox.MobileListboxModel.AjaxOptions = ajaxOpt;
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
            return new HtmlString(mListbox.Render().ToString());
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
