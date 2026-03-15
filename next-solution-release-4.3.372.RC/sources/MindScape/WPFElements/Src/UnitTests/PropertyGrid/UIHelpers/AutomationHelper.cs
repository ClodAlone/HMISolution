using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public static class AutomationHelper
  {
    public static string GetValue(AutomationElement element)
    {
      ValuePattern.ValuePatternInformation vpi = AsValuePattern(element).Current;
      return vpi.Value;
    }

    public static void SetValue(AutomationElement element, string value)
    {
      AsValuePattern(element).SetValue(value);
    }

    public static AutomationElement GetNextSibling(AutomationElement startFrom, string className)
    {
      AutomationElement result = startFrom;

      while (true)
      {
        result = TreeWalker.RawViewWalker.GetNextSibling(result);
        if (result == null)
        {
          return null;
        }
        if ((string)(result.GetCurrentPropertyValue(AutomationElement.ClassNameProperty)) == className)
        {
          return result;
        }
      }
    }

    public static void AssertTextBoxValueAndUpdate(AutomationElement textBox, string expectedValue, string valueToSet)
    {
      Assert.IsNotNull(textBox);
      Assert.AreEqual(AutomationClassName.TextBox, textBox.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as string);
      Assert.AreEqual(expectedValue, GetValue(textBox));
      SetValue(textBox, valueToSet);
    }

    public static Condition VisibleElementsOfClass(string className)
    {
      PropertyCondition isVisible = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
      PropertyCondition isRightType = AllElementsOfClass(className);
      return new AndCondition(isVisible, isRightType);
    }

    public static PropertyCondition AllElementsOfClass(string className)
    {
      return new PropertyCondition(AutomationElement.ClassNameProperty, className);
    }

    public static AutomationElement FindFirstDescendant(AutomationElement startFrom, string className)
    {
      return startFrom.FindFirst(TreeScope.Descendants, VisibleElementsOfClass(className));
    }

    public static AutomationElementCollection FindAllChildren(AutomationElement startFrom, string className)
    {
      return startFrom.FindAll(TreeScope.Children, VisibleElementsOfClass(className));
    }

    public static AutomationElementCollection FindAllChildren(AutomationElement startFrom, string className, bool allowInvisible)
    {
      Condition condition = allowInvisible ? AllElementsOfClass(className) : VisibleElementsOfClass(className);
      return startFrom.FindAll(TreeScope.Children, condition);
    }

    public static TogglePattern AsTogglePattern(AutomationElement automationElement)
    {
      return AsPattern<TogglePattern>(automationElement, TogglePattern.Pattern);
    }

    public static ValuePattern AsValuePattern(AutomationElement automationElement)
    {
      return AsPattern<ValuePattern>(automationElement, ValuePattern.Pattern);
    }

    public static ExpandCollapsePattern AsExpandCollapsePattern(AutomationElement automationElement)
    {
      return AsPattern<ExpandCollapsePattern>(automationElement, ExpandCollapsePattern.Pattern);
    }

    public static SelectionPattern AsSelectionPattern(AutomationElement automationElement)
    {
      return AsPattern<SelectionPattern>(automationElement, SelectionPattern.Pattern);
    }

    public static SelectionItemPattern AsSelectionItemPattern(AutomationElement automationElement)
    {
      return AsPattern<SelectionItemPattern>(automationElement, SelectionItemPattern.Pattern);
    }

    private static T AsPattern<T>(AutomationElement automationElement, AutomationPattern pattern)
      where T : class
    {
      return automationElement.GetCurrentPattern(pattern) as T;
    }
  }

  public static class AutomationClassName
  {
    public const string CheckBox = "CheckBox";
    public const string ComboBox = "ComboBox";
    public const string ListBoxItem = "ListBoxItem";
    public const string TextBox = "TextBox";
  }
}
