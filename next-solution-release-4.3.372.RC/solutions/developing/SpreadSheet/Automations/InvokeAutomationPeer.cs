using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;

namespace SpreadSheet.Automations
{
    internal class InvokeAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
    {
        #region Constructors

        public InvokeAutomationPeer(SpreadSheet owner)
            : base(owner)
        {
        }

        #endregion

        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        protected override string GetNameCore()
        {
            return "SpreadSheet";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        #region ISelectionItemProvider Members

        public void AddToSelection()
        {
            throw new NotImplementedException();
        }

        public bool IsSelected
        {
            get
            {
                return false;
            }
        }

        public void RemoveFromSelection()
        {

        }

        public void Select()
        {
            if (this.Owner is SpreadSheet)
            {
                (this.Owner as SpreadSheet).SelectWorkSheet();
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

    }
}
