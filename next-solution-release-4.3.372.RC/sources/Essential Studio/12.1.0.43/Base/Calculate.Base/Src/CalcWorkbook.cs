//-------------------------------------------------------------------------------------------------
// <copyright file="CalcWorkbook.cs" company="syncfusion">
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
#if  SILVERLIGHT || WINDOWS_UWP || WP
    using ArrayList = System.Collections.Generic.List<object>;
    using Stack = System.Collections.Generic.Stack<object>;
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;
#endif

    /// <summary>
    /// CalcWorkbook holds a collection of <see cref="CalcSheet"/> objects.
    /// </summary>
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
    [Serializable]
#endif
    public class CalcWorkbook
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        : ISerializable
#endif
    {
        private CalcSheetList calcSheetList;

        ////private Hashtable namedRanges;

        private CalcEngine engine = null;
        internal Hashtable idLookUp = null;

        internal int sheetfamilyID = -1;

        /// <summary>
        /// ArrayList of strings holding the CalcSheets names.
        /// </summary>
        [Obsolete("This field will be removed in a future version. Please use the CalcSheetList property instead.", false)]
        public ArrayList sheetNames;

        private int VersionNumber = 0;

        #region Constructors
        ////NameRanges keys must be upper case...
        
        /// <summary>
        /// Initializes a new instance of the CalcWorkbook class.
        /// </summary>
        /// <param name="calcSheets">The calc sheets.</param>
        /// <param name="namedRanges">Hashtable of key, value pairs for Namedrange values.</param>
        public CalcWorkbook(CalcSheet[] calcSheets, Hashtable namedRanges)
        {
            this.calcSheetList = new CalcSheetList(calcSheets, this);

            int sheetCount = this.calcSheetList.Count;
            this.sheetNames = new ArrayList(sheetCount); ////Obsolete
            this.idLookUp = new Hashtable();
            this.InitCalcWorkbook(sheetCount);
            if (sheetCount > 0)
            {
                Hashtable namedRanges1 = new Hashtable();
                if (namedRanges != null)
                {
                    foreach (string s in namedRanges.Keys)
                    {
                        namedRanges1.Add(
#if WINDOWS_UWP
         s.ToUpperInvariant()
#else
                        s.ToUpper(System.Globalization.CultureInfo.InvariantCulture)
#endif
         , namedRanges[s]);
                    }

                }
                this.engine.NamedRanges = namedRanges1;
            }
        }

#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        /// <summary>
        /// Initializes a new <see cref="CalcWorkbook"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected CalcWorkbook(SerializationInfo info, StreamingContext context)
        {
            this.calcSheetList = new CalcSheetList((CalcSheet[])info.GetValue("calcSheets", typeof(CalcSheet[])), this);

            Hashtable ht = (Hashtable)info.GetValue("namedRanges", typeof(Hashtable));

            ////this.sheetNames = (ArrayList) info.GetValue("sheetNames", typeof(ArrayList));
            int sheetCount = this.calcSheetList.Count;
            this.idLookUp = new Hashtable();
            this.sheetNames = new ArrayList(sheetCount); ////obsolete
            this.InitCalcWorkbook(sheetCount);
            this.engine.NamedRanges = ht;
        }
        
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets an Arraylist of CalcSheet objects used in this workbook.
        /// </summary>
        public CalcSheetList CalcSheetList
        {
            get { return this.calcSheetList; }
            set { calcSheetList = value; }
        }

        /// <summary>
        /// Array of CalcSheets objects used in this workbook.
        /// </summary>
        [Obsolete("This property will be removed in a future version. Please use the CalcSheetList property instead.", false)]
        public CalcSheet[] calcSheets
        {
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
            get { return (CalcSheet[])calcSheetList.ToArray(typeof(CalcSheet)); }
#else
            get { return (CalcSheet[])calcSheetList.ToArray(); }
#endif
        }

        /// <summary>
        /// Gets/Sets the CalcEngine object.
        /// </summary>
        /// <remarks>
        /// The setter only sets once, and only if InitCalcWorkbook has been called
        /// with a zero CalcSheet count.
        /// </remarks>
        public CalcEngine Engine
        {
            get
            {
                return this.engine;
            }

            set
            {
                if (this.engine == null)
                {
                    this.engine = value;
                }
            }
        }

        /// <summary>
        /// Number of CalcSheets in this workbook.
        /// </summary>
        public int SheetCount
        {
            get
            {
                return this.calcSheetList.Count;
            }
        }

        /// <summary>
        /// Gets / sets a CalcSheet object with the given name.
        /// </summary>
        /// <param name="sheetName">The sheet name.</param>
        public CalcSheet this[string sheetName]
        {
            get { return this.calcSheetList[GetSheetID(sheetName)]; }
            set { this.calcSheetList[GetSheetID(sheetName)].data = value.data; }
        }

        /// <summary>
        /// Gets / sets a CalcSheet object with the given index.
        /// </summary>
        /// <param name="sheetIndex">The sheet index.</param>
        public CalcSheet this[int sheetIndex]
        {
            get
            {
                return this.calcSheetList[sheetIndex];
            }

            set
            {
                this.calcSheetList[sheetIndex].data = value.data;
            }
        }
        #endregion

        /// <summary>
        /// A Virtual method to calculate all formulas in this workbook.
        /// </summary>
        public virtual void CalculateAll()
        {
            foreach (CalcSheet sheet in this.calcSheetList)
            {
                sheet.CalculationsSuspended = false;
            }

            foreach (CalcSheet sheet in this.calcSheetList)
            {
                sheet.Engine.UpdateCalcID();
                for (int row = 1; row <= sheet.RowCount; ++row)
                {
                    for (int col = 1; col <= sheet.ColCount; ++col)
                    {
                        object o = sheet[row, col];
                        if (o != null)
                        {
                            string s2 = o.ToString();
                            if (s2.Length > 0 && s2[0] == CalcEngine.FormulaCharacter)
                            {
                                sheet[row, col] = s2;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all formulas in the given CalcSheet.
        /// </summary>
        /// <param name="sheet">The CalcSheet.</param>
        public void ClearFormulas(CalcSheet sheet)
        {
            DateTime start = DateTime.Now;
            string keyToRemove = string.Format("!{0}!", this.GetSheetID(sheet.Name));
            ArrayList remove = new ArrayList();
            foreach (string key in this.Engine.FormulaInfoTable.Keys)
            {
                if (key.StartsWith(keyToRemove))
                {
                    remove.Add(key);
                }
            }

            ////clear up dependencies as well...
            foreach (string key in remove)
            {
                this.Engine.FormulaInfoTable.Remove(key);
                ArrayList remove1 = new ArrayList();
                foreach (string key1 in this.Engine.DependentCells.Keys)
                {
                    if (key1.StartsWith(keyToRemove))
                    {
                        remove1.Add(key1);
                    }
                }

                foreach (string key1 in remove1)
                {
                    this.Engine.DependentCells.Remove(key1);
                }

                remove1.Clear();

                foreach (string key1 in this.Engine.DependentFormulaCells.Keys)
                {
                    if (key1.StartsWith(keyToRemove))
                    {
                        remove1.Add(key1);
                    }
                }

                foreach (string key1 in remove1)
                {
                    this.Engine.DependentFormulaCells.Remove(key1);
                }
            }
        }

#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the workbook.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the cell model.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("calcSheets", this.calcSheetList.ToArray());
            info.AddValue("namedRanges", this.Engine.NamedRanges);
        }
        
#endif
        /// <summary>
        /// A method that gets the integer ID of a CalcSheet.
        /// </summary>
        /// <param name="sheetName">The CalcSheet name.</param>
        /// <returns>The integer ID.</returns>
        public int GetSheetID(string sheetName)
        {
            return this.idLookUp.ContainsKey(sheetName.ToLower())
                ? (int)this.idLookUp[sheetName.ToLower()] : -1;
        }

        private void GetSheetRowCol(string key, out int sheet, out int row, out int col)
        {
            key = key.Substring(1);
            int i = key.IndexOf('!');
            sheet = int.Parse(key.Substring(0, i));
            key = key.Substring(i + 1);
            row = this.Engine.RowIndex(key);
            col = this.Engine.ColIndex(key);
        }

        private void InitCalcWorkbook(int sheetCount)
        {
            CalcEngine.ResetSheetFamilyID();
            if (sheetCount > 0)
            {
                this.engine = new CalcEngine((CalcSheet)calcSheetList[0]);
                this.engine.UseDependencies = true;
                this.sheetfamilyID = CalcEngine.CreateSheetFamilyID();

                for (int i = 0; i < sheetCount; i++)
                {
                    string s = ((CalcSheet)calcSheetList[i]).Name;
                    this.engine.RegisterGridAsSheet(s, ((CalcSheet)calcSheetList[i]), sheetfamilyID);

                    ////sheetNames is obsolete - still used for backward compatibility
                    this.sheetNames.Add(s); ////Obsolete

                    this.idLookUp.Add(s.ToLower(), i);
                    ((CalcSheet)calcSheetList[i]).engine = engine;
                }
            }
        }
#if !WINDOWS_UWP
        /// <summary>
        /// Creates a CalcWorkbook from a file written using WriteSSS.
        /// </summary>
        /// <param name="fileName">Pathname of the file.</param>
        /// <returns>A CalcWorkbook object.</returns>
        public static CalcWorkbook ReadSSS(string fileName)
        {
            ////SSS code
            CalcWorkbook proc = null;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String s;
                    s = sr.ReadLine();
                    int version = int.Parse(s);
                    s = sr.ReadLine();
                    int nSheets = int.Parse(s);
                    s = sr.ReadLine();
                    int nRanges = int.Parse(s);
                    Hashtable ht = new Hashtable(nRanges);
                    for (int i = 0; i < nRanges; ++i)
                    {
                        s = sr.ReadLine();
                        string[] cols = s.Split('\t');
                        ht.Add(cols[0], cols[1]);
                    }

                    CalcSheet[] sheets = new CalcSheet[nSheets];
                    for (int i = 0; i < nSheets; ++i)
                    {
                        sheets[i] = CalcSheet.ReadSSS(sr);
                    }

                    sr.Close();
                    proc = new CalcWorkbook(sheets, ht);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return proc;
        }

        /// <summary>
        /// Writes a tab-delimited file holding the Workbook information.
        /// </summary>
        /// <param name="fileName">The pathname of the file to be written.</param>
        public void WriteSSS(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(this.VersionNumber);
                    int nSheets = this.calcSheetList.Count;
                    sw.WriteLine(nSheets);
                    sw.WriteLine(this.Engine.NamedRanges.Count);
                    foreach (string key in this.Engine.NamedRanges.Keys)
                    {
                        sw.Write(key);
                        sw.Write('\t');
                        sw.WriteLine(this.Engine.NamedRanges[key]);
                    }

                    for (int i = 0; i < nSheets; ++i)
                    {
                        ((CalcSheet)calcSheetList[i]).WriteSSS(sw);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
#endif
    }

    /// <summary>
    /// Wrapper ArrayList that holds a collection of CalcSheets.
    /// </summary>
    public class CalcSheetList : ArrayList
    {
        private static int nextCalcSheetNumber = 0;
        private CalcWorkbook workBook = null;

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalcSheetList()
        {
        }

        /// <summary>
        /// Creates an CalcSheetList instance owned by the given workbook with the given CalcSheet list.
        /// </summary>
        /// <param name="list">list of CalcSheets</param>
        /// <param name="parentWorkBook">The Workbook</param>
        public CalcSheetList(CalcSheet[] list, CalcWorkbook parentWorkBook)
            : base()
        {
            if (list != null)
            {
                foreach (CalcSheet sheet in list)
                {
                    base.Add(sheet);
                    nextCalcSheetNumber++;
                }
            }

            this.workBook = parentWorkBook;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the CalcSheet at the given index.
        /// </summary>
        /// <param name="i">The sheet index.</param>
        public new CalcSheet this[int i]
        {
            get
            {
                return (CalcSheet)base[i];
            }

            set
            {
                base[i] = value;
            }
        }

        /// <summary>
        /// Gets or sets the CalcSheet with a given name.
        /// </summary>
        /// <param name="sheetName">The sheet name.</param>
        public CalcSheet this[string sheetName]
        {
            get
            {
                int i = this.NameToIndex(sheetName);
                if (i == -1)
                {
                    throw new ArgumentOutOfRangeException(string.Format("{0} not found.", sheetName));
                }

                return (CalcSheet)base[i];
            }

            set
            {
                int i = this.NameToIndex(sheetName);
                if (i == -1)
                {
                    throw new ArgumentOutOfRangeException(string.Format("{0} not found.", sheetName));
                }

                base[i] = value;
            }
        }
        #endregion

        /// <summary>
        /// A method that adds a new CalcSheet.
        /// </summary>
        /// <param name="o">The CalcSheet to be added.</param>
        /// <returns>The index of the added CalcSheet.</returns>
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
        public new int Add(object o)
#else
        public new void Add(object o)
#endif
        {
            CalcSheet sheet = o as CalcSheet;
            if (sheet == null)
            {
                throw new ArgumentException("Must add a CalcSheet object");
            }

            CalcEngine engine = null;
            if (this.Count == 0)
            {
                CalcEngine.ResetSheetFamilyID();
                engine = new CalcEngine(sheet);
                engine.UseDependencies = true;
                if (this.workBook.sheetfamilyID == -1)
                {
                    this.workBook.sheetfamilyID = CalcEngine.CreateSheetFamilyID();
                }
            }
            else
            {
                engine = this[0].engine;
            }

            sheet.engine = engine;
            string s = sheet.Name;
            engine.RegisterGridAsSheet(s, sheet, this.workBook.sheetfamilyID);

            ////sheetNames is obsolete - still used for backward compatibility
            this.workBook.sheetNames.Add(s); ////obsolete

            int i = nextCalcSheetNumber;
            nextCalcSheetNumber++;
            this.workBook.idLookUp.Add(s.ToLower(), i);

#if !SILVERLIGHT && !WINDOWS_UWP && !WP
            return base.Add(sheet);
#else
            base.Add(sheet);
#endif
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public new void Insert(int index, object o)
        {
            throw new NotImplementedException("Insert");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public new void InsertRange(int index, ICollection c)
        {
            throw new NotImplementedException("InsertRange");
        }

        /// <summary>
        /// Returns the index for a CalcSheet.
        /// </summary>
        /// <param name="sheetName">The name of the CalcSheet.</param>
        /// <returns>The index of the CalcSheet.</returns>
        public int NameToIndex(string sheetName)
        {
            int index = -1;
            string s = sheetName.ToLower();
            for (int i = 0; i < this.Count; ++i)
            {
                if (this[i].Name.ToLower() == s)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// A method that removes a CalcSheet.
        /// </summary>
        /// <param name="o">The CalcSheet to be removed.</param>
        public new void Remove(object o)
        {
            CalcSheet sheet = o as CalcSheet;
            if (sheet == null)
            {
                throw new ArgumentException("Must add a CalcSheet object");
            }

            this.workBook.sheetNames.Remove(sheet.Name); ////obsolete
            this.workBook.idLookUp.Remove(sheet.Name.ToLower());

            GridSheetFamilyItem family = CalcEngine.GetSheetFamilyItem(sheet);
            if (family.SheetNameToToken.ContainsKey(sheet.Name.ToUpper()))
            {
                family.SheetNameToToken.Remove(sheet.Name.ToUpper());
            }

            base.Remove(o);
        }

        /// <summary>
        /// Removes a CalcSheet.
        /// </summary>
        /// <param name="index">The index of the CalcSheet to be removed.</param>
        public new void RemoveAt(int index)
        {
            CalcSheet o = this[index];
            this.Remove(o);
        }

        /// <summary>
        ///  The CalcSheets in this collection.
        /// </summary>
        /// <returns>Returns a CalcSheet[].</returns>
        public new CalcSheet[] ToArray()
        {
#if !SILVERLIGHT && !WINDOWS_UWP && !WP
            return (CalcSheet[])base.ToArray(typeof(CalcSheet));
#else
            return (CalcSheet[])base.ToArray();
#endif
        }
    }
}
