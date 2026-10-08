using System;
using System.Linq;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Client;
class P {
  static void Main() {
    var t = typeof(SessionClientExtensions);
    foreach (var m in t.GetMethods(BindingFlags.Public|BindingFlags.Static).Where(m => m.Name=="BrowseNextAsync"))
      Console.WriteLine(m);
    Console.WriteLine("-- BrowseNextResponse props --");
    foreach (var p in typeof(BrowseNextResponse).GetProperties(BindingFlags.Public|BindingFlags.Instance))
      Console.WriteLine(p.PropertyType + " " + p.Name);
    Console.WriteLine("-- Variant ctors --");
    foreach (var c in typeof(Variant).GetConstructors())
      Console.WriteLine(c);
  }
}
