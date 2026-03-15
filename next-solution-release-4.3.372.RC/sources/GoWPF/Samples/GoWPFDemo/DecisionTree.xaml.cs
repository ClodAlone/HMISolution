/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace DecisionTree {
  public partial class DecisionTree : UserControl {
    public DecisionTree() {
      InitializeComponent();

      // Although the graph is just a tree, we don't use a TreeModel because we
      // want to distinguish links coming from different "ports".
      var model = new GraphLinksModel<Info, String, String, UniversalLinkData>();
      model.NodeKeyPath = "Key";
      model.NodeCategoryPath = "Category";  // the DataTemplate name for Node templates

      // Create all of the node data that we might show.
      var nodes = new List<Info>() {
        new Info() { Key = StartName },  // the root node
        
        // intermediate nodes: decisions on personality characteristics
        new Info() { Key = "I" },
        new Info() { Key = "E" },
        
        new Info() { Key = "IN" },
        new Info() { Key = "IS" },
        new Info() { Key = "EN" },
        new Info() { Key = "ES" },

        new Info() { Key = "INT" },
        new Info() { Key = "INF" },
        new Info() { Key = "IST" },
        new Info() { Key = "ISF" },
        new Info() { Key = "ENT" },
        new Info() { Key = "ENF" },
        new Info() { Key = "EST" },
        new Info() { Key = "ESF" },

        // terminal nodes: the personality descriptions
        new Info() { Key = "INTJ", 
          Text="INTJ: Scientist\rThe most self-confident of all types.  They focus on possibilities and use empirical logic to think about the future.  They prefer that events and people serve some positive use.  1% of population.	"},
        new Info() { Key = "INTP", 
          Text="INTP: Architect\rAn architect of ideas, number systems, computer languages, and many other concepts.  They exhibit great precision in thought and language.  1% of the population."},
        new Info() { Key = "INFJ", 
          Text="INFJ: Author\rFocus on possibilities.  Place emphasis on values and come to decisions easily.  They have a strong drive to contribute to the welfare of others.  1% of population."},
        new Info() { Key = "INFP", 
          Text="INFP: Questor\rPresent a calm and pleasant face to the world.  Although they seem reserved, they are actually very idealistic and care passionately about a few special people or a cause.  1% of the population."},
        new Info() { Key = "ISTJ", 
          Text="ISTJ: Trustee\rISTJ's like organized lives. They are dependable and trustworthy, as they dislike chaos and work on a task until completion. They prefer to deal with facts than emotions. 6% of the population."},
        new Info() { Key = "ISTP", 
          Text="ISTP: Artisan\rISTP's are quiet people who are very capable at analyzing how things work. Though quiet, they can be influential, with their seclusion making them all the more skilled. 17% of the population."},
        new Info() { Key = "ISFJ", 
          Text="ISFJ: Conservator\rISFJ's are not particularly social and tend to be most concerned with maintaining order in their lives. They are dutiful, and respectful towards and interested in others, though they are often shy. They are, therefore, trustworthy, but not bossy. 6% of the population."},
        new Info() { Key = "ISFP", 
          Text="ISFP: Author\rFocus on possibilities.  Place emphasis on values and come to decisions easily.  They have a strong drive to contribute to the welfare of others.  1% of population."},
        new Info() { Key = "ENTJ", 
          Text="ENTJ: Fieldmarshal\rThe driving force of this personality is to lead.  They like to impose structure and harness people to work towards distant goals.  They reject inefficiency.  5% of the population."},
        new Info() { Key = "ENTP", 
          Text="ENTP: Inventor\rExercise their ingenuity by dealing with social, physical, and mechanical relationships.  They are always sensitive to future possibilities.  5% of the population."},
        new Info() { Key = "ENFJ", 
          Text="ENFJ: Pedagogue\rExcellent leaders; they are charismatic and never doubt that others will follow them and do as they ask.   They place a high value on cooperation.  5% of the population."},
        new Info() { Key = "ENFP", 
          Text="ENFP: Journalist\rPlace significance in everyday occurrences.  They have great ability to understand the motives of others.  They see life as a great drama.  They have a great impact on others.  5% of the population."},
        new Info() { Key = "ESTJ", 
          Text="ESTJ: Administrator\rESTJ's are pragmatic, and thus well-suited for business or administrative roles. They are traditionalists are conservatives, believing in the status quo. 13% of the population."},
        new Info() { Key = "ESTP", 
          Text="ESTP: Promoter\rESTP's tend to manipulate others in order to attain access to the finer aspects of life. However, they enjoy heading to such places with others. They are social and outgoing and are well-connected. 13% of the population."},
        new Info() { Key = "ESFJ", 
          Text="ESFJ: Seller\rESFJ's tend to be social and concerned for others. They follow tradition and enjoy a structured community environment. Always magnanimous towards others, they expect the same respect and appreciation themselves. 13% of the population."},
        new Info() { Key = "ESFP", 
          Text="ESFP: Entertainer\rThe mantra of the ESFP would be \"Carpe Diem.\" They enjoy life to the fullest. They do not, thus, like routines and long-term goals. In general, they are very concerned with others and tend to always try to help others, often perceiving well their needs. 13% of the population."}
      };

      // Provide the same choice information for all of the nodes on each level.
      // The level is implicit in the number of characters in the Key, except for the root node.
      // In a different application, there might be different choices for each node, so the initialization would be above, where the Info's are created.
      // But for this application, it makes sense to share the initialization code based on tree level.
      foreach (Info d in nodes) {
        if (d.Key == StartName) {
          d.Category = "DecisionNode";
          d.A = "I";
          d.AText = "Introversion";
          d.AToolTip  =  "The Introvert is “territorial” and desires space and solitude to recover energy.  Introverts enjoy solitary activities such as reading and meditating.  25% of the population.";
          d.B = "E";
          d.BText = "Extraversion";
          d.BToolTip  =  "The Extravert is “sociable” and is energized by the presence of other people.  Extraverts experience loneliness when not in contact with others.  75% of the population.";
        } else {
          switch (d.Key.Length) {
            case 1:
              d.Category = "DecisionNode";
              d.A = "N";
              d.AText = "Intuition";
              d.AToolTip  =  "The “intuitive” person bases their lives on predictions and ingenuity.  They consider the future and enjoy planning ahead.  25% of the population.";
              d.B = "S";
              d.BText = "Sensing";
              d.BToolTip = "The “sensing” person bases their life in facts, thinking primarily of their present situation.  They are realistic and practical.  75% of the population.";
              break;
            case 2:
              d.Category = "DecisionNode";
              d.A = "T";
              d.AText = "Thinking";
              d.AToolTip = "The “thinking” person bases their decisions based on facts and without personal bias.  They are more comfortable with making impersonal judgments.  50% of the population.";
              d.B = "F";
              d.BText = "Feeling";
              d.BToolTip = "The “feeling” person bases their decisions on personal experience and emotion.  They make their emotions very visible.  50% of the population.";
              break;
            case 3:
              d.Category = "DecisionNode";
              d.A = "J";
              d.AText = "Judgment";
              d.AToolTip = "The “judging” person enjoys closure.  They establish deadlines and take them seriously.  They despise being late.  50% of the population.";
              d.B = "P";
              d.BText = "Perception";
              d.BToolTip = "The “perceiving” person likes to keep options open and fluid.  They have little regard for deadlines.  Dislikes making decisions unless they are completely sure of they are right.  50% of the population.";
              break;
            default:
              d.Category = "PersonalityNode";
              break;
          }
        }
      }
      model.NodesSource = nodes;

      // Use the Key of each Info to construct the proper link data from the parent Info.
      // In a different application such programmatic construction of the links might not be possible.
      // But in this application, the relationships can be determined by splitting up the Key string.
      var links = new List<UniversalLinkData>();
      foreach (Info info in nodes) {
        String key = info.Key;
        if (key == StartName) continue;
        if (key.Length == 0) continue;
        // e.g., if key=="INTJ", we want: prefix="INT" and letter="J"
        String prefix = key.Substring(0, key.Length-1);
        String letter = key[key.Length-1].ToString();
        if (prefix.Length == 0) prefix = StartName;
        // e.g., connect node "INT" with port "J" to node "INTJ" with the whole node as the port
        links.Add(new UniversalLinkData(prefix, letter, key, null));
      }
      model.LinksSource = links;

      myDiagram.Model = model;
    }

    private readonly String StartName = "Start";

    private void Button_Click(object sender, RoutedEventArgs e) {
      Button button = sender as Button;
      if (button == null) return;
      Node node = Part.FindAncestor<Node>(button);
      if (node == null) return;
      myDiagram.StartTransaction("ExpandCollapse");
      // Hide/show children of this node via the particular Button which also acts as the "port"
      String pid = Node.GetPortId(button);
      if (node.FindLinksOutOfPort(pid).Any(l => l.Visibility == Visibility.Collapsed))
        ExpandTree(node, pid);
      else
        CollapseTree(node, pid);
      myDiagram.CommitTransaction("ExpandCollapse");
    }

    public void CollapseTree(Node node, String portid) {
      foreach (Link l in node.FindLinksOutOfPort(portid)) {
        Node n = l.GetOtherNode(node);
        if (n != null && n != node) {
          // Hide both the link and the node
          l.Visible = false;
          n.Visible = false;
          // Recursively collapse all children
          CollapseTree(n, null);
        }
      }
    }

    public void ExpandTree(Node node, String portid) {
      foreach (Link l in node.FindLinksOutOfPort(portid)) {
        Node n = l.GetOtherNode(node);
        if (n != null && n != node) {
          // Calculate a reasonable initial location for the layout animation;
          // LayoutDiagram (called due to CommitTransaction) will move it to its proper location
          n.Location = Spot.TopRight.PointInRect(node.Bounds);
          // Show both the link and the node
          l.Visible = true;
          n.Visible = true;
        }
      }
    }

    // After all of the nodes and links have been created,
    // make sure the "Start" node is visible.
    private void myDiagram_Loaded(object sender, RoutedEventArgs e) {
      if (myDiagram.Model == null) return;
      Info start = myDiagram.Model.FindNodeByKey(StartName) as Info;
      if (start != null) {
        Node node = myDiagram.PartManager.FindNodeForData(start, myDiagram.Model);
        if (node != null) {
          myDiagram.StartTransaction("Loaded");
          node.Visible = true;
          myDiagram.CommitTransaction("Loaded");
        }
      }
    }
  }

  public class Info : GraphLinksModelNodeData<String> {
    public String A { get; set; }         // the letter for the first choice
    public String AText { get; set; }     // the one word description of the first choice
    public String AToolTip { get; set; }  // the tooltip for the first button
    public String B { get; set; }         // the letter for the second choice
    public String BText { get; set; }     // the one word description of the second choice
    public String BToolTip { get; set; }  // the tooltip for the second button
  }
}
