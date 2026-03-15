#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.ProjIO
{
    /// <summary>
    /// Abstract class for defining Baselines
    /// </summary>
    public abstract class Baseline
    {
        public abstract string Number { get; set; }
        public abstract string Work { get; set; }
        public abstract float Cost { get; set; }
        public abstract float BCWS { get; set; }
        public abstract float BCWP { get; set; }
    }
}
