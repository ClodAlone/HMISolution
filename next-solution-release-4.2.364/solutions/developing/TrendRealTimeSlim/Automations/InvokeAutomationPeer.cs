using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using Utilities;

namespace TrendRealTimeSlim.Automations
{
    internal class InvokeAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        #region Constructors

        public InvokeAutomationPeer(RealTimeTrendControl owner)
            : base(owner)
        {
        }

        #endregion

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
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
                ((RealTimeTrendControl)Owner).RealTimeTrendControlInvokeAction();
            });
        }

        protected override string GetClassNameCore()
        {
            return "RealTimeSDataValue";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }
    }
}
