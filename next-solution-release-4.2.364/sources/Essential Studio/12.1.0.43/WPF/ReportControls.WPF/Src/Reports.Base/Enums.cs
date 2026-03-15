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

namespace Syncfusion.RDL.Internal
{
    internal enum ModelType
    {
        TablixModel,
        GaugeModel,
        RectangleModel,
        ChartModel,
        TextBoxModel,
        LineModel,
        MapModel,
        ImageModel,
        SubReportModel
    }
    
    internal enum GaugeType
    {
        RadialGauge,
        LinearGauge
    }

    internal enum ReportItemViewMode
    {
        None,
        Normal,
        Print
    }

    #region SummaryType enums

    internal enum ReportingComputationType
    {
        /// <summary>
        /// Computes the sum of double or integer values.
        /// </summary>
        Sum,
        /// <summary>
        /// Computes the simple average of double or integer values.
        /// </summary>
        Avg,
        /// <summary>
        /// Computes the maximum of double or integer values.
        /// </summary>
        Max,
        /// <summary>
        /// Computes the minimum of double or integer values.
        /// </summary>
        Min,
        /// <summary>
        /// Computes the standard deviation of double or integer values.
        /// </summary>
        StDev,
        /// <summary>
        /// Computes the standard deviation population of double or integer values.
        /// </summary>
        StDevP,
        /// <summary>
        /// Computes the Population variance of double or integer values.
        /// </summary>
        VarP,
        /// <summary>
        /// Computes the variance of double or integer values.
        /// </summary>
        Var,
        /// <summary>
        /// Computes the count of double or integer values.
        /// </summary>
        Count,
        /// <summary>
        /// Computes the count unique of double or integer values.
        /// </summary>
        CountDistinct,
        /// <summary>
        /// Computes the FirstText.
        /// </summary>        
        First,
        /// <summary>
        /// Computes the FirstText.
        /// </summary>        
        Last,
        /// <summary>
        /// Computes the FirstText.
        /// </summary>        
        Data,
    }

    /// <summary>
    /// Enumerates the summary types availabe for use as calculations in the Tablix Grid.
    /// </summary>
    /// <remarks>
    /// If you use the value Custom in a ComputationInfo object, then you are required to explicitly set the ComputationInfo.Summary value.
    /// </remarks>
    internal enum ComputationType
    {
        /// <summary>
        /// Computes the sum of double or integer values.
        /// </summary>
        DoubleTotalSum,
        /// <summary>
        /// Computes the simple average of double or integer values.
        /// </summary>
        DoubleAverage,
        /// <summary>
        /// Computes the maximum of double or integer values.
        /// </summary>
        DoubleMaximum,
        /// <summary>
        /// Computes the minimum of double or integer values.
        /// </summary>
        DoubleMinimum,
        /// <summary>
        /// Computes the standard deviation of double or integer values.
        /// </summary>
        DoubleStandardDeviation,
        /// <summary>
        /// Computes the variance of double or integer values.
        /// </summary>
        DoubleVariance,
        /// <summary>
        /// Computes the count of double or integer values.
        /// </summary>
        Count,
        /// <summary>
        /// Computes the sum of decimal values.
        /// </summary>
        DecimalTotalSum,
        /// <summary>
        /// Computes the sum of integer values.
        /// </summary>
        IntTotalSum,
        /// <summary>
        /// Computes the Text.
        /// </summary>
        Text,
        /// <summary>
        /// Computes the Text.
        /// </summary>
        Data,
        /// <summary>
        /// Specifies that you are using a custom SummaryBase object to define the calculation.
        /// </summary>
        Custom

    }

    #endregion

    #region ChartEngineType

    internal enum ChartEngineType
    {
        Default,
        Shape
    }

    #endregion    
}
