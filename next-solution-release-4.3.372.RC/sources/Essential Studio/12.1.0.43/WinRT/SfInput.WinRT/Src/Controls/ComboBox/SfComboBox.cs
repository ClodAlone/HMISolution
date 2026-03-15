#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using Syncfusion.UI.Xaml.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;
using Windows.UI;
using Windows.Foundation;
using Syncfusion.UI.Xaml.Controls.Data;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that allows the user to select and edit a date by using a drop-down
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> control for each DateTime part.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    /// 
 
    [ClassReference(IsReviewed = false)]
    public class SfComboBox : ComboBox, IDataValidator
    {
        #region Private members
        
        private SfTextBoxExt TextEditor;

        private ContentPresenter content;

        private bool IsItemSelectedFromDropDown = false;

        private bool IsLoaded = false;

        private bool IsTextChangedInternally = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> class.
        /// </summary>
        public SfComboBox()
        {
          DefaultStyleKey = typeof(SfComboBox);
          this.Loaded += SfComboBox_Loaded;
          this.SelectionChanged += SfComboBox_SelectionChanged;
          this.Unloaded+=SfComboBox_Unloaded;
        }

        #endregion

        #region DependencyProperty

        /// <summary>
        /// Gets or sets the mode for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> control.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.ComboBoxModes"/>
        /// <remarks> The default value is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.ComboBoxModes.DropDown"/> </remarks>
        public ComboBoxModes ComboBoxMode
        {
            get { return (ComboBoxModes)GetValue(ComboBoxModeProperty); }
            set { SetValue(ComboBoxModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ComboBoxModeProperty =
            DependencyProperty.Register("ComboBoxMode", typeof(ComboBoxModes), typeof(SfComboBox), new PropertyMetadata(ComboBoxModes.DropDown,new PropertyChangedCallback(OnComboBoxModeChanged)));

        private static void OnComboBoxModeChanged(DependencyObject b,DependencyPropertyChangedEventArgs e)
        {
            SfComboBox control = (b as SfComboBox);
            if (control != null)
            {
               
                control.OnComboBoxModeChanged(e);
                
            }
        }

        private List<object> AutoCompleteItems
        {
            get { return (List<object>)GetValue(AutoCompleteItemsProperty); }
            set { SetValue(AutoCompleteItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoCompleteItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemsProperty =
            DependencyProperty.Register("AutoCompleteItems", typeof(List<object>), typeof(SfComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data used as WateMark
        /// </summary>
        /// <value> The default value is null </value>
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }


        /// <summary>
        /// Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(SfComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the data used as WateMark
        /// </summary>
        /// <value> The default value is null </value>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(SfComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TemplateSelector for the data used as WateMark
        /// </summary>
        /// <value> The default value is null </value>
        public DataTemplateSelector WatermarkTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(WatermarkTemplateSelectorProperty); }
            set { SetValue(WatermarkTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateSelectorProperty =
            DependencyProperty.Register("WatermarkTemplateSelector", typeof(DataTemplateSelector), typeof(SfComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Getsor sets the Path for the Display Member string
        /// </summary>
        public new string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SearchItemPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly new DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(SfComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> that has been selected.
        /// </summary>
        public new object SelectedItem
        {
            get { return (object )GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly new DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SfComboBox), new PropertyMetadata(null,new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Getsor sets the Index of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> that has been selected.
        /// </summary>
        /// <value> The default value is -1 </value>
        public new int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly new DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SfComboBox), new PropertyMetadata(-1,new PropertyChangedCallback(OnSelectedIndexChanged)));       

        #endregion

        #region Callbacks

        void SfComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedIndex != base.SelectedIndex)
            {
                this.SelectedIndex = base.SelectedIndex;
            }
            if (this.SelectedItem != base.SelectedItem)
            {
                this.SelectedItem = base.SelectedItem;
            }

            IsItemSelectedFromDropDown = true;
            if (TextEditor != null && TextEditor.SelectedItem != SelectedItem)
            {
                if (this.SelectedItem is FrameworkElement)
                {
                    TextEditor.SelectedItem = GetContent(this.SelectedItem as FrameworkElement);
                }
                else
                {
                    TextEditor.SelectedItem = SelectedItem;
                }
            }
            IsItemSelectedFromDropDown = false;
        }

        private void OnComboBoxModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                UpdateVisualState((ComboBoxModes)e.NewValue);
            }
        }

        void SfComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectionChanged += SfComboBox_SelectionChanged;
            IsLoaded = true;
            UpdateVisualState(this.ComboBoxMode);
            if (TextEditor != null)
            {
                TextEditor.SelectedItemChanged += TextEditor_SelectedItemChanged;
                TextEditor.TextChanged += TextEditor_TextChanged;
            }
            base.SelectedIndex = this.SelectedIndex;
            base.SelectedItem = this.SelectedItem;
        }

        void SfComboBox_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SfComboBox_Unloaded;
            this.Unloaded -= SfComboBox_Unloaded;
            this.SelectionChanged -= SfComboBox_SelectionChanged;
            if (TextEditor != null)
            {
                TextEditor.SelectedItemChanged -= TextEditor_SelectedItemChanged;
                TextEditor.TextChanged -= TextEditor_TextChanged;
            }
            IsLoaded = false;
        }

        void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextEditor != null && TextEditor.AutoCompleteSource != null)
            {
                var list = (from object item in TextEditor.AutoCompleteSource
                            where item != null
                            let strvalue = GetStringFromSearchItemPath(item, TextEditor.SearchItemPath)
                            where
                                !String.IsNullOrEmpty(strvalue) &&
                                TextEditor.IgnoreCase ? strvalue.ToLower().Equals(TextEditor.Text.ToLower(), StringComparison.CurrentCulture) : strvalue.Equals(TextEditor.Text, StringComparison.CurrentCulture)
                            select item).ToList<object>();

                if (list.Count > 0)
                {
                    IsTextChangedInternally = true;
                    this.SelectedIndex = TextEditor.AutoCompleteSource.Cast<object>().ToList<object>().IndexOf(list.ElementAt<object>(0));
                    TextEditor.SelectedItem = list[0];
                    IsTextChangedInternally = false;
                }
            }
        }

        void TextEditor_SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItem is FrameworkElement)
            {
                FrameworkElement item=null;
                if (TextEditor.SelectedItem != null)
                {
                    item = Items[AutoCompleteItems.IndexOf(TextEditor.SelectedItem)] as FrameworkElement;
                }
                else
                {
                    item = null;
                }
                if (item != this.SelectedItem)
                {
                    this.SelectedItem = item;
                }
            }
            else if (ItemsSource != null && TextEditor.SelectedItem != this.SelectedItem)
            {
                this.SelectedItem = TextEditor.SelectedItem;
            }

            if (IsItemSelectedFromDropDown)
            {
                UpdatePropertyValue<bool>("canappend", false);
                
            }
        }

        private static void OnSelectedItemChanged(DependencyObject sender,DependencyPropertyChangedEventArgs args)
        {
           SfComboBox instance = (sender as SfComboBox);
           if (instance != null)
           {
               instance.OnSelectedItemChanged(args);
           }
        }

        /// <summary>
        /// Occurs when the Selected Combobox item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> has changed.
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (base.SelectedItem != e.NewValue)
                {
                    base.SelectedItem = e.NewValue;
                }
            }
          
        }

        private static void OnSelectedIndexChanged(DependencyObject sender ,DependencyPropertyChangedEventArgs e)
        {
            SfComboBox instance = (sender as SfComboBox);
            if (instance != null)
            {
                instance.OnSelectedIndexChanged(e);
            }
        }

        /// <summary>
        /// Occurs when index of the Selected Combobox item <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> has changed.
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && base.SelectedIndex != (int)e.NewValue)
            {
                base.SelectedIndex = (int)e.NewValue;
            }
        }

        #endregion

        #region Methods

        private bool MatchPropertyInfo(PropertyInfo info, string path)
        {
            if (info.Name == path)
            {
                return true;
            }
            return false;
        }

        private string GetStringFromSearchItemPath(object item, string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                PropertyInfo info = item.GetType().GetRuntimeProperties().ToList().FirstOrDefault(_info => MatchPropertyInfo(_info, path));
                if (info != null && info.GetValue(item) != null)
                {
                    String trimmedText = info.GetValue(item).ToString().TrimStart();
                    return trimmedText;
                }
                else
                {
                    if (item != null)
                    {
                        return item.ToString().TrimStart();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            else
            {
                return item.ToString();
            }
        }

        private void UpdateVisualState(ComboBoxModes mode)
        {
            if (mode == ComboBoxModes.Editable)
            {
                VisualStateManager.GoToState(this, "Editable", true);
                if (TextEditor != null)
                {
                    if (this.IsTabStop == true)
                    {
                        this.IsTabStop = false;
                        TextEditor.IsTabStop = true;
                    }

                    TextEditor.SelectionStart = TextEditor.Text.Length;
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "DropDown", true);
                this.IsTabStop = true;
                if (TextEditor != null)
                TextEditor.IsTabStop = false;
            }
        }

        private object GetContent(FrameworkElement element)
        {
            if (element is ContentControl)
            {
                return (element as ContentControl).Content;
            }
            else if (element is ContentPresenter)
            {
                return ((element as ContentPresenter).Content);
            }
            else if (element is TextBox)
            {
                return (element as TextBox).Text as object;
            }
            else if (element is HeaderedItemsControl)
            {
                return (element as HeaderedItemsControl).Header;
            }
            else
            { 
                return element.ToString();
            }
        }

        private void UpdatePropertyValue<T>(string propertyName,T value)
        {
            //Changing the value of canappend to false.
            var elements = TextEditor.GetType().GetRuntimeFields();

            foreach (var fieldInfo in elements.Where(fieldInfo => fieldInfo.Name == propertyName))
            {
                fieldInfo.SetValue(TextEditor, (T)value);
            }
        }
       
        #endregion

        #region Override

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (TextEditor != null && (string.IsNullOrEmpty(TextEditor.Text) || string.IsNullOrWhiteSpace(TextEditor.Text)))
            {
                this.SelectedItem = null;
                this.SelectedIndex = -1;
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBoxItem"/> items has changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            List<object> itemscollection = new List<object>();
            foreach (object item in Items)
            {
                if (item is FrameworkElement)
                {
                    itemscollection.Add(GetContent(item as FrameworkElement));
                }
            }
            if (Items != null && TextEditor != null)
            {
                if(TextEditor.AutoCompleteSource==null && Items.Count > 0)
                    TextEditor.AutoCompleteSource = Items;
                if (Items.Count == 0)
                    TextEditor.Text = string.Empty;
            }
            AutoCompleteItems = itemscollection;
            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Occurs when the 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> is open.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDownOpened(object e)
        {
            IsItemSelectedFromDropDown = true;
            //if (TextEditor != null && (string.IsNullOrEmpty(TextEditor.Text) || string.IsNullOrWhiteSpace(TextEditor.Text))
            //    || SelectedItem != null && TextEditor.SelectedItem != null && SelectedItem is FrameworkElement &&
            //    TextEditor.SelectedItem.ToString().ToLower() != GetContent(SelectedItem as FrameworkElement).ToString().ToLower()
            //    || (!(SelectedItem is FrameworkElement) && SelectedItem != TextEditor.SelectedItem))
            //{
            //    this.SelectedItem = null;
            //}
            base.OnDropDownOpened(e);
        }
    
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            TextEditor = GetTemplateChild("PART_Editor") as SfTextBoxExt;

            content = GetTemplateChild("ContentPresenter") as ContentPresenter;

            if (TextEditor != null)
            {
                if (Items.Count > 0 && ItemsSource == null)
                {
                    TextEditor.SetBinding(SfTextBoxExt.AutoCompleteSourceProperty,
                       new Binding()
                       {
                           Source = this,
                           Path = new PropertyPath("AutoCompleteItems"),
                           Mode = BindingMode.TwoWay
                       });
                }
                else if (ItemsSource != null)
                {
                    TextEditor.SetBinding(SfTextBoxExt.AutoCompleteSourceProperty,
                       new Binding()
                       {
                           Source = this,
                           Path = new PropertyPath("ItemsSource"),
                           Mode = BindingMode.TwoWay
                       });

                    AutoCompleteItems = (from object item in TextEditor.AutoCompleteSource
                                         select item).ToList<object>();
                }
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Occurs when the 
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfComboBox"/> is closed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDownClosed(object e)
        {
            IsItemSelectedFromDropDown = false;
            if (this.ComboBoxMode == ComboBoxModes.Editable && TextEditor != null)
            {
                UpdatePropertyValue<bool>("canappend", true);
                if (!IsTextChangedInternally)
                {
                    if(TextEditor.SelectedItem != null)
                    {
                         TextEditor.Text = GetStringFromSearchItemPath(TextEditor.SelectedItem, TextEditor.SearchItemPath);
                    }
                    else
                        TextEditor.Text = string.Empty;
                }
                TextEditor.SelectionStart = this.TextEditor.Text.Length;
                TextEditor.Focus(FocusState.Pointer);
            }
           base.OnDropDownClosed(e);
        }

        /// <summary>
        ///  Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfComboBoxItem"/>
        /// </summary>
        /// <returns>Dependency Object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SfComboBoxItem();
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfComboBoxItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if it is a SfComboBoxItem; otherwise, <c>false</c>
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfComboBoxItem;
        }

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            SfComboBoxItem comboboxItem = item as SfComboBoxItem;
            base.PrepareContainerForItemOverride(element, item);
            if(comboboxItem==null)
            {
                comboboxItem = this.ItemContainerGenerator.ContainerFromItem(item) as SfComboBoxItem;
            }
            if (comboboxItem != null)
            {
                if (ItemsSource != null && this.ItemTemplate==null)
                {
                    comboboxItem.Content = GetStringFromSearchItemPath(item, DisplayMemberPath);
                }
            }
        }

        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (ComboBoxMode == ComboBoxModes.Editable)
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.Space:
                    case Windows.System.VirtualKey.Right:
                    case Windows.System.VirtualKey.Left:
                        return;
                }
            }
           base.OnKeyDown(e);
        }
        
        #endregion

        #region validation
        /// <summary>
        /// Validates the states
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.VisualStates"/>
        /// </summary>
        /// <param name="args"></param>
        public void Validate(ValidationEventArgs args)
        {
            if (args.HasError)
            {
                VisualStateManager.GoToState(this, "HasError", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoError", true);
            }
        }
        #endregion
    }
}
