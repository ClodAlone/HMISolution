#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Defines a collection of Pdf3DNode objects. 
    /// </summary>
    public class Pdf3DNodeCollection : CollectionBase
    {
        #region Methods
        public int Add(Pdf3DNode value)
        {
            return base.List.Add(value);
        }

        public bool Contains(Pdf3DNode value)
        {
            return base.List.Contains(value);
        }

        public int IndexOf(Pdf3DNode value)
        {
            return base.List.IndexOf(value);
        }

        public void Insert(int index, Pdf3DNode value)
        {
            base.List.Insert(index, value);
        }

        protected override void OnInsert(int index, object value)
        {
        }

        protected override void OnRemove(int index, object value)
        {
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
        }

        protected override void OnValidate(object value)
        {
            if (value.GetType() != typeof(Pdf3DNode))
            {
                throw new ArgumentException("value must be of type Pdf3DNode.", "value");
            }
        }

        public void Remove(Pdf3DNode value)
        {
            base.List.Remove(value);
        }

        #endregion

        #region Properties

        public Pdf3DNode this[int index]
        {
            get
            {
                return (Pdf3DNode)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        #endregion
    }
}
