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
using System.Collections.Generic;
#endregion 
  
namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FieldTypeDefiner.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FieldTypeDefiner
    {
        #region Class members
        [ThreadStatic]
        private static Dictionary<string, FieldType> m_hashStrType;
        [ThreadStatic]
        private static Dictionary<FieldType, string> m_hashTypeStr;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal static Dictionary<string, FieldType> StrTypeTable
        {
            get
            {
                if (m_hashStrType == null)
                {
                    InitStrTypeHash();
                }

                return m_hashStrType;
            }
        }
        /// <summary>
        /// Gets the type STR table.
        /// </summary>
        /// <value>The type STR table.</value>
        internal static Dictionary<FieldType, string> TypeStrTable
        {
            get
            {
                if (m_hashTypeStr == null)
                {
                    InitTypeStrHash();
                }

                return m_hashTypeStr;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal FieldTypeDefiner()
        { }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldCode"></param>
        /// <returns></returns>
        internal static FieldType GetFieldType(string fieldCode)
        {
            //Both (char) 32 and 160 represents space
            char[] separator = new char[3] {(char)32,(char)160, '"'};
            string[] stringArr = fieldCode.TrimStart(new char[0]).Split(separator);
            if (stringArr.Length == 0)
            {
                throw new Exception(string.Format("Specified fieldcode is not valid.", fieldCode));
            }
            string code = stringArr[0].ToUpper();

            if (code.StartsWith("="))
                return FieldType.FieldExpression;

            if (StrTypeTable.ContainsKey(code))
            {
                return StrTypeTable[code];
            }
            return FieldType.FieldUnknown;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        internal static string GetFieldCode(FieldType fieldType)
        {
            //      foreach( DictionaryEntry entry in StrTypeTable )
            //	    {
            //	      if( ( FieldType )entry.Value == fieldType )
            //	        return " " + ( string )entry.Key;
            //	    }
            //	    return null;
            if (TypeStrTable.ContainsKey(fieldType))
            {
                return TypeStrTable[fieldType];
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld"></param>
        /// <returns></returns>
        internal static bool IsFormField(FieldDescriptor fld)
        {
            if (fld == null)
            {
                return false;
            }
            return (fld.Type == FieldType.FieldFormCheckBox || fld.Type == FieldType.FieldFormDropDown || fld.Type == FieldType.FieldFormTextInput);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private static void InitStrTypeHash()
        {
            m_hashStrType = new Dictionary<string, FieldType>();
            m_hashStrType.Add("=", FieldType.FieldFormula);
            m_hashStrType.Add("ADVANCE", FieldType.FieldAdvance);
            m_hashStrType.Add("ASK", FieldType.FieldAsk);
            m_hashStrType.Add("AUTHOR", FieldType.FieldAuthor);
            m_hashStrType.Add("AUTONUM", FieldType.FieldAutoNum);
            m_hashStrType.Add("AUTONUMLGL", FieldType.FieldAutoNumLegal);
            m_hashStrType.Add("AUTONUMOUT", FieldType.FieldAutoNumOutline);
            m_hashStrType.Add("AUTOTEXT", FieldType.FieldAutoText);
            m_hashStrType.Add("AUTOTEXTLIST", FieldType.FieldAutoTextList);
            m_hashStrType.Add("BARCODE", FieldType.FieldBarCode);
            m_hashStrType.Add("COMMENTS", FieldType.FieldComments);
            m_hashStrType.Add("COMPARE", FieldType.FieldCompare);
            m_hashStrType.Add("CREATEDATE", FieldType.FieldCreateDate);
            m_hashStrType.Add("DATABASE", FieldType.FieldDatabase);
            m_hashStrType.Add("DATE", FieldType.FieldDate);
            m_hashStrType.Add("DOCPROPERTY", FieldType.FieldDocProperty);
            m_hashStrType.Add("DOCVARIABLE", FieldType.FieldDocVariable);
            m_hashStrType.Add("EDITTIME", FieldType.FieldEditTime);
            m_hashStrType.Add("EQ", FieldType.FieldExpression);
            m_hashStrType.Add("FILENAME", FieldType.FieldFileName);
            m_hashStrType.Add("FILESIZE", FieldType.FieldFileSize);
            m_hashStrType.Add("FILLIN", FieldType.FieldFillIn);
            m_hashStrType.Add("GOTOBUTTON", FieldType.FieldGoToButton);
            m_hashStrType.Add("HYPERLINK", FieldType.FieldHyperlink);
            m_hashStrType.Add("IF", FieldType.FieldIf);
            m_hashStrType.Add("INCLUDETEXT", FieldType.FieldIncludeText);
            m_hashStrType.Add("INCLUDEPICTURE", FieldType.FieldIncludePicture);
            m_hashStrType.Add("INDEX", FieldType.FieldIndex);
            m_hashStrType.Add("INFO", FieldType.FieldInfo);
            m_hashStrType.Add("KEYWORDS", FieldType.FieldKeyWord);
            m_hashStrType.Add("LASTSAVEDBY", FieldType.FieldLastSavedBy);
            m_hashStrType.Add("LINK", FieldType.FieldLink);
            m_hashStrType.Add("LISTNUM", FieldType.FieldListNum);
            m_hashStrType.Add("MACROBUTTON", FieldType.FieldMacroButton);
            m_hashStrType.Add("MERGEFIELD", FieldType.FieldMergeField);
            m_hashStrType.Add("MERGEREC", FieldType.FieldMergeRec);
            m_hashStrType.Add("MERGESEQ", FieldType.FieldMergeSeq);
            m_hashStrType.Add("NEXT", FieldType.FieldNext);
            m_hashStrType.Add("NEXTIF", FieldType.FieldNextIf);
            m_hashStrType.Add("NOTEREF", FieldType.FieldNoteRef);
            m_hashStrType.Add("NUMCHARS", FieldType.FieldNumChars);
            m_hashStrType.Add("NUMPAGES", FieldType.FieldNumPages);
            m_hashStrType.Add("NUMWORDS", FieldType.FieldNumWords);
            m_hashStrType.Add("PAGE", FieldType.FieldPage);
            m_hashStrType.Add("PAGEREF", FieldType.FieldPageRef);
            m_hashStrType.Add("PRINT", FieldType.FieldPrint);
            m_hashStrType.Add("PRINTDATE", FieldType.FieldPrintDate);
            m_hashStrType.Add("PRIVATE", FieldType.FieldPrivate);
            m_hashStrType.Add("QUOTE", FieldType.FieldQuote);
            m_hashStrType.Add("REF", FieldType.FieldRef);
            m_hashStrType.Add("RD", FieldType.FieldRefDoc);
            m_hashStrType.Add("REVNUM", FieldType.FieldRevisionNum);
            m_hashStrType.Add("SAVEDATE", FieldType.FieldSaveDate);
            m_hashStrType.Add("SECTION", FieldType.FieldSection);
            m_hashStrType.Add("SECTIONPAGES", FieldType.FieldSectionPages);
            m_hashStrType.Add("SEQ", FieldType.FieldSequence);
            m_hashStrType.Add("SET", FieldType.FieldSet);
            m_hashStrType.Add("SKIPIF", FieldType.FieldSkipIf);
            m_hashStrType.Add("STYLEREF", FieldType.FieldStyleRef);
            m_hashStrType.Add("SUBJECT", FieldType.FieldSubject);
            m_hashStrType.Add("SYMBOL", FieldType.FieldSymbol);
            m_hashStrType.Add("TEMPLATE", FieldType.FieldTemplate);
            m_hashStrType.Add("TIME", FieldType.FieldTime);
            m_hashStrType.Add("TITLE", FieldType.FieldTitle);
            m_hashStrType.Add("TOA", FieldType.FieldTOA);
            m_hashStrType.Add("TA", FieldType.FieldTOAEntry);
            m_hashStrType.Add("TOC", FieldType.FieldTOC);
            m_hashStrType.Add("TC", FieldType.FieldTOCEntry);
            m_hashStrType.Add("USERADDRESS", FieldType.FieldUserAddress);
            m_hashStrType.Add("USERINITIALS", FieldType.FieldUserInitials);
            m_hashStrType.Add("USERNAME", FieldType.FieldUserName);
            m_hashStrType.Add("XE", FieldType.FieldIndexEntry);
            m_hashStrType.Add("SHAPE", FieldType.FieldShape);
            m_hashStrType.Add("ADDIN", FieldType.FieldAddin);

            m_hashStrType.Add("FORMCHECKBOX", FieldType.FieldFormCheckBox);
            m_hashStrType.Add("FORMDROPDOWN", FieldType.FieldFormDropDown);
            m_hashStrType.Add("FORMTEXT", FieldType.FieldFormTextInput);
            m_hashStrType.Add("CONTROL", FieldType.FieldOCX);
            m_hashStrType.Add("EMBED", FieldType.FieldEmbed);

            m_hashStrType.Add("ADDRESSBLOCK", FieldType.FieldAddressBlock);
            m_hashStrType.Add("BIDIOUTLINE", FieldType.FieldBidiOutline);

        }
        /// <summary>
        /// 
        /// </summary>
        private static void InitTypeStrHash()
        {
            if (m_hashStrType != null)
            {
                m_hashStrType.Clear();
                m_hashStrType = null;
            }

            m_hashTypeStr = new Dictionary<FieldType, string>();
            m_hashTypeStr.Add(FieldType.FieldFormula, "=");
            m_hashTypeStr.Add(FieldType.FieldAdvance, "ADVANCE");
            m_hashTypeStr.Add(FieldType.FieldAsk, "ASK");
            m_hashTypeStr.Add(FieldType.FieldAuthor, "AUTHOR");
            m_hashTypeStr.Add(FieldType.FieldAutoNum, "AUTONUM");
            m_hashTypeStr.Add(FieldType.FieldAutoNumLegal, "AUTONUMLGL");
            m_hashTypeStr.Add(FieldType.FieldAutoNumOutline, "AUTONUMOUT");
            m_hashTypeStr.Add(FieldType.FieldAutoText, "AUTOTEXT");
            m_hashTypeStr.Add(FieldType.FieldAutoTextList, "AUTOTEXTLIST");
            m_hashTypeStr.Add(FieldType.FieldBarCode, "BARCODE");
            m_hashTypeStr.Add(FieldType.FieldComments, "COMMENTS");
            m_hashTypeStr.Add(FieldType.FieldCompare, "COMPARE");
            m_hashTypeStr.Add(FieldType.FieldCreateDate, "CREATEDATE");
            m_hashTypeStr.Add(FieldType.FieldDatabase, "DATABASE");
            m_hashTypeStr.Add(FieldType.FieldDate, "DATE");
            m_hashTypeStr.Add(FieldType.FieldDocProperty, "DOCPROPERTY");
            m_hashTypeStr.Add(FieldType.FieldDocVariable, "DOCVARIABLE");
            m_hashTypeStr.Add(FieldType.FieldEditTime, "EDITTIME");
            m_hashTypeStr.Add(FieldType.FieldExpression, "EQ");
            m_hashTypeStr.Add(FieldType.FieldFileName, "FILENAME");
            m_hashTypeStr.Add(FieldType.FieldFileSize, "FILESIZE");
            m_hashTypeStr.Add(FieldType.FieldFillIn, "FILLIN");
            m_hashTypeStr.Add(FieldType.FieldGoToButton, "GOTOBUTTON");
            m_hashTypeStr.Add(FieldType.FieldHyperlink, "HYPERLINK");
            m_hashTypeStr.Add(FieldType.FieldIf, "IF");
            m_hashTypeStr.Add(FieldType.FieldIncludeText, "INCLUDETEXT");
            m_hashTypeStr.Add(FieldType.FieldIncludePicture, "INCLUDEPICTURE");
            m_hashTypeStr.Add(FieldType.FieldIndex, "INDEX");
            m_hashTypeStr.Add(FieldType.FieldInfo, "INFO");
            m_hashTypeStr.Add(FieldType.FieldKeyWord, "KEYWORDS");
            m_hashTypeStr.Add(FieldType.FieldLastSavedBy, "LASTSAVEDBY");
            m_hashTypeStr.Add(FieldType.FieldLink, "LINK");
            m_hashTypeStr.Add(FieldType.FieldListNum, "LISTNUM");
            m_hashTypeStr.Add(FieldType.FieldMacroButton, "MACROBUTTON");
            m_hashTypeStr.Add(FieldType.FieldMergeField, "MERGEFIELD");
            m_hashTypeStr.Add(FieldType.FieldMergeRec, "MERGEREC");
            m_hashTypeStr.Add(FieldType.FieldMergeSeq, "MERGESEQ");
            m_hashTypeStr.Add(FieldType.FieldNext, "NEXT");
            m_hashTypeStr.Add(FieldType.FieldNextIf, "NEXTIF");
            m_hashTypeStr.Add(FieldType.FieldNoteRef, "NOTEREF");
            m_hashTypeStr.Add(FieldType.FieldNumChars, "NUMCHARS");
            m_hashTypeStr.Add(FieldType.FieldNumPages, "NUMPAGES");
            m_hashTypeStr.Add(FieldType.FieldNumWords, "NUMWORDS");
            m_hashTypeStr.Add(FieldType.FieldPage, "PAGE");
            m_hashTypeStr.Add(FieldType.FieldPageRef, "PAGEREF");
            m_hashTypeStr.Add(FieldType.FieldPrint, "PRINT");
            m_hashTypeStr.Add(FieldType.FieldPrintDate, "PRINTDATE");
            m_hashTypeStr.Add(FieldType.FieldPrivate, "PRIVATE");
            m_hashTypeStr.Add(FieldType.FieldQuote, "QUOTE");
            m_hashTypeStr.Add(FieldType.FieldRef, "REF");
            m_hashTypeStr.Add(FieldType.FieldRefDoc, "RD");
            m_hashTypeStr.Add(FieldType.FieldRevisionNum, "REVNUM");
            m_hashTypeStr.Add(FieldType.FieldSaveDate, "SAVEDATE");
            m_hashTypeStr.Add(FieldType.FieldSection, "SECTION");
            m_hashTypeStr.Add(FieldType.FieldSectionPages, "SECTIONPAGES");
            m_hashTypeStr.Add(FieldType.FieldSequence, "SEQ");
            m_hashTypeStr.Add(FieldType.FieldSet, "SET");
            m_hashTypeStr.Add(FieldType.FieldSkipIf, "SKIPIF");
            m_hashTypeStr.Add(FieldType.FieldStyleRef, "STYLEREF");
            m_hashTypeStr.Add(FieldType.FieldSubject, "SUBJECT");
            m_hashTypeStr.Add(FieldType.FieldSymbol, "SYMBOL");
            m_hashTypeStr.Add(FieldType.FieldTemplate, "TEMPLATE");
            m_hashTypeStr.Add(FieldType.FieldTime, "TIME");
            m_hashTypeStr.Add(FieldType.FieldTitle, "TITLE");
            m_hashTypeStr.Add(FieldType.FieldTOA, "TOA");
            m_hashTypeStr.Add(FieldType.FieldTOAEntry, "TA");
            m_hashTypeStr.Add(FieldType.FieldTOC, "TOC");
            m_hashTypeStr.Add(FieldType.FieldTOCEntry, "TC");
            m_hashTypeStr.Add(FieldType.FieldUserAddress, "USERADDRESS");
            m_hashTypeStr.Add(FieldType.FieldUserInitials, "USERINITIALS");
            m_hashTypeStr.Add(FieldType.FieldUserName, "USERNAME");
            m_hashTypeStr.Add(FieldType.FieldIndexEntry, "XE");
            m_hashTypeStr.Add(FieldType.FieldShape, "SHAPE");
            m_hashTypeStr.Add(FieldType.FieldAddin, "ADDIN");

            m_hashTypeStr.Add(FieldType.FieldFormCheckBox, "FORMCHECKBOX");
            m_hashTypeStr.Add(FieldType.FieldFormDropDown, "FORMDROPDOWN");
            m_hashTypeStr.Add(FieldType.FieldFormTextInput, "FORMTEXT");
            m_hashTypeStr.Add(FieldType.FieldOCX, "CONTROL");
            m_hashTypeStr.Add(FieldType.FieldEmbed, "EMBED");
        }
        #endregion
    }
}
