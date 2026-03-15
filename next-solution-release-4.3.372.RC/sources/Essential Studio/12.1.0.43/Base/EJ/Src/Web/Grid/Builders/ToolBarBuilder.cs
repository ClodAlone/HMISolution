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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class ToolBarBuilder<T> where T : class
    {

        private ToolBar<T> toolBar = new ToolBar<T>();
        public ToolBarBuilder(ToolBar<T> tools)
        {
            toolBar = tools;
        }
        public ToolBarBuilder<T> AllowToolBar()
        {
            toolBar.AllowToolbar = true;
            return this;
        }
        public ToolBarBuilder<T> AllowToolBar(bool allowToolbar)
        {
            toolBar.AllowToolbar = allowToolbar;
            return this;
        }
        public ToolBarBuilder<T> ToolBarItems(Action<ToolBarItemsBuilder<T>> defaultItems)
        {
            var builder = new ToolBarItemsBuilder<T>(this.toolBar);
            if (defaultItems != null)
                defaultItems.Invoke(builder);
            return this;
        }
        public ToolBarBuilder<T> ToolBarItems(List<ToolBarItems> defaultItems)
        {
            List<ToolBarItems> defaultList = new List<ToolBarItems>();
            defaultList = defaultItems;
            foreach (var enumItem in defaultList)
            { 
                toolBar.ToolBarItems.Add(EnumToString.StringValue(enumItem));
            }
            return this;
        }
        public ToolBarBuilder<T> CustomToolbarItems(List<Object> customItems)
        {
            this.toolBar.CustomToolbarItems = customItems;                
            return this;
        }
    }
}
