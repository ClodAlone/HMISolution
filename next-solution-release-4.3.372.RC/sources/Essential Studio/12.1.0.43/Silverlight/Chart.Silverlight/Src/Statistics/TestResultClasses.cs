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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// The result of statistical Z test is stored in this class.
    /// If the Z value is closer to 0.0 than ZCriticalValueTwoTail or 
    /// even ZCriticalValueOneTail, then we can't deduce that D(hypothesized difference) is not 
    /// good mean value difference. In other case ( ZCriticalValueTwoTail is closer to 0.0
    /// than ZValue), there is a huge probability that  hypothesized difference D hadn't been 
    /// chosen correctly.
    /// </summary>
    public class ZTestResult
    {
        #region MEMBERS
        internal double firstSeriesMean;
        internal double firstSeriesVariance;
        internal double probabilityZOneTail;
        internal double probabilityZTwoTail;
        internal double secondSeriesMean;
        internal double secondSeriesVariance;
        internal double zCriticalValueOneTail;
        internal double zCriticalValueTwoTail;
        internal double zValue;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="ZTestResult"/> class.
        /// </summary>
        public ZTestResult()
        {
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets first series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series mean.</value>
        public double FirstSeriesMean
        {
            get { return firstSeriesMean; }
        }

        /// <summary>
        /// Gets first series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series variance.</value>
        public double FirstSeriesVariance
        {
            get { return firstSeriesVariance; }
        }

        /// <summary>
        /// Gets the probability that the random variable has values at the tail, assuming that null hypothesis is true.
        /// </summary>
        /// <value>The probability Z one tail.</value>
        public double ProbabilityZOneTail
        {
            get { return probabilityZOneTail; }
        }

        /// <summary>
        /// Gets the probability that the random variable has values at the tails, assuming that null hypothesis is true.
        /// </summary>
        /// <value>The probability Z two tail.</value>
        public double ProbabilityZTwoTail
        {
            get { return probabilityZTwoTail; }
        }

        /// <summary>
        /// Gets second series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series mean.</value>
        public double SecondSeriesMean
        {
            get { return secondSeriesMean; }
        }

        /// <summary>
        /// Gets second series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series variance.</value>
        public double SecondSeriesVariance
        {
            get { return secondSeriesVariance; }
        }

        /// <summary>
        /// Gets critical value of Z which corresponds to Alpha probability.
        /// The area under normal probability density curve of tail is equal to alpha probability.
        /// </summary>
        /// <value>The Z critical value one tail.</value>
        public double ZCriticalValueOneTail
        {
            get { return zCriticalValueOneTail; }
        }

        /// <summary>
        /// Gets critical value of Z which corresponds to Alpha probability.
        /// The area under normal probability density curve of two symmetrical tails is equal to alpha probability.
        /// </summary>
        /// <value>The Z critical value two tail.</value>
        public double ZCriticalValueTwoTail
        {
            get { return zCriticalValueTwoTail; }
        }

        /// <summary>
        /// Gets calculated z value. ( Value of normally distributed random variable with mean=0, and variance=1 ).
        /// </summary>
        /// <value>The Z value.</value>
        public double ZValue
        {
            get { return zValue; }
        }
        #endregion
    }

    /// <summary>
    /// The result of statistical T test is stored in this class.
    /// If the T value is closer to 0.0 than TCriticalValueTwoTail or 
    /// even TCriticalValueOneTail, then we can't deduce that D(hypothesized difference) is not 
    /// good mean value difference. In other case ( TCriticalValueTwoTail is closer to 0.0 
    /// than TValue), there is a huge probability that  hypothesized difference D hadn't been 
    /// chosen correctly.
    /// </summary>
    public class TTestResult
    {
        #region MEMBERS
        internal double degreeOfFreedom;
        internal double firstSeriesMean;
        internal double firstSeriesVariance;
        internal double probabilityTOneTail;
        internal double probabilityTTwoTail;
        internal double secondSeriesMean;
        internal double secondSeriesVariance;
        internal double tCriticalValueOneTail;
        internal double tCriticalValueTwoTail;
        internal double tValue;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="TTestResult"/> class.
        /// </summary>
        public TTestResult()
        {
        }
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets number of degrees of freedom of T variable student's distribution.
        /// </summary>
        /// <value>The degree of freedom.</value>
        public double DegreeOfFreedom
        {
            get { return degreeOfFreedom; }
        }

        /// <summary>
        /// Gets first series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series mean.</value>
        public double FirstSeriesMean
        {
            get { return firstSeriesMean; }
        }

        /// <summary>
        /// Gets first series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series variance.</value>
        public double FirstSeriesVariance
        {
            get { return firstSeriesVariance; }
        }

        /// <summary>
        /// Gets the probability that the random variable has values at the tail, assuming that null hypothesis is true.
        /// </summary>
        /// <value>The probability T one tail.</value>
        public double ProbabilityTOneTail
        {
            get { return probabilityTOneTail; }
        }

        /// <summary>
        /// Gets the probability that the random variable has values at the tails, assuming that null hypothesis is true.
        /// </summary>
        /// <value>The probability T two tail.</value>
        public double ProbabilityTTwoTail
        {
            get { return probabilityTTwoTail; }
        }

        /// <summary>
        /// Gets second series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series mean.</value>
        public double SecondSeriesMean
        {
            get { return secondSeriesMean; }
        }

        /// <summary>
        /// Gets second series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series variance.</value>
        public double SecondSeriesVariance
        {
            get { return secondSeriesVariance; }
        }

        /// <summary>
        /// Gets critical value of T which corresponds to Alpha probability.
        /// The area under normal probability density curve of tail is equal to alpha probability.
        /// </summary>
        /// <value>The T critical value one tail.</value>
        public double TCriticalValueOneTail
        {
            get { return tCriticalValueOneTail; }
        }

        /// <summary>
        /// Gets critical value of T which corresponds to Alpha probability.
        /// The area under normal probability density curve of two symmetrical tails is equal to alpha probability.
        /// </summary>
        /// <value>The T critical value two tail.</value>
        public double TCriticalValueTwoTail
        {
            get { return tCriticalValueTwoTail; }
        }

        /// <summary>
        /// Gets calculated T value. ( Value of normally distributed random variable with mean=0, and variance=1 ).
        /// </summary>
        /// <value>The T value.</value>
        public double TValue
        {
            get { return tValue; }
        }
        #endregion
    }

    /// <summary>
    /// The result of statistical F test is stored in this class.
    /// If the F value is closer to 1.0 than FCriticalValueOneTail, then we can't deduce that first variance 
    /// is smaller than second. But if F value is bigger than 1.0, then replace the series and run the 
    /// test again. Maybe second series variance is smaller than first.
    /// Note: That if the second test also fails, this doesn't automatically prove that your variances
    /// are equal.
    /// </summary>
    public class FTestResult
    {
        #region MEMBERS
        internal double firstSeriesMean;
        internal double firstSeriesVariance;
        internal double probabilityFOneTail;
        internal double secondSeriesMean;
        internal double secondSeriesVariance;
        internal double fCriticalValueOneTail;
        internal double fValue;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="FTestResult"/> class.
        /// </summary>
        public FTestResult()
        {
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets first series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series mean.</value>
        public double FirstSeriesMean
        {
            get { return firstSeriesMean; }
        }

        /// <summary>
        /// Gets first series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The first series variance.</value>
        public double FirstSeriesVariance
        {
            get { return firstSeriesVariance; }
        }

        /// <summary>
        /// Gets the probability that the random variable has values at the tail, assuming that null hypothesis is true.
        /// </summary>
        /// <value>The probability F one tail.</value>
        public double ProbabilityFOneTail
        {
            get { return probabilityFOneTail; }
        }

        /// <summary>
        /// Gets second series mean value. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series mean.</value>
        public double SecondSeriesMean
        {
            get { return secondSeriesMean; }
        }

        /// <summary>
        /// Gets second series variance. Series represents sample from studied population.
        /// </summary>
        /// <value>The second series variance.</value>
        public double SecondSeriesVariance
        {
            get { return secondSeriesVariance; }
        }

        /// <summary>
        /// Gets critical value of F which corresponds to Alpha probability.
        /// The area under normal probability density curve of tail is equal to alpha probability.
        /// </summary>
        /// <value>The F critical value one tail.</value>
        public double FCriticalValueOneTail
        {
            get { return fCriticalValueOneTail; }
        }

        /// <summary>
        /// Gets calculated F value. ( Value of normally distributed random variable with mean=0, and variance=1 ).
        /// </summary>
        /// <value>The F value.</value>
        public double FValue
        {
            get { return fValue; }
        }

        #endregion
    }

    /// <summary>
    /// Result of Anova test is stored in this class.
    /// If AnovaResult.FRatio is farther from unity than FCritical value, then the null hypothesis
    /// (that all means are equal) fails.
    /// </summary>
    public class AnovaResult
    {
        #region MEMEBERS
        internal double deegreeOfFreedomBetweenGroups;
        internal double degreeOfFreedomTotal;
        internal double degreeOfFreedomWithinGroups;
        internal double fCriticalValue;
        internal double fRatio;
        internal double meanSquareVarianceBetweenGroups;
        internal double meanSquareVarianceWithinGroups;
        internal double sumOfSquaresBetweenGroups;
        internal double sumOfSquaresTotal;
        internal double sumOfSquaresWithinGroups;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="AnovaResult"/> class.
        /// </summary>
        public AnovaResult()
        {
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets degrees of freedom between groups. This is simply a - 1, where a is number of series in anova test.
        /// </summary>
        /// <value>The degree of freedom between groups.</value>
        public double DegreeOfFreedomBetweenGroups
        {
            get { return deegreeOfFreedomBetweenGroups; }
        }

        /// <summary>
        /// Gets total degrees of freedom. This is simply n*a - 1, where a is number of series in anova test, and n is number of points in series.
        /// </summary>
        /// <value>The degree of freedom total.</value>
        public double DegreeOfFreedomTotal
        {
            get { return degreeOfFreedomTotal; }
        }

        /// <summary>
        /// Gets degrees of freedom within groups ( returns a*(n - 1) ).
        /// </summary>
        /// <value>The degree of freedom within groups.</value>
        public double DegreeOfFreedomWithinGroups
        {
            get { return degreeOfFreedomWithinGroups; }
        }

        /// <summary>
        /// Gets critical value of FRatio which corresponds to specified confidence probability.
        /// </summary>
        /// <value>The F critical value.</value>
        public double FCriticalValue
        {
            get { return fCriticalValue; }
        }

        /// <summary>
        /// Gets FRatio ( ratio of between group variance and within group variance). This ratio should be
        /// compared with FCritical value, and if it is farther from unity than FCritical value, then the null hypothesis
        /// (that all means are equal) fails.
        /// </summary>
        /// <value>The F ratio.</value>
        public double FRatio
        {
            get { return fRatio; }
        }

        /// <summary>
        /// Gets mean square variance between groups.
        /// </summary>
        /// <value>The mean square variance between groups.</value>
        public double MeanSquareVarianceBetweenGroups
        {
            get { return meanSquareVarianceBetweenGroups; }
        }

        /// <summary>
        /// Gets mean square variance within groups.
        /// </summary>
        /// <value>The mean square variance within groups.</value>
        public double MeanSquareVarianceWithinGroups
        {
            get { return meanSquareVarianceWithinGroups; }
        }

        /// <summary>
        /// Gets sum of squares between groups.
        /// </summary>
        /// <value>The sum of squares between groups.</value>
        public double SumOfSquaresBetweenGroups
        {
            get { return sumOfSquaresBetweenGroups; }
        }

        /// <summary>
        /// Gets total sum of squares.
        /// </summary>
        /// <value>The sum of squares total.</value>
        public double SumOfSquaresTotal
        {
            get { return sumOfSquaresTotal; }
        }

        /// <summary>
        /// Gets sum of squares within groups.
        /// </summary>
        /// <value>The sum of squares within groups.</value>
        public double SumOfSquaresWithinGroups
        {
            get { return sumOfSquaresWithinGroups; }
        }

        #endregion
    }
}
