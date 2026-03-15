using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using UFRecipeLayout.LayoutItemControls;

namespace UFRecipeLayout.Automations
{
    internal class IndexAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region  Declarations
        readonly IRecipeIndexUI itemControl;
        #endregion

        #region Constructors

        public IndexAutomationPeer(FrameworkElement owner)
            : base(owner)
        {
            itemControl = owner as IRecipeIndexUI;
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
            return AutomationControlType.ComboBox;
        }

        #region IValueProvider Members

        public bool IsReadOnly
        {
            get 
            {
                return true;
            }
        }

        public void SetValue(string value)
        {
            itemControl.TextValue = value;
        }

        public string Value
        {
            get
            {
                return itemControl.TextValue;
            }
        }

        #endregion
    }
}
