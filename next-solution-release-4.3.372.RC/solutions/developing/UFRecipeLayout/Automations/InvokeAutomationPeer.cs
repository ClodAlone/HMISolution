using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using UFRecipeLayout.LayoutItemControls;
using Utilities;

namespace UFRecipeLayout.Automations
{
    internal class InvokeAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        #region Constructors

        public InvokeAutomationPeer(RecipeLayoutControl owner)
            : base(owner)
        { }

        #endregion

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return "RecipeLayoutControl";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        #region IInvokeProvider Members

        void IInvokeProvider.Invoke()
        {
            if (!this.IsEnabled())
            {
                throw new InvalidOperationException();
            }
            
            // Asynchronous call of InvokeAction we don't want to block this thread
            //Owner.Dispatcher.BeginInvokeAsynchronously(() => ((RecipeLayoutControl)Owner).InvokeAction());
        }

        #endregion
    }
}
