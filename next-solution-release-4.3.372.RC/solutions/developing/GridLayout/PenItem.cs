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

namespace Trends
{
    public class PenItem : INotifyPropertyChanged
    {
        #region Private Members;
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
        #endregion
        #region Public Properties
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

        public PredefinedPenKinds PenStyle
        {
            get { return penstyle; }
            set
            {
                if (penstyle == value)
                    return;
                penstyle = value;
                OnPropertyChanged("PenStyle");
            }
        }
        #endregion

        public PenItem Clone()
        {
            return new PenItem()
            {
                arrayindex = arrayindex,
                name = name,
                tagname = tagname,
                range = range,
                useeurange = useeurange,
                lcolor = lcolor,
                visible = visible,
                tagreferenceXml = tagreferenceXml,
                strokethickness = strokethickness,
                dlrsource = dlrsource,
                historicalName = historicalName,
                columnName = columnName,
                penstyle = penstyle
            };
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
