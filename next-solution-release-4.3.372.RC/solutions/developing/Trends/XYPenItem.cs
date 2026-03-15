using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using Opc.Ua;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using Utilities;

namespace Trends
{
    public class XYPenItem : INotifyPropertyChanged
    {
        #region Constructors
        public XYPenItem()
        { }

        public XYPenItem(XYPenItem instance)
        {
            if (instance == null)
                return;

            NodeId = instance.NodeId;
            ArrayIndex = instance.ArrayIndex;
            Name = instance.Name;
            XTagName = instance.XTagName;
            YTagName = instance.YTagName;
            if (instance.Range != null)
                Range = new Range(instance.Range.High, instance.Range.Low);
            UseEURange = instance.UseEURange;
            LColor = instance.LColor;
            LFColor = instance.LFColor;
            Visible = instance.Visible;
            XTagReferenceXml = instance.XTagReferenceXml;
            YTagReferenceXml = instance.YTagReferenceXml;
            StrokeThickness = instance.StrokeThickness;
            Dlrsource = instance.Dlrsource;
            XHistoricalName = instance.XHistoricalName;
            XColumnName = instance.XColumnName;
            YHistoricalName = instance.YHistoricalName;
            YColumnName = instance.YColumnName;
            PenStyle = instance.PenStyle;
            PointPrecision = instance.PointPrecision == null ? -1 : instance.PointPrecision;
        }
        #endregion

        #region Private Members
        private String nodeid;
        private int arrayindex = 0;
        private String name;
        private String xtagname;
        private String ytagname;
        private Range range;
        private Boolean useeurange;
        private Color lcolor;
        private Color lfcolor;
        private Boolean visible;
        private String xtagreferenceXml;
        private String ytagreferenceXml;
        private double strokethickness = 1;
        private bool dlrsource;
        private String xhistoricalName;
        private String yhistoricalName;
        private String xcolumnName;
        private String ycolumnName;
        private PredefinedPenKinds penstyle;
        private int? pointPrecision;
        #endregion

        #region Public Properties
        public String serieTypeLineKey { get; set; }
        public String NodeId
        {
            get
            {
                if (string.IsNullOrEmpty(nodeid))
                    nodeid = Guid.NewGuid().ToString();
                return nodeid;
            }
            set
            {
                if (nodeid == value)
                    return;
                nodeid = value;
                OnPropertyChanged("NodeId");
            }
        }

        public double StrokeThickness
        {
            get { return strokethickness; }
            set
            {
                if (strokethickness == value)
                    return;
                strokethickness = value;
                OnPropertyChanged("StrokeThickness");
            }
        }
        public int ArrayIndex
        {
            get { return arrayindex; }
            set
            {
                if (arrayindex == value)
                    return;
                arrayindex = value;
                OnPropertyChanged("ArrayIndex");
            }
        }

        public bool Dlrsource
        {
            get { return dlrsource; }
            set
            {
                if (dlrsource == value)
                    return;
                dlrsource = value;
                OnPropertyChanged("Dlrsource");
            }
        }

        public String XHistoricalName
        {
            get { return xhistoricalName; }
            set
            {
                if (xhistoricalName == value)
                    return;
                xhistoricalName = value;
                OnPropertyChanged("XHistoricalName");
            }
        }

        public String YHistoricalName
        {
            get { return yhistoricalName; }
            set
            {
                if (yhistoricalName == value)
                    return;
                yhistoricalName = value;
                OnPropertyChanged("YHistoricalName");
            }
        }
        public String XColumnName
        {
            get { return xcolumnName; }
            set
            {
                if (xcolumnName == value)
                    return;
                xcolumnName = value;
                OnPropertyChanged("XColumnName");
            }
        }

