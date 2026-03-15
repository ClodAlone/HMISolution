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
    public class MobileGroupButtonBaseItem
    {
        #region Fields

        private string imageurl = "Imageurl";
        private string imageclass = "Imageclass";
        private string text = "Text";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("imageurl")]
        [DefaultValue("Imageurl")]
        public string Imageurl { get { return imageurl; } set { imageurl = value; } }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("imageclass")]
        [DefaultValue("Imageclass")]
        public string Imageclass { get { return imageclass; } set { imageclass = value; } }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("text")]
        [DefaultValue("Text")]
        public string Text { get { return text; } set { text = value; } }
        
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MTabBaseItem"/> class.
        /// </summary>
        public MobileGroupButtonBaseItem()
        {
        }
        #endregion

    }
}
namespace Syncfusion.JavaScript.Mobile
{
    public class MobileGroupButtonBaseItemBuilder
    {
        #region Fields
        private MobileGroupButtonBaseItem Item;
        #endregion

        #region Constructor
       
        /// <summary>
        /// Initializes a new instance of the <see cref="MTabBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileGroupButtonBaseItemBuilder(MobileGroupButtonBaseItem item)
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
        public MobileGroupButtonBaseItemBuilder ImageClass(string id)
        {
            this.Item.Imageclass = id;
            return this;
        }

        /// <summary>
        /// Identifiers the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileGroupButtonBaseItemBuilder Imageurl(string url)
        {
            this.Item.Imageurl = url;
            return this;
        }

        /// <summary>
        /// Identifiers the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public MobileGroupButtonBaseItemBuilder Text(string text)
        {
            this.Item.Text = text;
            return this;
        }

        #endregion

    }

    public class MobileGroupButtonBaseItemAdder
    {
        #region Fields
        private List<MobileGroupButtonBaseItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButtonBaseItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileGroupButtonBaseItemAdder(List<MobileGroupButtonBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileGroupButtonBaseItemBuilder Add()
        {
            MobileGroupButtonBaseItem newGroupButton = new MobileGroupButtonBaseItem();
            this.ItemList.Add(newGroupButton);
            return new MobileGroupButtonBaseItemBuilder(newGroupButton);
        }

        #endregion
    }
}