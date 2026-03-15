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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Tile Properties Builder
    /// </summary>
    public class MobileTilePropertiesBuilder
    {
        #region Fields
        /// <summary>
        /// The m tile
        /// </summary>
        private MobileTile mTile;

        /// <summary>
        /// Gets or sets the items collection.
        /// </summary>
        /// <value>
        /// The items collection.
        /// </value>
        internal List<MobileTileBaseItem> ItemsCollection { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTilePropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTile">The m tile.</param>
        public MobileTilePropertiesBuilder(MobileTile mTile)
        {
            this.mTile = new MobileTile(mTile.ID, mTile.MobileTileModel);

        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder RenderMode(RenderMode renderMode)
        {
            mTile.MobileTileModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder Theme(Theme theme)
        {
            mTile.MobileTileModel.Theme = theme;
            return this;
        }

        /// <summary>
        /// Shows the live tile.
        /// </summary>
        /// <param name="showLiveTile">if set to <c>true</c> [show live tile].</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder ShowLiveTile(bool showLiveTile)
        {
            mTile.MobileTileModel.ShowLiveTile = showLiveTile;
            return this;
        }

        /// <summary>
        /// Tiles the size.
        /// </summary>
        /// <param name="tileSize">Size of the tile.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder TileSize(TileSize tileSize) 
        {
            mTile.MobileTileModel.TileSize = tileSize;
            return this;
        }

        /// <summary>
        /// Templates the identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder TemplateId(string templateId)
        {
            mTile.MobileTileModel.TemplateId = templateId;
            return this;
        }

        /// <summary>
        /// Shows the tile icon.
        /// </summary>
        /// <param name="showTileIcon">if set to <c>true</c> [show tile icon].</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder ShowTileIcon(bool showTileIcon)
        {
            mTile.MobileTileModel.ShowTileIcon = showTileIcon;
            return this;
        }

        /// <summary>
        /// Lives the type of the tile.
        /// </summary>
        /// <param name="liveTileType">Type of the live tile.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder LiveTileType(LiveTileType liveTileType)
        {
            mTile.MobileTileModel.LiveTileType = liveTileType;
            return this;
        }

        /// <summary>
        /// Updates the interval.
        /// </summary>
        /// <param name="updateinterval">The updateinterval.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder UpdateInterval(int updateinterval)
        {
            mTile.MobileTileModel.UpdateInterval = updateinterval;
            return this;
        }

        /// <summary>
        /// Shows the tile badge.
        /// </summary>
        /// <param name="showTileBadge">if set to <c>true</c> [show tile badge].</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder ShowTileBadge(bool showTileBadge)
        {
            mTile.MobileTileModel.ShowTileBadge = showTileBadge;
            return this;
        }

        /// <summary>
        /// Badges the value.
        /// </summary>
        /// <param name="badgeValue">The badge value.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder BadgeValue(int badgeValue)
        {
            mTile.MobileTileModel.BadgeValue = badgeValue;
            return this;
        }

        /// <summary>
        /// Maximums the badge value.
        /// </summary>
        /// <param name="maxBadgeValue">The maximum badge value.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder MaxBadgeValue(int maxBadgeValue)
        {
            mTile.MobileTileModel.MaxBadgeValue = maxBadgeValue;
            return this;
        }

        /// <summary>
        /// CSSs the class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder CssClass(string cssClass)
        {
            mTile.MobileTileModel.CssClass = cssClass;
            return this;
        }

        /// <summary>
        /// Itemses the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder Items(Action<MobileTileBaseItemAdder> item)
        {
            this.ItemsCollection = new List<MobileTileBaseItem>();
            MobileTileBaseItemAdder mTileAdder = new MobileTileBaseItemAdder(mTile.MobileTileModel.Items);
            item.Invoke(mTileAdder);
            return this;
        }

        /// <summary>
        /// Clients the side events.
        /// </summary>
        /// <param name="clientSideEvents">The client side events.</param>
        /// <returns></returns>
        public MobileTilePropertiesBuilder ClientSideEvents(Action<MobileTileClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new MobileTileClientSideEventsBuilder(this.mTile.MobileTileModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        #endregion

        #region Render
        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString Render()
        {
            return new HtmlString(mTile.Render().ToString());
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return Render().ToString();
        }
        #endregion
    }
}
