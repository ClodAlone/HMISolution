#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Syncfusion.Olap.MDXQueryParser
{
    /// <summary>
    /// Parse the MDXquery by splitting into Tokens
    /// </summary>
    public class Tokens
    {        
        public char Find;
        public bool EndReader;
        private StringReader SR;
        public string curToken { get; set; }

        public Tokens(string MDXquery)
        {
            this.SR = new StringReader(MDXquery);
            SplitChar();
            SplitToken();
        }

        /// <summary>
        /// Split the MDXQuery By Character
        /// </summary>
        public void SplitChar()
        {
            char[] ch = new char[1];
            int EndReader = SR.Read(ch, 0, 1);

            if (EndReader == 0)
            {
                this.EndReader = true;
            }
            Find = ch[0];
        }

        /// <summary>
        /// Add the splitted character and form a Token
        /// </summary>
        public void SplitToken()
        {
            StringBuilder sb = new StringBuilder();
            string str = string.Empty;

            while (Find == ' ')
            {
                SplitChar();
            }

            while (Find != ' ' && !this.EndReader)
            {
                sb.Append(Find);
                SplitChar();
            }
            this.curToken = sb.ToString();
        }

        /// <summary>
        /// Check the current token with the keyword
        /// </summary>
        /// <param name="keyword"></param>
        public void CheckToken(string keyword)
        {
            if (this.curToken.ToUpper() != keyword)
            {
                throw new QuerySyntaxError("PARSER ERROR: '" + keyword + "' Expected in the query.");
            }
            SplitToken();
        }
    }
}