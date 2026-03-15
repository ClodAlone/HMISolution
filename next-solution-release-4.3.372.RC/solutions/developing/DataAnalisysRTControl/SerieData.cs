using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using Utilities;
using OPCUAViewModel;
using WPFPenHelpers;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using WPFUtilities;
using Utilities.Converters;

namespace DataAnalisysRTControl
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum LineType
    {
        lineSeries,
        lineStepSeries,
        lineStackedSeries,
        lineFullStackedSeries,
        areaSeries,
        areaStepSeries,
        areaStackedSeries,
        areaFullStackedSeries,
        barSideSeries
    };

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }


    [DataContract(Name = "SerieData")]
    public class SerieData : INotifyPropertyChanged
    {
        #region declaration
        bool _dlrsource;
        #endregion

        #region Constructors
        public SerieData()
        {
            Initialize();
        }

        public SerieData(SerieData instance)
        {
            if (instance != null)
            {
                tagName = instance.tagName;
                usesourcetimestamp = instance.usesourcetimestamp;
                localizeSourceTimeStamp = instance.localizeSourceTimeStamp;
                sourcetimestampColumnName = instance.sourcetimestampColumnName;
                historicalName = instance.historicalName;
                dlrsource = instance.dlrsource;
                useTableAggregation = instance.useTableAggregation;
                minAggregation = instance.minAggregation;
                maxAggregation = instance.maxAggregation;
                avgAggregation = instance.avgAggregation;
                showaxis = instance.showaxis;
                authomaticscale = instance.authomaticscale;
                eUnit = instance.eUnit;
                min = instance.min;
                max = instance.max;
                logarithmicYScale = instance.logarithmicYScale;
                logarithmicbaseYScale = instance.logarithmicbaseYScale;
                isstatisticenabled = instance.isstatisticenabled;
                title = instance.title;
                color = instance.color;
                thickness = instance.thickness;
                serieTypeLine = instance.serieTypeLine;
                serieType = instance.serieType;
                nodeID = instance.nodeID;
                isVisible = instance.isVisible;
                isSet = instance.isSet;
                tagreferenceXml = instance.tagreferenceXml;
                conditionalTagXml = instance.conditionalTagXml;
                guiId = instance.guiId;
                arrayindex = instance.arrayindex;
                useeuminmax = instance.useeuminmax;
                addVirtualPoints = instance.addVirtualPoints;
                pointPrecision = instance.pointPrecision == null ? -1 : instance.pointPrecision;
                penUnitConverter = instance.penUnitConverter;
            }

            Initialize();
        }

        void Initialize()
        {
            if (!isSet)
            {
                isVisible = true;
                authomaticscale = true;
                isSet = true;
            }
        }
        #endregion

        #region Members persistance
        [DataMember]
        public String tagName { get; set; }
        [DataMember]
        public String historicalName { get; set; }
        [DataMember]
        public bool useTableAggregation { get; set; }
        [DataMember]
        public bool minAggregation { get; set; }
        [DataMember]
        public bool maxAggregation { get; set; }
        [DataMember]
        public bool avgAggregation { get; set; }
        [DataMember]
        public bool dlrsource
        {
            get
            {
                return _dlrsource;
            }
            set
            {
                _dlrsource = value;
                OnPropertyChanged("dlrsource");
            }
        }

        [DataMember]
        public bool usesourcetimestamp { get; set; }
        [DataMember]
        public bool localizeSourceTimeStamp { get; set; }
        [DataMember]
        public String sourcetimestampColumnName { get; set; }
        [DataMember]
        public bool showaxis { get; set; }
        [DataMember]
        public bool authomaticscale { get; set; }
        [DataMember]
        public String eUnit { get; set; }
        [DataMember]
        public double min { get; set; }
        [DataMember]
        public double max { get; set; }
        [DataMember]
        public bool logarithmicYScale { get; set; }
        [DataMember]
        public double logarithmicbaseYScale { get; set; }
        [DataMember]
        public bool isstatisticenabled { get; set; }
        [DataMember]
        public String title { get; set; }
        [DataMember]
        public Color color { get; set; }
        [DataMember]
        public int thickness { get; set; }
        [Obsolete("Use serieType insted of this.")]
        [DataMember]
        public String serieTypeLine { get; set; }
        [Obsolete("Use serieType insted of this.")]
        [DataMember]
        public String serieTypeLineKey { get; set; }
        [DataMember]
        public String nodeID { get; set; }
        [DataMember]
        public bool isVisible { get; set; }
        [DataMember]
        public bool isSet { get; set; }
        [DataMember]
        public String tagreferenceXml { get; set; }
        [DataMember]
        public String conditionalTagXml { get; set; }
        [DataMember]
        public String guiId { get; set; }
        [DataMember]
        public int arrayindex { get; set; }
        [DataMember]
        public bool useeuminmax { get; set; }
        [DataMember]
        public bool addVirtualPoints { get; set; }
        [DataMember]
        public int? pointPrecision { get; set; }
        [DataMember]
        public string penUnitConverter { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LineType serieType 
        {
            get
            {
                LineType _serieType;
                if (Enum.TryParse(serieTypeLineKey, true, out _serieType))
                    return _serieType;
                else
                    return LineType.lineSeries;
            }
            set
            {
                serieTypeLineKey = value.ToString();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference tagReference
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
                    tagreferenceXml = value.ToXml();
                }
                else
                {
                    tagreferenceXml = string.Empty;
                }
                OnPropertyChanged("tagReference");
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference conditionalTag
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(conditionalTagXml))
                        return conditionalTagXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    conditionalTagXml = value.ToXml();
                }
                else
                {
                    conditionalTagXml = string.Empty;
                }
                OnPropertyChanged("conditionalTag");
            }
        }
        #endregion

        #region Methods

        public Dictionary<string, object> ToDictionary(IUFUAEditorManager UFUAEditor, DocumentManager.ComponentService.IDocument document, List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(tagreferenceXml))
                res.Add("SVGReferenceId", CreateUniqueName(title, usedIDs));
            else
                res.Add("SVGReferenceId", null);
            if (!string.IsNullOrEmpty(conditionalTagXml))
                res.Add("SVGReferenceConditionalId", CreateUniqueName($"{title}_Condition", usedIDs));
            else
                res.Add("SVGReferenceConditionalId", null);
            res.Add("tagName", tagName);
            res.Add("usesourcetimestamp", usesourcetimestamp);
            res.Add("localizeSourceTimeStamp", localizeSourceTimeStamp);
            res.Add("sourcetimestampColumnName", sourcetimestampColumnName);

            OPCUAEntityReference item = null;
            if (tagreferenceXml != null)
            {
                item = tagreferenceXml.FromXml<OPCUAEntityReference>();
                if (item != null && item.ResolvedNodeId != null)
                    res.Add("ColumnTagName", TagPathHelper.GetTagPath(null, item.ReadablePath, false));
                else
                    res.Add("ColumnTagName", string.Empty);
            }
            else
                res.Add("ColumnTagName", string.Empty);

            if (!dlrsource)
            {
                if (item != null && item.ResolvedNodeId != null)
                {
                    var doc = UFUAEditor.GetProjectDocument(document, item.AppName, true);
                    if (doc != null)
                    {
                        historicalName = UFUAEditor.GetHistorianName(doc, item.ResolvedNodeId);
                    }
                }
                else
                    historicalName = null;
            }

            res.Add("historicalName", historicalName);
            res.Add("dlrsource", dlrsource);
            res.Add("useTableAggregation", useTableAggregation);
            res.Add("minAggregation", minAggregation);
            res.Add("maxAggregation", maxAggregation);
            res.Add("avgAggregation", avgAggregation);
            res.Add("showaxis", showaxis);
            res.Add("authomaticscale", authomaticscale);
            res.Add("eUnit", eUnit);
            res.Add("min", min);
            res.Add("max", max);
            res.Add("logarithmicYScale", logarithmicYScale);
            res.Add("logarithmicbaseYScale", logarithmicbaseYScale);
            res.Add("isstatisticenabled", isstatisticenabled);
            res.Add("title", title);
            res.Add("color", $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}");
            res.Add("thickness", thickness);
            res.Add("serieTypeLine", serieTypeLine);
            res.Add("serieTypeLineKey", serieType.ToString());
            res.Add("serieType", serieType);
            res.Add("isVisible", isVisible);
            res.Add("isSet", isSet);
            res.Add("arrayindex", arrayindex);
            res.Add("useeuminmax", useeuminmax);
            res.Add("addVirtualPoints", addVirtualPoints);
            res.Add("nodeID", nodeID);
            res.Add("pointPrecision", pointPrecision);
            res.Add("penUnitConverter", penUnitConverter);
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

        public bool SetTag(IDocument document, string varName)
        {
            if (document == null)
                return false;

            var original = tagReference;
            string instance;
            string name;
            var UFUAEditor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor == null)
                return false;

            SmartControlHelper.GetInstanceName(varName, out instance, out name);
            var xml = UFUAEditor.GetTagEntityReference(document, name, instance);
            if (!String.IsNullOrEmpty(xml))
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                if (tag == null)
                    return false;

                tagReference = tag;
                guiId = null;
                nodeID = tag.ResolvedNodeId?.ToString();
                var doc = UFUAEditor.GetProjectDocument(document, tag.AppName, true);
                if (doc != null)
                {
                    historicalName = UFUAEditor.GetHistorianName(doc, tag.ResolvedNodeId);
                }
                var pos = varName.LastIndexOf("\\") + 1;
                tagName = varName.Substring(pos, varName.Length - pos);
                return true;
            }
            return false;
        }
        #endregion

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

    [CollectionDataContract(
    Name = "SerieDataList",
    ItemName = "SerieData")]
    public class SerieDataList : List<SerieData>
    {
        #region Constructors
        public SerieDataList()
        { }

        public SerieDataList(IEnumerable<SerieData> collection) :
            base(collection)
        { }

        public SerieDataList(SerieDataList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new SerieData(item));
        }
        #endregion

        public List<object> ToDictionary(IUFUAEditorManager UFUAEditor, DocumentManager.ComponentService.IDocument document)
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(UFUAEditor, document, list)));
            return res;
        }
    }

    internal class ConvertSerieDataList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            DocumentManager.ComponentService.IDocument iDocument = document as DocumentManager.ComponentService.IDocument;
            IUFUAEditorManager UFUAEditor = iDocument?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var entity = value as SerieDataList;
            if (entity == null)
                return (new SerieDataList()).ToDictionary(UFUAEditor, iDocument);

            return entity.ToDictionary(UFUAEditor, iDocument);
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<object>);
            }
        }
    }
}
