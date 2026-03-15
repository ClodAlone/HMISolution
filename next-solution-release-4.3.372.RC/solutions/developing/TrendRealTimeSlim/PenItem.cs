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

namespace TrendRealTimeSlim
{
    [DataContract(Name = "PenItem")]
    public class PenItem : INotifyPropertyChanged
    {
        #region Constructors
        public PenItem()
        {
            Initialize();
        }

        public PenItem(PenItem instance)
        {
            if (instance != null)
            {
                Name = instance.Name;
                ColuName = instance.ColuName;
                TagName = instance.TagName;
                ShowAxis = instance.ShowAxis;
                AutoScale = instance.AutoScale;
                Range = new Opc.Ua.Range(instance.Range.Low, instance.Range.High);
                title = instance.title;
                LColor = instance.LColor;
                StrokeThickness = instance.StrokeThickness;
                serieTypeLine = instance.serieTypeLine;
                PenStyle = instance.PenStyle;
                NodeId = instance.NodeId;
                Visible = instance.Visible;
                isSet = instance.isSet;
                TagReferenceXml = instance.TagReferenceXml;
                guiId = instance.guiId;
                ArrayIndex = instance.ArrayIndex;
                UseEURange = instance.UseEURange;
                addVirtualPoints = instance.addVirtualPoints;
                PointPrecision = instance.PointPrecision == null ? -1 : instance.PointPrecision;
                PenUnitConverter = instance.PenUnitConverter;
            }

            Initialize();
        }

        void Initialize()
        {
            if (!isSet)
            {
                Visible = true;
                AutoScale = true;
                isSet = true;
            }
        }
        #endregion

        #region Members persistance
        [DataMember]
        public String NodeId { get; set; }
        [DataMember]
        public int StrokeThickness { get; set; }
        [DataMember]
        public int ArrayIndex { get; set; }
        [DataMember]
        public String ColuName { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String TagName { get; set; }
        [DataMember]
        public Opc.Ua.Range Range { get; set; }
        [DataMember]
        public bool UseEURange { get; set; }
        [DataMember]
        public bool AutoScale { get; set; }
        [DataMember]
        public bool ShowAxis { get; set; }
        [DataMember]
        public Color LColor { get; set; }
        [DataMember]
        public PointSettings PointSettings { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        [DataMember]
        public String TagReferenceXml { get; set; }
        [DataMember]
        public String title { get; set; }
        [Obsolete("Use serieType insted of this.")]
        [DataMember]
        public String serieTypeLine { get; set; }
        [Obsolete("Use serieType insted of this.")]
        [DataMember]
        public String serieTypeLineKey { get; set; }
        [DataMember]
        public bool isSet { get; set; }
        [DataMember]
        public String guiId { get; set; }
        [DataMember]
        public bool addVirtualPoints { get; set; }
        [DataMember]
        public int? PointPrecision { get; set; }
        [DataMember]
        public string PenUnitConverter { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PredefinedPenKinds PenStyle 
        {
            get
            {
                PredefinedPenKinds _serieType;
                if (Enum.TryParse(serieTypeLineKey, true, out _serieType))
                    return _serieType;
                else
                    return PredefinedPenKinds.seriesLineStyle;
            }
            set
            {
                serieTypeLineKey = value.ToString();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagReference
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(TagReferenceXml))
                        return TagReferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    TagReferenceXml = value.ToXml();
                }
                else
                {
                    TagReferenceXml = string.Empty;
                }
                OnPropertyChanged("tagReference");
            }
        }
        #endregion

        #region Methods

        public Dictionary<string, object> ToDictionary(List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(TagReferenceXml))
                res.Add("SVGReferenceId", CreateUniqueName(title, usedIDs));
            else
                res.Add("SVGReferenceId", null);
            res.Add("StrokeThickness", StrokeThickness);
            res.Add("ArrayIndex", ArrayIndex);
            res.Add("Name", title);
            res.Add("TagName", NodeId);
            res.Add("Range", Range);
            res.Add("UseEURange", UseEURange);
            res.Add("AutoScale", AutoScale);
            res.Add("ShowAxis", ShowAxis);
            res.Add("LColor", $"#{LColor.R.ToString("X2")}{LColor.G.ToString("X2")}{LColor.B.ToString("X2")}");
            res.Add("Visible", Visible);
            res.Add("PenStyle", PenStyle);
            res.Add("PointPrecision", PointPrecision);
            res.Add("UnitConverter", PenUnitConverter);
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

                TagReference = tag;
                NodeId = tag.ResolvedNodeId?.ToString();
                var pos = varName.LastIndexOf("\\") + 1;
                TagName = varName.Substring(pos, varName.Length - pos);
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
    Name = "PenItemList",
    ItemName = "PenItem")]
    public class PenItemList : List<PenItem>
    {
        #region Constructors
        public PenItemList()
        { }

        public PenItemList(IEnumerable<PenItem> collection) :
            base(collection)
        { }

        public PenItemList(PenItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new PenItem(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(list)));
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
            var entity = value as PenItemList;
            if (entity == null)
                return (new PenItemList()).ToDictionary();

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
