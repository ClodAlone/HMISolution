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
using Syncfusion.JavaScript.Mobile.Models;
namespace Syncfusion.JavaScript
{
    public class MobileGridPropertiesBuilder<T> where T:class
    {
        public MobileGrid<T> grid;

        public MobileGridPropertiesBuilder(MobileGrid<T> grid)
        {
            this.grid = new MobileGrid<T>(grid.ID, grid.GridModel); 
        }

        public MobileGridPropertiesBuilder()
        {
        }

        /// <summary>
        /// Renders the mode.
        /// </summary>
        /// <param name="renderMode">The render mode.</param>
        /// <returns></returns>
        public MobileGridPropertiesBuilder<T> RenderMode(RenderMode renderMode)
        {
            grid.GridModel.RenderMode = renderMode;
            return this;
        }

        /// <summary>
        /// Themes the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns></returns>
        public MobileGridPropertiesBuilder<T> Theme(Theme theme)
        {
            grid.GridModel.Theme = theme;
            return this;
        }
        public MobileGridPropertiesBuilder<T> ShowCaption(bool show)
        {
            grid.GridModel.ShowCaption = show;
            return this;
        }
        public MobileGridPropertiesBuilder<T> ShowColumnSelector(bool show)
        {
            grid.GridModel.ShowColumnSelector = show;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Caption(string caption)
        {
            grid.GridModel.Caption = caption;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowPaging()
        {
            grid.GridModel.AllowPaging = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowPaging(bool allowPaging)
        {
            grid.GridModel.AllowPaging = allowPaging;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowFiltering()
        {
            grid.GridModel.AllowFiltering= true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowFiltering(bool allowFiltering)
        {
            grid.GridModel.AllowFiltering = allowFiltering;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowSelection()
        {
            grid.GridModel.AllowSelection = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowSelection(bool allowSelection)
        {
            grid.GridModel.AllowSelection = allowSelection;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            grid.GridModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> Datasource(DataSource dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Datasource(String dataURL)
        {
            grid.GridModel.DataSource = dataURL;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Datasource(IEnumerable dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Datasource(IEnumerable<T> dataSource)
        {
            grid.GridModel.DataSource = dataSource;
            return this;
        }

        public MobileGridPropertiesBuilder<T> Query(string query)
        {
            grid.GridModel.Query = query;
            return this;
        }

        public MobileGridPropertiesBuilder<T> CssClass(string cssclass)
        {
            grid.GridModel.CssClass = cssclass;
            return this;
        }

        public MobileGridPropertiesBuilder<T> EnablePersist()
        {
            grid.GridModel.EnablePersist = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> EnablePersist(bool enablePersist)
        {
            grid.GridModel.EnablePersist = enablePersist;
            return this;
        }

        public MobileGridPropertiesBuilder<T> SelectedRow()
        {
            grid.GridModel.SelectedRow = -1;
            return this;
        }
        public MobileGridPropertiesBuilder<T> SelectedRow(int selectedRow)
        {
            grid.GridModel.SelectedRow = selectedRow;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowScrolling()
        {
            grid.GridModel.AllowScrolling = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowScrolling(bool allowScrolling)
        {
            grid.GridModel.AllowScrolling = allowScrolling;
            return this;
        }
        public MobileGridPropertiesBuilder<T> Columns(Action<MobileColumnBuilder<T>> column)
        {

            var builder = new MobileColumnBuilder<T>(this.grid);
            
            if (column != null)
                column.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> Columns(List<MobileColumn<T>> column)
        {
            grid.GridModel.Columns = column;
            return this;
        }
        public MobileGridPropertiesBuilder<T> PageOption(Action<MobilePageOptionsBuilder<T>> pageOption)
        {
            var builder = new MobilePageOptionsBuilder<T>(this.grid.GridModel.PageOption);
            this.grid.GridModel.AllowPaging = true;
            if (pageOption != null)
                pageOption.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> PageOption(MobilePageOptions<T> pageOption)
        {
            grid.GridModel.PageOption = pageOption;
            this.grid.GridModel.AllowPaging = true;
            return this;
        }

        public MobileGridPropertiesBuilder<T> FilterOption(Action<MobileFilterOptionsBuilder<T>> filterOption)
        {
            var builder = new MobileFilterOptionsBuilder<T>(this.grid.GridModel.FilterOption);
            this.grid.GridModel.AllowFiltering = true;
            if (filterOption != null)
                filterOption.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> FilterOption(MobileFilterOptions<T> filterOption)
        {
            grid.GridModel.FilterOption = filterOption;
            this.grid.GridModel.AllowFiltering = true;
            return this;
        }

        public MobileGridPropertiesBuilder<T> AllowSorting()
        {
            grid.GridModel.AllowSorting = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> AllowSorting(bool allowSorting)
        {
            grid.GridModel.AllowSorting = allowSorting;
            return this;
        }
        public MobileGridPropertiesBuilder<T> SortOption(Action<MobileSortOptionsBuilder<T>> sortOption)
        {
            var builder = new MobileSortOptionsBuilder<T>(this.grid.GridModel.SortOption);
            this.grid.GridModel.AllowSorting = true;
            if (sortOption != null)
                sortOption.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> SortOption(MobileSortOptions<T> sortOption)
        {
            grid.GridModel.SortOption = sortOption;
            this.grid.GridModel.AllowSorting = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> ScrollOption(Action<MobileScrollOptionsBuilder<T>> scrollOption)
        {
            var builder = new MobileScrollOptionsBuilder<T>(this.grid.GridModel.ScrollOption);
            this.grid.GridModel.AllowScrolling = true;
            if (scrollOption != null)
                scrollOption.Invoke(builder);
            return this;
        }
        public MobileGridPropertiesBuilder<T> ScrollOption(MobileScrollOptions<T> scrollOption)
        {
            grid.GridModel.ScrollOption = scrollOption;
            this.grid.GridModel.AllowScrolling = true;
            return this;
        }
        public MobileGridPropertiesBuilder<T> ClientSideEvents(Action<MobileClientSideEventsBuilder<T>> clientSideEvents)
        {
            var builder = new MobileClientSideEventsBuilder<T>(this.grid.GridModel);
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
