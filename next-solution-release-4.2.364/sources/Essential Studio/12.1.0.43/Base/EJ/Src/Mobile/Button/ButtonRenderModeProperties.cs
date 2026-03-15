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
    /// Class for MobileButtonIOS7Properties
    /// </summary>
    public class MobileButtonIOS7Properties
    {
        #region Fields
        private IOS7ButtonStyle buttonStyle = IOS7ButtonStyle.Normal;
        private IOS7ButtonColor buttonColor = IOS7ButtonColor.Gray;
        #endregion

        #region IOS7Properties

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        [DefaultValue(IOS7ButtonStyle.Normal)]
        public IOS7ButtonStyle ButtonStyle { get { return buttonStyle; } set { buttonStyle = value; } }

        /// <summary>
        /// Gets or sets the color of the button.
        /// </summary>
        /// <value>
        /// The color of the button.
        /// </value>
        [DefaultValue(IOS7ButtonColor.Gray)]
        public IOS7ButtonColor ButtonColor { get { return buttonColor; } set { buttonColor = value; } } 

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonIOS7Properties"/> class.
        /// </summary>
        public MobileButtonIOS7Properties() { }
        #endregion
    }

    /// <summary>
    /// Class for MobileButtonAndroidProperties
    /// </summary>
    public class MobileButtonAndroidProperties
    {
        #region Fields
        private AndroidButtonStyle buttonStyle = AndroidButtonStyle.Normal;
        #endregion

        #region AndroidProperties

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        [DefaultValue(AndroidButtonStyle.Normal)]
        public AndroidButtonStyle ButtonStyle { get { return buttonStyle; } set { buttonStyle = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonAndroidProperties"/> class.
        /// </summary>
        public MobileButtonAndroidProperties() { }
        #endregion
    }
    /// <summary>
    /// Class for MobileButtonWindowsProperties
    /// </summary>
    public class MobileButtonWindowsProperties : WindowsBase
    {
        #region Fields
        private WindowsButtonStyle buttonStyle = WindowsButtonStyle.Normal;
        #endregion

        #region WindowsProperties

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        [DefaultValue(WindowsButtonStyle.Normal)]
        public WindowsButtonStyle ButtonStyle { get { return buttonStyle; } set { buttonStyle = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonWindowsProperties"/> class.
        /// </summary>
        public MobileButtonWindowsProperties() { }
        #endregion
    }
    /// <summary>
    /// Class for MobileButtonFlatProperties
    /// </summary>
    public class MobileButtonFlatProperties
    {
        #region Fields
        private FlatButtonStyle buttonStyle = FlatButtonStyle.Normal;
        #endregion

        #region FlatProperties

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        [DefaultValue(FlatButtonStyle.Normal)]
        public FlatButtonStyle ButtonStyle { get { return buttonStyle; } set { buttonStyle = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonFlatProperties"/> class.
        /// </summary>
        public MobileButtonFlatProperties() { }
        #endregion
    }

   
}
