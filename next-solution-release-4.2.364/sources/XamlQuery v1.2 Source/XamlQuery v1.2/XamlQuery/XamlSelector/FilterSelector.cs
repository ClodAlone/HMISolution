//FilterSelector.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace XamlQuery.XamlSelector
{
    internal class FilterSelector
    {
        #region Properties

        public string Query { get; set; }
        public FilterSelectorType Type { get; set; }
        public List<Token> Tokens { get; set; }

        #endregion

        #region Constructors

        public FilterSelector()
        {
            Query = string.Empty;
            Type = FilterSelectorType.None;
            Tokens = new List<Token>();
        }

        #endregion

        #region Methods

        public ControlSet Execute(ControlSet superSet)
        {
            Common.AddToLog("executing filter-q: " + this.Query);

            if (Tokens.Count < 2) return (new ControlSet()); //filter-selector should have minimum two tokens, the first one being the filter-delimiter symbol

            ControlSet matchedControls = new ControlSet();
            if (Type == FilterSelectorType.Style)
            {
                string styleName = Tokens[1].Content;
                Style styleObject = ControlUtility.GetStyleByName(styleName);
                if (styleObject != null)
                {
                    foreach (DependencyObject control in superSet) //find controls by style
                    {
                        if (control is FrameworkElement)
                        {
                            FrameworkElement element = (FrameworkElement)control;
                            if (element.Style != null)
                            {
                                //if (element.Style.Equals(styleObject))
                                if (ControlUtility.IsStyleEqualsOrBasedOn(element.Style, styleObject))
                                {
                                    Common.AddToLog("matched-by-style: " + element.Name + " based on style");
                                    matchedControls.Add(control);
                                }
                            }
                        }
                    }
                }
            }
            else if (Type == FilterSelectorType.Name)
            {
                string name = Tokens[1].Content;

                foreach (DependencyObject control in superSet) //find control by name
                {
                    if (control is FrameworkElement)
                    {
                        if (((FrameworkElement)control).Name.Equals(name))
                        {
                            Common.AddToLog("matched-by-name: " + ((FrameworkElement)control).Name);
                            matchedControls.Add(control);
                        }
                    }
                }
            }
            else if (Type == FilterSelectorType.Property)
            {
                matchedControls.AddRange(FilterByPropertyQuery(superSet));
            }

            return (matchedControls);
        }

        #endregion

        #region Internal-Methods

        private ControlSet FilterByPropertyQuery(ControlSet superSet)
        {
            ControlSet matchedControls = new ControlSet();

            //prepare query-string
            string queryString = string.Concat((from token in this.Tokens select token.Content).ToArray());
            queryString = queryString.Replace("[", "");
            queryString = queryString.Replace("]", "");
            queryString = queryString.Replace("!=", "!");
            queryString = queryString.Replace("^=", "^");
            queryString = queryString.Replace("$=", "$");
            queryString = queryString.Replace("~=", "~");
            Common.AddToLog("clean filter-query: " + queryString);

            //find controls using property-details
            FilterType filterType = ResolveFilterType(queryString);
            string[] propertyParts = queryString.Split(Parser.PropertyValueDelimiterSymbols.ToCharArray());
            if (propertyParts.Length > 0)
            {
                string propertyNameDescriptor = propertyParts[0];
                string propertyRawValue = propertyParts.Length > 1 ? propertyParts[1] : string.Empty;

                ControlSet subSet = FilterByDependencyProperty(superSet, filterType, propertyParts.Length, propertyNameDescriptor, propertyRawValue);
                if (subSet == null) //if not dependency property, check normal property
                {
                    subSet = FilterByNormalProperty(superSet, filterType, propertyParts.Length, propertyNameDescriptor, propertyRawValue);
                }
                matchedControls.AddRange(subSet);
            }

            return (matchedControls);
        }

        private ControlSet FilterByDependencyProperty(ControlSet superSet, FilterType filterType, int propertyPartsLength, string propertyNameDescriptor, string propertyRawValue)
        {
            Common.AddToLog("checking for dependency-property: " + propertyNameDescriptor);

            FieldInfo dependencyPropertyField = ControlUtility.ResolveDependencyProperty(propertyNameDescriptor);
            if (dependencyPropertyField != null) //if it is a dependency property
            {
                ControlSet matchedControls = new ControlSet();

                foreach (DependencyObject control in superSet)
                {
                    object dependencyPropertyObject = dependencyPropertyField.GetValue(control);
                    if (dependencyPropertyObject is DependencyProperty)
                    {
                        DependencyProperty dependencyProperty = (DependencyProperty)dependencyPropertyObject;
                        object dependencyPropertyValue = control.GetValue(dependencyProperty);
                        if (propertyPartsLength == 1)
                        {
                            object defaultValue = dependencyProperty.GetType().IsValueType ? Activator.CreateInstance(dependencyProperty.GetType()) : null;
                            if (dependencyPropertyValue != defaultValue)
                            {
                                Common.AddToLog("matched by dependency-property-name: " + control.ToString());
                                matchedControls.Add(control);
                            }
                        }
                        else if (propertyPartsLength == 2)
                        {
                            if (ControlSet.CheckPropertyValue(dependencyPropertyValue, propertyRawValue, filterType))
                            {
                                Common.AddToLog("matched by dependency-property-value: " + control.ToString());
                                matchedControls.Add(control);
                            }
                        }
                    }
                }

                return (matchedControls);
            }
            else
            {
                Common.AddToLog("dependency-property-null: " + propertyNameDescriptor);
            }

            return (null);
        }

        private ControlSet FilterByNormalProperty(ControlSet superSet, FilterType filterType, int propertyPartsLength, string propertyName, string propertyRawValue)
        {
            Common.AddToLog("checking for property: " + propertyName);

            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in superSet)
            {
                PropertyInfo propertyInfo = control.GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    object propertyValue = propertyInfo.GetValue(control, null);
                    if (propertyPartsLength == 1)
                    {
                        object defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
                        if (propertyValue != defaultValue)
                        {
                            Common.AddToLog("matched-by-property-name: " + control.ToString());
                            matchedControls.Add(control);
                        }
                    }
                    else if (propertyPartsLength == 2)
                    {
                        if (ControlSet.CheckPropertyValue(propertyValue, propertyRawValue, filterType))
                        {
                            Common.AddToLog("matched-by-property-value: " + control.ToString());
                            matchedControls.Add(control);
                        }
                        else
                        {
                            Common.AddToLog("not-matched: " + propertyValue + ", " + propertyRawValue);
                        }
                    }
                }
                else
                {
                    Common.AddToLog("property-null: " + propertyName);
                }
            }

            return (matchedControls);
        }

        private FilterType ResolveFilterType(string query)
        {
            if (query.IndexOf("=") >= 0)
            {
                return (FilterType.Equals);
            }
            else if (query.IndexOf("!") >= 0)
            {
                return (FilterType.NotEquals);
            }
            else if (query.IndexOf("^") >= 0)
            {
                return (FilterType.StartsWith);
            }
            else if (query.IndexOf("$") >= 0)
            {
                return (FilterType.EndsWith);
            }
            else if (query.IndexOf("~") >= 0)
            {
                return (FilterType.Contains);
            }
            return (0);
        }

        #endregion

        #region Static-Methods

        public static FilterSelector Parse(string query)
        {
            Common.AddToLog("parsing filter-query: [" + query + "]");

            List<Token> allTokens = Parser.Tokenize(query);
            if (allTokens.Count == 0) return (null);
            if (allTokens[0].Type != TokenType.Symbol) return (null); //first token of a filter-query must be a symbol

            FilterSelector filterSelector = new FilterSelector();
            filterSelector.Query = query;
            filterSelector.Tokens.AddRange(allTokens);

            //resolve filter-selector type
            switch (allTokens[0].Content)
            {
                case ".":
                    filterSelector.Type = FilterSelectorType.Style;
                    break;
                case "#":
                    filterSelector.Type = FilterSelectorType.Name;
                    break;
                case "[":
                    filterSelector.Type = FilterSelectorType.Property;
                    break;
            }

            return (filterSelector);
        }

        #endregion
    }
}
