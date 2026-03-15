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
namespace Syncfusion.JavaScript
{
    public class ColumnBuilder<T> where T : class
    {
        private GridProperties<T> model;
        private List<Column<T>> Columns;
        private Column<T> currentColumn = new Column<T>();

        public ColumnBuilder(Grid<T> grid)
        {
            model = grid.GridModel;
            Columns = new List<Column<T>>();
            this.model.Columns = new List<Column<T>>();

        }

        public ColumnBuilder<T> Field(String field)
        {
            currentColumn.Field = field;
            currentColumn.HeaderText = currentColumn.Field;
            return this;
        }
        public ColumnBuilder<T> HeaderText()
        {
            currentColumn.HeaderText = currentColumn.Field;
            return this;
        }
        public ColumnBuilder<T> HeaderText(String headerText)
        {
            currentColumn.HeaderText = headerText;
            return this;
        }
        public ColumnBuilder<T> AllowGrouping()
        {
            currentColumn.AllowGrouping = true;
            return this;
        }
        public ColumnBuilder<T> AllowGrouping(bool allowGrouping)
        {
            currentColumn.AllowGrouping = allowGrouping;
            return this;
        }
        public ColumnBuilder<T> AllowFiltering()
        {
            currentColumn.AllowFiltering = true;
            return this;
        }
        public ColumnBuilder<T> AllowFiltering(bool allowFiltering)
        {
            currentColumn.AllowFiltering = allowFiltering;
            return this;
        }
        public ColumnBuilder<T> AllowSorting()
        {
            currentColumn.AllowSorting = true;
            return this;
        }
        public ColumnBuilder<T> AllowSorting(bool allowSorting)
        {
            currentColumn.AllowSorting = allowSorting;
            return this;
        }
        public ColumnBuilder<T> AllowEditing()
        {
            currentColumn.AllowEditing = true;
            return this;
        }
        public ColumnBuilder<T> AllowEditing(bool allowEditing)
        {
            currentColumn.AllowEditing = allowEditing;
            return this;
        }
        public ColumnBuilder<T> ColumnTemplate(bool columnTemplate)
        {
            currentColumn.ColumnTemplate = columnTemplate;
            return this;
        }
        public ColumnBuilder<T> EditingType(EditingType editingType)
        {
            currentColumn.EditingType = editingType;
            return this;
        }
        public ColumnBuilder<T> TextAlign(TextAlign textAlign)
        {
            currentColumn.TextAlign = textAlign;
            return this;
        }
        public ColumnBuilder<T> IsIdentity(bool isIdentity)
        {
            currentColumn.IsIdentity = isIdentity;
            return this;
        }
        public ColumnBuilder<T> Key(bool key)
        {
            currentColumn.Key = key;
            return this;
        }
        public ColumnBuilder<T> Visible()
        {
            currentColumn.Visible = true;
            return this;
        }
        public ColumnBuilder<T> Visible(bool visible)
        {
            currentColumn.Visible = visible;
            return this;
        }
        public ColumnBuilder<T> IsUnbound(bool isUnbound)
        {
            currentColumn.IsUnbound = isUnbound;
            return this;
        }       
        public ColumnBuilder<T> DataSource(List<String> dataSource) //The type of the dropdown might be IEnumerable,Need to be verified.
        {
            currentColumn.DataSource = dataSource;
            return this;
        }
        public ColumnBuilder<T> DataSource(IEnumerable dataSource)
        {
            currentColumn.DataSource = dataSource;
            return this;
        }
        public ColumnBuilder<T> ForeignKeyField(String foreignKeyField)
        {
            currentColumn.ForeignKeyField = foreignKeyField;
            return this;
        }
        public ColumnBuilder<T> ForeignKeyValue(String foreignKeyValue)
        {
            currentColumn.ForeignKeyValue = foreignKeyValue;
            return this;
        }
        public ColumnBuilder<T> HeaderTemplateId(String headerTemplateId)
        {
            currentColumn.HeaderTemplateId = headerTemplateId;
            return this;
        }
        public ColumnBuilder<T> TemplateId(String templateId)
        {
            currentColumn.TemplateId = templateId;
            return this;
        }
        public ColumnBuilder<T> Width(int width)
        {
            currentColumn.Width = width;
            return this;
        }
        public ColumnBuilder<T> CssClass(string cssclass) 
        {
            currentColumn.CssClass = cssclass;
            return this;
        }
        public ColumnBuilder<T> DefaultValue(object defaultvalue) 
        {
            currentColumn.DefaultValue = defaultvalue;
            return this;
        }
        public ColumnBuilder<T> ValidationRules(Action<ValidationRuleBuilder<T>> validationRule)
        {
            var builder = new ValidationRuleBuilder<T>(currentColumn);
            if (builder != null)
                validationRule.Invoke(builder);
            return this;
        }
        public ColumnBuilder<T> ValidationRules(Dictionary<String, object> validationRule)
        {
            currentColumn.ValidationRules = validationRule;
            return this;
        }
        public ColumnBuilder<T> Format(String format)
        {
            currentColumn.Format = format;
            return this;
        }
        public ColumnBuilder<T> CustomAttributes(Action<CustomAttributeBuilder<T>> customAttributes)
        {
            var builder = new CustomAttributeBuilder<T>(currentColumn);
            if (customAttributes != null)
                customAttributes.Invoke(builder);
            return this;
        }
        public ColumnBuilder<T> CustomAttributes(Dictionary<String, object> customAttributes)
        {
            currentColumn.CustomAttributes = customAttributes;
            return this;
        }
        public ColumnBuilder<T> Commands(Action<CommandsBuilder<T>> commands)
        {
            var builder = new CommandsBuilder<T>(currentColumn, currentColumn.Commands);
            if (builder != null)
                commands.Invoke(builder);

            return this;
        }
        public ColumnBuilder<T> Commands(List<Commands<T>> commands)
        {
            currentColumn.Commands = commands;
            return this;
        }
        public void Add()
        {
            this.model.Columns.Add(currentColumn);
            this.Columns.Add(currentColumn);
            currentColumn = new Column<T>();
            //return this;
        }
        //public ColumnBuilder<T> Commands(Action<NumericEditParamBuilder<T>> numericeditparam)
        //{
        //    var builder = new NumericEditParamBuilder<T>();
        //    if (builder != null)
        //        commands.Invoke(builder);
        //    return this;
        //}
    }
}
