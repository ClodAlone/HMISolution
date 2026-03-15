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
using System.Drawing;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization
{
    public class LegendBuilder
    {
      
         private Legend m_Legend = null;
         public LegendBuilder(Legend options)
        {
            this.m_Legend = options;
        }

        public LegendBuilder Visible(bool visible)
        {
            this.m_Legend.Visible = visible;
            return this;
        }
        public LegendBuilder Border(Action<ChartBorderBuilder> font)
        {
            var obj = new ChartBorder();
            this.m_Legend.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public LegendBuilder Fill(string fill)
        {
            this.m_Legend.Fill = fill;
            return this;
        }
        public LegendBuilder Opacity(double Opacity)
        {
            this.m_Legend.Opacity = Opacity;
            return this;
        }
        public LegendBuilder RowCount(int rowCount)
        {
            this.m_Legend.RowCount = rowCount;
            return this;
        }
        public LegendBuilder ColumnCount(int columnCount)
        {
            this.m_Legend.ColumnCount = columnCount;
            return this;
        }
        public LegendBuilder ItemPadding(int itemPadding)
        {
            this.m_Legend.ItemPadding = itemPadding;
            return this;
        }
        
        public LegendBuilder Shape(ChartShape shape)
        {
            this.m_Legend.Shape = shape;
            return this;
        }
        public LegendBuilder Alignment(TextAlignment alignment)
        {
            this.m_Legend.Alignment = alignment;
            return this;
        }
        public LegendBuilder Position(LegendPosition position)
        {
            this.m_Legend.Position = position;
            return this;
        }
        public LegendBuilder Location(Action<LocationBuilder> location)
        {
            var obj = new Location();
            this.m_Legend.Location = obj;
            var builder = new LocationBuilder(obj);
            if (location != null)
                location.Invoke(builder);
            return this;
        }
        public LegendBuilder ItemSize(Action<ItemSizeBuilder> itemSize)
        {
            var obj = new ItemSize();
            this.m_Legend.ItemSize = obj;
            var builder = new ItemSizeBuilder(obj);
            if (itemSize != null)
                itemSize.Invoke(builder);
            return this;
        }
        
        public LegendBuilder Font(Action<ChartFontBuilder> font)
        {

            var obj = new ChartFont();
            this.m_Legend.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
  
        }

    }

    public class ItemSizeBuilder
    {
        private ItemSize itemsize = new ItemSize();
        public ItemSizeBuilder(ItemSize itemsize)
        {
            this.itemsize = itemsize;
        }
        public ItemSizeBuilder Height(int Height)
        {
            this.itemsize.Height = Height;
            return this;
        }
        public ItemSizeBuilder Width(int Width)
        {
            this.itemsize.Width = Width;
            return this;
        }
        public ItemSizeBuilder Border(Action<ChartBorderBuilder> border)
        {
            var obj = new ChartBorder();
            this.itemsize.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (border != null)
                border.Invoke(builder);
            return this;
        }
        
    }

    public class LocationBuilder
    {
        private Location location = new Location();
        public LocationBuilder(Location location)
        {
            this.location = location;
        }
        public LocationBuilder X(int X)
        {
            this.location.X = X;
            return this;
        }
        public LocationBuilder Y(int Y)
        {
            this.location.Y = Y;
            return this;
        }

    }

    public class TextOptionBuilder
    {

        private TextOption textoption = new TextOption();
        public TextOptionBuilder(TextOption textoption)
        {
            this.textoption = textoption;
        }
        public TextOptionBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.textoption.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        //public TextOptionBuilder Font(object Font)
        //{
        //    this.textoption.Font = Font;
        //    return this;
        //}
    }

   
}
