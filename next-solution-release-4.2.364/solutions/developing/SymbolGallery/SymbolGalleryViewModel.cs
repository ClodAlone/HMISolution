using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Windows;
using UFInterfaces;
using Utilities.Converters;

namespace SymbolGallery
{
    [DataContract(Name = "SymbolGallerySettings", Namespace = Namespaces.UriProgea)]
    public class SymbolGalleryViewModel : INotifyPropertyChanged
    {
        #region Enumarators
        [TypeConverter(typeof(LocalizedEnumConverter))]
        public enum VisualizationMode
        {
            Carousel,
            Grid
        }
        #endregion

        #region Constructors
        public SymbolGalleryViewModel(VisualizationMode mode)
        {
            _SymbolsDisplayMode = mode;
        }
        #endregion

        #region Properties
        [DataMember]
        VisualizationMode _SymbolsDisplayMode; 
        public VisualizationMode SymbolsDisplayMode 
        { 
            get
            {
                return _SymbolsDisplayMode;
            }

            internal set
            { 
                if (_SymbolsDisplayMode != value)
                {
                    _SymbolsDisplayMode = value;
                    OnPropertyChanged("SymbolsDisplayMode");
                    OnPropertyChanged("IsCarouselMode");
                    OnPropertyChanged("IsGridMode");
                }
            }
        }

        public bool IsCarouselMode 
        { 
            get 
            { 
                return SymbolsDisplayMode == SymbolGalleryViewModel.VisualizationMode.Carousel; 
            }
            set
            {
                if (IsCarouselMode != value)
                    SymbolsDisplayMode = value ? SymbolGalleryViewModel.VisualizationMode.Carousel : SymbolGalleryViewModel.VisualizationMode.Grid;
            }
        }
        
        public bool IsGridMode 
        { 
            get 
            { 
                return SymbolsDisplayMode == SymbolGalleryViewModel.VisualizationMode.Grid; 
            }
            set
            {
                if (IsGridMode != value)
                    SymbolsDisplayMode = value ? SymbolGalleryViewModel.VisualizationMode.Grid : SymbolGalleryViewModel.VisualizationMode.Carousel;
            }
        }

        [DataMember]
        double _ZoomLevel = 50;
        public double ZoomLevel
        {
            get
            {
                return _ZoomLevel;
            }
            set
            {
                if (_ZoomLevel != value)
                {
                    _ZoomLevel = value;
                    OnPropertyChanged("ZoomLevel");
                }
            }
        }

        [DataMember]
        String textSearch;
        public String TextSearch
        {
            get
            {
                return textSearch;
            }
            set
            {
                if (textSearch != value)
                {
                    textSearch = value;
                    OnPropertyChanged("TextSearch");
                }
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
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        #endregion
    }
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }

}
