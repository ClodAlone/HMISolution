#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Security;

namespace Syncfusion.Pdf.Primitives
{
#if NETFX_CORE || WP
    public class PdfDictionary :
#else
    internal class PdfDictionary:
#endif
        IPdfPrimitive,
        IPdfChangable
    {
        #region Constants
        /// <summary>
        /// Start marker for dictionary.
        /// </summary>
        private const string Prefix = "<<";
        /// <summary>
        /// End marker for dictionary.
        /// </summary>
        private const string Suffix = ">>";
        #endregion

        #region Fields
        private static object s_syncLock = new object();
        /// <summary>
        /// Collection of items in the object.
        /// </summary>
        private Dictionary<PdfName, IPdfPrimitive> m_items;
        /// <summary>
        /// Flag for PDF file formar 1.5 is dictionary archiving needed.
        /// </summary>
        private bool m_archive = true;
        /// <summary>
        /// Flag is dictionary need to encrypt.
        /// </summary>
        private bool m_encrypt;
        /// <summary>
        /// Flag is dictionary need to decrypt.
        /// </summary>
        private bool m_isDecrypted;
        /// <summary>
        /// Indicates if the object was changed.
        /// </summary>
        private bool m_bChanged;
        /// <summary>
        /// Shows the type of object status whether it is object registered or other status;
        /// </summary>
        private ObjectStatus m_status;
        /// <summary>
        /// Indicates if the object is currently in saving state or not.
        /// </summary>
        private bool m_isSaving;
        /// <summary>
        /// Holds the index number of the object.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Internal variable to store the position.
        /// </summary>
        private int m_position = -1;
        /// <summary>
        /// Internal variable to hold PdfCrossTable reference.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal variable to hold cloned object.
        /// </summary>
        private PdfDictionary m_clonedObject = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="T:IPdfSavable"/> with the specified key.
        /// </summary>
        public IPdfPrimitive this[PdfName key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (m_items.ContainsKey(key))
                    return m_items[key];
                else
                    return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (key == null)
                    throw new ArgumentNullException("key");

                m_items[key] = value;
                Modify();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:IPdfSavable"/> with the specified key.
        /// </summary>
        /// <value></value>
        public IPdfPrimitive this[string key]
        {
            get
            {
                if (key == null || key == string.Empty)
                    throw new ArgumentNullException("key");

                IPdfPrimitive value = this[new PdfName(key)];
                return value;
            }
            set
            {
                if (key == null || key == string.Empty)
                    throw new ArgumentNullException("key");

                PdfName name = GetName(key);
                this[name] = value;
                Modify();
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ICollection Values
        {
            get
            {
                return m_items.Values;
            }
        }

        /// <summary>
        /// Get or set flag if need to archive dictionary.
        /// </summary>
        internal bool Archive
        {
            get
            {
                return m_archive;
            }
            set
            {
                m_archive = value;
            }
        }

        /// <summary>
        /// Gets or sets flag if encryption is needed.
        /// </summary>
        internal bool Encrypt
        {
            get
            {
                return m_encrypt;
            }
            set
            {
                m_encrypt = value;
                Modify();
            }
        }
        /// <summary>
        /// Gets or sets flag if decryption is needed.
        /// </summary>
        internal bool IsDecrypted
        {
            get
            {
                return m_isDecrypted;
            }
            set
            {
                m_isDecrypted = value;
            }
        }
        /// <summary>
        /// Gets the keys.
        /// </summary>
        internal ICollection Keys
        {
            get
            {
                return m_items.Keys;
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
#if NETFX_CORE || WP
        public Dictionary<PdfName, IPdfPrimitive> Items
#else
        internal Dictionary<PdfName, IPdfPrimitive> Items
#endif
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Gets or sets the Status of the specified object.
        /// </summary>
        public ObjectStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return m_isSaving;
            }
            set
            {
                m_isSaving = value;
            }
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        public int ObjectCollectionIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        /// <summary>
        /// Returns PdfCrossTable associated with the object.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public virtual IPdfPrimitive ClonedObject
        {
            get
            {
                return m_clonedObject;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event. Raise before the object saves. 
        /// </summary>
        internal event SavePdfPrimitiveEventHandler BeginSave;
        /// <summary>
        /// Event. Raise after the object saved. 
        /// </summary>
        internal event SavePdfPrimitiveEventHandler EndSave;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new empty instance of the <see cref="T:PdfDictionary"/> class.
        /// </summary>
#if NETFX_CORE || WP
        public PdfDictionary()
#else
        internal PdfDictionary()
#endif
        {
            m_items = new Dictionary<PdfName, IPdfPrimitive>(); //new Hashtable();
            m_encrypt = true;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfDictionary"/> class
        /// with values taken from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfDictionary(PdfDictionary dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            m_items = new Dictionary<PdfName, IPdfPrimitive>();

            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in dictionary.m_items)
            {
                PdfName name = item.Key;
                IPdfPrimitive obj = item.Value;
                m_items[name] = obj;
            }

            this.Status = dictionary.Status;
            FreezeChanges(this);
            m_encrypt = true;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the dictionary contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the dictionary contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return ContainsKey(new PdfName(key));
        }
        /// <summary>
        /// Determines whether the dictionary contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the dictionary contains the key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(PdfName key)
        {
            return m_items.ContainsKey(key);
        }
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(PdfName key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            m_items.Remove(key);
            Modify();
        }
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            Remove(new PdfName(key));
        }
        /// <summary>
        /// Clears items from object dictionary.
        /// </summary>
        public void Clear()
        {
            m_items.Clear();
            Modify();
        }

        /// <summary>
        /// Creates a copy of PdfDictionary.
        /// </summary>
        public virtual IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            if (!(this is PdfStream))
            {
                if (m_clonedObject != null && m_clonedObject.CrossTable == crossTable)
                    return m_clonedObject;
                else
                    m_clonedObject = null;
            }

            // Else clone the object.
            PdfDictionary newDict = new PdfDictionary();

            foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_items)
            {
                PdfName name = item.Key;
                IPdfPrimitive obj = item.Value;
                IPdfPrimitive newObj = obj.Clone(crossTable);
                if (!(newObj is PdfNull))
                    newDict[name] = newObj;
            }

            newDict.Archive = m_archive;
            newDict.IsDecrypted = m_isDecrypted;
            newDict.Status = m_status;
            newDict.Encrypt = m_encrypt;
            newDict.FreezeChanges(this);
            newDict.m_crossTable = crossTable;

            if (!(this is PdfStream))
                m_clonedObject = newDict;

            return newDict;
        }

        /// <summary>
        /// Gets a value from itself or one of the parent dictionaries.
        /// </summary>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="parentKey">The key to the parent.</param>
        /// <returns>The value by the key.</returns>
        public IPdfPrimitive GetValue(PdfCrossTable crossTable, string key, string parentKey)
        {
            IPdfPrimitive obj = null;
            PdfDictionary dic = this;

            obj = PdfCrossTable.Dereference(dic[key]);

            while (obj == null)
            {
                dic = PdfCrossTable.Dereference(dic[parentKey]) as PdfDictionary;
                obj = PdfCrossTable.Dereference(dic[key]);
            }

            return obj;
        }

        /// <summary>
        /// Gets a value from itself or one of the parent dictionaries.
        /// </summary>
        /// <param name="key">The key of the value.</param>
        /// <param name="parentKey">The key to the parent.</param>
        /// <returns>The value by the key.</returns>
        public IPdfPrimitive GetValue(string key, string parentKey)
        {
            IPdfPrimitive obj = null;
            PdfDictionary dic = this;

            obj = PdfCrossTable.Dereference(dic[key]);

            while (obj == null)
            {
                dic = PdfCrossTable.Dereference(dic[parentKey]) as PdfDictionary;

                if (dic == null) break;

                obj = PdfCrossTable.Dereference(dic[key]);
            }

            return obj;
        }

        /// <summary>
        /// Returns the string specified by the propertyName parameter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The string by its name.</returns>
        internal PdfString GetString(string propertyName)
        {
            PdfString str = PdfCrossTable.Dereference(this[propertyName]) as PdfString;

            return str;
        }

        /// <summary>
        /// Returns the integer value of the dictionary entry specified by the propertyName variable.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The integer value of the property.</returns>
        internal int GetInt(string propertyName)
        {
            PdfNumber number = PdfCrossTable.Dereference(this[propertyName]) as PdfNumber;
            int result = 0;

            if (number != null)
            {
                result = number.IntValue;
            }

            return result;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Save dictionary items.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        internal virtual void SaveItems(IPdfWriter writer)
        {
            lock (s_syncLock)
            {
                writer.Write(Operators.NewLine);

                foreach (KeyValuePair<PdfName, IPdfPrimitive> item in m_items)
                {
                    PdfName name = item.Key;
                    name.Save(writer);
                    writer.Write(Operators.WhiteSpace);

                    IPdfPrimitive obj = item.Value;
                    obj.Save(writer);
                    writer.Write(Operators.NewLine);
                }
            }
        }

        /// <summary>
        /// Creates a PDF name object.
        /// </summary>
        /// <param name="name">The string which the object is initialized with.</param>
        /// <returns>The PDF object.</returns>
        protected internal PdfName GetName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            PdfName result = new PdfName(name);

            return result;
        }
        /// <summary>
        /// Raises event <see cref="BeginSave"/>.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected virtual void OnBeginSave(SavePdfPrimitiveEventArgs args)
        {
            lock (s_syncLock)
            {
                if (BeginSave != null)
                {
                    BeginSave(this, args);
                }
            }
        }
        /// <summary>
        /// Raises event <see cref="EndSave"/>.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected virtual void OnEndSave(SavePdfPrimitiveEventArgs args)
        {
            lock (s_syncLock)
            {
                if (EndSave != null)
                {
                    EndSave(this, args);
                }
            }
        }

        #endregion

        #region IPdfSavable Members
        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Save(IPdfWriter writer)
        {
            object syncLock = new object();

            lock (syncLock)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");

                Save(writer, true);
            }
        }

        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="bRaiseEvent">If true - raises the event, False - doesn't raise.</param>
        internal void Save(IPdfWriter writer, bool bRaiseEvent)
        {
            writer.Write(Prefix);

            // Raise event.
            if (bRaiseEvent)
            {
                SavePdfPrimitiveEventArgs args = new SavePdfPrimitiveEventArgs(writer);
                OnBeginSave(args);
            }

            if (Count > 0)
            {
#if SILVERLIGHT || NETFX_CORE || WP
                SaveItems(writer);
#else
                PdfSecurity sec = writer.Document.Security;
                bool state = sec.Enabled;

                if (!m_encrypt)
                {
                    sec.Enabled = false;
                }

                SaveItems(writer);

                if (!m_encrypt)
                {
                    sec.Enabled = state;
                }
#endif
            }
            writer.Write(Suffix);
            writer.Write(Operators.NewLine);

            // Raise event.
            if (bRaiseEvent)
            {
                SavePdfPrimitiveEventArgs args = new SavePdfPrimitiveEventArgs(writer);
                OnEndSave(args);
            }

            //FreezeChanges( this );
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the internal property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="primitive">The primitive.</param>
        internal void SetProperty(string key, IPdfPrimitive primitive)
        {
            if (primitive == null)
            {
                m_items.Remove(new PdfName(key));
            }
            else
            {
                this[key] = primitive;
            }
        }

        /// <summary>
        /// Sets the internal property.
        /// </summary>
        /// <param name="key">The PdfName.</param>
        /// <param name="primitive">The primitive.</param>
        internal void SetProperty(PdfName key, IPdfPrimitive primitive)
        {
            if (primitive == null)
            {
                m_items.Remove(key);
            }
            else
            {
                this[key] = primitive;
            }
        }

        /// <summary>
        /// Sets the internal property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="wrapper">The wrapper.</param>
        internal void SetProperty(string key, IPdfWrapper wrapper)
        {
            if (wrapper == null)
            {
                m_items.Remove(new PdfName(key));
            }
            else
            {
                SetProperty(key, wrapper.Element);
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="wrapper">The wrapper.</param>
        internal static void SetProperty(PdfDictionary dictionary, string key, IPdfWrapper wrapper)
        {
            if (wrapper == null)
            {
                dictionary.Remove(new PdfName(key));
            }
            else
            {
                PdfDictionary.SetProperty(dictionary, key, wrapper.Element);
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="primitive">The primitive.</param>
        internal static void SetProperty(PdfDictionary dictionary, string key, IPdfPrimitive primitive)
        {
            if (primitive == null)
            {
                dictionary.Remove(new PdfName(key));
            }
            else
            {
                dictionary[key] = primitive;
            }
        }

        /// <summary>
        /// Sets the boolean.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Boolean value.</param>
        internal void SetBoolean(string key, bool value)
        {
            PdfBoolean pdfBoolean = this[key] as PdfBoolean;

            if (pdfBoolean != null)
            {
                pdfBoolean.Value = value;
                Modify();
            }
            else
            {
                this[key] = new PdfBoolean(value);
            }
        }

        /// <summary>
        /// Sets the integer number.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal void SetNumber(string key, int value)
        {
            PdfNumber pdfNumber = this[key] as PdfNumber;

            if (pdfNumber != null)
            {
                pdfNumber.IntValue = value;
                Modify();
            }
            else
            {
                this[key] = new PdfNumber(value);
            }
        }

        /// <summary>
        /// Sets the float number.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal void SetNumber(string key, float value)
        {
            PdfNumber pdfNumber = this[key] as PdfNumber;

            if (pdfNumber != null)
            {
                pdfNumber.FloatValue = value;
                Modify();
            }
            else
            {
                this[key] = new PdfNumber(value);
            }
        }

        /// <summary>
        /// Sets the array.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="list">The list of primitives to be placed into array.</param>
        internal void SetArray(string key, params IPdfPrimitive[] list)
        {
            PdfArray pdfArray = this[key] as PdfArray;

            if (pdfArray != null)
            {
                pdfArray.Clear();
                Modify();
            }
            else
            {
                pdfArray = new PdfArray();
                this[key] = pdfArray;
            }

            foreach (IPdfPrimitive primitive in list)
            {
                pdfArray.Add(primitive);
            }
        }

        /// <summary>
        /// Sets the date time.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dateTime">The date time.</param>
        internal void SetDateTime(string key, DateTime dateTime)
        {
            PdfString pdfString = this[key] as PdfString;

            if (pdfString != null)
            {
                pdfString.Value = PdfString.FromDate(dateTime);
                Modify();
            }
            else
            {
                this[key] = new PdfString(PdfString.FromDate(dateTime));
            }
        }


        /// <summary>
        /// Gets the date time from Pdf standard date format.
        /// </summary>
        /// <param name="dateTimeString">The string, which contains Pdf standard date format.</param>
        /// <returns>The time in <see cref="System.DateTime" /></returns>
        internal DateTime GetDateTime(PdfString dateTimeString)
        {
            if (dateTimeString == null)
                throw new ArgumentNullException("dateTimeString");

            string dateTimeFormat = "yyyyMMddHHmmss";
            string prefixD = "D:";

            dateTimeString.Value = dateTimeString.Value.Trim(new char[] { '(', ')', 'D', ':' });
            if (dateTimeString.Value.StartsWith("191"))
                dateTimeString.Value = dateTimeString.Value.Remove(0, 3).Insert(0, "20");

            bool containPrefixD = dateTimeString.Value.Contains(prefixD);

            string localTime = string.Empty.PadRight(dateTimeFormat.Length);
            if (dateTimeString.Value.Length == 0)
                return DateTime.Now;

            localTime = (containPrefixD) ? dateTimeString.Value.Substring(prefixD.Length, localTime.Length)
                : dateTimeString.Value.Substring(0, localTime.Length);

            DateTime dateTime = DateTime.Now;
            DateTime.TryParseExact(localTime, dateTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite, out dateTime);

            return dateTime;
        }

        /// <summary>
        /// Sets the string primitive.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="str">The string.</param>
        internal void SetString(string key, string str)
        {
            PdfString pdfString = this[key] as PdfString;

            if (pdfString != null)
            {
                pdfString.Value = str;
                Modify();
            }
            else
            {
                this[key] = new PdfString(str);
            }
        }

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="name">The name.</param>
        internal static void SetName(PdfDictionary dictionary, string key, string name)
        {
            PdfName pdfName = dictionary[key] as PdfName;

            if (pdfName != null)
            {
                pdfName.Value = name;
                dictionary.Modify();
            }
            else
            {
                dictionary[key] = new PdfName(name);
            }
        }

        /// <summary>
        /// Sets the name primitive.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="name">The name.</param>
        internal void SetName(string key, string name)
        {
            PdfName pdfName = this[key] as PdfName;

            if (pdfName != null)
            {
                pdfName.Value = name;
                Modify();
            }
            else
            {
                this[key] = new PdfName(name);
            }
        }

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="name">The name.</param>
        /// <param name="processSpecialCharacters">Determines whether to process special characters.</param>
        internal void SetName(string key, string name, bool processSpecialCharacters)
        {
            PdfName pdfName = this[key] as PdfName;
            string str = name.Replace("#", "#23").Replace(" ", "#20").Replace("/", "#2F");

            if (pdfName != null)
            {
                pdfName.Value = str;
                Modify();
            }
            else
            {
                this[key] = new PdfName(str);
            }
        }
        #endregion

        #region IPdfChangable Members
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:PdfDictionary"/> is changed.
        /// </summary>
        /// <value><c>true</c> if changed; otherwise, <c>false</c>.</value>
        public bool Changed
        {
            get
            {
                if (!m_bChanged)
                {
                    m_bChanged = CheckChanges();
                }

                return m_bChanged;
            }
        }

        /// <summary>
        /// Checks the changes.
        /// </summary>
        /// <returns>Returns <b>true</b> if the dictionary was changed.</returns>
        private bool CheckChanges()
        {
            bool result = false;

            foreach (IPdfPrimitive obj in Values)
            {
                IPdfChangable chbl = obj as IPdfChangable;

                if (chbl != null && chbl.Changed)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Freezes the changes.
        /// </summary>
        /// <param name="freezer">The freezer.</param>
        public void FreezeChanges(object freezer)
        {
            if (freezer is PdfParser || freezer is PdfDictionary)
            {
                m_bChanged = false;
            }
        }

        /// <summary>
        /// Mark this instance modified.
        /// </summary>
        internal void Modify()
        {
            m_bChanged = true;
        }
        #endregion
    }

    #region Internal declaration
    /// <summary>
    /// Internal structure describing a pair of Key/Value.
    /// </summary>
    internal struct PdfPair
    {
        public PdfName Key;
        public IPdfPrimitive Value;

        internal PdfPair(PdfName key, IPdfPrimitive value)
        {
            Key = key;
            Value = value;
        }
    }
    /// <summary>
    /// Event arguments class.
    /// </summary>
#if NETFX_CORE || WP
    public class SavePdfPrimitiveEventArgs : EventArgs
#else
    internal class SavePdfPrimitiveEventArgs : EventArgs
#endif
    {
        #region Fields
        /// <summary>
        /// Pdf document writer.
        /// </summary>
        private IPdfWriter m_writer;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a document that is currently generating.
        /// </summary>
        public IPdfWriter Writer
        {
            get
            {
                return m_writer;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the new object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public SavePdfPrimitiveEventArgs(IPdfWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            m_writer = writer;
        }
        #endregion

    }
    /// <summary>
    /// Delegate type.
    /// </summary>
    internal delegate void SavePdfPrimitiveEventHandler(object sender, SavePdfPrimitiveEventArgs ars);
    #endregion
}
