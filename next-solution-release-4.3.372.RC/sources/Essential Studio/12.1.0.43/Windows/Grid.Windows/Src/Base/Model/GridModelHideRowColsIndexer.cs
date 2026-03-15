//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelHideRowColsIndexer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Security;
using System.Security.Permissions;
using System.Globalization;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines an interface used by <see cref="GridModelHideRowColsIndexer"/> to store hidden rows and columns.
    /// </summary>
    public interface IGridRowColHideDictionary
    {
        /// <summary>
        /// Occurs when rows or columns are moved.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <param name="dest">The Destination.</param>
        void MoveItems(int from, int count, int dest);

        /// <summary>
        /// Occurs when rows or columns are removed.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        void RemoveItems(int from, int count);

        /// <summary>
        /// Occurs when rows or columns are inserted.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        void InsertItems(int index, int count);

        /// <summary>
        /// The hidden state at a given row or column index.
        /// </summary>
        bool this[int index] 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        bool Modified { get; }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        void ResetModified();
    }

    /// <internalonly/>
    /// <summary>Internal only.</summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridRowColHideDictionaryTypeConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRowColHideDictionaryTypeConverter()
            : base()
        {
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        sealed class Binder : SerializationBinder
        {
            public override Type BindToType(
                string assemblyName, string typeName)
            {
                Type t = Type.GetType(typeName);

                if (t != null)
                {
                    return t;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(byte[]))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            byte[] bytes = value as byte[];

            if (bytes == null)
            {
                return base.ConvertFrom(context, culture, value);
            }

            object o = null;
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf = new BinaryFormatter();
            bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            bf.Binder = new Binder();
            o = bf.Deserialize(ms);
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            ms.Close();

            return o as GridRowColHideDictionary;
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(byte[]))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            GridRowColHideDictionary serType = value as GridRowColHideDictionary;

            if (serType != null && destinationType == typeof(byte[]))
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;

                bf.Serialize(ms, value);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
    
    /// <summary>
    /// This class implements the default dictionary that is used by <see cref="GridModelHideRowColsIndexer"/> 
    /// to store hidden rows and columns.
    /// </summary>
    /// <remarks>
    /// This is the default implementation for <see cref="IGridRowColHideDictionary"/>.
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(GridRowColHideDictionaryTypeConverter))]
    public class GridRowColHideDictionary : IGridRowColHideDictionary, ISerializable
    {
        internal GridIndexDictionary innerDict = new GridIndexDictionary(typeof(bool));

        /// <summary>
        /// Initalize a new instance of GridRowColHideDictionary.
        /// </summary>
        public GridRowColHideDictionary()
        {
        }

        /// <internalonly/>
        /// <summary>Gets InnerDictionary. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridIndexDictionary InnerDict
        {
            get
            {
                return innerDict;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        public bool Modified
        {
            get
            {
                return innerDict.Count > 0;
            }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        void IGridRowColHideDictionary.ResetModified()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridRowColHideDictionary"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRowColHideDictionary(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            innerDict = (GridIndexDictionary)info.GetValue("Dictionary", typeof(GridIndexDictionary));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridRowColHideDictionary"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Dictionary", innerDict); // GridIndexDictionary
        }

        /// <summary>
        /// Occurs when rows or columns are moved.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <param name="dest">The Destination.</param>
        /// <remarks>
        /// <seealso cref="GridIndexDictionary.MoveIndex"/>
        /// </remarks>
        public void MoveItems(int from, int count, int dest)
        {
            innerDict.MoveIndex(from, count, dest);
        }

        /// <summary>
        /// Occurs when rows or columns are removed.
        /// </summary>
        /// <param name="from">First row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <remarks>
        /// <seealso cref="GridIndexDictionary.RemoveIndex"/>
        /// </remarks>
        public void RemoveItems(int from, int count)
        {
            innerDict.RemoveIndex(from, count);
        }

        /// <summary>
        /// Occurs when rows or columns are inserted.
        /// </summary>
        /// <param name="index">Row or column index.</param>
        /// <param name="count">Number of rows or columns.</param>
        /// <remarks>
        /// <seealso cref="GridIndexDictionary.InsertIndex"/>
        /// </remarks>
        public void InsertItems(int index, int count)
        {
            innerDict.InsertIndex(index, count);
        }

        /// <summary>
        /// The hidden state at a given row or column index.
        /// </summary>
        /// <remarks>
        /// <seealso cref="GridIndexDictionary.this[int]"/>
        /// </remarks>
        public bool this[int index]
        {
            get
            {
                object f;
                bool b = innerDict.Lookup(index, out f);
                return b;
            }

            set
            {
                if (value)
                {
                    innerDict[index] = value;
                }
                else
                {
                    innerDict.Remove(index);
                }
            }
        }
    }

    /// <summary>
    /// This is an abstract base class that manages row heights
    /// and column widths in a grid and lets you change them.
    /// Events will be raised in the grid when settings are changed.
    /// </summary>
    /// <remarks>
    /// You typically access this class from a grid using the <see cref="GridModel.HideRows"/>
    /// and <see cref="GridModel.HideCols"/> properties of a <see cref="GridModel"/>.
    /// <para/>
    /// This class raises the following events in a <see cref="GridModel"/>:
    /// <list type="bullet">
    /// <listheader><term>Items</term><description>Descriptions</description></listheader>
    /// <item><term><see cref="GridModel.RowsHidden"/></term></item>
    /// <item><term><see cref="GridModel.RowsHiding"/></term></item>
    /// <item><term><see cref="GridModel.ColsHidden"/></term></item>
    /// <item><term><see cref="GridModel.ColsHiding"/></term></item>
    /// <item><term><see cref="GridModel.QueryHideCol"/></term></item>
    /// <item><term><see cref="GridModel.QueryHideRow"/></term></item>
    /// <item><term><see cref="GridModel.SaveHideCol"/></term></item>
    /// <item><term><see cref="GridModel.SaveHideRow"/></term></item>
    /// </list>
    /// </remarks>
    [Serializable]
    public abstract class GridModelHideRowColsIndexer : GridModelBound, ISerializable
    {
        // Fields.
        private IGridRowColHideDictionary indexer;

        // Events.

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public abstract void OnChanged(GridRowColHiddenEventArgs e);

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Grid.GridRowColHidingEventArgs"/> instance containing the event data.</param>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public abstract bool OnChanging(GridRowColHidingEventArgs e);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnSaveHide(GridRowColHideEventArgs e);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnQueryHide(GridRowColHideEventArgs e);

        /// <summary>
        /// Initializes a new <see cref="GridModelHideRowColsIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModelHideRowColsIndexer(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            indexer = (IGridRowColHideDictionary)info.GetValue("HideDictionary", typeof(IGridRowColHideDictionary));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelHideRowColsIndexer"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("HideDictionary", indexer);
        }

        /// <summary>
        /// Initializes a <see cref="GridModelHideRowColsIndexer"/> and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        protected GridModelHideRowColsIndexer(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets a reference to <see cref="GridModel.Rows"/> or <see cref="GridModel.Cols"/>.
        /// </summary>
        public abstract GridModelRowColOperations RowColObject { get; }
        
        /// <summary>
        /// Gets "HideRow" or "HideColumn" string.
        /// </summary>
        public abstract string RowColName { get; }

        /// <summary>
        /// Gets storage for all hidden row and column settings in the grid. 
        /// </summary>
        /// <remarks>
        /// You can replace this dictionary at run-time with a custom dictionary 
        /// if you implement the <see cref="IGridRowColSizeDictionary"/> interface.
        /// </remarks>
        public IGridRowColHideDictionary Dictionary
        {
            get
            {
                if (indexer == null)
                {
                    indexer = new GridRowColHideDictionary();
                }

                return indexer;
            }
        }

        /// <overload>
        /// Returns or sets the hidden state of a row or column. 
        /// </overload>
        /// <summary>
        /// Returns or sets the hidden row or column state for the specified index. 
        /// </summary>
        /// <remarks>
        /// Call ResetRange reset values to default. 
        /// </remarks>
        public bool this[int index]
        {
            get
            {
                return GetHidden(index);
            }

            set
            {
                SetRange(index, index, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dictionary was modified.
        /// </summary>
        public bool Modified
        {
            get { return this.Dictionary.Modified; }
        }

        /// <summary>
        /// Resets the <see cref="Modified"/> flag.
        /// </summary>
        void ResetModified()
        {
            this.Dictionary.ResetModified();
        }
        
        /// <summary>
        /// Returns or sets the hidden row or column state for the row or column that matches the specified name. 
        /// </summary>
        /// <remarks>
        /// Call ResetRange reset values to default. 
        /// Call IsDefault to check if a value is reset to default.
        /// Call IsHidden[n] to check if a row or column is hidden.
        /// </remarks>
        public abstract bool this[string name] 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets an array of <see cref="System.Boolean"/>. The array will have true values for 
        /// rows and columns that are hidden. Rows and columns that are displayed will have a false
        /// value.
        /// </summary>
        /// <param name="from">First row or column.</param>
        /// <param name="last">Last row or column.</param>
        /// <returns>An array with row and column hidden settings.</returns>
        public bool[] GetRange(int from, int last)
        {
            int count = last - from + 1;
            bool[] values = new bool[count];
            for (int n = 0; n < count; n++)
            {
                values[n] = GetHidden(from + n);
            }

            return values;
        }

        /// <summary>
        /// Resets the range of rows or columns to be displayed.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        public void ResetRange(int from, int last)
        {
            SetRange(from, last, false);
        }

        /// <overload>
        /// Changes the hidden state for a range of rows or columns.
        /// </overload>
        /// <summary>
        /// Changes the hidden state for a range of rows or columns.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="value">The hidden state to be applied.</param>
        /// <remarks>
        /// The method will generate undo information and push it onto the
        /// grid's command stack. <para/>
        /// A <see cref="GridModel.RowsHiding"/> or <see cref="GridModel.ColsHiding"/>) event is 
        /// raised before the values are modified and gives event listeners a chance to discard 
        /// the operation before any change happens.<para/>
        /// If the <see cref="GridModel.RowsHiding"/> event did not signal to cancel any changes, the operation
        /// will go ahead, apply changes, and raise a <see cref="GridModel.RowsHidden"/> event. The <see cref="GridModel.RowsHidden"/> 
        /// will indicate if changes were successful or not.
        /// </remarks>
        public void SetRange(int from, int last, bool value)
        {
            SetRange(from, last, new bool[] { value });
        }

        /// <summary>
        /// Changes the hidden state for a range of rows or columns.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with hidden states to be applied.</param>
        /// <genoverload/>
        public void SetRange(int from, int last, bool[] values)
        {
            SetRange(from, last, values, false);
        }

        /// <summary>
        /// Changes the hidden state for a range of rows or columns.
        /// </summary>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with hidden states to be applied.</param>
        /// <param name="discardUndo">True if no undo information should be generated.</param>
        /// <genoverload/>
        public void SetRange(int from, int last, bool[] values, bool discardUndo)
        {
            Model.NotifyChangingLayoutCells(RowColObject.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".SetRange(int, int, int)");
            try
            {
                bool workAround = GridControlBase.UseOldHiddenScrollLogic;  // workaround for SetRange issue. GridScroll does not correctly handle SetRange at this time,
                //// but looping through with index is ok (although much slower ...)

                if (workAround || OnChanging(new GridRowColHidingEventArgs(from, last, values)))
                {
                    bool success = false;
                    bool[] savedValues = null;
                    try
                    {
                        savedValues = GetRange(from, last);
                        int count = savedValues.Length;

                        for (int n = 0; n < count; n++)
                        {
                            bool hide = values[values.Length > 1 ? n : 0];
                            if (GetHidden(from + n) != hide)
                            {
                                int index = from + n;
                                if (workAround && OnChanging(new GridRowColHidingEventArgs(index, index, new bool[] { hide })))
                                {
                                    SetHidden(index, hide);
                                    success = true;
                                    // workAround
                                    OnChanged(new GridRowColHiddenEventArgs(index, index, new bool[] { !hide }, success));
                                }
                                else
                                {
                                    SetHidden(index, hide);
                                    success = true;
                                }
                            }
                        }

                        if (success && !discardUndo && model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelSetRowColHideCommand(this, from, last, savedValues));
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (!workAround)
                        {
                            OnChanged(new GridRowColHiddenEventArgs(from, last, savedValues, success));
                        }
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(RowColObject.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void SetHidden(int index, bool value)
        {
            Dictionary[index] = value;
        }

        /// <summary>
        /// Gets the hidden.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool GetHidden(int index)
        {
            return Dictionary[index];
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridRowColHideDictionary is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    class GridModelSetRowColHideCommand : GridModelCommand
    {
        private int from;
        private int to;
        private bool[] values;
        private GridModelHideRowColsIndexer rchi;

        /// <summary>
        /// Initializes <see ref="GridModelSetRowColHideCommand"/> object.
        /// </summary>
        /// <param name="rchi">A reference to the <see cref="GridModelHideRowColsIndexer"/> object.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with hidden states to be applied.</param>
        public GridModelSetRowColHideCommand(GridModelHideRowColsIndexer rchi, int from, int last, bool[] values)
            : base(rchi.model)
        {
            SetDescription(SR.GetString("Command" + rchi.RowColName, from, to));
            this.rchi = rchi;
            this.from = from;
            this.to = last;
            this.values = values;
        }

        public override void Execute()
        {
            rchi.SetRange(from, to, values);
            Grid.ScrollCellInView(rchi.RowColObject.CreateRangeFromTo(from, to), GridScrollCurrentCellReason.Command);
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelHideRowsIndexer : GridModelHideRowColsIndexer
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelHideRowsIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelHideRowsIndexer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        public GridModelHideRowsIndexer(GridModel model)
            : base(model)
        {
        }

        public override string RowColName
        {
            get { return "HideRow"; }
        }

        public override GridModelRowColOperations RowColObject
        {
            get { return model.Rows; }
        }

        /// <summary>
        /// Used internally.
        /// </summary>        
        /// <override/>
        public override void OnChanged(GridRowColHiddenEventArgs e)
        {            
            Model.RaiseRowsHidden(e);
            Model.Modified = true;
            Model.rowHiddenEntries = null;
        }

        /// <summary>
        /// Used internally.
        /// </summary>        
        /// <returns>returns boolean value</returns>
        /// <override/>
        public override bool OnChanging(GridRowColHidingEventArgs e)
        {
            Model.RaiseRowsHiding(e);
            return !e.Cancel;
        }

        /// <override/>
        protected override void OnSaveHide(GridRowColHideEventArgs e)
        {
            Model.RaiseSaveHideRow(e);
        }

        /// <override/>
        protected override void OnQueryHide(GridRowColHideEventArgs e)
        {
            Model.RaiseQueryHideRow(e);
        }

        /// <override/>
        public override bool this[string name]
        {
            get { return this[Model.NameToRowIndex(name)]; }
            set { this[Model.NameToRowIndex(name)] = value; }
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelHideColsIndexer : GridModelHideRowColsIndexer
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelHideColsIndexer"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelHideColsIndexer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {  
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount); 
            }
#else
               
            ;
#endif
        }

        public GridModelHideColsIndexer(GridModel model)
            : base(model)
        {
        }

        public override string RowColName
        {
            get { return "HideColumn"; }
        }

        public override GridModelRowColOperations RowColObject
        {
            get { return model.Cols; }
        }

        public override void OnChanged(GridRowColHiddenEventArgs e)
        {
            Model.RaiseColsHidden(e);
            Model.Modified = true;
            Model.colHiddenEntries = null;
        }

        public override bool OnChanging(GridRowColHidingEventArgs e)
        {
            Model.RaiseColsHiding(e);
            return !e.Cancel;
        }

        protected override void OnSaveHide(GridRowColHideEventArgs e)
        {
            Model.RaiseSaveHideCol(e);
        }

        protected override void OnQueryHide(GridRowColHideEventArgs e)
        {
            Model.RaiseQueryHideCol(e);
        }

        public override bool this[string name]
        {
            get { return this[Model.NameToColIndex(name)]; }
            set { this[Model.NameToColIndex(name)] = value; }
        }
    }
}
