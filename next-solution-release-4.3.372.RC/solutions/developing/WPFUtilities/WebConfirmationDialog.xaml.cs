using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace WPFUtilities
{
    /// <summary>
    /// Interaction logic for WebConfirmationDialog.xaml
    /// </summary>
    public partial class WebConfirmationDialog : UserControl, IDisposable
    {
        #region Properties
        public string Message {
            get
            {
                return webDialogTxt.Text;
            }
        }
        public bool IsShown
        {
            get { return bShown; }
        }
        #endregion

        #region Declarations
        #region Public Events
        public event EventHandler WebDialogYesClicked;
        public event EventHandler WebDialogNoClicked;
        #endregion
        #region Properties
        public DialogActionType ActionType { get; set; }
        #endregion
        public enum DialogActionType {
            None,
            DeleteItems,
            AddHolidays
        }
        object actionItems;
        bool bShown;
        bool bDisposed;
        #endregion

        #region ctor
        public WebConfirmationDialog()
        {
            InitializeComponent();
        }
        #endregion
        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new WebDialogAutomationPeer(this);
        }
        #endregion
        #region methods
        public bool Show(string message, object actionItems = null, DialogActionType actionType = DialogActionType.None)
        {
            if (bShown)
                Hide();
            bShown = true;
            webDialogTxt.Text = message;
            if (actionItems != null)
                this.actionItems = actionItems;
            ActionType = actionType;
            Visibility = Visibility.Visible;
            return true;
        }

        void Hide()
        {
            ClearActionInfo();
            Visibility = Visibility.Collapsed;
            bShown = false;
        }

        void ClearActionInfo()
        {
            ActionType = DialogActionType.None;
            (actionItems as IDisposable)?.Dispose();
            actionItems = null;
        }

        void WebDialogYes_Click(object sender, RoutedEventArgs e)
        {
            WebDialogYesClicked?.Invoke(this, new ConfirmationEventArgs() { ActionItems = actionItems });
            Hide();
        }

        void WebDialogNo_Click(object sender, RoutedEventArgs e)
        {
            WebDialogNoClicked?.Invoke(this, new ConfirmationEventArgs() { ActionItems = actionItems });
            Hide();
        }
        #endregion
        #region IDisposable
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            ClearActionInfo();
        }
        #endregion
    }

    public class ConfirmationEventArgs : EventArgs
    {
        public object ActionItems {
            get; set;
        }
    }

    internal class WebDialogAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        #region Constructors
        public WebDialogAutomationPeer(WebConfirmationDialog owner)
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

            // Asynchronous call of InvokeAction
            // we don't want to block this thread
            //Dispatcher.BeginInvoke(
            //    DispatcherPriority.Input,
            //    new DispatcherOperationCallback(delegate
            //    {
            //        ((WebConfirmationDialog)Owner).WebDialogInvokeAction();
            //        return null;
            //    }),
            //    null);
        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().FullName;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }
    }
}
