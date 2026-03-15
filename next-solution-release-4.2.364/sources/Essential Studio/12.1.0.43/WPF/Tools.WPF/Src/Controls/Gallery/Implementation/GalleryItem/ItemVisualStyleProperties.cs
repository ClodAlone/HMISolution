// <copyright file="ItemVisualStyleProperties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// represents the partial class Gallery
    /// </summary>

    public partial class Gallery
    {
        #region Constants
        /// <summary>
        /// Contains default value for some Brush type properties.
        /// </summary>
        private static readonly Brush c_defaultBrushValue = Brushes.Transparent;

        /// <summary>
        /// Contains GroupStandardPanelBrush key name from dictionary.
        /// </summary>
        private const string C_standardPanelBrush = "Luna.GroupStandardPanelBrush";

        /// <summary>
        /// Contains MouseOverBrush key name from dictionary.
        /// </summary>
        private const string C_mouseOverDefault = "Luna.MouseOverBrush.Default";

        /// <summary>
        /// Contains SelectedBrush key name from dictionary.
        /// </summary>
        private const string C_selectedDefault = "Luna.SelectedBrush.Default";

        /// <summary>
        /// Contains SelectedFocusedBrush key name from dictionary.
        /// </summary>
        private const string C_SelectedFocusedDefault = "Luna.SelectedFocusedBrush.Default";

        /// <summary>
        /// Contains ContentBorderBrush key name from dictionary.
        /// </summary>
        private const string C_ContentBorderBrush = "Luna.ContentBorderBrush";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value that represents margins of items in groups. This is dependency properties.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>        
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)GetValue(ItemMarginProperty);
            }

            set
            {
                SetValue(ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents width of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 50.
        /// </value>
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
        /// Gets or sets the value that represents height of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 50.
        /// </value>
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

        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>
        public double ItemMinWidth
        {
            get
            {
                return (double)GetValue(ItemMinWidthProperty);
            }

            set
            {
                SetValue(ItemMinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 100.
        /// </value>
        public double ItemMaxWidth
        {
            get
            {
                return (double)GetValue(ItemMaxWidthProperty);
            }

            set
            {
                SetValue(ItemMaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>
        public double ItemMinHeight
        {
            get
            {
                return (double)GetValue(ItemMinHeightProperty);
            }

            set
            {
                SetValue(ItemMinHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>
        public double ItemMaxHeight
        {
            get
            {
                return (double)GetValue(ItemMaxHeightProperty);
            }

            set
            {
                SetValue(ItemMaxHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the template of content data. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// <para/>        
        /// Default value is null.
        /// </value>        
        public DataTemplate ItemContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemContentTemplateProperty);
            }

            set
            {
                SetValue(ItemContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that provides a way to choose a <see cref="DataTemplate"/> based on the data object 
        /// and the data-bound element.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplateSelector"/>
        /// <para/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplateSelector"/>
        public DataTemplateSelector ItemContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty);
            }

            set
            {
                SetValue(ItemContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets corner radius value of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// Default value is 3.
        /// </value>
        public double ItemCornerRadius
        {
            get
            {
                return (double)GetValue(ItemCornerRadiusProperty);
            }

            set
            {
                SetValue(ItemCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets border thickness value of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 4.
        /// </value>
        public double ItemBorderThickness
        {
            get
            {
                return (double)GetValue(ItemBorderThicknessProperty);
            }

            set
            {
                SetValue(ItemBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border brush of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
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
        /// Gets or sets the value that describes background brush of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush ItemBackground
        {
            get
            {
                return (Brush)GetValue(ItemBackgroundProperty);
            }

            set
            {
                SetValue(ItemBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes foreground brush of items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is black brush.
        /// </value>
        public Brush ItemForeground
        {
            get
            {
                return (Brush)GetValue(ItemForegroundProperty);
            }

            set
            {
                SetValue(ItemForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes background brush of item in group when mouse over that item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush ItemMouseOverBackground
        {
            get
            {
                return (Brush)GetValue(ItemMouseOverBackgroundProperty);
            }

            set
            {
                SetValue(ItemMouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes foreground brush of item in group when mouse over that item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is black brush.
        /// </value>
        public Brush ItemMouseOverForeground
        {
            get
            {
                return (Brush)GetValue(ItemMouseOverForegroundProperty);
            }

            set
            {
                SetValue(ItemMouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes background brush of item in group when it is selected. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush ItemSelectedBackground
        {
            get
            {
                return (Brush)GetValue(ItemSelectedBackgroundProperty);
            }

            set
            {
                SetValue(ItemSelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes foreground brush of item in group when it is selected. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is black brush.
        /// </value>
        public Brush ItemSelectedForeground
        {
            get
            {
                return (Brush)GetValue(ItemSelectedForegroundProperty);
            }

            set
            {
                SetValue(ItemSelectedForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents padding for items in groups. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>        
        /// </value>
        public Thickness ItemPadding
        {
            get
            {
                return (Thickness)GetValue(ItemPaddingProperty);
            }

            set
            {
                SetValue(ItemPaddingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border brush of item in group when mouse over that item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush ItemMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(ItemMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border brush of item in group when it is selected. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush ItemSelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(ItemSelectedBorderBrushProperty);
            }

            set
            {
                SetValue(ItemSelectedBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border brush of item in group when it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush FocusedItemBorderBrush
        {
            get
            {
                return (Brush)GetValue(FocusedItemBorderBrushProperty);
            }

            set
            {
                SetValue(FocusedItemBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes background brush of item in group when it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush FocusedItemBackground
        {
            get
            {
                return (Brush)GetValue(FocusedItemBackgroundProperty);
            }

            set
            {
                SetValue(FocusedItemBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes foreground brush of item in group when it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush FocusedItemForeground
        {
            get
            {
                return (Brush)GetValue(FocusedItemForegroundProperty);
            }

            set
            {
                SetValue(FocusedItemForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border brush of item in group when mouse over that item and it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush FocusedItemMouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(FocusedItemMouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(FocusedItemMouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes background brush of item in group when mouse over that item and it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is transparent brush.
        /// </value>
        public Brush FocusedItemMouseOverBackground
        {
            get
            {
                return (Brush)GetValue(FocusedItemMouseOverBackgroundProperty);
            }

            set
            {
                SetValue(FocusedItemMouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes foreground brush of item in group when mouse over that item and it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>        
        /// Default value is black brush.
        /// </value>
        public Brush FocusedItemMouseOverForeground
        {
            get
            {
                return (Brush)GetValue(FocusedItemMouseOverForegroundProperty);
            }

            set
            {
                SetValue(FocusedItemMouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border thickness of item in group when it is selected. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>
        public double SelectedItemBorderThickness
        {
            get
            {
                return (double)GetValue(SelectedItemBorderThicknessProperty);
            }

            set
            {
                SetValue(SelectedItemBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes border thickness of item in group when it is focused. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>        
        /// Default value is 0.
        /// </value>
        public double FocusedItemBorderThickness
        {
            get
            {
                return (double)GetValue(FocusedItemBorderThicknessProperty);
            }

            set
            {
                SetValue(FocusedItemBorderThicknessProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when template changed.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetBackgroundBrushes();
        }

        /// <summary>
        /// Sets some property values of the default Gallery style.
        /// </summary>
        private void SetBackgroundBrushes()
        {
            //if (ItemBackground == c_defaultBrushValue)
            //{
            //    if (VisualMode == GalleryVisualMode.Detailed)
            //    {
            //        ItemBackground = m_dictionary[C_standardPanelBrush] as Brush;
            //    }
            //    else
            //    {
            //        ItemBackground = m_dictionary[C_ContentBorderBrush] as Brush;
            //    }
            //}

            //if (ItemMouseOverBackground == c_defaultBrushValue)
            //{
            //    ItemMouseOverBackground = m_dictionary[C_mouseOverDefault] as Brush;
            //}

            //if (ItemSelectedBackground == c_defaultBrushValue)
            //{
            //    ItemSelectedBackground = m_dictionary[C_selectedDefault] as Brush;
            //}

            //if (FocusedItemBackground == c_defaultBrushValue)
            //{
            //    FocusedItemBackground = m_dictionary[C_SelectedFocusedDefault] as Brush;
            //}

            //if (FocusedItemMouseOverBackground == c_defaultBrushValue)
            //{
            //    FocusedItemMouseOverBackground = m_dictionary[C_SelectedFocusedDefault] as Brush;
            //}

            //if (GroupMouseOverBackground == c_defaultBrushValue)
            //{
            //    GroupMouseOverBackground = m_dictionary[C_mouseOverDefault] as Brush;
            //}

            //if (GroupSelectedBackground == c_defaultBrushValue)
            //{
            //    GroupSelectedBackground = m_dictionary[C_selectedDefault] as Brush;
            //}
        }

        /// <summary>
        /// Calls OnItemContentTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemContentTemplateChanged(e);
        }

        /// <summary>
        /// Calls OnItemContentTemplateSelectorChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemContentTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Calls OnItemMarginChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMarginChanged(e);
        }

        /// <summary>
        /// Calls OnItemWidthChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemHeightChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemHeightChanged(e);
        }

        /// <summary>
        /// Calls OnItemMinWidthChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMinWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemMaxWidthChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMaxWidthChanged(e);
        }

        /// <summary>
        /// Calls OnItemMinHeightChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMinHeightChanged(e);
        }

        /// <summary>
        /// Calls OnItemMaxHeightChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnItemMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMaxHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// ItemContentTemplateChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemContentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemContentTemplateChanged != null)
            {
                ItemContentTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// ItemContentTemplateSelectorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemContentTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemContentTemplateSelectorChanged != null)
            {
                ItemContentTemplateSelectorChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMarginChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMarginChanged != null)
            {
                ItemMarginChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemWidthChanged != null)
            {
                ItemWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemHeightChanged != null)
            {
                ItemHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMinWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMinWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMinWidthChanged != null)
            {
                ItemMinWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMaxWidthChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMaxWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMaxWidthChanged != null)
            {
                ItemMaxWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMinHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMinHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMinHeightChanged != null)
            {
                ItemMinHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ItemMaxHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnItemMaxHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMaxHeightChanged != null)
            {
                ItemMaxHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="ItemCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemCornerRadiusChanged != null)
            {
                ItemCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemBorderThicknessChanged != null)
            {
                ItemBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemBorderBrushChanged != null)
            {
                ItemBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemBackgroundChanged != null)
            {
                ItemBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnItemForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemForegroundChanged != null)
            {
                ItemForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="ItemMouseOverBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMouseOverBackgroundChanged != null)
            {
                ItemMouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemMouseOverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnItemMouseOverForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemMouseOverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMouseOverForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemMouseOverForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemMouseOverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMouseOverForegroundChanged != null)
            {
                ItemMouseOverForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="ItemSelectedBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemSelectedBackgroundChanged != null)
            {
                ItemSelectedBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemSelectedBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemSelectedBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnItemSelectedForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemSelectedForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemSelectedForegroundChanged != null)
            {
                ItemSelectedForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="ItemPaddingChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemPaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemPaddingChanged != null)
            {
                ItemPaddingChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemPaddingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemPaddingChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemMouseOverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemMouseOverBorderBrushChanged != null)
            {
                ItemMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="ItemSelectedBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnItemSelectedBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemSelectedBorderBrushChanged != null)
            {
                ItemSelectedBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemSelectedBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnItemSelectedBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnItemSelectedBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemBorderBrushChanged != null)
            {
                FocusedItemBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedItemBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemBackgroundChanged != null)
            {
                FocusedItemBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedItemBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnFocusedItemForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemForegroundChanged != null)
            {
                FocusedItemForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="FocusedItemMouseOverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemMouseOverBorderBrushChanged != null)
            {
                FocusedItemMouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedItemMouseOverBorderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemMouseOverBorderBrushChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemMouseOverBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemMouseOverBackgroundChanged != null)
            {
                FocusedItemMouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedItemMouseOverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Calls OnFocusedItemMouseOverForegroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemMouseOverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemMouseOverForegroundChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemMouseOverForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemMouseOverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemMouseOverForegroundChanged != null)
            {
                FocusedItemMouseOverForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnSelectedItemBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="SelectedItemBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemBorderThicknessChanged != null)
            {
                SelectedItemBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedItemBorderThicknessChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFocusedItemBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Gallery instance = (Gallery)d;
            instance.OnFocusedItemBorderThicknessChanged(e);
        }

        /// <summary>
        /// Raises <see cref="FocusedItemBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedItemBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItemBorderThicknessChanged != null)
            {
                FocusedItemBorderThicknessChanged(this, e);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="ItemContentTemplate"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemContentTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemContentTemplateSelector"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemContentTemplateSelectorChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMargin"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMarginChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMinWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMinWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMaxWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMaxWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMinHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMinHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMaxHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMaxHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMouseOverBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMouseOverForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMouseOverForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemSelectedBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemSelectedBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemSelectedForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemSelectedForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemPadding"/> is changed.
        /// </summary>
        public event PropertyChangedCallback ItemPaddingChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemMouseOverBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemSelectedBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemSelectedBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemMouseOverBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemMouseOverBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemMouseOverBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemMouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemMouseOverForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemMouseOverForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedItemBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedItemBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemBorderThicknessChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="ItemMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(1), FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMarginChanged)));

        /// <summary>
        /// Identifies <see cref="ItemWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemWidthChanged)));

        /// <summary>
        /// Identifies <see cref="ItemHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemHeightChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMinWidthProperty =
            DependencyProperty.Register("ItemMinWidth", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMinWidthChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMaxWidthProperty =
            DependencyProperty.Register("ItemMaxWidth", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(Double.MaxValue, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMaxWidthChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMinHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMinHeightProperty =
            DependencyProperty.Register("ItemMinHeight", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMinHeightChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMaxHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMaxHeightProperty =
            DependencyProperty.Register("ItemMaxHeight", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(Double.MaxValue, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnItemMaxHeightChanged)));

        /// <summary>
        /// Identifies <see cref="ItemContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContentTemplateProperty =
            DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContentTemplateChanged)));

        /// <summary>
        /// Identifies <see cref="ItemContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContentTemplateSelectorProperty =
            DependencyProperty.Register("ItemContentTemplateSelector", typeof(DataTemplateSelector), typeof(Gallery), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContentTemplateSelectorChanged)));

        /// <summary>
        /// Identifies <see cref="ItemCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCornerRadiusProperty =
            DependencyProperty.Register("ItemCornerRadius", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(3d, new PropertyChangedCallback(OnItemCornerRadiusChanged)));

        /// <summary>
        /// Identifies <see cref="ItemBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemBorderThicknessProperty =
            DependencyProperty.Register("ItemBorderThickness", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(4d, new PropertyChangedCallback(OnItemBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="ItemBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemBorderBrushProperty =
            DependencyProperty.Register("ItemBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnItemBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="ItemBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnItemBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemForegroundProperty =
            DependencyProperty.Register("ItemForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnItemForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMouseOverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBackgroundProperty =
            DependencyProperty.Register("ItemMouseOverBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnItemMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMouseOverForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverForegroundProperty =
            DependencyProperty.Register("ItemMouseOverForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnItemMouseOverForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemSelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemSelectedBackgroundProperty =
            DependencyProperty.Register("ItemSelectedBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnItemSelectedBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemSelectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemSelectedForegroundProperty =
            DependencyProperty.Register("ItemSelectedForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnItemSelectedForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="ItemPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(Gallery), new FrameworkPropertyMetadata(new Thickness(7.5, 4, 7.5, 4), new PropertyChangedCallback(OnItemPaddingChanged)));

        /// <summary>
        /// Identifies <see cref="ItemMouseOverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMouseOverBorderBrushProperty =
            DependencyProperty.Register("ItemMouseOverBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnItemMouseOverBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="ItemSelectedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemSelectedBorderBrushProperty =
            DependencyProperty.Register("ItemSelectedBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnItemSelectedBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemBorderBrushProperty =
            DependencyProperty.Register("FocusedItemBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGreen, new PropertyChangedCallback(OnFocusedItemBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemBackgroundProperty =
            DependencyProperty.Register("FocusedItemBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnFocusedItemBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemForegroundProperty =
            DependencyProperty.Register("FocusedItemForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnFocusedItemForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemMouseOverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemMouseOverBorderBrushProperty =
            DependencyProperty.Register("FocusedItemMouseOverBorderBrush", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.LightGreen, new PropertyChangedCallback(OnFocusedItemMouseOverBorderBrushChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemMouseOverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemMouseOverBackgroundProperty =
            DependencyProperty.Register("FocusedItemMouseOverBackground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(c_defaultBrushValue, new PropertyChangedCallback(OnFocusedItemMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemMouseOverForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemMouseOverForegroundProperty =
            DependencyProperty.Register("FocusedItemMouseOverForeground", typeof(Brush), typeof(Gallery), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnFocusedItemMouseOverForegroundChanged)));

        /// <summary>
        /// Identifies <see cref="SelectedItemBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemBorderThicknessProperty =
            DependencyProperty.Register("SelectedItemBorderThickness", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(4d, new PropertyChangedCallback(OnSelectedItemBorderThicknessChanged)));

        /// <summary>
        /// Identifies <see cref="FocusedItemBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemBorderThicknessProperty =
            DependencyProperty.Register("FocusedItemBorderThickness", typeof(double), typeof(Gallery), new FrameworkPropertyMetadata(4d, new PropertyChangedCallback(OnFocusedItemBorderThicknessChanged)));
        #endregion
    }
}
