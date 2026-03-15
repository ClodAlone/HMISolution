using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace PropertyControl.ComponentService
{
    public interface IPropertyControl : IUFInterfaceBase
    {
        event EventHandler Selecting;
        event EventHandler Selected;
        event EventHandler PrepareChanges;
        event EventHandler AcceptChanges;
        event EventHandler CancelChanges;

        Object SelectObject { get; set; }
        IList SelectObjects { get; set; }

        UserControl control { get; }
        UserControl controlNoSelection { get; }

        UserControl controlNoSelectionPriority { get; }

        void SetControlSelection(UserControl control, Object select);
        void UpdateControlSelection();

        void AddPropertyEditor(Type editedType, DataTemplate editorTemplate);
        void AddPropertyEditor(Type editedType, Type declaringType, DataTemplate editorTemplate);
        void AddPropertyEditor(String propertyName, Type editedType, Type declaringType, DataTemplate editorTemplate);
        void AddPropertyEditor(String propertyName, Type editedType, Type declaringType, Type documentType, DataTemplate editorTemplate, bool useBaseClass = false);
        IList PropertyEditors { get; }
        PropertyDescriptor GetLocalizedPropertyDescriptor(DependencyProperty dependencyProperty, Type targetType);

        void Activate();
    }
}
