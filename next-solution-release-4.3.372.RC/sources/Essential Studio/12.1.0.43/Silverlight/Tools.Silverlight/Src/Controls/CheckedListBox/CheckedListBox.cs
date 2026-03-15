#region Copyright
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
using System.Collections.Generic;
using System.Windows.Data;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
   

    /// <summary>
    /// Represents the CheckedListBox UI element
    /// </summary>
    /// <remarks>
    /// The Checked List Box control implements a classic list box with check list box items. The control displays items with a check box using which multiple items can be checked .
    /// </remarks>
    /// <example>
    /// <para>The following example shows how to create a CheckedListBox in C#.</para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C#</term></listheader>
    /// <item>
    /// <description>Using Syncfusion.Windows.Tools.Controls;
    /// <para></para>
    /// <para>CheckedListBox checkedlistbox = new CheckedListBox();</para>
    /// <para>            checklistbox.RightToLeft = true;</para>
    /// <para>            checklistbox.CheckOnClick = true;</para>
    /// <para>            grid.Children.Add(checklistbox);</para></description></item></list>
    /// <para></para>
    /// <para>The following example shows how to create a CheckedListBox in Xaml.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot;
    /// <para>  </para>
    /// <para>&lt;syncfusion:CheckedListBox Name=&quot;checklistbox&quot; CheckOnClick=&quot;true&quot; RightToleft=&quot;true&quot; &gt;</para>
    /// <para>&lt;/syncfusion:CheckedListBox</para>
    /// <para>       </para></description></item></list>
    /// </example>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Blend;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Default;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2003;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Windows7;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.VS2010;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Metro;component/CheckedListBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(CheckedListBox), XamlResource = "/Syncfusion.Theming.Transparent;component/CheckedListBox.xaml")]

    public class CheckedListBox : ItemsControl
    {
        internal bool m_externalcontentselect = false;

        internal bool m_checkedItemsAddedInternally = false;
        #region Public Dependency Properties

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckOnClick">CheckOnClick</see> dependency property.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the CheckedListBox item is checked when it is selected and when it is false the the item is checked only after it is selected.The default value is False.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">Boolean</see>
        /// </returns>
        public static readonly DependencyProperty CheckOnClickProperty = DependencyProperty.Register("CheckOnClick", typeof(bool), typeof(CheckedListBox), new PropertyMetadata(false, OnCheckOnClickChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.SelectedItem">SelectedItem</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to retrieve the selected item in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Object">object</see>
        /// </returns>
        /// 


        public static readonly DependencyProperty CheckedListBoxAlignmentProperty = DependencyProperty.Register("CheckedListBoxAlignment", typeof(CheckedBoxAlignment), typeof(CheckedListBox), new PropertyMetadata(CheckedBoxAlignment.Left,OnAlignmentChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CheckedListBox), new PropertyMetadata(OnSelectionChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FullRowSelectionProperty = DependencyProperty.Register("FullRowSelection", typeof(bool), typeof(CheckedListBox), new PropertyMetadata(false, OnFullRowSelectionChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.SelectedIndex">SelectedIndex</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to retrieve the selected item index in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Int32">Int</see>
        /// </returns>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CheckedListBox), new PropertyMetadata(OnSelectionChanged));

        ///// <summary>
        ///// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.RightToLeft">RightToLeft</see> dependency property.
        ///// </summary>
        ///// <remarks>
        ///// When this property is set to true, the checkbox in CheckedListBox item is aligned to right.The default value is False.
        ///// </remarks>
        ///// <returns>
        ///// Type:<see cref="T:System.Boolean">Boolean</see>
        ///// </returns>
       // public static readonly DependencyProperty RightToLeftProperty = DependencyProperty.Register("RightToLeft", typeof(bool), typeof(CheckedListBox), new PropertyMetadata(OnRightToLeftChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.SelectedItemBackground">SelectedItemBackground</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to set the selected color in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.SolidColorBrush">SolidColorBrush</see>
        /// </returns>
        public static readonly DependencyProperty SelectedItemBackgroundProperty = DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(CheckedListBox), new PropertyMetadata(OnSelectedItemBackgroundChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.MouseOverBackground">MouseOverBackground</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to set the Hover color in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.SolidColorBrush">SolidColorBrush</see>
        /// </returns>
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(CheckedListBox), new PropertyMetadata(OnMouseOverBackgroundChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckedItems">CheckedItems</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to retrieve the checked items in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:Observable Collection of objects
        /// </returns>
        public static readonly DependencyProperty CheckedItemsProperty = DependencyProperty.Register("CheckedItems", typeof(ObservableCollection<object>), typeof(CheckedListBox), new PropertyMetadata(new ObservableCollection<object>(), OnCheckedItemsChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckBoxStyle">CheckBoxStyle</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to set the CheckBox Style in the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">Boolean</see>
        /// </returns>
        public static readonly DependencyProperty CheckBoxStyleProperty = DependencyProperty.Register("CheckBoxStyle", typeof(Style), typeof(CheckedListBox), new PropertyMetadata(OnCheckBoxStyleChanged));


        /// <summary>
        /// Identifies  <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckBoxStyle">Mode</see> Dependency Property
        /// </summary>
        /// <remarks>
        /// This property is used the set the Mode of the CheckedListBox.
        /// </remarks>
        /// <returns>
        /// Type:Enum
        /// </returns>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Modes), typeof(CheckedListBox), new PropertyMetadata(Modes.Checked , OnModeChanged));

      //  public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(CheckedListBox), new PropertyMetadata(false, new PropertyChangedCallback(OnAllowDragDropChanged)));

                    

        #endregion Public Dependency Properties.

        /// <summary>
        /// Gets the CheckedListBoxItem from the given object⌈
        /// </summary>
        private IDictionary<object, CheckedListBoxItem> _objectToCheckedListBoxItem;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Tools.Controls.CheckedListBox">CheckedListBox</see> class
        /// </summary>
        public CheckedListBox()
        {
            DefaultStyleKey = typeof(CheckedListBox);
            CheckedItems = new ObservableCollection<object>();
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            observableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCheckedItemsCollectionChanged);
            //SetValue(CheckedItemsProperty, observableCollection);
         }
        #endregion Constructor

        #region Public Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckOnClick">CheckOnClick</see> is changed.
        /// </summary>

        public event PropertyChangedCallback FullRowSelectionChanged;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback CheckOnClickChanged;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback AlignmentChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.SelectionChanged">SelectionChanged</see> is changed.
        /// </summary>
        public event PropertyChangedCallback SelectionChanged;

        ///// <summary>
        ///// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.RightToLeft">RightToLeft</see> is changed.
        ///// </summary>
        //public event PropertyChangedCallback RightToLeftChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckedItems">CheckedItems</see> is changed.
        /// </summary>
        public event PropertyChangedCallback CheckedItemsChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.SelectedItemBackground">SelectedItemBackground</see> is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.Mode"> Mode</see>is changed
        /// </summary>
        public event PropertyChangedCallback ModeChanged;

       // public event PropertyChangedCallback DragChanged;


        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.MouseOverBackground">MouseOverBackground</see> is changed.
        /// </summary>
        public event PropertyChangedCallback MouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckBoxStyle">CheckBoxStyle</see> is changed.
        /// </summary>
        public event PropertyChangedCallback CheckBoxStyleChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets or sets the CheckedItems.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Using this property ,the checked items in the Checked Listbox can be retreived.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Collections.ObjectModel.ObservableCollection`1">ObservableCollection</see>
        /// </value>
        public ObservableCollection<object> CheckedItems
        {
            get
            {
                return (ObservableCollection<object>)GetValue(CheckedItemsProperty);
            }

            set
            {
                if (value != null)
                {
                    SetValue(CheckedItemsProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CheckOnClick property is set to true or false.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the CheckedListBox item is checked when it is selected and when it is false the the item is checked only after it is selected.The default value is False.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Boolean">Boolean</see>
        /// </value>
        public bool CheckOnClick
        {
            get { return (bool)GetValue(CheckOnClickProperty); }
            set { SetValue(CheckOnClickProperty, value); }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether the RightToLeft property is set to true or false .This is a dependency property.
        ///// </summary>
        ///// <remarks>
        ///// When this property is set to true, the checkbox in CheckedListBox item is aligned to right.The default value is False.
        ///// </remarks>
        ///// <value>
        ///// Type : <see cref="T:System.Boolean">Boolean</see>
        ///// </value>
        //public bool RightToLeft
        //{
        //    get { return (bool)GetValue(RightToLeftProperty); }
        //    set { SetValue(RightToLeftProperty, value); }
        //}


        /// <summary>
        /// Gets or sets the Mode. This is a dependency Property.
        /// </summary>
        /// <remarks>
        /// when this propery is changed, the CheckedListBox items change to RadioGroup or ListBox or NoramlList. The Default value is CheckedBox.
        /// </remarks>
        /// <value>
        /// Type:Enum
        /// </value>
        public Modes Mode
        {

            get { return (Modes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedIndex property.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to retrieve the selected item index in the CheckedListBox.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Int32">Int</see>
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItem property.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to retrieve the selected item in the CheckedListBox.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Object">object</see>
        /// </value>
        public object SelectedItem
        {
            get { return (object)base.GetValue(SelectedItemProperty); }
            set { base.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FullRowSelection
        {
            get { return (bool)base.GetValue(FullRowSelectionProperty); }
            set { base.SetValue(FullRowSelectionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public CheckedBoxAlignment CheckedListBoxAlignment
        {
            get { return (CheckedBoxAlignment)GetValue(CheckedListBoxAlignmentProperty); }
            set { SetValue(CheckedListBoxAlignmentProperty, value); }
        }


        /// <summary>
        /// Gets or sets the SelectedItemBackground property.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to set or get the selected color in the CheckedListBox.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Int32">Int</see>
        /// </value>
        public Brush SelectedItemBackground
        {
            get { return (Brush)GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the MouseOverBackground property.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to set or get the hover color in the CheckedListBox.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Int32">Int</see>
        /// </value>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CheckBoxStyle Dependency Property
        /// </summary>
        /// <value>The check box style.</value>
        public Style CheckBoxStyle
        {
            get
            {
                return (Style)base.GetValue(CheckBoxStyleProperty);
            }

            set
            {
                base.SetValue(CheckBoxStyleProperty, value);
            }
        }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the CheckedListBoxItem from the framework element.
        /// </summary>
        /// <param name="element">Framework element</param>
        /// <returns>
        /// Returns a CheckedListBoxItem
        /// </returns>
        public static CheckedListBoxItem GetCheckedListBoxItemFromChildren(FrameworkElement element)
        {
            CheckedListBoxItem item = null;
            if (element != null)
            {
                item = element as CheckedListBoxItem;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is CheckedListBoxItem)
                        {
                            item = (CheckedListBoxItem)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Applies the Template for the File Upload control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
                     
            if (this.Items != null)
            {
                
                foreach (object obj in this.Items)
                {
                    CheckedListBoxItem item = this.GetCheckedListBoxItemForObject(obj);                    
                    if (item != null)
                    {
                        if ((bool)item.IsChecked)
                        {
                            this.CheckedItems.Add(item);

                        }
                    }
                  
                  }
            }
            
        }
        #endregion Public Methods
      
        #region Protected Override Methods
        private static int clickcount=0;
        /// <summary>
        /// Invokes OnMouseLeftButton is Up.
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            CheckedListBox box = (CheckedListBox)this;
            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            CheckedListBoxItem checkeditem = CheckedListBox.GetCheckedListBoxItemFromChildren(fe);
            if (checkeditem != null && Mode!=Modes.Normal )
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    CheckedListBoxItem item = this.GetCheckedListBoxItemForObject(this.Items[i]);
                    if (item == checkeditem)
                    {
                        this.SelectedIndex = i;
                        this.SelectedItem = item;
                        item.IsSelected = true;
                        if (this.CheckOnClick == true)
                        {
                            item.IsChecked = item.itemcheckbox.IsChecked;
                            item.IsChecked = !item.IsChecked;
                            item.IsSelected = true;
                            if(!(bool)(item.IsChecked))
                            {
                                if (box.CheckedItems != null)
                                {
                                    box.CheckedItems.Remove(item);
                                }
                            }
                        }
                        else
                        {
                            item.selected = true;
                            if(this.CheckOnClick == false)
                                clickcount = clickcount+1;
                            if (clickcount == 2)
                            {
                                item.IsChecked = item.itemcheckbox.IsChecked;
                                item.IsChecked = !item.IsChecked;
                                item.IsSelected = true;
                                if (!(bool)(item.IsChecked))
                                {
                                    if (box.CheckedItems != null)
                                    {
                                        box.CheckedItems.Remove(item);
                                    }
                                }
                                clickcount = 0;
                            }
                        }
                    }
                    else
                    {
                        item.IsSelected = false;
                        item.selected = false;
                        if ((bool)!item.IsChecked)
                        {
                            if (box.CheckedItems != null)
                            {
                                box.CheckedItems.Remove(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected  void OnInitialized(EventArgs e)
        {
            this.Loaded+=new RoutedEventHandler(CheckedListBox_Loaded);
            this.Unloaded+=new RoutedEventHandler(CheckedListBox_Unloaded);
            
        }
        void CheckedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (object obj in Items)
            {
                CheckedListBoxItem item = this.GetCheckedListBoxItemForObject(obj);
            
            }

        }

        void CheckedListBox_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckedItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCheckedItemsCollectionChanged);

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
            if (item is CheckedListBoxItem)
            {
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// Method  Prepares the container for the item
        /// </summary>
        /// <param name="element">The element is a dependency object is used to prepare container for item override</param>
        /// <param name="item">The item is a object is used to prepare container</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            CheckedListBoxItem checkedlistBoxItem = element as CheckedListBoxItem;
            checkedlistBoxItem.Mode = this.Mode;            
            if (this.FullRowSelection == true)
            {
                checkedlistBoxItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                if (this.CheckedListBoxAlignment == CheckedBoxAlignment.Right)
                {
                    checkedlistBoxItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                }
                else if (this.CheckedListBoxAlignment == CheckedBoxAlignment.Left)
                {
                    checkedlistBoxItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                }
            }
            else
            {
                checkedlistBoxItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                if (this.CheckedListBoxAlignment == CheckedBoxAlignment.Right)
                {
                    checkedlistBoxItem.HorizontalAlignment = HorizontalAlignment.Right;
                    checkedlistBoxItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                }
                else if (this.CheckedListBoxAlignment == CheckedBoxAlignment.Left)
                {
                    checkedlistBoxItem.HorizontalAlignment = HorizontalAlignment.Left;
                    checkedlistBoxItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                }
            }
            bool setContent = true;
            if (checkedlistBoxItem != item)
            {
                if (null != ItemTemplate)
                {
                    checkedlistBoxItem.ContentTemplate = ItemTemplate;
                }

                if (setContent)
                {
                    checkedlistBoxItem.Content = item;
                }

                ObjectToCheckedListBoxItem[item] = checkedlistBoxItem;
            }
        }

        /// <summary>
        /// Method returns the container
        /// </summary>
        /// <returns>Type :CheckedListBoxItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            CheckedListBoxItem checkedlistBoxItem = new CheckedListBoxItem();
            return checkedlistBoxItem;
        }

        /// <summary>
        /// Method clears the container 
        /// </summary>
        /// <param name="element">The element is a dependency object to clear container for item override</param>
        /// <param name="item">Indicates the Node to be Removed</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            CheckedListBoxItem checkedlistBoxItem = element as CheckedListBoxItem;
            checkedlistBoxItem.IsSelected = false;
            checkedlistBoxItem.ParentCheckedListBox = null;
            if (checkedlistBoxItem == item)
            {
                ObjectToCheckedListBoxItem.Remove(item);
            }

            this.SelectedItem = null;
            this.SelectedIndex = -1;
        }

        #endregion Protected Override Methods

        #region Protected virtual Methods
        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnCheckOnClickChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CheckOnClickChanged != null)
            {
                this.CheckOnClickChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFullRowSelectionChanged(DependencyPropertyChangedEventArgs e)
        {            
            if (this.FullRowSelectionChanged != null)
            {
                this.FullRowSelectionChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnCheckedItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue!=e.OldValue && e.NewValue!=null)
            {
                if (CheckedItems != null)
                {
                    if (CheckedItems.Count == 0)
                    {
                        foreach (Object obj in Items)
                        {
                            CheckedListBoxItem check = GetCheckedListBoxItemForObject(obj);
                            if (check != null)
                            {
                                if (check.IsChecked == true)
                                {
                                    check.IsChecked = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Object obj in CheckedItems)
                        {
                            CheckedListBoxItem check = GetCheckedListBoxItemForObject(obj);
                            if (check != null)
                            {
                                if (check.IsChecked == false)
                                {
                                    check.IsChecked = true;
                                }
                            }
                        }

                    }
                    if (CheckedItems != Ocollections)
                    {
                        Ocollections = CheckedItems;
                        Ocollections.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCheckedItemsCollectionChanged);
                    }
                    if (this.CheckedItemsChanged != null)
                    {
                        this.CheckedItemsChanged(this, e);
                    }
                }
            }

        }           

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnSelectedItemBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedItemBackgroundChanged != null)
            {
                this.SelectedItemBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MouseOverBackgroundChanged != null)
            {
                this.MouseOverBackgroundChanged(this, e);
            }
        }        

        /// <summary>
        /// Updates property value and raises event <see cref="E:ModeChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details such as old and new value.</param>
        protected virtual void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {

            if (this.ModeChanged != null)
            {
                this.ModeChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if(this.AlignmentChanged!=null)
            {
                this.AlignmentChanged(this,e);
            }

        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CheckBoxStyleChanged"/> event.
        /// </summary>
        protected virtual void OnCheckBoxStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CheckBoxStyleChanged != null)
            {
                CheckBoxStyleChanged(this, e);
            }
        }

        #endregion Protected virtual Methods

        #region Private Static Methods

        /// <summary>
        /// Calls OnCheckBoxStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnCheckBoxStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox source = (CheckedListBox)d;
            source.OnCheckBoxStyleChanged(e);

        }
        private static void OnGroupName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            // CaptureSource. OnGroupName(e);
        }
        ObservableCollection<object> Ocollections = new ObservableCollection<object>();
        /// <summary>
        /// Called when [checked items collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal void OnCheckedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //m_checkedItemsAddedInternally = true;
            ObservableCollection<object> collections = new ObservableCollection<object>();
            foreach(Object obj in (ObservableCollection<object>)sender)
            {
                collections.Add(obj);
            }
            //collections.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCheckedItemsCollectionChanged);
            SetValue(CheckedItemsProperty, collections);
            CheckedItems = collections;
           // CheckedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCheckedItemsCollectionChanged);
        }

        /// <summary>
        /// Calls OnCheckOnClickChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCheckOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnCheckOnClickChanged(e);
            }
        }
        
        /// <summary>
        /// Calls OnCheckedItemsChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCheckedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue && e.NewValue!=null)
            {
                instance.OnCheckedItemsChanged(e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnSelectedItemBackgroundChanged(e);
                foreach (object obj in instance.Items)
                {
                  CheckedListBoxItem item = instance.GetCheckedListBoxItemForObject(obj);
                  if (item != null && item.selectedcolor != null)
                  {
                      item.selectedcolor.Fill = instance.SelectedItemBackground;
                  }
                }
            }
        }

        /// <summary>
        /// Calls OnMouseOverBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnMouseOverBackgroundChanged(e);
                foreach (object obj in instance.Items)
                {
                    CheckedListBoxItem item = instance.GetCheckedListBoxItemForObject(obj);
                    if (item.hovercolor != null)
                    {
                        item.hovercolor.Fill = instance.MouseOverBackground;
                    }
                }
            }
        }

        
        /// <summary>
        /// Called OnModeChanged Method of the instance, notifies the dependency value changes.
        /// </summary>
        /// <param name="d">Dependency Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value. <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnModeChanged(e);
                foreach (object obj in instance.Items)
                {
                    CheckedListBoxItem item = instance.GetCheckedListBoxItemForObject(obj);
                    if (item.itemcheckbox != null && item.itemRadio != null)
                        instance.OnUpdate(item);
                }

            }
        }


        private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance =(CheckedListBox)d;
            if(e.NewValue!=e.OldValue)
            {
                instance.OnAlignmentChanged(e);
                foreach(object obj in instance.Items)
                {
                   CheckedListBoxItem item=instance.GetCheckedListBoxItemForObject(obj);
                    if(item.itemcheckbox!= null  && item.itemRadio!=null)
                        instance.OnUpdateAlignment(item);
                }
            }
        }

      
        /// <summary>
        /// Calls OnSelectionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                if (e.OldValue is CheckedListBoxItem)
                {
                    ((CheckedListBoxItem)e.OldValue).IsSelected = false;
                    ((CheckedListBoxItem)e.OldValue).selected = false;
                }
                instance.OnSelectionChanged(e);
                if ((CheckedListBoxItem)instance.SelectedItem!=null)
                {
                    ((CheckedListBoxItem)instance.SelectedItem).IsSelected = false;
                    ((CheckedListBoxItem)instance.SelectedItem).selected = false;
                }

                if (e.NewValue != null)
                {
                    if (e.NewValue is CheckedListBoxItem)
                    {
                        if (instance.SelectedItem != null)
                        {
                            (instance.SelectedItem as CheckedListBoxItem).IsSelected = true;
                        }
                    }
                    else
                    {
                        if (int.Parse(e.NewValue.ToString()) >= 0)
                        {
                            if (instance.Items[instance.SelectedIndex] is CheckedListBoxItem)
                            {
                                (instance.Items[instance.SelectedIndex] as CheckedListBoxItem).IsSelected = true;
                            }
                            else
                            {
                                CheckedListBoxItem item = instance.GetCheckedListBoxItemForObject(instance.Items[instance.SelectedIndex]) as CheckedListBoxItem;
                                item.IsSelected = true;
                            }
                        }
                    }
                }               
            }
        }

        private static void OnFullRowSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBox instance = (CheckedListBox)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnFullRowSelectionChanged(e);
            }
        }


        /// <summary>
        /// Gets the object to checked list box item.
        /// </summary>
        /// <value>The object to checked list box item.</value>
        private IDictionary<object, CheckedListBoxItem> ObjectToCheckedListBoxItem
        {
            get
            {
                if (null == _objectToCheckedListBoxItem)
                {
                    _objectToCheckedListBoxItem = new Dictionary<object, CheckedListBoxItem>();
                }

                return _objectToCheckedListBoxItem;
            }
        }
        #endregion Private Static Methods

        #region Internal Method

        /// <summary>
        /// This method is used to get CheckedlistboxItem from an object.
        /// </summary>
        /// <param name="value">object that is to be converted to CheckedListBoxItems</param>
        /// <returns>
        /// Returns a CheckedListBoxItem
        /// </returns>
        internal CheckedListBoxItem GetCheckedListBoxItemForObject(object value)
        {
            CheckedListBoxItem selectedListBoxItem = value as CheckedListBoxItem;
            if (selectedListBoxItem == null)
            {
                    ObjectToCheckedListBoxItem.TryGetValue(value, out selectedListBoxItem);
            }

            return selectedListBoxItem;
        }


        internal void OnUpdateAlignment(CheckedListBoxItem item)
        {
            if (item != null)
            {
                switch (CheckedListBoxAlignment)
                {
                    case CheckedBoxAlignment.Right:
                        item.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        break;
                    default:
                        item.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        break;
                }
            }
        }

        /// <summary>
        /// Invokes when the Mode Property is changed.
        /// </summary>
        /// <param name="item">CheckedLIstBoxItem to which the Mode is applied</param>
        internal void OnUpdate(CheckedListBoxItem item)
        {

            if (item != null)
            {
                switch (Mode)
                {
                    case Modes.Normal:
                        item.itemcheckbox.IsChecked = null;
                        item.itemRadio.IsChecked = null;;
                        
                       item.itemcheckbox.Visibility = Visibility.Collapsed  ;
                        item.itemRadio.Visibility = Visibility.Collapsed  ;
                        
                        break;

                    case Modes.RadioGroup:
                        item.itemRadio.IsChecked = false;
                        item.itemcheckbox.IsChecked = false;
                        item.itemcheckbox.Visibility = Visibility.Collapsed;
                       item.itemRadio.Visibility = Visibility.Visible;
                        break;
                    default:
                        item.itemcheckbox.IsChecked = false;
                        item.itemRadio.IsChecked = false;
                        item.itemRadio.Visibility = Visibility.Collapsed;
                        item.itemcheckbox.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        internal void OnDrag(CheckedListBoxItem item)
        {
            if (item != null)
            {
                
            }
        }

        #endregion Internal Method
    }
}
