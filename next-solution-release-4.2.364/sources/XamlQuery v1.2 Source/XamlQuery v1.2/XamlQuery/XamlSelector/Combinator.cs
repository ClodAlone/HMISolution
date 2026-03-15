//Combinator.cs

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
using System.Windows.Controls;

namespace XamlQuery.XamlSelector
{
    internal class Combinator
    {
        #region Properties

        public CombinatorType Type { get; set; }
        public Token Token { get; set; }

        #endregion

        #region Constructors

        public Combinator()
        {
            Type = CombinatorType.None;
            Token = new Token();
        }

        #endregion

        #region Methods

        public ControlSet Execute(ControlSet superSet)
        {
            Common.AddToLog("executing combinator: " + this.Type);

            ControlSet result = new ControlSet();

            switch (this.Type)
            {
                case CombinatorType.Descendant:
                    //get descendants (children in visual-tree) of all controls in superset
                    foreach (DependencyObject control in superSet)
                    {
                        ControlSet controlSet = XamlQuery.All(control);
                        foreach (DependencyObject c in controlSet)
                        {
                            if (!result.Contains(c))
                            {
                                result.Add(c);
                            }
                        }
                    }
                    break;
                case CombinatorType.Child:
                    //get children of all Panel controls in superset
                    foreach (DependencyObject control in superSet)
                    {
                        if (control is Panel)
                        {
                            result.AddRange(from child in ((Panel)control).Children select (DependencyObject)child);
                        }
                    }
                    break;
                case CombinatorType.Adjacent:
                    //get children of all parent Panels of all controls in superset
                    //if a parent is not a Panel, then it is ignored
                    foreach (DependencyObject control in superSet)
                    {
                        if (control is FrameworkElement)
                        {
                            DependencyObject parent = ((FrameworkElement)control).Parent;
                            if (parent is Panel)
                            {
                                result.AddRange(from child in ((Panel)parent).Children select (DependencyObject)child);
                            }
                        }
                    }

                    //include original controls also in result
                    result.AddRange(superSet);
                    Common.AddToLog("adj-superset-added");
                    break;
            }

            //remove duplicate-controls
            ControlSet uniqueControls = new ControlSet();
            foreach (DependencyObject c in result)
            {
                if (!uniqueControls.Contains(c))
                {
                    uniqueControls.Add(c);
                }
            }

            return (uniqueControls);
        }

        #endregion

        #region Static-Methods

        public static Combinator Parse(char c)
        {
            Combinator combinator = new Combinator();

            //set combinator-type
            switch (c)
            {
                case ' ':
                    combinator.Type = CombinatorType.Descendant;
                    break;
                case '>':
                    combinator.Type = CombinatorType.Child;
                    break;
                case '+':
                    combinator.Type = CombinatorType.Adjacent;
                    break;
            }

            //set combinator-token
            combinator.Token = new Token()
            {
                Type = TokenType.Symbol,
                Content = c + string.Empty
            };

            return (combinator);
        }

        #endregion
    }
}
