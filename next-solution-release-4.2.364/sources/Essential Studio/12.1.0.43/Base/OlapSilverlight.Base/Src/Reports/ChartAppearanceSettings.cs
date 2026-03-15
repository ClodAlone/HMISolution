#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.ComponentModel;
namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Represents the Chart Appearance settings.
    /// </summary>
    public class ChartAppearanceSettings
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAppearanceSettings"/> class.
        /// </summary>
        public ChartAppearanceSettings()
        {
            this.ShowSeriesTooltip = true;
            this.ShowLegend = true;
            this.ShowProcessingBar = true;
            this.ShowLegendIcon = true;
        }
        #endregion

        #region Chart Toolbar Properties
        /// <summary>
        /// Gets or sets a value indicating whether [show series tooltip].
        /// </summary>
        /// <value><c>true</c> if [show series tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowSeriesTooltip { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show processing bar].
        /// </summary>
        /// <value><c>true</c> if [show processing bar]; otherwise, <c>false</c>.</value>
        public bool ShowProcessingBar { get; set; }
        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public string ChartType { get; set; }
        /// <summary>
        /// Gets or sets the chart color palette.
        /// </summary>
        /// <value>The chart color palette.</value>
        public string ChartColorPalette { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show legend].
        /// </summary>
        /// <value><c>true</c> if [show legend]; otherwise, <c>false</c>.</value>
        public bool ShowLegend { get; set; }
        /// <summary>
        /// Gets or sets the legend dock position.
        /// </summary>
        /// <value>The legend dock position.</value>
        public string LegendDockPosition { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show legend checkbox].
        /// </summary>
        /// <value><c>true</c> if [show legend checkbox]; otherwise, <c>false</c>.</value>
        public bool ShowLegendCheckbox { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show legend icon].
        /// </summary>
        /// <value><c>true</c> if [show legend icon]; otherwise, <c>false</c>.</value>
        public bool ShowLegendIcon { get; set; }
        /// <summary>
        /// Gets or sets the chart visual style.
        /// </summary>
        /// <value>The chart visual style.</value>
        public string ChartVisualStyle { get; set; }
        #endregion
    }
}
