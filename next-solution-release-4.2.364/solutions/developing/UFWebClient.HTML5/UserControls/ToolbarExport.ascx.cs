using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace UFWebClient.HTML5.UserControls
{
    public enum DemoExportFormat { Pdf, Xls, Xlsx, Rtf, Csv }

    public partial class ToolbarExport : UserControl
    {
        Dictionary<DemoExportFormat, string> itemIcons;
        DemoExportFormat[] exportItemTypes;
        static readonly object EventItemClick = new object();
        public delegate void ExportItemClickEventHandler(object source, ExportItemClickEventArgs e);

        [TypeConverter(typeof(EnumConverter))]
        public DemoExportFormat[] ExportItemTypes
        {
            get
            {
                if (exportItemTypes == null)
                    exportItemTypes = Enum.GetValues(typeof(DemoExportFormat)).Cast<DemoExportFormat>().ToArray();
                return exportItemTypes;
            }
            set { exportItemTypes = value; }
        }
        public bool IsDataAwareXls { get; set; }
        public bool IsDataAwareXlsx { get; set; }
        public event ExportItemClickEventHandler ItemClick
        {
            add { Events.AddHandler(EventItemClick, value); }
            remove { Events.RemoveHandler(EventItemClick, value); }
        }
        Dictionary<DemoExportFormat, string> ItemIcons
        {
            get
            {
                if (itemIcons == null)
                {
                    itemIcons = new Dictionary<DemoExportFormat, string>();
                    FillItemIcons();
                }
                return itemIcons;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            foreach (var type in ExportItemTypes)
                CreateMenuItem(type);
        }
        void CreateMenuItem(DemoExportFormat type)
        {
            var item = new MenuItem(string.Empty, type.ToString());
            MenuExportButtons.Items.Add(item);
            if (ItemIcons.ContainsKey(type))
                item.Image.IconID = ItemIcons[type];
            item.ToolTip = GetItemToolTip(type);
        }
        string GetItemToolTip(DemoExportFormat type)
        {
            var result = "Export to " + type.ToString();
            if ((IsDataAwareXls && type == DemoExportFormat.Xls) || (IsDataAwareXlsx && type == DemoExportFormat.Xlsx))
                result += " (DataAware)";
            return result;
        }
        void FillItemIcons()
        {
            ItemIcons[DemoExportFormat.Pdf] = "export_exporttopdf_32x32";
            ItemIcons[DemoExportFormat.Xls] = "export_exporttoxls_32x32";
            ItemIcons[DemoExportFormat.Xlsx] = "export_exporttoxlsx_32x32";
            ItemIcons[DemoExportFormat.Rtf] = "export_exporttortf_32x32";
            ItemIcons[DemoExportFormat.Csv] = "export_exporttocsv_32x32";
        }
        protected void MenuExportButtons_ItemClick(object source, MenuItemEventArgs e)
        {
            var handler = (ExportItemClickEventHandler)Events[EventItemClick];
            if (handler != null)
                handler(this, new ExportItemClickEventArgs((DemoExportFormat)Enum.Parse(typeof(DemoExportFormat), e.Item.Name)));
        }
    }
    public class ItemTooltips : Collection<ItemTooltip>
    {
        public ItemTooltips()
            : base()
        {
        }
    }
    public class ItemTooltip : CollectionItem
    {
        public ItemTooltip()
        {
        }
        public ItemTooltip(DemoExportFormat type, string toolTip)
        {
            Type = type;
            ToolTip = toolTip;
        }
        public DemoExportFormat Type { get; set; }
        public string ToolTip { get; set; }
    }

    public class ExportItemClickEventArgs : EventArgs
    {
        public ExportItemClickEventArgs(DemoExportFormat type)
        {
            ExportType = type;
        }
        public DemoExportFormat ExportType { get; set; }
    }
    public class EnumConverter : StringToObjectTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var items = value.ToString().Split(',');
            var result = new DemoExportFormat[items.Length];
            for (var i = 0; i < items.Length; ++i)
                Enum.TryParse(items[i], out result[i]);
            return result;
        }
    }
}