#region Copyright Syncfusion Inc. 2001 - 2014
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

namespace Syncfusion.Windows.Tools.Controls
{
    internal class CarouselPanelHelper
    {
        private CarouselPanel carouselPanel;

        public CarouselPanelHelper(CarouselPanel panel)
        {
            this.carouselPanel = panel;
        }

        public int GetItemsCount()
        {
            int children = this.carouselPanel.Children.Count;
            if (this.carouselPanel.IsItemsHost)
            {
                children = this.GetParentItemsControl().Items.Count;
            }
            return children;
        }

        private ItemsControl GetParentItemsControl()
        {
            ItemsControl parent = null;
            if (this.carouselPanel.IsItemsHost)
            {
                parent = ItemsControl.GetItemsOwner(this.carouselPanel);
            }
            return parent;
        }

        public int ItemsCount
        {
            get
            {
                return this.GetItemsCount();
            }
        }

        public int ItemsPerPage
        {
            get
            {
                return this.carouselPanel.ItemsPerPage;
            }
        }
    }
}
