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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Tab IOS7 Properties Builder
    /// </summary>
    public class MobileTabIOS7PropertiesBuilder
    {
        #region Fields
        private MobileTabProperties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabIOS7PropertiesBuilder(MobileTabProperties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Shows the image.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [show image].</param>
        /// <returns></returns>
        public MobileTabIOS7PropertiesBuilder ShowImage(bool showImage)
        {
            this.MobileTabModel.IOS7.ShowImage = showImage;
            return this;
        }

        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileTabIOS7PropertiesBuilder ImageClass(string imageClass)
        {
            this.MobileTabModel.IOS7.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Shows the more badge.
        /// </summary>
        /// <param name="showMoreBadge">if set to <c>true</c> [show more badge].</param>
        /// <returns></returns>
        public MobileTabIOS7PropertiesBuilder ShowMoreBadge(bool showMoreBadge)
        {
            this.MobileTabModel.IOS7.ShowMoreBadge = showMoreBadge;
            return this;
        }

        /// <summary>
        /// Mores the badge value.
        /// </summary>
        /// <param name="moreBadgeValue">The more badge value.</param>
        /// <returns></returns>
        public MobileTabIOS7PropertiesBuilder MoreBadgeValue(double moreBadgeValue)
        {
            this.MobileTabModel.IOS7.MoreBadgeValue = moreBadgeValue;
            return this;
        }

        /// <summary>
        /// Mores the badge maximum value.
        /// </summary>
        /// <param name="moreBadgeMaxValue">The more badge maximum value.</param>
        /// <returns></returns>
        public MobileTabIOS7PropertiesBuilder MoreBadgeMaxValue(double moreBadgeMaxValue)
        {
            this.MobileTabModel.IOS7.MoreBadgeMaxValue = moreBadgeMaxValue;
            return this;
        }
        #endregion

    }

    /// <summary>
    /// Tab Android Properties Builder
    /// </summary>
    public class MobileTabAndroidPropertiesBuilder
    {
        #region Fields
        private MobileTabProperties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabAndroidPropertiesBuilder(MobileTabProperties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Shows the image.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [show image].</param>
        /// <returns></returns>
        public MobileTabAndroidPropertiesBuilder ShowImage(bool showImage)
        {
            this.MobileTabModel.Android.ShowImage = showImage;
            return this;
        }

        /// <summary>
        /// Images the class.
        /// </summary>
        /// <param name="imageClass">The image class.</param>
        /// <returns></returns>
        public MobileTabAndroidPropertiesBuilder ImageClass(string imageClass)
        {
            this.MobileTabModel.Android.ImageClass = imageClass;
            return this;
        }

        /// <summary>
        /// Positions the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public MobileTabAndroidPropertiesBuilder Position(ControlPosition position)
        {
            this.MobileTabModel.Android.Position = position;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Tab Windows Properties Builder
    /// </summary>
    public class MobileTabWindowsPropertiesBuilder
    {
        #region Fields
        private MobileTabProperties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabWindowsPropertiesBuilder(MobileTabProperties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Customs the text.
        /// </summary>
        /// <param name="customText">if set to <c>true</c> [custom text].</param>
        /// <returns></returns>
        public MobileTabWindowsPropertiesBuilder CustomText(bool customText)
        {
            this.MobileTabModel.Windows.CustomText = customText;
            return this;
        }

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileTabWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileTabModel.Windows.RenderDefault = renderDefault;
            return this;
        }

        /// <summary>
        /// Positions the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public MobileTabWindowsPropertiesBuilder Position(ControlPosition position)
        {
            this.MobileTabModel.Windows.Position = position;
            return this;
        }
        #endregion
    }

    /// <summary>
    /// Tab Flat Properties Builder
    /// </summary>
    public class MobileTabFlatPropertiesBuilder
    {
        #region Fields
        private MobileTabProperties MobileTabModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabFlatPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabFlatPropertiesBuilder(MobileTabProperties mTabModel)
        {
            this.MobileTabModel = mTabModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Positions the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public MobileTabFlatPropertiesBuilder Position(ControlPosition position)
        {
            this.MobileTabModel.Windows.Position = position;
            return this;
        }
        #endregion
    }
}
