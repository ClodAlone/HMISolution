using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using UnitConverterManager.Converters;

namespace UnitConverterManager.Controls
{
    public class Renamed : Object
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    /// <summary>
    /// Interaction logic for UnitConverterSelector.xaml
    /// </summary>
    public partial class UnitConverterRenamer : UserControl
    {
        List<Renamed> Columns = new List<Renamed>();
        bool bLoaded;
        public UnitConverterRenamer(IList<String> converters)
        {
            InitializeComponent();
            Loaded += (o, e) =>
                {
                    gridControl.ItemsSource = null;
                    if (bLoaded || converters == null)
                        return;
                    bLoaded = true;
                    converters.ToList().ForEach(x => Columns.Add(new Renamed() { OldValue = x, NewValue = x }));
                    gridControl.ItemsSource = Columns;
                };
        }

        public List<Renamed> GetConverters()
        {
            Columns.ForEach(col =>
            {
                if (TextToVisibilityConverter.IsNotValidName(col.NewValue))
                    col.NewValue = col.OldValue;
            });
            return Columns;
        }
    }
}
