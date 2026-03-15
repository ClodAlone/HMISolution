/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace PERT {
  public partial class PERT : UserControl {
    public PERT() {
      InitializeComponent();

      // create the diagram's data model
      var model = new GraphModel<Activity, int>();
      model.NodesSource = new ObservableCollection<Activity>() {
        // don't use Key==0
        new Activity() { Key=1, Text="Start", FromKeys=P(), Length=0, EarlyStart=0, LateFinish=0, Critical=true },
        new Activity() { Key=2, Text="a", FromKeys=P( 1 ), Length=4, EarlyStart=0, LateFinish=4, Critical=true },
        new Activity() { Key=3, Text="b", FromKeys=P( 1 ), Length=5.33, EarlyStart=0, LateFinish=9.17 },
        new Activity() { Key=4, Text="c", FromKeys=P( 2 ), Length=5.17, EarlyStart=4, LateFinish=9.17, Critical=true },
        new Activity() { Key=5, Text="d", FromKeys=P( 2 ), Length=6.33, EarlyStart=4, LateFinish=15.01 },
        new Activity() { Key=6, Text="e", FromKeys=P( 3, 4 ), Length=5.17, EarlyStart=9.17, LateFinish=14.34, Critical=true },
        new Activity() { Key=7, Text="f", FromKeys=P( 5 ), Length=4.5, EarlyStart=10.33, LateFinish=19.51 },
        new Activity() { Key=8, Text="g", FromKeys=P( 6 ), Length=5.17, EarlyStart=14.34, LateFinish=19.51, Critical=true },
        new Activity() { Key=9, Text="Finish", FromKeys=P( 7, 8 ), Length=0, EarlyStart=19.51, LateFinish=19.51, Critical=true },
      };
      myDiagram.Model = model;
    }

    // this is just for convenience in typing in the predecessors for each Activity
    private ObservableCollection<int> P(params int[] args) {
      var coll = new ObservableCollection<int>();
      foreach (int i in args) coll.Add(i);
      return coll;
    }
  }


  // the data for each node
  public class Activity : GraphModelNodeData<int> {
    public Activity() {
      this.Length = 5;
      this.EarlyStart = 0;
      this.LateFinish = 0;
      this.Critical = false;
    }

    public double Length {
      get { return _Length; }
      set {
        if (_Length != value) {
          double old = _Length;
          _Length = value;
          RaisePropertyChanged("Length", old, value);
        }
      }
    }
    private double _Length;

    public double EarlyStart {
      get { return _EarlyStart; }
      set {
        value = Math.Round(value, 2);
        if (_EarlyStart != value) {
          double old = _EarlyStart;
          _EarlyStart = value;
          RaisePropertyChanged("EarlyStart", old, value);
        }
      }
    }
    private double _EarlyStart;

    public double EarlyFinish {
      get { return Math.Round(this.EarlyStart + this.Length, 2); }
    }

    public double LateStart {
      get { return Math.Round(this.LateFinish - this.Length, 2); }
    }

    public double LateFinish {
      get { return _LateFinish; }
      set {
        value = Math.Round(value, 2);
        if (_LateFinish != value) {
          double old = _LateFinish;
          _LateFinish = value;
          RaisePropertyChanged("LateFinish", old, value);
        }
      }
    }
    private double _LateFinish;

    public double Slack {
      get { return Math.Round(this.LateFinish - this.EarlyFinish, 2); }
    }

    public bool Critical {
      get { return _Critical; }
      set {
        if (_Critical != value) {
          bool old = _Critical;
          _Critical = value;
          RaisePropertyChanged("Critical", old, value);
        }
      }
    }
    private bool _Critical;
  }

  // Return the TrueBrush if both connected Activity nodes are Critical;
  // otherwise return the FalseBrush.
  public class LinkConverter : BooleanBrushConverter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var ldata = value as PartManager.VirtualLinkData;
      if (ldata != null) {
        var from = ldata.From as Activity;
        var to = ldata.To as Activity;
        if (from != null && to != null && from.Critical && to.Critical) return this.TrueBrush;
      }
      return this.FalseBrush;
    }
  }
}
