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

namespace Meters.Automations
{
    internal class FastLinearMeterAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, IValueProvider
    {
        #region Constructors

        public FastLinearMeterAutomationPeer(FastLinearMeter owner)
            : base(owner)
        {
        }
        public FastLinearMeterAutomationPeer(FastLinearMeterControl owner)
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
                if (Owner is FastLinearMeterControl)
                    ((FastLinearMeterControl)Owner).FastLinearMeterInvokeAction();
                else if (Owner is FastLinearMeter)
                    ((FastLinearMeter)Owner).FastLinearMeterInvokeAction();
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
                if (Owner is FastLinearMeterControl)
                {
                    return !((FastLinearMeterControl)Owner).IsEnabled || (!((FastLinearMeterControl)Owner).MarkerIsInteractive && !((FastLinearMeterControl)Owner).LevelIsInteractive && !((FastLinearMeterControl)Owner).RangeBarIsInteractive);
                }
                else if (Owner is FastLinearMeter)
                {
                    return !((FastLinearMeter)Owner).IsEnabled || (!((FastLinearMeter)Owner).MarkerIsInteractive && !((FastLinearMeter)Owner).LevelIsInteractive && !((FastLinearMeter)Owner).RangeBarIsInteractive);
                }
                else
                {
                    return true;
                } 
            }
        }

        void IValueProvider.SetValue(string value)
        {
            if (Owner is FastLinearMeterControl)
            {
                ((FastLinearMeterControl)Owner).Value = Convert.ToDouble(value,CultureInfo.CurrentCulture);
            }
            else if (Owner is FastLinearMeter)
            {
                ((FastLinearMeter)Owner).Value = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
        }

        string IValueProvider.Value
        {
            get
            {
                if (Owner is FastLinearMeterControl)
                {
                    return ((FastLinearMeterControl)Owner).Value.ToString(CultureInfo.CurrentCulture);
                }
                else if (Owner is FastLinearMeter)
                {
                    return ((FastLinearMeter)Owner).Value.ToString(CultureInfo.CurrentCulture);
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
