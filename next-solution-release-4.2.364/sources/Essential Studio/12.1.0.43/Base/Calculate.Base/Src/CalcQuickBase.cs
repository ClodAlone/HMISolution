//-------------------------------------------------------------------------------------------------
// <copyright file="CalcQuickBase.cs" company="syncfusion">
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Calculate
{
    using System;
    using System.Collections;
    using System.Globalization;
#if !SILVERLIGHT && !WINDOWS_UWP && !WP && !NET_STANDARD
    using System.Windows.Forms;
#else
    using ArrayList = System.Collections.Generic.List<object>;
    using Stack = System.Collections.Generic.Stack<object>;
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;
#endif

    /// <summary>
    /// A class that allows you to quickly add calculation support for controls on a form, or usercontrol.
    /// </summary>
    /// <remarks>
    /// To use CalcQuick, you instantiate an instance of the class. Then just by indexing the
    /// class object with string names to identify a formula object, you can have calculation support
    /// in your form. Alternatively, you can add a collection of Control-derived objects and the
    /// CalcQuick object will bind the Control.Text property allowing you to use the Control/Name property
    /// to reference other controls in a formula.
    /// </remarks>
    /// <example>
    /// Here is code that uses three TextBoxes, the first showing a value for an angle in degrees,
    /// and the other two displaying the sine and cosine of this angle. In this code, the calculations
    /// are done on the click of a button:
    /// <code lang="C#">
    ///     CalcQuick calculator = null;
    ///        private void AngleForm_Load(object sender, System.EventArgs e)
    ///        {
    ///            //TextBox Angle = new TextBox();
    ///            this.Angle.Name = "Angle";
    ///            this.Angle.Text = "30";
    ///            //cosTB = new TextBox();
    ///            this.cosTB.Name = "cosTB";
    ///            this.cosTB.Text = "= cos([Angle] * pi() / 180) ";
    ///            //sinTB = new TextBox();
    ///            this.sinTB.Name = "sinTB";
    ///            this.sinTB.Text = "= sin([Angle] * pi() / 180) ";
    ///            // Instantiate the CalcQuick object:
    ///            this.calculator = new CalcQuick();
    ///        }
    ///        // Perform a manual calculation:
    ///        private void ComputeButton_Click(object sender, System.EventArgs e)
    ///        {
    ///            // Let the calculator know the values/formulas
    ///            // by using an indexer on the calculator object.
    ///            // Here we are using the TextBox.Name as the indexer key
    ///            // provided to the calculator object. This is not required.
    ///            // The only restriction for the indexer key values is that they
    ///            // be unique nonempty strings:
    ///            this.calculator["Angle"] = this.Angle.Text;
    ///            this.calculator["cosTB"] = this.cosTB.Text;
    ///            this.calculator["sinTB"] = this.sinTB.Text;
    ///            // Mark the calculator dirty:
    ///            this.calculator.SetDirty();
    ///            // Now as the values are retrieved from the calculator, they
    ///            // will be the newly calculated values:
    ///            this.cosTB.Text = this.calculator["cosTB"];
    ///            this.sinTB.Text = this.calculator["sinTB"];
    ///        }
    /// </code>
    /// <code lang="VB">
    /// Dim calculator As CalcQuick = Nothing
    ///    Private Sub AngleForm_Load(sender As Object, e As System.EventArgs)
    ///        'TextBox Angle = new TextBox();
    ///        Me.Angle.Name = "Angle"
    ///        Me.Angle.Text = "30"
    ///        'cosTB = new TextBox();
    ///        Me.cosTB.Name = "cosTB"
    ///        Me.cosTB.Text = "= cos([Angle] * pi() / 180) "
    /// <para/>
    ///        'sinTB = new TextBox();
    ///        Me.sinTB.Name = "sinTB"
    ///        Me.sinTB.Text = "= sin([Angle] * pi() / 180) "
    /// <para/>
    ///        'Instantiate the CalcQuick object:
    ///        Me.calculator = New CalcQuick()
    ///        End Sub 'AngleForm_Load
    /// <para/>
    ///    'Perform a manual calculation:
    ///    Private Sub ComputeButton_Click(sender As Object, e As System.EventArgs)
    ///        'Let the calculator know the values/formulas
    ///        'by using an indexer on the calculator object.
    ///        'Here we are using the TextBox.Name as the indexer key
    ///        'provided to the calculator object. This is not required.
    ///        'The only restriction for the indexer key values is that they
    ///        'be unique nonempty strings:
    ///        Me.calculator("Angle") = Me.Angle.Text
    ///        Me.calculator("cosTB") = Me.cosTB.Text
    ///        Me.calculator("sinTB") = Me.sinTB.Text
    /// <para/>
    ///        'Mark the calculator dirty:
    ///        Me.calculator.SetDirty()
    /// <para/>
    ///        'Now as the values are retrieved from the calculator, they
    ///        'will be the newly calculated values:
    ///        Me.cosTB.Text = Me.calculator("cosTB")
    ///        Me.sinTB.Text = Me.calculator("sinTB")
    ///    End Sub 'ComputeButton_Click
    /// </code>
    /// Here is code that uses the same three TextBoxes as above, but this time
    /// the code is set up to automatically compute things as you change the
    /// value in the Angle TextBox. There is no longer a need for a button handler
    /// to trigger setting / getting values.
    /// <code lang="C#">
    ///     CalcQuick calculator = null;
    /// <para/>
    ///        private void AngleForm_Load(object sender, System.EventArgs e)
    ///        {
    ///            //TextBox Angle = new TextBox();
    ///            this.Angle.Name = "Angle";
    ///            this.Angle.Text = "30";
    /// <para/>
    ///            //cosTB = new TextBox();
    ///            this.cosTB.Name = "cosTB";
    ///            this.cosTB.Text = "= cos([Angle] * pi() / 180) ";
    /// <para/>
    ///            //sinTB = new TextBox();
    ///            this.sinTB.Name = "sinTB";
    ///            this.sinTB.Text = "= sin([Angle] * pi() / 180) ";
    /// <para/>
    ///            // Instantiate the CalcQuick object:
    ///            this.calculator = new CalcQuick();
    ///        }
    /// <para/>
    ///        // Perform a manual calculation:
    ///        private void ComputeButton_Click(object sender, System.EventArgs e)
    ///        {
    ///            // Let the calculator know the values/formulas
    ///            // by using an indexer on the calculator object.
    ///            // Here we are using the TextBox.Name as the indexer key
    ///            // provided to the calculator object. This is not required.
    ///            // The only restriction for the indexer key values is that they
    ///            // be unique nonempty strings:
    ///            this.calculator["Angle"] = this.Angle.Text;
    ///            this.calculator["cosTB"] = this.cosTB.Text;
    ///            this.calculator["sinTB"] = this.sinTB.Text;
    /// <para/>
    ///            // Mark the calculator dirty:
    ///            this.calculator.SetDirty();
    /// <para/>
    ///            // Now as the values are retrieved from the calculator, they
    ///            // will be the newly calculated values:
    ///            this.cosTB.Text = this.calculator["cosTB"];
    ///            this.sinTB.Text = this.calculator["sinTB"];
    ///        }
    /// </code>
    /// <code lang="VB">
    /// Dim calculator As CalcQuick = Nothing
    /// <para/>
    ///    Private Sub AngleForm_Load(sender As Object, e As System.EventArgs)
    ///        'TextBox Angle = new TextBox();
    ///        Me.Angle.Name = "Angle"
    ///        Me.Angle.Text = "30"
    /// <para/>
    ///        'cosTB = new TextBox();
    ///        Me.cosTB.Name = "cosTB"
    ///        Me.cosTB.Text = "= cos([Angle] * pi() / 180) "
    /// <para/>
    ///        'sinTB = new TextBox();
    ///        Me.sinTB.Name = "sinTB"
    ///        Me.sinTB.Text = "= sin([Angle] * pi() / 180) "
    /// <para/>
    ///        'Instantiate the CalcQuick object:
    ///        Me.calculator = New CalcQuick()
    ///        End Sub 'AngleForm_Load
    /// <para/>
    ///    'Perform a manual calculation:
    ///    Private Sub ComputeButton_Click(sender As Object, e As System.EventArgs)
    ///        'Let the calculator know the values/formulas
    ///        'by using an indexer on the calculator object.
    ///        'Here we are using the TextBox.Name as the indexer key
    ///        'provided to the calculator object. This is not required.
    ///        'The only restriction for the indexer key values is that they
    ///        'be unique nonempty strings:
    ///        Me.calculator("Angle") = Me.Angle.Text
    ///        Me.calculator("cosTB") = Me.cosTB.Text
    ///        Me.calculator("sinTB") = Me.sinTB.Text
    /// <para/>
    ///        'Mark the calculator dirty:
    ///        Me.calculator.SetDirty()
    /// <para/>
    ///        'Now as the values are retrieved from the calculator, they
    ///        'will be the newly calculated values:
    ///        Me.cosTB.Text = Me.calculator("cosTB")
    ///        Me.sinTB.Text = Me.calculator("sinTB")
    ///    End Sub 'ComputeButton_Click
    /// </code>
    /// </example>
    public class CalcQuickBase : ISheetData, IDisposable
    {
        private int _calcQuickID = 0;

        private Hashtable _controlModifiedFlags;

        private FormulaInfoHashtable _dataStore;

        private CalcEngine _engine;

        private Hashtable _keyToRowsMap;

        private Hashtable _keyToVectors;

        private Hashtable _nameToControlMap;

        private Hashtable _rowsToKeyMap;

        private bool autoCalc = false;

        // Used internally to name the calcsheets.
        private string cellPrefix = "!0!A";
        private bool checkKeys = true;

        bool disposeEngineResource = true;

        protected bool ignoreChanges = false;
        private const string LEFTBRACE = "{";
        private string TIC = "\"";

        ////Used to change the Hashtable key references to
        ////row,col references needed by the CalcEngine.
        ////Every key is swapped for a reference like A1 or A2 or A14.
        private const char LEFTBRACKET = '[';
        private const char RIGHTBRACKET = ']';

        private string VALIDLEFTCHARS = "+-*/><=^(&" + CalcEngine.ParseArgumentSeparator;
        private string VALIDRIGHTCHARS = "+-*/><=^)&" + CalcEngine.ParseArgumentSeparator;

        #region Constructors
        /// <summary>
        /// Default constructor:
        /// </summary>
        /// <remarks>Use this constructor when you want to have
        /// several CalcQuick objects that access the same
        /// static members of the CalcEngine.
        /// </remarks>
        public CalcQuickBase()
        {
            this.InitCalcQuick(false);
        }

        /// <summary>
        /// Constructor that resets the CalcEngine object.
        /// </summary>
        /// <param name="resetStaticMembers">
        /// Indicates whether the static members of the CalcEngine class will be cleared.</param>
        public CalcQuickBase(bool resetStaticMembers)
        {
            this.InitCalcQuick(resetStaticMembers);
        }
        #endregion

        #region Events
        /// <summary>
        /// For internal CalcQuick use only.
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Occurs when one of the FormulaInfo objects being
        /// maintained by the CalcQuick instance has changed.
        /// </summary>
        public event QuickValueSetEventHandler ValueSet;
        #endregion

        #region Properties
        /// <summary>
        /// A property that gets/sets the auto calculation mode of the CalcQuick.
        /// </summary>
        /// <remarks>
        /// By default, the CalcQuick will not update other values when you change
        /// a FormulaInfo object. By default, you explicitly call SetDirty()
        /// of the CalcQuick instance to force calculations to be done the next time
        /// they are required. Setting AutoCalc to True tells the CalcQuick to maintain
        /// the dependency information necessary to automatically update
        /// dependent formulas when values that affect these formulas change.
        /// </remarks>
        public bool AutoCalc
        {
            get
            {
                return this.autoCalc;
            }

            set
            {
                this.autoCalc = value;
                this.Engine.CalculatingSuspended = !value;
                this.Engine.IgnoreValueChanged = !value;
                this.Engine.UseDependencies = value;
                if (value)
                {
                    this.SetDirty();
                }
            }
        }

        private int calcQuickID
        {
            get
            {
                this._calcQuickID++;
                if (this._calcQuickID == int.MaxValue)
                {
                    this._calcQuickID = 1;
                }

                return this._calcQuickID;
            }
        }

        /// <summary>
        /// Gets or sets whether formulas should be checked for syntax during key substitutions. Default is true.
        /// </summary>
        /// <remarks>
        /// Prior to version 4.4, no syntax checking was performed during the initial parsing process of substituting
        /// for keys (variable names enclosed in square brackets). This early syntax checking support has been added to
        /// catch cases where a keys was not preceded (or followed) properly in the formula. This CheckKeys property
        /// is available for backward compatibility. To maintain the exact parsing algorithm found in versions
        /// prior to 4.4, set this property to false.
        /// </remarks>
        public bool CheckKeys
        {
            get { return this.checkKeys; }
            set { this.checkKeys = value; }
        }

        /// <summary>
        /// Maintains a set of modified flags indicating whether
        /// any control has had a value changed.
        /// </summary>
        protected Hashtable ControlModifiedFlags
        {
            get { return this._controlModifiedFlags; }
        }

        /// <summary>
        /// Maintains a collection of FormulaInfo objects.
        /// </summary>
        /// <remarks>
        ///  This Hashtable serves as the data store for the
        ///  CalcQuick instance. The keys are the strings used
        ///  to identify formulas and the values are FormulaInfo
        ///  objects that hold the information on each formula or value.
        /// </remarks>
        protected FormulaInfoHashtable DataStore
        {
            get { return this._dataStore; }
        }

        /// <summary>
        /// Determines whether the CalcEngine object of this CalcQuick should be disposed on disposing this object.
        /// <para/>Default value is true.
        /// </summary>
        public bool DisposeEngineResource
        {
            get { return this.disposeEngineResource; }
            set { this.disposeEngineResource = value; }
        }

        /// <summary>
        /// A read-only property that gets the reference to the CalcEngine object being used by this CalcQuick instance.
        /// </summary>
        public CalcEngine Engine
        {
            get { return this._engine; }
        }

        /// <summary>
        /// A property that gets/sets character by which string starts with, are treated as
        /// formulas when indexing a CalcQuick object.
        /// </summary>
        /// <remarks>If you use the technique of indexing the CalcQuick object
        /// to set a varaible value, then you indicate that the value should be a
        /// formula by starting the string with this character. If you do not want
        /// to require your formulas to start with this character, then you will not
        /// be able to use the indexing technique. Instead, you will have to call
        /// ParseAndCompute directly to handle formulas not starting with this
        /// character.</remarks>
        public char FormulaCharacter
        {
            get
            {
                return CalcEngine.FormulaCharacter;
            }

            set
            {
                CalcEngine.FormulaCharacter = value;
            }
        }

        /// <summary>
        /// Maintains a mapping between the string key and the row
        /// used in a CalcSheet to identify a FormulaInfo object.
        /// </summary>
        protected Hashtable KeyToRowsMap
        {
            get { return this._keyToRowsMap; }
        }

        /// <summary>
        /// Maintains a mapping between the string key and a
        /// vector of numbers entered using a brace expression.
        /// </summary>
        protected Hashtable KeyToVectors
        {
            get { return this._keyToVectors; }
        }

        /// <summary>
        /// Maintains a mapping between the string key and the control
        /// which is being used to identify a FormulaInfo object.
        /// </summary>
        protected Hashtable NameToControlMap
        {
            get { return this._nameToControlMap; }
        }

        /// <summary>
        /// Maintains a mapping between the row used in a CalcSheet
        /// and the string key used to identify a FormulaInfo object.
        /// </summary>
        protected Hashtable RowsToKeyMap
        {
            get { return this._rowsToKeyMap; }
        }

        /// <summary>
        /// A method to reset all the keys registered with CalcQuickBase object.
        /// </summary>
        public void ResetKeys()
        {
            this.DataStore.Clear();
            this.KeyToRowsMap.Clear();
            this.RowsToKeyMap.Clear();
            this.KeyToVectors.Clear();
            this.NameToControlMap.Clear();
        }

        /// <summary>
        /// Gets / sets formula values for CalcQuick.
        /// </summary>
        /// <param name="key">The indexer used to identify the formula.</param>
        /// <remarks>
        /// Using an indexer on the CalcQuick instance is the primary method
        /// of setting a value to be used in a CalcQuick object. The string
        /// used as the indexer is the key that you use to reference this formula
        /// value in other formulas used in this CalcQuick instance.
        /// </remarks>
        public string this[string key]
        {
            get
            {
                key = key.ToUpper();

                if (this.DataStore.ContainsKey(key))
                {
                    FormulaInfo fInfo = this.DataStore[key];
                    string s = fInfo.FormulaText;
                    if (s.Length > 0 && s[0] == CalcEngine.FormulaCharacter &&
                        fInfo.calcID != this.Engine.GetCalcID())
                    {
                        this.Engine.cell = this.cellPrefix + this.KeyToRowsMap[key].ToString();

                        s = s.Substring(1); ////strip out formulaChar.

                        try
                        {
                            fInfo.ParsedFormula = this.Engine.ParseFormula(this.MarkKeys(s));
                        }
                        catch (Exception ex)
                        {
                            if (this.CheckKeys)
                            {
                                fInfo.FormulaValue = ex.Message;
                                fInfo.calcID = this.Engine.GetCalcID();

                                if (this.ValueSet != null)
                                {
                                    this.ValueSet(this, new QuickValueSetEventArgs(key, fInfo.FormulaValue, FormulaInfoSetAction.CalculatedValueSet));
                                }

                                return this.DataStore[key].FormulaValue;
                            }
                        }

                        try
                        {
                            fInfo.FormulaValue = this.Engine.ComputeFormula(fInfo.ParsedFormula);
                        }
                        catch (Exception ex)
                        {
                            if (this.ThrowCircularException && ex.Message.StartsWith(this.Engine.FormulaErrorStrings[this.Engine.circular_reference_]))
                            {
                                throw ex;
                            }
                        }

                        fInfo.calcID = this.Engine.GetCalcID();

                        if (this.ValueSet != null)
                        {
                            this.ValueSet(this, new QuickValueSetEventArgs(key, fInfo.FormulaValue, FormulaInfoSetAction.CalculatedValueSet));
                        }
                    }
                    if (this.Engine.ThrowCircularException)
                    {
                        if (this.Engine.IterationMaxCount > 0)
                        {
                            fInfo.FormulaValue = this.Engine.HandleIteration(this.Engine.cell, fInfo);
                        }
                    }
                    return this.DataStore[key].FormulaValue;
                }
                else if (this.KeyToVectors.ContainsKey(key))
                {
                    return this.KeyToVectors[key].ToString();
                }
                else
                {
                    return string.Empty; ////null;
                }
            }

            set
            {
                key = key.ToUpper();
                string s = value.ToString().Trim();

                ////removed the support for vectors for avoing a memory leak (case 30498)
                if (!DataStore.ContainsKey(key)/* || s.StartsWith(LEFTBRACE)*/)
                {
                    /*
                    ////support for vectors
                    if (s.StartsWith(LEFTBRACE))
                    {
                        if (!this.KeyToVectors.ContainsKey(key))
                        {
                            this.KeyToVectors.Add(key, string.Empty);
                        }

                        s = s.Substring(1, s.Length - 2);
                        int i = this.KeyToRowsMap.Count + 1;
                        string[] ss = s.Split(new char[] { CalcEngine.ParseArgumentSeparator });
                        string range = string.Format("A{0}:A{1}", i, i + ss.GetLength(0) - 1);
                        this.KeyToVectors[key] = range;
                        foreach (string s1 in ss)
                        {
                            string key1 = string.Format("Q_{0}", this.KeyToRowsMap.Count + 1);
                            this.DataStore.Add(key1, new FormulaInfo());
                            this.KeyToRowsMap.Add(key1, this.KeyToRowsMap.Count + 1);
                            this.RowsToKeyMap.Add(this.RowsToKeyMap.Count + 1, key1);

                            FormulaInfo fInfo1 = this.DataStore[key1];
                            fInfo1.FormulaText = string.Empty;
                            fInfo1.ParsedFormula = string.Empty;

                            ////fInfo1.FormulaValue = s1;
                            fInfo1.FormulaValue = this.ParseAndCompute(s1);
                        }

                        return;
                    }
                    else
                    */
                    {
                        ////simple variable
                        this.DataStore.Add(key, new FormulaInfo());
                        this.KeyToRowsMap.Add(key, this.KeyToRowsMap.Count + 1);
                        this.RowsToKeyMap.Add(this.RowsToKeyMap.Count + 1, key);
                    }
                }

                if (this.KeyToVectors.ContainsKey(key))
                {
                    this.KeyToVectors.Remove(key);
                }

                FormulaInfo fInfo = this.DataStore[key];
                if (!ignoreChanges && fInfo.FormulaText != null && fInfo.FormulaText.Length > 0 && fInfo.FormulaText != s)
                {
                    ////Reset its dependencies if there are any...
                    string s1 = this.cellPrefix + this.KeyToRowsMap[key].ToString();
                    object ht = this.Engine.DependentFormulaCells[s1];
                    if (ht != null)
                    {
                        this.Engine.ClearFormulaDependentCells(s1);
                    }
                }

                if (s.Length > 0 && s[0] == CalcEngine.FormulaCharacter)
                {
                    fInfo.FormulaText = s;
                    if (this.ValueSet != null)
                    {
                        this.ValueSet(this, new QuickValueSetEventArgs(key, s, FormulaInfoSetAction.FormulaSet));
                    }
                }
                else if (fInfo.FormulaValue != s)
                {
                    fInfo.FormulaText = string.Empty;
                    fInfo.ParsedFormula = string.Empty;
                    fInfo.FormulaValue = s;
                    if (this.ValueSet != null)
                    {
                        this.ValueSet(this, new QuickValueSetEventArgs(key, s, FormulaInfoSetAction.NonFormulaSet));
                    }
                }

                if (this.AutoCalc)
                {
                    this.UpdateDependencies(key);
                }
            }
        }

       

        /// <summary>
        /// Gets / sets whether the CalcQuick should throw an exception when a circular calculation is encountered.
        /// </summary>
        /// <remarks>If this property is True, the CalcQuick will throw an exception
        /// when it detects a circular calculation. If ThrowCircularException is False, then
        /// no exception is thrown and the calculation will loop recursively until Engine.MaximumRecursiveCalls
        /// is exceeded.
        /// </remarks>
        public bool ThrowCircularException
        {
            get { return this.Engine.ThrowCircularException; }
            set { this.Engine.ThrowCircularException = value; }
        }
        #endregion

        private bool CheckAdjacentPiece(string s, string validChars, bool first)
        {
            bool b = true;
            s = s.Trim();
            if (s.Length > 0)
            {
                b = validChars.IndexOf(s[first ? 0 : s.Length - 1]) > -1;
            }

            return b;
        }

        /// <summary>
        /// Creates the <see cref="CalcEngine"/> object used by this CalQuick object.
        /// </summary>
        /// <returns>Returns an instance of a CalcEngine object.</returns>
        /// <remarks>You can override this method and return a derived CalcEngine object use
        /// by the derived CalcQuick object.
        /// </remarks>
        public virtual CalcEngine CreateEngine()
        {
            return new CalcEngine(this);
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._dataStore = null;
            this._rowsToKeyMap = null;
            this._keyToRowsMap = null;
            this._keyToVectors = null;

            this._controlModifiedFlags = null;
            this._nameToControlMap = null;
            this.ValueChanged -= new ValueChangedEventHandler(this._engine.grid_ValueChanged);

            CalcEngine.UnregisterGridAsSheet(RangeInfo.GetAlphaLabel(calcQuickID), this);

            if (this.DisposeEngineResource)
            {
                ////CalcEngine.ResetSheetFamilyID();
                if (this._engine != null)
                {
                    this._engine.DependentFormulaCells.Clear();
                    this._engine.DependentCells.Clear();
                    this._engine.Dispose();
                }
                this._engine = null;
            }
        }
        #endregion

        /// <summary>
        /// A method that parses and computes a well-formed algebraic expression passed in.
        /// </summary>
        /// <param name="formulaText">The text of the formula.</param>
        /// <returns>The computed value.</returns>
        /// <remarks>You would use this method if you have a formula string which
        /// contains only constants or library function references. Such formulas
        /// do not depend upon other values. If you have registered a variable through
        /// an indexer, then that variable can be used in a formula expression passed into this
        /// method. This method will return the Exception text if an exception is thrown 
        /// during the computation.
        /// </remarks>
        public string TryParseAndCompute(string formulaText)
        {
            string ret = "";
            try
            {
                ret = ParseAndCompute(formulaText);
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// A method that returns the formula string associated with the key passed in from a FormulaInfo object.
        /// </summary>
        /// <param name="key">The Hashtable key associated with the FormulaInfo object.</param>
        /// <returns>The formula string may be the empty string if no formula is stored with this key.</returns>
        public string GetFormula(string key)
        {
            key = key.ToUpper();
            if (this.DataStore.ContainsKey(key))
            {
                return this.DataStore[key].FormulaText;
            }

            return string.Empty;
        }

        /// <summary>
        /// A method to get the value of the cell referred. For internal CalcQuick use only.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>(row, col) data.</returns>
        /// <remarks>
        /// CalcQuick does not expose a (row, col) data access model.
        /// But since CalcEngine requires such a model, CalcQuick uses
        /// a row, col access model internally, but only exposes the
        /// formula Key model to access values.
        /// </remarks>
        public object GetValueRowCol(int row, int col)
        {
            ////CalcQuick objects do not expose a row, col data access model.
            ////But the CalcEngine requires such a model. So, CalcQuick uses
            ////a row, col access model internally, but only exposes using
            ////formula Key values to access formulas.
            string key = this.RowsToKeyMap[row].ToString();
            string s = this[key].ToString();
            if (s != null && s.EndsWith("%") && s.Length > 1)
            {
                double d;
                if (double.TryParse(s.Substring(0, s.Length - 1), NumberStyles.Any, null, out d))
                {
                    s = (d / 100).ToString();
                }
            }

            return s;
        }

        /// <summary>
        /// Initializes any structures needed by this instance.
        /// </summary>
        /// <param name="resetStaticMembers">
        /// Indicates whether the static members of the CalcEngine class will be cleared.</param>
        protected void InitCalcQuick(bool resetStaticMembers)
        {
            this._dataStore = new FormulaInfoHashtable();
            this._rowsToKeyMap = new Hashtable();
            this._keyToRowsMap = new Hashtable();
            this._keyToVectors = new Hashtable();

            this._controlModifiedFlags = new Hashtable();
            this._nameToControlMap = new Hashtable();

            this._engine = this.CreateEngine();
            if (resetStaticMembers)
            {
                CalcEngine.ResetSheetFamilyID();
                this._engine.DependentFormulaCells.Clear();
                this._engine.DependentCells.Clear();
            }

            int i = CalcEngine.CreateSheetFamilyID();
            this.cellPrefix = string.Format("!{0}!A", i);
            this._engine.RegisterGridAsSheet(RangeInfo.GetAlphaLabel(calcQuickID), this, i);

            ////By default, we will trigger the calculations locally,
            ////so turn off the engine calculating mechanism.
            this._engine.CalculatingSuspended = true;
            this._engine.IgnoreValueChanged = true;
        }

        private string MarkKeys(string formula, bool avoidUpperCase = false)
        {
            int left = formula.IndexOf(LEFTBRACKET);
            while (left > -1)
            {
                int len = formula.Substring(left).IndexOf(RIGHTBRACKET) - 1;
                string key = string.Empty;
                string keyOrig = string.Empty;

                if (len > 0)
                {
                    keyOrig = formula.Substring(left + 1, len);
                    key = keyOrig.ToUpper();
#if !SILVERLIGHT && !WINDOWS_UWP && !WP && !NET_STANDARD
                    if (this.KeyToVectors.Contains(key))
#else
                    if (this.KeyToVectors.ContainsKey(key))
#endif
                    {
                        string rightPiece = (left + len + 2 < formula.Length) ?
                            formula.Substring(left + len + 2) : string.Empty;
                        if (this.CheckKeys && !CheckAdjacentPiece(rightPiece, VALIDRIGHTCHARS, true))
                        {
                            throw new ArgumentException(string.Format("[{0}] not followed properly", key));
                        }

                        string leftPiece = (left > 0) ? formula.Substring(0, left) : string.Empty;
                        if (this.CheckKeys && !CheckAdjacentPiece(leftPiece, VALIDLEFTCHARS, false))
                        {
                            throw new ArgumentException(string.Format("[{0}] not preceded properly", key));
                        }

                        formula = leftPiece + this.KeyToVectors[key].ToString() + rightPiece;
                        left = formula.IndexOf(LEFTBRACKET);
                    }
#if !SILVERLIGHT && !WINDOWS_UWP && !WP && !NET_STANDARD
                    else if (this.KeyToRowsMap.Contains(key))
#else
                    else if (this.KeyToRowsMap.ContainsKey(key))
#endif
                    {
                        string rightPiece = (left + len + 2 < formula.Length) ?
                            formula.Substring(left + len + 2) : string.Empty;
                        if (this.CheckKeys && !CheckAdjacentPiece(rightPiece, VALIDRIGHTCHARS, true))
                        {
                            throw new ArgumentException(string.Format("[{0}] not followed properly", key));
                        }

                        string leftPiece = (left > 0) ? formula.Substring(0, left) : string.Empty;
                        if (this.CheckKeys && !CheckAdjacentPiece(leftPiece, VALIDLEFTCHARS, false))
                        {
                            throw new ArgumentException(string.Format("[{0}] not preceded properly", key));
                        }

                        formula = leftPiece + "A" + this.KeyToRowsMap[key].ToString() + rightPiece;
                        left = formula.IndexOf(LEFTBRACKET);
                    }
                    else
                    {
                        if (formula.ToUpper().IndexOf(this.TIC + LEFTBRACKET + key + RIGHTBRACKET + this.TIC) > 0)
                        {
                            break;
                        }
                        else
                        {
                            string keyName = avoidUpperCase ? keyOrig : key;
                            throw new ArgumentException("Unknown key: " + keyName);
                        }
                    }
                }
                else
                {
                    left = -1;
                }
            }

            return formula;
        }

        /// <summary>
        /// A method that parses and computes a well-formed algebraic expression passed in.
        /// </summary>
        /// <param name="formulaText">The text of the formula.</param>
        /// <returns>The computed value.</returns>
        /// <remarks>You would use this method if you have a formula string which
        /// contains only constants or library function references. Such formulas
        /// do not depend upon other values. If you have registered a variable through
        /// an indexer, then that variable can be used in a formula expression passed into this
        /// method.
        /// </remarks>
        public string ParseAndCompute(string formulaText, bool avoidUpperCase = false)
        {
            if (formulaText.Length > 0 && formulaText[0] == CalcEngine.FormulaCharacter)
            {
                formulaText = formulaText.Substring(1);
            }

            return this.Engine.ParseAndComputeFormula(this.MarkKeys(formulaText, avoidUpperCase));
        }

        /// <summary>
        /// A method that recompute any formulas stored in the CalcQuick instance.
        /// </summary>
        /// <remarks>
        /// This method only has is used when AutoCalc is False. It loops through
        /// all FormulaInfo objects stored in the CalcQuick object and recomputes
        /// any formulas.
        /// </remarks>
        public void RefreshAllCalculations()
        {
            if (!this.AutoCalc)
            {
                return; ////only used when AutoCalc is off
            }

            this.SetDirty();

            this.ignoreChanges = true;

            foreach (string key in this.DataStore.Keys)
            {
                FormulaInfo fInfo = this.DataStore[key];
                string s = fInfo.FormulaText;
                if (s.Length > 0 && s[0] == CalcEngine.FormulaCharacter &&
                    fInfo.calcID != this.Engine.GetCalcID())
                {
                    s = s.Substring(1); ////strip out formulaChar.
                    this.Engine.cell = this.cellPrefix + this.KeyToRowsMap[key].ToString();
                    fInfo.ParsedFormula = this.Engine.ParseFormula(this.MarkKeys(s));
                    fInfo.FormulaValue = this.Engine.ComputeFormula(fInfo.ParsedFormula);
                    fInfo.calcID = this.Engine.GetCalcID();
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged(this, new ValueChangedEventArgs((int)this.KeyToRowsMap[key], 1, fInfo.FormulaValue));
                    }
                }

                if (this.ValueSet != null)
                {
                    this.ValueSet(this, new QuickValueSetEventArgs(key, fInfo.FormulaValue, FormulaInfoSetAction.CalculatedValueSet));
                }
            }

            this.ignoreChanges = false;
        }

        /// <summary>
        /// A method to force all calculations to be performed the next time the CalcQuick object is
        /// accessed with an indexer requesting the value.
        /// </summary>
        /// <remarks>
        /// Each FormulaInfo object contained in the CalcQuick instance
        /// has a calculation index that is checked any time the computed value is needed. If this index
        /// is current, no calculation is done, and the last computed value is returned. If this index
        /// is not current, the calculation is redone before the value is returned. Calling this method
        /// guarantees that no FormulaInfo object's calculation indexes will be current.
        /// </remarks>
        public void SetDirty()
        {
            this.Engine.UpdateCalcID();
        }

        /// <summary>
        /// A method to set value to the specified cell. For internal CalcQuick use only.
        /// </summary>
        public void SetValueRowCol(object value, int row, int col)
        {
            ////No implementation code in this class.
            //// TODO:  Add CalcQuick.SetValueRowCol implementation
        }

        /// <summary>
        /// Loops through and updates all formula items that depend
        /// on the FormulaInfo object pointed to by the key.
        /// </summary>
        /// <param name="key">Identifies FormulaInfo object that triggered the update.</param>
        public void UpdateDependencies(string key)
        {
            if (this.AutoCalc)
            {
                string s = this.cellPrefix + this.KeyToRowsMap[key].ToString();
                ArrayList ht = this.Engine.DependentCells[s] as ArrayList;
                this.SetDirty();
                if (ht != null)
                {
                    foreach (string s1 in ht)
                    {
                        int i = s1.IndexOf('A');
                        if (i > -1)
                        {
                            i = int.Parse(s1.Substring(i + 1));
                            key = this.RowsToKeyMap[i].ToString();
                            this.ignoreChanges = true;
                            this[key] = this[key]; ////triggers calculation in the getter
                            this.ignoreChanges = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A method that wires the ParentObject after the CalcQuick object is created. For internal CalcQuick use only.
        /// </summary>
        public void WireParentObject()
        {
            ////no implementation code in this class
            //// TODO:  Add CalcQuick.WireParentObject implementation
        }

        #region ISheetData Members

        /// <summary>
        /// Get the idex of the first row in UsedRange
        /// </summary>
        /// <returns>index of first row</returns>
        public int GetFirstRow()
        {
            return 1;
        }

        /// <summary>
        /// get the index of the last row in UsedRange
        /// </summary>
        /// <returns>index of last row</returns>
        public int GetLastRow()
        {
            return this.RowsToKeyMap.Count;
        }

        /// <summary>
        /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
        /// </summary>
        /// <returns></returns>
        public int GetRowCount()
        {
            return this.RowsToKeyMap.Count;
        }

        /// <summary>
        /// Gets the first column index.
        /// </summary>
        /// <returns>Index of first column</returns>
        public int GetFirstColumn()
        {
            return 1;
        }

        /// <summary>
        /// Gets the last column index / column count.
        /// </summary>
        /// <returns>Index of last column</returns>
        public int GetLastColumn()
        {
            //
            return 25;
        }
        /// <summary>
        /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
        /// </summary>
        /// <returns></returns>
        public int GetColumnCount()
        {
            //
            return 25;
        }
        #endregion
    }

    #region ValueSet event

    /// <summary>
    /// An event handler that represents the method to handle the <see cref="CalcQuick.ValueSet"/> event.
    /// </summary>
    /// <remarks>
    /// This event is raised whenever an indexer is used on the CalcQuick object to assign
    /// it a value or when a value is assigned as the result of a calculation being done.
    /// </remarks>
    public delegate void QuickValueSetEventHandler(object sender, QuickValueSetEventArgs e);

    /// <summary>
    /// Event argument class for the <see cref="CalcQuick.ValueSet"/> event.
    /// </summary>
    public class QuickValueSetEventArgs : EventArgs
    {
        private FormulaInfoSetAction action;
        private string id;
        private string val;

        /// <summary>
        /// The only constructor for QuickValueSetEventArgs.
        /// </summary>
        /// <param name="key">This is the object that is used as the key value in the Hashtable to
        /// identify the formula information. It is also the string you use in formulas (enclosed in brackets)
        /// to reference a formula from another formula.</param>
        /// <param name="value">New value being assigned.</param>
        /// <param name="action">Indicates the reason the event is being raised. See FormulaInfoSetAction.</param>
        public QuickValueSetEventArgs(string key, string value, FormulaInfoSetAction action)
        {
            this.id = key;
            this.val = value;
            this.action = action;
        }

        /// <summary>
        /// The reason the event was raised.
        /// </summary>
        public FormulaInfoSetAction Action
        {
            get { return this.action; }
            set { action = value; }
        }

        /// <summary>
        /// A property that gets/sets the Hashtable lookup object for the FormulaInfo object being changed.
        /// </summary>
        public string Key
        {
            get { return this.id; }
            set { id = value; }
        }

        /// <summary>
        /// A property that gets/sets the new value being set.
        /// </summary>
        public string Value
        {
            get { return this.val; }
            set { val = value; }
        }
    }

    /// <summary>
    /// Flags the reason that quickValueSet was raised.
    /// </summary>
    /// <remarks>When QuickValueSet event is raised, it passes an
    /// argument of FormulaInfoSetAction to indicate what was being assigned
    /// to the CalcQuick object using an indexer. </remarks>
    public enum FormulaInfoSetAction
    {
        /// <summary>
        /// A formula (string starting with FormulaCharacter) was assigned.
        /// </summary>
        FormulaSet = 0,

        /// <summary>
        /// Something other than a formula was assigned.
        /// </summary>
        NonFormulaSet,

        /// <summary>
        /// A calculated value was assigned.
        /// </summary>
        CalculatedValueSet
    }
    #endregion

    #region FormulaInfoHashtable class

    /// <summary>
    /// Typed Hashtable returning FormulaInfo objects.
    /// </summary>
    public class FormulaInfoHashtable : Hashtable
    {
        /// <summary>
        /// Gets or sets the FormulaInfo with the specified obj.
        /// </summary>
        /// <param name="obj">The key to identify the given FormulaInfo.</param>
        /// <value>FormulaInfo</value>
        public new FormulaInfo this[object obj]
        {
            get
            {
                return base[obj] as FormulaInfo;
            }

            set
            {
                base[obj] = value;
            }
        }
    }
    #endregion
}
