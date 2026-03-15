using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.ComponentModel;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using System.Reflection;
using Mindscape.WpfElements.PropertyEditing;
using System.Collections;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ObjectFilterDescriptionTests
  {
    private IPropertyInfo _info;
    private ObjectFilterDescription _description;

    [SetUp]
    public void SetUp()
    {
      PropertyInfo info = typeof(Person).GetProperty("Status");
      _info = new PassthroughPropertyInfoAdapter(info);

      IList values = new List<object>();
      values.Add(CitizenshipStatus.Alien);
      values.Add(CitizenshipStatus.Resident);
      values.Add(CitizenshipStatus.Citizen);
      values.Add(CitizenshipStatus.WorkerVisa);
      _description = new ObjectFilterDescription(_info, values);
    }

    [Test]
    public void EmptyValuesList()
    {
      ObjectFilterDescription desc = new ObjectFilterDescription(_info, new List<object>());
      Assert.IsNotNull(desc.Values);
      Assert.AreEqual(0, desc.Values.Count);

      Assert.IsNull(desc.Filter);
      Assert.IsFalse(desc.IsAllSelected.Value);
    }

    [Test]
    public void EnumValues()
    {
      Assert.IsNotNull(_description.Values);
      Assert.AreEqual(4, _description.Values.Count);

      SelectableObject so = _description.Values[0] as SelectableObject;
      Assert.IsNotNull(so);
      Assert.AreEqual(CitizenshipStatus.Alien, so.ActualValue);
      Assert.AreEqual("Alien", so.Value);
      Assert.IsFalse(so.IsSelected);

      so = _description.Values[1] as SelectableObject;
      Assert.IsNotNull(so);
      Assert.AreEqual(CitizenshipStatus.Resident, so.ActualValue);
      Assert.AreEqual("Resident", so.Value);
      Assert.IsFalse(so.IsSelected);

      so = _description.Values[2] as SelectableObject;
      Assert.IsNotNull(so);
      Assert.AreEqual(CitizenshipStatus.Citizen, so.ActualValue);
      Assert.AreEqual("Citizen", so.Value);
      Assert.IsFalse(so.IsSelected);

      so = _description.Values[3] as SelectableObject;
      Assert.IsNotNull(so);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, so.ActualValue);
      Assert.AreEqual("WorkerVisa", so.Value);
      Assert.IsFalse(so.IsSelected);

      Assert.IsNull(_description.Filter);
      Assert.IsFalse(_description.IsAllSelected.Value);
    }

    [Test]
    public void IsAllSelected_SomeSelected()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      Assert.IsNull(_description.IsAllSelected);
    }

    [Test]
    public void IsAllSelected_AllSelected()
    {
      foreach(SelectableObject so in _description.Values)
      {
        so.IsSelected = true;
      }

      Assert.IsTrue(_description.IsAllSelected.Value);
    }

    [Test]
    public void IsAllSelected_NoneSelected()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;
      so.IsSelected = false;

      Assert.IsFalse(_description.IsAllSelected.Value);
    }

    [Test]
    public void SetIsAllSelectedToTrue_WhenOneItemSelected()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;
      _description.IsAllSelected = true;

      foreach (SelectableObject obj in _description.Values)
      {
        Assert.IsTrue(obj.IsSelected);
      }
    }

    [Test]
    public void SetIsAllSelectedToTrue_WhenNoneSelected()
    {
      _description.IsAllSelected = true;

      foreach (SelectableObject so in _description.Values)
      {
        Assert.IsTrue(so.IsSelected);
      }
    }

    [Test]
    public void SetIsAllSelectedToFalse_WhenOneItemSelected()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;
      _description.IsAllSelected = false;

      foreach (SelectableObject obj in _description.Values)
      {
        Assert.IsFalse(obj.IsSelected);
      }
    }

    [Test]
    public void SetIsAllSelectedToFalse_WhenAllSelected()
    {
      _description.IsAllSelected = true;
      _description.IsAllSelected = false;

      foreach (SelectableObject so in _description.Values)
      {
        Assert.IsFalse(so.IsSelected);
      }
    }

    [Test]
    public void SetIsAllSelectedToNull_WhenNoneSelected()
    {
      _description.IsAllSelected = null;

      // The items remain unselected:
      foreach (SelectableObject so in _description.Values)
      {
        Assert.IsFalse(so.IsSelected);
      }

      // IsAllSelected correctly changes back to false:
      Assert.IsFalse(_description.IsAllSelected.Value);
    }

    [Test]
    public void SetIsAllSelectedToNull_WhenAllSelected()
    {
      _description.IsAllSelected = true;
      _description.IsAllSelected = null;

      // The items remain selected:
      foreach (SelectableObject so in _description.Values)
      {
        Assert.IsTrue(so.IsSelected);
      }

      // IsAllSelected correctly changes back to true:
      Assert.IsTrue(_description.IsAllSelected.Value);
    }

    [Test]
    public void SetIsAllSelectedToNull_WhenOneItemSelected()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      _description.IsAllSelected = null;

      // The items are unchanged:
      foreach (SelectableObject obj in _description.Values)
      {
        if (obj == so)
        {
          Assert.IsTrue(obj.IsSelected);
        }
        else
        {
          Assert.IsFalse(obj.IsSelected);
        }
      }

      // IsAllSelected correctly remains null
      Assert.IsNull(_description.IsAllSelected);
    }

    [Test]
    public void SelectingItem_RaisesEvent()
    {
      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };
      SelectableObject so = _description.Values[1] as SelectableObject;
      Assert.AreEqual(0, raised);
      so.IsSelected = true;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void DeselectingItem_RaisesEvent()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;
      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };
      Assert.AreEqual(0, raised);
      so.IsSelected = false;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToTrue_WhenOneItemSelected_RaisesEvent()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = true;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToTrue_WhenNoneSelected_RaisesEvent()
    {
      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = true;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToTrue_WhenAllSelected_DoesNotRaiseEvent()
    {
      _description.IsAllSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = true;
      Assert.AreEqual(0, raised);
    }

    [Test]
    public void SetIsAllSelectedToFalse_WhenOneItemSelected_RaisesEvent()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = false;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToFalse_WhenAllSelected_RaisesEvent()
    {
      _description.IsAllSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = false;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToFalse_WhenNoneSelected_DoesNotRaiseEvent()
    {
      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = false;
      Assert.AreEqual(0, raised);
    }

    [Test]
    [Ignore("Low priority")]
    public void SetIsAllSelectedToNull_WhenNoneSelected_DoesNotRaiseEvent()
    {
      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = null;
      Assert.AreEqual(0, raised);
    }

    [Test]
    [Ignore("Low priority")]
    public void SetIsAllSelectedToNull_WhenAllSelected_DoesNotRaiseEvent()
    {
      _description.IsAllSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = null;
      Assert.AreEqual(0, raised);
    }

    [Test]
    public void SetIsAllSelectedToNull_WhenOneItemSelected_DoesNotRaiseEvent()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      int raised = 0;
      _description.FilterChanged += (o, e) => { raised++; };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = null;
      Assert.AreEqual(0, raised);
    }

    [Test]
    public void SetIsAllSelected_RaisesPropertyChanged()
    {
      int raised = 0;
      _description.PropertyChanged += (o, e) => { if ("IsAllSelected".Equals(e.PropertyName)) { raised++; } };

      Assert.AreEqual(0, raised);
      _description.IsAllSelected = true;
      Assert.AreEqual(1, raised);

      raised = 0;
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = false;
      Assert.AreEqual(1, raised);
    }

    [Test]
    public void SetIsAllSelectedToTrue_UpdatesFilter()
    {
      _description.IsAllSelected = true;

      OrFilter filter = _description.Filter as OrFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(4, filter.Filters.Count);

      EqualsFilter ef = filter.Filters[0] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Alien, ef.Value);

      ef = filter.Filters[1] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Resident, ef.Value);

      ef = filter.Filters[2] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Citizen, ef.Value);

      ef = filter.Filters[3] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ef.Value);
    }

    [Test]
    public void SetIsAllSelectedToFalse_UpdatesFilter()
    {
      _description.IsAllSelected = true;
      _description.IsAllSelected = false;

      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SelectItem_UpdatesFilter()
    {
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = true;

      // Note that when there is only one item selected, there is a single EqualsFilter rather than an OrFilter.
      EqualsFilter ef = _description.Filter as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Resident, ef.Value);
    }

    [Test]
    public void DeselectItem_UpdatesFilter()
    {
      _description.IsAllSelected = true;
      SelectableObject so = _description.Values[1] as SelectableObject;
      so.IsSelected = false;

      OrFilter filter = _description.Filter as OrFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(3, filter.Filters.Count);

      EqualsFilter ef = filter.Filters[0] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Alien, ef.Value);

      ef = filter.Filters[1] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.Citizen, ef.Value);

      ef = filter.Filters[2] as EqualsFilter;
      Assert.IsNotNull(ef);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ef.Value);
    }

    // TODO: test that only one event is raised when calling SetAs.
    // TODO: test if the OrFilter contains something other than an EqualsFilter.

    [Test]
    public void SetAsOrFilter()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[0]).IsSelected = true;

      OrFilter filter = new OrFilter();
      filter.Add(new EqualsFilter(CitizenshipStatus.WorkerVisa));
      filter.Add(new EqualsFilter(CitizenshipStatus.Resident));

      _description.SetAs(filter);

      // Test the order of the values to ensure the IsSelected assertions are correct
      Assert.AreEqual(CitizenshipStatus.Alien, ((SelectableObject)_description.Values[0]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Resident, ((SelectableObject)_description.Values[1]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Citizen, ((SelectableObject)_description.Values[2]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ((SelectableObject)_description.Values[3]).ActualValue);

      // The appropriate items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsNull(_description.IsAllSelected);

      // The resulting filter is mimicked
      OrFilter f = _description.Filter as OrFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(2, f.Filters.Count);
      EqualsFilter f1 = f.Filters[0] as EqualsFilter;
      Assert.IsNotNull(f1);
      Assert.AreEqual(CitizenshipStatus.Resident, f1.Value);
      EqualsFilter f2 = f.Filters[1] as EqualsFilter;
      Assert.IsNotNull(f2);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, f2.Value);
    }

    [Test]
    public void SetAsEmptyOrFilter()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[2]).IsSelected = true;

      OrFilter filter = new OrFilter();

      _description.SetAs(filter);

      // The appropriate items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsFalse(_description.IsAllSelected.Value);

      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsSingleOrFilter()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[3]).IsSelected = true;

      OrFilter filter = new OrFilter();
      filter.Add(new EqualsFilter(CitizenshipStatus.Resident));

      _description.SetAs(filter);

      // Test the order of the values to ensure the IsSelected assertions are correct
      Assert.AreEqual(CitizenshipStatus.Alien, ((SelectableObject)_description.Values[0]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Resident, ((SelectableObject)_description.Values[1]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Citizen, ((SelectableObject)_description.Values[2]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ((SelectableObject)_description.Values[3]).ActualValue);

      // The appropriate items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsNull(_description.IsAllSelected);

      // The resulting filter mimics the lone filter
      EqualsFilter f = _description.Filter as EqualsFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(CitizenshipStatus.Resident, f.Value);
    }

    [Test]
    public void SetAsFullOrFilter()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new EqualsFilter(CitizenshipStatus.WorkerVisa));
      filter.Add(new EqualsFilter(CitizenshipStatus.Resident));
      filter.Add(new EqualsFilter(CitizenshipStatus.Alien));
      filter.Add(new EqualsFilter(CitizenshipStatus.Citizen));

      _description.SetAs(filter);

      // The appropriate items are selected
      Assert.IsTrue(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsTrue(_description.IsAllSelected.Value);

      // The resulting filter is mimicked
      OrFilter f = _description.Filter as OrFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(4, f.Filters.Count);
      // These are in the order that they were added to the description
      EqualsFilter f1 = f.Filters[0] as EqualsFilter;
      Assert.IsNotNull(f1);
      Assert.AreEqual(CitizenshipStatus.Alien, f1.Value);
      EqualsFilter f2 = f.Filters[1] as EqualsFilter;
      Assert.IsNotNull(f2);
      Assert.AreEqual(CitizenshipStatus.Resident, f2.Value);
      EqualsFilter f3 = f.Filters[2] as EqualsFilter;
      Assert.IsNotNull(f3);
      Assert.AreEqual(CitizenshipStatus.Citizen, f3.Value);
      EqualsFilter f4 = f.Filters[3] as EqualsFilter;
      Assert.IsNotNull(f4);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, f4.Value);
    }

    [Test]
    public void SetAsSingleEqualsFilter()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[3]).IsSelected = true;

      EqualsFilter filter = new EqualsFilter(CitizenshipStatus.Resident);
      _description.SetAs(filter);

      // Test the order of the values to ensure the IsSelected assertions are correct
      Assert.AreEqual(CitizenshipStatus.Alien, ((SelectableObject)_description.Values[0]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Resident, ((SelectableObject)_description.Values[1]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Citizen, ((SelectableObject)_description.Values[2]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ((SelectableObject)_description.Values[3]).ActualValue);

      // The appropriate items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsTrue(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsNull(_description.IsAllSelected);

      // The resulting filter mimics the lone filter
      EqualsFilter f = _description.Filter as EqualsFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(CitizenshipStatus.Resident, f.Value);
    }

    [Test]
    public void SetAsNull()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[1]).IsSelected = true;

      _description.SetAs(null);

      // None of the items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsFalse(_description.IsAllSelected.Value);

      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsUnsupportedFilter()
    {
      // Set a non-default value
      ((SelectableObject)_description.Values[1]).IsSelected = true;

      FalseFilter filter = new FalseFilter();
      _description.SetAs(filter);

      // None of the items are selected
      Assert.IsFalse(((SelectableObject)_description.Values[0]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[1]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[2]).IsSelected);
      Assert.IsFalse(((SelectableObject)_description.Values[3]).IsSelected);

      Assert.IsFalse(_description.IsAllSelected.Value);

      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }
  }
}
