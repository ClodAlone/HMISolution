#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains utility methods to add and remove elements inside a panel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ClassReference(IsReviewed = false)]
    public class UIElementsRecycler<T> : IEnumerable<T> where T : FrameworkElement
    {
        #region fields

        internal List<T> generatedElements {get; set;}

        Panel panel;

        private Dictionary<DependencyProperty, Binding> bindingsProvider;

        #endregion

        #region properties

        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Get the value of CLR property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int Count
        {
            get
            {
                return generatedElements.Count;
            }
        }

        /// <summary>
        /// Provides binding objects to be attached with the generated FrameworkElement
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Dictionary<DependencyProperty, Binding> BindingProvider
        {
            get
            {
                return bindingsProvider;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UIElementsRecycler(Panel panel)
        {
            generatedElements = new List<T>();
            bindingsProvider = new Dictionary<DependencyProperty, Binding>();
            this.panel = panel;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public UIElementsRecycler()
        {
            generatedElements = new List<T>();
            bindingsProvider = new Dictionary<DependencyProperty, Binding>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Generates or recycles the elements
        /// </summary>
        /// <param name="count">Number of elements to be generated</param>
        [ClassReference(IsReviewed = false)]
        public void GenerateElements(int count)
        {
            T element;
            if (count > generatedElements.Count)
            {
                count = count - generatedElements.Count;
                for (int i = 0; i < count; i++)
                {
                    element = Activator.CreateInstance<T>();

                    foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
                    {
                        element.SetBinding(bindings.Key, bindings.Value);
                    }

                    generatedElements.Add(element);
                    if (panel != null)
                        panel.Children.Add(element);
                }
            }
            else if (count < generatedElements.Count)
            {
                count = generatedElements.Count - count;

                for (int i = 0; i < count; i++)
                {
                    element = generatedElements.ElementAt(0);
                    generatedElements.Remove(element);
                    if (panel != null && this.panel.Children.Contains(element))
                    {
                        this.panel.Children.Remove(element);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        public void Add(T element)
        {
            foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
            {
                element.SetBinding(bindings.Key, bindings.Value);
            }

            if (panel != null && !this.panel.Children.Contains(element))
            {
                generatedElements.Add(element);
                panel.Children.Add(element);
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public int IndexOf(T element)
        {
            return generatedElements.IndexOf(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        public void Remove(T element)
        {
            if (panel !=null && this.panel.Children.Contains(element))
            {
                generatedElements.Remove(element);
                panel.Children.Remove(element);
            }
        }

        /// <summary>
        /// Creates a new instance of the specified type
        /// </summary>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public T CreateNewInstance()
        {
            T element = Activator.CreateInstance<T>();

            foreach (KeyValuePair<DependencyProperty, Binding> bindings in bindingsProvider)
            {
                element.SetBinding(bindings.Key, bindings.Value);
            }

            generatedElements.Add(element);
            if (panel != null)
                panel.Children.Add(element);

            return element;
        }

        /// <summary>
        /// Removes the particular binding from the generated elements
        /// </summary>
        /// <param name="property"></param>
        [ClassReference(IsReviewed = false)]
        public void RemoveBinding(DependencyProperty property)
        {
            BindingProvider.Remove(property);
            foreach (T element in generatedElements)
            {
#if WPF
                BindingOperations.ClearBinding(element, property);
#else
                element.SetBinding(property, null);
#endif
            }
        }

        /// <summary>
        /// Clears the generated elements
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void Clear()
        {
            if (panel != null)
            {
                foreach (T element in generatedElements)
                {

                    if (this.panel.Children.Contains(element))
                    {
                        this.panel.Children.Remove(element);
                    }
                }
            }

            generatedElements.Clear();
        }

        /// <summary>
        /// Return the panel's child at the corresponding index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public T this[int index]
        {
            get
            {
                return generatedElements.Count > index ? generatedElements[index] : null;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.generatedElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.generatedElements.GetEnumerator();
        }

        #endregion
    }
}
