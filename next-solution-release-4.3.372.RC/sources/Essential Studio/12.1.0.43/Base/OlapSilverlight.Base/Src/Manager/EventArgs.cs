#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.OlapSilverlight.Manager
{
    /// <summary>
    /// Represent the event argument class for CellSet Changed event.
    /// </summary>
    public class CellSetChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new cell set.
        /// </summary>
        /// <value>The new cell set.</value>
        public CellSet NewCellSet { get; set; }
        /// <summary>
        /// Gets or sets the result set.
        /// </summary>
        /// <value>The result set.</value>
        public object ResultSet { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for CCellSet Changing event.
    /// </summary>
    public class CellSetChangingEventArgs : EventArgs
    {   }

    /// <summary>
    /// Represent the event argument class for CubeInfo Collection Changed event.
    /// </summary>
    public class CubeInfoCollectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new cubes.
        /// </summary>
        /// <value>The new cubes.</value>
        public CubeInfoCollection NewCubes { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Cube Schema Changed event.
    /// </summary>
    public class CubeSchemaChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new cube schema.
        /// </summary>
        /// <value>The new cube schema.</value>
        public CubeSchema NewCubeSchema { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Cube Changed event.
    /// </summary>
    public class CubeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new name of the cube.
        /// </summary>
        /// <value>The new name of the cube.</value>
        public string NewCubeName { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Report Changed event. 
    /// </summary>
    public class ReportChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new report.
        /// </summary>
        /// <value>The new report.</value>
        public OlapReport NewReport { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Axis Element Changed event.
    /// </summary>
    public class AxisElementChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new position.
        /// </summary>
        /// <value>The new position.</value>
        public AxisPosition NewPosition { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Level members obtained Changed event.
    /// </summary>
    public class LevelMembersObtainedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the level members.
        /// </summary>
        /// <value>The level members.</value>
        public MemberCollection LevelMembers { get; set; }
    }

    /// <summary>
    /// Represent the event argument class for Child members obtained Changed event.
    /// </summary>
    public class ChildMembersObtaindedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the child members.
        /// </summary>
        /// <value>The child members.</value>
        public MemberCollection ChildMembers { get; set; }
    }

    /// <summary>
    /// Event handler for CellSet changed.
    /// </summary>
    public delegate void CellSetChangedEventHandler(object sender, CellSetChangedEventArgs e);
    /// <summary>
    /// Event handler for CubeInfo Collection changed.
    /// </summary>
    public delegate void CubeInfoCollectionChangedEventHandler(object sender, CubeInfoCollectionChangedEventArgs e);
    /// <summary>
    /// Event handler for CubeSchema changed.
    /// </summary>
    public delegate void CubeSchemaChangedEventHandler(object sender, CubeSchemaChangedEventArgs e);
    /// <summary>
    /// Event handler for Cube changed.
    /// </summary>
    public delegate void CubeChangedEventHandler(object sender, CubeChangedEventArgs e);
    /// <summary>
    /// Event handler for Report changed.
    /// </summary>
    public delegate void ReportChangedEventHandler(object sender, ReportChangedEventArgs e);
    /// <summary>
    /// Event handler for AxisElement changed.
    /// </summary>
    public delegate void AxisElementChangedEventHandler(object sender, AxisElementChangedEventArgs e);
    /// <summary>
    /// Event handler for LevelMembers obtained.
    /// </summary>
    public delegate void LevelMembersObtainedHandler(object sender, LevelMembersObtainedEventArgs e);
    /// <summary>
    /// Event handler for ChildMembers obtained.
    /// </summary>
    public delegate void ChildMembersObtainedHandler(object sender, ChildMembersObtaindedEventArgs e);
    /// <summary>
    /// Event handler for CellSet changing.
    /// </summary>
    public delegate void CellSetChangingEventHandler(object sender, CellSetChangingEventArgs e);
    /// <summary>
    /// Event handle for MDX obtained.
    /// </summary>
    public delegate void MdxObtainedEventHandler();
}
