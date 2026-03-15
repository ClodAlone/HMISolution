/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Northwoods.GoXam;

namespace NodeFigures {
  public partial class NodeFigures : UserControl {
    public NodeFigures() {
      InitializeComponent();
#if SILVERLIGHT  // no Enum.GetValues
      List<NodeFigure> figs = new List<NodeFigure>();
      int i = 0;
      while (Enum.IsDefined(typeof(NodeFigure), i)) {
        figs.Add((NodeFigure)i);
        i++;
      }
      myDiagram.Model.NodesSource = figs;
#else
      myDiagram.Model.NodesSource = Enum.GetValues(typeof(NodeFigure));
#endif
      myDiagram.Model.Modifiable = false;
    }
  }
}
