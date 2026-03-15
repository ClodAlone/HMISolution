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
    public class IkeywordCollection : CollectionBase
    {
        public IkeywordCollection()
        {
        }

        public IKeyword this[int index]
        {
            get
            {
                return (IKeyword)base.List[index];
            }

            set
            {
                base.List[index] = value;
            }
        }

        public int Add(IKeyword ikeyword)
        {
            return base.List.Add(ikeyword);
        }

        public void Insert(int index, IKeyword ikeyword)
        {
            base.List.Insert(index, ikeyword);
        }

        public void Remove(IKeyword ikeyword)
        {
            base.List.Remove(ikeyword);
        }
    }
}
