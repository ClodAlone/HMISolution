using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitConverterManager.ComponentService;
using UnitConverterManager.Document;
using WPFUtilities.PropertyDataTemplate;

namespace UnitConverterManager.Controls
{
    /// <summary>
    /// Interaction logic for NewConverterItem.xaml
    /// </summary>
    public partial class NewConverterItem : UserControl
    {
        #region ConverterItem
        public static readonly DependencyProperty ConverterItemProperty = DependencyProperty.Register("ConverterItem", typeof(UnitConverterModel.UFConverterItem), typeof(NewConverter), new UIPropertyMetadata(null));
        public UnitConverterModel.UFConverterItem ConverterItem
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (UnitConverterModel.UFConverterItem)GetValue(ConverterItemProperty);
            }
            set
            {
                SetValue(ConverterItemProperty, value);
            }
        }

        #endregion
        public NewConverterItem(UnitConverterModel.UFConverterItem converterItem)
        {
            InitializeComponent();
            ConverterItem = converterItem;
            Loaded += (o, e) =>
            {
                if (ConverterItem != null)
                {
                    PropertyReferenceModel InputExpression = new PropertyReferenceModel() { Value = ConverterItem.InputExpression };
                    txtConverterIExpression.Workspace = UnitConverterEditorManagerComponent.unitConverterEditorManager.Workspace;
                    txtConverterIExpression.DataContext = InputExpression;

                    PropertyReferenceModel OutputExpression = new PropertyReferenceModel() { Value = ConverterItem.OutputExpression };
                    txtConverterOExpression.Workspace = UnitConverterEditorManagerComponent.unitConverterEditorManager.Workspace;
                    txtConverterOExpression.DataContext = OutputExpression;

                    InputExpression.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (InputExpression.Value != null)
                                ConverterItem.InputExpression = InputExpression.Value;
                            else
                                ConverterItem.InputExpression = null;
                        }
                    };

                    OutputExpression.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (OutputExpression.Value != null)
                                ConverterItem.OutputExpression = OutputExpression.Value;
                            else
                                ConverterItem.OutputExpression = null;
                        }
                    };

                }
            };
        }
    }
}
