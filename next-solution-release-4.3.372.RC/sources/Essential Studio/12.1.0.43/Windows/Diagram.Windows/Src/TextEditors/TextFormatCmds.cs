#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for text formatting commands.
    /// </summary>
    /// <remarks>
    /// This is the base class for a series of text formatting commands that
    /// modify the font style and alignment of text objects.
    /// </remarks>
    public abstract class TextFormatCmd
        : ICommand
    {
        #region Class members
        /// <summary>
        /// New format value.
        /// </summary>
        protected object m_fNewFormatValue;

        /// <summary>
        /// Old format value.
        /// </summary>
        /// <remarks>
        /// Valid only when new formatting is applied to one node
        /// or when formatting is applied using textEditor.
        /// </remarks>
        protected object m_fOldFormatValue;

        /// <summary>
        /// Helper Hashtable containing old format values.
        /// </summary>
        /// <remarks>
        /// Valid only when new formatting is applied to multiple nodes.
        /// </remarks>
        protected Hashtable m_hashOldFormatValues;

        /// <summary>
        /// Nodes Affected with new format.
        /// </summary>
        protected NodeCollection m_nodesAffected;

        /// <summary>
        /// TextEditor used to format text.
        /// </summary>
        /// <remarks>
        /// Valid only when one formatting is applied to one node using textEditor.
        /// </remarks>
        protected TextEditor m_txtEditor;

        /// <summary>
        /// Short, user-friendly description of the command.
        /// </summary>
        protected string m_strDescription;

        #endregion

        #region Class initilalize/finalize members
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFormatCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">The nodes affected.</param>
        public TextFormatCmd(NodeCollection nodesAffected)
        {
            m_nodesAffected = nodesAffected;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFormatCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">The node affected.</param>
        public TextFormatCmd(TextNode nodeAffected)
        {
            if (nodeAffected == null)
                throw new ArgumentNullException("nodeAffected", "nodeAffected can not be null!");

            m_nodesAffected = new NodeCollection();
            m_nodesAffected.Add(nodeAffected);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFormatCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        public TextFormatCmd(TextEditor txtEditor, TextNode nodeEditing)
        {
            m_txtEditor = txtEditor;
            m_nodesAffected = new NodeCollection();
            m_nodesAffected.Add(nodeEditing);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the new format value.
        /// </summary>
        /// <value>The new format value.</value>
        public object NewFormatValue
        {
            get { return m_fNewFormatValue; }
        }

        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public NodeCollection NodeAffected
        {
            get { return m_nodesAffected; }
        }

        /// <summary>
        /// Gets the hash of old values where key is textnode, value is format style.
        /// </summary>
        /// <value>The old values.</value>
        protected Hashtable OldValues
        {
            get
            {
                if (m_hashOldFormatValues == null)
                    m_hashOldFormatValues = new Hashtable();

                return m_hashOldFormatValues;
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// Applies the text formatting value to the text nodes attached to the command.
        /// </summary>
        /// <param name="cmdTarget">Command target.</param>
        /// <returns>True if formatting applied to at least one node; otherwise False.</returns>
        public virtual bool Do(object cmdTarget)
        {
            bool success = false;

            if (m_txtEditor != null)
            {
                TextNode nodeText = m_nodesAffected.First as TextNode;

                if (nodeText != null)
                {
                    m_txtEditor.BeginEdit(nodeText, false);
                    FormatTextEditor();
                }
            }
            else if ((m_nodesAffected.Count == 1) && (m_fOldFormatValue != null))
            {
                TextNode nodeText = m_nodesAffected.First as TextNode;

                if (nodeText != null)
                    m_fOldFormatValue = Format(nodeText);
            }
            else
            {
                foreach (Node nodeCur in m_nodesAffected)
                {
                    TextNode nodeText = nodeCur as TextNode;

                    if (nodeText != null)
                    {
                        object fmtOldValue = Format(nodeText);

                        // Save old format value.
                        if (!this.OldValues.ContainsKey(nodeText))
                            this.OldValues.Add(nodeText, fmtOldValue);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Restores the original text formatting to each node attached to the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        public virtual bool Undo()
        {
            if (m_txtEditor != null)
            {
                TextNode nodeText = m_nodesAffected.First as TextNode;

                if (nodeText != null)
                {
                    m_txtEditor.BeginEdit(nodeText, false);
                    UndoFormatTextEditor();
                }
            }           
            else if ((m_nodesAffected.Count == 1) && (m_fOldFormatValue != null))
            {
                // if there is only one affected node - old format value 
                // is stored in m_fOldFormatValue field
                TextNode nodeText = m_nodesAffected.First as TextNode;

                if (nodeText != null)
                    UndoFormat(nodeText, m_fOldFormatValue);
            }
            else
            {
                foreach (INode nodeCur in m_nodesAffected)
                {
                    TextNode nodeText = nodeCur as TextNode;

                    if (nodeText != null)
                    {
                        object fmtValue = null;

                        // Get Old format value.
                        if (m_hashOldFormatValues.ContainsKey(nodeText))
                            fmtValue = m_hashOldFormatValues[nodeText];

                        UndoFormat(nodeText, fmtValue);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports Undo.
        /// </summary>
        /// <remarks>
        /// <para>
        /// All text formatting commands support Undo.
        /// </para>
        /// </remarks>
        public virtual bool CanUndo
        {
            get { return true; }
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
            return false;
        }

        /// <summary>
        /// Merges the specified command to merge.
        /// </summary>
        /// <param name="cmdMerging">The command to merge.</param>
        public virtual void Merge(ICommand cmdMerging)
        { 
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Formats the specified node to format.
        /// </summary>
        /// <param name="nodeToFormat">The node to format.</param>
        /// <returns>The object</returns>
        protected abstract object Format(TextNode nodeToFormat);

        /// <summary>
        /// Undoes the format changes.
        /// </summary>
        /// <param name="nodeToFormat">The node to format.</param>
        /// <param name="fmtValue">The format style.</param>
        protected abstract void UndoFormat(TextNode nodeToFormat, object fmtValue);

        /// <summary>
        /// Formats the text editor.
        /// </summary>
        protected abstract void FormatTextEditor();

        /// <summary>
        /// Undoes the format text editor.
        /// </summary>
        protected abstract void UndoFormatTextEditor();
        #endregion
    }

    #region Formatting commands
    /// <summary>
    /// Sets the value of the Bold text property.
    /// </summary>
    public class BoldTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BoldTextCmd"/> class.
        /// Format text node with textEdit.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewBoldFormatValue">Formatted value.</param>
        public BoldTextCmd(TextEditor txtEditor, TextNode nodeEditing, bool bNewBoldFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewBoldFormatValue;
            m_fOldFormatValue = !bNewBoldFormatValue;
            m_strDescription = "Bold";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoldTextCmd"/> class.
        /// Format selection nodes text without textEditor.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewBoldFormatValue">Formatted value.</param>
        public BoldTextCmd(NodeCollection nodesAffected, bool bNewBoldFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewBoldFormatValue;
            m_strDescription = "Bold";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoldTextCmd"/> class.
        /// Format selection node text without textEditor.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewBoldFormatValue">Formatted value.</param>
        public BoldTextCmd(TextNode nodeAffected, bool bNewBoldFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewBoldFormatValue;
            m_strDescription = "Bold";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            // Save current format value.
            bool bOldValue = nodeToFormat.FontStyle.Bold;

            // Apply new format value.
            nodeToFormat.FontStyle.Bold = (bool)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.Bold = (bool)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Bold = (bool)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Bold = (bool)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the Italic text property.
    /// </summary>
    public class ItalicTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ItalicTextCmd"/> class.
        /// Format text node with textEdit.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewItalicFormatValue">Formatted value.</param>
        public ItalicTextCmd(TextEditor txtEditor, TextNode nodeEditing, bool bNewItalicFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewItalicFormatValue;
            m_fOldFormatValue = !bNewItalicFormatValue;
            m_strDescription = "Italic";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItalicTextCmd"/> class.
        /// Format selection nodes text without textEditor.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewItalicFormatValue">Formatted value.</param>
        public ItalicTextCmd(NodeCollection nodesAffected, bool bNewItalicFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewItalicFormatValue;
            m_strDescription = "Italic";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItalicTextCmd"/> class.
        /// Format selection node text without textEditor.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewItalicFormatValue">Formatted value.</param>
        public ItalicTextCmd(TextNode nodeAffected, bool bNewItalicFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewItalicFormatValue;
            m_strDescription = "Italic";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting must be TextBase derived!");

            // Save current format value.
            bool bOldValue = nodeToFormat.FontStyle.Italic;

            // Apply new format value.
            nodeToFormat.FontStyle.Italic = (bool)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.Italic = (bool)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Italic = (bool)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Italic = (bool)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the Underline text property.
    /// </summary>
    public class UnderlineTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlineTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">The text editor.</param>
        /// <param name="nodeEditing">The node editing.</param>
        /// <param name="bNewUnderlineFormatValue">if set to <c>true</c> new underline format value.</param>
        public UnderlineTextCmd(TextEditor txtEditor, TextNode nodeEditing, bool bNewUnderlineFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewUnderlineFormatValue;
            m_fOldFormatValue = !bNewUnderlineFormatValue;
            m_strDescription = "Underline";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlineTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewUnderlineFormatValue">Formatted value.</param>
        public UnderlineTextCmd(NodeCollection nodesAffected, bool bNewUnderlineFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewUnderlineFormatValue;
            m_strDescription = "Underline";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlineTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewUnderlineFormatValue">Formatted value.</param>
        public UnderlineTextCmd(TextNode nodeAffected, bool bNewUnderlineFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewUnderlineFormatValue;
            m_strDescription = "Underline";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            // Save current format value.
            bool bOldValue = nodeToFormat.FontStyle.Underline;

            // Apply new format value.
            nodeToFormat.FontStyle.Underline = (bool)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.Underline = (bool)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Underline = (bool)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Underline = (bool)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the Strikeout text property.
    /// </summary>
    public class StrikeoutTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="StrikeoutTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewStrikeoutFormatValue">Formatted value.</param>
        public StrikeoutTextCmd(TextEditor txtEditor, TextNode nodeEditing, bool bNewStrikeoutFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewStrikeoutFormatValue;
            m_fOldFormatValue = !bNewStrikeoutFormatValue;
            m_strDescription = "Strikeout";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrikeoutTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewStrikeoutFormatValue">Formatted value.</param>
        public StrikeoutTextCmd(NodeCollection nodesAffected, bool bNewStrikeoutFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewStrikeoutFormatValue;
            m_strDescription = "Strikeout";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrikeoutTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewStrikeoutFormatValue">Formatted value.</param>
        public StrikeoutTextCmd(TextNode nodeAffected, bool bNewStrikeoutFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewStrikeoutFormatValue;
            m_strDescription = "Strikeout";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting must be TextBase derived!");

            // Save current format value.
            bool bOldValue = nodeToFormat.FontStyle.Strikeout;

            // Apply new format value.
            nodeToFormat.FontStyle.Strikeout = (bool)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.Strikeout = (bool)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Strikeout = (bool)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.Strikeout = (bool)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the Strikeout text property.
    /// </summary>
    public class HorizontalAlignmentTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalAlignmentTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewAligmentFormatValue">Formatted value.</param>
        public HorizontalAlignmentTextCmd(TextEditor txtEditor, TextNode nodeEditing, StringAlignment bNewAligmentFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewAligmentFormatValue;
            m_fOldFormatValue = nodeEditing.HorizontalAlignment;
            m_strDescription = "Alignment";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalAlignmentTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewAligmentFormatValue">Formatted value.</param>
        public HorizontalAlignmentTextCmd(NodeCollection nodesAffected, StringAlignment bNewAligmentFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewAligmentFormatValue;
            m_strDescription = "Alignment";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalAlignmentTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewAligmentFormatValue">Formatted value.</param>
        public HorizontalAlignmentTextCmd(TextNode nodeAffected, StringAlignment bNewAligmentFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewAligmentFormatValue;
            m_strDescription = "Alignment";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting must be TextBase derived!");

            // Save current format value.
            StringAlignment bOldValue = nodeToFormat.HorizontalAlignment;

            // Apply new format value.
            nodeToFormat.HorizontalAlignment = (StringAlignment)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.HorizontalAlignment = (StringAlignment)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.HorizontalAlignment = (StringAlignment)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.HorizontalAlignment = (StringAlignment)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the FontFamily property.
    /// </summary>
    public class FontFamilyTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewFontFamilyValue">Formatted value.</param>
        public FontFamilyTextCmd(TextEditor txtEditor, TextNode nodeEditing, string bNewFontFamilyValue)
            : base(txtEditor, nodeEditing)
        {
            if (nodeEditing == null)
                throw new ArgumentNullException("nodeEditing", "nodeEditing can not be null!");

            m_fNewFormatValue = bNewFontFamilyValue;
            m_fOldFormatValue = nodeEditing.FontStyle.Family;
            m_strDescription = "Font";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewFontFamilyValue">Formatted value.</param>
        public FontFamilyTextCmd(NodeCollection nodesAffected, string bNewFontFamilyValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewFontFamilyValue;
            m_strDescription = "Font";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewFontFamilyValue">Formatted value.</param>
        public FontFamilyTextCmd(TextNode nodeAffected, string bNewFontFamilyValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewFontFamilyValue;
            m_strDescription = "Font";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting must be TextBase derived!");

            // Save current format value.
            string bOldValue = nodeToFormat.FontStyle.Family;

            // Apply new format value.
            nodeToFormat.FontStyle.Family = (string)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.Family = (string)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.FamilyName = (string)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.FamilyName = (string)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the FontFamily property.
    /// </summary>
    public class FontHeightTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FontHeightTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewFontHeightFormatValue">Formatted value.</param>
        public FontHeightTextCmd(TextEditor txtEditor, TextNode nodeEditing, float bNewFontHeightFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewFontHeightFormatValue;
            m_fOldFormatValue = nodeEditing.FontStyle.PointSize;
            m_strDescription = "Font Height";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontHeightTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewFontHeightFormatValue">Formatted value.</param>
        public FontHeightTextCmd(NodeCollection nodesAffected, float bNewFontHeightFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewFontHeightFormatValue;
            m_strDescription = "Font Height";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontHeightTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewFontHeightFormatValue">Formatted value.</param>
        public FontHeightTextCmd(TextNode nodeAffected, float bNewFontHeightFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewFontHeightFormatValue;
            m_strDescription = "Font Height";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting must be TextBase derived!");

            // Save current format value.
            float bOldValue = nodeToFormat.FontStyle.PointSize;

            // Apply new format value.
            nodeToFormat.FontStyle.PointSize = (float)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontStyle.PointSize = (float)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.PointSize = (float)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.PointSize = (float)m_fOldFormatValue;
        }
        #endregion
    }

    /// <summary>
    /// Sets the value of the Bold text property.
    /// </summary>
    public class ColorTextCmd : TextFormatCmd
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTextCmd"/> class.
        /// </summary>
        /// <param name="txtEditor">Current text editor.</param>
        /// <param name="nodeEditing">Editable node.</param>
        /// <param name="bNewColorFormatValue">Formatted value.</param>
        public ColorTextCmd(TextEditor txtEditor, TextNode nodeEditing, Color bNewColorFormatValue)
            : base(txtEditor, nodeEditing)
        {
            m_fNewFormatValue = bNewColorFormatValue;
            m_fOldFormatValue = nodeEditing.FontColorStyle.Color;
            m_strDescription = "Text Color";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTextCmd"/> class.
        /// </summary>
        /// <param name="nodesAffected">Collection of selection nodes.</param>
        /// <param name="bNewColorFormatValue">Formatted value.</param>
        public ColorTextCmd(NodeCollection nodesAffected, Color bNewColorFormatValue)
            : base(nodesAffected)
        {
            m_fNewFormatValue = bNewColorFormatValue;
            m_strDescription = "Text Color";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTextCmd"/> class.
        /// </summary>
        /// <param name="nodeAffected">Selection node.</param>
        /// <param name="bNewColorFormatValue">Formatted value.</param>
        public ColorTextCmd(TextNode nodeAffected, bool bNewColorFormatValue)
            : base(nodeAffected)
        {
            m_fNewFormatValue = bNewColorFormatValue;
            m_strDescription = "Text Color";
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Applies formatting value to the given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <returns>Previous value of formatting property.</returns>
        protected override object Format(TextNode nodeToFormat)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            // Save current format value.
            Color bOldValue = nodeToFormat.FontColorStyle.Color;

            // Apply new format value.
            nodeToFormat.FontColorStyle.Color = (Color)m_fNewFormatValue;

            return bOldValue;
        }

        /// <summary>
        /// Undo format text to given node without TextEditor.
        /// </summary>
        /// <param name="nodeToFormat">Node to format.</param>
        /// <param name="fmtValue">Formatting value.</param>
        protected override void UndoFormat(TextNode nodeToFormat, object fmtValue)
        {
            if (nodeToFormat == null)
                throw new ArgumentNullException("nodeToFormat", "Node Formatting can't be null!");

            if (fmtValue == null)
                throw new ArgumentNullException("fmtValue", "Format value can not be null!");

            // Apply new format value.
            nodeToFormat.FontColorStyle.Color = (Color)fmtValue;
        }

        /// <summary>
        /// Applies formatting value to text in TextEditor.
        /// </summary>
        protected override void FormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.TextColor = (Color)m_fNewFormatValue;
        }

        /// <summary>
        /// Undo format text in TextEditor.
        /// </summary>
        protected override void UndoFormatTextEditor()
        {
            if (m_txtEditor != null)
                m_txtEditor.TextColor = (Color)m_fOldFormatValue;
        }

        #endregion
    }
    #endregion
}

