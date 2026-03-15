using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MulticolumnTreeViewTests
  {
    [Test]
    [STAThread]
    public void ColumnWrappersPreserveProperties()
    {
      GridViewColumn column0 = new GridViewColumn();

      column0.CellTemplate = new DataTemplate();
      column0.CellTemplateSelector = new DataTemplateSelector();
      column0.DisplayMemberBinding = new Binding();
      column0.Header = "Fie";
      column0.HeaderContainerStyle = new Style();
      column0.HeaderTemplate = new DataTemplate();
      column0.HeaderTemplateSelector = new DataTemplateSelector();
      column0.Width = 123;

      GridViewColumn column1 = new GridViewColumn();

      column1.CellTemplate = new DataTemplate();
      column1.CellTemplateSelector = new DataTemplateSelector();
      column1.DisplayMemberBinding = new Binding();
      column1.Header = "Tchah";
      column1.HeaderContainerStyle = new Style();
      column1.HeaderTemplate = new DataTemplate();
      column1.HeaderTemplateSelector = new DataTemplateSelector();
      column1.Width = 321;

      MulticolumnTreeView tv = new MulticolumnTreeView();
      tv.Columns.Add(column0);
      tv.Columns.Add(column1);

      GridViewColumn wrapper0 = tv.WrappedColumns[0];

      Assert.AreEqual(column0.Header, wrapper0.Header);
      Assert.AreEqual(column0.HeaderContainerStyle, wrapper0.HeaderContainerStyle);
      Assert.AreEqual(column0.HeaderTemplate, wrapper0.HeaderTemplate);
      Assert.AreEqual(column0.HeaderTemplateSelector, wrapper0.HeaderTemplateSelector);
      Assert.AreEqual(column0.Width, wrapper0.Width);

      GridViewColumn wrapper1 = tv.WrappedColumns[1];

      Assert.AreEqual(column1.CellTemplate, wrapper1.CellTemplate);
      Assert.AreEqual(column1.CellTemplateSelector, wrapper1.CellTemplateSelector);
      Assert.AreEqual(column1.DisplayMemberBinding, wrapper1.DisplayMemberBinding);
      Assert.AreEqual(column1.Header, wrapper1.Header);
      Assert.AreEqual(column1.HeaderContainerStyle, wrapper1.HeaderContainerStyle);
      Assert.AreEqual(column1.HeaderTemplate, wrapper1.HeaderTemplate);
      Assert.AreEqual(column1.HeaderTemplateSelector, wrapper1.HeaderTemplateSelector);
      Assert.AreEqual(column1.Width, wrapper1.Width);
    }

    [Test]
    [STAThread]
    public void ColumnWrappersTrackColumnPropertyChanges()
    {
      GridViewColumn column0 = new GridViewColumn();

      column0.CellTemplate = new DataTemplate();
      column0.CellTemplateSelector = new DataTemplateSelector();
      column0.DisplayMemberBinding = new Binding();
      column0.Header = "Fie";
      column0.HeaderContainerStyle = new Style();
      column0.HeaderTemplate = new DataTemplate();
      column0.HeaderTemplateSelector = new DataTemplateSelector();
      column0.Width = 123;

      GridViewColumn column1 = new GridViewColumn();

      column1.CellTemplate = new DataTemplate();
      column1.CellTemplateSelector = new DataTemplateSelector();
      column1.DisplayMemberBinding = new Binding();
      column1.Header = "Tchah";
      column1.HeaderContainerStyle = new Style();
      column1.HeaderTemplate = new DataTemplate();
      column1.HeaderTemplateSelector = new DataTemplateSelector();
      column1.Width = 321;

      MulticolumnTreeView tv = new MulticolumnTreeView();
      tv.Columns.Add(column0);
      tv.Columns.Add(column1);

      GridViewColumn wrapper0 = tv.WrappedColumns[0];

      Assert.AreEqual(column0.Header, wrapper0.Header);
      Assert.AreEqual(column0.HeaderContainerStyle, wrapper0.HeaderContainerStyle);
      Assert.AreEqual(column0.HeaderTemplate, wrapper0.HeaderTemplate);
      Assert.AreEqual(column0.HeaderTemplateSelector, wrapper0.HeaderTemplateSelector);
      Assert.AreEqual(column0.Width, wrapper0.Width);

      column0.Header = "Changed";
      column0.HeaderContainerStyle = new Style();
      column0.HeaderTemplate = new DataTemplate();
      column0.HeaderTemplateSelector = new DataTemplateSelector();
      column0.Width = 12345;

      Assert.AreEqual(column0.Header, wrapper0.Header);
      Assert.AreEqual(column0.HeaderContainerStyle, wrapper0.HeaderContainerStyle);
      Assert.AreEqual(column0.HeaderTemplate, wrapper0.HeaderTemplate);
      Assert.AreEqual(column0.HeaderTemplateSelector, wrapper0.HeaderTemplateSelector);
      Assert.AreEqual(column0.Width, wrapper0.Width);

      column0.CellTemplate = new DataTemplate();
      column0.CellTemplateSelector = new DataTemplateSelector();
      column0.DisplayMemberBinding = null;

      Assert.AreEqual(column0.CellTemplate, ((MulticolumnTreeViewColumn)wrapper0).OriginalCellTemplate);
      Assert.AreEqual(column0.CellTemplateSelector, ((MulticolumnTreeViewColumn)wrapper0).OriginalCellTemplateSelector);

      column0.DisplayMemberBinding = new Binding();

      Assert.AreNotEqual(column0.CellTemplate, ((MulticolumnTreeViewColumn)wrapper0).OriginalCellTemplate);

      column1.CellTemplate = new DataTemplate();
      column1.CellTemplateSelector = new DataTemplateSelector();
      column1.DisplayMemberBinding = new Binding();
      column1.Header = "Also changed";
      column1.HeaderContainerStyle = new Style();
      column1.HeaderTemplate = new DataTemplate();
      column1.HeaderTemplateSelector = new DataTemplateSelector();
      column1.Width = 3210;

      GridViewColumn wrapper1 = tv.WrappedColumns[1];

      Assert.AreEqual(column1.CellTemplate, wrapper1.CellTemplate);
      Assert.AreEqual(column1.CellTemplateSelector, wrapper1.CellTemplateSelector);
      Assert.AreEqual(column1.DisplayMemberBinding, wrapper1.DisplayMemberBinding);
      Assert.AreEqual(column1.Header, wrapper1.Header);
      Assert.AreEqual(column1.HeaderContainerStyle, wrapper1.HeaderContainerStyle);
      Assert.AreEqual(column1.HeaderTemplate, wrapper1.HeaderTemplate);
      Assert.AreEqual(column1.HeaderTemplateSelector, wrapper1.HeaderTemplateSelector);
      Assert.AreEqual(column1.Width, wrapper1.Width);
    }

    [Test]
    [STAThread]
    public void ColumnWrappersTrackColumnCollectionChanges()
    {
      GridViewColumn columnA = new GridViewColumn();
      columnA.DisplayMemberBinding = new Binding("A");

      GridViewColumn columnB = new GridViewColumn();
      columnB.DisplayMemberBinding = new Binding("B");

      GridViewColumn columnC = new GridViewColumn();
      DataTemplate templateC = new DataTemplate();
      columnC.CellTemplate = templateC;

      GridViewColumn columnD = new GridViewColumn();
      DataTemplateSelector selectorD = new DataTemplateSelector();
      columnD.CellTemplateSelector = selectorD;

      DataTemplate expander = new DataTemplate();

      MulticolumnTreeView tv = new MulticolumnTreeView();
      tv.ExpandingDecorator = expander;
      tv.Columns.Add(columnA);
      tv.Columns.Add(columnB);

      Assert.AreEqual(2, tv.WrappedColumns.Count);
      Assert.AreEqual(expander, tv.WrappedColumns[0].CellTemplate);
      Assert.IsNull(tv.WrappedColumns[1].CellTemplate);

      tv.Columns.RemoveAt(0);

      Assert.AreEqual(1, tv.WrappedColumns.Count);
      Assert.AreEqual(expander, tv.WrappedColumns[0].CellTemplate);

      tv.Columns.Insert(0, columnC);

      Assert.AreEqual(2, tv.WrappedColumns.Count);
      Assert.AreEqual(expander, tv.WrappedColumns[0].CellTemplate);
      Assert.AreEqual(templateC, ((MulticolumnTreeViewColumn)(tv.WrappedColumns[0])).OriginalCellTemplate);
      Assert.IsNull(tv.WrappedColumns[1].CellTemplate);

      tv.Columns.Insert(0, columnD);

      Assert.AreEqual(3, tv.WrappedColumns.Count);
      Assert.AreEqual(expander, tv.WrappedColumns[0].CellTemplate);
      Assert.AreEqual(selectorD, ((MulticolumnTreeViewColumn)(tv.WrappedColumns[0])).OriginalCellTemplateSelector);
      Assert.AreEqual(templateC, tv.WrappedColumns[1].CellTemplate);
      Assert.IsNull(tv.WrappedColumns[2].CellTemplate);

      tv.Columns.Clear();

      Assert.AreEqual(0, tv.WrappedColumns.Count);

      GridViewColumnCollection newColumns = new GridViewColumnCollection();
      newColumns.Add(new GridViewColumn());
      newColumns.Add(new GridViewColumn());
      tv.Columns = newColumns;

      Assert.AreEqual(2, tv.WrappedColumns.Count);
    }

    [Test]
    [STAThread]
    public void ItemLevel()
    {
      MulticolumnTreeViewItem root = new MulticolumnTreeViewItem();
      MulticolumnTreeViewItem child = new MulticolumnTreeViewItem();
      MulticolumnTreeViewItem grandchild = new MulticolumnTreeViewItem();
      root.Items.Add(child);
      child.Items.Add(grandchild);

      Assert.AreEqual(0, root.Level);
      Assert.AreEqual(1, child.Level);
      Assert.AreEqual(2, grandchild.Level);
    }

    [Test]
    public void Indenting()
    {
      MulticolumnTreeViewIndentConverter indenter = new MulticolumnTreeViewIndentConverter();
      indenter.Indent = 10;

      Assert.AreEqual(new Thickness(10, 0, 0, 0), indenter.Convert(1, null, null, null));
      Assert.AreEqual(new Thickness(50, 0, 0, 0), indenter.Convert(5, null, null, null));
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void Indenting_ConvertBack()
    {
      MulticolumnTreeViewIndentConverter indenter = new MulticolumnTreeViewIndentConverter();
      indenter.Indent = 10;

      indenter.ConvertBack(new Thickness(10, 0, 0, 0), null, null, null);
    }
  }
}
