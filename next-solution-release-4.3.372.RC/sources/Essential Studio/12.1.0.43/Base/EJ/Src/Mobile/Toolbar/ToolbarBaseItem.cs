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
    public class MobileToolbarBaseItem
    {
        #region ToolbarItemFields

        private string iconName = "IconName";
        private string iconUrl = "IconUrl";
        private string text = "Text";

        #endregion

        #region ToolbarItemProperties

        /// <summary>
        /// Gets or sets the name of the icon.
        /// </summary>
        /// <value>
        /// The name of the icon.
        /// </value>
        [JsonProperty("iconname")]
        [DefaultValue("IconName")]
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the icon URL.
        /// </summary>
        /// <value>
        /// The icon URL.
        /// </value>
        [JsonProperty("iconurl")]
        [DefaultValue("IconUrl")]
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("Text")]
        public string Text { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarBaseItem"/> class.
        /// </summary>
        public MobileToolbarBaseItem() { }

        #endregion
    }

}

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileToolbarBaseItemBuilder
    {

        #region Fields
        private MobileToolbarBaseItem ToolbarItem;
        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="toolbarItem">The toolbar item.</param>
        public MobileToolbarBaseItemBuilder(MobileToolbarBaseItem toolbarItem)
        {
            this.ToolbarItem = toolbarItem;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Icons the name.
        /// </summary>
        /// <param name="iconName">Name of the icon.</param>
        /// <returns></returns>
        public MobileToolbarBaseItemBuilder IconName(string iconName)
        {
            this.ToolbarItem.IconName = iconName;
            return this;
        }

        /// <summary>
        /// Icons the URL.
        /// </summary>
        /// <param name="iconUrl">The icon URL.</param>
        /// <returns></returns>
        public MobileToolbarBaseItemBuilder IconUrl(string iconUrl)
        {
            this.ToolbarItem.IconUrl = iconUrl;
            return this;
        }

        /// <summary>
        /// Texts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileToolbarBaseItemBuilder Text(string text)
        {
            this.ToolbarItem.Text = text;
            return this;
        }

        #endregion
    }

    public class MobileToolbarBaseItemAdder
    {
        #region Fields

        private List<MobileToolbarBaseItem> ToolbarItemList;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarBaseItemAdder"/> class.
        /// </summary>
        /// <param name="toolbarItemList">The toolbar item list.</param>
        public MobileToolbarBaseItemAdder(List<MobileToolbarBaseItem> toolbarItemList)
        {
            this.ToolbarItemList = toolbarItemList;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileToolbarBaseItemBuilder Add()
        {
            MobileToolbarBaseItem newitem = new MobileToolbarBaseItem();
            this.ToolbarItemList.Add(newitem);
            return new MobileToolbarBaseItemBuilder(newitem);
        }

        #endregion
    }

}
