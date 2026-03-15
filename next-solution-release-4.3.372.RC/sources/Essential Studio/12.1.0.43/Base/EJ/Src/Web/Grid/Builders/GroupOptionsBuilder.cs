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
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript
{
    public class GroupOptionsBuilder<T> where T : class
    {
       
        private GroupOptions<T> groupOption = new GroupOptions<T>();
        public GroupOptionsBuilder(GroupOptions<T> group)
        {
            groupOption = group;
        }
        public GroupOptionsBuilder<T> ToggleGroup()
        {
            groupOption.ToggleGroup = true;
            return this;
        }
        public GroupOptionsBuilder<T> ToggleGroup(bool toogleGroup)
        {
            groupOption.ToggleGroup = toogleGroup;
            return this;
        }
        public GroupOptionsBuilder<T> GroupedColumnShow(bool groupedColumnShow)
        {
            groupOption.GroupedColumnShow = groupedColumnShow;
            return this;
        }
        public GroupOptionsBuilder<T> GroupedColumnShow()
        {
            groupOption.GroupedColumnShow = true;
            return this;
        }
        public GroupOptionsBuilder<T> ShowUngroupButton()
        {
            groupOption.ShowUngroupButton = true;
            return this;
        }
        public GroupOptionsBuilder<T> ShowUngroupButton(bool showUnGroupButton)
        {
            groupOption.ShowUngroupButton = showUnGroupButton;
            return this;
        }
        public GroupOptionsBuilder<T> ShowAnimateButton()
        {
            groupOption.ShowAnimateButton = true;
            return this;
        }
        public GroupOptionsBuilder<T> ShowAnimateButton(bool showAnimateButton)
        {
            groupOption.ShowAnimateButton = showAnimateButton;
            return this;
        }
        public GroupOptionsBuilder<T> GroupedColumn(Action<GroupedColumnBuilder<T>> groupedColumn)
        {
            var builder = new GroupedColumnBuilder<T>(groupOption);
            if (groupedColumn != null)
                groupedColumn.Invoke(builder);
            return this;
        }
        public GroupOptionsBuilder<T> GroupedColumn(List<String> groupedColumn)
        {
            groupOption.GroupedColumn = groupedColumn;
            return this;
        }
        
       
    }
}
