using DevExpress.Xpf.Editors;
using System;
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for TextEditWeb.xaml
    /// </summary>
    public partial class WebDateEdit : TextEdit
    {
        public WebDateEdit()
        {
            InitializeComponent();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
    }

    internal class ValueAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        #region Constructors

        public ValueAutomationPeer(WebDateEdit owner)
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
            //if (Owner is CalendarEditDisplay || Owner is DateEditDisplay || Owner is EditDisplay)
                return AutomationControlType.Custom;
            //else
            //    return AutomationControlType.Text;
        }

        #region IValueProvider Members

        public bool IsReadOnly
        {
            get
            {
                if (Owner is WebDateEdit)
                    return (Owner as WebDateEdit).IsReadOnly;
                else
                    return true;
            }
        }

        public void SetValue(string value)
        {
            if (Owner is WebDateEdit)
            {
                var txtedit = Owner as WebDateEdit;
                DateTime date;
                if (!DateTime.TryParse(value, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out date))
                    txtedit.Text = string.Empty;
                else
                    txtedit.Text = date.ToString(System.Threading.Thread.CurrentThread.CurrentCulture);
            }
        }

        public string Value
        {
            get
            {
                if (Owner is WebDateEdit)
                    return (Owner as WebDateEdit).EditValue.ToString();
                
                return string.Empty;
            }
        }
        #endregion
    }
}
