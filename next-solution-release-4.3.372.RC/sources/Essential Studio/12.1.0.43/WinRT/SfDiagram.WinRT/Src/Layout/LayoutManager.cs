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
using Syncfusion.UI.Xaml.Diagram.Layout.Base;

namespace Syncfusion.UI.Xaml.Diagram.Layout
{
    public class LayoutManager
    {
        private LayoutBase _mLayoutBase;
        private IGraphInternal _mGraph;
        internal IGraphInternal Graph
        {
            get { return _mGraph; }
            set
            {
                _mGraph = value;
                if (Layout != null)
                {
                    (Layout as IInternalLayout).Graph = value;
                }
            }
        }

        public LayoutBase Layout
        {
            get { return _mLayoutBase; }
            set
            {
                var oldValue = _mLayoutBase; 
                _mLayoutBase = value;
                OnLayoutChanged(oldValue, _mLayoutBase);
            }
        }

        protected virtual void OnLayoutChanged(LayoutBase oldValue, LayoutBase newValue)
        {
            if(oldValue !=null)
            {
                (oldValue as IInternalLayout).Graph = null;
            }
            if (newValue != null)
            {
                (newValue as IInternalLayout).Graph = Graph;
            }
        }
    }
}