using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CommonControls
{
    public class CommonProperties
    {
        private static Dictionary<UIElement, int> animatingElements = new Dictionary<UIElement, int>();
        
        public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.RegisterAttached("IsAnimating", typeof(Boolean), typeof(CommonProperties),
            new PropertyMetadata(false));
        public static void SetIsBrushAnimating(UIElement element, Boolean value)
        {
            if (!value)
            {
                if (animatingElements.ContainsKey(element))
                {
                    animatingElements[element]--;
                    if (animatingElements[element] == 0)
                    {
                        animatingElements.Remove(element);
                        element.SetValue(IsAnimatingProperty, false);
                    }
                }
            }
            else
            {
                if (!animatingElements.ContainsKey(element))
                    animatingElements.Add(element, 0);
                animatingElements[element]++;
                element.SetValue(IsAnimatingProperty, true);
            }
        }
        public static Boolean GetIsBrushAnimating(UIElement element)
        {
            return (Boolean)element.GetValue(IsAnimatingProperty);
        }
    }
}
