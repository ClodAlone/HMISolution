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
using System.Windows;
using System.Collections;
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Collections;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for XyDataseries 
    /// </summary>
    public abstract class XyDataSeries:CartesianSeries
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the property path to retrieve y data from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { SetValue(YBindingPathProperty, value); }
        }

       
       /// <summary>
        /// Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(XyDataSeries), new PropertyMetadata(null,OnYBindingPathChanged));

        private static void OnYBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as XyDataSeries).OnBindingPathChanged(e);
        }
        /// <summary>
        /// Get or Set YValues
        /// </summary>
        protected internal IList<double> YValues
        {
            get;
            set;
        }

        #endregion

        #region Ctor
        /// <summary>
        /// Called when instance created for XyDataSeries 
        /// </summary>
        public XyDataSeries()
        {
            YValues = new List<double>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            if(YBindingPath!=null)
                GeneratePoints(new string[] { YBindingPath }, YValues);
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            YValues.Clear();
            GeneratePoints(new string[] { YBindingPath }, YValues);
            this.UpdateArea();
        }


        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in YValues)
            {
                if(double.IsNaN(yValue) && ShowEmptyPoints)
                    ValidateDataPoints(YValues);break;
            }
           
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as XyDataSeries).YBindingPath = this.YBindingPath;
            return base.CloneSeries(obj);
        }

        #endregion 
    }
}
