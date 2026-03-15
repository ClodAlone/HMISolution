using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;

namespace Buttons.Helpers
{
    internal static class TagUIElementHelper
    {
        public static bool IsStyledSymbol(FrameworkElement fe)
        {
            var element = fe;
            while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
            {
                if (!String.IsNullOrEmpty(element.Name))
                    break;
            }

            return element is ContentControl && (element as ContentControl).Content is UIElement && !(element is UserControl);
        }

        public static bool TagNameExists(UIElement uie, string tagName)
        {
            return (from c in uie.GetVisualChildrenOfType<Panel>()
                    where (c.Tag as String) == tagName
                    select c).FirstOrDefault() != null ||
                    (from c in uie.GetVisualChildrenOfType<Control>()
                     where (c.Tag as String) == tagName
                     select c).FirstOrDefault() != null ||
                    (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                     where (c.Tag as String) == tagName
                     select c).FirstOrDefault() != null ||
                     (from c in uie.GetVisualChildrenOfType<Border>()
                      where (c.Tag as String) == tagName
                      select c).FirstOrDefault() != null;
        }
    }
}
