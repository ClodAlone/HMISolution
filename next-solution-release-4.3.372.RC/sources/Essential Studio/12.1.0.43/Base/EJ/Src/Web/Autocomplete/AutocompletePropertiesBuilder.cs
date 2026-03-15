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

namespace Syncfusion.JavaScript
{
    public class AutocompletePropertiesBuilder
    {
        public Autocomplete autocomplete;

        public AutocompletePropertiesBuilder(Autocomplete autocomplete)
        { this.autocomplete = new Autocomplete(autocomplete.ID, autocomplete.AutocompleteModel); }

        public AutocompletePropertiesBuilder()
        {
        }
        //Boolean values
        public AutocompletePropertiesBuilder Grouping()
        {
            autocomplete.AutocompleteModel.Grouping = true;
            return this;
        }
        public AutocompletePropertiesBuilder Grouping(bool grouping)
        {
            autocomplete.AutocompleteModel.Grouping = grouping;
            return this;
        }
        public AutocompletePropertiesBuilder Distinct()
        {
            autocomplete.AutocompleteModel.Distinct = true;
            return this;
        }
        public AutocompletePropertiesBuilder Distinct(bool distinct)
        {
            autocomplete.AutocompleteModel.Distinct = distinct;
            return this;
        }
        public AutocompletePropertiesBuilder AllowSorting()
        {
            autocomplete.AutocompleteModel.AllowSorting = true;
            return this;
        }
        public AutocompletePropertiesBuilder AllowSorting(bool allowSorting)
        {
            autocomplete.AutocompleteModel.AllowSorting = allowSorting;
            return this;
        }
        public AutocompletePropertiesBuilder RoundedCorner()
        {
            autocomplete.AutocompleteModel.RoundedCorner = true;
            return this;
        }
        public AutocompletePropertiesBuilder RoundedCorner(bool roundedCorner)
        {
            autocomplete.AutocompleteModel.RoundedCorner = roundedCorner;
            return this;
        }
        public AutocompletePropertiesBuilder Readonly()
        {
            autocomplete.AutocompleteModel.ReadOnly = true;
            return this;
        }
        public AutocompletePropertiesBuilder Readonly(bool readOnly)
        {
            autocomplete.AutocompleteModel.ReadOnly = readOnly;
            return this;
        }
        public AutocompletePropertiesBuilder CaseSensitive()
        {
            autocomplete.AutocompleteModel.CaseSensitive = true;
            return this;
        }
        public AutocompletePropertiesBuilder CaseSensitive(bool caseSensitive)
        {
            autocomplete.AutocompleteModel.CaseSensitive = caseSensitive;
            return this;
        }
        public AutocompletePropertiesBuilder LoadingImage()
        {
            autocomplete.AutocompleteModel.LoadingImage = true;
            return this;
        }
        public AutocompletePropertiesBuilder LoadingImage(bool loadingImage)
        {
            autocomplete.AutocompleteModel.LoadingImage = loadingImage;
            return this;
        }
        public AutocompletePropertiesBuilder Dropdown()
        {
            autocomplete.AutocompleteModel.Dropdown = true;
            return this;
        }
        public AutocompletePropertiesBuilder Dropdown(bool dropdown)
        {
            autocomplete.AutocompleteModel.Dropdown = dropdown;
            return this;
        }
        public AutocompletePropertiesBuilder HighlightSearch()
        {
            autocomplete.AutocompleteModel.HighlightSearch = true;
            return this;
        }
        public AutocompletePropertiesBuilder HighlightSearch(bool highlightSearch)
        {
            autocomplete.AutocompleteModel.HighlightSearch = highlightSearch;
            return this;
        }
        public AutocompletePropertiesBuilder AutoFill()
        {
            autocomplete.AutocompleteModel.AutoFill = true;
            return this;
        }
        public AutocompletePropertiesBuilder AutoFill(bool autoFill)
        {
            autocomplete.AutocompleteModel.AutoFill = autoFill;
            return this;
        }
        public AutocompletePropertiesBuilder Rtl()
        {
            autocomplete.AutocompleteModel.Rtl = true;
            return this;
        }
        public AutocompletePropertiesBuilder Rtl(bool rtl)
        {
            autocomplete.AutocompleteModel.Rtl = rtl;
            return this;
        }
        public AutocompletePropertiesBuilder Persist()
        {
            autocomplete.AutocompleteModel.Persist = true;
            return this;
        }
        public AutocompletePropertiesBuilder Persist(bool persist)
        {
            autocomplete.AutocompleteModel.Persist = persist;
            return this;
        }
        public AutocompletePropertiesBuilder Enabled()
        {
            autocomplete.AutocompleteModel.Enabled = true;
            return this;
        }
        public AutocompletePropertiesBuilder Enabled(bool enabled)
        {
            autocomplete.AutocompleteModel.Enabled = enabled;
            return this;
        }
        public AutocompletePropertiesBuilder ShowNoResults()
        {
            autocomplete.AutocompleteModel.ShowNoResults = true;
            return this;
        }
        public AutocompletePropertiesBuilder ShowNoResults(bool showNoResults)
        {
            autocomplete.AutocompleteModel.ShowNoResults = showNoResults;
            return this;
        }        
        public AutocompletePropertiesBuilder AllowNew()
        {
            autocomplete.AutocompleteModel.AllowNew = true;
            return this;
        }
        public AutocompletePropertiesBuilder AllowNew(bool allowNew)
        {
            autocomplete.AutocompleteModel.AllowNew = allowNew;
            return this;
        }
        //EnumValues
        public AutocompletePropertiesBuilder Filter(FilterOperatorType filter)
        {
            autocomplete.AutocompleteModel.Filter= filter;
            return this;
        }
        public AutocompletePropertiesBuilder SortBy(SortOrder sortBy)
        {
            autocomplete.AutocompleteModel.SortBy = sortBy;
            return this;
        }
        public AutocompletePropertiesBuilder MultiSelectMode(MultiSelectModeTypes multiSelectMode)
        {
            autocomplete.AutocompleteModel.MultiSelectMode = multiSelectMode;
            return this;
        }
        //object values
        public AutocompletePropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            autocomplete.AutocompleteModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public AutocompletePropertiesBuilder Datasource(DataSource dataSource)
        {
            autocomplete.AutocompleteModel.DataSource = dataSource;
            return this;
        }
        public AutocompletePropertiesBuilder Datasource(IEnumerable dataSource)
        {
            autocomplete.AutocompleteModel.DataSource = dataSource;
            return this;
        }
        // fields
        public AutocompletePropertiesBuilder AutocompleteFields(Action<AutocompleteFieldsBuilder> fields)
        {
            var flds = new AutocompleteFields();
            autocomplete.AutocompleteModel.AutocompleteFields = flds;
            var builder = new AutocompleteFieldsBuilder(flds);
            if (fields != null)
                fields.Invoke(builder);
            return this;
        }
        //String Values
        public AutocompletePropertiesBuilder Height(String height)
        {
            autocomplete.AutocompleteModel.Height = height;
            return this;
        }
        public AutocompletePropertiesBuilder Width(String width)
        {
            autocomplete.AutocompleteModel.Width = width;
            return this;
        }
        public AutocompletePropertiesBuilder CssClass(String cssClass)
        {
            autocomplete.AutocompleteModel.CssClass = cssClass;
            return this;
        }
        public AutocompletePropertiesBuilder Delimiter(String delimiter)
        {
            autocomplete.AutocompleteModel.Delimiter = delimiter;
            return this;
        }
        public AutocompletePropertiesBuilder Value(String value)
        {
            autocomplete.AutocompleteModel.Value = value;
            return this;
        }
        public AutocompletePropertiesBuilder Template(String template)
        {
            autocomplete.AutocompleteModel.Template = template;
            return this;
        }
        public AutocompletePropertiesBuilder Watermark(String watermark)
        {
            autocomplete.AutocompleteModel.Watermark = watermark;
            return this;
        }
        public AutocompletePropertiesBuilder NoResults(String noResults)
        {
            autocomplete.AutocompleteModel.NoResults = noResults;
            return this;
        }
        public AutocompletePropertiesBuilder SuggestionBoxHeight(String suggestionBoxHeight)
        {
            autocomplete.AutocompleteModel.SuggestionBoxHeight = suggestionBoxHeight;
            return this;
        }
        public AutocompletePropertiesBuilder SuggestionBoxWidth(String suggestionBoxWidth)
        {
            autocomplete.AutocompleteModel.SuggestionBoxWidth = suggestionBoxWidth;
            return this;
        }
        public AutocompletePropertiesBuilder AddNewText(String addNewText)
        {
            autocomplete.AutocompleteModel.AddNewText = addNewText;
            return this;
        }

        //Integers
        public AutocompletePropertiesBuilder ListSize(int listSize)
        {
            autocomplete.AutocompleteModel.ListSize = listSize;
            return this;
        }
        public AutocompletePropertiesBuilder MinCharacter(int minCharacter)
        {
            autocomplete.AutocompleteModel.MinCharacter = minCharacter;
            return this;
        }
        public AutocompletePropertiesBuilder Query(String query)
        {
            autocomplete.AutocompleteModel.Query = query;
            return this;
        }
        //Events
        public AutocompletePropertiesBuilder ClientSideEvents(Action<AutocompleteClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new AutocompleteClientSideEventsBuilder(this.autocomplete.AutocompleteModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render 
        public HtmlString Render()
        {
            return new HtmlString(autocomplete.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }
    }
}
