#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class TemplateSelector : DependencyObject
    {
        #region properties

        private double maximumY;

        /// <summary>
        /// Gets the maximum Y axis value.
        /// </summary>
        public double MaximumY
        {
            get { return maximumY; }
            internal set { maximumY = value; }
        }

        private double minimumY;

        /// <summary>
        /// Gets the minimum Y axis value.
        /// </summary>
        public double MinimumY
        {
            get { return minimumY; }
            internal set { minimumY = value; }
        }

        private SparklineBase sparkline;

        /// <summary>
        /// Gets the sparkline.
        /// </summary>
        public SparklineBase Sparkline
        {
            get { return sparkline; }
            internal set { sparkline = value; }
        }

        private int dataCount;

        /// <summary>
        /// Gets the data count.
        /// </summary>
        public int DataCount
        {
            get { return dataCount; }
            internal set { dataCount = value; }
        }

        private double minimumX;

        /// <summary>
        /// Gets the minimum X axis value.
        /// </summary>
        public double MinimumX
        {
            get { return minimumX; }
            internal set { minimumX = value; }
        }


        #endregion

        #region ctor

        public TemplateSelector()
        {

        }

        #endregion

        #region methods

        protected internal virtual DataTemplate SelectTemplate(double x, double y)
        {
            return null;
        }

        internal void SetData(SparklineBase sparkline, int count)
        {
            this.minimumY = sparkline.minYValue;
            this.maximumY = sparkline.maxYValue;
            this.dataCount = count;
            this.sparkline = sparkline;
            minimumX = sparkline.minXValue;
        }

        #endregion

    }
}
