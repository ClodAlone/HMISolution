using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using System.ComponentModel;
using System.Windows;

namespace WPFUtilities
{
    public static class PropertyNameHelper
    {
        public static PropertyDescriptor GetLocalizedPropertyDescriptor(IDocument document, Type targetType, DependencyProperty dependencyProperty)
        {
            if (document != null)
            {
                IPropertyControl propertyControl = document.GetService(typeof(IPropertyControl)) as IPropertyControl;
                if (propertyControl != null)
                    return propertyControl.GetLocalizedPropertyDescriptor(dependencyProperty, targetType);
                else
                    return DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType) as PropertyDescriptor;

            }
            return DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType) as PropertyDescriptor;
        }
    }
}
