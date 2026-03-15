/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam.Model;

namespace UpdateDemo {
  public partial class UpdateDemo : UserControl {
    public UpdateDemo() {
      InitializeComponent();

      // create a couple of nodes initially connected by a link
      var nodes = new ObservableCollection<TestData>() {
        new TestData() { Key="N1", Location=new Point(0, 0) },
        new TestData() { Key="N2", Location=new Point(100, 100) }
      };
      var links = new ObservableCollection<TestLink>() {
        new TestLink() { From="N1", To="N2", Text="N1-N2" }
      };
      var model = new TestModel();
      model.NodesSource = nodes;
      model.LinksSource = links;
      model.Modifiable = true;
      model.HasUndoManager = true;
      model.Changed += model_Changed;  // update the log and TreeView

      // a background double-click creates a new node
      myDiagram.Model = model;
      myDiagram.ClickCreatingTool.DoubleClick = true;
      myDiagram.ClickCreatingTool.PrototypeData = new TestData() { Key="I1" };

      // setup up the second Diagram that shares the single Model
      myDiagram2.Model = model;
      myDiagram2.ClickCreatingTool.DoubleClick = true;
      myDiagram2.ClickCreatingTool.PrototypeData = new TestData() { Key="I1" };
    }

    void model_Changed(object sender, ModelChangedEventArgs e) {
      // add an entry to the log
      if (e.Change >= 0) {
        if (ShowChanges.IsChecked == true) {
          myLog.Text += e.ToString() + Environment.NewLine;
          myScroller.ScrollToVerticalOffset(9e9);
        }
      } else {
        if (ShowLayouts.IsChecked == true ||
            (!"Layout".Equals(e.Data) && !"DelayedRouting".Equals(e.Data))) {
          myLog.Text += e.ToString() + Environment.NewLine;
          myScroller.ScrollToVerticalOffset(9e9);
        }
      }
      // add an item to the tree view showing the UndoManager state
      if (e.Change == ModelChange.CommittedTransaction) {
        UndoManager.CompoundEdit cedit = e.OldValue as UndoManager.CompoundEdit;
        if (cedit != null) {
          if (this.EditToRedo != null) {
            // remove from TreeView all transactions starting with this.EditToRedo
            ItemCollection coll = myTreeView.Items;
            int idx = coll.IndexOf(this.EditToRedo);
            if (idx >= 0) {
              while (coll.Count > idx) coll.RemoveAt(idx);
            }
            this.EditToRedo = null;
          }
          // add a TreeViewItem representing the completed transaction
          TreeViewItem citem = new TreeViewItem();
          citem.Tag = cedit;
          citem.Header = cedit.Name;
          // all of the changes within the transaction are tree view children 
          foreach (IUndoableEdit edit in cedit.Edits) {
            TreeViewItem eitem = new TreeViewItem();
            eitem.Tag = edit;
            eitem.Header = edit.ToString();
            citem.Items.Add(eitem);
          }
          myTreeView.Items.Add(citem);
#if !SILVERLIGHT
          citem.BringIntoView();
#endif
        }
      } else if (e.Change == ModelChange.FinishedUndo || e.Change == ModelChange.FinishedRedo) {
        // unselect any currently selected transaction
        if (this.EditToRedo != null) {
          this.EditToRedo.IsSelected = false;
          this.EditToRedo = null;
        }
        IUndoableEdit nextedit = myDiagram.Model.UndoManager.EditToRedo;
        if (nextedit != null) {
          // find the next edit to redo, and select it
          foreach (TreeViewItem item in myTreeView.Items) {
            if (item.Tag == nextedit) {
              item.IsSelected = true;
#if !SILVERLIGHT
              item.BringIntoView();
#endif
              this.EditToRedo = item;
              break;
            }
          }
        }
      }
    }
    private TreeViewItem EditToRedo { get; set; }

    private void ClearLog(object sender, RoutedEventArgs e) {
      myLog.Text = "";
    }
  }

  public class TestModel : GraphLinksModel<TestData, String, String, TestLink> {
    // initialize each link's text label to a string
    protected override TestLink InsertLink(TestData fromdata, string fromparam, TestData todata, string toparam) {
      TestLink link = new TestLink() { From=fromdata.Key, FromPort=fromparam, To=todata.Key, ToPort=toparam };
      link.Text = link.From + "-" + link.To;
      var links = this.LinksSource as IList<TestLink>;
      if (link != null) links.Add(link);
      return link;
    }
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class TestData : GraphLinksModelNodeData<String> { }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class TestLink : GraphLinksModelLinkData<String, String> { }
}
