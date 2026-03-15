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

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// SplitPane Properties
    /// </summary>
    public class MobileSplitPaneProperties : IMobileBase
    {
        #region Fields
        private string leftHeaderTitle = "Title";
        private string rightHeaderTitle = "Title";
        private bool leftScrolling = true;
        private bool rightScrolling = true;
        private bool checkDOMChanges = false;
        private int leftPaneWidth = 0;
        private Theme theme = Theme.Auto;
        private MvcTemplate leftPaneTemplate = new MvcTemplate();
        private MvcTemplate rightPaneTemplate = new MvcTemplate();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileSplitPaneIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        [JsonProperty("android")]
        public MobileSplitPaneAndroidProperties Android { get; set; }

        /// <summary>
        /// Gets or sets the left header title.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("leftHdrTitle")]
        [DefaultValue("Title")]
        public string LeftHeaderTitle { get { return leftHeaderTitle; } set { leftHeaderTitle = value; } }

        /// <summary>
        /// Gets or sets the rightheader title.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("rightHdrTitle")]
        [DefaultValue("Title")]
        public string RightHeaderTitle { get { return rightHeaderTitle; } set { rightHeaderTitle = value; } }

        /// <summary>
        /// Gets or sets the rendermode.
        /// </summary>
        /// <value>
        /// RenderMode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// Theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get; set; }

        /// <summary>
        /// Gets or sets the right scrolling.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("rightScrolling")]
        [DefaultValue(true)]
        public bool RightScrolling { get { return rightScrolling; } set { rightScrolling = value; } }


        /// <summary>
        /// Gets or sets the right scrolling.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("leftScrolling")]
        [DefaultValue(true)]
        public bool LeftScrolling { get { return leftScrolling; } set { leftScrolling = value; } }

        /// <summary>
        /// Gets or sets the checkDOMChanges.
        /// </summary>
        /// <value>
        /// true/false.
        /// </value>
        [JsonProperty("checkDOMChanges")]
        [DefaultValue(false)]
        public bool CheckDOMChanges { get { return checkDOMChanges; } set { checkDOMChanges = value; } }

        /// <summary>
        /// Gets or sets the leftPane Width.
        /// </summary>
        /// <value>
        /// value for leftpane.
        /// </value>
        [JsonProperty("leftPaneWidth")]
        [DefaultValue(0)]
        public int LeftPaneWidth { get { return leftPaneWidth; } set { leftPaneWidth = value; } }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileSplitPaneWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the leftPane template.
        /// </summary>
        /// <value>
        /// template
        /// </value>
        [JsonIgnore]
        public MvcTemplate LeftPaneTemplate { get { return leftPaneTemplate; } set { leftPaneTemplate = value; } }

        /// <summary>
        /// Gets or sets the rightPane template.
        /// </summary>
        /// <value>
        /// template
        /// </value>
        [JsonIgnore]
        public MvcTemplate RightPaneTemplate { get { return rightPaneTemplate; } set { rightPaneTemplate = value; } }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabProperties"/> class.
        /// </summary>
        public MobileSplitPaneProperties()
        {
            this.IOS7 = new MobileSplitPaneIOS7Properties();
            this.Android = new MobileSplitPaneAndroidProperties();
            this.Windows = new MobileSplitPaneWindowsProperties();
        }
        #endregion
    }
}
