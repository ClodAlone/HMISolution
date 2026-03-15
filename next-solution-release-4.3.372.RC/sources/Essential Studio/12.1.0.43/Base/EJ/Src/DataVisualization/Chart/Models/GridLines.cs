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
using Syncfusion.JavaScript.Shared.Serializer;
using System.Drawing;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class MajorGridLines
   {
       #region Field

       private double m_width=-1;
       private bool  m_visible=true;
       private string m_dasharray=null;
       private string  m_color=null;
       private double m_opacity=-1;
       private int m_offset =0;
       #endregion

       #region Property
       /// <summary>
       /// gets or sets Color
       /// </summary>
       [JsonProperty("color")]
       [DefaultValue(null)]
       public string Color
       {
           get
           {
               return this.m_color;
           }
           set
           {
               this.m_color = value;
           }
       }
       /// <summary>
       /// gets or sets offset for axisLine
       /// </summary>
       [JsonProperty("offset")]
       [DefaultValue(0)]
       public int Offset
       {
           get
           {
               return this.m_offset;
           }
           set
           {
               this.m_offset = value;
           }
       }
       /// <summary>
       /// gets or sets DashArray
       /// </summary>
        [JsonProperty("dashArray")]
       [DefaultValue(null)]
       public string DashArray
       {
           get
           {
               return this.m_dasharray;
           }
           set
           {
               this.m_dasharray = value;
           }
       }
       /// <summary>
       /// gets or sets Width
       /// </summary>
       [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
       {
           get
           {
               return this.m_width;
           }
           set
           {
               this.m_width = value;
           }
       }
       /// <summary>
       /// gets or sets Visible
       /// </summary>
       
       [JsonProperty("visible")]
       [DefaultValue(true)]
       public bool Visible
       {
           get
           {
               return this.m_visible;
           }
           set
           {
               this.m_visible = value;
           }
       }
       /// <summary>
       /// gets or sets Width
       /// </summary>
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

       #endregion

   }
   public class MinorGridLines
   {
       #region Field

       private double m_width = -1;
       private bool m_visible = false;
       private string m_dasharray = null;
       private string m_color = null;
       private double m_opacity = -1;
       #endregion

       #region Property
       /// <summary>
       /// gets or sets Color
       /// </summary>
       [JsonProperty("color")]
       [DefaultValue(null)]
       public string Color
       {
           get
           {
               return this.m_color;
           }
           set
           {
               this.m_color = value;
           }
       }
       /// <summary>
       /// gets or sets DashArray
       /// </summary>
       [JsonProperty("dashArray")]
       [DefaultValue(null)]
       public string DashArray
       {
           get
           {
               return this.m_dasharray;
           }
           set
           {
               this.m_dasharray = value;
           }
       }
       /// <summary>
       /// gets or sets Width
       /// </summary>
       [JsonProperty("width")]
       [DefaultValue(-1)]
       public double Width
       {
           get
           {
               return this.m_width;
           }
           set
           {
               this.m_width = value;
           }
       }
       /// <summary>
       /// gets or sets Visible
       /// </summary>

       [JsonProperty("visible")]
       [DefaultValue(false)]
       public bool Visible
       {
           get
           {
               return this.m_visible;
           }
           set
           {
               this.m_visible = value;
           }
       }
       /// <summary>
       /// gets or sets Width
       /// </summary>
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

       #endregion

   }

    public class MajorTicks
    {
        #region Field
        private double m_width=-1;
        private int m_size=-1;
        private bool m_visible=true;
        private string m_color=null;
        #endregion

        #region Properties
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.m_visible;}
            set { this.m_visible = value;}
        }
        /// <summary>
        /// gets or sets Width
        /// </summary>
        [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
        {
            get
            {
                return this.m_width;
            }
            set
            {
                this.m_width = value;
            }
        }
        /// <summary>
        /// gets or sets Size
        /// </summary>
        [JsonProperty("size")]
        [DefaultValue(-1)]
        public int Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// gets or sets Color
        /// </summary>
        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get
            {
                return this.m_color;
            }
            set
            {
                this.m_color = value;
            }
        }
        #endregion
    }
    public class MinorTicks
    {
        #region Field
        private double m_width = -1;
        private int m_size = -1;
        private bool m_visible = false;
        private string m_color = null;
        #endregion

        #region Properties
        [JsonProperty("visible")]
        [DefaultValue(false)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }
        /// <summary>
        /// gets or sets Width
        /// </summary>
        [JsonProperty("width")]
        [DefaultValue(-1)]
        public double Width
        {
            get
            {
                return this.m_width;
            }
            set
            {
                this.m_width = value;
            }
        }
        /// <summary>
        /// gets or sets Size
        /// </summary>
        [JsonProperty("size")]
        [DefaultValue(-1)]
        public int Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// gets or sets Color
        /// </summary>
        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get
            {
                return this.m_color;
            }
            set
            {
                this.m_color = value;
            }
        }
        #endregion
    }
   
}
