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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;


    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_ContentControl", Type = typeof(ContentControl))]

    public class HierarchyNavigatorHistoryControl : ContentControl
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorHistoryControl()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigatorHistoryControl);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsSelected && templated == false)
            {
                VisualStateManager.GoToState(this, "MouseOver", false);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(HierarchyNavigatorHistoryControl), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisitm = sender as HierarchyNavigatorHistoryControl;
            thisitm.GotoVSMState();
        }

        private bool templated = true;

        private void GotoVSMState()
        {
            if (this.IsSelected)
            {
                templated = VisualStateManager.GoToState(this, "MouseOver", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        private HierarchyNavigatorItemsCollection hierarchyItemCollection = new HierarchyNavigatorItemsCollection();
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsCollection HierarchyItemCollection
        {
            get { return hierarchyItemCollection; }
            set { hierarchyItemCollection = value; }
        }
    }
}
