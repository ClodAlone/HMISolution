#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Ribbon Title Panel Class.
    /// </summary>
    public sealed class RibbonTitlePanel : Control
    {
        #region Constructors

        /// <summary>
        /// Initializes the new instance of <see cref="RibbonTitlePanel"/>
        /// </summary>
        public RibbonTitlePanel()
        {
            System.Windows.Application.LoadComponent(this, new System.Uri("/Syncfusion.Ribbon.Silverlight;component/Ribbon/Layout/RibbonTitlePanel.xaml", System.UriKind.Relative));
        }

        #endregion

        #region Proprties

        QuickAccessToolBar tempQATToolBar;
        internal QuickAccessToolBar TempQATToolBar
        {
            get { return tempQATToolBar; }
            set
            {
                if (value != tempQATToolBar)
                {
                    tempQATToolBar = value;
                    if (this.panel != null && (tempQATToolBar != null))
                    {
                        this.panel.tempQATToolBar = this.tempQATToolBar;
                    }
                }
            }
        }
        
        #region Caption

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public object Caption
        {
            get { return (object)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the Caption Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption",
            typeof(object),
            typeof(RibbonTitlePanel),
            new PropertyMetadata(CaptionChangedCalback));

        /// <summary>
        /// Captions the changed calback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CaptionChangedCalback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RibbonTitlePanel)d).OnCaptionChanged();
        }

        #endregion

        #region GroupItems

        /// <summary>
        /// Gets the group items.
        /// </summary>
        /// <value>The group items.</value>
        internal RibbonItemsCollection GroupItems
        {
            get
            {
                if (this.groupItems == null)
                {
                    this.groupItems = new RibbonItemsCollection();
                    this.groupItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnGroupItemsChanged);
                }

                return this.groupItems;
            }
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ContentControl root = this.GetTemplateChild("Part_Root") as ContentControl;

            if (root != null)
            {
                this.panel = new RibbonTitlePanelInternal();

                root.Content = this.panel;

                if (this.panel != null && (TempQATToolBar != null))
                {
                    this.panel.tempQATToolBar = this.TempQATToolBar;
                }

                this.OnCaptionChanged();
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when [group items changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnGroupItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnAddGroupItems(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnRemoveGroupItems(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnRemoveGroupItems(e.OldItems);
                    this.OnAddGroupItems(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [layout changed].
        /// </summary>
        internal void OnLayoutChanged()
        {
            if (this.panel != null)
            {
                this.panel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Called when [caption changed].
        /// </summary>
        private void OnCaptionChanged()
        {
            if (this.panel != null)
            {
                this.panel.Caption = this.Caption;
            }
        }

        /// <summary>
        /// Called when [add group items].
        /// </summary>
        /// <param name="items">The items.</param>
        private void OnAddGroupItems(IList items)
        {
            if (this.panel != null)
            {
                foreach (UIElement item in items)
                {
                    this.panel.Children.Add(item);
                }
            }
        }

        /// <summary>
        /// Called when [remove group items].
        /// </summary>
        /// <param name="items">The items.</param>
        private void OnRemoveGroupItems(IList items)
        {
            if (this.panel != null)
            {
                foreach (UIElement item in items)
                {
                    this.panel.Children.Remove(item);
                }
            }
        }

        #endregion

        #region Fields

        private RibbonTitlePanelInternal panel;
        private RibbonItemsCollection groupItems;

        #endregion

        #region Nested classes
        /// <summary>
        /// Represents panel for layouting ribbon's title items (QAT and caption).
        /// </summary>
        /// <remarks>
        /// Only 2 items placed int the according order are supported: QAT <see cref="QuickAccessToolBar"/> and caption (<see cref="ContentControl"/>).
        /// </remarks>
        internal class RibbonTitlePanelInternal : Panel
        {
            #region Constructor;

            /// <summary>
            /// Initializes a new instance of the <see cref="RibbonTitlePanelInternal"/> class.
            /// </summary>
            internal RibbonTitlePanelInternal()
            {
                this.quickBarPresenter = new ContentPresenter();
                this.quickBarPresenter.Margin = new Thickness(-19, 0, 0, 1);

                this.captionPresenter = new ContentPresenter();
                this.captionPresenter.VerticalAlignment = VerticalAlignment.Center;

                //this.Children.Add(this.quickBarPresenter);
                this.Children.Add(this.captionPresenter);
            }

            #endregion

            #region Properties

            /// <summary>
            /// Sets the caption.
            /// </summary>
            /// <value>The caption.</value>
            internal object Caption
            {
                set
                {
                    this.captionPresenter.Content = value;
                }
            }

            #endregion

            #region Overrides

            /// <summary>
            /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
            /// </summary>
            /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
            /// <returns>The actual size used.</returns>
            protected override Size ArrangeOverride(Size finalSize)
            {
                UIElement rt = this.quickBarPresenter;
                if (this.tempQATToolBar != null && QuickAccessToolBar.CurrentQATState == QATState.AboveRibbon)
                    rt = this.tempQATToolBar;
                UIElement cc = this.captionPresenter;

                Rect rtRect = new Rect(new Point(), rt.DesiredSize);
                Rect ccRect = new Rect(0.0, 0.0, cc.DesiredSize.Width, finalSize.Height);

                rtRect.Y = finalSize.Height - rtRect.Height;

                if (ccRect.Width > 0)
                {
                    ccRect.X = (rtRect.Right - 20) + ((finalSize.Width - (rtRect.Width - 20) - ccRect.Width) / 2.0);
                    ccRect.X = ((finalSize.Width - ccRect.Width) / 2.0);
                }

                if (rt.DesiredSize.Width > ccRect.X)
                    ccRect.X = rt.DesiredSize.Width;
                //rt.Arrange(rtRect);
                cc.Arrange(ccRect);

                for (int i = 0; i < this.Children.Count; i++)
                {
                    RibbonTabsGroupItem tabsGroupItem = this.Children[i] as RibbonTabsGroupItem;

                    if (tabsGroupItem != null)
                    {
                        Point pt = new Point(0, 0);

                        double left = -1;
                        double right = -1;

                        RibbonTabsGroup tabsGroup = tabsGroupItem.TabsGroup;

                        foreach (RibbonTab tab in tabsGroup.Tabs)
                        {
                            TabButton tabItem = tab.TabItem;

                            if (tabItem != null && (tab.Visibility == Visibility.Visible || tab.IsCollapsed))
                            {
                                tabItem.Visibility = Visibility.Visible;
                                GeneralTransform gt = tabItem.TransformToVisual(this);

                                double tabLeft = gt.Transform(pt).X;

                                if (tabLeft >= rtRect.Right)
                                {
                                    double tabRight = tabLeft + tabItem.ActualWidth;

                                    if (tabRight <= finalSize.Width - 75)
                                    {
                                        if (left < 0)
                                        {
                                            if (tabRight < ccRect.Left || tabLeft > ccRect.Right)
                                            {
                                                left = tabLeft;
                                                right = tabRight;
                                            }
                                            else
                                            {
                                                //to avoid hiding contextual tabs
                                                left = tabLeft;
                                                right = tabRight;
                                                var currentRect = ccRect;
                                                currentRect.X = rt.DesiredSize.Width;
                                                cc.Arrange(currentRect);
                                            }
                                        }
                                        else
                                        {
                                            if (left < ccRect.Left)
                                            {
                                                if (left > tabLeft)
                                                {
                                                    left = tabLeft;
                                                }
                                                else
                                                {
                                                    if (tabRight < ccRect.Left)
                                                    {
                                                        right = tabRight;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (right < tabRight)
                                                {
                                                    right = tabRight;
                                                }
                                                else
                                                {
                                                    if (tabLeft > ccRect.Right)
                                                    {
                                                        left = tabLeft;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var ContextualTabWidth = right - left;
                        if (ContextualTabWidth <= 0)
                        {
                            ContextualTabWidth = tabsGroupItem.DesiredSize.Width;
                        }
                        Rect rcGroupItem = new Rect(left, 0, ContextualTabWidth, finalSize.Height);

                        tabsGroupItem.Arrange(rcGroupItem);
                    }
                }

                return finalSize;
            }

            /// <summary>
            /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
            /// </summary>
            /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
            /// <returns>
            /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
            /// </returns>
            protected override Size MeasureOverride(Size availableSize)
            {
                UIElement rt = this.quickBarPresenter;
                if (this.tempQATToolBar != null && QuickAccessToolBar.CurrentQATState == QATState.AboveRibbon)
                    rt = this.tempQATToolBar;
                UIElement cc = this.captionPresenter;

                rt.Measure(availableSize);
                cc.Measure(availableSize);

                double rtDesiredWidth = rt.DesiredSize.Width;
                double ccDesiredWidth = cc.DesiredSize.Width;

                if (rtDesiredWidth + ccDesiredWidth <= availableSize.Width)
                {
                    Size ccSize = new Size(availableSize.Width - rtDesiredWidth, availableSize.Height);

                    cc.Measure(ccSize);

                    ccDesiredWidth = cc.DesiredSize.Width;
                }
                else
                {
                    Size rtSize = new Size(availableSize.Width - ccDesiredWidth, availableSize.Height);

                    rt.Measure(rtSize);

                    rtDesiredWidth = rt.DesiredSize.Width;
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = rtDesiredWidth + ccDesiredWidth;
                }

                return availableSize;
            }

            #endregion

            #region Fields

            private ContentPresenter quickBarPresenter;
            private ContentPresenter captionPresenter;
            internal QuickAccessToolBar tempQATToolBar1;
            internal QuickAccessToolBar tempQATToolBar
            {
                get { return tempQATToolBar1; }
                set
                {
                    if (tempQATToolBar1 != value)
                    {
                        tempQATToolBar1 = value;
                        tempQATToolBar.OnQATItemsChanged += new System.EventHandler(tempQATToolBar1_OnQATItemsChanged);
                    }
                }
            }

            void tempQATToolBar1_OnQATItemsChanged(object sender, System.EventArgs e)
            {
                this.InvalidateArrange();
            }
            #endregion
        }
        #endregion
    }
}
