#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    #region Skin

    /// <summary>
    /// Defines identifiers that represent control themes.
    /// </summary>
    public enum Skin
    {
        /// <summary>
        /// Indicates usage of default control theme
        /// </summary>
        Default,

        /// <summary>
        /// Indicates usage of Blend theme
        /// </summary>
        Blend,

        /// <summary>
        /// Indicates usage of Office 2003 theme
        /// </summary>
        Office2003,

        /// <summary>
        /// Indicates usage of Office 2007 blue theme
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Indicates usage of Office 2007 black theme
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Indicates usage of Office 2007 silver theme
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Indicates usage of Office 2007 blue theme
        /// </summary>
        SyncOrange,

        /// <summary>
        /// Indicates usage of Office 2007 black theme
        /// </summary>
        ShinyRed,

        /// <summary>
        /// Indicates usage of Office 2007 silver theme
        /// </summary>
        ShinyBlue,

        /// <summary>
        /// Indicates usage of Office 2010 Blue theme
        /// </summary>
        Office2010Blue,

        /// <summary>
        /// Indicates usage of Office 2010 Black theme
        /// </summary>
        Office2010Black,

        /// <summary>
        /// Indicates usage of Office 2010 Silver theme
        /// </summary>
        Office2010Silver,

        /// <summary>
        /// Indicates usage of VS2010 theme
        /// </summary>
        VS2010,

        /// <summary>
        /// Indicates usage of Metro theme
        /// </summary>
        Metro,
        /// <summary>
        /// Indicates usage of Transparent theme
        /// </summary>

        Transparent,
        /// <summary>
        /// Indicates usage of Office2013 theme
        /// </summary>
        Office2013,
        
        /// <summary>
        /// Indicates usage of Windows8 theme
        /// </summary>
        Windows8
    }

    #endregion

    /// <summary>
    /// Custom Attribute for mentioning theming dictionary,
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SkinTypeAttribute : Attribute
    {
        private Skin skinVisualStyle;
        /// <summary>
        /// 
        /// </summary>
        public Skin SkinVisualStyle { get { return skinVisualStyle; } set { skinVisualStyle = value; } }

        private string xamlResource;
        /// <summary>
        /// 
        /// </summary>
        public string XamlResource { get { return xamlResource; } set { xamlResource = value; } }

        private Type type;
        /// <summary>
        /// 
        /// </summary>
        public Type Type { get { return type; } set { type = value; } }
        /// <summary>
        /// 
        /// </summary>
        public SkinTypeAttribute()
        {

        }
    }
}
