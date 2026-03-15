using System;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using System.Windows;
using AnimationManagerService.ComponentService;
using PropertyControl.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using OPCUAViewModel.PropertyDataTemplate;

namespace AnimationManager.ComponentService
{
    public class AnimationManagerComponent : ComponentBase<IAnimationManagerService>, IAnimationManagerService
    {
        #region Declaration

        public static IPropertyControl propertyService { get; protected set; }
        public static IWorkspace workspace { get; protected set; }
        public static bool propertyServiceAvailable { get { return propertyService != null; } }

        #endregion
        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void GetComponentInterfaces()
        {
            if (propertyService == null)
                propertyService = GetService(typeof(IPropertyControl)) as IPropertyControl;
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;

            CreatePropertyDataTemplates();
        }
        void CreatePropertyDataTemplates()
        {
            if (propertyService == null)
                return;

            var dt = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(ExpressionPropertyEditor));
            factory.SetValue(ExpressionPropertyEditor.WorkspaceProperty, workspace); 
            dt.DataType = typeof(string);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("Expression", typeof(String), typeof(AnimationManager), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(StrokeDashArrayPropertyEditor));
            dt.DataType = typeof(System.Windows.Media.DoubleCollection);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("StrokeDashArray", typeof(System.Windows.Media.DoubleCollection), typeof(BorderAnimation), dt);
        }
    }
}
