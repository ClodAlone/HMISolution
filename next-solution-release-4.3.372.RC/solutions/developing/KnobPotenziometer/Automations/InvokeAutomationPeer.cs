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

namespace KnobPotenziometer.Automations
{
    internal class InvokeAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, IValueProvider
    {
        #region Constructors

        public InvokeAutomationPeer(KnobExpress owner)
            : base(owner)
        {
        }

        public InvokeAutomationPeer(KnobExpressPotenziometer owner)
            : base(owner)
        {
        }

        public InvokeAutomationPeer(KnobPotenziometer owner)
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

            if (Owner is KnobExpressPotenziometer)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    ((KnobExpressPotenziometer)Owner).KnobExpressPotenziometerInvokeAction();
                });
            }
            else if (Owner is KnobExpress)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    ((KnobExpress)Owner).KnobExpressInvokeAction();
                });
            }
            else
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    ((KnobPotenziometer)Owner).KnobPotenziometerInvokeAction();
                });
            }
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
                if (Owner is KnobExpressPotenziometer)
                {
                    return !((KnobExpressPotenziometer)Owner).IsEnabled;
                }
                else if(Owner is KnobExpress)
                {
                    return !((KnobExpress)Owner).IsEnabled;
                }
                else
                {
                    return !((KnobPotenziometer)Owner).IsEnabled;
                } 
            }
        }

        void IValueProvider.SetValue(string value)
        {
            if (Owner is KnobExpressPotenziometer)
            {
                ((KnobExpressPotenziometer)Owner).Value = Convert.ToDouble(value,CultureInfo.CurrentCulture);
            }
            else if (Owner is KnobExpress)
            {
                ((KnobExpress)Owner).Value = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
            else
            {
                ((KnobPotenziometer)Owner).Value = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
        }

        string IValueProvider.Value
        {
            get
            {
                if (Owner is KnobExpressPotenziometer)
                {
                    return ((KnobExpressPotenziometer)Owner).Value.ToString(CultureInfo.CurrentCulture);
                }
                else if (Owner is KnobExpress)
                {
                    return ((KnobExpress)Owner).Value.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    return ((KnobPotenziometer)Owner).Value.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        #endregion
    }
}
