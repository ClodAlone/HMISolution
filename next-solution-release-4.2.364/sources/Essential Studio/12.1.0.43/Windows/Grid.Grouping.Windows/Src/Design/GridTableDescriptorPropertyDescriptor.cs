//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableDescriptorPropertyDescriptor.cs" company="syncfusion">
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

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping.Design
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping.Design
#endif
{    
    internal class GridTableDescriptorPropertyDescriptor : PropertyDescriptor 
    {
        // Fields
        private TableDescriptor sip;
        string name;

        // Constructor
        public GridTableDescriptorPropertyDescriptor(string name, TableDescriptor sip, Attribute[] attributes)
            : base(name, attributes)
        {
            this.sip = sip;
            this.name = name;
        }

        // Methods

        /// <internalonly/>
        /// <summary>
        ///    Determines if the the component will allow its value to be reset.
        /// </summary>
        /// <param name="comp">The component to reset.</param>
        /// <returns>
        ///    true if the component supports resetting its value.
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool CanResetValue(object comp)  
        {
            return sip.GetModified();
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the value of the property for the given component.  This will
        ///    throw an exception if the component does not have this property.
        /// </summary>
        /// <param name="comp">The component.</param>
        /// <returns>
        ///    the value of the property on comp.  This can be casted
        ///    to the property type.
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override object GetValue(object comp)  
        {
            return sip;
        }

        /// <internalonly/>
        /// <summary>
        ///    Resets the value of this property on comp to the default value.
        /// </summary>
        /// <param name="comp">The component whose property is to be reset.</param>
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        public override void ResetValue(object comp)  
        {
            sip.ResetTableDescriptor();
        }

        /// <internalonly/>
        /// <summary>
        ///    Sets the value of this property on the given component.
        /// </summary>
        /// <param name="comp">The component whose property is to be set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        public override void SetValue(object comp, object value)  
        {
        }

        /// <internalonly/>
        /// <summary>
        ///    <para>Determines if this property should be persisted. A property is
        ///       to be persisted if it is marked as persistable through a
        ///       PersistableAttribute, and if the property contains something other
        ///       than the default value. Note, however, that this method will
        ///       return true for design time properties as well, so callers
        ///       should also check to see if a property is design time only before
        ///       persisting to runtime storage.</para>
        /// </summary>
        /// <param name='comp'>The component on which the property resides.</param>
        /// <returns>
        ///    <para>true if the property should be persisted to either
        ///       design time or run time storage.</para>
        /// </returns>
        public override bool ShouldSerializeValue(object comp)  
        {
            return sip.GetModified();
        }

        // Properties

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the type of the component this PropertyDescriptor is bound to.
        /// </summary>
        /// <returns>
        ///    the type of component.
        /// </returns>
        public override Type ComponentType
        {
            get 
            {
                return typeof(GridGroupingControl);
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the display name of the property.  This is the name that will
        ///    be displayed in a property browser.  This will be the same as the property
        ///    name for most properties.
        /// </summary>
        /// <returns>
        ///    a string containing the name to display in the property browser.
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
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
        ///    true if the property can be written to.
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override bool IsReadOnly
        {
            get 
            {
                return false; ////this.Attributes[typeof(System.ComponentModel.ReadOnlyAttribute)].Equals((object)ReadOnlyAttribute.Yes);
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    Retrieves the data type of the property.
        /// </summary>
        /// <returns>
        ///    a class representing the data type of the property.
        /// <seealso cref="T:System.ComponentModel.PropertyDescriptor"/>
        /// </returns>
        public override Type PropertyType
        {
            get 
            {
                return sip.GetType();
            }
        }
    }
}
