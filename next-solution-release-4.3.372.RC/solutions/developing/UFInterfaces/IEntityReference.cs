using System;
using System.Collections.Generic;
using System.Text;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
#else
public class ImageSource
{
}
public class ContextMenu
{
}
#endif

namespace UFInterfaces
{
    public interface IEntityReference
    {
        ImageSource CollapsedImageSource { get; }
        ImageSource ExpandedImageSource { get; }

        ContextMenu contextMenu { get; }
        Object Tooltip { get; }

        Object ContainedObject { get; }
        Object EntityParent { get; }

        String TypeDefinitionString { get; }
    }
}
