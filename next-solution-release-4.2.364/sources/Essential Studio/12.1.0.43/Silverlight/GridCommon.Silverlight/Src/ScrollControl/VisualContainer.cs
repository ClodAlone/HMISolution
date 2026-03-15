#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;

#if !WinRT
using System.Windows.Controls;
using Syncfusion.Windows.GridCommon;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Syncfusion.WinRT.GridCommon;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// VisualContainer maintains a collection of child elements. Adding
    /// and removing elements from the children collection does not trigger
    /// calls to InvalidateMeasure. This allows adding and removing elements
    /// on the fly. A derived control is responsible to call Measure and Arrange
    /// on child elements since this base class will not do this by itsself.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class VisualContainer : Panel
    {
        #region DependencyProperties

        #region WantsMouseInputProperty
        /// <summary>
        /// <see cref="MouseControllerDispatcher"/> checks this attached property for the UIElement returned by
        /// MouseDevice.DirectlyOver when the user hovers the mouse over an element. When
        /// it is true the MouseControllerDispatcher assumes the child UI Element wants to 
        /// handle mouse events itsself. In such case each MouseController will take this 
        /// into consideration. The default value is null. TextBox and Checkbox renderers
        /// usually set this property to true. <para/>
        /// If the property is true the SelectCellsMouseController.HitTest method will return 0 
        /// and thus indicating that it will not want to take control. Another MouseController 
        /// might decide to handle the mouse action anyway and in such case the UIElement will 
        /// not get the mouse action. <para/>
        /// When the value is null the parent element is queried. <para/>
        /// When the value is false the parent element is not queried. <para/>
        /// </summary>
        public static readonly DependencyProperty WantsMouseInputProperty = DependencyProperty.RegisterAttached(
            "WantsMouseInput", typeof(bool?), typeof(VisualContainer), null);

        /// <summary>
        /// Gets the <see cref="WantsMouseInputProperty"/> attached dependency property value.
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <param name="falseIfParent">Abprt search parent elements when the parent matches the given UIElement.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static bool? GetWantsMouseInput(DependencyObject dpo, UIElement falseIfParent)
        {
            while (dpo.GetValue(WantsMouseInputProperty) == null)
            {
                DependencyObject parent = GetParent(dpo);
                if (parent == falseIfParent)
                    return false;
                if (parent == null)
                    return null;
                dpo = parent;
            }
            return (bool?)dpo.GetValue(WantsMouseInputProperty);
        }

        /// <summary>
        /// Sets the <see cref="WantsMouseInputProperty"/>  attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetWantsMouseInput(DependencyObject dpo, bool? value)
        {
            dpo.SetValue(WantsMouseInputProperty, value);
        }

        #endregion
        #region CellRenderBoundsProperty
        /// <summary>
        /// The value of this property represents the distance between the left side of an element 
        /// and the left side of its parent Canvas. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty CellRenderBoundsProperty = DependencyProperty.RegisterAttached(
            "CellRenderBounds", typeof(Rect), typeof(VisualContainer), new PropertyMetadata(Rect.Empty));

        /// <summary>
        /// Gets the <see cref="CellRenderBoundsProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static Rect GetRenderBoundsInherited(DependencyObject dpo)
        {
            return (Rect) GridUtil.GetValueInherited(dpo, CellRenderBoundsProperty, Rect.Empty);
        }

        public static Rect GetRenderBounds(DependencyObject dpo)
        {
            return (Rect) dpo.GetValue(CellRenderBoundsProperty);
        }

        /// <summary>
        /// Sets the <see cref="CellRenderBoundsProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetRenderBounds(DependencyObject dpo, Rect value)
        {
            dpo.SetValue(CellRenderBoundsProperty, value);
        }
        #endregion

        public static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }
            FrameworkElement element = current as FrameworkElement;
            if (element != null)
            {
                if (element.Parent != null)
                    return element.Parent;
                //else if (element.TemplatedParent != null)
                //    return element.TemplatedParent;
            }
            //FrameworkContentElement element2 = current as FrameworkContentElement;
            //if (element2 != null)
            //{
            //    return element2.Parent;
            //}
            return null;
        }



        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualContainer"/> class.
        /// </summary>
        public VisualContainer(string name)
        {
            //Name = name;
            this.Background = new SolidColorBrush(Colors.Transparent);
        }
        public VisualContainer()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
        }
        #endregion
        #region Layout and Rendering is all handled by derived control
        /// <summary>
        /// Returns the same exact same size as specified by constraint.
        /// </summary>
        /// <param name="constraint">The maximum size limit for the control.</param>
        /// <returns>Same size as given in constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        /// <summary>
        /// Returns the same exact same size as specified by finalSize without arranging
        /// child elements.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>Same size as given in finalSize.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //Trace.Write("ArrangeOverride: " + ToString());
            return finalSize;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{2}: {0} with {1} elements.", Name, Children.Count, GetType().Name);
        }

    }
}
