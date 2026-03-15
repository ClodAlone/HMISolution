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
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
   /// <summary>
    /// Class implementation for TriangularSeriesBase
   /// </summary>
   public abstract class TriangularSeriesBase:AccumulationSeriesBase
   {

        #region properties

       ///<summary>
        ///Gets or Sets GapRatio.
        ///</summary>
        [ClassReference(IsReviewed = false)]
        public double GapRatio
        {
            get { return (double)GetValue(GapRatioProperty); }
            set { SetValue(GapRatioProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for GapRatio.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty GapRatioProperty =
            DependencyProperty.Register("GapRatio", typeof(double), typeof(TriangularSeriesBase), new PropertyMetadata(0d,new PropertyChangedCallback(OnGapRatioChanged)));

        private static void OnGapRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as TriangularSeriesBase).Area!=null)
                (d as TriangularSeriesBase).Area.ScheduleUpdate();
        }

        /// <summary>
        /// Get or Set ExplodeOffset property
        /// </summary>
        public double ExplodeOffset
        {
            get { return (double)GetValue(ExplodeOffsetProperty); }
            set { SetValue(ExplodeOffsetProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ExplodeOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExplodeOffsetProperty =
            DependencyProperty.Register("ExplodeOffset", typeof(double), typeof(TriangularSeriesBase), new PropertyMetadata(40d));

       #endregion

        #region methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as TriangularSeriesBase).GapRatio = this.GapRatio;
            (obj as TriangularSeriesBase).ExplodeOffset = this.ExplodeOffset;
            return base.CloneSeries(obj);
        }

        #endregion
   }
}
