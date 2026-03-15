using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace Utilities.WPF
{
    static public class Cloners
    {
        /// <summary>
        /// Clones the specified obj.
        /// </summary>
        /// <param name="obj">The object to be cloned.</param>
        /// <returns>The cloned object.</returns>
        public static object Clone(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            object cloneObj = obj.GetType().GetConstructors()[0].Invoke(null);
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);
                if (value != null)
                {
                    try
                    {
                        if (IsPresentationFrameworkCollection(value.GetType()))
                        {
                            object collection = property.GetValue(obj, null);
                            int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                            for (int i = 0; i < count; i++)
                            {
                                object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                object cloneChild = Clone(child);
                                object cloneCollection = property.GetValue(cloneObj, null);
                                collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                            }
                        }

                        if (value is UIElement)
                        {
                            object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                            Clone(obj2);
                            property.SetValue(cloneObj, obj2, null);
                        }
                        else if (property.CanWrite)
                        {
                            property.SetValue(cloneObj, value, null);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return cloneObj;
        }

        private static bool IsPresentationFrameworkCollection(Type type)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.Name.StartsWith("PresentationFrameworkCollection"))
            {
                return true;
            }

            return IsPresentationFrameworkCollection(type.BaseType);
        }

        public static IList<DependencyProperty> GetSetedProperties(DependencyObject obj)
        {
            List<DependencyProperty> seted = new List<DependencyProperty>();

            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj,
                new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                DependencyPropertyDescriptor dpd =
                    DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null && obj.ReadLocalValue(dpd.DependencyProperty) != DependencyProperty.UnsetValue)
                {
                    seted.Add(dpd.DependencyProperty);
                }
            }

            return seted;
        }

        public static void CopyObjects(DependencyObject source, DependencyObject destination, bool bIncludeChild)
        {
            if (bIncludeChild && source is IAddChild)
            {
                ContentPropertyAttribute srcCntAttr;
                if ((TypeDescriptor.GetAttributes(source)[typeof(ContentPropertyAttribute)] is ContentPropertyAttribute))
                {
                    srcCntAttr = TypeDescriptor.GetAttributes(source)[typeof(ContentPropertyAttribute)] as ContentPropertyAttribute;
                    if (destination.GetType().GetProperty(srcCntAttr.Name) != null)
                    {

                        System.Reflection.PropertyInfo pi = source.GetType().GetProperty(srcCntAttr.Name);
                        object srcChild = pi.GetValue(source, null);
                        object destChild = pi.GetValue(destination, null);
                        if (srcChild is UIElementCollection && destChild is UIElementCollection)
                        {
                            UIElementCollection srcColl = srcChild as UIElementCollection;
                            UIElementCollection destColl = destChild as UIElementCollection;
                            destColl.Clear();
                            while (srcColl.Count > 0)
                            {
                                UIElement el = srcColl[0];
                                srcColl.Remove(el);
                                destColl.Add(el);
                            }

                        }
                        else if (srcChild is DependencyObject && destChild is DependencyObject)
                        {
                            CopyObjects(srcChild as DependencyObject, destChild as DependencyObject, bIncludeChild);
                        }

                    }
                }
            }
            IList<DependencyProperty> spl = GetSetedProperties(source);
            IList<DependencyProperty> dpl = GetSetedProperties(destination);
            foreach (DependencyProperty property in dpl)
            {
                try
                {
                    if (property.ReadOnly != true && property.Name != "Style")
                        // destination.SetValue(property, source.ReadLocalValue(property));
                        destination.SetValue(property, DependencyProperty.UnsetValue);
                }
                catch (Exception)
                {
                }
            }
            foreach (DependencyProperty property in spl)
            {
                if (property.ReadOnly != true && property.Name != "Style")
                    destination.SetValue(property, source.ReadLocalValue(property));
            }
        }

        public static UIElement Clone(this UIElement source)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings 
            { 
                Indent = true, 
                OmitXmlDeclaration = true 
            };
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(sb, settings));
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(source, dsm);

            // string clon = XamlWriter.Save(source);
            return sb.ToString().ReadUIElement();
        }

        public static Object CloneUsingXaml(Object o)
        {
            InitXamlBinding();

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(sb, settings));
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(o, dsm);
            // string xaml = XamlWriter.Save(o);
            return sb.ToString().ReadUIElement();
        }

        public static T DeepClone<T>(T from) 
        { 
            using (MemoryStream s = new MemoryStream()) 
            { 
                BinaryFormatter f = new BinaryFormatter(); 
                f.Serialize(s, from); 
                s.Position = 0; 
                object clone = f.Deserialize(s); 
                return (T)clone; 
            } 
        }

        static bool Initialized;
        public static void InitXamlBinding()
        {
            if (Initialized)
                return;
            Initialized = true;
            EditorHelper.Register<BindingExpression, BindingConvertor>();
        }
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
                BindingExpression bindingExpression = value as BindingExpression;
                if (bindingExpression == null)
                    throw new Exception();
                return bindingExpression.ParentBinding;
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
        public BindingTypeDescriptionProvider()
            : base(defaultTypeProvider)
        {

        }
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);
            return instance == null ? defaultDescriptor : new BindingCustomTypeDescriptor(defaultDescriptor);
        }
    }

    class BindingCustomTypeDescriptor : CustomTypeDescriptor
    {
        public BindingCustomTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
        }
        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(base.GetProperties().Cast<PropertyDescriptor>().ToArray());
            PropertyDescriptor pd;
            if ((pd = pdc.Find("Source", false)) != null)
            {
                pdc.Add(TypeDescriptor.CreateProperty(typeof(System.Windows.Data.Binding), pd, new Attribute[] { new System.ComponentModel.DefaultValueAttribute("null") }));
                pdc.Remove(pd);
            }
            return pdc;
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(base.GetProperties(attributes).Cast<PropertyDescriptor>().ToArray());
            PropertyDescriptor pd;
            if ((pd = pdc.Find("Source", false)) != null)
            {
                pdc.Add(TypeDescriptor.CreateProperty(typeof(System.Windows.Data.Binding), pd, new Attribute[] { new System.ComponentModel.DefaultValueAttribute("null") }));
                pdc.Remove(pd);
            }
            return pdc;
        }
    }
}
