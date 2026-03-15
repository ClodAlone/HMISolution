#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Text Box control for editing text objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements a text box control that is used for editing
    /// text nodes derived from the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.TextNode"/>.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.TextNode"/>
    /// </remarks>
    [ToolboxItem(false)]
    public class TextEdit
        : TextBox,
          ITextEditor
    {
        #region Constants
        private const string c_str_DEF_RTF_HEADER = @"^({\\rtf1)";
        #endregion

        #region Class members
        private bool m_bInitialized = false;
        
        /// <summary>
        /// Front char on current selection position.
        /// Used for delete command by Del key where need know about front symbol.
        /// </summary>
        private string m_strFrontSelectedText = String.Empty;
        private string m_strPrevSelectedText;
        private int m_nPrevSelectionLength;
        private int m_nPrevTextLength;
        private int m_nPrevSelectionStart;
        private bool m_bPaste;
        private bool m_bCut;
        private bool m_bCanMegre = true;
        private bool m_bDelete;

        /// <summary>
        /// TextEditor parent.
        /// </summary>
        private IViewer view;
        private TextNode textObj;
        private SizeF minSize;
        private SizeF maxSize = new SizeF(float.MaxValue, float.MaxValue);
        private bool autoResize;
        private bool allowResize;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEdit"/> class.
        /// </summary>
        /// <param name="view">View that is hosting the text edit control.</param>
        public TextEdit(IViewer view)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.view = view;
            this.autoResize = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEdit"/> class.
        /// </summary>
        /// <param name="view">View that is hosting the text edit control.</param>
        /// <param name="autoResize">Flag indicating if text box should be automatically resized.</param>
        public TextEdit(IViewer view, bool autoResize)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.view = view;
            this.autoResize = autoResize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEdit"/> class.
        /// </summary>
        /// <param name="view">View that is hosting the text edit control.</param>
        /// <param name="maxSize">Maximum size the text can grow to.</param>
        /// <param name="autoResize">Flag indicating if text box should be automatically resized.</param>
        public TextEdit(IViewer view, SizeF maxSize, bool autoResize)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.view = view;
            this.maxSize = maxSize;
            this.autoResize = autoResize;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // TextEdit
            // 
            this.AutoSize = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Enabled = false;
            this.Size = new System.Drawing.Size(100, 20);
            this.Visible = false;
        }
        #endregion

        #endregion

        #region public interface

        #region methods
        /// <summary>
        /// Starts editing node.
        /// </summary>
        /// <param name="nodeEditing">node to edit</param>
        /// <returns>success operation value</returns>
        public bool BeginEdit(Node nodeEditing)
        {
            if ((nodeEditing == null) || !(nodeEditing is TextNode))
                throw new ArgumentException("nodeEditing", "Editing Node must be of TextNode type. Check your code.");

            textObj = nodeEditing as TextNode;

            // Load control with the background color of the attached node.
            Color backColor = textObj.BackgroundStyle.Color;

            if (backColor != Color.Transparent)
            {
                this.BackColor = backColor;
            }

            // Load control with the text value of the attached node.
            this.Text = textObj.Text;

            // If text node wraps text, then enable multi-line support in the control.
            this.Multiline = textObj.WrapText;
            this.AcceptsReturn = this.Multiline;

            // Set text alignment.
            SetTextAlign(textObj.HorizontalAlignment);

            // Set the text color.
            this.ForeColor = textObj.FontColorStyle.Color;

            // Load the correct font into the control applying Magnification factor
            float fFontHeight = (textObj.FontStyle.PointSize * view.Magnification) / 100f;
            textObj.FontStyle.PointSize = fFontHeight;
            this.Font = new Font(textObj.FontStyle.Family, fFontHeight, textObj.FontStyle.Style);

            // Calculate minimum size allowed for text box.
            if (minSize.Width == 0.0f || minSize.Height == 0.0f)
            {
                Graphics grfx = CreateGraphics();
                if (grfx != null)
                {
                    SizeF minsize = grfx.MeasureString("X", this.Font, int.MaxValue, textObj.GetStringFormat());
                    minSize = MeasureUnitsConverter.FromPixels(minsize, view.Model.MeasurementUnits);
                    grfx.Dispose();
                }
            }

            // Set position and size of the text edit control.
            UpdateControlBounds();
            //this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Show and enable the control.
            this.Parent = (Control)view;
            this.Visible = true;
            this.Enabled = true;
            this.allowResize = true;

            m_bInitialized = true;
            return m_bInitialized;
        }

        /// <summary>
        /// Saves the changes made in the control to the attached text node and
        /// ends edit mode.
        /// </summary>
        /// <param name="bSaveChanges">Save the changes made in the control.</param>
        public void EndEdit(bool bSaveChanges)
        {
            this.Enabled = false;
            this.Visible = false;

            if (textObj != null && bSaveChanges)
            {
                // get HistoryManager
                Model document = ( Model ) ( ( IServiceReferenceProvider ) view ).ProvideServiceReference( typeof( Model ).TypeHandle );
                
                // start transaction
                if (document != null)
                {
                    document.HistoryManager.StartAtomicAction("SetText");
                }

                UpdateTextNode();

                // Update TextNode size.
                if (allowResize && autoResize)
                    UpdateTextNodeSize();

                // end transaction
                if (document != null)
                {
                    document.HistoryManager.EndAtomicAction();
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets a value indicating whether text box should be automatically resized.
        /// </summary>
        public bool AutoResize
        {
            get
            {
                return this.autoResize;
            }
            set
            {
                this.autoResize = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum size the text box can be.
        /// </summary>
        public SizeF MinSize
        {
            get
            {
                return this.minSize;
            }
            set
            {
                this.minSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum size that the text box can grow to.
        /// </summary>
        public SizeF MaxSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                this.maxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the current contents of the text editor.
        /// </summary>
        public string CurrentText
        {
            get
            {
                return this.SelectedText;
            }

            set
            {
                this.SelectedText = ParseRtfText(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is superscript.
        /// Not used in TextNode
        /// </summary>
        public bool Superscript
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is subscript.
        /// Not used in TextNode
        /// </summary>
        public bool Subscript
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is upper.        
        /// </summary>
        public bool Upper
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is lower.
        /// </summary>
        public bool Lower
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets char offset.
        /// Not used in TextNode.
        /// </summary>
        public int CharOffset
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is bold.
        /// </summary>
        public bool Bold
        {
            get
            {
                return this.Font.Bold;
            }
            set
            {
                if (this.textObj != null && this.Bold !=value)
                {
                    this.Font = new Font(this.Font, this.Font.Style ^ System.Drawing.FontStyle.Bold);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is in italics.
        /// </summary>
        public bool Italic
        {
            get
            {
                return this.Font.Italic;
            }
            set
            {
                if (this.textObj != null && this.Italic != value)
                    this.Font = new Font(this.Font, this.Font.Style ^ System.Drawing.FontStyle.Italic);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is underlined.
        /// </summary>
        public bool Underline
        {
            get
            {
                return this.Font.Underline;
            }
            set
            {
                if (this.textObj != null && this.Underline !=value)
                    this.Font = new Font(this.Font, this.Font.Style ^ System.Drawing.FontStyle.Underline);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text has strikeout property.
        /// </summary>
        public bool Strikeout
        {
            get
            {
                return this.Font.Strikeout;
            }
            set
            {
                if (this.textObj != null)
                    this.Font = new Font(this.Font, this.Font.Style ^ System.Drawing.FontStyle.Strikeout);
            }
        }

        /// <summary>
        /// Gets or sets name of font family.
        /// </summary>
        public string FontFamily
        {
            get
            {
                return this.Font.FontFamily.Name;
            }
            set
            {
                if ((this.textObj != null) && (value != null))
                    this.Font = new Font(new FontFamily(value), this.Font.Size, this.Font.Style);
            }
        }

        /// <summary>
        /// Gets or sets size of font in points.
        /// </summary>
        public float PointSize
        {
            get
            {
                return this.Font.SizeInPoints;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("PointSize", "Font Size can not be of negative value.");

                if (this.textObj != null)
                {
                    this.Font = new Font(this.Font.FontFamily, value, this.Font.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets horizontal alignment of text.
        /// </summary>
        public StringAlignment HorizontalAlignment
        {
            get
            {
                return this.Convert(this.TextAlign);
            }
            set
            {
                this.SetTextAlign(value);
            }
        }

        /// <summary>
        /// Gets or sets color of text.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return this.ForeColor;
            }
            set
            {
                this.ForeColor = value;
            }
        }

        #endregion

        #endregion

        #region Class events
        /// <summary>
        /// Occurs when text changed.
        /// </summary>
        public event EditorTextChangedEventHandler EditorTextChanged;
        #endregion

        #region Class override
        /// <summary>
        /// Called when the text in the control changes.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// <para>
        /// If the auto-resize option is turned on, this method resizes the control
        /// so that the text fits.
        /// </para>
        /// </remarks>
        protected override void OnTextChanged(EventArgs e)
        {
            if (this.allowResize && this.autoResize)
                this.UpdateControlBounds();

            if (m_bInitialized)
            {
                EditorTextChangedEventArgs evtArgs;
                string strCurr = this.Text;

                if ((m_nPrevSelectionLength > 0) && (this.SelectionLength == 0))
                {
                    // If selection eng more that zero, command can't be merged.
                    m_bCanMegre = false;

                    int nInsertedStringLength = this.SelectionStart - m_nPrevSelectionStart;
                    int nStart = this.SelectionStart - nInsertedStringLength;
                    string strAdded = strCurr.Substring(nStart, nInsertedStringLength);

                    if (m_bPaste)
                    {
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.PasteText, m_nPrevSelectionStart, m_strPrevSelectedText, strAdded, m_bCanMegre);
                        m_bPaste = false;
                    }
                    else if (m_bCut)
                    {
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.CutText, m_nPrevSelectionStart, m_strPrevSelectedText, string.Empty, m_bCanMegre);
                        m_bCut = false;
                    }
                    else
                    {
                        if (strAdded == string.Empty)
                        {
                            evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, m_nPrevSelectionStart, m_strPrevSelectedText, string.Empty, m_bCanMegre);

                            // Delete don't refresh selection text because
                            // selection position don't change , so there selection text is refreshed.
                            if (m_bDelete)
                                UpdateSelection();
                        }
                        else
                            evtArgs = new EditorTextChangedEventArgs(TextFormatting.InsertText, m_nPrevSelectionStart, m_strPrevSelectedText, strAdded, true);
                    }
                }
                else if (m_nPrevTextLength > this.TextLength)
                {
                    if (m_bDelete)
                    {
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, this.SelectionStart, m_strFrontSelectedText, m_strFrontSelectedText, m_bCanMegre);

                        // Delete don't refresh selection text because
                        // selection position don't change , so there text is refreshed.
                        UpdateSelection();
                    }
                    else
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, this.SelectionStart, m_strPrevSelectedText, m_strPrevSelectedText, m_bCanMegre);
                }
                else
                {
                    int nInsertedStringLength = this.SelectionStart - m_nPrevSelectionStart;
                    int nStart = this.SelectionStart - nInsertedStringLength;
                    string strAdded = strCurr.Substring(nStart, nInsertedStringLength);

                    if (m_bPaste)
                    {
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.PasteText, m_nPrevSelectionStart, string.Empty, strAdded, false);
                        m_bPaste = false;
                    }
                    else
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.InsertText, m_nPrevSelectionStart, string.Empty, strAdded, m_bCanMegre);
                }

                // If seection length more that zero, command can't be merged
                if (m_nPrevSelectionLength == 0 && !m_bCanMegre && !m_bDelete)
                    m_bCanMegre = true;

                // reset delete key pressed
                if (m_bDelete)
                    m_bDelete = false;

                OnEditorTextChanged(evtArgs);
            }
        }

        /// <summary>
        /// Gets a value indicating the state of the <see cref="P:System.Windows.Forms.TextBoxBase.ShortcutsEnabled"/> property.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference that represents the window message to process.</param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the shortcut key to process.</param>
        /// <returns>
        /// true if the shortcut key was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bResult;

            // Override Shift Insert standart shortcut for parse rtf and paste text .
            // Fixed: D11142
            // if( (( keyData & Keys.Shift ) == Keys.Shift) && (( keyData & Keys.Insert ) == Keys.Insert) )
            if (keyData == (Keys.Shift | Keys.Insert))
            {
                bResult = m_bPaste = true;
                PasteClipboardText();
            }
            else if (keyData == Keys.Delete)
            {
                bResult = base.ProcessKeyMessage(ref msg);
            }
            else
            {
                bResult = base.ProcessCmdKey(ref msg, keyData);
            }

            return bResult;
        }

        /// <summary>
        /// Update selection change in text.
        /// </summary>
        /// <remarks>
        /// There sets pevius and current selection position and length.
        /// </remarks>
        private void UpdateSelection()
        {
            m_strFrontSelectedText = String.Empty;
            m_nPrevSelectionLength = this.SelectionLength;
            m_nPrevTextLength = this.TextLength;
            m_nPrevSelectionStart = this.SelectionStart;

            if (this.SelectionLength > 0)
            {
                m_strPrevSelectedText = this.Text.Substring(this.SelectionStart, this.SelectionLength);
            }
            else
            {
                if (this.SelectionStart < this.TextLength)
                    m_strFrontSelectedText = this.Text.Substring(this.SelectionStart, 1);

                if (this.SelectionStart > 0)
                    m_strPrevSelectedText = this.Text.Substring(this.SelectionStart - 1, 1);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // if( e.KeyValue >= 16 && e.KeyValue <= 18 ) // Alt, Control, Shift
            //   return;
            UpdateSelection();

            // if we moved caret manually no more merging.
            if (e.KeyValue >= 33 && e.KeyValue <= 40)
                m_bCanMegre = false;

            if ((e.Control && (e.KeyData & Keys.V) == Keys.V) ||
                (e.Shift && (e.KeyData & Keys.Insert) == Keys.Insert))
                m_bPaste = true;
            else if ((e.Control && (e.KeyData & Keys.X) == Keys.X) ||
                (e.Shift && (e.KeyData & Keys.Delete) == Keys.Delete))
                m_bCut = true;
            else if ((e.KeyData & Keys.Delete) == Keys.Delete)
                m_bDelete = true;
        }

        /// <summary>
        /// Raises the <see cref="E:EditorTextChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.EditorTextChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditorTextChanged(EditorTextChangedEventArgs evtArgs)
        {
            if (this.EditorTextChanged != null)
                this.EditorTextChanged(this, evtArgs);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Paste parsed text from clipboard.
        /// </summary>
        private void PasteClipboardText()
        {
            string strResult = String.Empty;

            IDataObject dataClipboard = Clipboard.GetDataObject();
            if (dataClipboard.GetDataPresent(typeof(string)))
                strResult = ParseRtfText((string)dataClipboard.GetData(typeof(string)));

            this.SelectedText = strResult;
        }

        /// <summary>
        /// Converting rtf to easy text.
        /// </summary>
        /// <param name="rtfText">The RTF text.</param>
        /// <returns>Easy text.</returns>
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
        private void UpdateTextNodeSize()
        {
            Graphics grfx = CreateGraphics();

            if (grfx != null)
            {
                textObj.SizeToText(grfx, maxSize);
                grfx.Dispose();
            }
            else
            {
                textObj.SizeToText(maxSize);
            }
        }
        private void UpdateTextNode()
        {
            // Save Text.
            textObj.Text = this.Text;
            
            // Save Font style.
            textObj.FontStyle.Bold = this.Font.Bold;
            textObj.FontStyle.Family = this.Font.FontFamily.Name;
            float fFontHeight = (this.Font.SizeInPoints * 100f) / view.Magnification;
            textObj.FontStyle.PointSize = fFontHeight;
            textObj.FontStyle.Family = this.Font.Name;
            textObj.FontStyle.Style = this.Font.Style;

            // Save Alignment.
            this.textObj.HorizontalAlignment = Convert(this.TextAlign);

            // Save Font Color.
            this.textObj.FontColorStyle.Color = this.ForeColor;
        }
        private void UpdateControlBounds()
        {
            if (this.view != null)
            {
                SizeF szSizeUnitIndependent = ((IUnitIndependent)textObj).GetSize(MeasureUnits.Pixel);
                RectangleF rectBoundingRectUnitIndependent = ((IUnitIndependent)textObj).GetBoundingRectangle(MeasureUnits.Pixel, false);

                PointF ptLocation = new PointF(
                    rectBoundingRectUnitIndependent.X + (rectBoundingRectUnitIndependent.Width / 2 - szSizeUnitIndependent.Width / 2),
                    rectBoundingRectUnitIndependent.Y + (rectBoundingRectUnitIndependent.Height / 2 - szSizeUnitIndependent.Height / 2));

                float fMagnification = view.Magnification / 100f;
                int nRulers = view.ShowRulers ? view.RulersHeight : 0;

                PointF[] ptsLocation = new PointF[] { ptLocation };

                Matrix mtxTransform = HandlesHitTesting.GetParentsTransformations(textObj);
                mtxTransform.TransformPoints(ptsLocation);
                ptLocation = ptsLocation[0];

                ptLocation.X = (ptLocation.X - view.Origin.X) * fMagnification + nRulers;
                ptLocation.Y = (ptLocation.Y - view.Origin.Y) * fMagnification + nRulers;

                szSizeUnitIndependent.Width *= fMagnification;
                szSizeUnitIndependent.Height *= fMagnification;

                this.Location = Geometry.ConvertPoint(ptLocation);
                this.Size = new Size((int)szSizeUnitIndependent.Width, (int)szSizeUnitIndependent.Height);
            }
        }

        private void SetTextAlign(StringAlignment value)
        {
            switch (value)
            {
                case StringAlignment.Near:
                    this.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
                    break;

                case StringAlignment.Center:
                    this.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                    break;

                case StringAlignment.Far:
                    this.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
                    break;
            }
        }
        private StringAlignment Convert(HorizontalAlignment alignToConvert)
        {
            StringAlignment alignmentToReturn = StringAlignment.Near;

            switch (alignToConvert)
            {
                case System.Windows.Forms.HorizontalAlignment.Center:
                    alignmentToReturn = StringAlignment.Center;
                    break;
                case System.Windows.Forms.HorizontalAlignment.Right:
                    alignmentToReturn = StringAlignment.Far;
                    break;
            }

            return alignmentToReturn;
        }
        #endregion
    }
}
