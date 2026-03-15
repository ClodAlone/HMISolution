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
    public class RTEpropertiesBuilder
    {
        public RTE rte;

        public RTEpropertiesBuilder(RTE rte)
        {
            this.rte = new RTE(rte.ID, rte.RTEModel);
        }
        public RTEpropertiesBuilder() { }
        //Boolean values
        public RTEpropertiesBuilder AllowEdit()
        {
            rte.RTEModel.AllowEdit = true;
            return this;
        }
        public RTEpropertiesBuilder AllowEdit(bool allowEdit)
        {
            rte.RTEModel.AllowEdit = allowEdit;
            return this;
        }
        public RTEpropertiesBuilder AllowKeyboardnavigation()
        {
            rte.RTEModel.AllowKeyboardNavigation = true;
            return this;
        }
        public RTEpropertiesBuilder AllowKeyboardnavigation(bool allowKeyboardNavigation)
        {
            rte.RTEModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        public RTEpropertiesBuilder Enabled()
        {
            rte.RTEModel.Enabled = true;
            return this;
        }
        public RTEpropertiesBuilder Enabled(bool enabled)
        {
            rte.RTEModel.Enabled = enabled;
            return this;
        }
        public RTEpropertiesBuilder ShowToolBar()
        {
            rte.RTEModel.ShowToolBar = true;
            return this;
        }
        public RTEpropertiesBuilder ShowToolBar(bool showToolbar)
        {
            rte.RTEModel.ShowToolBar = showToolbar;
            return this;
        }
        public RTEpropertiesBuilder ShowHtmlSource()
        {
            rte.RTEModel.ShowHtmlSource = true;
            return this;
        }
        public RTEpropertiesBuilder ShowHtmlSource(bool showHtmlSource)
        {
            rte.RTEModel.ShowHtmlSource = showHtmlSource;
            return this;
        }
        public RTEpropertiesBuilder ShowWordCount()
        {
            rte.RTEModel.ShowWordCount = true;
            return this;
        }
        public RTEpropertiesBuilder ShowWordCount(bool showWordCount)
        {
            rte.RTEModel.ShowWordCount = showWordCount;
            return this;
        }
        public RTEpropertiesBuilder ShowHtmlTagInfo()
        {
            rte.RTEModel.ShowHtmlTagInfo = true;
            return this;
        }
        public RTEpropertiesBuilder ShowHtmlTagInfo(bool showHtmlTagInfo)
        {
            rte.RTEModel.ShowHtmlTagInfo = showHtmlTagInfo;
            return this;
        }
        public RTEpropertiesBuilder ShowClearAll()
        {
            rte.RTEModel.ShowClearAll = true;
            return this;
        }
        public RTEpropertiesBuilder ShowClearAll(bool showClearAll)
        {
            rte.RTEModel.ShowClearAll = showClearAll;
            return this;
        }
        public RTEpropertiesBuilder ShowClearFormat()
        {
            rte.RTEModel.ShowClearFormat = true;
            return this;
        }
        public RTEpropertiesBuilder ShowClearFormat(bool showClearFormat)
        {
            rte.RTEModel.ShowClearFormat = showClearFormat;
            return this;
        }
        public RTEpropertiesBuilder ShowFontOption()
        {
            rte.RTEModel.ShowFontOption = true;
            return this;
        }
        public RTEpropertiesBuilder ShowFontOption(bool showFontOption)
        {
            rte.RTEModel.ShowFontOption = showFontOption;
            return this;
        }
        public RTEpropertiesBuilder ShowCustomTable()
        {
            rte.RTEModel.ShowCustomTable = true;
            return this;
        }
        public RTEpropertiesBuilder ShowCustomTable(bool showCustomTable)
        {
            rte.RTEModel.ShowCustomTable = showCustomTable;
            return this;
        }
        public RTEpropertiesBuilder ShowFooter()
        {
            rte.RTEModel.ShowFooter = false;
            return this;
        }
        public RTEpropertiesBuilder ShowFooter(bool showFooter)
        {
            rte.RTEModel.ShowFooter = showFooter;
            return this;
        }
        public RTEpropertiesBuilder Rtl()
        {
            rte.RTEModel.Rtl = true;
            return this;
        }
        public RTEpropertiesBuilder Rtl(bool rtl)
        {
            rte.RTEModel.Rtl = rtl;
            return this;
        }
        public RTEpropertiesBuilder Persist()
        {
            rte.RTEModel.Persist= true;
            return this;
        }
        public RTEpropertiesBuilder Persist(bool persist)
        {
            rte.RTEModel.Persist = persist;
            return this;
        }
        public RTEpropertiesBuilder Resizable()
        {
            rte.RTEModel.Resizable = true;
            return this;
        }
        public RTEpropertiesBuilder Resizable(bool resizable)
        {
            rte.RTEModel.Resizable = resizable;
            return this;
        }
        //String Values

        public RTEpropertiesBuilder IFrameAttribute(String iframeAttribute)
        {
            rte.RTEModel.IFrameAttribute = iframeAttribute;
            return this;
        }
        public RTEpropertiesBuilder CssClass(String cssClass)
        {
            rte.RTEModel.CssClass = cssClass;
            return this;
        }
        public RTEpropertiesBuilder Width(String width)
        {
            rte.RTEModel.Width = width;
            return this;
        }
        public RTEpropertiesBuilder Height(String height)
        {
            rte.RTEModel.Height =height;
            return this;
        }
        public RTEpropertiesBuilder MaxWidth(String maxWidth)
        {
            rte.RTEModel.MaxWidth = maxWidth;
            return this;
        }
         public RTEpropertiesBuilder MinWidth(String minWidth)
        {
            rte.RTEModel.MinWidth = minWidth;
            return this;
        }
        public RTEpropertiesBuilder MaxHeight(String maxHeight)
        {
            rte.RTEModel.MaxHeight = maxHeight;
            return this;
        }
        public RTEpropertiesBuilder MinHeight(String minHeight)
        {
            rte.RTEModel.MinHeight = minHeight;
            return this;
        }
        public RTEpropertiesBuilder Value(String value)
        {
            rte.RTEModel.Value= value;
            return this;
        }
        public RTEpropertiesBuilder Name(String name)
        {
            rte.RTEModel.Name = name;
            return this;
        }
        public RTEpropertiesBuilder Localization(String localization)
        {
            rte.RTEModel.Localization = localization;
            return this;
        }
        //int values
        public RTEpropertiesBuilder MaxLength(int maxLength)
        {
            rte.RTEModel.MaxLength = maxLength;
            return this;
        }
        public RTEpropertiesBuilder TableRows(int tableRows)
        {
            rte.RTEModel.TableRows = tableRows;
            return this;
        }
        public RTEpropertiesBuilder TableColumns(int tableColumns)
        {
            rte.RTEModel.TableColumns = tableColumns;
            return this;
        }
        public RTEpropertiesBuilder ColorPaletteRows(int colorPaletteRows)
        {
            rte.RTEModel.ColorPaletteRows = colorPaletteRows;
            return this;
        }
        public RTEpropertiesBuilder ColorPaletteColumns(int colorPaletteColumns)
        {
            rte.RTEModel.ColorPaletteColumns = colorPaletteColumns;
            return this;
        }
        public RTEpropertiesBuilder UndoStackLimit(int undoStackLimit)
        {
            rte.RTEModel.UndoStackLimit = undoStackLimit;
            return this;
        }
        //Events
        public RTEpropertiesBuilder Create(String create)
        {
            rte.RTEModel.Create = create;
            return this;
        }
        public RTEpropertiesBuilder Change(String change)
        {
            rte.RTEModel.Change = change;
            return this;
        }
        public RTEpropertiesBuilder Execute(String execute)
        {
            rte.RTEModel.Execute = execute;
            return this;
        }
        public RTEpropertiesBuilder Keydown(String keydown)
        {
            rte.RTEModel.Keydown = keydown;
            return this;
        }
        public RTEpropertiesBuilder Keyup(String keyup)
        {
            rte.RTEModel.Keyup = keyup;
            return this;
        }
        public RTEpropertiesBuilder Destroy(String destroy)
        {
            rte.RTEModel.Destroy = destroy;
            return this;
        }
        public RTEpropertiesBuilder ColorCode(List<String> colorCode)
        {
            rte.RTEModel.ColorCode = colorCode;
            return this;
        }
        
        public RTEpropertiesBuilder ToolsList(List<String> toolsList)
        {
            rte.RTEModel.ToolsList = toolsList;
            return this;
        }
        // Object
        public RTEpropertiesBuilder Tools(Action<RTEtoolsBuilder> tools)
        {
            var tool = new RTEtools();
            rte.RTEModel.Tools = tool;
            var builder = new RTEtoolsBuilder(tool);
            if (tools != null)
                tools.Invoke(builder);
            return this;
        }
        //Events
        public RTEpropertiesBuilder ClientSideEvents(Action<RTEclientSideEventsBuilder> clientSideEvents)
        {
            var builder = new RTEclientSideEventsBuilder(this.rte.RTEModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }        
        public RTEpropertiesBuilder Items(Action<RTEBaseItemBuilder> items)
        {
            RTEBaseItemBuilder tabAdded = new RTEBaseItemBuilder(this.rte.RTEModel.Items);
            items.Invoke(tabAdded);
            return this as RTEpropertiesBuilder;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(rte.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }       
    }
}
