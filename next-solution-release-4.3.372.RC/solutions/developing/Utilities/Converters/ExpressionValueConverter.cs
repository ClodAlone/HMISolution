using System;
using System.Collections.Generic;
#if !NET_STANDARD
using System.Windows.Data;
using System.Windows.Controls;
#endif
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Functions;
using System.Text.RegularExpressions;

namespace Utilities.Converters
{
    public enum ExpressionType
    {
        none,
        Bit,
        Array,
        ArrayPlusBit,
        Mask,
        Expression,
        error
    }

    [Serializable]
    public struct ConverterValue : IFormattable
    {
        #region Declarations
        string value;
        int indexArray;
        int indexBit;
        #endregion

        #region Constructors
        public ConverterValue(string value)
        {
            this.value = value;
            this.indexArray = -1;
            this.indexBit = -1;
        }

        public ConverterValue(string value, int indexArray)
        {
            this.value = value;
            this.indexArray = indexArray;
            this.indexBit = -1;
        }

        public ConverterValue(string value, int indexArray, int indexBit)
        {
            this.value = value;
            this.indexArray = indexArray;
            this.indexBit = indexBit;
        }
        #endregion

        #region Public Properties
        public String Value
        {
            get
            {
                return value;
            }
        }

        public int IndexArray
        {
            get
            {
                return indexArray;
            }
        }

        public int IndexBit
        {
            get
            {
                return indexBit;
            }
        }
        #endregion

        #region Overridden Methods
        public override string ToString()
        {
            return ToString(null, null);
        }
        #endregion

        #region IFormattable Members
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return String.Format(formatProvider, "{0}:{1}:{2}", value ?? String.Empty, indexArray, indexBit);
            }

