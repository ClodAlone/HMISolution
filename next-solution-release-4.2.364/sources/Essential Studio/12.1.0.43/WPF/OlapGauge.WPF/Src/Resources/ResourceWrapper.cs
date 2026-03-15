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

namespace Syncfusion.Windows.Gauge.Olap.Resources
{
    public sealed class ResourceWrapper
    {
        #region Constants

        const string TOOLTIP_POINTER = "txtPointerTooltip";
        const string TOOLTIP_MARKER = "txtMarkerTooltip";
        //const string LOADING_TEXT = "OlapGauge_LoadingIndicator_Text";

        #endregion

        #region Constructor

        public ResourceWrapper()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            markertext = SR.GetString(culture, TOOLTIP_MARKER);
            //toolTipGoal = SR.GetString(culture, TOOLTIP_POINTER);
        }

        #endregion

        #region Private Variable

        private string toolTipValue;
        private string toolTipGoal;
        private string markertext;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets the LoadingText for localization use.
        /// </summary>
        public string MarkerText
        {
            get { return markertext; }
            set { markertext = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTipValue for localization use.
        /// </summary>
        //public string ToolTipValue
        //{
        //    get { return toolTipValue; }
        //    set { toolTipValue = value; }
        //}

        /// <summary>
        /// Gets or Sets ToolTipGoal for localization use.
        /// </summary>
        //public string ToolTipGoal
        //{
        //    get { return toolTipGoal; }
        //    set { toolTipGoal = value; }
        //}

        #endregion
    }
}
