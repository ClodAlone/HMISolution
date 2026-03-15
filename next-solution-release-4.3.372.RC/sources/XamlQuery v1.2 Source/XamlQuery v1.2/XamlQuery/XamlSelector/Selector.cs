//Selector.cs

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

namespace XamlQuery.XamlSelector
{
    internal class Selector
    {
        #region Properties

        public string Query { get; set; }
        public List<SimpleSelector> SimpleSelectors { get; set; }
        public List<Combinator> Combinators { get; set; }

        #endregion

        #region Constructors

        public Selector()
        {
            Query = string.Empty;
            SimpleSelectors = new List<SimpleSelector>();
            Combinators = new List<Combinator>();
        }

        #endregion

        #region Methods

        public static DependencyObject SourceControl = null;
        public ControlSet Execute(DependencyObject sourceControl)
        {
            Common.AddToLog("executing selector: " + this.Query);

            SourceControl = sourceControl;
            ControlSet superSet = XamlQuery.All(sourceControl);
            ControlSet matchedControls = new ControlSet();

            if (SimpleSelectors.Count == 1) //if only one simple-selector, then execute it and return the result
            {
                return (SimpleSelectors[0].Execute(superSet));
            }
            else if (SimpleSelectors.Count > 1) //if more than one simple-selectors, then apply combinators while executing
            {
                //apply each combinator to result of simple-selector that precedes it
                ControlSet currentSet = new ControlSet();
                currentSet.AddRange(superSet);
                for (int index = 0; index < SimpleSelectors.Count; index++)
                {
                    if (index > 0)
                    {
                        Common.AddToLog("**Executing Combinator: " + Combinators[index - 1].Token.Content);
                        currentSet = Combinators[index - 1].Execute(currentSet);
                        Common.AddToLog("**Combinator-Result: " + currentSet.Count + " controls");
                        if (Combinators[index - 1].Type == CombinatorType.Adjacent)
                        {
                            //for adjacent combinator
                            //   get first-set by executing simple-selector before + with result of combinator
                            //   get second-set by executing simple-selector after + with result of combinator
                            //   combine first and second sets as result of combinator and its simple-selectors
                            ControlSet firstSet = SimpleSelectors[index - 1].Execute(currentSet);
                            ControlSet secondSet = SimpleSelectors[index].Execute(currentSet);
                            currentSet.Clear();
                            currentSet.AddRange(firstSet);
                            currentSet.AddRange(secondSet);
                            Common.AddToLog("**First-Set-Result: " + firstSet.Count + " controls. " + string.Join(",", (from c in firstSet select c.GetType().ToString()).ToArray()));
                            Common.AddToLog("**Second-Set-Result: " + secondSet.Count + " controls. " + string.Join(",", (from c in secondSet select c.GetType().ToString()).ToArray()));
                            Common.AddToLog("**Simple-Selector-Result: " + currentSet.Count + " controls");
                            continue;
                        }
                    }
                    Common.AddToLog("**Executing Simple-Selector: " + SimpleSelectors[index].Query);
                    currentSet = SimpleSelectors[index].Execute(currentSet);
                    Common.AddToLog("**Simple-Selector-Result: " + currentSet.Count + " controls");
                }
                matchedControls.AddRange(currentSet);
            }

            return (matchedControls);
        }

        #endregion

        #region Static-Methods

        public static Selector Parse(string query)
        {
            Common.AddToLog("parsing selector-query: " + query);

            Selector selector = new Selector();
            selector.Query = query;

            //resolve all simple-queries
            string[] allSimpleQueries = query.Split(Parser.CombinatorSymbols.ToCharArray());
            foreach (string simpleQuery in allSimpleQueries)
            {
                SimpleSelector simpleSelector = SimpleSelector.Parse(simpleQuery);
                if (simpleSelector != null)
                {
                    selector.SimpleSelectors.Add(simpleSelector);
                }
            }

            //fill combinators
            char[] q = query.ToCharArray();
            foreach (char c in q)
            {
                if (Parser.CombinatorSymbols.IndexOf(c) >= 0)
                {
                    selector.Combinators.Add(Combinator.Parse(c));
                }
            }

            return (selector);
        }

        #endregion
    }
}
