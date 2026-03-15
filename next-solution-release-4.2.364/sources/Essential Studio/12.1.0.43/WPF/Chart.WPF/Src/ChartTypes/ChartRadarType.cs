// <copyright file="ChartRadarType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Represents ChartRadarType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartRadarType : ChartLineType
    {
        #region Public methods
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Indexed;
            }
        }

        /// <summary>
        /// Gets axes type that are required for chart to be built.
        /// </summary>
        public override ChartAxesType AxesType
        {
            get
            {
                return ChartAxesType.PolarAxes;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartRadarType"/>
        public override string ToString()
        {
            return "Radar";
        }
        #endregion

        #region Dependency property
        /// <summary>
        ///  Identifies the IsClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
       DependencyProperty.RegisterAttached("IsClosed", typeof(bool), typeof(ChartRadarType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        ///  Identifies the DrawType dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawTypeProperty =
        DependencyProperty.RegisterAttached("DrawType", typeof(ChartRadarDrawType), typeof(ChartRadarType), new PropertyMetadata(ChartRadarDrawType.Line, new PropertyChangedCallback(OnDataChanged)));

        //public static readonly DependencyProperty IsNetGridEnabledProperty =
       //DependencyProperty.RegisterAttached("IsNetGridEnabled", typeof(bool), typeof(ChartRadarType), new PropertyMetadata(true, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        ///  Identifies the Radarsymbol dependency property.
        /// </summary>
        public static readonly DependencyProperty RadarSymbolProperty =
      DependencyProperty.RegisterAttached("RadarSymbol", typeof(DataTemplate), typeof(ChartRadarType), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        #endregion

        #region Static method
        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="area">The Chartarea.</param>
        /// <returns>The IsClosed</returns>
        public static bool GetIsClosed(ChartArea area)
        {
            return (bool)area.GetValue(IsClosedProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetIsClosed(ChartArea area, bool value)
        {
            area.SetValue(IsClosedProperty, value);
        }

        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        /// <returns>The IsClosed</returns>
        public static DataTemplate GetRadarSymbol(ChartSeries series)
        {
            return (DataTemplate)series.GetValue(RadarSymbolProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        /// <param name="value">The value.</param>
        public static void SetRadarSymbol(ChartSeries series, DataTemplate value)
        {
            series.SetValue(RadarSymbolProperty, value);
        }

        
        //public static bool GetIsNetGridEnabled(ChartArea area)
        //{
        //    return (bool)area.GetValue(IsNetGridEnabledProperty);
        //}

        //public static void SetIsNetGridEnabled(ChartArea area, bool value)
        //{
        //    area.SetValue(IsNetGridEnabledProperty, value);
        //}
        /// <summary>
        /// Gets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The Drawtype</returns>
        public static ChartRadarDrawType GetDrawType(ChartArea area)
        {
            return (ChartRadarDrawType)area.GetValue(DrawTypeProperty);
        }

        /// <summary>
        /// Sets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetDrawType(ChartArea area, ChartRadarDrawType value)
        {
            area.SetValue(DrawTypeProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.m_visibleSeriesSegmentsRecountRequired = true;
                area.UpdateArea();
            }
        }
        #endregion
    }
}
