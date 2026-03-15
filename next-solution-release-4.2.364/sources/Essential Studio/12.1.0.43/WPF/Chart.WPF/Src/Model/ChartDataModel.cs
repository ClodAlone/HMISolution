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

using System.Data;
using System.Collections.ObjectModel;
using System.Xml;
using MS.Internal.Data;
using System.Diagnostics;


namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartDataModel
    /// </summary>
    public class ChartDataModel : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Empty constructor implementation for ChartDataModel
        /// </summary>
        public ChartDataModel()
        {
        }
        /// <summary>
        /// Called when instance created for ChartDataModel with single arguments.
        /// </summary>
        /// <param name="source"></param>
        public ChartDataModel(IEnumerable source)
        {
            this.Source = source;
        }
        /// <summary>
        /// Called when instance created for ChartDataModel with three arguments.
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
        /// Return IEnumerable value based on given object
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


        void notifyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void View_RecordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
  
        }
        #endregion

        internal ChartSeries m_ChartSeries = null;

        ChartValueType m_xValueType = ChartValueType.Double;
        /// <summary>
        /// Get or Set XValueType property
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
        /// Method implementation for BeginInit for performance
        /// </summary>
        public void BeginInit()
        {
            this.isBeginInit = true;

        }
        /// <summary>
        /// Method implementation for EndInit for Performance
        /// </summary>
        public void EndInit()
        {
            this.isBeginInit = false;  
        }


        internal void BeginChartInit()
        {

            //Missed to Uncomment the below line with chart wpf's performance fix commit- Revision 169540
            if (m_ChartSeries != null && m_ChartSeries.Area != null)
            {
                m_ChartSeries.Area.BeginInit();
            }

        }

        internal void EndChartInit()
        {

            //Missed to Uncomment the below line with chart wpf's performance fix commit- Revision 169540
            if (m_ChartSeries != null && m_ChartSeries.Area != null)
            {
                m_ChartSeries.Area.EndInit();
            }

        }

        private void UnwireSource(IEnumerable source)
        {
            if (source is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)source).CollectionChanged -= new NotifyCollectionChangedEventHandler(ChartDataModel_CollectionChanged);
            }

            foreach (object o in source)
            {
                if (o is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)o).PropertyChanged -= new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                }
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

                if (m_Source != value)
                {
                    if (m_Source != null)
                    {
                        UnwireSource(Source as IEnumerable);
                    }
                    m_Source = value;

                    NotifyPropertyChanged("Source");
                }

                
            }
        }


        private ChartListData m_points = new ChartListData();
        /// <summary>
        /// Get or Set ChartPoints property
        /// </summary>
        public ChartListData ChartPoints

        {
            get { return m_points; }
            set 
            { 
                m_points = value;
                NotifyPropertyChanged("ChartPoints");
            }
        }


        bool m_IsIndexed = true;

        /// <summary>
        /// Get and Set IsIndexed Property
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
        /// get and Set PathX property
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
        /// Get and Set PathY property
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
        /// Get and Set PositionPath property
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

        void View_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }
     

        internal bool CheckIsLegacyDataTable(IEnumerable source)
        {
            return source is DataTable || source is DataView;
        }

        internal bool IsLegacyDataTable
        {
            get
            {
                return this.Source is DataView || this.Source is DataTable;
            }
        }


        void 
            Refresh()
        {
		
            if (!this.isBeginInit )
            {
                if (m_ChartSeries.Area != null && !this.m_ChartSeries.Area.updateArea)
                {
                    this.GetChartPoints(this.Source, this.PathX, this.PathsY);
                }
                else
                {
                    this.BeginChartInit();
                    this.GetChartPoints(this.Source, this.PathX, this.PathsY);
                    this.EndChartInit();
                }
            }
			
        }


        internal ChartPoint GetIndividualChartPoint(object underlyingObject, string pathX, IEnumerable<string> pathsY)

        {
            if ((underlyingObject == null || pathX == null || pathsY == null) && (m_ChartSeries.Type != ChartTypes.Histogram))
            {
                return null;
            }

            object xval = null;
            
            if (underlyingObject is INotifyPropertyChanged)
            {
                (underlyingObject as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
            }
            if (pathX != string.Empty)
            {
                var propertyInfo_records = underlyingObject.GetType().GetProperty(pathX);
                if (propertyInfo_records != null)
                {
                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_records);
                    xval = accessor.GetValue(underlyingObject);
                }
                else
                {
                    xval = ChartDataUtils.GetObjectByPath(underlyingObject, pathX);
                }
            }
     ChartPoint point = new ChartPoint();
            object[] yval = null;
            if (pathsY != null)
                yval = new object[pathsY.Count<string>()];
            else
                yval = new object[1];

            if (this.ChartPoints == null)
            {
                return null;
            }
            int index = this.ChartPoints.Count;


            int i = 0;
            if (pathsY != null)
            {
                foreach (string pathy in pathsY)
                {

                    var propertyInfo_Y = underlyingObject.GetType().GetProperty(pathy);
                    if (propertyInfo_Y != null)
                    {
                        var accessor_y = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_Y);
                        yval[i++] = accessor_y.GetValue(underlyingObject);
                    }
                    else
                    {
                        yval[i++] = ChartDataUtils.GetObjectByPath(underlyingObject, pathy);
                    }

                }
            }

            point.X = this.GetXValue(xval, ref index);
            point.StringItem = this.XValueType == ChartValueType.String && xval != null ? xval.ToString() : index.ToString();

            if (yval != null)
            {
                point.Values = this.GetYValues(yval, ref index);
                point.Y = point.Values.Length > 0 ? point.Values[0] : double.NaN;
            }
            point.Tag = underlyingObject;

            point.Item = underlyingObject;

            //if(double.IsNaN(point.X))
            //    point = null;
            return point;
        }


        internal ChartPoint UpdatePoint(object underlyingObject, string pathX, IEnumerable<string> pathsY,int position)

        {
            if ((underlyingObject == null || pathX == null || pathsY == null) && (m_ChartSeries.Type != ChartTypes.Histogram))
            {
                return null;
            }

            object xval = null;
            //Commented for SD13801 - performance in dynamic update with INotifyChanged
            //if (underlyingObject is INotifyPropertyChanged)
            //{
            //    (underlyingObject as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
            //}
            if (pathX != string.Empty)
            {
                var propertyInfo_records = underlyingObject.GetType().GetProperty(pathX);
                if (propertyInfo_records != null)
                {
                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_records);
                    xval = accessor.GetValue(underlyingObject);
                }
                else
                {
                    xval = ChartDataUtils.GetObjectByPath(underlyingObject, pathX);
                }
            }

            ChartPoint point = new ChartPoint();
            object[] yval = null;
            if (pathsY != null)
                yval = new object[pathsY.Count<string>()];
            else
                yval = new object[1];

            if (this.ChartPoints == null)
            {
                return null;
            }
            
            int index = position;


            int i = 0;
            if (pathsY != null)
            {
                foreach (string pathy in pathsY)
                {

                    var propertyInfo_Y = underlyingObject.GetType().GetProperty(pathy);
                    if (propertyInfo_Y != null)
                    {
                        var accessor_y = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_Y);
                        yval[i++] = accessor_y.GetValue(underlyingObject);
                    }
                    else
                    {
                        yval[i++] = ChartDataUtils.GetObjectByPath(underlyingObject, pathy);
                    }

                }
            }

            point.X = this.GetXValue(xval, ref index);
            point.StringItem = this.XValueType == ChartValueType.String && xval != null ? xval.ToString() : index.ToString();

            if (yval != null)
            {
                point.Values = this.GetYValues(yval, ref index);
                point.Y = point.Values.Length > 0 ? point.Values[0] : double.NaN;
            }
            point.Tag = underlyingObject;

            point.Item = underlyingObject;

            //if(double.IsNaN(point.X))
            //    point = null;
            return point;
        }
        /// <summary>
        /// Static variable initialization for _pp
        /// </summary>
        public static ChartPoint _pp;
        /// <summary>
        /// Get or Set PP property
        /// </summary>
        public  ChartPoint pp
        {
            get { return _pp;}
            set { _pp=value; }
        }

        private static void CreateXmlSourceListWrapper(XmlNode itemsSource, ref IList source)
        {
            XmlNode xmlData = itemsSource as XmlNode;
            if (xmlData != null)
            {
                source.Add(xmlData);
                CreateXmlSourceListWrapper(xmlData.NextSibling, ref source);
            }
        }


        private void GeneratePoints(IEnumerable obj)
        {
            if(this.ChartPoints != null)
            this.ChartPoints.Clear();
            if (obj != null)
            {
                IEnumerator a;               
                if (obj is System.Xml.XmlElement)
                {
                    IList source = new List<XmlNode>();
                    CreateXmlSourceListWrapper((obj as XmlElement), ref source);
                    a = (source as IEnumerable).GetEnumerator();
                }
                else
                {
                    a = ((obj).GetEnumerator());
                }                
                if (a.MoveNext())
                {
                    
                    var propertyInfo_records1 = a.Current.GetType().GetProperty(this.PathX);
                    IPropertyAccessor accessor_X = null;
                    if (propertyInfo_records1 != null)
                        accessor_X = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_records1);
                    bool IsanyYvaluePropInfoIsNull = false;
                    IPropertyAccessor[] accessor_Ys = null;
                    if (this.PathsY != null)
                    {
                        accessor_Ys = new IPropertyAccessor[this.PathsY.Count()];
                        int i = 0;
                        foreach (string str in this.PathsY)
                        {
                            var propertyInfo_records2 = (a.Current).GetType().GetProperty(str);
                            if (propertyInfo_records2 != null)
                            {
                                accessor_Ys[i++] = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo_records2);
                            }
                            else
                            {
                                IsanyYvaluePropInfoIsNull = true;
                            }
                        }
                    }

                    ChartSeries ser = this.m_ChartSeries;
                    object xval = null;
                    // object obj1 = a.Current;
                    if(PathX != string.Empty)
                    {
                     xval= ChartDataUtils.GetObjectByPath(a.Current, PathX);
                    }
                    double d1;
                    if ((xval is string && !double.TryParse(xval.ToString(), out d1)) || this.IsIndexed || (xval is string && !(obj is XmlElement)))
                    {
                        this.XValueType = ChartValueType.String;
                    }
                    else if (xval is DateTime)
                    {
                        this.XValueType = ChartValueType.DateTime;
                    }

                    else if (xval is TimeSpan)
                    {
                        this.XValueType = ChartValueType.TimeSpan;
                    }
                    else
                    {
                        this.XValueType = ChartValueType.Double;
                    }
                    int index = 0;
                    if (a.Current is INotifyPropertyChanged)
                    {
                        # region each obj is INotifyPropertyChanged
                        switch (xval == null)
                        {
                            case true:
                                {
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = true;
                                    int ycou;
                                    if (this.PathsY != null)
                                    {
                                        ycou = this.PathsY.Count();
                                    }
                                    else
                                    {
                                        ycou = 0;
                                    }
                                    if (ycou > 0)
                                    {
                                        switch (this.XValueType)
                                        {
                                            case ChartValueType.DateTime:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.Double:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            //double d;
                                                            //double.TryParse(obj.ToString(), out d);

                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double d;
                                                            //double.TryParse(obj.ToString(), out d);

                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.String:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.TimeSpan:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        switch (this.XValueType)
                                        {
                                            case ChartValueType.DateTime:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                        //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.Double:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                        // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.String:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                        // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.TimeSpan:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                        //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                        }
                                    }
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = false;
                                    break;
                                }
                            case false:
                                {
                                    //if(m_ChartSeries != null)
                                    m_ChartSeries.HoldDataUpdate = true;
                                    int ycou;
                                    if (this.PathsY != null)
                                    {
                                        ycou = this.PathsY.Count();
                                    }
                                    else
                                    {
                                        ycou = 0;
                                    }
                                    if (ycou > 0)
                                    {
                                        if (accessor_X != null && accessor_X.GetValue(a.Current) != null)
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = ((DateTime)gen).ToOADate();
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = ((DateTime)gen).ToOADate();
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                //double X_val;
                                                                //double.TryParse(gen.ToString(), out X_val);
                                                                p.X = Convert.ToDouble(gen);
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                //double X_val;
                                                                //double.TryParse(gen.ToString(), out X_val);
                                                                p.X = Convert.ToDouble(gen);
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            //for (int l = 0; l < ycou; l++)
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            //for (int l = 0; l < ycou; l++)
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (accessor_X != null && accessor_X.GetValue(a.Current) != null)
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            (cur as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(ChartDataModel_PropertyChanged);

                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen;
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }

                                        }
                                    }
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = false;
                                    break;
                                }
                        }
                        #endregion
                    }
                    else
                    {
                        # region each obj is not INotifyPropertyChanged
                        switch (xval == null)
                        {
                            case true:
                                {
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = true;
                                    int ycou;
                                    if (this.PathsY != null)
                                    {
                                        ycou = this.PathsY.Count();
                                    }
                                    else
                                    {
                                        ycou = 0;
                                    }
                                    if (ycou > 0)
                                    {
                                        switch (this.XValueType)
                                        {
                                            case ChartValueType.DateTime:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.Double:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            //double d;
                                                            //double.TryParse(obj.ToString(), out d);

                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] =Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double d;
                                                            //double.TryParse(obj.ToString(), out d);

                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.String:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                            case ChartValueType.TimeSpan:
                                                {
                                                    ChartPoint p;
                                                    if (IsanyYvaluePropInfoIsNull)
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object[] Val = new object[ycou];
                                                            int l = 0;
                                                            foreach (string str in PathsY)
                                                            {
                                                                Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                l++;
                                                            }
                                                            // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = GetYValues(Val, ref index);
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    else
                                                    {
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            for (int l = 0; l < ycou; l++)
                                                            {
                                                                Val[l] = Convert.ToDouble(accessor_Ys[l].GetValue(cur));
                                                            }
                                                            //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        switch (this.XValueType)
                                        {
                                            case ChartValueType.DateTime:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        
                                                        //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.Double:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        
                                                        // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.String:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        
                                                        // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                            case ChartValueType.TimeSpan:
                                                {
                                                    ChartPoint p;
                                                    double[] Val = new double[1];
                                                    //for (int l = 0; l < ycou; l++)
                                                    //{
                                                    Val[0] = double.NaN;
                                                    // }
                                                    do
                                                    {
                                                        p = new ChartPoint();
                                                        object cur = a.Current;
                                                        
                                                        //object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                        p.Values = Val;
                                                        p.X = index;
                                                        p.Y = Val[0];
                                                        p.Tag = p.Item = cur;
                                                        p.StringItem = index.ToString();
                                                        this.ChartPoints.Add(p);
                                                        index++;
                                                    } while (a.MoveNext());
                                                    break;
                                                }
                                        }
                                    }
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = false;
                                    break;
                                }
                            case false:
                                {
                                    //if(m_ChartSeries != null)
                                    m_ChartSeries.HoldDataUpdate = true;
                                    int ycou;
                                    if (this.PathsY != null)
                                    {
                                        ycou = this.PathsY.Count();
                                    }
                                    else
                                    {
                                        ycou = 0;
                                    }
                                    if (ycou > 0)
                                    {
                                        if (accessor_X != null && accessor_X.GetValue(a.Current) != null)
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = ((DateTime)gen).ToOADate();
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = ((DateTime)gen).ToOADate();
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                //double X_val;
                                                                //double.TryParse(gen.ToString(), out X_val);
                                                                p.X = Convert.ToDouble(gen);
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                //double X_val;
                                                                //double.TryParse(gen.ToString(), out X_val);
                                                                p.X = Convert.ToDouble(gen);
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        if (IsanyYvaluePropInfoIsNull)
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                object[] Val = new object[ycou];
                                                                int l = 0;
                                                                foreach (string str in PathsY)
                                                                {
                                                                    Val[l] = ChartDataUtils.GetObjectByPath(cur, str);
                                                                    l++;
                                                                }
                                                                // object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = GetYValues(Val, ref index);
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                
                                                                double[] Val = new double[ycou];
                                                                for (int l = 0; l < ycou; l++)
                                                                {
                                                                    Val[l] = GetYValue(accessor_Ys[l].GetValue(cur), ref index);
                                                                }
                                                                object gen = accessor_X.GetValue(cur);
                                                                p.Values = Val;
                                                                p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = index.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            //for (int l = 0; l < ycou; l++)
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        if (!(a.Current is XmlElement))
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                               
                                                                double[] Val = new double[ycou];
                                                                int l = 0;
                                                                //for (int l = 0; l < ycou; l++)
                                                                foreach (string pathy in PathsY)
                                                                {
                                                                    Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                    l++;
                                                                }
                                                                object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = Val;
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                                
                                                            } while (a.MoveNext());
                                                        }
                                                        else
                                                        {
                                                            do
                                                            {
                                                                p = new ChartPoint();
                                                                object cur = a.Current;
                                                                if( ((XmlNode)cur).NodeType == XmlNodeType.Element)
                                                                {
                                                                double[] Val = new double[ycou];
                                                                int l = 0;
                                                                //for (int l = 0; l < ycou; l++)
                                                                foreach (string pathy in PathsY)
                                                                {
                                                                    Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                    l++;
                                                                }
                                                                object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                                p.Values = Val;
                                                                p.X = index;
                                                                p.Y = p.Values[0];
                                                                p.Tag = p.Item = cur;
                                                                p.StringItem = gen.ToString();
                                                                this.ChartPoints.Add(p);
                                                                index++;
                                                                }
                                                            } while (a.MoveNext());
                                                        }
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            double[] Val = new double[ycou];
                                                            int l = 0;
                                                            foreach (string pathy in PathsY)
                                                            {
                                                                Val[l] = GetYValue(ChartDataUtils.GetObjectByPath(cur, pathy), ref index);
                                                                l++;
                                                            }
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = p.Values[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (accessor_X != null && accessor_X.GetValue(a.Current) != null)
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = accessor_X.GetValue(cur);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            switch (this.XValueType)
                                            {
                                                case ChartValueType.DateTime:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((DateTime)gen).ToOADate();
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.Double:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            //double X_val;
                                                            //double.TryParse(gen.ToString(), out X_val);
                                                            p.X = Convert.ToDouble(gen);
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = index.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.String:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = index;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen.ToString();
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                                case ChartValueType.TimeSpan:
                                                    {
                                                        ChartPoint p;
                                                        double[] Val = new double[1];
                                                        //for (int l = 0; l < ycou; l++)
                                                        //{
                                                        Val[0] = double.NaN;
                                                        // }
                                                        do
                                                        {
                                                            p = new ChartPoint();
                                                            object cur = a.Current;
                                                            object gen = ChartDataUtils.GetObjectByPath(cur, PathX);
                                                            p.Values = Val;
                                                            p.X = ((TimeSpan)gen).TotalMilliseconds;
                                                            p.Y = Val[0];
                                                            p.Tag = p.Item = cur;
                                                            p.StringItem = gen;
                                                            this.ChartPoints.Add(p);
                                                            index++;
                                                        } while (a.MoveNext());
                                                        break;
                                                    }
                                            }

                                        }
                                    }
                                    if (m_ChartSeries != null)
                                        m_ChartSeries.HoldDataUpdate = false;
                                    break;
                                }
                        }
                        #endregion
                    }
                    //if (a.Current is XmlElement)
                    //{
                    //    ChartPoint p = new ChartPoint();
                    //    ChartPoint p1 = new ChartPoint();

                    //    do
                    //    {                        
                    //        int k = 0;
                    //        double[] val = new double[accessor_Ys.Count()];
                    //        foreach (IPropertyAccessor acc in accessor_Ys)
                    //        {
                    //            val[k] = (double)acc.GetValue(a.Current);
                    //            k++;
                    //        }
                    //        p.Values = val;                  
                    //        p.X = ((Double)accessor_X.GetValue(a.Current));
                    //        p.Y = p.Values[0];

                    //        p.Tag = a.Current;
                    //        p.Item = a.Current;
                    //        p.StringItem = accessor_X.GetValue(a.Current);
                    //        p1 = p;
                    //        this.ChartPoints.Add(p1);
                    //        index++;
                    //    } while (a.MoveNext());
                    //}
                    //else
                    //{     
                    //    Stopwatch sss = new Stopwatch();
                    //    sss.Start();
                    //    m_ChartSeries.HoldDataUpdate = true;
                    //    int ycou = this.PathsY.Count();     

                    //    ChartPoint p;
                    //    do
                    //    {
                    //            p = new ChartPoint();                           
                    //            object cur = a.Current;
                    //            double[] Val = new double[ycou];
                    //            for (int l = 0; l < ycou; l++)
                    //            {
                    //                Val[l] = (double)accessor_Ys[l].GetValue(cur);
                    //            }
                    //            object gen = accessor_X.GetValue(cur);
                    //            p.Values = Val;
                    //            p.X = ((DateTime)gen).ToOADate();
                    //            p.Y = p.Values[0];
                    //            p.Tag = p.Item = cur;
                    //            p.StringItem = gen;                           
                    //            this.ChartPoints.Add(p);                         
                    //            index++;                        
                    //    } while (a.MoveNext());
                    //    m_ChartSeries.HoldDataUpdate = false;
                    //    Console.WriteLine("Time taken in loop:" +sss.ElapsedMilliseconds.ToString());
                    //    sss.Stop();
                    //    sss.Reset();
                    //}
                }
            }
        }

        void ChartDataModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //ChartBindingData h= new ChartBindingData ();
            int position = 0;
            object obj = sender; 
            foreach (object o in Source as IEnumerable)
            {
                if (o.Equals(sender))
                {
                    obj = o;
                    break;
                }
                position++;
                
            }

            this.ChartPoints[position] = this.UpdatePoint(obj, this.PathX, this.PathsY, position);
           
        }


        internal ChartPoint GetChartPoints(object underlyingObject, string pathX, IEnumerable<string> pathsY)


        {
            if ((m_ChartSeries != null) && (m_ChartSeries.Type != ChartTypes.Histogram) && (underlyingObject == null || pathX == null || pathsY == null))
            {
                return null;
            }
           
            if (underlyingObject is IEnumerable)
            {



                this.GeneratePoints(underlyingObject as IEnumerable);



            }


            return null;
            }           
        /// <summary>
        /// Return the object based on given object and string values
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetOtherData(object data, string propertyName)
        {
            object result = null;

            if (data is XmlElement)
            {
                result = ((XmlElement)data).GetAttribute(propertyName);
            }

            return result;
        }



        /// <summary>
        /// Return IchartData values based on the given values
        /// </summary>
        /// <param name="pathX"></param>
        /// <param name="pathsY"></param>
        /// <returns></returns>
        public IChartData GetChartPointsData(string pathX, IEnumerable<string> pathsY)

        {

            return null;
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
            if (obj is DateTime)
            {
                Yvalue = ((DateTime)obj).ToOADate();
            }

            else
            {
                if (obj != null)
                {
                    //double d = 0d;
                    //double.TryParse(obj.ToString(), out d);
                    Yvalue = Convert.ToDouble(obj);
                }
            }
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

            else if (obj is TimeSpan)
            {
                //Xvalue = Convert.ToDateTime(obj.ToString()).ToOADate();
                TimeSpan ts = (TimeSpan)obj;
                obj = ts;
                this.XValueType = ChartValueType.TimeSpan;
                xValue = ts.TotalMilliseconds;
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
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.ChartPoints != null)
            {
                //this.ChartPoints.Dispose();
                this.BeginChartInit();
                for (int i = 0; i < this.ChartPoints.Count; i++)
                {

                    this.ChartPoints[i].Item = null;

                    this.ChartPoints[i].Tag = null;

                    this.ChartPoints[i].ParentSegment = null;

                }
                this.EndChartInit();
                this.ChartPoints = null;
            }

            this.PathX = null;
            this.PathsY = null;
            this.PropertyChanged = null;
            this.m_PathsY = null;
            this.m_ChartSeries = null;

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

                if ((this.Source != null && this.PathsY != null && this.ChartPoints != null ) ||( this.m_ChartSeries!= null && this.m_ChartSeries.Type == ChartTypes.Histogram))

                {
                    this.Refresh();
                }
            }
            else if (info.Equals("IsIndexed"))
            {
               
                if (this.Source != null && this.PathX != string.Empty && this.PathX != null && this.PathsY != null)

                {
                    this.Refresh();
                }
            }
            else if (info.Equals("PositionPath"))
            {
               
            }
            else if (info.Equals("ContentPath"))
            {
                
            }
            else if (info.Equals("Source"))
            {
                if (this.ChartPoints == null)
                    return;
                this.ChartPoints.Clear();
				
               // View = this.CreateCollectionViewAdv(this.Source);
                if (this.Source is IEnumerable)
                {

                    if (Source is INotifyCollectionChanged)
                    {
                        (this.Source as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ChartDataModel_CollectionChanged);
                    }
                }
				
                else if (this.Source is CollectionViewSource)
                {
                    //this.View.AddListener((this.Source as CollectionViewSource).View);
                    var collectionViewSource = this.Source as CollectionViewSource;
                    (collectionViewSource.SortDescriptions as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ChartDataModel_CollectionChanged);
                    collectionViewSource.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupDescriptions_CollectionChanged);
                }
                //this.ChartRecords.Clear();

     
                if ((this.Source != null && this.PathX != string.Empty && this.PathsY != null)||(m_ChartSeries!= null && m_ChartSeries.Type == ChartTypes.Histogram))

                {
                    //this.GetChartPointsData(this.PathX, this.PathsY);
                  
                    //m_ChartSeries.Area.HoldUpdate = false;
					
                    this.Refresh();
                }
            }
        }

        void ChartDataModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           
           
           
             switch (e.Action)
             {
                 case NotifyCollectionChangedAction.Replace:
                     if (e.NewStartingIndex > -1 && e.NewStartingIndex < this.ChartPoints.Count && e.NewItems[0] != null)
                     {
                         this.ChartPoints[e.NewStartingIndex] = this.GetIndividualChartPoint(e.NewItems[0], this.PathX, this.PathsY);
                     }
                     break;
                 case NotifyCollectionChangedAction.Reset:
                     if ((this.Source != null && this.PathX != null && this.PathsY != null) || (this.m_ChartSeries != null && this.m_ChartSeries.Type == ChartTypes.Histogram))
                     {
                         this.Refresh();
                     }
                     break;
                 case NotifyCollectionChangedAction.Remove:
                     if ((this.Source != null && this.PathX != null && this.PathX != string.Empty && this.PathsY != null) || (this.m_ChartSeries != null && this.m_ChartSeries.Type == ChartTypes.Histogram))
                     {
                         this.Refresh();
                     }
                     break;
                 case NotifyCollectionChangedAction.Add:
                     {
                         if ((this.Source != null && this.PathX != null && this.PathsY != null) || (this.m_ChartSeries != null && this.m_ChartSeries.Type == ChartTypes.Histogram))
                         {
                             ChartPoint p = this.GetIndividualChartPoint(e.NewItems[0], this.PathX, this.PathsY);
                             this.ChartPoints.Add(p);
                         }
                         break;
                     }
             }
           
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

        }
        #endregion
    }

   




}
