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
        const string _Close = "Close";
        const string _Move = "Move";
        const string _Restore = "Restore";
        const string _Minimize = "Minimize";
        const string _Maximize = "Maximize";
        const string _Size = "Size";
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

        #region Predefined Alerts
        const string _Ok = "Ok";
        const string _Cancel = "Cancel";
        const string _Abort = "Abort";
        const string _Retry = "Retry";
        const string _Yes = "Yes";
        const string _No = "No";
        const string _Apply = "Apply";
#endregion
        
        const string _CalendarLabel = "CalendarLabel";
        const string _TodayLabel = "TodayLabel";
        const string _NoneLabel = "NoneLabel";

        /// <summary>
        /// 
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            close = SR.GetString(ci, _Close);
            if(SR.GetString(ci, _Move) != null)
                move = SR.GetString(ci, _Move);

            restore = SR.GetString(ci, _Restore);
            minimize = SR.GetString(ci, _Minimize);
            maximize = SR.GetString(ci, _Maximize);
            sized = SR.GetString(ci, _Size);

            ok = SR.GetString(ci, _Ok);
            cancel = SR.GetString(ci, _Cancel);
            abort = SR.GetString(ci, _Abort);
            retry = SR.GetString(ci, _Retry);
            yes = SR.GetString(ci, _Yes);
            no = SR.GetString(ci, _No);
            apply = SR.GetString(ci, _Apply);
            
            calendarLabel = SR.GetString(ci, _CalendarLabel);
            todayLabel = SR.GetString(ci, _TodayLabel);
            noneLabel = SR.GetString(ci, _NoneLabel);
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

        string calendarLabel;
        /// <summary>
        /// 
        /// </summary>
        public string CalendarLabel
        {
            get { return calendarLabel; }
            set { calendarLabel = value; }
        }

        string todayLabel;

        /// <summary>
        /// 
        /// </summary>
        public string TodayLabel
        {
            get { return todayLabel; }
            set { todayLabel = value; }
        }

        string noneLabel;
        /// <summary>
        /// 
        /// </summary>
        public string NoneLabel
        {
            get { return noneLabel; }
            set { noneLabel = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Sized
        {
            get { return sized; }
            set { sized = value; }
        }
        string sized;
        /// <summary>
        /// 
        /// </summary>
        public string Maximize
        {
            get { return maximize; }
            set { maximize = value; }
        }
        string maximize;
        /// <summary>
        /// 
        /// </summary>
        public string Minimize
        {
            get { return minimize; }
            set { minimize = value; }
        }
        string minimize;
        /// <summary>
        /// 
        /// </summary>
        public string Restore
        {
            get { return restore; }
            set { restore = value; }
        }
        string restore;
        /// <summary>
        /// 
        /// </summary>
        public string Close
        {
            get { return close; }
            set { close = value; }
        }
        string close;

        /// <summary>
        /// 
        /// </summary>
        public string AutoEllipsisString
        {
            get
            {
                return _autoEllipsisString;
            }
            set
            {
                _autoEllipsisString = value;
            }

        }
        string _autoEllipsisString;
        /// <summary>
        /// 
        /// </summary>
        public string AutomationPeerName_TotalPageCountKnown
        {
            get
            {
                return _automationPeerName_TotalPageCountKnown;
            }
            set
            {
                _automationPeerName_TotalPageCountKnown=value;
            }
        }
        string _automationPeerName_TotalPageCountKnown;
        /// <summary>
        /// 
        /// </summary>
        public string AutomationPeerName_TotalPageCountUnknown
        {
            get
            {
                return _automationPeerName_TotalPageCountUnknown;
            }
            set
            {
                _automationPeerName_TotalPageCountUnknown = value;
            }
        }
        string _automationPeerName_TotalPageCountUnknown;
         
        /// <summary>
        /// 
        /// </summary>
        public string CurrentPagePrefix_TotalPageCountKnown
        {
            get
            {
                return _currentPagePrefix_TotalPageCountKnown;
            }
            set
            {
                _currentPagePrefix_TotalPageCountKnown = value;
            }
        }
        string _currentPagePrefix_TotalPageCountKnown;
        /// <summary>
        /// 
        /// </summary>
        public string CurrentPagePrefix_TotalPageCountUnknown
        {
            get
            {
                return _currentPagePrefix_TotalPageCountUnknown;
            }
            set
            {
                _currentPagePrefix_TotalPageCountUnknown = value;
            }
        }
        string _currentPagePrefix_TotalPageCountUnknown;
        /// <summary>
        /// 
        /// </summary>
        public string CurrentPageSuffix_TotalPageCountKnown
        {
            get
            {
                return _currentPageSuffix_TotalPageCountKnown;
            }
            set
            {
                _currentPageSuffix_TotalPageCountKnown = value;
            }
        }
        string _currentPageSuffix_TotalPageCountKnown;
        /// <summary>
        /// 
        /// </summary>
        public string CurrentPageSuffix_TotalPageCountUnknown
        {
            get
            {
                return _currentPageSuffix_TotalPageCountUnknown;
            }
            set
            {
                _currentPageSuffix_TotalPageCountUnknown = value;
            }
        }
        string _currentPageSuffix_TotalPageCountUnknown;
        /// <summary>
        /// 
        /// </summary>
        public string InvalidButtonPanelContent
        {
            get
            {
                return _invalidButtonPanelContent;
            }
            set
            {
                _invalidButtonPanelContent = value;
            }
        }
        string _invalidButtonPanelContent;
        /// <summary>
        /// 
        /// </summary>
        public string InvalidTimeSpan
        {
            get
            {
                return _invalidTimeSpan;
            }
            set
            {
                _invalidTimeSpan = value;
            }
        }
        string _invalidTimeSpan;
        /// <summary>
        /// 
        /// </summary>
        public string PageIndexMustBeNegativeOne
        {
            get
            {
                return _pageIndexMustBeNegativeOne;
            }
            set
            {
                _pageIndexMustBeNegativeOne = value;
            }
        }
        string _pageIndexMustBeNegativeOne;

        /// <summary>
        /// 
        /// </summary>
        public string UnderlyingPropertyIsReadOnly
        {
            get
            {
                return _underlyingPropertyIsReadOnly;
            }
            set
            {
                _underlyingPropertyIsReadOnly = value;
            }
        }
        string _underlyingPropertyIsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        public string ValueMustBeGreaterThanOrEqualTo
        {
            get
            {
                return _valueMustBeGreaterThanOrEqualTo;
            }
            set
            {
                _valueMustBeGreaterThanOrEqualTo = value;
            }
        }
        string _valueMustBeGreaterThanOrEqualTo;
       
        /// <summary>
        /// 
        /// </summary>
        public string Move
        {
            get { return move; }
            set { move = value; }
        }
        string move;
        /// <summary>
        /// 
        /// </summary>
        public string Ok
        {
            get { return ok; }
            set { ok = value; }
        }
        string ok;
        /// <summary>
        /// 
        /// </summary>
        public string Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
        string cancel;
        /// <summary>
        /// 
        /// </summary>
        public string Apply
        {
            get { return apply; }
            set { apply = value; }
        }
        string apply;

        /// <summary>
        /// 
        /// </summary>
        public string Yes
        {
            get { return yes; }
            set { yes = value; }
        }
        string yes;
        /// <summary>
        /// 
        /// </summary>
        public string No
        {
            get { return no; }
            set { no = value; }
        }
        string no;
        /// <summary>
        /// 
        /// </summary>
        public string Abort
        {
            get { return abort; }
            set { abort = value; }
        }
        string abort;
        /// <summary>
        /// 
        /// </summary>
        public string Retry
        {
            get { return retry; }
            set { retry = value; }
        }
        string retry;

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
