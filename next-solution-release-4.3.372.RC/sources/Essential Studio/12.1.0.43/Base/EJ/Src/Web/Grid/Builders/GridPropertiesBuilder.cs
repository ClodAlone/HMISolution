#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript
{
    public class GridPropertiesBuilder<T> where T:class
    {
        public Grid<T> grid;
       
        public GridPropertiesBuilder(Grid<T> grid)
        { 
            this.grid = new Grid<T>(grid.ID,grid.GridModel); 
        }
        
        public GridPropertiesBuilder()
        {
        }
        public GridPropertiesBuilder<T> AllowPaging()
        {
            grid.GridModel.AllowPaging = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowPaging(bool allowPaging)
        {
            grid.GridModel.AllowPaging = allowPaging;
            return this;
        }
        public GridPropertiesBuilder<T> AllowFiltering()
        {
            grid.GridModel.AllowFiltering= true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowFiltering(bool allowFiltering)
        {
            grid.GridModel.AllowFiltering = allowFiltering;
            return this;
        }
        public GridPropertiesBuilder<T> AllowGrouping()
        {
            grid.GridModel.AllowGrouping = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowGrouping(bool allowGrouping)
        {
            grid.GridModel.AllowGrouping = allowGrouping;
            return this;
        }
        public GridPropertiesBuilder<T> AllowEditing()
        {
            grid.GridModel.AllowEditing = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowEditing(bool allowEditing)
        {
            grid.GridModel.AllowEditing = allowEditing;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSelection()
        {
            grid.GridModel.AllowSelection = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSelection(bool allowSelection)
        {
            grid.GridModel.AllowSelection = allowSelection;
            return this;
        }
        public GridPropertiesBuilder<T> Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            grid.GridModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> Datasource(DataSource dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }
        public GridPropertiesBuilder<T> Datasource(String dataURL)
        {
            grid.GridModel.DataSource = dataURL;
            return this;
        }
        public GridPropertiesBuilder<T> Datasource(IEnumerable dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }
        public GridPropertiesBuilder<T> Datasource(IEnumerable<T> dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }

        public GridPropertiesBuilder<T> Query(string query)
        {
            grid.GridModel.Query = query;
            return this;
        }
        public GridPropertiesBuilder<T> EnableEffects()
        {
            grid.GridModel.EnableEffects = true;
            return this;
        }
        public GridPropertiesBuilder<T> EnableEffects(bool enableEffect)
        {
            grid.GridModel.EnableEffects = enableEffect;
            return this;
        }
        public GridPropertiesBuilder<T> CssClass(string cssclass)
        {
            grid.GridModel.CssClass = cssclass;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSummary()
        {
            grid.GridModel.AllowSummary = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSummary(bool allowSummary)
        {
            grid.GridModel.AllowSummary = allowSummary;
            return this;
        }
        public GridPropertiesBuilder<T> RowHover()
        {
            grid.GridModel.RowHover = true;
            return this;
        }
        public GridPropertiesBuilder<T> RowHover(bool rowHover)
        {
            grid.GridModel.RowHover = rowHover;
            return this;
        }
        public GridPropertiesBuilder<T> EnablePersist()
        {
            grid.GridModel.EnablePersist = true;
            return this;
        }
        public GridPropertiesBuilder<T> EnablePersist(bool enablePersist)
        {
            grid.GridModel.EnablePersist = enablePersist;
            return this;
        }
        public GridPropertiesBuilder<T> AllowResizing()
        {
            grid.GridModel.AllowResizing = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowResizing(bool allowResizing)
        {
            grid.GridModel.AllowResizing = allowResizing;
            return this;
        }
        public GridPropertiesBuilder<T> AllowResizeToFit()
        {
            grid.GridModel.AllowResizeToFit = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowResizeToFit(bool allowResizeToFit)
        {
            grid.GridModel.AllowResizeToFit = allowResizeToFit;
            return this;
        }
        public GridPropertiesBuilder<T> EditOnDoubleClick()
        {
            grid.GridModel.EditOndoubleClick = true;
            return this;
        }
        public GridPropertiesBuilder<T> EditOnDoubleClick(bool editOnDoubleClick)
        {
            grid.GridModel.EditOndoubleClick = editOnDoubleClick;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSearching()
        {
            grid.GridModel.AllowSearching = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSearching(bool allowSearching)
        {
            grid.GridModel.AllowSearching = allowSearching;
            return this;
        }
        public GridPropertiesBuilder<T> SelectedRow()
        {
            grid.GridModel.SelectedRow = -1;
            return this;
        }
        public GridPropertiesBuilder<T> SelectedRow(int selectedRow)
        {
            grid.GridModel.SelectedRow = selectedRow;
            return this;
        }
        public GridPropertiesBuilder<T> HeaderEffects()
        {
            grid.GridModel.HeaderEffect = true;
            return this;
        }
        public GridPropertiesBuilder<T> HeaderEffects(bool headerEffect)
        {
            grid.GridModel.HeaderEffect = headerEffect;
            return this;
        }
        public GridPropertiesBuilder<T> AllowReordering()
        {
            grid.GridModel.AllowReordering = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowReordering(bool allowReordering)
        {
            grid.GridModel.AllowReordering = allowReordering;
            return this;
        }
        public GridPropertiesBuilder<T> AllowKeyboardNavigation()
        {
            grid.GridModel.AllowKeyboardNavigation = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            grid.GridModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        public GridPropertiesBuilder<T> SelectionType(SelectionType selectionType)
        {
            grid.GridModel.Selectiontype = selectionType;
            return this;
        }
        public GridPropertiesBuilder<T> AllowScrolling()
        {
            grid.GridModel.AllowScrolling = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowScrolling(bool allowScrolling)
        {
            grid.GridModel.AllowScrolling = allowScrolling;
            return this;
        }
        public GridPropertiesBuilder<T> Localization(String localization)
        {
            grid.GridModel.Localization = localization;
            return this;
        }
        public GridPropertiesBuilder<T> AutoSaveOnRowSelection()
        {
            grid.GridModel.AutoSaveOnRowSelection = true;
            return this;
        }
        public GridPropertiesBuilder<T> AutoSaveOnRowSelection(bool autoSaveOnRowSelection)
        {
            grid.GridModel.AutoSaveOnRowSelection = autoSaveOnRowSelection;
            return this;
        }
        public GridPropertiesBuilder<T> AllowMultiSorting()
        {
            grid.GridModel.AllowMultiSorting = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowMultiSorting(bool allowMultiSorting)
        {
            grid.GridModel.AllowMultiSorting = allowMultiSorting;
            return this;
        }
        public GridPropertiesBuilder<T> DetailTemplate(String detailTemplate)
        {
            grid.GridModel.DetailTemplate = detailTemplate;
            return this;
        }
        public GridPropertiesBuilder<T> RowTemplate(String rowTemplate)
        {
            grid.GridModel.RowTemplate = rowTemplate;
            return this;
        }
        public GridPropertiesBuilder<T> RTL()
        {
            grid.GridModel.RTL = true;
            return this;
        }
        public GridPropertiesBuilder<T> RTL(bool rtl)
        {
            grid.GridModel.RTL = rtl;
            return this;
        }
        public GridPropertiesBuilder<T> AltRow()
        {
            grid.GridModel.AltRow = true;
            return this;
        }
        public GridPropertiesBuilder<T> AltRow(bool altRow)
        {
            grid.GridModel.AltRow = altRow;
            return this;
        }
        public GridPropertiesBuilder<T> Columns(Action<ColumnBuilder<T>> column)
        {
            
               var builder = new ColumnBuilder<T>(this.grid);
            
            if (column != null)
                column.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> Columns(List<Column<T>> column)
        {
            grid.GridModel.Columns = column;
            return this;
        }
        public GridPropertiesBuilder<T> PageOption(Action<PageOptionsBuilder<T>> pageOption)
        {
            var builder = new PageOptionsBuilder<T>(this.grid.GridModel.PageOption);
            this.grid.GridModel.AllowPaging = true;
            if (pageOption != null)
                pageOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> PageOption(PageOptions<T> pageOption)
        {
            grid.GridModel.PageOption = pageOption;
            this.grid.GridModel.AllowPaging = true;
            return this;
        }
        public GridPropertiesBuilder<T> GroupOption(Action<GroupOptionsBuilder<T>> groupOption)
        {
            var builder = new GroupOptionsBuilder<T>(this.grid.GridModel.GroupOption);
            this.grid.GridModel.AllowGrouping = true;
            if (groupOption != null)
                groupOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> GroupOption(GroupOptions<T> groupOption)
        {
            grid.GridModel.GroupOption = groupOption;
            this.grid.GridModel.AllowGrouping = true;
            return this;
        }
        public GridPropertiesBuilder<T> FilterOption(Action<FilterOptionsBuilder<T>> filterOption)
        {
            var builder = new FilterOptionsBuilder<T>(this.grid.GridModel.FilterOption);
            this.grid.GridModel.AllowFiltering = true;
            if (filterOption != null)
                filterOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> FilterOption(FilterOptions<T> filterOption)
        {
            grid.GridModel.FilterOption = filterOption;
            this.grid.GridModel.AllowFiltering = true;
            return this;
        }

        public GridPropertiesBuilder<T> AllowSorting()
        {
            grid.GridModel.AllowSorting = true;
            return this;
        }
        public GridPropertiesBuilder<T> AllowSorting(bool allowSorting)
        {
            grid.GridModel.AllowSorting = allowSorting;
            return this;
        }
        public GridPropertiesBuilder<T> SortOption(Action<SortOptionsBuilder<T>> sortOption)
        {
            var builder = new SortOptionsBuilder<T>(this.grid.GridModel.SortOption);
            this.grid.GridModel.AllowSorting= true;
            if (sortOption != null)
                sortOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> SortOption(SortOptions<T> sortOption)
        {
            grid.GridModel.SortOption = sortOption;
            this.grid.GridModel.AllowSorting = true;
            return this;
        }
        public GridPropertiesBuilder<T> ScrollOption(Action<ScrollOptionsBuilder<T>> scrollOption)
        {
            var builder = new ScrollOptionsBuilder<T>(this.grid.GridModel.ScrollOption);
            this.grid.GridModel.AllowScrolling = true;
            if (scrollOption != null)
                scrollOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> ScrollOption(ScrollOptions<T> scrollOption)
        {
            grid.GridModel.ScrollOption = scrollOption;
            this.grid.GridModel.AllowScrolling = true;
            return this;
        }
        public GridPropertiesBuilder<T> EditOption(Action<EditOptionsBuilder<T>> editOption)
        {
            var builder = new EditOptionsBuilder<T>(this.grid.GridModel.EditOption);
            this.grid.GridModel.AllowEditing = true;
            if (editOption != null)
                editOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> EditOption(EditOptions<T> editOption)
        {
            grid.GridModel.EditOption = editOption;
            return this;
        }
        public GridPropertiesBuilder<T> ToolBar(Action<ToolBarBuilder<T>> toolBarOption)
        {
            var builder = new ToolBarBuilder<T>(this.grid.GridModel.Toolbar);
            if (toolBarOption != null)
                toolBarOption.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> ToolBar(ToolBar<T> toolBar)
        {
            grid.GridModel.Toolbar = toolBar;
            return this;
        }
        public GridPropertiesBuilder<T> SummaryRow(Action<SummaryRowsBuilder<T>> summaryRow)
        {
            var builder = new SummaryRowsBuilder<T>(this.grid.GridModel);
            if (summaryRow != null)
                summaryRow.Invoke(builder);
            return this;
        }
        public GridPropertiesBuilder<T> SummaryRow(List<SummaryRows<T>> summaryRow)
        {
            grid.GridModel.SummaryRow = summaryRow;
            return this;
        }
        public GridPropertiesBuilder<T> ClientSideEvents(Action<ClientSideEventsBuilder<T>> clientSideEvents)
        {
            var builder = new ClientSideEventsBuilder<T>(this.grid.GridModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
       
        public HtmlString Render()
        { 
            return new HtmlString(grid.Render().ToString());
        }
        public override String ToString()
        {        
            return Render().ToString();
        }
    }
}
