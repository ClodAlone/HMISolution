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

namespace Syncfusion.Pdf
{
    public abstract class JBIG2BaseFlags
    {
        protected internal int flagsAsInt;
        protected internal IDictionary flags = new Dictionary<string, int>();
        public int GetFlagValue(string key)
        {
            int? value = (int?)flags[key];
            return (int)value;
        }
        public abstract void setFlags(int flagsAsInt);
    }
}
