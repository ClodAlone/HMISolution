#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;

namespace Syncfusion.XlsIO
{
    #if SyncfusionFramework4_0 || SyncfusionFramework4_5
    /// <summary>
    /// Represents the XlsIO Chart to image converter.
    /// </summary>
    public interface IChartToImageConverter
    {
        /// <summary>
        /// Converts the XlsIO chart to Image as stream.
        /// </summary>
        /// <param name="chart">Represents the XlsIO Chart object.</param>
        /// <param name="imageAsStream">In where the image is streamed.</param>
        void SaveAsImage(IChart chart, Stream imageAsStream);
        /// <summary>
        /// It represents the chart image Scaling.
        /// </summary>
        ScalingMode ScalingMode { get; set; }   
    }
#endif
}
