#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Tile Control
    /// </summary>
    public class MobileTile : Control
    {
        #region Fields
        /// <summary>
        /// The plugin string
        /// </summary>
        private string pluginString = "data-ej-";


        /// <summary>
        /// Gets or sets the mobile tile model.
        /// </summary>
        /// <value>
        /// The mobile tile model.
        /// </value>
        public MobileTileProperties MobileTileModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        public override string TagName
        {
            get
            {
                return "div";
            }
        }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        public override string PluginName
        {
            get { return "ejmTile"; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected override object Model
        {
            get { return this.MobileTileModel; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTile"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="propModel">The property model.</param>
        public MobileTile(String id, MobileTileProperties propModel)
        {
            this.ID = id;
            this.MobileTileModel = propModel;
        }
        #endregion

        #region Render
        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes("id", ID);
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            foreach (MobileTileBaseItem item in this.MobileTileModel.Items)
            {
                HtmlTag liTag = new HtmlTag("div").Attributes("data-ej-layout", "tile");
                liTag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString));
                container.Add(liTag);
            }
            return new HtmlString(String.Format(container.ToString()));
        }

        /// <summary>
        /// Creates the un obtrusive container.
        /// </summary>
        /// <param name="controlId">The control identifier.</param>
        /// <returns></returns>
        public override HtmlString CreateUnObtrusiveContainer(string controlId)
        {
            HtmlTag container = new HtmlTag(TagName);
            container.Attributes("id", ID);
            container.Attributes("data-role", PluginName.ToLower());
            container.Attributes(CreateUnObtrusiveDataDictionary(this.Model, controlId, pluginString));
            foreach (MobileTileBaseItem item in this.MobileTileModel.Items)
            {
                HtmlTag liTag = new HtmlTag("div").Attributes("data-ej-layout", "tile");
                liTag.Attributes(CreateUnObtrusiveDataDictionary(item, this.ID, pluginString));
                container.Add(liTag);
            }
            return new HtmlString(container.ToString());
        }
        #endregion
    }
    /// <summary>
    /// Enum for Live Tile Type
    /// </summary>
    [DataContract]
    public enum LiveTileType
    {
        [EnumMember(Value = "flip")]
        Flip,
        [EnumMember(Value = "slide")]
        Slide,
        [EnumMember(Value = "carousel")]
        Carousel,
    }

    /// <summary>
    /// Enum for TileSize
    /// </summary>
    [DataContract]
    public enum TileSize
    {
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "small")]
        Small,
        [EnumMember(Value = "large")]
        Large,
        [EnumMember(Value = "wide")]
        Wide,
    }

    /// <summary>
    /// Enum for TileType
    /// </summary>
    [DataContract]
    public enum TileType
    {
        [EnumMember(Value = "main")]
        Main,
        [EnumMember(Value = "live")]
        Live,
    }

    /// <summary>
    /// Enum for TileTextAlign
    /// </summary>
    [DataContract]
    public enum TileTextAlign
    {
        [EnumMember (Value = "normal")]
        Normal,
        [EnumMember (Value = "left")]
        Left,
        [EnumMember (Value = "center")]
        Center,
        [EnumMember (Value = "right")]
        Right,
    }

    /// <summary>
    /// Enum for TileImagePosition
    /// </summary>
    [DataContract]
    public enum TileImagePosition
    {
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "top")]
        Top,
        [EnumMember(Value = "bottom")]
        Bottom,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "topleft")]
        TopLeft,
        [EnumMember(Value = "topright")]
        TopRight,
        [EnumMember(Value = "bottomright")]
        BottomRight,
        [EnumMember(Value = "bottomleft")]
        BottomLeft,
        [EnumMember(Value = "fill")]
        Fill,
    }
}
