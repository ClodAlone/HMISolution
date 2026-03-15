#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    public class JPXImageCoordinates
    {
        public int x;
        public int y;
        public JPXImageCoordinates()
        {
        }
        public JPXImageCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public JPXImageCoordinates(JPXImageCoordinates c)
        {
            this.x = c.x;
            this.y = c.y;
        }
        public override System.String ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }
}