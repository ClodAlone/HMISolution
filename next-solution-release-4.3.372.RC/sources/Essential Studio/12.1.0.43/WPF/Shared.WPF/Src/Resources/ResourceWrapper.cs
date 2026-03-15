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
using System.Globalization;

namespace Syncfusion.Windows.Shared.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceWrapper
    {
        const string AccessCalendarTextName = "AccessCalendarText";
        const string AccessWatchTextName = "AccessWatchText";
        const string AccessEmptyDateTextName = "AccessEmptyDateText";
        const string AccessTodayTextName = "AccessTodayText";
        const string MinimizeTooltipName = "MinimizeTooltip";
        const string MaximizeTooltipName = "MaximizeTooltip";
        const string CloseTooltipName = "CloseTooltip";
        const string RestoreTooltipName = "RestoreTooltip";
        const string TodayLabelName = "TodayLabel";
        const string AccessClockTextName = "AccessClockText";
        const string StandardColorsTextName = "StandardColorsText";
        const string ThemeColorsTextName = "ThemeColorsText";
        const string RecentlyUsedTextName = "RecentlyUsedText";
        const string MoreColorsTextName = "MoreColorsText";
        const string AutomaticTextName = "AutomaticText";
        const string StandardTextName = "StandardText";
        const string ColorsTextName = "ColorsText";
        const string CustomTextName = "CustomText";
        const string ColorModelsTextName = "ColorModelsText";
        const string CancelTextName = "CancelText";
        const string OKTextName = "OKText";
        const string NewTextName = "NewText";
        const string CurrentTextName = "CurrentText";
        const string MoreColorsWindowTitleName = "MoreColorsWindowTitleText";
        /// <summary>
        /// 
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            AccessCalendarText = SR.GetString(ci, AccessCalendarTextName);
            AccessWatchText = SR.GetString(ci, AccessWatchTextName);
            AccessEmptyDateText = SR.GetString(ci, AccessEmptyDateTextName);
            AccessTodayText = SR.GetString(ci, AccessTodayTextName);
            MinimizeTooltip = SR.GetString(ci, MinimizeTooltipName);
            MaximizeTooltip = SR.GetString(ci, MaximizeTooltipName);
            CloseTooltip = SR.GetString(ci, CloseTooltipName);
            RestoreTooltip = SR.GetString(ci, RestoreTooltipName);
            TodayLabel = SR.GetString(ci, TodayLabelName);
            AccessClockText = SR.GetString(ci, AccessClockTextName);
            StandardColorsText = SR.GetString(ci, StandardColorsTextName);
            ThemeColorsText = SR.GetString(ci, ThemeColorsTextName);
            RecentlyUsedText = SR.GetString(ci, RecentlyUsedTextName);
            MoreColorsText = SR.GetString(ci, MoreColorsTextName);
            AutomaticText = SR.GetString(ci, AutomaticTextName);
            StandardText = SR.GetString(ci, StandardTextName);
            ColorsText = SR.GetString(ci, ColorsTextName);
            CustomText = SR.GetString(ci, CustomTextName);
            ColorModelsText = SR.GetString(ci, ColorModelsTextName);
            CancelText = SR.GetString(ci, CancelTextName);
            OKText = SR.GetString(ci, OKTextName);
            NewText = SR.GetString(ci, NewTextName);
            CurrentText = SR.GetString(ci, CurrentTextName);
            MoreColorsWindowTitleText = SR.GetString(ci, MoreColorsWindowTitleName);
        }
       /// <summary>
       /// 
       /// </summary>
        public string  AccessCalendarText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccessWatchText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccessEmptyDateText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccessTodayText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinimizeTooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MaximizeTooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CloseTooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RestoreTooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TodayLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccessClockText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ThemeColorsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StandardColorsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RecentlyUsedText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MoreColorsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AutomaticText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StandardText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColorsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColorModelsText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CancelText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OKText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NewText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MoreColorsWindowTitleText { get; set; }
    }
}
