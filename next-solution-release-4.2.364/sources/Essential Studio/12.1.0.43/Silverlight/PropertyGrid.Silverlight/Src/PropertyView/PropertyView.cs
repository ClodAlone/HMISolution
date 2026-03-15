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
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;

namespace Syncfusion.Windows.PropertyGrid
{

    public class PropertyView : ItemsControl
    {
        internal PropertyGrid PropertyGrid;

        /// <summary>
        /// Gets or sets the selected property.
        /// </summary>
        /// <value>The selected property.</value>
        public PropertyItem SelectedProperty
        {
            get { return (PropertyItem)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register("SelectedProperty", typeof(PropertyItem), typeof(PropertyView), new PropertyMetadata(OnSelectedPropertyChanged));

        /// <summary>
        /// Called when [selected property changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Raises the <see cref="E:SelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnSelectedPropertyChanged(DependencyPropertyChangedEventArgs args) { }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public PropertyViewItem SelectedItem
        {
            get { return (PropertyViewItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(PropertyViewItem), typeof(PropertyView), new PropertyMetadata(OnSelectedItemChanged));

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyView propertyView = (PropertyView)obj;
            if (propertyView != null)
                propertyView.OnSelectedItemChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedItemChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue)
            {

            }
            if (args.OldValue != null)
            {
                PropertyViewItem propertyViewItem = (PropertyViewItem)args.OldValue;
                if (propertyViewItem != null)
                {
                    propertyViewItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable grouping].
        /// </summary>
        /// <value><c>true</c> if [enable grouping]; otherwise, <c>false</c>.</value>
        public bool EnableGrouping
        {
            get { return (bool)GetValue(EnableGroupingProperty); }
            set { SetValue(EnableGroupingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableGroupingProperty =
            DependencyProperty.Register("EnableGrouping", typeof(bool), typeof(PropertyView), new PropertyMetadata(false, OnEnableGroupingChanged));

        /// <summary>
        /// Called when [enable grouping changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableGroupingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Raises the <see cref="E:EnableGroupingChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnEnableGroupingChanged(DependencyPropertyChangedEventArgs args) { }

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
            DependencyProperty.Register("CategoryForeground", typeof(Brush), typeof(PropertyView), new PropertyMetadata(OnCategoryForegroundChanged));

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
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(PropertyView), new PropertyMetadata(OnLineColorChanged));

        /// <summary>
        /// Gets or sets the color of the view background.
        /// </summary>
        /// <value>The color of the view background.</value>
        public Brush ViewBackgroundColor
        {
            get { return (Brush)GetValue(ViewBackgroundColorProperty); }
            set { SetValue(ViewBackgroundColorProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ViewBackgroundColorProperty =
            DependencyProperty.Register("ViewBackgroundColor", typeof(Brush), typeof(PropertyView), new PropertyMetadata(OnViewBackgroundColorChanged));


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
            DependencyProperty.Register("EditableBackground", typeof(Brush), typeof(PropertyView), new PropertyMetadata(null));
        
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
            DependencyProperty.Register("EditableFontWeight", typeof(FontWeight), typeof(PropertyView), new PropertyMetadata(FontWeights.Normal));


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
            DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(PropertyView), new PropertyMetadata(null));
        
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
            DependencyProperty.Register("ReadOnlyFontWeight", typeof(FontWeight), typeof(PropertyView), new PropertyMetadata(FontWeights.Normal));

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

        /// <summary>
        /// Called when [view background color changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewBackgroundColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyView"/> class.
        /// </summary>
        public PropertyView()
        {
            DefaultStyleKey = typeof(PropertyView);
#if WPF
            if (Syncfusion.Licensing.EnvironmentTest.IsSecurityGranted)
            {
                Syncfusion.Licensing.EnvironmentTest.StartValidateLicense(typeof(PropertyView));
            }
#endif
        }
        #endregion

        #region overrides
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if(!PropertyGrid.isCalledFromFilterPropertyGrid && element is PropertyViewItem)    
            {
                PropertyViewItem ele = (PropertyViewItem)element;
                PropertyItem info = (PropertyItem)item;
                ITypeEditor Typeeditor = info.Editor as ITypeEditor;
                if (info != null)
                {
                    //info.Editor = null;
                    info.PropertyEditor = null;
                    info.PropertyGrid = null;
                    info.SelectedObjectProperties = null;
                    info.ClearValue(PropertyItem.ValueProperty);
                }
                Typeeditor.Detach(ele);
            }
            base.ClearContainerForItemOverride(element, item);
        }
        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (this.EnableGrouping)
            {
                PropertyCatagoryViewItem ele = (PropertyCatagoryViewItem)element;
                PropertyCategoryViewItemCollection info = (PropertyCategoryViewItemCollection)item;
                ele.PropertyView = this;
                ele.Header = info.Category;
                ele.ItemTemplate = this.ItemTemplate;
                ele.ItemsSource = info.Properties;
                
                if (info.Category == "Text")
                {
                    Debug.WriteLine(info.Category);
                }

                var selected = from prop in info.Properties where ((PropertyItem)prop).IsSelected == true select prop;
                if (selected.Count() > 0)
                {
                    if (PropertyGrid != null && !PropertyGrid.CollapsedCategoryViewItems.Contains(ele.Header))
                        ele.IsExpanded = true;
                }
                Binding bindCategoryForeground = new Binding("CategoryForeground");
                bindCategoryForeground.Source = this;
                bindCategoryForeground.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.CategoryForegroundProperty, bindCategoryForeground);

                Binding bindLineColor = new Binding("LineColor");
                bindLineColor.Source = this;
                bindLineColor.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.LineColorProperty, bindLineColor);

                Binding bindBackground = new Binding("ViewBackgroundColor");
                bindBackground.Source = this;
                bindBackground.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.BackgroundProperty, bindBackground);

                Binding bindEditableBackground = new Binding("EditableBackground");
                bindEditableBackground.Source = this;
                bindEditableBackground.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.EditableBackgroundProperty, bindEditableBackground);

                Binding bindEditableFontWeight = new Binding("EditableFontWeight");
                bindEditableFontWeight.Source = this;
                bindEditableFontWeight.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.EditableFontWeightProperty, bindEditableFontWeight);

                Binding bindReadOnlyBackground = new Binding("ReadOnlyBackground");
                bindReadOnlyBackground.Source = this;
                bindReadOnlyBackground.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.ReadOnlyBackgroundProperty, bindReadOnlyBackground);

                Binding bindReadOnlyFontweight = new Binding("ReadOnlyFontWeight");
                bindReadOnlyFontweight.Source = this;
                bindReadOnlyFontweight.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyCatagoryViewItem.ReadOnlyFontWeightProperty, bindReadOnlyFontweight);

                base.PrepareContainerForItemOverride(ele, item);
            }
            else
            {
                object neweditor =null;
                PropertyViewItem ele = (PropertyViewItem)element;
                PropertyItem info = (PropertyItem)item;
                ele.Header = info;
                ele.ItemsSource = info.SelectedObjectProperties;
                ele.ItemTemplate = info.Template;
                ITypeEditor editor = info.Editor as ITypeEditor;
                if (editor != null)
                    neweditor = editor.Create(info.PropertyInformation);

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
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            Binding bind = new Binding(info.Name);
                            bind.Source = info.PropertyGrid.SelectedObject;
                            bind.ValidatesOnExceptions = true;
                            bind.ValidatesOnDataErrors = true;
                            bind.Mode = BindingMode.OneWay;
                            BindingOperations.SetBinding(info, PropertyItem.ValueProperty, bind);
                        }
                        catch { }
                    }
                    editor.Attach(ele, info);
                }
                info.PropertyEditor = neweditor;

                if (info.IsSelected)
                {
                    ele.IsSelected = info.IsSelected;
                    this.SelectedItem = ele;
                    this.SelectedProperty = info;
                }
                ele.EditorTemplate = info.Template;
                ele.PropertyView = this;

                Binding bindBackground = new Binding("ViewBackgroundColor");
                bindBackground.Source = this;
                bindBackground.Mode = BindingMode.TwoWay;
                BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindBackground);

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
            if (this.EnableGrouping)
                return item is PropertyCatagoryViewItem;
            else
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
            if (this.EnableGrouping)
                return new PropertyCatagoryViewItem();
            else
                return new PropertyViewItem();
        }

        #endregion

    }
}
