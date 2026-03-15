using System;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UFRecipeLayout.LayoutItemControls
{
    public interface ICommandUI
    {
        void SetBinding(ICommand command);
        void SetCaption(String text);
        void SetIcon(BitmapImage bmp);
        void ShowIcon(bool showicon);
        void SetToolTip(String text);
    }
}
