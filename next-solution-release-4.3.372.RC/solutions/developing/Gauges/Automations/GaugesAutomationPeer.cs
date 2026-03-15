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

namespace Gauges.Automations
{
    internal class GaugesAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, IValueProvider
    {
        #region Constructors

        public GaugesAutomationPeer(GaugeControl owner)
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
                ((GaugeControl)Owner).CircularGaugeInvokeAction();
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
                if (Owner is GaugeControl)
                {
                    return !((GaugeControl)Owner).IsEnabled || (!((GaugeControl)Owner).MarkerIsInteractive && !((GaugeControl)Owner).NeedleIsInteractive && !((GaugeControl)Owner).RangeBarIsInteractive);
                }
                else
                {
                    return true;
                } 
            }
        }

        void IValueProvider.SetValue(string value)
        {
            if (Owner is GaugeControl)
            {
                ((GaugeControl)Owner).Value = Convert.ToDouble(value,CultureInfo.CurrentCulture);
            }
        }

        string IValueProvider.Value
        {
            get
            {
                if (Owner is GaugeControl)
                {
                    return ((GaugeControl)Owner).Value.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion    
    }
}
