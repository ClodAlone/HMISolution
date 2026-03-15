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
using System.Windows.Forms;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Text Editor used in Text Node for text editing purposes.
    /// </summary>
    public class TextEditor
        : ITextEdit
    {
        #region constants
        private const string c_strDEFAULT_FONT_FAMILY = "Microsoft Sans Serif";
        private const int c_nDEFAULT_FONT_HEIGHT = 8;
        private const string c_str_DEF_RTF_HEADER = @"^({\\rtf1)";
        #endregion

        #region fields
        /// <summary>
        /// Diagram Controller.
        /// </summary>
        private DiagramController m_controller;

        /// <summary>
        /// TextEditor used to edit text.
        /// </summary>
        private ITextEditor m_txtEditor;

        /// <summary>
        /// Node being edited.
        /// </summary>
        private Node m_nodeEditing;
        private bool m_bUpper;
        private bool m_bLower;

        #endregion

        #region events
        /// <summary>
        /// Occurs when format changed.
        /// </summary>
        public event EventHandler FormatChanged;
        /// <summary>
        /// Occurs when text changed.
        /// </summary>
        public event EventHandler TextChanged;

        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditor"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public TextEditor(DiagramController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller", "Diagram controller can't be null");

            m_controller = controller;
        }
        #endregion

        #region ITextEdit
        void ITextEdit.Cut()
        {
            // create cut cmd
            CutTextCmd cmdCutText = new CutTextCmd(this, m_nodeEditing, ((TextBoxBase)m_txtEditor).SelectionStart, this.CurrentText, string.Empty);

            // unsubscribe from current text editor events
            EventsUnsubscribe(m_txtEditor);

            // insert selected text into Clipboard
            Clipboard.SetDataObject(this.CurrentText, false);
            this.CurrentText = string.Empty;

            // subscribe from current text editor events
            SubscribeForEvents(m_txtEditor);
        }
        void ITextEdit.Paste(string strText)
        {
            // create cut cmd
            PasteTextCmd cmdPasteText = new PasteTextCmd(this, m_nodeEditing, ((TextBoxBase)m_txtEditor).SelectionStart, this.CurrentText, strText);

            // unsubscribe from current text editor events
            EventsUnsubscribe(m_txtEditor);

            this.CurrentText = strText;

            // subscribe from current text editor events
            SubscribeForEvents(m_txtEditor);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the current text.
        /// </summary>
        /// <value>The current text.</value>
        public string CurrentText
        {
            get
            {
                string bValueToReturn = string.Empty;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.CurrentText;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                    m_txtEditor.CurrentText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                string bValueToReturn = string.Empty;

                if (m_txtEditor != null)
                {
                    if (m_txtEditor is TextEdit)
                    {
                        bValueToReturn = ((TextEdit)m_txtEditor).Text;
                    }
                    else if (m_txtEditor is RichTextEdit)
                    {
                        bValueToReturn = ((RichTextEdit)m_txtEditor).Text;
                    }
                }

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_txtEditor is TextEdit)
                    {
                        ((TextEdit)m_txtEditor).Text = value;
                    }
                    else if (m_txtEditor is RichTextEdit)
                    {
                        ((RichTextEdit)m_txtEditor).Text = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get { return m_txtEditor != null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is upper.
        /// </summary>
        public bool Upper
        {
            get
            {
                return m_bUpper; 
            }
            set 
            {
                if (m_bUpper != value)
                {
                    m_bUpper = value;
                    this.CurrentText = ParseRtfText(this.CurrentText).ToUpper();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is lower.
        /// </summary>        
        public bool Lower
        {
            get 
            { 
                return m_bLower; 
            }
            set
            {
                if (m_bLower != value)
                {
                    m_bLower = value;
                    this.CurrentText = ParseRtfText(this.CurrentText).ToLower();
                }
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether this <see cref="TextEditor"/> is bold.
        /// </summary>
        public bool Bold
        {
            get
            {
                bool bValueToReturn;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Bold;
                else
                    bValueToReturn = GetSelectionFormatValue(TextFormatting.Bold);

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null && m_txtEditor.Bold != value)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Bold, value);
                    else
                        CreateTextCmd(TextFormatting.Bold, value);

                    m_txtEditor.Bold = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        bool bFormatValueToSet = false;

                        // determine value to set
                        foreach (TextNode node in nodesText)
                        {
                            if (!node.FontStyle.Bold)
                            {
                                bFormatValueToSet = true;
                            }
                        }

                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Bold");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.Bold = bFormatValueToSet;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is underline.
        /// </summary>
        /// <value><c>true</c> if underline; otherwise, <c>false</c>.</value>
        public bool Underline
        {
            get
            {
                bool bValueToReturn;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Underline;
                else
                    bValueToReturn = GetSelectionFormatValue(TextFormatting.Underline);

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null && m_txtEditor.Underline != value)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Underline, value);
                    else
                        CreateTextCmd(TextFormatting.Underline, value);

                    m_txtEditor.Underline = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        bool bFormatValueToSet = false;

                        // determine value to set
                        foreach (TextNode node in nodesText)
                        {
                            if (!node.FontStyle.Underline)
                            {
                                bFormatValueToSet = true;
                            }
                        }

                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Underline");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.Underline = bFormatValueToSet;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is italic.
        /// </summary>
        /// <value><c>true</c> if italic; otherwise, <c>false</c>.</value>
        public bool Italic
        {
            get
            {
                bool bValueToReturn;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Italic;
                else
                    bValueToReturn = GetSelectionFormatValue(TextFormatting.Italic);

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null && m_txtEditor.Italic != value)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Italic, value);
                    else
                        CreateTextCmd(TextFormatting.Italic, value);

                    m_txtEditor.Italic = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        bool bFormatValueToSet = false;

                        // determine value to set
                        foreach (TextNode node in nodesText)
                        {
                            if (!node.FontStyle.Italic)
                            {
                                bFormatValueToSet = true;
                            }
                        }

                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Italic");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.Italic = bFormatValueToSet;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is strikeout.
        /// </summary>
        /// <value><c>true</c> if strikeout; otherwise, <c>false</c>.</value>
        public bool Strikeout
        {
            get
            {
                bool bValueToReturn;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Strikeout;
                else
                    bValueToReturn = GetSelectionFormatValue(TextFormatting.Strikeout);

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null && m_txtEditor.Strikeout != value)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Strikeout, value);
                    else
                        CreateTextCmd(TextFormatting.Strikeout, value);

                    m_txtEditor.Strikeout = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        bool bFormatValueToSet = false;

                        // determine value to set
                        foreach (TextNode node in nodesText)
                        {
                            if (!node.FontStyle.Strikeout)
                            {
                                bFormatValueToSet = true;
                            }
                        }

                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Strikeout");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.Strikeout = bFormatValueToSet;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is subscript.
        /// </summary>
        /// <value><c>true</c> if subscript; otherwise, <c>false</c>.</value>
        public bool Subscript
        {
            get
            {
                bool bValueToReturn = false;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Subscript;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Subscript, value);

                    m_txtEditor.Subscript = value;
                    OnFormatChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TextEditor"/> is superscript.
        /// </summary>
        /// <value><c>true</c> if superscript; otherwise, <c>false</c>.</value>
        public bool Superscript
        {
            get
            {
                bool bValueToReturn = false;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.Superscript;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.Superscript, value);

                    m_txtEditor.Superscript = value;
                    OnFormatChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get
            {
                Color bValueToReturn = Color.Empty;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.TextColor;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.TextColor, value);
                    else
                        CreateTextCmd(TextFormatting.TextColor, value);

                    m_txtEditor.TextColor = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Font Color");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontColorStyle.Color = value;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the family.
        /// </summary>
        /// <value>The name of the family.</value>
        public string FamilyName
        {
            get
            {
                string bValueToReturn = c_strDEFAULT_FONT_FAMILY;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.FontFamily;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.FamilyName, value);
                    else
                        CreateTextCmd(TextFormatting.FamilyName, value);

                    m_txtEditor.FontFamily = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Family Name");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.Family = value;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the char offset.
        /// </summary>
        /// <value>The char offset.</value>
        public int CharOffset
        {
            get
            {
                int bValueToReturn = 0;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.CharOffset;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.CharOffset, value);

                    m_txtEditor.CharOffset = value;
                    OnFormatChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the point.
        /// </summary>
        /// <value>The size of the point.</value>
        public float PointSize
        {
            get
            {
                float bValueToReturn = c_nDEFAULT_FONT_HEIGHT;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.PointSize;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.PointSize, value);
                    else
                        CreateTextCmd(TextFormatting.PointSize, value);

                    m_txtEditor.PointSize = value * (this.Controller.View.Magnification / 100f);
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Font Size");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.FontStyle.PointSize = value;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public StringAlignment HorizontalAlignment
        {
            get
            {
                StringAlignment bValueToReturn = StringAlignment.Near;

                if (m_txtEditor != null)
                    bValueToReturn = m_txtEditor.HorizontalAlignment;

                return bValueToReturn;
            }
            set
            {
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing is RichTextNode)
                        CreateRTFCmd(TextFormatting.HorizontalAlignment, value);
                    else
                        CreateTextCmd(TextFormatting.HorizontalAlignment, value);

                    m_txtEditor.HorizontalAlignment = value;
                    OnFormatChanged();
                }
                else
                {
                    NodeCollection nodesText = GetTextBaseNodes(m_controller.SelectionList);

                    if (nodesText != null)
                    {
                        // start transaction
                        this.Controller.Model.HistoryManager.StartAtomicAction("Text Alignment");

                        // set new format value
                        foreach (TextNode node in nodesText)
                        {
                            node.HorizontalAlignment = value;
                        }

                        // end transaction
                        this.Controller.Model.HistoryManager.EndAtomicAction();
                    }
                }
            }
        }

        #region protected
        /// <summary>
        /// Gets the reference to diagram controller.
        /// </summary>
        /// <value>The controller.</value>
        protected DiagramController Controller
        {
            get { return m_controller; }
        }
        #endregion

        #endregion

        #region public methods
        /// <summary>
        /// Gets selected nodes text formatting values.
        /// </summary>
        /// <remarks>
        /// Iterates through controller.SelectionList NodeCollection but only text nodes
        /// formatting values are taken into account.
        /// </remarks>
        /// <param name="bFirstTexBaseInSelected">Text in the selection base.</param>
        /// <returns>SelectionFormat structure containing text formatting values</returns>
        public SelectionFormat GetSelectionFormat(bool bFirstTexBaseInSelected)
        {
            System.Drawing.FontStyle fntStyle = System.Drawing.FontStyle.Regular;
            Color clrText = Color.Empty;
            StringAlignment alignment = StringAlignment.Near;
            string strFontFamily = string.Empty;
            float fHeightInPoints = 0;
            bool bInit = false;
            NodeCollection nodesSelected = m_controller.SelectionList;

            if ((nodesSelected != null) && (nodesSelected.Count > 0))
            {
                foreach (INode nodeCur in nodesSelected)
                {
                    TextNode nodeText = nodeCur as TextNode;

                    if (nodeText != null)
                    {
                        if (!bInit)
                        {
                            // Init format values.
                            fntStyle = nodeText.FontStyle.Style;
                            clrText = nodeText.FontColorStyle.Color;
                            strFontFamily = nodeText.FontStyle.Family;
                            fHeightInPoints = nodeText.FontStyle.PointSize;
                            alignment = nodeText.HorizontalAlignment;

                            if (bFirstTexBaseInSelected)
                                break;

                            bInit = true;
                        }
                        else
                        {
                            System.Drawing.FontStyle fntStyleCur = nodeText.FontStyle.Style;

                            CheckFontStyle(fntStyleCur, ref fntStyle);

                            if (clrText != nodeText.FontColorStyle.Color)
                                clrText = Color.Empty;

                            if (strFontFamily != nodeText.FontStyle.Family)
                                strFontFamily = string.Empty;

                            if (fHeightInPoints != nodeText.FontStyle.PointSize)
                                fHeightInPoints = 0;

                            if (alignment != nodeText.HorizontalAlignment)
                                alignment = StringAlignment.Near;
                        }
                    }
                }
            }

            return new SelectionFormat(fntStyle, strFontFamily, fHeightInPoints, clrText, alignment, bInit);
        }

        /// <summary>
        /// Sets the selection.
        /// </summary>
        /// <param name="nSelectionStart">The n selection start.</param>
        /// <param name="nSelectionLength">Length of the selection.</param>
        public void SetSelection(int nSelectionStart, int nSelectionLength)
        {
            if (m_txtEditor != null)
            {
                if (m_txtEditor is RichTextBox)
                {
                    ((RichTextBox)m_txtEditor).SelectionStart = nSelectionStart;
                    ((RichTextBox)m_txtEditor).SelectionLength = nSelectionLength;
                }
                else if (m_txtEditor is TextBox)
                {
                    ((TextBox)m_txtEditor).SelectionStart = nSelectionStart;
                    ((TextBox)m_txtEditor).SelectionLength = nSelectionLength;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified node to edit is editable.
        /// </summary>
        /// <param name="nodeToEdit">The node to edit.</param>
        /// <returns>
        /// <c>true</c> if the specified node to edit is editable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEditable(Node nodeToEdit)
        {
            Node resultNode = null;
            Node nodeEditing = CheckForBelongingNode(nodeToEdit);

            // 1 - check for a TextNode object.
            resultNode = CheckForTextNode(nodeEditing);

            // 2 - check for a RichTextNode object.
            if (resultNode == null)
                resultNode = CheckForRTFNode(nodeEditing);

            // 3 - check for a Label object.
            if (resultNode == null)
                resultNode = CheckForLabel(nodeEditing);
            
            // // 4 - check for a TextNode object.
            // if( resultNode == null )
            //   resultNode = CheckForDelaultLabel( nodeToEdit );
            return resultNode != null;
        }

        /// <summary>
        /// Begins the edit.
        /// </summary>
        /// <param name="nodeToEdit">The node to edit.</param>
        /// <param name="bAutoResize">if set to <c>true</c> automatic resize field.</param>
        public void BeginEdit(Node nodeToEdit, bool bAutoResize)
        {
            if (nodeToEdit == null)
                throw new ArgumentNullException("nodeToEdit", "Editing node must be non null.");

            Node nodeEditing = GetNodeToEdit(nodeToEdit);

            if (nodeEditing != null)
            {
                // if there is already opened text editor close it.
                if (m_txtEditor != null)
                {
                    if (m_nodeEditing != nodeEditing)
                    {
                        m_txtEditor.EndEdit(true);
                        m_txtEditor = StartEditing(nodeEditing, bAutoResize);
                    }
                }
                else
                    m_txtEditor = StartEditing(nodeEditing, bAutoResize);
            }

            OnFormatChanged();
        }

        /// <summary>
        /// Ends the edit.
        /// </summary>
        /// <param name="bSaveChanges">if set to <c>true</c> save changes.</param>
        public void EndEdit(bool bSaveChanges)
        {
            if (m_txtEditor != null)
            {
                m_txtEditor.EndEdit(bSaveChanges);

                EventsUnsubscribe(m_txtEditor);

                m_txtEditor = null;
                m_nodeEditing = null;

                OnFormatChanged();
            }
        }
        #endregion

        #region helper methods

        #region start/end edit
        /// <summary>
        /// Starts the editing.
        /// </summary>
        /// <param name="nodeToEdit">The node to edit.</param>
        /// <param name="bAutoResize">if set to <c>true</c> automatic resize text field.</param>
        /// <returns>The text editor to edit.</returns>
        protected virtual ITextEditor StartEditing(Node nodeToEdit, bool bAutoResize)
        {
            ITextEditor txtEditor = null;

            if (nodeToEdit is RichTextNode)
                txtEditor = StartRTFNodeEdit(nodeToEdit as RichTextNode);
            else if (nodeToEdit is TextNode)
                txtEditor = StartTextNodEdit(nodeToEdit as TextNode, bAutoResize);

            if (txtEditor != null)
                m_nodeEditing = nodeToEdit;

            return txtEditor;
        }

        /// <summary>
        /// Starts the RTF node edit.
        /// </summary>
        /// <param name="nodeRTF">The RTF node.</param>
        /// <returns>The text editor to edit RTF.</returns>
        protected virtual ITextEditor StartRTFNodeEdit(RichTextNode nodeRTF)
        {
            if (nodeRTF == null)
                throw new ArgumentNullException("nodeRTF", "Editing node must be RichTextNode.");

            RichTextEdit rtfEdit = null;

            // check whether viewer is Control subclass
            if (m_controller.Viewer is Control)
            {
                rtfEdit = new RichTextEdit(m_controller.Viewer);

                SubscribeForEvents(rtfEdit);
               
                rtfEdit.BeginEdit(nodeRTF);
                rtfEdit.BorderStyle = BorderStyle.FixedSingle;
                rtfEdit.Focus();
            }

            return rtfEdit;
        }

        /// <summary>
        /// Starts the text node edit.
        /// </summary>
        /// <param name="nodeText">The node text.</param>
        /// <param name="bAutoResize">if set to <c>true</c> automatic resize text field.</param>
        /// <returns>The text editor to edit text node.</returns>
        protected virtual ITextEditor StartTextNodEdit(TextNode nodeText, bool bAutoResize)
        {
            if (nodeText == null)
                throw new ArgumentNullException("nodeText", "Editing node must be derived from TextBase.");

            TextEdit txtEdit = null;

            // check whether viewer is Control subclass
            if (m_controller.Viewer is Control)
            {
                txtEdit = new TextEdit(m_controller.Viewer, bAutoResize);

                SubscribeForEvents(txtEdit);
                txtEdit.BorderStyle = BorderStyle.FixedSingle;
                txtEdit.BeginEdit(nodeText);
                txtEdit.Focus();
            }

            return txtEdit;
        }
        private void SubscribeForEvents(ITextEditor txtEditor)
        {
            if (txtEditor == null)
                throw new ArgumentNullException("txtEditor", "Text Editor can not be null!");

            if (txtEditor is RichTextBox)
            {
                RichTextEdit rtfEdit = txtEditor as RichTextEdit;

                if (rtfEdit != null)
                {
                    rtfEdit.EditorTextChanged += new EditorTextChangedEventHandler(EditorTextChanged);
                    rtfEdit.SelectionChanged += new EventHandler(RTFEditor_SelectionChanged);
                }
            }
            else if (txtEditor is TextBoxBase)
            {
                TextEdit txtEdit = txtEditor as TextEdit;

                if (txtEdit != null)
                    txtEdit.EditorTextChanged += new EditorTextChangedEventHandler(EditorTextChanged);
            }
        }
        private void EventsUnsubscribe(ITextEditor txtEditor)
        {
            if (txtEditor == null)
                throw new ArgumentNullException("txtEditor", "Text Editor can not be null!");

            if (txtEditor is RichTextBox)
            {
                RichTextEdit rtfEdit = txtEditor as RichTextEdit;

                if (rtfEdit != null)
                {
                    rtfEdit.EditorTextChanged -= new EditorTextChangedEventHandler(EditorTextChanged);
                    rtfEdit.SelectionChanged -= new EventHandler(RTFEditor_SelectionChanged);
                }
            }
            else if (txtEditor is TextBoxBase)
            {
                TextEdit txtEdit = txtEditor as TextEdit;

                if (txtEdit != null)
                    txtEdit.EditorTextChanged -= new EditorTextChangedEventHandler(EditorTextChanged);
            }
        }

        private string ParseRtfText(string rtfText)
        {
            string strResult = rtfText;

            if (strResult != null && System.Text.RegularExpressions.Regex.Match(strResult, c_str_DEF_RTF_HEADER).Success)
            {
                // if current text in rtf format
                using (RichTextBox richConvertor = new RichTextBox())
                {
                    richConvertor.Rtf = strResult;
                    strResult = richConvertor.Text;
                }
            }

            return strResult;
        }

        #endregion

        #region checks
        /// <summary>
        /// Checks for belonging node.
        /// </summary>
        /// <param name="nodeToEdit">The node to edit.</param>
        /// <returns>Node to edit.</returns>
        private Node CheckForBelongingNode(Node nodeToEdit)
        {
            Node nodeResult = nodeToEdit;

            // Check to see if we hit container node.
            ICompositeNode compositeNode = nodeToEdit as ICompositeNode;

            if (compositeNode != null)
            {
                NodeCollection childrenHit = new NodeCollection();
                compositeNode.GetChildrenAtPoint(childrenHit, this.Controller.MouseLocation);
                int nChildCount = childrenHit.Count;

                if (nChildCount > 0)
                {
                    // if node is container then look in.
                    Node nodeChild = CheckForBelongingNode(childrenHit[nChildCount - 1]);

                    nodeResult = (nodeChild != null) ? nodeChild : childrenHit[nChildCount - 1];
                }
            }

            return nodeResult;
        }
        private Node CheckForLabel(Node nodeToEdit)
        {
            // INode nodeEditable = null;
            //
            // // Check to see if a label was double-clicked.
            // Syncfusion.Windows.Forms.Diagram.Label label = nodeToEdit as Syncfusion.Windows.Forms.Diagram.Label;
            // 
            // if( label != null && !label.ReadOnly )
            // {
            //     ILabelContainer labelCont = label.Container;
            //
            //     if( labelCont != null )
            //       nodeEditable = label;
            // }
            // return nodeEditable;
            return null;
        }

        // private Node CheckForDelaultLabel( Node nodeToEdit )
        // {
        //    Node nodeEditable = null;
        //    // Check to see if a label container was double-clicked.
        //    ILabelContainer labelContainer = nodeToEdit as ILabelContainer;
        //
        //    if( labelContainer != null )
        //    {
        //          Syncfusion.Windows.Forms.Diagram.Label defaultLabel = labelContainer.DefaultLabel;
        //
        //          if( defaultLabel != null && !defaultLabel.ReadOnly )
        //            nodeEditable = defaultLabel;
        //    }
        //
        //    return nodeEditable;
        // }
        private Node CheckForTextNode(Node nodeToEdit)
        {
            TextNode nodeEditable = nodeToEdit as TextNode;

            if (nodeEditable != null && (nodeEditable.ReadOnly || !nodeEditable.EditStyle.Enabled))
                nodeEditable = null;

            return nodeEditable;
        }
        private Node CheckForRTFNode(Node nodeToEdit)
        {
            RichTextNode nodeEditable = nodeToEdit as RichTextNode;

            if (nodeEditable != null && (nodeEditable.ReadOnly || !nodeEditable.EditStyle.Enabled))
                nodeEditable = null;

            return nodeEditable;
        }
        private Node GetNodeToEdit(Node nodeToEdit)
        {
            Node editNode = CheckForBelongingNode(nodeToEdit);

            Node nodeToReturn = CheckForTextNode(editNode);

            if (nodeToReturn == null)
                nodeToReturn = CheckForRTFNode(editNode);

            // if( nodeToReturn == null )
            //    nodeToReturn = CheckForLabel( nodeToEdit );
            //
            // if( nodeToReturn == null )
            //    nodeToReturn = CheckForDelaultLabel( nodeToEdit ); 
            return nodeToReturn;
        }
        #endregion

        #region Commands

        #region RTF
        private void CreateRTFCmd(TextFormatting typeFmt, object value)
        {
            ICommand cmdFormat = null;

            switch (typeFmt)
            {
                case TextFormatting.Bold:
                    cmdFormat = new RTFBoldCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, (bool)value);
                    break;
                case TextFormatting.Italic:
                    cmdFormat = new RTFItalicCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, (bool)value);
                    break;
                case TextFormatting.Underline:
                    cmdFormat = new RTFUnderlineCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, (bool)value);
                    break;
                case TextFormatting.TextColor:
                    cmdFormat = new RTFColorCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.TextColor, (Color)value);
                    break;
                case TextFormatting.Strikeout:
                    cmdFormat = new RTFStrikeoutCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, (bool)value);
                    break;
                case TextFormatting.Subscript:
                    cmdFormat = new RTFSubscriptCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.Superscript, (bool)value);
                    break;
                case TextFormatting.Superscript:
                    cmdFormat = new RTFSuperscriptCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.Subscript, (bool)value);
                    break;
                case TextFormatting.FamilyName:
                    cmdFormat = new RTFFamilyNameCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.FamilyName, (string)value);
                    break;
                case TextFormatting.PointSize:
                    cmdFormat = new RTFPointSizeCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.PointSize, (float)value);
                    break;
                case TextFormatting.CharOffset:
                    cmdFormat = new RTFCharOffsetCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.CharOffset, (int)value);
                    break;
                case TextFormatting.HorizontalAlignment:
                    cmdFormat = new RTFHorizontalAlignmentCmd(this, m_nodeEditing as RichTextNode, ((RichTextBox)m_txtEditor).SelectionStart, ((RichTextBox)m_txtEditor).SelectionLength, this.HorizontalAlignment, (StringAlignment)value);
                    break;
            }

            // if( cmdFormat != null )
            //     m_controller.AddCommand( cmdFormat, false );
        }
        #endregion

        #region Text
        private void CreateTextCmd(TextFormatting typeFmt, object value)
        {
            ICommand cmdFormat = null;
            TextNode nodeEditing = m_nodeEditing as TextNode;

            if (nodeEditing != null)
            {
                switch (typeFmt)
                {
                    case TextFormatting.Bold:
                        cmdFormat = new BoldTextCmd(this, nodeEditing, (bool)value);
                        break;
                    case TextFormatting.Underline:
                        cmdFormat = new UnderlineTextCmd(this, nodeEditing, (bool)value);
                        break;
                    case TextFormatting.Italic:
                        cmdFormat = new ItalicTextCmd(this, nodeEditing, (bool)value);
                        break;
                    case TextFormatting.Strikeout:
                        cmdFormat = new StrikeoutTextCmd(this, nodeEditing, (bool)value);
                        break;
                    case TextFormatting.FamilyName:
                        cmdFormat = new FontFamilyTextCmd(this, nodeEditing, (string)value);
                        break;
                    case TextFormatting.PointSize:
                        cmdFormat = new FontHeightTextCmd(this, nodeEditing, (float)value);
                        break;
                    case TextFormatting.TextColor:
                        cmdFormat = new ColorTextCmd(this, nodeEditing, (Color)value);
                        break;
                    case TextFormatting.HorizontalAlignment:
                        cmdFormat = new HorizontalAlignmentTextCmd(this, nodeEditing, (StringAlignment)value);
                        break;
                }

                // m_controller.AddCommand( cmdFormat, false );
            }
        }
        private void CreateTextCmd(TextFormatting typeFmt, NodeCollection nodesAffected, object value)
        {
            ICommand cmdFormat = null;

            switch (typeFmt)
            {
                case TextFormatting.Bold:
                    cmdFormat = new BoldTextCmd(nodesAffected, (bool)value);
                    break;
                case TextFormatting.Underline:
                    cmdFormat = new UnderlineTextCmd(nodesAffected, (bool)value);
                    break;
                case TextFormatting.Italic:
                    cmdFormat = new ItalicTextCmd(nodesAffected, (bool)value);
                    break;
                case TextFormatting.Strikeout:
                    cmdFormat = new StrikeoutTextCmd(nodesAffected, (bool)value);
                    break;
                case TextFormatting.FamilyName:
                    cmdFormat = new FontFamilyTextCmd(nodesAffected, (string)value);
                    break;
                case TextFormatting.PointSize:
                    cmdFormat = new FontHeightTextCmd(nodesAffected, (float)value);
                    break;
                case TextFormatting.TextColor:
                    cmdFormat = new ColorTextCmd(nodesAffected, (Color)value);
                    break;
                case TextFormatting.HorizontalAlignment:
                    cmdFormat = new HorizontalAlignmentTextCmd(nodesAffected, (StringAlignment)value);
                    break;
            }

            // m_controller.AddCommand( cmdFormat, true );
        }
        #endregion

        #endregion

        #region utility
        private bool GetSelectionFormatValue(TextFormatting fmtText)
        {
            bool fmtValueToReturn = false;
            SelectionFormat fmtSelection = GetSelectionFormat(false);

            if (fmtSelection.Valid)
            {
                switch (fmtText)
                {
                    case TextFormatting.Bold:
                        fmtValueToReturn =
                            ((fmtSelection.FontStyle & System.Drawing.FontStyle.Bold) == System.Drawing.FontStyle.Bold) ? true : false;
                        break;
                    case TextFormatting.Italic:
                        fmtValueToReturn =
                            ((fmtSelection.FontStyle & System.Drawing.FontStyle.Italic) == System.Drawing.FontStyle.Italic) ? true : false;
                        break;
                    case TextFormatting.Underline:
                        fmtValueToReturn =
                            ((fmtSelection.FontStyle & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Underline) ? true : false;
                        break;
                    case TextFormatting.Strikeout:
                        fmtValueToReturn =
                            ((fmtSelection.FontStyle & System.Drawing.FontStyle.Strikeout) == System.Drawing.FontStyle.Strikeout) ? true : false;
                        break;
                }
            }

            return fmtValueToReturn;
        }

        /// <summary>
        /// Called when format changed.
        /// </summary>
        protected virtual void OnFormatChanged()
        {
            if (FormatChanged != null)
                FormatChanged(this, new EventArgs());
        }
        private NodeCollection GetTextBaseNodes(NodeCollection nodesSelected)
        {
            NodeCollection nodesToReturn = null;

            foreach (Node nodeCur in nodesSelected)
            {
                if (nodeCur is TextNode)
                {
                    if (nodesToReturn == null)
                        nodesToReturn = new NodeCollection();

                    nodesToReturn.Add(nodeCur);
                }
            }

            return nodesToReturn;
        }
        private void CheckFontStyle(System.Drawing.FontStyle fntStyleCur, ref System.Drawing.FontStyle fntStyle)
        {
            if ((fntStyleCur & System.Drawing.FontStyle.Bold) != System.Drawing.FontStyle.Bold)
                fntStyle = fntStyle & (~System.Drawing.FontStyle.Bold);

            if ((fntStyleCur & System.Drawing.FontStyle.Italic) != System.Drawing.FontStyle.Italic)
                fntStyle = fntStyle & (~System.Drawing.FontStyle.Italic);

            if ((fntStyleCur & System.Drawing.FontStyle.Underline) != System.Drawing.FontStyle.Underline)
                fntStyle = fntStyle & (~System.Drawing.FontStyle.Underline);

            if ((fntStyleCur & System.Drawing.FontStyle.Strikeout) != System.Drawing.FontStyle.Strikeout)
                fntStyle = fntStyle & (~System.Drawing.FontStyle.Strikeout);
        }
        #endregion

        #endregion

        #region event handlers
        private void EditorTextChanged(object sender, EditorTextChangedEventArgs evtArgs)
        {
            if (evtArgs != null)
            {
                ICommand cmdRTFText = null;

                switch (evtArgs.TextFormatting)
                {
                    case TextFormatting.InsertText:
                        cmdRTFText = new SetTextCmd(this, m_nodeEditing, evtArgs.SelectionStart, evtArgs.DeletedText, evtArgs.InsertedText);
                        break;
                    case TextFormatting.PasteText:
                        cmdRTFText = new PasteTextCmd(this, m_nodeEditing, evtArgs.SelectionStart, evtArgs.DeletedText, evtArgs.InsertedText);
                        break;
                    case TextFormatting.CutText:
                        cmdRTFText = new CutTextCmd(this, m_nodeEditing, evtArgs.SelectionStart, evtArgs.DeletedText, string.Empty);
                        break;
                    case TextFormatting.DeleteText:
                        cmdRTFText = new DeleteTextCmd(this, m_nodeEditing, evtArgs.SelectionStart, evtArgs.DeletedText, string.Empty);
                        break;
                }

                if (this.TextChanged != null)
                {
                    this.TextChanged(this, new EventArgs());
                }
                // if( evtArgs.Merge )
                // {
                //      if( m_controller.Model.HistoryManager.CanMerge( cmdRTFText ) )
                //         m_controller.Model.HistoryManager.Merge( cmdRTFText );
                //      else
                //         m_controller.Model.HistoryManager.AddEntry( cmdRTFText, null, false ); 
                // }
                // else
                //    m_controller.Model.HistoryManager.AddEntry( cmdRTFText, null, false );
            }
        }
        private void RTFEditor_SelectionChanged(object sender, EventArgs e)
        {
            OnFormatChanged();
        }
        #endregion
    }

    /// <summary>
    /// Helper class used primarily when obtaining 
    /// TextFormatting for SelectedNodes
    /// </summary>
    public class SelectionFormat
    {
        #region fields
        private System.Drawing.FontStyle m_fntStyle;
        private Color m_clrText;
        private float m_fHeightInPoints;
        private StringAlignment m_aligment;
        private string m_strFontFamily;
        private bool m_bValid;
        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionFormat"/> class.
        /// </summary>
        /// <param name="fntStyle">The font style.</param>
        /// <param name="strFontFamily">The font family name.</param>
        /// <param name="fHeightInPoints">The font height in points.</param>
        /// <param name="clrText">The text color.</param>
        /// <param name="alignment">The text alignment.</param>
        /// <param name="bValid">if set to <c>true</c> text format is valid.</param>
        public SelectionFormat(System.Drawing.FontStyle fntStyle, string strFontFamily, float fHeightInPoints, Color clrText, StringAlignment alignment, bool bValid)
        {
            m_fntStyle = fntStyle;
            m_strFontFamily = strFontFamily;
            m_fHeightInPoints = fHeightInPoints;
            m_clrText = clrText;
            m_aligment = alignment;
            m_bValid = bValid;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="SelectionFormat"/> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
        public bool Valid
        {
            get { return m_bValid; }
        }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public System.Drawing.FontStyle FontStyle
        {
            get { return m_fntStyle; }
        }

        /// <summary>
        /// Gets the font family name.
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily
        {
            get { return m_strFontFamily; }
        }

        /// <summary>
        /// Gets the text color.
        /// </summary>
        /// <value>The text color.</value>
        public Color TextColol
        {
            get { return m_clrText; }
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <value>The height of the font.</value>
        public float FontHeight
        {
            get { return m_fHeightInPoints; }
        }

        /// <summary>
        /// Gets the text alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public StringAlignment Alignment
        {
            get { return m_aligment; }
        }
        #endregion
    }
}
