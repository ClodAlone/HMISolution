using System.Windows.Media;
using System.Linq;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utilities;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
namespace Converters
{
    //[global::System.ComponentModel.TypeConverter(typeof(FontSettingsConverter))]
    [DataContract(Name = "FontSettings", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    [KnownType(typeof(FontWeight))]
    [KnownType(typeof(FontStyle))]
    [KnownType(typeof(FontFamily))]
    [KnownType(typeof(int))]
    
    [Serializable]
    public partial class FontSettings : ICloneable, INotifyPropertyChanged, IDataErrorInfo
    {
        #region Private Members;
        private FontFamily _fontFamily;
        private FontStyle _fontStyle;
        private FontWeight _fontWeight;
        private int _fontSize;
        private String _fontFamilyXml;
        private String _fontStyleXml;
        private String _fontWeightXml;
        private FontFamilyConverter ffc = new FontFamilyConverter();
        private FontStyleConverter fsc = new FontStyleConverter();
        private FontWeightConverter fwc = new FontWeightConverter();
        #endregion
        //private string StandardChar = "@";
        public FontSettings(FontWeight w, FontStyle st, FontFamily f, int s)
        {
            FontWeight = w;
            FontStyle = st;
            FontFamily = f;
            FontSize = s;
        }
        public FontSettings(FontSettings f)
            :base()
        {
            if (f == null)
                return;
            FontWeight = f.FontWeight;
            FontStyle = f.FontStyle;
            FontFamily = f.FontFamily;
            FontSize = f.FontSize;
        }
        public FontSettings()
        {
            FontWeight = FontWeights.Light;
            FontStyle = FontStyles.Normal;
            FontFamily = new FontFamily("Segoe UI");
            FontSize = 12;
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FontFamily FontFamily
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(_fontFamilyXml))
                    {
                        if (ffc == null)
                            ffc = new FontFamilyConverter();
                        return (FontFamily)ffc.ConvertFromString(_fontFamilyXml.FromXml<String>());
                    }
                }
                catch
                { }

                return new FontFamily("Segoe UI");;
            }
            set
            {
                try
                {
                    if (ffc == null)
                        ffc = new FontFamilyConverter();
                    FontFamilyXml = ffc.ConvertToInvariantString(value).ToXml();
                    OnPropertyChanged("FontFamily");
                }
                catch
                { }
            }
        }
        [DataMember]
        public String FontFamilyXml
        {
            get { return _fontFamilyXml; }
            set
            {
                if (_fontFamilyXml == value)
                    return;
                _fontFamilyXml = value;
                OnPropertyChanged("FontFamilyXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FontStyle FontStyle
        {
            get
            {
                try
                {
                    if (fsc == null)
                        fsc = new FontStyleConverter();
                    if (!String.IsNullOrEmpty(_fontStyleXml))
                        return (FontStyle)fsc.ConvertFromString(_fontStyleXml.FromXml<String>());
                }
                catch
                { }

                return FontStyles.Normal;
            }
            set
            {
                try
                {
                    if (fsc == null)
                        fsc = new FontStyleConverter();
                    FontStyleXml = fsc.ConvertToInvariantString(value).ToXml();
                    OnPropertyChanged("FontStyle");
                }
                catch
                { }
            }
        }
        [DataMember]
        public String FontStyleXml
        {
            get { return _fontStyleXml; }
            set
            {
                if (_fontStyleXml == value)
                    return;
                _fontStyleXml = value;
                OnPropertyChanged("FontStyleXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FontWeight FontWeight
        {
            get
            {
                try
                {
                    if (fwc == null)
                        fwc = new FontWeightConverter();
                    if (!String.IsNullOrEmpty(_fontWeightXml))
                        return (FontWeight)fwc.ConvertFromString(_fontWeightXml.FromXml<String>());
                }
                catch
                { }

                return FontWeights.Normal;
            }
            set
            {
                try
                {
                    if (fwc == null)
                        fwc = new FontWeightConverter();
                    FontWeightXml = fwc.ConvertToInvariantString(value).ToXml();
                    OnPropertyChanged("FontWeight");
                }
                catch
                { }
            }
        }
        [DataMember]
        public String FontWeightXml
        {
            get { return _fontWeightXml; }
            set
            {
                if (_fontWeightXml == value)
                    return;
                _fontWeightXml = value;
                OnPropertyChanged("FontWeightXml");
            }
        }
        [DataMember]
        public int FontSize
        {
            get
            {
                if (_fontSize < 0)
                    return 0; 
                if (_fontSize > Int16.MaxValue)
                    return Int16.MaxValue;
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }
        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion
        #region Private Methods
        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "FontSize")
            {
                if (_fontSize < 0 || _fontSize > Int16.MaxValue)
                {
                    return string.Format(Properties.Resources.ParameterValueError,0, Int16.MaxValue);
                }
            }

            return null;
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


        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("FontFamily", FontFamily.ToString());
            res.Add("FontSize", FontSize);
            res.Add("FontStyle", FontStyle.ToString());
            res.Add("FontWeight", FontWeight.ToString());
            return res;
        }

        public FontSettings Clone()
        {
            return (FontSettings)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }

    public class ConvertFontSettings : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            var entity = value as FontSettings;
            if (entity == null)
                return (new FontSettings()).ToDictionary();

            return entity.ToDictionary();
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
}
