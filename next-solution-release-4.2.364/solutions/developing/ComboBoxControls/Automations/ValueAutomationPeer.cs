using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;


namespace ComboBoxControls.Automations
{
    internal class ValueAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region  Declarations
        readonly ComboControl itemControl;
        #endregion
        
        #region Constructors

        public ValueAutomationPeer(FrameworkElement owner)
            : base(owner)
        {
            itemControl = owner as ComboControl;
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
            itemControl.Value = value;
        }

        public string Value
        {
            get
            {
                return itemControl.GetIndexValue();
            }
        }

        #endregion
    }
}
