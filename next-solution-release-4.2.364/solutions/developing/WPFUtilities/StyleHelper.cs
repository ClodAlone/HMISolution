using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DevExpress.LookAndFeel;
using DevExpress.Xpf.Core;
using Utilities;
using System.Windows.Markup;
using System.Windows.Data;
using System.Xaml;

namespace WPFUtilities
{
    public static class MergedStylesExtension : object
    {
        public static void ProvideCustomValues<T>(T root)
        {
            try
            {
                FrameworkElement fe = root as FrameworkElement;
                if (fe == null)
                    return;
                string mergeStyle = $"Custom{typeof(T).Name}Style";
                Style MergeStyle = fe.TryFindResource(mergeStyle) as Style;

                if (MergeStyle == null)
                    return;
               
                MergeWithStyle<T>(root,MergeStyle);

                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        private static void MergeWithStyle<T>(T root,Style mergeStyle)
        {
            // Recursively merge with any Styles this Style
            // might be BasedOn.
            if (mergeStyle.BasedOn != null)
            {
                MergeWithStyle(root, mergeStyle.BasedOn);
            }

            // Merge the Setters...
            foreach (System.Windows.Setter setter in mergeStyle.Setters)
            {
                string propName = setter.Property.Name;
                var prop = typeof(T).GetProperty(propName);
                try
                {
                    prop?.SetValue(root, setter.Value);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
