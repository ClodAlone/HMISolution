using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using UFInterfaces;

namespace CommandManager.Executer
{
    public class EntityWeakReference : IEntityReference
    {
        #region IEntityReference Members
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
