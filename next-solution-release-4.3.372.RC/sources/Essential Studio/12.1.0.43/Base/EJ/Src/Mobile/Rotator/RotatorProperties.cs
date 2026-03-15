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
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileRotatorProperties : RotatorPropertiesBase,IMobileBase
    {
        #region Fields
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;       
        private bool pager = true;
        private bool header = false;
        private bool dataBinding = false;
        private string targetHeight="auto";
        private string targetWidth="auto";
        private string headerTitle="Title";
        private int currentIndex=0;
        private object dataSource = new object();
        private MobPager pagerPosition = MobPager.Bottom;
        #endregion

        #region Properties

        public MobileRotatorProperties()
        {
            this.Windows = new RotatorRenderModeProperties();
        }
        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [showPager].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showPager]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showPager")]
        [DefaultValue(true)]
        public bool Pager { get { return pager; } set { pager = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [header].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [header]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("header")]
        [DefaultValue(false)]
        public bool Header { get { return header; } set { header = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [dataBinding].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dataBinding]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("dataBinding")]
        [DefaultValue(false)]
        public bool DataBinding { get { return dataBinding; } set { dataBinding = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [targetHeight].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [targetHeight]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("targetHeight")]
        [DefaultValue("auto")]
        public string TargetHeight { get { return targetHeight; } set { targetHeight = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [targetWidth].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [targetWidth]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("targetWidth")]
        [DefaultValue("auto")]
        public string TargetWidth { get { return targetWidth; } set { targetWidth = value; } }

        /// <summary>
        /// Gets or sets the headerTitle value.
        /// </summary>
        /// <value>
        /// The headerTitle value.
        /// </value>
        [JsonProperty("headerTitle")]
        [DefaultValue("Title")]
        public string HeaderTitle { get { return headerTitle; } set { headerTitle = value; } }

        /// <summary>
        /// Gets or sets the currentIndex value.
        /// </summary>
        /// <value>
        /// The currentIndex value.
        /// </value>
        [JsonProperty("currentIndex")]
        [DefaultValue(0)]
        public int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }

        /// <summary>
        /// Gets or sets the targetId.
        /// </summary>
        /// <value>
        /// The targetId value.
        /// </value>
        [JsonProperty("targetId")]
        public string TargetId { get; set; }        

        /// <summary>
        /// Gets or sets the pagerPosition.
        /// </summary>
        /// <value>
        /// The dataSource value.
        /// </value>
        [JsonProperty("pagerPosition")]
        public MobPager PagerPosition { get { return pagerPosition; } set { pagerPosition = value; } }
        
        /// <summary>
        /// Gets or sets the swipeLeft.
        /// </summary>
        /// <value>
        /// The swipeLeft.
        /// </value>
        [JsonProperty("swipeLeft")]
        public string SwipeLeft { get; set; }

        /// <summary>
        /// Gets or sets the swipeRight.
        /// </summary>
        /// <value>
        /// The swipeRight.
        /// </value>
        [JsonProperty("swipeRight")]
        public string SwipeRight { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public RotatorRenderModeProperties Windows { get; set; }


        #endregion Properties
    }
    
    public enum MobPager
    {
        Top,
        Bottom,
    }
}
