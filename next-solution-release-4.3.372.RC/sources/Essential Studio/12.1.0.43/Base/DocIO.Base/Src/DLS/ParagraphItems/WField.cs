#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Windows;
using System.Collections.Generic;

#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
using Font = System.Drawing.Font;
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WField.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WField :
      WTextRange,
      IWField
#if !SILVERLIGHT && !WP
      ,ILeafWidget
#endif
    {
        #region Constants
        private const char COMMASEPARATOR = ',';
        private const char OPENPARENTHESIS = '(';
        private const char CLOSEPARENTHESIS = ')';
        private const string PARAGRAPHMARK = "\r";
        private const string CELLMARK = "\a";
        private const string ROWMARK = "\r\a";
        private const char TableStartMark = (char)19;
        private const char TableEndMark = (char)21;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_fieldPattern = "{0}";
        /// <summary>
        /// 
        /// </summary>
        protected FieldType m_fieldType;
        /// <summary>
        /// 
        /// </summary>
        protected bool m_bConvertedToText = false;

        //    private Regex m_regex = new Regex( "(\\w+)\\s+(\\\\l\\s+)?\"([^\"]+)\"" );
        static private Regex m_regex = new Regex("(\\w+)\\s+\"?([^:\"]+):?([^\"]*)\"?");
        static private Regex m_regexFillIn = new Regex("(\\w+)\\s+\"?([^:\"]+)?([^\"]*)\"?");
        /// <summary>
        /// For Hyperlinks only
        /// </summary>   
        static private Regex m_hyperlinkRegex = new Regex("HYPERLINK\\s+(\\\\l\\s+)?[\"]?([^\"]+)(\"| )");
        //    static private Regex m_tocRegex = new Regex( @"(TOC)(?<Options>\s+(\\o)\s+(""\d-\d"")?(\s+\\\w+)+)" );
        static private Regex m_tocRegex = new Regex(@"(TOC\s+)(?<Options>.*)");
        static private Regex m_includePictureRegex = new Regex(@"INCLUDEPICTURE\s+""([^""]+)""(?<Options>.*)");
        //(TOC\s+)(?<Options>.*)
        private bool m_bIsLocal;
        /// <summary>
        /// 
        /// </summary>
        protected ParagraphItemType m_paraItemType;
        /// <summary>
        /// 
        /// </summary>
        protected internal string m_formattingString = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        protected internal string m_fieldValue = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        protected TextFormat m_textFormat;
        /// <summary>
        /// Local reference string for the hyperlink
        /// </summary>
        private string m_localRef;
        /// <summary>
        /// 
        /// </summary>
        private string m_fieldCode = string.Empty;

        /// <summary>
        /// The source field type value (used in case field type is unknown)
        /// </summary>
        private int m_sourceFldType;
        private bool m_isCloned;
        private Range m_range;
        protected bool m_bIsFieldRangeUpdated;
        private string m_fieldResult = string.Empty;
        protected bool m_bIsFieldSeparator;
        protected bool m_bIsSkip;
        protected Stack<WField> m_nestedFields = new Stack<WField>();
        private List<Entity> m_itemsToUpdate = new List<Entity>();
        private WFieldMark m_fieldSeparator;
        private WFieldMark m_fieldEnd;
        /// <summary>
        /// Preserves the character formatting of field's previous result.
        /// </summary>
        private WCharacterFormat m_resultFormat = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/ sets regular text format.
        /// </summary>
        public TextFormat TextFormat
        {
            get
            {
                return m_textFormat;
            }
            set
            {
                m_textFormat = value;
                SetModified();
                SetTextFormatSwitchString();
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Field;
            }
        }
        /// <summary>
        /// Gets / sets field pattern.
        /// </summary>
        public string FieldPattern
        {
            get
            {
                return m_fieldPattern;
            }
            set
            {
                m_fieldPattern = value;
                SetModified();
            }
        }
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <value>The field value.</value>
        public string FieldValue
        {
            get
            {
                return m_fieldValue;
            }
        }
        /// <summary>
        /// Gets / sets field type
        /// </summary>
        /// <value></value>
        public FieldType FieldType
        {
            get
            {
                return m_fieldType;
            }
            set
            {
                m_fieldType = value;
                SetModified();
            }
        }
        /// <summary>
        /// Is Local Hyperlink
        /// </summary>
        internal bool IsLocal
        {
            get
            {
                return m_bIsLocal;
            }
            set
            {
                m_bIsLocal = value;
                SetModified();
                SetLocalSwitchString();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [converted to text].
        /// </summary>
        /// <value>If it converted to text, set to <c>true</c>.</value>
        internal bool ConvertedToText
        {
            get
            {
                return m_bConvertedToText;
            }
            set
            {
                m_bConvertedToText = value;
                SetModified();
            }
        }
        /// <summary>
        /// Gets the formatting string.
        /// </summary>
        /// <value>The formatting string.</value>
        internal string FormattingString
        {
            get
            {
                return m_formattingString;
            }
            set
            {
                m_formattingString = value;
                SetModified();
            }
        }
        /// <summary>
        /// Gets the local reference for the hyperlink.
        /// </summary>
        /// <value>The local reference.</value>
        internal string LocalReference
        {
            get
            {
                return m_localRef;
            }
            set
            {
                m_localRef = value;
                SetModified();
            }
        }
        /// <summary>
        /// Gets or sets the field code.
        /// </summary>
        /// <value>The field code.</value>
        public string FieldCode
        {
            get
            {
                return m_fieldCode;
            }
            set
            {
                m_fieldCode = value;
                if (!this.Document.IsOpening)
                {
                    //Update field type
                    m_fieldType = FieldTypeDefiner.GetFieldType(m_fieldCode);
                    //Updates FieldValue, FormattingString, MergeFormats (if mergefield)
                    this.UpdateFieldCode(m_fieldCode);
                }

                if (!this.Document.IsOpening 
                    && !this.Document .IsMailMerge 
                    && this.FieldType == FieldType.FieldMergeField 
                    && this.Text == string.Empty)
                {
                    char symbol1 = (char)((byte)171);
                    char symbol2 = (char)((byte)187);
                    this.Text = symbol1 + (this as WMergeField ).FieldName  + symbol2;
                }
            }
        }
        /// <summary>
        /// Gets or sets the type of the source field.
        /// </summary>
        /// <value>The type of the source field.</value>
        internal int SourceFieldType
        {
            get
            {
                return m_sourceFldType;
            }
            set
            {
                m_sourceFldType = value;
            }
        }
        /// <summary>
        /// Gets or sets the field result.
        /// </summary>
        /// <value>The field result.</value>
        internal string FieldResult
        {
            get
            {
                return m_fieldResult;
            }
            set
            {
                m_fieldResult = value;
            }
        }
        /// <summary>
        /// Gets the nested field code.
        /// </summary>
        /// <value>The nested field code.</value>
        internal string NestedFieldCode
        {
            get
            {
                return UpdateNestedFieldCode();
            }
        }
        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <value>The range.</value>
        internal Range Range
        {
            get
            {
                if (m_range == null)
                    m_range = new Range(Document, this);
                else if (!m_bIsFieldRangeUpdated
                    && !Document.IsOpening
                    && !Document.IsCloning)
                    m_range.Items.Clear();
                if (!m_bIsFieldRangeUpdated
                    && !Document.IsOpening
                    && !Document.IsCloning)
                    UpdateFieldRange();
                return m_range;
            }
        }
        /// <summary>
        /// Gets or sets the field separator.
        /// </summary>
        /// <value>The field separator.</value>
        internal WFieldMark FieldSeparator
        {
            get
            {
                return m_fieldSeparator;
            }
            set
            {
                m_fieldSeparator = value;
            }
        }
        /// <summary>
        /// Gets or sets the field end.
        /// </summary>
        /// <value>The field end.</value>
        internal WFieldMark FieldEnd
        {
            get
            {
                return m_fieldEnd;
            }
            set
            {
                m_fieldEnd = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates WField object for specified document
        /// </summary>
        public WField(IWordDocument doc)
            : base(doc)
        {
            m_paraItemType = ParagraphItemType.Field;
        }
        #endregion

        #region XDLSSerializationBase overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            m_fieldType = (FieldType)reader.ReadEnum(XDLSConstants.FieldTypeAttr, typeof(FieldType));
            m_bConvertedToText = reader.ReadBoolean(XDLSConstants.FieldConvertedAttr);

            if (reader.HasAttribute(XDLSConstants.FormFieldTextFormatAttr))
            {
                m_textFormat = (TextFormat)reader.ReadEnum(XDLSConstants.FormFieldTextFormatAttr, typeof(TextFormat));
            }

            if (reader.HasAttribute(XDLSConstants.FieldIsLocalAttr))
            {
                m_bIsLocal = reader.ReadBoolean(XDLSConstants.FieldIsLocalAttr);
            }

            if (reader.HasAttribute(XDLSConstants.FieldFormattingAttr))
            {
                m_formattingString = reader.ReadString(XDLSConstants.FieldFormattingAttr);
            }
            if (reader.HasAttribute("FieldValue"))
            {
                m_fieldValue = reader.ReadString("FieldValue");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, m_paraItemType);
            writer.WriteValue(XDLSConstants.FieldTypeAttr, FieldType);
            writer.WriteValue(XDLSConstants.FieldConvertedAttr, ConvertedToText);
            writer.WriteValue(XDLSConstants.FormFieldTextFormatAttr, m_textFormat);

            if (m_bIsLocal)
            {
                writer.WriteValue(XDLSConstants.FieldIsLocalAttr, m_bIsLocal);
            }
            if (m_formattingString != string.Empty)
            {
                writer.WriteValue(XDLSConstants.FieldFormattingAttr, m_formattingString);
            }
            if (m_fieldValue != null && m_fieldValue != "")
            {
                writer.WriteValue(XDLSConstants.FieldValueAttr, m_fieldValue);
            }
        }
//#endif
        #endregion

        #region WidgetBase overrides
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new FieldLayoutInfo(ChildrenLayoutDirection.Horizontal);
            if ((FieldSeparator == null
                || FieldSeparator.Owner == null)
                && FieldType != FieldType.FieldSymbol
                && FieldType != FieldType.FieldMergeField
                && FieldType != FieldType.FieldUnknown)
            {
                //Handled to skip rendering of the field codes.
                m_layoutInfo.IsSkip = true;
                for (int i = 0; i < Range.Count; i++)
                {
                    (Range.Items[i] as IWidget).LayoutInfo.IsSkip = true;
                }
                return;
            }
            if (CharacterFormat.Position != 0)
            {
                m_layoutInfo.Margins.Top = -CharacterFormat.Position;
            }

            FieldLayoutInfo lfInfo = m_layoutInfo as FieldLayoutInfo;

            if (FieldType == FieldType.FieldPage)
            {
                lfInfo.FieldType = 1;
            }
            else if (FieldType == FieldType.FieldNumPages)
            {
                lfInfo.FieldType = 2;
            }
            else
            {
                lfInfo.FieldType = 0;
            }

            if (lfInfo.FieldType > 0)
            {
                ParagraphItemCollection paraItems = (Owner is SDTInlineContent) ? (Owner as SDTInlineContent).ParagraphItems : OwnerParagraph.Items;
                int index = paraItems.IndexOf(this);

                if (paraItems.Count > index + 1)
                {
                    WTextRange tr = paraItems[index + 1] as WTextRange;
                    if (tr != null)
                    {
                        tr.Text = "";
                    }
                }
            }
        }
#endif
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldCode"></param>
        virtual internal protected void ParseFieldCode(string fieldCode)
        {
            m_fieldType = FieldTypeDefiner.GetFieldType(fieldCode);
            this.FieldCode = fieldCode;
            UpdateFieldCode(fieldCode);
        }
        /// <summary>
        /// Updates Field code 
        /// </summary>
        /// <param name="fieldCode"></param>
        virtual internal protected void UpdateFieldCode(string fieldCode)
        {
            switch (m_fieldType)
            {
                case FieldType.FieldHyperlink:
                    fieldCode = fieldCode.Trim();
                    Match match = m_hyperlinkRegex.Match(fieldCode);

                    if (match.Groups[2].Value == string.Empty)
                    {
                        m_fieldValue = fieldCode.Replace("HYPERLINK", string.Empty);
                    }
                    else if (match.Groups[2].Value == "\\l")
                    {
                        m_fieldValue = fieldCode.Replace("HYPERLINK", string.Empty);
                        m_fieldValue = m_fieldValue.Replace("\\l", string.Empty);
                    }
                    else
                    {
                        m_fieldValue = "\"" + match.Groups[2].Value + "\"";
                    }

                    int localIndex = fieldCode.IndexOf("\\l");
                    if (match.Groups[1].Length > 0 || localIndex != -1)
                    {
                        m_bIsLocal = true;
                        SetLocalSwitchString();
                        if (fieldCode.IndexOf(m_fieldValue) < localIndex)
                            ParseLocalRef(fieldCode, localIndex);
                    }
                    break;
                case FieldType.FieldIncludePicture:
                    fieldCode = fieldCode.Trim();
                    match = m_includePictureRegex.Match(fieldCode);
                    m_fieldValue = "\"" + match.Groups[1].Value + "\"";
                    m_formattingString = match.Groups[2].Value;
                    break;
                case FieldType.FieldTOC:
                    fieldCode = fieldCode.Trim();
                    match = m_tocRegex.Match(fieldCode);
                    m_formattingString = match.Groups["Options"].Value;
                    break;
                case FieldType.FieldPageRef:
                    fieldCode = fieldCode.Trim();
                    match = m_regex.Match(fieldCode);
                    m_fieldValue = match.Groups[2].Value;
                    break;
                case FieldType.FieldFormula:
                    fieldCode = fieldCode.Trim();
                    fieldCode = fieldCode.Replace("=", string.Empty);
                    m_fieldValue = fieldCode;
                    break;
                case FieldType.FieldLink:
                    fieldCode = fieldCode.Trim();
                    fieldCode = fieldCode.Replace("LINK ", string.Empty);
                    m_fieldValue = fieldCode;
                    break;
                case FieldType.FieldFillIn:
                    fieldCode = fieldCode.Trim();
                    Match march = m_regexFillIn.Match(fieldCode);
                    for (int i = 2, cnt = march.Groups.Count; i < cnt; i++)
                    {
                        if (march.Groups[i].Length > 0)
                        {
                            m_fieldValue += march.Groups[i].Value;
                        }
                    }
                    break;
                default:
                    ParseField(fieldCode);
                    break;
            }
        }
        /// <summary>
        /// Set the TextFormat Switch String
        /// </summary>
        /// <returns></returns>
        private void SetTextFormatSwitchString()
        {
            //Clear prevoius switch string if exists
            m_formattingString = m_formattingString.Replace(@"\* Upper", string.Empty);
            m_formattingString = m_formattingString.Replace(@"\* Lower", string.Empty);
            m_formattingString = m_formattingString.Replace(@"\* FirstCap", string.Empty);
            m_formattingString = m_formattingString.Replace(@"\* Caps", string.Empty);
            //Set new switch string
            switch (m_textFormat)
            {
                case TextFormat.Uppercase:
                    m_formattingString += @"\* Upper ";
                    break;
                case TextFormat.Lowercase:
                    m_formattingString += @"\* Lower ";
                    break;
                case TextFormat.FirstCapital:
                    m_formattingString += @"\* FirstCap ";
                    break;
                case TextFormat.Titlecase:
                    m_formattingString += @"\* Caps ";
                    break;
            }
        }
        /// <summary>
        /// Set the Local Switch String
        /// </summary>
        /// <returns></returns>
        private void SetLocalSwitchString()
        {
            //Clear prevoius switch string if exists
            m_formattingString = m_formattingString.Replace(@" \l", string.Empty);
            //Set new switch string
            if (IsLocal)
            {
                m_formattingString += @" \l";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual protected internal string ConvertSwitchesToString()
        {
            SetTextFormatSwitchString();
            SetLocalSwitchString();
            return m_formattingString;
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemPos">The item pos.</param>
        internal override void Attach(WParagraph owner, int itemPos)
        {
            base.Attach(owner, itemPos);

            if (!DeepDetached)
            {
                Document.Fields.Add(this);
                m_isCloned = false;
            }
            else
            {
                m_isCloned = true;
            }
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            if (FieldEnd != null
                && FieldEnd.OwnerParagraph != null)
            {
                if (OwnerParagraph == FieldEnd.OwnerParagraph)
                {
                    // Removes paragraph items within the field.
                    for (int i = this.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.Items.Count; i++)
                    {
                        Entity ent = OwnerParagraph.Items[i] as Entity;
                        int cnt = OwnerParagraph.Items.Count;
                        OwnerParagraph.Items.Remove(ent);
                        i--;
                        if (ent == FieldEnd)
                            break;
                    }
                }
                else
                {
                    // Removes paragraph items within the field.
                    for (int i = this.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.Items.Count; i++)
                    {
                        int cnt = OwnerParagraph.Items.Count;
                        OwnerParagraph.Items.Remove(OwnerParagraph.Items[i] as Entity);
                        i--;
                    }
                    // Removes textbody items within the field.
                    for (int i = OwnerParagraph.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.OwnerTextBody.Items.Count; i++)
                    {
                        Entity ent = OwnerParagraph.OwnerTextBody.Items[i] as Entity;
                        int cnt = OwnerParagraph.OwnerTextBody.Items.Count;
                        OwnerParagraph.OwnerTextBody.Items.Remove(ent);
                        i--;
                        if (ent == FieldEnd.OwnerParagraph)
                            break;
                    }
                }
            }
            base.Detach();

            if (!DeepDetached)
            {
                Document.Fields.Remove(this);
            }
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        internal override void CloneCommit()
        {
            if (m_isCloned)
            {
                Document.Fields.Add(this);
                m_isCloned = false;
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WField field = (WField)base.CloneImpl();
            field.m_range = new Range(Document, field);
            field.m_bIsFieldRangeUpdated = false;
            field.m_isCloned = true;
            return field;
        }

        /// <summary>
        /// Gets the field as WSymbol item.
        /// </summary>
        /// <returns></returns>
        internal WSymbol GetAsSymbol()
        {
            string[] symbolData = FieldCode.Split('\\');
            string fontName = string.Empty;
            float fontSize = 0;
            WSymbol symbol = new WSymbol(Document);
            symbol.SetOwner(OwnerParagraph);
            foreach (string value in symbolData)
            {
                if (value.ToUpper().StartsWith("SYMBOL"))
                    symbol.CharacterCode = Convert.ToByte(value.ToUpper().Replace("SYMBOL", string.Empty).Trim());
                else if (value.StartsWith("f"))
                {
                    fontName = value.Replace("f", string.Empty).Trim();
                    symbol.FontName = fontName.Trim('\"');
                }
                else if (value.StartsWith("s"))
                    fontSize = (float)Convert.ToDouble(value.Replace("s", string.Empty).Trim());
            }
            symbol.CharacterFormat.ImportContainer(CharacterFormat);
            symbol.CharacterFormat.CopyProperties(CharacterFormat);
            if (CharacterFormat.BaseFormat != null)
                symbol.CharacterFormat.ApplyBase(CharacterFormat.BaseFormat);
            if (fontSize > 0)
                symbol.CharacterFormat.FontSize = fontSize;
            return symbol;
        }
        #endregion

        #region Update field.
        /// <summary>
        /// Update the Field
        /// </summary>
        public void Update()
        {
            m_bIsFieldRangeUpdated = false;
            switch (this.FieldType)
            {
                case FieldType.FieldDate:
                case FieldType.FieldTime:
                    UpdateDateField();
                    break;
                case FieldType.FieldDocVariable:
                    string c_invertedCommas = ((char)34).ToString();
                    string value = m_doc.Variables[this.FieldValue.Replace(c_invertedCommas, string.Empty)];
                    if (value != null)
                        UpdateFieldResult(value);
                    break;
                case FieldType.FieldFormula:
                case FieldType.FieldExpression:
                    UpdateFormulaField();
                    break;
                case FieldType.FieldCompare:
                    UpdateCompareField();
                    break;
                case FieldType.FieldIf:
                    (this as WIfField).UpdateIfField();
                    break;
                case FieldType.FieldDocProperty:
                    UpdateDocPropertyField();
                    break;
                case FieldType.FieldSection:
                    UpdateSectionField();
                    break;
#if !SILVERLIGHT && !WP
                case FieldType.FieldNumPages:
                    UpdateFieldResult(Document.PageCount.ToString());
                    break;
#endif
            }
            if (!Document.UpdatedFields.Contains(this))
                Document.UpdatedFields.Add(this);
        }
        /// <summary>
        /// Update Datefield
        /// </summary>
        private void UpdateDateField()
        {
            if (!m_bIsFieldRangeUpdated
                && Document.IsOpening)
            {
                //Updates Range items, if the Date/Time fields is updated while opening the document.
                Range.Items.Clear();
                UpdateFieldRange();
            }
            DateTime currentDateTime = DateTime.Now;
            bool isMeridiemDefined = false;
            string text = this.FormattingString.Trim().ToString();
            if (text.Contains("\\* MERGEFORMAT"))
                text = text.Remove(text.IndexOf("\\* MERGEFORMAT")).Trim();
            else if (text.Contains("\\* Mergeformat"))
                text = text.Remove(text.IndexOf("\\* Mergeformat")).Trim();
            bool ordinalString = false;
            if (text.ToLower().Contains("\\* ordinal"))
            {
                text = text.Remove(text.ToLower().IndexOf("\\* ordinal")).Trim();
                ordinalString = true;
            }
            if (this.FormattingString.Trim() != string.Empty && text.Contains("\\@"))
            {
                text = ParseSwitches(text, text.IndexOf("\\@"));
                text = RemoveMeridiem(text, out isMeridiemDefined);
                text = UpdateDateValue(text, currentDateTime);
                //Get Ordinal string for numeric value
                text = GetOrdinalstring(ordinalString, text);
                if (isMeridiemDefined)
                    text = UpdateMeridiem(text, currentDateTime);
            }
            else
#if WINRT
                text = currentDateTime.ToString("d");//ToShortDateString();
#else
                text = currentDateTime.ToShortDateString();
#endif
                UpdateFieldResult(text);
        }
        /// <summary>
        /// Get ordinal string for numeric value
        /// </summary>
        /// <param name="ordinalString">Ordinal string</param>
        /// <param name="text">text</param>
        /// <returns></returns>
        private string GetOrdinalstring(bool ordinalString, string text)
        {
            text = text.Trim().ToString();
            //Check whether the text contain alphabet or not
            if (ordinalString)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsLetter(text[i]))
                    {
                        ordinalString = false;
                        break;
                    }
                }
                //Get ordinal string
                if (ordinalString)
                {
                    int ordinalnumber = 0;
                    if (text.Length >= 2 && char.IsNumber(text[text.Length - 1]) && char.IsNumber(text[text.Length - 2]))
                    {
                        ordinalnumber = int.Parse(text.Substring(text.Length - 2, 2));
                        text = text.Remove(text.Length - 2, 2);
                        text += this.Document.GetOrdinal(ordinalnumber, this.CharacterFormat);
                    }
                    else if (text.Length >= 1 && char.IsNumber(text[text.Length - 1]))
                    {
                        ordinalnumber = int.Parse(text.Substring(text.Length - 1));
                        text = text.Remove(text.Length - 1);
                        text += this.Document.GetOrdinal(ordinalnumber,this.CharacterFormat);
                    }
                }
            }
            return text;
        }
        /// <summary>
        /// Updates the next if field.
        /// </summary>
        /// <returns></returns>
        internal bool UpdateNextIfField()
        {
            string fieldCode = RemoveMergeFormat(NestedFieldCode);
            fieldCode = RemoveText(fieldCode, "nextif");
            string text = string.Empty;
            text = UpdateCondition(fieldCode);
            return (text == "1")? true : false;
        }
        /// <summary>
        /// Updates the section field.
        /// </summary>
        private void UpdateSectionField()
        {
            Entity entity = this.OwnerBase as Entity;
            int index = 1;
            while (!(entity is WSection))
            {
                if (entity.Owner == null)
                    break;
                entity = entity.Owner as Entity;
            }
            if (entity != null
                && entity is WSection)
                index += (entity as WSection).GetIndexInOwnerCollection();
            UpdateFieldResult(index.ToString());
        }
        /// <summary>
        /// Updates the doc property field.
        /// </summary>
        private void UpdateDocPropertyField()
        {
            string fieldCode = RemoveMergeFormat(NestedFieldCode);
            fieldCode = RemoveText(fieldCode, "docproperty");
            //Removes the unwanted text in the begining of the field code
            fieldCode = TrimBeginingText(fieldCode);
            //Removes the unwanted text in the end of the field code
            fieldCode = TrimEndText(fieldCode);
            string text = string.Empty;
            string error = "Error! Unknown document property name.";
            switch (fieldCode.ToLower())
            {
                case "author":
                    if (Document.BuiltinDocumentProperties.Author != null)
                        text = Document.BuiltinDocumentProperties.Author;
                    break;
                case "bytes":
                    if (Document.BuiltinDocumentProperties.BytesCount != null)
                        text = Document.BuiltinDocumentProperties.BytesCount.ToString();
                    break;
                case "category":
                    if (Document.BuiltinDocumentProperties.Category != null)
                        text = Document.BuiltinDocumentProperties.Category.ToString();
                    break;
                case "characters":
                case "characterswithspaces":
                    if (Document.BuiltinDocumentProperties.CharCount != null)
                        text = Document.BuiltinDocumentProperties.CharCount.ToString();
                    break;
                case "comments":
                    if (Document.BuiltinDocumentProperties.Comments == null)
                        text = error;
                    else
                        text = Document.BuiltinDocumentProperties.Comments;
                    break;
                case "company":
                    if (Document.BuiltinDocumentProperties.Company != null)
                        text = Document.BuiltinDocumentProperties.Company;
                    break;
                case "createtime":
                    if (Document.BuiltinDocumentProperties.CreateDate != null)
                        text = Document.BuiltinDocumentProperties.CreateDate.ToString("g");
                    break;
                case "keywords":
                    if (Document.BuiltinDocumentProperties.Keywords == null)
                        text = error;
                    else
                        text = Document.BuiltinDocumentProperties.Keywords;
                    break;
                case "lastprinted":
                    if (Document.BuiltinDocumentProperties.LastPrinted != null)
                        text = Document.BuiltinDocumentProperties.LastPrinted.ToString("g");
                    break;
                case "lastsavedby":
                    if (Document.BuiltinDocumentProperties.LastAuthor != null)
                        text = Document.BuiltinDocumentProperties.LastAuthor;
                    break;
                case "lastsavedtime":
                    if (Document.BuiltinDocumentProperties.LastSaveDate != null)
                        text = Document.BuiltinDocumentProperties.LastSaveDate.ToString("g");
                    break;
                case "lines":
                    if (Document.BuiltinDocumentProperties.LinesCount != null)
                        text = Document.BuiltinDocumentProperties.LinesCount.ToString();
                    break;
                case "manager":
                    if (Document.BuiltinDocumentProperties.Manager != null)
                        text = Document.BuiltinDocumentProperties.Manager;
                    break;
                case "nameofapplication":
                    if (Document.BuiltinDocumentProperties.ApplicationName != null)
                        text = Document.BuiltinDocumentProperties.ApplicationName;
                    break;
                case "odmadocid":
                    text = "Error! This property is only valid for ODMA documents.";
                    break;
                case "pages":
                    if (Document.BuiltinDocumentProperties.PageCount != null)
                        text = Document.BuiltinDocumentProperties.PageCount.ToString();
                    break;
                case "paragraphs":
                    if (Document.BuiltinDocumentProperties.ParagraphCount != null)
                        text = Document.BuiltinDocumentProperties.ParagraphCount.ToString();
                    break;
                case "revisionnumber":
                    if (Document.BuiltinDocumentProperties.RevisionNumber != null)
                        text = Document.BuiltinDocumentProperties.RevisionNumber.ToString();
                    break;
                case "security":
                    if (Document.BuiltinDocumentProperties.DocSecurity != null)
                        text = Document.BuiltinDocumentProperties.DocSecurity.ToString();
                    break;
                case "subject":
                    if (Document.BuiltinDocumentProperties.Subject == null)
                        text = error;
                    else
                        text = Document.BuiltinDocumentProperties.Subject;
                    break;
                case "template":
                    if (Document.BuiltinDocumentProperties.Template != null)
                        text = Document.BuiltinDocumentProperties.Template;
                    break;
                case "title":
                    if (Document.BuiltinDocumentProperties.Title == null)
                        text = error;
                    else
                        text = Document.BuiltinDocumentProperties.Title;
                    break;
                case "totaleditingtime":
                    if (Document.BuiltinDocumentProperties.TotalEditingTime.TotalMinutes > 0)
                        text = Document.BuiltinDocumentProperties.TotalEditingTime.TotalMinutes.ToString();
                    break;
                case "words":
                    if (Document.BuiltinDocumentProperties.WordCount != null)
                        text = Document.BuiltinDocumentProperties.WordCount.ToString();
                    break;
                default:
                    if (Document.CustomDocumentProperties.CustomHash.ContainsKey(fieldCode))
                        text = Document.CustomDocumentProperties.CustomHash[fieldCode].Value.ToString();
                    else
                        text = error;
                    break;
            }
            UpdateFieldResult(text);
        }
       /// <summary>
        /// Removes the unwanted text in the begining of the field code
       /// </summary>
       /// <param name="fieldCode"></param>
       /// <returns></returns>
        private string TrimBeginingText(string fieldCode)
        {
            while (fieldCode != string.Empty
                   && (fieldCode.StartsWith(ControlChar.DoubleQuote.ToString())
                   || fieldCode.StartsWith(ControlChar.LeftDoubleQuote.ToString())
                   || fieldCode.StartsWith(ControlChar.RightDoubleQuote.ToString())
                   || fieldCode.StartsWith(ControlChar.DoubleLowQuote.ToString())))
                fieldCode = fieldCode.Remove(0, 1);
            return fieldCode;
        }
        /// <summary>
        /// Removes the unwanted text in the end of the field code
        /// </summary>
        /// <param name="fieldCode"></param>
        /// <returns></returns>
        private string TrimEndText(string fieldCode)
        {
            while (fieldCode != string.Empty
                  && (fieldCode.EndsWith(ControlChar.DoubleQuote.ToString())
                  || fieldCode.EndsWith(ControlChar.LeftDoubleQuote.ToString())
                  || fieldCode.EndsWith(ControlChar.RightDoubleQuote.ToString())
                  || fieldCode.EndsWith(ControlChar.DoubleLowQuote.ToString())))
                fieldCode = fieldCode.Substring(0, fieldCode.Length - 1);
            return fieldCode;
        }
        /// <summary>
        /// Removes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="textToRevome">The text to revome.</param>
        /// <returns></returns>
        protected string RemoveText(string text, string textToRevome)
        {
#if WINRT
            if (text.StartsWith(textToRevome, StringComparison.OrdinalIgnoreCase))
#else
             if (text.StartsWith(textToRevome, StringComparison.InvariantCultureIgnoreCase))
#endif
            text = text.Substring(textToRevome.Length, text.Length - textToRevome.Length).Trim();
            return text;
        }
        /// <summary>
        /// Splits if arguments.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected List<string> SplitIfArguments(string text)
        {
            List<string> arguments = new List<string>();
            List<string> operators = new List<string>(new string[] { "<=", ">=", "<>", "=", "<", ">" });
            string condition = string.Empty;
            bool isOperator = false;
            List<int> operatorIndex = GetOperatorIndex(operators, text);
            try
            {
                while (text != string.Empty)
                {
                    int tableStart = text.IndexOf(TableStartMark);
                    if (text.StartsWith("\""))
                        SplitFieldCode(tableStart, ref text, ref condition);
                    else
                        SplitFieldCode(operators, arguments, operatorIndex, isOperator, ref text, ref condition);
                    if (isOperator
                        && arguments.Count == 0)
                    {
                        arguments.Insert(0, condition);
                        condition = string.Empty;
                        isOperator = false;
                    }
                    else if (arguments.Count > 0)
                    {
                        arguments.Add(condition);
                        condition = string.Empty;
                        if (!text.StartsWith("\"")
                            && text.TrimStart().StartsWith("\""))
                            text = text.TrimStart();
                        arguments.Add(text.Trim('\"'));
                        text = string.Empty;
                    }
                    if (arguments.Count == 0)
                        isOperator = IsOperator(operators, ref text, ref condition);
                }
            }
            catch
            {
                while (arguments.Count < 3)
                {
                    arguments.Add(string.Empty);
                }
            }
            return arguments;
        }
        /// <summary>
        /// Splits the field code.
        /// </summary>
        /// <param name="tableStart">The table start.</param>
        /// <param name="text">The text.</param>
        /// <param name="condition">The condition.</param>
        private void SplitFieldCode(int tableStart, ref string text, ref string condition)
        {
            text = text.Substring(text.IndexOf("\"") + 1);
            tableStart = text.IndexOf(TableStartMark);
            if (tableStart >= 0
                && tableStart < text.IndexOf("\""))
                condition += GetTextInTable(ref text);
            condition += text.Substring(0, text.IndexOf("\"")).Trim(ControlChar.SpaceChar);
            text = text.Substring(text.IndexOf("\"") + 1).Trim(ControlChar.SpaceChar);
        }
        /// <summary>
        /// Splits the field code.
        /// </summary>
        /// <param name="operators">The operators.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="operatorIndex">Index of the operator.</param>
        /// <param name="isOperator">if set to <c>true</c> [is operator].</param>
        /// <param name="text">The text.</param>
        /// <param name="condition">The condition.</param>
        private void SplitFieldCode(List<string> operators, List<string> arguments, List<int> operatorIndex, bool isOperator, ref string text, ref string condition)
        {
            for (int i = 0; i < operators.Count && arguments.Count == 0; i++)
            {
                if (text.Contains(operators[i])
                    && operatorIndex[0] == text.IndexOf(operators[i]))
                {
                    condition += text.Substring(0, text.IndexOf(operators[i])).Trim(ControlChar.SpaceChar);
                    text = text.Substring(text.IndexOf(operators[i])).Trim(ControlChar.SpaceChar);
                    break;
                }
            }
            if (isOperator
                || arguments.Count > 0)
            {
                condition += text.Substring(0, text.IndexOf(" ")).Trim(ControlChar.SpaceChar);
                text = text.Substring(text.IndexOf(" ")).Trim(ControlChar.SpaceChar);
            }
        }
        /// <summary>
        /// Determines whether the specified operators is operator.
        /// </summary>
        /// <param name="operators">The operators.</param>
        /// <param name="text">The text.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// 	<c>true</c> if the specified operators is operator; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOperator(List<string> operators, ref string text, ref string condition)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                if (text.StartsWith(operators[i]))
                {
                    condition += operators[i];
                    text = text.Substring(text.IndexOf(operators[i]) + operators[i].Length).Trim();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the index of the operator.
        /// </summary>
        /// <param name="operators">The operators.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private List<int> GetOperatorIndex(List<string> operators, string text)
        {
            List<int> operatorIndex = new List<int>();
            for (int i = 0; i < operators.Count; i++)
            {
                if (text.Contains(operators[i]))
                {
                    operatorIndex.Add(text.IndexOf(operators[i]));
                }
            }
            operatorIndex.Sort();
            return operatorIndex;
        }
        /// <summary>
        /// Gets the text in table.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string GetTextInTable(ref string text)
        {
            string tableText = string.Empty;
            int tableStart = text.IndexOf(TableStartMark);
            while (tableStart >= 0 && tableStart < text.IndexOf("\""))
            {
                string txt = text.Substring(0, text.IndexOf(TableEndMark) + 1);
                tableText += txt;
                text = text.Substring(text.IndexOf(TableEndMark) + 1);
                string[] splitedText = txt.Split(TableStartMark);
                int nestedCount = splitedText.Length - 2;
                while (nestedCount > 0)
                {
                    tableText += text.Substring(0, text.IndexOf(TableEndMark) + 1);
                    text = text.Substring(text.IndexOf(TableEndMark) + 1);
                }
                tableStart = text.IndexOf(TableStartMark);
            }
            return tableText;
        }
        /// <summary>
        /// Updates the condition.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected string UpdateCondition(string text)
        {
            List<string> operators = new List<string>(new string[]{ "<=", ">=", "<>", "=", "<", ">" });
            string[] operands = text.Split(operators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (operands.Length > 1)
            {
                operands[0] = operands[0].Trim(new char[] { '\"', ' ' });
                operands[1] = operands[1].Trim(new char[] { '\"', ' ' });
            }
            else
            {
                if (operands.Length == 0)
                    operands = new string[]{string.Empty, string.Empty};
                else
                    operands = new string[] { operands[0], string.Empty };
            }
            string result = "1";
            double operand1, operand2;
            if (operands.Length > 1)
            {
                if (!double.TryParse(operands[0], out operand1))
                    try
                    {
                        operands[0] = UpdateFormula(operands[0].Trim());
                    }
                    catch (Exception e)
                    { }
                if (!double.TryParse(operands[1], out operand2))
                    try
                    {
                        operands[1] = UpdateFormula(operands[1].Trim());
                    }
                    catch (Exception e)
                    { }

                if (double.TryParse(operands[0], out operand1)
                    && double.TryParse(operands[1], out operand2))
                {
                    foreach (string op in operators)
                    {
                        if (text.Contains(op))
                        {
                            result = CompareExpression(operand1, operand2, op);
                            break;
                        }
                    }
                }
                else
                {
                    if (text.Contains("="))
                        result = (operands[0].Trim() == operands[1].Trim()) ? "1" : "0";
                    else if (text.Contains("<>"))
                        result = (operands[0].Trim() != operands[1].Trim()) ? "1" : "0";
                }
            }
            return result;
        }
        /// <summary>
        /// Updates the compare field.
        /// </summary>
        private void UpdateCompareField()
        {
            string fieldCode = RemoveMergeFormat(NestedFieldCode);
            fieldCode = RemoveText(fieldCode, "compare");
            string text = string.Empty;
            try
            {
                text = UpdateCondition(fieldCode);
            }
            catch (Exception e)
            {
                text = "Error! Unknown op code for conditional.";
            }
            UpdateFieldResult(text);
        }
        /// <summary>
        /// Compares the expression.
        /// </summary>
        /// <param name="operand1">The operand1.</param>
        /// <param name="operand2">The operand2.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private string CompareExpression(double operand1, double operand2, string operation)
        {
            double value = 0;
            switch (operation)
            {
                case "=":
                    value = (operand1 == operand2) ? 1 : 0;
                    break;
                case "<":
                    value = (operand1 < operand2) ? 1 : 0;
                    break;
                case "<=":
                    value = (operand1 <= operand2) ? 1 : 0;
                    break;
                case ">":
                    value = (operand1 > operand2) ? 1 : 0;
                    break;
                case ">=":
                    value = (operand1 >= operand2) ? 1 : 0;
                    break;
                case "<>":
                    value = (operand1 != operand2) ? 1 : 0;
                    break;
            }
            return value.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Updates the formula field.
        /// </summary>
        private void UpdateFormulaField()
        {
            string fieldCode = RemoveMergeFormat(NestedFieldCode);
            string numberFormat = string.Empty;
            if (fieldCode.Contains("\\#"))
            {
                numberFormat = fieldCode.Substring(fieldCode.IndexOf("\\#"));
                numberFormat = numberFormat.Substring(numberFormat.IndexOf("\"") + 1);
                numberFormat = numberFormat.Remove(numberFormat.LastIndexOf("\"")).Trim();
                fieldCode = fieldCode.Remove(fieldCode.IndexOf("\\#")).Trim();
            }
            if (fieldCode.StartsWith("="))
                fieldCode = fieldCode.Substring(1).Trim();
            string fieldResult = string.Empty;
            try
            {
                fieldResult = UpdateFormula(fieldCode);
            }
            catch (Exception e)
            {
                fieldResult = e.Message;
            }
            // Convert the result to specified format.
            fieldResult = UpdateNumberFormat(fieldResult, numberFormat);
            
            UpdateFieldResult(fieldResult);
        }
        /// <summary>
        /// Updates the number format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="numberFormat">The number format.</param>
        /// <returns></returns>
        private string UpdateNumberFormat(string text, string numberFormat)
        {
            double value;
            if (double.TryParse(text, out value))
            {
                string format = numberFormat.TrimEnd('%');
                format = format.Replace("#", "0");
                text = value.ToString(format, CultureInfo.InvariantCulture);
                int count = numberFormat.Length;
                if (numberFormat.Contains("."))
                    count = numberFormat.IndexOf(".");
                int index = numberFormat.IndexOf("#");
                if (text.StartsWith(OPENPARENTHESIS.ToString()))
                    index++;
                if (index < 0)
                    index = 0;
                for (int i = index; i < count; i++)
                {
                    if (char.IsNumber(text[i])
                        && text[i] != '0')
                        break;
                    else if (numberFormat[i] != '0') 
                    {
                        if (text[i] == '0')
                        {
                            text = text.Remove(i, 1);
                            text = text.Insert(i, " ");
                        }
                        else
                        {
                            text = text.Remove(i, 1);
                            numberFormat = numberFormat.Remove(i, 1);
                            i--;
                            count--;
                        }
                    }
                }

                if (numberFormat.Contains("%"))
                    text += "%";
            }
            return text;
        }
        /// <summary>
        /// Removes the merge format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected string RemoveMergeFormat(string text)
        {
            if (text.Contains("\\* MERGEFORMAT"))
                text = text.Remove(text.IndexOf("\\* MERGEFORMAT")).Trim();
            else if (text.Contains("\\* Mergeformat"))
                text = text.Remove(text.IndexOf("\\* Mergeformat")).Trim();
            return text.Trim();
        }
        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        /// <returns></returns>
        private string UpdateFormula(string fieldCode)
        {
            double result;
            if (IsFunction(fieldCode.ToLower()))
            {
                result = UpdateFunction(fieldCode);
            }
            else if (IsExpression(fieldCode))
            {
                result = UpdateExpression(fieldCode);
            }
            else
            {
                Bookmark bookmark = Document.Bookmarks.FindByName(fieldCode);
                if (bookmark != null)
                {
                    string bkText = string.Empty;
                    if(bookmark.BookmarkStart.OwnerParagraph == bookmark.BookmarkEnd.OwnerParagraph)
                        bkText = bookmark.BookmarkStart.OwnerParagraph.Text.Substring(bookmark.BookmarkStart.EndPos, bookmark.BookmarkEnd.EndPos - bookmark.BookmarkStart.EndPos);
                    double.TryParse(bkText, out result);
                }
                else
                    throw new Exception("!Undefined Bookmark, " + fieldCode.ToUpper());
            }
            return result.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Updates the function.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        /// <returns></returns>
        private double UpdateFunction(string fieldCode)
        {
            double result;
            string operation = fieldCode.ToLower();
            if (fieldCode.Contains(OPENPARENTHESIS.ToString()))
                operation = fieldCode.Remove(fieldCode.IndexOf(OPENPARENTHESIS)).ToLower().Trim();
            List<double> operands = new List<double>();
            if (fieldCode.IndexOf(OPENPARENTHESIS) + 1 < fieldCode.LastIndexOf(CLOSEPARENTHESIS))
                operands = SplitOperands(fieldCode.Substring(fieldCode.IndexOf(OPENPARENTHESIS) + 1, fieldCode.LastIndexOf(CLOSEPARENTHESIS) - fieldCode.IndexOf(OPENPARENTHESIS) - 1));
            else if(!(operation == "true" 
                || operation == "false"))
                throw new Exception("!Syntax Error, " + CLOSEPARENTHESIS);
            switch (operation)
            {
                case "product":
                    result = Product(operands);
                    break;
                case "sum":
                    result = Sum(operands);
                    break;
                case "average":
                    result = Average(operands);
                    break;
                case "mod":
                    result = Modulo(operands[0], operands[1]);
                    break;
                case "abs":
                    result = Absolute(operands[0]);
                    break;
                case "int":
                    result = Int(operands[0]);
                    break;
                case "round":
                    result = RoundOf(operands[0], (int)operands[1]);
                    break;
                case "sign":
                    result = Sign(operands[0]);
                    break;
                case "count":
                    result = Count(operands);
                    break;
                case "defined":
                    result = Defined(operands[0].ToString(CultureInfo.InvariantCulture));
                    break;
                case "or":
                    result = Or((int)operands[0], (int)operands[1]);
                    break;
                case "and":
                    result = And((int)operands[0], (int)operands[1]);
                    break;
                case "not":
                    result = Not((int)operands[0]);
                    break;
                case "max":
                    result = Maximum(operands);
                    break;
                case "min":
                    result = Minimum(operands);
                    break;
                case "true":
                    result = 1;
                    break;
                case "false":
                    result = 0;
                    break;
                case "if":
                    result = (operands[0] == 1) ? operands[1] : operands[2];
                    break;
                default:
                    throw new NotSupportedException("The operation" + operation + "is not supported.");
                    break;
            }
            return result;
        }

        #region Math operations
        /// <summary>
        /// Calulates the product of specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Product(List<double> operands)
        {
            double result = 1;
            for (int i = 0; i < operands.Count; i++)
            {
                result *= operands[i];
            }
            return result;
        }
        /// <summary>
        /// Calulates the sum of specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Sum(List<double> operands)
        {
            double result = 0;
            for (int i = 0; i < operands.Count; i++)
            {
                result += operands[i];
            }
            return result;
        }
        /// <summary>
        /// Calulates the average of specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Average(List<double> operands)
        {
            return Sum(operands) / operands.Count;
        }
        /// <summary>
        /// Calulates the modulo of a to b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double Modulo(double a, double b)
        {
            return a % b;
        }
        /// <summary>
        /// Calulates the absolute of specified operand.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private double Absolute(double operand)
        {
            return Math.Abs(operand);
        }
        /// <summary>
        /// Rounds of the operand to specified decimal point.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="decimalPoint">The decimal point.</param>
        /// <returns></returns>
        private double RoundOf(double operand, int decimalPoint)
        {
            return Math.Round(operand, decimalPoint);
        }
        /// <summary>
        /// Counts the specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Count(List<double> operands)
        {
            return operands.Count - 1;
        }
        /// <summary>
        /// Sign of specified operand.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private double Sign(double operand)
        {
            if (operand < 0)
                return 0;
            return 1;
        }
        /// <summary>
        /// Calulates the minimum of specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Minimum(List<double> operands)
        {
            operands.Sort();
            return operands[0];
        }
        /// <summary>
        /// Calulates the maximum of specified operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns></returns>
        private double Maximum(List<double> operands)
        {
            operands.Sort();
            return operands[operands.Count - 1];
        }
        /// <summary>
        /// Calulates the integer value of specified operand.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private double Int(double operand)
        {
            return Math.Floor(operand);
        }
        /// <summary>
        /// Checks whether the specified operand is numeric value.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private double Defined(string operand)
        {
            double value;
            if (double.TryParse(operand, out value))
                return 1;
            return 0;
        }
        /// <summary>
        /// Calulates the OR of a and b.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double Or(int a, int b)
        {
            if ((a | b) == 0)
                return 0;
            return 1;
        }
        /// <summary>
        /// Calulates the AND of a and b.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double And(int a, int b)
        {
            if ((a & b) == 0)
                return 0;
            return 1;
        }
        /// <summary>
        /// Calulates the NOT of specified operand.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <returns></returns>
        private double Not(int operand)
        {
            if (operand == 0)
                return 1;
            return 0;
        }
        #endregion

        /// <summary>
        /// Determines whether the specified text is function.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is function; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFunction(string text)
        {
            List<string> functions = new List<string>(new string[]{ "product", "sum", "average", "mod", "abs", "int", "round", "sign", "count", "defined", "or", "and", "not", "max", "min", "true", "false", "if" });
            bool isFunction = false;
            foreach (string fn in functions)
            {
                if (text.StartsWith(fn))
                {
                    isFunction = true;
                    break;
                }
            }
            return isFunction;
        }
        /// <summary>
        /// Determines whether the specified text is expression.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is expression; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExpression(string text)
        {
            List<string> operators = new List<string>(new string[]{"+", "-", "*", "/", "%", "^", "=", "<", "<=", ">", ">=", "<>" });
            bool isExpression = false;
            foreach(string op in operators)
            {
                if (text.Contains(op))
                {
                    isExpression = true;
                    break;
                }
            }
            return isExpression;
        }
        /// <summary>
        /// Updates the expression.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private double UpdateExpression(string text)
        {
            double result = 0;
            List<string> operators = new List<string>(new string[]{ "=", "<", "<=", ">", ">=", "<>", "^", "%", "/", "*", "-", "+"});
            List<string> expression = SplitExpression(text, operators);
            foreach (string operation in operators)
            {
                EvaluateExpression(ref expression, operation);
            }
            result = double.Parse(expression[0], CultureInfo.InvariantCulture);
            return result;
        }
        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="operation">The operation.</param>
        private void EvaluateExpression(ref List<string> expression, string operation)
        {
            while (expression.Contains(operation))
            {
                int index = expression.LastIndexOf(operation);
                double value = 0;
                switch (operation)
                {
                    case "+":
                        value = double.Parse(expression[index - 1], CultureInfo.InvariantCulture) + double.Parse(expression[index + 1], CultureInfo.InvariantCulture);
                        break;
                    case "-":
                        value = double.Parse(expression[index - 1], CultureInfo.InvariantCulture) - double.Parse(expression[index + 1], CultureInfo.InvariantCulture);
                        break;
                    case "*":
                        value = double.Parse(expression[index - 1], CultureInfo.InvariantCulture) * double.Parse(expression[index + 1], CultureInfo.InvariantCulture);
                        break;
                    case "/":
                        value = double.Parse(expression[index - 1], CultureInfo.InvariantCulture) / double.Parse(expression[index + 1], CultureInfo.InvariantCulture);
                        break;
                    case "%":
                        value = double.Parse(expression[index - 1], CultureInfo.InvariantCulture) % double.Parse(expression[index + 1], CultureInfo.InvariantCulture);
                        break;
                    case "^":
                        value = Math.Pow(double.Parse(expression[index - 1], CultureInfo.InvariantCulture), double.Parse(expression[index + 1], CultureInfo.InvariantCulture));
                        break;
                    case "=":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) == double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                    case "<":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) < double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                    case "<=":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) <= double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                    case ">":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) > double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                    case ">=":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) >= double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                    case "<>":
                        value = (double.Parse(expression[index - 1], CultureInfo.InvariantCulture) != double.Parse(expression[index + 1], CultureInfo.InvariantCulture)) ? 1 : 0;
                        break;
                }
                expression.RemoveAt(index + 1);
                expression.RemoveAt(index);
                expression.RemoveAt(index - 1);
                expression.Insert(index - 1, value.ToString());
            }
        }
        /// <summary>
        /// Splits the expression.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="operators">The operators.</param>
        /// <returns></returns>
        private List<string> SplitExpression(string text, List<string> operators)
        {
            List<string> expression = new List<string>();
            int openParenthesisCount = 0;
            string operand = string.Empty;
            string operation = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                operation = string.Empty;
                if (operators.Contains(text[i].ToString()))
                {
                    operation = text[i].ToString();
                    if (i != text.Length - 1
                        && (text[i + 1].ToString() == "="
                        || text[i + 1].ToString() == ">"))
                        operation += text[i++].ToString();
                }
                if (i == text.Length - 1
                    && text[i] != CLOSEPARENTHESIS)
                    operand += text[i];
                if (text[i] == OPENPARENTHESIS)
                    openParenthesisCount++;
                else if (text[i] == CLOSEPARENTHESIS)
                    openParenthesisCount--;
                if (operators.Contains(operation)
                    && openParenthesisCount == 0
                    || i == text.Length - 1)
                {
                    operand = operand.Trim();
                    double value;
                    if (double.TryParse(operand, out value))
                    {
                        expression.Add(value.ToString(CultureInfo.InvariantCulture));
                        operand = string.Empty;
                    }
                    else
                    {
                        operand = UpdateFormula(operand);
                        if (double.TryParse(operand, out value))
                        {
                            expression.Add(value.ToString(CultureInfo.InvariantCulture));
                            operand = string.Empty;
                        }
                        else
                        {
                            // To handle the exceptional cases.
                            throw new Exception("!Syntax Error, " + operand);
                        }
                    }
                    if (i != text.Length - 1
                        && operators.Contains(operation))
                    {
                        expression.Add(operation.Trim());
                    }
                }
                else if ((text[i] == OPENPARENTHESIS
                        && openParenthesisCount > 1)
                        || (text[i] == CLOSEPARENTHESIS
                        && openParenthesisCount > 0)
                        || (text[i] != OPENPARENTHESIS
                        && text[i] != CLOSEPARENTHESIS))
                    operand += text[i];
            }
            return expression;
        }
        /// <summary>
        /// Splits the operands.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private List<double> SplitOperands(string text)
        {
            List<double> operands = new List<double>();
            int openParenthesisCount = 0;
            string operand = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (i == text.Length - 1)
                    operand += text[i];
                if (text[i] == OPENPARENTHESIS)
                    openParenthesisCount++;
                else if(text[i] == CLOSEPARENTHESIS)
                    openParenthesisCount--;
                if (text[i] == COMMASEPARATOR
                    && openParenthesisCount == 0
                    || i == text.Length - 1)
                {
                    operand = operand.Trim();
                    double value;
                    if (double.TryParse(operand, out value))
                    {
                        operands.Add(value);
                        operand = string.Empty;
                    }
                    else
                    {
                        operand = UpdateFormula(operand);
                        if (double.TryParse(operand, out value))
                        {
                            operands.Add(value);
                            operand = string.Empty;
                        }
                        else
                        {
                            // To handle the exceptional cases.
                            throw new Exception("!Syntax Error, " + operand);
                        }
                    }
                }
                else
                    operand += text[i];
            }
            return operands;
        }
        #endregion

        #region Update nested field code.
        /// <summary>
        /// Updates the nested field code.
        /// </summary>
        private void UpdateFieldRange()
        {
            try
            {
                if (OwnerParagraph == FieldEnd.OwnerParagraph)
                {
                    // Updates paragraph items within the field.
                    for (int i = this.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.Items.Count; i++)
                    {
                        m_range.Items.Add(OwnerParagraph.Items[i] as Entity);
                        if (OwnerParagraph.Items[i] == FieldEnd)
                            break;
                    }
                }
                else
                {
                    // Updates paragraph items within the field.
                    for (int i = this.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.Items.Count; i++)
                    {
                        m_range.Items.Add(OwnerParagraph.Items[i] as Entity);
                    }
                    // Updates textbody items within the field.
                    for (int i = OwnerParagraph.GetIndexInOwnerCollection() + 1; i < OwnerParagraph.OwnerTextBody.Items.Count; i++)
                    {
                        m_range.Items.Add(OwnerParagraph.OwnerTextBody.Items[i] as Entity);
                        if (OwnerParagraph.OwnerTextBody.Items[i] == FieldEnd.OwnerParagraph)
                            break;
                    }
                }
                m_bIsFieldRangeUpdated = true;
            }
            catch
            {
                m_range.Items.Clear();
            }
        }
        #endregion

        #region Update nested field code.
        /// <summary>
        /// Updates the nested field code.
        /// </summary>
        private string UpdateNestedFieldCode()
        {
            string code = FieldCode;
            m_bIsFieldSeparator = false;
            m_bIsSkip = false;
            m_nestedFields.Clear();
            for (int i = 0; i < Range.Items.Count; i++)
            {
                Entity entity = Range.Items[i] as Entity;
                if (entity is ParagraphItem)
                    code += UpdateTextForParagraphItem(entity);
                else
                    code += UpdateTextForTextBodyItem(entity);
                if (m_bIsFieldSeparator)
                    break;
            }
            m_bIsFieldSeparator = false;
            m_bIsSkip = false;
            m_nestedFields.Clear();
            return code;
        }
        /// <summary>
        /// Updates the text for text body item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected string UpdateTextForTextBodyItem(Entity entity)
        {
            string text = string.Empty;
            if (entity is WParagraph)
            {
                for (int i = 0; i < (entity as WParagraph).Items.Count; i++)
                {
                    text += UpdateTextForParagraphItem((entity as WParagraph).Items[i]);
                    if (m_bIsFieldSeparator)
                        return text;
                }
                if (!m_bIsSkip)
                    text += PARAGRAPHMARK;
            }
            else if (entity is WTable
                && !m_bIsSkip)
            {
                text += TableStartMark + UpdateTextForTable(entity) + TableEndMark;
            }
            return text;
        }
        /// <summary>
        /// Updates the text for table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private string UpdateTextForTable(Entity entity)
        {
            string text = string.Empty;
            for (int i = 0; i < (entity as WTable).Rows.Count; i++)
            {
                WTableRow row = (entity as WTable).Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    WTableCell cell = row.Cells[j];
                    for (int k = 0; k < cell.Items.Count; k++)
                    {
                        text += UpdateTextForTextBodyItem(cell.Items[k]);
                        if (m_bIsFieldSeparator)
                            return text;
                    }
                    if (!m_bIsSkip)
                        text += CELLMARK;
                }
                if (!m_bIsSkip)
                    text += ROWMARK;
            }
            return text;
        }
        /// <summary>
        /// Updates the text for paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected string UpdateTextForParagraphItem(Entity entity)
        {
            string text = string.Empty;
            if (m_bIsFieldSeparator)
                return text;
            if (entity is WField
                && !m_bIsSkip)
            {
                if ((entity as WField).FieldType == FieldType.FieldMergeField)
                    text = (entity as WTextRange).Text;
                else
                {
                    if ((entity as WField).FieldEnd != null)
                    {
                        m_nestedFields.Push(entity as WField);
                        m_bIsSkip = true;
                    }
                    if (!Document.UpdatedFields.Contains(entity as WField))
                        (entity as WField).Update();
                    text = (entity as WField).FieldResult;
                }
            }
            else if (FieldSeparator == entity)
                m_bIsFieldSeparator = true;
            else if (m_bIsSkip
                && m_nestedFields.Peek().FieldEnd == entity)
            {
                m_bIsSkip = false; 
                m_nestedFields.Pop();
            }
            else if (entity is WTextRange
                && !m_bIsSkip)
                text = (entity as WTextRange).Text;
            return text;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the field result.
        /// </summary>
        /// <returns></returns>
        internal string GetFieldResult()
        {
            //Retrieves the text contents, present between field separator and field end mark.
            string text = string.Empty;
            if (FieldSeparator != null
                && FieldSeparator.OwnerParagraph != null
                && FieldEnd != null
                && FieldEnd.OwnerParagraph != null)
            {
                int index = 0;
                if (Range.Items.Count == 0 && !m_bIsFieldRangeUpdated)
                    UpdateFieldRange();
                if (FieldSeparator.OwnerParagraph == OwnerParagraph)
                    index = Range.Items.IndexOf(FieldSeparator) + 1;
                else
                    index = Range.Items.IndexOf(FieldSeparator.OwnerParagraph);
                for (int i = index; i < Range.Items.Count; i++)
                {
                    Entity entity = Range.Items[i] as Entity;
                    if (entity is WParagraph)
                    {
                        int start = 0, end = (entity as WParagraph).Items.Count - 1;
                        if (FieldSeparator.OwnerParagraph == entity)
                            start = FieldSeparator.GetIndexInOwnerCollection() + 1;
                        if (FieldEnd.OwnerParagraph == entity)
                            end = FieldEnd.GetIndexInOwnerCollection() - 1;
                        text += (entity as WParagraph).GetText(start, end);
                        if (FieldEnd.OwnerParagraph != entity)
                            text += ControlChar.ParagraphBreak;
                        if (end < (entity as WParagraph).Items.Count - 1)
                        {
                            text += (entity as WParagraph).GetText(end + 1, (entity as WParagraph).Items.Count - 1) + ControlChar.ParagraphBreak;
                            Document.m_prevClonedEntity = FieldEnd.OwnerParagraph;
                        }
                    }
                    else if (entity is WTable)
                        text += (entity as WTable).GetTableText();
                    else
                    {
                        if (entity is WMergeField)
                            text += (entity as WTextRange).Text;
                        else if (entity is WTextRange)
                            text += (entity as WTextRange).Text;
                        else if (entity is Break)
                            text += ControlChar.ParagraphBreak;
                    }
                }
            }
            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldValue"></param>
        private void ParseFieldValue(string fieldValue)
        {
            Match match1 = m_regex.Match(fieldValue.Trim());
            if (match1.Groups[2].Length == 0)
            {
                //m_prefix = "";
                m_fieldValue = match1.Groups[1].Value;
            }
            else
            {
                //m_prefix = match1.Groups[ 1 ].Value;
                m_fieldValue = match1.Groups[2].Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ClearStringFromOtherCharacters(string value)
        {
            string str = value.Remove(0, 1);
            //str = str.Trim();
            char[] chArr = new char[1] { '"' };
            return str.Trim(chArr);
        }
        /// <summary>
        /// Parses the field.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        protected void ParseField(string fieldCode)
        {
            char[] separator = new char[1] { '\\' };
            string[] fieldValues = fieldCode.Split(separator);
            ParseFieldValue(fieldValues[0]);
            ParseFieldFormat(fieldValues);
        }
        /// <summary>
        /// Parses the field format.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        protected void ParseFieldFormat(string[] fieldValues)
        {
            for (int i = 1; i < fieldValues.Length; i++)
            {
                string clearedValue;
                string currValue = fieldValues[i];
                if (currValue.Length > 0)
                {
                    clearedValue = ClearStringFromOtherCharacters(currValue);
                    char firstChar = currValue[0];
                    switch (firstChar)
                    {
                        case '*':
                            if (clearedValue == "Upper")
                            {
                                m_textFormat = TextFormat.Uppercase;
                            }
                            else if (clearedValue == "Lower")
                            {
                                m_textFormat = TextFormat.Lowercase;
                            }
                            else if (clearedValue == "Caps")
                            {
                                m_textFormat = TextFormat.Titlecase;
                            }
                            else if (clearedValue == "FirstCap")
                            {
                                m_textFormat = TextFormat.FirstCapital;
                            }
                            else
                            {
                                m_formattingString += " \\" + currValue;
                            }
                            break;
                        case '@':
                        case '\\':
                        default:
                            m_formattingString += " \\" + currValue;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Parses the local reference string for the hyperlink.
        /// </summary>
        /// <param name="fieldCode">The field code.</param>
        /// <param name="startPos">The start pos.</param>
        private void ParseLocalRef(string fieldCode, int startPos)
        {
            startPos += 2;
            if (fieldCode.Length > startPos)
            {
                string strToParse = fieldCode.Substring(startPos, fieldCode.Length - startPos).Trim();

                int bkmkNameStart = strToParse.IndexOf("\"");
                if (bkmkNameStart == -1)
                    return;

                int bkmkNameEnd = strToParse.IndexOf("\"", bkmkNameStart + 1);
                if (bkmkNameEnd == -1)
                    return;

                m_localRef = strToParse.Substring(bkmkNameStart, bkmkNameEnd + 1 - bkmkNameStart);
            }
        }

        /// <summary>
        /// Sets the modified.
        /// </summary>
        private void SetModified()
        {
            if (!this.Document.IsOpening)
            {
                m_fieldCode = string.Empty;
            }
        }
        /// <summary>
        /// Parse the switches
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="index">Index</param>
        /// <returns></returns>
        private string ParseSwitches(string text, int index)
        {
            text = text.Remove(0, index + 2).Trim();
            if (text.StartsWith("\"") && text.EndsWith("\""))
            {
                text = text.Remove(0, text.IndexOf("\"") + 1);
                text = text.Remove(text.LastIndexOf("\""));
            }
            return text;
        }

        #region UpdateDateField
        /// <summary>
        /// Removes the Meridiem if present
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="isMeridiumDefined">Is Meridiem defined</param>
        /// <returns></returns>
        private string RemoveMeridiem(string text, out bool isMeridiemDefined)
        {
            isMeridiemDefined = false;
            if (text.Contains("am/pm"))
            {
                text = text.Remove(text.IndexOf("am/pm"), text.Length - text.IndexOf("am/pm"));
                isMeridiemDefined = true;
            }
            if (text.Contains("AM/PM"))
            {
                text = text.Remove(text.IndexOf("AM/PM"), text.Length - text.IndexOf("AM/PM"));
                isMeridiemDefined = true;
            }
            return text;
        }
        /// <summary>
        /// Updates the Meridiem
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <returns></returns>
        private string UpdateMeridiem(string text, DateTime currentDateTime)
        {
            if (currentDateTime.ToString().Contains("AM"))
            {
                text += "AM";
            }
            else
            {
                text += "PM";
            }
            return text;
        }
        /// <summary>
        /// Updates the date value
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="currentDateTime">Current date Time</param>
        /// <returns>Updated date value</returns>
        private string UpdateDateValue(string text, DateTime currentDateTime)
        {
            int count = 0;
            string dateValue = string.Empty;
            for (int i = 0; i < text.Length; )
            {
                switch (text[i])
                {
                    //Day
                    case 'D':
                    case 'd':
                        while (i < text.Length && (text[i] == 'd' || text[i] == 'D'))
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateDay(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    //Month
                    case 'M':
                        while (i < text.Length && text[i] == 'M')
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateMonth(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    //Year
                    case 'Y':
                    case 'y':
                        while (i < text.Length && (text[i] == 'y' || text[i] == 'Y'))
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateYear(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    //Hour
                    case 'h':
                    case 'H':
                        while (i < text.Length && (text[i] == 'h' || text[i] == 'H'))
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateHour(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    //Minutes
                    case 'm':
                        while (i < text.Length && text[i] == 'm')
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateMinute(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    //Seconds
                    case 's':
                    case 'S':
                        while (i < text.Length && (text[i] == 's'||text[i]=='S'))
                        {
                            i++;
                            count++;
                        }
                        dateValue = UpdateSecond(dateValue, currentDateTime, count);
                        count = 0;
                        break;
                    case '\'':
                    case '\\':
                        i++;
                        //Ignore 
                        break;
                    default:
                        dateValue += text[i];
                        i++;
                        break;
                }
            }
            return dateValue;
        }
        /// <summary>
        /// Updates Day
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateDay(string dateValue, DateTime currentDateTime, int count)
        {
            string dayString = string.Empty;                
            switch (count)
            {
                case 1:
                    dayString = currentDateTime.Day.ToString();
                    break;
                case 2:
                    if (Convert.ToInt16(currentDateTime.Day.ToString()) < 10)
                        dayString ="0" + currentDateTime.Day.ToString();
                    else
                        dayString =currentDateTime.Day.ToString();
                    break;
                case 3:
                    dayString = currentDateTime.DayOfWeek.ToString().Substring(0, 3);
                    break;
                default:
                    string langASCII = GetCultureName((LocaleIDs)this.CharacterFormat.LocaleIdASCII);
                    CultureInfo culture = new CultureInfo(langASCII);
                    dayString = culture.DateTimeFormat.DayNames[(int)currentDateTime.DayOfWeek];
                    break;
            }
            dateValue += dayString;
            return dateValue;
        }

        /// <summary>
        /// Gets the name of the culture.
        /// </summary>
        /// <param name="localID">The local ID.</param>
        /// <returns></returns>
        private string GetCultureName(LocaleIDs localID)
        {

            switch (localID)
            {
                case LocaleIDs.es_ES_tradnl :
                    return "es-ES_tradnl";//replce the first underscore only for spanish traditional 
                default :
                    return localID.ToString().Replace('_', '-');//replace all underscore to hypen for others
            }
        }
        /// <summary>
        /// Updates Month
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateMonth(string dateValue, DateTime currentDateTime, int count)
        {
            string monthString = string.Empty;
                  
            switch (count)
            {
                case 1:
                    monthString = currentDateTime.Month.ToString();
                    break;
                case 2:
                    if (Convert.ToInt16(currentDateTime.Month.ToString()) < 10)
                        monthString = "0" + currentDateTime.Month.ToString();
                    else
                        monthString =  currentDateTime.Month.ToString();
                    break;
                case 3:
                    monthString =  Enum.GetName(typeof(Month), Convert.ToInt32(currentDateTime.Month.ToString())).Substring(0, 3);
                    break;
                case 4:
                    monthString =  Enum.GetName(typeof(Month), Convert.ToInt32(currentDateTime.Month.ToString()));
                    break;
                default:
                    monthString =  Enum.GetName(typeof(Month), Convert.ToInt32(currentDateTime.Month.ToString()));
                    break;
            }
            dateValue += monthString;
            return dateValue;
        }
        /// <summary>
        /// Updates year
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateYear(string dateValue, DateTime currentDateTime, int count)
        {
            string yearString = string.Empty;
                        
            switch (count)
            {
                case 1:
                    yearString = currentDateTime.Year.ToString().Remove(0, 2);
                    break;
                case 2:
                    yearString = currentDateTime.Year.ToString().Remove(0, 2);
                    break;
                case 4:
                    yearString =  currentDateTime.Year.ToString();
                    break;
                default:
                    yearString =  currentDateTime.Year.ToString();
                    break;
            }
            dateValue += yearString;
            return dateValue;
        }
        /// <summary>
        /// Updates hour
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateHour(string dateValue, DateTime currentDateTime, int count)
        {
            string hourString = string.Empty;
                        
            switch (count)
            {
                case 1:
                    hourString =  currentDateTime.Hour.ToString();
                    break;
                case 2:
                    if (Convert.ToInt16(currentDateTime.Hour.ToString()) < 10)
                        hourString =  "0" + currentDateTime.Hour.ToString();
                    else
                        hourString =  currentDateTime.Hour.ToString();
                    break;
                default:
                    hourString =  currentDateTime.Hour.ToString();
                    break;
            }
            dateValue += hourString;
            return dateValue;
        }
        /// <summary>
        /// Updates Minute
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateMinute(string dateValue, DateTime currentDateTime, int count)
        {
            string minuteString = string.Empty;
                        
            switch (count)
            {
                case 1:
                    minuteString = currentDateTime.Minute.ToString();
                    break;
                case 2:
                    if (Convert.ToInt16(currentDateTime.Minute.ToString()) < 10)
                        minuteString = "0" + currentDateTime.Minute.ToString();
                    else
                        minuteString =  currentDateTime.Minute.ToString();
                    break;
                default:
                    minuteString =  currentDateTime.Minute.ToString();
                    break;
            }
            dateValue += minuteString;
            return dateValue;
        }
        /// <summary>
        /// Updates Seconds
        /// </summary>
        /// <param name="dateValue">Date value</param>
        /// <param name="currentDateTime">Current date time</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        private string UpdateSecond(string dateValue, DateTime currentDateTime, int count)
        {
            string secondString = string.Empty;
                        
            switch (count)
            {
                case 1:
                    secondString =  currentDateTime.Second.ToString();
                    break;
                case 2:
                    if (Convert.ToInt16(currentDateTime.Second.ToString()) < 10)
                        secondString =  "0" + currentDateTime.Second.ToString();
                    else
                        secondString =  currentDateTime.Second.ToString();
                    break;
                default:
                    secondString = currentDateTime.Second.ToString();
                    break;
            }
            dateValue += secondString;
            return dateValue;
        }
        #endregion
        #endregion

        #region Update field result
        /// <summary>
        /// Updates if field result.
        /// </summary>
        private void UpdateIfFieldResult(string text)
        {
            //Updates the field result with multiple text body items.
            int index = Range.Items.Count > 0 ? Range.Items.Count - 1 : 0;
            WParagraph lastPara = Range.Items[index] as WParagraph;
            for (int i = 0; i < m_itemsToUpdate.Count; i++)
            {
                Entity entity = m_itemsToUpdate[i];
                if (entity is TextBodyItem)
                {
                    if (entity is WParagraph
                        && i == m_itemsToUpdate.Count - 1
                        && !text.EndsWith(PARAGRAPHMARK))
                    {
                        for (int j = 0; j < (entity as WParagraph).Items.Count; j++)
                        {
                            lastPara.Items.Insert(j, (entity as WParagraph).Items[j].Clone());
                        }
                    }
                    else if (entity is WParagraph
                        && i == 0
                        && !text.StartsWith(PARAGRAPHMARK))
                    {
                        for (int j = 0; j < (entity as WParagraph).Items.Count; j++)
                        {
                            FieldSeparator.OwnerParagraph.Items.Add((entity as WParagraph).Items[j].Clone());
                        }
                    }
                    else
                    {
                        WTextBody textBody = lastPara.OwnerTextBody;
                        textBody.Items.Insert(lastPara.GetIndexInOwnerCollection(), entity);
                    }
                }
                else if (entity is ParagraphItem)
                    FieldSeparator.OwnerParagraph.Items.Add(entity);
            }
        }
        /// <summary>
        /// Checks the field separator.
        /// </summary>
        protected void CheckFieldSeparator()
        {
            //Inserts field separator mark.
            if (FieldSeparator == null)
            {
                WFieldMark fieldMark = new WFieldMark(Document, FieldMarkType.FieldSeparator);
                if (OwnerParagraph == FieldEnd.OwnerParagraph)
                {
                    OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), fieldMark);
                }
                else
                {
                    WParagraph para = FieldEnd.OwnerParagraph.Clone() as WParagraph;
                    para.ClearItems();
                    int paraIndex = FieldEnd.OwnerParagraph.GetIndexInOwnerCollection();
                    FieldEnd.OwnerParagraph.OwnerTextBody.Items.Insert(paraIndex, para);
                    int count = FieldEnd.OwnerParagraph.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (FieldEnd == FieldEnd.OwnerParagraph.Items[0])
                            break;
                        para.Items.Add(FieldEnd.OwnerParagraph.Items[0]);
                    }
                    para.Items.Add(fieldMark);
                }
                FieldSeparator = fieldMark;
                m_bIsFieldRangeUpdated = false;
            }
            else if (FieldSeparator.NextSibling != null)
                m_resultFormat = (FieldSeparator.NextSibling as ParagraphItem).ParaItemCharFormat.CloneInt() as WCharacterFormat;
        }
        /// <summary>
        /// Gets the items to update.
        /// </summary>
        /// <param name="text">The text.</param>
        private void GetItemsToUpdate(string text)
        {
            //Retrieves the items to be updated as field result.
            m_itemsToUpdate.Clear();
            if (text == string.Empty)
                return;
            int index = NestedFieldCode.IndexOf(text);
            string itemText = string.Empty;
            int itemIndex = GetStartItemIndex(index, ref itemText);
            string resultText = string.Empty;
            string remainderText = text;
            if (remainderText.Length < itemText.Length)
                itemText = remainderText;
            string txt = string.Empty;
            m_bIsFieldSeparator = false;
            m_bIsSkip = false;
            m_nestedFields.Clear();
            for (int i = itemIndex; i < Range.Items.Count - 1; i++)
            {
                Entity entity = Range.Items[i] as Entity;
                if (entity is ParagraphItem)
                    txt = UpdateTextForParagraphItem(entity);
                else
                    txt = UpdateTextForTextBodyItem(entity);
                Entity clonedEntity = null, nextItem = null;
                if (i == itemIndex
                    && itemText != string.Empty)
                {
                    resultText += itemText;
                    if (itemText.Contains(PARAGRAPHMARK))
                        itemText = itemText.Replace(PARAGRAPHMARK, string.Empty);
                    clonedEntity = GetClonedEntity(entity, itemText, ref nextItem);
                }
                else if (text.Length < (resultText + txt).Length)
                {
                    resultText += remainderText;
                    clonedEntity = GetClonedEntity(entity, remainderText, ref nextItem);
                }
                else
                {
                    resultText += txt;
                    clonedEntity = GetClonedEntity(entity, null, ref nextItem);
                }
                if (clonedEntity != null)
                    m_itemsToUpdate.Add(clonedEntity);
                remainderText = text.Substring(resultText.Length);
                if (nextItem != null)
                {
                    i = Range.Items.IndexOf(nextItem);
                    m_nestedFields.Clear();
                    m_bIsSkip = false;
                    nextItem = null;
                }
                if (resultText == text)
                    break;
            }
            m_bIsFieldSeparator = false;
            m_bIsSkip = false;
            m_nestedFields.Clear();
        }
        /// <summary>
        /// Gets the cloned entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="nextItem">The next item.</param>
        /// <returns></returns>
        private Entity GetClonedEntity(Entity entity, string txt, ref Entity nextItem)
        {
            //Retrieves the cloned item to be updated as field result.
            Entity clonedEntity;
            if (entity is ParagraphItem)
            {
                clonedEntity = GetClonedParagraphItem(entity, txt, ref nextItem);
            }
            else if (entity is WParagraph)
            {
                List<Entity> itemsToUpdate = GetClonedParagraph(entity, txt, ref nextItem);
                for (int i = 0; i < itemsToUpdate.Count - 1; i++)
                {
                    m_itemsToUpdate.Add(itemsToUpdate[i]);
                }
                clonedEntity = itemsToUpdate[itemsToUpdate.Count - 1];
            }
            else
            {
                clonedEntity = GetClonedTable(entity);
            }
            return clonedEntity;
        }
        /// <summary>
        /// Gets the cloned table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected Entity GetClonedTable(Entity entity)
        {
            //Clones the table to be updated as field result.
            Entity clonedEntity = entity.Clone();
            WTable clonedTable = clonedEntity as WTable;
            WTable table = entity as WTable;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                {
                    UpdateClonedTextBodyItem(table.Rows[i].Cells[j], clonedTable.Rows[i].Cells[j]);
                }
            }
            return clonedEntity;
        }
        /// <summary>
        /// Updates the cloned text body item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void UpdateClonedTextBodyItem(WTextBody source, WTextBody destination)
        {
            //Updates the contents of cloned text body.
            destination.Items.Clear();
            for (int i = 0; i < source.Items.Count; i++)
            {
                Entity nextItem = null;
                if (source.Items[i] is WParagraph)
                {
                    List<Entity> itemsToUpdate = GetClonedParagraph(source.Items[i], null, ref nextItem);
                    foreach(Entity entity in itemsToUpdate)
                    {
                        destination.Items.Add(entity);
                    }
                    if (nextItem != null)
                    {
                        i = source.Items.IndexOf(nextItem);
                        m_nestedFields.Clear();
                        m_bIsSkip = false;
                        nextItem = null;
                    }
                }
                else
                {
                    destination.Items.Add(GetClonedTable(source.Items[i]));
                }
            }
        }
        /// <summary>
        /// Gets the cloned paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="nextItem">The next item.</param>
        /// <returns></returns>
        private Entity GetClonedParagraphItem(Entity entity, string txt, ref Entity nextItem)
        {
            //Retrieves the paragraph item to be updated as field result.
            Entity clonedEntity = entity.Clone();
            if (entity is WField
                && (entity as WField).FieldSeparator != null
                && (entity as WField).FieldEnd != null
                && (entity as WField).FieldType != FieldType.FieldHyperlink)
            {
                int index = 0;
                if ((entity as WField).Range.Items.Contains((entity as WField).FieldSeparator))
                    index = (entity as WField).Range.Items.IndexOf((entity as WField).FieldSeparator) + 1;
                else if ((entity as WField).Range.Items.Contains((entity as WField).FieldSeparator.OwnerParagraph))
                    index = (entity as WField).Range.Items.IndexOf((entity as WField).FieldSeparator.OwnerParagraph);
                for (int i = index; i < (entity as WField).Range.Items.Count - 1; i++)
                {
                    if ((entity as WField).Range.Items[i] == (entity as WField).FieldSeparator.OwnerParagraph)
                    {
                        if (((entity as WField).Range.Items[i] as WParagraph).LastItem == (entity as WField).FieldSeparator)
                            continue;
                        WParagraph clonedPara = ((entity as WField).Range.Items[i] as Entity).Clone() as WParagraph;
                        clonedPara.ClearItems();
                        Entity item = (entity as WField).FieldSeparator.NextSibling as Entity;
                        while (item != null)
                        {
                            clonedPara.Items.Add(item.Clone());
                            item = (entity as WField).FieldSeparator.NextSibling as Entity;
                        }
                        clonedEntity = clonedPara;
                    }
                    else
                        clonedEntity = ((entity as WField).Range.Items[i] as Entity).Clone();
                    if (i == (entity as WField).Range.Items.Count - 2)
                        break;
                    m_itemsToUpdate.Add(clonedEntity);
                }
                nextItem = (entity as WField).Range.Items[(entity as WField).Range.Items.Count - 1] as Entity;
            }
            else if (!string.IsNullOrEmpty(txt)
                    && (clonedEntity is WTextRange))
            {
                (clonedEntity as WTextRange).Text = txt;
            }
            return clonedEntity;
        }
        /// <summary>
        /// Gets the cloned paragraph.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="nextItem">The next item.</param>
        /// <returns></returns>
        private List<Entity> GetClonedParagraph(Entity entity, string txt, ref Entity nextItem)
        {
            //Retrieves the paragraph item to be updated as field result.
            List<Entity> itemsToUpdate = new List<Entity>();
            WParagraph clonedPara = entity.Clone() as WParagraph;
            clonedPara.ClearItems();
            if (txt != null && txt.EndsWith(PARAGRAPHMARK))
                txt = txt.Remove(txt.Length - 1);
            string itemText = string.Empty, remainderText = txt;
            int itemIndex = GetParagraphItemIndex(entity, txt);
            //Updates the contents based on the field results.
            for (int i = itemIndex; i < (entity as WParagraph).Items.Count; i++)
            {
                ParagraphItem item = (entity as WParagraph).Items[i];
                Entity clonedEntity = item.Clone();
                bool isfieldSeparator = m_bIsFieldSeparator;
                m_bIsFieldSeparator = false;
                itemText = UpdateTextForParagraphItem((entity as WParagraph).Items[i]);
                m_bIsFieldSeparator = isfieldSeparator;
                bool isTextBodyItems = false;
                if (item is WField
                    && (item as WField).FieldSeparator != null
                    && (item as WField).FieldEnd != null
                    && (item as WField).FieldType != FieldType.FieldHyperlink)
                {
                    isTextBodyItems = UpdateFieldItems(item, clonedEntity, clonedPara, ref itemsToUpdate);
                    nextItem = (item as WField).Range.Items[(item as WField).Range.Items.Count - 1] as Entity;
                }
                else if (txt != null
                    && clonedEntity is WTextRange)
                {
                    if (i == itemIndex
                        && itemText.Contains(txt)
                        && !itemText.StartsWith(txt))
                        itemText = txt;
                    if (itemText.Contains(remainderText)
                        && !remainderText.Contains(itemText))
                        itemText = remainderText;
                    (clonedEntity as WTextRange).Text = itemText;
                }
                if (!string.IsNullOrEmpty(remainderText))
                    remainderText = remainderText.Substring(itemText.Length);
                if (isTextBodyItems)
                    break;
                else
                {
                    clonedPara.Items.Add(clonedEntity);
                    if (remainderText != null 
                        && remainderText == string.Empty)
                        break;
                    if (nextItem != null)
                    {
                        i = (entity as WParagraph).Items.IndexOf(nextItem);
                        m_nestedFields.Clear();
                        m_bIsSkip = false;
                        nextItem = null;
                    }
                }
            }
            itemsToUpdate.Add(clonedPara);
            return itemsToUpdate;
        }
        /// <summary>
        /// Updates the field items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="itemsToUpdate">The items to update.</param>
        /// <returns></returns>
        private bool UpdateFieldItems(ParagraphItem item, Entity entity, WParagraph paragraph, ref List<Entity> itemsToUpdate)
        {
            bool isTextBodyItems = false;
            int index = 0;
            if ((item as WField).Range.Items.Contains((item as WField).FieldSeparator))
                index = (item as WField).Range.Items.IndexOf((item as WField).FieldSeparator) + 1;
            else if ((item as WField).Range.Items.Contains((item as WField).FieldSeparator.OwnerParagraph))
                index = (item as WField).Range.Items.IndexOf((item as WField).FieldSeparator.OwnerParagraph);
            for (int j = index; j < (item as WField).Range.Items.Count - 1; j++)
            {
                if ((item as WField).Range.Items[j] == (item as WField).FieldSeparator.OwnerParagraph)
                {
                    if (((item as WField).Range.Items[j] as WParagraph).LastItem == (item as WField).FieldSeparator)
                        continue;
                    WParagraph para = ((item as WField).Range.Items[j] as Entity).Clone() as WParagraph;
                    paragraph.ClearItems();
                    Entity ent = (item as WField).FieldSeparator.NextSibling as Entity;
                    while (ent != null)
                    {
                        para.Items.Add(ent.Clone());
                        ent = (item as WField).FieldSeparator.NextSibling as Entity;
                    }
                    entity = para;
                }
                else
                    entity = ((item as WField).Range.Items[j] as Entity).Clone();
                if (entity is TextBodyItem)
                    isTextBodyItems = true;
                if (j == (item as WField).Range.Items.Count - 2)
                    break;
                if (entity is TextBodyItem)
                {
                    itemsToUpdate.Add(paragraph);
                    itemsToUpdate.Add(entity);
                }
                else
                    paragraph.Items.Add(entity);
            }
            return isTextBodyItems;
        }
        /// <summary>
        /// Gets the index of the paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="txt">The TXT.</param>
        /// <returns></returns>
        private int GetParagraphItemIndex(Entity entity, string txt)
        {
            //Retrieves the index of the item to updated as field result.
            int itemIndex = 0;
            string itemText = string.Empty;
            if (txt != null)
            {
                bool isfieldSeparator = m_bIsFieldSeparator;
                m_bIsFieldSeparator = false;
                string text = UpdateTextForTextBodyItem(entity);
                m_bIsFieldSeparator = isfieldSeparator;
                if (text.EndsWith(PARAGRAPHMARK))
                    text = text.Remove(text.Length - 1);
                int textIndex = text.IndexOf(txt);
                if (textIndex > 0)
                    for (int i = 0; i < (entity as WParagraph).Items.Count; i++)
                    {
                        itemText += UpdateTextForParagraphItem((entity as WParagraph).Items[i]);
                        if (textIndex == itemText.Length)
                        {
                            itemIndex = i + 1;
                            break;
                        }
                        else if (textIndex < itemText.Length)
                        {
                            txt = itemText.Substring(textIndex);
                            itemIndex = i;
                            break;
                        }
                    }
            }
            return itemIndex;
        }
        /// <summary>
        /// Gets the start index of the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private int GetStartItemIndex(int index, ref string text)
        {
            //Retrieves the index of the item in Range collection.
            string fieldCode = FieldCode;
            m_bIsFieldSeparator = false;
            m_nestedFields.Clear();
            m_bIsSkip = false;
            string txt = string.Empty;
            //Gets the index of the first item.
            for (int i = 0; i < Range.Items.Count; i++)
            {
                Entity entity = Range.Items[i] as Entity;
                if (entity is ParagraphItem)
                    txt = UpdateTextForParagraphItem(entity);
                else
                    txt = UpdateTextForTextBodyItem(entity);
                fieldCode += txt;
                if (m_bIsFieldSeparator)
                    break;
                if (index == fieldCode.Length)
                {
                    index = i + 1;
                    break;
                }
                else if (index < fieldCode.Length)
                {
                    if (m_bIsSkip)
                    {
                        WField curField = m_nestedFields.Pop();
                        if (curField.FieldSeparator.GetIndexInOwnerCollection() == curField.FieldSeparator.OwnerParagraph.Items.Count - 1)
                            index = Range.Items.IndexOf(curField.FieldSeparator.OwnerParagraph) + 1;
                        else
                        {
                            text = fieldCode.Substring(index);
                            index = Range.Items.IndexOf(curField.FieldSeparator.OwnerParagraph);
                        }
                    }
                    else
                    {
                        text = fieldCode.Substring(index);
                        index = i;
                    }
                    break;
                }
            }
            m_bIsFieldSeparator = false;
            m_nestedFields.Clear();
            m_bIsSkip = false;
            return index;
        }
        /// <summary>
        /// Removes the previous result.
        /// </summary>
        protected void RemovePreviousResult()
        {
            //Removes the previous result.
            for (int i = Range.Count - 1; i >= 0; i--)
            {
                Entity entity = Range.Items[i] as Entity;
                WTextBody textBody;
                if (entity == FieldEnd)
                    continue;
                if (entity == FieldSeparator)
                    break;
                if(entity is ParagraphItem)
                {
                    //Removes the paragraph items from paragraph.
                    Range.Items.Remove(entity);
                    OwnerParagraph.Items.Remove(entity);
                }
                else if (entity is WParagraph)
                {
                    textBody = (entity as WParagraph).OwnerTextBody;
                    if (!m_bIsFieldSeparator)
                        CheckPragragh(entity as WParagraph);
                    if (m_bIsFieldSeparator)
                        break;
                    else if ((entity as WParagraph).Items.Count == 0)
                    {
                        //Removes the paragraph from textbody.
                        Range.Items.Remove(entity);
                        textBody.Items.Remove(entity);
                    }
                }
                else if (entity is WTable)
                {
                    textBody = (entity as WTable).OwnerTextBody;
                    if (!m_bIsFieldSeparator)
                    {
                        //Removes the table from textbody.
                        Range.Items.Remove(entity);
                        textBody.Items.Remove(entity);
                    }
                }
            }
            m_bIsFieldRangeUpdated = false;
        }
        /// <summary>
        /// Checks the pragragh.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void CheckPragragh(WParagraph paragraph)
        {
            //Checks whether the paragraph has field separator or field end.
            for (int i = paragraph.Items.Count - 1; i >= 0; i--)
            {
                ParagraphItem item = paragraph.Items[i];
                if (FieldEnd.OwnerParagraph == paragraph
                    && i > paragraph.Items.IndexOf(FieldEnd))
                {
                    i = paragraph.Items.IndexOf(FieldEnd);
                    continue;
                }
                else if (item == FieldEnd)
                    continue;

                if (item == FieldSeparator)
                {
                    m_bIsFieldSeparator = true;
                    break;
                }
                if (!m_bIsFieldSeparator)
                    paragraph.Items.Remove(item);
            }
        }
        /// <summary>
        /// Updates the paragraph text.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="isLastItem">if set to <c>true</c> [is last item].</param>
        /// <param name="result">The result.</param>
        private void UpdateParagraphText(WParagraph paragraph, bool isLastItem, ref string result)
        {
            bool isFieldSeparator = m_bIsFieldSeparator;
            m_bIsFieldSeparator = false;
            result = UpdateTextForTextBodyItem(paragraph) + result;
            m_bIsFieldSeparator = isFieldSeparator;
            //Removes the paragraph mark for the last item.
            if (isLastItem)
                result = result.TrimEnd('\r');
        }
        /// <summary>
        /// Gets the paragraph item text.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private string GetParagraphItemText(ParagraphItem item)
        {
            //Retrieves the paragraph items text.
            string text = string.Empty;
            if (item is WField)
            {
                if ((item as WField).FieldType == FieldType.FieldMergeField)
                    text = (item as WTextRange).Text;
                else
                {
                    if (!Document.UpdatedFields.Contains(item as WField)
                        && item != this)
                        (item as WField).Update();
                    text = (item as WField).FieldResult;
                }
            }
            else if (item is WTextRange)
                text = (item as WTextRange).Text;
            return text;
        }
        /// <summary>
        /// Updates the field result.
        /// </summary>
        /// <param name="text">The text.</param>
        internal void UpdateFieldResult(string text)
        {
            FieldResult = text;
            if (OwnerParagraph == null
                || FieldEnd == null)
                return;
            CheckFieldSeparator();
            RemovePreviousResult();
            if (OwnerParagraph == FieldEnd.OwnerParagraph)
            {
                text = text.Replace(ControlChar.CrLf, ControlChar.ParagraphBreak);
                text = text.Replace(ControlChar.LineFeedChar, ControlChar.ParagraphBreakChar);
                int index = text.IndexOf(ControlChar.ParagraphBreakChar);
                if (index != -1)
                {
                    //Updates the field's previous result formatting for first text range (Text before carriage retrun).
                    string splittedText = text.Substring(index);
                    text = text.Substring(0, index);
                    OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), GetTextRange(text));
                    OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), GetTextRange(splittedText));
                }
                else
                    OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), GetTextRange(text));
            }
            else
            {
                text = text.Replace(ControlChar.CrLf, ControlChar.ParagraphBreak);
                text = text.Replace(ControlChar.LineFeedChar, ControlChar.ParagraphBreakChar);
                string[] splittedText = text.Split(ControlChar.ParagraphBreakChar);
                for (int i = 0; i < splittedText.Length; i++)
                {
                    WTextRange textRange = new WTextRange(Document);
                    // Updates the character format.
                    if (m_resultFormat != null)
                    {
                        textRange.CharacterFormat.ImportContainer(m_resultFormat);
                        textRange.CharacterFormat.CopyProperties(m_resultFormat);
                        m_resultFormat = null;
                    }
                    else
                    {
                        textRange.CharacterFormat.ImportContainer(this.CharacterFormat);
                        textRange.CharacterFormat.CopyProperties(this.CharacterFormat);
                    }
                    if (textRange.CharacterFormat.Sprms != null
                        && textRange.CharacterFormat.Sprms.HasSprm(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec))
                        textRange.CharacterFormat.Sprms.RemoveValue(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec);
                    // Updates the text.
                    textRange.Text = splittedText[i];
                    if (i == 0)
                        FieldSeparator.OwnerParagraph.Items.Add(textRange);
                    else if (i == splittedText.Length - 1)
                        FieldEnd.OwnerParagraph.Items.Insert(FieldEnd.GetIndexInOwnerCollection(), textRange);
                    else
                    {
                        WParagraph para = FieldEnd.OwnerParagraph.Clone() as WParagraph;
                        para.ClearItems();
                        int paraIndex = FieldEnd.OwnerParagraph.GetIndexInOwnerCollection();
                        FieldEnd.OwnerParagraph.OwnerTextBody.Items.Insert(paraIndex, para);
                        para.Items.Add(textRange);
                    }
                }
            }
            m_bIsFieldRangeUpdated = false;
        }
        /// <summary>
        /// Gets the text range.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected WTextRange GetTextRange(string text)
        {
            WTextRange textRange = new WTextRange(Document);
            // Updates the character format.
            if (m_resultFormat != null)
            {
                textRange.CharacterFormat.ImportContainer(m_resultFormat);
                textRange.CharacterFormat.CopyProperties(m_resultFormat);
                m_resultFormat = null;
            }
            else
            {
                textRange.CharacterFormat.ImportContainer(this.CharacterFormat);
                textRange.CharacterFormat.CopyProperties(this.CharacterFormat);
            }
            if (textRange.CharacterFormat.Sprms != null
                && textRange.CharacterFormat.Sprms.HasSprm(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec))
                textRange.CharacterFormat.Sprms.RemoveValue(Syncfusion.DocIO.ReaderWriter.Biff_Records.WordSprmOptions.sprmCFSpec);
            // Updates the text.
            textRange.Text = text;
            return textRange;
        }
        #endregion
        #region SkipLayoutOfFieldCode
#if !SILVERLIGHT && !WINRT && !WP
        /// <summary>
        /// Skips the layouting of field code.
        /// </summary>
        private void SkipLayoutingOfFieldCode()
        {
            m_bIsFieldSeparator = false;
            for (int i = 0; i < Range.Items.Count; i++)
            {
                Entity entity = Range.Items[i] as Entity;
                if (entity is ParagraphItem)
                    SkipLayoutingOfParagraphItem(entity);
                else
                    SkipLayoutingOfTextBodyItem(entity);
                if (m_bIsFieldSeparator)
                    break;
            }
            m_bIsFieldSeparator = false;
        }

        /// <summary>
        /// Skips the layouting of paragraph item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void SkipLayoutingOfParagraphItem(Entity entity)
        {
            if (!m_bIsFieldSeparator)
            {
                if (FieldSeparator == entity)
                {
                    m_bIsFieldSeparator = true;
                    return;
                }
                (entity as IWidget).LayoutInfo.IsSkip = true;
            }
        }

        /// <summary>
        /// Skips the layouting of text body item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SkipLayoutingOfTextBodyItem(Entity entity)
        {
            if (entity is WParagraph)
            {
                if ((entity as WParagraph).ChildEntities.Contains(FieldSeparator))
                {
                    for (int i = 0; i < (entity as WParagraph).Items.Count; i++)
                    {
                        SkipLayoutingOfParagraphItem((entity as WParagraph).Items[i]);
                        if (m_bIsFieldSeparator)
                            return;
                    }
                }
                else
                    (entity as IWidget).LayoutInfo.IsSkip = true;
            }
            else if (entity is WTable)
            {
                SkipLayoutingOfTable(entity);
            }
        }

        /// <summary>
        /// Skips the layouting of table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SkipLayoutingOfTable(Entity entity)
        {
            for (int i = 0; i < (entity as WTable).Rows.Count; i++)
            {
                WTableRow row = (entity as WTable).Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    WTableCell cell = row.Cells[j];
                    for (int k = 0; k < cell.Items.Count; k++)
                    {
                        SkipLayoutingOfTextBodyItem(cell.Items[k]);
                        if (m_bIsFieldSeparator)
                            return;
                    }
                }
            }
            if (!m_bIsFieldSeparator)
                (entity as IWidget).LayoutInfo.IsSkip = true;
        }
#endif
        #endregion

        #region ILeafWidget Members
#if !SILVERLIGHT && !WP
        SizeF ILeafWidget.Measure(Syncfusion.DocIO.Rendering.DrawingContext dc)
        {
            SizeF size = SizeF.Empty;
            string displayText = "";
            WCharacterFormat charFormat = GetCharFormat();

            switch (this.FieldType)
            {
                case FieldType.FieldMergeField:
                    displayText = (FieldValue == string.Empty) ? UpdateMergeFieldText(this as WMergeField) : FieldValue;
                    if (displayText == string.Empty)
                    {
                        size = dc.MeasureString(" ", charFormat.Font, null);
                        size.Width = 0;
                    }
                    else
                        size = dc.MeasureString(displayText, charFormat.Font, null, charFormat,false);
                    (this as IWidget).LayoutInfo.Size = size;
                    break;

                case FieldType.FieldDocVariable:
                    if (!(this.NextSibling != null && this.NextSibling is WFieldMark && (this.NextSibling as WFieldMark).Type == FieldMarkType.FieldSeparator))
                    {
                        displayText = this.Document.Variables[this.FieldValue];
                        size = dc.MeasureString(displayText, charFormat.Font, null, charFormat,false);
                        (this as IWidget).LayoutInfo.Size = size;
                    }
                    else
                        this.m_layoutInfo.IsSkip = true;
                    break;
                case FieldType.FieldNumPages:
                    string text = string.Empty;
                    charFormat = GetCharacterFormat();
                    if (Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.IsFirstLayouting)
                    {
                        (this as WTextRange).Text = "";
                        Entity nextSibling = this.NextSibling as Entity;
                        while (!(nextSibling != null && (nextSibling is WFieldMark)
                            && (nextSibling as WFieldMark).Type == FieldMarkType.FieldEnd))
                        {
                            if (nextSibling is ParagraphItem)
                                ((nextSibling as ParagraphItem) as IWidget).LayoutInfo.IsSkip = true;
                            nextSibling = nextSibling.NextSibling as Entity;
                        }
                    }
                    else
                         text = (this as WTextRange).Text;
                    size = dc.MeasureString(text, charFormat.Font, null, charFormat,false);
                    if(text != string.Empty)
                        (this as IWidget).LayoutInfo.Size = size;
                    break;
                case FieldType.FieldPage:
                    if (Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.IsFirstLayouting)
                    {
                        Entity nextSibling = this.NextSibling as Entity;
                        while (!(nextSibling != null && (nextSibling is WFieldMark)
                            && (nextSibling as WFieldMark).Type == FieldMarkType.FieldEnd))
                        {
                            if (nextSibling is ParagraphItem)
                                ((nextSibling as ParagraphItem) as IWidget).LayoutInfo.IsSkip = true;
                            nextSibling = nextSibling.NextSibling as Entity;
                        }
                    }
                    break;
                case FieldType.FieldIf:
                case FieldType.FieldExpression:
                    if (Syncfusion.DocIO.DLS.Rendering.DocumentLayouter.IsFirstLayouting && FieldEnd != null)
                    {
                        //Skip to layout the field code items
                        SkipLayoutingOfFieldCode();
                    }
                    break;
                case FieldType.FieldUnknown:
                    if (FieldEnd == null)
                    {
                        size = dc.MeasureString(FieldCode, charFormat.Font, null, charFormat,false);
                        (this as IWidget).LayoutInfo.Size = size;
                    }
                    break;
                default:
                    break;
            }
            return size;
        }
        /// <summary>
        /// Get CharacterFormat of the PAGE and NUMPAGES field based on TextRanges preserved inside the field
        /// </summary>
        /// <returns></returns>
        internal WCharacterFormat GetCharacterFormat()
        {
            Entity nextSibling = this.NextSibling as Entity;
            while (this.FormattingString.ToUpper().Contains("\\* MERGEFORMAT")//Check whether the formatting string contains or not if dose not contains means gettig field code character format.
              && !(nextSibling != null && (nextSibling is WFieldMark)//check whether the nextsibling is not field mark
               && (nextSibling as WFieldMark).Type == FieldMarkType.FieldEnd))
            {
                if (nextSibling is WTextRange)
                {
                    return (nextSibling as WTextRange).CharacterFormat;
                }
                nextSibling = nextSibling.NextSibling as Entity;
            }
            return this.CharacterFormat;
        }
#endif
        /// <summary>
        /// Update Merge Field Text
        /// </summary>
        /// <param name="mergeField"></param>
        /// <returns></returns>
        internal string UpdateMergeFieldText(WMergeField mergeField)
        {
            string text = mergeField.Text;
            try
            {
                if (mergeField.ConvertedToText && !string.IsNullOrEmpty(mergeField.Text))
                {
                    //Updating TextBefore and TextAfter values after updating the Field
                    text = mergeField.TextBefore + text + mergeField.TextAfter;
                    if (mergeField.NumberFormat != string.Empty)
                    {
                        text = text.Replace(",", ".");
                        double d = double.Parse(text, CultureInfo.InvariantCulture);
                        if (mergeField.NumberFormat.Contains("%"))
                            d = d / 100;
                        string formattedValue = d.ToString(mergeField.NumberFormat, CultureInfo.InvariantCulture);
                        text = formattedValue;
                    }
                    else if (mergeField.DateFormat != string.Empty)
                    {
                        DateTime dateTime = DateTime.Parse(text);
                        string value = dateTime.ToString(mergeField.DateFormat, DateTimeFormatInfo.CurrentInfo);
                        text = value;
                    }
                }
            }
            catch
            {}
            return text;
        }

        #endregion

        #region IWidget Members
#if !SILVERLIGHT && !WP

        ILayoutInfo IWidget.LayoutInfo
        {
            get
            {
                if (m_layoutInfo == null)
                    CreateLayoutInfo();
                return m_layoutInfo;
            }
        }

        void IWidget.Draw(Syncfusion.DocIO.Rendering.DrawingContext dc, LayoutedWidget ltWidget)
        {
            switch (this.FieldType)
            {
                case FieldType.FieldHyperlink:
                    dc.currHyperlink = new Hyperlink(this);
                    break;
                case FieldType.FieldMergeField:
                    WMergeField mergeField = (this as WMergeField);
                    dc.DrawMergeField(mergeField, ltWidget);
                    break;

                case FieldType.FieldPage:
                    WField pageNumField = (this as WField);
                    dc.currTextRange = GetCurrentTextRange();
                    dc.DrawString(ltWidget.TextTag, GetCharacterFormat(), pageNumField.GetOwnerParagraph().ParagraphFormat, ltWidget.Bounds, ltWidget.Bounds.Width, ltWidget);
                    break;

                case FieldType.FieldDocVariable:
                    WField docVariable = (this as WField);
                    ltWidget.TextTag = this.Document.Variables[docVariable.FieldValue];
                    dc.DrawString(ltWidget.TextTag, docVariable.CharacterFormat, docVariable.OwnerParagraph.ParagraphFormat, ltWidget.Bounds, ltWidget.Bounds.Width, ltWidget);
                    break;

                case FieldType.FieldNumPages:
                    WField numPages = this as WField;
                    WTextRange tr = this as WTextRange;
                    dc.currTextRange = GetCurrentTextRange();
                    dc.DrawString(tr.Text, GetCharacterFormat(), numPages.GetOwnerParagraph().ParagraphFormat, ltWidget.Bounds, ltWidget.Bounds.Width, ltWidget);
                    break;
                case FieldType.FieldUnknown:
                    if (FieldEnd == null)
                    {
                        dc.currTextRange = (this as WTextRange);
                        dc.DrawString(FieldCode, GetCharFormat(), this.GetOwnerParagraph().ParagraphFormat, ltWidget.Bounds, ltWidget.Bounds.Width, ltWidget);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        /// <summary>
        /// Get Text Range preserved inside the PAGE and NUMPAGES field
        /// </summary>
        /// <returns></returns>
        private WTextRange GetCurrentTextRange()
        {
            Entity nextSibling = this.NextSibling as Entity;
            while (this.FormattingString.ToUpper().Contains("\\* MERGEFORMAT")//Check whether the formatting string contains or not if dose not contains means gettig field code textrange as current textrange
                && !(nextSibling != null && (nextSibling is WFieldMark)//check whether the nextsibling is not field mark
                && (nextSibling as WFieldMark).Type == FieldMarkType.FieldEnd))
            {
                if (nextSibling is WTextRange)
                {
                    return (nextSibling as WTextRange);
                }
                nextSibling = nextSibling.NextSibling as Entity;
            }
            return (this as WTextRange);
        }
#endif

        /// <summary>
        /// Month
        /// </summary>
        internal enum Month
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        #endregion
    }
}
