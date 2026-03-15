#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for all of the RTF releated commands.
    /// </summary>
    public abstract class RTFFormatCmd
        : ICommand
    {
        #region fields
        protected RichTextNode m_nodeEditing;
        protected int m_nSelectionStart;
        protected int m_nSelectionLength;
        protected string m_strDescription = string.Empty;
        protected TextEditor m_txtEditor;
        protected object m_fNewFormatValue;
        protected object m_fOldFormatValue;
        #endregion

        #region properties
        /// <summary>
        /// Gets the editing node.
        /// </summary>
        /// <value>The node editing.</value>
        public RichTextNode NodeEditing
        {
            get { return m_nodeEditing; }
        }

        /// <summary>
        /// Gets the new format value.
        /// </summary>
        /// <value>The new format value.</value>
        public object NewFormatValue
        {
            get { return m_fNewFormatValue; }
        }

        /// <summary>
        /// Gets the old format value.
        /// </summary>
        /// <value>The old format value.</value>
        public object OldFormatValue
        {
            get { return m_fOldFormatValue; }
        }

        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFFormatCmd"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="nodeFormatting">The node formatting.</param>
        /// <param name="nSelectionStart">The n selection start.</param>
        /// <param name="nSelectionLength">Length of the n selection.</param>
        public RTFFormatCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength)
        {
            if (textEditor == null)
                throw new ArgumentNullException("TextEditor");

            m_txtEditor = textEditor;
            m_nodeEditing = nodeFormatting;
            m_nSelectionStart = nSelectionStart;
            m_nSelectionLength = nSelectionLength;
        }
        #endregion

        #region abstract
        /// <summary>
        /// Formats the text.
        /// </summary>
        protected abstract void FormatText();

        /// <summary>
        /// Undoes the format text.
        /// </summary>
        protected abstract void UndoFormatText();

        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanMerge(ICommand cmd);

        /// <summary>
        /// Merges the specified command with last recorded user operation.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        public abstract void Merge(ICommand cmd);
        #endregion

        #region ICommand Members
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value></value>
        public bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Undo()
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            UndoFormatText();

            return true;
        }
        #endregion

        #region IVerb Members
        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public bool Do(object target)
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            FormatText();

            return true;
        }
        #endregion
    }

    #region Formatting commands
    /// <summary>
    /// Command used to format text bold
    /// </summary>
    public class RTFBoldCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFBoldCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bBold">bold value</param>
        public RTFBoldCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bBold)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("BoldCmd");
            m_fNewFormatValue = bBold;
            m_fOldFormatValue = !bBold;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatBold((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatBold((bool)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with bold.
        /// </summary>
        /// <param name="bBold">formatting value</param>
        private void FormatBold(bool bBold)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Bold = bBold;
            }
        }
        #endregion
    }

    /// <summary>
    /// Command used to format text italic
    /// </summary>
    public class RTFItalicCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFItalicCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bItalic">italic value</param>
        public RTFItalicCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bItalic)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("ItalicCmd");
            m_fNewFormatValue = bItalic;
            m_fOldFormatValue = !bItalic;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatItalic((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatItalic((bool)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with italic.
        /// </summary>
        /// <param name="bItalic">formatting value</param>
        private void FormatItalic(bool bItalic)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Italic = bItalic;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to underline text.
    /// </summary>
    public class RTFUnderlineCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFUnderlineCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bUnderline">underline value</param>
        public RTFUnderlineCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bUnderline)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("UnderlineCmd");
            m_fNewFormatValue = bUnderline;
            m_fOldFormatValue = !bUnderline;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatUnderline((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatUnderline((bool)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with underline.
        /// </summary>
        /// <param name="bUnderline">formatting value</param>
        private void FormatUnderline(bool bUnderline)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Underline = bUnderline;
            }
        }
        #endregion
    }

    /// <summary>
    /// RFT Subscript commands.
    /// </summary>
    public class RTFSubscriptCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFSubscriptCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bSuperscript">Previous superscript value</param>
        /// <param name="bSubscript">Subscript value</param>
        public RTFSubscriptCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bSuperscript, bool bSubscript)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("SubscriptCmd");
            m_fNewFormatValue = bSubscript;
            m_fOldFormatValue = bSuperscript;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatSubscript((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            if ((bool)m_fOldFormatValue)
                FormatSuperscript(true);
            else
                FormatSubscript(!(bool)m_fNewFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with superscript.
        /// </summary>
        /// <param name="bSuperscript">formatting value</param>
        private void FormatSuperscript(bool bSuperscript)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Superscript = bSuperscript;
            }
        }

        /// <summary>
        /// Format selected text with subscript.
        /// </summary>
        /// <param name="bSubscript">formatting value</param>
        private void FormatSubscript(bool bSubscript)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Subscript = bSubscript;
            }
        }
        #endregion
    }

    /// <summary>
    /// RTF Superscript commands.
    /// </summary>
    public class RTFSuperscriptCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFSuperscriptCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bSubscript">previous subscript value</param>
        /// <param name="bSuperscript">Superscript value</param>
        public RTFSuperscriptCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bSubscript, bool bSuperscript)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("SuperscriptCmd");
            m_fNewFormatValue = bSuperscript;
            m_fOldFormatValue = bSubscript;
        }
        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatSuperscript((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            if ((bool)m_fOldFormatValue)
            {
                FormatSubscript(true);
            }
            else
            {
                FormatSuperscript(!(bool)m_fNewFormatValue);
            }
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with superscript.
        /// </summary>
        /// <param name="bSuperscript">formatting value</param>
        private void FormatSuperscript(bool bSuperscript)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Superscript = bSuperscript;
            }
        }

        /// <summary>
        /// Format selected text with subscript.
        /// </summary>
        /// <param name="bSubscript">formatting value</param>
        private void FormatSubscript(bool bSubscript)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Subscript = bSubscript;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to set color for text.
    /// </summary>
    public class RTFColorCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFColorCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="previous">previous color value</param>
        /// <param name="color">color value</param>
        public RTFColorCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, Color previous, Color color)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("FontColorCmd");

            m_fOldFormatValue = (previous != Color.Empty) ? previous : Color.Black;
            m_fNewFormatValue = color;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatTextColor((Color)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatTextColor((Color)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with color.
        /// </summary>
        /// <param name="color">formatting value</param>
        private void FormatTextColor(Color color)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.TextColor = color;
            }
        }
        #endregion
    }
  /// <summary>
  /// Command used to strikeout the text.
  /// </summary>
    public class RTFStrikeoutCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFStrikeoutCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="bStrikeout">Strikeout value</param>
        public RTFStrikeoutCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, bool bStrikeout)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("StrikeoutCmd");
            m_fNewFormatValue = bStrikeout;
            m_fOldFormatValue = !bStrikeout;
        }

        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatStrikeout((bool)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatStrikeout((bool)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with strikeout.
        /// </summary>
        /// <param name="bStrikeout">formatting value</param>
        private void FormatStrikeout(bool bStrikeout)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.Strikeout = bStrikeout;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to set font family for text.
    /// </summary>
    public class RTFFamilyNameCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFFamilyNameCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="previusName">previous family name value</param>
        /// <param name="familyName">family name value</param>
        public RTFFamilyNameCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, string previusName, string familyName)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("FontCmd");
            m_fOldFormatValue = previusName;
            m_fNewFormatValue = familyName;
        }
        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatFamily((string)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatFamily((string)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with familyName.
        /// </summary>
        /// <param name="familyName">formatting value</param>
        private void FormatFamily(string familyName)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.FamilyName = familyName;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to set the font size for the text.
    /// </summary>
    public class RTFPointSizeCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFPointSizeCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="previous">previous size value</param>
        /// <param name="size">font size value</param>
        public RTFPointSizeCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, float previous, float size)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("FontHeightCmd");
            m_fNewFormatValue = size;
            m_fOldFormatValue = previous;
        }
        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatPointSize((float)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatPointSize((float)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with pointSize.
        /// </summary>
        /// <param name="pointSize">formatting value</param>
        private void FormatPointSize(float pointSize)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.PointSize = pointSize;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to set horizontal alignment of text.
    /// </summary>
    public class RTFHorizontalAlignmentCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFHorizontalAlignmentCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="previous">previous alignment value</param>
        /// <param name="alignment">alignment enum value</param>
        public RTFHorizontalAlignmentCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, StringAlignment previous, StringAlignment alignment)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("CharacterSettingCmd");
            m_fNewFormatValue = alignment;
            m_fOldFormatValue = previous;
        }
        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        public override void Merge(ICommand cmd)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            StringAlignment alignmentNew = (StringAlignment)Enum.Parse(typeof(StringAlignment), m_fNewFormatValue.ToString());
            FormatAligment(alignmentNew);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            StringAlignment alignmentOld = (StringAlignment)Enum.Parse(typeof(StringAlignment), m_fOldFormatValue.ToString());
            FormatAligment(alignmentOld);
        }

        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with alignment.
        /// </summary>
        /// <param name="alignment">formatting value</param>
        private void FormatAligment(StringAlignment alignment)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.HorizontalAlignment = alignment;
            }
        }
        #endregion
    }
    /// <summary>
    /// Command used to set character offset for text.
    /// </summary>
    public class RTFCharOffsetCmd
        : RTFFormatCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RTFCharOffsetCmd"/> class.
        /// </summary>
        /// <param name="textEditor">current text editor</param>
        /// <param name="nodeFormatting">RichText node</param>
        /// <param name="nSelectionStart">selection start position</param>
        /// <param name="nSelectionLength">selection length</param>
        /// <param name="previous">previous value offset</param>
        /// <param name="newValue">new value offset</param>
        public RTFCharOffsetCmd(TextEditor textEditor, RichTextNode nodeFormatting, int nSelectionStart, int nSelectionLength, int previous, int newValue)
            : base(textEditor, nodeFormatting, nSelectionStart, nSelectionLength)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("CharacterSettingCmd");
            m_fNewFormatValue = newValue;
            m_fOldFormatValue = previous;
        }
        #endregion

        #region override
        /// <summary>
        /// Checks whether text can be merged.
        /// </summary>
        /// <param name="cmd">Merging command</param>
        /// <returns>true - can be merged; false - not</returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merge text with command.
        /// </summary>
        /// <param name="cmdMerging">Merging command</param>
        public override void Merge(ICommand cmdMerging)
        { 
        }

        /// <summary>
        /// Execute format text.
        /// </summary>
        protected override void FormatText()
        {
            FormatOffset((int)m_fNewFormatValue);
        }

        /// <summary>
        /// Undo format text.
        /// </summary>
        protected override void UndoFormatText()
        {
            FormatOffset((int)m_fOldFormatValue);
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Format selected text with char offset.
        /// </summary>
        /// <param name="charOffset">formatting value</param>
        private void FormatOffset(int charOffset)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.SetSelection(m_nSelectionStart, m_nSelectionLength);
                m_txtEditor.CharOffset = charOffset;
            }
        }
        #endregion
    }
    #endregion

    #region set command
    /// <summary>
    /// Command used to delete text.
    /// </summary>
    public class DeleteTextCmd
        : CutTextCmd
    {
        #region initialise/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTextCmd"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        /// <param name="nInsertionLocation">The n insertion location.</param>
        /// <param name="strDeletedText">The deleted text.</param>
        /// <param name="strTextToInsert">The text to insert.</param>
        public DeleteTextCmd(TextEditor textEditor, Node nodeEditing, int nInsertionLocation, string strDeletedText, string strTextToInsert)
            : base(textEditor, nodeEditing, nInsertionLocation, strDeletedText, strTextToInsert)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("DeleteCmd");
        }
        #endregion

        #region override

        /// <summary>
        /// Determines whether this instance can merge the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanMerge(ICommand cmd)
        {
            bool bCanMerge = false;
            DeleteTextCmd cmdDeleteText = cmd as DeleteTextCmd;

            if (cmdDeleteText != null && cmdDeleteText.NodeEditing.Equals(m_nodeEditing)
                && m_nInsertionStart - 1 == cmdDeleteText.InsertionLocation)
                bCanMerge = true;

            return bCanMerge;
        }

        /// <summary>
        /// Merges the specified CMD merging.
        /// </summary>
        /// <param name="cmdMerging">The CMD merging.</param>
        public override void Merge(ICommand cmdMerging)
        {
            if (!(cmdMerging is DeleteTextCmd))
                throw new ArgumentException("cmdMerging", "Merging command must be of DeleteTextCmd type.");

            if (CanMerge(cmdMerging))
            {
                DeleteTextCmd cmdDeleteText = cmdMerging as DeleteTextCmd;

                if (cmdDeleteText != null)
                {
                    m_nInsertionStart -= 1;

                    string[] arrString = new string[m_strDeleted.Length + 1];
                    Array.Copy(m_strDeleted, 0, arrString, 1, m_strDeleted.Length);

                    arrString[0] = cmdDeleteText.RTFTextDeleted;
                    m_strDeleted = arrString;
                }
            }
        }

        /// <summary>
        /// Does the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>true, if do the specified target.</returns>
        public override bool Do(object target)
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strDeleted).Length);
            m_txtEditor.CurrentText = string.Empty;
            m_txtEditor.SetSelection(m_nInsertionStart, 0);

            return true;
        }

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public override bool Undo()
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            m_txtEditor.SetSelection(m_nInsertionStart, 0);
            m_txtEditor.CurrentText = ComposeRtfString(m_strDeleted);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strDeleted).Length);

            return true;
        }
        #endregion
    }

    /// <summary>
    /// Cut text commands.
    /// </summary>
    public class CutTextCmd
        : SetTextCmd
    {
        #region initialise/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="CutTextCmd"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        /// <param name="nInsertionLocation">The n insertion location.</param>
        /// <param name="strDeletedText">The deleted text.</param>
        /// <param name="strTextToInsert">The text to insert.</param>
        public CutTextCmd(TextEditor textEditor, Node nodeEditing, int nInsertionLocation, string strDeletedText, string strTextToInsert)
            : base(textEditor, nodeEditing, nInsertionLocation, strDeletedText, strTextToInsert)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("CutCmd");
        }
        #endregion

        #region override
        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merges the specified merging command.
        /// </summary>
        /// <param name="cmdMerging">The merging command.</param>
        public override void Merge(ICommand cmdMerging)
        { 
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public override bool Do(object target)
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strDeleted).Length);
            m_txtEditor.CurrentText = string.Empty;
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strToInsert).Length);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Paste text commands.
    /// </summary>
    public class PasteTextCmd
        : SetTextCmd
    {
        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteTextCmd"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        /// <param name="nInsertionLocation">The n insertion location.</param>
        /// <param name="strDeletedText">The deleted text.</param>
        /// <param name="strTextToInsert">The text to insert.</param>
        public PasteTextCmd(TextEditor textEditor, Node nodeEditing, int nInsertionLocation, string strDeletedText, string strTextToInsert)
            : base(textEditor, nodeEditing, nInsertionLocation, strDeletedText, strTextToInsert)
        {
            m_strDescription = Resources.Strings.CommandDescriptions.Get("PasteCmd");
        }
        #endregion

        #region override
        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanMerge(ICommand cmd)
        {
            return false;
        }

        /// <summary>
        /// Merges the specified merging command.
        /// </summary>
        /// <param name="cmdMerging">The merging command.</param>
        public override void Merge(ICommand cmdMerging)
        { 
        }

        #endregion
    }

    /// <summary>
    /// Set text commands
    /// </summary>
    public class SetTextCmd
        : ICommand
    {
        #region Constants
        private const string c_str_DEF_RTF_HEADER = @"^({\\rtf1)";
        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTextCmd"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        /// <param name="nInsertionLocation">The n insertion location.</param>
        /// <param name="strDeletedText">The deleted text.</param>
        /// <param name="strTextToInsert">The text to insert.</param>
        public SetTextCmd(TextEditor textEditor, Node nodeEditing, int nInsertionLocation, string strDeletedText, string strTextToInsert)
        {
            m_txtEditor = textEditor;
            m_nodeEditing = nodeEditing;
            m_nInsertionStart = nInsertionLocation;
            m_strDeleted = new string[] { strDeletedText };
            m_strToInsert = new string[] { strTextToInsert };
            m_strDescription = Resources.Strings.CommandDescriptions.Get("SetTextCmd");
        }

        #endregion

        #region fields
        protected string m_strDescription;
        protected Node m_nodeEditing;
        protected int m_nInsertionStart;
        protected string[] m_strToInsert;
        protected string[] m_strDeleted;
        protected TextEditor m_txtEditor;
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node editing.
        /// </summary>
        /// <value>The node editing.</value>
        public INode NodeEditing
        {
            get { return m_nodeEditing; }
        }

        /// <summary>
        /// Gets the insertion location.
        /// </summary>
        /// <value>The insertion location.</value>
        public int InsertionLocation
        {
            get { return m_nInsertionStart; }
        }

        /// <summary>
        /// Gets the text inserting.
        /// </summary>
        /// <value>The text inserting.</value>
        public string TextInserting
        {
            get { return ComposeString(m_strToInsert); }
        }

        /// <summary>
        /// Gets the text deleted.
        /// </summary>
        /// <value>The text deleted.</value>
        public string TextDeleted
        {
            get { return ComposeString(m_strDeleted); }
        }

        /// <summary>
        /// Gets the RTF text inserting.
        /// </summary>
        /// <value>The RTF text inserting.</value>
        public string RTFTextInserting
        {
            get { return ComposeRtfString(m_strToInsert); }
        }

        /// <summary>
        /// Gets the RTF text deleted.
        /// </summary>
        /// <value>The RTF text deleted.</value>
        public string RTFTextDeleted
        {
            get { return ComposeRtfString(m_strDeleted); }
        }
        #endregion

        #region ICommand Members
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        /// <value></value>
        public virtual bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public virtual bool Undo()
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strToInsert).Length);
            m_txtEditor.CurrentText = ComposeRtfString(m_strDeleted);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strDeleted).Length);

            return true;
        }

        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanMerge(ICommand cmd)
        {
            bool bCanMerge = false;
            SetTextCmd cmdSetText = cmd as SetTextCmd;

            if (cmdSetText != null && cmdSetText.GetType() == typeof(SetTextCmd) && cmdSetText.NodeEditing.Equals(m_nodeEditing) &&
                (m_nInsertionStart + m_strToInsert.Length == cmdSetText.InsertionLocation))
                bCanMerge = true;

            return bCanMerge;
        }

        /// <summary>
        /// Merges the specified merging command.
        /// </summary>
        /// <param name="cmdMerging">The merging command.</param>
        public virtual void Merge(ICommand cmdMerging)
        {
            if (!(cmdMerging is SetTextCmd))
                throw new ArgumentException("cmdMerging", "Merging command must be of SetTextCmd type.");

            if (CanMerge(cmdMerging))
            {
                SetTextCmd cmdSetText = cmdMerging as SetTextCmd;
                string[] arrString = new string[m_strToInsert.Length + 1];
                Array.Copy(m_strToInsert, arrString, m_strToInsert.Length);

                arrString[arrString.Length - 1] = cmdSetText.RTFTextInserting;
                m_strToInsert = arrString;
            }
        }
        #endregion

        #region IVerb Members
        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon.</param>
        /// <returns>True if successful; otherwise False.</returns>
        public virtual bool Do(object target)
        {
            m_txtEditor.BeginEdit(m_nodeEditing, false);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strDeleted).Length);
            m_txtEditor.CurrentText = ComposeRtfString(m_strToInsert);
            m_txtEditor.SetSelection(m_nInsertionStart, ComposeString(m_strToInsert).Length);

            return true;
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Composes the RTF string.
        /// </summary>
        /// <param name="arrString">The string array.</param>
        /// <returns>The RTF string.</returns>
        protected string ComposeRtfString(string[] arrString)
        {
            string strToReturn;

            // is we are editing RixhTextNode compose rtf string,
            // otherwise return simple text.
            if (m_nodeEditing is RichTextNode)
            {
                // create RichTextBox
                RichTextBox rtfBox = new RichTextBox();

                foreach (string strCur in arrString)
                {
                    if (System.Text.RegularExpressions.Regex.Match(strCur, c_str_DEF_RTF_HEADER).Success)
                        rtfBox.SelectedRtf = strCur;
                    else
                        rtfBox.SelectedText = strCur;
                }

                // Set selection
                rtfBox.SelectionStart = 0;
                rtfBox.SelectionLength = rtfBox.TextLength;

                strToReturn = rtfBox.SelectedRtf;

                // Dispose RichTextBox
                rtfBox.Dispose();
            }
            else
                strToReturn = ComposeString(arrString);

            return strToReturn;
        }

        /// <summary>
        /// Composes the string.
        /// </summary>
        /// <param name="arrString">The string array.</param>
        /// <returns>The string</returns>
        protected string ComposeString(string[] arrString)
        {
            string strToReturn;

            if (m_nodeEditing is RichTextNode)
            {
                // create RichTextBox
                RichTextBox rtfBox = new RichTextBox();

                foreach (string strCur in arrString)
                {
                    if (System.Text.RegularExpressions.Regex.Match(strCur, c_str_DEF_RTF_HEADER).Success)
                        rtfBox.SelectedRtf = strCur;
                    else
                        rtfBox.SelectedText = strCur;
                }

                // Get text
                strToReturn = rtfBox.Text;

                // Dispose RichTextBox
                rtfBox.Dispose();
            }
            else
            {
                // if we are editing TextNode use StringBuilder
                StringBuilder strBuilder = new StringBuilder();

                foreach (string strCur in arrString)
                    strBuilder.Append(strCur);

                strToReturn = strBuilder.ToString();
            }

            return strToReturn;
        }
        #endregion
    }
    #endregion
}
