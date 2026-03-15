using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using DevExpress.Web;
using System.IO;
using DevExpress.Web.Data;

namespace UFUAWebEditor
{
    public partial class DriversList : System.Web.UI.UserControl
    {
        readonly Session session = XpoHelper.GetNewSession();

        protected void Page_Load(object sender, EventArgs e)
        {
            XpoDataSource1.Session =  XpoHelper.GetNewSession();
        }

        protected void ASPxGridView1_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType != GridViewRowType.Data)
                return;

            var drivers = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(XpoDataSource1.Session)/*.AsParallel()*/
                select tag).ToList();

            if (drivers.Count > 0)
            {
                var key = e.GetValue(ASPxGridView1.KeyFieldName).ToString();
                var drv = (from t in drivers[0].ComunicationDrivers where t.Name == Path.GetFileNameWithoutExtension(key) select t).ToList();
                if (drv.Count > 0)
                    ASPxGridView1.Selection.SelectRow(e.VisibleIndex);
            }
        }

        protected void ASPxGridView1_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            ASPxComboBox comboBox = e.Editor as ASPxComboBox;
            if (comboBox != null && e.Column.FieldName == "Free")
            {
                comboBox.Items.Add("");
                comboBox.Items.Add("yes");
                comboBox.Items.Add("no");
            }
        }
    }
}