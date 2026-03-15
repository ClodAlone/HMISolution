#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// VisualContainer maintains a collection of child elements. Adding
    /// and removing elements from the children collection does not trigger
    /// calls to InvalidateMeasure. This allows adding and removing elements
    /// on the fly. A derived control is responsible to call Measure and Arrange
    /// on child elements since this base class will not do this by itsself.
    /// </summary>
    public class VisualContainer : FrameworkElement, IDisposable
    {
        VisualCollection children;

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
        public static readonly DependencyProperty WantsMouseInputProperty = DependencyProperty.Register(
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
                DependencyObject parent = GridUtil.GetParent(dpo);
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
            while ((Rect)dpo.GetValue(CellRenderBoundsProperty) == Rect.Empty)
            {
                DependencyObject parent = GridUtil.GetParent(dpo);
                if (parent == null)
                {
                    return (Rect)dpo.GetValue(CellRenderBoundsProperty);
                }
                dpo = parent;
            }

            return (Rect)dpo.GetValue(CellRenderBoundsProperty);
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
             : this()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualContainer"/> class.
        /// </summary>
        public VisualContainer()
        {
            children = new VisualCollection(this);
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
            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
        }

        /// <summary>
        /// Implements <see cref="System.Windows.Media.Visual.HitTestCore(System.Windows.Media.GeometryHitTestParameters)"/> to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.GeometryHitTestResult"/>).
        /// </summary>
        /// <param name="hitTestParameters">Describes the hit test to perform, including the initial hit point.</param>
        /// <returns>
        /// Results of the test, including the evaluated geometry.
        /// </returns>
        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            if (GetWantsMouseInput(this, null) == false)
                return null;

            return base.HitTestCore(hitTestParameters);
        }

        /// <summary>
        /// Implements <see cref="System.Windows.Media.Visual.HitTestCore(System.Windows.Media.PointHitTestParameters)"/> to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.HitTestResult"/>).
        /// </summary>
        /// <param name="hitTestParameters">Describes the hit test to perform, including the initial hit point.</param>
        /// <returns>
        /// Results of the test, including the evaluated point.
        /// </returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (GetWantsMouseInput(this, null) == false)
                return null;

            return base.HitTestCore(hitTestParameters);
        }
        #endregion
        #region Children
        /// <summary>
        /// Gets the collection of visual children. Adding and removing elements through the this 
        /// collection does not trigger calls to InvalidateMeasure. 
        /// </summary>
        /// <value>The children.</value>
        public IList<Visual> Children
        {
            get
            {
                return children;
            }
        }

        // Methods to access child collection.
        void InternalAdd(Visual el)
        {
            AddVisualChild(el);
            AddLogicalChild(el);
            // don't: InvalidateMeasure();
        }

        void InternalRemove(Visual el)
        {
            RemoveVisualChild(el);
            RemoveLogicalChild(el);
            // don't: InvalidateMeasure();
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                if (children == null)
                    return 0;
                return children.Count;
            }
        }

        /// <summary>
        /// Overrides <see cref="System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index >= children.Count)
                throw new ArgumentOutOfRangeException("index");

            return children[index];
        }
        #endregion
        #region VisualCollection
        /// <summary>
        /// A collection of <see cref="Visual"/> objects. Adding and removing elements through the this 
        /// collection does not trigger calls to InvalidateMeasure in the parent container.
        /// </summary>
        public class VisualCollection : IList<Visual> , IDisposable
        {
            List<Visual> inner = new List<Visual>();
            VisualContainer owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="VisualCollection"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            public VisualCollection(VisualContainer owner)
            {
                this.owner = owner;
            }

            #region IList<Visual> Members

            /// <summary>
            /// Determines the index of a specific item in the <see cref="Visual"/>.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="Visual"/>.</param>
            /// <returns>
            /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
            /// </returns>
            public int IndexOf(Visual item)
            {
                return inner.IndexOf(item);
            }

            /// <summary>
            /// Inserts an item to the <see cref="Visual"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="Visual"/>.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="Visual"/>.</exception>
            public void Insert(int index, Visual item)
            {
                inner.Insert(index, item);
                owner.InternalAdd(item);
            }

            /// <summary>
            /// Removes the <see cref="Visual"/> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="Visual"/>.</exception>
            public void RemoveAt(int index)
            {
                owner.InternalRemove(this[index]);
                inner.RemoveAt(index);
            }

            /// <summary>
            /// Gets or sets the <see cref="Visual"/> at the specified index.
            /// </summary>
            /// <value></value>
            public Visual this[int index]
            {
                get
                {
                    return inner[index];
                }
                set
                {
                    if (inner[index] != value)
                    {
                        owner.InternalRemove(inner[index]);
                        inner[index] = value;
                        owner.InternalAdd(value);
                    }
                }
            }

            #endregion

            #region ICollection<Visual> Members

            /// <summary>
            /// Adds an item to the <see cref="VisualCollection"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="VisualCollection"/>.</param>
            public void Add(Visual item)
            {
                inner.Add(item);
                owner.InternalAdd(item);
            }

            /// <summary>
            /// Removes all items from the <see cref="VisualCollection"/>.
            /// </summary>
            public void Clear()
            {
                if (inner != null)
                {
                    foreach (Visual item in inner)
                        owner.InternalRemove(item);
                    inner.Clear();
                }
            }

            /// <summary>
            /// Determines whether the <see cref="VisualCollection"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="VisualCollection"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="VisualCollection"/>; otherwise, false.
            /// </returns>
            public bool Contains(Visual item)
            {
                return inner.Contains(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="VisualCollection"/> to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from <see cref="VisualCollection"/>. The <see cref="System.Array"/> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="System.ArgumentNullException">
            /// 	<paramref name="array"/> is null.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
            /// <exception cref="System.ArgumentException">
            /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="VisualCollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
            public void CopyTo(Visual[] array, int arrayIndex)
            {
                inner.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="VisualCollection"/>.
            /// </summary>
            /// <value></value>
            /// <returns>The number of elements contained in the <see cref="VisualCollection"/>.</returns>
            public int Count
            {
                get {
                    if (inner != null)
                        return inner.Count;
                    else
                        return 0;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="VisualCollection"/> is read-only.
            /// </summary>
            /// <value></value>
            /// <returns>false.</returns>
            public bool IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="VisualCollection"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="VisualCollection"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> was successfully removed from the <see cref="VisualCollection"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="VisualCollection"/>.
            /// </returns>
            public bool Remove(Visual item)
            {
                if (Contains(item))
                {
                    owner.InternalRemove(item);
                    return inner.Remove(item);
                }
                return false;
            }

            #endregion

            #region IEnumerable<Visual> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A enumerator that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<Visual> GetEnumerator()
            {
                return inner.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)inner).GetEnumerator();
            }

            #endregion

            public void Dispose()
            {
                this.inner = null;
                this.owner = null;
            }
        }
        #endregion

        public void Dispose()
        {
            if (this.children != null)
            {
                this.children.Clear();
                this.children = null;
            }
        }
    }
}
