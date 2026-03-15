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
    /// Menu IOS7 Properties Builder
    /// </summary>
    public class MobileMenuIOS7PropertiesBuilder
    {
        #region Fields
        private MobileMenuProperties MobileMenuModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuIOS7PropertiesBuilder(MobileMenuProperties mMenuModel)
        {
            this.MobileMenuModel = mMenuModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Menus the type.
        /// </summary>
        /// <param name="menuType">Type of the menu.</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder MenuType(IOS7MenuType menuType)
        {
            this.MobileMenuModel.IOS7.Menutype = menuType;
            return this;
        }

        /// <summary>
        /// Shows the title.
        /// </summary>
        /// <param name="showTitle">if set to <c>true</c> [show title].</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder ShowTitle(bool showTitle)
        {
            this.MobileMenuModel.IOS7.ShowTitle = showTitle;
            return this;
        }

        /// <summary>
        /// Titles the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder Title(string title)
        {
            this.MobileMenuModel.IOS7.Title = title;
            return this;
        }

        /// <summary>
        /// Shows the cancel.
        /// </summary>
        /// <param name="showCancel">if set to <c>true</c> [show cancel].</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder ShowCancel(bool showCancel)
        {
            this.MobileMenuModel.IOS7.ShowCancel = showCancel;
            return this;
        }

        /// <summary>
        /// Cancels the button text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder CancelButtonText(string text)
        {
            this.MobileMenuModel.IOS7.CancelButtonText = text;
            return this;
        }

        /// <summary>
        /// Cancels the color of the button.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder CancelButtonColor(string color)
        {
            this.MobileMenuModel.IOS7.CancelButtonColor = color;
            return this;
        }

        /// <summary>
        /// Called when [cancel touch end].
        /// </summary>
        /// <param name="cancelTouchEnd">The cancel touch end.</param>
        /// <returns></returns>
        public MobileMenuIOS7PropertiesBuilder CancelTouchEnd(string cancelTouchEnd)
        {
            this.MobileMenuModel.IOS7.CancelTouchEnd = cancelTouchEnd;
            return this;
        }
        #endregion

    }

    /// <summary>
    /// Menu Android Properties Builder
    /// </summary>
    public class MobileMenuAndroidPropertiesBuilder
    {
        #region Fields
        private MobileMenuProperties MobileMenuModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuAndroidPropertiesBuilder(MobileMenuProperties mMenuModel)
        {
            this.MobileMenuModel = mMenuModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Menus the type.
        /// </summary>
        /// <param name="menuType">Type of the menu.</param>
        /// <returns></returns>
        public MobileMenuAndroidPropertiesBuilder MenuType(AndroidMenuType menuType)
        {
            this.MobileMenuModel.Android.MenuType = menuType;
            return this;
        }
        #endregion
    }

    /// <summary>
    /// Menu Windows Properties Builder
    /// </summary>
    public class MobileMenuWindowsPropertiesBuilder
    {
        #region Fields
        private MobileMenuProperties MobileMenuModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuWindowsPropertiesBuilder(MobileMenuProperties mMenuModel)
        {
            this.MobileMenuModel = mMenuModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Menus the type.
        /// </summary>
        /// <param name="menuType">Type of the menu.</param>
        /// <returns></returns>
        public MobileMenuWindowsPropertiesBuilder MenuType(WindowsMenuType menuType)
        {
            this.MobileMenuModel.Windows.MenuType = menuType;
            return this;
        }
        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileMenuWindowsPropertiesBuilder RenderDefault(bool render)
        {
            this.MobileMenuModel.Windows.RenderDefault = render;
            return this;
        }
        #endregion
    }

}