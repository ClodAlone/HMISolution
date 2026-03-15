#region Copyright Syncfusion Inc. 2001 - 2014
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.Collections;
    using System.Windows.Data;


    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_OuterBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_InnerBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_FirstLine", Type = typeof(Line))]
    [TemplatePart(Name = "PART_RightLine", Type = typeof(Line))]
    [TemplatePart(Name = "PART_LeftLine", Type = typeof(Line))]
    [TemplatePart(Name = "PART_path", Type = typeof(Path))]
    [TemplatePart(Name = "PART_MouseOverNavigationButton", Type = typeof(Border))]
    [TemplatePart(Name = "PART_NavigationButton", Type = typeof(Border))]
    [TemplatePart(Name = "PART_PopUpBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ContentControl", Type = typeof(ContentControl))]
    [StyleTypedProperty(Property = "HeaderItemContainerStyle", StyleTargetType = typeof(HierarchyNavigatorDropDownItem))]
    [StyleTypedProperty(Property = "HeaderContainerStyle", StyleTargetType = typeof(HierarchyNavigatorItem))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "NavigationButtonMouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Released", GroupName = "CommonStates")]

    public class HierarchyNavigatorBarContent : HeaderedItemsControl, IHierarchyNavigatorModelHost
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorBarContent()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigatorBarContent);
        }

        //Checking whether the DropDownPopup is open .
        bool isDropdown_Open = true;

        #region HeaderItemContainerStyle

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderItemContainerStyleProperty = DependencyProperty.Register("HeaderItemContainerStyle", typeof(Style), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style HeaderItemContainerStyle
        {
            get
            {
                return (Style)base.GetValue(HierarchyNavigatorBarContent.HeaderItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(HierarchyNavigatorBarContent.HeaderItemContainerStyleProperty, value);
            }
        }

        #endregion

        #region HeaderContainerStyle

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderContainerStyleProperty = DependencyProperty.Register("HeaderContainerStyle", typeof(Style), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style HeaderContainerStyle
        {
            get
            {
                return (Style)base.GetValue(HierarchyNavigatorBarContent.HeaderContainerStyleProperty);
            }
            set
            {
                base.SetValue(HierarchyNavigatorBarContent.HeaderContainerStyleProperty, value);
            }
        }

        #endregion

#if WPF
        public new bool IsMouseOver
#endif
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
         public bool IsMouseOver
#endif
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

#if WPF
        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(false, OnMouseOverChanged));
#endif
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
         public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(false, OnMouseOverChanged));
#endif

        private static void OnMouseOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var snder = (HierarchyNavigatorBarContent)sender;
            snder.GoToHighlightingState();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(false, OnIsOpenChnaged));

        private static void OnIsOpenChnaged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sndrObj = sender as HierarchyNavigatorBarContent;
            sndrObj.GoToState();
        }

