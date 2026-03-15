// <copyright file="TemplatedAdornerInternalControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Control that have it's DefaultStyleKey initialized with the type of adorner.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TemplatedAdornerInternalControl : AutoTemplatedControl
    {
        #region Class Private Members
        /// <summary>
        /// Stores reference to the adorner.
        /// </summary>
        private readonly TemplatedAdornerBase m_adorner;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets adorned element.
        /// </summary>
        public UIElement AdornedElement
        {
            get
            {
                return m_adorner.AdornedElement;
            }
        }

        /// <summary>
        /// Gets adorner the control is inside of.
        /// </summary>
        public TemplatedAdornerBase Adorner
        {
            get
            {
                return m_adorner;
            }
        }
        #endregion

        #region Class Initialization
        /// <summary>
        /// Initializes a new instance of the TemplatedAdornerInternalControl class with DefaultStyleKey property. Type is adorner.
        /// </summary>
        /// <param name="adorner">Given adorner.</param>
        public TemplatedAdornerInternalControl(TemplatedAdornerBase adorner)
            : base(adorner.GetType())
        {
            if (adorner != null)
            {
                m_adorner = adorner;
            }
            else
            {
                throw new ArgumentNullException("adorner");
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Gets the template child control.
        /// </summary>
        /// <param name="childName"> Child name</param>
        /// <returns>Return correspondent child</returns>
        public DependencyObject GetTemplateChildInternal(string childName)
        {
            return this.GetTemplateChild(childName);
        }
        #endregion
    }
}