// <copyright file="SwicthPreviewControlBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents interface and base implementation for all preview controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class SwicthPreviewControlBase : Control
    {
        #region Constants
        /// <summary>
        /// Presents EMPTY_POINT
        /// </summary>
        private static readonly Point EMPTY_POINT = new Point(0, 0);
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when ItemBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemBorderBrushChanged;
        
        /// <summary>
        /// Event that is raised when CornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;
        
        /// <summary>
        /// Event that is raised when ItemBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemBorderThicknessChanged;
        
        /// <summary>
        /// Event that is raised when SelectedItemBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemBorderBrushChanged;
        
        /// <summary>
        /// Event that is raised when SelectedItemBorderThickness property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemBorderThicknessChanged;
        
        /// <summary>
        /// Event that is raised when SelectedItemBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemBackgroundChanged;
        
        /// <summary>
        /// Event that is raised when ItemCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemCornerRadiusChanged;
        
        /// <summary>
        /// Event that is raised when ItemWidth property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemWidthChanged;
        
        /// <summary>
        /// Event that is raised when ItemHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemHeightChanged;
        
        /// <summary>
        /// Occurs when [collapsed].
        /// </summary>
        public event EventHandler Collapsed;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="SwicthPreviewControlBase"/> class.
        /// </summary>
        static SwicthPreviewControlBase()
        {
            VisibilityProperty.OverrideMetadata(typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisibilityPropertyChanged)));
        }
        #endregion

        #region Abstracts properies
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public abstract object SelectedItem
        {
            get;
        }
        
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public abstract IEnumerable Items
        {
            get;
        }
        
        /// <summary>
        /// Gets a value indicating whether [used main collection].
        /// </summary>
        /// <value>
        /// <c>true</c> if [used main collection]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool UsedMainCollection
        {
            get;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the ItemBorderBrush dependency property.
        /// </summary>
        public Brush ItemBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemBorderBrushProperty);
            }

            set
            {
                SetValue(ItemBorderBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the CornerRadius dependency property.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ItemBorderThickness dependency property.
        /// </summary>
        public Thickness ItemBorderThickness
        {
            get
            {
                return (Thickness)GetValue(ItemBorderThicknessProperty);
            }

            set
            {
                SetValue(ItemBorderThicknessProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the SelectedItemBorderBrush dependency property.
        /// </summary>
        public Brush SelectedItemBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectedItemBorderBrushProperty);
            }

            set
            {
                SetValue(SelectedItemBorderBrushProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the SelectedItemBorderThickness dependency property.
        /// </summary>
        public Thickness SelectedItemBorderThickness
        {
            get
            {
                return (Thickness)GetValue(SelectedItemBorderThicknessProperty);
            }

            set
            {
                SetValue(SelectedItemBorderThicknessProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the SelectedItemBackground dependency property.
        /// </summary>
        public Brush SelectedItemBackground
        {
            get
            {
                return (Brush)GetValue(SelectedItemBackgroundProperty);
            }

            set
            {
                SetValue(SelectedItemBackgroundProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ItemCornerRadius dependency property.
        /// </summary>
        public CornerRadius ItemCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ItemCornerRadiusProperty);
            }

            set
            {
                SetValue(ItemCornerRadiusProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ItemWidth dependency property.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }

            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the ItemHeight dependency property.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }

            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }
        #endregion

        #region Abstracts methods
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        public abstract void AddItem(ContentControl item);
        
        /// <summary>
        /// Clears the items.
        /// </summary>
        public abstract void ClearItems();
        
        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        public abstract void MoveItem(bool isForward, SwitchDirection switchDirection);
        
        /// <summary>
        /// Shows the selected item.
        /// </summary>
        public abstract void ShowSelectedItem();
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the custom brush.
        /// </summary>
        /// <param name="previewBorder">The preview border.</param>
        /// <param name="element">The element.</param>
        internal static void SetCustomBrush(PreviewBorder previewBorder, FrameworkElement element)
        {
            Brush previewBrush = DocumentContainer.GetDocumentPreview(element);

            if (null == previewBrush)
            {
                DocumentContainer container = DocumentContainer.GetDocumentContainer(element);

                if (null != container)
                {
                    if (DocumentContainerMode.TDI == container.Mode)
                    {
                        Rect arrangeRect = new Rect(EMPTY_POINT, container.RenderSize);
                        previewBrush = CreateVisualBrush(element, arrangeRect);
                    }
                    else if (DocumentContainer.IsMinimized(element))
                    {
                        Rect mdiBounds = new Rect(EMPTY_POINT, container.RenderSize);
                        previewBrush = CreateVisualBrush(element, mdiBounds);
                    }
                }
            }

            if (null != previewBrush)
            {
                previewBorder.Background = previewBrush;
            }
        }

        /// <summary>
        /// Raises CornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CornerRadiusChanged != null)
            {
                CornerRadiusChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises ItemBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemBorderBrushChanged != null)
            {
                ItemBorderBrushChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ItemBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemBorderThicknessChanged != null)
            {
                ItemBorderThicknessChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises SelectedItemBorderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemBorderBrushChanged != null)
            {
                SelectedItemBorderBrushChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises SelectedItemBorderThicknessChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemBorderThicknessChanged != null)
            {
                SelectedItemBorderThicknessChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises SelectedItemBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemBackgroundChanged != null)
            {
                SelectedItemBackgroundChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ItemCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemCornerRadiusChanged != null)
            {
                ItemCornerRadiusChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ItemWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemWidthChanged != null)
            {
                ItemWidthChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises ItemHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnItemHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemHeightChanged != null)
            {
                ItemHeightChanged(this, e);
            }
        }
        
        /// <summary>
        /// Called when [visibility property changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnVisibilityPropertyChanged(Visibility newValue)
        {
            if (Visibility.Collapsed == newValue && null != Collapsed)
            {
                Collapsed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Calls OnItemBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnItemBorderBrushChanged(e);
        }
        
        /// <summary>
        /// Calls OnCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnCornerRadiusChanged(e);
        }
        
        /// <summary>
        /// Calls OnItemBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnItemBorderThicknessChanged(e);
        }
        
        /// <summary>
        /// Calls OnSelectedItemBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnSelectedItemBorderBrushChanged(e);
        }
        
        /// <summary>
        /// Calls OnSelectedItemBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnSelectedItemBorderThicknessChanged(e);
        }

        /// <summary>
        /// Calls OnSelectedItemBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnSelectedItemBackgroundChanged(e);
        }
        
        /// <summary>
        /// Calls OnItemCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnItemCornerRadiusChanged(e);
        }
        
        /// <summary>
        /// Calls OnItemWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnItemWidthChanged(e);
        }
        
        /// <summary>
        /// Calls OnItemHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            instance.OnItemHeightChanged(e);
        }
        
        /// <summary>
        /// Creates the visual brush for minimized window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="arrangeRect">The arrange rect.</param>
        /// <returns>VisualBrush element</returns>
        private static VisualBrush CreateVisualBrush(UIElement element, Rect arrangeRect)
        {
            element.Measure(arrangeRect.Size);
            element.Arrange(arrangeRect);

            VisualBrush visualBrush = new VisualBrush
            {
                Visual = element,
                Stretch = Stretch.Uniform
            };

            return visualBrush;
        }
        
        /// <summary>
        /// Called when [visibility property changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            SwicthPreviewControlBase instance = (SwicthPreviewControlBase)d;
            Visibility newValue = (Visibility)args.NewValue;
            instance.OnVisibilityPropertyChanged(newValue);
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies DocumentContainer.ItemBorderBrushProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty ItemBorderBrushProperty = DependencyProperty.Register("ItemBorderBrush", typeof(Brush), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemBorderBrushChanged)));
        
        /// <summary>
        /// Identifies DocumentContainer.CornerRadiusProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Rect"/>
        /// </value>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCornerRadiusChanged)));
        
        /// <summary>
        /// Identifies DocumentContainer.ItemBorderThicknessProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        public static readonly DependencyProperty ItemBorderThicknessProperty = DependencyProperty.Register("ItemBorderThickness", typeof(Thickness), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemBorderThicknessChanged)));
        
        /// <summary>
        /// Identifies DocumentContainer.SelectedItemBorderBrushProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty SelectedItemBorderBrushProperty = DependencyProperty.Register("SelectedItemBorderBrush", typeof(Brush), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedItemBorderBrushChanged)));
        
        /// <summary>
        /// Identifies DocumentContainer.SelectedItemBorderThicknessProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        public static readonly DependencyProperty SelectedItemBorderThicknessProperty = DependencyProperty.Register("SelectedItemBorderThickness", typeof(Thickness), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedItemBorderThicknessChanged)));

        /// <summary>
        /// Identifies DocumentContainer.SelectedItemBackgroundProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty SelectedItemBackgroundProperty = DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedItemBackgroundChanged)));

        /// <summary>
        /// Identifies DocumentContainer.ItemCornerRadiusProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Rect"/>
        /// </value>
        public static readonly DependencyProperty ItemCornerRadiusProperty = DependencyProperty.Register("ItemCornerRadius", typeof(CornerRadius), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemCornerRadiusChanged)));

        /// <summary>
        /// Identifies DocumentContainer.ItemWidthProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemWidthChanged)));
        
        /// <summary>
        /// Identifies DocumentContainer.ItemHeightProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(SwicthPreviewControlBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemHeightChanged)));
        #endregion
    }
}