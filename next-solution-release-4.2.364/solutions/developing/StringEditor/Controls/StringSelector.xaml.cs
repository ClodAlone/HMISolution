using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;

namespace StringManager.Controls
{
    /// <summary>
    /// Interaction logic for StringSelector.xaml
    /// </summary>
    public partial class StringSelector : UserControl
    {
        string[] Columns = null;
        List<ExpandoObject> Words = null;
        public StringSelector(string[] cols, List<object> addlist)
        {
            Columns = cols;

            Words = new List<ExpandoObject>(); 

            foreach(var ob in addlist)
            {
                ExpandoObject dynObject = new ExpandoObject();
                var st = ob as string[];
                if (st != null && st.Length >= Columns.Length)
                {
                    var p = dynObject as IDictionary<String, object>;
                    p["ID"] = st[0];
                    for (int i = 1; i < Columns.Length; i++)
                    {
                        p[Columns[i]] = st[i];    
                    }
                    Words.Add(dynObject);
                }
                
            }
            InitializeComponent();
            Loaded += (o, e) =>
                {
                    for (int i = 1; i < Columns.Length; i++)
                    {
                        var column = new GridColumn() { Header = Columns[i], FieldName = Columns[i] };
                        gridDataControl.Columns.Add(column);
                    }
                    gridDataControl.ItemsSource = null;
                    gridDataControl.ItemsSource = Words;
                };
        }
    }
}