#if WPF
        internal Storyboard ShowPopupAnimation()
        {
            DependencyObject dp = PART_HierarchyNavigatorDropDownPopup.Child as DependencyObject;
            PART_HierarchyNavigatorDropDownPopup.IsOpen = true;

            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames PopupScaleX = new DoubleAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames PopupScaleY = new DoubleAnimationUsingKeyFrames();
            DoubleAnimationUsingKeyFrames Arrowpath = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(PopupScaleX, dp);
            Storyboard.SetTarget(PopupScaleY, dp);
            Storyboard.SetTarget(Arrowpath, partPath);
            Storyboard.SetTargetProperty(PopupScaleX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(PopupScaleY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTargetProperty(Arrowpath, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            SplineDoubleKeyFrame splineDoubleKeyFrame = new SplineDoubleKeyFrame();
            SplineDoubleKeyFrame splineDoubleKeyFrame1 = new SplineDoubleKeyFrame();
            SplineDoubleKeyFrame pathDoubleKeyFrame = new SplineDoubleKeyFrame();
            SplineDoubleKeyFrame pathDoubleKeyFrame1 = new SplineDoubleKeyFrame();
            pathDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0));
            pathDoubleKeyFrame.Value = 0;
            pathDoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(Navigator.AnimationSpeed);
            pathDoubleKeyFrame1.Value = 90;
            splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(Navigator.AnimationSpeed);
            splineDoubleKeyFrame.Value = 1;
            splineDoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(Navigator.AnimationSpeed);
            splineDoubleKeyFrame1.Value = 1;
            PopupScaleX.KeyFrames.Add(splineDoubleKeyFrame);
            PopupScaleY.KeyFrames.Add(splineDoubleKeyFrame1);
            Arrowpath.KeyFrames.Add(pathDoubleKeyFrame);
            Arrowpath.KeyFrames.Add(pathDoubleKeyFrame1);
            sb.Children.Add(PopupScaleX);
            sb.Children.Add(PopupScaleY);
            sb.Children.Add(Arrowpath);

            return sb;
        }
#endif

        private void GoToState()
        {
            if (this.IsPopupOpen == true && isDropdown_Open)
            {
                if (PART_ScrollViewerRoot != null)
                {
                    PART_ScrollViewerRoot.ScrollToVerticalOffset(-PART_ScrollViewerRoot.VerticalOffset);
                }

#if WPF
                ShowPopupAnimation().Begin();
#endif
                VisualStateManager.GoToState(this, "Open", false);
            }
            else
            {
#if WPF
                Storyboard sb = new Storyboard();
                DoubleAnimationUsingKeyFrames Arrowpath = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTarget(Arrowpath, partPath);
                Storyboard.SetTargetProperty(Arrowpath, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
                SplineDoubleKeyFrame pathDoubleKeyFrame = new SplineDoubleKeyFrame();
                pathDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
                pathDoubleKeyFrame.Value = 1;
                pathDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0));
                pathDoubleKeyFrame.Value = 1;
                Arrowpath.KeyFrames.Add(pathDoubleKeyFrame);
                sb.Children.Add(Arrowpath);
                sb.Begin();
#endif
                VisualStateManager.GoToState(this, "Close", false);
                isDropdown_Open = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility NextButtonVisibility
        {
            get { return (Visibility)GetValue(NextButtonVisibilityProperty); }
            set { SetValue(NextButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextButtonVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NextButtonVisibilityProperty =
            DependencyProperty.Register("NextButtonVisibility", typeof(Visibility), typeof(HierarchyNavigatorBarContent), new PropertyMetadata(Visibility.Collapsed));

        private bool isTemplateApplied;
#if WPF
        private ToggleButton PART_NavigationButton;
#else
        private Border PART_NavigationButton;
#endif

        private ContentControl PART_ContentControl;
        Popup PART_HierarchyNavigatorDropDownPopup;
        private Line PART_LeftLine;
        private Line PART_RightLine;
        private Line PART_FirstLine;
        internal ScrollViewer PART_ScrollViewerRoot;
        private Border partBorder;        
        private Path partPath;

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_LeftLine = this.GetTemplateChild("PART_LeftLine") as Line;
            this.PART_RightLine = this.GetTemplateChild("PART_RightLine") as Line;
            this.PART_FirstLine = this.GetTemplateChild("PART_FirstLine") as Line;
            partBorder = this.GetTemplateChild("PART_PopUpBorder") as Border;
            partPath = this.GetTemplateChild("PART_path") as Path;            
#if WPF
            this.PART_NavigationButton = this.GetTemplateChild("PART_NavigationButton") as ToggleButton;
#else
            this.PART_NavigationButton = this.GetTemplateChild("PART_NavigationButton") as Border;
#endif

            this.PART_ContentControl = this.GetTemplateChild("PART_ContentControl") as ContentControl;
            this.PART_ScrollViewerRoot = this.GetTemplateChild("PART_ScrollViewerRoot") as ScrollViewer;
            this.PART_HierarchyNavigatorDropDownPopup = this.GetTemplateChild("PART_HierarchyNavigatorDropDownPopup") as Popup;
            this.isTemplateApplied = true;
            this.SubscribeEvents();
            if(this.PART_HierarchyNavigatorDropDownPopup != null)
            {
            this.PART_HierarchyNavigatorDropDownPopup.Closed += new EventHandler(PART_HierarchyNavigatorDropDownPopup_Closed);
            }
        }

        void PART_HierarchyNavigatorDropDownPopup_Closed(object sender, EventArgs e)
        {
            IsPopupOpen = false;
            //this.NavigationPopupShowHideClick(this, e);
        }

        #region IHierarchyNavigatorModelHost Members

        private HierarchyNavigatorModel model;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorModel Model
        {
            get { return model; }
        }

        internal HierarchyNavigator navigator;
        internal HierarchyNavigator Navigator
        {
            get { return navigator; }
            set
            {
                navigator = value;
            }
        }

        #endregion

        internal void SetHierarchyNavigationViewModel(HierarchyNavigatorModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }
            this.model = model;
            if (this.model != null)
            {
                this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }
        }

        internal void GenerateItems()
        {
            if (!isTemplateApplied || this.Model == null) return;

            if (this.Model.SelectedHierarchyNavigatorItem != null)
            {
                this.Items.Clear();
                var dropdwnItms = this.Model.SelectedHierarchyNavigatorItem.Items;
                foreach (var item in dropdwnItms)
                {
                    var itm = item.CloneDropDownItem();
                    if (this.HeaderItemContainerStyle != null)
                        itm.Style = this.HeaderItemContainerStyle;
                    itm.OriginalItem = item.Clone();
                    itm.OriginalItem.Level = this.Model.SelectedHierarchyNavigatorItem.Level + 1;
                    itm.Level = this.Model.SelectedHierarchyNavigatorItem.Level + 1;
                    itm.MouseLeftButtonUp += new MouseButtonEventHandler(itm_MouseLeftButtonDown);
                    itm.MouseEnter += new MouseEventHandler(itm_MouseEnter);
                    if (this.Model.ShowToolTip)
                    {
                        ToolTipService.SetToolTip(itm, itm.Content.ToString());
                    }
                    this.Items.Add(itm);
                }
            }
        }

        private void itm_MouseLeave(object sender, MouseEventArgs e)
        {
            var currentItem = sender as HierarchyNavigatorDropDownItem;
            if (currentItem != null && currentItem.IsMouseOver == true)
                currentItem.IsMouseOver = false;
        }

        private void itm_MouseEnter(object sender, MouseEventArgs e)
        {
            this.HideMouseOverForDropwItem();
            var currentItem = sender as HierarchyNavigatorDropDownItem;
            if (currentItem != null)
                currentItem.IsMouseOver = true;
        }

        internal void HideMouseOverForDropwItem()
        {
            foreach (var item in this.Items)
            {
                var naviItm = item as HierarchyNavigatorDropDownItem;
                if (naviItm != null)
                {
                    if (naviItm.IsMouseOver)
                        naviItm.IsMouseOver = false;
                }
                else
                {
                    naviItm = this.GetContainerItemFromObject(item);
                    if (naviItm != null && naviItm.IsMouseOver)
                        naviItm.IsMouseOver = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler DropDownItemSelected = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupShowHideClick = delegate { };

        private void itm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itm = this.Header as HierarchyNavigatorItem;
            if (itm != null)
            {
                this.Model.SelectedLevel = itm.Level;
            }
            else
            {
                if (this.ItemsSource != null)
                {

                    var drpdwnitm = sender as HierarchyNavigatorDropDownItem;
                    if (drpdwnitm != null)
                    {
                        drpdwnitm.IsMouseOver = false;
                        //this.Model.SelectedLevel = drpdwnitm.Level - 1;
                        HierarchyNavigatorItem hitem = (drpdwnitm.OriginalItem as HierarchyNavigatorItem);
                        if (hitem != null)
                            this.Model.SelectedLevel = (drpdwnitm.OriginalItem as HierarchyNavigatorItem).Level;
                    }
                    
                }
            }
            //this.IsMenuItemsShown = false;
            if (DropDownItemSelected != null)
            {
                
                DropDownItemSelected(sender, e);
            }
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model == null) return;
            //if (e.PropertyName == "SelectedHierarchyNavigatorItem")
            //{
            //    if (this.Model.SelectedHierarchyNavigatorItem.Level == this.Model.MaxDrillDownLevel && (this.Header as HierarchyNavigatorItem).Level == this.Model.MaxDrillDownLevel)
            //        this.NextButtonVisibility = Visibility.Collapsed;
            //    else if (this.Model.SelectedHierarchyNavigatorItem.Items.Count > 0)
            //        this.NextButtonVisibility = Visibility.Visible;
            //}
        }

        private void SubscribeEvents()
        {
            if (this.PART_NavigationButton != null)
            {
                this.PART_NavigationButton.MouseEnter += new MouseEventHandler(PART_NavigationButton_MouseEnter);
                this.PART_NavigationButton.MouseLeave += new MouseEventHandler(PART_NavigationButton_MouseLeave);
               
#if WPF                
                this.PART_NavigationButton.Click -= new RoutedEventHandler(PART_NavigationButton_Click);
                this.PART_NavigationButton.Click += new RoutedEventHandler(PART_NavigationButton_Click);
#else
                this.PART_NavigationButton.MouseLeftButtonDown -= new MouseButtonEventHandler(PART_NavigationButton_MouseLeftButtonDown);
                this.PART_NavigationButton.MouseLeftButtonDown += new MouseButtonEventHandler(PART_NavigationButton_MouseLeftButtonDown);
#endif
            }
            if (this.PART_ContentControl != null)
            {
                this.PART_ContentControl.MouseLeftButtonDown += new MouseButtonEventHandler(PART_ContentControl_MouseLeftButtonDown);
                this.PART_ContentControl.MouseLeftButtonUp += new MouseButtonEventHandler(PART_ContentControl_MouseLeftButtonUp);
                this.PART_ContentControl.MouseEnter += new MouseEventHandler(PART_ContentControl_MouseEnter);
            }
        }
#if WPF
        void PART_NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPopupShowHideClick(this, e);
            if (!this.IsMenuItemsShown)
                VisualStateManager.GoToState(this, "NavigationButtonMouseOver", false);
            isDropdown_Open = true;
        }
#endif
        void PART_ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", false);
        }

        private void PART_ContentControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsMenuItemsShown)
                this.IsMouseOver = true;
        }

        void PART_NavigationButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsMenuItemsShown)
            {
                this.IsMouseOver = false;
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        void PART_NavigationButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsMenuItemsShown)
                VisualStateManager.GoToState(this, "NavigationButtonMouseOver", true);
        }

        private void PART_ContentControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
            this.IsMenuItemsShown = false;
            var itm = this.Header as HierarchyNavigatorItem;
            if (itm != null)
            {
                this.Model.SelectedLevel = itm.Level - 1;
                if (DropDownItemSelected != null)
                    DropDownItemSelected(itm, e);
            }
            else
            {
                if (this.Items.Count > 0)
                {
                    var itmLevl = this.GetContainerItemFromObject(this.Items[0]);
                    if (itmLevl != null)
                        this.Model.SelectedLevel = itmLevl.Level - 1;
                    if (DropDownItemSelected != null)
                        DropDownItemSelected(sender, e);
                }
            }
        }

        private void GoToHighlightingState()
        {
            if (this.IsMenuItemsShown) return;
            if (this.IsMouseOver == true)
            {
                VisualStateManager.GoToState(this, "MouseOver", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }
#if SILVERLIGHT        
        private void PART_NavigationButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationPopupShowHideClick(this, e);
            if (!this.IsMenuItemsShown)
                VisualStateManager.GoToState(this, "NavigationButtonMouseOver", false);
        }
#endif
        private bool isMenuItemsShown;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMenuItemsShown
        {
            get { return isMenuItemsShown; }
            set
            {
                if (isMenuItemsShown != value)
                {
                    isMenuItemsShown = value;
                    this.GotoShowHidePopup();
                }
            }
        }

        private void GotoShowHidePopup()
        {
            if (isMenuItemsShown == false)
            {
                HidePopup();
            }
            else
            {
                ShowPopup();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is HierarchyNavigatorDropDownItem)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = this.Header as HierarchyNavigatorItem;
            if (item == null)
                item = new HierarchyNavigatorItem();
            var itm = item.CloneDropDownItem();
            if (this.HeaderItemContainerStyle != null)
                itm.Style = this.HeaderItemContainerStyle;
            itm.OriginalItem = item.Clone();
            var currentLevel = (this.Model.SelectedHierarchyNavigatorItem == null) ? this.Model.SelectedLevel + 1 : this.Model.SelectedHierarchyNavigatorItem.Level + 1;
            itm.OriginalItem.Level = currentLevel;
            itm.Level = currentLevel;
            itm.MouseLeftButtonDown += new MouseButtonEventHandler(itm_MouseLeftButtonDown);
            itm.MouseEnter += new MouseEventHandler(itm_MouseEnter);
            itm.MouseLeave += new MouseEventHandler(itm_MouseLeave);
            return itm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (!(item is HierarchyNavigatorDropDownItem))
            {
                base.PrepareContainerForItemOverride(element, item);
                HierarchyNavigatorDropDownItem ele = element as HierarchyNavigatorDropDownItem;
                ele.ParentBarContent = this;
                ele.Content = item;
                ele.Level = ele.Level + 1;
                ele.SetValue(HierarchyNavigatorDropDownItem.ContentTemplateProperty, this.ItemTemplate);
            }
            else
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }

        private Binding GetBinding(HierarchicalDataTemplate template, HierarchyNavigatorDropDownItem item)
        {
            Binding binding = new Binding();
            binding.Source = item.Content;
#if SILVERLIGHT
            binding.Converter = template.ItemsSource.Converter;
            binding.ConverterCulture = template.ItemsSource.ConverterCulture;
            binding.ConverterParameter = template.ItemsSource.ConverterParameter;
            binding.Mode = template.ItemsSource.Mode;
            binding.NotifyOnValidationError = template.ItemsSource.NotifyOnValidationError;
            binding.Path = template.ItemsSource.Path;
            binding.ValidatesOnExceptions = template.ItemsSource.ValidatesOnExceptions;
#endif
            return binding;
        }

        internal void ShowPopup()
        {
            if (this.Items.Count > 0)
            {
                this.HideMouseOverForDropwItem();
                //VisualStateManager.GoToState(this, "Pressed", false);
				this.IsPopupOpen = true;
            }
        }

        private void HidePopup()
        {
            VisualStateManager.GoToState(this, "Released", false);
            if(this.IsPopupOpen == true) this.IsPopupOpen = false;
        }

        internal HierarchyNavigatorDropDownItem GetContainerItemFromObject(object item)
        {
            var dropdownFirstItem = this.ItemContainerGenerator.ContainerFromItem(item) as HierarchyNavigatorDropDownItem;
            return dropdownFirstItem;
        }

        internal HierarchyNavigatorItem GetContainerHeaderItemFromObject(object item)
        {
            HierarchyNavigatorItem ele = new HierarchyNavigatorItem();
            ele.Content = item;
            ele.Level = ele.Level + 1;
            ele.SetValue(HierarchyNavigatorItem.ContentTemplateProperty, this.ItemTemplate);
            return ele;
        }
    }
}
