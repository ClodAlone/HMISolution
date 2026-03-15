using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUtilities.PropertyDataTemplate
{
    public class BitMaskLevelPropertyEditor : BitMaskPropertyEditor
    {
        public BitMaskLevelPropertyEditor()
        {
            bType = BitMaskEditor.BitMaskType.Level;
            InitializeComponent();
        }
    }
}
