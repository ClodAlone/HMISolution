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

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// Contains constants that defines all known sector types.
    /// </summary>
    sealed class SectorTypes
    {
        public const int FreeSector = -1;
        public const int EndOfChain = -2;
        public const int FatSector = -3;
        public const int DifSector = -4;
    }
}
