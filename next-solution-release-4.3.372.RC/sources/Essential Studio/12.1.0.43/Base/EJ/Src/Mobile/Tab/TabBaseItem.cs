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
using System.Threading.Tasks;
using Syncfusion.JavaScript;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for the tab base item
    /// </summary>
    public class MobileTabBaseItem
    {
        #region Fields
        private string id = string.Empty;
        private bool showBadge = false;
        private double badgeValue = 0;
        private double maxBadgeValue = 100;
        private string href = string.Empty;
        private bool loadAjaxContent = false;
        private string text = "Text";
        private string tStart = string.Empty;
        private string tEnd = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get { return id; } set { id = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show badge]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showBadge")]
        [DefaultValue(false)]
        public bool ShowBadge { get { return showBadge; } set { showBadge = value; } }

        /// <summary>
        /// Gets or sets the badge value.
        /// </summary>
        /// <value>
        /// The badge value.
        /// </value>
        [JsonProperty("badgeValue")]
        [DefaultValue(0)]
        public double BadgeValue { get { return badgeValue; } set { badgeValue = value; } }

        /// <summary>
        /// Gets or sets the maximum badge value.
        /// </summary>
        /// <value>
        /// The maximum badge value.
        /// </value>
        [JsonProperty("maxBadgeValue")]
        [DefaultValue(100)]
        public double MaxBadgeValue { get { return maxBadgeValue; } set { maxBadgeValue = value; } }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [JsonProperty("href")]
        public string Href { get { return href; } set { href = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [load ajax content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [load ajax content]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("loadAjaxContent")]
        [DefaultValue(false)]
        public bool LoadAjaxContent { get { return loadAjaxContent; } set { loadAjaxContent = value; } }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileTabItemIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        [JsonProperty("android")]
        public MobileTabItemAndroidProperties Android { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("Text")]
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonIgnore]
        public MvcTemplate<MobileTabBaseItem> Content { get; set; }

        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        [DefaultValue(null)]
        public string TouchStart { get { return tStart; } set { tStart = value; } }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        [DefaultValue(null)]
        public string TouchEnd { get { return tEnd; } set { tEnd = value; } }

        /// <summary>
        /// Gets or sets the HTML attributes.
        /// </summary>
        /// <value>
        /// The HTML attributes.
        /// </value>
        public string HtmlAttributes { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabBaseItem"/> class.
        /// </summary>
        public MobileTabBaseItem()
        {
            this.IOS7 = new MobileTabItemIOS7Properties();
            this.Android = new MobileTabItemAndroidProperties();
            this.Content = new MvcTemplate<MobileTabBaseItem>();
        }
        #endregion
    }

    #region TabItemIOS7
    /// <summary>
    /// Class for tab item ios7 specific
    /// </summary>
    public class MobileTabItemIOS7Properties
    {
        #region Fields
        private string imageClass = "ImageClass";
        #endregion

        #region IOS7Properties
        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        [DefaultValue("ImageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabItemIOS7Properties"/> class.
        /// </summary>
        public MobileTabItemIOS7Properties() { }
        #endregion
    }
    #endregion

    #region TabItemAndroid
    /// <summary>
    /// Class for tab item android specific
    /// </summary>
    public class MobileTabItemAndroidProperties
    {
        #region Fields
        private string imageClass = string.Empty;
        #endregion

        #region AndroidProperties
        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabItemAndroidProperties"/> class.
        /// </summary>
        public MobileTabItemAndroidProperties() { }
        #endregion
    }
    #endregion

    #region TabItemIOS7 Builder
    /// <summary>
    /// Class for tab item ios7 specific builder
    /// </summary>
    public class MobileTabItemIOS7PropertiesBuilder
    {
        #region Fields
        private MobileTabItemIOS7Properties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabItemIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabItemIOS7PropertiesBuilder(MobileTabItemIOS7Properties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imgClass">The img class.</param>
        /// <returns></returns>
        public MobileTabItemIOS7PropertiesBuilder ImageClass(string imgClass)
        {
            this.MobileTabModel.ImageClass = imgClass;
            return this;
        }
        #endregion

    }
    #endregion

    #region TabItemAndroid Builder
    /// <summary>
    /// Class for tab item Android specific builder
    /// </summary>
    public class MobileTabItemAndroidPropertiesBuilder
    {
        #region Fields
        private MobileTabItemAndroidProperties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabItemAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabItemAndroidPropertiesBuilder(MobileTabItemAndroidProperties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imgClass">The img class.</param>
        /// <returns></returns>
        public MobileTabItemAndroidPropertiesBuilder ImageClass(string imgClass)
        {
            this.MobileTabModel.ImageClass = imgClass;
            return this;
        }
        #endregion

    }
    #endregion
}
namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Tab Item Builder
    /// </summary>
    public class MobileTabBaseItemBuilder
    {
        #region Fields
        private MobileTabBaseItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileTabBaseItemBuilder(MobileTabBaseItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Identifiers the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Id(string id)
        {
            this.Item.Id = id;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        /// <summary>
        /// Hrefs the specified href.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Href(string href)
        {
            this.Item.Href = href;
            return this;
        }

        /// <summary>
        /// Shows the badge.
        /// </summary>
        /// <param name="showBadge">if set to <c>true</c> [show badge].</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder ShowBadge(bool showBadge)
        {
            this.Item.ShowBadge = showBadge;
            return this;
        }

        /// <summary>
        /// Badges the value.
        /// </summary>
        /// <param name="badgeValue">The badge value.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder BadgeValue(double badgeValue)
        {
            this.Item.BadgeValue = badgeValue;
            return this;
        }

        /// <summary>
        /// Maximums the badge value.
        /// </summary>
        /// <param name="maxBadgeValue">The maximum badge value.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder MaxBadgeValue(double maxBadgeValue)
        {
            this.Item.MaxBadgeValue = maxBadgeValue;
            return this;
        }

        /// <summary>
        /// Loads the content of the ajax.
        /// </summary>
        /// <param name="loadAjaxContent">if set to <c>true</c> [load ajax content].</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder LoadAjaxContent(bool loadAjaxContent)
        {
            this.Item.LoadAjaxContent = loadAjaxContent;
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder IOS7(Action<MobileTabItemIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileTabItemIOS7PropertiesBuilder(this.Item.IOS7);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified ios7 model.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Android(Action<MobileTabItemAndroidPropertiesBuilder> ios7Model)
        {
            var builder = new MobileTabItemAndroidPropertiesBuilder(this.Item.Android);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Contents the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Content(Action<MobileTabBaseItem> template)
        {
            this.Item.Content.WebFormDataTemplate = template;
            return this;
        }

        /// <summary>
        /// Contents the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Content(Func<MobileTabBaseItem, object> template)
        {
            this.Item.Content.RazorViewTemplate = template;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder TouchStart(string tStart)
        {
            this.Item.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder TouchEnd(string tEnd)
        {
            this.Item.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// HTMLs the attributes.
        /// </summary>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public MobileTabBaseItemBuilder HtmlAttributes(string htmlAttributes)
        {
            this.Item.HtmlAttributes = htmlAttributes;
            return this;
        }
        #endregion

    }

    /// <summary>
    /// Tab Item Adder
    /// </summary>
    public class MobileTabBaseItemAdder
    {
        #region Fields
        private List<MobileTabBaseItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabBaseItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileTabBaseItemAdder(List<MobileTabBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileTabBaseItemBuilder Add()
        {
            MobileTabBaseItem newTab = new MobileTabBaseItem();
            this.ItemList.Add(newTab);
            return new MobileTabBaseItemBuilder(newTab);
        }
        #endregion
    }
}


