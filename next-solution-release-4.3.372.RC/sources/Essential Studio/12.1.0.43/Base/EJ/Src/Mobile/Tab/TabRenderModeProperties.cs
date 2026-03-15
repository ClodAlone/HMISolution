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
    /// Tab IOS7 Properties
    /// </summary>
    public class MobileTabIOS7Properties
    {
        #region Fields
        private bool showImage = true;
        private string imageClass = string.Empty;
        private bool showBadge = false;
        private double moreBadgeValue = 0;
        private bool showMoreBadge = false;
        private double moreBadgeMaxValue = 100;
        #endregion

        #region IOS7Properties
        /// <summary>
        /// Gets or sets a value indicating whether [show image].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show image]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showImage")]
        [DefaultValue(true)]
        public bool ShowImage { get { return showImage; } set { showImage = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show more badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show more badge]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showMoreBadge")]
        [DefaultValue(false)]
        public bool ShowMoreBadge { get { return showMoreBadge; } set { showMoreBadge = value; } }

        /// <summary>
        /// Gets or sets the more badge value.
        /// </summary>
        /// <value>
        /// The more badge value.
        /// </value>
        [JsonProperty("moreBadgeValue")]
        [DefaultValue(0)]
        public double MoreBadgeValue { get { return moreBadgeValue; } set { moreBadgeValue = value; } }

        /// <summary>
        /// Gets or sets the more badge maximum value.
        /// </summary>
        /// <value>
        /// The more badge maximum value.
        /// </value>
        [JsonProperty("moreBadgeMaxValue")]
        [DefaultValue(100)]
        public double MoreBadgeMaxValue { get { return moreBadgeMaxValue; } set { moreBadgeMaxValue = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabIOS7Properties"/> class.
        /// </summary>
        public MobileTabIOS7Properties() { }
        #endregion
    }

    /// <summary>
    /// Tab Android Properties
    /// </summary>
    public class MobileTabAndroidProperties
    {
        #region Fields
        private bool showImage = false;
        private string imageClass = string.Empty;
        private ControlPosition position = ControlPosition.Fixed;
        #endregion

        #region AndroidProperties
        /// <summary>
        /// Gets or sets a value indicating whether [show image].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show image]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showImage")]
        [DefaultValue(false)]
        public bool ShowImage { get { return showImage; } set { showImage = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonProperty("position")]
        [DefaultValue(ControlPosition.Fixed)]
        public ControlPosition Position { get { return position; } set { position = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabAndroidProperties"/> class.
        /// </summary>
        public MobileTabAndroidProperties() { }
        #endregion
    }

    /// <summary>
    /// Tab Windows Properties
    /// </summary>
    public class MobileTabWindowsProperties : WindowsBase
    {
        #region Fields
        private bool customText = false;
        private bool renderDefault = false;
        private ControlPosition position = UserAgent.IsMobile() ? ControlPosition.Fixed : ControlPosition.Normal;
        #endregion

        #region WindowsProperties
        /// <summary>
        /// Gets or sets a value indicating whether [custom text].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [custom text]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("customText")]
        [DefaultValue(false)]
        public bool CustomText { get { return customText; } set { customText = value; } }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonProperty("position")]
        public ControlPosition Position { get { return position; } set { position = value; } }

        [JsonProperty("renderDefault")]
        [DefaultValue(false)]
        public bool RenderDefault { get { return renderDefault; } set { renderDefault = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabWindowsProperties"/> class.
        /// </summary>
        public MobileTabWindowsProperties() { }
        #endregion
    }

    /// <summary>
    /// Tab Flat Properties
    /// </summary>
    public class MobileTabFlatProperties
    {
        #region Fields
        private ControlPosition position = ControlPosition.Fixed;
        #endregion

        #region FlatProperties
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonProperty("postion")]
        [DefaultValue(ControlPosition.Fixed)]
        public ControlPosition Position { get { return position; } set { position = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabFlatProperties"/> class.
        /// </summary>
        public MobileTabFlatProperties() { }
        #endregion
    }
}
