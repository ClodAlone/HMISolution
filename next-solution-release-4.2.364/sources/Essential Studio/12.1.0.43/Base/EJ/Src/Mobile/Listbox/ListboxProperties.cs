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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileListboxProperties : IMobileBase
    {
        #region Fields
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool autoHeight = false;
        private int height;
        private int width;
        private int selectedItemIndex = 0;
        private string headerTitle = "Title";
        private bool headerBack = false;
        private string headerBackText = string.Empty;
        private bool showHeader = true;
        private bool autoScrollHeight = true;
        private bool adjustFixedPosition = true;
        private bool nativeScroll = (UserAgent.IsAndroid() && UserAgent.IsLowerAndroid() || UserAgent.IsWindows() && UserAgent.IsMobile() || UserAgent.IsIOS7()) ? false : (UserAgent.IsDevice() || UserAgent.IsWindows() && !UserAgent.IsMobile()) ? true : false;
        private bool headerHideForUnSupportedDevice = false;
        private bool groupList = false;
        private object dataSource = new object();
        private bool allowScrolling = false;
        private bool cache = false;
        private bool renderTemplate = false;
        private string templateId = string.Empty;
        private bool persistSelection = false;
        private bool preventSelection = false;
        private bool dataBinding = false;
        private bool enableFiltering = false;
        private bool enableCheckMark = false;
        private string transition = UserAgent.IsAndroid() ? "pop" : UserAgent.IsWindows() && UserAgent.IsMobile() ? "turn" : UserAgent.IsWindows() && !UserAgent.IsMobile() ? "none" : "slide";
        private object ajaxOptions = new jQueryAjaxOptions();
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic height].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic height]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("autoHeight")]
        [DefaultValue(false)]
        public bool AutoHeight { get { return autoHeight; } set { autoHeight = value; } }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonProperty("height")]
        public int Height { get { return height; } set { height = value; } }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [JsonProperty("width")]
        public int Width { get { return width; } set { width = value; } }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        /// <value>
        /// The index of the selected item.
        /// </value>
        [JsonProperty("selectedItemIndex")]
        [DefaultValue(0)]
        public int SelectedItemIndex { get { return selectedItemIndex; } set { selectedItemIndex = value; } }

        /// <summary>
        /// Gets or sets the header title.
        /// </summary>
        /// <value>
        /// The header title.
        /// </value>
        [JsonProperty("headerTitle")]
        [DefaultValue("Title")]
        public string HeaderTitle { get { return headerTitle; } set { headerTitle = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [header back].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [header back]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("headerBack")]
        [DefaultValue(false)]
        public bool HeaderBack { get { return headerBack; } set { headerBack = value; } }

        /// <summary>
        /// Gets or sets the header back text.
        /// </summary>
        /// <value>
        /// The header back text.
        /// </value>
        [JsonProperty("headerBackText")]
        public string HeaderBackText { get { return headerBackText; } set { headerBackText = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show header].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show header]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showHeader")]
        [DefaultValue(true)]
        public bool ShowHeader { get { return showHeader; } set { showHeader = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic scroll height].
        /// </summary>
        /// <value>
        /// <c>true</c> if [automatic scroll height]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("autoScrollHeight")]
        [DefaultValue(true)]
        public bool AutoScrollHeight { get { return autoScrollHeight; } set { autoScrollHeight = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [adjust fixed position].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [adjust fixed position]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("adjustFixedPosition")]
        [DefaultValue(true)]
        public bool AdjustFixedPosition { get { return adjustFixedPosition; } set { adjustFixedPosition = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [native scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [native scroll]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("nativeScroll")]
        public bool NativeScroll { get { return nativeScroll; } set { nativeScroll = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [header hide for un supported device].
        /// </summary>
        /// <value>
        /// <c>true</c> if [header hide for un supported device]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("headerHideForUnSupportedDevice")]
        [DefaultValue(false)]
        public bool HeaderHideForUnSupportedDevice { get { return headerHideForUnSupportedDevice; } set { headerHideForUnSupportedDevice = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [group list].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [group list]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("groupList")]
        [DefaultValue(false)]
        public bool GroupList { get { return groupList; } set { groupList = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [allow scrolling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow scrolling]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowScrolling")]
        [DefaultValue(false)]
        public bool AllowScrolling { get { return allowScrolling; } set { allowScrolling = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileListboxProperties"/> is cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cache; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("cache")]
        [DefaultValue(false)]
        public bool Cache { get { return cache; } set { cache = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [render template].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render template]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("renderTemplate")]
        [DefaultValue(false)]
        public bool RenderTemplate { get { return renderTemplate; } set { renderTemplate = value; } }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        public string TemplateId { get { return templateId; } set { templateId = value; } }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonIgnore]
        public MvcTemplate<MobileListboxProperties> ContentTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [persist selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [persist selection]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persistSelection")]
        [DefaultValue(false)]
        public bool PersistSelection { get { return persistSelection; } set { persistSelection = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [prevent selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent selection]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("preventSelection")]
        [DefaultValue(false)]
        public bool PreventSelection { get { return preventSelection; } set { preventSelection = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [data binding].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data binding]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("dataBinding")]
        [DefaultValue(false)]
        public bool DataBinding { get { return dataBinding; } set { dataBinding = value; } }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        [JsonProperty("query")]
        [JsonConverter(typeof(QueryConverter))]
        public string Query { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [enable filtering].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable filtering]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enableFiltering")]
        [DefaultValue(false)]
        public bool EnableFiltering { get { return enableFiltering; } set { enableFiltering = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [enable check mark].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable check mark]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enableCheckMark")]
        [DefaultValue(false)]
        public bool EnableCheckMark { get { return enableCheckMark; } set { enableCheckMark = value; } }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>
        /// The transition.
        /// </value>
        [JsonProperty("transition")]
        public string Transition { get { return transition; } set { transition = value; } }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileListboxItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        [JsonIgnore]
        public List<MobileListboxGroupItem> Groups { get; set; }

        /// <summary>
        /// Gets or sets the load.
        /// </summary>
        /// <value>
        /// The load.
        /// </value>
        [JsonProperty("load")]
        public string Load { get; set; }

        /// <summary>
        /// Gets or sets the load complete.
        /// </summary>
        /// <value>
        /// The load complete.
        /// </value>
        [JsonProperty("loadComplete")]
        public string LoadComplete { get; set; }

        /// <summary>
        /// Gets or sets the on ajax before send.
        /// </summary>
        /// <value>
        /// The on ajax before send.
        /// </value>
        [JsonProperty("ajaxBeforeSend")]
        public string AjaxBeforeSend { get; set; }

        /// <summary>
        /// Gets or sets the on ajax load success.
        /// </summary>
        /// <value>
        /// The on ajax load success.
        /// </value>
        [JsonProperty("ajaxLoadSuccess")]
        public string AjaxLoadSuccess { get; set; }

        /// <summary>
        /// Gets or sets the on ajax load error.
        /// </summary>
        /// <value>
        /// The on ajax load error.
        /// </value>
        [JsonProperty("ajaxLoadError")]
        public string AjaxLoadError { get; set; }

        /// <summary>
        /// Gets or sets the on ajax load complete.
        /// </summary>
        /// <value>
        /// The on ajax load complete.
        /// </value>
        [JsonProperty("ajaxLoadComplete")]
        public string AjaxLoadComplete { get; set; }

        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        public string TouchStart { get; set; }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        public string TouchEnd { get; set; }

        /// <summary>
        /// Gets or sets the header button click.
        /// </summary>
        /// <value>
        /// The header button click.
        /// </value>
        [JsonProperty("headerButtonTap")]
        public string HeaderButtonTap { get; set; }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        [JsonProperty("dataSource")]
        [DefaultValue(null)]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource { get { return dataSource; } set { dataSource = value; } }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public MobileListboxFields Fields { get; set; }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileListboxIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileListboxWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the ajax options.
        /// </summary>
        /// <value>
        /// The ajax options.
        /// </value>
        [JsonProperty("ajaxOptions")]
        public object AjaxOptions
        {
            get { return this.ajaxOptions; }
            set { this.ajaxOptions = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxProperties"/> class.
        /// </summary>
        public MobileListboxProperties()
        {
            this.IOS7 = new MobileListboxIOS7Properties();
            this.Windows = new MobileListboxWindowsProperties();
            this.Items = new List<MobileListboxItem>();
            this.Groups = new List<MobileListboxGroupItem>();
            this.ContentTemplate = new MvcTemplate<MobileListboxProperties>();
        }
        #endregion

    }
}
