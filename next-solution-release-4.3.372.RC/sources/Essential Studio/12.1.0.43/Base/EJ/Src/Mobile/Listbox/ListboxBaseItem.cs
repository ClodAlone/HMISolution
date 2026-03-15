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
    /// Listbox group item
    /// </summary>
    public class MobileListboxGroupItem
    {
        #region Properties
        /// <summary>
        /// Gets or sets the group items.
        /// </summary>
        /// <value>
        /// The group items.
        /// </value>
        public List<MobileListboxItem> GroupItems { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxGroupItem"/> class.
        /// </summary>
        #endregion

        #region Constructor
        public MobileListboxGroupItem()
        {
        }
        #endregion
    }
    /// <summary>
    /// Group list item builder
    /// </summary>
    public class MobileListboxGroupItemBuilder
    {
        #region Fields
        private MobileListboxGroupItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxGroupItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileListboxGroupItemBuilder(MobileListboxGroupItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Titles the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MobileListboxGroupItemBuilder Title(string title)
        {
            this.Item.Title = title;
            return this;
        }

        /// <summary>
        /// Itemses the specified list item.
        /// </summary>
        /// <param name="listItem">The list item.</param>
        /// <returns></returns>
        public MobileListboxGroupItemBuilder Items(Action<MobileListboxItemAdder> listItem)
        {
            this.Item.GroupItems = new List<MobileListboxItem>();
            MobileListboxItemAdder mListItemAdder = new MobileListboxItemAdder(this.Item.GroupItems);
            listItem.Invoke(mListItemAdder);
            return this;
        }

        #endregion
    }
    /// <summary>
    /// Group List item adder
    /// </summary>
    public class MobileListboxGroupItemAdder
    {
        #region Fields
        private List<MobileListboxGroupItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxGroupItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileListboxGroupItemAdder(List<MobileListboxGroupItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileListboxGroupItemBuilder Add()
        {
            MobileListboxGroupItem newTab = new MobileListboxGroupItem();
            this.ItemList.Add(newTab);
            return new MobileListboxGroupItemBuilder(newTab);
        }
        #endregion
    }
    /// <summary>
    /// List item
    /// </summary>
    public class MobileListboxItem
    {
        #region Fields
        private string navigateUrl = string.Empty;
        private string href = string.Empty;
        private bool loadAjaxContent = false;
        private bool preventSelection = false;
        private bool persistSelection = false;
        private string text = string.Empty;
        private bool enableCheckMark = false;
        private bool isChecked = false;
        private int primaryKey = 0;
        private string imageClass = string.Empty;
        private string imageUrl = string.Empty;
        private string childTitle = string.Empty;
        private string childButtonText = string.Empty;
        private bool renderTemplate = false;
        private string templateId = string.Empty;
        private string tStart = null;
        private string tEnd = null;
        private string htmlAttributes = null;
        List<MobileListboxItem> children = null;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the navigate URL.
        /// </summary>
        /// <value>
        /// The navigate URL.
        /// </value>
        [JsonProperty("navigateUrl")]
        public string NavigateUrl { get { return navigateUrl; } set { navigateUrl = value; } }

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
        /// Gets or sets a value indicating whether [prevent selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent selection]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("preventSelection")]
        [DefaultValue(false)]
        public bool PreventSelection { get { return preventSelection; } set { preventSelection = value; } }

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
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        public string Text { get { return text; } set { text = value; } }

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
        /// Gets or sets a value indicating whether this <see cref="MobileListboxItem"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("isChecked")]
        [DefaultValue(false)]
        public bool Checked { get { return isChecked; } set { isChecked = value; } }

        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        [JsonProperty("primaryKey")]
        [DefaultValue(0)]
        public int PrimaryKey { get { return primaryKey; } set { primaryKey = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get { return imageUrl; } set { imageUrl = value; } }

        /// <summary>
        /// Gets or sets the child title.
        /// </summary>
        /// <value>
        /// The child title.
        /// </value>
        [JsonProperty("childTitle")]
        public string ChildTitle { get { return childTitle; } set { childTitle = value; } }

        /// <summary>
        /// Gets or sets the child button text.
        /// </summary>
        /// <value>
        /// The child button text.
        /// </value>
        [JsonProperty("childButtonText")]
        public string ChildButtonText { get { return childButtonText; } set { childButtonText = value; } }

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
        /// Gets or sets the content template.
        /// </summary>
        /// <value>
        /// The content template.
        /// </value>
        [JsonIgnore]
        public MvcTemplate<MobileListboxItem> ContentTemplate { get; set; }

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
        [JsonProperty("htmlAttributes")]
        public string HtmlAttributes { get { return htmlAttributes; } set { htmlAttributes = value; } }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        [JsonProperty("children")]
        [DefaultValue(null)]
        public List<MobileListboxItem> Children { get { return children; } set { children = value; } }
        #endregion

        #region Constructor
        public MobileListboxItem()
        {
            this.Children = new List<MobileListboxItem>();
            this.ContentTemplate = new MvcTemplate<MobileListboxItem>();
        }
        #endregion
    }
    /// <summary>
    /// List item fields for databinding
    /// </summary>
    public class MobileListboxFields
    {
        #region Fields
        private string navigateUrl = "NavigateUrl";
        private string href = "Href";
        private string loadAjaxContent = "LoadAjaxContent";
        private string preventSelection = "PreventSelection";
        private string persistSelection = "PersistSelection";
        private string text = "Text";
        private string enableCheckMark = "EnableCheckMark";
        private string isChecked = "Checked";
        private string primaryKey = "PrimaryKey";
        private string parentPrimaryKey = "ParentPrimaryKey";
        private string imageClass = "ImageClass";
        private string imageUrl = "ImageUrl";
        private string childTitle = "ChildTitle";
        private string childButtonText = "ChildButtonText";
        private string renderTemplate = "RenderTemplate";
        private string templateId = "TemplateId";
        private string tStart = "TouchStart";
        private string tEnd = "TouchEnd";
        private string attributes = "Attributes";
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the navigate URL.
        /// </summary>
        /// <value>
        /// The navigate URL.
        /// </value>
        [JsonProperty("NavigateUrl")]
        [DefaultValue("NavigateUrl")]
        public string NavigateUrl { get { return navigateUrl; } set { navigateUrl = value; } }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [JsonProperty("Href")]
        [DefaultValue("Href")]
        public string Href { get { return href; } set { href = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [load ajax content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [load ajax content]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("LoadAjaxContent")]
        [DefaultValue("LoadAjaxContent")]
        public string LoadAjaxContent { get { return loadAjaxContent; } set { loadAjaxContent = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [prevent selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prevent selection]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("PreventSelection")]
        [DefaultValue("PreventSelection")]
        public string PreventSelection { get { return preventSelection; } set { preventSelection = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [persist selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [persist selection]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("PersistSelection")]
        [DefaultValue("PersistSelection")]
        public string PersistSelection { get { return persistSelection; } set { persistSelection = value; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("Text")]
        [DefaultValue("Text")]
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [enable check mark].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable check mark]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("EnableCheckMark")]
        [DefaultValue("EnableCheckMark")]
        public string EnableCheckMark { get { return enableCheckMark; } set { enableCheckMark = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileListboxItem"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("Checked")]
        [DefaultValue("Checked")]
        public string Checked { get { return isChecked; } set { isChecked = value; } }

        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        [JsonProperty("PrimaryKey")]
        [DefaultValue("PrimaryKey")]
        public string PrimaryKey { get { return primaryKey; } set { primaryKey = value; } }

        /// <summary>
        /// parentPrimaryKey
        /// </summary>
        /// <value>
        /// parentPrimaryKey
        /// </value>
        [JsonProperty("ParentPrimaryKey")]
        [DefaultValue("ParentPrimaryKey")]
        public string ParentPrimaryKey { get { return parentPrimaryKey; } set { parentPrimaryKey = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("ImageClass")]
        [DefaultValue("ImageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [JsonProperty("ImageUrl")]
        [DefaultValue("ImageUrl")]
        public string ImageUrl { get { return imageUrl; } set { imageUrl = value; } }

        /// <summary>
        /// Gets or sets the child title.
        /// </summary>
        /// <value>
        /// The child title.
        /// </value>
        [JsonProperty("ChildTitle")]
        [DefaultValue("ChildTitle")]
        public string ChildTitle { get { return childTitle; } set { childTitle = value; } }

        /// <summary>
        /// Gets or sets the child button text.
        /// </summary>
        /// <value>
        /// The child button text.
        /// </value>
        [JsonProperty("ChildButtonText")]
        [DefaultValue("ChildButtonText")]
        public string ChildButtonText { get { return childButtonText; } set { childButtonText = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [render template].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render template]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("RenderTemplate")]
        [DefaultValue("RenderTemplate")]
        public string RenderTemplate { get { return renderTemplate; } set { renderTemplate = value; } }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("TemplateId")]
        [DefaultValue("TemplateId")]
        public string TemplateId { get { return templateId; } set { templateId = value; } }

        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("TouchStart")]
        [DefaultValue("TouchStart")]
        public string TouchStart { get { return tStart; } set { tStart = value; } }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("TouchEnd")]
        [DefaultValue("TouchEnd")]
        public string TouchEnd { get { return tEnd; } set { tEnd = value; } }

        /// <summary>
        /// Gets or sets the HTML attributes.
        /// </summary>
        /// <value>
        /// The HTML attributes.
        /// </value>
        [JsonProperty("Attributes")]
        [DefaultValue("Attributes")]
        public string Attributes { get { return attributes; } set { attributes = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxFields"/> class.
        /// </summary>
        public MobileListboxFields()
        {
        }
        #endregion

    }
}
namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// List item builder
    /// </summary>
    public class MobileListboxItemBuilder
    {
        #region Fields
        private MobileListboxItem Item;
        #endregion

        #region Constructor
        public MobileListboxItemBuilder(MobileListboxItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Navigates the URL.
        /// </summary>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder NavigateUrl(string navigateUrl)
        {
            this.Item.NavigateUrl = navigateUrl;
            return this;
        }

        /// <summary>
        /// Hrefs the specified href.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder Href(string href)
        {
            this.Item.Href = href;
            return this;
        }

        /// <summary>
        /// Loads the content of the ajax.
        /// </summary>
        /// <param name="loadAjaxContent">if set to <c>true</c> [load ajax content].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder LoadAjaxContent(bool loadAjaxContent)
        {
            this.Item.LoadAjaxContent = loadAjaxContent;
            return this;
        }

        /// <summary>
        /// Prevents the selection.
        /// </summary>
        /// <param name="preventSelection">if set to <c>true</c> [prevent selection].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder PreventSelection(bool preventSelection)
        {
            this.Item.PreventSelection = preventSelection;
            return this;
        }

        /// <summary>
        /// Persists the selection.
        /// </summary>
        /// <param name="persistSelection">if set to <c>true</c> [persist selection].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder PersistSelection(bool persistSelection)
        {
            this.Item.PersistSelection = persistSelection;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        /// <summary>
        /// Enables the check mark.
        /// </summary>
        /// <param name="enableCheckMark">if set to <c>true</c> [enable check mark].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder EnableCheckMark(bool enableCheckMark)
        {
            this.Item.EnableCheckMark = enableCheckMark;
            return this;
        }

        /// <summary>
        /// Checkeds the specified check.
        /// </summary>
        /// <param name="check">if set to <c>true</c> [check].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder Checked(bool check)
        {
            this.Item.Checked = check;
            return this;
        }

        /// <summary>
        /// Primaries the key.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder PrimaryKey(int primaryKey)
        {
            this.Item.PrimaryKey = primaryKey;
            return this;
        }


        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ImageClass(string imageClass)
        {
            this.Item.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Images the URL.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ImageUrl(string imageUrl)
        {
            this.Item.ImageUrl = imageUrl;
            return this;
        }

        /// <summary>
        /// Childs the title.
        /// </summary>
        /// <param name="childTitle">The child title.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ChildTitle(string childTitle)
        {
            this.Item.ChildTitle = childTitle;
            return this;
        }

        /// <summary>
        /// Childs the button text.
        /// </summary>
        /// <param name="childButtonText">The child button text.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ChildButtonText(string childButtonText)
        {
            this.Item.ChildButtonText = childButtonText;
            return this;
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="renderTemplate">if set to <c>true</c> [render template].</param>
        /// <returns></returns>
        public MobileListboxItemBuilder RenderTemplate(bool renderTemplate)
        {
            this.Item.RenderTemplate = renderTemplate;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder TemplateId(string templateId)
        {
            this.Item.TemplateId = templateId;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder TouchStart(string tStart)
        {
            this.Item.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder TouchEnd(string tEnd)
        {
            this.Item.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// HTMLs the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder HtmlAttributes(string attributes)
        {
            this.Item.HtmlAttributes = attributes;
            return this;
        }

        /// <summary>
        /// Childrens the specified list item.
        /// </summary>
        /// <param name="listItem">The list item.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder Children(Action<MobileListboxItemAdder> listItem)
        {
            this.Item.Children = new List<MobileListboxItem>();
            MobileListboxItemAdder mListItemAdder = new MobileListboxItemAdder(this.Item.Children);
            listItem.Invoke(mListItemAdder);
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ContentTemplate(Action<MobileListboxItem> template)
        {
            this.Item.ContentTemplate.WebFormDataTemplate = template;
            return this;
        }

        /// <summary>
        /// Contents the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileListboxItemBuilder ContentTemplate(Func<MobileListboxItem, object> template)
        {
            this.Item.ContentTemplate.RazorViewTemplate = template;
            return this;
        }
        #endregion
    }
    /// <summary>
    /// List item fields builder
    /// </summary>
    public class MobileListboxFieldsBuilder
    {
        #region Fields
        private MobileListboxFields Fields;
        #endregion

        #region Constructor
        public MobileListboxFieldsBuilder(MobileListboxFields item)
        {
            this.Fields = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Navigates the URL.
        /// </summary>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder NavigateUrl(string navigateUrl)
        {
            this.Fields.NavigateUrl = navigateUrl;
            return this;
        }

        /// <summary>
        /// Hrefs the specified href.
        /// </summary>
        /// <param name="href">The href.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder Href(string href)
        {
            this.Fields.Href = href;
            return this;
        }

        /// <summary>
        /// Loads the content of the ajax.
        /// </summary>
        /// <param name="loadAjaxContent">if set to <c>true</c> [load ajax content].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder LoadAjaxContent(string loadAjaxContent)
        {
            this.Fields.LoadAjaxContent = loadAjaxContent;
            return this;
        }

        /// <summary>
        /// Prevents the selection.
        /// </summary>
        /// <param name="preventSelection">if set to <c>true</c> [prevent selection].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder PreventSelection(string preventSelection)
        {
            this.Fields.PreventSelection = preventSelection;
            return this;
        }

        /// <summary>
        /// Persists the selection.
        /// </summary>
        /// <param name="persistSelection">if set to <c>true</c> [persist selection].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder PersistSelection(string persistSelection)
        {
            this.Fields.PersistSelection = persistSelection;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder Text(string text)
        {
            this.Fields.Text = text;
            return this;
        }

        /// <summary>
        /// Enables the check mark.
        /// </summary>
        /// <param name="enableCheckMark">if set to <c>true</c> [enable check mark].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder EnableCheckMark(string enableCheckMark)
        {
            this.Fields.EnableCheckMark = enableCheckMark;
            return this;
        }

        /// <summary>
        /// Checkeds the specified check.
        /// </summary>
        /// <param name="check">if set to <c>true</c> [check].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder Checked(string check)
        {
            this.Fields.Checked = check;
            return this;
        }

        /// <summary>
        /// Primaries the key.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder PrimaryKey(string primaryKey)
        {
            this.Fields.PrimaryKey = primaryKey;
            return this;
        }
        /// <summary>
        /// Primaries the key.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder ParentPrimaryKey(string parentPrimaryKey)
        {
            this.Fields.ParentPrimaryKey = parentPrimaryKey;
            return this;
        }


        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder ImageClass(string imageClass)
        {
            this.Fields.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Images the URL.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder ImageUrl(string imageUrl)
        {
            this.Fields.ImageUrl = imageUrl;
            return this;
        }

        /// <summary>
        /// Childs the title.
        /// </summary>
        /// <param name="childTitle">The child title.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder ChildTitle(string childTitle)
        {
            this.Fields.ChildTitle = childTitle;
            return this;
        }

        /// <summary>
        /// Childs the button text.
        /// </summary>
        /// <param name="childButtonText">The child button text.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder ChildButtonText(string childButtonText)
        {
            this.Fields.ChildButtonText = childButtonText;
            return this;
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="renderTemplate">if set to <c>true</c> [render template].</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder RenderTemplate(string renderTemplate)
        {
            this.Fields.RenderTemplate = renderTemplate;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder TemplateId(string templateId)
        {
            this.Fields.TemplateId = templateId;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder TouchStart(string tStart)
        {
            this.Fields.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder TouchEnd(string tEnd)
        {
            this.Fields.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// HTMLs the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public MobileListboxFieldsBuilder Attributes(string attributes)
        {
            this.Fields.Attributes = attributes;
            return this;
        }

        #endregion
    }
    /// <summary>
    /// List item adder
    /// </summary>
    public class MobileListboxItemAdder
    {
        #region Fields
        private List<MobileListboxItem> ItemList;
        #endregion

        #region Constructor
        public MobileListboxItemAdder(List<MobileListboxItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileListboxItemBuilder Add()
        {
            MobileListboxItem newTab = new MobileListboxItem();
            this.ItemList.Add(newTab);
            return new MobileListboxItemBuilder(newTab);
        }
        #endregion
    }
    /// <summary>
    /// List item field adder
    /// </summary>
    public class MobileListboxFieldsAdder
    {
        #region Fields
        private List<MobileListboxFields> ItemList;
        #endregion

        #region Constructor
        public MobileListboxFieldsAdder(List<MobileListboxFields> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileListboxFieldsBuilder Add()
        {
            MobileListboxFields newTab = new MobileListboxFields();
            this.ItemList.Add(newTab);
            return new MobileListboxFieldsBuilder(newTab);
        }
        #endregion
    }
}