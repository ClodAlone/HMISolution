using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using UFRecipeLayout.Automations;
using UFRecipeLayout.LayoutItemControls;

namespace UFRecipeLayout
{
    public class RecipeLayoutControl : LayoutControl
    {
        #region Override Methods
        protected override PanelControllerBase CreateController()
        {
            return new RecipeLayoutControlController(this);
        }

        protected override Type GetGroupType()
        {
            return typeof(RecipeLayoutGroup);
        }

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }

        //public void InvokeAction()
        //{
        //    //TODO: handle some operations over this object
        //}
        #endregion

        #endregion
    }

    public class RecipeLayoutControlController : LayoutControlController
    {
        public RecipeLayoutControlController(ILayoutControl control) : base(control) { }

        public override void DropElement(FrameworkElement element, LayoutItemInsertionPoint insertionPoint, LayoutItemInsertionKind insertionKind)
        {
            var customizationController = CustomizationController as RecipeLayoutControlCustomizationController;
            if (customizationController != null && customizationController.CustomControl != null)
            {
                if (customizationController.CustomControl.AllowDrag)
                {
                    base.DropElement(element, insertionPoint, insertionKind);
                }
                else
                {
                    base.DropElement(element, new LayoutItemInsertionPoint(this.Control, true), LayoutItemInsertionKind.Bottom);
                    base.Scroll(System.Windows.Controls.Orientation.Vertical, element.GetPosition(this.Control).Y);
                }
            }
        }

        protected override LayoutControlCustomizationController CreateCustomizationController()
        {
            return new RecipeLayoutControlCustomizationController(this);
        }
    }

    public class RecipeLayoutControlCustomizationController : LayoutControlCustomizationController
    {
        public RecipeLayoutControlCustomizationControl CustomControl;

        public RecipeLayoutControlCustomizationController(LayoutControlController controller) : base(controller) { }

        protected override LayoutControlCustomizationControl CreateCustomizationControl()
        {
            return new RecipeLayoutControlCustomizationControl();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            CustomControl = CustomizationControl as RecipeLayoutControlCustomizationControl;
        }
    }

    public class RecipeLayoutControlCustomizationControl : LayoutControlCustomizationControl
    {
        public bool AllowDrag;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public RecipeLayoutControlCustomizationControl()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            AllowDrag = true;
            dispatcherTimer.Stop();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (LayoutHelper.FindParentObject<LayoutControlAvailableListBoxItem>(e.OriginalSource as DependencyObject) != null)
            {
                AllowDrag = false;
                dispatcherTimer.Start();
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }
    }
}
