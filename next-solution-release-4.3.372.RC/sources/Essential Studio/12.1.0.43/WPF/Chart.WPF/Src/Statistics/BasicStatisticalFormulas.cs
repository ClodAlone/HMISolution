#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents class for BasicStatisticalFormulas
    /// </summary>
    public class BasicStatisticalFormulas
    {       
        #region Implementation
        /// <summary>
        /// Calculates mean value of series X values. 
        /// </summary>
        /// <param name="series">The name of the Series object that stores the first group's data for which an average is required.</param>
        /// <returns>Returns a double value that represents the average of all the data points in the given series. </returns>
        /// <example>
        /// <p>The following code demonstrate how to get the average of the data points in a series.
        /// </p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        /// double Mean1=BasicStatisticalFormulas.Mean(series1);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Mean1 As Double
        /// Mean1=BasicStatisticalFormulas.Mean(series1)
        /// </code>
        /// </example>
        /// <remarks>
        /// <p>Use this method to calculate the mean (i.e. average) of the points stored in a series.</p>
        ///	<p>If the specified input series does not exist in the SeriesCollection at the time of the method call than an exception will be thrown. </p>
        /// </remarks>
        public static double Mean(ChartSeries series)
        {
            double sum = 0;
            int len = series.Data.Count;
            for (int i = 0; i < len; i++)
                sum += series.Data[i].X;
            return sum / len;
        }

        /// <summary>
        /// Calculates mean value of series Y values.
        /// </summary>
        /// <param name="series">The name of the Series object that stores the first group's data for which an average is required.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// Returns a double value that represents the average of all the data points in the given series.
        /// </returns>
        /// <example>
        ///    <p>The following code demonstrate how to get the average of the data points in a series.
        /// </p>
        ///    <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Mean1=BasicStatisticalFormulas.Mean(series1, 0);
        /// </code>
        ///  <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Mean1 As Double
        /// Mean1=BasicStatisticalFormulas.Mean(series1, 0)
        /// </code>
        /// </example>
        /// <remarks>
        ///     <p>Use this method to calculate the mean (i.e. average) of the points stored in a series.</p>
        ///     <p>If the specified input series does not exist in the SeriesCollection at the time of the method call than an exception will be thrown. </p>
        /// </remarks>
        public static double Mean(ChartSeries series, int yIndex)
        {
            double sum = 0;
            int len = series.Data.Count;

            for (int i = 0; i < len; i++)
            {
                sum += series.Data[i].Values[yIndex];
            }

            return sum / len;
        }

        /// <summary>
        /// Calculates variance of series X values. 
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data. 
        /// </param>
        /// <returns>A double that represents the variance within the group of data.
        /// </returns>
        /// <example>
        /// <p>The following Code demonstrate how to gets the VarianceUnBasedEstimator of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double VarianceUnBased1= Statistics.BasicStatisticalFormulas.VarianceUnBiasedEstimator(series);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim VarianceUnBased1 As Double
        ///	VarianceUnBased1=BasicStatisticalFormulas.VarianceUnBiasedEstimator(series)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>This method estimates the variance for a sample.</p>
        ///	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double VarianceUnbiasedEstimator(ChartSeries series)
        {
            double sum = 0, mean = Mean(series);
            int len = series.Data.Count;
            for (int i = 0; i < len; i++)
            {
                double tt = (series.Data[i].X - mean);
                sum += tt * tt;
            }

            return sum / (len - 1);
        }

        /// <summary>
        /// Calculates variance of series Y values.
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// A double that represents the variance within the group of data.
        /// </returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the VarianceUnBasedEstimator of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double VarianceUnBased1= Statistics.BasicStatisticalFormulas.VarianceUnBiasedEstimator(series, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim VarianceUnBased1 As Double
        /// VarianceUnBased1=BasicStatisticalFormulas.VarianceUnBiasedEstimator(series, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method estimates the variance for a sample.</p>
        /// 	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double VarianceUnbiasedEstimator(ChartSeries series, int yIndex)
        {
            double sum = 0, mean = Mean(series, yIndex);
            int len = series.Data.Count;

            for (int i = 0; i < len; i++)
            {
                double tt = (series.Data[i].Values[yIndex] - mean);
                sum += tt * tt;
            }

            return sum / (len - 1);
        }

        /// <summary>
        /// Calculates variance of series X values. 
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data. 
        ///</param>
        /// <returns>A double that represents the variance within the group of data.
        ///</returns>
        /// /// <example>
        /// <p>The following Code demonstrate how to gets the VarianceBasedEstimator of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double VarianceBased1= Statistics.BasicStatisticalFormulas.VarianceBiasedEstimator(series);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim VarianceBased1 As Double
        ///	VarianceBased1=BasicStatisticalFormulas.VarianceBiasedEstimator(series)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>This method estimates the variance for a sample.</p>
        ///	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double VarianceBiasedEstimator(ChartSeries series)
        {
            double sum = 0, mean = Mean(series);
            int len = series.Data.Count;
            for (int i = 0; i < len; i++)
            {
                double tt = (series.Data[i].X - mean);
                sum += tt * tt;
            }

            return sum / len;
        }

        /// <summary>
        /// Calculates variance of series Y values.
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// A double that represents the variance within the group of data.
        /// </returns>
        /// /// 
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the VarianceBasedEstimator of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double VarianceBased1= Statistics.BasicStatisticalFormulas.VarianceBiasedEstimator(series, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim VarianceBased1 As Double
        /// VarianceBased1=BasicStatisticalFormulas.VarianceBiasedEstimator(series, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method estimates the variance for a sample.</p>
        /// 	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double VarianceBiasedEstimator(ChartSeries series, int yIndex)
        {
            double sum = 0, mean = Mean(series, yIndex);
            int len = series.Data.Count;
            for (int i = 0; i < len; i++)
            {
                double tt = (series.Data[i].Values[yIndex] - mean);
                sum += tt * tt;
            }

            return sum / len;
        }

        /// <summary>
        /// Calculates variance of series X values. 
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data. 
        ///</param>
        /// <param name="sampleVariance">True if the data is a sample of a population, false if it is the entire population. </param>
        /// <returns>A double that represents the variance within the group of data.
        ///</returns>
        /// <example>
        /// <p>The following Code demonstrate how to gets the Variance of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double Variance1= Statistics.BasicStatisticalFormulas.Variance(series,true);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Variance1 As Double
        ///	Variance1=BasicStatisticalFormulas.Variance(series,true)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>This method estimates the variance for a sample.</p>
        ///	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double Variance(ChartSeries series, bool sampleVariance)
        {
            if (sampleVariance) return VarianceUnbiasedEstimator(series);
            else return VarianceBiasedEstimator(series);
        }

        /// <summary>
        /// Calculates variance of series Y values.
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <param name="sampleVariance">True if the data is a sample of a population, false if it is the entire population.</param>
        /// <returns>
        /// A double that represents the variance within the group of data.
        /// </returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the Variance of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Variance1= Statistics.BasicStatisticalFormulas.Variance(series,true);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Variance1 As Double
        /// Variance1=BasicStatisticalFormulas.Variance(series,true)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method estimates the variance for a sample.</p>
        /// 	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double Variance(ChartSeries series, int yIndex, bool sampleVariance)
        {
            return sampleVariance ?
              VarianceUnbiasedEstimator(series, yIndex) : VarianceBiasedEstimator(series, yIndex);
        }

        /// <summary>
        /// Calculates variance of series X values. 
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data. 
        ///</param>
        /// <param name="sampleVariance">True if the data is a sample of a population, false if it is the entire population. </param>
        /// <returns>A double that represents the Standard Deviation within the group of data.
        /// </returns>
        /// <example>
        /// <p>The following Code demonstrate how to gets the Standard Deviation of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double Standard1= Statistics.BasicStatisticalFormulas.StandartDeviation(series,false);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Standard1 As Double
        ///	Standard1=BasicStatisticalFormulas.StandartDeviation(series,false)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>This method estimates the Standard Deviation for a sample.</p>
        ///	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double StandardDeviation(ChartSeries series, bool sampleVariance)
        {
            return Math.Sqrt(Variance(series, sampleVariance));
        }

        /// <summary>
        /// Calculates variance of series Y values.
        /// </summary>
        /// <param name="series">The name of the Series object that stores the group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <param name="sampleVariance">True if the data is a sample of a population, false if it is the entire population.</param>
        /// <returns>
        /// A double that represents the Standard Deviation within the group of data.
        /// </returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the Standard Deviation of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Standard1= Statistics.BasicStatisticalFormulas.StandardDeviation(series,0,false);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Standard1 As Double
        /// Standard1=BasicStatisticalFormulas.StandardDeviation(series,0,false)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method estimates the Standard Deviation for a sample.</p>
        /// 	<p>If the specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double StandardDeviation(ChartSeries series, int yIndex, bool sampleVariance)
        {
            return Math.Sqrt(Variance(series, yIndex, sampleVariance));
        }

        /// <summary>
        /// Calculates covariance of series X values. 
        /// </summary>
        /// <param name="series1">The name of the Series object that stores the first group's data. 
        ///</param>
        /// <param name="series2"> The name of the Series object that stores the second group's data.
        /// An exception will be raised if the input series do not have the same number of data points. </param>
        /// <returns>A double that represents the covariance value between the two groups of data.
        ///</returns>
        /// <example>
        /// <p>The following Code demonstrate how to gets the Covariance of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double Covariance1= Statistics.BasicStatisticalFormulas.Covariance(series1,series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Covariance1 As Double
        ///	Covariance1=BasicStatisticalFormulas.Covariance(series1,series2)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>This method returns the average of the product of deviations of the data points from their respective means.</p>
        /// <p>Covariance is a measure of the relationship between two ranges of data, and can be used to determine whether two ranges of data move together - that is, whether large values of one set are associated with large values of the other (positive covariance), whether small values of one set are associated with large values of the other (negative covariance), or whether values in both sets are unrelated (covariance near zero).</p>
        ///	<p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown. An exception will also be raised if the series do not have the same number of data points.</p>
        /// </remarks>
        public static double Covariance(ChartSeries series1, ChartSeries series2)
        {
            double mean1 = Mean(series1);
            double mean2 = Mean(series2);

            double sum = 0;
            int len1 = series1.Data.Count;
            int len2 = series2.Data.Count;

            if (len1 != len2)
                throw new InvalidOperationException("Series have different lengths.");

            for (int i = 0; i < len1; i++)
            {
                sum += (series1.Data[i].X - mean1) * (series2.Data[i].X - mean2);
            }

            return sum / len1;
        }

        /// <summary>
        /// Calculates covariance of series Y values.
        /// </summary>
        /// <param name="series1">The name of the Series object that stores the first group's data.</param>
        /// <param name="series2">The name of the Series object that stores the second group's data.
        /// An exception will be raised if the input series do not have the same number of data points.</param>
        /// <param name="yIndex">Index of the Y index.</param>
        /// <returns>
        /// A double that represents the covariance value between the two groups of data.
        /// </returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the Covariance of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Covariance1= Statistics.BasicStatisticalFormulas.Covariance(series1,series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Covariance1 As Double
        /// Covariance1=BasicStatisticalFormulas.Covariance(series1,series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method returns the average of the product of deviations of the data points from their respective means.</p>
        /// 	<p>Covariance is a measure of the relationship between two ranges of data, and can be used to determine whether two ranges of data move together - that is, whether large values of one set are associated with large values of the other (positive covariance), whether small values of one set are associated with large values of the other (negative covariance), or whether values in both sets are unrelated (covariance near zero).</p>
        /// 	<p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown. An exception will also be raised if the series do not have the same number of data points.</p>
        /// </remarks>
        public static double Covariance(ChartSeries series1, ChartSeries series2, int yIndex)
        {
            double mean1 = Mean(series1, yIndex);
            double mean2 = Mean(series2, yIndex);

            double sum = 0;
            int len1 = series1.Data.Count;
            int len2 = series2.Data.Count;

            if (len1 != len2)
                throw new InvalidOperationException("Series have different lengths.");

            for (int i = 0; i < len1; i++)
            {
                sum += (series1.Data[i].Values[yIndex] - mean1) * (series2.Data[i].Values[yIndex] - mean2);
            }

            return sum / len1;
        }

        /// <summary>
        /// Calculates correlation of series X values. 
        /// </summary>
        /// <param name="series1">The name of the Series object that stores the first group's data. 
        ///</param>
        /// <param name="series2"> The name of the Series object that stores the second group's data.
        /// An exception will be raised if the input series do not have the same number of data points. </param>
        /// <returns>A double that represents the Correlation value between the two groups of data.
        /// </returns>
        /// <example>
        /// <p>The following Code demonstrate how to gets the Correlation of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double Correlation1= Statistics.BasicStatisticalFormulas.Correlation(series1,series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Correlation1 As Double
        ///	Correlation1=BasicStatisticalFormulas.Correlation(series1,series2)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>Correlation measures the relationship between two data sets that are scaled to be independent of the unit of measurement. This correlation method returns the covariance of two data sets divided by the product of their standard deviations, and always ranges from -1 to 1.
        /// Use correlation to determine whether two ranges of data move together that is, whether large values of one set are associated with large values of the other (positive correlation), whether small values of one set are associated with large values of the other (negative correlation), or whether values in both sets are unrelated (correlation near zero).</p>
        /// <p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown. An exception will also be raised if the series do not have the same number of data points.</p>
        /// </remarks>
        public static double Correlation(ChartSeries series1, ChartSeries series2)
        {
            double mean1 = Mean(series1);
            double mean2 = Mean(series2);

            int len1 = series1.Data.Count;
            int len2 = series2.Data.Count;

            if (len1 != len2)
                throw new InvalidOperationException("Series have different lengths.");

            double sum = 0;
            double varSum1 = 0;
            double varSum2 = 0;
            for (int i = 0; i < len1; i++)
            {
                double t1 = (series1.Data[i].X - mean1);
                varSum1 += t1 * t1;
                double t2 = (series2.Data[i].X - mean2);
                varSum2 += t2 * t2;
                sum += t1 * t2;
            }

            return sum / Math.Sqrt(varSum1 * varSum2);
        }

        /// <summary>
        /// Calculates correlation of series Y values.
        /// </summary>
        /// <param name="series1">The name of the Series object that stores the first group's data.</param>
        /// <param name="series2">The name of the Series object that stores the second group's data.
        /// An exception will be raised if the input series do not have the same number of data points.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// A double that represents the Correlation value between the two groups of data.
        /// </returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the Correlation of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Correlation1= Statistics.BasicStatisticalFormulas.Correlation(series1,series2,0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Correlation1 As Double
        /// Correlation1=BasicStatisticalFormulas.Correlation(series1,series2,0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>Correlation measures the relationship between two data sets that are scaled to be independent of the unit of measurement. This correlation method returns the covariance of two data sets divided by the product of their standard deviations, and always ranges from -1 to 1.
        /// Use correlation to determine whether two ranges of data move together that is, whether large values of one set are associated with large values of the other (positive correlation), whether small values of one set are associated with large values of the other (negative correlation), or whether values in both sets are unrelated (correlation near zero).</p>
        /// 	<p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown. An exception will also be raised if the series do not have the same number of data points.</p>
        /// </remarks>
        public static double Correlation(ChartSeries series1, ChartSeries series2, int yIndex)
        {
            double mean1 = Mean(series1, yIndex);
            double mean2 = Mean(series2, yIndex);

            int len1 = series1.Data.Count;
            int len2 = series2.Data.Count;

            if (len1 != len2)
                throw new InvalidOperationException("Series have different lengths.");

            double sum = 0;
            double varSum1 = 0;
            double varSum2 = 0;
            for (int i = 0; i < len1; i++)
            {
                double t1 = (series1.Data[i].Values[yIndex] - mean1);
                varSum1 += t1 * t1;
                double t2 = (series2.Data[i].Values[yIndex] - mean2);
                varSum2 += t2 * t2;
                sum += t1 * t2;
            }

            return sum / Math.Sqrt(varSum1 * varSum2);
        }

        /// <summary>
        /// Calculates Median of series X values. 
        /// </summary>
        /// <param name="series">The input series</param>
        /// <example>
        /// <p>The following Code demonstrate how to gets the median of the data points in a series</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        ///	............
        ///	double Median1= Statistics.BasicStatisticalFormulas.Median(series1);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        ///	.............
        ///	Dim Median1 As Double
        ///	Median1=BasicStatisticalFormulas.Median(series1)
        ///	</code>
        /// </example>
        /// <remarks>
        /// <p>Use this method to calculate the median of the points stored in a series.
        ///	The median is the middle value of a sample set, where half of the members are greater in size and half the members are lesser in size.</p>
        ///<p>if the specified input series does not exist in the SeriesCollection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double Median(ChartSeries series)
        {
            int len = series.Data.Count;
            double median;

            if (len % 2 == 1)
            {
                median = series.Data[(len - 1) / 2].X;
            }
            else
            {
                double d1 = (series.Data[(len) / 2]).X;
                double d2 = (series.Data[(len) / 2 - 1]).X;
                median = (d1 + d2) / 2.0d;
            }

            return median;
        }

        /// <summary>
        /// Calculates Median of series Y values.
        /// </summary>
        /// <param name="series">The input series</param>
        /// <param name="yIndex">Index of the Y index.</param>
        /// <returns></returns>
        /// <example>
        /// 	<p>The following Code demonstrate how to gets the median of the data points in a series</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ............
        /// double Median1= Statistics.BasicStatisticalFormulas.Median(series1, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// .............
        /// Dim Median1 As Double
        /// Median1=BasicStatisticalFormulas.Median(series1, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>Use this method to calculate the median of the points stored in a series.
        /// The median is the middle value of a sample set, where half of the members are greater in size and half the members are lesser in size.</p>
        /// 	<p>if the specified input series does not exist in the SeriesCollection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static double Median(ChartSeries series, int yIndex)
        {
            int len = series.Data.Count;
            double median;

            if (len % 2 == 1)
            {
                median = series.Data[(len - 1) / 2].Values[yIndex];
            }
            else
            {
                double d1 = (series.Data[(len) / 2]).Values[yIndex];
                double d2 = (series.Data[(len) / 2 - 1]).Values[yIndex];
                median = (d1 + d2) / 2.0d;
            }

            return median;
        }
        #endregion

        /// <summary>
        /// Performs Anova (Analysis of variance test) on input series. All series should have the same 
        /// number of points. The tests null hypothesis assumes that all series means are equal and 
        /// that all variances of series are also equal. The alternative to null hypothesis is that there 
        /// is one inequality between means of series. For better understanding of this test, we recommend to read:
        /// Dowdy, S. M.
        /// Statistics for research / Shirley Dowdy, Stanley Weardon, Daniel Chilko.
        /// p. cm. – (Wiley series in probability and statistics; 1345)
        /// </summary>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="inputSeries">Series array</param>
        /// <returns> AnovaResult class </returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate AnovaTest</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// AnovaResult ar = BasicStatisticalFormulas.Anova(0.5,new ChartSeries[]{ series1, series2, series3} );
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ar As AnovaResult = BasicStatisticalFormulas.Anova(0.5, New ChartSeries(){ series1, series2, series3})
        /// </code> 
        /// </example> 
        /// <remarks>
        /// <p>An ANOVA test is used to test the difference between the means of two or more groups of data.</p>
        ///	<p>Two or more groups of data (series) must be specified, and each series must have the same number of data points otherwise an exception will be raised.</p>
        /// <p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static AnovaResult Anova(double probability, ChartSeries[] inputSeries)
        {
            int a = inputSeries.Length;
            Debug.Assert(inputSeries.Length > 0, " Series array is empty. ");
            int n = inputSeries[0].Data.Count;
            for (int i = 0; i < a; i++)
            {
                Debug.Assert(n == inputSeries[i].Data.Count, " Series lengths a different.");
                if (n != inputSeries[i].Data.Count)
                    throw new InvalidOperationException("Series have different lengths.");
            }

            AnovaResult ar = new AnovaResult();

            ar.degreeOfFreedomTotal = n * a - 1;
            ar.degreeOfFreedomWithinGroups = a * (n - 1);
            ar.deegreeOfFreedomBetweenGroups = a - 1;

            ChartListData withinGroupVariance = new ChartListData();
            for (int i = 0; i < a; i++)
                withinGroupVariance.Add(new ChartPoint(VarianceUnbiasedEstimator(inputSeries[i]), 0));
            ChartSeries withGroupVarianceSeries = new ChartSeries();
            withGroupVarianceSeries.Data = withinGroupVariance;

            ar.meanSquareVarianceWithinGroups = Mean(withGroupVarianceSeries);
            ar.sumOfSquaresWithinGroups = ar.DegreeOfFreedomWithinGroups * ar.meanSquareVarianceWithinGroups;

            ChartListData amongGroupVariance = new ChartListData();
            for (int i = 0; i < a; i++)
                amongGroupVariance.Add(new ChartPoint(Mean(inputSeries[i]), 0));
            ChartSeries amongGroupVarianceseries = new ChartSeries();
            amongGroupVarianceseries.Data = amongGroupVariance;

            ar.meanSquareVarianceBetweenGroups = n * VarianceUnbiasedEstimator(amongGroupVarianceseries);
            ar.sumOfSquaresBetweenGroups = (a - 1) * ar.meanSquareVarianceBetweenGroups;

            ar.sumOfSquaresTotal = ar.sumOfSquaresBetweenGroups + ar.sumOfSquaresWithinGroups;

            ar.fRatio = ar.meanSquareVarianceBetweenGroups / ar.meanSquareVarianceWithinGroups;

            ar.fCriticalValue = UtilityFunctions.InverseFCumulativeDistribution(probability, ar.deegreeOfFreedomBetweenGroups, ar.degreeOfFreedomWithinGroups);

            return ar;
        }

        /// <summary>
        /// Performs Anova (Analysis of variance test) on input series. All series should have the same
        /// number of points. The tests null hypothesis assumes that all series means are equal and
        /// that all variances of series are also equal. The alternative to null hypothesis is that there
        /// is one inequality between means of series. For better understanding of this test, we recommend to read:
        /// Dowdy, S. M.
        /// Statistics for research / Shirley Dowdy, Stanley Weardon, Daniel Chilko.
        /// p. cm. – (Wiley series in probability and statistics; 1345)
        /// </summary>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="inputSeries">Series array</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>AnovaResult class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate AnovaTest</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// AnovaResult ar = BasicStatisticalFormulas.Anova(0.5,new ChartSeries[]{ series1, series2, series3}, 0 );
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ar As AnovaResult = BasicStatisticalFormulas.Anova(0.5, New ChartSeries(){ series1, series2, series3}, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>An ANOVA test is used to test the difference between the means of two or more groups of data.</p>
        /// 	<p>Two or more groups of data (series) must be specified, and each series must have the same number of data points otherwise an exception will be raised.</p>
        /// 	<p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static AnovaResult Anova(double probability, ChartSeries[] inputSeries, int yIndex)
        {
            int a = inputSeries.Length;
            Debug.Assert(inputSeries.Length > 0, " Series array is empty. ");
            int n = inputSeries[0].Data.Count;
            for (int i = 0; i < a; i++)
            {
                Debug.Assert(n == inputSeries[i].Data.Count, " Series lengths a different.");
                if (n != inputSeries[i].Data.Count)
                    throw new InvalidOperationException("Series have different lengths.");
            }

            AnovaResult ar = new AnovaResult();

            ar.degreeOfFreedomTotal = n * a - 1;
            ar.degreeOfFreedomWithinGroups = a * (n - 1);
            ar.deegreeOfFreedomBetweenGroups = a - 1;

            ChartListData withinGroupVariance = new ChartListData();
            for (int i = 0; i < a; i++)
                withinGroupVariance.Add(new ChartPoint(VarianceUnbiasedEstimator(inputSeries[i], yIndex), 0));

            ChartSeries withinGroupVarianceseries = new ChartSeries();
            withinGroupVarianceseries.Data = withinGroupVariance;

            ar.meanSquareVarianceWithinGroups = Mean(withinGroupVarianceseries, yIndex);
            ar.sumOfSquaresWithinGroups = ar.DegreeOfFreedomWithinGroups * ar.meanSquareVarianceWithinGroups;

            ChartListData amongGroupVariance = new ChartListData();
            for (int i = 0; i < a; i++)
                amongGroupVariance.Add(new ChartPoint(Mean(inputSeries[i], yIndex), 0));

            ChartSeries amongGroupVarianceseries = new ChartSeries();
            amongGroupVarianceseries.Data = amongGroupVariance;
            ar.meanSquareVarianceBetweenGroups = n * VarianceUnbiasedEstimator(amongGroupVarianceseries, yIndex);
            ar.sumOfSquaresBetweenGroups = (a - 1) * ar.meanSquareVarianceBetweenGroups;

            ar.sumOfSquaresTotal = ar.sumOfSquaresBetweenGroups + ar.sumOfSquaresWithinGroups;

            ar.fRatio = ar.meanSquareVarianceBetweenGroups / ar.meanSquareVarianceWithinGroups;

            ar.fCriticalValue = UtilityFunctions.InverseFCumulativeDistribution(probability, ar.deegreeOfFreedomBetweenGroups, ar.degreeOfFreedomWithinGroups);

            return ar;
        }

        /// <summary>
        /// Performs F test on input series. This test looks whether first series variance is smaller than second series variance.
        /// If FValue in FTestResult is bigger than FCriticalValueOneTail than we can't deduce that it is truly smaller.
        /// The test tests ratio of two variances s1^2/s2^2 which is proved to be distributed 
        /// according F distribution. The null hypothesis is that variances are equal.
        /// </summary>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <returns> FTestResult class </returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate FTest.</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// FTestResult ftr = BasicStatisticalFormulas.FTest(0.05, series1, series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ftr As FTestResult = BasicStatisticalFormulas.FTest(0.05, series1, series2)
        /// </code> 
        /// </example>
        /// <remarks>
        /// <p>This method returns the results of the F-test using an FTestResult object.</p>
        /// <p>FTest performs a two-sample F-test to compare two population variances. For example, it can be used to determine whether the time scores in a swimming meet have a difference in variance for samples from two teams.</p>
        /// <p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static FTestResult FTest(double probability, ChartSeries firstInputSeries, ChartSeries secondInputSeries)
        {
            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in paired T test.  ");
            if (firstSerLen != secondSerLen)
                throw new InvalidOperationException("Series have different lengths.");

            FTestResult ftr = new FTestResult();

            ftr.firstSeriesMean = Mean(firstInputSeries);
            ftr.secondSeriesMean = Mean(secondInputSeries);
            ftr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries);
            ftr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries);

            ftr.fValue = ftr.firstSeriesVariance / ftr.secondSeriesVariance;

            ftr.fCriticalValueOneTail = UtilityFunctions.InverseFCumulativeDistribution(1 - probability, firstSerLen - 1, secondSerLen - 1);

            ftr.probabilityFOneTail = 1 - UtilityFunctions.FCumulativeDistribution(ftr.fValue, firstSerLen - 1, secondSerLen - 1);

            return ftr;
        }

        /// <summary>
        /// Performs F test on input series. This test looks whether first series variance is smaller than second series variance.
        /// If FValue in FTestResult is bigger than FCriticalValueOneTail than we can't deduce that it is truly smaller.
        /// The test tests ratio of two variances s1^2/s2^2 which is proved to be distributed
        /// according F distribution. The null hypothesis is that variances are equal.
        /// </summary>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <param name="yIndex">Index of the y.</param>
        /// <returns>FTestResult class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate FTest.</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// FTestResult ftr = BasicStatisticalFormulas.FTest(0.05, series1, series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ftr As FTestResult = BasicStatisticalFormulas.FTest(0.05, series1, series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method returns the results of the F-test using an FTestResult object.</p>
        /// 	<p>FTest performs a two-sample F-test to compare two population variances. For example, it can be used to determine whether the time scores in a swimming meet have a difference in variance for samples from two teams.</p>
        /// 	<p>If a specified input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static FTestResult FTest(double probability,
          ChartSeries firstInputSeries, ChartSeries secondInputSeries, int yIndex)
        {
            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in paired T test.  ");
            if (firstSerLen != secondSerLen)
                throw new InvalidOperationException("Series have different lengths.");

            FTestResult ftr = new FTestResult();

            ftr.firstSeriesMean = Mean(firstInputSeries, yIndex);
            ftr.secondSeriesMean = Mean(secondInputSeries, yIndex);
            ftr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries, yIndex);
            ftr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries, yIndex);

            ftr.fValue = ftr.firstSeriesVariance / ftr.secondSeriesVariance;

            ftr.fCriticalValueOneTail = UtilityFunctions.InverseFCumulativeDistribution(1 - probability, firstSerLen - 1, secondSerLen - 1);

            ftr.probabilityFOneTail = 1 - UtilityFunctions.FCumulativeDistribution(ftr.fValue, firstSerLen - 1, secondSerLen - 1);

            return ftr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that 
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be unequal. So this method is not statistically exact,
        /// but it works well, and sometimes is called robust T test.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <returns> TTestResult class </returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate TTestPaired</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestPaired(0.2, 0.05, series1, series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestPaired (0.2, 0.05, series1, series2)
        /// </code> 
        /// </example>
        /// <remarks>
        /// <p>This method performs a paired two-sample student's t-test to determine whether a sample's means are distinct. This form of the t-test does not assume that the variances of both populations are equal.</p>
        ///	<p>Use a paired test when there is a natural pairing of observations in the samples, such as a sample group that is tested twice (e.g. before and after an experiment).</p>
        ///	<p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestPaired(double hypothesizedMeanDifference, double probability, ChartSeries firstInputSeries, ChartSeries secondInputSeries)
        {
            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in paired T test.  ");
            if (firstSerLen != secondSerLen)
                throw new InvalidOperationException("Series have different lengths.");

            ChartSeries difSeries = DifferenceX(firstInputSeries, secondInputSeries);

            TTestResult ttr = new TTestResult();

            double difSeriesMean = Mean(difSeries);
            ttr.firstSeriesMean = Mean(firstInputSeries);
            ttr.secondSeriesMean = Mean(secondInputSeries);
            double difSeriesVariance = VarianceUnbiasedEstimator(difSeries);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries);

            double variance = difSeriesVariance / firstSerLen;
            double varianceSQRT = Math.Sqrt(variance);

            ttr.degreeOfFreedom = firstSerLen - 1;

            ttr.tValue = (difSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be unequal. So this method is not statistically exact,
        /// but it works well, and sometimes is called robust T test.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>TTestResult class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate TTestPaired</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestPaired(0.2, 0.05, series1, series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestPaired (0.2, 0.05, series1, series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method performs a paired two-sample student's t-test to determine whether a sample's means are distinct. This form of the t-test does not assume that the variances of both populations are equal.</p>
        /// 	<p>Use a paired test when there is a natural pairing of observations in the samples, such as a sample group that is tested twice (e.g. before and after an experiment).</p>
        /// 	<p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestPaired(double hypothesizedMeanDifference, double probability,
          ChartSeries firstInputSeries, ChartSeries secondInputSeries, int yIndex)
        {
            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in paired T test.  ");
            if (firstSerLen != secondSerLen)
                throw new InvalidOperationException("Series have different lengths.");

            ChartSeries difSeries = DifferenceY(firstInputSeries, secondInputSeries, yIndex);

            TTestResult ttr = new TTestResult();

            double difSeriesMean = Mean(difSeries, yIndex);
            ttr.firstSeriesMean = Mean(firstInputSeries, yIndex);
            ttr.secondSeriesMean = Mean(secondInputSeries, yIndex);
            double difSeriesVariance = VarianceUnbiasedEstimator(difSeries, yIndex);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries, yIndex);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries, yIndex);

            double variance = difSeriesVariance / firstSerLen;
            double varianceSQRT = Math.Sqrt(variance);

            ttr.degreeOfFreedom = firstSerLen - 1;

            ttr.tValue = (difSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that 
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be equal. This is a key feature of the test, because 
        /// there is no exact T test for two samples from populations with different variances.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <returns> TTestResult class </returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate TTest Equal Variance</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestEqualVariances (0.2, 0.05, series1, series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestEqualVariances (0.2, 0.05, series1, series2)
        /// </code> 
        /// </example>
        /// <remarks>
        /// <p>This method performs a T test for two groups of data, and assumes equal variances between the two groups (i.e. series).</p>
        /// <p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestEqualVariances(double hypothesizedMeanDifference, double probability, ChartSeries firstInputSeries, ChartSeries secondInputSeries)
        {
            TTestResult ttr = new TTestResult();

            ttr.degreeOfFreedom = firstInputSeries.Data.Count + secondInputSeries.Data.Count - 2;

            ttr.firstSeriesMean = Mean(firstInputSeries);
            ttr.secondSeriesMean = Mean(secondInputSeries);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries);

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;
            double pooledVariance = ((firstSerLen - 1) * ttr.firstSeriesVariance + (secondSerLen - 1) * ttr.secondSeriesVariance) / (ttr.degreeOfFreedom);
            double varianceSQRT = Math.Sqrt(pooledVariance / firstSerLen + pooledVariance / secondSerLen);

            ttr.tValue = (ttr.firstSeriesMean - ttr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be equal. This is a key feature of the test, because
        /// there is no exact T test for two samples from populations with different variances.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <param name="yIndex">Index of the Y index.</param>
        /// <returns>TTestResult class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate TTest Equal Variance</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestEqualVariances (0.2, 0.05, series1, series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestEqualVariances (0.2, 0.05, series1, series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method performs a T test for two groups of data, and assumes equal variances between the two groups (i.e. series).</p>
        /// 	<p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestEqualVariances(double hypothesizedMeanDifference, double probability,
          ChartSeries firstInputSeries, ChartSeries secondInputSeries, int yIndex)
        {
            TTestResult ttr = new TTestResult();

            ttr.degreeOfFreedom = firstInputSeries.Data.Count + secondInputSeries.Data.Count - 2;

            ttr.firstSeriesMean = Mean(firstInputSeries, yIndex);
            ttr.secondSeriesMean = Mean(secondInputSeries, yIndex);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries, yIndex);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries, yIndex);

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;
            double pooledVariance = ((firstSerLen - 1) * ttr.firstSeriesVariance + (secondSerLen - 1) * ttr.secondSeriesVariance) / (ttr.degreeOfFreedom);
            double varianceSQRT = Math.Sqrt(pooledVariance / firstSerLen + pooledVariance / secondSerLen);

            ttr.tValue = (ttr.firstSeriesMean - ttr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that 
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be unequal. So this method is not statistically exact,
        /// but it works well, and sometimes is called robust T test.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <returns> TTestResult class </returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate TTest UnEqual Variance</p>
        /// <code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestUnEqualVariances (0.2, 0.05, series1, series2);
        /// </code>
        /// <code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestUnEqualVariances (0.2, 0.05, series1, series2)
        /// </code> 
        /// </example>
        /// <remarks>
        /// <p>This method performs a T test for two groups of data, and assumes unequal variances between the two groups (i.e. series).</p>
        /// <p>This analysis tool is referred to as a heteroscedastic t-test, and can be used when the groups under study are distinct. Use a paired test when there is one group before and after a treatment.</p>
        /// <p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestUnEqualVariances(double hypothesizedMeanDifference, double probability, ChartSeries firstInputSeries, ChartSeries secondInputSeries)
        {
            TTestResult ttr = new TTestResult();

            ttr.firstSeriesMean = Mean(firstInputSeries);
            ttr.secondSeriesMean = Mean(secondInputSeries);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries);

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            double firstMeanVariance = ttr.firstSeriesVariance / firstSerLen;
            double secondMeanVariance = ttr.secondSeriesVariance / secondSerLen;
            double variance = (firstMeanVariance + secondMeanVariance);
            double varianceSQRT = Math.Sqrt(variance);

            ttr.degreeOfFreedom = variance * variance / (firstMeanVariance * firstMeanVariance / (firstSerLen - 1) + secondMeanVariance * secondMeanVariance / (secondSerLen - 1));

            ttr.tValue = (ttr.firstSeriesMean - ttr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }

        /// <summary>
        /// Performs T test on input series. This test assumes that
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations.
        /// The population variances are assumed to be unequal. So this method is not statistically exact,
        /// but it works well, and sometimes is called robust T test.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data.</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>TTestResult class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate TTest UnEqual Variance</p>
        /// 	<code lang="C#">
        /// using Syncfusion.Windows.Forms.Chart.Statistics;
        /// ........
        /// TTestResult ttr = BasicStatisticalFormulas.TTestUnEqualVariances (0.2, 0.05, series1, series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Imports Syncfusion.Windows.Forms.Chart.Statistics
        /// ........
        /// Dim ttr As TTestResult = BasicStatisticalFormulas.TTestUnEqualVariances (0.2, 0.05, series1, series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method performs a T test for two groups of data, and assumes unequal variances between the two groups (i.e. series).</p>
        /// 	<p>This analysis tool is referred to as a heteroscedastic t-test, and can be used when the groups under study are distinct. Use a paired test when there is one group before and after a treatment.</p>
        /// 	<p>If either input series does not exist in the series collection at the time of the method call an exception will be thrown.</p>
        /// </remarks>
        public static TTestResult TTestUnEqualVariances(double hypothesizedMeanDifference, double probability,
          ChartSeries firstInputSeries, ChartSeries secondInputSeries, int yIndex)
        {
            TTestResult ttr = new TTestResult();

            ttr.firstSeriesMean = Mean(firstInputSeries, yIndex);
            ttr.secondSeriesMean = Mean(secondInputSeries, yIndex);
            ttr.firstSeriesVariance = VarianceUnbiasedEstimator(firstInputSeries, yIndex);
            ttr.secondSeriesVariance = VarianceUnbiasedEstimator(secondInputSeries, yIndex);

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;

            double firstMeanVariance = ttr.firstSeriesVariance / firstSerLen;
            double secondMeanVariance = ttr.secondSeriesVariance / secondSerLen;
            double variance = (firstMeanVariance + secondMeanVariance);
            double varianceSQRT = Math.Sqrt(variance);

            ttr.degreeOfFreedom = variance * variance / (firstMeanVariance * firstMeanVariance / (firstSerLen - 1) + secondMeanVariance * secondMeanVariance / (secondSerLen - 1));

            ttr.tValue = (ttr.firstSeriesMean - ttr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ttr.tCriticalValueOneTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability, probability), ttr.degreeOfFreedom, true);
            ttr.tCriticalValueTwoTail = UtilityFunctions.InverseTCumulativeDistribution(Math.Max(1 - probability / 2, probability / 2), ttr.degreeOfFreedom, true);

            ttr.probabilityTOneTail = UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);
            ttr.probabilityTTwoTail = 2 * UtilityFunctions.TCumulativeDistribution(-Math.Abs(ttr.tValue), ttr.degreeOfFreedom, true);

            return ttr;
        }


        /// <summary>
        /// Performs Z test on input series. This test assumes that 
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations 
        /// with known variances.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="varianceFirstGroup">Variance of the first series population.</param>
        /// <param name="varianceSecondGroup">Variance of the second series population.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data..</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data..</param>
        /// <returns>ZTestResult Class</returns>
        /// <example>
        /// <p>The following code demonstrate how to calculate Ztest</p>
        /// <code lang="C#">
        /// ZTestResult ztr = BasicStatisticalFormulas.ZTest( Convert.ToDouble(TextBox6.Text.ToString()),sqrtVarianceOfFirstSeries*sqrtVarianceOfFirstSeries,sqrtVarianceOfSecondSeries* sqrtVarianceOfSecondSeries,0.05,series1,series2);
        /// </code>
        /// <code lang="VB">
        /// Dim ztr As ZTestResult = BasicStatisticalFormulas.ZTest(Convert.ToDouble(TextBox6.Text.ToString()), sqrtVarianceOfFirstSeries*sqrtVarianceOfFirstSeries, sqrtVarianceOfSecondSeries*sqrtVarianceOfSecondSeries, 0.05, series1, series2)
        /// </code> 
        /// </example>
        /// <remarks>
        /// <p>This method performs a Z test for two groups of data, and returns the results using a ZTestResult object.</p>
        /// <p>Two and only two groups of data must be specified. If either input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static ZTestResult ZTest(double hypothesizedMeanDifference, double varianceFirstGroup, double varianceSecondGroup, double probability, ChartSeries firstInputSeries, ChartSeries secondInputSeries)
        {
            ZTestResult ztr = new ZTestResult();

            ztr.firstSeriesMean = Mean(firstInputSeries);
            ztr.secondSeriesMean = Mean(secondInputSeries);
            ztr.firstSeriesVariance = varianceFirstGroup;
            ztr.secondSeriesVariance = varianceSecondGroup;

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;
            double varianceSQRT = Math.Sqrt(varianceFirstGroup / firstSerLen + varianceSecondGroup / secondSerLen);

            ztr.zValue = (ztr.firstSeriesMean - ztr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ztr.zCriticalValueOneTail = UtilityFunctions.InverseNormalDistribution(Math.Max(1 - probability, probability));
            ztr.zCriticalValueTwoTail = UtilityFunctions.InverseNormalDistribution(Math.Max(1 - probability / 2, probability / 2));

            ztr.probabilityZOneTail = UtilityFunctions.NormalDistribution(-Math.Abs(ztr.zValue));
            ztr.probabilityZTwoTail = 2 * UtilityFunctions.NormalDistribution(-Math.Abs(ztr.zValue));

            return ztr;
        }

        /// <summary>
        /// Performs Z test on input series. This test assumes that
        /// there is some difference between mean values of input series populations.
        /// Input series are regarded as samples from normally distributed populations
        /// with known variances.
        /// </summary>
        /// <param name="hypothesizedMeanDifference">Difference between populations means.</param>
        /// <param name="varianceFirstGroup">Variance of the first series population.</param>
        /// <param name="varianceSecondGroup">Variance of the second series population.</param>
        /// <param name="probability">Probability that gives confidence level. (Typically 0.05)</param>
        /// <param name="firstInputSeries">The name of the series that stores the first group of data..</param>
        /// <param name="secondInputSeries">The name of the series that stores the second group of data..</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>ZTestResult Class</returns>
        /// <example>
        /// 	<p>The following code demonstrate how to calculate Ztest</p>
        /// 	<code lang="C#">
        /// ZTestResult ztr = BasicStatisticalFormulas.ZTest( Convert.ToDouble(TextBox6.Text.ToString()),sqrtVarianceOfFirstSeries*sqrtVarianceOfFirstSeries,sqrtVarianceOfSecondSeries* sqrtVarianceOfSecondSeries,0.05,series1,series2, 0);
        /// </code>
        /// 	<code lang="VB">
        /// Dim ztr As ZTestResult = BasicStatisticalFormulas.ZTest(Convert.ToDouble(TextBox6.Text.ToString()), sqrtVarianceOfFirstSeries*sqrtVarianceOfFirstSeries, sqrtVarianceOfSecondSeries*sqrtVarianceOfSecondSeries, 0.05, series1, series2, 0)
        /// </code>
        /// </example>
        /// <remarks>
        /// 	<p>This method performs a Z test for two groups of data, and returns the results using a ZTestResult object.</p>
        /// 	<p>Two and only two groups of data must be specified. If either input series does not exist in the series collection at the time of the method call than an exception will be thrown.</p>
        /// </remarks>
        public static ZTestResult ZTest(double hypothesizedMeanDifference, double varianceFirstGroup,
          double varianceSecondGroup, double probability,
          ChartSeries firstInputSeries, ChartSeries secondInputSeries, int yIndex)
        {
            ZTestResult ztr = new ZTestResult();

            ztr.firstSeriesMean = Mean(firstInputSeries, yIndex);
            ztr.secondSeriesMean = Mean(secondInputSeries, yIndex);
            ztr.firstSeriesVariance = varianceFirstGroup;
            ztr.secondSeriesVariance = varianceSecondGroup;

            int firstSerLen = firstInputSeries.Data.Count;
            int secondSerLen = secondInputSeries.Data.Count;
            double varianceSQRT = Math.Sqrt(varianceFirstGroup / firstSerLen + varianceSecondGroup / secondSerLen);

            ztr.zValue = (ztr.firstSeriesMean - ztr.secondSeriesMean - hypothesizedMeanDifference) / varianceSQRT;

            ztr.zCriticalValueOneTail = UtilityFunctions.InverseNormalDistribution(Math.Max(1 - probability, probability));
            ztr.zCriticalValueTwoTail = UtilityFunctions.InverseNormalDistribution(Math.Max(1 - probability / 2, probability / 2));

            ztr.probabilityZOneTail = UtilityFunctions.NormalDistribution(-Math.Abs(ztr.zValue));
            ztr.probabilityZTwoTail = 2 * UtilityFunctions.NormalDistribution(-Math.Abs(ztr.zValue));

            return ztr;
        }



        #region Helper methdos
        /// <summary>
        /// Calculates new series by substracting corresponding values of second series from firs series. 
        /// </summary>
        /// <param name="series1">The name of the series that stores the first group of data.</param>
        /// <param name="series2">The name of the series that stores the second group of data.</param>
        /// <returns>Return difference between the two series points</returns>
        private static ChartSeries DifferenceX(ChartSeries series1, ChartSeries series2)
        {
            int firstSerLen = series1.Data.Count;
            int secondSerLen = series2.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in DifferenceX. ");

            ChartSeries cs = new ChartSeries();
            ChartListData data = new ChartListData();
            for (int i = 0; i < firstSerLen; i++)
                data.Add(new ChartPoint(series1.Data[i].X - series2.Data[i].X, 0));
            cs.Data = data;
            return cs;
        }

        /// <summary>
        /// Calculates new series by substracting corresponding values of second series from firs series.
        /// </summary>
        /// <param name="series1">The name of the series that stores the first group of data.</param>
        /// <param name="series2">The name of the series that stores the second group of data.</param>
        /// <param name="yIndex">Index of the Y value.</param>
        /// <returns>
        /// Return difference between the two series points
        /// </returns>
        private static ChartSeries DifferenceY(ChartSeries series1, ChartSeries series2, int yIndex)
        {
            int firstSerLen = series1.Data.Count;
            int secondSerLen = series2.Data.Count;

            Debug.Assert(firstSerLen == secondSerLen, " Different sample sizes in DifferenceX. ");

            ChartSeries cs = new ChartSeries();
            ChartListData data = new ChartListData();
            for (int i = 0; i < firstSerLen; i++)
                data.Add(new ChartPoint(series1.Data[i].Values[yIndex] - series2.Data[i].Values[yIndex], 0));

            cs.DataSource = data;
            cs.BindingPathX = "X";
            cs.BindingPathsY = new string[] { "Y" };
            return cs;
        }
        #endregion
    }
}
