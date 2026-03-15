#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents ChartAdornmentPresenter
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartAdornmentPresenter: Canvas
    {
        #region ctor

        

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the VisibleSeries. This is a dependency property.
        /// </summary>
        /// <value>The VisibleSeries.</value>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<ChartSeriesBase> VisibleSeries
        {
            get { return (ObservableCollection<ChartSeriesBase>)GetValue(VisibleSeriesProperty); }
            set { SetValue(VisibleSeriesProperty, value); }
        }

        /// <summary>
        ///  Identifies the VisibleSeries dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleSeriesProperty =
            DependencyProperty.Register("VisibleSeries", typeof(ObservableCollection<ChartSeriesBase>), typeof(ChartAdornmentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnVisibleSeriesPropertyChanged)));

        /// <summary>
        /// Gets or Sets the Series collection in Chart.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesBase Series
        {
            get { return (ChartSeriesBase)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeriesBase), typeof(ChartAdornmentPresenter), new PropertyMetadata(null));

        #endregion

        #region methods

        private static void OnVisibleSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        internal void Update(Size availableSize)
        {
            if (Series != null && Series.adornmentInfo != null)
            {
                Series.adornmentInfo.Measure(availableSize, this);
            }
        }

        internal void Arrange(Size finalSize)
        {
            if (Series != null && Series.adornmentInfo != null)
            {
                Series.adornmentInfo.Arrange(finalSize);
            }
        }

        #endregion
    }
}
