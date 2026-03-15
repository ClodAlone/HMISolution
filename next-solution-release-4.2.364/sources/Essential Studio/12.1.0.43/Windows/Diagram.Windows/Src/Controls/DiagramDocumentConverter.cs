#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// Type converter for a DiagramDocument.
    /// </summary>
    public class DiagramDocumentConverter
        : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            bool bCanConvertFrom = base.CanConvertTo(context, destinationType);

            if (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor))
            {
                bCanConvertFrom = true;
            }

            return bCanConvertFrom;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object objToReturn = null;

            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ci = typeof(DiagramDocument).GetConstructor(new Type[] { typeof(Model), typeof(View) });
                DiagramDocument t = (DiagramDocument)value;

                objToReturn = new InstanceDescriptor(ci, new object[] { t.Model, t.View });
            }
            else
                if (CanConvertTo(context, destinationType))
                {
                    DiagramDocument document = value as DiagramDocument;

                    if (document != null && document.Model != null)
                    {
                        objToReturn = document.Model.Name;
                    }
                }
                else
                {
                    objToReturn = base.ConvertTo(context, culture, value, destinationType);
                }

            return objToReturn;
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            bool bCanConvertFrom = base.CanConvertFrom(context, sourceType);

            if (sourceType == typeof(byte[]))
            {
                bCanConvertFrom = true;
            }

            return bCanConvertFrom;
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            object objToReturn = null;

            if (value.GetType() == typeof(string))
            {
                string fileName = (string)value;
                Diagram document = context.Instance as Diagram;

                if (document != null)
                {
                    // create temporary diagram control to load document
                    Diagram diagram = new Diagram();
                    diagram.LoadBinary(fileName);

                    // add model component if need
                    if (document.Model == null)
                        document.Container.Add(diagram.Model);
                    else
                    {
                        bool bPresent = false;

                        foreach (object component in document.Container.Components)
                        {
                            if (component == document.Model)
                            {
                                diagram.Document.ModelComponent = component as Model;
                                bPresent = true;
                                break;
                            }
                        }

                        if (!bPresent)
                            document.Container.Add(document.Model);
                    }

                    // return loaded document
                    objToReturn = diagram.Document;
                }
            }
            else
            {
                objToReturn = base.ConvertFrom(context, culture, value);
            }

            return objToReturn;
        }

        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value; otherwise, false.
        /// </returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}