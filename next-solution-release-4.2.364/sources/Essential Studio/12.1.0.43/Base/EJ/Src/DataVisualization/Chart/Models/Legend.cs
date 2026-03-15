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
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.Shared;
using System.Drawing;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class Legend
    {
        #region Fields

        //Boolean Values
        private bool m_Visible = true;
       private object m_border = null;
        //Integer Values
        private int m_RowCount = 0;
        private int m_ColumnCount = 0;
        private int m_ItemPadding = 10;
         
        // enum
        private ChartShape m_Shape = ChartShape.None;
        private LegendPosition m_Position = LegendPosition.Bottom;
        private TextAlignment m_Alignment = TextAlignment.Center;
        // Object Values
        private object location = null;
        private object itemSize = null;
        private object textOption = null;
        private double m_opacity = -1;
        private string m_fill = null;

        #endregion

        #region Properties

        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return m_Visible; }
            set { this.m_Visible = value; }
        }
        [JsonProperty("fill")]
        [DefaultValue(null)]
        public string Fill
        {
            get
            {
                return this.m_fill;
            }
            set
            {
                this.m_fill = value;
            }
        }
        [JsonProperty("opacity")]
        [DefaultValue(-1)]
        public double Opacity
        {
            get
            {
                return this.m_opacity;
            }
            set
            {
                this.m_opacity = value;
            }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        //Integer Values
        [JsonProperty("rowCount")]
        [DefaultValue(0)]
        public int RowCount
        {
            get { return this.m_RowCount; }
            set { this.m_RowCount = value; }
        }

        [JsonProperty("columnCount")]
        [DefaultValue(0)]
        public int ColumnCount
        {
            get { return this.m_ColumnCount; }
            set { this.m_ColumnCount = value; }
        }

        [JsonProperty("itemPadding")]
        [DefaultValue(10)]
        public int ItemPadding
        {
            get { return this.m_ItemPadding; }
            set { this.m_ItemPadding = value; }
        }

       
       // Enum Values

        [JsonProperty("shape")]
        [DefaultValue(ChartShape.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartShape Shape
        {
            get { return this.m_Shape; }
            set { this.m_Shape = value; }
        }

        [JsonProperty("alignment")]
        [DefaultValue(TextAlignment.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextAlignment Alignment
        {
            get { return this.m_Alignment; }
            set { this.m_Alignment = value; }
        }

        [JsonProperty("position")]
        [DefaultValue(LegendPosition.Bottom)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LegendPosition Position
        {
            get { return this.m_Position; }
            set { this.m_Position = value; }
        }

        [JsonProperty("location")]
        public object Location
        {
            get { return this.location; }
            set { this.location = value; }
        }

        [JsonProperty("itemSize")]
        public object ItemSize
        {
            get { return this.itemSize; }
            set { this.itemSize = value; }
        }

        

        [JsonProperty("font")]
        public object Font
        {
            get { return this.textOption; }
            set { this.textOption = value; }
        }
       
        #endregion

        #region ShouldSerialize Methods

        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(Location, new Location()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeItemSize()
        {
            if (Utils.PropertyCompare(ItemSize, new ItemSize()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new ChartBorder()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new Font()))
                return true;
            else
                return false;
        }

        #endregion
    }

   public class Location{
       
        #region Fields

        private int m_x = 0;
        private int m_y = 0;

        #endregion

        #region Properties

        [JsonProperty("X")]
        [DefaultValue(0)]
        public int X
        {
            get { return this.m_x;}
            set { this.m_x = value;}
        }
        [JsonProperty("Y")]
        [DefaultValue(0)]
        public int Y
        {
            get {return this.m_y;}
            set { this.m_y = value;}
        }
       #endregion
    }

   public class ItemSize
    {

        #region Fields

        private int m_Height = 10;
        private int m_Width = 10;
        private object m_border = null;

        #endregion

        #region Properties

        [JsonProperty("height")]
        [DefaultValue(10)]
        public int Height
        {
            get { return this.m_Height; }
            set { this.m_Height = value; }
        }

        [JsonProperty("width")]
        [DefaultValue(10)]
        public int Width
        {
            get { return this.m_Width; }
            set { this.m_Width = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new ChartBorder()))
                return true;
            else
                return false;
        }



        #endregion
    }
       
   public class TextOption
    {
        #region Fields

        private object m_Font =null;
        
        #endregion

        [JsonProperty("font")]
        public object Font
        {
            get { return this.m_Font; }
            set { this.m_Font = value; }
        }

        public bool ShouldSerializeStyle()
        {
            if (Utils.PropertyCompare(Font, new ChartFont()))
                return true;
            else
                return false;
        }

    }

   
}
