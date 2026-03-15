#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileToolbarProperties : ToolBarPropertiesBase, IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private ToolbarPosition toolbarposition = ToolbarPosition.Fixed;

        private string touchStart = "";
        private string touchEnd = "";
        private string create = "";
        private string destroy = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }



        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        public string TouchStart { get; set; }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        public string TouchEnd { get; set; }

        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>
        /// The create.
        /// </value>
        [JsonProperty("create")]
        public string Create { get; set; }

        /// <summary>
        /// Gets or sets the destroy.
        /// </summary>
        /// <value>
        /// The destroy.
        /// </value>
        [JsonProperty("destroy")]
        public string Destroy { get; set; }



        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
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
        /// Gets or sets the toolbar position.
        /// </summary>
        /// <value>
        /// The toolbar position.
        /// </value>
        [JsonProperty("toolbarPosition")]
        [DefaultValue(ToolbarPosition.Fixed)]
        public ToolbarPosition ToolbarPosition { get { return toolbarposition; } set { toolbarposition = value; } }



        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        internal MvcTemplate<Toolbar> Content { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileToolbarBaseItem> Items
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public MobileToolbarWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        public MobileToolbarAndroidProperties Android { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarProperties"/> class.
        /// </summary>
        public MobileToolbarProperties()
        {
            this.Windows = new MobileToolbarWindowsProperties();
            this.Android = new MobileToolbarAndroidProperties();
            this.Items = new List<MobileToolbarBaseItem>();
        }

        #endregion
    }

}
