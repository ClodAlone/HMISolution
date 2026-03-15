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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.DocIO.DLS.WField"/> objects that 
    /// represent the fields in the document.
    /// </summary>
    internal class FieldCollection : CollectionImpl
    {
        
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WField"/> with the specified name.
        /// </summary>
        /// <value></value>
        internal WField this[string name]
        {
            get
            {
                return FindByName(name);
            }
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WField"/> at the specified index.
        /// </summary>
        /// <value></value>
        internal WField this[int index]
        {
            get
            {
                return InnerList[index] as WField;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCollection"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        internal FieldCollection(WordDocument doc)
            : base(doc, doc)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Finds <see cref="Syncfusion.DocIO.DLS.WField"/> object by specified name
        /// </summary>
        /// <param name="name">The Field value</param>
        /// <returns></returns>
        public WField FindByName(string name)
        {
            String name1 = name.Replace('-', '_');
            for (int i = 0; i < InnerList.Count; i++)
            {
                WField field = InnerList[i] as WField;

#if SyncfusionFramework2_0

                if (field.FieldValue.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return field;
                }
#else
        if( field.FieldValue.ToUpper() == name1.ToUpper() )
        {
          return field;
        }
#endif
            }

            return null;
        }
        /// <summary>
        /// Removes a Field at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            WField field = InnerList[index] as WField;
            Remove(field);
        }
        /// <summary>
        /// Removes the specified bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        public void Remove(WField field)
        {
            InnerList.Remove(field);
        }
        /// <summary>
        /// Removes all Fields from the document. 
        /// </summary>
        public void Clear()
        {
            while (InnerList.Count > 0)
            {
                int lastIndex = InnerList.Count - 1;
                RemoveAt(lastIndex);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds field objects to the collection.
        /// </summary>
        /// <param name="bookmark"></param>
        internal void Add(WField field)
        {
            if (!InnerList.Contains(field))
                InnerList.Add(field);
        }
        #endregion
    }
}
