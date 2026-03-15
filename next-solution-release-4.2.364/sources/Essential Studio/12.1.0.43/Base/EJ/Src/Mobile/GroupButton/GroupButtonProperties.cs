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
    public class MobileGroupButtonProperties : IMobileBase
    {
        #region Fields
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private GroupButtonType groupButtonType = GroupButtonType.radio;
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
        /// Gets or sets the on touch start.
        /// </summary>
        /// <value>
        /// The on touch start.
        /// </value>
        [JsonProperty("touchstart")]
        public string touchStart { get; set; }

        /// <summary>
        /// Gets or sets the on touch end.
        /// </summary>
        /// <value>
        /// The on touch end.
        /// </value>
        [JsonProperty("touchend")]
        public string touchEnd { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
       [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the menutype.
        /// </summary>
        /// <value>
        /// The menutype.
        /// </value>
        [JsonProperty("groupButtonType")]
        [DefaultValue(GroupButtonType.radio)]
        public GroupButtonType GroupButtonType { get { return groupButtonType; } set { groupButtonType = value; } }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public MobileGroupButtonWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        /// 
        [JsonIgnore]
        public List<MobileGroupButtonBaseItem> Buttons
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButtonProperties"/> class.
        /// </summary>
        public MobileGroupButtonProperties()
        {
            this.Windows = new MobileGroupButtonWindowsProperties();
            this.Buttons = new List<MobileGroupButtonBaseItem>();
        }
        #endregion

    }
}
