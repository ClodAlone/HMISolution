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


#if WINRT
using System.Threading.Tasks;
#endif
namespace Syncfusion.UI.Xaml.Maps
{
    internal class ShapeFileData
    {
        public ShapeFileData()
        {
            this.Records = new List<ShapeFileRecord>();
        }

        public string FileName
        {
            get;
            private set;
        }

        public ShapeFileHeader FileHeader
        {
            get;
            internal set;
        }

        public List<ShapeFileRecord> Records
        {
            get;
            internal set;
        }
    }
}
