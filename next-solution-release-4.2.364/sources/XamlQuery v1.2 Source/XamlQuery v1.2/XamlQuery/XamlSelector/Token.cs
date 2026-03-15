//Token.cs

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

namespace XamlQuery.XamlSelector
{
    internal class Token
    {
        #region Properties

        public TokenType Type { get; set; }
        public string Content { get; set; }

        #endregion

        #region Constructors

        public Token()
        {
            Type = TokenType.None;
            Content = string.Empty;
        }

        #endregion
    }
}
