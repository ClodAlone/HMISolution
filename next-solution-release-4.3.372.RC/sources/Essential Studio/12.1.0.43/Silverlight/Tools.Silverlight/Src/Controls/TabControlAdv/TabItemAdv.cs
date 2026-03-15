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
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents item of TabControlAdv.
    /// </summary>

    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "UnSelected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "LostFocus", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "GotFocus", GroupName = "FocusStates")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
  Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Blend;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Default;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Office2003;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Metro;component/TabControlAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(TabItemAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/TabControlAdv.xaml")]

    [CLSCompliant(false)]
    public class TabItemAdv : Syncfusion.Windows.Controls.HeaderedContentControl
    {
        #region Private members
        /// <summary>
        /// Tab item's content presenter.
        /// </summary>
        private ContentPresenter mContentPresenter;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabItemAdv class.
        /// </summary>
        public TabItemAdv()
        {
            DefaultStyleKey = typeof(TabItemAdv);
            this.Loaded += new RoutedEventHandler(TabItemAdv_Loaded);
        }

        void TabItemAdv_Loaded(object sender, RoutedEventArgs e)
        {
            string s = Syncfusion.Windows.Controls.Theming.SkinManager.GetVisualStyle(this).ToString();
            if (s == "Default")
            {
                this.isMouseOver = true;
                this.UpdateCloseButtonVisibility();
                if (this.TabControlParent != null)
                {

                    // this.TabControlParent.InvalidateTabs();
                    if (this.tabBorder != null)
                    {
                        bool isLeftRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Left;
                        bool isRightRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Right;
                        if (this.IsSelected)
                            this.tabBorder.SetVisualState(true, isRightRotated, isLeftRotated);

                        if (this.TabControlParent != null)
                        {
                            for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
                            {
                                TabItemAdv tabItem = this.TabControlParent.TabHeaders[i] as TabItemAdv;
                                if (tabItem != this && tabItem.TabBorder != null)
                                {
                                    tabItem.TabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
                                }
                            }
                        }
                    }
                }
                UpdateVisualState();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tab item's content presenter.
        /// </summary>
        internal ContentPresenter TabContentPresenter
        {
            get
            {
                return mContentPresenter;
            }

            set
            {
                mContentPresenter = value;
            }
        }
        #endregion

        #region DP getters and setters
        ///// <summary>
        ///// Gets or sets the tab header control.
        ///// </summary>
        //internal Tab Tab
        //{
        //    get
        //    {
        //        return (Tab)GetValue(TabProperty);
        //    }

        //    set
        //    {
        //        SetValue(TabProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the tab template.
        /// </summary>
        internal ControlTemplate TabTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TabTemplateProperty);
            }

            set
            {
                SetValue(TabTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab item's header.
        /// </summary>
        /// <value>The header.</value>
        [Description("Enter Header of the Tab Item")]
        [Category("Tab Item Properties")]
        public new object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab item is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab item's background when mouse is over it.
        /// </summary>
        /// <value>The hover background.</value>       
        public Brush HoverBackground
        {
            get
            {
                return (Brush)GetValue(HoverBackgroundProperty);
            }

            set
            {
                SetValue(HoverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ImageAlignment dependency property.
        /// </summary>
        /// <value>The image alignment.</value>
        [Description("Used to Align Header Image")]
        [Category("Tab Item Properties")]
        public ImageAlignment ImageAlignment
        {
            get
            {
                return (ImageAlignment)GetValue(ImageAlignmentProperty);
            }

            set
            {
                SetValue(ImageAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the tab item's header.
        /// </summary>
        /// <value>The header alignment.</value>
        [Description("Used to Align Tab Item Header")]
        [Category("Tab Item Properties")]
        public HeaderAlignment HeaderAlignment
        {
            get
            {
                return (HeaderAlignment)GetValue(HeaderAlignmentProperty);
            }

            set
            {
                SetValue(HeaderAlignmentProperty, value);
            }
        }

        ///// <summary>
        ///// Gets or sets the tab item's icon.
        ///// </summary>
        //public ImageSource Image
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
        ///// Gets or sets tab item's parent.
        ///// </summary>
        //public TabControlAdv TabControlParent
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
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Tab property is changed.
        /// </summary>
        internal event PropertyChangedCallback TabChanged;

        /// <summary>
        /// Event that is raised when TabTemplate property is changed.
        /// </summary>
        internal event PropertyChangedCallback TabTemplateChanged;

        /// <summary>
        /// Event that is raised when TabControlParent property is changed.
        /// </summary>
        internal event PropertyChangedCallback TabControlParentChanged;

        /// <summary>
        /// Event that is raised when HasHeader property is changed.
        /// </summary>
        public event PropertyChangedCallback HasHeaderChanged;

        /// <summary>
        /// Event that is raised when Header property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderChanged;

        /// <summary>
        /// Event that is raised when IsSelected property is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when HoverBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback HoverBackgroundChanged;

        /// <summary>
        /// Event that is raised when ImageAlignment property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageAlignmentChanged;

        /// <summary>
        /// Event that is raised when HeaderAlignment property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderAlignmentChanged;

        /// <summary>
        /// Event that is raised when Image property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageChanged;
        #endregion

        #region Dependency properties
        ///// <summary>
        ///// Identifies the <see cref="Tab"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty TabProperty =
        //    DependencyProperty.Register("Tab", typeof(Tab), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabChanged)));

        /// <summary>
        /// Identifies the <see cref="TabTemplate"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabTemplateProperty =
            DependencyProperty.Register("TabTemplate", typeof(ControlTemplate), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="TabControlParent"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabControlParentProperty =
            DependencyProperty.Register("TabControlParent", typeof(TabControlAdv), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnTabControlParentChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register("HasHeader", typeof(bool), typeof(TabItemAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnHasHeaderChanged)));

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public new static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies the <see cref="HoverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnHoverBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="ImageAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageAlignmentProperty =
            DependencyProperty.Register("ImageAlignment", typeof(ImageAlignment), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnImageAlignmentChanged)));

        /// <summary>
        /// Identifies the <see cref="HeaderAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderAlignmentProperty =
            DependencyProperty.Register("HeaderAlignment", typeof(HeaderAlignment), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnHeaderAlignmentChanged)));

        /// <summary>
        /// Identifies the <see cref="Image"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(TabItemAdv), new PropertyMetadata(new PropertyChangedCallback(OnImageChanged)));
        #endregion

        #region Overrides
        ///// <summary>
        ///// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        ///// call System.Windows.Controls.Control.ApplyTemplate() method.
        ///// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    mContentPresenter = this.GetTemplateChild("ContentPresenter") as ContentPresenter;
        //    if (mContentPresenter != null)
        //    {
        //        if (!this.IsSelected)
        //        {
        //            mContentPresenter.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            mContentPresenter.Visibility = Visibility.Visible;
        //        }
        //    }
        //}
        #endregion

        #region Implementation
        /// <summary>
        /// Ensures that item has a nams.
        /// </summary>
        internal void EnsureName()
        {
            if (this.Name == string.Empty)
            {
                string uniqueStr = "_uniqueTabName";
                if (this.TabControlParent != null)
                {
                    int num = 0;
                    string nameStr = "";
                    while (this.Name == string.Empty)
                    {
                        nameStr = uniqueStr + num.ToString();
                        bool bIsUnique = true;
                        for (int i = 0; i < this.TabControlParent.Items.Count; i++)
                        {
                            if (this.TabControlParent.Items[i] is TabItemAdv)
                            {
                                TabItemAdv tabItem = this.TabControlParent.Items[i] as TabItemAdv;
                                if (tabItem.Name == nameStr)
                                {
                                    bIsUnique = false;
                                    break;
                                }
                            }
                        }

                        if (bIsUnique)
                        {
                            this.Name = nameStr;
                        }
                        else
                        {
                            num++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnTabChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnTabChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Tab != null && this.TabTemplate != null)
            //{
            //    this.Tab.Template = this.TabTemplate;
            //}

            if (this.TabChanged != null)
            {
                this.TabChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabTemplateChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnTabTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabTemplateChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Tab != null && this.TabTemplate != null)
            //{
            //    this.Tab.Template = this.TabTemplate;
            //}

            if (this.TabTemplateChanged != null)
            {
                this.TabTemplateChanged(this, e);
            }
        }

        ///// <summary>
        ///// Calls OnTabControlParentChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnTabControlParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    TabItemAdv instance = (TabItemAdv)d;
        //    instance.OnTabControlParentChanged(e);
        //}

        ///// <summary>
        ///// Updates property value cache and raises TabControlParentChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnTabControlParentChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.TabControlParent != null)
        //    {
        //        if (this.IsSelected)
        //        {
        //            this.TabControlParent.SelectedItem = this;
        //        }                
        //    }

        //    if (this.TabControlParentChanged != null)
        //    {
        //        this.TabControlParentChanged(this, e);
        //    }
        //}

        /// <summary>
        /// Calls OnHasHeaderChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHasHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnHasHeaderChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasHeaderChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHasHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.HasHeaderChanged != null)
            {
                this.HasHeaderChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHeaderChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnHeaderChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HeaderChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabContent != null)
            {
                this.TabContent.Content = this.Header;
            }

            if (this.HeaderChanged != null)
            {
                this.HeaderChanged(this, e);
            }
        }

        ///// <summary>
        ///// Calls OnIsSelectedChanged method of the instance, notifies of the depencency property value changes.
        ///// </summary>
        ///// <param name="d">Dependency object, the change occures on.</param>
        ///// <param name="e">Property change details, such as old value and new value.</param>
        //private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    TabItemAdv instance = (TabItemAdv)d;
        //    instance.OnIsSelectedChanged(e);
        //}

        ///// <summary>
        ///// Updates property value cache and raises IsSelectedChanged event.
        ///// </summary>
        ///// <param name="e">
        ///// Property change details, such as old value and new value.</param>
        //protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    RoutedEventArgs args = new RoutedEventArgs();
        //    if ((bool)e.NewValue)
        //    {
        //        if (mContentPresenter != null)
        //        {
        //            mContentPresenter.Visibility = Visibility.Visible;
        //        }

        //        if (this.TabControlParent != null)
        //        {
        //            this.TabControlParent.SelectedItem = this;
        //        }
        //    }
        //    else
        //    {
        //        if (mContentPresenter != null)
        //        {
        //            this.mContentPresenter.Visibility = Visibility.Collapsed;
        //        }
        //    }

        //    if (this.TabControlParent != null)
        //    {
        //        this.TabControlParent.InvalidateTabs();
        //    }

        //    if (this.Tab != null)
        //    {
        //        //this.Tab.IsSelected = (bool)e.NewValue;
        //    }

        //    if (this.IsSelectedChanged != null)
        //    {
        //        this.IsSelectedChanged(this, e);
        //    }
        //}

        /// <summary>
        /// Calls OnHoverBackgroundChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHoverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnHoverBackgroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HoverBackgroundChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHoverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.HoverBackgroundChanged != null)
            {
                this.HoverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageAlignmentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnImageAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ImageAlignmentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Tab != null)
            {
                this.UpdateImageLocation();
                this.UpdateTabsMargin();
                //if (this.TabControlParent != null)
                //{
                //    this.TabControlParent.InvalidateTabs();
                //}
            }

            if (this.ImageAlignmentChanged != null)
            {
                this.ImageAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHeaderAlignmentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeaderAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnHeaderAlignmentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HeaderAlignmentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHeaderAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Tab != null)
            {
                this.UpdateHeaderLocation();
            }

            if (this.HeaderAlignmentChanged != null)
            {
                this.HeaderAlignmentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnImageChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ImageChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Tab != null)
            {
                //       this.Tab.Image = this.Image;
            }

            if (this.ImageChanged != null)
            {
                this.ImageChanged(this, e);
            }
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Constant Height
        /// </summary>
        private const double cDefaultTabHeight = 14;
        #endregion

        #region Private members
        /// <summary>
        /// Border used for drawing tab item.
        /// </summary>
        private TabItemAdvBorder tabBorder;

        /// <summary>
        /// TextBox used in label editing.
        /// </summary>        
        private TextBox tabTextBox;

        /// <summary>
        /// Used to position all tab item's internal elements.
        /// </summary>
        private DockPanel tabDockPanel;

        /// <summary>
        /// Tab item's close button.
        /// </summary>
        private CloseButton closeButton;

        /// <summary>
        /// Tab item's icon.
        /// </summary>
        private Image image;

        /// <summary>
        /// Tab item's content panel.
        /// </summary>
        private TabContentPanel mTabContentPanel;

        /// <summary>
        /// Indicates whether mouse is over the tab item.
        /// </summary>
        private bool isMouseOver = false;

        private bool isGotFocus;

        private bool isLostFocus;

        /// <summary>
        /// Drag marker.
        /// </summary>
        private DragMarker dragMarker;

        /// <summary>
        /// The tab item parent;
        /// </summary>
        private TabItemAdv mTabItemParent;

        /// <summary>
        /// The tab header content.
        /// </summary>
        private ContentPresenter mTabContent;
        #endregion


        #region Properties
        /// <summary>
        /// Gets a value indicating whether text must be rotated.
        /// </summary>
        internal bool IsTextRotated
        {
            get
            {
                bool isRotated = false;
                if (this.TabControlParent != null && this.TabControlParent.RotateTextWhenVertical &&
                    (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left
                    || this.TabControlParent.TabStripPlacement == TabStripPlacement.Right))
                {
                    isRotated = true;
                }

                return isRotated;
            }
        }

        /// <summary>
        /// Gets a border used for drawing tab item.
        /// </summary>
        internal TabItemAdvBorder TabBorder
        {
            get
            {
                return tabBorder;
            }
        }

        /// <summary>
        /// Gets or sets the textBox used in label editing.
        /// </summary>
        internal TextBox TabTextBox
        {
            get
            {
                return this.tabTextBox;
            }

            set
            {
                this.tabTextBox = value;
            }
        }

        /// <summary>
        /// Gets the tab item's grid panel.
        /// </summary>
        internal DockPanel TabDockPanel
        {
            get
            {
                return this.tabDockPanel;
            }
        }

        /// <summary>
        /// Gets or sets the tab item's content.
        /// </summary>
        internal ContentPresenter TabContent
        {
            get
            {
                return this.mTabContent;
            }

            set
            {
                this.mTabContent = value;
            }
        }

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

        /// <summary>
        /// Gets the tab item's header text.
        /// </summary>
        internal string TabText
        {
            get
            {
                string text = string.Empty;
                if (this.TabItemParent != null)
                {
                    if (this.TabItemParent.Header is TextBlock)
                    {
                        text = (this.TabItemParent.Header as TextBlock).Text;
                    }
                    else if (this.TabItemParent.Header != null)
                    {
                        text = this.TabItemParent.Header.ToString();
                    }
                }

                return text;
            }
        }

        /// <summary>
        /// Gets the tab item's close button.
        /// </summary>
        internal CloseButton CloseButton
        {
            get
            {
                return closeButton;
            }
        }

        /// <summary>
        /// Gets a drag marker.
        /// </summary>
        internal DragMarker DragMarker
        {
            get
            {
                return this.dragMarker;
            }
        }

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

        /// <summary>
        /// Gets or sets the tab image.
        /// </summary>
        /// <value>The image.</value>
        [Description("To set Tab Item Header Image")]
        [Category("Tab Item Properties")]
        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }

            set
            {
                SetValue(ImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab item parent.
        /// </summary>
        /// <value>The tab item parent.</value>
        internal TabItemAdv TabItemParent
        {
            get
            {
                return mTabItemParent;
            }

            set
            {
                mTabItemParent = value;
            }
        }

        /// <summary>
        /// Gets or sets tab item's parent.
        /// </summary>
        /// <value>The tab control parent.</value>
        internal TabControlAdv TabControlParent
        {
            get
            {
                return (TabControlAdv)GetValue(TabControlParentProperty);
            }

            set
            {
                SetValue(TabControlParentProperty, value);
            }
        }
        #endregion

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

        #region Overrides
        /// <summary>
        /// Called before the System.Windows.UIElement.MouseLeftButtonDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.TabControlParent != null && this.TabControlParent.TabItemOpenMode == TabItemOpenMode.OnMouseClick && IsEnabled && this.TabControlParent.TabHeaders.Contains(this))
                {
                    if (this.TabItemParent != null)
                    {
                        if (this.TabControlParent.Items.Contains(TabItemParent))
                        {
                            this.TabControlParent.SelectedItem = this.TabItemParent;
                            this.TabControlParent.SelectedIndex = this.TabControlParent.GetIndexOfTabItemAdv(this.TabItemParent);
                            this.TabItemParent.Focus();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.MouseMove event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.TabControlParent != null &&  this.TabControlParent.TabItemOpenMode == TabItemOpenMode.OnMouseOver && IsEnabled && this.TabControlParent.TabHeaders.Contains(this))
            {
                if (this.TabItemParent != null)
                {
                    this.TabControlParent.SelectedItem = this.TabItemParent;
                    this.TabControlParent.SelectedIndex = this.TabControlParent.GetIndexOfTabItemAdv(this.TabItemParent);
                    this.TabItemParent.Focus();
                }
            }

            UpdateVisualState();
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass)
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (TabControlParent.ItemContainerStyle != null)
            {
                if (this.Style == null)
                {
                    //this.Style=TabControlParent.ItemContainerStyle;
                    Binding binding = new Binding();
                    binding.Source = TabControlParent;
                    binding.Path = new PropertyPath("ItemContainerStyle");
                    this.SetBinding(TabItemAdv.StyleProperty, binding);
                }
            }
            TabItemParent = this;
            this.tabBorder = this.GetTemplateChild("TabItemBorder") as TabItemAdvBorder;
            if (this.tabBorder != null)
            {
                this.tabBorder.TabParent = this;
                this.tabBorder.TabControlParent = this.TabControlParent;
            }

            this.tabTextBox = this.GetTemplateChild("PART_TextBox") as TextBox;
            this.tabDockPanel = this.GetTemplateChild("PART_DockPanel") as DockPanel;
            this.TabContent = this.GetTemplateChild("PART_Content") as ContentPresenter;
            if (this.TabContent != null)
            {

                // this.TabContent.ContentTemplate = this.TabItemParent.HeaderTemplate;
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("HeaderTemplate");
                this.TabContent.SetBinding(ContentPresenter.ContentTemplateProperty, binding);
                this.TabContent.Content = this.GetTabTextBlock(this.TabText);
            }


            this.image = this.GetTemplateChild("PART_Image") as Image;
            if (this.image != null)
            {
                this.image.Source = this.Image;
                if (this.image.Source != null)
                {
                    this.image.Height = 16;
                    this.image.Width = 16;
                }
            }
           
            this.closeButton = this.GetTemplateChild("PART_CloseButton") as CloseButton;
            if (this.closeButton != null)
            {
                this.closeButton.Click += new RoutedEventHandler(this.CloseButtonClick);
            }

            this.mTabContentPanel = this.GetTemplateChild("TabContentPanel") as TabContentPanel;
            this.dragMarker = this.GetTemplateChild("DragMarker") as DragMarker;

            for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
            {
                TabItemAdv tabHeader = this.TabControlParent.TabHeaders[i] as TabItemAdv;

                if (this.TabControlParent.TabVisualStyle != TabVisualStyle.None)
                {

                    if (i == 0)
                    {
                        tabHeader.Margin = new Thickness(0, 0, -4, 0);
                    }

                }
                else
                {
                    if (i == 0)
                        tabHeader.Margin = this.Margin;
                }
            }
            this.UpdateCloseButtonVisibility();
            this.UpdateImageLocation();
            this.UpdateHeaderLocation();
            this.UpdateVisualState();
            this.UpdateTabsMargin();
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes
        /// can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself
        /// and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.tabBorder != null)
            {
                this.tabBorder.InvalidateArrange();
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes
        /// can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can
        /// be specified as a value to indicate that the object will size to whatever
        /// content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its
        /// calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.TabBorder != null)
            {
                this.TabBorder.InvalidateMeasure();
            }

            if (this.TabContent != null)
            {
                double width = availableSize.Width;
                if (this.IsTextRotated)
                {
                    width = availableSize.Height;
                }

                string str = this.GetStringWithEllipsis(width);
                TextBlock txt = this.GetTabTextBlock(str);
                if (txt.Width > this.GetTextSpace(width))
                {
                    this.TabContent.MaxWidth = this.GetTextSpace(width) - 10;
                }
                if (this.TabItemParent.Header is TextBlock || this.TabItemParent.Header is String)
                {
                    txt.TextTrimming = TextTrimming.WordEllipsis;
                    if (this.TabControlParent.TabVisualStyle == TabVisualStyle.ExcelBlue ||  this.TabControlParent.TabVisualStyle == TabVisualStyle.ExcelBlack || this.TabControlParent.TabVisualStyle==TabVisualStyle.ExcelSilver)
                    {
                        txt.FontFamily = new System.Windows.Media.FontFamily("Calibiri");
                        txt.Padding = new Thickness(8, 0, 0, 0);
                        txt.FontSize = 12.0;
                        //txt.FontWeight = FontWeights.Bold;
                    }
                    this.TabContent.Content = txt;
                }
                else
                this.TabContent.Content = this.TabItemParent.Header;

            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.MouseEnter event occurs.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.isMouseOver = true;
            this.UpdateCloseButtonVisibility();
            if (this.TabControlParent != null)
            {
               
              // this.TabControlParent.InvalidateTabs();
                if (this.tabBorder != null)
                {
                    bool isLeftRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Left;
                    bool isRightRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Right;
                    this.tabBorder.SetVisualState(true, isRightRotated, isLeftRotated);

                    if (this.TabControlParent != null)
                    {
                        for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
                        {
                            TabItemAdv tabItem = this.TabControlParent.TabHeaders[i] as TabItemAdv;
                            if (tabItem != this && tabItem.TabBorder != null)
                            {
                                tabItem.TabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
                            }
                        }
                    }
                }
            }
            UpdateVisualState();

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.MouseLeave event occurs.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.isMouseOver = false;
            this.UpdateCloseButtonVisibility();
            if (this.TabControlParent != null)
            {
                
             //  this.TabControlParent.InvalidateTabs();

                if (this.tabBorder != null)
                {
                    bool isLeftRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Left;
                    bool isRightRotated = this.IsTextRotated && this.TabControlParent.TabStripPlacement == TabStripPlacement.Right;
                    this.tabBorder.SetVisualState(false, isRightRotated, isLeftRotated);
                }
            }

            UpdateVisualState();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.GotFocus event occurs.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.isGotFocus = true;
            UpdateVisualState();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.LostFocus event occurs.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.isGotFocus = false;
            this.isLostFocus = true;
            UpdateVisualState();
            base.OnLostFocus(e);
            this.isLostFocus = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.TabControlParent != null)
                this.TabControlParent.UpdateSelectedContent();

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Update visual state of the tab header.
        /// </summary>
        internal void UpdateVisualState()
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnSelected", true);

                if (this.isMouseOver && !this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "MouseOver", true);
                }
                else if (!this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }

            if (this.isGotFocus)
                VisualStateManager.GoToState(this, "GotFocus", true);

            if (this.isLostFocus)
                VisualStateManager.GoToState(this, "LostFocus", true);

            if (!this.IsSelected && !this.isMouseOver && !this.isGotFocus && !this.isLostFocus)
                VisualStateManager.GoToState(this, "Normal", true);
            if(this.IsEnabled==false)
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        /// <summary>
        /// Occurs when either the System.Windows.FrameworkElement.ActualHeight or the
        /// System.Windows.FrameworkElement.ActualWidth properties change value on a
        /// System.Windows.FrameworkElement.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateTabsMargin();
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Updates margin of the tabs according to the style and tab strip placement.
        /// </summary>
        internal void UpdateTabsMargin()
        {
            if (this.TabControlParent != null && this.TabItemParent != null && this.TabDockPanel != null)
            {

                if (this.IsTextRotated)
                {
                    this.TabDockPanel.Margin = new Thickness(8, 5, 0, 3);
                }
                else
                {
                    this.TabDockPanel.Margin = new Thickness(5, 3, 5, 3);
                }
                if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Bottom)
                {
                    if (this.tabDockPanel != null)
                    {
                        if (TabControlParent.TabVisualStyle == TabVisualStyle.None)
                        {
                            this.tabDockPanel.Margin = new Thickness(10, 3, 0, 3);
                        }
                        else
                        {
                            this.tabDockPanel.Margin = new Thickness(20, 0, 0, 2);
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// Gets the textblock.
        /// </summary>
        /// <param name="text">String to set in textblock.</param>
        /// <returns>The textBlock.</returns>
        private TextBlock GetTabTextBlock(string text)
        {
            TextBlock txtBlock = new TextBlock();

            txtBlock.Text = text;
            return txtBlock;
        }

        /// <summary>
        /// Gets the space needed for the header text displaying.
        /// </summary>
        /// <param name="availableWidth">Tab item's width.</param>
        /// <returns>The text space.</returns>
        private double GetTextSpace(double availableWidth)
        {
            double space = availableWidth;
            if (space != 0)
            {
                if (this.image != null && this.image.ActualWidth != 0 &&
                    (this.ImageAlignment == ImageAlignment.LeftOfText || this.ImageAlignment == ImageAlignment.RightOfText))
                {
                    space -= this.image.ActualWidth + this.image.Margin.Left + this.image.Margin.Right;
                }

                if (this.closeButton != null && this.closeButton.Visibility == Visibility.Visible)
                {
                    space -= this.closeButton.ActualWidth + this.closeButton.Margin.Left + this.closeButton.Margin.Right;
                }

                if (this.tabDockPanel != null)
                {
                    space -= this.tabDockPanel.Margin.Left + this.tabDockPanel.Margin.Right;
                }
                if (this.IsSelected)
                {
                    space -= 10;
                }
            }

            return space;
        }

        /// <summary>
        /// Gets the string converted with ellipsis.
        /// </summary>
        /// <param name="availableWidth">Tab item's width.</param>
        /// <returns>Converted string.</returns>
        internal string GetStringWithEllipsis(double availableWidth)
        {
            string str = string.Empty;
            if (this.TabContent != null)
            {
                TextBlock txtBlock = new TextBlock();
                txtBlock.Text = this.TabText;
                double actualWidth = txtBlock.ActualWidth;
                double specWidth = this.GetTextSpace(availableWidth);
                if (specWidth >= actualWidth)
                {
                    str = this.TabText;
                }
                else
                {
                    TextBlock auxTxt = new TextBlock();
                    auxTxt.FontFamily = txtBlock.FontFamily;
                    auxTxt.FontSize = txtBlock.FontSize;
                    auxTxt.FontWeight = txtBlock.FontWeight;
                    auxTxt.Text = "...";

                    int i = 0;
                    for (; i < txtBlock.Text.Length; i++)
                    {
                        if (auxTxt.ActualWidth >= specWidth)
                        {
                            break;
                        }

                        auxTxt.Text += txtBlock.Text[i];
                    }

                    if (auxTxt.Text == "...")
                    {
                        str = ".";
                        if (specWidth < 7.5)
                        {
                            str = string.Empty;
                        }
                    }
                    else
                    {
                        str = i < txtBlock.Text.Length && i > 0 ?
                            string.Concat(txtBlock.Text.Substring(0, i - 1), "...") : auxTxt.Text;
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Updates image location according to the ImageAlignment property value.
        /// </summary>
        internal void UpdateImageLocation()
        {
            if (this.image != null)
            {
                switch (this.ImageAlignment)
                {
                    case ImageAlignment.LeftOfText:
                        DockPanel.SetDock(this.image, Dock.Left);
                        break;
                    case ImageAlignment.RightOfText:
                        DockPanel.SetDock(this.image, Dock.Right);
                        break;
                    case ImageAlignment.AboveText:
                        DockPanel.SetDock(this.image, Dock.Top);
                        break;
                    case ImageAlignment.BelowText:
                        DockPanel.SetDock(this.image, Dock.Bottom);
                        break;
                }

                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Updates header location according to the HeaderAlignment property value.
        /// </summary>
        internal void UpdateHeaderLocation()
        {
            if (this.mTabContentPanel != null)
            {
                switch (this.HeaderAlignment)
                {
                    case HeaderAlignment.Left:
                        this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case HeaderAlignment.Center:
                        this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case HeaderAlignment.Right:
                        this.mTabContentPanel.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }

                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Updates close buttons visibility according to the TabControlAdv.CloseButtonType property value.
        /// </summary>
        internal void UpdateCloseButtonVisibility()
        {
            if (this.TabControlParent != null && this.closeButton != null)
            {
                this.closeButton.ShowButton();
                switch (this.TabControlParent.CloseButtonType)
                {
                    case CloseButtonType.Both:
                        this.closeButton.Visibility = Visibility.Visible;
                        break;
                    case CloseButtonType.Common:
                        this.closeButton.Visibility = Visibility.Collapsed;
                        break;
                    case CloseButtonType.Hide:
                        this.closeButton.Visibility = Visibility.Collapsed;
                        break;
                    case CloseButtonType.Individual:
                        this.closeButton.Visibility = Visibility.Visible;
                        break;
                    case CloseButtonType.IndividualOnMouseOver:
                        this.closeButton.Visibility = Visibility.Visible;
                        this.closeButton.ApplyTemplate();
                        if (this.isMouseOver)
                        {
                            if (this.TabControlParent != null)
                            {
                                //for (int i = 0; i < this.TabControlParent.TabHeaders.Count; i++)
                                //{
                                //    Tab tabItem = this.TabControlParent.TabHeaders[i] as Tab;
                                //    if (tabItem != null)
                                //    {
                                //        tabItem.CloseButton.HideButton();
                                //    }
                                //}
                            }

                            this.closeButton.ShowButton();
                        }
                        else
                        {
                            this.closeButton.HideButton();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when close button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.TabControlParent != null && this.TabItemParent != null)
            {
                CloseTabEventArgs args = new CloseTabEventArgs(this.TabItemParent);
                this.TabControlParent.FireTabClosing(args);
                if (!args.cancel)
                {
                    this.Visibility = Visibility.Collapsed;
                    this.TabItemParent.Visibility = Visibility.Collapsed;
                    int itemIndex = this.TabControlParent.GetIndexOfTabItemAdv(this.TabItemParent);

                    if (itemIndex == this.TabControlParent.SelectedIndex)
                    {
                        if (itemIndex < this.TabControlParent.Items.Count + 1)
                        {
                            this.TabControlParent.IsAllTabsClosed = false;
                            for (int i = itemIndex + 1; i < this.TabControlParent.Items.Count; i++)
                            {
                                TabItemAdv item = this.TabControlParent.Items[i] as TabItemAdv;
                                if (item != null && item != null && item.Visibility == Visibility.Visible)
                                {
                                    this.TabControlParent.SelectedItem = this.TabControlParent.Items[i] as TabItemAdv;
                                    this.TabControlParent.SelectedIndex = i;
                                    break;
                                }
                            }

                            if (itemIndex == this.TabControlParent.SelectedIndex)
                            {
                                for (int i = itemIndex - 1; i >= 0; i--)
                                {
                                    TabItemAdv item = this.TabControlParent.Items[i] as TabItemAdv;
                                    if (item != null && item != null
                                        && item.Visibility == Visibility.Visible)
                                    {
                                        this.TabControlParent.SelectedItem = this.TabControlParent.Items[i] as TabItemAdv;
                                        this.TabControlParent.SelectedIndex = i;
                                        break;
                                    }
                                }
                            }

                            if (itemIndex == this.TabControlParent.SelectedIndex)
                            {
                                this.TabControlParent.SelectedIndex = -1;
                                this.TabControlParent.IsAllTabsClosed = true;
                            }
                        }
                    }
                }
            }

            if (this.TabControlParent.IsAllTabsClosed)
            {
                this.TabControlParent.CloseSelectedTabItem();
            }
        }

        /// <summary>
        /// Calls OnTabControlParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabControlParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnTabControlParentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabControlParentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabControlParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.tabBorder != null)
            {
                this.tabBorder.TabControlParent = this.TabControlParent;
            }
            if (this.TabControlParent != null)
            {
                if (this.IsSelected)
                {
                    this.TabControlParent.SelectedItem = this;
                }
            }


            if (this.TabControlParentChanged != null)
            {
                this.TabControlParentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdv instance = (TabItemAdv)d;
            instance.OnIsSelectedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsSelectedChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (this.TabContent != null)
                {
                    double width = this.ActualWidth;
                    if (this.IsTextRotated)
                    {
                        width = this.ActualHeight;
                    }

                    string str = this.GetStringWithEllipsis(width);
                    this.TabContent.Content = this.GetTabTextBlock(str);
                }
            }
            this.UpdateVisualState();

            if (this.IsSelectedChanged != null)
            {
                this.IsSelectedChanged(this, e);
            }

            RoutedEventArgs args = new RoutedEventArgs();
            if ((bool)e.NewValue)
            {
                if (mContentPresenter != null)
                {
                    mContentPresenter.Visibility = Visibility.Visible;
                }

                if (this.TabControlParent != null)
                {
                    this.TabControlParent.SelectedItem = this;
                }
            }
            else
            {
                if (mContentPresenter != null)
                {
                    this.mContentPresenter.Visibility = Visibility.Collapsed;
                }
            }

            if (this.TabControlParent != null)
            {
                this.TabControlParent.InvalidateTabs();
            }

            //if (this.Tab != null)
            {
                this.IsSelected = (bool)e.NewValue;
            }


        }

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
        #endregion
    }
}
