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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Runtime.Serialization;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;


namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Tile Properties
    /// </summary>
    public class MobileTileProperties : IMobileBase
    {
        #region Fields
        /// <summary>
        /// The theme
        /// </summary>
        private Theme theme = Theme.Auto;
        /// <summary>
        /// The show live tile
        /// </summary>
        private bool showLiveTile;
        /// <summary>
        /// The tile size
        /// </summary>
        private TileSize tileSize = TileSize.Medium;
        /// <summary>
        /// The template identifier
        /// </summary>
        private string templateId;
        /// <summary>
        /// The show tile icon
        /// </summary>
        private bool showTileIcon;
        /// <summary>
        /// The live tile type
        /// </summary>
        private LiveTileType liveTileType = LiveTileType.Flip;
        /// <summary>
        /// The update interval
        /// </summary>
        private int updateInterval = 2000;
        /// <summary>
        /// The show tile badge
        /// </summary>
        private bool showTileBadge;
        /// <summary>
        /// The badge value
        /// </summary>
        private int badgeValue = 1;
        /// <summary>
        /// The maximum badge value
        /// </summary>
        private int maxBadgeValue = 100;
        /// <summary>
        /// The CSS class
        /// </summary>
        private string cssClass;
        /// <summary>
        /// The touch start
        /// </summary>
        private string touchStart;
        /// <summary>
        /// The touch end
        /// </summary>
        private string touchEnd;
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
        public RenderMode RenderMode { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value;} }

        /// <summary>
        /// Gets or sets a value indicating whether [show live tile].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show live tile]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showLiveTile")]
        [DefaultValue(false)]
        public bool ShowLiveTile { get {return showLiveTile;} set{ showLiveTile = value;} }

        /// <summary>
        /// Gets or sets the size of the tile.
        /// </summary>
        /// <value>
        /// The size of the tile.
        /// </value>
        [JsonProperty("tileSize")]
        [DefaultValue(TileSize.Medium)]
        public TileSize TileSize { get{return tileSize;} set{tileSize = value;} }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        [DefaultValue("undefined")]
        public string TemplateId { get{return templateId;} set{templateId = value;} }

        /// <summary>
        /// Gets or sets a value indicating whether [show tile icon].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show tile icon]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showTileIcon")]
        [DefaultValue(false)]
        public bool ShowTileIcon { get{return showTileIcon;} set{showTileIcon= value;} }

        /// <summary>
        /// Gets or sets the type of the live tile.
        /// </summary>
        /// <value>
        /// The type of the live tile.
        /// </value>
        [JsonProperty("liveTileType")]
        [DefaultValue(LiveTileType.Flip)]
        public LiveTileType LiveTileType { get{return liveTileType;} set{liveTileType = value;} }

        /// <summary>
        /// Gets or sets the update interval.
        /// </summary>
        /// <value>
        /// The update interval.
        /// </value>
        [JsonProperty("updateInterval")]
        [DefaultValue(2000)]
        public int UpdateInterval { get{ return updateInterval;} set{ updateInterval = value;} }

        /// <summary>
        /// Gets or sets a value indicating whether [show tile badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show tile badge]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showTileBadge")]
        [DefaultValue(false)]
        public bool ShowTileBadge { get{return showTileBadge;} set{ showTileBadge = value; } }

        /// <summary>
        /// Gets or sets the badge value.
        /// </summary>
        /// <value>
        /// The badge value.
        /// </value>
        [JsonProperty("badgeValue")]
        [DefaultValue(1)]
        public int  BadgeValue { get{return badgeValue ;} set{ badgeValue = value;} }

        /// <summary>
        /// Gets or sets the maximum badge value.
        /// </summary>
        /// <value>
        /// The maximum badge value.
        /// </value>
        [JsonProperty("maxBadgeValue")]
        [DefaultValue(100)]
        public int MaxBadgeValue { get{return maxBadgeValue;} set{maxBadgeValue= value;} }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public string CssClass { get { return cssClass; } set { cssClass = value; } }


        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        [DefaultValue("undefined")]
        public string  TouchStart {get{return touchStart;} set{touchStart = value;} }


        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        [DefaultValue("undefined")] 
        public string TouchEnd { get{ return touchEnd;} set{ touchEnd = value;} }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileTileBaseItem> Items
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTileProperties"/> class.
        /// </summary>
        public MobileTileProperties()
        {
            this.Items = new List<MobileTileBaseItem>();
        }
        #endregion
    }
}
