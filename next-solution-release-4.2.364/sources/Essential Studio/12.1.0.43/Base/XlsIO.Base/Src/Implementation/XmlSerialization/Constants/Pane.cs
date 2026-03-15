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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// This class contains constants used for pane parsing/serialization.
  /// </summary>
  sealed class Pane
  {
    /// <summary>
    /// Represents worksheet view pane.
    /// </summary>
    public const string TagName = "pane";
    /// <summary>
    /// Horizontal position of the split, in 1/20th of a point; 0 (zero) if none.
    /// If the pane is frozen, this value indicates the number of columns visible
    /// in the top pane.
    /// </summary>
    public const string XSplit = "xSplit";
    /// <summary>
    /// Vertical position of the split, in 1/20th of a point; 0 (zero) if none.
    /// If the pane is frozen, this value indicates the number of rows visible
    /// in the left pane.
    /// </summary>
    public const string YSplit = "ySplit";
    /// <summary>
    /// Location of the top left visible cell in the bottom right pane (when in Left-To-Right mode).
    /// </summary>
    public const string TopLeftCell = "topLeftCell";
    /// <summary>
    /// The pane that is active.
    /// </summary>
    public const string Active = "activePane";
    /// <summary>
    /// Indicates whether the pane has horizontal / vertical splits,
    /// and whether those splits are frozen.
    /// </summary>
    public const string State = "state";
    /// <summary>
    /// Panes are frozen, but were not split being frozen. In this state, when the
    /// panes are unfrozen again, a single pane results, with no split.
    /// </summary>
    public const string StateFrozen = "frozen";
    /// <summary>
    /// Panes are frozen and were split before being frozen. In this state, when
    /// the panes are unfrozen again, the split remains, but is adjustable.
    /// </summary>
    public const string StateFrozenSplit = "frozenSplit";
    /// <summary>
    /// Panes are split, but not frozen. In this state, the split bars are adjustable by the user.
    /// </summary>
    public const string StateSplit = "split";
    /// <summary>
    /// Selected or active cell information
    /// </summary>
    public const string Selection = "selection";
    /// <summary>
    /// Active cell in the worksheet
    /// </summary>
    public const string ActiveCell = "activeCell";
    /// <summary>
    /// Sequence of References
    /// </summary>
    public const string Sqref = "sqref";

    /// <summary>
    /// Possible values for active pane.
    /// </summary>
    public enum ActivePane
    {
      /// <summary>
      /// Bottom left pane, when both vertical and horizontal splits are applied.
      /// This value is also used when only a horizontal split has been applied,
      /// dividing the pane into upper and lower regions. In that case, this value
      /// specifies the bottom pane.
      /// </summary>
      bottomLeft = 2,
      /// <summary>
      /// Bottom right pane, when both vertical and horizontal splits are applied.
      /// </summary>
      bottomRight = 0,
      /// <summary>
      /// Top left pane, when both vertical and horizontal splits are applied.
      /// This value is also used when only a horizontal split has been applied,
      /// dividing the pane into upper and lower regions. In that case, this value
      /// specifies the top pane. This value is also used when only a vertical split
      /// has been applied, dividing the pane into right and left regions. In that
      /// case, this value specifies the left pane.
      /// </summary>
      topLeft = 3,
      /// <summary>
      /// Top right pane, when both vertical and horizontal splits are applied.
      /// This value is also used when only a vertical split has been applied,
      /// dividing the pane into right and left regions. In that case, this value
      /// specifies the right pane.
      /// </summary>
      topRight = 1,
    }

    public static readonly Dictionary<string, ActivePane> PaneStrings;

    static Pane()
    {
      PaneStrings = new Dictionary<string, ActivePane>();
      PaneStrings.Add( "bottomLeft", ActivePane.bottomLeft );
      PaneStrings.Add( "bottomRight", ActivePane.bottomRight );
      PaneStrings.Add( "topLeft", ActivePane.topLeft );
      PaneStrings.Add( "topRight", ActivePane.topRight );
    }
  }
}
