using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.PropertyDataTemplate
{
    public class BrushEditorOptions
    {
        public bool ItemIsSymbol{get;set;}
        public bool ItemIsComposed { get; set; }
        public string ItemName { get; set; }
        public bool SpreadOnChild { get; set; }

        public BrushEditorOptions(BrushEditorOptions options)
        {
            this.ItemIsSymbol = ItemIsSymbol;
            this.ItemIsComposed = ItemIsComposed;
            this.ItemName = ItemName;
            this.SpreadOnChild = SpreadOnChild;
        }
        public BrushEditorOptions()
        {
            this.ItemIsSymbol = false;
            this.ItemIsComposed = false;
            this.ItemName = string.Empty;
            this.SpreadOnChild = false;
        }
    }
}
