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
using System.Collections;
using Syncfusion.Windows.Chart;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Markup;
using Syncfusion.Linq;
using Syncfusion.Windows.Data;


namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartDataModel
    /// </summary>
    public class ChartDataModel : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Called when instance created with emplty arguments for ChartDataModel
        /// </summary>
        public ChartDataModel()
        {
        }

        /// <summary>
        /// Called when instance created for ChartDataModel with single argument 
        /// </summary>
        /// <param name="source"></param>
        public ChartDataModel(IEnumerable source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Called when instance created for ChartDataModel with three arguments
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pathX"></param>
        /// <param name="pathsY"></param>
        public ChartDataModel(IEnumerable source, string pathX, IEnumerable<string> pathsY)
        {
            this.BeginInit();
            this.Source = source;
            this.PathX = pathX;
            this.PathsY = pathsY;
            this.EndInit();
        }


        /// <summary>
        /// Virtual method return IEnumerable value from given object
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {

                if (source is CollectionViewSource)
                {

                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = GetSourceList(cvs.View.SourceCollection);
                    }
                }


                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).SourceCollection;
                    result = GetSourceList(sourceList);
                }
                else
                {
                    result = source as IEnumerable;
                }

            }

            return result;
        }


        void View_RecordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
            object record = sender;
            int index = this.View.Records.IndexOfRecord(record);
            ChartPointRecord chartRecord = this.View.Records.GetRecord(record) as ChartPointRecord;
            IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
            if (chartRecord != null && index >= 0 && this.ChartPoints != null && index < this.ChartPoints.Count)
            {
                this.BeginChartInit();
                ChartPoint updatedPoint = this.GetIndividualChartPoint(record, this.PathX, this.PathsY);

                this.ChartPoints.series.IsDataModified = true;

                if (chartRecord.Data is string || this.IsIndexed)
                {
                    updatedPoint.X = index;
                }
                this.ChartPoints[index] = updatedPoint;
                chartRecord.Point = updatedPoint;
                this.EndChartInit();
            }

        }
        #endregion

        internal ChartSeries m_ChartSeries = null;

        ChartValueType m_xValueType = ChartValueType.Double;
        /// <summary>
        /// Get or set XValueType property
        /// </summary>
        public ChartValueType XValueType
        {
            get
            {
                return m_xValueType;
            }

            internal set
            {
                m_xValueType = value;
            }
        }

        bool isBeginInit = false;
        /// <summary>
        /// Method implementation for increase performence 
        /// </summary>
        public void BeginInit()
        {
            this.isBeginInit = true;

            if(View != null)
            View.BeginInit();

        }

        /// <summary>
        /// EndInit method implementation for  performance 
        /// </summary>
        public void EndInit()
        {
            this.isBeginInit = false;
            
            if(View != null)
            View.EndInit();
            //this.Refresh();

        }


        internal void BeginChartInit()
        {

            if (this.ChartPoints.series != null && this.ChartPoints.series.Area != null)
            {
                this.ChartPoints.series.Area.BeginInit();
            }

        }

        internal void EndChartInit()
        {

            if (this.ChartPoints.series != null && this.ChartPoints.series.Area != null)
            {
                this.ChartPoints.series.Area.EndInit();
            }

        }

        object m_Source = null;
        /// <summary>
        /// Get or Set Source property 
        /// </summary>
        public object Source
        {
            get { return m_Source; }
            set
            {

                m_Source = value;

                NotifyPropertyChanged("Source");

                
            }
        }

        ICollectionViewAdv m_View = null;
        /// <summary>
        /// Get or Set view property 
        /// </summary>
        public ICollectionViewAdv View
        {
            get { return m_View; }
            internal set
            {
                m_View = value;
                NotifyPropertyChanged("View");
            }
        }


        private ChartPointsCollection m_points = new ChartPointsCollection();
        /// <summary>
        /// Get or Set ChartPoints
        /// </summary>
        public ChartPointsCollection ChartPoints

        {
            get { return m_points; }
            set 
            { 
                m_points = value;
                NotifyPropertyChanged("ChartPoints");
            }
        }


        bool m_IsIndexed = false;

        /// <summary>
        /// Get or Set IsIndexed property 
        /// </summary>
        public bool IsIndexed
        {
            get { return m_IsIndexed; }
            set
            {
                if (value != m_IsIndexed)
                {
                    m_IsIndexed = value;
                    NotifyPropertyChanged("IsIndexed");
                }
            }
        }

        string m_PathX = string.Empty;
        /// <summary>
        /// Get or Set pathX property 
        /// </summary>
        public string PathX
        {
            get { return m_PathX; }
            set
            {
                if (value != m_PathX)
                {
                    m_PathX = value;
                    NotifyPropertyChanged("PathX");
                }
            }
        }

        IEnumerable<string> m_PathsY = null;
        /// <summary>
        /// Get or Set PathY property
        /// </summary>
        public IEnumerable<string> PathsY
        {
            get { return m_PathsY; }
            set
            {
                m_PathsY = value;
                NotifyPropertyChanged("PathsY");
            }
        }

        string m_ContentPath = null;
        /// <summary>
        /// Get or Set ContentPath property
        /// </summary>
        public string ContentPath
        {
            get { return m_ContentPath; }
            set
            {
                m_ContentPath = value;
                NotifyPropertyChanged("ContentPath");
            }
        }

        string m_PositionPath = null;
        /// <summary>
        /// Get or Set positionPath property 
        /// </summary>
        public string PositionPath
        {
            get { return m_PositionPath; }
            set
            {
                m_PositionPath = value;
                NotifyPropertyChanged("PositionPath");
            }
        }

        /// <summary>
        /// Get or Set GetAxisContents property
        /// </summary>
        public List<object> GetAxisContents
        {
            get
            {
                if (this.ChartPoints.Count > 0 && this.ContentPath != null)
                {
                    return this.ChartPoints.Select(point => point.AxisContent).Cast<object>().ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get or Set getAxisPosition property
        /// </summary>
        public List<double> GetAxisPositions
        {
            get
            {
                if (this.ChartPoints.Count > 0 && this.PositionPath != null)
                {
                    return this.ChartPoints.Select(point => point.AxisPosition).Cast<double>().ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Method implementation for Update the Axis contentPath from the given string
        /// </summary>
        /// <param name="propertyName"></param>
        public void RefreshAxisContentPath(string propertyName)
        {
            if (this.View != null)
            {
                IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
                foreach (ChartPoint point in this.ChartPoints)
                {
                    point.AxisContent = access.GetValue(point.Tag, propertyName);
                }
            }
        }

        /// <summary>
        /// Method implementation for Update the Axis contentPath from the given string
        /// </summary>
        /// <param name="propertyName"></param>
        public void RefreshAxisPositionPath(string propertyName)
        {
            if (this.View != null)
            {
                IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
                int index = 0;
                foreach (ChartPoint point in this.ChartPoints)
                {
                    index++;
                    point.AxisPosition = this.IsIndexed ? index : this.GetXValue(access.GetValue(point.Tag, propertyName), ref index);
                }
            }
        }

        void View_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            this.ChartPoints.series.IsDataModified = true;

            IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex > -1 && e.NewStartingIndex < this.ChartPoints.Count && e.NewItems[0] != null)
                    {
                        this.ChartPoints[e.NewStartingIndex] = this.GetIndividualChartPoint(e.NewItems[0], this.PathX, this.PathsY);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if ((this.View != null && this.PathX!= null && this.PathsY != null && !this.isInitialRefresh) || (this.m_ChartSeries != null && this.m_ChartSeries.Type == ChartTypes.Histogram))
                    {
                        this.GetChartPointsData(this.PathX, this.PathsY);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if ((this.View != null && this.PathX != null && this.PathX != string.Empty && this.PathsY != null && !this.isInitialRefresh && (this.IsIndexed == true||this.XValueType== ChartValueType.String)) || (this.m_ChartSeries != null && this.m_ChartSeries.Type == ChartTypes.Histogram))
                    {
                        this.GetChartPointsData(this.PathX, this.PathsY);
                    }
                    break;
            }

            if (this.isInitialRefresh && this.ChartPoints.Count>0)
                this.isInitialRefresh = false;

        }
     /// <summary>
        /// Virtual method implementation for CreateCollectionViewAdv
     /// </summary>
     /// <param name="source"></param>
     /// <returns></returns>
        protected virtual ICollectionViewAdv CreateCollectionViewAdv(object source)
        {
            var res = source as IEnumerable;
            var datasource = this.GetSourceList(source);

            ICollectionViewAdv view = null;
            if (datasource != null)
            {
                    ChartDataQueryableCollectionViewWrapper data = new ChartDataQueryableCollectionViewWrapper(datasource, this);
                    //{
                    //    if (this.View != null && this.PathX != string.Empty && this.PathsY != null)
                    //    {
                    //        ChartPoint point = this.GetIndividualChartPoint(rec, this.PathX, this.PathsY);
                    //        this.ChartPoints.Add(point);

                    //        ChartPointRecord record = new ChartPointRecord(rec, point);
                    //        return record;
                    //    }

                    //    return null;
                    //});
                    view = data;

            }

            return view;
        }



        void 
            Refresh()
        {

			 if (!this.isBeginInit && View != null)
            {
                this.BeginChartInit();
                View.Refresh();
                this.EndChartInit();
            }
			
        }


        internal ChartPoint GetIndividualChartPoint(object underlyingObject, string pathX, IEnumerable<string> pathsY)

        {
            if ((underlyingObject == null || pathX == null || pathsY == null) && (m_ChartSeries.Type != ChartTypes.Histogram))
            {
                return null;
            }

            IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
            object xval = access.GetValue(underlyingObject,pathX);

            ChartPoint point = new ChartPoint();
            object[] yval = null;
            if (pathsY != null)
                yval = new object[pathsY.Count<string>()];
            else
                yval = new object[1];

            int index = this.ChartPoints.Count + 1;

            int i = 0;
            if (pathsY != null)
            {
                foreach (string pathy in pathsY)
                {


                    yval[i++] = access.GetValue(underlyingObject, pathy);

                }
            }

            point.X = this.GetXValue(xval, ref index);
            point.StringItem = this.XValueType == ChartValueType.String && xval != null ? xval.ToString() : index.ToString();

            point.AxisContent = this.ContentPath != null ? access.GetValue(underlyingObject, this.ContentPath) : null;
            point.AxisPosition = this.PositionPath != null ? this.GetXValue(access.GetValue(underlyingObject, this.PositionPath), ref index) : double.NaN;

            if (yval != null)
            {
                point.Values = this.GetYValues(yval, ref index);
                point.Y = point.Values.Length > 0 ? point.Values[0] : double.NaN;
            }
            point.Tag = underlyingObject;

            //if(double.IsNaN(point.X))
            //    point = null;
            return point;
        }


        internal ChartPoint UpdatePoint(object underlyingObject, string pathX, IEnumerable<string> pathsY)

        {
            if ((underlyingObject == null || pathX == null || pathsY == null) && (m_ChartSeries.Type != ChartTypes.Histogram))
            {
                return null;
            }

            IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
            object xval = access.GetValue(underlyingObject,pathX);

            ChartPoint point = new ChartPoint();
            object[] yval = null;
            if (pathsY != null)
                yval = new object[pathsY.Count<string>()];
            else
                yval = new object[1];

            int index = this.ChartPoints.Count + 1;

            int i = 0;
            if (pathsY != null)
            {
                foreach (string pathy in pathsY)
                {


                    yval[i++] = access.GetValue(underlyingObject, pathy);
                    

                }
            }

            point.X = this.GetXValue(xval, ref index);
            point.StringItem = this.XValueType == ChartValueType.String && xval != null ? xval.ToString() : index.ToString();

            point.AxisContent = this.ContentPath != null ? access.GetValue(underlyingObject, this.ContentPath) : null;
            point.AxisPosition = this.PositionPath != null ? this.GetXValue(access.GetValue(underlyingObject, this.PositionPath), ref index) : double.NaN;

            if (yval != null)
            {
                point.Values = this.GetYValues(yval, ref index);
                point.Y = point.Values.Length > 0 ? point.Values[0] : double.NaN;
            }
            point.Tag = underlyingObject;

            //if(double.IsNaN(point.X))
            //    point = null;
            return point;
        }
       


        /// <summary>
        /// Return object Fron the given object value
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetOtherData(object data, string propertyName)
        {
            object result = null;

            return result;
        }



        /// <summary>
        /// Return ChartPointsCollection values from the given values 
        /// </summary>
        /// <param name="pathX"></param>
        /// <param name="pathsY"></param>
        /// <returns></returns>
        public ChartPointsCollection GetChartPointsData(string pathX, IEnumerable<string> pathsY)

        {

            if (View != null)
            {
                //bool isAddNew = false;
                IPropertyAccessProvider access = this.View.GetPropertyAccessProvider();
                //int index = 1;

                //this.BeginChartInit();
                if(this.ChartPoints!= null)
                    this.ChartPoints.Clear();
                else
                    return null;
                foreach (ChartPointRecord record in this.View.Records)
                {
                    //isAddNew = false;
                    //ChartPoint point = new ChartPoint();// this.GetChartPointObject(index, (this.View.Records.Count == index), out isAddNew);
                    //object xval = access.GetValue(record.Data, pathX);
                    //if (xval == null)
                    //{
                    //    xval = this.GetOtherData(record.Data, pathX);
                    //}

                    //object[] yval = new object[pathsY.Count<string>()];
                    //int i=0;
                    //foreach (string pathy in pathsY)
                    //{
                    //    object actualValue = access.GetValue(record.Data, pathy);
                    //    yval[i++] = actualValue == null ? this.GetOtherData(record.Data, pathy) : actualValue;
                    //}

                    //point.X = this.GetXValue(xval, ref index);
                    //point.Values = this.GetYValues(yval, ref index);
                    //point.Y = point.Values.Length > 0 ? point.Values[0] : double.NaN;
                    //point.Tag = record.Data;
                    ////if (isAddNew)
                    ////{
                    //    //ChartPoints.Add(point);
                    ////}

                    ////this.ChartRecords.Add(new ChartPointRecord(record.Data, point));


                    //index++;
                    ChartPoint point = this.GetIndividualChartPoint(record.Data, pathX, pathsY);
                    record.Point = point;
                    this.ChartPoints.Add(point);

                }
                //this.EndChartInit();

                return ChartPoints;
            }


       
            return new ChartPointsCollection();

            
        }

        double[] GetYValues(IEnumerable<object> objects, ref int index)
        {
            double[] YValues = new double[objects.Count<object>()];
            int i = 0;
            foreach (object obj in objects)
            {
                YValues[i++] = this.GetYValue(obj, ref index);
            }

            return YValues;
        }

        double GetYValue(object obj, ref int index)
        {
            double Yvalue = double.NaN;
            //try
            //{
                //if (obj is DateTime)
                //{
                //    Yvalue = ((DateTime)obj).ToOADate();
                //}

                //else
                //{
                    if (obj != null)
                    {
                        //double d = 0d;
                        //double.TryParse(obj.ToString(), out d);
                        Yvalue = Convert.ToDouble(obj);
                    }
                //}
            //}
            //catch
            //{
            //    Yvalue = index;
            //}

            return Yvalue;
        }
        
        double GetXValue(object obj, ref int index)
        {
            double xValue = double.NaN;
            this.XValueType = ChartValueType.Double;
            //try
            //{
            double d1;
            if ((obj is string && !double.TryParse(obj.ToString(), out d1)) || this.IsIndexed || (obj is string))
            { 
                xValue = index;
                this.XValueType = ChartValueType.String;
            }
            else if (obj is DateTime)
            {
                xValue = ((DateTime)obj).ToOADate();
                this.XValueType = ChartValueType.DateTime;
            }

            else
            {
                if (obj != null)
                {
                    double d;
                    double.TryParse(obj.ToString(), out d);
                    xValue = d;
                }
                else
                {
                    xValue = index;
                }
            }
            //}
            //catch
            //{
            //    Xvalue = index;
            //}

            return xValue;
        }


        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.ChartPoints != null)
            {
                //this.ChartPoints.Dispose();
                this.BeginChartInit();
                for (int i = 0; i < this.ChartPoints.Count; i++)
                {

                    this.ChartPoints[i].Tag = null;

                }
                this.EndChartInit();
                this.ChartPoints = null;
            }

            this.PathX = null;
            this.PathsY = null;
            this.PropertyChanged = null;
            this.m_PathsY = null;
            this.m_ChartSeries = null;

            if (View != null)
            {
                for (int i = 0; i < View.Records.Count; i++)
                {
                    View.Records[i].Dispose();
                }
                View.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(View_CollectionChanged);
                var collectionViewAdv = this.View as CollectionViewAdv;
                if (collectionViewAdv != null)
                {
                    collectionViewAdv.RecordsListCollectionChanged -= Records_CollectionChanged;
                }

                View.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(View_PropertyChanged);
                View.RecordPropertyChanged -= new PropertyChangedEventHandler(View_RecordPropertyChanged);                
            }

            if (this.View is ChartDataQueryableCollectionViewWrapper)
            {
                ((ChartDataQueryableCollectionViewWrapper)this.View).Dispose();
            }

            this.View = null;
            this.m_View = null;
            this.m_points = null;

        }
        #endregion

        #region Property Changed Event
        /// <summary>
        /// Called when property changed in ChartDataModel
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }

            if (info.Equals("PathX") || info.Equals("PathsY"))
            {

                if ((this.View != null && this.PathsY != null) ||( this.m_ChartSeries!= null && this.m_ChartSeries.Type == ChartTypes.Histogram))

                {
                    this.Refresh();
                }
            }
            else if (info.Equals("IsIndexed"))
            {
                
                if (this.View != null && this.PathX != string.Empty && this.PathX != null && this.PathsY != null)

                {
                    this.Refresh();
                }
            }
            else if (info.Equals("PositionPath"))
            {
              
                if (this.View != null && this.ContentPath != string.Empty && this.ChartPoints.Count > 0)
                {

                    RefreshAxisPositionPath(this.ContentPath);

                }
                    
            }
            else if (info.Equals("ContentPath"))
            {
                
                if (this.View != null &&  this.ContentPath != null && this.ContentPath != string.Empty && this.ChartPoints.Count > 0)
                {

                    RefreshAxisContentPath(this.ContentPath);

                }
              
            }
            else if (info.Equals("Source"))
            {
                if (this.ChartPoints == null)
                    return;
                this.ChartPoints.Clear();
		
				View = this.CreateCollectionViewAdv(this.Source);
                if (this.Source is ICollectionView)
                {
                    //this.View.AddListener(this.Source as ICollectionView);
                    ((this.Source as ICollectionView).SortDescriptions as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ChartDataModel_CollectionChanged);
                    (this.Source as ICollectionView).GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupDescriptions_CollectionChanged);
                }
				
                else if (this.Source is CollectionViewSource)
                {
                    //this.View.AddListener((this.Source as CollectionViewSource).View);
                    var collectionViewSource = this.Source as CollectionViewSource;
                    (collectionViewSource.SortDescriptions as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ChartDataModel_CollectionChanged);
                    collectionViewSource.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupDescriptions_CollectionChanged);
                }
                //this.ChartRecords.Clear();

                if (View != null)
                {
                    View.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(View_CollectionChanged);
                    var collectionViewAdv = this.View as CollectionViewAdv;
                    if (collectionViewAdv != null)
                    {
                        collectionViewAdv.RecordsListCollectionChanged += Records_CollectionChanged;
                    }

                    View.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(View_PropertyChanged);
                    View.RecordPropertyChanged += new PropertyChangedEventHandler(View_RecordPropertyChanged);
                }

               
                if ((this.View != null && this.PathX != string.Empty && this.PathsY != null)||(m_ChartSeries!= null && m_ChartSeries.Type == ChartTypes.Histogram))

                {
                    //this.GetChartPointsData(this.PathX, this.PathsY);
                    this.isInitialRefresh = true;
				    
                    this.Refresh();
                }
            }
        }
        bool isInitialRefresh = true;

        void ChartDataModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           
         
			var collectionView = this.Source as ICollectionView;
            collectionView = collectionView == null ? (this.Source as CollectionViewSource).View : collectionView;
            var descriptions = (sender as IEnumerable).AsQueryable();
            if (collectionView != null && descriptions != null)
            {
                bool flag = true;
                foreach (SortDescription desc in descriptions)
                {
                    if (flag)
                    {
                        this.View.SortDescriptions.Clear();
                        flag = !flag;
                    }

                    this.View.SortDescriptions.Add(desc);
                }
            }
            //if (collectionView != null && descriptions != null)
            //{
            //    if (e.Action == NotifyCollectionChangedAction.Reset)
            //    {
            //        foreach (SortDescription desc in descriptions)
            //        {
            //            this.View.SortDescriptions.Add(desc);
            //        }
            //    }
            //    else if (e.Action == NotifyCollectionChangedAction.Add)
            //    {
            //        foreach (SortDescription item in descriptions)
            //        {
            //            this.View.SortDescriptions.Add(item);
            //        }
            //    }
            //    else if (e.Action == NotifyCollectionChangedAction.Remove)
            //    {
            //        foreach (SortDescription item in descriptions)
            //        {
            //            this.View.SortDescriptions.Remove(item);
            //        }
            //    }
            //}
			
        }

        void GroupDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
		    
            var collectionView = this.Source as ICollectionView;
            var descriptions = (sender as IEnumerable).AsQueryable();
            if (collectionView != null && descriptions != null)
            {
                bool flag = true;
                foreach (GroupDescription desc in collectionView.GroupDescriptions)
                {
                    if (flag)
                    {
                        this.View.SortDescriptions.Clear();
                        flag = !flag;
                    }

                    this.View.GroupDescriptions.Add(desc);
                }
			
            }
           
            //if (e.Action == NotifyCollectionChangedAction.Reset)
            //{
            //    foreach (GroupDescription desc in (this.Source as ICollectionView).GroupDescriptions)
            //    {
            //        this.View.GroupDescriptions.Add(desc);
            //    }
            //}
        }

        void Records_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ChartPointRecord record in e.OldItems)
                {
                    this.ChartPoints.Remove(record.Point);
                }
            }

        }
        #endregion
    }

    /// <summary>
    /// ChartPoint Record
    /// </summary>
	
    public class ChartPointRecord : RecordEntry, IDisposable
    {
        /// <summary>
        /// Called when instance created for chartpointRecord with two arguments
        /// </summary>
        /// <param name="data"></param>
        /// <param name="chartPoint"></param>
        public ChartPointRecord(object data, ChartPoint chartPoint)
            : base(null, -1, data)
        {
            this.Point = chartPoint;
        }

        /// <summary>
        /// Get or Set point property
        /// </summary>
        public ChartPoint Point
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Querable collectionview Wrapper class
    /// </summary>
    public class ChartDataQueryableCollectionViewWrapper : QueryableCollectionView, IDisposable
    {
        /// <summary>
        /// Called when instance created for ChartDataQueryableCollectionViewWrapper
        /// </summary>
        /// <param name="source"></param>
        /// <param name="model"></param>
        public ChartDataQueryableCollectionViewWrapper(IEnumerable source, ChartDataModel model)
            : base(source)
        {
            this.model = model;
        }

        private ChartDataModel model;

        /// <summary>
        /// Creates the record entry.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns/>
        public override RecordEntry CreateRecordEntry(object data)
        {
            //if ((data != null && this.PathX != null && this.PathsY != null) || (this.m_series != null && this.type == ChartTypes.Histogram ))
            if ((data != null && this.model.PathX != null && this.model.PathsY != null) || (this.model.m_ChartSeries != null && this.model.m_ChartSeries.Type == ChartTypes.Histogram))
            {
                if (this.model.ChartPoints == null)
                    return null;
                ChartPoint point = this.model.GetIndividualChartPoint(data, this.model.PathX, this.model.PathsY);
                this.model.ChartPoints.Add(point);

                ChartPointRecord record = new ChartPointRecord(data, point);
                return record;
            }

            return null;

            //return this.createRecordFunc(data, this);
        }

        /// <summary>
        /// Creates the item properties provider. Override this method to have return custom <see cref="T:Syncfusion.Windows.Data.IPropertyAccessProvider"/> and have customizations.
        /// </summary>
        /// <returns/>
        protected override IPropertyAccessProvider CreateItemPropertiesProvider()
        {
            return base.CreateItemPropertiesProvider();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.model.View.DeferRefresh();
            this.model = null;
        }

        #endregion
    }






}
