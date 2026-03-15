using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;

namespace EditDisplay.Automations
{
    internal class ValueAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region Constructors

        public ValueAutomationPeer(EditDisplay owner)
            : base(owner)
        {
        }

        public ValueAutomationPeer(DateEditDisplay owner)
            : base(owner)
        {
        }

        public ValueAutomationPeer(CalendarEditDisplay owner)
            : base(owner)
        {
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
            if (Owner is CalendarEditDisplay || Owner is DateEditDisplay || Owner is EditDisplay)
                return AutomationControlType.Custom;
            else
                return AutomationControlType.Text;
        }

        #region IValueProvider Members

        public bool IsReadOnly
        {
            get 
            {
                if (Owner is CalendarEditDisplay)
                    return !(Owner as CalendarEditDisplay).IsReadOnly;
                else if (Owner is DateEditDisplay)
                    return !(Owner as DateEditDisplay).IsReadOnly;
                else if (Owner is EditDisplay)
                    return (Owner as EditDisplay).IsReadOnly;
                else
                    return true;
            }
        }

        public void SetValue(string value)
        {
            if (Owner is CalendarEditDisplay)
                (Owner as CalendarEditDisplay).SetDisplayValue(value);
            else if (Owner is DateEditDisplay)
                (Owner as DateEditDisplay).SetDisplayValue(value);
            else if (Owner is EditDisplay)
                (Owner as EditDisplay).SetDisplayValue(value);
        }

        public string Value
        {
            get
            {
                if (Owner is CalendarEditDisplay)
                    return (Owner as CalendarEditDisplay).GetDisplayValue();
                else if (Owner is DateEditDisplay)
                    return (Owner as DateEditDisplay).GetDisplayValue();
                else if (Owner is EditDisplay)
                    return (Owner as EditDisplay).GetDisplayValue();
                else
                    return string.Empty;
            }
        }
        #endregion
    }
}
