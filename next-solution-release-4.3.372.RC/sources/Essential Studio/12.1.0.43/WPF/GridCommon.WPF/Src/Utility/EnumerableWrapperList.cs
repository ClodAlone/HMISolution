#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.GridCommon
{
    public class EnumarableWrapperList : ArrayList, ITypedList
    {
        PropertyDescriptorCollection pdc;
        IEnumerable enumerable;

        public EnumarableWrapperList(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
            foreach (object item in enumerable)
            {
                if (item != null && pdc == null)
                    pdc = TypeDescriptor.GetProperties(item);
                Add(item);
            }
        }

        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            ITypedList tl = enumerable as ITypedList;
            if (tl != null)
                return tl.GetItemProperties(listAccessors);

            return pdc;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            ITypedList tl = enumerable as ITypedList;
            if (tl != null)
                return tl.GetListName(listAccessors);

            return "";
        }

        #endregion
    }

}
