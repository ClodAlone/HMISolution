/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Arrowheads {
  public partial class Arrowheads : UserControl {
    public Arrowheads() {
      InitializeComponent();

      // Initialize a new GraphLinksModel
      var model = new GraphLinksModel<MyNodeData, int, string, MyLinkData>();
      model.NodeKeyPath = "Key";
      model.LinkLabelNodePath = "";
      var nodesSource = new ObservableCollection<MyNodeData> { };
      model.NodesSource = nodesSource;
      model.LinksSource = new ObservableCollection<MyLinkData> { };
      model.Modifiable = true;  // enables AddNode and AddLink, below

      // Create the big node in the center
      CenterNode = new MyNodeData() {
        Key=-1,
        Diameter = CenterNodeDiameter,
        Text="Click to\rPause!"
      };
      CenterNode.Location = new Point(0,0);
      model.AddNode(CenterNode);

      // Create outer nodes and links to them from the center node,
      // for each pair of Arrowhead enum values.
      int i = 1;
      while (Enum.IsDefined(typeof(Arrowhead), i)) {
        MyNodeData mnd = new MyNodeData() {
          Key = (i/2),
          Diameter = OuterNodeDiameter
        };
        model.AddNode(mnd);
        Arrowhead fromarrow = Arrowhead.None;
        if (Enum.IsDefined(typeof(Arrowhead), i+1)) fromarrow = (Arrowhead)(i+1);
        model.AddLink(new MyLinkData() {
          ToArrow = (Arrowhead)i,
          FromArrow = fromarrow,
          Text = ((Arrowhead)i).ToString(),
          From = CenterNode.Key,
          To = mnd.Key
        });
        i+=2;
      }

      // The expression (NumberOfNodes - 1) is often used to ignore the center node.
      NumberOfNodes = nodesSource.Count;
      // Calculates the SpokeLength so that the distance between the centers
      // of consecutive nodes is equal to RimSegmentLength.
      SpokeLength = (RimSegmentLength+OuterNodeDiameter)
                    * Math.Sin(0.5*(Math.PI-2*Math.PI/(NumberOfNodes-1)))
                    / Math.Sin(2*Math.PI/(NumberOfNodes-1));
      // Doesn't allow SpokeLength to be too close to the centerNode's edge.
      double minSpokeLength = (0.75*CenterNodeDiameter);
      if (SpokeLength < minSpokeLength) SpokeLength = minSpokeLength;

      // Assign the node Locations.
      double currentAngle = 0.0, currentX = 0.0, currentY = 0.0;
      for (int j = 1; j < NumberOfNodes; j++) {
        currentX = SpokeLength*Math.Cos(currentAngle);
        currentY = SpokeLength*Math.Sin(currentAngle);
        MyNodeData node = nodesSource[j];
        node.Location = new Point(currentX, currentY);
        currentAngle = (j)*(2*Math.PI/(NumberOfNodes-1));
      }

      myDiagram.Model = model;
    }

    private const double OuterNodeDiameter = 25;
    private const double CenterNodeDiameter = 200;
    // The shortest length between the edges of two consecutive nodes in the circle:
    private const double RimSegmentLength = 10;

    private Storyboard LocationStoryBoard;
    // Needed for pausing and resuming the animation.
    private bool IsAnimationRunning = true;
    private double SpokeLength;
    private int NumberOfNodes;
    private MyNodeData CenterNode;

    private void myDiagram_Loaded(object sender, RoutedEventArgs e) {
      LocationStoryBoard = new Storyboard();
      // The time for each complete revolution is equal to one second for each node in the outer circle.
      Duration duration = new Duration(TimeSpan.FromSeconds(NumberOfNodes-1));
      LocationStoryBoard.Duration = duration;
      // All of the animations repeat continuously.
      LocationStoryBoard.RepeatBehavior = RepeatBehavior.Forever;

      foreach (Node n in myDiagram.Nodes) {
        MyNodeData mnd = n.Data as MyNodeData;
        if (mnd.Key < 0) continue;
        // The NodePanel should be the first visual child of a Node.
        try {
          AnimateNode(mnd.Key, n.VisualElement as NodePanel, duration);
        } catch (Exception) { }
      }

      LocationStoryBoard.Begin();
      myDiagram.MouseLeftButtonUp += new MouseButtonEventHandler(myDiagram_PauseResumeAnimation);
    }

    // Handles pausing and resuming the Storyboard once Storyboard.Begin() has been called.
    private void myDiagram_PauseResumeAnimation(object sender, MouseButtonEventArgs e) {
      if (IsAnimationRunning) {
        LocationStoryBoard.Pause();
        IsAnimationRunning = false;
        CenterNode.Text = "Click to\rResume!";
      } else {
        LocationStoryBoard.Resume();
        IsAnimationRunning = true;
        CenterNode.Text = "Click to\rPause!";
      }
    }

    // Adds individual animations to the locationAnimation StoryBoard.
    private void AnimateNode(int key, NodePanel np, Duration duration) {
      if (key < 0) return;
      if (np == null) return;

      PointAnimationUsingKeyFrames locationAnimation = new PointAnimationUsingKeyFrames();
      double keyTime = 0.0;
      // Gets the LinearPointKeyFrames for the PointAnimationUsingKeyFrames.
      // Starts at key-1 to account for an initial KeyFrame.
      for (int i = key - 1; i < (key+NumberOfNodes - 1); i++) {
        LinearPointKeyFrame lpkf = new LinearPointKeyFrame();
        lpkf.KeyTime = TimeSpan.FromSeconds(keyTime);

        int nextNodeKeyInt = i+1;
        if (nextNodeKeyInt >= (NumberOfNodes-1))
          nextNodeKeyInt = nextNodeKeyInt % (NumberOfNodes-1);
        MyNodeData nextNodeData = myDiagram.Model.FindNodeByKey((nextNodeKeyInt)) as MyNodeData;
        if (nextNodeData == null || nextNodeData.Location == null) return;
        lpkf.Value = nextNodeData.Location;
        locationAnimation.KeyFrames.Add(lpkf);

        // If duration equals TimeSpan.FromSeconds(NumberOfNodes - 1),
        // then keyTime increases by 1 with each iteration.
        keyTime += ((double)duration.TimeSpan.TotalSeconds) / (NumberOfNodes-1);
      }

      LocationStoryBoard.Children.Add(locationAnimation);
      Storyboard.SetTarget(locationAnimation, np);
      Storyboard.SetTargetProperty(locationAnimation, new PropertyPath(Node.LocationProperty));
    }

    // Find the link data of the Link coming into this Node
    // and update the text fields about the arrowhead names.
    private void NodePanel_MouseEnter(object sender, MouseEventArgs e) {
      NodePanel np = sender as NodePanel;
      if (np == null) return;

      Node node = VisualTreeHelper.GetParent(np) as Node;
      if (node == null) return;

      Link link = node.LinksInto.FirstOrDefault();
      if (link == null) return;

      MyLinkData mld = link.Data as MyLinkData;
      if (mld == null) return;

      ToArrowheadName.Text = "ToArrow = " + mld.ToArrow.ToString();
      FromArrowheadName.Text = "FromArrow = " + mld.FromArrow.ToString();
    }

  }


  // Remember the node's diameter, which is assumed to be unchanging.
  public class MyNodeData : GraphLinksModelNodeData<int> {
    public double Diameter { get; set; }
  }

  // Data-bind each link's arrowhead shape to the corresponding property on the link data.
  public class MyLinkData : GraphLinksModelLinkData<int, string> {
    public Arrowhead ToArrow { get; set; }
    public Arrowhead FromArrow { get; set; }
  }


  // Since the StreamGeometry path markup syntax is used for Arrowheads,
  // nothing is known about its shape (except its bounds).
  // Therefore, a converter is needed to convert between a value of
  // the Arrowhead enum and a brush for the Path's Fill property.
  public class ArrowheadFillConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      return LinkPanel.IsFilled((Arrowhead)value) ? this.Fill : null;
    }

    public Brush Fill { get; set; }
  }
}