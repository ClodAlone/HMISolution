using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using UFUAEditor.ComponentService;
using Utilities;
using WPFPenHelpers;

namespace StatesChartControl
{
    [Serializable]
    public class PenList : WPFPenHelpers.SerieData, INotifyPropertyChanged
    {
        #region Constructors
        public PenList()
        { }

        public PenList(PenList instance):
            base(instance)
        {
            if (instance == null)
                return;

            NodeId = instance.NodeId;
            Value = instance.Value;
            Text = instance.Text;
            Text = instance.Text;
            FirstBackground = instance.FirstBackground?.Clone();
            Foreground = instance.Foreground?.Clone();
            Valueitem = new ValueItem(instance.Valueitem);
            Valuelist = new ValueItemList(instance.Valuelist);
            TagName = instance.TagName;
            Dlrsource = instance.Dlrsource;
            HistoricalDlrName = instance.HistoricalDlrName;
            ColuName = instance.ColuName;
            TagReferenceXml = instance.TagReferenceXml;
            //TranslatedTitle = instance.TranslatedTitle;
            ValueNotFoundBackground = instance.ValueNotFoundBackground?.Clone();
        }

        public Dictionary<string, object> ToDictionary(List<string> usedIDs, object document, IUFUAEditorManager uFUAEditorManager)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(tagreferenceXml))
                res.Add("SVGReferenceId", CreateUniqueName(title, usedIDs));
            else
                res.Add("SVGReferenceId", null);
            res.Add("Title", title);
            res.Add("Value", Value);
            res.Add("Text", Text);
            res.Add("FirstBackground", FirstBackground);
            res.Add("Foreground", Foreground);
            res.Add("Valueitem", Valueitem?.ToDictionary());
            res.Add("Valuelist", Valuelist?.ToDictionary());
            res.Add("Dlrsource", Dlrsource);

            var tag = tagReference;
            if (tag.HasValidValue)
            {
                TagName = GetRelativePath($"{tag.RelativePath}");
                try
                {
                    if (!Dlrsource)
                    {
                        if (uFUAEditorManager != null && tag.ResolvedNodeId != null)
                        {
                            var doc = uFUAEditorManager.GetProjectDocument((document as IDocument).Parent, tag.AppName, true) ?? document;
                            if (doc != null)
                            {
                                HistoricalDlrName = uFUAEditorManager.GetHistorianName(doc as IDocument, tag.ResolvedNodeId);
                            }
                        }
                        ColuName = TagName;
                    }
                    NodeId = tagReference.ResolvedNodeId.ToString();
                }
                catch
                {
                }
            }

            res.Add("NodeId", NodeId);
            res.Add("TagName", TagName);
            res.Add("HistoricalDlrName", HistoricalDlrName);
            res.Add("ColuName", ColuName);
            res.Add("ValueNotFoundBackground", ValueNotFoundBackground);
            return res;
        }

        internal string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                string relative = string.Format("{0}", (value).Replace(oldChars, ""));
                return relative;
            }

            return String.Empty;
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
        #endregion

        #region Private Members
        private double threshold;
        private String text;
        private Brush background=Brushes.Transparent;
        private Brush foreground;
        private ValueItem valueitem;
        private ValueItemList valuelist;
        private String columnName;
        #endregion
        
        #region Public Properties

        public String NodeId
        {
            get
            {
                if (string.IsNullOrEmpty(guiId))
                    guiId = Guid.NewGuid().ToString();
                return guiId;
            }
            set
            {
                if (guiId == value)
                    return;
                guiId = value;
                OnPropertyChanged("NodeId");
            }
        }
        //public int? ArrayIndex
        //{
        //    get { return arrayIndex.HasValue ? arrayIndex : null; }
        //    set
        //    {
        //        if (arrayIndex == value)
        //            return;
        //        arrayIndex = value;
        //        OnPropertyChanged("ArrayIndex");
        //    }
        //}
        public double Value
        {
            get { return threshold; }
            set
            {
                if (threshold == value)
                    return;
                threshold = value;
                OnPropertyChanged("Value");
            }
        }
        public String Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                text = value;
                OnPropertyChanged("Text");
            }
        }
        public Brush FirstBackground
        {
            get { return background; }
            set
            {
                if (background == value)
                    return;
                background = value;
                OnPropertyChanged("FirstBackground");
            }
        }
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (foreground == value)
                    return;
                foreground = value;
                OnPropertyChanged("Foreground");
            }
        }
        public ValueItem Valueitem
        {
            get { return valueitem; }
            set
            {
                if (valueitem == value)
                    return;
                valueitem = value;
                OnPropertyChanged("ValueItem");
            }
        }
        public ValueItemList Valuelist
        {
            get
            {
                if (valuelist == null)
                    valuelist = new ValueItemList();
                return valuelist;
            }
            set
            {
                if (valuelist == value)
                    return;
                valuelist = value;
                OnPropertyChanged("Valuelist");
            }
        }
        public String TagName
        {
            get { return tagName; }
            set
            {
                if (tagName == value)
                    return;
                tagName = value;
                OnPropertyChanged("TagName");
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
        public String HistoricalDlrName
        {
            get { return historicalName; }
            set
            {
                if (historicalName == value)
                    return;
                historicalName = value;
                OnPropertyChanged("HistoricalDlrName");
            }
        }
        public String ColuName
        {
            get 
            {
                if (dlrsource)
                    return columnName;
                else
                {
                    var tag = tagReference;
                    if (tag != null && tag.HasValidValue)
                        return GetRelativePath($"{tag.RelativePath}");
                }

                return columnName; 
            }
            set
            {
                if (columnName == value)
                    return;
                columnName = value;
                OnPropertyChanged("ColuName");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible == value)
                    return;
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use tagreferenceXml instead of this")]
        public String TagReferenceXml
        {
            get
            {
                return tagreferenceXml;
            }
            set
            {
                tagreferenceXml = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use tagreferenceXml instead of this")]
        public OPCUAEntityReference TagReference
        {
            get
            {
                return tagReference;
            }
            set
            {
                tagReference = value;
            }
        }

        string translatedTitle;
        public String TranslatedTitle
        {
            get
            {
                if (translatedTitle == null)
                    translatedTitle = title;
                return translatedTitle;
            }
            set
            {
                if (translatedTitle == value)
                    return;
                translatedTitle = value;
                OnPropertyChanged("TranslatedTitle");
            }
        }
        
        private Brush _valueNotFoundBackground;
        public Brush ValueNotFoundBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                if (_valueNotFoundBackground == null)
                    _valueNotFoundBackground = SmartControlUtilities.TagColorHelper.RandomBrush();
                return _valueNotFoundBackground;
            }
            set
            {
                if (_valueNotFoundBackground == value)
                    return;
                _valueNotFoundBackground = value;
                OnPropertyChanged("ValueNotFoundBackground");
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

}