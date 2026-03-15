/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace LocalExpand {
  public partial class LocalExpand : UserControl {
    public LocalExpand() {
      InitializeComponent();

      var nodes = new ObservableCollection<NodeData>() {
        new NodeData() { Key="Group 1", IsSubGraph=true },
        new NodeData() { Key="Group 1a", IsSubGraph=true, SubGraphKey="Group 1" },
        new NodeData() { Key="Group 2", IsSubGraph=true },
        new NodeData() { Key="Group 3", IsSubGraph=true },
        new NodeData() { Key="Group 4", IsSubGraph=true },
        new NodeData() { Key="Group 5", IsSubGraph=true },
        new NodeData() { Key="1", SubGraphKey="Group 1a" },
        new NodeData() { Key="2", SubGraphKey="Group 1a" },
        new NodeData() { Key="3", SubGraphKey="Group 1a" },
        new NodeData() { Key="4", SubGraphKey="Group 2" },
        new NodeData() { Key="5", SubGraphKey="Group 2" },
        new NodeData() { Key="6", SubGraphKey="Group 2" },
        new NodeData() { Key="7", SubGraphKey="Group 1" },
        new NodeData() { Key="8", SubGraphKey="Group 3" },
        new NodeData() { Key="9", SubGraphKey="Group 3" },
        new NodeData() { Key="10", SubGraphKey="Group 3" },
        new NodeData() { Key="11", SubGraphKey="Group 4" },
        new NodeData() { Key="12", SubGraphKey="Group 4" },
        new NodeData() { Key="13", SubGraphKey="Group 4" },
        new NodeData() { Key="14", SubGraphKey="Group 4" },
        new NodeData() { Key="15", SubGraphKey="Group 5" },
        new NodeData() { Key="16", SubGraphKey="Group 5" },
      };

      var links = new ObservableCollection<LinkData>() {
        new LinkData() { From="1", To="2" },
        new LinkData() { From="1", To="3" },
        new LinkData() { From="2", To="3" },
        new LinkData() { From="4", To="5" },
        new LinkData() { From="4", To="6" },
        new LinkData() { From="5", To="15" },
        new LinkData() { From="6", To="5" },
        new LinkData() { From="6", To="8" },
        new LinkData() { From="7", To="10" },
        new LinkData() { From="7", To="Group 1a" },
        new LinkData() { From="8", To="10" },
        new LinkData() { From="9", To="8" },
        new LinkData() { From="10", To="9" },
        new LinkData() { From="10", To="Group 4" },
        new LinkData() { From="11", To="12" },
        new LinkData() { From="12", To="13" },
        new LinkData() { From="13", To="14" },
        new LinkData() { From="14", To="11" },
        new LinkData() { From="15", To="16" },
        new LinkData() { From="Group 1a", To="Group 2" },
      };

      var model = new GraphLinksModel<NodeData, String, String, LinkData>();
      model.NodesSource = nodes;
      model.LinksSource = links;
      model.Modifiable = true;
      myDiagram.Model = model;

      myDiagram.InitialLayoutCompleted += (s, e) => {
        myDiagram.DefaultTool = new LocallyExpandingToolManager();
      };
    }
  }


  public class LocallyExpandingToolManager : ToolManager {
    public LocallyExpandingToolManager() {
      this.Distance = 150;
    }

    public double Distance { get; set; }

    // all Groups are expanded, unless when the mouse is near a Group
    public override void DoMouseMove() {
      if (AllGroups == null) {
        AllGroups = new HashSet<Group>(this.Diagram.Nodes.OfType<Group>());
      }
      Point p = this.Diagram.LastMousePointInModel;
      double dist = this.Distance;
      Rect area = new Rect(p.X-dist, p.Y-dist, dist*2, dist*2);
      var neargroups = new HashSet<Group>(this.Diagram.Panel.FindPartsIn<Group>(area, SearchFlags.Group, SearchInclusion.Intersects, SearchLayers.Nodes));
      foreach (Group g in AllGroups) {
        g.IsExpandedSubGraph = neargroups.Contains(g);
      }
      base.DoMouseMove();
    }

    private HashSet<Group> AllGroups;  // if the set of Groups changes, this needs to be recomputed
  }


  public class NodeData : GraphLinksModelNodeData<String> { }

  public class LinkData : GraphLinksModelLinkData<String, String> { }
}
