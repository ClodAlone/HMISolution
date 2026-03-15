using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFUtilities.Converters;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TextDefaultPropertyEditor.xaml
    /// </summary>
    public partial class TextDefaultPropertyEditor : UserControl, INotifyPropertyChanged
    {
        #region Declarations
        DependencyObject depObject;
        DependencyPropertyDescriptor dpd;
        object initialValue;
        #endregion

        #region Constructors
        public TextDefaultPropertyEditor()
        {
            InitializeComponent();

            DataContextChanged += (o, e) => 
            {
                if (e.NewValue != null && e.NewValue is Mindscape.WpfElements.PropertyEditing.ObjectWrapper)
                {
                    var wrapper = e.NewValue as Mindscape.WpfElements.PropertyEditing.ObjectWrapper;
                    if (wrapper.UnderlyingObject is ICustomTypeDescriptor)
                    {
                        var typeDesc = wrapper.UnderlyingObject as ICustomTypeDescriptor;
                        depObject = typeDesc.GetPropertyOwner(wrapper.Property.Property.AsPropertyDescriptor) as DependencyObject;
                    }
                    else if (wrapper.UnderlyingObject is DependencyObject)
                        depObject = wrapper.UnderlyingObject as DependencyObject;

                    if (depObject != null)
                    {
                        dpd = DependencyPropertyDescriptor.FromProperty(wrapper.Property.Property.AsPropertyDescriptor);
                    }

                    initialValue = wrapper.Property.Value;
                }
                else
                {
                    depObject = null;
                    dpd = null;
                    initialValue = null;
                }
            };
        }
        #endregion

        #region Events Handler
        void unsetValueBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (depObject != null && dpd != null)
            {
                var value = depObject.ReadLocalValue(dpd.DependencyProperty);
                if (initialValue != null && !object.Equals(value, initialValue))
                    dpd.SetValue(depObject, initialValue);
                else if (dpd.DependencyProperty.DefaultMetadata.DefaultValue != null)
                {
                    dpd.SetValue(depObject, DependencyProperty.UnsetValue);
                    initialValue = null;
                }

                text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
            else if (DataContext != null)
            {
                PropertyInfo prop = DataContext.GetType().GetProperty("Value");
                if (prop != null)
                { 
                    prop.SetValue(DataContext, initialValue, null);
                    text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            }
        }

        void text_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                text.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }

        #endregion
    }
}
