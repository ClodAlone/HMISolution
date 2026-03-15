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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for AdornmentSeries
    /// </summary>
    public abstract class AdornmentSeries : ChartSeries
    {

        #region Properties

      

        /// <summary>
        /// Gets or Sets ChartAdornmentInfo. This allows us to customize the appearance of a data point by displaying labels, shapes and connector lines.
        /// </summary>
        /// <value>
        /// The <see cref="ChartAdornmentInfo" /> value.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public ChartAdornmentInfo AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                SetValue(AdornmentsInfoProperty, value);
            }
        }

        /// <summary>
        /// Identifies the AdornmentsInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register("AdornmentsInfo", typeof(ChartAdornmentInfo), typeof(ChartSeriesBase), new PropertyMetadata(null, OnAdornmentsInfoChanged));

        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as AdornmentSeries;

            if (e.OldValue != null)
            {
                var adornmentInfo = e.OldValue as ChartAdornmentInfo;
                if (series != null) series.Adornments.Clear();

                if (adornmentInfo != null) 
                { 
                    adornmentInfo.ClearChildren();
                    adornmentInfo.Series = null;
                }
            }

            if (e.NewValue != null)
            {
                if (series != null)
                {
                    series.adornmentInfo = e.NewValue as ChartAdornmentInfo;
                    series.AdornmentsInfo.Series = series;
                    if (series.Area != null && series.AdornmentsInfo != null)
                    {
                        //Panel panel = series.Area.GetMarkerPresenter();
                        Panel panel = series.AdornmentPresenter;
                        if (panel != null)
                        {
                            series.AdornmentsInfo.PanelChanged(panel);
                            series.Area.ScheduleUpdate();
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// call this method when Adornments render on Series
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (!(this is ErrorBarSeries))
            {
                AdornmentPresenter.Series = this;
                if (Area != null && AdornmentsInfo != null)
                {
                    //Panel panel = Area.GetMarkerPresenter();
                    Panel panel = AdornmentPresenter;
                    if (panel != null)
                    {
                        AdornmentsInfo.PanelChanged(panel);
                    }
                }
            }
        }

        /// <summary>
        /// An abstract method which will be called over to create segments.
        /// </summary>
        public override void CreateSegments()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series"></param>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        protected virtual ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double xPos, double yPos)
        {
            return new ChartAdornment(xVal, yVal, xPos, yPos, series);
        }
        /// <summary>
        /// Method implementation for Add ColumnAdornments in Chart
        /// </summary>
        /// <param name="values"></param>
        protected virtual void AddColumnAdornments(params double[] values)
        {
            //values[0] -->   xData
            //values[1] -->   yData
            //values[2] -->   xPos
            //values[3] -->   yPos
            //values[4] -->   data point index
            //values[5] -->   Median value.

            double adornposX = values[2] + values[5], adornposY = values[3];
            int pointIndex = (int)values[4];

            if (pointIndex < Adornments.Count)
            {
                Adornments[pointIndex].SetData(values[0], values[1], adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, values[0], values[1], adornposX, adornposY));
            }
            if (ActualData.Count > pointIndex)
                Adornments[pointIndex].Item = ActualData[pointIndex];
        }
        /// <summary>
        /// Method implementation for Add Adornments at XY
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pointindex"></param>
        protected virtual void AddAdornmentAtXY(double x,double y,int pointindex)
        {
            double adornposX = x, adornposY = y;

            if (pointindex < Adornments.Count)
            {
                Adornments[pointindex].SetData(x, y, adornposX, adornposY);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, x, y, adornposX, adornposY));
            }
            if (ActualData.Count > pointindex)
                Adornments[pointindex].Item = ActualData[pointindex];
        }
        /// <summary>
        /// Method implementation for Add AreaAdornments in ChartAdornments
        /// </summary>
        /// <param name="values"></param>
        protected virtual void AddAreaAdornments(params IList<double>[] values)
        {
            IList<double> yValues = values[0];
            List<double> xValues = GetXValues();

            if (values.Length == 1)
            {
                int i;
                for (i = 0; i < DataCount; i++)
                {
                    double adornX = xValues[i];
                    double adornY = yValues[i];
                    if (i < Adornments.Count)
                    {
                        Adornments[i].SetData(xValues[i], yValues[i], adornX, adornY);
                    }
                    else
                    {
                        Adornments.Add(CreateAdornment(this, xValues[i], yValues[i], adornX, adornY));
                    }
                    if (ActualData.Count > i)
                        Adornments[i].Item = ActualData[i];
                }
            }
        }

        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (AdornmentPresenter != null && AdornmentsInfo != null)
            {
                AdornmentsInfo.UpdateElements();
            }

            base.UpdateOnSeriesBoundChanged(size);

            if (AdornmentPresenter != null && AdornmentsInfo != null)
            {
                AdornmentPresenter.Update(size);
                AdornmentPresenter.Arrange(size);
            }
        }

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            if (AdornmentsInfo != null)
            {
                Adornments.Clear();
                AdornmentsInfo.UpdateElements();
            }
            base.OnDataSourceChanged(oldValue, newValue);
        }
        /// <summary>
        /// Method implementation for Clear Unused Adornments
        /// </summary>
        /// <param name="startIndex"></param>
        protected void ClearUnUsedAdornments(int startIndex)
        {
            if (Adornments.Count > startIndex)
            {
                int count = Adornments.Count;

                for (int i = startIndex; i < count; i++)
                {
                    Adornments.RemoveAt(startIndex);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            if (this.AdornmentsInfo != null)
                (obj as AdornmentSeries).AdornmentsInfo = (ChartAdornmentInfo)this.AdornmentsInfo.Clone();
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
