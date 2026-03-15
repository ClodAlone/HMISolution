//-------------------------------------------------------------------------------------------------
// <copyright file="CalcSheet.cs" company="syncfusion">
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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// An event handler that represents the method to handle the <see cref="CalcSheet.ValueChanged"/>
    /// event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ValueChangedEventArgs"/> that contains the event data.</param>
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    /// <summary>
    /// CalcSheet represents a single worksheet in a workbook.
    /// </summary>
    /// <remarks>
    /// A CalcSheet plays the role of an Excel Worksheet. It maintains its own internal data object
    /// to hold FormulaInfo objects used by the CalcEngine in its calculation work. The CalcSheet accesses
    /// the raw data / formulas that you want to use in the calculations via the ICalcData interface.
    /// </remarks>
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
    [Serializable]
#endif
    public class CalcSheet : ISheetData
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        , ISerializable
#endif
    {
        private bool calcSuspended = true;
        internal object[,] data;

        [ThreadStaticAttribute]
        private static char delimiter = '\t';

        internal CalcEngine engine = null;

        ////Internal flag to note when object is being serialized.
        private bool inSerialization = false;

        private bool lockSheetChanges = false;

        private string name = "";

        private int VersionNumber = 0;

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalcSheet()
        {
            this.data = null;
        }

        /// <summary>
        /// Constructor that initializes an internal object array to
        /// hold data.
        /// </summary>
        /// <param name="rows">Number of rows in the data object.</param>
        /// <param name="cols">Number of columns in the data object.</param>
        public CalcSheet(int rows, int cols)
        {
            this.data = new object[rows, cols];
        }

#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        /// <summary>
        /// Constructor used during serialization.
        /// </summary>
        protected CalcSheet(SerializationInfo info, StreamingContext context)
        {
            this.name = (string)info.GetValue("name", typeof(string));
            int rowCount = (int)info.GetValue("rowCount", typeof(int));
            int colCount = (int)info.GetValue("colCount", typeof(int));
            this.data = new object[rowCount, colCount];
            string text = (string)info.GetValue("data", typeof(string));
            int start = 0;
            int len;
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    len = text.Substring(start).IndexOf(delimiter);
                    this.data[row, col] = text.Substring(start, len);

                    ////this.SetValue(row, col, text.Substring(start, len));
                    start = start + len + 1;
                }
            }
        }
#endif
        #endregion

        #region Events
        /// <summary>
        /// This event is raised in SetValueRowCol.
        /// </summary>
        public event Syncfusion.Calculate.ValueChangedEventHandler CalculatedValueChanged;

        /// <summary>
        /// This event should be raised by the implementer of ICalcData whenever a value changes.
        /// </summary>
        public event Syncfusion.Calculate.ValueChangedEventHandler ValueChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / sets a flag that indicates whether to compute dependent values
        /// as cells change.
        /// </summary>
        public bool CalculationsSuspended
        {
            get { return this.calcSuspended; }
            set { calcSuspended = value; }
        }

        /// <summary>
        /// A read-only property that gets the column count.
        /// </summary>
        public int ColCount
        {
            get { return this.data.GetLength(1); }
        }

        /// <summary>
        /// Gets / sets the field delimiter for the
        /// WriteSheetToFile method.
        /// </summary>
        /// <remarks>The default value is tab.</remarks>
        public static char Delimter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        /// <summary>
        /// A read-only property that gets the CalcEngine object used by this CalcSheet.
        /// </summary>
        public CalcEngine Engine
        {
            get { return this.engine; }
            set { this.Engine = value; }
        }

        /// <summary>
        /// Enables or disables saving changes within the CalcSheet.
        /// </summary>
        public bool LockSheetChanges
        {
            get { return this.lockSheetChanges; }
            set { lockSheetChanges = value; }
        }

        /// <summary>
        /// A property that gets / sets the name used to refer to this sheet.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { name = value; }
        }

        /// <summary>
        /// A read-only property that gets the row count.
        /// </summary>
        public int RowCount
        {
            get { return this.data.GetLength(0); }
        }

        /// <summary>
        /// Gets / sets a value through the ICalcData.GetValueRowCol
        /// and ICalcData.SetValueRowCol implementation methods.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        public object this[int row, int col]
        {  
            ////Row and col are always one-based.
            get
            {
                return (this as ICalcData).GetValueRowCol(row, col);
            }

            set
            {
                this.SetValue(row, col, value.ToString());
            }
        }
        #endregion
