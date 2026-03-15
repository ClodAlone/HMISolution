using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Markup;
using System.Collections.Generic;
using Utilities;

namespace StatesChartControl
{
    [SvgValueConverter(typeof(ConvertValueItemList))]
    public class ValueItemList : ObservableCollection<ValueItem>
    {
        private bool validStatus = true;

        #region Constructors
        public ValueItemList()
        { }

        public ValueItemList(ObservableCollection<ValueItem> collection)
            : base(collection)
        { }

        public ValueItemList(List<ValueItem> itemList)
            : base(itemList)
        { }

        public ValueItemList(ValueItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new ValueItem(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary()));
            return res;
        }

        public bool CheckDuplicateItemsValue(string value)
        {
            return this.AsParallel().Where(x => x.Value == value).Count() > 1;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ValidStatus
        {
            get
            {
                return validStatus;
            }
            set
            {
                if (value != validStatus)
                    validStatus = value;
            }
        }

        public bool CheckDuplicateItems()
        {
            return this.AsParallel().GroupBy(x => x.Value).Where(g => g.Count() > 1).FirstOrDefault() != null;
        }

        public string ToXml()
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    writer.WriteStartElement("ValueItemList");
                    string _value = XamlWriter.Save(this);
                    valueSerializer.Serialize(writer, _value);
                    writer.WriteEndElement();
                }
                return sw.ToString();
            }
        }
        static public ValueItemList FromXml(string xml)
        {
            object value;
            using (var sw = new System.IO.StringReader(xml))
            {
                using (var reader = XmlReader.Create(sw))
                {
                    XmlSerializer valueSerializer = new XmlSerializer(typeof(String));
                    try
                    {
                        reader.ReadStartElement("ValueItemList");
                        string svalue = (string)valueSerializer.Deserialize(reader);
                        value = XamlReader.Parse(svalue);
                        reader.ReadEndElement();
                    }
                    catch
                    {
                        return null;
                    }
                }
                return value as ValueItemList;
            }
        }
    }

    [Serializable]
    public partial class ValueItem : INotifyPropertyChanged
    {
        public System.Windows.Media.Brush _brush = SmartControlUtilities.TagColorHelper.RandomBrush();
        public string _value, _label, _guiid, _translatedLabel;
        //TagBrushPair _value;

        #region Constructors
        public ValueItem()
        {
            _brush = SmartControlUtilities.TagColorHelper.RandomBrush();
            _label = this._value = String.Empty;
            _guiid = Guid.NewGuid().ToString();
        }

        public ValueItem(ValueItem instance)
        {
            if (instance == null)
                return;

            ControlBackground = instance.ControlBackground?.Clone();
            Label = instance.Label;
            TranslatedLabel = instance.TranslatedLabel;
            Value = instance.Value;
            ControlBackground = instance.ControlBackground;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Label", Label);
            res.Add("TranslatedLabel", TranslatedLabel);
            res.Add("Value", Value);
            res.Add("dValue", dValue);
            res.Add("ControlBackground", ControlBackground);
            return res;
        }
        #endregion

        #region DP

        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (_label == value)
                    return;
                _label = value;
                OnPropertyChanged("Label");
            }
        }
        public string TranslatedLabel
        {
            get
            {
                if (String.IsNullOrEmpty(_translatedLabel))
                    _translatedLabel = Label;
                return _translatedLabel;
            }
            set
            {
                if (_translatedLabel == value)
                    return;
                _translatedLabel = value;
                OnPropertyChanged("TranslatedLabel");
            }
        }
        public string Value
        {
            get {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        public double dValue
        {
            get
            {
                double dv;
                double.TryParse(Value, out dv);
                return dv;
            }
        }
        public Brush ControlBackground
        {
            get { return _brush; }
            set
            {
                if (_brush == value)
                    return;
                _brush = value;
                OnPropertyChanged("ControlBackground");
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

    internal class ConvertValueItemList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            var entity = value as ValueItemList;
            if (entity == null)
                return (new ValueItemList()).ToDictionary();

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
