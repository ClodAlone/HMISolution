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
    /// Class for Tile Base Item
    /// </summary>
    public class MobileTileBaseItem
    {
        #region Fields

        private string tileImageurl = "TileImageurl";
        private bool showTileText = false;
        private string imageClass = "ImageClass";
        private string templateId = "TemplateId";
        private string tileText = "TileText";
        private TileType tileType = TileType.Main;
        private TileTextAlign textAlign = TileTextAlign.Center;
        private TileImagePosition tileImagePosition = TileImagePosition.Center;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tile imageurl.
        /// </summary>
        /// <value>
        /// The tile imageurl.
        /// </value>
       
        [JsonProperty("tileImageurl")]
        [DefaultValue("TileImageurl")]
        public string TileImageurl { get { return tileImageurl; } set { tileImageurl = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show tile text].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show tile text]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showTileText")]
        [DefaultValue(false)]
        public bool ShowTileText { get { return showTileText; } set { showTileText = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
       
        [JsonProperty("imageClass")]
        [DefaultValue("ImageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        [DefaultValue("TemplateId")]
        public string TemplateId { get { return templateId; } set { templateId = value; } }

        /// <summary>
        /// Gets or sets the tile text.
        /// </summary>
        /// <value>
        /// The tile text.
        /// </value>
        [JsonProperty("tileText")]
        [DefaultValue("TileText")]
        public string TileText { get { return tileText; } set { tileText = value; } }

        /// <summary>
        /// Gets or sets the type of the tile.
        /// </summary>
        /// <value>
        /// The type of the tile.
        /// </value>
        [JsonProperty("tileType")]
        [DefaultValue(TileType.Main)]
        public TileType TileType { get { return tileType; } set { tileType = value; } }

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        /// <value>
        /// The text align.
        /// </value>
        [JsonProperty("textAlign")]
        [DefaultValue(TileTextAlign.Center)]
        public TileTextAlign TextAlign { get { return textAlign; } set { textAlign = value; } }

        /// <summary>
        /// Gets or sets the tile image position.
        /// </summary>
        /// <value>
        /// The tile image position.
        /// </value>
        [JsonProperty("tileImagePosition")]
        [DefaultValue(TileImagePosition.Center)]
        public TileImagePosition TileImagePosition { get { return tileImagePosition; } set { tileImagePosition = value; } }
        #endregion

        #region Constructor
        #endregion
    }
}
namespace Syncfusion.JavaScript
{
    /// <summary>
    /// Class for Tile Base Item Builder
    /// </summary>
    public class MobileTileBaseItemBuilder
    {
        #region Field
        /// <summary>
        /// The item
        /// </summary>
        private MobileTileBaseItem Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTileBaseItemBuilder"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public MobileTileBaseItemBuilder(MobileTileBaseItem item)
        {
            this.Item = item;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Tiles the imageurl.
        /// </summary>
        /// <param name="tileImageurl">The tile imageurl.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TileImageurl(string tileImageurl)
        {
            this.Item.TileImageurl = tileImageurl;
            return this;
        }

        /// <summary>
        /// Shows the tile text.
        /// </summary>
        /// <param name="showTileText">if set to <c>true</c> [show tile text].</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder ShowTileText(bool showTileText)
        {
            this.Item.ShowTileText = showTileText;
            return this;
        }

        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder ImageClass(string imageClass)
        {
            this.Item.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TemplateId(string templateId)
        {
            this.Item.TemplateId = templateId; 
            return this;
        }

        /// <summary>
        /// Tiles the text.
        /// </summary>
        /// <param name="tileText">The tile text.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TileText(string tileText)
        {
            this.Item.TileText = tileText;
            return this;
        }

        /// <summary>
        /// Tiles the type.
        /// </summary>
        /// <param name="tileType">Type of the tile.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TileType(TileType tileType)
        {
            this.Item.TileType = tileType;
            return this;
        }

        /// <summary>
        /// Texts the align.
        /// </summary>
        /// <param name="textAlign">The text align.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TextAlign(TileTextAlign textAlign)
        {
            this.Item.TextAlign = textAlign;
            return this;
        }

        /// <summary>
        /// Tiles the image position.
        /// </summary>
        /// <param name="tileImagePosition">The tile image position.</param>
        /// <returns></returns>
        public MobileTileBaseItemBuilder TileImagePosition(TileImagePosition tileImagePosition)
        {
            this.Item.TileImagePosition = tileImagePosition;
            return this;
        }
        #endregion
    }

    public class MobileTileBaseItemAdder
    {
        #region Field
        /// <summary>
        /// The item list
        /// </summary>
        private List<MobileTileBaseItem> ItemList;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTileBaseItemAdder"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MobileTileBaseItemAdder(List<MobileTileBaseItem> itemList)
        {
            this.ItemList = itemList;
        }
        #endregion

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public MobileTileBaseItemBuilder Add()
        {
            MobileTileBaseItem newItem = new MobileTileBaseItem();
            this.ItemList.Add(newItem);
            return new MobileTileBaseItemBuilder(newItem);
        }
    }
}