//SimpleSelector.cs

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
    internal class SimpleSelector
    {
        #region Properties

        public string Query { get; set; }
        public SimpleSelectorType Type { get; set; }
        public Token MainToken { get; set; }
        public List<FilterSelector> FilterSelectors { get; set; }

        #endregion

        #region Constructors

        public SimpleSelector()
        {
            Query = string.Empty;
            Type = SimpleSelectorType.None;
            MainToken = new Token();
            FilterSelectors = new List<FilterSelector>();
        }

        #endregion

        #region Methods

        public ControlSet Execute(ControlSet superSet)
        {
            Common.AddToLog("executing simple-query: " + this.Query);

            if (Type == SimpleSelectorType.None) return (new ControlSet());

            //set main controls
            ControlSet mainControls = new ControlSet();
            if (Type == SimpleSelectorType.Universal)
            {
                mainControls.AddRange(superSet);
            }
            else if (Type == SimpleSelectorType.Element)
            {
                Type givenControlType = ControlUtility.ResolveControlType(MainToken.Content);
                if (givenControlType != null)
                {
                    foreach (DependencyObject control in superSet)
                    {
                        //if (givenControlType.IsAssignableFrom(control.GetType()))
                        //if (givenControlType.IsSubclassOf(control.GetType()))
                        //if (givenControlType.FullName.Equals(control.GetType().FullName))
                        if (givenControlType.IsInstanceOfType(control))
                        {
                            mainControls.Add(control);
                        }
                    }
                }
            }

            //apply filter-selectors
            if (this.FilterSelectors.Count == 0)
            {
                return (mainControls);
            }
            else
            {
                foreach (FilterSelector filterSelector in this.FilterSelectors)
                {
                    mainControls = filterSelector.Execute(mainControls);
                }
                return (mainControls);
            }
        }

        #endregion

        #region Static-Methods

        public static SimpleSelector Parse(string query)
        {
            Common.AddToLog("parsing simple-query: " + query);

            SimpleSelector simpleSelector = new SimpleSelector();
            simpleSelector.Query = query;

            List<Token> allTokens = Parser.Tokenize(query);
            if (allTokens.Count == 0) return (null);
            int filterStartIndex = -1;

            #region Resolve main-token

            if (allTokens[0].Type == TokenType.Symbol) //if first token is a symbol
            {
                if (allTokens[0].Content.Equals("*")) //if first-token is universal
                {
                    simpleSelector.Type = SimpleSelectorType.Universal;
                    simpleSelector.MainToken = allTokens[0];
                    filterStartIndex = 1;
                }
                else //if first is a symbol but not universal, then assume as universal
                {
                    simpleSelector.Type = SimpleSelectorType.Universal;
                    simpleSelector.MainToken = new Token() { Type = TokenType.Symbol, Content = "*" };
                    filterStartIndex = 0;
                }
            }
            else if (allTokens[0].Type == TokenType.Identifier) //if first token is an identifier (element-selector)
            {
                simpleSelector.Type = SimpleSelectorType.Element;
                simpleSelector.MainToken = allTokens[0];
                filterStartIndex = 1;
            }

            if (filterStartIndex == -1)
            {
                return (simpleSelector);
            }

            #endregion

            #region Resolve all filter-selectors

            List<Token> filterTokens = allTokens.GetRange(filterStartIndex, allTokens.Count - filterStartIndex);
            string fullFilterQuery = string.Concat((from token in filterTokens select token.Content).ToArray());
            Common.AddToLog("splitting filters of simple-query: " + fullFilterQuery);

            //identify all filter-queries
            List<string> allFilterQueries = new List<string>();

            char[] q = fullFilterQuery.ToCharArray();
            string filterQueryText = string.Empty;
            foreach (char c in q)
            {
                if (Parser.FilterDelimiterSymbols.IndexOf(c) >= 0) //if it is a filter-delimiter
                {
                    if (filterQueryText.Length > 0)
                    {
                        allFilterQueries.Add(filterQueryText);
                    }

                    filterQueryText = c + string.Empty;
                }
                else
                {
                    filterQueryText += c;
                }
            }

            //check if any filter-query at last
            if (filterQueryText.Length > 0)
            {
                allFilterQueries.Add(filterQueryText);
            }

            Common.AddToLog(string.Join("//", allFilterQueries.ToArray()) + ", count: " + allFilterQueries.Count);

            //build filter-selector objects from filter-query strings
            foreach (string filterQuery in allFilterQueries)
            {
                FilterSelector filterSelector = FilterSelector.Parse(filterQuery);
                if (filterSelector != null)
                {
                    simpleSelector.FilterSelectors.Add(filterSelector);
                }
            }

            #endregion

            return (simpleSelector);
        }

        #endregion
    }
}
