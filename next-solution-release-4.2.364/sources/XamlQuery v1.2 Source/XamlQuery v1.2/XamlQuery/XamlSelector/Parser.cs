//Parser.cs

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
using System.Text.RegularExpressions;
using System.Windows;

namespace XamlQuery.XamlSelector
{
    internal class Parser
    {
        #region Constants

        /// <summary>
        /// List of valid characters supported by XamlSelector query string.
        /// 
        /// a-z FOR alphabets for identifier, property name and value
        /// A-Z FOR alphabets for identifier, property name and value
        /// 0-9 FOR numbers for identifier, property name and value
        /// , FOR selector delimiter
        /// * FOR universal selector
        /// . FOR class/style selector
        /// # FOR name selector
        /// > FOR child selector (combinator)
        /// + FOR adjacent selector (combinator)
        /// [ FOR property selector
        /// ] FOR property selector
        /// _ FOR property selector (delimiter b/w control name and property name, applies to dependency properties)
        /// = FOR property selector (equals)
        /// ! FOR property selector (not-equals)
        /// ^ FOR property selector (starts-with)
        /// $ FOR property selector (ends-with)
        /// ~ FOR property selector (contains-within-string)
        /// space FOR combinator
        /// \n FOR combinator
        /// \r FOR combinator
        /// \t FOR combinator
        /// 
        /// Future:
        ///     Symbols for pseudo-classes
        ///         _, (, ), -
        /// </summary>
        public const string AllCharacters = "a-zA-Z0-9";
        public const string AllSymbols = ",*.#>+[]_=!^$~ ";
        public const string WhiteSpaceSymbols = " \n\r\t"; //subset of all symbols
        public const string CombinatorSymbols = " >+"; //subset of all symbols
        public const string FilterDelimiterSymbols = ".#["; //subset of all symbols
        public const string PropertyValueDelimiterSymbols = "=!^$~"; //subset of all symbols

        #endregion

        #region Methods

        public static List<Selector> Parse(string fullQuery)
        {
            //strip white-space
            StripWhiteSpace(ref fullQuery);
            Common.AddToLog("parser: parsing: [" + fullQuery + "]");

            //check if valid query
            if (!IsValidQuery(fullQuery))
            {
                return (new List<Selector>());
            }

            //find all selectors in group
            //(group is comma-separated list of selectors)
            string[] allQueries = fullQuery.Split(',');

            List<Selector> allSelectors = new List<Selector>();
            foreach (string query in allQueries)
            {
                allSelectors.Add(Selector.Parse(query));
            }

            return (allSelectors);
        }

        public static List<Token> Tokenize(string query)
        {
            List<Token> allTokens = new List<Token>();

            char[] q = query.ToCharArray();
            string identifier = string.Empty;
            foreach (char c in q)
            {
                if (IsSymbol(c))
                {
                    if (identifier.Length > 0)
                    {
                        Token identifierToken = new Token() { Type = TokenType.Identifier, Content = identifier };
                        allTokens.Add(identifierToken);
                        identifier = string.Empty;
                    }

                    Token symbolToken = new Token() { Type = TokenType.Symbol, Content = "" + c };
                    allTokens.Add(symbolToken);
                }
                else
                {
                    identifier += c;
                }
            }

            //check if any token at last
            if (identifier.Length > 0)
            {
                Token identifierToken = new Token() { Type = TokenType.Identifier, Content = identifier };
                allTokens.Add(identifierToken);
            }

            return (allTokens);
        }

        #endregion

        #region Internal-Methods

        private static void StripWhiteSpace(ref string query)
        {
            //remove escape sequences
            query = query.Replace("\n", " ");
            query = query.Replace("\r", " ");
            query = query.Replace("\t", " ");
            query = query.Trim();

            //remove consecutive spaces
            while (query.IndexOf("  ") >= 0)
            {
                query = query.Replace("  ", string.Empty);
            }

            //remove spaces around combinators
            query = query.Replace(" >", ">");
            query = query.Replace("> ", ">");
            query = query.Replace(" +", "+");
            query = query.Replace("+ ", "+");
        }

        private static bool IsValidQuery(string query)
        {
            char[] q = query.ToCharArray();
            foreach (char c in q)
            {
                if (IsAlpha(c)) continue;
                if (IsNumber(c)) continue;
                if (IsSymbol(c)) continue;
                return (false);
            }
            return (true);
        }

        private static bool IsAlpha(char c)
        {
            return ((c >= 65 && c <= 90) || (c >= 97 && c <= 122));
        }

        private static bool IsNumber(char c)
        {
            return (c >= 48 && c <= 57);
        }

        private static bool IsSymbol(char c)
        {
            return (AllSymbols.IndexOf(c) >= 0);
        }

        private static bool IsIdentifier(string s)
        {
            return (Regex.IsMatch(s, "[a-zA-Z][a-zA-Z0-9]*"));
        }

        #endregion
    }
}
