#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.Models;
using System.Collections;
using Syncfusion.JavaScript.Mobile.Models;
namespace Syncfusion.JavaScript
{
    public class MobileColumnBuilder<T> where T : class
    {
        private MobileGridProperties<T> model;
        private List<MobileColumn<T>> Columns;
        private MobileColumn<T> currentColumn = new MobileColumn<T>();

        public MobileColumnBuilder(MobileGrid<T> grid)
        {
            model = grid.GridModel;
            Columns = new List<MobileColumn<T>>();
            this.model.Columns = new List<MobileColumn<T>>();

        }

        public MobileColumnBuilder<T> Field(String field)
        {
            currentColumn.Field = field;
            currentColumn.HeaderText = currentColumn.Field;
            return this;
        }
        public MobileColumnBuilder<T> HeaderText()
        {
            currentColumn.HeaderText = currentColumn.Field;
            return this;
        }
        public MobileColumnBuilder<T> HeaderText(String headerText)
        {
            currentColumn.HeaderText = headerText;
            return this;
        }

        public MobileColumnBuilder<T> AllowFiltering()
        {
            currentColumn.AllowFiltering = true;
            return this;
        }
        public MobileColumnBuilder<T> AllowFiltering(bool allowFiltering)
        {
            currentColumn.AllowFiltering = allowFiltering;
            return this;
        }
        public MobileColumnBuilder<T> AllowSorting()
        {
            currentColumn.AllowSorting = true;
            return this;
        }
        public MobileColumnBuilder<T> AllowSorting(bool allowSorting)
        {
            currentColumn.AllowSorting = allowSorting;
            return this;
        }

        public MobileColumnBuilder<T> TextAlign(TextAlign textAlign)
        {
            currentColumn.TextAlign = textAlign;
            return this;
        }

        public MobileColumnBuilder<T> Visible()
        {
            currentColumn.Visible = true;
            return this;
        }
        public MobileColumnBuilder<T> Visible(bool visible)
        {
            currentColumn.Visible = visible;
            return this;
        }

        public MobileColumnBuilder<T> DataSource(List<String> dataSource) //The type of the dropdown might be IEnumerable,Need to be verified.
        {
            currentColumn.DataSource = dataSource;
            return this;
        }
        public MobileColumnBuilder<T> DataSource(IEnumerable dataSource)
        {
            currentColumn.DataSource = dataSource;
            return this;
        }

        public MobileColumnBuilder<T> Width(int width)
        {
            currentColumn.Width = width;
            return this;
        }
        public MobileColumnBuilder<T> CssClass(string cssclass) 
        {
            currentColumn.CssClass = cssclass;
            return this;
        }

        public MobileColumnBuilder<T> ValidationRules(Action<MobileValidationRuleBuilder<T>> validationRule)
        {
            var builder = new MobileValidationRuleBuilder<T>(currentColumn);
            if (builder != null)
                validationRule.Invoke(builder);
            return this;
        }
        public MobileColumnBuilder<T> ValidationRules(Dictionary<String, String> validationRule)
        {
            currentColumn.ValidationRules = validationRule;
            return this;
        }
        public MobileColumnBuilder<T> Format(String format)
        {
            currentColumn.Format = format;
            return this;
        }
        //public MobileColumnBuilder<T> CustomAttributes(Action<CustomAttributeBuilder<T>> customAttributes)
        //{
        //    var builder = new CustomAttributeBuilder<T>(currentColumn);
        //    if (customAttributes != null)
        //        customAttributes.Invoke(builder);
        //    return this;
        //}
        //public MobileColumnBuilder<T> CustomAttributes(Dictionary<String, String> customAttributes)
        //{
        //    currentColumn.CustomAttributes = customAttributes;
        //    return this;
        //}
        public void Add()
        {
            this.model.Columns.Add(currentColumn);
            this.Columns.Add(currentColumn);
            currentColumn = new MobileColumn<T>();
            //return this;
        }
    }
}
