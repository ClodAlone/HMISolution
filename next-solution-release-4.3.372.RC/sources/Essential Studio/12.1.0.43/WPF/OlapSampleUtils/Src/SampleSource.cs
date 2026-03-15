#region Copyright

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 

#endregion

namespace SampleUtils
{
    /// <summary>
    /// Sample source.
    /// </summary>
    public class SampleSource
    {
        /// <summary>
        /// Initializes the Sample source <see cref="SampleSource"/> object.
        /// </summary>
        public SampleSource()
        {
            Source = Source.SyncfusionOfflineCube;
            FilePath = string.Empty;
            ServerName = string.Empty;
            DatabaseName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the data base name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Source"/> object.
        /// </summary>
        public Source Source { get; set; }
    }
}