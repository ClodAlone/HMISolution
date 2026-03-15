using System.Threading;
using System.Windows;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Windows.Automation;
using System.Reflection;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public static class TemplateInstantiationHelper
  {
#if DEBUG

    private const string UnitTestUIHostExe = @"..\..\..\UnitTestUIHost\bin\Debug\UnitTestUIHost.exe";

#else

    private const string UnitTestUIHostExe = @"..\..\..\UnitTestUIHost\bin\Release\UnitTestUIHost.exe";

#endif

    private static void SanityCheckUISetupCallback(Type setupCallbackDeclaringType, string setupCallbackMethodName)
    {
      MethodInfo callbackMethod = setupCallbackDeclaringType.GetMethod(setupCallbackMethodName, BindingFlags.Public | BindingFlags.Static);
      if (callbackMethod == null)
      {
        Assert.Fail("UI test setup callback method not found: " + setupCallbackDeclaringType.FullName + "." + setupCallbackMethodName);
      }

      ParameterInfo[] callbackParameters = callbackMethod.GetParameters();
      if (callbackParameters.Length != 1 || callbackParameters[0].ParameterType != typeof(PropertyGrid))
      {
        Assert.Fail("UI test setup callback has wrong signature");
      }
    }

    public static List<string> RunUITest(Type setupCallbackDeclaringType, string setupCallbackMethodName, Action<AutomationElement> testCallback)
    {
      SanityCheckUISetupCallback(setupCallbackDeclaringType, setupCallbackMethodName);

      List<string> messages = new List<string>();

      string guid = Guid.NewGuid().ToString();
      string args = String.Format("{0} {1} {2}", guid, setupCallbackDeclaringType.FullName, setupCallbackMethodName);
      ProcessStartInfo psi = new ProcessStartInfo(UnitTestUIHostExe, args);
      psi.UseShellExecute = false;
      psi.RedirectStandardOutput = true;

      Process p = Process.Start(psi);
      p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
      {
        if (!String.IsNullOrEmpty(e.Data))
        {
          Debug.WriteLine("MESSAGE: " + e.Data);
          messages.Add(e.Data);
        }
      };
      p.BeginOutputReadLine();
      p.WaitForInputIdle();

      AutomationElement window = null;
      int numberOfDecisecondsWaited = 0;
      while (window == null)
      {
        if (numberOfDecisecondsWaited > 50)
        {
          Assert.Fail("timed out waiting for window to appear on automation list");
        }
        ++numberOfDecisecondsWaited;
        Thread.Sleep(100);
        window = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, guid));
      }

      try
      {
        testCallback(window);
      }
      finally
      {
        if (window != null)
        {
          WindowPattern windowPattern = window.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
          windowPattern.Close();
          p.WaitForExit(2000);
        }
        if (!p.HasExited)
        {
          p.Kill();
        }
      }

      return messages;
    }

    public static DataTemplate LoadFromXaml(string xaml)
    {
      using (XmlReader reader = XmlReader.Create(new StringReader(xaml)))
      {
        return XamlReader.Load(reader) as DataTemplate;
      }
    }
  }

  public class TestHolder<T> : Entity
  {
    private T _myProperty;

    public T MyProperty
    {
      get { return _myProperty; }
      set { Set(ref _myProperty, value, "MyProperty"); }
    }

    public TestHolder(T initialValue)
    {
      _myProperty = initialValue;

      PropertyChanged += OnPropertyChanged;

      if (_myProperty is INotifyPropertyChanged)
      {
        ((INotifyPropertyChanged)_myProperty).PropertyChanged += OnPropertyChanged;
      }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      object newValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null);
      Console.WriteLine(e.PropertyName + "=" + newValue);
    }
  }
}
