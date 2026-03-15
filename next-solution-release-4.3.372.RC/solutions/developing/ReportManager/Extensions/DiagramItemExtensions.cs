using DevExpress.Xpf.Diagram;
using System;
using System.ComponentModel;

namespace ReportManager.Extensions
{
    internal static class DiagramItemExtensions
    {
        public static PropertyDescriptor FindPropertyDescriptor(this DiagramItem item, string propertyName)
        {
            foreach (PropertyDescriptor prop in item.Controller.EditableProperties)
            {
                if (prop.Name == propertyName)
                    return prop;
            }

            return null;
        }
    }
}
