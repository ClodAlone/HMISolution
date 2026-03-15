// <copyright file="BlockCodes.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Collections;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Blockcodes is an internal class used to hold a group of values for syntax highlighting purpose.
    /// </summary>

#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class BlockCodes
    {
        #region Properties

        /// <summary>
        /// instance for BlockStartLine property.
        /// </summary>
        private int startline;

        /// <summary>
        /// instance for CloseAtEOL property.
        /// </summary>
        private bool endateol;

        /// <summary>
        /// instance for BlockStartText property.
        /// </summary>
        private string starttext;

        /// <summary>
        /// instance for BlockEndText property.
        /// </summary>
        private string endtext;

        /// <summary>
        /// Gets or sets the Block StartLine property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int BlockStartLine
        {
            get
            {
                return startline;
            }

            set
            {
                startline = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether lexical element has multiline or not. By default it is set to false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool CloseAtEOL
        {
            get
            {
                return endateol;
            }

            set
            {
                endateol = value;
            }
        }

        /// <summary>
        /// Gets or sets the Block Start Text
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockStartText
        {
            get
            {
                return starttext;
            }

            set
            {
                starttext = value;
            }
        }

        /// <summary>
        /// Gets or sets the Block End Text
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockEndText
        {
            get
            {
                return endtext;
            }

            set
            {
                endtext = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sub Lexems
        /// </summary>
        /// <value>
        /// Type: System.Collections.IEnumerable
        /// </value>
        public IEnumerable SubLexems
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Lexem type
        /// </summary>
        public EditTokenType LexemType
        {
            get;
            set;
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCodes"/> class.
        /// </summary>
        public BlockCodes()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCodes"/> class.
        /// </summary>
        /// <param name="startLine">Gets the integer value of startLine from the reporting source</param>
        /// <param name="eol">Gets the bool value of eol from the reporting source</param>
        /// <param name="startText">Gets the string value of startText from the reporting source</param>
        /// <param name="endText">Gets the string value of endText from the reporting source</param>
        public BlockCodes(int startLine, bool eol, string startText, string endText)
        {
            startline = startLine;
            endateol = eol;
            starttext = startText;
            endtext = endText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.starttext;
        }

        #endregion Constructor
    }
}