            throw new FormatException(String.Format(Properties.Resources.InvalidFormatString, format));
        }
        #endregion

        #region Static Members
        public static ConverterValue Empty
        {
            get { return s_Empty; }
        }

        private static readonly ConverterValue s_Empty = new ConverterValue(String.Empty, -1, -1);

        public static bool TryParse(string textToParse, out ConverterValue convertedValue)
        {
            try
            {
                convertedValue = Parse(textToParse);
                return true;
            }
            catch (Exception ex)
            {
                convertedValue = ConverterValue.Empty;
                return false;
            }
        }

        public static ConverterValue Parse(string textToParse)
        {
            string value = textToParse;
            int indexArray = -1;
            int indexBit = -1;
            if (!String.IsNullOrEmpty(textToParse))
            {
                var index1 = textToParse.IndexOf(':');
                if (index1 != -1)
                {
                    value = textToParse.Substring(0, index1);
                    var index2 = textToParse.IndexOf(':', index1 + 1);
                    if (index2 != -1)
                    {
                        indexArray = Convert.ToInt32(textToParse.Substring(index1 + 1, index2 - index1 - 1), System.Globalization.CultureInfo.InvariantCulture);
                        indexBit = Convert.ToInt32(textToParse.Substring(index2 + 1), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }

            return new ConverterValue(value, indexArray, indexBit);
        }
        #endregion
    }

    public class CombiningConverter : IValueConverter, IDisposable
    {
        public IValueConverter Converter1 { get; set; }
        public IValueConverter Converter2 { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
#if WINDOWS_UWP            
            String culture)
#else
            System.Globalization.CultureInfo culture)
#endif
        {
            object convertedValue = Converter2.Convert(value, targetType, parameter, culture);
            return Converter1.Convert(convertedValue, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
#if WINDOWS_UWP
            String culture)
#else
            System.Globalization.CultureInfo culture)
#endif
        {
            object convertedValue = Converter1.ConvertBack(value, targetType, parameter, culture);
            return Converter2.ConvertBack(convertedValue, targetType, parameter, culture);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (Converter1 is IDisposable)
                (Converter1 as IDisposable).Dispose();
            if (Converter2 is IDisposable)
                (Converter2 as IDisposable).Dispose();
        }
        #endregion
    }

    /// <summary>
    /// A Value converter
    /// </summary>
    public class ExpressionValueConverter : IExpressionValueConverter
    {
        #region Declarations
        long lastValue;
        object lastValueObject;
        int index;
        int indexBit;
        ExpressionType expressionType;
        List<String> listVariables;
        List<String> listReverseVariables;
        String parserError;
        String sReverseFormula;
        String sReverseFormulaParsed;
        String sFormula;
        String sFormulaParsed;
        long maskNumber;
        bool bThrowExceptions;
        bool bAdjustOutOfRangeArrayIndex;
        bool bDisposeCalcEngine;

        /*
        static object staticLock = new object();
        static List<String> referenceNodeIds;
        static Dictionary<String, long> updateMasks;
        String referenceNodeId;
        */

        const String expressionTag = "=";
        const String bitTag = ".";
        const String maskTag = "0x";
        const char bitTagChar = '.';
        internal const String xTag = "X";
        internal const String openTag = "[";
        internal const String closeTag = "]";
        internal const String openStringTag = "'[";
        internal const String closeStringTag = "]'";
        const char openArrayTagChar = '[';
        const char closeArrayTagChar = ']';
        const String openArrayValue = "{";
        const String closeArrayValue = "}";
        const String separatorArrayValue = " |";
        const String openArrayIndex = "(";
        const String closeArrayIndex = ")";

        const String validCharsConstantName = @"[^a-zA-Z0-9_.]+";
        #endregion

        #region Constructors
        public ExpressionValueConverter() :
            this(null, null)
        { }

        public ExpressionValueConverter(string formula) :
            this(formula, null)
        { }

        public ExpressionValueConverter(string formula, string reverseformula)
        {
            sFormula = formula;
            sReverseFormula = reverseformula;
        }
        #endregion

        #region Events
        public event EventHandler ParserErrorEvent;
        void OnParserError()
        {
            var e = ParserErrorEvent;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        #endregion

        #region Properties
        IWorkbook calcEngine;
        public IWorkbook CalcEngine
        {
            get
            {
                return calcEngine;
            }
            set
            {
                if (calcEngine == value)
                    return;
                else if (calcEngine != null)
                    calcEngine.Dispose();
                calcEngine = value;
                bDisposeCalcEngine = false;
            }
        }

        public String ReverseFormula
        {
            get
            {
                return sReverseFormula;
            }
            set
            {
                if (sReverseFormula == value)
                    return;
                sReverseFormula = value;
                sReverseFormulaParsed = null;
            }
        }

        public String Formula
        {
            get
            {
                return sFormula;
            }
            set
            {
                if (sFormula == value)
                    return;
                sFormula = value;
                sFormulaParsed = null;
                listVariables = null;
            }
        }

        public bool ThrowExceptions
        {
            get
            {
                return bThrowExceptions;
            }
            set
            {
                if (bThrowExceptions == value)
                    return;
                bThrowExceptions = value;
            }
        }

        public bool AdjustOutOfRangeArrayIndex
        {
            get
            {
                return bAdjustOutOfRangeArrayIndex;
            }
            set
            {
                if (bAdjustOutOfRangeArrayIndex == value)
                    return;
                bAdjustOutOfRangeArrayIndex = value;
            }
        }

        Dictionary<String, String> mapCurrentParameteItems;
        public Dictionary<String, String> MapCurrentParameteItems
        {
            get
            {
                return mapCurrentParameteItems;
            }
            set
            {
                if (value == null)
                {
                    mapCurrentParameteItems = value;
                    return;
                }

                if (mapCurrentParameteItems == null)
                    mapCurrentParameteItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                else
                    mapCurrentParameteItems.Clear();
                char separator = ':';
                String rootTags = "Tags/";
                foreach (var pair in value)
                {
                    if (!mapCurrentParameteItems.ContainsKey(pair.Key))
                        mapCurrentParameteItems.Add(pair.Key, pair.Value);
                    int nFound = pair.Key.LastIndexOf(separator);
                    /*
                    if (nFound != -1)
                    {
                        var key = pair.Key.Substring(nFound + 1);
                        var val = pair.Value;
                        nFound = pair.Value.LastIndexOf(separator);
                        if (nFound != -1)
                            val = val.Substring(nFound + 1);
                        if (!mapCurrentParameteItems.ContainsKey(key))
                            mapCurrentParameteItems.Add(key, val);
                    }
                    */
                    if (nFound > 0)
                    {
                        var toReplace1 = String.Format("{0}{1}", pair.Key[nFound - 1], separator);

                        var key = pair.Key.Replace(toReplace1, "");
                        if (key.StartsWith(rootTags))
                            key = key.Remove(0, rootTags.Length);
                        var val = pair.Value.Replace("\\", "/");
                        //var val = pair.Value.Replace(toReplace1, "");
                        //if (val.StartsWith(rootTags))
                        //    val = val.Remove(0, rootTags.Length);
                        if (!mapCurrentParameteItems.ContainsKey(key))
                            mapCurrentParameteItems.Add(key, val);

                        key = key.Replace("/", "\\");
                        if (!mapCurrentParameteItems.ContainsKey(key))
                            mapCurrentParameteItems.Add(key, val);
                    }
                }
            }
        }

        public ExpressionType Type
        {
            get
            {
                return expressionType;
            }
        }

        string ParserError
        {
            get
            {
                return parserError;
            }
            set
            {
                if (parserError == value)
                    return;
                parserError = value;

                if (!string.IsNullOrEmpty(parserError))
                    OnParserError();
            }
        }
        #endregion

        #region Methods
        public String GetParserError()
        {
            return ParserError;
        }

        public static IWorkbook CreateCalcEngine()
        {
            return new WorkbookExtended();
        }

        public List<string> GetListVarInExpression(string expression)
        {
            Formula = expression;
            ParseFormula();
            var error = GetParserError();
            if (String.IsNullOrEmpty(error))
            {
                return GetFormulaVariables(expression);
            }
            return new List<string>();
        }

        public static ExpressionType GetFormulaType(String formula)
        {
            int arrayIndex;
            int bitNumber;

            return GetFormulaType(formula, out arrayIndex, out bitNumber);
        }

        public static ExpressionType GetFormulaType(String formula, out int arrayIndex, out int bitNumber)
        {
            arrayIndex = -1;
            bitNumber = -1;

            long mask;
            if (GetMaskNumber(formula, out mask))
                return ExpressionType.Mask;
            bitNumber = GetBitNumber(formula);
            if (bitNumber >= 0)
                return ExpressionType.Bit;
            string extended;
            arrayIndex = GetArrayIndexNumber(formula, out extended);
            if (arrayIndex >= 0)
            {
                if (!String.IsNullOrWhiteSpace(extended))
                {
                    bitNumber = GetBitNumber(extended);
                    if (bitNumber >= 0)
                        return ExpressionType.ArrayPlusBit;
                }
                else
                    return ExpressionType.Array;
            }
            if (formula.StartsWith(expressionTag))
                return ExpressionType.Expression;

            return ExpressionType.none;
        }

        public static string GetArrayFormula(int index)
        {
            return String.Format("{0}{1}{2}", openArrayTagChar, index, closeArrayTagChar);
        }

        public List<String> GetAllParsedVariables()
        {
            var ret = new List<String>();

            if (listVariables != null)
            {
                if (mapCurrentParameteItems == null || mapCurrentParameteItems.Count == 0)
                {
                    foreach (var var in listVariables)
                    {
                        string varName;
                        GetArrayIndex(var, out varName);
                        if (!String.Equals(varName, xTag, StringComparison.OrdinalIgnoreCase) && !ret.Contains(varName))
                            ret.Add(varName);
                    }
                }
                else
                {
                    foreach (var var in listVariables)
                    {
                        string varName;
                        GetArrayIndex(var, out varName);
                        if (!String.Equals(varName, xTag, StringComparison.OrdinalIgnoreCase))
                        {
                            bool bFound = false;
                            if (mapCurrentParameteItems.ContainsKey(varName) && !ret.Contains(mapCurrentParameteItems[varName]))
                                ret.Add(mapCurrentParameteItems[varName]);
                            else if (!mapCurrentParameteItems.ContainsKey(varName))
                            {
                                var path = NamespaceTableConverter.GetSanitizedReadableValue(varName);
                                var index = path.LastIndexOf('\\');
                                while (index > 0)
                                {
                                    var key = String.Format("{0}{1}", path.Substring(0, index), NamespaceTableConverter.WholeFolderWildChar);
                                    if (mapCurrentParameteItems.ContainsKey(key))
                                    {
                                        var newVarName = mapCurrentParameteItems[key];
                                        var searchPath = NamespaceTableConverter.GetRelativePathValue(key);
                                        var replacePath = NamespaceTableConverter.GetRelativePathValue(mapCurrentParameteItems[key]);
                                        if (searchPath != replacePath)
                                            newVarName = Regex.Replace(varName, Regex.Escape(searchPath), replacePath, RegexOptions.IgnoreCase);
                                        mapCurrentParameteItems.Add(varName, newVarName);
                                        if (!ret.Contains(newVarName))
                                            ret.Add(newVarName);
                                        bFound = true;
                                        break;
                                    }
                                    index = path.LastIndexOf('\\', index - 1);
                                }
                            }

                            if (!bFound && !ret.Contains(varName))
                                ret.Add(varName);
                        }
                    }
                }

            }
            if (listReverseVariables != null)
            {
                if (mapCurrentParameteItems == null || mapCurrentParameteItems.Count == 0)
                    foreach (var var in listReverseVariables)
                    {
                        string varName;
                        GetArrayIndex(var, out varName);
                        if (!String.Equals(varName, xTag, StringComparison.OrdinalIgnoreCase) && !ret.Contains(varName))
                            ret.Add(varName);
                    }
                else
                {
                    foreach (var var in listReverseVariables)
                    {
                        string varName;
                        GetArrayIndex(var, out varName);
                        if (!String.Equals(varName, xTag, StringComparison.OrdinalIgnoreCase))
                        {
                            bool bFound = false;
                            if (mapCurrentParameteItems.ContainsKey(varName) && !ret.Contains(mapCurrentParameteItems[varName]))
                                ret.Add(mapCurrentParameteItems[varName]);
                            else if (!mapCurrentParameteItems.ContainsKey(varName))
                            {
                                var path = NamespaceTableConverter.GetSanitizedReadableValue(varName);
                                var index = path.LastIndexOf('\\');
                                while (index > 0)
                                {
                                    var key = String.Format("{0}{1}", path.Substring(0, index), NamespaceTableConverter.WholeFolderWildChar);
                                    if (mapCurrentParameteItems.ContainsKey(key))
                                    {
                                        var newVarName = mapCurrentParameteItems[key];
                                        var searchPath = NamespaceTableConverter.GetRelativePathValue(key);
                                        var replacePath = NamespaceTableConverter.GetRelativePathValue(mapCurrentParameteItems[key]);
                                        if (searchPath != replacePath)
                                            newVarName = Regex.Replace(varName, Regex.Escape(searchPath), replacePath, RegexOptions.IgnoreCase);
                                        mapCurrentParameteItems.Add(varName, newVarName);
                                        if (!ret.Contains(newVarName))
                                            ret.Add(newVarName);
                                        bFound = true;
                                        break;
                                    }
                                    index = path.LastIndexOf('\\', index - 1);
                                }
                            }

                            if (!bFound && !ret.Contains(varName))
                                ret.Add(varName);
                        }
                    }
                }
            }

            return ret;
        }
        public List<String> GetFormulaVariables(String formula)
        {
            return GetFormulaVariables(formula, true);
        }

        internal List<String> GetFormulaVariables(String formula, bool distinctVariables)
        {
            ParserError = String.Empty;

#if !WINDOWS_UWP
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
#endif
            var list = new List<String>();
            try
            {
                if (formula.StartsWith(expressionTag))
                {
                    if (calcEngine == null)
                    {
                        calcEngine = new WorkbookExtended();
                        bDisposeCalcEngine = true;
                    }

                    try
                    {
                        formula = PrepareParseFormula(formula);
                        calcEngine.DefinedNames.Clear();
                        var parsedExpression = calcEngine.FormulaEngine.Parse(formula);
                        var tagNameParser = new TagNamesParser(distinctVariables);
                        parsedExpression.Expression.Visit(tagNameParser);
                        if (tagNameParser.UnknowFunctions.Count > 0)
                        {
                            throw new InvalidOperationException(String.Format(Properties.Resources.UnknownFunctionExpression, String.Join(", ", tagNameParser.UnknowFunctions)));
                        }
                        return tagNameParser.TagNames;
                    }
                    catch (Exception ex)
                    {
                        ParserError = ex.Message;
                    }
                }
            }
            finally
            {
#if !WINDOWS_UWP
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
            }

            return list;
        }

        public String GetFormulaError(String formula)
        {
            if (formula.StartsWith(expressionTag))
            {
#if !WINDOWS_UWP
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
#endif
                try
                {
                    if (calcEngine == null)
                    {
                        calcEngine = new WorkbookExtended();
                        bDisposeCalcEngine = true;
                    }

                    formula = PrepareParseFormula(formula);
                    calcEngine.DefinedNames.Clear();
                    calcEngine.FormulaEngine.Parse(formula);
                    return String.Empty;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
#if !WINDOWS_UWP
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
                }
            }

            return String.Empty;
        }

        static int GetBitNumber(String parameter)
        {
            if (!parameter.StartsWith(bitTag))
                return -1;

            var bitString = parameter.Split(bitTagChar);
            try
            {
                int bitNumber = System.Convert.ToInt32(bitString[1]);
                return bitNumber;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }

        static bool GetMaskNumber(String parameter, out long mask)
        {
            mask = 0;
            if (!parameter.StartsWith(maskTag))
                return false;

            var maskString = parameter.Replace(maskTag, "");
            try
            {
                mask = System.Convert.ToInt64(maskString, 16);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        static int GetArrayIndexNumber(String parameter, out String extended)
        {
            extended = null;
            if (!parameter.StartsWith(openTag))
                return -1;

            var arrayString = parameter.Split(openArrayTagChar);
            var arrayStringIndex = arrayString[1].Split(closeArrayTagChar);
            if (arrayStringIndex.Length < 1 || arrayStringIndex[0].Length == 0 || !Char.IsDigit(arrayStringIndex[0][0]))
                return -1;

            try
            {
                int indexNumber = System.Convert.ToInt32(arrayStringIndex[0]);
                extended = arrayStringIndex[arrayStringIndex.Length - 1];
                return indexNumber;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }

        void InvalidateFormula()
        {
            ConvertingVariables = null;
            ConvertedValue = null;
            ConvertingBackVariables = null;
            ConvertedBackValue = null;

            lastValue = 0;
            lastValueObject = null;
            index = 0;
            indexBit = 0;
            expressionType = ExpressionType.none;
            listVariables = null;
            listReverseVariables = null;
            parserError = null;
            sReverseFormulaParsed = null;
            sFormulaParsed = null;
            maskNumber = 0;
        }

        public void ParseFormula()
        {
            InvalidateFormula();
            if (GetMaskNumber(sFormula, out maskNumber))
            {
                expressionType = ExpressionType.Mask;
                var binary = System.Convert.ToString(maskNumber, 2);
                index = 0;
                for (int i = binary.Length - 1; i >= 0; --i)
                {
                    if (binary[i] == '0')
                        ++index;
                    else
                        break;
                }
                return;
            }

            int bitNumber = GetBitNumber(sFormula);
            if (bitNumber >= 0)
            {
                expressionType = ExpressionType.Bit;
                index = bitNumber;
                return;
            }
            string extended;
            bitNumber = GetArrayIndexNumber(sFormula, out extended);
            if (bitNumber >= 0)
            {
                expressionType = ExpressionType.Array;
                index = bitNumber;
                if (extended != null)
                {
                    bitNumber = GetBitNumber(extended);
                    if (bitNumber >= 0)
                    {
                        expressionType = ExpressionType.ArrayPlusBit;
                        indexBit = bitNumber;
                    }
                }
                return;
            }
            if (sFormula.StartsWith(expressionTag))
            {
                expressionType = ExpressionType.Expression;
                listVariables = GetFormulaVariables(sFormula, false);
                sFormulaParsed = sFormula.Replace(String.Format("{0}{1}{2}", openStringTag, xTag, closeStringTag), GetValidConstantName(xTag, true));
                sFormulaParsed = sFormulaParsed.Replace(String.Format("{0}{1}{2}", openStringTag, xTag.ToLower(), closeStringTag), GetValidConstantName(xTag, true));
                sFormulaParsed = sFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, xTag, closeTag), GetValidConstantName(xTag));
                sFormulaParsed = sFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, xTag.ToLower(), closeTag), GetValidConstantName(xTag));
                listVariables.ForEach((tagName) =>
                {
                    sFormulaParsed = sFormulaParsed.Replace(String.Format("{0}{1}{2}", openStringTag, tagName, closeStringTag), GetValidConstantName(tagName, true));
                    sFormulaParsed = sFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, tagName, closeTag), GetValidConstantName(tagName));
                });
                if (!String.IsNullOrEmpty(ParserError))
                {
                    expressionType = ExpressionType.error;
                }
                if (!String.IsNullOrEmpty(sReverseFormula) && sReverseFormula.StartsWith(expressionTag))
                {
                    listReverseVariables = GetFormulaVariables(sReverseFormula, false);
                    sReverseFormulaParsed = sReverseFormula.Replace(String.Format("{0}{1}{2}", openStringTag, xTag, closeStringTag), GetValidConstantName(xTag, true));
                    sReverseFormulaParsed = sReverseFormulaParsed.Replace(String.Format("{0}{1}{2}", openStringTag, xTag.ToLower(), closeStringTag), GetValidConstantName(xTag, true));
                    sReverseFormulaParsed = sReverseFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, xTag, closeTag), GetValidConstantName(xTag));
                    sReverseFormulaParsed = sReverseFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, xTag.ToLower(), closeTag), GetValidConstantName(xTag));
                    listReverseVariables.ForEach((tagName) =>
                    {
                        sReverseFormulaParsed = sReverseFormulaParsed.Replace(String.Format("{0}{1}{2}", openStringTag, tagName, closeStringTag), GetValidConstantName(tagName, true));
                        sReverseFormulaParsed = sReverseFormulaParsed.Replace(String.Format("{0}{1}{2}", openTag, tagName, closeTag), GetValidConstantName(tagName));
                    });
                }
                else
                    listReverseVariables = null;
            }
        }

        string PrepareParseFormula(string formula)
        {
            return formula.Replace(openStringTag, openTag).Replace(closeStringTag, closeTag)
                .Replace(openTag, String.Format("\"{0}", openTag)).Replace(closeTag, String.Format("{0}\"", closeTag));
        }

        string GetValidConstantName(string varName, bool isString = false)
        {
            if (isString)
                return String.Format("STRING_{0}", System.Text.RegularExpressions.Regex.Replace(varName, validCharsConstantName, "_"));
            else
                return String.Format("_{0}", System.Text.RegularExpressions.Regex.Replace(varName, validCharsConstantName, "_"));
        }

        string GetValidConstantValue(string constValue, bool isString = false)
        {
            double dvalue;
            if (!isString && double.TryParse(constValue, out dvalue))
                return String.Format("{0}", constValue);
            else
                return String.Format("\"{0}\"", constValue);
        }

        /*
        public void AddReferenceNodeId(Opc.Ua.NodeId nodeId)
        {
            if (expressionType != ExpressionType.Bit && expressionType != ExpressionType.ArrayPlusBit)
                return;

            lock (staticLock)
            {
                long shift = 1;
                long mask = 0;
                if (expressionType == ExpressionType.Bit)
                {
                    referenceNodeId = nodeId.ToString();
                    mask = ~(shift << index);
                }
                else if (expressionType == ExpressionType.ArrayPlusBit)
                {
                    referenceNodeId = String.Format("{0}[{1}]", nodeId.ToString(), index);
                    mask = ~(shift << indexBit);
                }

                if (referenceNodeIds == null)
                    referenceNodeIds = new List<String>();
                referenceNodeIds.Add(referenceNodeId);
              
                if (updateMasks == null)
                    updateMasks = new Dictionary<String, long>();
                if (!updateMasks.ContainsKey(referenceNodeId))
                    updateMasks.Add(referenceNodeId, mask);
                else
                    updateMasks[referenceNodeId] &= mask;
            }
        }

        public void ClearReferenceNodeId()
        {
            lock (staticLock)
            {
                if (referenceNodeId == null || referenceNodeIds == null)
                    return;

                referenceNodeIds.Remove(referenceNodeId);
                if (!referenceNodeIds.Contains(referenceNodeId) &&
                    updateMasks != null && updateMasks.ContainsKey(referenceNodeId))
                {
                    updateMasks.Remove(referenceNodeId);
                }
                referenceNodeId = null;
            }
        }

        public void UpdateLastValue(object value)
        {
            try
            {
                var digitSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (value == null)
                    return;

                switch (expressionType)
                {
                    case ExpressionType.Mask:
                        {
                            long lValue = System.Convert.ToInt64(value);
                            lastValue = lValue;
                            break;
                        }
                    case ExpressionType.Bit:
                        {
                            long mask = -1;
                            if (referenceNodeId != null)
                            {
                                lock (staticLock)
                                {
                                    if (updateMasks.ContainsKey(referenceNodeId))
                                        mask = updateMasks[referenceNodeId];
                                }
                            }

                            long lValue = System.Convert.ToInt64(value);
                            lastValue = lValue & mask;
                            break;
                        }
                    case ExpressionType.Array:
                        {
                            if (value is Array)
                            {
                                var array = value as Array;
                                lastValueObject = array;
                            }
                            else if (value is String &&
                                ((value as String).Length > 2 && (value as String)[0] == '{' &&
                                (value as String)[(value as String).Length - 1] == '}'))
                            {
                                var str = value as String;
                                lastValueObject = str;
                            }
                            else if (value is String &&
                                (value as String).Length > 2)
                            {
                                var str = value as String;
                                lastValueObject = str;
                            }
                            break;
                        }
                    case ExpressionType.ArrayPlusBit:
                        {
                            long mask = -1;
                            if (referenceNodeId != null)
                            {
                                lock (staticLock)
                                {
                                    if (updateMasks.ContainsKey(referenceNodeId))
                                        mask = updateMasks[referenceNodeId];
                                }
                            }

                            if (value is Array)
                            {
                                var array = value as Array;
                                lastValueObject = array;
                                long lValue = System.Convert.ToInt64(array.GetValue(index));
                                lastValue = lValue & mask;
                            }
                            else if (value is String &&
                                ((value as String).Length > 2 && (value as String)[0] == '{' &&
                                (value as String)[(value as String).Length - 1] == '}'))
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = str.Substring(1, str.Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                                if (values.Length > index)
                                {
                                    long lValue = System.Convert.ToInt64(values[index].Replace(digitSeparator, "."));
                                    lastValue = lValue & mask;
                                }
                            }
                            else if (value is String &&
                                (value as String).Length > 2)
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = new String[str.Length / 2];
                                for (int ii = 0; ii < str.Length / 2; ii++)
                                    values[ii] = str.Substring(ii * 2, 2);
                                if (values.Length > index)
                                {
                                    long lValue = System.Convert.ToInt64(values[index]);
                                    lastValue = lValue & mask;
                                }
                            }
                            break;
                        }
                }
            }
            catch
            { }
        }
        */
        #endregion

        String ConvertingVariables;
        String ConvertedValue;
        String ConvertingBackVariables;
        String ConvertedBackValue;

        #region IValueConverter Members
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter,
#if WINDOWS_UWP
            String culture)
