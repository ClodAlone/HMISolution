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
using Utilities;
using UFUAEditor.ComponentService;
using DevExpress.Pdf.Native;

namespace Trends
{
    public class PenItem : INotifyPropertyChanged
    {
        #region Constructors
        public PenItem()
        { }

        public PenItem(PenItem instance)
        {
            if (instance == null)
                return;

            NodeId = instance.NodeId;
            ArrayIndex = instance.ArrayIndex;
            Name = instance.Name;
            TagName = instance.TagName;
            if (instance.Range != null)
                Range = new Range(instance.Range.High, instance.Range.Low);
            UseEURange = instance.UseEURange;
            LColor = instance.LColor;
            Visible = instance.Visible;
            TagReferenceXml = instance.TagReferenceXml;
            StrokeThickness = instance.StrokeThickness;
            Dlrsource = instance.Dlrsource;
            HistoricalName = instance.HistoricalName;
            ColuName = instance.ColuName;
            PenStyle = instance.PenStyle;
            PointSettings = new PointSettings(instance.PointSettings);
            PointPrecision = instance.PointPrecision == null ? -1 : instance.PointPrecision;
        }
        #endregion

        #region Private Members
        private String nodeid;
        private int arrayindex = 0;
        private String name;
        private String tagname;
        private Range range;
        private Boolean useeurange;
        private Color lcolor;
        private Boolean visible;
        private String tagreferenceXml;
        private double strokethickness = 1;
        private bool dlrsource;
        private String historicalName;
        private String columnName;
        private PredefinedPenKinds penstyle;
        private PointSettings pointSettings;
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

        public String HistoricalName
        {
            get { return historicalName; }
            set
            {
                if (historicalName == value)
                    return;
                historicalName = value;
                OnPropertyChanged("HistoricalName");
            }
        }

        public String ColuName
        {
            get { return columnName; }
            set
            {
                if (columnName == value)
                    return;
                columnName = value;
                OnPropertyChanged("ColuName");
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

        public String TagName
        {
            get { return tagname; }
            set
            {
                if (tagname == value)
                    return;
                tagname = value;
                OnPropertyChanged("TagName");
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

        public PointSettings PointSettings
        {
            get
            {
                if (pointSettings == null)
                    pointSettings = new PointSettings();
                return pointSettings;
            }
            set
            {
                if (pointSettings == value)
                    return;
                pointSettings = value;
                OnPropertyChanged("PointSettings");
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

        public int? PointPrecision
        {
            get { return pointPrecision; }
            set
            {
                if (pointPrecision == value)
                    return;
                pointPrecision = value;
                OnPropertyChanged(nameof(PointPrecision));
            }
        }

        public String TagReferenceXml
        {
            get { return tagreferenceXml; }
            set
            {
                if (tagreferenceXml == value)
                    return;
                tagreferenceXml = value;
                OnPropertyChanged("TagReferenceXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(tagreferenceXml))
                        return tagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    TagName = value.ResolvedNodeId != null ? value.ResolvedNodeId.ToString() : value.RelativePath;
                    TagReferenceXml = value.ToXml();
                    OnPropertyChanged("TagReference");
                }
                else
                {
                    TagName = string.Empty;
                    TagReferenceXml = string.Empty;
                    OnPropertyChanged("TagReference");
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
        #endregion

        public Dictionary<string, object> ToDictionary(IUFUAEditorManager UFUAEditor, DocumentManager.ComponentService.IDocument document, List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            string historicalName = null;
            OPCUAEntityReference item = tagreferenceXml.FromXml<OPCUAEntityReference>();
            if (item != null && item.ResolvedNodeId != null)
            {
                historicalName = UFUAEditor.GetHistorianName(document, item.ResolvedNodeId);
            }
            if (!string.IsNullOrEmpty(tagreferenceXml))
                res.Add("SVGReferenceId", CreateUniqueName(Name, usedIDs));
            else
                res.Add("SVGReferenceId", null);
            res.Add("StrokeThickness", StrokeThickness);
            res.Add("ArrayIndex", ArrayIndex);
            res.Add("ColuName", ColuName);
            res.Add("Name", Name);
            res.Add("TagName", TagName);
            res.Add("HistoricalName", historicalName);
            res.Add("Range", Range);
            res.Add("UseEURange", UseEURange);
            res.Add("LColor", $"#{LColor.R.ToString("X2")}{LColor.G.ToString("X2")}{LColor.B.ToString("X2")}");
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
