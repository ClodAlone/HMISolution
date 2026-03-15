#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    public interface BlockImageDataSource : ImageData
    {
        int getFixedPoint(int c);
        DataBlock getInternCompData(DataBlock blk, int c);
        DataBlock getCompData(DataBlock blk, int c);
    }
}