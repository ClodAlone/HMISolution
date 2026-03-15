using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using OPCUAViewModel;
using Utilities;

namespace ReportParameters
{
    public class Parameter : INotifyPropertyChanged, IDisposable
    {
        #region Properties

        String name; 
        ///// <summary>
        ///// Parameter name.
        ///// </summary>
        public String Name 
        {
            get 
            {
                return name;
            }
            set
            {
                if (name == value)
                    return;

                name = value;
                OnPropertyChanged("Name");
            }
        }

        ParameterType type;
        /// <summary>
        /// Parameter type.
        /// </summary>
        public ParameterType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type == value)
                    return;

                type = value;
                OnPropertyChanged("Type");
            }
        }

        Object value;
        /// <summary>
        /// Parameter value.
        /// </summary>
        public Object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value == value)
                    return;

                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        String tagRefXml;
        /// <summary>
        /// XML string from serialization of the OPC UA entity reference parameter.
        /// </summary>
        [IgnoreDataMember]
        public String TagRefXml
        {
            get
            {
                return tagRefXml;
            }
            set
            {
                if (tagRefXml == value)
                    return;

                tagRefXml = value;
                

                if (!String.IsNullOrEmpty(tagRefXml))
                {
                    try
                    {
                        tagRef = tagRefXml.FromXml<OPCUAEntityReference>();
                    }
                    catch 
                    {
                        tagRef = null;
                    }
                }
                else
                    tagRef = null;

                OnPropertyChanged("TagRef");
                OnPropertyChanged("TagRefXml");
            }
        }

        OPCUAEntityReference tagRef;
        /// <summary>
        /// Parameter OPC UA entity reference.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagRef
        {
            get
            {
                return tagRef;
            }
            set
            {
                tagRef = value;
                if (tagRef != null)
                {
                    tagRefXml = tagRef.ToXml();
                    stringRef = tagRef.StringRepresentation;
                    OnPropertyChanged("StringRef");
                }
                else
                    tagRefXml = null;

                OnPropertyChanged("TagRef");
                OnPropertyChanged("TagRefXml");
            }
        }
        bool loading = false;
        [IgnoreDataMember]
        public bool Loading
        {
            get
            {
                return loading;
            }
            set
            {
                if (loading != value)
                {
                    loading = value;
                    OnPropertyChanged("Loading");
                }
            }
        }
        string stringRef;
        public string StringRef
        {
            get
            {
                if (stringRef == null && tagRef != null)
                    stringRef = tagRef.StringRepresentation;
                return stringRef;
            }
            set
            {
                if (stringRef != value)
                {
                    stringRef = value;
                    OnPropertyChanged("StringRef");
                }
            }
        }
        string guiId { get; set; }
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int? SVGReferenceId { get; set; }
        #endregion

        #region Methods

        public Type ToSystemType()
        {
            if (Type == ParameterType.Boolean)
                return typeof(Boolean);
            else if (Type == ParameterType.DateTime)
                return typeof(DateTime);
            else if (Type == ParameterType.Decimal)
                return typeof(Decimal);
            else if (Type == ParameterType.Integer)
                return typeof(UInt64);
            else 
                return typeof(String);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Value != null && Value is IDisposable)
                (Value as IDisposable).Dispose();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
