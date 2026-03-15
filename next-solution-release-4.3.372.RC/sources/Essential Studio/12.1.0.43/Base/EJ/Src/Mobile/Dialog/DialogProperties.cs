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
    public class MobileDialogProperties : DialogPropertiesBase,IMobileBase
    {
        #region Fields  
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool allowScrolling = true;       
        private bool showButton = true;
        private bool checkDOMChanges = false;                
        private DialogMode dialogMode = DialogMode.Alert;
        private string leftButtonCaption="Cancel";
        private string rightButtonCaption = "Continue";       
        #endregion

        #region Properties

        public MobileDialogProperties()
        {
            this.Content = new MvcTemplate<MobileDialogProperties>();
            this.Windows = new DialogRenderModeProperties();
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
        /// Gets or sets a value indicating whether [allow scrolling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow scrolling]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowScrolling")]
        [DefaultValue(true)]
        public bool AllowScrolling { get { return allowScrolling; } set { allowScrolling = value; } }        

        /// <summary>
        /// Gets or sets a value indicating whether [showbutton].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showbutton]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showButton")]
        [DefaultValue(true)]
        public bool ShowButton { get { return showButton; } set { showButton = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [checkDOMChanges].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [checkDOMChanges]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("checkDOMChanges")]
        [DefaultValue(false)]
        public bool CheckDOMChanges { get { return checkDOMChanges; } set { checkDOMChanges = value; } }

        /// <summary>
        /// Gets or sets the dialogMode value.
        /// </summary>
        /// <value>
        /// The dialogMode value.
        /// </value>
        [JsonProperty("dialogMode")]
        [DefaultValue(DialogMode.Alert)]
        public DialogMode DialogMode{ get { return dialogMode; } set { dialogMode = value; } }

        /// <summary>
        /// Gets or sets the leftButtonCaption value.
        /// </summary>
        /// <value>
        /// The leftButtonCaption value.
        /// </value>
        [JsonProperty("leftButtonCaption")]
        [DefaultValue("Cancel")]
        public string LeftButtonCaption { get { return leftButtonCaption; } set { leftButtonCaption = value; } }

        /// <summary>
        /// Gets or sets the rightButtonCaption.
        /// </summary>
        /// <value>
        /// The rightButtonCaption value.
        /// </value>
        [JsonProperty("rightButtonCaption")]
        public string RightButtonCaption { get { return rightButtonCaption; } set { rightButtonCaption = value; } }      

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        [JsonProperty("template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>
        /// The Content.
        /// </value>
        [JsonIgnore]
        public MvcTemplate<MobileDialogProperties> Content { get; set; }

        /// <summary>
        /// Gets or sets the targetHeight.
        /// </summary>
        /// <value>
        /// The targetHeight.
        /// </value>
        [JsonProperty("targetHeight")]
        public int TargetHeight { get; set; }

        /// <summary>
        /// Gets or sets the buttonTap.
        /// </summary>
        /// <value>
        /// The buttonTap.
        /// </value>
        [JsonProperty("buttonTap")]
        public string ButtonTap { get; set; }

        /// <summary>
        /// Gets or sets the open.
        /// </summary>
        /// <value>
        /// The open.
        /// </value>
        [JsonProperty("open")]
        public string Open { get; set; }       

        /// <summary>
        /// Gets or sets the beforeClose.
        /// </summary>
        /// <value>
        /// The beforeClose.
        /// </value>
        [JsonProperty("beforeClose")]
        public string BeforeClose { get; set; }

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>
        /// The close.
        /// </value>
        [JsonProperty("close")]
        public string Close { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public DialogRenderModeProperties Windows { get; set; }


        #endregion Properties
    }
     public enum DialogMode
    {
        Alert,
        Confirm,
        Normal,
        Fullview
    }   
}
