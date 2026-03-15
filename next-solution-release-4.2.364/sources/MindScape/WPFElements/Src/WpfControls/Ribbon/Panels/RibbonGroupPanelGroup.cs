using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Mindscape.WpfElements;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  internal class RibbonGroupPanelGroup
  {
    private readonly IList<UIElement> _elements = new List<UIElement>();

    internal ReadOnlyCollection<UIElement> Elements
    {
      get { return new ReadOnlyCollection<UIElement>(_elements); }
    }

    internal double CollapsedWidth { get; set; }

    internal bool TryAddElement(UIElement element)
    {
      RibbonEditorSize smallestAvailableState;
      RibbonEditorSize largestAvailableState;
      if (CanAddElement(element, out smallestAvailableState, out largestAvailableState))
      {
        if (SmallestAvailableState == null)
        {
          SmallestAvailableState = smallestAvailableState;
        }
        else
        {
          // TODO: this will not be needed if a single group only accepts items with the same set of available states.
          SmallestAvailableState = LargestSize(SmallestAvailableState.Value, smallestAvailableState);
        }

        Ribbon.SetEditorSize(element, largestAvailableState);
        LargestState = largestAvailableState;

        _elements.Insert(0, element);
        return true;
      }
      return false;
    }

    private bool _canHaveLargeState;
    private bool _canHaveMediumState;
    private bool _canHaveSmallState;

    private bool CanAddElement(UIElement element, out RibbonEditorSize smallestAvailableState, out RibbonEditorSize largestAvailableState)
    {
      bool canHaveLargeState = CanHaveLargeState(element);
      bool canHaveMediumState = CanHaveMediumState(element);
      bool canHaveSmallState = CanHaveSmallState(element);
      if (canHaveSmallState)
      {
        smallestAvailableState = RibbonEditorSize.Small;
      }
      else if (canHaveMediumState)
      {
        smallestAvailableState = RibbonEditorSize.Medium;
      }
      else
      {
        smallestAvailableState = RibbonEditorSize.Large;
        canHaveLargeState = true;
      }

      if (canHaveLargeState)
      {
        largestAvailableState = RibbonEditorSize.Large;
      }
      else if (canHaveMediumState)
      {
        largestAvailableState = RibbonEditorSize.Medium;
      }
      else
      {
        largestAvailableState = RibbonEditorSize.Small;
      }

      if (_elements.Count == 0)
      {
        _canHaveLargeState = canHaveLargeState;
        _canHaveMediumState = canHaveMediumState;
        _canHaveSmallState = canHaveSmallState;
      }
      else
      {
        if (_canHaveLargeState == false && element is TextBlock)
        {
          return true;
        }
        if (_canHaveLargeState != canHaveLargeState || _canHaveMediumState != canHaveMediumState || _canHaveSmallState != canHaveSmallState)
        {
          return false;
        }
      }

      /*if (canHaveMediumState || canHaveSmallState)
      {
        return true;
      }*/
      if (!IsFull)
      {
        return true;
      }
      return false;
    }

    internal bool TryShrink()
    {
      if (CanShrink())
      {
        // TODO consider that case where all elements can only be in large or small state, not medium.
        RibbonEditorSize smallerSize = LargestState;
        switch (LargestState)
        {
          case RibbonEditorSize.Large:
            smallerSize = RibbonEditorSize.Medium;
            break;
          case RibbonEditorSize.Medium:
            smallerSize = RibbonEditorSize.Small;
            break;
        }
        foreach (UIElement element in _elements)
        {
          if (element is Gallery)
          {
            // TODO: this will be replaced with code to put the Gallery into a collapsed state.
            return false;
          }
          else
          {
            Ribbon.SetEditorSize(element, smallerSize);
          }
        }
        LargestState = smallerSize;
        return true;
      }
      return false;
    }

    public bool CanShrink()
    {
      if (_elements.Count == 0)
      {
        return false;
      }
      foreach (UIElement element in _elements)
      {
        if (element is Gallery)
        {
          return true;
        }
      }
      return SmallestAvailableState != LargestState;
    }

    private RibbonEditorSize? SmallestAvailableState { get; set; }

    // Gets the largest state out of all the elements in this group.
    private RibbonEditorSize LargestState { get; set; }

    // The higher the priority, the sooner it gets shrunk.
    internal int Priority
    {
      get
      {
        switch (LargestState)
        {
          case RibbonEditorSize.Small:
            return 1;
          case RibbonEditorSize.Medium:
            return 2;
        }
        return 3;
      }
    }

    // TODO: this should be temporary for now to help with the priority sorting.
    internal int Index { get; set; }

    internal bool IsFull
    {
      get { return _elements.Count >= 3; }
    }

    private static bool CanHaveLargeState(UIElement element)
    {
      if (element is NumericUpDown || element is IntegerUpDown)
      {
        return false;
      }
      if (element is ComboBox)
      {
        return false;
      }
      if (element is CheckBox)
      {
        return false;
      }
      if (element is DropDownColorPicker)
      {
        return false;
      }
      if (element is TextBlock)
      {
        return false;
      }
      if (element is Separator)
      {
        return true;
      }
      bool canHaveLargeState = Ribbon.GetLargeIcon(element) != null;
      return canHaveLargeState;
    }

    private static bool CanHaveMediumState(UIElement element)
    {
      if (element is NumericUpDown || element is IntegerUpDown)
      {
        return true;
      }
      if (element is ComboBox)
      {
        return true;
      }
      if (element is CheckBox)
      {
        return true;
      }
      if (element is DropDownColorPicker)
      {
        return true;
      }
      if (element is TextBlock)
      {
        return true;
      }
      if (element is Separator)
      {
        return false;
      }
      bool canHaveMediumState = Ribbon.GetSmallIcon(element) != null;
      return canHaveMediumState;
    }

    private static bool CanHaveSmallState(UIElement element)
    {
      if (element is NumericUpDown || element is IntegerUpDown)
      {
        return true;
      }
      if (element is ComboBox)
      {
        return true;
      }
      if (element is CheckBox)
      {
        return true; // really this should be false. Maybe allow both small-only and medium-only editors to be in the same group.
      }
      if (element is DropDownColorPicker)
      {
        return true;
      }
      if (element is TextBlock)
      {
        return true;
      }
      if (element is Separator)
      {
        return false;
      }
      bool canHaveSmallState = Ribbon.GetSmallIcon(element) != null;
      return canHaveSmallState;
    }

    private static RibbonEditorSize LargestSize(RibbonEditorSize size1, RibbonEditorSize size2)
    {
      if (size1 > size2)
      {
        return size1;
      }
      return size2;
    }
  }
}
