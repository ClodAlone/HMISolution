//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorderCodeDomSerializer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///        A <see cref="CodeDomSerializer"/> for <see cref="GridBorder"/>.
    /// </summary>
    public class GridBorderCodeDomSerializer : CodeDomSerializer
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridBorderCodeDomSerializer()
            : base()
        {
        }

        // Methods.

        /// <override/>
        /// <summary>
        /// Deserializes the specified serialized CodeDOM object into an object.
        /// </summary>
        /// <param name="manager">A serialization manager interface that is used during the
        /// deserialization process. </param>
        /// <param name="codeObject">A serialized CodeDOM object to deserialize. </param>
        /// <returns>
        /// The deserialized CodeDOM object.
        /// </returns>
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            return null;
        }

        /// <override/>
        /// <summary>
        /// Serializes the specified object into a CodeDOM object.
        /// </summary>
        /// <param name="manager">The serialization manager to use during serialization.
        /// </param>
        /// <param name="value">The object to serialize. </param>
        /// <returns>
        /// A CodeDOM object representing the object that has been serialized.
        /// </returns>
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(@"manager");
            }

            if (!(value is GridBorder) || value == null)
            {
                throw new ArgumentException(@"value");
            }

            GridBorder border = (GridBorder)value;

            CodeObjectCreateExpression coce = new CodeObjectCreateExpression();
            coce.CreateType = new CodeTypeReference("Syncfusion.Windows.Forms.Grid.GridBorder");

            switch (border.Style)
            {
                case GridBorderStyle.NotSet:
                    break;

                case GridBorderStyle.None:
                    coce.Parameters.Add(this.SerializeToExpression(manager, border.Style));
                    break;

                default:
                    if (border.Weight == GridBorderWeight.Thin)
                    {
                        coce.Parameters.Add(this.SerializeToExpression(manager, border.Style));
                        coce.Parameters.Add(this.SerializeToExpression(manager, border.Color));
                    }
                    else
                    {
                        coce.Parameters.Add(this.SerializeToExpression(manager, border.Style));
                        coce.Parameters.Add(this.SerializeToExpression(manager, border.Color));
                        coce.Parameters.Add(this.SerializeToExpression(manager, border.Weight));
                    }

                    break;
            }

            return coce;
        }

        // Properties.

        /// <summary>
        /// Gets a default global instance of <see cref="GridBorderCodeDomSerializer"/>.
        /// </summary>
        public static GridBorderCodeDomSerializer Default
        {
            get
            {
                if (GridBorderCodeDomSerializer.defaultSerializer == null)
                {
                    GridBorderCodeDomSerializer.defaultSerializer = new GridBorderCodeDomSerializer();
                }

                return GridBorderCodeDomSerializer.defaultSerializer;
            }
        }

        // Fields.
        private static GridBorderCodeDomSerializer defaultSerializer;
    }
}
