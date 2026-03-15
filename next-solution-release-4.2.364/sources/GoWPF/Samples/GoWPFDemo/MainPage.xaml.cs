/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;

namespace Demo {
  public partial class MainPage :
#if SILVERLIGHT
    UserControl
#else
    Page
#endif
  {
    public MainPage() {
      InitializeComponent();

      MainPage.Instance = this;
      myVersion.Text = Diagram.VersionName;

      List<TypeInfo> sampletypes = new List<TypeInfo>() {
        new TypeInfo() { Name="Class Hierarchy", Type=typeof(ClassHierarchy.ClassHierarchy) },
        new TypeInfo() { Name="Node Figures", Type=typeof(NodeFigures.NodeFigures) },
        new TypeInfo() { Name="Org Chart Static", Type=typeof(OrgChart.OrgChart) },
        new TypeInfo() { Name="Org Chart Editor", Type=typeof(OrgChartEditor.OrgChartEditor) },
        new TypeInfo() { Name="Org Chart Bus", Type=typeof(OrgChartBus.OrgChartBus) },
        new TypeInfo() { Name="Family Tree", Type=typeof(FamilyTree.FamilyTree) },
        new TypeInfo() { Name="Decision Tree", Type=typeof(DecisionTree.DecisionTree) },
        new TypeInfo() { Name="Double Tree", Type=typeof(DoubleTree.DoubleTree) },
        new TypeInfo() { Name="Fishbone", Type=typeof(Fishbone.Fishbone) },
        new TypeInfo() { Name="Visual Tree", Type=typeof(VisualTree.VisualTree) },
        new TypeInfo() { Name="Local View", Type=typeof(LocalView.LocalView) },
        new TypeInfo() { Name="Local Expand", Type=typeof(LocalExpand.LocalExpand) },
        new TypeInfo() { Name="Update Demo", Type=typeof(UpdateDemo.UpdateDemo) },
        new TypeInfo() { Name="Table", Type=typeof(Table.Table) },
        new TypeInfo() { Name="Friend Wheel", Type=typeof(FriendWheel.FriendWheel) },
        new TypeInfo() { Name="Beat Paths", Type=typeof(BeatPaths.BeatPaths) },
        new TypeInfo() { Name="Pipe Tree", Type=typeof(PipeTree.PipeTree) },
        new TypeInfo() { Name="Genogram", Type=typeof(Genogram.Genogram) },
        new TypeInfo() { Name="Swim Lanes", Type=typeof(SwimLanes.SwimLanes) },
        new TypeInfo() { Name="Planogram", Type=typeof(Planogram.Planogram) },
        new TypeInfo() { Name="Incremental Tree", Type=typeof(IncrementalTree.IncrementalTree) },
        new TypeInfo() { Name="Grouping", Type=typeof(Grouping.Grouping) },
        new TypeInfo() { Name="Navigator", Type=typeof(Navigator.Navigator) },
        new TypeInfo() { Name="Link Demo", Type=typeof(LinkDemo.LinkDemo) },
        new TypeInfo() { Name="Arrowheads", Type=typeof(Arrowheads.Arrowheads) },
        new TypeInfo() { Name="Interactive Force", Type=typeof(InteractiveForce.InteractiveForce) },
        new TypeInfo() { Name="Force Directed", Type=typeof(FDLayout.FDLayout) },
        new TypeInfo() { Name="Layered Digraph", Type=typeof(LDLayout.LDLayout) },
        new TypeInfo() { Name="Tree Layout", Type=typeof(TLayout.TLayout) },
        new TypeInfo() { Name="Circular Layout", Type=typeof(CLayout.CLayout) },
        new TypeInfo() { Name="Grid Layout", Type=typeof(GLayout.GLayout) },
        new TypeInfo() { Name="Serpentine Layout", Type=typeof(Serpentine.Serpentine) },
        new TypeInfo() { Name="State Chart", Type=typeof(StateChart.StateChart) },
        new TypeInfo() { Name="Flow Chart", Type=typeof(FlowChart.FlowChart) },
        new TypeInfo() { Name="Flowgrammer", Type=typeof(FlowGrammer.FlowGrammer) },
        new TypeInfo() { Name="ER Diagram", Type=typeof(EntityRelationship.EntityRelationship) },
        new TypeInfo() { Name="Dynamic Ports", Type=typeof(DynamicPorts.DynamicPorts) },
        new TypeInfo() { Name="MindMap", Type=typeof(MindMap.MindMap) },
        new TypeInfo() { Name="Logic Circuit", Type=typeof(LogicCircuit.LogicCircuit) },
        new TypeInfo() { Name="Piping", Type=typeof(Piping.Piping) },
        new TypeInfo() { Name="Draggable Link", Type=typeof(DraggableLink.DraggableLink) },
        new TypeInfo() { Name="Virtualizing", Type=typeof(Virtualizing.Virtualizing) },
        new TypeInfo() { Name="Gantt", Type=typeof(Gantt.Gantt) },
        new TypeInfo() { Name="PERT", Type=typeof(PERT.PERT) },
        new TypeInfo() { Name="Sequential Function", Type=typeof(SequentialFunction.SequentialFunction) },
        new TypeInfo() { Name="Sequence Diagram", Type=typeof(SequenceDiagram.SequenceDiagram) },
      };
      myListBox.ItemsSource = sampletypes;
      myListBox.SelectedIndex = 0;
    }

    public static MainPage Instance { get; set; }

    public class TypeInfo {
      public String Name { get; set; }
      public Type Type { get; set; }
    }

    private void myListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      TypeInfo typeinfo = e.AddedItems.OfType<TypeInfo>().FirstOrDefault();
      if (typeinfo != null) {
        Type sampletype = typeinfo.Type;
        String typename = sampletype.Name;
        FrameworkElement sample = Activator.CreateInstance(sampletype) as FrameworkElement;
        if (sample != null) {
          mySampleContainer.Content = sample;
          myXamlTextBox.Text = LoadText("source." + typename, "xamltxt");
          myCodeTextBox.Text = LoadText("source." + typename, "xamlcstxt");
          myDataTextBox.Text = LoadText(typename, "xml");
          mySavedTextBox.Text = "";
#if SILVERLIGHT
          NavigateToHtml(typename + ".html");
#else
          NavigateToHtml(typename);
#endif
          // manage the tabs
          SetVisibility(myXamlTabItem, myXamlTextBox.Text);
          SetVisibility(myCodeTabItem, myCodeTextBox.Text);
          SetVisibility(myDataTabItem, myDataTextBox.Text);
          SetVisibility(mySavedTabItem, mySavedTextBox.Text);
        }
      }
    }

