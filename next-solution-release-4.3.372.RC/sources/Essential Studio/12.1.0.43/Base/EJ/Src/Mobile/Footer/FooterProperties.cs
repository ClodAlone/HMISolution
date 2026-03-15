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
    public class MobileFooterProperties : IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool hideForUnSupportedDevice = false;
        private string leftButtonNavigationUrl = null;
        private string rightButtonNavigationUrl = null;
        private string title = "Title";
        private bool showTitle = true;
        private FooterPosition position = FooterPosition.Fixed;
        private string leftButtonCaption = "Back";
        private string rightButtonCaption = "Right";
        private bool showLeftButton = false;
        private bool showRightButton = false;
        private string templateId = null;

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
        /// Gets or sets a value indicating whether [hide for un supported device].
        /// </summary>
        /// <value>
        /// <c>true</c> if [hide for un supported device]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("hideForUnSupportedDevice")]
        [DefaultValue(false)]
        public bool HideForUnSupportedDevice { get { return hideForUnSupportedDevice; } set { hideForUnSupportedDevice = value; } }

        /// <summary>
        /// Gets or sets the left button navigation URL.
        /// </summary>
        /// <value>
        /// The left button navigation URL.
        /// </value>
        [JsonProperty("leftButtonNavigationUrl")]
        [DefaultValue(null)]
        public string LeftButtonNavigationUrl { get { return leftButtonNavigationUrl; } set { leftButtonNavigationUrl = value; } }

        /// <summary>
        /// Gets or sets the right button navigation URL.
        /// </summary>
        /// <value>
        /// The right button navigation URL.
        /// </value>
        [JsonProperty("rightButtonNavigationUrl")]
        [DefaultValue(null)]
        public string RightButtonNavigationUrl { get { return rightButtonNavigationUrl; } set { rightButtonNavigationUrl = value; } }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        [DefaultValue("Title")]
        public string Title { get { return title; } set { title = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show title]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showTitle")]
        [DefaultValue(true)]
        public bool ShowTitle { get { return showTitle; } set { showTitle = value; } }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonProperty("position")]
        [DefaultValue(FooterPosition.Fixed)]
        public FooterPosition Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Gets or sets the left button caption.
        /// </summary>
        /// <value>
        /// The left button caption.
        /// </value>
        [JsonProperty("leftButtonCaption")]
        [DefaultValue("Back")]
        public string LeftButtonCaption { get { return leftButtonCaption; } set { leftButtonCaption = value; } }

        /// <summary>
        /// Gets or sets the right button caption.
        /// </summary>
        /// <value>
        /// The right button caption.
        /// </value>
        [JsonProperty("rightButtonCaption")]
        [DefaultValue("Right")]
        public string RightButtonCaption { get { return rightButtonCaption; } set { rightButtonCaption = value; } }


        /// <summary>
        /// Gets or sets a value indicating whether [show left button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show left button]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showLeftButton")]
        [DefaultValue(false)]
        public bool ShowLeftButton { get { return showLeftButton; } set { showLeftButton = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show right button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show right button]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showRightButton")]
        [DefaultValue(false)]
        public bool ShowRightButton { get { return showRightButton; } set { showRightButton = value; } }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        [DefaultValue(null)]
        public string TemplateID { get { return templateId; } set { templateId = value; } }

        /// <summary>
        /// Gets or sets the on left button click.
        /// </summary>
        /// <value>
        /// The on left button click.
        /// </value>
        [JsonProperty("leftButtonTap")]
        public string LeftButtonTap { get; set; }

        /// <summary>
        /// Gets or sets the on right button click.
        /// </summary>
        /// <value>
        /// The on right button click.
        /// </value>
        [JsonProperty("rightButtonTap")]
        public string RightButtonTap { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileFooterWindowsProperties Windows { get; set; }

        #endregion

       #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileFooterProperties"/> class.
        /// </summary>
        public MobileFooterProperties()
        {
            this.Windows = new MobileFooterWindowsProperties();
        }
       #endregion

    }
}
