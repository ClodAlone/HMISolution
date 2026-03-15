using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using UFInterfaces;
using Utilities.Converters;

namespace WPFUtilities
{
    internal class ComboValueAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region  Declarations
        readonly ComboBox itemControl;
        readonly string itemPropertyName;
        #endregion
        
        #region Constructors

        public ComboValueAutomationPeer(FrameworkElement owner, String itemPropName = null)
            : base(owner)
        {
            itemControl = owner as ComboBox;
            itemPropertyName = itemPropName;
        }

        #endregion

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().FullName;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

        #region IValueProvider Members

        public bool IsReadOnly
        {
            get 
            {
                return itemControl.IsReadOnly;
            }
        }

        public void SetValue(string value)
        {
            if (itemControl.ItemsSource != null)
            {
                foreach (var item in itemControl.ItemsSource)
                {
                    if (ItemToString(item) == value)
                    {
                        itemControl.SelectedItem = item;
                        break;
                    }
                }
            }
            else if (itemControl.Items.Count > 0)
            {
                foreach (var item in itemControl.Items)
                {
                    if (ItemToString((item as ComboBoxItem)?.Content) == value)
                    {
                        itemControl.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public string Value
        {
            get
            {
                //return itemControl.SelectedValue.ToString();
                var finalString = ListSerializationHelper.tagListSelection;
                if (itemControl.ItemsSource != null)
                    foreach (var item in itemControl.ItemsSource)
                        finalString = String.Format("{0}{1}{2}", finalString, ItemToString(item), ListSerializationHelper.tagSelectionSeparator);
                else if (itemControl.Items.Count > 0)
                    foreach (var item in itemControl.Items)
                        finalString = String.Format("{0}{1}{2}", finalString, ItemToString((item as ComboBoxItem)?.Content), ListSerializationHelper.tagSelectionSeparator);
                finalString = finalString.Remove(finalString.Length - 1);
                return finalString;
            }
        }

        private string ItemToString(object item)
        {
            string itemString = String.Empty;
            if (itemPropertyName != null)
            {
                try
                {
                    ResourceEnumConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(item.GetType()) as ResourceEnumConverter;
                    if(converter != null)
                        itemString = converter.ConvertTo(item, typeof(string)) as String;
                    else
                        itemString = item.GetType().GetProperty(itemPropertyName).GetValue(item, null) as String;
                }
                catch { }
            }
            else
                itemString = item.ToString();
            return itemString ?? String.Empty;
        }
        #endregion
    }
}
