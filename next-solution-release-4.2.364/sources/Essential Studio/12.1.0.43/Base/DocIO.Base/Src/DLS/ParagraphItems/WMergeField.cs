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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WMergeField.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WMergeField
      : WField,
        IWMergeField
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected string m_fieldName = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_textBefore = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_textAfter = "";
        /// <summary>
        /// 
        /// </summary>
        static private Regex m_regex = new Regex("MERGEFIELD\\s+\"?([^:\"]+):?([^\"]*)\"?");
        /// <summary>
        /// 
        /// </summary>
        private string m_prefix = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_numberFormat = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_dateFormat = "";

        /// <summary>
        /// The collection of text ranges which form field value
        /// </summary>
        private ParagraphItemCollection m_pItemColl;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.MergeField;
            }
        }
        /// <summary>
        /// Gets / sets field name
        /// </summary>
        public string FieldName
        {
            get
            {
                return m_fieldName;
            }
            set
            {
                m_fieldName = value;
                if (!this.Document.IsOpening 
                    && !this.Document .IsMailMerge 
                    && Text == string.Empty)
                {
                    char symbol1 = (char)((byte)171);
                    char symbol2 = (char)((byte)187);
                    Text = symbol1 + m_fieldName + symbol2;
                }
            }
        }
        /// <summary>
        /// Gets / sets the text before merge field
        /// </summary>
        public string TextBefore
        {
            get
            {
                return m_textBefore;
            }
            set
            {
                m_textBefore = value;
                m_formattingString = ConvertSwitchesToString();
            }
        }
        /// <summary>
        /// Gets / sets the text after merge field
        /// </summary>
        public string TextAfter
        {
            get
            {
                return m_textAfter;
            }
            set
            {
                m_textAfter = value;
                m_formattingString = ConvertSwitchesToString();
            }
        }
        /// <summary>
        /// Gets the prefix of merge field.
        /// </summary>
        public string Prefix
        {
            get
            {
                return m_prefix;
            }
            internal set
            {
                m_prefix = value;
            }
        }
        /// <summary>
        /// Gets the number format.
        /// </summary>
        public string NumberFormat
        {
            get
            {
                return m_numberFormat;
            }
        }
        /// <summary>
        /// Gets the date format.
        /// </summary>
        public string DateFormat
        {
            get
            {
                return m_dateFormat;
            }
        }

        /// <summary>
        /// Gets the text items.
        /// </summary>
        /// <value>The text items.</value>
        public ParagraphItemCollection TextItems
        {
            get
            {
                if (m_pItemColl == null)
                {
                    m_pItemColl = new ParagraphItemCollection(Document);
                    m_pItemColl.SetOwner(this);
                }
                return m_pItemColl;
            }
        }

        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WMergeField"/> class.
        /// </summary>
        /// <param name="doc">The document</param>
        public WMergeField(IWordDocument doc)
            : base(doc)
        {
            m_paraItemType = ParagraphItemType.MergeField;
            m_pItemColl = new ParagraphItemCollection( doc as WordDocument );
            m_pItemColl.SetOwner(this);
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
            m_fieldName = reader.ReadString(XDLSConstants.FieldNameAttr);

            if (reader.HasAttribute(XDLSConstants.MergeFieldTextBeforeAttr))
            {
                m_textBefore = reader.ReadString(XDLSConstants.MergeFieldTextBeforeAttr);
            }

            if (reader.HasAttribute(XDLSConstants.MergeFieldTextAfterAttr))
            {
                m_textAfter = reader.ReadString(XDLSConstants.MergeFieldTextAfterAttr);
            }

            if (reader.HasAttribute(XDLSConstants.MergeFieldNumberFormatAttr))
            {
                m_numberFormat = reader.ReadString(XDLSConstants.MergeFieldNumberFormatAttr);
            }

            if (reader.HasAttribute(XDLSConstants.MergeFieldDateFormatAttr))
            {
                m_dateFormat = reader.ReadString(XDLSConstants.MergeFieldDateFormatAttr);
            }

            if (reader.HasAttribute(XDLSConstants.MergeFieldPrefixAttr))
            {
                m_prefix = reader.ReadString(XDLSConstants.MergeFieldPrefixAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            //      writer.WriteValue( XDLSConstants.TypeTag, WParagraphItemType.MergeField );

            if (FieldName != string.Empty)
            {
                writer.WriteValue(XDLSConstants.FieldNameAttr, FieldName);
            }

            if (m_textBefore != string.Empty)
            {
                writer.WriteValue(XDLSConstants.MergeFieldTextBeforeAttr, TextBefore);
            }

            if (m_textAfter != string.Empty)
            {
                writer.WriteValue(XDLSConstants.MergeFieldTextAfterAttr, TextAfter);
            }

            if (m_numberFormat != string.Empty)
            {
                writer.WriteValue(XDLSConstants.MergeFieldNumberFormatAttr, NumberFormat);
            }

            if (m_dateFormat != string.Empty)
            {
                writer.WriteValue(XDLSConstants.MergeFieldDateFormatAttr, DateFormat);
            }

            if (m_prefix != string.Empty)
            {
                writer.WriteValue(XDLSConstants.MergeFieldPrefixAttr, m_prefix);
            }
        }
//#endif
        #endregion

        #region Class internal methods
        /// <summary>
        /// </summary>
        /// <param name="fieldCode"></param>
        override internal protected void ParseFieldCode(string fieldCode)
        {
            this.FieldCode = fieldCode;
            UpdateFieldCode(fieldCode);
        }
        /// <summary>
        /// Get Field Values - Parse for Field name and field switches
        /// </summary>
        /// <param name="filedValue"></param>
        internal protected string[] GetFieldValues(string fieldvalue)
        {
            int index = 0;
            //Get indexes of the separators
            List<int> slashIndex = new List<int>();
            //Get double quotes start and end location as a pair
            List<KeyValuePair<int, int>> indexForDoubleQuoteList = new List<KeyValuePair<int, int>>();
            int doubleQuoteStartIndex = 0;
            int doubleQuoteEndIndex = 0;
            bool isdoubleQuoteOpen = true;
            //Check for slash, double quote start and end index
            for (int i = 0; i < fieldvalue.Length; i++)
            {
                if (fieldvalue[i] == '\\')
                    slashIndex.Add(i);
                else if (i < fieldvalue.Length - 1 && fieldvalue[i] == '"')
                {
                    if (isdoubleQuoteOpen)
                    {
                        doubleQuoteStartIndex = i;
                        isdoubleQuoteOpen = false;
                    }
                    else
                    {
                        doubleQuoteEndIndex = i;
                        isdoubleQuoteOpen = true;
                        indexForDoubleQuoteList.Add(new KeyValuePair<int, int>( doubleQuoteStartIndex, doubleQuoteEndIndex));
                    }
                }

            }
            List<int> copySlashindeces = new List<int>(slashIndex);
            // check for slash index present between double quotes and remove it from the collection
            foreach (int i in slashIndex)
            {
                foreach (KeyValuePair<int, int> j in indexForDoubleQuoteList)
                {
                    if (j.Key < i && j.Value > i)//Delete the slash index if exsit between the double quotes
                    {
                        copySlashindeces.Remove(i);
                        break;
                    }
                }

            }
            string[] fieldValues = new string[copySlashindeces.Count + 1];
            if (copySlashindeces.Count > 0)
            {
                int startIndex = 0;
                int i = 0;
                //Split the string based on the separator index
                for (i = 0; i < copySlashindeces.Count; i++)
                {
                    fieldValues[i] = fieldvalue.Substring(startIndex, copySlashindeces[i] - startIndex);
                    startIndex = copySlashindeces[i] + 1;
                }
                fieldValues[i] = fieldvalue.Substring(startIndex);
            }
            else
            {
                fieldValues[0] = fieldvalue;
            }

            return fieldValues;
        }
        /// <summary>
        /// updates field code
        /// </summary>
        /// <param name="fieldCode"></param>
        override internal protected void UpdateFieldCode(string fieldCode)
        {
            bool isFieldName = true;
            string fieldvalue = UpdateFieldValue(fieldCode);
            string[] fieldValues = GetFieldValues(fieldvalue);//getting the field values            
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
                        case 'b':
                        case 'B':
                            m_textBefore = clearedValue;
                            isFieldName = false;
                            break;
                        case 'f':
                        case 'F':
                            m_textAfter = clearedValue;
                            isFieldName = false;
                            break;
                        case 'm':
                        case 'M':
                        case 'v':
                        case 'V':
                            isFieldName = false;
                            break;
                        default:
                            if(isFieldName)
                                fieldValues[0] += " \\" + fieldValues[i];
                            break;
                    }
                }
            }
            ParseFieldName(fieldValues[0]);
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        override protected internal string ConvertSwitchesToString()
        {
            string fieldSwitches = "";
            if (TextBefore != "")
            {
                fieldSwitches = "\\b \"" + TextBefore + "\"";
            }
            if (TextAfter != "")
            {
                fieldSwitches += " \\f \"" + TextAfter + "\"";
            }

            fieldSwitches += base.ConvertSwitchesToString();
            //      if( UpperCase )
            //      {
            //        fieldSwitches += @" *\ Upper";
            //      }
            //      if( LowerCase )
            //      {
            //        fieldSwitches += @" *\ Lower";
            //      }
            //      if( TitleCase )
            //      {
            //        fieldSwitches += @" *\ Caps";
            //      }
            //      if( FirstCapital )
            //      {
            //        fieldSwitches += @" *\ FirstCap";
            //      }

            return fieldSwitches;
        }
        /// <summary>
        /// Applies the base format.
        /// </summary>
        internal void ApplyBaseFormat()
        {
            for (int i = 0; i < TextItems.Count; i++)
            {
                ParagraphItem item = TextItems[i];
                item.ParaItemCharFormat.ApplyBase(CharacterFormat.BaseFormat);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Parses the name of the field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        private void ParseFieldName(string fieldName)
        {
            string[] splitFieldName = fieldName.Trim().Split(' ');
            splitFieldName[0] = splitFieldName[0].ToUpper();
            string fieldname = "";
            for (int i = 0; i < splitFieldName.Length; i++)
            {
                fieldname += splitFieldName[i] + " ";
            }
            fieldName = fieldname;
            if (fieldname.StartsWith("MERGEFIELD") && (fieldname.Contains("\\") || fieldname.Contains(":")))
            {
                ParseFieldNameHavingGroupExpression(fieldName);
            }
            else
            {
                ParseFieldNameUsingRegex(fieldName);
            }
        }
        /// <summary>
        /// Parse Field Name using regular expression
        /// </summary>
        /// <param name="fieldName">field name</param>
        private void ParseFieldNameUsingRegex(string fieldName)
        {
            Match match1 = m_regex.Match(fieldName.Trim());
            if (match1.Groups[2].Length == 0)
            {
                m_prefix = "";
                m_fieldName = match1.Groups[1].Value;
            }
            else
            {
                m_prefix = match1.Groups[1].Value;
                m_fieldName = match1.Groups[2].Value;
            }
        }
        /// <summary>
        /// Parses field name having group ":" and backslash "\" expression
        /// </summary>
        /// <param name="fieldName">field name</param>
        private void ParseFieldNameHavingGroupExpression(string fieldName)
        {
            bool isGroupField = false;
            string mergeFieldName = fieldName;
            mergeFieldName = mergeFieldName.Replace("MERGEFIELD ", string.Empty).Trim();
            if (mergeFieldName.IndexOf("\"") == 0 && mergeFieldName.LastIndexOf("\"") == mergeFieldName.Length - 1)
            {
                mergeFieldName = mergeFieldName.Remove(0, 1);
                mergeFieldName = mergeFieldName.Remove(mergeFieldName.Length - 1, 1);
            }
            if (mergeFieldName.Contains(":"))
            {
                m_prefix = mergeFieldName.Substring(0, mergeFieldName.IndexOf(":"));
                if (m_prefix == "BeginGroup" || m_prefix == "EndGroup" || m_prefix == "TableStart" || m_prefix == "TableEnd" || m_prefix == "Image")
                {
                    isGroupField = true;
                    if (!fieldName.Contains("\\"))
                    {
                        ParseFieldNameUsingRegex(fieldName);
                        return;
                    }
                }
                else
                    m_prefix = string.Empty;
            }
            if (isGroupField)
                mergeFieldName = mergeFieldName.Substring(mergeFieldName.IndexOf(":"), mergeFieldName.Length);
            m_fieldName = mergeFieldName;

        }
        /// <summary>
        /// Clears the string from other characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string ClearStringFromOtherCharacters(string value)
        {
            string str = value.Remove(0, 1);
            str = str.Trim();
            char[] chArr = new char[1] { '"' };
            return str.Trim(chArr);
        }
        /// <summary>
        /// Updates field value and format string (merge formats) by parsing through field code
        /// </summary>
        /// <param name="fieldCode"></param>
        private string UpdateFieldValue(string fieldCode)
        {
            string fieldvalue = fieldCode, format = string.Empty;
            m_formattingString = string.Empty;
            List<int> formatIndex = new List<int>();
            //Todo - In Ms Word Mailmerge happens based on Visible characters of 40 Length - Need to investigate further on this.
            //Text Format *
            while (fieldvalue.Contains("\\*"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex,"\\*");
            }
            //Number Format
            if (fieldvalue.Contains("\\#"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\#");
            }
            else if (fieldvalue.Contains("\\n"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\n");
            }
            else if (fieldvalue.Contains("\\N"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\N");
            }
            //Date Format
            if (fieldvalue.Contains("\\@"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\@");
            }
            else if (fieldvalue.Contains("\\d"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\d");
            }
            else if (fieldvalue.Contains("\\D"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\D");
            }
            else if (fieldvalue.Contains("\\"))
            {
                fieldvalue = UpdateFormatIndexAndFieldValue(fieldvalue, ref formatIndex, "\\");
            }
            formatIndex.Sort();
            for (int j = 0; j < formatIndex.Count; j++)
            {
                int length = j == formatIndex.Count - 1 ? fieldCode.Length - formatIndex[j] : formatIndex[j + 1] - formatIndex[j];
                format = fieldCode.Substring(formatIndex[j], length);
                format = format.Substring(1, format.Length - 1);
                if (format.Contains("\\"))
                {
                    format = format.Substring(0, format.IndexOf("\\"));
                }
                ParseSwitches(format);
            }
            return fieldvalue;
        }
        /// <summary>
        /// Parse switches (Text format, Number format and Date format)
        /// </summary>
        /// <param name="currValue">merge format</param>
        private void ParseSwitches(string mergeFormat)
        {
            string clearedValue = string.Empty;
            if (mergeFormat.Length > 0)
            {
                clearedValue = ClearStringFromOtherCharacters(mergeFormat);
                char firstChar = mergeFormat[0];
                switch (firstChar)
                {
                    case '#':
                    case 'n':
                    case 'N':
                        m_numberFormat = clearedValue;
                        break;
                    case '@':
                    case 'd':
                    case 'D':
                        m_dateFormat = clearedValue;
                        break;
                    case '*':
                        switch (clearedValue)
                        {
                            case "Upper":
                                m_textFormat = TextFormat.Uppercase;
                                break;
                            case "Lower":
                                m_textFormat = TextFormat.Lowercase;
                                break;
                            case "Caps":
                                m_textFormat = TextFormat.Titlecase;
                                break;
                            case "FirstCap":
                                m_textFormat = TextFormat.FirstCapital;
                                break;
                            default:
                                m_formattingString += " \\" + mergeFormat;
                                break;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Updates field Value by removing parsed switches using format index collection
        /// </summary>
        /// <param name="fieldValue">field value</param>
        /// <param name="formatIndex">format index</param>
        /// <returns></returns>
        private string UpdateFieldValue(string fieldValue,List<int> formatIndex)
        {
            int i = fieldValue.Substring(formatIndex[formatIndex.Count - 1] + 1).IndexOf("\\");
            if (i == -1)
                fieldValue = fieldValue.Substring(0, formatIndex[formatIndex.Count - 1]);
            else
                fieldValue = fieldValue.Remove(formatIndex[formatIndex.Count - 1], i);
            return fieldValue;
        }
        /// <summary>
        /// Updates format Index and Field value
        /// </summary>
        /// <param name="fieldvalue">Field Value</param>
        /// <param name="formatIndex">Format Index (list)</param>
        /// <param name="mergeSwitch">Merge Switch</param>
        /// <returns></returns>
        private string UpdateFormatIndexAndFieldValue(string fieldvalue, ref List<int> formatIndex, string mergeSwitch)
        {
            int i = fieldvalue.LastIndexOf(mergeSwitch);
            int count = 0;
            char[] specialCases = {'b', 'B','f', 'F', 'm', 'M', 'v', 'V'};
            while (fieldvalue[i] == '\\' && i >= 0)
            {
                i--;
                count++;
            }
            if (mergeSwitch == "\\")
                foreach (char character in specialCases)
                {
                    if (fieldvalue[fieldvalue.LastIndexOf("\\") + 1] == character)
                        return fieldvalue;
                }
            //For Odd case
            if (count % 2 != 0)
            {                
                formatIndex.Add(fieldvalue.LastIndexOf(mergeSwitch));
                fieldvalue = UpdateFieldValue(fieldvalue, formatIndex);
            }
            return fieldvalue;
        }
        #endregion
    }
}