#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Dynamic;
using Syncfusion.Linq;

namespace Syncfusion.Windows.Data
{
    public class DynamicPropertiesProvider : IPropertyAccessProvider, IDisposable
    {
        ICollectionViewAdv view;
        internal DynamicHelper dynamicHelper;

        public DynamicPropertiesProvider(ICollectionViewAdv view)
        {
            this.view = view;
            this.dynamicHelper = new DynamicHelper();
        }

        public virtual object GetValue(object record, string propName)
        {
            var result = this.dynamicHelper.GetValue(record, propName);
            return result;
        }

        public virtual bool SetValue(object record, string propName, object value)
        {
            var result = true;
            try
            {
                this.dynamicHelper.SetValue(record, propName, value);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public void Dispose()
        {
            this.dynamicHelper.Dispose();
        }
    }
}
