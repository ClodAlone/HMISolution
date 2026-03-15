using System;
using System.ComponentModel;

namespace WPFUtilities.Extensions
{
    public static class DependencyPropertyDescriptorExtension
    {
        public static void AddValueChangedSafe(this DependencyPropertyDescriptor dpd, object component, EventHandler handler)
        {
            lock (dpd)
            {
                dpd.AddValueChanged(component, handler);
            }
        }

        public static void RemoveValueChangedSafe(this DependencyPropertyDescriptor dpd, object component, EventHandler handler)
        {
            lock (dpd)
            {
                dpd.RemoveValueChanged(component, handler);
            }
        }
    }
}
