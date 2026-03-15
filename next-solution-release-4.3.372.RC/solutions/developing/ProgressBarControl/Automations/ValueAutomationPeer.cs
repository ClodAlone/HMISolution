using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;


namespace ProgressBarControl.Automations
{
    internal class ValueAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region  Declarations
        readonly ProgressBarControl itemControl;
        #endregion
        
        #region Constructors

        public ValueAutomationPeer(FrameworkElement owner)
            : base(owner)
        {
            itemControl = owner as ProgressBarControl;
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
                return !itemControl.IsEnabled;
            }
        }

        public void SetValue(string value)
        {
            itemControl.Value = System.Convert.ToDouble(value, CultureInfo.CurrentCulture); 
        }

        public string Value
        {
            get
            {
                return itemControl.Value.ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion
    }
}
