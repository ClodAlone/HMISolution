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
using System.Web;
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript
{
    public class ToolBarOptionsBuilder
    {
        private ToolBarOptions toolBar=new ToolBarOptions();

        public ToolBarOptionsBuilder(ToolBarOptions tools)
        {
            toolBar = tools;
        }
        public ToolBarOptionsBuilder AllowToolBar()
        {
            toolBar.AllowToolbar = true;
            return this;
        }
        public ToolBarOptionsBuilder AllowToolBar(bool allowToolbar)
        {
            toolBar.AllowToolbar = allowToolbar;
            return this;
        }
       
        public ToolBarOptionsBuilder ToolBarItems(List<GanttToolBarItems> defaultItems)
        {
            List<GanttToolBarItems> defaultList = new List<GanttToolBarItems>();
            defaultList = defaultItems;
            foreach (var enumItem in defaultList)
            { 
                toolBar.ToolBarItems.Add(EnumToString.StringValue(enumItem));
            }
            return this;
        }

        //public ToolBarOptionsBuilder ToolBarItems(Action<ToolBarItemsBuilder<T>> defaultItems)
        //{
        //    var builder = new ToolBarItemsBuilder<T>(this.toolBar);
        //    if (defaultItems != null)
        //        defaultItems.Invoke(builder);
        //    return this;
        //}

    }
}