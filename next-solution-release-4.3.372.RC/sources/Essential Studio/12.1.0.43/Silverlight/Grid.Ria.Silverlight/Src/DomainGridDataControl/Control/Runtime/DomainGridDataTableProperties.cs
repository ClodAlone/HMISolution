#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Ria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Data;
    using Syncfusion.Windows.Data;
    using System.Collections.Specialized;
    using System.Xml.Serialization;
    using System.Windows.Controls;

    public class DomainGridDataTableProperties : GridDataTableProperties
    {
        public DomainGridDataTableProperties()
            : base()
        {

        }

        public DomainGridDataTableModel Model
        {
            get;
            internal set;
        }

        protected override void OnSortColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Model.HasRIASource && !this.Model.RIASource.CanLoad)
            {
                return;
            }


            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var sortColumn = e.NewItems[0] as GridDataSortColumn;
                this.Model.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });

                if (this.Model.HasRIASource)
                {
                    this.Model.RIASource
                              .SortDescriptors
                              .Insert(e.NewStartingIndex,
#if SyncfusionFramework3_5
                                      new SortDescriptor()
                                      {
                                          PropertyPath = new Parameter()
                                          {
                                              ParameterName = sortColumn.ColumnName,
                                              Value = sortColumn.ColumnName
                                          },
                                          Direction = sortColumn.SortDirection
                                      });
#else
                                new SortDescriptor() { PropertyPath = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
#endif
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var sortColumn = e.OldItems[0] as GridDataSortColumn;
                var sortDesc = this.Model.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumn.ColumnName);
                if (sortDesc != null)
                {
                    this.Model.View.SortDescriptions.Remove(sortDesc);
                }

                if (this.Model.HasRIASource)
                {
                    this.Model.RIASource
                              .SortDescriptors
#if SyncfusionFramework3_5
                              .Remove(new SortDescriptor()
                              {
                                  PropertyPath = new Parameter()
                                  {
                                      ParameterName = sortDesc.PropertyName,
                                      Value = sortDesc.PropertyName
                                  },
                                  Direction = sortDesc.Direction
                              });
#else
.Remove(new SortDescriptor() { PropertyPath = sortDesc.PropertyName, Direction = sortDesc.Direction });
#endif
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var sortColumn = e.NewItems[0] as GridDataSortColumn;
                var sortDesc = this.Model.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumn.ColumnName);
                if (sortDesc != null)
                {
                    this.Model.View.SortDescriptions.Remove(sortDesc);
                    this.Model.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
                }

                if (this.Model.HasRIASource)
                {
                    this.Model.RIASource
                              .SortDescriptors
#if SyncfusionFramework3_5
                              .Remove(new SortDescriptor()
                              {
                                  PropertyPath = new Parameter()
                                  {
                                      ParameterName = sortDesc.PropertyName,
                                      Value = sortDesc.PropertyName
                                  },
                                  Direction = sortDesc.Direction
                              });
#else
.Remove(new SortDescriptor() { PropertyPath = sortDesc.PropertyName, Direction = sortDesc.Direction });
#endif

                    this.Model.RIASource
                              .SortDescriptors
                              .Insert(e.NewStartingIndex,
#if SyncfusionFramework3_5
                                      new SortDescriptor()
                                      {
                                          PropertyPath = new Parameter()
                                          {
                                              ParameterName = sortColumn.ColumnName,
                                              Value = sortColumn.ColumnName
                                          },
                                          Direction = sortColumn.SortDirection
                                      });
#else
 new SortDescriptor() { PropertyPath = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
#endif
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                using (this.Model.View.DeferRefresh())
                {
                    this.Model.View.SortDescriptions.Clear();

                    if (this.Model.HasRIASource)
                    {
                        this.Model.RIASource.SortDescriptors.Clear();
                    }
                }
            }

            var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
            if (dataGrid != null && dataGrid.ShowGroupDropArea && dataGrid.GroupDropAreaGrid.Model != null)
            {
                dataGrid.GroupDropAreaGrid.InvalidateCells();
            }
        }
    }
}
