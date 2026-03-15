#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Interface of IChartSerializer
    /// </summary>
    public interface IChartSerializer
    {
        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        string Serialize();

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        object Deserialize(string xamlString);
    }

    class BindingConvertor : ExpressionConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
                return true;
            else return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context,
                                        System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {
                if (value is MultiBindingExpression)
                {
                    MultiBindingExpression bindingExpression = value as MultiBindingExpression;
                    if (bindingExpression == null)
                        throw new Exception();
                    return bindingExpression.ParentMultiBinding;
                }
                else if (value is BindingExpression)
                {
                    BindingExpression bindingExpression = value as BindingExpression;
                    if (bindingExpression == null)
                        throw new Exception();
                    return bindingExpression.ParentBinding;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    static class EditorHelper
    {
        public static void Register<T, TC>()
        {
            Attribute[] attr = new Attribute[1];
            TypeConverterAttribute vConv = new TypeConverterAttribute(typeof(TC));
            attr[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attr);
        }
    }

    class BindingTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider defaultTypeProvider = TypeDescriptor.GetProvider(typeof(System.Windows.Data.Binding));
        public BindingTypeDescriptionProvider() : base(defaultTypeProvider)
        {

        }
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType,instance);
            return instance == null ? defaultDescriptor : new BindingCustomTypeDescriptor(defaultDescriptor);
        }
    }
    
    class BindingCustomTypeDescriptor:CustomTypeDescriptor
    {
        public BindingCustomTypeDescriptor(ICustomTypeDescriptor parent) :base(parent)
        {
        }
        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(base.GetProperties().Cast<PropertyDescriptor>().ToArray());
            PropertyDescriptor pd;
            if((pd = pdc.Find("Source",false))!=null)
            {
                pdc.Add(TypeDescriptor.CreateProperty(typeof(System.Windows.Data.Binding),pd,new Attribute[]{new System.ComponentModel.DefaultValueAttribute("null")}));
                pdc.Remove(pd);
            }
            return pdc;
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[]attributes)
        {
            PropertyDescriptorCollection pdc=new PropertyDescriptorCollection(base.GetProperties(attributes).Cast<PropertyDescriptor>().ToArray());
            PropertyDescriptor pd;
            if((pd = pdc.Find("Source",false))!=null)
            {
                pdc.Add(TypeDescriptor.CreateProperty(typeof(System.Windows.Data.Binding),pd,new Attribute[]{new System.ComponentModel.DefaultValueAttribute("null")}));
                pdc.Remove(pd);
            }
            return pdc;
        }
    }

}
