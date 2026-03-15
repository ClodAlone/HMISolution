using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;
using System.Windows;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace ScreenManager.ComponentService
{
    public interface IScreenManager : IUFInterfaceBase
    {
#if !WINDOWS_UWP && !NET_STANDARD
        Uri CreateNewTemplatedDocument(Uri relative, IDocument parent, bool encryptFile = false);

        UserControl GetNewScreenTemplateControl();
        UserControl GetNewScreenTemplateControl(Uri uri);
        bool OpenMap(IDocument Parent, ExecutionMode mode, double X, double Y, bool IsRelative, 
            IEntityReference Entity, Rect zoomTo, bool enableZoomingScrolling, 
            bool showMiniMap, bool showNextButton);
        void UpdateStringId(UIElement element, bool bAlreadyUntranslated, bool bClearTexts, bool bClearTooltips);
        bool SaveToSvg(UFInterfaces.Editors.SvgModel model, Color tileColor);
#endif
    }
}
