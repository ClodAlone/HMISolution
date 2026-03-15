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
    /// Class for the menu base item
    /// </summary>
    public class MobileMenuBaseItem
    {
        #region Fields

        private string id = string.Empty;
        private string text = string.Empty;
        private string href = string.Empty;
        private bool renderTemplate = false;
        private string templateId = string.Empty;
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
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("Text")]
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [JsonProperty("href")]
        public string Href { get; set; }

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
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        public string TouchStart { get { return tStart; } set { tStart = value; } }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        public string TouchEnd { get { return tEnd; } set { tEnd = value; } }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        public MobileMenuItemIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        public MobileMenuItemAndroidProperties Android { get; set; }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>
        /// The content template.
        /// </value>
        public MvcTemplate<MobileMenuBaseItem> ContentTemplate { get; set; }

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
        /// Initializes a new instance of the <see cref="MobileMenuBaseItem"/> class.
        /// </summary>
        public MobileMenuBaseItem()
        {
            this.IOS7 = new MobileMenuItemIOS7Properties();
            this.Android = new MobileMenuItemAndroidProperties();
            this.ContentTemplate = new MvcTemplate<MobileMenuBaseItem>();
        }
        #endregion

    }

    #region MenuItemIOS7
    /// <summary>
    /// Class for Menu item ios7 specific
    /// </summary>
    public class MobileMenuItemIOS7Properties
    {

        #region IOS7Properties
        /// <summary>
        /// Gets or sets the color of the button.
        /// </summary>
        /// <value>
        /// The color of the button.
        /// </value>
        [JsonProperty("buttonColor")]
        [DefaultValue(IOS7ButtonColor.Blue)]
        public IOS7ButtonColor ButtonColor { get; set; }

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        [JsonProperty("buttonStyle")]
        public IOS7ButtonStyle ButtonStyle { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        public Theme Theme { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuItemIOS7Properties"/> class.
        /// </summary>
        public MobileMenuItemIOS7Properties() { }
        #endregion
    }
    #endregion

    #region MenuItemAndroid
    /// <summary>
    /// Class for Menu item android specific
    /// </summary>
    public class MobileMenuItemAndroidProperties
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
        /// Initializes a new instance of the <see cref="MobileMenuItemAndroidProperties"/> class.
        /// </summary>
        public MobileMenuItemAndroidProperties() { }
        #endregion
    }
    #endregion

    #region MenuItemIOS7 Builder
    /// <summary>
    /// Class for Menu item ios7 specific builder
    /// </summary>
    public class MobileMenuItemIOS7PropertiesBuilder
    {
        #region Fields
        private MobileMenuItemIOS7Properties MobileMenuModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuItemIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuItemIOS7PropertiesBuilder(MobileMenuItemIOS7Properties mMenuModel)
        {
            this.MobileMenuModel = mMenuModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Buttons the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public MobileMenuItemIOS7PropertiesBuilder ButtonColor(IOS7ButtonColor color)
        {
            this.MobileMenuModel.ButtonColor = color;
            return this;
        }
        /// <summary>
        /// Buttons the style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public MobileMenuItemIOS7PropertiesBuilder ButtonStyle(IOS7ButtonStyle style)
        {
            this.MobileMenuModel.ButtonStyle = style;
            return this;
        }
        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileMenuItemIOS7PropertiesBuilder Theme(Theme theme)
        {
            this.MobileMenuModel.Theme = theme;
            return this;
        }
        #endregion

    }
    #endregion

    #region MenuItemAndroid Builder
    /// <summary>
    /// Class for Menu item Android specific builder
    /// </summary>
    public class MobileMenuItemAndroidPropertiesBuilder
    {
        #region Fields
        private MobileMenuItemAndroidProperties MobileMenuModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuItemAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuItemAndroidPropertiesBuilder(MobileMenuItemAndroidProperties mMenuModel)
        {
            this.MobileMenuModel = mMenuModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imgClass">The img class.</param>
        /// <returns></returns>
        public MobileMenuItemAndroidPropertiesBuilder ImageClass(string imgClass)
        {
            this.MobileMenuModel.ImageClass = imgClass;
            return this;
        }
        #endregion
    }
    #endregion
}

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Menu Base Builder
    /// </summary>
    public class MobileMenuBaseItemBuilder
    {
        #region Fields
        private MobileMenuBaseItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileMenuBaseItemBuilder(MobileMenuBaseItem item)
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
        public MobileMenuBaseItemBuilder Id(string id)
        {
            this.Item.Id = id;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        /// <summary>
        /// Hrefs the specified href.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder Href(string href)
        {
            this.Item.Href = href;
            return this;
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder RenderTemplate(bool render)
        {
            this.Item.RenderTemplate = render;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="tempId">The temporary identifier.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder TemplateId(string tempId)
        {
            this.Item.TemplateId = tempId;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder TouchStart(string start)
        {
            this.Item.TouchStart = start;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder TouchEnd(string end)
        {
            this.Item.TouchEnd = end;
            return this;
        }

        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder IOS7(Action<MobileMenuItemIOS7PropertiesBuilder> ios7Model)
        {
            var builder = new MobileMenuItemIOS7PropertiesBuilder(this.Item.IOS7);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Androids the specified ios7 model.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder Android(Action<MobileMenuItemAndroidPropertiesBuilder> ios7Model)
        {
            var builder = new MobileMenuItemAndroidPropertiesBuilder(this.Item.Android);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder ContentTemplate(Action<MobileMenuBaseItem> template)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = template;
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder ContentTemplate(Func<MobileMenuBaseItem, object> template)
        {
            this.Item.ContentTemplate.RazorViewTemplate = template;
            return this;
        }
        /// <summary>
        /// HTMLs the attributes.
        /// </summary>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder HtmlAttributes(string htmlAttributes)
        {
            this.Item.HtmlAttributes = htmlAttributes;
            return this;
        }

        #endregion

    }

    /// <summary>
    /// Menu Base Item Adder
    /// </summary>
    public class MobileMenuBaseItemAdder
    {
        #region Fields
        private List<MobileMenuBaseItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuBaseItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileMenuBaseItemAdder(List<MobileMenuBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileMenuBaseItemBuilder Add()
        {
            MobileMenuBaseItem newMenu = new MobileMenuBaseItem();
            this.ItemList.Add(newMenu);
            return new MobileMenuBaseItemBuilder(newMenu);
        }
        #endregion
    }
}




