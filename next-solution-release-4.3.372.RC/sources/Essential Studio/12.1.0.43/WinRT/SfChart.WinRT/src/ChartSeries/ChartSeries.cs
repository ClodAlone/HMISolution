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
#if WINDOWS_PHONE
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif


namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for ChartSeries
    /// </summary>
    public abstract class ChartSeries : ChartSeriesBase
    {
        #region properties
        /// <summary>
        /// Get or Set Area property 
        /// </summary>
        public SfChart Area
        {
            get { return ActualArea as SfChart; }
            set { ActualArea = value; }
        }

        #endregion

        #region methods

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render sfchart
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            AdornmentPresenter = this.GetTemplateChild("adornmentPresenter") as ChartAdornmentPresenter;
            SeriesRootPanel = this.GetTemplateChild("PART_SeriesRootPanel") as Panel;
            SeriesPanel = this.GetTemplateChild("seriesPanel") as ChartSeriesPanel;
            SeriesPanel.Series = this;
            if(this is StackingSeriesBase)
            Canvas.SetZIndex(SeriesPanel.Series, -ActualArea.GetSeriesIndex(this));
        }

        #endregion
    }
}