#else
            System.Globalization.CultureInfo culture)
#endif
        {
            try
            {
                if (value == null)
                    return value;

                //if (Formula == null || String.IsNullOrEmpty(Formula))
                //    throw new InvalidOperationException("Formula is required");

                var digitSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                switch (expressionType)
                {
                    case ExpressionType.Mask:
                        {
                            long lValue = System.Convert.ToInt64(value);
                            lastValue = lValue;
                            var masked = lValue & maskNumber;
                            return masked >> index;
                        }

                    case ExpressionType.Bit:
                        {
                            /*
                            long mask = -1;
                            if (referenceNodeId != null)
                            {
                                lock (staticLock)
                                {
                                    if (updateMasks.ContainsKey(referenceNodeId))
                                        mask = updateMasks[referenceNodeId];
                                }
                            }
                            */

                            long lValue = 0;
                            try
                            {
                                lValue = System.Convert.ToInt64(value);
                            }
                            catch
                            {
                                try
                                {
                                    lValue = (long)System.Convert.ToUInt64(value);
                                }
                                catch
                                {
                                    lValue = (long)System.Convert.ToDouble(value);
                                }
                            }
                            long shift = 1;
                            lastValue = lValue/* & mask*/;
                            var bit = (lValue & (shift << index)) != 0;
                            return bit;
                        }

                    case ExpressionType.Array:
                        {
                            if (value is Array)
                            {
                                var array = value as Array;
                                // if (array.Length < indexArray)
                                lastValueObject = array;
                                var ret = array.GetValue(index);
                                if (ret is float)
                                {
                                    var sVal = System.Convert.ToString(ret, culture);
                                    return System.Convert.ToDouble(sVal, culture);
                                }
                                return ret;
                            }
                            else if (value is String &&
                                ((value as String).Length > 2 && (value as String)[0] == '{' &&
                                (value as String)[(value as String).Length - 1] == '}'))
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = str.Substring(1, str.Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                                if (values.Length > index)
                                    return values[index];
                                else if (AdjustOutOfRangeArrayIndex)
                                    return String.Empty;
                                else
                                    throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);
                            }
                            else if (value is String &&
                                (value as String).Length > 2)
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = new String[str.Length / 2];
                                for (int ii = 0; ii < str.Length / 2; ii++)
                                    values[ii] = str.Substring(ii * 2, 2);
                                if (values.Length > index)
                                    return values[index];
                                else if (AdjustOutOfRangeArrayIndex)
                                    return String.Empty;
                                else
                                    throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);
                            }
                            else
                                throw new InvalidOperationException(Properties.Resources.InvalidArrayValue);
                        }
                    case ExpressionType.ArrayPlusBit:
                        {
                            /*
                            long mask = -1;
                            if (referenceNodeId != null)
                            {
                                lock (staticLock)
                                {
                                    if (updateMasks.ContainsKey(referenceNodeId))
                                        mask = updateMasks[referenceNodeId];
                                }
                            }
                            */

                            long shift = 1;
                            if (value is Array)
                            {
                                var array = value as Array;
                                // if (array.Length < indexArray)
                                lastValueObject = array;
                                long lValue = System.Convert.ToInt64(array.GetValue(index));
                                lastValue = lValue/* & mask*/;
                                var bit = (lValue & (shift << indexBit)) != 0;
                                return bit;
                            }
                            else if (value is String &&
                                ((value as String).Length > 2 && (value as String)[0] == '{' &&
                                (value as String)[(value as String).Length - 1] == '}'))
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = str.Substring(1, str.Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                                if (values.Length > index)
                                {
                                    var sValue = values[index];
                                    if (culture != System.Globalization.CultureInfo.InvariantCulture)
                                        sValue = sValue.Replace(digitSeparator, ".");
                                    long lValue = System.Convert.ToInt64(sValue);
                                    lastValue = lValue/* & mask*/;
                                    var bit = (lValue & (shift << indexBit)) != 0;
                                    return bit;
                                }
                                else if (AdjustOutOfRangeArrayIndex)
                                    return false;
                                else
                                    throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);

                            }
                            else if (value is String &&
                                (value as String).Length > 2)
                            {
                                var str = value as String;
                                lastValueObject = str;
                                var values = new String[str.Length / 2];
                                for (int ii = 0; ii < str.Length / 2; ii++)
                                    values[ii] = str.Substring(ii * 2, 2);
                                if (values.Length > index)
                                {
                                    long lValue = System.Convert.ToInt64(values[index]);
                                    lastValue = lValue/* & mask*/;
                                    var bit = (lValue & (shift << indexBit)) != 0;
                                    return bit;
                                }
                                else if (AdjustOutOfRangeArrayIndex)
                                    return false;
                                else
                                    throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);
                            }
                            else
                                throw new InvalidOperationException(Properties.Resources.InvalidArrayValue);
                        }
                    case ExpressionType.Expression:
                        {
#if !WINDOWS_UWP
                            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

                            var currentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
#endif
                            try
                            {
                                var formula = sFormulaParsed;
                                if (String.IsNullOrEmpty(formula))
                                    return value;
                                calcEngine.DefinedNames.Clear();
                                if (value is String)
                                {
                                    if (culture != System.Globalization.CultureInfo.InvariantCulture)
                                    {
                                        var str = value as String;
                                        value = str.Replace(digitSeparator, ".");
                                    }
                                }
                                var convertingCompareVariables = NormalizedBool(ConvertToString(value));
                                calcEngine.DefinedNames.Add(GetValidConstantName(xTag), GetValidConstantValue(convertingCompareVariables));
                                calcEngine.DefinedNames.Add(GetValidConstantName(xTag, true), GetValidConstantValue(convertingCompareVariables, true));
                                if (listVariables != null && listVariables.Count > 0)
                                {
                                    Dictionary<String, Object> map = null;
                                    var sourceMap = parameter as IDictionary<String, Object>;
                                    if (sourceMap != null)
                                    {
                                        map = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
                                        foreach (var pair in sourceMap)
                                        {
                                            if (map.ContainsKey(pair.Key))
                                                map.Remove(pair.Key);
                                            map.Add(pair.Key, pair.Value);
                                        }
                                        /*
                                        var map = parameter as IDictionary<String, Object>;
                                        foreach (var name in map.Keys)
                                        {
                                            var val = map[name];
                                            if (val is String)
                                            {
                                                var str = val as String;
                                                str = str.Replace(digitSeparator, ".");
                                                calcEngine[name.ToUpper()] = NormalizedBool(str);
                                            }
                                            else
                                                calcEngine[name.ToUpper()] = NormalizedBool(map[name].ToString());
                                        }
                                        */
                                    }
                                    listVariables.ForEach(var =>
                                        {
                                            var orginalVar = var;
                                            var index = GetArrayIndex(orginalVar, out var);
                                            if (index != -1 && String.Equals(var, xTag, StringComparison.OrdinalIgnoreCase))
                                            {
                                                string[] array;
                                                if (TryGetArrayValue(convertingCompareVariables, out array) && index < array.Length)
                                                {
                                                    var str = array[index];
                                                    str = NormalizedBool(str);
                                                    var definedName = GetValidConstantName(orginalVar);
                                                    if (!calcEngine.DefinedNames.Contains(definedName))
                                                        calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str));
                                                    definedName = GetValidConstantName(orginalVar, true);
                                                    if (!calcEngine.DefinedNames.Contains(definedName))
                                                        calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str, true));
                                                }
                                            }
                                            else if (map != null)
                                            {
                                                if (mapCurrentParameteItems != null &&
                                                    mapCurrentParameteItems.ContainsKey(var))
                                                    var = mapCurrentParameteItems[var];
                                                if (map.ContainsKey(var))
                                                {
                                                    var val = map[var];
                                                    if (val is String)
                                                    {
                                                        var str = val as String;
                                                        if (index != -1)
                                                        {
                                                            string[] array;
                                                            if (TryGetArrayValue(str, out array) && index < array.Length)
                                                                str = array[index];
                                                        }
                                                        str = str.Replace(digitSeparator, ".");
                                                        str = NormalizedBool(str);
                                                        var definedName = GetValidConstantName(orginalVar);
                                                        if (!calcEngine.DefinedNames.Contains(definedName))
                                                            calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str));
                                                        definedName = GetValidConstantName(orginalVar, true);
                                                        if (!calcEngine.DefinedNames.Contains(definedName))
                                                            calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str, true));
                                                        convertingCompareVariables = String.Format("{0},{1}", convertingCompareVariables, str);
                                                    }
                                                    else
                                                    {
                                                        var str = NormalizedBool(map[var].ToString());
                                                        var definedName = GetValidConstantName(orginalVar);
                                                        if (!calcEngine.DefinedNames.Contains(definedName))
                                                            calcEngine.DefinedNames.Add(definedName.ToUpper(), GetValidConstantValue(str));
                                                        definedName = GetValidConstantName(orginalVar, true);
                                                        if (!calcEngine.DefinedNames.Contains(definedName))
                                                            calcEngine.DefinedNames.Add(definedName.ToUpper(), GetValidConstantValue(str, true));
                                                        convertingCompareVariables = String.Format("{0},{1}", convertingCompareVariables, str);
                                                    }
                                                }
                                            }
                                        });
                                }

                                if (listVariables != null && listVariables.Count * 2 > calcEngine.DefinedNames.Count - 2)
                                    return null;

                                if (convertingCompareVariables == ConvertingVariables)
                                {
                                    if (culture != System.Globalization.CultureInfo.InvariantCulture)
                                        return NormalizedBoolObject(ConvertedValue.Replace(".", digitSeparator));
                                    else
                                        return NormalizedBoolObject(ConvertedValue);
                                }
                                // return System.Convert.ChangeType(calcEngine[valueTag], targetType);
                                var parameterValue = calcEngine.FormulaEngine.Evaluate(formula);
                                if (parameterValue.IsError)
                                    throw new InvalidOperationException(parameterValue.ErrorValue.Description);
                                ConvertingVariables = convertingCompareVariables;
                                if (parameterValue.IsNumeric)
                                    ConvertedValue = parameterValue.NumericValue.ToString("G17");
                                else
                                    ConvertedValue = parameterValue.ToString();
                                if (culture != System.Globalization.CultureInfo.InvariantCulture)
                                    return NormalizedBoolObject(ConvertedValue.Replace(".", digitSeparator));
                                else
                                    return NormalizedBoolObject(ConvertedValue);
                            }
                            finally
                            {
#if !WINDOWS_UWP
                                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
                                System.Threading.Thread.CurrentThread.CurrentUICulture = currentUICulture;
#endif
                            }
                        }

                    case ExpressionType.error:
                        return ParserError;
                }

                return value;
            }
            catch (Exception ex)
            {
                if (bThrowExceptions)
                    throw ex;
                return null;
            }
        }

        static String NormalizedBool(String value)
        {
            if (String.Compare(value, "FALSE", true) == 0)
                return "0";
            if (String.Compare(value, "TRUE", true) == 0)
                return "1";
            return value;
        }

        static Object NormalizedBoolObject(String value)
        {
            if (String.Compare(value, "FALSE", true) == 0)
                return 0.0;
            if (String.Compare(value, "TRUE", true) == 0)
                return 1.0;
            return value;
        }

        static bool TryGetArrayValue(String value, out String[] array)
        {
            if (!String.IsNullOrEmpty(value) && value.StartsWith(openArrayValue) && value.EndsWith(closeArrayValue))
            {
                var values = value.Substring(1, value.Length - 2);
                array = values.Split(new String[] { separatorArrayValue }, StringSplitOptions.None);
                return true;
            }
            else
            {
                array = new String[0];
                return false;
            }
        }

        internal static int GetArrayIndex(String parameter, out String varName)
        {
            int index = -1;
            varName = parameter;
            var found = parameter.IndexOf(openArrayIndex);
            if (found != -1 && parameter.EndsWith(closeArrayIndex))
            {
                if (int.TryParse(parameter.Substring(found + openArrayIndex.Length,
                    parameter.Length - found - openArrayIndex.Length - closeArrayIndex.Length), out index))
                {
                    varName = parameter.Substring(0, found);
                    return index;
                }
            }

            return index;
        }

        static string ConvertToString(object value)
        {
            if (value is Array)
            {
                var dValue = new Opc.Ua.Variant(value);
                return String.Format("{0}", dValue);
            }
            else
                return value.ToString();
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
#if WINDOWS_UWP
            String culture)
