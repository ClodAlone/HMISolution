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

namespace Syncfusion.Windows.Tools.Controls
{
    public class BreadthsInfo
    {
        #region Members

        private double minwidth = 0.0;

        private double maxwidth = 0.0;

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public BreadthsInfo(double min,double max):this()
        {
            minwidth = min;
            maxwidth = max;
        }

        /// <summary>
        /// 
        /// </summary>
        public BreadthsInfo()
        {

        }


        /// <summary>
        /// It gets/sets the MinWidth value
        /// </summary>
        internal double MinWidth
        {
            get
            {
                return minwidth;
            }
            set
            {
                minwidth = value;
            }
        }


        /// <summary>
        /// It gets/sets the MaxWidth value
        /// </summary>
        internal double MaxWidth
        {
            get
            {
                return maxwidth;
            }
            set
            {
                maxwidth = value;
            }
        }

        /// <summary>
        /// Default Breadths value
        /// </summary>
        internal BreadthsInfo DefaultBreadths
        {
            get
            {
                return new BreadthsInfo(20d, 20d);
            }
        }

        /// <summary>
        /// Returns the Max of two Breadths
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        internal static BreadthsInfo Max(BreadthsInfo first, BreadthsInfo second)
        {
            return new BreadthsInfo(Math.Max(first.MinWidth, second.MinWidth), Math.Max(first.MaxWidth, second.MaxWidth));
        }

        /// <summary>
        /// Returns the Min of Two Breadths
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        internal static BreadthsInfo Min(BreadthsInfo first, BreadthsInfo second)
        {
            return new BreadthsInfo(Math.Min(first.MinWidth, second.MinWidth), Math.Min(first.MaxWidth, second.MaxWidth));
        }
    }
}
