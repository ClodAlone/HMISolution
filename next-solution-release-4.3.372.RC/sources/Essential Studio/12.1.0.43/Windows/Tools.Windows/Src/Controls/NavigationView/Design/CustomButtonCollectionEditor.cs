#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    public class CustomButtonCollectionEditor :
        CollectionEditor
    {
        public CustomButtonCollectionEditor(Type type) :
            base(type)
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            CustomButton btn = (CustomButton)base.CreateInstance(itemType);
            NavigationView nv = this.Context.Instance as NavigationView;

            if (nv != null)
            {
                btn.Height = nv.Height;
            }

            return btn;
        }
    }
}
