#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the tab header.
    /// </summary>
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    internal class Tab : HeaderedContentControl
    {
        //#region Class constants
        ///// <summary>
        ///// Constant Height
        ///// </summary>
        //private const double cDefaultTabHeight = 14;
        //#endregion

        //#region Private members
        ///// <summary>
        ///// Border used for drawing tab item.
        ///// </summary>
        //private TabItemAdvBorder tabBorder;

        ///// <summary>
        ///// TextBox used in label editing.
        ///// </summary>        
        //private TextBox tabTextBox;

        ///// <summary>
        ///// Used to position all tab item's internal elements.
        ///// </summary>
        //private DockPanel tabDockPanel;

        ///// <summary>
        ///// Tab item's close button.
        ///// </summary>
        //private CloseButton closeButton;

        ///// <summary>
        ///// Tab item's icon.
        ///// </summary>
        //private Image image;

        ///// <summary>
        ///// Tab item's content panel.
        ///// </summary>
        //private TabContentPanel mTabContentPanel;

        ///// <summary>
        ///// Indicates whether mouse is over the tab item.
        ///// </summary>
        //private bool isMouseOver = false;

        ///// <summary>
        ///// Drag marker.
        ///// </summary>
        //private DragMarker dragMarker;

        ///// <summary>
        ///// The tab item parent;
        ///// </summary>
        //private TabItemAdv mTabItemParent;

        ///// <summary>
        ///// The tab header content.
        ///// </summary>
        //private ContentPresenter mTabContent;
        //#endregion

        //#region Initialization
        ///// <summary>
        ///// Initializes new instance of the Tab class.
        ///// </summary>
        //public Tab()
        //{
        //    DefaultStyleKey = typeof(Tab);
        //    this.SizeChanged += new SizeChangedEventHandler(TabSizeChanged);
        //}
        //#endregion

        //#region Properties
        ///// <summary>
        ///// Gets a value indicating whether text must be rotated.
        ///// </summary>
        //internal bool IsTextRotated
        //{
        //    get
        //    {
        //        bool isRotated = false;
        //        if (this.TabControlParent != null && this.TabControlParent.RotateTextWhenVertical &&
        //            (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left
        //            || this.TabControlParent.TabStripPlacement == TabStripPlacement.Right))
        //        {
        //            isRotated = true;
        //        }

        //        return isRotated;
        //    }
        //}

        ///// <summary>
        ///// Gets a border used for drawing tab item.
        ///// </summary>
        //internal TabItemAdvBorder TabBorder
        //{
        //    get
        //    {
        //        return tabBorder;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the textBox used in label editing.
        ///// </summary>
        //internal TextBox TabTextBox
        //{
        //    get
        //    {
        //        return this.tabTextBox;
        //    }

        //    set
        //    {
        //        this.tabTextBox = value;
        //    }
        //}

        ///// <summary>
        ///// Gets the tab item's grid panel.
        ///// </summary>
        //internal DockPanel TabDockPanel
        //{
        //    get
        //    {
        //        return this.tabDockPanel;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the tab item's content.
        ///// </summary>
        //internal ContentPresenter TabContent
        //{
        //    get
        //    {
        //        return this.mTabContent;
        //    }

        //    set
        //    {
        //        this.mTabContent = value;
        //    }
        //}

        ///// <summary>
        ///// Gets the tab image alignment.
        ///// </summary>
        //internal ImageAlignment ImageAlignment
        //{
        //    get
        //    {
        //        ImageAlignment align = ImageAlignment.LeftOfText;
        //        if (this.TabItemParent != null)
        //        {
        //            align = this.TabItemParent.ImageAlignment;
        //        }

        //        return align;
        //    }
        //}

        ///// <summary>
        ///// Gets the tab header alignment.
        ///// </summary>
        //internal HeaderAlignment HeaderAlignment
        //{
        //    get
        //    {
        //        HeaderAlignment align = HeaderAlignment.Center;
        //        if (this.TabItemParent != null)
        //        {
        //            align = this.TabItemParent.HeaderAlignment;
        //        }

        //        return align;
        //    }
        //}

        ///// <summary>
        ///// Gets the tab item's header text.
        ///// </summary>
        //internal string TabText
        //{
        //    get
        //    {
        //        string text = string.Empty;
        //        if (this.TabItemParent != null)
        //        {
        //            if (this.TabItemParent.Header is TextBlock)
        //            {
        //                text = (this.TabItemParent.Header as TextBlock).Text;
        //            }
        //            else if (this.TabItemParent.Header != null)
        //            {
        //                text = this.TabItemParent.Header.ToString();
        //            }
        //        }

        //        return text;
        //    }
        //}

        ///// <summary>
        ///// Gets the tab item's close button.
        ///// </summary>
        //internal CloseButton CloseButton
        //{
        //    get
        //    {
        //        return closeButton;
        //    }
        //}

        ///// <summary>
        ///// Gets a drag marker.
        ///// </summary>
        //internal DragMarker DragMarker
        //{
        //    get
        //    {
        //        return this.dragMarker;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether the tab is selected.
        ///// </summary>
        //internal bool IsSelected
        //{
        //    get
        //    {
        //        return (bool)GetValue(IsSelectedProperty);
        //    }

        //    set
        //    {
        //        SetValue(IsSelectedProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the tab image.
        ///// </summary>
        //internal ImageSource Image
        //{
        //    get
        //    {
        //        return (ImageSource)GetValue(ImageProperty);
        //    }

        //    set
        //    {
        //        SetValue(ImageProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the tab item parent.
        ///// </summary>
        //internal TabItemAdv TabItemParent
        //{
        //    get
        //    {
        //        return mTabItemParent;
        //    }

        //    set
        //    {
        //        mTabItemParent = value;
        //    }
        //}

        ///// <summary>
        ///// Gets or sets tab item's parent.
        ///// </summary>
        //internal TabControlAdv TabControlParent
        //{
        //    get
        //    {
        //        return (TabControlAdv)GetValue(TabControlParentProperty);
        //    }

        //    set
        //    {
        //        SetValue(TabControlParentProperty, value);
        //    }
        //}
        //#endregion

        //#region Events
        ///// <summary>
        ///// Event that is raised when TabControlParent property is changed.
        ///// </summary>
        //internal event PropertyChangedCallback TabControlParentChanged;

        ///// <summary>
        ///// Event that is raised when IsSelected property is changed.
        ///// </summary>
        //internal event PropertyChangedCallback IsSelectedChanged;

        ///// <summary>
        ///// Event that is raised when Image property is changed.
        ///// </summary>
        //internal event PropertyChangedCallback ImageChanged;
        //#endregion

        //#region Dependency properties
        ///// <summary>
        ///// Identifies the <see cref="TabControlParent"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty TabControlParentProperty =
        //    DependencyProperty.Register("TabControlParent", typeof(TabControlAdv), typeof(Tab), new PropertyMetadata(new PropertyChangedCallback(OnTabControlParentChanged)));

        ///// <summary>
        ///// Identifies the <see cref="IsSelected"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty IsSelectedProperty =
        //    DependencyProperty.Register("IsSelected", typeof(bool), typeof(Tab), new PropertyMetadata(new PropertyChangedCallback(OnIsSelectedChanged)));

        ///// <summary>
        ///// Identifies the <see cref="Image"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty ImageProperty =
        //    DependencyProperty.Register("Image", typeof(ImageSource), typeof(Tab), new PropertyMetadata(new PropertyChangedCallback(OnImageChanged)));
        //#endregion

        //#region Overrides
        ///// <summary>
        ///// Called before the System.Windows.UIElement.MouseLeftButtonDown event occurs.
        ///// </summary>
        ///// <param name="e">The data for the event.</param>
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    if (IsEnabled && this.TabControlParent != null && this.TabControlParent.TabHeaders.Contains(this))
        //    {
        //        if (this.TabItemParent != null)
        //        {
        //            this.TabControlParent.SelectedIndex = this.TabControlParent.ItemContainerGenerator.IndexFromContainer(this.TabItemParent);
        //            this.TabItemParent.Focus();
        //        }
        //    }

        //    base.OnMouseLeftButtonDown(e);
        //}

        ///// <summary>
        ///// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        ///// call System.Windows.Controls.Control.ApplyTemplate() method.
        ///// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    this.tabBorder = this.GetTemplateChild("TabItemBorder") as TabItemAdvBorder;
        //    if (this.tabBorder != null)
        //    {
        //        this.tabBorder.TabParent = this;
        //        this.tabBorder.TabControlParent = this.TabControlParent;
        //    }

        //    this.tabTextBox = this.GetTemplateChild("PART_TextBox") as TextBox;
        //    this.tabDockPanel = this.GetTemplateChild("PART_DockPanel") as DockPanel;
        //    this.TabContent = this.GetTemplateChild("PART_Content") as ContentPresenter;
        //    if (this.TabContent != null)
        //    {
        //        this.TabContent.ContentTemplate = this.TabItemParent.HeaderTemplate;

        //        this.TabContent.Content = this.GetTabTextBlock(this.TabText);
        //    }


        //    this.image = this.GetTemplateChild("PART_Image") as Image;
        //    if (this.image != null)
        //    {
        //        this.image.Source = this.Image;
        //    }

        //    this.closeButton = this.GetTemplateChild("PART_CloseButton") as CloseButton;
        //    if (this.closeButton != null)
        //    {
        //        this.closeButton.Click += new RoutedEventHandler(this.CloseButtonClick);
        //    }

        //    this.mTabContentPanel = this.GetTemplateChild("TabContentPanel") as TabContentPanel;
        //    this.dragMarker = this.GetTemplateChild("DragMarker") as DragMarker;

        //    this.UpdateCloseButtonVisibility();
        //    this.UpdateImageLocation();
        //    this.UpdateHeaderLocation();
        //    this.UpdateVisualState();
        //    this.UpdateTabsMargin();
        //}

        ///// <summary>
        ///// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes
        /////  can override this method to define their own arrange pass behavior.
        ///// </summary>
        ///// <param name="finalSize">The final area within the parent that this object should use to arrange itself
        ///// and its children.</param>
        ///// <returns>The actual size used.</returns>
        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    if (this.tabBorder != null)
        //    {
        //        this.tabBorder.InvalidateArrange();
        //    }

        //    return base.ArrangeOverride(finalSize);
        //}

        ///// <summary>
        ///// Provides the behavior for the "measure" pass of Silverlight layout. Classes
        ///// can override this method to define their own measure pass behavior.
        ///// </summary>
        ///// <param name="availableSize">The available size that this object can give to child objects. Infinity can
        ///// be specified as a value to indicate that the object will size to whatever
        ///// content is available.</param>
        ///// <returns>The size that this object determines it needs during layout, based on its
        ///// calculations of child object allotted sizes.</returns>
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    if (this.TabBorder != null)
        //    {
        //        this.TabBorder.InvalidateMeasure();
        //    }

        //    if (this.TabContent != null)
        //    {
        //        double width = availableSize.Width;
        //        if (this.IsTextRotated)
        //        {
        //            width = availableSize.Height;
        //        }

        //        string str = this.GetStringWithEllipsis(width);
        //        TextBlock txt = this.GetTabTextBlock(str);
        //        if (txt.Width > this.GetTextSpace(width))
        //        {
        //            this.TabContent.MaxWidth = this.GetTextSpace(width) - 10;
        //        }
        //        this.TabContent.Content = this.TabItemParent.Header;
                
        //    }
        //    return base.MeasureOverride(availableSize);
        //}

        ///// <summary>
        ///// Called before the System.Windows.UIElement.MouseEnter event occurs.
        ///// </summary>
        ///// <param name="e">The event data.</param>
        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    this.isMouseOver = true;
        //    this.UpdateCloseButtonVisibility();
        //    if (this.TabControlParent != null)
        //    {
        //        this.TabControlParent.InvalidateTabs();
        //        if (this.tabBorder != null)
        //        {
        //            bool isLeftRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Left;
        //            bool isRightRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Right;
        //            this.tabBorder.SetVisualState(true, isRightRotated, isLeftRotated);

        //            if (this.TabControlParent != null)
        //            {
        //                for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
        //                {
        //                    Tab tabItem = this.TabControlParent.TabHeaders[i] as Tab;
        //                    if (tabItem != this && tabItem.TabBorder != null)
        //                    {
        //                        tabItem.TabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    base.OnMouseEnter(e);
        //}

        ///// <summary>
        ///// Called before the System.Windows.UIElement.MouseLeave event occurs.
        ///// </summary>
        ///// <param name="e">The event data.</param>
        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    this.isMouseOver = false;
        //    this.UpdateCloseButtonVisibility();
        //    if (this.TabControlParent != null)
        //    {
        //        this.TabControlParent.InvalidateTabs();

        //        if (this.tabBorder != null)
        //        {
        //            bool isLeftRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Left;
        //            bool isRightRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Right;
        //            this.tabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
        //        }
        //    }

        //    base.OnMouseLeave(e);
        //}
        //#endregion

        //#region Implementation
        ///// <summary>
        ///// Update visual state of the tab header.
        ///// </summary>
        //internal void UpdateVisualState()
        //{
        //    if (this.IsSelected)
        //    {
        //        VisualStateManager.GoToState(this, "Selected", true);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(this, "Normal", true);
        //    }
        //}

        ///// <summary>
        ///// Occurs when either the System.Windows.FrameworkElement.ActualHeight or the
        ///// System.Windows.FrameworkElement.ActualWidth properties change value on a
        ///// System.Windows.FrameworkElement.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The event data.</param>
        //private void TabSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    this.UpdateTabsMargin();
        //    this.InvalidateMeasure();
        //}

        ///// <summary>
        ///// Updates margin of the tabs according to the style and tab strip placement.
        ///// </summary>
        //internal void UpdateTabsMargin()
        //{
        //    if (this.TabControlParent != null && this.TabItemParent != null && this.TabDockPanel != null)
        //    {
        //        if (this.TabControlParent.VisualStyle == TabControlVisualStyle.IE)
        //        {
        //            if (this.IsTextRotated)
        //            {
        //                this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
        //            }
        //            else
        //            {
        //                this.TabDockPanel.Margin = new Thickness(5, 3, 5, 5);
        //            }
        //        }
        //        else if (this.TabControlParent.VisualStyle == TabControlVisualStyle.VS2008
        //            || this.TabControlParent.VisualStyle == TabControlVisualStyle.Aero)
        //        {
        //            double tabHeight = cDefaultTabHeight;
        //            if (this.IsTextRotated)
        //            {
        //                if (this.ActualWidth != 0)
        //                {
        //                    tabHeight = this.ActualWidth / 2;
        //                }

        //                if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left)
        //                {
        //                    tabHeight += 4;
        //                    this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);

        //                    if (this.image != null)
        //                    {
        //                        if (this.TabItemParent.ImageAlignment == ImageAlignment.LeftOfText)
        //                        {
        //                            this.image.Margin = new Thickness(tabHeight + 2, 0, 2, 0);
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(2, 0, 2, 0);
        //                            }
        //                        }
        //                        else if (this.TabItemParent.ImageAlignment == ImageAlignment.BelowText)
        //                        {
        //                            tabHeight = (2 * (this.ActualWidth / 3)) + 4;
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            }

        //                            this.image.Margin = new Thickness(2, 0, 2, 0);
        //                        }
        //                        else if (this.TabItemParent.ImageAlignment == ImageAlignment.AboveText)
        //                        {
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            }

        //                            this.image.Margin = new Thickness(tabHeight * 2, 0, 5, 0);
        //                        }
        //                        else
        //                        {
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            }

        //                            this.image.Margin = new Thickness(2, 0, 2, 0);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    this.TabDockPanel.Margin = new Thickness(8, 1, 5, 7);
        //                    if (this.image != null)
        //                    {
        //                        if (this.TabItemParent.ImageAlignment == ImageAlignment.LeftOfText)
        //                        {
        //                            this.image.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(2, 0, 5, 0);
        //                            }
        //                        }
        //                        else if (this.TabItemParent.ImageAlignment == ImageAlignment.BelowText)
        //                        {
        //                            tabHeight = (2 * (this.ActualWidth / 3)) + 4;
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 5, 0);
        //                            }
        //                        }
        //                        else if (this.TabItemParent.ImageAlignment == ImageAlignment.AboveText)
        //                        {
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            }

        //                            this.image.Margin = new Thickness(tabHeight * 2, 0, 5, 0);
        //                        }
        //                        else
        //                        {
        //                            if (this.TabContent != null)
        //                            {
        //                                this.TabContent.Margin = new Thickness(tabHeight, 0, 2, 0);
        //                            }

        //                            this.image.Margin = new Thickness(2, 0, 5, 0);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (this.ActualHeight != 0)
        //                {
        //                    tabHeight = (this.ActualHeight / 2) + 1;
        //                }

        //                if (this.image != null && (this.TabItemParent.ImageAlignment == ImageAlignment.BelowText
        //                    || this.TabItemParent.ImageAlignment == ImageAlignment.AboveText))
        //                {
        //                    tabHeight += 4;
        //                }

        //                this.TabDockPanel.Margin = new Thickness(tabHeight, 3, 5, 2);
        //                if (this.TabContent != null)
        //                {
        //                    this.TabContent.Margin = new Thickness(1, 0, 1, 0);
        //                }

        //                if (this.image != null)
        //                {
        //                    this.image.Margin = new Thickness(4, 0, 4, 0);
        //                }
        //            }
        //        }
        //        else if (this.TabControlParent.VisualStyle == TabControlVisualStyle.Blend)
        //        {
        //            if (this.IsTextRotated)
        //            {
        //                this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
        //            }
        //            else
        //            {
        //                this.TabDockPanel.Margin = new Thickness(2, 4, 3, 6);
        //            }
        //        }
        //        else if (this.TabControlParent.VisualStyle == TabControlVisualStyle.Office2003)
        //        {
        //            if (this.IsTextRotated)
        //            {
        //                this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
        //            }
        //            else
        //            {
        //                this.TabDockPanel.Margin = new Thickness(5, 3, 5, 5);
        //            }
        //        }
        //        else if (this.TabControlParent.VisualStyle == TabControlVisualStyle.Office2007Black
        //            || this.TabControlParent.VisualStyle == TabControlVisualStyle.Office2007Blue
        //            || this.TabControlParent.VisualStyle == TabControlVisualStyle.Office2007Silver)
        //        {
        //            if (this.IsTextRotated)
        //            {
        //                this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
        //            }
        //            else
        //            {
        //                this.TabDockPanel.Margin = new Thickness(5, 3, 5, 2);
        //            }
        //        }
        //        else
        //        {
        //            if (this.IsTextRotated)
        //            {
        //                this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
        //            }
        //            else
        //            {
        //                this.TabDockPanel.Margin = new Thickness(5, 3, 5, 3);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets the textblock.
        ///// </summary>
        ///// <param name="text">String to set in textblock.</param>
        ///// <returns>The textBlock.</returns>
        //private TextBlock GetTabTextBlock(string text)
        //{
        //    TextBlock txtBlock = new TextBlock();
        //    if (this.TabControlParent != null)
        //    {
        //        if (this.IsSelected)
        //        {
        //            txtBlock.FontWeight = FontWeights.SemiBold;
        //        }
        //        else
        //        {
        //            if (this.TabControlParent.VisualStyle == TabControlVisualStyle.Office2007Black)
        //            {
        //                txtBlock.Foreground = new SolidColorBrush(Colors.White);
        //            }
        //        }
        //        if (this.TabControlParent.VisualStyle == TabControlVisualStyle.Blend)
        //        {
        //            txtBlock.Foreground = new SolidColorBrush(Colors.White);
        //        }
        //    }

        //    txtBlock.Text = text;
        //    return txtBlock;
        //}

        ///// <summary>
        ///// Gets the space needed for the header text displaying.
        ///// </summary>
        ///// <param name="availableWidth">Tab item's width.</param>
        ///// <returns>The text space.</returns>
        //private double GetTextSpace(double availableWidth)
        //{
        //    double space = availableWidth;
        //    if (space != 0)
        //    {
        //        if (this.image != null && this.image.ActualWidth != 0 &&
        //            (this.ImageAlignment == ImageAlignment.LeftOfText || this.ImageAlignment == ImageAlignment.RightOfText))
        //        {
        //            space -= this.image.ActualWidth + this.image.Margin.Left + this.image.Margin.Right;
        //        }

        //        if (this.closeButton != null && this.closeButton.Visibility == Visibility.Visible)
        //        {
        //            space -= this.closeButton.ActualWidth + this.closeButton.Margin.Left + this.closeButton.Margin.Right;
        //        }

        //        if (this.tabDockPanel != null)
        //        {
        //            space -= this.tabDockPanel.Margin.Left + this.tabDockPanel.Margin.Right;
        //        }
        //        if (this.IsSelected)
        //        {
        //            space -= 10;
        //        }
        //    }

        //    return space;
        //}

        ///// <summary>
        ///// Gets the string converted with ellipsis.
        ///// </summary>
        ///// <param name="availableWidth">Tab item's width.</param>
        ///// <returns>Converted string.</returns>
        //internal string GetStringWithEllipsis(double availableWidth)
        //{
        //    string str = string.Empty;
        //    if (this.TabContent != null)
        //    {
        //        TextBlock txtBlock = new TextBlock();
        //        txtBlock.Text = this.TabText;
        //        double actualWidth = txtBlock.ActualWidth;
        //        double specWidth = this.GetTextSpace(availableWidth);
        //        if (specWidth >= actualWidth)
        //        {
        //            str = this.TabText;
        //        }
        //        else
        //        {
        //            TextBlock auxTxt = new TextBlock();
        //            auxTxt.FontFamily = txtBlock.FontFamily;
        //            auxTxt.FontSize = txtBlock.FontSize;
        //            auxTxt.FontWeight = txtBlock.FontWeight;
        //            auxTxt.Text = "...";

        //            int i = 0;
        //            for (; i < txtBlock.Text.Length; i++)
        //            {
        //                if (auxTxt.ActualWidth >= specWidth)
        //                {
        //                    break;
        //                }

        //                auxTxt.Text += txtBlock.Text[i];
        //            }

        //            if (auxTxt.Text == "...")
        //            {
        //                str = ".";
        //                if (specWidth < 7.5)
        //                {
        //                    str = string.Empty;
        //                }
        //            }
        //            else
        //            {
        //                str = i < txtBlock.Text.Length && i > 0 ?
        //                    string.Concat(txtBlock.Text.Substring(0, i - 1), "...") : auxTxt.Text;
        //            }
        //        }
        //    }

        //    return str;
        //}

        ///// <summary>
        ///// Updates image location according to the ImageAlignment property value.
        ///// </summary>
        //internal void UpdateImageLocation()
        //{
        //    if (this.image != null)
        //    {
        //        switch (this.ImageAlignment)
        //        {
        //            case ImageAlignment.LeftOfText:
        //                DockPanel.SetDock(this.image, Dock.Left);
        //                break;
        //            case ImageAlignment.RightOfText:
        //                DockPanel.SetDock(this.image, Dock.Right);
        //                break;
        //            case ImageAlignment.AboveText:
        //                DockPanel.SetDock(this.image, Dock.Top);
        //                break;
        //            case ImageAlignment.BelowText:
        //                DockPanel.SetDock(this.image, Dock.Bottom);
        //                break;
        //        }

        //        this.InvalidateMeasure();
        //        this.InvalidateArrange();
        //    }
        //}

        ///// <summary>
        ///// Updates header location according to the HeaderAlignment property value.
        ///// </summary>
        //internal void UpdateHeaderLocation()
        //{
        //    if (this.mTabContentPanel != null)
        //    {
        //        switch (this.HeaderAlignment)
        //        {
        //            case HeaderAlignment.Left:
        //                this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Left;
        //                break;
        //            case HeaderAlignment.Center:
        //                this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Center;
        //                break;
        //            case HeaderAlignment.Right:
        //                this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Right;
        //                break;
        //        }

        //        this.InvalidateMeasure();
        //        this.InvalidateArrange();
        //    }
        //}

        ///// <summary>
        ///// Updates close buttons visibility according to the TabControlAdv.CloseButtonType property value.
        ///// </summary>
        //internal void UpdateCloseButtonVisibility()
        //{
        //    if (this.TabControlParent != null && this.closeButton != null)
        //    {
        //        this.closeButton.ShowButton();
        //        switch (this.TabControlParent.CloseButtonType)
        //        {
        //            case CloseButtonType.Both:
        //                this.closeButton.Visibility = Visibility.Visible;
        //                break;
        //            case CloseButtonType.Common:
        //                this.closeButton.Visibility = Visibility.Collapsed;
        //                break;
        //            case CloseButtonType.Hide:
        //                this.closeButton.Visibility = Visibility.Collapsed;
        //                break;
        //            case CloseButtonType.Individual:
        //                this.closeButton.Visibility = Visibility.Visible;
        //                break;
        //            case CloseButtonType.IndividualOnMouseOver:
        //                this.closeButton.Visibility = Visibility.Visible;
        //                this.closeButton.ApplyTemplate();
        //                if (this.isMouseOver)
        //                {
        //                    if (this.TabControlParent != null)
        //                    {
        //                        for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
        //                        {
        //                            Tab tabItem = this.TabControlParent.TabHeaders[i] as Tab;
        //                            if (tabItem != null)
        //                            {
        //                                tabItem.CloseButton.HideButton();
        //                            }
        //                        }
        //                    }

        //                    this.closeButton.ShowButton();
        //                }
        //                else
        //                {
        //                    this.closeButton.HideButton();
        //                }

        //                break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Occurs when close button is clicked.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The event data.</param>
        //private void CloseButtonClick(object sender, RoutedEventArgs e)
        //{
        //    this.Visibility = Visibility.Collapsed;
        //    if (this.TabControlParent != null && this.TabItemParent != null)
        //    {
        //        this.TabItemParent.Visibility = Visibility.Collapsed;
        //        int itemIndex = this.TabControlParent.Items.IndexOf(this.TabItemParent);
        //        if (itemIndex == this.TabControlParent.SelectedIndex)
        //        {
        //            if (itemIndex < this.TabControlParent.Items.Count + 1)
        //            {
        //                this.TabControlParent.IsAllTabsClosed = false;
        //                for (int i = itemIndex + 1; i < this.TabControlParent.Items.Count; i++)
        //                {
        //                    TabItemAdv item = this.TabControlParent.Items[i] as TabItemAdv;
        //                    if (item != null && item.Tab != null
        //                        && item.Tab.Visibility == Visibility.Visible)
        //                    {
        //                        this.TabControlParent.SelectedIndex = i;
        //                        break;
        //                    }
        //                }

        //                if (itemIndex == this.TabControlParent.SelectedIndex)
        //                {
        //                    for (int i = itemIndex - 1; i >= 0; i--)
        //                    {
        //                        TabItemAdv item = this.TabControlParent.Items[i] as TabItemAdv;
        //                        if (item != null && item.Tab != null
        //                            && item.Tab.Visibility == Visibility.Visible)
        //                        {
        //                            this.TabControlParent.SelectedIndex = i;
        //                            break;
        //                        }
        //                    }
        //                }

        //                if (itemIndex == this.TabControlParent.SelectedIndex)
        //                {
        //                    this.TabControlParent.SelectedIndex = -1;
        //                    this.TabControlParent.IsAllTabsClosed = true;
        //                }
        //            }
        //        }
        //    }

        //    if (this.TabControlParent.IsAllTabsClosed)
        //    {
        //        this.TabControlParent.CloseSelectedTabItem();
        //    }
        //}

        ///// <summary>
        ///// Calls OnTabControlParentChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnTabControlParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Tab instance = (Tab)d;
        //    instance.OnTabControlParentChanged(e);
        //}

        ///// <summary>
        ///// Updates property value cache and raises TabControlParentChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnTabControlParentChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.tabBorder != null)
        //    {
        //        this.tabBorder.TabControlParent = this.TabControlParent;
        //    }

        //    if (this.TabControlParentChanged != null)
        //    {
        //        this.TabControlParentChanged(this, e);
        //    }
        //}

        ///// <summary>
        ///// Calls OnIsSelectedChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Tab instance = (Tab)d;
        //    instance.OnIsSelectedChanged(e);
        //}

        ///// <summary>
        ///// Updates property value cache and raises IsSelectedChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if ((bool)e.NewValue)
        //    {
        //        if (this.TabContent != null)
        //        {
        //            double width = this.ActualWidth;
        //            if (this.IsTextRotated)
        //            {
        //                width = this.ActualHeight;
        //            }

        //            string str = this.GetStringWithEllipsis(width);
        //            this.TabContent.Content = this.GetTabTextBlock(str);
        //        }
        //    }
        //    this.UpdateVisualState();

        //    if (this.IsSelectedChanged != null)
        //    {
        //        this.IsSelectedChanged(this, e);
        //    }
        //}

        ///// <summary>
        ///// Calls OnImageChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Tab instance = (Tab)d;
        //    instance.OnImageChanged(e);
        //}

        ///// <summary>
        ///// Updates property value cache and raises ImageChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnImageChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.image != null)
        //    {
        //        this.image.Source = this.Image;
        //    }

        //    if (this.ImageChanged != null)
        //    {
        //        this.ImageChanged(this, e);
        //    }
        //}
        //#endregion
    }
}
