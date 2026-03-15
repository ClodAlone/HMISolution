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

#region File using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Class that represents MS Word document's variables.
    /// </summary>
    public class DocVariables
    {
        #region Fields
        /// <summary>
        /// Document variables
        /// </summary>
        private Dictionary<string, string> m_variables;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the variable with the specified name.
        /// </summary>
        /// <value></value>
        public string this[string name]
        {
            get
            {
                if (m_variables.ContainsKey(name))
                {
                    return m_variables[name];
                }
                return null;
            }
            set
            {
                m_variables[name] = value;
            }
        }
        /// <summary>
        /// Gets the count of variables.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_variables.Count;
            }
        }
        /// <summary>
        /// Gets the collection of name/value items.
        /// </summary>
        /// <value>The items.</value>
        internal Dictionary<string, string> Items
        {
            get
            {
                return m_variables;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Variables"/> class.
        /// </summary>
        public DocVariables()
        {
            m_variables = new Dictionary<string, string>();
        }
        #endregion

        #region Implementation / public methods
        /// <summary>
        /// Adds new document variable to document.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, string value)
        {
            if (value == null)
                value = string.Empty;
            m_variables.Add(name, value);
        }
        /// <summary>
        /// Gets variable's key by the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetNameByIndex(int index)
        {
            CheckIndex(index);
            return FindItem(index, true);
        }
        /// <summary>
        /// Gets variable's value by the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetValueByIndex(int index)
        {
            CheckIndex(index);
            return FindItem(index, false);
        }
        /// <summary>
        /// Removes document variable with specified name from the document.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            m_variables.Remove(name);
        }
        #endregion

        #region Implementation / helper methods
        /// <summary>
        /// Updates the variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        internal void UpdateVariables(byte[] variables)
        {
            MemoryStream stream = new MemoryStream(variables);
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

            reader.ReadInt16();
            int count = reader.ReadInt16();
            reader.ReadInt16();

            // Read varible names.
            char[] name;
            int length = 0;
            string[] names = new string[count];

            for (int i = 0; i < count; i++)
            {
                length = reader.ReadInt16();
                name = reader.ReadChars(length);
                names[i] = new string(name);
                // Skip unknown data
                reader.ReadInt32();
            }

            // Read values
            char[] value;
            for (int j = 0; j < count; j++)
            {
                length = reader.ReadUInt16();
                value = reader.ReadChars(length);
                m_variables.Add(names[j], new string(value));
            }
        }
        /// <summary>
        /// Converts collection of variables to byte array.
        /// </summary>
        /// <returns></returns>
        internal byte[] ToByteArray()
        {
            if (m_variables.Count == 0)
                return null;

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);
            string[] names = new string[m_variables.Count];
            string[] values = new string[m_variables.Count];
            int index = 0;
            SortedDictionary<string, string> docVars = new SortedDictionary<string, string>();
            foreach (string key in m_variables.Keys)
                docVars.Add(key, m_variables[key]);

            foreach (string key in docVars.Keys)
            {
                names[index] = key;
                values[index] = docVars[key];
                index += 1;
            }

            // Data start with 0xff and 0xff
            writer.Write(byte.MaxValue);
            writer.Write(byte.MaxValue);

            // Write the number of name/value pairs
            short count = (short)m_variables.Count;
            writer.Write(count);
            // Write undefined value 4;
            writer.Write((short)4);

            // Write variable names
            for (int i = 0; i < count; i++)
            {
                // Write length of the name
                writer.Write((short)names[i].Length);
                // Write variable's name
                writer.Write(names[i].ToCharArray());
                writer.Write(int.MaxValue);
            }

            // Write values
            for (int j = 0; j < count; j++)
            {
                // Write length of the name
                writer.Write((short)values[j].Length);
                // Write variable's name
                writer.Write(values[j].ToCharArray());
            }

            return stream.ToArray();
        }
        /// <summary>
        /// Finds the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="returnName">if returns name, set to <c>true</c>.</param>
        /// <returns></returns>
        private string FindItem(int index, bool returnName)
        {
            IDictionaryEnumerator enumerator = m_variables.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                enumerator.MoveNext();
            }

            return (string)((returnName) ? enumerator.Entry.Key : enumerator.Entry.Value);
        }
        /// <summary>
        /// Checks the index.
        /// </summary>
        /// <param name="index">The index.</param>
        private void CheckIndex(int index)
        {
            if (index < 0 || index >= m_variables.Count)
            {
                throw new ArgumentOutOfRangeException("index",
                  "Index must be larger than 0 and lower than number of variables in the document");
            }
        }
        #endregion
    }
}
