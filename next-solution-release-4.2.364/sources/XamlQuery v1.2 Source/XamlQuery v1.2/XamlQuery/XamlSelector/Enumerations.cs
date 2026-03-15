//Enumerations.cs

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

namespace XamlQuery.XamlSelector
{
    internal enum TokenType
    {
        None = 0,
        Identifier = 1,
        Symbol = 2
    }

    internal enum SimpleSelectorType
    {
        None = 0,
        Universal = 1,
        Element = 2
    }

    internal enum FilterSelectorType
    {
        None = 0,
        Style = 1,
        Name = 2,
        Property = 3
    }

    internal enum CombinatorType
    {
        None = 0,
        Descendant = 2,
        Child = 3,
        Adjacent = 4
    }
}
