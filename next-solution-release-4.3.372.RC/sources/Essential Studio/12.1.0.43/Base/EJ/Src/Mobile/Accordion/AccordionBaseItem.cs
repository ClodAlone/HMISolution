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
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for AccordionBaseItem
    /// </summary>
    public class MobileAccordionBaseItem
    {
        #region Fields


        protected string logoClass = "LogoClass";
        protected string text = "Text";
        protected string ajaxUrl = "AjaxUrl";


        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logo class.
        /// </summary>
        /// <value>
        /// The logo class.
        /// </value>
        [JsonProperty("logoClass")]
        [DefaultValue("LogoClass")]
        public string LogoClass { get { return logoClass; } set { logoClass=value;} }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("Text")]
        public string Text { get { return text; } set{text=value;} }

        /// <summary>
        /// Gets or sets the ajax URL.
        /// </summary>
        /// <value>
        /// The ajax URL.
        /// </value>
        [JsonProperty("ajaxUrl")]
        [DefaultValue("AjaxUrl")]
        public string AjaxUrl { get{return ajaxUrl;} set{ajaxUrl=value;} }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonIgnore]
        public MvcTemplate<MobileAccordionBaseItem> Content { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionBaseItem"/> class.
        /// </summary>
        public MobileAccordionBaseItem()
        {
            this.Content = new MvcTemplate<MobileAccordionBaseItem>();
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    public class MobileAccordionBaseItemBuilder : MobileAccordionBaseItem
    {
        #region Field
        /// <summary>
        /// The item
        /// </summary>
        private MobileAccordionBaseItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileAccordionBaseItemBuilder(MobileAccordionBaseItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        /// <summary>
        /// Logoes the class.
        /// </summary>
        /// <param name="logoClass">The logo class.</param>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder LogoClass(string logoClass)
        {
            this.Item.LogoClass = logoClass;
            return this;
        }

        /// <summary>
        /// Ajaxes the URL.
        /// </summary>
        /// <param name="ajaxUrl">The ajax URL.</param>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder AjaxUrl(string ajaxUrl)
        {
            this.Item.AjaxUrl = ajaxUrl;
            return this;
        }

        /// <summary>
        /// Contents the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder Content(Action<MobileAccordionBaseItem> template)
        {
            this.Item.Content.WebFormDataTemplate = template;
            return this;
        }

        /// <summary>
        /// Contents the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder Content(Func<MobileAccordionBaseItem, object> template)
        {
            this.Item.Content.RazorViewTemplate = template;
            return this;
        }
        #endregion
    }

    /// <summary>
    /// Class for AccordionBaseItemAdder
    /// </summary>
    public class MobileAccordionBaseItemAdder
    {
        #region Field
        /// <summary>
        /// The item list
        /// </summary>
        private List<MobileAccordionBaseItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionBaseItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileAccordionBaseItemAdder(List<MobileAccordionBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileAccordionBaseItemBuilder Add()
        {
            MobileAccordionBaseItem newItem = new MobileAccordionBaseItem();
            this.ItemList.Add(newItem);
            return new MobileAccordionBaseItemBuilder(newItem);
        }
    }
}