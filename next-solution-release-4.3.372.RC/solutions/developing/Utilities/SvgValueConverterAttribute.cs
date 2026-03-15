using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Utilities
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field, Inherited = true)]
    public sealed class SvgValueConverterAttribute : Attribute
    {
        public bool RequiredKey { get; set; }
        public bool HasStyles { get; set; }
        public string TypeName { get; set; }
        public string StyleTypeName { get; set; }
        public string PropertyName { get; set; }
        public bool HasBrushes { get; set; }
        public bool NeedSVGUrlBrushes { get; set; }
        public bool HasOverrideBrushProperties { get; set; }
        public Type ConverterType { get; set; }
        public CustomValueConverter Converter { get; }
        public object ConverterParameter { get; set; }
        public SvgValueConverterAttribute()
        {
        }
        public SvgValueConverterAttribute(bool requiredKey)
        {
            RequiredKey = requiredKey;
        }
        public SvgValueConverterAttribute(Type converterType, object converterParameter = null)
        {
            RequiredKey = true;
            ConverterType = converterType;
            ConverterParameter = converterParameter;
        }
    }

    public abstract class CustomValueConverter
    {
        protected CustomValueConverter(){}
        public abstract Type StorageType { get; }
        public abstract object ConvertFromStorageType(object value, object sender);
        public abstract object ConvertToStorageType(object value, object sender, object document, object property, object parameter);
        public virtual object ConvertFromStorageType(object value, object sender, object document)
        {
            return ConvertFromStorageType(value, sender);
        }
    }
}
