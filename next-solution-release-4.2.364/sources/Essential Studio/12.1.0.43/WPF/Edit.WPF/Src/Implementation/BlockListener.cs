// <copyright file="BlockListener.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// BlockListener class is used to hold values for expand or collapse the items.
    /// </summary>

#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class BlockListener
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Local Variable and Properties

        /// <summary>
        /// instance for BlockStart property.
        /// </summary>
        private string mstarttext;

        /// <summary>
        /// instance for BlockEnd property.
        /// </summary>
        private string mendtext;

        /// <summary>
        /// instance for ParentLineNumber property.
        /// </summary>
        private int mplinenumber = -1;

        /// <summary>
        /// instance for IsPreprocessor property.
        /// </summary>
        private bool misprep;

        /// <summary>
        /// Gets or sets the Block StartLine property.
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockStart
        {
            get
            {
                return mstarttext;
            }

            set
            {
                mstarttext = value;
            }
        }

        /// <summary>
        /// Gets or sets the Block StartLine
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string BlockEnd
        {
            get
            {
                return mendtext;
            }

            set
            {
                mendtext = value;
            }
        }

        /// <summary>
        /// Gets or sets the Block StartLine
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int ParentLineNumber
        {
            get
            {
                return mplinenumber;
            }

            set
            {
                mplinenumber = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Lexem element is Preprocessor or not. By default it is set to false.
        /// </summary>
        /// <value>
        /// Type: System.Boolean
        /// </value>
        public bool IsPreprocessor
        {
            get
            {
                return misprep;
            }

            set
            {
                misprep = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsRegex { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool CheckParentType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public EditTokenType LexemType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public EditTokenType ParentLexemType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int EndLineNumber { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ScopeLevel ScopeLevel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsCollapsible { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsIndent { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IgnoreEndBlock { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool EndBlockOnRecurrence { get; set; }

        #endregion Local Variable and Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockListener"/> class.
        /// </summary>
        public BlockListener()
        {
            CheckParentType = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockListener"/> class.
        /// </summary>
        /// <param name="parent">Gets the integer value of parent from the reporting source</param>
        /// <param name="startText">Gets the string value of startText from the reporting source</param>
        /// <param name="endText">Gets the string value of endText from the reporting source</param>
        /// <param name="isPreprocessor">Gets the bool value of isPreprocessor from the reporting source</param>
        public BlockListener(int parent, string startText, string endText, bool isPreprocessor)
        {
            mplinenumber = parent;
            mstarttext = startText;
            mendtext = endText;
            misprep = isPreprocessor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("BlockStart={0}, BlockEnd={1}, IsRegex={2},IsPreProcessor={3},ParentLineNumber={4}", new object[] { this.BlockStart, this.BlockEnd, this.IsRegex, this.IsPreprocessor, this.ParentLineNumber });
        }

        #endregion Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BlockListener)
            {
                BlockListener block = obj as BlockListener;
                return (block.BlockStart == this.BlockStart && block.BlockEnd == this.BlockEnd && block.CheckParentType == this.CheckParentType && block.IsCollapsible == this.IsCollapsible && block.IsIndent == this.IsIndent && block.IsPreprocessor == this.IsPreprocessor && block.IsRegex == this.IsRegex && block.LexemType == this.LexemType && block.ParentLineNumber == this.ParentLineNumber);
            }
            return false;
        }
    }
}