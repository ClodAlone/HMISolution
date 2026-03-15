//-------------------------------------------------------------------------------------------------
// <copyright file="UnboundFieldDescriptor.cs" company="syncfusion">
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
using System.Text;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A collection of <see cref="FieldDescriptor"/> fields with unbound fields. 
    /// An instance of this collection is returned by the <see cref="TableDescriptor.UnboundFields"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class UnboundFieldDescriptorCollection : FieldDescriptorCollection
    {
        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static new readonly UnboundFieldDescriptorCollection Empty = new UnboundFieldDescriptorCollection(null);

        /// <override/>
        protected override void EnsureInitialized(bool populate)
        {
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public UnboundFieldDescriptorCollection()
        {
        }

        internal UnboundFieldDescriptorCollection(TableDescriptor tableDescriptor)
            : base(tableDescriptor)
        {
        }

        internal UnboundFieldDescriptorCollection(TableDescriptor tableDescriptor, FieldDescriptor[] columnDescriptors)
            : base(tableDescriptor, columnDescriptors)
        {
        }

        /// <summary>
        /// Called from InternalClone to create a new collection and attach it to the specified table descriptor
        /// and insert the specified fields. The fields have already been cloned.
        /// </summary>
        /// <param name="td">The table descriptor.</param>
        /// <param name="fieldDescriptors">The cloned field descriptors.</param>
        /// <returns>A new FieldDescriptorCollection.</returns>
        /// <override/>
        protected override FieldDescriptorCollection CreateCollection(TableDescriptor td, FieldDescriptor[] fieldDescriptors)
        {
            return new UnboundFieldDescriptorCollection(td, fieldDescriptors);
        }

        /// <override/>
        protected internal override void SuggestName(FieldDescriptor value)
        {
            int n = 1;
            foreach (FieldDescriptor col in this)
            {
                if (col.Name.StartsWith("Unbound "))
                {
                    double d;
                    if (double.TryParse(col.Name.Substring("Unbound ".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = (int)d + 1;
                    }
                }
            }

            value.Name = "Unbound " + n.ToString();
            value.nameModified = false;
        }
    }
}

