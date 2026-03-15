//-------------------------------------------------------------------------------------------------
// <copyright file="CurrentRecordProperty.cs" company="syncfusion">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;
using Syncfusion.Styles;

namespace Syncfusion.Grouping
{
    #region CurrentRecordPropertyCollection
    /// <summary>
    /// A collection of <see cref="CurrentRecordProperty"/> elements that provides storage for modified
    /// values for the current <see cref="Record"/> in a <see cref="CurrentRecordManager"/>.
    /// An instance of this collection is returned by the <see cref="CurrentRecordManager.Properties"/> property
    /// of a <see cref="Table.CurrentRecordManager"/> object.
    /// </summary>
    public class CurrentRecordPropertyCollection : IList
    {
        internal ArrayList _inner;
        internal CurrentRecordManager _currentRecordManager;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static CurrentRecordPropertyCollection Empty = new CurrentRecordPropertyCollection(null);

        internal CurrentRecordPropertyCollection(CurrentRecordManager currentRecordManager)
        {
            _currentRecordManager = currentRecordManager;
        }

        internal void Reset()
        {
            _inner = null;
        }

        void EnsureInitialized()
        {
            if (_inner == null)
            {
                if (_currentRecordManager == null)
                {
                    _inner = new ArrayList(0);
                }
                else
                {
                    FieldDescriptorCollection fields = _currentRecordManager.ParentTable.TableDescriptor.Fields;
                    _inner = new ArrayList(fields.Count);
                    foreach (FieldDescriptor fieldDescriptor in fields)
                    {
                        CurrentRecordProperty field = new CurrentRecordProperty(_currentRecordManager, fieldDescriptor);
                        _inner.Add(field);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        public CurrentRecordProperty this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (CurrentRecordProperty)_inner[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Gets the element for the descriptor.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        public CurrentRecordProperty this[FieldDescriptor fd]
        {
            get
            {
                return this[fd.Name];
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Gets the element for the field with the specified name.
        /// </summary>
        public CurrentRecordProperty this[string name]
        {
            get
            {
                for (int n = 0; n < Count; n++)
                {
                    CurrentRecordProperty prop = (CurrentRecordProperty)_inner[n];
                    if (prop.FieldDescriptor.Name == name)
                    {
                        return prop;
                    }
                }

                return null;
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(CurrentRecordProperty value)
        {
            if (value == null)
            {
                return false;
            }

            EnsureInitialized();
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(CurrentRecordProperty value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(CurrentRecordProperty[] array, int index)
        {
            int n = 0;
            foreach (CurrentRecordProperty item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        ////        public CurrentRecordPropertyCollection SyncRoot
       ////        {
        ////            get
        ////            {
        ////                return null;
        ////            }
        ////        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading of the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public CurrentRecordPropertyCollectionEnumerator GetEnumerator()
        {
            return new CurrentRecordPropertyCollectionEnumerator(this);
        }

        #region IList Members

        /// <summary>
        /// Returns True because this collection is always Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }
        }

        /// <summary>
        /// Not supported because collection is readonly.
        /// </summary>
        /// <param name="index">The index value</param>
        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        bool IList.Contains(object value)
        {
            return Contains((CurrentRecordProperty)value);
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((CurrentRecordProperty)value);
        }

        int IList.Add(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// If changes in the TableDescriptor are detected, the
        /// method will reinitialize the field descriptors before returning the count.
        /// </remarks>
        public int Count
        {
            get
            {
                EnsureInitialized();
                return _inner.Count;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((CurrentRecordProperty[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="CurrentRecordProperty"/> items of a <see cref="CurrentRecordPropertyCollection"/>.
    /// </summary>
    public class CurrentRecordPropertyCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        CurrentRecordPropertyCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public CurrentRecordPropertyCollectionEnumerator(CurrentRecordPropertyCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public CurrentRecordProperty Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            if (_next + 1 >= _coll.Count)
            {
                _next = -1;
            }
            else
            {
                _next++;
            }

            return _cursor != -1;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// A <see cref="CurrentRecordProperty"/> element provides storage for modified
    /// values for the current <see cref="Record"/> in a <see cref="CurrentRecordManager"/>.
    /// CurrentRecordProperty objects are accessed through the collection returned by the <see cref="CurrentRecordManager.Properties"/> property
    /// of a <see cref="Table.CurrentRecordManager"/> object.
    /// </summary>
    public class CurrentRecordProperty
    {
        CurrentRecordManager currentRecordManager;
        FieldDescriptor fd;
        object originalValue;
        bool hasOriginalValue = false;
        object modifiedValue;
        bool isModified;
        Exception exception;
        bool saveInEditableRow = false;

        /// <summary>Returns string representation of the CurrentRecordProperty object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(fd.Name);
            if (isModified)
            {
                sb.AppendFormat(" (Modified)");
            }

            sb.Append(" = ");
            if (CurrentValue == null || CurrentValue is DBNull)
            {
                sb.Append("(null)");
            }
            else
            {
                sb.Append(CurrentValue.ToString());
            }

            return sb.ToString();
        }

        internal CurrentRecordProperty(CurrentRecordManager currentRecordManager, FieldDescriptor fd)
        {
            this.currentRecordManager = currentRecordManager;
            this.fd = fd;
        }

        /// <summary>
        /// The current record.
        /// </summary>
        public Record Record
        {
            get
            {
                return currentRecordManager.CurrentRecord;
            }
        }

        /// <summary>
        /// The field descriptor.
        /// </summary>
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                return fd;
            }
        }

        /// <summary>
        /// The current (possibly modified) value.
        /// </summary>
        public object CurrentValue
        {
            get
            {
                if (IsModified)
                {
                    return ModifiedValue;
                }
                else
                {
                    return OriginalValue;
                }
            }
        }

        /// <summary>
        /// The original value.
        /// </summary>
        public object OriginalValue
        {
            get
            {
                if (!hasOriginalValue && Record.GetData() != null)
                {
                    originalValue = fd.GetValue(Record);
                    hasOriginalValue = !fd.IsExpressionField() && !fd.IsForeignKeyField() && !fd.IsUnboundField();
                }

                return originalValue;
            }
        }

        static bool EqualValues(object val1, object val2)
        {
            bool isNull1 = val1 == null || val1 is DBNull;
            bool isNull2 = val2 == null || val2 is DBNull;
            if (isNull1 && isNull2)
            {
                return true;
            }
            else if (isNull1 || isNull2)
            {
                return false;
            }
            else
            {
                return val1.Equals(val2);
            }
        }

        /// <summary>
        /// The modified value; NULL if not modified,
        /// </summary>
        public object ModifiedValue
        {
            get
            {
                return modifiedValue;
            }

            set
            {
                if (value == OriginalValue)
                {
                    if (IsModified)
                    {
                        ResetModifiedValue();
                    }
                }
                else
                {
                    if (!IsModified || !EqualValues(value, modifiedValue))
                    {
                        currentRecordManager.isSetModifiedValue = true;
                        modifiedValue = value;
                        exception = null;
                        isModified = true;
                        try
                        {
                            saveInEditableRow = fd.SaveInEditableRow(Record, value);
                            if (!saveInEditableRow)
                            {
                                // let's check at least if value can be converted to target type
                                // we delay saving the value into the record until EndEdit is called
                                if (value != null && !(value is DBNull))
                                {
                                    modifiedValue = NullableHelper.ChangeType(value, fd.GetPropertyType());
                                }

                                modifiedValue = NullableHelper.FixDbNUllasNull(value, fd.GetPropertyType());
                            }

                            // Check first if CurrentRecordManager.Reset was called before setting it modified.
                            // This could have happened if the CurrentRecordProperty.ModifiedValue property setter
                            // calls fd.SaveInEditableRow and this triggered a ItemChanged event for the current 
                            // record. In that case reset the ModifiedValue and do not mark the current record
                            // as modified.
                            if (!currentRecordManager.inNotifyBeginEdit && !currentRecordManager.IsEditing)
                            {
                                // do not mark current record as modified.
                                modifiedValue = null;
                                isModified = false;
                                exception = null;
                                hasOriginalValue = false;
                            }
                            else
                            {
                                currentRecordManager.NotifyPropertyChanged(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                            currentRecordManager.NotifyException(this);
                        }
                        finally
                        {
                            currentRecordManager.isSetModifiedValue = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the modified value and marks the property as not modified.
        /// </summary>
        public void ResetModifiedValue()
        {
            if (isModified)
            {
                try
                {
                    currentRecordManager.isResetModifiedValue = true;
                    modifiedValue = null;
                    isModified = false;
                    exception = null;
                    /*if (pd.CanResetValue(Record.Data))
                        pd.ResetValue(Record.Data);
                    else */
                    if (hasOriginalValue && saveInEditableRow)
                    {
                        fd.GetPropertyDescriptor().SetValue(Record.GetData(), originalValue);
                        saveInEditableRow = false;
                    }

                    currentRecordManager.NotifyPropertyChanged(this);
                }
                finally
                {
                    currentRecordManager.isResetModifiedValue = false;
                }
            }
        }

        /// <summary>
        /// Saves the changes into the underlying datasource. 
        /// </summary>
        public void SaveChanges()
        {
            if (isModified)
            {
                exception = null;
                if (!saveInEditableRow)
                {
                    try
                    {
                        fd.SetValue(Record, modifiedValue, false);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        currentRecordManager.NotifyException(this);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if value is modified.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return isModified;
            }
        }

        /// <summary>
        /// Determines if an exception was thrown in a previous <see cref="SaveChanges"/> call.
        /// </summary>
        public bool IsError
        {
            get
            {
                return exception != null;
            }
        }

        /// <summary>
        /// Gets / sets an exception object that was thrown when the record was validated. 
        /// </summary>
        public Exception Exception
        {
            get
            {
                return exception;
            }

            set
            {
                exception = value;
            }
        }
    }
}