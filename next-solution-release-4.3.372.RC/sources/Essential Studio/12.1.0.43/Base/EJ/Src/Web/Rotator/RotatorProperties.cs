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

using System.Collections.ObjectModel;

namespace Syncfusion.JavaScript.Models
{
    public class RotatorProperties
    {
        #region Fields
        //Boolean Values
        private bool enabled = true;
        private bool autoPlay = false;
        private bool enablePlaybtn = false;
        private bool slideButton = true;
        private bool allowResize =false;
        private bool thumbItem = false;
        private bool pager = true;
        private bool caption = false;
        private bool allowKeyboardNavigation = true;
        private bool rtl=true;


        //int values
        private int speed = 600;

        //string values
        private String query = null;
        private String itemDisplay = "1";
        private String itemMove = "1";
        private String startIndex="0";
        private String slideWidth = "";
        private String slideHeight = "";
        private String frameSpace = "";
        private String thumbSource=null;
        private String animation = "slide";
        private String cssClass = "";

        //Enumeration values
        private Orientation orientation = Orientation.Horizontal;
        private PagerPosition pagerPosition = PagerPosition.Outside;
     
        //Objects
        private Object dataSource = new object();
        private Object fields = new RotatorFields();

        //Events
        private String create = null;
        private String change = null;
        private String start = null;
        private String stop=null;
        private String destroy = null;
        private String thumbClick=null;
        private String pagerClick = null;


        private Rotator rotator=new Rotator();
        #endregion
        public RotatorProperties(){
            this.Items = new List<RotatorBaseItem>();
        }


#region Properties
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("autoPlay")]
        [DefaultValue(false)]
        public bool AutoPlay
        {
            get { return this.autoPlay; }
            set { this.autoPlay = value; }
        }
        [JsonProperty("enablePlaybtn")]
        [DefaultValue(false)]
        public bool EnablePlaybtn
        {
            get { return this.enablePlaybtn; }
            set { this.enablePlaybtn = value; }
        }
        [JsonProperty("slideButton")]
        [DefaultValue(true)]
        public bool SlideButton
        {
            get { return this.slideButton; }
            set { this.slideButton = value; }
        }
        [JsonProperty("allowResize")]
        [DefaultValue(false)]
        public bool AllowResize
        {
            get { return this.allowResize; }
            set { this.allowResize = value; }
        }
        [JsonProperty("thumbItem")]
        [DefaultValue(false)]
        public bool ThumbItem
        {
            get { return this.thumbItem; }
            set { this.thumbItem = value; }
        }
        [JsonProperty("pager")]
        [DefaultValue(true)]
        public bool Pager
        {
            get { return this.pager; }
            set { this.pager = value; }
        }
        [JsonProperty("caption")]
        [DefaultValue(false)]
        public bool Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }
        [JsonProperty("allowKeyboardNavigation")]
        [DefaultValue(true)]
        public bool AllowKeyboardNavigation
        {
            get { return this.allowKeyboardNavigation; }
            set { this.allowKeyboardNavigation = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(true)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl= value; }
        }
         //object values
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public Object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }
        
        [JsonProperty("fields")]
        public Object RotatorFields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }
        //String
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }
        [JsonProperty("itemDisplay")]
        [DefaultValue("1")]
        public String ItemDisplay
        {
            get { return this.itemDisplay; }
            set { this.itemDisplay = value; }
        }
        [JsonProperty("itemMove")]
        [DefaultValue("1")]
        public String ItemMove
        {
            get { return this.itemMove; }
            set { this.itemMove = value; }
        }
        [JsonProperty("startIndex")]
        [DefaultValue("0")]
        public String StartIndex
        {
            get { return this.startIndex; }
            set { this.startIndex = value; }
        }
        [JsonProperty("slideWidth")]
        [DefaultValue("")]
        public String SlideWidth
        {
            get { return this.slideWidth; }
            set { this.slideWidth = value; }
        }
        [JsonProperty("slideHeight")]
        [DefaultValue("")]
        public String SlideHeight
        {
            get { return this.slideHeight; }
            set { this.slideHeight = value; }
        }
        [JsonProperty("frameSpace")]
        [DefaultValue("")]
        public String FrameSpace
        {
            get { return this.frameSpace; }
            set { this.frameSpace = value; }
        }
        [JsonProperty("thumbSource")]
        [DefaultValue(null)]
        public String ThumbSource
        {
            get { return this.thumbSource; }
            set { this.thumbSource = value; }
        }
        [JsonProperty("animation")]
        [DefaultValue("slide")]
        public String Animation
        {
            get { return this.animation; }
            set { this.animation = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        //Enum values
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }
        [JsonProperty("pagerPosition")]
        [DefaultValue(PagerPosition.Outside)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PagerPosition PagerPosition
        {
            get { return this.pagerPosition; }
            set { this.pagerPosition = value; }
        }
       //int values
        [JsonProperty("speed")]
        [DefaultValue(600)]
        public int Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }
       //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
         [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
         [JsonProperty("start")]
        [DefaultValue(null)]
        public String Start
        {
            get { return this.start; }
            set { this.start = value; }
        }
         [JsonProperty("stop")]
        [DefaultValue(null)]
        public String Stop
        {
            get { return this.stop; }
            set { this.stop = value; }
        }
         [JsonProperty("thumbClick")]
         [DefaultValue(null)]
         public String ThumbClick
         {
             get { return this.thumbClick; }
             set { this.thumbClick = value; }
         }
         [JsonProperty("pagerClick")]
         [DefaultValue(null)]
         public String PagerClick
         {
             get { return this.pagerClick; }
             set { this.pagerClick = value; }
         }
         [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
         #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeRotatorFields()
        {
            if (Utils.PropertyCompare(RotatorFields, new RotatorFields()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeDataSource()
        {
            if (typeof(DataSource).IsAssignableFrom(this.DataSource.GetType()))
            {
                if (Utils.PropertyCompare(DataSource, new DataSource()))
                    return true;
                else
                    return false;
            }
            else if (this.DataSource is IEnumerable)
            {
                ICollection data = DataSource as ICollection;
                if (data.Count != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        #endregion
        [JsonIgnore]
        public List<RotatorBaseItem> Items { get; set; }
    }
}
