// <copyright file="DocumentContainer_VS200XCase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Events
        /// <summary>
        /// Event that is raised when ToolWindowsList property is changed.
        /// </summary>
        public event PropertyChangedCallback ToolWindowsListChanged;
        
        /// <summary>
        /// Event that is raised when ToolWindowsListHeader property is changed.
        /// </summary>
        public event PropertyChangedCallback ToolWindowsListHeaderChanged;
        
        /// <summary>
        /// Event that is raised when ToolWindowsListHeaderTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback ToolWindowsListHeaderTemplateChanged;
        
        /// <summary>
        /// Event that is raised when DocumentListHeader property is changed.
        /// </summary>
        public event PropertyChangedCallback DocumentListHeaderChanged;
        
        /// <summary>
        /// Event that is raised when DocumentListHeaderTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback DocumentListHeaderTemplateChanged;
        
        /// <summary>
        /// Occurs when Tool windows item selected.
        /// </summary>
        public event EventHandler ToolWindowsItemSelected;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the ToolWindowsList dependency property.
        /// </summary>
        public ObservableFrameworkElements ToolWindowsList
        {
            get
            {
                return (ObservableFrameworkElements)GetValue(ToolWindowsListProperty);
            }

            set
            {
                SetValue(ToolWindowsListProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ToolWindowsListHeader dependency property.
        /// </summary>
        public object ToolWindowsListHeader
        {
            get
            {
                return GetValue(ToolWindowsListHeaderProperty);
            }

            set
            {
                SetValue(ToolWindowsListHeaderProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ToolWindowsListHeaderTemplate dependency property.
        /// </summary>
        public DataTemplate ToolWindowsListHeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ToolWindowsListHeaderTemplateProperty);
            }

            set
            {
                SetValue(ToolWindowsListHeaderTemplateProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the DocumentListHeader dependency property.
        /// </summary>
        public object DocumentListHeader
        {
            get
            {
                return GetValue(DocumentListHeaderProperty);
            }

            set
            {
                SetValue(DocumentListHeaderProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the DocumentListHeaderTemplate dependency property.
        /// </summary>
        public DataTemplate DocumentListHeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DocumentListHeaderTemplateProperty);
            }

            set
            {
                SetValue(DocumentListHeaderTemplateProperty, value);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when [tool windows item selected].
        /// </summary>
        /// <param name="item">The item OnToolWindowsItemSelected.</param>
        public virtual void OnToolWindowsItemSelected(object item)
        {
            if (null != ToolWindowsItemSelected)
            {
                ToolWindowsItemSelected(item, EventArgs.Empty);
            }

            if (FlipParent != null)
            {
                FlipParent.SelectItem(item);
            }
        }

        /// <summary>
        /// Tools the window item selected close preview.
        /// </summary>
        /// <param name="item">The item ToolWindowItemSelectedClosePreview.</param>
        private void ToolWindowItemSelectedClosePreview(object item)
        {
            ClosePreview();
            OnToolWindowsItemSelected(item);
        }

        /// <summary>
        /// Gets the document description.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>object value type </returns>
        public static object GetDocumentDescription(DependencyObject obj)
        {
            return obj.GetValue(DocumentDescriptionProperty);
        }
        
        /// <summary>
        /// Sets the document description.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentDescription(DependencyObject obj, object value)
        {
            obj.SetValue(DocumentDescriptionProperty, value);
        }
        
        /// <summary>
        /// Gets the document description template.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>DataTemplate value</returns>
        public static DataTemplate GetDocumentDescriptionTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DocumentDescriptionTemplateProperty);
        }
        
        /// <summary>
        /// Sets the document description template.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentDescriptionTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DocumentDescriptionTemplateProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates property value cache and raises ToolWindowsListChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnToolWindowsListChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ToolWindowsListChanged != null)
            {
                ToolWindowsListChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ToolWindowsListHeaderChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnToolWindowsListHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ToolWindowsListHeaderChanged != null)
            {
                ToolWindowsListHeaderChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ToolWindowsListHeaderTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnToolWindowsListHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ToolWindowsListHeaderTemplateChanged != null)
            {
                ToolWindowsListHeaderTemplateChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises DocumentListHeaderChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDocumentListHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DocumentListHeaderChanged != null)
            {
                DocumentListHeaderChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises DocumentListHeaderTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDocumentListHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DocumentListHeaderTemplateChanged != null)
            {
                DocumentListHeaderTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnToolWindowsListChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnToolWindowsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnToolWindowsListChanged(e);
        }
        
        /// <summary>
        /// Calls OnToolWindowsListHeaderChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnToolWindowsListHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnToolWindowsListHeaderChanged(e);
        }
        
        /// <summary>
        /// Calls OnToolWindowsListHeaderTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnToolWindowsListHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnToolWindowsListHeaderTemplateChanged(e);
        }
        
        /// <summary>
        /// Calls OnDocumentListHeaderChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDocumentListHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnDocumentListHeaderChanged(e);
        }
        
        /// <summary>
        /// Calls OnDocumentListHeaderTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDocumentListHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnDocumentListHeaderTemplateChanged(e);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// This property present tool window list. This list doesn't dispose in DocumentContainer.
        /// It is implemented for VS2005 (VS2008?) mode and you will be able add own realization
        /// , if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty ToolWindowsListProperty = DependencyProperty.Register("ToolWindowsList", typeof(ObservableFrameworkElements), typeof(DocumentContainer), new FrameworkPropertyMetadata(new ObservableFrameworkElements(), new PropertyChangedCallback(OnToolWindowsListChanged)));
        
        /// <summary>
        /// This property present tool window list header. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty ToolWindowsListHeaderProperty = DependencyProperty.Register("ToolWindowsListHeader", typeof(object), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnToolWindowsListHeaderChanged)));
        
        /// <summary>
        /// This property present tool window list header template. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty ToolWindowsListHeaderTemplateProperty = DependencyProperty.Register("ToolWindowsListHeaderTemplate", typeof(DataTemplate), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnToolWindowsListHeaderTemplateChanged)));
        
        /// <summary>
        /// This property present list header. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty DocumentListHeaderProperty = DependencyProperty.Register("DocumentListHeader", typeof(object), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentListHeaderChanged)));
        
        /// <summary>
        /// This property present list header template. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty DocumentListHeaderTemplateProperty = DependencyProperty.Register("DocumentListHeaderTemplate", typeof(DataTemplate), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentListHeaderTemplateChanged)));
        
        /// <summary>
        /// /// This property present document description. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty DocumentDescriptionProperty = DependencyProperty.RegisterAttached("DocumentDescription", typeof(object), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        
        /// <summary>
        /// This property present document description template. It is implemented for VS2005 (VS2008?) mode
        /// and you will be able add own realization, if you will add own mode.
        /// </summary>
        public static readonly DependencyProperty DocumentDescriptionTemplateProperty = DependencyProperty.RegisterAttached("DocumentDescriptionTemplate", typeof(DataTemplate), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        #endregion
    }
}
