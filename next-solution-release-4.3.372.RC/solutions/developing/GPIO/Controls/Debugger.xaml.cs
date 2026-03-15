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

namespace GPIO.Controls
{
    /// <summary>
    /// Interaction logic for Debugger.xaml
    /// </summary>
    public partial class Debugger : UserControl
    {
        readonly GPIO variables;
        bool bLoaded;

        public Debugger(GPIO v)
        {
            InitializeComponent();
            variables = v;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    variables.CurrentDocument.ListPIN.ForEach(settings =>
                    {
                        var variable = variables.GetVariable(settings.Name);
                        if (variable != null)
                        {
                            if (settings.Type == Settings.GPIOType.input)
                            {
                                var item = TryFindResource("inputControl") as DataTemplate;
                                var view = item.LoadContent() as FrameworkElement;
                                view.DataContext = variable;
                                panelInputs.Children.Add(view);
                            }
                            else
                            {
                                var item = TryFindResource("outputControl") as DataTemplate;
                                var view = item.LoadContent() as FrameworkElement;
                                view.DataContext = variable;
                                panelOutputs.Children.Add(view);
                            }
                        }
                    });
                }
            };
        }
    }
}
