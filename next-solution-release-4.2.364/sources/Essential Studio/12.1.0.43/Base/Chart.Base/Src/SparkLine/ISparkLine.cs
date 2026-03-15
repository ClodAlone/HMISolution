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
using System.Drawing;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart
{
   public interface ISparkLine
    {
      
     double GetHighPoint();
       
     double GetLowPoint();
       
     double GetStartPoint();
        
     double GetEndPoint();

     double[] GetNegativePoint();

     object Source { get; set; }

     double StartPoint{ get; set; }

     double EndPoint{ get; set; }

     double LowPoint{ get; set; }

     double HighPoint { get; set; }

     int ControlWidth { get; }

     int ControlHeight { get; }

     Markers Markers { get; }
     Line LineStyle { get; }
     Column ColumnStyle { get; }
     double[] NegativeItem { get; set;}




    }
}
