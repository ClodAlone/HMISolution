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
using System.Runtime.Serialization;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Menu IOS7 Properties
    /// </summary>
    public class MobileMenuIOS7Properties
    {
        #region Fields
        private bool showTitle = true;
        private bool showCancel = true;
        private IOS7MenuType menuType = IOS7MenuType.Auto;
        #endregion

        #region IOS7Properties
        /// <summary>
        /// Gets or sets the menutype.
        /// </summary>
        /// <value>
        /// The menutype.
        /// </value>
        [DefaultValue(IOS7MenuType.Auto)]
        public IOS7MenuType Menutype { get { return menuType; } set { menuType = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show title]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowTitle { get { return showTitle; } set { showTitle = value; } }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DefaultValue("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show cancel].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show cancel]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowCancel { get { return showCancel; } set { showCancel = value; } }

        /// <summary>
        /// Gets or sets the cancel button text.
        /// </summary>
        /// <value>
        /// The cancel button text.
        /// </value>
        [DefaultValue("Cancel")]
        public string CancelButtonText { get; set; }

        /// <summary>
        /// Gets or sets the color of the cancel button.
        /// </summary>
        /// <value>
        /// The color of the cancel button.
        /// </value>
        [DefaultValue(IOS7ButtonColor.Blue)]
        public string CancelButtonColor { get; set; }

        /// <summary>
        /// Gets or sets the on cancel touch end.
        /// </summary>
        /// <value>
        /// The on cancel touch end.
        /// </value>
        public string CancelTouchEnd { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuIOS7Properties"/> class.
        /// </summary>
        public MobileMenuIOS7Properties() { }
        #endregion
    }

    /// <summary>
    /// Menu Android Properties
    /// </summary>
    public class MobileMenuAndroidProperties
    {
        #region Fields
        private AndroidMenuType menuType = AndroidMenuType.Contextual;
        #endregion

        #region AndroidProperties
        /// <summary>
        /// Gets or sets the type of the menu.
        /// </summary>
        /// <value>
        /// The type of the menu.
        /// </value>
        [DefaultValue(AndroidMenuType.Contextual)]
        public AndroidMenuType MenuType { get { return menuType; } set { menuType = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuAndroidProperties"/> class.
        /// </summary>
        public MobileMenuAndroidProperties() { }
        #endregion
    }

    /// <summary>
    /// Menu Windows Properties
    /// </summary>
    public class MobileMenuWindowsProperties : WindowsBase
    {
        #region Fields
        private WindowsMenuType menuType = WindowsMenuType.Contextual;
        #endregion

        #region WindowsProperties
        /// <summary>
        /// Gets or sets the type of the menu.
        /// </summary>
        /// <value>
        /// The type of the menu.
        /// </value>
        [DefaultValue(WindowsMenuType.Contextual)]
        public WindowsMenuType MenuType { get { return menuType; } set { menuType = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuWindowsProperties"/> class.
        /// </summary>
        public MobileMenuWindowsProperties() { }
        #endregion
    }

}
