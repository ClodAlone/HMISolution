//-------------------------------------------------------------------------------------------------
// <copyright file="TableFieldPropertyDescriptor.cs" company="syncfusion">
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
using System.Threading;
using System.Data;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <internalonly/>
    /// <summary>
    /// A custom PropertyDescriptor that is used within a GridGroupTypedListRecordsCollection to access
    /// a FieldDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class TableFieldPropertyDescriptor : PropertyDescriptor
    {
        private FieldDescriptor field;

        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a FieldDescriptor.
        /// </summary>
        /// <param name="field">The Field.</param>
        public TableFieldPropertyDescriptor(FieldDescriptor field)
            : base(field.Name, null)
        {
            this.field = field;
        }

        /// <summary>Determines if the given component should be serialized.</summary>
        /// <param name="component">The component.</param>
        /// <returns>returns False.</returns>
        /// <override/>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>Sets a value for the given component.</summary>
        /// <param name="component">The Component.</param>
        /// <param name="value">Value to be set.</param>
        /// <override/>
        public override void SetValue(object component, object value)
        {
            Record record = (Record)component;
            record.SetValue(field, value);
            this.OnValueChanged(record, EventArgs.Empty);
        }

        /// <summary>Resets the component.</summary>
        /// <param name="component">Component to reset.</param>
        /// <override/>
        public override void ResetValue(object component)
        {
            Record record = (Record)component;
            record.SetValue(field, DBNull.Value);
            this.OnValueChanged(record, EventArgs.Empty);
        }

        /// <summary>Returns the value of given component.</summary>
        /// <param name="component">The Component.</param>
        /// <returns>returns Value.</returns>
        /// <override/>
        public override object GetValue(object component)
        {
            Record record = (Record)component;
            return record.GetValue(field);
        }
        
        /// <override/>
        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes
        /// its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability. </param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            Record record = (Record)component;
            object value = record.GetValue(field);
            return value != null && !(value is DBNull);
        }

        /// <summary>Gets the result type of this field.</summary>
        /// <override/>
        public override Type PropertyType
        {
            get
            {
                return this.field.GetPropertyType();
            }
        }

        /// <summary>Determines if the field is read-only.</summary>
        /// <override/>
        public override bool IsReadOnly
        {
            get
            {
                return this.field.ReadOnly;
            }
        }

        /// <summary>Gets the type of the record.</summary>
        /// <override/>
        public override Type ComponentType
        {
            get
            {
                return typeof(Record);
            }
        }

        /// <summary>Indicates if the field is browsable.</summary>
        /// <override/>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return this.field.GetHashCode();
        }
        
        /// <override/>
        /// <summary>
        /// Compares this to another object to see if they are equivalent.
        /// </summary>
        /// <param name="other">The object to compare. </param>
        /// <returns>
        /// true if the values are equivalent; otherwise, false.
        /// </returns>
        public override bool Equals(object other)
        {
            if (other is TableFieldPropertyDescriptor)
            {
                TableFieldPropertyDescriptor other0 = (TableFieldPropertyDescriptor)other;
                return other0.field == this.field;
            }

            return false;
        }
        
        /// <summary>
        /// The FieldDescriptor.
        /// </summary>
        public FieldDescriptor Field
        {
            get
            {
                return this.field;
            }
        }
    }

    /// <summary>
    /// A custom PropertyDescriptor that is used within a GridGroupTypedListRecordsCollection to access
    /// a FieldDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class TableRecordDataPropertyDescriptor : PropertyDescriptor
    {
        Type type;

        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a FieldDescriptor.
        /// </summary>
        /// <param name="type">The Type value.</param>
        public TableRecordDataPropertyDescriptor(Type type)
            : base("__data__", null)
        {
            this.type = type;
        }

        /// <summary>Determines if the given component should be serialized.</summary>
        /// <param name="component">The component.</param>
        /// <returns>returns False.</returns>
        /// <override/>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>Sets a value for the given component.</summary>
        /// <param name="component">The Component.</param>
        /// <param name="value">Value to be set.</param>
        /// <override/>
        public override void SetValue(object component, object value)
        {
        }
        
        /// <override/>
        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the
        /// component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be
        /// reset to the default value. </param>
        public override void ResetValue(object component)
        {
        }
        
        /// <override/>
        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a
        /// component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve
        /// the value. </param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            Record record = (Record)component;
            return record.GetData();
        }

        /// <override/>
        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes
        /// its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability. </param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }
        
        /// <override/>
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return type;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets a value indicating whether this
        /// property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the type of the component this property
        /// is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return typeof(Record);
            }
        }

        /// <override/>
        /// <summary>
        /// Gets a value indicating whether the member is browsable.
        /// </summary>
        public override bool IsBrowsable
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// A custom PropertyDescriptor that is used within a GridGroupTypedListRecordsCollection to access
    /// a FieldDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class TableRecordIndexPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a FieldDescriptor.
        /// </summary>
        public TableRecordIndexPropertyDescriptor()
            : base("__index__", null)
        {
        }

        /// <override/>
        /// <summary>
        /// Determines a value indicating whether the
        /// value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for
        /// persistence. </param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        
        /// <override/>
        /// <summary>
        /// Sets the value of the component to a
        /// different value.
        /// </summary>       
        /// <param name="component">The component with the property value that is to be set.
        /// </param>
        /// <param name="value">The new value. </param>
        public override void SetValue(object component, object value)
        {
        }
        
        /// <override/>
        /// <summary>
        /// Resets the value for this property of the
        /// component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be
        /// reset to the default value. </param>
        public override void ResetValue(object component)
        {
        }
        
        /// <override/>
        /// <summary>
        /// Gets the current value of the property on a
        /// component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve
        /// the value. </param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            Record record = (Record)component;
            return record.ParentChildTable.Records.IndexOf(record);
        }
        
        /// <override/>
        /// <summary>
        /// Returns whether resetting an object changes
        /// its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability. </param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <override/>
        public override Type PropertyType
        {
            get
            {
                return typeof(int);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this
        /// property is read-only.
        /// </summary>
        /// <override/>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the type of the component this property
        /// is bound to.
        /// </summary>
        /// <override/>
        public override Type ComponentType
        {
            get
            {
                return typeof(Record);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the member is browsable.
        /// </summary>
        /// <override/>
        public override bool IsBrowsable
        {
            get
            {
                return false;
            }
        }
    }
}
