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
    using System.Collections.ObjectModel;


    /// <summary>
    /// Represents HierarchyNavigatorDropDownItem
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_MouseOverRoot", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BorderRoot", Type = typeof(Border))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_ContentControl", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_FolderIcon", Type = typeof(Canvas))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]

    public class HierarchyNavigatorDropDownItem : ContentControl, IHierarchyNavigatorModelHost
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorDropDownItem()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigatorDropDownItem);
            Items = new ObservableCollection<HierarchyNavigatorItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility StipperVisibility
        {
            get { return (Visibility)GetValue(StipperVisibilityProperty); }
            set { SetValue(StipperVisibilityProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty StipperVisibilityProperty =
            DependencyProperty.Register("StipperVisibility", typeof(Visibility), typeof(HierarchyNavigatorDropDownItem), new PropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// 
        /// </summary>
        public FontWeight HighLightSelectedItem
        {
            get { return (FontWeight)GetValue(HighLightSelectedItemProperty); }
            set { SetValue(HighLightSelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HighLightSelectedItemProperty =
            DependencyProperty.Register("HighLightSelectedItem", typeof(FontWeight), typeof(HierarchyNavigatorDropDownItem), new PropertyMetadata(FontWeights.Normal));
        
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
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(HierarchyNavigatorDropDownItem), new PropertyMetadata(false, OnMouseEnterChanged));
#endif
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
           public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(HierarchyNavigatorDropDownItem), new PropertyMetadata(false, OnMouseEnterChanged));
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnMouseEnterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var snder = (HierarchyNavigatorDropDownItem)sender;
            snder.GoToHighlightingState();
        }

        private void GoToHighlightingState()
        {
            if (this.IsMouseOver == true)
            {
                VisualStateManager.GoToState(this, "MouseOver", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        HierarchyNavigatorItem originalItem = new HierarchyNavigatorItem();
        
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItem OriginalItem { get { return originalItem; } set { originalItem = value; } }

        internal HierarchyNavigatorBarContent ParentBarContent { get; set; }

        internal HierarchyNavigatorDropDownItem Clone(HierarchyNavigatorItem item)
        {
            var newCloneItem = new HierarchyNavigatorDropDownItem();
            newCloneItem.Content = item.Content;
            newCloneItem.Items = item.Items;
            newCloneItem.Level = item.Level;            
            newCloneItem.ContentTemplate = item.ContentTemplate;
            //newCloneItem.Model = this.Model;
            return newCloneItem;
        }

        internal HierarchyNavigatorItem CloneHierarchyNavigatorItem()
        {
            var newCloneItem = new HierarchyNavigatorItem();
            newCloneItem.Content = this.Content;
            newCloneItem.Items = this.Items;
            newCloneItem.Level = this.Level;
            newCloneItem.ContentTemplate = this.ContentTemplate;
            //newCloneItem.Model = this.Model;
            return newCloneItem;
        }

        internal bool IsMatched(HierarchyNavigatorItem newCloneItem)
        {
            //if (newCloneItem.Content.ToString() == this.Content.ToString() || newCloneItem.Content.GetHashCode() == this.Content.GetHashCode()) return true;
            if (newCloneItem.Content.GetHashCode() == this.Content.GetHashCode()) return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(HierarchyNavigatorDropDownItem), new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<HierarchyNavigatorItem> Items { get; set; }

        #region IHierarchyNavigatorModelHost Members

        private HierarchyNavigatorModel model;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorModel Model
        {
            get { return model; }
        }

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

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

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        
    }
}
