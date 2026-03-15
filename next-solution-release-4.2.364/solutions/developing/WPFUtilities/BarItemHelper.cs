namespace WPFUtilities.BarItemHelper
{
    public class BarButtonItemEx : DevExpress.Xpf.Bars.BarButtonItem
    {
        public BarButtonItemEx()
        {
            Unloaded += (s, a) =>
            {
                if (Command != null)
                    UnhookCommand(Command);
            };
        }
    }

    public class BarCheckItemEx : DevExpress.Xpf.Bars.BarCheckItem
    {
        public BarCheckItemEx()
        {
            Unloaded += (s, a) =>
            {
                if (Command != null)
                    UnhookCommand(Command);
            };
        }
    }

    public class BarSubItemEx : DevExpress.Xpf.Bars.BarSubItem
    {
        public BarSubItemEx()
        {
            Unloaded += (s, a) =>
            {
                if (Command != null)
                    UnhookCommand(Command);
            };
        }
    }

    public class GalleryItemEx : DevExpress.Xpf.Bars.GalleryItem
    {
        public GalleryItemEx()
        {
            Unloaded += (s, a) =>
            {
                if (Command != null)
                    RequestUnhookCommand(Command);
            };
        }
    }
}
