using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// A control for editing multiple instances of a property.
  /// </summary>
  public class ManyEditor : Control, IWeakEventListener
  {
    /// <summary>
    /// A command for initialising an inconsistent set of values.
    /// </summary>
    public static readonly RoutedCommand ResetCommand = new RoutedCommand("Reset", typeof(ManyEditor));

    static ManyEditor()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ManyEditor),
        new FrameworkPropertyMetadata(typeof(ManyEditor)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManyEditor"/> class.
    /// </summary>
    public ManyEditor()
    {
      CommandBindings.Add(new CommandBinding(ResetCommand, ResetCommand_Executed, ResetCommand_CanExecute));
    }

    /// <summary>
    /// Gets or sets the value displayed in the editor.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ManyHolderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObjectWrapper ManyHolder
    {
      get { return (ObjectWrapper)GetValue(ManyHolderProperty); }
      set { SetValue(ManyHolderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ManyHolder"/> property.
    /// </summary>
    public static readonly DependencyProperty ManyHolderProperty =
      DependencyProperty.Register("ManyHolder", typeof(ObjectWrapper), typeof(ManyEditor),
      new FrameworkPropertyMetadata(OnManyHolderPropertyChanged));

    private static void OnManyHolderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ManyEditor)d).OnManyHolderPropertyChanged(e);
    }

    private void OnManyHolderPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      ObjectWrapper oldHolder = e.OldValue as ObjectWrapper;
      if (oldHolder != null)
      {
        Many many = oldHolder.RawValue as Many;
        if (many != null)
        {
          PropertyChangedEventManager.RemoveListener(many, this, "IsConsistent");
        }
      }

      if (ManyHolder != null)
      {
        Many many = ManyHolder.RawValue as Many;
        if (many != null)
        {
          PropertyChangedEventManager.AddListener(many, this, "IsConsistent");
          SetIsConsistent(many.IsConsistent);
        }
      }
    }

    /// <summary>
    /// Receives events from the centralized event manager.
    /// </summary>
    /// <param name="managerType">The type of the WeakEventManager calling this method.</param>
    /// <param name="sender">Object that originated the event.</param>
    /// <param name="e">Event data.</param>
    /// <returns>true if the event was handled; otherwise false.</returns>
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      PropertyChangedEventArgs pcea = e as PropertyChangedEventArgs;
      if (pcea != null)
      {
        Debug.Assert(pcea.PropertyName.Equals("IsConsistent", StringComparison.OrdinalIgnoreCase));
        bool isConsistent = ((Many)sender).IsConsistent;
        SetIsConsistent(isConsistent);
        return true;
      }
      return false;
    }

    private void SetIsConsistent(bool isConsistent)
    {
      // Runge were updating objects on a background thread and we had a bug
      // that was causing property change notifications to be fired even when
      // the object was not displayed in the grid.  The underlying bug is now
      // fixed but we leave the thread-safety code in for additional safety.
      if (CheckAccess())
      {
        SetValue(IsConsistentPropertyKey, isConsistent);
      }
      else
      {
        Action<bool> action = (b) => SetValue(IsConsistentPropertyKey, b);
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, action, isConsistent);
      }
    }

    /// <summary>
    /// Gets whether the held Many has a consistent value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsConsistentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsConsistent
    {
      get { return (bool)GetValue(IsConsistentProperty); }
    }

    private static readonly DependencyPropertyKey IsConsistentPropertyKey =
      DependencyProperty.RegisterReadOnly("IsConsistent", typeof(bool), typeof(ManyEditor), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsConsistent"/> property.
    /// </summary>
    public static readonly DependencyProperty IsConsistentProperty =
      IsConsistentPropertyKey.DependencyProperty;

    private void ResetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      ObjectWrapper ow = (ObjectWrapper)(e.Parameter);
      if (ow == null)
      {
        e.CanExecute = false;
      }
      else
      {
        try
        {
          Many many = (Many)(ow.RawValue);
          e.CanExecute = !many.IsReadOnly;
        }
        catch (ArgumentNullException)
        {
          e.CanExecute = false;
        }
      }
    }

    private void ResetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      ObjectWrapper ow = (ObjectWrapper)(e.Parameter);
      Many many = (Many)(ow.RawValue);
      many.Reset();
      ow.Property.RefreshChildren();
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the content of the "Reset" button.
    /// </summary>
    public static ComponentResourceKey ResetButtonContentKey
    {
      get { return new ComponentResourceKey(typeof(ManyEditor), "ResetButtonContent"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the tooltip of the "Reset" button.
    /// </summary>
    public static ComponentResourceKey ResetToolTipContentKey
    {
      get { return new ComponentResourceKey(typeof(ManyEditor), "ResetToolTipContent"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the "multiple values" data template.
    /// </summary>
    public static ComponentResourceKey InconsistentValuesTemplateKey
    {
      get { return new ComponentResourceKey(typeof(ManyEditor), "InconsistentValuesTemplate"); }
    }
  }
}