#if !WINDOWS_UWP
        /// <summary>
        /// Creates a CalcSheet from a delimited text file
        /// created by WriteSheetToFile.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A CalcSheet instantiated with the file content.</returns>
        public static CalcSheet CreateSheetFromFile(string fileName)
        {
            CalcSheet sheet = null;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String s;
                    s = sr.ReadLine();
                    int version = int.Parse(s);
                    sheet = ReadSSS(sr);
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sheet;
        }
#endif

#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        /// <summary>
        /// Supports serialization.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Describes source and destination of the given stream..</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this.name);
            info.AddValue("rowCount", this.RowCount);
            info.AddValue("colCount", this.ColCount);

            this.inSerialization = true;
            StringBuilder sb = new StringBuilder();
            for (int row = 1;row <= this.RowCount;++row)
            {
                for (int col = 1;col <= this.ColCount;++col)
                {
                    ////string key = RangeInfo.GetAlphaLabel(col) + row.ToString();
                    object val = ((ICalcData)this).GetValueRowCol(row, col);
                    sb.AppendFormat("{0}{1}", val, delimiter);
                }
            }

            info.AddValue("data", sb.ToString());
            this.inSerialization = false;
        }
        
#endif
        /// <summary>
        /// Returns the value at the row and column.
        /// </summary>
        /// <param name="row">One-based row index.</param>
        /// <param name="col">One based column index.</param>
        /// <returns>The cell value.</returns>
        public virtual object GetValueRowCol(int row, int col)
        {
            ////Console.WriteLine("GetValue:  Row: {0}, Col: {1}, Value: {2}", row, col, this.data[row, col]);
            if (row <= this.data.GetLength(0) && col <= this.data.GetLength(1))
            {
                if (this.inSerialization && engine != null)
                {
                    string s = this.engine.GetFormulaRowCol(this, row, col);
                    if (s.Length > 0)
                    {
                        return s;
                    }
                }

                return this.data[row - 1, col - 1];
            }
            else
            {
                ////Console.WriteLine("Requested Index out of range  Row: {0}, Col: {1}", row, col);
                return "0";
            }
        }

        /// <summary>
        /// Raises the CalculatedValueChanged event.
        /// </summary>
        /// <param name="e">Includes the row, col, and value of the change.</param>
        /// <remarks>This event should be raised any time a value changes.</remarks>
        protected virtual void OnCalculatedValueChanged(ValueChangedEventArgs e)
        {
            if (this.CalculatedValueChanged != null)
            {
                this.CalculatedValueChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="e">Includes the row, col, and value of the change.</param>
        /// <remarks>This event should be raised any time a value changes.</remarks>
        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Creates a CalcSheet object from the content of a StreamReader.
        /// </summary>
        /// <param name="sr">The StreamReader.</param>
        /// <returns>The newly created CalcSheet object.</returns>
        public static CalcSheet ReadSSS(StreamReader sr)
        {
            ////SSS code
            string s = sr.ReadLine();
            int rowCount = int.Parse(s);
            s = sr.ReadLine();
            int colCount = int.Parse(s);
            CalcSheet sheet = new CalcSheet(rowCount, colCount);
            s = sr.ReadLine();
            sheet.name = s;
            for (int row = 0; row < rowCount; ++row)
            {
                s = sr.ReadLine();
                int col = 0;
                foreach (string s1 in s.Split(delimiter))
                {
                    ////if(s1.Length > 0)
                    sheet.data[row, col] = s1;
                    col++;
                }
            }

            return sheet;
        }

        /// <summary>
        /// A Virtual method to save the value through the ICalcData.SetValueRowCol implementation method
        /// and raise the ValueChanged event.
        /// </summary>
        /// <param name="row">The row index, one-based.</param>
        /// <param name="col">The column index, one-based.</param>
        /// <param name="val">The value.</param>
        public virtual void SetValue(int row, int col, string val)
        {
            if (this.LockSheetChanges)
            {
                return;
            }

            this.SetValueRowCol(val, row, col);

            ////this.data[row, col] = val;

            if (this.CalculationsSuspended)
            {
                return;
            }

            this.Engine.GetFormulaText(ref val);
            ValueChangedEventArgs e1 = new ValueChangedEventArgs(row, col, val);
            this.OnValueChanged(e1);
        }

        /// <summary>
        /// A Virtual method to set a value at a given row and column.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        /// <param name="row">One-based row index.</param>
        /// <param name="col">One-based column index.</param>
        public virtual void SetValueRowCol(object value, int row, int col)
        {
            ////Row and col are always one-based...
            if (!LockSheetChanges)
            {
                this.data[row - 1, col - 1] = value;
                if (this.CalculatedValueChanged != null)
                {
                    ValueChangedEventArgs e1 = new ValueChangedEventArgs(row, col, value.ToString());
                    this.OnCalculatedValueChanged(e1);
                }
            }

            ////Console.WriteLine("SetValue:  Row: {0}, Col: {1}, Value: {2}", row, col, value);
        }


        /// <summary>
        /// A Virtual method that can be used to handle subscribing to any base object events necessary for implementing the
        /// ValueChanged event.
        /// </summary>
        /// <remarks>For example, when implementing ICalcData on a DataGrid-derived object,
        /// you need to know when something in the DataGrid changes so you can raise the
        /// required ICalcData.ValueChanged event. In WireParentObject, the DataGrid can
        /// subscribe to its DataSource's change event to handle this requirement. If the
        /// DataSource is a DataTable, then the DataTable.ColumnChanged event can fulfill
        /// the requirement.</remarks>
        public virtual void WireParentObject()
        {
            ////no implementation
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
            return this.RowCount;
        }

        /// <summary>
        /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
        /// </summary>
        /// <returns></returns>
        public int GetRowCount()
        {
            return this.RowCount;
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
            return this.ColCount;
        }
        /// <summary>
        /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
        /// </summary>
        /// <returns></returns>
        public int GetColumnCount()
        {
            return this.ColCount;
        }
        #endregion

#if !WINDOWS_UWP
        /// <summary>
        /// Writes a delimited file.
        /// </summary>
        /// <param name="fileName">The output file name.</param>
        /// <remarks>The static Delimiter member specifies the field delimiter.
        /// Rows are delimited by Environment.NewLine characters.
        /// This method serializes formulas instead of computed values. To
        /// write a file containing computed values, use the WriteValuesToFile method.</remarks>
        public void WriteSheetToFile(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(this.VersionNumber);
                    this.WriteSSS(sw);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
#endif
        /// <summary>
        /// Writes this CalcSheet object to the given StreamWriter.
        /// </summary>
        /// <param name="sw">The StreamWriter.</param>
        /// <param name="valuesOnly">Indicates whether to serialize formulas (False)
        /// or computed values (True).</param>
        public void WriteSSS(StreamWriter sw, bool valuesOnly)
        {
            ////SSS code
            int rowCount = this.data.GetLength(0);
            int colCount = this.data.GetLength(1);
            sw.WriteLine(rowCount);
            sw.WriteLine(colCount);
            sw.WriteLine(this.name);
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    if (col > 0)
                    {
                        sw.Write(delimiter);
                    }

                    if (!valuesOnly)
                    {
                        string s = this.engine.GetFormulaRowCol(this, row + 1, col + 1); ////row col need to be one based
                        if (s.Length > 0)
                        {
                            sw.Write(s);
                            continue;
                        }
                    }

                    sw.Write(this.data[row, col]);
                }

                sw.WriteLine("");
            }
        }
        /// <summary>
        /// Writes this CalcSheet object with formulas serialized to the given StreamWriter.
        /// </summary>
        /// <param name="sw">The StreamWriter.</param>
        public void WriteSSS(StreamWriter sw)
        {
            this.WriteSSS(sw, false);
        }
#if !WINDOWS_UWP
        /// <summary>
        /// Serializes the data with computed values to a delimited
        /// text file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <remarks>The static Delimiter member specifies the field delimiter.
        /// Rows are delimited by Environment.NewLine characters.
        /// This method serializes computed values instead of formulas. To
        /// write a file containing formulas, use the WriteSheetToFile method.</remarks>
        public void WriteValuesToFile(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    this.WriteSSS(sw, true);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
#endif

    }
}
