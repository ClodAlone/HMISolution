#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Silverlight.Chart.Olap
{
    /// <summary>
    /// Data object for defining the chart points
    /// </summary>
    public class OlapChartPoint
    {
        #region Initilize/Finalize
        
        public OlapChartPoint()
        {
            this.BindingX = string.Empty;
        }

        public OlapChartPoint(string x, double y)
        {
            this.BindingX = x;
            this.BindingY = y;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the binding X.
        /// </summary>
        /// <value>The binding X.</value>
        public string BindingX 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the binding Y.
        /// </summary>
        /// <value>The binding Y.</value>
        public double BindingY 
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        #endregion
    }
}
