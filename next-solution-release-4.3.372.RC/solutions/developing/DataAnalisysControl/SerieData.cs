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

namespace DataAnalisysControl
{
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
                serieTypeLineKey = instance.serieTypeLineKey;
                nodeID = instance.nodeID;
                isVisible = instance.isVisible;
                isSet = instance.isSet;
                tagreferenceXml = instance.tagreferenceXml;
                guiId = instance.guiId;
            }

            Initialize();
        }

        void Initialize()
        {
            if (!isSet)
            {
                isVisible = true;
                isSet = true;
            }
        }
        #endregion

        #region Methods

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("SVGReferenceId", guiId);
            res.Add("tagName", tagName);
            res.Add("usesourcetimestamp", usesourcetimestamp);
            res.Add("localizeSourceTimeStamp", localizeSourceTimeStamp);
            res.Add("sourcetimestampColumnName", sourcetimestampColumnName);
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
            res.Add("serieTypeLineKey", serieTypeLineKey);
            res.Add("isVisible", isVisible);
            res.Add("isSet", isSet);
            res.Add("nodeID", nodeID);
            return res;
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
        [DataMember]
        public String serieTypeLine { get; set; }
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
        public String guiId { get; set; }
        //[DataMember]
        //public int? arrayIndex { get; set; }

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

        public List<object> ToDictionary()
        {
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary()));
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
            var entity = value as SerieDataList;
            if (entity == null)
                return (new SerieDataList()).ToDictionary();

            return entity.ToDictionary();
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
