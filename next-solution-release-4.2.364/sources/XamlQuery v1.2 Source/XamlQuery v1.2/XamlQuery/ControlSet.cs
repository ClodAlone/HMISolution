//ControlSet.cs

/*
 * XamlQuery Silverlight Library v1.2
 * http://XamlQuery.codeplex.com/
 * 
 * Author: cincoutprabu (cincoutprabu@gmail.com)
 * Copyright 2010 cincoutprabu. All rights reserved.
 * 
 * Released under Microsoft Public License.
 * http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 *
 * Date: 2010-Sep-10 06:00 IST
 * Revision: 1
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace XamlQuery
{
    /// <summary>
    /// ControlSet class is used to hold the set of controls returned by XamlQuery.
    /// ControlSet provides numerous helper methods to carry out a complex task very easily.
    /// </summary>
    public class ControlSet : List<DependencyObject>
    {
        #region Filter-Related-Methods

        /// <summary>
        /// Finds controls of specified type.
        /// </summary>
        /// <typeparam name="T">The required type of controls.</typeparam>
        /// <returns>ControlSet containing the matched controls.</returns>
        public ControlSet FilterByType<T>()
        {
            ControlSet filteredControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is T)
                {
                    filteredControls.Add(control);
                }
            }

            return (filteredControls);
        }

        /// <summary>
        /// Finds controls whose type is any one of specified types.
        /// </summary>
        /// <param name="types">The list of required types of controls.</param>
        /// <returns>ControlSet containing the matched controls.</returns>
        public ControlSet FilterByTypes(List<Type> types)
        {
            ControlSet filteredControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (types.Contains(control.GetType()))
                {
                    filteredControls.Add(control);
                }
            }

            return (filteredControls);
        }

        /// <summary>
        /// Finds controls whose property value is equal to a given value.
        /// </summary>
        /// <param name="property">Dependency property whose value is used for matching.</param>
        /// <param name="value">The required value of the given dependency property. The controls whose property value is equal to this value are chosen.</param>
        /// <returns>ControlSet containing the matched controls.</returns>
        public ControlSet FilterByProperty(DependencyProperty property, object value)
        {
            return (FilterByProperty(property, value, FilterType.Equals));
        }

        /// <summary>
        /// Finds controls whose property value matches a specified criteria. The criteria is defined by the "filterType" parameter. For "Equal" and "NotEqual" filters, the "Equals()" method of the property is used for comparison and for other filters ("StartsWith", "EndsWith", "Contains") the string representation of the property value is used for comparison.
        /// </summary>
        /// <param name="property">Dependency property whose value is used for matching.</param>
        /// <param name="value">The matching value of the given dependency property. This value is matched with all controls in the ControlSet.</param>
        /// <param name="filterType">The filter-type enumeration value that defines the search criteria.</param>
        /// <returns>ControlSet containing the matched controls.</returns>
        public ControlSet FilterByProperty(DependencyProperty property, object value, FilterType filterType)
        {
            ControlSet filteredControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                object propertyValue = control.GetValue(property);
                if (CheckPropertyValue(propertyValue, value, filterType))
                {
                    filteredControls.Add(control);
                }
            }

            return (filteredControls);
        }

        /// <summary>
        /// Removes the given list of controls and returns the resultant ControlSet.
        /// </summary>
        /// <param name="controlSet">The ControlSet containing the controls to be removed.</param>
        /// <returns>ControlSet after removing the specified controls.</returns>
        public ControlSet Not(ControlSet controlSet)
        {
            ControlSet allControls = new ControlSet();
            allControls.AddRange(this);

            //remove the subset
            foreach (DependencyObject control in controlSet)
            {
                allControls.Remove(control);
            }

            return (allControls);
        }

        /// <summary>
        /// Removes controls of specified type from the ControlSet.
        /// </summary>
        /// <typeparam name="T">The type of the controls to be removed.</typeparam>
        /// <returns>Returns the removed controls.</returns>
        public ControlSet RemoveByType<T>()
        {
            //find controls to be removed
            ControlSet removedControls = new ControlSet();
            for (int controlIndex = 0; controlIndex < this.Count; controlIndex++)
            {
                if (this[controlIndex] is T)
                {
                    removedControls.Add(this[controlIndex]);
                }
            }

            //remove the controls
            foreach (DependencyObject control in removedControls)
            {
                this.Remove(control);
            }

            return (removedControls);
        }

        /// <summary>
        /// Removes the controls whose type is any one of specified types.
        /// </summary>
        /// <param name="types">The list of control types to be removed.</param>
        /// <returns>Returns the removed controls.</returns>
        public ControlSet RemoveByTypes(List<Type> types)
        {
            //find controls to be removed
            ControlSet removedControls = new ControlSet();
            for (int controlIndex = 0; controlIndex < this.Count; controlIndex++)
            {
                if (types.Contains(this[controlIndex].GetType()))
                {
                    removedControls.Add(this[controlIndex]);
                }
            }

            //remove the controls
            foreach (DependencyObject control in removedControls)
            {
                this.Remove(control);
            }

            return (removedControls);
        }

        #region Filtering-by-Index-Methods

        /// <summary>
        /// Gets the controls at even indices. Since index is zero-based, it returns first-control, third-control, and so on.
        /// </summary>
        /// <returns>ControlSet containing the controls at even indices.</returns>
        public ControlSet Even()
        {
            ControlSet matchedControls = new ControlSet();

            int index = 0;
            foreach (DependencyObject control in this)
            {
                if (index % 2 == 0) matchedControls.Add(control);
                index += 1;
            }

            return (matchedControls);
        }

        /// <summary>
        /// Gets the controls at odd indices. Since index is zero-based, it returns second-control, fourth-control, and so on.
        /// </summary>
        /// <returns>ControlSet containing the controls at odd indices.</returns>
        public ControlSet Odd()
        {
            ControlSet matchedControls = new ControlSet();

            int index = 0;
            foreach (DependencyObject control in this)
            {
                if (index % 2 != 0) matchedControls.Add(control);
                index += 1;
            }

            return (matchedControls);
        }

        /// <summary>
        /// Gets the controls at index greater than the specified index. The index is zero-based and so it returns (i+1)'th element to (n-1)'th element, where i is index and n is number-of-controls. The i'th element is not included in the resultant ControlSet.
        /// </summary>
        /// <param name="index">The zero-based reference index.</param>
        /// <returns>ControlSet containing the controls in the range defined by index.</returns>
        public ControlSet Gt(int index)
        {
            ControlSet matchedControls = new ControlSet();
            matchedControls.AddRange(this.GetRange(index + 1, this.Count - (index + 1)));
            return (matchedControls);
        }

        /// <summary>
        /// Gets the controls at index lesser than the specified index. The index is zero-based and so it returns 0'th element to (i-1)'th element, where i is the index. The i'th element is not included in the resultant ControlSet.
        /// </summary>
        /// <param name="index">The zero-based reference index.</param>
        /// <returns>ControlSet containing the controls in the range defined by index.</returns>
        public ControlSet Lt(int index)
        {
            ControlSet matchedControls = new ControlSet();
            matchedControls.AddRange(this.GetRange(0, index));
            return (matchedControls);
        }

        #endregion

        #region Filtering-by-Property-Value-Methods

        /// <summary>
        /// Finds controls that are enabled in the user-interface (checks IsEnabled property of dependency objects extended from System.Windows.Controls.Control).
        /// </summary>
        /// <returns>ControlSet containing the enabled controls.</returns>
        public ControlSet Enabled()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is Control && ((Control)control).IsEnabled == true)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        /// <summary>
        /// Finds controls that are disabled in the user-interface (checks IsEnabled property of dependency objects extended from System.Windows.Controls.Control).
        /// </summary>
        /// <returns>ControlSet containing the disabled controls.</returns>
        public ControlSet Disabled()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is Control && ((Control)control).IsEnabled == false)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        /// <summary>
        /// Finds controls that are visible in the user-interface (checks Visibility property of dependency objects extended from System.Windows.UIElement).
        /// </summary>
        /// <returns>ControlSet containing the visible controls.</returns>
        public ControlSet Visible()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is UIElement && ((UIElement)control).Visibility == Visibility.Visible)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        /// <summary>
        /// Finds controls that are invisible in the user-interface (checks Visibility property of dependency objects extended from System.Windows.UIElement).
        /// </summary>
        /// <returns>ControlSet containing the invisible controls.</returns>
        public ControlSet Invisible()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is UIElement && ((UIElement)control).Visibility == Visibility.Collapsed)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        /// <summary>
        /// Finds controls that are marked as checked in the user-interface (checks IsChecked property of dependency objects extended from System.Windows.Controls.Primitives.ToggleButton).
        /// </summary>
        /// <returns>ControlSet containing the checked controls.</returns>
        public ControlSet Checked()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is ToggleButton && ((ToggleButton)control).IsChecked == true)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        /// <summary>
        /// Finds controls that are not marked as checked in the user-interface (checks IsChecked property of dependency objects extended from System.Windows.Controls.Primitives.ToggleButton).
        /// </summary>
        /// <returns>ControlSet containing the unchecked controls.</returns>
        public ControlSet Unchecked()
        {
            ControlSet matchedControls = new ControlSet();

            foreach (DependencyObject control in this)
            {
                if (control is ToggleButton && ((ToggleButton)control).IsChecked == false)
                {
                    matchedControls.Add(control);
                }
            }

            return (matchedControls);
        }

        #endregion

        #endregion

        #region Value-Related-Methods

        /// <summary>
        /// Sets a value to specified-property of all controls.
        /// </summary>
        /// <param name="property">The destination property to which the value is assigned.</param>
        /// <param name="value">The value to be assigned to specified dependency property.</param>
        public void SetValue(DependencyProperty property, object value)
        {
            foreach (DependencyObject control in this)
            {
                control.SetValue(property, value);
            }
        }

        /// <summary>
        /// Gets value of specified-property of all controls. The values of all controls are returned as a List object.
        /// </summary>
        /// <param name="property">Dependency property whose value is fetched.</param>
        /// <returns>A List object containing the specified property's value of all controls.</returns>
        public List<object> GetValue(DependencyProperty property)
        {
            List<object> valueList = new List<object>();
            foreach (DependencyObject control in this)
            {
                valueList.Add(control.GetValue(property));
            }
            return (valueList);
        }

        /// <summary>
        /// Attaches a binding to specified-property of all controls.
        /// </summary>
        /// <param name="property">The destination property where the binding is established.</param>
        /// <param name="binding">The binding object to be attached.</param>
        public void SetBinding(DependencyProperty property, Binding binding)
        {
            foreach (DependencyObject control in this)
            {
                if (control is FrameworkElement)
                {
                    ((FrameworkElement)control).SetBinding(property, binding);
                }
            }
        }

        /// <summary>
        /// Clears the local value of a property of all controls.
        /// </summary>
        /// <param name="property">Dependency property whose value is cleared.</param>
        public void ClearValue(DependencyProperty property)
        {
            foreach (DependencyObject control in this)
            {
                control.ClearValue(property);
            }
        }

        /// <summary>
        /// Gets the current value of first control. For TextBlock and TextBox, it returns value of Text property; for Selector controls (like ListBox, ComboBox), it returns value of SelectedItem property.
        /// </summary>
        /// <returns>The current value of first control (according to the control type).</returns>
        public string Val()
        {
            if (this.Count > 0)
            {
                DependencyObject first = this.First();
                if (first is TextBlock)
                {
                    return (((TextBlock)first).Text);
                }
                else if (first is TextBox)
                {
                    return (((TextBox)first).Text);
                }
                if (first is Selector)
                {
                    object selectedItem = ((Selector)first).SelectedItem;
                    if (selectedItem != null)
                    {
                        return (selectedItem.ToString());
                    }
                }
            }

            return (string.Empty);
        }

        /// <summary>
        /// Attaches any arbitrary object to all controls. Any number of objects can be attached and each object is represented by a key. XamlQuery maintains an internal dependency-object store where these objects are stored. This is similar to having multiple Tag properties for a Silverlight control.
        /// </summary>
        /// <param name="key">A string key that uniquely identifies an object attached using this method.</param>
        /// <param name="value">The arbitrary object of any type which will be attached to all controls.</param>
        public void Data(string key, object value)
        {
            foreach (DependencyObject control in this)
            {
                DependencyObjectStore.SetData(control, key, value);
            }
        }

        /// <summary>
        /// Gets the arbitrary objects from dependency-object store identified by the given key.
        /// </summary>
        /// <param name="key">The string key that identifies the objects to be retrieved from the store.</param>
        /// <returns>A List containing the arbitrary objects attached to all controls.</returns>
        public List<object> Data(string key)
        {
            List<object> dataList = new List<object>();
            foreach (DependencyObject control in this)
            {
                dataList.Add(DependencyObjectStore.GetData(control, key));
            }
            return (dataList);
        }

        #endregion

        #region Visibility-Related-Methods

        /// <summary>
        /// Hides the controls (without animation). The Visibility property will be set to Collapsed.
        /// </summary>
        public void Hide()
        {
            SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
        }

        /// <summary>
        /// Displays the controls (without animation). The Visibility property is will be set to Visible.
        /// </summary>
        public void Show()
        {
            SetValue(UIElement.VisibilityProperty, Visibility.Visible);
        }

        /// <summary>
        /// Alternates the visibility of controls (without animation). The Visibility property is swapped between Collapsed and Visible.
        /// </summary>
        public void Toggle()
        {
            foreach (DependencyObject control in this)
            {
                Visibility visibility = (Visibility)control.GetValue(UIElement.VisibilityProperty);
                control.SetValue(UIElement.VisibilityProperty, visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Hides the conrols by fading them to transparent. The fading process is animated in 'Normal' speed.
        /// </summary>
        public void FadeOut()
        {
            FadeOut(Speed.Normal);
        }

        /// <summary>
        /// Hides the controls by fading them to transparent. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for fading animation).</param>
        public void FadeOut(Speed speed)
        {
            FadeTo((int)speed, 0.0);
        }

        /// <summary>
        /// Hides the controls by fading them to transparent. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the fading animation.</param>
        public void FadeOut(int duration)
        {
            FadeTo(duration, 0.0);
        }

        /// <summary>
        /// Displays the controls by fading them to opaque. The fading process is animated in 'Normal' speed.
        /// </summary>
        public void FadeIn()
        {
            FadeIn(Speed.Normal);
        }

        /// <summary>
        /// Displays the controls by fading them to opaque. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for fading animation).</param>
        public void FadeIn(Speed speed)
        {
            FadeTo((int)speed, 1.0);
        }

        /// <summary>
        /// Displays the controls by fading them to opaque. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the fading animation.</param>
        public void FadeIn(int duration)
        {
            FadeTo(duration, 1.0);
        }

        /// <summary>
        /// Sets transparency (opacity) level to specified value. The fading process is animated in 'Normal' speed.
        /// </summary>
        /// <param name="targetOpacity">Required level of transparency between 0.0 and 1.0.</param>
        public void FadeTo(double targetOpacity)
        {
            FadeTo(Speed.Normal, targetOpacity);
        }

        /// <summary>
        /// Sets transparency (opacity) level to specified value. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for fading animation).</param>
        /// <param name="targetOpacity">Required level of transparency between 0.0 and 1.0.</param>
        public void FadeTo(Speed speed, double targetOpacity)
        {
            FadeTo((int)speed, targetOpacity);
        }

        /// <summary>
        /// Sets transparency (opacity) level to specified value. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the fading animation.</param>
        /// <param name="targetOpacity">Required level of transparency between 0.0 and 1.0.</param>
        public void FadeTo(int duration, double targetOpacity)
        {
            foreach (DependencyObject control in this)
            {
                double fromOpacity = ((UIElement)control).Opacity;
                if (fromOpacity != targetOpacity)
                {
                    DoDoubleAnimation(control, UIElement.OpacityProperty, duration, fromOpacity, targetOpacity, null);
                }
            }
        }

        /// <summary>
        /// Alternates the visibility of controls between transparent and opaque. The fading process is animated in 'Normal' speed.
        /// </summary>
        public void FadeToggle()
        {
            FadeToggle(Speed.Normal);
        }

        /// <summary>
        /// Alternates the visibility of controls between transparent and opaque. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for fading animation).</param>
        public void FadeToggle(Speed speed)
        {
            FadeToggle((int)speed);
        }

        /// <summary>
        /// Alternates the visibility of controls between transparent and opaque. The fading process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the fading animation.</param>
        public void FadeToggle(int duration)
        {
            foreach (DependencyObject control in this)
            {
                double opacity = ((UIElement)control).Opacity;
                if (opacity <= 0.0)
                {
                    ControlSet.Create(control).FadeIn(duration);
                }
                else
                {
                    ControlSet.Create(control).FadeOut(duration);
                }
            }
        }

        #region Slide-Related-Methods

        /// <summary>
        /// Hides the controls by gradually decreasing the height. The sliding process is animated in 'Normal' speed.
        /// </summary>
        public void SlideUp()
        {
            SlideUp(Speed.Normal);
        }

        /// <summary>
        /// Hides the controls by gradually decreasing the height. The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for sliding animation).</param>
        public void SlideUp(Speed speed)
        {
            SlideUp((int)speed);
        }

        /// <summary>
        /// Hides the controls by gradually decreasing the height. The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the sliding animation.</param>
        public void SlideUp(int duration)
        {
            foreach (DependencyObject control in this)
            {
                if (control is FrameworkElement)
                {
                    double currentHeight = ((FrameworkElement)control).ActualHeight;
                    DependencyObjectStore.SetData(control, "OriginalHeight", currentHeight);
                    DoDoubleAnimation(control, FrameworkElement.HeightProperty, duration, currentHeight, 0, null);
                }
            }
        }

        /// <summary>
        /// Displays the controls by gradually increasing the height to original height before sliding up. The sliding process is animated in 'Normal' speed.
        /// </summary>
        public void SlideDown()
        {
            SlideDown(Speed.Normal);
        }

        /// <summary>
        /// Displays the controls by gradually increasing the height to original height before sliding up. The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for sliding animation).</param>
        public void SlideDown(Speed speed)
        {
            SlideDown((int)speed);
        }

        /// <summary>
        /// Displays the controls by gradually increasing the height to original height before sliding up. The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the sliding animation.</param>
        public void SlideDown(int duration)
        {
            foreach (DependencyObject control in this)
            {
                if (control is FrameworkElement)
                {
                    object originalHeightObject = DependencyObjectStore.GetData(control, "OriginalHeight");
                    if (originalHeightObject != null)
                    {
                        double originalHeight = (double)originalHeightObject;
                        DoDoubleAnimation(control, FrameworkElement.HeightProperty, duration, 0, originalHeight, null);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the height of controls to a specified value. The sliding process in animated in 'Normal' speed.
        /// </summary>
        /// <param name="targetHeight">The required height.</param>
        public void SlideTo(double targetHeight)
        {
            SlideTo(Speed.Normal, targetHeight);
        }

        /// <summary>
        /// Sets the height of controls to a specified value. The sliding process in animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for sliding animation).</param>
        /// <param name="targetHeight">The required height.</param>
        public void SlideTo(Speed speed, double targetHeight)
        {
            SlideTo((int)speed, targetHeight);
        }

        /// <summary>
        /// Sets the height of controls to a specified value. The sliding process in animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the sliding animation.</param>
        /// <param name="targetHeight">The required height.</param>
        public void SlideTo(int duration, double targetHeight)
        {
            foreach (DependencyObject control in this)
            {
                if (control is FrameworkElement)
                {
                    double currentHeight = ((FrameworkElement)control).ActualHeight;
                    DoDoubleAnimation(control, FrameworkElement.HeightProperty, duration, currentHeight, targetHeight, null);
                }
            }
        }

        /// <summary>
        /// Alternates the height of controls between zero and original height (before sliding up). The sliding process is animated in 'Normal' speed.
        /// </summary>
        public void SlideToggle()
        {
            SlideToggle(Speed.Normal);
        }

        /// <summary>
        /// Alternates the height of controls between zero and original height (before sliding up). The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="speed">Any one of 'Speed' enumeration constants (for sliding animation).</param>
        public void SlideToggle(Speed speed)
        {
            SlideToggle((int)speed);
        }

        /// <summary>
        /// Alternates the height of controls between zero and original height (before sliding up). The sliding process is animated in a specified speed.
        /// </summary>
        /// <param name="duration">Duration (in milliseconds) for the sliding animation.</param>
        public void SlideToggle(int duration)
        {
            foreach (DependencyObject control in this)
            {
                double currentHeight = ((FrameworkElement)control).ActualHeight;
                if (currentHeight <= 0.0)
                {
                    ControlSet.Create(control).SlideDown(duration);
                }
                else
                {
                    ControlSet.Create(control).SlideUp(duration);
                }
            }
        }

        #endregion

        #endregion

        #region Animation-Related-Methods

        /// <summary>
        /// Performs a custom animation on the specified property of all controls in the ControlSet. The duration of animation and begin/end property values can be set via parameters.
        /// </summary>
        /// <param name="property">Dependency property whose value changes gradually during animation.</param>
        /// <param name="duration">Duration (in milliseconds) for the animation.</param>
        /// <param name="from">Begin value of property when the animation starts.</param>
        /// <param name="to">End value of property when the animation ends.</param>
        public void Animate(DependencyProperty property, int duration, double from, double to)
        {
            foreach (DependencyObject control in this)
            {
                DoDoubleAnimation(control, property, duration, from, to, null);
            }
        }

        /// <summary>
        /// Performs a custom animation on the specified property of all controls in the ControlSet. The duration of animation and begin/end property values can be set via parameters.
        /// </summary>
        /// <param name="property">Dependency property whose value changes gradually during animation.</param>
        /// <param name="duration">Duration (in milliseconds) for the animation.</param>
        /// <param name="from">Begin value of property when the animation starts.</param>
        /// <param name="to">End value of property when the animation ends.</param>
        /// <param name="callback">The method to be invoked after the animation.</param>
        public void Animate(DependencyProperty property, int duration, double from, double to, EventHandler callback)
        {
            foreach (DependencyObject control in this)
            {
                DoDoubleAnimation(control, property, duration, from, to, callback);
            }
        }

        #endregion

        #region Event-Related-Methods

        /// <summary>
        /// Attaches an event handler delegate/method to specified routed-event of all controls in the ControlSet.
        /// </summary>
        /// <param name="routedEvent">The routed event to be handled.</param>
        /// <param name="handler">The event-handler method which is invoked when the event occurs.</param>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler)
        {
            AddHandler(routedEvent, handler, false);
        }

        /// <summary>
        /// Attaches an event handler delegate/method to specified routed-event of all controls in the ControlSet.
        /// </summary>
        /// <param name="routedEvent">The routed event to be handled.</param>
        /// <param name="handler">The event-handler method which is invoked when the event occurs.</param>
        /// <param name="handledEventsToo">Boolean that indicates whether to rehandle the event even if it is marked handled.</param>
        public void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            foreach (DependencyObject control in this)
            {
                if (control is UIElement)
                {
                    ((UIElement)control).AddHandler(routedEvent, handler, handledEventsToo);
                }
            }
        }

        /// <summary>
        /// Removes the specified routed event handler from all controls in the ControlSet.
        /// </summary>
        /// <param name="routedEvent">The routed event for which the handler is attached.</param>
        /// <param name="handler">The event handler to be removed.</param>
        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            foreach (DependencyObject control in this)
            {
                if (control is UIElement)
                {
                    ((UIElement)control).RemoveHandler(routedEvent, handler);
                }
            }
        }

        /// <summary>
        /// Attaches an event-handler to the event specified by EventType enumeration. Events attached using this method, can be invoked/fired manually without the actual event occurs.
        /// </summary>
        /// <param name="eventType">Any one of EventType enumeration constants, that indicates the event to be handled.</param>
        /// <param name="handler">The event-handler method which is invoked when the event occurs or manually fired. The signature of the event-handler method should match the event. For example, a mouse event handler cannot be attached to a keyboard event.</param>
        public void Bind(EventType eventType, Delegate handler)
        {
            ValidateEventDelegate(eventType, handler);

            foreach (DependencyObject control in this)
            {
                bool bound = false;

                //check System.Windows.UIElement events
                if (control is UIElement)
                {
                    UIElement element = (UIElement)control;
                    switch (eventType)
                    {
                        case EventType.GotFocus:
                            element.GotFocus += (RoutedEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.LostFocus:
                            element.LostFocus += (RoutedEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.KeyUp:
                            element.KeyUp += (KeyEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.KeyDown:
                            element.KeyDown += (KeyEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseEnter:
                            element.MouseEnter += (MouseEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseLeave:
                            element.MouseLeave += (MouseEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseMove:
                            element.MouseMove += (MouseEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.LostMouseCapture:
                            element.LostMouseCapture += (MouseEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseLeftButtonDown:
                            element.MouseLeftButtonDown += (MouseButtonEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseLeftButtonUp:
                            element.MouseLeftButtonUp += (MouseButtonEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.MouseWheel:
                            element.MouseWheel += (MouseWheelEventHandler)handler;
                            bound = true;
                            break;
                    }
                }

                //check System.Windows.FrameworkElement events
                else if (control is FrameworkElement)
                {
                    FrameworkElement element = (FrameworkElement)control;
                    switch (eventType)
                    {
                        case EventType.BindingValidationError:
                            element.BindingValidationError += (EventHandler<ValidationErrorEventArgs>)handler;
                            bound = true;
                            break;
                        case EventType.Loaded:
                            element.Loaded += (RoutedEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.LayoutUpdated:
                            element.LayoutUpdated += (EventHandler)handler;
                            bound = true;
                            break;
                        case EventType.SizeChanged:
                            element.SizeChanged += (SizeChangedEventHandler)handler;
                            bound = true;
                            break;
                    }
                }

                //check System.Windows.Controls.Control events
                else if (control is Control)
                {
                    Control controlObject = (Control)control;
                    switch (eventType)
                    {
                        case EventType.IsEnabledChanged:
                            controlObject.IsEnabledChanged += (DependencyPropertyChangedEventHandler)handler;
                            bound = true;
                            break;
                    }
                }

                //check System.Windows.Controls.TextBox events
                else if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    switch (eventType)
                    {
                        case EventType.TextChanged:
                            textBox.TextChanged += (TextChangedEventHandler)handler;
                            bound = true;
                            break;
                        case EventType.TextSelectionChanged:
                            textBox.SelectionChanged += (RoutedEventHandler)handler;
                            bound = true;
                            break;
                    }
                }

                //check System.Windows.Controls.Primitives.Selector events
                else if (control is Selector)
                {
                    Selector selector = (Selector)control;
                    switch (eventType)
                    {
                        case EventType.SelectionChanged:
                            selector.SelectionChanged += (SelectionChangedEventHandler)handler;
                            bound = true;
                            break;
                    }
                }

                if (bound)
                {
                    SaveEvent(control, eventType, handler);
                }
            }
        }

        /// <summary>
        /// Fires the event. This method manually invokes the event-handler method without the actual event occurs.
        /// </summary>
        /// <param name="eventType">Any one of EventType enumeration constants, that indicates the event to be fired.</param>
        public void Trigger(EventType eventType)
        {
            foreach (DependencyObject control in this)
            {
                FireEvent(control, eventType);
            }
        }

        #region Event-Helper-Methods

        /// <summary>
        /// Fires the GotFocus event.
        /// </summary>
        public void GotFocus()
        {
            Trigger(EventType.GotFocus);
        }

        /// <summary>
        /// Attaches an event-handler to GotFocus event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when GotFocus event occurs or manually fired.</param>
        public void GotFocus(Delegate handler)
        {
            Bind(EventType.GotFocus, handler);
        }

        /// <summary>
        /// Fires the LostFocus event.
        /// </summary>
        public void LostFocus()
        {
            Trigger(EventType.LostFocus);
        }

        /// <summary>
        /// Attaches an event-handler to LostFocus event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when LostFocus event occurs or manually fired.</param>
        public void LostFocus(Delegate handler)
        {
            Bind(EventType.LostFocus, handler);
        }

        /// <summary>
        /// Fires the KeyDown event.
        /// </summary>
        public void KeyDown()
        {
            Trigger(EventType.KeyDown);
        }

        /// <summary>
        /// Attaches an event-handler to KeyDown event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when KeyDown event occurs or manually fired.</param>
        public void KeyDown(Delegate handler)
        {
            Bind(EventType.KeyDown, handler);
        }

        /// <summary>
        /// Fires the KeyUp event.
        /// </summary>
        public void KeyUp()
        {
            Trigger(EventType.KeyUp);
        }

        /// <summary>
        /// Attaches an event-handler to KeyUp event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when KeyUp event occurs or manually fired.</param>
        public void KeyUp(Delegate handler)
        {
            Bind(EventType.KeyUp, handler);
        }

        /// <summary>
        /// Fires the MouseEnter event.
        /// </summary>
        public void MouseEnter()
        {
            Trigger(EventType.MouseEnter);
        }

        /// <summary>
        /// Attaches an event-handler to MouseEnter event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseEnter event occurs or manually fired.</param>
        public void MouseEnter(Delegate handler)
        {
            Bind(EventType.MouseEnter, handler);
        }

        /// <summary>
        /// Fires the MouseLeave event.
        /// </summary>
        public void MouseLeave()
        {
            Trigger(EventType.MouseLeave);
        }

        /// <summary>
        /// Attaches an event-handler to MouseLeave event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseLeave event occurs or manually fired.</param>
        public void MouseLeave(Delegate handler)
        {
            Bind(EventType.MouseLeave, handler);
        }

        /// <summary>
        /// Fires the MouseMove event.
        /// </summary>
        public void MouseMove()
        {
            Trigger(EventType.MouseMove);
        }

        /// <summary>
        /// Attaches an event-handler to MouseMove event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseMove event occurs or manually fired.</param>
        public void MouseMove(Delegate handler)
        {
            Bind(EventType.MouseMove, handler);
        }

        /// <summary>
        /// Fires the LostMouseCapture event.
        /// </summary>
        public void LostMouseCapture()
        {
            Trigger(EventType.LostMouseCapture);
        }

        /// <summary>
        /// Attaches an event-handler to LostMouseCapture event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when LostMouseCapture event occurs or manually fired.</param>
        public void LostMouseCapture(Delegate handler)
        {
            Bind(EventType.LostMouseCapture, handler);
        }

        /// <summary>
        /// Fires the MouseLeftButtonDown event.
        /// </summary>
        public void MouseLeftButtonDown()
        {
            Trigger(EventType.MouseLeftButtonDown);
        }

        /// <summary>
        /// Attaches an event-handler to MouseLeftButtonDown event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseLeftButtonDown event occurs or manually fired.</param>
        public void MouseLeftButtonDown(Delegate handler)
        {
            Bind(EventType.MouseLeftButtonDown, handler);
        }

        /// <summary>
        /// Fires the MouseLeftButtonUp event.
        /// </summary>
        public void MouseLeftButtonUp()
        {
            Trigger(EventType.MouseLeftButtonUp);
        }

        /// <summary>
        /// Attaches an event-handler to MouseLeftButtonUp event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseLeftButtonUp event occurs or manually fired.</param>
        public void MouseLeftButtonUp(Delegate handler)
        {
            Bind(EventType.MouseLeftButtonUp, handler);
        }

        /// <summary>
        /// Fires the MouseWheel event.
        /// </summary>
        public void MouseWheel()
        {
            Trigger(EventType.MouseWheel);
        }

        /// <summary>
        /// Attaches an event-handler to MouseWheel event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when MouseWheel event occurs or manually fired.</param>
        public void MouseWheel(Delegate handler)
        {
            Bind(EventType.MouseWheel, handler);
        }

        /// <summary>
        /// Fires the BindingValidationError event.
        /// </summary>
        public void BindingValidationError()
        {
            Trigger(EventType.BindingValidationError);
        }

        /// <summary>
        /// Attaches an event-handler to BindingValidationError event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when BindingValidationError event occurs or manually fired.</param>
        public void BindingValidationError(Delegate handler)
        {
            Bind(EventType.BindingValidationError, handler);
        }

        /// <summary>
        /// Fires the Loaded event.
        /// </summary>
        public void Loaded()
        {
            Trigger(EventType.Loaded);
        }

        /// <summary>
        /// Attaches an event-handler to Loaded event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when Loaded event occurs or manually fired.</param>
        public void Loaded(Delegate handler)
        {
            Bind(EventType.Loaded, handler);
        }

        /// <summary>
        /// Fires the LayoutUpdated event.
        /// </summary>
        public void LayoutUpdated()
        {
            Trigger(EventType.LayoutUpdated);
        }

        /// <summary>
        /// Attaches an event-handler to LayoutUpdated event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when LayoutUpdated event occurs or manually fired.</param>
        public void LayoutUpdated(Delegate handler)
        {
            Bind(EventType.LayoutUpdated, handler);
        }

        /// <summary>
        /// Fires the SizeChanged event.
        /// </summary>
        public void SizeChanged()
        {
            Trigger(EventType.SizeChanged);
        }

        /// <summary>
        /// Attaches an event-handler to SizeChanged event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when SizeChanged event occurs or manually fired.</param>
        public void SizeChanged(Delegate handler)
        {
            Bind(EventType.SizeChanged, handler);
        }

        /// <summary>
        /// Fires the IsEnabledChanged event.
        /// </summary>
        public void IsEnabledChanged()
        {
            Trigger(EventType.IsEnabledChanged);
        }

        /// <summary>
        /// Attaches an event-handler to IsEnabledChanged event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when IsEnabledChanged event occurs or manually fired.</param>
        public void IsEnabledChanged(Delegate handler)
        {
            Bind(EventType.IsEnabledChanged, handler);
        }

        /// <summary>
        /// Fires the TextChanged event.
        /// </summary>
        public void TextChanged()
        {
            Trigger(EventType.TextChanged);
        }

        /// <summary>
        /// Attaches an event-handler to TextChanged event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when TextChanged event occurs or manually fired.</param>
        public void TextChanged(Delegate handler)
        {
            Bind(EventType.TextChanged, handler);
        }

        /// <summary>
        /// Fires the TextSelectionChanged event.
        /// </summary>
        public void TextSelectionChanged()
        {
            Trigger(EventType.TextSelectionChanged);
        }

        /// <summary>
        /// Attaches an event-handler to TextSelectionChanged event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when TextSelectionChanged event occurs or manually fired.</param>
        public void TextSelectionChanged(Delegate handler)
        {
            Bind(EventType.TextSelectionChanged, handler);
        }

        /// <summary>
        /// Fires the SelectionChanged event.
        /// </summary>
        public void SelectionChanged()
        {
            Trigger(EventType.SelectionChanged);
        }

        /// <summary>
        /// Attaches an event-handler to SelectionChanged event.
        /// </summary>
        /// <param name="handler">The event-handler method which is invoked when SelectionChanged event occurs or manually fired.</param>
        public void SelectionChanged(Delegate handler)
        {
            Bind(EventType.SelectionChanged, handler);
        }

        #endregion

        #endregion

        #region Layout-Related-Methods

        /// <summary>
        /// Detaches/removes/deletes all the controls from the rendered Silverlight output. Controls which are direct children of container controls (like Panel, Grid, StackPanel, WrapPanel, etc) can be detached.
        /// </summary>
        public void Detach()
        {
            //find controls to be detached
            ControlSet detachedControls = new ControlSet();
            foreach (DependencyObject control in this)
            {
                detachedControls.Add(control);
            }

            //detach the controls
            foreach (DependencyObject control in detachedControls)
            {
                DetachControl(control);
            }
        }

        /// <summary>
        /// Detaches/deletes/removes the controls of specified type from the rendered Silverlight output. Controls which are direct children of container controls (like Panel, Grid, StackPanel, WrapPanel, etc) can be detached.
        /// </summary>
        /// <typeparam name="T">Type of the controls to be detached.</typeparam>
        public void DetachByType<T>()
        {
            //find controls to be detached
            ControlSet detachedControls = new ControlSet();
            foreach (DependencyObject control in this)
            {
                if (control is T)
                {
                    detachedControls.Add(control);
                }
            }

            //detach the controls
            foreach (DependencyObject control in detachedControls)
            {
                DetachControl(control);
            }
        }

        /// <summary>
        /// Detaches/deletes/removes the controls of specified types from the rendered Silverlight output. Controls which are direct children of container controls (like Panel, Grid, StackPanel, WrapPanel, etc) can be detached.
        /// </summary>
        /// <param name="types">List of types of the controls to be detached.</param>
        public void DetachByTypes(List<Type> types)
        {
            //find controls to be detached
            ControlSet detachedControls = new ControlSet();
            foreach (DependencyObject control in this)
            {
                if (types.Contains(control.GetType()))
                {
                    detachedControls.Add(control);
                }
            }

            //detach the controls
            foreach (DependencyObject control in detachedControls)
            {
                DetachControl(control);
            }
        }

        /// <summary>
        /// Removes all the children of all container controls in the ControlSet. The container controls are those that are extended from Panel class.
        /// </summary>
        public void Empty()
        {
            foreach (DependencyObject control in this)
            {
                if (control is Panel)
                {
                    ((Panel)control).Children.Clear();
                }
            }
        }

        /// <summary>
        /// Returns the actual width of first control in the ControlSet. Controls extended from FrameworkElement has a width associated with it.
        /// </summary>
        /// <returns>Actual width of the first control; -1 if the first control is not a FrameworkElement.</returns>
        public double Width()
        {
            if (this.Count > 0)
            {
                DependencyObject first = this.First();
                if (first is FrameworkElement)
                {
                    return (((FrameworkElement)first).ActualWidth);
                }
            }

            return (-1);
        }

        /// <summary>
        /// Returns the actual height of first control in the ControlSet. Controls extended from FrameworkElement has a height associated with it.
        /// </summary>
        /// <returns>Actual height of the first control; -1 if the first control is not a FrameworkElement.</returns>
        public double Height()
        {
            if (this.Count > 0)
            {
                DependencyObject first = this.First();
                if (first is FrameworkElement)
                {
                    return (((FrameworkElement)first).ActualHeight);
                }
            }

            return (-1);
        }

        /// <summary>
        /// Returns the Position of first control in the ControlSet. The position is calculated with reference to its immediate parent.
        /// </summary>
        /// <returns>Position of the first control with reference to its immediate parent; default value of Point if the first control has no parent.</returns>
        public Point Position()
        {
            if (this.Count > 0)
            {
                DependencyObject first = this.First();
                if (first is UIElement)
                {
                    DependencyObject parent = VisualTreeHelper.GetParent(first);
                    if (parent is UIElement)
                    {
                        if ((UIElement)parent != null)
                        {
                            GeneralTransform transform = ((UIElement)first).TransformToVisual((UIElement)parent);
                            return (transform.Transform(new Point(0, 0)));
                        }
                    }
                }
            }

            return (default(Point));
        }

        #endregion

        #region Internal-Methods

        private void DoDoubleAnimation(DependencyObject control, DependencyProperty property, int duration, double from, double to, EventHandler callback)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));

            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, control);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(property));
            storyboard.Children.Add(doubleAnimation);

            if (callback != null)
            {
                storyboard.Completed += callback;
            }
            storyboard.Begin();
        }

        private void DetachControl(DependencyObject control)
        {
            if (control is UIElement)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(control);
                if (parent is Panel)
                {
                    ((Panel)parent).Children.Remove((UIElement)control);
                    this.Remove(control);
                }
            }
        }

        private void SaveEvent(DependencyObject control, EventType eventType, Delegate handler)
        {
            DependencyObjectStore.SetData(control, eventType.ToString(), handler);
        }

        private Delegate FindEvent(DependencyObject control, EventType eventType)
        {
            EventInfo eventInfo = typeof(DependencyObject).GetEvent(eventType + "Event");
            return (null);
        }

        private void FireEvent(DependencyObject control, EventType eventType)
        {
            Delegate eventHandler = (Delegate)DependencyObjectStore.GetData(control, eventType.ToString());
            if (eventHandler != null)
            {
                eventHandler.DynamicInvoke(new object[] { control, null });
            }
        }

        private void ValidateEventDelegate(EventType eventType, Delegate handler)
        {
            switch (eventType)
            {
                //validate System.Windows.UIElement events
                case EventType.GotFocus:
                case EventType.LostFocus:
                    if (!(handler is RoutedEventHandler))
                    {
                        throw new InvalidCastException(typeof(RoutedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.KeyUp:
                case EventType.KeyDown:
                    if (!(handler is KeyEventHandler))
                    {
                        throw new InvalidCastException(typeof(KeyEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.MouseEnter:
                case EventType.MouseLeave:
                case EventType.MouseMove:
                case EventType.LostMouseCapture:
                    if (!(handler is MouseEventHandler))
                    {
                        throw new InvalidCastException(typeof(MouseEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.MouseLeftButtonDown:
                case EventType.MouseLeftButtonUp:
                    if (!(handler is MouseButtonEventHandler))
                    {
                        throw new InvalidCastException(typeof(MouseButtonEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.MouseWheel:
                    if (!(handler is MouseWheelEventHandler))
                    {
                        throw new InvalidCastException(typeof(MouseWheelEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;

                //validate System.Windows.FrameworkElement events
                case EventType.BindingValidationError:
                    if (!(handler is EventHandler<ValidationErrorEventArgs>))
                    {
                        throw new InvalidCastException(typeof(EventHandler<ValidationErrorEventArgs>) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.Loaded:
                    if (!(handler is RoutedEventHandler))
                    {
                        throw new InvalidCastException(typeof(RoutedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.LayoutUpdated:
                    if (!(handler is EventHandler))
                    {
                        throw new InvalidCastException(typeof(EventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.SizeChanged:
                    if (!(handler is SizeChangedEventHandler))
                    {
                        throw new InvalidCastException(typeof(SizeChangedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;

                //validate System.Windows.Controls.Control events
                case EventType.IsEnabledChanged:
                    if (!(handler is DependencyPropertyChangedEventHandler))
                    {
                        throw new InvalidCastException(typeof(DependencyPropertyChangedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;

                //validate System.Windows.Controls.TextBox events
                case EventType.TextChanged:
                    if (!(handler is TextChangedEventHandler))
                    {
                        throw new InvalidCastException(typeof(TextChangedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
                case EventType.TextSelectionChanged:
                    if (!(handler is RoutedEventHandler))
                    {
                        throw new InvalidCastException(typeof(RoutedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;

                //validate System.Windows.Controls.Primitives.Selector events
                case EventType.SelectionChanged:
                    if (!(handler is SelectionChangedEventHandler))
                    {
                        throw new InvalidCastException(typeof(SelectionChangedEventHandler) + " is expected for " + eventType + ".");
                    }
                    break;
            }
        }

        #endregion

        #region Static-Methods

        /// <summary>
        /// Creates a new ControlSet with a single control. This method will be useful when methods of ControlSet needs to be accessed for a single control. This will be particularly useful inside ForEach(Action&lt;&gt;) method of ControlSet (similar to jQuery's 'this' behavior inside foreach loop).
        /// </summary>
        /// <param name="control">Dependency object which will be added to the new ControlSet.</param>
        /// <returns>The new ControlSet object created.</returns>
        public static ControlSet Create(DependencyObject control)
        {
            return (new ControlSet() { control });
        }

        internal static bool CheckPropertyValue(object controlValue, object queryValue, FilterType filterType)
        {
            if (controlValue == null || queryValue == null) return (false);

            try
            {
                queryValue = Convert.ChangeType(queryValue, controlValue.GetType(), null);
            }
            catch
            {
            }

            switch (filterType)
            {
                case FilterType.Equals:
                    if (controlValue.Equals(queryValue))
                    {
                        return (true);
                    }
                    break;
                case FilterType.NotEquals:
                    if (!controlValue.Equals(queryValue))
                    {
                        return (true);
                    }
                    break;
                case FilterType.StartsWith:
                    if (controlValue.ToString().StartsWith(queryValue.ToString()))
                    {
                        return (true);
                    }
                    break;
                case FilterType.EndsWith:
                    if (controlValue.ToString().EndsWith(queryValue.ToString()))
                    {
                        return (true);
                    }
                    break;
                case FilterType.Contains:
                    if (controlValue.ToString().Contains(queryValue.ToString()))
                    {
                        return (true);
                    }
                    break;
            }

            return (false);
        }

        #endregion
    }
}
