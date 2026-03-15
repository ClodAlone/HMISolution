using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;
using Utilities;
using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace DBControls
{
    [SvgValueConverter(typeof(ConvertImageThresholdCollection))]
    [DataContract(Name = "ImageThresholdCollection")]
    public class ImageThresholdCollection : ObservableCollection<ImageThreshold>
    {
        public ImageThresholdCollection()
        {

        }
        public ImageThresholdCollection(ObservableCollection<ImageThreshold> collection) : base(collection)
        {
            
        }
        public ImageThresholdCollection(List<ImageThreshold> list) : base(list)
        {

        }
        public List<ImageThreshold> this[string colName]
        {
            get
            {
                return (from ImageThreshold thr in this.AsParallel() where thr.ColumnName == colName select thr).ToList();
            }
        }

        public string ToXml()
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    writer.WriteStartElement("ImageThresholdCollection");
                    string _value = XamlWriter.Save(this);
                    valueSerializer.Serialize(writer, _value);
                    writer.WriteEndElement();
                }
                return sw.ToString();
            }
        }

        static public ImageThresholdCollection FromXml(string xml)
        {
            object value;
            using (var sw = new System.IO.StringReader(xml))
            {
                using (var reader = XmlReader.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    try
                    {
                        reader.ReadStartElement("ImageThresholdCollection");
                        string svalue = (string)valueSerializer.Deserialize(reader);
                        value = XamlReader.Parse(svalue);
                        reader.ReadEndElement();
                    }
                    catch
                    {
                        return null;
                    }
                }
                return value as ImageThresholdCollection;
            }
        }

        public List<object> ToDictionary(object document)
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(list, document)));
            return res;
        }
    }

    [DataContract(Name = "ImageThreshold")]
    public class ImageThreshold : INotifyPropertyChanged
    {
        #region Public Props
        
        Uri imgPath;
        //"Value" property will be populated with image path by SourceFilePropertyEditor        
        [DataMember]
        public Uri Value
        {
            get
            {
                return imgPath;
            }
            set
            {
                if (value != imgPath)
                {
                    imgPath = value;
                    OnPropertyChanged("Value");
                }
            }
        }
        string imgThresholdVal = "";
        [DataMember]
        public string ImageThresholdValue
        {
            get
            {
                return imgThresholdVal;
            }
            set
            {
                if (value != imgThresholdVal)
                {
                    imgThresholdVal = value;
                    OnPropertyChanged("ImageThresholdValue");
                }
            }
        }
        string colName;
        [DataMember]
        public string ColumnName
        {
            get
            {
                return colName;
            }
            set
            {
                if (value != colName)
                {
                    colName = value;
                    OnPropertyChanged("ColumnName");
                }
            }
        }
        CaptionAlignmentEnum imageCaption;
        [DataMember]
        public CaptionAlignmentEnum ImageCaption
        {
            get { return imageCaption; }
            set
            {
                if (imageCaption == value)
                    return;
                imageCaption = value;
                OnPropertyChanged("ImageCaption");
            }
        }
        #endregion

        public Dictionary<string, object> ToDictionary(List<string> usedIDs, object document)
        {
            string _value = Value.GetPathString();
            if (!Value.IsAbsoluteUri && document is IDocument)
            {
                _value = $"images\\{DocumentHelper.GetRootParent(document as IDocument, false).Title}\\{_value}";
            }
            
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Value", _value);
            res.Add("ImageThresholdValue", ImageThresholdValue);
            res.Add("ColumnName", ColumnName);
            res.Add("ImageCaption", ImageCaption);
            return res;
        }


        public ImageThreshold()
        {

        }

        public ImageThreshold(string colName)
        {
            ColumnName = colName;
        }

        public ImageThreshold(ImageThreshold baseThres)
        {

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

    internal class ConvertImageThresholdCollection : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            var entity = value as ImageThresholdCollection;
            if (entity == null)
                return (new ImageThresholdCollection()).ToDictionary(document);

            return entity.ToDictionary(document);
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
