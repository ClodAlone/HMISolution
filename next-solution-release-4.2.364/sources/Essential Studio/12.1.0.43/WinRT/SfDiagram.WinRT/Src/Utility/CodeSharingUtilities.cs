#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
#if WINRT_USING
using Syncfusion.UI.Xaml.Diagram.Utility;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Markup;
using PointerRoutedEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows.Input; 
#endif

#if !WINRT

namespace Syncfusion.UI.Xaml.Diagram
{
    internal static class CodeSharingUtilities
    {
        public static UIElement ElementAt(this UIElementCollection collection, int index)
        {
            return collection[index];
        }

        public static Type[] ImplementedInterfaces(this Type s)
        {
            return s.GetInterfaces();
        }

        public static Type[] DefinedTypes(this Assembly s)
        {
            return s.GetTypes();
        }

        public static Type[] GenericTypeArguments(this Type s)
        {
            return s.GetGenericArguments();
        }

        public static bool CapturePointer(this UIElement s, PointerRoutedEventArgs dummy)
        {
            return s.CaptureMouse();
        }

        public static void ReleasePointerCapture(this UIElement s, PointerRoutedEventArgs dummy)
        {
            s.ReleaseMouseCapture();
        }

        public static bool IsControlKeyPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }
#if SILVERLIGHT
        public static bool IsDesignMode(this DependencyObject s)
        {
            return DesignerProperties.IsInDesignTool;
        } 

        public static object LoadXaml(this string xaml)
        {
            return XamlReader.Load(xaml);
        }
#endif
#if WPF
        public static bool IsDesignMode(this DependencyObject s)
        {
            return DesignerProperties.GetIsInDesignMode(s);
        } 

        public static object LoadXaml(this string xaml)
        {
            return XamlReader.Parse(xaml);
        }
#endif
    }

    public partial class Selector
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Node
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Connector
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class SfDiagram
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

}

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    public partial class ScrollViewer
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class AnnotationEditor
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Ruler
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class RulerSegment
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class DiagramThumb
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
}

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
    public partial class Stencil : Control
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
    public partial class SymbolGroup : ItemsControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Symbol : ContentControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
}

#endif
#if WINRT

namespace Syncfusion.UI.Xaml.Diagram
{
    internal static class CodeSharingUtilities
    {
        public static IEnumerable<Type> ImplementedInterfaces(this TypeInfo s)
        {
            return s.ImplementedInterfaces;
        }

        public static IEnumerable<TypeInfo> DefinedTypes(this Assembly s)
        {
            return s.DefinedTypes;
        }
        
        public static Type[] GenericTypeArguments(this Type s)
        {
            return s.GenericTypeArguments;
        }

        public static bool CapturePointer(this UIElement s, PointerRoutedEventArgs args)
        {
            if (s.Visibility == Visibility.Visible)
            {
                return s.CapturePointer(args.Pointer);
            }
            return false;
        }

        public static void ReleasePointerCapture(this UIElement s, PointerRoutedEventArgs args)
        {
            s.ReleasePointerCapture(args.Pointer);
        }

        public static bool IsControlKeyPressed()
        {
            return Window.Current.CoreWindow.GetKeyState(VirtualKey.Control)
                         .Contains(CoreVirtualKeyStates.Down);
        }

        public static bool IsDesignMode(this DependencyObject s)
        {
            return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
        }

        public static object LoadXaml(this string xaml)
        {
            return XamlReader.Load(xaml);
        }

        public static IEnumerable<DependencyObject> FindElementsInHostCoordinates(this UIElement s, Point relativeTo)
        {
            return VisualTreeHelper.FindElementsInHostCoordinates(relativeTo, s);
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this TypeInfo tf)
        {
            return tf.DeclaredConstructors;
        }
       
    }

    public partial class Selector
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Node
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Connector
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class SfDiagram
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
}

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    public partial class ScrollViewer
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
    public partial class AnnotationEditor
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
    public partial class Ruler
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class RulerSegment
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class DiagramThumb
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
}

namespace Syncfusion.UI.Xaml.Diagram.Stencil
{
    public partial class Stencil : Control
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class SymbolGroup : ItemsControl
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }

    public partial class Symbol : ContentControl
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DoApplyTemplate();
        }
    }
}


#endif
