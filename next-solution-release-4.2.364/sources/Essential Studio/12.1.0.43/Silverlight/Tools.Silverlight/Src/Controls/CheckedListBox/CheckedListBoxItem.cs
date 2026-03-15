#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
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
    using System.Windows.Media.Imaging;
    using System.Resources;
    using System.Collections.Generic;
    using System.Windows.Data;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;

    /// <summary>
    /// Represents the CheckedListBoxItem UI element
    /// </summary>
    /// <example>
    /// <para>The following example shows how to create a CheckedListBoxItem in C#.</para>
    /// <para></para>
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
    /// <para>CheckedListBoxItem item = new CheckedListBoxItem();</para>
    /// <para>            item.Content = &quot;Silverlight&quot;;</para>
    /// <para>            checklistbox.Items.Add(item);</para>
    /// <para>            grid.Children.Add(checklistbox);</para></description></item></list>
    /// <para>The following example shows how to create a CheckedListBoxItem in Xaml.</para>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight&quot; 
    /// <para>  </para>
    /// <para>&lt;syncfusion:CheckedListBox Name=&quot;checklistbox&quot; CheckOnClick=&quot;true&quot; RightToleft=&quot;true&quot; &gt;</para>
    /// <para>&lt;syncfusion:CheckedListBoxItem Content=&quot;Silverlight&quot;/&gt;</para>
    /// <para>&lt;syncfusion:CheckedListBoxItem Content=&quot;Syncfusion&quot;/&gt;</para>
    /// <para>&lt;/syncfusion:CheckedListBox</para>
    /// <para>       </para></description></item></list>
    /// </example>
    [TemplatePart(Name = CheckedListBoxItem.ElementRootName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name ="Pressed", GroupName = "CommonStates")]
    public class CheckedListBoxItem : ContentControl
    {
        #region Public Dependency Properties

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem.IsChecked">IsChecked</see> dependency property.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the CheckedListBox item is checked and when it is false the the item is unchecked .The default value is False.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">Boolean</see>
        /// </returns>
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(CheckedListBoxItem), new PropertyMetadata(false ,  OnIsCheckedChanged));
        

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem.IsSelected">IsSelected</see> dependency property.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the CheckedListBox item is selected and when it is false the the item is unselected.The default value is False.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">Boolean</see>
        /// </returns>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(CheckedListBoxItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBox.CheckOnClick">CheckOnClick</see> dependency property.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the mouse pointer is on the CheckedListBox item .The default value is False.
        /// </remarks>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">Boolean</see>
        /// </returns>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(CheckedListBoxItem), new PropertyMetadata(new PropertyChangedCallback(OnIsMouseOverChanged)));

        /// <summary>
        /// This Property indicates the source for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageSourceProperty =
            DependencyProperty.Register("LeftImageSource", typeof(ImageSource), typeof(CheckedListBoxItem), new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnLeftImageSourceChanged)));

        /// <summary>
        /// This Property indicates the source for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageSourceProperty =
            DependencyProperty.Register("RightImageSource", typeof(ImageSource), typeof(CheckedListBoxItem), new PropertyMetadata(new BitmapImage(), new PropertyChangedCallback(OnRightImageSourceChanged)));

        /// <summary>
        /// This Property indicates the Width for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageWidthProperty =
            DependencyProperty.Register("LeftImageWidth", typeof(double), typeof(CheckedListBoxItem), new PropertyMetadata(15d, new PropertyChangedCallback(OnLeftImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Width for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageWidthProperty =
            DependencyProperty.Register("RightImageWidth", typeof(double), typeof(CheckedListBoxItem), new PropertyMetadata(15d, new PropertyChangedCallback(OnRightImageWidthChanged)));

        /// <summary>
        /// This Property indicates the Height for the Left image of the item.
        /// </summary>
        public static readonly DependencyProperty LeftImageHeightProperty =
            DependencyProperty.Register("LeftImageHeight", typeof(double), typeof(CheckedListBoxItem), new PropertyMetadata(15d, new PropertyChangedCallback(OnLeftImageHeightChanged)));

        /// <summary>
        /// This Property indicates the Height for the Right image of the item.
        /// </summary>
        public static readonly DependencyProperty RightImageHeightProperty =
            DependencyProperty.Register("RightImageHeight", typeof(double), typeof(CheckedListBoxItem), new PropertyMetadata(15d, new PropertyChangedCallback(OnRightImageHeightChanged)));

        /// <summary>
        /// This Property indicates the Vertical Alignment for the image of the item.
        /// </summary>
        public static readonly DependencyProperty ImageVerticalAlignmentProperty =
            DependencyProperty.Register("ImageVerticalAlignment", typeof(VerticalAlignment), typeof(CheckedListBoxItem), new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnImageVerticalAlignmentChanged)));

        /// <summary>
        /// This Property indicates the Margin for the image of the item.
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(CheckedListBoxItem), new PropertyMetadata(new PropertyChangedCallback(OnImageMarginChanged)));


     // public static readonly DependencyProperty IsAllowDragDropProperty = DependencyProperty.Register("IsAllowDragDrop", typeof(bool), typeof(CheckedListBoxItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsAllowDragDropChanged)));


        
        #endregion Public Dependency Properties.

        #region Internal Members
        
        /// <summary>
        /// To check whether selected or not
        /// </summary>
        internal bool selected=false;

        /// <summary>
        /// Template ContentPresenter object
        /// </summary>
        internal ContentPresenter itemcontent;

        /// <summary>
        /// Template Checkbox object
        /// </summary>
        internal CheckBox itemcheckbox;
        /// <summary>
        /// Template RadioBox Object
        /// </summary>
        internal RadioButton itemRadio;
        
        /// <summary>
        /// Template Grid 
        /// </summary>
        internal Grid itemgrid;

        /// <summary>
        /// Selected Rectangle
        /// </summary>
        internal Rectangle selectedcolor;

        /// <summary>
        /// Hover Rectangle 
        /// </summary>
        internal Rectangle hovercolor;

        double horLineAlignment = 0.0;

        /// <summary>
        /// Template Images 
        /// </summary>
        internal Image rightimage, leftimage;

        internal const string ElementRootName = "LayoutRoot";

        #endregion Internal Members
       
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem">CheckedListBoxItem</see> class
        /// </summary>
        public CheckedListBoxItem()
        {
            DefaultStyleKey = typeof(CheckedListBoxItem);
            MouseEnter += this.OnMouseEnter;
            MouseLeave += this.OnMouseLeave;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(CheckedListBoxItem_MouseLeftButtonDown);
        }
        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the CheckedListBoxItem is checked or not.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The default value is False.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Boolean">Boolean</see>
        /// </value>
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CheckedListBoxItem is selected or not.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The default value is False.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Boolean">Boolean</see>
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is over the CheckedListBoxItem or not.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// The default value is False.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Boolean">Boolean</see>
        /// </value>
         public bool IsMouseOver
        {
            get
            {
                return (bool)GetValue(IsMouseOverProperty);
            }

            internal set
            {
                SetValue(IsMouseOverProperty, value);
            }
        }

         /// <summary>
         /// Gets or sets ParentCheckListBox for the item. This is a dependency property.
         /// </summary>
         /// <value>
         /// <para>Type: <see cref="T:Syncfusion.Windows.Tools.Controls.CheckedListBox">CheckedListBox</see> </para>
         /// </value>
         internal CheckedListBox ParentCheckedListBox { get; set; }

         /// <summary>
         /// Gets the parent for CheckedListBoxItem.
         /// </summary>
         /// /// <value>
         /// <para>Type: <see cref="T:Syncfusion.Windows.Tools.Controls.CheckedListBox">CheckedListBox</see> </para>
         /// </value>
        internal CheckedListBox LogicalParent
        {
            get
            {
                this.GetParentItem(this);
                return this.GetParentItem(this) as CheckedListBox;
            }
        }

        /// <summary>
        /// Gets or sets The LeftImageSource Dependency Property
        /// </summary>
        public ImageSource LeftImageSource
        {
            get
            {
                return (ImageSource)GetValue(LeftImageSourceProperty);
            }

            set
            {
                SetValue(LeftImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageSoruce Dependency Property
        /// </summary>
        /// <value>The right image source.</value>
        public ImageSource RightImageSource
        {
            get
            {
                return (ImageSource)GetValue(RightImageSourceProperty);
            }

            set
            {
                SetValue(RightImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LeftImageWidth Dependency Property
        /// </summary>
        /// <value>The width of the left image.</value>
        public double LeftImageWidth
        {
            get
            {
                return (double)GetValue(LeftImageWidthProperty);
            }

            set
            {
                SetValue(LeftImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageWidth Dependency Property
        /// </summary>
        /// <value>The width of the right image.</value>
        public double RightImageWidth
        {
            get
            {
                return (double)GetValue(RightImageWidthProperty);
            }

            set
            {
                SetValue(RightImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LeftImageHeight Dependency Property
        /// </summary>
        /// <value>The height of the left image.</value>
        public double LeftImageHeight
        {
            get
            {
                return (double)GetValue(LeftImageHeightProperty);
            }

            set
            {
                SetValue(LeftImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the RightImageHeight Dependency Property
        /// </summary>
        /// <value>The height of the right image.</value>
        public double RightImageHeight
        {
            get
            {
                return (double)GetValue(RightImageHeightProperty);
            }

            set
            {
                SetValue(RightImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageVerticalAlignment Dependency Property
        /// </summary>
        /// <value>The image vertical alignment.</value>
        public VerticalAlignment ImageVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(ImageVerticalAlignmentProperty);
            }

            set
            {
                SetValue(ImageVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ImageMargin Dependency Property
        /// </summary>
        /// <value>The image margin.</value>
        public Thickness ImageMargin
        {
            get
            {
                return (Thickness)GetValue(ImageMarginProperty);
            }

            set
            {
                SetValue(ImageMarginProperty, value);
            }
        }
        #endregion Public Properties

        #region Public Events
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem.IsChecked">IsChecked</see> is changed.
        /// </summary>
        public event PropertyChangedCallback IsCheckedChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem.IsSelected">IsSelected</see> is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Tools.Controls.CheckedListBoxItem.IsMouseOverChanged">IsMouseOverChanged</see> is changed.
        /// </summary>
        public event PropertyChangedCallback IsMouseOverChanged;

        /// <summary>
        /// Event that is raised when the LeftImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageSourceChanged;

        /// <summary>
        /// Event that is raised when the RightImageSource Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageSourceChanged;

        /// <summary>
        /// Event that is raised when the  LeftImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageWidthChanged;

        /// <summary>
        /// Event that is raised when the RightImageWidth Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageWidthChanged;

        /// <summary>
        /// Event that is raised when the LeftImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback LeftImageHeightChanged;

        /// <summary>
        /// Event that is raised when the RightImageHeight Property is Changed.
        /// </summary>
        public event PropertyChangedCallback RightImageHeightChanged;

        /// <summary>
        /// Event that is raised when the ImageVerticalAlignment Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ImageVerticalAlignmentChanged;

        /// <summary>
        /// Event that is raised when the ImageMargin Property is Changed.
        /// </summary>
        public event PropertyChangedCallback ImageMarginChanged;

        #endregion Public Events

        #region Override Methods

        /// <summary>
        /// Applies the Template for the File Upload control
        /// </summary>
        internal Modes Mode;
       


        private static void OnIsAlloDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem instance = d as CheckedListBoxItem;
            instance.OnIsAllowDragDropChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIsAllowDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            

        }



       
       /// <summary>
       /// 
       /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            itemgrid = this.GetTemplateChild("PART_Grid" ) as Grid;
            itemcontent = this.GetTemplateChild("PART_Content") as ContentPresenter;
            itemRadio = this.GetTemplateChild("PART_Radio") as RadioButton;
            itemcheckbox = this.GetTemplateChild("PART_CheckBox") as CheckBox ;
            selectedcolor = this.GetTemplateChild("fillColor2") as Rectangle;
            rightimage = this.GetTemplateChild("RightImage") as Image;
            leftimage = this.GetTemplateChild("LeftImage") as Image;
            hovercolor = this.GetTemplateChild("fillColor") as Rectangle;

            if ((this.itemcheckbox != null || this.itemRadio != null))
            {
                
                switch (Mode)
                {
                    case Modes.Normal :
                        itemRadio.IsChecked = null;
                        itemcheckbox.IsChecked = null;
                        
                        itemcheckbox.Visibility = Visibility.Collapsed;
                        itemRadio.Visibility = Visibility.Collapsed;
                        break;
                    case Modes.RadioGroup:
                        itemRadio.IsChecked = false;
                        itemcheckbox.Visibility = Visibility.Collapsed;
                        itemRadio.Visibility = Visibility.Visible;
                        break;
                    default:
                        itemcheckbox.IsChecked = false;
                        itemRadio.Visibility = Visibility.Collapsed;
                        itemcheckbox.Visibility = Visibility.Visible;
                        break;
                }
            }           
            
            if (!this.IsEnabled)
            {
                this.Opacity = 0.5;
            }

            if (this.LogicalParent.SelectedItemBackground != null && selectedcolor !=null)
            {
                selectedcolor.Fill = this.LogicalParent.SelectedItemBackground;
            }

            if (this.LogicalParent.MouseOverBackground != null && hovercolor!=null)
            {
                hovercolor.Fill = this.LogicalParent.MouseOverBackground;
            }

            this.itemcheckbox.IsChecked = this.IsChecked;
            this.itemRadio.IsChecked = this.IsChecked;
            if (this.IsSelected && this.IsEnabled)
            {
                if (this.LogicalParent.CheckOnClick)
                {
                    this.itemcheckbox.IsChecked = true;
                    this.itemRadio.IsChecked = true;
                }

                this.UpdateVisualState(true);
            }

            if (this.LogicalParent.CheckBoxStyle != null)
            {
                this.itemcheckbox.Style = this.LogicalParent.CheckBoxStyle;
                this.itemRadio.Style = this.LogicalParent.CheckBoxStyle;

            }

            if (this.Mode == Modes.Checked)
            {
                this.itemcheckbox.Click += new RoutedEventHandler(itemcheckbox_Click);
            }
          if (this.Mode == Modes.RadioGroup)
            {
                this.itemRadio.Click += new RoutedEventHandler(itemRadio_Click);
            }
          
        }

        void itemRadio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton newitem = sender as RadioButton;
            this.IsChecked = newitem.IsChecked;
            if ((this.LogicalParent as CheckedListBox) != null && !(this.LogicalParent as CheckedListBox).m_checkedItemsAddedInternally)
            {
                this.selected = true;
                this.IsSelected = true;
                this.LogicalParent.SelectedItem = this;
                this.LogicalParent.SelectedIndex = this.LogicalParent.Items.IndexOf(this);
                this.itemcheckbox.IsChecked = newitem.IsChecked;
                this.itemRadio.IsChecked = newitem.IsChecked;
            }
        }

        void itemcheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox newitem = sender as CheckBox;
            this.IsChecked = newitem.IsChecked;
            this.IsSelected = true;
            if ((this.LogicalParent as CheckedListBox) != null && !(this.LogicalParent as CheckedListBox).m_checkedItemsAddedInternally)
            {
                this.selected = true ;
                this.IsSelected = true;
                this.LogicalParent.SelectedItem = this;
                this.LogicalParent.SelectedIndex = this.LogicalParent.Items.IndexOf(this);
                this.itemcheckbox.IsChecked = newitem.IsChecked;
                this.itemRadio.IsChecked = newitem.IsChecked;
            }
        }

        void CheckedListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this,"Pressed", true);
            if ((this.LogicalParent as CheckedListBox) != null && !(this.LogicalParent as CheckedListBox).m_checkedItemsAddedInternally)
            {
                this.selected = true;
                this.IsSelected = true;
                this.LogicalParent.SelectedItem = this;
                this.LogicalParent.SelectedIndex = this.LogicalParent.Items.IndexOf(this);                
                    this.itemcheckbox.IsChecked = this.IsChecked;
                    this.itemRadio.IsChecked = this.IsChecked;
            }
        }      


        #endregion Override Method

        #region Public Methods
        /// <summary>
        /// Method to retreive Parent element for a framework element
        /// </summary>
        /// <param name="e">FrameworkElement whose parent element is to be retreived.</param>
        /// <returns>
        /// Returns the Parent framework element.
        /// </returns>
        public static FrameworkElement GetParent(FrameworkElement e)
        {
            if (!(e is UIElement))
            {
                return e.Parent as FrameworkElement;
            }

            Panel parent = e.Parent as Panel;
            if (parent == null)
            {
                return null;
            }
            return parent.Parent as FrameworkElement;
            
        }

        #endregion Public Methods

        #region RoutedEvents
              

        /// <summary>
        /// Invokes when mouse pointer is entered.
        /// </summary>
        /// <param name="sender">Item on which the mouse is pointed.</param>
        /// <param name="e">Mouse pointer details,such as point.</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            if (!this.IsSelected)
            {
                this.UpdateVisualState(true);
            }

            this.Opacity = 0.75;
        }

        /// <summary>
        /// Invokes when mouse pointer is left from the item.
        /// </summary>
        /// <param name="sender">Item from which the mouse is left.</param>
        /// <param name="e">Mouse pointer details,such as point.</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(false);
            this.Opacity = 1;
        }

        #endregion RoutedEvents

        #region Protected Methods

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnIsMouseOverChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsMouseOverChanged != null)
            {
                this.IsMouseOverChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsCheckedChanged != null)
            {
                this.IsCheckedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value and raises event
        /// </summary>
        /// <param name="e">Property change details,such as old value and new value.</param>
        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsSelectedChanged != null)
            {
                this.IsSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftImageSourceChanged != null)
            {
                LeftImageSourceChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageSourceChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightImageSourceChanged != null)
            {
                RightImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightImageWidthChanged != null)
            {
                RightImageWidthChanged(this, e);
            }
        }
       
        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageWidthChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftImageWidthChanged != null)
            {
                LeftImageWidthChanged(this, e);
            }
        }
  
        /// <summary>
        /// Updates property value cache and raises <see cref="RightImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnRightImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (horLineAlignment < (double)e.NewValue)
            {
                horLineAlignment = (double)e.NewValue;
            }

            checkForHorLineMovement(horLineAlignment);
            if (RightImageHeightChanged != null)
            {
                RightImageHeightChanged(this, e);
            }
        }
       
        /// <summary>
        /// Updates property value cache and raises <see cref="LeftImageHeightChanged"/> event.
        /// </summary>
        protected virtual void OnLeftImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (horLineAlignment < (double)e.NewValue)
            {
                horLineAlignment = (double)e.NewValue;
            }

            checkForHorLineMovement(horLineAlignment);

            if (LeftImageHeightChanged != null)
            {
                LeftImageHeightChanged(this, e);
            }
        }
       
        /// <summary>
        /// Updates property value cache and raises <see cref="ImageVerticalAlignmentChanged"/> event.
        /// </summary>
        protected virtual void OnImageVerticalAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageVerticalAlignmentChanged != null)
            {
                ImageVerticalAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageMarginChanged"/> event.
        /// </summary>
        protected virtual void OnImageMarginChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageMarginChanged != null)
            {
                ImageMarginChanged(this, e);
            }
        }
        #endregion Protected Method

        #region Private Methods

        /// <summary>
        /// Calls OnIsMouseOverChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem instance = (CheckedListBoxItem)d;
            instance.UpdateVisualState(true);
            instance.OnIsMouseOverChanged(e);
        }
        
 
        /// <summary>
        /// Calls OnIsCheckedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem instance = (CheckedListBoxItem)d;
            if (e.NewValue != e.OldValue)
            {
                CheckedListBox parent = (instance.LogicalParent as CheckedListBox);
                if (parent != null)
                {
                    if (parent.CheckedItems == null)
                    {
                        parent.CheckedItems = new ObservableCollection<object>();
                        ObservableCollection<object> observableCollection = new ObservableCollection<object>();
                        observableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(parent.OnCheckedItemsCollectionChanged);
                        parent.CheckedItems = observableCollection;
                    }
                    if (instance.IsChecked == true)
                    {
                        if (parent.ItemsSource == null)
                        {
                            if (!(parent.CheckedItems.Contains(instance)))
                                parent.CheckedItems.Add(instance);
                        }
                        else
                        {
                            if (!(parent.CheckedItems.Contains(instance.DataContext)))
                                parent.CheckedItems.Add(instance.DataContext);
                        }
                    }
                    else
                    {
                        if (parent.ItemsSource == null)
                        {
                            if (parent.CheckedItems.Contains(instance))
                                parent.CheckedItems.Remove(instance);
                        }
                        else
                        {
                            if (parent.CheckedItems.Contains(instance.DataContext))
                                parent.CheckedItems.Remove(instance.DataContext);
                        }
                    }
                }

                if (instance.itemcheckbox != null)
                {
                    instance.itemcheckbox.IsChecked = instance.IsChecked;
                }
                if (instance.itemRadio != null)
                {
                    instance.itemRadio.IsChecked = instance.IsChecked;
                }
                instance.OnIsCheckedChanged(e);
            }
        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem instance = (CheckedListBoxItem)d;
            if (e.NewValue != e.OldValue)
            {
                instance.OnIsSelectedChanged(e);
                instance.UpdateVisualState(true);
            }
        }

        /// <summary>
        /// Method to get ParentItem for an element
        /// </summary>
        /// <param name="element">object to which the parent item should be retrieved</param>
        /// <returns>
        /// Returns the Parent CheckedListBox
        /// </returns>
        private CheckedListBox GetParentItem(DependencyObject element)
        {
            if (element != null)
            {
                return VisualUtil.FindAncestor(element, typeof(CheckedListBox)) as CheckedListBox;
            }
            else
            {
                return null;
            }
            //while (!(element is CheckedListBox))
            //{
            //    if (element != null)
            //    {
            //        element = VisualTreeHelper.GetParent(element);
            //    }
            //}
           
            //if (element is CheckedListBox)
            //{
            //    return (CheckedListBox)element;
            //}
            //else
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// Method to set the visual state transition.
        /// </summary>
        /// <param name="useTransitions">Whether to apply transition or not.</param>
        /// <param name="stateNames">Which state transition is to be applied</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method to update VisualState.
        /// </summary>
        /// <param name="useTransitions">Boolean value to know whether the transition is to be applied or not.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "Selected" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Unselected" });
                }

            if (this.IsMouseOver && !this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }
           }

        /// <summary>
        /// Checks for hor line movement.
        /// </summary>
        /// <param name="val">The val.</param>
        private void checkForHorLineMovement(double val)
        {
        }

        /// <summary>
        /// Calls OnImageMarginChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnImageMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnImageMarginChanged(e);
        }

        /// <summary>
        /// Calls OnImageVerticalAlignmentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnImageVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnImageVerticalAlignmentChanged(e);
        }

        /// <summary>
        /// Calls OnLeftImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnLeftImageHeightChanged(e);
        }

        /// <summary>
        /// Calls OnLeftImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnLeftImageWidthChanged(e);
        }

        /// <summary>
        /// Calls OnRightImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnRightImageHeightChanged(e);
        }

        /// <summary>
        /// Calls OnRightImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnRightImageSourceChanged(e);
        }

        /// <summary>
        /// Calls OnRightImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRightImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnRightImageWidthChanged(e);
        }

        /// <summary>
        /// Calls OnLeftImageSourceChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLeftImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckedListBoxItem obj = (CheckedListBoxItem)d;
            obj.OnLeftImageSourceChanged(e);
        }
        #endregion Private Methods
    }
}