#else
            System.Globalization.CultureInfo culture)
#endif
        {
            try
            {
                if (value == null)
                    return value;

                //if (ReverseFormula == null || String.IsNullOrEmpty(ReverseFormula))
                //    throw new InvalidOperationException("ReverseFormula is required");

                var digitSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#if !WINDOWS_UWP
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
#endif
                switch (expressionType)
                {
                    case ExpressionType.Mask:
                        {
                            long lValue = System.Convert.ToInt64(value) << index;
                            lValue &= maskNumber;
                            var masked = lastValue &= ~(maskNumber);
                            return masked |= lValue;
                        }

                    case ExpressionType.Bit:
                        {
                            long shift = 1;
                            try
                            {
                                bool bValue = System.Convert.ToBoolean(value);
                                lastValue &= ~(shift << index);
                                if (bValue)
                                    lastValue |= shift << index;
                            }
                            catch
                            {
                                long nValue = System.Convert.ToInt64(value);
                                lastValue &= ~(shift << index);
                                if (nValue > 0)
                                    lastValue |= shift << index;
                            }
                            if (targetType == typeof(String))
                                return new ConverterValue(lastValue.ToString(), -1, index).ToXml();
                            else if (targetType != null)
                                return System.Convert.ChangeType(lastValue, targetType);
                            return lastValue;
                        }
                    case ExpressionType.Array:
                        {
#if !WINDOWS_UWP
                            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
#endif
                            try
                            {
                                if (parameter is String)
                                    lastValueObject = parameter as String;

                                if (lastValueObject is Array)
                                {
                                    var array = lastValueObject as Array;
                                    var v = InternalChangeType(value, array.GetType().GetElementType());
                                    // if (array.Length < indexArray)
                                    array.SetValue(v, index);
                                    return array;
                                }
                                else if (lastValueObject is String &&
                                    ((lastValueObject as String).Length > 2 && (lastValueObject as String)[0] == '{' &&
                                    (lastValueObject as String)[(lastValueObject as String).Length - 1] == '}'))
                                {
                                    var str = lastValueObject as String;
                                    var values = str.Substring(1, str.Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                                    if (AdjustOutOfRangeArrayIndex && values.Length <= index)
                                    {
                                        var newValues = new string[index + 1];
                                        Array.Copy(values, newValues, values.Length);
                                        values = newValues;
                                    }
                                    if (values.Length > index)
                                        values.SetValue(System.Convert.ChangeType(value, values[0].GetType()), index);
                                    else
                                        throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);

                                    if (targetType == typeof(String))
                                        return new ConverterValue(String.Format("{0}{1}{2}", '{', String.Join(" |", values), '}'), index, -1).ToXml();
                                    else
                                        return String.Format("{0}{1}{2}", '{', String.Join(" |", values), '}');
                                }
                                else if (lastValueObject is String &&
                                    (lastValueObject as String).Length > 2)
                                {
                                    var str = lastValueObject as String;
                                    if (parameter is String)
                                        str = parameter as String;
                                    var values = new String[str.Length / 2];
                                    for (int ii = 0; ii < str.Length / 2; ii++)
                                        values[ii] = str.Substring(ii * 2, 2);
                                    if (AdjustOutOfRangeArrayIndex && values.Length <= index)
                                    {
                                        var newValues = new string[index + 1];
                                        Array.Copy(values, newValues, values.Length);
                                        values = newValues;
                                    }
                                    if (values.Length > index)
                                    {
                                        var set = value.ToString();
                                        if (set.Length > 2)
                                            set = set.Substring(0, 2);
                                        set = set.Replace(".", "");
                                        values.SetValue(set.PadLeft(2, '0'), index);
                                    }
                                    else
                                        throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);

                                    if (targetType == typeof(String))
                                        return new ConverterValue(String.Join(String.Empty, values), index, -1).ToXml();
                                    else
                                        return String.Join(String.Empty, values);
                                }
                                else
                                    throw new InvalidOperationException(Properties.Resources.InvalidArrayValue);
                            }
                            finally
                            {
#if !WINDOWS_UWP
                                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
                            }
                        }
                    case ExpressionType.ArrayPlusBit:
                        {
#if !WINDOWS_UWP
                            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
#endif
                            long shift = 1;

                            try
                            {
                                if (parameter is String)
                                    lastValueObject = parameter as String;


                                if (lastValueObject is Array)
                                {
                                    try
                                    {
                                        bool bValue = System.Convert.ToBoolean(value);
                                        lastValue &= ~(shift << indexBit);
                                        if (bValue)
                                            lastValue |= shift << indexBit;
                                    }
                                    catch
                                    {
                                        long nValue = System.Convert.ToInt64(value);
                                        lastValue &= ~(shift << indexBit);
                                        if (nValue > 0)
                                            lastValue |= shift << indexBit;
                                    }
                                    var array = lastValueObject as Array;
                                    var v = InternalChangeType(lastValue, array.GetType().GetElementType());
                                    // if (array.Length < indexArray)
                                    array.SetValue(v, index);
                                    return array;
                                }
                                else if (lastValueObject is String &&
                                    ((lastValueObject as String).Length > 2 && (lastValueObject as String)[0] == '{' &&
                                    (lastValueObject as String)[(lastValueObject as String).Length - 1] == '}'))
                                {
                                    var str = lastValueObject as String;
                                    var values = str.Substring(1, str.Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                                    if (AdjustOutOfRangeArrayIndex && values.Length <= index)
                                    {
                                        var newValues = new string[index + 1];
                                        Array.Copy(values, newValues, values.Length);
                                        values = newValues;
                                    }
                                    if (values.Length > index)
                                    {
                                        try
                                        {
                                            bool bValue = System.Convert.ToBoolean(value);
                                            lastValue &= ~(shift << indexBit);
                                            if (bValue)
                                                lastValue |= shift << indexBit;
                                        }
                                        catch
                                        {
                                            long nValue = System.Convert.ToInt64(value);
                                            lastValue &= ~(shift << indexBit);
                                            if (nValue > 0)
                                                lastValue |= shift << indexBit;
                                        }
                                        values.SetValue(System.Convert.ChangeType(lastValue, values[0].GetType()), index);
                                    }
                                    else
                                        throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);

                                    if (targetType == typeof(String))
                                        return new ConverterValue(String.Format("{0}{1}{2}", '{', String.Join(" |", values), '}'), index, indexBit).ToXml();
                                    else
                                        return String.Format("{0}{1}{2}", '{', String.Join(" |", values), '}');
                                }
                                else if (lastValueObject is String &&
                                    (lastValueObject as String).Length > 2)
                                {
                                    var str = lastValueObject as String;
                                    var values = new String[str.Length / 2];
                                    for (int ii = 0; ii < str.Length / 2; ii++)
                                        values[ii] = str.Substring(ii * 2, 2);
                                    if (AdjustOutOfRangeArrayIndex && values.Length <= index)
                                    {
                                        var newValues = new string[index + 1];
                                        Array.Copy(values, newValues, values.Length);
                                        values = newValues;
                                    }
                                    if (values.Length > index)
                                    {
                                        try
                                        {
                                            bool bValue = System.Convert.ToBoolean(value);
                                            lastValue &= ~(shift << indexBit);
                                            if (bValue)
                                                lastValue |= shift << indexBit;
                                        }
                                        catch
                                        {
                                            long nValue = System.Convert.ToInt64(value);
                                            lastValue &= ~(shift << indexBit);
                                            if (nValue > 0)
                                                lastValue |= shift << indexBit;
                                        }
                                        var set = lastValue.ToString();
                                        if (set.Length > 2)
                                            set = set.Substring(0, 2);
                                        set = set.Replace(".", "");
                                        values.SetValue(set.PadLeft(2, '0'), index);
                                    }
                                    else
                                        throw new InvalidOperationException(Properties.Resources.OutOfRangeArrayIndex);

                                    if (targetType == typeof(String))
                                        return new ConverterValue(String.Join(String.Empty, values), index, indexBit).ToXml();
                                    else
                                        return String.Join(String.Empty, values);
                                }
                                else
                                    throw new InvalidOperationException(Properties.Resources.InvalidArrayValue);
                            }
                            finally
                            {
#if !WINDOWS_UWP
                                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
                            }
                        }
                    case ExpressionType.Expression:
                        {
#if !WINDOWS_UWP
                            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
#endif
                            try
                            {
                                var formula = sReverseFormulaParsed;
                                if (String.IsNullOrEmpty(formula))
                                    return value;
                                calcEngine.DefinedNames.Clear();
                                if (value is String)
                                {
                                    if (culture != System.Globalization.CultureInfo.InvariantCulture)
                                    {
                                        var str = value as String;
                                        value = str.Replace(digitSeparator, ".");
                                    }
                                }
                                var convertingBackCompareVariables = NormalizedBool(ConvertToString(value));
                                calcEngine.DefinedNames.Add(GetValidConstantName(xTag), GetValidConstantValue(convertingBackCompareVariables));
                                calcEngine.DefinedNames.Add(GetValidConstantName(xTag, true), GetValidConstantValue(convertingBackCompareVariables, true));
                                if (listReverseVariables != null && listReverseVariables.Count > 0 &&
                                    parameter is IDictionary<String, Object>)
                                {
                                    var sourceMap = parameter as IDictionary<String, Object>;
                                    var map = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
                                    foreach (var pair in sourceMap)
                                    {
                                        if (map.ContainsKey(pair.Key))
                                            map.Remove(pair.Key);
                                        map.Add(pair.Key, pair.Value);
                                    }
                                    /*
                                    var map = parameter as IDictionary<String, Object>;
                                    foreach (var name in map.Keys)
                                    {
                                        var val = map[name];
                                        if (val is String)
                                        {
                                            var str = val as String;
                                            str = str.Replace(digitSeparator, ".");
                                            calcEngine[name.ToUpper()] = NormalizedBool(str);
                                        }
                                        else
                                            calcEngine[name.ToUpper()] = map[name].ToString();
                                    }
                                    */
                                    listReverseVariables.ForEach(var =>
                                    {
                                        var orginalVar = var;
                                        if (mapCurrentParameteItems != null &&
                                            mapCurrentParameteItems.ContainsKey(var))
                                            var = mapCurrentParameteItems[var];
                                        if (map.ContainsKey(var))
                                        {
                                            var val = map[var];
                                            if (val is String)
                                            {
                                                var str = val as String;
                                                str = str.Replace(digitSeparator, ".");
                                                str = NormalizedBool(str);
                                                var definedName = GetValidConstantName(orginalVar);
                                                if (!calcEngine.DefinedNames.Contains(definedName))
                                                    calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str));
                                                definedName = GetValidConstantName(orginalVar, true);
                                                if (!calcEngine.DefinedNames.Contains(definedName))
                                                    calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str, true));
                                                convertingBackCompareVariables = String.Format("{0},{1}", convertingBackCompareVariables, str);
                                            }
                                            else
                                            {
                                                var str = map[var].ToString();
                                                var definedName = GetValidConstantName(orginalVar);
                                                if (!calcEngine.DefinedNames.Contains(definedName))
                                                    calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str));
                                                definedName = GetValidConstantName(orginalVar, true);
                                                if (!calcEngine.DefinedNames.Contains(definedName))
                                                    calcEngine.DefinedNames.Add(definedName, GetValidConstantValue(str, true));
                                                convertingBackCompareVariables = String.Format("{0},{1}", convertingBackCompareVariables, str);
                                            }
                                        }
                                    });
                                }

                                if (listReverseVariables != null && listReverseVariables.Count * 2 > calcEngine.DefinedNames.Count - 2)
                                    return null;

                                if (convertingBackCompareVariables == ConvertingBackVariables)
                                    return ConvertedBackValue;
                                // return System.Convert.ChangeType(calcEngine[valueTag], targetType);
                                var parameterValue = calcEngine.FormulaEngine.Evaluate(formula);
                                if (parameterValue.IsError)
                                    throw new InvalidOperationException(parameterValue.ErrorValue.Description);
                                ConvertingBackVariables = convertingBackCompareVariables;
                                if (parameterValue.IsNumeric)
                                    ConvertedBackValue = parameterValue.NumericValue.ToString("G17");
                                else
                                    ConvertedBackValue = parameterValue.ToString();
                                return ConvertedBackValue;
                            }
                            finally
                            {
#if !WINDOWS_UWP
                                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
                            }
                        }
                    case ExpressionType.error:
                        return ParserError;
                }

                return value;
            }
            catch (Exception ex)
            {
                if (bThrowExceptions)
                    throw ex;
#if !NET_STANDARD
                return new ValidationResult(
#if !WINDOWS_UWP
                    false,
#endif
                    ex.Message);
#else
                return new System.ComponentModel.DataAnnotations.ValidationResult(ex.Message);
#endif
            }
        }
        #endregion

        object InternalChangeType(object value, Type type)
        {
            try
            {
                return System.Convert.ChangeType(value, type);
            }
            catch (System.FormatException formatExc)
            {
                if (Decimal.TryParse((string)value, out decimal decValue))
                    return InternalChangeType(System.Convert.ToInt64(decValue), type);
                else
                    throw;
            }
            catch
            {
                return InternalChangeType(System.Convert.ToInt64(value), type);
            }
        }

        object InternalChangeType(long value, Type type)
        {
            try
            {
                return System.Convert.ChangeType(value, type);
            }
            catch
            {
                string binary = null;
                switch (type.Name)
                {
                    case "Byte":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 8));
                        return System.Convert.ToByte(binary, 2);
                    case "SByte":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 8));
                        return System.Convert.ToSByte(binary, 2);
                    case "Int16":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 16));
                        return System.Convert.ToInt16(binary, 2);
                    case "UInt16":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 16));
                        return System.Convert.ToUInt16(binary, 2);
                    case "Int32":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 32));
                        return System.Convert.ToInt32(binary, 2);
                    case "UInt32":
                        binary = System.Convert.ToString(value, 2);
                        binary = binary.Substring(Math.Max(0, binary.Length - 32));
                        return System.Convert.ToUInt32(binary, 2);
                    case "Int64":
                        binary = System.Convert.ToString(value, 2);
                        // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                        return System.Convert.ToInt64(binary, 2);
                    case "UInt64":
                        binary = System.Convert.ToString(value, 2);
                        // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                        return System.Convert.ToUInt64(binary, 2);
                    default:
                        return value;
                }
            }
        }

        public void Dispose()
        {
            if (bDisposeCalcEngine && calcEngine != null)
            {
                calcEngine.Dispose();
                calcEngine = null;
            }

            //ClearReferenceNodeId();
        }
    }

    public static class ExpressionValueConverterHelper
    {
        public static List<string> GetListVarInExpression(string expression)
        {
            using (var expressor = new ExpressionValueConverter())
            {
                return expressor.GetFormulaVariables(expression, true);
            }
        }

        static List<string> libraryFunctions;
        public static List<string> LibraryFunctions
        {
            get
            {
                if (libraryFunctions == null)
                {
                    // Operators
                    libraryFunctions = new List<string>() { @"+", @"-", @"*", @"/", @"%"};

                    var sortedFunctions = new List<string>();
                    using (var workbook = new WorkbookExtended())
                    {
                        // Built-in Mathematical Functions
                        var properties = workbook.Functions.Math.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var function = prop.GetValue(workbook.Functions.Math) as IFunction;
                                if (function != null)
                                    sortedFunctions.Add(function.Name);
                            }
                            catch
                            { }
                        }

                        // Built-in Statistical Functions
                        properties = workbook.Functions.Statistical.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var function = prop.GetValue(workbook.Functions.Statistical) as IFunction;
                                if (function != null)
                                    sortedFunctions.Add(function.Name);
                            }
                            catch
                            { }
                        }

                        // Built-in Engineering Functions
                        properties = workbook.Functions.Engineering.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var function = prop.GetValue(workbook.Functions.Engineering) as IFunction;
                                if (function != null)
                                    sortedFunctions.Add(function.Name);
                            }
                            catch
                            { }
                        }

                        // Built-in Logical Functions
                        properties = workbook.Functions.Logical.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var function = prop.GetValue(workbook.Functions.Logical) as IFunction;
                                if (function != null)
                                    sortedFunctions.Add(function.Name);
                            }
                            catch
                            { }
                        }

                        // Built-in Text Functions
                        properties = workbook.Functions.Text.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var function = prop.GetValue(workbook.Functions.Text) as IFunction;
                                if (function != null)
                                    sortedFunctions.Add(function.Name);
                            }
                            catch
                            { }
                        }

                        // User-Defined Functions (UDF)
                        foreach (var function in workbook.CustomFunctions)
                        {
                            sortedFunctions.Add(function.Name);
                        }

                        // User-Defined Functions (UDF) defined globally
                        foreach (var function in workbook.GlobalCustomFunctions)
                        {
                            sortedFunctions.Add(function.Name);
                        }
                    }

                    sortedFunctions.Sort();
                    libraryFunctions.AddRange(sortedFunctions);
                }

                return libraryFunctions;
            }
        }
    }
}
