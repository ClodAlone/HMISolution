/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace ClassHierarchy {
  public partial class ClassHierarchy : UserControl {
    public ClassHierarchy() {
      InitializeComponent();

      TreeModel<TypeInfo, String> model = new TreeModel<TypeInfo, String>();
      // assume this model is not Modifiable
      model.NodeKeyPath = "FullName"; // these are properties of TypeInfo, defined below
      model.ParentNodePath = "BaseTypeName";

      // collect all of the interesting public classes in Northwoods.GoXam,
      // plus all of their base classes regardless of assembly or namespace
      HashSet<Type> types = new HashSet<Type>();
      types.UnionWith(typeof(Diagram).Assembly.GetTypes()
        .Where(t => t.IsClass && t.IsPublic && !t.IsGenericType
                    && !typeof(Attribute).IsAssignableFrom(t))
        .ToList());
      int oldcount = 0;
      while (types.Count > oldcount) {
        oldcount = types.Count;
        types.UnionWith(types
          .Where(t => t.IsClass && t.IsPublic && !t.IsGenericType
                      && !typeof(Attribute).IsAssignableFrom(t))
          .Select(t => t.BaseType)
          .Where(t => t != null)
          .ToList());
      }

      // remember the information in TypeInfo objects;
      // to avoid repeated recomputations of deferred Linq query, call ToList()
      model.NodesSource = types
        .Select(t => new TypeInfo() {
          Name = t.Name,
          FullName = (t.FullName != null ? t.FullName : t.Name),
          BaseTypeName = (t.BaseType != null ? t.BaseType.FullName : null)
        })
        .ToList();

      myDiagram.Model = model;
    }

    // copy the Type data into a separate class, to avoid Reflection permission problems
    public class TypeInfo {
      public String Name { get; set; }      // we're assuming the model is not modifiable,
      public String FullName { get; set; }  // so don't notify the model when these are set
      public String BaseTypeName { get; set; }
    }
  }
}
