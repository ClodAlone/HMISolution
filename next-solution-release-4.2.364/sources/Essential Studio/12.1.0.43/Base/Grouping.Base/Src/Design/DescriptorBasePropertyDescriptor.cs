//-------------------------------------------------------------------------------------------------
// <copyright file="DescriptorBasePropertyDescriptor.cs" company="syncfusion">
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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A custom PropertyDescriptor that adds design-time support for descriptor collections
    /// in the grouping enegine. DescriptorBasePropertyDescriptor objects help with adding support for
    /// expanding a collection similar to an expandable object in a property grid. 
    /// </summary>
    public class DescriptorBasePropertyDescriptor : PropertyDescriptor
    {
        // Fields
        private DescriptorBase columnDescriptor;
        string name;
        Type componentType;

        // Constructor

        /// <summary>
        /// Initializes a new DescriptorBasePropertyDescriptor object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="columnDescriptor">The descriptor element.</param>
        /// <param name="attributes">Property attributes (passed through to PropertyDescriptor ctor).</param>
        /// <param name="componentType">The component type (passed through to PropertyDescriptor ctor).</param>
        public DescriptorBasePropertyDescriptor(string name, DescriptorBase columnDescriptor, Attribute[] attributes, Type componentType)
            : base(name, attributes)
        {
            this.columnDescriptor = columnDescriptor;
            this.name = name;
            this.componentType = componentType;
        }

        // Methods

        /// <internalonly/>
        /// <summary>
        ///    Determines if the the component will allow its value to be reset.
        /// </summary>
        /// <param name="comp">The component to reset.</param>
        /// <returns>
        ///    True if the component supports resetting its value.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool CanResetValue(object comp)
        {
            return columnDescriptor.CanResetValue();
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the value of the property for the given component. This will
        ///    throw an exception if the component does not have this property.
        /// </summary>
        /// <param name="comp">The component.</param>
        /// <returns>
        ///    The value of the property on comp. This can be cast
        ///    to the property type.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override object GetValue(object comp)
        {
            return columnDescriptor;
        }

        /// <internalonly/>
        /// <summary>
        ///    Resets the value of this property on comp to the default value.
        /// </summary>
        /// <param name="comp">The component whose property is to be reset.</param>
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        public override void ResetValue(object comp)
        {
            columnDescriptor.Reset();
        }

        /// <internalonly/>
        /// <summary>
        ///    Sets the value of this property on the given component.
        /// </summary>
        /// <param name="comp">The component whose property is to be set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        public override void SetValue(object comp, object value)
        {
        }

        /// <internalonly/>
        /// <summary>
        ///    <para>Determines if this property should be persisted. A property is
        ///       to be persisted if it is marked as persistable through a
        ///       PersistableAttribute and if the property contains something other
        ///       than the default value. Note, however, that this method will
        ///       return True for design-time properties as well, so callers
        ///       should also check to see if a property is design-time only before
        ///       persisting to run-time storage.</para>
        /// </summary>
        /// <param name='comp'>The component on which the property resides.</param>
        /// <returns>
        ///    <para>True if the property should be persisted to either
        ///       design-time or run-time storage.</para>
        /// </returns>
        public override bool ShouldSerializeValue(object comp)
        {
            return columnDescriptor.ShouldSerialize();
        }

        // Properties

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        /// <returns>
        ///    The type of component.
        /// </returns>
        public override Type ComponentType
        {
            get
            {
                return componentType;
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the display name of the property. This is the name that will
        ///    be displayed in a property browser. This will be the same as the property
        ///    name for most properties.
        /// </summary>
        /// <returns>
        ///    A string containing the name to display in the property browser.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override string DisplayName
        {
            get
            {
                return this.name;
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    Determines if the property can be written to.
        /// </summary>
        /// <returns>
        ///    True if the property can be written to.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the data type of the property.
        /// </summary>
        /// <returns>
        ///    A class representing the data type of the property.
        /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override Type PropertyType
        {
            get
            {
                return columnDescriptor.GetType();
            }
        }
    }
}