        public String YColumnName
        {
            get { return ycolumnName; }
            set
            {
                if (ycolumnName == value)
                    return;
                ycolumnName = value;
                OnPropertyChanged("YColumnName");
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public String XTagName
        {
            get { return xtagname; }
            set
            {
                if (xtagname == value)
                    return;
                xtagname = value;
                OnPropertyChanged("XTagName");
            }
        }

        public String YTagName
        {
            get { return ytagname; }
            set
            {
                if (ytagname == value)
                    return;
                ytagname = value;
                OnPropertyChanged("YTagName");
            }
        }

        public Range Range
        {
            get { return range; }
            set
            {
                if (range == value)
                    return;
                range = value;
                OnPropertyChanged("Range");
            }
        }

        public Boolean UseEURange
        {
            get { return useeurange; }
            set
            {
                if (useeurange == value)
                    return;
                useeurange = value;
                OnPropertyChanged("UseEURange");
            }
        }

        public Color LColor
        {
            get { return lcolor; }
            set
            {
                if (lcolor == value)
                    return;
                lcolor = value;
                OnPropertyChanged("LColor");
            }
        }

        public Color LFColor
        {
            get { return lfcolor; }
            set
            {
                if (lfcolor == value)
                    return;
                lfcolor = value;
                OnPropertyChanged("LFColor");
            }
        }

        public Boolean Visible
        {
            get { return visible; }
            set
            {
                if (visible == value)
                    return;
                visible = value;
                OnPropertyChanged("Visible");
            }
        }

        public String XTagReferenceXml
        {
            get { return xtagreferenceXml; }
            set
            {
                if (xtagreferenceXml == value)
                    return;
                xtagreferenceXml = value;
                OnPropertyChanged("XTagReferenceXml");
            }
        }
        public String YTagReferenceXml
        {
            get { return ytagreferenceXml; }
            set
            {
                if (ytagreferenceXml == value)
                    return;
                ytagreferenceXml = value;
                OnPropertyChanged("YTagReferenceXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference XTagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(xtagreferenceXml))
                        return xtagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    XTagName = value.ResolvedNodeId != null ? value.ResolvedNodeId.ToString() : value.RelativePath;
                    XTagReferenceXml = value.ToXml();
                    OnPropertyChanged("XTagReference");
                }
                else
                {
                    XTagName = string.Empty;
                    XTagReferenceXml = string.Empty;
                    OnPropertyChanged("XTagReference");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference YTagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(ytagreferenceXml))
                        return ytagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    YTagName = value.ResolvedNodeId != null ? value.ResolvedNodeId.ToString() : value.RelativePath;
                    YTagReferenceXml = value.ToXml();
                    OnPropertyChanged("YTagReference");
                }
                else
                {
                    YTagName = string.Empty;
                    YTagReferenceXml = string.Empty;
                    OnPropertyChanged("YTagReference");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PredefinedPenKinds PenStyle
        {
            get
            {
                PredefinedPenKinds _serieType;
                if (Enum.TryParse(serieTypeLineKey, true, out _serieType))
                    return _serieType;
                else
                    return penstyle;
            }
            set
            {
                serieTypeLineKey = value.ToString();
            }
        }

        public int? PointPrecision
        {
            get { return pointPrecision; }
            set
            {
                pointPrecision = value;
                OnPropertyChanged("PointPrecision");
            }
        }
        #endregion

        public Dictionary<string, object> ToDictionary(IUFUAEditorManager UFUAEditor, DocumentManager.ComponentService.IDocument document, List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            string xhistoricalName = null;
            string yhistoricalName = null;
            OPCUAEntityReference itemX = xtagreferenceXml.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference itemY = ytagreferenceXml.FromXml<OPCUAEntityReference>();
            if (itemX != null && itemX.ResolvedNodeId != null)
            {
                xhistoricalName = UFUAEditor.GetHistorianName(document, itemX.ResolvedNodeId);
            }
            if (itemY != null && itemY.ResolvedNodeId != null)
            {
                yhistoricalName = UFUAEditor.GetHistorianName(document, itemY.ResolvedNodeId);
            }
            Dictionary<string, object> xtagRef = new Dictionary<string, object>();
            Dictionary<string, object> ytagRef = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(xtagreferenceXml))
                xtagRef.Add("SVGReferenceId", CreateUniqueName($"{Name}{ChartXY.ArgumentPlaceHolder}", usedIDs));
            else
                xtagRef.Add("SVGReferenceId", null);
            if (!string.IsNullOrEmpty(ytagreferenceXml))
                ytagRef.Add("SVGReferenceId", CreateUniqueName($"{Name}{ChartXY.ValuePlaceHolder}", usedIDs));
            else
                ytagRef.Add("SVGReferenceId", null);

            res.Add("XTagReference", xtagRef);
            res.Add("YTagReference", ytagRef);
            res.Add("StrokeThickness", StrokeThickness);
            res.Add("ArrayIndex", ArrayIndex);
            res.Add("Name", Name);
            res.Add("XTagName", XTagName);
            res.Add("YTagName", YTagName);
            res.Add("XColumnName", XColumnName);
            res.Add("YColumnName", YColumnName);
            res.Add("XHistoricalName", xhistoricalName);
            res.Add("YHistoricalName", yhistoricalName);
            res.Add("Range", Range);
            res.Add("UseEURange", UseEURange);
            res.Add("LColor", $"#{LColor.R.ToString("X2")}{LColor.G.ToString("X2")}{LColor.B.ToString("X2")}");
            res.Add("LFColor", $"#{LFColor.R.ToString("X2")}{LFColor.G.ToString("X2")}{LFColor.B.ToString("X2")}");
            res.Add("Visible", Visible);
            res.Add("PenStyle", PenStyle);
            res.Add("PointPrecision", PointPrecision);
            return res;
        }

        internal String CreateUniqueName(String name, List<String> list)
        {
            if (string.IsNullOrEmpty(name))
                name = $"{Properties.Resources.PropertyInspectorPenPrefix} {Properties.Resources.DefaultPenName}";
            else
                name = $"{Properties.Resources.PropertyInspectorPenPrefix} {name}";
            if (!list.Contains(name))
            {
                list.Add(name);
                return name;
            }
            var newname = name;
            int i = 0;
            while (list.Contains(newname))
                newname = String.Format("{0} ({1})", name, ++i);

            list.Add(newname);
            return newname;
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
