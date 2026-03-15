//-------------------------------------------------------------------------------------------------
// <copyright file="Formulas.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Data;
using System.Text.RegularExpressions;

namespace Syncfusion.Grouping
{
    #region ExpressionFieldEvaluator Class

    /// <summary>
    /// Encapsulates the code required to parse and compute formulas. Hashtable
    /// properties maintain a Formula Library of functions as well as a list
    /// of dependent cells.
    /// <para/>
    /// You can add and remove library functions.
    /// </summary>
    public class ExpressionFieldEvaluator : IExpressionFieldEvaluator
    {
        char NumberDecimalSeparator
        {
            get
            {
                return engine.NumberDecimalSeparator;
            }
        }

        /// <summary>
        /// Gets the culture information.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return engine.Culture;
            }
        }

        char[] BRACEDELIMETER
        {
            get
            {
                return new char[] { Convert.ToChar(engine.Culture.TextInfo.ListSeparator) };
            }
        }

        ////holds the cell being calculated.. set in CellModel.GetFormattedText
        internal string cell = string.Empty;

        /// <summary>
        /// Displays information on the cell currently being calculated.
        /// </summary>
        /// <returns>String with information on the cell currently being calculated.</returns>
        public override string ToString()
        {
            return "ExpressionFieldEvaluator { Cell: " + (cell != null ? cell.ToString() : "null") + "}";
        }

        Engine engine;

        /// <summary>
        /// Constructor for ExpressionFieldEvaluator.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        public ExpressionFieldEvaluator(TableDescriptor tableDescriptor)
        {
            ////switches TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, this);
            this.engine = tableDescriptor.Engine;
            this.tableDescriptor = tableDescriptor;
        }

        private int maximumRecursiveCalls = 20;

        /// <summary>
        /// Specifies the maximum number of recursive calls that can be used to compute a value.
        /// </summary>
        /// <remarks>This property comes into play when you have a calculated formula cell that depends on  
        /// another calculated formula that depends on another calculated formula and so on. If the number of
        /// 'depends on another formula' exceeds MaximumRecursiveCalls, you will see a Too Complex message
        /// displayed in the cell. The default value is 20, but you can set it higher or lower depending upon 
        /// your expected needs. The purpose of the limit is to avoid a circular reference locking up your
        /// application.
        /// </remarks>
        public int MaximumRecursiveCalls
        {
            get { return maximumRecursiveCalls; }
            set { maximumRecursiveCalls = value; }
        }

    #endregion

        #region Error Messages

        /// <summary>
        /// String array that holds the strings used in error messages within the Formula Engine.
        /// </summary>
        /// <remarks>If you want to change the error messages displayed within the Formula Engine,
        /// you can set your new strings into the appropriate position in the FormulaErrorStrings 
        /// array. Here is the code that shows the default settings. You should assign your new
        /// strings to the corresponding positions. 
        /// </remarks>
        /// <example>Here is the code that shows position of each string in FormulaErrorStrings.
        /// <code lang="C#">
        ///        public string[] FormulaErrorStrings = new string[]
        ///        {
        ///            "binary operators cannot start an expression",    //0
        ///            "cannot parse",                                    //1
        ///            "bad library",                                    //2
        ///            "invalid char in front of",                        //3
        ///            "number contains 2 decimal points",                //4
        ///            "expression cannot end with an operator",        //5
        ///            "invalid characters following an operator",        //6
        ///            "invalid character in number",                    //7
        ///            "mismatched parentheses",                        //8
        ///            "unknown formula name",                            //9
        ///            "requires a single argument",                    //10
        ///            "requires 3 arguments",                            //11
        ///            "invalid Math argument",                        //12
        ///            "requires 2 arguments",                            //13
        ///            "bad index",                                    //14
        ///            "too complex",                                    //15
        ///            "circular reference: ",                            //16
        ///            "missing formula",                                //17
        ///            "improper formula",                                //18
        ///            "invalid expression",                            //19
        ///            "cell empty",                                    //20
        ///            "empty expression",                                //21
        ///         "mismatched string tics",                        //22
        ///         "named functions not supported in expressions",    //23
        ///         "not a formula",                                //24
        ///         "missing operand"                                //25
        ///        };
        /// </code>
        /// </example>
        public string[] FormulaErrorStrings = new string[]
            {
                "binary operators cannot start an expression",    ////0
                "cannot parse",                                    ////1
                "bad library",                                    ////2
                "invalid char in front of",                        ////3
                "number contains 2 decimal points",                ////4
                "expression cannot end with an operator",        ////5
                "invalid characters following an operator",        ////6
                "invalid character in number",                    ////7
                "mismatched parentheses",                        ////8
                "unknown formula name",                            ////9
                "requires a single argument",                    ////10
                "requires 3 arguments",                            ////11
                "invalid Math argument",                        ////12
                "requires 2 arguments",                            ////13
                "bad index",                                    ////14
                "too complex",                                    ////15
                "circular reference: ",                            ////16
                "missing formula",                                ////17
                "improper formula",                                ////18
                "invalid expression",                            ////19
                "cell empty",                                    ////20
                "empty expression",                                ////21
                "mismatched string tics",                        ////22
                "named functions not supported in expressions",    ////23
                "not a formula",    ////24
                "missing operand"    ////25
            };

        internal int operators_cannot_start_an_expression = 0;
        internal int cannot_parse = 1;
        internal int bad_library = 2;
        internal int invalid_char_in_front_of = 3;
        internal int number_contains_2_decimal_points = 4;
        internal int expression_cannot_end_with_an_operator = 5;
        internal int invalid_characters_following_an_operator = 6;
        internal int invalid_char_in_number = 7;
        internal int mismatched_parentheses = 8;
        internal int unknown_formula_name = 9;
        internal int requires_a_single_argument = 10;
        internal int requires_3_args = 11;
        internal int invalid_Math_argument = 12;
        internal int requires_2_args = 13;
        internal int bad_index = 14;
        internal int too_complex = 15;
        internal int circular_reference_ = 16;
        internal int missing_formula = 17;
        internal int improper_formula = 18;
        internal int invalid_expression = 19;
        internal int cell_empty = 20;
        internal int empty_expression = 21;
        internal int mismatched_tics = 22;
        internal int named_functions_not_supported_in_expressions = 23;
        internal int not_a_formula = 24;
        internal int missing_operand = 25;

        #endregion

        #region Grouping Calculation Code

        private Record currentTableItemIndex;
        private Hashtable nameToTableToken = null;
        private Hashtable tokenToTableName = null;
        private Hashtable dataTableColumns = null;
        private int dataTableTokenCount = -1;
        private TableDescriptor tableDescriptor = null;
        int fieldsVersion = -1;

        internal void EnsureFieldDescriptors()
        {
            if (tableDescriptor != null && fieldsVersion != this.tableDescriptor.Fields.Version)
            {
                InitializeTableFormulaTokens();
                fieldsVersion = this.tableDescriptor.Fields.Version;
            }
        }

        /// <summary>
        /// Loads item properties from the table descriptor and creates tokens that can be used
        /// in compiled expressions.
        /// </summary>
        public void InitializeTableFormulaTokens()
        {
            TableDescriptor td = this.tableDescriptor;
            if (this.nameToTableToken == null)
            {
                nameToTableToken = new Hashtable();
            }
            else
            {
                nameToTableToken.Clear();
            }

            if (this.tokenToTableName == null)
            {
                tokenToTableName = new Hashtable();
            }
            else
            {
                tokenToTableName.Clear();
            }

            int i = 0;

            this.dataTableColumns = new Hashtable();
            // TODO: should this be fields?
            foreach (FieldDescriptor column in td.Fields)
            {
                i++;
                string token = GetAlphaLabel(i).ToLower() + "1";
                nameToTableToken.Add(column.Name.ToLower(), token);
                tokenToTableName.Add(token, column.Name);
                dataTableColumns.Add(column.Name.ToLower(), column);
            }

            dataTableTokenCount = i;
        }

        /// <summary>
        /// Returns a string in the format "A, B, C, ... AA, AB ..." to be used for column labels.
        /// </summary>
        /// <param name="nCol">The column index.</param>
        /// <returns>
        ///   <para>A string that contains the column label for the column index.</para>
        /// </returns>
        static string GetAlphaLabel(int nCol)
        {
            char[] cols = new char[10];
            int n = 0;
            while (nCol > 0 && n < 9)
            {
                nCol--;
                cols[n] = (char)(nCol % 26 + 'A');
                nCol = nCol / 26;
                n++;
            }

            char[] chs = new char[n];
            for (int i = 0; i < n; i++)
            {
                chs[n - i - 1] = cols[i];
            }

            return new String(chs);
        }

        /// <summary>
        /// Replaces column references with tokens.
        /// </summary>
        /// <param name="formula">The formula expression.</param>
        /// <returns>A prepared expression string.</returns>
        public string PutTokensInFormula(string formula)
        {
            EnsureFieldDescriptors();
            foreach (string columnName in this.dataTableColumns.Keys)
            {
                string name = columnName.ToLower();
                string token = ((string)nameToTableToken[name]).ToLower();
                formula = formula.Replace("[" + name + "]", token);
            }

            return formula;
        }

        ////compute values for the position-th item in the datasource

        /// <summary>
        /// Compute values for the record in the datasource.
        /// </summary>
        /// <param name="formula">The pre-compiled formula expression.</param>
        /// <param name="position">The record.</param>
        /// <param name="expressionName">The name of the expression being computed.</param>
        /// <returns>The resulting value.</returns>
        public string ComputeFormulaValueAt(string formula, Record position, string expressionName)
        {
            if (inValidFormulaTest)
            {
                string saveName = this.cell;
                this.cell = expressionName;
                string s;
                try
                {
                    s = ComputeFormulaValueAt(formula, position);
                }
                catch
                {
                    throw new ArgumentException(FormulaErrorStrings[circular_reference_]);
                }
                finally
                {
                    this.cell = saveName;
                }

                return s;
            }
            else
            {
                return ComputeFormulaValueAt(formula, position);
            }
        }

        /// <summary>
        /// Compute values for the record of inner formulas.
        /// </summary>
        /// <param name="formula">The pre-compiled formula expression.</param>
        /// <returns>The resulting value.</returns>
        public string ComputeFormulaValueAt(string formula)
        {
            if (formula.Length > 0 && (formula[0] == FUNCTIONMARKER || formula[0] == 'q'))
            {
                ////this is a function call
                int i = formula.IndexOf(FUNCTIONMARKER, 2);
                if (i > 0)
                {
                    string args = formula.Substring(i + 2, formula.Length - i - 3);
                    string name = formula.Substring(1, i - 1);
                    LibraryFunction func = this.FunctionNames[name] as LibraryFunction;
                    if (func != null)
                    {
                        string computedArgs = string.Empty;
                        foreach (string s in args.Split(BRACEDELIMETER))
                        {
                            if (computedArgs.Length > 0)
                            {
                                computedArgs += BRACEDELIMETER[0];
                            }

                            computedArgs += ComputedValue(s);
                        }

                        return func(computedArgs);
                    }
                }
            }

            return ComputedValue(formula);
        }
        /// <summary>
        /// Compute values for the record in the datasource.
        /// </summary>
        /// <param name="formula">The pre-compiled formula expression.</param>
        /// <param name="position">The record.</param>
        /// <returns>The resulting value.</returns>
        public string ComputeFormulaValueAt(string formula, Record position)
        {
            currentTableItemIndex = position;
            if (formula.Length > 0 && (formula[0] == FUNCTIONMARKER || formula[0] == 'q'))
            {
                ////this is a function call
                int i = formula.IndexOf(FUNCTIONMARKER, 2);
                if (i > 0)
                {
                    string args = formula.Substring(i + 2, formula.Length - i - 3);
                    string name = formula.Substring(1, i - 1);
                    LibraryFunction func = this.FunctionNames[name] as LibraryFunction;
                    if (func != null)
                    {
                        string computedArgs = string.Empty;
                        foreach (string s in args.Split(BRACEDELIMETER))
                        {
                            if (computedArgs.Length > 0)
                            {
                                computedArgs += BRACEDELIMETER[0];
                            }

                            computedArgs += ComputedValue(s);
                        }

                        return func(computedArgs);
                    }
                }
            }

            return ComputedValue(formula);
        }

        ////Override to return the value for the currentTableItemIndex position in the datasource
        //// based on the token passed in. Use tokenToTableName to map the token to column MappingName.

        /// <summary>
        /// Returns the value for the specified field / token from the record.
        /// </summary>
        /// <param name="token">The column token.</param>
        /// <returns>The value from the record.</returns>
        public virtual string GetValueFromDataTable(string token)
        {
            EnsureFieldDescriptors();
            object name = tokenToTableName[token.ToLower()];
            if (name == null)
            {
                return FormulaErrorStrings[improper_formula];
            }
            else
            {
                if (currentTableItemIndex == null)
                {
                    return string.Empty;
                }
                // Todo: Could use this.dataTableColumns instead.
                object value = currentTableItemIndex.GetValue((string)name);
                return value != null ? ConvertToString(value) : string.Empty;
            }
        }

        #endregion

        #region Parsing Code
        private char UNIQUESTRINGMARKER = (char)127;
        private Hashtable SaveStrings(ref string text)
        {
            // If Only one Brace is entered ({ or }), then just return null
            if (TIC.Length == 1 && (text.Split(TIC[0]).Length - 1) % 2 == 1)
            {
                return null;
            }
            Hashtable strings = null;
            string TICs2 = TIC + TIC;
            int id = 0;
            int i = -1;
            if ((i = text.IndexOf(TIC)) > -1)
            {
                while (i > -1 && i < text.Length)
                {
                    if (strings == null)
                    {
                        strings = new Hashtable();
                    }

                    int j = (i + 1) < text.Length ? text.IndexOf(TIC, i + 1) : -1;
                    if (j > -1)
                    {
                        string key = TIC + UNIQUESTRINGMARKER + id.ToString() + TIC;
                        if (j < text.Length - 2 && text[j + 1] == TIC[0])
                        {
                            j = text.IndexOf(TIC, j + 2);
                            if (j == -1)
                            {
                                throw new ArgumentException(FormulaErrorStrings[mismatched_tics]);
                            }
                        }

                        string s = text.Substring(i, j - i + 1);
                        strings.Add(key, s);
                        s = s.Replace(TICs2, TIC);
                        id++;
                        text = text.Substring(0, i) + key + text.Substring(j + 1);
                        i = i + key.Length;
                        if (i < text.Length)
                        {
                            i = text.IndexOf(TIC, i);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(FormulaErrorStrings[mismatched_tics]);
                    }
                }
            }

            return strings;
        }

        private void SetStrings(ref string retValue, Hashtable strings)
        {
            foreach (string s in strings.Keys)
            {
                retValue = retValue.Replace(s, (string)strings[s]);
            }
        }

        ////internal 
        string IExpressionFieldEvaluator.Parse(string text)
        {
            ////switches TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, text, this);

            if (text.Length == 0)
            {
                return text;
            }

            ////strip out leading equal sign to avoid offending excel users
            if (text[0] == '=')
            {
                text = text.Substring(1);
            }

            ////make braces strings...
            text = text.Replace(BRACELEFT, TIC);
            text = text.Replace(BRACERIGHT, TIC);
            text = text.Replace("-+", "-");
            int i = 0;
            Hashtable formulaStrings = SaveStrings(ref text);

            text = text.ToUpper(CultureInfo.InvariantCulture);
            text = text.Replace(" ", string.Empty);

            ////handle function library calls
            try
            {
                MarkLibraryFormulas(ref text);
            }
            catch (Exception ex)
            {
                return TIC + ex.Message + TIC;
            }

            while ((i = text.IndexOf(')')) > -1)
            {
                int k = text.Substring(0, i).LastIndexOf('(');
                if (k == -1)
                {
                    throw new ArgumentException(this.FormulaErrorStrings[mismatched_parentheses]);
                }

                if (k == i - 1)
                {
                    throw new ArgumentException(this.FormulaErrorStrings[empty_expression]);
                }
                string s = string.Empty;
                s = text.Substring(k + 1, i - k - 1);
                text = text.Substring(0, k) + this.ParseSimple(s) + text.Substring(i + 1);
            }
            ////look for inner matching & parse pieces without parens with ParseSimple
            ParseInnerParens(ref text);

            string retValue = string.Empty;
            try
            {
                ////all parens should have been removed  
                if (text.IndexOf('(') > -1)
                {
                    throw new ArgumentException(FormulaErrorStrings[mismatched_parentheses]);
                }

                retValue = ParseSimple(text);
                if (formulaStrings != null && formulaStrings.Count > 0)
                {
                    SetStrings(ref retValue, formulaStrings);
                }
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }

            return retValue;
        }

        void ParseInnerParens(ref string text)
        {
            ////look for inner matching & parse pieces without parens with ParseSimple
            int i;
            while ((i = text.IndexOf(')')) > -1)
            {
                int k = text.Substring(0, i).LastIndexOf('(');
                if (k == -1)
                {
                    throw new ArgumentException(FormulaErrorStrings[mismatched_parentheses]);
                }

                if (k == i - 1)
                {
                    throw new ArgumentException(FormulaErrorStrings[empty_expression]);
                }

                string s = text.Substring(k + 1, i - k - 1);
                text = text.Substring(0, k) + ParseSimple(s) + text.Substring(i + 1);
            }
        }
        private char RIGHTBRACKET = (char)131;//(char)131; ////']';
        private char LEFTBRACKET = (char)130;//(char)130; ////'[';

        /// <summary>
        /// It parse the formula for library fnction.
        /// </summary>
        /// <param name="formula">formula to compute</param>
        /// <returns>parsed formula with functions.</returns>
        private bool MarkLibraryFormulas(ref string formula)
        {
            int rightParens = formula.IndexOf(')');
            if (rightParens == -1)
            {
                return false;
            }
            else
            {
                while (rightParens > -1)
                {
                    int parenCount = 0;
                    int leftParens = rightParens - 1;
                    while (leftParens > -1 && (formula[leftParens] != '(' || parenCount != 0))
                    {
                        if (formula[leftParens] == ')')
                        {
                            parenCount++;
                        }
                        else if (formula[leftParens] == ')')
                        {
                            parenCount--;
                        }

                        leftParens--;
                    }

                    if (leftParens == -1)
                    {
                        return false;
                    }

                    int i = leftParens - 1;
                    while (i > -1 && char.IsLetterOrDigit(formula[i]))// || this.validFunctionNameChars.IndexOf(formula[i]) > -1 || formula[i].Equals(parseDecimalSeparator)))
                    {
                        i--;
                    }

                    int len = leftParens - i - 1;
                    if (len > 0 && FunctionNames[formula.Substring(i + 1, len)] != null)
                    {
                        //string s = formula.Substring(leftParens + 1, formula.Length - leftParens - 2).ToUpper();
                        string s = formula.Substring(leftParens, rightParens - leftParens + 1);
                        formula = formula.Substring(0, i + 1) + 'q' + formula.Substring(i + 1, len) + FUNCTIONMARKER + s.Replace('(', this.LEFTBRACKET).Replace(')', RIGHTBRACKET) + formula.Substring(rightParens + 1);
                    }
                    else
                    {
                        string s = string.Empty;
                        if (leftParens > 0)
                        {
                            s = formula.Substring(0, leftParens);
                        }

                        s = s + '{' +
                            formula.Substring(leftParens + 1, rightParens - leftParens - 1) + '}';
                        if (rightParens < formula.Length)
                        {
                            s = s + formula.Substring(rightParens + 1);
                        }

                        formula = s;
                    }

                    rightParens = formula.IndexOf(')');
                }
            }

            formula = formula.Replace('{', '(').Replace('}', ')');
            return true;
        }
        bool HasFunction(ref string text)
        {
            bool ret = false;

            int i = text.IndexOf('(');
            if (i > 0)
            {
                string name = text.Substring(0, i);
                name = name.ToUpper(CultureInfo.InvariantCulture);
                if (FunctionNames.ContainsKey(name))
                {
                    if (text[text.Length - 1] == ')')
                    {
                        string args = text.Substring(i + 1, text.Length - i - 2).ToUpper();
                        string parsedArgs = string.Empty;
                        try
                        {
                            foreach (string s in args.Split(BRACEDELIMETER))
                            {
                                if (parsedArgs.Length > 0)
                                {
                                    parsedArgs += BRACEDELIMETER[0];
                                }

                                string s1 = s;
                                ParseInnerParens(ref s1);
                                parsedArgs += this.ParseSimple(s1);
                            }

                            text = FUNCTIONMARKER + name + FUNCTIONMARKER + parsedArgs;
                            ret = true;
                        }
                        catch
                        {
                            throw new ArgumentException(FormulaErrorStrings[named_functions_not_supported_in_expressions]);
                        }
                    }
                    else
                    {
                        ////dont allow nested functions calls
                        throw new ArgumentException(FormulaErrorStrings[named_functions_not_supported_in_expressions]);
                    }
                }
            }

            return ret;
        }

        private Hashtable functionNames;

        /// <summary>
        /// A hashtable whose keys are function names and 
        /// whose values are LibraryFunction delgates.
        /// </summary>
        public Hashtable FunctionNames
        {
            get
            {
                if (functionNames == null)
                {
                    functionNames = new Hashtable();
                }

                return functionNames;
            }
        }

        /// <summary>
        /// Delegate for custom functions used with <see cref="AddFunction"/>.
        /// </summary>
        public delegate string LibraryFunction(string args);

        /// <summary>
        /// Adds a function to the Function Library.
        /// </summary>
        /// <param name="name">The name of the function to be added.</param>
        /// <param name="func">The function to be added.</param>
        /// <returns>True if successfully added, False otherwise.</returns>
        /// <remarks>
        /// LibraryFunction is a delegate the defines the signature of functions that
        /// you can add to the Function Library.
        /// Adding a custom function requires two steps. The first is to register a name
        /// and LibraryFunction delegate with the ExpressionFielEvaluator object. The second step
        /// is to add a method to your code that implements the LibraryFunction delegate to perform 
        /// the calculations you want done.
        /// <para/>
        /// There are restrictions on the use Functions within expressions. Functions can only be used 
        /// stand-alone. They cannot be used as part of a more complex expression. So, 
        /// "Func([Col1], 2*[Col2]+1)" is a valid use of a function named Func that accepts two
        /// arguments. But "2 * Func([Col1], 2*[Col2]+1) + 1" is not valid. If you need to use
        /// functions in algebraic expressions, then first add an Expression field that uses the 
        /// function stand-alone. Then in your algebraic expression, you can refer to this Expression
        /// field. Argument used in library function calls, can be any algebraic combination of 
        /// fields and constants, but they cannot contain function references. During calculations, the
        /// arguments are fully evaluated before being passed into the method you implement.
        /// </remarks>
        /// <example>  
        /// In the sample below,
        /// ComputeFunc is the name of the method we add to our code to compute the function value.
        /// Func is the string name that we use in an expression to reference the custom function as in
        /// "Func([Col1], [Col2])". 
        /// <code lang="C#">
        /// // step 1 - register the function name and delegate
        /// ExpressionFieldEvaluator evaluator = this.groupingEngine.TableDescriptor.ExpressionFieldEvaluator;//.CreateExpressionFieldEvaluator(this.groupingEngine.TableDescriptor);
        /// evaluator.AddFunction("Func", new ExpressionFieldEvaluator.LibraryFunction(ComputeFunc));
        /// <para/>
        /// //. . . 
        ///  <para/>
        /// // step 1 - defining the method
        /// // Computes the absolute value of arg1-2*arg2
        ///    // parameter s- a list of 2 arguments 
        /// // returns string holding computed value
        /// public string ComputeFunc(string s)
        /// {
        ///     //get the list delimiter (for en-us, its is a comma)
        ///     char comma = Convert.ToChar(this.gridGroupingControl1.Culture.TextInfo.ListSeparator);
        ///     string[] ss = s.Split(comma);
        ///     if(ss.GetLength(0) != 2)
        ///         throw new ArgumentException("Requires 2 arguments.");
        ///     double arg1, arg2;
        ///     if(double.TryParse(ss[0], System.Globalization.NumberStyles.Any, null, out arg1)
        ///         &amp;&amp; double.TryParse(ss[1], System.Globalization.NumberStyles.Any, null, out arg2))
        ///     {
        ///         return Math.Abs(arg1 - 2 * arg2).ToString();
        ///     }
        ///     return "";
        /// }
        /// </code>
        /// <code lang="VB">
        /// ' step 1 - register the function name and delegate
        /// Dim evaluator As ExpressionFieldEvaluator = Me.groupingEngine.TableDescriptor.ExpressionFieldEvaluator
        /// evaluator.AddFunction("Func", New ExpressionFieldEvaluator.LibraryFunction(AddressOf ComputeFunc)) 
        ///  <para/>
        /// '. . .
        ///  <para/>
        /// ' step 1 - defining the method
        /// ' Computes the absolute value of arg1-2*arg2
        /// ' parameter s- a list of 2 arguments 
        /// ' returns string holding computed value
        /// Public Function ComputeFunc(s As String) As String
        ///     'get the list delimiter (for en-us, its is a comma)
        ///     Dim comma As Char = Convert.ToChar(Me.gridGroupingControl1.Culture.TextInfo.ListSeparator)
        ///     Dim ss As String() = s.Split(comma)
        ///     If ss.GetLength(0) &lt;&gt; 2 Then
        ///            Throw New ArgumentException("Requires 2 arguments.")
        ///     End If
        ///      Dim arg1, arg2 As Double
        ///     If Double.TryParse(ss(0), System.Globalization.NumberStyles.Any, Nothing, arg1) _ 
        ///            AndAlso Double.TryParse(ss(1), System.Globalization.NumberStyles.Any, Nothing, arg2) Then
        ///         Return Math.Abs((arg1 - 2 * arg2)).ToString()
        ///     End If
        ///     Return ""
        /// End Function 'ComputeFunc
        /// </code>
        /// </example>
        public bool AddFunction(string name, LibraryFunction func)
        {
            name = name.ToUpper(CultureInfo.InvariantCulture);

            if (FunctionNames[name] == null)
            {
                FunctionNames.Add(name, func);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a function from the Function Library.
        /// </summary>
        /// <param name="name">The name of the function to be removed.</param>
        /// <returns>True if successfully removed, False otherwise.</returns>
        public bool RemoveFunction(string name)
        {
            name = name.ToUpper(CultureInfo.InvariantCulture);

            if (FunctionNames[name] != null)
            {
                FunctionNames.Remove(name);
                return true;
            }

            return false;
        }

        ////Operator Parsing
        ////CHAR_xxxx used in formulas; swapped for TOKEN_xxxx in parsed formula
        ////TOKEN_xxxx is lowercase alpha char
        ////STRING_xxxx identifies the operators that require multiple characters; get mapped to CHAR_xxxx to fit into single char algorithm
        ////lowercase letters used: abcdefghijklmnopqrstuwxvyz,126,@
        private const char TOKEN_multiply = 'm';
        private char CHAR_multiply = '*';
        private const char TOKEN_divide = 'd';
        private char CHAR_divide = '/';
        private const char TOKEN_add = 'a';
        private char CHAR_add = '+';
        private const char TOKEN_subtract = 's';
        private char CHAR_subtract = '-';

        private const char TOKEN_less = 'l';
        private char CHAR_less = '<';
        private const char TOKEN_greater = 'g';
        private char CHAR_greater = '>';
        private const char TOKEN_equal = 'e';
        private char CHAR_equal = '=';

        private const char TOKEN_lesseq = 'k';
        private char CHAR_lesseq = 'f';
        private string STRING_lesseq = "<=";
        private const char TOKEN_greatereq = 'j';
        private char CHAR_greatereq = 'h';
        private string STRING_greatereq = ">=";
        private const char TOKEN_noequal = 'o';
        private char CHAR_noequal = 'p';
        private string STRING_noequal = "<>";

        private const char TOKEN_and = 'c';
        private char CHAR_and = 'i';
        private string STRING_and = "AND";
        private const char TOKEN_or = (char)126; ////'k';
        private char CHAR_or = 'w';
        private string STRING_or = "OR";
        private const char TOKEN_xor = 'x';
        private char CHAR_xor = 'r';
        private string STRING_xor = "XOR";
        private const char TOKEN_like = 't';
        private char CHAR_like = 'v';
        private string STRING_like = "LIKE";
        private const char TOKEN_match = 'y';
        private char CHAR_match = 'z';
        private string STRING_match = "MATCH";

        private const char TOKEN_between = (char)130;
        private char CHAR_between = (char)131;
        private string STRING_between = "BETWEEN";

        private const char TOKEN_in = (char)132;
        private char CHAR_in = (char)133;
        private string STRING_in = "IN";

        private const char TOKEN_betweenT = (char)134;
        private char CHAR_betweenT = (char)135;
        private string STRING_betweenT = "BETWEENTIME";

        private string TIC = "'"; ////used to mark strings
        private string BRACELEFT = "{";
        private string BRACERIGHT = "}";

        private char FUNCTIONMARKER = '@';

        private string ParseSimple(string text)
        {
            ////switches TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, text, this);

            text = text.Replace(STRING_lesseq, CHAR_lesseq.ToString());
            text = text.Replace(STRING_greatereq, CHAR_greatereq.ToString());
            text = text.Replace(STRING_noequal, CHAR_noequal.ToString());

            text = text.Replace(STRING_match, CHAR_match.ToString());
            text = text.Replace(STRING_like, CHAR_like.ToString());

            text = text.Replace(STRING_xor, CHAR_xor.ToString());
            text = text.Replace(STRING_or, CHAR_or.ToString());
            text = text.Replace(STRING_and, CHAR_and.ToString());

            text = text.Replace(STRING_in, CHAR_in.ToString());
            text = text.Replace(STRING_betweenT, CHAR_betweenT.ToString()); ////do betweenT before between
            text = text.Replace(STRING_between, CHAR_between.ToString());

            ////doing things sequentially imposes computation hierarchy for 5 levels
            // 1. * /
            // 2. + -
            // 3. < > = <= >= <>
            // 4. match, like, in, between
            // 5. or, and, xor
            // in each level parsing done left to right
            text = ParseSimple(text, new char[] { TOKEN_multiply, TOKEN_divide }, new char[] { CHAR_multiply, CHAR_divide });
            text = ParseSimple(text, new char[] { TOKEN_add, TOKEN_subtract }, new char[] { CHAR_add, CHAR_subtract });
            text = ParseSimple(text, new char[] { TOKEN_less, TOKEN_greater, TOKEN_equal, TOKEN_lesseq, TOKEN_greatereq, TOKEN_noequal }, new char[] { CHAR_less, CHAR_greater, CHAR_equal, CHAR_lesseq, CHAR_greatereq, CHAR_noequal });
            text = ParseSimple(text, new char[] { TOKEN_match, TOKEN_like }, new char[] { CHAR_match, CHAR_like });
            text = ParseSimple(text, new char[] { TOKEN_or, TOKEN_and, TOKEN_xor }, new char[] { CHAR_or, CHAR_and, CHAR_xor });

            return text;
        }

        private string ParseSimple(string text, char[] markers, char[] operators)
        {
            ////switches TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, text, markers, operators);
            int i;

            ////op is string containing CHAR_xxxx's for each operator in this level
            string op = string.Empty;
            foreach (char c in operators)
            {
                op = op + c;
            }

            ////mark unary minus with u-token
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text);
            text = sb.Replace(",-", ",u").Replace("[-", "[u").Replace("=-", "=u").Replace(">-", ">u").Replace("<-", "<u").Replace("/-", "/u").Replace("*-", "*u").Replace("+-", "+u").Replace("--", "-u").ToString();

            ////Get rid of leading pluses.
            text = sb.Replace(",+", ",").Replace("[+", "[").Replace("=+", "=").Replace(">+", ">").Replace("<+", "<").Replace("/+", "/").Replace("*+", "*").Replace("++", "+").ToString();

            if (text.Length > 0 && text[0] == '-')
            {
                ////leading unary minus  
                text = "0" + text;
            }

            if (text.IndexOfAny(operators) > -1)
            {
                while ((i = text.IndexOfAny(operators)) > -1)
                {
                    string left = string.Empty;
                    string right = string.Empty;

                    int leftIndex = 0;
                    int rightIndex = 0;

                    if (i < 1 && text[i] != '-')
                    {
                        throw new ArgumentException(FormulaErrorStrings[operators_cannot_start_an_expression]);
                    }

                    ////process left argument
                    int j = i - 1;

                    if (i == 0 && text[i] == '-')
                    {
                        ////unary minus - block and continue
                        text = "bnu" + text.Substring(1) + 'b';
                        continue;
                    }
                    else if (text[j] == TIC[0])
                    {
                        ////string
                        int k = text.Substring(0, j - 1).LastIndexOf(TIC);
                        if (k < 0)
                        {
                            throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                        }

                        left = text.Substring(k + 1, j - k - 1);
                        leftIndex = k + 1;
                    }
                    else if (text[j] == 'b')
                    {
                        ////block of already parsed code
                        int k = text.Substring(0, j - 1).LastIndexOf('b');
                        if (k < 0)
                        {
                            throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                        }

                        left = text.Substring(k + 1, j - k - 1);
                        leftIndex = k + 1;
                    }
                    else if (text[j] == RIGHTBRACKET)
                    {
                        ////library member
                        int bracketCount = 0;
                        int k = j - 1;
                        while (k > 0 && (text[k] != 'q' || bracketCount != 0))
                        {
                            if (text[k] == 'q')
                            {
                                bracketCount--;
                            }
                            else if (text[k] == ']')
                            {
                                bracketCount++;
                            }

                            k--;
                        }

                        if (k < 0)
                        {
                            throw new ArgumentException(FormulaErrorStrings[bad_library]);
                        }

                        left = text.Substring(k, j - k + 1);
                        leftIndex = k;
                    }
                    else if (!char.IsDigit(text[j]))
                    {
                        ////number
                        throw new ArgumentException(FormulaErrorStrings[invalid_char_in_front_of] + text[i]);
                    }
                    else
                    {
                        bool period = false;

                        while (j > -1 && (char.IsDigit(text[j]) || (!period && text[j] == NumberDecimalSeparator)))
                        {
                            if (text[j] == NumberDecimalSeparator)
                            {
                                period = true;
                            }

                            j = j - 1;
                        }
                        if (j > -1 && text[j] == 'u')
                        {
                            j = j - 1;
                        }

                        if (j > -1 && period && text[j] == NumberDecimalSeparator)
                        {
                            throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);
                        }

                        j = j + 1;

                        if (j == 0 || (j > 0 && !char.IsUpper(text[j - 1]))
                            || (j == 1 && text[0] == 'u'))
                        {
                            if (j == 1 && text[0] == 'u')
                            {
                                j--;
                            }

                            left = 'n' + text.Substring(j, i - j); ////'n' for number
                            leftIndex = j;
                        }
                        else
                        {
                            ////we have a cell reference
                            j = j - 1;
                            while (j > -1 && char.IsUpper(text[j]))
                            {
                                j = j - 1;
                            }

                            j = j + 1;
                            left = text.Substring(j, i - j);
                            leftIndex = j;
                        }
                    }

                    ////process right argument
                    if (i == text.Length - 1)
                    {
                        throw new ArgumentException(FormulaErrorStrings[expression_cannot_end_with_an_operator]);
                    }
                    else
                    {
                        j = i + 1;
                        ////string
                        if (text[j] == TIC[0])
                        {
                            int k = text.Substring(j + 1).IndexOf(TIC);
                            if (k < 0)
                            {
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                            }

                            right = text.Substring(j, k + 2);
                            rightIndex = k + j + 2;
                        }
                        else if (text[j] == 'b')
                        {
                            ////block of already parsed code
                            int k = text.Substring(j + 1).IndexOf('b');
                            if (k < 0)
                            {
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                            }

                            right = text.Substring(j + 1, k);
                            rightIndex = k + j + 2;
                        }
                        else if (text[j] == 'q' || text[j] == FUNCTIONMARKER)
                        {
                            ////library
                            int bracketCount = 0;
                            int k = j + 1;
                            while (k < text.Length  && (text[k] != RIGHTBRACKET || bracketCount != 0))
                            {
                                if (text[k] == RIGHTBRACKET)
                                {
                                    bracketCount++;
                                }
                                else if (text[k] == 'q')
                                {
                                    bracketCount--;
                                }

                                k++;
                            }
                            if (k == (text.Length))
                            {
                                throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                            }

                            right = text.Substring(j, k - j + 1);
                            rightIndex = k + 1;
                        }
                        else if (char.IsDigit(text[j]) || text[j] == NumberDecimalSeparator)
                        {
                            bool period = text[j] == NumberDecimalSeparator;
                            j = j + 1;
                            while (j < text.Length && (char.IsDigit(text[j]) || (!period && text[j] == NumberDecimalSeparator)))
                            {
                                if (text[j] == NumberDecimalSeparator)
                                {
                                    period = true;
                                }

                                j = j + 1;
                            }

                            if (period && j < text.Length && text[j] == NumberDecimalSeparator)
                            {
                                throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);
                            }

                            right = 'n' + text.Substring(i + 1, j - i - 1);
                            rightIndex = j;
                        }
                        else if (char.IsUpper(text[j]) || text[j] == 'u')
                        {
                            bool period = text[j] == NumberDecimalSeparator;

                            j = j + 1;
                            while (j < text.Length && char.IsUpper(text[j]))
                            {
                                j = j + 1;
                            }

                            if (j == text.Length)
                            {
                                throw new ArgumentException(FormulaErrorStrings[invalid_characters_following_an_operator]);
                            }

                            if (!char.IsDigit(text[j]))
                            {
                                throw new ArgumentException(FormulaErrorStrings[invalid_characters_following_an_operator]);
                            }
                            
                            while (j < text.Length && (char.IsDigit(text[j]) || (!period && text[j] == NumberDecimalSeparator)))
                            {
                                if (text[j] == NumberDecimalSeparator)
                                {
                                    period = true;
                                }

                                j = j + 1;
                            }

                            j = j - 1;

                            right = text.Substring(i + 1, j - i);
                            rightIndex = j + 1;
                        }
                        else
                        {
                            throw new ArgumentException(FormulaErrorStrings[invalid_characters_following_an_operator]);
                        }
                    }

                    int p = op.IndexOf(text[i]);
                    string s = 'b' + left.Replace("b", string.Empty) + right.Replace("b", string.Empty) + markers[p] + 'b';
                    if (leftIndex > 0)
                    {
                        s = text.Substring(0, leftIndex) + s;
                    }

                    if (rightIndex < text.Length)
                    {
                        s = s + text.Substring(rightIndex);
                    }

                    s = s.Replace("bb", "b");
                    s = s.Replace(TIC + TIC, TIC);
                    text = s;
                }
            }
            else
            {
                ////no operators  ..must be number,reference or library method

                ////process left argument
                int j = text.Length - 1;

                if (text[j] == TIC[0])
                {
                    ////block of already parsed code
                    int k = text.Substring(0, j - 1).LastIndexOf(TIC);
                    if (k < 0)
                    {
                        throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                    }
                }
                else if (text[j] == 'b')
                {
                    ////block of already parsed code
                    int k = text.Substring(0, j - 1).LastIndexOf('b');
                    if (k < 0)
                    {
                        throw new ArgumentException(FormulaErrorStrings[cannot_parse]);
                    }
                }
                else if (text[j] == ']')
                {
                    ////library member
                    int bracketCount = 0;
                    int k = j - 1;
                    while (k > 0 && (text[k] != 'q' || bracketCount != 0))
                    {
                        if (text[k] == 'q')
                        {
                            bracketCount--;
                        }
                        else if (text[k] == ']')
                        {
                            bracketCount++;
                        }

                        k--;
                    }

                    ////int k = text.Substring(0, j-1).LastIndexOf('q');
                    if (k < 0)
                    {
                        throw new ArgumentException(FormulaErrorStrings[bad_library]);
                    }
                }
                else if (text[j] == this.RIGHTBRACKET)
                {
                    ////library member
                    int bracketCount = 0;
                    int k = j - 1;
                    while (k > 0 && (text[k] != '@' || bracketCount != 0))
                    {
                        if (text[k] == 'q')
                        {
                            bracketCount--;
                        }
                        else if (text[k] == this.RIGHTBRACKET)
                        {
                            bracketCount++;
                        }

                        k--;
                    }

                    if (k < 0)
                    {
                        throw new ArgumentException(this.FormulaErrorStrings[bad_library]);
                    }
                }
                else if (!char.IsDigit(text[j]))
                {
                    ////number
                    throw new ArgumentException(FormulaErrorStrings[invalid_char_in_number]);
                }
                else
                {
                    bool period = false;

                    while (j > -1 && (char.IsDigit(text[j]) || (!period && text[j] == NumberDecimalSeparator)))
                    {
                        if (text[j] == NumberDecimalSeparator)
                        {
                            period = true;
                        }

                        j = j - 1;
                    }

                    if (j > -1 && period && text[j] == NumberDecimalSeparator)
                    {
                        throw new ArgumentException(FormulaErrorStrings[number_contains_2_decimal_points]);
                    }
                }
            }

            return text;
        }

        #endregion

        #region Computations

        int computeFunctionLevel = 0;
        internal string ComputeInteriorFunctions(string formula)
        {
            try
            {
                if (formula.Length == 0)
                {
                    return formula;
                }

                computeFunctionLevel++;

                int q = formula.LastIndexOf('q');
                while (q > 0)
                {
                    int last = formula.Substring(q).IndexOf(RIGHTBRACKET);
                    if (last == -1)
                    {
                        return "bad formula";
                    }

                    string s = formula.Substring(q, last + 1);
                    formula = formula.Substring(0, q) + 'n' + ComputedValue(s) + formula.Substring(q + last + 1);
                    q = formula.LastIndexOf('q');
                }
            }
            catch (Exception ex)
            {
                ////TraceUtil.TraceExceptionCatched(ex);
                ////if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                ////    throw ex;
                return ex.Message;
            }
            finally
            {
                computeFunctionLevel--;
            }

            return formula;
        }

        private bool inValidFormulaTest = false;
        private string errorTOKEN = "*_@E$"; ////unque string

        /// <summary>
        /// Checks if an expression formula is valid for a particular host expression.
        /// </summary>
        /// <param name="expressionFieldName">The name of the host ExpressionFieldDescriptor.</param>
        /// <param name="formula">The formula.</param>
        /// <param name="errorString">Returns the error string, if any.</param>
        /// <returns>True if formula is valid, false otherwise.</returns>
        public bool IsExpressionValid(string expressionFieldName, string formula, out string errorString)
        {
            if (formula.Replace(" ","").Contains("]["))
            {
                errorString = FormulaErrorStrings[invalid_expression].ToString();
            }
            else
            {
                errorString = string.Empty;

                inValidFormulaTest = true;
                circCheckName = expressionFieldName.ToLower();
                string s = PutTokensInFormula(formula.ToLower());
                try
                {
                    s = ((IExpressionFieldEvaluator)this).Parse(s);
                    s = ComputedValue(s);
                    if (s.StartsWith(errorTOKEN))
                    {
                        errorString = FormulaErrorStrings[missing_operand];
                    }
                    else
                    {
                        foreach (string s1 in this.FormulaErrorStrings)
                        {
                            if (s == s1)
                            {
                                errorString = s;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    errorString = FormulaErrorStrings[circular_reference_] + expressionFieldName;
                }
            }
            inValidFormulaTest = false;
            return errorString.Length == 0;
        }

        ////max calculation stack depth
        private int maxStackDepth = 100;
        private int computedValueLevel = 0;
        private string circCheckName;

        internal string ComputedValue(string formula)
        {
            ////switches TraceUtil.TraceCurrentMethodInfoIf(Switches.FormulaCell.TraceVerbose, formula, computedValueLevel, this);
            if (formula == null || formula.Length == 0)
            {
                return string.Empty;
            }

            if (inValidFormulaTest)
            {
                if (circCheckName == this.cell)
                {
                    computedValueLevel = 0;
                    throw new ArgumentException(FormulaErrorStrings[circular_reference_]);
                }
            }

            try
            {
                computedValueLevel++;

                if (computedValueLevel > maximumRecursiveCalls)
                {
                    computedValueLevel = 0;
                    throw new ArgumentException(FormulaErrorStrings[too_complex]);
                }

                Stack _stack = new Stack(maxStackDepth);

                int i = 0;
                _stack.Clear();

                while (i < formula.Length)
                {
                    if (char.IsUpper(formula[i]))
                    {
                        ////cell loc
                        string s = string.Empty;
                        while (char.IsUpper(formula[i]))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        while (i < formula.Length && char.IsDigit(formula[i]))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        _stack.Push(GetValueFromDataTable(s));
                    }
                    else if (formula[i] == 'b')
                    {
                        i = i + 1;
                    }
                    else if (formula[i] == TIC[0])
                    {
                        i = i + 1;
                        string s = string.Empty;
                        bool twoTics = false;
                        while ((i < formula.Length && formula[i] != TIC[0])
                  || (twoTics = i < formula.Length - 1 && formula[i] == TIC[0] && formula[i + 1] == TIC[0]))
                        {
                            if (twoTics)
                            {
                                s += formula[i];
                                i = i + 2; ////skip 2nd tic
                                twoTics = false;
                            }
                            else
                            {
                                s += formula[i];
                                i = i + 1;
                            }
                        }

                        i = i + 1;
                        _stack.Push(s);
                    }
                    else if (formula[i] == 'q')
                    {
                        ////library
                        formula = ComputeInteriorFunctions(formula);

                        int ii = formula.Substring(i + 1).IndexOf(LEFTBRACKET);
                        if (ii > 0)
                        {
                            int bracketCount = 0;
                            bool bracketFound = false;
                            int start = ii + i + 2;
                            int k = start;
                            while (k < formula.Length && (formula[k] != RIGHTBRACKET || bracketCount > 0))
                            {
                                if (formula[k] == LEFTBRACKET)
                                {
                                    bracketCount++;
                                    bracketFound = true;
                                }
                                else if (formula[k] == LEFTBRACKET)
                                {
                                    bracketCount--;
                                }

                                k++;
                            }

                            if (bracketFound)
                            {
                                string s = formula.Substring(start, k - start - 2);
                                string s1 = string.Empty;
                                foreach (string t in s.Split(new Char[] { ',' }))
                                {
                                    if (s1.Length > 0)
                                    {
                                        s1 += ",";
                                    }

                                    int j = t.LastIndexOf('q');
                                    if (j > 0)
                                    {
                                        s1 += t.Substring(0, j) + ComputedValue(t.Substring(j));
                                    }
                                    else
                                    {
                                        s1 += ComputedValue(t);
                                    }
                                }
                                ////    s = ComputedValue(s);
                                formula = formula.Substring(0, start) + s1 + formula.Substring(k - 2);
                            }

                            string name = formula.Substring(i + 1, ii-1);
                            if (this.FunctionNames.Contains(name))
                            {
                                int j = formula.Substring(i + ii + 1).IndexOf(this.RIGHTBRACKET);
                                string args = formula.Substring(i + ii + 2, j - 1);
                                try
                                {
                                    string result = ComputeFormulaValueAt(formula);
                                    result = ParseSimple(result);
                                    _stack.Push(result);
                                }
                                catch (Exception ex)
                                {
                                    return ex.Message;
                                }

                                i += j + ii + 2;
                            }
                            else
                            {
                                return this.FormulaErrorStrings[missing_formula];
                            }
                            //////                            if(this.LibraryFunctions[name] != null)
                            //////                            {
                            //////                                int j = formula.Substring(i+ii+1).IndexOf(']');
                            //////                                string args = formula.Substring(i+ii+2, j-1);
                            //////                                try
                            //////                                {
                            //////                                    LibraryFunction func = (LibraryFunction)this.LibraryFunctions[name];
                            //////                                    _stack.Push(func(args));
                            //////                                }
                            //////                                catch(Exception ex)
                            //////                                {
                            //////                                    TraceUtil.TraceExceptionCatched(ex);
                            //////                                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                            //////                                        throw ex;
                            //////                                    return ex.Message;
                            //////                                }
                            //////                                
                            //////                                i += j + ii + 2;
                            //////                            }
                            //////                            else
                            //////                                return FormulaErrorStrings[missing_formula];
                        }
                        else if (formula[0] == 'b')
                        {
                            ////restart the processing with the formula without library finctions
                            i = 0;
                            continue;
                        }
                        else
                        {
                            return FormulaErrorStrings[improper_formula];
                        }
                    }
                    else if (char.IsDigit(formula[i]) || formula[i] == 'u')
                    {
                        string s = string.Empty;
                        if (formula[i] == 'u')
                        {
                            s = "-";
                            i++;
                        }

                        while (i < formula.Length && (char.IsDigit(formula[i]) || formula[i] == NumberDecimalSeparator))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        _stack.Push(s);
                    }
                    else if (formula[i] == 'n')
                    {
                        i = i + 1;

                        string s = string.Empty;
                        if (formula[i] == 'u')
                        {
                            s = "-";
                            i = i + 1;
                        }

                        while (i < formula.Length && (char.IsDigit(formula[i]) || formula[i] == NumberDecimalSeparator))
                        {
                            s = s + formula[i];
                            i = i + 1;
                        }

                        _stack.Push(s);
                    }
                    else
                    {
                        ////computations
                        switch (formula[i])
                        {
                            case TOKEN_add:
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 + d).ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_subtract: ////subtract
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 - d).ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_multiply: ////multiply
                                {
                                    double d = Pop(_stack); ////double.Parse(g);
                                    double d1 = Pop(_stack); ////double.Parse(g1);

                                    _stack.Push((d1 * d).ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_divide: ////division
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    _stack.Push((d1 / d).ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_less: ////lessthan
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 < d) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = date1.CompareTo(date2);
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_greater: ////greater
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 > d) ? 1 : 0;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = date2.CompareTo(date1);
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_equal: ////equal
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 == d) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = (date2.CompareTo(date1) == 0) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else
                                    {
                                        bool b = Microsoft.VisualBasic.CompilerServices.StringType.StrLikeText(
                                            o.ToString(), o1.ToString());
                                        int val = b ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_lesseq: ////lessthaneq
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 <= d) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = date1.CompareTo(date2);
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_greatereq: ////greaterthaneq
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 >= d) ? 1 : 0;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = (date2.CompareTo(date1) != -1) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_noequal: ////not equal
                                {
                                    object o = _stack.Pop();
                                    object o1 = _stack.Pop();
                                    double d;
                                    double d1;
                                    DateTime date1, date2;
                                    if (double.TryParse(o.ToString(), out d) && double.TryParse(o1.ToString(), out d1))
                                    {
                                        int val = (d1 != d) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else if (DateTime.TryParse(o.ToString(), out date1) && DateTime.TryParse(o1.ToString(), out date2))
                                    {
                                        int val = (date1.CompareTo(date2) != 0) ? TRUEVALUE : FALSEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    else
                                    {
                                        bool b = Microsoft.VisualBasic.CompilerServices.StringType.StrLikeText(
                                            o.ToString(), o1.ToString());
                                        int val = b ? FALSEVALUE : TRUEVALUE;
                                        _stack.Push(val.ToString(Culture));
                                    }
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_like: ////like
                                {
                                    string s1 = PopString(_stack);
                                    string s = PopString(_stack);
                                    bool b = true;
                                    DateTime date1;
                                    if (DateTime.TryParse(s, out date1))
                                    {
                                        switch (s1)
                                        {
                                            case "quarter1":
                                                b = (date1.Month < 4);
                                                break;
                                            case "quarter2":
                                                b = (date1.Month > 3 && date1.Month < 7); 
                                                break;
                                            case "quarter3":
                                                b = (date1.Month > 6 && date1.Month < 10);
                                                break;
                                            case "quarter4":
                                                b = (date1.Month > 9 && date1.Month < 13);
                                                break;
                                            case "january":
                                                b = (date1.Month == 1);
                                                break;
                                            case "february":
                                                b = (date1.Month == 2);
                                               break;
                                            case "march":
                                               b = (date1.Month == 3);
                                                break;
                                            case "april":
                                                b = (date1.Month == 4);
                                                break;
                                            case "may":
                                                b = (date1.Month == 5);
                                                break;
                                            case "june":
                                                b = (date1.Month == 6);
                                                break;
                                            case "july":
                                                b = (date1.Month == 7);
                                                break;
                                            case "august":
                                                b = (date1.Month == 8);
                                                break;
                                            case "september":
                                                b = (date1.Month == 9);
                                                break;
                                            case "october":
                                                b = (date1.Month == 10);
                                                break;
                                            case "november":
                                                b = (date1.Month == 11);
                                                break;
                                            case "december":
                                                b = (date1.Month == 12);
                                                break;
                                            default:
                                                b = false;
                                                break;
                                        }
                                    }
                                    else if (s.Length == 0 && s1.Length == 0)
                                    {
                                        b = true;
                                    }
                                    else if (s.Length == 0 || s1.Length == 0)
                                    {
                                        b = false;
                                    }
                                    else
                                    {
                                        if (String.Equals(s, s1, StringComparison.InvariantCultureIgnoreCase))
                                            b = true;
                                        else
                                            b = Microsoft.VisualBasic.CompilerServices.StringType.StrLike(
                                               s, s1, Microsoft.VisualBasic.CompareMethod.Text);
                                    }

                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_match: ////match
                                {
                                    string s1 = PopString(_stack);
                                    ////string s = PopString(_stack); 
                                    string s = PopString(_stack).ToLower(); ////make search case insensitive as the search string is lowercase
                                    bool b = true;
                                    if (s.Length == 0 && s1.Length == 0)
                                    {
                                        b = true;
                                    }
                                    else if (s.Length == 0 || s1.Length == 0)
                                    {
                                        b = false;
                                    }
                                    else
                                    {
                                        if (regexValue == null || oldregexValueCompare != s1)
                                        {
                                            regexValue = new System.Text.RegularExpressions.Regex(s1);
                                            oldregexValueCompare = s1;
                                        }

                                        b = regexValue.IsMatch(s);
                                    }

                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_in: ////in uses Like
                                {
                                    string s1 = PopString(_stack);
                                    string s = PopString(_stack);
                                    bool b = false;
                                    if (s.Length == 0 && s1.Length == 0)
                                    {
                                        b = true;
                                    }
                                    else if (s.Length == 0 || s1.Length == 0)
                                    {
                                        b = false;
                                    }
                                    else
                                    {
                                        s = s.Trim();
                                        foreach (string s2 in s1.Split(BRACEDELIMETER))
                                        {
                                            b |= Microsoft.VisualBasic.CompilerServices.StringType.StrLike(
                                                s, s2.Trim(), Microsoft.VisualBasic.CompareMethod.Text);

                                            ////                                if (regexValue == null || oldregexValueCompare != s2)
                                            ////                                {
                                            ////                                    regexValue = new System.Text.RegularExpressions.Regex(s2);
                                            ////                                    oldregexValueCompare = s2;
                                            ////                                }
                                            ////                                b |= regexValue.IsMatch(s);
                                        }
                                    }

                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_betweenT: ////handles dates and times - use TODAY for todays's date
                                {
                                    string s1 = PopString(_stack);
                                    string s = PopString(_stack);
                                    bool b = false;
                                    if (s.Length == 0 && s1.Length == 0)
                                    {
                                        b = true;
                                    }
                                    else if (s.Length != 0 && s1.Length != 0)
                                    {
                                        ////uses only dates, no time...
                                        DateTime leftDate = (s.Length == 0) ? DateTime.MinValue : DateTime.Parse(s, Culture);
                                        DateTime startDate = DateTime.MinValue;
                                        DateTime endDate = DateTime.MaxValue;
                                        string[] dates = s1.Split(BRACEDELIMETER);
                                        if (dates.GetLength(0) == 2)
                                        {
                                            if (dates[0] == "today")
                                            {
                                                startDate = DateTime.Now;
                                            }
                                            else if (dates[0].Length > 0)
                                            {
                                                startDate = DateTime.Parse(dates[0], Culture);
                                            }

                                            if (dates[1] == "today")
                                            {
                                                endDate = DateTime.Now;
                                            }
                                            else if (dates[1].Length > 0)
                                            {
                                                endDate = DateTime.Parse(dates[1], Culture);
                                            }
                                        }

                                        b = (startDate <= leftDate && endDate > leftDate)
                                            || (endDate == startDate && endDate == leftDate);
                                    }

                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_between: ////handles dates - use TODAY for todays's date
                                {
                                    string s1 = PopString(_stack);
                                    string s = PopString(_stack);
                                    bool b = false;
                                    if (s.Length == 0 && s1.Length == 0)
                                    {
                                        b = true;
                                    }
                                    else if (s.Length != 0 && s1.Length != 0)
                                    {
                                        ////uses only dates, no time...
                                        DateTime leftDate = (s.Length == 0) ? DateTime.MinValue.Date : DateTime.Parse(s, Culture).Date;
                                        DateTime startDate = DateTime.MinValue.Date;
                                        DateTime endDate = DateTime.MaxValue.Date;
                                        string[] dates = s1.Split(BRACEDELIMETER);
                                        if (dates.GetLength(0) == 2)
                                        {
                                            if (dates[0] == "today")
                                            {
                                                startDate = DateTime.Now.Date;
                                            }
                                            else if (dates[0].Length > 0)
                                            {
                                                startDate = DateTime.Parse(dates[0], Culture).Date;
                                            }

                                            if (dates[1] == "today")
                                            {
                                                endDate = DateTime.Now.Date;
                                            }
                                            else if (dates[1].Length > 0)
                                            {
                                                endDate = DateTime.Parse(dates[1], Culture).Date;
                                            }
                                        }

                                        b = (startDate <= leftDate && endDate > leftDate)
                                            || (endDate == startDate && endDate == leftDate);
                                    }

                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_and: ////and
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    bool b = IsEqual(d1, TRUEVALUE) && IsEqual(d, TRUEVALUE);
                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_or: ////or
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    bool b = IsEqual(d1, TRUEVALUE) || IsEqual(d, TRUEVALUE);
                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            case TOKEN_xor: ////xor
                                {
                                    double d = Pop(_stack);
                                    double d1 = Pop(_stack);

                                    bool b = (IsEqual(d1, TRUEVALUE) && !IsEqual(d, TRUEVALUE))
                                            || (IsEqual(d, TRUEVALUE) && !IsEqual(d1, TRUEVALUE));
                                    int val = b ? TRUEVALUE : FALSEVALUE;
                                    _stack.Push(val.ToString(Culture));
                                    i = i + 1;
                                }

                                break;
                            default:
                                computedValueLevel = 0;
                                throw new ArgumentException(FormulaErrorStrings[invalid_expression]);
                        }
                    }
                }

                if (_stack.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    string s = ConvertToString(_stack.Pop());
                    if (s.Length == 0)
                    {
                        s = "0"; ////empty is zero in calculation
                    }

                    return s;
                }
            }
            catch (Exception ex)
            {
                computedValueLevel = 0;
                if (ex.Message.IndexOf(FormulaErrorStrings[circular_reference_]) > -1)
                {
                    throw ex;
                }
                ////if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                ////    throw ex;
                if (ex.Message.IndexOf(FormulaErrorStrings[cell_empty]) > -1)
                {
                    return string.Empty;
                }
                else if (inValidFormulaTest)
                {
                    return errorTOKEN + ex.Message;
                }
                else
                {
                    return ex.Message; // + ":" + formula;
                }
            }
            finally
            {
                computedValueLevel--;
            }
        }

        string ConvertToString(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }

            if (o is IConvertible)
            {
                return Convert.ToString(o, Culture);
            }

            return o.ToString();
        }

        private double Pop(Stack _stack)
        {
            object o = _stack.Pop();
            if (!(o is string) || ((string)o).Length == 0)
            {
                ////throw new ArgumentException("Cell Empty");
                o = "0";
            }

            return double.Parse((string)o, Culture);
        }

        /////logical and string variable used in parsing/computing
        System.Text.RegularExpressions.Regex regexValue = null; ////used in string matches
        string oldregexValueCompare = string.Empty;
        private double ABSOLUTEZERO = 1e-20;
        private int TRUEVALUE = 1;
        private int FALSEVALUE = 0;

        private string PopString(Stack _stack)
        {
            object o = _stack.Pop();
            if (!(o is string))
            {
                ////throw new ArgumentException("Cell Empty");
                o = string.Empty;
            }

            return ConvertToString(o);
        }

        private bool IsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < ABSOLUTEZERO;
        }

        #endregion
    }
}
