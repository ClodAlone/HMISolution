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
using System.Collections;

namespace Syncfusion.Olap.MDXQueryParser
{   
    public class MemberCollection : CollectionBase
    {
        public MemberCollection()
        {
        }

        public IMember this[int index]
        {
            get
            {
                return (IMember)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        public int Add(IMember imember)
        {
            return base.List.Add(imember);
        }

        public void Insert(int index, IMember imember)
        {
            base.List.Insert(index, imember);
        }

        public void Remove(IMember imember)
        {
            base.List.Remove(imember);
        }
    }
}
