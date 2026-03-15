using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security.Permissions;
using System.Security;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  internal class DialogHelper
  {
    #region ScheduleItemDialog

    internal static void ShowItemDialog(ScheduleItemDialog dlg, UIElement owner, SchedulerFormatter formatter)
    {
      if (formatter != null && formatter.ScheduleItemDialogStyle != null)
      {
        dlg.Style = formatter.ScheduleItemDialogStyle;
      }
      if (HasAllWindowsPermission())
      {
        Window wnd = new Window();
        //wnd.Title = "New Appointment";
        wnd.ResizeMode = ResizeMode.NoResize;
        wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wnd.Content = dlg;
        //wnd.Width = 400;
        //wnd.Height = 220;
        wnd.SizeToContent = SizeToContent.WidthAndHeight;
        wnd.Closed += new EventHandler(Window_Closed);
        wnd.Owner = VisualTreeUtils.FindAncestor<Window>(owner);
        wnd.ShowDialog();
      }
      else
      {
        Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(owner);
        Grid grid = VisualTreeUtils.FindAncestor<Grid>(scheduler);
        if (grid != null)
        {
          dlg.Height = 180;
          ChildWindow child = new ChildWindow();
          child.WindowWidth = 400;
          child.WindowHeight = 200;
          child.Content = dlg;

          dlg.HorizontalAlignment = HorizontalAlignment.Center;
          dlg.VerticalAlignment = VerticalAlignment.Center;
          dlg.Closed += new RoutedEventHandler(ScheduleItemDialog_Closed);
          
          grid.Children.Add(child);
        }
      }
    }

    private static void ScheduleItemDialog_Closed(object sender, RoutedEventArgs e)
    {
      ScheduleItemDialog dlg = sender as ScheduleItemDialog;
      dlg.Closed -= new RoutedEventHandler(ScheduleItemDialog_Closed);

      ChildWindow child = VisualTreeUtils.FindAncestor<ChildWindow>(dlg);
      Grid grid = child.Parent as Grid;
      grid.Children.Remove(child);
    }

    private static void Window_Closed(object sender, EventArgs e)
    {
      Window window = sender as Window;
      ScheduleItemDialog dlg = window.Content as ScheduleItemDialog;
      if (dlg != null)
      {
        dlg.OnClosed();
      }
    }

    #endregion // ScheduleItemDialog

    #region RecurrenceDialog

    internal static void ShowRecurrenceDialog(RecurrenceDialog dlg, UIElement owner, SchedulerFormatter formatter)
    {
      if (formatter != null && formatter.RecurrenceDialogStyle != null)
      {
        dlg.Style = formatter.RecurrenceDialogStyle;
      }
      if (HasAllWindowsPermission())
      {
        Window wnd = new Window();
        //wnd.Title = "Appointment Recurrence";
        wnd.ResizeMode = ResizeMode.NoResize;
        wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wnd.Content = dlg;
        //wnd.Width = 510;
        //wnd.Height = 450;
        wnd.SizeToContent = SizeToContent.WidthAndHeight;
        wnd.Closed += new EventHandler(RecurrenceWindow_Closed);
        wnd.Owner = VisualTreeUtils.FindAncestor<Window>(owner);
        wnd.ShowDialog();
      }
      else
      {
        Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(owner);
        Grid grid = VisualTreeUtils.FindAncestor<Grid>(scheduler);
        if (grid != null)
        {
          dlg.Height = 430;
          ChildWindow child = new ChildWindow();
          child.BackgroundBrush = new SolidColorBrush(); // To avoid double layers of transparency.
          child.WindowWidth = 510;
          child.WindowHeight = 450;
          child.Content = dlg;

          dlg.HorizontalAlignment = HorizontalAlignment.Center;
          dlg.VerticalAlignment = VerticalAlignment.Center;
          dlg.Closed += new RoutedEventHandler(RecurrenceDialog_Closed);

          grid.Children.Add(child);
        }
      }
    }

    private static void RecurrenceDialog_Closed(object sender, RoutedEventArgs e)
    {
      RecurrenceDialog dlg = sender as RecurrenceDialog;
      dlg.Closed -= new RoutedEventHandler(RecurrenceDialog_Closed);

      ChildWindow child = VisualTreeUtils.FindAncestor<ChildWindow>(dlg);
      Grid grid = child.Parent as Grid;
      grid.Children.Remove(child);
    }

    private static void RecurrenceWindow_Closed(object sender, EventArgs e)
    {
      Window window = sender as Window;
      RecurrenceDialog dlg = window.Content as RecurrenceDialog;
      if (dlg != null)
      {
        dlg.OnClosed();
      }
    }

    #endregion // RecurrenceDialog

    #region DeleteRecurrenceDialog

    internal static void ShowDeleteRecurrenceDialog(DeleteRecurrenceDialog dlg, UIElement owner, SchedulerFormatter formatter)
    {
      if (formatter != null && formatter.DeleteRecurrenceDialogStyle != null)
      {
        dlg.Style = formatter.DeleteRecurrenceDialogStyle;
      }
      if (HasAllWindowsPermission())
      {
        Window wnd = new Window();
        //wnd.Title = "Confirm Delete";
        wnd.ResizeMode = ResizeMode.NoResize;
        wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        wnd.Content = dlg;
        //wnd.Width = 270;
        //wnd.Height = 185;
        wnd.SizeToContent = SizeToContent.WidthAndHeight;
        wnd.Closed += new EventHandler(DeleteRecurrenceWindow_Closed);
        wnd.Owner = VisualTreeUtils.FindAncestor<Window>(owner);
        wnd.ShowDialog();
      }
      else
      {
        Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(owner);
        Grid grid = VisualTreeUtils.FindAncestor<Grid>(scheduler);
        if (grid != null)
        {
          dlg.Height = 145;
          ChildWindow child = new ChildWindow();
          child.WindowWidth = 270;
          child.WindowHeight = 145;
          child.Content = dlg;

          dlg.HorizontalAlignment = HorizontalAlignment.Center;
          dlg.VerticalAlignment = VerticalAlignment.Center;
          dlg.Closed += new RoutedEventHandler(DeleteRecurrenceDialog_Closed);

          grid.Children.Add(child);
        }
      }
    }

    private static void DeleteRecurrenceDialog_Closed(object sender, RoutedEventArgs e)
    {
      DeleteRecurrenceDialog dlg = sender as DeleteRecurrenceDialog;
      dlg.Closed -= new RoutedEventHandler(DeleteRecurrenceDialog_Closed);

      ChildWindow child = VisualTreeUtils.FindAncestor<ChildWindow>(dlg);
      Grid grid = child.Parent as Grid;
      grid.Children.Remove(child);
    }

    private static void DeleteRecurrenceWindow_Closed(object sender, EventArgs e)
    {
      Window window = sender as Window;
      DeleteRecurrenceDialog dlg = window.Content as DeleteRecurrenceDialog;
      if (dlg != null)
      {
        dlg.OnClosed();
      }
    }

    #endregion // DeleteRecurrenceDialog

    private static bool HasAllWindowsPermission()
    {
      UIPermission p = new UIPermission(UIPermissionWindow.AllWindows);
      try
      {
        p.Demand();
        return true;
      }
      catch (SecurityException)
      {
        return false;
      }
    }
  }
}
