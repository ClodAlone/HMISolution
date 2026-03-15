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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices.WinAPI;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// RichTextBox control for editing text objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements a text box control that is used for editing
    /// text nodes derived from
    /// <see cref="Syncfusion.Windows.Forms.Diagram.RichTextNode"/>.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RichTextNode"/>
    /// </remarks>
    [ToolboxItem(false)]
    public class RichTextEdit
        : RichTextBox,
          ITextEditor
    {
        #region constants
        private const int CFM_BOLD = 1; // 0x0001 
        private const int CFE_BOLD = 1; // 0x0001 
        private const int CFM_ITALIC = 2; // 0x0002 
        private const int CFE_ITALIC = 2; // 0x0002 
        private const int CFM_UNDERLINE = 4; // 0x0004 
        private const int CFE_UNDERLINE = 4; // 0x0004 
        private const int CFM_STRIKEOUT = 8; // 0x0008 
        private const int CFE_STRIKEOUT = 8; // 0x0008 
        private const int CFM_SUPERSCRIPT = 196608; // 0x30000
        private const int CFM_SUBSCRIPT = 196608; // 0x30000
        private const int CFE_SUBSCRIPT = 65536; // 0x10000 
        private const int CFE_SUPERSCRIPT = 131072; // 0x20000 
        private const int EM_SETCHARFORMAT = 1092; // 0x0444 
        private const int EM_GETCHARFORMAT = 1082; // 0x043a 
        private const int SCF_SELECTION = 1; // 0x0001 
        private const uint CFM_SIZE = 0x80000000;
        private const uint CFM_FACE = 0x20000000;
        private const string c_str_DEF_RTF_HEADER = @"^({\\rtf1)";
        #endregion

        #region fields
        private IViewer view;
        private RichTextNode rtfObj;
        private float m_fZoomFactor;
        private bool m_bInitialized;
        private bool m_bFormat;

        /// <summary>
        /// Helper fields for determining corrent action type.
        /// </summary>
        private int m_nSelectionStart;
        private int m_nPrevSelectionStart;
        private int m_nPrevSelectionLength;
        private int m_nSelectionLength;

        /// <summary>
        /// Front rtf text on current selection position.
        /// Used for delete command by Del key where need know about front symbol.
        /// </summary>
        private string m_strFrontSelectedText = String.Empty;
        private string m_strPrevSelectedText = string.Empty;
        private string m_strSelectedText = string.Empty;
        private int m_nPrevTextLength;
        private int m_nTextLength;
        private string m_nPrevText = string.Empty;
        private string m_nText = string.Empty;
        private bool m_bPaste;
        private bool m_bCut;

        /// <summary>
        /// This flag indicate Delete key pressed.
        /// </summary>
        private bool m_bDelete;

        /// <summary>
        /// This flag is used for cut by using Shift+Delete keys shortcut
        /// </summary>
        /// <remarks>
        /// Internal usage only
        /// </remarks>
        private bool m_bShiftCut;

        /// <summary>
        /// This flag is used for paste by using Shift+Insert keys shortcut
        /// </summary>
        /// <remarks>
        /// Internal usage only
        /// </remarks>
        private bool m_bShiftPaste;

        /// <summary>
        /// This flag is used for copy by using Ctrl+Insert keys shortcut
        /// </summary>
        private bool m_bCtrlCopy;

        /// <summary>
        /// Clipboard container for paste by using Shift+Insert keys shortcut
        /// </summary>        
        private string m_strInTimeClipboard = String.Empty;
        private bool m_bCanMegre = true;
        #endregion

        #region events
        /// <summary>
        /// Occurs when text changed.
        /// </summary>
        public event EditorTextChangedEventHandler EditorTextChanged;
        #endregion

        #region initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEdit"/> class.
        /// </summary>
        /// <param name="view">View that is hosting the text edit control.</param>
        public RichTextEdit(IViewer view)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.view = view;
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
            this.SelectionChanged += new EventHandler(RichTextEdit_SelectionChanged);
            this.TextChanged += new EventHandler(RichTextEdit_TextChanged);
        }
        #endregion

        #endregion

        #region public interface

        #region methods
        /// <summary>
        /// Loads and positions the text edit control and goes into edit mode.
        /// </summary>
        /// <param name="node">Node to edit.</param>
        /// <returns>True if editing started; otherwise False.</returns>
        /// <remarks>
        /// <para>
        /// This method loads the control with the text value and properties of the
        /// attached text node. It also positions the control to correspond to
        /// bounds of the text node. Then it hides the text node and makes the
        /// control visible.
        /// </para>
        /// </remarks>
        public bool BeginEdit(Node node)
        {
            if (rtfObj != null)
                return false;

            rtfObj = node as RichTextNode;

            // Set the background color of the control.
            Color backColor = rtfObj.BackgroundColor;

            if (backColor != Color.Transparent)
            {
                this.BackColor = backColor;
            }

            // Rich text is always multiline.
            this.Multiline = true;

            // Apply magnification factor
            ApplyMagnificationFactor(true);

            // Set position and size of the text edit control.
            UpdateControlBounds();
            //this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Show and enable the control.
            this.Parent = (Control)view;
            this.Visible = true;
            this.Enabled = true;

            // Load control with the rich text value of the attached node.
            this.Rtf = rtfObj.RichText;
           
            // Set Initialized flag.
            m_bInitialized = true;

            UpdateTextFormatting();

            return m_bInitialized;
        }

        /// <summary>
        /// Saves the changes made in the control to the attached text node and
        /// ends edit mode.
        /// </summary>
        /// <param name="bSaveChanges">if set to <c>true</c> save changes.</param>
        public void EndEdit(bool bSaveChanges)
        {
            this.Enabled = false;
            this.Visible = false;

            if (rtfObj != null && bSaveChanges)
            {
                ApplyMagnificationFactor(false);

                // since all RTF changes are already in HistoryList
                // no need to record RTF string assigning
                rtfObj.RichText = this.Rtf;
            }
        }
        #endregion

        #region properties
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
        /// Gets or sets the current contents of the text editor.
        /// </summary>
        public string CurrentText
        {
            get
            {
                if (this.rtfObj != null)
                    return this.SelectedRtf;

                return string.Empty;
            }

            set
            {
                if (this.rtfObj != null)
                {
                    if (value == string.Empty)
                        this.SelectedText = value;
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.Match(value, c_str_DEF_RTF_HEADER).Success)
                            this.SelectedRtf = value;
                        else
                            this.SelectedText = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is bold.
        /// </summary>
        public bool Bold
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Bold);
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.Bold, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is in italics.
        /// </summary>
        public bool Italic
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Italic);
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.Italic, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is underlined.
        /// </summary>
        public bool Underline
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Underline);
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.Underline, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text has strikeout property.
        /// </summary>
        public bool Strikeout
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Strikeout);
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.Strikeout, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text has superscript property.
        /// </summary>
        public bool Superscript
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Superscript);
            }
            set
            {
                m_bFormat = true;

                SetFormattingValue(TextFormatting.Superscript, value);

                if (!value)
                    this.CharOffset = 0;

                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text has subscript property.
        /// </summary>
        public bool Subscript
        {
            get
            {
                return (bool)GetFormattingValue(TextFormatting.Subscript);
            }
            set
            {
                m_bFormat = true;

                SetFormattingValue(TextFormatting.Subscript, value);

                if (!value)
                    this.CharOffset = 0;

                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets name of font family.
        /// </summary>
        public string FontFamily
        {
            get
            {
                string strFamilyNameToReturn = string.Empty;

                // if selected text is formatted with different fonts
                // SelectionFont is null --> return empty string.
                if (this.SelectionFont != null)
                    strFamilyNameToReturn = this.SelectionFont.FontFamily.Name;

                return strFamilyNameToReturn;
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.FamilyName, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets size of font in points.
        /// </summary>
        public float PointSize
        {
            get
            {
                return (int)GetFormattingValue(TextFormatting.FontHeight);
            }
            set
            {
                m_bFormat = true;
                SetFormattingValue(TextFormatting.FontHeight, value);
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets horizontal alignment of text.
        /// </summary>
        public StringAlignment HorizontalAlignment
        {
            get
            {
                StringAlignment value = StringAlignment.Near;

                switch (this.SelectionAlignment)
                {
                    case System.Windows.Forms.HorizontalAlignment.Left:
                        value = StringAlignment.Near;
                        break;

                    case System.Windows.Forms.HorizontalAlignment.Center:
                        value = StringAlignment.Center;
                        break;

                    case System.Windows.Forms.HorizontalAlignment.Right:
                        value = StringAlignment.Far;
                        break;
                }

                return value;
            }
            set
            {
                m_bFormat = true;

                switch (value)
                {
                    case StringAlignment.Near:
                        this.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
                        break;
                    case StringAlignment.Center:
                        this.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
                        break;
                    case StringAlignment.Far:
                        this.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Right;
                        break;
                }

                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets color of text.
        /// </summary>
        public Color TextColor
        {
            get 
            { 
                return this.SelectionColor; 
            }
            set
            {
                m_bFormat = true;
                this.SelectionColor = value;
                m_bFormat = false;
            }
        }

        /// <summary>
        /// Gets or sets the selection char offset.
        /// </summary>
        /// <value></value>
        public int CharOffset
        {
            get 
            { 
                return this.SelectionCharOffset; 
            }
            set
            {
                m_bFormat = true;
                this.SelectionCharOffset = value;
                m_bFormat = false;
            }
        }
        #endregion

        #endregion

        #region Class override
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
            bool bReturnValue;

            if (keyData == Keys.Delete)
                bReturnValue = base.ProcessKeyMessage(ref msg);
            else
                bReturnValue = base.ProcessCmdKey(ref msg, keyData);

            return bReturnValue;
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Raises the <see cref="E:EditorTextChanged"/> event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.EditorTextChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditorTextChanged(EditorTextChangedEventArgs evtArgs)
        {
            if (this.EditorTextChanged != null)
                this.EditorTextChanged(this, evtArgs);
        }
        private void UpdateControlBounds()
        {
            if (view != null)
            {
                SizeF szSizeUnitIndependent = ((IUnitIndependent)rtfObj).GetSize(MeasureUnits.Pixel);
                RectangleF rectBoundingRectUnitIndependent = ((IUnitIndependent)rtfObj).GetBoundingRectangle(MeasureUnits.Pixel, false);

                PointF ptLocation = new PointF(
                    rectBoundingRectUnitIndependent.X + (rectBoundingRectUnitIndependent.Width / 2 - szSizeUnitIndependent.Width / 2),
                    rectBoundingRectUnitIndependent.Y + (rectBoundingRectUnitIndependent.Height / 2 - szSizeUnitIndependent.Height / 2));

                float fMagnification = this.ZoomFactor;
                int nRulers = view.ShowRulers ? view.RulersHeight : 0;

                PointF[] ptsLocation = new PointF[] { ptLocation };

                Matrix mtxTransform = HandlesHitTesting.GetParentsTransformations(rtfObj);
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

        private void ApplyMagnificationFactor(bool bApply)
        {
            if (bApply)
            {
                m_fZoomFactor = this.ZoomFactor;
                this.ZoomFactor = this.view.Magnification / 100f;
            }
            else
                this.ZoomFactor = m_fZoomFactor;
        }

        private void SetFormattingValue(TextFormatting format, object objValue)
        {
            RichTextNativeMethods.CHARFORMAT2 chFormat = new RichTextNativeMethods.CHARFORMAT2();
            chFormat.cbSize = Marshal.SizeOf(chFormat);

            switch (format)
            {
                case TextFormatting.Subscript:
                    chFormat.dwMask |= CFM_SUBSCRIPT;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_SUBSCRIPT;
                    break;
                case TextFormatting.Superscript:
                    chFormat.dwMask |= CFM_SUPERSCRIPT;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_SUPERSCRIPT;
                    break;
                case TextFormatting.Italic:
                    chFormat.dwMask |= CFM_ITALIC;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_ITALIC;
                    break;
                case TextFormatting.Bold:
                    chFormat.dwMask |= CFM_BOLD;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_BOLD;
                    break;
                case TextFormatting.Underline:
                    chFormat.dwMask |= CFM_UNDERLINE;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_UNDERLINE;
                    break;
                case TextFormatting.Strikeout:
                    chFormat.dwMask |= CFM_STRIKEOUT;
                    if ((bool)objValue)
                        chFormat.dwEffects |= CFE_STRIKEOUT;
                    break;
                case TextFormatting.FontHeight:
                    chFormat.dwMask |= CFM_SIZE;
                    float temp = float.Parse(objValue.ToString()) * 20;
                    chFormat.yHeight = (int)temp;
                    break;
                case TextFormatting.FamilyName:
                    chFormat.dwMask |= CFM_FACE;
                    chFormat.szFaceName = new char[32];

                    string strFamilyName = objValue.ToString();
                    for (int i = 0; i < strFamilyName.Length; i++)
                        chFormat.szFaceName[i] = strFamilyName[i];
                    break;
            }

            RichTextNativeMethods.SendMessage(this.Handle, EM_SETCHARFORMAT, SCF_SELECTION, ref chFormat);

            // Update fotmat values
            UpdateTextFormatting();
        }

        private object GetFormattingValue(TextFormatting formatValue)
        {
            object objValueToReturn = null;

            RichTextNativeMethods.CHARFORMAT2 chFormat = new RichTextNativeMethods.CHARFORMAT2();
            chFormat.cbSize = Marshal.SizeOf(chFormat);
            int nSelectedTextFormatting = RichTextNativeMethods.SendMessage(this.Handle, EM_GETCHARFORMAT, SCF_SELECTION, ref chFormat);

            switch (formatValue)
            {
                case TextFormatting.Superscript:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_SUPERSCRIPT) == CFM_SUPERSCRIPT)
                        if ((chFormat.dwEffects & CFE_SUPERSCRIPT) == CFE_SUPERSCRIPT)
                            objValueToReturn = true;
                    break;
                case TextFormatting.Subscript:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_SUBSCRIPT) == CFM_SUBSCRIPT)
                        if ((chFormat.dwEffects & CFE_SUBSCRIPT) == CFE_SUBSCRIPT)
                            objValueToReturn = true;
                    break;
                case TextFormatting.Bold:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_BOLD) == CFM_BOLD)
                        if ((chFormat.dwEffects & CFE_BOLD) == CFE_BOLD)
                            objValueToReturn = true;
                    break;
                case TextFormatting.Italic:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_ITALIC) == CFM_ITALIC)
                        if ((chFormat.dwEffects & CFE_ITALIC) == CFE_ITALIC)
                            objValueToReturn = true;
                    break;
                case TextFormatting.Underline:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_UNDERLINE) == CFM_UNDERLINE)
                        if ((chFormat.dwEffects & CFE_UNDERLINE) == CFE_UNDERLINE)
                            objValueToReturn = true;
                    break;
                case TextFormatting.Strikeout:
                    objValueToReturn = false;
                    if ((nSelectedTextFormatting & CFM_STRIKEOUT) == CFM_STRIKEOUT)
                        if ((chFormat.dwEffects & CFE_STRIKEOUT) == CFE_STRIKEOUT)
                            objValueToReturn = true;
                    break;
                case TextFormatting.FontHeight:
                    objValueToReturn = 0;
                    if ((nSelectedTextFormatting & CFM_SIZE) == CFM_SIZE)
                        objValueToReturn = chFormat.yHeight / 20;
                    break;
            }
            return objValueToReturn;
        }
        private void UpdateTextFormatting()
        {
            // Update Formatting
        }
        private string GetRTF(int nSelectionStart, int nSelectionLength)
        {
            string strRTFToReturn;

            // unsubscribe from SelectionChanged event
            this.SelectionChanged -= new EventHandler(RichTextEdit_SelectionChanged);

            // Save current selection values
            int nSaveSelectionStart = this.SelectionStart;
            int nSaveSelectionLength = this.SelectionLength;

            // change selection values
            this.SelectionStart = nSelectionStart;
            this.SelectionLength = Math.Abs(nSelectionLength);

            // get needed rtf
            strRTFToReturn = this.SelectedRtf;

            // restore saved selection values
            this.SelectionStart = nSaveSelectionStart;
            this.SelectionLength = nSaveSelectionLength;

            // Subscribe back to SelectionChanged event
            this.SelectionChanged += new EventHandler(RichTextEdit_SelectionChanged);

            return strRTFToReturn;
        }
        #endregion Helper Methods

        #region Event handlers
        private void RichTextEdit_SelectionChanged(object sender, EventArgs e)
        {
            m_strFrontSelectedText = String.Empty;
            m_nPrevSelectionStart = m_nSelectionStart;
            m_nSelectionStart = this.SelectionStart;

            m_nPrevSelectionLength = m_nSelectionLength;
            m_nSelectionLength = this.SelectionLength;

            m_nPrevTextLength = m_nTextLength;
            m_nTextLength = this.TextLength;

            m_nPrevText = m_nText;
            m_nText = this.Text;

            // Update text on changed selection position and lenght.
            if (m_nSelectionLength > 0)
            {
                m_strPrevSelectedText = m_strSelectedText;
                m_strSelectedText = this.SelectedRtf;
            }
            else
            {
                if (m_nSelectionStart < this.TextLength)
                    m_strFrontSelectedText = GetRTF(m_nSelectionStart, 1);

                if (m_nSelectionStart > 0)
                {
                    m_strPrevSelectedText = m_strSelectedText;
                    m_strSelectedText = GetRTF(m_nSelectionStart - 1, 1);
                }
                else
                    m_strPrevSelectedText = m_strSelectedText;
            }
        }
        private void RichTextEdit_TextChanged(object sender, EventArgs e)
        {
            if (m_bInitialized && !m_bFormat)
            {
                EditorTextChangedEventArgs evtArgs;

                if ((m_nPrevSelectionLength > 0) && (m_nSelectionLength == 0))
                {
                    // If selection eng more that zero, command can't be merged.
                    m_bCanMegre = false;

                    int nInsertedStringLength = m_nSelectionStart - m_nPrevSelectionStart;
                    int nStart = m_nSelectionStart - nInsertedStringLength;
                    string strAdded = GetRTF(nStart, nInsertedStringLength);

                    if (m_bPaste)
                    {
                        // Support for Shift+Ins shortcut for copying to Clipboard
                        // as we can assign only one shortcut to existing BarItems
                        if (m_bShiftPaste)
                            strAdded = m_strInTimeClipboard;

                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.PasteText, m_nPrevSelectionStart, m_strPrevSelectedText, strAdded, m_bCanMegre);
                        m_bPaste = false;
                    }
                    else if (m_bCut)
                    {
                        // Support for Shift+Del shortcut for copying to Clipboard
                        // as we can assign only one shortcut to existing BarItems
                        if (m_bShiftCut)
                        {
                            Clipboard.SetDataObject(m_strPrevSelectedText, true);
                            m_bShiftCut = false;
                        }

                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.CutText, m_nPrevSelectionStart, m_strPrevSelectedText, string.Empty, m_bCanMegre);
                        m_bCut = false;
                    }
                    else
                    {
                        if (nInsertedStringLength == 0)
                        {
                            evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, m_nPrevSelectionStart, m_strPrevSelectedText, m_strPrevSelectedText, m_bCanMegre);

                            // Delete don't refresh selection text because
                            // selection position don't change , so there selection text is refreshed.
                            if (m_bDelete)
                                OnSelectionChanged(EventArgs.Empty);
                        }
                        else
                            evtArgs = new EditorTextChangedEventArgs(TextFormatting.InsertText, m_nPrevSelectionStart, m_strPrevSelectedText, strAdded, true);
                    }
                }
                else if (m_nPrevTextLength > this.TextLength)
                {
                    if (m_bDelete)
                    {
                        // On deleted by Delete key.
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, m_nSelectionStart, m_strFrontSelectedText, m_strFrontSelectedText, m_bCanMegre);

                        // Delete don't refresh selection text because
                        // selection position don't change , so there text is refreshed.
                        OnSelectionChanged(EventArgs.Empty);
                    }
                    else
                        
                        // On deleted by Backspace key.
                        evtArgs = new EditorTextChangedEventArgs(TextFormatting.DeleteText, m_nSelectionStart, m_strPrevSelectedText, m_strPrevSelectedText, m_bCanMegre);
                }
                else
                {
                    int nInsertedStringLength = m_nSelectionStart - m_nPrevSelectionStart;
                    int nStart = m_nSelectionStart - nInsertedStringLength;
                    string strAdded = GetRTF(nStart, nInsertedStringLength); // strCurr.Substring( nStart, nInsertedStringLength );

                    if (m_bPaste)
                    {
                        // Support for Shift+Ins shortcut for paste from Clipboard
                        // as we can assign only one shortcut to existing BarItems
                        // Workaround. Clipboard value taken from easy member where
                        // it was accept in OnKeyDown. 
                        if (m_bShiftPaste)
                            strAdded = m_strInTimeClipboard;

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
            else if (m_bFormat)
            {
                // Refresh selection text because selection position don't change
                // but text what was selected changed, so there text is refreshed.
                OnSelectionChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // if we moved caret manually no more merging.
            if (e.KeyValue >= 33 && e.KeyValue <= 40)
                m_bCanMegre = false;

            if (e.Control && (e.KeyData & Keys.V) == Keys.V)
                m_bPaste = true;
            else if (e.KeyData == (Keys.Shift | Keys.Insert))
            {
                m_bShiftPaste = true;
                m_bPaste = true;

                // Workaround. Shift+Insert paste rtf text to richtextbox text 
                // but we needed pase rtf text. So we clearing clipboard that 
                // nothing copying by windows message and paste to clipboard
                // later.
                IDataObject clipboardObject = Clipboard.GetDataObject();
                if (clipboardObject.GetDataPresent(typeof(string)))
                {
                    m_strInTimeClipboard = (string)clipboardObject.GetData(typeof(string));
                    Clipboard.SetDataObject(String.Empty, false);

                    this.CurrentText = m_strInTimeClipboard;
                }
            }
            else if (e.Control && (e.KeyData & Keys.X) == Keys.X)
                m_bCut = true;
            else if (e.Control && (e.KeyData & Keys.Insert) == Keys.Insert)
            {
                m_bCtrlCopy = true;
            }
            else if (e.Shift && (e.KeyData & Keys.Delete) == Keys.Delete)
            {
                m_bShiftCut = true;
                m_bCut = true;
            }
            else if ((e.KeyData & Keys.Delete) == Keys.Delete)
                m_bDelete = true;

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Support for Shift+Parse shortcut for copying back 
            // selecting rtf text.
            if (m_bShiftPaste)
            {
                Clipboard.SetDataObject(m_strInTimeClipboard, true);
                m_bShiftPaste = false;
            }
            
            // Support for Ctrl+Ins shortcut for copying to Clipboard
            // as we can assign only one shortcut to existing BarItems
            if (m_bCtrlCopy)
            {
                Clipboard.SetDataObject(this.SelectedRtf, true);
                m_bCtrlCopy = false;
            }

            base.OnKeyUp(e);
        }

        #endregion
    }
}