    private void SetVisibility(TabItem item, String text) {
      if (text == null || text == "") {
        item.Visibility = Visibility.Collapsed;
        if (item.IsSelected) myXamlTabItem.IsSelected = true;
      } else {
        item.Visibility = Visibility.Visible;
      }
    }

    // instead of using a file, just save the text to memory here
    public String SavedXML {
      get { return mySavedTextBox.Text; }
      set {
        mySavedTextBox.Text = value;
        SetVisibility(mySavedTabItem, mySavedTextBox.Text);
        if (value != "") mySavedTabItem.IsSelected = true;
      }
    }

    public void NavigateToHtml(String To) {
#if SILVERLIGHT
      System.Windows.Browser.HtmlElement element = System.Windows.Browser.HtmlPage.Document.GetElementById("Comments");
      if (element != null) {
        element.SetProperty("src", string.Format("source/{0}", To));
      }
#else
      Stream s = GetStream("source." + To, "html");
      if (s != null) myComment.NavigateToStream(s);
#endif
    }

#if !SILVERLIGHT
    // By using a MainPage instead of a MainWindow for a normal WPF app,
    // to share more implementation between GoWpfDemo and GoXbapDemo,
    // we need to try to set the actual main Window's Title.
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      Window mainwin = Part.FindAncestor<Window>(this);
      if (mainwin != null) {  // might not find a Window
        try {
          mainwin.Title = this.Title;
        } catch (Exception) { }  // might get a permission exception
      }
    }
#endif

    public String LoadText(String name, String extension) {
      try {
        using (Stream stream = GetStream(name, extension)) {
          if (stream == null) return "";
          using (StreamReader reader = new StreamReader(stream)) {
            return reader.ReadToEnd();
          }
        }
      } catch (Exception) {
        return "";
      }
    }

    public Stream GetStream(String name, String extension) {
      try {
        return typeof(MainPage).Assembly.GetManifestResourceStream("Demo." + name + "." + extension);
      } catch (Exception) {
        return null;
      }
    }
  }
}
