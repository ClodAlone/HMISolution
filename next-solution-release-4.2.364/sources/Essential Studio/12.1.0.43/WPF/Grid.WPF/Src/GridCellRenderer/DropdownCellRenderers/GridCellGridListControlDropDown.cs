#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the drop-down GridListControl that is used with DropDownList cells. It is based on <see cref="GridCellDropDownControlBase"/>,
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridCellGridListControlDropDown : GridCellDropDownControlBase
    {
        protected override void OnContentLoaded(System.Windows.Controls.ContentControl popupContent)
        {         
            popupContent.Content = new GridListControl();
            // this will tag the events in the base implementation.
            base.OnContentLoaded(popupContent);
        }

        /// <summary>
        /// Gets the GridListControl that is associated with the drop-down list cell.
        /// </summary>
        public GridListControlImpl GridListControlPart
        {
            get
            {
                if (this.PopupContent != null)
                {                    
                    return (this.PopupContent.Content as GridListControl).InternalGrid;
                }
                return null;
            }
        }


        //bool enableTrack = false;
        protected override void OnDropDownOpened()
        {
            base.OnDropDownOpened();
        }

        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsMouseTrackingEnabled)//When the mouse tracking is disabled then the selection was occur in the DropDown.
            {
            
                 Rect rect = GridListControlPart.RangeToClippedVisibleRect(GridListControlPart.ScrollCellsRange);
                 GridListControlPart.MouseControllerDispatcher.TrackMouse = rect;

            }
            base.OnPreviewMouseMove(e);
        }

        //private void WireEvents()
        //{
        //    this.GridListControlPart.LostFocus += new System.Windows.RoutedEventHandler(GridListControlPart_LostFocus);
        //}

        //void GridListControlPart_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if ((!this.IsMouseOverInnerTextBox && !this.IsMouseOverPopupHost) || !this.StaysOpenOnEdit)
        //    {
        //        this.Close();
        //    }
        //}
    }
}
