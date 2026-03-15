//-------------------------------------------------------------------------------------------------
// <copyright file="DescriptorBase.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Design;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// The type converter for <see cref="DescriptorBase"/> objects. <see cref="DescriptorBaseConverter"/> 
    /// is a <see cref="ExpandableObjectConverter"/>. It overrides the default behavior of the 
    /// <see cref="ConvertTo"/> method and returns the <see cref="DescriptorBase.GetName"/> result
    /// of the <see cref="DescriptorBase"/> object. <see cref="ConvertTo"/> 
    /// is called from a property grid to determine the name of the object to be displayed.
    /// </summary>
    public class DescriptorBaseConverter : ExpandableObjectConverter
    {
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The Value.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string name = ((DescriptorBase)value).GetName();
                return name != null ? name : string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }

    /// <summary>
    /// Base class for schema definition objects of the grouping engine.
    /// </summary>
    [TypeConverter(typeof(DescriptorBaseConverter))]
    public abstract class DescriptorBase : ShouldSerializeBasedPersisterType, IDisposable
    {
        bool inDispose;
        bool inDisposed;
        bool isDisposed;

        ////        ~DescriptorBase()
        ////        {
        ////            inDispose = true;
        ////            Dispose(false);
        ////            inDispose = false;
        ////        }

        /// <summary>
        /// Returns True if object is executing the <see cref="Dispose"/> method call.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Disposing
        {
            get
            {
                return inDispose;
            }
        }

        /// <summary>
        /// Returns after object was disposed and object is executing the <see cref="Disposed"/> event.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InDisposed
        {
            get
            {
                return inDisposed;
            }
        }

        /// <summary>
        /// Gets if object has been disposed.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed || inDispose)
            {
                return;
            }

            inDispose = true;
            Dispose(true);
            inDispose = false;
            isDisposed = true;
            inDisposed = true;
            OnDisposed(EventArgs.Empty);
            inDisposed = false;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Occurs after the object was disposed.
        /// </summary>
        [Description("Occurs after the object is disposed")]
        public event EventHandler Disposed;

        /// <summary>
        /// Raises the <see cref="Disposed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDisposed(EventArgs e)
        {
            if (Disposed != null)
            {
                Disposed(this, e);
            }
        }

        /// <summary>
        /// Called to clean up state of this object when it is disposed.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose"/>; False if called from Finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            ////            if (disposing)
            ////            {
            ////            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The framework calls this method to determine the name of this object. 
        /// </summary>
        /// <returns>Object name.</returns>
        public abstract string GetName();

        /// <summary>
        /// The framework calls this method to determine whether calling Reset will have any effect.
        /// </summary>
        /// <returns>True if Reset can be called; False otherwise.</returns>
        public virtual bool CanResetValue()
        {
            return ShouldSerialize();
        }

        /// <summary>
        /// The framework calls this method to reset the object back to its default state.
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// The framework calls this method to determine whether properties or child objects of this object
        /// should be serialized. (Code serialization and / or XML Serialization).
        /// </summary>
        /// <returns>True if the object should be serialized; False otherwise.</returns>
        public virtual bool ShouldSerialize()
        {
            return true;
        }

        /// <summary>Returns string representation of the DescriptorBase object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            string name = GetName();
            if (name == null)
            {
                return GetType().Name;
            }

            return GetType().Name + " { " + name + "}";
        }
    }
}
