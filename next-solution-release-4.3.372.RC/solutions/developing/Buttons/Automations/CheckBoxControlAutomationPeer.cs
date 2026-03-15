using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using Utilities;

namespace Buttons.Automations
{
    internal class CheckBoxControlAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, IValueProvider
    {
        #region Constructors

        public CheckBoxControlAutomationPeer(CheckBoxControl owner)
            : base(owner)
        {
        }
        
        #endregion

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke || patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        void IInvokeProvider.Invoke()
        {
            if (!this.IsEnabled())
            {
                throw new InvalidOperationException();
            }

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                ((CheckBoxControl)Owner).CheckBoxControlInvokeAction();
            });
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

        bool IValueProvider.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        void IValueProvider.SetValue(string value)
        {
        }

        string IValueProvider.Value
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion    
    }
}
