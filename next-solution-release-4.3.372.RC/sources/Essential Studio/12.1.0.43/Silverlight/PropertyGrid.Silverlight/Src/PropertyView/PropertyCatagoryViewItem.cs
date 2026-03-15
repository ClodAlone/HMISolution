#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;

namespace Syncfusion.Windows.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(Name = "Collapsed", GroupName = "ItemStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "ItemStates")]
    public class PropertyCatagoryViewItem : HeaderedItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        internal PropertyView PropertyView;

        #region properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PropertyCatagoryViewItem), new PropertyMetadata(true, OnIsExpandedChanged));

        /// <summary>
        /// Gets or sets the text color used for category headings.
        /// </summary>
        /// <value>The category foreground.</value>
        [Category("Appearence")]
        public Brush CategoryForeground
        {
            get { return (Brush)GetValue(CategoryForegroundProperty); }
            set { SetValue(CategoryForegroundProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CategoryForegroundProperty =
            DependencyProperty.Register("CategoryForeground", typeof(Brush), typeof(PropertyCatagoryViewItem), new PropertyMetadata(OnCategoryForegroundChanged));

        /// <summary>
        /// Gets or sets the color of the borders and category heading background.
        /// </summary>
        /// <value>The color of the line.</value>
        [Category("Appearance")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(PropertyCatagoryViewItem), new PropertyMetadata(OnLineColorChanged));


        /// <summary>
        /// Gets or sets the editable background.
        /// </summary>
        /// <value>The editable background.</value>
        public Brush EditableBackground
        {
            get { return (Brush)GetValue(EditableBackgroundProperty); }
            set { SetValue(EditableBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditableBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditableBackgroundProperty =
            DependencyProperty.Register("EditableBackground", typeof(Brush), typeof(PropertyCatagoryViewItem), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the editable font weight.
        /// </summary>
        /// <value>The editable font weight.</value>
        public FontWeight EditableFontWeight
        {
            get { return (FontWeight)GetValue(EditableFontWeightProperty); }
            set { SetValue(EditableFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditableFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditableFontWeightProperty =
            DependencyProperty.Register("EditableFontWeight", typeof(FontWeight), typeof(PropertyCatagoryViewItem), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// Gets or sets the read only background.
        /// </summary>
        /// <value>The read only background.</value>
        public Brush ReadOnlyBackground
        {
            get { return (Brush)GetValue(ReadOnlyBackgroundProperty); }
            set { SetValue(ReadOnlyBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnlyBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyBackgroundProperty =
            DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(PropertyCatagoryViewItem), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the read only font weight.
        /// </summary>
        /// <value>The read only font weight.</value>
        public FontWeight ReadOnlyFontWeight
        {
            get { return (FontWeight)GetValue(ReadOnlyFontWeightProperty); }
            set { SetValue(ReadOnlyFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnlyFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyFontWeightProperty =
            DependencyProperty.Register("ReadOnlyFontWeight", typeof(FontWeight), typeof(PropertyCatagoryViewItem), new PropertyMetadata(FontWeights.Normal));


        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCatagoryViewItem"/> class.
        /// </summary>
        public PropertyCatagoryViewItem()
        {
            DefaultStyleKey = typeof(PropertyCatagoryViewItem);
#if WPF
            if (Syncfusion.Licensing.EnvironmentTest.IsSecurityGranted)
            {
                Syncfusion.Licensing.EnvironmentTest.StartValidateLicense(typeof(PropertyCatagoryViewItem));
            }
#endif
        }
        #endregion

        #region overrides

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            if (newHeader != null && this.PropertyView != null && this.PropertyView.PropertyGrid != null)
            {
                if (this.PropertyView.PropertyGrid.CollapsedCategoryViewItems.Contains(newHeader))
                    this.IsExpanded = false;
            }
            base.OnHeaderChanged(oldHeader, newHeader);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PropertyViewItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PropertyViewItem();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            PropertyViewItem ele = (PropertyViewItem)element;
            PropertyItem info = (PropertyItem)item;
            ele.Header = info;
            ele.ItemsSource = info.SelectedObjectProperties;
            ele.ItemTemplate = info.Template;
            ITypeEditor editor = info.Editor as ITypeEditor;
            ele.IsSelected = info.IsSelected;
            if (!info.IsCategoryEditorEnabled)
            {
                if (editor != null)
                {
                    object neweditor = editor.Create(info.PropertyInformation);
                    if (neweditor != null)
                    {
                        if (info.CanWrite)
                        {
                            try
                            {
                                Binding bind = new Binding(info.Name);
                                bind.Source = info.SelectedObject;
                                bind.ValidatesOnExceptions = true;
                                bind.ValidatesOnDataErrors = true;
                                bind.Mode = BindingMode.TwoWay;
                                BindingOperations.SetBinding(info, PropertyItem.ValueProperty, bind);
                                editor.Attach(new PropertyViewItem(), info);
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                Binding bind = new Binding(info.Name);
                                bind.Source = info.SelectedObject;
                                bind.ValidatesOnExceptions = true;
                                bind.ValidatesOnDataErrors = true;
                                bind.Mode = BindingMode.OneWay;
                                BindingOperations.SetBinding(info, PropertyItem.ValueProperty, bind);
                                editor.Attach(new PropertyViewItem(), info);
                            }
                            catch { }
                        }

                        info.PropertyEditor = neweditor;
                    }
                }
            }
            else
            {
                foreach (PropertyItem propertyCategoryItem in info.CategoryValueProperties)
                {
                    if (propertyCategoryItem.CanWrite)
                    {
                        try
                        {
                            Binding bind = new Binding(propertyCategoryItem.Name);
                            bind.Source = propertyCategoryItem.SelectedObject;
                            bind.ValidatesOnExceptions = true;
                            bind.ValidatesOnDataErrors = true;
                            bind.Mode = BindingMode.TwoWay;
                            BindingOperations.SetBinding(propertyCategoryItem, PropertyItem.ValueProperty, bind);
                            editor.Attach(new PropertyViewItem(), propertyCategoryItem);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            Binding bind = new Binding(propertyCategoryItem.Name);
                            bind.Source = propertyCategoryItem.SelectedObject;
                            bind.ValidatesOnExceptions = true;
                            bind.ValidatesOnDataErrors = true;
                            bind.Mode = BindingMode.OneWay;
                            BindingOperations.SetBinding(propertyCategoryItem, PropertyItem.ValueProperty, bind);
                            editor.Attach(new PropertyViewItem(), propertyCategoryItem);
                        }
                        catch { }
                    }
                }
            }

            Binding bindBackground = new Binding("Background");
            bindBackground.Source = this;
            bindBackground.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindBackground);

            ele.EditorTemplate = info.Template;
            ele.PropertyView = this.PropertyView;
            ele.PropertyCatagoryViewItem = this;
            
            if (info.IsSelected)
            {
                ele.IsSelected = info.IsSelected;
                this.PropertyView.SelectedItem = ele;
                this.PropertyView.SelectedProperty = info;
            }


            if (info.CanWrite)
            {
                if (this.EditableBackground != null)
                {
                    Binding bindEditableBackground = new Binding("EditableBackground");
                    bindEditableBackground.Source = this;
                    bindEditableBackground.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindEditableBackground);
                }
                if (this.EditableFontWeight != FontWeights.Normal)
                {
                    Binding bindEditableFontWeight = new Binding("EditableFontWeight");
                    bindEditableFontWeight.Source = this;
                    bindEditableFontWeight.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.FontWeightProperty, bindEditableFontWeight);
                }
            }

            else
            {
                if (this.ReadOnlyBackground != null)
                {
                    Binding bindReadOnlyBackground = new Binding("ReadOnlyBackground");
                    bindReadOnlyBackground.Source = this;
                    bindReadOnlyBackground.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindReadOnlyBackground);
                }

                if (this.ReadOnlyFontWeight != FontWeights.Normal)
                {
                    Binding bindReadOnlyFontWeight = new Binding("ReadOnlyFontWeight");
                    bindReadOnlyFontWeight.Source = this;
                    bindReadOnlyFontWeight.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.FontWeightProperty, bindReadOnlyFontWeight);
                }
            }

            base.PrepareContainerForItemOverride(ele, item);
        }

        ContentControl PART_Header;
        ItemsPresenter PART_Presenter;
        ToggleButton PART_Expander;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Header = this.GetTemplateChild("PART_Header") as ContentControl;
            PART_Presenter = this.GetTemplateChild("PART_Presenter") as ItemsPresenter;
            PART_Expander = this.GetTemplateChild("PART_Expander") as ToggleButton;

            //if (this.PART_Presenter != null && this.IsExpanded)
            //{
            //    this.PART_Presenter.Visibility = Visibility.Visible;
            //}
        }
        #endregion

        /// <summary>
        /// Called when [is expanded changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsExpandedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyCatagoryViewItem item = (PropertyCatagoryViewItem)obj;
            if (item != null)
                item.OnIsExpandedChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:IsExpandedChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsExpandedChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsExpanded)
            {
                if (this.PropertyView != null && this.PropertyView.PropertyGrid != null)
                {
                    if (this.Header != null && this.PropertyView.PropertyGrid.CollapsedCategoryViewItems.Contains(this.Header))
                        this.PropertyView.PropertyGrid.CollapsedCategoryViewItems.Remove(this.Header);
                }
            }
            else
            {
                if (this.PropertyView != null && this.PropertyView.PropertyGrid != null && this.Header != null)
                {
                    if (!this.PropertyView.PropertyGrid.CollapsedCategoryViewItems.Contains(this.Header))
                        this.PropertyView.PropertyGrid.CollapsedCategoryViewItems.Add(this.Header);
                }
            }
#if SILVERLIGHT
            UpdateVisualState(true);
#endif
        }

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (this.IsExpanded)
                VisualStateManager.GoToState(this, "Expanded", true);
            else
                VisualStateManager.GoToState(this, "Collapsed", true);
        }

        /// <summary>
        /// Called when [category foreground changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCategoryForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Called when [line color changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }
    }
}
