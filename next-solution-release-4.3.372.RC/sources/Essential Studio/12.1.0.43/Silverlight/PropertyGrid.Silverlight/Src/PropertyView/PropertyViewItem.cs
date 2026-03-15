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
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.PropertyGrid
{
    //[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "SelectedMouseOver", GroupName = "CommonStates")]
    //[TemplateVisualState(Name = "UnFocusedSelection", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    //[TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    public class PropertyViewItem : HeaderedItemsControl
    {

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PropertyViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PropertyViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            PropertyViewItem ele = (PropertyViewItem)element;
            PropertyItem info = (PropertyItem)item;
            ele.Header = info;
            ele.ItemsSource = info.SelectedObjectProperties;
            ele.ItemTemplate = info.Template;
            ITypeEditor editor = info.Editor as ITypeEditor;
            object neweditor = editor.Create(info.PropertyInformation);

            if (neweditor != null)
            {
                if (info.CanWrite)
                {
                    try
                    {
                        Binding bind = new Binding(info.SelectedObject.ToString() + "." + info.Name);

                        bind.Mode = BindingMode.TwoWay;
                        bind.Source = info.PropertyGrid.SelectedObject;
                        bind.ValidatesOnExceptions = true;
                        bind.ValidatesOnDataErrors = true;
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
                PropertyView.SelectedItem = ele;
                PropertyView.SelectedProperty = info;
                PropertyView.PropertyGrid.SelectedPropertyItem = info;
            }
            ele.EditorTemplate = info.Template;
            ele.PropertyView = PropertyView;

            Binding bindBackground = new Binding("ViewBackgroundColor");
            bindBackground.Source = PropertyView;
            bindBackground.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindBackground);

            if (info.CanWrite)
            {
                if (PropertyView.EditableBackground != null)
                {
                    Binding bindEditableBackground = new Binding("EditableBackground");
                    bindEditableBackground.Source = PropertyView;
                    bindEditableBackground.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindEditableBackground);
                }
                if (PropertyView.EditableFontWeight != FontWeights.Normal)
                {
                    Binding bindEditableFontWeight = new Binding("EditableFontWeight");
                    bindEditableFontWeight.Source = PropertyView;
                    bindEditableFontWeight.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.FontWeightProperty, bindEditableFontWeight);
                }
            }
            else
            {
                if (PropertyView.ReadOnlyBackground != null)
                {
                    Binding bindReadOnlyBackground = new Binding("ReadOnlyBackground");
                    bindReadOnlyBackground.Source = PropertyView;
                    bindReadOnlyBackground.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.BackgroundProperty, bindReadOnlyBackground);
                }

                if (PropertyView.ReadOnlyFontWeight != FontWeights.Normal)
                {
                    Binding bindReadOnlyFontWeight = new Binding("ReadOnlyFontWeight");
                    bindReadOnlyFontWeight.Source = PropertyView;
                    bindReadOnlyFontWeight.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(ele, PropertyViewItem.FontWeightProperty, bindReadOnlyFontWeight);
                }
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        internal PropertyView PropertyView;
        internal PropertyCatagoryViewItem PropertyCatagoryViewItem;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(PropertyViewItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this element (including child elements in the visual tree).
        /// </summary>
        /// <value></value>
        /// <returns>true if mouse pointer is over the element or its child elements; otherwise, false. The default is false.</returns>
        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(PropertyViewItem), new PropertyMetadata(false, OnIsMouseOverChanged));

        /// <summary>
        /// 
        /// </summary>
        public bool HasItems
        {
            get
            {
                return this.Items.Count != 0;
            }
        }

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyViewItem"/> class.
        /// </summary>
        public PropertyViewItem()
        {
            DefaultStyleKey = typeof(PropertyViewItem);
#if WPF
            if (Syncfusion.Licensing.EnvironmentTest.IsSecurityGranted)
            {
                Syncfusion.Licensing.EnvironmentTest.StartValidateLicense(typeof(PropertyViewItem));
            }
#endif

        }
        #endregion

        /// <summary>
        /// Gets or sets the editor template.
        /// </summary>
        /// <value>The editor template.</value>
        public DataTemplate EditorTemplate
        {
            get { return (DataTemplate)GetValue(EditorTemplateProperty); }
            set { SetValue(EditorTemplateProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EditorTemplateProperty =
            DependencyProperty.Register("EditorTemplate", typeof(DataTemplate), typeof(PropertyViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
#if WPF
            if (this.editor != null)
            {
                this.IsSelected = true;
                (this.Header as PropertyItem).PropertyGrid.SelectedPropertyItem = this.Header as PropertyItem;
                //this.editor.Focus();
            }
#endif

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
#if WPF
            if (this.editor != null)
            {
                this.IsSelected = true;
                this.editor.Focus();
                (this.Header as PropertyItem).PropertyGrid.SelectedPropertyItem = this.Header as PropertyItem;
            }
            else
            {
                this.Focus();
            }
#endif
#if SILVERLIGHT
            this.Focus();
#endif
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (this.editor != null)
                this.IsMouseOver = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
#if SILVERLIGHT
            if (this.editor != null)
                this.IsMouseOver = true;
#endif
        }

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyViewItem item = (PropertyViewItem)obj;
            if (item != null)
                item.OnIsSelectedChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:IsSelectedChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsSelectedChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsSelected)
            {
                if (this.PropertyView != null)
                {
                    this.PropertyView.SelectedProperty = (PropertyItem)this.Header;
                    this.PropertyView.SelectedItem = this;

                    if (this.Header != null)
                    {
                        PropertyItem propertyViewItem = this.Header as PropertyItem;
                        if (propertyViewItem != null)
                        {
                            propertyViewItem.IsSelected = true;
                            if (propertyViewItem.PropertyGrid != null)
                                propertyViewItem.PropertyGrid.SelectedPropertyItem = this.Header as PropertyItem;
                        }
                    }
                }
            }
            else
            {
                if (this.Header != null)
                {
                    PropertyItem propertyViewItem = this.Header as PropertyItem;
                    if (propertyViewItem != null)
                    {
                        propertyViewItem.IsSelected = false;
                    }
                }
            }
            UpdateVisualState(true);
        }

        /// <summary>
        /// Called when [is mouse over changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMouseOverChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyViewItem propertyViewItem = (PropertyViewItem)obj;
            if (propertyViewItem != null)
                propertyViewItem.OnIsMouseOverChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:IsMouseOverChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsMouseOverChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="useTransitions">if set to <c>true</c> [use transitions].</param>
        private void UpdateVisualState(bool useTransitions)
        {
#if SILVERLIGHT
            if (this.IsSelected)
            {
                if (this.IsMouseOver)
                {
                    VisualStateManager.GoToState(this, "SelectedMouseOver", true);
                }
                VisualStateManager.GoToState(this, "Selected", true);
            }
            else if (this.IsMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
#endif
        }

        /// <summary>
        /// Gets or sets the identifying name of the element. The name provides a reference so that code-behind, such as event handler code, can refer to a markup element after it is constructed during processing by a XAML processor.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the element. The default is an empty string.</returns>
        public new string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(PropertyViewItem), new PropertyMetadata(string.Empty));

        private UIElement editor;
        ToggleButton PART_Toggle;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (editor != null)
            {
                editor.GotFocus -= new RoutedEventHandler(editor_GotFocus);
            }
            if (PART_Toggle != null)
            {

                PART_Toggle.Checked -= new RoutedEventHandler(PART_Toggle_Checked);
            }
            PART_Toggle = this.GetTemplateChild("PART_Toggle") as ToggleButton;
            if (PART_Toggle != null)
            {

                PART_Toggle.Checked += new RoutedEventHandler(PART_Toggle_Checked);
            }
            if (this.Header != null)
            {
                PropertyItem propertyViewItem = this.Header as PropertyItem;
                if (propertyViewItem != null)
                {
                    if (propertyViewItem.PropertyEditor != null & propertyViewItem.PropertyEditor is UIElement)
                    {
                        editor = propertyViewItem.PropertyEditor as UIElement;
                        editor.GotFocus += new RoutedEventHandler(editor_GotFocus);
                    }
                    if (PropertyView != null && PropertyView.SelectedItem == null)
                    {
                        if (PropertyView.PropertyGrid != null)
                        {
                            if (propertyViewItem.Name == PropertyView.PropertyGrid.DefaultPropertyPath)
                            {
                                this.IsSelected = true;
                                UpdateVisualState(true);
                                propertyViewItem.PropertyGrid.SelectedPropertyItem = this.Header as PropertyItem;
                            }
                        }
                    }
                }

            }



            if (this.IsSelected && this.editor != null)
            {
#if WPF
                this.editor.Focus();
#endif
#if SILVERLIGHT
                this.Focus();
#endif
            }
            this.Unloaded -= new RoutedEventHandler(PropertyViewItem_Unloaded);
            this.Unloaded += new RoutedEventHandler(PropertyViewItem_Unloaded);

        }
      
        void PART_Toggle_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        void PropertyViewItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (editor != null)
                editor.GotFocus -= new RoutedEventHandler(editor_GotFocus);
            if (this.PropertyView != null)
                this.PropertyView = null;
            if (this.Items.Count > 0)
                this.ItemsSource = null;
            if (this.PropertyCatagoryViewItem != null)
                this.PropertyCatagoryViewItem = null;

            this.Unloaded -= new RoutedEventHandler(PropertyViewItem_Unloaded);
        }

        void editor_GotFocus(object sender, RoutedEventArgs e)
        {
            this.IsSelected = true;
            if ((this.Header as PropertyItem) != null && (this.Header as PropertyItem).PropertyGrid != null)
                (this.Header as PropertyItem).PropertyGrid.SelectedPropertyItem = this.Header as PropertyItem;
        }

    }
}
