using Mindscape.WpfElements.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces.PropertyControl;

namespace PropertyControl.ComponentService
{
    public class PropertyTypeEditor : TypeEditor
    {
        /// <summary>
        /// Constructs a <see cref="DataTemplate"/> which can be bound to the specified node
        /// to edit its value.
        /// </summary>
        /// <param name="node">The node for which an editor is required.</param>
        /// <returns>A data template for editing the node value.</returns>
        public override DataTemplate BuildTemplate(Node node)
        {
            var template = base.BuildTemplate(node);

            if (node.PropertyType == typeof(string))
            {
                if (!node.CanWrite)
                {
                    template.VisualTree.SetValue(ContentControl.IsEnabledProperty, false);
                    template.VisualTree.SetValue(ContentControl.OpacityProperty, 0.4);
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
                    template.VisualTree.SetValue(ContentControl.IsEnabledProperty, false);
                    template.VisualTree.SetValue(ContentControl.OpacityProperty, 0.4);
                }
            }
            return template;
        }
    }
}
