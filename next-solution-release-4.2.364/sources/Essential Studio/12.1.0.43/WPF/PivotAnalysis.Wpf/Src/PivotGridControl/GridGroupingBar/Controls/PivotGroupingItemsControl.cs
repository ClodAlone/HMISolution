#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

#if SILVERLIGHT

using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Media;
namespace Syncfusion.Silverlight.Controls.PivotGrid

#else    
namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Class that holds the members of PivotGroupingItemsControl
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGroupingItemsControl : ListBox
    {

#if SILVERLIGHT

        #region [ Private Members ]

        #endregion

        #region [ Internal Properties ]

        internal Button FilterButton { get; set; }
        
        #endregion         

        #region [ Events ]

        public event CommandExecuteChanged CommandExecute;

        public event ArrangeOverrideExecuted ArrangeOverrideExecute;

        public event ContainerPrepared ContainerPrepared;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            FilterButton = sender as Button;
        }

        void Command_CanExecuteChangedForFilter(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender, false,FilterButton));
            }
        }
           
        void Command_CanExecuteChangedForSort(object sender, EventArgs e)
        {
            if (this.CommandExecute != null)
            {
                CommandExecute(this, new CommandEventArgs(sender,true,null));
            }
        }

        void tgbtn_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "MouseOver", false);
        }

        #endregion

        #region [ Overrides ]

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Name == "PART_RowList")
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ListBoxItem item = this.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    //item.Height = 31;

                    item.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    //item.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    //this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                    if (item.Content != null)
                    {
                        ToggleButton tgbtn = Common.FindVisualChild<ToggleButton>(item);
                        if (tgbtn != null)
                        {
                            tgbtn.MouseEnter += new MouseEventHandler(tgbtn_MouseEnter);
                            tgbtn.Command = new SortPivotRowItemCommand();
                            tgbtn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteChangedForSort);
                            Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");

                            if (filterBtn != null)
                            {
                                //// for Cell Renderer
                                // PivotGridControlBase controlBase = Common.GetParentElement<PivotGridControlBase>(this);

                                //// for Popup
                                //// Temporary fix: Grouping bar demo sample get blank on loading due to the below code and hence commented.
                                //PivotGridControlBase controlBase = VisualTreeHelper.FindElementsInHostCoordinates(new Point(double.MaxValue, double.MinValue),
                                //    Application.Current.RootVisual as UIElement).Where(j => j.GetType() == typeof(PivotGridControl)).FirstOrDefault() as PivotGridControlBase;

                                //if (controlBase != null)
                                //{
                                //    bool AllowFiltering = controlBase.GridControl.GroupingBar.AllowFiltering;
                                //    if (!AllowFiltering)
                                //    {
                                //        filterBtn.Visibility = System.Windows.Visibility.Collapsed;
                                //    }
                                //    else
                                //        filterBtn.Visibility = System.Windows.Visibility.Visible;
                                //}
                            }
                        }

                        Button btn = Common.FindVisualChild<Button>(item);
                        if (btn != null)
                        {
                            btn.Click += new RoutedEventHandler(btn_Click);
                            if (btn.Command != null)
                            {
                                btn.Command = new FilterPivotItemCommand();
                                btn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteChangedForFilter);
                            }
                        }
                    }

                    if (item != null)
                    {
                        if (this.ArrangeOverrideExecute != null)
                        {
                            ArrangeOverrideExecute(this, new ArrangeOverrideEventArgs(item));
                        }
                    }
                    
                }
            }
            else
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ListBoxItem item = this.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                    if (item != null)
                    {
                        ToggleButton tgbtn = Common.FindVisualChild<ToggleButton>(item);
                        if (tgbtn != null)
                        {
                            tgbtn.MouseEnter+=new MouseEventHandler(tgbtn_MouseEnter);
                            if(tgbtn.Command !=null)
                            {
                                tgbtn.Command = new SortPivotColumnItemCommand();
                                tgbtn.Command.CanExecuteChanged+=new EventHandler(Command_CanExecuteChangedForSort);
                            }
                        }

                        Button btn = Common.FindVisualChild<Button>(item);
                        if (btn != null)
                        {
                            btn.Click += new RoutedEventHandler(btn_Click);
                            if (btn.Command != null)
                            {
                                btn.Command = new FilterPivotItemCommand();
                                btn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteChangedForFilter);
                            }
                        }
                      
                        if (this.ArrangeOverrideExecute != null)
                        {
                            ArrangeOverrideExecute(this, new ArrangeOverrideEventArgs(item));
                        }                        
                    }
                }
            }

            return base.ArrangeOverride(finalSize);         
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (this.ContainerPrepared != null)
            {
                ContainerPrepared(this,new ItemContainerPrepared(element as ListBoxItem));
            }
        }

        #endregion
#endif
    }
}
