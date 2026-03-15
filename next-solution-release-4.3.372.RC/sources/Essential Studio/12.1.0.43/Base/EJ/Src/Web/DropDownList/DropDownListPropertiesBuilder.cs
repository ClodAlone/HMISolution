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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class DropDownListPropertiesBuilder
    {
        public DropDownList dropDownList;

        public DropDownListPropertiesBuilder(DropDownList dropDownList)
        { this.dropDownList = new DropDownList(dropDownList.ID, dropDownList.DropDownListModel); }

        public DropDownListPropertiesBuilder()
        {
        }
        //Boolean values
        public DropDownListPropertiesBuilder RoundedCorner()
        {
            dropDownList.DropDownListModel.RoundedCorner = true;
            return this;
        }
        public DropDownListPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            dropDownList.DropDownListModel.RoundedCorner = roundedCorner;
            return this;
        }
        public DropDownListPropertiesBuilder ShowPopupOnLoad()
        {
            dropDownList.DropDownListModel.ShowPopupOnLoad = true;
            return this;
        }
        public DropDownListPropertiesBuilder ShowPopupOnLoad(bool showPopupOnLoad)
        {
            dropDownList.DropDownListModel.ShowPopupOnLoad = showPopupOnLoad;
            return this;
        }

        public DropDownListPropertiesBuilder MultiSelectMode()
        {
            dropDownList.DropDownListModel.MultiSelectMode = true;
            return this;
        }
        public DropDownListPropertiesBuilder MultiSelectMode(bool multiSelectMode)
        {
            dropDownList.DropDownListModel.MultiSelectMode = multiSelectMode;
            return this;
        }
        public DropDownListPropertiesBuilder Rtl()
        {
            dropDownList.DropDownListModel.Rtl = true;
            return this;
        }
        public DropDownListPropertiesBuilder Rtl(bool rtl)
        {
            dropDownList.DropDownListModel.Rtl = rtl;
            return this;
        }
        public DropDownListPropertiesBuilder Enabled()
        {
            dropDownList.DropDownListModel.Enabled = true;
            return this;
        }
        public DropDownListPropertiesBuilder Enabled(bool enabled)
        {
            dropDownList.DropDownListModel.Enabled = enabled;
            return this;
        }
        public DropDownListPropertiesBuilder CaseSensitive()
        {
            dropDownList.DropDownListModel.CaseSensitive = true;
            return this;
        }
        public DropDownListPropertiesBuilder CaseSensitive(bool caseSensitive)
        {
            dropDownList.DropDownListModel.CaseSensitive = caseSensitive;
            return this;
        }
        public DropDownListPropertiesBuilder ShowCheckbox()
        {
            dropDownList.DropDownListModel.ShowCheckbox = true;
            return this;
        }
        public DropDownListPropertiesBuilder ShowCheckbox(bool showCheckbox)
        {
            dropDownList.DropDownListModel.ShowCheckbox = showCheckbox;
            return this;
        }
        public DropDownListPropertiesBuilder CheckAll()
        {
            dropDownList.DropDownListModel.CheckAll = true;
            return this;
        }
        public DropDownListPropertiesBuilder CheckAll(bool checkAll)
        {
            dropDownList.DropDownListModel.CheckAll = checkAll;
            return this;
        }
        public DropDownListPropertiesBuilder UncheckAll()
        {
            dropDownList.DropDownListModel.UncheckAll = true;
            return this;
        }
        public DropDownListPropertiesBuilder UncheckAll(bool uncheckAll)
        {
            dropDownList.DropDownListModel.UncheckAll = uncheckAll;
            return this;
        }
        public DropDownListPropertiesBuilder Persist()
        {
            dropDownList.DropDownListModel.Persist = true;
            return this;
        }
        public DropDownListPropertiesBuilder Persist(bool persist)
        {
            dropDownList.DropDownListModel.Persist = persist;
            return this;
        }
        public DropDownListPropertiesBuilder IncrementalSearch()
        {
            dropDownList.DropDownListModel.IncrementalSearch = true;
            return this;
        }
        public DropDownListPropertiesBuilder IncrementalSearch(bool incrementalSearch)
        {
            dropDownList.DropDownListModel.IncrementalSearch = incrementalSearch;
            return this;
        }
        public DropDownListPropertiesBuilder ReadOnly()
        {
            dropDownList.DropDownListModel.ReadOnly = true;
            return this;
        }
        public DropDownListPropertiesBuilder ReadOnly(bool readOnly)
        {
            dropDownList.DropDownListModel.ReadOnly = readOnly;
            return this;
        }
        public DropDownListPropertiesBuilder Boxmodel()
        {
            dropDownList.DropDownListModel.Boxmodel = true;
            return this;
        }
        public DropDownListPropertiesBuilder Boxmodel(bool boxmodel)
        {
            dropDownList.DropDownListModel.Boxmodel = boxmodel;
            return this;
        }
        //object values
        public DropDownListPropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            dropDownList.DropDownListModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public DropDownListPropertiesBuilder Datasource(DataSource dataSource)
        {
            dropDownList.DropDownListModel.DataSource = dataSource;
            return this;
        }
        
        public DropDownListPropertiesBuilder Datasource(IEnumerable dataSource)
        {
            dropDownList.DropDownListModel.DataSource = dataSource;
            return this;
        }
        // fields
        public DropDownListPropertiesBuilder DropDownListFields(Action<DropDownListFieldsBuilder> fields)
        {
            var flds = new DropDownListFields();
            dropDownList.DropDownListModel.DropDownListFields = flds;
            var builder = new DropDownListFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //Arrays
        public DropDownListPropertiesBuilder MultiSelectedItemsIndex(List<int> multiSelectedItemsIndex)
        {
            dropDownList.DropDownListModel.MultiSelectedItemsIndex = multiSelectedItemsIndex;
            return this;
        }
        //Integers
        public DropDownListPropertiesBuilder ListSize(int listSize)
        {
            dropDownList.DropDownListModel.ListSize = listSize;
            return this;
        }
        public DropDownListPropertiesBuilder SelectedItem(int selectedItem)
        {
            dropDownList.DropDownListModel.SelectedItem = selectedItem;
            return this;
        }
        //String Values
        public DropDownListPropertiesBuilder Height(String height)
        {
            dropDownList.DropDownListModel.Height = height;
            return this;
        }

        public DropDownListPropertiesBuilder WaterMark(String waterMark)
        {
            dropDownList.DropDownListModel.WaterMark = waterMark;
            return this;
        }
        public DropDownListPropertiesBuilder Width(String width)
        {
            dropDownList.DropDownListModel.Width = width;
            return this;
        }
        public DropDownListPropertiesBuilder CssClass(String cssClass)
        {
            dropDownList.DropDownListModel.CssClass = cssClass;
            return this;
        }
        public DropDownListPropertiesBuilder Value(String value)
        {
            dropDownList.DropDownListModel.Value = value;
            return this;
        }
        public DropDownListPropertiesBuilder ItemValue(String itemValue)
        {
            dropDownList.DropDownListModel.ItemValue = itemValue;
            return this;
        }
        public DropDownListPropertiesBuilder Text(String text)
        {
            dropDownList.DropDownListModel.Text = text;
            return this;
        }
        public DropDownListPropertiesBuilder PopupPanelHeight(String popupPanelHeight)
        {
            dropDownList.DropDownListModel.PopupPanelHeight = popupPanelHeight;
            return this;
        }
        public DropDownListPropertiesBuilder PopupPanelWidth(String popupPanelWidth)
        {
            dropDownList.DropDownListModel.PopupPanelWidth = popupPanelWidth;
            return this;
        }
        public DropDownListPropertiesBuilder TargetId(String targetId)
        {
            dropDownList.DropDownListModel.TargetId = targetId;
            return this;
        }
        public DropDownListPropertiesBuilder Template(String template)
        {
            dropDownList.DropDownListModel.Template = template;
            return this;
        }
        public DropDownListPropertiesBuilder SelectedTo(String selectedTo)
        {
            dropDownList.DropDownListModel.SelectedTo = selectedTo;
            return this;
        }
        public DropDownListPropertiesBuilder Query(String query)
        {
            dropDownList.DropDownListModel.Query = query;
            return this;
        }
        //Events
        public DropDownListPropertiesBuilder ClientSideEvents(Action<DropDownListClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new DropDownListClientSideEventsBuilder(this.dropDownList.DropDownListModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(dropDownList.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
