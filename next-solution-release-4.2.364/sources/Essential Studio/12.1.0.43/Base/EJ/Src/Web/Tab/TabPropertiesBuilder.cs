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
    public class TabPropertiesBuilder
    {
        public Tab tab;

        public TabPropertiesBuilder(Tab tab)
        { this.tab = new Tab(tab.ID, tab.TabModel); }

        public TabPropertiesBuilder()
        {
        }
        //int values
        public TabPropertiesBuilder SelectedItemIndex(int selectedItemIndex)
        {
            tab.TabModel.SelectedItemIndex = selectedItemIndex;
            return this;
        }
        //Boolean values
        public TabPropertiesBuilder Collapsible()
        {
            tab.TabModel.Collapsible = true;
            return this;
        }
        public TabPropertiesBuilder Collapsible(bool collapsible)
        {
            tab.TabModel.Collapsible = collapsible;
            return this;
        }
        public TabPropertiesBuilder ShowCloseButton()
        {
            tab.TabModel.ShowCloseButton = true;
            return this;
        }
        public TabPropertiesBuilder ShowCloseButton(bool showCloseButton)
        {
            tab.TabModel.ShowCloseButton = showCloseButton;
            return this;
        }
        public TabPropertiesBuilder Rtl()
        {
            tab.TabModel.Rtl = true;
            return this;
        }
        public TabPropertiesBuilder Rtl(bool rtl)
        {
            tab.TabModel.Rtl = rtl;
            return this;
        }
        public TabPropertiesBuilder AllowKeyboardNavigation()
        {
            tab.TabModel.AllowKeyboardNavigation = true;
            return this;
        }
        public TabPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            tab.TabModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }
        public TabPropertiesBuilder RoundedCorner()
        {
            tab.TabModel.RoundedCorner = true;
            return this;
        }
        public TabPropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            tab.TabModel.RoundedCorner = roundedCorner;
            return this;
        }
        public TabPropertiesBuilder Persist()
        {
            tab.TabModel.Persist = true;
            return this;
        }
        public TabPropertiesBuilder Persist(bool persist)
        {
            tab.TabModel.Persist = persist;
            return this;
        }
        public TabPropertiesBuilder Enabled()
        {
            tab.TabModel.Enabled = true;
            return this;
        }
        public TabPropertiesBuilder Enabled(bool enabled)
        {
            tab.TabModel.Enabled = enabled;
            return this;
        }
        //String values
        public TabPropertiesBuilder Events(String events)
        {
            tab.TabModel.Events = events;
            return this;
        }
        public TabPropertiesBuilder Width(String width)
        {
            tab.TabModel.Width = width;
            return this;
        }
        public TabPropertiesBuilder Height(String height)
        {
            tab.TabModel.Height = height;
            return this;
        }
        public TabPropertiesBuilder CssClass(String cssClass)
        {
            tab.TabModel.CssClass = cssClass;
            return this;
        }
        public TabPropertiesBuilder IdPrefix(String idPrefix)
        {
            tab.TabModel.IdPrefix = idPrefix;
            return this;
        }
        public TabPropertiesBuilder DisabledItems(List<String> disabledItems)
        {
            tab.TabModel.DisabledItems = disabledItems;
            return this;
        }
        //Enum values
        public TabPropertiesBuilder HeightStyle(HeightStyle heightStyle)
        {
            tab.TabModel.HeightStyle = heightStyle;
            return this;
        }
        public TabPropertiesBuilder HeaderPosition(HeaderPosition headerPosition)
        {
            tab.TabModel.HeaderPosition = headerPosition;
            return this;
        }
        //Objects
        public TabPropertiesBuilder AjaxOptions(Action<jQueryAjaxOptionsBuilder> ajaxOptions)
        {
            var ajaxOpt = new jQueryAjaxOptions();
            tab.TabModel.AjaxOptions = ajaxOpt;
            var builder = new jQueryAjaxOptionsBuilder(ajaxOpt);
            if (ajaxOptions != null)
                ajaxOptions.Invoke(builder);
            return this;
        }
        //Events
        public TabPropertiesBuilder ClientSideEvents(Action<TabClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new TabClientSideEventsBuilder(this.tab.TabModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        public TabPropertiesBuilder Items(Action<TabBaseItemAdder> items)
        {
            this.ItemsCollection = new List<TabBaseItem>();
            TabBaseItemAdder tabAdded = new TabBaseItemAdder(this.tab.TabModel.Items);
            items.Invoke(tabAdded);
            return this as TabPropertiesBuilder;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(tab.Render().ToString());
        }
        public override String ToString()
        {
            return Render().ToString();
        }
        public List<TabBaseItem> ItemsCollection { get; set; }
    }
}
