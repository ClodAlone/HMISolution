#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Web;
using System.Text.RegularExpressions;

namespace Syncfusion.JavaScript
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TVal> Merge<TKey, TVal>(this IDictionary<TKey, TVal> dictA, IDictionary<TKey, TVal> dictB)
        { 
            foreach (KeyValuePair<TKey, TVal> pair in dictB)
            {
                // TODO: Check for collisions?
                dictA.Add(pair.Key, pair.Value);
            }

            return dictA;
        }
    }

    public static class UserAgent
    {
        /// <summary>
        /// Determines whether this instance is android.
        /// </summary>
        /// <returns></returns>
        public static bool IsAndroid()
        {
            var exp = new Regex("android");
            return HttpContext.Current.Request.UserAgent != null && exp.IsMatch(HttpContext.Current.Request.UserAgent.ToLower());
        }

        /// <summary>
        /// Gets the android version.
        /// </summary>
        /// <returns></returns>
        public static float GetAndroidVersion()
        {
            string ua = HttpContext.Current.Request.UserAgent.ToLower();
            var temp = ua.Substring(ua.IndexOf("android") + 8);
            var andVersion = temp.Substring(0, temp.IndexOf(';'));
            var andVersionTemp = andVersion.Split('.');
            if (andVersionTemp.Length > 2)
                andVersion = andVersion.Substring(0, andVersion.LastIndexOf("."));
            return float.Parse(andVersion);
        }

        /// <summary>
        /// Determines whether [is lower android].
        /// </summary>
        /// <returns></returns>
        public static bool IsLowerAndroid()
        {
            return GetAndroidVersion() <= 4.0 ? true : false;
        }

        /// <summary>
        /// Determines whether [is io s7].
        /// </summary>
        /// <returns></returns>
        public static bool IsIOS7()
        {
            var exp = new Regex(@"(ipad|iphone|ipod touch);.*os 7_\d");
            return HttpContext.Current.Request.UserAgent != null && exp.IsMatch(HttpContext.Current.Request.UserAgent.ToLower());
        }

        /// <summary>
        /// Determines whether this instance is windows.
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows()
        {
            var exp = new Regex("trident|windows phone");
            return HttpContext.Current.Request.UserAgent != null && exp.IsMatch(HttpContext.Current.Request.UserAgent.ToLower());
        }

        /// <summary>
        /// Determines whether this instance is device.
        /// </summary>
        /// <returns></returns>
        public static bool IsDevice()
        {
            var exp = new Regex("mobile|tablet|android|kindle");
            return HttpContext.Current.Request.UserAgent != null && exp.IsMatch(HttpContext.Current.Request.UserAgent.ToLower());
        }

        /// <summary>
        /// Determines whether this instance is mobile.
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            var exp = new Regex("iphone|ipod|android|blackberry|opera|mini|windows\\sce|palm|smartphone|iemobile");
            return HttpContext.Current.Request.UserAgent != null && exp.IsMatch(HttpContext.Current.Request.UserAgent.ToLower());
        }
        
    }

    public interface IMobileBase
    {
        RenderMode RenderMode { get; set; }
        Theme Theme { get; set; }
      
    }
    
    public class WindowsBase
    {
        [JsonProperty("renderDefault")]
        [DefaultValue(false)]
        public bool RenderDefault { get; set; }
    }

    public enum IOS7MenuType
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "animate")]
        Animate
    }

    public enum AndroidMenuType
    {
        [EnumMember(Value = "contextual")]
        Contextual,
        [EnumMember(Value = "optionslist")]
        OptionsList,
        [EnumMember(Value = "optionsmenu")]
        OptionsMenu,
        [EnumMember(Value = "popup")]
        Popup
    }

    public enum WindowsMenuType
    {
        [EnumMember(Value = "contextual")]
        Contextual,
        [EnumMember(Value = "popup")]
        Popup
    }
    [DataContract]
    public enum RenderMode
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "ios7")]
        IOS7,
        [EnumMember(Value = "android")]
        Android,
        [EnumMember(Value = "windows")]
        Windows,
        [EnumMember(Value = "flat")]
        Flat
    }
    [DataContract]
    public enum Theme
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "dark")]
        Dark,
        [EnumMember(Value = "light")]
        Light
    }

    [DataContract]
    public enum ControlPosition
    {
        [EnumMember(Value = "Fixed")]
        Fixed,
        [EnumMember(Value = "Normal")]
        Normal
    }
    [DataContract]
    public enum IOS7ButtonStyle
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Back")]
        Back,
        [EnumMember(Value = "Header")]
        Header,
        [EnumMember(Value = "Dialog")]
        Dialog
    }
    [DataContract]
    public enum AndroidButtonStyle
    {
        [EnumMember(Value = "Small")]
        Small,
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Dialog")]
        Dialog
    }
    [DataContract]
    public enum WindowsButtonStyle
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Back")]
        Back
    }
    [DataContract]
    public enum FlatButtonStyle
    {
        [EnumMember(Value = "Normal")]
        Normal
    }
    [DataContract]
    public enum IOS7ButtonColor
    {
        [EnumMember(Value = "Blue")]
        Blue,
        [EnumMember(Value = "Red")]
        Red,
        [EnumMember(Value = "Green")]
        Green,
        [EnumMember(Value = "Black")]
        Black,
        [EnumMember(Value = "Gray")]
        Gray
    }

    [DataContract]
    public enum GroupButtonType
    {
        [EnumMember(Value = "radio")]
        radio,
        [EnumMember(Value = "checkbox")]
        checkbox
    }

}