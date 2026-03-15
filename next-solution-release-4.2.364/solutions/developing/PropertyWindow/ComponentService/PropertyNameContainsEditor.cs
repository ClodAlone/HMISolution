using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using Utilities;
using System.Windows.Controls;
using Mindscape.WpfElements.WpfPropertyGrid;
using PropertyControl.PropertyDataTemplate;
using Mindscape.WpfElements.PropertyEditing;
using DocumentManager.ComponentService;
using UFInterfaces.PropertyControl;

namespace PropertyControl.ComponentService
{
    public class PropertyNameContainsEditor : ObjectWrappingEditor
    {
        protected override void SetContentTemplate(FrameworkElementFactory factory, Node node)
        {
            factory.SetValue(ContentControl.ContentTemplateProperty, EditorTemplate);
        }

        public String PropertyName { get; set; }
        public Type EditType { get; set; }
        public Type DeclaringType { get; set; }
        public Type DocumentType { get; set; }
        public bool UseBaseClass { get; set; }

        public override bool CanEdit(Node node)
        {
            bool canedit = false;

            Type documentType = null;
            if (DocumentType != null)
            {
                var document = ComponentService.PropertyControlComponent.workspace.ContextDocument;
                if (document != null)
                    documentType = document.GetType();
            }
            //objects of the same type will be collected on Value
            //objects of different type will be collected on Source
            Many many = node.Value is Many ? node.Value as Many : node.Source as Many;
            if (many != null)
            {
                if (UseBaseClass)
                    canedit = many.IsConsistent && (many.ConsistentType == DeclaringType || (many.ConsistentType).IsSubclassOf(DeclaringType)) && node.PropertyType == EditType && documentType == DocumentType;
                else
                    canedit = many.IsConsistent && many.ConsistentType == DeclaringType && node.PropertyType == EditType && documentType == DocumentType;

                if (canedit && !String.IsNullOrEmpty(PropertyName))
                    canedit = PropertyName == many.PropertyName;
            }
            else
            {
                // Ignoring issues of casing, culture, etc. for simplicity
                if (UseBaseClass)
                    canedit = (node.PropertyInfo.DeclaringType == DeclaringType || (node.PropertyInfo.DeclaringType).IsSubclassOf(DeclaringType))
                        && node.PropertyType == EditType && documentType == DocumentType;
                else 
                    canedit = node.PropertyInfo.DeclaringType == DeclaringType && node.PropertyType == EditType && documentType == DocumentType;
                if (canedit && !String.IsNullOrEmpty(PropertyName))
                    canedit = node.Name == PropertyName;
            }

            return canedit;
        }

        /// <summary>
        /// Allows derived classes to customize the visual tree of the template constructed in 
        /// <see cref="BuildTemplate"/>, for example by setting up additional properties.
        /// </summary>
        /// <param name="factory">The FrameworkElementFactory that defines the template visual tree.</param>
        /// <param name="node">The node which the template will edit.</param>
        protected override void OnCustomizeTemplate(FrameworkElementFactory factory, Node node)
        {
            if (node.PropertyType == typeof(string))
            {
                if (!node.CanWrite)
                {
                    factory.SetValue(ContentControl.IsEnabledProperty, false);
                    factory.SetValue(ContentControl.OpacityProperty, 0.4);
                }
            }
            else
            {
                var propertyName = node.Property.Name;
                Many many = node.Source as Many;
                if (many != null)
                    propertyName = many.PropertyName;
                var canWrite = !PropertyControlComponent.propertyControlComponent.IsSelectedPropertyReadOnly(propertyName);
                if (!canWrite)
                {
                    factory.SetValue(ContentControl.IsEnabledProperty, false);
                    factory.SetValue(ContentControl.OpacityProperty, 0.4);
                }
            }
        }
    }
}
