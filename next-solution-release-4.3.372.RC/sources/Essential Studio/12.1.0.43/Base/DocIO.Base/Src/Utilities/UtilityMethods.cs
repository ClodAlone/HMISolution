#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Syncfusion.DocIO
{
    /// <summary>
    /// Represents the Utility methods
    /// </summary>
    internal static class UtilityMethods
    {
        /// <summary>
        /// Clones a stream object
        /// </summary>
        /// <param name="input">Stream</param>
        /// <returns></returns>
        internal static Stream CloneStream(Stream input)
        {
            Stream output = new MemoryStream();
            using (StreamReader reader = new StreamReader(input))
            using (StreamWriter writer = new StreamWriter(output))
            {
                writer.Write(reader.ReadToEnd());
            }
            input.Position = 0;
            output.Position = 0;
            return output;
        }
    }
}
#endif
