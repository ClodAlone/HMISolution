#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// GroupBarItem control
    /// </summary>
    public class GroupBarItem : ContentControl
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="GroupBarItem"/>
        /// </summary>
        public GroupBarItem()
        {
            this.DefaultStyleKey = typeof(GroupBarItem);          
        }

       

        internal GroupBar _groupBar;
        #endregion

        #region Constants
        private const string HeaderElement = "HighlightHeaderElement";
        private const string ContentContainer = "ContentContainer";
        private const string PanelContent = "PanelContent";
        private const string ItemContentPresenterElement = "ItemContentPresenter";
        private const string HighlightExpandBorderElement = "HighlightExpandBorder";
        private const string HighlightImageElement = "HighlightImage";
        private const string ExpandBorderElement = "ExpandBorder";
        private const string ImageElement = "Image";
        private const string UnderLineElement = "UnderLine";
        private const string HeaderElementTranslateTransformElement = "HighlightHeaderElementTranslateTransform";
        private const string BorderContentElement = "BorderContent";
        private const string HighlightOuterBorderElement = "HighlightOuterBorder";
        private const string OuterBorderElement = "OuterBorder";

        /// <summary>
        /// Width of the image.
        /// </summary>
        private const double DefImageWidth = 16;

        /// <summary>
        /// Height of the image.
        /// </summary>
        private const double DefImageHeight = 16;
        #endregion

        #region Fields
        private FrameworkElement headerElement;
        private Panel contentContainer;
        private FrameworkElement content;
        private bool isMouseEnter;
        private FrameworkElement expandBorderElement;
        private FrameworkElement imageElement;
        private FrameworkElement highlightexpandBorderElement;
        private FrameworkElement highlightimageElement;
        private FrameworkElement underLineElement;
        private FrameworkElement borderContentElement;
        private Border highlightOuterBorderElement;
        internal Grid itemHostGrid = null;
        internal object _element;

        private ContentControl highlightHeaderText;
        private ContentControl headerText;

        private Border outerBorderElement;
        private bool isMouseCaptured = false;
        private bool isDragging = false;
        private Point clickPosition;
        private bool showInGroupBar = true;
        private bool hidden = false;
        private bool isPressed = false;
        private bool isSelected = false;
        private bool isDragPopupCreated = false;
        private Popup DragPopup = new Popup();        
        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBarItem), new PropertyMetadata(null));

        

        /// <summary>
        /// Gets or sets the text to be shown in the header.
        /// </summary>
        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }

            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Header Background
        /// </summary>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Header Foreground
        /// </summary>
        public Brush HeaderForeground
        {
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }

            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Header FontFamily
        /// </summary>
        public FontFamily HeaderFontFamily
        {
            get
            {
                return (FontFamily)GetValue(HeaderFontFamilyProperty);
            }

            set
            {
                SetValue(HeaderFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Header FontSize
        /// </summary>
        public double HeaderFontSize
        {
            get
            {
                return (double)GetValue(HeaderFontSizeProperty);
            }

            set
            {
                SetValue(HeaderFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the HeaderFontWeight
        /// </summary>
        public FontWeight HeaderFontWeight
        {
            get
            {
                return (FontWeight)GetValue(HeaderFontWeightProperty);
            }

            set
            {
                SetValue(HeaderFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical parent as <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/>.
        /// </summary>
        internal GroupBar LogicalParent
        {
            get
            {
                return this.Parent as GroupBar;
            }
        }

        /// <summary>
        /// Gets or sets the source of the image shown in the header.
        /// </summary>
        public ImageSource HeaderImageSource
        {
            get
            {
                return (ImageSource)GetValue(HeaderImageSourceProperty);
            }

            set
            {
                SetValue(HeaderImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the image height.
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }

            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the image width.
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }

            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse enter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse enter; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMouseEnter
        {
            get
            {
                return this.isMouseEnter;
            }

            set
            {
                this.isMouseEnter = value;
            }
        }

        /// <summary>
        /// Gets or sets text horizontal alignment.
        /// </summary>
        public HorizontalAlignment TextHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(TextHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show in group bar].
        /// </summary>
        /// <value><c>true</c> if [show in group bar]; otherwise, <c>false</c>.</value>
        internal bool ShowInGroupBar
        {
            get
            {
                return this.showInGroupBar;
            }

            set
            {
                this.showInGroupBar = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GroupBarItem"/> is hidden.
        /// </summary>
        /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
        internal bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                this.hidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPressed
        {
            get
            {
                return this.isPressed;
            }

            set
            {
                this.isPressed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
                UpdateVisualState();
                if (value == true)
                {
                    UpdateSelection();
                }
            }
        }

        /// <summary>
        /// Gets the highlight outer border.
        /// </summary>
        /// <value>The highlight outer border.</value>
        internal Border HighlightOuterBorder
        {
            get
            {
                return this.highlightOuterBorderElement;
            }
        }

        /// <summary>
        /// Gets the outer border.
        /// </summary>
        /// <value>The outer border.</value>
        internal Border OuterBorder
        {
            get
            {
                return this.outerBorderElement;
            }
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="IsExpandedProperty">IsExpanded</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(GroupBarItem), new PropertyMetadata(new PropertyChangedCallback(IsExpandedChanged)));

        /// <summary>
        /// Identifies <see cref="HeaderForegroundProperty">HeaderForeground</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderBackgroundProperty">HeaderBackground</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderFontFamilyProperty">HeaderFontFamily</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderFontSizeProperty">HeaderFontSize</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderFontWeightProperty">HeaderFontWeight</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderImageSourceProperty">HeaderText</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="HeaderImageSourceProperty">HeaderImageSource</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderImageSourceProperty =
            DependencyProperty.Register("HeaderImageSource", typeof(ImageSource), typeof(GroupBarItem), null);

        /// <summary>
        /// Identifies <see cref="ImageHeightProperty">ImageHeight</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(GroupBarItem), new PropertyMetadata(DefImageHeight, new PropertyChangedCallback(IsImageHeightChanged)));

        /// <summary>
        /// Identifies <see cref="ImageWidthProperty">ImageWidth</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(GroupBarItem), new PropertyMetadata(DefImageWidth, new PropertyChangedCallback(IsImageWidthChanged)));

        /// <summary>
        /// Identifies <see cref="TextHorizontalAlignmentProperty">TextHorizontalAlignment</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty TextHorizontalAlignmentProperty =
            DependencyProperty.Register("TextHorizontalAlignment", typeof(HorizontalAlignment), typeof(GroupBarItem), null);

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="T:Syncfusion.Windows.Tools.Controls.GroupBarItem.Expanded"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ExpandedChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupBarItem.ImageWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupBarItem.ImageHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="T:Syncfusion.Windows.Tools.Controls.GroupBarItem.BorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="T:Syncfusion.Windows.Tools.Controls.GroupBarItem.BorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderThicknessChanged;

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LoadTemplateChildren();            
        }

        /// <summary>
        /// Loads the template children.
        /// </summary>
        internal void LoadTemplateChildren()
        {
            this.headerElement = GetTemplateChild(HeaderElement) as FrameworkElement;
            this.contentContainer = GetTemplateChild(ContentContainer) as Panel;
            this.content = GetTemplateChild(PanelContent) as FrameworkElement;
            this.expandBorderElement = GetTemplateChild(ExpandBorderElement) as FrameworkElement;
            this.imageElement = GetTemplateChild(ImageElement) as FrameworkElement;
            this.highlightexpandBorderElement = GetTemplateChild(HighlightExpandBorderElement) as FrameworkElement;
            this.highlightimageElement = GetTemplateChild(HighlightImageElement) as FrameworkElement;
            this.underLineElement = GetTemplateChild(UnderLineElement) as FrameworkElement;
            this.borderContentElement = GetTemplateChild(BorderContentElement) as FrameworkElement;
            this.highlightOuterBorderElement = GetTemplateChild(HighlightOuterBorderElement) as Border;
            this.outerBorderElement = GetTemplateChild(OuterBorderElement) as Border;
            this.itemHostGrid = GetTemplateChild("ItemHost") as Grid;
            this.highlightHeaderText = GetTemplateChild("HighlightHeaderText") as ContentControl;
            this.headerText = GetTemplateChild("HeaderText") as ContentControl;
            
            if (_element != null)
            {
                if(this.highlightHeaderText != null)
                this.highlightHeaderText.Content = _element;
                if(this.headerText != null)
                this.headerText.Content = _element;               
            }
            

            if (this.expandBorderElement != null && this._groupBar!= null && this._groupBar.VisualMode == VisualMode.MultipleExpansion)
            {
                this.expandBorderElement.Visibility = Visibility.Visible;
                this.imageElement.Visibility = Visibility.Collapsed;

                this.highlightexpandBorderElement.Visibility = Visibility.Visible;
                this.highlightimageElement.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (imageElement != null)
                {
                    this.imageElement.Visibility = Visibility.Visible;
                }

                if (expandBorderElement != null)
                {

                    this.expandBorderElement.Visibility = Visibility.Collapsed;
                }

                if (highlightimageElement != null)
                {

                    this.highlightimageElement.Visibility = Visibility.Visible;
                }
                if (highlightexpandBorderElement != null)
                {

                    this.highlightexpandBorderElement.Visibility = Visibility.Collapsed;
                }
            }

            if (this.highlightOuterBorderElement != null)
            {
                this.highlightOuterBorderElement.MouseLeave += new MouseEventHandler(highlightOuterBorderElement_MouseLeave);
            }

            if (this.outerBorderElement != null)
            {
                this.outerBorderElement.MouseEnter += new MouseEventHandler(outerBorderElement_MouseEnter);
            }

            if (this.headerElement != null)
            {
                this.headerElement.MouseLeftButtonDown -= new MouseButtonEventHandler(this.HeaderElementMouseLeftButtonDown);
                this.headerElement.MouseLeftButtonUp -= new MouseButtonEventHandler(this.HeaderElementMouseLeftButtonUp);
                this.headerElement.MouseEnter -= new MouseEventHandler(this.HeaderElementMouseEnter);
                this.headerElement.MouseLeave -= new MouseEventHandler(this.HeaderElementMouseLeave);
                this.headerElement.MouseMove -= new MouseEventHandler(this.HeaderElementMouseMove);

                this.headerElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.HeaderElementMouseLeftButtonDown);
                this.headerElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.HeaderElementMouseLeftButtonUp);
                this.headerElement.MouseEnter += new MouseEventHandler(this.HeaderElementMouseEnter);
                this.headerElement.MouseLeave += new MouseEventHandler(this.HeaderElementMouseLeave);
                this.headerElement.MouseMove += new MouseEventHandler(this.HeaderElementMouseMove);
            }

            if (this.IsExpanded)
            {
                if (this._groupBar.VisualMode != VisualMode.MultipleExpansion)
                {
                    foreach (GroupBarItem item in this._groupBar.Items)
                    {
                        if (item != this)
                        {
                            item.IsPressed = false;
                            item.IsExpanded = false;
                            item.IsSelected = false;
                            item.UpdateVisualState();
                        }
                    }
                    if (highlightexpandBorderElement != null)
                    {
                        this.highlightOuterBorderElement.Visibility = Visibility.Visible;
                    }
                    if (outerBorderElement != null)
                    {
                        this.outerBorderElement.Visibility = Visibility.Collapsed;
                    }
                    this.SetExpanded(this._groupBar);
                }
                else
                {
                    if (this.IsExpanded)
                    {
                        if (this.highlightOuterBorderElement != null && this.outerBorderElement != null)
                        {
                            this.highlightOuterBorderElement.Visibility = Visibility.Visible;
                            this.outerBorderElement.Visibility = Visibility.Collapsed;
                        }

                        this.SetExpanded(this.LogicalParent);
                    }
                    else if (this._groupBar != null && this._groupBar.VisualMode == VisualMode.MultipleExpansion)
                    {
                        this.CollapseContent();
                        this.UnSelectItem();
                        this.UpdateVisualState();
                    }
                }
            }

            this.UpdateVisualState();
        }
        #endregion

        #region Implementation

        internal void UpdateSelection()
        {
            if (this._groupBar != null)
            {
                foreach (var itm in this._groupBar.Items)
                {
                    GroupBarItem item = itm as GroupBarItem;
                    if (item == null)
                        item = this._groupBar.ItemContainerGenerator.ContainerFromItem(itm) as GroupBarItem;
                    if (item != null && item != this)
                        item.IsSelected = false;
                }
            }
        }
        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        internal virtual void UpdateVisualState()
        {
            if (this.IsMouseEnter && !this.IsPressed)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else if (!this.IsMouseEnter && !this.IsPressed)
            {
                if (this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "Selected", true);
                }
                else 
                {
                    VisualStateManager.GoToState(this,"UnSelected",true);
                }
                //else
                //{
                //    VisualStateManager.GoToState(this, "MouseOver", true);
                //    VisualStateManager.GoToState(this, "MouseOut", true);                    
                    if (!this.IsExpanded)
                    {
                        VisualStateManager.GoToState(this, "Collapsed", true);
                    }
                //}
            }

            else if (this.IsPressed)
            {
                if (this.IsExpanded)
                {
                    VisualStateManager.GoToState(this, "Expanded", true);
                }

                VisualStateManager.GoToState(this, "Selected", true);
            }
            else if (!this.IsPressed && this.IsMouseEnter)
            {
                VisualStateManager.GoToState(this, "UnSelected", true);

                if (!this.IsExpanded)
                {
                    VisualStateManager.GoToState(this, "Collapsed", true);
                }

                if (this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "Selected", true);
                }
            }

            
        }

        /// <summary>
        /// Uns the select item.
        /// </summary>
        internal void UnSelectItem()
        {
            VisualStateManager.GoToState(this, "UnSelected", true);
            VisualStateManager.GoToState(this, "Collapsed", true);
        }

        /// <summary>
        /// Handles the MouseEnter event of the outerBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void outerBorderElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.highlightOuterBorderElement.Visibility = Visibility.Visible;
            this.outerBorderElement.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the MouseLeave event of the highlightOuterBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void highlightOuterBorderElement_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsSelected)
            {
                this.highlightOuterBorderElement.Visibility = Visibility.Collapsed;
                this.outerBorderElement.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Invoked when the mouse enters the header.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains information about the cursor position</param>
        private void HeaderElementMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseEnter = true;
            this.UpdateVisualState();
        }

        /// <summary>
        /// Invoked when the mouse leaves the header.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains information about the cursor position</param>
        private void HeaderElementMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseEnter = false;
            this.UpdateVisualState();
        }

        /// <summary>
        /// Invoked when the mouse leaves the header.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains information about the cursor position</param>
        private void HeaderElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.isDragging)
            {
                if (_groupBar.VisualMode == VisualMode.MultipleExpansion)
                {
                    this.IsExpanded = !this.IsExpanded;
                }
                _groupBar.SelectedItem = this;
                this.ChangeItemExpandMode(_groupBar);
            }
            else
            {
                Point point = e.GetPosition(this);
                Point mousePoint = e.GetPosition(sender as Grid);
                double deltaY = point.Y;
                (sender as Grid).Opacity = 1;
                DragDirection m_dragDirection = deltaY > 0 ? DragDirection.Down : DragDirection.Up;

                double absDeltaY = Math.Abs(deltaY);

                if (m_dragDirection == DragDirection.Up)
                {
                    for (int i = 0; i < this._groupBar.Items.Count; i++)
                    {
                        GroupBarItem it = null;

                        if (!(_groupBar.VisualMode == VisualMode.StackMode) && _groupBar.nonStackGridElement != null)
                        {
                            it = _groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }
                        else if (_groupBar.VisualMode == VisualMode.StackMode && _groupBar.stackGridElement != null)
                        {
                            it = _groupBar.stackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }

                        Point itempoint = e.GetPosition(it);

                        if (mousePoint.Y > itempoint.Y - this.clickPosition.Y)
                        {
                            GroupBar parent = this._groupBar;
                            GroupBarItem item = parent.DragItem;
                            parent.ReplaceItem(item, i);
                            item.IsSelected = true;
                            item.IsPressed = true;
                            item.UpdateVisualState();
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = this._groupBar.Items.Count - 1; i > 0; i--)
                    {
                        GroupBarItem it = null;

                        if (!(_groupBar.VisualMode == VisualMode.StackMode) && _groupBar.nonStackGridElement != null)
                        {
                            it = _groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }
                        else if (_groupBar.VisualMode == VisualMode.StackMode && _groupBar.stackGridElement != null)
                        {
                            it = _groupBar.stackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }

                        Point itempoint = e.GetPosition(it);

                        if (mousePoint.Y < itempoint.Y + this.clickPosition.Y)
                        {
                            GroupBar parent = this._groupBar;
                            GroupBarItem item = parent.DragItem;
                            parent.ReplaceItem(item, i);
                            item.IsSelected = true;
                            item.IsPressed = true;
                            item.UpdateVisualState();
                            break;
                        }
                    }
                }

                GroupBar parentGroupBar = this._groupBar;

                if (parentGroupBar.VisualMode == VisualMode.StackMode)
                {
                    for (int i = 0; i < parentGroupBar.Items.Count; i++)
                    {
                        GroupBarItem it = null;

                        if (!(_groupBar.VisualMode == VisualMode.StackMode) && _groupBar.nonStackGridElement != null)
                        {
                            it = _groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }
                        else if (_groupBar.VisualMode == VisualMode.StackMode && _groupBar.stackGridElement != null)
                        {
                            it = _groupBar.stackGridElement.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        }

                        if (it!= null && it.Hidden)
                        {
                            it.Hidden = false;
                            parentGroupBar.StackGrid.HideItem(i);
                        }
                    }
                }

                TranslateTransform headerElementTranslateTransform = GetTemplateChild(HeaderElementTranslateTransformElement) as TranslateTransform;
                if (headerElementTranslateTransform != null)
                {
                    headerElementTranslateTransform.X = 0;
                    headerElementTranslateTransform.Y = 0;
                }
            }

            DragPopup.IsOpen = false;
            (sender as Grid).ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
            this.isDragging = false;
            this.isMouseCaptured = false;
            this.isDragPopupCreated = false;
            this.UpdateVisualState();
        }

        internal void Expand()
        {
            if (_groupBar.VisualMode == VisualMode.MultipleExpansion)
            {
                this.IsExpanded = !this.IsExpanded;
                _groupBar.SelectedItem = this;
                this.ChangeItemExpandMode(_groupBar);
            }
            else
            {
                _groupBar.SelectedItem = this;
            }
        }

        /// <summary>
        /// Changes the item expand mode.
        /// </summary>
        internal void ChangeItemExpandMode(GroupBar LocalGroupBar)
        {
            if (LocalGroupBar != null)
            {
                foreach (var item in LocalGroupBar.Items)
                {                
                    //GroupBarItem it = null;

                    //if(_groupBar.VisualMode == VisualMode.StackMode)
                    //{
                    //    it = _groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    //}
                    //else
                    //{
                    //    it = _groupBar.stackGrid.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    //}

                    GroupBarItem it = null;

                    if (!(LocalGroupBar.VisualMode == VisualMode.StackMode) && LocalGroupBar.nonStackGridElement != null)
                    {
                        it = LocalGroupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    }
                    else if (LocalGroupBar.VisualMode == VisualMode.StackMode && LocalGroupBar.stackGridElement != null)
                    {
                        it = LocalGroupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    }

                    if (it != null && it != this && it.highlightOuterBorderElement != null && it.outerBorderElement != null)
                    {
                        it.IsPressed = false;
                        it.IsSelected = false;
                        it.UpdateVisualState();

                        it.highlightOuterBorderElement.Visibility = Visibility.Collapsed;
                        it.outerBorderElement.Visibility = Visibility.Visible;
                    }
                }
            }

            if (LocalGroupBar.VisualMode == VisualMode.StackMode && !this.IsExpanded)
            {
                this.IsExpanded = !this.IsExpanded;
            }
            else if (LocalGroupBar.VisualMode == VisualMode.MultipleExpansion)
            {
                foreach (var gbItem in LocalGroupBar.Items)
                {
                    //GroupBarItem item = null;
                    //if (LocalGroupBar.VisualMode == VisualMode.StackMode)
                    //{
                    //    item = LocalGroupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    //}
                    //else
                    //{
                    //    item = LocalGroupBar.stackGrid.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    //}

                    GroupBarItem item = null;

                    if (!(LocalGroupBar.VisualMode == VisualMode.StackMode) && LocalGroupBar.nonStackGridElement != null)
                    {
                        item = LocalGroupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    }
                    else if (LocalGroupBar.VisualMode == VisualMode.StackMode && LocalGroupBar.stackGridElement != null)
                    {
                        item = LocalGroupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    }

                    if (item != null && !item.IsExpanded)
                    {
                        item.IsPressed = false;
                        item.IsSelected = false;
                        item.UpdateVisualState();
                    }
                }

                //this.IsExpanded = !this.IsExpanded;

                //if (!this.IsExpanded)
                //{
                    this.SetExpanded(LocalGroupBar);
                //}
            }
            else
            {
                if (!this.IsExpanded)
                {
                    this.IsExpanded = !this.IsExpanded;
                }
            }

            if (LocalGroupBar.VisualMode != VisualMode.MultipleExpansion)
            {
                this.SetExpanded(LocalGroupBar);
            }
        }


        /// <summary>
        /// Invoked when the left mouse button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains information about the cursor position</param>
        private void HeaderElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.clickPosition = e.GetPosition(sender as Grid);
            (sender as Grid).CaptureMouse();

            //foreach (GroupBarItem item in this.LogicalParent.Items)
            //{
            //    if (item != this)
            //    {
            //        item.SetValue(Canvas.ZIndexProperty, 0);
            //    }
            //    else
            //    {
            //        item.contentContainer.SetValue(Canvas.ZIndexProperty, -1);
            //        item.SetValue(Canvas.ZIndexProperty, 1);
            //    }
            //}

            this.IsPressed = true;
            this.IsSelected = true;
            this.UpdateVisualState();

            if (this._groupBar != null)
            {
                foreach (var item in this._groupBar.Items)
                {
                    GroupBarItem gbarItem = null;
                    if (!(this._groupBar.VisualMode == VisualMode.StackMode) && this._groupBar.nonStackGridElement != null)
                    {
                        gbarItem = this._groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    }
                    else if (this._groupBar.VisualMode == VisualMode.StackMode && this._groupBar.stackGridElement != null)
                    {
                        gbarItem = this._groupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(item) as GroupBarItem;
                    }
                    
                    if (gbarItem!= null && gbarItem != this)
                    {
                        gbarItem.IsPressed = false;
                    }
                }
            }

            this.isMouseCaptured = true;
        }

        /// <summary>
        /// Invoked when the coordinate position of the mouse changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains information about the cursor position</param>
        private void HeaderElementMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseCaptured && this._groupBar != null && this._groupBar.EnableItemsDragging)
            {
                this.headerElement.Opacity = 0.01;
                if (!this.isDragPopupCreated)
                {
                    this.CreatePopup();
                }

                if (!DragPopup.IsOpen)
                {
                    DragPopup.IsOpen = true;
                }

                double x = e.GetPosition(null).X - this.clickPosition.X;
                double y = e.GetPosition(null).Y - this.clickPosition.Y;

                if (this._groupBar.Orientation == Orientation.Vertical)
                {
                    DragPopup.VerticalOffset = y;
                    DragPopup.HorizontalOffset = x;
                }
                else
                {
                    DragPopup.VerticalOffset = x;
                    DragPopup.HorizontalOffset = -y;
                }

                TranslateTransform headerElementTranslateTransform = GetTemplateChild(HeaderElementTranslateTransformElement) as TranslateTransform;

                x = e.GetPosition(this).X - this.clickPosition.X;
                y = e.GetPosition(this).Y - this.clickPosition.Y;
                if (headerElementTranslateTransform != null)
                {
                    headerElementTranslateTransform.X = x;
                    headerElementTranslateTransform.Y = y;
                }
                this.isDragging = true;
                this.Cursor = Cursors.Hand;
                this._groupBar.DragItem = this;
            }
        }

        /// <summary>
        /// Collapse item.
        /// </summary>
        internal void CollapseContent()
        {
            if (this._groupBar != null && this.contentContainer != null)
            {
                //this.contentContainer.Children.Remove(this.Content as UIElement);
                this.contentContainer.Visibility = Visibility.Collapsed;
                this.borderContentElement.Visibility = Visibility.Collapsed;
                this.IsPressed = false;
                this.IsExpanded = false;
            }
        }

        /// <summary>
        /// Expand item.
        /// </summary>
        internal void SetExpanded(GroupBar LocalGroupBar)
        {
            if (LocalGroupBar != null && this.Content != null)
            {
                if (LocalGroupBar.VisualMode == VisualMode.MultipleExpansion)
                {
                    if (this.IsExpanded)
                    {
                        //this.contentContainer.Children.Add(this.Content as UIElement);
                        if (this.Content != null && this.Content is FrameworkElement)
                        {
                            (this.Content as FrameworkElement).HorizontalAlignment = this.HorizontalContentAlignment;
                            (this.Content as FrameworkElement).VerticalAlignment = this.VerticalContentAlignment;
                        }
                        if(this.contentContainer!=null)
                            this.contentContainer.Visibility = Visibility.Visible;
                        if(this.borderContentElement!=null)
                            this.borderContentElement.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //this.contentContainer.Children.Remove(this.Content as UIElement);
                        this.contentContainer.Visibility = Visibility.Collapsed;
                        this.borderContentElement.Visibility = Visibility.Collapsed;
                    }

                    this.IsPressed = this.IsExpanded;
                    this.IsSelected = true;
                    if (this.IsExpanded)
                    {
                        this.UnSelectItem();
                        this.UpdateVisualState();
                    }
                    else
                    {
                        this.UnSelectItem();
                    }
                }
                else if (LocalGroupBar.VisualMode == VisualMode.StackMode)
                {
                    GroupBar parent = LocalGroupBar;
                    this.IsPressed = true;                    

                    if (parent.IsNavPaneMode)
                    {
                        parent.StackGrid.CollapseNavPane();
                    }

                    StackGrid parentStackGrid = (StackGrid)parent.StackModeHeaderGrid;
                    Grid itemGrid = null;
                    ContentControl headerText= null;
                        Grid mainHost= null;
                        Border itemBorder = null;
                    if (parentStackGrid != null)
                    {
                        itemGrid = (Grid)parentStackGrid.ItemGrid;
                        //headerText = (TextBlock)parentStackGrid.HeaderTextElement;
                        headerText= (ContentControl)parentStackGrid.HeaderTextElement;
                        mainHost = (Grid)parentStackGrid.MainHostGrid;
                        itemBorder = (Border)parentStackGrid.ItemBorder;
                    }

                    if (itemGrid != null)
                    {
                        itemGrid.Children.Clear();

                        if (this.Content is UIElement)
                        {
                            itemGrid.Children.Add(this.Content as UIElement);
                        }
                        else
                        {
                            ContentControl con = new ContentControl();
                            con.Content = this.Content;
                            con.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            con.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                            con.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                            con.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                            con.ContentTemplate = this.ContentTemplate;
                            itemGrid.Children.Add(con);
                        }
                        itemGrid.HorizontalAlignment = this.HorizontalContentAlignment;
                        itemGrid.VerticalAlignment = this.VerticalContentAlignment;
                        if (this._element == null)
                        {
                            headerText.Content = this.HeaderText;
                        }
                        else
                        {
                            headerText.Content = _element;
                            headerText.ContentTemplate = HeaderTemplate;
                        }
                        if (this.headerText != null)
                        {
                            if (this.headerText.Foreground == null) headerText.Foreground = this.HeaderForeground;
                            if (this.headerText.FontFamily == null) headerText.FontFamily = this.HeaderFontFamily;
                            if (this.headerText.FontSize == 0.0) headerText.FontSize = this.HeaderFontSize;
                            if (this.headerText.FontWeight == null) headerText.FontWeight = this.HeaderFontWeight;
                            if (mainHost.Background != null) mainHost.Background = this.Background;
                            parentStackGrid.LocalParent.SelectedItem = this;
                        }
                    }

                    if (this.borderContentElement != null && itemBorder != null && !parent.IsNavPaneMode)
                    {
                        itemBorder.BorderBrush = (this.borderContentElement as Border).BorderBrush;
                        itemBorder.BorderThickness = (this.borderContentElement as Border).BorderThickness;
                    }

                    if (parent.StackModeHeaderGrid != null)
                    {
                        (parent.StackModeHeaderGrid as StackGrid).HeaderBackground = this.HeaderBackground;
                    }

                    this.UnSelectItem();
                    this.UpdateVisualState();
                }
                else
                {
                    foreach (var gbItem in LocalGroupBar.Items)
                    {
                        GroupBarItem item = null;

                        if(!(LocalGroupBar.VisualMode == VisualMode.StackMode) && LocalGroupBar.nonStackGridElement != null)
                        {
                            item = LocalGroupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                        }
                        else if (LocalGroupBar.VisualMode == VisualMode.StackMode && LocalGroupBar.stackGridElement != null)
                        {
                            item = LocalGroupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                        }

                        if (item != null && item != this && item.contentContainer != null)
                        {
                            //item.contentContainer.Children.Clear();
                            item.contentContainer.Visibility = Visibility.Collapsed;
                            item.borderContentElement.Visibility = Visibility.Collapsed;
                            item.IsExpanded = false;
                            item.IsPressed = false;
                            item.IsSelected = false;
                            item.UnSelectItem();
                        }
                    }

                    if (this.IsExpanded)
                    {
                        //this.contentContainer.Children.Clear();
                        this.contentContainer.Visibility = Visibility.Visible;
                        this.borderContentElement.Visibility = Visibility.Visible;
                        //this.contentContainer.Children.Add(this.Content as UIElement);
                        this.IsPressed = true;
                    }

                    this.UnSelectItem();
                    this.UpdateVisualState();
                }
            }

            if (LocalGroupBar != null)
            {
                this.RotateGroupBarItem(LocalGroupBar);
            }

            this.UpdateLayout();
        }

        internal bool rotateFlag=false;

        /// <summary>
        /// Rotate item.
        /// </summary>
        internal void RotateGroupBarItem(GroupBar LocalGroupBar)
        {
            Grid itemHost = this.GetTemplateChild("ContentContainer") as Grid;
            Border itemHostBorder = this.GetTemplateChild("OuterBorder") as Border;
            double borderHeight=0;
            if (itemHostBorder != null)
            {
                if (itemHostBorder.ActualHeight != 0)
                    borderHeight = itemHostBorder.ActualHeight;
                else
                    borderHeight = 24;
            }
            if (itemHost != null && LocalGroupBar!=null)
            {
                if (LocalGroupBar.Orientation == Orientation.Horizontal)
                {
                    rotateFlag = true;
                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 90;
                    FrameworkElement content = (FrameworkElement)this.Content;

                    if (content != null)
                    {
                        content.HorizontalAlignment = HorizontalAlignment.Stretch;
                        content.VerticalAlignment = VerticalAlignment.Stretch;
                        content.RenderTransform = rotateTransform;
                        content.RenderTransformOrigin = new Point(0.5, 0.5);
                    }
                    if (LocalGroupBar.VisualMode != VisualMode.StackMode)
                    {
                        if (IsExpanded)
                        {
                            if (LocalGroupBar.FitContent)
                            {
                                itemHost.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                                itemHost.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                                content.Width = itemHost.ActualHeight;
                            }
                            else
                            {
                                if (!double.IsNaN(LocalGroupBar.Height) && !double.IsNaN(LocalGroupBar.Width))
                                {
                                    itemHost.RowDefinitions[0].Height = new GridLength((LocalGroupBar.Height - 20) - (borderHeight * LocalGroupBar.Items.Count));
                                    itemHost.ColumnDefinitions[0].Width = new GridLength(LocalGroupBar.Width - 20);
                                    content.Width = (double)((itemHost.RowDefinitions[0].Height).Value);
                                }
                                else if (LocalGroupBar.ActualHeight != 0 && LocalGroupBar.ActualWidth != 0)
                                {
                                    itemHost.RowDefinitions[0].Height = new GridLength((LocalGroupBar.ActualHeight - 20) - (borderHeight * LocalGroupBar.Items.Count));
                                    itemHost.ColumnDefinitions[0].Width = new GridLength(LocalGroupBar.ActualWidth - 20);
                                    content.Width = (double)((itemHost.RowDefinitions[0].Height).Value);
                                }
                            }
                        }
                        else
                        {
                            itemHost.RowDefinitions[0].Height = new GridLength(0);
                            itemHost.ColumnDefinitions[0].Width = new GridLength(0);
                        }
                    }
                    else
                    {
                        Grid itemGrid = (Grid)((StackGrid)LocalGroupBar.StackModeHeaderGrid).ItemGrid;
                        if (itemGrid != null)
                        {
                            itemGrid.ColumnDefinitions[0].Width = new GridLength(LocalGroupBar.ActualWidth - 20);
                            itemGrid.RowDefinitions[0].Height = new GridLength(LocalGroupBar.ActualHeight - 20);
                        }
                    }
                }
                else
                {
                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 0;
                    FrameworkElement content = null;
                    if (this.Content != null && this.Content is FrameworkElement)
                        content = (FrameworkElement)this.Content;

                    if (content != null)
                    {
                        content.HorizontalAlignment = this.HorizontalContentAlignment;
                        content.VerticalAlignment = this.VerticalContentAlignment;
                        content.RenderTransformOrigin = new Point(0.5, 0.5);
                        content.RenderTransform = rotateTransform;
                    }

                    GridLength gridSize = new GridLength(1, GridUnitType.Star);

                    if (LocalGroupBar.VisualMode != VisualMode.StackMode)
                    {
                        if (this.IsExpanded)
                        {
                            if (LocalGroupBar.FitContent)
                            {
                                itemHost.RowDefinitions[0].Height = gridSize;
                                itemHost.ColumnDefinitions[0].Width = gridSize;
                                if (rotateFlag)
                                    content.Width = double.NaN;
                            }
                            else
                            {
                                GroupBar parent = this._groupBar as GroupBar;
                                if (parent != null)
                                {
                                    double itemrowhght = 0.0;
                                    if (!double.IsNaN(LocalGroupBar.Height) && !double.IsNaN(LocalGroupBar.Width))
                                    {                                        
                                        itemrowhght=(LocalGroupBar.Height - 20) - (borderHeight * parent.Items.Count);
                                        if (itemrowhght < 0)
                                        {
                                            itemrowhght = (LocalGroupBar.Height) - (borderHeight * parent.Items.Count);
                                            itemHost.RowDefinitions[0].MinHeight = itemrowhght > 0 ? itemrowhght : Math.Abs(itemrowhght);
                                            itemHost.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);                                            
                                        }
                                        else
                                        {
                                            itemHost.RowDefinitions[0].MinHeight = itemrowhght;
                                            itemHost.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                                        }
                                        itemHost.ColumnDefinitions[0].Width = gridSize;
                                        if (rotateFlag)
                                            content.Width = double.NaN;
                                    }
                                    else if (LocalGroupBar.ActualHeight != 0 && LocalGroupBar.ActualWidth != 0)
                                    {
                                        itemrowhght = (LocalGroupBar.ActualHeight - 20) - (borderHeight * parent.Items.Count);
                                        if (itemrowhght < 0)
                                        {
                                            itemrowhght = (LocalGroupBar.ActualHeight) - (borderHeight * parent.Items.Count);
                                            itemHost.RowDefinitions[0].MinHeight = itemrowhght > 0 ? itemrowhght : Math.Abs(itemrowhght);
                                            itemHost.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);                                            
                                        }
                                        else
                                        {
                                            itemHost.RowDefinitions[0].MinHeight = itemrowhght;
                                            itemHost.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                                        }
                                        itemHost.ColumnDefinitions[0].Width = gridSize;
                                        if (rotateFlag)
                                            content.Width = double.NaN;
                                    }
                                }
                            }
                        }
                        else
                        {
                            itemHost.RowDefinitions[0].Height = new GridLength(0);
                            itemHost.ColumnDefinitions[0].Width = new GridLength(0);
                        }
                        rotateFlag = false;
                    }
                    else
                    {
                        Grid itemGrid = null;
                        if (LocalGroupBar.StackModeHeaderGrid != null)
                        {
                            itemGrid = (Grid)(LocalGroupBar.StackModeHeaderGrid as StackGrid).ItemGrid;
                        }

                        if (itemGrid != null)
                        {
                            itemGrid.ColumnDefinitions[0].Width = gridSize;
                            itemGrid.RowDefinitions[0].Height = gridSize;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="GroupBarItem.ImageWidthChanged">ImageWidthChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void IsImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ImageWidthChanged != null)
            {
                this.ImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="GroupBarItem.ImageWidthChanged">ImageHeightChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void IsImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ImageHeightChanged != null)
            {
                this.ImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="GroupBarItem.ImageWidthChanged">ExpandedChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void IsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ExpandedChanged != null)
            {
                this.ExpandedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="GroupBarItem.BorderBrushChanged">BorderBrushChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void IsBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.BorderBrushChanged != null)
            {
                this.BorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see
        /// cref="GroupBarItem.BorderThicknessChanged">BorderThicknessChanged</see> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void IsBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.BorderThicknessChanged != null)
            {
                this.BorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Calls IsImageHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="item" >Current GroupBarItem Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void IsImageHeightChanged(DependencyObject item, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = item as GroupBarItem;
            groupBarItem.IsImageHeightChanged(e);
        }

        /// <summary>
        /// Calls IsImageWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="item" >Current GroupBarItem Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void IsImageWidthChanged(DependencyObject item, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = item as GroupBarItem;
            groupBarItem.IsImageWidthChanged(e);
        }

        /// <summary>
        /// Calls IsExpandedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="item" >Current GroupBarItem Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void IsExpandedChanged(DependencyObject item, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = item as GroupBarItem;

            if (groupBarItem._groupBar != null && groupBarItem.IsExpanded)
            {
                foreach (var gbItem in groupBarItem._groupBar.Items)
                {
                    //GroupBarItem it = groupBarItem._groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;

                    GroupBarItem it = null;

                    if (!(groupBarItem._groupBar.VisualMode == VisualMode.StackMode) && groupBarItem._groupBar.nonStackGridElement != null)
                    {
                        it = groupBarItem._groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    }
                    else if (groupBarItem._groupBar.VisualMode == VisualMode.StackMode && groupBarItem._groupBar.stackGridElement != null)
                    {
                        it = groupBarItem._groupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                    }

                    if (it!= null && it != groupBarItem)
                    {
                        it.IsPressed = false;
                        it.IsSelected = false;
                        it.UpdateVisualState();
                    }
                }
            }

            if (groupBarItem.IsExpanded)
            {
                if (groupBarItem.highlightOuterBorderElement != null && groupBarItem.outerBorderElement != null)
                {
                    groupBarItem.highlightOuterBorderElement.Visibility = Visibility.Visible;
                    groupBarItem.outerBorderElement.Visibility = Visibility.Collapsed;
                }

                groupBarItem.SetExpanded(groupBarItem.LogicalParent);
            }
            else if (groupBarItem._groupBar != null && groupBarItem._groupBar.VisualMode == VisualMode.MultipleExpansion)
            {
                groupBarItem.CollapseContent();
                groupBarItem.UnSelectItem();
                groupBarItem.UpdateVisualState();
            }

            groupBarItem.IsExpandedChanged(e);
        }

        /// <summary>
        /// Calls IsBorderBrushChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="item" >Current GroupBarItem Control Instance</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void IsBorderBrushChanged(DependencyObject item, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = item as GroupBarItem;
            groupBarItem.IsBorderBrushChanged(e);
        }

        /// <summary>
        /// Determines whether [is border thickness changed] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsBorderThicknessChanged(DependencyObject item, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupBarItem = item as GroupBarItem;
            groupBarItem.IsBorderThicknessChanged(e);
        }

        /// <summary>
        /// Creates the popup.
        /// </summary>
        private void CreatePopup()
        {
            Grid tempGrid = new Grid();
            if (this.IsSelected)
            {
                tempGrid.Background = this.HeaderBackground;
            }
            else
            {
                tempGrid.Background = this.HeaderBackground;
            }
            tempGrid.Width = this.headerElement.ActualWidth;
            tempGrid.Height = this.headerElement.ActualHeight;

            ColumnDefinition firstColDef = new ColumnDefinition();
            firstColDef.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinition secondColDef = new ColumnDefinition();
            secondColDef.Width = new GridLength(1, GridUnitType.Star);
            tempGrid.ColumnDefinitions.Add(firstColDef);
            tempGrid.ColumnDefinitions.Add(secondColDef);

            Image icon = new Image();
            icon.Source = this.HeaderImageSource;
            icon.Height = this.ImageHeight;
            icon.Width = this.ImageWidth;
            icon.Margin = new Thickness(3);
            icon.HorizontalAlignment = HorizontalAlignment.Left;
            icon.Stretch = Stretch.UniformToFill;
            icon.SetValue(Grid.ColumnProperty, 0);
            tempGrid.Children.Add(icon);

            TextBlock headerText = new TextBlock();
            headerText.SetValue(Grid.ColumnProperty,1);
            headerText.Margin=new Thickness(5,2,2,2);
            headerText.Text=this.HeaderText;
            headerText.Foreground = this.HeaderForeground;
            headerText.FontFamily = this.HeaderFontFamily;
            headerText.FontSize = this.HeaderFontSize;
            headerText.FontWeight  = this.HeaderFontWeight;
            headerText.HorizontalAlignment = this.TextHorizontalAlignment;
            headerText.VerticalAlignment = VerticalAlignment.Center;
            tempGrid.Children.Add(headerText);

            if ((this._groupBar.VisualMode == VisualMode.StackMode) && (this._groupBar.IsNavPaneMode))
            {
                tempGrid.Width = this.ImageWidth + 7;
            }

            RotateTransform rotateDragPopup = new RotateTransform();
            rotateDragPopup.CenterX = this.clickPosition.X;
            rotateDragPopup.CenterY = this.clickPosition.Y;

            if (this._groupBar.Orientation == Orientation.Horizontal)
            {
                rotateDragPopup.Angle = -90;
            }
            else
            {
                rotateDragPopup.Angle = 0;
            }

            DragPopup.RenderTransform = rotateDragPopup;

            DragPopup.Child = tempGrid;

            if (DragPopup.Parent == null)
            {
                IEnumerable<DependencyObject> RootVisualChildren = VisualTreeExtensions.GetVisualChildren(Application.Current.RootVisual);
                foreach (DependencyObject obj in RootVisualChildren)
                {
                    if (obj is Panel)
                    {
                        (obj as Panel).Children.Add(DragPopup);
                    }
                }
            }

            this.isDragPopupCreated = true;
        }



        #endregion
    }
}
